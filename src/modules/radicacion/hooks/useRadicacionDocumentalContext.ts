import { useContext } from "react";
import { RadicacionDocumentalContext } from "../context/radicacionDocumentalContextValue";

export function useRadicacionDocumentalContext() {
  const context = useContext(RadicacionDocumentalContext);

  if (!context) {
    throw new Error(
      "useRadicacionDocumentalContext debe usarse dentro de RadicacionDocumentalProvider.",
    );
  }

  return context;
}
