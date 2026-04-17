import http from "k6/http";
import { check } from "k6";
import { buildApiUrl, buildDefaultHeaders, getRequiredEnv } from "./env.js";

const LOGIN_ENDPOINT = "/api/accout/ValidaUserAplicacion";

export function buildLoginPayload() {
  return {
    IdEmpresa: Number(getRequiredEnv("LOGIN_EMPRESA_ID")),
    IdModulo: Number(getRequiredEnv("LOGIN_MODULO_ID")),
    User: getRequiredEnv("LOGIN_USER"),
    Password: getRequiredEnv("LOGIN_PASSWORD"),
  };
}

export function loginAndGetToken() {
  const response = http.post(buildApiUrl(LOGIN_ENDPOINT), JSON.stringify(buildLoginPayload()), {
    headers: buildDefaultHeaders(),
    tags: {
      suite: "auth",
      endpoint: "login",
    },
  });

  check(response, {
    "login status is 200": (res) => res.status === 200,
  });

  const body = response.json();
  const token = body?.data?.token ?? body?.Data?.token ?? body?.token;

  if (!token || String(token).trim().length === 0) {
    throw new Error("Login response did not include a token");
  }

  return String(token).trim();
}
