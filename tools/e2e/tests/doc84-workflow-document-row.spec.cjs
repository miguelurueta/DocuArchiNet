const path = require('node:path');
const { test, expect } = require('@playwright/test');

const repositoryRoot = path.resolve(__dirname, '..', '..', '..');
const legacyGridScript = path.join(repositoryRoot, 'js', 'java_general', 'GredviewControl.js');
const workflowAppenderScript = path.join(
  repositoryRoot,
  'js',
  'workflow',
  'importar-servicio-web',
  'importar-servicio-web-workflow-document-list-adapter.js'
);

test('DOC-84 proyecta una fila Workflow real con idd_wf y todas sus acciones operables', async ({ page }) => {
  const browserRequests = [];
  page.on('request', request => browserRequests.push(request.url()));

  await page.setContent(`
    <!doctype html>
    <html lang="es">
      <body>
        <span id="Label_docu_relacionado_wf">Documentos 0</span>
        <input id="Hidden_numero_doc_rel_wf" value="0">
        <table id="GridView_list_documento_relacion_wf">
          <tbody><tr><th>Seleccionar</th><th>Documento</th></tr></tbody>
        </table>
      </body>
    </html>
  `);

  await page.evaluate(() => {
    window.__doc84Actions = [];
    window.prevent = (event, element) => {
      event.preventDefault();
      window.__doc84Actions.push({
        type: element.getAttribute('tip_event'),
        documentId: element.getAttribute('id_wf'),
        contract: element.getAttribute('idd_wf')
      });
    };
    window.alert = message => { throw new Error(message); };
  });

  await page.addScriptTag({ path: legacyGridScript });
  await page.addScriptTag({ path: workflowAppenderScript });

  const projection = {
    cabinetName: 'MERCANTIL',
    documentId: 93,
    radicado: 'S002469800',
    storageType: 'PDF',
    documentTypeName: 'Constancia <de> Inscripción|SII',
    taskId: 220586,
    signatureStatus: 0,
    iconClass: 'fa-file-pdf'
  };

  const appended = await page.evaluate(item => {
    const append = window.ImportarServicioWebWorkflowDocumentListAdapter.create({
      document: window.document,
      currentTaskId: () => 220586,
      insertRow: (data, destination, versioned) => {
        window.__doc84Insert = { data, destination, versioned };
        window.insert_row_documento_relacionado(data, destination, versioned);
      }
    });
    return append({ documentId: item.documentId, taskId: item.taskId, workflowProjection: item });
  }, projection);

  expect(appended).toBe(true);
  await expect(page.locator('#GridView_list_documento_relacion_wf tr.GridviewRow')).toHaveCount(1);

  const evidence = await page.evaluate(() => {
    const row = document.querySelector('#GridView_list_documento_relacion_wf tr.GridviewRow');
    const controls = Array.from(row.querySelectorAll('[tip_event]'));
    return {
      insert: window.__doc84Insert,
      rowId: row.getAttribute('id_wf'),
      rowContract: row.getAttribute('idd_wf'),
      visibleText: row.textContent.replace(/\s+/g, ' ').trim(),
      iconClass: row.querySelector('.f_d_v_d_a_93 i').getAttribute('class'),
      controls: controls.map(control => ({
        type: control.getAttribute('tip_event'),
        documentId: control.getAttribute('id_wf'),
        contract: control.getAttribute('idd_wf')
      }))
    };
  });

  const expectedContract = 'MERCANTIL|93|S002469800|PDF|Constancia &lt;de&gt; InscripciónSII|220586|0|fa-file-pdf';
  expect(evidence.insert).toEqual({ data: expectedContract, destination: 'wf', versioned: 1 });
  expect(evidence.rowId).toBe('93');
  expect(evidence.rowContract).toBe(expectedContract);
  expect(evidence.visibleText).toContain('Constancia <de> InscripciónSII');
  expect(evidence.iconClass).toContain('fa-file-pdf');
  expect(evidence.controls.map(control => control.type)).toEqual([
    'vis_doc_selecion_wf',
    'elim_doc_selecion_wf',
    'cambia_doc_selecion_wf',
    'firma_doc_selecion_wf',
    'lista_ver_doc_selecion_wf',
    'remplaza_ver_doc_selecion_wf'
  ]);

  const expectedDomFields = 'MERCANTIL|93|S002469800|PDF|Constancia <de> InscripciónSII|220586|0|fa-file-pdf'.split('|');
  for (const control of evidence.controls) {
    expect(control.documentId.trim()).toBe('93');
    const fields = control.contract.trim().split('|');
    expect(fields.slice(0, 8)).toEqual(expectedDomFields);
    if (control.type === 'firma_doc_selecion_wf' || control.type === 'remplaza_ver_doc_selecion_wf') {
      expect(fields[8]).toBe('f_d_v_d_a_93');
    } else {
      expect(fields).toHaveLength(8);
    }
  }

  for (const action of evidence.controls.map(control => control.type)) {
    await page.locator(`[tip_event="${action}"]`).click();
  }
  expect(await page.evaluate(() => window.__doc84Actions.map(action => action.type))).toEqual(
    evidence.controls.map(control => control.type)
  );

  expect(await page.locator('#Hidden_numero_doc_rel_wf').inputValue()).toBe('1');
  await expect(page.locator('#Label_docu_relacionado_wf')).toHaveText('Documentos 1');
  expect(browserRequests).toEqual([]);
});
