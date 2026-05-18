import { render, screen } from "@testing-library/react";
import { describe, expect, test, vi } from "vitest";
import type React from "react";
import Autorizado from "./Autorizado";
import AutenticacionContext from "../Estado/AutenticacionContext";
import type Claim from "../Dto/Claim";

const mocks = vi.hoisted(() => ({
  sesionValida: vi.fn(),
}));

vi.mock("../Infraestructura/ManejadorJWT", () => ({
  sesionValida: () => mocks.sesionValida(),
}));

const renderWithClaims = (claims: Claim[], element: React.ReactNode) =>
  render(
    <AutenticacionContext.Provider value={{ claims, refrescarClaims: vi.fn() }}>
      {element}
    </AutenticacionContext.Provider>,
  );

describe("[SPEC:ACTUALIZACION-CLAIM-004] Autorizado", () => {
  test("permite rutas sin claims requeridos cuando hay sesion valida", () => {
    mocks.sesionValida.mockReturnValue(true);

    renderWithClaims(
      [],
      <Autorizado
        autorizado={<span>ok</span>}
        noAutorizado={<span>denegado</span>}
      />,
    );

    expect(screen.getByText("ok")).toBeInTheDocument();
  });

  test("valida permisos requeridos usando claims normalizados", () => {
    mocks.sesionValida.mockReturnValue(true);

    renderWithClaims(
      [{ nombre: "perm", valor: "tramites.gestionar" }],
      <Autorizado
        claims={["TRAMITES.GESTIONAR"]}
        autorizado={<span>ok</span>}
        noAutorizado={<span>denegado</span>}
      />,
    );

    expect(screen.getByText("ok")).toBeInTheDocument();
  });
});
