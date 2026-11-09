using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public static partial class ImportacionCarpetasBehaviorTests {
    //Barreras entre hilos propios; no pausas del runtime ni del escritor real.
    sealed class MatrixV2Window:IDisposable {
        readonly ManualResetEventSlim reached=new ManualResetEventSlim(false),release=new ManualResetEventSlim(false);
        Task worker;public ResultadoEfectoImportacion Result;
        public void Pause(){reached.Set();if(!release.Wait(8000))throw new InvalidOperationException("MATRIX_WINDOW_TIMEOUT");}
        public void Start(Func<ResultadoEfectoImportacion> action){worker=Task.Run(()=>{Result=action();});Check(reached.Wait(5000),"Matrix worker did not reach window");Check(!worker.IsCompleted,"Matrix window is not active");}
        public void Complete(){release.Set();Check(worker.Wait(10000),"Matrix worker did not finish");}
        public void Dispose(){try{release.Set();if(worker!=null)Check(worker.Wait(10000),"Matrix worker leaked");}finally{reached.Dispose();release.Dispose();}}
    }
    static object MatrixV2Protocol(SqlV2Fixture sql,string phase,Action pause){
        var type=sql.Protocol.GetType();var ctor=type.GetConstructors(BindingFlags.NonPublic|BindingFlags.Instance).Single();
        var names=new[]{"Iniciar","AutorizarInvocacion","ObservarRetorno","RegistrarResultado"};var phases=new[]{"start","gate","observe","checkpoint"};
        return ctor.Invoke(ctor.GetParameters().Select((p,n)=>{
            var callback=(Delegate)type.GetField(names[n],BindingFlags.NonPublic|BindingFlags.Instance).GetValue(sql.Protocol);var selected=phases[n];
            return (object)V2Callback(p.ParameterType,a=>{object output;try{output=callback.DynamicInvoke(a);}catch(TargetInvocationException e){throw e.InnerException;}if(selected==phase)pause();return output;});
        }).ToArray());
    }
    static void MatrixV2Blocked(RecoveryV2Fixture f){
        var before=f.Sql.State.Snapshot();int writes=f.Writes,consolidations=f.Consolidations;bool blocked=false;
        try{f.Run();}catch(PerfilImportacionException){blocked=true;}
        Check(blocked&&f.Writes==writes&&f.Consolidations==consolidations&&f.Sql.State.Snapshot()==before,"Blocked matrix recovery changed durable evidence");
    }
    static void MatrixV2LiveWindow(string phase,bool history=false){
        using(var f=new RecoveryV2Fixture(begin:false))using(var window=new MatrixV2Window()){
            f.ExpectedWriterCalls=1;var protocol=MatrixV2Protocol(f.Sql,phase,window.Pause);if(phase=="writer")f.Sql.Flow.OnPhase=p=>{if(p=="writer")window.Pause();};
            window.Start(()=>f.Sql.Flow.Run(protocol));
            try{
                int sqlWrites=f.Sql.Writes,recoveryWrites=f.Writes;var before=f.Sql.State.Snapshot();
                if(history){var r=f.Run(1,1);Check(r.Repeticion&&r.Resultado.DocumentoId=="90"&&f.Guards==0,"Committed matrix history requires physical guard");}
                else MatrixV2Blocked(f);
                Check(f.Sql.Writes==sqlWrites&&f.Writes==recoveryWrites&&f.Sql.State.Snapshot()==before,"Recovery displaced active executor");
            }finally{window.Complete();}
            Check(window.Result.Estado=="CONFIRMADO"&&f.Sql.Flow.Writers==1&&f.Sql.State.Attempt.Estado=="CONFIRMADO"&&f.Sql.Consolidations==1,"Executor did not retain unique checkpoint");
            int writes=f.Sql.Writes;var replay=f.Run(1,1);Check(replay.Repeticion&&replay.Resultado.DocumentoId=="90"&&f.Writes==0&&f.Sql.Writes==writes,"Executor history replays work");
        }
    }
    static void MatrixV2Reader(RecoveryV2Fixture f,Action<string> whileAlive,Action<string> afterExit){
        var disk=f.Sql.Flow.Disk;string epoch=null;using(var child=StartStagingV2Child(disk,"hold")){
            var root=disk.Source.Stage.Receipt.Root;var ready=Path.Combine(root,"v2-reader.ready");var timer=Stopwatch.StartNew();
            try{
                while(!File.Exists(ready)&&!child.HasExited&&timer.ElapsedMilliseconds<2500)Thread.Sleep(10);
                Check(File.Exists(ready)&&!child.HasExited,"Matrix reader not alive");epoch=File.ReadAllText(Path.Combine(root,"v2-child.epoch"));whileAlive(epoch);
            }finally{
                File.WriteAllText(Path.Combine(root,"v2-reader.release"),"RELEASE");Check(child.WaitForExit(6000)&&child.ExitCode==0,"Matrix reader not closed");
            }
        }afterExit(epoch);
    }
    static void MatrixV2Cases(){
        int cases=0,children=0;Action<string,Action> run=(name,action)=>{action();cases++;Console.WriteLine("DOC-118 matrix V2 case: "+name+": passed");};
        run("live_prepared_executor_blocks_recovery",()=>MatrixV2LiveWindow("start"));
        run("live_gate_and_writer_never_grant_recovery",()=>{MatrixV2LiveWindow("gate");MatrixV2LiveWindow("writer");});
        run("live_observed_ids_still_require_executor_exit",()=>MatrixV2LiveWindow("observe"));
        run("committed_history_is_readable_while_retention_is_live",()=>MatrixV2LiveWindow("checkpoint",true));
        run("lost_start_reply_closes_only_after_retention_release",()=>{using(var f=new RecoveryV2Fixture(begin:false)){
            f.Sql.LostResponseStep="INICIO";Check(f.Sql.Flow.Run(f.Sql.Protocol).Estado=="INCIERTO"&&f.Sql.Flow.Writers==0,"Lost start invokes writer");
            var before=File.ReadAllBytes(f.Sql.Flow.Disk.Path);var r=f.Run();Check(r.Resultado.Codigo=="EJECUCION_CERRADA_SIN_INVOCACION"&&f.Writes==4&&f.Consolidations==1&&File.ReadAllBytes(f.Sql.Flow.Disk.Path).SequenceEqual(before),"Prepared post-exit closure loses original");
        }});
        run("lost_gate_reply_remains_ambiguous_after_exit",()=>{using(var f=new RecoveryV2Fixture(begin:false)){
            f.Sql.LostResponseStep="AUTORIZAR";Check(f.Sql.Flow.Run(f.Sql.Protocol).Estado=="INCIERTO"&&f.Sql.Flow.Writers==0,"Lost gate invokes writer");MatrixV2Blocked(f);Check(f.Guards==0&&f.Sql.State.File.IntentoActivoId==f.Sql.State.Attempt.IntentoId,"Released handle proves absent invocation");
        }});
        run("lost_observation_reply_recovers_original_observed_ids",()=>{using(var f=new RecoveryV2Fixture(begin:false)){
            f.ExpectedWriterCalls=1;f.Sql.LostResponseStep="OBSERVAR";var before=f.Sql.Flow.Run(f.Sql.Protocol);Check(before.Estado=="INCIERTO"&&before.DocumentoId=="90"&&f.Sql.Flow.Writers==1,"Observation reply loss not exercised");
            var r=f.Run();Check(r.Resultado.Estado=="CONFIRMADO"&&r.Resultado.DocumentoId=="90"&&r.Resultado.InventarioId=="100"&&f.Sql.Flow.Writers==1&&f.Writes==4&&f.Consolidations==1,"Recovery re-executes observed invocation");
        }});
        run("two_recoverers_only_one_commit_then_history",()=>{foreach(var positive in new[]{false,true})using(var f=new RecoveryV2Fixture(positive))using(var window=new MatrixV2Window()){
            f.BeforeSave=window.Pause;window.Start(()=>f.Run().Resultado);
            try{MatrixV2Blocked(f);Check(f.Writes==0,"Contender committed through existing guard");}finally{window.Complete();}
            f.BeforeSave=null;var again=f.Run(1,1);Check(again.Repeticion&&again.Resultado.Estado==window.Result.Estado&&f.Writes==4&&f.Consolidations==1,"Contender replay duplicated recovery commit");
        }});
        run("rollback_excludes_contender_then_retry_commits_once",()=>{using(var f=new RecoveryV2Fixture())using(var reached=new ManualResetEventSlim(false))using(var release=new ManualResetEventSlim(false)){
            f.FailAt=2;f.BeforeSave=()=>{reached.Set();if(!release.Wait(8000))throw new Exception("Matrix rollback release timeout");};bool rolledBack=false;
            var worker=Task.Run(()=>{try{f.Run();}catch(InvalidOperationException){rolledBack=true;}});
            try{Check(reached.Wait(5000),"Rollback worker not scheduled");MatrixV2Blocked(f);}finally{release.Set();Check(worker.Wait(10000),"Rollback worker leaked");}
            Check(rolledBack&&f.Writes==0&&f.Sql.State.Attempt.Estado=="EN_CURSO"&&f.Sql.State.File.IntentoActivoId==f.Sql.State.Attempt.IntentoId,"Rollback published partial recovery");
            f.FailAt=0;f.BeforeSave=null;Check(f.Run().Resultado.Estado=="RECHAZADO"&&f.Writes==4&&f.Consolidations==1,"Rollback guard leaked or retry duplicated commit");
        }});
        run("reader_process_exit_allows_only_same_epoch_recovery",()=>{using(var f=new RecoveryV2Fixture()){
            MatrixV2Reader(f,epoch=>{MatrixV2Blocked(f);Check(f.Writes==0,"Reader process permits recovery");},epoch=>{Check(f.Run().Resultado.Estado=="RECHAZADO"&&f.Writes==4,"Exited reader leaks handle");});children++;
        }});
        run("foreign_epoch_stays_blocked_after_reader_process_exit",()=>{using(var f=new RecoveryV2Fixture()){
            MatrixV2Reader(f,epoch=>{var j=JObject.Parse(f.Sql.State.Evidence);Check((string)j["Inicio"]["Original"]["NodoRef"]!=epoch,"Child reused parent epoch");j["Inicio"]["Original"]["NodoRef"]=epoch;f.Sql.State.Evidence=V2Json(V2("Leer",j.ToString()));f.Sql.State.Journal.Rows[1]["detalle_doc"]=f.Sql.State.Evidence;MatrixV2Blocked(f);},epoch=>{MatrixV2Blocked(f);Check(f.Writes==0,"Process exit qualifies another provider epoch");});children++;
        }});
        run("protected_v1_attempts_are_unchanged_in_mixed_protocol",()=>{foreach(var id in new[]{"41","53","69"})using(var f=new RecoveryV2Fixture()){
            f.Sql.State.Attempt.IntentoId=id;f.Sql.State.File.IntentoActivoId=id;f.Sql.State.Evidence=PerfilImportacionJson.Escribir(RepositoryHelper("EvidenciaInicialDocumento",f.Sql.Command));f.Sql.State.Attempt.Resultado=PerfilImportacionJson.Leer<ResultadoEfectoImportacion>(f.Sql.State.Evidence);
            var original=File.ReadAllBytes(f.Sql.Flow.Disk.Path);MatrixV2Blocked(f);Check(f.Guards==0&&File.ReadAllBytes(f.Sql.Flow.Disk.Path).SequenceEqual(original),"Mixed protocol mutates protected synthetic attempt");
        }});
        Check(cases==12&&children==2,"Matrix V2 accounting changed");Console.WriteLine("DOC-118 V2 interleaving matrix cases: 12 passed; synthetic child processes: 2 closed; real guards; SQL doubles; no real runtime");
    }
}
