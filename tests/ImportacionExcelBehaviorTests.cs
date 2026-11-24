using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.SessionState;
using System.Web.Services.Description;
using Newtonsoft.Json;

public static class ImportacionExcelBehaviorTests {
 static int checks;
 static void Check(bool ok,string name){Interlocked.Increment(ref checks);if(!ok)throw new Exception(name);}
 static void Reject(Action action,string code){try{action();}catch(ImportacionExcelException e){Check(e.Codigo==code,"expected "+code+" got "+e.Codigo);return;}throw new Exception("expected "+code);}
 static T Copy<T>(T value){return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(value));}
 static ContextoImportacionExcel Context(){return new ContextoImportacionExcel{UsuarioId=1,EmpresaId=2,Autorizado=true};}
 static PlantillaExcel Template(){return new PlantillaExcel{
  ID_PLANTILLAIMP=5,IMP01_XMLCONEXION_ID_XMLCONEXION=7,NOMBRE_PLANTILLA="imp_02_sintetico",
  CONEXION_FUENTE="12.0 xlsx",LIBRO_EXCEL="Hoja ñ",RUTA_ARCHIVO_FUENTE=@"Z:\historico\origen.xlsx",
  INSERTA_RECORD=1,ESTADO_PLANTILLA=1,CAMPO_UNICO="CLAVE",REVISION_CARGA=1,
  Campos=new List<CampoPlantillaExcel>{
   new CampoPlantillaExcel{ID_CAMPOS_PLANTILLA=10,CAMPO_FUENTE="Código",CAMPO_DESTINO="CLAVE",TIPO_CAMPO="VARCHAR",REMPLAZA_COMA=0,REMPLAZA_PUNTO=0,OPCION_SPLITER=0,CARACTER_SPLITER=null,NUMERO_MATRIZ_EXPLITER=null},
   new CampoPlantillaExcel{ID_CAMPOS_PLANTILLA=11,CAMPO_FUENTE="Nombre",CAMPO_DESTINO="BENEFICIARIO",TIPO_CAMPO="VARCHAR",REMPLAZA_COMA=null,REMPLAZA_PUNTO=null,OPCION_SPLITER=null,CARACTER_SPLITER=null,NUMERO_MATRIZ_EXPLITER=null}
  }};}
 static SolicitudGuardarPlantillaExcel Edit(PlantillaExcel p){return new SolicitudGuardarPlantillaExcel{ConexionId=7,RevisionEsperada=p.REVISION_CARGA,HuellaEsperada=ServicioPlantillasExcel.Huella(p),Plantilla=Copy(p)};}
 static void Rules(){
  var vector=Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText("doc122-snapshot-v1.json"));
  Check(ServicioPlantillasExcel.Huella(vector["plantilla"].ToObject<PlantillaExcel>())==(string)vector["sha256"],"canonical ASCII/NULL/ordinal snapshot v1 vector");
  var stable=ServicioPlantillasExcel.Huella(Template());var previousSettings=JsonConvert.DefaultSettings;
  try{JsonConvert.DefaultSettings=()=>new JsonSerializerSettings{ContractResolver=new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()};Check(ServicioPlantillasExcel.Huella(Template())==stable,"global JSON settings cannot change snapshot");}
  finally{JsonConvert.DefaultSettings=previousSettings;}
  var p=Template();var hash=ServicioPlantillasExcel.Huella(p);
  Check(hash.Length==64,"SHA256");
  p.Campos.Reverse();Check(hash==ServicioPlantillasExcel.Huella(p),"ordinal mapping order");
  p.Campos[0].REMPLAZA_COMA=0;Check(hash!=ServicioPlantillasExcel.Huella(p),"NULL distinct from zero");
  p=Template();p.RUTA_ARCHIVO_FUENTE+="x";Check(hash!=ServicioPlantillasExcel.Huella(p),"legacy route in snapshot");
  p=Template();p.Campos[0].CARACTER_SPLITER="ñ";Check(hash!=ServicioPlantillasExcel.Huella(p),"transformation in snapshot");
  p=Template();p.Campos.Add(Copy(p.Campos[0]));Reject(()=>ServicioPlantillasExcel.Huella(p),"CONFIGURACION_INVALIDA");
  foreach(var bad in new[]{"imp_02_;DROP","imp_02_","IMP_02_a","other","imp_02_a/b","imp_02_a\u0000"})Check(!ServicioPlantillasExcel.TablaValida(bad),"invalid table");
  foreach(var bad in new object[]{null,DBNull.Value,0,2,-1,"01",true})Check(!ImportacionExcelContextAdapter.PermisoValido(bad),"exact permission");
  Check(ImportacionExcelContextAdapter.PermisoValido(1),"permission 1");
  Check(ImportacionExcelConexionAdapter.DestinoCoincide("host","db","HOST","db"),"server ordinal insensitive");
  Check(!ImportacionExcelConexionAdapter.DestinoCoincide("host","db","host","other"),"different DA");
  Check(!ImportacionExcelConexionAdapter.DestinoCoincide("host","db","host","DB"),"database exact case");
  foreach(var tipo in new[]{"VARCHAR(4096)","CHAR(255)","TINYINT UNSIGNED","DECIMAL(65,30)","NUMERIC(10,0)","DATE"})Check(ServicioPlantillasExcel.TipoCreable(tipo),"supported type");
  foreach(var tipo in new[]{"VARCHAR(4097)","CHAR(256)","DECIMAL(2,3)","DECIMAL(66,2)","INT;DROP TABLE a","VARCHAR","DATETIME"})Check(!ServicioPlantillasExcel.TipoCreable(tipo),"reject unsafe or unsupported create type");
  p=Template();Check(ServicioPlantillasExcel.ModoHistorico(p)=="AGREGAR","legacy flags add");
  p.ELIMINA_INSERTA_RECORD=1;p.INSERTA_RECORD=0;Check(ServicioPlantillasExcel.ModoHistorico(p)=="REEMPLAZAR","legacy flags replace");
  p.ELIMINA_TABLA=1;Reject(()=>ServicioPlantillasExcel.ModoHistorico(p),"CONFIGURACION_INVALIDA");
 }
 static void Persistence(){
  var store=new Store{P=Template()};var repo=new MySqlPlantillaExcelRepository(()=>new Connection(store),7);
  var service=new ServicioPlantillasExcel(repo,7);var ctx=Context();
  var initial=service.Obtener(ctx,5);Check(initial.Huella==ServicioPlantillasExcel.Huella(initial),"detail hash");
  var filter=new SolicitudListaPlantillasExcel{ConexionId=7,Termino="sintetico",Pagina=1,TamanoPagina=1};
  Check(service.Listar(ctx,filter).Plantillas.Count==1,"list authorized connection");
  Reject(()=>service.Listar(ctx,new SolicitudListaPlantillasExcel{ConexionId=8}),"SOLICITUD_INVALIDA");
  Reject(()=>service.Listar(ctx,new SolicitudListaPlantillasExcel{ConexionId=7,TamanoPagina=101}),"SOLICITUD_INVALIDA");
  var denied=Context();denied.Autorizado=false;var reads=store.Commands;
  Reject(()=>service.Obtener(denied,5),"ACCESO_DENEGADO");Check(store.Commands==reads,"auth before DB");
  Reject(()=>service.Obtener(ctx,6),"NO_ENCONTRADO");
  var request=Edit(initial);request.Plantilla.Modo="AGREGAR";request.Plantilla.Formato="XLSX";
  request.Plantilla.RUTA_ARCHIVO_FUENTE="../../browser";request.Plantilla.LIBRO_EXCEL="Actualizada";
  var saved=service.Guardar(ctx,request);
  Check(saved.REVISION_CARGA==2,"revision once");Check(saved.RUTA_ARCHIVO_FUENTE==initial.RUTA_ARCHIVO_FUENTE,"preserve historical path");
  Check(saved.Campos[0].ID_CAMPOS_PLANTILLA==10 && saved.Campos[1].REMPLAZA_COMA==null,"IDs and NULL transforms preserved");
  Check(saved.Campos[1].TIPO_CAMPO=="VARCHAR","expanded physical varchar does not rewrite mapping");
  Reject(()=>service.Guardar(ctx,request),"CONFLICTO_REVISION");
  request=Edit(saved);request.Plantilla.Modo="AGREGAR";request.Plantilla.Formato="XLSX";
  store.P.LIBRO_EXCEL="editor antiguo";Reject(()=>service.Guardar(ctx,request),"CONFLICTO_REVISION");
  var before=ServicioPlantillasExcel.Huella(store.P);request=Edit(store.P);request.Plantilla.Modo="AGREGAR";request.Plantilla.Formato="XLSX";request.Plantilla.LIBRO_EXCEL="intento";
  request.Plantilla.Campos[0].CAMPO_FUENTE="Cambio parcial";
  store.FailField=true;Reject(()=>service.Guardar(ctx,request),"GUARDADO_NO_COMPLETADO");
  Check(before==ServicioPlantillasExcel.Huella(store.P),"second field fault rolls back header and first mapping");Check(store.Rollbacks>0 && store.FieldAttempts==2,"rollback after partial mapping write");
  store.FailField=false;store.Engine="MyISAM";Reject(()=>repo.Guardar(request.Plantilla,request.RevisionEsperada,request.HuellaEsperada),"CAPACIDAD_NO_DISPONIBLE");store.Engine="InnoDB";
  var fresh=service.Obtener(ctx,5);var a=Edit(fresh);var b=Edit(fresh);
  foreach(var s in new[]{a,b}){s.Plantilla.Modo="AGREGAR";s.Plantilla.Formato="XLSX";}
  int wins=0,conflicts=0;
  Parallel.Invoke(()=>Race(service,ctx,a,ref wins,ref conflicts),()=>Race(service,ctx,b,ref wins,ref conflicts));
  Check(wins==1 && conflicts==1,"same revision has one winner");
  Check(store.P.REVISION_CARGA==fresh.REVISION_CARGA+1,"concurrent revision increments once");
  var legacy=Copy(store.P);legacy.CAMPO_UNICO=null;store.P=legacy;
  Check(service.Obtener(ctx,5).CAMPO_UNICO==null,"legacy no key readable");
  var invalid=Edit(legacy);invalid.Plantilla.Modo="AGREGAR";invalid.Plantilla.Formato="XLSX";Reject(()=>service.Guardar(ctx,invalid),"CONFIGURACION_INVALIDA");
  store.P=Template();store.DestinationExists=false;
  var create=Edit(store.P);create.Plantilla.ID_PLANTILLAIMP=0;create.RevisionEsperada=0;create.HuellaEsperada=null;create.Plantilla.Modo="CREAR";create.Plantilla.Formato="XLS";
  create.Plantilla.NOMBRE_PLANTILLA="imp_02_nueva_sintetica";
  foreach(var c in create.Plantilla.Campos){c.ID_CAMPOS_PLANTILLA=0;c.TIPO_CAMPO="VARCHAR(250)";}
  var newP=service.Guardar(ctx,create);
  Check(newP.RUTA_ARCHIVO_FUENTE.StartsWith("web/") && newP.RUTA_ARCHIVO_FUENTE.EndsWith(".xls") && newP.RUTA_ARCHIVO_FUENTE.Length<=120,"server-generated relative path");
  Check(newP.REVISION_CARGA==1 && newP.CONEXION_FUENTE=="8.0 xls","new format and revision");
  Check(service.Obtener(ctx,newP.ID_PLANTILLAIMP).Modo=="CREAR","shared legacy flags resolved using target metadata");
  Check(!store.Sql.Any(x=>x.StartsWith("ALTER")||x.StartsWith("CREATE")||x.StartsWith("DROP")),"no destination or installation SQL");
 }
 static void Race(ServicioPlantillasExcel s,ContextoImportacionExcel c,SolicitudGuardarPlantillaExcel r,ref int wins,ref int conflicts){
  try{s.Guardar(c,r);Interlocked.Increment(ref wins);}catch(ImportacionExcelException e){if(e.Codigo!="CONFLICTO_REVISION")throw;Interlocked.Increment(ref conflicts);}
 }
 public class Worker:SimpleWorkerRequest {
  public string Token,Origin="http://127.0.0.1";public string Verb="POST",Content="application/json";public int Bytes=2;
  public byte[] Body;
  public Worker():base("/","C:\\","excel.asmx","",new StringWriter()){}
  public override string GetHttpVerbName(){return Verb;}
  public override byte[] GetPreloadedEntityBody(){return Body??base.GetPreloadedEntityBody();}
  public override int GetTotalEntityBodyLength(){return Body==null?Bytes:Body.Length;}
  public override bool IsEntireEntityBodyIsPreloaded(){return Body!=null||base.IsEntireEntityBodyIsPreloaded();}
  public override string GetKnownRequestHeader(int i){if(i==HttpWorkerRequest.HeaderContentType)return Content;if(i==HttpWorkerRequest.HeaderContentLength)return Bytes.ToString();return base.GetKnownRequestHeader(i);}
  public override string[][] GetUnknownRequestHeaders(){return new[]{new[]{"X-Importacion-Excel-CSRF",Token},new[]{"Origin",Origin}};}
 }
 public static HttpContext Http(Worker w,HttpContext previous=null){
  var h=new HttpContext(w);
  var items=new SessionStateItemCollection();if(previous!=null)foreach(string key in previous.Session.Keys)items[key]=previous.Session[key];
  SessionStateUtility.AddHttpSessionStateToContext(h,new HttpSessionStateContainer("doc122-synthetic",items,new HttpStaticObjectsCollection(),20,true,HttpCookieMode.UseCookies,SessionStateMode.InProc,false));
  return h;
 }
 static void GuardAndAuthority(){
  var w=new Worker();var h=Http(w);w.Origin=h.Request.Url.GetLeftPart(UriPartial.Authority);var c=Context();
  var token=ImportacionExcelRequestGuard.Emitir(h,c);w.Token=token;h=Http(w,h);ImportacionExcelRequestGuard.Validar(h,c);
  Check(token==ImportacionExcelRequestGuard.Emitir(h,c),"multi-tab token stable");
  c.EmpresaId=3;Reject(()=>ImportacionExcelRequestGuard.Validar(h,c),"SOLICITUD_NO_VERIFICADA");c.EmpresaId=2;
  w.Token=new string('?',44);h=Http(w,h);Reject(()=>ImportacionExcelRequestGuard.Validar(h,c),"SOLICITUD_NO_VERIFICADA");w.Token=token;
  w.Origin="https://attacker.invalid";h=Http(w,h);Reject(()=>ImportacionExcelRequestGuard.Emitir(h,c),"SOLICITUD_NO_VERIFICADA");
  w.Origin=h.Request.Url.GetLeftPart(UriPartial.Authority);w.Verb="GET";h=Http(w,h);Reject(()=>ImportacionExcelRequestGuard.Emitir(h,c),"SOLICITUD_NO_VERIFICADA");w.Verb="POST";
  w.Content="text/plain";h=Http(w,h);Reject(()=>ImportacionExcelRequestGuard.Validar(h,c),"SOLICITUD_NO_VERIFICADA");w.Content="application/json";
  w.Bytes=ImportacionExcelRequestGuard.MaximoBytesSolicitud+1;h=Http(w,h);Reject(()=>ImportacionExcelRequestGuard.Validar(h,c),"SOLICITUD_NO_VERIFICADA");w.Bytes=2;h=Http(w,h);
  var state=new Store{P=Template()};var authority=new ImportacionExcelContextAdapter(_=>new Connection(state));
  h.Session["GA_IDUSUARIOGESTION"]="1";h.Session["GA_IDEMPRESA"]="2";h.Session["GA_LOGINUSUARIOGESTION"]="sintetico";
  foreach(var kv in new[]{new[]{"IP_SERVER_MODULO","fixture"},new[]{"DB_NAME_MODULO","fixture"},new[]{"USER_DBMS_MODULO","fixture"},new[]{"PASW_DBMS_MODULO",Guid.NewGuid().ToString("N")},new[]{"TYPE_DBMS_MODULO","mysql"}})h.Session["RA_"+kv[0]]=kv[1];
  Check(authority.Resolver(h).Autorizado,"own permission no profile permission required");
  state.Permission=0;Reject(()=>authority.Resolver(h),"ACCESO_DENEGADO");state.Permission=1;
  state.Active=false;Reject(()=>authority.Resolver(h),"ACCESO_DENEGADO");state.Active=true;
  h.Session["GA_IDEMPRESA"]="3";Reject(()=>authority.Resolver(h),"ACCESO_DENEGADO");h.Session["GA_IDEMPRESA"]="2";
  state.DuplicatePermission=true;Reject(()=>authority.Resolver(h),"ACCESO_DENEGADO");
  Check(state.Sql.Where(x=>x.Contains("ADMINISTRAR_CARGAS_IMPORTACION")).All(x=>x.Contains("u.Estado_Usuario=1")&&!x.Contains("ADMINISTRAR_PERFILES")),"live own authority query");
 }
 static void Contract(){
  var importer=new ServiceDescriptionReflector();importer.Reflect(typeof(WebServiceImportacionExcel),"http://fixture.invalid/excel.asmx");
  Check(importer.ServiceDescriptions.Count>0,"ASMX SOAP reflection works");
  var json=JsonConvert.SerializeObject(new RespuestaPlantillaExcel{datos=Template()});
  Check(!json.Contains("Password")&&!json.Contains("Server=")&&!json.Contains("ConnectionString"),"typed response excludes connection secrets");
  var ws=new WebServiceImportacionExcel();var reply=ws.ListarPlantillas(null);
  Check(!reply.ok && reply.operacionId.Length==32 && reply.datos==null,"envelope fails closed without HTTP");
 }
 static void ModuleConnection(){
  var store=new Store{P=Template()};int opened=0;
  var limits=new Dictionary<string,string>();
  var ephemeralPassword=Guid.NewGuid().ToString("N");
  Func<string,DbConnection> factory=connectionString=>{
   opened++;
   var cs=new MySql.Data.MySqlClient.MySqlConnectionStringBuilder(connectionString);
   Check(cs.UserID=="module_fixture" && cs.Password==ephemeralPassword,"uses existing module account");
   Check(cs.Pooling && cs.MaximumPoolSize==3,"preserves module pool configuration");
   Check(!cs.PersistSecurityInfo && !cs.AllowUserVariables,"connection does not expose credentials or enable user variables");
   return new Connection(store);
  };
  var adapter=new ImportacionExcelConexionAdapter(factory,key=>limits.ContainsKey(key)?limits[key]:null);
  var h=Http(new Worker());var ctx=Context();
  Reject(()=>adapter.Resolver(h,ctx,7),"ACCESO_DENEGADO");Check(opened==0,"missing module context before connection");
  h.Session["DA_ID_EMPRESA"]="2";h.Session["DA_ID_MODULO"]="9";
  h.Session["DA_TYPE_DBMS_MODULO"]="mysql";h.Session["DA_IP_SERVER_MODULO"]="fixture";h.Session["DA_DB_NAME_MODULO"]="fixture";
  h.Session["DA_USER_DBMS_MODULO"]="module_fixture";h.Session["DA_PASW_DBMS_MODULO"]=ephemeralPassword;
  h.Session["DA_ACTIVA_POOL_DBMS"]="1";h.Session["DA_NUMERO_DBMS_CONEX"]="3";
  Check(adapter.Limite("MaxFilasDatos",10000)==10000 && adapter.Limite("MaxArchivoMB",10)==10,"default limits without connection config");
  var refs=adapter.Referencias(h,ctx);Check(refs.Count==1 && refs[0].ConexionId==7,"module resolver supplies matching historical reference");
  var json=JsonConvert.SerializeObject(refs);Check(!json.Contains(ephemeralPassword)&&!json.Contains("module_fixture"),"references exclude module credentials");
  Check(store.Sql.Any(sql=>sql.Contains("NOMBRE_DB_DEFAULT=@base")&&sql.Contains("IP_NOMBRE_SERVIDOR=@servidor")),"catalog lookup bound to module DA");
  Check(store.Sql.All(sql=>!sql.Contains("USUARIO_ADMIN_BD")&&!sql.Contains("PASW_USUARIO_ADMIN_DB")),"does not read historical credentials");
  Reject(()=>adapter.Resolver(h,ctx,8),"ACCESO_DENEGADO");
  int before=opened;
  h.Session["DA_ID_EMPRESA"]="3";Reject(()=>adapter.Resolver(h,ctx,7),"ACCESO_DENEGADO");Check(opened==before,"cross-company DA before connection");h.Session["DA_ID_EMPRESA"]="2";
  h.Session["DA_ID_MODULO"]="0";Reject(()=>adapter.Resolver(h,ctx,7),"ACCESO_DENEGADO");Check(opened==before,"invalid module before connection");h.Session["DA_ID_MODULO"]="9";
  h.Session["DA_TYPE_DBMS_MODULO"]="postgresql";Reject(()=>adapter.Resolver(h,ctx,7),"CAPACIDAD_NO_DISPONIBLE");Check(opened==before,"provider mismatch before connection");h.Session["DA_TYPE_DBMS_MODULO"]="mysql";
  h.Session["DA_PASW_DBMS_MODULO"]=null;Reject(()=>adapter.Resolver(h,ctx,7),"CAPACIDAD_NO_DISPONIBLE");Check(opened==before,"no fallback when module credential absent");h.Session["DA_PASW_DBMS_MODULO"]=ephemeralPassword;
  store.HistoricalDatabase="other";Reject(()=>adapter.Resolver(h,ctx,7),"ACCESO_DENEGADO");
  store.HistoricalDatabase="FIXTURE";Reject(()=>adapter.Resolver(h,ctx,7),"ACCESO_DENEGADO");store.HistoricalDatabase="fixture";
  store.HistoricalServer="other";Reject(()=>adapter.Resolver(h,ctx,7),"ACCESO_DENEGADO");store.HistoricalServer="fixture";
  store.HistoricalProvider="postgresql";Reject(()=>adapter.Resolver(h,ctx,7),"ACCESO_DENEGADO");store.HistoricalProvider="mysql";
  store.DuplicateHistorical=true;Reject(()=>adapter.Resolver(h,ctx,7),"CAPACIDAD_NO_DISPONIBLE");store.DuplicateHistorical=false;
  var repository=adapter.Resolver(h,ctx,7);before=opened;
  h.Session["DA_ID_MODULO"]="10";Reject(()=>repository.Listar("",1,25),"ACCESO_DENEGADO");Check(opened==before,"captured repository rejects switched module");h.Session["DA_ID_MODULO"]="9";
  h.Session["DA_PASW_DBMS_MODULO"]=Guid.NewGuid().ToString("N");Reject(()=>repository.Listar("",1,25),"ACCESO_DENEGADO");Check(opened==before,"captured repository rejects changed connection");h.Session["DA_PASW_DBMS_MODULO"]=ephemeralPassword;
  Check(repository.Listar("",1,25).Plantillas.Count==1,"unchanged session can open repository");
  store.HideHistorical=true;Reject(()=>adapter.Referencias(h,ctx),"CAPACIDAD_NO_DISPONIBLE");store.HideHistorical=false;
  limits["ImportacionExcel.MaxFilasDatos"]="0";Reject(()=>adapter.Limite("MaxFilasDatos",10000),"CAPACIDAD_NO_DISPONIBLE");
  limits["ImportacionExcel.MaxArchivoMB"]="2147483647";Reject(()=>adapter.Limite("MaxArchivoMB",10),"CAPACIDAD_NO_DISPONIBLE");
 }
 public static int Main(){try{Rules();Persistence();GuardAndAuthority();ModuleConnection();Contract();Console.WriteLine("DOC122_NATIVE_PASS checks="+checks+"; synthetic ADO.NET, no MySQL/HTTP");ImportacionExcelLecturaTests.Run();ImportacionExcelPreparacionTests.Run();ImportacionExcelConfirmacionTests.Run();return 0;}catch(Exception e){Console.Error.WriteLine(e.ToString());return 1;}}

 sealed class Store {
  public PlantillaExcel P;public bool DestinationExists=true,FailField,Active=true,DuplicatePermission;public int Permission=1,Commands,Rollbacks;public string Engine="InnoDB";
  public readonly object Sync=new object();public readonly List<string> Sql=new List<string>();public uint NextField=20;
  public string HistoricalDatabase="fixture";
  public string HistoricalServer="fixture",HistoricalProvider="mysql";
  public bool HideHistorical,DuplicateHistorical;
  public int FieldAttempts;
 }
 sealed class Connection:DbConnection {
  readonly Store s;ConnectionState state;public Connection(Store x){s=x;}
  public override string ConnectionString{get;set;}public override string Database{get{return "synthetic";}}public override string DataSource{get{return "synthetic";}}public override string ServerVersion{get{return "5.1.fixture";}}public override ConnectionState State{get{return state;}}
  public override void ChangeDatabase(string name){throw new NotSupportedException();}public override void Open(){state=ConnectionState.Open;}public override void Close(){state=ConnectionState.Closed;}
  protected override DbTransaction BeginDbTransaction(IsolationLevel level){return new Transaction(this,s,level);}
  protected override DbCommand CreateDbCommand(){return new Command(this,s);}
 }
 sealed class Transaction:DbTransaction {
  readonly Connection c;readonly Store s;readonly PlantillaExcel backup;readonly IsolationLevel level;bool done;
  public Transaction(Connection c,Store s,IsolationLevel level){this.c=c;this.s=s;this.level=level;Monitor.Enter(s.Sync);backup=Copy(s.P);}
  public override IsolationLevel IsolationLevel{get{return level;}}protected override DbConnection DbConnection{get{return c;}}
  public override void Commit(){done=true;Monitor.Exit(s.Sync);}public override void Rollback(){s.P=Copy(backup);s.Rollbacks++;done=true;Monitor.Exit(s.Sync);}
  protected override void Dispose(bool disposing){if(!done)Rollback();base.Dispose(disposing);}
 }
 sealed class Failure:DbException{public Failure():base("synthetic provider detail must not escape"){}}
 sealed class Command:DbCommand {
  readonly Store s;readonly Params ps=new Params();public Command(Connection c,Store store){DbConnection=c;s=store;}
  public override string CommandText{get;set;}public override int CommandTimeout{get;set;}public override CommandType CommandType{get;set;}public override bool DesignTimeVisible{get;set;}public override UpdateRowSource UpdatedRowSource{get;set;}
  protected override DbConnection DbConnection{get;set;}protected override DbTransaction DbTransaction{get;set;}protected override DbParameterCollection DbParameterCollection{get{return ps;}}
  public override void Cancel(){}public override void Prepare(){}protected override DbParameter CreateDbParameter(){return new Parameter();}
  object V(string name){return ps.Cast<DbParameter>().Single(p=>p.ParameterName==name).Value;}
  void Track(){Interlocked.Increment(ref s.Commands);lock(s.Sql)s.Sql.Add(CommandText);Check(!CommandText.Contains("Código")&&!CommandText.Contains("Actualizada"),"SQL values parameterized");}
  static DataTable Table(string[] names,params object[][] rows){var t=new DataTable();foreach(var n in names)t.Columns.Add(n,typeof(object));foreach(var row in rows)t.Rows.Add(row);return t;}
  static object[] Values(object value,string[] names){return names.Select(n=>{var p=value.GetType().GetProperty(n);return p==null?DBNull.Value:p.GetValue(value,null)??DBNull.Value;}).ToArray();}
  static void Assign(object target,string name,object value){
   var prop=target.GetType().GetProperty(name);if(prop==null)return;if(value==DBNull.Value){prop.SetValue(target,null,null);return;}
   var type=Nullable.GetUnderlyingType(prop.PropertyType)??prop.PropertyType;prop.SetValue(target,Convert.ChangeType(value,type,CultureInfo.InvariantCulture),null);
  }
  public override object ExecuteScalar(){Track();
   if(CommandText.Contains("COUNT(*) FROM information_schema.TABLES"))return s.Engine=="InnoDB"?2:0;
   if(CommandText.Contains("SELECT ENGINE"))return s.DestinationExists?s.Engine:null;
   if(CommandText.Contains("LAST_INSERT_ID"))return 5;
   if(CommandText.Contains("COUNT(*) FROM imp01_plantillaimp"))return s.P.IMP01_XMLCONEXION_ID_XMLCONEXION==Convert.ToUInt32(V("@conexion"))?1:0;
   throw new Exception("unsupported scalar "+CommandText);
  }
  protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior){Track();
   if(CommandText.Contains("FROM imp01_xmlconexion")){
    var namesCatalog=new[]{"ID_XMLCONEXION","NOMBRE_CONEXION","IP_NOMBRE_SERVIDOR","NOMBRE_DB_DEFAULT","DBMS_BD"};
    var rowsCatalog=new List<object[]>();
    if(!s.HideHistorical&&(!ps.Contains("@id")||Convert.ToUInt32(V("@id"))==7)){
     var row=new object[]{7,"Catálogo sintético",s.HistoricalServer,s.HistoricalDatabase,s.HistoricalProvider};
     rowsCatalog.Add(row);if(s.DuplicateHistorical)rowsCatalog.Add(row);
    }
    return Table(namesCatalog,rowsCatalog.ToArray()).CreateDataReader();
   }
   if(CommandText.Contains("ADMINISTRAR_CARGAS_IMPORTACION")){
    var rows=new List<object[]>();if(s.Active && Convert.ToUInt32(V("@empresa"))==2 && Convert.ToUInt32(V("@usuario"))==1){rows.Add(new object[]{s.Permission});if(s.DuplicatePermission)rows.Add(new object[]{s.Permission});}
    return Table(new[]{"permission"},rows.ToArray()).CreateDataReader();
   }
   if(CommandText.Contains("information_schema.COLUMNS")){
    var table=Convert.ToString(V("@tabla"));
    if(CommandText.Contains("COLUMN_TYPE"))return Table(new[]{"COLUMN_NAME","DATA_TYPE","COLUMN_TYPE","CHARACTER_MAXIMUM_LENGTH","IS_NULLABLE","COLUMN_DEFAULT","CHARACTER_SET_NAME","COLLATION_NAME","EXTRA"},new object[]{"CLAVE","varchar","varchar(250)",250,"NO",DBNull.Value,"latin1","latin1_swedish_ci",""},new object[]{"BENEFICIARIO","varchar","varchar(2000)",2000,"YES",DBNull.Value,"latin1","latin1_swedish_ci",""}).CreateDataReader();
    var type=table=="imp01_plantillaimp"?typeof(PlantillaExcel):typeof(CampoPlantillaExcel);
    var rows=type.GetProperties().Where(p=>p.Name==p.Name.ToUpperInvariant()).Select(p=>new object[]{p.Name,120,"YES","latin1"}).ToList();
    if(table=="imp01_campos_plantilla")rows.Add(new object[]{"IMP01_PLANTILLAIMP_ID_PLANTILLAIMP",DBNull.Value,"NO",DBNull.Value});
    return Table(new[]{"COLUMN_NAME","CHARACTER_MAXIMUM_LENGTH","IS_NULLABLE","CHARACTER_SET_NAME"},rows.ToArray()).CreateDataReader();
   }
   var names=CommandText.Substring(7,CommandText.IndexOf(" FROM",StringComparison.Ordinal)-7).Split(',');
   if(CommandText.Contains("FROM imp01_plantillaimp")){
    if(CommandText.Contains("@id")&&(Convert.ToUInt32(V("@id"))!=s.P.ID_PLANTILLAIMP||Convert.ToUInt32(V("@conexion"))!=s.P.IMP01_XMLCONEXION_ID_XMLCONEXION))return Table(names).CreateDataReader();
    return Table(names,Values(s.P,names)).CreateDataReader();
   }
   if(CommandText.Contains("FROM imp01_campos_plantilla")){
    var rows=s.P.Campos.Select(c=>Values(c,names)).ToArray();foreach(var row in rows)row[1]=s.P.ID_PLANTILLAIMP;return Table(names,rows).CreateDataReader();
   }
   throw new Exception("unsupported reader "+CommandText);
  }
  public override int ExecuteNonQuery(){Track();
   if(CommandText.Contains("imp01_campos_plantilla") && s.FailField && ++s.FieldAttempts==2)throw new Failure();
   if(CommandText.StartsWith("INSERT INTO imp01_plantillaimp")){s.P=new PlantillaExcel{ID_PLANTILLAIMP=5};foreach(DbParameter p in ps)Assign(s.P,p.ParameterName.Substring(1),p.Value);return 1;}
   if(CommandText.StartsWith("UPDATE imp01_plantillaimp")){if(s.P.REVISION_CARGA!=Convert.ToUInt32(V("@revision")))return 0;foreach(DbParameter p in ps)Assign(s.P,p.ParameterName.Substring(1),p.Value);return 1;}
   if(CommandText.StartsWith("INSERT INTO imp01_campos_plantilla")){var c=new CampoPlantillaExcel{ID_CAMPOS_PLANTILLA=s.NextField++};foreach(DbParameter p in ps)Assign(c,p.ParameterName.Substring(1),p.Value);s.P.Campos.Add(c);return 1;}
   if(CommandText.StartsWith("UPDATE imp01_campos_plantilla")){var c=s.P.Campos.Single(x=>x.ID_CAMPOS_PLANTILLA==Convert.ToUInt32(V("@campoId")));foreach(DbParameter p in ps)Assign(c,p.ParameterName.Substring(1),p.Value);return 1;}
   if(CommandText.StartsWith("DELETE FROM imp01_campos_plantilla"))return s.P.Campos.RemoveAll(c=>c.ID_CAMPOS_PLANTILLA==Convert.ToUInt32(V("@campo")));
   throw new Exception("unsupported write "+CommandText);
  }
 }
 public sealed class Parameter:DbParameter {
  public override DbType DbType{get;set;}public override ParameterDirection Direction{get;set;}public override bool IsNullable{get;set;}public override string ParameterName{get;set;}public override string SourceColumn{get;set;}public override object Value{get;set;}public override bool SourceColumnNullMapping{get;set;}public override int Size{get;set;}public override void ResetDbType(){}
 }
 public sealed class Params:DbParameterCollection {
  readonly List<DbParameter> p=new List<DbParameter>();public override int Count{get{return p.Count;}}public override object SyncRoot{get{return this;}}
  public override int Add(object v){p.Add((DbParameter)v);return p.Count-1;}public override void AddRange(Array a){foreach(var v in a)Add(v);}public override void Clear(){p.Clear();}
  public override bool Contains(object v){return p.Contains((DbParameter)v);}public override bool Contains(string n){return p.Any(x=>x.ParameterName==n);}public override void CopyTo(Array a,int i){((ICollection)p).CopyTo(a,i);}public override IEnumerator GetEnumerator(){return p.GetEnumerator();}
  public override int IndexOf(object v){return p.IndexOf((DbParameter)v);}public override int IndexOf(string n){return p.FindIndex(x=>x.ParameterName==n);}public override void Insert(int i,object v){p.Insert(i,(DbParameter)v);}public override void Remove(object v){p.Remove((DbParameter)v);}public override void RemoveAt(int i){p.RemoveAt(i);}public override void RemoveAt(string n){p.RemoveAt(IndexOf(n));}
  protected override DbParameter GetParameter(int i){return p[i];}protected override DbParameter GetParameter(string n){return p[IndexOf(n)];}protected override void SetParameter(int i,DbParameter v){p[i]=v;}protected override void SetParameter(string n,DbParameter v){p[IndexOf(n)]=v;}
 }
}
