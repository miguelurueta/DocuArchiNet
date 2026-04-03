import { renderHook, act } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { useAppTableQueryState } from "../hooks/useAppTableQueryState";

describe("[SPEC:SCRUMCORE-39] useAppTableQueryState", () => {
  it("builds initial state from defaults plus a partial override", () => {
    const { result } = renderHook(() =>
      useAppTableQueryState({
        search: "tramite",
        pageSize: 50,
      }),
    );

    expect(result.current.queryState).toEqual({
      page: 1,
      pageSize: 50,
      search: "tramite",
      structuredFilters: [],
      sortField: undefined,
      sortDir: undefined,
      searchType: undefined,
    });
  });

  it("updates the query state through onQueryChange", () => {
    const { result } = renderHook(() => useAppTableQueryState());

    act(() => {
      result.current.onQueryChange({
        page: 4,
      });
    });

    expect(result.current.queryState.page).toBe(4);

    act(() => {
      result.current.onQueryChange({
        search: "nuevo",
      });
    });

    expect(result.current.queryState.page).toBe(1);
    expect(result.current.queryState.search).toBe("nuevo");
  });

  it("keeps a serialized view aligned with the current state", () => {
    const { result } = renderHook(() => useAppTableQueryState());

    act(() => {
      result.current.onQueryChange({
        searchType: 2,
        structuredFilters: [
          {
            field: "estado",
            operator: "eq",
            value: "abierto",
          },
        ],
      });
    });

    expect(result.current.serializedQueryState).toEqual({
      page: 1,
      pageSize: 25,
      search: "",
      searchType: 2,
      structuredFilters: [
        {
          field: "estado",
          operator: "eq",
          value: "abierto",
          valueFrom: undefined,
          valueTo: undefined,
        },
      ],
      sortField: undefined,
      sortDir: undefined,
    });
  });
});
