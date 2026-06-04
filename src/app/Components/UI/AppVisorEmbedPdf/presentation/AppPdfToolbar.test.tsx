import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, expect, it, vi } from "vitest";

import { AppPdfToolbar, type AppPdfToolbarProps } from "./AppPdfToolbar";

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
