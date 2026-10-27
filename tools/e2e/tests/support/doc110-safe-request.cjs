'use strict';
async function safePost(request,url,options,label){
  const operation=/^[A-Za-z]+$/.test(label||'')?label:'Request';
  const started=Date.now();
  try{
    return await request.post(url,{...options,timeout:30000,maxRetries:0});
  }catch{
    // No conservar cause/stack original: Playwright incluye cookies y cabeceras.
    throw new Error('DOC110_TRANSPORT_FAILED: '+operation+'; resultado no confirmado. Sin reintento automático.');
  }finally{
    console.log('DOC110_TIMING: '+operation+' '+(Date.now()-started)+'ms');
  }
}
module.exports={safePost};
