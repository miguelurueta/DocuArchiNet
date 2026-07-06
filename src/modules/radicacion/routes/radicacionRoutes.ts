export const RADICACION_ROUTE_SEGMENTS = {
  root: "radicacion",
  registro: "registro",
  documentos: "documentos",
} as const;

export const RADICACION_ROUTES = {
  root: "/dashboard/radicacion",
  registro: (idEstadoRadicado: number | string) =>
    `/dashboard/radicacion/registro/${idEstadoRadicado}`,
  documentos: (idEstadoRadicado: number | string) =>
    `/dashboard/radicacion/registro/${idEstadoRadicado}/documentos`,
} as const;

export const RADICACION_TAB_KEYS = {
  ia: "ia",
  radicacion: "radicacion",
  documentos: "documentos",
  gestionRadicados: "gestion-radicados",
} as const;

export type RadicacionTabKey =
  (typeof RADICACION_TAB_KEYS)[keyof typeof RADICACION_TAB_KEYS];

export const resolveRadicacionTabFromDestino = ({
  destinoPostRegistro,
  documentosDisponibles,
}: {
  destinoPostRegistro?: "resumen" | "documentos";
  documentosDisponibles: boolean;
}): RadicacionTabKey =>
  destinoPostRegistro === "documentos" && documentosDisponibles
    ? RADICACION_TAB_KEYS.documentos
    : RADICACION_TAB_KEYS.radicacion;
