using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using Newtonsoft.Json.Linq;

public static partial class ImportacionCarpetasBehaviorTests {
    sealed class RecoveryHttpFixture:IDisposable {
        public readonly RecoveryV2Fixture Recovery;
        public readonly ResourceFixture Authority=new ResourceFixture();
        public readonly ControlRepository Repository;
        public readonly WebServiceImportacionCarpetas Endpoint;
        public int Factories,Status;public string DestinationChange;public bool ThrowFactory;public ImportRequest LastWorker;
        public RecoveryHttpFixture(bool positive=false){
            Recovery=new RecoveryV2Fixture(positive);Repository=new ControlRepository{Lot=ReceiptClone(Recovery.Sql.State.Lot)};
            var ctor=typeof(CoordinadorImportacionCarpetas).GetConstructors(BindingFlags.NonPublic|BindingFlags.Instance).Single(c=>c.GetParameters().Length==5);
            Func<ContextoImportacionCarpetas> identity=()=>Authority.Adapter.Resolver(HttpContext.Current);
            Func<string,ContextoImportacionCarpetas> destination=n=>{
                var c=ReceiptClone(Recovery.Sql.State.Lot.Contexto);
                if(DestinationChange=="deny")throw new PerfilImportacionException("ACCESO_DENEGADO","synthetic");
                if(DestinationChange=="seal")c.SelloDestino=new string('f',64);
                return c;
            };
            var coordinator=(CoordinadorImportacionCarpetas)ctor.Invoke(new object[]{identity,Repository,destination,new Action<ContextoImportacionCarpetas,ConfiguracionPerfilImportacion>((c,cfg)=>Recovery.Authorize()),Recovery.Port});
            Endpoint=OperationsEndpoint(Authority,purpose=>{Factories++;Check(purpose=="RECUPERAR_DOCUMENTO_V2","Recovery HTTP composes another phase");if(ThrowFactory)throw new InvalidOperationException("PRIVATE_HTTP_RECOVERY_PATH_AND_TOKEN");return coordinator;});
        }
        public RecuperarDocumentoV2ImportacionSolicitud Request(){return new RecuperarDocumentoV2ImportacionSolicitud{loteId=Recovery.Sql.State.Lot.LoteId,intentoId=Recovery.Sql.State.Attempt.IntentoId,revisionLoteEsperada=Recovery.Sql.State.Lot.Revision,revisionArchivoEsperada=Recovery.Sql.State.File.Revision};}
        public string Body(){return PerfilImportacionJson.Escribir(new {solicitud=Request()});}
        public RespuestaPerfilImportacion Send(string body=null,Action<ImportRequest,HttpContext> change=null,RecuperarDocumentoV2ImportacionSolicitud supplied=null){
            var previous=HttpContext.Current;
            try{
                var w=new ImportRequest{Body=System.Text.Encoding.UTF8.GetBytes(body??Body())};var h=SessionFixture(worker:w);
                w.Token=ImportacionCarpetasRequestGuard.Emitir(h,RequestContext());if(change!=null)change(w,h);HttpContext.Current=h;LastWorker=w;
                var r=Endpoint.RecuperarDocumentoV2(supplied??new RecuperarDocumentoV2ImportacionSolicitud{loteId="999",intentoId="999",revisionLoteEsperada=1,revisionArchivoEsperada=1});
                Status=h.Response.StatusCode;Check(h.Response.Cache.GetCacheability()==HttpCacheability.NoCache,"Recovery HTTP caches response");
                Check(PerfilImportacionJson.Escribir(r).IndexOf("PRIVATE_HTTP_RECOVERY",StringComparison.Ordinal)<0,"Recovery HTTP exposes exception");return r;
            }finally{HttpContext.Current=previous;}
        }
        public void NoEffects(){Check(Recovery.Guards==0&&Recovery.Writes==0&&Recovery.Sql.Flow.Writers==0,"Rejected HTTP recovery reaches guard/write");}
        public void Dispose(){Recovery.Dispose();}
    }
    static RecuperacionDocumentoImportacionResultado RecoveryData(RespuestaPerfilImportacion response){Check(response.ok&&response.datos is RecuperacionDocumentoImportacionResultado,"Missing typed recovery response");return (RecuperacionDocumentoImportacionResultado)response.datos;}
    static object RecoveryComposition(string name,params object[] args){try{return typeof(ComposicionReconciliacionImportacion).GetMethod(name,BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,args);}catch(TargetInvocationException e){throw e.InnerException;}}
    static void RecoveryHttpV2Cases(){
        int cases=0;Action<string,Action> run=(name,action)=>{action();cases++;Console.WriteLine("DOC-118 recovery HTTP V2 case: "+name+": passed");};
        run("contract_four_references_post_and_session",()=>{
            Check(typeof(RecuperarDocumentoV2ImportacionSolicitud).GetProperties().Select(p=>p.Name).OrderBy(n=>n).SequenceEqual(new[]{"intentoId","loteId","revisionArchivoEsperada","revisionLoteEsperada"}),"Recovery accepts extra proof properties");
            var m=typeof(WebServiceImportacionCarpetas).GetMethod("RecuperarDocumentoV2");var web=(WebMethodAttribute)Attribute.GetCustomAttribute(m,typeof(WebMethodAttribute));var script=(ScriptMethodAttribute)Attribute.GetCustomAttribute(m,typeof(ScriptMethodAttribute));
            Check(web.EnableSession&&!script.UseHttpGet&&script.ResponseFormat==ResponseFormat.Json,"Recovery endpoint lacks POST/session contract");
            var dispatcher=typeof(WebServiceImportacionCarpetas).GetMethod("CrearCoordinacionOperativa",BindingFlags.NonPublic|BindingFlags.Static);
            bool reached=false;try{dispatcher.Invoke(null,new object[]{null,new ResourceFixture().Adapter,"RECUPERAR_DOCUMENTO_V2"});}catch(TargetInvocationException e){var failure=e.InnerException as PerfilImportacionException;reached=failure!=null&&failure.Codigo=="SESION_REQUERIDA";}
            Check(reached,"Production dispatcher does not reach real recovery composition");
        });
        run("prepared_response_distinguishes_control_closure",()=>{using(var f=new RecoveryHttpFixture()){
            var r=f.Send();var d=RecoveryData(r);Check(f.Status==200&&r.codigo=="EJECUCION_CERRADA_SIN_INVOCACION"&&d.estado=="RECHAZADO"&&d.resuelta&&d.cierreSinInvocacion&&!d.documentoConfirmado&&d.documentoId==null&&d.inventarioId==null&&f.Recovery.Writes==4,"HTTP negative claims document success or file rejection");
        }});
        run("observed_response_returns_only_checked_ids",()=>{using(var f=new RecoveryHttpFixture(true)){
            var r=f.Send();var d=RecoveryData(r);Check(f.Status==200&&r.codigo=="DOCUMENTO_CONFIRMADO"&&d.estado=="CONFIRMADO"&&d.documentoConfirmado&&!d.cierreSinInvocacion&&d.documentoId=="90"&&d.inventarioId=="100","HTTP positive changes identity");
            var json=PerfilImportacionJson.Escribir(r);foreach(var word in new[]{"TokenReclamacion","ValoresEfectivos","IdentidadFisicaHash","NodoRef","StagingRef","SolicitudHash","detalle_doc",f.Recovery.Sql.Flow.Disk.Path})Check(!json.Contains(word),"Recovery response leaks internal data");
        }});
        run("body_is_authoritative_over_asmx_object",()=>{using(var f=new RecoveryHttpFixture()){Check(RecoveryData(f.Send()).loteId=="20"&&f.Recovery.Reads==1,"ASMX object overrides guarded body");}});
        run("csrf_method_origin_and_content_type_block_before_factory",()=>{
            foreach(var change in new Action<ImportRequest,HttpContext>[] {(w,h)=>w.Token=null,(w,h)=>w.Token="wrong",(w,h)=>w.Verb="GET",(w,h)=>w.Origin="http://other.invalid",(w,h)=>w.Mime="text/plain",(w,h)=>w.FetchSite="cross-site"})using(var f=new RecoveryHttpFixture()){
                Check(!f.Send(change:change).ok&&f.Status==403&&f.Factories==0&&f.LastWorker.BodyReads==0,"Invalid HTTP recovery reaches body/factory");f.NoEffects();
            }
        });
        run("missing_or_public_session_blocks_before_body",()=>{
            foreach(var change in new Action<ImportRequest,HttpContext>[] {(w,h)=>h.Session.Remove("GA_IDUSUARIOGESTION"),(w,h)=>h.Session["GA_LOGINUSUARIOGESTION"]="CONSULTAPUBLICO"})using(var f=new RecoveryHttpFixture()){
                Check(f.Send(change:change).codigo=="SESION_REQUERIDA"&&f.Status==401&&f.Factories==0&&f.LastWorker.BodyReads==0,"Recovery accepts missing/public identity");f.NoEffects();
            }
        });
        run("revoked_import_permission_blocks_before_factory",()=>{using(var f=new RecoveryHttpFixture()){
            f.Authority.ImportAllowed=false;Check(f.Send().codigo=="ACCESO_DENEGADO"&&f.Status==403&&f.Factories==0&&f.LastWorker.BodyReads==0,"Recovery trusts cached permission/token");f.NoEffects();
        }});
        run("unknown_duplicate_coerced_or_forged_fields_block",()=>{
            foreach(var field in new[]{"usuarioRef","ambitoRef","nivelRef","evidencia","Inicio","Original","proveedor","ruta","documentoId","inventarioId","TokenReclamacion"})using(var f=new RecoveryHttpFixture()){
                var j=JObject.Parse(f.Body());j["solicitud"][field]="forged";Check(f.Send(j.ToString()).codigo=="CONFIGURACION_INVALIDA"&&f.Factories==0,"Recovery accepts forged evidence/authority");f.NoEffects();
            }
            foreach(var kind in new[]{"duplicate","string_revision","number_id","extra_envelope"})using(var f=new RecoveryHttpFixture()){
                var j=JObject.Parse(f.Body());string body;
                if(kind=="duplicate")body=f.Body().Replace("\"loteId\":\"20\"","\"loteId\":\"20\",\"loteId\":\"20\"");
                else{if(kind=="string_revision")j["solicitud"]["revisionLoteEsperada"]="1";if(kind=="number_id")j["solicitud"]["loteId"]=20;if(kind=="extra_envelope")j["prueba"]="forged";body=j.ToString();}
                Check(f.Send(body).codigo=="CONFIGURACION_INVALIDA"&&f.Factories==0,"Recovery accepts duplicate/coerced/envelope field");f.NoEffects();
            }
        });
        run("invalid_references_and_missing_revisions_block_before_factory",()=>{
            foreach(var field in new[]{"loteId","intentoId","revisionLoteEsperada","revisionArchivoEsperada"})using(var f=new RecoveryHttpFixture()){
                var j=JObject.Parse(f.Body());((JObject)j["solicitud"]).Remove(field);Check(f.Send(j.ToString()).codigo=="SOLICITUD_INVALIDA"&&f.Factories==0,"Incomplete recovery reaches composition");f.NoEffects();
            }
            using(var f=new RecoveryHttpFixture()){var j=JObject.Parse(f.Body());j["solicitud"]["loteId"]="020";Check(f.Send(j.ToString()).codigo=="SOLICITUD_INVALIDA"&&f.Factories==0,"Recovery accepts noncanonical ID");f.NoEffects();}
        });
        run("owner_scope_and_destination_mismatch_block_before_read",()=>{
            foreach(var mode in new[]{"owner","scope","deny","seal"})using(var f=new RecoveryHttpFixture()){
                if(mode=="owner")f.Repository.Lot.Contexto.UsuarioRef="9";else if(mode=="scope")f.Repository.Lot.Contexto.AmbitoRef="9";else f.DestinationChange=mode;
                Check(!f.Send().ok&&f.Recovery.Reads==0,"Recovery reads foreign/changed destination");f.NoEffects();
            }
        });
        run("reconcile_permissions_and_cabinet_authority_checked",()=>{
            foreach(var mode in new[]{"add","upload","cabinet","moved"}){var a=new ResourceFixture();a.Owner="9";var c=a.Adapter.ResolverDestino(a.Http,"7","RECONCILIAR");var cfg=a.Configuration();
                if(mode=="add")a.AddLevel=false;if(mode=="upload")a.UploadLevel=false;if(mode=="cabinet")a.CabinetAllowed=false;if(mode=="moved")a.Moved=true;
                bool blocked=false;try{a.Adapter.AutorizarRecursos(a.Http,c,cfg,"RECONCILIAR");}catch(PerfilImportacionException){blocked=true;}Check(blocked,"Recovery RECONCILIAR authority lost");
            }
        });
        run("stale_active_revisions_block_before_guard",()=>{foreach(var field in new[]{"revisionLoteEsperada","revisionArchivoEsperada"})using(var f=new RecoveryHttpFixture()){
            var j=JObject.Parse(f.Body());j["solicitud"][field]=1;Check(f.Send(j.ToString()).codigo=="CONFLICTO_REVISION"&&f.Status==409,"Recovery accepts stale revision");f.NoEffects();
        }});
        run("v1_and_gate_only_remain_blocked",()=>{foreach(var mode in new[]{"v1","gate"})using(var f=new RecoveryHttpFixture()){
            if(mode=="gate")f.Recovery.Sql.Gate();else{var s=f.Recovery.Sql.State;s.Attempt.IntentoId="69";s.File.IntentoActivoId="69";s.Evidence=PerfilImportacionJson.Escribir(RepositoryHelper("EvidenciaInicialDocumento",f.Recovery.Sql.Command));s.Attempt.Resultado=PerfilImportacionJson.Leer<ResultadoEfectoImportacion>(s.Evidence);}
            Check(!f.Send().ok,"HTTP recovery accepts V1 or uncertain gate");f.NoEffects();
        }});
        run("active_retention_and_unsupported_provider_remain_blocked",()=>{
            using(var f=new RecoveryHttpFixture())using(var held=(IDisposable)StagingV2(f.Recovery.Sql.Flow.Disk.Store,"ConservarOriginalV2",f.Recovery.Sql.State.Lot.Contexto)){
                Check(f.Send().codigo=="OPERACION_EN_CURSO"&&f.Status==409&&f.Recovery.Writes==0,"HTTP recovery bypasses live retention");
            }
            using(var f=new RecoveryHttpFixture()){
                var j=JObject.Parse(f.Recovery.Sql.State.Evidence);j["Inicio"]["Original"]["NodoRef"]=new string('f',32);f.Recovery.Sql.State.Evidence=V2Json(V2("Leer",j.ToString()));f.Recovery.Sql.State.Journal.Rows[1]["detalle_doc"]=f.Recovery.Sql.State.Evidence;
                Check(f.Send().codigo=="CAPACIDAD_NO_DISPONIBLE"&&f.Status==503&&f.Recovery.Writes==0,"HTTP recovery qualifies another epoch");
            }
        });
        run("history_replay_reauthorizes_without_writes",()=>{using(var f=new RecoveryHttpFixture()){
            var body=f.Body();var first=RecoveryData(f.Send(body));int writes=f.Recovery.Writes,guards=f.Recovery.Guards;f.Recovery.Sql.State.File.IntentoActivoId="999";f.Recovery.Sql.State.File.EstadoEfecto="EN_CURSO";f.Recovery.Sql.State.File.Revision++;
            var again=RecoveryData(f.Send(body));Check(again.repeticion&&again.codigo==first.codigo&&f.Recovery.Writes==writes&&f.Recovery.Guards==guards&&f.Recovery.Sql.State.File.IntentoActivoId=="999","HTTP replay touches new claim");
            f.Authority.ImportAllowed=false;Check(f.Send(body).codigo=="ACCESO_DENEGADO"&&f.Recovery.Writes==writes,"HTTP history skips authority");
        }});
        run("response_loss_replay_returns_same_history",()=>{foreach(var positive in new[]{false,true})using(var f=new RecoveryHttpFixture(positive)){
            var body=f.Body();f.Recovery.LostResponse=true;Check(f.Send(body).codigo=="ERROR_INTERNO"&&f.Status==500,"HTTP response loss not sanitized");f.Recovery.LostResponse=false;
            var d=RecoveryData(f.Send(body));Check(d.repeticion&&d.documentoConfirmado==positive&&f.Recovery.Writes==4&&f.Recovery.Consolidations==1,"HTTP response loss repeats recovery");
        }});
        run("exceptions_are_sanitized_no_internal_paths",()=>{using(var f=new RecoveryHttpFixture()){
            f.ThrowFactory=true;var r=f.Send();Check(!r.ok&&r.codigo=="ERROR_INTERNO"&&f.Status==500&&r.datos==null&&!PerfilImportacionJson.Escribir(r).Contains(f.Recovery.Sql.Flow.Disk.Path),"HTTP recovery leaks exception/path");f.NoEffects();
        }});
        run("request_and_origin_pins_are_revalidated",()=>{
            var previous=HttpContext.Current;try{
                var h=SessionFixture();ConfigureSyntheticLegacyRa(h);foreach(var suffix in new[]{"TYPE_DBMS_MODULO","IP_SERVER_MODULO","DB_NAME_MODULO","USER_DBMS_MODULO","PASW_DBMS_MODULO","ACTIVA_POOL_DBMS","NUMERO_DBMS_CONEX"})h.Session["DA_"+suffix]=h.Session["RA_"+suffix];HttpContext.Current=h;
                var ra=ModuleSessionConnectionStringResolver.Resolve(h,"RA_");var da=ModuleSessionConnectionStringResolver.Resolve(h,"DA_");var guard=(Action)RecoveryComposition("GuardiaOrigenRecuperacionV2",h,ra,da);guard();
                foreach(var prefix in new[]{"RA_","DA_"}){h.Session[prefix+"DB_NAME_MODULO"]="changed-synthetic";Rejected("ACCESO_DENEGADO",guard);h.Session[prefix+"DB_NAME_MODULO"]="synthetic-db";guard();}
                HttpContext.Current=SessionFixture();Rejected("SESION_REQUERIDA",guard);Rejected("SESION_REQUERIDA",()=>RecoveryComposition("CrearRecuperacionV2",h));
                Rejected("SESION_REQUERIDA",()=>RecoveryComposition("CrearRecuperacionV2",new object[]{null}));
            }finally{HttpContext.Current=previous;}
        });
        run("uncomposed_recovery_has_no_v1_fallback",()=>{using(var f=new RecoveryV2Fixture()){
            Rejected("CAPACIDAD_NO_DISPONIBLE",()=>f.Service.RecuperarDocumento(new RecuperarDocumentoV2ImportacionSolicitud{loteId="20",intentoId="50",revisionLoteEsperada=f.Sql.State.Lot.Revision,revisionArchivoEsperada=f.Sql.State.File.Revision}));Check(f.Reads==0&&f.Writes==0,"Uncomposed recovery falls back to V1");
        }});
        Check(cases==19,"Recovery HTTP case count changed");Console.WriteLine("DOC-118 V2 recovery HTTP behavior cases: 19 passed; synthetic HTTP; real guard; zero writer; no database connection");
    }
}
