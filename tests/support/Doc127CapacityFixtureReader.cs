using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
public static class Doc127FixtureReader {
 public static int Main(string[] args) {try{
  var manifest=JObject.Parse(File.ReadAllText(args[0]));if((bool?)manifest["synthetic"]!=true||(bool?)manifest["realExecution"]!=false)throw new Exception("SYNTHETIC_REQUIRED");
  var results=new List<object>();
  foreach(var c in manifest["cases"]){
   var file=(string)c["archivoPath"];var bytes=File.ReadAllBytes(file);string hash;
   using(var sha=SHA256.Create())hash=BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-","").ToLowerInvariant();
   if(hash!=(string)c["archivoHash"]||bytes.Length!=(int)c["bytes"])throw new Exception("HASH_CHANGED");
   var p=new PlantillaExcel{NOMBRE_PLANTILLA="imp_02_sintetico",CAMPO_UNICO="CLAVE",Modo=(string)c["modo"],Formato=(string)c["formato"]};
   var d=new DestinoPlantillaExcel{Existe=p.Modo!="CREAR",Motor="InnoDB"};
   foreach(var field in new[]{"CLAVE","VALOR"}){p.Campos.Add(new CampoPlantillaExcel{CAMPO_FUENTE=field,CAMPO_DESTINO=field,TIPO_CAMPO="VARCHAR(250)"});d.Columnas.Add(new ColumnaDestinoExcel{Nombre=field,Tipo="varchar",TipoCompleto="varchar(250)",Longitud=250,Charset="utf8",Nulable=false});}
   ResultadoLecturaExcel r;using(var f=File.OpenRead(file))r=new ServicioLecturaExcel().Leer(f,new SolicitudLecturaExcel{NombreArchivo=Path.GetFileName(file),FormatoEsperado=(string)c["formato"],Hoja="Datos",Plantilla=p,Destino=d,Limites=new LimitesLecturaExcel{MaxFilasDatos=20000,MaxArchivoMB=10}});
   if(!r.Valido||!r.LecturaCompleta||r.FilasDatosObservadas!=(int)c["filasEsperadas"]||r.Filas.Count!=(int)c["filasEsperadas"])throw new Exception("READER_REJECTED_"+string.Join("_",r.Incidencias.Select(x=>x.Codigo)));
   results.Add(new{formato=(string)c["formato"],modo=(string)c["modo"],filas=r.Filas.Count,archivoHash=hash,bytes=bytes.Length,localReader=true,realMySql=false});
  }
  File.WriteAllText(args[1],Newtonsoft.Json.JsonConvert.SerializeObject(new{status="LOCAL_READER_PASS",synthetic=true,realE2E=false,cases=results},Newtonsoft.Json.Formatting.Indented));
  Console.WriteLine("DOC127_LOCAL_READER_PASS cases="+results.Count+"; synthetic workbooks; no MySQL/HTTP");return 0;
 }catch(Exception e){Console.Error.WriteLine("DOC127_LOCAL_READER_FAILED "+e.GetType().Name+" "+e.Message);return 1;}}
}
