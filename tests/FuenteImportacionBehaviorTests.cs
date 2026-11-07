using System;
using System.Collections.Generic;
public static class FuenteImportacionBehaviorTests {
 static void Check(bool b,string m){if(!b)throw new Exception(m);}
 static void Denied(Action a,string code){try{a();}catch(PerfilImportacionException e){Check(e.Codigo==code,"unexpected source rejection");return;}throw new Exception("expected source rejection "+code);}
 sealed class Repo:IFuenteImportacionRepository {
  public int Reads,Writes;public bool ConflictOnWrite;public DocumentoFuenteImportacion Saved=new DocumentoFuenteImportacion{FuenteId="1",Revision=2,ConexionRef="DA",Tabla="guardada",ModoAislamiento="CONEXION",Columnas=new List<string>{"numero"}};
  public List<DocumentoFuenteImportacion> Listar(ContextoPerfilImportacion c,int p,string t){Reads++;return new List<DocumentoFuenteImportacion>{Saved};}
  public DocumentoFuenteImportacion Obtener(ContextoPerfilImportacion c,string id){Reads++;return Saved;}
  public DocumentoFuenteImportacion Guardar(ContextoPerfilImportacion c,string a,CambioFuenteImportacion ch){Writes++;if(ConflictOnWrite)throw new PerfilImportacionException("CONFLICTO_REVISION","Conflicto simulado bajo bloqueo.");return Saved;}
  public List<HistorialFuenteImportacion> Historial(ContextoPerfilImportacion c,string id,int p){Reads++;return new List<HistorialFuenteImportacion>();}
 }
 public static void Run(){
  var c=new ContextoPerfilImportacion{UsuarioRef="1",AmbitoRef="2",PuedeAdministrar=true,PuedeAdministrarProveedores=true};
  var repo=new MySqlFuenteImportacionRepository("invalid connection must never be used");
  Denied(()=>repo.Listar(c,1,null),"ACCESO_DENEGADO");
  Denied(()=>repo.Obtener(c,"1"),"ACCESO_DENEGADO");
  Denied(()=>repo.Historial(c,"1",1),"ACCESO_DENEGADO");
  Denied(()=>repo.Guardar(c,"ALTA",null),"ACCESO_DENEGADO");
  c.PuedeAdministrarFuentes=true;
  Denied(()=>repo.Listar(c,0,null),"SOLICITUD_INVALIDA");
  Denied(()=>repo.Obtener(c,"0"),"SOLICITUD_INVALIDA");
  var d=new DocumentoFuenteImportacion{ConexionRef="DA",Tabla="ordenes",ModoAislamiento="CONEXION",Columnas=new List<string>{"numero","nombre"}};
  var change=new CambioFuenteImportacion{Documento=d,Motivo="Autorizar fuente exclusiva"};
  ReglasFuenteImportacion.Validar("ALTA",change);
  d.ColumnaEmpresa="empresa";Denied(()=>repo.Guardar(c,"ALTA",change),"CONFIGURACION_INVALIDA");
  d.ModoAislamiento="COLUMNA";Denied(()=>repo.Guardar(c,"ALTA",change),"CONFIGURACION_INVALIDA");
  d.Columnas.Add("empresa");ReglasFuenteImportacion.Validar("ALTA",change);
  d.Columnas.Add("EMPRESA");Denied(()=>repo.Guardar(c,"ALTA",change),"CONFIGURACION_INVALIDA");d.Columnas.RemoveAt(3);
  d.FuenteId="1";Denied(()=>repo.Guardar(c,"ALTA",change),"SOLICITUD_INVALIDA");
  d.Revision=1;ReglasFuenteImportacion.Validar("EDITAR",change);
  change.Motivo=" ";Denied(()=>repo.Guardar(c,"REVOCAR",change),"SOLICITUD_INVALIDA");change.Motivo="Revocacion administrativa";
  d.Revision=uint.MaxValue;Denied(()=>repo.Guardar(c,"REVOCAR",change),"SOLICITUD_INVALIDA");d.Revision=1;
  var snapshot=PerfilImportacionJson.Escribir(d);
  Check(PerfilImportacionJson.Leer<DocumentoFuenteImportacion>(snapshot).Columnas.Count==3,"history snapshot round trip");
  var fake=new Repo();int validations=0;
  var svc=new ServicioFuenteImportacion(fake,(ctx,doc)=>{validations++;Check(doc.Tabla=="guardada","activate validates saved identity not client payload");});
  var request=new SolicitudFuenteImportacion{Cambio=new CambioFuenteImportacion{Motivo="Habilitar fuente",Documento=new DocumentoFuenteImportacion{FuenteId="1",Revision=2,Tabla="manipulada"}}};
  svc.Ejecutar(c,"ACTIVAR",request);Check(validations==1&&fake.Writes==1,"activation validates saved metadata");
  request.Cambio.Documento.Revision=1;Denied(()=>svc.Ejecutar(c,"ACTIVAR",request),"CONFLICTO_REVISION");Check(fake.Writes==1&&validations==1,"stale activation before metadata");
  request.Cambio.Documento.Revision=2;svc.Ejecutar(c,"REVOCAR",request);Check(fake.Writes==2&&validations==1,"revocation possible if origin unavailable");
  c.PuedeAdministrarFuentes=false;Denied(()=>svc.Ejecutar(c,"LISTAR",new SolicitudFuenteImportacion()),"ACCESO_DENEGADO");
  var metadata=new MetadatosFuenteImportacion(alias=>{throw new Exception("unauthorized metadata connected");});Denied(()=>metadata.Consultar(c,"TABLAS","DA",null,1),"ACCESO_DENEGADO");
  c.PuedeAdministrarFuentes=true;
  // No atribuir al servicio garantías transaccionales del repositorio: aquí se
  // comprueba el orden de autorización/revisión/metadatos y la propagación CAS.
  var guardedRepo=new Repo();int metadataCalls=0;bool metadataUnavailable=false;
  var guardedService=new ServicioFuenteImportacion(guardedRepo,(ctx,doc)=>{
   metadataCalls++;if(metadataUnavailable)throw new PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE","Origen simulado no disponible.");
  });
  var edit=new SolicitudFuenteImportacion{Cambio=new CambioFuenteImportacion{Motivo="Cambio de columnas de prueba",Documento=
   new DocumentoFuenteImportacion{FuenteId="1",Revision=2,ConexionRef="DA",Tabla="guardada",ModoAislamiento="CONEXION",Columnas=new List<string>{"otra"}}}};
  var original=PerfilImportacionJson.Escribir(guardedRepo.Saved);
  c.PuedeAdministrarFuentes=false;
  foreach(var action in new[]{"LISTAR","OBTENER","HISTORIAL","ALTA","EDITAR","ACTIVAR","REVOCAR"})
   Denied(()=>guardedService.Ejecutar(c,action,edit),"ACCESO_DENEGADO");
  Check(guardedRepo.Reads==0&&guardedRepo.Writes==0&&metadataCalls==0,"all source actions deny before repository and origin access");
  c.PuedeAdministrarFuentes=true;edit.Cambio.Documento.Revision=1;
  foreach(var action in new[]{"EDITAR","ACTIVAR","REVOCAR"}){
   Denied(()=>guardedService.Ejecutar(c,action,edit),"CONFLICTO_REVISION");
   Check(guardedRepo.Writes==0&&metadataCalls==0&&PerfilImportacionJson.Escribir(guardedRepo.Saved)==original,"stale source never validates origin or writes");
  }
  edit.Cambio.Documento.Revision=2;metadataUnavailable=true;
  foreach(var action in new[]{"EDITAR","ACTIVAR"}){
   Denied(()=>guardedService.Ejecutar(c,action,edit),"CAPACIDAD_NO_DISPONIBLE");
   Check(guardedRepo.Writes==0&&PerfilImportacionJson.Escribir(guardedRepo.Saved)==original,"metadata failure prevents partial source changes");
  }
  metadataUnavailable=false;guardedRepo.ConflictOnWrite=true;
  int attempts=guardedRepo.Writes,validationsBefore=metadataCalls;
  Denied(()=>guardedService.Ejecutar(c,"EDITAR",edit),"CONFLICTO_REVISION");
  Check(guardedRepo.Writes==attempts+1&&metadataCalls==validationsBefore+1,"repository race conflict after metadata propagates without retry");
  Check(PerfilImportacionJson.Escribir(guardedRepo.Saved)==original&&edit.Cambio.Documento.Columnas[0]=="otra","rejected edit preserves saved and proposed snapshots");
  guardedRepo.ConflictOnWrite=false;metadataUnavailable=true;validationsBefore=metadataCalls;
  guardedService.Ejecutar(c,"REVOCAR",edit);
  Check(metadataCalls==validationsBefore&&guardedRepo.Writes==attempts+2,"source can be revoked without contacting unavailable origin");
  // Candidatos administrativos no permiten alias libres, expresiones ni paginación inválida.
  Denied(()=>metadata.Consultar(c,"CONEXIONES",null,null,0),"SOLICITUD_INVALIDA");
  Denied(()=>metadata.Consultar(c,"TABLAS","DATABASE()",null,1),"CONFIGURACION_INVALIDA");
  Denied(()=>metadata.Consultar(c,"SELECT","DA",null,1),"CONFIGURACION_INVALIDA");
  Denied(()=>metadata.Consultar(c,"COLUMNAS","DA","imp_02_x`;DROP",1),"CONFIGURACION_INVALIDA");
  Denied(()=>metadata.Consultar(c,"COLUMNAS","DA","impX02_creditos",1),"ACCESO_DENEGADO");
  c.AmbitoRef="0";Denied(()=>repo.Listar(c,1,null),"ACCESO_DENEGADO");
 }
}
