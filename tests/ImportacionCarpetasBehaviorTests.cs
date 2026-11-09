using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using System.Web;
using System.Web.Hosting;
using System.Web.SessionState;

public static partial class ImportacionCarpetasBehaviorTests {
    static void Check(bool condition,string message) { if(!condition)throw new Exception(message); }

    sealed class StagingStream : Stream {
        readonly MemoryStream inner;public long BytesRead;public int MaxRequested;public bool Fail;public Action OnRead;
        public StagingStream(byte[] contents){inner=new MemoryStream(contents);}
        public override int Read(byte[] buffer,int offset,int count){MaxRequested=Math.Max(MaxRequested,count);if(OnRead!=null)OnRead();if(Fail)throw new IOException("DOC111_PRIVATE_PATH_SENTINEL");var read=inner.Read(buffer,offset,Math.Min(count,113));BytesRead+=read;return read;}
        public override bool CanRead{get{return true;}}public override bool CanSeek{get{return false;}}public override bool CanWrite{get{return false;}}
        public override long Length{get{throw new NotSupportedException();}}public override long Position{get{throw new NotSupportedException();}set{throw new NotSupportedException();}}
        public override long Seek(long offset,SeekOrigin origin){throw new NotSupportedException();}public override void SetLength(long value){throw new NotSupportedException();}
        public override void Write(byte[] buffer,int offset,int count){throw new NotSupportedException();}public override void Flush(){}
        protected override void Dispose(bool disposing){if(disposing)inner.Dispose();base.Dispose(disposing);}
    }

    static byte[] StagingPdf(int pages=1,bool encrypted=false) {
        using(var bytes=new MemoryStream()) {
            var document=new iTextSharp.text.Document();var writer=iTextSharp.text.pdf.PdfWriter.GetInstance(document,bytes);writer.CloseStream=false;
            if(encrypted)writer.SetEncryption(System.Text.Encoding.ASCII.GetBytes("fixture-user"),System.Text.Encoding.ASCII.GetBytes("fixture-owner"),iTextSharp.text.pdf.PdfWriter.ALLOW_PRINTING,iTextSharp.text.pdf.PdfWriter.ENCRYPTION_AES_128);
            document.Open();for(int i=0;i<pages;i++){if(i>0)document.NewPage();document.Add(new iTextSharp.text.Paragraph("DOC111 local synthetic staging page "+i));}document.Close();return bytes.ToArray();
        }
    }

    static T ReceiptClone<T>(T value){return PerfilImportacionJson.Leer<T>(PerfilImportacionJson.Escribir(value));}
    // Doble de persistencia en memoria: usa los guards reales, pero no acredita
    // SQL, bloqueos, commit o rollback MySQL. Las corridas reales son separadas.
    sealed class ReceiptRepository : IRecepcionArchivosImportacionRepository {
        readonly object sync=new object();readonly List<IntentoImportacionCarpetas> history=new List<IntentoImportacionCarpetas>();
        public LoteImportacionCarpetas Lot;public ArchivoImportacionCarpetas File;public IntentoImportacionCarpetas Attempt;
        public bool FailBeforeCommit,FailAfterCommit;public int Reservations,Commits,Events,FailureCalls;
        void Owner(ContextoImportacionCarpetas c){if(c==null||!c.PuedeImportar||c.UsuarioRef!=Lot.Contexto.UsuarioRef||c.AmbitoRef!=Lot.Contexto.AmbitoRef||c.NivelRef!=Lot.Contexto.NivelRef||c.SelloDestino!=Lot.Contexto.SelloDestino)throw new PerfilImportacionException("ACCESO_DENEGADO","Fixture");}
        void Resource(string lot,string file){if(lot!=Lot.LoteId||file!=File.ArchivoId)throw new PerfilImportacionException("ACCESO_DENEGADO","Fixture");}
        public ContextoImportacionCarpetas ObtenerContextoLote(ContextoImportacionCarpetas c,string id){lock(sync){if(id!=Lot.LoteId||c==null||!c.PuedeImportar||c.UsuarioRef!=Lot.Contexto.UsuarioRef||c.AmbitoRef!=Lot.Contexto.AmbitoRef)throw new PerfilImportacionException("ACCESO_DENEGADO","Fixture");return ReceiptClone(Lot.Contexto);}}
        public LoteImportacionCarpetas ObtenerLote(ContextoImportacionCarpetas c,string id){lock(sync){Owner(c);if(id!=Lot.LoteId)throw new PerfilImportacionException("ACCESO_DENEGADO","Fixture");return ReceiptClone(Lot);}}
        public ArchivoImportacionCarpetas ObtenerArchivo(ContextoImportacionCarpetas c,string lot,string file){lock(sync){Owner(c);Resource(lot,file);return ReceiptClone(File);}}
        public IntentoImportacionCarpetas ReservarIntento(ContextoImportacionCarpetas c,ReclamacionImportacionCarpetas request){lock(sync){
            Owner(c);Resource(request.LoteId,request.RecursoId);RepositoryHelper("ValidarSolicitudRecepcion",request);
            var prior=history.LastOrDefault(i=>i.SolicitudHash==request.SolicitudHash);if(prior!=null)return ReceiptClone(prior);
            RepositoryHelper("ValidarReservaRecepcion",Lot,File,request);Reservations++;
            Attempt=new IntentoImportacionCarpetas{IntentoId=(49+Reservations).ToString(),LoteId=Lot.LoteId,RaizId=File.RaizId,ArchivoId=File.ArchivoId,Fase="RECEPCION",Estado="EN_CURSO",TokenReclamacion=request.TokenReclamacion,SolicitudHash=request.SolicitudHash,EjecutorRef=request.EjecutorRef,InicioUtc="2000-01-01T00:00:00Z"};history.Add(Attempt);
            File.EstadoRecepcion="RECIBIENDO";File.IntentoActivoId=Attempt.IntentoId;File.Revision++;Lot.Revision++;Events++;return ReceiptClone(Attempt);
        }}
        public IntentoImportacionCarpetas ObtenerIntentoRecepcion(ContextoImportacionCarpetas c,string lot,string file,string attempt){lock(sync){Owner(c);Resource(lot,file);var item=attempt==null?history.LastOrDefault(i=>i.Estado=="CONFIRMADO"):history.SingleOrDefault(i=>i.IntentoId==attempt);if(item==null)throw new PerfilImportacionException("CONFLICTO_REVISION","Fixture");return ReceiptClone(item);}}
        public ArchivoImportacionCarpetas ConfirmarRecepcion(ContextoImportacionCarpetas c,string lot,string file,string attempt,string token,RecepcionVerificadaImportacion receipt){lock(sync){
            Owner(c);Resource(lot,file);var item=history.Single(i=>i.IntentoId==attempt);RepositoryHelper("ValidarConfirmacionRecepcion",File,item,lot,token,receipt);
            if(FailBeforeCommit)throw new PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE","Fixture metadata failure");
            if(File.EstadoRecepcion=="RECIBIDO")return ReceiptClone(File);
            File.EstadoRecepcion="RECIBIDO";File.BytesRecibidos=receipt.BytesRecibidos;File.ContenidoHash=receipt.ContenidoHash;File.StagingRef=receipt.StagingRef;File.IntentoActivoId=null;File.Revision++;
            item.Estado="CONFIRMADO";item.Resultado=new ResultadoEfectoImportacion{Estado="CONFIRMADO",Codigo="ARCHIVO_RECIBIDO",Recepcion=ReceiptClone(receipt)};Commits++;Events++;
            if(FailAfterCommit)throw new PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE","Fixture lost commit response");return ReceiptClone(File);
        }}
        public void RegistrarFalloRecepcion(ContextoImportacionCarpetas c,string lot,string file,string attempt,string token,string code){lock(sync){
            Owner(c);Resource(lot,file);FailureCalls++;var item=history.Single(i=>i.IntentoId==attempt);Check(item.TokenReclamacion==token,"DOC111 failed receipt token changed");
            if(File.EstadoRecepcion=="RECIBIDO"||item.Estado=="CONFIRMADO"||item.Estado=="RECHAZADO")return;
            Check(File.IntentoActivoId==attempt,"DOC111 failed receipt steals another attempt");var known=new[]{"ARCHIVO_RECHAZADO","LIMITE_EXCEDIDO","FORMATO_NO_SOPORTADO","SOLICITUD_INVALIDA","RECEPCION_OCUPADA"}.Contains(code);
            item.Estado=known?"RECHAZADO":"INCIERTO";item.Resultado=new ResultadoEfectoImportacion{Estado=item.Estado,Codigo=code};File.EstadoRecepcion=item.Estado;if(known)File.IntentoActivoId=null;File.Revision++;Events++;
        }}
    }

    sealed class ReceiptFixture : IDisposable {
        public readonly ResourceFixture Resources=new ResourceFixture();public readonly ReceiptRepository Repository=new ReceiptRepository();
        public readonly byte[] Pdf=StagingPdf();public readonly string Root;public readonly LimitesImportacionCarpetas Limits;public readonly ServicioRecepcionImportacion Service;public int StageFactories;
        public readonly RecibirArchivoImportacionSolicitud Request=new RecibirArchivoImportacionSolicitud{loteId="20",archivoId="30",claveIntento="command-1"};
        public ReceiptFixture(){
            Root=Path.Combine(Path.GetTempPath(),"doc111-reception-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(Root);
            var context=Resources.Adapter.ResolverDestino(Resources.Http,"7");var config=Resources.Configuration();config.Extensiones=new List<string>{"pdf"};
            var snapshot=new SnapshotPerfilImportacionOperativa{PerfilId="4",VersionId="5",NumeroVersion=1,Configuracion=config};snapshot.Huella=Hex(PerfilImportacionJson.Huella(new VersionPerfilImportacion{Configuracion=config,Reglas=snapshot.Reglas}));
            Repository.Lot=new LoteImportacionCarpetas{LoteId="20",PlanId="12",Contexto=context,Snapshot=snapshot,Estado="RECIBIENDO",Revision=1,TotalRaices=1,TotalArchivos=1,TotalBytes=Pdf.Length.ToString()};
            Repository.File=new ArchivoImportacionCarpetas{ArchivoId="30",RaizId="40",ClienteArchivoId="f1",NombreOriginal="documento.pdf",RutaRelativa="Folder/documento.pdf",BytesPrevistos=Pdf.Length.ToString(),EstadoRecepcion="PENDIENTE",EstadoEfecto="NO_INICIADO",Revision=1};
            Limits=new LimitesImportacionCarpetas(2,4,(ulong)Pdf.Length*4,(ulong)Pdf.Length*100,16384,TimeSpan.FromMinutes(1),TimeSpan.FromHours(1));
            Service=new ServicioRecepcionImportacion(()=>Resources.Adapter.Resolver(Resources.Http),Repository,level=>Resources.Adapter.ResolverDestino(Resources.Http,level,"RECIBIR_ARCHIVO"),(c,s)=>Resources.Adapter.AutorizarRecursos(Resources.Http,c,s,"RECIBIR_ARCHIVO"),Limits,(l,f)=>{StageFactories++;return new AlmacenStagingImportacion(Root,Limits,l,f,10,200);});
        }
        public string[] Completed(){var folder=Path.Combine(Root,"App_Data");return Directory.Exists(folder)?Directory.GetFiles(folder,"*.bin",SearchOption.AllDirectories):new string[0];}
        public void Dispose(){var full=Path.GetFullPath(Root);Check(Path.GetDirectoryName(full)==Path.GetFullPath(Path.GetTempPath()).TrimEnd(Path.DirectorySeparatorChar) && Path.GetFileName(full).StartsWith("doc111-reception-",StringComparison.Ordinal),"DOC111 reception cleanup outside test root");Directory.Delete(full,true);}
    }

    static void ReceiptServiceCases() {
        using(var f=new ReceiptFixture()) {
            foreach(var change in new[]{"permission","level","cabinet","lot","file","ineligible","stop"}) {
                var request=ReceiptClone(f.Request);if(change=="permission")f.Resources.ImportAllowed=false;if(change=="level")f.Resources.Moved=true;if(change=="cabinet")f.Resources.CabinetAllowed=false;if(change=="lot")request.loteId="999";if(change=="file")request.archivoId="999";if(change=="ineligible")f.Repository.File.EstadoEfecto="RECHAZADO";if(change=="stop")f.Repository.Lot.DetencionSolicitada=true;
                using(var input=new StagingStream(f.Pdf)){Rejected(change=="level"||change=="stop"?"CONFLICTO_REVISION":"ACCESO_DENEGADO",()=>f.Service.Recibir(request,input));Check(input.BytesRead==0 && f.StageFactories==0,"DOC111 unauthorized receipt reaches bytes or staging");}
                f.Resources.ImportAllowed=true;f.Resources.Moved=false;f.Resources.CabinetAllowed=true;f.Repository.File.EstadoEfecto="NO_INICIADO";f.Repository.Lot.DetencionSolicitada=false;
            }
            RecepcionArchivoImportacionResultado result;using(var input=new StagingStream(f.Pdf))result=f.Service.Recibir(f.Request,input);
            Check(result.estadoRecepcion=="RECIBIDO" && result.estadoEfecto=="NO_INICIADO" && result.bytesRecibidos==f.Pdf.Length.ToString() && f.Repository.Reservations==1 && f.Repository.Commits==1 && f.Repository.Events==2,"DOC111 receipt conflates content with documentary completion");
            var count=f.Completed().Length;using(var input=new StagingStream(f.Pdf)){var replay=f.Service.Recibir(f.Request,input);Check(replay.contenidoHash==result.contenidoHash && f.Repository.Reservations==1 && f.Repository.Commits==1 && f.Completed().Length==count,"DOC111 replay duplicates reception");}
            var changed=(byte[])f.Pdf.Clone();changed[20]^=1;using(var input=new StagingStream(changed))Rejected("CONTENIDO_DISTINTO",()=>f.Service.Recibir(f.Request,input));
            Check(f.Repository.File.ContenidoHash==result.contenidoHash && f.Repository.File.EstadoRecepcion=="RECIBIDO" && f.Repository.FailureCalls==0,"DOC111 incompatible replay mutates original evidence");
            var complete=f.Completed().Single();File.Move(complete,complete+".lost");
            Rejected("RESULTADO_INCIERTO",()=>f.Service.ReconciliarRecepcion(f.Request));using(var input=new StagingStream(f.Pdf)){Rejected("RESULTADO_INCIERTO",()=>f.Service.Recibir(f.Request,input));Check(input.BytesRead==0,"DOC111 missing staging authorizes replay");}File.Move(complete+".lost",complete);
            Check(f.Service.ReconciliarRecepcion(f.Request).contenidoHash==result.contenidoHash,"DOC111 verified receipt not queryable");
        }
        foreach(var after in new[]{false,true})using(var f=new ReceiptFixture()) {
            f.Repository.FailBeforeCommit=!after;f.Repository.FailAfterCommit=after;
            using(var input=new StagingStream(f.Pdf))Rejected("RESULTADO_INCIERTO",()=>f.Service.Recibir(f.Request,input));
            Check(f.Completed().Length==1 && f.Repository.File.EstadoRecepcion==(after?"RECIBIDO":"INCIERTO"),"DOC111 metadata failure loses complete or downgrades successful commit");
            f.Repository.FailBeforeCommit=false;f.Repository.FailAfterCommit=false;
            var restored=f.Service.ReconciliarRecepcion(f.Request);Check(restored.estadoRecepcion=="RECIBIDO" && f.Repository.Reservations==1 && f.Repository.Commits==1 && f.Completed().Length==1,"DOC111 reconciliation duplicates reception after response failure");
        }
        using(var f=new ReceiptFixture()) {
            using(var input=new StagingStream(f.Pdf.Take(f.Pdf.Length-1).ToArray()))Rejected("ARCHIVO_RECHAZADO",()=>f.Service.Recibir(f.Request,input));
            Check(f.Repository.File.EstadoRecepcion=="RECHAZADO" && f.Repository.File.EstadoEfecto=="NO_INICIADO" && f.Completed().Length==0,"DOC111 rejected content marked complete");
            using(var input=new StagingStream(f.Pdf)){Rejected("ARCHIVO_RECHAZADO",()=>f.Service.Recibir(f.Request,input));Check(input.BytesRead==0 && f.Repository.Reservations==1,"DOC111 terminal attempt blindly reused");}
            var retry=ReceiptClone(f.Request);retry.claveIntento="command-2";using(var input=new StagingStream(f.Pdf))Check(f.Service.Recibir(retry,input).estadoRecepcion=="RECIBIDO","DOC111 new explicit attempt cannot recover rejected receipt");
        }
        using(var f=new ReceiptFixture()) {
            using(var input=new StagingStream(f.Pdf){OnRead=()=>f.Resources.ImportAllowed=false})Rejected("ACCESO_DENEGADO",()=>f.Service.Recibir(f.Request,input));
            Check(f.Repository.Commits==0 && f.Repository.File.EstadoRecepcion=="INCIERTO" && f.Completed().Length==1,"DOC111 revoked authority confirms reception or drops retained evidence");
            f.Resources.ImportAllowed=true;Check(f.Service.ReconciliarRecepcion(f.Request).estadoRecepcion=="RECIBIDO","DOC111 authorized recovery cannot verify complete");
        }
        using(var f=new ReceiptFixture()) {
            var c=f.Repository.Lot.Contexto;var directory=Path.Combine(f.Root,"App_Data","ImportacionCarpetas",c.SelloDestino,c.AmbitoRef,c.UsuarioRef,"20");Directory.CreateDirectory(directory);
            using(var held=new FileStream(Path.Combine(directory,"recepcion.lock"),FileMode.OpenOrCreate,FileAccess.ReadWrite,FileShare.None))using(var input=new StagingStream(f.Pdf)){Rejected("RECEPCION_OCUPADA",()=>f.Service.Recibir(f.Request,input));Check(input.BytesRead==0 && f.Repository.File.EstadoRecepcion=="RECHAZADO" && f.Repository.File.IntentoActivoId==null,"DOC111 busy store leaves an uncertain writer despite proving no write started");}
            var next=ReceiptClone(f.Request);next.claveIntento="explicit-after-busy";using(var input=new StagingStream(f.Pdf))Check(f.Service.Recibir(next,input).estadoRecepcion=="RECIBIDO","DOC111 explicit retry cannot recover pre-write contention");
        }
        using(var f=new ReceiptFixture())using(var entered=new System.Threading.ManualResetEventSlim(false))using(var release=new System.Threading.ManualResetEventSlim(false)) {
            var first=System.Threading.Tasks.Task.Factory.StartNew(()=>{using(var input=new StagingStream(f.Pdf){OnRead=()=>{entered.Set();if(!release.Wait(TimeSpan.FromSeconds(10)))throw new IOException("Fixture gate timeout");}})return f.Service.Recibir(f.Request,input);});
            try {Check(entered.Wait(TimeSpan.FromSeconds(5)),"DOC111 concurrent receipt never reached stream");using(var second=new StagingStream(f.Pdf)){Rejected("OPERACION_EN_CURSO",()=>f.Service.Recibir(f.Request,second));Check(second.BytesRead==0 && f.Repository.Reservations==1 && f.Repository.File.EstadoRecepcion=="RECIBIENDO","DOC111 second executor writes or changes active attempt");}}
            finally {release.Set();}
            Check(first.Wait(TimeSpan.FromSeconds(10)) && first.Result.estadoRecepcion=="RECIBIDO" && f.Repository.Commits==1,"DOC111 original executor did not finish exactly once");
        }
        // DOC-115: tampering after a committed receipt preserves both evidence
        // and counters. A valid-size file is not sufficient to authorize replay.
        using(var f=new ReceiptFixture()) {
            RecepcionArchivoImportacionResultado prior;using(var input=new StagingStream(f.Pdf))prior=f.Service.Recibir(f.Request,input);
            var complete=f.Completed().Single();var changed=(byte[])f.Pdf.Clone();changed[20]^=1;File.WriteAllBytes(complete,changed);
            using(var input=new StagingStream(f.Pdf)){Rejected("RESULTADO_INCIERTO",()=>f.Service.Recibir(f.Request,input));Check(input.BytesRead==0,"DOC115 changed retained content consumes replay before verification");}
            Check(f.Repository.Commits==1 && f.Repository.Reservations==1 && f.Repository.File.ContenidoHash==prior.contenidoHash && f.Repository.File.EstadoEfecto=="NO_INICIADO","DOC115 altered retention overwrites confirmed row or starts document effects");
            File.WriteAllBytes(complete,f.Pdf);
        }
        // Two different files compete for one injected finite quota. The first
        // stream holds the real file lock; the second must not read any bytes.
        using(var f=new ReceiptFixture())using(var entered=new System.Threading.ManualResetEventSlim(false))using(var release=new System.Threading.ManualResetEventSlim(false)) {
            var quota=new LimitesImportacionCarpetas(2,4,(ulong)f.Pdf.Length,(ulong)f.Pdf.Length,16384,TimeSpan.FromMinutes(1),TimeSpan.FromHours(1));
            var other=ReceiptClone(f.Repository.File);other.ArchivoId="31";
            var firstAttempt=new IntentoImportacionCarpetas{IntentoId="50",LoteId="20",RaizId="40",ArchivoId="30",Fase="RECEPCION",Estado="EN_CURSO",TokenReclamacion=Guid.NewGuid().ToString("N")};
            var secondAttempt=ReceiptClone(firstAttempt);secondAttempt.IntentoId="51";secondAttempt.ArchivoId="31";secondAttempt.TokenReclamacion=Guid.NewGuid().ToString("N");
            var firstStore=new AlmacenStagingImportacion(f.Root,quota,f.Repository.Lot,f.Repository.File,10,200);
            var secondStore=new AlmacenStagingImportacion(f.Root,quota,f.Repository.Lot,other,10,200);
            var first=System.Threading.Tasks.Task.Factory.StartNew(()=>{using(var input=new StagingStream(f.Pdf){OnRead=()=>{entered.Set();if(!release.Wait(TimeSpan.FromSeconds(10)))throw new IOException("Fixture gate timeout");}})return firstStore.Recibir(f.Repository.Lot.Contexto,firstAttempt,input,f.Repository.File.BytesPrevistos);});
            try{Check(entered.Wait(TimeSpan.FromSeconds(5)),"DOC115 quota writer never entered");using(var input=new StagingStream(f.Pdf)){Rejected("RECEPCION_OCUPADA",()=>secondStore.Recibir(f.Repository.Lot.Contexto,secondAttempt,input,other.BytesPrevistos));Check(input.BytesRead==0,"DOC115 concurrent quota competitor consumed bytes");}}finally{release.Set();}
            Check(first.Wait(TimeSpan.FromSeconds(10)),"DOC115 quota writer did not complete");
            using(var input=new StagingStream(f.Pdf)){Rejected("LIMITE_EXCEDIDO",()=>secondStore.Recibir(f.Repository.Lot.Contexto,secondAttempt,input,other.BytesPrevistos));Check(input.BytesRead==0,"DOC115 completed retention not counted toward finite quota");}
            using(var input=new StagingStream(f.Pdf)){Check(firstStore.VerificarReenvio(f.Repository.Lot.Contexto,firstAttempt,input).ContenidoHash==first.Result.ContenidoHash,"DOC115 replay changes retained content");}
            Check(f.Completed().Length==1 && new FileInfo(f.Completed().Single()).Length==f.Pdf.Length,"DOC115 quota/replay duplicated retained bytes");
        }
        using(var f=new ReceiptFixture()) {
            var appData=Path.Combine(f.Root,"App_Data");Directory.CreateDirectory(appData);
            var link=Path.Combine(appData,"ImportacionCarpetas");var outside=Path.Combine(f.Root,"junction-target");Directory.CreateDirectory(outside);
            var start=new System.Diagnostics.ProcessStartInfo("cmd.exe","/c mklink /J \""+link+"\" \""+outside+"\""){UseShellExecute=false,CreateNoWindow=true,RedirectStandardOutput=true,RedirectStandardError=true};
            try{
                using(var process=System.Diagnostics.Process.Start(start)){process.StandardOutput.ReadToEnd();process.StandardError.ReadToEnd();Check(process.WaitForExit(5000)&&process.ExitCode==0,"DOC115 local junction fixture unavailable");}
                using(var input=new StagingStream(f.Pdf)){Rejected("CAPACIDAD_NO_DISPONIBLE",()=>f.Service.Recibir(f.Request,input));Check(input.BytesRead==0 && Directory.GetFiles(outside,"*",SearchOption.AllDirectories).Length==0,"DOC115 junction received bytes outside controlled staging");}
            }finally{
                // Remove only this verified link, without recursion or its target.
                Check(Path.GetFullPath(link)==Path.Combine(Path.GetFullPath(f.Root),"App_Data","ImportacionCarpetas"),"DOC115 junction cleanup outside fixture");
                if(Directory.Exists(link))Directory.Delete(link,false);
            }
        }
        Console.WriteLine("DOC-115 altered retention and concurrent finite quota (real disk, persistence double): passed");
        Console.WriteLine("DOC-111 receipt ownership, replay, recovery and concurrency: passed");
    }

    static byte[] ReceiptMultipart(byte[] pdf,bool extra=false,string extraField="usuarioRef") {
        using(var body=new MemoryStream()) {
            Action<string> append=s=>{var bytes=System.Text.Encoding.UTF8.GetBytes(s);body.Write(bytes,0,bytes.Length);};
            foreach(var item in new[]{new[]{"loteId","20"},new[]{"archivoId","30"},new[]{"claveIntento","command-1"}})append("--doc111-receipt\r\nContent-Disposition: form-data; name=\""+item[0]+"\"\r\n\r\n"+item[1]+"\r\n");
            if(extra)append("--doc111-receipt\r\nContent-Disposition: form-data; name=\""+extraField+"\"\r\n\r\n99\r\n");
            append("--doc111-receipt\r\nContent-Disposition: form-data; name=\"file\"; filename=\"client-name.html\"\r\nContent-Type: text/html\r\n\r\n");body.Write(pdf,0,pdf.Length);append("\r\n--doc111-receipt--\r\n");return body.ToArray();
        }
    }

    static void ReceiptHandlerCases() {
        var previous=HttpContext.Current;
        try {
            using(var f=new ReceiptFixture()) {
                var factories=0;
                Func<HttpContext,ImportacionCarpetasContextAdapter,LimitesImportacionCarpetas,ServicioRecepcionImportacion> factory=(h,a,l)=>{factories++;return f.Service;};
                var constructor=typeof(ImportacionCarpetasUpload).GetConstructors(BindingFlags.NonPublic|BindingFlags.Instance).Single();
                var handler=(ImportacionCarpetasUpload)constructor.Invoke(new object[]{f.Resources.Adapter,(Func<LimitesImportacionCarpetas>)(()=>f.Limits),factory});
                Check(!handler.IsReusable && handler is IRequiresSessionState,"DOC111 multipart handler does not use approved session lifecycle");
                foreach(var scenario in new[]{"token","origin","permission","session","extra","gabineteId","expedienteId","nivelRef","estadoRecepcion"}) {
                    var extra=new[]{"extra","gabineteId","expedienteId","nivelRef","estadoRecepcion"}.Contains(scenario);
                    var worker=new ImportRequest{Body=ReceiptMultipart(f.Pdf,extra,scenario=="extra"?"usuarioRef":scenario),Mime="multipart/form-data; boundary=doc111-receipt"};var http=SessionFixture(worker:worker);worker.Token=ImportacionCarpetasRequestGuard.Emitir(http,RequestContext());
                    if(scenario=="token")worker.Token=null;if(scenario=="origin")worker.Origin="https://foreign.invalid";if(scenario=="permission")f.Resources.ImportAllowed=false;if(scenario=="session")http.Session.Clear();
                    HttpContext.Current=http;handler.ProcessRequest(http);http.Response.Flush();
                    Check(http.Response.StatusCode==(scenario=="session"?401:extra?400:403) && factories==0,"DOC111 rejected multipart reaches service or wrong status");
                    if(!extra)Check(worker.BodyReads==0,"DOC111 rejected authority/token/origin reads multipart body");
                    Check(!worker.ResponseText.Contains("App_Data") && !worker.ResponseText.Contains("tokenReclamacion"),"DOC111 multipart error exposes private details");f.Resources.ImportAllowed=true;
                }
                var accepted=new ImportRequest{Body=ReceiptMultipart(f.Pdf),Mime="multipart/form-data; boundary=doc111-receipt"};var ok=SessionFixture(worker:accepted);accepted.Token=ImportacionCarpetasRequestGuard.Emitir(ok,RequestContext());HttpContext.Current=ok;
                handler.ProcessRequest(ok);ok.Response.Flush();
                var result=JsonConvert.DeserializeObject<RespuestaPerfilImportacion>(accepted.ResponseText);
                Check(ok.Response.StatusCode==200 && ok.Response.ContentType=="application/json" && ok.Response.SuppressFormsAuthenticationRedirect && result.ok && accepted.ResponseText.Contains("\"estadoRecepcion\":\"RECIBIDO\"") && accepted.ResponseText.Contains("\"estadoEfecto\":\"NO_INICIADO\""),"DOC111 receipt handler lost structured JSON semantics");
                Check(factories==1 && f.Repository.Commits==1 && !accepted.ResponseText.Contains("stagingRef") && !accepted.ResponseText.Contains("tokenReclamacion") && !accepted.ResponseText.Contains("App_Data") && !accepted.ResponseText.Contains(accepted.Token),"DOC111 successful multipart leaks internal storage or auth data");
                var factoryMethod=typeof(ImportacionCarpetasUpload).GetMethod("CrearServicio",BindingFlags.NonPublic|BindingFlags.Static);
                try{factoryMethod.Invoke(null,new object[]{ok,f.Resources.Adapter,f.Limits});throw new Exception("DOC111 missing PDF budget accepted");}
                catch(TargetInvocationException e){Check(e.InnerException is PerfilImportacionException && ((PerfilImportacionException)e.InnerException).Codigo=="CAPACIDAD_NO_DISPONIBLE","DOC111 unconfigured receiver does not fail closed");}
            }
        } finally {HttpContext.Current=previous;}
        Console.WriteLine("DOC-111 multipart handler guards and private JSON: passed");
    }

    static void ReceiptRecordCases() {
        var source=new ArchivoManifiestoImportacion{IdCliente="f1",RaizIdCliente="r1",Nombre="doc.pdf",RutaRelativa="Folder/doc.pdf",Bytes="7",UltimaModificacionLocal="0"};
        var data=new DataTable();foreach(var column in new[]{"archivo_id","raiz_id","cliente_archivo_id","origen_doc","datos_raiz","ruta_hash","bytes_previstos","estado_recepcion","estado_efecto","revision","bytes_recibidos","contenido_hash","staging_ref","intento_activo_id","plan_valores_doc","valores_efectivos_doc","resultado_doc","cliente_raiz"})data.Columns.Add(column,typeof(object));
        byte[] pathHash;using(var sha=System.Security.Cryptography.SHA256.Create())pathHash=sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(source.RutaRelativa));
        data.Rows.Add(30u,40u,"f1",PerfilImportacionJson.Escribir(source),PerfilImportacionJson.Escribir(new{Esquema=1,SeleccionId="s1",IdCliente="r1",NombreOriginal="Folder",ValoresPlan=new List<ValorReglaPerfilImportacion>()}),pathHash,7UL,"PENDIENTE","NO_INICIADO",1u,DBNull.Value,DBNull.Value,DBNull.Value,DBNull.Value,"[]",DBNull.Value,DBNull.Value,"r1");
        Func<ArchivoImportacionCarpetas> read=()=>{using(var reader=data.CreateDataReader()){reader.Read();return (ArchivoImportacionCarpetas)RepositoryHelper("LeerArchivoRecepcion",reader);}};
        Check(read().BytesPrevistos=="7" && read().NombreOriginal=="doc.pdf","DOC111 durable expected file not read");
        Check(!read().ValoresEfectivosFijados,"DOC111 SQL NULL marked as fixed values");data.Rows[0]["valores_efectivos_doc"]="[]";
        Check(read().ValoresEfectivosFijados&&read().ValoresEfectivos.Count==0,"DOC111 fixed empty collection confused with SQL NULL");data.Rows[0]["valores_efectivos_doc"]=DBNull.Value;
        data.Rows[0]["cliente_raiz"]="foreign";Unavailable(()=>read());data.Rows[0]["cliente_raiz"]="r1";
        data.Rows[0]["bytes_previstos"]=8UL;Unavailable(()=>read());data.Rows[0]["bytes_previstos"]=7UL;
        data.Rows[0]["ruta_hash"]=new byte[32];Unavailable(()=>read());
        var request=new ReclamacionImportacionCarpetas{LoteId="20",RecursoTipo="ARCHIVO",RecursoId="30",Fase="RECEPCION",RevisionEsperada=1,EjecutorRef="WEB_fixture",TokenReclamacion=new string('a',32),SolicitudHash=new string('b',64)};
        var lot=new LoteImportacionCarpetas{LoteId="20",Estado="RECIBIENDO"};var file=new ArchivoImportacionCarpetas{ArchivoId="30",RaizId="40",Revision=1,EstadoRecepcion="PENDIENTE",EstadoEfecto="NO_INICIADO",BytesPrevistos="7"};
        RepositoryHelper("ValidarReservaRecepcion",lot,file,request);
        file.EstadoRecepcion="INCIERTO";file.IntentoActivoId="50";Rejected("OPERACION_EN_CURSO",()=>RepositoryHelper("ValidarReservaRecepcion",lot,file,request));file.EstadoRecepcion="PENDIENTE";file.IntentoActivoId=null;
        lot.DetencionSolicitada=true;Rejected("CONFLICTO_REVISION",()=>RepositoryHelper("ValidarReservaRecepcion",lot,file,request));lot.DetencionSolicitada=false;
        file.EstadoEfecto="COMPLETADO";Rejected("CONFLICTO_REVISION",()=>RepositoryHelper("ValidarReservaRecepcion",lot,file,request));file.EstadoEfecto="NO_INICIADO";
        var attempt=new IntentoImportacionCarpetas{IntentoId="50",LoteId="20",RaizId="40",ArchivoId="30",Fase="RECEPCION",Estado="EN_CURSO",TokenReclamacion=request.TokenReclamacion};
        var proof=new RecepcionVerificadaImportacion{StagingRef=new string('c',32),ContenidoHash=new string('d',64),BytesRecibidos="7"};file.EstadoRecepcion="RECIBIENDO";file.IntentoActivoId="50";
        RepositoryHelper("ValidarConfirmacionRecepcion",file,attempt,"20",request.TokenReclamacion,proof);
        proof.BytesRecibidos="8";Rejected("SOLICITUD_INVALIDA",()=>RepositoryHelper("ValidarConfirmacionRecepcion",file,attempt,"20",request.TokenReclamacion,proof));proof.BytesRecibidos="7";
        Rejected("CONFLICTO_REVISION",()=>RepositoryHelper("ValidarConfirmacionRecepcion",file,attempt,"20",new string('e',32),proof));
        file.EstadoRecepcion="RECIBIDO";file.IntentoActivoId=null;file.StagingRef=proof.StagingRef;file.BytesRecibidos=proof.BytesRecibidos;file.ContenidoHash=proof.ContenidoHash;attempt.Estado="CONFIRMADO";attempt.Resultado=new ResultadoEfectoImportacion{Estado="CONFIRMADO",Recepcion=ReceiptClone(proof)};
        RepositoryHelper("ValidarConfirmacionRecepcion",file,attempt,"20",request.TokenReclamacion,proof);proof.ContenidoHash=new string('e',64);Rejected("CONFLICTO_REVISION",()=>RepositoryHelper("ValidarConfirmacionRecepcion",file,attempt,"20",request.TokenReclamacion,proof));
        Console.WriteLine("DOC-111 durable expected file record guards: passed");
    }

    // Doble local: bloquea en memoria y ejecuta los guards productivos. No
    // acredita bloqueos/rollback MySQL ni efectos legacy.
    sealed class ControlRepository : IControlLoteImportacionRepository {
        readonly object sync=new object();readonly List<IntentoImportacionCarpetas> attempts=new List<IntentoImportacionCarpetas>();
        public LoteImportacionCarpetas Lot=new LoteImportacionCarpetas{LoteId="20",Contexto=PlanFixture().Contexto,Snapshot=PlanFixture().Snapshot,Estado="RECIBIENDO",Revision=1,TotalRaices=2,TotalArchivos=3,TotalBytes="21"};
        public RaizImportacionCarpetas Root=new RaizImportacionCarpetas{LoteId="20",RaizId="40",ClienteRaizId="r1",Estado="PENDIENTE",Revision=1};
        public ArchivoImportacionCarpetas File=new ArchivoImportacionCarpetas{RaizId="40",ArchivoId="30",EstadoRecepcion="RECIBIDO",EstadoEfecto="NO_INICIADO",Revision=1,BytesPrevistos="7",BytesRecibidos="7",ContenidoHash=new string('a',64),StagingRef=new string('b',32)};
        public int Reservations,Stops;public bool FailAfterCommit;
        void Owner(ContextoImportacionCarpetas c,string id){if(c==null||!c.PuedeImportar||id!=Lot.LoteId||c.UsuarioRef!=Lot.Contexto.UsuarioRef||c.AmbitoRef!=Lot.Contexto.AmbitoRef||c.NivelRef!=Lot.Contexto.NivelRef||c.SelloDestino!=Lot.Contexto.SelloDestino)throw new PerfilImportacionException("ACCESO_DENEGADO","Fixture");}
        public ContextoImportacionCarpetas ObtenerContextoLote(ContextoImportacionCarpetas c,string id){lock(sync){if(c==null||!c.PuedeImportar||id!=Lot.LoteId||c.UsuarioRef!=Lot.Contexto.UsuarioRef||c.AmbitoRef!=Lot.Contexto.AmbitoRef)throw new PerfilImportacionException("ACCESO_DENEGADO","Fixture");return ReceiptClone(Lot.Contexto);}}
        public LoteImportacionCarpetas ObtenerLote(ContextoImportacionCarpetas c,string id){lock(sync){Owner(c,id);return ReceiptClone(Lot);}}
        public IntentoImportacionCarpetas ReservarIntento(ContextoImportacionCarpetas c,ReclamacionImportacionCarpetas command){lock(sync){
            Owner(c,command.LoteId);RepositoryHelper("ValidarSolicitudDocumental",command);
            var previous=attempts.FirstOrDefault(i=>i.Fase==command.Fase&&i.SolicitudHash==command.SolicitudHash&&((command.RecursoTipo=="RAIZ"&&i.RaizId==command.RecursoId)||(command.RecursoTipo=="ARCHIVO"&&i.ArchivoId==command.RecursoId)));if(previous!=null)return ReceiptClone(previous);
            RepositoryHelper("ValidarReservaDocumental",Lot,Root,command.RecursoTipo=="ARCHIVO"?File:null,command,attempts.Any(i=>new[]{"RESERVADO","EN_CURSO","INCIERTO"}.Contains(i.Estado)));
            var attempt=new IntentoImportacionCarpetas{IntentoId=(50+Reservations).ToString(),LoteId=Lot.LoteId,RaizId=Root.RaizId,ArchivoId=command.RecursoTipo=="ARCHIVO"?File.ArchivoId:null,Fase=command.Fase,Estado="RESERVADO",TokenReclamacion=command.TokenReclamacion,SolicitudHash=command.SolicitudHash,InicioUtc="2000-01-01T00:00:00Z"};attempts.Add(attempt);Reservations++;
            if(command.RecursoTipo=="RAIZ"){Root.Estado="CREANDO";Root.IntentoActivoId=attempt.IntentoId;Root.Revision++;}else{File.EstadoEfecto="EN_CURSO";File.IntentoActivoId=attempt.IntentoId;File.Revision++;}Lot.Estado="EN_CURSO";Lot.Revision++;
            if(FailAfterCommit)throw new PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE","Fixture lost response");var reserved=ReceiptClone(attempt);reserved.EsNuevaReserva=true;return reserved;
        }}
        public LoteImportacionCarpetas SolicitarDetencion(ContextoImportacionCarpetas c,string id,uint revision){lock(sync){Owner(c,id);RepositoryHelper("ValidarDetencion",Lot,revision);if(!Lot.DetencionSolicitada&&!new[]{"COMPLETADO","FALLIDO"}.Contains(Lot.Estado)){Lot.DetencionSolicitada=true;Lot.Revision++;Stops++;}return ReceiptClone(Lot);}}
    }

    static ReclamacionImportacionCarpetas ControlCommand(string phase="CREAR_EXPEDIENTE") {
        return new ReclamacionImportacionCarpetas{LoteId="20",RecursoTipo=phase=="CREAR_EXPEDIENTE"?"RAIZ":"ARCHIVO",RecursoId=phase=="CREAR_EXPEDIENTE"?"40":"30",RevisionEsperada=1,Fase=phase,TokenReclamacion=Guid.NewGuid().ToString("N"),SolicitudHash=new string('c',64),EjecutorRef="WEB_fixture"};
    }
    static void ControlCases() {
        var rows=new DataTable();foreach(var name in new[]{"raiz_id","lote_id","cliente_raiz_id","datos_doc","estado","revision","intento_activo_id","identidad_expediente_doc","valores_efectivos_doc"})rows.Columns.Add(name,typeof(object));
        rows.Rows.Add("40","20","r1",PerfilImportacionJson.Escribir(new{Esquema=1,IdCliente="r1",SeleccionId="s1",NombreOriginal="Órdenes",ValoresPlan=new List<ValorReglaPerfilImportacion>()}),"PENDIENTE",1,DBNull.Value,DBNull.Value,DBNull.Value);
        Func<RaizImportacionCarpetas> read=()=>{using(var reader=rows.CreateDataReader()){reader.Read();return (RaizImportacionCarpetas)RepositoryHelper("LeerRaizControl",reader);}};
        Check(read().NombreOriginal=="Órdenes"&&read().LoteId=="20","DOC111 root projection lost frozen identity");rows.Rows[0]["cliente_raiz_id"]="R1";Rejected("CAPACIDAD_NO_DISPONIBLE",()=>read());rows.Rows[0]["cliente_raiz_id"]="r1";
        var r=new ControlRepository();var command=ControlCommand();
        Action validate=()=>RepositoryHelper("ValidarReservaDocumental",r.Lot,r.Root,null,command,false);
        validate();command.RevisionEsperada=2;Rejected("CONFLICTO_REVISION",validate);command.RevisionEsperada=1;
        r.Root.LoteId="21";Rejected("CONFLICTO_REVISION",validate);r.Root.LoteId="20";
        foreach(var state in new[]{"BLOQUEADO","COMPLETADA","CANCELADA","EXPEDIENTE_CONFIRMADO"}){r.Root.Estado=state;Rejected("CONFLICTO_REVISION",validate);}r.Root.Estado="PENDIENTE";
        foreach(var state in new[]{"CREANDO","INCIERTO"}){r.Root.Estado=state;Rejected("OPERACION_EN_CURSO",validate);}r.Root.Estado="PENDIENTE";
        Rejected("OPERACION_EN_CURSO",()=>RepositoryHelper("ValidarReservaDocumental",r.Lot,r.Root,null,command,true));
        foreach(var state in new[]{"DETENIDO","PENDIENTE_VERIFICACION","COMPLETADO","FALLIDO"}){r.Lot.Estado=state;Rejected("CONFLICTO_REVISION",validate);}r.Lot.Estado="RECIBIENDO";
        r.Lot.DetencionSolicitada=true;Rejected("CONFLICTO_REVISION",validate);r.Lot.DetencionSolicitada=false;
        foreach(var invalid in new[]{"VERIFICAR","crear_expediente","ALMACENAR_DOCUMENTO",""}){command.Fase=invalid;Rejected("SOLICITUD_INVALIDA",validate);}command=ControlCommand();
        command.RecursoTipo="ARCHIVO";Rejected("SOLICITUD_INVALIDA",validate);command=ControlCommand();command.TokenReclamacion="bad";Rejected("SOLICITUD_INVALIDA",validate);

        r=new ControlRepository();command=ControlCommand("ALMACENAR");
        Action storage=()=>RepositoryHelper("ValidarReservaDocumental",r.Lot,r.Root,r.File,command,false);
        Rejected("CONFLICTO_REVISION",storage);r.Root.Estado="EXPEDIENTE_CONFIRMADO";
        var identity=new IdentidadExpedienteImportacion{OrigenLogico="RA",ExpedienteId="60",RelacionNivelId="70",NivelRef=r.Lot.Contexto.NivelRef};r.Root.IdentidadExpediente=identity;
        Rejected("CONFLICTO_REVISION",storage);r.File.ValoresEfectivosFijados=true;storage();
        identity.NivelRef="99";Rejected("CONFLICTO_REVISION",storage);identity.NivelRef=r.Lot.Contexto.NivelRef;
        r.File.EstadoRecepcion="PENDIENTE";Rejected("CONFLICTO_REVISION",storage);r.File.EstadoRecepcion="RECIBIDO";
        r.File.ContenidoHash="bad";Rejected("CONFLICTO_REVISION",storage);r.File.ContenidoHash=new string('a',64);
        r.File.BytesRecibidos="6";Rejected("CONFLICTO_REVISION",storage);r.File.BytesRecibidos="7";
        foreach(var state in new[]{"RECHAZADO","COMPLETADO","CANCELADO","ALMACENADO"}){r.File.EstadoEfecto=state;Rejected("CONFLICTO_REVISION",storage);}
        foreach(var state in new[]{"EN_CURSO","INCIERTO"}){r.File.EstadoEfecto=state;Rejected("OPERACION_EN_CURSO",storage);}
        r.File.EstadoEfecto="NO_INICIADO";r.File.IntentoActivoId="80";Rejected("OPERACION_EN_CURSO",storage);r.File.IntentoActivoId=null;
        command=ControlCommand("INCORPORAR");Rejected("CONFLICTO_REVISION",storage);r.File.EstadoEfecto="ALMACENADO";
        r.File.Resultado=new ResultadoEfectoImportacion{Estado="ParcialConocido",Expediente=ReceiptClone(identity),DocumentoId="90",GabineteId="9",OrigenGabinete="DA"};r.Lot.Snapshot.Configuracion.GabineteId=9;storage();
        r.File.Resultado.GabineteId="10";Rejected("CONFLICTO_REVISION",storage);r.File.Resultado.GabineteId="9";
        r.File.Resultado.Expediente.RelacionNivelId="71";Rejected("CONFLICTO_REVISION",storage);r.File.Resultado.Expediente=ReceiptClone(identity);
        command=ControlCommand("ALMACENAR");Rejected("CONFLICTO_REVISION",storage);

        // Carrera en el doble, usando el mismo guard productivo bajo exclusión.
        r=new ControlRepository();var start=new System.Threading.ManualResetEventSlim(false);int winners=0,blocked=0;
        var workers=Enumerable.Range(0,2).Select(i=>System.Threading.Tasks.Task.Run(()=>{var request=ControlCommand();request.SolicitudHash=new string(i==0?'d':'e',64);start.Wait();try{Check(r.ReservarIntento(r.Lot.Contexto,request).EsNuevaReserva,"DOC111 new claim is not distinguished");System.Threading.Interlocked.Increment(ref winners);}catch(PerfilImportacionException e){if(e.Codigo!="OPERACION_EN_CURSO")throw;System.Threading.Interlocked.Increment(ref blocked);}})).ToArray();start.Set();System.Threading.Tasks.Task.WaitAll(workers);
        Check(winners==1&&blocked==1&&r.Reservations==1,"DOC111 duplicate documentary claim in local fixture");start.Dispose();
        r=new ControlRepository();winners=0;int repeats=0;
        using(var sameStart=new System.Threading.ManualResetEventSlim(false)){
            var sameWorkers=Enumerable.Range(0,2).Select(i=>System.Threading.Tasks.Task.Run(()=>{var request=ControlCommand();sameStart.Wait();var reserved=r.ReservarIntento(r.Lot.Contexto,request);if(reserved.EsNuevaReserva)System.Threading.Interlocked.Increment(ref winners);else System.Threading.Interlocked.Increment(ref repeats);Check(!PerfilImportacionJson.Escribir(reserved).Contains("EsNuevaReserva"),"DOC111 transient acquisition flag persisted");})).ToArray();sameStart.Set();System.Threading.Tasks.Task.WaitAll(sameWorkers);
        }
        Check(winners==1&&repeats==1&&r.Reservations==1,"DOC111 same command grants two new reservations");
        // Respuesta perdida: el comando retorna el token original sin otra reserva.
        r=new ControlRepository();command=ControlCommand();r.FailAfterCommit=true;Rejected("CAPACIDAD_NO_DISPONIBLE",()=>r.ReservarIntento(r.Lot.Contexto,command));r.FailAfterCommit=false;
        var firstToken=command.TokenReclamacion;command.TokenReclamacion=Guid.NewGuid().ToString("N");var replay=r.ReservarIntento(r.Lot.Contexto,command);
        Check(!replay.EsNuevaReserva&&replay.TokenReclamacion==firstToken&&replay.TokenReclamacion!=command.TokenReclamacion&&replay.Estado=="RESERVADO"&&r.Reservations==1,"DOC111 claim replay grants new execution");
        command.TokenReclamacion=firstToken;Check(!r.ReservarIntento(r.Lot.Contexto,command).EsNuevaReserva,"DOC111 same token replay grants a second reservation");
        var next=ControlCommand();next.SolicitudHash=new string('f',64);next.RevisionEsperada=r.Root.Revision;Rejected("OPERACION_EN_CURSO",()=>r.ReservarIntento(r.Lot.Contexto,next));

        // Detención con intento vivo: solo cambia señal/revisión, no evidencia.
        bool allowed=true;var current=ReceiptClone(r.Lot.Contexto);
        var coordinator=new CoordinadorImportacionCarpetas(()=>ReceiptClone(current),r,id=>ReceiptClone(current),(c,config)=>{if(!allowed)throw new PerfilImportacionException("ACCESO_DENEGADO","Fixture revoked");});
        var stop=new DetenerLoteImportacionSolicitud{loteId="20",revisionEsperada=r.Lot.Revision};
        var stopped=coordinator.SolicitarDetencion(stop);Check(stopped.detencionSolicitada&&stopped.estado=="EN_CURSO"&&r.Root.IntentoActivoId==replay.IntentoId&&r.Stops==1&&r.Lot.TotalArchivos==3&&r.Lot.TotalBytes=="21","DOC111 stop changes ongoing effect or totals");
        coordinator.SolicitarDetencion(stop);Check(r.Stops==1,"DOC111 repeated stop mutates twice");
        next=ControlCommand();next.SolicitudHash=new string('f',64);Rejected("CONFLICTO_REVISION",()=>r.ReservarIntento(r.Lot.Contexto,next));
        var json=PerfilImportacionJson.Escribir(stopped);Check(!json.Contains("token")&&!json.Contains("IntentoActivo")&&!json.Contains("staging"),"DOC111 stop exposes internal claim");
        allowed=false;Rejected("ACCESO_DENEGADO",()=>coordinator.SolicitarDetencion(stop));allowed=true;
        current.UsuarioRef="99";Rejected("ACCESO_DENEGADO",()=>coordinator.SolicitarDetencion(stop));current=ReceiptClone(r.Lot.Contexto);
        current.SelloDestino="moved";Rejected("ACCESO_DENEGADO",()=>coordinator.SolicitarDetencion(stop));current=ReceiptClone(r.Lot.Contexto);
        r=new ControlRepository();Rejected("CONFLICTO_REVISION",()=>r.SolicitarDetencion(r.Lot.Contexto,"20",2));Check(!r.Lot.DetencionSolicitada&&r.Stops==0,"DOC111 stale stop changed lot");
        foreach(var terminal in new[]{"COMPLETADO","FALLIDO"}){r.Lot.Estado=terminal;r.SolicitarDetencion(r.Lot.Contexto,"20",1);Check(!r.Lot.DetencionSolicitada&&r.Stops==0,"DOC111 stop rewrites terminal lot");}
        Console.WriteLine("DOC-111 documentary claim guards and stop signal (local persistence double): passed");
    }

    // Doble de SOLO lectura. No contiene metodos de mutacion o efectos.
    sealed class QueryRepository : IConsultaImportacionRepository {
        public bool HasConfirmation=true;public int ConfirmationReads;
        public ContextoImportacionCarpetas ObtenerContextoPlan(ContextoImportacionCarpetas c,string id){if(id!="17")throw new PerfilImportacionException("ACCESO_DENEGADO","Fixture");Owner(c,Lot.LoteId);return ReceiptClone(Lot.Contexto);}
        public ConfirmacionLecturaImportacion ConsultarConfirmacion(ContextoImportacionCarpetas c,string id){ObtenerContextoPlan(c,id);ConfirmationReads++;var plan=PlanFixture();plan.PlanId="17";plan.Contexto=ReceiptClone(Lot.Contexto);plan.Snapshot=ReceiptClone(Lot.Snapshot);plan.Estado=HasConfirmation?"CONFIRMADO":"REVISABLE";var lot=ReceiptClone(Lot);lot.PlanId="17";return new ConfirmacionLecturaImportacion{Plan=plan,Lote=HasConfirmation?lot:null};}
        public LoteImportacionCarpetas Lot=new LoteImportacionCarpetas{LoteId="20",Contexto=PlanFixture().Contexto,Snapshot=PlanFixture().Snapshot,Estado="PARCIAL",Revision=7,TotalRaices=3,TotalArchivos=3,TotalBytes="21",ClaveConfirmacion="PRIVATE_KEY",SolicitudHash="PRIVATE_HASH"};
        public List<RaizImportacionCarpetas> Roots=new List<RaizImportacionCarpetas>();public List<ArchivoImportacionCarpetas> Files=new List<ArchivoImportacionCarpetas>();public bool Uncertain=true,Active=true,AdvanceRevisionOnPage;public int Pages;
        void Owner(ContextoImportacionCarpetas c,string id){if(c==null||!c.PuedeImportar||id!=Lot.LoteId||c.UsuarioRef!=Lot.Contexto.UsuarioRef||c.AmbitoRef!=Lot.Contexto.AmbitoRef)throw new PerfilImportacionException("ACCESO_DENEGADO","Fixture");}
        public ContextoImportacionCarpetas ObtenerContextoLote(ContextoImportacionCarpetas c,string id){Owner(c,id);return ReceiptClone(Lot.Contexto);}
        public EstadoLecturaImportacion ConsultarEstado(ContextoImportacionCarpetas c,string id){Owner(c,id);return new EstadoLecturaImportacion{Lote=ReceiptClone(Lot),HayIntentosActivos=Active,HayIncertidumbre=Uncertain};}
        public PaginaRecursosImportacion<RaizImportacionCarpetas> ConsultarRaices(ContextoImportacionCarpetas c,string id,FiltroConsultaImportacion f){Owner(c,id);Pages++;var selected=Roots.Where(r=>(f.RaizId==null||r.RaizId==f.RaizId)&&(f.Estado==null||r.Estado==f.Estado)).OrderBy(r=>int.Parse(r.RaizId)).ToList();return new PaginaRecursosImportacion<RaizImportacionCarpetas>{Total=(uint)selected.Count,RevisionLectura=Lot.Revision,Elementos=ReceiptClone(selected.Skip((f.Pagina-1)*f.TamanoPagina).Take(f.TamanoPagina).ToList())};}
        public PaginaRecursosImportacion<ArchivoImportacionCarpetas> ConsultarArchivos(ContextoImportacionCarpetas c,string id,FiltroConsultaImportacion f){Owner(c,id);Pages++;if(AdvanceRevisionOnPage)Lot.Revision++;var selected=Files.Where(r=>(f.RaizId==null||r.RaizId==f.RaizId)&&(f.Estado==null||r.EstadoEfecto==f.Estado)&&(f.EstadoRecepcion==null||r.EstadoRecepcion==f.EstadoRecepcion)).OrderBy(r=>int.Parse(r.ArchivoId)).ToList();return new PaginaRecursosImportacion<ArchivoImportacionCarpetas>{Total=(uint)selected.Count,RevisionLectura=Lot.Revision,Elementos=ReceiptClone(selected.Skip((f.Pagina-1)*f.TamanoPagina).Take(f.TamanoPagina).ToList())};}
        public QueryRepository(){
            int id=40;foreach(var state in new[]{"BLOQUEADO","INCIERTO","CANCELADA"})Roots.Add(new RaizImportacionCarpetas{RaizId=(id++).ToString(),LoteId="20",ClienteRaizId="r"+id,NombreOriginal="=1+1",Estado=state,Revision=1,IntentoActivoId="PRIVATE_ATTEMPT"});
            id=30;foreach(var state in new[]{"RECHAZADO","INCIERTO","CANCELADO"})Files.Add(new ArchivoImportacionCarpetas{ArchivoId=(id++).ToString(),RaizId="40",ClienteArchivoId="f"+id,NombreOriginal="\t=1+1",EstadoRecepcion=state,EstadoEfecto=state,Revision=1,BytesPrevistos="7",StagingRef="PRIVATE_PATH",ContenidoHash="PRIVATE_CONTENT",IntentoActivoId="PRIVATE_ATTEMPT",ValoresEfectivos=new List<ValorReglaPerfilImportacion>{new ValorReglaPerfilImportacion{Valor="PRIVATE_VALUE"}},Resultado=new ResultadoEfectoImportacion{Mensaje="PRIVATE_ERROR",Codigo="PRIVATE_CODE",Recepcion=new RecepcionVerificadaImportacion{StagingRef="PRIVATE_PATH"}}});
        }
    }

    static void QueryCases() {
        var resources=new ResourceFixture();var repository=new QueryRepository();repository.Lot.Contexto=resources.Adapter.ResolverDestino(resources.Http,"7");repository.Lot.Snapshot.Configuracion=resources.Configuration();
        var service=new ServicioConsultaImportacion(()=>resources.Adapter.Resolver(resources.Http),repository,id=>resources.Adapter.ResolverDestino(resources.Http,id,"CONSULTAR"),(c,config)=>resources.Adapter.AutorizarRecursos(resources.Http,c,config,"CONSULTAR"),2);
        var request=new ConsultarLoteImportacionSolicitud{loteId="20"};var original=PerfilImportacionJson.Escribir(repository.Lot);var originalFiles=PerfilImportacionJson.Escribir(repository.Files);
        var state=service.Consultar(request);Check(state.totalArchivos==3&&state.totalBytes=="21"&&state.hayIncertidumbre&&state.hayIntentosActivos&&state.estado=="PARCIAL","DOC111 query changes aggregate or hides uncertainty");
        var confirmationRequest=new ConsultarConfirmacionImportacionSolicitud{planId="17"};
        var confirmation=service.ConsultarConfirmacion(confirmationRequest);Check(confirmation.confirmada&&confirmation.loteId=="20"&&confirmation.planId=="17","DOC111 read recovery does not return original lot");
        repository.HasConfirmation=false;confirmation=service.ConsultarConfirmacion(confirmationRequest);Check(!confirmation.confirmada&&confirmation.loteId==null,"DOC111 absent confirmation fabricates a lot");repository.HasConfirmation=true;
        confirmationRequest.planId="19";Rejected("ACCESO_DENEGADO",()=>service.ConsultarConfirmacion(confirmationRequest));confirmationRequest.planId="17";
        resources.CabinetAllowed=false;Rejected("ACCESO_DENEGADO",()=>service.ConsultarConfirmacion(confirmationRequest));resources.CabinetAllowed=true;
        Check(state.accionesPermitidas.SequenceEqual(new[]{"CONSULTAR","EXPORTAR_PAGINA","CONSULTAR_RAICES","SOLICITAR_DETENCION","SOLICITAR_VERIFICACION"})&&!state.accionesPermitidas.Contains("SOLICITAR_AVANCE"),"DOC111 query advertises advance while uncertain/active or omits bounded controls");
        foreach(var aggregate in new[]{"RECIBIENDO","EN_CURSO","DETENIDO","COMPLETADO","FALLIDO","PENDIENTE_VERIFICACION"}){repository.Lot.Estado=aggregate;Check(!service.Consultar(request).accionesPermitidas.Any(a=>a=="AVANZAR"||a=="RECONCILIAR"||a=="DETENER"),"DOC111 state label grants an effect");}repository.Lot.Estado="PARCIAL";
        repository.Active=false;repository.Uncertain=false;Check(service.Consultar(request).accionesPermitidas.Contains("SOLICITAR_AVANCE"),"DOC111 available bounded advance not announced");repository.Lot.DetencionSolicitada=true;Check(!service.Consultar(request).accionesPermitidas.Contains("SOLICITAR_AVANCE"),"DOC111 stopped lot advertises new phase");repository.Lot.DetencionSolicitada=false;repository.Active=true;repository.Uncertain=true;
        var pageRequest=new ConsultarRecursosImportacionSolicitud{loteId="20",tipo="ARCHIVOS",pagina=1,tamanoPagina=2};
        var first=service.ConsultarPagina(pageRequest);pageRequest.pagina=2;var second=service.ConsultarPagina(pageRequest);
        Check(first.totalOriginal==3&&first.totalFiltrado==3&&first.elementos.Count==2&&first.puedeHaberMas&&second.elementos.Single().estado=="CANCELADO"&&!second.puedeHaberMas,"DOC111 query paging loses original/rejected totals");
        Check(first.elementos.Select(r=>r.recursoId).Concat(second.elementos.Select(r=>r.recursoId)).Distinct().Count()==3,"DOC111 query paging repeats or drops IDs");
        pageRequest.pagina=1;pageRequest.estado="INCIERTO";var filtered=service.ConsultarPagina(pageRequest);Check(filtered.totalFiltrado==1&&filtered.totalOriginal==3&&filtered.elementos.Single().estado=="INCIERTO","DOC111 filtered query fabricates completion");pageRequest.estado=null;
        var json=PerfilImportacionJson.Escribir(first)+PerfilImportacionJson.Escribir(state);Check(!json.Contains("PRIVATE_")&&!json.Contains("StagingRef")&&!json.Contains("SolicitudHash"),"DOC111 read response leaks private evidence");
        var savedAttempt=repository.Files[0].IntentoActivoId;repository.Files[0].IntentoActivoId="50";Check(service.ConsultarPagina(pageRequest).elementos[0].intentoActivoId=="50","DOC111 response loss cannot recover known attempt reference");repository.Files[0].IntentoActivoId=savedAttempt;
        var export=service.ExportarPagina(pageRequest);Check(export.pagina.totalOriginal==3&&export.pagina.elementos.Count==2&&export.nombreArchivo=="importacion-20-archivos-pagina-1.csv"&&export.contenido.Contains("\"'\t=1+1\"")&&!export.contenido.Contains("PRIVATE_"),"DOC111 page CSV exposes evidence or spreadsheet formula");
        pageRequest.tipo="RAICES";var roots=service.ConsultarPagina(pageRequest);Check(roots.totalFiltrado==3&&roots.elementos[0].estado=="BLOQUEADO","DOC111 root query hides blocked resources");
        var savedRoot=ReceiptClone(repository.Roots[0]);repository.Roots[0].Estado="EXPEDIENTE_CONFIRMADO";repository.Roots[0].IdentidadExpediente=new IdentidadExpedienteImportacion{ExpedienteId="60",RelacionNivelId="70",NivelRef="7",OrigenLogico="RA"};repository.Roots[0].ValoresPlan=new List<ValorReglaPerfilImportacion>{new ValorReglaPerfilImportacion{DestinoTipo="CAMPO_EXPEDIENTE",DestinoRef="NOMBRE",Valor="Nombre confirmado distinto de carpeta"}};
        Check(service.ConsultarPagina(pageRequest).elementos[0].nombreExpediente=="Nombre confirmado distinto de carpeta","DOC111 tree label uses original folder instead of confirmed NOMBRE");repository.Roots[0].Estado="INCIERTO";Check(service.ConsultarPagina(pageRequest).elementos[0].nombreExpediente==null,"DOC111 uncertain root supplies confirmed tree label");repository.Roots[0]=savedRoot;
        pageRequest.estadoRecepcion="RECIBIDO";Rejected("SOLICITUD_INVALIDA",()=>service.ConsultarPagina(pageRequest));pageRequest.estadoRecepcion=null;
        pageRequest.tamanoPagina=3;Rejected("SOLICITUD_INVALIDA",()=>service.ExportarPagina(pageRequest));pageRequest.tamanoPagina=2;
        pageRequest.estado="INCIERTO' OR 1=1";Rejected("SOLICITUD_INVALIDA",()=>service.ConsultarPagina(pageRequest));pageRequest.estado=null;
        pageRequest.tipo="archivos";Rejected("SOLICITUD_INVALIDA",()=>service.ConsultarPagina(pageRequest));pageRequest.tipo="ARCHIVOS";
        pageRequest.pagina=0;Rejected("SOLICITUD_INVALIDA",()=>service.ConsultarPagina(pageRequest));pageRequest.pagina=1;
        pageRequest.raizId="040";Rejected("SOLICITUD_INVALIDA",()=>service.ConsultarPagina(pageRequest));pageRequest.raizId=null;
        request.loteId="21";Rejected("ACCESO_DENEGADO",()=>service.Consultar(request));request.loteId="20";
        resources.ImportAllowed=false;Rejected("ACCESO_DENEGADO",()=>service.Consultar(request));resources.ImportAllowed=true;
        resources.CabinetAllowed=false;var before=repository.Pages;Rejected("ACCESO_DENEGADO",()=>service.ExportarPagina(pageRequest));Check(repository.Pages==before,"DOC111 unauthorized export reads page");resources.CabinetAllowed=true;
        resources.Moved=true;Rejected("ACCESO_DENEGADO",()=>service.Consultar(request));resources.Moved=false;
        repository.Lot.Contexto.UsuarioRef="99";Rejected("ACCESO_DENEGADO",()=>service.Consultar(request));repository.Lot.Contexto.UsuarioRef="1";
        Check(PerfilImportacionJson.Escribir(repository.Lot)==original&&PerfilImportacionJson.Escribir(repository.Files)==originalFiles,"DOC111 read/export mutated durable data");
        Unavailable(()=>new ServicioConsultaImportacion(()=>RequestContext(),repository,id=>RequestContext(),(c,config)=>{},0));
        Check(ServicioConsultaImportacion.MaximoPagina==200,"DOC111 query changed existing technical page ceiling");
        foreach(var prefix in new[]{"=","+","-","@"," =","\t=","\r\n=","\ufeff=","\u200b="}) {
            var method=typeof(ServicioConsultaImportacion).GetMethod("CeldaCsv",BindingFlags.NonPublic|BindingFlags.Static);var value=(string)method.Invoke(null,new object[]{prefix+"SUM(1)"});Check(value.StartsWith("\"'"),"DOC111 unsafe CSV prefix "+prefix.Length);
        }
        var cell=typeof(ServicioConsultaImportacion).GetMethod("CeldaCsv",BindingFlags.NonPublic|BindingFlags.Static);Check((string)cell.Invoke(null,new object[]{"a,\"b\"\r\nc"})=="\"a,\"\"b\"\"\r\nc\"","DOC111 CSV loses quotes or multiline cells");
        Console.WriteLine("DOC-111 authorized read-only pages, private projection and CSV: passed");
    }

    static void RetentionDiagnosisCases() {
        var resources=new ResourceFixture();var repository=new QueryRepository();repository.Lot.Contexto=resources.Adapter.ResolverDestino(resources.Http,"7");repository.Lot.Snapshot.Configuracion=resources.Configuration();repository.Lot.Estado="COMPLETADO";repository.Lot.ActualizadoUtc="2026-10-28T12:00:00Z";repository.Active=false;repository.Uncertain=false;
        foreach(var root in repository.Roots){root.Estado="COMPLETADA";root.IntentoActivoId=null;}
        foreach(var file in repository.Files){file.EstadoRecepcion="RECIBIDO";file.EstadoEfecto="COMPLETADO";file.IntentoActivoId=null;file.StagingRef=new string('a',32);file.ContenidoHash=new string('b',64);file.BytesRecibidos=file.BytesPrevistos;file.Resultado=new ResultadoEfectoImportacion{DocumentoId="90",GabineteId="7",OrigenGabinete="DA",Expediente=new IdentidadExpedienteImportacion{OrigenLogico="RA",NivelRef="7",ExpedienteId="60",RelacionNivelId="70"},Mensaje="PRIVATE_DIAGNOSTIC_SENTINEL"};}
        var now=new DateTime(2026,10,29,12,0,0,DateTimeKind.Utc);
        var service=new ServicioConsultaImportacion(()=>resources.Adapter.Resolver(resources.Http),repository,id=>resources.Adapter.ResolverDestino(resources.Http,id,"CONSULTAR"),(c,config)=>resources.Adapter.AutorizarRecursos(resources.Http,c,config,"CONSULTAR"),2,()=>now);
        var request=new ConsultarRecursosImportacionSolicitud{loteId="20",tipo="ARCHIVOS",pagina=1,tamanoPagina=2};
        var original=PerfilImportacionJson.Escribir(repository.Lot)+PerfilImportacionJson.Escribir(repository.Files)+PerfilImportacionJson.Escribir(repository.Roots);
        var absent=service.DiagnosticarRetencion(request);Check(absent.diagnosticoEnSeco&&!absent.eliminacionHabilitada&&!absent.retencionConfigurada&&absent.candidatosEnPagina==0&&absent.elementos.All(i=>i.motivo=="RETENCION_NO_CONFIGURADA"),"DOC111 absent retention enables deletion/review");
        foreach(var duration in new[]{TimeSpan.Zero,TimeSpan.FromSeconds(-1)})Check(!service.DiagnosticarRetencion(request,duration).retencionConfigurada,"DOC111 invalid retention becomes configured");
        var result=service.DiagnosticarRetencion(request,TimeSpan.FromDays(1));
        Check(result.candidatosEnPagina==2&&result.totalOriginal==3&&result.totalFiltrado==3&&result.puedeHaberMas&&result.revisionEstable&&result.venceRetencionUtc=="2026-10-29T12:00:00Z"&&!result.eliminacionHabilitada,"DOC111 diagnostic changes totals or authorizes writes");
        request.pagina=2;Check(service.DiagnosticarRetencion(request,TimeSpan.FromDays(1)).candidatosEnPagina==1,"DOC111 diagnostic drops last page");request.pagina=1;
        var json=PerfilImportacionJson.Escribir(result);Check(!json.Contains("PRIVATE_")&&!json.Contains(new string('a',32))&&!json.Contains(new string('b',64))&&!json.Contains("StagingRef"),"DOC111 diagnostic leaks private evidence");
        Check(!typeof(DiagnosticoRetencionImportacionResultado).GetProperty("eliminacionHabilitada").CanWrite&&!typeof(DiagnosticoRetencionImportacionResultado).GetProperty("diagnosticoEnSeco").CanWrite,"DOC111 dry-run flags are writable");
        repository.Uncertain=true;result=service.DiagnosticarRetencion(request,TimeSpan.FromDays(1));Check(result.candidatosEnPagina==0&&result.elementos.All(i=>i.motivo=="CONSERVAR_INCERTIDUMBRE"),"DOC111 uncertain lot loses recovery evidence");repository.Uncertain=false;
        repository.Active=true;result=service.DiagnosticarRetencion(request,TimeSpan.FromDays(1));Check(result.candidatosEnPagina==0&&result.elementos.All(i=>i.motivo=="CONSERVAR_OPERACION_ACTIVA"),"DOC111 active lot becomes cleanup candidate");repository.Active=false;
        result=service.DiagnosticarRetencion(request,TimeSpan.FromDays(2));Check(result.candidatosEnPagina==0&&result.elementos.All(i=>i.motivo=="RETENCION_VIGENTE"),"DOC111 retention expires early");
        foreach(var date in new[]{null,"not-a-date","2026-10-30T12:00:00Z"}){repository.Lot.ActualizadoUtc=date;result=service.DiagnosticarRetencion(request,TimeSpan.FromDays(1));Check(result.candidatosEnPagina==0&&result.elementos.All(i=>i.motivo=="FECHA_NO_VERIFICABLE"),"DOC111 invalid/future timestamp permits cleanup");}repository.Lot.ActualizadoUtc="2026-10-28T12:00:00Z";
        Check(service.DiagnosticarRetencion(request,TimeSpan.MaxValue).candidatosEnPagina==0,"DOC111 retention overflow enables review");
        foreach(var state in new[]{"RECIBIENDO","LISTO","EN_CURSO","DETENIDO","PARCIAL","FALLIDO","PENDIENTE_VERIFICACION"}){repository.Lot.Estado=state;Check(service.DiagnosticarRetencion(request,TimeSpan.FromDays(1)).candidatosEnPagina==0,"DOC111 pending/failing lot becomes cleanup candidate");}repository.Lot.Estado="COMPLETADO";
        var savedFile=ReceiptClone(repository.Files[0]);
        foreach(var mutate in new Action<ArchivoImportacionCarpetas>[] {f=>f.EstadoRecepcion="INCIERTO",f=>f.EstadoEfecto="INCIERTO",f=>f.IntentoActivoId="50",f=>f.EstadoRecepcion="RECIBIENDO",f=>f.EstadoEfecto="EN_CURSO",f=>f.EstadoEfecto="RECHAZADO",f=>f.StagingRef=null,f=>f.ContenidoHash="bad",f=>f.BytesRecibidos="6",f=>f.Resultado.DocumentoId=null,f=>f.Resultado.Expediente.NivelRef="8"}) {
            repository.Files[0]=ReceiptClone(savedFile);mutate(repository.Files[0]);result=service.DiagnosticarRetencion(request,TimeSpan.FromDays(1));Check(!result.elementos[0].candidatoRevision,"DOC111 incomplete/uncertain evidence qualifies for cleanup");
        }repository.Files[0]=savedFile;
        Check(PerfilImportacionJson.Escribir(repository.Lot)+PerfilImportacionJson.Escribir(repository.Files)+PerfilImportacionJson.Escribir(repository.Roots)==original,"DOC111 diagnostic mutated operational state/evidence");
        repository.AdvanceRevisionOnPage=true;result=service.DiagnosticarRetencion(request,TimeSpan.FromDays(1));Check(!result.revisionEstable&&result.candidatosEnPagina==0&&result.elementos.All(i=>i.motivo=="REVISION_CAMBIO"),"DOC111 racing page produces stale cleanup candidates");repository.AdvanceRevisionOnPage=false;
        int before=repository.Pages;resources.ImportAllowed=false;Rejected("ACCESO_DENEGADO",()=>service.DiagnosticarRetencion(request,TimeSpan.FromDays(1)));resources.ImportAllowed=true;Check(repository.Pages==before,"DOC111 revoked diagnostic reads resource page");
        resources.CabinetAllowed=false;Rejected("ACCESO_DENEGADO",()=>service.DiagnosticarRetencion(request));resources.CabinetAllowed=true;
        request.tipo="RAICES";Rejected("SOLICITUD_INVALIDA",()=>service.DiagnosticarRetencion(request));request.tipo="ARCHIVOS";request.tamanoPagina=3;Rejected("SOLICITUD_INVALIDA",()=>service.DiagnosticarRetencion(request));request.tamanoPagina=2;
        now=DateTime.SpecifyKind(now,DateTimeKind.Local);Unavailable(()=>service.DiagnosticarRetencion(request));
        Console.WriteLine("DOC-111 retention diagnosis preserves uncertain evidence (read-only double): passed");
    }

    static void KnownExpedientRelationCases() {
        var context=PlanFixture().Contexto;
        var identity=new IdentidadExpedienteImportacion{OrigenLogico="RA",ExpedienteId="60",RelacionNivelId="70",NivelRef=context.NivelRef};
        var rows=new DataTable();foreach(var column in new[]{"expediente","relacion","nivel","empresa","usuario","nivel_activo","electronico","empresa_nivel","autorizado"})rows.Columns.Add(column,typeof(object));
        rows.Rows.Add("60","70",context.NivelRef,context.AmbitoRef,context.UsuarioRef,"1","2",context.AmbitoRef,"1");
        int reads=0;Func<string,object[],DataTable> query=(sql,args)=>{
            reads++;Check(sql.StartsWith("SELECT ")&&!sql.Contains("FOR UPDATE")&&sql.EndsWith("LIMIT 2"),"DOC111 relation verification is not a bounded read");
            foreach(var parameter in new[]{"@relacion","@expediente","@nivel","@empresa","@usuario"})Check(sql.Contains(parameter)&&args.Contains(parameter),"DOC111 known relation query lost scope");
            Check(sql.Contains("r.id_registro=@relacion")&&sql.Contains("e.ID_EXPEDIENTE=@expediente")&&sql.Contains("p.agregar_expediente=1")&&!sql.Contains("CODIGO_UNICO")&&!sql.Contains("LIKE")&&!sql.Contains("nombre_nivel"),"DOC111 known effect is inferred from labels");
            Check((uint)args[Array.IndexOf(args,"@expediente")+1]==60u&&(uint)args[Array.IndexOf(args,"@relacion")+1]==70u,"DOC111 known IDs are interpolated or substituted");return rows;
        };
        Func<bool> check=()=>(bool)RepositoryHelper("LeerRelacionExpedienteConocido",context,identity,query);
        Check(check(),"DOC111 exact known relation does not verify");
        foreach(DataColumn column in rows.Columns){var prior=rows.Rows[0][column];rows.Rows[0][column]="999";Check(!check(),"DOC111 foreign/mismatched relation component accepted: "+column.ColumnName);rows.Rows[0][column]=DBNull.Value;Check(!check(),"DOC111 null relation component accepted");rows.Rows[0][column]=prior;}
        rows.ImportRow(rows.Rows[0]);Check(!check(),"DOC111 ambiguous known relation verifies");rows.Rows.RemoveAt(1);
        var saved=rows.Rows[0].ItemArray;rows.Clear();Check(!check(),"DOC111 missing known relation verifies");rows.Rows.Add(saved);
        identity.NivelRef="999";var before=reads;Check(!check()&&reads==before,"DOC111 wrong fixed destination reaches SQL");identity.NivelRef=context.NivelRef;
        identity.ExpedienteId="60 OR 1=1";Rejected("SOLICITUD_INVALIDA",()=>check());Check(reads==before,"DOC111 malformed known ID reaches SQL");identity.ExpedienteId="60";
        identity.OrigenLogico="ra";Rejected("SOLICITUD_INVALIDA",()=>check());identity.OrigenLogico="RA";
        context.PuedeImportar=false;Rejected("ACCESO_DENEGADO",()=>check());context.PuedeImportar=true;
        rows.Columns.Remove("autorizado");Unavailable(()=>check());
        Console.WriteLine("DOC-111 known expedition relation rejects wrong scope (synthetic SELECT records): passed");
    }

    sealed class CreationCommandFixture {
        public ContextoImportacionCarpetas Context=PlanFixture().Contexto;
        public LoteImportacionCarpetas Lot;
        public RaizImportacionCarpetas Root;
        public IntentoImportacionCarpetas Attempt;
        public CreationCommandFixture() {
            var config=new ConfiguracionPerfilImportacion{NombreVisible="Fixture",GabineteId=7,HuellaOrigenGabinete=new string('a',64),FondoRef="1",InstrumentoRef="2",AreaRef="3",SerieRef="4",TipoExpedienteRef="5",UnidadDocumentalRef="6",ModoIntegridad="COMPLETO_OBLIGATORIO",AmbitoIntegridad="EXPEDIENTE",EstrategiaCarpetas="RAIZ_EXPEDIENTE",PoliticaSubcarpetas="INCLUIR",PoliticaCarpetasVacias="RECHAZAR",PoliticaArchivosSueltos="RECHAZAR",Extensiones=new List<string>{"pdf"}};
            var rules=new List<ReglaPerfilImportacion>();var values=new List<ValorReglaPerfilImportacion>();
            foreach(var field in new[]{"CODIGO","NOMBRE","FECHA_INICIAL","FECHA_FINAL","RESPONSABLE"}){
                string type=field.StartsWith("FECHA")?"FECHA":field=="RESPONSABLE"?"REFERENCIA":"TEXTO";
                string value=field=="CODIGO"?"000123":field=="NOMBRE"?"Órdenes d'Ávila":field=="FECHA_INICIAL"?"2026-10-29":field=="FECHA_FINAL"?"2026-10-30":"8";
                rules.Add(new ReglaPerfilImportacion{DestinoTipo="CAMPO_EXPEDIENTE",DestinoRef=field,TipoValor=type,FuenteTipo="CARPETA",FormatoFecha=type=="FECHA"?"dd-MM-yyyy":null,LongitudMaxima=160});
                values.Add(new ValorReglaPerfilImportacion{DestinoTipo="CAMPO_EXPEDIENTE",DestinoRef=field,TipoValor=type,FuenteTipo="CARPETA",Valor=value,Procedencia="fixture:already-resolved"});
            }
            rules.Add(new ReglaPerfilImportacion{DestinoTipo="TIPOLOGIA",DestinoRef="TIPO_DOCUMENTAL",TipoValor="REFERENCIA",FuenteTipo="CONSTANTE",ValorConstante="9"});
            var snapshot=new SnapshotPerfilImportacionOperativa{PerfilId="4",VersionId="5",NumeroVersion=1,Configuracion=config,Reglas=rules};
            Lot=new LoteImportacionCarpetas{LoteId="20",Contexto=ReceiptClone(Context),Snapshot=snapshot,Estado="EN_CURSO",Revision=2};
            Root=new RaizImportacionCarpetas{LoteId="20",RaizId="40",Estado="CREANDO",Revision=2,IntentoActivoId="50",ValoresPlan=values};
            Attempt=new IntentoImportacionCarpetas{IntentoId="50",LoteId="20",RaizId="40",Fase="CREAR_EXPEDIENTE",Estado="RESERVADO",EsNuevaReserva=true,TokenReclamacion=new string('a',32),SolicitudHash=new string('b',64)};
            Rehash();
        }
        public void Rehash(){Lot.Snapshot.Huella=Hex(PerfilImportacionJson.Huella(new VersionPerfilImportacion{Configuracion=Lot.Snapshot.Configuracion,Reglas=Lot.Snapshot.Reglas}));}
        public ComandoCreacionExpedienteImportacion Prepare(){return ComandoCreacionExpedienteImportacion.Preparar(Context,Lot,Root,Attempt);}
    }

    sealed class CreationCatalogFixture {
        public CreationCommandFixture Source=new CreationCommandFixture();
        public bool Allowed=true,Ambiguous,Missing,NullIdentification,NoColumn;
        public string ReverseId,ResponsibleId,ChangedName,Target="SERIE";
        public int Reads,ResponsibleReads;
        public Dictionary<string,string> Names=new Dictionary<string,string>{{"ORGANIGRAMA","Organigrama"},{"FONDO","Fondo"},{"AREA","Área"},{"SERIE","Serie d'Ávila"},{"SUBSERIE","Subserie"},{"TIPO_EXPEDIENTE","Tipo"},{"UNIDAD_DOCUMENTAL","Compuesta"},{"GABINETE","Gabinete"},{"CICLO_ARCHIVO","Archivo Gestión"}};
        public CreationCatalogFixture(){
            using(var sha=System.Security.Cryptography.SHA256.Create())Source.Lot.Snapshot.Configuracion.HuellaOrigenGabinete=Hex(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(PerfilImportacionJson.Escribir(new[]{"host-DA","3306","1","base-DA"}))));Source.Rehash();
        }
        static DataTable Row(string[] columns,params object[] values){var t=new DataTable();foreach(var c in columns)t.Columns.Add(c,typeof(object));if(values.Length>0)t.Rows.Add(values);return t;}
        static object Parameter(object[] args,string name){return args[Array.IndexOf(args,name)+1];}
        public Dictionary<string,string> Resolve(){return new PerfilImportacionCatalogAdapter("RA","DA",Query).ResolverDatosCreacionExpediente(Source.Prepare());}
        public Dictionary<string,string> PrepareLegacy(){return new PerfilImportacionCatalogAdapter("RA","DA",Query).PrepararDatosCreacionLegacy(Source.Prepare());}
        public DataTable Query(string origin,string sql,object[] args){
            Reads++;Check(sql.StartsWith("SELECT ")&&!sql.Contains("FOR UPDATE"),"DOC111 creation catalog performs mutations");
            if(sql.Contains("SELECT @@hostname")){Check(origin=="DA","DOC111 origin inspected outside DA");return Row(new[]{"host","puerto","servidor","base"},"host-DA","3306","1","base-DA");}
            if(sql.StartsWith("SELECT n.id_nivel")){Check(origin=="RA"&&sql.Contains("agregar_expediente=1"),"DOC111 creation destination loses authority");return Row(new[]{"id_nivel"},3);}
            if(sql.Contains("SELECT RELACION_DA"))return Row(new[]{"RELACION_DA"},10);
            if(sql.StartsWith("SELECT Clave_Usuario")){Check(origin=="DA"&&sql.Contains("Relacion_Gestion=@usuario"),"DOC111 reciprocal DA identity omitted");return Row(new[]{"Clave_Usuario"},10);}
            if(sql.StartsWith("SELECT s.id_gabinete")){Check(origin=="DA"&&sql.Contains("ALMACENA_IMAGEN=1"),"DOC111 creation cabinet loses permissions");return Allowed?Row(new[]{"id_gabinete","INVENTARIO_DOCUMENTAL","APLICA_TRD","ASIGNA_UNIDAD"},7,"1","1","1"):Row(new[]{"id_gabinete","INVENTARIO_DOCUMENTAL","APLICA_TRD","ASIGNA_UNIDAD"});}
            if(sql.Contains("@responsable")){
                ResponsibleReads++;Check(origin=="RA"&&sql.Contains("Estado_Usuario=1")&&sql.Contains("Empresa_Gestion_Documental_id_empresa=@empresa")&&Convert.ToString(Parameter(args,"@responsable"))=="8","DOC111 resolved responsible ID reinterpreted as original folder identification");
                return Row(new[]{"id","nombre","identificacion"},ResponsibleId??"8","Responsable",NullIdentification?DBNull.Value:(object)"00012345");
            }
            string kind,id;
            if(sql.Contains("FROM system1")){kind="GABINETE";id="7";Check(origin=="DA"&&(sql.Contains("ALMACENA_IMAGEN=1")||sql.Contains("WHERE NOMBRE=@nombre")),"DOC111 cabinet name resolved outside authorized DA");}
            else if(sql.Contains("FROM ra_de_fondo_documental")){kind="FONDO";id="1";}
            else if(sql.Contains("FROM ra_de_tipos_ciclos_archivo")){kind="CICLO_ARCHIVO";id="13";}
            else if(sql.Contains("FROM subseries_documentales")){kind="SUBSERIE";id="12";}
            else if(sql.Contains("FROM series_documentales")){kind="SERIE";id="4";}
            else if(sql.Contains("FROM areas_depart_radicacion")){kind="AREA";id="3";}
            else if(sql.Contains("FROM registro_organigrama")){kind="ORGANIGRAMA";id="11";}
            else if(sql.Contains("FROM tipos_unidad_documental")){kind="UNIDAD_DOCUMENTAL";id="6";}
            else if(sql.Contains("FROM ra_tipo_expediente")){kind="TIPO_EXPEDIENTE";id="5";Check(sql.Contains("ESTADO_TIPO=1"),"DOC111 creation type accepts inactive entry");}
            else throw new Exception("DOC111 unexpected creation catalog query");
            bool reverse=sql.Contains("@nombre");Check(origin==(kind=="GABINETE"?"DA":"RA"),"DOC111 catalog origin confused");
            if(reverse){Check(sql.EndsWith("LIMIT 2")&&Convert.ToString(Parameter(args,"@nombre"))==Names[kind]&&!sql.Contains("'"+Names[kind]+"'"),"DOC111 reverse lookup interpolates catalog text");if(kind=="SERIE")Check(sql.Contains("@area")&&sql.Contains("@instrumento"),"DOC111 legacy series lookup scope lost");if(kind=="AREA")Check(Convert.ToString(Parameter(args,"@organigrama"))=="11","DOC111 area lookup lost derived organigram");if(kind=="SUBSERIE")Check(Convert.ToString(Parameter(args,"@serie"))=="4","DOC111 subseries lookup lost series");}
            var t=Row(new[]{"id","nombre"},reverse&&kind==Target&&ReverseId!=null?ReverseId:id,reverse&&kind==Target&&ChangedName!=null?ChangedName:Names[kind]);
            if(reverse&&kind==Target){if(Ambiguous)t.ImportRow(t.Rows[0]);if(Missing)t.Rows.Clear();if(NoColumn)t.Columns.Remove("nombre");}
            return t;
        }
    }

    static void CreationCatalogCases(){
        var f=new CreationCatalogFixture();var original=PerfilImportacionJson.Escribir(f.Source.Lot);var names=f.Resolve();
        Check(names["ORGANIGRAMA_ID"]=="11"&&names["AREA_NOMBRE"]=="Área"&&names["SERIE_NOMBRE"]=="Serie d'Ávila"&&names["SUBSERIE_NOMBRE"]==null&&names["RESPONSABLE_IDENTIFICACION"]=="00012345"&&f.ResponsibleReads==1,"DOC111 catalog names/known absence/responsible not preserved");Check(PerfilImportacionJson.Escribir(f.Source.Lot)==original,"DOC111 creation lookup mutates snapshot");
        foreach(var kind in new[]{"ORGANIGRAMA","FONDO","AREA","SERIE","SUBSERIE","UNIDAD_DOCUMENTAL","GABINETE"}){
            f=new CreationCatalogFixture();f.Source.Lot.Snapshot.Configuracion.SubserieRef="12";f.Source.Rehash();f.Target=kind;f.Ambiguous=true;Rejected("ACCESO_DENEGADO",()=>f.Resolve());f.Ambiguous=false;f.Missing=true;Rejected("ACCESO_DENEGADO",()=>f.Resolve());f.Missing=false;f.ReverseId="999";Rejected("ACCESO_DENEGADO",()=>f.Resolve());f.ReverseId=null;f.ChangedName="renamed";Rejected("ACCESO_DENEGADO",()=>f.Resolve());f.ChangedName=null;f.NoColumn=true;Unavailable(()=>f.Resolve());
        }
        f=new CreationCatalogFixture();f.Allowed=false;Rejected("ACCESO_DENEGADO",()=>f.Resolve());Check(f.ResponsibleReads==0,"DOC111 denied cabinet still reads responsible");
        f=new CreationCatalogFixture();f.NullIdentification=true;Rejected("ACCESO_DENEGADO",()=>f.Resolve());f.NullIdentification=false;f.ResponsibleId="999";Rejected("ACCESO_DENEGADO",()=>f.Resolve());
        f=new CreationCatalogFixture();f.Names["UNIDAD_DOCUMENTAL"]=" Compuesta ";Rejected("ACCESO_DENEGADO",()=>f.Resolve());
        f=new CreationCatalogFixture();f.Source.Lot.Snapshot.Reglas.RemoveAll(r=>r.DestinoRef=="RESPONSABLE");f.Source.Root.ValoresPlan.RemoveAll(v=>v.DestinoRef=="RESPONSABLE");f.Source.Rehash();names=f.Resolve();Check(names["RESPONSABLE_NOMBRE"]==null&&names["RESPONSABLE_IDENTIFICACION"]==null&&f.ResponsibleReads==0,"DOC111 creation lookup fabricates default responsible");
        Console.WriteLine("DOC-111 shared creation catalogs verify exact IDs and reverse names (synthetic SELECT records): passed");
    }

    static CreationCatalogFixture SafeCreationCatalog(){var f=new CreationCatalogFixture();f.Source.Root.ValoresPlan[1].Valor="Órdenes \"2026\"";f.Names["SERIE"]="Serie";return f;}

    static void CreationLegacyCompatibilityCases(){
        var f=SafeCreationCatalog();var before=PerfilImportacionJson.Escribir(f.Source.Lot)+PerfilImportacionJson.Escribir(f.Source.Root);var names=f.PrepareLegacy();
        Check(names["AREA_NOMBRE"]=="Área"&&names["RESPONSABLE_IDENTIFICACION"]=="00012345"&&f.Source.Prepare().Nombre=="Órdenes \"2026\"","DOC111 legacy preparation loses exact text");
        Check(before==PerfilImportacionJson.Escribir(f.Source.Lot)+PerfilImportacionJson.Escribir(f.Source.Root),"DOC111 compatibility gate modifies confirmed values");
        foreach(var bad in new[]{"O'Brien","A\\B","A\nB","A\0B","A\tB"})foreach(var index in new[]{0,1}){
            f=SafeCreationCatalog();f.Source.Root.ValoresPlan[index].Valor=bad;Rejected("CONFIGURACION_INVALIDA",()=>f.PrepareLegacy());Check(f.Reads==0,"DOC111 incompatible input reaches catalog/legacy");
        }
        f=SafeCreationCatalog();f.Source.Root.ValoresPlan[1].Valor=new string('N',121);Rejected("CONFIGURACION_INVALIDA",()=>f.PrepareLegacy());Check(f.Reads==0,"DOC111 visible alias too long reaches writer");
        foreach(var key in new[]{"ORGANIGRAMA","FONDO","AREA","SERIE","UNIDAD_DOCUMENTAL","GABINETE"}){
            f=SafeCreationCatalog();f.Names[key]="O'Brien";Rejected("CONFIGURACION_INVALIDA",()=>f.PrepareLegacy());Check(f.Names[key]=="O'Brien","DOC111 incompatible catalog label silently renamed");
        }
        foreach(var change in new Action<CreationCommandFixture>[] {
            x=>{x.Context.UsuarioRef="2147483648";x.Lot.Contexto.UsuarioRef=x.Context.UsuarioRef;},
            x=>{x.Context.AmbitoRef="2147483648";x.Lot.Contexto.AmbitoRef=x.Context.AmbitoRef;},
            x=>{x.Context.NivelRef="2147483648";x.Lot.Contexto.NivelRef=x.Context.NivelRef;},
            x=>x.Lot.Snapshot.Configuracion.InstrumentoRef="2147483648",x=>x.Lot.Snapshot.Configuracion.TipoExpedienteRef="2147483648",x=>x.Lot.Snapshot.Configuracion.FondoRef="2147483648",x=>x.Lot.Snapshot.Configuracion.AreaRef="2147483648",x=>x.Lot.Snapshot.Configuracion.SerieRef="2147483648",x=>x.Lot.Snapshot.Configuracion.SubserieRef="2147483648",x=>x.Lot.Snapshot.Configuracion.UnidadDocumentalRef="2147483648"
        }){
            f=SafeCreationCatalog();change(f.Source);f.Source.Rehash();Rejected("CONFIGURACION_INVALIDA",()=>f.PrepareLegacy());Check(f.Reads==0,"DOC111 out-of-range legacy reference reaches catalog");
        }
        f=SafeCreationCatalog();f.Source.Context.UsuarioRef="2147483647";f.Source.Lot.Contexto.UsuarioRef=f.Source.Context.UsuarioRef;f.Source.Lot.Snapshot.Configuracion.SerieRef="2147483647";f.Source.Rehash();f.Source.Prepare().ValidarEntradasLegacy();
        f=SafeCreationCatalog();f.Source.Lot.LoteId="4294967295";f.Source.Root.LoteId=f.Source.Lot.LoteId;f.Source.Attempt.LoteId=f.Source.Lot.LoteId;f.Source.Attempt.IntentoId="4294967295";f.Source.Root.IntentoActivoId=f.Source.Attempt.IntentoId;f.Source.Prepare().ValidarEntradasLegacy();
        f=SafeCreationCatalog();var command=f.Source.Prepare();names=f.Resolve();names["ORGANIGRAMA_ID"]="2147483648";Rejected("CONFIGURACION_INVALIDA",()=>command.ValidarDatosLegacy(names));names["ORGANIGRAMA_ID"]="11";
        names["SUBSERIE_NOMBRE"]="Unexpected";Rejected("CONFIGURACION_INVALIDA",()=>command.ValidarDatosLegacy(names));names["SUBSERIE_NOMBRE"]=null;
        names["RESPONSABLE_IDENTIFICACION"]="A\\B";Rejected("CONFIGURACION_INVALIDA",()=>command.ValidarDatosLegacy(names));names["RESPONSABLE_IDENTIFICACION"]="00012345";
        names.Remove("GABINETE_NOMBRE");Rejected("CONFIGURACION_INVALIDA",()=>command.ValidarDatosLegacy(names));
        Console.WriteLine("DOC-111 legacy creation compatibility rejects unsafe text and Integer overflow (no effects): passed");
    }

    sealed class CallbackDisposable : IDisposable {
        Action close;public CallbackDisposable(Action action){close=action;}public void Dispose(){var action=close;close=null;if(action!=null)action();}
    }
    sealed class CreationAdapterFixture {
        public CreationCatalogFixture Catalog=SafeCreationCatalog();
        public int Authorizations,Calls,Starts,Verifications,Saves;public bool Started,CanVerify=true,ThrowWriter,FailStart,ThrowVerification,RevokeAfterWriter,RevokeAfterStart,ThrowSave,ThrowAfterSave;public ResultadoEfectoImportacion Saved;
        public string Reply="YES";public int Expedition=60,Relation=70;
        public bool LeaseActive,FailLease;public int LeaseOpens,LeaseCloses;
        public AdaptadorExpedienteImportacion Adapter;
        public CreationAdapterFixture(){
            Action<ComandoCreacionExpedienteImportacion> authorize=c=>{Authorizations++;if((RevokeAfterWriter&&Calls>0)||(RevokeAfterStart&&Started))throw new PerfilImportacionException("ACCESO_DENEGADO","synthetic revoked");Check(c.Contexto.NivelRef=="3","DOC111 adapter changes fixed level");};
            Func<ComandoCreacionExpedienteImportacion,Dictionary<string,string>> catalogs=c=>Catalog.PrepareLegacy();
            Func<ComandoCreacionExpedienteImportacion,Dictionary<string,string>,bool> start=(c,d)=>{Starts++;RepositoryHelper("ValidarInicioCreacion",c,Catalog.Source.Lot,Catalog.Source.Root);if(FailStart||Started)return false;Started=true;Check(d["CICLO_ARCHIVO_NOMBRE"]=="Archivo Gestión","DOC111 start loses approved cycle evidence");return true;};
            Action<ComandoCreacionExpedienteImportacion,Dictionary<string,string>,SalidaCreacionExpedienteLegacy> writer=(c,d,s)=>{Calls++;Check(Started&&Authorizations==4&&LeaseActive&&c.Codigo=="000123"&&d["GABINETE_NOMBRE"]=="Gabinete","DOC111 writer before durable start/authority or retained staging proof");s.ExpedienteId=Expedition;s.RelacionNivelId=Relation;s.Respuesta=Reply;if(ThrowWriter)throw new Exception("DOC111_PRIVATE_SQL_OR_PATH_SENTINEL");};
            Func<ComandoCreacionExpedienteImportacion,IdentidadExpedienteImportacion,Dictionary<string,string>,bool> verify=(c,id,d)=>{Verifications++;Check(id.ExpedienteId==Expedition.ToString()&&id.RelacionNivelId==Relation.ToString()&&id.NivelRef==c.Contexto.NivelRef,"DOC111 returned identity not verified");if(ThrowVerification)throw new Exception("DOC111_PRIVATE_VERIFY_SENTINEL");return CanVerify;};
            Func<ComandoCreacionExpedienteImportacion,ResultadoEfectoImportacion,ResultadoEfectoImportacion> save=(c,r)=>{Saves++;Check(Calls==0||LeaseActive,"DOC111 staging released before checkpoint");if(ThrowSave)throw new Exception("DOC111_PRIVATE_CHECKPOINT_SENTINEL");Saved=ReceiptClone(r);if(ThrowAfterSave)throw new Exception("DOC111_PRIVATE_COMMITTED_RESPONSE_SENTINEL");return ReceiptClone(Saved);};
            Func<IDisposable> keep=()=>{if(FailLease)throw new PerfilImportacionException("RESULTADO_INCIERTO","synthetic missing staging");LeaseActive=true;LeaseOpens++;return new CallbackDisposable(()=>{LeaseActive=false;LeaseCloses++;});};
            var constructor=typeof(AdaptadorExpedienteImportacion).GetConstructors(BindingFlags.NonPublic|BindingFlags.Instance).Single();Adapter=(AdaptadorExpedienteImportacion)constructor.Invoke(new object[]{authorize,catalogs,start,writer,verify,save,keep});
        }
        public ResultadoEfectoImportacion Create(){var s=Catalog.Source;return Adapter.Crear(s.Context,s.Lot,s.Root,s.Attempt);}
    }

    static void CreationAdapterCases(){
        var f=new CreationAdapterFixture();var result=f.Create();Check(result.Estado=="CONFIRMADO"&&result.Codigo=="EXPEDIENTE_CONFIRMADO"&&result.Creacion.IntentoId=="50"&&result.Creacion.CicloArchivoId=="13"&&f.Calls==1&&f.Verifications==1&&!f.Catalog.Source.Attempt.EsNuevaReserva,"DOC111 creation not confirmed from one verified call");
        Rejected("CONFLICTO_REVISION",()=>f.Create());Check(f.Calls==1,"DOC111 duplicate adapter call repeats effect");f.Catalog.Source.Attempt.EsNuevaReserva=true;Rejected("CONFLICTO_REVISION",()=>f.Create());Check(f.Calls==1,"DOC111 repeated start invokes legacy");
        f=new CreationAdapterFixture();f.FailStart=true;Rejected("CONFLICTO_REVISION",()=>f.Create());Check(f.Calls==0,"DOC111 unsuccessful CAS invokes legacy");
        f=new CreationAdapterFixture();f.RevokeAfterStart=true;result=f.Create();Check(result.Estado=="INCIERTO"&&f.Calls==0&&f.Starts==1,"DOC111 authority revoked after start invokes legacy");
        f=new CreationAdapterFixture();f.FailLease=true;result=f.Create();Check(result.Estado=="INCIERTO"&&f.Calls==0&&f.Saves==1,"DOC111 absent retained proof invokes writer");
        foreach(bool after in new[]{false,true}){f=new CreationAdapterFixture();f.ThrowSave=!after;f.ThrowAfterSave=after;result=f.Create();Check(result.Estado=="INCIERTO"&&result.Expediente.ExpedienteId=="60"&&f.Calls==1&&f.Saves==1&&!PerfilImportacionJson.Escribir(result).Contains("SENTINEL"),"DOC111 checkpoint error repeats writer, loses IDs or exposes private error");if(after)Check(f.Saved.Estado=="CONFIRMADO","DOC111 lost checkpoint response corrupts committed result");}
        foreach(var change in new Action<CreationAdapterFixture>[] {x=>x.ThrowWriter=true,x=>x.Reply="NO",x=>x.Reply="yes",x=>x.CanVerify=false,x=>x.ThrowVerification=true,x=>x.RevokeAfterWriter=true,x=>x.Expedition=0,x=>x.Relation=0}){
            f=new CreationAdapterFixture();change(f);result=f.Create();Check(result.Estado=="INCIERTO"&&result.Codigo=="RESULTADO_INCIERTO"&&f.Calls==1&&result.Creacion.CicloArchivoNombre=="Archivo Gestión"&&!PerfilImportacionJson.Escribir(result).Contains("SENTINEL"),"DOC111 failed/partial writer permits replay or leaks raw error");
            if(f.Expedition>0)Check(result.Expediente.ExpedienteId==f.Expedition.ToString(),"DOC111 uncertain result loses known expedition");if(f.Relation>0)Check(result.Expediente.RelacionNivelId==f.Relation.ToString(),"DOC111 uncertain result loses known relation");
        }
        f=new CreationAdapterFixture();f.Catalog.Source.Root.ValoresPlan[1].Valor="O'Brien";Rejected("CONFIGURACION_INVALIDA",()=>f.Create());Check(f.Starts==0&&f.Calls==0,"DOC111 invalid root reaches writer");
        f=new CreationAdapterFixture();f.Catalog.Ambiguous=true;Rejected("ACCESO_DENEGADO",()=>f.Create());Check(f.Starts==0&&f.Calls==0,"DOC111 ambiguous classification starts effect");
        Action<Action<CreationCommandFixture>> stale=change=>{var source=SafeCreationCatalog().Source;var cmd=source.Prepare();change(source);Rejected("CONFLICTO_REVISION",()=>RepositoryHelper("ValidarInicioCreacion",cmd,source.Lot,source.Root));};
        stale(x=>x.Lot.DetencionSolicitada=true);stale(x=>x.Root.Revision++);stale(x=>x.Root.IntentoActivoId="51");stale(x=>x.Root.ValoresPlan[0].Valor="changed");stale(x=>x.Lot.Snapshot.Configuracion.AreaRef="9");stale(x=>x.Lot.Contexto.NivelRef="9");
        Console.WriteLine("DOC-111 creation adapter starts once and preserves uncertain known IDs (explicit doubles): passed");
    }

    static void CreationResultSqlCases(){
        var f=SafeCreationCatalog();var command=f.Source.Prepare();var names=f.PrepareLegacy();var identity=new IdentidadExpedienteImportacion{OrigenLogico="RA",ExpedienteId="60",RelacionNivelId="70",NivelRef=command.Contexto.NivelRef};
        var rows=new DataTable();foreach(var col in new[]{"expediente","relacion","nivel","empresa","usuario","nivel_activo","electronico","empresa_nivel","autorizado","fondo","instrumento","area","serie","subserie","tipo","unidad","codigo","nombre","fecha_inicial","fecha_final","ciclo_id","ciclo_nombre","gabinete","responsable_nombre","responsable_identificacion","volumen"})rows.Columns.Add(col,typeof(object));
        rows.Rows.Add("60","70","3","2","1","1","2","2","1","1","2","3","4",DBNull.Value,"5","6","000123",command.Nombre,"2026-10-29","2026-10-30","13","Archivo Gestión","Gabinete","Responsable","00012345","1");
        rows.Columns.Add("alias_visible",typeof(object));rows.Rows[0]["alias_visible"]=command.Nombre;
        int calls=0;Func<string,object[],DataTable> query=(sql,args)=>{calls++;Check(sql.StartsWith("SELECT ")&&sql.EndsWith("LIMIT 2")&&sql.Contains("r.id_registro=@relacion")&&sql.Contains("e.ID_EXPEDIENTE=@expediente")&&!sql.Contains("FOR UPDATE")&&!sql.Contains("'000123'"),"DOC111 result lookup is not exact bounded SELECT");return rows;};
        Func<bool> check=()=>(bool)RepositoryHelper("LeerResultadoCreacion",command,identity,names,query);Check(check(),"DOC111 correct created result rejected");
        foreach(DataColumn col in rows.Columns){var saved=rows.Rows[0][col];rows.Rows[0][col]="999";Check(!check(),"DOC111 inconsistent created field accepted: "+col.ColumnName);if(saved!=DBNull.Value){rows.Rows[0][col]=DBNull.Value;Check(!check(),"DOC111 null created field accepted");}rows.Rows[0][col]=saved;}
        rows.ImportRow(rows.Rows[0]);Check(!check(),"DOC111 ambiguous created result accepted");rows.Rows.RemoveAt(1);var original=rows.Rows[0].ItemArray;rows.Clear();Check(!check(),"DOC111 missing created effect accepted");rows.Rows.Add(original);
        rows.Columns.Remove("ciclo_id");Unavailable(()=>check());
        Console.WriteLine("DOC-111 creation result checks destination classification cycle and effective values (synthetic SELECT): passed");
    }

    sealed class CreationCheckpointFixture {
        public CreationCommandFixture Source=SafeCreationCatalog().Source;
        public ComandoCreacionExpedienteImportacion Command;
        public IntentoImportacionCarpetas Attempt;
        public ResultadoEfectoImportacion Result;
        public List<KeyValuePair<string,object[]>> Writes=new List<KeyValuePair<string,object[]>>();public int Events,FailAt;public bool FailEvent;
        public CreationCheckpointFixture(){
            Command=Source.Prepare();Attempt=ReceiptClone(Source.Attempt);Attempt.Estado="EN_CURSO";Attempt.Resultado=new ResultadoEfectoImportacion{Estado="EN_CURSO",Codigo="CREACION_PREPARADA",Creacion=new EvidenciaCreacionExpedienteImportacion{IntentoId="50",CicloArchivoId="13",CicloArchivoNombre="Archivo Gestión"}};
            Result=new ResultadoEfectoImportacion{Estado="CONFIRMADO",Codigo="UNTRUSTED",Mensaje="DOC111_PRIVATE_INPUT_SENTINEL",Creacion=ReceiptClone(Attempt.Resultado.Creacion),Expediente=new IdentidadExpedienteImportacion{OrigenLogico="RA",ExpedienteId="60",RelacionNivelId="70",NivelRef=Command.Contexto.NivelRef}};
        }
        public ResultadoEfectoImportacion Save(){
            Func<string,object[],int> execute=(sql,args)=>{Writes.Add(new KeyValuePair<string,object[]>(sql,args));return Writes.Count==FailAt?0:1;};
            Action eventWriter=()=>{Check(Writes.Count==3,"DOC111 checkpoint event written before resource mutations");Events++;if(FailEvent)throw new PerfilImportacionException("PRUEBA_EVENTO","synthetic event failure");};
            return (ResultadoEfectoImportacion)RepositoryHelper("PersistirResultadoCreacion",Command,Source.Lot,Source.Root,Attempt,Result,new DateTime(2026,10,29,12,0,0,DateTimeKind.Utc),execute,eventWriter);
        }
        public object Arg(int write,string name){var args=Writes[write].Value;return args[Array.IndexOf(args,name)+1];}
        public void Terminal(ResultadoEfectoImportacion result){Attempt.Resultado=ReceiptClone(result);Attempt.Estado=result.Estado;Attempt.FinUtc=result.Estado=="CONFIRMADO"?"2026-10-29T12:00:00Z":null;Source.Root.Estado=result.Estado=="CONFIRMADO"?"EXPEDIENTE_CONFIRMADO":"INCIERTO";Source.Root.IdentidadExpediente=result.Estado=="CONFIRMADO"?ReceiptClone(result.Expediente):null;Source.Root.IntentoActivoId=result.Estado=="CONFIRMADO"?null:"50";Source.Root.Revision++;Writes.Clear();Events=0;}
    }

    static void CreationCheckpointCases(){
        var f=new CreationCheckpointFixture();var initial=PerfilImportacionJson.Escribir(new{f.Source.Lot,f.Source.Root,f.Attempt});var result=f.Save();
        Check(result.Estado=="CONFIRMADO"&&result.Codigo=="EXPEDIENTE_CONFIRMADO"&&!PerfilImportacionJson.Escribir(result).Contains("SENTINEL")&&f.Writes.Count==3&&f.Events==1,"DOC111 checkpoint not canonical or incomplete");
        Check(initial==PerfilImportacionJson.Escribir(new{f.Source.Lot,f.Source.Root,f.Attempt}),"DOC111 SQL core mutates caller resources before transaction commit");
        Check(f.Writes[0].Key.Contains("evidencia_doc=@evidencia")&&f.Writes[0].Key.Contains("estado='EN_CURSO'")&&f.Writes[0].Key.Contains("token_reclamacion=@token")&&f.Writes[0].Key.Contains("solicitud_hash=@hash")&&Convert.ToString(f.Arg(0,"@estado"))=="CONFIRMADO"&&f.Arg(0,"@fin") is DateTime,"DOC111 attempt checkpoint missing evidence/fencing or completion time");
        Check(Convert.ToString(f.Arg(1,"@estado"))=="EXPEDIENTE_CONFIRMADO"&&f.Arg(1,"@activo")==DBNull.Value&&Convert.ToString(f.Arg(1,"@identidad")).Contains("60")&&Convert.ToString(f.Arg(2,"@estado"))=="EN_CURSO","DOC111 confirmed checkpoint marks lot complete or loses root identity");
        f.Terminal(result);var again=f.Save();Check(again.Expediente.ExpedienteId=="60"&&f.Writes.Count==0&&f.Events==0,"DOC111 repeated checkpoint mutates resources or duplicates event");f.Result.Expediente.ExpedienteId="999";Rejected("CONFLICTO_REVISION",()=>f.Save());Check(f.Writes.Count==0,"DOC111 changed terminal result rewrites identity");
        f=new CreationCheckpointFixture();f.Result.Estado="INCIERTO";f.Result.Expediente.RelacionNivelId=null;result=f.Save();Check(f.Arg(0,"@fin")==DBNull.Value&&f.Arg(1,"@identidad")==DBNull.Value&&Convert.ToString(f.Arg(1,"@activo"))=="50"&&Convert.ToString(f.Arg(2,"@estado"))=="PENDIENTE_VERIFICACION"&&result.Expediente.ExpedienteId=="60","DOC111 uncertain checkpoint loses evidence, releases marker or fabricates root identity");
        f.Terminal(result);f.Save();Check(f.Writes.Count==0&&f.Events==0,"DOC111 repeated uncertain checkpoint mutates state");f.Result.Estado="CONFIRMADO";f.Result.Expediente.RelacionNivelId="70";Rejected("CONFLICTO_REVISION",()=>f.Save());Check(f.Writes.Count==0,"DOC111 checkpoint silently reconciles uncertainty");
        f=new CreationCheckpointFixture();f.Source.Lot.DetencionSolicitada=true;f.Save();Check(f.Source.Lot.DetencionSolicitada&&!f.Writes[2].Key.Contains("detencion_solicitada="),"DOC111 recording inflight result discards stop signal");
        foreach(var change in new Action<CreationCheckpointFixture>[] {x=>x.Attempt.TokenReclamacion=new string('0',32),x=>x.Attempt.SolicitudHash=new string('0',64),x=>x.Attempt.Estado="RESERVADO",x=>x.Attempt.RaizId="41",x=>x.Attempt.Fase="ALMACENAR",x=>x.Attempt.Resultado.Creacion.CicloArchivoId="99",x=>x.Source.Root.Revision++,x=>x.Source.Lot.Contexto.NivelRef="9",x=>x.Source.Lot.Snapshot.Configuracion.AreaRef="9",x=>x.Attempt.Resultado.Expediente=new IdentidadExpedienteImportacion{OrigenLogico="RA",NivelRef=x.Command.Contexto.NivelRef,ExpedienteId="999"}}){f=new CreationCheckpointFixture();change(f);Rejected("CONFLICTO_REVISION",()=>f.Save());Check(f.Writes.Count==0&&f.Events==0,"DOC111 foreign/stale checkpoint reaches SQL");}
        foreach(var change in new Action<CreationCheckpointFixture>[] {x=>x.Result.Expediente.NivelRef="9",x=>x.Result.Expediente.OrigenLogico="DA",x=>x.Result.Expediente.RelacionNivelId=null,x=>x.Result.DocumentoId="77",x=>x.Result.Creacion.CicloArchivoNombre="Archivo Central",x=>x.Result.Creacion=null}){f=new CreationCheckpointFixture();change(f);Rejected("SOLICITUD_INVALIDA",()=>f.Save());Check(f.Writes.Count==0,"DOC111 malformed creation result reaches SQL");}
        for(int failure=1;failure<=3;failure++){f=new CreationCheckpointFixture();f.FailAt=failure;Rejected("CONFLICTO_REVISION",()=>f.Save());Check(f.Writes.Count==failure&&f.Events==0,"DOC111 checkpoint continues after failed compare-and-set");}
        f=new CreationCheckpointFixture();f.FailEvent=true;Rejected("PRUEBA_EVENTO",()=>f.Save());Check(f.Events==1,"DOC111 failed audit event was swallowed");
        Console.WriteLine("DOC-111 creation checkpoint idempotence scope and uncertainty (synthetic SQL executor): passed");
    }

    sealed class CreationBarrierFixture {
        public PlanImportacionCarpetas Plan=PlanFixture();public LoteImportacionCarpetas Lot;public List<RaizImportacionCarpetas> Roots=new List<RaizImportacionCarpetas>();public List<ArchivoImportacionCarpetas> Files=new List<ArchivoImportacionCarpetas>();public int Target,Queries;
        public CreationBarrierFixture(){
            Plan.PlanId="17";Plan.Estado="CONFIRMADO";Lot=new LoteImportacionCarpetas{LoteId="20",PlanId="17",Estado="RECIBIENDO",Revision=1,Contexto=ReceiptClone(Plan.Contexto),Snapshot=ReceiptClone(Plan.Snapshot),TotalRaices=2,TotalArchivos=2,TotalBytes="14"};
            Roots.Add(new RaizImportacionCarpetas{RaizId="40",LoteId="20",ClienteRaizId="r1",NombreOriginal="Órdenes",Estado="PENDIENTE",Revision=1});Roots.Add(new RaizImportacionCarpetas{RaizId="41",LoteId="20",ClienteRaizId="r2",NombreOriginal="Órdenes",Estado="PENDIENTE",Revision=1});
            foreach(var root in Roots)Files.Add(new ArchivoImportacionCarpetas{ArchivoId=(30+Files.Count).ToString(),RaizId=root.RaizId,ClienteArchivoId="a1",BytesPrevistos="7",BytesRecibidos="7",ContenidoHash=new string('a',64),StagingRef=new string('b',32),EstadoRecepcion="RECIBIDO",EstadoEfecto="NO_INICIADO",Revision=1});
        }
        public void Policy(string mode,string scope){Plan.Snapshot.Configuracion.ModoIntegridad=mode;Plan.Snapshot.Configuracion.AmbitoIntegridad=scope;Lot.Snapshot=ReceiptClone(Plan.Snapshot);}
        bool GlobalScope(){return Plan.Snapshot.Configuracion.ModoIntegridad=="COMPLETO_OBLIGATORIO"&&Plan.Snapshot.Configuracion.AmbitoIntegridad=="LOTE";}
        List<RaizImportacionCarpetas> ScopedRoots(){return GlobalScope()?Roots:Roots.Where(r=>r.RaizId==Roots[Target].RaizId).ToList();}
        List<ArchivoImportacionCarpetas> ScopedFiles(){return GlobalScope()?Files:Files.Where(f=>f.RaizId==Roots[Target].RaizId).ToList();}
        public bool Check(){var method=typeof(CoordinadorImportacionCarpetas).GetMethod("EvaluarBarreraCreacion",BindingFlags.Static|BindingFlags.NonPublic);try{return (bool)method.Invoke(null,new object[]{Lot,Plan,Roots[Target],ScopedRoots(),ScopedFiles()});}catch(TargetInvocationException e){throw e.InnerException;}}
        public bool ReadCheck(){
            var roots=new DataTable();foreach(var col in new[]{"raiz_id","lote_id","cliente_raiz_id","estado"})roots.Columns.Add(col,typeof(object));foreach(var r in ScopedRoots())roots.Rows.Add(r.RaizId,r.LoteId,r.ClienteRaizId,r.Estado);
            var files=new DataTable();foreach(var col in new[]{"archivo_id","raiz_id","cliente_archivo_id","bytes_previstos","bytes_recibidos","contenido_hash","staging_ref","estado_recepcion","estado_efecto","intento_activo_id","revision"})files.Columns.Add(col,typeof(object));foreach(var f in ScopedFiles())files.Rows.Add(f.ArchivoId,f.RaizId,f.ClienteArchivoId,f.BytesPrevistos,f.BytesRecibidos==null?DBNull.Value:(object)f.BytesRecibidos,f.ContenidoHash==null?DBNull.Value:(object)Unhex(f.ContenidoHash),f.StagingRef==null?DBNull.Value:(object)f.StagingRef,f.EstadoRecepcion,f.EstadoEfecto,f.IntentoActivoId==null?DBNull.Value:(object)f.IntentoActivoId,f.Revision);
            Func<string,object[],DataTable> query=(sql,args)=>{Queries++;ImportacionCarpetasBehaviorTests.Check(sql.StartsWith("SELECT ")&&sql.Contains("r.lote_id=@lote")&&sql.EndsWith("LIMIT @limite")&&!sql.Contains("FOR UPDATE")&&!sql.Contains("origen_doc")&&!sql.Contains("datos_doc"),"DOC111 barrier query unscoped, unbounded or loads payload");ImportacionCarpetasBehaviorTests.Check(sql.Contains("r.raiz_id=@raiz")==!GlobalScope(),"DOC111 barrier query uses wrong integrity scope");bool rootQuery=sql.Contains("FROM ra_integracion_raiz r WHERE");ImportacionCarpetasBehaviorTests.Check((int)args[Array.IndexOf(args,"@limite")+1]==(rootQuery?Plan.Manifiesto.Raices.Count:Plan.Manifiesto.Archivos.Count)+1,"DOC111 barrier query lost original inventory bound");return rootQuery?roots:files;};
            return (bool)RepositoryHelper("LeerBarreraCreacion",Lot,Plan,Roots[Target],query);
        }
        public void AnotherFile(){Plan.Manifiesto.Archivos.Add(new ArchivoManifiestoImportacion{RaizIdCliente="r1",IdCliente="a2",Nombre="other.pdf",RutaRelativa="Órdenes/other.pdf",Bytes="7"});var file=ReceiptClone(Files[0]);file.ArchivoId="32";file.ClienteArchivoId="a2";Files.Add(file);Lot.TotalArchivos=3;Lot.TotalBytes="21";}
    }

    static void CreationBarrierCases(){
        var f=new CreationBarrierFixture();Check(f.Check()&&f.ReadCheck()&&f.Queries==2,"DOC111 full lot with homonymous roots not admitted");
        f.Files[1].EstadoRecepcion="PENDIENTE";Check(!f.Check()&&!f.ReadCheck(),"DOC111 LOTE starts before all files received");f.Policy("COMPLETO_OBLIGATORIO","EXPEDIENTE");Check(f.Check()&&f.ReadCheck(),"DOC111 EXPEDIENTE unnecessarily blocks ready independent root");f.Target=1;Check(!f.Check(),"DOC111 EXPEDIENTE starts incomplete root");
        f=new CreationBarrierFixture();f.Policy("PARCIAL_PERMITIDO","NO_APLICA");f.AnotherFile();f.Files[2].EstadoRecepcion="PENDIENTE";var original=PerfilImportacionJson.Escribir(new{f.Lot,f.Plan,f.Roots,f.Files});Check(f.Check()&&f.ReadCheck(),"DOC111 partial requires all eligible files before root creation");Check(original==PerfilImportacionJson.Escribir(new{f.Lot,f.Plan,f.Roots,f.Files})&&f.Lot.TotalArchivos==3&&f.Lot.TotalBytes=="21","DOC111 partial barrier modifies inventory/totals");
        f.Files[2].EstadoRecepcion="RECHAZADO";f.Files[2].EstadoEfecto="RECHAZADO";f.Plan.Resultado.Incidencias.Add(new IncidenciaImportacionCarpetas{RaizIdCliente="r1",ArchivoIdCliente="a2",BloqueaUnidad=true});Check(f.Check(),"DOC111 partial rejected file blocks independent eligible file");f.Files[0].EstadoRecepcion="PENDIENTE";Check(!f.Check(),"DOC111 partial creates root without any eligible received file");
        foreach(var change in new Action<CreationBarrierFixture>[] {x=>x.Lot.DetencionSolicitada=true,x=>x.Lot.TotalArchivos=1,x=>x.Lot.TotalBytes="7",x=>x.Plan.Estado="REVISABLE",x=>x.Plan.Resultado.PuedeConfirmar=false,x=>x.Lot.Snapshot.Configuracion.AmbitoIntegridad="EXPEDIENTE",x=>x.Roots[1].Estado="INCIERTO",x=>x.Files.RemoveAt(1),x=>x.Files[1].ArchivoId=x.Files[0].ArchivoId,x=>x.Files[1].RaizId="999",x=>x.Files[0].BytesRecibidos="6",x=>x.Files[0].ContenidoHash=null,x=>x.Files[0].StagingRef=null,x=>x.Files[0].IntentoActivoId="50",x=>x.Files[0].EstadoEfecto="INCIERTO",x=>x.Files[0].EstadoRecepcion="UNKNOWN",x=>x.Files[0].ClienteArchivoId="foreign",x=>x.Roots[1].ClienteRaizId="r1"}){f=new CreationBarrierFixture();change(f);Check(!f.Check(),"DOC111 inconsistent/missing barrier proof accepted");}
        f=new CreationBarrierFixture();f.Policy("PARCIAL_PERMITIDO","LOTE");Rejected("CONFIGURACION_INVALIDA",()=>f.Check());
        f=new CreationBarrierFixture();f.Roots[1].Estado="EXPEDIENTE_CONFIRMADO";f.Files[1].EstadoEfecto="COMPLETADO";Check(f.Check(),"DOC111 already confirmed other root invalidates original full-lot receipt barrier");
        Console.WriteLine("DOC-111 creation receipt barriers preserve inventory for EXPEDIENTE LOTE and partial (synthetic metadata): passed");
    }

    sealed class CreationStagingFixture : IDisposable {
        public ReceiptFixture Receipt=new ReceiptFixture();public VistaBarreraCreacionImportacion View;public ReclamacionImportacionCarpetas Command=ControlCommand();public Dictionary<string,ArchivoImportacionCarpetas> FullFiles=new Dictionary<string,ArchivoImportacionCarpetas>();public int Reservations,Stages;public bool RevisionChanged,Replay;
        public CreationStagingFixture(){
            using(var input=new StagingStream(Receipt.Pdf))Receipt.Service.Recibir(Receipt.Request,input);
            var lot=Receipt.Repository.Lot;lot.Snapshot.Configuracion.ModoIntegridad="COMPLETO_OBLIGATORIO";lot.Snapshot.Configuracion.AmbitoIntegridad="EXPEDIENTE";
            lot.Snapshot.Huella=Hex(PerfilImportacionJson.Huella(new VersionPerfilImportacion{Configuracion=lot.Snapshot.Configuracion,Reglas=lot.Snapshot.Reglas}));
            var root=new RaizImportacionCarpetas{RaizId="40",LoteId="20",ClienteRaizId="r1",NombreOriginal="Folder",Estado="PENDIENTE",Revision=1};
            var file=ReceiptClone(Receipt.Repository.File);FullFiles.Add(file.ArchivoId,file);
            var plan=new PlanImportacionCarpetas{PlanId=lot.PlanId,Estado="CONFIRMADO",Contexto=ReceiptClone(lot.Contexto),Snapshot=ReceiptClone(lot.Snapshot),Manifiesto=new ManifiestoImportacionCarpetas(),Resultado=new ResultadoPreparacionImportacion{PuedeConfirmar=true}};
            plan.Manifiesto.Raices.Add(new RaizManifiestoImportacion{IdCliente="r1",SeleccionId="s1",Nombre="Folder"});plan.Manifiesto.Archivos.Add(new ArchivoManifiestoImportacion{IdCliente=file.ClienteArchivoId,RaizIdCliente="r1",Nombre=file.NombreOriginal,RutaRelativa=file.RutaRelativa,Bytes=file.BytesPrevistos});plan.Resultado.UnidadesElegibles.Add("r1");
            View=new VistaBarreraCreacionImportacion{Lote=lot,Plan=plan,Raiz=root,Raices=new List<RaizImportacionCarpetas>{root},Archivos=new List<ArchivoImportacionCarpetas>{ReceiptClone(file)}};
        }
        public void Partial(){View.Lote.Snapshot.Configuracion.ModoIntegridad="PARCIAL_PERMITIDO";View.Lote.Snapshot.Configuracion.AmbitoIntegridad="NO_APLICA";View.Plan.Snapshot=ReceiptClone(View.Lote.Snapshot);}
        public void AddFile(){
            var file=ReceiptClone(FullFiles["30"]);file.ArchivoId="31";file.ClienteArchivoId="f2";file.NombreOriginal="second.pdf";file.RutaRelativa="Folder/second.pdf";file.EstadoRecepcion="PENDIENTE";file.StagingRef=null;file.ContenidoHash=null;file.BytesRecibidos=null;
            var attempt=new IntentoImportacionCarpetas{IntentoId="61",LoteId="20",RaizId="40",ArchivoId="31",Fase="RECEPCION",Estado="EN_CURSO",TokenReclamacion=Guid.NewGuid().ToString("N")};
            var store=new AlmacenStagingImportacion(Receipt.Root,Receipt.Limits,View.Lote,file,10,200);RecepcionVerificadaImportacion received;using(var input=new StagingStream(Receipt.Pdf))received=store.Recibir(View.Lote.Contexto,attempt,input,file.BytesPrevistos);
            file.EstadoRecepcion="RECIBIDO";file.StagingRef=received.StagingRef;file.ContenidoHash=received.ContenidoHash;file.BytesRecibidos=received.BytesRecibidos;FullFiles.Add(file.ArchivoId,file);View.Archivos.Add(ReceiptClone(file));View.Plan.Manifiesto.Archivos.Add(new ArchivoManifiestoImportacion{IdCliente="f2",RaizIdCliente="r1",Nombre=file.NombreOriginal,RutaRelativa=file.RutaRelativa,Bytes=file.BytesPrevistos});View.Lote.TotalArchivos=2;View.Lote.TotalBytes=((ulong)Receipt.Pdf.Length*2).ToString();
        }
        public string PathFor(string id){return Receipt.Completed().Single(p=>Path.GetFileName(p)==FullFiles[id].StagingRef+".bin");}
        public ReservaCreacionVerificadaImportacion Reserve(){
            Func<ContextoImportacionCarpetas,string,string,ArchivoImportacionCarpetas> load=(c,l,id)=>{var f=ReceiptClone(FullFiles[id]);if(RevisionChanged)f.Revision++;return f;};
            Func<LoteImportacionCarpetas,ArchivoImportacionCarpetas,IAlmacenStagingImportacion> stage=(l,f)=>{Stages++;return new AlmacenStagingImportacion(Receipt.Root,Receipt.Limits,l,f,10,200);};
            Action<ContextoImportacionCarpetas,ConfiguracionPerfilImportacion> authorize=(c,s)=>Receipt.Resources.Adapter.AutorizarRecursos(Receipt.Resources.Http,c,s,"CREAR_EXPEDIENTE");
            Func<ContextoImportacionCarpetas,ReclamacionImportacionCarpetas,IntentoImportacionCarpetas> reserve=(c,r)=>{Reservations++;Check(Stages>0,"DOC111 reserve before physical staging validation");return new IntentoImportacionCarpetas{IntentoId="50",LoteId=r.LoteId,RaizId=r.RecursoId,Fase=r.Fase,Estado=Replay?"EN_CURSO":"RESERVADO",EsNuevaReserva=!Replay,TokenReclamacion=r.TokenReclamacion,SolicitudHash=r.SolicitudHash};};
            var method=typeof(CoordinadorImportacionCarpetas).GetMethod("ReservarCreacionConStaging",BindingFlags.Static|BindingFlags.NonPublic);try{return (ReservaCreacionVerificadaImportacion)method.Invoke(null,new object[]{View,Command,load,stage,authorize,reserve});}catch(TargetInvocationException e){throw e.InnerException;}
        }
        public RetencionesStagingCreacionImportacion Keep(){
            Func<ContextoImportacionCarpetas,string,string,ArchivoImportacionCarpetas> load=(c,l,id)=>ReceiptClone(FullFiles[id]);
            Func<LoteImportacionCarpetas,ArchivoImportacionCarpetas,IAlmacenStagingImportacion> stage=(l,f)=>new AlmacenStagingImportacion(Receipt.Root,Receipt.Limits,l,f,10,200);
            Action<ContextoImportacionCarpetas,ConfiguracionPerfilImportacion> authorize=(c,s)=>Receipt.Resources.Adapter.AutorizarRecursos(Receipt.Resources.Http,c,s,"CREAR_EXPEDIENTE");
            var method=typeof(CoordinadorImportacionCarpetas).GetMethod("ConservarStagingCreacion",BindingFlags.Static|BindingFlags.NonPublic);try{return (RetencionesStagingCreacionImportacion)method.Invoke(null,new object[]{View,load,stage,authorize});}catch(TargetInvocationException e){throw e.InnerException;}
        }
        public void Dispose(){Receipt.Dispose();}
    }

    static void PhysicalCreationStagingCases(){
        using(var f=new CreationStagingFixture()){
            var path=f.PathFor("30");using(var held=f.Keep()){bool writeBlocked=false,deleteBlocked=false;try{using(var output=new FileStream(path,FileMode.Open,FileAccess.Write,FileShare.Read)){}}catch(IOException){writeBlocked=true;}try{File.Delete(path);}catch(IOException){deleteBlocked=true;}Check(writeBlocked&&deleteBlocked&&held.Resultado.ArchivosVerificados.Single()=="30","DOC111 retained read proof permits write/delete or loses identity");}File.WriteAllBytes(path,f.Receipt.Pdf);
        }
        using(var f=new CreationStagingFixture()){
            f.AddFile();var retained=f.PathFor("30");var missing=f.PathFor("31");File.Move(missing,missing+".missing");Rejected("RESULTADO_INCIERTO",()=>f.Keep());File.WriteAllBytes(retained,f.Receipt.Pdf);
        }
        using(var f=new CreationStagingFixture()){
            var original=File.ReadAllBytes(f.PathFor("30"));var metadata=PerfilImportacionJson.Escribir(f.View);var result=f.Reserve();Check(f.Reservations==1&&result.Intento.EsNuevaReserva&&result.ArchivosVerificados.SequenceEqual(new[]{"30"})&&result.ArchivosInciertos.Count==0&&metadata==PerfilImportacionJson.Escribir(f.View)&&File.ReadAllBytes(f.PathFor("30")).SequenceEqual(original),"DOC111 physical verification changes received evidence or reserves without proof");
        }
        using(var f=new CreationStagingFixture()){
            var path=f.PathFor("30");File.Move(path,path+".missing");Rejected("RESULTADO_INCIERTO",()=>f.Reserve());Check(f.Reservations==0,"DOC111 missing retained content authorizes creation reservation");File.Move(path+".missing",path);
            var changed=(byte[])f.Receipt.Pdf.Clone();changed[20]^=1;File.WriteAllBytes(path,changed);Rejected("RESULTADO_INCIERTO",()=>f.Reserve());Check(f.Reservations==0&&f.View.Archivos[0].EstadoRecepcion=="RECIBIDO","DOC111 changed physical content reserves or rewrites positive reception");
        }
        using(var f=new CreationStagingFixture()){
            f.RevisionChanged=true;Rejected("CONFLICTO_REVISION",()=>f.Reserve());Check(f.Reservations==0&&f.Stages==0,"DOC111 stale metadata reaches physical verification/reservation");
        }
        using(var f=new CreationStagingFixture()){
            f.Receipt.Resources.ImportAllowed=false;Rejected("ACCESO_DENEGADO",()=>f.Reserve());Check(f.Reservations==0&&f.Stages==0,"DOC111 revoked authority reads staging");
        }
        using(var f=new CreationStagingFixture()){
            f.Partial();f.AddFile();var bad=f.PathFor("30");File.Move(bad,bad+".missing");var result=f.Reserve();Check(f.Reservations==1&&result.ArchivosVerificados.SequenceEqual(new[]{"31"})&&result.ArchivosInciertos.SequenceEqual(new[]{"30"})&&f.View.Lote.TotalArchivos==2&&f.View.Archivos.Count==2,"DOC111 partial cannot advance valid content independently or shrinks inventory");
        }
        using(var f=new CreationStagingFixture()){
            f.Partial();f.AddFile();foreach(var id in new[]{"30","31"}){var bad=f.PathFor(id);File.Move(bad,bad+".missing");}Rejected("RESULTADO_INCIERTO",()=>f.Reserve());Check(f.Reservations==0,"DOC111 partial reserves without any physical file");
        }
        using(var f=new CreationStagingFixture()){
            var lotDirectory=Path.GetDirectoryName(Path.GetDirectoryName(f.PathFor("30")));using(var held=new FileStream(Path.Combine(lotDirectory,"recepcion.lock"),FileMode.Open,FileAccess.ReadWrite,FileShare.None)){Rejected("OPERACION_EN_CURSO",()=>f.Reserve());Check(f.Reservations==0,"DOC111 busy staging treated as a skippable file error");}
        }
        using(var f=new CreationStagingFixture()){f.Command.RecursoId="41";Rejected("SOLICITUD_INVALIDA",()=>f.Reserve());Check(f.Reservations==0&&f.Stages==0,"DOC111 foreign reservation scans staging");}
        using(var f=new CreationStagingFixture()){f.Replay=true;var result=f.Reserve();Check(!result.Intento.EsNuevaReserva,"DOC111 physical recheck turns historical reservation into new execution permission");}
        Console.WriteLine("DOC-111 physical retained staging checked before creation reservation (local PDF and reservation double): passed");
    }

    sealed class CreationPort : ICreacionExpedienteImportacion {
        public Func<ContextoImportacionCarpetas,LoteImportacionCarpetas,RaizImportacionCarpetas,IntentoImportacionCarpetas,ResultadoEfectoImportacion> Run;
        public ResultadoEfectoImportacion Crear(ContextoImportacionCarpetas c,LoteImportacionCarpetas l,RaizImportacionCarpetas r,IntentoImportacionCarpetas i){return Run(c,l,r,i);}
    }
    sealed class CreationPhaseFixture : IControlLoteImportacionRepository,IDisposable {
        public CreationStagingFixture Stage=new CreationStagingFixture();public IntentoImportacionCarpetas Attempt;public int Reserves,Writers,Checkpoints,Factories,StageFactories,Leases;public bool StopAfterReserve,CorruptAfterReserve,ThrowWriter,LoseCommitResponse,FailCheckpoint,RevokeAfterReserve;
        public CoordinadorImportacionCarpetas Coordinator;
        public CreationPhaseFixture(){
            var source=new CreationCommandFixture();var snapshot=Stage.View.Lote.Snapshot;var origin=snapshot.Configuracion.HuellaOrigenGabinete;snapshot.Configuracion=ReceiptClone(source.Lot.Snapshot.Configuracion);snapshot.Configuracion.HuellaOrigenGabinete=origin;snapshot.Configuracion.AmbitoIntegridad="EXPEDIENTE";snapshot.Reglas=ReceiptClone(source.Lot.Snapshot.Reglas);
            Stage.View.Raiz.ValoresPlan=ReceiptClone(source.Root.ValoresPlan);Stage.View.Raiz.ValoresPlan.Single(v=>v.DestinoRef=="NOMBRE").Valor="Folder";
            foreach(var rule in snapshot.Reglas){rule.FuenteTipo="CONSTANTE";if(rule.DestinoTipo=="CAMPO_EXPEDIENTE")rule.ValorConstante=Stage.View.Raiz.ValoresPlan.Single(v=>v.DestinoRef==rule.DestinoRef).Valor;if(rule.TipoValor=="FECHA")rule.FormatoFecha="yyyy-MM-dd";}
            foreach(var value in Stage.View.Raiz.ValoresPlan)value.FuenteTipo="CONSTANTE";
            snapshot.Huella=Hex(PerfilImportacionJson.Huella(new VersionPerfilImportacion{Configuracion=snapshot.Configuracion,Reglas=snapshot.Reglas}));Stage.View.Plan.Snapshot=ReceiptClone(snapshot);
            Func<ContextoImportacionCarpetas,string,string,VistaBarreraCreacionImportacion> view=(c,l,r)=>{Owner(c,l);Check(r=="40","DOC111 phase changes target root");return ReceiptClone(Stage.View);};
            Func<ContextoImportacionCarpetas,string,string,ArchivoImportacionCarpetas> file=(c,l,id)=>{Owner(c,l);return ReceiptClone(Stage.FullFiles[id]);};
            Func<ContextoImportacionCarpetas,ReclamacionImportacionCarpetas,IntentoImportacionCarpetas> history=(c,r)=>{Owner(c,r.LoteId);return Attempt!=null&&Attempt.SolicitudHash==r.SolicitudHash?ReceiptClone(Attempt):null;};
            Func<LoteImportacionCarpetas,ArchivoImportacionCarpetas,IAlmacenStagingImportacion> staging=(l,f)=>{StageFactories++;return new AlmacenStagingImportacion(Stage.Receipt.Root,Stage.Receipt.Limits,l,f,10,200);};
            Coordinator=new CoordinadorImportacionCarpetas(()=>Stage.Receipt.Resources.Adapter.Resolver(Stage.Receipt.Resources.Http),this,n=>Stage.Receipt.Resources.Adapter.ResolverDestino(Stage.Receipt.Resources.Http,n,"CREAR_EXPEDIENTE"),Authorize,view,file,history,staging,CreateAdapter);
        }
        void Owner(ContextoImportacionCarpetas c,string l){Check(l=="20"&&c.UsuarioRef=="1"&&c.AmbitoRef=="2"&&c.NivelRef=="7","DOC111 phase escaped owner/destination");}
        void Authorize(ContextoImportacionCarpetas c,ConfiguracionPerfilImportacion s){Stage.Receipt.Resources.Adapter.AutorizarRecursos(Stage.Receipt.Resources.Http,c,s,"CREAR_EXPEDIENTE");}
        public ContextoImportacionCarpetas ObtenerContextoLote(ContextoImportacionCarpetas c,string id){Check(c.UsuarioRef=="1"&&c.AmbitoRef=="2"&&id=="20","DOC111 phase identity changed");return ReceiptClone(Stage.View.Lote.Contexto);}
        public LoteImportacionCarpetas ObtenerLote(ContextoImportacionCarpetas c,string id){Owner(c,id);return ReceiptClone(Stage.View.Lote);}
        public IntentoImportacionCarpetas ReservarIntento(ContextoImportacionCarpetas c,ReclamacionImportacionCarpetas r){
            Owner(c,r.LoteId);Reserves++;Check(Attempt==null&&r.RecursoId=="40"&&r.Fase=="CREAR_EXPEDIENTE"&&r.RevisionEsperada==Stage.View.Raiz.Revision,"DOC111 phase reserves another effect/root");
            Attempt=new IntentoImportacionCarpetas{IntentoId="50",LoteId="20",RaizId="40",Fase=r.Fase,Estado="RESERVADO",TokenReclamacion=r.TokenReclamacion,SolicitudHash=r.SolicitudHash,EjecutorRef=r.EjecutorRef};Stage.View.Raiz.Estado="CREANDO";Stage.View.Raiz.IntentoActivoId="50";Stage.View.Raiz.Revision++;Stage.View.Lote.Estado="EN_CURSO";Stage.View.Lote.Revision++;
            if(StopAfterReserve)Stage.View.Lote.DetencionSolicitada=true;if(CorruptAfterReserve){var p=Stage.PathFor("30");File.Move(p,p+".missing");}if(RevokeAfterReserve)Stage.Receipt.Resources.ImportAllowed=false;
            var result=ReceiptClone(Attempt);result.EsNuevaReserva=true;return result;
        }
        public LoteImportacionCarpetas SolicitarDetencion(ContextoImportacionCarpetas c,string id,uint rev){Owner(c,id);Stage.View.Lote.DetencionSolicitada=true;return ReceiptClone(Stage.View.Lote);}
        ICreacionExpedienteImportacion CreateAdapter(Func<IDisposable> keep){
            Factories++;Action<ComandoCreacionExpedienteImportacion> authorize=c=>Authorize(c.Contexto,c.Snapshot.Configuracion);
            Func<ComandoCreacionExpedienteImportacion,Dictionary<string,string>> catalogs=c=>new Dictionary<string,string>{{"ORGANIGRAMA_ID","11"},{"ORGANIGRAMA_NOMBRE","Org"},{"FONDO_NOMBRE","Fondo"},{"AREA_NOMBRE","Area"},{"SERIE_NOMBRE","Serie"},{"SUBSERIE_NOMBRE",null},{"TIPO_EXPEDIENTE_NOMBRE","Tipo"},{"UNIDAD_DOCUMENTAL_NOMBRE","Unidad"},{"GABINETE_NOMBRE","Gabinete"},{"RESPONSABLE_NOMBRE","Responsable"},{"RESPONSABLE_IDENTIFICACION","00012345"},{"CICLO_ARCHIVO_ID","13"},{"CICLO_ARCHIVO_NOMBRE","Archivo Gestión"}};
            Func<ComandoCreacionExpedienteImportacion,Dictionary<string,string>,bool> start=(c,d)=>{RepositoryHelper("ValidarInicioCreacion",c,Stage.View.Lote,Stage.View.Raiz);if(Attempt.Estado!="RESERVADO")return false;Attempt.Estado="EN_CURSO";Attempt.Resultado=new ResultadoEfectoImportacion{Estado="EN_CURSO",Codigo="CREACION_PREPARADA",Creacion=new EvidenciaCreacionExpedienteImportacion{IntentoId="50",CicloArchivoId="13",CicloArchivoNombre="Archivo Gestión"}};Stage.View.Lote.Revision++;return true;};
            Action<ComandoCreacionExpedienteImportacion,Dictionary<string,string>,SalidaCreacionExpedienteLegacy> writer=(c,d,s)=>{Writers++;Check(Leases==1&&Attempt.Estado=="EN_CURSO"&&c.Contexto.NivelRef=="7"&&c.Codigo=="000123","DOC111 phase writer before retained proof or wrong command");var path=Stage.PathFor("30");bool blocked=false;try{File.Delete(path);}catch(IOException){blocked=true;}Check(blocked,"DOC111 phase releases original before writer");s.ExpedienteId=60;s.RelacionNivelId=70;s.Respuesta="YES";if(ThrowWriter)throw new Exception("DOC111_PRIVATE_WRITER_SENTINEL");};
            Func<ComandoCreacionExpedienteImportacion,IdentidadExpedienteImportacion,Dictionary<string,string>,bool> verify=(c,i,d)=>true;
            Func<ComandoCreacionExpedienteImportacion,ResultadoEfectoImportacion,ResultadoEfectoImportacion> save=(c,r)=>{
                if(FailCheckpoint)throw new Exception("DOC111_PRIVATE_CHECKPOINT_SENTINEL");int writes=0,events=0;Func<string,object[],int> execute=(sql,args)=>{writes++;return 1;};Action record=()=>events++;
                var normal=(ResultadoEfectoImportacion)RepositoryHelper("PersistirResultadoCreacion",c,Stage.View.Lote,Stage.View.Raiz,Attempt,r,new DateTime(2026,10,29,12,0,0,DateTimeKind.Utc),execute,record);Check(writes==3&&events==1,"DOC111 phase checkpoint incomplete");Checkpoints++;Attempt.Resultado=ReceiptClone(normal);Attempt.Estado=normal.Estado;Attempt.FinUtc=normal.Estado=="CONFIRMADO"?"2026-10-29T12:00:00Z":null;Stage.View.Raiz.Estado=normal.Estado=="CONFIRMADO"?"EXPEDIENTE_CONFIRMADO":"INCIERTO";Stage.View.Raiz.IdentidadExpediente=normal.Estado=="CONFIRMADO"?ReceiptClone(normal.Expediente):null;Stage.View.Raiz.IntentoActivoId=normal.Estado=="CONFIRMADO"?null:"50";Stage.View.Raiz.Revision++;if(normal.Estado=="INCIERTO")Stage.View.Lote.Estado="PENDIENTE_VERIFICACION";Stage.View.Lote.Revision++;if(LoseCommitResponse)throw new Exception("DOC111_PRIVATE_LOST_RESPONSE_SENTINEL");return normal;
            };
            Func<IDisposable> retained=()=>{var proof=keep();Leases++;return proof;};
            var constructor=typeof(AdaptadorExpedienteImportacion).GetConstructors(BindingFlags.NonPublic|BindingFlags.Instance).Single();return (AdaptadorExpedienteImportacion)constructor.Invoke(new object[]{authorize,catalogs,start,writer,verify,save,retained});
        }
        public ResultadoFaseCreacionImportacion Run(){return Coordinator.CrearExpediente("20","40",1,"stable-command");}
        public void Dispose(){Stage.Dispose();}
    }

    static void CoordinatedCreationCases(){
        using(var f=new CreationPhaseFixture()){
            var result=f.Run();Check(result.Resultado.Estado=="CONFIRMADO"&&result.Resultado.Expediente.ExpedienteId=="60"&&result.IntentoId=="50"&&result.ArchivosVerificados.Single()=="30"&&f.Reserves==1&&f.Writers==1&&f.Checkpoints==1,"DOC111 composed phase not exactly one verified creation");var stages=f.StageFactories;var repeated=f.Run();Check(repeated.Repeticion&&repeated.Resultado.Expediente.ExpedienteId=="60"&&f.Writers==1&&f.Reserves==1&&f.StageFactories==stages,"DOC111 repeated phase rehashes/reserves/writes");File.WriteAllBytes(f.Stage.PathFor("30"),f.Stage.Receipt.Pdf);
        }
        using(var f=new CreationPhaseFixture()){f.LoseCommitResponse=true;var result=f.Run();Check(result.Resultado.Estado=="CONFIRMADO"&&f.Writers==1&&f.Checkpoints==1,"DOC111 phase repeats writer or loses durable checkpoint after response loss");}
        using(var f=new CreationPhaseFixture()){f.FailCheckpoint=true;var result=f.Run();Check(result.Resultado.Estado=="INCIERTO"&&result.Resultado.Expediente.ExpedienteId=="60"&&f.Writers==1&&f.Attempt.Estado=="EN_CURSO","DOC111 failed checkpoint loses known IDs or grants replay");var repeated=f.Run();Check(repeated.Resultado.Estado=="INCIERTO"&&f.Writers==1,"DOC111 uncertain phase repeats writer");}
        using(var f=new CreationPhaseFixture()){f.ThrowWriter=true;var result=f.Run();Check(result.Resultado.Estado=="INCIERTO"&&result.Resultado.Expediente.ExpedienteId=="60"&&f.Checkpoints==1&&!PerfilImportacionJson.Escribir(result).Contains("SENTINEL"),"DOC111 writer error loses evidence or leaks private text");}
        using(var f=new CreationPhaseFixture()){f.StopAfterReserve=true;var result=f.Run();Check(result.Resultado.Estado=="INCIERTO"&&f.Writers==0&&f.Factories==0&&f.Attempt.Estado=="RESERVADO"&&f.Stage.View.Lote.DetencionSolicitada,"DOC111 stop after reserve starts effect or hides active reservation");}
        using(var f=new CreationPhaseFixture()){f.CorruptAfterReserve=true;var result=f.Run();Check(result.Resultado.Estado=="INCIERTO"&&f.Writers==0&&f.Checkpoints==1,"DOC111 staging lost after reserve still starts writer");}
        using(var f=new CreationPhaseFixture()){f.RevokeAfterReserve=true;var result=f.Run();Check(result.Resultado.Estado=="INCIERTO"&&f.Writers==0&&f.Factories==0,"DOC111 permission revoked after reserve starts writer");}
        using(var f=new CreationPhaseFixture()){f.Stage.View.Archivos[0].EstadoRecepcion="PENDIENTE";Rejected("OPERACION_EN_CURSO",()=>f.Run());Check(f.Reserves==0&&f.Writers==0,"DOC111 incomplete reception reaches reservation/effect");}
        Console.WriteLine("DOC-111 coordinated single creation phase retained staging and durable replay (local PDF and persistence/writer doubles): passed");
    }

    static void ProductionArchiveCycleCases(){
        var f=SafeCreationCatalog();var command=f.Source.Prepare();var names=f.PrepareLegacy();
        Check(command.CicloArchivoNombre=="Archivo Gestión"&&names["CICLO_ARCHIVO_ID"]=="13"&&names["CICLO_ARCHIVO_NOMBRE"]==command.CicloArchivoNombre,"DOC111 approved production archive cycle not bound to catalog");
        f.Target="CICLO_ARCHIVO";f.Missing=true;Rejected("ACCESO_DENEGADO",()=>f.PrepareLegacy());f.Missing=false;
        f.Ambiguous=true;Rejected("ACCESO_DENEGADO",()=>f.PrepareLegacy());f.Ambiguous=false;
        f.ChangedName="Archivo Central";Rejected("ACCESO_DENEGADO",()=>f.PrepareLegacy());f.ChangedName=null;
        f.ReverseId="0";Rejected("ACCESO_DENEGADO",()=>f.PrepareLegacy());f.ReverseId="2147483648";Rejected("CONFIGURACION_INVALIDA",()=>f.PrepareLegacy());f.ReverseId=null;
        f.NoColumn=true;Unavailable(()=>f.PrepareLegacy());f.NoColumn=false;
        names=f.PrepareLegacy();names["CICLO_ARCHIVO_NOMBRE"]="Archivo Central";Rejected("CONFIGURACION_INVALIDA",()=>command.ValidarDatosLegacy(names));
        names["CICLO_ARCHIVO_NOMBRE"]="Archivo Gestión";names.Remove("CICLO_ARCHIVO_ID");Rejected("CONFIGURACION_INVALIDA",()=>command.ValidarDatosLegacy(names));
        Console.WriteLine("DOC-111 approved production archive cycle requires exact unambiguous catalog row (synthetic SELECT): passed");
    }

    static void CreationCommandCases() {
        var f=new CreationCommandFixture();var before=PerfilImportacionJson.Escribir(new{f.Context,f.Lot,f.Root,f.Attempt});var command=f.Prepare();
        Check(command.Codigo=="000123"&&command.Nombre=="Órdenes d'Ávila"&&command.FechaInicial=="2026-10-29"&&command.FechaFinal=="2026-10-30"&&command.ResponsableRef=="8","DOC111 creation command re-resolves, renames or converts effective values");
        Check(command.LoteId=="20"&&command.RaizId=="40"&&command.IntentoId=="50"&&command.Contexto.NivelRef==f.Context.NivelRef&&command.RevisionRaiz==2&&command.TokenReclamacion==f.Attempt.TokenReclamacion,"DOC111 command loses fixed destination or reservation");
        Check(PerfilImportacionJson.Escribir(new{f.Context,f.Lot,f.Root,f.Attempt})==before,"DOC111 command preparation mutates confirmed resources");
        command.Contexto.NivelRef="999";command.Snapshot.Configuracion.AreaRef="999";command.Valores[0].Valor="999";
        f.Context.NivelRef="888";f.Lot.Snapshot.Configuracion.AreaRef="888";f.Root.ValoresPlan[0].Valor="888";
        Check(command.Contexto.NivelRef=="3"&&command.Snapshot.Configuracion.AreaRef=="3"&&command.Codigo=="000123"&&command.Valores[0].Valor=="000123"&&command.Valores[0].Procedencia=="fixture:already-resolved","DOC111 command shares mutable source/output state");
        Action<string,Action<CreationCommandFixture>> invalid=(code,change)=>{var item=new CreationCommandFixture();change(item);Rejected(code,()=>item.Prepare());};
        invalid("ACCESO_DENEGADO",x=>x.Context.PuedeImportar=false);
        invalid("ACCESO_DENEGADO",x=>x.Context.UsuarioRef="9");invalid("ACCESO_DENEGADO",x=>x.Context.AmbitoRef="9");invalid("ACCESO_DENEGADO",x=>x.Context.NivelRef="9");invalid("ACCESO_DENEGADO",x=>x.Context.SelloDestino="moved");invalid("ACCESO_DENEGADO",x=>x.Context.RutaCanonica="moved");
        invalid("CONFLICTO_REVISION",x=>x.Lot.DetencionSolicitada=true);invalid("CONFLICTO_REVISION",x=>x.Lot.Estado="DETENIDO");
        invalid("CONFLICTO_REVISION",x=>x.Root.LoteId="21");invalid("CONFLICTO_REVISION",x=>x.Root.Estado="INCIERTO");invalid("CONFLICTO_REVISION",x=>x.Root.IntentoActivoId="51");invalid("CONFLICTO_REVISION",x=>x.Root.IdentidadExpediente=new IdentidadExpedienteImportacion());
        invalid("CONFLICTO_REVISION",x=>x.Attempt.EsNuevaReserva=false);invalid("CONFLICTO_REVISION",x=>x.Attempt.RaizId="41");invalid("CONFLICTO_REVISION",x=>x.Attempt.LoteId="21");invalid("CONFLICTO_REVISION",x=>x.Attempt.ArchivoId="30");invalid("CONFLICTO_REVISION",x=>x.Attempt.Fase="ALMACENAR");invalid("CONFLICTO_REVISION",x=>x.Attempt.Estado="INCIERTO");invalid("CONFLICTO_REVISION",x=>x.Attempt.Resultado=new ResultadoEfectoImportacion());invalid("CONFLICTO_REVISION",x=>x.Attempt.TokenReclamacion="invalid");
        invalid("CONFIGURACION_INVALIDA",x=>x.Lot.Snapshot.Huella=new string('0',64));invalid("CONFIGURACION_INVALIDA",x=>x.Lot.Snapshot.Reglas[0]=null);
        invalid("CONFIGURACION_INVALIDA",x=>x.Root.ValoresPlan.Add(ReceiptClone(x.Root.ValoresPlan[0])));
        invalid("CONFIGURACION_INVALIDA",x=>x.Root.ValoresPlan[1]=ReceiptClone(x.Root.ValoresPlan[0]));
        invalid("CONFIGURACION_INVALIDA",x=>x.Root.ValoresPlan.RemoveAt(0));invalid("CONFIGURACION_INVALIDA",x=>x.Root.ValoresPlan[0].Diferido=true);invalid("CONFIGURACION_INVALIDA",x=>x.Root.ValoresPlan[0].PropiedadPendiente="ID");invalid("CONFIGURACION_INVALIDA",x=>x.Root.ValoresPlan[0].TipoValor="ENTERO");invalid("CONFIGURACION_INVALIDA",x=>x.Root.ValoresPlan[0].FuenteTipo="ARCHIVO");
        invalid("CONFIGURACION_INVALIDA",x=>x.Root.ValoresPlan[0].Valor=new string('0',46));invalid("CONFIGURACION_INVALIDA",x=>x.Root.ValoresPlan[1].Valor=" ");invalid("CONFIGURACION_INVALIDA",x=>x.Root.ValoresPlan[2].Valor="29/10/2026");invalid("CONFIGURACION_INVALIDA",x=>x.Root.ValoresPlan[2].Valor="2026-02-30");invalid("CONFIGURACION_INVALIDA",x=>x.Root.ValoresPlan[3].Valor="2026-10-28");invalid("CONFIGURACION_INVALIDA",x=>x.Root.ValoresPlan[4].Valor="08");
        f=new CreationCommandFixture();f.Lot.Snapshot.Reglas.RemoveAll(r=>r.DestinoRef=="FECHA_FINAL"||r.DestinoRef=="RESPONSABLE");f.Root.ValoresPlan.RemoveAll(v=>v.DestinoRef=="FECHA_FINAL"||v.DestinoRef=="RESPONSABLE");f.Rehash();command=f.Prepare();Check(command.FechaFinal==null&&command.ResponsableRef==null,"DOC111 optional values fabricated");
        Console.WriteLine("DOC-111 immutable creation command from confirmed root values (no effects): passed");
    }

    static void KnownExpedientClassificationCases() {
        var context=PlanFixture().Contexto;
        var identity=new IdentidadExpedienteImportacion{OrigenLogico="RA",ExpedienteId="60",RelacionNivelId="70",NivelRef=context.NivelRef};
        var config=new ConfiguracionPerfilImportacion{FondoRef="1",InstrumentoRef="2",AreaRef="3",SerieRef="4",SubserieRef="5",TipoExpedienteRef="6",UnidadDocumentalRef="7"};
        var rows=new DataTable();foreach(var name in new[]{"expediente","relacion","nivel","empresa","usuario","nivel_activo","electronico","empresa_nivel","autorizado","fondo","instrumento","area","serie","subserie","tipo","unidad"})rows.Columns.Add(name,typeof(object));
        rows.Rows.Add("60","70",context.NivelRef,context.AmbitoRef,context.UsuarioRef,"1","2",context.AmbitoRef,"1","1","2","3","4","5","6","7");
        int reads=0;Func<string,object[],DataTable> query=(sql,args)=>{
            reads++;Check(sql.StartsWith("SELECT ")&&sql.EndsWith("LIMIT 2")&&!sql.Contains("FOR UPDATE")&&sql.Contains("r.id_registro=@relacion")&&sql.Contains("e.ID_EXPEDIENTE=@expediente"),"DOC111 classification loses exact bounded read");
            foreach(var field in new[]{"ID_FONDO AS fondo","id_instrumento AS instrumento","CODIGO_AREA_TRD AS area","CODIGO_SERIE_TRD AS serie","CODIGO_SUB_SERIE_TRD AS subserie","RA_TIP_EXPE_ID_TIPO_EXPEDIENTE AS tipo","ID_TIPO_UNIDAD_DOCUMENTAL AS unidad"})Check(sql.Contains(field),"DOC111 persisted classification field omitted: "+field);
            return rows;
        };
        Func<bool> check=()=>(bool)RepositoryHelper("LeerClasificacionExpedienteConocido",context,identity,config,query);
        Check(check(),"DOC111 matching persisted classification rejected");
        foreach(DataColumn column in rows.Columns){var prior=rows.Rows[0][column];rows.Rows[0][column]="999";Check(!check(),"DOC111 different classification/scope accepted: "+column.ColumnName);rows.Rows[0][column]=DBNull.Value;Check(!check(),"DOC111 missing classification/scope accepted: "+column.ColumnName);rows.Rows[0][column]=prior;}
        rows.ImportRow(rows.Rows[0]);Check(!check(),"DOC111 ambiguous classification accepted");rows.Rows.RemoveAt(1);
        var saved=rows.Rows[0].ItemArray;rows.Clear();Check(!check(),"DOC111 absent classified expedition accepted");rows.Rows.Add(saved);
        config.SubserieRef=null;Check(!check(),"DOC111 unexpected subseries ignored");rows.Rows[0]["subserie"]=DBNull.Value;Check(check(),"DOC111 SQL NULL absent subseries rejected");
        foreach(var wrong in new[]{"0","","5"}){rows.Rows[0]["subserie"]=wrong;Check(!check(),"DOC111 absent subseries normalized from wrong value");}rows.Rows[0]["subserie"]=DBNull.Value;
        var before=reads;config.SerieRef="4 OR 1=1";Rejected("SOLICITUD_INVALIDA",()=>check());Check(reads==before,"DOC111 malformed classification reaches SQL");config.SerieRef="4";
        config.SubserieRef="0";Rejected("SOLICITUD_INVALIDA",()=>check());config.SubserieRef=null;
        config.FondoRef=null;Rejected("SOLICITUD_INVALIDA",()=>check());config.FondoRef="1";
        Rejected("SOLICITUD_INVALIDA",()=>RepositoryHelper("LeerClasificacionExpedienteConocido",context,identity,null,query));
        identity.NivelRef="999";before=reads;Check(!check()&&reads==before,"DOC111 classified effect redirected to another level");identity.NivelRef=context.NivelRef;
        context.PuedeImportar=false;Rejected("ACCESO_DENEGADO",()=>check());context.PuedeImportar=true;
        rows.Columns.Remove("serie");Unavailable(()=>check());
        Console.WriteLine("DOC-111 known expedition classification and optional subseries (synthetic SELECT records): passed");
    }

    static void QuerySqlCases() {
        var lot=new QueryRepository().Lot;var rootRows=new DataTable();foreach(var name in new[]{"raiz_id","lote_id","cliente_raiz_id","datos_doc","estado","revision","intento_activo_id","identidad_expediente_doc","valores_efectivos_doc"})rootRows.Columns.Add(name,typeof(object));
        rootRows.Rows.Add("40","20","r1",PerfilImportacionJson.Escribir(new{Esquema=1,IdCliente="r1",SeleccionId="s1",NombreOriginal="Folder",ValoresPlan=new List<ValorReglaPerfilImportacion>()}),"INCIERTO",1,DBNull.Value,DBNull.Value,DBNull.Value);
        var source=new ArchivoManifiestoImportacion{IdCliente="f1",RaizIdCliente="r1",Nombre="doc.pdf",RutaRelativa="Folder/doc.pdf",Bytes="7"};
        var fileRows=new DataTable();foreach(var column in new[]{"archivo_id","raiz_id","cliente_archivo_id","origen_doc","datos_raiz","ruta_hash","bytes_previstos","estado_recepcion","estado_efecto","revision","bytes_recibidos","contenido_hash","staging_ref","intento_activo_id","plan_valores_doc","valores_efectivos_doc","resultado_doc","cliente_raiz"})fileRows.Columns.Add(column,typeof(object));
        byte[] pathHash;using(var sha=System.Security.Cryptography.SHA256.Create())pathHash=sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(source.RutaRelativa));
        fileRows.Rows.Add(30u,40u,"f1",PerfilImportacionJson.Escribir(source),rootRows.Rows[0]["datos_doc"],pathHash,7UL,"INCIERTO","INCIERTO",1u,DBNull.Value,DBNull.Value,DBNull.Value,DBNull.Value,"[]",DBNull.Value,DBNull.Value,"r1");
        int calls=0;string kind="RAICES";var filter=new FiltroConsultaImportacion{Pagina=2,TamanoPagina=1,RaizId="40",Estado="INCIERTO"};
        Func<string,object[],DataTable> query=(sql,args)=>{
            calls++;Check(sql.StartsWith("SELECT ")&&!sql.Contains("FOR UPDATE")&&sql.Contains("l.ambito_ref=@ambito")&&sql.Contains("l.solicitante_ref=@usuario")&&sql.Contains("l.nivel_ref=@nivel")&&sql.Contains("l.lote_id=@lote"),"DOC111 page query lost scope or writes");
            Check(sql.Contains("r.raiz_id=@raiz")&&!sql.Contains("INCIERTO")&&!sql.Contains("=40"),"DOC111 filter interpolated into SQL");
            Check(Convert.ToString(args[Array.IndexOf(args,"@estado")+1])=="INCIERTO","DOC111 state filter lost parameter");
            if(sql.StartsWith("SELECT COUNT")){var count=new DataTable();count.Columns.Add("total",typeof(uint));count.Rows.Add(2u);return count;}
            Check(sql.EndsWith("LIMIT @tamano OFFSET @inicio")&&sql.Contains("ORDER BY "+(kind=="RAICES"?"r.raiz_id":"f.archivo_id"))&&(long)args[Array.IndexOf(args,"@inicio")+1]==1L,"DOC111 page order/offset is not stable and bounded");
            return kind=="RAICES"?rootRows:fileRows;
        };
        var roots=(PaginaRecursosImportacion<RaizImportacionCarpetas>)RepositoryHelper("LeerRaicesConsulta",lot,filter,query);Check(roots.Total==2&&roots.Elementos.Single().Estado=="INCIERTO"&&roots.RevisionLectura==7&&calls==2,"DOC111 root SQL page reader lost revision/count");
        kind="ARCHIVOS";filter.EstadoRecepcion="INCIERTO";var files=(PaginaRecursosImportacion<ArchivoImportacionCarpetas>)RepositoryHelper("LeerArchivosConsulta",lot,filter,query);Check(files.Total==2&&files.Elementos.Single().EstadoRecepcion=="INCIERTO","DOC111 file SQL projection loses uncertain receipt");
        filter.Estado="DROP TABLE";var before=calls;Rejected("SOLICITUD_INVALIDA",()=>RepositoryHelper("LeerArchivosConsulta",lot,filter,query));Check(calls==before,"DOC111 invalid filter reaches SQL");filter.Estado=null;
        filter.Pagina=int.MaxValue;filter.TamanoPagina=200;Rejected("SOLICITUD_INVALIDA",()=>RepositoryHelper("LeerArchivosConsulta",lot,filter,query));
        Console.WriteLine("DOC-111 scoped SELECT page queries and closed filters (synthetic records): passed");
    }

    static void QueryDispatcherCases() {
        var fixture=new ResourceFixture();var repository=new QueryRepository();repository.Lot.Contexto=fixture.Adapter.ResolverDestino(fixture.Http,"7");repository.Lot.Snapshot.Configuracion=fixture.Configuration();int factories=0;
        Func<HttpContext,ImportacionCarpetasContextAdapter,ServicioConsultaImportacion> factory=(http,authority)=>{factories++;return new ServicioConsultaImportacion(()=>authority.Resolver(http),repository,id=>authority.ResolverDestino(http,id,"CONSULTAR"),(c,config)=>authority.AutorizarRecursos(http,c,config,"CONSULTAR"),50);};
        var constructor=typeof(WebServiceImportacionCarpetas).GetConstructors(BindingFlags.NonPublic|BindingFlags.Instance).Single();var endpoint=(WebServiceImportacionCarpetas)constructor.Invoke(new object[]{fixture.Adapter,(Func<LimitesImportacionCarpetas>)(()=>RequestLimits(16384)),null,null,factory,null});var previous=HttpContext.Current;
        Func<string,string,bool,RespuestaPerfilImportacion> send=(operation,body,token)=>{
            var worker=new ImportRequest{Body=System.Text.Encoding.UTF8.GetBytes(body)};var http=SessionFixture(worker:worker);worker.Token=ImportacionCarpetasRequestGuard.Emitir(http,RequestContext());if(!token)worker.Token=null;HttpContext.Current=http;
            return operation=="CONFIRMACION"?endpoint.ConsultarConfirmacionLote(new ConsultarConfirmacionImportacionSolicitud{planId="999"}):operation=="ESTADO"?endpoint.ConsultarLote(new ConsultarLoteImportacionSolicitud{loteId="999"}):operation=="PAGINA"?endpoint.ConsultarRecursosLote(new ConsultarRecursosImportacionSolicitud{loteId="999"}):endpoint.ExportarPaginaLote(new ConsultarRecursosImportacionSolicitud{loteId="999"});
        };
        try{
            const string state="{\"solicitud\":{\"loteId\":\"20\"}}";const string page="{\"solicitud\":{\"loteId\":\"20\",\"tipo\":\"ARCHIVOS\",\"pagina\":1,\"tamanoPagina\":2}}";
            Check(send("ESTADO",state,true).ok&&send("PAGINA",page,true).ok&&send("CSV",page,true).ok,"DOC111 operational query facade failed");
            var confirmation=send("CONFIRMACION","{\"solicitud\":{\"planId\":\"17\"}}",true);Check(confirmation.ok&&((ConsultaConfirmacionImportacionResultado)confirmation.datos).loteId=="20","DOC111 ASMX confirmation recovery does not use guarded body");
            Check(((EstadoLoteImportacionResultado)send("ESTADO",state,true).datos).loteId=="20","DOC111 query facade trusts coerced request");
            int before=factories;Check(send("ESTADO",state,false).codigo=="SOLICITUD_NO_VERIFICADA"&&factories==before,"DOC111 query token missing reaches service");
            Check(!send("PAGINA","{\"solicitud\":{\"loteId\":\"20\",\"tipo\":\"ARCHIVOS\",\"usuarioRef\":\"99\"}}",true).ok&&factories==before,"DOC111 query accepts authority injection");
            fixture.ImportAllowed=false;Check(send("ESTADO",state,true).codigo=="ACCESO_DENEGADO"&&factories==before,"DOC111 query facade caches authority");fixture.ImportAllowed=true;
            Check(send("PAGINA","{\"solicitud\":{\"loteId\":\"20\",\"tipo\":\"ARCHIVOS\",\"pagina\":0}}",true).codigo=="SOLICITUD_INVALIDA","DOC111 facade accepts invalid page");
            Check(!endpoint.ListarPerfiles(new ListarPerfilesSolicitud()).ok,"DOC111 query capability elevates administration");
        }finally{HttpContext.Current=previous;}
        Console.WriteLine("DOC-111 ASMX read/export guard and typed responses (synthetic HTTP): passed");
    }

    static WebServiceImportacionCarpetas OperationsEndpoint(ResourceFixture authority,Func<string,CoordinadorImportacionCarpetas> factory){
        var constructor=typeof(WebServiceImportacionCarpetas).GetConstructors(BindingFlags.NonPublic|BindingFlags.Instance).Single();
        return (WebServiceImportacionCarpetas)constructor.Invoke(new object[]{authority.Adapter,(Func<LimitesImportacionCarpetas>)(()=>RequestLimits(16384)),null,null,null,new Func<HttpContext,ImportacionCarpetasContextAdapter,string,CoordinadorImportacionCarpetas>((http,a,action)=>factory(action))});
    }
    static RespuestaPerfilImportacion SendOperation(WebServiceImportacionCarpetas endpoint,string operation,object request,Action<ImportRequest,HttpContext> change=null,string raw=null){
        var previous=HttpContext.Current;
        try{
            var worker=new ImportRequest{Body=System.Text.Encoding.UTF8.GetBytes(raw??PerfilImportacionJson.Escribir(new{solicitud=request}))};var http=SessionFixture(worker:worker);
            worker.Token=ImportacionCarpetasRequestGuard.Emitir(http,RequestContext());if(change!=null)change(worker,http);HttpContext.Current=http;
            // El objeto que entrega ASMX no es autoridad: se usa el cuerpo cerrado.
            var result=operation=="CERRAR"?endpoint.CerrarReservaSinInicio(new ReconciliarRecursoImportacionSolicitud{loteId="999"}):operation=="DETENER"?endpoint.SolicitarDetencion(new DetenerLoteImportacionSolicitud{loteId="999"}):operation=="RECONCILIAR"?endpoint.ReconciliarRecurso(new ReconciliarRecursoImportacionSolicitud{loteId="999"}):endpoint.AvanzarLote(new AvanzarLoteImportacionSolicitud{loteId="999"});
            int status=result.ok?200:result.codigo=="RESULTADO_INCIERTO"||result.codigo=="CONFLICTO_REVISION"||result.codigo=="OPERACION_EN_CURSO"?409:result.codigo=="ACCESO_DENEGADO"||result.codigo=="SOLICITUD_NO_VERIFICADA"?403:result.codigo=="ARCHIVO_RECHAZADO"?422:result.codigo=="ERROR_INTERNO"?500:400;
            Check(http.Response.StatusCode==status&&http.Response.Cache.GetCacheability()==HttpCacheability.NoCache,"DOC111 phase facade wrong HTTP/cache policy");
            return result;
        }finally{HttpContext.Current=previous;}
    }
    static AvanzarLoteImportacionSolicitud AdvanceRequest(string phase,string resource,uint revision,string key){return new AvanzarLoteImportacionSolicitud{loteId="20",recursoId=resource,fase=phase,revisionEsperada=revision,claveOperacion=key};}
    static void OperationHttpCases(){
        var authority=new ResourceFixture();int factories=0;
        var guarded=OperationsEndpoint(authority,purpose=>{factories++;throw new Exception("PRIVATE_UNREACHABLE_FACTORY");});
        foreach(var operation in new[]{"AVANZAR","DETENER","RECONCILIAR","CERRAR"}){
            object request=operation=="DETENER"?(object)new DetenerLoteImportacionSolicitud{loteId="20",revisionEsperada=1}:operation=="RECONCILIAR"?(object)new ReconciliarRecursoImportacionSolicitud{loteId="20",intentoId="50",revisionEsperada=1}:AdvanceRequest("ALMACENAR","30",1,"http-guard");
            Check(SendOperation(guarded,operation,request,(w,h)=>w.Token=null).codigo=="SOLICITUD_NO_VERIFICADA","Mutation endpoint skips token guard");
            Check(SendOperation(guarded,operation,request,(w,h)=>w.Verb="GET").codigo=="SOLICITUD_NO_VERIFICADA","Mutation endpoint accepts GET");
            Check(SendOperation(guarded,operation,request,(w,h)=>w.Origin="http://other.invalid").codigo=="SOLICITUD_NO_VERIFICADA","Mutation endpoint accepts foreign origin");
        }
        Check(factories==0,"HTTP guard failure reaches phase composition");
        using(var f=new CreationPhaseFixture()){
            var endpoint=OperationsEndpoint(authority,phase=>{factories++;Check(phase=="CREAR_EXPEDIENTE","Creation request composed another phase");return f.Coordinator;});
            var request=AdvanceRequest("CREAR_EXPEDIENTE","40",1,"http-creation");var r=SendOperation(endpoint,"AVANZAR",request);var data=(AvanceLoteImportacionResultado)r.datos;
            Check(r.ok&&data.fase==request.fase&&data.raizId=="40"&&data.expedienteId=="60"&&data.relacionNivelId=="70"&&f.Writers==1&&data.documentoId==null,"DOC111 HTTP creation lost IDs or chained document effect");
            var output=PerfilImportacionJson.Escribir(data);Check(!output.Contains("Token")&&!output.Contains("Staging")&&!output.Contains("Valores")&&!output.Contains("Ciclo")&&!output.Contains("SENTINEL"),"DOC111 phase HTTP exposes internal proof");
            data=(AvanceLoteImportacionResultado)SendOperation(endpoint,"AVANZAR",request).datos;Check(data.repeticion==true&&f.Writers==1&&f.Reserves==1,"DOC111 HTTP repeats creation on exact replay");
            int before=factories;
            Check(SendOperation(endpoint,"AVANZAR",request,(w,h)=>w.Token=null).codigo=="SOLICITUD_NO_VERIFICADA"&&factories==before,"Missing phase token reaches factory");
            Check(!SendOperation(endpoint,"AVANZAR",request,null,"{\"solicitud\":{\"loteId\":\"20\",\"usuarioRef\":\"1\"}}").ok&&factories==before,"Phase authority injection reaches factory");
            Check(!SendOperation(endpoint,"AVANZAR",request,null,"{\"solicitud\":{\"loteId\":\"20\",\"loteId\":\"999\"}}").ok&&factories==before,"Duplicate phase field reaches factory");
            request.fase="crear_expediente";Check(SendOperation(endpoint,"AVANZAR",request).codigo=="SOLICITUD_INVALIDA"&&factories==before,"Case insensitive phase selects execution under Option Compare Text");
            request.fase="INCORPORAR";Check(SendOperation(endpoint,"AVANZAR",request).codigo=="SOLICITUD_INVALIDA"&&factories==before,"HTTP exposes independent incorporation adapter");request.fase="CREAR_EXPEDIENTE";
            authority.ImportAllowed=false;Check(SendOperation(endpoint,"AVANZAR",request).codigo=="ACCESO_DENEGADO"&&factories==before,"Phase facade cached import authority");authority.ImportAllowed=true;
        }
        using(var f=new DocumentPhaseFixture()){
            var endpoint=OperationsEndpoint(authority,phase=>{Check(phase=="ALMACENAR","Storage request composed another phase");return f.Coordinator;});var request=AdvanceRequest("ALMACENAR","30",f.OriginalRevision,"http-document");
            var result=SendOperation(endpoint,"AVANZAR",request);var data=(AvanceLoteImportacionResultado)result.datos;
            Check(result.ok&&data.documentoId=="90"&&data.inventarioId=="100"&&data.archivoId=="30"&&data.gabineteId=="7"&&f.Source.Writers==1,"DOC111 HTTP document phase loses known IDs");
            Check(((AvanceLoteImportacionResultado)SendOperation(endpoint,"AVANZAR",request).datos).repeticion==true&&f.Source.Writers==1,"DOC111 HTTP document replay repeats writer");
        }
        using(var f=new DocumentPhaseFixture()){
            var endpoint=OperationsEndpoint(authority,phase=>f.Coordinator);f.Source.VerifyResult=false;
            var r=SendOperation(endpoint,"AVANZAR",AdvanceRequest("ALMACENAR","30",f.OriginalRevision,"http-uncertain"));Check(!r.ok&&r.codigo=="RESULTADO_INCIERTO"&&((AvanceLoteImportacionResultado)r.datos).documentoId=="90"&&f.Source.Writers==1,"DOC111 HTTP reports uncertain storage as success or loses IDs");
        }
        var values=new ValuesFixture();var valuesEndpoint=OperationsEndpoint(authority,phase=>{Check(phase=="MATERIALIZAR_VALORES","Value request selected another phase");return values.Coordinator;});
        var valueRequest=AdvanceRequest("MATERIALIZAR_VALORES","30",3,"http-values");var fixedValues=SendOperation(valuesEndpoint,"AVANZAR",valueRequest);
        Check(fixedValues.ok&&((AvanceLoteImportacionResultado)fixedValues.datos).codigo=="VALORES_FIJADOS"&&values.Engine.Calls==1&&values.View.Archivo.EstadoEfecto=="NO_INICIADO","DOC111 HTTP materialization claims documentary completion");
        Check(SendOperation(valuesEndpoint,"AVANZAR",valueRequest).ok&&values.Engine.Calls==1,"DOC111 HTTP value replay reconsults provider");
        values=new ValuesFixture();values.Engine.Code="SIN_COINCIDENCIA";var rejected=SendOperation(valuesEndpoint,"AVANZAR",valueRequest);Check(!rejected.ok&&rejected.codigo=="ARCHIVO_RECHAZADO","DOC111 HTTP incompatible values reported as success");
        using(var f=new DocumentPhaseFixture()){
            var endpoint=OperationsEndpoint(authority,phase=>{Check(phase=="DETENER_LOTE"||phase=="ALMACENAR","Stop request composed wrong purpose");return f.Coordinator;});
            var request=new DetenerLoteImportacionSolicitud{loteId="20",revisionEsperada=f.Source.Lot.Revision};var result=SendOperation(endpoint,"DETENER",request);
            Check(result.ok&&((DetencionLoteImportacionResultado)result.datos).detencionSolicitada&&f.Stops==1&&f.Source.Writers==0,"DOC111 HTTP stop executes effect or loses signal");
            Check(SendOperation(endpoint,"DETENER",request).ok&&f.Stops==1,"DOC111 HTTP stop replay emits another signal");
            Check(!SendOperation(endpoint,"AVANZAR",AdvanceRequest("ALMACENAR","30",f.OriginalRevision,"http-stopped")).ok&&f.Source.Writers==0,"DOC111 HTTP starts effect after stop");
        }
        using(var f=new ReconciliationFixture()){
            var endpoint=OperationsEndpoint(authority,phase=>{Check(phase=="RECONCILIAR","Reconciliation selected another purpose");return f.Coordinator;});
            var request=new ReconciliarRecursoImportacionSolicitud{loteId="20",intentoId="50",revisionEsperada=f.Source.Phase.Source.Lot.Revision};var r=SendOperation(endpoint,"RECONCILIAR",request);var data=(ReconciliacionRecursoImportacionResultado)r.datos;
            Check(r.ok&&data.resuelta&&data.documentoId=="90"&&f.Saves==1&&f.Source.Phase.Source.Writers==1,"DOC111 HTTP reconciliation repeats effect or misses durable proof");
            Check(((ReconciliacionRecursoImportacionResultado)SendOperation(endpoint,"RECONCILIAR",request).datos).repeticion==true&&f.Saves==1,"DOC111 HTTP reconciliation replay rewrites checkpoint");
        }
        using(var f=new ReconciliationFixture()){
            f.Verify=false;var endpoint=OperationsEndpoint(authority,phase=>f.Coordinator);var r=SendOperation(endpoint,"RECONCILIAR",new ReconciliarRecursoImportacionSolicitud{loteId="20",intentoId="50",revisionEsperada=f.Source.Phase.Source.Lot.Revision});
            Check(!r.ok&&r.codigo=="RESULTADO_INCIERTO"&&!((ReconciliacionRecursoImportacionResultado)r.datos).resuelta&&f.Saves==0,"DOC111 HTTP reconciliation without proof declares success");
        }
        foreach(var name in new[]{"AvanzarLote","SolicitarDetencion","ReconciliarRecurso","CerrarReservaSinInicio"}){
            var method=typeof(WebServiceImportacionCarpetas).GetMethod(name);var web=(System.Web.Services.WebMethodAttribute)method.GetCustomAttributes(typeof(System.Web.Services.WebMethodAttribute),false).Single();Check(web.EnableSession,"DOC111 phase endpoint lacks required session");
        }
        Console.WriteLine("DOC-111 ASMX bounded phases stop and reconciliation reuse guard and safe projection (synthetic HTTP and effect doubles): passed");
    }
    static void TransitionCases() {
        var states=new Dictionary<string,string[]> {
            {"PLAN",new[]{"REVISABLE","BLOQUEADO","CONFIRMADO","EXPIRADO"}},
            {"RECEPCION",new[]{"PENDIENTE","RECIBIENDO","RECIBIDO","RECHAZADO","INCIERTO","CANCELADO"}},
            {"EFECTO",new[]{"NO_INICIADO","EN_CURSO","ALMACENADO","COMPLETADO","RECHAZADO","INCIERTO","CANCELADO"}},
            {"RAIZ",new[]{"PENDIENTE","CREANDO","EXPEDIENTE_CONFIRMADO","INCIERTO","BLOQUEADO","COMPLETADA","PARCIAL","CANCELADA"}},
            {"INTENTO",new[]{"RESERVADO","EN_CURSO","CONFIRMADO","RECHAZADO","INCIERTO"}},
            {"LOTE",new[]{"RECIBIENDO","LISTO","EN_CURSO","PENDIENTE_VERIFICACION","DETENIDO","COMPLETADO","PARCIAL","FALLIDO"}}
        };
        var terminals=new Dictionary<string,string[]> {
            {"PLAN",new[]{"CONFIRMADO","EXPIRADO"}},{"RECEPCION",new[]{"CANCELADO"}},{"EFECTO",new[]{"COMPLETADO","CANCELADO"}},
            {"RAIZ",new[]{"COMPLETADA","CANCELADA"}},{"INTENTO",new[]{"CONFIRMADO","RECHAZADO"}},{"LOTE",new[]{"COMPLETADO","FALLIDO"}}
        };
        foreach(var dimension in states) {
            var actual=CoordinadorImportacionCarpetas.EstadosDimension(dimension.Key);
            Check(new HashSet<string>(actual,StringComparer.Ordinal).SetEquals(dimension.Value),"DOC111 transition table omits schema state");
            foreach(var from in dimension.Value) {
                var next=CoordinadorImportacionCarpetas.SiguientesPermitidos(dimension.Key,from);
                Check(next.All(n=>dimension.Value.Contains(n)) && !next.Contains(from),"DOC111 transition points to foreign or identical state");
                if(terminals[dimension.Key].Contains(from))Check(next.Count==0,"DOC111 terminal result can be replayed or erased");
                foreach(var to in dimension.Value) {
                    if(CoordinadorImportacionCarpetas.EsTransicionPermitida(dimension.Key,from,to))CoordinadorImportacionCarpetas.ValidarTransicion(dimension.Key,from,to);
                    else Rejected("CONFLICTO_REVISION",()=>CoordinadorImportacionCarpetas.ValidarTransicion(dimension.Key,from,to));
                }
                Check(!CoordinadorImportacionCarpetas.EsTransicionPermitida(dimension.Key,from,"UNKNOWN") && !CoordinadorImportacionCarpetas.EsTransicionPermitida(dimension.Key,from,null),"DOC111 unknown destination transition accepted");
            }
            var before=CoordinadorImportacionCarpetas.EstadosDimension(dimension.Key);actual.Clear();Check(CoordinadorImportacionCarpetas.EstadosDimension(dimension.Key).SequenceEqual(before),"DOC111 caller mutates private transition schema");
        }
        foreach(var transition in new[]{new[]{"PLAN","BLOQUEADO","REVISABLE"},new[]{"RECEPCION","RECIBIDO","COMPLETADO"},new[]{"RECEPCION","INCIERTO","RECIBIENDO"},new[]{"EFECTO","INCIERTO","EN_CURSO"},new[]{"EFECTO","ALMACENADO","NO_INICIADO"},new[]{"RAIZ","INCIERTO","CREANDO"},new[]{"RAIZ","EXPEDIENTE_CONFIRMADO","PENDIENTE"},new[]{"INTENTO","INCIERTO","EN_CURSO"},new[]{"INTENTO","RECHAZADO","RESERVADO"},new[]{"LOTE","PENDIENTE_VERIFICACION","EN_CURSO"},new[]{"LOTE","PARCIAL","FALLIDO"}})
            Rejected("CONFLICTO_REVISION",()=>CoordinadorImportacionCarpetas.ValidarTransicion(transition[0],transition[1],transition[2]));
        CoordinadorImportacionCarpetas.ValidarTransicion("RECEPCION","INCIERTO","RECIBIDO");CoordinadorImportacionCarpetas.ValidarTransicion("RAIZ","INCIERTO","EXPEDIENTE_CONFIRMADO");
        Check(!CoordinadorImportacionCarpetas.EsTransicionPermitida("recepcion","PENDIENTE","RECIBIENDO") && !CoordinadorImportacionCarpetas.EsTransicionPermitida("RECEPCION","pendiente","RECIBIENDO"),"DOC111 project Option Compare Text broadens state protocol");
        Rejected("CONFLICTO_REVISION",()=>CoordinadorImportacionCarpetas.EstadosDimension("UNKNOWN"));Rejected("CONFLICTO_REVISION",()=>CoordinadorImportacionCarpetas.SiguientesPermitidos("RAIZ",null));
        var exposed=CoordinadorImportacionCarpetas.SiguientesPermitidos("INTENTO","INCIERTO");exposed.Add("EN_CURSO");Check(!CoordinadorImportacionCarpetas.EsTransicionPermitida("INTENTO","INCIERTO","EN_CURSO"),"DOC111 client can add a replay transition");
        Console.WriteLine("DOC-111 exhaustive state dimensions and forbidden replay: passed");
    }

    static void StagingCases() {
        var root=Path.Combine(Path.GetTempPath(),"doc111-staging-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(root);
        try {
            var context=new ResourceFixture().Adapter.ResolverDestino(SessionFixture(),"7");
            var pdf=StagingPdf();var limits=new LimitesImportacionCarpetas(2,4,(ulong)pdf.Length*4,(ulong)pdf.Length*100,16384,TimeSpan.FromMinutes(1),TimeSpan.FromHours(1));
            var lot=new LoteImportacionCarpetas{LoteId="20",Contexto=context,Estado="RECIBIENDO",Snapshot=new SnapshotPerfilImportacionOperativa{Configuracion=new ConfiguracionPerfilImportacion{Extensiones=new List<string>{"pdf"}}}};
            var file=new ArchivoImportacionCarpetas{ArchivoId="30",RaizId="40",NombreOriginal="Órdenes de compra.pdf",BytesPrevistos=pdf.Length.ToString(),EstadoRecepcion="PENDIENTE"};
            var attempt=new IntentoImportacionCarpetas{IntentoId="50",LoteId="20",RaizId="40",ArchivoId="30",Fase="RECEPCION",Estado="EN_CURSO",TokenReclamacion=Guid.NewGuid().ToString("N")};
            Func<AlmacenStagingImportacion> create=()=>new AlmacenStagingImportacion(root,limits,lot,file,10,200);
            var store=create();var denied=PerfilImportacionJson.Leer<ContextoImportacionCarpetas>(PerfilImportacionJson.Escribir(context));denied.UsuarioRef="99";
            using(var input=new StagingStream(pdf)){Rejected("ACCESO_DENEGADO",()=>store.Recibir(denied,attempt,input,file.BytesPrevistos));Check(input.BytesRead==0 && !Directory.Exists(Path.Combine(root,"App_Data")),"DOC111 unauthorized stream touches private storage");}
            attempt.ArchivoId="31";using(var input=new StagingStream(pdf)){Rejected("SOLICITUD_INVALIDA",()=>store.Recibir(context,attempt,input,file.BytesPrevistos));Check(input.BytesRead==0,"DOC111 foreign file read before rejection");}attempt.ArchivoId="30";
            var limitedLot=PerfilImportacionJson.Leer<LoteImportacionCarpetas>(PerfilImportacionJson.Escribir(lot));limitedLot.Snapshot.Configuracion.MaximoBytesArchivo=pdf.Length-1;
            using(var input=new StagingStream(pdf)){Rejected("LIMITE_EXCEDIDO",()=>new AlmacenStagingImportacion(root,limits,limitedLot,file).Recibir(context,attempt,input,file.BytesPrevistos));Check(input.BytesRead==0&&!Directory.Exists(Path.Combine(root,"App_Data")),"DOC111 native composition bypasses published file size limit");}
            RecepcionVerificadaImportacion received;
            using(var input=new StagingStream(pdf)){received=store.Recibir(context,attempt,input,file.BytesPrevistos);Check(input.BytesRead==pdf.Length && input.MaxRequested<=4096,"DOC111 streaming reads whole file or requires seek");}
            string expected;using(var sha=System.Security.Cryptography.SHA256.Create())expected=Hex(sha.ComputeHash(pdf));
            Check(received.BytesRecibidos==pdf.Length.ToString() && received.ContenidoHash==expected && received.StagingRef.Length==32 && received.StagingRef!=attempt.TokenReclamacion,"DOC111 staging does not report actual content evidence");
            var complete=Directory.GetFiles(Path.Combine(root,"App_Data"),"*.bin",SearchOption.AllDirectories).Single();Check(Path.GetFileName(complete)==received.StagingRef+".bin" && !complete.Contains(file.NombreOriginal),"DOC111 original filename determines retained storage path");
            Check(store.Verificar(context,attempt).ContenidoHash==expected,"DOC111 complete part cannot be reconciled before row confirmation");
            using(var input=new StagingStream(pdf)){Rejected("OPERACION_EN_CURSO",()=>store.Recibir(context,attempt,input,file.BytesPrevistos));Check(input.BytesRead==0,"DOC111 blindly replays complete reception");}
            var storedFile=PerfilImportacionJson.Leer<ArchivoImportacionCarpetas>(PerfilImportacionJson.Escribir(file));storedFile.EstadoRecepcion="RECIBIDO";storedFile.StagingRef=received.StagingRef;storedFile.ContenidoHash=expected;storedFile.BytesRecibidos=received.BytesRecibidos;
            var retained=new AlmacenStagingImportacion(root,limits,lot,storedFile,10,200);var work=retained.CrearCopiaTrabajo(context,received.StagingRef,file.NombreOriginal);
            Check(new AlmacenStagingImportacion(root,limits,lot,storedFile).Verificar(context,attempt).ContenidoHash==expected,"DOC111 retained PDF requires new appSettings");
            Check(Path.GetFileName(work)==file.NombreOriginal && File.ReadAllBytes(work).SequenceEqual(pdf) && File.Exists(complete) && work!=complete,"DOC111 work copy renames document or consumes retained staging");
            File.Delete(work);Check(retained.Verificar(context,attempt).ContenidoHash==expected && File.Exists(complete),"DOC111 legacy copy consumption destroys retained content");
            File.Move(complete,complete+".missing");Rejected("RESULTADO_INCIERTO",()=>retained.Verificar(context,attempt));File.Move(complete+".missing",complete);
            var tampered=(byte[])pdf.Clone();tampered[0]=0;File.WriteAllBytes(complete,tampered);Rejected("RESULTADO_INCIERTO",()=>retained.Verificar(context,attempt));File.WriteAllBytes(complete,pdf);
            using(var held=new FileStream(Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(complete)),"recepcion.lock"),FileMode.Open,FileAccess.ReadWrite,FileShare.None)){Rejected("OPERACION_EN_CURSO",()=>retained.Verificar(context,attempt));}
            Rejected("SOLICITUD_INVALIDA",()=>retained.CrearCopiaTrabajo(context,received.StagingRef,"../OrdenCompra.pdf"));
            storedFile.ContenidoHash=new string('0',64);var changedRow=new AlmacenStagingImportacion(root,limits,lot,storedFile,10,200);Rejected("RESULTADO_INCIERTO",()=>changedRow.Verificar(context,attempt));
            var nextId=60;
            Action<byte[],string,string,int,int,int> reject=(body,name,code,pages,objects,expectedSize)=>{
                var broken=PerfilImportacionJson.Leer<ArchivoImportacionCarpetas>(PerfilImportacionJson.Escribir(file));broken.ArchivoId=(nextId++).ToString();broken.NombreOriginal=name;
                broken.BytesPrevistos=expectedSize.ToString();
                // El tamano esperado procede del manifiesto, no del stream.
                var current=new IntentoImportacionCarpetas{IntentoId=(nextId++).ToString(),LoteId="20",RaizId="40",ArchivoId=broken.ArchivoId,Fase="RECEPCION",Estado="EN_CURSO",TokenReclamacion=Guid.NewGuid().ToString("N")};
                var receiver=new AlmacenStagingImportacion(root,limits,lot,broken,pages,objects);
                using(var input=new StagingStream(body)){Rejected(code,()=>receiver.Recibir(context,current,input,broken.BytesPrevistos));Check(input.BytesRead<=expectedSize+1,"DOC111 rejected stream read beyond detection byte");}
                Rejected("RESULTADO_INCIERTO",()=>receiver.Verificar(context,current));
                Check(Directory.GetFiles(Path.Combine(root,"App_Data"),"*.bin",SearchOption.AllDirectories).Length==1,"DOC111 invalid content promoted to complete staging");
            };
            reject(pdf.Take(pdf.Length-1).ToArray(),"truncated.pdf","ARCHIVO_RECHAZADO",10,200,pdf.Length);
            reject(pdf.Concat(new byte[4096]).ToArray(),"oversized.pdf","LIMITE_EXCEDIDO",10,200,pdf.Length);
            reject(new byte[pdf.Length],"fake.pdf","ARCHIVO_RECHAZADO",10,200,pdf.Length);
            var fabricated=new byte[pdf.Length];System.Text.Encoding.ASCII.GetBytes("%PDF-1.7\n").CopyTo(fabricated,0);System.Text.Encoding.ASCII.GetBytes("%%EOF").CopyTo(fabricated,fabricated.Length-5);
            reject(fabricated,"fake-envelope.pdf","ARCHIVO_RECHAZADO",10,200,fabricated.Length);
            // Validacion previa al disco: no se puede verificar contenido para
            // nombres/formatos que ni siquiera admite el contrato del archivo.
            foreach(var name in new[]{"CON.pdf","COM¹.pdf","file.pdf.","file.pdf ","file.pdf:stream","../file.pdf"}){var invalidFile=PerfilImportacionJson.Leer<ArchivoImportacionCarpetas>(PerfilImportacionJson.Escribir(file));invalidFile.NombreOriginal=name;using(var input=new StagingStream(pdf))Rejected("SOLICITUD_INVALIDA",()=>new AlmacenStagingImportacion(root,limits,lot,invalidFile,10,200).Recibir(context,attempt,input,invalidFile.BytesPrevistos));}
            lot.Snapshot.Configuracion.Extensiones.Clear();foreach(var name in new[]{"file.tif","file.tiff"}){var unsupported=PerfilImportacionJson.Leer<ArchivoImportacionCarpetas>(PerfilImportacionJson.Escribir(file));unsupported.NombreOriginal=name;using(var input=new StagingStream(pdf)){Rejected("FORMATO_NO_SOPORTADO",()=>new AlmacenStagingImportacion(root,limits,lot,unsupported,10,200).Recibir(context,attempt,input,unsupported.BytesPrevistos));Check(input.BytesRead==0,"DOC111 unsupported format read before rejection");}}lot.Snapshot.Configuracion.Extensiones.Add("pdf");
            reject(pdf,"limits.pdf","ARCHIVO_RECHAZADO",10,1,pdf.Length);
            var twoPages=StagingPdf(2);reject(twoPages,"pages.pdf","ARCHIVO_RECHAZADO",1,200,twoPages.Length);
            var encrypted=StagingPdf(1,true);reject(encrypted,"protected.pdf","ARCHIVO_RECHAZADO",10,200,encrypted.Length);
            reject(encrypted,"protected-native.pdf","ARCHIVO_RECHAZADO",Int32.MaxValue,Int32.MaxValue,encrypted.Length);
            var interrupted=PerfilImportacionJson.Leer<ArchivoImportacionCarpetas>(PerfilImportacionJson.Escribir(file));interrupted.ArchivoId="92";
            var interruptedAttempt=new IntentoImportacionCarpetas{IntentoId="93",LoteId="20",RaizId="40",ArchivoId="92",Fase="RECEPCION",Estado="EN_CURSO",TokenReclamacion=Guid.NewGuid().ToString("N")};
            var interruptedStore=new AlmacenStagingImportacion(root,limits,lot,interrupted,10,200);
            using(var input=new StagingStream(pdf){Fail=true}) {
                try{interruptedStore.Recibir(context,interruptedAttempt,input,interrupted.BytesPrevistos);throw new Exception("DOC111 read fault accepted");}
                catch(PerfilImportacionException ex){Check(ex.Codigo=="CAPACIDAD_NO_DISPONIBLE" && !ex.Message.Contains("SENTINEL"),"DOC111 private IO details leaked");}
            }
            Rejected("RESULTADO_INCIERTO",()=>interruptedStore.Verificar(context,interruptedAttempt));
            using(var input=new StagingStream(pdf)){Rejected("OPERACION_EN_CURSO",()=>interruptedStore.Recibir(context,interruptedAttempt,input,interrupted.BytesPrevistos));Check(input.BytesRead==0,"DOC111 interrupted part silently overwritten");}
            // Los restos .part tambien consumen cuota. No se borran para volver
            // a habilitar una carga ni se confunden con una recepcion completa.
            var quota=new LimitesImportacionCarpetas(2,4,(ulong)pdf.Length,(ulong)pdf.Length,16384,TimeSpan.FromMinutes(1),TimeSpan.FromHours(1));
            var second=PerfilImportacionJson.Leer<ArchivoImportacionCarpetas>(PerfilImportacionJson.Escribir(file));second.ArchivoId="90";
            attempt.ArchivoId="90";attempt.IntentoId="91";attempt.TokenReclamacion=Guid.NewGuid().ToString("N");
            using(var input=new StagingStream(pdf)){Rejected("LIMITE_EXCEDIDO",()=>new AlmacenStagingImportacion(root,quota,lot,second,10,200).Recibir(context,attempt,input,second.BytesPrevistos));Check(input.BytesRead==0,"DOC111 retained quota checked after reading input");}
            Unavailable(()=>new AlmacenStagingImportacion(root,limits,lot,file,0,200));
        } finally {
            var resolved=Path.GetFullPath(root);Check(Path.GetDirectoryName(resolved)==Path.GetFullPath(Path.GetTempPath()).TrimEnd(Path.DirectorySeparatorChar) && Path.GetFileName(resolved).StartsWith("doc111-staging-",StringComparison.Ordinal),"DOC111 temporary cleanup escaped its test root");
            Directory.Delete(resolved,true);
        }
        Console.WriteLine("DOC-111 private streaming staging and retained copy: passed");
    }
    static void InvalidJson<T>(string json) {
        try { PerfilImportacionJson.Leer<T>(json); }
        catch(PerfilImportacionException e) { Check(e.Codigo=="CONFIGURACION_INVALIDA","DOC111 unexpected JSON rejection");return; }
        throw new Exception("DOC111 untrusted request accepted");
    }
    static void Unavailable(Action operation) {
        try { operation(); }
        catch(PerfilImportacionException e) { Check(e.Codigo=="CAPACIDAD_NO_DISPONIBLE","DOC111 unexpected policy rejection");return; }
        throw new Exception("DOC111 unconfigured capability accepted");
    }

    static void ValidateSchema(DataSet schema) {
        try {
            typeof(MySqlImportacionCarpetasRepository).GetMethod("ValidarMetadatos",BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,new object[]{schema});
        } catch(TargetInvocationException e) {
            if(e.InnerException is PerfilImportacionException)throw e.InnerException;
            throw;
        }
    }
    static DataRow SchemaRow(DataSet schema,string table,string name,string value) {
        return schema.Tables[table].Rows.Cast<DataRow>().First(r=>Convert.ToString(r[name])==value);
    }
    static void SchemaGuards() {
        // Metadatos capturados mediante SELECT en MySQL 5.1.37 tras aplicar el SQL.
        // Los defectos se introducen solo en copias locales; no se altera la BD.
        var schema=JsonConvert.DeserializeObject<DataSet>(File.ReadAllText("doc111-esquema-instalado-mysql51.json"));
        ValidateSchema(schema);
        Action<Action<DataSet>> reject=change=>{var broken=schema.Copy();change(broken);Unavailable(()=>ValidateSchema(broken));};
        reject(s=>s.Tables["Tablas"].Rows.Remove(SchemaRow(s,"Tablas","TABLE_NAME","ra_integracion_evento")));
        reject(s=>SchemaRow(s,"Tablas","TABLE_NAME","ra_integracion_lote")["ENGINE"]="MyISAM");
        reject(s=>SchemaRow(s,"Columnas","COLUMN_NAME","bytes_previstos")["DATA_TYPE"]="int");
        reject(s=>SchemaRow(s,"Columnas","COLUMN_NAME","plan_id")["COLUMN_TYPE"]="int(10)");
        reject(s=>SchemaRow(s,"Columnas","COLUMN_NAME","IMPORTAR_CARPETAS")["COLUMN_DEFAULT"]="1");
        reject(s=>SchemaRow(s,"Columnas","COLUMN_NAME","cliente_archivo_id")["COLLATION_NAME"]="ascii_general_ci");
        reject(s=>SchemaRow(s,"Indices","INDEX_NAME","uq_integracion_lote_plan")["NON_UNIQUE"]=1);
        reject(s=>SchemaRow(s,"Indices","INDEX_NAME","uq_integracion_lote_confirmacion")["SUB_PART"]=1);
        reject(s=>SchemaRow(s,"Claves","CONSTRAINT_NAME","fk_integracion_lote_plan")["MISMO_ESQUEMA"]=0);
        reject(s=>SchemaRow(s,"Reglas","CONSTRAINT_NAME","fk_integracion_raiz_lote")["DELETE_RULE"]="CASCADE");
        reject(s=>s.Tables["Claves"].Rows.Remove(SchemaRow(s,"Claves","CONSTRAINT_NAME","fk_integracion_archivo_raiz")));
        ValidateSchema(schema); // Ninguna aceptación global se conserva tras el primer control.
        try { new MySqlImportacionCarpetasRepository("DOC111_UNSUPPORTED_OPTION=DOC111_SENTINEL").VerificarEsquema();throw new Exception("DOC111 bad connection accepted"); }
        catch(PerfilImportacionException e) {
            Check(e.Codigo=="CAPACIDAD_NO_DISPONIBLE" && !e.Message.Contains("DOC111_SENTINEL") && e.InnerException==null,"DOC111 connection details exposed");
        }
        Console.WriteLine("DOC-111 schema guards: passed");
    }

    static object RepositoryHelper(string method,params object[] args) {
        try { return typeof(MySqlImportacionCarpetasRepository).GetMethod(method,BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,args); }
        catch(TargetInvocationException e) { if(e.InnerException is PerfilImportacionException)throw e.InnerException;throw; }
    }
    static string Hex(byte[] value) { return BitConverter.ToString(value).Replace("-","").ToLowerInvariant(); }
    static byte[] Unhex(string value) { return Enumerable.Range(0,32).Select(i=>Convert.ToByte(value.Substring(i*2,2),16)).ToArray(); }
    static void Rejected(string code,Action operation) {
        try {operation();}catch(PerfilImportacionException e){Check(e.Codigo==code,"DOC111 incorrect rejection: "+e.Codigo);return;}
        throw new Exception("DOC111 invalid operation accepted");
    }
    static PlanImportacionCarpetas PlanFixture() {
        var result=new PlanImportacionCarpetas {
            EsquemaDoc=1,CreadoUtc="2026-10-27T20:00:00Z",ExpiraUtc="2026-10-27T20:10:00Z",
            Contexto=new ContextoImportacionCarpetas {UsuarioRef="1",AmbitoRef="2",NivelRef="3",RutaCanonica="Destino / Órdenes",SelloDestino="sello-fixture",PuedeImportar=true},
            Snapshot=new SnapshotPerfilImportacionOperativa {
                PerfilId="4",VersionId="5",NumeroVersion=1,
                Configuracion=new ConfiguracionPerfilImportacion {ModoIntegridad="COMPLETO_OBLIGATORIO",AmbitoIntegridad="LOTE"},
                Reglas=new List<ReglaPerfilImportacion>{new ReglaPerfilImportacion {ReglaId="6",DestinoTipo="CAMPO_EXPEDIENTE",DestinoRef="CODIGO",FuenteTipo="CONSTANTE",ValorConstante="000123"}}
            },Manifiesto=new ManifiestoImportacionCarpetas(),Resultado=new ResultadoPreparacionImportacion {PuedeConfirmar=true}
        };
        result.Snapshot.Huella=Hex(PerfilImportacionJson.Huella(new VersionPerfilImportacion{Configuracion=result.Snapshot.Configuracion,Reglas=result.Snapshot.Reglas}));
        foreach(var id in new[]{"r2","r1"}) {
            result.Manifiesto.Raices.Add(new RaizManifiestoImportacion {IdCliente=id,SeleccionId="s-"+id,Nombre="Órdenes"});
            result.Manifiesto.Archivos.Add(new ArchivoManifiestoImportacion {IdCliente="a1",RaizIdCliente=id,Nombre="000123.pdf",RutaRelativa="Órdenes/000123.pdf",Bytes="7"});
            result.Resultado.UnidadesElegibles.Add(id);
        }
        result.Resultado.Elementos.Add(new ValoresElementoImportacion {Ambito="ARCHIVO",RaizIdCliente="r1",ArchivoIdCliente="a1",Valores=new List<ValorReglaPerfilImportacion> {new ValorReglaPerfilImportacion {DestinoTipo="INDICE_GABINETE",DestinoRef="9",Diferido=true,PropiedadPendiente="ID",Valor=null}}});
        return result;
    }
    static PlanImportacionCarpetas Prepare(PlanImportacionCarpetas plan) {return (PlanImportacionCarpetas)RepositoryHelper("PrepararPersistencia",plan.Contexto,plan);}
    static DataTable StoredPlan(PlanImportacionCarpetas plan) {
        var row=new DataTable();
        foreach(var name in new[]{"plan_id","ambito_ref","usuario_ref","nivel_ref","version_id","estado","revision","esquema_doc","contexto_doc","config_doc","manifiesto_doc","resultado_doc","plan_hash","creado_utc","expira_utc"})row.Columns.Add(name,typeof(object));
        row.Rows.Add(plan.PlanId==null?(uint)12:Convert.ToUInt32(plan.PlanId),plan.Contexto.AmbitoRef,plan.Contexto.UsuarioRef,plan.Contexto.NivelRef,(uint)5,plan.Estado,plan.Revision,plan.EsquemaDoc,
            PerfilImportacionJson.Escribir(plan.Contexto),PerfilImportacionJson.Escribir(plan.Snapshot),PerfilImportacionJson.Escribir(plan.Manifiesto),PerfilImportacionJson.Escribir(plan.Resultado),Unhex(plan.Huella),DateTime.Parse(plan.CreadoUtc,System.Globalization.CultureInfo.InvariantCulture,System.Globalization.DateTimeStyles.AdjustToUniversal),DateTime.Parse(plan.ExpiraUtc,System.Globalization.CultureInfo.InvariantCulture,System.Globalization.DateTimeStyles.AdjustToUniversal));
        return row;
    }
    static PlanImportacionCarpetas ReadStored(DataTable table) {
        using(var reader=table.CreateDataReader()){reader.Read();return (PlanImportacionCarpetas)RepositoryHelper("LeerPlan",reader);}
    }
    static void PlanStorageCases() {
        var source=PlanFixture();var prepared=Prepare(source);
        var letterCase=PlanFixture();
        foreach(var root in letterCase.Manifiesto.Raices)root.IdCliente=root.IdCliente=="r1"?"rA":"ra";
        foreach(var file in letterCase.Manifiesto.Archivos)file.RaizIdCliente=file.RaizIdCliente=="r1"?"rA":"ra";
        letterCase.Resultado.UnidadesElegibles=new List<string>{"rA","ra"};letterCase.Resultado.Elementos[0].RaizIdCliente="rA";
        var distinct=Prepare(letterCase);
        Check(((List<ValorReglaPerfilImportacion>)RepositoryHelper("ValoresPlan",distinct,"rA","a1")).Count==1 && ((List<ValorReglaPerfilImportacion>)RepositoryHelper("ValoresPlan",distinct,"ra","a1")).Count==0,"DOC111 project Option Compare Text merges root identities");
        Check(prepared.Revision==1 && prepared.Estado=="REVISABLE" && prepared.PlanId==null,"DOC111 new plan identity or initial state invented");
        var reordered=PlanFixture();reordered.Manifiesto.Raices.Reverse();reordered.Manifiesto.Archivos.Reverse();reordered.Resultado.UnidadesElegibles.Reverse();
        Check(Prepare(reordered).Huella==prepared.Huella,"DOC111 display ordering changes canonical plan hash");
        source.Snapshot.Reglas[0].ValorConstante="changed";
        Check(prepared.Snapshot.Reglas[0].ValorConstante=="000123","DOC111 prepared snapshot is mutable through caller");
        var stored=StoredPlan(prepared);var restored=ReadStored(stored);
        Check(restored.PlanId=="12" && restored.Manifiesto.Raices.Count==2 && restored.Manifiesto.Archivos.Count==2 && restored.Resultado.Elementos[0].Valores[0].Diferido,"DOC111 persisted plan loses correlations or deferred values");
        Check(restored.Huella==prepared.Huella && restored.CreadoUtc==prepared.CreadoUtc,"DOC111 database datetime roundtrip changes hash");
        var large=PlanFixture();large.Manifiesto.Archivos[0].Bytes=UInt64.MaxValue.ToString();large.Manifiesto.Archivos[1].Bytes="0";
        Check(ReadStored(StoredPlan(Prepare(large))).Manifiesto.Archivos.Any(a=>a.Bytes==UInt64.MaxValue.ToString()),"DOC111 stored byte counts lose UInt64 precision");
        stored.Rows[0]["estado"]="CONFIRMADO";stored.Rows[0]["revision"]=(uint)2;
        Check(ReadStored(stored).Huella==prepared.Huella,"DOC111 confirmation invalidates immutable plan hash");
        Action<Action<DataRow>> tamper=change=>{var damaged=StoredPlan(prepared);change(damaged.Rows[0]);Unavailable(()=>ReadStored(damaged));};
        tamper(r=>r["manifiesto_doc"]=((string)r["manifiesto_doc"]).Replace("000123.pdf","000124.pdf"));
        tamper(r=>r["expira_utc"]=new DateTime(2026,10,27,20,11,0));
        tamper(r=>r["nivel_ref"]="99");
        tamper(r=>r["contexto_doc"]="{\"AuthorityFromClient\":true}");
        Action<Action<PlanImportacionCarpetas>> invalid=change=>{var p=PlanFixture();change(p);Rejected("SOLICITUD_INVALIDA",()=>Prepare(p));};
        invalid(p=>p.Manifiesto.Archivos[0].Bytes="01");
        invalid(p=>p.Manifiesto.Archivos[0].Bytes="1e3");
        invalid(p=>{p.Manifiesto.Archivos[0].Bytes=UInt64.MaxValue.ToString();p.Manifiesto.Archivos[1].Bytes="1";});
        invalid(p=>p.Manifiesto.Archivos[0].RutaRelativa="Órdenes/../000123.pdf");
        invalid(p=>p.Manifiesto.Archivos.Add(p.Manifiesto.Archivos[0]));
        invalid(p=>p.Resultado.Elementos[0].ArchivoIdCliente="unknown");
        invalid(p=>p.Snapshot.Huella=new string('0',64));
        invalid(p=>p.CreadoUtc="0001-01-01T00:00:00Z");
        invalid(p=>p.ExpiraUtc=p.CreadoUtc);
        invalid(p=>p.Resultado.Incidencias.Add(new IncidenciaImportacionCarpetas {BloqueaUnidad=true,Codigo="GLOBAL"}));
        var complete=Prepare(PlanFixture());RepositoryHelper("ValidarConfirmable",complete);
        complete.Resultado.UnidadesElegibles.Remove("r2");Rejected("CONFLICTO_REVISION",()=>RepositoryHelper("ValidarConfirmable",complete));
        complete.Snapshot.Configuracion.AmbitoIntegridad="EXPEDIENTE";RepositoryHelper("ValidarConfirmable",complete);
        complete.Resultado.Incidencias.Add(new IncidenciaImportacionCarpetas {RaizIdCliente="r1",ArchivoIdCliente="a1",BloqueaUnidad=true});
        Rejected("CONFLICTO_REVISION",()=>RepositoryHelper("ValidarConfirmable",complete));
        var denied=PlanFixture().Contexto;denied.PuedeImportar=false;
        var repo=new MySqlImportacionCarpetasRepository("DOC111_UNSUPPORTED_OPTION=DOC111_SENTINEL");
        Rejected("ACCESO_DENEGADO",()=>repo.ObtenerPlan(denied,"12"));
        Rejected("SOLICITUD_INVALIDA",()=>repo.ObtenerPlan(prepared.Contexto,"0012"));
        Rejected("SOLICITUD_INVALIDA",()=>repo.ConfirmarPlan(prepared.Contexto,"12",1,new string('0',64),"invalid key"));
        using(var connection=new MySql.Data.MySqlClient.MySqlConnection())
        using(var command=(MySql.Data.MySqlClient.MySqlCommand)RepositoryHelper("ComandoOperacion",connection,null,"SELECT @bytes,@hash",new object[]{"@bytes",UInt64.MaxValue,"@hash",new byte[32]})) {
            Check(command.Parameters["@bytes"].MySqlDbType==MySql.Data.MySqlClient.MySqlDbType.UInt64 && (UInt64)command.Parameters["@bytes"].Value==UInt64.MaxValue,"DOC111 connector truncates unsigned bytes");
            Check(command.Parameters["@hash"].Value is byte[] && ((byte[])command.Parameters["@hash"].Value).Length==32,"DOC111 binary fingerprint sent as text");
        }
        var prior=new LoteImportacionCarpetas {PlanId="12",LoteId="13",SolicitudHash=new string('0',64)};
        Check(((LoteImportacionCarpetas)RepositoryHelper("Repeticion",prior,"12",new byte[32])).LoteId=="13","DOC111 replay changes lot identity");
        Rejected("CONFLICTO_REVISION",()=>RepositoryHelper("Repeticion",prior,"14",new byte[32]));
        var changedHash=new byte[32];changedHash[0]=1;
        Rejected("CONFLICTO_REVISION",()=>RepositoryHelper("Repeticion",prior,"12",changedHash));
        Console.WriteLine("DOC-111 plan storage guards: passed");
    }

    sealed class CoincidenceQueryFixture {
        public string Charset="latin1",Collation="latin1_swedish_ci",CodeType="varchar";
        public bool Duplicate,Unrepresentable,MissingMetadata;
        public int Candidates,Reads,BusinessReads;
        public readonly List<string> Sql=new List<string>();
        public DataTable Query(string sql,object[] parameters) {
            Reads++;Sql.Add(sql);Check(sql.StartsWith("SELECT "),"DOC111 coincidence reader attempts mutation");
            var table=new DataTable();
            if(sql.Contains("information_schema.COLUMNS")) {
                foreach(var col in new[]{"COLUMN_NAME","DATA_TYPE","CHARACTER_SET_NAME","COLLATION_NAME","CHARACTER_MAXIMUM_LENGTH"})table.Columns.Add(col,typeof(object));
                table.Rows.Add("CODIGO_UNICO",CodeType,Charset,Collation,70);
                foreach(var col in new[]{"ID_EMPRESA_EXPEDIENTE","CODIGO_AREA_TRD","ID_EXPEDIENTE"})table.Rows.Add(col,"int",DBNull.Value,DBNull.Value,DBNull.Value);
                if(MissingMetadata)table.Rows.RemoveAt(0);return table;
            }
            var roots=new List<string>();for(int i=0;i<parameters.Length;i+=2)if(((string)parameters[i]).StartsWith("@raiz"))roots.Add((string)parameters[i+1]);
            if(sql.Contains("AS representable")) {
                Check(sql.Contains("HEX(CONVERT(") && sql.Contains("USING utf8"),"DOC111 representability not checked without loss");
                table.Columns.Add("raiz",typeof(string));table.Columns.Add("representable",typeof(int));
                foreach(var root in roots)table.Rows.Add(root,Unrepresentable?0:1);return table;
            }
            if(sql.Contains("INNER JOIN (")) {
                Check(sql.Contains("COLLATE ascii_bin AS raiz") && sql.Contains("COLLATE "+Collation+" AS codigo"),"DOC111 roots or codes use wrong comparison");
                table.Columns.Add("raiz",typeof(string));if(Duplicate)foreach(var root in roots)table.Rows.Add(root);return table;
            }
            BusinessReads++;
            Check(sql.EndsWith("LIMIT 2") && sql.Contains("e.ID_EMPRESA_EXPEDIENTE=@empresa") && sql.Contains("e.CODIGO_AREA_TRD=@area") && sql.Contains("e.CODIGO_UNICO=@codigo") && !sql.Contains("VOLUMEN_EXPEDIENTE="),"DOC111 candidates omit scope, parameterization or assume volume");
            Check(Convert.ToString(parameters[Array.IndexOf(parameters,"@empresa")+1])=="2" && Convert.ToString(parameters[Array.IndexOf(parameters,"@area")+1])=="3","DOC111 candidate actor/scope comes from wrong source");
            table.Columns.Add("ID_EXPEDIENTE",typeof(uint));for(uint i=1;i<=Candidates;i++)table.Rows.Add(i);return table;
        }
    }

    static void CoincidenceReadCases() {
        var plan=PlanFixture();plan.Snapshot.Configuracion.AreaRef="3";plan.Resultado.Elementos.Clear();
        foreach(var root in plan.Manifiesto.Raices)plan.Resultado.Elementos.Add(new ValoresElementoImportacion{Ambito="RAIZ",RaizIdCliente=root.IdCliente,Valores=new List<ValorReglaPerfilImportacion>{new ValorReglaPerfilImportacion{DestinoTipo="CAMPO_EXPEDIENTE",DestinoRef="CODIGO",Valor="000123'OR'1"}}});
        var fixture=new CoincidenceQueryFixture();
        Func<List<IncidenciaImportacionCarpetas>> read=()=> (List<IncidenciaImportacionCarpetas>)RepositoryHelper("LeerCoincidencias",plan.Contexto,plan.Snapshot,plan.Manifiesto,plan.Resultado,(Func<string,object[],DataTable>)fixture.Query);
        Check(read().Count==0 && fixture.BusinessReads==2,"DOC111 no-candidate plan not checked for every root");
        Check(fixture.Sql.All(sql=>!sql.Contains("000123'OR'1")),"DOC111 code interpolated into query");
        fixture.Candidates=1;Check(read().All(i=>i.Codigo=="CODIGO_EXPEDIENTE_EXISTENTE" && i.Ambito=="RAIZ" && i.BloqueaUnidad),"DOC111 BLOQUEAR ignores existing candidate");
        plan.Snapshot.Configuracion.PoliticaCoincidencias="REUTILIZAR_VALIDADO";Check(read().All(i=>i.Codigo=="REUTILIZACION_NO_DISPONIBLE"),"DOC111 unverified candidate reused or silently downgraded");
        fixture.Candidates=2;Check(read().All(i=>i.Codigo=="COINCIDENCIA_AMBIGUA"),"DOC111 chooses first ambiguous candidate");fixture.Candidates=0;
        fixture.Duplicate=true;var before=fixture.BusinessReads;Check(read().Count==2 && fixture.BusinessReads==before,"DOC111 collation duplicate roots not both blocked");fixture.Duplicate=false;
        fixture.Unrepresentable=true;Check(read().All(i=>i.Codigo=="CODIGO_NO_REPRESENTABLE") && fixture.BusinessReads==before,"DOC111 lossy code used for candidates");fixture.Unrepresentable=false;
        fixture.Collation="latin1_swedish_ci;DROP_TABLE";Unavailable(()=>read());fixture.Collation="latin1_swedish_ci";
        fixture.MissingMetadata=true;Unavailable(()=>read());fixture.MissingMetadata=false;
        fixture.CodeType="text";Unavailable(()=>read());fixture.CodeType="varchar";
        fixture.Charset="utf8mb4";Unavailable(()=>read());fixture.Charset="latin1";
        plan.Contexto.PuedeImportar=false;var reads=fixture.Reads;Rejected("ACCESO_DENEGADO",()=>read());Check(fixture.Reads==reads,"DOC111 denied reader reaches SQL");
        Console.WriteLine("DOC-111 parametrized coincidence reader: passed");
    }

    static void ConfirmationServiceCases() {
        var fixture=new ResourceFixture();var plan=PlanFixture();plan.Contexto=fixture.Adapter.ResolverDestino(fixture.Http,"7");
        foreach(var file in plan.Manifiesto.Archivos)file.UltimaModificacionLocal="0";
        plan.Snapshot.Configuracion=fixture.Configuration();
        plan.Snapshot.Configuracion.ModoIntegridad="COMPLETO_OBLIGATORIO";plan.Snapshot.Configuracion.AmbitoIntegridad="EXPEDIENTE";
        plan.Snapshot.Reglas.Insert(0,new ReglaPerfilImportacion{ReglaId="7",DestinoTipo="CAMPO_EXPEDIENTE",DestinoRef="NOMBRE",FuenteTipo="CONSTANTE",ValorConstante="Órdenes"});
        plan.Snapshot.Huella=Hex(PerfilImportacionJson.Huella(new VersionPerfilImportacion{Configuracion=plan.Snapshot.Configuracion,Reglas=plan.Snapshot.Reglas}));
        var stored=Prepare(plan);stored.PlanId="12";var now=new DateTime(2026,10,27,20,5,0,DateTimeKind.Utc);
        var publication=new PublicationPort{Value=plan.Snapshot};var catalog=new OperativeCatalogPort();
        var originalPublication=PerfilImportacionJson.Escribir(publication.Value);var originalStored=PerfilImportacionJson.Escribir(stored.Snapshot);
        Check(originalPublication!=originalStored&&PerfilImportacionJson.MismaPublicacion(publication.Value,stored.Snapshot),"DOC111 canonical persistence order rejects unchanged publication");
        Check(PerfilImportacionJson.Escribir(publication.Value)==originalPublication&&PerfilImportacionJson.Escribir(stored.Snapshot)==originalStored,"DOC111 publication comparison mutates source or persisted plan");
        foreach(var change in new Action<SnapshotPerfilImportacionOperativa>[] {s=>s.NumeroVersion++,s=>s.PerfilId="9",s=>s.VersionId="9",s=>s.Huella=new string('0',64),s=>s.Configuracion.NombreVisible="changed",s=>s.Reglas[0].ValorConstante="órdenes",s=>s.Reglas[0].Orden++,s=>s.Reglas.RemoveAt(0)}){
            var changed=PerfilImportacionJson.Leer<SnapshotPerfilImportacionOperativa>(originalPublication);change(changed);Check(!PerfilImportacionJson.MismaPublicacion(changed,stored.Snapshot),"DOC111 publication comparison ignores real changes");
        }
        var calls=0;var matchingCalls=0;var newConflict=false;
        Func<ContextoImportacionCarpetas,SnapshotPerfilImportacionOperativa,ManifiestoImportacionCarpetas,ResultadoPreparacionImportacion,List<IncidenciaImportacionCarpetas>> matching=(c,s,m,r)=>{matchingCalls++;return newConflict?new List<IncidenciaImportacionCarpetas>{new IncidenciaImportacionCarpetas{RaizIdCliente="r1",BloqueaUnidad=true}}:new List<IncidenciaImportacionCarpetas>();};
        var service=new ServicioRecepcionImportacion(()=>fixture.Adapter.Resolver(fixture.Http),(c,id)=>{Check(c.UsuarioRef=="1" && c.AmbitoRef=="2" && id=="12","DOC111 plan reference lookup not tied to actor");return stored.Contexto;},
            level=>fixture.Adapter.ResolverDestino(fixture.Http,level,"CONFIRMAR"),(c,id)=>stored,
            (c,config)=>fixture.Adapter.AutorizarRecursos(fixture.Http,c,config,"CONFIRMAR"),new AdaptadorPerfilImportacionOperativa(publication,catalog,null),matching,
            (c,id,revision,hash,key)=>{calls++;Check(c.NivelRef=="7" && id=="12" && hash==stored.Huella,"DOC111 confirmation uses mutable destination");if(revision!=1)throw new PerfilImportacionException("CONFLICTO_REVISION","Fixture");stored.Estado="CONFIRMADO";stored.Revision=2;return new LoteImportacionCarpetas{LoteId="20",PlanId=id,Contexto=c,Estado="RECIBIENDO",Revision=1,TotalArchivos=2,TotalRaices=2,TotalBytes="14"};},RequestLimits(16384),()=>now);
        var request=new ConfirmarPlanImportacionSolicitud{planId="12",revisionEsperada=1,planHashEsperado=stored.Huella,claveConfirmacion="confirmation-1"};
        publication.Value.Reglas[0].ValorConstante="órdenes";publication.Value.Huella=Hex(PerfilImportacionJson.Huella(new VersionPerfilImportacion{Configuracion=publication.Value.Configuracion,Reglas=publication.Value.Reglas}));
        Rejected("CONFLICTO_REVISION",()=>service.Confirmar(request));Check(calls==0,"DOC111 real published content change reaches confirmation writer");
        publication.Value.Reglas[0].ValorConstante="Órdenes";publication.Value.Huella=plan.Snapshot.Huella=stored.Snapshot.Huella;
        var hash=request.planHashEsperado;request.planHashEsperado=new string('0',64);Rejected("CONFLICTO_REVISION",()=>service.Confirmar(request));request.planHashEsperado=hash;
        now=now.AddDays(1);Rejected("CONFLICTO_REVISION",()=>service.Confirmar(request));now=now.AddDays(-1);
        newConflict=true;Rejected("CONFLICTO_REVISION",()=>service.Confirmar(request));newConflict=false;
        fixture.Moved=true;Rejected("CONFLICTO_REVISION",()=>service.Confirmar(request));fixture.Moved=false;
        fixture.CabinetAllowed=false;Rejected("ACCESO_DENEGADO",()=>service.Confirmar(request));fixture.CabinetAllowed=true;
        catalog.Incompatible=true;Rejected("CONFIGURACION_INVALIDA",()=>service.Confirmar(request));catalog.Incompatible=false;
        publication.Substitute=true;Rejected("ACCESO_DENEGADO",()=>service.Confirmar(request));publication.Substitute=false;
        Check(calls==0,"DOC111 failed validation reaches confirmation transaction");
        var confirmed=service.Confirmar(request);Check(confirmed.loteId=="20" && confirmed.nivelRef=="7" && confirmed.totalArchivos==2 && calls==1,"DOC111 valid confirmation loses result");
        var matched=matchingCalls;now=now.AddDays(1);catalog.Unavailable=true;
        var replay=service.Confirmar(request);Check(replay.loteId==confirmed.loteId && matchingCalls==matched && calls==2,"DOC111 replay revalidates expired plan or inactive publication as new confirmation");
        request.revisionEsperada=2;Rejected("CONFLICTO_REVISION",()=>service.Confirmar(request));request.revisionEsperada=1;
        fixture.ImportAllowed=false;var before=calls;Rejected("ACCESO_DENEGADO",()=>service.Confirmar(request));Check(calls==before,"DOC111 revoked authority reaches replay");fixture.ImportAllowed=true;
        var previous=HttpContext.Current;
        try {
            var constructor=typeof(WebServiceImportacionCarpetas).GetConstructors(BindingFlags.NonPublic|BindingFlags.Instance).Single();
            Func<HttpContext,ImportacionCarpetasContextAdapter,LimitesImportacionCarpetas,ServicioRecepcionImportacion> receiver=(h,a,l)=>service;
            var endpoint=(WebServiceImportacionCarpetas)constructor.Invoke(new object[]{fixture.Adapter,(Func<LimitesImportacionCarpetas>)(()=>RequestLimits(16384)),null,receiver,null,null});
            var worker=new ImportRequest{Body=System.Text.Encoding.UTF8.GetBytes(PerfilImportacionJson.Escribir(new SobreSolicitudPerfil<ConfirmarPlanImportacionSolicitud>{solicitud=request}))};
            var http=SessionFixture(worker:worker);worker.Token=ImportacionCarpetasRequestGuard.Emitir(http,RequestContext());HttpContext.Current=http;
            var output=endpoint.ConfirmarPlan(new ConfirmarPlanImportacionSolicitud{planId="999"});Check(output.ok && ((ConfirmacionImportacionResultado)output.datos).loteId=="20","DOC111 confirmation ASMX trusts coerced object");
            before=calls;
            var invalidWorker=new ImportRequest{Body=System.Text.Encoding.UTF8.GetBytes("{\"solicitud\":{\"planId\":\"12\",\"nivelRef\":\"9\"}}")};var ih=SessionFixture(worker:invalidWorker);invalidWorker.Token=ImportacionCarpetasRequestGuard.Emitir(ih,RequestContext());HttpContext.Current=ih;
            output=endpoint.ConfirmarPlan(request);Check(!output.ok && output.datos==null && calls==before,"DOC111 client destination accepted in confirmation");
        } finally {HttpContext.Current=previous;}
        Console.WriteLine("DOC-111 confirmation revalidation and ASMX replay: passed");
    }

    // Integración de persistencia, sin login/HTTP ni efectos documentales.
    // Solo se invoca con autorización explícita y contexto/snapshot de fixture
    // publicados y comprobados por el ejecutor. El Run() local nunca la llama.
    public static string[] RunPersistenceIntegration(bool authorized,string connection,ContextoImportacionCarpetas context,SnapshotPerfilImportacionOperativa snapshot) {
        if(!authorized)throw new Exception("DOC111_PERSISTENCE_AUTHORIZATION_REQUIRED");
        if(context==null || snapshot==null)throw new Exception("DOC111_PERSISTENCE_AUTHORIZED_FIXTURE_REQUIRED");
        var clock=DateTime.UtcNow;clock=new DateTime(clock.Ticks-clock.Ticks%TimeSpan.TicksPerSecond,DateTimeKind.Utc);
        Func<PlanImportacionCarpetas> fixture=()=>{
            var plan=PlanFixture();
            plan.Contexto=PerfilImportacionJson.Leer<ContextoImportacionCarpetas>(PerfilImportacionJson.Escribir(context));
            plan.Snapshot=PerfilImportacionJson.Leer<SnapshotPerfilImportacionOperativa>(PerfilImportacionJson.Escribir(snapshot));
            plan.Resultado.Elementos.Clear();
            plan.CreadoUtc=clock.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'",System.Globalization.CultureInfo.InvariantCulture);
            plan.ExpiraUtc=clock.AddMinutes(10).ToString("yyyy-MM-dd'T'HH:mm:ss'Z'",System.Globalization.CultureInfo.InvariantCulture);
            return plan;
        };
        var repo=new MySqlImportacionCarpetasRepository(connection,()=>clock);
        var saved=repo.GuardarPlan(context,fixture());
        Console.WriteLine("DOC111_PERSISTENCE_PLAN_ID: "+saved.PlanId);
        var roundtrip=repo.ObtenerPlan(context,saved.PlanId);
        Check(roundtrip.Huella==saved.Huella && roundtrip.Manifiesto.Archivos.Count==2,"DOC111 integration plan roundtrip failed");
        var key="e2e-"+Guid.NewGuid().ToString("N");
        var gate=new System.Threading.Barrier(2);
        Func<System.Threading.Tasks.Task<LoteImportacionCarpetas>> confirm=()=>System.Threading.Tasks.Task.Factory.StartNew(()=>{
            if(!gate.SignalAndWait(TimeSpan.FromSeconds(20)))throw new Exception("DOC111_CONFIRMATION_GATE_TIMEOUT");
            return new MySqlImportacionCarpetasRepository(connection,()=>clock).ConfirmarPlan(context,saved.PlanId,saved.Revision,saved.Huella,key);
        });
        LoteImportacionCarpetas lote;
        try {
            var a=confirm();var b=confirm();System.Threading.Tasks.Task.WaitAll(a,b);
            Check(a.Result.LoteId==b.Result.LoteId,"DOC111 concurrent confirmation created different lots");
            lote=a.Result;
            Console.WriteLine("DOC111_PERSISTENCE_LOT_ID: "+lote.LoteId);
            Check(lote.TotalRaices==2 && lote.TotalArchivos==2 && lote.TotalBytes=="14" && lote.Contexto.NivelRef==context.NivelRef,"DOC111 lot totals or destination changed");
        } finally {gate.Dispose();}
        var expiredClock=new MySqlImportacionCarpetasRepository(connection,()=>clock.AddMinutes(11));
        Check(expiredClock.ConfirmarPlan(context,saved.PlanId,saved.Revision,saved.Huella,key).LoteId==lote.LoteId,"DOC111 confirmed replay incorrectly requires live plan TTL");
        Check(repo.ConfirmarPlan(context,saved.PlanId,saved.Revision,saved.Huella,"other-"+Guid.NewGuid().ToString("N")).LoteId==lote.LoteId,"DOC111 plan confirmed twice under another key");
        var other=repo.GuardarPlan(context,fixture());
        Console.WriteLine("DOC111_PERSISTENCE_SECOND_PLAN_ID: "+other.PlanId);
        Rejected("CONFLICTO_REVISION",()=>repo.ConfirmarPlan(context,other.PlanId,other.Revision,other.Huella,key));
        Rejected("CONFLICTO_REVISION",()=>expiredClock.ConfirmarPlan(context,other.PlanId,other.Revision,other.Huella,"expired-"+Guid.NewGuid().ToString("N")));
        Check(repo.ObtenerPlan(context,saved.PlanId).Estado=="CONFIRMADO","DOC111 plan not confirmed durably");
        Check(repo.ObtenerPlan(context,other.PlanId).Estado=="REVISABLE","DOC111 rejected confirmation changed plan");
        using(var db=new MySql.Data.MySqlClient.MySqlConnection(connection)) {
            db.Open();
            Func<string,uint,long> count=(sql,id)=>{using(var cmd=new MySql.Data.MySqlClient.MySqlCommand(sql,db)){cmd.CommandTimeout=15;cmd.Parameters.AddWithValue("@id",id);return Convert.ToInt64(cmd.ExecuteScalar());}};
            var lotId=UInt32.Parse(lote.LoteId,System.Globalization.CultureInfo.InvariantCulture);
            Check(count("SELECT COUNT(*) FROM ra_integracion_lote WHERE plan_id=@id",UInt32.Parse(saved.PlanId))==1,"DOC111 duplicate lot inventory");
            Check(count("SELECT COUNT(*) FROM ra_integracion_raiz WHERE lote_id=@id",lotId)==2,"DOC111 roots not atomic");
            Check(count("SELECT COUNT(*) FROM ra_integracion_archivo a INNER JOIN ra_integracion_raiz r ON r.raiz_id=a.raiz_id WHERE r.lote_id=@id",lotId)==2,"DOC111 files not atomic");
            Check(count("SELECT COUNT(*) FROM ra_integracion_evento WHERE lote_id=@id AND codigo='LOTE_CONFIRMADO'",lotId)==1,"DOC111 replay duplicates event");
            Check(count("SELECT COUNT(*) FROM ra_integracion_intento WHERE lote_id=@id",lotId)==0,"DOC111 confirmation started effects");
            Check(count("SELECT COUNT(*) FROM ra_integracion_lote WHERE plan_id=@id",UInt32.Parse(other.PlanId))==0,"DOC111 rejected confirmation left partial lot");
        }
        // Se conservan fixtures auditables; no DELETE/TRUNCATE ni efectos sobre configuración.
        return new[]{saved.PlanId,other.PlanId,lote.LoteId};
    }

    public static void RunPersistedReadChecks(string connection,ContextoImportacionCarpetas context,string planId,string lotId) {
        var repo=new MySqlImportacionCarpetasRepository(connection);
        var plan=repo.ObtenerPlan(context,planId);var lot=repo.ObtenerLote(context,lotId);
        Check(plan.Estado=="CONFIRMADO" && plan.Revision==2 && lot.Revision==1,"DOC111 persisted revisions changed after replay");
        var differentOwner=PerfilImportacionJson.Leer<ContextoImportacionCarpetas>(PerfilImportacionJson.Escribir(context));
        // Contextos negativos del test de repositorio; no sesiones ni cuentas
        // autenticadas. Se conserva la misma empresa de la instalación.
        differentOwner.UsuarioRef=UInt32.MaxValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
        Rejected("ACCESO_DENEGADO",()=>repo.ObtenerPlan(differentOwner,planId));
        Rejected("ACCESO_DENEGADO",()=>repo.ObtenerLote(differentOwner,lotId));
        var differentLevel=PerfilImportacionJson.Leer<ContextoImportacionCarpetas>(PerfilImportacionJson.Escribir(context));
        differentLevel.NivelRef=UInt32.MaxValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
        Rejected("ACCESO_DENEGADO",()=>repo.ObtenerPlan(differentLevel,planId));
        Rejected("ACCESO_DENEGADO",()=>repo.ObtenerLote(differentLevel,lotId));
        var changedDestination=PerfilImportacionJson.Leer<ContextoImportacionCarpetas>(PerfilImportacionJson.Escribir(context));
        changedDestination.SelloDestino="changed-by-negative-read-test";
        Rejected("CONFLICTO_REVISION",()=>repo.ObtenerPlan(changedDestination,planId));
        Rejected("CONFLICTO_REVISION",()=>repo.ObtenerLote(changedDestination,lotId));
        Console.WriteLine("DOC111_PERSISTENCE_READ_ISOLATION_PASS");
    }

    public static string[] RunRollbackIntegration(bool authorized,string connection,ContextoImportacionCarpetas context,string planId,string existingLotId) {
        return RunRollbackIntegration(authorized,connection,context,planId,existingLotId,"1");
    }
    public static string[] RunRollbackIntegration(bool authorized,string connection,ContextoImportacionCarpetas context,string planId,string existingLotId,string committedPlanId) {
        if(!authorized)throw new Exception("DOC111_ROLLBACK_AUTHORIZATION_REQUIRED");
        var normal=new MySqlImportacionCarpetasRepository(connection);
        var original=normal.ObtenerPlan(context,planId);
        Check(original.Estado=="REVISABLE" && original.Revision==1,"DOC111 rollback requires original unconfirmed fixture");
        var clock=DateTime.ParseExact(original.CreadoUtc,"yyyy-MM-dd'T'HH:mm:ss'Z'",System.Globalization.CultureInfo.InvariantCulture,System.Globalization.DateTimeStyles.AssumeUniversal|System.Globalization.DateTimeStyles.AdjustToUniversal).AddSeconds(1);
        // Reloj de fixture dentro del TTL original; no se modifica la fecha guardada.
        var probeType=typeof(Action<string,MySql.Data.MySqlClient.MySqlConnection,MySql.Data.MySqlClient.MySqlTransaction>);
        var constructor=typeof(MySqlImportacionCarpetasRepository).GetConstructor(BindingFlags.NonPublic|BindingFlags.Instance,null,new[]{typeof(string),typeof(Func<DateTime>),probeType},null);
        Check(constructor!=null,"DOC111 internal rollback observer constructor missing");
        Check(typeof(MySqlImportacionCarpetasRepository).GetConstructors().All(c=>c.GetParameters().Length==2),"DOC111 rollback observer exposed as public constructor");
        var stages=new[]{"ANTES_EVENTO","EVENTO_INSERTADO","PLAN_CONFIRMADO"};
        foreach(var target in stages) {
            var reached=0;
            Action<string,MySql.Data.MySqlClient.MySqlConnection,MySql.Data.MySqlClient.MySqlTransaction> probe=(stage,db,tx)=>{
                if(stage!=target)return;
                reached++;
                Check(tx!=null && Object.ReferenceEquals(tx.Connection,db),"DOC111 observer outside production transaction");
                Func<string,long> count=sql=>{using(var cmd=new MySql.Data.MySqlClient.MySqlCommand(sql,db,tx)){cmd.CommandTimeout=15;cmd.Parameters.AddWithValue("@plan",UInt32.Parse(planId));return Convert.ToInt64(cmd.ExecuteScalar());}};
                Check(count("SELECT COUNT(*) FROM ra_integracion_lote WHERE plan_id=@plan")==1,"DOC111 provisional lot not visible in transaction");
                Check(count("SELECT COUNT(*) FROM ra_integracion_raiz r INNER JOIN ra_integracion_lote l ON l.lote_id=r.lote_id WHERE l.plan_id=@plan")==2,"DOC111 provisional roots missing");
                Check(count("SELECT COUNT(*) FROM ra_integracion_archivo a INNER JOIN ra_integracion_raiz r ON r.raiz_id=a.raiz_id INNER JOIN ra_integracion_lote l ON l.lote_id=r.lote_id WHERE l.plan_id=@plan")==2,"DOC111 provisional files missing");
                Check(count("SELECT COUNT(*) FROM ra_integracion_evento e INNER JOIN ra_integracion_lote l ON l.lote_id=e.lote_id WHERE l.plan_id=@plan")== (target=="ANTES_EVENTO"?0:1),"DOC111 provisional event unexpected");
                Check(count("SELECT COUNT(*) FROM ra_integracion_plan WHERE plan_id=@plan AND estado='CONFIRMADO' AND revision=2")== (target=="PLAN_CONFIRMADO"?1:0),"DOC111 provisional plan state unexpected");
                throw new PerfilImportacionException("PRUEBA_ROLLBACK","Fallo controlado de la prueba de transacción.");
            };
            var failing=(MySqlImportacionCarpetasRepository)constructor.Invoke(new object[]{connection,new Func<DateTime>(()=>clock),probe});
            Rejected("PRUEBA_ROLLBACK",()=>failing.ConfirmarPlan(context,planId,original.Revision,original.Huella,"rollback-"+Guid.NewGuid().ToString("N")));
            Check(reached==1,"DOC111 rollback checkpoint not reached exactly once");
            var after=normal.ObtenerPlan(context,planId);
            Check(after.Estado==original.Estado && after.Revision==original.Revision && after.Huella==original.Huella,"DOC111 failed confirmation changed original plan");
            using(var db=new MySql.Data.MySqlClient.MySqlConnection(connection)) {
                db.Open();
                using(var cmd=new MySql.Data.MySqlClient.MySqlCommand("SELECT COUNT(*) FROM ra_integracion_lote WHERE plan_id=@plan",db)) {
                    cmd.CommandTimeout=15;cmd.Parameters.AddWithValue("@plan",UInt32.Parse(planId));
                    Check(Convert.ToInt64(cmd.ExecuteScalar())==0,"DOC111 failed confirmation committed a lot");
                }
            }
            Console.WriteLine("DOC111_ROLLBACK_STAGE_PASS: "+target);
        }
        var committed=normal.ObtenerPlan(context,committedPlanId);
        Check(normal.ConfirmarPlan(context,committed.PlanId,1,committed.Huella,"post-rollback-"+Guid.NewGuid().ToString("N")).LoteId==existingLotId,"DOC111 rollback damaged committed replay");
        return stages;
    }

    static HttpContext SessionFixture(string user="1",string company="2",HttpWorkerRequest worker=null,string sessionId="doc111-context-fixture") {
        var http=new HttpContext(worker??new SimpleWorkerRequest("/",Path.GetTempPath(),"doc111.aspx","",new StringWriter()));
        SessionStateUtility.AddHttpSessionStateToContext(http,new HttpSessionStateContainer(sessionId,new SessionStateItemCollection(),new HttpStaticObjectsCollection(),20,true,HttpCookieMode.UseCookies,SessionStateMode.InProc,false));
        http.Session["GA_IDUSUARIOGESTION"]=user;http.Session["GA_IDEMPRESA"]=company;http.Session["GA_LOGINUSUARIOGESTION"]="authorized-fixture";
        http.Session["IMPORTAR_CARPETAS"]=1;http.Session["PuedeImportar"]=true;http.Session["PuedeAdministrar"]=true;
        return http;
    }
    static DataTable AuthorityFixture() {
        var data=new DataTable();foreach(var name in new[]{"usuario_ref","empresa_ref","usuario_activo","empresa_existente","puede_importar"})data.Columns.Add(name,typeof(object));
        data.Rows.Add("1","2",1,"2",1);return data;
    }
    static void ContextAuthorityCases() {
        var http=SessionFixture();var data=AuthorityFixture();var reads=0;
        Func<HttpContext,uint,DataTable> query=(request,id)=>{reads++;Check(Object.ReferenceEquals(request,http) && id==1,"DOC111 authority query changed user");return data.Copy();};
        var constructor=typeof(ImportacionCarpetasContextAdapter).GetConstructor(BindingFlags.NonPublic|BindingFlags.Instance,null,new[]{typeof(Func<HttpContext,uint,DataTable>)},null);
        var adapter=(ImportacionCarpetasContextAdapter)constructor.Invoke(new object[]{query});
        var allowed=adapter.Resolver(http);
        Check(allowed.PuedeImportar && allowed.UsuarioRef=="1" && allowed.AmbitoRef=="2" && allowed.NivelRef==null,"DOC111 operational context invents destination or loses identity");
        Check(typeof(ContextoImportacionCarpetas).GetProperty("PuedeAdministrar")==null,"DOC111 operational context requires administrator");
        data.Rows[0]["puede_importar"]=0;Rejected("ACCESO_DENEGADO",()=>adapter.Resolver(http));
        data.Rows[0]["puede_importar"]=DBNull.Value;Rejected("ACCESO_DENEGADO",()=>adapter.Resolver(http));
        data.Rows[0]["puede_importar"]=1;data.Rows[0]["usuario_activo"]=0;Rejected("ACCESO_DENEGADO",()=>adapter.Resolver(http));
        data=AuthorityFixture();data.Rows[0]["empresa_existente"]=DBNull.Value;Rejected("ACCESO_DENEGADO",()=>adapter.Resolver(http));
        data=AuthorityFixture();data.Rows.Add(data.Rows[0].ItemArray);Rejected("ACCESO_DENEGADO",()=>adapter.Resolver(http));
        data=AuthorityFixture();data.Clear();Rejected("ACCESO_DENEGADO",()=>adapter.Resolver(http));
        data=AuthorityFixture();data.Columns.Remove("puede_importar");Unavailable(()=>adapter.Resolver(http));
        data=AuthorityFixture();var priorReads=reads;http.Session["GA_LOGINUSUARIOGESTION"]="CONSULTAPUBLICO";
        Rejected("SESION_REQUERIDA",()=>adapter.Resolver(http));
        http.Session["GA_LOGINUSUARIOGESTION"]="authorized-fixture";http.Session["GA_IDUSUARIOGESTION"]="01";
        Rejected("SESION_REQUERIDA",()=>adapter.Resolver(http));
        http.Session["GA_IDUSUARIOGESTION"]="1";http.Session["GA_IDEMPRESA"]="0";
        Rejected("ACCESO_DENEGADO",()=>adapter.Resolver(http));
        Rejected("SESION_REQUERIDA",()=>adapter.Resolver(null));
        Check(reads==priorReads,"DOC111 invalid session reaches authority query");
        http.Session["GA_IDEMPRESA"]="2";Check(adapter.Resolver(http).PuedeImportar && reads==priorReads+1,"DOC111 resolver caches old authority");
        Console.WriteLine("DOC-111 operational authority: passed");
    }

    public static void RunContextReadIntegration(string connection) {
        // Session local con configuración RA en memoria. No login HTTP, no
        // contraseña almacenada ni permiso concedido en el ambiente.
        var http=SessionFixture("141","2");
        var builder=new MySql.Data.MySqlClient.MySqlConnectionStringBuilder(connection);
        http.Session["RA_IP_SERVER_MODULO"]=builder.Server;http.Session["RA_DB_NAME_MODULO"]=builder.Database;
        http.Session["RA_USER_DBMS_MODULO"]=builder.UserID;http.Session["RA_PASW_DBMS_MODULO"]=builder.Password;http.Session["RA_TYPE_DBMS_MODULO"]="mysql";
        var adapter=new ImportacionCarpetasContextAdapter();
        Rejected("ACCESO_DENEGADO",()=>adapter.Resolver(http));
        http.Session["PuedeImportar"]=true;http.Session["IMPORTAR_CARPETAS"]=1;
        Rejected("ACCESO_DENEGADO",()=>adapter.Resolver(http));
        Console.WriteLine("DOC111_REAL_PERMISSION_ZERO_DENIED_DESPITE_SESSION_FLAG");
    }

    sealed class ResourceFixture {
        public bool LevelAllowed=true,CabinetAllowed=true,ImportAllowed=true,Cycle=false,MissingParent=false,RelationAllowed=true,AddLevel=true,UploadLevel=true,Moved=false,DaUserAllowed=true;
        public string RootName="Compras",RaRealm="ra-A",DaRealm="da-A",CabinetFlag="1",TrdFlag="1",UnitFlag="1",Owner="1";
        public int Reads,CabinetReads,IdentityReads;
        public readonly HttpContext Http=SessionFixture();
        public readonly ImportacionCarpetasContextAdapter Adapter;
        public readonly Func<string,string,object[],DataTable> Query;
        static object Parameter(object[] parameters,string name) {return parameters[Array.IndexOf(parameters,name)+1];}
        static DataTable Table(params string[] columns) {var result=new DataTable();foreach(var column in columns)result.Columns.Add(column,typeof(object));return result;}
        public ResourceFixture() {
            Func<HttpContext,uint,DataTable> identity=(http,id)=>{IdentityReads++;var data=AuthorityFixture();data.Rows[0]["puede_importar"]=ImportAllowed?1:0;return data;};
            Func<string,string,object[],DataTable> resources=(origin,sql,parameters)=>{
                Reads++;
                if(sql.Contains("@@hostname")) {var t=Table("host","puerto","servidor","base");t.Rows.Add("host-"+origin,"3306","1",origin=="RA"?RaRealm:DaRealm);return t;}
                if(sql.StartsWith("SELECT n.id_nivel FROM")) {
                    Check(origin=="RA" && Convert.ToString(Parameter(parameters,"@usuario"))=="1" && Convert.ToString(Parameter(parameters,"@empresa"))=="2","DOC111 level authority source or actor changed");
                    var sharedAllowed=(!sql.Contains("p.agregar_expediente=1")||AddLevel)&&(!sql.Contains("p.carga_archivo=1")||UploadLevel);
                    var t=Table("id_nivel");if(LevelAllowed && (Owner=="1"||sharedAllowed))t.Rows.Add(7);return t;
                }
                if(sql.StartsWith("SELECT n.id_nivel,n.id_nivel_padre")) {
                    Check(origin=="RA","DOC111 ancestor queried outside RA");var t=Table("id_nivel","id_nivel_padre","nombre_nivel","estado_nivel","propietario");
                    var id=Convert.ToString(Parameter(parameters,"@nivel"));
                    if(id=="7")t.Rows.Add(7,Moved?5:3,"Órdenes",1,Owner);
                    else if(!MissingParent)t.Rows.Add(Convert.ToUInt32(id),Cycle?7:0,RootName,1,Owner);
                    return t;
                }
                if(sql.Contains("SELECT RELACION_DA")) {Check(origin=="RA","DOC111 DA relation resolved outside RA");var t=Table("RELACION_DA");if(RelationAllowed)t.Rows.Add(10);return t;}
                if(sql.Contains("FROM usuarios_da")) {Check(origin=="DA" && sql.Contains("Relacion_Gestion=@usuario"),"DOC111 DA reciprocal identity not checked");var t=Table("Clave_Usuario");if(DaUserAllowed)t.Rows.Add(10);return t;}
                if(sql.Contains("information_schema.COLUMNS")) {
                    Check(origin=="DA","DOC111 index metadata queried outside DA");
                    var t=Table("id","CAMPO","ALEAS_CAMPO","IS_REQUIRED_BACKEND","DATA_TYPE","COLUMN_TYPE","NUMERIC_PRECISION","NUMERIC_SCALE","CHARACTER_SET_NAME","CHARACTER_MAXIMUM_LENGTH");
                    t.Rows.Add(99,"order_number","Order number",0,"varchar","varchar(20)",DBNull.Value,DBNull.Value,"utf8",20);return t;
                }
                if(sql.StartsWith("SELECT id,nombre FROM") && sql.Contains("FROM system1")) {
                    Check(origin=="DA" && sql.Contains("ALMACENA_IMAGEN=1"),"DOC111 operative cabinet catalog drops authorization");
                    var t=Table("id","nombre");if(CabinetAllowed && Convert.ToString(Parameter(parameters,"@ref"))=="7")t.Rows.Add(7,"Synthetic cabinet");return t;
                }
                if(sql.Contains("FROM system1")) {
                    Check(origin=="DA" && sql.Contains("ALMACENA_IMAGEN=1") && sql.Contains("permisos_usuarios_gabinetes") && sql.Contains("permisos_grupos_gabinetes"),"DOC111 cabinet storage permission omitted");
                    Check(Convert.ToString(Parameter(parameters,"@usuarioDA"))=="10","DOC111 RA actor used as DA identity");CabinetReads++;
                    var t=Table("id_gabinete","INVENTARIO_DOCUMENTAL","APLICA_TRD","ASIGNA_UNIDAD");if(CabinetAllowed && Convert.ToString(Parameter(parameters,"@gabinete"))=="7")t.Rows.Add(7,CabinetFlag,TrdFlag,UnitFlag);return t;
                }
                throw new Exception("DOC111 unexpected resource query");
            };
            var constructor=typeof(ImportacionCarpetasContextAdapter).GetConstructor(BindingFlags.NonPublic|BindingFlags.Instance,null,new[]{identity.GetType(),resources.GetType()},null);
            Query=resources;
            Adapter=(ImportacionCarpetasContextAdapter)constructor.Invoke(new object[]{identity,resources});
        }
        public ConfiguracionPerfilImportacion Configuration() {
            using(var sha=System.Security.Cryptography.SHA256.Create()) {
                return new ConfiguracionPerfilImportacion{GabineteId=7,OrigenGabinete="DA",HuellaOrigenGabinete=Hex(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(PerfilImportacionJson.Escribir(new[]{"host-DA","3306","1",DaRealm}))))};
            }
        }
    }
    static void ConfigureSyntheticLegacyRa(HttpContext http){
        http.Session["RA_TYPE_DBMS_MODULO"]="mysql";http.Session["RA_IP_SERVER_MODULO"]="synthetic-host";http.Session["RA_DB_NAME_MODULO"]="synthetic-db";http.Session["RA_USER_DBMS_MODULO"]="synthetic-user";http.Session["RA_PASW_DBMS_MODULO"]="synthetic-only";http.Session["RA_ACTIVA_POOL_DBMS"]="1";http.Session["RA_NUMERO_DBMS_CONEX"]="10";
    }

    static ComandoCreacionExpedienteImportacion AuthorizedCreationCommand(ResourceFixture fixture){
        var source=new CreationCommandFixture();source.Context=fixture.Adapter.ResolverDestino(fixture.Http,"7","CREAR_EXPEDIENTE");source.Lot.Contexto=ReceiptClone(source.Context);source.Lot.Snapshot.Configuracion.HuellaOrigenGabinete=fixture.Configuration().HuellaOrigenGabinete;source.Root.ValoresPlan[1].Valor="Órdenes";source.Rehash();return source.Prepare();
    }

    static void LegacyCreationAuthorityCases(){
        var previous=HttpContext.Current;
        try{
            var f=new ResourceFixture();ConfigureSyntheticLegacyRa(f.Http);HttpContext.Current=f.Http;var command=AuthorizedCreationCommand(f);
            var keys=new[]{"GA_IDUSUARIOGESTION","GA_IDEMPRESA","RA_IP_SERVER_MODULO","RA_DB_NAME_MODULO","RA_USER_DBMS_MODULO","RA_PASW_DBMS_MODULO","RA_ACTIVA_POOL_DBMS","RA_NUMERO_DBMS_CONEX"};
            var original=keys.Select(k=>Convert.ToString(f.Http.Session[k])).ToArray();
            var allowed=f.Adapter.AutorizarCreacionLegacy(f.Http,command);Check(allowed.NivelRef=="7"&&allowed.SelloDestino==command.Contexto.SelloDestino&&original.SequenceEqual(keys.Select(k=>Convert.ToString(f.Http.Session[k]))),"DOC111 legacy authority redirects destination or changes session");
            f.ImportAllowed=false;Rejected("ACCESO_DENEGADO",()=>f.Adapter.AutorizarCreacionLegacy(f.Http,command));f.ImportAllowed=true;
            f.Http.Session["GA_IDUSUARIOGESTION"]="9";Rejected("ACCESO_DENEGADO",()=>f.Adapter.AutorizarCreacionLegacy(f.Http,command));f.Http.Session["GA_IDUSUARIOGESTION"]="1";
            f.Http.Session["GA_IDEMPRESA"]="9";Rejected("ACCESO_DENEGADO",()=>f.Adapter.AutorizarCreacionLegacy(f.Http,command));f.Http.Session["GA_IDEMPRESA"]="2";
            f.Moved=true;Rejected("CONFLICTO_REVISION",()=>f.Adapter.AutorizarCreacionLegacy(f.Http,command));f.Moved=false;
            f.CabinetAllowed=false;Rejected("ACCESO_DENEGADO",()=>f.Adapter.AutorizarCreacionLegacy(f.Http,command));f.CabinetAllowed=true;
            f.RelationAllowed=false;Rejected("ACCESO_DENEGADO",()=>f.Adapter.AutorizarCreacionLegacy(f.Http,command));f.RelationAllowed=true;
            f.DaUserAllowed=false;Rejected("ACCESO_DENEGADO",()=>f.Adapter.AutorizarCreacionLegacy(f.Http,command));f.DaUserAllowed=true;
            f.RaRealm="redirected-origin";Rejected("CONFLICTO_REVISION",()=>f.Adapter.AutorizarCreacionLegacy(f.Http,command));f.RaRealm="ra-A";
            var reads=f.Reads;HttpContext.Current=SessionFixture();Rejected("SESION_REQUERIDA",()=>f.Adapter.AutorizarCreacionLegacy(f.Http,command));Check(f.Reads==reads,"DOC111 foreign current request reaches resources");HttpContext.Current=f.Http;
            foreach(var pair in new[]{new[]{"RA_DB_NAME_MODULO","synthetic-db;database=redirected"},new[]{"RA_IP_SERVER_MODULO","synthetic-host;Port=3307"},new[]{"RA_USER_DBMS_MODULO","synthetic-user;SslMode=None"},new[]{"RA_PASW_DBMS_MODULO","synthetic-only;Pooling=False"},new[]{"RA_ACTIVA_POOL_DBMS","true"},new[]{"RA_NUMERO_DBMS_CONEX","0"},new[]{"RA_NUMERO_DBMS_CONEX","invalid"},new[]{"RA_TYPE_DBMS_MODULO","other"}}){
                var saved=f.Http.Session[pair[0]];f.Http.Session[pair[0]]=pair[1];reads=f.Reads;bool refused=false;try{f.Adapter.AutorizarCreacionLegacy(f.Http,command);}catch(PerfilImportacionException e){Check(e.Codigo=="CAPACIDAD_NO_DISPONIBLE","DOC111 connection error leaked outside contract");refused=true;}Check(refused,"DOC111 incompatible connection accepted for session key "+pair[0]);Check(f.Reads==reads,"DOC111 incompatible effective connection reaches RA/DA query");f.Http.Session[pair[0]]=saved;
            }
            f.Http.Session["RA_NUMERO_DBMS_CONEX"]="";f.Adapter.AutorizarCreacionLegacy(f.Http,command);f.Http.Session["RA_NUMERO_DBMS_CONEX"]="10";
            Console.WriteLine("DOC-111 legacy creation authority and effective RA connection (synthetic session/SELECT only): passed");
        }finally{HttpContext.Current=previous;}
    }

    static void ResourceAuthorizationCases() {
        var fixture=new ResourceFixture();
        var context=fixture.Adapter.ResolverDestino(fixture.Http,"7");
        Check(context.NivelRef=="7" && context.RutaCanonica=="Compras / Órdenes" && context.SelloDestino.Length==64 && !context.RutaCanonica.Contains("host-RA"),"DOC111 canonical destination not server derived");
        Check(fixture.Adapter.ResolverDestino(fixture.Http,"7").SelloDestino==context.SelloDestino,"DOC111 stable graph changes seal");
        fixture.Adapter.AutorizarRecursos(fixture.Http,context,fixture.Configuration(),"CONFIRMAR");
        Check(fixture.CabinetReads==1,"DOC111 importer without administrator cannot authorize cabinet");
        fixture.LevelAllowed=false;Rejected("ACCESO_DENEGADO",()=>fixture.Adapter.ResolverDestino(fixture.Http,"7"));fixture.LevelAllowed=true;
        fixture.ImportAllowed=false;var before=fixture.Reads;Rejected("ACCESO_DENEGADO",()=>fixture.Adapter.ResolverDestino(fixture.Http,"7"));Check(before==fixture.Reads,"DOC111 revoked import permission reaches resources");fixture.ImportAllowed=true;
        fixture.RootName="<Compras renombradas>";Check(fixture.Adapter.ResolverDestino(fixture.Http,"7").SelloDestino!=context.SelloDestino,"DOC111 ancestor rename does not invalidate seal");
        before=fixture.CabinetReads;Rejected("CONFLICTO_REVISION",()=>fixture.Adapter.AutorizarRecursos(fixture.Http,context,fixture.Configuration(),"CONFIRMAR"));Check(before==fixture.CabinetReads,"DOC111 stale destination reaches cabinet");fixture.RootName="Compras";
        fixture.RaRealm="ra-B";Check(fixture.Adapter.ResolverDestino(fixture.Http,"7").SelloDestino!=context.SelloDestino,"DOC111 same IDs in another RA origin preserve seal");fixture.RaRealm="ra-A";
        fixture.Moved=true;var moved=fixture.Adapter.ResolverDestino(fixture.Http,"7");Check(moved.RutaCanonica==context.RutaCanonica && moved.SelloDestino!=context.SelloDestino,"DOC111 same labels after moving level preserve destination identity");Rejected("CONFLICTO_REVISION",()=>fixture.Adapter.AutorizarRecursos(fixture.Http,context,fixture.Configuration(),"CONFIRMAR"));fixture.Moved=false;
        fixture.Owner="9";Check(fixture.Adapter.ResolverDestino(fixture.Http,"7").SelloDestino!=context.SelloDestino,"DOC111 changed owner preserves seal");fixture.Owner="1";
        fixture.Owner="9";fixture.AddLevel=false;
        Rejected("ACCESO_DENEGADO",()=>fixture.Adapter.ResolverDestino(fixture.Http,"7","ABRIR"));
        Rejected("ACCESO_DENEGADO",()=>fixture.Adapter.ResolverDestino(fixture.Http,"7","CREAR_EXPEDIENTE"));
        foreach(var action in new[]{"DETENER_LOTE","RECONCILIAR","LIMPIAR_STAGING"})Rejected("ACCESO_DENEGADO",()=>fixture.Adapter.ResolverDestino(fixture.Http,"7",action));
        Check(fixture.Adapter.ResolverDestino(fixture.Http,"7","RECIBIR_ARCHIVO").NivelRef=="7","DOC111 upload action demands create permission");
        fixture.AddLevel=true;fixture.UploadLevel=false;
        Rejected("ACCESO_DENEGADO",()=>fixture.Adapter.ResolverDestino(fixture.Http,"7","PREPARAR"));
        Check(fixture.Adapter.ResolverDestino(fixture.Http,"7","CREAR_EXPEDIENTE").NivelRef=="7","DOC111 create action demands upload permission");
        Rejected("ACCESO_DENEGADO",()=>fixture.Adapter.ResolverDestino(fixture.Http,"7","ALMACENAR_DOCUMENTO"));
        foreach(var action in new[]{"DETENER_LOTE","RECONCILIAR","LIMPIAR_STAGING"})Rejected("ACCESO_DENEGADO",()=>fixture.Adapter.ResolverDestino(fixture.Http,"7",action));
        fixture.UploadLevel=true;fixture.Owner="1";
        fixture.Cycle=true;Rejected("ACCESO_DENEGADO",()=>fixture.Adapter.ResolverDestino(fixture.Http,"7"));fixture.Cycle=false;
        fixture.MissingParent=true;Rejected("ACCESO_DENEGADO",()=>fixture.Adapter.ResolverDestino(fixture.Http,"7"));fixture.MissingParent=false;
        fixture.CabinetAllowed=false;Rejected("ACCESO_DENEGADO",()=>fixture.Adapter.AutorizarRecursos(fixture.Http,context,fixture.Configuration(),"RECIBIR_ARCHIVO"));fixture.CabinetAllowed=true;
        fixture.CabinetFlag="0";Rejected("ACCESO_DENEGADO",()=>fixture.Adapter.AutorizarRecursos(fixture.Http,context,fixture.Configuration(),"ALMACENAR_DOCUMENTO"));fixture.CabinetFlag="1";
        fixture.TrdFlag="0";Rejected("ACCESO_DENEGADO",()=>fixture.Adapter.AutorizarRecursos(fixture.Http,context,fixture.Configuration(),"ALMACENAR_DOCUMENTO"));fixture.TrdFlag="1";
        fixture.UnitFlag="0";Rejected("ACCESO_DENEGADO",()=>fixture.Adapter.AutorizarRecursos(fixture.Http,context,fixture.Configuration(),"ALMACENAR_DOCUMENTO"));fixture.UnitFlag="1";
        fixture.RelationAllowed=false;Rejected("ACCESO_DENEGADO",()=>fixture.Adapter.AutorizarRecursos(fixture.Http,context,fixture.Configuration(),"CREAR_EXPEDIENTE"));fixture.RelationAllowed=true;
        fixture.DaUserAllowed=false;Rejected("ACCESO_DENEGADO",()=>fixture.Adapter.AutorizarRecursos(fixture.Http,context,fixture.Configuration(),"CREAR_EXPEDIENTE"));fixture.DaUserAllowed=true;
        var config=fixture.Configuration();fixture.DaRealm="da-B";Rejected("ACCESO_DENEGADO",()=>fixture.Adapter.AutorizarRecursos(fixture.Http,context,config,"CONFIRMAR"));fixture.DaRealm="da-A";
        config=fixture.Configuration();config.GabineteId=8;Rejected("ACCESO_DENEGADO",()=>fixture.Adapter.AutorizarRecursos(fixture.Http,context,config,"CONFIRMAR"));
        before=fixture.Reads;Rejected("SOLICITUD_INVALIDA",()=>fixture.Adapter.ResolverDestino(fixture.Http,"7 OR 1=1"));Rejected("SOLICITUD_INVALIDA",()=>fixture.Adapter.ResolverDestino(fixture.Http,"7","UNSUPPORTED"));Check(before==fixture.Reads,"DOC111 invalid destination reaches SQL");
        Console.WriteLine("DOC-111 destination and cabinet authority: passed");
    }

    sealed class ImportRequest : SimpleWorkerRequest {
        readonly MemoryStream response=new MemoryStream();
        public string ResponseText{get{return System.Text.Encoding.UTF8.GetString(response.ToArray());}}
        public override void SendResponseFromMemory(byte[] data,int length){response.Write(data,0,length);}
        public byte[] Body=System.Text.Encoding.UTF8.GetBytes("{\"solicitud\":{\"nivelRef\":\"7\"}}");
        public string Mime="application/json; charset=utf-8",Verb="POST",Token,AdminToken,Origin,FetchSite;
        public int BodyReads;
        public ImportRequest():base("/",Path.GetTempPath(),"doc111.asmx","",new StringWriter()){}
        public override string GetHttpVerbName(){return Verb;}
        public override string GetServerName(){return "test.local";}
        public override int GetLocalPort(){return 80;}
        public override string GetKnownRequestHeader(int index){
            if(index==HttpWorkerRequest.HeaderContentType)return Mime;
            if(index==HttpWorkerRequest.HeaderContentLength)return Body.Length.ToString(System.Globalization.CultureInfo.InvariantCulture);
            if(index==HttpWorkerRequest.HeaderHost)return "test.local";
            return base.GetKnownRequestHeader(index);
        }
        public override string[][] GetUnknownRequestHeaders(){
            var headers=new List<string[]>();
            if(Token!=null)headers.Add(new[]{ImportacionCarpetasRequestGuard.CabeceraToken,Token});
            if(AdminToken!=null)headers.Add(new[]{"X-Importacion-Perfil-CSRF",AdminToken});
            if(Origin!=null)headers.Add(new[]{"Origin",Origin});
            if(FetchSite!=null)headers.Add(new[]{"Sec-Fetch-Site",FetchSite});
            return headers.ToArray();
        }
        public override byte[] GetPreloadedEntityBody(){BodyReads++;return Body;}
        public override bool IsEntireEntityBodyIsPreloaded(){return true;}
        public override int GetTotalEntityBodyLength(){return Body.Length;}
    }
    static LimitesImportacionCarpetas RequestLimits(int manifest=1024,ulong file=1024){return new LimitesImportacionCarpetas(2,4,file,4096,manifest,TimeSpan.FromMinutes(1),TimeSpan.FromHours(1));}
    static ContextoImportacionCarpetas RequestContext(){return new ContextoImportacionCarpetas{UsuarioRef="1",AmbitoRef="2",PuedeImportar=true};}
    static byte[] Multipart(bool duplicate=false,bool extra=false,bool withFile=true) {
        const string boundary="doc111-boundary";var body=new System.Text.StringBuilder();
        foreach(var field in new[]{new[]{"loteId","1"},new[]{"archivoId","2"},new[]{"claveIntento","attempt-1"}})body.Append("--"+boundary+"\r\nContent-Disposition: form-data; name=\""+field[0]+"\"\r\n\r\n"+field[1]+"\r\n");
        if(duplicate)body.Append("--"+boundary+"\r\nContent-Disposition: form-data; name=\"loteId\"\r\n\r\n1\r\n");
        if(extra)body.Append("--"+boundary+"\r\nContent-Disposition: form-data; name=\"usuarioRef\"\r\n\r\n99\r\n");
        if(withFile)body.Append("--"+boundary+"\r\nContent-Disposition: form-data; name=\"file\"; filename=\"ignored.pdf\"\r\nContent-Type: application/pdf\r\n\r\n%PDF\r\n");
        body.Append("--"+boundary+"--\r\n");return System.Text.Encoding.UTF8.GetBytes(body.ToString());
    }
    static void RequestGuardCases() {
        var worker=new ImportRequest{Origin="http://test.local"};var http=SessionFixture(worker:worker);var context=RequestContext();
        var reference=ImportacionCarpetasRequestGuard.ReferenciaRecuperacion(http,context);Check(reference.Length==32&&reference==ImportacionCarpetasRequestGuard.ReferenciaRecuperacion(http,context),"DOC111 recovery reference is not stable in session");
        http.Session["RA_DB_NAME_MODULO"]="synthetic-realm-b";var otherReference=ImportacionCarpetasRequestGuard.ReferenciaRecuperacion(http,context);Check(otherReference!=reference,"DOC111 another RA realm reuses browser references");http.Session["RA_DB_NAME_MODULO"]=null;
        worker.Token=ImportacionCarpetasRequestGuard.Emitir(http,context);
        Check(worker.Token==ImportacionCarpetasRequestGuard.Emitir(http,context),"DOC111 token changes between same-user tabs");
        Check(ImportacionCarpetasRequestGuard.LeerJson<AbrirImportacionSolicitud>(http,context,RequestLimits()).nivelRef=="7","DOC111 valid JSON not parsed");
        Action<Action<ImportRequest,HttpContext,ContextoImportacionCarpetas>,string> deny=(change,code)=>{
            var w=new ImportRequest();var h=SessionFixture(worker:w);var c=RequestContext();w.Token=ImportacionCarpetasRequestGuard.Emitir(h,c);change(w,h,c);
            Rejected(code,()=>ImportacionCarpetasRequestGuard.LeerJson<AbrirImportacionSolicitud>(h,c,RequestLimits()));
        };
        deny((w,h,c)=>w.Token=null,"SOLICITUD_NO_VERIFICADA");
        deny((w,h,c)=>w.Token=new string('?',44),"SOLICITUD_NO_VERIFICADA");
        deny((w,h,c)=>w.Verb="GET","SOLICITUD_NO_VERIFICADA");
        deny((w,h,c)=>w.Mime="text/plain","SOLICITUD_NO_VERIFICADA");
        deny((w,h,c)=>w.Mime="","SOLICITUD_NO_VERIFICADA");
        deny((w,h,c)=>w.Mime="application/json; charset=iso-8859-1","SOLICITUD_NO_VERIFICADA");
        deny((w,h,c)=>w.Origin="https://foreign.test","SOLICITUD_NO_VERIFICADA");
        deny((w,h,c)=>w.Origin="null","SOLICITUD_NO_VERIFICADA");
        deny((w,h,c)=>w.Origin="http://test.local/unexpected-path","SOLICITUD_NO_VERIFICADA");
        deny((w,h,c)=>w.FetchSite="cross-site","SOLICITUD_NO_VERIFICADA");
        deny((w,h,c)=>c.PuedeImportar=false,"SOLICITUD_NO_VERIFICADA");
        deny((w,h,c)=>h.Session["GA_IDUSUARIOGESTION"]="9","SOLICITUD_NO_VERIFICADA");
        deny((w,h,c)=>w.Body=System.Text.Encoding.UTF8.GetBytes("{\"solicitud\":{\"nivelRef\":\"7\",\"usuarioRef\":\"99\"}}"),"CONFIGURACION_INVALIDA");
        deny((w,h,c)=>w.Body=System.Text.Encoding.UTF8.GetBytes("{\"solicitud\":{\"nivelRef\":\"7\",\"nivelRef\":\"8\"}}"),"CONFIGURACION_INVALIDA");
        deny((w,h,c)=>w.Body=System.Text.Encoding.UTF8.GetBytes("{\"solicitud\":null}"),"SOLICITUD_INVALIDA");
        deny((w,h,c)=>w.Body=new byte[]{0xc3,0x28},"SOLICITUD_INVALIDA");
        var over=new ImportRequest();var overHttp=SessionFixture(worker:over);over.Token=ImportacionCarpetasRequestGuard.Emitir(overHttp,context);
        Rejected("LIMITE_EXCEDIDO",()=>ImportacionCarpetasRequestGuard.LeerJson<AbrirImportacionSolicitud>(overHttp,context,RequestLimits(8)));
        Check(over.BodyReads==0,"DOC111 over-limit JSON body consumed before rejection");
        var noPolicy=new ImportRequest();var np=SessionFixture(worker:noPolicy);noPolicy.Token=ImportacionCarpetasRequestGuard.Emitir(np,context);
        Unavailable(()=>ImportacionCarpetasRequestGuard.LeerJson<AbrirImportacionSolicitud>(np,context,null));Check(noPolicy.BodyReads==0,"DOC111 unconfigured request reaches body");
        var otherWorker=new ImportRequest();var other=SessionFixture(worker:otherWorker,sessionId:"another-session");ImportacionCarpetasRequestGuard.Emitir(other,context);otherWorker.Token=worker.Token;
        Rejected("SOLICITUD_NO_VERIFICADA",()=>ImportacionCarpetasRequestGuard.LeerJson<AbrirImportacionSolicitud>(other,context,RequestLimits()));
        var mixed=new ImportRequest();var mixedHttp=SessionFixture(worker:mixed);ImportacionCarpetasRequestGuard.Emitir(mixedHttp,context);
        mixed.Token=PerfilImportacionRequestGuard.Emitir(mixedHttp,new ContextoPerfilImportacion{UsuarioRef="1",AmbitoRef="2",PuedeAdministrar=true});
        Rejected("SOLICITUD_NO_VERIFICADA",()=>ImportacionCarpetasRequestGuard.LeerJson<AbrirImportacionSolicitud>(mixedHttp,context,RequestLimits()));
        var multipart=new ImportRequest{Body=Multipart(),Mime="multipart/form-data; boundary=doc111-boundary"};var mh=SessionFixture(worker:multipart);multipart.Token=ImportacionCarpetasRequestGuard.Emitir(mh,context);
        var received=ImportacionCarpetasRequestGuard.LeerMultipart(mh,context,RequestLimits());Check(received.loteId=="1" && received.archivoId=="2" && mh.Request.Files[0].ContentLength==4,"DOC111 valid multipart references changed");
        Action<ImportRequest,string,LimitesImportacionCarpetas> denyMultipart=(w,code,limits)=>{var h=SessionFixture(worker:w);w.Token=ImportacionCarpetasRequestGuard.Emitir(h,context);Rejected(code,()=>ImportacionCarpetasRequestGuard.LeerMultipart(h,context,limits));};
        denyMultipart(new ImportRequest{Body=Multipart(true),Mime="multipart/form-data; boundary=doc111-boundary"},"SOLICITUD_INVALIDA",RequestLimits());
        denyMultipart(new ImportRequest{Body=Multipart(false,true),Mime="multipart/form-data; boundary=doc111-boundary"},"SOLICITUD_INVALIDA",RequestLimits());
        denyMultipart(new ImportRequest{Body=Multipart(false,false,false),Mime="multipart/form-data; boundary=doc111-boundary"},"SOLICITUD_INVALIDA",RequestLimits());
        denyMultipart(new ImportRequest{Body=Multipart(),Mime="multipart/form-data; boundary=doc111-boundary"},"LIMITE_EXCEDIDO",RequestLimits(file:3));
        var tooBig=new ImportRequest{Body=Multipart(),Mime="multipart/form-data; boundary=doc111-boundary"};denyMultipart(tooBig,"LIMITE_EXCEDIDO",RequestLimits(manifest:8,file:8));Check(tooBig.BodyReads==0,"DOC111 multipart envelope exceeds quota before parsing");
        denyMultipart(new ImportRequest{Body=Multipart(),Mime="multipart/form-data; boundary=doc111-boundary; boundary=other"},"SOLICITUD_NO_VERIFICADA",RequestLimits());
        var unauthorized=new ImportRequest{Body=Multipart(),Mime="multipart/form-data; boundary=doc111-boundary"};var uh=SessionFixture(worker:unauthorized);ImportacionCarpetasRequestGuard.Emitir(uh,context);
        Rejected("SOLICITUD_NO_VERIFICADA",()=>ImportacionCarpetasRequestGuard.LeerMultipart(uh,context,RequestLimits()));Check(unauthorized.BodyReads==0,"DOC111 unauthenticated multipart parsed before token validation");
        Console.WriteLine("DOC-111 operational request guard: passed");
    }

    static void AuthenticatedRevocationCases(){
        var previous=HttpContext.Current;
        try{
            var fixture=new ResourceFixture();var repository=new QueryRepository();repository.Lot.Contexto=fixture.Adapter.ResolverDestino(fixture.Http,"7");repository.Lot.Snapshot.Configuracion=fixture.Configuration();
            int factories=0,policyReads=0;
            Func<HttpContext,ImportacionCarpetasContextAdapter,ServicioConsultaImportacion> query=(http,a)=>{factories++;return new ServicioConsultaImportacion(()=>a.Resolver(http),repository,n=>a.ResolverDestino(http,n,"CONSULTAR"),(c,config)=>a.AutorizarRecursos(http,c,config,"CONSULTAR"),50);};
            Func<HttpContext,ImportacionCarpetasContextAdapter,LimitesImportacionCarpetas,ServicioPreparacionImportacion> prepare=(h,a,l)=>{factories++;throw new Exception("Revoked request reaches preparation");};
            Func<HttpContext,ImportacionCarpetasContextAdapter,LimitesImportacionCarpetas,ServicioRecepcionImportacion> receipt=(h,a,l)=>{factories++;throw new Exception("Revoked request reaches receipt");};
            Func<HttpContext,ImportacionCarpetasContextAdapter,string,CoordinadorImportacionCarpetas> effect=(h,a,p)=>{factories++;throw new Exception("Revoked request reaches phase/effect");};
            Func<LimitesImportacionCarpetas> policy=()=>{policyReads++;return RequestLimits(16384);};
            var endpoint=(WebServiceImportacionCarpetas)typeof(WebServiceImportacionCarpetas).GetConstructors(BindingFlags.NonPublic|BindingFlags.Instance).Single().Invoke(new object[]{fixture.Adapter,policy,prepare,receipt,query,effect});
            var firstWorker=new ImportRequest{Body=System.Text.Encoding.UTF8.GetBytes("{\"solicitud\":{\"loteId\":\"20\"}}")};var first=SessionFixture(worker:firstWorker,sessionId:"doc111-still-authenticated");
            var cached=fixture.Adapter.Resolver(first);firstWorker.Token=ImportacionCarpetasRequestGuard.Emitir(first,cached);HttpContext.Current=first;
            Check(endpoint.ConsultarLote(new ConsultarLoteImportacionSolicitud{loteId="20"}).ok,"Revocation fixture has no previously authorized request");
            var savedLot=PerfilImportacionJson.Escribir(repository.Lot);var token=firstWorker.Token;var factoryCount=factories;var policyCount=policyReads;var resourceReads=fixture.Reads;
            fixture.ImportAllowed=false;
            var operations=new[]{"AbrirImportacion","ListarPerfilesImportacion","PrepararPlan","ConfirmarPlan","ConsultarLote","ConsultarConfirmacionLote","ConsultarRecursosLote","ExportarPaginaLote","AvanzarLote","SolicitarDetencion","ReconciliarRecurso","CerrarReservaSinInicio","RecuperarDocumentoV2"};
            foreach(var operation in operations){
                var method=typeof(WebServiceImportacionCarpetas).GetMethod(operation);var request=Activator.CreateInstance(method.GetParameters().Single().ParameterType);
                var worker=new ImportRequest{Body=System.Text.Encoding.UTF8.GetBytes(PerfilImportacionJson.Escribir(new{solicitud=request})),Token=token};var next=SessionFixture(worker:worker,sessionId:first.Session.SessionID);
                foreach(string key in first.Session.Keys)next.Session[key]=first.Session[key];
                Check(next.Session["IMPORTAR_CARPETAS"].Equals(1)&&next.Session["PuedeImportar"].Equals(true)&&ImportacionCarpetasRequestGuard.Emitir(next,cached)==token,"Revocation fixture lost cached permission/token/session identity");
                HttpContext.Current=next;int identityReads=fixture.IdentityReads;
                var result=(RespuestaPerfilImportacion)method.Invoke(endpoint,new[]{request});
                Check(!result.ok&&result.codigo=="ACCESO_DENEGADO"&&result.datos==null&&next.Response.StatusCode==403,"Revoked authenticated request not denied: "+operation);
                Check(fixture.IdentityReads==identityReads+1&&factories==factoryCount&&policyReads==policyCount&&fixture.Reads==resourceReads&&worker.BodyReads==0,"Revoked request reads body/resources or reaches effect port: "+operation);
            }
            var uploadWorker=new ImportRequest{Token=token};var uploadHttp=SessionFixture(worker:uploadWorker,sessionId:first.Session.SessionID);foreach(string key in first.Session.Keys)uploadHttp.Session[key]=first.Session[key];
            var upload=(ImportacionCarpetasUpload)typeof(ImportacionCarpetasUpload).GetConstructors(BindingFlags.NonPublic|BindingFlags.Instance).Single().Invoke(new object[]{fixture.Adapter,policy,receipt});
            upload.ProcessRequest(uploadHttp);uploadHttp.Response.Flush();
            Check(uploadHttp.Response.StatusCode==403&&uploadWorker.ResponseText.Contains("ACCESO_DENEGADO")&&factories==factoryCount&&policyReads==policyCount&&uploadWorker.BodyReads==0,"Revoked multipart enters staging/receipt");
            Check(PerfilImportacionJson.Escribir(repository.Lot)==savedLot,"Revocation changes persisted fixture lot");
            Console.WriteLine("DOC111 5.9 authenticated stale session/token rejected across 12 ASMX operations and upload (synthetic HTTP/authority): passed");
        }finally{HttpContext.Current=previous;}
    }
    static void AdminOperativeCompatibilityCases(){
        foreach(var import in new[]{false,true})foreach(var admin in new[]{false,true}){
            var fixture=new ResourceFixture{ImportAllowed=import};var worker=new ImportRequest();var http=SessionFixture(worker:worker);
            // Los headers pertenecen a una peticion: establecer ambos antes
            // de que HttpRequest los materialice/cachee por primera vez.
            if(import)worker.Token=ImportacionCarpetasRequestGuard.Emitir(http,fixture.Adapter.Resolver(http));
            var adminContext=new ContextoPerfilImportacion{UsuarioRef="1",AmbitoRef="2",PuedeAdministrar=admin};
            var repository=new PerfilImportacionServiceBehaviorTests.RepositoryDouble();var catalogs=new PerfilImportacionServiceBehaviorTests.CatalogDouble();var service=new ServicioPerfilImportacion(repository,catalogs);
            var filter=new FiltroPerfilesImportacion{Pagina=1,TamanoPagina=20};string adminToken=null,adminBefore=null;
            if(admin){adminToken=PerfilImportacionRequestGuard.Emitir(http,adminContext);worker.AdminToken=adminToken;PerfilImportacionRequestGuard.Validar(http,adminContext);adminBefore=PerfilImportacionJson.Escribir(service.Listar(adminContext,filter));}
            else{Rejected("SOLICITUD_NO_VERIFICADA",()=>PerfilImportacionRequestGuard.Emitir(http,adminContext));Rejected("ACCESO_DENEGADO",()=>service.Listar(adminContext,filter));}
            var contextBefore=PerfilImportacionJson.Escribir(adminContext);
            if(import){var context=fixture.Adapter.ResolverDestino(http,"7");worker.Token=ImportacionCarpetasRequestGuard.Emitir(http,context);Check(ImportacionCarpetasRequestGuard.LeerJson<AbrirImportacionSolicitud>(http,context,RequestLimits()).nivelRef=="7","Operative input changed");fixture.Adapter.AutorizarRecursos(http,context,fixture.Configuration(),"PREPARAR");}
            else Rejected("ACCESO_DENEGADO",()=>fixture.Adapter.ResolverDestino(http,"7"));
            Check(PerfilImportacionJson.Escribir(adminContext)==contextBefore,"Operative read changes administration context");
            if(admin){Check(PerfilImportacionRequestGuard.Emitir(http,adminContext)==adminToken,"Operative token replaces administrative token");PerfilImportacionRequestGuard.Validar(http,adminContext);Check(PerfilImportacionJson.Escribir(service.Listar(adminContext,filter))==adminBefore&&Object.ReferenceEquals(repository.LastFilter,filter),"Operative read changes DOC110 administrative listing");Rejected("SOLICITUD_INVALIDA",()=>service.Listar(adminContext,new FiltroPerfilesImportacion{Termino=new string('x',161)}));}
            else Rejected("ACCESO_DENEGADO",()=>service.Listar(adminContext,filter));
            Check(repository.Writes==0,"Operative read changes administrative profile");
        }
        Console.WriteLine("DOC111 6.6 DOC110 admin inputs/tokens/listing/rejections preserved across import/admin matrix (shared doubles): passed");
    }
    static void ImportAdminMatrixCases() {
        AuthenticatedRevocationCases();AdminOperativeCompatibilityCases();
        foreach(var import in new[]{false,true})foreach(var admin in new[]{false,true}) {
            var fixture=new ResourceFixture{ImportAllowed=import};
            var worker=new ImportRequest();var http=SessionFixture(worker:worker);http.Session["PuedeAdministrar"]=admin;
            if(import) {
                var context=fixture.Adapter.ResolverDestino(http,"7");
                worker.Token=ImportacionCarpetasRequestGuard.Emitir(http,context);
                Check(ImportacionCarpetasRequestGuard.LeerJson<AbrirImportacionSolicitud>(http,context,RequestLimits()).nivelRef=="7","DOC111 importer blocked without admin permission");
                fixture.Adapter.AutorizarRecursos(http,context,fixture.Configuration(),"CONFIRMAR");
            } else Rejected("ACCESO_DENEGADO",()=>fixture.Adapter.ResolverDestino(http,"7"));
            var adminWorker=new ImportRequest();var ah=SessionFixture(worker:adminWorker);
            var adminContext=new ContextoPerfilImportacion{UsuarioRef="1",AmbitoRef="2",PuedeAdministrar=admin};
            if(admin) {adminWorker.AdminToken=PerfilImportacionRequestGuard.Emitir(ah,adminContext);PerfilImportacionRequestGuard.Validar(ah,adminContext);}
            else Rejected("SOLICITUD_NO_VERIFICADA",()=>PerfilImportacionRequestGuard.Emitir(ah,adminContext));
        }
        Console.WriteLine("DOC-111 import/admin capability matrix: passed");
    }

    static SnapshotPerfilImportacionOperativa SnapshotReadFixture() {
        var source=PlanFixture().Snapshot;source.Huella=Hex(PerfilImportacionJson.Huella(new VersionPerfilImportacion{Configuracion=source.Configuracion,Reglas=source.Reglas}));return source;
    }
    static void PublishedReadCases() {
        var context=PlanFixture().Contexto;var snapshot=SnapshotReadFixture();
        var profile=new PerfilImportacion{PerfilId="4",AmbitoRef="2",Activo=true,VersionPublicadaId="6",Version=new VersionPerfilImportacion{PerfilId="4",VersionId="5",Numero=1,Estado="PUBLICADA",Configuracion=snapshot.Configuracion,Reglas=snapshot.Reglas}};
        var method=typeof(MySqlPerfilImportacionRepository).GetMethod("SnapshotOperativo",BindingFlags.NonPublic|BindingFlags.Static);
        Func<SnapshotPerfilImportacionOperativa> read=()=>{try{return (SnapshotPerfilImportacionOperativa)method.Invoke(null,new object[]{context,profile,"5",Unhex(snapshot.Huella)});}catch(TargetInvocationException e){if(e.InnerException is PerfilImportacionException)throw e.InnerException;throw;}};
        var result=read();Check(result.VersionId=="5" && result.NumeroVersion==1,"DOC111 requested historical publication replaced by latest");
        profile.Version.Reglas[0].ValorConstante="mutated";Check(result.Reglas[0].ValorConstante=="000123","DOC111 read snapshot shares mutable source");Unavailable(()=>read());profile.Version.Reglas[0].ValorConstante="000123";
        profile.Activo=false;Rejected("ACCESO_DENEGADO",()=>read());profile.Activo=true;
        profile.Version.Estado="BORRADOR";Rejected("ACCESO_DENEGADO",()=>read());profile.Version.Estado="PUBLICADA";
        profile.Version.VersionId="6";Rejected("ACCESO_DENEGADO",()=>read());profile.Version.VersionId="5";
        Check(typeof(IPerfilImportacionLecturaOperativa).GetMethods().All(m=>!new[]{"Crear","Guardar","Publicar","CambiarDisponibilidad","CrearVersion"}.Contains(m.Name)),"DOC111 import port exposes administrative mutations");
        var repository=new MySqlPerfilImportacionRepository("DOC111_UNSUPPORTED_OPTION=DOC111_SENTINEL");
        var denied=new ContextoImportacionCarpetas{UsuarioRef="1",AmbitoRef="2",PuedeImportar=false};
        Rejected("ACCESO_DENEGADO",()=>repository.ObtenerPublicacion(denied,"5"));
        Rejected("ACCESO_DENEGADO",()=>repository.Obtener(new ContextoPerfilImportacion{UsuarioRef="1",AmbitoRef="2",PuedeAdministrar=false},"4","5"));
        Console.WriteLine("DOC-111 published operational snapshots: passed");
    }

    static void OperativeCatalogCases() {
        var fixture=new ResourceFixture();var context=fixture.Adapter.ResolverDestino(fixture.Http,"7");
        var adapter=new PerfilImportacionCatalogAdapter("RA","DA",fixture.Query);var config=fixture.Configuration();
        Check(adapter.ConsultarOperativo(context,"GABINETE",config,1,1,"7").Single().value=="7","DOC111 import-only catalog unavailable");
        var rules=new List<ReglaPerfilImportacion>{new ReglaPerfilImportacion{DestinoTipo="INDICE_GABINETE",DestinoRef="99",FuenteTipo="ARCHIVO",TipoValor="TEXTO",LongitudMaxima=20}};
        Check(adapter.ValidarOperativo(context,config,rules).Count==0,"DOC111 shared operative catalog validation differs");
        Check(adapter.ValidarValoresOperativos(context,config,new List<ValorReglaPerfilImportacion>{new ValorReglaPerfilImportacion{DestinoTipo="INDICE_GABINETE",DestinoRef="99",FuenteTipo="ARCHIVO",TipoValor="TEXTO",Valor="000123"}}).Count==0,"DOC111 operative values lose backend metadata constraints");
        Check(adapter.ValidarValoresOperativos(context,config,new List<ValorReglaPerfilImportacion>{new ValorReglaPerfilImportacion{DestinoTipo="INDICE_GABINETE",DestinoRef="99",Valor=new string('x',21)}}).Count==1,"DOC111 operative index length not enforced");
        var reads=fixture.Reads;context.PuedeImportar=false;Rejected("ACCESO_DENEGADO",()=>adapter.ConsultarOperativo(context,"GABINETE",config,1,1,"7"));Check(reads==fixture.Reads,"DOC111 denied catalog reaches IO");context.PuedeImportar=true;
        Rejected("SOLICITUD_INVALIDA",()=>adapter.Consultar(new ContextoPerfilImportacion{UsuarioRef="1",AmbitoRef="2",PuedeAdministrar=false},"GABINETE",config,1,1,"7"));
        fixture.CabinetAllowed=false;Check(adapter.ValidarOperativo(context,config,rules).Any(e=>e.Codigo=="GABINETE_NO_AUTORIZADO"),"DOC111 operative catalog ignores revoked cabinet permission");
        Console.WriteLine("DOC-111 operative shared catalogs: passed");
    }

    sealed class PublicationPort : IPerfilImportacionLecturaOperativa {
        public string Requested;public bool Substitute;
        public SnapshotPerfilImportacionOperativa Value=SnapshotReadFixture();
        public void VerificarEsquema(){}
        public SnapshotPerfilImportacionOperativa ObtenerPublicacion(ContextoImportacionCarpetas context,string version){Requested=version;var copy=PerfilImportacionJson.Leer<SnapshotPerfilImportacionOperativa>(PerfilImportacionJson.Escribir(Value));if(Substitute)copy.VersionId="6";return copy;}
        public List<SnapshotPerfilImportacionOperativa> ListarPublicaciones(ContextoImportacionCarpetas context,int page,int size){return new List<SnapshotPerfilImportacionOperativa>{Value};}
    }
    sealed class OperativeCatalogPort : IPerfilImportacionCatalogosOperativos {
        public Func<List<ValorReglaPerfilImportacion>,List<IncidenciaPerfilImportacion>> ValidateValues;
        public bool Level=true,Incompatible,Unavailable,Mutate;
        public List<IncidenciaPerfilImportacion> ValidarOperativo(ContextoImportacionCarpetas c,ConfiguracionPerfilImportacion config,List<ReglaPerfilImportacion> rules){
            if(Unavailable)throw new PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE","Fixture unavailable.");
            if(Mutate)config.NombreVisible="mutated";
            return Incompatible?new List<IncidenciaPerfilImportacion>{new IncidenciaPerfilImportacion("fixture","INCOMPATIBLE","Fixture")}:new List<IncidenciaPerfilImportacion>();
        }
        public bool NivelCompatibleOperativo(ContextoImportacionCarpetas c,ConfiguracionPerfilImportacion config,string level){return Level&&level==c.NivelRef;}
        public List<IncidenciaPerfilImportacion> ValidarValoresOperativos(ContextoImportacionCarpetas c,ConfiguracionPerfilImportacion config,List<ValorReglaPerfilImportacion> values){return ValidateValues==null?new List<IncidenciaPerfilImportacion>():ValidateValues(values);}
        public Dictionary<string,string> ClasificacionOperativa(ContextoImportacionCarpetas c,ConfiguracionPerfilImportacion config){return new Dictionary<string,string>();}
    }
    static void ProfileSelectionCases() {
        var read=new PublicationPort();var catalog=new OperativeCatalogPort();var adapter=new AdaptadorPerfilImportacionOperativa(read,catalog);var context=PlanFixture().Contexto;
        Check(adapter.Seleccionar(context,"5").VersionId=="5" && read.Requested=="5","DOC111 selection not bound to explicit version");
        read.Substitute=true;Rejected("ACCESO_DENEGADO",()=>adapter.Seleccionar(context,"5"));read.Substitute=false;
        catalog.Level=false;Rejected("ACCESO_DENEGADO",()=>adapter.Seleccionar(context,"5"));catalog.Level=true;
        catalog.Incompatible=true;Rejected("CONFIGURACION_INVALIDA",()=>adapter.Seleccionar(context,"5"));Check(adapter.Listar(context,1,10).Count==0,"DOC111 incompatible publication advertised");catalog.Incompatible=false;
        catalog.Unavailable=true;Unavailable(()=>adapter.Listar(context,1,10));catalog.Unavailable=false;
        catalog.Mutate=true;Unavailable(()=>adapter.Seleccionar(context,"5"));catalog.Mutate=false;
        Check(read.Value.Configuracion.NombreVisible==null,"DOC111 selection mutates repository snapshot");
        read.Value.Reglas.Add(new ReglaPerfilImportacion{FuenteTipo="PROVEEDOR"});read.Value.Huella=Hex(PerfilImportacionJson.Huella(new VersionPerfilImportacion{Configuracion=read.Value.Configuracion,Reglas=read.Value.Reglas}));Unavailable(()=>adapter.Seleccionar(context,"5"));
        Console.WriteLine("DOC-111 operative profile selection: passed");
    }

    static void OpeningCases() {
        var fixture=new ResourceFixture();var read=new PublicationPort();var catalog=new OperativeCatalogPort();
        var profiles=new AdaptadorPerfilImportacionOperativa(read,catalog,null);var schemaReads=0;
        var service=new ServicioPreparacionImportacion(level=>fixture.Adapter.ResolverDestino(fixture.Http,level,"ABRIR"),profiles,RequestLimits(),()=>schemaReads++);
        var result=service.Abrir(new AbrirImportacionSolicitud{nivelRef="7"});
        Check(result.nivelRef=="7" && result.rutaCanonica=="Compras / Órdenes" && result.selloDestino.Length==64,"DOC111 opening loses server destination");
        Check(result.perfiles.Count==1 && result.perfiles[0].VersionId=="5" && result.limites.maximoBytesArchivo=="1024" && schemaReads==1,"DOC111 opening loses publications or configured limits");
        Check(result.paginaPerfiles.pagina==1 && result.paginaPerfiles.tamanoPagina==50 && !result.paginaPerfiles.puedeHaberMas,"DOC111 opening does not declare catalog page");
        fixture.LevelAllowed=false;Rejected("ACCESO_DENEGADO",()=>service.Abrir(new AbrirImportacionSolicitud{nivelRef="7"}));Check(schemaReads==1,"DOC111 unauthorized opening reaches operational storage");fixture.LevelAllowed=true;
        fixture.ImportAllowed=false;Rejected("ACCESO_DENEGADO",()=>service.Abrir(new AbrirImportacionSolicitud{nivelRef="7"}));fixture.ImportAllowed=true;
        fixture.RootName="Nuevo origen";Check(service.Abrir(new AbrirImportacionSolicitud{nivelRef="7"}).rutaCanonica=="Nuevo origen / Órdenes","DOC111 opening caches destination from earlier request");
        catalog.Incompatible=true;Check(service.Abrir(new AbrirImportacionSolicitud{nivelRef="7"}).perfiles.Count==0,"DOC111 opening advertises incompatible profile");
        var page=service.ListarPerfiles(new ListarPerfilesImportacionSolicitud{nivelRef="7",pagina=2,tamanoPagina=1});
        Check(page.perfiles.Count==0 && page.puedeHaberMas && page.pagina==2,"DOC111 filtered full page hides subsequent pages");catalog.Incompatible=false;
        Rejected("SOLICITUD_INVALIDA",()=>service.ListarPerfiles(new ListarPerfilesImportacionSolicitud{nivelRef="7",pagina=0}));
        var unavailable=new ServicioPreparacionImportacion(level=>fixture.Adapter.ResolverDestino(fixture.Http,level,"ABRIR"),profiles,RequestLimits(),()=>{throw new PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE","Fixture");});
        Unavailable(()=>unavailable.Abrir(new AbrirImportacionSolicitud{nivelRef="7"}));
        var swapped=new ServicioPreparacionImportacion(level=>PlanFixture().Contexto,profiles,RequestLimits(),()=>{});
        Rejected("ACCESO_DENEGADO",()=>swapped.Abrir(new AbrirImportacionSolicitud{nivelRef="7"}));
        Console.WriteLine("DOC-111 read-only opening: passed");
    }

    static void PreflightCalculationCases() {
        var fixture=new ResourceFixture();var read=new PublicationPort();var catalog=new OperativeCatalogPort();
        read.Value.Configuracion=new ConfiguracionPerfilImportacion{NombreVisible="Fixture",GabineteId=7,HuellaOrigenGabinete=new string('a',64),FondoRef="1",InstrumentoRef="2",AreaRef="3",SerieRef="4",TipoExpedienteRef="5",UnidadDocumentalRef="6",ModoIntegridad="COMPLETO_OBLIGATORIO",AmbitoIntegridad="EXPEDIENTE",EstrategiaCarpetas="RAIZ_EXPEDIENTE",PoliticaSubcarpetas="INCLUIR",PoliticaCarpetasVacias="RECHAZAR",PoliticaArchivosSueltos="RECHAZAR",Extensiones=new List<string>{"pdf"}};
        read.Value.Reglas=new List<ReglaPerfilImportacion>{
            new ReglaPerfilImportacion{DestinoTipo="CAMPO_EXPEDIENTE",DestinoRef="CODIGO",FuenteTipo="CARPETA",Extraccion="SEGMENTO",Separador="-",Posicion=0,TipoValor="TEXTO",LongitudMaxima=160},
            new ReglaPerfilImportacion{DestinoTipo="CAMPO_EXPEDIENTE",DestinoRef="NOMBRE",FuenteTipo="CARPETA"},
            new ReglaPerfilImportacion{DestinoTipo="CAMPO_EXPEDIENTE",DestinoRef="FECHA_INICIAL",FuenteTipo="CARPETA",Extraccion="SEGMENTO",Separador="-",Posicion=1,TipoValor="FECHA",FormatoFecha="yyyyMMdd"},
            new ReglaPerfilImportacion{DestinoTipo="TIPOLOGIA",DestinoRef="TIPO_DOCUMENTAL",FuenteTipo="ARCHIVO",Extraccion="SEGMENTO",Separador="-",Posicion=1,QuitarExtension=true},
            new ReglaPerfilImportacion{DestinoTipo="INDICE_GABINETE",DestinoRef="99",FuenteTipo="EXPEDIENTE",PropiedadExpediente="ID",EtapaResolucion="TRAS_EXPEDIENTE"}
        };
        Action seal=()=>read.Value.Huella=Hex(PerfilImportacionJson.Huella(new VersionPerfilImportacion{Configuracion=read.Value.Configuracion,Reglas=read.Value.Reglas}));seal();
        var engine=new MySqlProveedorDatosIndices(alias=>{throw new Exception("DOC111 metadata-only preflight tried business IO");});
        var coordinator=new CoordinadorProveedorIndices(new OperativeProviderPort(),engine);
        var service=new ServicioPreparacionImportacion(level=>fixture.Adapter.ResolverDestino(fixture.Http,level,"PREPARAR"),new AdaptadorPerfilImportacionOperativa(read,catalog,null),RequestLimits(16384),()=>{},catalog,coordinator);
        var request=new PrepararPlanImportacionSolicitud{nivelRef="7",versionId="5",raices=new List<RaizManifiestoImportacionSolicitud>{new RaizManifiestoImportacionSolicitud{seleccionId="s1",idCliente="r1",nombre="000123-20240229"},new RaizManifiestoImportacionSolicitud{seleccionId="s2",idCliente="r2",nombre="000123-20240229"}},archivos=new List<ArchivoManifiestoImportacionSolicitud>{new ArchivoManifiestoImportacionSolicitud{idCliente="a1",raizIdCliente="r1",nombre="000123-125.pdf",rutaRelativa="000123-20240229/000123-125.pdf",bytes="7",ultimaModificacionLocal="0"},new ArchivoManifiestoImportacionSolicitud{idCliente="a2",raizIdCliente="r2",nombre="000123-125.pdf",rutaRelativa="000123-20240229/000123-125.pdf",bytes="7",ultimaModificacionLocal="0"}}};
        var first=service.CalcularPlan(request);
        Check(first.Resultado.Elementos.Count==4 && first.Manifiesto.Raices.Count==2 && first.PlanId==null,"DOC111 preflight collapses homonyms or claims persistence");
        foreach(var root in first.Resultado.Elementos.Where(e=>e.Ambito=="RAIZ")) {
            Check(root.Valores.Single(v=>v.DestinoRef=="CODIGO").Valor=="000123","DOC111 preflight loses code zeroes");
            Check(root.Valores.Single(v=>v.DestinoRef=="FECHA_INICIAL").Valor=="2024-02-29","DOC111 preflight ignores declared date format");
        }
        Check(first.Resultado.Elementos.Where(e=>e.Ambito=="ARCHIVO").All(e=>e.Valores.Single(v=>v.DestinoRef=="99").Diferido && e.Valores.Single(v=>v.DestinoRef=="99").Valor==null && e.Valores.Single(v=>v.DestinoRef=="TIPO_DOCUMENTAL").Valor=="125"),"DOC111 preflight manufactures future ID or loses file rule");
        Check(!first.Resultado.PuedeConfirmar && first.Resultado.Incidencias.Single().Campo=="coincidencias","DOC111 uncharacterized identity advertised as confirmable");
        Check(PerfilImportacionJson.Escribir(first)==PerfilImportacionJson.Escribir(service.CalcularPlan(request)),"DOC111 metadata preflight not deterministic");
        var visibleNameRule=read.Value.Reglas.Single(r=>r.DestinoRef=="NOMBRE");visibleNameRule.FuenteTipo="CONSTANTE";visibleNameRule.ValorConstante=new string('N',121);seal();var longName=service.CalcularPlan(request);Check(!longName.Resultado.PuedeConfirmar&&longName.Resultado.Incidencias.Any(i=>i.Campo=="CAMPO_EXPEDIENTE:NOMBRE"&&i.Codigo=="LONGITUD_NOMBRE_EXCEDIDA"),"DOC111 preflight fails to report legacy visible name capacity");visibleNameRule.FuenteTipo="CARPETA";visibleNameRule.ValorConstante=null;seal();
        foreach(var rule in read.Value.Reglas.Where(r=>r.FuenteTipo=="CARPETA" && r.Extraccion=="SEGMENTO"))rule.Separador="#";
        read.Value.Reglas.Single(r=>r.DestinoRef=="FECHA_INICIAL").FormatoFecha="dd-MM-yyyy";seal();
        foreach(var root in request.raices)root.nombre="000456#29-02-2024";
        foreach(var file in request.archivos)file.rutaRelativa="000456#29-02-2024/"+file.nombre;
        var second=service.CalcularPlan(request);
        Check(second.Resultado.Elementos.Where(e=>e.Ambito=="RAIZ").All(e=>e.Valores.Single(v=>v.DestinoRef=="CODIGO").Valor=="000456" && e.Valores.Single(v=>v.DestinoRef=="FECHA_INICIAL").Valor=="2024-02-29"),"DOC111 second configured naming format not resolved");
        request.archivos[1].nombre="invalid.pdf";request.archivos[1].rutaRelativa="000456#29-02-2024/invalid.pdf";
        var invalid=service.CalcularPlan(request);Check(invalid.Resultado.Incidencias.Any(i=>i.ArchivoIdCliente=="a2" && i.Codigo=="SEGMENTO_AUSENTE") && invalid.Manifiesto.Archivos.Count==2,"DOC111 individual error discarded original file");
        // Elimina solo en el doble local la dependencia de coincidencias para
        // comprobar el calculo de barreras; no habilita la capacidad productiva.
        var eligibility=typeof(ServicioPreparacionImportacion).GetMethod("EvaluarElegibilidad",BindingFlags.NonPublic|BindingFlags.Static);
        Action evaluate=()=>eligibility.Invoke(null,new object[]{invalid.Manifiesto,read.Value.Configuracion,invalid.Resultado});
        invalid.Resultado.Incidencias.RemoveAll(i=>i.Ambito=="GLOBAL");evaluate();
        Check(invalid.Resultado.PuedeConfirmar && invalid.Resultado.UnidadesElegibles.SequenceEqual(new[]{"r1"}),"DOC111 complete expediente does not preserve eligible root");
        read.Value.Configuracion.AmbitoIntegridad="LOTE";evaluate();Check(!invalid.Resultado.PuedeConfirmar && invalid.Resultado.UnidadesElegibles.Count==0,"DOC111 complete lote permits individual error");
        read.Value.Configuracion.ModoIntegridad="PARCIAL_PERMITIDO";read.Value.Configuracion.AmbitoIntegridad="NO_APLICA";evaluate();Check(invalid.Resultado.PuedeConfirmar && invalid.Resultado.UnidadesElegibles.SequenceEqual(new[]{"r1"}),"DOC111 partial blocks valid root");
        invalid.Manifiesto.Archivos.Add(new ArchivoManifiestoImportacion{IdCliente="a3",RaizIdCliente="r2"});evaluate();Check(invalid.Resultado.UnidadesElegibles.Count==2,"DOC111 partial cannot select valid file in affected root");
        invalid.Resultado.Incidencias.Add(new IncidenciaImportacionCarpetas{Ambito="GLOBAL",BloqueaUnidad=true});evaluate();Check(!invalid.Resultado.PuedeConfirmar && invalid.Resultado.UnidadesElegibles.Count==0,"DOC111 global error does not block partial");
        read.Value.Configuracion.ModoIntegridad="COMPLETO_OBLIGATORIO";read.Value.Configuracion.AmbitoIntegridad="EXPEDIENTE";seal();
        request.raices[1].nombre="000457#29-02-2024";request.archivos[1].rutaRelativa="000457#29-02-2024/"+request.archivos[1].nombre;
        var coincidences=new CoincidenceQueryFixture();var storedPlans=new List<PlanImportacionCarpetas>();
        Func<ContextoImportacionCarpetas,SnapshotPerfilImportacionOperativa,ManifiestoImportacionCarpetas,ResultadoPreparacionImportacion,List<IncidenciaImportacionCarpetas>> check=(c,s,m,r)=>(List<IncidenciaImportacionCarpetas>)RepositoryHelper("LeerCoincidencias",c,s,m,r,(Func<string,object[],DataTable>)coincidences.Query);
        Func<ContextoImportacionCarpetas,PlanImportacionCarpetas,PlanImportacionCarpetas> save=(c,p)=>{var prepared=Prepare(p);prepared.PlanId=(12+storedPlans.Count).ToString();storedPlans.Add(prepared);return ReadStored(StoredPlan(prepared));};
        var clock=new DateTime(2026,10,28,12,30,45,987,DateTimeKind.Utc);
        var configured=new ServicioPreparacionImportacion(level=>fixture.Adapter.ResolverDestino(fixture.Http,level,"PREPARAR"),new AdaptadorPerfilImportacionOperativa(read,catalog,null),RequestLimits(16384),()=>{},catalog,coordinator,check,save,()=>clock);
        var preparedResult=configured.Preparar(request);
        Check(preparedResult.planId=="12" && preparedResult.revision==1 && preparedResult.planHash.Length==64 && preparedResult.resultado.PuedeConfirmar && preparedResult.resultado.UnidadesElegibles.SequenceEqual(new[]{"r1"}),"DOC111 service does not persist computed eligibility");
        Check(storedPlans[0].Manifiesto.Archivos.Count==2 && storedPlans[0].CreadoUtc=="2026-10-28T12:30:45Z" && preparedResult.expiraUtc=="2026-10-28T12:31:45Z" && storedPlans[0].Resultado.Incidencias.Any(i=>i.ArchivoIdCliente=="a2"),"DOC111 persisted plan loses original totals, issues or server TTL");
        Check(preparedResult.planHash==Hex(PerfilImportacionJson.HuellaPlan(storedPlans[0])),"DOC111 service returns fingerprint for different contents");
        read.Value.Configuracion.AmbitoIntegridad="LOTE";seal();var whole=configured.Preparar(request);Check(!whole.resultado.PuedeConfirmar && whole.resultado.UnidadesElegibles.Count==0,"DOC111 integrated lote preparation ignores invalid file");
        read.Value.Configuracion.ModoIntegridad="PARCIAL_PERMITIDO";read.Value.Configuracion.AmbitoIntegridad="NO_APLICA";seal();
        request.archivos.Add(new ArchivoManifiestoImportacionSolicitud{idCliente="a3",raizIdCliente="r2",nombre="000457-125.pdf",rutaRelativa="000457#29-02-2024/000457-125.pdf",bytes="7",ultimaModificacionLocal="0"});
        var partial=configured.Preparar(request);Check(partial.resultado.PuedeConfirmar && partial.resultado.UnidadesElegibles.Count==2 && storedPlans.Last().Manifiesto.Archivos.Count==3 && partial.resultado.Incidencias.Any(i=>i.ArchivoIdCliente=="a2"),"DOC111 integrated partial preparation drops rejected inventory or blocks valid file");
        request.archivos.RemoveAt(2);read.Value.Configuracion.ModoIntegridad="COMPLETO_OBLIGATORIO";read.Value.Configuracion.AmbitoIntegridad="EXPEDIENTE";seal();
        coincidences.MissingMetadata=true;var blocked=configured.Preparar(request);Check(!blocked.resultado.PuedeConfirmar && blocked.resultado.Incidencias.Any(i=>i.Ambito=="GLOBAL"),"DOC111 missing comparison metadata advertised as confirmable");coincidences.MissingMetadata=false;
        var previousHttp=HttpContext.Current;
        try {
            var constructor=typeof(WebServiceImportacionCarpetas).GetConstructors(BindingFlags.NonPublic|BindingFlags.Instance).Single();
            Func<HttpContext,ImportacionCarpetasContextAdapter,LimitesImportacionCarpetas,ServicioPreparacionImportacion> factory=(h,a,l)=>configured;
            var endpoint=(WebServiceImportacionCarpetas)constructor.Invoke(new object[]{fixture.Adapter,(Func<LimitesImportacionCarpetas>)(()=>RequestLimits(16384)),factory,null,null,null});
            var worker=new ImportRequest{Body=System.Text.Encoding.UTF8.GetBytes(PerfilImportacionJson.Escribir(new SobreSolicitudPerfil<PrepararPlanImportacionSolicitud>{solicitud=request}))};
            var http=SessionFixture(worker:worker);worker.Token=ImportacionCarpetasRequestGuard.Emitir(http,RequestContext());HttpContext.Current=http;
            var response=endpoint.PrepararPlan(new PrepararPlanImportacionSolicitud{nivelRef="999",versionId="999"});
            Check(response.ok && ((PreparacionImportacionResultado)response.datos).nivelRef=="7" && http.Response.StatusCode==200,"DOC111 preparation trusts ASMX input instead of closed body");
            var before=storedPlans.Count;
            var bad=new ImportRequest{Body=System.Text.Encoding.UTF8.GetBytes("{\"solicitud\":{\"nivelRef\":\"7\",\"versionId\":\"5\",\"resultado\":{}}}")};var deniedHttp=SessionFixture(worker:bad);bad.Token=ImportacionCarpetasRequestGuard.Emitir(deniedHttp,RequestContext());HttpContext.Current=deniedHttp;
            response=endpoint.PrepararPlan(request);Check(!response.ok && response.datos==null && storedPlans.Count==before,"DOC111 client calculated result reaches persistence");
        } finally {HttpContext.Current=previousHttp;}
        var savedBefore=storedPlans.Count;
        fixture.ImportAllowed=false;Rejected("ACCESO_DENEGADO",()=>configured.Preparar(request));Check(storedPlans.Count==savedBefore,"DOC111 permission revoked but plan persisted");fixture.ImportAllowed=true;
        fixture.ImportAllowed=false;Rejected("ACCESO_DENEGADO",()=>service.CalcularPlan(request));fixture.ImportAllowed=true;
        request.archivos[0].rutaRelativa="../escape.pdf";Rejected("SOLICITUD_INVALIDA",()=>service.CalcularPlan(request));
        Console.WriteLine("DOC-111 metadata preflight and typed deferred sources: passed");
        Console.WriteLine("DOC-111 service plan persistence and ASMX preparation: passed");
    }

    static void OperationalDispatcherCases() {
        var fixture=new ResourceFixture();var read=new PublicationPort();var catalog=new OperativeCatalogPort();var calls=0;
        Func<HttpContext,ImportacionCarpetasContextAdapter,LimitesImportacionCarpetas,ServicioPreparacionImportacion> factory=(http,authority,limits)=>{
            calls++;return new ServicioPreparacionImportacion(level=>authority.ResolverDestino(http,level,"ABRIR"),new AdaptadorPerfilImportacionOperativa(read,catalog,null),limits,()=>{});
        };
        var constructor=typeof(WebServiceImportacionCarpetas).GetConstructors(BindingFlags.NonPublic|BindingFlags.Instance).Single();
        var endpoint=(WebServiceImportacionCarpetas)constructor.Invoke(new object[]{fixture.Adapter,(Func<LimitesImportacionCarpetas>)(()=>RequestLimits()),factory,null,null,null});
        var previous=HttpContext.Current;
        Func<string,Action<ImportRequest,HttpContext>,RespuestaPerfilImportacion> request=(body,change)=>{
            var worker=new ImportRequest{Body=System.Text.Encoding.UTF8.GetBytes(body)};var http=SessionFixture(worker:worker);
            worker.Token=ImportacionCarpetasRequestGuard.Emitir(http,RequestContext());if(change!=null)change(worker,http);
            HttpContext.Current=http;
            var output=endpoint.AbrirImportacion(new AbrirImportacionSolicitud{nivelRef="999"});
            Check(http.Response.StatusCode==(output.ok?200:output.codigo=="ACCESO_DENEGADO"||output.codigo=="SOLICITUD_NO_VERIFICADA"?403:output.codigo=="ERROR_INTERNO"?500:400),"DOC111 incorrect dispatcher HTTP status");
            return output;
        };
        const string valid="{\"solicitud\":{\"nivelRef\":\"7\"}}";
        try {
            var ok=request(valid,null);Check(ok.ok && ((AperturaImportacionResultado)ok.datos).nivelRef=="7" && calls==1,"DOC111 dispatcher trusts ASMX coerced input or requires admin");
            fixture.ImportAllowed=false;var denied=request(valid,null);Check(!denied.ok && denied.codigo=="ACCESO_DENEGADO" && denied.datos==null && calls==1,"DOC111 dispatcher caches import authority");fixture.ImportAllowed=true;
            Check(request(valid,(w,h)=>w.Token=null).codigo=="SOLICITUD_NO_VERIFICADA" && calls==1,"DOC111 invalid token reaches service factory");
            Check(!request("{\"solicitud\":{\"nivelRef\":\"7\",\"usuarioRef\":\"99\"}}",null).ok && calls==1,"DOC111 extra authority field accepted");
            Check(!request("{\"solicitud\":{\"nivelRef\":\"7\",\"nivelRef\":\"8\"}}",null).ok && calls==1,"DOC111 duplicate destination accepted");
            Check(request(valid,(w,h)=>w.Origin="https://other.invalid").codigo=="SOLICITUD_NO_VERIFICADA" && calls==1,"DOC111 foreign origin reaches factory");
            Check(request("{\"solicitud\":{}}",null).codigo=="SOLICITUD_INVALIDA","DOC111 missing destination accepted");
            // La ruta administrativa sigue resolviendo su propio contexto y no
            // acepta la facultad operativa ni el token por otro proposito.
            var admin=endpoint.ListarPerfiles(new ListarPerfilesSolicitud());Check(!admin.ok,"DOC111 operational dispatcher opens administrative endpoint");
        } finally {HttpContext.Current=previous;}
        Console.WriteLine("DOC-111 operational ASMX dispatcher: passed");
    }

    static void ConfiguredLimitsCases() {
        var session=TimeSpan.FromMinutes(20);
        var transport=LimitesImportacionCarpetas.DesdeTransporte(4096UL*1024UL,session);
        Check(transport.MaximoBytesArchivo==4096UL*1024UL&&transport.DuracionPlan==session,"DOC111 does not preserve existing transport/session capacity");
        Check(transport.MaximoBytesManifiesto==PerfilImportacionJson.MaximoCaracteres*3&&transport.MaximoBytesLote==UInt64.MaxValue,"DOC111 adds an unapproved batch quota");
        Check(transport.RetencionStaging==TimeSpan.MaxValue,"DOC111 silently expires originals");
        Check(LimitesImportacionCarpetas.DesdeTransporte(1024,session).MaximoBytesManifiesto==1024,"DOC111 JSON capacity exceeds transport");
        Unavailable(()=>LimitesImportacionCarpetas.DesdeTransporte(0,session));Unavailable(()=>LimitesImportacionCarpetas.DesdeTransporte(1024,TimeSpan.Zero));
        Rejected("SESION_REQUERIDA",()=>LimitesImportacionCarpetas.DesdeContexto(null));
        var previous=HttpContext.Current;
        try{
            var http=SessionFixture(sessionId:"native-policy");HttpContext.Current=http;
            var actual=LimitesImportacionCarpetas.DesdeContexto(http);
            Check(actual.MaximoBytesArchivo>0&&actual.DuracionPlan==TimeSpan.FromMinutes(http.Session.Timeout),"DOC111 requires new appSettings for an existing session");
        }finally{HttpContext.Current=previous;}
        Console.WriteLine("DOC-111 existing transport/session capacity without appSettings: passed");
    }

    static void OperativeProviderCases() {
        var context=PlanFixture().Contexto;
        var version=new ProveedorIndicesVersion{ProveedorId="7",VersionId="8",AmbitoRef="2",Estado="PUBLICADA",Activo=true,ConexionRef="DA",Tabla="imp_02_fixture",ColumnaClave="clave",TipoClave="TEXTO",ColumnaEmpresa="",Propiedades=new List<PropiedadProveedorIndices>{new PropiedadProveedorIndices{Nombre="valor",Columna="dato",Tipo="TEXTO"}}};
        var policy=new PoliticaFuentesProveedor(PerfilImportacionJson.Escribir(new List<FuenteProveedorAutorizada>{new FuenteProveedorAutorizada{AmbitoRef="2",ConexionRef="DA",Tabla="imp_02_fixture",ExclusivaEmpresa=true,ColumnaEmpresaAutorizada="",Columnas=new List<string>{"clave","dato"}}}));
        var reads=0;bool allowed=true;
        Func<string,string,object[],DataTable> query=(connection,sql,args)=>{
            reads++;Check(connection=="DA_TEST","DOC111 operational provider changes alias");
            var data=new DataTable();
            if(sql.Contains("information_schema.COLUMNS")){
                foreach(var col in new[]{"COLUMN_NAME","DATA_TYPE","COLUMN_TYPE","CHARACTER_MAXIMUM_LENGTH","NUMERIC_PRECISION","NUMERIC_SCALE","CHARACTER_SET_NAME"})data.Columns.Add(col,typeof(object));
                foreach(var name in new[]{"clave","dato"})data.Rows.Add(name,"varchar","varchar(20)",20,DBNull.Value,DBNull.Value,"utf8");
            }else{Check(sql.Contains("@clave") && sql.EndsWith("LIMIT 2"),"DOC111 provider query not bounded/parameterized");data.Columns.Add("valor",typeof(string));data.Rows.Add("0007");}
            return data;
        };
        var engine=new MySqlProveedorDatosIndices(alias=>alias+"_TEST",query,null,(c,v)=>allowed&&policy.PermiteOperativa(c,v));
        Check(engine.ConsultarOperativo(context,version,"0001").Valores["valor"]=="0007","DOC111 operational provider loses leading zeros");
        var before=reads;allowed=false;Check(engine.ConsultarOperativo(context,version,"0001").Codigo=="ACCESO_DENEGADO" && reads==before,"DOC111 revoked source still reads business rows");allowed=true;
        version.Estado="BORRADOR";Check(engine.ConsultarOperativo(context,version,"0001").Codigo=="ACCESO_DENEGADO" && reads==before,"DOC111 provider draft available operationally");version.Estado="PUBLICADA";
        version.Activo=false;Check(!engine.AutorizarOperativo(context,version),"DOC111 inactive provider authorized");version.Activo=true;
        version.Tabla="outside_prefix";Check(!engine.AutorizarOperativo(context,version),"DOC111 literal source prefix or registered table bypassed");version.Tabla="imp_02_fixture";
        var missingPolicy=new MySqlProveedorDatosIndices(alias=>alias+"_TEST",query,(c,v)=>true);
        Check(missingPolicy.ConsultarOperativo(context,version,"0001").Codigo=="ACCESO_DENEGADO","DOC111 admin source grant substituted for operative policy");
        Check(engine.Consultar(new ContextoPerfilImportacion{UsuarioRef="1",AmbitoRef="2",PuedeAdministrar=false},version,"0001").Codigo=="ACCESO_DENEGADO","DOC111 operative grant enlarges administrative path");
        var denied=new ContextoImportacionCarpetas{UsuarioRef="1",AmbitoRef="2",PuedeImportar=false};
        Rejected("ACCESO_DENEGADO",()=>new MySqlProveedorIndicesRepository("invalid").ObtenerPublicacion(denied,"7","8"));
        Rejected("ACCESO_DENEGADO",()=>new MySqlFuenteImportacionRepository("invalid").CargarPoliticaOperativa(denied,null));
        var readPort=new OperativeProviderPort{Value=version};var coordinator=new CoordinadorProveedorIndices(readPort,engine);
        var providerRule=new ReglaPerfilImportacion{FuenteTipo="PROVEEDOR",DestinoTipo="INDICE_GABINETE",DestinoRef="99",TipoValor="TEXTO",Proveedor=new FuenteProveedorIndices{ProveedorId="7",VersionId="8",Propiedad="valor",Clave=new ClaveProveedorIndices{FuenteTipo="CONSTANTE",ValorConstante="0001",TipoValor="TEXTO"}}};
        Check(coordinator.ValidarOperativo(context,new List<ReglaPerfilImportacion>{providerRule}).Count==0 && readPort.Reads==1,"DOC111 provider validation uses administrative getter");
        var rules=new List<ReglaPerfilImportacion>{providerRule};var examples=new List<EjemploPerfilImportacion>{new EjemploPerfilImportacion()};
        var resolved=coordinator.ResolverOperativo(context,rules,examples).Single();
        Check(resolved.Incidencias.Count==0 && resolved.Valores.Single().Valor=="0007" && resolved.Valores.Single().Procedencia=="7:8:valor","DOC111 operative resolution loses value or provenance");
        providerRule.Proveedor.Clave=new ClaveProveedorIndices{FuenteTipo="EXPEDIENTE",PropiedadExpediente="ID",TipoValor="TEXTO"};providerRule.EtapaResolucion="TRAS_EXPEDIENTE";
        before=reads;resolved=coordinator.ResolverOperativo(context,rules,examples).Single();
        Check(reads==before && resolved.Valores.Single().Diferido && resolved.Valores.Single().PropiedadPendiente=="ID" && resolved.Valores.Single().Procedencia=="7:8:valor","DOC111 future provider key queried or loses dependency");
        before=reads;allowed=false;Check(coordinator.ValidarOperativo(context,new List<ReglaPerfilImportacion>{providerRule}).Count==1 && reads==before,"DOC111 revoked provider source accepted in profile validation");allowed=true;
        Console.WriteLine("DOC-111 operative provider authority: passed");
    }

    sealed class DocumentAdapterFixture : IDisposable {
        public CreationStagingFixture Stage=new CreationStagingFixture();
        public LoteImportacionCarpetas Lot;public RaizImportacionCarpetas Root;public ArchivoImportacionCarpetas FileRecord;public IntentoImportacionCarpetas Attempt;
        public DatosAlmacenamientoImportacion Data;public AdaptadorDocumentoImportacion Adapter;public ComandoAlmacenamientoCarpetas Command;
        public int Starts,Writers,Verifications,Checkpoints,Writes,Events,Authorizations;public string CopyPath;
        public bool StartAllowed=true,VerifyResult=true,ThrowWriter,FailCheckpoint,LoseCommitResponse,WrongName,CorruptCopy,RejectWriter,RevokeBeforeWriter,StopDuringWriter;
        public int FailSqlAt;
        public Action OnWriter;
        public DocumentAdapterFixture(){
            var source=new CreationCommandFixture();Lot=Stage.View.Lote;Lot.Estado="EN_CURSO";Lot.Snapshot=ReceiptClone(source.Lot.Snapshot);
            Root=ReceiptClone(source.Root);Root.NombreOriginal="Folder";Root.Estado="EXPEDIENTE_CONFIRMADO";Root.IntentoActivoId=null;Root.IdentidadExpediente=new IdentidadExpedienteImportacion{OrigenLogico="RA",ExpedienteId="60",RelacionNivelId="70",NivelRef=Lot.Contexto.NivelRef};
            FileRecord=ReceiptClone(Stage.FullFiles["30"]);FileRecord.NombreOriginal="Órdenes 000123.pdf";FileRecord.RutaRelativa="Folder/Órdenes 000123.pdf";
            var rule=new ReglaPerfilImportacion{DestinoTipo="INDICE_GABINETE",DestinoRef="91",FuenteTipo="CONSTANTE",ValorConstante="0007",TipoValor="TEXTO"};Lot.Snapshot.Reglas.Add(rule);Rehash();
            FileRecord.ValoresEfectivos=new List<ValorReglaPerfilImportacion>{new ValorReglaPerfilImportacion{DestinoTipo="TIPOLOGIA",DestinoRef="TIPO_DOCUMENTAL",FuenteTipo="CONSTANTE",TipoValor="REFERENCIA",Valor="9"},new ValorReglaPerfilImportacion{DestinoTipo="INDICE_GABINETE",DestinoRef="91",FuenteTipo="CONSTANTE",TipoValor="TEXTO",Valor="0007"}};
            FileRecord.ValoresPlan=ReceiptClone(FileRecord.ValoresEfectivos);FileRecord.ValoresEfectivosFijados=true;FileRecord.EstadoEfecto="EN_CURSO";FileRecord.IntentoActivoId="50";FileRecord.Revision++;
            Attempt=new IntentoImportacionCarpetas{IntentoId="50",LoteId=Lot.LoteId,RaizId=Root.RaizId,ArchivoId=FileRecord.ArchivoId,Fase="ALMACENAR",Estado="RESERVADO",EsNuevaReserva=true,TokenReclamacion=new string('a',32),SolicitudHash=new string('b',64)};
            Data=new DatosAlmacenamientoImportacion{GabineteNombre="gab_fixture",TipologiaNombre="Contrato",ClaseDocumentoId="3",ClaseDocumentoNombre="DOCUMENTO ELECTRONICO",TipoArchivoId="6",Campos=new List<CampoIndiceImportacionCarpetas>{new CampoIndiceImportacionCarpetas{Referencia="91",Nombre="CODIGO_DOC",Valor="0007"}}};
            var store=new AlmacenStagingImportacion(Stage.Receipt.Root,Stage.Receipt.Limits,Lot,FileRecord,10,200);
            Action<ComandoAlmacenamientoCarpetas> authorize=c=>{Authorizations++;if(RevokeBeforeWriter&&Authorizations==4)throw new PerfilImportacionException("ACCESO_DENEGADO","fixture");};
            Func<ComandoAlmacenamientoCarpetas,DatosAlmacenamientoImportacion,bool> start=(c,d)=>{Starts++;if(!StartAllowed)return false;RepositoryHelper("ValidarInicioDocumento",c,Lot,Root,FileRecord);Attempt.Estado="EN_CURSO";Attempt.Resultado=(ResultadoEfectoImportacion)RepositoryHelper("EvidenciaInicialDocumento",c);return true;};
            Action<ComandoAlmacenamientoCarpetas,DatosAlmacenamientoImportacion,string,SalidaAlmacenamientoLegacy> writer=(c,d,path,outcome)=>{
                Writers++;CopyPath=path;Check(Path.GetFileName(path)==FileRecord.NombreOriginal&&System.IO.File.Exists(path),"DOC111 document copy name lost");
                if(OnWriter!=null)OnWriter();
                Check(System.IO.File.Exists(Stage.PathFor("30"))&&path!=Stage.PathFor("30"),"DOC111 writer receives original staging");
                bool held=false;try{using(var exclusive=new FileStream(Stage.PathFor("30"),FileMode.Open,FileAccess.ReadWrite,FileShare.None)){} }catch(IOException){held=true;}Check(held,"DOC111 original not retained during documentary effect");
                Check(d.Campos.Single().Valor=="0007"&&c.Archivo.ValoresEfectivos.Single(v=>v.DestinoRef=="91").Valor=="0007","DOC111 fixed index lost zeros");
                outcome.DocumentoId=90;outcome.InventarioId=100;outcome.NombreArchivo=WrongName?"changed.pdf":FileRecord.NombreOriginal;outcome.GabineteNombre=d.GabineteNombre;outcome.Respuesta=RejectWriter?"NO":"YES";
                if(StopDuringWriter)Lot.DetencionSolicitada=true;
                System.IO.File.Delete(path);
                if(ThrowWriter)throw new Exception("DOC111_SECRET_SQL_PATH_MUST_NOT_LEAK");
            };
            Func<ComandoAlmacenamientoCarpetas,DatosAlmacenamientoImportacion,ResultadoEfectoImportacion,bool> verify=(c,d,r)=>{Verifications++;return VerifyResult;};
            Func<ComandoAlmacenamientoCarpetas,ResultadoEfectoImportacion,ResultadoEfectoImportacion> save=(c,r)=>{Checkpoints++;if(FailCheckpoint)throw new Exception("secret checkpoint");var result=Save(c,r);if(LoseCommitResponse)throw new Exception("lost response");return result;};
            var constructor=typeof(AdaptadorDocumentoImportacion).GetConstructors(BindingFlags.NonPublic|BindingFlags.Instance).Single(x=>x.GetParameters().Length==7);
            Adapter=(AdaptadorDocumentoImportacion)constructor.Invoke(new object[]{authorize,new Func<ComandoAlmacenamientoCarpetas,DatosAlmacenamientoImportacion>(c=>ReceiptClone(Data)),start,writer,verify,save,store});
        }
        public void Rehash(){Lot.Snapshot.Huella=Hex(PerfilImportacionJson.Huella(new VersionPerfilImportacion{Configuracion=Lot.Snapshot.Configuracion,Reglas=Lot.Snapshot.Reglas}));}
        public ComandoAlmacenamientoCarpetas Prepare(){return ComandoAlmacenamientoCarpetas.Preparar(Lot.Contexto,Root,FileRecord,Lot.Snapshot,Attempt);}
        public ResultadoEfectoImportacion Run(){Command=Prepare();return Adapter.Almacenar(Lot.Contexto,Root,FileRecord,Lot.Snapshot,Attempt);}
        public ResultadoEfectoImportacion Save(ComandoAlmacenamientoCarpetas c,ResultadoEfectoImportacion r){
            var result=(ResultadoEfectoImportacion)RepositoryHelper("PersistirResultadoDocumento",c,Lot,Root,FileRecord,Attempt,r,DateTime.UtcNow,new Func<string,object[],int>((sql,args)=>{Writes++;Check(!sql.Contains("ra_integracion_raiz SET")&&!sql.Contains("expediente_archivo"),"Document checkpoint writes created expedition");return FailSqlAt==Writes?0:1;}),new Action(()=>Events++));
            Attempt.Estado=result.Estado;Attempt.Resultado=ReceiptClone(result);Attempt.FinUtc=result.Estado=="CONFIRMADO"?"2026-10-30T12:00:00Z":null;
            FileRecord.EstadoEfecto=result.Estado=="CONFIRMADO"?"COMPLETADO":"INCIERTO";FileRecord.Resultado=ReceiptClone(result);FileRecord.IntentoActivoId=result.Estado=="CONFIRMADO"?null:Attempt.IntentoId;FileRecord.Revision++;Lot.Revision++;if(result.Estado=="INCIERTO")Lot.Estado="PENDIENTE_VERIFICACION";return result;
        }
        public void Dispose(){Stage.Dispose();}
    }

    sealed class DocumentPhaseFixture : IControlLoteImportacionRepository,IDisposable {
        public DocumentAdapterFixture Source=new DocumentAdapterFixture();
        public PlanImportacionCarpetas Plan;public Dictionary<string,ArchivoImportacionCarpetas> Files=new Dictionary<string,ArchivoImportacionCarpetas>();
        public CoordinadorImportacionCarpetas Coordinator;public uint OriginalRevision;
        public int Reservations,Factories,StageFactories,Stops,Authorizations;public bool Allowed=true;public Action AfterReserve;
        public DocumentPhaseFixture(){
            Source.Root.ClienteRaizId="r1";Source.FileRecord.EstadoEfecto="NO_INICIADO";Source.FileRecord.IntentoActivoId=null;Source.FileRecord.Revision--;Source.Attempt=null;OriginalRevision=Source.FileRecord.Revision;
            Files.Add("30",Source.FileRecord);Plan=ReceiptClone(Source.Stage.View.Plan);Plan.Snapshot=ReceiptClone(Source.Lot.Snapshot);Plan.Manifiesto.Archivos[0].Nombre=Source.FileRecord.NombreOriginal;Plan.Manifiesto.Archivos[0].RutaRelativa=Source.FileRecord.RutaRelativa;
            Func<ContextoImportacionCarpetas,string,string,VistaBarreraCreacionImportacion> view=(c,l,r)=>{Owner(c,l);Check(r=="40","DOC111 document phase changes root");return View();};
            Func<ContextoImportacionCarpetas,string,string,ArchivoImportacionCarpetas> file=(c,l,id)=>{Owner(c,l);return ReceiptClone(Files[id]);};
            Func<ContextoImportacionCarpetas,ReclamacionImportacionCarpetas,IntentoImportacionCarpetas> history=(c,r)=>{Owner(c,r.LoteId);return Source.Attempt!=null&&Source.Attempt.SolicitudHash==r.SolicitudHash?ReceiptClone(Source.Attempt):null;};
            Func<LoteImportacionCarpetas,ArchivoImportacionCarpetas,IAlmacenStagingImportacion> staging=(l,f)=>{StageFactories++;return new AlmacenStagingImportacion(Source.Stage.Receipt.Root,Source.Stage.Receipt.Limits,l,f,10,200);};
            Func<IAlmacenStagingImportacion,IAlmacenamientoDocumentoImportacion> storage=s=>{Factories++;return Source.Adapter;};
            Coordinator=new CoordinadorImportacionCarpetas(()=>ReceiptClone(Source.Lot.Contexto),this,n=>ReceiptClone(Source.Lot.Contexto),Authorize,view,file,history,staging,storage);
        }
        void Owner(ContextoImportacionCarpetas c,string id){Check(c!=null&&c.PuedeImportar&&id=="20"&&c.UsuarioRef==Source.Lot.Contexto.UsuarioRef&&c.AmbitoRef==Source.Lot.Contexto.AmbitoRef&&c.NivelRef==Source.Lot.Contexto.NivelRef,"DOC111 phase escaped owner or destination");}
        void Authorize(ContextoImportacionCarpetas c,ConfiguracionPerfilImportacion s){Authorizations++;if(!Allowed)throw new PerfilImportacionException("ACCESO_DENEGADO","fixture");Owner(c,"20");}
        public VistaBarreraCreacionImportacion View(){bool global=Source.Lot.Snapshot.Configuracion.ModoIntegridad=="COMPLETO_OBLIGATORIO"&&Source.Lot.Snapshot.Configuracion.AmbitoIntegridad=="LOTE";return ReceiptClone(new VistaBarreraCreacionImportacion{Lote=Source.Lot,Plan=Plan,Raiz=Source.Root,Raices=new List<RaizImportacionCarpetas>{Source.Root},Archivos=Files.Values.Where(f=>global||f.RaizId==Source.Root.RaizId).ToList()});}
        public ContextoImportacionCarpetas ObtenerContextoLote(ContextoImportacionCarpetas c,string id){Owner(c,id);return ReceiptClone(Source.Lot.Contexto);}
        public LoteImportacionCarpetas ObtenerLote(ContextoImportacionCarpetas c,string id){Owner(c,id);return ReceiptClone(Source.Lot);}
        public IntentoImportacionCarpetas ReservarIntento(ContextoImportacionCarpetas c,ReclamacionImportacionCarpetas request){
            Owner(c,request.LoteId);Check(request.Fase=="ALMACENAR"&&request.RecursoTipo=="ARCHIVO"&&request.RecursoId=="30","DOC111 phase reserves different resource");
            if(Source.Attempt!=null&&Source.Attempt.SolicitudHash==request.SolicitudHash)return ReceiptClone(Source.Attempt);
            RepositoryHelper("ValidarReservaDocumental",Source.Lot,Source.Root,Source.FileRecord,request,Source.Attempt!=null);
            Check(DocumentBarrier(View(),Source.FileRecord,null),"DOC111 phase reserves before metadata barrier");
            Reservations++;Source.Attempt=new IntentoImportacionCarpetas{IntentoId="50",LoteId="20",RaizId="40",ArchivoId="30",Fase="ALMACENAR",Estado="RESERVADO",EjecutorRef=request.EjecutorRef,TokenReclamacion=request.TokenReclamacion,SolicitudHash=request.SolicitudHash};Source.FileRecord.EstadoEfecto="EN_CURSO";Source.FileRecord.IntentoActivoId="50";Source.FileRecord.Revision++;Source.Lot.Estado="EN_CURSO";Source.Lot.Revision++;
            var copy=ReceiptClone(Source.Attempt);copy.EsNuevaReserva=true;if(AfterReserve!=null)AfterReserve();return copy;
        }
        public LoteImportacionCarpetas SolicitarDetencion(ContextoImportacionCarpetas c,string id,uint revision){Owner(c,id);RepositoryHelper("ValidarDetencion",Source.Lot,revision);if(!Source.Lot.DetencionSolicitada){Stops++;Source.Lot.DetencionSolicitada=true;Source.Lot.Revision++;}return ReceiptClone(Source.Lot);}
        public void AddFile(){Source.Stage.AddFile();var file=ReceiptClone(Source.Stage.FullFiles["31"]);Files.Add("31",file);Plan.Manifiesto.Archivos.Add(new ArchivoManifiestoImportacion{RaizIdCliente="r1",IdCliente=file.ClienteArchivoId,Nombre=file.NombreOriginal,RutaRelativa=file.RutaRelativa,Bytes=file.BytesPrevistos});}
        public void Partial(){Source.Lot.Snapshot.Configuracion.ModoIntegridad="PARCIAL_PERMITIDO";Source.Lot.Snapshot.Configuracion.AmbitoIntegridad="NO_APLICA";Source.Rehash();Plan.Snapshot=ReceiptClone(Source.Lot.Snapshot);}
        public ResultadoFaseDocumentoImportacion Run(){return Coordinator.AlmacenarArchivo("20","30",OriginalRevision,"stable-document");}
        public void Dispose(){Source.Dispose();}
    }
    static bool DocumentBarrier(VistaBarreraCreacionImportacion view,ArchivoImportacionCarpetas file,string attempt){try{return (bool)typeof(CoordinadorImportacionCarpetas).GetMethod("EvaluarBarreraAlmacenamiento",BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,new object[]{view,file,attempt});}catch(TargetInvocationException e){throw e.InnerException;}}
    static void DocumentPhaseCases(){
        using(var f=new DocumentPhaseFixture()){
            Unavailable(()=>f.Coordinator.CrearExpediente("20","40",1,"wrong-phase"));Check(f.Reservations==0,"DOC111 document composition permits another phase");
            var r=f.Run();Check(r.Resultado.Estado=="CONFIRMADO"&&r.Resultado.DocumentoId=="90"&&r.Resultado.InventarioId=="100"&&f.Reservations==1&&f.Source.Writers==1&&f.Source.Checkpoints==1&&r.ArchivosVerificados.SequenceEqual(new[]{"30"}),"DOC111 composed document phase did not confirm one effect");
            Check(f.Source.Lot.Estado=="EN_CURSO"&&f.Source.Root.Estado=="EXPEDIENTE_CONFIRMADO"&&f.Source.Lot.TotalArchivos==1,"DOC111 phase completes unit/lote or changes inventory");
            var stages=f.StageFactories;var replay=f.Run();Check(replay.Repeticion&&replay.Resultado.Estado=="CONFIRMADO"&&f.Source.Writers==1&&f.StageFactories==stages&&f.Reservations==1,"DOC111 historical document phase reopens staging or replays writer");
            Rejected("CONFLICTO_REVISION",()=>f.Coordinator.AlmacenarArchivo("20","30",f.OriginalRevision,"different-key"));Check(f.Source.Writers==1,"DOC111 different key replays completed file");
            f.Allowed=false;Rejected("ACCESO_DENEGADO",()=>f.Run());Check(f.StageFactories==stages,"DOC111 revoked replay opens disk");
        }
        using(var f=new DocumentPhaseFixture()){f.Source.LoseCommitResponse=true;var r=f.Run();Check(r.Resultado.Estado=="CONFIRMADO"&&f.Source.FileRecord.EstadoEfecto=="COMPLETADO"&&f.Source.Writers==1,"DOC111 lost checkpoint response not recovered by durable read");}
        foreach(var mode in new[]{"throw","checkpoint","verify"})using(var f=new DocumentPhaseFixture()){
            f.Source.ThrowWriter=mode=="throw";f.Source.FailCheckpoint=mode=="checkpoint";f.Source.VerifyResult=mode!="verify";var r=f.Run();Check(r.Resultado.Estado=="INCIERTO"&&r.Resultado.DocumentoId=="90"&&r.Resultado.InventarioId=="100"&&f.Source.Writers==1,"DOC111 document coordinator loses uncertain known IDs");var stages=f.StageFactories;r=f.Run();Check(r.Repeticion&&r.Resultado.Estado=="INCIERTO"&&f.Source.Writers==1&&f.StageFactories==stages,"DOC111 uncertain document phase automatically replays");
        }
        foreach(var mode in new[]{"stop","revoke","missing"})using(var f=new DocumentPhaseFixture()){
            f.AfterReserve=()=>{if(mode=="stop")f.Source.Lot.DetencionSolicitada=true;else if(mode=="revoke")f.Allowed=false;else {var path=f.Source.Stage.PathFor("30");System.IO.File.Move(path,path+".missing");}};
            var r=f.Run();Check(r.Resultado.Estado=="INCIERTO"&&f.Reservations==1&&f.Factories==0&&f.Source.Writers==0&&f.Source.Attempt.Estado=="RESERVADO","DOC111 post-reservation stop/revocation/missing staging reaches writer");
        }
        using(var f=new DocumentPhaseFixture()){f.Source.OnWriter=()=>f.Coordinator.SolicitarDetencion(new DetenerLoteImportacionSolicitud{loteId="20",revisionEsperada=f.Source.Lot.Revision});var r=f.Run();Check(r.Resultado.Estado=="CONFIRMADO"&&f.Source.Lot.DetencionSolicitada&&f.Stops==1,"DOC111 stopping in-flight document loses confirmed effect/signal");}
        foreach(var scope in new[]{"EXPEDIENTE","LOTE"})using(var f=new DocumentPhaseFixture()){
            f.Source.Lot.Snapshot.Configuracion.AmbitoIntegridad=scope;f.Source.Rehash();f.Plan.Snapshot=ReceiptClone(f.Source.Lot.Snapshot);f.AddFile();f.Files["31"].EstadoRecepcion="PENDIENTE";
            Rejected("CONFLICTO_REVISION",()=>f.Run());Check(f.Reservations==0&&f.Source.Writers==0,"DOC111 complete unit bypasses missing reception");
        }
        using(var f=new DocumentPhaseFixture()){
            f.AddFile();f.Source.OnWriter=()=>{bool held=false;try{using(var exclusive=new FileStream(f.Source.Stage.PathFor("31"),FileMode.Open,FileAccess.ReadWrite,FileShare.None)){} }catch(IOException){held=true;}Check(held,"DOC111 complete unit sibling staging not retained during effect");};
            var r=f.Run();Check(r.Resultado.Estado=="CONFIRMADO"&&r.ArchivosVerificados.Count==2&&f.Source.Writers==1&&f.Files["31"].EstadoEfecto=="NO_INICIADO","DOC111 phase processes sibling or does not verify full unit");using(var released=new FileStream(f.Source.Stage.PathFor("31"),FileMode.Open,FileAccess.ReadWrite,FileShare.None)){}
        }
        using(var f=new DocumentPhaseFixture()){
            f.AddFile();var path=f.Source.Stage.PathFor("31");System.IO.File.Move(path,path+".missing");Rejected("RESULTADO_INCIERTO",()=>f.Run());Check(f.Reservations==0,"DOC111 complete unit reserves with missing physical sibling");
            f.Partial();f.Files["31"].EstadoRecepcion="RECHAZADO";f.Files["31"].EstadoEfecto="RECHAZADO";f.Plan.Resultado.Incidencias.Add(new IncidenciaImportacionCarpetas{RaizIdCliente="r1",ArchivoIdCliente="f2",BloqueaUnidad=true});var r=f.Run();Check(r.Resultado.Estado=="CONFIRMADO"&&f.Source.Lot.TotalArchivos==2&&f.Files.Count==2&&f.Source.Writers==1,"DOC111 partial phase blocks independent file or shrinks rejected inventory");
        }
        Console.WriteLine("DOC-111 coordinated document phase barriers retained unit durable replay and stop (local PDF and persistence writer doubles): passed");
    }

    static bool StorageBarrierFixture(CreationBarrierFixture source){
        source.Plan.Snapshot.Huella=Hex(PerfilImportacionJson.Huella(new VersionPerfilImportacion{Configuracion=source.Plan.Snapshot.Configuracion,Reglas=source.Plan.Snapshot.Reglas}));source.Lot.Snapshot=ReceiptClone(source.Plan.Snapshot);source.Lot.Estado="EN_CURSO";
        var root=source.Roots[source.Target];root.Estado="EXPEDIENTE_CONFIRMADO";root.IdentidadExpediente=new IdentidadExpedienteImportacion{OrigenLogico="RA",ExpedienteId="60",RelacionNivelId="70",NivelRef=source.Lot.Contexto.NivelRef};
        var file=source.Files.First(x=>x.RaizId==root.RaizId);file.ValoresEfectivosFijados=true;
        var global=source.Plan.Snapshot.Configuracion.ModoIntegridad=="COMPLETO_OBLIGATORIO"&&source.Plan.Snapshot.Configuracion.AmbitoIntegridad=="LOTE";
        return DocumentBarrier(new VistaBarreraCreacionImportacion{Lote=source.Lot,Plan=source.Plan,Raiz=root,Raices=source.Roots.Where(x=>global||x.RaizId==root.RaizId).ToList(),Archivos=source.Files.Where(x=>global||x.RaizId==root.RaizId).ToList()},file,null);
    }
    static void DocumentGlobalBarrierCases(){
        var f=new CreationBarrierFixture();Check(StorageBarrierFixture(f),"DOC111 full global receipt barrier rejects confirmed root");
        f.Files[1].EstadoRecepcion="PENDIENTE";Check(!StorageBarrierFixture(f),"DOC111 storage bypasses unreceived file in other root");f.Policy("COMPLETO_OBLIGATORIO","EXPEDIENTE");Check(StorageBarrierFixture(f),"DOC111 storage unnecessarily blocks complete independent root");
        f=new CreationBarrierFixture();f.Files[1].EstadoEfecto="INCIERTO";Check(!StorageBarrierFixture(f),"DOC111 complete LOTE ignores uncertain sibling effect");
        f=new CreationBarrierFixture();f.Files[1].EstadoEfecto="COMPLETADO";Check(StorageBarrierFixture(f),"DOC111 storage receipt barrier rejects completed sibling proof");
        f=new CreationBarrierFixture();f.AnotherFile();f.Files[2].EstadoRecepcion="RECHAZADO";f.Files[2].EstadoEfecto="RECHAZADO";f.Plan.Resultado.Incidencias.Add(new IncidenciaImportacionCarpetas{RaizIdCliente="r1",ArchivoIdCliente="a2",BloqueaUnidad=true});Check(!StorageBarrierFixture(f),"DOC111 full scope ignores rejected original");f.Policy("PARCIAL_PERMITIDO","NO_APLICA");Check(StorageBarrierFixture(f)&&f.Lot.TotalArchivos==3&&f.Files.Count==3,"DOC111 partial storage shrinks original inventory or blocks independent file");
        f.Plan.Resultado.Incidencias.Add(new IncidenciaImportacionCarpetas{RaizIdCliente="r1",ArchivoIdCliente="a1",BloqueaUnidad=true});Check(!StorageBarrierFixture(f),"DOC111 partial storage selects ineligible target");
        Console.WriteLine("DOC-111 shared receipt barrier covers storage across independent roots without changing inventory (synthetic metadata): passed");
    }

    sealed class ConsolidationFixture : IDisposable {
        public DocumentPhaseFixture Phase=new DocumentPhaseFixture();public List<RaizImportacionCarpetas> Roots=new List<RaizImportacionCarpetas>();public List<IntentoImportacionCarpetas> Proofs=new List<IntentoImportacionCarpetas>();public int Writes,Events,FailAt;
        public ConsolidationFixture(){Roots.Add(Phase.Source.Root);CreationProof(Phase.Source.Root,"49");Elements();}
        public void CreationProof(RaizImportacionCarpetas root,string id){Proofs.Add(new IntentoImportacionCarpetas{IntentoId=id,LoteId="20",RaizId=root.RaizId,Fase="CREAR_EXPEDIENTE",Estado="CONFIRMADO",FinUtc="2026-10-30T12:00:00Z",TokenReclamacion=new string('a',32),SolicitudHash=new string('b',64),Resultado=new ResultadoEfectoImportacion{Estado="CONFIRMADO",Codigo="EXPEDIENTE_CONFIRMADO",Expediente=ReceiptClone(root.IdentidadExpediente),Creacion=new EvidenciaCreacionExpedienteImportacion{IntentoId=id,CicloArchivoId="13",CicloArchivoNombre="Archivo Gestión"}}});}
        public void Elements(){
            Phase.Plan.Resultado.Elementos.Clear();
            foreach(var root in Roots)Phase.Plan.Resultado.Elementos.Add(new ValoresElementoImportacion{Ambito="RAIZ",RaizIdCliente=root.ClienteRaizId,Valores=ReceiptClone(root.ValoresPlan)});
            foreach(var file in Phase.Files.Values){var root=Roots.Single(r=>r.RaizId==file.RaizId);Phase.Plan.Resultado.Elementos.Add(new ValoresElementoImportacion{Ambito="ARCHIVO",RaizIdCliente=root.ClienteRaizId,ArchivoIdCliente=file.ClienteArchivoId,Valores=ReceiptClone(file.ValoresPlan)});}
        }
        public VistaConsolidacionImportacion View(){var attempts=Proofs.ToList();if(Phase.Source.Attempt!=null)attempts.Add(Phase.Source.Attempt);return new VistaConsolidacionImportacion{Lote=Phase.Source.Lot,Plan=Phase.Plan,Raices=Roots,Archivos=Phase.Files.Values.ToList(),Intentos=attempts};}
        public ConsolidacionImportacion Evaluate(){return Consolidation(View());}
        public ConsolidacionImportacion Save(){
            var result=(ConsolidacionImportacion)RepositoryHelper("PersistirConsolidacion",View(),DateTime.UtcNow,new Func<string,object[],int>((sql,args)=>{
                Writes++;Check((sql.StartsWith("UPDATE ra_integracion_raiz SET estado=")||sql.StartsWith("UPDATE ra_integracion_lote SET estado="))&&sql.Contains("revision=@revision")&&sql.Contains("estado=@anterior")&&!sql.Contains("identidad_expediente_doc=")&&!sql.Contains("detencion_solicitada=")&&!sql.Contains("DELETE "),"DOC111 consolidation changes effects, totals, signal or lacks CAS");return Writes==FailAt?0:1;
            }),new Action<ConsolidacionImportacion>(r=>Events++));
            bool changed=result.Estado!=result.EstadoOriginal;
            foreach(var item in result.Raices.Where(r=>r.Estado!=r.EstadoOriginal)){var root=Roots.Single(r=>r.RaizId==item.RaizId);root.Estado=item.Estado;root.Revision++;changed=true;}
            if(changed){Phase.Source.Lot.Estado=result.Estado;Phase.Source.Lot.Revision++;}return result;
        }
        public void Finish(){Check(Phase.Run().Resultado.Estado=="CONFIRMADO","DOC111 consolidation fixture document did not finish");}
        public void AddFile(){Phase.AddFile();Phase.Files["31"].ValoresPlan=ReceiptClone(Phase.Source.FileRecord.ValoresPlan);Elements();}
        public void AddRoot(){
            AddFile();var root=ReceiptClone(Phase.Source.Root);root.RaizId="41";root.ClienteRaizId="r2";root.Estado="PENDIENTE";root.IdentidadExpediente=null;root.Revision=1;root.ValoresPlan.Single(v=>v.DestinoRef=="CODIGO").Valor="000124";Roots.Add(root);
            Phase.Plan.Manifiesto.Raices.Add(new RaizManifiestoImportacion{IdCliente="r2",SeleccionId="s2",Nombre=root.NombreOriginal});Phase.Plan.Resultado.UnidadesElegibles.Add("r2");Phase.Source.Lot.TotalRaices=2;
            var file=Phase.Files["31"];file.RaizId="41";file.ClienteArchivoId=Phase.Source.FileRecord.ClienteArchivoId;file.NombreOriginal=Phase.Source.FileRecord.NombreOriginal;file.RutaRelativa=Phase.Source.FileRecord.RutaRelativa;
            var original=Phase.Plan.Manifiesto.Archivos[1];original.RaizIdCliente="r2";original.IdCliente=file.ClienteArchivoId;original.Nombre=file.NombreOriginal;original.RutaRelativa=file.RutaRelativa;Elements();
        }
        public void FinishSecond(){
            var root=Roots.Single(r=>r.RaizId=="41");root.Estado="EXPEDIENTE_CONFIRMADO";root.IdentidadExpediente=new IdentidadExpedienteImportacion{OrigenLogico="RA",ExpedienteId="61",RelacionNivelId="71",NivelRef=Phase.Source.Lot.Contexto.NivelRef};CreationProof(root,"51");
            var file=Phase.Files["31"];file.EstadoEfecto="COMPLETADO";file.ValoresEfectivosFijados=true;file.ValoresEfectivos=ReceiptClone(file.ValoresPlan);
            file.Resultado=new ResultadoEfectoImportacion{Estado="CONFIRMADO",Codigo="DOCUMENTO_CONFIRMADO",Expediente=ReceiptClone(root.IdentidadExpediente),OrigenGabinete="DA",GabineteId="7",DocumentoId="91",InventarioId="101",ValoresEfectivos=ReceiptClone(file.ValoresEfectivos)};
            Proofs.Add(new IntentoImportacionCarpetas{IntentoId="52",LoteId="20",RaizId="41",ArchivoId="31",Fase="ALMACENAR",Estado="CONFIRMADO",FinUtc="2026-10-30T12:00:00Z",TokenReclamacion=new string('c',32),SolicitudHash=new string('d',64),Resultado=ReceiptClone(file.Resultado)});
        }
        public void Dispose(){Phase.Dispose();}
    }
    static ConsolidacionImportacion Consolidation(VistaConsolidacionImportacion view){try{return (ConsolidacionImportacion)typeof(CoordinadorImportacionCarpetas).GetMethod("EvaluarConsolidacion",BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,new object[]{view});}catch(TargetInvocationException e){throw e.InnerException;}}
    sealed class ReconciliationFixture : IDisposable {
        public ConsolidationFixture Source=new ConsolidationFixture();
        public int Reads,Verifies,Saves,Writes,Events,FailAt;public bool Verify=true,ThrowVerify,ThrowSave,LoseResponse,Allowed=true,RevokeAfterVerify;
        public CoordinadorImportacionCarpetas Coordinator;
        public ReconciliationFixture(){
            Source.Phase.Source.VerifyResult=false;Source.Phase.Run();Source.Save();
            var p=Source.Phase;var s=p.Source;
            Coordinator=new CoordinadorImportacionCarpetas(()=>ReceiptClone(s.Lot.Contexto),p,n=>ReceiptClone(s.Lot.Contexto),
                (c,config)=>{if(!Allowed)throw new PerfilImportacionException("ACCESO_DENEGADO","fixture");},
                (c,l,i)=>{Reads++;Check(l=="20"&&i=="50","Reconciliation changed IDs");return ReceiptClone(View());},
                view=>{Verifies++;if(ThrowVerify)throw new Exception("PRIVATE_RECON_QUERY");var result=Verifier().Verificar(view);if(RevokeAfterVerify)Allowed=false;return result;},
                (expected,result)=>{
                    Saves++;if(ThrowSave)throw new Exception("PRIVATE_RECON_SAVE");
                    var normal=Save(expected,View(),result);
                    s.Attempt.Estado="CONFIRMADO";s.Attempt.Resultado=ReceiptClone(normal);s.Attempt.FinUtc="2026-10-30T12:00:00Z";
                    s.FileRecord.EstadoEfecto="COMPLETADO";s.FileRecord.Resultado=ReceiptClone(normal);s.FileRecord.IntentoActivoId=null;s.FileRecord.Revision++;
                    if(s.Root.Estado=="INCIERTO"){s.Root.Estado="EXPEDIENTE_CONFIRMADO";s.Root.Revision++;}
                    s.Lot.Estado=s.Lot.DetencionSolicitada||s.Lot.Estado=="DETENIDO"?"DETENIDO":"LISTO";s.Lot.Revision++;Source.Save();
                    if(LoseResponse)throw new Exception("PRIVATE_RECON_LOST_RESPONSE");return normal;
                });
        }
        public VistaReconciliacionImportacion View(){var s=Source.Phase.Source;return new VistaReconciliacionImportacion{Lote=s.Lot,Plan=Source.Phase.Plan,Raiz=s.Root,Archivo=s.FileRecord,Intento=s.Attempt};}
        public VerificadorReconciliacionImportacion Verifier(){return ReconciliationVerifier(c=>{throw new Exception("Document reconciliation queried creation");},(c,i,d)=>{throw new Exception("Document reconciliation queried creation");},
            c=>{Check(c.EsVerificacion&&c.Intento.Estado=="INCIERTO"&&!c.Intento.EsNuevaReserva&&c.Archivo.EstadoEfecto=="INCIERTO","Reconciliation fabricated executable state");return Source.Phase.Source.Data;},
            (c,d,r)=>{Check(r.DocumentoId=="90"&&r.InventarioId=="100"&&r.Expediente.ExpedienteId=="60","Reconciliation changed original evidence");return Verify;});}
        public ResultadoEfectoImportacion Save(VistaReconciliacionImportacion expected,VistaReconciliacionImportacion actual,ResultadoEfectoImportacion result){
            return (ResultadoEfectoImportacion)RepositoryHelper("PersistirReconciliacion",expected,actual,result,DateTime.UtcNow,new Func<string,object[],int>((sql,args)=>{
                Writes++;Check(sql.StartsWith("UPDATE ra_integracion_")&&!sql.Contains("detencion_solicitada=")&&!sql.Contains("expediente_archivo")&&!sql.Contains("DELETE ")&&sql.Contains("WHERE "),"Reconciliation writes legacy effects or clears stop");return Writes==FailAt?0:1;
            }),new Action<ResultadoEfectoImportacion>(r=>Events++));
        }
        public ResultadoReconciliacionImportacion Run(){return Coordinator.ReconciliarIntento("20","50",Source.Phase.Source.Lot.Revision);}
        public void Dispose(){Source.Dispose();}
    }
    static VerificadorReconciliacionImportacion ReconciliationVerifier(Func<ComandoCreacionExpedienteImportacion,Dictionary<string,string>> creationData,Func<ComandoCreacionExpedienteImportacion,IdentidadExpedienteImportacion,Dictionary<string,string>,bool> creationRead,Func<ComandoAlmacenamientoCarpetas,DatosAlmacenamientoImportacion> documentData,Func<ComandoAlmacenamientoCarpetas,DatosAlmacenamientoImportacion,ResultadoEfectoImportacion,bool> documentRead){
        return (VerificadorReconciliacionImportacion)typeof(VerificadorReconciliacionImportacion).GetConstructors(BindingFlags.Instance|BindingFlags.NonPublic).Single().Invoke(new object[]{creationData,creationRead,documentData,documentRead});
    }
    static void CloseUnstartedReservationCases(){
        Func<DocumentPhaseFixture,VistaReconciliacionImportacion> view=f=>new VistaReconciliacionImportacion{Lote=f.Source.Lot,Plan=f.Plan,Raiz=f.Source.Root,Archivo=f.Source.FileRecord,Intento=f.Source.Attempt};
        Action<DocumentPhaseFixture> reserve=f=>{f.AfterReserve=()=>f.Allowed=false;f.Run();f.Allowed=true;Check(f.Source.Attempt.EjecutorRef=="WEB_DOCUMENTO"&&f.Source.Attempt.EjecutorRef!=f.Source.Lot.Contexto.UsuarioRef,"Closure fixture replaces technical executor with actor identity");
            f.Plan.Resultado.Elementos.Clear();f.Plan.Resultado.Elementos.Add(new ValoresElementoImportacion{Ambito="RAIZ",RaizIdCliente=f.Source.Root.ClienteRaizId,Valores=ReceiptClone(f.Source.Root.ValoresPlan)});
            f.Plan.Resultado.Elementos.Add(new ValoresElementoImportacion{Ambito="ARCHIVO",RaizIdCliente=f.Source.Root.ClienteRaizId,ArchivoIdCliente=f.Source.FileRecord.ClienteArchivoId,Valores=ReceiptClone(f.Source.FileRecord.ValoresPlan)});};
        Func<VistaReconciliacionImportacion,VistaReconciliacionImportacion,int,int,int,Action,ResultadoEfectoImportacion> close=(expected,actual,reserved,closed,other,onWrite)=>(ResultadoEfectoImportacion)RepositoryHelper("PersistirCierreReservaSinInicio",expected,actual,reserved,closed,other,DateTime.UtcNow,new Func<string,object[],int>((sql,args)=>{onWrite();Check(sql.StartsWith("UPDATE ra_integracion_")&&sql.Contains("WHERE ")&&!sql.Contains("DELETE "),"Prestart close writes documentary effects");return 1;}),new Action<ResultadoEfectoImportacion>(r=>onWrite()));
        using(var f=new DocumentPhaseFixture()){
            reserve(f);var v=view(f);var original=ReceiptClone(v);int writes=0;
            var claim=ReceiptClone(v.Intento);claim.EsNuevaReserva=true;
            var stale=ComandoAlmacenamientoCarpetas.Preparar(v.Lote.Contexto,v.Raiz,v.Archivo,v.Lote.Snapshot,claim);
            var r=close(original,v,1,0,0,()=>writes++);
            Check(writes==4&&r.Estado=="RECHAZADO"&&r.Codigo=="RESERVA_CERRADA_SIN_INICIO"&&r.DocumentoId==null&&r.InventarioId==null&&f.Source.Writers==0,"Prestart close fabricates documentary success");
            v.Intento.Estado="RECHAZADO";v.Intento.FinUtc="2026-11-01T12:00:00Z";v.Intento.Resultado=r;
            v.Archivo.EstadoEfecto="NO_INICIADO";v.Archivo.IntentoActivoId=null;v.Archivo.Revision++;v.Lote.Revision++;
            Rejected("CONFLICTO_REVISION",()=>RepositoryHelper("ValidarInicioDocumento",stale,v.Lote,v.Raiz,v.Archivo));
            Check(v.Archivo.ValoresEfectivosFijados&&v.Raiz.IdentidadExpediente.ExpedienteId==original.Raiz.IdentidadExpediente.ExpedienteId&&File.Exists(f.Source.Stage.PathFor("30")),"Prestart close loses confirmed expedition/values/original");
            // A later reservation is never cleared by a repeated old close.
            v.Archivo.EstadoEfecto="EN_CURSO";v.Archivo.IntentoActivoId="99";v.Archivo.Revision++;
            var replay=close(original,v,1,1,0,()=>writes++);Check(writes==4&&replay.Codigo==r.Codigo&&v.Archivo.IntentoActivoId=="99","Repeated close cancels a new owner");
        }
        foreach(var mutate in new Action<VistaReconciliacionImportacion>[] {
            v=>v.Intento.Estado="EN_CURSO",v=>v.Intento.Estado="INCIERTO",v=>v.Intento.Resultado=new ResultadoEfectoImportacion{Estado="EN_CURSO",Codigo="ALMACENAMIENTO_PREPARADO"},
            v=>v.Intento.FinUtc="2026-11-01T12:00:00Z",v=>v.Intento.EjecutorRef="99",v=>v.Intento.Fase="CREAR_EXPEDIENTE",v=>v.Archivo.IntentoActivoId="99",v=>v.Archivo.EstadoRecepcion="PENDIENTE",
            v=>v.Archivo.ValoresEfectivosFijados=false,v=>v.Archivo.Resultado=new ResultadoEfectoImportacion(),v=>v.Lote.DetencionSolicitada=true,v=>v.Raiz.Estado="INCIERTO",v=>v.Lote.Contexto.UsuarioRef="99"
        })using(var f=new DocumentPhaseFixture()){
            reserve(f);var expected=ReceiptClone(view(f));mutate(view(f));int writes=0;
            try{close(expected,view(f),1,0,0,()=>writes++);throw new Exception("Unsafe prestart close accepted");}catch(PerfilImportacionException){}
            Check(writes==0&&f.Source.Writers==0,"Unsafe close mutated state");
        }
        foreach(var counts in new[]{new[]{0,0,0},new[]{2,0,0},new[]{1,0,1},new[]{1,1,0}})using(var f=new DocumentPhaseFixture()){
            reserve(f);var v=view(f);int writes=0;Rejected("CONFLICTO_REVISION",()=>close(ReceiptClone(v),v,counts[0],counts[1],counts[2],()=>writes++));Check(writes==0,"Close ignored start/result journal");
        }
        using(var f=new DocumentPhaseFixture()){
            reserve(f);var v=view(f);int writes=0;bool allowed=true;int closed=0;
            var coordinator=new CoordinadorImportacionCarpetas(()=>ReceiptClone(v.Lote.Contexto),f,n=>ReceiptClone(v.Lote.Contexto),(c,config)=>{if(!allowed)throw new PerfilImportacionException("ACCESO_DENEGADO","Fixture");},
                (c,l,i)=>ReceiptClone(v),x=>{throw new Exception("Close invokes documentary verifier");},(x,r)=>{throw new Exception("Close invokes positive reconciliation");},
                expected=>{var result=close(expected,v,1,closed,0,()=>writes++);if(closed==0){closed=1;v.Intento.Estado="RECHAZADO";v.Intento.FinUtc="2026-11-01T12:00:00Z";v.Intento.Resultado=result;v.Archivo.EstadoEfecto="NO_INICIADO";v.Archivo.IntentoActivoId=null;v.Archivo.Revision++;v.Lote.Revision++;}return result;});
            var request=new ReconciliarRecursoImportacionSolicitud{loteId="20",intentoId="50",revisionEsperada=v.Lote.Revision};
            var endpoint=OperationsEndpoint(new ResourceFixture(),purpose=>coordinator);
            var r=SendOperation(endpoint,"CERRAR",request);var data=(ReconciliacionRecursoImportacionResultado)r.datos;
            Check(r.ok&&data.codigo=="RESERVA_CERRADA_SIN_INICIO"&&data.resuelta&&data.documentoId==null&&writes==4&&f.Source.Writers==0,"HTTP closure chains effects or claims a document");
            Check(((ReconciliacionRecursoImportacionResultado)SendOperation(endpoint,"CERRAR",request).datos).repeticion==true&&writes==4,"HTTP close replay writes");
            allowed=false;Check(SendOperation(endpoint,"CERRAR",request).codigo=="ACCESO_DENEGADO"&&writes==4,"Close replay skips current authority");
        }
        Console.WriteLine("DOC111 explicit prestart closure, journal guards, stale-start fence and replay: passed");
    }
    static void ReconciliationCases(){
        CloseUnstartedReservationCases();
        using(var f=new ReconciliationFixture()){
            var original=ReceiptClone(f.View());var revision=f.Source.Phase.Source.Lot.Revision;var result=f.Run();var s=f.Source.Phase.Source;
            Check(result.Resuelta&&!result.Repeticion&&f.Verifies==1&&f.Saves==1&&f.Writes==4&&f.Events==1&&s.Lot.Estado=="COMPLETADO"&&s.Root.Estado=="COMPLETADA"&&s.Writers==1,"Reconciliation did not durably confirm and consolidate known document");
            var replay=f.Coordinator.ReconciliarIntento("20","50",revision);Check(replay.Resuelta&&replay.Repeticion&&f.Verifies==1&&f.Saves==1&&s.Writers==1,"Reconciliation replay rechecks or repeats effects");
            f.Save(original,f.View(),s.Attempt.Resultado);Check(f.Writes==4&&f.Events==1,"Repeated reconciliation checkpoint writes/events again");
        }
        foreach(var fail in new Action<ReconciliationFixture>[] {f=>f.Verify=false,f=>f.ThrowVerify=true,f=>f.ThrowSave=true})using(var f=new ReconciliationFixture()){
            fail(f);var before=PerfilImportacionJson.Escribir(f.View());var r=f.Run();Check(!r.Resuelta&&r.Resultado.Estado=="INCIERTO"&&PerfilImportacionJson.Escribir(f.View())==before&&f.Events==0&&f.Source.Phase.Source.Writers==1&&!PerfilImportacionJson.Escribir(r).Contains("PRIVATE_"),"Failed reconciliation changed durable evidence or leaked error");
        }
        using(var f=new ReconciliationFixture()){f.LoseResponse=true;Check(f.Run().Resuelta&&f.Saves==1&&f.Events==1&&f.Source.Phase.Source.Writers==1,"Lost reconciliation response hides durable checkpoint or repeats effect");}
        foreach(var state in new[]{"RESERVADO","EN_CURSO"})using(var f=new ReconciliationFixture()){
            f.Source.Phase.Source.Attempt.Estado=state;f.Source.Phase.Source.Attempt.InicioUtc="2000-01-01T00:00:00Z";
            Check(!f.Run().Resuelta&&f.Verifies==0&&f.Saves==0&&f.Source.Phase.Source.FileRecord.IntentoActivoId=="50","Expired claim authorizes reconciliation or clears owner");
        }
        using(var f=new ReconciliationFixture()){
            f.Source.Phase.Source.Attempt.Resultado.InventarioId=null;f.Source.Phase.Source.FileRecord.Resultado=ReceiptClone(f.Source.Phase.Source.Attempt.Resultado);
            Check(!f.Run().Resuelta&&f.Verifies==0&&f.Saves==0,"Incomplete known IDs authorize reconciliation");
        }
        using(var f=new ReconciliationFixture()){f.RevokeAfterVerify=true;Rejected("ACCESO_DENEGADO",()=>f.Run());Check(f.Saves==0&&f.Events==0,"Revoked authority permits reconciliation checkpoint");}
        using(var f=new ReconciliationFixture()){Rejected("CONFLICTO_REVISION",()=>f.Coordinator.ReconciliarIntento("20","50",1));Check(f.Verifies==0&&f.Saves==0,"Stale revision reaches verifier");}
        using(var f=new ReconciliationFixture()){f.Source.Phase.Source.Lot.DetencionSolicitada=true;Check(f.Run().Resuelta&&f.Source.Phase.Source.Lot.DetencionSolicitada,"Reconciliation cleared stop signal");}
        using(var f=new ReconciliationFixture()){f.Source.AddFile();f.Source.Phase.Source.Lot.DetencionSolicitada=true;Check(f.Run().Resuelta&&f.Source.Phase.Source.Lot.Estado=="DETENIDO"&&f.Source.Phase.Source.Lot.DetencionSolicitada,"Reconciliation restarted a stopped lot with remaining work");}
        using(var f=new ReconciliationFixture()){f.Source.Phase.Source.FileRecord.NombreOriginal="different.pdf";Rejected("CONFLICTO_REVISION",()=>f.Run());Check(f.Verifies==0&&f.Saves==0,"Changed original name reaches verifier");}
        using(var f=new ReconciliationFixture()){
            var v=f.View();var c=ComandoAlmacenamientoCarpetas.PrepararVerificacion(v.Lote.Contexto,v.Raiz,v.Archivo,v.Lote.Snapshot,v.Intento);var repo=new MySqlImportacionCarpetasRepository("invalid-reconciliation-test");
            Rejected("SOLICITUD_INVALIDA",()=>repo.IniciarAlmacenamientoDocumento(c,f.Source.Phase.Source.Data));Rejected("SOLICITUD_INVALIDA",()=>repo.RegistrarResultadoDocumento(c,v.Intento.Resultado));
        }
        using(var f=new ReconciliationFixture()){
            f.Source.AddFile();var other=ReceiptClone(f.Source.Phase.Source.Attempt);other.IntentoId="53";other.ArchivoId="31";f.Source.Proofs.Add(other);var file=f.Source.Phase.Files["31"];file.EstadoEfecto="INCIERTO";file.IntentoActivoId="53";
            Check(f.Run().Resuelta&&f.Source.Phase.Source.Lot.Estado=="PENDIENTE_VERIFICACION"&&file.IntentoActivoId=="53","One reconciliation cleared another uncertain effect");
        }
        foreach(var failAt in new[]{1,2,3,4})using(var f=new ReconciliationFixture()){
            f.FailAt=failAt;var expected=ReceiptClone(f.View());var result=f.Verifier().Verificar(expected);var before=PerfilImportacionJson.Escribir(f.View());Rejected("CONFLICTO_REVISION",()=>f.Save(expected,f.View(),result));Check(f.Events==0&&PerfilImportacionJson.Escribir(f.View())==before,"Failed CAS emits confirmation or mutates input");
        }
        using(var f=new ReconciliationFixture()){
            var expected=ReceiptClone(f.View());var result=f.Verifier().Verificar(expected);result.DocumentoId="999";Rejected("CONFLICTO_REVISION",()=>f.Save(expected,f.View(),result));Check(f.Writes==0,"Verifier substituted evidence IDs");
            result=f.Verifier().Verificar(expected);f.Source.Phase.Source.FileRecord.Revision++;Rejected("CONFLICTO_REVISION",()=>f.Save(expected,f.View(),result));Check(f.Writes==0,"Changed view accepted after SELECT");
        }
        using(var f=new CreationPhaseFixture()){
            f.ThrowWriter=true;f.Run();var v=new VistaReconciliacionImportacion{Lote=f.Stage.View.Lote,Plan=f.Stage.View.Plan,Raiz=f.Stage.View.Raiz,Intento=f.Attempt};
            v.Plan.Resultado.Elementos.Clear();v.Plan.Resultado.Elementos.Add(new ValoresElementoImportacion{Ambito="RAIZ",RaizIdCliente=v.Raiz.ClienteRaizId,Valores=ReceiptClone(v.Raiz.ValoresPlan)});
            int reads=0,writes=0,events=0;string cycle="13";
            var verifier=ReconciliationVerifier(c=>{Check(c.EsVerificacion&&c.IntentoId=="50","Creation reconciliation fabricated execution grant");return new Dictionary<string,string>{{"CICLO_ARCHIVO_ID",cycle}};},(c,i,d)=>{reads++;Check(i.ExpedienteId=="60"&&i.RelacionNivelId=="70","Creation reconciliation changed IDs");return true;},c=>{throw new Exception("Creation queried document");},(c,d,r)=>{throw new Exception("Creation queried document");});
            var result=verifier.Verificar(v);Check(result.Estado=="CONFIRMADO"&&reads==1,"Known creation SELECT not reconcilable");
            var c=ComandoCreacionExpedienteImportacion.PrepararVerificacion(v.Lote.Contexto,v.Lote,v.Raiz,v.Intento);var repo=new MySqlImportacionCarpetasRepository("invalid-reconciliation-test");
            Rejected("SOLICITUD_INVALIDA",()=>repo.IniciarCreacionExpediente(c,new Dictionary<string,string>()));Rejected("SOLICITUD_INVALIDA",()=>repo.RegistrarResultadoCreacion(c,result));
            RepositoryHelper("PersistirReconciliacion",ReceiptClone(v),ReceiptClone(v),result,DateTime.UtcNow,new Func<string,object[],int>((sql,args)=>{writes++;Check(!sql.Contains("detencion_solicitada="),"Creation reconciliation clears stop");return 1;}),new Action<ResultadoEfectoImportacion>(r=>events++));Check(writes==3&&events==1&&f.Writers==1,"Creation reconciliation repeats writer or misses checkpoint");
            cycle="14";Check(verifier.Verificar(v)==null&&reads==1,"Changed approved cycle accepted");
            Rejected("CONFLICTO_REVISION",()=>ComandoCreacionExpedienteImportacion.Preparar(v.Lote.Contexto,v.Lote,v.Raiz,v.Intento));
        }
        Console.WriteLine("DOC-111 reconciliation confirms only known uncertain evidence without replaying effects (synthetic SELECT and SQL): passed");
    }
    sealed class CleanupFixture : IDisposable,ILimpiezaOriginalImportacionRepository {
        public ConsolidationFixture Source=new ConsolidationFixture();public QueryRepository Query=new QueryRepository();
        public PoliticaLimpiezaOriginalImportacion Policy=new PoliticaLimpiezaOriginalImportacion(true,TimeSpan.FromDays(1));
        public DateTime Now=new DateTime(2026,10,30,14,0,0,DateTimeKind.Utc);public bool Allowed=true,Verify=true,FailPrepare,FailFinal,LosePrepareResponse,LoseFinalResponse;
        public int StageFactories,Verifications,PreparedEvents,FinalEvents,Removed;public Action OnVerify,AfterPrepare;
        public AutorizacionLimpiezaOriginalImportacion Permit;public string FinalCode,Failure;public ServicioConsultaImportacion Service;readonly string original;
        public CleanupFixture(){
            Source.Finish();Source.Save();var s=Source.Phase.Source;s.Lot.ActualizadoUtc="2026-10-28T12:00:00Z";s.Attempt.FinUtc="2026-10-28T12:00:00Z";foreach(var proof in Source.Proofs)proof.FinUtc="2026-10-28T12:00:00Z";original=s.Stage.PathFor("30");
            Query.Lot=s.Lot;Query.Roots=Source.Roots;Query.Files=Source.Phase.Files.Values.ToList();Query.Active=false;Query.Uncertain=false;
            var constructor=typeof(VerificadorLimpiezaOriginalImportacion).GetConstructors(BindingFlags.NonPublic|BindingFlags.Instance).Single();
            var verifier=(VerificadorLimpiezaOriginalImportacion)constructor.Invoke(new object[]{new Func<ComandoAlmacenamientoCarpetas,DatosAlmacenamientoImportacion>(c=>{
                Check(c.EsVerificacion&&c.Intento.Estado=="CONFIRMADO"&&c.Intento.FinUtc!=null&&c.Archivo.EstadoEfecto=="COMPLETADO"&&c.Raiz.Estado=="COMPLETADA","Cleanup fabricates an executable reservation");return s.Data;
            }),new Func<ComandoAlmacenamientoCarpetas,DatosAlmacenamientoImportacion,ResultadoEfectoImportacion,bool>((c,d,r)=>{Verifications++;Check(r.DocumentoId=="90"&&r.InventarioId=="100","Cleanup verifies other effect");if(OnVerify!=null)OnVerify();return Verify;})});
            Service=new ServicioConsultaImportacion(()=>ReceiptClone(s.Lot.Contexto),Query,n=>ReceiptClone(s.Lot.Contexto),(c,config)=>{if(!Allowed)throw new PerfilImportacionException("ACCESO_DENEGADO","fixture");},2,()=>Policy,this,verifier.Verificar,
                (lot,file)=>{StageFactories++;return new AlmacenStagingImportacion(s.Stage.Receipt.Root,s.Stage.Receipt.Limits,lot,file,10,200);},()=>Now);
        }
        public VistaLimpiezaOriginalImportacion View(){return new VistaLimpiezaOriginalImportacion{Inventario=ReceiptClone(Source.View()),ArchivoId="30",Autorizacion=Permit==null?null:ReceiptClone(Permit),CodigoFinal=FinalCode};}
        public string Original{get{return original;}}
        public LimpiarOriginalImportacionSolicitud Request(){return new LimpiarOriginalImportacionSolicitud{loteId="20",archivoId="30",revisionLoteEsperada=Source.Phase.Source.Lot.Revision,revisionArchivoEsperada=Source.Phase.Source.FileRecord.Revision};}
        public LimpiezaOriginalImportacionResultado Run(){return Service.LimpiarOriginal(Request());}
        public VistaLimpiezaOriginalImportacion ObtenerVistaLimpieza(ContextoImportacionCarpetas c,string lot,string file){Check(lot=="20"&&file=="30"&&c.UsuarioRef==Source.Phase.Source.Lot.Contexto.UsuarioRef,"Cleanup escaped scope");return View();}
        void Journal(string action,string code,AutorizacionLimpiezaOriginalImportacion permit){
            if(action=="PREPARAR_LIMPIEZA"){
                if(FailPrepare)throw new Exception("PRIVATE_PREPARE_FAILURE");Check(Permit==null&&System.IO.File.Exists(Original),"Cleanup authorization written after deletion or twice");Permit=ReceiptClone(permit);PreparedEvents++;
            }else{
                if(FailFinal)throw new Exception("PRIVATE_FINAL_FAILURE");Check(action=="CONCLUIR_LIMPIEZA"&&Permit!=null&&FinalCode==null&&!System.IO.File.Exists(Original),"Cleanup journal confirmed before effect or without durable preparation");FinalCode=code;FinalEvents++;
            }
        }
        public VistaLimpiezaOriginalImportacion PrepararLimpieza(VistaLimpiezaOriginalImportacion expected,PoliticaLimpiezaOriginalImportacion policy,Action<VistaLimpiezaOriginalImportacion> check){
            var result=(VistaLimpiezaOriginalImportacion)RepositoryHelper("PrepararLimpiezaNucleo",expected,View(),policy,Now,check,new Action<string,string,AutorizacionLimpiezaOriginalImportacion>(Journal));
            if(AfterPrepare!=null)AfterPrepare();if(LosePrepareResponse)throw new Exception("PRIVATE_LOST_PREPARE_RESPONSE");return result;
        }
        public LimpiezaOriginalImportacionResultado AplicarLimpieza(VistaLimpiezaOriginalImportacion expected,PoliticaLimpiezaOriginalImportacion policy,Action<VistaLimpiezaOriginalImportacion> check,Func<bool> remove){
            LimpiezaOriginalImportacionResultado result;try{result=(LimpiezaOriginalImportacionResultado)RepositoryHelper("AplicarLimpiezaNucleo",expected,View(),policy,Now,check,new Func<bool>(()=>{Check(Permit!=null,"Delete before committed authorization");bool removed=remove();if(removed)Removed++;return removed;}),new Action<string,string,AutorizacionLimpiezaOriginalImportacion>(Journal));}catch(Exception e){Failure=(e.InnerException??e).Message;throw;}
            if(LoseFinalResponse)throw new Exception("PRIVATE_LOST_FINAL_RESPONSE");return result;
        }
        public void Dispose(){Source.Dispose();}
    }
    static void CleanupCases(){
        Check(!new PoliticaLimpiezaOriginalImportacion(false,TimeSpan.Zero).Habilitada,"Absent explicit cleanup policy enables deletion");
        Unavailable(()=>new PoliticaLimpiezaOriginalImportacion(true,TimeSpan.Zero));Unavailable(()=>new PoliticaLimpiezaOriginalImportacion(true,TimeSpan.FromSeconds(-1)));
        Check(new PoliticaLimpiezaOriginalImportacion(true,TimeSpan.FromDays(1)).Retencion==TimeSpan.FromDays(1),"Explicit cleanup period changed");
        using(var f=new CleanupFixture()){
            var before=PerfilImportacionJson.Escribir(f.View().Inventario);var request=f.Request();var s=f.Source.Phase.Source;
            // Elementos ajenos y copias nunca forman parte del retiro exacto.
            var part=f.Original+".part";System.IO.File.WriteAllText(part,"synthetic extra");
            var result=f.Run();Check(result.estado=="CONFIRMADO"&&result.codigo=="STAGING_ELIMINADO"&&f.PreparedEvents==1&&f.FinalEvents==1&&f.Removed==1&&!System.IO.File.Exists(f.Original)&&System.IO.File.Exists(part),"Cleanup did not remove only owned original after durable preparation: "+result.estado+"; stages="+f.StageFactories+"; prepared="+f.PreparedEvents+"; final="+f.FinalEvents+"; removed="+f.Removed+"; failure="+f.Failure);
            Check(PerfilImportacionJson.Escribir(f.View().Inventario)==before,"Cleanup changes states, IDs, hashes, revisions, counts or history");
            var factories=f.StageFactories;result=f.Service.LimpiarOriginal(request);Check(result.repeticion&&f.StageFactories==factories&&f.Removed==1&&f.PreparedEvents==1&&f.FinalEvents==1,"Cleanup replay touches disk or journals again");
            var replay=f.Source.Phase.Run();Check(replay.Repeticion&&replay.Resultado.Estado=="CONFIRMADO"&&s.Writers==1,"Cleanup breaks documentary historical read or repeats writer");
            var projected=PerfilImportacionJson.Escribir(result);Check(!projected.Contains("Staging")&&!projected.Contains("Huella")&&!projected.Contains("App_Data")&&!projected.Contains("PRIVATE"),"Cleanup output exposes internal references");
            var command=ComandoAlmacenamientoCarpetas.PrepararComprobacionFinal(s.Lot.Contexto,s.Root,s.FileRecord,s.Lot.Snapshot,s.Attempt);var repo=new MySqlImportacionCarpetasRepository("invalid-cleanup-test");
            Rejected("SOLICITUD_INVALIDA",()=>repo.IniciarAlmacenamientoDocumento(command,s.Data));Rejected("SOLICITUD_INVALIDA",()=>repo.RegistrarResultadoDocumento(command,s.Attempt.Resultado));
        }
        foreach(var mutate in new Action<CleanupFixture>[] {f=>f.Policy=new PoliticaLimpiezaOriginalImportacion(false,TimeSpan.Zero),f=>f.Policy=new PoliticaLimpiezaOriginalImportacion(true,TimeSpan.FromDays(30)),f=>f.Source.Phase.Source.Lot.ActualizadoUtc="invalid",f=>f.Source.Phase.Source.Lot.ActualizadoUtc="2026-11-01T00:00:00Z",f=>f.Source.Phase.Source.Lot.Estado="PARCIAL",f=>f.Source.Phase.Source.Lot.Estado="PENDIENTE_VERIFICACION",f=>f.Source.Phase.Source.FileRecord.IntentoActivoId="999",f=>f.Source.Proofs.Clear(),f=>f.Source.Phase.Source.Attempt.FinUtc=null,f=>f.Verify=false,f=>f.Allowed=false})using(var f=new CleanupFixture()){
            mutate(f);bool rejected=false;try{f.Run();}catch(PerfilImportacionException){rejected=true;}Check(rejected&&System.IO.File.Exists(f.Original)&&f.StageFactories==0&&f.PreparedEvents==0&&f.Removed==0,"Invalid/active/uncertain or unauthorized candidate reaches disk or delete");
        }
        using(var f=new CleanupFixture()){f.Source.Phase.Source.Attempt.FinUtc="2026-10-30T13:00:00Z";Unavailable(()=>f.Run());Check(f.StageFactories==0&&f.Removed==0,"Expired lot date overrides retention of a more recent final checkpoint");}
        using(var f=new CleanupFixture()){var request=f.Request();request.revisionArchivoEsperada--;Rejected("CONFLICTO_REVISION",()=>f.Service.LimpiarOriginal(request));Check(f.StageFactories==0&&f.Removed==0,"Stale cleanup revision opens disk");}
        using(var f=new CleanupFixture()){f.OnVerify=()=>f.Allowed=false;Rejected("ACCESO_DENEGADO",()=>f.Run());Check(f.Removed==0&&f.PreparedEvents==0&&System.IO.File.Exists(f.Original),"Revoked authority permits durable delete authorization");}
        using(var f=new CleanupFixture()){f.AfterPrepare=()=>f.Allowed=false;Rejected("ACCESO_DENEGADO",()=>f.Run());Check(f.PreparedEvents==1&&f.Removed==0&&System.IO.File.Exists(f.Original),"Revoked authority after journal permits deletion");}
        using(var f=new CleanupFixture()){f.AfterPrepare=()=>f.Policy=new PoliticaLimpiezaOriginalImportacion(true,TimeSpan.FromDays(30));Check(f.Run().estado=="INCIERTO"&&f.PreparedEvents==1&&f.Removed==0&&System.IO.File.Exists(f.Original),"Old authorization overrides a longer current retention");}
        using(var f=new CleanupFixture()){f.OnVerify=()=>{if(f.Verifications==3)f.Verify=false;};Check(f.Run().estado=="INCIERTO"&&f.PreparedEvents==1&&f.Removed==0&&System.IO.File.Exists(f.Original),"Document no longer verifiable after journal permits deletion");}
        using(var f=new CleanupFixture()){f.AfterPrepare=()=>f.Policy=new PoliticaLimpiezaOriginalImportacion(false,TimeSpan.Zero);var result=f.Run();Check(result.estado=="INCIERTO"&&f.PreparedEvents==1&&f.FinalEvents==0&&f.Removed==0&&System.IO.File.Exists(f.Original),"Policy disabled after journal permits deletion");}
        using(var f=new CleanupFixture()){f.FailPrepare=true;Check(f.Run().estado=="INCIERTO"&&f.Removed==0&&System.IO.File.Exists(f.Original),"Prepare failure deletes original");}
        using(var f=new CleanupFixture()){f.LosePrepareResponse=true;Check(f.Run().estado=="INCIERTO"&&f.PreparedEvents==1&&f.Removed==0,"Lost prepare response starts deletion");f.LosePrepareResponse=false;Check(f.Run().estado=="CONFIRMADO"&&f.PreparedEvents==1&&f.Removed==1,"Prepare recovery creates another authorization");}
        using(var f=new CleanupFixture()){f.FailFinal=true;var r=f.Run();Check(r.estado=="INCIERTO"&&f.Permit!=null&&f.Removed==1&&f.FinalEvents==0&&!PerfilImportacionJson.Escribir(r).Contains("PRIVATE"),"Final journal failure loses recovery evidence or leaks details");f.FailFinal=false;r=f.Run();Check(r.estado=="CONFIRMADO"&&r.codigo=="STAGING_AUSENTE_AUTORIZADO"&&f.Removed==1&&f.PreparedEvents==1&&f.FinalEvents==1,"Recovery deletes twice or cannot close authorized absence");}
        using(var f=new CleanupFixture()){f.LoseFinalResponse=true;Check(f.Run().estado=="INCIERTO"&&f.FinalEvents==1,"Lost final response reported without durable result");f.LoseFinalResponse=false;Check(f.Run().repeticion&&f.Removed==1&&f.FinalEvents==1,"Lost final response repeats cleanup");}
        using(var f=new CleanupFixture()){System.IO.File.Delete(f.Original);var r=f.Run();Check(r.estado=="CONSERVADO"&&r.codigo=="ORIGINAL_NO_VERIFICABLE"&&f.PreparedEvents==0&&f.FinalEvents==0,"Absent original without journal is attributed to cleanup");}
        using(var f=new CleanupFixture()){
            var s=f.Source.Phase.Source;var store=new AlmacenStagingImportacion(s.Stage.Receipt.Root,s.Stage.Receipt.Limits,s.Lot,s.FileRecord,10,200);
            using(var retention=store.ConservarVerificado(s.Lot.Contexto)){Check(f.Run().estado=="INCIERTO"&&f.PreparedEvents==0&&f.Removed==0&&System.IO.File.Exists(f.Original),"Cleanup deletes an original retained by a reader");}
        }
        using(var f=new CleanupFixture()){System.IO.File.WriteAllText(f.Original,"changed");Check(f.Run().estado=="INCIERTO"&&f.PreparedEvents==0&&f.Removed==0,"Cleanup deletes altered original");}
        using(var f=new CleanupFixture()){f.AfterPrepare=()=>f.Source.Phase.Source.FileRecord.Revision++;Check(f.Run().estado=="INCIERTO"&&f.Removed==0&&System.IO.File.Exists(f.Original),"Changed metadata after journal permits cleanup");}
        Check(typeof(WebServiceImportacionCarpetas).GetMethod("LimpiarOriginal")==null,"Cleanup unexpectedly exposed as endpoint");
        Console.WriteLine("DOC-111 cleanup retains recovery evidence and journals exact original removal (local PDF and SQL doubles): passed");
    }
    static void ConsolidationCases(){
        using(var f=new ConsolidationFixture()){
            f.Finish();var original=PerfilImportacionJson.Escribir(f.View());var r=f.Evaluate();Check(r.Estado=="COMPLETADO"&&r.Raices.Single().Estado=="COMPLETADA"&&r.ArchivosConfirmados==1&&!r.HayPendientes&&!r.HayIncertidumbre,"DOC111 complete result lacks positive proof consolidation");Check(PerfilImportacionJson.Escribir(f.View())==original,"DOC111 pure consolidation mutates records");
            f.Save();Check(f.Writes==2&&f.Events==1&&f.Phase.Source.Lot.TotalArchivos==1,"DOC111 consolidation updates more than owned states");f.Save();Check(f.Writes==2&&f.Events==1,"DOC111 unchanged consolidation writes/events again");
            var stages=f.Phase.StageFactories;var replay=f.Phase.Run();Check(replay.Repeticion&&replay.Resultado.Estado=="CONFIRMADO"&&f.Phase.Source.Writers==1&&f.Phase.StageFactories==stages,"DOC111 completed root/lote prevents safe historical read or replays effect");
            f.Phase.Source.FileRecord.Resultado.DocumentoId="999";Rejected("CONFIGURACION_INVALIDA",()=>f.Save());Check(f.Events==1,"DOC111 inconsistent terminal completion silently repaired");
        }
        using(var f=new ConsolidationFixture()){
            f.AddRoot();f.Finish();var r=f.Save();Check(r.Estado=="EN_CURSO"&&f.Roots[0].Estado=="COMPLETADA"&&f.Roots[1].Estado=="PENDIENTE"&&r.HayPendientes&&r.ArchivosConfirmados==1,"DOC111 first completed homonymous root completes entire lot");
            f.FinishSecond();r=f.Save();Check(r.Estado=="COMPLETADO"&&r.Raices.All(x=>x.Estado=="COMPLETADA")&&r.ArchivosConfirmados==2&&f.Phase.Source.Lot.TotalRaices==2&&f.Phase.Source.Lot.TotalArchivos==2,"DOC111 all original roots not consolidated independently");
        }
        using(var f=new ConsolidationFixture()){
            f.AddFile();f.Finish();var r=f.Evaluate();Check(r.Estado=="EN_CURSO"&&r.Raices.Single().Estado=="EXPEDIENTE_CONFIRMADO"&&r.HayPendientes,"DOC111 pending sibling hidden by completed document");
            f.Phase.Partial();f.Phase.Files["31"].EstadoRecepcion="RECHAZADO";f.Phase.Files["31"].EstadoEfecto="RECHAZADO";f.Phase.Plan.Resultado.Incidencias.Add(new IncidenciaImportacionCarpetas{RaizIdCliente="r1",ArchivoIdCliente="f2",BloqueaUnidad=true});var snapshot=PerfilImportacionJson.Escribir(f.Phase.Source.Lot.Snapshot);r=f.Save();Check(r.Estado=="PARCIAL"&&r.Raices.Single().Estado=="PARCIAL"&&r.HayRechazados&&r.HayPendientes&&f.Phase.Source.Lot.TotalArchivos==2&&f.Phase.Files.Count==2&&snapshot==PerfilImportacionJson.Escribir(f.Phase.Source.Lot.Snapshot),"DOC111 partial excludes rejection or changes original policy/totals");
        }
        foreach(var mutate in new Action<ConsolidationFixture>[] {f=>f.Proofs.Clear(),f=>f.Phase.Source.Attempt.FinUtc=null,f=>f.Phase.Source.Attempt.Resultado.InventarioId="999",f=>f.Phase.Source.FileRecord.ValoresEfectivosFijados=false,f=>f.Phase.Source.FileRecord.Resultado.Expediente.ExpedienteId="999",f=>{var copy=ReceiptClone(f.Phase.Source.Attempt);copy.IntentoId="53";f.Proofs.Add(copy);}})using(var f=new ConsolidationFixture()){
            f.Finish();mutate(f);var r=f.Evaluate();Check(r.Estado=="PENDIENTE_VERIFICACION"&&r.HayIncertidumbre,"DOC111 label COMPLETADO accepted without consistent final proof");
        }
        using(var f=new ConsolidationFixture()){f.Phase.Source.FileRecord.IntentoActivoId="999";var r=f.Save();Check(r.Estado=="PENDIENTE_VERIFICACION"&&r.HayIncertidumbre&&r.HayIntentosActivos&&f.Phase.Source.FileRecord.IntentoActivoId=="999","DOC111 orphan claim cleared or treated as completion");}
        using(var f=new ConsolidationFixture()){f.Phase.Source.Root.Estado="INCIERTO";var r=f.Evaluate();Check(r.Estado=="PENDIENTE_VERIFICACION"&&r.Raices[0].Estado=="INCIERTO","DOC111 aggregation automatically reconciles root");}
        using(var f=new ConsolidationFixture()){f.Phase.Source.VerifyResult=false;var first=f.Phase.Run();f.Save();var stages=f.Phase.StageFactories;var replay=f.Phase.Run();Check(first.Resultado.Estado=="INCIERTO"&&f.Phase.Source.Root.Estado=="INCIERTO"&&replay.Repeticion&&replay.Resultado.DocumentoId=="90"&&f.Phase.Source.Writers==1&&f.Phase.StageFactories==stages,"DOC111 consolidated uncertain root prevents evidence read or permits replay");}
        using(var f=new ConsolidationFixture()){f.AddRoot();f.Finish();f.FinishSecond();f.Phase.Files["31"].Resultado.DocumentoId="90";f.Phase.Files["31"].Resultado.InventarioId="100";f.Proofs.Single(i=>i.IntentoId=="52").Resultado=ReceiptClone(f.Phase.Files["31"].Resultado);var r=f.Evaluate();Check(r.Estado=="PENDIENTE_VERIFICACION"&&r.Raices.All(x=>x.Estado=="INCIERTO"),"DOC111 same document/inventory assigned to two original files completes lot");}
        using(var f=new ConsolidationFixture()){f.Finish();var other=ReceiptClone(f.Phase.Source.Attempt);other.IntentoId="53";other.Estado="EN_CURSO";other.FinUtc=null;f.Proofs.Add(other);var r=f.Evaluate();Check(r.Estado=="PENDIENTE_VERIFICACION"&&r.Raices[0].Estado=="INCIERTO"&&r.HayIntentosActivos,"DOC111 orphan active attempt permits root completion");}
        using(var f=new ConsolidationFixture()){f.Phase.Source.Lot.DetencionSolicitada=true;var r=f.Save();Check(r.Estado=="DETENIDO"&&f.Phase.Source.Root.IdentidadExpediente.ExpedienteId=="60"&&r.HayPendientes,"DOC111 stopped lot loses confirmed expedition");}
        using(var f=new ConsolidationFixture()){
            var request=ControlCommand("ALMACENAR");request.RevisionEsperada=f.Phase.OriginalRevision;f.Phase.ReservarIntento(f.Phase.Source.Lot.Contexto,request);f.Phase.Source.Attempt.InicioUtc="2000-01-01T00:00:00Z";f.Phase.Source.Lot.DetencionSolicitada=true;
            var r=f.Evaluate();Check(r.Estado=="EN_CURSO"&&r.HayIntentosActivos&&!r.HayIncertidumbre,"DOC111 active expired attempt becomes stopped or loses claim");
            f.Phase.Source.Attempt.Estado="INCIERTO";r=f.Evaluate();Check(r.Estado=="PENDIENTE_VERIFICACION"&&r.HayIntentosActivos&&r.HayIncertidumbre,"DOC111 uncertainty does not take priority over stop/activity");
        }
        using(var f=new ConsolidationFixture()){f.Finish();f.Phase.Source.Lot.DetencionSolicitada=true;var r=f.Save();Check(r.Estado=="COMPLETADO"&&f.Phase.Source.Lot.DetencionSolicitada,"DOC111 stop arriving after final effect hides completion or is cleared");}
        foreach(var mutate in new Action<ConsolidationFixture>[] {f=>f.Phase.Source.Lot.TotalArchivos=0,f=>f.Phase.Source.Lot.TotalBytes="0",f=>f.Phase.Source.FileRecord.RaizId="999",f=>f.Phase.Plan.Resultado.Elementos.RemoveAt(0),f=>f.Phase.Source.FileRecord.ValoresPlan[0].Valor="999",f=>f.Proofs[0].LoteId="999",f=>f.Phase.Plan.Contexto.NivelRef="999"})using(var f=new ConsolidationFixture()){
            mutate(f);Rejected("CONFIGURACION_INVALIDA",()=>f.Save());Check(f.Writes==0&&f.Events==0,"DOC111 inconsistent or foreign aggregate reaches updates");
        }
        foreach(var fail in new[]{1,2})using(var f=new ConsolidationFixture()){f.Finish();f.FailAt=fail;var before=PerfilImportacionJson.Escribir(f.View());Rejected("CONFLICTO_REVISION",()=>f.Save());Check(f.Events==0&&PerfilImportacionJson.Escribir(f.View())==before,"DOC111 failed state CAS publishes aggregate");}
        Console.WriteLine("DOC-111 consolidation requires full original inventory positive checkpoints and preserves uncertainty stop (synthetic SQL): passed");
    }

    static object DocumentCatalogHelper(string method,params object[] args){try{return typeof(PerfilImportacionCatalogAdapter).GetMethod(method,BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,args);}catch(TargetInvocationException e){throw e.InnerException;}}
    static DataTable DocumentRow(IDictionary<string,object> values){var t=new DataTable();foreach(var key in values.Keys)t.Columns.Add(key,typeof(object));t.Rows.Add(values.Values.ToArray());return t;}
    static void DocumentAdapterCases(){
        using(var f=new DocumentAdapterFixture()){
            var originalRoot=PerfilImportacionJson.Escribir(f.Root);var originalValues=PerfilImportacionJson.Escribir(f.FileRecord.ValoresEfectivos);
            var originalBytes=System.IO.File.ReadAllBytes(f.Stage.PathFor("30"));
            var command=f.Prepare();command.Archivo.ValoresEfectivos.Clear();Check(f.Prepare().Archivo.ValoresEfectivos.Count==2,"DOC111 document command not immutable");
            var r=f.Run();Check(r.Estado=="CONFIRMADO"&&r.DocumentoId=="90"&&r.InventarioId=="100"&&f.Writers==1&&f.Starts==1&&f.Verifications==1&&f.Writes==3&&f.Events==1,"DOC111 document adapter not invoked/checkpointed exactly once");
            Check(System.IO.File.Exists(f.Stage.PathFor("30"))&&!System.IO.File.Exists(f.CopyPath)&&PerfilImportacionJson.Escribir(f.Root)==originalRoot&&PerfilImportacionJson.Escribir(f.FileRecord.ValoresEfectivos)==originalValues,"DOC111 consumed copy destroys original, root or fixed values");
            Check(originalBytes.SequenceEqual(System.IO.File.ReadAllBytes(f.Stage.PathFor("30"))),"DOC117 consumed copy changes original bytes");
            using(var released=new FileStream(f.Stage.PathFor("30"),FileMode.Open,FileAccess.ReadWrite,FileShare.None)){}
            Rejected("CONFIGURACION_INVALIDA",()=>f.Run());Check(f.Writers==1,"DOC111 document adapter replays old attempt");
            RepositoryHelper("PersistirResultadoDocumento",f.Command,f.Lot,f.Root,f.FileRecord,f.Attempt,r,DateTime.UtcNow,new Func<string,object[],int>((sql,args)=>{throw new Exception("Terminal replay writes");}),new Action(()=>{throw new Exception("Terminal replay event");}));
        }
        foreach(var configure in new Action<DocumentAdapterFixture>[] {f=>f.ThrowWriter=true,f=>f.VerifyResult=false,f=>f.WrongName=true,f=>f.RejectWriter=true,f=>f.FailCheckpoint=true,f=>f.LoseCommitResponse=true})using(var f=new DocumentAdapterFixture()){
            configure(f);var r=f.Run();Check(r.Estado=="INCIERTO"&&r.DocumentoId=="90"&&r.InventarioId=="100"&&f.Writers==1&&System.IO.File.Exists(f.Stage.PathFor("30")),"DOC111 failed documentary phase loses known evidence or original");Check(!PerfilImportacionJson.Escribir(r).Contains("SECRET"),"DOC111 legacy exception leaks");Rejected("CONFIGURACION_INVALIDA",()=>f.Run());
        }
        using(var f=new DocumentAdapterFixture()){f.StartAllowed=false;Rejected("CONFLICTO_REVISION",()=>f.Run());Check(f.Writers==0&&f.Checkpoints==0,"DOC111 failed start invokes writer");}
        using(var f=new DocumentAdapterFixture()){f.RevokeBeforeWriter=true;var r=f.Run();Check(r.Estado=="INCIERTO"&&f.Writers==0&&f.Checkpoints==1,"DOC111 revocation before writer ignored");}
        using(var f=new DocumentAdapterFixture()){f.StopDuringWriter=true;var r=f.Run();Check(r.Estado=="CONFIRMADO"&&f.Lot.DetencionSolicitada,"DOC111 checkpoint loses stop signal or completed effect");}
        foreach(var mutate in new Action<DocumentAdapterFixture>[] {f=>f.FileRecord.ValoresEfectivosFijados=false,f=>f.Attempt.EsNuevaReserva=false,f=>f.Attempt.TokenReclamacion="invalid",f=>f.FileRecord.IntentoActivoId="99",f=>f.FileRecord.NombreOriginal="../wrong.pdf",f=>f.FileRecord.NombreOriginal="O'Brien.pdf",f=>f.FileRecord.NombreOriginal="name|other.pdf",f=>f.Root.IdentidadExpediente.NivelRef="99",f=>f.Lot.Snapshot.Reglas[0].ValorConstante="changed"})using(var f=new DocumentAdapterFixture()){
            mutate(f);Rejected("CONFIGURACION_INVALIDA",()=>f.Run());Check(f.Starts==0&&f.Writers==0,"DOC111 invalid documentary command reaches effect");
        }
        foreach(var name in new[]{"other.tif","other.tiff","multipage.TIF","multipage.TIFF"})using(var f=new DocumentAdapterFixture()){f.FileRecord.NombreOriginal=name;Rejected("FORMATO_NO_SOPORTADO",()=>f.Run());Check(f.Starts==0&&f.Writers==0,"DOC117 unsupported TIFF reaches effect");}
        using(var f=new DocumentAdapterFixture()){f.FileRecord.ValoresEfectivos.Add(ReceiptClone(f.FileRecord.ValoresEfectivos[0]));Rejected("CONFLICTO_REVISION",()=>f.Run());Check(f.Starts==0,"DOC111 duplicate effective values reach writer");}
        foreach(var name in new[]{"NOMBRE","nombre","CLASEDOCUMENTO","FECHAELABORACION","NOMBRESERIE","ID","ID_EXPEDIENTE","ID_INVENTARIO_DOCUMENTAL","bad-name"})using(var f=new DocumentAdapterFixture()){
            f.Data.Campos[0].Nombre=name;Rejected("CONFIGURACION_INVALIDA",()=>f.Run());Check(f.Starts==0,"DOC111 conflicting automatic/technical index ignored by legacy");
        }
        foreach(var fail in new[]{1,2,3})using(var f=new DocumentAdapterFixture()){f.FailSqlAt=fail;var r=f.Run();Check(r.Estado=="INCIERTO"&&f.Events==0&&f.FileRecord.Resultado==null,"DOC111 partial checkpoint publishes completion");}
        Console.WriteLine("DOC-111 documentary adapter retains original consumes named copy and checkpoints once (local PDF and writer SQL doubles): passed");
    }

    static void DocumentVerificationCases(){
        using(var f=new DocumentAdapterFixture()){
            var command=f.Prepare();var c=f.Lot.Contexto;var cfg=f.Lot.Snapshot.Configuracion;
            var ra=DocumentRow(new Dictionary<string,object>{{"expediente","60"},{"relacion","70"},{"nivel",c.NivelRef},{"empresa",c.AmbitoRef},{"usuario",c.UsuarioRef},{"nivel_activo","1"},{"electronico","2"},{"empresa_nivel",c.AmbitoRef},{"autorizado","1"},{"fondo",cfg.FondoRef},{"instrumento",cfg.InstrumentoRef},{"area",cfg.AreaRef},{"serie",cfg.SerieRef},{"subserie",DBNull.Value},{"tipo",cfg.TipoExpedienteRef},{"unidad",cfg.UnidadDocumentalRef}});
            var da=DocumentRow(new Dictionary<string,object>{{"documento","90"},{"inventario","100"},{"expediente","60"},{"usuario",c.UsuarioRef},{"nombre",f.FileRecord.NombreOriginal},{"gabinete","7"},{"documento_inventario","90"},{"expediente_inventario","60"},{"empresa",c.AmbitoRef},{"usuario_inventario",c.UsuarioRef},{"gabinete_nombre",f.Data.GabineteNombre},{"tipologia","9"},{"clase","3"},{"segundo_nombre",f.FileRecord.NombreOriginal},{"expediente_indice","60"},{"inventario_indice","100"},{"nombre_indice",f.FileRecord.NombreOriginal},{"valor_0","0007"}});
            foreach(var key in new[]{"tipo_archivo","formato_inventario","formato_indice"}){da.Columns.Add(key,typeof(object));da.Rows[0][key]=key=="tipo_archivo"?"6":".PDF";}
            da.Columns.Add("nombre_configuraciones",typeof(object));da.Rows[0]["nombre_configuraciones"]="1";da.Columns.Add("nombre_visible",typeof(object));da.Rows[0]["nombre_visible"]="1";
            int reads=0;Func<string,string,object[],DataTable> query=(origin,sql,args)=>{reads++;Check(sql.StartsWith("SELECT ")&&sql.EndsWith("LIMIT 2")&&!sql.Contains("FOR UPDATE"),"DOC111 document verification mutates or not bounded");if(origin=="DA")Check(sql.Contains("FROM `gab_fixture`")&&sql.Contains("g.ID=@documento")&&sql.Contains("g.ID_INVENTARIO_DOCUMENTAL=@inventario")&&sql.Contains("CAST(g.`CODIGO_DOC` AS CHAR) AS valor_0"),"DOC111 document verification ignores fixed IDs/index");return origin=="RA"?ra:da;};
            var result=(ResultadoEfectoImportacion)RepositoryHelper("EvidenciaInicialDocumento",command);result.DocumentoId="90";result.InventarioId="100";
            Func<bool> verify=()=>(bool)DocumentCatalogHelper("LeerAlmacenamiento",command,f.Data,result,query);
            Check(verify()&&reads==2,"DOC111 exact documentary relations not verified");
            da.Rows[0]["nombre"]=DBNull.Value;Check(!verify(),"Visible cabinet name can be absent");
            da.Rows[0]["nombre_visible"]="0";Check(verify(),"Invisible legacy cabinet name incorrectly blocks canonical inventory/index names");
            da.Rows[0]["nombre_configuraciones"]="0";Check(verify(),"Unconfigured physical name incorrectly required");
            da.Rows[0]["nombre"]="other.pdf";Check(!verify(),"Unexpected nonempty physical name accepted");
            da.Rows[0]["nombre"]="";foreach(var key in new[]{"segundo_nombre","nombre_indice"}){var saved=da.Rows[0][key];da.Rows[0][key]="other.pdf";Check(!verify(),"Invisible cabinet bypasses canonical original name: "+key);da.Rows[0][key]=saved;}
            da.Rows[0]["nombre_configuraciones"]="2";Check(!verify(),"Ambiguous physical-name metadata accepted");
            da.Rows[0]["nombre_configuraciones"]="1";da.Rows[0]["nombre_visible"]="-1";Check(!verify(),"NULL configured visibility silently treated as disabled");
            da.Rows[0]["nombre_configuraciones"]="1";da.Rows[0]["nombre_visible"]="1";da.Rows[0]["nombre"]=f.FileRecord.NombreOriginal;
            foreach(var table in new[]{ra,da})foreach(DataColumn column in table.Columns){var saved=table.Rows[0][column];table.Rows[0][column]="999";Check(!verify(),"DOC111 wrong documentary relation accepted: "+column.ColumnName);table.Rows[0][column]=saved;}
            da.ImportRow(da.Rows[0]);Check(!verify(),"DOC111 duplicate inventory/index relation accepted");da.Rows.RemoveAt(1);
            // Escala numerica fisica frente a texto con ceros y fechas DATE.
            var rule=f.Lot.Snapshot.Reglas.Single(x=>x.DestinoRef=="91");rule.TipoValor="DECIMAL";rule.ValorConstante="1.2";f.FileRecord.ValoresEfectivos[1].TipoValor="DECIMAL";f.FileRecord.ValoresEfectivos[1].Valor="1.2";f.Data.Campos[0].Valor="1.2";f.Rehash();command=f.Prepare();result.ValoresEfectivos=command.Archivo.ValoresEfectivos;da.Rows[0]["valor_0"]="1.20";Check(verify(),"DOC111 decimal physical scale causes false uncertainty");da.Rows[0]["valor_0"]="2.20";Check(!verify(),"DOC111 changed decimal accepted");
            rule.TipoValor="FECHA";rule.FormatoFecha="yyyy-MM-dd";rule.ValorConstante="2026-10-30";f.FileRecord.ValoresEfectivos[1].TipoValor="FECHA";f.FileRecord.ValoresEfectivos[1].Valor="2026-10-30";f.Data.Campos[0].Valor="2026-10-30";f.Rehash();command=f.Prepare();result.ValoresEfectivos=command.Archivo.ValoresEfectivos;da.Rows[0]["valor_0"]="2026-10-30";Check(verify(),"DOC111 canonical DATE not verified");
            var record=da.Rows[0].ItemArray;da.Clear();Check(!verify(),"DOC111 missing document accepted");da.Rows.Add(record);da.Columns.Remove("valor_0");Rejected("SOLICITUD_INVALIDA",()=>verify());
        }
        Console.WriteLine("DOC-111 documentary verification checks cabinet inventory incorporation names and fixed indices (synthetic SELECT): passed");
    }

    static void DocumentCatalogCases(){
        using(var f=new DocumentAdapterFixture()){
            var shared=new CreationCatalogFixture();shared.Names["GABINETE"]="gab_fixture";
            f.Lot.Snapshot.Configuracion.HuellaOrigenGabinete=shared.Source.Lot.Snapshot.Configuracion.HuellaOrigenGabinete;f.Rehash();
            bool duplicateClass=false,wrongClass=false,duplicateExtension=false,visibleName=true;string physical="CODIGO_DOC",formatCode="6",formatName=".PDF";int nameLength=255;
            Func<string,string,object[],DataTable> query=(origin,sql,args)=>{
                Check(sql.StartsWith("SELECT ")&&!sql.Contains("FOR UPDATE"),"DOC111 documentary catalog performs effects");
                if(sql.Contains("FROM tipo_doc_series")){Check(origin=="RA"&&sql.Contains("id_instrumento=@instrumento")&&sql.Contains("Estado_Tipo=1"),"DOC111 document type loses classification");return DocumentRow(new Dictionary<string,object>{{"id","9"},{"nombre","Contrato"}});}
                if(sql.Contains("FROM ra_tipo_documento")){Check(origin=="RA"&&sql.EndsWith("LIMIT 2")&&Convert.ToString(args[1])=="DOCUMENTO ELECTRONICO","DOC111 documentary class invented");var rows=DocumentRow(new Dictionary<string,object>{{"id","3"},{"nombre",wrongClass?"Other":"DOCUMENTO ELECTRONICO"}});if(duplicateClass)rows.ImportRow(rows.Rows[0]);return rows;}
                if(sql.Contains("FROM DA_EXTENSION")){Check(origin=="DA"&&sql.EndsWith("LIMIT 2")&&Convert.ToString(args[1])==".PDF","DOC111 PDF type lookup not scoped/bounded");var extensions=DocumentRow(new Dictionary<string,object>{{"id",formatCode},{"nombre",formatName}});if(duplicateExtension)extensions.ImportRow(extensions.Rows[0]);return extensions;}
                if(sql.Contains("ic.COLUMN_NAME='NOMBRE'")){Check(origin=="DA"&&sql.Contains("s.id_gabinete=@gabinete")&&sql.EndsWith("LIMIT 2"),"DOC111 automatic filename lookup is not cabinet scoped");return DocumentRow(new Dictionary<string,object>{{"DATA_TYPE","varchar"},{"CHARACTER_MAXIMUM_LENGTH",nameLength},{"CHARACTER_SET_NAME","utf8"}});}
                if(sql.Contains("INNER JOIN detalle_gabienete")){
                    Check(origin=="DA"&&sql.Contains("ALMACENA_IMAGEN=1")&&sql.EndsWith("LIMIT 129"),"DOC111 physical field mapping outside authorized cabinet");
                    var fields=DocumentRow(new Dictionary<string,object>{{"id","91"},{"CAMPO",physical},{"ALEAS_CAMPO","Etiqueta distinta del nombre fisico"},{"IS_REQUIRED_BACKEND","1"},{"DATA_TYPE","varchar"},{"COLUMN_TYPE","varchar(40)"},{"NUMERIC_PRECISION",DBNull.Value},{"NUMERIC_SCALE",DBNull.Value},{"CHARACTER_SET_NAME","utf8"},{"CHARACTER_MAXIMUM_LENGTH",40}});
                    if(visibleName){var name=fields.NewRow();name.ItemArray=fields.Rows[0].ItemArray;name["id"]="92";name["CAMPO"]="NOMBRE";name["CHARACTER_MAXIMUM_LENGTH"]=255;name["COLUMN_TYPE"]="varchar(255)";fields.Rows.Add(name);}return fields;
                }
                return shared.Query(origin,sql,args);
            };
            var catalog=new PerfilImportacionCatalogAdapter("RA","DA",query);var command=f.Prepare();var before=PerfilImportacionJson.Escribir(f.FileRecord.ValoresEfectivos);
            var data=catalog.PrepararDatosAlmacenamiento(command);Check(data.Campos.Single().Nombre=="CODIGO_DOC"&&data.Campos.Single().Valor=="0007"&&data.ClaseDocumentoId=="3"&&data.GabineteNombre=="gab_fixture"&&PerfilImportacionJson.Escribir(f.FileRecord.ValoresEfectivos)==before,"DOC111 mapping uses alias or mutates fixed values");
            visibleName=false;Check(catalog.PrepararDatosAlmacenamiento(command).Campos.Single().Nombre=="CODIGO_DOC","DOC111 requires system filename to be a visible user index");
            nameLength=1;Rejected("SOLICITUD_INVALIDA",()=>catalog.PrepararDatosAlmacenamiento(command));nameLength=255;visibleName=true;
            foreach(var code in new[]{"-2","0",Int32.MinValue.ToString(),Int32.MaxValue.ToString()}){formatCode=code;var native=catalog.PrepararDatosAlmacenamiento(command);Check(native.TipoArchivoId==code,"DOC111 replaces or rejects signed format code from PDF catalog");command.ValidarDatos(native);}
            foreach(var invalid in new[]{"-02","+2","2.0","2147483648","-2147483649"," 2","2 ",""}){formatCode=invalid;Rejected("CONFIGURACION_INVALIDA",()=>catalog.PrepararDatosAlmacenamiento(command));}formatCode="-2";
            duplicateExtension=true;Rejected("ACCESO_DENEGADO",()=>catalog.PrepararDatosAlmacenamiento(command));duplicateExtension=false;
            formatName=".TIF";Rejected("SOLICITUD_INVALIDA",()=>catalog.PrepararDatosAlmacenamiento(command));formatName=".PDF";
            duplicateClass=true;Rejected("ACCESO_DENEGADO",()=>catalog.PrepararDatosAlmacenamiento(command));duplicateClass=false;
            wrongClass=true;Rejected("SOLICITUD_INVALIDA",()=>catalog.PrepararDatosAlmacenamiento(command));wrongClass=false;
            physical="NOMBRE";Rejected("SOLICITUD_INVALIDA",()=>catalog.PrepararDatosAlmacenamiento(command));physical="ID_EXPEDIENTE";Rejected("CONFIGURACION_INVALIDA",()=>catalog.PrepararDatosAlmacenamiento(command));
            shared.Allowed=false;Rejected("ACCESO_DENEGADO",()=>catalog.PrepararDatosAlmacenamiento(command));
        }
        Console.WriteLine("DOC-111 documentary catalogs map physical indices and reject automatic field conflicts (synthetic SELECT): passed");
    }

    sealed class DeferredProviderEngine : IProveedorDatosIndices,IProveedorDatosIndicesOperativos {
        public int Calls,Authorizations;public string Value="0007",Code="OK",LastKey;
        public bool AutorizarOperativo(ContextoImportacionCarpetas c,ProveedorIndicesVersion v){Authorizations++;return true;}
        public ResultadoProveedorIndices ConsultarOperativo(ContextoImportacionCarpetas c,ProveedorIndicesVersion v,string key){Calls++;LastKey=key;return new ResultadoProveedorIndices{Codigo=Code,Valores=new Dictionary<string,string>{{"valor",Value}}};}
        public ResultadoProveedorIndices Consultar(ContextoPerfilImportacion c,ProveedorIndicesVersion v,string key){throw new Exception("Administrative provider invoked");}
    }
    sealed class ValuesFixture {
        public CreationCommandFixture Source=new CreationCommandFixture();
        public VistaValoresImportacion View;
        public DeferredProviderEngine Engine=new DeferredProviderEngine();
        public OperativeProviderPort Provider=new OperativeProviderPort();
        public OperativeCatalogPort Catalog=new OperativeCatalogPort();
        public int Writes,Events,Saves,Authorizations;public int FailAt;public Action OnAuthorize;
        public CoordinadorImportacionCarpetas Coordinator;
        public ValuesFixture(){
            Source.Root.Estado="EXPEDIENTE_CONFIRMADO";Source.Root.IntentoActivoId=null;Source.Root.NombreOriginal="original";
            Source.Root.IdentidadExpediente=new IdentidadExpedienteImportacion{OrigenLogico="RA",ExpedienteId="60",RelacionNivelId="70",NivelRef=Source.Context.NivelRef};
            var rules=Source.Lot.Snapshot.Reglas;
            // Este proveedor ya quedo confirmado en el preflight: no se vuelve a leer.
            rules.Add(new ReglaPerfilImportacion{DestinoTipo="INDICE_GABINETE",DestinoRef="90",FuenteTipo="PROVEEDOR",TipoValor="TEXTO",Proveedor=new FuenteProveedorIndices{ProveedorId="7",VersionId="8",Propiedad="valor",Clave=new ClaveProveedorIndices{FuenteTipo="CONSTANTE",ValorConstante="old"}}});
            rules.Add(new ReglaPerfilImportacion{DestinoTipo="INDICE_GABINETE",DestinoRef="91",FuenteTipo="EXPEDIENTE",PropiedadExpediente="CODIGO",EtapaResolucion="TRAS_EXPEDIENTE"});
            rules.Add(new ReglaPerfilImportacion{DestinoTipo="INDICE_GABINETE",DestinoRef="92",FuenteTipo="PROVEEDOR",EtapaResolucion="TRAS_EXPEDIENTE",Proveedor=new FuenteProveedorIndices{ProveedorId="7",VersionId="8",Propiedad="valor",Clave=new ClaveProveedorIndices{FuenteTipo="EXPEDIENTE",PropiedadExpediente="ID"}}});
            Source.Rehash();
            var file=new ArchivoImportacionCarpetas{ArchivoId="30",RaizId="40",ClienteArchivoId="a1",NombreOriginal="original.pdf",EstadoRecepcion="RECIBIDO",EstadoEfecto="NO_INICIADO",Revision=3,BytesPrevistos="7",BytesRecibidos="7",ContenidoHash=new string('a',64),StagingRef=new string('b',32),ValoresPlan=new List<ValorReglaPerfilImportacion>{
                new ValorReglaPerfilImportacion{DestinoTipo="TIPOLOGIA",DestinoRef="TIPO_DOCUMENTAL",FuenteTipo="CONSTANTE",TipoValor="REFERENCIA",Valor="9"},
                new ValorReglaPerfilImportacion{DestinoTipo="INDICE_GABINETE",DestinoRef="90",FuenteTipo="PROVEEDOR",TipoValor="TEXTO",Valor="preflight-0001",Procedencia="7:8:valor"},
                new ValorReglaPerfilImportacion{DestinoTipo="INDICE_GABINETE",DestinoRef="91",FuenteTipo="EXPEDIENTE",TipoValor="TEXTO",Diferido=true,PropiedadPendiente="CODIGO"},
                new ValorReglaPerfilImportacion{DestinoTipo="INDICE_GABINETE",DestinoRef="92",FuenteTipo="PROVEEDOR",TipoValor="TEXTO",Diferido=true,PropiedadPendiente="ID",Procedencia="7:8:valor"}
            }};
            View=new VistaValoresImportacion{Lote=Source.Lot,Raiz=Source.Root,Archivo=file};
            Provider.Value=new ProveedorIndicesVersion{ProveedorId="7",VersionId="8",AmbitoRef=Source.Context.AmbitoRef,Activo=true,Estado="PUBLICADA",TipoClave="TEXTO",Propiedades=new List<PropiedadProveedorIndices>{new PropiedadProveedorIndices{Nombre="valor",Tipo="TEXTO"}}};
            var owner=new ControlRepository();owner.Lot=Source.Lot;
            Coordinator=new CoordinadorImportacionCarpetas(()=>ReceiptClone(Source.Context),owner,level=>ReceiptClone(Source.Context),(c,config)=>{Authorizations++;if(OnAuthorize!=null)OnAuthorize();},
                (c,l,a)=>ReceiptClone(View),new CoordinadorProveedorIndices(Provider,Engine),Catalog,(c,expected,result)=>Save(c,expected,result));
        }
        public ArchivoImportacionCarpetas Save(ContextoImportacionCarpetas c,VistaValoresImportacion expected,ResultadoEnsayoPerfilImportacion result){
            Saves++;
            var file=(ArchivoImportacionCarpetas)RepositoryHelper("PersistirValoresArchivo",c,expected,View,result,DateTime.UtcNow,
                new Func<string,object[],int>((sql,args)=>{Writes++;if(FailAt==Writes)return 0;Check(!sql.Contains("expediente_archivo")&&!sql.Contains("ra_integracion_raiz SET"),"Deferred checkpoint changed created expedition");Check(sql.Contains("revision=@revision"),"Deferred checkpoint missing CAS");return 1;}),new Action<string>(code=>Events++));
            View.Archivo=ReceiptClone(file);View.Lote.Revision++;return file;
        }
        public ArchivoImportacionCarpetas Run(){return Coordinator.MaterializarArchivo("20","30",3);}
    }
    static void DeferredValuesCases(){
        var f=new ValuesFixture();var originalView=ReceiptClone(f.View);var root=PerfilImportacionJson.Escribir(f.View.Raiz);var plan=PerfilImportacionJson.Escribir(f.View.Archivo.ValoresPlan);
        f.Catalog.ValidateValues=values=>{Check(values.Count==2&&values.All(v=>v.DestinoRef=="91"||v.DestinoRef=="92"),"Previously resolved values revalidated as external codes");return new List<IncidenciaPerfilImportacion>();};
        var result=f.Run();
        Check(result.ValoresEfectivosFijados&&result.ValoresEfectivos.All(v=>!v.Diferido)&&f.Engine.Calls==1&&f.Engine.LastKey=="60"&&f.Writes==2&&f.Events==1,"Deferred provider not resolved once with confirmed expedition ID");
        Check(result.ValoresEfectivos.Single(v=>v.DestinoRef=="91").Valor=="000123"&&result.ValoresEfectivos.Single(v=>v.DestinoRef=="92").Valor=="0007"&&result.ValoresEfectivos.Single(v=>v.DestinoRef=="92").Procedencia=="7:8:valor","Deferred values lose zeros or provenance");
        Check(PerfilImportacionJson.Escribir(f.View.Raiz)==root&&PerfilImportacionJson.Escribir(f.View.Archivo.ValoresPlan)==plan&&result.EstadoEfecto=="NO_INICIADO","Materialization mutates creation, plan or documentary completion");
        var reads=f.Provider.Reads;var auth=f.Engine.Authorizations;f.Engine.Value="changed";
        var replay=f.Run();Check(f.Engine.Calls==1&&f.Provider.Reads==reads&&f.Engine.Authorizations==auth&&f.Saves==1&&f.Writes==2&&replay.ValoresEfectivos.Single(v=>v.DestinoRef=="92").Valor=="0007","Retry reconsults provider or silently recalculates confirmed values");
        var checkpoint=new ResultadoEnsayoPerfilImportacion{Valores=ReceiptClone(result.ValoresEfectivos)};
        Action sqlReplay=()=>RepositoryHelper("PersistirValoresArchivo",f.Source.Context,originalView,f.View,checkpoint,DateTime.UtcNow,new Func<string,object[],int>((sql,args)=>{throw new Exception("Replay wrote SQL");}),new Action<string>(code=>{throw new Exception("Replay wrote event");}));
        sqlReplay();checkpoint.Valores[3].Valor="changed";Rejected("CONFLICTO_REVISION",sqlReplay);
        replay.ValoresEfectivos.Clear();Check(f.View.Archivo.ValoresEfectivos.Count==4,"Replay leaks mutable persisted values");
        f=new ValuesFixture();f.Catalog.ValidateValues=values=>new List<IncidenciaPerfilImportacion>{new IncidenciaPerfilImportacion("INDICE_GABINETE:92","VALOR_INDICE_INVALIDO","fixture")};
        result=f.Run();Check(!result.ValoresEfectivosFijados&&result.ValoresEfectivos.Count==0&&result.EstadoEfecto=="RECHAZADO"&&result.Resultado.Codigo=="VALORES_INCOMPATIBLES"&&f.View.Raiz.Estado=="EXPEDIENTE_CONFIRMADO"&&f.View.Raiz.IdentidadExpediente.ExpedienteId=="60","Incompatible deferred value loses expedition or allows completion");
        Rejected("CONFLICTO_REVISION",()=>f.Run());Check(f.Engine.Calls==1,"Rejected unit automatically queries provider again");
        f=new ValuesFixture();f.Engine.Code="SIN_COINCIDENCIA";result=f.Run();Check(result.EstadoEfecto=="RECHAZADO"&&!result.ValoresEfectivosFijados,"Missing provider match permits storage");
        f=new ValuesFixture();f.View.Lote.Snapshot.Reglas.Single(r=>r.DestinoRef=="91").LongitudMaxima=2;f.Source.Rehash();result=f.Run();Check(result.EstadoEfecto=="RECHAZADO"&&f.View.Raiz.IdentidadExpediente.ExpedienteId=="60","Deferred conversion failure loses created expedition");
        foreach(var mutate in new Action<ValuesFixture>[] {x=>x.View.Raiz.Estado="CREANDO",x=>x.View.Raiz.IdentidadExpediente=null,x=>x.View.Archivo.ValoresPlan.Add(ReceiptClone(x.View.Archivo.ValoresPlan[0])),x=>x.View.Archivo.ValoresPlan.RemoveAt(0),x=>x.View.Archivo.ValoresPlan[3].PropiedadPendiente="CODIGO",x=>x.View.Archivo.ValoresPlan[3].Procedencia="7:9:valor",x=>x.View.Lote.DetencionSolicitada=true,x=>x.View.Archivo.Revision++,x=>x.View.Lote.Snapshot.Reglas[2].LongitudMaxima++}){
            f=new ValuesFixture();mutate(f);Rejected("CONFLICTO_REVISION",()=>f.Run());Check(f.Engine.Calls==0&&f.Writes==0,"Invalid/stale deferred view reaches provider or checkpoint");
        }
        f=new ValuesFixture();f.OnAuthorize=()=>{if(f.Authorizations==2)throw new PerfilImportacionException("ACCESO_DENEGADO","fixture");};Rejected("ACCESO_DENEGADO",()=>f.Run());Check(f.Writes==0,"Revocation before checkpoint ignored");
        foreach(var failAt in new[]{1,2}){f=new ValuesFixture();f.FailAt=failAt;Rejected("CONFLICTO_REVISION",()=>f.Run());Check(f.Events==0&&!f.View.Archivo.ValoresEfectivosFijados,"Failed CAS publishes fixed values");}
        // SQL NULL frente a []: incluso sin reglas documentales, [] se fija una sola vez.
        f=new ValuesFixture();f.View.Lote.Snapshot.Reglas.RemoveAll(r=>r.DestinoTipo!="CAMPO_EXPEDIENTE");f.Source.Rehash();f.View.Archivo.ValoresPlan.Clear();result=f.Run();f.Run();Check(result.ValoresEfectivosFijados&&result.ValoresEfectivos.Count==0&&f.Saves==1&&f.Engine.Calls==0&&f.Provider.Reads==0,"Empty effective checkpoint confused with unresolved SQL NULL");
        Console.WriteLine("DOC-111 deferred values fixed once with provenance and preserved expedition (provider and SQL doubles): passed");
        DocumentAdapterCases();DocumentVerificationCases();DocumentCatalogCases();DocumentPhaseCases();DocumentGlobalBarrierCases();ConsolidationCases();ReconciliationCases();OperationHttpCases();CleanupCases();
    }

    sealed class OperativeProviderPort : IProveedorIndicesRepository,IProveedorIndicesLecturaOperativa {
        public ProveedorIndicesVersion Value;public int Reads;
        public ProveedorIndicesVersion ObtenerPublicacion(ContextoImportacionCarpetas context,string provider,string version){Reads++;Check(context.PuedeImportar && provider=="7" && version=="8","DOC111 provider version substituted");return Value;}
        public ProveedorIndicesVersion Obtener(ContextoPerfilImportacion context,string provider,string version){throw new Exception("DOC111 administrative provider getter called from operative path");}
        public List<OpcionProveedorIndices> Listar(ContextoPerfilImportacion context,int page,int size,string provider,string version){throw new Exception("DOC111 administrative provider list called from operative path");}
    }

    public static void RunOperativePublicationReadChecks(string connection) {
        var context=new ContextoImportacionCarpetas{UsuarioRef="141",AmbitoRef="2",NivelRef="7",RutaCanonica="Repository read fixture",SelloDestino="fixture",PuedeImportar=true};
        var repo=new MySqlPerfilImportacionRepository(connection);repo.VerificarEsquema();
        // Fixture de autoridad del repositorio; no se concede permiso ni se
        // autentica una cuenta HTTP. El perfil 24 debe seguir inactivo.
        Rejected("ACCESO_DENEGADO",()=>repo.ObtenerPublicacion(context,"38"));
        Rejected("ACCESO_DENEGADO",()=>repo.ObtenerPublicacion(context,"4294967295"));
        Check(repo.ListarPublicaciones(context,1,50).All(p=>p.PerfilId!="24"),"DOC111 inactive profile in operative list");
        var denied=new ContextoImportacionCarpetas{UsuarioRef="141",AmbitoRef="2",PuedeImportar=false};
        Rejected("ACCESO_DENEGADO",()=>repo.ObtenerPublicacion(denied,"38"));
        Console.WriteLine("DOC111_REAL_OPERATIVE_READ_DENIES_INACTIVE_OR_UNKNOWN_PROFILE");
    }

    public static string[] RunPublicationIntegration(bool authorized,string connection,ContextoImportacionCarpetas context) {
        if(!authorized)throw new Exception("DOC111_PUBLICATION_READ_AUTHORIZATION_REQUIRED");
        var repository=new MySqlPerfilImportacionRepository(connection);
        IPerfilImportacionLecturaOperativa read=repository;read.VerificarEsquema();
        var current=read.ObtenerPublicacion(context,"38");var previous=read.ObtenerPublicacion(context,"37");
        Check(current.PerfilId=="24" && current.VersionId=="38" && current.NumeroVersion==2,"DOC111 current publication identity wrong");
        Check(previous.PerfilId=="24" && previous.VersionId=="37" && previous.NumeroVersion==1,"DOC111 historical publication silently replaced");
        Check(read.ListarPublicaciones(context,1,50).Any(p=>p.PerfilId=="24" && p.VersionId=="38"),"DOC111 operative list omits current publication");
        var fingerprint=current.Huella;current.Configuracion.NombreVisible="local-mutation-only";
        var restored=read.ObtenerPublicacion(context,"38");
        Check(restored.Huella==fingerprint && restored.Configuracion.NombreVisible!="local-mutation-only","DOC111 caller mutates publication source");
        Rejected("ACCESO_DENEGADO",()=>read.ObtenerPublicacion(context,"4294967295"));
        Rejected("ACCESO_DENEGADO",()=>repository.Obtener(new ContextoPerfilImportacion{UsuarioRef=context.UsuarioRef,AmbitoRef=context.AmbitoRef,PuedeAdministrar=false},"24","38"));
        Console.WriteLine("DOC111_REAL_EXPLICIT_PUBLICATIONS_READ_WITHOUT_ADMIN_ESCALATION");
        return new[]{current.VersionId,previous.VersionId};
    }

    static void ManifestCases() {
        var original=PlanFixture().Manifiesto;
        foreach(var file in original.Archivos)file.UltimaModificacionLocal="0";
        var limits=new LimitesImportacionCarpetas(2,2,7,14,8192,TimeSpan.FromMinutes(1),TimeSpan.FromHours(1));
        var canonical=CanonizadorManifiestoImportacion.Canonizar(original,limits);
        Check(canonical.Raices.Count==2 && canonical.Archivos.Count==2 && canonical.Raices[0].IdCliente=="r1","DOC111 homonymous roots collapsed or unordered");
        canonical.Raices[0].Nombre="mutation";
        Check(original.Raices.All(r=>r.Nombre=="Órdenes"),"DOC111 canonical manifest aliases caller");
        Func<ManifiestoImportacionCarpetas> copy=()=>PerfilImportacionJson.Leer<ManifiestoImportacionCarpetas>(PerfilImportacionJson.Escribir(original));
        Action<Action<ManifiestoImportacionCarpetas>> reject=change=>{var broken=copy();change(broken);Rejected("SOLICITUD_INVALIDA",()=>CanonizadorManifiestoImportacion.Canonizar(broken,limits));};
        foreach(var path in new[]{"/Órdenes/a.pdf","C:/a.pdf","Órdenes/../000123.pdf","Órdenes/./000123.pdf","Órdenes//000123.pdf","Órdenes\\000123.pdf","000123.pdf","Otra/000123.pdf","Órdenes/otro.pdf"})
            reject(m=>m.Archivos[0].RutaRelativa=path);
        reject(m=>m.Archivos[0].RutaRelativa="órdenes/000123.pdf");
        reject(m=>m.Archivos[0].RaizIdCliente="missing");
        reject(m=>m.Raices[1].SeleccionId=m.Raices[0].SeleccionId);
        reject(m=>m.Archivos.RemoveAt(0));
        reject(m=>m.Archivos[0].Bytes="07");
        reject(m=>m.Archivos[0].UltimaModificacionLocal=null);
        reject(m=>m.Archivos[0].IdCliente="id:ambiguous");
        var collision=copy(); collision.Archivos[1].RaizIdCliente=collision.Archivos[0].RaizIdCliente;collision.Archivos[1].IdCliente="a2";
        collision.Archivos[0].Nombre="é.pdf";collision.Archivos[0].RutaRelativa="Órdenes/é.pdf";
        collision.Archivos[1].Nombre="e\u0301.pdf";collision.Archivos[1].RutaRelativa="Órdenes/e\u0301.pdf";
        Rejected("SOLICITUD_INVALIDA",()=>CanonizadorManifiestoImportacion.Canonizar(collision,limits));
        var ancestor=copy();ancestor.Archivos[1].RaizIdCliente=ancestor.Archivos[0].RaizIdCliente;ancestor.Archivos[1].IdCliente="a2";
        ancestor.Archivos[1].RutaRelativa="Órdenes/000123.pdf/000123.pdf";ancestor.Raices.RemoveAt(1);
        Rejected("SOLICITUD_INVALIDA",()=>CanonizadorManifiestoImportacion.Canonizar(ancestor,limits));
        var huge=copy();huge.Archivos[0].Bytes=UInt64.MaxValue.ToString();huge.Archivos[1].Bytes="1";
        var max=new LimitesImportacionCarpetas(2,2,UInt64.MaxValue,UInt64.MaxValue,8192,TimeSpan.FromMinutes(1),TimeSpan.FromHours(1));
        Rejected("LIMITE_EXCEDIDO",()=>CanonizadorManifiestoImportacion.Canonizar(huge,max));
        foreach(var quota in new[]{
            new LimitesImportacionCarpetas(1,2,7,14,8192,TimeSpan.FromMinutes(1),TimeSpan.FromHours(1)),
            new LimitesImportacionCarpetas(2,1,7,14,8192,TimeSpan.FromMinutes(1),TimeSpan.FromHours(1)),
            new LimitesImportacionCarpetas(2,2,6,14,8192,TimeSpan.FromMinutes(1),TimeSpan.FromHours(1)),
            new LimitesImportacionCarpetas(2,2,7,13,8192,TimeSpan.FromMinutes(1),TimeSpan.FromHours(1)),
            new LimitesImportacionCarpetas(2,2,7,14,1,TimeSpan.FromMinutes(1),TimeSpan.FromHours(1))})
            Rejected("LIMITE_EXCEDIDO",()=>CanonizadorManifiestoImportacion.Canonizar(original,quota));
        Unavailable(()=>CanonizadorManifiestoImportacion.Canonizar(original,null));
        var nested=copy();nested.Archivos[0].RutaRelativa="Órdenes/Sub/000123.pdf";
        var grouping=new ConfiguracionPerfilImportacion {EstrategiaCarpetas="RAIZ_EXPEDIENTE",PoliticaSubcarpetas="INCLUIR"};
        Check(CanonizadorManifiestoImportacion.EvaluarAgrupacion(nested,grouping).Count==0,"DOC111 allowed subfolder rejected");
        grouping.PoliticaSubcarpetas="RECHAZAR";
        var issues=CanonizadorManifiestoImportacion.EvaluarAgrupacion(nested,grouping);
        Check(issues.Count==1 && issues[0].RaizIdCliente==nested.Archivos[0].RaizIdCliente && issues[0].ArchivoIdCliente=="a1" && issues[0].BloqueaUnidad,"DOC111 subfolder rejection loses element identity");
        grouping.EstrategiaCarpetas="CADA_SUBCARPETA";
        Rejected("CONFIGURACION_INVALIDA",()=>CanonizadorManifiestoImportacion.EvaluarAgrupacion(nested,grouping));
        Check(nested.Raices.Count==2 && nested.Archivos.Count==2,"DOC111 grouping manufactured or discarded inventory");
        Console.WriteLine("DOC-111 manifest canonicalization and quotas: passed");
    }

    public static void Run() {
        DocumentProtocolV2Cases();
        StagingV2Cases();
        AdapterV2Cases();
        PersistenceV2Cases();
        RecoveryV2Cases();
        RecoveryHttpV2Cases();
        Doc115RepositoryGuardCases();
        QueryCases();QuerySqlCases();QueryDispatcherCases();RetentionDiagnosisCases();KnownExpedientRelationCases();KnownExpedientClassificationCases();CreationCommandCases();CreationCatalogCases();CreationLegacyCompatibilityCases();LegacyCreationAuthorityCases();ProductionArchiveCycleCases();CreationAdapterCases();CreationResultSqlCases();CreationCheckpointCases();CreationBarrierCases();PhysicalCreationStagingCases();CoordinatedCreationCases();DeferredValuesCases();
        ControlCases();
        TransitionCases();
        ReceiptRecordCases();
        ReceiptHandlerCases();
        ReceiptServiceCases();
        StagingCases();
        ManifestCases();
        SchemaGuards();
        PlanStorageCases();
        CoincidenceReadCases();
        ConfirmationServiceCases();
        ContextAuthorityCases();
        ResourceAuthorizationCases();
        RequestGuardCases();
        ImportAdminMatrixCases();
        PublishedReadCases();
        OperativeCatalogCases();
        ProfileSelectionCases();
        OpeningCases();
        PreflightCalculationCases();
        OperationalDispatcherCases();
        ConfiguredLimitsCases();
        OperativeProviderCases();
        // Valores de prueba, nunca valores predeterminados de operación.
        var valid=new LimitesImportacionCarpetas(2,4,UInt64.MaxValue,UInt64.MaxValue,1024,TimeSpan.FromMinutes(1),TimeSpan.FromHours(1));
        LimitesImportacionCarpetas.Exigir(valid);
        Check(valid.MaximoBytesArchivo==UInt64.MaxValue,"DOC111 policy truncates byte limits");
        Unavailable(()=>LimitesImportacionCarpetas.Exigir(null));
        Unavailable(()=>new LimitesImportacionCarpetas(0,4,1,2,1024,TimeSpan.FromMinutes(1),TimeSpan.FromHours(1)));
        Unavailable(()=>new LimitesImportacionCarpetas(2,-1,1,2,1024,TimeSpan.FromMinutes(1),TimeSpan.FromHours(1)));
        Unavailable(()=>new LimitesImportacionCarpetas(2,4,0,2,1024,TimeSpan.FromMinutes(1),TimeSpan.FromHours(1)));
        Unavailable(()=>new LimitesImportacionCarpetas(2,4,1,0,1024,TimeSpan.FromMinutes(1),TimeSpan.FromHours(1)));
        Unavailable(()=>new LimitesImportacionCarpetas(2,4,1,2,0,TimeSpan.FromMinutes(1),TimeSpan.FromHours(1)));
        Unavailable(()=>new LimitesImportacionCarpetas(2,4,1,2,1024,TimeSpan.Zero,TimeSpan.FromHours(1)));
        Unavailable(()=>new LimitesImportacionCarpetas(2,4,1,2,1024,TimeSpan.FromMinutes(1),TimeSpan.FromSeconds(-1)));

        var plan=new PlanImportacionCarpetas {
            PlanId="4294967295",Revision=1,EsquemaDoc=1,Estado="REVISABLE",
            Contexto=new ContextoImportacionCarpetas {UsuarioRef="1",AmbitoRef="2",NivelRef="3",PuedeImportar=true},
            Snapshot=new SnapshotPerfilImportacionOperativa {
                PerfilId="4",VersionId="5",NumeroVersion=1,
                Configuracion=new ConfiguracionPerfilImportacion {ModoIntegridad="COMPLETO_OBLIGATORIO",AmbitoIntegridad="LOTE"},
                Reglas=new List<ReglaPerfilImportacion> {new ReglaPerfilImportacion {ReglaId="6",DestinoTipo="INDICE_GABINETE",DestinoRef="7",ValorConstante="000123"}}
            },
            Manifiesto=new ManifiestoImportacionCarpetas(),
            Resultado=new ResultadoPreparacionImportacion()
        };
        plan.Manifiesto.Raices.Add(new RaizManifiestoImportacion {SeleccionId="s1",IdCliente="r1",Nombre="Órdenes"});
        plan.Manifiesto.Raices.Add(new RaizManifiestoImportacion {SeleccionId="s2",IdCliente="r2",Nombre="Órdenes"});
        plan.Manifiesto.Archivos.Add(new ArchivoManifiestoImportacion {IdCliente="a1",RaizIdCliente="r1",Nombre="000123.pdf",RutaRelativa="Órdenes/000123.pdf",Bytes="18446744073709551615"});
        var deferred=new ValoresElementoImportacion {Ambito="ARCHIVO",RaizIdCliente="r1",ArchivoIdCliente="a1"};
        deferred.Valores.Add(new ValorReglaPerfilImportacion {Diferido=true,PropiedadPendiente="ID",TipoValor="ENTERO",Valor=null});
        plan.Resultado.Elementos.Add(deferred);
        var serialized=PerfilImportacionJson.Escribir(plan);
        Check(!serialized.Contains("Ó"),"DOC111 snapshot is not ASCII escaped");
        var restored=PerfilImportacionJson.Leer<PlanImportacionCarpetas>(serialized);
        Check(restored.Manifiesto.Archivos[0].Bytes=="18446744073709551615","DOC111 bytes lost precision");
        Check(restored.Manifiesto.Raices.Count==2 && restored.Manifiesto.Raices[0].IdCliente!=restored.Manifiesto.Raices[1].IdCliente,"DOC111 homonymous roots merged");
        Check(restored.Resultado.Elementos[0].Valores[0].Diferido && restored.Resultado.Elementos[0].Valores[0].Valor==null,"DOC111 deferred value invented an ID");
        Check(!restored.Resultado.PuedeConfirmar,"DOC111 model authorizes confirmation by default");
        plan.Snapshot.Reglas[0].ValorConstante="changed";
        Check(restored.Snapshot.Reglas[0].ValorConstante=="000123","DOC111 persisted snapshot tracks mutable source");

        var request=PerfilImportacionJson.Leer<PrepararPlanImportacionSolicitud>("{\"nivelRef\":\"3\",\"versionId\":\"5\",\"raices\":[{\"idCliente\":\"r1\",\"nombre\":\"Ordenes\"}],\"archivos\":[{\"idCliente\":\"a1\",\"raizIdCliente\":\"r1\",\"bytes\":\"18446744073709551615\"}]}");
        Check(request.archivos[0].bytes=="18446744073709551615","DOC111 request coerces byte strings");
        InvalidJson<PrepararPlanImportacionSolicitud>("{\"nivelRef\":\"3\",\"usuarioRef\":\"99\"}");
        InvalidJson<PrepararPlanImportacionSolicitud>("{\"archivos\":[{\"bytes\":9007199254740993}]}");
        InvalidJson<ConfirmarPlanImportacionSolicitud>("{\"planId\":\"1\",\"revisionEsperada\":\"1\"}");
        InvalidJson<ConfirmarPlanImportacionSolicitud>("{\"planId\":\"1\",\"snapshot\":{}}");
        InvalidJson<RecibirArchivoImportacionSolicitud>("{\"loteId\":\"1\",\"rutaFisica\":\"server-file\"}");
        InvalidJson<AbrirImportacionSolicitud>("{\"nivelRef\":\"1\",\"nivelRef\":\"2\"}");
        Check(EstadosImportacionCarpetas.EsRecepcion("RECIBIDO") && !EstadosImportacionCarpetas.EsEfecto("RECIBIDO"),"DOC111 reception confused with document effect");
        Check(EstadosImportacionCarpetas.EsEfecto("COMPLETADO") && !EstadosImportacionCarpetas.EsRecepcion("COMPLETADO"),"DOC111 document completion confused with transfer");
        Check(!EstadosImportacionCarpetas.EsPlan(null) && !EstadosImportacionCarpetas.EsPlan("revisable") && !EstadosImportacionCarpetas.EsPlan("REVISABLE "),"DOC111 state aliases accepted");
        Check(EstadosImportacionCarpetas.EsRaiz("EXPEDIENTE_CONFIRMADO") && EstadosImportacionCarpetas.EsIntento("INCIERTO") && EstadosImportacionCarpetas.EsLote("PENDIENTE_VERIFICACION"),"DOC111 recovery dimensions missing");
        Console.WriteLine("DOC-111 contracts and limits: passed");
    }
}
