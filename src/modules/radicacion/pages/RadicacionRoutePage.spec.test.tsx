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

vi.mock("../hooks/useRadicacionEstadoActivo", () => ({
  useRadicacionEstadoActivo: () => ({
    data: null,
    contextoDocumental: null,
    isLoading: false,
    isFetching: false,
    isError: false,
    error: null,
    refetch: vi.fn(),
  }),
}));

describe("RadicacionRoutePage", () => {
  it("mounts RadicacionPage using internal default plantilla", async () => {
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

    fireEvent.click(await screen.findByRole("tab", { name: /Radicación/i }));

    expect(await screen.findByRole("button", { name: /Radicar/i })).toBeInTheDocument();
  }, 10000);
});
