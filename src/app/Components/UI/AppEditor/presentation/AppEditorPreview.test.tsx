import { render, waitFor } from "@testing-library/react";
import { afterAll, beforeAll, describe, expect, it, vi } from "vitest";
import { AppEditorPreview } from "./AppEditorPreview";

const PAGE_MARGINS = {
  top: 10,
  right: 10,
  bottom: 10,
  left: 10,
};

let originalOffsetHeight: PropertyDescriptor | undefined;

beforeAll(() => {
  originalOffsetHeight = Object.getOwnPropertyDescriptor(HTMLElement.prototype, "offsetHeight");

  Object.defineProperty(HTMLElement.prototype, "offsetHeight", {
    configurable: true,
    get() {
      const text = this.textContent ?? "";

      if (text.includes("Tall image")) {
        return 140;
      }

      if (text.includes("Tall table")) {
        return 140;
      }

      if (text.includes("h40")) {
        return 40;
      }

      return 30;
    },
  });
});

afterAll(() => {
  if (originalOffsetHeight) {
    Object.defineProperty(HTMLElement.prototype, "offsetHeight", originalOffsetHeight);
  }
});

describe("AppEditorPreview", () => {
  it("pagina bloques simples usando medicion DOM", async () => {
    const handlePageCountChange = vi.fn();

    render(
      <AppEditorPreview
        html="<h1>Titulo h40</h1><p>Parrafo h40</p><p>Otro parrafo h40</p>"
        pageWidth={200}
        pageHeight={120}
        pageGap={10}
        pageMargins={PAGE_MARGINS}
        zoomLevel={1}
        onPageCountChange={handlePageCountChange}
      />,
    );

    await waitFor(() => {
      expect(handlePageCountChange).toHaveBeenLastCalledWith(2);
    });

    expect(document.querySelectorAll("article")).toHaveLength(2);
  });

  it("fragmenta listas por li y conserva la numeracion de ol continuadas", async () => {
    render(
      <AppEditorPreview
        html={`<ol start="3"><li>Item h40 uno</li><li>Item h40 dos</li><li>Item h40 tres</li><li>Item h40 cuatro</li></ol>`}
        pageWidth={200}
        pageHeight={100}
        pageGap={10}
        pageMargins={PAGE_MARGINS}
        zoomLevel={1}
      />,
    );

    await waitFor(() => {
      expect(document.querySelectorAll("article")).toHaveLength(2);
    });

    const orderedLists = Array.from(document.querySelectorAll("article ol"));

    expect(orderedLists).toHaveLength(2);
    expect(orderedLists[0]).toHaveAttribute("start", "3");
    expect(orderedLists[1]).toHaveAttribute("start", "5");
  });

  it("marca imagenes y tablas oversized sin fragmentarlas", async () => {
    render(
      <AppEditorPreview
        html={`<figure><img alt="Tall image" src="x.png" /><figcaption>Tall image</figcaption></figure><table><tbody><tr><td>Tall table</td></tr></tbody></table>`}
        pageWidth={200}
        pageHeight={100}
        pageGap={10}
        pageMargins={PAGE_MARGINS}
        zoomLevel={1}
      />,
    );

    await waitFor(() => {
      expect(document.querySelectorAll("article")).toHaveLength(2);
    });

    expect(document.querySelector(".app-editor-preview-oversized-image")).toBeInTheDocument();
    expect(document.querySelector(".app-editor-preview-oversized-table")).toBeInTheDocument();
  });
});
