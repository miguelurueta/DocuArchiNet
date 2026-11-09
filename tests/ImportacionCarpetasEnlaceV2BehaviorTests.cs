using System;
using System.Linq;
using System.Reflection;
using System.Web;
using Newtonsoft.Json.Linq;

public static partial class ImportacionCarpetasBehaviorTests {
    static IAlmacenamientoDocumentoImportacion StorageV2Binding(AdaptadorDocumentoImportacion adapter,object persistence,Func<ContextoImportacionCarpetas,string,LoteImportacionCarpetas> lot){
        try{return (IAlmacenamientoDocumentoImportacion)typeof(AdaptadorDocumentoImportacion).GetMethod("EnlazarV2",BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,new object[]{adapter,persistence,lot});}catch(TargetInvocationException e){throw e.InnerException;}
    }
    static ResultadoEfectoImportacion RunBinding(SqlV2Fixture f,IAlmacenamientoDocumentoImportacion binding){var s=f.Flow.Disk.Source;return binding.Almacenar(s.Lot.Contexto,s.Root,s.FileRecord,s.Lot.Snapshot,s.Attempt);}
    static void BindingV2Cases(){
        int cases=0;Action<string,Action> run=(name,action)=>{action();cases++;Console.WriteLine("DOC-118 binding V2 case: "+name+": passed");};
        run("typed_binding_uses_current_server_lot_revision",()=>{using(var f=new SqlV2Fixture()){
            f.State.Lot.Revision=30;int reads=0;var binding=StorageV2Binding(f.Flow.Adapter,f.Protocol,(context,id)=>{reads++;Check(id=="20"&&context.UsuarioRef==f.State.Lot.Contexto.UsuarioRef,"Binding loses server scope");return ReceiptClone(f.State.Lot);});var result=RunBinding(f,binding);
            var initial=JObject.Parse((string)f.State.Journal.Rows[1]["detalle_doc"]);Check((uint)initial["Inicio"]["RevisionLote"]==30&&reads==1&&result.Estado=="CONFIRMADO"&&f.Flow.Writers==1&&f.Commits==4&&f.Consolidations==1,"Binding uses stale/client revision or misses V2 protocol");
        }});
        run("null_or_foreign_lot_is_rejected_before_start",()=>{foreach(var mode in new[]{"null","id","user","scope","level","seal","path","snapshot","revision","overflow","stop","state"})using(var f=new SqlV2Fixture()){
            var lot=ReceiptClone(f.State.Lot);if(mode=="id")lot.LoteId="999";if(mode=="user")lot.Contexto.UsuarioRef="999";if(mode=="scope")lot.Contexto.AmbitoRef="999";if(mode=="level")lot.Contexto.NivelRef="999";if(mode=="seal")lot.Contexto.SelloDestino=new string('f',64);if(mode=="path")lot.Contexto.RutaCanonica="changed";if(mode=="snapshot")lot.Snapshot.Huella=new string('f',64);if(mode=="revision")lot.Revision=0;if(mode=="overflow")lot.Revision=UInt32.MaxValue;if(mode=="stop")lot.DetencionSolicitada=true;if(mode=="state")lot.Estado="COMPLETADO";
            var binding=StorageV2Binding(f.Flow.Adapter,f.Protocol,(c,id)=>mode=="null"?null:lot);Rejected("CONFLICTO_REVISION",()=>RunBinding(f,binding));Check(f.Writes==0&&f.Flow.Writers==0&&f.Flow.Disk.Source.Attempt.EsNuevaReserva,"Invalid binding reaches start/writer");
        }});
        run("authority_denied_before_server_lot_read",()=>{using(var f=new SqlV2Fixture()){
            int reads=0;f.Flow.OnPhase=p=>{if(p=="authorize")throw new PerfilImportacionException("ACCESO_DENEGADO","synthetic");};var binding=StorageV2Binding(f.Flow.Adapter,f.Protocol,(c,id)=>{reads++;return f.State.Lot;});Rejected("ACCESO_DENEGADO",()=>RunBinding(f,binding));Check(reads==0&&f.Writes==0&&f.Flow.Writers==0,"Binding reads lot before authority");
        }});
        run("server_lot_change_after_read_does_not_start",()=>{using(var f=new SqlV2Fixture()){
            var binding=StorageV2Binding(f.Flow.Adapter,f.Protocol,(c,id)=>{var before=ReceiptClone(f.State.Lot);f.State.Lot.Revision++;return before;});var result=RunBinding(f,binding);Check(result.Estado=="INCIERTO"&&f.Writes==0&&f.Flow.Writers==0,"Binding bypasses repository revision CAS");
        }});
        run("missing_persistence_or_reader_has_no_fallback",()=>{using(var f=new SqlV2Fixture()){
            foreach(var mode in new[]{"adapter","persistence","reader"}){bool blocked=false;try{StorageV2Binding(mode=="adapter"?null:f.Flow.Adapter,mode=="persistence"?null:f.Protocol,mode=="reader"?null:new Func<ContextoImportacionCarpetas,string,LoteImportacionCarpetas>((c,id)=>f.State.Lot));}catch(ArgumentNullException){blocked=true;}Check(blocked,"Binding accepts missing capability");}Check(f.Writes==0&&f.Flow.Writers==0,"Missing capability reaches V1");
        }});
        run("lost_start_ack_consumes_no_writer",()=>{using(var f=new SqlV2Fixture()){
            f.LostResponseStep="INICIO";var binding=StorageV2Binding(f.Flow.Adapter,f.Protocol,(c,id)=>ReceiptClone(f.State.Lot));var result=RunBinding(f,binding);Check(result.Estado=="INCIERTO"&&f.Flow.Writers==0&&f.Commits==1&&f.State.Attempt.Estado=="EN_CURSO","Binding retries lost start");
        }});
        run("lost_gate_ack_never_grants_invocation",()=>{using(var f=new SqlV2Fixture()){
            f.LostResponseStep="AUTORIZAR";var result=RunBinding(f,StorageV2Binding(f.Flow.Adapter,f.Protocol,(c,id)=>ReceiptClone(f.State.Lot)));Check(result.Estado=="INCIERTO"&&f.Flow.Writers==0&&f.Commits==2,"Binding invokes after lost gate");
        }});
        run("lost_checkpoint_response_preserves_ids_and_single_writer",()=>{using(var f=new SqlV2Fixture()){
            f.LostResponseStep="RESULTADO";var result=RunBinding(f,StorageV2Binding(f.Flow.Adapter,f.Protocol,(c,id)=>ReceiptClone(f.State.Lot)));Check(result.Estado=="INCIERTO"&&result.DocumentoId=="90"&&f.State.Attempt.Estado=="CONFIRMADO"&&f.Flow.Writers==1&&f.Consolidations==1,"Binding loses checkpoint IDs or retries writer");
        }});
        run("same_adapter_instance_cannot_write_twice",()=>{using(var f=new SqlV2Fixture()){
            var binding=StorageV2Binding(f.Flow.Adapter,f.Protocol,(c,id)=>ReceiptClone(f.State.Lot));RunBinding(f,binding);bool blocked=false;try{RunBinding(f,binding);}catch(PerfilImportacionException){blocked=true;}Check(blocked&&f.Flow.Writers==1&&f.Commits==4,"Binding replays writer");
        }});
        run("v1_history_returns_before_new_writer_factory",()=>{using(var f=new DocumentPhaseFixture()){
            var first=f.Run();Check(first.Resultado.Estado=="CONFIRMADO","V1 baseline not confirmed");var before=f.Source.Writers;int factories=0;
            var fresh=new CoordinadorImportacionCarpetas(()=>ReceiptClone(f.Source.Lot.Contexto),f,n=>ReceiptClone(f.Source.Lot.Contexto),(c,cfg)=>Check(f.Allowed,"V1 replay lacks authority"),
                (c,l,r)=>f.View(),(c,l,id)=>ReceiptClone(f.Source.FileRecord),(c,request)=>f.Source.Attempt.SolicitudHash==request.SolicitudHash?ReceiptClone(f.Source.Attempt):null,
                (l,a)=>{throw new Exception("V1 history constructs staging");},(IAlmacenStagingImportacion store)=>{factories++;throw new Exception("V1 history constructs V2 writer");});
            var replay=fresh.AlmacenarArchivo("20","30",f.OriginalRevision,"stable-document");Check(replay.Repeticion&&replay.Resultado.DocumentoId==first.Resultado.DocumentoId&&factories==0&&f.Source.Writers==before&&f.Reservations==1,"V1 history replays storage");
        }});
        run("production_composition_and_dispatcher_are_compiled",()=>{
            Rejected("SESION_REQUERIDA",()=>ComposicionDocumentoImportacion.Crear(null));var dispatcher=typeof(WebServiceImportacionCarpetas).GetMethod("CrearCoordinacionOperativa",BindingFlags.NonPublic|BindingFlags.Static);bool reached=false;
            try{dispatcher.Invoke(null,new object[]{null,new ResourceFixture().Adapter,"ALMACENAR"});}catch(TargetInvocationException e){var failure=e.InnerException as PerfilImportacionException;reached=failure!=null&&failure.Codigo=="SESION_REQUERIDA";}Check(reached,"Dispatcher still uses documentary factory stub");
        });
        run("binding_retains_original_before_native_v2_start",()=>{using(var f=new SqlV2Fixture()){
            var protocol=MatrixV2Protocol(f,"start",()=>Rejected("OPERACION_EN_CURSO",()=>f.Flow.Disk.Guard(f.Current())));var result=RunBinding(f,StorageV2Binding(f.Flow.Adapter,protocol,(c,id)=>ReceiptClone(f.State.Lot)));Check(result.Estado=="CONFIRMADO"&&f.Flow.Writers==1,"Binding does not retain before start");
        }});
        Check(cases==12,"Binding V2 count changed");Console.WriteLine("DOC-118 V2 storage binding cases: 12 passed; production composition compiled; server revision; SQL and writer doubles");
    }
}
