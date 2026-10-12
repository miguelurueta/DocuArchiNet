'use strict';

const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');

const root = path.resolve(__dirname, '..');
const read = relative => fs.readFileSync(path.join(root, relative), 'utf8');

test('gestor contiene un único panel OTP accesible y recursos aislados', () => {
  const page = read('gestor.aspx');
  assert.equal((page.match(/id="docuarchi-segundo-factor-panel"/g) || []).length, 1);
  assert.match(page, /docuarchi-segundo-factor-panel[\s\S]*hidden/);
  assert.match(page, /for="docuarchi-segundo-factor-codigo"/);
  assert.match(page, /inputmode="numeric"/);
  assert.match(page, /autocomplete="one-time-code"/);
  assert.match(page, /maxlength="6"/);
  assert.match(page, /aria-live="polite"/);
  assert.match(page, /role="alert"/);
  assert.match(page, /id="docuarchi-segundo-factor-login-error"[^>]*role="alert"/);
  assert.equal((page.match(/<button id="docuarchi-segundo-factor-(?:validar|reenviar|cancelar)"/g) || []).length, 3);
  assert.equal((page.match(/js\/login\/segundo-factor\.js/g) || []).length, 1);
  assert.equal((page.match(/Styles\/login\/segundo-factor\.css/g) || []).length, 1);
});

test('code-behind ramifica solo el código público y no incrusta datos en script', () => {
  const codeBehind = read('gestor.aspx.vb');
  const branch = codeBehind.indexOf('If Result = "SECOND_FACTOR_REQUIRED" Then');
  const generic = codeBehind.indexOf('If Result <> "YES" Then');
  assert.ok(branch >= 0 && branch < generic);
  assert.match(codeBehind, /TextBoxpasw\.Text = String\.Empty/);
  assert.match(codeBehind, /window\.DocuArchiSegundoFactor\.iniciar\(\)/);
  assert.doesNotMatch(codeBehind, /DocuArchiSegundoFactor[^\r\n]*(?:TextBoxuser|Hiddenempresagestion|DropDownListmodulos)/);
});

test('cliente no usa storage, consola ni datos de identidad', () => {
  const client = read('js/login/segundo-factor.js');
  assert.doesNotMatch(client, /localStorage|sessionStorage|console\.|document\.cookie/);
  assert.doesNotMatch(client, /ChallengeId|AuthUserId|correoCompleto|contraseña/iu);
  assert.match(client, /operation === 'Verificar' \? \{ codigo:/);
  assert.match(client, /withCredentials = true/);
});

test('assets están registrados una vez y el script legacy permanece libre de 2FA', () => {
  const project = read('GestionDocumental-Docuarchi.net.vbproj');
  const legacy = read('js/workflow/gestor.js');
  assert.equal((project.match(/Content Include="js\\login\\segundo-factor\.js"/g) || []).length, 1);
  assert.equal((project.match(/Content Include="Styles\\login\\segundo-factor\.css"/g) || []).length, 1);
  assert.doesNotMatch(legacy, /SegundoFactor|SECOND_FACTOR|WebServiceLoginSegundoFactor/);
});

test('DOC-96 no introduce controles server OTP ni otro endpoint', () => {
  const designer = read('gestor.aspx.designer.vb');
  const page = read('gestor.aspx');
  assert.doesNotMatch(designer, /segundo.factor|otp/i);
  assert.doesNotMatch(page, /Login2FA\.aspx|<asp:.*segundo-factor/i);
  assert.equal(fs.existsSync(path.join(root, 'Login2FA.aspx')), false);
});
