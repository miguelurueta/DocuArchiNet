const PAGE_WRAPPER_PATTERN = /<\/?div\b(?=[^>]*\bdata-app-editor-page="true")[^>]*>/gi;

export function hasVisualPageWrappers(value: string | null | undefined) {
  return /data-app-editor-page="true"/i.test(String(value ?? ""));
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
    return safeValue.replace(PAGE_WRAPPER_PATTERN, "").trim();
  }

  return unwrapVisualPageHtml(safeValue).trim();
}
