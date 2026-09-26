const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');

const root = path.resolve(__dirname, '..');
const storageSource = fs.readFileSync(path.join(root, 'workflow/ClassAlmacenamiento.vb'), 'utf8');
const serviceSource = fs.readFileSync(path.join(root, 'webservice/WebService_integracion_sii.asmx.vb'), 'utf8');

const functionStart = storageSource.indexOf('Function PreAlmacenaDocumentoAnexosEnlaceIntegracionSII');
const functionEnd = storageSource.indexOf('Function PreAlmacenaConstanciaIsncripcionsSII', functionStart);
assert.notEqual(functionStart, -1, 'No se encontró la función legacy de anexos ENLASE');
assert.notEqual(functionEnd, -1, 'No se pudo delimitar la función legacy de anexos ENLASE');
const enlaseStorage = storageSource.slice(functionStart, functionEnd);

const methodStart = serviceSource.indexOf('Public Function SeviceGuardaDocumentoAnexoSII');
const methodEnd = serviceSource.indexOf('Public Function ServiceSolicitaArchivosAnexosrelacionadosRadicadoSII', methodStart);
assert.notEqual(methodStart, -1, 'No se encontró el WebMethod legacy de anexos ENLASE');
assert.notEqual(methodEnd, -1, 'No se pudo delimitar el WebMethod legacy de anexos ENLASE');
const enlaseWebMethod = serviceSource.slice(methodStart, methodEnd);

test('congela la firma y las salidas byref del almacenamiento ENLASE', () => {
  for (const parameter of [
    'IdTipoChekLista As Integer',
    'DescripcionTipo As String',
    'IdTipoTaramite As Integer',
    'MultiAnexos As Integer',
    'Gabinete As String',
    'CodigoBarras As String',
    'ReciboSII As String',
    'CDlistaAnexosSII As CDlistaAnexosSII',
    'NombreClaseDocumento As String',
    'ByRef IdImagenAlamacenada As Integer',
    'ByRef EstructuraDatosImagen As stru_datos_image_lista',
  ]) assert.match(enlaseStorage, new RegExp(parameter));
});

test('caracteriza autoridad y precondiciones tomadas de sesion', () => {
  assert.match(enlaseStorage, /Session\("WF_RUTAWORKFLOW"\)/);
  assert.match(enlaseStorage, /Session\("Id_Ruta_Workflow"\)/);
  assert.match(enlaseStorage, /Session\.Item\("ID_TAREA_SELECCIONDA_ENLACE"\)/);
  assert.match(enlaseStorage, /Session\.Item\("GA_IDUSUARIOGESTION"\)/);
  assert.match(enlaseStorage, /OBLIGA_LISTA_CHEQUEO = 1/);
  assert.match(enlaseStorage, /IdTipoChekLista = -1 Or IdTipoChekLista = 0/);
});

test('caracteriza descarga, metadatos y unica persistencia documental', () => {
  assert.equal((enlaseStorage.match(/DownloadFileViaRestAPI\(/g) || []).length, 1);
  assert.equal((enlaseStorage.match(/AlmacenaDocumentoTareaWorkflow\(/g) || []).length, 1);
  assert.match(enlaseStorage, /NombreCampoGabinete = "CODBARRAS"/);
  assert.match(enlaseStorage, /NombreCampoGabinete = "ENLASE"/);
  assert.match(enlaseStorage, /Case "MERCANTIL"/);
  assert.match(enlaseStorage, /Case "ESAL"/);
  assert.match(enlaseStorage, /Case "RUP"/);
  assert.match(enlaseStorage, /AcualizaIdImagenTareaWorkflow\(/);
});

test('caracteriza YES como retorno legacy y no como evidencia fisica', () => {
  assert.match(enlaseStorage, /PreAlmacenaDocumentoAnexosEnlaceIntegracionSII = "YES"/);
  assert.match(enlaseStorage, /Catch ex As Exception/);
  assert.doesNotMatch(enlaseStorage, /File\.Exists\([^\r\n]*IdImagen|VerificarDocumentoFisico|PhysicalEvidence/i);
});

test('el WebMethod traduce parametros pero expone aun el retorno textual legacy', () => {
  assert.equal((enlaseWebMethod.match(/PreAlmacenaDocumentoAnexosEnlaceIntegracionSII\(/g) || []).length, 1);
  assert.match(enlaseWebMethod, /error_gestion = ClassAlmacenamiento\.PreAlmacenaDocumentoAnexosEnlaceIntegracionSII/);
  assert.match(enlaseWebMethod, /If CCDEstadoAnexosSII\.error_gestion = "YES" Then/);
  assert.match(enlaseWebMethod, /Catch ex As Exception/);
});

test('la ruta caracterizada no asigna ni cierra la tarea', () => {
  assert.doesNotMatch(enlaseStorage + enlaseWebMethod, /Terminar_Tarea_Workflow|AsignarTarea|CerrarTarea/i);
});