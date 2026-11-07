'use strict';
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const ROOT=path.resolve(__dirname,'../../../..'),PROTOCOL='ALM03_CANONICAL_TYPOLOGY_DESCRIPTION_V1',sha=v=>crypto.createHash('sha256').update(v).digest('hex');
const check=(v,c)=>{if(!v)throw Error('DOC117_ALM03_DESCRIPTION_'+c);};
const whitespace=/[\u0009-\u000d\u0020\u0085\u00a0\u1680\u2000-\u200a\u2028\u2029\u202f\u205f\u3000]+/gu;
function normalize(value){check(typeof value==='string'&&value.length>0&&value.length<=256&&!/[\u0000-\u0008\u000e-\u001f\u007f-\u0084\u0086-\u009f]/u.test(value)&&!/[\uD800-\uDBFF](?![\uDC00-\uDFFF])|(?<![\uD800-\uDBFF])[\uDC00-\uDFFF]/u.test(value),'INVALID');return value.normalize('NFKC').replace(whitespace,' ').replace(/^ +| +$/g,'').toUpperCase();}
function bindingFor(description){check(normalize(description)==='COMPROBANTE DE EGRESO','NOT_APPROVED');return Object.freeze({version:1,protocol:PROTOCOL,companyId:'2',expedientId:'467',typologyId:'50',description,descriptionSha256:sha(Buffer.from(description,'utf8'))});}
function validateBinding(binding){check(binding&&Object.keys(binding).sort().join(',')==='companyId,description,descriptionSha256,expedientId,protocol,typologyId,version','FIELDS_CHANGED');const expected=bindingFor(binding.description);check(JSON.stringify(expected)===JSON.stringify({version:binding.version,protocol:binding.protocol,companyId:binding.companyId,expedientId:binding.expedientId,typologyId:binding.typologyId,description:binding.description,descriptionSha256:binding.descriptionSha256}),'BINDING_CHANGED');return expected;}
async function readBinding(query){
 const rows=await query('RA','SELECT CAST(t.Id_Tipo_Doc_Series AS CHAR) AS type_id,t.Descripcion_Documento AS canonical_description FROM tipo_doc_series t INNER JOIN expediente_archivo e ON e.id_instrumento=t.id_instrumento INNER JOIN ra_registro_instrumento_archivistico i ON i.id_instrumento=t.id_instrumento INNER JOIN registro_organigrama o ON o.ID_ORGANIGRAMA=i.registro_organigrama_ID_ORGANIGRAMA WHERE e.ID_EXPEDIENTE=? AND e.ID_EMPRESA_EXPEDIENTE=? AND o.EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA=? AND t.Estado_Tipo=1 AND i.Estado_instrumento=1 AND ((t.Series_Documentales_Id_Series=e.CODIGO_SERIE_TRD AND COALESCE(t.sub_serie_id_serie,0)=COALESCE(e.CODIGO_SUB_SERIE_TRD,0)) OR t.tipo_doc_trasversal=1) AND t.Id_Tipo_Doc_Series=? LIMIT 2',['467','2','2','50']);
 check(Array.isArray(rows)&&rows.length===1&&String(rows[0].type_id)==='50','CATALOG_NOT_UNIQUE');return bindingFor(rows[0].canonical_description);
}
function transformGuard(source,binding=null){
 const literal='nombre <> "Comprobante de Egreso"',signature='    Friend Shared Function PrepararIndividual(';check(source.split(literal).length===2&&source.split(signature).length===2&&!source.includes('CanonicalDescriptionSha256'),'GUARD_CHANGED');
 const digest=binding?validateBinding(binding).descriptionSha256:'NO_CANONICAL_BINDING';
 const helper=`    Private Const CanonicalDescriptionSha256 As String = "${digest}"
    Private Shared Function MatchesCanonicalDescription(nombre As String) As Boolean
        If CanonicalDescriptionSha256 = "NO_CANONICAL_BINDING" OrElse nombre Is Nothing Then Return False
        Using digest = System.Security.Cryptography.SHA256.Create()
            Dim actual = BitConverter.ToString(digest.ComputeHash(System.Text.Encoding.UTF8.GetBytes(nombre))).Replace("-", "").ToLowerInvariant()
            Return String.Equals(actual, CanonicalDescriptionSha256, StringComparison.Ordinal)
        End Using
    End Function
`;
 return source.replace(literal,'Not MatchesCanonicalDescription(nombre)').replace(signature,helper+signature);
}
function generateDormant(directory){
 const dormant=require('./doc117-alm03-candidate.cjs'),g=dormant.generate(directory),storage=path.join(g.dir,'ClassAlmacenamiento.Laboratory.vb'),guard=path.join(g.dir,'Doc117Alm03ClosedGuard.Canonical.vb');let source=fs.readFileSync(storage,'utf8');
 const start='            Result = AlmacenamientoDocumentoProduccionDocumental(NombreGabinete,',end='                                                                 EstructuraDatosImagen)',begin=source.indexOf(start),finish=source.indexOf(end,begin)+end.length;check(begin>=0&&source.indexOf(start,begin+1)<0&&finish>begin,'CALLER_CHANGED');
 source=source.slice(0,begin)+'            Using doc117Caller = Doc117Alm03ClosedGuard.PrepararIndividual(NombreGabinete, RutaArchivoAlmacenar, DescripcionTipo, IdExpediente, IdTipoLogia, TipoAlmacenamiento, NombreClaseFormatoDocumento)\r\n'+source.slice(begin,finish)+'\r\n            End Using'+source.slice(finish);fs.writeFileSync(storage,source);fs.writeFileSync(guard,transformGuard(fs.readFileSync(path.join(ROOT,'tools/e2e/fixtures/Doc117Alm03ClosedGuard.vb'),'utf8')),{flag:'wx'});
 const targets=fs.readFileSync(g.targetsPath,'utf8');check(targets.split('</ItemGroup>').length===2,'TARGETS_CHANGED');fs.writeFileSync(g.targetsPath,targets.replace('</ItemGroup>','<Compile Include="'+guard.replaceAll('&','&amp;').replaceAll('"','&quot;')+'" /></ItemGroup>'));
 return{...g,overlaySha256:sha(fs.readFileSync(storage)),canonicalGuardSha256:sha(fs.readFileSync(guard)),descriptionProtocol:PROTOCOL,canonicalBindingPresent:false,writerAllowed:false,importsAllowed:false};
}
module.exports={ROOT,PROTOCOL,sha,normalize,bindingFor,validateBinding,readBinding,transformGuard,generateDormant};
