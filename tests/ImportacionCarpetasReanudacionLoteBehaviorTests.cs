using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

public static partial class ImportacionCarpetasBehaviorTests {
    static ConsolidacionImportacion ResumeEvaluation(VistaConsolidacionImportacion view) {
        try { return (ConsolidacionImportacion)typeof(CoordinadorImportacionCarpetas).GetMethod("EvaluarReanudacionLote",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,new object[]{view}); }
        catch(TargetInvocationException e) { throw e.InnerException; }
    }
    static void MustRefuseResume(Action work) {
        bool refused=false;try { work(); } catch(PerfilImportacionException) { refused=true; }
        Check(refused,"Stopped lot with contradictory/activity evidence resumed");
    }
    // Transaction model runs the production evaluator/SQL producer. It is NOT a MySQL rollback test.
    sealed class ResumeLotFixture : IControlLoteImportacionRepository,IReanudacionLoteImportacionRepository,IDisposable {
        public ConsolidationFixture Source=new ConsolidationFixture(); readonly object gate=new object();
        public bool FailEvent,FailUpdate;public int Events,Controls;
        public LoteImportacionCarpetas Lot {get {return Source.Phase.Source.Lot;}}
        public ResumeLotFixture(){Source.AddFile();Source.Finish();Lot.Estado="DETENIDO";Lot.DetencionSolicitada=true;}
        public ContextoImportacionCarpetas ObtenerContextoLote(ContextoImportacionCarpetas c,string id){if(id!=Lot.LoteId||c.UsuarioRef!=Lot.Contexto.UsuarioRef||c.AmbitoRef!=Lot.Contexto.AmbitoRef)throw new PerfilImportacionException("ACCESO_DENEGADO","fixture");return ReceiptClone(Lot.Contexto);}
        public LoteImportacionCarpetas ObtenerLote(ContextoImportacionCarpetas c,string id){ObtenerContextoLote(c,id);return ReceiptClone(Lot);}
        public IntentoImportacionCarpetas ReservarIntento(ContextoImportacionCarpetas c,ReclamacionImportacionCarpetas s){throw new Exception("Resume attempted a reservation");}
        public LoteImportacionCarpetas SolicitarDetencion(ContextoImportacionCarpetas c,string id,uint rev){throw new Exception("Unexpected stop");}
        public LoteImportacionCarpetas ReanudarLote(ContextoImportacionCarpetas c,string id,uint rev){lock(gate){
            ObtenerContextoLote(c,id);var pending=new List<Action>();int events=0;
            RepositoryHelper("PersistirReanudacionLote",Source.View(),rev,DateTime.UtcNow,
                new Func<string,object[],int>((sql,args)=>{
                    Check(!sql.Contains("archivo SET")&&!sql.Contains("intento SET")&&!sql.Contains("identidad_expediente_doc=")&&!sql.Contains("DELETE "),"Resume modified business evidence");
                    Func<string,object> arg=name=>args[Array.IndexOf(args,name)+1];
                    if(FailUpdate)return 0;
                    if(sql.Contains("ra_integracion_lote SET")){
                        Check(sql.Contains("estado='DETENIDO'")&&sql.Contains("detencion_solicitada=1")&&sql.Contains("revision=@revision"),"Resume lacks stop/revision CAS");
                        pending.Add(()=>{Lot.Estado=(string)arg("@estado");Lot.DetencionSolicitada=false;Lot.Revision++;});
                    }else {
                        var root=Source.Roots.Single(r=>r.RaizId==Convert.ToString(arg("@raiz")));
                        pending.Add(()=>{root.Estado=(string)arg("@estado");root.Revision++;});
                    }return 1;
                }),new Action<ConsolidacionImportacion>(result=>{if(FailEvent)throw new Exception("synthetic event failure");events++;}));
            foreach(var commit in pending)commit();Events+=events;Controls++;return ReceiptClone(Lot);
        }}
        public void Dispose(){Source.Dispose();}
    }
    static void ReanudacionLoteCases(){
        using(var f=new ResumeLotFixture()) {
            var frozen=PerfilImportacionJson.Escribir(f.Source.View());var result=ResumeEvaluation(f.Source.View());
            Check(result.Estado=="EN_CURSO"&&result.ArchivosConfirmados==1&&result.HayPendientes,"Resume did not derive remaining inventory");
            Check(f.Source.Evaluate().Estado=="DETENIDO"&&frozen==PerfilImportacionJson.Escribir(f.Source.View()),"Normal consolidation cleared stop or evaluator mutated data");
            var files=PerfilImportacionJson.Escribir(f.Source.Phase.Files);var attempts=PerfilImportacionJson.Escribir(f.Source.Proofs);var identity=PerfilImportacionJson.Escribir(f.Source.Roots[0].IdentidadExpediente);var revision=f.Lot.Revision;
            var ack=f.ReanudarLote(f.Lot.Contexto,"20",revision);
            Check(!ack.DetencionSolicitada&&ack.Revision==revision+1&&ack.Estado=="EN_CURSO"&&f.Events==1,"Resume control did not commit once");
            Check(files==PerfilImportacionJson.Escribir(f.Source.Phase.Files)&&attempts==PerfilImportacionJson.Escribir(f.Source.Proofs)&&identity==PerfilImportacionJson.Escribir(f.Source.Roots[0].IdentidadExpediente),"Resume changed original results");
            Rejected("CONFLICTO_REVISION",()=>f.ReanudarLote(f.Lot.Contexto,"20",revision));
            f.Lot.Estado="DETENIDO";f.Lot.DetencionSolicitada=true;f.Lot.Revision++;
            Rejected("CONFLICTO_REVISION",()=>f.ReanudarLote(f.Lot.Contexto,"20",revision));Check(f.Lot.DetencionSolicitada&&f.Events==1,"Old resume cleared new stop");
        }
        foreach(bool received in new[]{false,true})using(var f=new ConsolidationFixture()) {
            f.Proofs.Clear();f.Roots[0].IdentidadExpediente=null;f.Roots[0].Estado="PENDIENTE";
            if(!received){var file=f.Phase.Source.FileRecord;file.EstadoRecepcion="PENDIENTE";file.BytesRecibidos="0";file.StagingRef=null;file.ContenidoHash=null;}
            f.Phase.Source.Lot.Estado="DETENIDO";f.Phase.Source.Lot.DetencionSolicitada=true;
            Check(ResumeEvaluation(f.View()).Estado==(received?"LISTO":"RECIBIENDO"),"Resume forced EN_CURSO without progress");
        }
        using(var f=new ConsolidationFixture()) {f.Finish();f.Phase.Source.Lot.Estado="DETENIDO";f.Phase.Source.Lot.DetencionSolicitada=true;Check(ResumeEvaluation(f.View()).Estado=="COMPLETADO","Resume hid final evidence");}
        foreach(var mutation in new Action<ResumeLotFixture>[] {
            f=>f.Lot.Estado="COMPLETADO",f=>f.Lot.Estado="FALLIDO",f=>f.Lot.Estado="PENDIENTE_VERIFICACION",f=>f.Lot.DetencionSolicitada=false,
            f=>f.Source.Phase.Files["31"].IntentoActivoId="999",f=>f.Source.Phase.Files["31"].EstadoEfecto="EN_CURSO",f=>f.Source.Phase.Files["31"].EstadoRecepcion="INCIERTO",
            f=>f.Source.Roots[0].Estado="INCIERTO",f=>f.Source.Proofs[0].Estado="RESERVADO",f=>f.Source.Phase.Source.FileRecord.Resultado.DocumentoId="999"
        })using(var f=new ResumeLotFixture()){mutation(f);var before=PerfilImportacionJson.Escribir(f.Source.View());MustRefuseResume(()=>f.ReanudarLote(f.Lot.Contexto,"20",f.Lot.Revision));Check(f.Events==0&&before==PerfilImportacionJson.Escribir(f.Source.View()),"Rejected resume mutated data");}
        foreach(bool failEvent in new[]{false,true})using(var f=new ResumeLotFixture()){
            f.FailEvent=failEvent;f.FailUpdate=!failEvent;var before=PerfilImportacionJson.Escribir(f.Source.View());bool failed=false;try{f.ReanudarLote(f.Lot.Contexto,"20",f.Lot.Revision);}catch(Exception){failed=true;}
            Check(failed&&f.Events==0&&before==PerfilImportacionJson.Escribir(f.Source.View()),"Transaction model committed failed update/event");
        }
        using(var f=new ResumeLotFixture())using(var start=new ManualResetEventSlim(false)) {
            var revision=f.Lot.Revision;int wins=0,conflicts=0;
            var workers=Enumerable.Range(0,2).Select(i=>Task.Run(()=>{start.Wait();try{f.ReanudarLote(f.Lot.Contexto,"20",revision);Interlocked.Increment(ref wins);}catch(PerfilImportacionException e){Check(e.Codigo=="CONFLICTO_REVISION","Unexpected concurrent rejection");Interlocked.Increment(ref conflicts);}})).ToArray();start.Set();Task.WaitAll(workers);
            Check(wins==1&&conflicts==1&&f.Events==1&&f.Controls==1,"Two tabs resumed twice in transaction model");
        }
        using(var f=new ResumeLotFixture()) {
            var identity=ReceiptClone(f.Lot.Contexto);var destination=ReceiptClone(f.Lot.Contexto);bool allowed=true;
            var service=new CoordinadorImportacionCarpetas(()=>identity,f,n=>destination,(c,s)=>{if(!allowed)throw new PerfilImportacionException("ACCESO_DENEGADO","fixture");});
            var request=new ReanudarLoteImportacionSolicitud{loteId="20",revisionEsperada=f.Lot.Revision};
            allowed=false;Rejected("ACCESO_DENEGADO",()=>service.ReanudarLote(request));allowed=true;
            identity.UsuarioRef="99";Rejected("ACCESO_DENEGADO",()=>service.ReanudarLote(request));identity=ReceiptClone(f.Lot.Contexto);
            identity.AmbitoRef="99";Rejected("ACCESO_DENEGADO",()=>service.ReanudarLote(request));identity=ReceiptClone(f.Lot.Contexto);
            destination.SelloDestino="changed";Rejected("ACCESO_DENEGADO",()=>service.ReanudarLote(request));destination=ReceiptClone(f.Lot.Contexto);
            Check(f.Controls==0,"Rejected authority reached persistence");var ack=service.ReanudarLote(request);Check(!ack.detencionSolicitada&&f.Controls==1,"Authorized resume did not use control");
        }
        var q=new QueryRepository();q.Lot.Estado="DETENIDO";q.Lot.DetencionSolicitada=true;q.Active=false;q.Uncertain=false;
        var query=new ServicioConsultaImportacion(()=>ReceiptClone(q.Lot.Contexto),q,n=>ReceiptClone(q.Lot.Contexto),(c,s)=>{},25);
        var qr=new ConsultarLoteImportacionSolicitud{loteId="20"};var original=PerfilImportacionJson.Escribir(q.Lot);var projected=query.Consultar(qr);
        Check(projected.accionesPermitidas.Contains("REANUDAR_LOTE")&&!projected.accionesPermitidas.Contains("SOLICITAR_AVANCE")&&projected.motivoBloqueo=="REANUDACION_DISPONIBLE"&&original==PerfilImportacionJson.Escribir(q.Lot),"Query resumed or confused capability");
        q.Active=true;Check(!query.Consultar(qr).accionesPermitidas.Contains("REANUDAR_LOTE"),"Active query offers resume");q.Active=false;q.Uncertain=true;Check(query.Consultar(qr).motivoBloqueo=="RESULTADO_INCIERTO","Uncertain query misclassified");
        var resources=new ResourceFixture();resources.AddLevel=false;resources.Owner="99";Rejected("ACCESO_DENEGADO",()=>resources.Adapter.ResolverDestino(resources.Http,"7","REANUDAR_LOTE"));
        resources.AddLevel=true;resources.UploadLevel=false;Rejected("ACCESO_DENEGADO",()=>resources.Adapter.ResolverDestino(resources.Http,"7","REANUDAR_LOTE"));
        Check(typeof(WebServiceImportacionCarpetas).GetMethod("ReanudarLote")!=null,"ASMX resume contract missing");
        using(var f=new ResumeLotFixture()) {
            var fixture=new ResourceFixture();int factories=0;
            var service=new CoordinadorImportacionCarpetas(()=>ReceiptClone(f.Lot.Contexto),f,n=>ReceiptClone(f.Lot.Contexto),(c,s)=>{});
            Func<System.Web.HttpContext,ImportacionCarpetasContextAdapter,string,CoordinadorImportacionCarpetas> compose=(http,authority,action)=>{Check(action=="REANUDAR_LOTE","ASMX redirected resume to a business phase");factories++;return service;};
            var constructor=typeof(WebServiceImportacionCarpetas).GetConstructors(BindingFlags.NonPublic|BindingFlags.Instance).Single();
            var endpoint=(WebServiceImportacionCarpetas)constructor.Invoke(new object[]{fixture.Adapter,(Func<LimitesImportacionCarpetas>)(()=>RequestLimits()),null,null,null,compose});
            var previous=System.Web.HttpContext.Current;
            Func<string,bool,RespuestaPerfilImportacion> invoke=(body,token)=>{
                var worker=new ImportRequest{Body=System.Text.Encoding.UTF8.GetBytes(body)};var http=SessionFixture(worker:worker);worker.Token=ImportacionCarpetasRequestGuard.Emitir(http,RequestContext());if(!token)worker.Token=null;System.Web.HttpContext.Current=http;
                // The dispatcher must read the guarded body, never these coerced arguments.
                return endpoint.ReanudarLote(new ReanudarLoteImportacionSolicitud{loteId="999",revisionEsperada=999});
            };
            var body="{\"solicitud\":{\"loteId\":\"20\",\"revisionEsperada\":"+f.Lot.Revision+"}}";
            try {
                Check(invoke(body,false).codigo=="SOLICITUD_NO_VERIFICADA"&&factories==0,"Resume bypassed CSRF");
                Check(!invoke(body.Replace("\"loteId\":\"20\"","\"loteId\":\"20\",\"usuarioRef\":\"99\""),true).ok&&factories==0,"Resume accepted extra authority fields");
                var ack=invoke(body,true);Check(ack.ok&&((DetencionLoteImportacionResultado)ack.datos).loteId=="20"&&factories==1&&f.Controls==1,"Resume body/envelope or guarded composition mismatch");
            }finally{System.Web.HttpContext.Current=previous;}
        }
        Console.WriteLine("DOC-120 lot resume: eligibility, authority, preserved evidence, revision races and transaction model (not real MySQL): passed");
    }
}
