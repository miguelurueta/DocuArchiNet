import { act, renderHook } from "@testing-library/react";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { useWorkflowInboxAutocomplete } from "../hooks/useWorkflowInboxAutocomplete";
import * as autocompleteService from "../services/workflowInboxAutocomplete.service";

vi.mock("../services/workflowInboxAutocomplete.service", () => ({
  getWorkflowInboxAutocomplete: vi.fn(),
}));

describe("[SPEC:gestion-correspondencia] useWorkflowInboxAutocomplete", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    vi.useFakeTimers();
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it("no consulta backend cuando el texto es menor a minLength", () => {
    const { result } = renderHook(() =>
      useWorkflowInboxAutocomplete({
        minLength: 3,
        limit: 10,
      }),
    );

    act(() => {
      result.current.setSearchText("ra");
      vi.advanceTimersByTime(300);
    });

    expect(autocompleteService.getWorkflowInboxAutocomplete).not.toHaveBeenCalled();
    expect(result.current.items).toEqual([]);
  });

  it("consulta backend con search y limit, expone loading y mapea items", async () => {
    let resolveRequest: ((value: { value: string; label?: string }[]) => void) | undefined;

    vi.mocked(autocompleteService.getWorkflowInboxAutocomplete).mockImplementation(
      () =>
        new Promise((resolve) => {
          resolveRequest = resolve;
        }),
    );

    const { result } = renderHook(() =>
      useWorkflowInboxAutocomplete({
        minLength: 2,
        limit: 10,
      }),
    );

    act(() => {
      result.current.setSearchText("rad");
    });

    await act(async () => {
      vi.advanceTimersByTime(300);
      await Promise.resolve();
    });

    expect(result.current.loading).toBe(true);
    expect(autocompleteService.getWorkflowInboxAutocomplete).toHaveBeenCalledWith({
      search: "rad",
      limit: 10,
    });

    await act(async () => {
      resolveRequest?.([{ value: "RAD-1", label: "Radicado 1" }]);
      await Promise.resolve();
    });

    expect(result.current.loading).toBe(false);
    expect(result.current.items).toEqual([{ value: "RAD-1", label: "Radicado 1" }]);
    expect(result.current.error).toBeNull();
  });

  it("maneja errores sin lanzar excepción al componente", async () => {
    vi.mocked(autocompleteService.getWorkflowInboxAutocomplete).mockRejectedValue(
      new Error("boom"),
    );

    const { result } = renderHook(() =>
      useWorkflowInboxAutocomplete({
        minLength: 2,
        limit: 10,
      }),
    );

    act(() => {
      result.current.setSearchText("rad");
    });

    await act(async () => {
      vi.advanceTimersByTime(300);
      await Promise.resolve();
    });

    await act(async () => {
      await Promise.resolve();
    });

    expect(result.current.loading).toBe(false);
    expect(result.current.items).toEqual([]);
    expect(result.current.error).toEqual(expect.any(Error));
    expect(result.current.error?.message).toBe("boom");
  });

  it("limpia sugerencias cuando el texto deja de cumplir minLength", async () => {
    vi.mocked(autocompleteService.getWorkflowInboxAutocomplete).mockResolvedValue([
      { value: "RAD-1", label: "Radicado 1" },
    ]);

    const { result } = renderHook(() =>
      useWorkflowInboxAutocomplete({
        minLength: 2,
        limit: 10,
      }),
    );

    act(() => {
      result.current.setSearchText("rad");
    });

    await act(async () => {
      vi.advanceTimersByTime(300);
      await Promise.resolve();
    });

    await act(async () => {
      await Promise.resolve();
    });

    expect(result.current.items).toEqual([{ value: "RAD-1", label: "Radicado 1" }]);

    act(() => {
      result.current.setSearchText("r");
    });

    expect(result.current.items).toEqual([]);
    expect(result.current.loading).toBe(false);
  });

  it("ignora respuestas obsoletas cuando llega una búsqueda más reciente", async () => {
    let firstResolve:
      | ((value: Array<{ value: string; label?: string }>) => void)
      | undefined;
    let secondResolve:
      | ((value: Array<{ value: string; label?: string }>) => void)
      | undefined;

    vi.mocked(autocompleteService.getWorkflowInboxAutocomplete)
      .mockImplementationOnce(
        () =>
          new Promise((resolve) => {
            firstResolve = resolve;
          }),
      )
      .mockImplementationOnce(
        () =>
          new Promise((resolve) => {
            secondResolve = resolve;
          }),
      );

    const { result } = renderHook(() =>
      useWorkflowInboxAutocomplete({
        minLength: 2,
        limit: 10,
      }),
    );

    act(() => {
      result.current.setSearchText("rad");
    });

    await act(async () => {
      vi.advanceTimersByTime(300);
      await Promise.resolve();
    });

    act(() => {
      result.current.setSearchText("radi");
    });

    await act(async () => {
      vi.advanceTimersByTime(300);
      await Promise.resolve();
    });

    await act(async () => {
      secondResolve?.([{ value: "NEW", label: "Nuevo" }]);
      await Promise.resolve();
    });

    expect(result.current.items).toEqual([{ value: "NEW", label: "Nuevo" }]);

    await act(async () => {
      firstResolve?.([{ value: "OLD", label: "Viejo" }]);
      await Promise.resolve();
    });

    expect(result.current.items).toEqual([{ value: "NEW", label: "Nuevo" }]);
  });
});
