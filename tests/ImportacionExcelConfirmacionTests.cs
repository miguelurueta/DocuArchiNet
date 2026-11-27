using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;

//Mismo ejecutable DOC122. Libro/lector reales; servidor, sesiones y fallos ADO sintéticos.
//No acredita transacciones, igualdad ni carreras de un MySQL real.
public static class ImportacionExcelConfirmacionTests {
 static int checks;
 static void Check(bool v,string n){checks++;if(!v)throw new Exception("DOC125 "+n);}
 static void Reject(Action f,string code){try{f();}catch(ImportacionExcelException e){Check(e.Codigo==code,"expected "+code+" got "+e.Codigo);return;}throw new Exception("DOC125 missing "+code);}
 static T Copy<T>(T v){return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(v));}
 static ContextoCargaExcel Context(){return new ContextoCargaExcel{UsuarioId=1,EmpresaId=2,ModuloId=3,ConexionId=7,Autorizado=true,IdentidadConexion=new string('a',64)};}
 static PlantillaExcel Template(string mode){return new PlantillaExcel{ID_PLANTILLAIMP=5,IMP01_XMLCONEXION_ID_XMLCONEXION=7,NOMBRE_PLANTILLA="imp_02_doc125_sintetico",CONEXION_FUENTE="12.0 xlsx",LIBRO_EXCEL="Datos",CAMPO_UNICO="CLAVE",REVISION_CARGA=1,INSERTA_RECORD=mode=="REEMPLAZAR"?0u:1u,ELIMINA_INSERTA_RECORD=mode=="REEMPLAZAR"?1u:0u,ESTADO_PLANTILLA=1,Campos=new List<CampoPlantillaExcel>{new CampoPlantillaExcel{ID_CAMPOS_PLANTILLA=1,CAMPO_FUENTE="Código",CAMPO_DESTINO="CLAVE",TIPO_CAMPO=mode=="CREAR"?"VARCHAR(250)":"VARCHAR"}}};}
 static PoliticaDestinoCargaExcel Policy(){return new PoliticaDestinoCargaExcel{FuenteId="synthetic",AmbitoRef="2",Revision=1,Modo="CONEXION"};}
 static List<FilaNormalizadaExcel> Rows(int count){return Enumerable.Range(0,count).Select(i=>new FilaNormalizadaExcel{Fila=i+2,Valores=new List<ValorNormalizadoExcel>{new ValorNormalizadoExcel{CampoDestino="CLAVE",Tipo="TEXTO",Texto="K"+i.ToString("D5")}}}).ToList();}
 sealed class Store:IAlmacenArchivoExcel,IArchivoRetenidoExcel {
  public byte[] Bytes;public int Reads;public bool Changed;
  public ArchivoRecibidoExcel Recibir(ContextoCargaExcel c,string id,string n,string f,Stream s,LimitesLecturaExcel l){throw new NotSupportedException();}
  public IArchivoRetenidoExcel Retener(ContextoCargaExcel c,string id,ArchivoRecibidoExcel a){Reads++;Check(a.Hash==ReglasCargaExcel.Hash(Bytes),"retained hash");return this;}
  public Stream Contenido{get{return new MemoryStream(Bytes);}} public void Verificar(){if(Changed)throw ReglasCargaExcel.Fallo("ARCHIVO_INVALIDO");}public void Dispose(){}
 }
 sealed class Fixture {
  public readonly State Db;public readonly Store File;public readonly MySqlCargaExcelRepository Repo;public readonly ContextoCargaExcel C=Context();
  public LimitesLecturaExcel Limits=new LimitesLecturaExcel{MaxFilasDatos=20000};public bool Authorized=true;public int LimitReads,ContextReads;public Action<ContextoCargaExcel,int> ContextHook;
  public Fixture(string mode,int count=2){
   Db=new State{P=Template(mode),Exists=mode!="CREAR",Charset=mode=="CREAR"?"utf8":"latin1",Collation=mode=="CREAR"?"utf8_general_ci":"latin1_swedish_ci"};
   File=new Store{Bytes=ImportacionExcelLecturaTests.Book(Rows(count).Select(r=>new object[]{r.Valores[0].Texto}).ToArray(),new[]{"Código"})};
   Repo=new MySqlCargaExcelRepository(()=>new Connection(Db),t=>Policy(),cn=>Db.Discarded++);
   EsquemaCargaExcel schema;using(var cn=new Connection(Db)){cn.Open();schema=new MySqlPreparacionExcelValidator(cn,C,t=>Policy()).Inspeccionar(Db.P,mode);}
   var e=new CargaExcelEjecucion{Id=Guid.NewGuid().ToString("D"),EmpresaId=2,UsuarioId=1,ConexionRef=C.ConexionRef,PlantillaId=5,Tabla=Db.P.NOMBRE_PLANTILLA,Estado="PREPARADA",Modo=mode,RevisionPlantilla=1,Archivo=new ArchivoRecibidoExcel{Nombre="x.xlsx",Referencia="synthetic/private",Bytes=File.Bytes.Length,Hash=ReglasCargaExcel.Hash(File.Bytes),Formato="XLSX"},EsquemaHash=schema.Hash,FilasPrevistas=count};
   e.ConfiguracionDoc=ServicioPreparacionExcel.Documento(C,Db.P,schema,new SolicitudPrepararExcel{Modo=mode,Hoja="Datos"},Limits);e.ConfiguracionHash=ReglasCargaExcel.Hash(Encoding.UTF8.GetBytes(e.ConfiguracionDoc));Db.Row=e;
  }
  public ServicioConfirmacionExcel Service(){return new ServicioConfirmacionExcel(()=>{var c=Copy(C);c.Autorizado=Authorized;ContextReads++;if(ContextHook!=null)ContextHook(c,ContextReads);return c;},Repo,File,()=>{LimitReads++;return Limits;});}
  public SolicitudConfirmarExcel Request(){return new SolicitudConfirmarExcel{ConexionId=7,IdCarga=Db.Row.Id,ArchivoHash=Db.Row.Archivo.Hash,ConfiguracionHash=Db.Row.ConfiguracionHash,EsquemaHash=Db.Row.EsquemaHash,ConfirmacionReemplazo=true};}
 }
 static void Modes(){
  Check(DifferentLockOwners(1L,2L),"independent active load and destination lock owners observed");
  Check(!DifferentLockOwners(1L,1L),"same owner does not prove independent active locks");
  Check(!DifferentLockOwners(DBNull.Value,2L),"missing load lock does not prove active writer");
  Check(!DifferentLockOwners(1L,null),"missing destination lock does not prove active writer");
  var observations=0;WaitForImportInsert(()=>++observations==3,_=>{},1000);
  Check(observations==3,"external writer waits for observed importer INSERT, independent of preflight latency");
  observations=0;try{WaitForImportInsert(()=>{observations++;return false;},_=>{},0);throw new Exception("barrier accepted expired window");}catch(InvalidOperationException){Check(observations==0,"expired barrier never authorizes external COMMIT");}
  foreach(var mode in new[]{"AGREGAR","REEMPLAZAR","CREAR"}){
   var f=new Fixture(mode);if(mode!="CREAR")f.Db.Rows.Add(new Dictionary<string,object>{{"CLAVE","old"}});
   var result=f.Service().Confirmar(f.Request());
   Check(result.Estado=="CONFIRMADA"&&result.FilasConfirmadas==2&&result.ConfirmadoUtc.HasValue,"durable result "+mode);
   Check(f.Db.Rows.Count==(mode=="AGREGAR"?3:2),"mode effects "+mode);
   Check(f.Db.Locks.Count==0&&f.Db.OpenTransactions==0&&f.Db.Modes.All(x=>x=="ANSI_QUOTES"),"resources released "+mode);
   Check(f.Db.Sql.Count(x=>x.Contains("information_schema.KEY_COLUMN_USAGE"))==1,"one complete incoming/outgoing FK inspection per confirmation "+mode);
   Check(f.Db.Sql.Count(x=>x.Contains("information_schema.TRIGGERS"))>1,"trigger visibility and effects rechecked "+mode);
   Check(f.Db.Sql.Any(x=>x.Contains("FROM imp01_plantillaimp")&&x.EndsWith("FOR UPDATE"))&&f.Db.Sql.Any(x=>x.Contains("FROM imp01_campos_plantilla")&&x.EndsWith("FOR UPDATE")),"header and fields locked in data transaction "+mode);
   Check(f.Db.Sql.Where(x=>x.StartsWith("INSERT INTO `imp_02_")||x.StartsWith("DELETE FROM `imp_02_")||x.Contains("estado='CONFIRMADA'")).All(x=>!x.Contains("K000")),"parameterized content "+mode);
   var writes=f.Db.BusinessInserts;var history=new ServicioConfirmacionExcel(()=>f.C,f.Repo,(Func<IAlmacenArchivoExcel>)(()=>{throw new Exception("no historical file access");}),()=>{throw new Exception("no historical limit access");});
   f.Db.P.REVISION_CARGA++;Check(history.Confirmar(f.Request()).FilasConfirmadas==2&&history.ConsultarResultado(f.Request()).Estado=="CONFIRMADA"&&f.Db.BusinessInserts==writes,"historical replay avoids new configuration "+mode);
   var forged=f.Request();forged.ArchivoHash=new string('b',64);Reject(()=>history.Confirmar(forged),"CONFLICTO_CONFIRMACION");
   if(mode=="CREAR")Check(f.Db.Comment==MySqlEstructuraExcel.Marca(f.Db.Row)&&f.Db.Creates==1,"identified create once");
   if(mode=="AGREGAR"){
    f.Db.ForeignKeys=1;
    using(var next=f.Repo.AbrirConfirmacion(f.C,f.Db.Row.Id)){
     next.BloquearDestino(f.Db.Row.Tabla);
     Reject(()=>next.Inspeccionar(f.Db.P,f.Db.Row),"EFECTOS_RELACIONADOS_NO_ADMITIDOS");
    }
    Check(f.Db.Sql.Count(x=>x.Contains("information_schema.KEY_COLUMN_USAGE"))==2,"FK accreditation never reused by another confirmation session");
   }
  }
 }
 static void Rejections(){
  var f=new Fixture("REEMPLAZAR");var request=f.Request();request.ConfirmacionReemplazo=false;Reject(()=>f.Service().Confirmar(request),"CONFIRMACION_REEMPLAZO_REQUERIDA");Check(f.Db.BusinessInserts==0&&f.Db.Deletes==0,"replace explicit before effects");
  f=new Fixture("AGREGAR");f.Authorized=false;Reject(()=>f.Service().Confirmar(f.Request()),"ACCESO_DENEGADO");f.Authorized=true;var other=f.Request();other.IdCarga=Guid.NewGuid().ToString("D");Reject(()=>f.Service().Confirmar(other),"RESULTADO_INCIERTO");
  foreach(var mutate in new Action<Fixture>[] {x=>x.Db.P.REVISION_CARGA++,x=>x.Limits.MaxFilasDatos=10000,x=>x.Db.Row.Archivo.Bytes=10000001,x=>x.File.Changed=true,x=>x.Db.GlobalSelect=false,x=>x.Db.TriggerPermission=false,x=>x.Db.Triggers=1,x=>x.Db.ForeignKeys=1,x=>x.Db.ControlEngine="MyISAM",x=>x.Db.Prefix=4}){
   f=new Fixture("AGREGAR");mutate(f);try{f.Service().Confirmar(f.Request());throw new Exception("mutation accepted");}catch(ImportacionExcelException){Check(f.Db.BusinessInserts==0&&f.Db.Deletes==0&&f.Db.Locks.Count==0,"invalid pending rejected before writes");}
  }
  f=new Fixture("AGREGAR");f.Db.Row.Estado="RECIBIDA";Reject(()=>f.Service().Confirmar(f.Request()),"ESTADO_NO_CONFIRMABLE");
 }
 static void Transactions(){
  foreach(var mode in new[]{"AGREGAR","REEMPLAZAR","CREAR"}){
   var f=new Fixture(mode,300);if(mode!="CREAR")f.Db.Rows.Add(new Dictionary<string,object>{{"CLAVE","old"}});var before=f.Db.Rows.Count;f.Db.FailInsertAfter=1;
   Reject(()=>f.Service().Confirmar(f.Request()),"ESCRITURA_RECHAZADA");Check(f.Db.Rows.Count==before&&f.Db.Row.Estado=="FALLIDA"&&f.Db.Row.ConfiguracionDoc!=null&&f.Db.Locks.Count==0,"rollback and retained snapshots "+mode);
   if(mode=="CREAR")Check(f.Db.Exists&&f.Db.Creates==1&&f.Db.Rows.Count==0,"DDL survives known data rollback");
   f.Db.FailInsertAfter=-1;Check(f.Service().Confirmar(f.Request()).Estado=="CONFIRMADA"&&f.Db.Creates==(mode=="CREAR"?1:0),"explicit retry same ID "+mode);
  }
  foreach(var commit in new[]{"before","after"}){var f=new Fixture("AGREGAR");f.Db.CommitFailure=commit;Reject(()=>f.Service().Confirmar(f.Request()),"RESULTADO_INCIERTO");Check(f.Db.Row.Estado==(commit=="after"?"CONFIRMADA":"PREPARADA")&&f.Db.Rows.Count==(commit=="after"?2:0),"commit uncertainty "+commit);f.Db.CommitFailure=null;Check(f.Service().ConsultarResultado(f.Request()).Estado==f.Db.Row.Estado,"query after uncertain commit "+commit);}
  var race=new Fixture("AGREGAR");race.Db.ExternalKey="K00000";Reject(()=>race.Service().Confirmar(race.Request()),"CONFLICTO_CLAVE");Check(race.Db.Rows.Count==1&&(string)race.Db.Rows[0]["CLAVE"]=="K00000"&&race.Db.Row.Estado=="FALLIDA","external winner survives rejected transaction");
  var clean=new Fixture("AGREGAR");clean.Db.ReleaseFailure=true;Reject(()=>clean.Service().Confirmar(clean.Request()),"RESULTADO_INCIERTO");Check(clean.Db.Row.Estado=="CONFIRMADA"&&clean.Db.Rows.Count==2&&clean.Db.Discarded>0,"cleanup cannot degrade committed result");
  clean=new Fixture("AGREGAR");clean.Db.IdReleaseFailure=true;Reject(()=>clean.Service().Confirmar(clean.Request()),"RESULTADO_INCIERTO");Check(clean.Db.Row.Estado=="CONFIRMADA"&&clean.Db.Discarded==1,"ID release failure remains uncertain after successful destination release");
  var revoked=new Fixture("AGREGAR");revoked.Db.AfterInsert=()=>revoked.Authorized=false;Reject(()=>revoked.Service().Confirmar(revoked.Request()),"ACCESO_DENEGADO");Check(revoked.Db.Rows.Count==0&&revoked.Db.Row.Estado=="PREPARADA","authority revoked before commit rolls back without unauthorized failure update");
  var warning=new Fixture("REEMPLAZAR");warning.Db.Rows.Add(new Dictionary<string,object>{{"CLAVE","old"}});warning.Db.Warning=true;Reject(()=>warning.Service().Confirmar(warning.Request()),"CONVERSION_MYSQL_INVALIDA");Check(warning.Db.Rows.Count==1&&(string)warning.Db.Rows[0]["CLAVE"]=="old","conversion warnings rollback replacement");
 }
 static void Creation(){
  var binding=new Fixture("CREAR");var marker=MySqlEstructuraExcel.Marca(binding.Db.Row);
  Check(marker.Length==56&&Encoding.UTF8.GetByteCount(marker)==56,"provenance fits MySQL 5.1 60-character table comment without truncation");
  var other=Copy(binding.Db.Row);other.Id=Guid.NewGuid().ToString("D");Check(MySqlEstructuraExcel.Marca(other)!=marker,"compact provenance binds entire load UUID");
  other=Copy(binding.Db.Row);other.EsquemaHash=other.EsquemaHash.Substring(0,63)+(other.EsquemaHash[63]=='0'?"1":"0");Check(MySqlEstructuraExcel.Marca(other)!=marker,"compact provenance binds last nibble of complete schema hash");
  binding.Db.Exists=true;binding.Db.Comment="docuarchi-excel/v1/"+binding.Db.Row.Id+"/"+binding.Db.Row.EsquemaHash;
  Check(binding.Service().Confirmar(binding.Request()).Estado=="CONFIRMADA"&&binding.Db.Creates==0,"exact legacy provenance remains recoverable without ALTER or CREATE");
  var f=new Fixture("CREAR");f.Db.CreateFailure=true;Reject(()=>f.Service().Confirmar(f.Request()),"RESULTADO_INCIERTO");Check(f.Db.Exists&&f.Db.Rows.Count==0&&f.Db.Row.Estado=="PREPARADA","uncertain DDL preserves identity");Check(f.Service().ConsultarResultado(f.Request()).EstadoEstructura=="PROPIA_COMPATIBLE","read only structural recovery");f.Db.CreateFailure=false;Check(f.Service().Confirmar(f.Request()).Estado=="CONFIRMADA"&&f.Db.Creates==1,"recover DDL without recreate");
  f=new Fixture("CREAR");f.Db.Exists=true;f.Db.Comment="foreign";Reject(()=>f.Service().Confirmar(f.Request()),"CONFLICTO_ESTRUCTURA");Check(f.Db.Creates==0&&f.Db.BusinessInserts==0,"foreign table preserved");
  f=new Fixture("CREAR");f.Db.Exists=true;f.Db.Comment=MySqlEstructuraExcel.Marca(f.Db.Row);f.Db.Rows.Add(new Dictionary<string,object>{{"CLAVE","unexplained"}});Reject(()=>f.Service().Confirmar(f.Request()),"CONFLICTO_ESTRUCTURA");Check(f.Db.Rows.Count==1,"do not empty surviving structure");
 }
 static void Locks(){
  var f=new Fixture("AGREGAR");using(var a=f.Repo.AbrirConfirmacion(f.C,f.Db.Row.Id)){
   a.BloquearDestino(f.Db.P.NOMBRE_PLANTILLA);Check(f.Db.Locks.Count==2,"separate sessions retain ID and destination locks on 5.1");
   using(var b=f.Repo.AbrirConfirmacion(f.C,Guid.NewGuid().ToString("D"))){Reject(()=>b.BloquearDestino(f.Db.P.NOMBRE_PLANTILLA),"OPERACION_EN_CURSO");Check(f.Db.Locks.Count==3,"destination conflict does not release first ID lock");}
   Reject(()=>{using(var same=f.Repo.AbrirConfirmacion(f.C,f.Db.Row.Id)){}},"OPERACION_EN_CURSO");
  }Check(f.Db.Locks.Count==0,"both explicit lock releases");
  var n=MySqlCargaExcelRepository.NombreLockDestino("physical",3306,"DA","imp_02_ABC",1);Check(n.Length==64&&n==MySqlCargaExcelRepository.NombreLockDestino("physical",3306,"da","imp_02_abc",1),"case folded physical destination");Check(n!=MySqlCargaExcelRepository.NombreLockDestino("physical",3306,"da","imp_02_other",1),"different tables independent");
 }
 static void BoundariesAndTypes(){
  var invalidLimits=new ImportacionExcelConexionAdapter(s=>{throw new Exception("bootstrap limits must not open database");},key=>"invalid").AperturaLimites();Check(!invalidLimits.PermiteNuevaCarga&&invalidLimits.CodigoCapacidad=="CAPACIDAD_NO_DISPONIBLE"&&invalidLimits.MaxFilasDatos==0,"history bootstrap reports capacity closed without blocking query context");
  var f=new Fixture("AGREGAR");using(var cn=new Connection(f.Db)){cn.Open();using(var tx=cn.BeginTransaction()){
   var w=new MySqlConfirmacionExcelWriter(cn,tx);var groups=w.Agrupar(f.Db.P,Rows(20000));Check(groups.Sum(x=>x.Count)==20000&&groups.All(x=>x.Count<=128),"20000 bounded commands single transaction");
   var huge=Rows(1);huge[0].Valores[0].Texto=new string('a',70000);Reject(()=>w.Agrupar(f.Db.P,huge),"LIMITE_PAQUETE_EXCEDIDO");Check(f.Db.Deletes==0&&f.Db.BusinessInserts==0,"budget validates before effects");tx.Rollback();
  }}
  f=new Fixture("AGREGAR",20001);Reject(()=>f.Service().Confirmar(f.Request()),"LECTURA_INVALIDA");Check(f.Db.BusinessInserts==0,"over row limit before writing");
  f=new Fixture("AGREGAR");using(var cn=new Connection(f.Db)){cn.Open();using(var tx=cn.BeginTransaction()){
   var p=Template("AGREGAR");p.Campos.AddRange(new[]{new CampoPlantillaExcel{CAMPO_DESTINO="NUM"},new CampoPlantillaExcel{CAMPO_DESTINO="FECHA"},new CampoPlantillaExcel{CAMPO_DESTINO="VACIO"}});
   var rows=Rows(1);rows[0].Valores[0].Texto="00017";rows[0].Valores.AddRange(new[]{new ValorNormalizadoExcel{CampoDestino="NUM",Tipo="DECIMAL",NumeroCanonico="123456789012345678901234567890123456789.123"},new ValorNormalizadoExcel{CampoDestino="FECHA",Tipo="DATE",Fecha=new DateTime(2025,1,2)},new ValorNormalizadoExcel{CampoDestino="VACIO",Tipo="TEXTO",EsNulo=true}});
   var w=new MySqlConfirmacionExcelWriter(cn,tx);var groups=w.Agrupar(p,rows);w.Escribir(f.C,p,new EsquemaCargaExcel{Politica=Policy()},"AGREGAR",groups);tx.Commit();
   Check((string)f.Db.Rows[0]["CLAVE"]=="00017"&&(string)f.Db.Rows[0]["NUM"]==rows[0].Valores[1].NumeroCanonico&&f.Db.Rows[0]["FECHA"] is DateTime&&f.Db.Rows[0]["VACIO"]==DBNull.Value,"typed parameters preserve meaning without decimal CLR loss");
  }}
  f=new Fixture("REEMPLAZAR");f.Db.Rows.Add(new Dictionary<string,object>{{"CLAVE","mine"},{"EMPRESA","2"}});f.Db.Rows.Add(new Dictionary<string,object>{{"CLAVE","foreign"},{"EMPRESA","3"}});
  using(var cn=new Connection(f.Db)){cn.Open();using(var tx=cn.BeginTransaction()){
   var p=Template("REEMPLAZAR");p.Campos.Add(new CampoPlantillaExcel{CAMPO_DESTINO="EMPRESA"});var rows=Rows(1);rows[0].Valores.Add(new ValorNormalizadoExcel{CampoDestino="EMPRESA",Tipo="TEXTO",Texto="2"});
   var policy=Policy();policy.Modo="COLUMNA";policy.ColumnaEmpresa="EMPRESA";var w=new MySqlConfirmacionExcelWriter(cn,tx);w.Escribir(f.C,p,new EsquemaCargaExcel{Politica=policy},"REEMPLAZAR",w.Agrupar(p,rows));tx.Commit();
   Check(f.Db.Rows.Count==2&&f.Db.Rows.Any(x=>(string)x["EMPRESA"]=="3"&&(string)x["CLAVE"]=="foreign")&&!f.Db.Rows.Any(x=>(string)x["CLAVE"]=="mine"),"replacement retains other company with parameterized filter");
  }}
  var schema=new EsquemaCargaExcel{IdentidadFisica="same",Destino=new DestinoPlantillaExcel{Motor="InnoDB",Columnas=new List<ColumnaDestinoExcel>{new ColumnaDestinoExcel{Nombre="CLAVE",Tipo="int",TipoCompleto="int",Extra=""}}}};var materialized=Copy(schema);materialized.Destino.Existe=true;materialized.Destino.Columnas[0].TipoCompleto="int(11)";
  Check(MySqlEstructuraExcel.HuellaCreable(schema)==MySqlEstructuraExcel.HuellaCreable(materialized),"CREATE comparison normalizes display width and existence only");materialized.Destino.Columnas[0].TipoCompleto="int(11) unsigned";Check(MySqlEstructuraExcel.HuellaCreable(schema)!=MySqlEstructuraExcel.HuellaCreable(materialized),"CREATE comparison retains unsigned meaning");
  f=new Fixture("AGREGAR");Array.Resize(ref f.File.Bytes,10000000);f.Db.Row.Archivo.Bytes=f.File.Bytes.Length;f.Db.Row.Archivo.Hash=ReglasCargaExcel.Hash(f.File.Bytes);Check(f.Service().Confirmar(f.Request()).FilasConfirmadas==2,"exact decimal 10MB remains readable and confirmable locally");
 }
 static void AuthorityMatrix(){
  var names=new[]{"PERMISO","USUARIO","EMPRESA","MODULO","CONEXION","IDENTIDAD_FISICA"};
  var mutations=new Action<ContextoCargaExcel>[] {c=>c.Autorizado=false,c=>c.UsuarioId++,c=>c.EmpresaId++,c=>c.ModuloId++,c=>c.ConexionId++,c=>c.IdentidadConexion=new string('b',64)};
  foreach(var mode in new[]{"CREAR","AGREGAR","REEMPLAZAR"})for(int i=0;i<mutations.Length;i++){
   var f=new Fixture(mode);var mutation=mutations[i];var request=f.Request();
   if(mode!="CREAR")f.Db.Rows.Add(new Dictionary<string,object>{{"CLAVE","old"}});
   var before=JsonConvert.SerializeObject(f.Db.Rows);
   f.ContextHook=(c,n)=>{if(n>=2)mutation(c);};
   Reject(()=>f.Service().Confirmar(request),"ACCESO_DENEGADO");
   Check(f.Db.BusinessInserts==0&&f.Db.Deletes==0&&f.Db.Creates==0&&f.File.Reads==0,"DOC127 authority drift stops before files/DDL/DML "+mode+"/"+names[i]);
   Check(JsonConvert.SerializeObject(f.Db.Rows)==before&&f.Db.Row.Estado=="PREPARADA"&&f.Db.Locks.Count==0&&f.Db.OpenTransactions==0,"DOC127 rejected authority preserves prepared execution and business "+mode+"/"+names[i]);
   Console.WriteLine("DOC127_AUTHORITY_LOCAL_PASS stage=PREWRITE mode="+mode+" change="+names[i]);

   f=new Fixture(mode);request=f.Request();if(mode!="CREAR")f.Db.Rows.Add(new Dictionary<string,object>{{"CLAVE","old"}});
   before=JsonConvert.SerializeObject(f.Db.Rows);var switched=false;
   f.Db.AfterInsert=()=>switched=true;f.ContextHook=(c,n)=>{if(switched)mutation(c);};
   Reject(()=>f.Service().Confirmar(request),"ACCESO_DENEGADO");
   Check(switched&&f.Db.BusinessInserts>0,"DOC127 late authority case actually reached writer "+mode+"/"+names[i]);
   Check(JsonConvert.SerializeObject(f.Db.Rows)==before&&f.Db.Row.Estado=="PREPARADA"&&f.Db.Locks.Count==0&&f.Db.OpenTransactions==0,"DOC127 late authority rejects commit and rolls back full DML "+mode+"/"+names[i]);
   if(mode=="CREAR")Check(f.Db.Exists&&f.Db.Creates==1,"DOC127 CREATE DDL survives rejected DML; no automatic drop");
   Console.WriteLine("DOC127_AUTHORITY_LOCAL_PASS stage=PRECOMMIT mode="+mode+" change="+names[i]);
  }
 }
 public static void Run(){Modes();Rejections();Transactions();Creation();Locks();BoundariesAndTypes();AuthorityMatrix();Console.WriteLine("DOC125_NATIVE_PASS checks="+checks+"; existing harness, real workbook/reader and independent synthetic ADO; no real MySQL/HTTP");}

 public static void CreateE2EFixtures(string directory,string runTag){
  if(!System.Text.RegularExpressions.Regex.IsMatch(runTag??"",@"\A[a-f0-9]{32}\z"))throw new InvalidOperationException("DOC125_FIXTURE_TAG_REQUIRED");
  Directory.CreateDirectory(directory);var metadata=new List<object>();
  foreach(var scenario in new[]{"crear","agregar","reemplazar","rollback","carrera"}){
   int count=scenario=="rollback"?300:scenario=="carrera"?10:1000;
   var rows=Enumerable.Range(0,count).Select(i=>new object[]{(scenario=="carrera"?"DOC125R_":"DOC125_"+scenario+"_")+runTag+"_"+i.ToString("D5"),"V_"+scenario+"_"+runTag+"_"+i.ToString("D5")}).ToArray();
   if(scenario=="rollback"){rows[0][1]="DUP_"+runTag;rows[count-1][1]="DUP_"+runTag;}
   var bytes=ImportacionExcelLecturaTests.Book(rows,new[]{"CLAVE","VALOR"});var file=Path.GetFullPath(Path.Combine(directory,scenario+".xlsx"));File.WriteAllBytes(file,bytes);
   metadata.Add(new{escenario=scenario,archivoPath=file,archivoHash=ReglasCargaExcel.Hash(bytes),hoja="Datos",filasEsperadas=count,claveExterna=scenario=="carrera"?(string)rows[0][0]:null});
  }
  File.WriteAllText(Path.Combine(directory,"manifest.json"),JsonConvert.SerializeObject(new{issue="DOC-125",runTag=runTag,synthetic=true,realExecution=false,cases=metadata},Formatting.Indented),new UTF8Encoding(false));
 }

 //Invocado sólo por la ventana DOC125 del ejecutor nativo DOC115 existente.
 //Una operación de escritor externo; sin instalación, siembra ni limpieza.
 public static void RunExternalWriter(bool authorized,string registeredConnection,string json){
  ExternalWriter(authorized,registeredConnection,json,true);
 }
 public static void CheckExternalWriter(bool authorized,string registeredConnection,string json){
  ExternalWriter(authorized,registeredConnection,json,false);
 }
 static void ExternalWriter(bool authorized,string registeredConnection,string json,bool write){
  if(!authorized)throw new InvalidOperationException("DOC125_NATIVE_SCOPE_REQUIRED");
  var r=Newtonsoft.Json.Linq.JObject.Parse(json);var s=r["scope"];var item=r["item"];var req=r["prepared"].ToObject<SolicitudConfirmarExcel>();
  var issue=(string)s["issue"];var resilience=issue=="DOC-126"&&(string)s["mode"]=="resilience";var prefix=issue=="DOC-125"?"imp_02_doc125_e2e_":issue=="DOC-126"&&((string)s["mode"]=="write"||resilience)?"imp_02_doc126_e2e_":null;
  if(!authorized||prefix==null||(bool?)s["disposable"]!=true||(bool?)s["preserveResources"]!=true||(bool?)s["destinationReadOnly"]==true||(s["allowedEffects"]==null||!s["allowedEffects"].ToObject<string[]>().Contains("EXTERNAL_INSERT"))||(string)item["escenario"]!="carrera"||(string)item["modo"]!="AGREGAR"||!System.Text.RegularExpressions.Regex.IsMatch((string)item["tabla"]??"",@"\A"+prefix+@"[A-Za-z0-9_]{1,44}\z"))throw new InvalidOperationException("DOC125_NATIVE_SCOPE_REQUIRED");
  if(resilience&&((int?)s["maxBusinessRows"]!=311||(int?)s["maxDeletedRows"]!=1000||(int?)item["filasEsperadas"]!=10||(int?)item["filasDestinoEsperadas"]!=1001||(string)item["tabla"]!="imp_02_doc126_e2e_agregar"))throw new InvalidOperationException("DOC125_NATIVE_SCOPE_REQUIRED");
  var builder=new MySql.Data.MySqlClient.MySqlConnectionStringBuilder(registeredConnection);builder.AllowUserVariables=false;builder.PersistSecurityInfo=false;builder.ConnectionTimeout=5;
  var ctx=new ContextoCargaExcel{UsuarioId=(uint)s["usuarioId"],EmpresaId=(uint)s["empresaId"],ModuloId=(uint)s["moduloId"],ConexionId=(uint)item["conexionId"],Autorizado=true,IdentidadConexion=ReglasCargaExcel.HashDocumento(new{Servidor=builder.Server.ToLowerInvariant(),Puerto=builder.Port,Base=builder.Database})};
  Func<DbConnection> factory=()=>new MySql.Data.MySqlClient.MySqlConnection(builder.ConnectionString);
  var repo=new MySqlCargaExcelRepository(factory,t=>{throw new InvalidOperationException("DOC125_POLICY_NOT_FOR_IMPORT");});var e=repo.Consultar(ctx,req.IdCarga);ReglasCargaExcel.Propietario(ctx,e);ServicioConfirmacionExcel.Identidad(req,e);
  if(e.Estado!="PREPARADA"||e.Modo!="AGREGAR"||e.Tabla!=(string)item["tabla"]||e.PlantillaId!=(uint)item["plantillaId"])throw new InvalidOperationException("DOC125_NATIVE_LOAD_CHANGED");
  var doc=ServicioConfirmacionExcel.Documento(e);var policy=doc["Politica"].ToObject<PoliticaDestinoCargaExcel>();
  if(policy.Modo!="CONEXION"||policy.AmbitoRef!=ctx.EmpresaId.ToString())throw new InvalidOperationException("DOC125_NATIVE_POLICY_CHANGED");
  var file=(string)item["archivoPath"];var info=new FileInfo(file);if(!info.Exists||info.Length<8||info.Length>10000000)throw new InvalidOperationException("DOC125_NATIVE_FILE_INVALID");var bytes=File.ReadAllBytes(file);
  if(ReglasCargaExcel.Hash(bytes)!=e.Archivo.Hash||bytes.Length!=e.Archivo.Bytes)throw new InvalidOperationException("DOC125_NATIVE_FILE_CHANGED");
  using(var cn=factory()){
   cn.Open();var p=MySqlPlantillaExcelRepository.LeerCarga(cn,null,e.PlantillaId,ctx.ConexionId,false);
   ReglasCargaExcel.Plantilla(ctx,p,e.RevisionPlantilla.Value,(string)doc["PlantillaHash"]);p.Modo="AGREGAR";
   var validator=new MySqlPreparacionExcelValidator(cn,ctx,t=>policy);var schema=validator.Inspeccionar(p,"AGREGAR");
   if(schema.Hash!=e.EsquemaHash)throw new InvalidOperationException("DOC125_NATIVE_SCHEMA_CHANGED");
   new MySqlEstructuraExcel(cn,validator).VerificarSeguridad(p.NOMBRE_PLANTILLA);
   ResultadoLecturaExcel read;using(var stream=new MemoryStream(bytes))read=new ServicioLecturaExcel().Leer(stream,new SolicitudLecturaExcel{NombreArchivo=e.Archivo.Nombre,FormatoEsperado=ServicioPlantillasExcel.FormatoHistorico(p.CONEXION_FUENTE),Hoja=(string)doc["Hoja"],Plantilla=p,Destino=schema.Destino,Limites=new LimitesLecturaExcel{MaxFilasDatos=20000,MaxArchivoMB=10}});
   if(!read.Valido||read.FilasDatosObservadas!=(int)item["filasEsperadas"]||read.FilasDatosObservadas>1000)throw new InvalidOperationException("DOC125_NATIVE_FILE_INVALID");
   if(read.Filas[0].Valores.Single(v=>v.CampoDestino==p.CAMPO_UNICO).Texto!=(string)item["claveExterna"])throw new InvalidOperationException("DOC125_NATIVE_KEY_CHANGED");
   if(!write){Console.WriteLine("DOC125_EXTERNAL_PREFLIGHT_PASS");return;}
   string destinationLock=null;
   if(resilience)using(var q=cn.CreateCommand()){
    q.CommandText="SELECT @@hostname,@@port,DATABASE(),@@lower_case_table_names";using(var physical=q.ExecuteReader()){if(!physical.Read())throw new InvalidOperationException("DOC125_NATIVE_SCHEMA_CHANGED");destinationLock=MySqlCargaExcelRepository.NombreLockDestino(Convert.ToString(physical[0]),Convert.ToUInt32(physical[1]),Convert.ToString(physical[2]),p.NOMBRE_PLANTILLA,Convert.ToInt32(physical[3]));}
   }
   //No GET_LOCK: este actor verifica la protección física del UNIQUE.
   using(var tx=cn.BeginTransaction()){
    var w=new MySqlConfirmacionExcelWriter(cn,tx);var one=read.Filas.Take(1).ToList();w.Escribir(ctx,p,schema,"AGREGAR",w.Agrupar(p,one));
    Console.WriteLine("DOC125_EXTERNAL_LOCKED");Console.Out.Flush();
    var loadLock="dx"+ReglasCargaExcel.HashDocumento(new{Fisica=(string)doc["IdentidadFisica"],Id=e.Id}).Substring(0,62);
    WaitForImportInsert(()=>{
     using(var q=cn.CreateCommand()){
      q.Transaction=tx;q.CommandTimeout=2;
      q.CommandText="SELECT COUNT(*) FROM information_schema.PROCESSLIST WHERE ID=IS_USED_LOCK(@lock) AND LEFT(INFO,CHAR_LENGTH(@prefix))=@prefix";
      var lockParameter=q.CreateParameter();lockParameter.ParameterName="@lock";lockParameter.Value=loadLock;q.Parameters.Add(lockParameter);
      var prefixParameter=q.CreateParameter();prefixParameter.ParameterName="@prefix";prefixParameter.Value="INSERT INTO `"+p.NOMBRE_PLANTILLA+"`";q.Parameters.Add(prefixParameter);
      if(Convert.ToInt32(q.ExecuteScalar())!=1)return false;
      if(!resilience)return true;
      using(var locks=cn.CreateCommand()){
       locks.Transaction=tx;locks.CommandTimeout=2;locks.CommandText="SELECT IS_USED_LOCK(@load),IS_USED_LOCK(@destination)";
       var a=locks.CreateParameter();a.ParameterName="@load";a.Value=loadLock;locks.Parameters.Add(a);var b=locks.CreateParameter();b.ParameterName="@destination";b.Value=destinationLock;locks.Parameters.Add(b);
       using(var owners=locks.ExecuteReader()){return owners.Read()&&DifferentLockOwners(owners[0],owners[1]);}
      }
     }
    },System.Threading.Thread.Sleep,20000);
    if(resilience){Console.WriteLine("DOC125_EXTERNAL_ACTIVE_LOCKS_OBSERVED");Console.Out.Flush();}
    Console.WriteLine("DOC125_EXTERNAL_IMPORT_WAIT_OBSERVED");Console.Out.Flush();
    tx.Commit();Console.WriteLine("DOC125_EXTERNAL_COMMITTED");Console.Out.Flush();
   }
  }
 }

 static bool DifferentLockOwners(object load,object destination){return load!=null&&load!=DBNull.Value&&destination!=null&&destination!=DBNull.Value&&Convert.ToInt64(load)!=Convert.ToInt64(destination);}
 static void WaitForImportInsert(Func<bool> observed,Action<int> pause,int timeoutMs){
  var clock=System.Diagnostics.Stopwatch.StartNew();
  while(clock.ElapsedMilliseconds<timeoutMs){if(observed())return;pause(50);}
  throw new InvalidOperationException("DOC125_IMPORT_INSERT_NOT_OBSERVED");
 }

 sealed class State {
  public PlantillaExcel P;public CargaExcelEjecucion Row;public bool Exists=true,GlobalSelect=true,TriggerPermission=true,CreateFailure,ReleaseFailure,IdReleaseFailure,Warning;public Action AfterInsert;
  public string Charset="latin1",Collation="latin1_swedish_ci",ControlEngine="InnoDB",Comment="",CommitFailure,ExternalKey;public int? Prefix;
  public int Triggers,ForeignKeys,Creates,BusinessInserts,Deletes,Discarded,OpenTransactions,FailInsertAfter=-1;
  public List<Dictionary<string,object>> Rows=new List<Dictionary<string,object>>();public List<string> Sql=new List<string>(),Modes=new List<string>();public Dictionary<string,Connection> Locks=new Dictionary<string,Connection>();
 }
 sealed class Connection:DbConnection {
  public readonly State S;public Tx Active;public string NamedLock,Mode="ANSI_QUOTES";ConnectionState state;
  public Connection(State s){S=s;}public override string ConnectionString{get;set;}public override string Database{get{return "fixture";}}public override string DataSource{get{return "synthetic";}}public override string ServerVersion{get{return "5.1.synthetic";}}public override ConnectionState State{get{return state;}}public override void Open(){state=ConnectionState.Open;}public override void Close(){if(NamedLock!=null&&S.Locks.ContainsKey(NamedLock)&&S.Locks[NamedLock]==this)S.Locks.Remove(NamedLock);state=ConnectionState.Closed;}protected override void Dispose(bool d){S.Modes.Add(Mode);Close();base.Dispose(d);}public override void ChangeDatabase(string n){throw new NotSupportedException();}protected override DbTransaction BeginDbTransaction(IsolationLevel l){return new Tx(this,l);}protected override DbCommand CreateDbCommand(){return new Command(this);}
 }
 sealed class Tx:DbTransaction {
  readonly Connection c;readonly IsolationLevel level;bool done;public CargaExcelEjecucion Row;public List<Dictionary<string,object>> Rows;
  public Tx(Connection cn,IsolationLevel l){c=cn;level=l;Check(c.Active==null,"no nested transaction");Row=Copy(c.S.Row);Rows=c.S.Rows.Select(r=>new Dictionary<string,object>(r)).ToList();c.Active=this;c.S.OpenTransactions++;}
  public override IsolationLevel IsolationLevel{get{return level;}}protected override DbConnection DbConnection{get{return c;}}
  public override void Commit(){if(c.S.CommitFailure=="before")throw new SyntheticDbError();c.S.Row=Row;c.S.Rows=Rows;Finish();if(c.S.CommitFailure=="after")throw new SyntheticDbError();}
  void Finish(){if(done)return;done=true;c.Active=null;c.S.OpenTransactions--;}
  public override void Rollback(){Finish();}protected override void Dispose(bool d){Finish();base.Dispose(d);}
 }
 sealed class SyntheticDbError:DbException{}
 static Exception Duplicate(){var ctor=typeof(MySql.Data.MySqlClient.MySqlException).GetConstructors(BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public).First(c=>c.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new[]{typeof(string),typeof(int)}));return (Exception)ctor.Invoke(new object[]{"synthetic duplicate",1062});}
 sealed class Command:DbCommand {
  readonly Connection c;readonly ImportacionExcelBehaviorTests.Params ps=new ImportacionExcelBehaviorTests.Params();public Command(Connection cn){c=cn;DbConnection=cn;}public override string CommandText{get;set;}public override int CommandTimeout{get;set;}public override CommandType CommandType{get;set;}public override bool DesignTimeVisible{get;set;}public override UpdateRowSource UpdatedRowSource{get;set;}protected override DbConnection DbConnection{get;set;}protected override DbTransaction DbTransaction{get;set;}protected override DbParameterCollection DbParameterCollection{get{return ps;}}public override void Cancel(){}public override void Prepare(){}protected override DbParameter CreateDbParameter(){return new ImportacionExcelBehaviorTests.Parameter();}
  object V(string n){return ps[n].Value;}void Track(){c.S.Sql.Add(CommandText);if(c.Active!=null)Check(DbTransaction==c.Active,"all commands attached to active transaction");else Check(DbTransaction==null,"no disposed transaction reused");}
  CargaExcelEjecucion Row{get{return c.Active==null?c.S.Row:c.Active.Row;}}List<Dictionary<string,object>> Rows{get{return c.Active==null?c.S.Rows:c.Active.Rows;}}
  public override object ExecuteScalar(){Track();
   if(CommandText.StartsWith("SELECT GET_LOCK")){if(c.NamedLock!=null&&c.S.Locks.ContainsKey(c.NamedLock)&&c.S.Locks[c.NamedLock]==c)c.S.Locks.Remove(c.NamedLock);c.NamedLock=null;var key=(string)V("@nombre");if(c.S.Locks.ContainsKey(key))return 0;c.S.Locks[key]=c;c.NamedLock=key;return 1;}
   if(CommandText.StartsWith("SELECT RELEASE_LOCK")){if(c.S.ReleaseFailure||(c.S.IdReleaseFailure&&((string)V("@nombre")).StartsWith("dx")))return 0;var key=(string)V("@nombre");if(!c.S.Locks.ContainsKey(key)||c.S.Locks[key]!=c)return 0;c.S.Locks.Remove(key);c.NamedLock=null;return 1;}
   if(CommandText.StartsWith("SELECT IS_USED_LOCK"))return c.S.Locks.ContainsKey((string)V("@nombre"))&&c.S.Locks[(string)V("@nombre")]==c?1:0;
   if(CommandText=="SELECT @@session.sql_mode")return c.Mode;if(CommandText=="SELECT @@max_allowed_packet")return 65536;if(CommandText=="SHOW COUNT(*) WARNINGS")return c.S.Warning&&c.S.BusinessInserts>0?1:0;if(CommandText.StartsWith("SELECT MAXLEN"))return c.S.Charset=="utf8"?3:1;
   if(CommandText.StartsWith("SELECT TABLE_COMMENT"))return c.S.Comment;
   if(CommandText.StartsWith("SELECT 1 FROM `imp_02_"))return Rows.Count>0?(object)1:null;
   if(CommandText.Contains("information_schema.USER_PRIVILEGES"))return (string)V("@privilegio")=="SELECT"?(c.S.GlobalSelect?1:0):(c.S.TriggerPermission?1:0);
   if(CommandText.Contains("information_schema.SCHEMA_PRIVILEGES")||CommandText.Contains("information_schema.TABLE_PRIVILEGES"))return 0;
   if(CommandText.Contains("information_schema.TRIGGERS"))return c.S.Triggers;if(CommandText.Contains("information_schema.KEY_COLUMN_USAGE"))return c.S.ForeignKeys;
   throw new Exception("unsupported DOC125 scalar "+CommandText);
  }
  public override int ExecuteNonQuery(){Track();
   if(CommandText.StartsWith("SET SESSION")){c.Mode=(string)V("@modo");return 0;}
   if(CommandText.StartsWith("CREATE TEMPORARY")||CommandText.StartsWith("DROP TEMPORARY")||CommandText.StartsWith("INSERT INTO dx_"))return 0;
   if(CommandText.StartsWith("CREATE TABLE `imp_02_")){Check(c.Active==null,"DDL outside data transaction");Check(((string)V("@marca")).Length<=60,"server 5.1 table comment length");c.S.Creates++;c.S.Exists=true;c.S.Comment=(string)V("@marca");if(c.S.CreateFailure)throw new SyntheticDbError();return 0;}
   if(CommandText.StartsWith("DELETE FROM `imp_02_")){Check(c.Active!=null,"delete transactional");c.S.Deletes++;if(CommandText.Contains("WHERE")){var n=Rows.RemoveAll(x=>Convert.ToString(x["EMPRESA"])==Convert.ToString(V("@empresa")));return n;}var count=Rows.Count;Rows.Clear();return count;}
   if(CommandText.StartsWith("INSERT INTO `imp_02_")){
    Check(c.Active!=null,"insert transactional");if(c.S.FailInsertAfter>=0&&c.S.BusinessInserts>=c.S.FailInsertAfter)throw new SyntheticDbError();c.S.BusinessInserts++;
    if(c.S.ExternalKey!=null){c.S.Rows.Add(new Dictionary<string,object>{{"CLAVE",c.S.ExternalKey}});c.S.ExternalKey=null;throw Duplicate();}
    int start=CommandText.IndexOf('('),end=CommandText.IndexOf(')',start);var cols=CommandText.Substring(start+1,end-start-1).Split(',').Select(x=>x.Trim('`')).ToArray();Check(ps.Count<=128,"bounded writer parameters");
    for(int i=0;i<ps.Count;i+=cols.Length){var r=new Dictionary<string,object>();for(int j=0;j<cols.Length;j++)r[cols[j]]=ps[i+j].Value;if(Rows.Any(x=>String.Equals(Convert.ToString(x["CLAVE"]).TrimEnd(),Convert.ToString(r["CLAVE"]).TrimEnd(),StringComparison.OrdinalIgnoreCase)))throw Duplicate();Rows.Add(r);}if(c.S.AfterInsert!=null)c.S.AfterInsert();return ps.Count/cols.Length;
   }
   if(CommandText.StartsWith("UPDATE imp01_carga_ejecucion")){
    Check(c.Active!=null,"result transactional");if(Row==null||Row.Id!=(string)V("@id")||Row.ConfiguracionHash!=BitConverter.ToString((byte[])V("@config")).Replace("-","").ToLowerInvariant())return 0;
    if(CommandText.Contains("estado='CONFIRMADA'")){Row.Estado="CONFIRMADA";Row.FilasConfirmadas=Convert.ToInt32(V("@filas"));Row.ConfirmadoUtc=DateTime.UtcNow;Row.CodigoIncidencia=null;}else{Check(Row.Estado!="CONFIRMADA","CAS cannot degrade confirmed result");Row.Estado="FALLIDA";Row.CodigoIncidencia=(string)V("@codigo");}return 1;
   }
   throw new Exception("unsupported DOC125 mutation "+CommandText);
  }
  static DbDataReader Table(string[] names,params object[][] rows){var t=new DataTable();foreach(var n in names)t.Columns.Add(n,typeof(object));foreach(var r in rows)t.Rows.Add(r.Select(v=>v??DBNull.Value).ToArray());return t.CreateDataReader();}
  protected override DbDataReader ExecuteDbDataReader(CommandBehavior b){Track();
   if(CommandText=="SELECT @@hostname,@@port,DATABASE()")return Table(new[]{"host","port","db"},new object[]{"synthetic",3306,"fixture"});
   if(CommandText=="SELECT @@hostname,@@port,DATABASE(),@@lower_case_table_names")return Table(new[]{"host","port","db","lower"},new object[]{"synthetic",3306,"fixture",0});
   if(CommandText=="SELECT @@character_set_database,@@collation_database")return Table(new[]{"charset","collation"},new object[]{c.S.Charset,c.S.Collation});
   if(CommandText.Contains("information_schema.TABLES")){var n=(string)V("@tabla");return Table(new[]{"ENGINE","TABLE_TYPE"},c.S.Exists||!n.StartsWith("imp_02_")?new[]{new object[]{n.StartsWith("imp_02_")?"InnoDB":c.S.ControlEngine,"BASE TABLE"}}:new object[0][]);}
   if(CommandText.Contains("information_schema.COLUMNS"))return Table(new[]{"COLUMN_NAME","DATA_TYPE","COLUMN_TYPE","CHARACTER_MAXIMUM_LENGTH","IS_NULLABLE","COLUMN_DEFAULT","CHARACTER_SET_NAME","COLLATION_NAME","EXTRA"},new object[]{"CLAVE","varchar","varchar(250)",250,"NO",DBNull.Value,c.S.Charset,c.S.Collation,""});
   if(CommandText.Contains("information_schema.STATISTICS"))return Table(new[]{"INDEX_NAME","NON_UNIQUE","SEQ_IN_INDEX","COLUMN_NAME","SUB_PART"},new object[]{"uq_carga_clave",0,1,"CLAVE",c.S.Prefix});
   if(CommandText.Contains("FROM imp01_plantillaimp")){var p=c.S.P;return Table(CommandText.Substring(7,CommandText.IndexOf(" FROM")-7).Split(','),new object[]{p.ID_PLANTILLAIMP,p.IMP01_XMLCONEXION_ID_XMLCONEXION,p.NOMBRE_PLANTILLA,p.CONEXION_FUENTE,p.LIBRO_EXCEL,p.RUTA_ARCHIVO_FUENTE,p.ELIMINA_TABLA,p.ELIMINA_INSERTA_RECORD,p.INSERTA_RECORD,p.ESTADO_PLANTILLA,p.CAMPO_UNICO,p.REVISION_CARGA});}
   if(CommandText.Contains("FROM imp01_campos_plantilla")){var p=c.S.P;return Table(CommandText.Substring(7,CommandText.IndexOf(" FROM")-7).Split(','),p.Campos.Select(x=>new object[]{x.ID_CAMPOS_PLANTILLA,5,x.CAMPO_FUENTE,x.CAMPO_DESTINO,x.TIPO_CAMPO,x.REMPLAZA_COMA,x.REMPLAZA_PUNTO,x.OPCION_SPLITER,x.CARACTER_SPLITER,x.NUMERO_MATRIZ_EXPLITER}).ToArray());}
   if(CommandText.Contains("FROM imp01_carga_ejecucion")){
    var e=Row;var columns=CommandText.Substring(7,CommandText.IndexOf(" FROM")-7).Split(',');if(e==null||e.Id!=(string)V("@id")||e.EmpresaId.ToString()!=(string)V("@empresa")||e.UsuarioId.ToString()!=(string)V("@usuario")||e.ConexionRef!=(string)V("@conexion"))return Table(columns);
    return Table(columns,new object[]{e.Id,e.PlantillaId,e.EmpresaId,e.UsuarioId,e.ConexionRef,e.Tabla,e.Archivo.Nombre,e.Archivo.Referencia,e.Archivo.Bytes,SqlCargaExcel.Binario(e.Archivo.Hash),e.Modo,e.RevisionPlantilla,e.ConfiguracionDoc,SqlCargaExcel.Binario(e.ConfiguracionHash),SqlCargaExcel.Binario(e.EsquemaHash),e.Estado,e.FilasPrevistas,e.CodigoIncidencia,e.FilasConfirmadas,e.ConfirmadoUtc});
   }
   if(CommandText.Contains("dx_f_"))return Table(new[]{"_fila"});
   throw new Exception("unsupported DOC125 reader "+CommandText);
  }
 }
}
