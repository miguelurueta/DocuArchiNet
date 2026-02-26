export interface CDeRelacionEstadoRetriccionDto {
  IdRestriTipoDestInterno: number;
  IdTipoRestriccion: number;
  DescripcionTipo: string;
  MoluloRadicacion: number;
  ModuloRadicacionSimple: number;
  ModuloRadicacionInterna: number;
}

export const C_DE_RELACION_ESTADO_RETRICCION_DESTINATARIO_DEFAULT: CDeRelacionEstadoRetriccionDto =
  {
    IdRestriTipoDestInterno: 0,
    IdTipoRestriccion: 0,
    DescripcionTipo: "string",
    MoluloRadicacion: 0,
    ModuloRadicacionSimple: 0,
    ModuloRadicacionInterna: 0,
  };
