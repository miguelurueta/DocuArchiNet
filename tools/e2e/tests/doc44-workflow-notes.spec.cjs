'use strict';

const { test, expect } = require('@playwright/test');
const { createAuthenticatedWorkflowSession } = require('./support/authenticated-workflow-session.cjs');

test.use({ screenshot: 'off', trace: 'off', video: 'off' });

function required(name) {
  const value = process.env[name];
  if (!value || !value.trim()) throw new Error(`${name} es obligatoria.`);
  return value.trim();
}

test('@doc44-workflow-notes regresión exclusiva conserva tarea explícita y una mutación por acción', async ({ browser }) => {
  expect(required('DOC44_E2E_ENVIRONMENT_AUTHORIZED')).toBe('true');
  expect(required('DOC44_E2E_EXECUTION_AUTHORIZED')).toBe('true');
  expect(required('DOC44_E2E_GATE_AUTHORIZED')).toBe('true');
  const baseUrl = new URL(required('DOC44_E2E_BASE_URL')).toString();
  const taskId = Number(required('DOC44_E2E_TASK_ID'));
  const foreignTaskId = Number(required('DOC44_E2E_FOREIGN_TASK_ID'));
  const inactiveTaskId = Number(required('DOC44_E2E_INACTIVE_TASK_ID'));
  const foreignNoteId = Number(required('DOC44_E2E_FOREIGN_NOTE_ID'));
  expect(Number.isSafeInteger(taskId) && taskId > 0).toBeTruthy();
  for (const value of [foreignTaskId, inactiveTaskId, foreignNoteId]) expect(Number.isSafeInteger(value) && value > 0).toBeTruthy();

  const context = await createAuthenticatedWorkflowSession(browser, {
    baseUrl,
    moduleEnvironmentVariable: 'DOC44_E2E_MODULE',
    userEnvironmentVariable: 'DOC44_E2E_AUTHORIZED_USER',
    passwordEnvironmentVariable: 'DOC44_E2E_AUTHORIZED_PASSWORD',
    ignoreHTTPSErrors: process.env.DOC44_E2E_IGNORE_HTTPS_ERRORS === 'true'
  });
  const page = await context.newPage();
  try {
    await page.goto(new URL('workflow/Webworkflow.aspx', baseUrl).toString(), { waitUntil: 'domcontentloaded' });
    await expect(page.locator('#Hidden_id_tarea_selecionada')).toHaveValue(String(taskId));
    const root = page.locator('[data-workflow-notes-modern="true"]');
    await expect(root).toBeVisible();
    await expect(page.locator('[id$="Panel_Buttonanotacion"]')).toBeHidden();

    const invoke = async (operation, data) => {
      const response = await context.request.post(new URL(`webservice/WebServiceWorkflowNotesModern.asmx/${operation}`, baseUrl).toString(), { headers: { 'X-Requested-With': 'XMLHttpRequest' }, data });
      expect(response.ok()).toBeTruthy();
      const payload = await response.json();
      return payload && payload.d !== undefined ? payload.d : payload;
    };
    const functionalCode = dto => dto && (dto.Codigo || (dto.Error && dto.Error.Codigo));
    const foreignTask = await invoke('ListarNotas', { idTarea: foreignTaskId, cursor: '', tamanoPagina: 1 });
    expect(foreignTask.Exito).not.toBe(true);
    expect(['Forbidden', 'TaskNotActive']).toContain(functionalCode(foreignTask));
    const inactiveTask = await invoke('ListarNotas', { idTarea: inactiveTaskId, cursor: '', tamanoPagina: 1 });
    expect(inactiveTask.Exito).not.toBe(true);
    expect(['Forbidden', 'TaskNotActive']).toContain(functionalCode(inactiveTask));
    const crossedNote = await invoke('ConsultarNota', { idTarea: taskId, idNota: foreignNoteId });
    expect(crossedNote.Exito).not.toBe(true);
    expect(['Forbidden', 'NoteNotFound', 'NotOwner']).toContain(functionalCode(crossedNote));

    const marker = `DOC44-${Date.now()}`;
    const content = `${marker}-á漢字-<script>alert("xss")</script>`;
    await root.locator('#workflow-notes-modern-new').click();
    await root.locator('#workflow-notes-modern-text').fill(content);
    await root.locator('#workflow-notes-modern-save').click();
    await expect(root.locator('#workflow-notes-modern-status')).toContainText('Nota guardada.');
    await expect(root.locator('.note').filter({ hasText: content })).toHaveCount(1);

    const note = root.locator('.note').filter({ hasText: marker });
    await note.getByRole('button', { name: 'Editar' }).click();
    await root.locator('#workflow-notes-modern-text').fill(`${content}-editada`);
    await root.locator('#workflow-notes-modern-save').click();
    await expect(root.locator('#workflow-notes-modern-status')).toContainText('Nota actualizada.');

    page.once('dialog', dialog => dialog.accept());
    await root.locator('.note').filter({ hasText: marker }).getByRole('button', { name: 'Eliminar' }).click();
    await expect(root.locator('#workflow-notes-modern-status')).toContainText('Nota eliminada.');
    await expect(root.locator('.note').filter({ hasText: marker })).toHaveCount(0);
  } finally {
    await context.close();
  }
});
