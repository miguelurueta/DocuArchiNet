using System;
using System.Collections.Generic;
using System.Linq;

public static class PerfilImportacionServiceBehaviorTests {
    static void Check(bool value, string name) { if (!value) throw new Exception(name); }
    static void Reject(Action action, string code) {
        try { action(); } catch(PerfilImportacionException error) { Check(error.Codigo==code,"expected "+code); return; }
        throw new Exception("expected rejection "+code);
    }
    public static void Run() {
        var repo = new RepositoryDouble();
        var catalog = new CatalogDouble();
        var service = new ServicioPerfilImportacion(repo,catalog);
        var context = new ContextoPerfilImportacion {UsuarioRef="1",AmbitoRef="2",PuedeAdministrar=true};
        var filter = new FiltroPerfilesImportacion {Termino="Orden",Pagina=1,TamanoPagina=10};
        service.Listar(context,filter);
        Check(repo.LastFilter.Termino=="Orden" && repo.LastContext.AmbitoRef=="2","search preserves term and authorized scope");
        Reject(()=>service.Listar(context,new FiltroPerfilesImportacion {Termino=new string('a',161)}),"SOLICITUD_INVALIDA");
        Reject(()=>service.Listar(new ContextoPerfilImportacion {UsuarioRef="1",AmbitoRef="2",PuedeAdministrar=false},filter),"ACCESO_DENEGADO");
        var config = new ConfiguracionPerfilImportacion {
            NombreVisible="Órdenes",GabineteId=1,FondoRef="1",InstrumentoRef="2",AreaRef="3",SerieRef="4",
            TipoExpedienteRef="1",UnidadDocumentalRef="1",EstrategiaCarpetas="RAIZ_EXPEDIENTE",
            PoliticaSubcarpetas="RECHAZAR",PoliticaCarpetasVacias="RECHAZAR",PoliticaArchivosSueltos="RECHAZAR"
        };
        var rules = new List<ReglaPerfilImportacion> {
            Constant("CAMPO_EXPEDIENTE","CODIGO","TEXTO","000123"),
            Constant("CAMPO_EXPEDIENTE","NOMBRE","TEXTO","Orden"),
            Constant("CAMPO_EXPEDIENTE","FECHA_INICIAL","FECHA","2026-10-20"),
            Constant("TIPOLOGIA","TIPO_DOCUMENTAL","REFERENCIA","1")
        };
        repo.Profile = new PerfilImportacion {
            PerfilId="1", AmbitoRef="2",Revision=1,Activo=true,
            Version=new VersionPerfilImportacion {VersionId="2",Revision=1,Estado="BORRADOR",Configuracion=config,Reglas=rules}
        };
        Reject(()=>service.Publicar(context,repo.Profile.PerfilId,repo.Profile.Version.VersionId,1,1),"CONFIGURACION_INVALIDA");
        Check(repo.Writes==0,"incomplete publication cannot reach persistence");
        config.AmbitoIntegridad="EXPEDIENTE";
        catalog.Reject=true;
        Reject(()=>service.Publicar(context,repo.Profile.PerfilId,repo.Profile.Version.VersionId,1,1),"CONFIGURACION_INVALIDA");
        Check(repo.Writes==0,"catalog rejection cannot publish");
        catalog.Reject=false;
        Reject(()=>service.Publicar(context,repo.Profile.PerfilId,repo.Profile.Version.VersionId,2,1),"CONFLICTO_REVISION");
        Check(repo.Writes==0,"stale header rejected");
        service.Publicar(context,repo.Profile.PerfilId,repo.Profile.Version.VersionId,1,1);
        Check(repo.Writes==1 && repo.Hash.Length==32,"validated publication delegates with snapshot hash");
        foreach(var invalid in new[]{"0","-1","01","+1","1.0"," 1","4294967296",new string('a',32)})
            Reject(()=>service.Obtener(context,invalid,null),"SOLICITUD_INVALIDA");
        service.Obtener(context,"4294967295",null);
        context.PuedeAdministrar=false;
        int reads=repo.Reads;
        Reject(()=>service.Obtener(context,repo.Profile.PerfilId,null),"ACCESO_DENEGADO");
        Check(repo.Reads==reads,"authorization precedes resource lookup");
        context.PuedeAdministrar=true;
        Reject(()=>service.Seleccionar(context,repo.Profile.PerfilId,repo.Profile.Version.VersionId,"1","IMPORTACION"),"CAPACIDAD_NO_DISPONIBLE");
        repo.Profile.Version.Estado="PUBLICADA";
        Reject(()=>service.Guardar(context,repo.Profile.PerfilId,repo.Profile.Version),"CONFLICTO_REVISION");
        int before=repo.Writes;
        service.Ensayar(context,repo.Profile.PerfilId,repo.Profile.Version,new List<EjemploPerfilImportacion>{new EjemploPerfilImportacion()});
        Check(repo.Writes==before,"rule rehearsal has no writes");
        repo.Profile.Activo=false;
        Reject(()=>service.Seleccionar(context,repo.Profile.PerfilId,repo.Profile.Version.VersionId,"1","ADMINISTRACION"),"ACCESO_DENEGADO");
        Check(PerfilImportacionContextAdapter.PermisoValido(1),"exact permission");
        foreach(object value in new object[]{null,DBNull.Value,0,2,-1,"true","01",true})
            Check(!PerfilImportacionContextAdapter.PermisoValido(value),"permission fails closed");
        repo.Profile.Activo=true;repo.Profile.Version.Estado="BORRADOR";
        var providerRule=new ReglaPerfilImportacion{DestinoTipo="INDICE_GABINETE",DestinoRef="99",FuenteTipo="PROVEEDOR",
            Proveedor=new FuenteProveedorIndices{ProveedorId="1",VersionId="3",Propiedad="nombre",Clave=new ClaveProveedorIndices{FuenteTipo="ARCHIVO"}}};
        repo.Profile.Version.Reglas.Add(providerRule);
        var provider=new ProveedorIndicesReglasBehaviorTests.Provider();
        var providerRepo=new ProveedorIndicesReglasBehaviorTests.Repo();
        var withProvider=new ServicioPerfilImportacion(repo,catalog,new CoordinadorProveedorIndices(providerRepo,provider));
        before=repo.Writes;
        withProvider.Guardar(context,repo.Profile.PerfilId,repo.Profile.Version);
        Check(repo.Writes==before+1 && provider.Calls==0,"saving validates provider contract without reading business data");
        var rehearsed=withProvider.Ensayar(context,repo.Profile.PerfilId,repo.Profile.Version,new List<EjemploPerfilImportacion>{new EjemploPerfilImportacion{NombreArchivo="00125"}});
        Check(rehearsed[0].Incidencias.Count==0 && rehearsed[0].Valores.Any(v=>v.FuenteTipo=="PROVEEDOR") && catalog.ValueChecks>0,"service resolves provider and applies cabinet value validation");
        before=repo.Writes;providerRule.Proveedor.Propiedad="unknown";
        Reject(()=>withProvider.Guardar(context,repo.Profile.PerfilId,repo.Profile.Version),"CONFIGURACION_INVALIDA");
        Check(before==repo.Writes,"invalid property never saved");providerRule.Proveedor.Propiedad="nombre";
        // Revalidar el snapshot persistido: no basta con haber guardado cuando el proveedor era válido.
        before=repo.Writes;
        int sourceReads=provider.Calls;
        providerRepo.Draft=true;
        var draftSnapshot=PerfilImportacionJson.Escribir(repo.Profile);
        Reject(()=>withProvider.Publicar(context,"1","2",1,1),"CONFIGURACION_INVALIDA");
        Check(repo.Writes==before && PerfilImportacionJson.Escribir(repo.Profile)==draftSnapshot,"draft provider cannot publish or mutate stored profile");
        repo.Profile.Version.Estado="PUBLICADA";
        var publishedSnapshot=PerfilImportacionJson.Escribir(repo.Profile);
        Reject(()=>withProvider.Seleccionar(context,"1","2","1","ADMINISTRACION"),"CONFIGURACION_INVALIDA");
        Check(repo.Writes==before && PerfilImportacionJson.Escribir(repo.Profile)==publishedSnapshot,"draft provider cannot be selected through a published profile");
        Check(provider.Calls==sourceReads,"rejected publication and selection never query provider rows");
        providerRepo.Draft=false;
        // El repositorio MySQL deniega directamente referencias no disponibles antes de devolver metadatos.
        providerRepo.Unavailable=true;
        Reject(()=>withProvider.Seleccionar(context,"1","2","1","ADMINISTRACION"),"ACCESO_DENEGADO");
        Check(PerfilImportacionJson.Escribir(repo.Profile)==publishedSnapshot,"unavailable provider leaves historical publication intact");
        repo.Profile.Version.Estado="BORRADOR";
        Reject(()=>withProvider.Publicar(context,"1","2",1,1),"ACCESO_DENEGADO");
        Check(repo.Writes==before && provider.Calls==sourceReads && PerfilImportacionJson.Escribir(repo.Profile)==draftSnapshot,"unavailable provider prevents publication without side effects");
        providerRepo.Unavailable=false;repo.Profile.Version.Estado="PUBLICADA";
        Check(Object.ReferenceEquals(withProvider.Seleccionar(context,"1","2","1","ADMINISTRACION"),repo.Profile),"published provider permits administrative selection from persisted profile");
        Check(repo.Writes==before && provider.Calls==sourceReads,"selection validates configuration without writes or business queries");
        // Defensa del servicio incluso si un repositorio devuelve metadatos incorrectos.
        var invalidProviders=new Dictionary<string,Action<ProveedorIndicesVersion>>{
            {"foreign scope",v=>v.AmbitoRef="3"},
            {"different provider",v=>v.ProveedorId="9"},
            {"different version",v=>v.VersionId="9"},
            {"inactive provider",v=>v.Activo=false},
            {"draft version",v=>v.Estado="BORRADOR"},
            {"incompatible key",v=>v.TipoClave="FECHA"},
            {"missing properties",v=>v.Propiedades=null},
            {"unknown property",v=>v.Propiedades.RemoveAll(p=>p.Nombre=="nombre")},
            {"duplicate property",v=>v.Propiedades.Add(new PropiedadProveedorIndices{Nombre="nombre",Tipo="TEXTO"})},
            {"incompatible property",v=>v.Propiedades.Single(p=>p.Nombre=="nombre").Tipo="FECHA"},
            {"nonexistent provider",null},
            {"repository access denied",null}
        };
        foreach(var scenario in invalidProviders){
            providerRepo.Alter=scenario.Value;
            providerRepo.Missing=scenario.Key=="nonexistent provider";
            providerRepo.Unavailable=scenario.Key=="repository access denied";
            var expected=providerRepo.Unavailable?"ACCESO_DENEGADO":"CONFIGURACION_INVALIDA";
            foreach(var operation in new[]{"save","publish","select"}){
                repo.Profile.Version.Estado=operation=="select"?"PUBLICADA":"BORRADOR";
                var original=PerfilImportacionJson.Escribir(repo.Profile);
                if(operation=="save")Reject(()=>withProvider.Guardar(context,"1",repo.Profile.Version),expected);
                else if(operation=="publish")Reject(()=>withProvider.Publicar(context,"1","2",1,1),expected);
                else Reject(()=>withProvider.Seleccionar(context,"1","2","1","ADMINISTRACION"),expected);
                Check(repo.Writes==before && provider.Calls==sourceReads && PerfilImportacionJson.Escribir(repo.Profile)==original,
                    scenario.Key+" rejected on "+operation+" without changing snapshot, writing or querying business data");
            }
        }
        providerRepo.Alter=null;providerRepo.Missing=false;providerRepo.Unavailable=false;
        repo.Profile.Version.Estado="BORRADOR";
        Reject(()=>service.Guardar(context,repo.Profile.PerfilId,repo.Profile.Version),"CAPACIDAD_NO_DISPONIBLE");
        repo.Profile.Version.Reglas.Remove(providerRule);
        var inherited=new ReglaPerfilImportacion{DestinoTipo="INDICE_GABINETE",DestinoRef="99",FuenteTipo="EXPEDIENTE",PropiedadExpediente="SERIE_CODIGO",EtapaResolucion="TRAS_EXPEDIENTE",TipoValor="TEXTO"};
        repo.Profile.Version.Reglas.Add(inherited);
        var spoof=new EjemploPerfilImportacion();spoof.Expediente["SERIE_CODIGO"]="CLIENT_VALUE";
        before=repo.Writes;
        var projection=service.Ensayar(context,"1",repo.Profile.Version,new List<EjemploPerfilImportacion>{spoof})[0].Valores.Single(v=>v.DestinoRef=="99");
        Check(projection.Valor=="0007" && projection.Procedencia=="PROPUESTA_PERFIL","server catalog replaces client classification and marks proposal");
        Check(spoof.Expediente["SERIE_CODIGO"]=="CLIENT_VALUE" && before==repo.Writes,"rehearsal leaves input and persistence untouched");
        repo.Profile.Version.Estado="PUBLICADA";
        var snapshot=PerfilImportacionJson.Escribir(repo.Profile.Version);
        var forgedVersion=PerfilImportacionJson.Leer<VersionPerfilImportacion>(snapshot);forgedVersion.Reglas.Last().PropiedadExpediente="SUBSERIE_NOMBRE";
        projection=service.Ensayar(context,"1",forgedVersion,new List<EjemploPerfilImportacion>{spoof})[0].Valores.Single(v=>v.DestinoRef=="99");
        Check(projection.Valor=="0007" && PerfilImportacionJson.Escribir(repo.Profile.Version)==snapshot,"published classification uses persisted rules without mutation");
    }
    static ReglaPerfilImportacion Constant(string type,string field,string valueType,string value) {
        return new ReglaPerfilImportacion {DestinoTipo=type,DestinoRef=field,FuenteTipo="CONSTANTE",TipoValor=valueType,ValorConstante=value,FormatoFecha="yyyy-MM-dd"};
    }
    // Dobles explícitos: prueban orquestación, no transacciones ni aislamiento MySQL.
    class CatalogDouble : IPerfilImportacionCatalogos, IClasificacionEnsayoPerfil {
        public Dictionary<string,string> ClasificacionPropuesta(ContextoPerfilImportacion c,ConfiguracionPerfilImportacion p){return new Dictionary<string,string>{{"SERIE_CODIGO","0007"}};}
        public bool Reject;
        public int ValueChecks;
        public List<IncidenciaPerfilImportacion> Validar(ContextoPerfilImportacion c,ConfiguracionPerfilImportacion p,List<ReglaPerfilImportacion> r,bool publish) {
            return Reject ? new List<IncidenciaPerfilImportacion>{new IncidenciaPerfilImportacion("GabineteId","AJENO","No autorizado")} : new List<IncidenciaPerfilImportacion>();
        }
        public List<IncidenciaPerfilImportacion> ValidarValores(ContextoPerfilImportacion c,ConfiguracionPerfilImportacion p,List<ValorReglaPerfilImportacion> v) { ValueChecks++;return new List<IncidenciaPerfilImportacion>(); }
        public bool NivelCompatible(ContextoPerfilImportacion c,ConfiguracionPerfilImportacion p,string n) { return true; }
    }
    class RepositoryDouble : IPerfilImportacionRepository {
        public PerfilImportacion Profile; public int Writes, Reads; public byte[] Hash; public FiltroPerfilesImportacion LastFilter; public ContextoPerfilImportacion LastContext;
        public void VerificarEsquema() {}
        public PerfilImportacion Obtener(ContextoPerfilImportacion c,string p,string v) {Reads++;return Profile;}
        public PerfilImportacion Publicar(ContextoPerfilImportacion c,string p,string v,uint rp,uint rv,byte[] hash) {Writes++;Hash=hash;return Profile;}
        public PaginaPerfilesImportacion Listar(ContextoPerfilImportacion c,FiltroPerfilesImportacion f) {LastFilter=f;LastContext=c;return new PaginaPerfilesImportacion();}
        public List<VersionPerfilImportacion> Historial(ContextoPerfilImportacion c,string p,int page,int size) {throw new NotSupportedException();}
        public PerfilImportacion Crear(ContextoPerfilImportacion c,string code,string name,string desc) {throw new NotSupportedException();}
        public PerfilImportacion Guardar(ContextoPerfilImportacion c,string p,VersionPerfilImportacion v) {Writes++;return Profile;}
        public PerfilImportacion CrearVersion(ContextoPerfilImportacion c,string p,string v,uint rp,uint rv) {throw new NotSupportedException();}
        public PerfilImportacion ActualizarCabecera(ContextoPerfilImportacion c,string p,uint r,string name,string desc) {throw new NotSupportedException();}
        public PerfilImportacion CambiarDisponibilidad(ContextoPerfilImportacion c,string p,uint r,bool active) {throw new NotSupportedException();}
    }
}
