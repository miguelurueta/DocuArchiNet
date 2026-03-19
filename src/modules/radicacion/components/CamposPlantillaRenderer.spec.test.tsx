import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { CamposPlantillaRenderer } from "./CamposPlantillaRenderer";
import type { CampoPlantillaDTO } from "../models/CampoPlantillaDTO";
import styles from "../style/FormRadicacion.module.css";

const baseCampo: CampoPlantillaDTO = {
  Tupcae_label: "",
  label_input_class_font: null,
  Place_Holder: null,
  control_input_class: null,
  name_campo: "campo_base",
  aleas_campo: "Campo Base",
  title_control: "Titulo Base",
  tipo_control: null,
  value_campo: "",
  obligatorio_campo: 0,
  disable_campo: 0,
  tipo_campo: "texto",
  max_leng_campo: 20,
  campo_tip: 1,
  control_tip_correo: 0,
  error_gestion: "",
  tooltipAyuda: "Ayuda base",
  onChangeAction: null,
  serviceName: null,
  apiMethod: "GET",
  placeholder: null,
  TagSesion: "grupo-1",
  ComportamientoCampo: null,
  dataClear: null,
  event_control: null,
  ilist_row_drowlist: null,
  config_service_drowlis: null,
  config_service_controls_error: null,
  Item_Tom_Select: null,
  Item_Tom_row: null,
  CamposUpdateIndiceBach: null,
  TomPParameterTomSelelect: null,
};

describe("CamposPlantillaRenderer", () => {
  it("[SPEC:PL-001] renderiza campos filtrados y control select/autocomplete", () => {
    const campos: CampoPlantillaDTO[] = [
      {
        ...baseCampo,
        name_campo: "tipo_tramite",
        aleas_campo: "Tipo Trámite",
        ComportamientoCampo: "SELECCION",
        obligatorio_campo: 1,
        disable_campo: 1,
        max_leng_campo: 12,
        ilist_row_drowlist: [
          { id_value: "1", value_campo: "Opcion 1" },
          { id_value: "2", value_campo: "Opcion 2" },
        ],
      },
      {
        ...baseCampo,
        name_campo: "correo",
        aleas_campo: "Correo",
        ComportamientoCampo: "AUTOCOMPLETE",
        control_tip_correo: 1,
      },
      {
        ...baseCampo,
        name_campo: "no_visible",
        campo_tip: 0,
      },
    ];

    const { container } = render(
      <CamposPlantillaRenderer camposPlantilla={campos} />,
    );

    expect(screen.getByText("Tipo Trámite")).toBeInTheDocument();
    expect(screen.getByText("Correo")).toBeInTheDocument();
    expect(screen.queryByText("no_visible")).not.toBeInTheDocument();
    expect(screen.getByText("Tipo Trámite")).toHaveClass(styles.labelCapitalize);
    expect(screen.getByText("Correo")).toHaveClass(styles.labelCapitalize);

    const selectWrapper = container.querySelector(
      '[data-ident="pl-radicacion-spe-tipo_tramite"]',
    );
    expect(selectWrapper).toBeTruthy();

    const selectInput = screen.getByRole("combobox", {
      name: "Tipo Trámite",
    });
    expect(selectInput).toHaveAttribute("aria-required", "true");
    expect(selectInput).toBeDisabled();

    const input = screen.getByRole("textbox", { name: "Correo" });
    expect(input).toHaveAttribute("type", "email");
  });

  it("[SPEC:PL-002] aplica accesibilidad, tooltip y validaciones", () => {
    const handleChange = vi.fn();
    const campos: CampoPlantillaDTO[] = [
      {
        ...baseCampo,
        name_campo: "fecha_inicio",
        aleas_campo: "Fecha Inicio",
        ComportamientoCampo: "AUTOCOMPLETE",
        tipo_campo: "FECHA",
        tooltipAyuda: "Selecciona una fecha",
      },
      {
        ...baseCampo,
        name_campo: "cantidad",
        aleas_campo: "Cantidad",
        ComportamientoCampo: "AUTOCOMPLETE",
        tipo_campo: "NUMERO",
        tooltipAyuda: "",
      },
    ];

    render(
      <CamposPlantillaRenderer
        camposPlantilla={campos}
        onChange={handleChange}
      />,
    );

    const fechaInput = screen.getByLabelText("Fecha Inicio", {
      selector: "input",
    });
    expect(fechaInput).toHaveAttribute("type", "date");
    expect(
      screen.getByLabelText("Mostrar ayuda para Fecha Inicio"),
    ).toBeInTheDocument();
    expect(screen.getByText("Fecha Inicio")).toHaveClass(styles.labelCapitalize);

    const cantidadInput = screen.getByRole("spinbutton", { name: "Cantidad" });
    expect(cantidadInput).toHaveAttribute("type", "number");
    expect(cantidadInput).toHaveAttribute("pattern", "^[0-9]+$");
    expect(screen.getByText("Cantidad")).toHaveClass(styles.labelCapitalize);
  });

  it("[SPEC:RBK-002] mantiene onChange estable cuando el valor se limpia con backspace", () => {
    const handleChange = vi.fn();
    const campos: CampoPlantillaDTO[] = [
      {
        ...baseCampo,
        name_campo: "asunto_dinamico",
        aleas_campo: "Asunto Dinamico",
        ComportamientoCampo: "AUTOCOMPLETE",
      },
    ];

    render(
      <CamposPlantillaRenderer
        camposPlantilla={campos}
        onChange={handleChange}
      />,
    );

    const input = screen.getByRole("textbox", { name: "Asunto Dinamico" });
    fireEvent.change(input, { target: { value: "ABC" } });
    fireEvent.change(input, { target: { value: "" } });

    expect(handleChange).toHaveBeenCalledWith(
      "ABC",
      expect.objectContaining({ name_campo: "asunto_dinamico" }),
    );
    expect(handleChange).toHaveBeenCalledWith(
      "",
      expect.objectContaining({ name_campo: "asunto_dinamico" }),
    );
  });
});
