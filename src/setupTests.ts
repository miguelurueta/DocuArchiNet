import "@testing-library/jest-dom";
const originalError = console.error;
console.error = (...args: unknown[]) => {
  if (typeof args[0] === "string" && args[0].includes("Could not parse CSS stylesheet")) {
    return;
  }
  originalError(...args);
};

const originalStderrWrite = process.stderr.write.bind(process.stderr);
process.stderr.write = ((chunk: unknown, ...args: unknown[]) => {
  if (typeof chunk === "string" && chunk.includes("Could not parse CSS stylesheet")) {
    return true;
  }
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  return (originalStderrWrite as any)(chunk, ...args);
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
}) as any;

// Polyfills for UI libs in jsdom
if (!window.matchMedia) {
  window.matchMedia = (query: string) =>
    ({
      matches: false,
      media: query,
      onchange: null,
      addListener: () => {},
      removeListener: () => {},
      addEventListener: () => {},
      removeEventListener: () => {},
      dispatchEvent: () => false,
    }) as MediaQueryList;
}

if (!("ResizeObserver" in window)) {
  class ResizeObserverMock {
    observe() {}
    unobserve() {}
    disconnect() {}
  }
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  (window as any).ResizeObserver = ResizeObserverMock;
}
