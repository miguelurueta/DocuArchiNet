'use strict';

const { test, expect } = require('@playwright/test');
const { createAuthenticatedWorkflowSession } = require('./support/authenticated-workflow-session.cjs');

test.use({ screenshot: 'off', trace: 'off', video: 'off' });
test.setTimeout(180000);

function required(name) {
  const value = process.env[name];
  if (!value || !value.trim()) throw new Error(`${name} es obligatoria.`);
  return value.trim();
}

test('@doc44-workflow-notes regresión exclusiva conserva tarea explícita y una mutación por acción', async ({ browser }) => {
  expect(required('DOC44_E2E_ENVIRONMENT_AUTHORIZED')).toBe('true');
  expect(required('DOC44_E2E_EXECUTION_AUTHORIZED')).toBe('true');
  const baseUrl = new URL(required('DOC44_E2E_BASE_URL')).toString();
  const taskId = Number(required('DOC44_E2E_TASK_ID'));
  const foreignTaskId = Number(required('DOC44_E2E_FOREIGN_TASK_ID'));
  const inactiveTaskId = Number(required('DOC44_E2E_INACTIVE_TASK_ID'));
  const foreignNoteId = Number(required('DOC44_E2E_FOREIGN_NOTE_ID'));
  const nonOwnerNoteId = Number(required('DOC44_E2E_NON_OWNER_NOTE_ID'));
  expect(Number.isSafeInteger(taskId) && taskId > 0).toBeTruthy();
  for (const value of [foreignTaskId, inactiveTaskId, foreignNoteId, nonOwnerNoteId]) expect(Number.isSafeInteger(value) && value > 0).toBeTruthy();

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
    const selectedTask = page.locator('#Hidden_id_tarea_selecionada');
    if (await selectedTask.inputValue() !== String(taskId)) {
      await page.evaluate(() => window.hide_area_workflow_seleccion());
      const selectCommand = page.locator(`[tip_event="seleccion_tarea_wf"][idd="${taskId}"]:visible`).first();
      if (!await selectCommand.count()) {
        const taskSearch = page.locator('#auto_complex:visible');
        await expect(taskSearch, 'La vista de tareas no habilitó su búsqueda.').toBeVisible({ timeout: 15000 });
        await taskSearch.fill(String(taskId));
        await page.locator('button[title="consultar lista"]:visible').click();
      }
      await expect(selectCommand, 'La tarea autorizada no está disponible para seleccionarse en la UI Workflow.').toBeVisible();
      await selectCommand.click();
      await expect(selectedTask, 'La UI Workflow no confirmó la selección de la tarea autorizada.').toHaveValue(String(taskId), { timeout: 30000 });
    }
    await expect(page.locator('#Hidden_id_tarea_selecionada')).toHaveValue(String(taskId));
    const root = page.locator('[data-workflow-notes-modern="true"]');
    await expect(root).toBeHidden();
    await expect(page.locator('[id$="Panel_Buttonanotacion"]')).toHaveCount(0);
    const notesAccess = page.locator('#workflow-notes-modern-access');
    await expect(notesAccess).toBeVisible();
    await expect(notesAccess.locator('#workflow-notes-modern-access-count')).toHaveText(/^\d+$/);
    await notesAccess.click();
    await expect(root).toBeVisible();
    await expect(root).toBeFocused();
    const initialModalHeight = (await root.locator(':scope > .notes-shell').boundingBox()).height;

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

    const nonOwner = await invoke('ConsultarNota', { idTarea: taskId, idNota: nonOwnerNoteId });
    expect(nonOwner.Exito).toBe(true);
    expect(nonOwner.Nota.PuedeGestionar).toBe(false);
    const nonOwnerCard = root.locator(`[data-note-id="${nonOwnerNoteId}"]`);
    await expect(nonOwnerCard).toHaveCount(1);
    await expect(nonOwnerCard.getByRole('button', { name: 'Editar' })).toHaveCount(0);
    await expect(nonOwnerCard.getByRole('button', { name: 'Eliminar' })).toHaveCount(0);
    const rejectedUpdate = await invoke('ActualizarNota', { idTarea: taskId, idNota: nonOwnerNoteId, contenido: nonOwner.Nota.Contenido, version: nonOwner.Nota.Version });
    expect(rejectedUpdate.Exito).not.toBe(true);
    expect(functionalCode(rejectedUpdate)).toBe('NotOwner');
    const rejectedDelete = await invoke('EliminarNota', { idTarea: taskId, idNota: nonOwnerNoteId, version: nonOwner.Nota.Version });
    expect(rejectedDelete.Exito).not.toBe(true);
    expect(functionalCode(rejectedDelete)).toBe('NotOwner');
    const afterRejectedMutations = await invoke('ConsultarNota', { idTarea: taskId, idNota: nonOwnerNoteId });
    expect(afterRejectedMutations.Exito).toBe(true);
    expect(afterRejectedMutations.Nota.Version).toBe(nonOwner.Nota.Version);

    const marker = `DOC44-${Date.now()}`;
    const content = `${marker}-á漢字-<script>alert("xss")</script>-${'contexto-seguro-'.repeat(20)}`;
    await root.locator('#workflow-notes-modern-new').click();
    await root.locator('#workflow-notes-modern-text').fill(content);
    await root.locator('#workflow-notes-modern-save').click();
    await expect(root.locator('#workflow-notes-modern-status')).toContainText('Nota guardada.');
    await expect(root.locator('.note').filter({ hasText: content })).toHaveCount(1);

    const note = root.locator('.note').filter({ hasText: marker });
    const viewFull = note.getByRole('button', { name: 'Ver nota completa' });
    await viewFull.click();
    const viewer = root.locator('#workflow-notes-modern-viewer');
    await expect(viewer).toBeVisible();
    await expect(viewer.locator('#workflow-notes-modern-viewer-content')).toBeFocused();
    await viewer.press('Escape');
    await expect(viewer).toBeHidden();
    await expect(viewFull).toBeFocused();
    await note.getByRole('button', { name: 'Editar' }).click();
    await root.locator('#workflow-notes-modern-text').fill(`${content}-editada`);
    await root.locator('#workflow-notes-modern-save').click();
    await expect(root.locator('#workflow-notes-modern-status')).toContainText('Nota actualizada.');

    const updatedNote = root.locator('.note').filter({ hasText: `${content}-editada` });
    await expect(updatedNote).toHaveCount(1);
    await updatedNote.getByRole('button', { name: 'Eliminar' }).click();
    const deleteConfirm = root.locator('#workflow-notes-modern-delete-confirm');
    await expect(deleteConfirm).toBeVisible();
    await deleteConfirm.getByRole('button', { name: 'Cancelar' }).click();
    await expect(deleteConfirm).toBeHidden();
    await expect(updatedNote).toHaveCount(1);
    await updatedNote.getByRole('button', { name: 'Eliminar' }).click();
    await deleteConfirm.getByRole('button', { name: 'Eliminar nota' }).click();
    await expect(root.locator('#workflow-notes-modern-status')).toContainText('Nota eliminada.');
    await expect(root.locator('.note').filter({ hasText: marker })).toHaveCount(0);
    await expect(root.locator('#workflow-notes-modern-status')).toHaveText('', { timeout: 5000 });
    const finalModalHeight = (await root.locator(':scope > .notes-shell').boundingBox()).height;
    expect(Math.abs(finalModalHeight - initialModalHeight)).toBeLessThanOrEqual(1);
    await root.locator('#workflow-notes-modern-dismiss').click();
    await expect(root).toBeHidden();
    await expect(notesAccess).toBeFocused();
  } finally {
    if (browser.isConnected()) await context.close().catch(() => {});
  }
});

test('@doc45-unassigned-color tarea no asignada conserva acción verde', async ({ browser }) => {
  expect(required('DOC44_E2E_ENVIRONMENT_AUTHORIZED')).toBe('true');
  const baseUrl = new URL(required('DOC44_E2E_BASE_URL')).toString();

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
    await page.evaluate(() => window.hide_area_workflow_seleccion());
    const action = page.locator('#GridView2 [tip_event="seleccion_tarea_wf"].btn-success:visible').first();
    await expect(action, 'La cuenta autorizada no tiene una tarea no asignada visible en la página actual.').toBeVisible();
    await expect.poll(() => action.evaluate(element => getComputedStyle(element).backgroundColor)).toBe('rgb(33, 136, 56)');
    await expect.poll(() => action.locator('i, svg').first().evaluate(element => getComputedStyle(element).color)).toBe('rgb(255, 255, 255)');
  } finally {
    await context.close().catch(() => {});
  }
});

test('@doc45-empty-notes estado vacío ofrece creación inmediata y se restaura', async ({ browser }) => {
  expect(required('DOC44_E2E_ENVIRONMENT_AUTHORIZED')).toBe('true');
  expect(required('DOC44_E2E_EXECUTION_AUTHORIZED')).toBe('true');
  expect(required('DOC44_E2E_EMPTY_MODE')).toBe('true');
  const baseUrl = new URL(required('DOC44_E2E_BASE_URL')).toString();
  const taskId = Number(required('DOC44_E2E_TASK_ID'));
  expect(Number.isSafeInteger(taskId) && taskId > 0).toBeTruthy();

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
    const selectedTask = page.locator('#Hidden_id_tarea_selecionada');
    if (await selectedTask.inputValue() !== String(taskId)) {
      await page.evaluate(() => window.hide_area_workflow_seleccion());
      const selectCommand = page.locator(`[tip_event="seleccion_tarea_wf"][idd="${taskId}"]:visible`).first();
      if (!await selectCommand.count()) {
        const taskSearch = page.locator('#auto_complex:visible');
        await expect(taskSearch, 'La vista de tareas no habilitó su búsqueda.').toBeVisible({ timeout: 15000 });
        await taskSearch.fill(String(taskId));
        await page.locator('button[title="consultar lista"]:visible').click();
      }
      await expect(selectCommand, 'La tarea vacía autorizada no está disponible para seleccionarse.').toBeVisible();
      await selectCommand.click();
      await expect(selectedTask).toHaveValue(String(taskId), { timeout: 30000 });
    }

    const root = page.locator('[data-workflow-notes-modern="true"]');
    const access = page.locator('#workflow-notes-modern-access');
    await expect(access.locator('#workflow-notes-modern-access-label')).toHaveText(/Nueva nota/);
    await expect(access.locator('#workflow-notes-modern-access-count')).toHaveText('0');
    await access.click();
    await expect(root).toBeVisible();
    const editor = root.locator('#workflow-notes-modern-editor');
    await expect(editor).toBeVisible();
    await expect(root.locator('#workflow-notes-modern-text')).toBeFocused();

    const marker = `DOC45-EMPTY-${Date.now()}`;
    await root.locator('#workflow-notes-modern-text').fill(marker);
    await root.locator('#workflow-notes-modern-save').click();
    await expect(root.locator('#workflow-notes-modern-status')).toContainText('Nota guardada.');
    await expect(access.locator('#workflow-notes-modern-access-label')).toHaveText(/Notas/);
    await expect(access.locator('#workflow-notes-modern-access-count')).toHaveText('1');
    const note = root.locator('.note').filter({ hasText: marker });
    await expect(note).toHaveCount(1);
    await note.getByRole('button', { name: 'Eliminar' }).click();
    await root.locator('#workflow-notes-modern-delete-confirm').getByRole('button', { name: 'Eliminar nota' }).click();
    await expect(root.locator('#workflow-notes-modern-status')).toContainText('Nota eliminada.');
    await expect(note).toHaveCount(0);
    await expect(access.locator('#workflow-notes-modern-access-label')).toHaveText(/Nueva nota/);
    await expect(access.locator('#workflow-notes-modern-access-count')).toHaveText('0');
  } finally {
    if (browser.isConnected()) await context.close().catch(() => {});
  }
});
