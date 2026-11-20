'use strict';
const test=require('node:test'),assert=require('node:assert/strict');
const api=require('../tools/e2e/scripts/inspect-doc120-egresos-length.cjs');
function fixture(capacity=60){return[
 [{owner_ref:'141',company_ref:'2'}],
 [{id_gabinete:'10',tabla:'CONTABIL',columna:'BENEFICIARIO',DATA_TYPE:'varchar',CHARACTER_MAXIMUM_LENGTH:String(capacity)}],
 [{perfil_id:'26',perfil_revision:'2',version_publicada_id:'40',numero:'1',version_revision:'3',configuracion_doc:JSON.stringify({GabineteId:10})}],
 [{regla_id:'17',regla_doc:JSON.stringify({DestinoTipo:'INDICE_GABINETE',DestinoRef:'17',TipoValor:'TEXTO',LongitudMaxima:60,Proveedor:{ProveedorId:'3',VersionId:'7',Propiedad:'Beneficiario'}})}],
 [{conexion_ref:'DA',tabla_nombre:'imp_02_egresos',propiedades_doc:JSON.stringify([{Nombre:'Beneficiario',Columna:'BENEFICIARIO'}])}],
 [{DATA_TYPE:'varchar',CHARACTER_MAXIMUM_LENGTH:'250'}],
 [{registros:'38000',longitud_maxima:'63',longitud_maxima_sin_espacios:'63',exceden_regla:'8'}],
 [{COLUMN_NAME:'CAMPO'},{COLUMN_NAME:'TIPO'}],[]
];}
test('DOC120-LENGTH01: mide toda la fuente y distingue regla, columna y contrato de origen',async()=>{
 const data=fixture(),calls=[];const result=await api.inspect(async(origin,sql,parameters)=>{calls.push({origin,sql,parameters});return data[calls.length-1];},'synthetic-owner');
 assert.equal(calls.length,9);assert.equal(result.source.maximum,63);assert.equal(result.source.declaredCapacity,250);assert.equal(result.ruleLimit,60);assert.equal(result.physicalCapacity,60);assert.equal(result.profileOnlyPossible,false);assert.equal(result.source.exceedingCurrentRule,8);
 for(const call of calls){assert.match(call.sql,/^SELECT /);assert.doesNotMatch(call.sql,/;|\b(?:INSERT|UPDATE|DELETE|ALTER|CREATE|DROP|TRUNCATE|INTO|LOCK)\b/i);}
 assert.match(calls[6].sql,/MAX\(CHAR_LENGTH\(`BENEFICIARIO`\)\)/);assert.doesNotMatch(calls[6].sql,/LIMIT/);assert.deepEqual(calls[6].parameters,['60']);
});
test('DOC120-LENGTH02: una columna amplia permite cambiar solo la regla del perfil',async()=>{
 const data=fixture(250);let i=0;const result=await api.inspect(async()=>data[i++],'synthetic-owner');assert.equal(result.profileOnlyPossible,true);assert.equal(result.requiredForExistingData,63);
});
test('DOC120-LENGTH03: origen o columna ajenos detienen antes de consultar registros',async()=>{
 for(const mode of ['table','column','connection']){const data=fixture();if(mode==='table')data[4][0].tabla_nombre='other';if(mode==='connection')data[4][0].conexion_ref='RA';if(mode==='column')data[4][0].propiedades_doc=JSON.stringify([{Nombre:'Beneficiario',Columna:'BENEFICIARIO;DELETE'}]);let i=0;await assert.rejects(api.inspect(async()=>data[i++],'synthetic-owner'),/SCOPE_CHANGED/);assert.equal(i,5);}
});
test('DOC120-LENGTH04: perfil ambiguo o gabinete distinto no selecciona otro por defecto',async()=>{
 for(const mode of ['duplicate','cabinet']){const data=fixture();if(mode==='duplicate')data[2].push({...data[2][0]});else data[2][0].configuracion_doc=JSON.stringify({GabineteId:11});let i=0;await assert.rejects(api.inspect(async()=>data[i++],'synthetic-owner'),/SCOPE_CHANGED/);assert.equal(i,3);}
});
test('DOC120-LENGTH05: error de lectura se conserva cerrado sin reintentar ni revelar mensaje',async()=>{
 let reads=0;await assert.rejects(api.inspect(async()=>{reads++;throw Error('DOC111_SELECT_AUTHENTICATION_FAILED');},'synthetic-owner'),/DOC111_SELECT/);assert.equal(reads,1);assert.equal(api.errorCode(Error('DOC111_SELECT_AUTHENTICATION_FAILED')),'DOC120_LENGTH_DB_AUTHENTICATION_FAILED');assert.equal(api.errorCode(Error('password=private')),'DOC120_LENGTH_STOPPED');
});
