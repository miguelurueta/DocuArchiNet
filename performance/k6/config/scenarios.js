export function createSmokeOptions(overrides = {}) {
  return {
    vus: 1,
    iterations: 1,
    ...overrides,
  };
}

export function createLoadOptions(overrides = {}) {
  return {
    vus: 10,
    duration: "30s",
    ...overrides,
  };
}
