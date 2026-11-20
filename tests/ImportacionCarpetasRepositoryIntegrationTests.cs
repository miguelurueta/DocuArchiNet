using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

// Direct repository integration only. Not a web endpoint, not called by Run().
// The existing Framework harness compiles this partial; the authorized runner
// supplies the real context/publication and retains every created resource.
public static partial class ImportacionCarpetasBehaviorTests {
    static void Doc115RepositoryGuardCases() {
        try{RunDoc115RepositoryIntegration(false,"PRIVATE_CONNECTION_SENTINEL",null,null,null,null,null,null);throw new Exception("guard missing");}catch(Exception e){Check(e.Message=="DOC115_REPOSITORY_AUTHORIZATION_REQUIRED","DOC115 integration opens database without authorization");}
        try{RunDoc115RepositoryIntegration(true,"PRIVATE_CONNECTION_SENTINEL",null,null,null,null,null,null);throw new Exception("scope missing");}catch(Exception e){Check(e.Message=="DOC115_REPOSITORY_SCOPE_INVALID","DOC115 integration opens database without scoped fixture");}
        Console.WriteLine("DOC-115 repository integration authorization guard (no database): passed");
        Doc120FileRepositoryGuards();
    }
    sealed class Doc115Confirmation {
        public LoteImportacionCarpetas Lot;
        public string Code;
    }
    static T[] Doc115Pair<T>(Func<T> left,Func<T> right) {
        using(var gate=new Barrier(2)) {
            Func<Func<T>,Task<T>> start=f=>Task.Factory.StartNew(()=>{Check(gate.SignalAndWait(TimeSpan.FromSeconds(20)),"DOC115_PAIR_GATE_TIMEOUT");return f();});
            var a=start(left);var b=start(right);Task.WaitAll(a,b);return new[]{a.Result,b.Result};
        }
    }
    static Doc115Confirmation Doc115Confirm(string connection,ContextoImportacionCarpetas context,PlanImportacionCarpetas plan,string key,DateTime clock) {
        try{return new Doc115Confirmation{Lot=new MySqlImportacionCarpetasRepository(connection,()=>clock).ConfirmarPlan(context,plan.PlanId,plan.Revision,plan.Huella,key)};}
        catch(PerfilImportacionException e){if(e.Codigo!="CONFLICTO_REVISION")throw;return new Doc115Confirmation{Code=e.Codigo};}
    }
    static MySqlImportacionCarpetasRepository Doc115Observed(string connection,DateTime clock,Action<string,MySqlConnection,MySqlTransaction> probe) {
        var constructor=typeof(MySqlImportacionCarpetasRepository).GetConstructor(BindingFlags.NonPublic|BindingFlags.Instance,null,new[]{typeof(string),typeof(Func<DateTime>),typeof(Action<string,MySqlConnection,MySqlTransaction>)},null);
        Check(constructor!=null,"DOC115_OBSERVER_UNAVAILABLE");
        return (MySqlImportacionCarpetasRepository)constructor.Invoke(new object[]{connection,new Func<DateTime>(()=>clock),probe});
    }

    // Uses two already received, pending E2E resources. Only the native control
    // repository is invoked: no storage adapter, original removal or SQL DML
    // from the test. A stopped run is retained for review, never auto-replayed.
    sealed class Doc120FileControls {
        readonly string connection;
        readonly System.Diagnostics.Stopwatch time=System.Diagnostics.Stopwatch.StartNew();
        public int Selects;
        public Doc120FileControls(string value){connection=value;}
        public string Read(string sql,MySqlConnection db=null,MySqlTransaction tx=null){
            Check(sql.StartsWith("SELECT ",StringComparison.Ordinal)&&!sql.Contains(";"),"DOC120_FILES_SELECT_REQUIRED");
            Check(time.Elapsed<TimeSpan.FromMinutes(15)&&Interlocked.Increment(ref Selects)<=64,"DOC120_FILES_SELECT_BUDGET");
            if(db==null)using(var c=new MySqlConnection(connection)){c.Open();return Execute(sql,c,null);}
            return Execute(sql,db,tx);
        }
        static string Execute(string sql,MySqlConnection db,MySqlTransaction tx){
            using(var cmd=new MySqlCommand(sql,db,tx)){
                cmd.CommandTimeout=10;var rows=new List<object[]>();
                using(var r=cmd.ExecuteReader())while(r.Read()){var values=new object[r.FieldCount];r.GetValues(values);rows.Add(values);}
                return Newtonsoft.Json.JsonConvert.SerializeObject(rows);
            }
        }
        public string State(){
            var hashes=new List<string>();
            foreach(var query in new[]{"SELECT * FROM ra_integracion_lote WHERE lote_id=50", "SELECT * FROM ra_integracion_raiz WHERE lote_id=50 ORDER BY raiz_id", "SELECT a.* FROM ra_integracion_archivo a INNER JOIN ra_integracion_raiz r ON r.raiz_id=a.raiz_id WHERE r.lote_id=50 ORDER BY a.archivo_id", "SELECT * FROM ra_integracion_intento WHERE lote_id=50 ORDER BY intento_id", "SELECT * FROM ra_integracion_evento WHERE lote_id=50 ORDER BY evento_id"})
                hashes.Add(Doc120DatabaseHash("state",Read(query)));
            return String.Join("|",hashes);
        }
    }
    static void Doc120FileScope(ContextoImportacionCarpetas context,string run,string environment,string lot,string[] files,Action<string,string> reference){
        if(context==null||context.UsuarioRef!="141"||context.AmbitoRef!="2"||context.NivelRef!="223"||!context.PuedeImportar||
           run==null||!System.Text.RegularExpressions.Regex.IsMatch(run,@"\A[a-f0-9]{32}\z")||environment==null||!System.Text.RegularExpressions.Regex.IsMatch(environment,@"\A[a-f0-9]{64}\z")||
           lot!="50"||files==null||!files.SequenceEqual(new[]{"122","123"})||reference==null)throw new Exception("DOC120_FILES_SCOPE_INVALID");
    }
    static void Doc120FileRepositoryGuards(){
        try{RunDoc120FileRepositoryIntegration(false,"PRIVATE",null,null,null,null,null,null);throw new Exception("guard");}
        catch(Exception e){Check(e.Message=="DOC120_FILES_AUTHORIZATION_REQUIRED","File integration authorization guard");}
        try{RunDoc120FileRepositoryIntegration(true,"PRIVATE",null,null,null,null,null,null);throw new Exception("guard");}
        catch(Exception e){Check(e.Message=="DOC120_FILES_SCOPE_INVALID","File integration scope guard");}
        var c=new ContextoImportacionCarpetas{UsuarioRef="141",AmbitoRef="2",NivelRef="223",PuedeImportar=true};
        foreach(var files in new[]{new[]{"122","124"},new[]{"123","122"},new[]{"122"}}){
            try{Doc120FileScope(c,new string('a',32),new string('b',64),"50",files,(k,id)=>{});throw new Exception("guard");}
            catch(Exception e){Check(e.Message=="DOC120_FILES_SCOPE_INVALID","File integration broadens resources");}
        }
        try{RunDoc120FileRepositoryIntegration(true,"PRIVATE_OPTION=PRIVATE",c,new string('a',32),new string('b',64),"50",new[]{"122","123"},(k,id)=>{});throw new Exception("guard");}
        catch(Exception e){Check(e.Message=="DOC120_FILES_CONNECTION_INVALID","File integration parses connections safely");}
        try{RunDoc120FileRepositoryIntegration(true,"Server=127.0.0.1;Database=doc120_lab;User ID=PRIVATE",c,new string('a',32),new string('b',64),"50",new[]{"122","123"},(k,id)=>{});throw new Exception("guard");}
        catch(Exception e){Check(e.Message=="DOC120_FILES_ENVIRONMENT_CHANGED","File integration checks target before connecting");}
        Console.WriteLine("DOC-120 native file reservation guards: passed (no database)");
    }
    public static Dictionary<string,object> RunDoc120FileRepositoryIntegration(bool authorized,string connection,
        ContextoImportacionCarpetas context,string runId,string environmentHash,string lotId,string[] files,Action<string,string> reference){
        if(!authorized)throw new Exception("DOC120_FILES_AUTHORIZATION_REQUIRED");
        Doc120FileScope(context,runId,environmentHash,lotId,files,reference);
        MySqlConnectionStringBuilder builder;
        try{builder=new MySqlConnectionStringBuilder(connection);}catch{throw new Exception("DOC120_FILES_CONNECTION_INVALID");}
        if(Doc120DatabaseHash(builder.Server,builder.Database)!=environmentHash)throw new Exception("DOC120_FILES_ENVIRONMENT_CHANGED");
        builder.Pooling=false;builder.ConnectionTimeout=5;
        var window=new Doc120SegmentedWindow(builder.ConnectionString);var controls=new Doc120FileControls(builder.ConnectionString);
        var lot=window.Call(r=>r.ObtenerLote(context,lotId));
        Check(lot.ControlArchivos!=null&&!lot.DetencionSolicitada&&lot.TotalArchivos==1000&&lot.TotalRaices==1&&lot.PlanId=="90"&&lot.Snapshot.VersionId=="45"&&lot.Snapshot.PerfilId=="28","DOC120_FILES_LOT_CHANGED");
        var originals=files.Select(id=>window.Call(r=>r.ObtenerArchivo(context,lotId,id))).ToArray();
        Check(originals[0].RaizId==originals[1].RaizId&&originals.All(f=>f.EstadoRecepcion=="RECIBIDO"&&f.EstadoEfecto=="NO_INICIADO"&&f.IntentoActivoId==null&&f.Resultado==null),"DOC120_FILES_NOT_PENDING");
        Check(controls.Read("SELECT COUNT(*) FROM ra_integracion_intento WHERE lote_id=50 AND estado IN ('RESERVADO','EN_CURSO','INCIERTO')")=="[[0]]","DOC120_FILES_EXISTING_ACTIVE_ATTEMPT");
        Func<ArchivoImportacionCarpetas,string,ReclamacionImportacionCarpetas> claim=(file,tag)=>new ReclamacionImportacionCarpetas{LoteId=lotId,RecursoTipo="ARCHIVO",RecursoId=file.ArchivoId,Fase="ALMACENAR",RevisionEsperada=file.Revision,EjecutorRef="DOC120_FILES",TokenReclamacion=Guid.NewGuid().ToString("N"),SolicitudHash=Doc120DatabaseHash(runId,tag)};
        var initial=controls.State();int noReserveFault=0;
        Rejected("PRUEBA_ROLLBACK",()=>window.Call(r=>{r.RegistrarOriginalNoDisponible(context,lotId,files[0],originals[0].Revision,"ORIGINAL_NO_COMPROBADO");return true;},false,(stage,db,tx)=>{
            if(stage!="CONTROL_ANTES_EVENTO")return;noReserveFault++;
            Check(controls.Read("SELECT COUNT(*) FROM ra_integracion_archivo WHERE archivo_id=122 AND resultado_doc IS NOT NULL AND intento_activo_id IS NULL",db,tx)=="[[1]]","DOC120_FILES_PROVISIONAL_INCIDENT_MISSING");
            throw new PerfilImportacionException("PRUEBA_ROLLBACK","Fallo controlado del diario de incidencia.");}));
        Check(noReserveFault==1&&initial==controls.State(),"DOC120_FILES_UNCLAIMED_INCIDENT_ROLLBACK_FAILED");
        reference("phase","RESERVATION_ROLLBACK");int reservationFault=0;
        Rejected("PRUEBA_ROLLBACK",()=>window.Call(r=>r.ReservarIntento(context,claim(originals[0],"rollback")),false,(stage,db,tx)=>{
            if(stage!="RECLAMACION_RECURSO_RESERVADO")return;reservationFault++;
            Check(controls.Read("SELECT COUNT(*) FROM ra_integracion_archivo WHERE archivo_id=122 AND estado_efecto='EN_CURSO' AND intento_activo_id IS NOT NULL",db,tx)=="[[1]]","DOC120_FILES_PROVISIONAL_CLAIM_MISSING");
            throw new PerfilImportacionException("PRUEBA_ROLLBACK","Fallo controlado de reserva.");}));
        Check(reservationFault==1&&initial==controls.State(),"DOC120_FILES_RESERVATION_ROLLBACK_FAILED");
        var old=claim(originals[0],"stale");old.RevisionEsperada++;
        Rejected("CONFLICTO_REVISION",()=>window.Call(r=>r.ReservarIntento(context,old)));
        Check(initial==controls.State(),"DOC120_FILES_STALE_CHANGED_STATE");
        reference("phase","FILE_RESERVATION_RACE");
        var requests=new[]{claim(originals[0],"left"),claim(originals[0],"right")};
        Func<int,IntentoImportacionCarpetas> reserve=index=>{
            try{return window.Call(r=>r.ReservarIntento(context,requests[index]));}
            catch(PerfilImportacionException e){if(e.Codigo!="OPERACION_EN_CURSO")throw;return null;}
        };
        var race=Doc115Pair(()=>reserve(0),()=>reserve(1));
        Check(race.Count(i=>i!=null)==1,"DOC120_FILES_RACE_NOT_UNIQUE");
        var active=race.Single(i=>i!=null);var winner=requests[race[0]!=null?0:1];
        reference("intento",active.IntentoId);
        var replay=window.Call(r=>r.ReservarIntento(context,winner));
        Check(replay.IntentoId==active.IntentoId&&replay.TokenReclamacion==active.TokenReclamacion&&replay.SolicitudHash==active.SolicitudHash&&!replay.EsNuevaReserva,"DOC120_FILES_REPLAY_CHANGED_TOKEN");
        Rejected("OPERACION_EN_CURSO",()=>window.Call(r=>r.ReservarIntento(context,claim(originals[1],"lot-contention"))));
        var current=window.Call(r=>r.ObtenerArchivo(context,lotId,files[0]));var beforeIncident=controls.State();int incidentFault=0;
        Rejected("PRUEBA_ROLLBACK",()=>window.Call(r=>{r.RegistrarOriginalNoDisponible(context,lotId,files[0],current.Revision,"ORIGINAL_NO_COMPROBADO",active);return true;},false,(stage,db,tx)=>{
            if(stage!="CONTROL_ANTES_EVENTO")return;incidentFault++;
            Check(controls.Read("SELECT COUNT(*) FROM ra_integracion_intento WHERE lote_id=50 AND archivo_id=122 AND fase='ALMACENAR' AND estado='RECHAZADO' AND evidencia_doc IS NOT NULL AND fin_utc IS NOT NULL",db,tx)=="[[1]]","DOC120_FILES_PROVISIONAL_CLOSE_MISSING");
            throw new PerfilImportacionException("PRUEBA_ROLLBACK","Fallo controlado al cerrar reserva propia.");}));
        Check(incidentFault==1&&beforeIncident==controls.State(),"DOC120_FILES_CLAIMED_INCIDENT_ROLLBACK_FAILED");
        var wrongToken=ReceiptClone(active);wrongToken.TokenReclamacion=Guid.NewGuid().ToString("N");
        Rejected("CONFLICTO_REVISION",()=>window.Call(r=>{r.RegistrarOriginalNoDisponible(context,lotId,files[0],current.Revision,"ORIGINAL_NO_COMPROBADO",wrongToken);return true;}));
        Check(beforeIncident==controls.State(),"DOC120_FILES_FOREIGN_CLOSE_CHANGED_STATE");
        window.Call(r=>{r.RegistrarOriginalNoDisponible(context,lotId,files[0],current.Revision,"ORIGINAL_NO_COMPROBADO",active);return true;});
        var aClosed=window.Call(r=>r.ObtenerArchivo(context,lotId,files[0]));
        Check(aClosed.EstadoEfecto=="NO_INICIADO"&&aClosed.IntentoActivoId==null&&aClosed.Revision==current.Revision+1&&ControlArchivoIndependienteImportacion.EsNoDisponible(aClosed.Resultado),"DOC120_FILES_PREINVOCATION_CLOSE_FAILED");
        reference("phase","ACTIVE_B_EXCLUDES_A");
        var bClaim=claim(originals[1],"active-b");var b=window.Call(r=>r.ReservarIntento(context,bClaim));reference("intento",b.IntentoId);
        var activeBState=controls.State();
        Rejected("OPERACION_EN_CURSO",()=>window.Call(r=>r.ReservarIntento(context,claim(aClosed,"a-with-active-b"))));
        Check(activeBState==controls.State(),"DOC120_FILES_ACTIVE_B_LOST_EVIDENCE");
        var bFile=window.Call(r=>r.ObtenerArchivo(context,lotId,files[1]));
        window.Call(r=>{r.RegistrarOriginalNoDisponible(context,lotId,files[1],bFile.Revision,"ORIGINAL_NO_COMPROBADO",b);return true;});
        var bClosed=window.Call(r=>r.ObtenerArchivo(context,lotId,files[1]));
        Check(bClosed.EstadoEfecto=="NO_INICIADO"&&bClosed.IntentoActivoId==null&&bClosed.Revision==bFile.Revision+1&&ControlArchivoIndependienteImportacion.EsNoDisponible(bClosed.Resultado),"DOC120_FILES_B_CLOSE_FAILED");
        foreach(var request in new[]{winner,bClaim}){
            var persisted=window.Call(r=>r.ObtenerIntentoAlmacenamiento(context,request));
            var expected=request==winner?active:b;
            Check(persisted.IntentoId==expected.IntentoId&&persisted.TokenReclamacion==expected.TokenReclamacion&&persisted.SolicitudHash==expected.SolicitudHash&&persisted.Estado=="RECHAZADO"&&persisted.FinUtc!=null&&ControlArchivoIndependienteImportacion.EsNoDisponible(persisted.Resultado),"DOC120_FILES_FINAL_EVIDENCE_CHANGED");
        }
        Check(controls.Read("SELECT COUNT(*) FROM ra_integracion_intento WHERE lote_id=50 AND archivo_id IN (122,123) AND fase='ALMACENAR' AND estado='RECHAZADO' AND fin_utc IS NOT NULL AND evidencia_doc IS NOT NULL")=="[[2]]","DOC120_FILES_FINAL_ATTEMPTS_INVALID");
        Check(controls.Read("SELECT COUNT(*) FROM ra_integracion_intento WHERE lote_id=50 AND estado IN ('RESERVADO','EN_CURSO','INCIERTO')")=="[[0]]","DOC120_FILES_ACTIVE_AT_CLOSE");
        Check(controls.Read("SELECT COUNT(*) FROM ra_integracion_evento WHERE lote_id=50 AND recurso_tipo='ARCHIVO' AND recurso_id IN (122,123) AND accion IN ('AUTORIZAR_INVOCACION_DOCUMENTAL','INICIAR_ALMACENAMIENTO_V2','OBSERVAR_RETORNO_DOCUMENTAL','RESULTADO_ALMACENAMIENTO_V2')")=="[[0]]","DOC120_FILES_UNEXPECTED_INVOCATION");
        return new Dictionary<string,object>{{"status","PASS_FILE_RESERVATIONS_REPOSITORY"},{"lotId",lotId},{"files",files},{"attempts",new[]{active.IntentoId,b.IntentoId}},{"nativeCalls",window.Calls},{"nativeControlSelects",controls.Selects},{"rollbackStages",3},{"sameFileUnique",true},{"staleRejected",true},{"activeBExcludedA",true},{"preinvocationClosures",2},{"writerInvocations",0},{"physicalPooling",false}};
    }
    public static Dictionary<string,object> RunDoc115RepositoryIntegration(bool authorized,string connection,ContextoImportacionCarpetas context,SnapshotPerfilImportacionOperativa snapshot,string[] rootNames,string runtimeRoot,byte[] pdf,Action<string,string> reference) {
        if(!authorized)throw new Exception("DOC115_REPOSITORY_AUTHORIZATION_REQUIRED");
        if(context==null||snapshot==null||rootNames==null||rootNames.Length!=6||rootNames.Distinct(StringComparer.Ordinal).Count()!=6||rootNames.Any(n=>n==null||!System.Text.RegularExpressions.Regex.IsMatch(n,@"\ADOC115-[0-9]{8}-07A-0[1-6]\z"))||pdf==null||pdf.Length!=677||reference==null||String.IsNullOrEmpty(runtimeRoot))throw new Exception("DOC115_REPOSITORY_SCOPE_INVALID");
        // Each invocation opens a fresh physical connection. No application or
        // global connection-pool configuration is changed or persisted.
        var builder=new MySqlConnectionStringBuilder(connection);builder.Pooling=false;connection=builder.ConnectionString;
        var clock=DateTime.UtcNow;clock=new DateTime(clock.Ticks-clock.Ticks%TimeSpan.TicksPerSecond,DateTimeKind.Utc);
        var repo=new MySqlImportacionCarpetasRepository(connection,()=>clock);
        var plans=new List<PlanImportacionCarpetas>();
        for(int i=0;i<3;i++) {
            var seed=PlanFixture();seed.Contexto=ReceiptClone(context);seed.Snapshot=ReceiptClone(snapshot);seed.Resultado.Elementos.Clear();
            seed.CreadoUtc=clock.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'",CultureInfo.InvariantCulture);seed.ExpiraUtc=clock.AddMinutes(10).ToString("yyyy-MM-dd'T'HH:mm:ss'Z'",CultureInfo.InvariantCulture);
            var roots=seed.Manifiesto.Raices.OrderBy(r=>r.IdCliente,StringComparer.Ordinal).ToArray();
            for(int j=0;j<roots.Length;j++){roots[j].Nombre=rootNames[i*2+j];foreach(var file in seed.Manifiesto.Archivos.Where(f=>f.RaizIdCliente==roots[j].IdCliente)){file.Nombre="DOC115-repositorio.pdf";file.RutaRelativa=roots[j].Nombre+"/"+file.Nombre;file.Bytes=pdf.Length.ToString(CultureInfo.InvariantCulture);file.UltimaModificacionLocal="0";}}
            var saved=repo.GuardarPlan(context,seed);reference("plan",saved.PlanId);plans.Add(saved);
        }
        var shared="doc115-"+Guid.NewGuid().ToString("N");
        var same=Doc115Pair(()=>Doc115Confirm(connection,context,plans[0],shared,clock),()=>Doc115Confirm(connection,context,plans[0],shared,clock));
        foreach(var item in same.Where(x=>x.Lot!=null))reference("lote",item.Lot.LoteId);
        Check(same.Any(x=>x.Lot!=null),"DOC115_SAME_PLAN_NO_WINNER");
        var lot=same.First(x=>x.Lot!=null).Lot;
        Check(same.Where(x=>x.Lot!=null).All(x=>x.Lot.LoteId==lot.LoteId),"DOC115_SAME_PLAN_DUPLICATE_LOT");
        Check(repo.ConfirmarPlan(context,plans[0].PlanId,plans[0].Revision,plans[0].Huella,shared).LoteId==lot.LoteId,"DOC115_REPLAY_CHANGED");
        Rejected("CONFLICTO_REVISION",()=>repo.ConfirmarPlan(context,plans[0].PlanId,plans[0].Revision+1,plans[0].Huella,shared));
        var differentKey="doc115-"+Guid.NewGuid().ToString("N");
        var different=Doc115Pair(()=>Doc115Confirm(connection,context,plans[1],differentKey,clock),()=>Doc115Confirm(connection,context,plans[2],differentKey,clock));
        foreach(var item in different.Where(x=>x.Lot!=null))reference("lote",item.Lot.LoteId);
        Check(different.Count(x=>x.Lot!=null)==1&&different.Count(x=>x.Code=="CONFLICTO_REVISION")==1,"DOC115_SHARED_KEY_NOT_UNIQUE");
        var second=different.Single(x=>x.Lot!=null).Lot;var loser=repo.ObtenerPlan(context,plans[1].PlanId).Estado=="REVISABLE"?plans[1]:plans[2];
        var rollback=RunRollbackIntegration(true,connection,context,loser.PlanId,lot.LoteId,plans[0].PlanId);
        string fileId;
        using(var db=new MySqlConnection(connection)) {
            db.Open();using(var cmd=new MySqlCommand("SELECT f.archivo_id FROM ra_integracion_archivo f INNER JOIN ra_integracion_raiz r ON r.raiz_id=f.raiz_id WHERE r.lote_id=@lot ORDER BY f.archivo_id LIMIT 1",db)){cmd.Parameters.AddWithValue("@lot",UInt32.Parse(lot.LoteId));fileId=Convert.ToString(cmd.ExecuteScalar(),CultureInfo.InvariantCulture);}
        }
        reference("archivo",fileId);
        var fileRecord=repo.ObtenerArchivo(context,lot.LoteId,fileId);
        string requestHash;using(var sha=System.Security.Cryptography.SHA256.Create())requestHash=Hex(sha.ComputeHash(System.Text.Encoding.ASCII.GetBytes("DOC115|"+lot.LoteId+"|"+fileId)));
        Func<ReclamacionImportacionCarpetas> claim=()=>new ReclamacionImportacionCarpetas{LoteId=lot.LoteId,RecursoTipo="ARCHIVO",RecursoId=fileId,Fase="RECEPCION",RevisionEsperada=fileRecord.Revision,EjecutorRef="DOC115_07A",TokenReclamacion=Guid.NewGuid().ToString("N"),SolicitudHash=requestHash};
        var ca=claim();var cb=claim();
        var attempts=Doc115Pair(()=>new MySqlImportacionCarpetasRepository(connection,()=>clock).ReservarIntento(context,ca),()=>new MySqlImportacionCarpetasRepository(connection,()=>clock).ReservarIntento(context,cb));
        foreach(var attempt in attempts)reference("intento",attempt.IntentoId);
        Check(attempts[0].IntentoId==attempts[1].IntentoId&&attempts[0].TokenReclamacion==attempts[1].TokenReclamacion,"DOC115_RECEIPT_CLAIM_DUPLICATED");
        var active=attempts[0];var stale=claim();stale.SolicitudHash=new string('0',64);
        Rejected("CONFLICTO_REVISION",()=>repo.ReservarIntento(context,stale));
        var current=repo.ObtenerArchivo(context,lot.LoteId,fileId);
        var limits=new LimitesImportacionCarpetas(2,2,(ulong)pdf.Length,(ulong)pdf.Length*2,16384,TimeSpan.FromMinutes(10),TimeSpan.FromHours(1));
        var storage=new AlmacenStagingImportacion(runtimeRoot,limits,lot,current);
        RecepcionVerificadaImportacion receipt;using(var input=new System.IO.MemoryStream(pdf,false))receipt=storage.Recibir(context,active,input,current.BytesPrevistos);
        Check(storage.Verificar(context,active).ContenidoHash==receipt.ContenidoHash,"DOC115_RETAINED_CONTENT_INVALID");
        int reached=0;
        var failing=Doc115Observed(connection,clock,(stage,db,tx)=>{if(stage!="RECEPCION_ARCHIVO_CONFIRMADO")return;reached++;Check(tx!=null&&Object.ReferenceEquals(tx.Connection,db),"DOC115_RECEIPT_OBSERVER_OUTSIDE_TX");using(var cmd=new MySqlCommand("SELECT COUNT(*) FROM ra_integracion_archivo WHERE archivo_id=@file AND estado_recepcion='RECIBIDO'",db,tx)){cmd.Parameters.AddWithValue("@file",UInt32.Parse(fileId));Check(Convert.ToInt64(cmd.ExecuteScalar())==1,"DOC115_RECEIPT_PROVISIONAL_ROW_MISSING");}throw new PerfilImportacionException("PRUEBA_ROLLBACK","Fallo controlado del test de recepción.");});
        Rejected("PRUEBA_ROLLBACK",()=>failing.ConfirmarRecepcion(context,lot.LoteId,fileId,active.IntentoId,active.TokenReclamacion,receipt));
        var rolledBack=repo.ObtenerArchivo(context,lot.LoteId,fileId);
        Check(reached==1&&rolledBack.EstadoRecepcion=="RECIBIENDO"&&rolledBack.IntentoActivoId==active.IntentoId&&rolledBack.StagingRef==null&&rolledBack.BytesRecibidos==null&&rolledBack.Revision==current.Revision,"DOC115_RECEIPT_ROLLBACK_FAILED");
        var committed=Doc115Pair(()=>new MySqlImportacionCarpetasRepository(connection,()=>clock).ConfirmarRecepcion(context,lot.LoteId,fileId,active.IntentoId,active.TokenReclamacion,receipt),()=>new MySqlImportacionCarpetasRepository(connection,()=>clock).ConfirmarRecepcion(context,lot.LoteId,fileId,active.IntentoId,active.TokenReclamacion,receipt));
        Check(committed.All(f=>f.EstadoRecepcion=="RECIBIDO"&&f.EstadoEfecto=="NO_INICIADO"&&f.BytesRecibidos==pdf.Length.ToString(CultureInfo.InvariantCulture)&&f.ContenidoHash==receipt.ContenidoHash&&f.StagingRef==receipt.StagingRef&&f.Revision==current.Revision+1),"DOC115_RECEIPT_COMMIT_NOT_UNIQUE");
        using(var db=new MySqlConnection(connection)) {
            db.Open();Func<string,long> count=sql=>{using(var cmd=new MySqlCommand(sql,db)){cmd.Parameters.AddWithValue("@lot",UInt32.Parse(lot.LoteId));return Convert.ToInt64(cmd.ExecuteScalar());}};
            Check(count("SELECT COUNT(*) FROM ra_integracion_lote WHERE lote_id=@lot")==1,"DOC115_LOT_MISSING");
            Check(count("SELECT COUNT(*) FROM ra_integracion_intento WHERE lote_id=@lot")==1,"DOC115_ATTEMPT_COUNT_CHANGED");
            Check(count("SELECT COUNT(*) FROM ra_integracion_intento WHERE lote_id=@lot AND estado='CONFIRMADO' AND fase='RECEPCION'")==1,"DOC115_RECEIPT_ATTEMPT_NOT_CONFIRMED");
            Check(count("SELECT COUNT(*) FROM ra_integracion_evento WHERE lote_id=@lot AND codigo='ARCHIVO_RECIBIDO'")==1,"DOC115_RECEIPT_EVENT_DUPLICATED");
            Check(count("SELECT COUNT(*) FROM ra_integracion_archivo f INNER JOIN ra_integracion_raiz r ON r.raiz_id=f.raiz_id WHERE r.lote_id=@lot AND f.estado_efecto<>'NO_INICIADO'")==0,"DOC115_DOCUMENT_EFFECT_DETECTED");
        }
        return new Dictionary<string,object>{{"status","PASS"},{"plans",plans.Select(p=>p.PlanId).ToArray()},{"lots",new[]{lot.LoteId,second.LoteId}},{"fileId",fileId},{"attemptId",active.IntentoId},{"samePlanConverged",true},{"sharedKeyExcluded",true},{"rollbackStages",rollback},{"receiptRollbackVerified",true},{"receiptCommitUnique",true},{"physicalPooling",false},{"documentEffects",0}};
    }
}
