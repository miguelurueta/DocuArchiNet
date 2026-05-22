export const buildJiraAuthHeader = (email, apiToken) => {
  if (!email || !apiToken) {
    throw new Error("Faltan credenciales JIRA: JIRA_EMAIL y/o JIRA_API_TOKEN.");
  }

  const encoded = Buffer.from(`${email}:${apiToken}`).toString("base64");
  return `Basic ${encoded}`;
};

const extractTextFromAdfNode = (node) => {
  if (!node) return "";
  if (typeof node === "string") return node;
  if (Array.isArray(node)) {
    return node.map(extractTextFromAdfNode).filter(Boolean).join("");
  }
  if (node.type === "text" && typeof node.text === "string") {
    return node.text;
  }
  if (Array.isArray(node.content)) {
    const inner = node.content.map(extractTextFromAdfNode).filter(Boolean).join("");
    if (node.type === "paragraph" || node.type === "heading" || node.type === "listItem") {
      return `${inner}\n`;
    }
    return inner;
  }
  return "";
};

export const normalizeDescription = (description) => {
  if (!description) return "";
  if (typeof description === "string") return description.trim();
  const text = extractTextFromAdfNode(description).trim();
  return text.replace(/\n{3,}/g, "\n\n");
};

const normalizeComments = (commentsField) => {
  const comments = commentsField?.comments;
  if (!Array.isArray(comments)) return [];

  return comments
    .map((item) => {
      if (!item || typeof item !== "object") return null;
      const id = typeof item.id === "string" ? item.id : "";
      const body = normalizeDescription(item.body);
      if (!id && !body) return null;
      return { id, body };
    })
    .filter(Boolean);
};

export const fetchJiraIssue = async ({
  issueKey,
  baseUrl,
  email,
  apiToken,
  commandName = "fetch-jira.js",
  fetchImpl = fetch,
}) => {
  if (!issueKey) {
    throw new Error(`Falta issueKey. Uso: node scripts/${commandName} <ISSUE-KEY>.`);
  }
  if (!baseUrl) {
    throw new Error("Falta JIRA_BASE_URL. Definelo en el entorno antes de ejecutar.");
  }

  const authHeader = buildJiraAuthHeader(email, apiToken);
  const normalizedBase = baseUrl.replace(/\/+$/, "");
  const requestedFields = [
    "summary",
    "description",
    "issuetype",
    "priority",
    "labels",
    "components",
    "subtasks",
    "comment",
  ].join(",");
  const url = `${normalizedBase}/rest/api/3/issue/${issueKey}?fields=${requestedFields}`;

  const response = await fetchImpl(url, {
    headers: {
      Authorization: authHeader,
      Accept: "application/json",
    },
  });

  if (!response.ok) {
    const body = await response.text().catch(() => "");
    const detail = body ? ` Detalle: ${body}` : "";
    throw new Error(
      `Error consultando JIRA (${response.status} ${response.statusText}).${detail}`
    );
  }

  const payload = await response.json();
  const fields = payload?.fields ?? {};
  const summary = fields.summary ?? "";
  const description = normalizeDescription(fields.description);
  const labels = Array.isArray(fields.labels) ? fields.labels : [];
  const components = Array.isArray(fields.components)
    ? fields.components
        .map((item) => (typeof item?.name === "string" ? item.name : null))
        .filter(Boolean)
    : [];
  const subtasks = Array.isArray(fields.subtasks)
    ? fields.subtasks
        .map((item) =>
          typeof item?.key === "string"
            ? {
                key: item.key,
                summary:
                  typeof item?.fields?.summary === "string"
                    ? item.fields.summary
                    : "",
              }
            : null,
        )
        .filter(Boolean)
    : [];
  const comments = normalizeComments(fields.comment);

  return {
    issueKey,
    summary,
    description,
    metadata: {
      issueType: fields?.issuetype?.name ?? "",
      priority: fields?.priority?.name ?? "",
      labels,
      components,
      subtasks,
      comments,
    },
  };
};

const jiraRequest = async ({
  method,
  url,
  email,
  apiToken,
  body,
  fetchImpl = fetch,
}) => {
  const authHeader = buildJiraAuthHeader(email, apiToken);
  const response = await fetchImpl(url, {
    method,
    headers: {
      Authorization: authHeader,
      Accept: "application/json",
      "Content-Type": "application/json",
    },
    body: body ? JSON.stringify(body) : undefined,
  });

  if (!response.ok) {
    const raw = await response.text().catch(() => "");
    const detail = raw ? ` Detalle: ${raw}` : "";
    throw new Error(
      `Error Jira (${response.status} ${response.statusText}).${detail}`,
    );
  }

  if (response.status === 204) {
    return null;
  }

  return response.json();
};

export const getJiraTransitions = async ({
  issueKey,
  baseUrl,
  email,
  apiToken,
  fetchImpl = fetch,
}) => {
  if (!baseUrl) {
    throw new Error("Falta JIRA_BASE_URL. Definelo en el entorno antes de ejecutar.");
  }

  const normalizedBase = baseUrl.replace(/\/+$/, "");
  return jiraRequest({
    method: "GET",
    url: `${normalizedBase}/rest/api/3/issue/${issueKey}/transitions`,
    email,
    apiToken,
    fetchImpl,
  });
};

const findTransition = ({ transitions, target }) => {
  const list = Array.isArray(transitions) ? transitions : [];
  if (target === "done") {
    return (
      list.find((item) => item?.to?.statusCategory?.key === "done") ??
      list.find((item) => /done|final|cerrad|resuelt/i.test(item?.name ?? ""))
    );
  }

  return (
    list.find((item) => item?.to?.statusCategory?.key === "indeterminate") ??
    list.find((item) => /progress|progreso|curso/i.test(item?.name ?? ""))
  );
};

export const transitionJiraIssue = async ({
  issueKey,
  baseUrl,
  email,
  apiToken,
  target,
  fetchImpl = fetch,
}) => {
  const transitions = await getJiraTransitions({
    issueKey,
    baseUrl,
    email,
    apiToken,
    fetchImpl,
  });
  const transition = findTransition({
    transitions: transitions?.transitions ?? [],
    target,
  });

  if (!transition?.id) {
    throw new Error(
      `No se encontro una transicion Jira compatible con target='${target}' para ${issueKey}.`,
    );
  }

  const normalizedBase = baseUrl.replace(/\/+$/, "");
  await jiraRequest({
    method: "POST",
    url: `${normalizedBase}/rest/api/3/issue/${issueKey}/transitions`,
    email,
    apiToken,
    fetchImpl,
    body: {
      transition: {
        id: String(transition.id),
      },
    },
  });

  return transition;
};

const toAdfParagraph = (text) => ({
  type: "paragraph",
  content: [{ type: "text", text }],
});

export const addJiraComment = async ({
  issueKey,
  baseUrl,
  email,
  apiToken,
  message,
  fetchImpl = fetch,
}) => {
  if (!message?.trim()) {
    throw new Error("No se puede comentar en Jira: message es obligatorio.");
  }
  const normalizedBase = baseUrl.replace(/\/+$/, "");
  await jiraRequest({
    method: "POST",
    url: `${normalizedBase}/rest/api/3/issue/${issueKey}/comment`,
    email,
    apiToken,
    fetchImpl,
    body: {
      body: {
        type: "doc",
        version: 1,
        content: [toAdfParagraph(message.trim())],
      },
    },
  });
};
