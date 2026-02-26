import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { renderHook, waitFor } from "@testing-library/react";
import React from "react";
import type { ReactNode } from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import {
  useAutocompleteCamposPlantilla,
  buildAutocompletePayload,
  normalizeAutoCompleteItems,
  resolveAutocompleteEndpoint,
} from "./useAutocompleteCamposPlantilla";

const { postMock } = vi.hoisted(() => ({
  postMock: vi.fn(),
}));

vi.mock("../../../api/Clienteaxios", () => ({
  default: {
    post: postMock,
  },
}));

const createWrapper = () => {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: { retry: false },
    },
  });

  return ({ children }: { children: ReactNode }) => (
    React.createElement(QueryClientProvider, { client: queryClient }, children)
  );
};

describe("useAutocompleteCamposPlantilla endpoint resolver", () => {
  beforeEach(() => {
    postMock.mockReset();
  });

  it("[SPEC:RMT-001] usa endpoint de tercero para REMITENTE_COR", () => {
    expect(resolveAutocompleteEndpoint("REMITENTE_COR")).toBe(
      "/api/PlantillaRadicado/autoCompleteTercero",
    );
    expect(resolveAutocompleteEndpoint(" remitente_cor ")).toBe(
      "/api/PlantillaRadicado/autoCompleteTercero",
    );
  });

  it("[SPEC:RMT-002] usa endpoint default para otros campos", () => {
    expect(resolveAutocompleteEndpoint("ANEXOS_COR")).toBe(
      "/api/PlantillaRadicado/solicitaAutoCompleteCampos",
    );
  });

  it("[SPEC:DSR-001] usa endpoint de restriccion para DESTINATARIO_COR", () => {
    expect(resolveAutocompleteEndpoint("DESTINATARIO_COR")).toBe(
      "/api/PlantillaRadicado/solicitaAutoCompleteDestinatarioRestriccion",
    );
    expect(resolveAutocompleteEndpoint(" Destinatario_Cor ")).toBe(
      "/api/PlantillaRadicado/solicitaAutoCompleteDestinatarioRestriccion",
    );
  });

  it("[SPEC:RMT-004] construye payload de tercero para REMITENTE_COR", () => {
    const endpoint = resolveAutocompleteEndpoint("REMITENTE_COR");
    const payload = buildAutocompletePayload(endpoint, {
      TextoBuscado: "mi",
      defaultDbAlias: "",
      tbl_control: "RAD_GESTION",
      name_campo: "REMITENTE_COR",
      idScript: 84,
    });

    expect(payload).toEqual({
      idScript: 84,
      nombreCampo: "REMITENTE_COR",
      valueCampo: "mi",
    });
  });

  it("[SPEC:RMT-005] mantiene payload legacy para otros campos", () => {
    const endpoint = resolveAutocompleteEndpoint("ANEXOS_COR");
    const payload = buildAutocompletePayload(endpoint, {
      TextoBuscado: "55",
      defaultDbAlias: "",
      tbl_control: "rad_gestion",
      name_campo: "ANEXOS_COR",
    });

    expect(payload).toEqual({
      TextoBuscado: "55",
      defaultDbAlias: "",
      tbl_control: "rad_gestion",
      name_campo: "ANEXOS_COR",
    });
  });

  it("[SPEC:DSR-002] construye payload restringido para DESTINATARIO_COR", () => {
    const endpoint = resolveAutocompleteEndpoint("DESTINATARIO_COR");
    const payload = buildAutocompletePayload(endpoint, {
      TextoBuscado: "cam",
      defaultDbAlias: "",
      tbl_control: "RAD_GESTION",
      name_campo: "Destinatario_Cor",
      idScript: 321,
      CDeRelacionEstadoRetriccionDto: {
        IdRestriTipoDestInterno: 0,
        IdTipoRestriccion: 0,
        DescripcionTipo: "",
        MoluloRadicacion: 0,
        ModuloRadicacionSimple: 0,
        ModuloRadicacionInterna: 0,
      },
    });

    expect(payload).toEqual({
      ValueAuto: "cam",
      CDeRelacionEstadoRetriccionDto: {
        IdRestriTipoDestInterno: 0,
        IdTipoRestriccion: 0,
        DescripcionTipo: "",
        MoluloRadicacion: 0,
        ModuloRadicacionSimple: 0,
        ModuloRadicacionInterna: 0,
      },
    });
  });

  it("[SPEC:RMT-006] normaliza respuesta con estructura Data/valueCampo", () => {
    const items = normalizeAutoCompleteItems({
      Data: [
        { idTercero: 101, valueCampo: "MIGUEL URUETA" },
        { Id: "202", Value: "MARIA VICTORIA" },
      ],
    });

    expect(items).toEqual([
      { idValue: "101", texValue: "MIGUEL URUETA" },
      { idValue: "202", texValue: "MARIA VICTORIA" },
    ]);
  });

  it("[SPEC:DSR-006] al escribir en Destinatario_Cor invoca API de restriccion", async () => {
    postMock.mockResolvedValue({
      data: {
        success: true,
        message: "OK",
        data: [{ idValue: "44", texValue: "Carlos Ruiz" }],
      },
    });

    const restriccion = {
      IdRestriTipoDestInterno: 0,
      IdTipoRestriccion: 0,
      DescripcionTipo: "",
      MoluloRadicacion: 0,
      ModuloRadicacionSimple: 0,
      ModuloRadicacionInterna: 0,
    };

    const { result } = renderHook(
      () =>
        useAutocompleteCamposPlantilla(
          {
            TextoBuscado: "car",
            defaultDbAlias: "",
            tbl_control: "RAD_GESTION",
            name_campo: "Destinatario_Cor",
            CDeRelacionEstadoRetriccionDto: restriccion,
          },
          true,
        ),
      { wrapper: createWrapper() },
    );

    await waitFor(() => {
      expect(postMock).toHaveBeenCalledTimes(1);
    });
    await waitFor(() => {
      expect(result.current.data).toEqual([
        { idValue: "44", texValue: "Carlos Ruiz" },
      ]);
    });

    expect(postMock).toHaveBeenCalledWith(
      "/api/PlantillaRadicado/solicitaAutoCompleteDestinatarioRestriccion",
      {
        ValueAuto: "car",
        CDeRelacionEstadoRetriccionDto: {
          ...restriccion,
        },
      },
    );
  });
});
