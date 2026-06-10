import { act, renderHook } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { useDigitalizacionDocumentalState } from "./useDigitalizacionDocumentalState";
import type { DigitalizacionContext } from "../types/digitalizacion.types";

const contextA: DigitalizacionContext = {
  modo: "crear",
  nombreGabinete: "Gestion",
  radicado: "RAD-1",
  requiereMetadata: true,
};

const contextB: DigitalizacionContext = {
  modo: "adjuntar",
  nombreGabinete: "Archivo",
  radicado: "RAD-2",
  idDocumentoDestino: 44,
};

describe("[SPEC:SCRUMCORE-239] useDigitalizacionDocumentalState", () => {
  it("initializes separated state from valid context", () => {
    const { result } = renderHook(() =>
      useDigitalizacionDocumentalState({ open: true, context: contextA }),
    );

    expect(result.current.state.context).toEqual(contextA);
    expect(result.current.state.metadata.required).toBe(true);
    expect(result.current.state.scanner.pages).toEqual([]);
    expect(result.current.state.operation.status).toBe("idle");
  });

  it("reports invalid context", () => {
    const onInvalidContext = vi.fn();

    const { result } = renderHook(() =>
      useDigitalizacionDocumentalState({
        open: true,
        context: null,
        onInvalidContext,
      }),
    );

    expect(result.current.state.validationError?.code).toBe("CONTEXT_REQUIRED");
    expect(result.current.state.operation.status).toBe("error");
    expect(onInvalidContext).toHaveBeenCalledWith(
      expect.objectContaining({ code: "CONTEXT_REQUIRED" }),
    );
  });

  it("resets state and invalidates generation when context changes", () => {
    const { result, rerender } = renderHook(
      ({ context }) => useDigitalizacionDocumentalState({ open: true, context }),
      { initialProps: { context: contextA } },
    );

    const initialGeneration = result.current.currentGeneration;

    act(() => {
      result.current.setOperation({ status: "saving" });
    });

    expect(result.current.state.operation.status).toBe("saving");

    rerender({ context: contextB });

    expect(result.current.state.context).toEqual(contextB);
    expect(result.current.state.operation.status).toBe("idle");
    expect(result.current.state.metadata.required).toBe(false);
    expect(result.current.isCurrentGeneration(initialGeneration)).toBe(false);
  });
});
