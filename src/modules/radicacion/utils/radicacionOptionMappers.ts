import type { CampoPlantillaDTO } from "../models/CampoPlantillaDTO";

export type RadicacionSelectOption = {
  value: string | number;
  label: string;
};

type BackendDropdownOption = {
  idValue?: string | number | null;
  id_value?: string | number | null;
  value?: string | number | null;
  Value?: string | null;
  value_campo?: string | null;
  label?: string | null;
};

type CampoWithIdScript = CampoPlantillaDTO & {
  id_escript?: number | null;
};

const isBackendDropdownOption = (
  option: unknown,
): option is BackendDropdownOption =>
  typeof option === "object" && option !== null;

const toNonEmptyLabel = (value: unknown): string => String(value ?? "").trim();

const mapBackendDropdownOptions = (
  options: ReadonlyArray<unknown>,
  resolveValue: (option: BackendDropdownOption) => string | number | null | undefined,
  resolveLabel: (option: BackendDropdownOption) => unknown,
): RadicacionSelectOption[] =>
  options
    .filter(isBackendDropdownOption)
    .map((option) => {
      const value = resolveValue(option) ?? "";
      const label = toNonEmptyLabel(resolveLabel(option));
      return { value, label };
    })
    .filter((option) => option.value !== "" || option.label !== "");

export const mapTramiteOptions = (
  options: ReadonlyArray<unknown> | null | undefined,
): RadicacionSelectOption[] =>
  mapBackendDropdownOptions(
    options ?? [],
    (option) => option.id_value ?? option.idValue ?? option.value,
    (option) => option.value_campo ?? option.Value ?? option.label,
  );

export const mapTipoRadicadoOptions = (
  options: ReadonlyArray<unknown> | null | undefined,
): RadicacionSelectOption[] => [
  { value: "", label: "Seleccionar" },
  ...mapBackendDropdownOptions(
    options ?? [],
    (option) => option.idValue ?? option.id_value ?? option.value,
    (option) => option.Value ?? option.value_campo ?? option.label,
  ),
];

export const mapCampoDrowlistOptions = (
  options: ReadonlyArray<unknown> | null | undefined,
): RadicacionSelectOption[] =>
  (options ?? [])
    .filter(isBackendDropdownOption)
    .map((option, index) => {
      const value =
        option.idValue ?? option.id_value ?? option.value ?? String(index);
      const label = toNonEmptyLabel(
        option.Value ?? option.value_campo ?? option.label ?? value,
      );
      return { value: value ?? "", label };
    });

export const normalizeCampoName = (value: string | null | undefined): string =>
  String(value ?? "").trim().toUpperCase();

export const resolveCampoIdScript = (
  campo: CampoPlantillaDTO,
): number | undefined => {
  const nestedId = campo.TomPParameterTomSelelect?.id_escript;
  if (typeof nestedId === "number" && Number.isFinite(nestedId)) {
    return nestedId;
  }

  const directId = (campo as CampoWithIdScript).id_escript;
  if (typeof directId === "number" && Number.isFinite(directId)) {
    return directId;
  }

  return undefined;
};
