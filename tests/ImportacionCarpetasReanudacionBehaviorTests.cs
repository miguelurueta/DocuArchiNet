using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public static partial class ImportacionCarpetasBehaviorTests {
    sealed partial class CreationPhaseFixture {
        readonly object resumeSync=new object();
        public int Handoffs,ResumePreflights,ResumeWrites,ResumeEvents,ResumeFailAt;
        public bool ResumeCatalogFailure,LostHandoffResponse,ResumeEventFailure;
        public Action BeforeHandoff;
        public List<string> ResumeSql=new List<string>();
        public void EnableResume(bool stopped=true) {
            Stage.View.Lote.Estado="EN_CURSO";Stage.View.Lote.Revision=5;Stage.View.Lote.DetencionSolicitada=stopped;
            Stage.View.Raiz.Estado="CREANDO";Stage.View.Raiz.Revision=2;Stage.View.Raiz.IntentoActivoId="50";
            Stage.View.Plan.Resultado.Elementos.RemoveAll(e=>e.Ambito=="RAIZ"&&e.RaizIdCliente==Stage.View.Raiz.ClienteRaizId);
            Stage.View.Plan.Resultado.Elementos.Add(new ValoresElementoImportacion{Ambito="RAIZ",RaizIdCliente=Stage.View.Raiz.ClienteRaizId,Valores=ReceiptClone(Stage.View.Raiz.ValoresPlan)});
            Attempt=new IntentoImportacionCarpetas{IntentoId="50",LoteId="20",RaizId="40",Fase="CREAR_EXPEDIENTE",Estado="RESERVADO",TokenReclamacion=new string('a',32),SolicitudHash=new string('b',64),EjecutorRef="WEB_CREACION",InicioUtc="2026-10-29T12:00:00Z",UltimoContactoUtc="2026-10-29T12:00:00Z"};
            Coordinator=new CoordinadorImportacionCarpetas(()=>Stage.Receipt.Resources.Adapter.Resolver(Stage.Receipt.Resources.Http),this,
                n=>Stage.Receipt.Resources.Adapter.ResolverDestino(Stage.Receipt.Resources.Http,n,"CREAR_EXPEDIENTE"),Authorize,
                (c,l,r)=>{Owner(c,l);Check(r=="40","resume escaped root");return ReceiptClone(Stage.View);},
                (c,l,id)=>{Owner(c,l);return ReceiptClone(Stage.FullFiles[id]);},
                (c,r)=>{Owner(c,r.LoteId);return Attempt.SolicitudHash==r.SolicitudHash?ReceiptClone(Attempt):null;},
                (l,f)=>new AlmacenStagingImportacion(Stage.Receipt.Root,Stage.Receipt.Limits,l,f,10,200),CreateAdapter,
                (c,l,id)=>{Owner(c,l);Check(id=="50","resume escaped attempt");return ResumeView();},ClaimResume,
                command=>{ResumePreflights++;Check(command.EsPreflightReanudacion&&!command.EsVerificacion,"resume preflight can write");Authorize(command.Contexto,command.Snapshot.Configuracion);command.ValidarEntradasLegacy();if(ResumeCatalogFailure)throw new PerfilImportacionException("CONFIGURACION_INVALIDA","Synthetic catalog failure");});
        }
        public VistaReconciliacionImportacion ResumeView(){return new VistaReconciliacionImportacion{Lote=ReceiptClone(Stage.View.Lote),Plan=ReceiptClone(Stage.View.Plan),Raiz=ReceiptClone(Stage.View.Raiz),Intento=ReceiptClone(Attempt)};}
        public AvanzarLoteImportacionSolicitud ResumeRequest(){return new AvanzarLoteImportacionSolicitud{loteId="20",recursoId="40",intentoId="50",fase="RETOMAR_CREACION",revisionEsperada=Stage.View.Raiz.Revision,revisionLoteEsperada=Stage.View.Lote.Revision,levantarDetencion=Stage.View.Lote.DetencionSolicitada,claveOperacion="resume-command"};}
        IntentoImportacionCarpetas ClaimResume(VistaReconciliacionImportacion expected,string token) {
            lock(resumeSync){if(BeforeHandoff!=null)BeforeHandoff();int writes=0;
                Func<string,object[],int> execute=(sql,args)=>{writes++;ResumeWrites++;ResumeSql.Add(sql);var changes=sql.Substring(0,sql.IndexOf(" WHERE ",StringComparison.Ordinal));Check(!sql.Contains("INSERT")&&!changes.Contains("inicio_utc=")&&!changes.Contains("solicitud_hash="),"resume rewrites immutable attempt");return ResumeFailAt==writes?0:1;};
                Action record=()=>{if(ResumeEventFailure)throw new PerfilImportacionException("PRUEBA_EVENTO","Synthetic event rollback");ResumeEvents++;};
                var result=(IntentoImportacionCarpetas)RepositoryHelper("PersistirReanudacionCreacion",expected,ResumeView(),token,1,0,false,new DateTime(2026,10,29,12,1,0,DateTimeKind.Utc),execute,record);
                Handoffs++;Attempt=ReceiptClone(result);Attempt.EsNuevaReserva=false;Stage.View.Lote.DetencionSolicitada=false;Stage.View.Lote.Revision++;Stage.View.Raiz.Revision++;
                if(LostHandoffResponse)throw new PerfilImportacionException("RESULTADO_INCIERTO","Synthetic handoff response loss");return result;
            }
        }
        public ResultadoFaseCreacionImportacion Resume(){return Coordinator.RetomarCreacion(ResumeRequest());}
    }

    static void ReanudacionCreacionCases() {
        using(var f=new CreationPhaseFixture()) {
            f.EnableResume();var receipt=PerfilImportacionJson.Escribir(f.Stage.FullFiles);var snapshot=PerfilImportacionJson.Escribir(f.Stage.View.Lote.Snapshot);var hash=f.Attempt.SolicitudHash;var started=f.Attempt.InicioUtc;
            var old=f.ResumeView();var oldIntent=ReceiptClone(old.Intento);oldIntent.EsNuevaReserva=true;var oldLot=ReceiptClone(old.Lote);oldLot.DetencionSolicitada=false;
            var staleCommand=ComandoCreacionExpedienteImportacion.Preparar(oldLot.Contexto,oldLot,old.Raiz,oldIntent);
            var outcome=f.Resume();Check(outcome.Resultado.Estado=="CONFIRMADO"&&outcome.IntentoId=="50"&&f.Handoffs==1&&f.Reserves==0&&f.Writers==1&&f.Checkpoints==1,"resume is not a single creation on original attempt");
            Check(f.Attempt.TokenReclamacion!=old.Intento.TokenReclamacion&&f.Attempt.SolicitudHash==hash&&f.Attempt.InicioUtc==started&&!f.Stage.View.Lote.DetencionSolicitada,"resume loses original identity/hash/start or ignores stop consent");
            Check(PerfilImportacionJson.Escribir(f.Stage.FullFiles)==receipt&&PerfilImportacionJson.Escribir(f.Stage.View.Lote.Snapshot)==snapshot,"resume changes receipt or snapshot");
            Rejected("CONFLICTO_REVISION",()=>RepositoryHelper("ValidarInicioCreacion",staleCommand,f.Stage.View.Lote,f.Stage.View.Raiz));
            bool refused=false;try{f.Resume();}catch(PerfilImportacionException){refused=true;}Check(refused&&f.Writers==1&&f.Handoffs==1,"confirmed resume replays writer");
        }
        using(var f=new CreationPhaseFixture()){f.EnableResume(false);Check(f.Resume().Resultado.Estado=="CONFIRMADO"&&f.Writers==1,"resume incorrectly requires a prior stop");}
        using(var f=new CreationPhaseFixture()) {
            f.EnableResume();var command=ComandoCreacionExpedienteImportacion.PrepararReanudacion(f.Stage.View.Lote.Contexto,f.Stage.View.Lote,f.Stage.View.Raiz,f.Attempt);
            Rejected("SOLICITUD_INVALIDA",()=>RepositoryHelper("ValidarInicioCreacion",command,f.Stage.View.Lote,f.Stage.View.Raiz));
            Rejected("SOLICITUD_INVALIDA",()=>RepositoryHelper("NormalizarResultadoCreacion",command,new ResultadoEfectoImportacion()));
            Check(f.Attempt.EsNuevaReserva==false&&f.Handoffs==0,"preflight manufactures a claim");
        }
        foreach(var state in new[]{"EN_CURSO","INCIERTO","CONFIRMADO","RECHAZADO"})using(var f=new CreationPhaseFixture()){f.EnableResume();f.Attempt.Estado=state;bool rejected=false;try{f.Resume();}catch(PerfilImportacionException){rejected=true;}Check(rejected&&f.Handoffs==0&&f.Writers==0,"started/closed attempt can resume");}
        foreach(var alter in new Action<AvanzarLoteImportacionSolicitud>[] {r=>r.levantarDetencion=false,r=>r.revisionEsperada--,r=>r.revisionLoteEsperada--})using(var f=new CreationPhaseFixture()){f.EnableResume();var r=f.ResumeRequest();alter(r);bool refused=false;try{f.Coordinator.RetomarCreacion(r);}catch(PerfilImportacionException){refused=true;}Check(refused&&f.Handoffs==0&&f.ResumePreflights==0,"stale revision/stop consent reaches preflight or claim");}
        using(var f=new CreationPhaseFixture()){f.EnableResume();f.ResumeCatalogFailure=true;Rejected("CONFIGURACION_INVALIDA",()=>f.Resume());Check(f.Handoffs==0&&f.Stage.View.Lote.DetencionSolicitada&&f.Writers==0,"catalog failure changes claim/stop");}
        using(var f=new CreationPhaseFixture()){f.EnableResume();f.Stage.Receipt.Resources.ImportAllowed=false;Rejected("ACCESO_DENEGADO",()=>f.Resume());Check(f.Handoffs==0&&f.Writers==0,"revoked authority changes claim");}
        using(var f=new CreationPhaseFixture()){f.EnableResume();File.Move(f.Stage.PathFor("30"),f.Stage.PathFor("30")+".missing");bool refused=false;try{f.Resume();}catch(PerfilImportacionException){refused=true;}Check(refused&&f.Handoffs==0&&f.Writers==0&&f.Stage.View.Lote.DetencionSolicitada,"missing staging changes claim");}
        using(var f=new CreationPhaseFixture()){f.EnableResume();f.BeforeHandoff=()=>f.Stage.View.Lote.Revision++;bool refused=false;try{f.Resume();}catch(PerfilImportacionException){refused=true;}Check(refused&&f.Handoffs==0&&f.ResumeWrites==0&&f.Stage.View.Lote.DetencionSolicitada,"new stop/revision is cleared by stale resume");}
        using(var f=new CreationPhaseFixture()){f.EnableResume();f.LostHandoffResponse=true;var old=f.ResumeRequest();Rejected("RESULTADO_INCIERTO",()=>f.Coordinator.RetomarCreacion(old));Check(f.Writers==0&&f.Handoffs==1&&f.Attempt.Estado=="RESERVADO","lost claim response starts writer");bool refused=false;try{f.Coordinator.RetomarCreacion(old);}catch(PerfilImportacionException){refused=true;}Check(refused&&f.Handoffs==1,"lost claim response automatically reclaims");f.LostHandoffResponse=false;Check(f.Resume().Resultado.Estado=="CONFIRMADO"&&f.Writers==1,"fresh explicit resume after reading cannot continue same attempt");}
        using(var f=new CreationPhaseFixture()){f.EnableResume();f.LoseCommitResponse=true;Check(f.Resume().Resultado.Estado=="CONFIRMADO"&&f.Writers==1,"durable creation result lost after response loss");}
        for(int failed=1;failed<=3;failed++)using(var f=new CreationPhaseFixture()){f.EnableResume();var before=PerfilImportacionJson.Escribir(f.ResumeView());f.ResumeFailAt=failed;Rejected("CONFLICTO_REVISION",()=>f.Resume());Check(f.Handoffs==0&&f.Writers==0&&f.ResumeEvents==0&&before==PerfilImportacionJson.Escribir(f.ResumeView()),"failed CAS committed resume metadata");}
        ResumeSqlGuardCases();ResumeQueryHintCases();ResumeRaceCases();
        Console.WriteLine("DOC-120 permanent resume fences the same attempt, validates preflight and rejects replay (SQL/writer doubles, real staging): passed");
    }
    static void ResumeSqlGuardCases() {
        using(var f=new CreationPhaseFixture()){f.EnableResume();var expected=f.ResumeView();
            foreach(var counts in new[]{new[]{0,0,0},new[]{2,0,0},new[]{1,1,0},new[]{1,0,1}}){int writes=0;Func<string,object[],int> execute=(sql,args)=>{writes++;return 1;};Rejected("CONFLICTO_REVISION",()=>RepositoryHelper("PersistirReanudacionCreacion",expected,f.ResumeView(),new string('c',32),counts[0],counts[1],counts[2]==1,DateTime.UtcNow,execute,(Action)(()=>{})));Check(writes==0,"invalid journal/other claim reaches writes");}
            foreach(var edit in new Action<VistaReconciliacionImportacion>[] {v=>v.Lote.Revision++,v=>v.Lote.DetencionSolicitada=false,v=>v.Raiz.Revision++,v=>v.Intento.TokenReclamacion=new string('d',32),v=>v.Intento.InicioUtc="different"}){var actual=f.ResumeView();edit(actual);int writes=0;Func<string,object[],int> execute=(sql,args)=>{writes++;return 1;};bool refused=false;try{RepositoryHelper("PersistirReanudacionCreacion",expected,actual,new string('c',32),1,0,false,DateTime.UtcNow,execute,(Action)(()=>{}));}catch(PerfilImportacionException){refused=true;}Check(refused&&writes==0,"changed persisted view reaches writes");}
            var predicate=(string)RepositoryHelper("PredicadoRetomarCreacion");Check(predicate.Contains("i.estado='RESERVADO'")&&predicate.Contains("e.accion='RESERVAR_INTENTO'")&&predicate.Contains("i.evidencia_doc IS NULL")&&!predicate.Contains("ultimo_contacto_utc")&&!predicate.Contains("NOW("),"resume eligibility grants authority by lease/time");
        }
    }
    static void ResumeQueryHintCases() {
        using(var f=new CreationPhaseFixture()){f.EnableResume();var page=new PaginaRecursosImportacion<RaizImportacionCarpetas>{Elementos=new List<RaizImportacionCarpetas>{ReceiptClone(f.Stage.View.Raiz)}};int reads=0;
            Func<string,object[],DataTable> query=(sql,args)=>{reads++;Check(sql.StartsWith("SELECT ")&&!sql.Contains("FOR UPDATE")&&sql.Contains("l.solicitante_ref=@usuario")&&sql.Contains("l.ambito_ref=@ambito")&&sql.Contains("l.nivel_ref=@nivel")&&sql.Contains("r.raiz_id IN (@r0)")&&sql.Contains("i.estado='RESERVADO'"),"resume hint query is not scoped/read-only");var table=new DataTable();table.Columns.Add("raiz_id");table.Columns.Add("elegible",typeof(long));table.Rows.Add("40",1L);return table;};
            RepositoryHelper("CompletarRetomarCreacionConsulta",f.Stage.View.Lote,page,query);Check(reads==1&&page.Elementos[0].PuedeRetomarCreacion&&!PerfilImportacionJson.Escribir(page.Elementos[0]).Contains("PuedeRetomarCreacion"),"resume hint not ephemeral");
        }
    }
    static void ResumeRaceCases() {
        using(var f=new CreationPhaseFixture()){f.EnableResume();var expected=f.ResumeView();int winners=0,rejected=0,writers=0;var gate=new object();
            Action<string> contender=token=>{lock(gate){try{Func<string,object[],int> execute=(sql,args)=>1;var claim=(IntentoImportacionCarpetas)RepositoryHelper("PersistirReanudacionCreacion",expected,f.ResumeView(),token,1,0,false,DateTime.UtcNow,execute,(Action)(()=>{}));f.Attempt=ReceiptClone(claim);f.Attempt.EsNuevaReserva=false;f.Stage.View.Raiz.Revision++;f.Stage.View.Lote.Revision++;f.Stage.View.Lote.DetencionSolicitada=false;winners++;if(f.Attempt.Estado=="RESERVADO"&&f.Attempt.TokenReclamacion==token){f.Attempt.Estado="EN_CURSO";writers++;}}catch(PerfilImportacionException){rejected++;}}};
            Task.WaitAll(Task.Run(()=>contender(new string('c',32))),Task.Run(()=>contender(new string('d',32))));Check(winners==1&&rejected==1&&writers==1&&f.Attempt.IntentoId=="50","two contenders start more than one writer");
        }
    }
}
