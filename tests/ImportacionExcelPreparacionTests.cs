using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Security.Cryptography;
using Newtonsoft.Json;

//Amplía el ejecutable DOC122: archivo/lector reales, autoridad/persistencia/ADO sintéticos.
//Las comparaciones del doble no acreditan collation ni concurrencia de un servidor MySQL.
public static class ImportacionExcelPreparacionTests {
 static int checks;
 static void Check(bool v,string name){checks++;if(!v)throw new Exception("DOC124 "+name);}
 static void Reject(Action f,string code){try{f();}catch(ImportacionExcelException e){Check(e.Codigo==code,"expected "+code+" got "+e.Codigo);return;}throw new Exception("DOC124 missing "+code);}
 static T Copy<T>(T o){return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(o));}
 static ContextoCargaExcel Context(){return new ContextoCargaExcel{UsuarioId=1,EmpresaId=2,ModuloId=3,ConexionId=7,Autorizado=true,IdentidadConexion=new string('a',64)};}
 static PlantillaExcel Template(){return new PlantillaExcel{ID_PLANTILLAIMP=5,IMP01_XMLCONEXION_ID_XMLCONEXION=7,NOMBRE_PLANTILLA="imp_02_doc124_sintetico",CONEXION_FUENTE="12.0 xlsx",LIBRO_EXCEL="Datos",CAMPO_UNICO="CLAVE",REVISION_CARGA=1,INSERTA_RECORD=1,ESTADO_PLANTILLA=1,Campos=new List<CampoPlantillaExcel>{new CampoPlantillaExcel{ID_CAMPOS_PLANTILLA=1,CAMPO_FUENTE="Código",CAMPO_DESTINO="CLAVE",TIPO_CAMPO="VARCHAR"}}};}
 static ColumnaDestinoExcel Col(string n="CLAVE"){return new ColumnaDestinoExcel{Nombre=n,Tipo="varchar",TipoCompleto="varchar(250)",Longitud=250,Nulable=false,Charset="latin1",Collation="latin1_swedish_ci",Extra=""};}
 static EsquemaCargaExcel Schema(bool exists=true){var e=new EsquemaCargaExcel{Destino=new DestinoPlantillaExcel{Existe=exists,Motor="InnoDB",Columnas=new List<ColumnaDestinoExcel>{Col()}},IdentidadFisica=new string('b',64),Politica=new PoliticaDestinoCargaExcel{FuenteId=exists?"9":null,Revision=exists?1u:0u,Modo=exists?"CONEXION":"PROYECTADA"},Indices=new List<IndiceCargaExcel>{new IndiceCargaExcel{Nombre="uq_clave",Unico=true,Componentes=new List<ComponenteIndiceCargaExcel>{new ComponenteIndiceCargaExcel{Columna="CLAVE",Posicion=1}}}}};e.Hash=MySqlPreparacionExcelValidator.HuellaEsquema(e);return e;}
 static byte[] Book(params object[][] rows){return ImportacionExcelLecturaTests.Book(rows,new[]{"Código"});}
 static void Tokens(){
  var now=new DateTime(2026,11,23,12,0,0,DateTimeKind.Utc);var c=Context();var p=Template();p.Formato="XLSX";
  //Firma sintética sólo para inyectar reloj y fallos; protección real es MachineKey del producto.
  Func<byte[],byte[]> protect=b=>{using(var h=new HMACSHA256(Encoding.ASCII.GetBytes("synthetic-test-key")))return h.ComputeHash(b).Concat(b).ToArray();};
  Func<byte[],byte[]> unprotect=b=>{if(b.Length<32)throw new FormatException();var plain=b.Skip(32).ToArray();if(!protect(plain).SequenceEqual(b))throw new FormatException();return plain;};
  var codec=new ReferenciaRecepcionExcel(protect,unprotect,()=>now);var i=codec.Emitir(c,p);Check(ReglasCargaExcel.IdValido(i.IdCarga)&&i.ExpiraUtc==now.AddMinutes(30),"prior ID and expiry");
  var s=new SolicitudRecepcionExcel{IdCarga=i.IdCarga,ConexionId=7,PlantillaId=5,ReferenciaRecepcion=i.ReferenciaRecepcion};codec.Validar(c,p,s);checks++;
  foreach(var mutate in new Action<ContextoCargaExcel>[] {x=>x.UsuarioId++,x=>x.EmpresaId++,x=>x.ModuloId++,x=>x.ConexionId++,x=>x.IdentidadConexion=new string('c',64)}){var changed=Copy(c);mutate(changed);Reject(()=>codec.Validar(changed,p,s),"REFERENCIA_NO_DISPONIBLE");}
  var other=Copy(s);other.IdCarga=Guid.NewGuid().ToString("D");Reject(()=>codec.Validar(c,p,other),"REFERENCIA_NO_DISPONIBLE");
  other=Copy(s);var bytes=Convert.FromBase64String(other.ReferenciaRecepcion);bytes[bytes.Length-1]^=1;other.ReferenciaRecepcion=Convert.ToBase64String(bytes);Reject(()=>codec.Validar(c,p,other),"REFERENCIA_NO_DISPONIBLE");
  p.REVISION_CARGA++;Reject(()=>codec.Validar(c,p,s),"REFERENCIA_NO_DISPONIBLE");p.REVISION_CARGA--;now=now.AddMinutes(30);Reject(()=>codec.Validar(c,p,s),"REFERENCIA_NO_DISPONIBLE");
  var real=new ReferenciaRecepcionExcel();var emitted=real.Emitir(c,p);real.Validar(c,p,new SolicitudRecepcionExcel{IdCarga=emitted.IdCarga,ConexionId=7,PlantillaId=5,ReferenciaRecepcion=emitted.ReferenciaRecepcion});checks++;
  Check(!ReglasCargaExcel.IdValido(Guid.Empty.ToString("D"))&&!ReglasCargaExcel.IdValido(Guid.NewGuid().ToString("N")),"canonical UUID only");
 }
 static void Multipart(){
  var w=new ImportacionExcelBehaviorTests.Worker();var h=ImportacionExcelBehaviorTests.Http(w);w.Origin=h.Request.Url.GetLeftPart(System.UriPartial.Authority);var c=Context();w.Token=ImportacionExcelRequestGuard.Emitir(h,c);
  w.Content="multipart/form-data; boundary=doc124-synthetic";w.Bytes=100;h=ImportacionExcelBehaviorTests.Http(w,h);ImportacionExcelRequestGuard.ValidarMultipart(h,c,new LimitesLecturaExcel());checks++;
  foreach(var content in new[]{"application/json","multipart/form-data","multipart/form-data; boundary=","multipart/form-data; boundary=x; another=y","multipart/form-data; boundary="+new string('a',71)}){w.Content=content;var bad=ImportacionExcelBehaviorTests.Http(w,h);Reject(()=>ImportacionExcelRequestGuard.ValidarMultipart(bad,c,new LimitesLecturaExcel()),"SOLICITUD_NO_VERIFICADA");}
  w.Content="multipart/form-data; boundary=\"abc-x\"";w.Bytes=10262144;ImportacionExcelRequestGuard.ValidarMultipart(ImportacionExcelBehaviorTests.Http(w,h),c,new LimitesLecturaExcel());checks++;
  w.Bytes++;Reject(()=>ImportacionExcelRequestGuard.ValidarMultipart(ImportacionExcelBehaviorTests.Http(w,h),c,new LimitesLecturaExcel()),"SOLICITUD_NO_VERIFICADA");
  w.Bytes=100;w.Token="bad";Reject(()=>ImportacionExcelRequestGuard.ValidarMultipart(ImportacionExcelBehaviorTests.Http(w,h),c,new LimitesLecturaExcel()),"SOLICITUD_NO_VERIFICADA");
  w.Token=(string)h.Session["DOC122_EXCEL_CSRF"];w.Origin="https://other.invalid";Reject(()=>ImportacionExcelRequestGuard.ValidarMultipart(ImportacionExcelBehaviorTests.Http(w,h),c,new LimitesLecturaExcel()),"SOLICITUD_NO_VERIFICADA");
  w.Origin=h.Request.Url.GetLeftPart(System.UriPartial.Authority);w.Content="multipart/form-data; boundary=doc124";
  var id=Guid.NewGuid().ToString("D");var fields=new List<KeyValuePair<string,string>>{new KeyValuePair<string,string>("idCarga",id),new KeyValuePair<string,string>("referenciaRecepcion","opaque"),new KeyValuePair<string,string>("conexionId","7"),new KeyValuePair<string,string>("plantillaId","5")};
  Func<int,byte[]> body=files=>Encoding.UTF8.GetBytes(string.Concat(fields.Select(k=>"--doc124\r\nContent-Disposition: form-data; name=\""+k.Key+"\"\r\n\r\n"+k.Value+"\r\n"))+string.Concat(Enumerable.Range(0,files).Select(n=>"--doc124\r\nContent-Disposition: form-data; name=\"archivo\"; filename=\"x.xlsx\"\r\nContent-Type: application/octet-stream\r\n\r\n12345678\r\n"))+"--doc124--\r\n");
  w.Body=body(1);w.Bytes=w.Body.Length;var parsed=ImportacionExcelRequestGuard.LeerMultipart(ImportacionExcelBehaviorTests.Http(w,h));Check(parsed.IdCarga==id&&parsed.ConexionId==7&&parsed.PlantillaId==5,"actual multipart parser fields");
  fields.Add(new KeyValuePair<string,string>("idCarga",id));w.Body=body(1);w.Bytes=w.Body.Length;Reject(()=>ImportacionExcelRequestGuard.LeerMultipart(ImportacionExcelBehaviorTests.Http(w,h)),"SOLICITUD_NO_VERIFICADA");fields.RemoveAt(fields.Count-1);
  fields.Add(new KeyValuePair<string,string>("extra","1"));w.Body=body(1);w.Bytes=w.Body.Length;Reject(()=>ImportacionExcelRequestGuard.LeerMultipart(ImportacionExcelBehaviorTests.Http(w,h)),"SOLICITUD_NO_VERIFICADA");fields.RemoveAt(fields.Count-1);
  w.Body=body(2);w.Bytes=w.Body.Length;Reject(()=>ImportacionExcelRequestGuard.LeerMultipart(ImportacionExcelBehaviorTests.Http(w,h)),"SOLICITUD_NO_VERIFICADA");
 }
 static void Files(){
  var root=Path.Combine(Path.GetTempPath(),"doc124-files-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(root);
  try{
   var a=new AlmacenArchivoExcel(root);var c=Context();var l=new LimitesLecturaExcel();
   foreach(var ext in new[]{"xls","xlsx"}){
    var bytes=File.ReadAllBytes(Path.Combine("excel-fixtures","valido."+ext));var id=Guid.NewGuid().ToString("D");ArchivoRecibidoExcel file;
    using(var input=new MemoryStream(bytes))file=a.Recibir(c,id,@"C:\fakepath\prueba."+ext,ext.ToUpperInvariant(),input,l);
    Check(file.Nombre=="prueba."+ext&&file.Bytes==bytes.Length&&file.Hash==ReglasCargaExcel.Hash(bytes)&&file.Hojas.SequenceEqual(new[]{"Datos","Otra"}),"physical receipt "+ext);
    using(var retained=a.Retener(c,id,file)){
     retained.Contenido.Position=3;retained.Verificar();Check(retained.Contenido.Position==3,"retention position");
     try{using(var write=File.Open(Path.Combine(root,file.Referencia),FileMode.Open,FileAccess.Write,FileShare.ReadWrite)){}throw new Exception("write allowed");}catch(IOException){checks++;}
    }
    using(var input=new MemoryStream(bytes))Check(a.Recibir(c,id,file.Nombre,ext.ToUpperInvariant(),input,l).Hash==file.Hash,"explicit replay identical");
    var changed=Copy(file);changed.Hash=new string('0',64);Reject(()=>{using(var held=a.Retener(c,id,changed)){}},"ARCHIVO_ALTERADO");
    var old=File.ReadAllBytes(Path.Combine(root,file.Referencia));File.WriteAllBytes(Path.Combine(root,file.Referencia),new byte[]{1,2,3});Reject(()=>{using(var held=a.Retener(c,id,file)){}},"ARCHIVO_ALTERADO");File.WriteAllBytes(Path.Combine(root,file.Referencia),old);
   }
   foreach(var name in new[]{"",new string('x',256),"a\u0001.xlsx","a\ud83d\ude00.xlsx"})Reject(()=>AlmacenArchivoExcel.NombreSeguro(name),"NOMBRE_ARCHIVO_INVALIDO");
   var newId=Guid.NewGuid().ToString("D");using(var empty=new MemoryStream())Reject(()=>a.Recibir(c,newId,"empty.xlsx","XLSX",empty,l),"ARCHIVO_VACIO");
   using(var input=new MemoryStream(new byte[1000001]))Reject(()=>a.Recibir(c,newId,"large.xlsx","XLSX",input,new LimitesLecturaExcel{MaxArchivoMB=1}),"LIMITE_ARCHIVO_EXCEDIDO");
   using(var input=new MemoryStream(File.ReadAllBytes(Path.Combine("excel-fixtures","valido.xls"))))Reject(()=>a.Recibir(c,newId,"wrong.xlsx","XLSX",input,l),"FORMATO_NO_COINCIDE");
   Check(!Directory.GetFiles(root,"*.part",SearchOption.AllDirectories).Any(),"known own partials removed");
   var part=Path.Combine(root,"2","3",newId,"contenido.bin.part");Directory.CreateDirectory(Path.GetDirectoryName(part));File.WriteAllBytes(part,new byte[]{42});
   using(var input=new MemoryStream(Book(new object[]{"A"})))Reject(()=>a.Recibir(c,newId,"x.xlsx","XLSX",input,l),"RESULTADO_INCIERTO");Check(File.ReadAllBytes(part).SequenceEqual(new byte[]{42}),"never reclaim prior partial");
  }finally{Directory.Delete(root,true);}
 }
 sealed class Templates:IPlantillaExcelRepository{
  public PlantillaExcel P=Template();public Action OnGet;public int Reads;
  public PlantillaExcel Obtener(uint id){Reads++;if(OnGet!=null)OnGet();return id==P.ID_PLANTILLAIMP?Copy(P):null;}
  public DestinoPlantillaExcel Destino(string table){return Schema().Destino;}
  public PaginaPlantillasExcel Listar(string s,int p,int n){throw new NotSupportedException();}
  public PlantillaExcel Guardar(PlantillaExcel p,uint r,string h){throw new NotSupportedException();}
 }
 sealed class Loads:ICargaExcelRepository,ISesionCargaExcel{
  public CargaExcelEjecucion E;public EsquemaCargaExcel S=Schema();public int Registers,Publish,Rejects,DuplicateCalls,Inspections;public bool Busy,UncertainReceipt,UncertainPublish,Closed;public Action DuringKeys;public IncidenciasClavesExcel Keys=new IncidenciasClavesExcel();
  public ISesionCargaExcel Abrir(ContextoCargaExcel c,string id){if(Busy)throw ReglasCargaExcel.Fallo("OPERACION_EN_CURSO");Busy=true;return this;}
  public CargaExcelEjecucion Consultar(ContextoCargaExcel c,string id){return E!=null&&E.Id==id&&E.UsuarioId==c.UsuarioId&&E.EmpresaId==c.EmpresaId&&E.ConexionRef==c.ConexionRef?Copy(E):null;}
  public CargaExcelEjecucion Obtener(){return Copy(E);}
  public CargaExcelEjecucion RegistrarRecibida(CargaExcelEjecucion e){Registers++;if(E!=null){if(E.Archivo.Hash!=e.Archivo.Hash)throw ReglasCargaExcel.Fallo("CONFLICTO_RECEPCION");return Copy(E);}E=Copy(e);if(UncertainReceipt)throw ReglasCargaExcel.Fallo("RESULTADO_INCIERTO");return Copy(E);}
  public EsquemaCargaExcel Inspeccionar(PlantillaExcel p,string mode){Inspections++;var result=Copy(S);result.Hash=MySqlPreparacionExcelValidator.HuellaEsquema(result);MySqlPreparacionExcelValidator.ValidarEsquema(p,result,mode);return result;}
  public IncidenciasClavesExcel ValidarClaves(PlantillaExcel p,EsquemaCargaExcel e,List<FilaNormalizadaExcel> f,string mode){DuplicateCalls++;Check(f.Count>0&&f.First().Fila==2,"full reader rows before keys");if(DuringKeys!=null)DuringKeys();return Copy(Keys);}
  public CargaExcelEjecucion Publicar(CargaExcelEjecucion e,string expected){Publish++;if(E.Estado=="PREPARADA"&&E.ConfiguracionHash!=e.ConfiguracionHash&&E.ConfiguracionHash!=expected)throw ReglasCargaExcel.Fallo("CONFLICTO_PREPARACION");E=Copy(e);if(UncertainPublish)throw ReglasCargaExcel.Fallo("RESULTADO_INCIERTO");return Copy(E);}
  public void Rechazar(string expected,string code){Rejects++;if(E.ConfiguracionHash!=expected)throw ReglasCargaExcel.Fallo("CONFLICTO_PREPARACION");E.Estado="RECIBIDA";E.ConfiguracionDoc=null;E.ConfiguracionHash=null;E.EsquemaHash=null;E.FilasPrevistas=null;E.CodigoIncidencia=code;}
  public void Dispose(){Busy=false;Closed=true;}
 }
 sealed class Flow:IDisposable{
  public string Root=Path.Combine(Path.GetTempPath(),"doc124-flow-"+Guid.NewGuid().ToString("N"));public ContextoCargaExcel C=Context();public Templates T=new Templates();public Loads L=new Loads();public LimitesLecturaExcel Limits=new LimitesLecturaExcel();public AlmacenArchivoExcel A;public ServicioRecepcionExcel Receive;public ServicioPreparacionExcel Prepare;public bool Authorized=true;public Action ContextHook;
  public Flow(){Directory.CreateDirectory(Root);A=new AlmacenArchivoExcel(Root);Func<ContextoCargaExcel> context=()=>{if(ContextHook!=null)ContextHook();if(!Authorized)throw ReglasCargaExcel.Fallo("ACCESO_DENEGADO");return Copy(C);};Receive=new ServicioRecepcionExcel(context,T,L,A,new ReferenciaRecepcionExcel(),Limits);Prepare=new ServicioPreparacionExcel(context,T,L,A,Limits);}
  public SolicitudRecepcionExcel Start(){var i=Receive.Iniciar(new SolicitudInicioRecepcionExcel{ConexionId=7,PlantillaId=5,RevisionEsperada=T.P.REVISION_CARGA,HuellaEsperada=ServicioPlantillasExcel.Huella(T.P)});return new SolicitudRecepcionExcel{ConexionId=7,PlantillaId=5,IdCarga=i.IdCarga,ReferenciaRecepcion=i.ReferenciaRecepcion};}
  public EstadoCargaExcel Upload(SolicitudRecepcionExcel s,byte[] b){using(var f=new MemoryStream(b))return Receive.Recibir(s,"prueba.xlsx",f,b.Length);}
  public SolicitudPrepararExcel Request(){return new SolicitudPrepararExcel{ConexionId=7,PlantillaId=5,IdCarga=L.E.Id,RevisionEsperada=T.P.REVISION_CARGA,HuellaEsperada=ServicioPlantillasExcel.Huella(T.P),Hoja="Datos",Modo=T.P.ELIMINA_INSERTA_RECORD==1?"REEMPLAZAR":(L.S.Destino.Existe?"AGREGAR":"CREAR")};}
  public void Dispose(){Directory.Delete(Root,true);}
 }
 static void Reception(){
  var body=Book(new object[]{"A"});
  using(var f=new Flow()){
   var s=f.Start();Check(f.L.E==null&&!Directory.GetFiles(f.Root,"*",SearchOption.AllDirectories).Any(),"start has no row/file");
   f.L.Busy=true;Reject(()=>f.Upload(s,body),"OPERACION_EN_CURSO");f.L.Busy=false;Check(f.L.E==null,"busy no row");
   f.L.UncertainReceipt=true;Reject(()=>f.Upload(s,body),"RESULTADO_INCIERTO");Check(Directory.GetFiles(f.Root,"*.bin",SearchOption.AllDirectories).Length==1&&f.L.E.Estado=="RECIBIDA","uncertain commit preserves complete file");
   var query=new SolicitudConsultaCargaExcel{ConexionId=7,IdCarga=s.IdCarga};var attempts=f.L.Registers;Check(f.Receive.Consultar(query).Estado=="RECIBIDA"&&f.L.Registers==attempts,"consult no repair");
   f.C.UsuarioId++;Reject(()=>f.Receive.Consultar(query),"RESULTADO_INCIERTO");f.C.UsuarioId--;
   f.L.UncertainReceipt=false;Check(f.Upload(s,body).IdCarga==s.IdCarga&&f.L.E.Archivo.Bytes>0,"same identity replay");
   Reject(()=>f.Upload(s,Book(new object[]{"B"})),"ARCHIVO_ALTERADO");Check(f.L.E.Estado=="RECIBIDA","different replay preserves prior row");
  }
  using(var f=new Flow()){
   var s=f.Start();var calls=0;f.ContextHook=()=>{if(++calls==2)f.Authorized=false;};Reject(()=>f.Upload(s,body),"ACCESO_DENEGADO");Check(f.L.E==null&&Directory.GetFiles(f.Root,"*.bin",SearchOption.AllDirectories).Length==1,"post-file authorization failure preserves file but no fake row");
   f.ContextHook=null;f.Authorized=true;Check(f.Upload(s,body).Estado=="RECIBIDA","explicit recovery after file publication");
  }
 }
 static void Preparation(){
  foreach(var mode in new[]{"CREAR","AGREGAR","REEMPLAZAR"})using(var f=new Flow()){
   if(mode=="CREAR"){f.L.S=Schema(false);f.T.P.Campos[0].TIPO_CAMPO="VARCHAR(250)";}
   if(mode=="REEMPLAZAR"){f.T.P.INSERTA_RECORD=0;f.T.P.ELIMINA_INSERTA_RECORD=1;}
   f.Upload(f.Start(),Book(new object[]{"A"},new object[]{"B"}));var request=f.Request();var original=ServicioPlantillasExcel.Huella(f.T.P);var result=f.Prepare.Preparar(request);
   Check(result.Codigo=="OK"&&result.Carga.Estado=="PREPARADA"&&result.Carga.FilasPrevistas==2&&result.LecturaCompleta&&result.DuplicadosEvaluados&&result.ValidacionCompleta,"prepared "+mode);
   Check(result.Muestra.Count==2&&result.Muestra.Last().Fila==3&&f.L.Publish==1&&f.L.Closed,"first/last preview and resource disposal");
   Check(original==ServicioPlantillasExcel.Huella(f.T.P)&&result.Carga.ConfiguracionHash!=original&&result.Carga.EsquemaHash==f.L.E.EsquemaHash,"independent effective hash");
   var firstHash=result.Carga.ConfiguracionHash;Check(f.Prepare.Preparar(request).Carga.ConfiguracionHash==firstHash,"equivalent explicit prepare");
   var changed=Copy(request);changed.Hoja="Otra";Reject(()=>f.Prepare.Preparar(changed),"CONFLICTO_PREPARACION");Check(f.L.E.ConfiguracionHash==firstHash&&f.L.Rejects==0,"new intent without CAS preserves prior prepared state");
   f.L.Keys=new IncidenciasClavesExcel{Total=30,Muestra=Enumerable.Range(2,25).Select(n=>new IncidenciaLecturaExcel{Codigo="CLAVE_DUPLICADA_ARCHIVO",Fila=n}).ToList()};request.PreparacionHashEsperada=firstHash;result=f.Prepare.Preparar(request);
   Check(result.Codigo=="CLAVES_INVALIDAS"&&result.Carga.Estado=="RECIBIDA"&&result.Incidencias.Count==25&&result.IncidenciasTruncadas&&result.TotalIncidencias==30&&result.Carga.ConfiguracionHash==null,"invalid explicit reprepare clears own prior CAS and capped totals");
  }
  using(var f=new Flow()){
   f.Upload(f.Start(),Book(new object[]{""}));var r=f.Prepare.Preparar(f.Request());Check(r.Codigo=="LECTURA_INVALIDA"&&!r.DuplicadosEvaluados&&f.L.DuplicateCalls==0&&f.L.Publish==0,"invalid reader never validates partial keys");
  }
  using(var f=new Flow()){
   f.Limits.MaxFilasDatos=1;f.Upload(f.Start(),Book(new object[]{"A"},new object[]{"B"}));var r=f.Prepare.Preparar(f.Request());Check(!r.LecturaCompleta&&r.FilasObservadas==2&&r.Muestra.Count==0&&!r.DuplicadosEvaluados,"no prefix prepared above row limit");
  }
  foreach(var drift in new[]{"schema","policy","template","authority"})using(var f=new Flow()){
   f.Upload(f.Start(),Book(new object[]{"A"}));f.L.DuringKeys=()=>{if(drift=="schema")f.L.S.Destino.Columnas[0].Longitud++;if(drift=="policy")f.L.S.Politica.Revision++;if(drift=="template")f.T.P.REVISION_CARGA++;if(drift=="authority")f.Authorized=false;};
   if(drift=="authority")Reject(()=>f.Prepare.Preparar(f.Request()),"ACCESO_DENEGADO");else Check(f.Prepare.Preparar(f.Request()).Codigo==(drift=="template"?"CONFLICTO_REVISION":"ESQUEMA_MODIFICADO"),"final revalidation "+drift);
   Check(f.L.Publish==0&&f.L.E.Estado=="RECIBIDA"&&f.L.Closed,"no publish after drift "+drift);
  }
  using(var f=new Flow()){
   f.Upload(f.Start(),Book(new object[]{"A"}));f.L.UncertainPublish=true;Reject(()=>f.Prepare.Preparar(f.Request()),"RESULTADO_INCIERTO");Check(f.L.E.Estado=="PREPARADA"&&f.L.Rejects==0,"unknown commit never degrades prepared winner");
  }
  using(var f=new Flow()){
   f.Upload(f.Start(),Book(new object[]{"A"}));f.L.E.Estado="CONFIRMADA";Reject(()=>f.Prepare.Preparar(f.Request()),"ESTADO_NO_PREPARABLE");Check(f.L.Publish==0&&f.L.Rejects==0,"future confirmed immutable");
  }
  using(var f=new Flow()){
   f.Upload(f.Start(),Book(new object[]{"A"}));var request=f.Request();f.C.UsuarioId++;f.L.Busy=true;Reject(()=>f.Prepare.Preparar(request),"REFERENCIA_NO_DISPONIBLE");Check(f.L.Publish==0,"foreign busy ID does not reveal lock activity");
  }
  using(var f=new Flow()){
   f.Upload(f.Start(),Book(new object[]{"A"}));var request=f.Request();Check(f.Prepare.Preparar(request).Codigo=="OK","prepare before invalidating equivalent intent");f.T.P.REVISION_CARGA++;var result=f.Prepare.Preparar(request);Check(result.Codigo=="CONFLICTO_REVISION"&&f.L.E.Estado=="RECIBIDA"&&f.L.E.ConfiguracionHash==null,"known invalid equivalent revalidation clears obsolete snapshot by captured CAS");
  }
 }
 static void Metadata(){
  var policies=new List<PoliticaDestinoCargaExcel>{new PoliticaDestinoCargaExcel{FuenteId="1",AmbitoRef="2",Modo="CONEXION",Revision=1}};
  Check(PoliticaDestinoCargaExcelAdapter.Seleccionar(policies,2).FuenteId=="1","own explicit exclusive policy");
  Reject(()=>PoliticaDestinoCargaExcelAdapter.Seleccionar(policies,3),"POLITICA_DESTINO_NO_DEFINIDA");
  policies.Add(Copy(policies[0]));Reject(()=>PoliticaDestinoCargaExcelAdapter.Seleccionar(policies,2),"POLITICA_DESTINO_NO_DEFINIDA");
  policies[1].AmbitoRef="3";Reject(()=>PoliticaDestinoCargaExcelAdapter.Seleccionar(policies,2),"POLITICA_DESTINO_NO_DEFINIDA");
  foreach(var policy in policies){policy.Modo="COLUMNA";policy.ColumnaEmpresa="EMPRESA";}Check(PoliticaDestinoCargaExcelAdapter.Seleccionar(policies,2).AmbitoRef=="2","compatible shared-column policies");
  policies[1].ColumnaEmpresa="OTRA";Reject(()=>PoliticaDestinoCargaExcelAdapter.Seleccionar(policies,2),"POLITICA_DESTINO_NO_DEFINIDA");
  var p=Template();var valid=Schema();MySqlPreparacionExcelValidator.ValidarEsquema(p,valid,"AGREGAR");checks++;
  foreach(var change in new Action<EsquemaCargaExcel>[] {e=>e.Destino.Motor="MyISAM",e=>e.Destino.Columnas[0].Nulable=true,e=>e.Politica.Modo="",e=>e.Destino.Columnas[0].TipoCompleto="varchar(2);DROP TABLE t"}){var e=Copy(valid);change(e);try{MySqlPreparacionExcelValidator.ValidarEsquema(p,e,"AGREGAR");throw new Exception("bad metadata allowed");}catch(ImportacionExcelException){checks++;}}
  foreach(var change in new Action<EsquemaCargaExcel>[] {e=>e.Indices[0].Unico=false,e=>e.Indices[0].Componentes[0].Prefijo=8,e=>e.Indices[0].Componentes.Add(new ComponenteIndiceCargaExcel{Columna="OTRA",Posicion=2}),e=>e.Indices[0].Componentes[0].Columna="id_tecnico"}){var e=Copy(valid);change(e);Reject(()=>MySqlPreparacionExcelValidator.ValidarEsquema(p,e,"AGREGAR"),"INDICE_UNICO_REQUERIDO");}
  valid.Politica.Modo="COLUMNA";valid.Politica.ColumnaEmpresa="EMPRESA";valid.Destino.Columnas.Add(Col("EMPRESA"));p.Campos.Add(new CampoPlantillaExcel{CAMPO_FUENTE="Empresa",CAMPO_DESTINO="EMPRESA",TIPO_CAMPO="VARCHAR"});valid.Indices[0].Componentes.Insert(0,new ComponenteIndiceCargaExcel{Columna="EMPRESA",Posicion=2});MySqlPreparacionExcelValidator.ValidarEsquema(p,valid,"AGREGAR");checks++;valid.Indices[0].Componentes.Reverse();MySqlPreparacionExcelValidator.ValidarEsquema(p,valid,"REEMPLAZAR");checks++;
  var before=MySqlPreparacionExcelValidator.HuellaEsquema(valid);valid.Destino.Columnas.Reverse();valid.Indices.Reverse();Check(before==MySqlPreparacionExcelValidator.HuellaEsquema(valid),"canonical metadata independent catalog order");valid.Indices[0].Componentes[0].Posicion=9;Check(before!=MySqlPreparacionExcelValidator.HuellaEsquema(valid),"physical index order retained in hash");
  Check(MySqlPreparacionExcelValidator.Definicion(new ColumnaDestinoExcel{TipoCompleto="decimal(65,30) unsigned"})=="decimal(65,30) unsigned NOT NULL","decimal65 server conversion definition");
  var c=Context();p=Template();var e2=Schema();var s=new SolicitudPrepararExcel{Modo="AGREGAR",Hoja="Datos"};var limits=new LimitesLecturaExcel();var doc=ServicioPreparacionExcel.Documento(c,p,e2,s,limits);Check(doc.All(x=>x<128)&&doc.Contains("null"),"ASCII and NULL effective document");
  var hash=ReglasCargaExcel.Hash(Encoding.UTF8.GetBytes(doc));s.Hoja="Otra";Check(hash!=ReglasCargaExcel.Hash(Encoding.UTF8.GetBytes(ServicioPreparacionExcel.Documento(c,p,e2,s,limits))),"sheet affects effective hash");s.Hoja="Datos";e2.Politica.Revision++;Check(hash!=ReglasCargaExcel.Hash(Encoding.UTF8.GetBytes(ServicioPreparacionExcel.Documento(c,p,e2,s,limits))),"policy revision affects effective hash");
  var vector=Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText(Path.Combine("excel-fixtures","doc124-canonico-v1.json")));Check(ReglasCargaExcel.Canonico(vector["documento"])==(string)vector["canonico"]&&ReglasCargaExcel.HashDocumento(vector["documento"])==(string)vector["sha256"],"independent canonical vector");
 }
 public static void Run(){Tokens();Multipart();Files();Reception();Preparation();Metadata();Ado();Repository();Console.WriteLine("DOC124_NATIVE_PASS checks="+checks+"; actual file/reader and synthetic state/ADO; no real MySQL/HTTP");}
 static CargaExcelEjecucion Execution(){var c=Context();return new CargaExcelEjecucion{Id=Guid.NewGuid().ToString("D"),EmpresaId=c.EmpresaId,UsuarioId=c.UsuarioId,ConexionRef=c.ConexionRef,PlantillaId=5,Tabla=Template().NOMBRE_PLANTILLA,Estado="RECIBIDA",Archivo=new ArchivoRecibidoExcel{Nombre="x.xlsx",Referencia="2/3/synthetic/contenido.bin",Bytes=8,Hash=new string('c',64),Formato="XLSX"}};}
 static void Repository(){
  foreach(var commit in new[]{null,"before","after"}){
   var db=new DbState{CommitFailure=commit};var c=Context();var e=Execution();var repo=new MySqlCargaExcelRepository(()=>new ProbeConnection(db),t=>Schema().Politica);
   using(var writer=repo.Abrir(c,e.Id)){
    Check(writer.Obtener()==null&&!db.InTransaction,"no row/long transaction before receipt");
    Reject(()=>{using(var second=repo.Abrir(c,e.Id)){}},"OPERACION_EN_CURSO");
    if(commit==null)Check(writer.RegistrarRecibida(e).Estado=="RECIBIDA","real repository receipt command");else Reject(()=>writer.RegistrarRecibida(e),"RESULTADO_INCIERTO");
   }
   Check(db.Row!=null==(commit!="before")&&db.LockOwner==null&&!db.InTransaction,"commit uncertainty and lock cleanup "+commit);
   db.CommitFailure=null;using(var writer=repo.Abrir(c,e.Id)){
    if(db.Row==null)writer.RegistrarRecibida(e);else Check(writer.RegistrarRecibida(e).Archivo.Hash==e.Archivo.Hash,"same receipt remains");
    var prepared=Copy(e);prepared.Estado="PREPARADA";prepared.Modo="AGREGAR";prepared.RevisionPlantilla=1;prepared.ConfiguracionDoc="{}";prepared.ConfiguracionHash=new string('d',64);prepared.EsquemaHash=new string('e',64);prepared.FilasPrevistas=1;
    Check(writer.Publicar(prepared,null).Estado=="PREPARADA","actual repository transition");
    Reject(()=>writer.Rechazar(new string('0',64),"INVALIDA"),"CONFLICTO_PREPARACION");Check(db.Row.Estado=="PREPARADA","CAS protects prepared winner");
    writer.Rechazar(prepared.ConfiguracionHash,"INVALIDA");Check(db.Row.Estado=="RECIBIDA"&&db.Row.ConfiguracionHash==null&&db.Row.CodigoIncidencia=="INVALIDA","own rejection clears prepared fields");
   }
   var foreign=Copy(c);foreign.ModuloId++;Check(repo.Consultar(foreign,e.Id)==null,"module-bound own read");foreign=Copy(c);foreign.UsuarioId++;Check(repo.Consultar(foreign,e.Id)==null,"owner-bound own read");
   Check(db.Sql.Where(x=>x.Contains("FROM imp01_carga_ejecucion")).All(x=>x.Contains("ambito_ref=@empresa")&&x.Contains("usuario_ref=@usuario")&&x.Contains("conexion_ref=@conexion")),"all execution reads constrain owner/company/module");
  }
  foreach(var failure in new[]{"drop","restore"}){
   var db=new DbState{Failure=failure};var discarded=0;var repo=new MySqlCargaExcelRepository(()=>new ProbeConnection(db),t=>Schema().Politica,cn=>discarded++);
   using(var writer=repo.Abrir(Context(),Guid.NewGuid().ToString("D")))writer.ValidarClaves(Template(),Schema(),new List<FilaNormalizadaExcel>{new FilaNormalizadaExcel{Fila=2,Valores=new List<ValorNormalizadoExcel>{new ValorNormalizadoExcel{CampoDestino="CLAVE",Tipo="TEXTO",Texto="x"}}}},"AGREGAR");
   Check(discarded==1&&db.LockOwner==null,"dirty session triggers targeted discard after release "+failure);
  }
 }
 //Sigue el protocolo real de comandos del validador. Reutiliza parámetros ADO del harness DOC122.
 static void Ado(){
  foreach(var mode in new[]{"CREAR","AGREGAR","REEMPLAZAR"}){
   var db=new DbState{Exists=mode!="CREAR"};using(var cn=new ProbeConnection(db)){cn.Open();var validator=new MySqlPreparacionExcelValidator(cn,Context(),t=>Schema().Politica);var p=Template();p.Modo=mode;if(mode=="CREAR")p.Campos[0].TIPO_CAMPO="VARCHAR(250)";
    var e=validator.Inspeccionar(p,mode);Check(e.Destino.Existe==db.Exists&&ReglasCargaExcel.HashValido(e.Hash),"actual metadata/projection protocol "+mode);
    var rows=new List<FilaNormalizadaExcel>{new FilaNormalizadaExcel{Fila=2,Valores=new List<ValorNormalizadoExcel>{new ValorNormalizadoExcel{CampoDestino="CLAVE",Tipo="TEXTO",Texto="Á"}}},new FilaNormalizadaExcel{Fila=3,Valores=new List<ValorNormalizadoExcel>{new ValorNormalizadoExcel{CampoDestino="CLAVE",Tipo="TEXTO",Texto="a "}}}};
    var result=validator.Validar(p,e,rows,mode);Check(result.Total==2,"ADO duplicate response represented, "+mode);Check(validator.Cerrar()&&db.Mode=="ANSI_QUOTES"&&db.Tables==0,"temp cleanup and original sql_mode "+mode);
    Check(db.Sql.Count(x=>x.StartsWith("CREATE TEMPORARY"))==2&&db.Sql.Any(x=>x.Contains("GROUP BY")&&x.Contains("HAVING COUNT(*)>1")),"two temporaries grouping protocol");
    Check(db.Sql.Any(x=>x.Contains("INNER JOIN `imp_02_"))==(mode=="AGREGAR"),"existing keys queried only add");
    Check(db.Sql.All(x=>!x.StartsWith("CREATE TABLE")&&!x.StartsWith("DELETE")&&!x.StartsWith("ALTER")&&!x.StartsWith("INSERT INTO `imp_02_")),"no permanent writes");
   }
  }
  foreach(var fail in new[]{"insert","warning","drop","restore"}){
   var db=new DbState{Failure=fail};using(var cn=new ProbeConnection(db)){cn.Open();var v=new MySqlPreparacionExcelValidator(cn,Context(),t=>Schema().Politica);
    try{v.Validar(Template(),Schema(),new List<FilaNormalizadaExcel>{new FilaNormalizadaExcel{Fila=2,Valores=new List<ValorNormalizadoExcel>{new ValorNormalizadoExcel{CampoDestino="CLAVE",Tipo="TEXTO",Texto="x"}}}},"AGREGAR");Check(fail=="drop"||fail=="restore","failure injected");}catch(DbException){Check(fail=="insert","ADO insert failure");}catch(ImportacionExcelException ex){Check(fail=="warning"&&ex.Codigo=="CONVERSION_MYSQL_INVALIDA","warnings rejected");}
    Check(v.Cerrar()==(fail!="drop"&&fail!="restore"),"cleanup failure signaled "+fail);Check(db.Sql.Any(x=>x.StartsWith("DROP TEMPORARY")),"DROP attempted after "+fail);
   }
  }
  var method=typeof(MySql.Data.MySqlClient.MySqlConnection).GetMethod("ClearPool",new[]{typeof(MySql.Data.MySqlClient.MySqlConnection)});Check(method!=null&&method.IsStatic,"actual existing connector has targeted pool discard");
  using(var cn=new ProbeConnection(new DbState{Exists=false})){cn.Open();var validator=new MySqlPreparacionExcelValidator(cn,Context(),t=>Schema().Politica);var p=Template();p.Modo="CREAR";p.Campos[0].TIPO_CAMPO="VARCHAR(256)";Reject(()=>validator.Inspeccionar(p,"CREAR"),"INDICE_COMPLETO_NO_SOPORTADO");}
 }
 sealed class ProbeFailure:DbException{public ProbeFailure():base("synthetic failure"){} }
 sealed class DbState{public string Mode="ANSI_QUOTES",Failure,CommitFailure;public int Tables;public bool InTransaction,Exists=true;public CargaExcelEjecucion Row;public ProbeConnection LockOwner;public List<string> Sql=new List<string>();}
 sealed class ProbeConnection:DbConnection{
  public readonly DbState S;ConnectionState state;public ProbeConnection(DbState s){S=s;}public override string ConnectionString{get;set;}public override string Database{get{return "synthetic";}}public override string DataSource{get{return "synthetic";}}public override string ServerVersion{get{return "5.1.synthetic";}}public override ConnectionState State{get{return state;}}public override void Open(){state=ConnectionState.Open;}public override void Close(){state=ConnectionState.Closed;}public override void ChangeDatabase(string n){throw new NotSupportedException();}protected override DbTransaction BeginDbTransaction(IsolationLevel l){return new ProbeTransaction(this,l);}protected override DbCommand CreateDbCommand(){return new ProbeCommand(this);}
 }
 sealed class ProbeTransaction:DbTransaction{
  readonly ProbeConnection c;readonly CargaExcelEjecucion backup;readonly IsolationLevel level;bool done;public ProbeTransaction(ProbeConnection c,IsolationLevel l){this.c=c;level=l;backup=Copy(c.S.Row);Check(!c.S.InTransaction,"short nonnested transaction");c.S.InTransaction=true;}public override IsolationLevel IsolationLevel{get{return level;}}protected override DbConnection DbConnection{get{return c;}}public override void Commit(){if(c.S.CommitFailure=="before")throw new ProbeFailure();done=true;c.S.InTransaction=false;if(c.S.CommitFailure=="after")throw new ProbeFailure();}public override void Rollback(){c.S.Row=Copy(backup);done=true;c.S.InTransaction=false;}protected override void Dispose(bool disposing){if(!done)Rollback();base.Dispose(disposing);}
 }
 sealed class ProbeCommand:DbCommand{
  readonly ProbeConnection cn;readonly ImportacionExcelBehaviorTests.Params parameters=new ImportacionExcelBehaviorTests.Params();public ProbeCommand(ProbeConnection c){cn=c;DbConnection=c;}public override string CommandText{get;set;}public override int CommandTimeout{get;set;}public override CommandType CommandType{get;set;}public override bool DesignTimeVisible{get;set;}public override UpdateRowSource UpdatedRowSource{get;set;}protected override DbConnection DbConnection{get;set;}protected override DbTransaction DbTransaction{get;set;}protected override DbParameterCollection DbParameterCollection{get{return parameters;}}public override void Cancel(){}public override void Prepare(){}protected override DbParameter CreateDbParameter(){return new ImportacionExcelBehaviorTests.Parameter();}
  void Track(){cn.S.Sql.Add(CommandText);Check(!CommandText.Contains("Á")&&!CommandText.Contains("'a '"),"ADO parameterized content");}
  object V(string name){return parameters.Cast<DbParameter>().Single(p=>p.ParameterName==name).Value;}
  public override object ExecuteScalar(){Track();if(CommandText.StartsWith("SELECT GET_LOCK")){Check(((string)V("@nombre")).Length==64,"bounded physical lock name");if(cn.S.LockOwner!=null&&cn.S.LockOwner!=cn)return 0;cn.S.LockOwner=cn;Check(!cn.S.InTransaction,"lock outside transaction");return 1;}if(CommandText.StartsWith("SELECT RELEASE_LOCK")){if(cn.S.LockOwner!=cn)return 0;cn.S.LockOwner=null;return 1;}if(CommandText.StartsWith("SELECT MAXLEN"))return 3;if(CommandText=="SELECT @@session.sql_mode")return cn.S.Mode;if(CommandText=="SELECT @@max_allowed_packet")return 65536;if(CommandText=="SHOW COUNT(*) WARNINGS")return cn.S.Failure=="warning"?1:0;throw new Exception("unsupported probe scalar "+CommandText);}
  public override int ExecuteNonQuery(){Track();
   if(CommandText.StartsWith("INSERT INTO imp01_carga_ejecucion")){Check(cn.S.InTransaction,"receipt transactional");cn.S.Row=new CargaExcelEjecucion{Id=(string)V("@id"),PlantillaId=Convert.ToUInt32(V("@plantilla")),EmpresaId=Convert.ToUInt32(V("@empresa")),UsuarioId=Convert.ToUInt32(V("@usuario")),ConexionRef=(string)V("@conexion"),Tabla=(string)V("@tabla"),Archivo=new ArchivoRecibidoExcel{Nombre=(string)V("@nombre"),Referencia=(string)V("@ref"),Bytes=Convert.ToInt64(V("@bytes")),Hash=BitConverter.ToString((byte[])V("@hash")).Replace("-","").ToLowerInvariant()},Estado="RECIBIDA"};return 1;}
   if(CommandText.StartsWith("UPDATE imp01_carga_ejecucion")){Check(cn.S.InTransaction,"state transition transactional");var row=cn.S.Row;if(row==null)return 0;if(CommandText.Contains("estado='RECIBIDA'")){row.Estado="RECIBIDA";row.Modo=null;row.RevisionPlantilla=null;row.ConfiguracionDoc=null;row.ConfiguracionHash=null;row.EsquemaHash=null;row.FilasPrevistas=null;row.CodigoIncidencia=(string)V("@codigo");}else{row.Estado="PREPARADA";row.Modo=(string)V("@modo");row.RevisionPlantilla=Convert.ToUInt32(V("@revision"));row.ConfiguracionDoc=(string)V("@doc");row.ConfiguracionHash=BitConverter.ToString((byte[])V("@config")).Replace("-","").ToLowerInvariant();row.EsquemaHash=BitConverter.ToString((byte[])V("@esquema")).Replace("-","").ToLowerInvariant();row.FilasPrevistas=Convert.ToInt32(V("@filas"));row.CodigoIncidencia=null;}return 1;}
   if(CommandText.StartsWith("SET SESSION")){var mode=Convert.ToString(parameters.Cast<DbParameter>().Single().Value);if(mode=="ANSI_QUOTES"&&cn.S.Failure=="restore")throw new ProbeFailure();cn.S.Mode=mode;return 0;}if(CommandText.StartsWith("CREATE TEMPORARY")){cn.S.Tables++;return 0;}if(CommandText.StartsWith("DROP TEMPORARY")){if(cn.S.Failure=="drop")throw new ProbeFailure();cn.S.Tables--;return 0;}if(CommandText.StartsWith("INSERT INTO")){if(cn.S.Failure=="insert")throw new ProbeFailure();Check(parameters.Count<=128,"bounded parameters");return 2;}throw new Exception("unsupported probe mutation "+CommandText);}
  protected override DbDataReader ExecuteDbDataReader(CommandBehavior b){Track();
   var t=new DataTable();
   if(CommandText=="SELECT @@hostname,@@port,DATABASE()"){foreach(var name in new[]{"host","port","db"})t.Columns.Add(name,typeof(object));t.Rows.Add("synthetic",3306,"fixture");return t.CreateDataReader();}
   if(CommandText=="SELECT @@character_set_database,@@collation_database"){t.Columns.Add("charset");t.Columns.Add("collation");t.Rows.Add("utf8","utf8_general_ci");return t.CreateDataReader();}
   if(CommandText.Contains("FROM information_schema.TABLES")){t.Columns.Add("ENGINE");t.Columns.Add("TABLE_TYPE");if(cn.S.Exists)t.Rows.Add("InnoDB","BASE TABLE");return t.CreateDataReader();}
   if(CommandText.Contains("FROM information_schema.COLUMNS")){foreach(var n in new[]{"COLUMN_NAME","DATA_TYPE","COLUMN_TYPE","CHARACTER_MAXIMUM_LENGTH","IS_NULLABLE","COLUMN_DEFAULT","CHARACTER_SET_NAME","COLLATION_NAME","EXTRA"})t.Columns.Add(n,typeof(object));t.Rows.Add("CLAVE","varchar","varchar(250)",250,"NO",DBNull.Value,"latin1","latin1_swedish_ci","");return t.CreateDataReader();}
   if(CommandText.Contains("FROM information_schema.STATISTICS")){foreach(var n in new[]{"INDEX_NAME","NON_UNIQUE","SEQ_IN_INDEX","COLUMN_NAME","SUB_PART"})t.Columns.Add(n,typeof(object));t.Rows.Add("uq_clave",0,1,"CLAVE",DBNull.Value);return t.CreateDataReader();}
   if(CommandText.Contains("FROM imp01_carga_ejecucion")){
    var names=CommandText.Substring(7,CommandText.IndexOf(" FROM",StringComparison.Ordinal)-7).Split(',');foreach(var name in names)t.Columns.Add(name,typeof(object));var e=cn.S.Row;
    if(e!=null&&e.Id==(string)V("@id")&&e.EmpresaId.ToString()==(string)V("@empresa")&&e.UsuarioId.ToString()==(string)V("@usuario")&&e.ConexionRef==(string)V("@conexion"))t.Rows.Add(new object[]{e.Id,e.PlantillaId,e.EmpresaId,e.UsuarioId,e.ConexionRef,e.Tabla,e.Archivo.Nombre,e.Archivo.Referencia,e.Archivo.Bytes,SqlCargaExcel.Binario(e.Archivo.Hash),e.Modo,(object)e.RevisionPlantilla??DBNull.Value,e.ConfiguracionDoc,SqlCargaExcel.Binario(e.ConfiguracionHash),SqlCargaExcel.Binario(e.EsquemaHash),e.Estado,(object)e.FilasPrevistas??DBNull.Value,e.CodigoIncidencia,e.FilasConfirmadas,(object)e.ConfirmadoUtc??DBNull.Value}.Select(x=>x??DBNull.Value).ToArray());return t.CreateDataReader();
   }
   t.Columns.Add("fila",typeof(int));if(CommandText.Contains("INNER JOIN dx_g_")){t.Rows.Add(2);t.Rows.Add(3);}else if(!CommandText.Contains("INNER JOIN `imp_02_"))throw new Exception("unsupported probe reader "+CommandText);return t.CreateDataReader();}
 }
}
