import { act, renderHook, waitFor } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { useDigitalizacionApiOperation } from "../hooks/useDigitalizacionApiOperation";

const createDeferred = <T,>() => {
  let resolveValue: (value: T) => void = () => undefined;
  const promise = new Promise<T>((resolve) => {
    resolveValue = resolve;
  });

  return { promise, resolve: resolveValue };
};

describe("[SPEC:SCRUMCORE-242] useDigitalizacionApiOperation", () => {
  it("blocks double submit while an operation is active", async () => {
    const deferred = createDeferred<string>();
    const operation = vi.fn(() => deferred.promise);
    const { result } = renderHook(() =>
      useDigitalizacionApiOperation<string, string>({
        operation,
        concurrentErrorCode: "CREATE_ALREADY_IN_PROGRESS",
      }),
    );

    void act(() => {
      void result.current.run("first");
    });

    await waitFor(() => {
      expect(result.current.loading).toBe(true);
    });

    await act(async () => {
      await expect(result.current.run("second")).rejects.toMatchObject({
        detail: expect.objectContaining({ code: "CREATE_ALREADY_IN_PROGRESS" }),
      });
    });

    await act(async () => {
      deferred.resolve("done");
    });
  });

  it("ignores stale response after cancel", async () => {
    const deferred = createDeferred<string>();
    const operation = vi.fn(() => deferred.promise);
    const { result } = renderHook(() =>
      useDigitalizacionApiOperation<string, string>({
        operation,
        concurrentErrorCode: "UPLOAD_ALREADY_IN_PROGRESS",
      }),
    );

    void act(() => {
      void result.current.run("upload");
    });
    await waitFor(() => {
      expect(result.current.loading).toBe(true);
    });

    act(() => {
      result.current.cancel();
    });
    await act(async () => {
      deferred.resolve("stale");
    });

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
      expect(result.current.data).toBeNull();
    });
  });
});
