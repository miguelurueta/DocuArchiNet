import { act, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { AppVisorPdf } from "./AppVisorPdf";

const appButtonMock = vi.fn((props: unknown) => {
  const typed = props as { children?: unknown; ["aria-label"]?: string; onClick?: () => void };
  return (
    <button type="button" aria-label={typed["aria-label"]} onClick={typed.onClick}>
      {typed.children}
    </button>
  );
});

vi.mock("../AppButton", () => ({
  AppButton: (props: unknown) => appButtonMock(props),
}));

describe("AppVisorPdf [SPEC:SCRUMCORE-190]", () => {
  it("renderiza empty state cuando no hay input", () => {
    render(<AppVisorPdf input={null} aria-label="Visor" />);
    expect(screen.getByRole("status")).toHaveTextContent("No hay PDF seleccionado");
  });

  it("dispara callbacks de page/zoom al interactuar con la toolbar", () => {
    const onPageChange = vi.fn();
    const onZoomChange = vi.fn();

    render(
      <AppVisorPdf
        input={{ kind: "url", url: "https://example.com/doc.pdf" }}
        defaultPage={2}
        onPageChange={onPageChange}
        defaultZoom={1}
        onZoomChange={onZoomChange}
      />,
    );

    act(() => {
      screen.getByRole("button", { name: "Pagina siguiente" }).click();
    });
    expect(onPageChange).toHaveBeenCalledWith(3);

    act(() => {
      screen.getByRole("button", { name: "Zoom in" }).click();
    });
    expect(onZoomChange).toHaveBeenCalled();
  });

  it("usa AppButton para acciones de la toolbar", () => {
    render(<AppVisorPdf input={null} />);
    expect(appButtonMock).toHaveBeenCalled();
  });
});
