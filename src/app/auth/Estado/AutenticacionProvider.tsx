// import React, { useEffect, useState } from "react";
// import type Claim from "./Claim";
// import { obtenerClaims } from "./ManejadorJWT";
// import  AutenticacionContext  from "./AutenticacionContext";
// export function AutenticacionProvider({
//   children,
// }: {
//   children: React.ReactNode;
// }) {
//   const [claims, setClaims] = useState<Claim[]>([]);

//   useEffect(() => {
//     setClaims(obtenerClaims());
//   }, []);

//   function actualizar(nuevosClaims: Claim[]) {
//     //console.log(nuevosClaims);
//     setClaims(nuevosClaims);
//   }

// return (
//     <AutenticacionContext.Provider value={{ claims }}>
//       {children}
//     </AutenticacionContext.Provider>
//   );
// }

// import React, { useEffect, useState } from "react";
// import type Claim from "./Claim";
// import { obtenerClaims } from "./ManejadorJWT";
// import AutenticacionContext from "./AutenticacionContext";

// export function AutenticacionProvider({
//   children,
// }: {
//   children: React.ReactNode;
// }) {
// const [claims, setClaims] = useState<Claim[]>([]);

// const refrescarClaims = () => {
//   const nuevosClaims = obtenerClaims();
//   setClaims(nuevosClaims);
// };

//   // Rehidratar claims al cargar la app
//   useEffect(() => {
//     const actuales = obtenerClaims();
//     setClaims(actuales);
//   }, []);
// // Rehidratación inicial
//   useEffect(() => {
//     refrescarClaims();
//   }, [refrescarClaims]);
//   return (
//     <AutenticacionContext.Provider value={{ claims, refrescarClaims }}>
//       {children}
//     </AutenticacionContext.Provider>
//   );
// }

import React, { useEffect, useCallback, useState } from "react";
import type Claim from "../Dto/Claim";
import { obtenerClaims } from "../Infraestructura/ManejadorJWT";
import AutenticacionContext from "./AutenticacionContext";

export function AutenticacionProvider({
  children,
}: {
  children: React.ReactNode;
}) {
  const [claims, setClaims] = useState<Claim[]>([]);

  // 👇 Función única y estable para refrescar claims
  const refrescarClaims = useCallback(() => {
    const nuevosClaims = obtenerClaims();
    setClaims(nuevosClaims);
  }, []);

  // 👇 Rehidratación inicial (una sola vez)
  useEffect(() => {
    refrescarClaims();
  }, [refrescarClaims]);

  return (
    <AutenticacionContext.Provider value={{ claims, refrescarClaims }}>
      {children}
    </AutenticacionContext.Provider>
  );
}

