// DOC-121: auditoria local de entregables. No conecta a MySQL ni ejecuta SQL.
import fs from 'node:fs/promises';
import path from 'node:path';
import crypto from 'node:crypto';
import {execFileSync} from 'node:child_process';

const root=process.cwd();
const change='doc-121-importacion-excel-01-esquema-contratos-configuracion';
const activeRoot='openspec/changes/'+change;
const archiveParent='openspec/changes/archive';
const activeExists=await fs.access(path.join(root,activeRoot)).then(()=>true).catch(()=>false);
const archivedCandidates=activeExists?[]:(await fs.readdir(path.join(root,archiveParent))).filter(name=>/^\d{4}-\d{2}-\d{2}-/.test(name)&&name.endsWith('-'+change));
if(!activeExists&&archivedCandidates.length!==1)throw Error('DOC121_CHANGE_LOCATION_AMBIGUOUS');
const changeRoot=activeExists?activeRoot:archiveParent+'/'+archivedCandidates[0];
const base='Doc/Actualizacion/Importacion/';
const sqlNames=['00-preflight-doc121.sql','01-permiso-carga-ra-doc121.sql',
 '02-plantilla-da-doc121.sql','03-ejecucion-da-doc121.sql',
 '04-adecuacion-clave-doc121.sql','05-concesiones-da-doc121.sql','06-verificacion-doc121.sql'];
const checks=[];
const check=(name,ok,detail)=>checks.push({name,status:ok?'PASS':'FAIL',...(detail?{detail}:{})});
const read=file=>fs.readFile(path.join(root,file),'utf8');
const hash=text=>crypto.createHash('sha256').update(text).digest('hex');

// Excluye comentarios, literales e identificadores citados para distinguir
// sentencias ejecutables de SQL que el generador devuelve como texto.
function executableStatements(sql){
 let quote=null,line=false,block=false,code='';
 for(let i=0;i<sql.length;i++){
  const c=sql[i],n=sql[i+1];
  if(line){if(c==='\n'){line=false;code+='\n';}continue;}
  if(block){if(c==='*'&&n==='/'){block=false;i++;}continue;}
  if(quote){if(c==='\\'&&quote!=='`'){i++;continue;}if(c===quote){if(n===quote){i++;}else quote=null;}continue;}
  if(c==='-'&&n==='-'){line=true;i++;code+=' ';continue;}
  if(c==='/'&&n==='*'){block=true;i++;code+=' ';continue;}
  if(c==='\''||c==='"'||c==='`'){quote=c;code+=' ';continue;}
  code+=c;
 }
 if(quote||block)throw Error('SQL_LEXICAL_STRUCTURE_INVALID');
 return code.split(';').map(s=>s.trim()).filter(Boolean);
}

const sources={};
for(const name of sqlNames){
 const file=base+'Sql/'+name,sql=await read(file),statements=executableStatements(sql);
 sources[file]=hash(sql.replace(/\r\n/g,'\n'));
 if(/^(00|04|06)-/.test(name))check(name+':only_select',statements.length>0&&statements.every(s=>/^SELECT\b/i.test(s)),String(statements.length)+' SELECT statements');
 if(/^0[123]-/.test(name)){
  check(name+':single_ddl',statements.length===1&&/^(?:ALTER TABLE|CREATE TABLE)\b/i.test(statements[0]));
  check(name+':mysql51_scope',!statements.some(s=>/\b(?:JSON|CHECK|TRIGGER|SIGNAL|DELIMITER|utf8mb4)\b|FOREIGN\s+KEY|IF\s+NOT\s+EXISTS|DATETIME\s*\(/i.test(s)));
 }
 if(name.startsWith('01-'))check('permission_default_denied',/ADMINISTRAR_CARGAS_IMPORTACION\s+TINYINT UNSIGNED NOT NULL DEFAULT 0/i.test(sql));
 if(name.startsWith('02-'))check('only_two_additive_columns',(sql.match(/ADD COLUMN/g)||[]).length===2&&!/\b(?:MODIFY|DROP|UPDATE|INSERT)\b/.test(statements.join(' ')));
 if(name.startsWith('03-')){
  const columns=[...sql.matchAll(/^  ([a-z_]+) (?:CHAR|VARCHAR|INT|BIGINT|BINARY|LONGTEXT|DATETIME)\b/gm)].map(m=>m[1]);
  check('execution_22_columns',columns.length===22&&new Set(columns).size===22);
  check('execution_innodb',/ENGINE=InnoDB/.test(sql));
  check('execution_preparation_nullable',['modo','revision_plantilla','configuracion_doc','configuracion_hash','esquema_hash','filas_previstas'].every(c=>new RegExp('^  '+c+' [^\\n]+ NULL,?$','m').test(sql)));
  check('execution_primary_and_three_indexes',(sql.match(/^  PRIMARY KEY/gm)||[]).length===1&&(sql.match(/^  KEY /gm)||[]).length===3);
  check('execution_index_bytes_bounded',[36,128+128+8+36,4+8+36,128+64+64+32].every(n=>n<767),'Conservative declared ASCII key lengths; not a server measurement.');
 }
 if(name.startsWith('04-')){
  check('key_generator_not_dynamic_execution',statements.length===1&&!/\b(?:PREPARE|EXECUTE|CALL)\b/i.test(statements[0]));
  check('key_generator_preserves_metadata',['COLUMN_TYPE','CHARACTER_SET_NAME','COLLATION_NAME','COLUMN_DEFAULT','COLUMN_COMMENT'].every(f=>sql.includes('c.'+f)));
 }
 if(name.startsWith('05-')){
  check('grants_inert_template',statements.length===0);
  const grant=/-- GRANT ([\s\S]+?);/.exec(sql)?.[1].replace(/^--\s*/gm,'')||'';
  check('grants_scoped_no_secret',/CREATE TEMPORARY TABLES/.test(grant)&&/BASE_DA_AUTORIZADA/.test(grant)&&!/\*\.\*|IDENTIFIED BY|GRANT OPTION|\bDROP\b/.test(grant));
 }
}

const specFile=changeRoot+'/specs/importacion-excel-01-esquema-contratos-configuracion/spec.md';
const spec=await read(specFile),design=await read(changeRoot+'/design.md'),tasks=await read(changeRoot+'/tasks.md');
check('eight_requirements',(spec.match(/^### Requirement:/gm)||[]).length===8);
check('twenty_four_scenarios',(spec.match(/^#### Scenario:/gm)||[]).length===24);
check('eight_decisions_in_design_spec_tasks',Array.from({length:8},(_,i)=>'D-'+String(i+1).padStart(2,'0')).every(id=>[design,spec,tasks].every(t=>t.includes(id))));
const contract=await read(base+'Diseno/01-contratos-configuracion-doc121.md');
check('operations_defined',['AbrirCargaExcel','ListarPlantillas','ConsultarPlantilla','GuardarPlantilla','RecibirArchivo','Preparar','Confirmar','ConsultarResultado'].every(op=>contract.includes('| '+op)));
check('chosen_limits_and_recovery',contract.includes('10000000')&&contract.includes('10000')&&contract.includes('CONFIRMADA')&&contract.includes('no repetir por tiempo'));

const pendingTasks=(tasks.match(/^- \[ \]/gm)||[]).length;
const completedTasks=(tasks.match(/^- \[[xX]\]/gm)||[]).length;
const reviewFile='Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-verificacion-final-20261120.json';
const previous=JSON.parse(await read(reviewFile));
const changedInputs=[];
for(const [file,expected] of Object.entries(previous.sources))if(hash(await fs.readFile(path.join(root,file)))!==expected)changedInputs.push(file);
check('doc120_execution_inputs_unchanged',changedInputs.length===0,String(Object.keys(previous.sources).length)+' prior execution inputs checked');

const changed=execFileSync('git',['diff','--name-only','HEAD'],{cwd:root,encoding:'utf8',stdio:['ignore','pipe','ignore']}).trim().split(/\r?\n/).filter(Boolean);
const untracked=execFileSync('git',['ls-files','--others','--exclude-standard'],{cwd:root,encoding:'utf8'}).trim().split(/\r?\n/).filter(Boolean);
const allowed=[changeRoot+'/',activeRoot+'/','openspec/specs/importacion-excel-01-esquema-contratos-configuracion/',base+'Sql/',base+'Diseno/',base+'Validacion/','Doc/Tecnica/Opsxj/'+change+'/'];
check('delivery_scope_only',changed.concat(untracked).every(f=>allowed.some(prefix=>f.startsWith(prefix))));
const docs=[base+'Diseno/01-contratos-configuracion-doc121.md',base+'Sql/README.md',base+'Validacion/revisar-doc121.mjs',
 ...['proposal.md','design.md','refinement.md','tasks.md'].map(n=>changeRoot+'/'+n),specFile,
 ...['01-ResumenTecnico.md','02-ImpactoUI.md','03-ServiciosYReglas.md','04-ContratosIntegracion.md','05-PruebasEvidencia.md'].map(n=>'Doc/Tecnica/Opsxj/'+change+'/'+n)];
for(const file of docs){const content=await read(file);sources[file]=hash(content.replace(/\r\n/g,'\n'));}
const report={issueKey:'DOC-121',recordedUtc:new Date().toISOString(),reviewBase:execFileSync('git',['rev-parse','HEAD'],{cwd:root,encoding:'utf8'}).trim(),status:checks.every(c=>c.status==='PASS')?'PASS':'FAIL',checks,taskCounts:{complete:completedTasks,pending:pendingTasks},sources,sourceHashFormat:'SHA256 of UTF8 text normalized to LF; working-tree delivery fingerprint, final commit binding in excluded OPSXJ evidence.',installation:'NOT_EVALUATED_BY_STATIC_AUDIT',databaseConnections:0,sqlExecutions:0,realE2E:0,limitations:['Lexical/static checks do not parse SQL in MySQL or certify engine behavior.','This static audit does not perform or assess installation; actual SQL08 execution is recorded separately.','Existing DOC-120 source identity is regression evidence, not Excel execution.']};
if(process.argv.includes('--write-report'))await fs.writeFile(base+'Validacion/doc121-revision-fase01-20261120.json',JSON.stringify(report,null,2)+'\n');
console.log(JSON.stringify({status:report.status,checks:checks.length,passed:checks.filter(c=>c.status==='PASS').length,failed:checks.filter(c=>c.status==='FAIL').map(c=>c.name),taskCounts:report.taskCounts,installation:report.installation,sqlExecutions:0}));
if(report.status!=='PASS')process.exitCode=1;
