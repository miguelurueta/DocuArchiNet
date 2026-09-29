'use strict';

const path = require('node:path');
const { spawnSync } = require('node:child_process');

const repositoryRoot = path.resolve(__dirname, '..', '..', '..');
const tests = Object.freeze([
  'tests/importar-servicio-web-enlase-context.test.cjs',
  'tests/importar-servicio-web-enlase-list.test.cjs',
  'tests/importar-servicio-web-preview-authorization.test.cjs',
  'tests/importar-servicio-web-preview-mediation.test.cjs',
  'tests/importar-servicio-web-preparation.test.cjs',
  'tests/importar-servicio-web-document-type-catalog.test.cjs',
  'tests/importar-servicio-web-doc83-receipt-cardinality.test.cjs',
  'tests/importar-servicio-web-enlase-persistence-contract.test.cjs',
  'tests/importar-servicio-web-execution-contract.test.cjs',
  'tests/importar-servicio-web-intent-idempotency.test.cjs',
  'tests/importar-servicio-web-intent-concurrency.test.cjs',
  'tests/importar-servicio-web-reconciliation.test.cjs',
  'tests/importar-servicio-web-task-context-guard.test.cjs',
  'tests/importar-servicio-web-task-isolation.test.cjs',
  'tests/importar-servicio-web-progress-adapter.test.cjs',
  'tests/importar-servicio-web-reconciliation-ui.test.cjs',
  'tests/importar-servicio-web-document-list-adapter.test.cjs',
  'tests/importar-servicio-web-ui-architecture.test.cjs',
  'tests/importar-servicio-web-multi-tab-context.test.cjs',
  'tests/importar-servicio-web-enlase-assignment.test.cjs',
  'tests/workflow-transition-confirmation-integration.test.cjs',
  'tests/importar-servicio-web-enlase-ui.test.cjs',
  'tests/importar-servicio-web-enlase-accessibility.test.cjs',
  'tests/importar-servicio-web-gate.test.cjs',
  'tests/importar-servicio-web-gate-regression.test.cjs',
  'tests/importar-servicio-web-legacy-regression.test.cjs',
  'tests/importar-servicio-web-legacy-surface-invariance.test.cjs',
  'tests/importar-servicio-web-legacy-ui-regression.test.cjs',
  'tests/importar-servicio-web-authorization.test.cjs',
  'tests/importar-servicio-web-preview-security.test.cjs',
  'tools/e2e/tests/workflow-e2e-platform-profile.test.cjs',
  'tools/e2e/tests/workflow-e2e-platform-security.test.cjs',
  'tools/e2e/tests/workflow-e2e-platform.test.cjs',
  'tools/e2e/tests/doc80-import-sii-enlase-read.test.cjs',
  'tools/e2e/tests/doc81-import-sii-enlase-execution.test.cjs',
  'tools/e2e/tests/doc82-import-sii-enlase-ui.test.cjs',
  'tools/e2e/tests/doc83-import-sii-enlase-anonymous.test.cjs',
  'tools/e2e/tests/doc83-import-sii-enlase-assignment.test.cjs',
  'tools/e2e/tests/doc83-import-sii-enlase-manual-visual.test.cjs',
  'tools/e2e/tests/doc83-import-sii-enlase-layout-review.test.cjs',
  'tools/e2e/tests/doc83-sii-enlase-closure-matrix.test.cjs'
]);

const result = spawnSync(process.execPath, ['--test', ...tests], {
  cwd: repositoryRoot,
  stdio: 'inherit',
  windowsHide: true
});

if (result.error) throw result.error;
process.exitCode = result.status === null ? 1 : result.status;
