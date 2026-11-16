'use strict';
// Diagnostico local: importa el cliente real con transporte/efectos sinteticos.
// No autentica, no conecta a BD, no lee PDF ni crea lotes de aplicacion.
const fs = require('node:fs');
const path = require('node:path');
const assert = require('node:assert/strict');
const crypto = require('node:crypto');
const {performance} = require('node:perf_hooks');
const root = path.resolve(__dirname, '../../..');
const inventoryModule = require(path.join(root, 'js/gestion/ImportacionCarpetas/InventarioImportacion.js'));
const core = require(path.join(root, 'js/gestion/ImportacionCarpetas/ImportacionCarpetas.js'));
const apiModule = require(path.join(root, 'js/gestion/ImportacionCarpetas/ImportacionCarpetasApi.js'))({});
const sources = ['js/gestion/ImportacionCarpetas/InventarioImportacion.js',
    'js/gestion/ImportacionCarpetas/ImportacionCarpetas.js',
    'js/gestion/ImportacionCarpetas/ImportacionCarpetasApi.js',
    'Services/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionJson.vb',
    'Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.vb',
    'tools/e2e/scripts/measure-doc120-client-capacity.cjs'];
function digest(file) { return crypto.createHash('sha256').update(fs.readFileSync(file, 'utf8').replace(/\r\n/g, '\n')).digest('hex'); }
function metadata(planPath) {
    if (!planPath) return {source:'SYNTHETIC_METADATA', files:Array.from({length:1000}, (_, i) => ({
        name:i+'.pdf', webkitRelativePath:'Cantidad/'+i+'.pdf', size:6, lastModified:0}))};
    const text = fs.readFileSync(planPath, 'utf8').replace(/^\uFEFF/, '');
    const plan = JSON.parse(text), target = path.resolve(plan.target);
    const files = plan.files.map(item => {
        const local = path.resolve(target, item.relative);
        assert.ok(local.startsWith(target + path.sep), 'Archivo fuera del corpus');
        const stat = fs.statSync(local);
        assert.ok(stat.isFile());
        assert.equal(stat.size, item.bytes);
        return {name:path.basename(local), webkitRelativePath:item.relative.replace(/\\/g, '/'),
            size:stat.size, lastModified:Math.floor(stat.mtimeMs)};
    });
    assert.equal(files.length, 1000);
    assert.equal(files.reduce((n,f)=>n+f.size,0), 44859102);
    return {source:'EXISTING_LOCAL_CORPUS_METADATA_ONLY', files};
}
async function baseline(count) {
    const start = performance.now(), initialHeap = process.memoryUsage().heapUsed;
    let sampledHeap = initialHeap, reads = 0, snapshots = 0, effects = 0, failure = null;
    const perSnapshot = 2 + Math.ceil(1/25) + Math.ceil(count/25);
    const state = {loteId:'34',nivelRef:'7',perfilId:'28',versionId:'43',rutaCanonica:'SYNTHETIC',
        estado:'RECIBIENDO',revision:1,totalRaices:1,totalArchivos:count,
        detencionSolicitada:false,hayIntentosActivos:false,hayIncertidumbre:false,
        accionesPermitidas:['CONSULTAR','CONSULTAR_RAICES','SOLICITAR_AVANCE']};
    const parent = {recursoId:'52',idCliente:'r2',nombre:'Synthetic',estado:'PENDIENTE',
        revision:1,recepcionListaParaCrear:false,tieneIntentoActivo:false};
    const files = Array.from({length:count},(_,i)=>({recursoId:String(78+i),idCliente:'a'+i,
        raizId:'52',nombre:i+'.pdf',estadoRecepcion:'PENDIENTE',estado:'NO_INICIADO',
        revision:1,valoresEfectivosFijados:false,tieneIntentoActivo:false}));
    function sample() { sampledHeap=Math.max(sampledHeap,process.memoryUsage().heapUsed); }
    const engine = core.crearCoordinacion({vigente:()=>true,original:()=>true,
        fallar:error=>{failure=error.codigo;},
        leer:async consume=>{
            // Reproduce el coste HTTP de readAll; no hace solicitudes.
            for(let i=0;i<perSnapshot;i++){consume();reads++;}
            snapshots++;sample();
            return structuredClone({estado:state,raices:[parent],archivos:files});
        },
        ejecutar:async op=>{
            effects++;
            const row = op.tipo==='RAICES'?parent:files.find(f=>f.recursoId===op.row.recursoId);
            const result={loteId:'34',fase:op.fase,estado:'CONFIRMADO'};
            if(op.tipo==='RAICES') {
                Object.assign(row,{estado:'EXPEDIENTE_CONFIRMADO',expedienteId:'100',relacionNivelId:'200'});
                Object.assign(result,{raizId:'52',expedienteId:'100',relacionNivelId:'200'});
            } else {
                result.archivoId=row.recursoId;
                if(op.fase==='RECEPCION') {
                    row.estadoRecepcion='RECIBIDO';result.estadoRecepcion='RECIBIDO';
                    if(files.every(f=>f.estadoRecepcion==='RECIBIDO'))parent.recepcionListaParaCrear=true;
                }
                if(op.fase==='MATERIALIZAR_VALORES')row.valoresEfectivosFijados=true;
                if(op.fase==='ALMACENAR') {
                    Object.assign(row,{estado:'COMPLETADO',documentoId:String(300+files.indexOf(row)),gabineteId:'10'});
                    Object.assign(result,{documentoId:row.documentoId,gabineteId:'10'});
                }
            }
            row.revision++;state.revision++;
            state.estado=files.every(f=>f.estado==='COMPLETADO')?'COMPLETADO':'EN_CURSO';
            sample();return result;
        }});
    await engine.iniciar();
    const summary = engine.estado();
    if(count===1000) {
        assert.equal(summary.codigo,'COORDINACION_LIMITE');assert.equal(effects,232);
        assert.equal(reads,10000);assert.equal(summary.recibidos,231);
        assert.equal(files.filter(f=>f.estadoRecepcion==='RECIBIDO').length,232);
    } else {
        assert.equal(summary.completo,true);assert.equal(effects,3*count+1);
        assert.equal(reads,perSnapshot*(effects+1));
    }
    return {files:count,roots:1,readsPerSnapshot:perSnapshot,
        derivedEffectsForCompleteRun:3*count+1,derivedReadsForCompleteRun:perSnapshot*(3*count+2),
        observed:{status:summary.estado,code:summary.codigo,syntheticEffects:effects,
            simulatedAcceptedReads:reads,completedSnapshots:snapshots,receivedInLastSnapshot:summary.recibidos,
            simulatedServerReceipts:files.filter(f=>f.estadoRecepcion==='RECIBIDO').length,
            stored:summary.guardados,elapsedMs:Math.round(performance.now()-start),
            sampledHeapIncreaseBytes:Math.max(0,sampledHeap-initialHeap)},
        limits:engine.limites};
}
async function main() {
    const args=process.argv.slice(2);
    if(args.length && (args.length!==2 || args[0]!=='--plan'))throw Error('Uso: node measure-doc120-client-capacity.cjs [--plan archivo.json]');
    const selected=metadata(args[1]), inventory=inventoryModule.crear();
    inventory.agregarSeleccion(selected.files);
    const manifest=inventory.manifiesto(), request={nivelRef:'7',versionId:'43',...manifest};
    const serialized=JSON.stringify({solicitud:request});let xhr=0,preflight;
    const api=apiModule.crear({url:'/synthetic.asmx',uploadUrl:'/synthetic.ashx',token:'synthetic',
        maximoJsonSolicitud:102400,xhrFactory:()=>{xhr++;throw Error('Transporte no permitido');}});
    try {await api.invocar('PrepararPlan',request);assert.fail('Debe rechazar el cuerpo grande');}
    catch(error){assert.equal(error.codigo,'REVISION_TAMANO_EXCEDIDO');preflight=error.codigo;}
    assert.equal(xhr,0);
    const result={kind:'DOC120_OFFLINE_CAPACITY_BASELINE',generatedUtc:new Date().toISOString(),
        node:process.version,platform:process.platform,metadata:{source:selected.source,
            roots:manifest.raices.length,files:manifest.archivos.length,
            bytes:selected.files.reduce((n,f)=>n+f.size,0),requestUtf16Characters:serialized.length,
            requestUtf8Bytes:Buffer.byteLength(serialized),exampleAsmxLimit:102400,
            effectiveIisConfiguration:'NOT_INSPECTED',preflight,xhrCreated:xhr},
        sourceHashes:Object.fromEntries(sources.map(file=>[file,digest(path.join(root,file))])),
        scenarios:[],applicationHttp:0,databaseSelect:0,authenticatedSessions:0,realEffects:0,
        limitations:['El coste HTTP de readAll se modela; no se ejecuta su transporte.',
            'Tiempos y memoria corresponden a Node con dobles; no a navegador, IIS, SQL ni PDF.',
            'Heap muestreado no es pico exacto ni presupuesto de memoria del servidor.',
            '102400 es el limite ASMX de referencia, no una inspeccion de IIS.']};
    for(const count of [1,100,1000])result.scenarios.push(await baseline(count));
    process.stdout.write(JSON.stringify(result,null,2)+'\n');
}
main().catch(error=>{
    // Solo categoria y linea del auxiliar: no valores de archivo ni mensajes libres.
    const location=String(error.stack||'').match(/measure-doc120-client-capacity\.cjs:\d+:\d+/);
    process.stderr.write(JSON.stringify({code:'DOC120_OFFLINE_BASELINE_FAILED',
        category:error.name,location:location?location[0]:null})+'\n');process.exitCode=1;
});
