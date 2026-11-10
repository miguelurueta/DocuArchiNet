'use strict';
// Read file metadata only. Matching files do not prove loaded runtime symbols.
const crypto=require('node:crypto');
const check=(v)=>{if(!v)throw Error('DOC118_SYMBOL_METADATA_UNCONFIRMED');};
function span(bytes,start,length){check(Buffer.isBuffer(bytes)&&Number.isSafeInteger(start)&&Number.isSafeInteger(length)&&start>=0&&length>=0&&start+length<=bytes.length);return bytes.subarray(start,start+length);}
const u32=(b,p)=>span(b,p,4).readUInt32LE();
function codeView(pe){
 check(span(pe,0,2).toString()==='MZ');const header=u32(pe,60);check(span(pe,header,4).equals(Buffer.from([80,69,0,0])));
 const sections=span(pe,header+6,2).readUInt16LE(),optionalSize=span(pe,header+20,2).readUInt16LE(),optional=header+24,magic=span(pe,optional,2).readUInt16LE();
 check(sections>0&&sections<=128&&(magic===0x10b||magic===0x20b));
 const directories=optional+(magic===0x10b?96:112);check(directories+56<=optional+optionalSize);
 const debugRva=u32(pe,directories+48),debugSize=u32(pe,directories+52);check(debugRva>0&&debugSize>=28&&debugSize%28===0&&debugSize<=28*128);
 let raw=null;
 for(let i=0;i<sections;i++){const s=optional+optionalSize+i*40,va=u32(pe,s+12),size=u32(pe,s+16);if(debugRva>=va&&debugRva-va+debugSize<=size){check(raw===null);raw=u32(pe,s+20)+debugRva-va;}}
 check(raw!==null);const found=[];
 for(let i=0;i<debugSize;i+=28){const entry=raw+i;if(u32(pe,entry+12)!==2)continue;const size=u32(pe,entry+16),pointer=u32(pe,entry+24),data=span(pe,pointer,size);check(size>=24&&data.subarray(0,4).toString()==='RSDS');found.push({guid:data.subarray(4,20).toString('hex'),age:u32(data,20)});}
 check(found.length===1);return found[0];
}
function pdbInfo(pdb){
 check(span(pdb,0,32).equals(Buffer.from('Microsoft C/C++ MSF 7.00\r\n\x1aDS\0\0\0','binary')));
 const size=u32(pdb,32),blocks=u32(pdb,40),directorySize=u32(pdb,44),map=u32(pdb,52);
 check([512,1024,2048,4096].includes(size)&&blocks*size===pdb.length&&directorySize>0&&directorySize<=pdb.length);
 const pages=Math.ceil(directorySize/size);check(map<blocks);span(pdb,map*size,pages*4);
 const readPages=(list,length)=>Buffer.concat(list.map(i=>{check(i<blocks);return span(pdb,i*size,size);})).subarray(0,length);
 const directory=readPages(Array.from({length:pages},(_,i)=>u32(pdb,map*size+i*4)),directorySize),streams=u32(directory,0);
 check(streams>=2&&streams<=100000);let cursor=4+streams*4,info;
 for(let i=0;i<streams;i++){const length=u32(directory,4+i*4);if(length===0xffffffff)continue;const count=Math.ceil(length/size);check(cursor+count*4<=directory.length);if(i===1)info=readPages(Array.from({length:count},(_,j)=>u32(directory,cursor+j*4)),length);cursor+=count*4;}
 check(info&&info.length>=28);return{guid:span(info,12,16).toString('hex'),age:u32(info,8)};
}
function verifySymbolFiles(pe,pdb){
 const expected=codeView(pe),actual=pdbInfo(pdb);check(expected.guid===actual.guid&&expected.age===actual.age);
 const hash=b=>crypto.createHash('sha256').update(b).digest('hex');
 return{kind:'ON_DISK_CODEVIEW_PDB_MATCH',dllSha256:hash(pe),pdbSha256:hash(pdb),guidAndAgeMatch:true,loadedModuleVerified:false,activeBodyObservationQualified:false};
}
module.exports={codeView,pdbInfo,verifySymbolFiles};
