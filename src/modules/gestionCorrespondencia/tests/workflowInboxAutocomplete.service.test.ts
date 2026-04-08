import { beforeEach, describe, expect, it, vi } from "vitest";
import clienteApi from "../../../api/Clienteaxios";
import {
  getWorkflowInboxAutocomplete,
  WORKFLOW_INBOX_AUTOCOMPLETE_ENDPOINT,
} from "../services/workflowInboxAutocomplete.service";

vi.mock("../../../api/Clienteaxios", () => ({
  default: {
    post: vi.fn(),
  },
}));

describe("[SPEC:gestion-correspondencia] workflowInboxAutocomplete.service", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("llama el endpoint esperado con search y limit controlados", async () => {
    vi.mocked(clienteApi.post).mockResolvedValue({
      data: {
        success: true,
        data: {
          items: [],
        },
        errors: [],
      },
    });

    await getWorkflowInboxAutocomplete({
      search: "radicado",
      limit: 10,
    });

    expect(clienteApi.post).toHaveBeenCalledWith(
      WORKFLOW_INBOX_AUTOCOMPLETE_ENDPOINT,
      {
        search: "radicado",
        limit: 10,
      },
    );
  });

  it("mapea la respuesta backend al contrato value/label sin filtrar campos extra", async () => {
    vi.mocked(clienteApi.post).mockResolvedValue({
      data: {
        success: true,
        data: {
          items: [
            {
              value: "RAD-1",
              label: "Radicado 1",
              field: "RADICADO",
            },
            {
              Value: "RAD-2",
              Label: "Radicado 2",
              Field: "TRAMITE",
            },
          ],
        },
        errors: [],
      },
    });

    await expect(
      getWorkflowInboxAutocomplete({
        search: "rad",
        limit: 10,
      }),
    ).resolves.toEqual([
      {
        value: "RAD-1",
        label: "Radicado 1",
      },
      {
        value: "RAD-2",
        label: "Radicado 2",
      },
    ]);
  });
});
