import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { AppContasoftLoader } from "./AppContasoftLoader";

describe("[SPEC:app-contasoft-loader] AppContasoftLoader", () => {
  it("renderiza el isotipo Contasoft como SVG accesible", () => {
    const { container } = render(<AppContasoftLoader label="Procesando digitalizacion" />);

    expect(screen.getByRole("img", { name: "Procesando digitalizacion" })).toBeInTheDocument();
    expect(container.querySelector("svg")).toBeInTheDocument();
    expect(container.querySelector("canvas")).not.toBeInTheDocument();
    expect(container.querySelector("img")).not.toBeInTheDocument();
  });

  it("permite ajustar el tamano sin estado React", () => {
    render(<AppContasoftLoader size={80} />);

    expect(screen.getByRole("img", { name: "Loader Contasoft" })).toHaveStyle({
      "--app-contasoft-loader-size": "80px",
    });
  });
});
