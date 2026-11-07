using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using System.Web;
using System.Web.SessionState;
using Newtonsoft.Json;
public static class Doc117Alm03ClosedGuardBehaviorTests {
 static Type Guard;static readonly Type Owner=typeof(ImportacionCarpetasBehaviorTests);
 static void Check(bool v,string c){if(!v)throw new Exception(c);}
 static T Field<T>(object f,string name){return(T)f.GetType().GetField(name).GetValue(f);}
 static object Call(object target,string name,params object[] args){try{return target.GetType().GetMethod(name).Invoke(target,args);}catch(TargetInvocationException e){throw e.InnerException;}}
 static object Lab(string name,params object[] args){try{return Guard.GetMethod(name,BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,args);}catch(TargetInvocationException e){throw e.InnerException;}}
 static void Reject(Action a,string code){try{a();}catch(Exception e){Check(e.Message.Contains(code),"WRONG_REJECTION_"+code);return;}throw new Exception("MISSING_REJECTION_"+code);}
 static string Hash(byte[] bytes){using(var s=SHA256.Create())return BitConverter.ToString(s.ComputeHash(bytes)).Replace("-","").ToLowerInvariant();}
 static object Source(bool b,byte[] pdf){
  var f=Activator.CreateInstance(Owner.GetNestedType("DocumentAdapterFixture",BindingFlags.NonPublic),true);var lot=Field<LoteImportacionCarpetas>(f,"Lot");var r=Field<RaizImportacionCarpetas>(f,"Root");var a=Field<ArchivoImportacionCarpetas>(f,"FileRecord");var i=Field<IntentoImportacionCarpetas>(f,"Attempt");
  lot.LoteId="22";lot.Contexto.UsuarioRef="141";lot.Contexto.AmbitoRef="2";lot.Contexto.NivelRef="7";lot.Snapshot.VersionId="38";lot.Snapshot.Configuracion.GabineteId=10;lot.Snapshot.Configuracion.OrigenGabinete="DA";
  foreach(var v in lot.Snapshot.Reglas.Where(x=>x.DestinoTipo=="TIPOLOGIA"))v.ValorConstante="79";
  r.LoteId="22";r.RaizId=b?"36":"35";r.IdentidadExpediente.NivelRef="7";r.IdentidadExpediente.ExpedienteId=b?"667":"666";r.IdentidadExpediente.RelacionNivelId=b?"1092":"1091";
  a.RaizId=r.RaizId;a.ArchivoId=b?"37":"36";a.IntentoActivoId=b?"9902":"9901";a.NombreOriginal=b?"DOC117-ALM03-IMPORT-B-20261106.pdf":"DOC117-ALM03-IMPORT-A-20261106.pdf";a.RutaRelativa="Folder/"+a.NombreOriginal;a.BytesPrevistos="677";a.BytesRecibidos="677";a.ContenidoHash=Hash(pdf);
  foreach(var v in a.ValoresEfectivos.Where(x=>x.DestinoTipo=="TIPOLOGIA"))v.Valor="79";foreach(var v in a.ValoresPlan.Where(x=>x.DestinoTipo=="TIPOLOGIA"))v.Valor="79";
  i.LoteId="22";i.RaizId=r.RaizId;i.ArchivoId=a.ArchivoId;i.IntentoId=a.IntentoActivoId;Call(f,"Rehash");i.SolicitudHash=Doc117SolicitudHash.Calcular(lot.Contexto,"22",a.ArchivoId,a.Revision-1,b?"closed-b":"closed-a","ALMACENAR");return f;
 }
 static object Resource(object f){var c=(ComandoAlmacenamientoCarpetas)Call(f,"Prepare");return new{rootId=c.Raiz.RaizId,fileId=c.Archivo.ArchivoId,expedienteId=c.Raiz.IdentidadExpediente.ExpedienteId,relacionNivelId=c.Raiz.IdentidadExpediente.RelacionNivelId,fileName=c.Archivo.NombreOriginal,bytes="677",contentSha256=c.Archivo.ContenidoHash,requestSha256=c.Intento.SolicitudHash};}
 static HttpContext Context(bool individual,string session){
  var request=new HttpRequest("",individual?"https://localhost/GestionDocumental-Docuarchi.net/generic_control/FileUploadHandler_.ashx":"https://localhost/GestionDocumental-Docuarchi.net/webservice/WebServiceImportacionCarpetas.asmx/AvanzarLote","");var h=new HttpContext(request,new HttpResponse(new StringWriter()));
  SessionStateUtility.AddHttpSessionStateToContext(h,new HttpSessionStateContainer(session,new SessionStateItemCollection(),new HttpStaticObjectsCollection(),20,true,HttpCookieMode.UseCookies,SessionStateMode.InProc,false));h.Session["GA_IDUSUARIOGESTION"]="141";h.Session["GA_IDEMPRESA"]="2";h.Session["PG_SELECCION_TREVIEEW_PRODUCCION"]="1072|8|467";h.Session["PG_SELECCION_ID_EXPEIDENTE"]="467";h.Session["WF_TIPO_ADJUNTA"]="PRODUCCION";return h;
 }
 static void Finish(object ticket,int n){ticket.GetType().GetMethod("Salir",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(ticket,new object[]{"YES",20000+n,30000+n});}
 public static int Main(string[] args){
  try{
   byte[] pdf=File.ReadAllBytes(args[2]);object a=Source(false,pdf),b=Source(true,pdf);
   try{
    if(args[0]=="--describe"){
     var lot=Field<LoteImportacionCarpetas>(a,"Lot");var scope=new{individual=new{expedienteId="467",levelId="8",relacionNivelId="1072",typologyId="50",fileName="DOC117-ALM03-IND-20261106.pdf",classificationSha256=new string('f',64),cabinetName="gab_fixture",bytes="677",contentSha256=Hash(pdf)},issueKey="DOC-117",protocol="ALM03_CLOSED_RESOURCES_BEFORE_WRITER_V1",runtimeRoot=args[3],runId=new string('e',32),lotId="22",snapshotSha256=lot.Snapshot.Huella,resources=new[]{Resource(a),Resource(b)}};File.WriteAllText(args[1],JsonConvert.SerializeObject(scope));return 0;
    }
    Guard=Assembly.LoadFrom(args[1]).GetType("Doc117Alm03ClosedGuard");var ca=(ComandoAlmacenamientoCarpetas)Call(a,"Prepare");var cb=(ComandoAlmacenamientoCarpetas)Call(b,"Prepare");var da=Field<DatosAlmacenamientoImportacion>(a,"Data");var db=Field<DatosAlmacenamientoImportacion>(b,"Data");
    string baseDir=AppDomain.CurrentDomain.BaseDirectory,dir=Path.Combine(baseDir,"App_Data","ImportacionCarpetas","test");Directory.CreateDirectory(dir);string pa=Path.Combine(dir,ca.Archivo.NombreOriginal),pb=Path.Combine(dir,cb.Archivo.NombreOriginal);File.WriteAllBytes(pa,pdf);File.WriteAllBytes(pb,pdf);
    string individualDir=Path.Combine(baseDir,"Temp_Image","upload_file","141");Directory.CreateDirectory(individualDir);string pi=Path.Combine(individualDir,"DOC117-ALM03-IND-20261106.pdf");File.WriteAllBytes(pi,pdf);
    HttpContext.Current=Context(false,"synthetic-import-session");Reject(()=>Lab("Entrar","gab_fixture",pa,ca.Archivo.NombreOriginal,666,79,0,da.ClaseDocumentoNombre),"UNBOUND_WRITER");
    HttpContext.Current.Session["GA_IDUSUARIOGESTION"]="999";Reject(()=>Lab("PrepararImportacion",ca,da,pa),"CONTEXT_CHANGED");HttpContext.Current.Session["GA_IDUSUARIOGESTION"]="141";
    File.WriteAllBytes(pa,new byte[677]);Reject(()=>Lab("PrepararImportacion",ca,da,pa),"FILE_CHANGED");File.WriteAllBytes(pa,pdf);
    var altered=JsonConvert.DeserializeObject<IntentoImportacionCarpetas>(JsonConvert.SerializeObject(ca.Intento));altered.SolicitudHash=new string('0',64);altered.EsNuevaReserva=true;var lotA=Field<LoteImportacionCarpetas>(a,"Lot");var wrong=ComandoAlmacenamientoCarpetas.Preparar(lotA.Contexto,Field<RaizImportacionCarpetas>(a,"Root"),Field<ArchivoImportacionCarpetas>(a,"FileRecord"),lotA.Snapshot,altered);Reject(()=>Lab("PrepararImportacion",wrong,da,pa),"RESOURCE_CHANGED");
    Exception e1=null,e2=null;using(var started=new ManualResetEvent(false))using(var finished=new ManualResetEvent(false)){
     var first=new Thread(()=>{try{HttpContext.Current=Context(false,"synthetic-import-session");using((IDisposable)Lab("PrepararImportacion",ca,da,pa)){Reject(()=>Lab("Entrar","other",pa,ca.Archivo.NombreOriginal,666,79,0,da.ClaseDocumentoNombre),"WRITER_ARGUMENTS_CHANGED");var ticket=Lab("Entrar","gab_fixture",pa,ca.Archivo.NombreOriginal,666,79,0,da.ClaseDocumentoNombre);started.Set();if(!finished.WaitOne(5000))throw new Exception("SYNTHETIC_WAIT_TIMEOUT");Finish(ticket,1);}}catch(Exception e){e1=e;started.Set();}});
     var second=new Thread(()=>{try{if(!started.WaitOne(5000))throw new Exception("SYNTHETIC_WAIT_TIMEOUT");HttpContext.Current=Context(true,"synthetic-import-session");HttpContext.Current.Session["WF_RUTA_TEMPO_ADJUNTA"]=pi;Reject(()=>Lab("PrepararIndividual","gab_fixture",pi,"Comprobante de Egreso",467,50,2,da.ClaseDocumentoNombre),"SESSIONS_NOT_DISTINCT");HttpContext.Current=Context(true,"synthetic-individual-session");HttpContext.Current.Session["WF_RUTA_TEMPO_ADJUNTA"]=pi;HttpContext.Current.Session["PG_SELECCION_TREVIEEW_PRODUCCION"]="1072|7|467";Reject(()=>Lab("PrepararIndividual","gab_fixture",pi,"Comprobante de Egreso",467,50,2,da.ClaseDocumentoNombre),"INDIVIDUAL_SELECTION_CHANGED");HttpContext.Current.Session["PG_SELECCION_TREVIEEW_PRODUCCION"]="1072|8|467";using((IDisposable)Lab("PrepararIndividual","gab_fixture",pi,"Comprobante de Egreso",467,50,2,da.ClaseDocumentoNombre)){var ticket=Lab("Entrar","gab_fixture",pi,"Comprobante de Egreso",467,50,2,da.ClaseDocumentoNombre);Finish(ticket,2);}finished.Set();}catch(Exception e){e2=e;finished.Set();}});
     first.Start();second.Start();first.Join();second.Join();Check(e1==null&&e2==null,"BOUND_THREADS_FAILED_"+(e1==null?"":e1.Message)+"_"+(e2==null?"":e2.Message));
    }
    HttpContext.Current=Context(false,"synthetic-import-session");using((IDisposable)Lab("PrepararImportacion",cb,db,pb)){var ticket=Lab("Entrar","gab_fixture",pb,cb.Archivo.NombreOriginal,667,79,0,db.ClaseDocumentoNombre);Finish(ticket,3);}
    Reject(()=>Lab("PrepararImportacion",ca,da,pa),"ATTEMPT_ALREADY_PINNED");
    var journal=Path.Combine(baseDir,"App_Data","DOC117-ALM03-"+new string('e',32));Check(File.Exists(Path.Combine(journal,"summary.json")),"NO_COMPLETE_SEAL");foreach(var f in Directory.GetFiles(journal,"*.json"))Check(!File.ReadAllText(f).Contains("synthetic-import-session")&&!File.ReadAllText(f).Contains("synthetic-individual-session"),"RAW_SESSION_PERSISTED");Console.WriteLine("DOC117_ALM03_CLOSED_CALLERS_WITH_PRODUCT_COMMAND: PASS");return 0;
   }finally{((IDisposable)a).Dispose();((IDisposable)b).Dispose();}
  }catch(Exception e){Console.Error.WriteLine("DOC117_ALM03_CLOSED_CALLERS: FAILED "+e.Message);return 2;}
 }
}
