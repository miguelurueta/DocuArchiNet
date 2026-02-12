import { act, renderHook } from "@testing-library/react";
import { beforeEach, describe, expect, test, vi } from "vitest";
import { useOTPVerifyMutation } from "./useOTPVerifyMutation";

const mocks = vi.hoisted(() => ({
  navigate: vi.fn(),
  block: vi.fn(),
  unblock: vi.fn(),
  notifyError: vi.fn(),
  refrescarClaims: vi.fn(),
  iniciarSesion: vi.fn(),
  seVerificarSegundoFactor: vi.fn(),
}));

vi.mock("react-router", () => ({
  useNavigate: () => mocks.navigate,
}));

vi.mock("@tanstack/react-query", () => ({
  useMutation: (
    options: {
      mutationFn: (variables: unknown) => Promise<unknown>;
      onSuccess?: (data: unknown) => void;
      onError?: (error: unknown) => void;
      onSettled?: () => void;
    }
  ) => ({
    mutate: (variables: unknown) => {
      void options
        .mutationFn(variables)
        .then(data => options.onSuccess?.(data))
        .catch(error => options.onError?.(error))
        .finally(() => options.onSettled?.());
    },
  }),
}));

vi.mock("../../../app/Components/UI/OperationBlockerContext", () => ({
  useOperationBlocker: () => ({ block: mocks.block, unblock: mocks.unblock }),
}));

vi.mock("../../../shared/hooks/useAxiosErrorNotifier", () => ({
  useAxiosErrorNotifier: () => mocks.notifyError,
}));

vi.mock("../../../app/auth/Hoks/useAuth", () => ({
  useAuth: () => ({ refrescarClaims: mocks.refrescarClaims }),
}));

vi.mock("../service/AuthSessionService", () => ({
  AuthSessionService: {
    iniciarSesion: mocks.iniciarSesion,
  },
}));

vi.mock("../service/seVerificarSegundoFactor", () => ({
  seVerificarSegundoFactor: (payload: unknown) => mocks.seVerificarSegundoFactor(payload),
}));

describe("useOTPVerifyMutation", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  test("[SPEC:OTP-001] OTP correcto emite token y navega", async () => {
    mocks.seVerificarSegundoFactor.mockResolvedValueOnce({ token: "jwt-token" });
    const { result } = renderHook(() => useOTPVerifyMutation());

    await act(async () => {
      result.current.mutate({ ChallengeId: "abc", Code: "123456" });
      await Promise.resolve();
    });

    expect(mocks.iniciarSesion).toHaveBeenCalledTimes(1);
    expect(mocks.refrescarClaims).toHaveBeenCalledTimes(1);
    expect(mocks.navigate).toHaveBeenCalledWith("/dashboard");
    expect(mocks.block).toHaveBeenCalled();
    expect(mocks.unblock).toHaveBeenCalled();
  });

  test("[SPEC:OTP-002] OTP inválido muestra error funcional", async () => {
    const invalidError = new Error("OTP_INVALID");
    mocks.seVerificarSegundoFactor.mockRejectedValueOnce(invalidError);
    const { result } = renderHook(() => useOTPVerifyMutation());

    await act(async () => {
      result.current.mutate({ ChallengeId: "abc", Code: "111111" });
      await Promise.resolve();
    });

    expect(mocks.notifyError).toHaveBeenCalledWith(invalidError);
    expect(mocks.navigate).not.toHaveBeenCalledWith("/dashboard");
  });
});
