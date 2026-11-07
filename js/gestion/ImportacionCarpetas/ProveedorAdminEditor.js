(function(window){
    'use strict';
    function el(tag,text,attrs){var n=document.createElement(tag);if(text!=null)n.textContent=text;Object.keys(attrs||{}).forEach(function(k){n.setAttribute(k,attrs[k]);});return n;}
    function iniciar(host,api){
        if(host.getAttribute('data-proveedores')!=='1' || host.querySelector('[data-doc110=admin-proveedores]'))return;
        var open=el('button','Administrar proveedores',{type:'button','data-doc110':'admin-proveedores'}),panel=el('section',null,{role:'dialog','aria-modal':'true','aria-label':'Administrar proveedores',tabindex:'-1','class':'doc110-dialog'});
        panel.hidden=true;host.appendChild(open);host.appendChild(panel);
        var status=el('p','',{role:'status','class':'doc110-proveedor-estado','data-proveedor':'resultado','aria-live':'polite','aria-atomic':'true'}),body=el('div'),busy=false,epoch=0,current=null,read=null,baseline=null,uncertain=false,focus;
        var header=el('header',null,{'class':'doc110-header'}),heading=el('div');heading.appendChild(el('span','PRODUCCIÓN DOCUMENTAL',{'class':'doc110-eyebrow'}));heading.appendChild(el('h2','Proveedores de datos'));heading.appendChild(el('p','Define cómo buscar un registro y qué valores utilizar en los índices.'));header.appendChild(heading);
        body.className='doc110-body';panel.appendChild(header);panel.appendChild(status);panel.appendChild(body);
        function button(parent,label,fn,id){var b=el('button',label,{type:'button'});if(id)b.setAttribute('data-proveedor',id);b.onclick=function(){if(!busy)fn();};parent.appendChild(b);return b;}
        var close=button(panel,'Cerrar',function(){if(protect()){epoch++;panel.hidden=true;if(focus)focus.focus();}},'cerrar');
        header.appendChild(close);
        function group(parent,title,help){var section=el('section',null,{'class':'doc110-card'});section.appendChild(el('h4',title));if(help)section.appendChild(el('p',help,{'class':'doc110-help'}));parent.appendChild(section);return section;}
        function grid(parent,headers,label){var wrap=el('div',null,{'class':'doc110-scroll',tabindex:'0','aria-label':label}),t=el('table'),head=el('thead'),row=el('tr'),rows=el('tbody');t.appendChild(el('caption',label,{'class':'doc110-sr-only'}));headers.forEach(function(h){row.appendChild(el('th',h,{scope:'col'}));});head.appendChild(row);t.appendChild(head);t.appendChild(rows);wrap.appendChild(t);parent.appendChild(wrap);return rows;}
        function field(parent,label,value,type){var w=el('label',label+' '),n=el('input',null,{type:type||'text'});n.value=value==null?'':value;w.appendChild(n);parent.appendChild(w);return n;}
        function select(parent,label,value){var w=el('label',label+' '),n=el('select',null,{'aria-label':label});w.appendChild(n);parent.appendChild(w);fill(n,[],value);return n;}
        function fill(n,items,keep){n.replaceChildren(el('option','Seleccione…',{value:''}));items.forEach(function(i){n.appendChild(el('option',i.text||i.value,{value:i.value}));});if(keep && !items.some(function(i){return i.value===keep;}))n.appendChild(el('option','Referencia guardada: '+keep,{value:keep}));n.value=keep||'';}
        function dirty(){return read && JSON.stringify(read())!==baseline;}
        function protect(){return !dirty() || window.confirm('Hay cambios sin guardar. ¿Desea descartarlos?');}
        function reset(){epoch++;body.replaceChildren();current=null;read=null;baseline=null;}
        function call(op,data,done){
            if(busy)return;busy=true;var locked=Array.prototype.map.call(panel.querySelectorAll('button,input,select'),function(n){var state={node:n,disabled:n.disabled};n.disabled=true;return state;});function unlock(){locked.forEach(function(x){x.node.disabled=x.disabled;});busy=false;panel.removeAttribute('aria-busy');}panel.setAttribute('aria-busy','true');status.textContent='Procesando…';
            api.ejecutar(op,data).done(function(r){unlock();status.textContent=r.mensaje;done(r.datos);}).fail(function(e){unlock();uncertain=uncertain||!!e.resultadoIncierto;status.textContent=e.mensaje+(uncertain?' Consulte el estado antes de repetir una escritura.':'');});
        }
        function refs(){return {ProveedorId:current.ProveedorId,VersionId:current.VersionId,Revision:current.Revision,RevisionProveedor:current.RevisionProveedor};}
        function get(p,v){call('ObtenerAdministracionProveedor',{ProveedorId:p,VersionId:v},function(d){uncertain=false;detail(d);});}
        function list(page,provider,term){
            if(!protect())return;
            call('ListarAdministracionProveedores',{Pagina:page||1,ProveedorId:provider||null,Termino:term||null},function(items){
                reset();var toolbar=el('div',null,{'class':'doc110-toolbar'});body.appendChild(toolbar);var search=field(toolbar,'Buscar por código o nombre',term||'');button(toolbar,'Buscar',function(){list(1,provider,search.value);});
                button(toolbar,'Nuevo proveedor',create,'nuevo');
                var rows=grid(body,['Código','Nombre','Versión','Estado','Disponibilidad','Acción'],'Proveedores registrados');
                items.forEach(function(d){var line=el('tr');[d.Codigo,d.Nombre,d.Numero].forEach(function(v){line.appendChild(el('td',v));});var state=el('td');state.appendChild(el('span',d.Estado,{'class':'doc110-badge','data-tone':d.Estado==='PUBLICADA'?'success':'draft'}));line.appendChild(state);var active=el('td');active.appendChild(el('span',d.Activo?'Activo':'Inactivo',{'class':'doc110-badge','data-tone':d.Activo?'success':'neutral'}));line.appendChild(active);var cell=el('td');button(cell,'Abrir',function(){get(d.ProveedorId,d.VersionId);},'abrir');line.appendChild(cell);rows.appendChild(line);});
                if(!items.length){var empty=el('tr');empty.appendChild(el('td','No hay proveedores registrados. Crea un proveedor para configurar la búsqueda.',{colspan:'6','class':'doc110-empty'}));rows.appendChild(empty);}
                body.appendChild(el('p','Página '+(page||1)+' · '+items.length+' versiones en esta página',{'class':'doc110-help'}));
                if(page>1)button(body,'Anterior',function(){list(page-1,provider,term);});
                if(items.length===25)button(body,'Siguiente',function(){list((page||1)+1,provider,term);});
                if(provider)button(body,'Todos los proveedores',function(){list(1);});
            });
        }
        function create(){
            reset();var identity=group(body,'Nuevo proveedor','Primero identifica el proveedor. Después podrás elegir la fuente, la columna de búsqueda y los valores que devuelve.');var fields=el('div',null,{'class':'doc110-fields'});identity.appendChild(fields);var code=field(fields,'Código',''),name=field(fields,'Nombre','');code.maxLength=64;name.maxLength=160;code.setAttribute('data-proveedor','codigo');name.setAttribute('data-proveedor','nombre');
            read=function(){return {Codigo:code.value,Nombre:name.value};};baseline=JSON.stringify(read());
            button(body,'Crear borrador',function(){if(uncertain){status.textContent='Consulte el listado para comprobar si se creó el proveedor.';return;}call('CrearProveedor',read(),detail);},'crear');
            button(body,'Volver al listado',function(){list(1);});
        }
        function detail(d){
            reset();current=d;var view=epoch,c=d.Contrato,published=d.Estado==='PUBLICADA',rows=[],columns=[],metaLoading=false,metaSequence=0;
            body.appendChild(el('h3',d.Codigo+' — '+d.Nombre+' · versión '+d.Numero+' · '+d.Estado));
            body.appendChild(el('p',published?'Versión publicada · Solo lectura. Crea una nueva versión para modificar la configuración.':'Borrador · Guarda la configuración antes de probarla o publicarla.',{'class':'doc110-context'}));
            var form=el('fieldset',null,{'class':'doc110-provider-form'});form.disabled=published;body.appendChild(form);form.appendChild(el('legend','Configuración del proveedor'));
            var origin=group(form,'1. Fuente y búsqueda','La columna de búsqueda debe identificar un único registro en la fuente autorizada.'),fields=el('div',null,{'class':'doc110-fields'});origin.appendChild(fields);
            var connection=select(fields,'Conexión',c.ConexionRef),table=select(fields,'Tabla o vista',c.Tabla),key=select(fields,'Columna de búsqueda',c.ColumnaClave),company=select(fields,'Columna de empresa',c.ColumnaEmpresa),keyType=field(fields,'Tipo de clave',c.TipoClave||'TEXTO');keyType.readOnly=true;
            connection.setAttribute('data-proveedor','conexion');table.setAttribute('data-proveedor','tabla');key.setAttribute('data-proveedor','clave');company.setAttribute('data-proveedor','empresa');
            origin.appendChild(el('p','Deje la columna de empresa sin seleccionar solo para fuentes autorizadas como exclusivas de la empresa actual.',{'class':'doc110-help'}));
            var info=el('p','',{role:'status'});origin.appendChild(info);var mappingSection=group(form,'2. Valores que devuelve la consulta','Asigna un nombre de propiedad a cada columna que usarás en las reglas del perfil.'),mappings=grid(mappingSection,['Propiedad','Columna de origen','Tipo','Acción'],'Propiedades del proveedor');
            function add(p){var row=el('tr'),a=el('td'),b=el('td'),t=el('td'),actions=el('td');[a,b,t,actions].forEach(function(cell){row.appendChild(cell);});var name=field(a,'Propiedad',p.Nombre),col=select(b,'Columna',p.Columna),type=field(t,'Tipo',p.Tipo||'TEXTO');name.maxLength=64;type.readOnly=true;fill(col,columns.filter(function(x){return x.Tipo!=='NO_SOPORTADO';}),p.Columna);var entry={node:row,name:name,col:col,type:type};col.onchange=function(){var m=columns.find(function(x){return x.value===col.value;});type.value=m?m.Tipo:'';};button(actions,'Quitar propiedad',function(){rows=rows.filter(function(x){return x!==entry;});row.remove();});mappings.appendChild(row);rows.push(entry);}
            (c.Propiedades||[]).forEach(add);button(mappingSection,'Agregar propiedad',function(){if(rows.length<128)add({});},'agregar-propiedad');
            function clearMappings(){rows=[];mappings.replaceChildren();}
            function metadata(tipo,done){
                metaLoading=true;var request=++metaSequence;retry.hidden=true;var expected=connection.value+'|'+table.value;info.textContent='Consultando opciones…';
                api.ejecutar('ConsultarMetadatosProveedor',{tipo:tipo,conexionRef:connection.value||null,tabla:table.value||null,pagina:1}).done(function(r){
                    if(view!==epoch || request!==metaSequence || expected!==connection.value+'|'+table.value)return;
                    var items=r.datos;function finish(){metaLoading=false;info.textContent=items.length?'Opciones autorizadas disponibles.':'Sin opciones disponibles para esta selección.';done(items);}
                    function more(page){api.ejecutar('ConsultarMetadatosProveedor',{tipo:tipo,conexionRef:connection.value||null,tabla:table.value||null,pagina:page}).done(function(next){if(view!==epoch || request!==metaSequence || expected!==connection.value+'|'+table.value)return;items=items.concat(next.datos);if(next.datos.length===25 && page<12)more(page+1);else finish();}).fail(failed);}
                    if(items.length===25)more(2);else finish();
                }).fail(failed);
                function failed(){if(view!==epoch || request!==metaSequence || expected!==connection.value+'|'+table.value)return;metaLoading=false;retry.hidden=false;info.textContent='No se pudieron consultar las opciones. Use Reintentar.';}
            }
            function missing(values,items){return values.some(function(value){return value&&!items.some(function(item){return item.value===value;});});}
            function unavailable(){metaLoading=false;info.textContent='Referencias guardadas no disponibles. Se conservan los valores originales; revise la autorización de la fuente y sus columnas.';}
            function loadColumns(){if(!table.value){metaLoading=false;return;}metadata('COLUMNAS',function(items){columns=items;var supported=items.filter(function(x){return x.Tipo!=='NO_SOPORTADO';}),companies=items.filter(function(x){return x.Tipo==='TEXTO'||x.Tipo==='ENTERO';});fill(key,supported,key.value);fill(company,companies,company.value);rows.forEach(function(r){fill(r.col,supported,r.col.value);});if(missing([key.value].concat(rows.map(function(r){return r.col.value;})),supported)||missing([company.value],companies))unavailable();});}
            function loadTables(){if(!connection.value){metaLoading=false;return;}metadata('TABLAS',function(items){fill(table,items,table.value);if(missing([table.value],items)){unavailable();return;}loadColumns();});}
            function loadConnections(){metadata('CONEXIONES',function(items){fill(connection,items,connection.value);if(missing([connection.value],items)){unavailable();return;}loadTables();});}
            connection.onchange=function(){metaSequence++;fill(table,[],'');fill(key,[],'');fill(company,[],'');columns=[];clearMappings();loadTables();};
            table.onchange=function(){metaSequence++;fill(key,[],'');fill(company,[],'');columns=[];clearMappings();loadColumns();};
            key.onchange=function(){var m=columns.find(function(x){return x.value===key.value;});keyType.value=m?m.Tipo:'TEXTO';};
            var retry=button(form,'Reintentar opciones',loadConnections);retry.hidden=true;
            read=function(){return {ConexionRef:connection.value,Tabla:table.value,ColumnaClave:key.value,TipoClave:keyType.value,ColumnaEmpresa:company.value,Propiedades:rows.map(function(r){return {Nombre:r.name.value,Columna:r.col.value,Tipo:r.type.value};})};};baseline=JSON.stringify(read());
            function saved(){if(dirty()){status.textContent='Guarde el borrador antes de validar, probar o publicar.';return false;}if(uncertain){status.textContent='Consulte el estado antes de repetir una escritura.';return false;}if(metaLoading){status.textContent='Espere la carga de opciones.';return false;}return true;}
            var saveActions=el('div',null,{'class':'doc110-actions'});body.appendChild(saveActions);
            if(!published)button(saveActions,'Guardar borrador',function(){if(uncertain){status.textContent='Consulte el estado antes de repetir.';return;}var data=refs();data.Contrato=read();call('GuardarProveedor',data,detail);},'guardar');
            button(saveActions,'Validar estructura guardada',function(){if(saved())call('ValidarProveedor',refs(),function(){status.textContent='Estructura válida. No se ha publicado ni consultado una clave.';});},'validar');
            var trial=group(body,'3. Probar consulta','Consulta la configuración guardada sin modificar los datos de origen.'),trialBar=el('div',null,{'class':'doc110-toolbar'});trial.appendChild(trialBar);
            var example=field(trialBar,'Clave para probar','');example.setAttribute('data-proveedor','ejemplo');
            button(trialBar,'Probar consulta',function(){
                results.replaceChildren();
                if(!saved())return;
                if(!example.value.trim()){status.textContent='Ingrese una clave para probar la consulta.';example.focus();return;}
                var data=refs();data.ClaveEjemplo=example.value;
                call('EnsayarProveedor',data,function(r){
                    var messages={OK:'Consulta correcta: se encontró un único registro.',AMBIGUO:'Se encontró más de un registro para esta clave. No se seleccionó ninguno. La columna de búsqueda debe identificar un único registro.',SIN_COINCIDENCIA:'No se encontró un registro para esta clave.',CLAVE_INVALIDA:'La clave no cumple el tipo o el formato configurado.',CONFIGURACION_INVALIDA:'Revise las columnas y los tipos configurados en el proveedor.',ACCESO_DENEGADO:'La fuente o sus columnas no están autorizadas para esta consulta.',ORIGEN_NO_DISPONIBLE:'No fue posible consultar el origen. Intente nuevamente cuando esté disponible.'};
                    var message=messages[r.Codigo]||'La consulta devolvió un resultado no reconocido.';
                    status.textContent=message;results.appendChild(el('p',message));
                    if(r.Codigo==='OK')Object.keys(r.Valores||{}).forEach(function(k){results.appendChild(el('p',k+': '+(r.Valores[k]==null?'Sin valor':r.Valores[k])));});
                });
            },'ensayar');var results=el('div',null,{'aria-live':'polite','data-proveedor':'valores','class':'doc110-query-result'});trial.appendChild(results);
            var lifecycle=group(body,'Publicación y disponibilidad','Publicar conserva esta versión. La disponibilidad controla si el proveedor puede utilizarse en los perfiles.');
            if(!published)button(lifecycle,'Publicar versión guardada',function(){if(saved())call('PublicarProveedor',refs(),detail);},'publicar');
            else button(lifecycle,'Crear nueva versión',function(){if(saved())call('CrearVersionProveedor',refs(),detail);},'version');
            var availability=button(lifecycle,d.Activo?'Desactivar proveedor':'Activar proveedor',function(){if(!saved())return;var data=refs();data.Activo=!d.Activo;call('CambiarDisponibilidadProveedor',data,detail);},'disponibilidad');availability.className=d.Activo?'doc110-danger':'';
            var navigation=el('nav',null,{'class':'doc110-actions','aria-label':'Navegación del proveedor'});body.appendChild(navigation);
            button(navigation,'Consultar estado',function(){if(protect())get(d.ProveedorId,d.VersionId);},'estado');
            button(navigation,'Historial',function(){list(1,d.ProveedorId);},'historial');button(navigation,'Volver al listado',function(){list(1);});loadConnections();
        }
        open.onclick=function(){if(busy)return;var other=host.querySelector('section[role=dialog]:not([hidden])');if(other && other!==panel){window.alert('Cierre el administrador de perfiles antes de abrir proveedores.');return;}focus=document.activeElement;panel.hidden=false;panel.focus();list(1);};
        panel.addEventListener('keydown',function(e){if(e.key==='Escape'&&!busy)close.click();if(e.key==='Tab'){var nodes=Array.prototype.filter.call(panel.querySelectorAll('button,input,select'),function(n){return !n.disabled&&!n.closest('fieldset:disabled')&&n.getClientRects().length;});var first=nodes[0],last=nodes[nodes.length-1];if(e.shiftKey&&document.activeElement===first){e.preventDefault();last.focus();}else if(!e.shiftKey&&document.activeElement===last){e.preventDefault();first.focus();}}});
    }
    window.ProveedorAdminEditor={inicializar:iniciar};
}(window));
