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
