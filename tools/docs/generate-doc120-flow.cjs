/* Documentary source inventory. No HTTP, database, application or Git mutations. */
'use strict';
const fs = require('node:fs');
const path = require('node:path');
const crypto = require('node:crypto');
const root = path.resolve(__dirname, '../..');
const base = 'Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/';
const stem = 'flujo-tecnico-importacion-carpetas-doc120';
const main = base + stem + '.txt';
const annex = base + stem + '-anexo.txt';
const manifest = base + stem + '-fuentes.json';
const check = process.argv.includes('--check');
if (process.argv.slice(2).some(a => a !== '--check')) throw Error('Uso: node tools/docs/generate-doc120-flow.cjs [--check]');
const read = file => fs.readFileSync(path.join(root, file), 'utf8').replace(/^\uFEFF/, '').replace(/\r\n?/g, '\n');
const hash = value => crypto.createHash('sha256').update(value, 'utf8').digest('hex');
const dirs = [
    'js/gestion/ImportacionCarpetas',
    'Services/ProduccionDocumental/ImportacionCarpetas',
    'Infrastructure/ProduccionDocumental/ImportacionCarpetas',
    'Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas',
    'DTOs/ProduccionDocumental/ImportacionCarpetas',
    'Modelo/ProduccionDocumental/ImportacionCarpetas'
];
function walk(dir) {
    return fs.readdirSync(path.join(root, dir), { withFileTypes: true }).flatMap(entry =>
        entry.isDirectory() ? walk(dir + '/' + entry.name) : /\.(vb|js)$/.test(entry.name) ? [dir + '/' + entry.name] : []);
}
let files = dirs.flatMap(walk);
files.push(...fs.readdirSync(path.join(root, 'webservice')).filter(f => /^WebServiceImportacionCarpetas.*\.vb$/.test(f)).map(f => 'webservice/' + f));
files.push('generic_control/ImportacionCarpetasUpload.ashx.vb');
files.push('Gestion/WebFormProducionDocumental.aspx', 'generic_control/ImportacionCarpetasUpload.ashx', 'webservice/WebServiceImportacionCarpetas.asmx');
// Shared files: inventory only the real integration methods, not unrelated modules.
const selections = {
    'Gestion/WebFormProducionDocumental.aspx.vb': ['RenderImportacionCarpetas', 'ImportacionCarpetas_PreRender', 'RenderPerfilImportacion', 'PerfilImportacion_PreRender'],
    'js/gestion/WebFormProducionDocumental.js': ['Doc119NivelPulsado', 'Doc119InvocarImportacion', 'Doc111ActualizarRama', 'create_trenode_treview'],
    'Gestion/ClassGaExpediente.vb': ['Registrar_Expediente_Conservacion', 'SolicitaDatosEstructuraExpediente'],
    'workflow/ClassAlmacenamiento.vb': ['AlmacenamientoDocumentoProduccionDocumental', 'Almacenamiento']
};
files.push(...Object.keys(selections));
files = [...new Set(files)].sort();

function vbCode(line) {
    let quoted = false;
    for (let i = 0; i < line.length; i++) {
        if (line[i] === '"') {
            if (quoted && line[i + 1] === '"') { i++; continue; }
            quoted = !quoted;
        } else if (line[i] === "'" && !quoted) return line.slice(0, i).trimEnd();
    }
    return line;
}
const signature = /^(?:(?:Public|Private|Protected|Friend|Shared|Overloads|Overrides|Overridable|NotOverridable|MustOverride|Default|ReadOnly|WriteOnly|Partial|Async|Iterator|Protected Friend)\s+)*(?:Function|Sub)\s+([\w\u0080-\uffff]+)\s*(?:\(|$)/i;
function ranges(file, lines) {
    if (!selections[file]) return [[0, lines.length]];
    const result = [];
    for (const name of selections[file]) {
        const start = lines.findIndex(line => file.endsWith('.js') ? new RegExp('^function ' + name + '\\(').test(line) : signature.exec(line.trim())?.[1] === name);
        if (start < 0) throw Error('Metodo ausente: ' + file + ':' + name);
        let end;
        if (file.endsWith('.js')) {
            end = start + 1;
            while (end < lines.length && !/^function\s/.test(lines[end])) end++;
        } else {
            const indent = lines[start].match(/^\s*/)[0];
            end = start + 1;
            while (end < lines.length && !new RegExp('^' + indent + 'End (Function|Sub)\\s*$', 'i').test(lines[end])) end++;
            if (end === lines.length) throw Error('Fin de metodo ausente: ' + file + ':' + name);
            end++;
        }
        result.push([start, end]);
    }
    return result.sort((a, b) => a[0] - b[0]);
}
// Lexical inventory, deliberately not a call-graph/reachability proof.
// Context lines retain multiline predicates, SQL parameters and local bindings.
const decision = /\b(?:If|ElseIf|Else|Select Case|Case|Catch|Finally|Throw|Return|For Each|For|While|Do|Loop|Exit|Continue|Using|Try|When|AndAlso|OrElse)\b|\.Where\(|\.Any\(|\.All\(|\.Single(?:OrDefault)?\(|AddressOf|\b(?:SELECT|UPDATE|INSERT INTO|DELETE FROM|WHERE|ORDER BY|LIMIT|FOR UPDATE)\b/i;
const declaration = /^(?:Public|Private|Friend|Protected|Shared|Partial|NotInheritable|MustInherit|ReadOnly|WriteOnly|Overloads|Overrides|Overridable|Default|Async|Iterator|Function|Sub|Class|Interface|Enum|Inherits|Implements)\b/i;
const data = [];
const out = [
    'DOC-120 | ANEXO DE CONTRATOS, PREDICADOS Y FILTROS DEL FLUJO TECNICO',
    '================================================================',
    'Generado: node tools/docs/generate-doc120-flow.cjs',
    'Validar:  node tools/docs/generate-doc120-flow.cjs --check',
    '',
    'Leer primero ' + stem + '.txt (cajas/flechas/decisiones por fase).',
    'Este anexo es un indice LITERAL de fuentes, no pseudocodigo propuesto.',
    'Lnnnn indica linea original; los intervalos omitidos no satisfacen el',
    'selector lexico de contratos/control/filtros, no son una rama inventada.',
    'Incluye declaracion y contexto de cada sentencia de control VB; JS completo',
    'del modulo para conservar callbacks/promesas/decisiones en una sola linea.',
    'Las condiciones continuadas se conservan hasta la siguiente sentencia',
    'del mismo o menor sangrado. Los comentarios no son comportamiento activo.',
    'Se omiten espacios de formato al final de linea en la transcripcion;',
    'las huellas conservan esos espacios en la fuente original normalizada LF.',
    'No es un analizador de alcanzabilidad. Codigo administrativo, V1, limpieza',
    'y ayudas internas se inventarian sin afirmar que la UI los invoque.',
    'La composicion y allowlist reales determinan la ruta activa del mapa.',
    'Las firmas de DTO incluyen herencia: sumar propiedades de la clase base.',
    'HTTP se traduce por EstadoHttp del ASMX, salvo capturas locales documentadas.',
    'Catalogos de diagnostico conservan motivos; un motivo no equivale al codigo',
    'superior del sobre. Rechazos anteriores a ASMX/IIS no tienen sobre garantizado.',
    '',
    'FRONTERA LEGACY: se incluyen las dos entradas escritoras y metodos directos',
    'indicados en el manifiesto. Sus llamadas a otros modulos siguen siendo',
    'dependencias existentes; no se afirma enumerar internamente toda la aplicacion.',
    'La huella de un archivo compartido cubre su archivo entero, aun cuando solo',
    'se muestren metodos seleccionados. No contiene datos de una sesion real.',
    '',
    'INDICE DE FUENTES (SHA256 del texto UTF-8 normalizado LF, sin BOM):'
];
for (const file of files) {
    const source = read(file), lines = source.split('\n'), selected = ranges(file, lines);
    data.push({ path: file, sha256Lf: hash(source), lines: lines.length, selectedMethods: selections[file] || null });
    out.push(file + ' | ' + hash(source));
}
out.push('', 'CONTRATOS HTTP Y DESPACHO REAL (NO CONFUNDIR ADMINISTRACION CON OPERACION)', '======================================================================');
const requestTypes = new Set();
let endpointCount = 0;
for (const file of files.filter(f => f.startsWith('webservice/'))) {
    const lines = read(file).split('\n');
    for (let i = 0; i < lines.length; i++) {
        if (!lines[i].includes('<WebMethod(')) continue;
        const start = i;
        while (++i < lines.length && !signature.test(lines[i].trim())) { /* attributes */ }
        if (i === lines.length) throw Error('WebMethod sin firma: ' + file);
        const declarationLine = i;
        while (++i < lines.length && !/^\s*End Function\s*$/i.test(lines[i])) { /* endpoint body */ }
        const body = lines.slice(start, i + 1).join('\n');
        const dto = /\bsolicitud As (\w+)/.exec(body)?.[1];
        if (dto) requestTypes.add(dto);
        const operative = /EjecutarEntradaOperativa|EjecutarOperativo/.test(body);
        out.push('', '+ HTTP-' + String(++endpointCount).padStart(2, '0') + ' | ' + (operative ? 'OPERATIVA' : 'ADMINISTRACION') + ' | ' + file + ':' + (declarationLine + 1));
        for (let j = start; j <= i; j++) out.push('L' + String(j + 1).padStart(5, '0') + ' | ' + lines[j]);
    }
}
out.push('', 'DTO DE ENTRADA (PROPIEDADES LITERALES; INCLUIR TAMBIEN SU BASE)', '============================================================');
for (const file of files.filter(f => f.startsWith('DTOs/'))) {
    const lines = read(file).split('\n');
    for (let i = 0; i < lines.length; i++) {
        const cls = /^Public Class (\w+)/.exec(lines[i]);
        if (!cls || !requestTypes.has(cls[1])) continue;
        out.push('', '+ ' + cls[1] + ' | ' + file + ':' + (i + 1));
        do { out.push('L' + String(i + 1).padStart(5, '0') + ' | ' + lines[i]); } while (++i < lines.length && !/^End Class/.test(lines[i]));
        if (i < lines.length) out.push('L' + String(i + 1).padStart(5, '0') + ' | ' + lines[i]);
    }
}
out.push('', 'DETALLE POR FUENTE Y ORDEN ORIGINAL', '==================================');
let extracted = 0;
for (const file of files) {
    const lines = read(file).split('\n'), selected = ranges(file, lines);
    const literal = !file.endsWith('.vb');
    const keep = new Set();
    for (const [start, end] of selected) {
        for (let i = start; i < end; i++) {
            const code = literal ? lines[i].trim() : vbCode(lines[i]).trim();
            if (!code || code.startsWith('//')) continue;
            if (literal || declaration.test(code) || decision.test(code) || /^Dim\b/i.test(code) || /^<.*WebMethod/.test(code)) {
                keep.add(i);
                if (!literal) {
                    const indent = lines[i].match(/^\s*/)[0].length;
                    // Retain multiline signatures, lambdas, constructors and predicates.
                    for (let j = i + 1; j < end; j++) {
                        const next = vbCode(lines[j]);
                        if (!next.trim()) continue;
                        const more = lines[j].match(/^\s*/)[0].length > indent;
                        if (!more && !/\b(?:AndAlso|OrElse)\s*$|_\s*$|[,(&]\s*$/.test(vbCode(lines[j - 1]))) break;
                        keep.add(j);
                    }
                }
            }
        }
    }
    out.push('', '+ FUENTE: ' + file, '| Alcance: ' + (selections[file]?.join(', ') || 'archivo de modulo completo'), '+----------------------------------------');
    let previous = -2;
    for (const i of [...keep].sort((a, b) => a - b)) {
        if (i > previous + 1) out.push('    ...');
        out.push('L' + String(i + 1).padStart(5, '0') + ' | ' + lines[i]);
        previous = i; extracted++;
    }
}
out.push('', 'FIN DEL INVENTARIO. Archivos: ' + files.length + '; lineas fuente reproducidas: ' + extracted + '.', '');
const appendix = out.map(line => line.trimEnd()).join('\n');
const mainText = read(main);
const info = JSON.stringify({
    schema: 1,
    implementationCut: 'DOC120-controles-cierre-20261119',
    implementationBaseCommit: '557f8485447744858ded632b6f5f36c92f160053',
    purpose: 'Flujo resumido implementado localmente por archivo; fuentes del mismo cambio, sin nueva ejecucion real.',
    diagram: { path: main, sha256Lf: hash(mainText) },
    annex: { path: annex, sha256Lf: hash(appendix) },
    generator: { path: 'tools/docs/generate-doc120-flow.cjs', sha256Lf: hash(read('tools/docs/generate-doc120-flow.cjs')) },
    sourceCount: data.length,
    httpEndpoints: endpointCount,
    extractedLines: extracted,
    sourceInventory: data
}, null, 2) + '\n';
for (const [file, expected] of [[annex, appendix], [manifest, info]]) {
    if (check) {
        if (!fs.existsSync(path.join(root, file)) || read(file) !== expected) throw Error('Desactualizado: ' + file + '. Revisar mapa y regenerar en el mismo cambio.');
    } else fs.writeFileSync(path.join(root, file), expected, 'utf8');
}
console.log((check ? 'FLOW_CURRENT' : 'FLOW_GENERATED') + ': ' + files.length + ' fuentes; ' + extracted + ' lineas. Sin HTTP/SQL/efectos de negocio.');
