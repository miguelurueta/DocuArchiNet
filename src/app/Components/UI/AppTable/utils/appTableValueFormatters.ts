const DATE_TIME_PATTERN =
  /^(\d{4})-(\d{2})-(\d{2})(?:[T\s](\d{2}):(\d{2})(?::\d{2}(?:\.\d+)?)?(?:Z|[+-]\d{2}:?\d{2})?)?$/;

export const formatAppTableDateValue = (
  value: unknown,
  options: { includeTime?: boolean } = {},
): string => {
  if (value == null || value === "") {
    return "";
  }

  const rawValue =
    value instanceof Date
      ? [
          String(value.getFullYear()).padStart(4, "0"),
          String(value.getMonth() + 1).padStart(2, "0"),
          String(value.getDate()).padStart(2, "0"),
        ].join("-") +
        `T${String(value.getHours()).padStart(2, "0")}:${String(value.getMinutes()).padStart(
          2,
          "0",
        )}`
      : String(value).trim();
  const match = rawValue.match(DATE_TIME_PATTERN);

  if (!match) {
    return rawValue;
  }

  const [, year, month, day, hour, minute] = match;
  const formattedDate = `${day}/${month}/${year}`;

  if (!options.includeTime || !hour || !minute) {
    return formattedDate;
  }

  return `${formattedDate} ${hour}:${minute}`;
};
