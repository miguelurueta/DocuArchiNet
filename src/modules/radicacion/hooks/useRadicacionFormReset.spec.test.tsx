import { act, renderHook } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import type { FormInstance } from "antd/es/form";
import { useRadicacionFormReset } from "./useRadicacionFormReset";

describe("useRadicacionFormReset", () => {
  it("[SPEC:TD-FE-05] reinicia campos y estado local sin tocar estado documental", () => {
    const resetFields = vi.fn();
    const setFieldValue = vi.fn();
    const setSelectedTramiteId = vi.fn();
    const setHasUserChangedTramite = vi.fn();
    const setResetKey = vi.fn((updater: (value: number) => number) =>
      updater(7),
    );
    const setModalVisible = vi.fn();
    const setUsuarioSeleccionado = vi.fn();
    const form = {
      resetFields,
      setFieldValue,
    } as unknown as FormInstance;

    const { result } = renderHook(() =>
      useRadicacionFormReset({
        form,
        setSelectedTramiteId,
        setHasUserChangedTramite,
        setResetKey,
        setModalVisible,
        setUsuarioSeleccionado,
      }),
    );

    act(() => {
      result.current.handleClearRadicacionForm();
    });

    expect(resetFields).toHaveBeenCalledTimes(1);
    expect(setFieldValue).toHaveBeenCalledWith("tipoRadicado", undefined);
    expect(setFieldValue).toHaveBeenCalledWith("flujo", undefined);
    expect(setSelectedTramiteId).toHaveBeenCalledWith(null);
    expect(setHasUserChangedTramite).toHaveBeenCalledWith(false);
    expect(setModalVisible).toHaveBeenCalledWith(false);
    expect(setUsuarioSeleccionado).toHaveBeenCalledWith(null);
    expect(setResetKey).toHaveBeenCalledTimes(1);
    expect(setResetKey.mock.results[0]?.value).toBe(8);
  });
});
