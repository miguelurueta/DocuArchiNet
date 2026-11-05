'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),path=require('node:path'),os=require('node:os');
const {execFileSync}=require('node:child_process');
const root=path.resolve(__dirname,'..');
test('actual PowerShell protocol restores on EOF, faults and lost results without a database',()=>{
 const temp=fs.mkdtempSync(path.join(os.tmpdir(),'doc116-level-'));
 try{const output=execFileSync('powershell.exe',['-NoProfile','-NonInteractive','-ExecutionPolicy','Bypass','-File',path.join(root,'tools/e2e/tests/doc116-level-permission-local.ps1'),'-RepositoryRoot',root,'-TemporaryRoot',temp],{cwd:root,encoding:'utf8',windowsHide:true});const result=JSON.parse(output.trim());assert.equal(result.status,'PASS');assert.ok(result.checks>=40);assert.equal(result.realDatabase,false);assert.equal(result.permissionChanges,false);}finally{if(fs.readdirSync(temp).length===0)fs.rmdirSync(temp);}
});
test('new level mode cannot start the shared bridge without exact ephemeral authorization',()=>{
 const {startDatabaseBridge}=require('../tools/e2e/tests/support/doc110-permission-revocation.cjs');
 assert.throws(()=>startDatabaseBridge('doc110-permission-db.ps1',{doc116LevelResourcesFile:'relative.json'}),/SCRIPT_INVALID/);
 assert.throws(()=>startDatabaseBridge('doc110-permission-db.ps1',{doc116LevelResourcesFile:path.join(root,'.tmp/not-a-resource.json')}),/SCRIPT_INVALID/);
 assert.throws(()=>startDatabaseBridge('doc110-profile-schema-db.ps1',{doc116LevelResourcesFile:path.join(root,'.tmp/not-a-resource.json')}),/SCRIPT_INVALID/);
});
test('actual administrative entry rejects an unauthorized DOC116 scope before resolving a database',()=>{
 const env=Object.fromEntries(Object.entries(process.env).filter(([key])=>!key.startsWith('DOC116_')));
 let failure;try{execFileSync('powershell.exe',['-NoProfile','-NonInteractive','-ExecutionPolicy','Bypass','-File',path.join(root,'tools/e2e/scripts/support/doc110-permission-db.ps1'),'-Doc116LevelResourcesFile',path.join(root,'.tmp/does-not-exist-doc116.json')],{cwd:root,encoding:'utf8',windowsHide:true,env});}catch(error){failure=error;}
 assert.ok(failure);const result=JSON.parse(failure.stdout.trim());assert.equal(result.ok,false);assert.equal(result.code,'DOC116_LEVEL_OPERATION_OR_RECOVERY_FAILED');assert.equal(String(failure.stderr).trim(),'');
});
