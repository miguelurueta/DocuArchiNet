import React from "react";
import type Claim from "./Claim";
const AutenticacionContext= React.createContext<autenticacionContextparrams>({claims:[], actualizar:()=>{}})
interface autenticacionContextparrams{
    claims:Claim[];
    actualizar(claims : Claim[]): void;
}
export  default AutenticacionContext