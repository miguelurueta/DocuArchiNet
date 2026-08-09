import { fetchJiraIssue } from "./jiraClient.js";
import {
  buildChangeNameFromJiraSummary,
  buildProposalContent,
  writeRefinementArtifacts,
  writeProposalFile,
} from "./proposalGenerator.js";

export const createProposalFromJira = async ({
  issueKey,
  baseUrl,
  email,
  apiToken,
  baseDir,
  commandName,
  folderStrategy = "issueKey",
  impact,
  fetchImpl = fetch,
}) => {
  const issue = await fetchJiraIssue({
    issueKey,
    baseUrl,
    email,
    apiToken,
    commandName,
    fetchImpl,
  });

  const content = buildProposalContent(issue);
  const changeName =
    folderStrategy === "summary"
      ? buildChangeNameFromJiraSummary(issue)
      : issue.issueKey;

  const proposalPath = await writeProposalFile({
    issueKey: issue.issueKey,
    changeName,
    content,
    baseDir,
  });

  const refinementArtifacts = await writeRefinementArtifacts({
    issueKey: issue.issueKey,
    changeName,
    summary: issue.summary,
    description: issue.description,
    metadata: issue.metadata,
    impact,
    baseDir,
  });

  return {
    issue,
    content,
    proposalPath,
    changeName,
    refinementArtifacts,
  };
};
