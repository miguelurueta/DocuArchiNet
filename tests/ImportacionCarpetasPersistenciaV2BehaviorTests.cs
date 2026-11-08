using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

public static partial class ImportacionCarpetasBehaviorTests {
    sealed class SqlV2State {
        public LoteImportacionCarpetas Lot;public RaizImportacionCarpetas Root;public ArchivoImportacionCarpetas File;public IntentoImportacionCarpetas Attempt;public string Evidence;public DataTable Journal;
        public SqlV2State Copy(){return new SqlV2State{Lot=ReceiptClone(Lot),Root=ReceiptClone(Root),File=ReceiptClone(File),Attempt=ReceiptClone(Attempt),Evidence=Evidence,Journal=Journal.Copy()};}
        public string Snapshot(){return PerfilImportacionJson.Escribir(new {Lot,Root,File,Attempt,Evidence,Events=Journal.Rows.Cast<DataRow>().Select(r=>String.Join("|",r.ItemArray.Select(v=>Convert.ToString(v,CultureInfo.InvariantCulture)))).ToArray()});}
    }
    sealed class SqlV2Fixture:IDisposable {
        public readonly AdapterV2Fixture Flow=new AdapterV2Fixture();public readonly ComandoAlmacenamientoCarpetas Command;public readonly object Protocol;public SqlV2State State;
        public int Writes,Commits,Consolidations,FailAt;public bool FailConsolidation,ReturnZeroAtFailure;public string LostResponseStep;public Exception LastFailure;readonly object sync=new object();
        public SqlV2Fixture(){
            Flow.Disk.Source.Attempt.EjecutorRef="WEB_DOCUMENTO";Command=Flow.Disk.Source.Prepare();
            State=new SqlV2State{Lot=ReceiptClone(Flow.Disk.Source.Lot),Root=ReceiptClone(Flow.Disk.Source.Root),File=ReceiptClone(Flow.Disk.Source.FileRecord),Attempt=ReceiptClone(Flow.Disk.Source.Attempt),Journal=new DataTable()};
            foreach(var name in new[]{"accion","codigo","actor_ref","recurso_tipo","recurso_id","lote_id","intento_id","detalle_doc"})State.Journal.Columns.Add(name,typeof(string));
            AddEvent(State,"RESERVAR_INTENTO","INTENTO_RESERVADO","{}");
            var ctor=typeof(PerfilImportacionJson).Assembly.GetType("PersistenciaDocumentoV2Importacion",true).GetConstructors(BindingFlags.NonPublic|BindingFlags.Instance).Single();
            Func<object[],object>[] handlers={a=>V2Ack(V2("Leer",Step("INICIO",a[2]))),a=>V2Ack(V2("Leer",Step("AUTORIZAR",a[1]))),a=>V2Ack(V2("Leer",Step("OBSERVAR",a[1]))),a=>PerfilImportacionJson.Leer<ResultadoEfectoImportacion>(Step("RESULTADO",a[1],(ResultadoEfectoImportacion)a[2]))};
            Protocol=ctor.Invoke(ctor.GetParameters().Select((p,i)=>(object)V2Callback(p.ParameterType,handlers[i])).ToArray());
        }
        static void AddEvent(SqlV2State s,string action,string code,string detail){s.Journal.Rows.Add(action,code,s.Lot.Contexto.UsuarioRef,"ARCHIVO",s.File.ArchivoId,s.Lot.LoteId,s.Attempt.IntentoId,detail);}
        public object Prepared(){return Flow.Disk.Document;}
        public object Current(){return V2("Leer",State.Evidence);}
        public string Step(string step,object doc,ResultadoEfectoImportacion result=null){
            lock(sync){
                var pending=State.Copy();var type=typeof(PerfilImportacionJson).Assembly.GetType("VistaPersistenciaDocumentoV2Importacion",true);var view=Activator.CreateInstance(type,true);
                foreach(var pair in new Dictionary<string,object>{{"Lote",pending.Lot},{"Raiz",pending.Root},{"Archivo",pending.File},{"Intento",pending.Attempt},{"Evidencia",pending.Evidence},{"Diario",pending.Journal}})type.GetProperty(pair.Key).SetValue(view,pair.Value,null);
                int localWrites=0,localConsolidations=0;
                Func<string,object[],int> execute=(sql,values)=>{
                    localWrites++;var p=new Dictionary<string,object>();for(int i=0;i<values.Length;i+=2)p.Add((string)values[i],values[i+1]);
                    if(FailAt==localWrites&&ReturnZeroAtFailure)return 0;
                    Func<string,string> text=k=>p[k]==DBNull.Value?null:Convert.ToString(p[k],CultureInfo.InvariantCulture);
                    Check(!sql.Contains("; ")&&(sql.Contains("@lote")||(sql.Contains("@archivo")&&sql.Contains("@raiz"))),"V2 SQL not scoped/parameterized");
                    if(sql.StartsWith("UPDATE ra_integracion_intento",StringComparison.Ordinal)){
                        Check(sql.Contains("evidencia_doc <=> @anterior")&&sql.Contains("token_reclamacion=@token")&&sql.Contains("solicitud_hash=@hash"),"V2 CAS lacks original evidence/token/hash");
                        Check(text("@anterior")==pending.Evidence&&text("@anteriorEstado")==pending.Attempt.Estado&&text("@token")==pending.Attempt.TokenReclamacion,"V2 wrong attempt CAS");
                        pending.Attempt.Estado=text("@estado");pending.Evidence=text("@evidencia");pending.Attempt.FinUtc=p["@fin"]==DBNull.Value?null:((DateTime)p["@fin"]).ToString("o");
                        pending.Attempt.Resultado=(ResultadoEfectoImportacion)RepositoryHelper("LeerEvidenciaDocumentalCompatible",pending.Evidence,pending.Attempt);
                    }else if(sql.StartsWith("UPDATE ra_integracion_archivo",StringComparison.Ordinal)){
                        Check(text("@intento")==pending.File.IntentoActivoId,"V2 erases another active claim");pending.File.EstadoEfecto=text("@estado");pending.File.Resultado=PerfilImportacionJson.Leer<ResultadoEfectoImportacion>(text("@resultado"));pending.File.IntentoActivoId=text("@activo");pending.File.Revision++;
                    }else if(sql.StartsWith("UPDATE ra_integracion_lote",StringComparison.Ordinal)){
                        Check(Convert.ToUInt32(p["@revision"])==pending.Lot.Revision,"V2 wrong lot CAS");pending.Lot.Estado=text("@estado");pending.Lot.Revision++;
                    }else if(sql.StartsWith("INSERT INTO ra_integracion_evento",StringComparison.Ordinal)){
                        Check(text("@accion").Length<=40&&text("@codigo").Length<=40,"V2 event exceeds installed schema");AddEvent(pending,text("@accion"),text("@codigo"),text("@detalle"));
                    }else throw new Exception("V2 unexpected SQL");
                    if(FailAt==localWrites)throw new InvalidOperationException("V2_SYNTHETIC_SQL_FAILURE");return 1;
                };
                Action consolidate=()=>{localConsolidations++;if(FailConsolidation)throw new InvalidOperationException("V2_SYNTHETIC_CONSOLIDATION_FAILURE");};
                string output;try{output=(string)RepositoryHelper("PersistirPasoDocumentoV2",Command,view,doc,result,step,new DateTime(2026,11,8,12,0,0,DateTimeKind.Utc),execute,consolidate);}catch(Exception e){LastFailure=e.GetBaseException();throw LastFailure;}
                State=pending;Writes+=localWrites;Consolidations+=localConsolidations;Commits++;
                if(LostResponseStep==step)throw new InvalidOperationException("V2_SYNTHETIC_RESPONSE_LOSS");return output;
            }
        }
        public object Begin(){return V2("Leer",Step("INICIO",Prepared()));}
        public object Gate(){return V2("Leer",Step("AUTORIZAR",Current()));}
        public object Observe(bool partial=false){var result=Result();if(partial){result.Estado="INCIERTO";result.InventarioId=null;}var observed=V2("ConRetorno",Current(),result);Step("OBSERVAR",observed);return observed;}
        public ResultadoEfectoImportacion Result(){var r=(ResultadoEfectoImportacion)RepositoryHelper("EvidenciaInicialDocumento",Command);r.Estado="CONFIRMADO";r.DocumentoId="90";r.InventarioId="100";return r;}
        public void Dispose(){Flow.Dispose();}
    }
    static void PersistenceV2Cases(){
        int cases=0;Action<string,Action> run=(name,action)=>{action();cases++;Console.WriteLine("DOC-118 SQL V2 case: "+name+": passed");};
        run("adapter_uses_repository_core_for_full_protocol",()=>{using(var f=new SqlV2Fixture()){
            var r=f.Flow.Run(f.Protocol);Check(r.Estado=="CONFIRMADO"&&f.Flow.Writers==1&&f.State.Attempt.Estado=="CONFIRMADO"&&f.State.File.EstadoEfecto=="COMPLETADO"&&f.State.Journal.Rows.Count==5&&f.Consolidations==1,"V2 repository/adapter protocol incomplete: commits="+f.Commits+"; writer="+f.Flow.Writers+"; events="+f.State.Journal.Rows.Count+"; failure="+(f.LastFailure==null?"none":f.LastFailure.GetType().Name+":"+f.LastFailure.Message));
            Check(f.State.Lot.TotalArchivos==1&&f.State.File.Resultado.DocumentoId=="90"&&f.State.File.IntentoActivoId==null,"V2 inventory/identity changed");f.Flow.Released();
        }});
        run("lost_start_commit_has_no_gate_or_writer",()=>{using(var f=new SqlV2Fixture()){f.LostResponseStep="INICIO";Check(f.Flow.Run(f.Protocol).Estado=="INCIERTO"&&f.State.Attempt.Estado=="EN_CURSO"&&f.State.Journal.Rows.Count==2&&f.Flow.Writers==0,"V2 start loss grants writer");}});
        run("lost_gate_commit_cannot_be_issued_again",()=>{using(var f=new SqlV2Fixture()){
            f.LostResponseStep="AUTORIZAR";Check(f.Flow.Run(f.Protocol).Estado=="INCIERTO"&&f.Flow.Writers==0&&f.State.Journal.Rows.Count==3,"V2 gate response loss ignored");f.LostResponseStep=null;var before=f.State.Snapshot();Rejected("CONFLICTO_REVISION",()=>f.Step("AUTORIZAR",f.Prepared()));Check(f.State.Snapshot()==before,"V2 gate replay writes");
        }});
        run("concurrent_gate_only_one_permission",()=>{using(var f=new SqlV2Fixture()){var prepared=f.Begin();int wins=0,blocked=0;Parallel.For(0,2,n=>{try{f.Step("AUTORIZAR",prepared);System.Threading.Interlocked.Increment(ref wins);}catch(PerfilImportacionException){System.Threading.Interlocked.Increment(ref blocked);}});Check(wins==1&&blocked==1&&f.State.Journal.Rows.Count==3,"V2 duplicate gate permissions");}});
        run("duplicate_initial_start_rejected",()=>{using(var f=new SqlV2Fixture()){f.Begin();var before=f.State.Snapshot();Rejected("CONFLICTO_REVISION",()=>f.Step("INICIO",f.Prepared()));Check(f.State.Snapshot()==before,"V2 duplicate start writes");}});
        run("start_and_gate_rollback_on_each_statement",()=>{foreach(var step in new[]{"INICIO","AUTORIZAR"})foreach(var fail in new[]{1,2,3})foreach(var zero in new[]{false,true})using(var f=new SqlV2Fixture()){object doc=f.Prepared();if(step=="AUTORIZAR")doc=f.Begin();var before=f.State.Snapshot();f.FailAt=fail;f.ReturnZeroAtFailure=zero;bool failed=false;try{f.Step(step,doc);}catch(InvalidOperationException){failed=!zero;}catch(PerfilImportacionException e){failed=zero&&e.Codigo=="CONFLICTO_REVISION";}Check(failed&&f.State.Snapshot()==before,"V2 partial transaction committed");}});
        run("checkpoint_rollback_on_each_statement",()=>{foreach(var fail in new[]{1,2,3,4})using(var f=new SqlV2Fixture()){f.Begin();f.Gate();var observed=f.Observe();var before=f.State.Snapshot();f.FailAt=fail;bool failed=false;try{f.Step("RESULTADO",observed,f.Result());}catch(InvalidOperationException){failed=true;}Check(failed&&f.State.Snapshot()==before&&f.Consolidations==0,"V2 checkpoint partial commit");}});
        run("consolidation_failure_rolls_back_result_and_event",()=>{using(var f=new SqlV2Fixture()){f.Begin();f.Gate();var observed=f.Observe();var before=f.State.Snapshot();f.FailConsolidation=true;bool failed=false;try{f.Step("RESULTADO",observed,f.Result());}catch(InvalidOperationException){failed=true;}Check(failed&&f.State.Snapshot()==before,"V2 consolidation failure commits result");}});
        run("confirmed_checkpoint_replay_is_history_only",()=>{using(var f=new SqlV2Fixture()){f.Begin();f.Gate();var observed=f.Observe();var result=f.Result();var first=f.Step("RESULTADO",observed,result);f.State.Lot.Estado="COMPLETADO";f.State.Root.Estado="COMPLETADA";var before=f.State.Snapshot();var writes=f.Writes;var again=f.Step("RESULTADO",observed,result);Check(first==again&&f.State.Snapshot()==before&&f.Writes==writes&&f.Consolidations==1,"V2 result replay mutates or consolidates");}});
        run("uncertain_checkpoint_keeps_claim_and_partial_ids",()=>{using(var f=new SqlV2Fixture()){f.Begin();f.Gate();var observed=f.Observe(true);var result=f.Result();result.Estado="INCIERTO";result.InventarioId=null;f.Step("RESULTADO",observed,result);Check(f.State.Lot.Estado=="PENDIENTE_VERIFICACION"&&f.State.File.IntentoActivoId==f.Command.Intento.IntentoId&&f.State.Attempt.FinUtc==null&&f.State.File.Resultado.InventarioId==null,"V2 uncertainty released claim/invented IDs");}});
        run("stop_blocks_gate_but_is_preserved_after_writer",()=>{
            using(var f=new SqlV2Fixture()){var p=f.Begin();f.State.Lot.DetencionSolicitada=true;Rejected("CONFLICTO_REVISION",()=>f.Step("AUTORIZAR",p));}
            using(var f=new SqlV2Fixture()){f.Begin();f.Gate();f.State.Lot.DetencionSolicitada=true;var observed=f.Observe();f.Step("RESULTADO",observed,f.Result());Check(f.State.Lot.DetencionSolicitada&&f.State.File.Resultado.Estado=="CONFIRMADO","V2 stop lost confirmed effect");}
        });
        run("known_ids_must_match_observed_return",()=>{using(var f=new SqlV2Fixture()){f.Begin();f.Gate();var obs=f.Observe();var result=f.Result();result.DocumentoId="999";var before=f.State.Snapshot();Rejected("CONFLICTO_REVISION",()=>f.Step("RESULTADO",obs,result));Check(f.State.Snapshot()==before,"V2 attributes foreign identity");}});
        run("duplicated_missing_and_unknown_journal_rejected",()=>{foreach(var mode in new[]{"duplicate","missing","unknown","order"})using(var f=new SqlV2Fixture()){var p=f.Begin();if(mode=="duplicate")f.State.Journal.ImportRow(f.State.Journal.Rows[1]);if(mode=="missing")f.State.Journal.Rows.RemoveAt(1);if(mode=="unknown")f.State.Journal.Rows[1]["accion"]="OTHER";if(mode=="order"){var row=f.State.Journal.Rows[0].ItemArray;f.State.Journal.Rows.RemoveAt(0);f.State.Journal.Rows.Add(row);}var before=f.State.Snapshot();Rejected("CONFLICTO_REVISION",()=>f.Step("AUTORIZAR",p));Check(f.State.Snapshot()==before,"V2 journal mismatch changed data");}});
        run("actor_resource_and_event_code_must_match",()=>{foreach(var field in new[]{"actor_ref","recurso_id","recurso_tipo","codigo","intento_id","lote_id"})using(var f=new SqlV2Fixture()){var p=f.Begin();f.State.Journal.Rows[1][field]="999";Rejected("CONFLICTO_REVISION",()=>f.Step("AUTORIZAR",p));}});
        run("token_hash_scope_and_revision_changes_rejected",()=>{foreach(var mode in new[]{"token","hash","scope","revision"})using(var f=new SqlV2Fixture()){var p=f.Begin();if(mode=="token")f.State.Attempt.TokenReclamacion=new string('f',32);if(mode=="hash")f.State.Attempt.SolicitudHash=new string('f',64);if(mode=="scope")f.State.Lot.Contexto.UsuarioRef="999";if(mode=="revision")f.State.File.Revision++;Rejected("CONFLICTO_REVISION",()=>f.Step("AUTORIZAR",p));}});
        run("v1_active_evidence_never_backfilled",()=>{using(var f=new SqlV2Fixture()){f.State.Attempt.Estado="EN_CURSO";f.State.Evidence=PerfilImportacionJson.Escribir(RepositoryHelper("EvidenciaInicialDocumento",f.Command));var before=f.State.Snapshot();Rejected("CONFLICTO_REVISION",()=>f.Step("INICIO",f.Prepared()));Check(f.State.Snapshot()==before,"V2 backfilled V1");}});
        run("v2_read_projection_rejects_v1_checkpoint",()=>{using(var f=new SqlV2Fixture()){f.Begin();int writes=0;Rejected("CONFLICTO_REVISION",()=>RepositoryHelper("PersistirResultadoDocumento",f.Command,f.State.Lot,f.State.Root,f.State.File,f.State.Attempt,f.Result(),DateTime.UtcNow,new Func<string,object[],int>((s,p)=>{writes++;return 1;}),new Action(()=>{})));Check(writes==0,"V1 checkpoint accepted V2 evidence");var json=PerfilImportacionJson.Escribir(f.State.Attempt.Resultado);Check(!json.Contains("NodoRef")&&!json.Contains("IdentidadFisicaHash")&&!json.Contains("TokenReclamacion"),"V2 projection leaks proof");}});
        run("v1_reader_roundtrip_and_malformed_v2_fail_closed",()=>{using(var f=new SqlV2Fixture()){var old=(ResultadoEfectoImportacion)RepositoryHelper("EvidenciaInicialDocumento",f.Command);var text=PerfilImportacionJson.Escribir(old);var loaded=RepositoryHelper("LeerEvidenciaDocumentalCompatible",text,f.State.Attempt);Check(PerfilImportacionJson.Escribir(loaded)==text,"V1 reader changed");foreach(var bad in new[]{"{","{\"Version\":1}",new string(' ',PerfilImportacionJson.MaximoCaracteres+1)})Rejected("CAPACIDAD_NO_DISPONIBLE",()=>RepositoryHelper("LeerEvidenciaDocumentalCompatible",bad,f.State.Attempt));}});
        run("lost_observation_ack_stops_without_checkpoint",()=>{using(var f=new SqlV2Fixture()){f.LostResponseStep="OBSERVAR";var r=f.Flow.Run(f.Protocol);Check(r.Estado=="INCIERTO"&&r.DocumentoId=="90"&&f.Flow.Writers==1&&f.State.Journal.Rows.Count==4&&f.Consolidations==0,"V2 lost observation repeats effect/checkpoint");}});
        run("lost_checkpoint_ack_keeps_durable_confirmation",()=>{using(var f=new SqlV2Fixture()){f.LostResponseStep="RESULTADO";var r=f.Flow.Run(f.Protocol);Check(r.Estado=="INCIERTO"&&r.DocumentoId=="90"&&f.State.Attempt.Estado=="CONFIRMADO"&&f.State.Journal.Rows.Count==5&&f.Flow.Writers==1,"V2 lost checkpoint destroys confirmation");}});
        Check(cases==20,"V2 SQL case accounting changed");Console.WriteLine("DOC-118 V2 repository SQL behavior cases: 20 passed; transactional double; adapter integrated; no database connection");
    }
}
