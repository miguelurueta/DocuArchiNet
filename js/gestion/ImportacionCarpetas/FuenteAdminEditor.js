(function(window){
    'use strict';
    function el(tag,text,attrs){var n=document.createElement(tag);if(text!=null)n.textContent=text;Object.keys(attrs||{}).forEach(function(k){n.setAttribute(k,attrs[k]);});return n;}
    function iniciar(host,api){
        if(host.getAttribute('data-fuentes')!=='1'||host.querySelector('[data-doc110=admin-fuentes]'))return;
        var open=el('button','Administrar fuentes',{type:'button','data-doc110':'admin-fuentes'});
        var panel=el('section',null,{role:'dialog','aria-modal':'true','aria-label':'Administrar fuentes',tabindex:'-1','class':'doc110-dialog'});
        panel.hidden=true;host.appendChild(open);host.appendChild(panel);
        var header=el('header',null,{'class':'doc110-header'}),heading=el('div');heading.appendChild(el('span','PRODUCCIÓN DOCUMENTAL',{'class':'doc110-eyebrow'}));heading.appendChild(el('h2','Fuentes de datos autorizadas'));heading.appendChild(el('p','Autoriza las tablas y columnas disponibles para configurar proveedores.'));header.appendChild(heading);panel.appendChild(header);
        var status=el('p','',{role:'status','aria-live':'polite','aria-atomic':'true','class':'doc110-fuente-estado','data-fuente':'resultado'}),body=el('div'),busy=false,epoch=0,read=null,baseline=null,uncertain=false,focus;
        body.className='doc110-body';panel.appendChild(status);panel.appendChild(body);
        function grid(parent,headers,label){var wrap=el('div',null,{'class':'doc110-scroll',tabindex:'0','aria-label':label}),t=el('table'),head=el('thead'),row=el('tr'),rows=el('tbody');t.appendChild(el('caption',label,{'class':'doc110-sr-only'}));headers.forEach(function(h){row.appendChild(el('th',h,{scope:'col'}));});head.appendChild(row);t.appendChild(head);t.appendChild(rows);wrap.appendChild(t);parent.appendChild(wrap);return rows;}
        function button(parent,label,fn,id){var n=el('button',label,{type:'button'});if(id)n.setAttribute('data-fuente',id);n.onclick=function(){if(!busy)fn();};parent.appendChild(n);return n;}
        function field(parent,label,value,id){var wrap=el('label',label),n=el('input',null,{type:'text','aria-label':label});if(id)n.setAttribute('data-fuente',id);n.value=value||'';wrap.appendChild(n);parent.appendChild(wrap);return n;}
        function select(parent,label,value,id){var wrap=el('label',label),n=el('select',null,{'aria-label':label,'data-fuente':id});wrap.appendChild(n);parent.appendChild(wrap);fill(n,[],value);return n;}
        function fill(n,items,value){n.replaceChildren(el('option','Seleccione…',{value:''}));items.forEach(function(x){n.appendChild(el('option',x.text||x.value,{value:x.value}));});if(value&&!items.some(function(x){return x.value===value;}))n.appendChild(el('option','Referencia guardada: '+value,{value:value}));n.value=value||'';}
        function dirty(){return read&&JSON.stringify(read())!==baseline;}
        function protect(){return !dirty()||window.confirm('Hay cambios sin guardar. ¿Desea descartarlos?');}
        function reset(){epoch++;read=null;baseline=null;body.replaceChildren();}
        function call(op,payload,done){
            if(busy)return;busy=true;panel.setAttribute('aria-busy','true');status.textContent='Procesando…';
            var locked=Array.prototype.map.call(panel.querySelectorAll('input,select,button'),function(n){var v={n:n,d:n.disabled};n.disabled=true;return v;});
            function unlock(){locked.forEach(function(x){x.n.disabled=x.d;});busy=false;panel.removeAttribute('aria-busy');}
            api.ejecutar(op,payload).done(function(r){unlock();status.textContent=r.mensaje;done(r.datos);}).fail(function(e){unlock();uncertain=uncertain||!!e.resultadoIncierto;status.textContent=e.mensaje+(uncertain?' La escritura anterior no se pudo confirmar. Vuelva al listado y verifique si la fuente existe antes de repetir.':'');});
        }
        function get(id){call('ObtenerFuente',{FuenteId:id},function(d){uncertain=false;detail(d);});}
        function list(page,term){if(!protect())return;call('ListarFuentes',{Pagina:page,Termino:term||null},function(items){reset();var toolbar=el('div',null,{'class':'doc110-toolbar'});body.appendChild(toolbar);var search=field(toolbar,'Buscar tabla',term);button(toolbar,'Buscar',function(){list(1,search.value);});button(toolbar,'Nueva fuente',function(){if(uncertain){status.textContent='Abra la fuente desde el listado para verificar el resultado anterior antes de crear otra.';return;}detail(null);},'nueva');
            var rows=grid(body,['Conexión','Tabla o vista','Estado','Revisión','Acción'],'Fuentes autorizadas');
            items.forEach(function(d){var row=el('tr');row.appendChild(el('td',d.ConexionRef));row.appendChild(el('td',d.Tabla));var state=el('td');state.appendChild(el('span',d.Activo?'Activa':'Inactiva',{'class':'doc110-badge','data-tone':d.Activo?'success':'neutral'}));row.appendChild(state);row.appendChild(el('td',d.Revision));var actions=el('td');button(actions,'Abrir',function(){get(d.FuenteId);},'abrir');row.appendChild(actions);rows.appendChild(row);});
            if(!items.length){var empty=el('tr');empty.appendChild(el('td','No hay fuentes registradas. Crea una fuente para autorizar su consulta.',{colspan:'5','class':'doc110-empty'}));rows.appendChild(empty);}
            body.appendChild(el('p','Página '+page+' · '+items.length+' fuentes en esta página',{'class':'doc110-help'}));
            if(page>1)button(body,'Anterior',function(){list(page-1,term);});if(items.length===25)button(body,'Siguiente',function(){list(page+1,term);});
        });}
        function history(d,page){
            if(!protect())return;
            call('HistorialFuente',{FuenteId:d.FuenteId,Pagina:page},function(items){
                reset();body.appendChild(el('h3','Historial: '+d.Tabla));
                var rows=grid(body,['Revisión','Acción','Usuario','Fecha UTC','Motivo','Configuración'],'Historial de autorizaciones');
                items.forEach(function(x){
                    var row=el('tr');[x.Revision,x.Accion,x.ActorRef,x.FechaUtc,x.Motivo].forEach(function(v){row.appendChild(el('td',v));});
                    var cell=el('td'),details=el('details'),snapshot=x.Estado||{};details.appendChild(el('summary','Ver detalle'));
                    var values=el('dl',null,{'class':'doc110-summary'});
                    [['Conexión',snapshot.ConexionRef],['Tabla',snapshot.Tabla],['Aislamiento',snapshot.ModoAislamiento],['Columna de empresa',snapshot.ColumnaEmpresa||'No aplica'],['Estado',snapshot.Activo?'Activa':'Inactiva'],['Columnas',(snapshot.Columnas||[]).join(', ')]].forEach(function(pair){values.appendChild(el('dt',pair[0]));values.appendChild(el('dd',pair[1]));});
                    details.appendChild(values);cell.appendChild(details);row.appendChild(cell);rows.appendChild(row);
                });
                if(!items.length){var empty=el('tr');empty.appendChild(el('td','Sin registros.',{colspan:'6','class':'doc110-empty'}));rows.appendChild(empty);}
                if(page>1)button(body,'Anterior',function(){history(d,page-1);});if(items.length===25)button(body,'Siguiente',function(){history(d,page+1);});button(body,'Volver a la fuente',function(){get(d.FuenteId);});
            });
        }
        function detail(saved){
            reset();var d=saved||{FuenteId:null,Revision:0,ConexionRef:'',Tabla:'',ModoAislamiento:'CONEXION',ColumnaEmpresa:'',Columnas:[]},view=epoch,sequence=0,pending=false,failed=false,tablePage=1,columnPage=1,choices=new Set(d.Columnas),known={};
            body.appendChild(el('h3',saved?d.Tabla+' · revisión '+d.Revision+' · '+(d.Activo?'Activa':'Inactiva'):'Nueva fuente'));
            body.appendChild(el('p','Fuente → Proveedor → Reglas del perfil. La columna de búsqueda se define en el proveedor.',{'class':'doc110-context'}));
            var form=el('fieldset');body.appendChild(form);form.appendChild(el('legend','1. Origen y aislamiento'));var fields=el('div',null,{'class':'doc110-fields'});form.appendChild(fields);
            var connection=select(fields,'Conexión',d.ConexionRef,'conexion'),table=select(fields,'Tabla o vista',d.Tabla,'tabla');
            connection.disabled=table.disabled=!!saved;
            var mode=select(fields,'Aislamiento',d.ModoAislamiento,'modo');fill(mode,[{value:'CONEXION',text:'Fuente exclusiva de la empresa por conexión'},{value:'COLUMNA',text:'Filtrar por columna de empresa'}],d.ModoAislamiento);
            var company=select(fields,'Columna de empresa',d.ColumnaEmpresa,'empresa');
            form.appendChild(el('p','Solo se ofrecen tablas con prefijo imp_02 y excepciones autorizadas por la configuración del servidor.',{'class':'doc110-help'}));
            var note=el('p','',{role:'status'});form.appendChild(note);
            var columnSection=el('fieldset');columnSection.appendChild(el('legend','2. Columnas autorizadas'));columnSection.appendChild(el('p','Selecciona únicamente los campos que los proveedores podrán consultar.',{'class':'doc110-help'}));body.appendChild(columnSection);
            var columns=el('div',null,{'class':'doc110-column-options'}),selection=el('div',null,{'class':'doc110-selection'});columnSection.appendChild(columns);columnSection.appendChild(selection);
            var audit=el('fieldset');audit.appendChild(el('legend','3. Justificación y disponibilidad'));audit.appendChild(el('p','La fuente se crea inactiva. Después de guardarla, activa su autorización para utilizarla en un proveedor.',{'class':'doc110-help'}));body.appendChild(audit);
            var reason=field(audit,'Motivo del cambio','','motivo');reason.maxLength=500;
            function showSelection(){selection.replaceChildren(el('p','Columnas seleccionadas: '+choices.size));Array.from(choices).sort().forEach(function(name){var row=el('span',name+' ');button(row,'Quitar '+name,function(){choices.delete(name);showSelection();Array.prototype.forEach.call(columns.querySelectorAll('input'),function(n){if(n.value===name)n.checked=false;});});selection.appendChild(row);});fill(company,Array.from(choices).filter(function(x){return !known[x]||['TEXTO','ENTERO'].indexOf(known[x])>=0;}).map(function(x){return {value:x};}),company.value);company.disabled=mode.value!=='COLUMNA';}
            function metadata(tipo,page,done){pending=true;failed=false;retry.hidden=true;var request=++sequence;note.textContent='Consultando metadatos…';api.ejecutar('ConsultarMetadatosFuente',{tipo:tipo,conexionRef:connection.value||null,tabla:table.value||null,pagina:page}).done(function(r){if(view!==epoch||request!==sequence)return;pending=false;note.textContent=r.datos.length?'Opciones disponibles.':'Sin opciones disponibles.';done(r.datos);}).fail(function(e){if(view!==epoch||request!==sequence)return;pending=false;failed=true;note.textContent=e.mensaje;retry.hidden=false;});}
            function loadColumns(page){columnPage=page;columns.replaceChildren();if(!table.value){pending=false;return;}metadata('COLUMNAS',page,function(items){items.forEach(function(o){known[o.value]=o.Tipo;var label=el('label',o.value+' ('+o.Tipo+') '),n=el('input',null,{type:'checkbox','aria-label':'Permitir '+o.value});n.value=o.value;n.checked=choices.has(o.value);n.disabled=o.Tipo==='NO_SOPORTADO';n.onchange=function(){if(n.checked&&choices.size>=128){n.checked=false;note.textContent='Máximo 128 columnas.';return;}if(n.checked)choices.add(n.value);else choices.delete(n.value);showSelection();};label.appendChild(n);columns.appendChild(label);});if(page>1)button(columns,'Columnas anteriores',function(){loadColumns(page-1);});if(items.length===25)button(columns,'Más columnas',function(){loadColumns(page+1);});showSelection();});}
            var tableNav=el('div');table.parentNode.appendChild(tableNav);
            function loadTables(page){tablePage=page;tableNav.replaceChildren();if(!connection.value){pending=false;return;}metadata('TABLAS',page,function(items){fill(table,items,table.value);if(page>1)button(tableNav,'Tablas anteriores',function(){loadTables(page-1);});if(items.length===25)button(tableNav,'Más tablas',function(){loadTables(page+1);});});}
            function loadConnections(){metadata('CONEXIONES',1,function(items){fill(connection,items,connection.value);if(saved)loadColumns(1);else if(connection.value)loadTables(tablePage);});}
            var retry=button(form,'Reintentar opciones',function(){if(table.value)loadColumns(columnPage);else if(connection.value)loadTables(tablePage);else loadConnections();},'reintentar');retry.hidden=true;
            function clearColumns(){sequence++;choices.clear();known={};company.value='';columns.replaceChildren();showSelection();}
            connection.onchange=function(){clearColumns();fill(table,[],'');loadTables(1);};table.onchange=function(){clearColumns();loadColumns(1);};mode.onchange=function(){if(mode.value==='CONEXION')company.value='';showSelection();};
            read=function(){return {FuenteId:d.FuenteId,Revision:d.Revision,ConexionRef:connection.value,Tabla:table.value,ModoAislamiento:mode.value,ColumnaEmpresa:company.value,Columnas:Array.from(choices).sort(),Motivo:reason.value};};showSelection();baseline=JSON.stringify(read());
            function mutate(op){if(uncertain){status.textContent='La escritura anterior no se pudo confirmar. Vuelva al listado y verifique si la fuente existe antes de repetir.';return;}if(op!=='RevocarFuente'&&(pending||failed)){status.textContent='Complete la consulta de metadatos antes de guardar o activar.';return;}if((op==='ActivarFuente'||op==='RevocarFuente')){var original=JSON.parse(baseline),now=read();delete original.Motivo;delete now.Motivo;if(JSON.stringify(original)!==JSON.stringify(now)){status.textContent='Guarde o descarte los cambios antes de cambiar disponibilidad.';return;}}
                if(!reason.value.trim()){status.textContent='Indique el motivo del cambio.';reason.focus();return;}var payload=read(),motivo=payload.Motivo;delete payload.Motivo;
                call(op,{Cambio:{Documento:payload,Motivo:motivo}},function(result){uncertain=false;detail(result);});
            }
            var actions=el('div',null,{'class':'doc110-actions'});audit.appendChild(actions);
            button(actions,saved?'Guardar cambios':'Crear fuente inactiva',function(){mutate(saved?'GuardarFuente':'CrearFuente');},'guardar');
            if(saved){var availability=button(actions,d.Activo?'Revocar autorización':'Activar autorización',function(){mutate(d.Activo?'RevocarFuente':'ActivarFuente');},'disponibilidad');availability.className=d.Activo?'doc110-danger':'';}
            var navigation=el('nav',null,{'class':'doc110-actions','aria-label':'Navegación de la fuente'});body.appendChild(navigation);
            if(saved){button(navigation,'Consultar estado',function(){if(protect())get(d.FuenteId);},'estado');button(navigation,'Historial',function(){history(d,1);},'historial');}
            button(navigation,'Volver al listado',function(){list(1);});loadConnections();
        }
        var close=button(panel,'Cerrar',function(){if(protect()){epoch++;panel.hidden=true;if(focus)focus.focus();}},'cerrar');
        header.appendChild(close);
        open.onclick=function(){if(busy)return;var other=host.querySelector('section[role=dialog]:not([hidden])');if(other&&other!==panel){other.focus();return;}focus=document.activeElement;panel.hidden=false;panel.focus();list(1);};
        panel.addEventListener('keydown',function(e){if(e.key==='Escape'&&!busy)close.click();if(e.key==='Tab'){var nodes=Array.prototype.filter.call(panel.querySelectorAll('button,input,select'),function(n){return !n.disabled&&n.getClientRects().length;});if(!nodes.length){e.preventDefault();return;}var first=nodes[0],last=nodes[nodes.length-1];if(e.shiftKey&&document.activeElement===first){e.preventDefault();last.focus();}else if(!e.shiftKey&&document.activeElement===last){e.preventDefault();first.focus();}}});
    }
    window.FuenteAdminEditor={inicializar:iniciar};
}(window));
