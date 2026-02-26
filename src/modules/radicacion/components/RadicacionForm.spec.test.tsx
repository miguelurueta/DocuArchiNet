import { describe, expect, it, vi, beforeEach, afterEach } from "vitest";
import { render, screen, fireEvent, act } from "@testing-library/react";
import RadicacionForm from "./RadicacionForm";
import { useCamposPlantilla } from "../hooks/useCamposPlantilla";
import { useAutocompleteCamposPlantilla } from "../hooks/useAutocompleteCamposPlantilla";
import { useFlujosRelacionadosTramite } from "../hooks/useFlujosRelacionadosTramite";
import { useEstructuraRelacionTipoRestriccion } from "../hooks/useEstructuraRelacionTipoRestriccion";
import type { CampoPlantillaDTO } from "../models/CampoPlantillaDTO";

vi.mock("../hooks/useCamposPlantilla", () => ({
  useCamposPlantilla: vi.fn(),
}));

vi.mock("../hooks/useAutocompleteCamposPlantilla", () => ({
  useAutocompleteCamposPlantilla: vi.fn(),
}));
vi.mock("../hooks/useFlujosRelacionadosTramite", () => ({
  useFlujosRelacionadosTramite: vi.fn(),
}));
vi.mock("../hooks/useEstructuraRelacionTipoRestriccion", () => ({
  useEstructuraRelacionTipoRestriccion: vi.fn(),
}));

const mockedUseCamposPlantilla = vi.mocked(useCamposPlantilla);
const mockedUseAutocompleteCamposPlantilla = vi.mocked(
  useAutocompleteCamposPlantilla,
);
const mockedUseFlujosRelacionadosTramite = vi.mocked(
  useFlujosRelacionadosTramite,
);
const mockedUseEstructuraRelacionTipoRestriccion = vi.mocked(
  useEstructuraRelacionTipoRestriccion,
);

describe("RadicacionForm", () => {
  beforeEach(() => {
    mockedUseCamposPlantilla.mockReturnValue({
      data: [],
      isLoading: false,
      error: null,
      refetch: vi.fn(),
    });

    mockedUseAutocompleteCamposPlantilla.mockReturnValue({
      data: [],
      isLoading: false,
      isFetching: false,
      error: null,
    });
    mockedUseFlujosRelacionadosTramite.mockReturnValue({
      data: [],
      isLoading: false,
      isFetching: false,
      error: null,
      shouldFetch: false,
    });
    mockedUseEstructuraRelacionTipoRestriccion.mockReturnValue({
      data: {
        IdRestriTipoDestInterno: 0,
        IdTipoRestriccion: 0,
        DescripcionTipo: "",
        MoluloRadicacion: 0,
        ModuloRadicacionSimple: 0,
        ModuloRadicacionInterna: 0,
      },
      isLoading: false,
      isFetching: false,
      error: null,
      shouldFetch: false,
    });
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it("[SPEC:RAD-001] muestra opciones dinámicas de trámite desde plantilla", () => {
    mockedUseCamposPlantilla.mockReturnValue({
      data: [
        {
          name_campo: "Descripcion_Documento",
          aleas_campo: "Trámite",
          title_control: "Título Trámite",
          tooltipAyuda: "Ayuda Trámite",
          ilist_row_drowlist: [
            { idValue: 23, Value: "CITACION" },
            { idValue: 37, Value: "CREDIRENTA LIBRAZA" },
          ],
        } as unknown as CampoPlantillaDTO,
        {
          name_campo: "RE_flujo_trabajo",
          aleas_campo: "Flujo Trámite",
          title_control: "Título Flujo",
          tooltipAyuda: "Ayuda Flujo",
        } as unknown as CampoPlantillaDTO,
      ],
      isLoading: false,
      error: null,
      refetch: vi.fn(),
    });

    render(<RadicacionForm />);

    const select = screen.getByTestId("ra_tipo_tramite_select");
    fireEvent.mouseDown(select);

    expect(screen.getByText("CITACION")).toBeInTheDocument();
    expect(screen.getByText("CREDIRENTA LIBRAZA")).toBeInTheDocument();
    expect(
      screen.getByLabelText("Mostrar ayuda para Trámite"),
    ).toBeInTheDocument();
    expect(screen.getByText("Trámite")).toHaveAttribute(
      "title",
      "Título Trámite",
    );
    expect(
      screen.getByLabelText("Mostrar ayuda para Flujo Trámite"),
    ).toBeInTheDocument();
    expect(screen.getByText("Flujo Trámite")).toHaveAttribute(
      "title",
      "Título Flujo",
    );
  });

  it("[SPEC:RAD-002] no muestra opciones si plantilla no trae opciones", () => {
    render(<RadicacionForm />);

    const select = screen.getByTestId("ra_tipo_tramite_select");
    fireEvent.mouseDown(select);

    expect(
      screen.queryByText("Correo Electrónico"),
    ).not.toBeInTheDocument();
    expect(screen.queryByText("PQRS")).not.toBeInTheDocument();
  });

  it("[SPEC:RAD-003] llena opciones del select TipoRadicado desde plantilla", () => {
    mockedUseCamposPlantilla.mockReturnValue({
      data: [
        {
          name_campo: "TipoRadicado",
          aleas_campo: "Tipo Radicado",
          title_control: "Título Tipo Radicado",
          tooltipAyuda: "Ayuda Tipo Radicado",
          ilist_row_drowlist: [
            { idValue: "1", Value: "Interno" },
            { idValue: "2", Value: "Externo" },
          ],
        } as unknown as CampoPlantillaDTO,
      ],
      isLoading: false,
      error: null,
      refetch: vi.fn(),
    });

    render(<RadicacionForm />);

    const select = screen.getByTestId("ra_tipo_radicado_select");
    fireEvent.mouseDown(select);

    expect(screen.getByText("Seleccionar")).toBeInTheDocument();
    expect(screen.getByText("Interno")).toBeInTheDocument();
    expect(screen.getByText("Externo")).toBeInTheDocument();
    expect(
      screen.getByLabelText("Mostrar ayuda para Tipo Radicado"),
    ).toBeInTheDocument();
  });

  it("[SPEC:MR-001] elimina el campo estatico de medio de recepcion", () => {
    const { container } = render(<RadicacionForm />);

    expect(
      container.querySelector('[data-ident="pl-radicacion-spe-Medio-recep"]'),
    ).toBeNull();
  });

  it("[SPEC:MR-002] renderiza medio de recepcion solo desde campos dinamicos", () => {
    mockedUseCamposPlantilla.mockReturnValue({
      data: [
        {
          name_campo: "MEDIORECEPCION",
          aleas_campo: "Tipo de recepción",
          campo_tip: 1,
          ComportamientoCampo: "SELECCION",
          obligatorio_campo: 1,
          disable_campo: 0,
          ilist_row_drowlist: [
            { id_value: "6", value_campo: "CORREO ELECTRONICO" },
            { id_value: "7", value_campo: "CORREO FISICO" },
          ],
        } as unknown as CampoPlantillaDTO,
      ],
      isLoading: false,
      error: null,
      refetch: vi.fn(),
    });

    const { container } = render(<RadicacionForm />);

    expect(
      container.querySelector('[data-ident="pl-radicacion-spe-Medio-recep"]'),
    ).toBeNull();
    expect(
      container.querySelector('[data-ident="pl-radicacion-card-spe"]'),
    ).toBeTruthy();
    expect(
      container.querySelector('[data-ident="pl-radicacion-spe-MEDIORECEPCION"]'),
    ).toBeTruthy();
  });

  it("[SPEC:RAD-006] renderiza title y tooltip para FECHALIMITERESPUESTA desde plantilla", () => {
    mockedUseCamposPlantilla.mockReturnValue({
      data: [
        {
          name_campo: "FECHALIMITERESPUESTA",
          aleas_campo: "Fecha Límite Respuesta",
          title_control: "Título Fecha Límite",
          tooltipAyuda: "Ayuda Fecha Límite",
        } as unknown as CampoPlantillaDTO,
      ],
      isLoading: false,
      error: null,
      refetch: vi.fn(),
    });

    render(<RadicacionForm />);

    expect(screen.getByText("Fecha Límite Respuesta")).toHaveAttribute(
      "title",
      "Título Fecha Límite",
    );
    expect(
      screen.getByLabelText("Mostrar ayuda para Fecha Límite Respuesta"),
    ).toBeInTheDocument();
  });

  it("[SPEC:RAD-007] mantiene DatePicker de FECHALIMITERESPUESTA con atributos declarativos", () => {
    mockedUseCamposPlantilla.mockReturnValue({
      data: [
        {
          name_campo: "FECHALIMITERESPUESTA",
          aleas_campo: "Fecha Límite Respuesta",
          title_control: "Título Fecha Límite",
          tooltipAyuda: "Ayuda Fecha Límite",
        } as unknown as CampoPlantillaDTO,
      ],
      isLoading: false,
      error: null,
      refetch: vi.fn(),
    });

    render(<RadicacionForm />);

    const datePicker = screen.getByTestId("ra_fecha_limite_picker");
    expect(datePicker).toHaveAttribute(
      "data-ident",
      "pl-radicacion-spe-FECHALIMITERESPUESTA",
    );
    expect(datePicker).toHaveAttribute(
      "aria-describedby",
      "pl-radicacion-spe-tooltip-FECHALIMITERESPUESTA",
    );
  });

  it("[SPEC:RAD-008] prioriza campo title para FECHALIMITERESPUESTA y mantiene tooltipAyuda", () => {
    mockedUseCamposPlantilla.mockReturnValue({
      data: [
        {
          name_campo: "FECHALIMITERESPUESTA",
          aleas_campo: "Fecha Límite Respuesta",
          title: "Título desde title",
          title_control: "Título desde title_control",
          tooltipAyuda: "Ayuda Fecha Límite",
        } as unknown as CampoPlantillaDTO,
      ],
      isLoading: false,
      error: null,
      refetch: vi.fn(),
    });

    render(<RadicacionForm />);

    expect(screen.getByText("Fecha Límite Respuesta")).toHaveAttribute(
      "title",
      "Título desde title",
    );
    expect(
      screen.getByLabelText("Mostrar ayuda para Fecha Límite Respuesta"),
    ).toBeInTheDocument();
  });

  it("[SPEC:RAD-004] renderiza autocompletado de ANEXOS_COR y consulta la API con los parámetros correctos", async () => {
    vi.useFakeTimers();

    mockedUseCamposPlantilla.mockReturnValue({
      data: [
        {
          name_campo: "ANEXOS_COR",
          aleas_campo: "Anexos Cor",
          campo_tip: 1,
          ComportamientoCampo: "AUTOCOMPLETE",
          tbl_control: "rad_gestion",
          obligatorio_campo: 1,
          disable_campo: 0,
          title_control: "Título Anexos",
          tooltipAyuda: "Ayuda Anexos",
          placeholder: "Buscar anexos",
        } as unknown as CampoPlantillaDTO,
      ],
      isLoading: false,
      error: null,
      refetch: vi.fn(),
    });

    mockedUseAutocompleteCamposPlantilla.mockReturnValue({
      data: [{ idValue: null, texValue: "55" }],
      isLoading: false,
      isFetching: false,
      error: null,
    });

    render(<RadicacionForm />);

    const [input] = screen.getAllByLabelText("Anexos Cor");
    fireEvent.change(input, { target: { value: "55" } });

    await act(async () => {
      vi.advanceTimersByTime(300);
    });

    expect(mockedUseAutocompleteCamposPlantilla).toHaveBeenLastCalledWith(
      {
        TextoBuscado: "55",
        defaultDbAlias: "",
        tbl_control: "rad_gestion",
        name_campo: "ANEXOS_COR",
      },
      true,
    );

  });

  it("[SPEC:RAD-005] muestra mensaje de error cuando falla el autocompletado", () => {
    mockedUseCamposPlantilla.mockReturnValue({
      data: [
        {
          name_campo: "ANEXOS_COR",
          aleas_campo: "Anexos Cor",
          campo_tip: 1,
        ComportamientoCampo: "AUTOCOMPLETE",
        tbl_control: "rad_gestion",
          obligatorio_campo: 1,
          disable_campo: 0,
        } as unknown as CampoPlantillaDTO,
      ],
      isLoading: false,
      error: null,
      refetch: vi.fn(),
    });

    mockedUseAutocompleteCamposPlantilla.mockReturnValue({
      data: [],
      isLoading: false,
      isFetching: false,
      error: new Error("boom"),
    });

    render(<RadicacionForm />);

    expect(
      screen.getAllByText(
        "No fue posible cargar las opciones. Intenta nuevamente.",
      ).length,
    ).toBeGreaterThan(0);
  });

  it("[SPEC:ASA-001] habilita autocompletado en ASUNTO usando metadata de camposPlantilla", async () => {
    vi.useFakeTimers();

    mockedUseCamposPlantilla.mockReturnValue({
      data: [
        {
          name_campo: "ASUNTO",
          aleas_campo: "Asunto",
          campo_tip: 1,
          ComportamientoCampo: "AUTOCOMPLETE",
          tbl_control: "rad_gestion",
          obligatorio_campo: 1,
          disable_campo: 0,
          title_control: "Titulo Asunto",
          tooltipAyuda: "Ayuda Asunto",
          placeholder: "Buscar asunto",
        } as unknown as CampoPlantillaDTO,
      ],
      isLoading: false,
      error: null,
      refetch: vi.fn(),
    });

    mockedUseAutocompleteCamposPlantilla.mockReturnValue({
      data: [{ idValue: null, texValue: "Asunto de prueba" }],
      isLoading: false,
      isFetching: false,
      error: null,
    });

    render(<RadicacionForm />);

    const input = screen.getByLabelText("Asunto");
    fireEvent.change(input, { target: { value: "Asunto de prueba" } });

    await act(async () => {
      vi.advanceTimersByTime(300);
    });

    expect(mockedUseAutocompleteCamposPlantilla).toHaveBeenLastCalledWith(
      {
        TextoBuscado: "Asunto de prueba",
        defaultDbAlias: "",
        tbl_control: "rad_gestion",
        name_campo: "ASUNTO",
      },
      true,
    );
    expect(input).toBeInTheDocument();
  });

  it("[SPEC:ASA-002] ante error del autocompletado de ASUNTO mantiene ingreso manual", () => {
    mockedUseCamposPlantilla.mockReturnValue({
      data: [
        {
          name_campo: "ASUNTO",
          aleas_campo: "Asunto",
          campo_tip: 1,
          ComportamientoCampo: "AUTOCOMPLETE",
          tbl_control: "rad_gestion",
          obligatorio_campo: 1,
          disable_campo: 0,
        } as unknown as CampoPlantillaDTO,
      ],
      isLoading: false,
      error: null,
      refetch: vi.fn(),
    });

    mockedUseAutocompleteCamposPlantilla.mockReturnValue({
      data: [],
      isLoading: false,
      isFetching: false,
      error: new Error("boom"),
    });

    render(<RadicacionForm />);

    expect(
      screen.getByText("No fue posible cargar las opciones. Intenta nuevamente."),
    ).toBeInTheDocument();

    const input = screen.getByLabelText("Asunto") as HTMLInputElement;
    fireEvent.change(input, { target: { value: "Texto manual" } });
    expect(input.value).toBe("Texto manual");
  });

  it("[SPEC:DSC-001] aplica metadata de Destinatario_Cor (required, disabled, title y tooltip)", () => {
    mockedUseCamposPlantilla.mockReturnValue({
      data: [
        {
          name_campo: " destinatario_cor ",
          aleas_campo: "Destino Corporativo",
          title_control: "Título Destinatario",
          tooltipAyuda: "Ayuda Destinatario",
          obligatorio_campo: 1,
          disable_campo: 1,
        } as unknown as CampoPlantillaDTO,
      ],
      isLoading: false,
      error: null,
      refetch: vi.fn(),
    });

    const { container } = render(<RadicacionForm />);

    const destinatarioLabel = screen
      .getByText("Destino Corporativo")
      .closest("label");
    expect(destinatarioLabel).toBeTruthy();
    expect(destinatarioLabel?.className).toContain("ant-form-item-required");
    expect(screen.getByText("Destino Corporativo")).toHaveAttribute(
      "title",
      "Título Destinatario",
    );
    expect(
      screen.getByLabelText("Mostrar ayuda para Destino Corporativo"),
    ).toBeInTheDocument();

    const destinatarioSelect = container.querySelector(
      '.ant-select[data-ident="pl-radicacion-spe-Destinatario_Cor"]',
    );
    expect(destinatarioSelect?.className).toContain("ant-select-disabled");
  });

  it("[SPEC:DSR-003] consulta autocomplete restringido para Destinatario_Cor con idScript de plantilla", () => {
    mockedUseCamposPlantilla.mockReturnValue({
      data: [
        {
          name_campo: "DESTINATARIO_COR",
          aleas_campo: "Destinatario",
          campo_tip: 1,
          ComportamientoCampo: "AUTOCOMPLETE",
          tbl_control: "terceros",
          obligatorio_campo: 1,
          disable_campo: 0,
          TomPParameterTomSelelect: {
            id_escript: 654,
          },
        } as unknown as CampoPlantillaDTO,
      ],
      isLoading: false,
      error: null,
      refetch: vi.fn(),
    });

    mockedUseAutocompleteCamposPlantilla.mockReturnValue({
      data: [{ idValue: "44", texValue: "Camila Urueta" }],
      isLoading: false,
      isFetching: false,
      error: null,
    });

    render(<RadicacionForm />);

    const input = screen.getByLabelText("Destinatario");
    fireEvent.change(input, { target: { value: "cam" } });

    expect(mockedUseAutocompleteCamposPlantilla).toHaveBeenLastCalledWith(
      {
        TextoBuscado: "cam",
        defaultDbAlias: "",
        tbl_control: "terceros",
        name_campo: "Destinatario_Cor",
        idScript: 654,
        CDeRelacionEstadoRetriccionDto: {
          IdRestriTipoDestInterno: 0,
          IdTipoRestriccion: 0,
          DescripcionTipo: "",
          MoluloRadicacion: 0,
          ModuloRadicacionSimple: 0,
          ModuloRadicacionInterna: 0,
        },
      },
      true,
    );
  });

  it("[SPEC:DSR-004] limpia items cuando la busqueda de Destinatario_Cor queda vacia", () => {
    mockedUseCamposPlantilla.mockReturnValue({
      data: [
        {
          name_campo: "DESTINATARIO_COR",
          aleas_campo: "Destinatario",
          campo_tip: 1,
          ComportamientoCampo: "AUTOCOMPLETE",
          tbl_control: "terceros",
          obligatorio_campo: 1,
          disable_campo: 0,
          TomPParameterTomSelelect: {
            id_escript: 654,
          },
        } as unknown as CampoPlantillaDTO,
      ],
      isLoading: false,
      error: null,
      refetch: vi.fn(),
    });

    mockedUseAutocompleteCamposPlantilla.mockReturnValue({
      data: [],
      isLoading: false,
      isFetching: false,
      error: null,
    });

    render(<RadicacionForm />);
    const input = screen.getByLabelText("Destinatario");

    fireEvent.change(input, { target: { value: "cam" } });
    fireEvent.change(input, { target: { value: "" } });

    expect(mockedUseAutocompleteCamposPlantilla).toHaveBeenLastCalledWith(
      null,
      false,
    );
  });

  it("[SPEC:DSR-005] muestra error controlado cuando falla API de Destinatario_Cor", () => {
    mockedUseCamposPlantilla.mockReturnValue({
      data: [
        {
          name_campo: "DESTINATARIO_COR",
          aleas_campo: "Destinatario",
          campo_tip: 1,
          ComportamientoCampo: "AUTOCOMPLETE",
          tbl_control: "terceros",
          obligatorio_campo: 1,
          disable_campo: 0,
        } as unknown as CampoPlantillaDTO,
      ],
      isLoading: false,
      error: null,
      refetch: vi.fn(),
    });

    mockedUseAutocompleteCamposPlantilla.mockReturnValue({
      data: [],
      isLoading: false,
      isFetching: false,
      error: new Error("boom"),
    });

    render(<RadicacionForm />);

    expect(
      screen.getAllByText(
        "No fue posible cargar las opciones. Intenta nuevamente.",
      ).length,
    ).toBeGreaterThan(0);
  });

  it("[SPEC:RBK-005] no falla al re-renderizar secciones de remitente y destinatario al limpiar", () => {
    render(<RadicacionForm />);

    const limpiarButton = screen.getByRole("button", { name: /limpiar/i });
    expect(() => {
      fireEvent.click(limpiarButton);
      fireEvent.click(limpiarButton);
    }).not.toThrow();
  });

  it("[SPEC:RMT-003] usa metadata de REMITENTE_COR y consulta autocompletado de tercero", async () => {
    vi.useFakeTimers();

    mockedUseCamposPlantilla.mockReturnValue({
      data: [
        {
          name_campo: " remitente_cor ",
          aleas_campo: "Remitente",
          campo_tip: 1,
          ComportamientoCampo: "AUTOCOMPLETE",
          tbl_control: "terceros",
          obligatorio_campo: 1,
          disable_campo: 0,
          title_control: "Título Remitente",
          tooltipAyuda: "Ayuda Remitente",
          TomPParameterTomSelelect: {
            id_escript: 987,
          },
        } as unknown as CampoPlantillaDTO,
      ],
      isLoading: false,
      error: null,
      refetch: vi.fn(),
    });

    mockedUseAutocompleteCamposPlantilla.mockReturnValue({
      data: [{ idValue: null, texValue: "Juan Perez" }],
      isLoading: false,
      isFetching: false,
      error: null,
    });

    const { container } = render(<RadicacionForm />);
    expect(
      container.querySelector('[data-ident="pl-radicacion-spe-REMITENTE_COR"]'),
    ).toBeTruthy();

    const input = screen.getByLabelText("Remitente");
    fireEvent.change(input, { target: { value: "juan" } });

    await act(async () => {
      vi.advanceTimersByTime(300);
    });

    expect(mockedUseAutocompleteCamposPlantilla).toHaveBeenLastCalledWith(
      {
        TextoBuscado: "juan",
        defaultDbAlias: "",
        tbl_control: "terceros",
        name_campo: "REMITENTE_COR",
        idScript: 987,
      },
      true,
    );
  });

  it("[SPEC:RMT-004] aplica required, disabled, title y tooltip en REMITENTE_COR", () => {
    mockedUseCamposPlantilla.mockReturnValue({
      data: [
        {
          name_campo: "REMITENTE_COR",
          aleas_campo: "Remitente Dinamico",
          campo_tip: 1,
          ComportamientoCampo: "AUTOCOMPLETE",
          tbl_control: "terceros",
          obligatorio_campo: 1,
          disable_campo: 1,
          title_control: "Título Remitente Dinamico",
          tooltipAyuda: "Ayuda Remitente Dinamico",
        } as unknown as CampoPlantillaDTO,
      ],
      isLoading: false,
      error: null,
      refetch: vi.fn(),
    });

    mockedUseAutocompleteCamposPlantilla.mockReturnValue({
      data: [],
      isLoading: false,
      isFetching: false,
      error: null,
    });

    const { container } = render(<RadicacionForm />);

    const remitenteLabel = screen.getByText("Remitente Dinamico").closest("label");
    expect(remitenteLabel).toBeTruthy();
    expect(remitenteLabel?.className).toContain("ant-form-item-required");
    expect(screen.getByText("Remitente Dinamico")).toHaveAttribute(
      "title",
      "Título Remitente Dinamico",
    );
    expect(
      screen.getByLabelText("Mostrar ayuda para Remitente Dinamico"),
    ).toBeInTheDocument();

    const remitenteSelect = container.querySelector(
      '.ant-select[data-ident="pl-radicacion-spe-REMITENTE_COR"]',
    );
    expect(remitenteSelect?.className).toContain("ant-select-disabled");
  });

  it("[SPEC:FLJ-001] consulta flujos al seleccionar tramite por idValue", () => {
    mockedUseCamposPlantilla.mockReturnValue({
      data: [
        {
          name_campo: "Descripcion_Documento",
          aleas_campo: "Trámite",
          ilist_row_drowlist: [{ idValue: 23, Value: "CITACION" }],
        } as unknown as CampoPlantillaDTO,
        {
          name_campo: "RE_flujo_trabajo",
          aleas_campo: "Flujo Trámite",
        } as unknown as CampoPlantillaDTO,
      ],
      isLoading: false,
      error: null,
      refetch: vi.fn(),
    });

    mockedUseFlujosRelacionadosTramite.mockImplementation((idTipo) => ({
      data:
        String(idTipo ?? "") === "23"
          ? [{ value: "11", label: "FLUJO RADICADO" }]
          : [],
      isLoading: false,
      isFetching: false,
      error: null,
      shouldFetch: String(idTipo ?? "") === "23",
    }));

    const { container } = render(<RadicacionForm />);
    const tramiteSelect = screen.getByTestId("ra_tipo_tramite_select");
    fireEvent.mouseDown(tramiteSelect);
    fireEvent.click(screen.getByText("CITACION"));

    expect(mockedUseFlujosRelacionadosTramite).toHaveBeenLastCalledWith("23", true);
    expect(mockedUseEstructuraRelacionTipoRestriccion).toHaveBeenLastCalledWith(
      "23",
      true,
    );

    const flujoSelect = container.querySelector(
      '[data-ident="pl-radicacion-spe-RE_flujo_trabajo"]',
    );
    expect(flujoSelect?.className).not.toContain("ant-select-disabled");
    if (flujoSelect) {
      fireEvent.mouseDown(flujoSelect);
    }
    expect(screen.getByText("FLUJO RADICADO")).toBeInTheDocument();
  });

  it("[SPEC:FLJ-002] mantiene flujo deshabilitado y sin opciones cuando tramite es null", () => {
    mockedUseCamposPlantilla.mockReturnValue({
      data: [
        {
          name_campo: "Descripcion_Documento",
          aleas_campo: "Trámite",
          ilist_row_drowlist: [{ idValue: null, Value: "SIN ID" }],
        } as unknown as CampoPlantillaDTO,
        {
          name_campo: "RE_flujo_trabajo",
          aleas_campo: "Flujo Trámite",
        } as unknown as CampoPlantillaDTO,
      ],
      isLoading: false,
      error: null,
      refetch: vi.fn(),
    });

    const { container } = render(<RadicacionForm />);
    expect(mockedUseFlujosRelacionadosTramite).toHaveBeenLastCalledWith(null, true);
    expect(mockedUseEstructuraRelacionTipoRestriccion).toHaveBeenLastCalledWith(
      null,
      true,
    );

    const flujoSelect = container.querySelector(
      '[data-ident="pl-radicacion-spe-RE_flujo_trabajo"]',
    );
    expect(flujoSelect?.className).toContain("ant-select-disabled");
  });

  it("[SPEC:RDS-004] aplica restriccion de destinatario desde CDeRelacionEstadoRetriccionDto", () => {
    mockedUseEstructuraRelacionTipoRestriccion.mockReturnValue({
      data: {
        IdRestriTipoDestInterno: 1,
        IdTipoRestriccion: 10,
        DescripcionTipo: "RESTRINGIDO",
        MoluloRadicacion: 0,
        ModuloRadicacionSimple: 0,
        ModuloRadicacionInterna: 0,
      },
      isLoading: false,
      isFetching: false,
      error: null,
      shouldFetch: false,
    });

    const { container } = render(<RadicacionForm />);
    const destinatarioSelect = container.querySelector(
      '.ant-select[data-ident="pl-radicacion-spe-Destinatario_Cor"]',
    );
    expect(destinatarioSelect?.className).toContain("ant-select-disabled");
  });
});
