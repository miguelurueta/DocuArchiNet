using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public static partial class ImportacionCarpetasBehaviorTests {
    static object AckLossControl(string phase,ComandoAlmacenamientoCarpetas command,uint revision,string run="DOC118-V2-local"){
        try{return typeof(PerfilImportacionJson).Assembly.GetType("Doc118PerdidaAcuseV2",true).GetConstructors(BindingFlags.NonPublic|BindingFlags.Instance).Single().Invoke(new object[]{run,phase,command,revision});}catch(TargetInvocationException e){throw e.InnerException;}
    }
    static JObject AckLossView(object control){return JObject.Parse(PerfilImportacionJson.Escribir(StagingV2(control,"Observacion")));}
    static object AckLossInvoke(object port,string callback,params object[] args){try{return ((Delegate)port.GetType().GetField(callback,BindingFlags.NonPublic|BindingFlags.Instance).GetValue(port)).DynamicInvoke(args);}catch(TargetInvocationException e){throw e.InnerException;}}
    static void AckLossV2Cases(){
        int cases=0;Action<string,Action> run=(name,action)=>{action();cases++;Console.WriteLine("DOC-118 ack loss V2 case: "+name+": passed");};
        run("native_start_ack_lost_then_negative_recovery",()=>{using(var f=new RecoveryV2Fixture(begin:false))using(var c=(IDisposable)AckLossControl("INICIO",f.Sql.Command,f.Sql.State.Lot.Revision)){
            var port=StagingV2(c,"Decorar",f.Sql.Protocol);var result=RunBinding(f.Sql,StorageV2Binding(f.Sql.Flow.Adapter,port,(ctx,id)=>ReceiptClone(f.Sql.State.Lot)));var view=AckLossView(c);
            Check(result.Estado=="INCIERTO"&&f.Sql.Commits==1&&f.Sql.Flow.Writers==0&&(int)view["callbacks"]==1&&(int)view["acusesValidos"]==1&&(int)view["perdidas"]==1,"Start loss before native acknowledgment");Check(f.Run().Resultado.Codigo=="EJECUCION_CERRADA_SIN_INVOCACION"&&f.Writes==4,"Start loss not recoverable under original guard");
        }});
        run("native_gate_ack_lost_stays_blocked",()=>{using(var f=new RecoveryV2Fixture(begin:false))using(var c=(IDisposable)AckLossControl("AUTORIZAR",f.Sql.Command,f.Sql.State.Lot.Revision)){
            var port=StagingV2(c,"Decorar",f.Sql.Protocol);Check(RunBinding(f.Sql,StorageV2Binding(f.Sql.Flow.Adapter,port,(ctx,id)=>ReceiptClone(f.Sql.State.Lot))).Estado=="INCIERTO"&&f.Sql.Commits==2&&f.Sql.Flow.Writers==0,"Gate loss grants writer");var view=AckLossView(c);Check((int)view["acusesValidos"]==2&&(int)view["perdidas"]==1,"Gate not lost after acknowledgment");MatrixV2Blocked(f);Check(f.Writes==0,"Gate ambiguity is reset");
        }});
        run("native_observation_ack_lost_recovers_same_ids",()=>{using(var f=new RecoveryV2Fixture(begin:false))using(var c=(IDisposable)AckLossControl("OBSERVAR",f.Sql.Command,f.Sql.State.Lot.Revision)){
            f.ExpectedWriterCalls=1;var port=StagingV2(c,"Decorar",f.Sql.Protocol);var result=RunBinding(f.Sql,StorageV2Binding(f.Sql.Flow.Adapter,port,(ctx,id)=>ReceiptClone(f.Sql.State.Lot)));var view=AckLossView(c);Check(result.Estado=="INCIERTO"&&result.DocumentoId=="90"&&f.Sql.Commits==3&&f.Sql.Flow.Writers==1&&(int)view["acusesValidos"]==3&&(int)view["perdidas"]==1,"Observation loss reaches checkpoint or loses IDs");Check(f.Run().Resultado.Estado=="CONFIRMADO"&&f.Sql.Flow.Writers==1&&f.Writes==4,"Observed loss replays writer");
        }});
        run("native_rollback_is_not_reported_as_lost_ack",()=>{foreach(var lostNativeReply in new[]{false,true})using(var f=new SqlV2Fixture())using(var c=(IDisposable)AckLossControl("INICIO",f.Command,f.State.Lot.Revision)){
            if(lostNativeReply)f.LostResponseStep="INICIO";else f.FailAt=1;var before=f.State.Snapshot();RunBinding(f,StorageV2Binding(f.Flow.Adapter,StagingV2(c,"Decorar",f.Protocol),(ctx,id)=>ReceiptClone(f.State.Lot)));var view=AckLossView(c);Check((lostNativeReply?f.Commits==1:f.State.Snapshot()==before&&f.Commits==0)&&(int)view["callbacks"]==1&&(int)view["acusesValidos"]==0&&(int)view["perdidas"]==0&&(bool)view["cerrado"],"Native failure fabricates an acknowledgment that was not returned");
        }});
        run("null_or_altered_ack_closes_without_loss_claim",()=>{foreach(var mode in new[]{"start_null","start_bad"})using(var f=new AdapterV2Fixture()){
            f.Disk.Source.Attempt.EjecutorRef="WEB_DOCUMENTO";using(var c=(IDisposable)AckLossControl("INICIO",f.Disk.Source.Prepare(),f.Disk.Source.Lot.Revision)){
            f.Mode=mode;var port=StagingV2(c,"Decorar",f.Protocol);Check(f.Run(port).Estado=="INCIERTO"&&f.Writers==0,"Invalid ack grants invocation");var view=AckLossView(c);Check((int)view["acusesValidos"]==0&&(int)view["perdidas"]==0&&(bool)view["cerrado"],"Invalid ack presented as committed loss");
        }}});
        run("different_command_is_rejected_before_native_callback",()=>{using(var f=new SqlV2Fixture())using(var c=(IDisposable)AckLossControl("INICIO",f.Command,f.State.Lot.Revision)){
            f.Flow.Disk.Source.Attempt.SolicitudHash=new string('f',64);RunBinding(f,StorageV2Binding(f.Flow.Adapter,StagingV2(c,"Decorar",f.Protocol),(ctx,id)=>ReceiptClone(f.State.Lot)));Check(f.Writes==0&&f.Flow.Writers==0&&(int)AckLossView(c)["callbacks"]==0,"Foreign command reaches native persistence");
        }});
        run("different_revision_is_rejected_before_native_callback",()=>{using(var f=new SqlV2Fixture())using(var c=(IDisposable)AckLossControl("INICIO",f.Command,f.State.Lot.Revision+1)){
            RunBinding(f,StorageV2Binding(f.Flow.Adapter,StagingV2(c,"Decorar",f.Protocol),(ctx,id)=>ReceiptClone(f.State.Lot)));Check(f.Writes==0&&(int)AckLossView(c)["callbacks"]==0,"Foreign revision reaches native persistence");
        }});
        run("phase_run_and_protected_attempts_are_refused",()=>{using(var f=new SqlV2Fixture()){
            foreach(var phase in new[]{"RESULTADO","iniciar","","OTHER"})Rejected("DOC118_PRUEBA_NO_DISPONIBLE",()=>AckLossControl(phase,f.Command,f.State.Lot.Revision));Rejected("DOC118_PRUEBA_NO_DISPONIBLE",()=>AckLossControl("INICIO",f.Command,0));Rejected("DOC118_PRUEBA_NO_DISPONIBLE",()=>AckLossControl("INICIO",f.Command,f.State.Lot.Revision,"http-client-value"));
            foreach(var id in new[]{"41","53","69"}){f.Flow.Disk.Source.Attempt.IntentoId=id;f.Flow.Disk.Source.FileRecord.IntentoActivoId=id;Rejected("DOC118_PRUEBA_NO_DISPONIBLE",()=>AckLossControl("INICIO",f.Flow.Disk.Source.Prepare(),f.State.Lot.Revision));}Check(f.Writes==0&&f.Flow.Writers==0,"Protected fixture attempt reached native callback");
        }});
        run("lost_ack_is_single_use_and_has_no_result_callback",()=>{using(var f=new SqlV2Fixture())using(var c=(IDisposable)AckLossControl("INICIO",f.Command,f.State.Lot.Revision)){
            var port=StagingV2(c,"Decorar",f.Protocol);Rejected("DOC118_ACUSE_PERDIDO",()=>AckLossInvoke(port,"Iniciar",f.Command,f.Flow.Disk.Source.Data,f.Prepared()));var writes=f.Writes;Rejected("DOC118_PRUEBA_NO_DISPONIBLE",()=>AckLossInvoke(port,"Iniciar",f.Command,f.Flow.Disk.Source.Data,f.Prepared()));Rejected("DOC118_PRUEBA_NO_DISPONIBLE",()=>AckLossInvoke(port,"RegistrarResultado",f.Command,f.Current(),f.Result()));Check(f.Writes==writes&&f.Consolidations==0&&(int)AckLossView(c)["perdidas"]==1,"Lost ack retried native callback/checkpoint");
        }});
        run("out_of_order_callback_closes_without_native_write",()=>{using(var f=new SqlV2Fixture())using(var c=(IDisposable)AckLossControl("OBSERVAR",f.Command,f.State.Lot.Revision)){
            var port=StagingV2(c,"Decorar",f.Protocol);Rejected("DOC118_PRUEBA_NO_DISPONIBLE",()=>AckLossInvoke(port,"AutorizarInvocacion",f.Command,f.Prepared()));Check(f.Writes==0&&(int)AckLossView(c)["callbacks"]==0&&(bool)AckLossView(c)["cerrado"],"Out of order callback reaches persistence");
        }});
        run("dispose_and_redecoration_never_rearm_capability",()=>{using(var f=new SqlV2Fixture())using(var c=(IDisposable)AckLossControl("INICIO",f.Command,f.State.Lot.Revision)){
            var port=StagingV2(c,"Decorar",f.Protocol);Rejected("DOC118_PRUEBA_NO_DISPONIBLE",()=>StagingV2(c,"Decorar",f.Protocol));c.Dispose();Rejected("DOC118_PRUEBA_NO_DISPONIBLE",()=>AckLossInvoke(port,"Iniciar",f.Command,f.Flow.Disk.Source.Data,f.Prepared()));Check(f.Writes==0&&PerfilImportacionJson.Escribir(c)=="{}","Serialized control grants authority");var json=AckLossView(c).ToString();Check(!json.Contains("Token")&&!json.Contains("Huella")&&!json.Contains("DOC118-V2-local")&&!json.Contains(f.Flow.Disk.Path),"Control observation leaks scope");
        }});
        run("parallel_start_has_one_native_callback_and_loss",()=>{using(var f=new SqlV2Fixture())using(var c=(IDisposable)AckLossControl("INICIO",f.Command,f.State.Lot.Revision)){
            var port=StagingV2(c,"Decorar",f.Protocol);int lost=0,blocked=0;Parallel.For(0,2,n=>{try{AckLossInvoke(port,"Iniciar",f.Command,f.Flow.Disk.Source.Data,f.Prepared());}catch(PerfilImportacionException e){if(e.Codigo=="DOC118_ACUSE_PERDIDO")System.Threading.Interlocked.Increment(ref lost);else if(e.Codigo=="DOC118_PRUEBA_NO_DISPONIBLE")System.Threading.Interlocked.Increment(ref blocked);else throw;}});Check(lost==1&&blocked==1&&f.Commits==1&&f.Flow.Writers==0&&(int)AckLossView(c)["callbacks"]==1&&(int)AckLossView(c)["perdidas"]==1,"Parallel start duplicates callback/loss");
        }});
        Check(cases==12,"Ack loss count changed");Console.WriteLine("DOC-118 V2 acknowledgment loss cases: 12 passed; test-only decorator; native core and real guard; SQL and writer doubles");
    }
}
