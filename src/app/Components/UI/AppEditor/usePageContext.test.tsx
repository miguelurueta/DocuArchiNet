import { act, fireEvent, render, screen, waitFor } from "@testing-library/react";
import { createRef } from "react";
import { describe, expect, it } from "vitest";
import { calculatePageFromOffset, usePageContext } from "./application/usePageContext";

describe("usePageContext [SPEC:IMPLEMENTACION-PAGINACION-APPEDITOR-08-FE]", () => {
  it("calcula la pagina actual a partir del offset y la altura util", () => {
    expect(
      calculatePageFromOffset({
        offset: 0,
        pageBoundaries: [931, 1862],
        totalPages: 3,
      }),
    ).toBe(1);

    expect(
      calculatePageFromOffset({
        offset: 1000,
        pageBoundaries: [931, 1862],
        totalPages: 3,
      }),
    ).toBe(2);

    expect(
      calculatePageFromOffset({
        offset: 4000,
        pageBoundaries: [931, 1862],
        totalPages: 3,
      }),
    ).toBe(3);
  });

  it("respeta limites duros creados por PageBreak", () => {
    expect(
      calculatePageFromOffset({
        offset: 520,
        pageBoundaries: [500, 1431],
        totalPages: 3,
      }),
    ).toBe(2);

    expect(
      calculatePageFromOffset({
        offset: 1500,
        pageBoundaries: [500, 1431],
        totalPages: 3,
      }),
    ).toBe(3);
  });

  it("escala los limites visuales cuando el zoom modifica el offset percibido", () => {
    expect(
      calculatePageFromOffset({
        offset: 1600,
        pageBoundaries: [1155, 2310],
        totalPages: 3,
        boundaryScale: 1.25,
      }),
    ).toBe(2);

    expect(
      calculatePageFromOffset({
        offset: 3100,
        pageBoundaries: [1155, 2310],
        totalPages: 3,
        boundaryScale: 1.25,
      }),
    ).toBe(3);
  });

  it("usa el fallback por scroll cuando no hay cursor activo", async () => {
    const canvasRef = createRef<HTMLDivElement>();

    function Harness() {
      const { currentPage } = usePageContext({
        enabled: true,
        totalPages: 3,
        pageBoundaries: [931, 1862],
        canvasRef,
        debounceMs: 0,
      });

      return (
        <div>
          <div ref={canvasRef}>
            <div data-pagination-sheet="true">sheet</div>
          </div>
          <output data-testid="current-page">{currentPage}</output>
        </div>
      );
    }

    render(<Harness />);

    const canvas = canvasRef.current;
    expect(canvas).toBeInstanceOf(HTMLElement);

    Object.defineProperty(canvas as HTMLDivElement, "scrollTop", {
      configurable: true,
      writable: true,
      value: 1000,
    });

    Object.defineProperty(
      (canvas as HTMLDivElement).querySelector('[data-pagination-sheet="true"]') as HTMLDivElement,
      "offsetTop",
      {
        configurable: true,
        value: 0,
      },
    );

    fireEvent.scroll(canvas as HTMLDivElement);

    await waitFor(() => {
      expect(screen.getByTestId("current-page")).toHaveTextContent("2");
    });
  });

  it("usa el scroll del editor aunque exista un cursor activo en otra hoja", async () => {
    const canvasRef = createRef<HTMLDivElement>();

    function Harness() {
      const { currentPage } = usePageContext({
        enabled: true,
        totalPages: 3,
        pageBoundaries: [931, 1862],
        canvasRef,
        debounceMs: 0,
      });

      return (
        <div>
          <div ref={canvasRef}>
            <div data-pagination-sheet="true">
              <div className="ProseMirror">contenido</div>
            </div>
          </div>
          <output data-testid="current-page">{currentPage}</output>
        </div>
      );
    }

    render(<Harness />);

    const canvas = canvasRef.current;
    const proseMirror = canvas?.querySelector(".ProseMirror");
    const sheet = canvas?.querySelector('[data-pagination-sheet="true"]');

    expect(canvas).toBeInstanceOf(HTMLElement);
    expect(proseMirror).toBeInstanceOf(HTMLElement);
    expect(sheet).toBeInstanceOf(HTMLElement);

    Object.defineProperty(canvas as HTMLDivElement, "scrollTop", {
      configurable: true,
      writable: true,
      value: 1000,
    });

    Object.defineProperty(sheet as HTMLDivElement, "offsetTop", {
      configurable: true,
      value: 0,
    });

    Object.defineProperty(proseMirror as HTMLDivElement, "getBoundingClientRect", {
      configurable: true,
      value: () => ({
        top: 0,
        left: 0,
        right: 0,
        bottom: 0,
        width: 0,
        height: 0,
        x: 0,
        y: 0,
        toJSON: () => ({}),
      }),
    });

    fireEvent.scroll(canvas as HTMLDivElement);

    await waitFor(() => {
      expect(screen.getByTestId("current-page")).toHaveTextContent("2");
    });
  });

  it("mantiene la pagina 1 si no existe desplazamiento interno del editor", async () => {
    const canvasRef = createRef<HTMLDivElement>();

    function Harness() {
      const { currentPage } = usePageContext({
        enabled: true,
        totalPages: 4,
        pageBoundaries: [1155, 2310, 3465],
        canvasRef,
        debounceMs: 0,
      });

      return (
        <div>
          <div ref={canvasRef}>
            <div className="ProseMirror">contenido</div>
          </div>
          <output data-testid="current-page">{currentPage}</output>
        </div>
      );
    }

    render(<Harness />);

    await waitFor(() => {
      expect(screen.getByTestId("current-page")).toHaveTextContent("1");
    });
  });

  it("sincroniza la pagina actual inmediatamente cuando la repaginacion notifica update visual", async () => {
    const canvasRef = createRef<HTMLDivElement>();

    function Harness() {
      const { currentPage } = usePageContext({
        enabled: true,
        totalPages: 3,
        pageBoundaries: [931, 1862],
        canvasRef,
        debounceMs: 1000,
      });

      return (
        <div>
          <div ref={canvasRef}>
            <div data-pagination-sheet="true">
              <div className="ProseMirror">contenido</div>
            </div>
          </div>
          <output data-testid="current-page">{currentPage}</output>
        </div>
      );
    }

    render(<Harness />);

    const canvas = canvasRef.current;
    const proseMirror = canvas?.querySelector(".ProseMirror");
    const sheet = canvas?.querySelector('[data-pagination-sheet="true"]');

    expect(canvas).toBeInstanceOf(HTMLElement);
    expect(proseMirror).toBeInstanceOf(HTMLElement);
    expect(sheet).toBeInstanceOf(HTMLElement);

    Object.defineProperty(canvas as HTMLDivElement, "scrollTop", {
      configurable: true,
      writable: true,
      value: 1000,
    });

    Object.defineProperty(sheet as HTMLDivElement, "offsetTop", {
      configurable: true,
      value: 0,
    });

    act(() => {
      (proseMirror as HTMLDivElement).dispatchEvent(
        new CustomEvent("app-editor-pagination-updated", { bubbles: true }),
      );
    });

    await waitFor(() => {
      expect(screen.getByTestId("current-page")).toHaveTextContent("2");
    });
  });

  it("evita cambiar de hoja por microvariaciones alrededor del borde visible", async () => {
    const canvasRef = createRef<HTMLDivElement>();

    function Harness() {
      const { currentPage } = usePageContext({
        enabled: true,
        totalPages: 3,
        pageBoundaries: [931, 1862],
        canvasRef,
        debounceMs: 0,
      });

      return (
        <div>
          <div ref={canvasRef}>
            <div data-pagination-sheet="true">sheet</div>
          </div>
          <output data-testid="current-page">{currentPage}</output>
        </div>
      );
    }

    render(<Harness />);

    const canvas = canvasRef.current;
    const sheet = canvas?.querySelector('[data-pagination-sheet="true"]');

    expect(canvas).toBeInstanceOf(HTMLElement);
    expect(sheet).toBeInstanceOf(HTMLElement);

    Object.defineProperty(canvas as HTMLDivElement, "scrollTop", {
      configurable: true,
      writable: true,
      value: 900,
    });

    Object.defineProperty(canvas as HTMLDivElement, "clientHeight", {
      configurable: true,
      value: 600,
    });

    Object.defineProperty(sheet as HTMLDivElement, "offsetTop", {
      configurable: true,
      value: 0,
    });

    fireEvent.scroll(canvas as HTMLDivElement);

    await waitFor(() => {
      expect(screen.getByTestId("current-page")).toHaveTextContent("1");
    });

    Object.defineProperty(canvas as HTMLDivElement, "scrollTop", {
      configurable: true,
      writable: true,
      value: 940,
    });

    fireEvent.scroll(canvas as HTMLDivElement);

    await waitFor(() => {
      expect(screen.getByTestId("current-page")).toHaveTextContent("1");
    });

    Object.defineProperty(canvas as HTMLDivElement, "scrollTop", {
      configurable: true,
      writable: true,
      value: 980,
    });

    fireEvent.scroll(canvas as HTMLDivElement);

    await waitFor(() => {
      expect(screen.getByTestId("current-page")).toHaveTextContent("2");
    });
  });

  it("mantiene el scroll como fuente prioritaria al navegar hacia la ultima hoja", async () => {
    const canvasRef = createRef<HTMLDivElement>();
    function Harness() {
      const { currentPage } = usePageContext({
        enabled: true,
        totalPages: 4,
        pageBoundaries: [1155, 2310, 3465],
        canvasRef,
        debounceMs: 0,
      });

      return (
        <div>
          <div ref={canvasRef}>
            <div data-pagination-sheet="true">
              <div className="ProseMirror">contenido</div>
            </div>
          </div>
          <output data-testid="current-page">{currentPage}</output>
        </div>
      );
    }

    render(<Harness />);

    const canvas = canvasRef.current;
    const sheet = canvas?.querySelector('[data-pagination-sheet="true"]');
    const proseMirror = canvas?.querySelector(".ProseMirror");

    expect(canvas).toBeInstanceOf(HTMLElement);
    expect(sheet).toBeInstanceOf(HTMLElement);
    expect(proseMirror).toBeInstanceOf(HTMLElement);

    Object.defineProperty(canvas as HTMLDivElement, "scrollTop", {
      configurable: true,
      writable: true,
      value: 3600,
    });

    Object.defineProperty(sheet as HTMLDivElement, "offsetTop", {
      configurable: true,
      value: 0,
    });

    Object.defineProperty(proseMirror as HTMLDivElement, "getBoundingClientRect", {
      configurable: true,
      value: () => ({
        top: 0,
        left: 0,
        right: 0,
        bottom: 0,
        width: 0,
        height: 0,
        x: 0,
        y: 0,
        toJSON: () => ({}),
      }),
    });

    fireEvent.scroll(canvas as HTMLDivElement);

    await waitFor(() => {
      expect(screen.getByTestId("current-page")).toHaveTextContent("4");
    });
  });
});
