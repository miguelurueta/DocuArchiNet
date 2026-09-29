const test=require('node:test');
const assert=require('node:assert/strict');
const fs=require('node:fs');
const path=require('node:path');
const root=path.resolve(__dirname,'..');
const read=(...p)=>fs.readFileSync(path.join(root,...p),'utf8');

test('catálogo usa fuente autoritativa, ID TRD y SQL parametrizado',()=>{
  const repo=read('Infrastructure','Repositories','Workflow','ImportarServicioWeb','MySqlImportDocumentTypeCatalogRepository.vb');
  assert.match(repo,/FROM ra_dig_tipos_docum_lista_chequeo rdt/);
  assert.match(repo,/INNER JOIN tipo_doc_series tds/);
  assert.match(repo,/tipo_doc_entrante_id_Tipo_Doc_Entrante=@procedureId/);
  assert.match(repo,/tipo_doc_series_Id_Tipo_Doc_Series AS document_type_id/);
  assert.match(repo,/OBLIGATORIO/); assert.match(repo,/ORDEN_LISTA/);
  assert.match(repo,/DOCUMENT_TYPE_CATALOG_AMBIGUOUS/);
  assert.match(repo,/Function RequiereSeleccion/);
  assert.match(repo,/MAX\(OBLIGA_LISTA_CHEQUEO\)/);
  assert.match(repo,/RA_DIG_CONFIG_DIGITALIZACION/);
  assert.doesNotMatch(repo,/@procedureId\s*"\s*&\s*contexto/i);
});

test('tipología opcional se propaga sin inventar una clasificación',()=>{
  const dto=read('DTOs','Workflow','ImportarServicioWeb','ImportarServicioWebDtos.vb');
  const presentation=read('Services','Workflow','ImportarServicioWeb','ImportItemPresentationService.vb');
  const preflight=read('Services','Workflow','ImportarServicioWeb','ServicioPreflightImportacion.vb');
  const storage=read('Services','Workflow','ImportarServicioWeb','ImportExecutionSteps.vb');
  const adapter=read('Infrastructure','Workflow','ImportarServicioWeb','Storage','LegacyEnlaseImportDocumentStorageAdapter.vb');
  const ui=read('js','workflow','importar-servicio-web','importar-servicio-web-ui.js');
  assert.match(dto,/Property DocumentTypeRequired As Boolean/);
  assert.match(presentation,/response\.DocumentTypeRequired = _catalog\.RequiereSeleccion\(context\)/);
  assert.match(preflight,/DOCUMENT_TYPE_NOT_REQUIRED/);
  assert.match(preflight,/If\(item\.DocumentTypeId\.HasValue/);
  assert.match(storage,/documentTypeRequired = _catalog\.RequiereSeleccion\(contexto\)/);
  assert.match(adapter,/Not comando\.DocumentTypeRequired/);
  assert.match(ui,/Este trámite permite importar sin tipología documental/);
});

test('capabilities enriquece sólo después de validar contexto',()=>{
  const service=read('webservice','WebServiceImportarServicioWebModern.asmx.vb');
  const block=service.slice(service.indexOf('Public Function ResolveCapabilities'),service.indexOf('Public Function QueryItems'));
  assert.ok(block.indexOf('TryBuildImportContext')<block.indexOf('EnrichCapabilities'));
  assert.match(block,/EnrichCapabilities\(importContext,response\)/);
});
