'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto'),cp=require('node:child_process');
const root=path.resolve(__dirname,'..'),probeId=crypto.randomBytes(16).toString('hex'),directory=path.join(root,'.tmp','doc118-staging-lock-probe-'+probeId);
let result;
test.before(()=>{
 const output=cp.execFileSync('powershell.exe',['-NoProfile','-NonInteractive','-ExecutionPolicy','Bypass','-File',path.join(root,'tools/e2e/scripts/run-doc118-staging-lock-probe.ps1'),'-ProbeId',probeId],{cwd:root,encoding:'utf8',windowsHide:true,timeout:60000,env:Object.fromEntries(Object.entries(process.env).filter(([key])=>['PATH','PATHEXT','SYSTEMROOT','WINDIR','TEMP','TMP','COMSPEC'].includes(key.toUpperCase())))});
 result=JSON.parse(output.trim().replace(/^\uFEFF/,''));
 fs.writeFileSync(path.join(root,'.tmp/doc118-staging-lock-probe-latest.json'),JSON.stringify(result,null,2)+'\n');
});
test.after(()=>{assert.ok(path.resolve(directory).startsWith(path.join(root,'.tmp')+path.sep));if(result?.cleanupConfirmed&&fs.existsSync(directory))fs.rmSync(directory,{recursive:true,force:true});});
const cases=['active_reader_blocks_recovery','graceful_exit_allows_same_original','process_exit_releases_handle','exclusive_guard_blocks_late_reader_and_contender','identical_copy_has_different_identity','hardlink_alias_rejected','same_path_replacement_rejected','missing_original_not_created','changed_content_rejected_without_handle_leak','disposed_guard_cannot_be_reused','reader_after_release_requires_durable_fence','network_path_refused_without_open'];
for(const name of cases)test('DOC118 local staging capability: '+name,()=>{const selected=result.cases.filter(c=>c.name===name);assert.equal(selected.length,1);assert.equal(selected[0].passed,true);});
test('DOC118 staging probe remains a local capability result without granting production recovery',()=>{
 assert.equal(result.status,'PASS');assert.equal(result.fileSystem,'NTFS');assert.equal(result.driveType,3);assert.equal(result.cleanupConfirmed,true);assert.equal(result.childrenStarted,6);assert.equal(result.childrenExited,6);assert.equal(result.productionProviderQualified,false);assert.equal(result.providerPortImplemented,false);assert.equal(result.requiresDurableFence,true);assert.equal(result.fileIdIsPermanent,false);assert.equal(result.crossNodeIdentityProven,false);assert.equal(result.appLoginAttempts+result.httpRequests+result.selectStatements+result.businessEffects,0);assert.equal(result.fixtureSha256,crypto.createHash('sha256').update(fs.readFileSync(path.join(root,'tests/doc118-staging-lock-probe.fixture.cs'))).digest('hex'));
});
