import { fireEvent, render, screen } from "@testing-library/react";
import { MemoryRouter, Route, Routes } from "react-router-dom";
import { describe, expect, test } from "vitest";
import WorkflowLayout from "../layout/WorkflowLayout";
import WorkflowAsignacion from "../pages/WorkflowAsignacion";
import WorkflowEnlace from "../pages/WorkflowEnlace";
import WorkflowRoute from "./WorkflowRoute";

function renderWorkflow(initialEntry: string) {
  return render(
    <MemoryRouter initialEntries={[initialEntry]}>
      <Routes>
        <Route path="/dashboard/workflow" element={<WorkflowLayout />}>
          <Route index element={<WorkflowRoute />} />
          <Route
            path="asignacion"
            element={
              <WorkflowRoute
                drawerTitle="Asignacion de workflow"
                drawerContent={<WorkflowAsignacion />}
              />
            }
          />
          <Route
            path="enlace"
            element={
              <WorkflowRoute
                drawerTitle="Enlace de workflow"
                drawerContent={<WorkflowEnlace />}
              />
            }
          />
        </Route>
      </Routes>
    </MemoryRouter>,
  );
}

describe("[SPEC:IMPLEMENTACION-ESTRUCTURA-WORKFLOW] Workflow routing", () => {
  test("renderiza layout y pagina principal en la ruta base", () => {
    renderWorkflow("/dashboard/workflow");

    expect(screen.getByTestId("workflow-content")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /Abrir asignacion/i })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /Abrir enlace/i })).toBeInTheDocument();
  });

  test("abre el drawer de asignacion y vuelve a la ruta base al cerrar", () => {
    renderWorkflow("/dashboard/workflow/asignacion");

    expect(screen.getByRole("dialog")).toBeInTheDocument();
    expect(
      screen.getByRole("heading", { name: /Workflow Asignacion/i }),
    ).toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: /^Close$/i }));

    expect(screen.queryByRole("dialog")).not.toBeInTheDocument();
    expect(screen.getByRole("button", { name: /Abrir asignacion/i })).toBeInTheDocument();
  });

  test("abre el drawer de enlace y vuelve a la ruta base al cerrar", () => {
    renderWorkflow("/dashboard/workflow/enlace");

    expect(screen.getByRole("dialog")).toBeInTheDocument();
    expect(
      screen.getByRole("heading", { name: /Workflow Enlace/i }),
    ).toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: /^Close$/i }));

    expect(screen.queryByRole("dialog")).not.toBeInTheDocument();
    expect(screen.getByRole("button", { name: /Abrir enlace/i })).toBeInTheDocument();
  });
});
