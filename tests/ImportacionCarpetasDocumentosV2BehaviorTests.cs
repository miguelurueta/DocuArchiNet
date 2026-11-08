using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;

// Parcial del harness existente. PDFs/metadatos sintéticos y dobles existentes;
// no conexión, HTTP, runtime de la aplicación ni intento real69.
public static partial class ImportacionCarpetasBehaviorTests {
    static readonly Type V2Codec=typeof(PerfilImportacionJson).Assembly.GetType("DocumentosAlmacenamientoV2Importacion",true);
    static object V2(string method,params object[] args){
        try{return V2Codec.GetMethod(method,BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,args);}
        catch(TargetInvocationException e){throw e.InnerException;}
    }
    static string V2Json(object doc){return (string)V2("Escribir",doc);}
    static object V2Original(DocumentAdapterFixture f){
        var original=Activator.CreateInstance(typeof(PerfilImportacionJson).Assembly.GetType("OriginalAlmacenamientoV2Importacion",true),true);
        Action<string,string> set=(name,value)=>original.GetType().GetProperty(name).SetValue(original,value,null);
        set("ProveedorRef",new string('1',32));set("NodoRef",new string('2',32));set("IdentidadFisicaHash",new string('3',64));
        set("ContenidoHash",f.FileRecord.ContenidoHash);set("Bytes",f.FileRecord.BytesRecibidos);return original;
    }
    static void V2Rejected(Action action){Rejected("EVIDENCIA_DOCUMENTAL_V2_INVALIDA",action);}

    static void DocumentProtocolV2Cases(){
        int cases=0;
        Action<string,Action> run=(name,action)=>{action();cases++;Console.WriteLine("DOC-118 V2 case: "+name+": passed");};
        using(var f=new DocumentAdapterFixture()){
            f.Root.Revision=Math.Max(f.Root.Revision,1U);
            var command=f.Prepare();var sourceBefore=PerfilImportacionJson.Escribir(new {f.Lot,f.Root,f.FileRecord,f.Attempt});
            var original=V2Original(f);var prepared=V2("Preparar",command,original,f.Lot.Revision);
            var preparedJson=V2Json(prepared);
            var authorized=V2("ConAutorizacion",prepared,new string('4',32),f.Lot.Revision+1U);
            var authorizedJson=V2Json(authorized);
            var result=(ResultadoEfectoImportacion)RepositoryHelper("EvidenciaInicialDocumento",command);
            result.Estado="CONFIRMADO";result.DocumentoId="90";result.InventarioId="100";
            result.Codigo="PRIVATE_CODE_SENTINEL";result.Mensaje="PRIVATE_MESSAGE_SENTINEL";
            var observed=V2("ConRetorno",authorized,result);var observedJson=V2Json(observed);
            Action<string,Action<JObject>> invalid=(text,mutate)=>{var json=JObject.Parse(text);mutate(json);V2Rejected(()=>V2("Leer",json.ToString()));};

            run("canonical_roundtrip_all_phases",()=>{
                foreach(var text in new[]{preparedJson,authorizedJson,observedJson})Check(V2Json(V2("Leer",text))==text,"V2 roundtrip changed bytes");
                var reversed=new JObject(JObject.Parse(observedJson).Properties().Reverse().Select(p=>new JProperty(p.Name,p.Value.DeepClone())));
                Check(V2Json(V2("Leer",reversed.ToString()))==observedJson,"V2 property order changes canonical bytes");
            });
            run("v1_not_upgraded_including_synthetic_69",()=>{
                var legacy=(ResultadoEfectoImportacion)RepositoryHelper("EvidenciaInicialDocumento",command);
                var text=PerfilImportacionJson.Escribir(legacy);V2Rejected(()=>V2("Leer",text));
                var attempt=ReceiptClone(f.Attempt);attempt.IntentoId="69";attempt.Estado="EN_CURSO";attempt.Resultado=legacy;
                V2Rejected(()=>V2("Leer",PerfilImportacionJson.Escribir(attempt)));
                Check(PerfilImportacionJson.Escribir(PerfilImportacionJson.Leer<ResultadoEfectoImportacion>(text))==text,"V1 changed");
            });
            run("missing_or_wrong_versions",()=>{
                invalid(preparedJson,j=>j.Remove("Version"));
                foreach(var version in new[]{0,1,3})invalid(preparedJson,j=>j["Version"]=version);
                invalid(preparedJson,j=>j["Version"]="2");invalid(preparedJson,j=>j["Version"]=2.5);
                invalid(preparedJson,j=>((JObject)j["Inicio"]).Remove("Version"));
                invalid(authorizedJson,j=>((JObject)j["Autorizacion"]).Remove("Version"));
                invalid(observedJson,j=>((JObject)j["Retorno"]).Remove("Version"));
            });
            run("origin_is_explicit_not_defaulted",()=>{
                foreach(var name in new[]{"Origen","Inicio.Origen"})invalid(preparedJson,j=>{var p=j.SelectToken(name);((JProperty)p.Parent).Remove();});
                foreach(var origin in new[]{"CLIENTE","servidor","",null})invalid(preparedJson,j=>j["Origen"]=origin);
                invalid(authorizedJson,j=>j["Autorizacion"]["Origen"]="CLIENTE");
                invalid(observedJson,j=>j["Retorno"]["Origen"]="CLIENTE");
            });
            run("all_initial_fields_required",()=>{
                foreach(var name in ((JObject)JObject.Parse(preparedJson)["Inicio"]).Properties().Select(p=>p.Name).ToArray())invalid(preparedJson,j=>((JObject)j["Inicio"]).Remove(name));
                foreach(var name in ((JObject)JObject.Parse(preparedJson)["Inicio"]["Original"]).Properties().Select(p=>p.Name).ToArray())invalid(preparedJson,j=>((JObject)j["Inicio"]["Original"]).Remove(name));
            });
            run("extra_client_claims_rejected",()=>{
                foreach(var name in new[]{"Pid","AusenciaFisica","CapacidadAcreditada","TokenReclamacion","$type"})invalid(preparedJson,j=>j[name]="PRIVATE_SENTINEL");
                invalid(preparedJson,j=>j["Inicio"]["Original"]["Ruta"]="PRIVATE_SENTINEL");
                invalid(preparedJson,j=>j["Inicio"]["cliente"]=true);
            });
            run("duplicate_wrong_case_trailing_and_oversize_json",()=>{
                V2Rejected(()=>V2("Leer",preparedJson.Replace("\"Version\":2","\"Version\":2,\"Version\":2")));
                V2Rejected(()=>V2("Leer",preparedJson.Replace("\"Version\"","\"version\"")));
                V2Rejected(()=>V2("Leer",preparedJson+preparedJson));V2Rejected(()=>V2("Leer",new string(' ',PerfilImportacionJson.MaximoCaracteres+1)));
                V2Rejected(()=>V2("Leer",(object)null));V2Rejected(()=>V2("Leer","null"));V2Rejected(()=>V2("Leer","{"));
            });
            run("canonical_identifiers_and_revisions",()=>{
                foreach(var id in new[]{"0","01","-1","9223372036854775808","PRIVATE_PATH"})invalid(preparedJson,j=>j["Inicio"]["IntentoId"]=id);
                foreach(var revision in new[]{0U,uint.MaxValue-1,uint.MaxValue})invalid(preparedJson,j=>j["Inicio"]["RevisionArchivo"]=revision);
                invalid(preparedJson,j=>j["Inicio"]["Original"]["Bytes"]="01");
                invalid(preparedJson,j=>j["Inicio"]["Original"]["Bytes"]="18446744073709551616");
            });
            run("hashes_and_provider_references_closed",()=>{
                foreach(var hash in new[]{new string('A',64),new string('a',63),new string('g',64),"PRIVATE_PATH"})invalid(preparedJson,j=>j["Inicio"]["ContextoHash"]=hash);
                invalid(preparedJson,j=>j["Inicio"]["Original"]["NodoRef"]="machine/path");
                invalid(preparedJson,j=>j["Inicio"]["OrigenGabinete"]="OTHER");
            });
            run("prepared_cannot_contain_later_evidence",()=>{
                invalid(authorizedJson,j=>j["Fase"]="PREPARADO");
                invalid(observedJson,j=>j["Fase"]="PREPARADO");
                invalid(preparedJson,j=>j["Fase"]="AUTORIZADO");
                invalid(preparedJson,j=>j["Fase"]="RETORNO_OBSERVADO");
            });
            run("authorization_binds_initial_document",()=>{
                foreach(var name in new[]{"IntentoId","LoteId","RaizId","ArchivoId","UsuarioRef","AmbitoRef","NivelRef","ExpedienteId","RelacionNivelId","GabineteId"})invalid(authorizedJson,j=>j["Inicio"][name]="999");
                foreach(var name in new[]{"SolicitudHash","ContextoHash","SnapshotHash","RecepcionHash","ValoresHash"})invalid(authorizedJson,j=>j["Inicio"][name]=new string('f',64));
                invalid(authorizedJson,j=>j["Inicio"]["Original"]["IdentidadFisicaHash"]=new string('f',64));
                invalid(authorizedJson,j=>j["Autorizacion"]["IntentoId"]="999");
                invalid(authorizedJson,j=>j["Autorizacion"]["InicioHash"]=new string('f',64));
                invalid(authorizedJson,j=>j["Autorizacion"]["RevisionLote"]=f.Lot.Revision);
                invalid(authorizedJson,j=>j["Autorizacion"]["AutorizacionRef"]="incomplete");
            });
            run("return_requires_authorization_and_exact_binding",()=>{
                invalid(observedJson,j=>j["Autorizacion"]=null);
                invalid(observedJson,j=>j["Retorno"]["InicioHash"]=new string('f',64));
                invalid(observedJson,j=>j["Retorno"]["AutorizacionHash"]=new string('f',64));
                invalid(observedJson,j=>j["Retorno"]["AutorizacionRef"]=new string('f',32));
                invalid(observedJson,j=>j["Retorno"]["IntentoId"]="999");
                invalid(observedJson,j=>j["Fase"]="AUTORIZADO");
            });
            run("confirmed_return_requires_both_observed_ids",()=>{
                invalid(observedJson,j=>j["Retorno"]["DocumentoId"]=null);
                invalid(observedJson,j=>j["Retorno"]["InventarioId"]=null);
                invalid(observedJson,j=>j["Retorno"]["DocumentoId"]="01");
                invalid(observedJson,j=>j["Retorno"]["Estado"]="RECHAZADO");
            });
            run("uncertain_return_preserves_partial_or_missing_ids",()=>{
                foreach(var document in new[]{null,"90"}){
                    var uncertain=ReceiptClone(result);uncertain.Estado="INCIERTO";uncertain.DocumentoId=document;uncertain.InventarioId=null;
                    var text=V2Json(V2("ConRetorno",authorized,uncertain));var json=JObject.Parse(text);
                    Check((string)json["Retorno"]["Estado"]=="INCIERTO"&&(string)json["Retorno"]["DocumentoId"]==document&&json["Retorno"]["InventarioId"].Type==JTokenType.Null,"V2 invents unknown IDs");
                }
            });
            run("pure_transitions_cannot_authorize_twice_or_skip_gate",()=>{
                V2Rejected(()=>V2("ConAutorizacion",authorized,new string('5',32),f.Lot.Revision+2U));
                V2Rejected(()=>V2("ConRetorno",prepared,result));
                V2Rejected(()=>V2("ConRetorno",observed,result));
                Check(V2Json(prepared)==preparedJson&&V2Json(authorized)==authorizedJson,"V2 mutates prior phase");
            });
            run("factory_checks_original_and_result_scope",()=>{
                var wrong=V2Original(f);wrong.GetType().GetProperty("ContenidoHash").SetValue(wrong,new string('f',64),null);
                V2Rejected(()=>V2("Preparar",command,wrong,f.Lot.Revision));
                var other=ReceiptClone(result);other.Expediente.ExpedienteId="999";V2Rejected(()=>V2("ConRetorno",authorized,other));
                other=ReceiptClone(result);other.ValoresEfectivos[0].Valor="999";V2Rejected(()=>V2("ConRetorno",authorized,other));
                other=ReceiptClone(result);other.Recepcion=new RecepcionVerificadaImportacion();V2Rejected(()=>V2("ConRetorno",authorized,other));
                other=ReceiptClone(result);other.ValoresEfectivos=null;V2Rejected(()=>V2("ConRetorno",authorized,other));
            });
            run("returned_objects_do_not_alias_input",()=>{
                original.GetType().GetProperty("IdentidadFisicaHash").SetValue(original,new string('f',64),null);
                result.DocumentoId="999";
                Check(V2Json(prepared)==preparedJson&&V2Json(observed)==observedJson,"V2 aliases mutable original/result");
                var read=V2("Leer",authorizedJson);read.GetType().GetProperty("Fase").SetValue(read,"PREPARADO",null);
                V2Rejected(()=>V2("Escribir",read));Check(V2Json(authorized)==authorizedJson,"Read aliases other instance");
            });
            run("raw_secrets_paths_and_messages_not_serialized",()=>{
                Check(!observedJson.Contains("PRIVATE_")&&!observedJson.Contains(command.Intento.TokenReclamacion)&&!observedJson.Contains(f.FileRecord.StagingRef)&&!observedJson.Contains(f.Stage.Receipt.Root)&&!observedJson.Contains("RutaCanonica")&&!observedJson.Contains("SelloDestino"),"V2 stores raw private fields");
            });
            run("json_is_data_not_client_authority_or_v1_result",()=>{
                // A client can copy a well-shaped document; validation alone must
                // remain data-only. Production paths still read V1 result types.
                Check(V2("Leer",preparedJson)!=null&&!V2Codec.IsPublic,"V2 contract exposed");
                Rejected("CONFIGURACION_INVALIDA",()=>PerfilImportacionJson.Leer<ResultadoEfectoImportacion>(observedJson));
                Check(typeof(WebServiceImportacionCarpetas).GetMethods().All(m=>m.ReturnType.Assembly!=V2Codec.Assembly||!m.ReturnType.Name.Contains("V2")),"V2 HTTP operation exposed");
                Check(f.Starts==0&&f.Writers==0&&f.Writes==0&&f.Events==0&&f.Checkpoints==0&&f.Authorizations==0,"V2 JSON has effects");
                Check(PerfilImportacionJson.Escribir(new {f.Lot,f.Root,f.FileRecord,f.Attempt})==sourceBefore,"V2 changes V1 state");
            });
        }
        Check(cases==19,"V2 case accounting changed");
        Console.WriteLine("DOC-118 V2 internal document behavior cases: "+cases+" passed; no execution capability");
    }
}
