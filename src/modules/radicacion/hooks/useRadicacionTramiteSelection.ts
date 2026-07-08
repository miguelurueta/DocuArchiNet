import { useCallback, useEffect, useMemo, useState } from "react";
import type { FormInstance } from "antd/es/form";
import type { CampoPlantillaDTO } from "../models/CampoPlantillaDTO";
import { useFlujosRelacionadosTramite } from "./useFlujosRelacionadosTramite";
import { mapTramiteOptions } from "../utils/radicacionOptionMappers";

type UseRadicacionTramiteSelectionParams = {
  form: FormInstance;
  campoTramite?: CampoPlantillaDTO;
};

export function useRadicacionTramiteSelection({
  form,
  campoTramite,
}: UseRadicacionTramiteSelectionParams) {
  const [selectedTramiteId, setSelectedTramiteId] = useState<string | null>(null);
  const [hasUserChangedTramite, setHasUserChangedTramite] = useState(false);

  const tramiteOptions = useMemo(
    () => mapTramiteOptions(campoTramite?.ilist_row_drowlist),
    [campoTramite],
  );

  const {
    data: flujosRelacionados,
    error: flujosRelacionadosError,
    isLoading: isLoadingFlujosRelacionados,
  } = useFlujosRelacionadosTramite(selectedTramiteId, true);

  const flujoOptions = useMemo(
    () => (selectedTramiteId ? flujosRelacionados : []),
    [flujosRelacionados, selectedTramiteId],
  );

  useEffect(() => {
    if (!selectedTramiteId || flujosRelacionadosError || flujoOptions.length === 0) {
      form.setFieldValue("flujo", undefined);
    }
  }, [flujoOptions.length, flujosRelacionadosError, form, selectedTramiteId]);

  const handleTramiteChange = useCallback((value: unknown) => {
    setHasUserChangedTramite(true);
    const normalized = String(value ?? "").trim();
    setSelectedTramiteId(normalized.length > 0 ? normalized : null);
  }, []);

  return {
    selectedTramiteId,
    setSelectedTramiteId,
    hasUserChangedTramite,
    setHasUserChangedTramite,
    tramiteOptions,
    flujoOptions,
    isLoadingFlujosRelacionados,
    handleTramiteChange,
  };
}
