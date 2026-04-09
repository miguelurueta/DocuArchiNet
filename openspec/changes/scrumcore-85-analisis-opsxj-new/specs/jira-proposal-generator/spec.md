## ADDED Requirements

### Requirement: Diagnostic scope stays read-only
The diagnostic workflow for `jira-proposal-generator` SHALL document the current failure modes and affected code paths without changing the functional behavior of `opsxj:new` in this ticket.

#### Scenario: Diagnosis records the affected layers
- **WHEN** the analysis reviews how `opsxj:new` creates `proposal.md`
- **THEN** it MUST identify whether the issue lives in command orchestration, Jira reading, or proposal inference logic

#### Scenario: Diagnosis does not alter runtime behavior
- **WHEN** the ticket completes
- **THEN** `opsxj:new` MUST retain the same observable command behavior because this change only delivers analysis and recommendations

### Requirement: Diagnosis captures proposal quality failures with evidence
The diagnostic output SHALL describe the concrete ways in which generated proposals become too generic and MUST reference repository evidence that demonstrates those failures.

#### Scenario: Repository tests are used as evidence
- **WHEN** the analysis identifies a mismatch between intended and observed proposal content
- **THEN** it MUST reference existing tests or archived changes that already express the intended behavior

#### Scenario: Artificial capability patterns are documented
- **WHEN** the analysis finds proposals that invent capabilities from issue keys, command names, or generic boilerplate
- **THEN** it MUST record those patterns as failure modes for the subsequent correction ticket

### Requirement: Diagnosis produces actionable recommendations
The diagnostic result SHALL leave explicit recommendations for a follow-up correction ticket, including likely code touchpoints and constraints to preserve.

#### Scenario: Recommendations preserve command scope
- **WHEN** the diagnosis proposes likely fixes
- **THEN** it MUST distinguish between fixes in proposal inference and the broader `opsxj:new` orchestration so the follow-up change stays properly scoped

#### Scenario: Recommendations identify next-step artifacts
- **WHEN** the diagnosis completes
- **THEN** it MUST leave enough detail to open a correction ticket with coherent proposal, design, specs, and implementation tasks
