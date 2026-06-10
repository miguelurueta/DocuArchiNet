export const DEFAULT_PAGE_MARGINS = {
  top: 96,
  right: 72,
  bottom: 96,
  left: 72,
} as const;

export const PAGE_DIMENSIONS = {
  A4: {
    portrait: {
      // 8.5x11 pulgadas a 96 DPI = 816x1056 px.
      width: 816,
      height: 1056,
    },
    landscape: {
      width: 1056,
      height: 816,
    },
  },
} as const;

export const PDF_PAGE_DIMENSIONS = {
  A4: {
    portrait: {
      width: 794,
      height: 1123,
    },
    landscape: {
      width: 1123,
      height: 794,
    },
  },
} as const;
