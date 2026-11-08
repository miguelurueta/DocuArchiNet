'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),path=require('node:path'),cp=require('node:child_process'),crypto=require('node:crypto');
test('DOC118 synthetic fixture and real SDK probe compile together without starting a process or attaching an IDE',()=>{
 const root=path.resolve(__dirname,'..'),probeId=crypto.randomBytes(16).toString('hex'),directory=path.join(root,'.tmp','doc118-release-probe-'+probeId);
 try{
  const output=cp.execFileSync('powershell.exe',['-NoProfile','-STA','-NonInteractive','-ExecutionPolicy','Bypass','-File',path.join(root,'tools/e2e/scripts/run-doc118-debugger-probe.ps1'),'-ProbeId',probeId,'-CompileOnly'],{cwd:root,encoding:'utf8',windowsHide:true});
  const r=JSON.parse(output.trim().replace(/^\uFEFF/,''));assert.equal(r.status,'COMPILE_PASS');assert.equal(r.observation,null);assert.equal(r.childExited,false);assert.equal(r.stage,'COMPILED');assert.equal(r.appLoginAttempts+r.httpRequests+r.selectStatements+r.documentEffects,0);assert.equal(r.controllerSha256,crypto.createHash('sha256').update(fs.readFileSync(path.join(root,'tools/e2e/scripts/support/Doc118Debugger.cs'))).digest('hex'));
 }finally{assert.ok(path.resolve(directory).startsWith(path.join(root,'.tmp')+path.sep));if(fs.existsSync(directory))fs.rmSync(directory,{recursive:true,force:true});}
});
