'use strict';
const {test}=require('node:test'),assert=require('node:assert/strict'),path=require('node:path');
const {execFileSync}=require('node:child_process');
const {TEST_FILTER,testFilter}=require('../tools/e2e/scripts/run-doc116-exp01-authorized.cjs');
test('EXP01 production runner filter discovers exactly the authorized test with no execution',()=>{
  const cwd=path.resolve(__dirname,'../tools/e2e');
  const output=execFileSync(process.execPath,[path.join(cwd,'node_modules/@playwright/test/cli.js'),'test','tests/doc116-exp01-authorized.spec.cjs','--grep',TEST_FILTER,'--workers=1','--retries=0','--list'],{cwd,encoding:'utf8',windowsHide:true});
  assert.match(output,/Total: 1 test in 1 file/);assert.match(output,/DOC116 SETUP-EXP01/);
});
test('EXP02 production runner selects only its test without execution',()=>{
  const cwd=path.resolve(__dirname,'../tools/e2e');
  const output=execFileSync(process.execPath,[path.join(cwd,'node_modules/@playwright/test/cli.js'),'test','tests/doc116-exp01-authorized.spec.cjs','--grep',testFilter('EXP-02'),'--workers=1','--retries=0','--list'],{cwd,encoding:'utf8',windowsHide:true});
  assert.match(output,/Total: 1 test in 1 file/);assert.match(output,/DOC116 SETUP-EXP02/);assert.doesNotMatch(output,/› DOC116 SETUP-EXP01/);assert.throws(()=>testFilter('EXP-04'),/SCOPE_INVALID/);
});
test('EXP03 production runner selects only its test without execution',()=>{
  const cwd=path.resolve(__dirname,'../tools/e2e');
  const output=execFileSync(process.execPath,[path.join(cwd,'node_modules/@playwright/test/cli.js'),'test','tests/doc116-exp01-authorized.spec.cjs','--grep',testFilter('EXP-03'),'--workers=1','--retries=0','--list'],{cwd,encoding:'utf8',windowsHide:true});
  assert.match(output,/Total: 1 test in 1 file/);assert.match(output,/DOC116 SETUP-EXP03/);assert.doesNotMatch(output,/› DOC116 SETUP-EXP0[12]/);
});
test('Prepared-lot EXP03 filter discovers exactly the existing creation test',()=>{
  const cwd=path.resolve(__dirname,'../tools/e2e');
  const output=execFileSync(process.execPath,[path.join(cwd,'node_modules/@playwright/test/cli.js'),'test','tests/doc116-expedientes-importacion.spec.cjs','--grep','DOC116 EXP-03$','--workers=1','--retries=0','--list'],{cwd,encoding:'utf8',windowsHide:true});
  assert.match(output,/Total: 1 test in 1 file/);assert.match(output,/DOC116 EXP-03/);
});
test('Internal EXP02 filter discovers exactly the gated laboratory test without executing it',()=>{
  const cwd=path.resolve(__dirname,'../tools/e2e');
  const output=execFileSync(process.execPath,[path.join(cwd,'node_modules/@playwright/test/cli.js'),'test','tests/doc116-internal-fault.spec.cjs','--grep','DOC116 INTERNAL-EXP02$','--workers=1','--retries=0','--list'],{cwd,encoding:'utf8',windowsHide:true});
  assert.match(output,/Total: 1 test in 1 file/);assert.match(output,/DOC116 INTERNAL-EXP02/);
});
