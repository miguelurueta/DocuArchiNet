'use strict';
const {ROOT,sha}=require('./doc117-alm03-setup-scope.cjs');
const {validateBinding}=require('../../scripts/support/doc117-alm03-canonical-description.cjs');
const PROTOCOL='ALM03_CANONICAL_CLOSED_RESOURCES_BEFORE_WRITER_V2',CASE='ALM-03C';
const check=(v,c)=>{if(!v)throw Error('DOC117_ALM03C_'+c);},id=v=>typeof v==='string'&&/^[1-9][0-9]{0,9}$/.test(v)&&BigInt(v)<=4294967295n,hash=v=>typeof v==='string'&&/^[a-f0-9]{64}$/.test(v);
const closed=(v,keys)=>check(v&&typeof v==='object'&&!Array.isArray(v)&&Object.keys(v).sort().join(',')===keys.slice().sort().join(','),'SCOPE_FIELDS_CHANGED');
const ROOT_NAMES=['DOC117-ALM03C-A-20261106','DOC117-ALM03C-B-20261106'],FILE_NAMES=['DOC117-ALM03C-IMPORT-A-20261106.pdf','DOC117-ALM03C-IMPORT-B-20261106.pdf'],INDIVIDUAL_NAME='DOC117-ALM03C-IND-20261106.pdf';
function validateScope(s){
 closed(s,['individual','issueKey','caseId','protocol','runtimeRoot','runId','lotId','snapshotSha256','resources','descriptionBinding']);
 check(s.issueKey==='DOC-117'&&s.caseId===CASE&&s.protocol===PROTOCOL&&s.runtimeRoot===ROOT&&/^[a-f0-9]{32}$/.test(s.runId)&&id(s.lotId)&&BigInt(s.lotId)>22n&&hash(s.snapshotSha256)&&Array.isArray(s.resources)&&s.resources.length===2,'RESOURCE_SCOPE_INVALID');
 for(const [i,r]of s.resources.entries()){closed(r,['rootId','fileId','expedienteId','relacionNivelId','fileName','bytes','contentSha256','requestSha256']);check(['rootId','fileId','expedienteId','relacionNivelId'].every(k=>id(r[k]))&&BigInt(r.rootId)>36n&&BigInt(r.fileId)>37n&&BigInt(r.expedienteId)>667n&&BigInt(r.relacionNivelId)>1092n&&r.fileName===FILE_NAMES[i]&&r.bytes==='677'&&r.contentSha256==='a5f1924afbae4bd631de893c49a56d9f09219075113e6da2ec983602cff84c6c'&&hash(r.requestSha256),'IMPORT_RESOURCE_INVALID');}
 check(['rootId','fileId','expedienteId','relacionNivelId','requestSha256'].every(k=>new Set(s.resources.map(r=>r[k])).size===2),'RESOURCE_COLLISION');
 closed(s.individual,['expedienteId','levelId','relacionNivelId','typologyId','fileName','classificationSha256','cabinetName','bytes','contentSha256']);const i=s.individual;
 check(i.expedienteId==='467'&&i.levelId==='8'&&i.relacionNivelId==='1072'&&i.typologyId==='50'&&i.fileName===INDIVIDUAL_NAME&&i.bytes==='677'&&hash(i.classificationSha256)&&/^[A-Za-z_][A-Za-z0-9_]{0,63}$/.test(i.cabinetName)&&i.contentSha256===s.resources[0].contentSha256,'INDIVIDUAL_SCOPE_INVALID');validateBinding(s.descriptionBinding);return s;
}
module.exports={ROOT,sha,PROTOCOL,CASE,ROOT_NAMES:Object.freeze(ROOT_NAMES),FILE_NAMES:Object.freeze(FILE_NAMES),INDIVIDUAL_NAME,validateScope};
