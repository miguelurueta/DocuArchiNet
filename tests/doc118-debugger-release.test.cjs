'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),path=require('node:path'),cp=require('node:child_process');
test('DOC118 C# release synchronizes before thaw and rejects break, thaw and go failures without touching an IDE',()=>{
 const output=cp.execFileSync('powershell.exe',['-NoProfile','-NonInteractive','-ExecutionPolicy','Bypass','-File',path.join(__dirname,'doc118-debugger-release.fixture.ps1')],{encoding:'utf8',windowsHide:true});
 assert.deepEqual(JSON.parse(output.replace(/^\uFEFF/,'')),Array(5).fill('PASS'));
});
