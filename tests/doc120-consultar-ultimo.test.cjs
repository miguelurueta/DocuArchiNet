'use strict';
const test=require('node:test'),assert=require('node:assert/strict');
const {fixture,select,click}=require('./support/doc120-coordinacion-browser.cjs');
test('LAST01: consultar último lote desde los accesos del asistente cambia la selección abierta a consulta y permite volver',()=>fixture(async page=>{
 await select(page,1,1);await click(page,'confirmar-importar');await page.getByRole('heading',{name:'Importación completada',exact:true}).waitFor();await page.getByRole('button',{name:'Cerrar',exact:true}).first().click();
 await page.locator('[data-doc111-open]').click();await page.getByLabel('Perfil publicado').waitFor();await select(page,1,2);
 const before=await page.evaluate(()=>calls.filter(c=>c.operation==='ConsultarLote').length);
 await click(page,'consultar-ultimo');await page.getByRole('heading',{name:'Consultar importación',exact:true}).waitFor();
 assert.ok(await page.evaluate(()=>calls.filter(c=>c.operation==='ConsultarLote').length)>before);
 assert.equal(await page.getByRole('button',{name:'Volver a mi importación',exact:true}).isVisible(),true);
 await click(page,'volver-importacion');await page.getByRole('button',{name:'Confirmar e importar',exact:true}).waitFor();
 assert.equal(await page.evaluate(()=>calls.filter(c=>c.operation==='ConfirmarPlan').length),1);
}));

async function eraseLocalReference(page){
 await page.evaluate(()=>{const host=document.getElementById('doc111-importacion');delete host.doc111UltimoLote;host.doc111Instancia.sincronizar();});
}
async function knownLot(page){
 await select(page,1,1);await click(page,'confirmar-importar');await page.getByRole('heading',{name:'Importación completada',exact:true}).waitFor();await page.getByRole('button',{name:'Cerrar',exact:true}).first().click();await eraseLocalReference(page);
}
test('LAST02: sin referencia ni perfil, el botón busca en servidor y consulta el mismo lote sin efectos',()=>fixture(async page=>{
 await knownLot(page);await page.locator('[data-doc111-open]').click();await page.getByLabel('Perfil publicado').waitFor();
 await page.evaluate(()=>{window.doc120ExtraReply=op=>op==='ConsultarUltimoLote'?{encontrado:true,loteId:'34'}:undefined;});
 assert.equal(await page.getByLabel('Perfil publicado').inputValue(),'');const button=page.locator('.doc111-shortcuts').getByRole('button',{name:'Consultar último lote',exact:true});assert.equal(await button.isEnabled(),true);
 const before=await page.evaluate(()=>calls.filter(c=>['ConfirmarPlan','RECEPCION','AvanzarLote'].includes(c.operation)).length);
 await button.click();await page.getByRole('heading',{name:'Importación finalizada',exact:true}).waitFor();
 assert.deepEqual(await page.evaluate(()=>calls.filter(c=>c.operation==='ConsultarUltimoLote').map(c=>c.request)),[{}]);
 assert.equal(await page.evaluate(()=>calls.filter(c=>['ConfirmarPlan','RECEPCION','AvanzarLote'].includes(c.operation)).length),before);
 await click(page,'volver-importacion');await page.getByLabel('Perfil publicado').waitFor();assert.equal(await page.getByLabel('Perfil publicado').inputValue(),'');
}));
test('LAST03: menú sin referencia abre directamente la búsqueda, incluso sin nivel seleccionado',()=>fixture(async page=>{
 await knownLot(page);await page.evaluate(()=>{document.getElementById('doc111-importacion').setAttribute('data-nivel','');window.doc120ExtraReply=op=>op==='ConsultarUltimoLote'?{encontrado:true,loteId:'34'}:undefined;});
 await page.locator('[data-doc111-resume]').click();await page.getByRole('heading',{name:'Importación completada',exact:true}).waitFor();assert.equal(await page.evaluate(()=>calls.filter(c=>c.operation==='ConsultarUltimoLote').length),1);
}));
for(const mode of ['empty','unsupported','denied','closed'])test('LAST04 '+mode+': ausencia, servidor anterior, denegación y cierre no seleccionan otro lote ni ejecutan efectos',()=>fixture(async page=>{
 await page.evaluate(mode=>{window.releaseLast=null;window.doc120ExtraReply=op=>{if(op!=='ConsultarUltimoLote')return;if(mode==='empty')return{encontrado:false,loteId:null};if(mode==='unsupported')return Promise.reject({status:500,codigo:'RESPUESTA_INVALIDA'});if(mode==='denied')return Promise.reject({status:403,codigo:'ACCESO_DENEGADO'});return new Promise(resolve=>window.releaseLast=()=>resolve({encontrado:true,loteId:'34'}));};},mode);
 assert.equal(await page.locator('.doc111-shortcuts').getByRole('button',{name:'Consultar último lote',exact:true}).isEnabled(),true);await click(page,'consultar-ultimo');
 if(mode==='closed'){await page.waitForFunction(()=>!!releaseLast);await page.getByRole('button',{name:'Cerrar',exact:true}).click();await page.evaluate(()=>releaseLast());assert.equal(await page.locator('.doc111-overlay').count(),0);}
 else {const message=mode==='empty'?/No hay importaciones confirmadas/:mode==='unsupported'?/Usa Consultar importación/:/No tiene autorización/;await page.locator('.doc111-sheet>.doc111-notice').filter({hasText:message}).waitFor();assert.equal(await page.locator('[data-doc111-action="abrir-consulta"]').first().isEnabled(),true);}
 assert.equal(await page.evaluate(()=>calls.filter(c=>['ConsultarLote','RECEPCION','AvanzarLote','ConfirmarPlan'].includes(c.operation)).length),0);
}));
