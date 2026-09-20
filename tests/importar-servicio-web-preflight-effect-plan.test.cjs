const test=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),path=require('node:path');
const root=path.resolve(__dirname,'..');
const dto=fs.readFileSync(path.join(root,'DTOs/Workflow/ImportarServicioWeb/ImportarServicioWebDtos.vb'),'utf8');
const builder=fs.readFileSync(path.join(root,'Services/Workflow/ImportarServicioWeb/ImportEffectPlanBuilder.vb'),'utf8');
test('DOC-70 publica un plan aditivo por item sin identidad física',()=>{
  for(const field of ['ClientItemId','TargetTaskId','DocumentTypeId','DocumentTypeName','DestinationMode','ExpedientRequired','Effects','Requirements']) assert.match(dto,new RegExp(`Property ${field}`));
  assert.match(dto,/Property Executable As Boolean/); assert.match(dto,/Property EffectPlans As IList\(Of ImportEffectPlanDto\)/);
  assert.doesNotMatch(dto.slice(dto.indexOf('Public Class ImportEffectPlanDto'),dto.indexOf('Public Class ImportItemResultDto')),/ExpedientId|Cabinet|TableName|Sql|PhysicalPath/);
  for(const code of ['DOCUMENT_STORAGE','EXPEDIENT_RESOLUTION','DOCUMENT_LINK','LINK_CACHE','DOCUMENT_INDEXES']) assert.match(builder,new RegExp(code));
});
test('builder produce modos Single y Multiple y estado Planned',()=>{
  assert.match(builder,/If\(configuration\.MultipleExpedients, "Multiple", "Single"\)/);
  assert.match(builder,/\.Status = "Planned"/);
});
