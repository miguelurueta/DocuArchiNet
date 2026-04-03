import { describe, expect, it } from "vitest";
import type { AppTableQueryState } from "../types/appTableQueryState.types";
import {
  getDefaultAppTableQueryState,
  serializeAppTableQueryState,
  updateAppTableQueryState,
} from "../utils/appTableQueryState";

describe("[SPEC:SCRUMCORE-39] appTableQueryState helpers", () => {
  it("returns the documented default query state", () => {
    expect(getDefaultAppTableQueryState()).toEqual({
      page: 1,
      pageSize: 25,
      search: "",
      structuredFilters: [],
      sortField: undefined,
      sortDir: undefined,
      searchType: undefined,
    });
  });

  it("resets page when search changes effectively", () => {
    const result = updateAppTableQueryState(
      {
        ...getDefaultAppTableQueryState(),
        page: 4,
        search: "actual",
      },
      {
        search: "nuevo",
      },
    );

    expect(result.page).toBe(1);
    expect(result.search).toBe("nuevo");
  });

  it("does not reset page when search value does not change effectively", () => {
    const result = updateAppTableQueryState(
      {
        ...getDefaultAppTableQueryState(),
        page: 4,
        search: "igual",
      },
      {
        search: "igual",
      },
    );

    expect(result.page).toBe(4);
  });

  it("resets page when structuredFilters change effectively", () => {
    const prev: AppTableQueryState = {
      ...getDefaultAppTableQueryState(),
      page: 7,
      structuredFilters: [
        {
          field: "estado",
          operator: "eq",
          value: "abierto",
        },
      ],
    };

    const result = updateAppTableQueryState(prev, {
      structuredFilters: [
        {
          field: "estado",
          operator: "eq",
          value: "cerrado",
        },
      ],
    });

    expect(result.page).toBe(1);
  });

  it("does not reset page when structuredFilters only change by reference", () => {
    const filters: AppTableQueryState["structuredFilters"] = [
      {
        field: "estado",
        operator: "eq",
        value: "abierto",
      },
    ];

    const result = updateAppTableQueryState(
      {
        ...getDefaultAppTableQueryState(),
        page: 6,
        structuredFilters: filters,
      },
      {
        structuredFilters: [
          {
            field: "estado",
            operator: "eq",
            value: "abierto",
          },
        ],
      },
    );

    expect(result.page).toBe(6);
  });

  it("resets page when sort changes", () => {
    const result = updateAppTableQueryState(
      {
        ...getDefaultAppTableQueryState(),
        page: 5,
        sortField: "fecha_inicio",
        sortDir: "desc",
      },
      {
        sortDir: "asc",
      },
    );

    expect(result.page).toBe(1);
    expect(result.sortDir).toBe("asc");
  });

  it("resets page when pageSize changes", () => {
    const result = updateAppTableQueryState(
      {
        ...getDefaultAppTableQueryState(),
        page: 9,
        pageSize: 25,
      },
      {
        pageSize: 50,
      },
    );

    expect(result.page).toBe(1);
    expect(result.pageSize).toBe(50);
  });

  it("preserves other fields when only page changes", () => {
    const result = updateAppTableQueryState(
      {
        ...getDefaultAppTableQueryState(),
        page: 2,
        search: "tramite",
        sortField: "fecha_inicio",
        sortDir: "desc",
      },
      {
        page: 3,
      },
    );

    expect(result).toEqual({
      ...getDefaultAppTableQueryState(),
      page: 3,
      search: "tramite",
      sortField: "fecha_inicio",
      sortDir: "desc",
    });
  });

  it("serializes the query state with between filters", () => {
    const serialized = serializeAppTableQueryState({
      page: 3,
      pageSize: 50,
      search: "tramite",
      searchType: 3,
      structuredFilters: [
        {
          field: "fecha_inicio",
          operator: "between",
          valueFrom: "2026-01-01",
          valueTo: "2026-12-31",
        },
      ],
      sortField: "fecha_inicio",
      sortDir: "desc",
    });

    expect(serialized).toEqual({
      page: 3,
      pageSize: 50,
      search: "tramite",
      searchType: 3,
      structuredFilters: [
        {
          field: "fecha_inicio",
          operator: "between",
          value: undefined,
          valueFrom: "2026-01-01",
          valueTo: "2026-12-31",
        },
      ],
      sortField: "fecha_inicio",
      sortDir: "desc",
    });
  });
});
