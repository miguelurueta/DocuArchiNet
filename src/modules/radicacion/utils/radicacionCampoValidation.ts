import type { Rule } from "antd/es/form";
import type { CampoPlantillaDTO } from "../models/CampoPlantillaDTO";

export type CampoValidationValueMode = "text" | "selection" | "number";

interface BuildCampoPlantillaRulesParams {
  label: string;
  mode?: CampoValidationValueMode;
}

export const getCampoMaxLength = (
  campo: Pick<CampoPlantillaDTO, "max_leng_campo">,
): number | undefined =>
  typeof campo.max_leng_campo === "number" && campo.max_leng_campo > 0
    ? campo.max_leng_campo
    : undefined;

export const shouldValidateCampoMaxLength = (
  mode: CampoValidationValueMode,
): boolean => mode === "text";

export function buildCampoPlantillaRules(
  campo: Pick<CampoPlantillaDTO, "obligatorio_campo" | "max_leng_campo">,
  { label, mode = "text" }: BuildCampoPlantillaRulesParams,
): Rule[] {
  const rules: Rule[] = [];
  const maxLength = getCampoMaxLength(campo);

  if (campo.obligatorio_campo === 1) {
    rules.push({
      required: true,
      message:
        mode === "selection" ? `Seleccione ${label}` : `Ingrese ${label}`,
    });
  }

  if (maxLength !== undefined && shouldValidateCampoMaxLength(mode)) {
    rules.push({
      max: maxLength,
      message: `${label} supera la longitud maxima permitida.`,
    });
  }

  return rules;
}
