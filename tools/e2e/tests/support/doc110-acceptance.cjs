'use strict';
const {expect}=require('@playwright/test');
const fs=require('node:fs'),path=require('node:path');
const {execFile}=require('node:child_process');
const {captureCodeEvidence}=require('./doc110-code-evidence.cjs');
async function exerciseAcceptance(auth,baseUrl,token){
  if(process.env.DOC110_E2E_USER?.toUpperCase()!=='LUZ.AGUILERA')throw new Error('DOC110_ACCEPTANCE_AUTHORIZED_ACCOUNT_REQUIRED');
  const evidence={status:'RUNNING',startedUtc:new Date().toISOString(),code:captureCodeEvidence()};
  const root=path.resolve(__dirname,'../../../..');
  const reports=path.resolve(__dirname,'../../artifacts');
  try{
    const draft=await require('./doc110-draft-lifecycle.cjs').exerciseDraft(auth,baseUrl,token);
    evidence.draft=draft;
    const response=await require('./doc110-admin-navigation.cjs').openImportAdministrator(auth.page,'abrir','ListarPerfiles');
    expect(response.status()).toBe(200);expect((await response.json()).d?.ok).toBe(true);
    const published=await require('./doc110-publication-lifecycle.cjs').exercisePublication(auth,baseUrl,token);
    evidence.published=published;
    await require('./doc110-permission-revocation.cjs').exercisePermissionRevocation(auth,baseUrl,token,{PerfilId:published.perfilId});
    // Solo IDs confirmados por servidor y validados antes de construir argumentos.
    for(const id of [draft.perfilId,published.perfilId])if(!/^[1-9][0-9]*$/.test(id))throw new Error('DOC110_ACCEPTANCE_PROFILE_ID_INVALID');
    await new Promise((resolve,reject)=>{
      execFile('powershell.exe',['-NoProfile','-NonInteractive','-ExecutionPolicy','Bypass','-File',
        path.join(root,'tools/e2e/scripts/verify-doc110-identities-readonly.ps1'),
        '-DraftProfileId',draft.perfilId,'-PublishedProfileId',published.perfilId],
      {windowsHide:true,timeout:45000},error=>error?reject(new Error('DOC110_ACCEPTANCE_SELECT_FAILED')):resolve());
    });
    const sql=JSON.parse(fs.readFileSync(path.join(reports,'doc110-acceptance-select-result.json'),'utf8').replace(/^\uFEFF/,''));
    expect(sql.status).toBe('PASS');expect(sql.cases.length).toBe(2);expect(sql.checks.every(c=>c.passed)).toBe(true);
    expect(sql.cases.map(c=>c.profileId)).toEqual([draft.perfilId,published.perfilId]);
    expect(captureCodeEvidence().scopeSha256).toBe(evidence.code.scopeSha256);
    evidence.select=sql;evidence.status='PASS';
    console.log('DOC110_PASS: PER_01_02_03_SHA_UI_HTTP_SELECT_CONFIRMED');
  }catch(error){evidence.status='FAIL';throw error;}
  finally{
    evidence.finishedUtc=new Date().toISOString();fs.mkdirSync(reports,{recursive:true});
    fs.writeFileSync(path.join(reports,'doc110-acceptance-result.json'),JSON.stringify(evidence,null,2));
  }
}
module.exports={exerciseAcceptance};
