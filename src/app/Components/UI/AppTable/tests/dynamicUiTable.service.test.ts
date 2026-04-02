import { describe, expect, it, vi, beforeEach } from "vitest";
import clienteApi from "../../../../../api/Clienteaxios";
import {
  createDynamicTableService,
  DEFAULT_DYNAMIC_UI_TABLE_ENDPOINT,
  getDynamicTable,
} from "../services/dynamicUiTable.service";

vi.mock("../../../../../api/Clienteaxios", () => ({
  default: {
    post: vi.fn(),
  },
}));

describe("[SPEC:CREA-QUERY-AG-GRID-FASE3] dynamicUiTable.service", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("delegates the request to clienteApi and preserves the backend response contract", async () => {
    const request = {
      tableId: "workflowInboxgestion",
      page: 1,
      pageSize: 25,
    };
    const responseData = {
      success: true,
      message: "OK",
      data: {
        TableId: "workflowInboxgestion",
        Columns: [],
        Rows: [],
      },
      meta: {
        source: "backend",
      },
      errors: [],
    };

    vi.mocked(clienteApi.post).mockResolvedValue({
      data: responseData,
    });

    const result = await getDynamicTable(request);

    expect(clienteApi.post).toHaveBeenCalledWith(DEFAULT_DYNAMIC_UI_TABLE_ENDPOINT, request);
    expect(result).toEqual(responseData);
  });

  it("supports an injected endpoint for other compatible dynamic tables", async () => {
    const endpoint = "/api/otro-modulo/otra-tabla";
    const request = {
      tableId: "otraTabla",
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

    const result = await getDynamicTable(endpoint, request);

    expect(clienteApi.post).toHaveBeenCalledWith(endpoint, request);
    expect(result).toEqual(responseData);
  });

  it("can build a bound service for a specific endpoint", async () => {
    const endpoint = "/api/gestion/documental/bandeja";
    const service = createDynamicTableService(endpoint);
    const request = {
      tableId: "bandeja",
      page: 2,
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

    const result = await service(request);

    expect(clienteApi.post).toHaveBeenCalledWith(endpoint, request);
    expect(result).toEqual(responseData);
  });
});
