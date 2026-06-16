import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import type { RefObject } from "react";
import type { Editor } from "@tiptap/react";
import type { Node as ProseMirrorNode } from "@tiptap/pm/model";
import type { VisualPage } from "../application/autoPagination";
import styles from "../AppEditor.module.css";

type AppEditorNavigationPanelProps = {
  editor: Editor | null;
  pages: VisualPage[];
  totalPages: number;
  currentPage: number;
  canvasRef: RefObject<HTMLElement | null>;
  zoomLevel: number;
  onGoToPage: (pageNumber: number) => void;
  showPages?: boolean;
  showOutline?: boolean;
};

type HeadingEntry = {
  id: string;
  level: 1 | 2 | 3;
  text: string;
  pos: number;
  blockIndex: number;
};

type HeadingTreeEntry = HeadingEntry & {
  children: HeadingTreeEntry[];
};

type ThumbnailMark = {
  key: string;
  kind: "heading" | "paragraph" | "list" | "image" | "table";
  width: "short" | "medium" | "long";
};

function buildPageItems({
  editor,
  pages,
  totalPages,
}: {
  editor: Editor | null;
  pages: VisualPage[];
  totalPages: number;
}) {
  const safeTotalPages = Math.max(1, Math.floor(totalPages));

  if (pages.length > 0) {
    return pages;
  }

  const blockCount = Math.max(0, editor?.state?.doc?.childCount ?? 0);
  const blocksPerPage = Math.max(1, Math.ceil(blockCount / safeTotalPages));

  return Array.from({ length: safeTotalPages }, (_, index) => ({
    pageNumber: index + 1,
    top: index,
    bottom: index + 1,
    contentTop: index,
    contentBottom: index + 1,
    startBlockIndex: Math.min(Math.max(0, index * blocksPerPage), Math.max(0, blockCount - 1)),
    endBlockIndex: Math.min(
      Math.max(0, index * blocksPerPage + blocksPerPage - 1),
      Math.max(0, blockCount - 1),
    ),
  }));
}

function resolveNodeMarkKind(node: ProseMirrorNode): ThumbnailMark["kind"] {
  switch (node.type.name) {
    case "heading":
      return "heading";
    case "bulletList":
    case "orderedList":
    case "taskList":
      return "list";
    case "image":
      return "image";
    case "table":
      return "table";
    default:
      return "paragraph";
  }
}

function resolveMarkWidth(node: ProseMirrorNode, index: number): ThumbnailMark["width"] {
  const textLength = node.textContent.trim().length;

  if (node.type.name === "image" || node.type.name === "table") {
    return "long";
  }

  if (textLength < 18) {
    return index % 2 === 0 ? "medium" : "short";
  }

  if (textLength < 48) {
    return "medium";
  }

  return "long";
}

function buildNodeMarks(node: ProseMirrorNode, blockIndex: number): ThumbnailMark[] {
  const kind = resolveNodeMarkKind(node);

  if (kind === "image" || kind === "table" || kind === "heading") {
    return [
      {
        key: `${blockIndex}-${kind}`,
        kind,
        width: resolveMarkWidth(node, blockIndex),
      },
    ];
  }

  if (kind === "list") {
    const itemCount = Math.max(1, Math.min(4, node.childCount || 1));
    return Array.from({ length: itemCount }, (_, index) => ({
      key: `${blockIndex}-list-${index}`,
      kind,
      width: index % 2 === 0 ? "long" : "medium",
    }));
  }

  const lineCount = Math.max(1, Math.min(4, Math.ceil(node.textContent.trim().length / 52) || 1));
  return Array.from({ length: lineCount }, (_, index) => ({
    key: `${blockIndex}-paragraph-${index}`,
    kind,
    width: index % 3 === 2 ? "short" : index % 2 === 1 ? "medium" : "long",
  }));
}

function buildThumbnailMarksByPage(editor: Editor | null, pages: VisualPage[]) {
  const marksByPage = new Map<number, ThumbnailMark[]>();

  if (!editor?.state?.doc) {
    return marksByPage;
  }

  const topLevelMarks: ThumbnailMark[][] = [];
  editor.state.doc.forEach((node, _offset, index) => {
    topLevelMarks[index] = buildNodeMarks(node, index);
  });

  pages.forEach((page) => {
    if (topLevelMarks.length === 0) {
      marksByPage.set(page.pageNumber, []);
      return;
    }

    const startIndex = Math.max(0, Math.min(page.startBlockIndex, topLevelMarks.length - 1));
    const endIndex = Math.max(startIndex, Math.min(page.endBlockIndex, topLevelMarks.length - 1));
    const pageMarks: ThumbnailMark[] = [];

    for (let index = startIndex; index <= endIndex; index += 1) {
      pageMarks.push(...(topLevelMarks[index] ?? []));

      if (pageMarks.length >= 12) {
        break;
      }
    }

    marksByPage.set(page.pageNumber, pageMarks.slice(0, 12));
  });

  return marksByPage;
}

function collectHeadings(editor: Editor | null): HeadingEntry[] {
  if (!editor?.state?.doc) {
    return [];
  }

  const headings: HeadingEntry[] = [];
  const topLevelBlocks: Array<{ index: number; from: number; to: number }> = [];

  editor.state.doc.forEach((node, offset, index) => {
    topLevelBlocks.push({
      index,
      from: offset,
      to: offset + node.nodeSize,
    });
  });

  editor.state.doc.descendants((node, pos) => {
    if (node.type.name !== "heading") {
      return true;
    }

    const level = node.attrs.level;
    if (level !== 1 && level !== 2 && level !== 3) {
      return true;
    }

    const text = node.textContent.trim();
    if (!text) {
      return true;
    }

    headings.push({
      id: `${pos}-${level}-${text}`,
      level,
      text,
      pos,
      blockIndex:
        topLevelBlocks.find((block) => pos >= block.from && pos < block.to)?.index ?? 0,
    });

    return false;
  });

  return headings;
}

function buildHeadingTree(headings: HeadingEntry[]): HeadingTreeEntry[] {
  const roots: HeadingTreeEntry[] = [];
  const stack: Partial<Record<1 | 2 | 3, HeadingTreeEntry>> = {};

  headings.forEach((heading) => {
    const entry: HeadingTreeEntry = {
      ...heading,
      children: [],
    };
    const parentLevel = heading.level === 1 ? null : heading.level === 2 ? 1 : 2;
    const parent = parentLevel ? stack[parentLevel] : null;

    if (parent) {
      parent.children.push(entry);
    } else {
      roots.push(entry);
    }

    stack[heading.level] = entry;
    if (heading.level === 1) {
      delete stack[2];
      delete stack[3];
    } else if (heading.level === 2) {
      delete stack[3];
    }
  });

  return roots;
}

function flattenVisibleHeadings(
  headings: HeadingTreeEntry[],
  expandedHeadingIds: Set<string> | null,
): HeadingTreeEntry[] {
  const visibleHeadings: HeadingTreeEntry[] = [];

  const appendEntry = (entry: HeadingTreeEntry) => {
    visibleHeadings.push(entry);

    if (entry.children.length > 0 && expandedHeadingIds?.has(entry.id)) {
      entry.children.forEach(appendEntry);
    }
  };

  headings.forEach(appendEntry);

  return visibleHeadings;
}

function buildPageTitleByPage(pages: VisualPage[], headings: HeadingEntry[]) {
  const pageTitleByPage = new Map<number, string>();

  pages.forEach((page) => {
    const pageHeadings = headings.filter(
      (heading) =>
        heading.blockIndex >= page.startBlockIndex && heading.blockIndex <= page.endBlockIndex,
    );
    const previousHeadings = headings.filter(
      (heading) => heading.blockIndex <= page.endBlockIndex,
    );
    const previousHeading = previousHeadings[previousHeadings.length - 1];
    const title = pageHeadings[0]?.text ?? previousHeading?.text;

    if (title) {
      pageTitleByPage.set(page.pageNumber, title);
    }
  });

  return pageTitleByPage;
}

function resolveHeadingTop({
  canvas,
  editor,
  pos,
  zoomLevel,
}: {
  canvas: HTMLElement;
  editor: Editor;
  pos: number;
  zoomLevel: number;
}) {
  const sheet = canvas.querySelector<HTMLElement>('[data-pagination-sheet="true"]');
  const originRect = (sheet ?? canvas).getBoundingClientRect();
  const safePos = Math.max(0, Math.min(pos + 1, editor.state.doc.content.size));

  try {
    const coords = editor.view.coordsAtPos(safePos);
    return (coords.top - originRect.top) / Math.max(0.1, zoomLevel);
  } catch {
    return null;
  }
}

export function AppEditorNavigationPanel({
  editor,
  pages,
  totalPages,
  currentPage,
  canvasRef,
  zoomLevel,
  onGoToPage,
  showPages = true,
  showOutline = true,
}: AppEditorNavigationPanelProps) {
  const [contentVersion, setContentVersion] = useState(0);
  const [activeHeadingId, setActiveHeadingId] = useState<string | null>(null);
  const [expandedHeadingIds, setExpandedHeadingIds] = useState<Set<string> | null>(null);
  const headingTopByIdRef = useRef(new Map<string, number>());
  const pageItems = useMemo(
    () => buildPageItems({ editor, pages, totalPages }),
    [editor, pages, totalPages],
  );
  const headings = useMemo(() => {
    void contentVersion;
    return collectHeadings(editor);
  }, [contentVersion, editor]);
  const headingTree = useMemo(() => buildHeadingTree(headings), [headings]);
  const visibleHeadings = useMemo(
    () => flattenVisibleHeadings(headingTree, expandedHeadingIds),
    [expandedHeadingIds, headingTree],
  );
  const thumbnailMarksByPage = useMemo(() => {
    void contentVersion;
    return buildThumbnailMarksByPage(editor, pageItems);
  }, [contentVersion, editor, pageItems]);
  const pageTitleByPage = useMemo(
    () => buildPageTitleByPage(pageItems, headings),
    [headings, pageItems],
  );
  const expandableHeadingIds = useMemo(
    () =>
      headings
        .filter((heading) =>
          headingTree.some((root) => {
            const hasDescendant = (entry: HeadingTreeEntry): boolean =>
              entry.id === heading.id
                ? entry.children.length > 0
                : entry.children.some(hasDescendant);

            return hasDescendant(root);
          }),
        )
        .map((heading) => heading.id),
    [headingTree, headings],
  );

  useEffect(() => {
    setExpandedHeadingIds((previousIds) => {
      const nextIds = new Set(previousIds ?? expandableHeadingIds);
      expandableHeadingIds.forEach((id) => {
        if (!previousIds) {
          nextIds.add(id);
        }
      });

      return nextIds;
    });
  }, [expandableHeadingIds]);

  const measureHeadingTops = useCallback(() => {
    const canvas = canvasRef.current;
    const nextHeadingTopById = new Map<string, number>();

    if (!editor || !canvas || headings.length === 0) {
      headingTopByIdRef.current = nextHeadingTopById;
      return;
    }

    headings.forEach((heading) => {
      const headingTop = resolveHeadingTop({
        canvas,
        editor,
        pos: heading.pos,
        zoomLevel,
      });

      if (headingTop !== null) {
        nextHeadingTopById.set(heading.id, headingTop);
      }
    });

    headingTopByIdRef.current = nextHeadingTopById;
  }, [canvasRef, editor, headings, zoomLevel]);

  const updateActiveHeading = useCallback(() => {
    const canvas = canvasRef.current;

    if (!editor || !canvas || headings.length === 0) {
      setActiveHeadingId(null);
      return;
    }

    const scrollY = canvas.scrollTop / Math.max(0.1, zoomLevel);
    const activationOffset = 96 / Math.max(0.1, zoomLevel);
    let nextActiveHeading = headings[0];

    headings.forEach((heading) => {
      const headingTop = headingTopByIdRef.current.get(heading.id) ?? null;

      if (headingTop !== null && headingTop <= scrollY + activationOffset) {
        nextActiveHeading = heading;
      }
    });

    setActiveHeadingId(nextActiveHeading.id);
  }, [canvasRef, editor, headings, zoomLevel]);

  useEffect(() => {
    if (!editor) {
      return undefined;
    }

    const handleTransaction = () => {
      setContentVersion((previousVersion) => previousVersion + 1);
    };

    editor.on("transaction", handleTransaction);

    return () => {
      editor.off("transaction", handleTransaction);
    };
  }, [editor]);

  useEffect(() => {
    const canvas = canvasRef.current;
    if (!canvas) {
      return undefined;
    }

    let frameId = 0;
    const scheduleUpdate = () => {
      window.cancelAnimationFrame(frameId);
      frameId = window.requestAnimationFrame(() => {
        measureHeadingTops();
        updateActiveHeading();
      });
    };

    scheduleUpdate();
    canvas.addEventListener("scroll", scheduleUpdate, { passive: true });
    window.addEventListener("resize", scheduleUpdate);

    return () => {
      window.cancelAnimationFrame(frameId);
      canvas.removeEventListener("scroll", scheduleUpdate);
      window.removeEventListener("resize", scheduleUpdate);
    };
  }, [canvasRef, measureHeadingTops, updateActiveHeading]);

  const toggleHeading = useCallback((headingId: string) => {
    setExpandedHeadingIds((previousIds) => {
      const nextIds = new Set(previousIds ?? []);

      if (nextIds.has(headingId)) {
        nextIds.delete(headingId);
      } else {
        nextIds.add(headingId);
      }

      return nextIds;
    });
  }, []);

  const handleHeadingClick = useCallback(
    (heading: HeadingEntry) => {
      const canvas = canvasRef.current;

      if (!editor || !canvas) {
        return;
      }

      const safePos = Math.max(0, Math.min(heading.pos + 1, editor.state.doc.content.size));

      try {
        const canvasRect = canvas.getBoundingClientRect();
        const coords = editor.view.coordsAtPos(safePos);
        const maxTop = Math.max(0, canvas.scrollHeight - canvas.clientHeight);
        const nextTop = Math.max(
          0,
          Math.min(maxTop, canvas.scrollTop + coords.top - canvasRect.top - 72),
        );

        canvas.scrollTo({ top: nextTop, behavior: "smooth" });
        editor.commands.focus(safePos, { scrollIntoView: false });
        setActiveHeadingId(heading.id);
      } catch {
        editor.commands.focus(safePos, { scrollIntoView: true });
      }
    },
    [canvasRef, editor],
  );

  return (
    <div className={styles.navigationPanel} aria-label="Navegacion del documento">
      {showPages ? (
        <section className={styles.navigationSection} aria-labelledby="app-editor-pages-heading">
          <h3 id="app-editor-pages-heading" className={styles.navigationSectionTitle}>
            Miniaturas
          </h3>
          <div className={styles.pageThumbnailList}>
            {pageItems.map((page) => {
              const isActive = page.pageNumber === currentPage;
              const thumbnailMarks = thumbnailMarksByPage.get(page.pageNumber) ?? [];
              const pageTitle = pageTitleByPage.get(page.pageNumber);

              return (
                <button
                  key={page.pageNumber}
                  type="button"
                  className={isActive ? styles.pageThumbnailActive : styles.pageThumbnail}
                  aria-current={isActive ? "page" : undefined}
                  aria-label={`Ir a pagina ${page.pageNumber}`}
                  onClick={() => onGoToPage(page.pageNumber)}
                >
                  <span className={styles.pageThumbnailSheet} aria-hidden="true">
                    {thumbnailMarks.length > 0 ? (
                      thumbnailMarks.map((mark) => (
                        <span
                          key={mark.key}
                          className={styles.pageThumbnailMark}
                          data-kind={mark.kind}
                          data-width={mark.width}
                        />
                      ))
                    ) : (
                      <>
                        <span
                          className={styles.pageThumbnailMark}
                          data-kind="paragraph"
                          data-width="long"
                        />
                        <span
                          className={styles.pageThumbnailMark}
                          data-kind="paragraph"
                          data-width="medium"
                        />
                        <span
                          className={styles.pageThumbnailMark}
                          data-kind="paragraph"
                          data-width="short"
                        />
                      </>
                    )}
                  </span>
                  <span className={styles.pageThumbnailLabel}>Pag. {page.pageNumber}</span>
                  {pageTitle ? (
                    <span className={styles.pageThumbnailTitle}>{pageTitle}</span>
                  ) : null}
                </button>
              );
            })}
          </div>
        </section>
      ) : null}

      {showOutline ? (
        <section className={styles.navigationSection} aria-labelledby="app-editor-outline-heading">
          <h3 id="app-editor-outline-heading" className={styles.navigationSectionTitle}>
            Estructura
          </h3>
          {headings.length > 0 ? (
            <nav className={styles.outlineList} aria-label="Titulos del documento">
              {visibleHeadings.map((heading) => {
                const isActive = heading.id === activeHeadingId;
                const hasChildren = heading.children.length > 0;
                const isExpanded = expandedHeadingIds?.has(heading.id) ?? false;

                return (
                  <div
                    key={heading.id}
                    className={styles.outlineTreeRow}
                    data-level={heading.level}
                  >
                    <button
                      type="button"
                      className={styles.outlineToggle}
                      aria-label={
                        hasChildren
                          ? isExpanded
                            ? `Contraer ${heading.text}`
                            : `Expandir ${heading.text}`
                          : undefined
                      }
                      aria-hidden={hasChildren ? undefined : true}
                      tabIndex={hasChildren ? 0 : -1}
                      onClick={() => toggleHeading(heading.id)}
                    >
                      {hasChildren ? (isExpanded ? "▼" : "►") : ""}
                    </button>
                    <button
                      type="button"
                      className={isActive ? styles.outlineItemActive : styles.outlineItem}
                      data-level={heading.level}
                      aria-current={isActive ? "location" : undefined}
                      onClick={() => handleHeadingClick(heading)}
                    >
                      {heading.text}
                    </button>
                  </div>
                );
              })}
            </nav>
          ) : (
            <p className={styles.navigationEmpty}>Sin titulos</p>
          )}
        </section>
      ) : null}
    </div>
  );
}
