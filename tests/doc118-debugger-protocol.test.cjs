'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),path=require('node:path'),cp=require('node:child_process');
test('DOC118 actual PowerShell protocol accepts empty inspection and refuses extra args, sequence reuse and expression injection',()=>{
 const arm={sequence:1,operation:'ARM',args:{expA:400,expB:401,nameA:'DOC118-C02-A-20261107.pdf',nameB:'DOC118-C02-B-20261107.pdf',resourceSha256:'a'.repeat(64)}};
 const commands=[{sequence:1,operation:'INSPECT',args:{}},{sequence:1,operation:'RESUME',args:{}},arm,{sequence:1,operation:'INSPECT',args:{Count:0}},{sequence:2,operation:'CLOSE',args:{}},{...arm,args:{...arm.args,nameA:'CallProductFunction()'}},{...arm,args:{...arm.args,expA:'400'}},{...arm,args:{...arm.args,nameB:'DOC118-C03-B-20261107.pdf'}},{sequence:1,operation:'CLOSE',args:null}];
 const output=cp.execFileSync('powershell.exe',['-NoProfile','-NonInteractive','-ExecutionPolicy','Bypass','-File',path.join(__dirname,'doc118-debugger-protocol.fixture.ps1')],{input:JSON.stringify(commands.map(command=>({command}))),encoding:'utf8',windowsHide:true,env:Object.fromEntries(Object.entries(process.env).filter(([k])=>['PATH','Path','SystemRoot','WINDIR','TEMP','TMP'].includes(k)))});
 assert.deepEqual(JSON.parse(output.replace(/^\uFEFF/,'')),[true,true,true,false,false,false,false,false,false]);
});
