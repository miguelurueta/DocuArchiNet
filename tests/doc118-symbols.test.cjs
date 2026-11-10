'use strict';
const test=require('node:test'),assert=require('node:assert/strict');
const {verifySymbolFiles}=require('../tools/e2e/tests/support/doc118-symbols.cjs');
function fixture(){
 const guid=Buffer.from('0123456789abcdef0123456789abcdef','hex'),pe=Buffer.alloc(1024),pdb=Buffer.alloc(2048);
 pe.write('MZ');pe.writeUInt32LE(128,60);pe.write('PE\0\0',128);pe.writeUInt16LE(1,134);pe.writeUInt16LE(224,148);pe.writeUInt16LE(0x10b,152);
 pe.writeUInt32LE(0x1000,296);pe.writeUInt32LE(28,300);pe.writeUInt32LE(0x1000,388);pe.writeUInt32LE(512,392);pe.writeUInt32LE(512,396);
 pe.writeUInt32LE(2,524);pe.writeUInt32LE(32,528);pe.writeUInt32LE(600,536);pe.write('RSDS',600);guid.copy(pe,604);pe.writeUInt32LE(1,620);
 pdb.write('Microsoft C/C++ MSF 7.00\r\n\x1aDS\0\0\0',0,'binary');pdb.writeUInt32LE(512,32);pdb.writeUInt32LE(4,40);pdb.writeUInt32LE(16,44);pdb.writeUInt32LE(1,52);pdb.writeUInt32LE(2,512);
 pdb.writeUInt32LE(2,1024);pdb.writeUInt32LE(0,1028);pdb.writeUInt32LE(28,1032);pdb.writeUInt32LE(3,1036);pdb.writeUInt32LE(1,1544);guid.copy(pdb,1548);
 return{pe,pdb};
}
test('DOC118 symbol match proves GUID/age on disk without claiming runtime observation',()=>{const f=fixture(),r=verifySymbolFiles(f.pe,f.pdb);assert.equal(r.guidAndAgeMatch,true);assert.equal(r.loadedModuleVerified,false);assert.equal(r.activeBodyObservationQualified,false);});
test('DOC118 symbol match rejects a different PDB identity or age',()=>{for(const offset of [1544,1548]){const f=fixture();f.pdb[offset]^=1;assert.throws(()=>verifySymbolFiles(f.pe,f.pdb),/UNCONFIRMED/);}});
test('DOC118 symbol metadata rejects truncated and unsupported formats',()=>{const f=fixture();assert.throws(()=>verifySymbolFiles(f.pe.subarray(0,200),f.pdb),/UNCONFIRMED/);f.pdb[0]=0;assert.throws(()=>verifySymbolFiles(f.pe,f.pdb),/UNCONFIRMED/);});
test('DOC118 reads a directory map spanning blocks as used by the installed PDB',()=>{
 const f=fixture(),pdb=Buffer.alloc(133*512),directory=Buffer.alloc(65544);f.pdb.copy(pdb,0,0,56);
 pdb.writeUInt32LE(133,40);pdb.writeUInt32LE(directory.length,44);pdb.writeUInt32LE(1,52);
 directory.writeUInt32LE(16384,0);directory.writeUInt32LE(28,8);directory.writeUInt32LE(132,65540);
 for(let i=0;i<129;i++){pdb.writeUInt32LE(i+3,512+i*4);directory.copy(pdb,(i+3)*512,i*512,Math.min((i+1)*512,directory.length));}
 f.pdb.copy(pdb,132*512,1536,1564);assert.equal(verifySymbolFiles(f.pe,pdb).guidAndAgeMatch,true);
});
