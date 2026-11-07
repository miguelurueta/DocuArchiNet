using System;
using System.Collections.Generic;
using System.Linq;
public static class PoliticaFuentesProveedorBehaviorTests {
    static void Check(bool value,string message){if(!value)throw new Exception(message);}
    public static void Run(){
        var restriction=new RestriccionTablasFuente();
        Check(restriction.Permite("DA","imp_02_egresos") && restriction.Permite("RA","imp_02_ordenes"),"default prefix both aliases");
        foreach(var deniedName in new[]{"usuarios","impX02_egresos","IMP_02_egresos","x_imp_02","imp_02%;DROP"})Check(!restriction.Permite("DA",deniedName),"literal prefix and identifier enforcement");
        var additional=new RestriccionTablasFuente("[{\"ConexionRef\":\"DA\",\"Tabla\":\"otra_tabla\"}]");
        Check(additional.Permite("DA","otra_tabla")&&!additional.Permite("RA","otra_tabla")&&!additional.Permite("DA","otra_tabla_extra"),"exact exception scoped to connection");
        bool invalidAdditional=false;try{new RestriccionTablasFuente("[{\"ConexionRef\":\"DA\",\"Tabla\":\"%\"}]");}catch(PerfilImportacionException){invalidAdditional=true;}Check(invalidAdditional,"wildcard rejected");
        var c=new ContextoPerfilImportacion{UsuarioRef="1",AmbitoRef="2",PuedeAdministrar=true,PuedeAdministrarProveedores=true};
        var json="[{\"AmbitoRef\":\"2\",\"ConexionRef\":\"RA\",\"Tabla\":\"imp_02_ordenes\",\"Columnas\":[\"numero\",\"empresa\",\"nombre\"]}]";
        var policy=new PoliticaFuentesProveedor(json);
        var forbiddenPolicy=new PoliticaFuentesProveedor(json.Replace("imp_02_ordenes","usuarios"));
        Check(forbiddenPolicy.Fuentes(c).Count==0,"persisted authorization cannot bypass server restriction");
        var exceptionPolicy=new PoliticaFuentesProveedor(json.Replace("imp_02_ordenes","otra_tabla").Replace("RA","DA"),additional);
        Check(exceptionPolicy.Fuentes(c).Count==1,"explicit exception still needs persisted authorization");
        var adminContext=new ContextoPerfilImportacion{UsuarioRef="1",AmbitoRef="2",PuedeAdministrar=true,PuedeAdministrarFuentes=true};
        var candidateMetadata=new MetadatosFuenteImportacion(alias=>{throw new Exception("forbidden table connected");});
        bool rejected=false;try{candidateMetadata.Consultar(adminContext,"COLUMNAS","DA","usuarios",1);}catch(PerfilImportacionException e){rejected=e.Codigo=="ACCESO_DENEGADO";}Check(rejected,"direct metadata bypass denied before connection");

        var v=new ProveedorIndicesVersion{ConexionRef="RA",Tabla="imp_02_ordenes",ColumnaClave="numero",ColumnaEmpresa="empresa",Propiedades=new List<PropiedadProveedorIndices>{new PropiedadProveedorIndices{Nombre="beneficiario",Columna="nombre"}}};
        Check(policy.Permite(c,v),"authorized exact source");
        v.Propiedades[0].Columna="password";Check(!policy.Permite(c,v),"column cannot be authorized by draft itself");v.Propiedades[0].Columna="nombre";
        c.AmbitoRef="3";Check(!policy.Permite(c,v),"company scope");c.AmbitoRef="2";
        Check(!new PoliticaFuentesProveedor(null).Permite(c,v),"missing policy is deny all");
        var metadata=new MetadatosProveedorIndices(policy,alias=>{throw new Exception("unexpected database access");});
        Check(metadata.Consultar(c,"CONEXIONES",null,null,1).Count==1,"only registered connection returned");
        Check(metadata.Consultar(c,"TABLAS","RA",null,1)[0].value=="imp_02_ordenes","only authorized table returned");
        bool denied=false;try{metadata.Consultar(c,"COLUMNAS","RA","usuarios",1);}catch(PerfilImportacionException e){denied=e.Codigo=="ACCESO_DENEGADO";}
        Check(denied,"unauthorized table rejected before connecting");
        v.ColumnaEmpresa="";
        Check(!policy.Permite(c,v),"missing company column denied by default");
        var exclusive=new PoliticaFuentesProveedor(json.Replace("\"ConexionRef\":", "\"ExclusivaEmpresa\":true,\"ConexionRef\":"));
        Check(exclusive.Permite(c,v),"explicit exclusive source accepts missing company column");
        var copies=exclusive.Fuentes(c);copies[0].ExclusivaEmpresa=false;
        Check(exclusive.Permite(c,v),"source copies cannot modify server authorization");
        c.AmbitoRef="3";Check(!exclusive.Permite(c,v),"exclusive source cannot cross company");c.AmbitoRef="2";
        v.ColumnaEmpresa="empresa";Check(policy.Permite(c,v),"legacy filtered source preserved");
        var exact=new PoliticaFuentesProveedor(json.Replace("\"ConexionRef\":", "\"ColumnaEmpresaAutorizada\":\"empresa\",\"ConexionRef\":"));
        Check(exact.Permite(c,v),"database policy exact company column");v.ColumnaEmpresa="nombre";Check(!exact.Permite(c,v),"authorized output column cannot replace company filter");v.ColumnaEmpresa="empresa";
        var exclusiveExact=new PoliticaFuentesProveedor(json.Replace("\"ConexionRef\":", "\"ExclusivaEmpresa\":true,\"ColumnaEmpresaAutorizada\":\"\",\"ConexionRef\":"));
        Check(!exclusiveExact.Permite(c,v),"exclusive database policy requires omitted company column");v.ColumnaEmpresa="";Check(exclusiveExact.Permite(c,v),"exclusive database policy accepts empty company");
        c.PuedeAdministrarProveedores=false;denied=false;try{metadata.Consultar(c,"CONEXIONES",null,null,1);}catch(PerfilImportacionException e){denied=e.Codigo=="ACCESO_DENEGADO";}
        Check(denied,"profile permission alone insufficient");
        bool malformed=false;try{new PoliticaFuentesProveedor(json.Replace("imp_02_ordenes","x;DROP"));}catch(PerfilImportacionException){malformed=true;}Check(malformed,"invalid identifier rejected");
        // Descubrimiento ordinario solo desde política autorizada, paginado sin IO de origen.
        c.PuedeAdministrarProveedores=true;
        var sources=new List<FuenteProveedorAutorizada>();
        for(int i=1;i<=26;i++)sources.Add(new FuenteProveedorAutorizada{AmbitoRef="2",ConexionRef="RA",Tabla="imp_02_t"+i.ToString("D2"),Columnas=new List<string>{"numero"}});
        sources.Add(new FuenteProveedorAutorizada{AmbitoRef="2",ConexionRef="DA",Tabla="imp_02_destino",Columnas=new List<string>{"numero"}});
        sources.Add(new FuenteProveedorAutorizada{AmbitoRef="3",ConexionRef="RA",Tabla="imp_02_ajena",Columnas=new List<string>{"numero"}});
        int originResolutions=0;
        var paged=new MetadatosProveedorIndices(new PoliticaFuentesProveedor(PerfilImportacionJson.Escribir(sources)),alias=>{originResolutions++;throw new Exception("unauthorized metadata must not resolve origin");});
        Check(paged.Consultar(c,"CONEXIONES",null,null,1).Select(o=>o.value).SequenceEqual(new[]{"DA","RA"}),"logical aliases only, no connection details");
        Check(paged.Consultar(c,"CONEXIONES",null,null,2).Count==0,"connection pagination exhausted");
        var first=paged.Consultar(c,"TABLAS","RA",null,1);var second=paged.Consultar(c,"TABLAS","RA",null,2);
        Check(first.Count==25&&second.Count==1&&second[0].value=="imp_02_t26","authorized tables paginate completely");
        Check(first.Concat(second).Select(o=>o.value).Distinct().Count()==26&&paged.Consultar(c,"TABLAS","RA",null,3).Count==0,"no duplicate or foreign table, final page empty");
        foreach(var name in new[]{"imp_02_ajena","imp_02_t01`;SELECT","DATABASE()"}){
            bool inaccessible=false;try{paged.Consultar(c,"COLUMNAS","RA",name,1);}catch(PerfilImportacionException e){inaccessible=e.Codigo=="ACCESO_DENEGADO";}
            Check(inaccessible,"direct unknown, foreign or expression table denied");
        }
        foreach(var page in new[]{0,1001}){
            bool invalidPage=false;try{paged.Consultar(c,"TABLAS","RA",null,page);}catch(PerfilImportacionException e){invalidPage=e.Codigo=="SOLICITUD_INVALIDA";}
            Check(invalidPage,"ordinary metadata page bounded before origin IO");
        }
        Check(originResolutions==0,"policy discovery and denied columns never resolve business origin");
    }
}
