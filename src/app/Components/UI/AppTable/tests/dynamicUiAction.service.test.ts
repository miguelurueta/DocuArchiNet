import { beforeEach, describe, expect, it, vi } from "vitest";
import clienteApi from "../../../../../api/Clienteaxios";
import {
  createDynamicUiActionService,
  DEFAULT_DYNAMIC_UI_ACTION_ENDPOINT,
  executeDynamicUiAction,
} from "../services/dynamicUiAction.service";

vi.mock("../../../../../api/Clienteaxios", () => ({
  default: {
    post: vi.fn(),
  },
}));

describe("[SPEC:CREA-ACTION-LAYER-AG-GRID] dynamicUiAction.service", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("delegates action execution to clienteApi and preserves the backend contract", async () => {
    const request = {
      tableId: "workflowInboxgestion",
      actionId: "gestionar_tramite",
      rowId: "924",
    };
    const responseData = {
      success: true,
      message: "OK",
      data: {
        result: "done",
      },
      errors: [],
    };

    vi.mocked(clienteApi.post).mockResolvedValue({
      data: responseData,
    });

    const result = await executeDynamicUiAction(request);

    expect(clienteApi.post).toHaveBeenCalledWith(DEFAULT_DYNAMIC_UI_ACTION_ENDPOINT, request);
    expect(result).toEqual(responseData);
  });

  it("supports an injected endpoint for compatible action backends", async () => {
    const endpoint = "/api/otro-modulo/actions";
    const request = {
      tableId: "otraTabla",
      actionId: "aprobar",
    };
    const responseData = {
      success: true,
      message: "OK",
      data: null,
      errors: [],
    };

    vi.mocked(clienteApi.post).mockResolvedValue({
      data: responseData,
    });

    const result = await executeDynamicUiAction(endpoint, request);

    expect(clienteApi.post).toHaveBeenCalledWith(endpoint, request);
    expect(result).toEqual(responseData);
  });

  it("can build a bound service for a specific endpoint", async () => {
    const endpoint = "/api/gestion/documental/actions";
    const service = createDynamicUiActionService(endpoint);
    const request = {
      tableId: "workflowInboxgestion",
      actionId: "archivar_tramite",
      selectedRowIds: ["924", "923"],
    };
    const responseData = {
      success: true,
      message: "OK",
      data: {
        processed: 2,
      },
      errors: [],
    };

    vi.mocked(clienteApi.post).mockResolvedValue({
      data: responseData,
    });

    const result = await service(request);

    expect(clienteApi.post).toHaveBeenCalledWith(endpoint, request);
    expect(result).toEqual(responseData);
  });
});
