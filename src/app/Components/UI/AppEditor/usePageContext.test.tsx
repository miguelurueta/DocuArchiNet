import { useRef } from "react";
import { act, fireEvent, render, screen, waitFor } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { usePageContext } from "./application/usePageContext";

function PageContextHarness({
  totalPages = 3,
  pageBoundaries = [1155, 2310],
  zoomLevel = 1,
}: {
  totalPages?: number;
  pageBoundaries?: number[];
  zoomLevel?: number;
}) {
  const canvasRef = useRef<HTMLDivElement>(null);
  const { currentPage } = usePageContext({
    enabled: true,
    totalPages,
    pageBoundaries,
    canvasRef,
    zoomLevel,
  });

  return (
    <div>
      <div ref={canvasRef} data-testid="canvas">
        <div data-pagination-sheet="true" data-testid="sheet" />
      </div>
      <output data-testid="current-page">{currentPage}</output>
    </div>
  );
}

describe("usePageContext", () => {
  it("actualiza la pagina actual segun el scroll del canvas", async () => {
    render(<PageContextHarness />);

    const canvas = screen.getByTestId("canvas");
    const sheet = screen.getByTestId("sheet");

    Object.defineProperty(canvas, "scrollTop", {
      configurable: true,
      writable: true,
      value: 1300,
    });
    Object.defineProperty(canvas, "clientHeight", {
      configurable: true,
      value: 900,
    });
    Object.defineProperty(sheet, "offsetTop", {
      configurable: true,
      value: 0,
    });

    fireEvent.scroll(canvas);

    await waitFor(() => {
      expect(screen.getByTestId("current-page")).toHaveTextContent("2");
    });
  });

  it("recalcula inmediatamente al recibir app-editor-pagination-updated", async () => {
    render(<PageContextHarness zoomLevel={1.25} />);

    const canvas = screen.getByTestId("canvas");
    const sheet = screen.getByTestId("sheet");

    Object.defineProperty(canvas, "scrollTop", {
      configurable: true,
      writable: true,
      value: 1500,
    });
    Object.defineProperty(canvas, "clientHeight", {
      configurable: true,
      value: 900,
    });
    Object.defineProperty(sheet, "offsetTop", {
      configurable: true,
      value: 0,
    });

    act(() => {
      canvas.dispatchEvent(new CustomEvent("app-editor-pagination-updated"));
    });

    await waitFor(() => {
      expect(screen.getByTestId("current-page")).toHaveTextContent("2");
    });
  });
});
