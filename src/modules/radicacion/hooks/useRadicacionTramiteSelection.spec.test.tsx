import { act, renderHook } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import type { FormInstance } from "antd/es/form";
import type { CampoPlantillaDTO } from "../models/CampoPlantillaDTO";
import { useFlujosRelacionadosTramite } from "./useFlujosRelacionadosTramite";
import { useRadicacionTramiteSelection } from "./useRadicacionTramiteSelection";

vi.mock("./useFlujosRelacionadosTramite", () => ({
  useFlujosRelacionadosTramite: vi.fn(),
}));

const mockedUseFlujosRelacionadosTramite = vi.mocked(useFlujosRelacionadosTramite);

const createForm = () =>
  ({
    setFieldValue: vi.fn(),
  }) as unknown as FormInstance;

describe("useRadicacionTramiteSelection", () => {
  beforeEach(() => {
    mockedUseFlujosRelacionadosTramite.mockReturnValue({
      data: [],
      isLoading: false,
      isFetching: false,
      error: null,
      shouldFetch: false,
    });
  });

  it("[SPEC:TD-FE-03] expone estado de seleccion, opciones y consulta flujos por tramite", () => {
    const form = createForm();
    const campoTramite = {
      ilist_row_drowlist: [{ idValue: 23, Value: "CITACION" }],
    } as unknown as CampoPlantillaDTO;

    const { result } = renderHook(() =>
      useRadicacionTramiteSelection({ form, campoTramite }),
    );

    expect(result.current.tramiteOptions).toEqual([
      { value: 23, label: "CITACION" },
    ]);
    expect(mockedUseFlujosRelacionadosTramite).toHaveBeenLastCalledWith(null, true);

    act(() => {
      result.current.handleTramiteChange(23);
    });

    expect(result.current.selectedTramiteId).toBe("23");
    expect(result.current.hasUserChangedTramite).toBe(true);
    expect(mockedUseFlujosRelacionadosTramite).toHaveBeenLastCalledWith("23", true);
  });

  it("[SPEC:TD-FE-03] limpia flujo cuando no hay tramite o cuando no hay opciones", () => {
    const form = createForm();

    renderHook(() => useRadicacionTramiteSelection({ form }));

    expect(form.setFieldValue).toHaveBeenCalledWith("flujo", undefined);
  });
});
