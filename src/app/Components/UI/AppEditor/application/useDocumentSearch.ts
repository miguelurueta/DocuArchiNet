import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import type { RefObject } from "react";
import type { Editor } from "@tiptap/react";

type UseDocumentSearchOptions = {
  editor: Editor | null;
  canvasRef: RefObject<HTMLElement | null>;
  query: string;
  enabled: boolean;
};

type SearchMatch = {
  range: Range;
};

type HighlightRegistry = {
  set: (name: string, highlight: unknown) => void;
  delete: (name: string) => void;
};

type HighlightConstructor = new (...ranges: Range[]) => unknown;

const SEARCH_HIGHLIGHT_NAME = "app-editor-search-match";
const ACTIVE_HIGHLIGHT_NAME = "app-editor-search-active";
const MAX_SEARCH_MATCHES = 2000;

function getHighlightApi() {
  if (typeof CSS === "undefined" || typeof window === "undefined") {
    return null;
  }

  const cssWithHighlights = CSS as typeof CSS & {
    highlights?: HighlightRegistry;
  };
  const windowWithHighlight = window as typeof window & {
    Highlight?: HighlightConstructor;
  };

  if (!cssWithHighlights.highlights || !windowWithHighlight.Highlight) {
    return null;
  }

  return {
    registry: cssWithHighlights.highlights,
    Highlight: windowWithHighlight.Highlight,
  };
}

function normalizeQuery(value: string) {
  return value.trim().toLocaleLowerCase();
}

function resolveSearchRoot(editor: Editor | null) {
  const root = editor?.view?.dom;

  return root instanceof HTMLElement ? root : null;
}

function collectTextNodes(root: HTMLElement) {
  const textNodes: Text[] = [];
  const nodeFilter = document.defaultView?.NodeFilter;

  if (!nodeFilter) {
    return textNodes;
  }

  const walker = document.createTreeWalker(root, nodeFilter.SHOW_TEXT, {
    acceptNode: (node) =>
      node.textContent?.trim()
        ? nodeFilter.FILTER_ACCEPT
        : nodeFilter.FILTER_REJECT,
  });

  let currentNode = walker.nextNode();
  while (currentNode) {
    if (currentNode instanceof Text) {
      textNodes.push(currentNode);
    }

    currentNode = walker.nextNode();
  }

  return textNodes;
}

function collectSearchMatches(editor: Editor | null, query: string): SearchMatch[] {
  const root = resolveSearchRoot(editor);
  const normalizedQuery = normalizeQuery(query);

  if (!root || !normalizedQuery) {
    return [];
  }

  const chunks = collectTextNodes(root).map((node) => ({
    node,
    text: node.textContent ?? "",
    start: 0,
    end: 0,
  }));
  let fullText = "";

  chunks.forEach((chunk) => {
    chunk.start = fullText.length;
    fullText += chunk.text;
    chunk.end = fullText.length;
  });

  const normalizedText = fullText.toLocaleLowerCase();
  const matches: SearchMatch[] = [];
  let searchFrom = 0;

  while (matches.length < MAX_SEARCH_MATCHES) {
    const matchIndex = normalizedText.indexOf(normalizedQuery, searchFrom);
    if (matchIndex === -1) {
      break;
    }

    const matchEnd = matchIndex + normalizedQuery.length;
    const startChunk = chunks.find(
      (chunk) => matchIndex >= chunk.start && matchIndex < chunk.end,
    );
    const endChunk = chunks.find(
      (chunk) => matchEnd > chunk.start && matchEnd <= chunk.end,
    );

    if (startChunk && endChunk) {
      const range = document.createRange();
      range.setStart(startChunk.node, matchIndex - startChunk.start);
      range.setEnd(endChunk.node, matchEnd - endChunk.start);
      matches.push({ range });
    }

    searchFrom = matchIndex + Math.max(1, normalizedQuery.length);
  }

  return matches;
}

function scrollRangeIntoCanvas(
  range: Range,
  canvas: HTMLElement | null,
  behavior: ScrollBehavior = "smooth",
) {
  if (!canvas) {
    return;
  }

  if (typeof range.getClientRects !== "function") {
    return;
  }

  const rect = range.getClientRects()[0] ?? range.getBoundingClientRect();
  if (!rect || rect.height === 0) {
    return;
  }

  const canvasRect = canvas.getBoundingClientRect();
  const maxTop = Math.max(0, canvas.scrollHeight - canvas.clientHeight);
  const targetTop = Math.min(
    maxTop,
    Math.max(0, canvas.scrollTop + rect.top - canvasRect.top - canvas.clientHeight * 0.28),
  );

  canvas.scrollTo({ top: targetTop, behavior });
}

export function useDocumentSearch({
  editor,
  canvasRef,
  query,
  enabled,
}: UseDocumentSearchOptions) {
  const [activeIndex, setActiveIndex] = useState(0);
  const [documentVersion, setDocumentVersion] = useState(0);
  const previousQueryRef = useRef("");
  const normalizedQuery = normalizeQuery(query);
  const matches = useMemo(() => {
    if (!enabled) {
      return [];
    }

    void documentVersion;
    return collectSearchMatches(editor, normalizedQuery);
  }, [documentVersion, editor, enabled, normalizedQuery]);

  useEffect(() => {
    if (!editor || !enabled) {
      return undefined;
    }

    const handleTransaction = () => {
      setDocumentVersion((previousVersion) => previousVersion + 1);
    };

    editor.on("transaction", handleTransaction);

    return () => {
      editor.off("transaction", handleTransaction);
    };
  }, [editor, enabled]);

  useEffect(() => {
    if (previousQueryRef.current !== normalizedQuery) {
      previousQueryRef.current = normalizedQuery;
      setActiveIndex(0);
      return;
    }

    setActiveIndex((previousIndex) =>
      matches.length === 0 ? 0 : Math.min(previousIndex, matches.length - 1),
    );
  }, [matches.length, normalizedQuery]);

  useEffect(() => {
    const api = getHighlightApi();
    if (!api || !enabled) {
      return undefined;
    }

    const activeRange = matches[activeIndex]?.range;
    const passiveRanges = matches
      .filter((_, index) => index !== activeIndex)
      .map((match) => match.range);

    api.registry.set(SEARCH_HIGHLIGHT_NAME, new api.Highlight(...passiveRanges));
    api.registry.set(
      ACTIVE_HIGHLIGHT_NAME,
      activeRange ? new api.Highlight(activeRange) : new api.Highlight(),
    );

    return () => {
      api.registry.delete(SEARCH_HIGHLIGHT_NAME);
      api.registry.delete(ACTIVE_HIGHLIGHT_NAME);
    };
  }, [activeIndex, enabled, matches]);

  useEffect(() => {
    const activeRange = matches[activeIndex]?.range;
    if (!activeRange || !enabled) {
      return;
    }

    scrollRangeIntoCanvas(activeRange, canvasRef.current);
  }, [activeIndex, canvasRef, enabled, matches]);

  const goToNext = useCallback(() => {
    setActiveIndex((previousIndex) =>
      matches.length === 0 ? 0 : (previousIndex + 1) % matches.length,
    );
  }, [matches.length]);

  const goToPrevious = useCallback(() => {
    setActiveIndex((previousIndex) =>
      matches.length === 0
        ? 0
        : (previousIndex - 1 + matches.length) % matches.length,
    );
  }, [matches.length]);

  const clearHighlights = useCallback(() => {
    const api = getHighlightApi();
    api?.registry.delete(SEARCH_HIGHLIGHT_NAME);
    api?.registry.delete(ACTIVE_HIGHLIGHT_NAME);
  }, []);

  return {
    activeIndex: matches.length === 0 ? 0 : activeIndex,
    totalMatches: matches.length,
    goToNext,
    goToPrevious,
    clearHighlights,
  };
}
