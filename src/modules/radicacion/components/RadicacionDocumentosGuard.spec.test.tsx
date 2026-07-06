import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { RadicacionDocumentalProvider } from "../context/RadicacionDocumentalContext";
import { RadicacionDocumentosGuard } from "./RadicacionDocumentosGuard";
import type { RadicacionDocumentalState } from "../types/radicacionDocumental.types";

const validDocumentalState: RadicacionDocumentalState = {
  idEstadoRadicado: 123,
  estadoActual: 0,
  requiereGestionDocumental: true,
  tieneTramiteDocumentalActivoEstado0: true,
};

const renderGuard = (initialState: RadicacionDocumentalState) =>
  render(
    <RadicacionDocumentalProvider initialState={initialState}>
      <RadicacionDocumentosGuard>
        <div>CapDocument listo</div>
      </RadicacionDocumentosGuard>
    </RadicacionDocumentalProvider>,
  );

describe("RadicacionDocumentosGuard", () => {
  it("[SPEC:DOC-006] renderiza documentos cuando el contexto es valido", () => {
    renderGuard(validDocumentalState);

    expect(screen.getByText("CapDocument listo")).toBeInTheDocument();
  });

  it("[SPEC:DOC-007] bloquea documentos cuando no hay estado 0 activo", () => {
    renderGuard({
      ...validDocumentalState,
      estadoActual: 1,
    });

    expect(screen.queryByText("CapDocument listo")).not.toBeInTheDocument();
    expect(
      screen.getByText(/captura de documentos est.*disponible/i),
    ).toBeInTheDocument();
  });

  it("[SPEC:DOC-008] bloquea documentos cuando requiereGestionDocumental es false", () => {
    renderGuard({
      ...validDocumentalState,
      requiereGestionDocumental: false,
    });

    expect(screen.queryByText("CapDocument listo")).not.toBeInTheDocument();
  });
});
