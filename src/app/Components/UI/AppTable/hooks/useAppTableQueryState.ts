import { useMemo, useState } from "react";
import type { AppTableQueryState } from "../types/appTableQueryState.types";
import {
  createAppTableQueryState,
  serializeAppTableQueryState,
  updateAppTableQueryState,
} from "../utils/appTableQueryState";

export type UseAppTableQueryStateResult = {
  queryState: AppTableQueryState;
  onQueryChange: (patch: Partial<AppTableQueryState>) => void;
  setQueryState: (nextState: AppTableQueryState) => void;
  serializedQueryState: Record<string, unknown>;
};

export const useAppTableQueryState = (
  initialState?: Partial<AppTableQueryState>,
): UseAppTableQueryStateResult => {
  const [queryState, setQueryState] = useState<AppTableQueryState>(() => createAppTableQueryState(initialState));

  const serializedQueryState = useMemo(
    () => serializeAppTableQueryState(queryState),
    [queryState],
  );

  return {
    queryState,
    onQueryChange: (patch) => {
      setQueryState((prev) => updateAppTableQueryState(prev, patch));
    },
    setQueryState,
    serializedQueryState,
  };
};
