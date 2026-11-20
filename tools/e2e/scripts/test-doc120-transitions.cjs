'use strict';
// Local synthetic regression. This runner never asks for application credentials.
const fs=require('node:fs'),path=require('node:path'),{spawnSync}=require('node:child_process');
const root=path.resolve(__dirname,'../../..');
const suites=[
 'doc111-importacion-carpetas-client','doc119-navigation','doc119-context-menu',
 'doc120-controles-cierre','doc120-coordinacion-core','doc120-consultar-ultimo',
 'doc120-importacion-guiada','doc120-importacion-fidelidad','doc120-importacion-flujo-final',
 'doc120-importacion-arbol','doc120-preparacion-segmentada','doc120-recepcion-creacion',
 'doc120-retomar-creacion','doc120-preparacion-tamano','doc120-espera-reanudacion'
].map(name=>'tests/'+name+'.test.cjs').concat(fs.readdirSync(path.join(root,'tests'))
 .filter(name=>/^doc120-.*-browser\.test\.cjs$/.test(name)).sort().map(name=>'tests/'+name));
if(require.main===module){
 if(process.argv.length===3&&process.argv[2]==='--list'){process.stdout.write(JSON.stringify(suites,null,2)+'\n');}
 else if(process.argv.length!==2){process.stderr.write('Usage: node tools/e2e/scripts/test-doc120-transitions.cjs [--list]\n');process.exitCode=2;}
 else{const result=spawnSync(process.execPath,['--test','--test-concurrency=2','--test-timeout=180000',...suites],{cwd:root,stdio:'inherit',windowsHide:true});if(result.error){process.stderr.write(result.error.message+'\n');}process.exitCode=result.status===0?0:1;}
}
module.exports={suites};
