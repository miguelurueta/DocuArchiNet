'use strict';
const {test}=require('node:test'),assert=require('node:assert/strict'),path=require('node:path'),crypto=require('node:crypto'),{execFileSync,spawnSync}=require('node:child_process');
const {compile}=require('../tools/e2e/scripts/compile-doc117-request-hash.cjs');
const {ROOT}=require('../tools/e2e/scripts/support/doc117-alm02-candidate.cjs');
test('Offline .NET console accepts two exact storage requests and emits only their hashes',()=>{
 const dir=path.join(ROOT,'.tmp','doc117-hash-console-test-'+crypto.randomUUID()),{exe}=compile(dir);
 const input={Contexto:{UsuarioRef:'141',AmbitoRef:'2',NivelRef:'7',RutaCanonica:'SYNTHETIC_PRIVATE_CONTEXT',SelloDestino:'a'.repeat(64),PuedeImportar:true},Solicitudes:[{LoteId:'900',ArchivoId:'902',RevisionEsperada:2,ClaveOperacion:'SYNTHETIC_PRIVATE_KEY_A',Fase:'ALMACENAR'},{LoteId:'900',ArchivoId:'905',RevisionEsperada:2,ClaveOperacion:'SYNTHETIC_PRIVATE_KEY_B',Fase:'ALMACENAR'}]};
 const run=v=>spawnSync(exe,[],{input:JSON.stringify(v),encoding:'utf8',windowsHide:true,cwd:dir}),result=run(input);assert.equal(result.status,0);assert.equal(result.stderr,'');const hashes=JSON.parse(result.stdout);assert.equal(hashes.length,2);assert.ok(hashes.every(h=>/^[a-f0-9]{64}$/.test(h)));assert.notEqual(hashes[0],hashes[1]);assert.ok(!result.stdout.includes('SYNTHETIC_PRIVATE'));
 for(const bad of [{...input,unexpected:'SYNTHETIC_PRIVATE'}, {...input,Solicitudes:[]},{...input,Solicitudes:[{...input.Solicitudes[0],Fase:'CREAR_EXPEDIENTE'},input.Solicitudes[1]]}]){const rejected=run(bad);assert.equal(rejected.status,2);assert.equal(rejected.stdout,'');assert.equal(rejected.stderr.trim(),'DOC117_HASH_INPUT_REJECTED');}
});
