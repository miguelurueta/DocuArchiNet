using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;

public static partial class ImportacionCarpetasBehaviorTests {
    sealed class IndependentPhasePort : IControlLoteImportacionRepository,IControlArchivosIndependientesImportacion,IDisposable {
        public readonly DocumentPhaseFixture F=new DocumentPhaseFixture();readonly object sync=new object();readonly ControlArchivosSelladoImportacion seal;
        public int Loads,Claims,Negatives;public Action AfterClaim;public CoordinadorImportacionCarpetas Coordinator;
        public IndependentPhasePort(){
            F.AddFile();seal=SealIndependent(F.View());F.Source.Lot.ControlArchivos=seal.Cabecera;
            Coordinator=new CoordinadorImportacionCarpetas(()=>F.Source.Lot.Contexto,this,id=>F.Source.Lot.Contexto,(c,p)=>{if(!F.Allowed)throw new PerfilImportacionException("ACCESO_DENEGADO","fixture");},
                (c,l,r)=>View(),(c,l,id)=>FileById(id),(c,r)=>F.Source.Attempt!=null&&F.Source.Attempt.SolicitudHash==r.SolicitudHash?ReceiptClone(F.Source.Attempt):null,
                (l,f)=>new AlmacenStagingImportacion(F.Source.Stage.Receipt.Root,F.Source.Stage.Receipt.Limits,l,f,10,200),s=>F.Source.Adapter);
        }
        public VistaBarreraCreacionImportacion View(){var lot=ReceiptClone(F.Source.Lot);lot.ControlArchivos=seal.Cabecera;return new VistaBarreraCreacionImportacion{Lote=lot,Raiz=ReceiptClone(F.Source.Root),EsFocal=true};}
        public ArchivoImportacionCarpetas FileById(string id){Loads++;Check(id=="30","Storage requested sibling metadata");var file=ReceiptClone(F.Files[id]);ControlArchivoIndependienteImportacion.Verificar(F.Source.Lot,F.Source.Root,file,seal.Cabecera,seal.Pruebas[id]);file.ProcedenciaVerificada=true;return file;}
        public ContextoImportacionCarpetas ObtenerContextoLote(ContextoImportacionCarpetas c,string id){return F.ObtenerContextoLote(c,id);}
        public LoteImportacionCarpetas ObtenerLote(ContextoImportacionCarpetas c,string id){var lot=F.ObtenerLote(c,id);lot.ControlArchivos=seal.Cabecera;return lot;}
        public LoteImportacionCarpetas SolicitarDetencion(ContextoImportacionCarpetas c,string id,uint revision){return F.SolicitarDetencion(c,id,revision);}
        public void PrepararControlArchivos(ContextoImportacionCarpetas c,string id){Check(id=="20","Foreign initialization");}
        public void RegistrarOriginalNoDisponible(ContextoImportacionCarpetas c,string lot,string id,uint revision,string cause,IntentoImportacionCarpetas attempt){
            lock(sync){var file=F.Source.FileRecord;Check(file.ArchivoId==id&&file.Revision==revision&&F.Source.Starts==0&&F.Source.Writers==0,"Negative after an invocation or stale state");if(attempt!=null){Check(F.Source.Attempt.Estado=="RESERVADO"&&F.Source.Attempt.TokenReclamacion==attempt.TokenReclamacion,"Foreign reservation closed");F.Source.Attempt.Estado="RECHAZADO";F.Source.Attempt.FinUtc="2026-11-18T00:00:00Z";}
                file.EstadoEfecto="NO_INICIADO";file.IntentoActivoId=null;file.Resultado=new ResultadoEfectoImportacion{Estado="RECHAZADO",Codigo="ORIGINAL_NO_DISPONIBLE",Mensaje=cause};file.Revision++;Negatives++;}
        }
        public IntentoImportacionCarpetas ReservarIntento(ContextoImportacionCarpetas c,ReclamacionImportacionCarpetas request){
            lock(sync){if(F.Source.Attempt!=null&&F.Source.Attempt.SolicitudHash==request.SolicitudHash)return ReceiptClone(F.Source.Attempt);
                RepositoryHelper("ValidarReservaDocumental",F.Source.Lot,F.Source.Root,F.Source.FileRecord,request,F.Source.Attempt!=null);
                Claims++;F.Source.Attempt=new IntentoImportacionCarpetas{IntentoId="50",LoteId="20",RaizId="40",ArchivoId="30",Fase="ALMACENAR",Estado="RESERVADO",EjecutorRef=request.EjecutorRef,TokenReclamacion=request.TokenReclamacion,SolicitudHash=request.SolicitudHash};F.Source.FileRecord.EstadoEfecto="EN_CURSO";F.Source.FileRecord.IntentoActivoId="50";F.Source.FileRecord.Revision++;F.Source.Lot.Estado="EN_CURSO";F.Source.Lot.Revision++;
                var copy=ReceiptClone(F.Source.Attempt);copy.EsNuevaReserva=true;if(AfterClaim!=null)AfterClaim();return copy;}
        }
        public ResultadoFaseDocumentoImportacion Run(){return Coordinator.AlmacenarArchivo("20","30",F.OriginalRevision,"independent-A");}
        public void Dispose(){F.Dispose();}
    }
    static T IndependentCall<T>(string name,params object[] args){try{return (T)typeof(CoordinadorImportacionCarpetas).GetMethod(name,BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,args);}catch(TargetInvocationException ex){throw ex.InnerException;}}
    static ControlArchivosSelladoImportacion SealIndependent(VistaBarreraCreacionImportacion view){
        view.Plan.Huella=new string('d',64); // Immutable synthetic plan identity, never a real plan.
        var seal=ControlArchivoIndependienteImportacion.Sellar(view.Lote,view.Plan,view.Raices,view.Archivos);
        view.Lote.ControlArchivos=seal.Cabecera;view.EsFocal=true;
        foreach(var file in view.Archivos){ControlArchivoIndependienteImportacion.Verificar(view.Lote,view.Raices.Single(r=>r.RaizId==file.RaizId),file,seal.Cabecera,seal.Pruebas[file.ArchivoId]);file.ProcedenciaVerificada=true;}
        return seal;
    }
    static ReservaCreacionVerificadaImportacion VerifyIndependent(VistaBarreraCreacionImportacion view,ArchivoImportacionCarpetas target,IAlmacenStagingImportacion store,List<IRetencionVerificadaImportacion> handles,Action authorize){
        return IndependentCall<ReservaCreacionVerificadaImportacion>("VerificarStagingCreacion",view,
            new Func<ContextoImportacionCarpetas,string,string,ArchivoImportacionCarpetas>((c,l,id)=>{throw new Exception("Cross-file metadata loaded: "+id);}),
            new Func<LoteImportacionCarpetas,ArchivoImportacionCarpetas,IAlmacenStagingImportacion>((l,f)=>{Check(f.ArchivoId==target.ArchivoId,"Another original was opened");return store;}),
            new Action<ContextoImportacionCarpetas,ConfiguracionPerfilImportacion>((c,s)=>authorize()),handles,target,null);
    }
    static void IndependentFileCases(){
        // The actual repository constructor must work before any file is loaded.
        // Prior tests supplied an already-built port and missed its null lists.
        foreach(bool created in new[]{false,true}){
            var lot=new LoteImportacionCarpetas{LoteId="20"};
            var root=new RaizImportacionCarpetas{RaizId="40",LoteId="20",IdentidadExpediente=created?new IdentidadExpedienteImportacion{ExpedienteId="60"}:null};
            var builder=typeof(MySqlImportacionCarpetasRepository).GetMethod("CrearVistaBarreraFocal",BindingFlags.Static|BindingFlags.NonPublic);
            Check(builder!=null,"Actual focal view constructor is missing");
            var view=(VistaBarreraCreacionImportacion)builder.Invoke(null,new object[]{lot,root});
            Check(view.EsFocal&&Object.ReferenceEquals(view.Lote,lot)&&Object.ReferenceEquals(view.Raiz,root)&&view.Plan==null,"Focal scope changed during construction");
            Check(view.Raices!=null&&view.Raices.Count==1&&Object.ReferenceEquals(view.Raices[0],root)&&view.Archivos!=null&&view.Archivos.Count==0,"Focal view collections unavailable before loading files");
            view.Archivos.Add(new ArchivoImportacionCarpetas{ArchivoId="30",RaizId="40"});
            Check(view.Archivos.Single().ArchivoId=="30","Pending root cannot collect its candidate file");
        }
        using(var f=new IndependentPhasePort()){
            var missing=f.F.Source.Stage.PathFor("31");File.Move(missing,missing+".missing");var result=f.Run();
            Check(result.Resultado.Estado=="CONFIRMADO"&&f.F.Source.Writers==1&&f.Claims==1&&f.Loads==3,"Complete storage path depends on B or reloads all originals");
            var repeated=f.Run();Check(repeated.Repeticion&&f.F.Source.Writers==1&&f.Claims==1,"Repeated A invokes writer again");
        }
        foreach(bool afterClaim in new[]{false,true})using(var f=new IndependentPhasePort()){
            var path=f.F.Source.Stage.PathFor("30");if(afterClaim)f.AfterClaim=()=>File.Move(path,path+".missing");else File.Move(path,path+".missing");
            try{f.Run();throw new Exception("Unavailable A reached writer");}catch(ArchivoNoDisponibleImportacionException ex){Check(ex.ArchivoId=="30"&&ex.Fase=="ALMACENAR","Wrong scoped no-invocation failure");}
            Check(f.F.Source.Writers==0&&f.F.Source.Starts==0&&f.Negatives==1&&f.Claims==(afterClaim?1:0)&&f.F.Source.FileRecord.EstadoEfecto=="NO_INICIADO"&&f.F.Source.FileRecord.IntentoActivoId==null,"No-effect closure lost certainty/progress");
        }
        foreach(int n in new[]{1,3,100,1000})using(var phase=new DocumentPhaseFixture()){
            var view=phase.View();view.Archivos.Clear();view.Plan.Manifiesto.Archivos.Clear();
            for(int k=0;k<n;k++){var file=ReceiptClone(phase.Source.FileRecord);file.ArchivoId=(30+k).ToString();file.ClienteArchivoId="f"+k;file.NombreOriginal=k+".pdf";file.RutaRelativa="Folder/"+file.NombreOriginal;view.Archivos.Add(file);}
            view.Lote.TotalArchivos=(uint)n;view.Lote.TotalBytes=(UInt64.Parse(view.Archivos[0].BytesPrevistos)*(ulong)n).ToString();
            var timer=Stopwatch.StartNew();var seal=SealIndependent(view);timer.Stop();
            Check(seal.Pruebas.Count==n&&seal.Pruebas.Values.All(p=>p.Hermanos.Count<=10),"Proof size is not bounded logarithmically");
            foreach(var file in view.Archivos)ControlArchivoIndependienteImportacion.Verificar(view.Lote,view.Raiz,file,seal.Cabecera,seal.Pruebas[file.ArchivoId]);
            var target=view.Archivos[0];int io=0,auth=0;var handles=new List<IRetencionVerificadaImportacion>();
            var store=new CountedStaging(target,()=>io++);var result=VerifyIndependent(view,target,store,handles,()=>auth++);
            Check(result.ArchivosVerificados.Single()==target.ArchivoId&&io==1&&auth==1&&handles.Count==1,"Focal retention scales with sibling count");handles.ForEach(h=>h.Dispose());
            Console.WriteLine("DOC120 independent local metrics files="+n+" proofs="+seal.Pruebas.Count+" sealMs="+timer.ElapsedMilliseconds+" focalMetadataLoads=0 originalVerifications="+io+" siblingVisits=0 SQL=not_executed");
            var broken=ReceiptClone(target);broken.NombreOriginal="changed.pdf";Rejected("CONFLICTO_REVISION",()=>ControlArchivoIndependienteImportacion.Verificar(view.Lote,view.Raiz,broken,seal.Cabecera,seal.Pruebas[target.ArchivoId]));
            broken=ReceiptClone(target);broken.ValoresPlan[0].Valor="changed";Rejected("CONFLICTO_REVISION",()=>ControlArchivoIndependienteImportacion.Verificar(view.Lote,view.Raiz,broken,seal.Cabecera,seal.Pruebas[target.ArchivoId]));
            broken=ReceiptClone(target);broken.RaizId="999";Rejected("CONFLICTO_REVISION",()=>ControlArchivoIndependienteImportacion.Verificar(view.Lote,view.Raiz,broken,seal.Cabecera,seal.Pruebas[target.ArchivoId]));
            broken=ReceiptClone(target);broken.EstadoEfecto="INCIERTO";broken.Revision++; // Mutable progress is not permanent provenance.
            ControlArchivoIndependienteImportacion.Verificar(view.Lote,view.Raiz,broken,seal.Cabecera,seal.Pruebas[target.ArchivoId]);
            var proof=ReceiptClone(seal.Pruebas[target.ArchivoId]);proof.Hermanos.Add(new string('e',64));Rejected("CONFLICTO_REVISION",()=>ControlArchivoIndependienteImportacion.Verificar(view.Lote,view.Raiz,target,seal.Cabecera,proof));
            if(n>1){proof=ReceiptClone(seal.Pruebas[target.ArchivoId]);proof.Indice=1;Rejected("CONFLICTO_REVISION",()=>ControlArchivoIndependienteImportacion.Verificar(view.Lote,view.Raiz,target,seal.Cabecera,proof));}
            var header=ReceiptClone(seal.Cabecera);header.ContextoHash=new string('f',64);Rejected("CONFLICTO_REVISION",()=>ControlArchivoIndependienteImportacion.Verificar(view.Lote,view.Raiz,target,header,seal.Pruebas[target.ArchivoId]));
        }
        // Real local bytes and handles: B may disappear, A remains independently usable.
        using(var fixture=new CreationStagingFixture()){
            fixture.AddFile();var view=fixture.View;SealIndependent(view);var a=view.Archivos.Single(f=>f.ArchivoId=="30");
            File.Move(fixture.PathFor("31"),fixture.PathFor("31")+".missing");
            var store=new AlmacenStagingImportacion(fixture.Receipt.Root,fixture.Receipt.Limits,view.Lote,a,10,200);
            var proof=fixture.Reserve();Check(fixture.Reservations==1&&proof.ArchivosVerificados.SequenceEqual(new[]{"30"}),"Root creation depends on B's missing original");
            view.Raiz.Estado="EXPEDIENTE_CONFIRMADO";view.Raiz.IdentidadExpediente=new IdentidadExpedienteImportacion{OrigenLogico="RA",ExpedienteId="60",RelacionNivelId="70",NivelRef=view.Lote.Contexto.NivelRef};
            a.ValoresPlan=new List<ValorReglaPerfilImportacion>();a.ValoresEfectivos=new List<ValorReglaPerfilImportacion>();a.ValoresEfectivosFijados=true;
            // Physical proof below exercises creation/retention without inventing documental rules.
            view.Raiz.Estado="PENDIENTE";view.Raiz.IdentidadExpediente=null;
            using(var held=fixture.Keep()){Check(held.Resultado.ArchivosVerificados.SequenceEqual(new[]{"30"}),"Retains sibling B");bool blocked=false;try{File.Delete(fixture.PathFor("30"));}catch(IOException){blocked=true;}Check(blocked,"Original handle released before effect");}
            File.Delete(fixture.PathFor("30"));
            try{fixture.Keep();throw new Exception("Missing A was accepted");}catch(ArchivoNoDisponibleImportacionException ex){Check(ex.ArchivoId=="30"&&ex.RaizId=="40"&&ex.Fase=="CREAR_EXPEDIENTE"&&ex.Causa=="ORIGINAL_AUSENTE","Missing original diagnostic lost scope/certainty");}
        }
        // Negative checkpoint has no documentary identity, never means completed.
        var negative=new ResultadoEfectoImportacion{Estado="RECHAZADO",Codigo="ORIGINAL_NO_DISPONIBLE"};Check(ControlArchivoIndependienteImportacion.EsNoDisponible(negative),"Typed no-effect diagnostic rejected");negative.DocumentoId="90";Check(!ControlArchivoIndependienteImportacion.EsNoDisponible(negative),"Document identity downgraded to no invocation");
        using(var phase=new DocumentPhaseFixture()){
            var view=phase.View();SealIndependent(view);var file=view.Archivos[0];
            foreach(string state in new[]{"EXPEDIENTE_CONFIRMADO","PARCIAL","INCIERTO"}){view.Raiz.Estado=state;Check(DocumentBarrier(view,file,null),"Confirmed identity blocked by aggregate state "+state);}
            file.ProcedenciaVerificada=false;Check(!DocumentBarrier(view,file,null),"Unproven target admitted");file.ProcedenciaVerificada=true;
            file.IntentoActivoId="999";Check(!DocumentBarrier(view,file,null),"Foreign reservation admitted");file.IntentoActivoId=null;
            view.Lote.DetencionSolicitada=true;Check(!DocumentBarrier(view,file,null),"Stop ignored");view.Lote.DetencionSolicitada=false;
            view.Raiz.IdentidadExpediente=null;Rejected("CONFLICTO_REVISION",()=>DocumentBarrier(view,file,null));
        }
        Check(new ConsultarRecursoLoteSolicitud().versionLectura==1,"Legacy focal version changed");
        Func<string,int,int,bool,bool,bool,int,string> aggregate=(state,total,complete,created,active,uncertain,incidents)=>(string)RepositoryHelper("CalcularEstadoRaizFocal",state,total,complete,created,active,uncertain,incidents);
        Check(aggregate("EXPEDIENTE_CONFIRMADO",3,2,true,false,false,1)=="PARCIAL","Pending original hidden by aggregate");
        Check(aggregate("EXPEDIENTE_CONFIRMADO",3,2,true,false,true,0)=="INCIERTO","Uncertainty discarded");
        Check(aggregate("EXPEDIENTE_CONFIRMADO",3,3,true,false,false,0)=="COMPLETADA","Confirmed completeness not recognized");
        foreach(string state in new[]{"BLOQUEADO","CANCELADA"})Check(aggregate(state,3,1,true,false,false,0)==state,"Unrelated checkpoint reactivates excluded root");
        Rejected("CONFLICTO_REVISION",()=>aggregate("COMPLETADA",3,2,true,false,false,0));Rejected("CONFLICTO_REVISION",()=>aggregate("PENDIENTE",0,0,false,false,false,0));
        var data=new DataTable();foreach(string col in new[]{"COLUMN_NAME","SEQ_IN_INDEX","NON_UNIQUE","SUB_PART"})data.Columns.Add(col,typeof(object));
        int index=0;foreach(string col in new[]{"lote_id","recurso_tipo","recurso_id","accion"})data.Rows.Add(col,++index,1,DBNull.Value);
        RepositoryHelper("ValidarIndiceControlFocal",data);
        foreach(var change in new Action<DataTable>[] {d=>d.Rows.RemoveAt(0),d=>d.Rows[0]["NON_UNIQUE"]=0,d=>d.Rows[0]["SUB_PART"]=1,d=>d.Rows[0]["COLUMN_NAME"]="other",d=>d.Rows[1]["SEQ_IN_INDEX"]=1}){var broken=data.Copy();change(broken);Rejected("CONTROL_FOCAL_NO_CONFIGURADO",()=>RepositoryHelper("ValidarIndiceControlFocal",broken));}
        var resources=new ResourceFixture();var repository=new QueryRepository();repository.Lot.Contexto=resources.Adapter.ResolverDestino(resources.Http,"7");repository.Lot.Snapshot.Configuracion=resources.Configuration();
        var service=new ServicioConsultaImportacion(()=>resources.Adapter.Resolver(resources.Http),repository,id=>resources.Adapter.ResolverDestino(resources.Http,id,"CONSULTAR"),(c,p)=>resources.Adapter.AutorizarRecursos(resources.Http,c,p,"CONSULTAR"),25);
        var request=new ConsultarRecursoLoteSolicitud{loteId="20",tipo="ARCHIVOS",recursoId="30",versionLectura=2};var focal=service.ConsultarFocal(request);
        Check(focal.versionLectura==2&&focal.recurso.recursoId=="30"&&focal.padre.recursoId=="40"&&focal.huellaRecursos==null&&focal.progreso==null,"Focal V2 computes all-resource digest or loses identity");
        foreach(var alter in new Action<VistaConsultaFocalImportacion>[] {v=>v.VersionLectura=1,v=>v.Archivo.RaizId="999",v=>v.Raiz.LoteId="999",v=>v.HuellaRecursos=new string('a',64),v=>v.Progreso=new ProgresoConfirmadoImportacion(),v=>v.Archivo.Revision=0}){repository.AlterFocalV2=alter;Unavailable(()=>service.ConsultarFocal(request));}
        repository.AlterFocalV2=null;request.versionLectura=3;Rejected("SOLICITUD_INVALIDA",()=>service.ConsultarFocal(request));request.versionLectura=2;
        int reads=repository.FocalReads;repository.Lot.Contexto.UsuarioRef="99";Rejected("ACCESO_DENEGADO",()=>service.ConsultarFocal(request));Check(repository.FocalReads==reads,"V2 other owner reaches file");repository.Lot.Contexto.UsuarioRef="1";
        resources.ImportAllowed=false;Rejected("ACCESO_DENEGADO",()=>service.ConsultarFocal(request));resources.ImportAllowed=true;
        using(var phase=new DocumentPhaseFixture()){
            var lot=phase.Source.Lot;var file=ReceiptClone(phase.Source.FileRecord);file.EstadoEfecto="EN_CURSO";file.IntentoActivoId="50";
            var attempt=new IntentoImportacionCarpetas{IntentoId="50",LoteId=lot.LoteId,RaizId=file.RaizId,ArchivoId=file.ArchivoId,Fase="ALMACENAR",Estado="RESERVADO"};
            var diary=new DataTable();foreach(string col in new[]{"accion","codigo","recurso_tipo","recurso_id","actor_ref"})diary.Columns.Add(col,typeof(object));diary.Rows.Add("RESERVAR_INTENTO","INTENTO_RESERVADO","ARCHIVO",file.ArchivoId,lot.Contexto.UsuarioRef);
            RepositoryHelper("ValidarReservaSinInvocacionFocal",lot,file,attempt,diary);
            foreach(var change in new Action<DataTable>[] {d=>d.Rows.Add("INICIAR_ALMACENAMIENTO_V2","ALMACENAMIENTO_PREPARADO_V2","ARCHIVO",file.ArchivoId,lot.Contexto.UsuarioRef),d=>d.Rows[0]["actor_ref"]="foreign",d=>d.Rows[0]["recurso_id"]="999",d=>d.Rows[0]["accion"]="AUTORIZAR_INVOCACION_DOCUMENTAL"}){var broken=diary.Copy();change(broken);Rejected("CONFLICTO_REVISION",()=>RepositoryHelper("ValidarReservaSinInvocacionFocal",lot,file,attempt,broken));}
            foreach(var change in new Action<IntentoImportacionCarpetas>[] {i=>i.Estado="EN_CURSO",i=>i.LoteId="999",i=>i.ArchivoId="999",i=>i.RaizId="999",i=>i.Resultado=new ResultadoEfectoImportacion(),i=>i.FinUtc="2026-11-18T00:00:00Z"}){var broken=ReceiptClone(attempt);change(broken);Rejected("CONFLICTO_REVISION",()=>RepositoryHelper("ValidarReservaSinInvocacionFocal",lot,file,broken,diary));}
        }
        var previousHttp=System.Web.HttpContext.Current;
        try {
            var worker=new ImportRequest{TimingRef=new string('c',32)};var http=SessionFixture(worker:worker);System.Web.HttpContext.Current=http;
            MedicionAlmacenamientoImportacion.Preparar(http,new AvanzarLoteImportacionSolicitud{loteId="20",recursoId="30",fase="ALMACENAR"});
            MedicionAlmacenamientoImportacion.MedirParte("sql_archivo",()=>true);MedicionAlmacenamientoImportacion.Contar("pdf_bytes",677);MedicionAlmacenamientoImportacion.Contar("original_open",1);MedicionAlmacenamientoImportacion.Contar("PRIVATE_SECRET",999);
            string metrics=(string)typeof(MedicionAlmacenamientoImportacion).GetMethod("Cabecera",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,null);
            Check(metrics.Contains("doc120_sql_archivo")&&metrics.Contains("doc120_pdf_bytes;dur=0.000;desc=\"n=677\"")&&metrics.Contains("doc120_original_open")&&!metrics.Contains("PRIVATE_SECRET"),"Opt-in counters lose units or leak labels");
            System.Web.HttpContext.Current=SessionFixture();MedicionAlmacenamientoImportacion.Contar("pdf_bytes",677);Check(typeof(MedicionAlmacenamientoImportacion).GetMethod("Cabecera",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,null)==null,"Counters cross request/window");
        } finally {System.Web.HttpContext.Current=previousHttp;}
        Check(DiagnosticoOperacionImportacion.Texto("CONTROL_FOCAL_NO_CONFIGURADO").Contains("script09"),"Manual schema prerequisite is generic");
        Console.WriteLine("DOC-120 independent files: provenance, bounded target retention, missing siblings, own-file diagnostics and legacy default passed (local bytes/ports, no DB)");
    }
}
