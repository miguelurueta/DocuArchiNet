import type { CampoPlantillaDTO } from "../models/CampoPlantillaDTO";
import type {
  DetallePlantillaRadicadoDTO,
  PlantillaRadicadoDTO,
} from "../models/PlantillaRadicadoDTO";
import type { FlujoRelacionadoOption } from "../hooks/useFlujosRelacionadosTramite";
import { mapCampoDrowlistOptions, normalizeCampoName } from "../utils/radicacionOptionMappers";
import type {
  RegistrarRadicacionCampoDto,
  RegistrarRadicacionEntranteRequestDto,
  RadicacionRegistroFormValue,
  RadicacionRegistroFormValues,
} from "../types/radicacionRegistro.types";

export const RADICACION_TIPO_MODULO_REGISTRO = 1;

interface BuildRegistrarRadicacionEntranteRequestParams {
  values: RadicacionRegistroFormValues;
  camposPlantilla: ReadonlyArray<CampoPlantillaDTO>;
  plantilla: PlantillaRadicadoDTO;
  flujoOptions?: ReadonlyArray<FlujoRelacionadoOption>;
  tipoModuloRadicacion?: number;
}

const toNumber = (value: unknown): number => {
  if (typeof value === "number" && Number.isFinite(value)) {
    return value;
  }

  if (typeof value === "string" && value.trim().length > 0) {
    const parsed = Number(value);
    return Number.isFinite(parsed) ? parsed : 0;
  }

  return 0;
};

const extractFirstValue = (value: RadicacionRegistroFormValue): unknown => {
  if (Array.isArray(value)) {
    return value[0]?.value ?? "";
  }

  if (typeof value === "object" && value !== null && "value" in value) {
    return value.value;
  }

  return value;
};

const extractFirstLabel = (value: RadicacionRegistroFormValue): string => {
  const source = Array.isArray(value) ? value[0]?.label : undefined;
  const label =
    source ??
    (typeof value === "object" && value !== null && "label" in value
      ? value.label
      : undefined);

  if (typeof label === "string" || typeof label === "number") {
    return String(label).trim();
  }

  return "";
};

export const normalizeRegistroString = (
  value: RadicacionRegistroFormValue,
): string => {
  if (
    typeof value === "object" &&
    value !== null &&
    "format" in value &&
    typeof value.format === "function"
  ) {
    return value.format("YYYY-MM-DD");
  }

  const raw = extractFirstValue(value);
  if (raw === null || raw === undefined) {
    return "";
  }

  if (typeof raw === "boolean") {
    return raw ? "true" : "false";
  }

  return String(raw).trim();
};

const getFieldValue = (
  values: RadicacionRegistroFormValues,
  keys: ReadonlyArray<string>,
): RadicacionRegistroFormValue => {
  for (const key of keys) {
    if (key in values) {
      return values[key];
    }
  }

  return "";
};

const normalizeComparableName = (value: string | null | undefined): string =>
  String(value ?? "")
    .normalize("NFD")
    .replace(/[\u0300-\u036f]/g, "")
    .replace(/[^a-zA-Z0-9]/g, "")
    .toUpperCase();

const getEquivalentFormKeys = (campo: CampoPlantillaDTO): string[] => {
  const normalizedName = normalizeComparableName(campo.name_campo);
  const normalizedAlias = normalizeComparableName(campo.aleas_campo);
  const keys = [campo.name_campo];

  if (normalizedName === "TIPORADICADO") keys.push("tipoRadicado");
  if (normalizedName === "DESCRIPCIONDOCUMENTO") keys.push("tramite");
  if (normalizedName === "REFLUJOTRABAJO") keys.push("flujo");
  if (normalizedName === "FECHALIMITERESPUESTA") keys.push("fechaLimite");
  if (normalizedName === "REMITENTECOR") keys.push("remitente", "REMITENTE_COR");
  if (normalizedName === "DESTINATARIOCOR") {
    keys.push("destinatario", "Destinatario_Cor", "DESTINATARIO_COR");
  }
  if (
    normalizedName.includes("TIPORADICADOPLANTILLA") ||
    normalizedAlias.includes("TIPORADICADOPLANTILLA")
  ) {
    keys.push("tipoRadicado", "TipoRadicado");
  }
  if (
    normalizedName.includes("NUMEROFOLIO") ||
    normalizedName.includes("NUMFOLIO") ||
    normalizedAlias.includes("NUMEROFOLIO") ||
    normalizedAlias.includes("NUMFOLIO")
  ) {
    keys.push("numeroFolios", "NumeroFolios", "NUMERO_FOLIOS");
  }

  return Array.from(new Set(keys));
};

const getCampoFormValue = (
  values: RadicacionRegistroFormValues,
  campo: CampoPlantillaDTO,
): RadicacionRegistroFormValue => {
  const directValue = getFieldValue(values, getEquivalentFormKeys(campo));
  if (directValue !== "") return directValue;

  const normalizedCampoName = normalizeComparableName(campo.name_campo);
  const matchingKey = Object.keys(values).find(
    (key) => normalizeComparableName(key) === normalizedCampoName,
  );
  return matchingKey ? values[matchingKey] : "";
};

const getDetalleEquivalentValue = (
  values: RadicacionRegistroFormValues,
  detalle: DetallePlantillaRadicadoDTO,
): RadicacionRegistroFormValue => {
  const normalizedName = normalizeComparableName(detalle.NombreCampo);
  const normalizedLabel = normalizeComparableName(detalle.Etiqueta);
  const keys = [detalle.NombreCampo];

  if (
    normalizedName.includes("TIPORADICADOPLANTILLA") ||
    normalizedLabel.includes("TIPORADICADOPLANTILLA")
  ) {
    keys.push("tipoRadicado", "TipoRadicado");
  }
  if (
    normalizedName.includes("NUMEROFOLIO") ||
    normalizedName.includes("NUMFOLIO") ||
    normalizedLabel.includes("NUMEROFOLIO") ||
    normalizedLabel.includes("NUMFOLIO")
  ) {
    keys.push("numeroFolios", "NumeroFolios", "NUMERO_FOLIOS");
  }

  const directValue = getFieldValue(values, keys);
  if (directValue !== "") return directValue;

  const matchingKey = Object.keys(values).find(
    (key) => normalizeComparableName(key) === normalizedName,
  );
  return matchingKey ? values[matchingKey] : "";
};

const getNumeroFolios = (
  values: RadicacionRegistroFormValues,
  camposPlantilla: ReadonlyArray<CampoPlantillaDTO>,
  plantilla: PlantillaRadicadoDTO,
): number | null => {
  const campoNumeroFolios = camposPlantilla.find((campo) => {
    const normalizedName = normalizeComparableName(campo.name_campo);
    const normalizedAlias = normalizeComparableName(campo.aleas_campo);
    return (
      normalizedName.includes("NUMEROFOLIO") ||
      normalizedName.includes("NUMFOLIO") ||
      normalizedAlias.includes("NUMEROFOLIO") ||
      normalizedAlias.includes("NUMFOLIO")
    );
  });
  const detalleNumeroFolios = plantilla.DetallePlantillaRadicadoDTO.find((detalle) => {
    const normalizedName = normalizeComparableName(detalle.NombreCampo);
    const normalizedLabel = normalizeComparableName(detalle.Etiqueta);
    return (
      normalizedName.includes("NUMEROFOLIO") ||
      normalizedName.includes("NUMFOLIO") ||
      normalizedLabel.includes("NUMEROFOLIO") ||
      normalizedLabel.includes("NUMFOLIO")
    );
  });
  const value = campoNumeroFolios
    ? getCampoFormValue(values, campoNumeroFolios)
    : detalleNumeroFolios
      ? getDetalleEquivalentValue(values, detalleNumeroFolios)
      : getFieldValue(values, ["numeroFolios", "NumeroFolios", "NUMERO_FOLIOS"]);
  const numberValue = toNumber(extractFirstValue(value));
  return numberValue > 0 ? numberValue : null;
};

const findCampo = (
  camposPlantilla: ReadonlyArray<CampoPlantillaDTO>,
  normalizedName: string,
) =>
  camposPlantilla.find(
    (campo) => normalizeCampoName(campo.name_campo) === normalizedName,
  );

const resolveOptionLabel = (
  campo: CampoPlantillaDTO | undefined,
  selectedValue: string,
  fallback: string,
): string => {
  const options = mapCampoDrowlistOptions(campo?.ilist_row_drowlist);
  return (
    options.find((option) => String(option.value) === selectedValue)?.label ??
    fallback
  );
};

const resolveSelectedOption = (
  campo: CampoPlantillaDTO | undefined,
  selectedValue: string,
) => {
  const options = mapCampoDrowlistOptions(campo?.ilist_row_drowlist);
  return options.find((option) => String(option.value) === selectedValue);
};

const resolveFlujoLabel = (
  flujoOptions: ReadonlyArray<FlujoRelacionadoOption>,
  selectedValue: string,
) =>
  flujoOptions.find((option) => String(option.value) === selectedValue)?.label ??
  "";

const getDetalleId = (
  plantilla: PlantillaRadicadoDTO,
  campo: CampoPlantillaDTO,
  index: number,
): number => {
  const detalle = plantilla.DetallePlantillaRadicadoDTO.find(
    (item) =>
      normalizeCampoName(item.NombreCampo) === normalizeCampoName(campo.name_campo),
  );

  return detalle?.IdDetallePlantillaRadicado ?? index + 1;
};

const buildCampos = (
  values: RadicacionRegistroFormValues,
  camposPlantilla: ReadonlyArray<CampoPlantillaDTO>,
  plantilla: PlantillaRadicadoDTO,
): RegistrarRadicacionCampoDto[] => {
  const campos = camposPlantilla.map((campo, index) => ({
    IdDetallePlantillaRadicado: getDetalleId(plantilla, campo, index),
    NombreCampo: campo.name_campo,
    Valor: normalizeRegistroString(getCampoFormValue(values, campo)),
  }));

  const existingNames = new Set(
    campos.map((campo) => normalizeComparableName(campo.NombreCampo)),
  );
  const detallesFaltantes = plantilla.DetallePlantillaRadicadoDTO.filter(
    (detalle) => !existingNames.has(normalizeComparableName(detalle.NombreCampo)),
  );

  return [
    ...campos,
    ...detallesFaltantes.map((detalle) => ({
      IdDetallePlantillaRadicado: detalle.IdDetallePlantillaRadicado,
      NombreCampo: detalle.NombreCampo,
      Valor: normalizeRegistroString(getDetalleEquivalentValue(values, detalle)),
    })),
  ];
};

export function buildRegistrarRadicacionEntranteRequest({
  values,
  camposPlantilla,
  plantilla,
  flujoOptions = [],
  tipoModuloRadicacion = RADICACION_TIPO_MODULO_REGISTRO,
}: BuildRegistrarRadicacionEntranteRequestParams): RegistrarRadicacionEntranteRequestDto {
  const campoTipoRadicado = findCampo(camposPlantilla, "TIPORADICADO");
  const campoTramite = findCampo(camposPlantilla, "DESCRIPCION_DOCUMENTO");
  const campoFlujo = findCampo(camposPlantilla, "RE_FLUJO_TRABAJO");

  const tipoRadicadoValue = normalizeRegistroString(values.tipoRadicado);
  const tipoRadicadoOption = resolveSelectedOption(campoTipoRadicado, tipoRadicadoValue);
  const tipoRadicadoLabel = tipoRadicadoOption?.label ?? tipoRadicadoValue;
  const tipoRadicadoId = toNumber(tipoRadicadoOption?.value ?? tipoRadicadoValue);
  const tramiteValue = normalizeRegistroString(values.tramite);
  const flujoValue = normalizeRegistroString(values.flujo);
  const remitenteValue = getFieldValue(values, ["remitente", "REMITENTE_COR"]);
  const destinatarioValue = getFieldValue(values, [
    "destinatario",
    "Destinatario_Cor",
    "DESTINATARIO_COR",
  ]);
  const expediente = normalizeRegistroString(values.expedienteRelacionado);

  return {
    tipoModuloRadicacion,
    ASUNTO: normalizeRegistroString(getFieldValue(values, ["ASUNTO", "asunto"])),
    Remitente: {
      Nombre:
        extractFirstLabel(remitenteValue) || normalizeRegistroString(remitenteValue),
      id_Dest_Ext: toNumber(extractFirstValue(remitenteValue)),
    },
    Destinatario: {
      Destinatario:
        extractFirstLabel(destinatarioValue) ||
        normalizeRegistroString(destinatarioValue),
      id_Remit_Dest_Int: toNumber(extractFirstValue(destinatarioValue)),
    },
    Tipo_tramite: {
      Descripcion: resolveOptionLabel(campoTramite, tramiteValue, tramiteValue),
      tipo_doc_entrante: toNumber(tramiteValue),
    },
    RE_flujo_trabajo: {
      NombreFlujo:
        resolveFlujoLabel(flujoOptions, flujoValue) ||
        resolveOptionLabel(campoFlujo, flujoValue, flujoValue),
      id_tipo_flujo_workflow: toNumber(flujoValue),
    },
    TipoRadicado: {
      TipoRadicacion: tipoRadicadoLabel,
      IdTipoRadicado: tipoRadicadoId,
    },
    TipoPlantillaRadicado: {
      TipoPlantillaRadicado: tipoRadicadoLabel,
      IdTipoPlantillaRdicado: tipoRadicadoId,
    },
    expedienteRelacionado: {
      Expediente: expediente,
      idExpediente: toNumber(expediente),
    },
    radicadoRelacionados: [],
    ANEXOS_COR: normalizeRegistroString(getFieldValue(values, ["ANEXOS_COR", "anexos"])),
    FECHALIMITERESPUESTA: normalizeRegistroString(
      getFieldValue(values, ["FECHALIMITERESPUESTA", "fechaLimite"]),
    ),
    numeroFolios: getNumeroFolios(values, camposPlantilla, plantilla),
    Campos: buildCampos(values, camposPlantilla, plantilla),
  };
}
