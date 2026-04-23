import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { AppSteps } from "./AppSteps";

const baseItems = [
  { key: "step-1", title: "Paso 1" },
  { key: "step-2", title: "Paso 2" },
  { key: "step-3", title: "Paso 3", disabled: true },
];

const makeMatchMedia = (matches: boolean) =>
  vi.fn().mockImplementation(() => ({
    matches,
    media: "(max-width: 900px)",
    addEventListener: vi.fn(),
    removeEventListener: vi.fn(),
  }));

describe("AppSteps [SPEC:APP-APPSTEPS-01-FE]", () => {
  it("renderiza items y permite cambio de step habilitado", async () => {
    const onChange = vi.fn();

    render(<AppSteps items={baseItems} defaultCurrent={0} onChange={onChange} />);

    fireEvent.click(screen.getByText("Paso 2"));

    await waitFor(() => {
      expect(onChange).toHaveBeenCalledWith(1);
    });
    expect(screen.getByText("Paso 2")).toHaveAttribute("aria-current", "step");
  });

  it("bloquea cambio en step disabled", async () => {
    const onChange = vi.fn();

    render(<AppSteps items={baseItems} defaultCurrent={0} onChange={onChange} />);
    fireEvent.click(screen.getByText("Paso 3"));

    await waitFor(() => {
      expect(onChange).not.toHaveBeenCalled();
    });
    expect(screen.getByText("Paso 1")).toHaveAttribute("aria-current", "step");
  });

  it("respeta modo controlado con current", () => {
    const { rerender } = render(
      <AppSteps items={baseItems} current={0} onChange={vi.fn()} />,
    );

    expect(screen.getByText("Paso 1")).toHaveAttribute("aria-current", "step");

    rerender(<AppSteps items={baseItems} current={1} onChange={vi.fn()} />);
    expect(screen.getByText("Paso 2")).toHaveAttribute("aria-current", "step");
  });

  it("respeta modo no controlado con defaultCurrent", () => {
    render(<AppSteps items={baseItems} defaultCurrent={1} />);

    expect(screen.getByText("Paso 2")).toHaveAttribute("aria-current", "step");
  });

  it("en variant form bloquea avance cuando validateStep falla", async () => {
    const onChange = vi.fn();
    const validateStep = vi.fn(() => false);

    render(
      <AppSteps
        items={baseItems}
        variant="form"
        defaultCurrent={0}
        validateStep={validateStep}
        onChange={onChange}
      />,
    );

    fireEvent.click(screen.getByText("Paso 2"));

    await waitFor(() => {
      expect(validateStep).toHaveBeenCalledWith(0);
    });
    expect(onChange).not.toHaveBeenCalled();
    expect(
      screen.getByText("Paso 1").closest(".ant-steps-item"),
    ).toHaveClass("ant-steps-item-error");
    expect(screen.getByText("Paso 1")).toHaveAttribute("aria-current", "step");
  });

  it("en variant form permite avance cuando validateStep retorna true", async () => {
    const onChange = vi.fn();
    const validateStep = vi.fn(() => true);

    render(
      <AppSteps
        items={baseItems}
        variant="form"
        defaultCurrent={0}
        validateStep={validateStep}
        onChange={onChange}
      />,
    );

    fireEvent.click(screen.getByText("Paso 2"));

    await waitFor(() => {
      expect(validateStep).toHaveBeenCalledWith(0);
    });
    await waitFor(() => {
      expect(onChange).toHaveBeenCalledWith(1);
    });
    expect(screen.getByText("Paso 2")).toHaveAttribute("aria-current", "step");
  });

  it("soporta validateStep async", async () => {
    const onChange = vi.fn();
    const validateStep = vi.fn(async () => true);

    render(
      <AppSteps
        items={baseItems}
        variant="form"
        defaultCurrent={0}
        validateStep={validateStep}
        onChange={onChange}
      />,
    );

    fireEvent.click(screen.getByText("Paso 2"));

    await waitFor(() => {
      expect(validateStep).toHaveBeenCalledWith(0);
    });
    await waitFor(() => {
      expect(onChange).toHaveBeenCalledWith(1);
    });
  });
});

describe("AppSteps [SPEC:APP-APPSTEPS-02-FE]", () => {
  it("renderiza progreso global cuando variant progress recibe progressPercent", () => {
    render(
      <AppSteps
        items={baseItems}
        variant="progress"
        progressPercent={68}
        defaultCurrent={1}
      />,
    );

    expect(screen.getByText("68%")).toBeInTheDocument();
  });

  it("no renderiza bloque de progreso cuando falta progressPercent", () => {
    render(<AppSteps items={baseItems} variant="progress" defaultCurrent={1} />);

    expect(screen.queryByText(/%/)).not.toBeInTheDocument();
  });

  it("timeline fuerza orientacion vertical y renderiza timestamp", () => {
    render(
      <AppSteps
        items={[
          {
            key: "t1",
            title: "Recepcion",
            description: "Solicitud recibida",
            timestamp: "2026-04-23 09:15",
            status: "process",
          },
          { key: "t2", title: "Revision", timestamp: "2026-04-23 10:30" },
        ]}
        variant="timeline"
        direction="horizontal"
      />,
    );

    expect(screen.getByText("2026-04-23 09:15")).toBeInTheDocument();
    const steps = document.querySelector(".ant-steps");
    expect(steps).toHaveClass("ant-steps-vertical");
  });

  it("hace fallback vertical en viewport angosto para variantes horizontales", () => {
    const originalMatchMedia = window.matchMedia;
    const mock = makeMatchMedia(true);
    window.matchMedia = mock;

    render(<AppSteps items={baseItems} variant="progress" defaultCurrent={0} responsive />);

    const steps = document.querySelector(".ant-steps");
    expect(steps).toHaveClass("ant-steps-vertical");
    expect(mock).toHaveBeenCalled();

    window.matchMedia = originalMatchMedia;
  });

  it("expone señales semánticas de estado además del color", () => {
    render(
      <AppSteps
        items={[
          { key: "s1", title: "Paso activo", status: "process" },
          { key: "s2", title: "Paso error", status: "error" },
        ]}
        defaultCurrent={0}
      />,
    );

    expect(screen.getByText(/estado process/i)).toBeInTheDocument();
    expect(screen.getByText(/estado error/i)).toBeInTheDocument();
    expect(screen.getByText("Paso activo")).toHaveAttribute("aria-current", "step");
  });
});
