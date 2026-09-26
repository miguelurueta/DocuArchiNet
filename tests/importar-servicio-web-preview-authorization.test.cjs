const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');

const descriptor = fs.readFileSync('Infrastructure/Workflow/ImportarServicioWeb/Preview/ImportPreviewDescriptorService.vb', 'utf8');
const repository = fs.readFileSync('Infrastructure/Workflow/ImportarServicioWeb/Preview/ImportPreviewDescriptorRepository.vb', 'utf8');
const handler = fs.readFileSync('workflow/ImportarServicioWebPreview.ashx.vb', 'utf8');

test('descriptor es CSPRNG opaco y solo persiste SHA-256', () => {
  assert.match(descriptor, /RandomNumberGenerator\.Create/);
  assert.match(descriptor, /Dim tokenBytes\(31\) As Byte/);
  assert.match(descriptor, /SHA256\.Create/);
  assert.match(descriptor, /String\.Equals\(ToBase64Url\(result\), canonical, StringComparison\.Ordinal\)/);
  assert.doesNotMatch(repository, /descriptor_id|external_key/i);
  assert.match(repository, /descriptor_hash=@hash/);
});

test('autoridad se liga a usuario tarea y proveedor en HEAD y GET', () => {
  for (const field of ['user_id=@userId', 'task_id=@taskId', 'provider_id=@providerId']) {
    assert.ok(repository.includes(field), field);
  }
  assert.match(handler, /AsegurarContexto\(\)/);
  assert.match(handler, /TryResolveTrustedTaskId\(context, taskId\)/);
  assert.match(handler, /SiiImportProvider\.CanonicalProviderId/);
});


test('preview ENLASE usa su tarea autoritativa y no cae a la selección estándar', () => {
  const resolver = handler.match(/Private Shared Function TryResolveTrustedTaskId[\s\S]*?End Function/)[0];
  assert.match(resolver, /SELECCIONTEMPORAL/);
  assert.match(resolver, /selection\.Length >= 4/);
  assert.match(resolver, /String\.Equals\(selection\(3\)\.Trim\(\), "ENLASE", StringComparison\.OrdinalIgnoreCase\)/);
  assert.match(resolver, /ID_TAREA_SELECCIONDA_ENLACE/);
  assert.match(resolver, /Long\.TryParse\(selection\(0\), selectionTaskId\) AndAlso selectionTaskId = taskId/);
  const enlaseBranch = resolver.match(/If isEnlase Then[\s\S]*?End If/)[0];
  assert.doesNotMatch(enlaseBranch, /ID_TAREA_SELECCIONDA"/);
  assert.match(resolver, /ID_TAREA_SELECCIONDA"/);
});
test('ausente alterado vencido ajeno y consumido comparten rechazo opaco', () => {
  assert.match(handler, /Reject\(context, 404\)/);
  assert.match(repository, /status='Disponible' AND expires_utc>@now/);
  assert.doesNotMatch(handler, /Write\([^\n]*(exception|descriptor|token)/i);
});
