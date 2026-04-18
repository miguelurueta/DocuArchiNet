import http from "k6/http";
import { sleep } from "k6";
import { createSmokeOptions } from "../config/scenarios.js";
import { defaultThresholds } from "../config/thresholds.js";
import { checkJsonResponse } from "../utils/checks.js";
import { buildApiUrl, buildDefaultHeaders } from "../utils/env.js";

const DEFAULT_ENDPOINT = "/api/workflowInboxgestion/inboxgestion";

function buildAppTablePayload() {
  return {
    tableId: __ENV.APP_TABLE_ID ?? "inbox-gestion",
    page: 1,
    pageSize: 10,
    sorting: {
      field: "id",
      direction: "desc",
    },
    filters: {},
  };
}

export const options = {
  ...createSmokeOptions(),
  thresholds: defaultThresholds,
};

export default function () {
  const endpoint = __ENV.APP_TABLE_ENDPOINT ?? DEFAULT_ENDPOINT;
  const response = http.post(buildApiUrl(endpoint), JSON.stringify(buildAppTablePayload()), {
    headers: buildDefaultHeaders(),
    tags: {
      suite: "smoke",
      endpoint: "app-table",
    },
  });

  checkJsonResponse(response, 1000);
  sleep(1);
}
