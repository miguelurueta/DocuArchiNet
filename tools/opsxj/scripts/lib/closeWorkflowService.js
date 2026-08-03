import { addJiraComment, transitionJiraIssue } from "./jiraClient.js";
import { getMergedPullRequestByBranch } from "./githubClient.js";

export const closeIssueFromMergedPr = async ({
  issueKey,
  branchName,
  jira,
  github,
  fetchImpl = fetch,
}) => {
  const mergedPrResult = await getMergedPullRequestByBranch({
    repo: github.repo,
    owner: github.owner,
    repoName: github.repoName,
    token: github.token,
    branchName,
    baseBranch: github.baseBranch ?? "main",
    fetchImpl,
  });

  const mergedPullRequest = mergedPrResult.pullRequest;
  if (!mergedPullRequest) {
    throw new Error(
      `No existe un PR mergeado para ${issueKey} en la rama ${branchName}.`,
    );
  }

  const transition = await transitionJiraIssue({
    issueKey,
    baseUrl: jira.baseUrl,
    email: jira.email,
    apiToken: jira.apiToken,
    target: "done",
    fetchImpl,
  });

  const prUrl = mergedPullRequest.html_url ?? "(sin URL)";
  await addJiraComment({
    issueKey,
    baseUrl: jira.baseUrl,
    email: jira.email,
    apiToken: jira.apiToken,
    message: `Cierre manual validado: PR mergeado ${prUrl}. Jira movido a ${transition?.to?.name ?? "Done"}.`,
    fetchImpl,
  });

  return {
    pullRequest: mergedPullRequest,
    transition,
  };
};
