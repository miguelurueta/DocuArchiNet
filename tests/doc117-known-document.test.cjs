'use strict';
const {test}=require('node:test'),assert=require('node:assert/strict');
const {createSelectControls}=require('../tools/e2e/tests/support/doc111-importacion-carpetas.cjs');
function fixture(failure){
 const config={GabineteId:10},ctx={UsuarioRef:'141',AmbitoRef:'2',NivelRef:'7'},identity={OrigenLogico:'RA',NivelRef:'7',ExpedienteId:'664',RelacionNivelId:'1089'},expected={codigo:'LOCAL',nombre:'LOCAL',tipologiaId:'79',indices:[{referencia:'14',tipo:'TEXTO',valor:'EEE'}]};
 const view={lot:{config_doc:JSON.stringify({VersionId:'38',Configuracion:{...config,ModoIntegridad:'COMPLETO_OBLIGATORIO',AmbitoIntegridad:'LOTE'}}),contexto_doc:JSON.stringify(ctx),estado:'EN_CURSO',total_raices:2,total_archivos:2},roots:[{raiz_id:'33',identidad_expediente_doc:JSON.stringify(identity)}],files:[],attempts:[{raiz_id:'33',fase:'CREAR_EXPEDIENTE',estado:'CONFIRMADO',fin_utc:'2026-11-06',evidencia_doc:JSON.stringify({Creacion:{CicloArchivoId:'1'},Expediente:identity})}]};
 const query=async(origin,sql,args)=>{
  assert.match(sql,/^SELECT\b/);assert.ok(!sql.includes(';'));
  if(sql.startsWith('SELECT lote_id,config_doc'))return[view.lot];if(sql.startsWith('SELECT raiz_id,'))return view.roots;if(sql.startsWith('SELECT f.archivo_id,'))return view.files;if(sql.startsWith('SELECT intento_id,'))return view.attempts;if(sql.startsWith('SELECT evento_id,'))return[];
  if(sql.includes('SELECT NOMBRE FROM system1'))return[{NOMBRE:'synthetic_cabinet'}];
  if(sql.includes('Nombre_Tipo_ciclo_archivo'))return[{id:'1',nombre:'Archivo Gestión'}];
  if(sql.includes('SELECT e.*'))return[{nivel:'7',ID_USUARIO_GESTION:'141',ID_EMPRESA_EXPEDIENTE:'2',CODIGO_UNICO:'LOCAL',ALEAS_EXPEDIENTE:'LOCAL',TEMA_EXPEDIENTE:'LOCAL',Id_tipos_ciclo_archivo:1,NOMBRE_CICLO_ARCHIVO:'Archivo Gestión'}];
  if(sql.includes('FROM ra_tipo_documento'))return[{id:'3'}];if(sql.includes('FROM DA_EXTENSION'))return[{id:'4'}];
  if(sql.includes('SELECT CAMPO'))return[{CAMPO:'code_field'}];if(sql.includes('SELECT VISIBLE'))return[{VISIBLE:'0'}];
  if(sql.includes('SELECT g.ID')){assert.deepEqual(args,['synthetic_cabinet','990','992']);return failure==='relation'?[]:[{ID:'990',ID_EXPEDIENTE:'664',ID_USUARIO_GESTION:'141',NOMBRE:null,DBT:4,ID_CLASE_DOCUMENTO:3,ID_TIPO_DOCUMENTO:79,ID_EMPRESA_DOCUMENTO:2,inventory_user:141,SEGUNDO_NOMBRE_DOCUMENTO:failure==='name'?'other.pdf':'fixture.pdf',inventory_format:'.PDF',segundo_nombre:'fixture.pdf',index_format:'.PDF',indexed_expedition:'664',value_0:failure==='index'?'OTHER':'EEE'}];}
  if(sql.includes('COUNT(*)'))return[{total:failure==='duplicate'?2:1}];assert.fail('Unexpected query');
 };
 return{view,controls:createSelectControls(query),expected,identity};
}
test('Known-ID physical verification accepts existing uncertain effect without changing operational state',async()=>{
 const f=fixture(),before=JSON.stringify(f.view),r=await f.controls.verifyKnownDocument(f.view,'33',{DocumentoId:'990',InventarioId:'992'},f.expected,'fixture.pdf');assert.equal(r.knownIdsVerified,true);assert.equal(JSON.stringify(f.view),before);assert.equal(f.view.lot.estado,'EN_CURSO');
});
test('Existing completed-lot verifier still rejects an uncertain operational state',async()=>{
 const f=fixture();await assert.rejects(f.controls.verify('900',{versionId:'38',nivelRef:'7',mode:'LOTE',expectations:[f.expected,f.expected]}),/FINAL_PROOF_INVALID/);
});
for(const failure of ['relation','name','index','duplicate'])test('Known-ID physical verification rejects '+failure,async()=>{
 const f=fixture(failure);await assert.rejects(f.controls.verifyKnownDocument(f.view,'33',{DocumentoId:'990',InventarioId:'992'},f.expected,'fixture.pdf'));
});
