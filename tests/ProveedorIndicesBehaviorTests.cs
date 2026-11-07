using System;
using System.Data;
using System.Collections.Generic;

public static class ProveedorIndicesBehaviorTests {
    static void Check(bool ok,string message){if(!ok)throw new Exception(message);}
    static void Reject(Action action,string code){
        try{action();}catch(PerfilImportacionException error){Check(error.Codigo==code,"repository rejection code");return;}
        throw new Exception("repository must reject before connection construction");
    }
    public static void Run(){
        // Una cadena deliberadamente inválida hace fallar la prueba si se intenta
        // construir la conexión antes de rechazar el contexto o filtro. Sin red.
        var consumerRepo=new MySqlProveedorIndicesRepository("DOC110_INVALID_CONNECTION_SENTINEL");
        var adminRepo=new MySqlAdministracionProveedorRepository("DOC110_INVALID_CONNECTION_SENTINEL");
        foreach(var deniedContext in new[]{
            (ContextoPerfilImportacion)null,
            new ContextoPerfilImportacion{UsuarioRef="1",AmbitoRef="2",PuedeAdministrar=false},
            new ContextoPerfilImportacion{UsuarioRef="01",AmbitoRef="2",PuedeAdministrar=true},
            new ContextoPerfilImportacion{UsuarioRef="1",AmbitoRef="0",PuedeAdministrar=true}
        }){
            Reject(()=>consumerRepo.Listar(deniedContext,1,25,null,null),"ACCESO_DENEGADO");
            Reject(()=>consumerRepo.Obtener(deniedContext,"1","3"),"ACCESO_DENEGADO");
            Reject(()=>adminRepo.Listar(deniedContext,new SolicitudProveedorAdmin()),"ACCESO_DENEGADO");
            Reject(()=>adminRepo.Obtener(deniedContext,"1","3"),"ACCESO_DENEGADO");
            Reject(()=>adminRepo.Mutar(deniedContext,"CREAR",new SolicitudProveedorAdmin(),null),"ACCESO_DENEGADO");
        }
        var authorized=new ContextoPerfilImportacion{UsuarioRef="1",AmbitoRef="2",PuedeAdministrar=true,PuedeAdministrarProveedores=true};
        foreach(var invalidId in new[]{"0","01","-1","1 OR 1=1","4294967296"}){
            Reject(()=>consumerRepo.Obtener(authorized,invalidId,"3"),"ACCESO_DENEGADO");
            Reject(()=>consumerRepo.Obtener(authorized,"1",invalidId),"ACCESO_DENEGADO");
            Reject(()=>consumerRepo.Listar(authorized,1,25,invalidId,"3"),"SOLICITUD_INVALIDA");
            Reject(()=>adminRepo.Obtener(authorized,invalidId,"3"),"SOLICITUD_INVALIDA");
            Reject(()=>adminRepo.Obtener(authorized,"1",invalidId),"SOLICITUD_INVALIDA");
        }
        Reject(()=>consumerRepo.Listar(authorized,1,25,"1",null),"SOLICITUD_INVALIDA");
        Reject(()=>consumerRepo.Listar(authorized,1,25,null,"3"),"SOLICITUD_INVALIDA");
        Reject(()=>consumerRepo.Listar(authorized,0,25,null,null),"SOLICITUD_INVALIDA");
        Reject(()=>consumerRepo.Listar(authorized,1,51,null,null),"SOLICITUD_INVALIDA");
        var profileOnly=new ContextoPerfilImportacion{UsuarioRef="1",AmbitoRef="2",PuedeAdministrar=true};
        Reject(()=>adminRepo.Obtener(profileOnly,"1","3"),"ACCESO_DENEGADO");
        Reject(()=>adminRepo.Mutar(profileOnly,"GUARDAR",new SolicitudProveedorAdmin(),null),"ACCESO_DENEGADO");
        int reads=0, rows=1; bool timeout=false, missing=false;
        var context=new ContextoPerfilImportacion{UsuarioRef="1",AmbitoRef="2",PuedeAdministrar=true};
        var version=new ProveedorIndicesVersion{ProveedorId="1",VersionId="3",AmbitoRef="2",Activo=true,ConexionRef="LOGICA",Tabla="ordenes_fixture",ColumnaClave="numero",ColumnaEmpresa="empresa",Propiedades=new List<PropiedadProveedorIndices>{new PropiedadProveedorIndices{Nombre="beneficiario",Columna="nombre"}}};
        var adapter=new MySqlProveedorDatosIndices(alias=>alias=="LOGICA"?"FAKE":null,(connection,sql,args)=>{
            reads++;
            Check(connection=="FAKE","server resolves logical connection");
            if(timeout)throw new TimeoutException("private details");
            var t=new DataTable();
            if(sql.Contains("information_schema")){
                Check((string)args[1]==version.Tabla,"metadata scoped to configured table");
                foreach(var c in new[]{"COLUMN_NAME","DATA_TYPE","COLUMN_TYPE","CHARACTER_MAXIMUM_LENGTH","NUMERIC_PRECISION","NUMERIC_SCALE","CHARACTER_SET_NAME"})t.Columns.Add(c,typeof(string));
                t.Rows.Add(version.ColumnaClave,"varchar","varchar(30)","30",DBNull.Value,DBNull.Value,"utf8");
                t.Rows.Add("empresa","int","int unsigned",DBNull.Value,"10","0",DBNull.Value);
                if(!missing)t.Rows.Add(version.Propiedades[0].Columna,"varchar","varchar(100)","100",DBNull.Value,DBNull.Value,"utf8");
            }else{
                Check(sql.Contains("FROM `"+version.Tabla+"`"),"same adapter uses configured table");
                Check(sql.Contains("`"+version.ColumnaClave+"`=@clave"+(String.IsNullOrEmpty(version.ColumnaEmpresa)?"":" AND `empresa`=@empresa")+" LIMIT 2"),"key and company parameterized, bounded");
                Check((string)args[1]=="00125" && (String.IsNullOrEmpty(version.ColumnaEmpresa)?args.Length==2:(string)args[3]=="2"),"leading zero and trusted company preserved");
                t.Columns.Add("beneficiario",typeof(string));for(int i=0;i<rows;i++)t.Rows.Add("Persona fixture");
            }
            return t;
        },(c,v)=>true);
        var result=adapter.Consultar(context,version,"00125");
        Check(result.Codigo=="OK" && result.Valores["beneficiario"]=="Persona fixture","first table");
        version.Tabla="solicitudes_fixture";version.ColumnaClave="referencia";version.Propiedades[0].Columna="razon";
        Check(adapter.Consultar(context,version,"00125").Codigo=="OK","second table without new code");
        version.ColumnaEmpresa="";
        Check(adapter.Consultar(context,version,"00125").Codigo=="OK","exclusive source without company column");
        var deniedAdapter=new MySqlProveedorDatosIndices(alias=>{throw new Exception("must not resolve unauthorized source");},null,(c,v)=>false);
        Check(deniedAdapter.Consultar(context,version,"00125").Codigo=="ACCESO_DENEGADO","empty company column does not bypass authorization");
        rows=2;Check(adapter.Consultar(context,version,"00125").Codigo=="AMBIGUO","exclusive source duplicate keys rejected");rows=1;
        version.ColumnaEmpresa="empresa";
        rows=0;Check(adapter.Consultar(context,version,"00125").Codigo=="SIN_COINCIDENCIA","zero rows");
        rows=2;Check(adapter.Consultar(context,version,"00125").Codigo=="AMBIGUO","two rows");rows=1;
        int before=reads;version.AmbitoRef="99";Check(adapter.Consultar(context,version,"00125").Codigo=="ACCESO_DENEGADO" && before==reads,"cross company denied before IO");version.AmbitoRef="2";
        version.Tabla="x`; DROP TABLE y";Check(adapter.Consultar(context,version,"00125").Codigo=="CONFIGURACION_INVALIDA" && before==reads,"identifier rejected before IO");version.Tabla="solicitudes_fixture";
        missing=true;before=reads;Check(adapter.Consultar(context,version,"00125").Codigo=="CONFIGURACION_INVALIDA" && reads==before+1,"schema mismatch prevents data query");missing=false;
        version.ConexionRef="UNKNOWN";before=reads;Check(adapter.Consultar(context,version,"00125").Codigo=="ORIGEN_NO_DISPONIBLE" && before==reads,"unknown origin");version.ConexionRef="LOGICA";
        timeout=true;Check(adapter.Consultar(context,version,"00125").Codigo=="ORIGEN_NO_DISPONIBLE","timeout is sanitized");
        Check(adapter.Consultar(context,version,"").Codigo=="CLAVE_INVALIDA","missing key never becomes zero");
        version.TipoClave="FECHA";before=reads;
        Check(adapter.Consultar(context,version,"2026-02-30").Codigo=="CLAVE_INVALIDA" && reads==before,"invalid date rejected before IO");
        version.TipoClave="TEXTO";
        Check(adapter.Consultar(context,version,"bad\0key").Codigo=="CLAVE_INVALIDA","null character rejected");
        var json=PerfilImportacionJson.Escribir(version.Propiedades);
        Check(PerfilImportacionJson.Leer<List<PropiedadProveedorIndices>>(json)[0].Nombre=="beneficiario","closed metadata JSON round trip");
        // Simular fallo del lector de política, no del origen de negocio: no debe
        // resolverse DA ni consultarse metadatos/filas con una autorización antigua.
        int policyReads=0, resolvedOrigins=0, sourceQueries=0;
        var policyFailure=new MySqlProveedorDatosIndices(alias=>{resolvedOrigins++;return "FAKE";},
            (connection,sql,args)=>{sourceQueries++;throw new Exception("source must not be queried");},
            (ctx,v)=>{policyReads++;throw new PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE","No se pudo leer la politica.");});
        bool policyRejected=false;
        try{policyFailure.Consultar(context,version,"00125");}
        catch(PerfilImportacionException error){policyRejected=error.Codigo=="CAPACIDAD_NO_DISPONIBLE";}
        Check(policyRejected && policyReads==1,"policy read failure propagates as unavailable instead of permission or cached data");
        Check(resolvedOrigins==0 && sourceQueries==0,"policy failure prevents connection resolution and all source queries");
        var emptyPolicy=new MySqlProveedorDatosIndices(alias=>{resolvedOrigins++;return "FAKE";},
            (connection,sql,args)=>{sourceQueries++;throw new Exception("source must not be queried");},
            (ctx,v)=>{policyReads++;return false;});
        var denied=emptyPolicy.Consultar(context,version,"00125");
        Check(denied.Codigo=="ACCESO_DENEGADO" && denied.Valores.Count==0,"empty or revoked policy produces no business values");
        Check(policyReads==2 && resolvedOrigins==0 && sourceQueries==0,"deny is checked before all source IO");
    }
}
