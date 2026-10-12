const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const test = require('node:test');

const root = path.resolve(__dirname, '..');
const manifestPath = path.join(__dirname, 'Fixtures', 'Login', 'SegundoFactor', 'doc97-regression-manifest.json');
const manifest = JSON.parse(fs.readFileSync(manifestPath, 'utf8'));
const read = relative => fs.readFileSync(path.join(root, ...relative.split('/')), 'utf8');

test('DOC-97: el manifiesto resuelve el flujo real y sus símbolos', () => {
  for (const reference of manifest.flow) {
    const source = read(reference.path);
    assert.ok(source.includes(reference.symbol), `${reference.path}: símbolo ausente ${reference.symbol}`);
  }
});

test('DOC-97: la matriz determinista enumera los cuatro módulos', () => {
  assert.deepEqual(manifest.modules, [
    'DOCUARCHI CONTENEDOR', 'GESTOR DOCUMENTAL', 'RADICACION DOCUMENTAL', 'WORKFLOW DOCUMENTAL'
  ]);
  const preauth = read('tests/LoginSecondFactorPreAuthenticationBehaviorTests.cs');
  for (const moduleType of manifest.modules) assert.ok(preauth.includes(`"${moduleType}"`), `falta módulo ${moduleType}`);
  for (const method of manifest.behaviorEvidence.slice(0, 3)) assert.match(preauth, new RegExp(`\\b${method}\\s*\\(`), `falta evidencia ${method}`);
});

test('DOC-97: límites, concurrencia, replay y sesión tienen evidencia conductual', () => {
  const authentication = read('tests/LoginSecondFactorAuthenticationBehaviorTests.cs');
  for (const method of manifest.behaviorEvidence.slice(3)) assert.match(authentication, new RegExp(`\\b${method}\\s*\\(`), `falta evidencia ${method}`);
  assert.match(authentication, /different-session-binding/);
  assert.match(authentication, /identidades cruzadas no finalizan/);
  assert.match(authentication, /challenge consumido no admite replay/);
});

test('DOC-97: secretos, persistencia mínima y ruta local permanecen cerrados', () => {
  const dto = read('DTOs/Login/SegundoFactor/SegundoFactorDtos.vb');
  assert.doesNotMatch(dto, /ChallengeId|Nonce|EmailAddress|NormalizedLogin|InternalUserId|Password|Connection/i);
  const repository = read('Infrastructure/Repositories/Login/SegundoFactor/MySqlSecondFactorChallengeRepository.vb');
  assert.match(repository, /AuthPayloadJson\) VALUES \([^\r\n]+,NULL\)/i);
  const hmac = read('Infrastructure/Login/SegundoFactor/Security/HmacSecondFactorCodeProtector.vb');
  assert.match(hmac, /Private Shared Function FixedTimeEquals/);
  assert.match(hmac, /difference = difference Or \(expected\(index\) Xor supplied\(index\)\)/);
  const route = read('webservice/Login/SegundoFactor/LegacyLoginLocalRouteResolver.vb');
  assert.match(route, /~\/Defaul\/WebFormInicioDocuarchiGestion\.aspx/);
});

test('DOC-97: orden seguro y fallos cerrados conservan evidencia ejecutable', () => {
  const behavior = read('tests/LoginSecondFactorAuthenticationBehaviorTests.cs');
  for (const evidence of [
    'CreationConfirmationAndFinalizationFailuresNeverIssueTicket',
    'ProtectionFailureDoesNotConsumeAnOtpAttempt',
    'FifthAttemptBlocksAndConcurrentVerifyHasOneWinner'
  ]) assert.match(behavior, new RegExp(`\\b${evidence}\\s*\\(`));
  assert.match(behavior, /Equal\("complete,ticket", String\.Join\(",", h\.Order\), "orden de cierre"\)/);
});

test('DOC-97: correo legacy, recuperación y registros públicos son invariantes', () => {
  const legacy = read('Defaul/ClassGestorSesion.vb');
  assert.match(legacy, /Dim refclas_correo As New ClassCorreo/);
  assert.match(read('gestor.aspx.vb'), /LinkButton_recupera_pw_Click/);
  const project = read('GestionDocumental-Docuarchi.net.vbproj');
  for (const reference of manifest.flow.filter(item => /SegundoFactor|segundo-factor/.test(item.path))) {
    const windowsPath = reference.path.replaceAll('/', '\\');
    assert.equal(project.split(`Include="${windowsPath}"`).length - 1, 1, `${windowsPath} debe registrarse una vez`);
  }
});
