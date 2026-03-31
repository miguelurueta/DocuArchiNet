import { render, screen } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { describe, expect, test } from "vitest";
import Workflow from "./Workflow";

describe("[SPEC:IMPLEMENTACION-APPTOLBAR-APPCONTENT-WORKFLOW] Workflow UI", () => {
  test("renderiza toolbar y contenido en orden correcto", () => {
    const { container } = render(
      <MemoryRouter>
        <Workflow />
      </MemoryRouter>,
    );

    const toolbar = screen.getByTestId("workflow-toolbar");
    const content = screen.getByTestId("workflow-appcontent");
    const position = toolbar.compareDocumentPosition(content);

    expect(position & Node.DOCUMENT_POSITION_FOLLOWING).toBeTruthy();
    expect(container.querySelector("section")).toBeTruthy();
  });

  test("muestra controles de toolbar y placeholder de contenido", () => {
    render(
      <MemoryRouter>
        <Workflow />
      </MemoryRouter>,
    );

    expect(screen.getByRole("button", { name: /Opciones/i })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /Actualizar/i })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /Abrir asignacion/i })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /Abrir enlace/i })).toBeInTheDocument();
    expect(screen.getByText(/Listado de Workflow/i)).toBeInTheDocument();
    expect(screen.getByText(/tabla/i)).toBeInTheDocument();
  });
});
