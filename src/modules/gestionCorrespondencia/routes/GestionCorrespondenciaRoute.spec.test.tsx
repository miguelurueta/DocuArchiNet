import { fireEvent, render, screen } from "@testing-library/react";
import { MemoryRouter, Route, Routes } from "react-router-dom";
import { describe, expect, test, vi } from "vitest";
import GestionCorrespondenciaLayout from "../layout/GestionCorrespondenciaLayout";
import GestionRespuesta from "../pages/GestionRespuesta";
import GestionCorrespondenciaRoute from "./GestionCorrespondenciaRoute";

vi.mock("../pages/GestionCorrespondenciaRoutePage", () => ({
  default: () => <div>Mocked GestionCorrespondenciaRoutePage</div>,
}));

function renderGestionCorrespondencia(initialEntry: string) {
  return render(
    <MemoryRouter initialEntries={[initialEntry]}>
      <Routes>
        <Route path="/dashboard/gestion-correspondencia" element={<GestionCorrespondenciaLayout />}>
          <Route index element={<GestionCorrespondenciaRoute />} />
          <Route
            path="respuesta/:id"
            element={<GestionCorrespondenciaRoute detailContent={<GestionRespuesta />} />}
          />
        </Route>
      </Routes>
    </MemoryRouter>,
  );
}

describe("[SPEC:SCRUMCORE-14] GestionCorrespondencia routing", () => {
  test("renderiza layout y pagina principal en la ruta base", () => {
    renderGestionCorrespondencia("/dashboard/gestion-correspondencia");

    expect(screen.getByText(/Mocked GestionCorrespondenciaRoutePage/i)).toBeInTheDocument();
    expect(screen.getByTestId("gestion-correspondencia-main-region")).toBeInTheDocument();
    expect(
      screen.queryByTestId("gestion-correspondencia-detail-region"),
    ).not.toBeInTheDocument();
  });

  test("abre el panel superpuesto por subruta y vuelve a la ruta base al cerrar", () => {
    renderGestionCorrespondencia("/dashboard/gestion-correspondencia/respuesta/924");

    expect(screen.getByText(/Mocked GestionCorrespondenciaRoutePage/i)).toBeInTheDocument();
    expect(screen.getByTestId("gestion-correspondencia-detail-region")).toBeInTheDocument();
    expect(screen.queryByRole("dialog")).not.toBeInTheDocument();
    expect(
      screen.getByRole("heading", { name: /Gestion de respuesta/i }),
    ).toBeInTheDocument();
    expect(screen.getByRole("heading", { name: /Respuesta contextual/i })).toBeInTheDocument();
    expect(screen.getByText(/Revisa el detalle sin salir de la bandeja/i)).toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: /Volver a la bandeja/i }));

    expect(screen.queryByTestId("gestion-correspondencia-detail-region")).not.toBeInTheDocument();
    expect(screen.getByText(/Mocked GestionCorrespondenciaRoutePage/i)).toBeInTheDocument();
  });

  test("resuelve deep link manteniendo principal y secundaria visibles", () => {
    renderGestionCorrespondencia("/dashboard/gestion-correspondencia/respuesta/924");

    expect(screen.getByTestId("gestion-correspondencia-main-region")).toBeInTheDocument();
    expect(screen.getByTestId("gestion-correspondencia-detail-region")).toBeInTheDocument();
  });

  test("usa un shell observable con panel superpuesto en lugar de dialog modal", () => {
    renderGestionCorrespondencia("/dashboard/gestion-correspondencia/respuesta/924");

    expect(screen.getByTestId("gestion-correspondencia-route-shell")).toBeInTheDocument();
    expect(screen.getByTestId("gestion-correspondencia-main-region")).toBeVisible();
    expect(screen.getByTestId("gestion-correspondencia-detail-region")).toBeVisible();
    expect(screen.queryByRole("dialog")).not.toBeInTheDocument();
    expect(
      screen.getByLabelText(/Panel superpuesto de gestion de correspondencia/i),
    ).toBeInTheDocument();
  });

  test("muestra una accion visible de retorno consistente con el patron master-detail", () => {
    renderGestionCorrespondencia("/dashboard/gestion-correspondencia/respuesta/924");

    expect(
      screen.getByRole("button", { name: /Volver a la bandeja/i }),
    ).toBeVisible();
    expect(screen.getByText(/Retorno contextual/i)).toBeInTheDocument();
  });
});
