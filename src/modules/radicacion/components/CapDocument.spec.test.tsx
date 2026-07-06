import { render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import CapDocument from "./CapDocument";
import { RadicacionDocumentalProvider } from "../context/RadicacionDocumentalContext";
import type { RadicacionDocumentalState } from "../types/radicacionDocumental.types";

const workspaceMock = vi.fn();

vi.mock("../../digitalizacion", () => ({
  DigitalizacionDocumentalWorkspace: (props: unknown) => {
    workspaceMock(props);
    return <div>Workspace documental</div>;
  },
}));

vi.mock("../../../app/Components/UI/AppDigitalizador/hooks/useAppDigitalizadorScannerClient", () => ({
  useAppDigitalizadorScannerClient: () => ({ type: "scanner-client" }),
}));

const activeState: RadicacionDocumentalState = {
  idEstadoRadicado: 44,
  idRadicado: 55,
  consecutivoRadicado: "RAD-55",
  estadoActual: 0,
  requiereGestionDocumental: true,
  tieneTramiteDocumentalActivoEstado0: true,
  destinoPostRegistro: "documentos",
};

const renderCapDocument = (initialState: RadicacionDocumentalState) =>
  render(
    <RadicacionDocumentalProvider initialState={initialState}>
      <CapDocument />
    </RadicacionDocumentalProvider>,
  );

describe("CapDocument", () => {
  beforeEach(() => {
    workspaceMock.mockClear();
  });

  it("[SPEC:NAV-004] no muestra documentos ni gabinete mock", () => {
    renderCapDocument(activeState);

    expect(screen.queryByText(/CAPDOCUMENT/i)).not.toBeInTheDocument();
    expect(screen.queryByText(/Factura\.pdf/i)).not.toBeInTheDocument();
    expect(screen.queryByText(/Documentos: 4/i)).not.toBeInTheDocument();
    expect(
      screen.getByText(/Contexto documental incompleto/i),
    ).toBeInTheDocument();
    expect(workspaceMock).not.toHaveBeenCalled();
  });

  it("[SPEC:NAV-005] inicializa digitalizacion solo con contexto real del backend", () => {
    renderCapDocument({
      ...activeState,
      contextoDocumental: {
        nombreGabinete: "GAB-REAL",
      },
    });

    expect(screen.getByText("Workspace documental")).toBeInTheDocument();
    expect(workspaceMock).toHaveBeenCalledWith(
      expect.objectContaining({
        context: {
          modo: "crear",
          nombreGabinete: "GAB-REAL",
          radicado: "RAD-55",
        },
      }),
    );
  });
});
