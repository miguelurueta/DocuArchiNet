'use strict';
const fs=require('node:fs'),crypto=require('node:crypto'),path=require('node:path');
const change='openspec/changes/doc-118-produccion-importacion-carga-carpeta-lote-recuperacion',authPath=change+'/v2-runtime-smoke-authorization.json',proposalPath=change+'/v2-runtime-smoke-proposal.json';
const hash=p=>crypto.createHash('sha256').update(fs.readFileSync(p)).digest('hex');
async function main(){
 if(!fs.existsSync(authPath))throw Error('DOC118_P4B_AUTHORIZATION_REQUIRED');
 const auth=JSON.parse(fs.readFileSync(authPath,'utf8')),p=JSON.parse(fs.readFileSync(proposalPath,'utf8'));
 if(auth.status!=='AUTHORIZED'||auth.stage!=='P4_B_HEAD_AND_SOURCE_BINDING_SMOKE'||auth.proposalSha256!==hash(proposalPath)||p.maximum.httpRequests!==1||p.environment.baseUrl!=='https://localhost/GestionDocumental-Docuarchi.net/'||p.http.method!=='HEAD'||p.http.relativePath!=='gestor.aspx'||p.http.retries!==0||p.http.redirects!==0||p.http.localTlsException!==true)throw Error('DOC118_P4B_AUTHORIZATION_SCOPE_INVALID');
 if(p.localResultDirectory!=='.tmp/doc118-p4b-20261109-01')throw Error('DOC118_P4B_HEAD_CAPACITY_UNAVAILABLE');
 const directory=path.resolve(p.localResultDirectory);
 if(!fs.existsSync(directory)||fs.realpathSync(directory).toLowerCase()!==directory.toLowerCase())throw Error('DOC118_P4B_HEAD_CAPACITY_UNAVAILABLE');
 try{fs.writeFileSync(path.join(directory,'head-dispatch.json'),JSON.stringify({operation:'HEAD_GESTOR',maximum:1,attemptConsumed:true})+'\n',{flag:'wx'});}catch{throw Error('DOC118_P4B_HEAD_CAPACITY_ALREADY_CONSUMED');}
 const {request}=require(path.resolve('tools/e2e/node_modules/@playwright/test'));
 const result={operation:'HEAD_GESTOR',httpRequests:0,status:null,redirectFollowed:false,authenticated:false};let client;
 try{client=await request.newContext({baseURL:p.environment.baseUrl,ignoreHTTPSErrors:true});result.httpRequests=1;const response=await client.head(p.http.relativePath,{timeout:10000,maxRetries:0,maxRedirects:0});result.status=response.status();await response.dispose();}catch{result.failure='HEAD_UNCONFIRMED';}finally{if(client)try{await client.dispose();}catch{result.failure='HTTP_CONTEXT_CLOSE_UNCONFIRMED';}}
 console.log(JSON.stringify(result));if(result.failure||![200,302].includes(result.status))process.exitCode=2;
}
main().catch(e=>{const publicCodes=new Set(['DOC118_P4B_AUTHORIZATION_REQUIRED','DOC118_P4B_AUTHORIZATION_SCOPE_INVALID','DOC118_P4B_HEAD_CAPACITY_UNAVAILABLE','DOC118_P4B_HEAD_CAPACITY_ALREADY_CONSUMED']);console.log(JSON.stringify({operation:'HEAD_GESTOR',httpRequests:0,failure:publicCodes.has(e?.message)?e.message:'DOC118_P4B_START_UNCONFIRMED'}));process.exitCode=2;});
