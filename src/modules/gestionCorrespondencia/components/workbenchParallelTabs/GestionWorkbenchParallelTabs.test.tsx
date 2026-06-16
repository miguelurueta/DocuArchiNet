import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { GestionWorkbenchParallelTabs } from "./GestionWorkbenchParallelTabs";

describe("[SCRUMCORE-251] GestionWorkbenchParallelTabs", () => {
  it("renderiza Gestion y Documentos simultaneamente con labels accesibles", () => {
    render(
      <GestionWorkbenchParallelTabs
        gestion={<div>Contenido Gestion</div>}
        documentos={<div>Contenido Documentos</div>}
      />,
    );

    expect(screen.getByLabelText("Vista paralela de Gestion y Documentos")).toBeInTheDocument();
    expect(screen.getByLabelText("Gestion")).toHaveTextContent("Contenido Gestion");
    expect(screen.getByLabelText("Documentos")).toHaveTextContent("Contenido Documentos");
    expect(screen.getByLabelText("Redimensionar paneles")).toBeInTheDocument();
  });
});
