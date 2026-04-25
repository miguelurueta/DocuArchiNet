const PAGE_WRAPPER_PATTERN = /<\/?div\b(?=[^>]*\bdata-app-editor-page="true")[^>]*>/gi;
const MANUAL_PAGE_BREAK_PATTERN =
  /<div\b(?=[^>]*\bdata-page-break="true")(?![^>]*\bdata-page-break-auto="true")[^>]*><\/div>/gi;
const MANUAL_PAGE_BREAK_MARKUP = '<div data-page-break="true"></div>';

const EMPTY_SEGMENT_HTML = "<p></p>";

export function hasVisualPageWrappers(value: string | null | undefined) {
  return /data-app-editor-page="true"/i.test(String(value ?? ""));
}

export function hasManualPageBreaks(value: string | null | undefined) {
  MANUAL_PAGE_BREAK_PATTERN.lastIndex = 0;
  return MANUAL_PAGE_BREAK_PATTERN.test(String(value ?? ""));
}

export function unwrapVisualPageHtml(value: string | null | undefined) {
  const safeValue = String(value ?? "").trim();

  if (!safeValue) {
    return "";
  }

  if (typeof document === "undefined") {
    return safeValue.replace(PAGE_WRAPPER_PATTERN, "").trim();
  }

  const container = document.createElement("div");
  container.innerHTML = safeValue;
  const pages = Array.from(container.querySelectorAll('[data-app-editor-page="true"]'));

  if (pages.length === 0) {
    return safeValue;
  }

  return pages.map((page) => page.innerHTML.trim()).join("").trim();
}

export function serializeVisualPageHtml(value: string | null | undefined) {
  const safeValue = String(value ?? "").trim();

  if (!safeValue || !hasVisualPageWrappers(safeValue)) {
    return safeValue;
  }

  if (typeof document === "undefined") {
    return safeValue
      .replace(PAGE_WRAPPER_PATTERN, "")
      .trim();
  }

  const container = document.createElement("div");
  container.innerHTML = safeValue;
  const pages = Array.from(container.querySelectorAll('[data-app-editor-page="true"]'));

  if (pages.length === 0) {
    return safeValue;
  }

  return pages
    .map((page) => normalizePageSegment(page.innerHTML))
    .join(MANUAL_PAGE_BREAK_MARKUP)
    .trim();
}

function normalizePageSegment(value: string) {
  const trimmedValue = value.trim();
  return trimmedValue.length > 0 ? trimmedValue : EMPTY_SEGMENT_HTML;
}

export function splitHtmlByManualPageBreaks(value: string | null | undefined) {
  const safeValue = String(value ?? "");
  const segments = safeValue.split(MANUAL_PAGE_BREAK_PATTERN);

  if (segments.length === 0) {
    return [EMPTY_SEGMENT_HTML];
  }

  return segments.map(normalizePageSegment);
}

export function wrapHtmlInVisualPages(value: string | null | undefined) {
  const safeValue = String(value ?? "").trim();

  if (!safeValue) {
    return '<div data-app-editor-page="true"><p></p></div>';
  }

  if (hasVisualPageWrappers(safeValue)) {
    return safeValue;
  }

  return splitHtmlByManualPageBreaks(safeValue)
    .map((segment) => `<div data-app-editor-page="true">${segment}</div>`)
    .join("");
}
