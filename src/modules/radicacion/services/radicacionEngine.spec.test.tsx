import { describe, expect, it } from "vitest";
import { render, screen } from "@testing-library/react";
import { renderHook, act } from "@testing-library/react";
import { mapPlantillaToFieldConfig } from "./radicacionMetadataMapper";
import { serializeRadicacionPayload } from "./radicacionPayloadSerializer";
import { useRadicacionDynamicForm } from "../hooks/useRadicacionDynamicForm";
import { RadicacionDynamicRenderer } from "../components/RadicacionDynamicRenderer";
import type { PlantillaRadicadoDTO } from "../models/PlantillaRadicadoDTO";

const plantilla: PlantillaRadicadoDTO = {
  IdPlantillaRadicado: 10,
  NombrePlantilla: "Ingreso",
  DetallePlantillaRadicadoDTO: [
    {
      IdDetallePlantillaRadicado: 2,
      NombreCampo: "descripcion",
      Etiqueta: "Descripción",
      TipoCampo: "textarea",
      Requerido: true,
      Orden: 2,
      Placeholder: "Ingrese descripción",
      ValorDefecto: "",
    },
    {
      IdDetallePlantillaRadicado: 1,
      NombreCampo: "asunto",
      Etiqueta: "Asunto",
      TipoCampo: "text",
      Requerido: true,
      Orden: 1,
      ValorDefecto: "Inicial",
    },
  ],
  CamposPlantillaValidacionDTO: [
    {
      IdCampoPlantillaValidacion: 100,
      TipoValidacion: "maxLength",
      MensajeValidacion: "Máximo 120",
      Parametro: "120",
    },
  ],
  RelCamposValRadicDTO: [
    {
      IdDetallePlantillaRadicado: 1,
      IdCampoPlantillaValidacion: 100,
    },
  ],
};

describe("radicacion engine", () => {
  it("maps metadata to ordered UI configuration", () => {
    const fields = mapPlantillaToFieldConfig(plantilla);

    expect(fields[0]?.name).toBe("asunto");
    expect(fields[0]?.validations).toHaveLength(1);
    expect(fields[1]?.type).toBe("textarea");
  });

  it("handles dynamic value state and serializes payload", () => {
    const { result } = renderHook(() => useRadicacionDynamicForm(plantilla));

    act(() => {
      result.current.setFieldValue("asunto", "Nuevo asunto");
      result.current.setFieldValue("descripcion", "Detalle interno");
    });

    expect(result.current.getInputValue("asunto")).toBe("Nuevo asunto");

    const payload = result.current.serialize();
    expect(payload.Campos).toEqual([
      {
        IdDetallePlantillaRadicado: 1,
        NombreCampo: "asunto",
        Valor: "Nuevo asunto",
      },
      {
        IdDetallePlantillaRadicado: 2,
        NombreCampo: "descripcion",
        Valor: "Detalle interno",
      },
    ]);
  });

  it("renders dynamic inputs using component registry", () => {
    const fields = mapPlantillaToFieldConfig(plantilla);
    const values = { asunto: "", descripcion: "" };
    const onChange = () => undefined;

    render(
      <RadicacionDynamicRenderer fields={fields} values={values} onChange={onChange} />,
    );

    const asuntoInput = screen.getByLabelText("Asunto");
    const descripcionInput = screen.getByLabelText("Descripción");

    expect(asuntoInput.tagName).toBe("INPUT");
    expect(descripcionInput.tagName).toBe("TEXTAREA");
  });

  it("serializes values by metadata field id", () => {
    const fields = mapPlantillaToFieldConfig(plantilla);
    const payload = serializeRadicacionPayload(fields, {
      asunto: "Memo",
      descripcion: "Contenido",
    });

    expect(payload.Campos[0]?.IdDetallePlantillaRadicado).toBe(1);
    expect(payload.Campos[1]?.Valor).toBe("Contenido");
  });
});
