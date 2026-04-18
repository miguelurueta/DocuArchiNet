import http from "k6/http";
import { sleep } from "k6";
import { createSmokeOptions } from "../config/scenarios.js";
import { defaultThresholds } from "../config/thresholds.js";
import { checkJsonResponse } from "../utils/checks.js";
import { buildApiUrl, buildDefaultHeaders } from "../utils/env.js";

const ENDPOINT = "/api/workflowInboxgestion/inboxgestion/autocomplete";

export const options = {
  ...createSmokeOptions(),
  thresholds: defaultThresholds,
};

export default function () {
  const payload = JSON.stringify({
    query: "rad",
    limit: 10,
  });

  const response = http.post(buildApiUrl(ENDPOINT), payload, {
    headers: buildDefaultHeaders(),
    tags: {
      suite: "smoke",
      endpoint: "workflow-inbox-autocomplete",
    },
  });

  checkJsonResponse(response, 800);
  sleep(1);
}
