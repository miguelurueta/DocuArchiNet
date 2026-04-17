import { check } from "k6";

export function checkJsonResponse(response, maxDurationMs) {
  return check(response, {
    "status is 200": (res) => res.status === 200,
    "response time within budget": (res) => res.timings.duration < maxDurationMs,
    "content-type is json-like": (res) =>
      String(res.headers["Content-Type"] ?? "").toLowerCase().includes("application/json"),
  });
}
