'use strict';
const crypto=require('node:crypto');
const check=(v,c)=>{if(!v)throw Error('DOC118_RESPONSE_'+c);};
const id=v=>typeof v==='string'&&/^[1-9][0-9]{0,9}$/.test(v);
function suppressFirstResponse({auth,profile,command,resourceSha256,checkpoint}){
 check(command?.fase==='ALMACENAR'&&!['18','21'].includes(command.loteId)&&crypto.createHash('sha256').update(JSON.stringify(command)).digest('hex')===resourceSha256&&typeof checkpoint==='function','SCOPE_INVALID');
 const url=new URL('webservice/WebServiceImportacionCarpetas.asmx/AvanzarLote',profile.baseUrl).href,pin=JSON.stringify(command);
 let attempted=false,resolve;const completed=new Promise(r=>{resolve=r;});
 const session={...auth,context:{request:{post:async(target,options)=>{
  if(target!==url||JSON.stringify(options.data?.solicitud)!==pin||attempted)return auth.context.request.post(target,options);
  attempted=true;
  try{
   const response=await auth.context.request.post(target,options);
   check(response.status()===200,'FIRST_OUTCOME_UNCONFIRMED');
   const durable=await checkpoint();
   check(id(durable?.documentoId)&&id(durable?.inventarioId)&&/^[a-f0-9]{64}$/.test(durable.fingerprint),'CHECKPOINT_UNCONFIRMED');
   await response.dispose?.();
   resolve({status:'DROPPED_AFTER_CHECKPOINT',boundary:'CLIENT_API_RESPONSE_DELIVERY',documentoId:durable.documentoId,inventarioId:durable.inventarioId,fingerprint:durable.fingerprint});
  }catch{resolve({status:'FIRST_OUTCOME_UNCONFIRMED',boundary:'CLIENT_API_RESPONSE_DELIVERY'});}
  // The Playwright transport received a response; the DOC111 driver never gets
  // its body. This is a client delivery fault, not a wire/network-loss claim.
  throw Error('DOC118_RESPONSE_DELIVERY_SUPPRESSED');
 }}}};
 return{session,async outcome(milliseconds=60000){let timer;try{return await Promise.race([completed,new Promise((_,reject)=>{timer=setTimeout(()=>reject(Error('DOC118_RESPONSE_OUTCOME_UNCONFIRMED')),milliseconds);})]);}finally{clearTimeout(timer);}}};
}
module.exports={suppressFirstResponse};
