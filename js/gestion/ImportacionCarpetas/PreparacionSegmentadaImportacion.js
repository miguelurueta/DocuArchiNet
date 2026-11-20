(function (root, factory) {
    'use strict';
    if (typeof module === 'object' && module.exports) { module.exports = factory(); }
    else { root.PreparacionSegmentadaImportacion = factory(); }
}(typeof window !== 'undefined' ? window : this, function () {
    'use strict';
    function error(code) { var e = new Error(code); e.codigo = code; return e; }
    function id(value) { return typeof value === 'string' && /^[1-9][0-9]{0,9}$/.test(value) && Number(value) <= 4294967295; }
    function correlation(value) { return typeof value === 'string' && /^[A-Za-z0-9_-]{1,64}$/.test(value); }
    function integer(value) { return Number.isSafeInteger(value) && value >= 0; }
    function sumar(a, b) {
        if (!/^(0|[1-9][0-9]*)$/.test(a) || !/^(0|[1-9][0-9]*)$/.test(b)) { throw error('SOLICITUD_INVALIDA'); }
        var carry = 0, result = '', i = a.length - 1, j = b.length - 1;
        while (i >= 0 || j >= 0 || carry) { var n = (i >= 0 ? Number(a[i--]) : 0) + (j >= 0 ? Number(b[j--]) : 0) + carry; result = String(n % 10) + result; carry = Math.floor(n / 10); }
        return result;
    }
    function escapedLength(text) { return text.replace(/[\u007f-\uffff]/g, function (c) { return '\\u' + ('0000' + c.charCodeAt(0).toString(16)).slice(-4); }).length; }
    function indivisible(row) { var e=error('REVISION_TAMANO_EXCEDIDO');e.elementoMetadatos=String(row.nombre||'Elemento seleccionado').slice(0,120);e.solicitudNoEnviada=true;return e; }
    function particionar(request, maximum) {
        if (!request || !Array.isArray(request.raices) || !request.raices.length || !Array.isArray(request.archivos) || !request.archivos.length) { throw error('SOLICITUD_INVALIDA'); }
        var limit = Number.isSafeInteger(maximum) && maximum > 0 ? Math.min(maximum, 65536) : 65536, parts = [];
        ['RAICES', 'ARCHIVOS'].forEach(function (type) {
            var rows = type === 'RAICES' ? request.raices : request.archivos, values = [], number = 1;
            function body(items) { return {solicitud:{preparacionId:'4294967295',revisionEsperada:4294967294,tipo:type,numero:4294967295,raices:type === 'RAICES' ? items : [],archivos:type === 'ARCHIVOS' ? items : []}}; }
            function fits(items) { var text = JSON.stringify(body(items)); return text.length <= limit && escapedLength(text) <= 262144; }
            function save() { parts.push({tipo:type,numero:number++,raices:type === 'RAICES' ? values : [],archivos:type === 'ARCHIVOS' ? values : []}); values = []; }
            rows.forEach(function (row) {
                if (!row || typeof row !== 'object') { throw error('SOLICITUD_INVALIDA'); }
                if (values.length === 100 || !fits(values.concat([row]))) { if (!values.length) { throw indivisible(row); } save(); }
                if (!fits([row])) { throw indivisible(row); }
                values.push(row);
            });
            if (values.length) { save(); }
        });
        return parts;
    }
    function crear(options) {
        var request = options.solicitud, parts = particionar(request, options.maximoJson), state = null, stage = 'INICIO', index = 0;
        var paused = false, disposed = false, running = null, summary = null, complete = null, uncertain = null;
        var key = options.clave, totalBytes = request.archivos.reduce(function (sum, row) { return sumar(sum, row.bytes); }, '0');
        if (!/^[a-f0-9]{32}$/.test(key || '') || !id(request.nivelRef) || !id(request.versionId)) { throw error('SOLICITUD_INVALIDA'); }
        function live() { return !disposed && (!options.vigente || options.vigente()); }
        function check() { if (!live()) { throw error('SOLICITUD_CANCELADA'); } if (paused) { throw error('PREPARACION_PAUSADA'); } }
        function notify() { if (live() && options.cambiar) { options.cambiar({fase:stage,pausada:paused,preparacionId:state && state.preparacionId,partes:parts.length,partesValidadas:state && state.partesValidadas || 0,totalArchivos:request.archivos.length,archivosRegistrados:stage === 'REGISTRAR' ? parts.slice(0,index).reduce(function(n,p){return n+p.archivos.length;},0) : (state ? request.archivos.length : 0),archivosRevisados:stage === 'VALIDAR' || stage === 'SELLAR' || stage === 'REVISION' || stage === 'COMPLETA' ? parts.slice(0,index).reduce(function (n,p) { return n + p.archivos.length; },0) : 0}); } }
        function accept(value) {
            if (value && value.existe === false && !state) { return null; }
            if (!value || !id(value.preparacionId) || !Number.isInteger(value.revision) || value.revision < 1 || value.revision >= 4294967295 || ['BORRADOR','SELLADA','EXPIRADA','INVALIDADA'].indexOf(value.estado) === -1 ||
                value.totalRaices !== request.raices.length || value.totalArchivos !== request.archivos.length || !integer(value.partesRecibidas) || !integer(value.partesValidadas) || value.partesValidadas > value.partesRecibidas || value.partesRecibidas > parts.length || state && (state.preparacionId !== value.preparacionId || value.revision < state.revision)) { throw error('RESPUESTA_INVALIDA'); }
            state = value; notify(); return value;
        }
        function readState() { return options.api.invocar('ConsultarPreparacionPlan', state ? {preparacionId:state.preparacionId,tipo:'RESUMEN',pagina:1} : {clavePreparacion:key,nivelRef:request.nivelRef,tipo:'RESUMEN',pagina:1}).then(accept); }
        function mutate(operation, value) {
            check();
            return options.api.invocar(operation, value).catch(function (failure) {
                if (!live() || !failure.resultadoIncierto) { throw failure; }
                uncertain = failure; paused = true; notify();
                // Solo lectura del resultado; continuar exige otro consentimiento.
                return readState().catch(function () {}).then(function () { throw failure; });
            });
        }
        function revisionPages(type, rows, eligible, page) {
            check();
            return options.api.invocar('ConsultarRevisionPlan', {planId:summary.planId,revision:summary.revision,tipo:type,pagina:page}).then(function (value) {
                check();
                if (!value || value.planId !== summary.planId || value.revision !== summary.revision || value.planHash !== summary.planHash || value.nivelRef !== request.nivelRef || value.tipo !== type || value.pagina !== page || typeof value.puedeHaberMas !== 'boolean') { throw error('RESPUESTA_INVALIDA'); }
                var items = type === 'VALORES' ? value.valores : value.incidencias;
                if (!Array.isArray(items) || items.length > 25 || value.puedeHaberMas && !items.length || page > 10000) { throw error('RESPUESTA_INVALIDA'); }
                Array.prototype.push.apply(rows, items);
                if (eligible && Array.isArray(value.unidadesElegiblesPagina)) { Array.prototype.push.apply(eligible,value.unidadesElegiblesPagina); }
                return value.puedeHaberMas ? revisionPages(type, rows, eligible, page + 1) : rows;
            });
        }
        function readRevision() {
            check();
            var initial = summary ? Promise.resolve(summary) : options.api.invocar('ConsultarRevisionPlan',{planId:state.planId,revision:1,tipo:'RESUMEN',pagina:1});
            return initial.then(function (value) {
                if (!value || !id(value.planId) || !Number.isInteger(value.revision) || value.revision < 1 || !/^[a-f0-9]{64}$/.test(value.planHash || '') || value.nivelRef !== request.nivelRef || typeof value.puedeConfirmar !== 'boolean' || !integer(value.unidadesElegibles) || value.totalArchivos !== request.archivos.length || !integer(value.totalIncidencias)) { throw error('RESPUESTA_INVALIDA'); }
                summary = value; var elements = [], incidents = [], eligible = [];
                return revisionPages('VALORES',elements,eligible,1).then(function () { return value.totalIncidencias ? revisionPages('INCIDENCIAS',incidents,null,1) : null; }).then(function () {
                    var keys = Object.create(null), roots = Object.create(null), files = Object.create(null);
                    request.raices.forEach(function(r){roots[r.idCliente] = true;}); request.archivos.forEach(function(f){files[f.raizIdCliente + ':' + f.idCliente] = true;});
                    elements.forEach(function(e){var name = e.RaizIdCliente + ':' + (e.ArchivoIdCliente || '');if(!roots[e.RaizIdCliente] || keys[name] || (e.Ambito === 'RAIZ' ? !!e.ArchivoIdCliente : e.Ambito !== 'ARCHIVO' || !files[name])){throw error('RESPUESTA_INVALIDA');}keys[name] = true;});
                    eligible = eligible.filter(function (r,i,all) { return all.indexOf(r) === i; });
                    if(elements.length !== request.raices.length + request.archivos.length || incidents.length !== value.totalIncidencias || eligible.length !== value.unidadesElegibles || eligible.some(function(r){return !roots[r];})){throw error('RESPUESTA_INVALIDA');}
                    complete = {planId:value.planId,revision:value.revision,planHash:value.planHash,nivelRef:value.nivelRef,expiraUtc:value.expiraUtc,
                        resultado:{PuedeConfirmar:value.puedeConfirmar,UnidadesElegibles:eligible,Elementos:elements,Incidencias:incidents}};
                    stage = 'COMPLETA'; notify(); return complete;
                });
            });
        }
        function cycle() {
            notify(); check();
            if (stage === 'INICIO') {
                if (uncertain) { return readState().then(function(){uncertain = null;stage = !state ? 'INICIO' : state.estado === 'SELLADA' ? 'REVISION' : 'REGISTRAR';return cycle();}); }
                return mutate('IniciarPreparacionPlan',{nivelRef:request.nivelRef,versionId:request.versionId,clavePreparacion:key,totalRaices:request.raices.length,totalArchivos:request.archivos.length,totalBytes:totalBytes,
                    partesRaices:parts.filter(function(p){return p.tipo === 'RAICES';}).length,partesArchivos:parts.filter(function(p){return p.tipo === 'ARCHIVOS';}).length}).then(function(value){accept(value);stage = state.estado === 'SELLADA' ? 'REVISION' : 'REGISTRAR';return cycle();});
            }
            if (stage === 'REVISION') { return readRevision(); }
            if (state && state.estado === 'SELLADA') { stage = 'REVISION'; return cycle(); }
            if (stage === 'REGISTRAR' && index === parts.length) { index = 0; stage = 'VALIDAR'; return cycle(); }
            if (index === parts.length) {
                stage = 'SELLAR'; notify();
                return mutate('SellarPreparacionPlan',{preparacionId:state.preparacionId,revisionEsperada:state.revision,claveCierre:key}).then(function(value){summary = value;stage = 'REVISION';return cycle();});
            }
            var part = parts[index];
            if (stage === 'REGISTRAR') {
                return mutate('RegistrarPartePreparacion',{preparacionId:state.preparacionId,revisionEsperada:state.revision,tipo:part.tipo,numero:part.numero,raices:part.raices,archivos:part.archivos}).then(function(value){accept(value);index++;return cycle();});
            }
            return mutate('ValidarPartePreparacion',{preparacionId:state.preparacionId,revisionEsperada:state.revision,tipo:part.tipo,numero:part.numero}).then(function(value){accept(value);index++;return cycle();});
        }
        return {iniciar:function () {
            if (running) { return running; } if (complete) { return Promise.resolve(complete); }
            paused = false;
            running = Promise.resolve().then(function(){return state && stage !== 'INICIO' ? readState().then(cycle) : cycle();}).finally(function(){running = null;}); return running;
        },pausar:function () { paused = true; notify();return running || Promise.resolve(); },destruir:function () { disposed = true;paused = true; },estado:function () { return {fase:stage,preparacionId:state && state.preparacionId,pausada:paused}; }};
    }
    function huellaRecursos(snapshot, crypto) {
        if (!crypto || !crypto.subtle || typeof TextEncoder === 'undefined') { return Promise.reject(error('NAVEGADOR_NO_COMPATIBLE')); }
        var rows = [];
        ['raices','archivos'].forEach(function(type){snapshot[type].forEach(function(row){if(!id(row.recursoId)||!correlation(row.idCliente)||!Number.isInteger(row.revision)||row.revision<1||type==='archivos'&&!id(row.raizId)){throw error('RESPUESTA_INVALIDA');}rows.push((type==='raices'?'R:':'A:')+row.recursoId+':'+(type==='archivos'?row.raizId+':':'')+row.idCliente+':'+row.revision);});});
        rows.sort();var bytes = new TextEncoder().encode('DOC120_RECURSOS_1\n'+snapshot.estado.loteId+'\n'+rows.join('\n')+'\n');
        return crypto.subtle.digest('SHA-256',bytes).then(function(buffer){return Array.prototype.map.call(new Uint8Array(buffer),function(b){return ('0'+b.toString(16)).slice(-2);}).join('');});
    }
    function vincularOriginales(root, expected, selected) {
        if (!root || !id(root.recursoId) || !Array.isArray(expected)) { throw error('SOLICITUD_INVALIDA'); }
        var index = Object.create(null), seen = Object.create(null), files = Array.prototype.slice.call(selected || []), directory = null;
        var report = {vinculados:Object.create(null),faltantes:[],modificados:[],ambiguos:[],sobrantes:[],yaRecibidos:0};
        function path(text){var parts = typeof text === 'string' ? text.split('/') : [];if(parts.length<2||parts.some(function(p){return !p||p==='.'||p==='..'||/[\\:\x00-\x1f\x7f]/.test(p);})){throw error('RUTA_RELATIVA_INVALIDA');}return parts;}
        expected.filter(function(row){return row.raizId===root.recursoId;}).forEach(function(row){var parts=path(row.rutaRelativa), key=parts.slice(1).join('/').normalize('NFC');if(index[key]){throw error('RESELECCION_NO_COINCIDE');}index[key]=row;});
        files.forEach(function(file){var parts=path(file.webkitRelativePath), key=parts.slice(1).join('/').normalize('NFC');if(directory!==null&&directory!==parts[0]){throw error('SELECCION_MULTIPLES_RAICES');}directory=parts[0];
            var row=index[key];if(seen[key]){if(row){delete report.vinculados[row.recursoId];report.ambiguos.push(row.recursoId);}return;}seen[key]=true;
            if(!row){report.sobrantes.push(parts.slice(1).join('/'));return;}
            if(row.estadoRecepcion==='RECIBIDO'){report.yaRecibidos++;return;}
            if(row.estadoRecepcion!=='PENDIENTE'||row.estado!=='NO_INICIADO'||row.tieneIntentoActivo){return;}
            if(file.name!==row.nombre||!integer(file.size)||!integer(file.lastModified)||String(file.size)!==row.bytesPrevistos||String(file.lastModified)!==row.ultimaModificacionLocal){report.modificados.push(row.recursoId);return;}
            report.vinculados[row.recursoId]=file;
        });
        expected.filter(function(row){return row.raizId===root.recursoId&&row.estadoRecepcion==='PENDIENTE'&&row.estado==='NO_INICIADO'&&!row.tieneIntentoActivo;}).forEach(function(row){if(!report.vinculados[row.recursoId]){report.faltantes.push(row.recursoId);}});
        return report;
    }
    function referencias(storage,scope) {
        var name = /^[a-f0-9]{32}$/.test(scope || '') ? 'DOC120_PREP_' + scope : null;
        function leer(){if(!storage||!name){return null;}try{var raw=storage.getItem(name);if(!raw){return null;}if(raw.length>96){throw error('INVALIDA');}var value=JSON.parse(raw);if(!value||value.v!==1||!id(value.preparacionId)||Object.keys(value).some(function(k){return ['v','preparacionId'].indexOf(k)===-1;})){throw error('INVALIDA');}return value.preparacionId;}catch(_){try{storage.removeItem(name);}catch(ignore){}return null;}}
        function guardar(value){if(!storage||!name){return false;}try{if(id(value)){storage.setItem(name,JSON.stringify({v:1,preparacionId:value}));}else{storage.removeItem(name);}return true;}catch(_){return false;}}
        return {leer:leer,guardar:guardar};
    }
    return {particionar:particionar,crear:crear,huellaRecursos:huellaRecursos,vincularOriginales:vincularOriginales,sumarDecimales:sumar,referencias:referencias};
}));
