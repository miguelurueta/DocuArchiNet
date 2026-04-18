import http from "k6/http";
import { sleep } from "k6";
import { createSmokeOptions } from "../config/scenarios.js";
import { defaultThresholds } from "../config/thresholds.js";
import { checkJsonResponse } from "../utils/checks.js";
import { buildApiUrl, buildDefaultHeaders } from "../utils/env.js";
import { buildLoginPayload } from "../utils/auth.js";

const ENDPOINT = "/api/accout/ValidaUserAplicacion";

export const options = {
  ...createSmokeOptions(),
  thresholds: defaultThresholds,
};

export default function () {
  const response = http.post(buildApiUrl(ENDPOINT), JSON.stringify(buildLoginPayload()), {
    headers: buildDefaultHeaders(),
    tags: {
      suite: "smoke",
      endpoint: "login",
    },
  });

  checkJsonResponse(response, 1000);
  sleep(1);
}
