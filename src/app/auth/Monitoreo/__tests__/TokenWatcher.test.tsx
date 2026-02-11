import { render, screen, act } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import {
  describe,
  test,
  beforeEach,
  afterEach,
  expect,
  vi,
} from "vitest";

import TokenWatcher from "../TokenWatcher";
import AutenticacionContext from "../../Estado/AutenticacionContext";
import * as jwt from "../../Infraestructura/ManejadorJWT";
import { authConfig } from "../../Configuracion/config";

// 🔐 Mock ESM-safe
vi.mock("../../Hoks/useRenovarToken", async () => {
  return {
    default: vi.fn().mockResolvedValue(undefined),
  };
});

describe("TokenWatcher", () => {
  const refrescarClaimsMock = vi.fn();

  beforeEach(() => {
    vi.useFakeTimers();

    vi.spyOn(jwt, "finalizarSesionYRedirigir").mockImplementation(() => {});
    vi.spyOn(jwt, "tokenExpirado").mockReturnValue(false);
    vi.spyOn(jwt, "existeTokenRegistrado").mockReturnValue(true);

    authConfig.tokenStrategy = "redirect";
    authConfig.checkIntervalMs = 30000;
    authConfig.avisoDelayMs = 1000;
  });

  afterEach(() => {
    vi.clearAllTimers();
    vi.restoreAllMocks();
    refrescarClaimsMock.mockClear();
  });

  test("no valida nada si la ruta NO es restringida", () => {
    vi.spyOn(jwt, "tokenExpirado").mockReturnValue(true);

    render(
      <MemoryRouter initialEntries={["/LoginPage"]}>
        <AutenticacionContext.Provider
          value={{ claims: [], refrescarClaims: refrescarClaimsMock }}
        >
          <TokenWatcher />
        </AutenticacionContext.Provider>
      </MemoryRouter>
    );

    act(() => {
      vi.advanceTimersByTime(authConfig.checkIntervalMs);
    });

    expect(screen.queryByText(/sesión ha caducado/i)).toBeNull();
    expect(jwt.finalizarSesionYRedirigir).not.toHaveBeenCalled();
  });

  test("si ruta restringida y NO hay token, no renderiza overlay", () => {
    vi.spyOn(jwt, "tokenExpirado").mockReturnValue(true);
    vi.spyOn(jwt, "existeTokenRegistrado").mockReturnValue(false);

    render(
      <MemoryRouter initialEntries={["/dashboard"]}>
        <AutenticacionContext.Provider
          value={{ claims: [], refrescarClaims: refrescarClaimsMock }}
        >
          <TokenWatcher />
        </AutenticacionContext.Provider>
      </MemoryRouter>
    );

    act(() => {
      vi.advanceTimersByTime(authConfig.checkIntervalMs);
    });

    expect(screen.queryByText(/sesión ha caducado/i)).toBeNull();
  });

  test("si ruta restringida y token expirado en modo redirect, muestra overlay y redirige", () => {
    authConfig.tokenStrategy = "redirect";

    vi.spyOn(jwt, "tokenExpirado").mockReturnValue(true);
    vi.spyOn(jwt, "existeTokenRegistrado").mockReturnValue(true);

    render(
      <MemoryRouter initialEntries={["/dashboard"]}>
        <AutenticacionContext.Provider
          value={{ claims: [], refrescarClaims: refrescarClaimsMock }}
        >
          <TokenWatcher />
        </AutenticacionContext.Provider>
      </MemoryRouter>
    );

    act(() => {
      vi.advanceTimersByTime(authConfig.checkIntervalMs);
    });

    expect(
      screen.getByText(/tu sesión ha caducado/i)
    ).toBeInTheDocument();

    act(() => {
      vi.advanceTimersByTime(authConfig.avisoDelayMs);
    });

    expect(jwt.finalizarSesionYRedirigir).toHaveBeenCalledTimes(1);
  });

  test("si ruta restringida y token expirado en modo renew, renueva token y refresca claims", async () => {
    authConfig.tokenStrategy = "renew";

    vi.spyOn(jwt, "tokenExpirado").mockReturnValue(true);
    vi.spyOn(jwt, "existeTokenRegistrado").mockReturnValue(true);

    const renovarToken =
      (await import("../../Hoks/useRenovarToken")).default;

    render(
      <MemoryRouter initialEntries={["/dashboard"]}>
        <AutenticacionContext.Provider
          value={{ claims: [], refrescarClaims: refrescarClaimsMock }}
        >
          <TokenWatcher />
        </AutenticacionContext.Provider>
      </MemoryRouter>
    );

    await act(async () => {
      vi.advanceTimersByTime(authConfig.checkIntervalMs);
    });

    expect(renovarToken).toHaveBeenCalledTimes(1);
    expect(refrescarClaimsMock).toHaveBeenCalledTimes(1);
    expect(jwt.finalizarSesionYRedirigir).not.toHaveBeenCalled();
    expect(screen.queryByText(/sesión ha caducado/i)).toBeNull();
  });
});
