import { render, screen, waitFor } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { RadicacionDocumentalProvider } from "../context/RadicacionDocumentalContext";
import { useRadicacionDocumentalContext } from "../hooks/useRadicacionDocumentalContext";
import { useRadicacionEstadoActivo } from "../hooks/useRadicacionEstadoActivo";
import { RadicacionStartupGuard } from "./RadicacionStartupGuard";

vi.mock("../hooks/useRadicacionEstadoActivo", () => ({
  useRadicacionEstadoActivo: vi.fn(),
}));

const mockedUseEstadoActivo = vi.mocked(useRadicacionEstadoActivo);

function ContextProbe() {
  const context = useRadicacionDocumentalContext();

  return (
    <div>
      <span>Modulo listo</span>
      <span data-testid="id-estado">{context.idEstadoRadicado ?? "none"}</span>
      <span data-testid="destino">{context.destinoPostRegistro}</span>
    </div>
  );
}

const renderStartup = () =>
  render(
    <RadicacionDocumentalProvider>
      <RadicacionStartupGuard>
        <ContextProbe />
      </RadicacionStartupGuard>
    </RadicacionDocumentalProvider>,
  );

describe("RadicacionStartupGuard", () => {
  it("[SPEC:BOOT-005] restaura el contexto antes del render funcional", async () => {
    mockedUseEstadoActivo.mockReturnValue({
      data: null,
      contextoDocumental: {
        idEstadoRadicado: 77,
        estadoActual: 0,
        requiereGestionDocumental: true,
        tieneTramiteDocumentalActivoEstado0: true,
        destinoPostRegistro: "documentos",
      },
      isLoading: false,
      isFetching: false,
      isError: false,
      error: null,
      refetch: vi.fn(),
    });

    renderStartup();

    expect(await screen.findByText("Modulo listo")).toBeInTheDocument();
    await waitFor(() => expect(screen.getByTestId("id-estado")).toHaveTextContent("77"));
    expect(screen.getByTestId("destino")).toHaveTextContent("documentos");
  });

  it("[SPEC:BOOT-006] limpia el contexto cuando no existe activo", async () => {
    mockedUseEstadoActivo.mockReturnValue({
      data: null,
      contextoDocumental: null,
      isLoading: false,
      isFetching: false,
      isError: false,
      error: null,
      refetch: vi.fn(),
    });

    renderStartup();

    expect(await screen.findByText("Modulo listo")).toBeInTheDocument();
    expect(screen.getByTestId("id-estado")).toHaveTextContent("none");
    expect(screen.getByTestId("destino")).toHaveTextContent("resumen");
  });

  it("[SPEC:BOOT-007] bloquea el render funcional mientras inicializa", () => {
    mockedUseEstadoActivo.mockReturnValue({
      data: null,
      contextoDocumental: null,
      isLoading: true,
      isFetching: false,
      isError: false,
      error: null,
      refetch: vi.fn(),
    });

    renderStartup();

    expect(screen.queryByText("Modulo listo")).not.toBeInTheDocument();
  });
});
