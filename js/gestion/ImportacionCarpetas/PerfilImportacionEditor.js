// IDs de MySQL: cadenas decimales canónicas; preservar null y no aplicar parseInt.
(function (window, $) {
    "use strict";
    function node(tag, text, attrs) {
        var el = document.createElement(tag);
        if (text !== null && text !== undefined) { el.textContent = text; }
        Object.keys(attrs || {}).forEach(function (key) { el.setAttribute(key, attrs[key]); });
        return el;
    }
    var inheritedProperties=["ID","CODIGO","NOMBRE","FECHA_INICIAL","FECHA_FINAL","RESPONSABLE",["SERIE_ID","Serie - ID interno"],["SERIE_CODIGO","Serie - c\u00f3digo configurado"],["SERIE_NOMBRE","Serie - nombre"],["SUBSERIE_ID","Subserie - ID interno"],["SUBSERIE_CODIGO","Subserie - c\u00f3digo configurado"],["SUBSERIE_NOMBRE","Subserie - nombre"]];
    function init() {
        var host = document.getElementById("doc110-perfiles");
        if (!host || host.getAttribute("data-initialized")) { return; }
        host.setAttribute("data-initialized", "1");
        host.addEventListener("keypress",function(event){event.stopPropagation();});
        host.addEventListener("keydown",function(event){
            if(event.key==="Enter" && event.target.tagName==="INPUT"){event.preventDefault();}
        });
        var api = window.PerfilImportacionApi.crear({url:host.getAttribute("data-api"),token:host.getAttribute("data-token")});
        host.removeAttribute("data-token");
        var open = node("button", "Perfiles de importación", {type:"button", "data-doc110":"abrir"});
        var dialog = node("section", null, {role:"dialog","aria-modal":"true","aria-labelledby":"doc110-title",tabindex:"-1","class":"doc110-dialog"});
        dialog.hidden = true;
        var title = node("h2", "Perfiles de importación", {id:"doc110-title"});
        var close = node("button","Cerrar",{type:"button"});
        var status = node("p","",{role:"status","aria-live":"polite","data-doc110":"estado"});
        var ruleToolbar=node("div",null,{"class":"doc110-rule-toolbar","data-doc110":"barra-reglas"});ruleToolbar.hidden=true;
        var content = node("div",null,{"class":"doc110-body"});
        var header=node("header",null,{"class":"doc110-header"}),heading=node("div");heading.appendChild(node("span","PRODUCCIÓN DOCUMENTAL",{"class":"doc110-eyebrow"}));heading.appendChild(title);heading.appendChild(node("p","Configura el destino documental y las reglas de cada importación."));header.appendChild(heading);header.appendChild(close);
        dialog.appendChild(header); dialog.appendChild(status); dialog.appendChild(ruleToolbar); dialog.appendChild(content);
        host.appendChild(open); host.appendChild(dialog);
        if(window.ProveedorAdminEditor){window.ProveedorAdminEditor.inicializar(host,api);}
        if(window.FuenteAdminEditor){window.FuenteAdminEditor.inicializar(host,api);}
        var options=window.ImportacionCarpetas && window.ImportacionCarpetas.asegurarMenu();
        if(options){
            var prior=options.querySelector('[data-doc119-administracion]');if(prior){prior.remove();}
            var administration=node('div',null,{'class':'doc119-administration-menu','data-doc119-administracion':'1'});
            administration.appendChild(node('span','Administración',{'class':'dropdown-header'}));
            [open,host.querySelector('[data-doc110=admin-proveedores]'),host.querySelector('[data-doc110=admin-fuentes]')].forEach(function(access,index){if(access){access.className='dropdown-item';window.ImportacionCarpetas.rotularAccesoMenu(access,access.textContent,['fa-sliders-h','fa-server','fa-database'][index]);administration.appendChild(access);}});
            options.appendChild(administration);
        }
        var current = null, busy = false, listPage = 1, listCode = "", listTerm = "", searchWidget = null, searchEpoch = 0, controls = {}, ruleBody, editorArea, rulesArea;
        var indices = [], restoreFocus, selectedLevel = "", uncertain = false;
        var viewEpoch=0, catalogs={}, indexState=null, baseline=null, recovery=null, publishButton=null, dirtyNotice=null;
        function dirty(){return baseline!==null && current && current.Version.Estado!=="PUBLICADA" && JSON.stringify(payload())!==baseline;}
        function protectChanges(){return !dirty() || window.confirm("Hay cambios sin guardar. ¿Desea descartarlos y continuar?");}
        function updateActions(){
            var pending=dirty();
            if(publishButton){publishButton.disabled=pending || uncertain;}
            if(dirtyNotice){dirtyNotice.textContent=pending?"Cambios sin guardar. Guarde el borrador antes de publicar.":"";}
        }
        content.addEventListener("input",updateActions);
        content.addEventListener("change",updateActions);
        window.addEventListener("beforeunload",function(event){if(dirty()){event.preventDefault();event.returnValue="";}});
        function message(value) {
            status.textContent = value || "";
        }
        var fieldLabels={CODIGO:"Código del expediente",NOMBRE:"Nombre del expediente",FECHA_INICIAL:"Fecha inicial del expediente",FECHA_FINAL:"Fecha final del expediente",RESPONSABLE:"Responsable del expediente",
            NombreVisible:"Nombre de versión",FondoRef:"Fondo",InstrumentoRef:"Instrumento",AreaRef:"Área",SerieRef:"Serie",SubserieRef:"Subserie",TipoExpedienteRef:"Tipo de expediente",UnidadDocumentalRef:"Unidad documental",GabineteId:"Gabinete DA",
            ModoIntegridad:"Integridad",AmbitoIntegridad:"Ámbito",EstrategiaCarpetas:"Organización",PoliticaSubcarpetas:"Subcarpetas",PoliticaCarpetasVacias:"Carpetas vacías",PoliticaArchivosSueltos:"Archivos fuera de carpetas",PoliticaCoincidencias:"Código del expediente",Extensiones:"Extensiones permitidas",MaximoBytesArchivo:"Límite por archivo",reglas:"Reglas de campos",ejemplo:"Nombres de prueba"};
        var incidentLabels={LONGITUD_EXCEDIDA:"El texto supera la longitud permitida. Revise el límite y la extracción de esta regla.",ENTERO_INVALIDO:"Se requiere un número entero, sin punto decimal ni separadores.",DECIMAL_INVALIDO:"Se requiere un número decimal con punto decimal.",CLAVE_INVALIDA:"La clave de consulta del proveedor no cumple las restricciones de su campo de búsqueda. Revise el origen y la extracción de clave.",
            VALOR_REQUERIDO:"Falta un valor obligatorio.",VALOR_VACIO:"La fuente produjo un valor vacío.",SEGMENTO_AUSENTE:"El nombre no contiene el segmento que exige la regla.",FECHA_INVALIDA:"La fecha no coincide con el formato configurado o no es válida.",SIN_COINCIDENCIA:"El proveedor no encontró datos para la clave de consulta.",AMBIGUO:"El proveedor encontró varias coincidencias; se requiere una sola.",PROVEEDOR_INCOMPATIBLE:"Revise la versión, propiedad y tipo de clave del proveedor.",PROPIEDAD_AUSENTE:"El proveedor no devolvió la propiedad requerida.",FUENTE_REQUERIDA:"Falta configurar la fuente de este campo.",DESTINO_DUPLICADO:"Hay más de una regla para este campo.",VALOR_INDICE_INVALIDO:"El valor no cumple el tipo, longitud, rango o caracteres admitidos por el índice del gabinete. Revise el valor y la definición del índice."};
        function fieldName(field) {
            field=String(field || "");
            if(field.indexOf("CAMPO_EXPEDIENTE:")===0){return fieldLabels[field.slice(17)] || "Campo del expediente";}
            if(field.indexOf("INDICE_GABINETE:")===0){
                var ref=field.slice(16),index=indices.find(function(item){return item.value===ref;});
                return index?index.text:"Índice DA (referencia "+ref+")";
            }
            if(field==="TIPOLOGIA" || field==="TIPOLOGIA:TIPO_DOCUMENTAL"){return "Tipo documental";}
            return fieldLabels[field] || "Configuración del perfil";
        }
        function indexRejectionMessage(item,values) {
            if(item.Codigo!=="VALOR_INDICE_INVALIDO" || String(item.Campo).indexOf("INDICE_GABINETE:")!==0){return "";}
            var ref=item.Campo.slice(16),field=indices.find(function(f){return f.value===ref;}),value=(values || []).find(function(v){return v.DestinoTipo==="INDICE_GABINETE" && v.DestinoRef===ref;});
            if(!field || !value || value.Diferido || value.Valor==null){return "";}
            var text=String(value.Valor);
            if(field.Longitud>0 && text.length>field.Longitud){return "El valor tiene "+text.length+" caracteres y el índice del gabinete admite como máximo "+field.Longitud+". Revise el valor que extrae la regla y la longitud del índice.";}
            if(field.Tipo!=="ENTERO"){return "";}
            if(!/^[+-]?[0-9]+$/.test(text)){return "El índice del gabinete requiere un número entero, sin punto decimal ni separadores. Revise el valor que extrae la regla.";}
            var physical=String(field.TipoFisico || ""),match=/^(tinyint|smallint|mediumint|int|bigint)\b/.exec(physical);
            if(!match){return "";}
            var bits={tinyint:8,smallint:16,mediumint:24,int:32,bigint:64}[match[1]],unsigned=/\bunsigned\b/.test(physical),min,max,number;
            if(typeof window.BigInt==="function"){
                var one=window.BigInt(1),power=one<<window.BigInt(unsigned?bits:bits-1);
                min=unsigned?window.BigInt(0):-power;max=power-one;number=window.BigInt(text);
            }else{
                // Sin BigInt no deducir rangos de 64 bits con números aproximados.
                if(bits===64){return "";}
                var limit=Math.pow(2,unsigned?bits:bits-1);min=unsigned?0:-limit;max=limit-1;number=Number(text);
            }
            if(number<min || number>max){return "El valor "+text+" está fuera del rango admitido por el índice del gabinete: de "+min.toString()+" a "+max.toString()+". Revise el valor y el tipo del índice en el gabinete; aumentar «Longitud máxima» de la regla no amplía este rango.";}
            return "";
        }
        function incidentMessage(item,values) {
            var generic=["Revise la referencia y sus permisos vigentes.","Revise la configuración del campo indicado.","Revise la clave del proveedor.","No se pudo resolver el valor desde el proveedor."];
            if(item.Mensaje && generic.indexOf(item.Mensaje)===-1){return item.Mensaje;}
            return indexRejectionMessage(item,values) || incidentLabels[item.Codigo] || item.Mensaje || "No se pudo completar la comprobación de este campo. Revise su configuración.";
        }
        function report(error) {
            var lines = [error.mensaje || "No se confirmó la operación."];
            (error.incidencias || []).forEach(function (item) { lines.push(fieldName(item.Campo) + ": " + incidentMessage(item)); });
            if (error.resultadoIncierto) {
                uncertain = true;
                lines.push("Consulte el estado antes de volver a guardar o publicar.");
            }
            if(recovery && (error.resultadoIncierto || error.codigo==="CONFLICTO_REVISION")){recovery.hidden=false;}
            updateActions();
            message(lines.join(" · "));
        }
        function request(operation, payload, success) {
            if (busy) { return; }
            if(uncertain && ["CrearPerfil","GuardarBorrador","PublicarVersion","CrearVersion","CambiarDisponibilidad","ActualizarCabecera"].indexOf(operation)!==-1){message("Consulte el estado antes de volver a escribir.");return;}
            var fields=Array.prototype.map.call(content.querySelectorAll("fieldset"),function(el){var saved=el.disabled;el.disabled=true;return {el:el,disabled:saved};});
            var toolbarButtons=Array.prototype.map.call(ruleToolbar.querySelectorAll("button"),function(el){var saved=el.disabled;el.disabled=true;return {el:el,disabled:saved};});
            function unlock(){fields.concat(toolbarButtons).forEach(function(f){f.el.disabled=f.disabled;});}
            busy = true; dialog.setAttribute("aria-busy","true"); message("Procesando…");
            api.ejecutar(operation,payload).done(function (result) {
                unlock();
                busy = false; dialog.removeAttribute("aria-busy");
                message(result.mensaje);
                if (success) { success(result.datos); }
            }).fail(function (error) {
                unlock();
                busy = false; dialog.removeAttribute("aria-busy"); report(error);
            });
        }
        function button(parent, text, action, id) {
            var b = node("button",text,{type:"button"});
            if (id) { b.setAttribute("data-doc110",id); }
            b.addEventListener("click",function () { if (!busy) { action(); } });
            parent.appendChild(b); return b;
        }
        function input(parent, label, value, type) {
            var wrap = node("label", label + " ");
            var control = node("input",null,{type:type || "text"});
            control.value = value === undefined || value === null ? "" : value;
            wrap.appendChild(control); parent.appendChild(wrap); return control;
        }
        function select(parent,label,options,value) {
            var wrap=node("label",label+" "),control=node("select");
            options.forEach(function (item) {
                var pair=typeof item==="string" ? [item,item] : item;
                control.appendChild(node("option",pair[1],{value:pair[0]}));
            });
            control.value=value || "";
            wrap.appendChild(control);parent.appendChild(wrap);return control;
        }
        function table(parent,headers) {
            var t=node("table"),head=node("tr"),body=node("tbody");
            headers.forEach(function (label) {head.appendChild(node("th",label,{scope:"col"}));});
            var thead=node("thead");thead.appendChild(head);t.appendChild(thead);t.appendChild(body);parent.appendChild(t);return body;
        }
        function level() {
            // Es una referencia de apertura; el servidor vuelve a autorizarla.
            return host.getAttribute("data-nivel") || "";
        }
        function destroySearch() {
            searchEpoch++;
            if(searchWidget){
                var widget=searchWidget.data("autocomplete");
                if(widget && widget.searching){window.clearTimeout(widget.searching);}
                searchWidget.autocomplete("destroy");searchWidget=null;
            }
        }
        function clearContent(){viewEpoch++;baseline=null;recovery=null;publishButton=null;dirtyNotice=null;catalogs={};indexState=null;indices=[];destroySearch();ruleToolbar.hidden=true;ruleToolbar.replaceChildren();content.replaceChildren();}
        function updateRuleOverview(){
            if(!ruleBody || !content.contains(ruleBody)){return;}
            Array.prototype.forEach.call(ruleBody.children,function(row){row.updateSummary();});
            var count=ruleToolbar.querySelector('[data-doc110="cantidad-reglas"]');
            if(count){count.textContent=ruleBody.children.length+" regla"+(ruleBody.children.length===1?"":"s");}
        }
        function attachSearch(search){
            search.maxLength=160;search.setAttribute("data-doc110","buscar-texto");
            search.addEventListener("input",function(){searchEpoch++;listCode="";});
            if(!$.fn.autocomplete){return;}
            searchWidget=$(search).autocomplete({
                appendTo:dialog,minLength:2,delay:300,
                source:function(query,response){
                    var epoch=++searchEpoch, term=query.term.trim();
                    if(busy || term.length<2){response([]);return;}
                    api.ejecutar("ListarPerfiles",{pagina:1,tamanoPagina:10,termino:term}).done(function(result){
                        if(epoch!==searchEpoch || dialog.hidden || !content.contains(search)){response([]);return;}
                        response(result.datos.Elementos.map(function(p){return {label:p.Codigo+" — "+p.Nombre,value:p.Codigo,perfilId:p.PerfilId};}));
                        message(result.datos.Elementos.length ? "Seleccione un perfil de las sugerencias." : "No hay perfiles coincidentes.");
                    }).fail(function(error){response([]);if(epoch===searchEpoch && !dialog.hidden){report(error);}});
                },
                focus:function(){return false;},
                select:function(event,ui){
                    if(busy){return false;}
                    listCode=ui.item.value;listTerm="";listPage=1;search.value=listCode;
                    // Ejecutar tras finalizar el evento del widget, antes de destruir el menú.
                    window.setTimeout(function(){if(!dialog.hidden){list();}},0);
                    return false;
                }
            });
            search.addEventListener("keydown",function(event){
                if(event.key==="Escape"){
                    var widget=$(search).data("autocomplete");
                    if(widget && widget.menu.element.is(":visible")){$(search).autocomplete("close");event.preventDefault();event.stopPropagation();}
                }
            },true);
        }
        function list() {
            if(!protectChanges()){return;}
            var restoreSearchFocus=content.contains(document.activeElement);
            destroySearch();
            request("ListarPerfiles",{pagina:listPage,tamanoPagina:25,codigo:listCode || null,termino:listTerm || null},function (data) {
                clearContent();current=null;uncertain=false;
                var actions=node("div",null,{"class":"doc110-actions"});content.appendChild(actions);
                button(actions,"Nuevo perfil",createForm,"nuevo");
                var search=input(actions,"Buscar por código o nombre",listCode || listTerm);attachSearch(search);
                button(actions,"Buscar",function(){listCode="";listTerm=search.value.trim();listPage=1;list();},"buscar");
                button(actions,"Limpiar",function(){listCode="";listTerm="";listPage=1;list();},"limpiar-busqueda");
                button(actions,"Anterior",function () {if(listPage>1){listPage--;list();}});
                button(actions,"Siguiente",function () {if(listPage*25<data.Total){listPage++;list();}});
                var body=table(content,["Código","Nombre","Disponibilidad","Acción"]);
                data.Elementos.forEach(function (profile) {
                    var row=node("tr");row.appendChild(node("td",profile.Codigo));row.appendChild(node("td",profile.Nombre));
                    row.appendChild(node("td",profile.Activo?"Activo":"Inactivo"));
                    var cell=node("td");button(cell,"Abrir",function(){load(profile.PerfilId,null);});row.appendChild(cell);body.appendChild(row);
                });
                if(!data.Elementos.length){content.appendChild(node("p","No hay perfiles en esta página."));}
                if(restoreSearchFocus && !dialog.hidden && document.activeElement===document.body){search.focus();}
            });
        }
        function createForm() {
            clearContent();current=null;
            var form=node("div",null,{"class":"doc110-fields"});content.appendChild(form);
            var code=input(form,"Código",""),name=input(form,"Nombre",""),description=input(form,"Descripción","");
            code.maxLength=64;name.maxLength=160;description.maxLength=16000;
            code.setAttribute("data-doc110","codigo");name.setAttribute("data-doc110","nombre");
            button(content,"Crear borrador",function () {
                if(uncertain){message("La creación anterior no está confirmada. Busque el código en el listado antes de repetir.");return;}
                request("CrearPerfil",{codigo:code.value,nombre:name.value,descripcion:description.value},render);
            },"crear");
            button(content,"Consultar listado",list);
            code.focus();
        }
        function load(id,version) {
            if(!protectChanges()){return;}
            request("ObtenerPerfil",{perfilId:id,versionId:version},function (data) {uncertain=false;render(data);});
        }
        function configuration() {
            var c=Object.assign({},current.Version.Configuracion);
            Object.keys(controls).forEach(function (key) {
                if(key==="GabineteId"){c[key]=controls[key].value?Number(controls[key].value):null;}
                else if(key==="MaximoBytesArchivo"){c[key]=controls[key].value?Number(controls[key].value):null;}
                else if(key==="Extensiones"){c[key]=controls[key].value.split(",").map(function(v){return v.trim().replace(/^\./,"").toLowerCase();}).filter(Boolean);}
                else {c[key]=controls[key].value || null;}
            });
            if(c.ModoIntegridad==="PARCIAL_PERMITIDO"){c.AmbitoIntegridad="NO_APLICA";}
            if(c.GabineteId!==current.Version.Configuracion.GabineteId){c.HuellaOrigenGabinete=null;}
            return c;
        }
        var prerequisites={AreaRef:["InstrumentoRef"],SerieRef:["InstrumentoRef","AreaRef"],SubserieRef:["InstrumentoRef","AreaRef","SerieRef"],INDICE:["GabineteId"]};
        function catalog(key,catalogName,dependencies) {
            var isIndex=key==="INDICE",el=isIndex?null:controls[key],wrap=isIndex?rulesArea:el.parentNode;
            var state={epoch:0,page:1,items:[],status:"WAITING"},view=viewEpoch;
            var note=node("small","",{"aria-live":"polite","data-doc110-catalog":key});wrap.appendChild(note);
            var retry=button(current.Version.Estado==="PUBLICADA"?content:wrap,"Reintentar "+catalogName.toLowerCase(),function(){state.load(true);},"reintentar-"+key);retry.hidden=true;
            var more=button(wrap,"Más resultados",function(){state.load(false);},"mas-"+key);more.hidden=true;
            function valid(){return view===viewEpoch && !dialog.hidden;}
            function ready(){return (prerequisites[key] || []).every(function(parent){return !!controls[parent].value;});}
            function refreshRows(clear){if(isIndex && ruleBody){Array.prototype.forEach.call(ruleBody.children,function(row){row.refreshIndices(clear);});}}
            function status(value,text){state.status=value;note.textContent=text;note.setAttribute("data-state",value);retry.hidden=value!=="ERROR";if(el){el.disabled=value!=="READY" || (!state.items.length && !el.value);}refreshRows(false);}
            function merge(options){
                options.forEach(function(option){
                    if(!state.items.some(function(o){return o.value===option.value;})){state.items.push(option);}
                    if(el){
                        var found=Array.prototype.find.call(el.options,function(o){return o.value===option.value;});
                        if(found){found.textContent=option.text;}else{el.appendChild(node("option",option.text,{value:option.value}));}
                    }
                });
                if(isIndex){indices=state.items.slice();refreshRows(false);}
            }
            state.load=function(reset){
                var epoch=++state.epoch,config=configuration();
                if(reset){
                    state.page=1;state.items=[];more.hidden=true;
                    if(el){var keep=el.value;el.replaceChildren(node("option","Seleccione…",{value:""}));if(keep){el.appendChild(node("option","Referencia guardada: "+keep,{value:keep}));el.value=keep;}}
                    else{indices=[];}
                }
                if(!ready()){status("WAITING","Seleccione primero los campos anteriores.");return;}
                var page=state.page,selected=el?el.value:"";
                status("LOADING","Cargando opciones…");more.hidden=true;
                api.ejecutar("ConsultarCatalogo",{catalogo:catalogName,configuracion:config,pagina:page,tamanoPagina:50}).done(function(result){
                    if(!valid() || epoch!==state.epoch){return;}
                    var options=result.datos;merge(options);state.page=page+1;
                    status("READY",state.items.length?"":"Sin opciones disponibles.");
                    more.hidden=isIndex || options.length!==50;
                    if(isIndex && options.length===50){state.load(false);return;}
                    if(selected && !state.items.some(function(o){return o.value===selected;})){
                        api.ejecutar("ConsultarCatalogo",{catalogo:catalogName,configuracion:config,pagina:1,tamanoPagina:1,referencia:selected}).done(function(found){
                            if(!valid() || epoch!==state.epoch || el.value!==selected){return;}
                            if(found.datos.length){merge(found.datos);}
                            else{note.textContent="La referencia guardada ya no está disponible en este contexto. Seleccione otra opción.";}
                        }).fail(function(){if(valid() && epoch===state.epoch){status("ERROR","No se pudo comprobar la referencia guardada.");}});
                    }
                }).fail(function(){if(valid() && epoch===state.epoch){more.hidden=true;status("ERROR","No se pudieron cargar las opciones.");}});
            };
            state.reset=function(){
                state.epoch++;state.page=1;state.items=[];more.hidden=true;
                if(el){el.replaceChildren(node("option","Seleccione…",{value:""}));}
                else{indices=[];refreshRows(true);}
                state.load(true);
            };
            if(el){el.addEventListener("change",function(){
                // Limpiar todos los descendientes antes de iniciar sus consultas.
                (dependencies || []).forEach(function(field){controls[field].replaceChildren(node("option","Seleccione…",{value:""}));});
                (dependencies || []).forEach(function(field){catalogs[field].reset();});
                if(key==="GabineteId" && indexState){indexState.reset();message("Revise los destinos de índice del nuevo gabinete.");}
            });}
            if(isIndex){indexState=state;}else{catalogs[key]=state;}
            return state;
        }
        function render(profile) {
            var expanded={};
            if(current && current.PerfilId===profile.PerfilId && current.Version.VersionId===profile.Version.VersionId && ruleBody && content.contains(ruleBody)){
                Array.prototype.forEach.call(ruleBody.children,function(row){var r=row.readRule(0);if(row.isExpanded()){expanded[JSON.stringify([r.DestinoTipo,r.DestinoRef])]=true;}});
            }
            current=profile;uncertain=false;controls={};indices=[];clearContent();
            var version=profile.Version,c=version.Configuracion,published=version.Estado==="PUBLICADA";
            var actions=node("div",null,{"class":"doc110-actions"});content.appendChild(actions);
            content.appendChild(node("h3",profile.Codigo+" — versión "+version.Numero+" · "+version.Estado));
            button(actions,"Volver al listado",list);
            recovery=button(actions,"Consultar estado",function(){load(profile.PerfilId,version.VersionId);},"recargar");recovery.hidden=true;
            button(actions,"Historial",history,"historial");
            button(actions,profile.Activo?"Desactivar":"Activar",function(){
                if(!protectChanges()){return;}
                request("CambiarDisponibilidad",{perfilId:profile.PerfilId,revisionPerfilEsperada:profile.Revision,activo:!profile.Activo},render);
            });
            button(actions,"Editar nombre y descripción",function(){
                if(!protectChanges()){return;}
                var name=window.prompt("Nombre del perfil",profile.Nombre);
                if(name===null){return;}
                var description=window.prompt("Descripción",profile.Descripcion || "");
                if(description===null){return;}
                request("ActualizarCabecera",{perfilId:profile.PerfilId,revisionPerfilEsperada:profile.Revision,nombre:name,descripcion:description},render);
            });
            if(published){
                button(actions,"Crear nueva versión",function(){
                    request("CrearVersion",{perfilId:profile.PerfilId,versionOrigenId:version.VersionId,revisionPerfilEsperada:profile.Revision,revisionOrigenEsperada:version.Revision},render);
                },"nueva-version");
            }
            editorArea=node("fieldset",null,{"class":"doc110-fields"});editorArea.disabled=published;content.appendChild(editorArea);
            editorArea.appendChild(node("legend","Configuración de la versión"));
            controls.NombreVisible=input(editorArea,"Nombre de versión",c.NombreVisible);controls.NombreVisible.maxLength=160;
            [
                ["FondoRef","Fondo","FONDO",[]],
                ["InstrumentoRef","Instrumento","INSTRUMENTO",["AreaRef","SerieRef","SubserieRef"]],
                ["AreaRef","Área","AREA",["SerieRef","SubserieRef"]],
                ["SerieRef","Serie","SERIE",["SubserieRef"]],
                ["SubserieRef","Subserie (opcional)","SUBSERIE",[]],
                ["TipoExpedienteRef","Tipo de expediente","TIPO_EXPEDIENTE",[]],
                ["UnidadDocumentalRef","Unidad documental","UNIDAD_DOCUMENTAL",[]],
                ["GabineteId","Gabinete DA","GABINETE",[]]
            ].forEach(function (field) {
                controls[field[0]]=select(editorArea,field[1],[["","Seleccione…"]].concat(c[field[0]]?[[String(c[field[0]]),"Seleccionado: "+c[field[0]]]]:[]),String(c[field[0]] || ""));
                catalog(field[0],field[2],field[3]);
            });
            controls.ModoIntegridad=select(editorArea,"Integridad",[["COMPLETO_OBLIGATORIO","Todos los archivos"],["PARCIAL_PERMITIDO","Permitir resultado parcial"]],c.ModoIntegridad);
            controls.AmbitoIntegridad=select(editorArea,"Ámbito",[["","Seleccione…"],["EXPEDIENTE","Por expediente"],["LOTE","Por lote"],["NO_APLICA","No aplica"]],c.AmbitoIntegridad);
            controls.ModoIntegridad.addEventListener("change",function(){
                controls.AmbitoIntegridad.value=controls.ModoIntegridad.value==="PARCIAL_PERMITIDO"?"NO_APLICA":"";
                controls.AmbitoIntegridad.disabled=controls.ModoIntegridad.value==="PARCIAL_PERMITIDO";
            });
            controls.AmbitoIntegridad.disabled=c.ModoIntegridad==="PARCIAL_PERMITIDO";
            if(controls.AmbitoIntegridad.disabled){controls.AmbitoIntegridad.value="NO_APLICA";}
            controls.EstrategiaCarpetas=select(editorArea,"Organización",[["","Seleccione…"],["RAIZ_EXPEDIENTE","Cada carpeta raíz crea un expediente"]],c.EstrategiaCarpetas);
            controls.PoliticaSubcarpetas=select(editorArea,"Subcarpetas",[["","Seleccione…"],["INCLUIR","Incluir documentos en el expediente raíz"],["RECHAZAR","Rechazar subcarpetas"]],c.PoliticaSubcarpetas);
            controls.PoliticaCarpetasVacias=select(editorArea,"Carpetas vacías",[["","Seleccione…"],["RECHAZAR","Rechazar"]],c.PoliticaCarpetasVacias);
            controls.PoliticaArchivosSueltos=select(editorArea,"Archivos fuera de carpetas",[["","Seleccione…"],["RECHAZAR","Rechazar"]],c.PoliticaArchivosSueltos);
            controls.PoliticaCoincidencias=select(editorArea,"Código del expediente",[["PERMITIR","Permitir códigos repetidos; cada expediente tiene su ID"]],"PERMITIR");
            controls.Extensiones=input(editorArea,"Extensiones permitidas, separadas por coma",(c.Extensiones || []).join(","));
            controls.MaximoBytesArchivo=input(editorArea,"Límite de bytes por archivo (opcional)",c.MaximoBytesArchivo,"number");
            controls.MaximoBytesArchivo.min="1";controls.MaximoBytesArchivo.step="1";
            Object.keys(controls).forEach(function(key){controls[key].setAttribute("data-doc110-field",key);});
            rulesArea=node("fieldset");rulesArea.disabled=published;content.appendChild(rulesArea);
            rulesArea.appendChild(node("legend","Reglas de campos"));
            catalog("INDICE","INDICE",[]);
            ruleToolbar.hidden=false;
            var add=button(ruleToolbar,"Agregar regla",function(){var row=addRule({DestinoTipo:"CAMPO_EXPEDIENTE",DestinoRef:"CODIGO",FuenteTipo:"CARPETA",TipoValor:"TEXTO",Extraccion:"COMPLETO",EtapaResolucion:"PREFLIGHT",LongitudMaxima:45,Obligatoria:true},true);updateActions();row.reveal();},"agregar-regla");add.disabled=published;
            ruleToolbar.appendChild(node("span","",{"data-doc110":"cantidad-reglas","aria-live":"polite"}));
            var scroll=node("div",null,{"class":"doc110-scroll"});rulesArea.appendChild(scroll);
            ruleBody=table(scroll,["Reglas configuradas"]);ruleBody.parentNode.classList.add("doc110-rules-table");
            version.Reglas.forEach(function(rule){addRule(rule,expanded[JSON.stringify([rule.DestinoTipo,rule.DestinoRef])]===true);});updateRuleOverview();
            if(!published){
                button(content,"Guardar borrador",function(){
                    if(uncertain){message("Consulte el estado antes de repetir.");return;}
                    request("GuardarBorrador",payload(),render);
                },"guardar");
                publishButton=button(content,"Publicar versión guardada",function(){
                    if(dirty()){updateActions();return;}
                    if(uncertain){message("Consulte el estado antes de repetir.");return;}
                    request("PublicarVersion",{perfilId:profile.PerfilId,versionId:version.VersionId,revisionPerfilEsperada:profile.Revision,revisionVersionEsperada:version.Revision},render);
                },"publicar");
            }
            dirtyNotice=node("p","",{"aria-live":"polite","data-doc110":"cambios-pendientes"});content.appendChild(dirtyNotice);
            var examples=node("fieldset");examples.appendChild(node("legend","Probar nombres sin cargar archivos"));content.appendChild(examples);
            var folder=input(examples,"Nombre de carpeta",""),file=input(examples,"Nombre de archivo","");
            folder.setAttribute("data-doc110","ejemplo-carpeta");file.setAttribute("data-doc110","ejemplo-archivo");
            button(examples,"Probar reglas",function(){
                resultArea.replaceChildren();
                var data=payload();data.ejemplos=[{NombreCarpeta:folder.value,NombreArchivo:file.value,Expediente:{}}];
                request("EnsayarReglas",data,function(results){
                    var incidents=0,pending=0;
                    results.forEach(function(result){
                        result.Valores.forEach(function(v){var rejected=result.Incidencias.some(function(e){return e.Campo===v.DestinoTipo+":"+v.DestinoRef;});if(v.Diferido){pending++;}resultArea.appendChild(node("p",fieldName(v.DestinoTipo+":"+v.DestinoRef)+": "+(v.Diferido?"Se resolverá tras crear el expediente":(v.Valor==null?"Sin valor":v.Valor))+(v.Procedencia==="PROPUESTA_PERFIL"?" (propuesta del perfil; pendiente de confirmar con el expediente)":"")+(rejected?" (requiere corrección; ver incidencia)":"")));});
                        result.Incidencias.forEach(function(e){
                            incidents++;
                            var item=node("article",null,{"class":"doc110-rule-incident","data-doc110":"incidencia-ensayo"});
                            item.appendChild(node("strong",fieldName(e.Campo)));
                            item.appendChild(node("p",incidentMessage(e,result.Valores)));
                            var detail=node("details");detail.appendChild(node("summary","Ver detalle técnico"));
                            detail.appendChild(node("p","Referencia: "+e.Campo+" · Código: "+e.Codigo));item.appendChild(detail);resultArea.appendChild(item);
                        });
                    });
                    var summary=incidents?"Prueba con "+incidents+" incidencia"+(incidents===1?"":"s")+". Revise las reglas indicadas.":pending?"Prueba sin errores; "+pending+" campo"+(pending===1?" pendiente":"s pendientes")+" de resolver tras crear el expediente.":"Prueba sin incidencias para los nombres indicados.";
                    message(summary);
                    resultArea.prepend(node("p",summary,{"class":"doc110-test-summary","data-tone":incidents?"error":pending?"pending":"success"}));
                });
            },"ensayar");
            var resultArea=node("div",null,{"aria-live":"polite","data-doc110":"resultado-ensayo"});examples.appendChild(resultArea);
            if(published){
                button(content,"Comprobar compatibilidad con el nivel seleccionado",function(){
                    if(!selectedLevel){message("Seleccione un nivel de Producción y vuelva a abrir el administrador.");return;}
                    request("ValidarSeleccionPerfil",{perfilId:profile.PerfilId,versionId:version.VersionId,nivelRef:selectedLevel,proposito:"ADMINISTRACION"},function(data){
                        message("Perfil compatible con el nivel "+data.nivelRef+". Esta comprobación no inicia una importación.");
                    });
                },"seleccionar");
            }
            baseline=JSON.stringify(payload());updateActions();
            Object.keys(catalogs).forEach(function(key){catalogs[key].load(true);});
            indexState.load(true);
        }
        function addRule(rule,expanded) {
            var row=node("tr"),cell=node("td"),cells=[],detail=node("details",null,{"class":"doc110-rule-editor","data-doc110":"regla"});detail.open=expanded===true;
            var summary=node("summary",null,{"data-doc110":"resumen-regla"}),summaryTitle=node("strong"),summarySource=node("span");summary.appendChild(summaryTitle);summary.appendChild(summarySource);detail.appendChild(summary);
            var grid=node("div",null,{"class":"doc110-rule-fields"});detail.appendChild(grid);cell.appendChild(detail);row.appendChild(cell);
            ["Destino","Fuente","Extracción","Conversión","Opciones","Acción"].forEach(function(label){var section=node("div");section.appendChild(node("h4",label));cells.push(section);grid.appendChild(section);});
            var destination=select(cells[0],"Destino",[["CAMPO_EXPEDIENTE","Expediente"],["INDICE_GABINETE","Índice DA"],["TIPOLOGIA","Tipo documental"]],rule.DestinoTipo);
            var ref=select(cells[0],"Campo",[],null);
            function fillDestinations(keep){
                var options=destination.value==="CAMPO_EXPEDIENTE" ?
                    [["CODIGO","Código del expediente"],["NOMBRE","Nombre del expediente"],["FECHA_INICIAL","Fecha inicial"],["FECHA_FINAL","Fecha final"],["RESPONSABLE","Responsable"]] :
                    (destination.value==="TIPOLOGIA" ? [["TIPO_DOCUMENTAL","Tipo documental"]] : indices.map(function(item){return [item.value,item.text];}));
                ref.replaceChildren(node("option","Seleccione…",{value:""}));
                options.forEach(function(item){
                    var option=node("option",item[1],{value:item[0]});
                    if(destination.value==="INDICE_GABINETE"){var metadata=indices.find(function(i){return i.value===item[0];});option.disabled=metadata && metadata.Tipo==="NO_SOPORTADO";}
                    ref.appendChild(option);
                });
                if(keep && !options.some(function(item){return item[0]===keep;})){ref.appendChild(node("option","Campo guardado: "+keep,{value:keep}));}
                ref.value=keep || (destination.value==="TIPOLOGIA"?"TIPO_DOCUMENTAL":"");
            }
            fillDestinations(rule.DestinoRef);
            destination.addEventListener("change",function(){fillDestinations(null);});
            row.refreshIndices=function(clear){if(destination.value==="INDICE_GABINETE"){fillDestinations(clear?null:ref.value);sync(false);}};
            var source=select(cells[1],"Origen",[["CONSTANTE","Constante"],["CARPETA","Carpeta"],["ARCHIVO","Archivo"],["EXPEDIENTE","Expediente confirmado"],["PROVEEDOR","Proveedor de datos"]],rule.FuenteTipo);
            var savedProvider=rule.Proveedor || {},savedKey=savedProvider.Clave || {},providerEpoch=0,providerPage=1,providerItems=[];
            var providerBox=node("div");cells[1].appendChild(providerBox);
            var providerSelect=select(providerBox,"Proveedor y versión",[["","Seleccione…"]],""),providerProperty=select(providerBox,"Propiedad del proveedor",[["","Seleccione…"]],"");
            var providerStatus=node("small","",{"aria-live":"polite"});providerBox.appendChild(providerStatus);
            var keySource=select(providerBox,"Origen de la clave",["CONSTANTE","CARPETA","ARCHIVO","EXPEDIENTE"],savedKey.FuenteTipo || "ARCHIVO");
            var keyConstant=input(providerBox,"Clave constante",savedKey.ValorConstante);
            var keyProperty=select(providerBox,"Propiedad del expediente para la clave",[["","Seleccione…"]].concat(inheritedProperties),savedKey.PropiedadExpediente);
            var keyExtraction=select(providerBox,"Extracción de clave",["COMPLETO","SEGMENTO"],savedKey.Extraccion || "COMPLETO");
            var keySeparator=input(providerBox,"Separador de clave",savedKey.Separador),keyPosition=input(providerBox,"Segmento de clave (desde 1)",(savedKey.Posicion || 0)+1,"number");keyPosition.min="1";keyPosition.max="128";
            var keyType=select(providerBox,"Tipo de clave",["TEXTO","ENTERO","DECIMAL","FECHA"],savedKey.TipoValor || "TEXTO");
            var keyFormat=select(providerBox,"Formato de fecha de clave",[["","No aplica"],"yyyy-MM-dd","yyyyMMdd","dd-MM-yyyy","dd_MM_yyyy"],savedKey.FormatoFecha);
            var keyLength=input(providerBox,"Longitud máxima de clave",savedKey.LongitudMaxima || 160,"number");keyLength.min="1";keyLength.max="4096";
            var keyExtension=input(providerBox,"Quitar extensión de clave","","checkbox");keyExtension.checked=!!savedKey.QuitarExtension;
            var keyTrim=input(providerBox,"Recortar espacios de clave","","checkbox");keyTrim.checked=!!savedKey.RecortarEspacios;
            var providerMore=button(providerBox,"Más proveedores",function(){loadProviders(providerPage+1);});providerMore.hidden=true;
            var providerRetry=button(providerBox,"Reintentar proveedores",function(){loadProviders(providerPage);});providerRetry.hidden=true;
            function providerId(p){return p.ProveedorId+":"+p.VersionId;}
            function fillProviderProperties(keep){
                providerProperty.replaceChildren(node("option","Seleccione…",{value:""}));
                var chosen=providerItems.find(function(p){return providerId(p)===providerSelect.value;});
                (chosen?chosen.Propiedades:[]).forEach(function(p){providerProperty.appendChild(node("option",p.Nombre,{value:p.Nombre}));});
                if(keep && !Array.prototype.some.call(providerProperty.options,function(o){return o.value===keep;})){providerProperty.appendChild(node("option","Propiedad guardada: "+keep,{value:keep}));}
                providerProperty.value=keep || "";
            }
            if(savedProvider.ProveedorId && savedProvider.VersionId){providerSelect.appendChild(node("option","Proveedor guardado: "+providerId(savedProvider),{value:providerId(savedProvider)}));providerSelect.value=providerId(savedProvider);fillProviderProperties(savedProvider.Propiedad);}
            function loadProviders(page,exact){
                var epoch=++providerEpoch,view=viewEpoch,keep=providerSelect.value,keepProperty=providerProperty.value;
                providerStatus.textContent="Consultando proveedores…";providerRetry.hidden=true;
                api.ejecutar("ConsultarProveedores",{pagina:page,tamanoPagina:25,proveedorId:exact?savedProvider.ProveedorId:null,versionId:exact?savedProvider.VersionId:null}).done(function(response){
                    if(view!==viewEpoch || epoch!==providerEpoch || !row.isConnected || source.value!=="PROVEEDOR"){return;}
                    var items=response.datos;
                    if(page===1 && !exact){providerItems=[];providerSelect.replaceChildren(node("option","Seleccione…",{value:""}));}
                    items.forEach(function(p){if(!providerItems.some(function(old){return providerId(old)===providerId(p);})){providerItems.push(p);}var old=Array.prototype.find.call(providerSelect.options,function(o){return o.value===providerId(p);});if(old){old.textContent=p.Nombre+" · versión "+p.Numero;}else{providerSelect.appendChild(node("option",p.Nombre+" · versión "+p.Numero,{value:providerId(p)}));}});
                    if(keep && !Array.prototype.some.call(providerSelect.options,function(o){return o.value===keep;})){providerSelect.appendChild(node("option","Proveedor guardado: "+keep,{value:keep}));}
                    providerSelect.value=keep;fillProviderProperties(keepProperty);
                    row.updateSummary();
                    if(!exact){providerPage=page;providerMore.hidden=items.length<25;}
                    providerStatus.textContent=providerItems.length?"Seleccione una propiedad de la versión autorizada.":"No hay proveedores habilitados para este ámbito.";
                    if(exact && !items.length){providerStatus.textContent="La versión guardada no está disponible. No se sustituirá automáticamente.";}
                    if(!exact && savedProvider.VersionId && keep===providerId(savedProvider) && !providerItems.some(function(p){return providerId(p)===keep;})){loadProviders(1,true);}
                }).fail(function(){if(view!==viewEpoch || epoch!==providerEpoch || !row.isConnected){return;}providerStatus.textContent="No se pudo consultar el catálogo de proveedores.";providerRetry.hidden=false;});
            }
            providerSelect.addEventListener("change",function(){providerEpoch++;fillProviderProperties(null);var p=providerItems.find(function(x){return providerId(x)===providerSelect.value;});if(p){keyType.value=p.TipoClave;}updateActions();});
            source.addEventListener("change",function(){providerEpoch++;if(source.value==="PROVEEDOR"){loadProviders(1);}});
            function syncKey(){var names=keySource.value==="ARCHIVO" || keySource.value==="CARPETA";keyConstant.disabled=keySource.value!=="CONSTANTE";keyProperty.disabled=keySource.value!=="EXPEDIENTE";keyExtraction.disabled=!names;keySeparator.disabled=keyPosition.disabled=!names || keyExtraction.value!=="SEGMENTO";keyExtension.disabled=keySource.value!=="ARCHIVO";keyFormat.disabled=keyType.value!=="FECHA";}
            [keySource,keyExtraction,keyType].forEach(function(el){el.addEventListener("change",syncKey);});syncKey();
            var providerControls={proveedor:providerSelect,proveedorPropiedad:providerProperty,claveFuente:keySource,claveConstante:keyConstant,clavePropiedad:keyProperty,claveExtraccion:keyExtraction,claveSeparador:keySeparator,clavePosicion:keyPosition,claveTipo:keyType,claveFormato:keyFormat,claveLongitud:keyLength,claveExtension:keyExtension};
            Object.keys(providerControls).forEach(function(k){providerControls[k].setAttribute("data-doc110-rule",k);});
            var constant=input(cells[1],"Valor constante",rule.ValorConstante);
            var property=select(cells[1],"Propiedad heredada",[["","No aplica"]].concat(inheritedProperties),rule.PropiedadExpediente);
            var extraction=select(cells[2],"Método",[["COMPLETO","Nombre completo"],["SEGMENTO","Segmento"]],rule.Extraccion);
            var separator=input(cells[2],"Separador",rule.Separador),position=input(cells[2],"Segmento (desde 1)",(rule.Posicion || 0)+1,"number");position.min="1";position.max="128";
            var type=select(cells[3],"Tipo",["TEXTO","ENTERO","DECIMAL","FECHA","REFERENCIA"],rule.TipoValor);
            var format=select(cells[3],"Formato de fecha",[["","No aplica"],"yyyy-MM-dd","yyyyMMdd","dd-MM-yyyy","dd_MM_yyyy","dd/MM/yyyy H:mm:ss"],rule.FormatoFecha);
            var length=input(cells[3],"Longitud máxima",rule.LongitudMaxima || 160,"number");length.min="1";length.max="4096";
            ref.addEventListener("change",function(){
                if(destination.value==="INDICE_GABINETE"){
                    var field=indices.find(function(item){return item.value===ref.value;});
                    if(field){type.value=field.Tipo;length.value=field.Longitud;}
                }
            });
            function check(label,value){var el=input(cells[4],label,"","checkbox");el.checked=!!value;return el;}
            var required=check("Obligatoria",rule.Obligatoria),extension=check("Quitar extensión",rule.QuitarExtension),trim=check("Recortar espacios",rule.RecortarEspacios);
            var warning=node("small","",{"aria-live":"polite","data-doc110":"regla-aviso"});cells[0].appendChild(warning);
            function sync(changed){
                if(changed && ((destination.value==="CAMPO_EXPEDIENTE" && (source.value==="ARCHIVO" || source.value==="EXPEDIENTE")) || (destination.value!=="INDICE_GABINETE" && source.value==="PROVEEDOR"))){source.value="";providerEpoch++;}
                var names=source.value==="CARPETA" || source.value==="ARCHIVO",inherited=source.value==="EXPEDIENTE";
                if(changed && destination.value==="CAMPO_EXPEDIENTE" && ref.value){type.value={CODIGO:"TEXTO",NOMBRE:"TEXTO",FECHA_INICIAL:"FECHA",FECHA_FINAL:"FECHA",RESPONSABLE:"REFERENCIA"}[ref.value] || type.value;}
                Array.prototype.forEach.call(source.options,function(o){o.disabled=(destination.value==="CAMPO_EXPEDIENTE" && (o.value==="ARCHIVO" || o.value==="EXPEDIENTE")) || (o.value==="PROVEEDOR" && destination.value!=="INDICE_GABINETE");});
                providerBox.hidden=source.value!=="PROVEEDOR";
                constant.disabled=source.value!=="CONSTANTE";property.disabled=!inherited;extraction.disabled=!names;
                separator.disabled=position.disabled=!names || extraction.value!=="SEGMENTO";
                extension.disabled=source.value!=="ARCHIVO";
                format.disabled=type.value!=="FECHA";
                Array.prototype.forEach.call(format.options,function(o){o.disabled=type.value==="FECHA" && (source.value==="CONSTANTE" || inherited) && o.value!=="yyyy-MM-dd" || (type.value==="FECHA" && source.value==="PROVEEDOR" && ["yyyy-MM-dd","dd/MM/yyyy H:mm:ss"].indexOf(o.value)===-1);});
                if(changed){
                    if(!names){extraction.value="COMPLETO";}
                    if(extension.disabled){extension.checked=false;}
                    if(type.value==="FECHA" && (source.value==="CONSTANTE" || inherited)){format.value="yyyy-MM-dd";}
                }
                ref.disabled=destination.value==="INDICE_GABINETE" && (!indexState || indexState.status!=="READY");
                var index=indices.find(function(i){return i.value===ref.value;});
                warning.textContent=!source.value?"Seleccione una fuente válida para este destino.":destination.value==="CAMPO_EXPEDIENTE" && (source.value==="ARCHIVO" || inherited)?"La fuente guardada no es válida para un campo de expediente.":
                    (destination.value==="INDICE_GABINETE" && ref.value && indexState && indexState.status==="READY" && !index?"El índice guardado no está disponible para este gabinete.":
                    (destination.value==="INDICE_GABINETE" && index && index.Tipo==="NO_SOPORTADO"?"Este índice tiene un tipo no soportado.":""));
                row.updateSummary();
            }
            row.updateSummary=function(){
                var number=Array.prototype.indexOf.call(ruleBody.children,row)+1;
                summaryTitle.textContent="Regla "+number+" · "+(ref.value?fieldName(destination.value+":"+ref.value):"Seleccione un campo");
                var selected=source.options[source.selectedIndex],text=selected?selected.textContent:"Seleccione un origen";
                if(source.value==="PROVEEDOR"){var p=providerSelect.options[providerSelect.selectedIndex];text+=" · "+(p?p.textContent:"Seleccione proveedor")+(providerProperty.value?" → "+providerProperty.value:"");}
                summarySource.textContent=text+" · "+type.value+(required.checked?" · Obligatoria":" · Opcional");
            };
            row.isExpanded=function(){return detail.open;};
            row.reveal=function(){detail.open=true;if(typeof detail.scrollIntoView==="function"){detail.scrollIntoView({block:"start"});}destination.focus({preventScroll:true});};
            row.addEventListener("input",row.updateSummary);row.addEventListener("change",row.updateSummary);
            [destination,source,extraction,type,property,ref].forEach(function(el){el.addEventListener("change",function(){sync(true);});});
            var ruleControls={destino:destination,campo:ref,fuente:source,constante:constant,propiedad:property,extraccion:extraction,separador:separator,posicion:position,tipo:type,formato:format,longitud:length,extension:extension};
            Object.keys(ruleControls).forEach(function(key){ruleControls[key].setAttribute("data-doc110-rule",key);});
            sync(false);
            button(cells[5],"Quitar",function(){row.remove();updateRuleOverview();updateActions();});
            row.readRule=function(index){
                var value={ReglaId:rule.ReglaId || null,DestinoTipo:destination.value,DestinoRef:ref.value,
                    FuenteTipo:source.value,EtapaResolucion:source.value==="EXPEDIENTE"?"TRAS_EXPEDIENTE":"PREFLIGHT",
                    Obligatoria:required.checked,Orden:index,ValorConstante:source.value==="CONSTANTE"?constant.value:null,
                    PropiedadExpediente:source.value==="EXPEDIENTE"?property.value:null,Extraccion:extraction.value,
                    Separador:extraction.value==="SEGMENTO"?(separator.value || null):null,Posicion:extraction.value==="SEGMENTO"?Number(position.value)-1:0,QuitarExtension:extension.checked,
                    RecortarEspacios:trim.checked,TipoValor:type.value,FormatoFecha:type.value==="FECHA"?(format.value || null):null,LongitudMaxima:Number(length.value)};
                if(source.value==="PROVEEDOR"){
                    var ids=providerSelect.value.split(":"),names=keySource.value==="ARCHIVO" || keySource.value==="CARPETA",segment=names && keyExtraction.value==="SEGMENTO";
                    value.Extraccion="COMPLETO";value.Separador=null;value.Posicion=0;value.QuitarExtension=false;value.EtapaResolucion=keySource.value==="EXPEDIENTE"?"TRAS_EXPEDIENTE":"PREFLIGHT";
                    if(value.TipoValor==="FECHA"){value.FormatoFecha=format.value || "yyyy-MM-dd";}
                    value.Proveedor={ProveedorId:ids[0] || null,VersionId:ids[1] || null,Propiedad:providerProperty.value,Clave:{FuenteTipo:keySource.value,ValorConstante:keySource.value==="CONSTANTE"?keyConstant.value:null,PropiedadExpediente:keySource.value==="EXPEDIENTE"?keyProperty.value:null,Extraccion:segment?"SEGMENTO":"COMPLETO",Separador:segment?keySeparator.value:null,Posicion:segment?Number(keyPosition.value)-1:0,QuitarExtension:keySource.value==="ARCHIVO" && keyExtension.checked,RecortarEspacios:keyTrim.checked,TipoValor:keyType.value,FormatoFecha:keyType.value==="FECHA"?(keySource.value==="CONSTANTE" || keySource.value==="EXPEDIENTE"?"yyyy-MM-dd":keyFormat.value):null,LongitudMaxima:Number(keyLength.value)}};
                }
                return value;
            };
            ruleBody.appendChild(row);
            updateRuleOverview();
            if(source.value==="PROVEEDOR"){loadProviders(1);}
            return row;
        }
        function payload() {
            return {perfilId:current.PerfilId,versionId:current.Version.VersionId,revisionVersionEsperada:current.Version.Revision,
                configuracion:configuration(),reglas:Array.prototype.map.call(ruleBody.children,function(row,index){return row.readRule(index);})};
        }
        function history(page) {
            if(!protectChanges()){return;}
            page=page || 1;
            var id=current.PerfilId;
            request("ConsultarHistorial",{perfilId:id,pagina:page,tamanoPagina:50},function(versions){
                clearContent();
                versions.forEach(function(v){button(content,"Versión "+v.Numero+" · "+v.Estado,function(){load(id,v.VersionId);});});
                if(page>1){button(content,"Versiones más recientes",function(){history(page-1);});}
                if(versions.length===50){button(content,"Versiones anteriores",function(){history(page+1);});}
                button(content,"Volver al listado",list);
            });
        }
        open.addEventListener("click",function(){
            var other=host.querySelector('section[role=dialog]:not([hidden])');
            if(other && other!==dialog){other.focus();return;}
            restoreFocus=document.activeElement;selectedLevel=level();dialog.hidden=false;dialog.focus();list();
        });
        close.addEventListener("click",function(){
            if(busy){message("Espere a que termine la solicitud.");return;}
            if(!protectChanges()){return;}
            viewEpoch++;
            clearContent();current=null;dialog.hidden=true;if(restoreFocus){restoreFocus.focus();}
        });
        dialog.addEventListener("keydown",function(event){
            if(event.key==="Escape" && !busy){close.click();}
            if(event.key==="Tab"){
                var focusable=Array.prototype.filter.call(dialog.querySelectorAll("button,input,select,summary,[tabindex='0']"),function(el){return !el.matches(":disabled") && el.getClientRects().length;});
                var first=focusable[0],last=focusable[focusable.length-1];
                if(event.shiftKey && document.activeElement===first){event.preventDefault();last.focus();}
                else if(!event.shiftKey && document.activeElement===last){event.preventDefault();first.focus();}
            }
        });
    }
    window.PerfilImportacionEditor={inicializar:init};
    $(function(){
        init();
        if(window.Sys && window.Sys.Application){window.Sys.Application.add_load(init);}
    });
}(window, window.jQuery));
