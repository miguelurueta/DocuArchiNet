import { fireEvent, render, screen } from "@testing-library/react";
import { MemoryRouter, Route, Routes } from "react-router-dom";
import { describe, expect, test, vi, beforeEach } from "vitest";

let breakpointState = { md: true };

vi.mock("antd", async () => {
  const actual = await vi.importActual<typeof import("antd")>("antd");
  return {
    ...actual,
    Grid: {
      ...actual.Grid,
      useBreakpoint: () => breakpointState,
    },
  };
});

vi.mock("../hooks/useDashboardMenu", () => ({
  useDashboardMenu: () => ({
    menuTree: [
      {
        IdMenuPrincipal: 1,
        NombreModulo: "Inicio",
        ToltipNode: "Ir a inicio",
        UrlNode: "/dashboard/home",
        VisibleNode: 1,
        IdPadre: 0,
        Orden: 1,
        Icono: "fa-solid fa-house",
        children: [],
      },
    ],
    isLoading: false,
    error: null,
  }),
}));

vi.mock("../hooks/useDashboardMetrics", () => ({
  useDashboardMetrics: () => ({
    metricMap: new Map([[1, 0]]),
    isLoading: false,
  }),
}));

vi.mock("../../../shared/hooks/useAppErrorNotifier", () => ({
  useAppErrorNotifier: () => vi.fn(),
}));

import DashboardLayout from "./DashboardLayout";

describe("DashboardLayout responsive navigation", () => {
  beforeEach(() => {
    breakpointState = { md: true };
  });

  test("mantiene sidebar fijo en desktop", () => {
    render(
      <MemoryRouter initialEntries={["/dashboard/home"]}>
        <Routes>
          <Route path="/dashboard" element={<DashboardLayout />}>
            <Route path="home" element={<div>Contenido</div>} />
          </Route>
        </Routes>
      </MemoryRouter>
    );

    expect(screen.getByAltText("DocuArchi")).toBeInTheDocument();
    expect(screen.queryByRole("dialog")).not.toBeInTheDocument();
    expect(screen.getByLabelText("Colapsar menú")).toBeInTheDocument();
  });

  test("abre drawer del sidebar en mobile", async () => {
    breakpointState = { md: false };

    render(
      <MemoryRouter initialEntries={["/dashboard/home"]}>
        <Routes>
          <Route path="/dashboard" element={<DashboardLayout />}>
            <Route path="home" element={<div>Contenido</div>} />
          </Route>
        </Routes>
      </MemoryRouter>
    );

    const trigger = screen.getByLabelText("Abrir menú");
    fireEvent.click(trigger);

    expect(await screen.findByRole("dialog")).toBeInTheDocument();
    expect(screen.getAllByAltText("DocuArchi").length).toBeGreaterThan(0);
  });
});
