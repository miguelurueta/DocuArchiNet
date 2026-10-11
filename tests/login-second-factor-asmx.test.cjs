const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const test = require('node:test');

const root = path.resolve(__dirname, '..');
const read = (...parts) => fs.readFileSync(path.join(root, ...parts), 'utf8');

test('DOC-95: descriptor y proyecto registran una vez la frontera 2FA', () => {
  const descriptor = read('webservice', 'WebServiceLoginSegundoFactor.asmx');
  assert.match(descriptor, /CodeBehind="WebServiceLoginSegundoFactor\.asmx\.vb"/);
  assert.match(descriptor, /Class="GestionDocumental_Docuarchi\.net\.WebServiceLoginSegundoFactor"/);
  const project = read('GestionDocumental-Docuarchi.net.vbproj');
  for (const include of [
    'webservice\\WebServiceLoginSegundoFactor.asmx',
    'webservice\\WebServiceLoginSegundoFactor.asmx.vb',
    'webservice\\Login\\SegundoFactor\\LoginSecondFactorSessionGate.vb',
    'webservice\\Login\\SegundoFactor\\FormsAuthenticationTicketIssuer.vb',
    'webservice\\Login\\SegundoFactor\\LegacyLoginLocalRouteResolver.vb',
    'webservice\\Login\\SegundoFactor\\LoginSecondFactorCompositionFactory.vb',
    'webservice\\Login\\SegundoFactor\\LoginSecondFactorPresentationMapper.vb'
  ]) assert.equal(project.split(`Include="${include}"`).length - 1, 1, `${include} debe registrarse una vez`);
});

test('DOC-95: ASMX expone exactamente cuatro WebMethods JSON con Session', () => {
  const source = read('webservice', 'WebServiceLoginSegundoFactor.asmx.vb');
  assert.match(source, /ScriptService\(\)/);
  const methods = [...source.matchAll(/<WebMethod\(EnableSession:=True\)>\s*<System\.Web\.Script\.Services\.ScriptMethod\(ResponseFormat:=System\.Web\.Script\.Services\.ResponseFormat\.Json\)>\s*Public Function (\w+)\(([^)]*)\) As (\w+)/g)]
    .map(match => ({ name: match[1], parameters: match[2].trim(), result: match[3] }));
  assert.deepEqual(methods, [
    { name: 'ObtenerEstado', parameters: '', result: 'SegundoFactorEstadoDto' },
    { name: 'Verificar', parameters: 'ByVal codigo As String', result: 'SegundoFactorResultadoDto' },
    { name: 'Reenviar', parameters: '', result: 'SegundoFactorResultadoDto' },
    { name: 'Cancelar', parameters: '', result: 'SegundoFactorResultadoDto' }
  ]);
  assert.doesNotMatch(source, /\bSELECT\b|\bINSERT\b|\bUPDATE\b|\bDELETE\b|SmtpClient|MailMessage|MySql|Odbc|returnUrl/i);
  for (const dependency of ['LoginSecondFactorSessionGate', 'LoginSecondFactorCompositionFactory', 'LoginSecondFactorPresentationMapper']) assert.ok(source.includes(dependency));
});

test('DOC-95: DTO JSON no publica identidad ni secretos', () => {
  const source = read('DTOs', 'Login', 'SegundoFactor', 'SegundoFactorDtos.vb');
  const properties = [...source.matchAll(/Public Property (\w+) As/g)].map(match => match[1]);
  assert.deepEqual(properties.sort(), ['Codigo', 'DestinoEnmascarado', 'Estado', 'EstadoPublico', 'Exito', 'ExpiraEnSegundos', 'MensajeVisible', 'ReenvioDisponibleEnSegundos', 'RequiereCodigo', 'RutaLocal'].sort());
  for (const forbidden of ['Challenge', 'IdUsuario', 'Login', 'Email', 'Nonce', 'Hmac', 'Hash', 'Exception', 'Sql', 'Smtp', 'ReturnUrl'])
    assert.equal(properties.some(property => property.toLowerCase().includes(forbidden.toLowerCase())), false, `Campo prohibido: ${forbidden}`);
});
