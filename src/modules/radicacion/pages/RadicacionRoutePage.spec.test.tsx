import { describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
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
    const queryClient = new QueryClient({
      defaultOptions: {
        queries: {
          retry: false,
        },
      },
    });

    render(
      <QueryClientProvider client={queryClient}>
        <RadicacionRoutePage />
      </QueryClientProvider>,
    );

    fireEvent.click(screen.getByRole("tab", { name: /Radicación/i }));

    expect(screen.getByRole("button", { name: /Radicar/i })).toBeInTheDocument();
  });
});
