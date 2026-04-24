import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { AppEditorPdf } from "./AppEditorPdf";

const appEditorMock = vi.fn(() => <div data-testid="app-editor-mock" />);

vi.mock("../AppEditor", () => ({
  AppEditor: (props: unknown) => appEditorMock(props),
}));

describe("AppEditorPdf [SPEC:APP-APPEDITORPDF-01-FE]", () => {
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
});

