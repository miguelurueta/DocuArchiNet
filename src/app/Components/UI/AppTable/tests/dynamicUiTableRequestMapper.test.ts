import { describe, expect, it } from "vitest";
import { mapDynamicUiServerTableRequest } from "../utils/dynamicUiTableRequestMapper";

describe("dynamicUiTableRequestMapper", () => {
  it("serializa búsqueda server completa en el shape backend-compatible", () => {
    expect(
      mapDynamicUiServerTableRequest({
        tableId: "workflowInboxgestion",
        page: 3,
        pageSize: 25,
        search: "  tramite  ",
        searchType: 2,
        structuredFilters: [
          {
            field: "fecha_inicio",
            operator: "between",
            valueFrom: "2026-01-01",
            valueTo: "2026-01-31",
          },
        ],
        sortField: "fecha_inicio",
        sortDir: "desc",
        includeConfig: true,
      }),
    ).toEqual({
      TableId: "workflowInboxgestion",
      Page: 3,
      PageSize: 25,
      Search: "tramite",
      SearchType: 2,
      StructuredFilters: [
        {
          Field: "fecha_inicio",
          Operator: "between",
          Value: undefined,
          ValueFrom: "2026-01-01",
          ValueTo: "2026-01-31",
        },
      ],
      SortField: "fecha_inicio",
      SortDir: "DESC",
      IncludeConfig: true,
    });
  });

  it("omite quick pieces vacías y soporta sortDirection legacy", () => {
    expect(
      mapDynamicUiServerTableRequest({
        tableId: "workflowInboxgestion",
        search: "   ",
        structuredFilters: [],
        sortDirection: "asc",
      }),
    ).toEqual({
      TableId: "workflowInboxgestion",
      Page: undefined,
      PageSize: undefined,
      Search: undefined,
      SearchType: undefined,
      StructuredFilters: undefined,
      SortField: undefined,
      SortDir: "ASC",
      IncludeConfig: undefined,
    });
  });
});
