import { describe, expect, it, vi } from "vitest";
import { render, screen } from "@testing-library/react";
import RadicacionRoutePage from "./RadicacionRoutePage";

vi.mock("../hooks/useCamposPlantilla", () => ({
  useCamposPlantilla: () => ({
    data: [],
    isLoading: false,
    error: null,
    refetch: vi.fn(),
  }),
}));

describe("RadicacionRoutePage", () => {
  it("mounts RadicacionPage using internal default plantilla", () => {
    render(<RadicacionRoutePage />);

    expect(screen.getByRole("button", { name: "Radicar" })).toBeInTheDocument();
  });
});
