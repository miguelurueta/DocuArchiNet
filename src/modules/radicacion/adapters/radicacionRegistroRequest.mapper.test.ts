import { describe, expect, it } from "vitest";
import type { CampoPlantillaDTO } from "../models/CampoPlantillaDTO";
import type { PlantillaRadicadoDTO } from "../models/PlantillaRadicadoDTO";
import { buildRegistrarRadicacionEntranteRequest } from "./radicacionRegistroRequest.mapper";

const plantilla: PlantillaRadicadoDTO = {
  IdPlantillaRadicado: 9,
  NombrePlantilla: "Entrada",
  DetallePlantillaRadicadoDTO: [
    {
      IdDetallePlantillaRadicado: 1,
      NombreCampo: "ASUNTO",
      Etiqueta: "Asunto",
      TipoCampo: "texto",
      Requerido: true,
      Orden: 1,
    },
    {
      IdDetallePlantillaRadicado: 2,
      NombreCampo: "ANEXOS_COR",
      Etiqueta: "Anexos",
      TipoCampo: "texto",
      Requerido: false,
      Orden: 2,
    },
  ],
  CamposPlantillaValidacionDTO: [],
  RelCamposValRadicDTO: [],
};

const campos = [
  {
    name_campo: "TipoRadicado",
    ilist_row_drowlist: [{ idValue: "5", Value: "Entrada" }],
  },
  {
    name_campo: "Descripcion_Documento",
    ilist_row_drowlist: [{ idValue: "3", Value: "PQRS" }],
  },
  { name_campo: "ASUNTO" },
  { name_campo: "ANEXOS_COR" },
] as unknown as CampoPlantillaDTO[];

describe("radicacionRegistroRequest.mapper", () => {
  it("[SPEC:FE-01] construye request con datos completos del formulario", () => {
    const request = buildRegistrarRadicacionEntranteRequest({
      plantilla,
      camposPlantilla: campos,
      flujoOptions: [{ value: "4", label: "Flujo juridico" }],
      values: {
        tipoRadicado: "5",
        tramite: "3",
        flujo: "4",
        ASUNTO: "Solicitud de prueba",
        ANEXOS_COR: "Cedula",
        remitente: [{ value: "11", label: "Juan Perez" }],
        destinatario: [{ value: "22", label: "Maria Ruiz" }],
        expedienteRelacionado: "",
      },
    });

    expect(request).toMatchObject({
      tipoModuloRadicacion: 1,
      ASUNTO: "Solicitud de prueba",
      Remitente: { Nombre: "Juan Perez", id_Dest_Ext: 11 },
      Destinatario: { Destinatario: "Maria Ruiz", id_Remit_Dest_Int: 22 },
      Tipo_tramite: { Descripcion: "PQRS", tipo_doc_entrante: 3 },
      RE_flujo_trabajo: {
        NombreFlujo: "Flujo juridico",
        id_tipo_flujo_workflow: 4,
      },
      TipoRadicado: { TipoRadicacion: "Entrada", IdTipoRadicado: 5 },
      TipoPlantillaRadicado: {
        TipoPlantillaRadicado: "Entrada",
        IdTipoPlantillaRdicado: 5,
      },
      ANEXOS_COR: "Cedula",
    });
    expect(request.Campos).toEqual(
      expect.arrayContaining([
        {
          IdDetallePlantillaRadicado: 1,
          NombreCampo: "ASUNTO",
          Valor: "Solicitud de prueba",
        },
      ]),
    );
  });

  it("[SPEC:FE-01] tolera campos opcionales vacios sin hardcodear ids", () => {
    const request = buildRegistrarRadicacionEntranteRequest({
      plantilla,
      camposPlantilla: campos,
      values: {
        tipoRadicado: "",
        tramite: "",
      },
    });

    expect(request.expedienteRelacionado).toEqual({
      Expediente: "",
      idExpediente: 0,
    });
    expect(request.radicadoRelacionados).toEqual([]);
    expect(request.numeroFolios).toBeNull();
  });

  it("[SPEC:FE-01] mapea campos obligatorios equivalentes aunque el formulario use nombres UI", () => {
    const request = buildRegistrarRadicacionEntranteRequest({
      plantilla,
      camposPlantilla: [
        ...campos,
        {
          name_campo: "Numero_Folios",
          aleas_campo: "Número Folios",
        },
        {
          name_campo: "Tipo_radicado_plantilla",
          aleas_campo: "Tipo_radicado_plantilla",
        },
      ] as unknown as CampoPlantillaDTO[],
      values: {
        tipoRadicado: "5",
        tramite: "3",
        flujo: "4",
        numeroFolios: "12",
      },
    });

    expect(request.numeroFolios).toBe(12);
    expect(request.Campos).toEqual(
      expect.arrayContaining([
        expect.objectContaining({
          NombreCampo: "Numero_Folios",
          Valor: "12",
        }),
        expect.objectContaining({
          NombreCampo: "Tipo_radicado_plantilla",
          Valor: "5",
        }),
      ]),
    );
  });

  it("[SPEC:FE-01] agrega detalles requeridos que no llegan en camposPlantilla", () => {
    const request = buildRegistrarRadicacionEntranteRequest({
      plantilla: {
        ...plantilla,
        DetallePlantillaRadicadoDTO: [
          ...plantilla.DetallePlantillaRadicadoDTO,
          {
            IdDetallePlantillaRadicado: 55,
            NombreCampo: "Tipo_radicado_plantilla",
            Etiqueta: "Tipo_radicado_plantilla",
            TipoCampo: "texto",
            Requerido: true,
            Orden: 3,
          },
          {
            IdDetallePlantillaRadicado: 56,
            NombreCampo: "Numero_Folios",
            Etiqueta: "Número Folios",
            TipoCampo: "numero",
            Requerido: true,
            Orden: 4,
          },
        ],
      },
      camposPlantilla: campos,
      values: {
        tipoRadicado: "5",
        numeroFolios: "8",
      },
    });

    expect(request.numeroFolios).toBe(8);
    expect(request.Campos).toEqual(
      expect.arrayContaining([
        {
          IdDetallePlantillaRadicado: 55,
          NombreCampo: "Tipo_radicado_plantilla",
          Valor: "5",
        },
        {
          IdDetallePlantillaRadicado: 56,
          NombreCampo: "Numero_Folios",
          Valor: "8",
        },
      ]),
    );
  });

  it("[SPEC:FE-01] calcula numeroFolios desde detalle requerido sin campoPlantilla", () => {
    const request = buildRegistrarRadicacionEntranteRequest({
      plantilla: {
        ...plantilla,
        DetallePlantillaRadicadoDTO: [
          ...plantilla.DetallePlantillaRadicadoDTO,
          {
            IdDetallePlantillaRadicado: 56,
            NombreCampo: "Numero_Folios",
            Etiqueta: "NÃºmero Folios",
            TipoCampo: "numero",
            Requerido: true,
            Orden: 4,
          },
        ],
      },
      camposPlantilla: campos,
      values: {
        Numero_Folios: "9",
      },
    });

    expect(request.numeroFolios).toBe(9);
  });

  it("[SPEC:FE-01] usa opcion TipoRadicado para TipoPlantillaRadicado", () => {
    const request = buildRegistrarRadicacionEntranteRequest({
      plantilla: {
        ...plantilla,
        IdPlantillaRadicado: 0,
        NombrePlantilla: "Plantilla Radicacion",
      },
      camposPlantilla: campos,
      values: {
        tipoRadicado: "5",
      },
    });

    expect(request.TipoRadicado).toEqual({
      TipoRadicacion: "Entrada",
      IdTipoRadicado: 5,
    });
    expect(request.TipoPlantillaRadicado).toEqual({
      TipoPlantillaRadicado: "Entrada",
      IdTipoPlantillaRdicado: 5,
    });
  });

  it("[SPEC:FE-01] no envia ModuloRegistro porque backend lo resuelve por tipoModuloRadicacion", () => {
    const request = buildRegistrarRadicacionEntranteRequest({
      plantilla,
      camposPlantilla: campos,
      values: {
        tipoRadicado: "5",
      },
    });

    expect(request).not.toHaveProperty("ModuloRegistro");
  });
});
