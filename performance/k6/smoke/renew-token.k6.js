import http from "k6/http";
import { check, sleep } from "k6";
import { createSmokeOptions } from "../config/scenarios.js";
import { defaultThresholds } from "../config/thresholds.js";
import { buildApiUrl, buildDefaultHeaders } from "../utils/env.js";

const ENDPOINT = "/api/auth/renew";

export const options = {
  ...createSmokeOptions(),
  thresholds: defaultThresholds,
};

export default function () {
  const response = http.post(buildApiUrl(ENDPOINT), null, {
    headers: buildDefaultHeaders(),
    tags: {
      suite: "smoke",
      endpoint: "renew-token",
    },
  });

  check(response, {
    "renew status is 200": (res) => res.status === 200,
    "renew response time within budget": (res) => res.timings.duration < 800,
    "renew returns token": (res) => {
      const body = res.json();
      return Boolean(body?.token ?? body?.data?.token ?? body?.Data?.token);
    },
  });

  sleep(1);
}
