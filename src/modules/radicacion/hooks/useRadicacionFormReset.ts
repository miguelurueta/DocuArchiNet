import { useCallback } from "react";
import type { Dispatch, SetStateAction } from "react";
import type { FormInstance } from "antd/es/form";

type UseRadicacionFormResetParams<TUsuarioSeleccionado> = {
  form: FormInstance;
  setSelectedTramiteId: Dispatch<SetStateAction<string | null>>;
  setHasUserChangedTramite: Dispatch<SetStateAction<boolean>>;
  setResetKey: Dispatch<SetStateAction<number>>;
  setModalVisible: Dispatch<SetStateAction<boolean>>;
  setUsuarioSeleccionado: Dispatch<SetStateAction<TUsuarioSeleccionado | null>>;
};

export function useRadicacionFormReset<TUsuarioSeleccionado>({
  form,
  setSelectedTramiteId,
  setHasUserChangedTramite,
  setResetKey,
  setModalVisible,
  setUsuarioSeleccionado,
}: UseRadicacionFormResetParams<TUsuarioSeleccionado>) {
  const handleClearRadicacionForm = useCallback(() => {
    form.resetFields();
    form.setFieldValue("tipoRadicado", undefined);
    form.setFieldValue("flujo", undefined);
    setSelectedTramiteId(null);
    setHasUserChangedTramite(false);
    setModalVisible(false);
    setUsuarioSeleccionado(null);
    setResetKey((prev) => prev + 1);
  }, [
    form,
    setHasUserChangedTramite,
    setModalVisible,
    setResetKey,
    setSelectedTramiteId,
    setUsuarioSeleccionado,
  ]);

  return { handleClearRadicacionForm };
}
