const test=require('node:test');
const assert=require('node:assert/strict');
const path=require('node:path');
const {chromium}=require('../tools/e2e/node_modules/playwright');
const client=require('../js/gestion/ImportacionCarpetas/ImportacionCarpetas.js');
const root=path.resolve(__dirname,'..');

test('DOC120-REC01: creación exige la recepción confirmada por servidor, aun con página visible recibida',()=>{
  const model={estado:{accionesPermitidas:['SOLICITAR_AVANCE']},resultados:{elementos:[{estadoRecepcion:'RECIBIDO'}]}};
  const pending={recursoId:'52',estado:'PENDIENTE',tieneIntentoActivo:false};
  for(const value of [undefined,null,false,1,'true'])assert.deepEqual(client.accionesRecurso(model,{...pending,recepcionListaParaCrear:value},true),[]);
  const ready={...pending,recepcionListaParaCrear:true};
  assert.deepEqual(client.accionesRecurso(model,ready,true),['CREAR_EXPEDIENTE']);
  for(const change of [m=>m.busy=true,m=>m.mutacionIncierta=true,m=>m.estado.accionesPermitidas=[]]){
    const changed=structuredClone(model);change(changed);assert.deepEqual(client.accionesRecurso(changed,ready,true),[]);
  }
  assert.deepEqual(client.accionesRecurso(model,{...ready,tieneIntentoActivo:true},true),[]);
});

test('DOC120-REC01: guía visible y recepción explícita habilitan creación sin confirmación ni replay (Chromium sintético)',async()=>{
  const browser=await chromium.launch({headless:true});
  try{
    for(const [mode,scope] of [['COMPLETO_OBLIGATORIO','EXPEDIENTE'],['COMPLETO_OBLIGATORIO','LOTE'],['PARCIAL_PERMITIDO','NO_APLICA']]){
      const page=await browser.newPage({viewport:{width:1280,height:900}}),errors=[],network=[];
      page.setDefaultTimeout(5000);page.on('pageerror',e=>errors.push(e.message));page.on('request',r=>{if(/^https?:/.test(r.url()))network.push(r.url());});
      await page.setContent('<html lang="es"><body><div id="doc111-importacion" data-api="/synthetic.asmx" data-upload="/synthetic.ashx" data-token="synthetic-only" data-nivel="7"><button data-doc111-open>Importar carpetas</button><button data-doc111-resume>Consultar último lote</button><span data-doc111-status></span></div></body></html>');
      await page.addStyleTag({path:path.join(root,'js/gestion/ImportacionCarpetas/ImportacionCarpetas.css')});
      await page.evaluate(({mode,scope})=>{
        document.getElementById('doc111-importacion').doc111UltimoLote={loteId:'31'};
        window.received=false;window.created=false;window.calls=[];window.stop=false;window.fixed=false;window.stored=false;
        window.XMLHttpRequest=class{
          constructor(){this.upload={};}open(_method,url){this.url=url;}setRequestHeader(){}abort(){if(this.onabort)this.onabort();}
          send(body){
            const operation=this.url.split('/').pop(),request=body instanceof FormData?Object.fromEntries([...body.entries()].filter(([key])=>key!=='file')):JSON.parse(body).solicitud;
            window.calls.push({operation,request});
            queueMicrotask(()=>{
              if(!this.onload)return;let data;
              if(operation==='ConsultarLote')data={loteId:'31',nivelRef:'7',rutaCanonica:'Destino sintético',estado:'RECIBIENDO',revision:1,maximoTamanoPagina:25,totalRaices:1,totalArchivos:1,totalBytes:'7',modoIntegridad:mode,ambitoIntegridad:scope,accionesPermitidas:['CONSULTAR','CONSULTAR_RAICES',...(!window.stop?['SOLICITAR_AVANCE']:[])]};
              else if(operation==='ConsultarRecursosLote')data={loteId:'31',tipo:request.tipo,pagina:1,tamanoPagina:25,totalOriginal:1,totalFiltrado:1,elementos:request.tipo==='RAICES'?[{recursoId:'52',estado:window.created?'EXPEDIENTE_CONFIRMADO':'PENDIENTE',revision:1,nombre:'Carpeta de prueba',recepcionListaParaCrear:window.received,tieneIntentoActivo:false,...(window.created?{expedienteId:'60',relacionNivelId:'70'}:{})}]:[{recursoId:'78',raizId:'52',idCliente:'a1',nombre:'10.pdf',rutaRelativa:'Carpeta de prueba/10.pdf',ultimaModificacionLocal:'0',bytesPrevistos:'7',estadoRecepcion:window.received?'RECIBIDO':'PENDIENTE',estado:window.stored?'COMPLETADO':'NO_INICIADO',revision:1,valoresEfectivosFijados:window.fixed,tieneIntentoActivo:false}]};
              else if(operation==='synthetic.ashx'){
                if(window.received)throw Error('REPLAY');window.received=true;data={loteId:'31',archivoId:'78',estadoRecepcion:'RECIBIDO'};
              }else if(operation==='AvanzarLote'){
                if(!window.received)throw Error('CREATED_BEFORE_RECEIPT');
                if(request.fase==='CREAR_EXPEDIENTE')window.created=true;
                else if(request.fase==='MATERIALIZAR_VALORES')window.fixed=true;
                else if(request.fase==='ALMACENAR')window.stored=true;
                data={loteId:'31',fase:request.fase,estado:'CONFIRMADO',raizId:'52',archivoId:'78'};
              }else throw Error('UNAUTHORIZED_SYNTHETIC_OPERATION '+operation);
              this.status=200;this.responseText=JSON.stringify({ok:true,codigo:'OK',mensaje:'Sintético',incidencias:[],datos:data});this.onload();
            });
          }
        };
      },{mode,scope});
      for(const name of ['InventarioImportacion','ImportacionCarpetasApi','ImportacionCarpetasVista','ImportacionCarpetas'])await page.addScriptTag({path:path.join(root,`js/gestion/ImportacionCarpetas/${name}.js`)});
      await page.getByRole('button',{name:'Consultar lote 31',exact:true}).click();
      const send=page.getByRole('button',{name:'Enviar pendientes de esta página',exact:true});
      await send.waitFor();assert.equal(await send.count(),1);assert.equal(await send.isDisabled(),true);
      assert.equal(await page.getByRole('button',{name:'Crear expediente 52',exact:true}).count(),0);
      const hint=mode==='PARCIAL_PERMITIDO'?'Recibe al menos un archivo elegible de esta carpeta':scope==='LOTE'?'Completa la recepción de los archivos del lote':'Completa la recepción de los archivos de esta carpeta';
      assert.equal(await page.getByText(hint,{exact:false}).count(),1);
      assert.equal(await send.evaluate(button=>!!(button.compareDocumentPosition(document.querySelector('[data-doc111-filtros]'))&Node.DOCUMENT_POSITION_FOLLOWING)),true);
      await page.setViewportSize({width:390,height:844});
      assert.equal(await page.evaluate(()=>document.documentElement.scrollWidth<=innerWidth),true);
      await page.setViewportSize({width:1280,height:900});
      assert.equal(await page.evaluate(()=>calls.some(c=>c.operation==='AvanzarLote'||c.operation==='synthetic.ashx')),false);
      await page.getByRole('button',{name:'Reseleccionar archivo 78',exact:true}).click();
      await page.evaluate(()=>{const transfer=new DataTransfer();transfer.items.add(new File(['example'],'10.pdf',{type:'application/pdf',lastModified:0}));const input=document.querySelector('input[aria-label="Reseleccionar archivo pendiente"]');input.files=transfer.files;input.dispatchEvent(new Event('change',{bubbles:true}));});
      assert.equal(await send.isEnabled(),true);await send.click();
      const create=page.getByRole('button',{name:'Crear expediente 52',exact:true});await create.waitFor();
      assert.equal(await send.isDisabled(),true);await create.click();
      await page.getByRole('button',{name:'Preparar índices 78',exact:true}).click();
      await page.getByRole('button',{name:'Almacenar archivo 78',exact:true}).click();
      await page.waitForFunction(()=>stored&&!document.querySelector('.doc111-content [data-doc111-action="fase"]'));
      const mutations=await page.evaluate(()=>calls.filter(c=>['synthetic.ashx','AvanzarLote','ConfirmarPlan'].includes(c.operation)));
      assert.deepEqual(mutations.map(c=>c.operation==='AvanzarLote'?c.request.fase:c.operation),['synthetic.ashx','CREAR_EXPEDIENTE','MATERIALIZAR_VALORES','ALMACENAR']);
      assert.deepEqual(errors,[]);assert.deepEqual(network,[]);await page.close();
    }
  }finally{await browser.close();}
});
