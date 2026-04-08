import { describe, expect, it } from "vitest";
import { mapGestionCorrespondenciaTableRequest } from "../adapters/gestionCorrespondenciaTableRequestMapper";

describe("mapGestionCorrespondenciaTableRequest [SPEC:gestion-correspondencia]", () => {
  it("envía SearchType 2 cuando hay búsqueda simple efectiva", () => {
    expect(
      mapGestionCorrespondenciaTableRequest({
        tableId: "workflowInboxgestion",
        page: 2,
        pageSize: 25,
        search: "  radicado  ",
        sortField: "fecha_inicio",
        sortDir: "desc",
        includeConfig: true,
      }),
    ).toEqual(
      expect.objectContaining({
        TableId: "workflowInboxgestion",
        Page: 2,
        PageSize: 25,
        Search: "radicado",
        SearchType: 2,
        SortField: "fecha_inicio",
        SortDir: "DESC",
        IncludeConfig: true,
      }),
    );
  });

  it("no fuerza SearchType 2 cuando no hay texto efectivo", () => {
    expect(
      mapGestionCorrespondenciaTableRequest({
        tableId: "workflowInboxgestion",
        search: "   ",
      }),
    ).toEqual(
      expect.objectContaining({
        Search: undefined,
        SearchType: undefined,
      }),
    );
  });

  it("preserva SearchType 3 para búsqueda avanzada", () => {
    expect(
      mapGestionCorrespondenciaTableRequest({
        tableId: "workflowInboxgestion",
        search: "radicado",
        searchType: 3,
      }),
    ).toEqual(
      expect.objectContaining({
        Search: "radicado",
        SearchType: 3,
      }),
    );
  });

  it("preserva filtros estructurados, paginación y ordenamiento", () => {
    expect(
      mapGestionCorrespondenciaTableRequest({
        tableId: "workflowInboxgestion",
        page: 1,
        pageSize: 50,
        search: "alpha",
        structuredFilters: [
          {
            field: "estado",
            operator: "eq",
            value: "Pendiente",
          },
        ],
        sortField: "fecha_inicio",
        sortDir: "asc",
        includeConfig: false,
      }),
    ).toEqual(
      expect.objectContaining({
        Page: 1,
        PageSize: 50,
        SearchType: 2,
        StructuredFilters: [
          {
            Field: "estado",
            Operator: "eq",
            Value: "Pendiente",
            ValueFrom: undefined,
            ValueTo: undefined,
          },
        ],
        SortField: "fecha_inicio",
        SortDir: "ASC",
        IncludeConfig: false,
      }),
    );
  });
});
