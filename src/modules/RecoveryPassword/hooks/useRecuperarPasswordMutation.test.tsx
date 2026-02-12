import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { renderHook, waitFor } from "@testing-library/react";
import type { ReactNode } from "react";
import { MemoryRouter } from "react-router";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { useRecuperarPasswordMutation } from "./useRecuperarPasswordMutation";

const navigateMock = vi.fn();
const notifyErrorMock = vi.fn();
const blockMock = vi.fn();
const unblockMock = vi.fn();
const setRecuperarPasswordMock = vi.fn();

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

vi.mock("../services/seRecuperarPassword", () => ({
  setRecuperarPassword: (data: unknown) => setRecuperarPasswordMock(data),
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

describe("useRecuperarPasswordMutation", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("[SPEC:REC-003] Usuario inexistente / respuesta funcional continúa el flujo", async () => {
    const payload = {
      challengeId: "challenge-1",
      destinoEnmascarado: "u***@mail.com",
      tiempoExpira: 15,
      userId: 3,
      idModule: 9,
    };
    setRecuperarPasswordMock.mockResolvedValue(payload);

    const { result } = renderHook(() => useRecuperarPasswordMutation(), {
      wrapper: createWrapper(),
    });

    result.current.mutate({ user: "ghost", idModule: 9, IdEmpresa: 2 });

    await waitFor(() => {
      expect(navigateMock).toHaveBeenCalledWith(
        "/RecoveryPassword/forgot-password/verify",
        { state: { payload } }
      );
    });

    expect(blockMock).toHaveBeenCalledWith("Enviando código de recuperación...");
    expect(unblockMock).toHaveBeenCalled();
  });

  it("[SPEC:REC-004] Error técnico de API notifica y detiene navegación", async () => {
    const error = new Error("network");
    setRecuperarPasswordMock.mockRejectedValue(error);

    const { result } = renderHook(() => useRecuperarPasswordMutation(), {
      wrapper: createWrapper(),
    });

    result.current.mutate({ user: "usuario.demo", idModule: 9, IdEmpresa: 2 });

    await waitFor(() => {
      expect(notifyErrorMock).toHaveBeenCalledWith(error);
    });

    expect(navigateMock).not.toHaveBeenCalled();
    expect(unblockMock).toHaveBeenCalled();
  });
});
