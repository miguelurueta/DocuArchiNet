import http from "k6/http";
import { sleep } from "k6";
import { createLoadOptions } from "../config/scenarios.js";
import { moderateLoadThresholds } from "../config/thresholds.js";
import { checkJsonResponse } from "../utils/checks.js";
import { buildApiUrl, buildDefaultHeaders } from "../utils/env.js";

const ENDPOINT = "/api/workflowInboxgestion/inboxgestion/autocomplete";
const TERMS = ["rad", "corr", "gestion", "doc", "tramite"];

export const options = {
  ...createLoadOptions(),
  thresholds: moderateLoadThresholds,
};

export default function () {
  const term = TERMS[Math.floor(Math.random() * TERMS.length)];
  const payload = JSON.stringify({
    query: term,
    limit: 10,
  });

  const response = http.post(buildApiUrl(ENDPOINT), payload, {
    headers: buildDefaultHeaders(),
    tags: {
      suite: "load",
      endpoint: "workflow-inbox-autocomplete",
    },
  });

  checkJsonResponse(response, 1200);
  sleep(1);
}
