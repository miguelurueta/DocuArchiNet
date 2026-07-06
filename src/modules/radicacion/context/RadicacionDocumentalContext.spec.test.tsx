import { act, renderHook } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import {
  RadicacionDocumentalProvider,
} from "./RadicacionDocumentalContext";
import { RADICACION_DOCUMENTAL_INITIAL_STATE } from "./radicacionDocumentalContextValue";
import { useRadicacionDocumentalContext } from "../hooks/useRadicacionDocumentalContext";
import type { RadicacionDocumentalState } from "../types/radicacionDocumental.types";

const validDocumentalState: RadicacionDocumentalState = {
  idEstadoRadicado: 123,
  idRadicado: 456,
  consecutivoRadicado: "RAD-123",
  idTareaWorkflow: 789,
  estadoActual: 0,
  requiereGestionDocumental: true,
  tieneTramiteDocumentalActivoEstado0: true,
  destinoPostRegistro: "documentos",
};

describe("RadicacionDocumentalContext", () => {
  it("[SPEC:DOC-001] expone ausencia de tramite documental como estado inicial", () => {
    const { result } = renderHook(() => useRadicacionDocumentalContext(), {
      wrapper: RadicacionDocumentalProvider,
    });

    expect(result.current.idEstadoRadicado).toBeNull();
    expect(result.current.estadoActual).toBeNull();
    expect(result.current.requiereGestionDocumental).toBe(false);
    expect(result.current.tieneTramiteDocumentalActivoEstado0).toBe(false);
    expect(result.current.destinoPostRegistro).toBe("resumen");
  });

  it("[SPEC:DOC-002] setContextoDocumental establece un contexto activo coherente", () => {
    const { result } = renderHook(() => useRadicacionDocumentalContext(), {
      wrapper: RadicacionDocumentalProvider,
    });

    act(() => {
      result.current.setContextoDocumental(validDocumentalState);
    });

    expect(result.current.idEstadoRadicado).toBe(123);
    expect(result.current.estadoActual).toBe(0);
    expect(result.current.requiereGestionDocumental).toBe(true);
    expect(result.current.tieneTramiteDocumentalActivoEstado0).toBe(true);
  });

  it("[SPEC:DOC-003] clearContextoDocumental restaura el estado inicial", () => {
    const { result } = renderHook(() => useRadicacionDocumentalContext(), {
      wrapper: RadicacionDocumentalProvider,
    });

    act(() => {
      result.current.setContextoDocumental(validDocumentalState);
      result.current.clearContextoDocumental();
    });

    expect(result.current).toMatchObject(RADICACION_DOCUMENTAL_INITIAL_STATE);
  });

  it("[SPEC:DOC-004] normaliza el flag activo si el estado documental no es 0", () => {
    const { result } = renderHook(() => useRadicacionDocumentalContext(), {
      wrapper: RadicacionDocumentalProvider,
    });

    act(() => {
      result.current.setContextoDocumental({
        ...validDocumentalState,
        estadoActual: 1,
      });
    });

    expect(result.current.tieneTramiteDocumentalActivoEstado0).toBe(false);
  });

  it("[SPEC:DOC-005] falla si el hook se consume fuera del provider", () => {
    expect(() => renderHook(() => useRadicacionDocumentalContext())).toThrow(
      /RadicacionDocumentalProvider/,
    );
  });
});
