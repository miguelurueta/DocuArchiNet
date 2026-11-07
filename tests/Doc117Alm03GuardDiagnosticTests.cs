using System;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.SessionState;
public static class Doc117Alm03GuardDiagnosticTests {
 static void Check(bool value,string code){if(!value)throw new Exception(code);}
 static string Invoke(Type type,object[] args){try{return(string)type.GetMethod("Evaluar",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,args);}catch(TargetInvocationException){throw new Exception("PROBE_THROW_NOT_MASKED");}}
 public static int Main(string[] args){try{
  string mode=args[0],run=args[1],baseDir=AppDomain.CurrentDomain.BaseDirectory;
  string directory=Path.Combine(baseDir,"Temp_Image","upload_file","141");Directory.CreateDirectory(directory);
  string file=Path.Combine(directory,"DOC117-ALM03-GUARD-DIAG-20261106.pdf");File.Copy(args[2],file,true);
  var h=new HttpContext(new HttpRequest("","https://localhost/GestionDocumental-Docuarchi.net/generic_control/FileUploadHandler_.ashx",""),new HttpResponse(new StringWriter()));
  SessionStateUtility.AddHttpSessionStateToContext(h,new HttpSessionStateContainer("PRIVATE_SESSION",new SessionStateItemCollection(),new HttpStaticObjectsCollection(),20,true,HttpCookieMode.UseCookies,SessionStateMode.InProc,false));
  h.Session["GA_IDUSUARIOGESTION"]=mode=="wrong-actor"?"999":"141";h.Session["GA_IDEMPRESA"]="2";h.Session["PG_SELECCION_TREVIEEW_PRODUCCION"]=mode=="wrong-level"?"1072|7|467":"1072|8|467";h.Session["PG_SELECCION_ID_EXPEIDENTE"]="467";h.Session["WF_TIPO_ADJUNTA"]="PRODUCCION";h.Session["WF_RUTA_TEMPO_ADJUNTA"]=file;h.Session["PRIVATE_PASSWORD"]="PRIVATE_PASSWORD";HttpContext.Current=h;
  if(mode=="content")File.WriteAllBytes(file,new byte[677]);
  string proofDir=Path.Combine(baseDir,"App_Data","DOC117-ALM03-GUARD-DIAG-"+run),proof=Path.Combine(proofDir,"probe.json");Directory.CreateDirectory(Path.GetDirectoryName(proofDir));if(mode=="io")File.WriteAllText(proofDir,"PRIVATE_SENTINEL");
  var assembly=Assembly.LoadFrom(Path.Combine(baseDir,"Probe.dll"));var type=assembly.GetType("Doc117Alm03GuardDiagnostic");
  var values=new object[]{"gab_fixture",mode=="unknown"?null:file,mode=="description-case"?"COMPROBANTE DE EGRESO":"Comprobante de Egreso",467,50,2,"DOCUMENTO ELECTRONICO"};
  string result=Invoke(type,values);Check(result==(mode=="wrong-actor"?"DOC117_ALM03_DIAGNOSTIC_CONTEXT_REJECTED":mode=="io"?"DOC117_ALM03_DIAGNOSTIC_EVIDENCE_FAILED":"DOC117_ALM03_DIAGNOSTIC_WRITER_BLOCKED"),"UNEXPECTED_TERMINAL_CODE");
  if(mode=="duplicate"){byte[] before=File.ReadAllBytes(proof);Check(Invoke(type,values)=="DOC117_ALM03_DIAGNOSTIC_EVIDENCE_FAILED","DUPLICATE_NOT_REJECTED");Check(Convert.ToBase64String(before)==Convert.ToBase64String(File.ReadAllBytes(proof)),"PROOF_OVERWRITTEN");}
  if(File.Exists(proof))Check(!File.ReadAllText(proof).Contains("PRIVATE_"),"SECRET_RETAINED");
  Check(File.ReadAllBytes(file).Length==677,"SOURCE_CONSUMED");
  var observer=assembly.GetType("Doc117Alm03MethodObserver");bool blocked=false;try{observer.GetMethod("Entrar",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,values);}catch(TargetInvocationException e){blocked=e.InnerException.Message=="DOC117_ALM03_DORMANT_SCOPE";}Check(blocked,"WRITER_NOT_DORMANT");
  Console.WriteLine("DOC117_ALM03_GUARD_DIAGNOSTIC_LOCAL: PASS "+mode);return 0;
 }catch(Exception){Console.Error.WriteLine("DOC117_ALM03_GUARD_DIAGNOSTIC_LOCAL: FAILED");return 2;}}
}
