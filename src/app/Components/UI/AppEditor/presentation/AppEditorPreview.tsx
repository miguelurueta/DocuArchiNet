import {
  forwardRef,
  useCallback,
  useImperativeHandle,
  useLayoutEffect,
  useMemo,
  useRef,
  useState,
} from "react";
import type { CSSProperties } from "react";
import type { AppEditorPageMargins } from "../domain/editor.types";
import styles from "../AppEditor.module.css";

type PreviewPage = {
  pageNumber: number;
  blocks: string[];
};

type PreviewFragment = {
  html: string;
  measureHtml: string;
  groupKey?: string;
  listTag?: "ul" | "ol";
  listAttrs?: string;
  listStart?: number;
  rootTag?: string;
};

type AppEditorPreviewProps = {
  html: string;
  pageWidth: number;
  pageHeight: number;
  pageGap: number;
  pageMargins: AppEditorPageMargins;
  zoomLevel: number;
  minHeight?: string;
  onPageCountChange?: (pageCount: number) => void;
  onCurrentPageChange?: (pageNumber: number) => void;
};

export type AppEditorPreviewHandle = {
  goToPage: (pageNumber: number) => void;
};

function getElementAttributes(element: Element) {
  return Array.from(element.attributes)
    .map((attribute) => `${attribute.name}="${attribute.value.replace(/"/g, "&quot;")}"`)
    .join(" ");
}

function wrapListItems({
  listTag,
  listAttrs,
  listStart,
  items,
}: {
  listTag: "ul" | "ol";
  listAttrs?: string;
  listStart?: number;
  items: string[];
}) {
  const resolvedAttrs =
    listTag === "ol" && typeof listStart === "number"
      ? replaceListStartAttribute(listAttrs, listStart)
      : listAttrs;
  const attrs = resolvedAttrs ? ` ${resolvedAttrs}` : "";
  return `<${listTag}${attrs}>${items.join("")}</${listTag}>`;
}

function replaceListStartAttribute(listAttrs: string | undefined, listStart: number) {
  const safeStart = Math.max(1, Math.floor(listStart));

  if (!listAttrs?.trim()) {
    return `start="${safeStart}"`;
  }

  if (/\bstart="/i.test(listAttrs)) {
    return listAttrs.replace(/\bstart="[^"]*"/i, `start="${safeStart}"`);
  }

  return `${listAttrs} start="${safeStart}"`;
}

function resolveOrderedListStart(element: Element) {
  const start = Number(element.getAttribute("start") ?? "1");
  return Number.isFinite(start) ? Math.max(1, Math.floor(start)) : 1;
}

function splitTopLevelBlocks(html: string): PreviewFragment[] {
  if (typeof document === "undefined") {
    return html.trim() ? [{ html, measureHtml: html }] : [];
  }

  const container = document.createElement("div");
  container.innerHTML = html;

  return Array.from(container.children).flatMap((child, blockIndex): PreviewFragment[] => {
    const tagName = child.tagName.toLowerCase();

    if ((tagName === "ul" || tagName === "ol") && child.children.length > 0) {
      const listTag = tagName as "ul" | "ol";
      const listAttrs = getElementAttributes(child);
      const groupKey = `list-${blockIndex}`;
      const orderedStart = listTag === "ol" ? resolveOrderedListStart(child) : undefined;

      return Array.from(child.children)
        .filter((item) => item.tagName.toLowerCase() === "li")
        .map((item, itemIndex) => {
          const htmlItem = item.outerHTML;
          const listStart =
            listTag === "ol" && typeof orderedStart === "number"
              ? orderedStart + itemIndex
              : undefined;

          return {
            html: htmlItem,
            measureHtml: wrapListItems({
              listTag,
              listAttrs,
              listStart,
              items: [htmlItem],
            }),
            groupKey,
            listTag,
            listAttrs,
            listStart,
            rootTag: "li",
          };
        });
    }

    return [
      {
        html: child.outerHTML,
        measureHtml: child.outerHTML,
        rootTag: tagName,
      },
    ];
  }).filter((fragment) => fragment.html.trim().length > 0);
}

function resolveOversizedClass(fragment: PreviewFragment, isOversized: boolean) {
  if (!isOversized) {
    return null;
  }

  if (fragment.rootTag === "table") {
    return "app-editor-preview-oversized app-editor-preview-oversized-table";
  }

  if (fragment.rootTag === "img" || fragment.rootTag === "figure") {
    return "app-editor-preview-oversized app-editor-preview-oversized-image";
  }

  return null;
}

function wrapOversizedFragment(fragment: PreviewFragment, isOversized: boolean) {
  const oversizedClass = resolveOversizedClass(fragment, isOversized);

  if (!oversizedClass) {
    return fragment.html;
  }

  return `<div class="${oversizedClass}" data-preview-oversized="true">${fragment.html}</div>`;
}

function parseCssPixels(value: string) {
  const parsedValue = Number.parseFloat(value);
  return Number.isFinite(parsedValue) ? parsedValue : 0;
}

function resolveElementHeight(element: HTMLElement) {
  return element.offsetHeight || element.getBoundingClientRect().height || element.scrollHeight || 0;
}

function measurePreviewBlockHeight(blockContainer: HTMLElement) {
  const measuredElement =
    blockContainer.firstElementChild instanceof HTMLElement
      ? blockContainer.firstElementChild
      : blockContainer;
  const styles = window.getComputedStyle(measuredElement);
  const verticalMargins =
    parseCssPixels(styles.marginTop) + parseCssPixels(styles.marginBottom);

  return Math.max(1, resolveElementHeight(measuredElement) + verticalMargins);
}

function appendFragmentToPage(
  pageBlocks: string[],
  fragment: PreviewFragment,
  options: { isOversized?: boolean } = {},
) {
  const previousBlock = pageBlocks[pageBlocks.length - 1];
  const fragmentHtml = wrapOversizedFragment(fragment, Boolean(options.isOversized));

  if (
    fragment.listTag &&
    fragment.groupKey &&
    previousBlock?.startsWith(`<!--${fragment.groupKey}-->`)
  ) {
    const closeTag = `</${fragment.listTag}>`;
    pageBlocks[pageBlocks.length - 1] = previousBlock.replace(
      closeTag,
      `${fragmentHtml}${closeTag}`,
    );
    return;
  }

  if (fragment.listTag) {
    pageBlocks.push(
      `<!--${fragment.groupKey}-->${wrapListItems({
        listTag: fragment.listTag,
        listAttrs: fragment.listAttrs,
        listStart: fragment.listStart,
        items: [fragmentHtml],
      })}`,
    );
    return;
  }

  pageBlocks.push(fragmentHtml);
}

function createInitialPages(blocks: PreviewFragment[]): PreviewPage[] {
  const pageBlocks: string[] = [];
  blocks.forEach((block) => {
    appendFragmentToPage(pageBlocks, block);
  });

  return [
    {
      pageNumber: 1,
      blocks: pageBlocks,
    },
  ];
}

export const AppEditorPreview = forwardRef<AppEditorPreviewHandle, AppEditorPreviewProps>(function AppEditorPreview({
  html,
  pageWidth,
  pageHeight,
  pageGap,
  pageMargins,
  zoomLevel,
  minHeight,
  onPageCountChange,
  onCurrentPageChange,
}, ref) {
  const viewportRef = useRef<HTMLDivElement>(null);
  const measureRef = useRef<HTMLDivElement>(null);
  const pageRefs = useRef<Array<HTMLElement | null>>([]);
  const scrollFrameRef = useRef<number | null>(null);
  const blocks = useMemo(() => splitTopLevelBlocks(html), [html]);
  const [pages, setPages] = useState<PreviewPage[]>(() => createInitialPages(blocks));
  const pageContentWidth = Math.max(1, pageWidth - pageMargins.left - pageMargins.right);
  const pageContentHeight = Math.max(1, pageHeight - pageMargins.top - pageMargins.bottom);
  const sheetHeight = pages.length * pageHeight + Math.max(0, pages.length - 1) * pageGap;
  const zoomedSheetWidth = pageWidth * zoomLevel;
  const zoomedSheetHeight = sheetHeight * zoomLevel;
  const style = {
    "--app-editor-page-width": `${pageWidth}px`,
    "--app-editor-page-height": `${pageHeight}px`,
    "--app-editor-page-gap": `${pageGap}px`,
    "--app-editor-page-margin-top": `${pageMargins.top}px`,
    "--app-editor-page-margin-right": `${pageMargins.right}px`,
    "--app-editor-page-margin-bottom": `${pageMargins.bottom}px`,
    "--app-editor-page-margin-left": `${pageMargins.left}px`,
    "--app-editor-page-content-width": `${pageContentWidth}px`,
    "--app-editor-page-content-height": `${pageContentHeight}px`,
    "--app-editor-zoom": String(zoomLevel),
    "--app-editor-sheet-height": `${sheetHeight}px`,
    "--app-editor-zoomed-sheet-width": `${zoomedSheetWidth}px`,
    "--app-editor-zoomed-sheet-height": `${zoomedSheetHeight}px`,
    "--app-editor-min-height": minHeight,
  } as CSSProperties;
  const resolveVisiblePage = useCallback(() => {
    const viewport = viewportRef.current;

    if (!viewport) {
      return 1;
    }

    const viewportRect = viewport.getBoundingClientRect();
    let bestPage = 1;
    let bestVisibleHeight = -1;

    pageRefs.current.forEach((page, index) => {
      if (!page) {
        return;
      }

      const pageRect = page.getBoundingClientRect();
      const visibleTop = Math.max(pageRect.top, viewportRect.top);
      const visibleBottom = Math.min(pageRect.bottom, viewportRect.bottom);
      const visibleHeight = Math.max(0, visibleBottom - visibleTop);

      if (visibleHeight > bestVisibleHeight) {
        bestVisibleHeight = visibleHeight;
        bestPage = index + 1;
      }
    });

    return bestPage;
  }, []);
  const reportVisiblePage = useCallback(() => {
    onCurrentPageChange?.(resolveVisiblePage());
  }, [onCurrentPageChange, resolveVisiblePage]);

  useImperativeHandle(
    ref,
    () => ({
      goToPage: (pageNumber: number) => {
        const viewport = viewportRef.current;
        const safePageNumber = Math.max(1, Math.min(Math.floor(pageNumber), pages.length));
        const page = pageRefs.current[safePageNumber - 1];

        if (!viewport || !page) {
          return;
        }

        viewport.scrollTo({
          top: page.offsetTop,
          behavior: "smooth",
        });
      },
    }),
    [pages.length],
  );

  useLayoutEffect(() => {
    const measure = () => {
      const measureRoot = measureRef.current;

      if (!measureRoot) {
        return;
      }

      const measuredBlocks = Array.from(measureRoot.children).map((child, index) => ({
        fragment: blocks[index],
        height:
          child instanceof HTMLElement
            ? measurePreviewBlockHeight(child)
            : 1,
      }));

      const nextPages: PreviewPage[] = [];
      let currentPageBlocks: string[] = [];
      let currentPageHeight = 0;

      measuredBlocks.forEach((block) => {
        const isOversized = block.height > pageContentHeight;
        const shouldStartNextPage =
          currentPageBlocks.length > 0 && currentPageHeight + block.height > pageContentHeight;

        if (shouldStartNextPage) {
          nextPages.push({
            pageNumber: nextPages.length + 1,
            blocks: currentPageBlocks,
          });
          currentPageBlocks = [];
          currentPageHeight = 0;
        }

        if (block.fragment) {
          appendFragmentToPage(currentPageBlocks, block.fragment, { isOversized });
        }
        currentPageHeight += block.height;
      });

      nextPages.push({
        pageNumber: nextPages.length + 1,
        blocks: currentPageBlocks,
      });

      const resolvedPages = nextPages.length > 0 ? nextPages : createInitialPages([]);
      setPages(resolvedPages);
      onPageCountChange?.(resolvedPages.length);
      window.requestAnimationFrame(() => {
        onCurrentPageChange?.(resolveVisiblePage());
      });
    };

    measure();

    const measureRoot = measureRef.current;
    const images = measureRoot
      ? Array.from(measureRoot.querySelectorAll("img")).filter(
          (image): image is HTMLImageElement => image instanceof HTMLImageElement,
        )
      : [];

    images.forEach((image) => {
      image.addEventListener("load", measure, { once: true });
      image.addEventListener("error", measure, { once: true });
    });

    return () => {
      images.forEach((image) => {
        image.removeEventListener("load", measure);
        image.removeEventListener("error", measure);
      });
    };
  }, [blocks, onCurrentPageChange, onPageCountChange, pageContentHeight, resolveVisiblePage]);

  useLayoutEffect(() => {
    reportVisiblePage();
  }, [pages, reportVisiblePage]);

  useLayoutEffect(
    () => () => {
      if (scrollFrameRef.current !== null) {
        window.cancelAnimationFrame(scrollFrameRef.current);
        scrollFrameRef.current = null;
      }
    },
    [],
  );

  return (
    <div
      className={styles.previewViewport}
      ref={viewportRef}
      style={style}
      onScroll={() => {
        if (scrollFrameRef.current !== null) {
          window.cancelAnimationFrame(scrollFrameRef.current);
        }

        scrollFrameRef.current = window.requestAnimationFrame(() => {
          scrollFrameRef.current = null;
          reportVisiblePage();
        });
      }}
    >
      <div className={styles.previewMeasure} ref={measureRef} aria-hidden="true">
        {blocks.map((block, index) => (
          <div
            key={`${index}-${block.html.length}`}
            className={styles.previewMeasureBlock}
            dangerouslySetInnerHTML={{ __html: block.measureHtml }}
          />
        ))}
      </div>
      <div className={styles.previewZoomStage}>
        <div className={styles.previewSheet}>
          {pages.map((page) => (
            <article
              key={page.pageNumber}
              className={styles.previewPage}
              ref={(node) => {
                pageRefs.current[page.pageNumber - 1] = node;
              }}
            >
              <div
                className={styles.previewPageContent}
                dangerouslySetInnerHTML={{ __html: page.blocks.join("") }}
              />
            </article>
          ))}
        </div>
      </div>
    </div>
  );
});
