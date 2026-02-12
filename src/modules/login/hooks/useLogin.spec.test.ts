import { act, renderHook } from "@testing-library/react";
import { beforeEach, describe, expect, test, vi } from "vitest";
import useLogin from "./useLogin";

const mocks = vi.hoisted(() => ({
  navigate: vi.fn(),
  block: vi.fn(),
  unblock: vi.fn(),
  notifyError: vi.fn(),
  refrescarClaims: vi.fn(),
  iniciarSesion: vi.fn(),
  seLoginUsuario: vi.fn(),
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
    mutateAsync: async (variables: unknown) => {
      try {
        const data = await options.mutationFn(variables);
        options.onSuccess?.(data);
        return data;
      } catch (error) {
        options.onError?.(error);
        throw error;
      } finally {
        options.onSettled?.();
      }
    },
    isPending: false,
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

vi.mock("../../OTP/service/AuthSessionService", () => ({
  AuthSessionService: {
    iniciarSesion: mocks.iniciarSesion,
  },
}));

vi.mock("../services/seLoginUsuario", () => ({
  seLoginUsuario: (data: unknown) => mocks.seLoginUsuario(data),
}));

describe("useLogin", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  test("[SPEC:AUTH-001] Login SUCCESS emite token y navega", async () => {
    mocks.seLoginUsuario.mockResolvedValueOnce({
      data: {
        token: "jwt-token",
        expiracion: "2026-01-01T00:00:00.000Z",
        usuario: {
          usuarioId: 1,
          login: "jdoe",
          nombre: "John Doe",
          activo: true,
          permisos: [],
        },
      },
    });

    const { result } = renderHook(() => useLogin());

    await act(async () => {
      await result.current.login({
        User: "jdoe",
        Password: "secret",
        IdModulo: 1,
        IdEmpresa: 1,
      });
    });

    expect(mocks.iniciarSesion).toHaveBeenCalledTimes(1);
    expect(mocks.refrescarClaims).toHaveBeenCalledTimes(1);
    expect(mocks.navigate).toHaveBeenCalledWith("/dashboard");
    expect(mocks.block).toHaveBeenCalled();
    expect(mocks.unblock).toHaveBeenCalled();
  });

  test("[SPEC:AUTH-002] Login SECOND_FACTOR navega a verificación OTP", async () => {
    mocks.seLoginUsuario.mockResolvedValueOnce({
      message: "SECOND_FACTOR_REQUIRED",
      data: {
        ChallengeId: "challenge-1",
        DestinoEnmascarado: "u***@mail.com",
        TiempoExpira: 5,
      },
    });

    const { result } = renderHook(() => useLogin());

    await act(async () => {
      await result.current.login({
        User: "jdoe",
        Password: "secret",
        IdModulo: 1,
        IdEmpresa: 1,
      });
    });

    expect(mocks.navigate).toHaveBeenCalledWith("/verificar-otp", {
      state: {
        tipo: "SECOND_FACTOR",
        payload: {
          ChallengeId: "challenge-1",
          DestinoEnmascarado: "u***@mail.com",
          TiempoExpira: 5,
        },
      },
    });
    expect(mocks.iniciarSesion).not.toHaveBeenCalled();
  });
});
