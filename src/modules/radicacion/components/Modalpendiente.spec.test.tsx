import { fireEvent, render, screen, within } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import ModalPendiente from "./Modalpendiente";

describe("ModalPendiente", () => {
  it("[SPEC:NAV-006] no presenta pendientes mock en runtime", () => {
    render(<ModalPendiente />);

    fireEvent.click(screen.getByRole("button", { name: /Pendientes/i }));

    const dialog = screen.getByRole("dialog");
    expect(
      within(dialog).getByText(/Funcionalidad pendiente de integración/i),
    ).toBeInTheDocument();
    expect(within(dialog).queryByText("25000270980")).not.toBeInTheDocument();
    expect(within(dialog).queryByText(/Juan/i)).not.toBeInTheDocument();
    expect(within(dialog).queryByRole("table")).not.toBeInTheDocument();
  });
});
