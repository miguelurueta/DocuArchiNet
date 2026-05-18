// import type React from "react";
// import { useContext, useEffect, useState } from "react";
// import AutenticacionContext from "./AutenticacionContext";
// export default function Autorizado( props:autorizadoProps ){
//     const [autorizado, setAutorizado]=useState(false);
//     const {claims}= useContext(AutenticacionContext);
//   useEffect(() => {
//   if (props.claims) {
//     for (let i = 0; i < props.claims.length; i++) {
//       const claim = props.claims[i];
//       const indiceClaim = claims.findIndex(c => c.nombre === claim);
//       if (indiceClaim > -1) {
//         setAutorizado(true);
//         return;
//       }
//     }
//     setAutorizado(false);
//   } else {
//     setAutorizado(claims.length > 0);
//   }
// }, [claims, props.claims]);

//     return(
//         <>
//          {autorizado? props.autorizado: props.noAutorizado}
//         </>
//     )
// }
//  interface autorizadoProps{
//     autorizado:React.ReactNode;
//     noAutorizado:React.ReactNode;
//     claims:string [];
//  }

// Autorizado.tsx
import type React from "react";
import { useContext, useMemo } from "react";
import AutenticacionContext from "../Estado/AutenticacionContext";
import { hasPermissionClaim } from "../Infraestructura/authClaimsAdapter";
import { sesionValida } from "../Infraestructura/ManejadorJWT";

interface AutorizadoProps {
  autorizado: React.ReactNode;
  noAutorizado: React.ReactNode;
  claims?: string[]; // permisos requeridos
}

export default function Autorizado({
  autorizado,
  noAutorizado,
  claims: requeridos,
}: AutorizadoProps) {
  const { claims: userClaims } = useContext(AutenticacionContext);

  const tieneAcceso = useMemo(() => {
    // 1️⃣ No hay sesión válida
    if (!sesionValida()) {
      return false;
    }

    // 2️⃣ Ruta solo requiere autenticación
    if (!requeridos || requeridos.length === 0) {
      return true;
    }

    // 3️⃣ Validación de permisos
    return requeridos.some((req) => hasPermissionClaim(userClaims, req));
  }, [userClaims, requeridos]);

  return <>{tieneAcceso ? autorizado : noAutorizado}</>;
}
