import { describe, expect, it } from "vitest";
import { render, screen } from "@testing-library/react";
import RadicacionRoutePage from "./RadicacionRoutePage";

describe("RadicacionRoutePage", () => {
  it("mounts RadicacionPage using internal default plantilla", () => {
    render(<RadicacionRoutePage />);

    expect(screen.getByRole("button", { name: "Radicar" })).toBeInTheDocument();
  });
});
