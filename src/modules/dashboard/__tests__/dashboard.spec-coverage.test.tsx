import { fireEvent, render, screen } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { beforeEach, describe, expect, test, vi } from "vitest";
import CardComponent from "../components/CardComponent";
import { buildMenuTree } from "../utils/buildMenuTree";
import type { RawMenuItem } from "../types/menu";

const navigateMock = vi.fn();

vi.mock("react-router-dom", async () => {
  const actual = await vi.importActual<typeof import("react-router-dom")>(
    "react-router-dom"
  );

  return {
    ...actual,
    useNavigate: () => navigateMock,
  };
});

const baseNode: RawMenuItem = {
  IdMenuPrincipal: 1,
  NombreModulo: "Inicio",
  ValueNode: "inicio",
  ToltipNode: "Ir a inicio",
  UrlNode: "/dashboard/home",
  PageName: "Dashboard",
  VisibleNode: 1,
  NodoPlantillaRadicado: "",
  TipoPlantilla: "",
  IdPlantilla: 0,
  UrlExterna: "",
  UrlContent: "",
  ValueContent: "",
  ValueCard: "",
  ValueCardConten: "",
  TIpoModulo: "interno",
  AcesoDirecto: 1,
  IdPadre: 0,
  Orden: 1,
  Icono: "fa-solid fa-house",
};

describe("Dashboard Spec Coverage", () => {
  beforeEach(() => {
    navigateMock.mockReset();
  });

  test("[SPEC:DASH-001] Render inicial con menú cargado", () => {
    const tree = buildMenuTree([baseNode]);

    expect(tree).toHaveLength(1);
    expect(tree[0]?.NombreModulo).toBe("Inicio");
    expect(tree[0]?.children).toEqual([]);
  });

  test("[SPEC:DASH-002] Navegación SPA al click en card", () => {
    const tree = buildMenuTree([baseNode]);

    render(
      <MemoryRouter>
        <CardComponent menuTree={tree} metricMap={new Map()} />
      </MemoryRouter>
    );

    fireEvent.click(screen.getByRole("button", { name: "Inicio" }));

    expect(navigateMock).toHaveBeenCalledWith("/dashboard/home");
  });
});
