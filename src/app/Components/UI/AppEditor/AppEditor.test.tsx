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
});
