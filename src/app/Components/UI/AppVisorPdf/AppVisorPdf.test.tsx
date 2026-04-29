import { act, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { readdirSync, readFileSync, statSync } from "node:fs";
import path from "node:path";

const appButtonMock = vi.fn((props: unknown) => {
  const typed = props as {
    children?: unknown;
    id?: string;
    ["aria-label"]?: string;
    ["aria-controls"]?: string;
    ["aria-expanded"]?: boolean;
    onClick?: () => void;
    onKeyDown?: (event: { key: string; preventDefault: () => void }) => void;
  };
  return (
    <button
      type="button"
      id={typed.id}
      aria-label={typed["aria-label"]}
      aria-controls={typed["aria-controls"]}
      aria-expanded={typed["aria-expanded"]}
      onClick={typed.onClick}
      // Minimal keyboard support for toolbar tests.
      onKeyDown={(event) =>
        typed.onKeyDown?.({
          key: (event as unknown as KeyboardEvent).key,
          preventDefault: () => (event as unknown as KeyboardEvent).preventDefault(),
        })
      }
    >
      {typed.children}
    </button>
  );
});

vi.mock("../AppButton", () => ({
  AppButton: (props: unknown) => appButtonMock(props),
}));

vi.mock("./engine/pdfjsEngine", () => ({
  createPdfjsEngine: () => ({
    load: vi.fn(),
    renderPage: vi.fn(),
    destroy: vi.fn(),
  }),
}));

vi.mock("./engine/fabricEngine", () => ({
  createFabricEngine: () => ({
    attach: vi.fn(),
    detach: vi.fn(),
    setTool: vi.fn(),
    undo: vi.fn(),
    redo: vi.fn(),
    serialize: vi.fn(),
    restore: vi.fn(),
    destroy: vi.fn(),
  }),
}));

describe("AppVisorPdf [SPEC:SCRUMCORE-190]", () => {
  it("renderiza empty state cuando no hay input", async () => {
    const { AppVisorPdf } = await import("./AppVisorPdf");
    render(<AppVisorPdf input={null} aria-label="Visor" />);
    expect(screen.getByRole("status")).toHaveTextContent("No hay PDF seleccionado");
  });

  it("permanece desacoplado de src/modules (no imports directos en el componente shared)", async () => {
    const { AppVisorPdf: _ } = await import("./AppVisorPdf");
    const baseDir = path.resolve(import.meta.dirname);
    const visit = (dir: string, files: string[] = []) => {
      for (const entry of readdirSync(dir)) {
        const fullPath = path.join(dir, entry);
        const stats = statSync(fullPath);
        if (stats.isDirectory()) {
          visit(fullPath, files);
        } else if (stats.isFile()) {
          files.push(fullPath);
        }
      }
      return files;
    };

    const files = visit(baseDir).filter((filePath) => {
      if (filePath.endsWith(".test.ts") || filePath.endsWith(".test.tsx")) {
        return false;
      }
      return (
        filePath.endsWith(".ts") ||
        filePath.endsWith(".tsx") ||
        filePath.endsWith(".css") ||
        filePath.endsWith(".md")
      );
    });

    for (const filePath of files) {
      const content = readFileSync(filePath, "utf8");
      expect(content).not.toContain("src/modules/");
      expect(content).not.toContain("src\\modules\\");
    }
  });

  it("dispara callbacks de page/zoom al interactuar con la toolbar", async () => {
    const { AppVisorPdf } = await import("./AppVisorPdf");
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

  it("usa AppButton para acciones de la toolbar", async () => {
    const { AppVisorPdf } = await import("./AppVisorPdf");
    render(<AppVisorPdf input={null} />);
    expect(appButtonMock).toHaveBeenCalled();
  });

  it("en mobile activa toolbar compacta", async () => {
    Object.defineProperty(window, "innerWidth", { value: 500, configurable: true });
    window.dispatchEvent(new Event("resize"));

    const { AppVisorPdf } = await import("./AppVisorPdf");
    const { container } = render(<AppVisorPdf input={null} />);
    expect(container.querySelector('[data-compact="true"]')).toBeTruthy();
  });

  it("toggle thumbnails expone aria-expanded y responde a teclado", async () => {
    Object.defineProperty(window, "innerWidth", { value: 500, configurable: true });
    window.dispatchEvent(new Event("resize"));

    const { AppVisorPdf } = await import("./AppVisorPdf");
    render(<AppVisorPdf input={null} />);

    const toggle = screen.getByRole("button", { name: "Thumbnails" });
    expect(toggle).toHaveAttribute("aria-expanded", "false");

    act(() => {
      toggle.dispatchEvent(new KeyboardEvent("keydown", { key: "Enter", bubbles: true }));
    });

    expect(toggle).toHaveAttribute("aria-expanded", "true");
  });
});
