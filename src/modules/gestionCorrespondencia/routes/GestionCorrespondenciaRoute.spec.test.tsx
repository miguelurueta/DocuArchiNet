import { act, fireEvent, render, screen, waitFor } from "@testing-library/react";
import { MemoryRouter, Route, Routes } from "react-router-dom";
import { beforeEach, describe, expect, test, vi } from "vitest";
import * as estructuraRespuestaHook from "../hooks/useEstructuraRespuestaIdTarea";
import * as gabineteService from "../services/solicitaGabineteRadicadoWorkflow.service";
import GestionCorrespondenciaLayout from "../layout/GestionCorrespondenciaLayout";
import GestionRespuesta from "../pages/GestionRespuesta";
import type { UseEstructuraRespuestaIdTareaResult } from "../hooks/useEstructuraRespuestaIdTarea";
import GestionCorrespondenciaRoute from "./GestionCorrespondenciaRoute";

vi.mock("../pages/GestionCorrespondenciaRoutePage", () => ({
  default: () => <div>Mocked GestionCorrespondenciaRoutePage</div>,
}));

vi.mock("../hooks/useEstructuraRespuestaIdTarea", () => ({
  useEstructuraRespuestaIdTarea: vi.fn(),
}));

vi.mock("../services/solicitaGabineteRadicadoWorkflow.service", async () => {
  const actual = await vi.importActual<
    typeof import("../services/solicitaGabineteRadicadoWorkflow.service")
  >("../services/solicitaGabineteRadicadoWorkflow.service");
  return {
    ...actual,
    getSolicitaGabinetePorTareaWorkflow: vi.fn(),
  };
});

const setHookState = (state: Partial<UseEstructuraRespuestaIdTareaResult>) => {
  vi.mocked(estructuraRespuestaHook.useEstructuraRespuestaIdTarea).mockReturnValue({
    estrucTuraRespuesta: {
      Radicado: "2025-0001",
      Destinatario: "Contasoft Company",
      TramiteDocumento: "Respuesta a derecho de peticion",
    },
    loading: false,
    fetching: false,
    error: null,
    isEmpty: false,
    isEmptyLatched: false,
    resolved: true,
    ...state,
  });
};

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

beforeEach(() => {
  setHookState({});
  vi.mocked(gabineteService.getSolicitaGabinetePorTareaWorkflow).mockResolvedValue({
    success: true,
    message: "OK",
    data: {
      NombreGabinete: "WF_DOCS",
      Radicado: "2025-0001",
      EstadoExistenciaRadicado: "YES",
    },
  });
});

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
    expect(screen.getByText("2025-0001")).toBeInTheDocument();
    expect(screen.getByText("Contasoft Company")).toBeInTheDocument();
    expect(screen.queryByRole("dialog")).not.toBeInTheDocument();
    expect(screen.getByText("Gestion")).toBeInTheDocument();
    expect(screen.getByText("Documentos")).toBeInTheDocument();
    expect(screen.queryByText(/^Guardar$/i)).not.toBeInTheDocument();
    expect(screen.queryByText(/Volver a la bandeja/i)).not.toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: /Volver a la bandeja/i }));

    expect(screen.queryByTestId("gestion-correspondencia-detail-region")).not.toBeInTheDocument();
    expect(screen.getByText(/Mocked GestionCorrespondenciaRoutePage/i)).toBeInTheDocument();
  });

  test("resuelve deep link manteniendo principal y secundaria visibles", () => {
    renderGestionCorrespondencia("/dashboard/gestion-correspondencia/respuesta/924");

    expect(screen.getByTestId("gestion-correspondencia-main-region")).toBeInTheDocument();
    expect(screen.getByTestId("gestion-correspondencia-detail-region")).toBeInTheDocument();
  });

  test("mantiene el contenido del segundo tab sin regresiones", async () => {
    renderGestionCorrespondencia("/dashboard/gestion-correspondencia/respuesta/924");

    await act(async () => {
      fireEvent.click(screen.getByRole("tab", { name: /Documentos/i }));
    });

    expect(await screen.findByText(/Contenido principal/i)).toBeInTheDocument();
    expect(
      await screen.findByRole("status", { name: /Zona de documento/i }),
    ).toBeInTheDocument();
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
  });
});

describe("[SPEC:SCRUMCORE-143] Bloqueo por estructura gestion respuesta", () => {
  test("muestra estado de carga y evita render operativo mientras resuelve estructura", () => {
    setHookState({
      estrucTuraRespuesta: null,
      loading: true,
    });

    renderGestionCorrespondencia("/dashboard/gestion-correspondencia/respuesta/924");

    expect(screen.getByTestId("gestion-correspondencia-loading-state")).toBeInTheDocument();
    expect(screen.queryByText("Gestion")).not.toBeInTheDocument();
    expect(screen.queryByText("Documentos")).not.toBeInTheDocument();
  });

  test("bloquea detalle cuando la estructura viene vacia", () => {
    setHookState({
      estrucTuraRespuesta: null,
      isEmpty: true,
    });

    renderGestionCorrespondencia("/dashboard/gestion-correspondencia/respuesta/924");

    expect(screen.getByTestId("gestion-correspondencia-blocked-state")).toBeInTheDocument();
    expect(screen.getByText(/Gestion respuesta bloqueada/i)).toBeInTheDocument();
    expect(screen.getByText(/IdTareaWf:\s*924/i)).toBeInTheDocument();
    expect(screen.queryByText("Gestion")).not.toBeInTheDocument();
  });

  test("cierra panel y retorna a bandeja cuando la consulta falla", async () => {
    setHookState({
      estrucTuraRespuesta: null,
      error: new Error("HTTP 500"),
    });

    renderGestionCorrespondencia("/dashboard/gestion-correspondencia/respuesta/924");

    expect(screen.getByTestId("gestion-correspondencia-blocked-state")).toBeInTheDocument();
    expect(screen.getByText(/HTTP 500/i)).toBeInTheDocument();
    expect(screen.getByText(/IdTareaWf:\s*924/i)).toBeInTheDocument();
  });

  test("cierra panel cuando el idTareaWf es invalido", async () => {
    renderGestionCorrespondencia("/dashboard/gestion-correspondencia/respuesta/no-valido");

    await waitFor(() => {
      expect(screen.queryByTestId("gestion-correspondencia-detail-region")).not.toBeInTheDocument();
    });
    expect(screen.getByText(/Mocked GestionCorrespondenciaRoutePage/i)).toBeInTheDocument();
  });

  test("permite retornar a bandeja desde estado bloqueado", () => {
    setHookState({
      estrucTuraRespuesta: null,
      isEmpty: true,
    });

    renderGestionCorrespondencia("/dashboard/gestion-correspondencia/respuesta/924");

    fireEvent.click(screen.getByRole("button", { name: /Volver a bandeja/i }));

    expect(screen.queryByTestId("gestion-correspondencia-detail-region")).not.toBeInTheDocument();
    expect(screen.getByText(/Mocked GestionCorrespondenciaRoutePage/i)).toBeInTheDocument();
  });

  test("renderiza contenido operativo cuando la estructura esta lista", () => {
    setHookState({});

    renderGestionCorrespondencia("/dashboard/gestion-correspondencia/respuesta/924");

    expect(screen.queryByTestId("gestion-correspondencia-loading-state")).not.toBeInTheDocument();
    expect(screen.queryByTestId("gestion-correspondencia-blocked-state")).not.toBeInTheDocument();
    expect(screen.getByText("Gestion")).toBeInTheDocument();
    expect(screen.getByText("Documentos")).toBeInTheDocument();
  });
});
