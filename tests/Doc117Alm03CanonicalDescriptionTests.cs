using System;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.SessionState;
public static class Doc117Alm03CanonicalDescriptionTests {
 static Type Guard;
 static void Check(bool ok,string code){if(!ok)throw new Exception(code);}
 static object Call(Type type,string name,params object[] args){try{return type.GetMethod(name,BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,args);}catch(TargetInvocationException ex){throw ex.InnerException;}}
 static void Reject(Action action,string code){try{action();}catch(Exception ex){Check(ex.Message==code,"UNEXPECTED_REJECTION");return;}throw new Exception("MISSING_REJECTION");}
 public static int Main(string[] args){try{
  var assembly=Assembly.LoadFrom(args[0]);Guard=assembly.GetType("Doc117Alm03ClosedGuard");var canonical=System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(args[2]));
  string root=AppDomain.CurrentDomain.BaseDirectory,dir=Path.Combine(root,"Temp_Image","upload_file","141");Directory.CreateDirectory(dir);string file=Path.Combine(dir,"DOC117-ALM03-IND-20261106.pdf");var pdf=File.ReadAllBytes(args[1]);File.WriteAllBytes(file,pdf);
  var h=new HttpContext(new HttpRequest("","https://localhost/GestionDocumental-Docuarchi.net/generic_control/FileUploadHandler_.ashx",""),new HttpResponse(new StringWriter()));SessionStateUtility.AddHttpSessionStateToContext(h,new HttpSessionStateContainer("PRIVATE_SYNTHETIC_SESSION",new SessionStateItemCollection(),new HttpStaticObjectsCollection(),20,true,HttpCookieMode.UseCookies,SessionStateMode.InProc,false));HttpContext.Current=h;
  h.Session["GA_IDUSUARIOGESTION"]="141";h.Session["GA_IDEMPRESA"]="2";h.Session["PG_SELECCION_TREVIEEW_PRODUCCION"]="1072|8|467";h.Session["PG_SELECCION_ID_EXPEIDENTE"]="467";h.Session["WF_TIPO_ADJUNTA"]="PRODUCCION";h.Session["WF_RUTA_TEMPO_ADJUNTA"]=file;
  Action<string,int> prepare=(name,type)=>{using((IDisposable)Call(Guard,"PrepararIndividual","CONTABIL",file,name,467,type,2,"DOCUMENTO ELECTRONICO")) { }};
  Reject(()=>prepare(canonical+" ",50),"DOC117_ALM03_GUARD_INDIVIDUAL_ARGUMENTS_CHANGED");Reject(()=>prepare("Otra tipologia",50),"DOC117_ALM03_GUARD_INDIVIDUAL_ARGUMENTS_CHANGED");Reject(()=>prepare(canonical,79),"DOC117_ALM03_GUARD_INDIVIDUAL_ARGUMENTS_CHANGED");
  if(args[3]=="dormant"){Reject(()=>prepare(canonical,50),"DOC117_ALM03_GUARD_INDIVIDUAL_ARGUMENTS_CHANGED");Console.WriteLine("CANONICAL_GUARD: PASS DORMANT");return 0;}
  h.Session["PG_SELECCION_TREVIEEW_PRODUCCION"]="1072|7|467";Reject(()=>prepare(canonical,50),"DOC117_ALM03_GUARD_INDIVIDUAL_SELECTION_CHANGED");h.Session["PG_SELECCION_TREVIEEW_PRODUCCION"]="1072|8|467";
  File.WriteAllBytes(file,new byte[677]);Reject(()=>prepare(canonical,50),"DOC117_ALM03_GUARD_FILE_CHANGED");File.WriteAllBytes(file,pdf);
  using((IDisposable)Call(Guard,"PrepararIndividual","CONTABIL",file,canonical,467,50,2,"DOCUMENTO ELECTRONICO")) {Reject(()=>Call(Guard,"Entrar","CONTABIL",file,canonical+" ",467,50,2,"DOCUMENTO ELECTRONICO"),"DOC117_ALM03_GUARD_WRITER_ARGUMENTS_CHANGED");}
  Reject(()=>Call(assembly.GetType("Doc117Alm03MethodObserver"),"Entrar","CONTABIL",file,canonical,467,50,2,"DOCUMENTO ELECTRONICO"),"DOC117_ALM03_DORMANT_SCOPE");
  foreach(var f in Directory.GetFiles(Path.Combine(root,"App_Data"),"*.json",SearchOption.AllDirectories)){Check(Path.GetFileName(f)=="reserved-INDIVIDUAL.json","WRITER_ENTRY_OBSERVED");Check(!File.ReadAllText(f).Contains("PRIVATE"),"SESSION_LEAK");}
  Check(File.ReadAllBytes(file).Length==677,"TEMP_CHANGED");Console.WriteLine("CANONICAL_GUARD: PASS BOUND");return 0;
 }catch{Console.Error.WriteLine("CANONICAL_GUARD: FAILED");return 2;}}
}
