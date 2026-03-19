import { fireEvent, render, screen, within } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { CamposPlantillaAutoCompleteRenderer } from "./CamposPlantillaAutoCompleteRenderer";
import type { CampoPlantillaDTO } from "../models/CampoPlantillaDTO";
import { useAutocompleteCamposPlantilla } from "../hooks/useAutocompleteCamposPlantilla";
import styles from "../style/FormRadicacion.module.css";

vi.mock("../hooks/useAutocompleteCamposPlantilla", () => ({
  useAutocompleteCamposPlantilla: vi.fn(),
}));

const mockedUseAutocomplete = vi.mocked(useAutocompleteCamposPlantilla);

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
  apiMethod: null,
  placeholder: null,
  TagSesion: "grupo-1",
  ComportamientoCampo: "AUTOCOMPLETE",
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

describe("CamposPlantillaAutoCompleteRenderer", () => {
  it("[SPEC:CPS-001] renderiza campos de autocompletado y seleccion con campo_tip=1", () => {
    mockedUseAutocomplete.mockReturnValue({
      data: [{ idValue: null, texValue: "55" }],
      isLoading: false,
      isFetching: false,
      error: undefined,
    });

    const campos: CampoPlantillaDTO[] = [
      {
        ...baseCampo,
        name_campo: "codigocliente",
        aleas_campo: "CÓDIGO CLIENTE",
      },
      {
        ...baseCampo,
        name_campo: "tipo_documento",
        aleas_campo: "TIPO DOCUMENTO",
        ComportamientoCampo: "SELECCION",
        apiMethod: "getTipos",
        ilist_row_drowlist: [{ idValue: "CC", Value: "Cédula" }] as unknown as CampoPlantillaDTO["ilist_row_drowlist"],
      },
      {
        ...baseCampo,
        name_campo: "no_visible",
        campo_tip: 0,
      },
    ];

    const { container } = render(
      <CamposPlantillaAutoCompleteRenderer camposPlantilla={campos} />,
    );

    expect(
      container.querySelector('[data-ident="pl-radicacion-card-spe"]'),
    ).toBeTruthy();
    expect(
      container.querySelector('[data-ident="pl-radicacion-spe-codigocliente"]'),
    ).toBeTruthy();

    expect(screen.getByLabelText("Código Cliente")).toBeInTheDocument();
    expect(screen.getByLabelText("Tipo Documento")).toBeInTheDocument();
    expect(screen.queryByLabelText("no_visible")).not.toBeInTheDocument();
    const selectRoot = screen
      .getByLabelText("Tipo Documento")
      .closest(".ant-select");
    expect(selectRoot).toBeTruthy();
    fireEvent.mouseDown(selectRoot as HTMLElement);
    expect(screen.getByRole("option", { name: "Cédula" })).toBeInTheDocument();
    expect(screen.getByText("Código Cliente")).toHaveClass(styles.labelCapitalize);
    expect(screen.getByText("Tipo Documento")).toHaveClass(styles.labelCapitalize);
  });

  it("[SPEC:AC-002] muestra mensaje amigable cuando falla la API", () => {
    mockedUseAutocomplete.mockReturnValue({
      data: [],
      isLoading: false,
      isFetching: false,
      error: new Error("boom") as never,
    });

    render(
      <CamposPlantillaAutoCompleteRenderer
        camposPlantilla={[
          { ...baseCampo, name_campo: "codigocliente", aleas_campo: "Código Cliente" },
        ]}
      />,
    );

    expect(
      screen.getByText("No fue posible cargar las opciones. Intenta nuevamente."),
    ).toBeInTheDocument();
  });

  it("[SPEC:CPS-002] aplica atributos declarativos y tooltip en campos seleccion", () => {
    mockedUseAutocomplete.mockReturnValue({
      data: [],
      isLoading: false,
      isFetching: false,
      error: undefined,
    });

    render(
      <CamposPlantillaAutoCompleteRenderer
        camposPlantilla={[
          {
            ...baseCampo,
            name_campo: "tipo_cliente",
            aleas_campo: "TIPO CLIENTE",
            ComportamientoCampo: "SELECCION",
            obligatorio_campo: 1,
            disable_campo: 1,
            apiMethod: "getTiposCliente",
            ilist_row_drowlist: [{ id_value: "A", value_campo: "Activo" }],
          },
        ]}
      />,
    );

    const selectRoot = document.querySelector(
      '[data-ident="pl-radicacion-spe-tipo_cliente"]',
    ) as HTMLElement | null;
    expect(selectRoot).toBeTruthy();
    expect(selectRoot).toHaveAttribute("data-api-method", "getTiposCliente");
    expect(selectRoot).toHaveAttribute(
      "data-ident",
      "pl-radicacion-spe-tipo_cliente",
    );
    const selectContainer = within(selectRoot as HTMLElement).getByRole("combobox", {
      name: "Tipo Cliente",
    }).closest(".ant-select");
    expect(selectContainer).toHaveClass(styles.dynamicSelect);
    const select = within(selectRoot as HTMLElement).getByRole("combobox", {
      name: "Tipo Cliente",
    });
    expect(select).toBeInTheDocument();
    expect(select).toBeDisabled();
    expect(select).toHaveAttribute("aria-required", "true");
    expect(screen.getByLabelText("Mostrar ayuda para Tipo Cliente")).toBeInTheDocument();
    expect(within(selectRoot as HTMLElement).getByText("Seleccionar")).toBeInTheDocument();
    expect(screen.getByText("Tipo Cliente")).toHaveClass(styles.labelCapitalize);
  });

  it("[SPEC:RBK-001] permite backspace hasta valor vacio sin errores en campos dinamicos", () => {
    mockedUseAutocomplete.mockReturnValue({
      data: [],
      isLoading: false,
      isFetching: false,
      error: undefined,
    });

    const handleChange = vi.fn();

    render(
      <CamposPlantillaAutoCompleteRenderer
        camposPlantilla={[
          { ...baseCampo, name_campo: "asunto", aleas_campo: "Asunto" },
        ]}
        onChange={handleChange}
      />,
    );

    const input = screen.getByLabelText("Asunto");
    fireEvent.change(input, { target: { value: "Texto temporal" } });
    fireEvent.change(input, { target: { value: "" } });

    expect(handleChange).toHaveBeenCalledWith(
      "Texto temporal",
      expect.objectContaining({ name_campo: "asunto" }),
    );
    expect(handleChange).toHaveBeenCalledWith(
      "",
      expect.objectContaining({ name_campo: "asunto" }),
    );
  });

  it("[SPEC:RBK-006] mantiene estable el render con campos repetidos y valor vacio", () => {
    mockedUseAutocomplete.mockReturnValue({
      data: [],
      isLoading: false,
      isFetching: false,
      error: undefined,
    });

    const handleChange = vi.fn();

    render(
      <CamposPlantillaAutoCompleteRenderer
        camposPlantilla={[
          {
            ...baseCampo,
            name_campo: "Identificacion_remitente",
            aleas_campo: "Identificacion_remitente",
            TagSesion: "remitente",
          },
          {
            ...baseCampo,
            name_campo: "Identificacion_remitente",
            aleas_campo: "Identificacion_remitente",
            TagSesion: "destinatario",
          },
        ]}
        onChange={handleChange}
      />,
    );

    const inputs = screen.getAllByLabelText("Identificacion_remitente");
    expect(inputs).toHaveLength(2);

    expect(() => {
      fireEvent.change(inputs[0], { target: { value: "12345" } });
      fireEvent.change(inputs[0], { target: { value: "" } });
    }).not.toThrow();

    expect(handleChange).toHaveBeenCalledWith(
      "",
      expect.objectContaining({ name_campo: "Identificacion_remitente" }),
    );
  });

  it("[SPEC:ACS-001] aplica clase de estilo consistente a autocomplete dinamico", () => {
    mockedUseAutocomplete.mockReturnValue({
      data: [],
      isLoading: false,
      isFetching: false,
      error: undefined,
    });

    const { container } = render(
      <CamposPlantillaAutoCompleteRenderer
        camposPlantilla={[
          { ...baseCampo, name_campo: "placa", aleas_campo: "Campo Placa" },
        ]}
      />,
    );

    const autoCompleteRoot = container.querySelector(
      '.ant-select[data-ident="pl-radicacion-spe-placa"]',
    );
    expect(autoCompleteRoot).toBeTruthy();
    expect(autoCompleteRoot).toHaveClass(styles.dynamicAutocomplete);
  });
});
