using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public static class ProveedorIndicesReglasBehaviorTests {
    static void Check(bool ok,string name){if(!ok)throw new Exception(name);}
    static ReglaPerfilImportacion Rule(string id,string property){
        return new ReglaPerfilImportacion{DestinoTipo="INDICE_GABINETE",DestinoRef=id,FuenteTipo="PROVEEDOR",Obligatoria=true,
            Proveedor=new FuenteProveedorIndices{ProveedorId="1",VersionId="3",Propiedad=property,
                Clave=new ClaveProveedorIndices{FuenteTipo="ARCHIVO",Extraccion="SEGMENTO",Separador="-",Posicion=0,QuitarExtension=true}}};
    }
    public static void Run(){
        DiagnosticCases();
        var legacy=PerfilImportacionJson.Leer<VersionPerfilImportacion>(File.ReadAllText("doc110-perfil-antes-proveedor.json"));
        Check(BitConverter.ToString(PerfilImportacionJson.Huella(legacy)).Replace("-","").ToLowerInvariant()=="c16eaf987e48e6897edfadd215b7803a97d484b02f1540e97257c58217f4f401","publication hash before provider unchanged");
        Check(!PerfilImportacionJson.Escribir(legacy).Contains("Proveedor"),"legacy JSON remains unchanged");
        var rules=new List<ReglaPerfilImportacion>{Rule("10","nombre"),Rule("11","nit")};
        var resolver=new ResolvedorReglasPerfilImportacion();
        Check(resolver.ValidarEstructura(rules).Count==0,"provider rule shape");
        Check(resolver.Resolver(rules,new EjemploPerfilImportacion()).Incidencias.All(x=>x.Codigo=="FUENTE_NO_RESUELTA"),"pure resolver never fabricates provider values");
        var copy=PerfilImportacionJson.Leer<ReglaPerfilImportacion>(PerfilImportacionJson.Escribir(rules[0]));
        Check(copy.Proveedor.Clave.Separador=="-","new rule closed JSON round trip");
        bool rejected=false;try{PerfilImportacionJson.Leer<FuenteProveedorIndices>("{\"Sql\":\"select 1\"}");}catch(PerfilImportacionException){rejected=true;}
        Check(rejected,"unknown source properties rejected");
        var repo=new Repo();var provider=new Provider();var coordinator=new CoordinadorProveedorIndices(repo,provider);
        var context=new ContextoPerfilImportacion{UsuarioRef="1",AmbitoRef="2",PuedeAdministrar=true};
        Check(coordinator.Validar(context,rules).Count==0 && provider.Calls==0,"configuration validates without querying business rows");
        repo.Draft=true;
        Check(coordinator.Validar(context,rules).Count==2,"active draft cannot be used by profile even with known IDs");
        bool draftRejected=false;
        try{coordinator.Ensayar(context,rules,new List<EjemploPerfilImportacion>{new EjemploPerfilImportacion{NombreArchivo="00125-abc.pdf"}});}catch(PerfilImportacionException){draftRejected=true;}
        Check(draftRejected && provider.Calls==0,"draft rejected before source lookup");repo.Draft=false;
        var examples=new List<EjemploPerfilImportacion>{new EjemploPerfilImportacion{NombreArchivo="00125-abc.pdf"},new EjemploPerfilImportacion{NombreArchivo="00125-other.pdf"}};
        var output=coordinator.Ensayar(context,rules,examples);
        Check(provider.Calls==1 && provider.Key=="00125","one request query shared by fields and equal keys");
        Check(output.All(r=>r.Incidencias.Count==0 && r.Valores.Count==2 && r.Valores.All(v=>v.FuenteTipo=="PROVEEDOR")),"resolved field origin preserved");
        Check(rules[0].FuenteTipo=="PROVEEDOR" && rules[0].ValorConstante==null,"persisted rules not mutated by rehearsal");
        coordinator.Ensayar(context,rules,examples);Check(provider.Calls==2,"new rehearsal does not reuse cached values");
        provider.Code="AMBIGUO";output=coordinator.Ensayar(context,rules,examples);
        Check(output.All(r=>r.Valores.Count==0 && r.Incidencias.Count==2),"ambiguous response reported per field");provider.Code="OK";
        rules[0].Proveedor.Clave=new ClaveProveedorIndices{FuenteTipo="EXPEDIENTE",PropiedadExpediente="CODIGO"};rules[0].EtapaResolucion="TRAS_EXPEDIENTE";
        int before=provider.Calls;output=coordinator.Ensayar(context,new List<ReglaPerfilImportacion>{rules[0]},examples);
        Check(provider.Calls==before && output.All(r=>r.Valores.Single().Diferido),"future expediente key remains deferred without calls");
        rules[0].Proveedor.Clave.FuenteTipo="PROVEEDOR";
        Check(resolver.ValidarEstructura(rules).Count>0,"nested providers forbidden");
        rules[0]=Rule("10","missing");Check(coordinator.Validar(context,rules).Count==1,"undeclared property rejected");
        rules[0]=Rule("10","nombre");rules[0].DestinoTipo="CAMPO_EXPEDIENTE";
        Check(resolver.ValidarEstructura(rules).Count>0,"provider limited to indices");
        var dateRule=Rule("12","fecha");dateRule.TipoValor="FECHA";dateRule.FormatoFecha="dd/MM/yyyy H:mm:ss";
        var dateRules=new List<ReglaPerfilImportacion>{dateRule};
        Check(coordinator.Validar(context,dateRules).Count==0,"explicit text date conversion compatible");
        foreach(var culture in new[]{"es-CO","en-US"}){
            var previous=System.Threading.Thread.CurrentThread.CurrentCulture;
            try{System.Threading.Thread.CurrentThread.CurrentCulture=new System.Globalization.CultureInfo(culture);
                provider.Date="23/01/2006 0:00:00";
                var result=coordinator.Ensayar(context,dateRules,examples)[0];
                Check(result.Incidencias.Count==0 && result.Valores.Single().Valor=="2006-01-23","date parsing independent of culture");
                Check(provider.Date=="23/01/2006 0:00:00" && dateRule.FormatoFecha=="dd/MM/yyyy H:mm:ss","source and rule remain untouched");
            }finally{System.Threading.Thread.CurrentThread.CurrentCulture=previous;}
        }
        foreach(var invalid in new[]{"31/02/2006 0:00:00","23/01/2006 25:00:00","2006-01-23","",null}){
            provider.Date=invalid;Check(coordinator.Ensayar(context,dateRules,examples)[0].Incidencias.Any(x=>x.Codigo=="FECHA_INVALIDA"),"invalid text date rejected");
        }
        dateRule.FormatoFecha="yyyy-MM-dd";provider.Date="2006-01-23";
        Check(coordinator.Ensayar(context,dateRules,examples)[0].Valores.Single().Valor=="2006-01-23","canonical date preserved");
    }
    static void DiagnosticCases(){
        var repo=new Repo();var provider=new Provider();var coordinator=new CoordinadorProveedorIndices(repo,provider);
        var context=new ContextoPerfilImportacion{UsuarioRef="1",AmbitoRef="2",PuedeAdministrar=true};
        var rule=Rule("546","nombre");rule.Proveedor.Clave.LongitudMaxima=3;
        var rules=new List<ReglaPerfilImportacion>{rule};
        var samples=new List<EjemploPerfilImportacion>{new EjemploPerfilImportacion{NombreArchivo="00125-archivo.pdf"}};
        string before=PerfilImportacionJson.Escribir(rules);
        var incident=coordinator.Ensayar(context,rules,samples)[0].Incidencias.Single();
        Check(incident.Codigo=="LONGITUD_EXCEDIDA"&&incident.Campo=="INDICE_GABINETE:546"&&incident.Mensaje.StartsWith("Clave de consulta del proveedor:")&&incident.Mensaje.Contains("Longitud observada: 5. Máximo permitido: 3."),"key extraction preserves real cause lengths and destination");
        Check(provider.Calls==0&&PerfilImportacionJson.Escribir(rules)==before,"key diagnostic must not query or change rules");
        rule.Proveedor.Clave.LongitudMaxima=30;rule.Proveedor.Clave.Posicion=3;
        incident=coordinator.Ensayar(context,rules,samples)[0].Incidencias.Single();
        Check(incident.Codigo=="SEGMENTO_AUSENTE"&&incident.Mensaje.Contains("Segmento requerido: 4 (desde 1)"),"key diagnostic preserves required segment");
        rule.Proveedor.Clave.Posicion=0;provider.Code="CLAVE_INVALIDA";provider.RejectedLength=57;provider.MaximumLength=20;
        incident=coordinator.Ensayar(context,rules,samples)[0].Incidencias.Single();
        Check(incident.Codigo=="CLAVE_INVALIDA"&&incident.Mensaje.Contains("Longitud observada: 57. Máximo permitido: 20.")&&incident.Mensaje.Contains("campo de búsqueda del proveedor"),"physical key diagnostic preserves metadata limit without changing code");
        provider.RejectedLength=null;provider.MaximumLength=null;
        incident=coordinator.Ensayar(context,rules,samples)[0].Incidencias.Single();
        Check(incident.Mensaje.Contains("longitud, el tipo o los caracteres")&&!incident.Mensaje.Contains("Máximo permitido"),"unknown physical cause never invents a length");
        provider.Code="OK";rule.LongitudMaxima=3;
        incident=coordinator.Ensayar(context,rules,samples)[0].Incidencias.Single();
        Check(incident.Codigo=="LONGITUD_EXCEDIDA"&&!incident.Mensaje.Contains("Clave de consulta")&&incident.Mensaje.Contains("Máximo permitido: 3."),"returned field value error must not be mislabeled as a key error");
        provider.Code="PRIVATE_SQL_PASSWORD";
        incident=coordinator.Ensayar(context,rules,samples)[0].Incidencias.Single();
        Check(incident.Codigo=="ORIGEN_NO_DISPONIBLE"&&!incident.Mensaje.Contains("PRIVATE_SQL_PASSWORD"),"unknown provider code remains sanitized");
        Console.WriteLine("DOC-110 provider diagnostics: extraction, physical key limit, field value and safe fallback passed");
    }
    public class Repo:IProveedorIndicesRepository{
        public bool Draft,Unavailable,Missing;
        public Action<ProveedorIndicesVersion> Alter;
        public List<OpcionProveedorIndices> Listar(ContextoPerfilImportacion c,int page,int size,string p,string v){return new List<OpcionProveedorIndices>();}
        public ProveedorIndicesVersion Obtener(ContextoPerfilImportacion c,string p,string v){
            if(Unavailable)throw new PerfilImportacionException("ACCESO_DENEGADO","Proveedor no disponible para el contexto.");
            if(Missing)return null;
            var result=new ProveedorIndicesVersion{ProveedorId=p,VersionId=v,AmbitoRef=c.AmbitoRef,Activo=true,Estado=Draft?"BORRADOR":"PUBLICADA",TipoClave="TEXTO",
                Propiedades=new List<PropiedadProveedorIndices>{new PropiedadProveedorIndices{Nombre="nombre",Tipo="TEXTO"},new PropiedadProveedorIndices{Nombre="nit",Tipo="TEXTO"},new PropiedadProveedorIndices{Nombre="fecha",Tipo="TEXTO"}}};
            if(Alter!=null)Alter(result);
            return result;
        }
    }
    public class Provider:IProveedorDatosIndices{
        public int Calls; public string Key,Code="OK",Date="23/01/2006 0:00:00";public int? RejectedLength,MaximumLength;
        public ResultadoProveedorIndices Consultar(ContextoPerfilImportacion c,ProveedorIndicesVersion v,string key){Calls++;Key=key;return new ResultadoProveedorIndices{Codigo=Code,LongitudClaveRechazada=RejectedLength,LongitudMaximaClave=MaximumLength,Valores=new Dictionary<string,string>{{"nombre","Persona fixture"},{"nit","00123"},{"fecha",Date}}};}
    }
}
