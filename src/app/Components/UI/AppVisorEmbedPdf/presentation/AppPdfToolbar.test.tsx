import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import type { ReactNode } from "react";
import { describe, expect, it, vi } from "vitest";

import { AppPdfToolbar, type AppPdfToolbarProps } from "./AppPdfToolbar";

vi.mock("../../AppDropdown", () => ({
  AppDropdown: ({
    trigger,
    items,
  }: {
    trigger: ReactNode;
    items: Array<{ key: string; label?: ReactNode; disabled?: boolean; onSelect?: () => void }>;
  }) => (
    <div>
      {trigger}
      <div role="menu" aria-label="Mas acciones PDF">
        {items.map((item) => (
          <button
            key={item.key}
            type="button"
            role="menuitem"
            disabled={item.disabled}
            onClick={item.onSelect}
          >
            {item.label}
          </button>
        ))}
      </div>
    </div>
  ),
}));

function renderToolbar(overrides: Partial<AppPdfToolbarProps> = {}) {
  const props: AppPdfToolbarProps = {
    zoomLevel: 1,
    onZoomIn: vi.fn(),
    onZoomOut: vi.fn(),
    onResetZoom: vi.fn(),
    onToggleThumbnails: vi.fn(),
    isThumbnailOpen: false,
    onRotateLeft: vi.fn(),
    onRotateRight: vi.fn(),
    onToggleSignatureModal: vi.fn(),
    isSignatureModalOpen: false,
    onDeleteSelectedSignature: vi.fn(),
    canDeleteSelectedSignature: false,
    onSaveSignedPdf: vi.fn(),
    isSignatureLocked: false,
    onPrint: vi.fn(),
    onExport: vi.fn(),
    ...overrides,
  };

  render(<AppPdfToolbar {...props} />);

  return props;
}

describe("AppPdfToolbar guide tour", () => {
  it("no muestra ayuda cuando no recibe props de guia", () => {
    renderToolbar();

    expect(screen.queryByRole("button", { name: /guia interactiva/i })).not.toBeInTheDocument();
  });

  it("muestra boton ayuda y dispara inicio por click y teclado", async () => {
    const onStartGuideTour = vi.fn();
    renderToolbar({ onStartGuideTour, isGuideTourAvailable: true });

    const user = userEvent.setup();
    const helpButton = screen.getByRole("button", { name: /guia interactiva/i });

    expect(helpButton).toHaveAttribute("data-guide-tour-id", "pdf-help");
    expect(helpButton).toHaveAttribute("title", expect.stringMatching(/ayuda/i));

    await user.click(helpButton);
    helpButton.focus();
    await user.keyboard("{Enter}");

    expect(onStartGuideTour).toHaveBeenCalledTimes(2);
  });
});

describe("AppPdfToolbar responsive overflow", () => {
  it("expone acciones secundarias en menu de mas acciones", () => {
    renderToolbar({ onStartGuideTour: vi.fn(), isGuideTourAvailable: true });

    expect(screen.getByRole("button", { name: /mas acciones pdf/i })).toBeInTheDocument();
    expect(screen.getByRole("menuitem", { name: /rotar izquierda/i })).toBeInTheDocument();
    expect(screen.getByRole("menuitem", { name: /rotar derecha/i })).toBeInTheDocument();
    expect(screen.getByRole("menuitem", { name: /imprimir/i })).toBeInTheDocument();
    expect(screen.getByRole("menuitem", { name: /exportar/i })).toBeInTheDocument();
    expect(screen.getByRole("menuitem", { name: /guia interactiva/i })).toBeInTheDocument();
  });
});

describe("AppPdfToolbar guardar paginas anotadas", () => {
  it("solo muestra accion cuando recibe callback y respeta disabled/loading", async () => {
    renderToolbar();

    expect(screen.queryByRole("button", { name: /guardar paginas anotadas/i })).not.toBeInTheDocument();

    const onSaveAnnotatedPages = vi.fn();
    renderToolbar({ onSaveAnnotatedPages });

    const user = userEvent.setup();
    await user.click(screen.getByRole("button", { name: /guardar paginas anotadas/i }));

    expect(onSaveAnnotatedPages).toHaveBeenCalledTimes(1);
  });

  it("deshabilita accion mientras guarda", () => {
    renderToolbar({ onSaveAnnotatedPages: vi.fn(), isSavingAnnotatedPages: true, saveAnnotatedPagesProgress: 0.42 });

    const button = screen.getByRole("button", { name: /guardar paginas anotadas/i });
    expect(button).toBeDisabled();
    expect(button).toHaveAttribute("aria-valuenow", "42");
    expect(button).toHaveAttribute("title", expect.stringContaining("42%"));
  });

});
