import { useCallback, useEffect, useRef, useState } from "react";
import { getWorkflowInboxAutocomplete } from "../services/workflowInboxAutocomplete.service";
import type { WorkflowInboxAutocompleteItem } from "../types/workflowInboxAutocomplete.types";

const DEFAULT_AUTOCOMPLETE_DEBOUNCE_MS = 300;

export type UseWorkflowInboxAutocompleteParams = {
  minLength: number;
  limit: number;
};

export type UseWorkflowInboxAutocompleteResult = {
  items: WorkflowInboxAutocompleteItem[];
  loading: boolean;
  error: Error | null;
  setSearchText: (value: string) => void;
  clear: () => void;
};

const toError = (error: unknown): Error =>
  error instanceof Error ? error : new Error("No se pudieron cargar sugerencias");

export const useWorkflowInboxAutocomplete = ({
  minLength,
  limit,
}: UseWorkflowInboxAutocompleteParams): UseWorkflowInboxAutocompleteResult => {
  const [searchText, setSearchTextState] = useState("");
  const [items, setItems] = useState<WorkflowInboxAutocompleteItem[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<Error | null>(null);
  const requestIdRef = useRef(0);

  const clear = useCallback(() => {
    requestIdRef.current += 1;
    setSearchTextState("");
    setItems([]);
    setLoading(false);
    setError(null);
  }, []);

  const setSearchText = useCallback((value: string) => {
    setSearchTextState(value);
  }, []);

  const trimmedSearch = searchText.trim();
  const meetsMinLength = trimmedSearch.length >= minLength;

  useEffect(() => {
    if (!meetsMinLength) {
      requestIdRef.current += 1;
      return;
    }

    const timeoutId = window.setTimeout(() => {
      const requestId = requestIdRef.current + 1;
      requestIdRef.current = requestId;
      setLoading(true);
      setError(null);

      void getWorkflowInboxAutocomplete({
        search: trimmedSearch,
        limit,
      })
        .then((nextItems) => {
          if (requestIdRef.current === requestId) {
            setItems(nextItems);
          }
        })
        .catch((nextError: unknown) => {
          if (requestIdRef.current === requestId) {
            setItems([]);
            setError(toError(nextError));
          }
        })
        .finally(() => {
          if (requestIdRef.current === requestId) {
            setLoading(false);
          }
        });
    }, DEFAULT_AUTOCOMPLETE_DEBOUNCE_MS);

    return () => {
      window.clearTimeout(timeoutId);
    };
  }, [limit, meetsMinLength, trimmedSearch]);

  return {
    items: meetsMinLength ? items : [],
    loading: meetsMinLength ? loading : false,
    error: meetsMinLength ? error : null,
    setSearchText,
    clear,
  };
};
