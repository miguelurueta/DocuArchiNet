import { Alert } from "@mui/material";
import type { ReactNode } from "react";
import { useRadicacionDocumentalContext } from "../hooks/useRadicacionDocumentalContext";

interface RadicacionDocumentosGuardProps {
  children: ReactNode;
  fallback?: ReactNode;
}

export function RadicacionDocumentosGuard({
  children,
  fallback,
}: RadicacionDocumentosGuardProps) {
  const {
    estadoActual,
    requiereGestionDocumental,
    tieneTramiteDocumentalActivoEstado0,
    idEstadoRadicado,
  } = useRadicacionDocumentalContext();

  const canRenderDocumentos =
    estadoActual === 0 &&
    requiereGestionDocumental === true &&
    tieneTramiteDocumentalActivoEstado0 === true &&
    Number(idEstadoRadicado ?? 0) > 0;

  if (!canRenderDocumentos) {
    return (
      fallback ?? (
        <Alert severity="info">
          La captura de documentos estÃ¡ disponible solo para trÃ¡mites
          documentales activos.
        </Alert>
      )
    );
  }

  return <>{children}</>;
}
