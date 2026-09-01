'use strict';

const { test, expect } = require('@playwright/test');
const { createAuthenticatedWorkflowSession } = require('./support/authenticated-workflow-session.cjs');

test.use({ screenshot: 'off', trace: 'off', video: 'off' });

function required(name) {
  const value = process.env[name];
  if (!value || !value.trim()) throw new Error(`${name} es obligatoria.`);
  return value.trim();
}

test('@doc43-notes-ui CRUD moderno usa tarea explícita y conflicto seguro', async ({ browser }) => {
  expect(required('DOC43_E2E_ENVIRONMENT_AUTHORIZED')).toBe('true');
  expect(required('DOC43_E2E_EXECUTION_AUTHORIZED')).toBe('true');
  const baseUrl = new URL(required('DOC43_E2E_BASE_URL')).toString();
  const taskId = Number(required('DOC43_E2E_TASK_ID'));
  expect(Number.isSafeInteger(taskId) && taskId > 0).toBeTruthy();

  const context = await createAuthenticatedWorkflowSession(browser, {
    baseUrl,
    moduleEnvironmentVariable: 'DOC43_E2E_MODULE',
    userEnvironmentVariable: 'DOC43_E2E_AUTHORIZED_USER',
    passwordEnvironmentVariable: 'DOC43_E2E_AUTHORIZED_PASSWORD',
    ignoreHTTPSErrors: process.env.DOC43_E2E_IGNORE_HTTPS_ERRORS === 'true'
  });
  const page = await context.newPage();
  try {
    await page.goto(new URL('workflow/Webworkflow.aspx', baseUrl).toString(), { waitUntil: 'domcontentloaded' });
    await expect(page.locator('#Hidden_id_tarea_selecionada')).toHaveValue(String(taskId));
    const root = page.locator('[data-workflow-notes-modern="true"]');
    await expect(root).toBeVisible();
    await expect(root.locator('#workflow-notes-modern-list')).toBeAttached();

    const residues = root.locator('.note').filter({ hasText: 'DOC43-' });
    while (await residues.count()) {
      const before = await residues.count();
      page.once('dialog', dialog => dialog.accept());
      await residues.first().getByRole('button', { name: 'Eliminar' }).click();
      await expect(residues).toHaveCount(before - 1);
    }

    const marker = `DOC43-${Date.now()}`;
    const unique = `${marker}-á漢字-<script>alert("xss")</script>\nlínea 2`;
    await root.locator('#workflow-notes-modern-new').click();
    await root.locator('#workflow-notes-modern-text').fill(unique);
    await root.locator('#workflow-notes-modern-save').click();
    await expect(root.locator('#workflow-notes-modern-status')).toContainText('Nota guardada.');

    const note = root.locator('.note').filter({ hasText: unique });
    await expect(note).toHaveCount(1);
    await note.getByRole('button', { name: 'Editar' }).click();
    await root.locator('#workflow-notes-modern-text').fill(`${unique}-editada`);
    await root.locator('#workflow-notes-modern-save').click();
    await expect(root.locator('#workflow-notes-modern-status')).toContainText('Nota actualizada.');

    const updated = root.locator('.note').filter({ hasText: `${unique}-editada` });
    page.once('dialog', dialog => dialog.accept());
    await updated.getByRole('button', { name: 'Eliminar' }).click();
    await expect(root.locator('#workflow-notes-modern-status')).toContainText('Nota eliminada.');
    await expect(root.locator('.note').filter({ hasText: marker })).toHaveCount(0);
  } finally {
    await context.close();
  }
});
