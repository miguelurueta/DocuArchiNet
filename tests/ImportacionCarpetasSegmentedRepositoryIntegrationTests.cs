using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using MySql.Data.MySqlClient;

// Explicit laboratory entry point. Run() exercises only its pre-connection
// guards. No application endpoint, credential discovery, DDL or cleanup.
public static partial class ImportacionCarpetasBehaviorTests {
    sealed class Doc120SegmentedWindow {
        readonly string connection;
        readonly Stopwatch elapsed = Stopwatch.StartNew();
        int calls, selects;
        public int Calls { get { return calls; } }
        public int Selects { get { return selects; } }
        public Doc120SegmentedWindow(string value) { connection=value; }
        void Admit() {
            if(elapsed.Elapsed > TimeSpan.FromMinutes(15))throw new Exception("DOC120_SEGMENTED_WINDOW_EXPIRED");
        }
        void AdmitSelect() {
            Admit();
            if(Interlocked.Increment(ref selects)>64)throw new Exception("DOC120_SEGMENTED_SELECT_BUDGET");
        }
        public T Call<T>(Func<MySqlImportacionCarpetasRepository,T> operation, bool expired=false,
            Action<string,MySqlConnection,MySqlTransaction> observer=null) {
            Admit();
            if(Interlocked.Increment(ref calls)>48)throw new Exception("DOC120_SEGMENTED_CALL_BUDGET");
            Func<DateTime> clock=()=>expired?DateTime.UtcNow.AddHours(1):DateTime.UtcNow;
            var repo=observer==null?new MySqlImportacionCarpetasRepository(connection,clock):
                Doc115Observed(connection,clock(),observer);
            return operation(repo);
        }
        public long Count(string sql, string plan, MySqlConnection db=null, MySqlTransaction tx=null) {
            AdmitSelect();
            if(db==null)using(var control=new MySqlConnection(connection)) {
                control.Open();return ExecuteCount(sql,plan,control,null);
            }
            return ExecuteCount(sql,plan,db,tx);
        }
        public bool InventoryMatches(string plan,ManifiestoImportacionCarpetas expected) {
            AdmitSelect();
            var seen=new HashSet<string>(StringComparer.Ordinal);
            using(var db=new MySqlConnection(connection)) {
                db.Open();
                using(var command=new MySqlCommand("SELECT r.cliente_raiz_id,a.cliente_archivo_id,a.bytes_previstos,a.origen_doc FROM ra_integracion_archivo a INNER JOIN ra_integracion_raiz r ON r.raiz_id=a.raiz_id INNER JOIN ra_integracion_lote l ON l.lote_id=r.lote_id WHERE l.plan_id=@plan ORDER BY a.archivo_id",db)) {
                    command.CommandTimeout=10;command.Parameters.AddWithValue("@plan",UInt32.Parse(plan,CultureInfo.InvariantCulture));
                    using(var reader=command.ExecuteReader())while(reader.Read()) {
                        var source=PerfilImportacionJson.Leer<ArchivoManifiestoImportacion>(reader.GetString(3));
                        var original=expected.Archivos.SingleOrDefault(a=>a.IdCliente==reader.GetString(1));
                        if(original==null||!seen.Add(original.IdCliente)||original.RaizIdCliente!=reader.GetString(0)||
                            original.Bytes!=Convert.ToString(reader.GetValue(2),CultureInfo.InvariantCulture)||
                            PerfilImportacionJson.Escribir(source)!=PerfilImportacionJson.Escribir(original))return false;
                    }
                }
            }
            return seen.Count==expected.Archivos.Count;
        }
        static long ExecuteCount(string sql,string plan,MySqlConnection db,MySqlTransaction tx) {
            using(var command=new MySqlCommand(sql,db,tx)) {
                command.CommandTimeout=10;
                command.Parameters.AddWithValue("@plan",UInt32.Parse(plan,CultureInfo.InvariantCulture));
                return Convert.ToInt64(command.ExecuteScalar(),CultureInfo.InvariantCulture);
            }
        }
    }

    static string Doc120DatabaseHash(string server,string database) {
        using(var hash=SHA256.Create())return Hex(hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(server+"|"+database)));
    }
    static void Doc120SegmentedScope(PlanImportacionCarpetas fixture,string runId,string environmentHash,Action<string,string> reference) {
        if(!System.Text.RegularExpressions.Regex.IsMatch(runId??"",@"\A[a-f0-9]{32}\z")||
            !System.Text.RegularExpressions.Regex.IsMatch(environmentHash??"",@"\A[a-f0-9]{64}\z")||reference==null||
            fixture==null||fixture.Contexto==null||!fixture.Contexto.PuedeImportar||fixture.Snapshot==null||
            fixture.Snapshot.Configuracion==null||fixture.EsquemaDoc!=1||fixture.PlanId!=null||
            fixture.Manifiesto==null||fixture.Manifiesto.Raices==null||fixture.Manifiesto.Archivos==null||
            fixture.Manifiesto.Raices.Any(r=>r==null)||fixture.Manifiesto.Archivos.Any(f=>f==null)||
            fixture.Resultado==null||fixture.Resultado.UnidadesElegibles==null||fixture.Resultado.Incidencias==null||!fixture.Resultado.PuedeConfirmar||
            fixture.Manifiesto.Raices.Count!=1||fixture.Manifiesto.Archivos.Count!=2||
            !fixture.Resultado.UnidadesElegibles.SequenceEqual(new[]{"r1"})||fixture.Resultado.Incidencias.Count!=0)
            throw new Exception("DOC120_SEGMENTED_SCOPE_INVALID");
        foreach(var id in new[]{fixture.Contexto.UsuarioRef,fixture.Contexto.AmbitoRef,fixture.Contexto.NivelRef,
            fixture.Snapshot.PerfilId,fixture.Snapshot.VersionId})
            if(!ValidadorPerfilImportacion.ReferenciaValida(id))throw new Exception("DOC120_SEGMENTED_SCOPE_INVALID");
        var root=fixture.Manifiesto.Raices.Single();
        if(root.IdCliente!="r1"||root.SeleccionId!="s1"||root.Nombre!="DOC120-"+runId+"-SEG")
            throw new Exception("DOC120_SEGMENTED_SCOPE_INVALID");
        for(int n=1;n<=2;n++) {
            var files=fixture.Manifiesto.Archivos.Where(f=>f.IdCliente=="a"+n).ToArray();
            ulong bytes;
            if(files.Length!=1||files[0].RaizIdCliente!="r1"||String.IsNullOrWhiteSpace(files[0].Nombre)||!files[0].Nombre.EndsWith(".pdf",StringComparison.OrdinalIgnoreCase)||
                files[0].RutaRelativa!=root.Nombre+"/"+files[0].Nombre||!UInt64.TryParse(files[0].Bytes,NumberStyles.None,CultureInfo.InvariantCulture,out bytes)||
                bytes==0||bytes>2097152||bytes.ToString(CultureInfo.InvariantCulture)!=files[0].Bytes||files[0].UltimaModificacionLocal!="0")
                throw new Exception("DOC120_SEGMENTED_SCOPE_INVALID");
        }
    }
    static PartePreparacionImportacion Doc120Part<T>(string type,uint number,List<T> rows) {
        var text=PerfilImportacionJson.Escribir(rows);
        return new PartePreparacionImportacion{Tipo=type,Numero=number,Contenido=text,Huella=PlanSegmentadoImportacionJson.HuellaTexto(text)};
    }
    static List<FragmentoPlanImportacion> Doc120PartResult(PlanImportacionCarpetas plan,string file) {
        var elements=plan.Resultado.Elementos.Where(e=>file==null?e.Ambito=="RAIZ":e.Ambito=="ARCHIVO"&&e.ArchivoIdCliente==file).ToList();
        var text=PerfilImportacionJson.Escribir(elements);
        return new List<FragmentoPlanImportacion>{new FragmentoPlanImportacion{Tipo="ELEMENTOS",Numero=1,Contenido=text,Huella=PlanSegmentadoImportacionJson.HuellaTexto(text)}};
    }
    static PlanImportacionCarpetas Doc120SegmentedPreparedFixture(PlanImportacionCarpetas fixture,DateTime now) {
        // Input is explicitly schema 1. The segmented-only clone rejects it;
        // the existing bounded JSON clone preserves this two-file fixture.
        var plan=ReceiptClone(fixture);
        now=new DateTime(now.Ticks-now.Ticks%TimeSpan.TicksPerSecond,DateTimeKind.Utc);
        plan.CreadoUtc=now.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'",CultureInfo.InvariantCulture);
        plan.ExpiraUtc=now.AddMinutes(10).ToString("yyyy-MM-dd'T'HH:mm:ss'Z'",CultureInfo.InvariantCulture);
        return Prepare(plan);
    }

    public static Dictionary<string,object> RunDoc120SegmentedRepositoryIntegration(bool authorized,string connection,
        PlanImportacionCarpetas fixture,string runId,string environmentHash,Action<string,string> reference) {
        if(!authorized)throw new Exception("DOC120_SEGMENTED_AUTHORIZATION_REQUIRED");
        Doc120SegmentedScope(fixture,runId,environmentHash,reference);
        MySqlConnectionStringBuilder builder;
        try { builder=new MySqlConnectionStringBuilder(connection); }
        catch { throw new Exception("DOC120_SEGMENTED_CONNECTION_INVALID"); }
        if(String.IsNullOrWhiteSpace(builder.Server)||String.IsNullOrWhiteSpace(builder.Database)||String.IsNullOrWhiteSpace(builder.UserID)||
            Doc120DatabaseHash(builder.Server,builder.Database)!=environmentHash)
            throw new Exception("DOC120_SEGMENTED_ENVIRONMENT_CHANGED");
        builder.Pooling=false;builder.ConnectionTimeout=5;
        try { return RunDoc120SegmentedRepositoryCases(builder.ConnectionString,fixture,runId,reference); }
        catch { throw new Exception("DOC120_SEGMENTED_STOPPED_REVIEW_REQUIRED"); }
    }

    static Dictionary<string,object> RunDoc120SegmentedRepositoryCases(string connection,PlanImportacionCarpetas fixture,
        string runId,Action<string,string> reference) {
        var window=new Doc120SegmentedWindow(connection);
        // Validate the complete synthetic input with the existing pure validator
        // before admitting any connection. Publication is rechecked by the repo.
        var plan=Doc120SegmentedPreparedFixture(fixture,DateTime.UtcNow);
        var context=plan.Contexto;
        var totalBytes=plan.Manifiesto.Archivos.Sum(a=>(long)UInt64.Parse(a.Bytes,CultureInfo.InvariantCulture)).ToString(CultureInfo.InvariantCulture);
        var draft=new PreparacionSegmentadaImportacion{Contexto=ReceiptClone(context),Snapshot=ReceiptClone(plan.Snapshot),
            Clave=runId,CabeceraHash=PlanSegmentadoImportacionJson.HuellaTexto(PerfilImportacionJson.Escribir(new {
                Contexto=context,Snapshot=plan.Snapshot,Manifiesto=plan.Manifiesto})),TotalRaices=1,TotalArchivos=2,
            TotalBytes=totalBytes,PartesRaices=1,PartesArchivos=2,Estado="BORRADOR",Revision=1,
            CreadoUtc=plan.CreadoUtc,ExpiraUtc=plan.ExpiraUtc};
        reference("phase","START");
        // A previous draft is never resumed by this entry point. The outer
        // approved runner must also retain its one-shot journal and resource seal.
        if(window.Call(r=>r.BuscarPreparacionPorClave(context,runId))!=null)
            throw new Exception("DOC120_SEGMENTED_PRIOR_RUN_REQUIRES_REVIEW");
        var state=window.Call(r=>r.IniciarPreparacion(context,draft));
        string preparationId=state.PreparacionId;
        reference("preparacion",preparationId);
        Check(window.Call(r=>r.IniciarPreparacion(context,draft)).PreparacionId==preparationId,"DOC120_SEGMENTED_START_DUPLICATED");
        var changedHeader=ReceiptClone(draft);changedHeader.CabeceraHash=new string('0',64);
        Rejected("CONFLICTO_REVISION",()=>window.Call(r=>r.IniciarPreparacion(context,changedHeader)));
        plan.EsquemaDoc=2;plan.PreparacionId=preparationId;
        var root=Doc120Part("RAICES",1,plan.Manifiesto.Raices);
        var first=Doc120Part("ARCHIVOS",1,plan.Manifiesto.Archivos.Where(a=>a.IdCliente=="a1").ToList());
        var second=Doc120Part("ARCHIVOS",2,plan.Manifiesto.Archivos.Where(a=>a.IdCliente=="a2").ToList());
        Rejected("CONFLICTO_REVISION",()=>window.Call(r=>r.SellarPreparacion(context,preparationId,state.Revision,runId,plan)));
        Rejected("CONFLICTO_REVISION",()=>window.Call(r=>r.RegistrarParte(context,preparationId,state.Revision,root),true));
        state=window.Call(r=>r.RegistrarParte(context,preparationId,state.Revision,root));
        var revision=state.Revision;
        state=window.Call(r=>r.RegistrarParte(context,preparationId,revision,second));
        Check(window.Call(r=>r.RegistrarParte(context,preparationId,revision,second)).Revision==state.Revision,"DOC120_SEGMENTED_PART_REPLAY_CHANGED");
        var changedFile=ReceiptClone(plan.Manifiesto.Archivos.Single(a=>a.IdCliente=="a2"));changedFile.Bytes=(UInt64.Parse(changedFile.Bytes,CultureInfo.InvariantCulture)+1).ToString(CultureInfo.InvariantCulture);
        var conflict=Doc120Part("ARCHIVOS",2,new List<ArchivoManifiestoImportacion>{changedFile});
        Rejected("CONFLICTO_REVISION",()=>window.Call(r=>r.RegistrarParte(context,preparationId,revision,conflict)));
        revision=state.Revision;
        reference("phase","PART_RACE");
        var concurrent=Doc115Pair(()=>window.Call(r=>r.RegistrarParte(context,preparationId,revision,first)),
            ()=>window.Call(r=>r.RegistrarParte(context,preparationId,revision,first)));
        Check(concurrent.All(p=>p.Revision==revision+1),"DOC120_SEGMENTED_PART_RACE_DUPLICATED");
        state=concurrent[0];
        var parts=window.Call(r=>r.LeerPartesPreparacion(context,preparationId));
        Check(parts.Count==3&&parts.Select(p=>p.Tipo+":"+p.Numero).Distinct().Count()==3,"DOC120_SEGMENTED_PART_POSITIONS_CHANGED");
        Rejected("CONFLICTO_REVISION",()=>window.Call(r=>r.SellarPreparacion(context,preparationId,state.Revision,runId,plan)));
        var stamp=new string('a',64);
        Rejected("CONFLICTO_REVISION",()=>window.Call(r=>r.GuardarValidacionParte(context,preparationId,revision,"RAICES",1,stamp,Doc120PartResult(plan,null))));
        state=window.Call(r=>r.GuardarValidacionParte(context,preparationId,state.Revision,"RAICES",1,stamp,Doc120PartResult(plan,null)));
        state=window.Call(r=>r.GuardarValidacionParte(context,preparationId,state.Revision,"ARCHIVOS",2,stamp,Doc120PartResult(plan,"a2")));
        state=window.Call(r=>r.GuardarValidacionParte(context,preparationId,state.Revision,"ARCHIVOS",1,stamp,Doc120PartResult(plan,"a1")));
        revision=state.Revision;
        reference("phase","SEAL_RACE");
        var sealedPlans=Doc115Pair(()=>window.Call(r=>r.SellarPreparacion(context,preparationId,revision,runId,plan)),
            ()=>window.Call(r=>r.SellarPreparacion(context,preparationId,revision,runId,plan)));
        var sealedPlan=sealedPlans[0];
        Check(sealedPlans.All(p=>p.PlanId==sealedPlan.PlanId&&p.Huella==sealedPlan.Huella),"DOC120_SEGMENTED_SEAL_DUPLICATED");
        reference("plan",sealedPlan.PlanId);
        Check(window.Call(r=>r.SellarPreparacion(context,preparationId,1,runId,plan)).PlanId==sealedPlan.PlanId,"DOC120_SEGMENTED_SEAL_REPLAY_CHANGED");
        var sealedState=window.Call(r=>r.ObtenerPreparacion(context,preparationId));
        Check(sealedState.Estado=="SELLADA"&&sealedState.PlanId==sealedPlan.PlanId&&sealedState.Revision==revision+1,"DOC120_SEGMENTED_SEAL_STATE_INVALID");
        var recovered=window.Call(r=>r.ObtenerPlan(context,sealedPlan.PlanId));
        Check(recovered.EsquemaDoc==2&&recovered.Manifiesto.Raices.Count==1&&recovered.Manifiesto.Archivos.Count==2&&
            PlanSegmentadoImportacionJson.HuellaInventario(recovered.Manifiesto)==PlanSegmentadoImportacionJson.HuellaInventario(plan.Manifiesto),"DOC120_SEGMENTED_INVENTORY_LOST");
        Rejected("CONFLICTO_REVISION",()=>window.Call(r=>r.ConfirmarPlan(context,sealedPlan.PlanId,1,sealedPlan.Huella,"expired-"+runId),true));
        var stages=RunConfirmationRollbackCases(sealedPlan,
            probe=>window.Call(r=>r.ConfirmarPlan(context,sealedPlan.PlanId,1,sealedPlan.Huella,"rollback-"+runId),false,probe),
            ()=>window.Call(r=>r.ObtenerPlan(context,sealedPlan.PlanId)),
            (sql,db,tx)=>window.Count(sql,sealedPlan.PlanId,db,tx),stage=>reference("phase",stage));
        reference("phase","CONFIRMATION_RACE");
        var lots=Doc115Pair(()=>window.Call(r=>r.ConfirmarPlan(context,sealedPlan.PlanId,1,sealedPlan.Huella,"left-"+runId)),
            ()=>window.Call(r=>r.ConfirmarPlan(context,sealedPlan.PlanId,1,sealedPlan.Huella,"right-"+runId)));
        var lot=lots[0];
        Check(lots.All(l=>l.LoteId==lot.LoteId&&l.TotalRaices==1&&l.TotalArchivos==2&&l.TotalBytes==totalBytes),"DOC120_SEGMENTED_CONFIRMATION_DUPLICATED");
        reference("lote",lot.LoteId);
        Check(window.Call(r=>r.ConfirmarPlan(context,sealedPlan.PlanId,1,sealedPlan.Huella,"left-"+runId)).LoteId==lot.LoteId,"DOC120_SEGMENTED_CONFIRMATION_REPLAY_CHANGED");
        Check(window.Count("SELECT COUNT(*) FROM ra_integracion_lote WHERE plan_id=@plan",sealedPlan.PlanId)==1,"DOC120_SEGMENTED_FINAL_LOT_COUNT");
        Check(window.Count("SELECT COUNT(*) FROM ra_integracion_raiz r INNER JOIN ra_integracion_lote l ON l.lote_id=r.lote_id WHERE l.plan_id=@plan",sealedPlan.PlanId)==1,"DOC120_SEGMENTED_FINAL_ROOT_COUNT");
        Check(window.Count("SELECT COUNT(*) FROM ra_integracion_archivo a INNER JOIN ra_integracion_raiz r ON r.raiz_id=a.raiz_id INNER JOIN ra_integracion_lote l ON l.lote_id=r.lote_id WHERE l.plan_id=@plan AND a.estado_recepcion='PENDIENTE' AND a.estado_efecto='NO_INICIADO'",sealedPlan.PlanId)==2,"DOC120_SEGMENTED_FINAL_INVENTORY_CHANGED");
        Check(window.Count("SELECT COUNT(*) FROM ra_integracion_evento e INNER JOIN ra_integracion_lote l ON l.lote_id=e.lote_id WHERE l.plan_id=@plan AND e.codigo='LOTE_CONFIRMADO'",sealedPlan.PlanId)==1,"DOC120_SEGMENTED_FINAL_EVENT_DUPLICATED");
        Check(window.Count("SELECT COUNT(*) FROM ra_integracion_intento i INNER JOIN ra_integracion_lote l ON l.lote_id=i.lote_id WHERE l.plan_id=@plan",sealedPlan.PlanId)==0,"DOC120_SEGMENTED_UNEXPECTED_ATTEMPT");
        Check(window.InventoryMatches(sealedPlan.PlanId,plan.Manifiesto),"DOC120_SEGMENTED_FINAL_INVENTORY_CORRELATION_FAILED");
        reference("phase","FINISHED");
        return new Dictionary<string,object>{{"status","PASS_SEGMENTED_REPOSITORY"},{"preparationId",preparationId},
            {"planId",sealedPlan.PlanId},{"lotId",lot.LoteId},{"repositoryCalls",window.Calls},{"controlSelects",window.Selects},
            {"rollbackStages",stages},{"roots",1},{"files",2},{"bytes",totalBytes},{"finalFragments",PlanSegmentadoImportacionJson.Fragmentar(recovered).Count},
            {"documentEffects",0},{"physicalPooling",false}};
    }

    static PlanImportacionCarpetas Doc120SegmentedGuardFixture(string runId) {
        var plan=PlanFixture();
        plan.Manifiesto.Raices.RemoveAll(r=>r.IdCliente!="r1");
        plan.Manifiesto.Raices[0].SeleccionId="s1";plan.Manifiesto.Raices[0].Nombre="DOC120-"+runId+"-SEG";
        plan.Manifiesto.Archivos.Clear();plan.Resultado.Elementos.Clear();plan.Resultado.UnidadesElegibles=new List<string>{"r1"};
        for(int n=1;n<=2;n++)plan.Manifiesto.Archivos.Add(new ArchivoManifiestoImportacion{IdCliente="a"+n,RaizIdCliente="r1",
            Nombre="0"+n+".pdf",RutaRelativa=plan.Manifiesto.Raices[0].Nombre+"/0"+n+".pdf",Bytes="7",UltimaModificacionLocal="0"});
        return plan;
    }
    static void Doc120SegmentedRepositoryGuardCases() {
        string run=new string('a',32),fingerprint=new string('b',64);
        Action<string,Action> rejects=(code,operation)=>{
            try { operation(); } catch(Exception error) { Check(error.Message==code,"DOC120_SEGMENTED_GUARD_UNEXPECTED_FAILURE");return; }
            throw new Exception("DOC120_SEGMENTED_GUARD_MISSING");
        };
        rejects("DOC120_SEGMENTED_AUTHORIZATION_REQUIRED",()=>RunDoc120SegmentedRepositoryIntegration(false,"PRIVATE_CONNECTION_SENTINEL",null,null,null,null));
        rejects("DOC120_SEGMENTED_SCOPE_INVALID",()=>RunDoc120SegmentedRepositoryIntegration(true,"PRIVATE_CONNECTION_SENTINEL",null,run,fingerprint,(type,id)=>{}));
        foreach(var change in new Action<PlanImportacionCarpetas>[] {
            p=>p.Contexto.PuedeImportar=false,p=>p.Contexto.UsuarioRef="01",p=>p.PlanId="9",p=>p.EsquemaDoc=2,
            p=>p.Manifiesto.Raices[0].Nombre="existing-root",p=>p.Manifiesto.Archivos[0].Bytes="0",
            p=>p.Manifiesto.Archivos[0].RutaRelativa="foreign/file.pdf",p=>p.Resultado.UnidadesElegibles.Clear()}) {
            var plan=Doc120SegmentedGuardFixture(run);change(plan);
            rejects("DOC120_SEGMENTED_SCOPE_INVALID",()=>RunDoc120SegmentedRepositoryIntegration(true,"PRIVATE_CONNECTION_SENTINEL",plan,run,fingerprint,(type,id)=>{}));
        }
        var valid=Doc120SegmentedGuardFixture(run);
        rejects("DOC120_SEGMENTED_CONNECTION_INVALID",()=>RunDoc120SegmentedRepositoryIntegration(true,"DOC120_INVALID_OPTION=PRIVATE",valid,run,fingerprint,(type,id)=>{}));
        rejects("DOC120_SEGMENTED_ENVIRONMENT_CHANGED",()=>RunDoc120SegmentedRepositoryIntegration(true,"Server=127.0.0.1;Database=doc120_lab;User ID=PRIVATE",valid,run,fingerprint,(type,id)=>{}));
        var invalidSnapshot=ReceiptClone(valid);invalidSnapshot.Snapshot.Huella="PRIVATE_SNAPSHOT_SENTINEL";
        rejects("DOC120_SEGMENTED_STOPPED_REVIEW_REQUIRED",()=>RunDoc120SegmentedRepositoryIntegration(true,
            "Server=127.0.0.1;Database=doc120_lab;User ID=PRIVATE",invalidSnapshot,run,Doc120DatabaseHash("127.0.0.1","doc120_lab"),(type,id)=>{}));
        var prepared=Doc120SegmentedPreparedFixture(valid,DateTime.UtcNow);
        Check(prepared.Estado=="REVISABLE"&&prepared.Manifiesto.Raices.Count==1&&prepared.Manifiesto.Archivos.Count==2,"DOC120_SEGMENTED_LOCAL_FIXTURE_INVALID");
        var parts=new[]{Doc120Part("RAICES",1,prepared.Manifiesto.Raices),
            Doc120Part("ARCHIVOS",2,prepared.Manifiesto.Archivos.Where(a=>a.IdCliente=="a2").ToList()),
            Doc120Part("ARCHIVOS",1,prepared.Manifiesto.Archivos.Where(a=>a.IdCliente=="a1").ToList())};
        var rebuilt=(ManifiestoImportacionCarpetas)RepositoryHelper("InventarioDePartes",parts.ToList());
        Check(PlanSegmentadoImportacionJson.HuellaInventario(rebuilt)==PlanSegmentadoImportacionJson.HuellaInventario(prepared.Manifiesto),"DOC120_SEGMENTED_LOCAL_PARTITION_CHANGED_INVENTORY");
        var budget=new Doc120SegmentedWindow("PRIVATE_CONNECTION_SENTINEL");
        for(int i=0;i<48;i++)Check(budget.Call(r=>true),"DOC120_SEGMENTED_LOCAL_ADMISSION_FAILED");
        rejects("DOC120_SEGMENTED_CALL_BUDGET",()=>budget.Call<bool>(r=>{throw new Exception("PRIVATE_OPERATION_SENTINEL");}));
        Console.WriteLine("DOC-120 segmented repository guards: 16 passed; no database connection");
    }
}
