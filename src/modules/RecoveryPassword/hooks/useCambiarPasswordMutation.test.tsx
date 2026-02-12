import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { renderHook, waitFor } from "@testing-library/react";
import type { ReactNode } from "react";
import { MemoryRouter } from "react-router";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { useCambiarPasswordMutation } from "./useCambiarPasswordMutation";

const navigateMock = vi.fn();
const notifyErrorMock = vi.fn();
const blockMock = vi.fn();
const unblockMock = vi.fn();
const seResetPasswordMock = vi.fn();

vi.mock("react-router", async importOriginal => {
  const actual = await importOriginal<typeof import("react-router")>();
  return {
    ...actual,
    useNavigate: () => navigateMock,
  };
});

vi.mock("../../../shared/hooks/useAxiosErrorNotifier", () => ({
  useAxiosErrorNotifier: () => notifyErrorMock,
}));

vi.mock("../../../app/Components/UI/OperationBlockerContext", () => ({
  useOperationBlocker: () => ({
    block: blockMock,
    unblock: unblockMock,
  }),
}));

vi.mock("../services/seCambiarPassword", () => ({
  seResetPassword: (data: unknown) => seResetPasswordMock(data),
}));

const createWrapper = () => {
  const queryClient = new QueryClient({
    defaultOptions: { mutations: { retry: false } },
  });

  return ({ children }: { children: ReactNode }) => (
    <MemoryRouter>
      <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
    </MemoryRouter>
  );
};

describe("useCambiarPasswordMutation", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("[SPEC:RST-001] Cambio de contraseña SUCCESS con token válido", async () => {
    seResetPasswordMock.mockResolvedValue(undefined);

    const { result } = renderHook(() => useCambiarPasswordMutation(), {
      wrapper: createWrapper(),
    });

    result.current.mutate({
      token: "token-ok",
      idModule: 9,
      userId: 2,
      newPassword: "Clave123!",
      confirmNewPassword: "Clave123!",
    });

    await waitFor(() => {
      expect(navigateMock).toHaveBeenCalledWith("/", {
        state: { reason: "PASSWORD_RESET_OK" },
      });
    });

    expect(blockMock).toHaveBeenCalledWith("Actualizando contraseña...");
    expect(unblockMock).toHaveBeenCalled();
  });

  it("[SPEC:RST-002] Token inválido / expirado / reutilizado", async () => {
    const error = new Error("TOKEN_INVALID_OR_EXPIRED");
    seResetPasswordMock.mockRejectedValue(error);

    const { result } = renderHook(() => useCambiarPasswordMutation(), {
      wrapper: createWrapper(),
    });

    result.current.mutate({
      token: "token-expired",
      idModule: 9,
      userId: 2,
      newPassword: "Clave123!",
      confirmNewPassword: "Clave123!",
    });

    await waitFor(() => {
      expect(notifyErrorMock).toHaveBeenCalledWith(error);
    });

    expect(navigateMock).not.toHaveBeenCalled();
    expect(unblockMock).toHaveBeenCalled();
  });
});
