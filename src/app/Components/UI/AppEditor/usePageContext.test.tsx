import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { createRef } from "react";
import { describe, expect, it } from "vitest";
import { calculatePageFromOffset, usePageContext } from "./application/usePageContext";

describe("usePageContext [SPEC:IMPLEMENTACION-PAGINACION-APPEDITOR-08-FE]", () => {
  it("calcula la pagina actual a partir del offset y la altura util", () => {
    expect(
      calculatePageFromOffset({
        offset: 0,
        pageContentHeight: 931,
        totalPages: 3,
      }),
    ).toBe(1);

    expect(
      calculatePageFromOffset({
        offset: 1000,
        pageContentHeight: 931,
        totalPages: 3,
      }),
    ).toBe(2);

    expect(
      calculatePageFromOffset({
        offset: 4000,
        pageContentHeight: 931,
        totalPages: 3,
      }),
    ).toBe(3);
  });

  it("usa el fallback por scroll cuando no hay cursor activo", async () => {
    const canvasRef = createRef<HTMLDivElement>();

    function Harness() {
      const { currentPage } = usePageContext({
        editor: null,
        enabled: true,
        totalPages: 3,
        pageContentHeight: 931,
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
});
