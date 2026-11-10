using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public static partial class ImportacionCarpetasBehaviorTests {
    static Delegate V2Callback(Type type,Func<object[],object> callback){
        var parameters=type.GetMethod("Invoke").GetParameters().Select(p=>Expression.Parameter(p.ParameterType,p.Name)).ToArray();
        var invoke=Expression.Invoke(Expression.Constant(callback),Expression.NewArrayInit(typeof(object),parameters.Select(p=>Expression.Convert(p,typeof(object)))));
        return Expression.Lambda(type,Expression.Convert(invoke,type.GetMethod("Invoke").ReturnType),parameters).Compile();
    }
    static object V2Ack(object document){
        var type=typeof(PerfilImportacionJson).Assembly.GetType("AcuseDocumentoV2Importacion",true);
        return type.GetConstructors(BindingFlags.NonPublic|BindingFlags.Instance).Single().Invoke(new[]{document});
    }
    sealed class AdapterV2Fixture:IDisposable {
        public readonly StagingV2Fixture Disk=new StagingV2Fixture();
        public readonly AdaptadorDocumentoImportacion Adapter;public readonly object Protocol;
        public int Starts,Gates,Observations,Saves,Writers,Verifications,LegacyStarts,LegacySaves;
        public object Prepared,Authorized,Observed;public string DurableResult;public string Mode="ok";public Action<string> OnPhase;
        public AdapterV2Fixture(){
            var constructor=typeof(AdaptadorDocumentoImportacion).GetConstructors(BindingFlags.NonPublic|BindingFlags.Instance).Single(c=>c.GetParameters().Length==7);
            Action<ComandoAlmacenamientoCarpetas> authorize=c=>Phase("authorize");
            Func<ComandoAlmacenamientoCarpetas,DatosAlmacenamientoImportacion> data=c=>{Phase("data");return ReceiptClone(Disk.Source.Data);};
            Func<ComandoAlmacenamientoCarpetas,DatosAlmacenamientoImportacion,bool> legacyStart=(c,d)=>{LegacyStarts++;throw new Exception("V1 start fallback");};
            Action<ComandoAlmacenamientoCarpetas,DatosAlmacenamientoImportacion,string,SalidaAlmacenamientoLegacy> writer=(c,d,path,output)=>{
                Writers++;Phase("writer");
                output.DocumentoId=90;output.InventarioId=Mode=="partial"?0:100;output.Respuesta=Mode=="no"?"NO":"YES";
                output.NombreArchivo=c.Archivo.NombreOriginal;output.GabineteNombre=d.GabineteNombre;
                File.Delete(path);
                if(Mode=="writer_fail"||Mode=="partial")throw new IOException("V2_PRIVATE_WRITER_SENTINEL");
                if(Mode=="cancel")throw new OperationCanceledException("V2_PRIVATE_CANCEL_SENTINEL");
                if(Mode=="abort")Thread.CurrentThread.Abort();
            };
            Func<ComandoAlmacenamientoCarpetas,DatosAlmacenamientoImportacion,ResultadoEfectoImportacion,bool> verify=(c,d,r)=>{Verifications++;Phase("verify");return Mode!="verify_false";};
            Func<ComandoAlmacenamientoCarpetas,ResultadoEfectoImportacion,ResultadoEfectoImportacion> legacySave=(c,r)=>{LegacySaves++;throw new Exception("V1 save fallback");};
            Adapter=(AdaptadorDocumentoImportacion)constructor.Invoke(new object[]{authorize,data,legacyStart,writer,verify,legacySave,Disk.Store});
            var type=typeof(PerfilImportacionJson).Assembly.GetType("PersistenciaDocumentoV2Importacion",true);var ctor=type.GetConstructors(BindingFlags.NonPublic|BindingFlags.Instance).Single();
            Func<object[],object>[] handlers={
                args=>{Starts++;Prepared=V2("Leer",V2Json(args[2]));Phase("start");if(Mode=="start_lost")throw new IOException("V2_PRIVATE_START_SENTINEL");if(Mode=="start_null")return null;if(Mode=="start_bad"){var j=JObject.Parse(V2Json(Prepared));j["Inicio"]["SnapshotHash"]=new string('f',64);return V2Ack(V2("Leer",j.ToString()));}return V2Ack(Prepared);},
                args=>{Gates++;Phase("gate");Authorized=V2("ConAutorizacion",args[1],new string('6',32),Disk.Source.Lot.Revision+1U);if(Mode=="gate_lost")throw new IOException("V2_PRIVATE_GATE_SENTINEL");if(Mode=="gate_null")return null;if(Mode=="gate_phase")return V2Ack(Prepared);if(Mode=="gate_bad"){var j=JObject.Parse(V2Json(Prepared));j["Inicio"]["ArchivoId"]="999";return V2Ack(V2("ConAutorizacion",V2("Leer",j.ToString()),new string('7',32),Disk.Source.Lot.Revision+1U));}return V2Ack(Authorized);},
                args=>{Observations++;Observed=V2("Leer",V2Json(args[1]));Phase("observe");if(Mode=="observe_lost")throw new IOException("V2_PRIVATE_OBSERVE_SENTINEL");if(Mode=="observe_null")return null;if(Mode=="observe_bad"){var j=JObject.Parse(V2Json(Observed));j["Retorno"]["DocumentoId"]="999";return V2Ack(V2("Leer",j.ToString()));}return V2Ack(Observed);},
                args=>{Saves++;Phase("save");var value=ReceiptClone((ResultadoEfectoImportacion)args[2]);DurableResult=PerfilImportacionJson.Escribir(value);if(Mode=="save_lost")throw new IOException("V2_PRIVATE_SAVE_SENTINEL");if(Mode=="save_bad")value.DocumentoId="999";return value;}
            };
            Protocol=ctor.Invoke(ctor.GetParameters().Select((p,i)=>(object)V2Callback(p.ParameterType,handlers[i])).ToArray());
        }
        public void Phase(string name){if(OnPhase!=null)OnPhase(name);}
        public ResultadoEfectoImportacion Run(object protocol=null){return (ResultadoEfectoImportacion)StagingV2(Adapter,"AlmacenarV2",Disk.Source.Lot.Contexto,Disk.Source.Root,Disk.Source.FileRecord,Disk.Source.Lot.Snapshot,Disk.Source.Attempt,Disk.Source.Lot.Revision,protocol??Protocol);}
        public void Held(){Check(Prepared!=null,"V2 start document absent");Rejected("OPERACION_EN_CURSO",()=>{using(var g=(IDisposable)Disk.Guard(Prepared)){} });}
        public void Released(){using(var g=(IDisposable)Disk.Guard(Prepared??Disk.Document))StagingV2(g,"Validar");}
        public void Dispose(){Check(LegacyStarts==0&&LegacySaves==0,"V2 used V1 fallback");Disk.Dispose();}
    }
    static void AdapterV2Cases(){
        int cases=0;Action<string,Action> run=(name,action)=>{action();cases++;Console.WriteLine("DOC-118 adapter V2 case: "+name+": passed");};
        run("retention_before_start_through_checkpoint",()=>{using(var f=new AdapterV2Fixture()){
            var phases=new System.Collections.Generic.List<string>();f.OnPhase=p=>{if(new[]{"start","gate","writer","observe","verify","save"}.Contains(p)){phases.Add(p);f.Held();}};
            var result=f.Run();Check(result.Estado=="CONFIRMADO"&&result.DocumentoId=="90"&&result.InventarioId=="100"&&phases.SequenceEqual(new[]{"start","gate","writer","observe","verify","save"}),"V2 ordering/result");
            Check(f.Starts==1&&f.Gates==1&&f.Writers==1&&f.Observations==1&&f.Saves==1,"V2 duplicate call");f.Released();
        }});
        run("lost_initial_ack_never_reaches_gate_or_writer",()=>{using(var f=new AdapterV2Fixture()){f.Mode="start_lost";var r=f.Run();Check(r.Estado=="INCIERTO"&&f.Starts==1&&f.Gates==0&&f.Writers==0&&f.Observations==0&&f.Saves==0&&!f.Disk.Source.Attempt.EsNuevaReserva,"V2 lost start retries");f.Released();}});
        run("null_or_altered_initial_ack_rejected",()=>{foreach(var mode in new[]{"start_null","start_bad"})using(var f=new AdapterV2Fixture()){f.Mode=mode;Check(f.Run().Estado=="INCIERTO"&&f.Writers==0&&f.Gates==0,"V2 accepts invalid start");f.Released();}});
        run("lost_gate_ack_never_invokes_writer",()=>{using(var f=new AdapterV2Fixture()){f.Mode="gate_lost";var r=f.Run();Check(r.Estado=="INCIERTO"&&f.Gates==1&&f.Writers==0&&f.Observations==0&&f.Saves==0,"V2 unknown gate grants writer");f.Released();}});
        run("null_wrong_phase_or_foreign_gate_ack_rejected",()=>{foreach(var mode in new[]{"gate_null","gate_phase","gate_bad"})using(var f=new AdapterV2Fixture()){f.Mode=mode;Check(f.Run().Estado=="INCIERTO"&&f.Writers==0&&f.Observations==0,"V2 accepts invalid gate");f.Released();}});
        run("revocation_after_gate_blocks_writer",()=>{
            using(var f=new AdapterV2Fixture()){f.OnPhase=p=>{if(p=="authorize")throw new PerfilImportacionException("ACCESO_DENEGADO","synthetic");};Rejected("ACCESO_DENEGADO",()=>f.Run());Check(f.Starts==0&&f.Writers==0,"V2 initial denial exposes result or starts");}
            using(var f=new AdapterV2Fixture()){f.OnPhase=p=>{if(p=="authorize"&&f.Gates==1)throw new PerfilImportacionException("ACCESO_DENEGADO","synthetic");};Check(f.Run().Estado=="INCIERTO"&&f.Gates==1&&f.Writers==0&&f.Observations==0,"V2 revoked authority ignored");f.Released();}
        });
        run("missing_original_prevents_any_durable_start",()=>{using(var f=new AdapterV2Fixture()){File.Move(f.Disk.Path,f.Disk.Path+".retained");Check(f.Run().Estado=="INCIERTO"&&f.Starts==0&&f.Gates==0&&f.Writers==0,"V2 start before retention");}});
        run("ordinary_writer_failure_preserves_observed_ids",()=>{using(var f=new AdapterV2Fixture()){f.Mode="writer_fail";var r=f.Run();Check(r.Estado=="INCIERTO"&&r.DocumentoId=="90"&&r.InventarioId=="100"&&f.Writers==1&&f.Observations==1&&f.Saves==1&&f.Verifications==0,"V2 writer failure loses IDs");f.Released();}});
        run("partial_ids_remain_partial",()=>{using(var f=new AdapterV2Fixture()){f.Mode="partial";var r=f.Run();var j=JObject.Parse(V2Json(f.Observed));Check(r.Estado=="INCIERTO"&&r.DocumentoId=="90"&&r.InventarioId==null&&(string)j["Retorno"]["DocumentoId"]=="90"&&j["Retorno"]["InventarioId"].Type==JTokenType.Null,"V2 invents missing IDs");f.Released();}});
        run("observation_is_not_physical_confirmation",()=>{using(var f=new AdapterV2Fixture()){var r=f.Run();Check(r.Estado=="CONFIRMADO"&&(string)JObject.Parse(V2Json(f.Observed))["Retorno"]["Estado"]=="INCIERTO","V2 conflates return and verification");}});
        run("lost_null_or_changed_observation_skips_checkpoint",()=>{foreach(var mode in new[]{"observe_lost","observe_null","observe_bad"})using(var f=new AdapterV2Fixture()){f.Mode=mode;var r=f.Run();Check(r.Estado=="INCIERTO"&&r.DocumentoId=="90"&&f.Observations==1&&f.Saves==0&&f.Verifications==0,"V2 unknown observation continues");f.Released();}});
        run("failed_physical_verification_stays_uncertain",()=>{using(var f=new AdapterV2Fixture()){f.Mode="verify_false";var r=f.Run();Check(r.Estado=="INCIERTO"&&f.Verifications==1&&f.Saves==1&&f.Writers==1,"V2 verifier bypassed");f.Released();}});
        run("lost_checkpoint_response_preserves_known_identity",()=>{using(var f=new AdapterV2Fixture()){f.Mode="save_lost";var r=f.Run();Check(r.Estado=="INCIERTO"&&r.DocumentoId=="90"&&r.InventarioId=="100"&&f.Saves==1&&PerfilImportacionJson.Leer<ResultadoEfectoImportacion>(f.DurableResult).Estado=="CONFIRMADO","V2 lost save destroys durable result or IDs");f.Released();}});
        run("altered_checkpoint_is_not_accepted",()=>{using(var f=new AdapterV2Fixture()){f.Mode="save_bad";var r=f.Run();Check(r.Estado=="INCIERTO"&&r.DocumentoId=="90"&&f.Writers==1,"V2 accepts foreign checkpoint identity");}});
        run("same_instance_cannot_execute_twice",()=>{using(var f=new AdapterV2Fixture()){f.Run();f.Disk.Source.Attempt.EsNuevaReserva=true;Rejected("OPERACION_EN_CURSO",()=>f.Run());Check(f.Writers==1&&f.Starts==1&&f.Gates==1,"V2 instance writer repeated");}});
        run("concurrent_call_on_same_adapter_is_rejected",()=>{using(var f=new AdapterV2Fixture())using(var entered=new ManualResetEventSlim(false))using(var release=new ManualResetEventSlim(false)){
            f.OnPhase=p=>{if(p=="data"){entered.Set();Check(release.Wait(5000),"V2 data pause timeout");}};
            var first=Task.Run(()=>f.Run());try{Check(entered.Wait(3000),"V2 pause absent");Rejected("OPERACION_EN_CURSO",()=>f.Run());}finally{release.Set();}
            Check(first.Wait(5000)&&first.Result.Estado=="CONFIRMADO"&&f.Writers==1,"V2 concurrent writer duplicated");
        }});
        run("cancellation_propagates_after_observation_and_release",()=>{using(var f=new AdapterV2Fixture()){f.Mode="cancel";bool cancelled=false;try{f.Run();}catch(OperationCanceledException){cancelled=true;}Check(cancelled&&f.Writers==1&&f.Observations==1&&f.Saves==0,"V2 cancellation suppressed");f.Released();}});
        run("thread_abort_propagates_without_reset_abort",()=>{using(var f=new AdapterV2Fixture()){
            f.Mode="abort";int aborted=0;Exception failure=null;var thread=new Thread(()=>{try{f.Run();}catch(ThreadAbortException){Interlocked.Increment(ref aborted);}catch(Exception e){failure=e;}}){IsBackground=true};thread.Start();
            Check(thread.Join(5000)&&failure==null&&aborted==1&&f.Writers==1&&f.Observations==1&&f.Saves==0,"V2 ThreadAbort swallowed or repeated");f.Released();
        }});
        run("internal_ack_cannot_be_json_authority",()=>{using(var f=new AdapterV2Fixture()){f.Run();var ack=V2Ack(f.Prepared);Check(PerfilImportacionJson.Escribir(ack)=="{}"&&!ack.GetType().IsPublic,"V2 acknowledgement exposed as payload");}});
        run("private_errors_sanitized_and_no_v1_fallback",()=>{foreach(var mode in new[]{"start_lost","gate_lost","writer_fail","observe_lost","save_lost"})using(var f=new AdapterV2Fixture()){f.Mode=mode;var json=PerfilImportacionJson.Escribir(f.Run());Check(!json.Contains("PRIVATE")&&!json.Contains(f.Disk.Path)&&!json.Contains("TokenReclamacion")&&f.LegacyStarts==0&&f.LegacySaves==0,"V2 leaks private errors or falls back");}});
        Check(cases==20,"V2 adapter case count changed");Console.WriteLine("DOC-118 V2 adapter behavior cases: 20 passed; persistence and writer doubles; no public activation");
    }
}
