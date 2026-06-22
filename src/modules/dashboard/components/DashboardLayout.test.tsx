import { fireEvent, render, screen } from "@testing-library/react";
import { MemoryRouter, Route, Routes } from "react-router-dom";
import { describe, expect, test, vi, beforeEach } from "vitest";

let breakpointState = { md: true, xl: true };

const setViewportWidth = (width: number) => {
  Object.defineProperty(window, "innerWidth", {
    configurable: true,
    writable: true,
    value: width,
  });
  window.dispatchEvent(new Event("resize"));
};

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
    breakpointState = { md: true, xl: true };
    setViewportWidth(1280);
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

  test("usa drawer en iPad Pro portrait para evitar reflow del contenido", async () => {
    breakpointState = { md: true, xl: false };
    setViewportWidth(1024);

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
    const trigger = screen.getByLabelText("Abrir menú");
    fireEvent.click(trigger);

    expect(await screen.findByRole("dialog")).toBeInTheDocument();
  });

  test("usa drawer en anchos estrechos aunque md siga activo", async () => {
    breakpointState = { md: true, xl: false };
    setViewportWidth(853);

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
  });

  test("abre drawer del sidebar en mobile", async () => {
    breakpointState = { md: false, xl: false };
    setViewportWidth(600);

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
