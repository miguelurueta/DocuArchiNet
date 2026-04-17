export function getRequiredEnv(name) {
  const value = __ENV[name];

  if (!value || String(value).trim().length === 0) {
    throw new Error(`Missing required k6 env var: ${name}`);
  }

  return value.trim();
}

export function buildDefaultHeaders() {
  const headers = {
    "Content-Type": "application/json",
  };

  const token = __ENV.API_TOKEN;

  if (token && token.trim().length > 0) {
    headers.Authorization = `Bearer ${token.trim()}`;
  }

  return headers;
}

export function buildApiUrl(path) {
  const apiUrl = getRequiredEnv("API_URL").replace(/\/+$/, "");
  const normalizedPath = path.startsWith("/") ? path : `/${path}`;
  return `${apiUrl}${normalizedPath}`;
}
