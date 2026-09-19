const test=require('node:test');
const assert=require('node:assert/strict');
const fs=require('node:fs');
const path=require('node:path');
const root=path.resolve(__dirname,'..');
const read=(...p)=>fs.readFileSync(path.join(root,...p),'utf8');

test('DOC-68 publica contratos aditivos 1.1 sin retirar campos 1.0',()=>{
  const dto=read('DTOs','Workflow','ImportarServicioWeb','ImportarServicioWebDtos.vb');
  for(const field of ['ExternalKey','DisplayName','ContentType','Length','PreviewAvailable']) assert.match(dto,new RegExp(`Property ${field} `));
  for(const symbol of ['ImportDocumentTypeDto','ImportItemMetadataDto','DocumentTypes','Metadata','ImportStatus','AllowedActions']) assert.match(dto,new RegExp(symbol));
  assert.match(dto,/PresentationSchemaVersion As String/);
});

test('mapper obtiene presentación de la misma respuesta y excluye datos sensibles',()=>{
  const mapper=read('Infrastructure','Workflow','ImportarServicioWeb','Sii','SiiImportContractMapper.vb');
  for(const code of ['book','registration','date','act','news','reference']) assert.ok(mapper.includes(`AddMetadata(item, "${code}"`));
  const block=mapper.slice(mapper.indexOf('Public Function MapQuery'),mapper.indexOf('Public Function MapPreview'));
  assert.doesNotMatch(block,/QuerySeal|QueryItemsAsync|GetPreview|Download|Value\([^\r\n]+"(?:matricula|identificacion|url|token)"/i);
});

test('fixtures DOC-68 cubren cero y múltiples elementos sin datos prohibidos',()=>{
  const dir=path.join(root,'Tests','Fixtures','Workflow','ImportarServicioWeb','sii-v1');
  for(const name of ['query-provider-empty-response.json','query-provider-multiple-response.json','query-provider-no-seal-response.json','query-provider-duplicate-seal-response.json']) {
    const raw=fs.readFileSync(path.join(dir,name),'utf8'); JSON.parse(raw);
    assert.doesNotMatch(raw,/https?:|token|contrase|900000|matricula|identificacion/i);
  }
});

test('DOC-68 solo presenta sellos de inscripción tipoanexo 505',()=>{
  const mapper=read('Infrastructure','Workflow','ImportarServicioWeb','Sii','SiiImportContractMapper.vb');
  const fixture=JSON.parse(fs.readFileSync(path.join(root,'Tests','Fixtures','Workflow','ImportarServicioWeb','sii-v1','query-provider-multiple-response.json'),'utf8'));
  const attachments=fixture.inscripciones.flatMap(value=>value.imagenes);
  assert.equal(attachments.filter(value=>value.tipoanexo==='505').length,2);
  assert.equal(attachments.filter(value=>value.tipoanexo==='518').length,1);
  assert.match(mapper,/IsInscriptionAttachment\(image\)/);
  assert.match(mapper,/String\.Equals\(Value\(image, "tipoanexo"\), "505", StringComparison\.Ordinal\)/);
  assert.match(mapper,/sealImages\.Count = 0 Then Throw New InvalidOperationException\("SII_INSCRIPTION_SEAL_MISSING"\)/);
  assert.match(mapper,/sealImages\.Count > 1 Then Throw New InvalidOperationException\("SII_INSCRIPTION_SEAL_AMBIGUOUS"\)/);
});
