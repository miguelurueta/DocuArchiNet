import type React from "react";
import { useContext, useEffect, useState } from "react";
import AutenticacionContext from "./AutenticacionContext";
export default function Autorizado( props:autorizadoProps ){
    const [autorizado, setAutorizado]=useState(false);
    const {claims}= useContext(AutenticacionContext);
  useEffect(() => {
  if (props.claims) {
    for (let i = 0; i < props.claims.length; i++) {
      const claim = props.claims[i];
      const indiceClaim = claims.findIndex(c => c.nombre === claim);
      if (indiceClaim > -1) {
        setAutorizado(true);
        return;
      }
    }
    setAutorizado(false);
  } else {
    setAutorizado(claims.length > 0);
  }
}, [claims, props.claims]);

    return(
        <>
         {autorizado? props.autorizado: props.noAutorizado}
        </>
    )
}
 interface autorizadoProps{
    autorizado:React.ReactNode;
    noAutorizado:React.ReactNode;
    claims:string [];
 }