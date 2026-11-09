using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;

public static partial class ImportacionCarpetasBehaviorTests {
    static object RecoveryCall(string name,params object[] args){
        try{return typeof(CoordinadorImportacionCarpetas).GetMethod(name,BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,args);}
        catch(TargetInvocationException e){throw e.InnerException;}
    }
    static object RecoveryRepo(string name,params object[] args){
        try{return typeof(MySqlImportacionCarpetasRepository).GetMethod(name,BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,args);}
        catch(TargetInvocationException e){throw e.InnerException;}
    }
    sealed class RecoveryV2Fixture:IDisposable {
        public readonly SqlV2Fixture Sql=new SqlV2Fixture();public readonly CoordinadorImportacionCarpetas Service;public readonly object Port;public PlanImportacionCarpetas Plan;
        public bool Allowed=true,Physical=true,LostResponse,FailConsolidation;public int Reads,Guards,Checks,AbsenceReads,Saves,Writes,Consolidations,FailAt;public Action BeforeSave,AfterRead;public object LastGuard;readonly object sync=new object();
        public RecoveryV2Fixture(bool positive=false){
            Sql.Begin();if(positive){Sql.Gate();Sql.Observe();}
            var s=Sql.State;Plan=new PlanImportacionCarpetas{PlanId=s.Lot.PlanId,Estado="CONFIRMADO",Contexto=ReceiptClone(s.Lot.Contexto),Snapshot=ReceiptClone(s.Lot.Snapshot),Manifiesto=new ManifiestoImportacionCarpetas(),Resultado=new ResultadoPreparacionImportacion{PuedeConfirmar=true}};
            Plan.Manifiesto.Raices.Add(new RaizManifiestoImportacion{IdCliente=s.Root.ClienteRaizId,Nombre=s.Root.NombreOriginal,SeleccionId="s1"});
            Plan.Manifiesto.Archivos.Add(new ArchivoManifiestoImportacion{IdCliente=s.File.ClienteArchivoId,RaizIdCliente=s.Root.ClienteRaizId,Nombre=s.File.NombreOriginal,RutaRelativa=s.File.RutaRelativa,Bytes=s.File.BytesPrevistos,UltimaModificacionLocal=s.File.UltimaModificacionLocal});
            Plan.Resultado.Elementos.Add(new ValoresElementoImportacion{Ambito="RAIZ",RaizIdCliente=s.Root.ClienteRaizId,Valores=ReceiptClone(s.Root.ValoresPlan)});
            Plan.Resultado.Elementos.Add(new ValoresElementoImportacion{Ambito="ARCHIVO",RaizIdCliente=s.Root.ClienteRaizId,ArchivoIdCliente=s.File.ClienteArchivoId,Valores=ReceiptClone(s.File.ValoresPlan)});Plan.Resultado.UnidadesElegibles.Add(s.Root.ClienteRaizId);
            var repository=new ControlRepository{Lot=ReceiptClone(s.Lot)};
            Service=new CoordinadorImportacionCarpetas(()=>ReceiptClone(Sql.State.Lot.Contexto),repository,level=>ReceiptClone(Sql.State.Lot.Contexto),(c,cfg)=>Authorize());
            var ctor=typeof(PerfilImportacionJson).Assembly.GetType("RecuperacionDocumentoV2Importacion",true).GetConstructors(BindingFlags.NonPublic|BindingFlags.Instance).Single();
            Func<object[],object>[] handlers={
                a=>{Reads++;var view=View();if(AfterRead!=null)AfterRead();return view;},
                a=>{Guards++;var op=Get(a[0],"Operacion");var store=new AlmacenStagingImportacion(Sql.Flow.Disk.Source.Stage.Receipt.Root,Sql.Flow.Disk.Source.Stage.Receipt.Limits,(LoteImportacionCarpetas)Get(op,"Lote"),(ArchivoImportacionCarpetas)Get(op,"Archivo"),10,200);LastGuard=StagingV2(store,"AbrirGuardiaV2",Sql.State.Lot.Contexto,a[1]);return LastGuard;},
                a=>{AbsenceReads++;Authorize();var c=(ComandoAlmacenamientoCarpetas)a[0];Check(c.EsVerificacion,"Recovery absence has writer command");return DocumentCatalogHelper("LeerAusenciaRecuperacionV2",c,Sql.Flow.Disk.Source.Data,new Func<string,string,object[],DataTable>((origin,query,parameters)=>Query(origin,query,parameters)));},
                a=>{Checks++;Authorize();var c=(ComandoAlmacenamientoCarpetas)a[0];Check(c.EsVerificacion,"Recovery verification has writer command");return DocumentCatalogHelper("LeerAlmacenamiento",c,Sql.Flow.Disk.Source.Data,(ResultadoEfectoImportacion)a[1],new Func<string,string,object[],DataTable>((origin,query,parameters)=>Query(origin,query,parameters)));},
                a=>Save(a[0],(ResultadoEfectoImportacion)a[1],a[2])
            };
            Port=ctor.Invoke(ctor.GetParameters().Select((p,i)=>(object)V2Callback(p.ParameterType,handlers[i])).ToArray());
        }
        public static object Get(object target,string name){return target.GetType().GetProperty(name).GetValue(target,null);}
        static void Set(object target,string name,object value){target.GetType().GetProperty(name).SetValue(target,value,null);}
        public object View(SqlV2State state=null){
            var s=(state??Sql.State).Copy();var op=Activator.CreateInstance(typeof(PerfilImportacionJson).Assembly.GetType("VistaPersistenciaDocumentoV2Importacion",true),true);
            if(!s.Journal.Columns.Contains("evento_id")){s.Journal.Columns.Add("evento_id",typeof(string));s.Journal.Columns.Add("ocurrido_utc",typeof(DateTime));}
            for(int n=0;n<s.Journal.Rows.Count;n++){s.Journal.Rows[n]["evento_id"]=(n+1).ToString(CultureInfo.InvariantCulture);s.Journal.Rows[n]["ocurrido_utc"]=new DateTime(2026,11,9,1,0,n,DateTimeKind.Utc);}
            foreach(var p in new Dictionary<string,object>{{"Lote",s.Lot},{"Raiz",s.Root},{"Archivo",s.File},{"Intento",s.Attempt},{"Evidencia",s.Evidence},{"Diario",s.Journal}})Set(op,p.Key,p.Value);
            var view=Activator.CreateInstance(typeof(PerfilImportacionJson).Assembly.GetType("VistaRecuperacionDocumentoV2Importacion",true),true);Set(view,"Operacion",op);Set(view,"Plan",ReceiptClone(Plan));return view;
        }
        public void Authorize(){if(!Allowed)throw new PerfilImportacionException("ACCESO_DENEGADO","synthetic");}
        DataTable Query(string origin,string sql,object[] parameters){
            Check(sql.StartsWith("SELECT ",StringComparison.Ordinal)&&!sql.Contains("DELETE ")&&!sql.Contains("UPDATE "),"Recovery physical query writes");
            var s=Sql.State;var c=s.Lot.Contexto;var cfg=s.Lot.Snapshot.Configuracion;
            if(origin=="RA")return DocumentRow(new Dictionary<string,object>{{"expediente","60"},{"relacion","70"},{"nivel",c.NivelRef},{"empresa",c.AmbitoRef},{"usuario",c.UsuarioRef},{"nivel_activo","1"},{"electronico","2"},{"empresa_nivel",c.AmbitoRef},{"autorizado","1"},{"fondo",cfg.FondoRef},{"instrumento",cfg.InstrumentoRef},{"area",cfg.AreaRef},{"serie",cfg.SerieRef},{"subserie",DBNull.Value},{"tipo",cfg.TipoExpedienteRef},{"unidad",cfg.UnidadDocumentalRef}});
            if(sql.StartsWith("SELECT COUNT(*)",StringComparison.Ordinal)){Check(sql.Contains("@expediente")&&!sql.Contains("NOMBRE="),"Recovery absence attributes by name");return DocumentRow(new Dictionary<string,object>{{"total",Physical?"0":"1"}});}
            Check(sql.Contains("g.ID=@documento")&&sql.Contains("g.ID_INVENTARIO_DOCUMENTAL=@inventario"),"Recovery positive searches by coincidence");
            return DocumentRow(new Dictionary<string,object>{{"documento",Physical?"90":"999"},{"inventario","100"},{"expediente","60"},{"usuario",c.UsuarioRef},{"nombre",s.File.NombreOriginal},{"gabinete","7"},{"documento_inventario","90"},{"expediente_inventario","60"},{"empresa",c.AmbitoRef},{"usuario_inventario",c.UsuarioRef},{"gabinete_nombre",Sql.Flow.Disk.Source.Data.GabineteNombre},{"tipologia","9"},{"clase","3"},{"segundo_nombre",s.File.NombreOriginal},{"expediente_indice","60"},{"inventario_indice","100"},{"nombre_indice",s.File.NombreOriginal},{"valor_0","0007"},{"tipo_archivo","6"},{"formato_inventario",".PDF"},{"formato_indice",".PDF"},{"nombre_configuraciones","1"},{"nombre_visible","1"}});
        }
        public ResultadoReconciliacionImportacion Run(uint? lot=null,uint? file=null){return (ResultadoReconciliacionImportacion)StagingV2(Service,"RecuperarDocumentoV2",Sql.State.Lot.LoteId,Sql.State.Attempt.IntentoId,lot??Sql.State.Lot.Revision,file??Sql.State.File.Revision,Port);}
        ResultadoEfectoImportacion Save(object expected,ResultadoEfectoImportacion result,object guard){
            Saves++;if(BeforeSave!=null)BeforeSave();ResultadoEfectoImportacion output=null;
            StagingV2(guard,"ConservarDurante",new Action(()=>{
                lock(sync){Authorize();var pending=Sql.State.Copy();var current=View(pending);int writes=0,consolidations=0;
                    Func<string,object[],int> execute=(sql,values)=>{
                        SharingV2(()=>{using(var reader=new FileStream(Sql.Flow.Disk.Path,FileMode.Open,FileAccess.Read,FileShare.Read)){} });
                        writes++;Check(sql.StartsWith("UPDATE ra_integracion_",StringComparison.Ordinal)||sql.StartsWith("INSERT INTO ra_integracion_evento",StringComparison.Ordinal),"Recovery SQL touches business storage");
                        var p=new Dictionary<string,object>();for(int n=0;n<values.Length;n+=2)p[(string)values[n]]=values[n+1];Func<string,string> text=k=>p[k]==DBNull.Value?null:Convert.ToString(p[k],CultureInfo.InvariantCulture);
                        if(sql.StartsWith("UPDATE ra_integracion_intento",StringComparison.Ordinal)){
                            Check(sql.Contains("evidencia_doc=@anterior")&&sql.Contains("token_reclamacion=@token")&&sql.Contains("solicitud_hash=@hash")&&text("@anterior")==pending.Evidence&&text("@token")==pending.Attempt.TokenReclamacion,"Recovery attempt CAS missing");
                            pending.Attempt.Estado=text("@estado");pending.Evidence=text("@evidencia");pending.Attempt.FinUtc=((DateTime)p["@ahora"]).ToString("o");pending.Attempt.Resultado=PerfilImportacionJson.Leer<ResultadoEfectoImportacion>(pending.Evidence);
                        }else if(sql.StartsWith("UPDATE ra_integracion_archivo",StringComparison.Ordinal)){
                            Check(text("@intento")==pending.File.IntentoActivoId&&sql.Contains("resultado_doc IS NULL"),"Recovery erases another claim");pending.File.EstadoEfecto=text("@estado");pending.File.Resultado=p["@resultado"]==DBNull.Value?null:PerfilImportacionJson.Leer<ResultadoEfectoImportacion>(text("@resultado"));pending.File.IntentoActivoId=null;pending.File.Revision++;
                        }else if(sql.StartsWith("UPDATE ra_integracion_lote",StringComparison.Ordinal)){Check(!sql.Contains("detencion_solicitada=")||sql.Contains("AND detencion_solicitada=0"),"Recovery changes stop signal");pending.Lot.Revision++;
                        }else{pending.Journal.Rows.Add("RECUPERAR_DOCUMENTO_V2",text("@codigo"),text("@actor"),"ARCHIVO",text("@archivo"),text("@lote"),text("@intento"),text("@detalle"));}
                        if(FailAt==writes)throw new InvalidOperationException("SYNTHETIC_RECOVERY_WRITE_FAILURE");return 1;
                    };
                    Action consolidate=()=>{consolidations++;if(FailConsolidation)throw new InvalidOperationException("SYNTHETIC_RECOVERY_CONSOLIDATION_FAILURE");};
                    output=(ResultadoEfectoImportacion)RecoveryRepo("PersistirRecuperacionDocumentoV2",expected,current,result,guard,new DateTime(2026,11,9,1,1,0,DateTimeKind.Utc),execute,consolidate);
                    Sql.State=pending;Writes+=writes;Consolidations+=consolidations;
                }
            }));
            if(LostResponse)throw new IOException("SYNTHETIC_RECOVERY_RESPONSE_LOSS");return output;
        }
        public void Dispose(){Check(Sql.Flow.Writers==0,"Recovery invoked writer");Sql.Dispose();}
    }
    static void RecoveryV2Cases(){
        int cases=0;Action<string,Action> run=(name,action)=>{action();cases++;Console.WriteLine("DOC-118 recovery V2 case: "+name+": passed");};
        run("prepared_closes_control_preserves_original_and_inventory",()=>{using(var f=new RecoveryV2Fixture()){
            var s=f.Sql.State;var bytes=File.ReadAllBytes(f.Sql.Flow.Disk.Path);var values=PerfilImportacionJson.Escribir(s.File.ValoresEfectivos);var r=f.Run();s=f.Sql.State;
            Check(r.Resuelta&&!r.Repeticion&&r.Resultado.Estado=="RECHAZADO"&&r.Resultado.Codigo=="EJECUCION_CERRADA_SIN_INVOCACION"&&r.Resultado.DocumentoId==null&&r.Resultado.InventarioId==null&&s.File.EstadoEfecto=="NO_INICIADO"&&s.File.IntentoActivoId==null&&s.File.Resultado==null,"Recovery negative claims documentary success");
            Check(s.File.EstadoRecepcion=="RECIBIDO"&&s.Root.IdentidadExpediente.ExpedienteId=="60"&&s.Lot.TotalArchivos==1&&PerfilImportacionJson.Escribir(s.File.ValoresEfectivos)==values&&File.ReadAllBytes(f.Sql.Flow.Disk.Path).SequenceEqual(bytes)&&f.Consolidations==1,"Recovery loses original/inventory");
        }});
        run("observed_complete_ids_confirm_without_writer",()=>{using(var f=new RecoveryV2Fixture(true)){var r=f.Run();Check(r.Resuelta&&r.Resultado.Estado=="CONFIRMADO"&&r.Resultado.DocumentoId=="90"&&r.Resultado.InventarioId=="100"&&f.Sql.State.File.EstadoEfecto=="COMPLETADO"&&f.Sql.State.Attempt.Estado=="CONFIRMADO"&&f.Checks==1&&f.AbsenceReads==0&&f.Consolidations==1,"Recovery positive not exact");}});
        run("gate_without_return_remains_blocked",()=>{using(var f=new RecoveryV2Fixture()){f.Sql.Gate();var before=f.Sql.State.Snapshot();Rejected("CONFLICTO_REVISION",()=>f.Run());Check(f.Writes==0&&f.Sql.State.Snapshot()==before,"Recovery gate-only reset");}});
        run("partial_observed_ids_remain_blocked",()=>{using(var f=new RecoveryV2Fixture()){f.Sql.Gate();f.Sql.Observe(true);Rejected("CONFLICTO_REVISION",()=>f.Run());Check(f.Guards==0&&f.Writes==0,"Recovery fills missing ID");}});
        run("v1_and_synthetic_69_are_not_backfilled",()=>{using(var f=new RecoveryV2Fixture()){
            f.Sql.State.Attempt.IntentoId="69";f.Sql.State.File.IntentoActivoId="69";f.Sql.State.Evidence=PerfilImportacionJson.Escribir(RepositoryHelper("EvidenciaInicialDocumento",f.Sql.Command));f.Sql.State.Attempt.Resultado=PerfilImportacionJson.Leer<ResultadoEfectoImportacion>(f.Sql.State.Evidence);
            bool blocked=false;try{f.Run();}catch(PerfilImportacionException){blocked=true;}Check(blocked&&f.Writes==0&&f.Guards==0,"Recovery backfills69/V1");
        }});
        run("active_retention_blocks_recovery",()=>{using(var f=new RecoveryV2Fixture())using(var retained=(IDisposable)StagingV2(f.Sql.Flow.Disk.Store,"ConservarOriginalV2",f.Sql.State.Lot.Contexto)){Rejected("OPERACION_EN_CURSO",()=>f.Run());Check(f.Writes==0,"Recovery ran against live executor");}});
        run("original_missing_or_replaced_is_not_recreated",()=>{foreach(var replace in new[]{false,true})using(var f=new RecoveryV2Fixture()){var path=f.Sql.Flow.Disk.Path;File.Move(path,path+".retained");if(replace)File.Copy(path+".retained",path);Rejected("RESULTADO_INCIERTO",()=>f.Run());Check(f.Writes==0&&File.Exists(path)==replace,"Recovery recreates or accepts changed original");}});
        run("negative_nonempty_expedition_and_positive_mismatch_block",()=>{foreach(var positive in new[]{false,true})using(var f=new RecoveryV2Fixture(positive)){f.Physical=false;bool rejected=false;try{f.Run();}catch(PerfilImportacionException){rejected=true;}Check(rejected&&f.Writes==0,"Recovery accepts discrepant physical evidence");}});
        run("stop_blocks_negative_and_preserves_positive",()=>{
            using(var f=new RecoveryV2Fixture()){f.Sql.State.Lot.DetencionSolicitada=true;Rejected("CONFLICTO_REVISION",()=>f.Run());Check(f.Writes==0,"Stopped negative closed");}
            using(var f=new RecoveryV2Fixture(true)){f.Sql.State.Lot.DetencionSolicitada=true;Check(f.Run().Resultado.Estado=="CONFIRMADO"&&f.Sql.State.Lot.DetencionSolicitada,"Recovery positive clears stop");}
        });
        run("stale_client_revisions_do_not_reach_guard",()=>{using(var f=new RecoveryV2Fixture()){bool denied=false;try{f.Run(f.Sql.State.Lot.Revision+1);}catch(PerfilImportacionException){denied=true;}Check(denied&&f.Guards==0&&f.Writes==0,"Recovery stale revision reaches effects");}});
        run("authority_denied_before_read_or_commit",()=>{
            using(var f=new RecoveryV2Fixture()){f.Allowed=false;Rejected("ACCESO_DENEGADO",()=>f.Run());Check(f.Writes==0&&f.Guards==0,"Unauthorized recovery opens guard");}
            using(var f=new RecoveryV2Fixture()){f.BeforeSave=()=>f.Allowed=false;Rejected("ACCESO_DENEGADO",()=>f.Run());Check(f.Writes==0,"Revoked recovery committed");}
        });
        run("snapshot_changes_before_commit_are_rejected",()=>{foreach(var mode in new[]{"lot","file","token","journal","plan"})using(var f=new RecoveryV2Fixture()){
            f.BeforeSave=()=>{if(mode=="lot")f.Sql.State.Lot.Revision++;if(mode=="file")f.Sql.State.File.Revision++;if(mode=="token")f.Sql.State.Attempt.TokenReclamacion=new string('f',32);if(mode=="journal")f.Sql.State.Journal.Rows[1]["codigo"]="OTHER";if(mode=="plan")f.Plan.Manifiesto.Archivos[0].UltimaModificacionLocal="changed";};
            Rejected("CONFLICTO_REVISION",()=>f.Run());Check(f.Writes==0,"Recovery stale snapshot writes");
        }});
        run("changed_claim_does_not_get_removed",()=>{using(var f=new RecoveryV2Fixture()){f.BeforeSave=()=>f.Sql.State.File.IntentoActivoId="999";Rejected("CONFLICTO_REVISION",()=>f.Run());Check(f.Sql.State.File.IntentoActivoId=="999"&&f.Writes==0,"Recovery erases new claim");}});
        run("rollback_on_every_write_and_consolidation",()=>{foreach(var positive in new[]{false,true})foreach(var failure in new[]{1,2,3,4,5})using(var f=new RecoveryV2Fixture(positive)){
            var before=f.Sql.State.Snapshot();if(failure==5)f.FailConsolidation=true;else f.FailAt=failure;bool failed=false;try{f.Run();}catch(InvalidOperationException){failed=true;}Check(failed&&f.Sql.State.Snapshot()==before&&f.Writes==0,"Recovery partial commit");
        }});
        run("guard_survives_writes_and_lost_guard_prevents_commit",()=>{
            using(var f=new RecoveryV2Fixture()){f.BeforeSave=()=>{SharingV2(()=>{using(var reader=new FileStream(f.Sql.Flow.Disk.Path,FileMode.Open,FileAccess.Read,FileShare.Read)){} });};f.Run();}
            using(var f=new RecoveryV2Fixture()){f.BeforeSave=()=>((IDisposable)f.LastGuard).Dispose();Rejected("OPERACION_EN_CURSO",()=>f.Run());Check(f.Writes==0,"Recovery committed after guard lost");}
        });
        run("replay_is_history_even_after_new_claim",()=>{using(var f=new RecoveryV2Fixture()){
            var first=f.Run();var writes=f.Writes;var guards=f.Guards;f.Sql.State.File.IntentoActivoId="999";f.Sql.State.File.EstadoEfecto="EN_CURSO";f.Sql.State.File.Revision++;
            var before=f.Sql.State.Snapshot();var again=f.Run(1,1);Check(again.Repeticion&&again.Resultado.Codigo==first.Resultado.Codigo&&f.Writes==writes&&f.Guards==guards&&f.Sql.State.Snapshot()==before,"Recovery replay changes later claim");
        }});
        run("lost_response_then_retry_returns_same_history",()=>{foreach(var positive in new[]{false,true})using(var f=new RecoveryV2Fixture(positive)){f.LostResponse=true;bool lost=false;try{f.Run();}catch(IOException){lost=true;}Check(lost,"Recovery response loss not exercised");f.LostResponse=false;var count=f.Writes;var again=f.Run(1,1);Check(again.Repeticion&&f.Writes==count&&f.Consolidations==1,"Recovery response loss repeats writes");}});
        run("existing_checkpoint_returns_history_without_guard",()=>{using(var f=new RecoveryV2Fixture(true)){f.Sql.Step("RESULTADO",f.Sql.Current(),f.Sql.Result());var r=f.Run(1,1);Check(r.Repeticion&&r.Resultado.Estado=="CONFIRMADO"&&f.Guards==0&&f.Writes==0,"Recovery repeats checkpoint");}});
        run("verification_command_rejects_storage_ports",()=>{using(var f=new RecoveryV2Fixture(true)){
            var view=f.View();var document=RecoveryRepo("ClasificarRecuperacionV2",view);var s=f.Sql.State;var method=typeof(ComandoAlmacenamientoCarpetas).GetMethod("PrepararRecuperacionV2",BindingFlags.NonPublic|BindingFlags.Static);var c=(ComandoAlmacenamientoCarpetas)method.Invoke(null,new object[]{s.Lot.Contexto,s.Root,s.File,s.Lot.Snapshot,s.Attempt,document});Check(c.EsVerificacion&&c.Intento.Estado=="EN_CURSO"&&c.Archivo.EstadoEfecto=="EN_CURSO","Recovery fakes states");
            var repo=new MySqlImportacionCarpetasRepository("synthetic-no-connection");Rejected("SOLICITUD_INVALIDA",()=>repo.IniciarAlmacenamientoDocumento(c,f.Sql.Flow.Disk.Source.Data));Rejected("SOLICITUD_INVALIDA",()=>repo.RegistrarResultadoDocumento(c,f.Sql.Result()));
        }});
        run("outcomes_do_not_expose_protocol_or_paths",()=>{foreach(var positive in new[]{false,true})using(var f=new RecoveryV2Fixture(positive)){var json=PerfilImportacionJson.Escribir(f.Run());Check(!json.Contains("NodoRef")&&!json.Contains("TokenReclamacion")&&!json.Contains("IdentidadFisicaHash")&&!json.Contains(f.Sql.Flow.Disk.Path),"Recovery leaks proof");}});
        run("another_epoch_is_not_qualified_for_recovery",()=>{using(var f=new RecoveryV2Fixture()){
            var j=JObject.Parse(f.Sql.State.Evidence);j["Inicio"]["Original"]["NodoRef"]=new string('f',32);f.Sql.State.Evidence=V2Json(V2("Leer",j.ToString()));f.Sql.State.Journal.Rows[1]["detalle_doc"]=f.Sql.State.Evidence;
            Rejected("CAPACIDAD_NO_DISPONIBLE",()=>f.Run());Check(f.Writes==0&&f.AbsenceReads==0,"Recovery trusts another epoch");
        }});
        run("corrupted_or_reordered_history_is_rejected",()=>{foreach(var mode in new[]{"actor","order"})using(var f=new RecoveryV2Fixture()){
            f.Run();if(mode=="actor")f.Sql.State.Journal.Rows[f.Sql.State.Journal.Rows.Count-1]["actor_ref"]="999";
            else{var row=f.Sql.State.Journal.Rows[0].ItemArray;f.Sql.State.Journal.Rows.RemoveAt(0);f.Sql.State.Journal.Rows.Add(row);}
            var writes=f.Writes;Rejected("CONFLICTO_REVISION",()=>f.Run(1,1));Check(f.Writes==writes,"Recovery corrupt history writes");
        }});
        Check(cases==22,"Recovery V2 case count changed");Console.WriteLine("DOC-118 V2 recovery behavior cases: 22 passed; real guard; SELECT and transaction doubles; zero writer");
    }
}
