import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { AppEditor } from "./index";

describe("AppEditor integration [SPEC:IMPLEMENTACION-COMPONENTE-APPEDITOR-03-FE]", () => {
  it("se puede consumir desde el barrel shared UI", () => {
    render(
      <AppEditor
        title="Editor compartido"
        label="Contenido compartido"
        defaultValue="<p>Integracion shared</p>"
      />,
    );

    expect(screen.getByText("Editor compartido")).toBeInTheDocument();
    expect(screen.getByLabelText("Contenido compartido")).toBeInTheDocument();
  });
});
