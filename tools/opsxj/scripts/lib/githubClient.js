const buildRepoInfo = ({
  repo,
  owner,
  name,
}) => {
  if (repo) {
    const [repoOwner, repoName] = String(repo).split("/");
    if (repoOwner && repoName) {
      return { owner: repoOwner, repo: repoName };
    }
  }
  if (owner && name) {
    return { owner, repo: name };
  }
  throw new Error(
    "Falta configuracion GitHub. Defina GITHUB_REPO (owner/repo) o GITHUB_OWNER + GITHUB_REPO_NAME.",
  );
};

const requestGitHub = async ({
  method,
  path,
  token,
  body,
  fetchImpl = fetch,
}) => {
  if (!token) {
    throw new Error("Falta GITHUB_TOKEN para operar con pull requests.");
  }

  const response = await fetchImpl(`https://api.github.com${path}`, {
    method,
    headers: {
      Authorization: `Bearer ${token}`,
      Accept: "application/vnd.github+json",
      "Content-Type": "application/json",
      "X-GitHub-Api-Version": "2022-11-28",
    },
    body: body ? JSON.stringify(body) : undefined,
  });

  if (!response.ok) {
    const raw = await response.text().catch(() => "");
    const detail = raw ? ` Detalle: ${raw}` : "";
    throw new Error(
      `Error GitHub (${response.status} ${response.statusText}).${detail}`,
    );
  }

  if (response.status === 204) {
    return null;
  }

  return response.json();
};

const findOpenPullRequest = async ({
  owner,
  repo,
  branchName,
  baseBranch,
  token,
  fetchImpl,
}) => {
  const encodedHead = encodeURIComponent(`${owner}:${branchName}`);
  const encodedBase = encodeURIComponent(baseBranch);
  const pulls = await requestGitHub({
    method: "GET",
    path: `/repos/${owner}/${repo}/pulls?state=open&head=${encodedHead}&base=${encodedBase}`,
    token,
    fetchImpl,
  });
  return Array.isArray(pulls) ? pulls[0] : null;
};

const listClosedPullRequestsByBranch = async ({
  owner,
  repo,
  branchName,
  baseBranch,
  token,
  fetchImpl,
}) => {
  const encodedHead = encodeURIComponent(`${owner}:${branchName}`);
  const encodedBase = encodeURIComponent(baseBranch);
  return requestGitHub({
    method: "GET",
    path: `/repos/${owner}/${repo}/pulls?state=closed&head=${encodedHead}&base=${encodedBase}`,
    token,
    fetchImpl,
  });
};

export const createOrGetPullRequest = async ({
  repo,
  owner,
  repoName,
  token,
  issueKey,
  summary,
  branchName,
  baseBranch = "main",
  fetchImpl = fetch,
}) => {
  const resolved = buildRepoInfo({
    repo,
    owner,
    name: repoName,
  });

  const titleSummary = String(summary ?? "").trim();
  const title = titleSummary
    ? `${String(issueKey).toUpperCase()} ${titleSummary}`
    : `${String(issueKey).toUpperCase()} OpenSpec Archive`;

  const body = [
    "## Trazabilidad",
    "",
    `- Jira: ${String(issueKey).toUpperCase()}`,
    `- Branch: \`${branchName}\``,
    "",
    "PR creado automaticamente por `opsxj:archive`.",
  ].join("\n");

  try {
    const created = await requestGitHub({
      method: "POST",
      path: `/repos/${resolved.owner}/${resolved.repo}/pulls`,
      token,
      body: {
        title,
        head: branchName,
        base: baseBranch,
        body,
      },
      fetchImpl,
    });
    return {
      pullRequest: created,
      created: true,
      repository: resolved,
    };
  } catch (error) {
    const message = error instanceof Error ? error.message : String(error);
    if (!/A pull request already exists/i.test(message)) {
      throw error;
    }

    const existing = await findOpenPullRequest({
      owner: resolved.owner,
      repo: resolved.repo,
      branchName,
      baseBranch,
      token,
      fetchImpl,
    });
    if (!existing) {
      throw new Error(
        `GitHub reporto PR existente para ${branchName}, pero no fue posible recuperarlo.`,
      );
    }
    return {
      pullRequest: existing,
      created: false,
      repository: resolved,
    };
  }
};

export const getMergedPullRequestByBranch = async ({
  repo,
  owner,
  repoName,
  token,
  branchName,
  baseBranch = "main",
  fetchImpl = fetch,
}) => {
  const resolved = buildRepoInfo({
    repo,
    owner,
    name: repoName,
  });

  const pulls = await listClosedPullRequestsByBranch({
    owner: resolved.owner,
    repo: resolved.repo,
    branchName,
    baseBranch,
    token,
    fetchImpl,
  });

  const merged = Array.isArray(pulls)
    ? pulls.find((pr) => Boolean(pr?.merged_at))
    : null;

  return {
    pullRequest: merged ?? null,
    repository: resolved,
  };
};

export const getPullRequestStatusByBranch = async ({
  repo,
  owner,
  repoName,
  token,
  branchName,
  baseBranch = "main",
  fetchImpl = fetch,
}) => {
  const resolved = buildRepoInfo({
    repo,
    owner,
    name: repoName,
  });

  const openPullRequest = await findOpenPullRequest({
    owner: resolved.owner,
    repo: resolved.repo,
    branchName,
    baseBranch,
    token,
    fetchImpl,
  });

  if (openPullRequest) {
    return {
      state: "open",
      merged: false,
      pullRequest: openPullRequest,
      repository: resolved,
    };
  }

  const closedPullRequests = await listClosedPullRequestsByBranch({
    owner: resolved.owner,
    repo: resolved.repo,
    branchName,
    baseBranch,
    token,
    fetchImpl,
  });
  const closedPullRequest = Array.isArray(closedPullRequests)
    ? closedPullRequests[0]
    : null;

  return {
    state: closedPullRequest ? "closed" : "missing",
    merged: Boolean(closedPullRequest?.merged_at),
    pullRequest: closedPullRequest,
    repository: resolved,
  };
};
