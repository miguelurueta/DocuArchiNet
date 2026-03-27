import { fireEvent, render, screen } from "@testing-library/react";
import { MemoryRouter, Route, Routes } from "react-router-dom";
import { describe, expect, test } from "vitest";
import GestionCorrespondenciaLayout from "../layout/GestionCorrespondenciaLayout";
import GestionRespuesta from "../pages/GestionRespuesta";
import GestionCorrespondenciaRoute from "./GestionCorrespondenciaRoute";

function renderGestionCorrespondencia(initialEntry: string) {
  return render(
    <MemoryRouter initialEntries={[initialEntry]}>
      <Routes>
        <Route path="/dashboard/gestion-correspondencia" element={<GestionCorrespondenciaLayout />}>
          <Route index element={<GestionCorrespondenciaRoute />} />
          <Route
            path="respuesta"
            element={<GestionCorrespondenciaRoute drawerContent={<GestionRespuesta />} />}
          />
        </Route>
      </Routes>
    </MemoryRouter>,
  );
}

describe("[SPEC:GCORR-001] GestionCorrespondencia routing", () => {
  test("renderiza layout y pagina principal en la ruta base", () => {
    renderGestionCorrespondencia("/dashboard/gestion-correspondencia");

    expect(
      screen.getByRole("heading", { name: /Gestion de Correspondencia/i }),
    ).toBeInTheDocument();
    expect(
      screen.getByRole("heading", { name: /Centro operativo del modulo/i }),
    ).toBeInTheDocument();
    expect(
      screen.getByRole("navigation", { name: /breadcrumb/i }),
    ).toBeInTheDocument();
    expect(
      screen.getByRole("button", { name: /Abrir respuesta contextual/i }),
    ).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /Actualizar resumen/i })).toBeInTheDocument();
  });

  test("abre el drawer por subruta y vuelve a la ruta base al cerrar", () => {
    renderGestionCorrespondencia("/dashboard/gestion-correspondencia/respuesta");

    expect(
      screen.getByRole("heading", { name: /Centro operativo del modulo/i }),
    ).toBeInTheDocument();
    expect(screen.getByRole("dialog")).toBeInTheDocument();
    expect(
      screen.getByRole("heading", { name: /Gestion de respuesta/i }),
    ).toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: /^Close$/i }));

    expect(screen.queryByRole("dialog")).not.toBeInTheDocument();
    expect(screen.getByRole("button", { name: /Abrir respuesta contextual/i })).toBeInTheDocument();
  });
});
