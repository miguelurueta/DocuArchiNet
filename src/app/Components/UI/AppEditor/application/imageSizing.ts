export function normalizeImageWidth(value: string) {
  const normalizedValue = value.trim();

  if (!normalizedValue) {
    return undefined;
  }

  if (/^\d+(\.\d+)?%$/.test(normalizedValue)) {
    return normalizedValue;
  }

  if (/^\d+(\.\d+)?px$/i.test(normalizedValue)) {
    return normalizedValue.toLowerCase();
  }

  if (/^\d+(\.\d+)?$/.test(normalizedValue)) {
    return `${normalizedValue}px`;
  }

  return normalizedValue;
}
