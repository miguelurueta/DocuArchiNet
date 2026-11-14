'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),path=require('node:path'),fs=require('node:fs');
const core=require('../js/gestion/ImportacionCarpetas/ImportacionCarpetas.js');
const {fixture,consult}=require('./support/doc120-importacion-browser.cjs');
const out=path.resolve(__dirname,'../.tmp/doc120-retomar');fs.mkdirSync(out,{recursive:true});
function policy(){const row={recursoId:'60',raizId:'60',revision:2,estado:'CREANDO',tieneIntentoActivo:true,intentoActivoId:'96',puedeRetomarCreacion:true};const model={loteId:'39',estado:{loteId:'39',estado:'EN_CURSO',revision:5,hayIncertidumbre:false,accionesPermitidas:['CONSULTAR','SOLICITAR_REANUDACION']},raices:{loteId:'39',tipo:'RAICES',revisionLectura:5,elementos:[row]}};return{model,row};}
test('DOC120-RSM01: acción permanente requiere pista estricta, página/lote vigentes y reserva visible',()=>{
 let {model,row}=policy();assert.deepEqual(core.accionesRecurso(model,row,true),['RETOMAR_CREACION']);assert.equal(core.bloqueoFlujo(model).retomable,true);
 for(const value of [false,undefined,1,'true']){({model,row}=policy());row.puedeRetomarCreacion=value;assert.deepEqual(core.accionesRecurso(model,row,true),[]);}
 for(const change of [p=>p.model.raices.revisionLectura=6,p=>p.model.raices.loteId='40',p=>p.model.estado.loteId='40',p=>p.model.estado.hayIncertidumbre=true,p=>p.model.busy=true,p=>p.model.mutacionIncierta=true,p=>p.row.estado='INCIERTO',p=>p.row.expedienteId='100',p=>p.row.tieneIntentoActivo=false]){const p=policy();change(p);assert.deepEqual(core.accionesRecurso(p.model,p.row,true),[]);}
 ({model,row}=policy());assert.deepEqual(core.accionesRecurso(model,row,false),[]);row.estado='INCIERTO';model.estado.accionesPermitidas=['SOLICITAR_VERIFICACION','SOLICITAR_REANUDACION'];assert.deepEqual(core.accionesRecurso(model,row,true),['RECONCILIAR']);
});
async function install(page,outcome='CONFIRMED'){
 await page.evaluate(outcome=>{window.resumeStage='RESERVED';window.resumeOutcome=outcome;window.resumeDialogs=[];window.resumeAccept=false;window.confirm=text=>{resumeDialogs.push(text);return resumeAccept;};window.received=true;const original=XMLHttpRequest.prototype.send;
 XMLHttpRequest.prototype.send=function(body){const request=JSON.parse(body).solicitud,op=this.operation;
  if(op==='AvanzarLote'){
   calls.push({operation:op,request});window.resumeStage=window.resumeOutcome==='LOSS'?'STARTED':'CONFIRMED';
   if(window.resumeOutcome==='LOSS'){queueMicrotask(()=>this.onerror());return;}
   this.status=200;this.responseText=JSON.stringify({ok:true,codigo:'OK',mensaje:'Sintético',incidencias:[],datos:{loteId:'39',fase:'RETOMAR_CREACION',estado:'CONFIRMADO',codigo:'EXPEDIENTE_CONFIRMADO',raizId:'60',intentoId:'96',expedienteId:'100',relacionNivelId:'101'}});queueMicrotask(()=>this.onload());return;
  }
  if(op==='ConsultarLote'||op==='ConsultarRecursosLote'){
   const callback=this.onload;this.onload=()=>{const response=JSON.parse(this.responseText),data=response.datos,created=window.resumeStage==='CONFIRMED',reserved=window.resumeStage==='RESERVED';
    if(op==='ConsultarLote')Object.assign(data,{estado:'EN_CURSO',revision:created?8:5,detencionSolicitada:reserved,hayIntentosActivos:!created,hayIncertidumbre:false,accionesPermitidas:['CONSULTAR','CONSULTAR_RAICES','EXPORTAR_PAGINA',created?'SOLICITAR_AVANCE':'SOLICITAR_REANUDACION']});
    else{data.revisionLectura=created?8:5;if(request.tipo==='RAICES'){for(const r of data.elementos)Object.assign(r,{recursoId:'60',raizId:'60',revision:created?4:2,estado:created?'EXPEDIENTE_CONFIRMADO':'CREANDO',tieneIntentoActivo:!created,intentoActivoId:created?null:'96',puedeRetomarCreacion:reserved,expedienteId:created?'100':null,relacionNivelId:created?'101':null});if(request.estado==='PENDIENTE'){data.elementos=[];data.totalFiltrado=0;}}
     else for(const f of data.elementos)Object.assign(f,{recursoId:'86',raizId:'60',estadoRecepcion:'RECIBIDO',estado:'NO_INICIADO',tieneIntentoActivo:false,valoresEfectivosFijados:false});}
    this.responseText=JSON.stringify(response);callback();};
  }original.call(this,body);
 };
 },outcome);await consult(page,'39');await page.getByRole('button',{name:'Retomar creación 60',exact:true}).waitFor();
}
test('DOC120-RSM02: cancelar no envía; confirmar por teclado retoma una fase y muestra índices sin encadenar',async()=>fixture(async page=>{
 await install(page);assert.match(await page.locator('.doc111-next').textContent(),/Puedes retomar/);
 for(const width of [390,1280]){await page.setViewportSize({width,height:844});await page.locator('.doc111-scroll').evaluate(el=>el.scrollTop=100000);const box=await page.getByRole('button',{name:'Retomar creación 60',exact:true}).boundingBox();assert.ok(box&&box.y>=0&&box.y+box.height<844);await page.screenshot({path:path.join(out,'retomar-'+width+'.png')});}
 const before=await page.evaluate(()=>calls.length);await page.getByRole('button',{name:'Retomar creación 60',exact:true}).focus();await page.keyboard.press('Enter');assert.equal(await page.evaluate(()=>calls.length),before);assert.match(await page.evaluate(()=>resumeDialogs[0]),/levantará la detención/);
 await page.evaluate(()=>resumeAccept=true);await page.getByRole('button',{name:'Retomar creación 60',exact:true}).click();await page.getByRole('button',{name:'Preparar índices 86',exact:true}).waitFor().catch(async error=>{fs.writeFileSync(path.join(out,'transition-diagnostic.json'),JSON.stringify({body:await page.locator('body').textContent(),calls:await page.evaluate(()=>calls)},null,2));throw error;});
 const effects=await page.evaluate(()=>calls.filter(c=>c.operation==='AvanzarLote'));assert.equal(effects.length,1);assert.equal(effects[0].request.fase,'RETOMAR_CREACION');assert.equal(effects[0].request.intentoId,'96');assert.equal(effects[0].request.revisionLoteEsperada,5);assert.equal(effects[0].request.revisionEsperada,2);assert.equal(effects[0].request.levantarDetencion,true);assert.equal(effects[0].request.recursoId,'60');
 assert.equal(await page.evaluate(()=>calls.some(c=>['ConfirmarPlan','PrepararPlan','RecibirArchivo'].includes(c.operation))),false);assert.equal(await page.getByRole('button',{name:'Retomar creación 60',exact:true}).count(),0);
}));
test('DOC120-RSM03: filtros que ocultan la reserva no conceden reanudación desde el archivo',async()=>fixture(async page=>{
 await install(page);await page.locator('[data-doc111-panel="RAICES"]>summary').click();await page.getByLabel('Estado de carpeta',{exact:true}).selectOption('PENDIENTE');await page.getByRole('button',{name:'Aplicar filtros de carpetas',exact:true}).click();await page.waitForFunction(()=>document.querySelector('.doc111-next')?.textContent.includes('fuera de esta página'));
 assert.equal(await page.locator('[data-doc111-action="fase"]').count(),0);assert.equal(await page.evaluate(()=>calls.filter(c=>c.operation==='AvanzarLote').length),0);
}));
test('DOC120-RSM04: respuesta perdida exige consulta y reapertura conserva bloqueo sin replay',async()=>fixture(async page=>{
 await install(page,'LOSS');await page.evaluate(()=>resumeAccept=true);await page.getByRole('button',{name:'Retomar creación 60',exact:true}).click();await page.waitForFunction(()=>document.querySelector('.doc111-notice')?.textContent.includes('No se confirmó la creación'));
 assert.equal(await page.locator('[data-doc111-action="fase"]').count(),0);await page.getByRole('button',{name:'Actualizar estado',exact:true}).click();await page.getByRole('button',{name:'Revisar bloqueo',exact:true}).waitFor();assert.equal(await page.locator('[data-doc111-action="fase"]').count(),0);assert.equal(await page.evaluate(()=>calls.filter(c=>c.operation==='AvanzarLote').length),1);
 await page.getByRole('button',{name:'Cerrar',exact:true}).click();await page.locator('[data-doc111-resume]').click();await page.getByRole('button',{name:'Revisar bloqueo',exact:true}).waitFor();assert.equal(await page.evaluate(()=>calls.filter(c=>c.operation==='AvanzarLote').length),1);
}));
