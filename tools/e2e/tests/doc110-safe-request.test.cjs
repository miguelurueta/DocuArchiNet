'use strict';
const {test}=require('node:test');
const assert=require('node:assert/strict');
const {safePost}=require('./support/doc110-safe-request.cjs');
test('DOC110: fallo de transporte no expone cabeceras ni reintenta',async()=>{
  let calls=0;
  const request={post:async(url,options)=>{
    calls++;assert.equal(options.timeout,30000);assert.equal(options.maxRetries,0);
    throw new Error('cookie: SYNTHETIC_COOKIE X-Importacion-Perfil-CSRF: SYNTHETIC_TOKEN');
  }};
  await assert.rejects(()=>safePost(request,'http://synthetic.invalid/',{},'EnsayarReglas'),error=>{
    assert.match(error.message,/DOC110_TRANSPORT_FAILED: EnsayarReglas/);
    assert.doesNotMatch(error.stack,/SYNTHETIC_COOKIE|SYNTHETIC_TOKEN|cookie:/);
    assert.equal(error.cause,undefined);return true;
  });
  assert.equal(calls,1);
});
test('DOC110: respuesta confirmada se conserva',async()=>{
  const response={status:()=>409};
  assert.equal(await safePost({post:async()=>response},'http://synthetic.invalid/',{},'GuardarBorrador'),response);
});
