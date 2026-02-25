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
  const url = `${normalizedBase}/rest/api/3/issue/${issueKey}?fields=summary,description`;

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

  return { issueKey, summary, description };
};
