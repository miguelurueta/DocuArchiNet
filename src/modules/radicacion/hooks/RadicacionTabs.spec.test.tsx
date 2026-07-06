import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import TabsDocu from "./RadicacionTabs";
import { RadicacionDocumentalProvider } from "../context/RadicacionDocumentalContext";
import { EMPTY_PLANTILLA_RADICADO } from "../services/radicacionDefaults";
import type { RadicacionDocumentalState } from "../types/radicacionDocumental.types";

vi.mock("../components/RadicacionForm", () => ({
  default: () => <div>Formulario radicacion</div>,
}));

vi.mock("../components/CapDocument", () => ({
  default: () => <div>Workbench documental</div>,
}));

vi.mock("../components/Modalpendiente", () => ({
  default: () => <button type="button">Pendientes</button>,
}));

const activeDocumentalState: RadicacionDocumentalState = {
  idEstadoRadicado: 123,
  estadoActual: 0,
  requiereGestionDocumental: true,
  tieneTramiteDocumentalActivoEstado0: true,
};

const renderTabs = (initialState?: RadicacionDocumentalState) =>
  render(
    <RadicacionDocumentalProvider initialState={initialState}>
      <TabsDocu plantilla={EMPTY_PLANTILLA_RADICADO} camposPlantilla={[]} />
    </RadicacionDocumentalProvider>,
  );

describe("RadicacionTabs", () => {
  it("[SPEC:DOC-009] deshabilita Documentos sin tramite documental activo", () => {
    renderTabs();

    const documentosTab = screen.getByRole("tab", {
      name: /Captura de Documentos/i,
    });

    expect(documentosTab).toHaveAttribute("aria-disabled", "true");
  });

  it("[SPEC:DOC-010] habilita Documentos desde el contexto y permite renderizar CapDocument", () => {
    renderTabs(activeDocumentalState);

    const documentosTab = screen.getByRole("tab", {
      name: /Captura de Documentos/i,
    });

    expect(documentosTab).toHaveAttribute("aria-disabled", "false");
    fireEvent.click(documentosTab);
    expect(screen.getByText("Workbench documental")).toBeInTheDocument();
  });
});
