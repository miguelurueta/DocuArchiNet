import { act, fireEvent, render, screen } from "@testing-library/react";
import { afterEach, beforeEach, describe, expect, test, vi } from "vitest";
import FormularioToken from "./FormularioToken";

describe("FormularioToken", () => {
  beforeEach(() => {
    vi.useFakeTimers();
  });

  afterEach(() => {
    vi.useRealTimers();
    vi.clearAllMocks();
  });

  test("[SPEC:OTP-003] OTP expirado dispara flujo de expiración", async () => {
    const onExpiredNavigate = vi.fn();

    render(
      <FormularioToken
        email="u***@mail.com"
        tiempoExpira={1 / 60}
        expired={false}
        onSubmit={() => undefined}
        onExpiredNavigate={onExpiredNavigate}
      />
    );

    act(() => {
      vi.advanceTimersByTime(1000);
    });

    expect(screen.getByText(/código vencido/i)).toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: /volver/i }));

    expect(onExpiredNavigate).toHaveBeenCalledTimes(1);
  });
});
