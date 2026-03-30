import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { AppContent } from "./AppContent";
import styles from "./AppContent.module.css";

describe("AppContent [SPEC:APP-CONTENT-001]", () => {
  it("renderiza el contenido principal sin regiones opcionales", () => {
    render(<AppContent>Contenido principal</AppContent>);

    expect(screen.getByText("Contenido principal")).toBeInTheDocument();
    expect(screen.queryByText("Cabecera")).toBeNull();
    expect(screen.queryByText("Pie")).toBeNull();
  });

  it("renderiza header y footer cuando se suministran", () => {
    render(
      <AppContent header={<h2>Cabecera</h2>} footer={<button type="button">Pie</button>}>
        Cuerpo
      </AppContent>,
    );

    expect(screen.getByRole("heading", { name: "Cabecera" })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Pie" })).toBeInTheDocument();
    expect(screen.getByText("Cuerpo")).toBeInTheDocument();
  });

  it("aplica variantes de ancho, densidad y clases adicionales", () => {
    const { container } = render(
      <AppContent
        width="wide"
        density="compact"
        className="custom-root"
        contentClassName="custom-body"
      >
        Variantes
      </AppContent>,
    );

    const root = screen.getByText("Variantes").closest("section");
    const body = container.querySelector(".custom-body");
    expect(root).toHaveClass(styles.widthWide);
    expect(root).toHaveClass(styles.densityCompact);
    expect(root).toHaveClass("custom-root");
    expect(body).toHaveClass("custom-body");
  });

  it("permite cambiar el elemento semantico raiz", () => {
    render(<AppContent as="main">Semantica</AppContent>);

    expect(screen.getByText("Semantica").closest("main")).toBeInTheDocument();
  });
});
