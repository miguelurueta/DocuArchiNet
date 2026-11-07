'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),path=require('node:path'),{execFileSync}=require('node:child_process');
const root=path.resolve(__dirname,'..'),script=path.join(root,'tools/e2e/scripts/run-doc111-persistence.ps1');
test('07-A mode refuses writes before compilation or connection when its authorization is absent',()=>{
  for(const flags of [[],['-AuthorizedDoc115','-ResourcesFile','absent.json','-RuntimeRoot',root]]){
    assert.throws(()=>execFileSync('powershell.exe',['-NoProfile','-NonInteractive','-ExecutionPolicy','Bypass','-File',script,'-Mode','Doc115',...flags],{env:{...process.env,DOC115_07A_AUTHORIZED:'false'},windowsHide:true,stdio:['ignore','pipe','pipe']}),e=>String(e.stderr).includes('DOC115_REPOSITORY_AUTHORIZATION_REQUIRED'));
  }
});
test('existing persistence modes retain their original two authorizations',()=>{
  assert.throws(()=>execFileSync('powershell.exe',['-NoProfile','-NonInteractive','-ExecutionPolicy','Bypass','-File',script,'-Mode','Create'],{env:{...process.env,DOC115_07A_AUTHORIZED:'false'},windowsHide:true,stdio:['ignore','pipe','pipe']}),e=>String(e.stderr).includes('DOC111_PERSISTENCE_AUTHORIZATION_REQUIRED'));
});
test('new PowerShell helpers parse without executing SQL or the harness',()=>{
  const files=[script,path.join(root,'tools/e2e/scripts/support/doc115-repository-integration.ps1'),path.join(root,'tools/e2e/scripts/support/doc115-readonly-failure.ps1')];
  for(const file of files){const literal=file.replace(/'/g,"''");const command="$e=$null;$t=$null;[Management.Automation.Language.Parser]::ParseFile('"+literal+"',[ref]$t,[ref]$e)|Out-Null;if($e.Count){exit 1}";execFileSync('powershell.exe',['-NoProfile','-NonInteractive','-Command',command],{windowsHide:true,stdio:['ignore','pipe','pipe']});}
});
test('read-only classification emits closed codes and never echoes supplied values',()=>{
  const helper=path.join(root,'tools/e2e/scripts/support/doc115-readonly-failure.ps1').replace(/'/g,"''");
  const commands=[". '"+helper+"'","Get-Doc115ReadOnlyFailure -Step OPEN_CONNECTION -States @('28000') -Native @(1045)","Get-Doc115ReadOnlyFailure -Step OPEN_CONNECTION -States @('IM002')","Get-Doc115ReadOnlyFailure -Step SELECT -Native @(1142)","Get-Doc115ReadOnlyFailure -Step SELECT -States @('42S22')","Get-Doc115ReadOnlyFailure -Step OPEN_CONNECTION -States @('PRIVATE_PASSWORD_SENTINEL')"];
  const result=execFileSync('powershell.exe',['-NoProfile','-NonInteractive','-ExecutionPolicy','Bypass','-Command',commands.join(';')],{windowsHide:true,encoding:'utf8',stdio:['ignore','pipe','pipe']}).trim().split(/\r?\n/);
  assert.deepEqual(result,['DOC115_READONLY_AUTHENTICATION_FAILED','DOC115_READONLY_DSN_UNAVAILABLE','DOC115_READONLY_SELECT_DENIED','DOC115_READONLY_COLUMN_UNAVAILABLE','DOC115_READONLY_OPEN_FAILED']);
});
