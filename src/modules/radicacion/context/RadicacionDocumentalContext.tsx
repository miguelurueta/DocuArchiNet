import { useCallback, useMemo, useState, type ReactNode } from "react";
import type {
  RadicacionDocumentalContextValue,
  RadicacionDocumentalState,
} from "../types/radicacionDocumental.types";
import {
  RADICACION_DOCUMENTAL_INITIAL_STATE,
  RadicacionDocumentalContext,
  normalizeRadicacionDocumentalState,
} from "./radicacionDocumentalContextValue";

interface RadicacionDocumentalProviderProps {
  children: ReactNode;
  initialState?: RadicacionDocumentalState;
}

export function RadicacionDocumentalProvider({
  children,
  initialState = RADICACION_DOCUMENTAL_INITIAL_STATE,
}: RadicacionDocumentalProviderProps) {
  const [state, setState] = useState<RadicacionDocumentalState>(() =>
    normalizeRadicacionDocumentalState(initialState),
  );

  const setContextoDocumental = useCallback(
    (value: RadicacionDocumentalState) => {
      setState(normalizeRadicacionDocumentalState(value));
    },
    [],
  );

  const clearContextoDocumental = useCallback(() => {
    setState(RADICACION_DOCUMENTAL_INITIAL_STATE);
  }, []);

  const value = useMemo<RadicacionDocumentalContextValue>(
    () => ({
      ...state,
      setContextoDocumental,
      clearContextoDocumental,
    }),
    [clearContextoDocumental, setContextoDocumental, state],
  );

  return (
    <RadicacionDocumentalContext.Provider value={value}>
      {children}
    </RadicacionDocumentalContext.Provider>
  );
}
