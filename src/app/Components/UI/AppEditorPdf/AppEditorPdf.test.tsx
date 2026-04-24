import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { AppEditorPdf } from "./AppEditorPdf";
import styles from "./AppEditorPdf.module.css";

const appEditorMock = vi.fn(() => <div data-testid="app-editor-mock" />);

vi.mock("../AppEditor", () => ({
  AppEditor: (props: unknown) => appEditorMock(props),
}));

describe("AppEditorPdf [SPEC:APP-APPEDITORPDF-03-FE]", () => {
  it("renderiza usando AppEditor como engine shared", () => {
    render(<AppEditorPdf label="Editor PDF" defaultValue="<p>Inicial</p>" />);

    expect(screen.getByTestId("app-editor-mock")).toBeInTheDocument();
    expect(appEditorMock).toHaveBeenCalledTimes(1);
  });

  it("pasa contrato controlado al AppEditor subyacente", () => {
    const onChange = vi.fn();

    render(
      <AppEditorPdf
        label="Editor PDF controlado"
        value="<p>Controlado</p>"
        onChange={onChange}
        readOnly
      />,
    );

    expect(appEditorMock).toHaveBeenCalledWith(
      expect.objectContaining({
        label: "Editor PDF controlado",
        value: "<p>Controlado</p>",
        onChange,
        readOnly: true,
      }),
    );
  });

  it("compone className responsive propio con className externo", () => {
    render(<AppEditorPdf label="Editor con clase" className="custom-shell" />);

    expect(appEditorMock).toHaveBeenCalledWith(
      expect.objectContaining({
        className: `${styles.root} custom-shell`,
      }),
    );
  });

  it("prioriza aria-label explicito cuando esta presente", () => {
    render(
      <AppEditorPdf
        label="Label visible"
        aria-label="Editor PDF accesible"
      />,
    );

    expect(appEditorMock).toHaveBeenCalledWith(
      expect.objectContaining({
        label: "Label visible",
        "aria-label": "Editor PDF accesible",
      }),
    );
  });

  it("usa label string como aria-label cuando no se provee uno explicito", () => {
    render(<AppEditorPdf label="Label como aria" />);

    expect(appEditorMock).toHaveBeenCalledWith(
      expect.objectContaining({
        label: "Label como aria",
        "aria-label": "Label como aria",
      }),
    );
  });

  it("aplica fallback accesible cuando no hay label string ni aria-label", () => {
    render(<AppEditorPdf />);

    expect(appEditorMock).toHaveBeenCalledWith(
      expect.objectContaining({
        "aria-label": "Editor PDF",
      }),
    );
  });
});
