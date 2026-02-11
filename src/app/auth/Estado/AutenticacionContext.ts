import { createContext } from "react";
import type Claim from "../Dto/Claim";

export interface AutenticacionContextValue {
  claims: Claim[];
   refrescarClaims: () => void;
}

const AutenticacionContext = createContext<AutenticacionContextValue>({
  claims: [],
  refrescarClaims: () => {},
});

export default AutenticacionContext;
