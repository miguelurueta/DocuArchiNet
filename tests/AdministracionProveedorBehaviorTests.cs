using System;
using System.Collections.Generic;
public static class AdministracionProveedorBehaviorTests {
 static void Check(bool b,string m){if(!b)throw new Exception(m);}
 static void Reject(Action a,string code){try{a();}catch(PerfilImportacionException e){Check(e.Codigo==code,"unexpected error "+e.Codigo);return;}throw new Exception("expected "+code);}
 sealed class Repo:IAdministracionProveedorRepository{
  public int Reads,Writes;public byte[] Hash;public DocumentoProveedorAdmin D=new DocumentoProveedorAdmin{ProveedorId="1",VersionId="2",Revision=1,RevisionProveedor=1,Estado="BORRADOR",Contrato=new ContratoProveedorIndices{Tabla="guardada"}};
  public List<DocumentoProveedorAdmin> Listar(ContextoPerfilImportacion c,SolicitudProveedorAdmin s){Reads++;return new List<DocumentoProveedorAdmin>{D};}
  public DocumentoProveedorAdmin Obtener(ContextoPerfilImportacion c,string p,string v){Reads++;return D;}
  public DocumentoProveedorAdmin Mutar(ContextoPerfilImportacion c,string a,SolicitudProveedorAdmin s,byte[] h){Writes++;Hash=h;return D;}
 }
 sealed class Query:IProveedorDatosIndices{
  public int Calls;public ResultadoProveedorIndices Consultar(ContextoPerfilImportacion c,ProveedorIndicesVersion v,string key){Calls++;Check(v.Tabla=="guardada" && key=="00012","test uses saved source and preserves key");return new ResultadoProveedorIndices{Codigo="ENCONTRADO"};}
 }
 public static void Run(){
  var c=new ContextoPerfilImportacion{UsuarioRef="1",AmbitoRef="2",PuedeAdministrar=true,PuedeAdministrarProveedores=false};var repo=new Repo();var query=new Query();int validations=0;
  var svc=new ServicioAdministracionProveedor(repo,(ctx,contract,complete)=>{validations++;if(complete)Check(contract.Tabla=="guardada","saved source required");},query);
  var req=new SolicitudProveedorAdmin{ProveedorId="1",VersionId="2",Revision=1,RevisionProveedor=1,ClaveEjemplo="00012",Contrato=new ContratoProveedorIndices{Tabla="inyectada"}};
  foreach(var action in new[]{"LISTAR","OBTENER","CREAR","GUARDAR","PUBLICAR","VERSION","DISPONIBILIDAD","VALIDAR","ENSAYAR"}){
   Reject(()=>svc.Ejecutar(c,action,req),"ACCESO_DENEGADO");
   Check(repo.Reads==0 && repo.Writes==0 && query.Calls==0 && validations==0,"permission checked before all provider operations");
  }
  c.PuedeAdministrarProveedores=true;
  foreach(var headerStale in new[]{false,true}){
   req.Revision=headerStale?1u:2u;req.RevisionProveedor=headerStale?2u:1u;
   foreach(var action in new[]{"GUARDAR","PUBLICAR","VERSION","DISPONIBILIDAD","VALIDAR","ENSAYAR"}){
    Reject(()=>svc.Ejecutar(c,action,req),"CONFLICTO_REVISION");
    Check(repo.Writes==0 && query.Calls==0 && validations==0,"both revisions checked before validation, query or mutation");
   }
  }
  req.Revision=1;req.RevisionProveedor=1;repo.Reads=0;c.PuedeAdministrarProveedores=false;
  Reject(()=>svc.Ejecutar(c,"LISTAR",req),"ACCESO_DENEGADO");Check(repo.Reads==0,"deny before repository");c.PuedeAdministrarProveedores=true;
  svc.Ejecutar(c,"VALIDAR",req);svc.Ejecutar(c,"ENSAYAR",req);Check(repo.Writes==0 && query.Calls==1,"validation/rehearsal are read only");
  req.Revision=2;Reject(()=>svc.Ejecutar(c,"PUBLICAR",req),"CONFLICTO_REVISION");Check(repo.Writes==0,"stale publication cannot write");req.Revision=1;
  svc.Ejecutar(c,"PUBLICAR",req);Check(repo.Writes==1 && repo.Hash.Length==32 && query.Calls==1,"publish hashes contract without example lookup");
  repo.D.Estado="PUBLICADA";Reject(()=>svc.Ejecutar(c,"GUARDAR",req),"CONFLICTO_REVISION");Reject(()=>svc.Ejecutar(c,"PUBLICAR",req),"CONFLICTO_REVISION");Check(repo.Writes==1,"publication immutable");
  svc.Ejecutar(c,"VERSION",req);Check(repo.Writes==2,"new version explicit");repo.D.Estado="BORRADOR";Reject(()=>svc.Ejecutar(c,"VERSION",req),"CONFLICTO_REVISION");
  c.PuedeAdministrarProveedores=false;Reject(()=>svc.Ejecutar(c,"DISPONIBILIDAD",req),"ACCESO_DENEGADO");Check(repo.Writes==2,"revocation blocks availability");
 }
}
