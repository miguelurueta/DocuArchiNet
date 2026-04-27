import { useState } from "react";
import type { FormEvent } from "react";
import { act, fireEvent, render, screen, waitFor } from "@testing-library/react";
import { afterAll, afterEach, beforeAll, beforeEach, describe, expect, it, vi } from "vitest";

const { saveImageMock, getImageMock } = vi.hoisted(() => ({
  saveImageMock: vi.fn(),
  getImageMock: vi.fn(),
}));

vi.mock("./infrastructure/indexeddb/appEditorImageStore", () => ({
  appEditorImageStore: {
    init: vi.fn(() => Promise.resolve()),
    saveImage: saveImageMock,
    getImage: getImageMock,
    deleteImage: vi.fn(() => Promise.resolve()),
    clearByScope: vi.fn(() => Promise.resolve()),
  },
}));

import { AppEditor } from "./presentation/AppEditor";
import styles from "./AppEditor.module.css";

const THREE_PAGE_DOCUMENT_HTML =
  "<p>Pagina uno</p><div data-page-break=\"true\"></div><p>Pagina dos</p><div data-page-break=\"true\"></div><p>Pagina tres</p>";

const originalElementGetClientRects = Element.prototype.getClientRects;
const originalElementGetBoundingClientRect = Element.prototype.getBoundingClientRect;
const originalRangeGetClientRects = Range.prototype.getClientRects;
const originalRangeGetBoundingClientRect = Range.prototype.getBoundingClientRect;
const originalCreateObjectURL = URL.createObjectURL;
const originalRevokeObjectURL = URL.revokeObjectURL;

function createRectList(): DOMRectList {
  const rect = {
    top: 0,
    left: 0,
    right: 0,
    bottom: 0,
    width: 0,
    height: 0,
    x: 0,
    y: 0,
    toJSON: () => ({}),
  };

  return {
    0: rect,
    length: 1,
    item: () => rect,
    [Symbol.iterator]: function* iterator() {
      yield rect;
    },
  } as DOMRectList;
}

function createRect(): DOMRect {
  return {
    top: 0,
    left: 0,
    right: 0,
    bottom: 0,
    width: 0,
    height: 0,
    x: 0,
    y: 0,
    toJSON: () => ({}),
  } as DOMRect;
}

beforeAll(() => {
  Element.prototype.getClientRects = function getClientRects() {
    return createRectList();
  };

  Element.prototype.getBoundingClientRect = function getBoundingClientRect() {
    return createRect();
  };

  Range.prototype.getClientRects = function getClientRects() {
    return createRectList();
  };

  Range.prototype.getBoundingClientRect = function getBoundingClientRect() {
    return createRect();
  };

  URL.createObjectURL = vi.fn(() => "blob:mock-local-image");
  URL.revokeObjectURL = vi.fn();
});

afterAll(() => {
  Element.prototype.getClientRects = originalElementGetClientRects;
  Element.prototype.getBoundingClientRect = originalElementGetBoundingClientRect;
  Range.prototype.getClientRects = originalRangeGetClientRects;
  Range.prototype.getBoundingClientRect = originalRangeGetBoundingClientRect;
  URL.createObjectURL = originalCreateObjectURL;
  URL.revokeObjectURL = originalRevokeObjectURL;
});

beforeEach(() => {
  saveImageMock.mockReset();
  getImageMock.mockReset();
  vi.mocked(URL.createObjectURL).mockReturnValue("blob:mock-local-image");
  vi.mocked(URL.revokeObjectURL).mockReset();
});

afterEach(() => {
  vi.clearAllMocks();
});

describe("AppEditor [SPEC:IMPLEMENTACION-COMPONENTE-APPEDITOR-01-FE]", () => {
  it("renderiza encabezado contextual, label y helperText", async () => {
    render(
      <AppEditor
        title="Editor principal"
        description="Superficie reusable de respuesta"
        label="Contenido"
        helperText="Puedes aplicar formato enriquecido"
        defaultValue="<p>Texto inicial</p>"
      />,
    );

    expect(screen.getByText("Editor principal")).toBeInTheDocument();
    expect(screen.getByText("Superficie reusable de respuesta")).toBeInTheDocument();
    expect(screen.getByText("Contenido")).toBeInTheDocument();
    expect(screen.getByText("Puedes aplicar formato enriquecido")).toBeInTheDocument();
    expect(screen.getByLabelText("Contenido")).toBeInTheDocument();

    await waitFor(() => {
      expect(screen.getByText("Texto inicial")).toBeInTheDocument();
    });
  });

  it("sincroniza contenido controlado al rerender", async () => {
    const { rerender } = render(
      <AppEditor value="<p>Version uno</p>" label="Contenido controlado" />,
    );

    await waitFor(() => {
      expect(screen.getByText("Version uno")).toBeInTheDocument();
    });

    rerender(<AppEditor value="<p>Version dos</p>" label="Contenido controlado" />);

    await waitFor(() => {
      expect(screen.getByText("Version dos")).toBeInTheDocument();
    });
  });

  it("expone estado readOnly y semantica de error", async () => {
    const { container } = render(
      <AppEditor
        label="Contenido"
        error="El contenido es obligatorio"
        readOnly
        defaultValue="<p>Solo lectura</p>"
      />,
    );

    await waitFor(() => {
      expect(screen.getByText("Solo lectura")).toBeInTheDocument();
    });

    expect(screen.getByText("El contenido es obligatorio")).toBeInTheDocument();
    expect(container.querySelector("[contenteditable='false']")).toBeInTheDocument();
    expect(screen.getByLabelText("Contenido")).toHaveAttribute("aria-invalid", "true");
  });

  it("renderiza acciones custom dentro de la toolbar del editor", async () => {
    render(
      <AppEditor
        label="Contenido"
        defaultValue="<p>Texto inicial</p>"
        toolbarActions={<button type="button" aria-label="Guardar en toolbar" />}
      />,
    );

    await waitFor(() => {
      expect(screen.getByText("Texto inicial")).toBeInTheDocument();
    });

    expect(screen.getByRole("button", { name: "Guardar en toolbar" })).toBeInTheDocument();
    expect(screen.getByRole("group", { name: "Acciones del editor" })).toBeInTheDocument();
  });

  it("marca el shell con data-attributes de estados visuales", async () => {
    const { container } = render(
      <AppEditor
        title="Editor visual"
        error="Error visual"
        disabled
        defaultValue="<p>Bloqueado</p>"
      />,
    );

    await waitFor(() => {
      expect(screen.getByText("Bloqueado")).toBeInTheDocument();
    });

    const shell = container.querySelector(`.${styles.editor}`);
    expect(shell).toHaveAttribute("data-disabled", "true");
    expect(shell).toHaveAttribute("data-error", "true");
    expect(shell).toHaveAttribute("data-readonly", "false");
    expect(shell).toHaveAttribute("data-pagination-mode", "none");
  });

  it("asocia helper y error al editor mediante aria-describedby", async () => {
    render(
      <AppEditor
        label="Contenido accesible"
        helperText="Ayuda contextual"
        error="Error contextual"
        defaultValue="<p>Texto</p>"
      />,
    );

    const editor = screen.getByLabelText("Contenido accesible");

    await waitFor(() => {
      expect(screen.getByText("Texto")).toBeInTheDocument();
    });

    const describedBy = editor.getAttribute("aria-describedby") ?? "";
    expect(describedBy).not.toHaveLength(0);
    expect(screen.getByText("Ayuda contextual").id).toBeTruthy();
    expect(screen.getByText("Error contextual").id).toBeTruthy();
    expect(describedBy).toContain(screen.getByText("Ayuda contextual").id);
    expect(describedBy).toContain(screen.getByText("Error contextual").id);
  });

  it("permite integracion representativa en un formulario con submit controlado", async () => {
    function FormHarness() {
      const [submitted, setSubmitted] = useState("");

      const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        setSubmitted(
          (event.currentTarget.elements.namedItem("snapshot") as HTMLInputElement).value,
        );
      };

      return (
        <form onSubmit={handleSubmit}>
          <AppEditor
            label="Contenido del formulario"
            value="<p>Payload estable</p>"
            onChange={() => {}}
          />
          <input name="snapshot" defaultValue="<p>Payload estable</p>" readOnly />
          <button type="submit">Guardar</button>
          <output data-testid="submit-output">{submitted}</output>
        </form>
      );
    }

    render(<FormHarness />);
    fireEvent.click(screen.getByText("Guardar"));

    expect(screen.getByLabelText("Contenido del formulario")).toBeInTheDocument();
    expect(screen.getByDisplayValue("<p>Payload estable</p>")).toBeInTheDocument();
    expect(screen.getByTestId("submit-output")).toHaveTextContent("<p>Payload estable</p>");
  });

  it("mantiene compatibilidad con themeMode externo sin toggle visible", () => {
    const { container } = render(
      <AppEditor themeMode="dark" label="Contenido tematico" defaultValue="<p>Texto</p>" />,
    );

    const shell = container.querySelector(`.${styles.editor}`);

    expect(shell).toHaveAttribute("data-theme", "dark");
    expect(screen.queryByRole("button", { name: /Tema .* activo/i })).not.toBeInTheDocument();
  });

  it("renderiza imagenes con ancho persistido desde la extension shared", async () => {
    const { container } = render(
      <AppEditor
        label="Contenido con imagen"
        defaultValue={'<p>Intro</p><img src="https://cdn.example.com/image.png" data-width="75%" />'}
      />,
    );

    await waitFor(() => {
      expect(screen.getByText("Intro")).toBeInTheDocument();
    });

    const image = container.querySelector("img");
    expect(image).toHaveAttribute("data-width", "75%");
  });

  it("permite insertar una imagen nueva desde la UI del editor", async () => {
    const { container } = render(
      <AppEditor label="Contenido con insercion de imagen" defaultValue="<p>Inicio</p>" />,
    );

    await waitFor(() => {
      expect(screen.getByText("Inicio")).toBeInTheDocument();
    });

    fireEvent.click(screen.getByLabelText("Insertar imagen"));
    fireEvent.change(await screen.findByLabelText("URL de la imagen"), {
      target: { value: "cdn.example.com/nueva.png" },
    });
    fireEvent.click(screen.getByRole("button", { name: /^Insertar$/ }));

    await waitFor(() => {
      const image = container.querySelector('img[src="https://cdn.example.com/nueva.png"]');
      expect(image).toBeInTheDocument();
    });
  });

  it("inserta una imagen local usando IndexedDB y serializa data-local-image-id", async () => {
    saveImageMock.mockImplementation(async (image) => image);
    vi.spyOn(crypto, "randomUUID").mockReturnValue(
      "local-1-uuid-2-uuid" as `${string}-${string}-${string}-${string}-${string}`,
    );

    const { container } = render(
      <AppEditor label="Contenido con imagen local" defaultValue="<p>Inicio</p>" />,
    );

    await waitFor(() => {
      expect(screen.getByText("Inicio")).toBeInTheDocument();
    });

    fireEvent.click(screen.getByLabelText("Insertar imagen"));
    const fileInput = document.querySelector('input[type="file"]') as HTMLInputElement;
    const file = new File(["local-image"], "logo.png", { type: "image/png" });
    fireEvent.change(fileInput, {
      target: { files: [file] },
    });

    await waitFor(() => {
      expect(saveImageMock).toHaveBeenCalledWith(
        expect.objectContaining({
          id: "img_local_local-1-uuid-2-uuid",
          fileName: "logo.png",
          contentType: "image/png",
        }),
      );
    });

    await waitFor(() => {
      const image = container.querySelector(
        'img[data-local-image-id="img_local_local-1-uuid-2-uuid"]',
      );
      expect(image).toHaveAttribute("src", "blob:mock-local-image");
      expect(image).toHaveAttribute("data-source", "local");
    });
  });

  it("rehidrata alineacion horizontal persistida de imagen", async () => {
    const { container } = render(
      <AppEditor
        label="Contenido con imagen alineada"
        defaultValue={
          '<p>Intro</p><img src="https://cdn.example.com/image.png" data-width="75%" data-align="center" />'
        }
      />,
    );

    await waitFor(() => {
      expect(screen.getByText("Intro")).toBeInTheDocument();
    });

    const image = container.querySelector("img");
    expect(image).toHaveAttribute("data-width", "75%");
    expect(image).toHaveAttribute("data-align", "center");
  });

  it("rehidrata una imagen local desde IndexedDB dentro de la sesion", async () => {
    getImageMock.mockResolvedValue({
      id: "img_local_rehydrated",
      fileName: "logo.png",
      contentType: "image/png",
      size: 12,
      blob: new Blob(["rehydrated"], { type: "image/png" }),
      createdAt: Date.now(),
    });
    vi.mocked(URL.createObjectURL).mockReturnValue("blob:rehydrated-image");

    const { container } = render(
      <AppEditor
        label="Contenido con imagen local rehidratada"
        defaultValue={
          '<p>Intro</p><img src="blob:stale-image" data-local-image-id="img_local_rehydrated" data-source="local" data-width="75%" data-align="center" />'
        }
      />,
    );

    await waitFor(() => {
      const image = container.querySelector('img[data-local-image-id="img_local_rehydrated"]');
      expect(image).toHaveAttribute("src", "blob:rehydrated-image");
      expect(image).toHaveAttribute("data-align", "center");
      expect(image).toHaveAttribute("data-width", "75%");
    });
  });

  it("libera object urls al desmontar el editor", async () => {
    getImageMock.mockResolvedValue({
      id: "img_local_rehydrated",
      fileName: "logo.png",
      contentType: "image/png",
      size: 12,
      blob: new Blob(["rehydrated"], { type: "image/png" }),
      createdAt: Date.now(),
    });
    vi.mocked(URL.createObjectURL).mockReturnValue("blob:rehydrated-image");

    const { unmount } = render(
      <AppEditor
        label="Contenido con limpieza local"
        defaultValue={
          '<p>Intro</p><img src="blob:stale-image" data-local-image-id="img_local_rehydrated" data-source="local" />'
        }
      />,
    );

    await waitFor(() => {
      expect(getImageMock).toHaveBeenCalledWith("img_local_rehydrated");
    });

    unmount();

    expect(URL.revokeObjectURL).toHaveBeenCalledWith("blob:rehydrated-image");
  });

  it("reutiliza la misma object url para una imagen local ya rehidratada", async () => {
    getImageMock.mockResolvedValue({
      id: "img_local_rehydrated",
      fileName: "logo.png",
      contentType: "image/png",
      size: 12,
      blob: new Blob(["rehydrated"], { type: "image/png" }),
      createdAt: Date.now(),
    });
    vi.mocked(URL.createObjectURL).mockReturnValue("blob:rehydrated-image");

    const initialValue =
      '<p>Intro</p><img src="blob:stale-image" data-local-image-id="img_local_rehydrated" data-source="local" />';

    const { rerender } = render(
      <AppEditor
        label="Contenido con imagen local estable"
        value={initialValue}
      />,
    );

    await waitFor(() => {
      expect(getImageMock).toHaveBeenCalledTimes(1);
    });

    rerender(
      <AppEditor
        label="Contenido con imagen local estable"
        value={initialValue}
      />,
    );

    await waitFor(() => {
      expect(URL.createObjectURL).toHaveBeenCalledTimes(1);
      expect(getImageMock).toHaveBeenCalledTimes(1);
    });
  });

  it("renderiza el modo visual con canvas y hoja centrada sin cambiar la semantica del editor", async () => {
    const { container } = render(
      <AppEditor
        label="Contenido paginado"
        paginationMode="visual"
        pageFormat="A4"
        pageOrientation="portrait"
        pageMargins={{ top: 96, right: 72, bottom: 96, left: 72 }}
        defaultValue="<p>Documento paginado</p>"
      />,
    );

    await waitFor(() => {
      expect(screen.getByText("Documento paginado")).toBeInTheDocument();
    });

    const shell = container.querySelector(`.${styles.editor}`);
    const wrapper = container.querySelector(`.${styles.editorWrapper}`);
    const canvas = container.querySelector(`.${styles.canvas}`);
    const sheet = container.querySelector(`.${styles.sheet}`);
    const editorContent = container.querySelector(`.${styles.editorContentPaged}`);
    const proseMirror = container.querySelector(".ProseMirror");
    const pageWrappers = proseMirror?.querySelectorAll('[data-app-editor-page="true"]');
    const editor = screen.getByLabelText("Contenido paginado");

    expect(shell).toHaveAttribute("data-pagination-mode", "visual");
    expect(wrapper).toBeInTheDocument();
    expect(canvas).toBeInTheDocument();
    expect(sheet).toBeInTheDocument();
    expect(editorContent).toBeInTheDocument();
    expect(editorContent?.parentElement).toBe(sheet);
    expect(proseMirror).toBeInTheDocument();
    expect(pageWrappers).toHaveLength(1);
    expect(editor).toBeInTheDocument();
    expect(screen.getByText("Pagina 1 de 1")).toBeInTheDocument();
  });

  it("renderiza el modo continuo sin wrapper intermedio redundante", async () => {
    const { container } = render(
      <AppEditor
        label="Contenido continuo estructural"
        defaultValue="<p>Documento continuo</p>"
      />,
    );

    await waitFor(() => {
      expect(screen.getByText("Documento continuo")).toBeInTheDocument();
    });

    const frame = container.querySelector(`.${styles.frame}`);
    const editorContent = container.querySelector(`.${styles.editorContent}`);

    expect(frame).toBeInTheDocument();
    expect(editorContent).toBeInTheDocument();
    expect(editorContent?.parentElement).toBe(frame);
  });

  it("mantiene el calculo interno de paginas sin dibujar lineas guia visibles", async () => {
    const { container } = render(
      <AppEditor
        label="Contenido con guias"
        paginationMode="visual"
        pageFormat="A4"
        pageOrientation="portrait"
        pageMargins={{ top: 96, right: 72, bottom: 96, left: 72 }}
        defaultValue={THREE_PAGE_DOCUMENT_HTML}
      />,
    );

    await waitFor(() => {
      expect(screen.getByText("Pagina uno")).toBeInTheDocument();
    });

    const proseMirror = container.querySelector(".ProseMirror");
    expect(proseMirror).toBeInstanceOf(HTMLElement);

    await waitFor(() => {
      expect(screen.getByText("Pagina 1 de 3")).toBeInTheDocument();
    });

    expect(
      proseMirror?.querySelectorAll('[data-app-editor-page="true"]'),
    ).toHaveLength(3);
    expect(container.querySelector(`.${styles.pageShell}`)).not.toBeInTheDocument();
  });

  it("mantiene el contador total visible cuando la repaginacion notifica un update visual", async () => {
    const { container } = render(
      <AppEditor
        label="Contenido con repaginacion notificada"
        paginationMode="visual"
        pageFormat="A4"
        pageOrientation="portrait"
        pageMargins={{ top: 96, right: 72, bottom: 96, left: 72 }}
        defaultValue={THREE_PAGE_DOCUMENT_HTML}
      />,
    );

    await waitFor(() => {
      expect(screen.getByText("Pagina uno")).toBeInTheDocument();
    });

    const proseMirror = container.querySelector(".ProseMirror");
    const canvas = container.querySelector(`.${styles.canvas}`);
    const sheet = container.querySelector('[data-pagination-sheet="true"]');
    expect(proseMirror).toBeInstanceOf(HTMLElement);
    expect(canvas).toBeInstanceOf(HTMLElement);
    expect(sheet).toBeInstanceOf(HTMLElement);

    Object.defineProperty(canvas as HTMLElement, "scrollTop", {
      configurable: true,
      writable: true,
      value: 1300,
    });
    Object.defineProperty(canvas as HTMLElement, "clientHeight", {
      configurable: true,
      value: 900,
    });
    Object.defineProperty(sheet as HTMLElement, "offsetTop", {
      configurable: true,
      value: 0,
    });

    act(() => {
      fireEvent.scroll(canvas as HTMLElement);
      (canvas as HTMLElement).dispatchEvent(new CustomEvent("app-editor-pagination-updated"));
    });

    await waitFor(() => {
      expect(screen.getByText("Pagina 1 de 3")).toBeInTheDocument();
    });
  });

  it("muestra el contador de pagina en modo visual fuera del canvas editable", async () => {
    const { container } = render(
      <AppEditor
        label="Contenido con contador"
        paginationMode="visual"
        pageFormat="A4"
        pageOrientation="portrait"
        pageMargins={{ top: 96, right: 72, bottom: 96, left: 72 }}
        defaultValue={THREE_PAGE_DOCUMENT_HTML}
      />,
    );

    await waitFor(() => {
      expect(screen.getByText("Pagina uno")).toBeInTheDocument();
    });

    const canvas = container.querySelector(`.${styles.canvas}`);
    const sheet = container.querySelector('[data-pagination-sheet="true"]');

    expect(canvas).toBeInstanceOf(HTMLElement);
    expect(sheet).toBeInstanceOf(HTMLElement);

    const pageCounterBadge = container.querySelector(`.${styles.pageCounterBadge}`);
    expect(pageCounterBadge).toBeInTheDocument();
    await waitFor(() => {
      expect(pageCounterBadge).toHaveTextContent("Pagina 1 de 3");
    });
    expect(container.querySelector(`.${styles.editorWrapper}`)?.textContent).not.toContain(
      "Pagina 1 de 3",
    );
  });

  it("muestra control de zoom solo en modo visual con valor por defecto de 100 por ciento", async () => {
    const { rerender } = render(
      <AppEditor
        label="Contenido con zoom"
        paginationMode="visual"
        defaultValue="<p>Documento con zoom</p>"
      />,
    );

    await waitFor(() => {
      expect(screen.getByText("Documento con zoom")).toBeInTheDocument();
    });

    expect(screen.getByRole("group", { name: "Control de zoom" })).toBeInTheDocument();
    expect(screen.getByText("100%")).toBeInTheDocument();

    rerender(<AppEditor label="Contenido continuo" defaultValue="<p>Documento con zoom</p>" />);

    expect(screen.queryByRole("group", { name: "Control de zoom" })).not.toBeInTheDocument();
  });

  it("retira contador y estructura paginada al volver de modo visual a continuo", async () => {
    const { container, rerender } = render(
      <AppEditor
        label="Contenido que cambia de modo"
        paginationMode="visual"
        pageFormat="A4"
        pageOrientation="portrait"
        pageMargins={{ top: 96, right: 72, bottom: 96, left: 72 }}
        defaultValue={THREE_PAGE_DOCUMENT_HTML}
      />,
    );

    await waitFor(() => {
      expect(screen.getByText("Pagina 1 de 3")).toBeInTheDocument();
    });

    rerender(
      <AppEditor
        label="Contenido que cambia de modo"
        defaultValue="<p>Documento que cambia de modo</p>"
      />,
    );

    await waitFor(() => {
      expect(screen.queryByText("Pagina 1 de 3")).not.toBeInTheDocument();
    });

    expect(container.querySelector(".ProseMirror")?.querySelector('[data-app-editor-page="true"]')).not.toBeInTheDocument();
    expect(container.querySelector(`.${styles.editorWrapper}`)).not.toBeInTheDocument();
    expect(container.querySelector(`.${styles.editorContent}`)).toBeInTheDocument();
  });

  it("rehidrata links dentro de paginas reales en modo visual", async () => {
    const { container } = render(
      <AppEditor
        label="Contenido con link visual"
        paginationMode="visual"
        defaultValue='<p><a href="https://docs.openai.com">Docs</a></p>'
      />,
    );

    const link = await screen.findByRole("link", { name: "Docs" });
    const page = container.querySelector('[data-app-editor-page="true"]');

    expect(page).toContainElement(link);
    expect(link).toHaveAttribute("href", "https://docs.openai.com");
  });

  it("inserta una imagen local en modo visual sin perder la estructura paginada real", async () => {
    saveImageMock.mockImplementation(async (image) => image);
    vi.spyOn(crypto, "randomUUID").mockReturnValue(
      "visual-local-1-uuid-2-uuid" as `${string}-${string}-${string}-${string}-${string}`,
    );

    const { container } = render(
      <AppEditor
        label="Contenido visual con imagen local"
        paginationMode="visual"
        defaultValue="<p>Inicio visual</p>"
      />,
    );

    await waitFor(() => {
      expect(screen.getByText("Inicio visual")).toBeInTheDocument();
    });

    fireEvent.click(screen.getByLabelText("Insertar imagen"));
    const fileInput = document.querySelector('input[type="file"]') as HTMLInputElement;
    const file = new File(["visual-local-image"], "visual-logo.png", { type: "image/png" });
    fireEvent.change(fileInput, {
      target: { files: [file] },
    });

    await waitFor(() => {
      expect(saveImageMock).toHaveBeenCalledWith(
        expect.objectContaining({
          id: "img_local_visual-local-1-uuid-2-uuid",
          fileName: "visual-logo.png",
          contentType: "image/png",
        }),
      );
    });

    await waitFor(() => {
      const image = container.querySelector(
        'img[data-local-image-id="img_local_visual-local-1-uuid-2-uuid"]',
      );
      expect(image).toBeInTheDocument();
      expect(
        container.querySelector('[data-app-editor-page="true"] img[data-local-image-id="img_local_visual-local-1-uuid-2-uuid"]'),
      ).toBeInTheDocument();
    });
  });

  it("respeta limites minimos y maximos del zoom visual", async () => {
    render(
      <AppEditor
        label="Contenido con limites de zoom"
        paginationMode="visual"
        defaultZoomLevel={1}
        minZoomLevel={0.75}
        maxZoomLevel={1.25}
        defaultValue="<p>Documento acotado</p>"
      />,
    );

    await waitFor(() => {
      expect(screen.getByText("Documento acotado")).toBeInTheDocument();
    });

    const decreaseButton = screen.getByRole("button", { name: "Reducir zoom" });
    const increaseButton = screen.getByRole("button", { name: "Aumentar zoom" });

    fireEvent.click(increaseButton);
    await waitFor(() => {
      expect(screen.getByText("125%")).toBeInTheDocument();
    });
    expect(increaseButton).toBeDisabled();

    fireEvent.click(decreaseButton);
    fireEvent.click(decreaseButton);

    await waitFor(() => {
      expect(screen.getByText("75%")).toBeInTheDocument();
    });
    expect(decreaseButton).toBeDisabled();
  });

  it("soporta zoom controlado sin mutar contenido ni disparar onChange del editor", async () => {
    function ZoomHarness() {
      const [currentZoom, setCurrentZoom] = useState(1);
      const handleChange = vi.fn();

      return (
        <div>
          <AppEditor
            label="Contenido con zoom controlado"
            paginationMode="visual"
            zoomLevel={currentZoom}
            onZoomChange={setCurrentZoom}
            onChange={handleChange}
            defaultValue='<p>Intro</p><img src="https://cdn.example.com/image.png" data-width="75%" data-align="center" />'
          />
          <output data-testid="zoom-output">{currentZoom}</output>
          <output data-testid="change-output">{handleChange.mock.calls.length}</output>
        </div>
      );
    }

    const { container } = render(<ZoomHarness />);

    await waitFor(() => {
      expect(screen.getByText("Intro")).toBeInTheDocument();
    });

    fireEvent.click(screen.getByRole("button", { name: "Aumentar zoom" }));

    await waitFor(() => {
      expect(screen.getByText("125%")).toBeInTheDocument();
      expect(screen.getByTestId("zoom-output")).toHaveTextContent("1.25");
    });

    const image = container.querySelector("img");
    expect(image).toHaveAttribute("data-width", "75%");
    expect(image).toHaveAttribute("data-align", "center");
    expect(screen.getByTestId("change-output")).toHaveTextContent("0");
  });

});
