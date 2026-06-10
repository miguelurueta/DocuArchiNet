import { serializeVisualPageHtml } from "./pageDocument";

const EMPTY_EDITOR_HTML_VALUES = new Set([
  "",
  "<p></p>",
  "<p><br></p>",
  "<p><br/></p>",
  "<p><br /></p>",
]);

const EMPTY_PARAGRAPH_PATTERN =
  /^<p(?:\s+[^>]*)?>\s*(?:<br(?:\s+[^>]*)?\s*\/?>)?\s*<\/p>$/i;

export function stripAutoLayoutMetadata(value: string | null | undefined) {
  return serializeVisualPageHtml(String(value ?? ""))
    .replace(/\u200B/g, "")
    .trim();
}

export function normalizeEditorHtml(value: string | null | undefined) {
  const normalizedValue = stripAutoLayoutMetadata(value);

  if (!normalizedValue) {
    return "";
  }

  const comparableValue = normalizedValue.toLowerCase();

  if (
    EMPTY_EDITOR_HTML_VALUES.has(comparableValue) ||
    EMPTY_PARAGRAPH_PATTERN.test(normalizedValue)
  ) {
    return "";
  }

  return normalizedValue;
}
