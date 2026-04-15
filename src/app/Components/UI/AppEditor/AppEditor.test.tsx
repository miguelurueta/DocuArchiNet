import { useState } from "react";
import type { FormEvent } from "react";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { AppEditor } from "./presentation/AppEditor";
import styles from "./AppEditor.module.css";

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

  it("permite alternar tema entre claro y oscuro desde el boton visible", () => {
    const { container } = render(<AppEditor label="Contenido tematico" defaultValue="<p>Texto</p>" />);

    const shell = container.querySelector(`.${styles.editor}`);
    const themeButton = screen.getByRole("button", {
      name: "Tema claro activo. Cambiar a oscuro",
    });

    expect(shell).toHaveAttribute("data-theme", "light");

    fireEvent.click(themeButton);
    expect(shell).toHaveAttribute("data-theme", "dark");
    expect(
      screen.getByRole("button", { name: "Tema oscuro activo. Cambiar a claro" }),
    ).toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: "Tema oscuro activo. Cambiar a claro" }));
    expect(shell).toHaveAttribute("data-theme", "light");
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
    const surface = container.querySelector(`.${styles.surfacePaged}`);
    const editor = screen.getByLabelText("Contenido paginado");

    expect(shell).toHaveAttribute("data-pagination-mode", "visual");
    expect(wrapper).toBeInTheDocument();
    expect(canvas).toBeInTheDocument();
    expect(sheet).toBeInTheDocument();
    expect(surface).toBeInTheDocument();
    expect(editor).toBeInTheDocument();
  });

  it("dibuja guias visuales cuando el contenido medido supera varias paginas", async () => {
    const { container } = render(
      <AppEditor
        label="Contenido con guias"
        paginationMode="visual"
        pageFormat="A4"
        pageOrientation="portrait"
        pageMargins={{ top: 96, right: 72, bottom: 96, left: 72 }}
        defaultValue="<p>Documento largo</p>"
      />,
    );

    await waitFor(() => {
      expect(screen.getByText("Documento largo")).toBeInTheDocument();
    });

    const proseMirror = container.querySelector(".ProseMirror");
    expect(proseMirror).toBeInstanceOf(HTMLElement);

    Object.defineProperty(proseMirror as HTMLElement, "scrollHeight", {
      configurable: true,
      value: 2200,
    });

    fireEvent(window, new Event("resize"));

    await waitFor(() => {
      expect(container.querySelectorAll(`.${styles.pageGuide}`)).toHaveLength(2);
    });
  });

  it("muestra el contador de pagina actual en modo visual", async () => {
    const { container } = render(
      <AppEditor
        label="Contenido con contador"
        paginationMode="visual"
        pageFormat="A4"
        pageOrientation="portrait"
        pageMargins={{ top: 96, right: 72, bottom: 96, left: 72 }}
        defaultValue="<p>Documento con contador</p>"
      />,
    );

    await waitFor(() => {
      expect(screen.getByText("Documento con contador")).toBeInTheDocument();
    });

    const proseMirror = container.querySelector(".ProseMirror");
    const canvas = container.querySelector(`.${styles.canvas}`);
    const sheet = container.querySelector('[data-pagination-sheet="true"]');

    expect(proseMirror).toBeInstanceOf(HTMLElement);
    expect(canvas).toBeInstanceOf(HTMLElement);
    expect(sheet).toBeInstanceOf(HTMLElement);

    Object.defineProperty(proseMirror as HTMLElement, "scrollHeight", {
      configurable: true,
      value: 2200,
    });

    Object.defineProperty(canvas as HTMLElement, "scrollTop", {
      configurable: true,
      writable: true,
      value: 1000,
    });

    Object.defineProperty(sheet as HTMLElement, "offsetTop", {
      configurable: true,
      value: 0,
    });

    fireEvent(window, new Event("resize"));
    fireEvent.scroll(canvas as HTMLElement);

    await waitFor(() => {
      expect(screen.getByText("Pagina 2 de 3")).toBeInTheDocument();
    });
  });

  it("permite insertar un salto de pagina manual desde el toolbar", async () => {
    const { container } = render(
      <AppEditor
        label="Contenido con salto"
        paginationMode="visual"
        pageFormat="A4"
        pageOrientation="portrait"
        pageMargins={{ top: 96, right: 72, bottom: 96, left: 72 }}
        defaultValue="<p>Antes</p><p>Despues</p>"
      />,
    );

    await waitFor(() => {
      expect(screen.getByText("Antes")).toBeInTheDocument();
    });

    fireEvent.click(screen.getByRole("button", { name: "Insertar salto de pagina" }));

    await waitFor(() => {
      expect(container.querySelector('[data-page-break="true"]')).toBeInTheDocument();
    });
  });
});
