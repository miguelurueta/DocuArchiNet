/* Prototipo autónomo: sin fetch, servicios, persistencia ni lectura de contenido.
 * La selección local solo inventaría metadatos. Los IDs y efectos documentales son simulados.
 * En ASP.NET, el servicio de importación debe reemplazar el validador y ejecutor de demostración.
 */
(() => {
  'use strict';
  const $ = id => document.getElementById(id);
  const escape = value => String(value ?? '').replace(/[&<>"']/g, char => ({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[char]));
  const profiles = {
    orders: {name:'Órdenes de trabajo',version:'3',code:'IMP-OT',series:'Órdenes de trabajo',subseries:'Órdenes de compra',gabinet:'Documentos de compras',types:{125:'Orden de compra',130:'Acta de entrega'},extensions:['pdf','jpg','png'],mode:'segments',rule:'Número - código de tipología - descripción',description:'Extrae los índices y la tipología del nombre del archivo.'},
    general: {name:'Expedientes administrativos',version:'1',code:'IMP-ADM',series:'Gestión administrativa',subseries:'No aplica',gabinet:'Documentos administrativos',types:{200:'Documento administrativo'},extensions:['pdf','jpg','png','docx'],mode:'whole',rule:'Nombre completo sin extensión',description:'Usa el nombre como descripción y una tipología predeterminada.'}
  };
  const destinations = {
    'nivel-demo-101': {name:'Gestión de compras',path:'Archivo de gestión / Área administrativa / Gestión de compras'},
    'nivel-demo-102': {name:'Correspondencia',path:'Archivo de gestión / Área administrativa / Correspondencia'}
  };
  const state = {step:1,profile:'orders',destination:null,batchDestination:null,roots:[],files:[],confirmed:false,scenario:'normal',running:false,paused:false,cancelled:false,timer:null,page:0,query:'',filter:'all',batch:null};
  let sequence=0;
  let selectedTreeLevel=null;
  const id = prefix => prefix + '-' + (++sequence);
  const profile = () => profiles[state.profile];
  const destination = () => state.step===4 ? state.batchDestination : destinations[state.destination];
  const bytes = n => n >= 1048576 ? (n/1048576).toLocaleString('es',{maximumFractionDigits:1})+' MB' : Math.max(1,Math.round(n/1024))+' KB';
  const tag = (label,kind='neutral') => `<span class="tag ${kind}">${escape(label)}</span>`;
  const total = () => state.files.reduce((sum,f) => sum+f.size,0);
  const errors = () => state.files.filter(f=>f.errors.length);
  const complete = () => state.files.filter(f=>f.status==='done').length;
  const rootOf = f => state.roots.find(r=>r.id===f.root);
  function notice(message,good=false){$('notice').textContent=message;$('notice').hidden=!message;$('notice').classList.toggle('good',good);}
  function dialog(title,html){$('dialog-title').textContent=title;$('dialog-content').innerHTML=html;$('detail-dialog').showModal();}
  const newFile = (root,name,path,size,file=null) => ({id:id('archivo'),root,name,path,size,file,errors:[],values:{},status:'pending',documentId:null,uncertaintyShown:false});
  function validate(){
    const p=profile();
    for(const f of state.files){
      const ext=f.name.includes('.')?f.name.split('.').pop().toLowerCase():'';
      const stem=f.name.replace(/\.[^.]*$/,'');
      f.errors=[]; f.values={description:stem,order:'—',type:''};
      if(!p.extensions.includes(ext)) f.errors.push('Formato no permitido por este perfil.');
      if(f.size>25*1048576) f.errors.push('Supera el límite ilustrativo de 25 MB por archivo.');
      if(f.size===0) f.errors.push('El archivo está vacío.');
      if(f.path.startsWith('/') || /(^|[\\/])\.\.([\\/]|$)/.test(f.path) || /[:\u0000]/.test(f.path)) f.errors.push('La ruta relativa no es válida.');
      const root=rootOf(f);
      if(root && state.roots.some(r=>r.id!==root.id && r.name.toLocaleLowerCase()===root.name.toLocaleLowerCase())) f.errors.push('Otra raíz tiene el mismo nombre. Requiere resolver la coincidencia; no se fusionará.');
      if(p.mode==='segments'){
        const parts=stem.split('-');
        f.values={order:parts[0]||'—',type:p.types[parts[1]]||'',description:parts.slice(2).join('-')};
        if(parts.length<3 || !parts[0] || !parts.slice(2).join('-').trim()) f.errors.push('Nombre incompatible con la regla de este perfil.');
        if(!p.types[parts[1]]) f.errors.push('Código de tipología no encontrado en el catálogo de ejemplo.');
      }else f.values.type=p.types[200];
      if(state.scenario==='conflict' && root===state.roots[0]) f.errors.push('Escenario: posible expediente existente. La política exige resolver el conflicto.');
    }
  }
  function invalidate(){state.confirmed=false;state.page=0;validate();}
  function renderSummary(){
    const p=profile(); $('summary-name').textContent=p.name;
    $('summary-content').innerHTML=`<div>${tag(p.code+' · versión '+p.version,'info')}</div><dl>
      <div><dt>Fondo / instrumento</dt><dd>Organización de ejemplo<small>TRD · versión de demostración</small></dd></div>
      <div><dt>Área / serie</dt><dd>Área administrativa<small>${escape(p.series)}</small></dd></div>
      <div><dt>Subserie</dt><dd>${escape(p.subseries)}</dd></div>
      <div><dt>Gabinete</dt><dd>${escape(p.gabinet)}</dd></div>
      <div><dt>Estructura de Producción · nivel padre</dt><dd>${destination()?escape(destination().path):'Pendiente de seleccionar'}<small>Aquí se crearán los expedientes; el gabinete es el destino de sus documentos.</small></dd></div>
      <div><dt>Responsable</dt><dd>Responsable de ejemplo</dd></div>
      <div><dt>Estructura de carpetas</dt><dd>Una raíz → un expediente<small>Subcarpetas: incluir en el mismo expediente.</small></dd></div>
      <div><dt>Restricciones ilustrativas</dt><dd>${p.extensions.map(x=>x.toUpperCase()).join(', ')}<small>25 MB por archivo · 500 archivos en esta maqueta</small></dd></div></dl>`;
  }
  function render(){
    document.querySelectorAll('.steps li').forEach((el,i)=>{el.classList.toggle('active',i===state.step-1);el.classList.toggle('done',i<state.step-1);el.toggleAttribute('aria-current',i===state.step-1);if(i===state.step-1)el.setAttribute('aria-current','step');el.firstElementChild.textContent=i<state.step-1?'✓':String(i+1);});
    renderSummary();
    const headings=[['Selecciona un perfil de importación','Elige la configuración que corresponde a tus documentos.'],['Agrega las carpetas a importar','Cada carpeta seleccionada será un expediente propuesto.'],['Revisa antes de importar','Comprueba el destino, los datos resueltos y las incidencias.'],['Resultado de la importación','Sigue cada documento y distingue recepción de incorporación.']];
    $('section-title').textContent=headings[state.step-1][0];$('section-description').textContent=headings[state.step-1][1];$('stage-label').textContent=state.step===4?'Simulación':'Paso '+state.step+' de 4';
    $('back').hidden=state.step===1||state.step===4;
    $('next').textContent=['Continuar a carpetas →','Revisar importación →','Confirmar y simular →','Nueva importación'][state.step-1];
    $('next').disabled=(state.step<4&&!destinations[state.destination])||(state.step===2&&!state.files.length)||(state.step===3&&(!state.confirmed||errors().length>0||!state.files.length))||(state.step===4&&state.running);
    const footers=['Se usará una versión publicada del perfil.','Seleccionar archivos no transfiere su contenido.','La revisión del servidor está simulada en este prototipo.','Resultados de ejemplo. No existen cambios en la aplicación.'];
    $('footer-info').textContent=footers[state.step-1];
    if(state.step===1)renderProfile(); if(state.step===2)renderFolders(); if(state.step===3)renderReview(); if(state.step===4)renderResults();
    renderDestination();
  }
  function renderDestination(){
    if(state.step===1){
      const box=document.createElement('div');box.className='destination-select';
      box.innerHTML=`<div class="field">Nivel de Producción seleccionado</div><div class="destination-path">${destination()?escape(destination().path):'No se recibió un nivel válido'}</div><p class="hint">Destino recibido al abrir desde el árbol de Producción Documental. Solo lectura.</p><div class="tiny-note">Para importar en otro nivel, cierra esta ventana, selecciona la estructura correspondiente y vuelve a abrir la importación. No se crean estructuras desde aquí.</div>`;
      $('stage-content').querySelector('.body-pad').appendChild(box);
    }else if(destination()){
      const box=document.createElement('div');box.className='destination-preview';
      box.innerHTML=`<div class="eyebrow">${state.step===4?'ESTRUCTURA ASOCIADA AL LOTE SIMULADO':'UBICACIÓN PROPUESTA DE LOS EXPEDIENTES'}</div><strong>${escape(destination().path)}</strong><ul>${state.roots.slice(0,5).map(r=>`<li><span aria-hidden="true">↳ ▰</span> ${escape(r.name)} <small>${state.step===4?(r.expedientId?'Expediente simulado: '+escape(r.expedientId):'Sin creación confirmada'):'Nuevo expediente'}</small></li>`).join('')||'<li>Las carpetas que agregues aparecerán bajo este nivel.</li>'}</ul>${state.roots.length>5?`<small>Y ${state.roots.length-5} expedientes más en este mismo nivel.</small>`:''}<p>Los documentos se almacenan en «${escape(profile().gabinet)}» y se incorporan a su expediente. No se reproduce la estructura física como niveles de Producción.</p>`;
      $('stage-content').prepend(box);
    }
  }
  function renderProfile(){const p=profile();$('stage-content').innerHTML=`<div class="body-pad"><label class="field" for="profile">Perfil publicado <span class="muted">*</span></label><select id="profile">${Object.entries(profiles).map(([key,v])=>`<option value="${key}" ${key===state.profile?'selected':''}>${escape(v.name)} · versión ${v.version}</option>`).join('')}</select><p class="hint">Catálogo de demostración. En producción se muestran solo perfiles autorizados.</p><div class="info-box"><strong>${escape(p.description)}</strong><p>La configuración se aplica a todas las carpetas de esta importación. Los valores de cada archivo se revisan en el siguiente paso.</p></div><div class="config-grid"><div class="config-item"><span>NOMBRE DEL EXPEDIENTE</span><strong>Nombre de la carpeta raíz</strong></div><div class="config-item"><span>COINCIDENCIAS</span><strong>Reportar conflicto; no fusionar</strong></div><div class="config-item"><span>ORIGEN DE LOS ARCHIVOS</span><strong>Dispositivo del usuario</strong></div><div class="config-item"><span>DATOS DE CLASIFICACIÓN</span><strong>Configuración interna del perfil</strong></div></div></div><div class="panel-bottom"><span>La versión queda asociada al lote al confirmar.</span><button class="link-button" data-action="rules">Ver reglas del perfil</button></div>`;}
  function metrics(){return `<div class="metrics"><div class="metric"><strong>${state.roots.length}</strong><span>Expedientes propuestos</span></div><div class="metric"><strong>${state.files.length}</strong><span>Documentos seleccionados</span></div><div class="metric"><strong>${state.files.length?bytes(total()):'0 KB'}</strong><span>Tamaño del contenido</span></div></div>`;}
  function renderFolders(){
    $('stage-content').innerHTML=`<div class="toolbar"><button class="button primary" data-action="add">＋ Agregar carpeta</button><button class="button" data-action="example">Cargar ejemplo</button></div>${state.roots.length?metrics()+`<div class="table-wrap"><table><thead><tr><th>Carpeta / expediente propuesto</th><th>Archivos</th><th>Tamaño</th><th><span class="muted">Acción</span></th></tr></thead><tbody>${state.roots.map(r=>{const files=state.files.filter(f=>f.root===r.id);return `<tr><td><div class="file-label"><span class="root-icon" aria-hidden="true">▰</span><div><strong>${escape(r.name)}</strong><small>${r.demo?'Ejemplo':'Selección local'} · ${escape(r.id)}${files.some(f=>f.path.split('/').length>2)?' · incluye subcarpetas':''}</small></div></div></td><td>${files.length}</td><td>${bytes(files.reduce((s,f)=>s+f.size,0))}</td><td><button class="icon-button" data-action="remove-root" data-id="${r.id}" aria-label="Quitar carpeta ${escape(r.name)}">×</button></td></tr>`;}).join('')}</tbody></table></div>`:`<div class="empty-state"><div class="empty-icon" aria-hidden="true">▱</div><h3>Tus carpetas, organizadas en expedientes</h3><p>Agrega una carpeta y repite la selección para incluir más. Conservamos los nombres y la estructura relativa.</p><button class="button" data-action="example">Explorar con datos de ejemplo</button></div>`}<div class="body-pad"><p class="tiny-note">Las subcarpetas aportan sus documentos al expediente de su raíz. Las carpetas vacías no se detectan con este selector. No se lee el contenido ni se envían archivos.</p>${state.roots.length?'<button class="link-button" data-action="clear">Quitar todas las carpetas</button>':''}</div>`;
  }
  function filteredFiles(){return state.files.filter(f=>(!state.query||(f.name+' '+f.path).toLocaleLowerCase().includes(state.query.toLocaleLowerCase()))&&(state.filter==='all'||(state.filter==='errors'?f.errors.length>0:f.status===state.filter)));}
  function filterBar(result=false){return `<div class="search-row"><input id="search" type="search" placeholder="Buscar por nombre o ruta…" aria-label="Buscar documentos" value="${escape(state.query)}"><select id="filter" aria-label="Filtrar documentos"><option value="all">Todos los documentos</option>${result?'<option value="done">Completados</option><option value="uncertain">Por verificar</option><option value="cancelled">Cancelados</option>':'<option value="errors">Con incidencias</option>'}</select></div>`;}
  function pager(count){const max=Math.max(1,Math.ceil(count/8));return `<div class="pagination"><span>${count} documentos · Página ${state.page+1} de ${max}</span><div><button class="button small" data-action="page-prev" ${state.page===0?'disabled':''} aria-label="Página anterior">←</button> <button class="button small" data-action="page-next" ${(state.page+1)>=max?'disabled':''} aria-label="Página siguiente">→</button></div></div>`;}
  function fileTable(result=false){
    const all=filteredFiles();state.page=Math.min(state.page,Math.max(0,Math.ceil(all.length/8)-1));
    return `<div class="table-wrap"><table><thead><tr><th>Documento / expediente</th><th>${result?'Estado':'Tipología / validación'}</th><th>Detalle</th></tr></thead><tbody>${all.slice(state.page*8,state.page*8+8).map(f=>`<tr><td><div class="file-label"><span class="file-icon">${escape(f.name.split('.').pop().slice(0,4).toUpperCase())}</span><div><strong>${escape(f.name)}</strong><small>${escape(f.path)} · ${bytes(f.size)}</small>${result&&f.documentId?`<small>${escape(f.documentId)} · ${escape(rootOf(f).expedientId)}</small>`:''}</div></div></td><td>${result?statusTag(f):f.errors.length?tag(f.errors.length+' incidencia(s)','error'):`${tag('Válido','success')}<small>${escape(f.values.type)}</small>`}</td><td><button class="link-button" data-action="detail" data-id="${f.id}">Ver</button>${!result?` <button class="icon-button" data-action="remove-file" data-id="${f.id}" aria-label="Quitar archivo ${escape(f.name)}">×</button>`:''}</td></tr>`).join('')||'<tr><td colspan="3">No hay documentos para este filtro.</td></tr>'}</tbody></table></div>${pager(all.length)}`;
  }
  function renderReview(){
    const count=errors().length;
    $('stage-content').innerHTML=metrics()+`<div class="toolbar"><div>${count?tag(count+' documentos requieren atención','error'):tag('Revisión ilustrativa sin incidencias','success')}</div><button class="link-button" data-action="rules">Reglas aplicadas</button></div><div class="toolbar"><label for="scenario">Escenario de demostración</label><select id="scenario"><option value="normal">Recorrido normal</option><option value="conflict">Conflicto de expediente</option><option value="partial">Respuesta perdida al almacenar</option></select></div>${filterBar()}${fileTable()}<label class="checkline"><input id="confirm" type="checkbox" ${state.confirmed?'checked':''} ${count||!state.files.length?'disabled':''}><span>Revisé el nivel padre de Producción, los expedientes propuestos y el gabinete de documentos.<small>${count?'Resuelve las incidencias o quita los documentos afectados para continuar.':'Al continuar se ejecutará únicamente una simulación visual de la importación.'}</small></span></label>`;
    $('scenario').value=state.scenario;$('filter').value=state.filter;
  }
  function statusTag(f){const map={pending:['Pendiente','neutral'],receiving:['Recibiendo · simulado','info'],received:['Recibido · simulado','info'],storing:['Almacenando · simulado','info'],done:['Incorporado · simulado','success'],uncertain:['Resultado por verificar','warning'],cancelled:['Cancelado','neutral']};return tag(...(map[f.status]||map.pending));}
  function renderResults(){
    const done=complete(), uncertain=state.files.filter(f=>f.status==='uncertain').length;
    const pending=state.files.some(f=>['pending','receiving','received','storing'].includes(f.status));
    const label=state.running?'Procesando documentos':state.paused?'Simulación pausada':uncertain?'Hay resultados pendientes de verificar':state.cancelled?'Se cancelaron las operaciones pendientes':'Simulación finalizada';
    $('stage-content').innerHTML=`<div class="result-banner ${uncertain||state.cancelled||state.paused?'partial':''}"><strong>${label}</strong><p>Lote ${escape(state.batch)} · ${done} de ${state.files.length} documentos incorporados en la simulación.</p></div><div class="body-pad"><div class="progress-head"><strong>Incorporación confirmada</strong><span>${Math.round(done/state.files.length*100)||0}% · simulado</span></div><progress max="${Math.max(1,state.files.length)}" value="${done}" aria-label="Documentos incorporados en la simulación"></progress><div class="status-message">Recibir un archivo no significa que ya esté almacenado e incorporado.</div></div><div class="toolbar"><div class="toolbar-group">${state.running?'<button class="button" data-action="pause">Pausar simulación</button>':''}${state.paused&&pending?'<button class="button primary" data-action="resume">Continuar simulación</button>':''}${pending?'<button class="button danger" data-action="cancel">Cancelar pendientes</button>':''}${uncertain?'<button class="button" data-action="reconcile">Simular verificación del resultado</button>':''}</div><button class="button" data-action="export">Exportar resultados</button></div>${filterBar(true)}${fileTable(true)}<div class="panel-bottom"><span>${uncertain?'No se reenvía el archivo: primero se verifica si ya fue almacenado.':'Los IDs mostrados son ficticios. No se ejecutaron operaciones documentales.'}</span></div>`;
    $('filter').value=state.filter;
  }
  function showRules(){const p=profile();dialog('Reglas del perfil · '+p.name,`<p>${escape(p.rule)}. Esta configuración es ilustrativa, no un formato obligatorio del sistema.</p><table class="rule-table"><thead><tr><th>Destino</th><th>Origen</th></tr></thead><tbody><tr><td>Nombre del expediente</td><td>Nombre de la raíz seleccionada</td></tr><tr><td>Clasificación y responsable</td><td>Versión ${p.version} del perfil</td></tr><tr><td>Número de orden</td><td>${p.mode==='segments'?'Primer segmento, conservado como texto':'No aplica'}</td></tr><tr><td>Tipología</td><td>${p.mode==='segments'?'Segundo segmento: 125 → Orden de compra; 130 → Acta de entrega':'Constante: Documento administrativo'}</td></tr><tr><td>Descripción</td><td>${p.mode==='segments'?'Resto del nombre desde el tercer segmento':'Nombre completo sin extensión'}</td></tr></tbody></table><div class="info-box">Subcarpetas: incluir en la raíz. Coincidencias: reportar conflicto. Código del expediente: consecutivo generado por el servicio, simulado aquí.</div><p class="tiny-note">En implementación, las reglas y catálogos se consultan y validan en servidor. No se guardan dentro del JavaScript.</p>`);}
  function showDetail(fileId){const f=state.files.find(x=>x.id===fileId);if(!f)return;const r=rootOf(f);dialog('Detalle del documento',`<dl class="detail-grid"><div class="wide"><dt>Nombre original</dt><dd>${escape(f.name)}</dd></div><div class="wide"><dt>Ruta relativa / identidad de selección</dt><dd>${escape(f.path)}<br><small>${escape(f.id)} · ${escape(r.id)}</small></dd></div><div><dt>Expediente propuesto</dt><dd>${escape(r.name)}</dd></div><div><dt>Gabinete</dt><dd>${escape(profile().gabinet)}</dd></div><div><dt>Número de orden</dt><dd>${escape(f.values.order)}</dd></div><div><dt>Tipología resuelta</dt><dd>${escape(f.values.type||'Sin resolver')}</dd></div><div class="wide"><dt>Descripción</dt><dd>${escape(f.values.description||'Sin resolver')}</dd></div></dl>${f.errors.length?`<div class="info-box"><strong>Requiere atención</strong><ul>${f.errors.map(e=>`<li>${escape(e)}</li>`).join('')}</ul></div>`:'<div class="info-box">Metadatos válidos en la demostración. La validación real de permisos, contenido y catálogos corresponde al servidor.</div>'}${state.step===4?`<div class="flow"><span>Recepción</span>→<span>Almacenamiento</span>→<span>Incorporación</span></div>${statusTag(f)}${f.status==='uncertain'?'<p>No se conoce el resultado final. Consulta la operación antes de reintentar.</p>':''}`:''}`);}
  function loadExamples(){state.roots=[];state.files=[];for(const number of ['1246566','1246567']){const r={id:id('raiz'),name:number,demo:true};state.roots.push(r);const names=state.profile==='orders'?[`${number}-125-OrdenCompra.pdf`,`${number}-130-ActaEntrega.pdf`]:['Solicitud.pdf','Soporte.docx'];names.forEach((name,i)=>state.files.push(newFile(r.id,name,r.name+'/'+(i?'Soportes/':'')+name,(i?0.8:1.4)*1048576)));}invalidate();notice('Ejemplo cargado: dos carpetas, cuatro documentos. Puedes revisar los valores resueltos.',true);render();}
  async function selectedFolders(event){
    const files=Array.from(event.target.files);event.target.value='';if(!files.length)return;
    if(files.some(f=>!f.webkitRelativePath)){notice('Este navegador no entregó rutas relativas. Usa un navegador compatible o explora con el ejemplo.');return;}
    if(state.files.length+files.length>500){notice('Esta maqueta admite hasta 500 archivos. La importación real deberá aplicar los límites del servicio.');return;}
    const roots=new Map();for(const file of files){const path=file.webkitRelativePath;const name=path.split('/')[0];if(!roots.has(name)){const root={id:id('raiz'),name,demo:false};roots.set(name,root);state.roots.push(root);}state.files.push(newFile(roots.get(name).id,file.name,path,file.size,file));}
    invalidate();notice('Se agregaron '+files.length+' archivos al inventario local. Su contenido no se ha leído ni enviado.',true);render();
  }
  function setStep(step){state.step=step;state.page=0;state.query='';state.filter='all';notice('');render();$('section-title').setAttribute('tabindex','-1');$('section-title').focus();}
  function startSimulation(){if(!destinations[state.destination])return;state.batchDestination={...destinations[state.destination],id:state.destination};state.batch='DEMO-'+String(Date.now()).slice(-6);state.cancelled=false;state.paused=false;state.running=true;state.files.forEach(f=>{f.status='pending';f.documentId=null;f.uncertaintyShown=false;});setStep(4);tick();}
  function tick(){
    if(!state.running)return;
    const f=state.files.find(x=>['pending','receiving','received','storing'].includes(x.status));
    if(!f){state.running=false;render();notice('La simulación terminó. No se transfirieron archivos ni se crearon registros.',true);return;}
    if(f.status==='pending')f.status='receiving';else if(f.status==='receiving')f.status='received';else if(f.status==='received')f.status='storing';else if(state.scenario==='partial'&&f===state.files[1]&&!f.uncertaintyShown){f.status='uncertain';f.uncertaintyShown=true;}else markDone(f);
    render();state.timer=setTimeout(tick,400);
  }
  function markDone(f){f.status='done';f.documentId='DOC-DEMO-'+(state.files.indexOf(f)+1).toString().padStart(4,'0');const r=rootOf(f);r.expedientId=r.expedientId||'EXP-DEMO-'+(state.roots.indexOf(r)+1).toString().padStart(3,'0');}
  function exportResults(){const safeCell=v=>'"'+String(v??'').replace(/^[=+@\-\t\r]/,"'$&").replace(/"/g,'""')+'"';const rows=[['SIMULACION SIN EFECTOS REALES','Carpeta','Ruta','Estado','Documento ficticio'],...state.files.map(f=>['DEMO',rootOf(f).name,f.path,f.status,f.documentId||''])];const blob=new Blob(['\uFEFF'+rows.map(r=>r.map(safeCell).join(';')).join('\r\n')],{type:'text/csv;charset=utf-8'});const url=URL.createObjectURL(blob);const link=document.createElement('a');link.href=url;link.download='resultado-simulado-importacion.csv';link.click();setTimeout(()=>URL.revokeObjectURL(url),1500);}
  $('next').addEventListener('click',()=>{if($('next').disabled)return;if(state.step===1)setStep(2);else if(state.step===2){invalidate();setStep(3);}else if(state.step===3){validate();if(!errors().length&&state.confirmed)startSimulation();else render();}else{clearTimeout(state.timer);Object.assign(state,{step:1,batchDestination:null,roots:[],files:[],confirmed:false,scenario:'normal',running:false,paused:false,cancelled:false,page:0,query:'',filter:'all',batch:null});notice('');render();}});
  $('back').addEventListener('click',()=>{state.confirmed=false;setStep(Math.max(1,state.step-1));});
  $('folder-picker').addEventListener('change',selectedFolders);
  $('stage-content').addEventListener('click',event=>{
    const button=event.target.closest('[data-action]');if(!button||button.disabled)return;const action=button.dataset.action,key=button.dataset.id;
    if(action==='rules')showRules();
    if(action==='detail')showDetail(key);
    if(action==='add'){if(!('webkitdirectory' in $('folder-picker'))){notice('El selector de carpetas no está disponible en este navegador. Puedes usar el ejemplo.');return;}$('folder-picker').click();}
    if(action==='example')loadExamples();
    if(action==='clear'){state.roots=[];state.files=[];invalidate();render();}
    if(action==='remove-root'){state.roots=state.roots.filter(r=>r.id!==key);state.files=state.files.filter(f=>f.root!==key);invalidate();render();}
    if(action==='remove-file'){state.files=state.files.filter(f=>f.id!==key);state.roots=state.roots.filter(r=>state.files.some(f=>f.root===r.id));invalidate();render();}
    if(action==='page-prev'||action==='page-next'){state.page+=action==='page-next'?1:-1;render();}
    if(action==='pause'){clearTimeout(state.timer);state.running=false;state.paused=true;render();notice('Pausa simulada. En producción una petición en vuelo debe consultarse antes de reanudar.');}
    if(action==='resume'){state.running=true;state.paused=false;notice('');tick();}
    if(action==='cancel'){clearTimeout(state.timer);state.running=false;state.paused=false;state.cancelled=true;state.files.forEach(f=>{if(f.status==='storing')f.status='uncertain';else if(['pending','receiving','received'].includes(f.status))f.status='cancelled';});render();notice('Se cancelaron los pendientes en la simulación. Los efectos ya confirmados se conservan; los inciertos requieren verificación.');}
    if(action==='reconcile'){state.files.filter(f=>f.status==='uncertain').forEach(markDone);render();notice('Verificación simulada: se encontró el documento y su relación con el expediente. No se volvió a almacenar.',true);}
    if(action==='export')exportResults();
  });
  $('stage-content').addEventListener('change',event=>{
    if(event.target.id==='profile'){state.profile=event.target.value;invalidate();render();}
    if(event.target.id==='confirm'){state.confirmed=event.target.checked;$('next').disabled=!state.confirmed||errors().length>0||!destinations[state.destination];}
    if(event.target.id==='scenario'){state.scenario=event.target.value;invalidate();render();}
    if(event.target.id==='filter'){state.filter=event.target.value;state.page=0;render();}
  });
  $('stage-content').addEventListener('input',event=>{if(event.target.id==='search'){const pos=event.target.selectionStart;state.query=event.target.value;state.page=0;render();$('search').focus();$('search').setSelectionRange(pos,pos);}});
  $('guide').addEventListener('click',()=>dialog('Cómo funciona la importación',`<ol><li><strong>Perfil:</strong> determina clasificación, gabinete y reglas de metadatos.</li><li><strong>Carpetas:</strong> agrega una o varias raíces desde tu dispositivo.</li><li><strong>Revisión:</strong> verifica expedientes, índices y tipologías antes de confirmar.</li><li><strong>Resultados:</strong> distingue transferencia, almacenamiento e incorporación.</li></ol><div class="info-box">Este prototipo funciona sin servicios. Puedes seleccionar carpetas reales: solo se consultan sus nombres, rutas relativas y tamaños. Nada se sube.</div><p>Para ver una incidencia, cambia el escenario en Revisión. Para probar otro formato, selecciona «Expedientes administrativos» en el primer paso.</p><p class="tiny-note">Al cerrar o recargar se descarta el inventario local. En la aplicación final, el lote se recuperará del servidor y los archivos pendientes requerirán nueva selección.</p>`));
  $('close-dialog').addEventListener('click',()=>$('detail-dialog').close());$('accept-dialog').addEventListener('click',()=>$('detail-dialog').close());
  window.addEventListener('beforeunload',event=>{if(state.running){event.preventDefault();event.returnValue='';}});
  document.querySelectorAll('[data-level]').forEach(button=>button.addEventListener('click',()=>{
    selectedTreeLevel=button.dataset.level;
    document.querySelectorAll('[data-level]').forEach(el=>{const selected=el.dataset.level===selectedTreeLevel;el.classList.toggle('selected',selected);el.setAttribute('aria-pressed',String(selected));});
    $('current-level').textContent=destinations[selectedTreeLevel].name;
    $('current-path').textContent=destinations[selectedTreeLevel].path;
    $('open-import').disabled=false;
  }));
  $('open-import').addEventListener('click',()=>{
    if(!destinations[selectedTreeLevel])return;
    clearTimeout(state.timer);
    Object.assign(state,{step:1,destination:selectedTreeLevel,batchDestination:null,roots:[],files:[],confirmed:false,scenario:'normal',running:false,paused:false,cancelled:false,page:0,query:'',filter:'all',batch:null});
    notice('');render();$('import-dialog').showModal();
  });
  function closeImport(event){if(state.running){if(event)event.preventDefault();notice('Pausa o cancela la simulación antes de cerrar.');return;}$('import-dialog').close();$('open-import').focus();}
  $('close-import').addEventListener('click',closeImport);
  $('import-dialog').addEventListener('cancel',event=>{event.preventDefault();closeImport(event);});
  render();
})();
