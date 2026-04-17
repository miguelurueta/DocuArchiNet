import http from "k6/http";
import { sleep } from "k6";
import { createLoadOptions } from "../config/scenarios.js";
import { moderateLoadThresholds } from "../config/thresholds.js";
import { checkJsonResponse } from "../utils/checks.js";
import { buildApiUrl, buildDefaultHeaders } from "../utils/env.js";

const DEFAULT_ENDPOINT = "/api/workflowInboxgestion/inboxgestion";
const SORT_FIELDS = ["id", "fecha", "estado", "asunto"];

function buildAppTablePayload() {
  const sortField = SORT_FIELDS[Math.floor(Math.random() * SORT_FIELDS.length)];

  return {
    tableId: __ENV.APP_TABLE_ID ?? "inbox-gestion",
    page: 1,
    pageSize: 10,
    sorting: {
      field: sortField,
      direction: Math.random() > 0.5 ? "asc" : "desc",
    },
    filters: {},
  };
}

export const options = {
  ...createLoadOptions(),
  thresholds: moderateLoadThresholds,
};

export default function () {
  const endpoint = __ENV.APP_TABLE_ENDPOINT ?? DEFAULT_ENDPOINT;
  const response = http.post(buildApiUrl(endpoint), JSON.stringify(buildAppTablePayload()), {
    headers: buildDefaultHeaders(),
    tags: {
      suite: "load",
      endpoint: "app-table",
    },
  });

  checkJsonResponse(response, 1200);
  sleep(1);
}
