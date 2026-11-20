'use strict';
const test=require('node:test'),assert=require('node:assert/strict');
const api=require('../tools/e2e/scripts/apply-doc120-egresos-length.cjs');
const measurement={profileId:'28',companyRef:'2',versionId:'43',profileRevision:2,versionRevision:3};
function profile(){return{PerfilId:'28',AmbitoRef:'2',Nombre:'Egresos',Activo:true,Revision:2,VersionPublicadaId:'43',Version:{VersionId:'43',Numero:1,Revision:3,Estado:'PUBLICADA',Configuracion:{GabineteId:10,NombreVisible:'Egresos',PoliticaCoincidencias:'BLOQUEAR'},Reglas:[{ReglaId:'1',DestinoTipo:'CAMPO_EXPEDIENTE',DestinoRef:'NOMBRE',ValorConstante:'Same'},
 {ReglaId:'2',DestinoTipo:'INDICE_GABINETE',DestinoRef:'17',TipoValor:'TEXTO',LongitudMaxima:60,Proveedor:{ProveedorId:'2',VersionId:'3',Propiedad:'Beneficiario'}}]}};}
function fixture(mode){let original=profile(),current=structuredClone(original);const calls=[];return{calls,original,call:async(op,input)=>{calls.push({op,input:structuredClone(input)});
 if(op==='ObtenerPerfil')return structuredClone(input.versionId==='43'?{...original,Revision:current.Revision,VersionPublicadaId:current.VersionPublicadaId}:current);
 if(op==='CrearVersion'){current=structuredClone(original);Object.assign(current,{Revision:3});Object.assign(current.Version,{VersionId:'44',Numero:2,Revision:1,Estado:'BORRADOR'});current.Version.Reglas.forEach(r=>r.ReglaId+='1');}
 if(op==='GuardarBorrador'){current.Version.Configuracion=input.configuracion;current.Version.Reglas=input.reglas;current.Version.Revision++;if(mode==='unrelated')current.Version.Configuracion.NombreVisible='Changed';}
 if(op==='PublicarVersion'){if(mode==='loss')throw Error('TRANSPORT_UNCONFIRMED');current.Version.Estado='PUBLICADA';current.Version.Revision++;current.VersionPublicadaId='44';current.Revision++;}
 return structuredClone(current);
 }};}
test('DOC120-LENCHANGE01: una version nueva cambia solo longitud17 y conserva publicacion original',async()=>{
 const f=fixture();const prepared=await api.changeProfile(f.call,measurement);assert.equal(f.calls.length,1);const result=await prepared.apply();
 assert.deepEqual(f.calls.map(c=>c.op),['ObtenerPerfil','CrearVersion','GuardarBorrador','PublicarVersion','ObtenerPerfil','ObtenerPerfil']);assert.equal(result.versionId,'44');assert.equal(result.newLimit,250);assert.equal(result.originalPublicationUnchanged,true);
 const payload=f.calls.find(c=>c.op==='GuardarBorrador').input;assert.equal(payload.reglas[1].LongitudMaxima,250);assert.equal(payload.configuracion.PoliticaCoincidencias,'BLOQUEAR');assert.equal(f.original.Version.Reglas[1].LongitudMaxima,60);
});
test('DOC120-LENCHANGE02: perdida de respuesta al publicar no repite ninguna mutacion',async()=>{
 const f=fixture('loss'),prepared=await api.changeProfile(f.call,measurement);await assert.rejects(prepared.apply(),/TRANSPORT_UNCONFIRMED/);assert.equal(f.calls.filter(c=>c.op==='PublicarVersion').length,1);assert.equal(f.calls.length,4);
});
test('DOC120-LENCHANGE03: cambio no solicitado impide publicar el borrador',async()=>{
 const f=fixture('unrelated'),prepared=await api.changeProfile(f.call,measurement);await assert.rejects(prepared.apply(),/UNRELATED_PROFILE_CHANGE/);assert.equal(f.calls.filter(c=>c.op==='PublicarVersion').length,0);
});
test('DOC120-LENCHANGE04: version vigente distinta o empresa ajena se rechaza antes de crear',async()=>{
 for(const patch of [{VersionPublicadaId:'45'},{AmbitoRef:'3'},{Nombre:'Otro'}]){const f=fixture();const call=async(op,input)=>Object.assign(await f.call(op,input),patch);await assert.rejects(api.changeProfile(call,measurement),/PUBLICATION_CHANGED|SCOPE_CHANGED/);assert.equal(f.calls.length,1);}
});
test('DOC120-LENCHANGE05: regla duplicada, limite inesperado o token no valido no se adivinan',()=>{
 const p=profile();p.Version.Reglas.push({...p.Version.Reglas[1]});assert.throws(()=>api.updatePayload(p,250),/RULE_CHANGED/);assert.throws(()=>api.updatePayload(profile(),251),/LIMIT_INVALID/);
 assert.throws(()=>api.csrfFromHtml('<div id="doc110-perfiles" data-token="bad"></div>'),/ADMIN_CONTROL_UNAVAILABLE/);assert.equal(api.csrfFromHtml('<div id="doc110-perfiles" data-token="'+'a'.repeat(43)+'="></div>'),'a'.repeat(43)+'=');
});
test('DOC120-LENCHANGE06: el auxiliar nativo rechaza alcance invalido antes de abrir ODBC',{skip:process.platform!=='win32'},async()=>{
 const result=await api.physicalChange({authorization:'INVALID',dsn:'Synthetic',user:'SECRET_USER',password:'SECRET_PASSWORD'});
 assert.equal(result.status,'STOPPED');assert.equal(result.failure,'INPUT_INVALID');assert.equal(result.select,0);assert.equal(result.alterAttempted,false);assert.equal(result.metadataUpdateAttempted,false);assert.equal(result.credentialsCleared,true);assert.doesNotMatch(JSON.stringify(result),/SECRET/);
});
test('DOC120-LENCHANGE07: fuente opcional y dashboard se omiten sin abrir recursos externos',()=>{
 const req=(url,method='GET',type='document',data=null)=>({url:()=>url,method:()=>method,resourceType:()=>type,postData:()=>data});
 assert.equal(api.loginRequestAction(req('https://fonts.googleapis.com/css?family=Open+Sans:300,400,400i,600,700,800','GET','stylesheet')),'OMIT_FONT');
 assert.equal(api.loginRequestAction(req('https://localhost/GestionDocumental-Docuarchi.net/defaul/webforminiciodocuarchigestion.aspx')),'OMIT_LANDING');
 assert.equal(api.loginRequestAction(req('https://example.com/anything')),'DENY');
 assert.equal(api.loginRequestAction(req('https://localhost/another-app/file.js')),'DENY');
});
test('DOC120-LENCHANGE08: solo login y consulta vacia del segundo factor permiten POST del navegador',()=>{
 const base='https://localhost/GestionDocumental-Docuarchi.net/',req=(p,data='{}',method='POST')=>({url:()=>base+p,method:()=>method,resourceType:()=>'xhr',postData:()=>data});
 assert.equal(api.loginRequestAction(req('gestor.aspx')),'ALLOW');
 assert.equal(api.loginRequestAction(req('webservice/webserviceloginsegundofactor.asmx/obtenerestado')),'ALLOW');
 for(const r of [req('webservice/webserviceloginsegundofactor.asmx/obtenerestado','{"extra":1}'),req('webservice/WebServiceImportacionCarpetas.asmx/ConfirmarPlan'),req('generic_control/ImportacionCarpetasUpload.ashx','{}','GET')])assert.equal(api.loginRequestAction(r),'DENY');
});
test('DOC120-LENCHANGE09: limite cambiado se detecta en preflight y revisiones observadas quedan disponibles',async()=>{
 const f=fixture(),steps=[];const call=async(op,input)=>{const value=await f.call(op,input);value.Version.Reglas[1].LongitudMaxima=100;return value;};
 await assert.rejects(api.changeProfile(call,measurement,(name,p)=>steps.push({name,revision:p.Revision})),/RULE_CHANGED/);
 assert.deepEqual(steps,[{name:'PROFILE_OBSERVED',revision:2}]);assert.equal(f.calls.length,1);
});
