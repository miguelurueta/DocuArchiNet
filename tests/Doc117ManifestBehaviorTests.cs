using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
public static class Doc117ManifestBehaviorTests {
 static void Check(bool value,string code){if(!value)throw new Exception(code);}
 public static int Main(string[] args){
  var manifest=JObject.Parse(File.ReadAllText(args[0]));var resolver=new ResolvedorReglasPerfilImportacion();
  foreach(var item in manifest["cases"]){
   var rules=item["rules"].ToObject<List<ReglaPerfilImportacion>>();
   Check(resolver.ValidarEstructura(rules).Count==0,"RULE_STRUCTURE");
   var example=new EjemploPerfilImportacion{NombreCarpeta=(string)item["rootName"],NombreArchivo=(string)item["fileName"]};
   var pre=resolver.Resolver(rules,example);Check(pre.Incidencias.Count==0&&pre.Valores.Count==5,"PREFLIGHT");
   Func<string,string,ValorReglaPerfilImportacion> get=(type,reference)=>pre.Valores.Single(v=>v.DestinoTipo==type&&v.DestinoRef==reference);
   Check(get("CAMPO_EXPEDIENTE","CODIGO").Valor==(string)item["expected"]["codigo"],"LEADING_ZERO_CODE");
   Check(get("CAMPO_EXPEDIENTE","NOMBRE").Valor==(string)item["rootName"],"ROOT_NAME");
   Check(get("CAMPO_EXPEDIENTE","FECHA_INICIAL").Valor=="2026-11-05","CANONICAL_DATE");
   Check(get("TIPOLOGIA","TIPO_DOCUMENTAL").Valor=="79","REFERENCE_TYPE");
   Check(get("INDICE_GABINETE","14").Diferido&&get("INDICE_GABINETE","14").Valor==null,"NO_INVENTED_EXPEDITION_ID");
   example.Expediente["ID"]="8675309"; // Identidad sintetica del doble local; nunca un recurso del ambiente.
   var post=resolver.Resolver(rules,example);var index=post.Valores.Single(v=>v.DestinoTipo=="INDICE_GABINETE"&&v.DestinoRef=="14");
   Check(post.Incidencias.Count==0&&!index.Diferido&&index.Valor=="8675309","DEFERRED_BINDING");
  }
  Console.WriteLine("DOC117_MANIFEST_ENGINE=PASS; CASES=2; REAL_EFFECTS=0");return 0;
 }
}
