using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

public static partial class ImportacionCarpetasBehaviorTests {
    sealed partial class QueryRepository : IConsultaFocalImportacionRepository {
        public int FocalReads;
        public Action<VistaConsultaFocalImportacion> AlterFocal;
        public VistaConsultaFocalImportacion ConsultarRecursoFocal(ContextoImportacionCarpetas c,string lot,string type,string id){
            Owner(c,lot);FocalReads++;
            var file=type=="ARCHIVOS"?Files.SingleOrDefault(f=>f.ArchivoId==id):null;
            var root=Roots.SingleOrDefault(r=>r.RaizId==(type=="RAICES"?id:file==null?null:file.RaizId));
            if(root==null)throw new PerfilImportacionException("ACCESO_DENEGADO","fixture");
            var view=new VistaConsultaFocalImportacion{Lectura=ConsultarEstado(c,lot),Raiz=ReceiptClone(root),Archivo=file==null?null:ReceiptClone(file),HuellaRecursos=new string('a',64),Progreso=new ProgresoConfirmadoImportacion()};
            if(AlterFocal!=null)AlterFocal(view);return view;
        }
    }
    static void SegmentedFocalServiceCases(){
        var resources=new ResourceFixture();var repository=new QueryRepository();repository.Lot.Contexto=resources.Adapter.ResolverDestino(resources.Http,"7");repository.Lot.Snapshot.Configuracion=resources.Configuration();
        var service=new ServicioConsultaImportacion(()=>resources.Adapter.Resolver(resources.Http),repository,id=>resources.Adapter.ResolverDestino(resources.Http,id,"CONSULTAR"),(c,p)=>resources.Adapter.AutorizarRecursos(resources.Http,c,p,"CONSULTAR"),25);
        var request=new ConsultarRecursoLoteSolicitud{loteId="20",tipo="ARCHIVOS",recursoId="30"};
        var original=PerfilImportacionJson.Escribir(repository.Files);var value=service.ConsultarFocal(request);
        Check(value.recurso.recursoId=="30"&&value.padre.recursoId=="40"&&value.estado.hayIncertidumbre,"DOC120 focal loses identity, parent or uncertainty");
        Check(!PerfilImportacionJson.Escribir(value).Contains("PRIVATE_"),"DOC120 focal response leaks private evidence");
        foreach(var alter in new Action<VistaConsultaFocalImportacion>[] {v=>v.Archivo.RaizId="41",v=>v.Raiz.LoteId="21",v=>v.Lectura.Lote.Snapshot.VersionId="999",v=>v.Lectura.Lote.Contexto.NivelRef="999",v=>v.HuellaRecursos="invalid",v=>v.Archivo.Revision=0}){
            repository.AlterFocal=alter;Unavailable(()=>service.ConsultarFocal(request));
        }
        repository.AlterFocal=null;request.tipo="RAICES";request.recursoId="40";Check(service.ConsultarFocal(request).padre==null,"DOC120 root focal invents a parent");
        int before=repository.FocalReads;repository.Lot.Contexto.UsuarioRef="99";Rejected("ACCESO_DENEGADO",()=>service.ConsultarFocal(request));Check(repository.FocalReads==before,"DOC120 other owner reaches focal resource");repository.Lot.Contexto.UsuarioRef="1";
        repository.Lot.Contexto.AmbitoRef="99";Rejected("ACCESO_DENEGADO",()=>service.ConsultarFocal(request));repository.Lot.Contexto.AmbitoRef="2";
        resources.ImportAllowed=false;Rejected("ACCESO_DENEGADO",()=>service.ConsultarFocal(request));resources.ImportAllowed=true;
        resources.CabinetAllowed=false;Rejected("ACCESO_DENEGADO",()=>service.ConsultarFocal(request));resources.CabinetAllowed=true;
        resources.Moved=true;Rejected("ACCESO_DENEGADO",()=>service.ConsultarFocal(request));resources.Moved=false;
        Check(PerfilImportacionJson.Escribir(repository.Files)==original,"DOC120 focal read mutated inventory");
        Console.WriteLine("DOC120 focal service scope, fresh authority, parent, privacy and malformed-result guards passed (memory port; no SQL/HTTP)");
    }
    sealed class SegmentedMemoryPort : IPreparacionSegmentadaImportacionRepository {
        public PreparacionSegmentadaImportacion P;public PlanImportacionCarpetas Plan;public bool Available=true;public int Starts,Writes,Seals;
        public readonly List<PartePreparacionImportacion> Parts=new List<PartePreparacionImportacion>();
        public readonly List<FragmentoPlanImportacion> Results=new List<FragmentoPlanImportacion>();
        static T Copy<T>(T x){return PerfilImportacionJson.Leer<T>(PerfilImportacionJson.Escribir(x));}
        public void VerificarEsquemaPreparacion(){if(!Available)throw new PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE","fixture");}
        void Owner(ContextoImportacionCarpetas c){if(P==null||c.UsuarioRef!=P.Contexto.UsuarioRef||c.AmbitoRef!=P.Contexto.AmbitoRef||!c.PuedeImportar)throw new PerfilImportacionException("ACCESO_DENEGADO","fixture");}
        void Cas(uint revision){if(P.Revision!=revision||P.Estado!="BORRADOR")throw new PerfilImportacionException("CONFLICTO_REVISION","fixture");}
        public PreparacionSegmentadaImportacion IniciarPreparacion(ContextoImportacionCarpetas c,PreparacionSegmentadaImportacion p){
            if(P!=null){Owner(c);if(p.Clave!=P.Clave||p.CabeceraHash!=P.CabeceraHash)throw new PerfilImportacionException("CONFLICTO_REVISION","fixture");return Copy(P);}
            Starts++;P=Copy(p);P.PreparacionId="77";return Copy(P);
        }
        public ContextoImportacionCarpetas ObtenerReferenciaPreparacion(ContextoImportacionCarpetas c,string id){Owner(c);if(id!="77")throw new PerfilImportacionException("ACCESO_DENEGADO","fixture");return Copy(P.Contexto);}
        public PreparacionSegmentadaImportacion ObtenerPreparacion(ContextoImportacionCarpetas c,string id){Owner(c);return Copy(P);}
        public PreparacionSegmentadaImportacion BuscarPreparacionPorClave(ContextoImportacionCarpetas c,string key){if(P==null)return null;Owner(c);return key==P.Clave?Copy(P):null;}
        public List<PartePreparacionImportacion> LeerPartesPreparacion(ContextoImportacionCarpetas c,string id){Owner(c);return Parts.Select(Copy).ToList();}
        public PreparacionSegmentadaImportacion RegistrarParte(ContextoImportacionCarpetas c,string id,uint revision,PartePreparacionImportacion part){
            Owner(c);var known=Parts.SingleOrDefault(x=>x.Tipo==part.Tipo&&x.Numero==part.Numero);
            if(known!=null){if(known.Huella!=part.Huella)throw new PerfilImportacionException("CONFLICTO_REVISION","fixture");return Copy(P);}
            Cas(revision);var row=Copy(part);row.Estado="REGISTRADA";Parts.Add(row);P.Revision++;Writes++;return Copy(P);
        }
        public PreparacionSegmentadaImportacion GuardarValidacionParte(ContextoImportacionCarpetas c,string id,uint revision,string type,uint number,string stamp,List<FragmentoPlanImportacion> fragments){
            Owner(c);var part=Parts.Single(x=>x.Tipo==type&&x.Numero==number);
            if(Parts.Any(x=>x.Estado=="VALIDADA"&&x.SelloCatalogos!=stamp))throw new PerfilImportacionException("CONFLICTO_REVISION","fixture");
            if(part.Estado=="VALIDADA")return Copy(P);Cas(revision);
            var prefix=MySqlImportacionCarpetasRepository.PrefijoResultadoParte(type,number);
            Results.AddRange(fragments.Select(f=>new FragmentoPlanImportacion{Tipo=prefix+f.Tipo,Numero=f.Numero,Contenido=f.Contenido,Huella=f.Huella}));
            part.Estado="VALIDADA";part.SelloCatalogos=stamp;P.Revision++;Writes++;return Copy(P);
        }
        public List<FragmentoPlanImportacion> LeerResultadosPreparacion(ContextoImportacionCarpetas c,string id){Owner(c);return Results.Select(Copy).ToList();}
        public PlanImportacionCarpetas SellarPreparacion(ContextoImportacionCarpetas c,string id,uint revision,string key,PlanImportacionCarpetas plan){
            Owner(c);if(P.Clave!=key)throw new PerfilImportacionException("CONFLICTO_REVISION","fixture");
            if(P.Estado=="SELLADA")return PlanSegmentadoImportacionJson.ClonarPlan(Plan);
            Cas(revision);RepositoryHelper("ExigirPartesCompletas",P,Parts,true);
            Plan=Prepare(plan);Plan.PlanId="11";P.PlanId="11";P.Estado="SELLADA";P.Revision++;Seals++;return PlanSegmentadoImportacionJson.ClonarPlan(Plan);
        }
        public PlanImportacionCarpetas ObtenerPlan(ContextoImportacionCarpetas c,string id){Owner(c);return PlanSegmentadoImportacionJson.ClonarPlan(Plan);}
        public ContextoImportacionCarpetas ObtenerContextoPlan(ContextoImportacionCarpetas c,string id){Owner(c);return Copy(P.Contexto);}
    }

    sealed class SegmentedServiceFixture {
        public readonly SegmentedMemoryPort Repo=new SegmentedMemoryPort();
        public readonly PublicationPort Profile=new PublicationPort();
        public ContextoImportacionCarpetas Identity=PlanFixture().Contexto;
        public DateTime Now=new DateTime(2026,11,16,14,0,0,DateTimeKind.Utc);
        public readonly ServicioPreparacionSegmentadaImportacion Service;
        public readonly ServicioPreparacionImportacion Calculation;
        public readonly IniciarPreparacionPlanSolicitud Start;
        public SegmentedServiceFixture(int perPart=100) {
            var catalog=new OperativeCatalogPort();
            Profile.Value.Configuracion=new ConfiguracionPerfilImportacion{NombreVisible="Fixture",GabineteId=7,HuellaOrigenGabinete=new string('a',64),FondoRef="1",InstrumentoRef="2",AreaRef="3",SerieRef="4",TipoExpedienteRef="5",UnidadDocumentalRef="6",ModoIntegridad="COMPLETO_OBLIGATORIO",AmbitoIntegridad="EXPEDIENTE",EstrategiaCarpetas="RAIZ_EXPEDIENTE",PoliticaSubcarpetas="INCLUIR",PoliticaCarpetasVacias="RECHAZAR",PoliticaArchivosSueltos="RECHAZAR",PoliticaCoincidencias="PERMITIR",Extensiones=new List<string>{"pdf"}};
            Profile.Value.Reglas=new List<ReglaPerfilImportacion>{new ReglaPerfilImportacion{DestinoTipo="CAMPO_EXPEDIENTE",DestinoRef="CODIGO",FuenteTipo="CARPETA",TipoValor="TEXTO",LongitudMaxima=45},new ReglaPerfilImportacion{DestinoTipo="CAMPO_EXPEDIENTE",DestinoRef="NOMBRE",FuenteTipo="CARPETA",TipoValor="TEXTO"},new ReglaPerfilImportacion{DestinoTipo="TIPOLOGIA",DestinoRef="TIPO_DOCUMENTAL",FuenteTipo="CONSTANTE",ValorConstante="1",TipoValor="TEXTO"}};
            Profile.Value.Reglas.Add(new ReglaPerfilImportacion{DestinoTipo="CAMPO_EXPEDIENTE",DestinoRef="FECHA_INICIAL",FuenteTipo="CONSTANTE",ValorConstante="2020-01-01",TipoValor="FECHA",FormatoFecha="yyyy-MM-dd"});
            Profile.Value.Huella=Hex(PerfilImportacionJson.Huella(new VersionPerfilImportacion{Configuracion=Profile.Value.Configuracion,Reglas=Profile.Value.Reglas}));
            var profiles=new AdaptadorPerfilImportacionOperativa(Profile,catalog,null);
            var limits=new LimitesImportacionCarpetas(10,2000,1000,1000000,786432,TimeSpan.FromMinutes(10),TimeSpan.FromHours(1));
            var provider=new CoordinadorProveedorIndices(new OperativeProviderPort(),new MySqlProveedorDatosIndices(alias=>{throw new Exception("Unexpected business IO");}));
            var calc=new ServicioPreparacionImportacion(level=>PlanFixture().Contexto,profiles,limits,()=>{},catalog,provider,(c,p,m,r)=>new List<IncidenciaImportacionCarpetas>());
            Calculation=calc;
            Service=new ServicioPreparacionSegmentadaImportacion(()=>Identity,level=>PlanFixture().Contexto,(c,p)=>{if(!Identity.PuedeImportar)throw new PerfilImportacionException("ACCESO_DENEGADO","fixture");},Repo,profiles,calc,limits,()=>Now);
            Start=new IniciarPreparacionPlanSolicitud{nivelRef="3",versionId="5",clavePreparacion=new string('a',32),totalRaices=1,totalArchivos=1000,totalBytes="677000",partesRaices=1,partesArchivos=(uint)(1000/perPart)};
        }
        public RegistrarPartePreparacionSolicitud Root(uint revision){return new RegistrarPartePreparacionSolicitud{preparacionId="77",revisionEsperada=revision,tipo="RAICES",numero=1,raices=new List<RaizManifiestoImportacionSolicitud>{new RaizManifiestoImportacionSolicitud{seleccionId="s1",idCliente="r1",nombre="Egresos"}}};}
        public RegistrarPartePreparacionSolicitud FilePart(int part,int perPart,uint revision){return new RegistrarPartePreparacionSolicitud{preparacionId="77",revisionEsperada=revision,tipo="ARCHIVOS",numero=(uint)part,archivos=Enumerable.Range((part-1)*perPart,perPart).Select(i=>new ArchivoManifiestoImportacionSolicitud{idCliente="a"+i,raizIdCliente="r1",nombre=i+".pdf",rutaRelativa="Egresos/"+i+".pdf",bytes="677",ultimaModificacionLocal="1234"}).ToList()};}
        public RevisionPlanSegmentadoResultado Run(int perPart=100,bool duplicateAcrossParts=false){
            var state=Service.Iniciar(Start);state=Service.Registrar(Root(state.revision));
            state=Service.Validar(new ValidarPartePreparacionSolicitud{preparacionId="77",revisionEsperada=state.revision,tipo="RAICES",numero=1});
            Rejected("CONFLICTO_REVISION",()=>Service.Sellar(new SellarPreparacionPlanSolicitud{preparacionId="77",revisionEsperada=state.revision,claveCierre=Start.clavePreparacion}));
            for(int n=1;n<=1000/perPart;n++){
                var part=FilePart(n,perPart,state.revision);
                if(duplicateAcrossParts&&n==2){part.archivos[0].idCliente="a0";part.archivos[0].nombre="0.pdf";part.archivos[0].rutaRelativa="Egresos/0.pdf";}
                state=Service.Registrar(part);
                state=Service.Validar(new ValidarPartePreparacionSolicitud{preparacionId="77",revisionEsperada=state.revision,tipo="ARCHIVOS",numero=(uint)n});
            }
            return Service.Sellar(new SellarPreparacionPlanSolicitud{preparacionId="77",revisionEsperada=state.revision,claveCierre=Start.clavePreparacion});
        }
    }

    static void SegmentedServiceCases(){
        var f=new SegmentedServiceFixture();var start=f.Service.Iniciar(f.Start);
        var repeated=f.Service.Iniciar(f.Start);Check(start.preparacionId==repeated.preparacionId&&f.Repo.Starts==1,"DOC120 lost start ack creates another draft");
        var request=f.Root(start.revision);var saved=f.Service.Registrar(request);
        Check(f.Service.Registrar(request).revision==saved.revision&&f.Repo.Writes==1,"DOC120 lost part ack duplicates metadata");
        request.raices[0].nombre="Different";Rejected("CONFLICTO_REVISION",()=>f.Service.Registrar(request));
        var before=f.Repo.Writes;var invalid=f.FilePart(1,100,saved.revision);invalid.archivos[0].ultimaModificacionLocal="01";
        Rejected("SOLICITUD_INVALIDA",()=>f.Service.Registrar(invalid));Check(f.Repo.Writes==before,"DOC120 invalid decimal persisted");
        var own=ReceiptClone(f.Identity);
        foreach(var foreign in new Action<ContextoImportacionCarpetas>[] {c=>c.UsuarioRef="99",c=>c.AmbitoRef="99",c=>c.PuedeImportar=false}){
            f.Identity=ReceiptClone(own);foreign(f.Identity);
            var writes=f.Repo.Writes;
            Rejected("ACCESO_DENEGADO",()=>f.Service.Consultar(new ConsultarPreparacionPlanSolicitud{preparacionId="77"}));
            Rejected("ACCESO_DENEGADO",()=>f.Service.Consultar(new ConsultarPreparacionPlanSolicitud{nivelRef="3",clavePreparacion=f.Start.clavePreparacion}));
            Rejected("ACCESO_DENEGADO",()=>f.Service.Registrar(f.Root(saved.revision)));
            Rejected("ACCESO_DENEGADO",()=>f.Service.Validar(new ValidarPartePreparacionSolicitud{preparacionId="77",tipo="RAICES",numero=1,revisionEsperada=saved.revision}));
            Rejected("ACCESO_DENEGADO",()=>f.Service.Sellar(new SellarPreparacionPlanSolicitud{preparacionId="77",revisionEsperada=saved.revision,claveCierre=f.Start.clavePreparacion}));
            Check(f.Repo.Writes==writes&&f.Repo.Seals==0,"DOC120 foreign preparation request changed parts or sealed a plan");
        }
        f=new SegmentedServiceFixture();f.Repo.Available=false;Unavailable(()=>f.Service.Iniciar(f.Start));Check(f.Repo.Starts==0,"DOC120 unavailable schema writes a draft");
        f=new SegmentedServiceFixture();f.Service.Iniciar(f.Start);f.Now=f.Now.AddHours(1);Rejected("PLAN_EXPIRADO",()=>f.Service.Registrar(f.Root(1)));
        f=new SegmentedServiceFixture();f.Service.Iniciar(f.Start);f.Profile.Value.Configuracion.NombreVisible="Changed";f.Profile.Value.Huella=Hex(PerfilImportacionJson.Huella(new VersionPerfilImportacion{Configuracion=f.Profile.Value.Configuracion,Reglas=f.Profile.Value.Reglas}));Rejected("PERFIL_CAMBIADO",()=>f.Service.Registrar(f.Root(1)));
        f=new SegmentedServiceFixture();var result=f.Run();Check(result.puedeConfirmar&&result.totalArchivos==1000&&result.unidadesElegibles==1&&f.Repo.Seals==1,"DOC120 1000 records do not seal one complete plan");
        var legacy=f.Calculation.CalcularPlan(new PrepararPlanImportacionSolicitud{nivelRef="3",versionId="5",raices=f.Root(1).raices,archivos=Enumerable.Range(1,10).SelectMany(n=>f.FilePart(n,100,1).archivos).ToList()});
        var direct=legacy.Resultado.Elementos.ToDictionary(e=>e.Ambito+":"+e.RaizIdCliente+":"+e.ArchivoIdCliente,e=>PerfilImportacionJson.Escribir(e));
        Check(f.Repo.Plan.Resultado.Elementos.All(e=>direct[e.Ambito+":"+e.RaizIdCliente+":"+e.ArchivoIdCliente]==PerfilImportacionJson.Escribir(e))&&direct.Count==f.Repo.Plan.Resultado.Elementos.Count,"DOC120 segmented values differ from legacy calculation");
        Check(legacy.Resultado.UnidadesElegibles.SequenceEqual(f.Repo.Plan.Resultado.UnidadesElegibles)&&legacy.Resultado.Incidencias.Count==f.Repo.Plan.Resultado.Incidencias.Count,"DOC120 segmented eligibility differs from legacy calculation");
        var again=f.Service.Sellar(new SellarPreparacionPlanSolicitud{preparacionId="77",revisionEsperada=1,claveCierre=f.Start.clavePreparacion});Check(again.planId==result.planId&&f.Repo.Seals==1,"DOC120 seal ack lost duplicates plan");
        var seen=new HashSet<string>();int page=1;bool more;
        do{var part=f.Service.Revision(new ConsultarRevisionPlanSolicitud{planId="11",revision=result.revision,tipo="VALORES",ambito="ARCHIVO",pagina=page++});more=part.puedeHaberMas;Check(part.valores.Count<=25&&PerfilImportacionJson.Escribir(part).Length<=65536,"DOC120 revision response exceeds count/size");foreach(var row in part.valores)Check(seen.Add(row.ArchivoIdCliente),"DOC120 review pagination repeats an item");}while(more);
        Check(seen.Count==1000,"DOC120 review pagination skips files");
        f.Identity.UsuarioRef="99";Rejected("ACCESO_DENEGADO",()=>f.Service.Revision(new ConsultarRevisionPlanSolicitud{planId="11",revision=result.revision}));
        var partitioned=new SegmentedServiceFixture(50);var other=partitioned.Run(50);Check(other.planHash==result.planHash,"DOC120 metadata boundaries change plan contents/hash");
        var duplicates=new SegmentedServiceFixture();Rejected("SOLICITUD_INVALIDA",()=>duplicates.Run(100,true));Check(duplicates.Repo.Seals==0,"DOC120 cross-part duplicate inventory seals");
        Console.WriteLine("DOC-120 segmented preparation service: 1000 metadata, idempotency, expiry, privacy, pagination and global integrity passed (memory port; no SQL/HTTP)");
    }
    static PlanImportacionCarpetas LargeSegmentedPlan() {
        var p=PlanFixture();p.EsquemaDoc=2;p.PreparacionId="77";
        var root=p.Manifiesto.Raices.First();p.Manifiesto.Raices=new List<RaizManifiestoImportacion>{root};
        p.Manifiesto.Archivos.Clear();p.Resultado.Elementos.Clear();p.Resultado.Incidencias.Clear();
        p.Resultado.UnidadesElegibles=new List<string>{root.IdCliente};
        for(int n=0;n<1000;n++) {
            var id="a"+n;
            p.Manifiesto.Archivos.Add(new ArchivoManifiestoImportacion{IdCliente=id,RaizIdCliente=root.IdCliente,Nombre=n+".pdf",RutaRelativa=root.Nombre+"/"+n+".pdf",Bytes="677",UltimaModificacionLocal="1234567890"});
            p.Resultado.Elementos.Add(new ValoresElementoImportacion{Ambito="ARCHIVO",RaizIdCliente=root.IdCliente,ArchivoIdCliente=id,
                Valores=new List<ValorReglaPerfilImportacion>{new ValorReglaPerfilImportacion{DestinoTipo="INDICE_GABINETE",DestinoRef="9",Valor=new string('\u00f1',200),TipoValor="TEXTO"}}});
        }
        return p;
    }
    static void SegmentedPlanCases() {
        var source=LargeSegmentedPlan();
        Rejected("SOLICITUD_INVALIDA",()=>PerfilImportacionJson.Escribir(source));
        var p=Prepare(source);
        Check(p.EsquemaDoc==2 && p.PreparacionId=="77" && p.Manifiesto.Archivos.Count==1000,"DOC120 plan2 clone loses identity/inventory");
        Check(!Object.ReferenceEquals(p.Manifiesto.Archivos[0],source.Manifiesto.Archivos[0]),"DOC120 plan2 shares mutable input");
        var parts=PlanSegmentadoImportacionJson.Fragmentar(p);
        Check(PlanSegmentadoImportacionJson.DocumentoRecuperacion(p).Length<1024,"DOC120 recovery serializes the aggregate plan2");
        var recoveryFingerprint=PlanSegmentadoImportacionJson.DocumentoRecuperacion(p);
        var recoveryChanged=PlanSegmentadoImportacionJson.ClonarPlan(p);recoveryChanged.Manifiesto.Archivos[0].Bytes="678";
        Check(PlanSegmentadoImportacionJson.DocumentoRecuperacion(recoveryChanged)!=recoveryFingerprint,"DOC120 recovery ignores changed immutable inventory");
        var barrier=new VistaBarreraCreacionImportacion{Plan=p,Lote=new LoteImportacionCarpetas(),Raiz=new RaizImportacionCarpetas(),Raices=new List<RaizImportacionCarpetas>(),Archivos=Enumerable.Range(0,1000).Select(i=>new ArchivoImportacionCarpetas{ArchivoId=(i+1).ToString(),ValoresPlan=p.Resultado.Elementos[i].Valores}).ToList()};
        var barrierCopy=PlanSegmentadoImportacionJson.ClonarBarrera(barrier);Check(barrierCopy.Archivos.Count==1000&&!Object.ReferenceEquals(barrierCopy.Archivos[0],barrier.Archivos[0]),"DOC120 permanent resume clones aggregate JSON or shares mutable metadata");
        var reconciliation=PlanSegmentadoImportacionJson.ClonarReconciliacion(new VistaReconciliacionImportacion{Plan=p,Lote=barrier.Lote,Raiz=barrier.Raiz,Archivo=barrier.Archivos[0],Intento=new IntentoImportacionCarpetas()});
        Check(reconciliation.Plan.Manifiesto.Archivos.Count==1000,"DOC120 reconciliation cannot clone plan2");
        Check(parts.Count>20 && parts.All(f=>f.Contenido.Length<=262144),"DOC120 aggregate result overflows a stored fragment");
        var copy=new PlanImportacionCarpetas{EsquemaDoc=2,Contexto=p.Contexto,Snapshot=p.Snapshot,CreadoUtc=p.CreadoUtc,ExpiraUtc=p.ExpiraUtc};
        PlanSegmentadoImportacionJson.Reconstruir(copy,parts.AsEnumerable().Reverse().ToList());
        Check(copy.Manifiesto.Archivos.Count==1000 && copy.Resultado.Elementos.Count==1000,"DOC120 fragment reconstruction loses rows");
        Check(Hex(PlanSegmentadoImportacionJson.HuellaPlan(copy))==p.Huella,"DOC120 fragment order changes sealed hash");
        var repartitioned=parts.Where(f=>f.Tipo!="ARCHIVOS").ToList();
        for(int i=0;i<20;i++) {
            var doc=PerfilImportacionJson.Escribir(p.Manifiesto.Archivos.Skip(i*50).Take(50).ToList());
            repartitioned.Add(new FragmentoPlanImportacion{Tipo="ARCHIVOS",Numero=(uint)i+1,Contenido=doc,Huella=PlanSegmentadoImportacionJson.HuellaTexto(doc)});
        }
        PlanSegmentadoImportacionJson.Reconstruir(copy,repartitioned);
        Check(Hex(PlanSegmentadoImportacionJson.HuellaPlan(copy))==p.Huella,"DOC120 storage boundaries change logical hash");
        var reversed=PlanSegmentadoImportacionJson.ClonarPlan(p);reversed.PlanId=null;reversed.Manifiesto.Archivos.Reverse();reversed.Resultado.Elementos.Reverse();
        Check(Prepare(reversed).Huella==p.Huella,"DOC120 canonical preparation depends on input order");
        foreach(var mode in new[]{"missing","duplicate","hash","shape","unknown","too-many"}) {
            var broken=parts.Select(f=>new FragmentoPlanImportacion{Tipo=f.Tipo,Numero=f.Numero,Contenido=f.Contenido,Huella=f.Huella}).ToList();
            var first=broken.First(f=>f.Tipo=="ARCHIVOS");
            if(mode=="missing")broken.Remove(first);
            if(mode=="duplicate")broken.Add(first);
            if(mode=="hash")first.Huella=new string('0',64);
            if(mode=="unknown")first.Tipo="UNKNOWN";
            if(mode=="shape"){first.Contenido="[{\"Bytes\":7}]";first.Huella=PlanSegmentadoImportacionJson.HuellaTexto(first.Contenido);}
            if(mode=="too-many"){first.Contenido=PerfilImportacionJson.Escribir(p.Manifiesto.Archivos.Take(101).ToList());first.Huella=PlanSegmentadoImportacionJson.HuellaTexto(first.Contenido);}
            try{PlanSegmentadoImportacionJson.Reconstruir(copy,broken);throw new Exception("DOC120 malformed fragments accepted: "+mode);}
            catch(PerfilImportacionException){}
        }
        var limits=new LimitesImportacionCarpetas(10,2000,1000,1000000,786432,TimeSpan.FromMinutes(10),TimeSpan.FromHours(1));
        var inventory=CanonizadorManifiestoImportacion.Canonizar(p.Manifiesto,limits,2);
        Check(inventory.Archivos.Count==1000,"DOC120 plan2 manifest capacity lost");
        var unicode=PlanSegmentadoImportacionJson.ClonarPlan(p);
        unicode.Manifiesto.Archivos.ForEach(f=>{f.Nombre=new string('\u00f1',50)+f.Nombre;f.RutaRelativa=unicode.Manifiesto.Raices[0].Nombre+"/"+f.Nombre;});
        Rejected("SOLICITUD_INVALIDA",()=>CanonizadorManifiestoImportacion.Canonizar(unicode.Manifiesto,limits));
        Check(CanonizadorManifiestoImportacion.Canonizar(unicode.Manifiesto,limits,2).Archivos.Count==1000,"DOC120 bounded record cloning still serializes the aggregate");
        Rejected("LIMITE_EXCEDIDO",()=>CanonizadorManifiestoImportacion.Canonizar(unicode.Manifiesto,new LimitesImportacionCarpetas(10,2000,1000,1000000,1000,TimeSpan.FromMinutes(10),TimeSpan.FromHours(1)),2));
        var tooLarge=LargeSegmentedPlan();tooLarge.Resultado.Elementos[0].Valores[0].Valor=new string('x',262144);
        Rejected("SOLICITUD_INVALIDA",()=>PlanSegmentadoImportacionJson.Fragmentar(tooLarge));
        Console.WriteLine("DOC-120 plan2: 1000 files, bounded fragments, canonical hash, corruption and legacy limits passed");
    }
    static void SegmentedSchemaCases() {
        // Fixture sintetico derivado del DDL08; no captura o DDL real.
        var data=JsonConvert.DeserializeObject<DataSet>(File.ReadAllText("doc120-preparacion-esquema-sintetico.json"));
        Action<DataSet> validate=s=>RepositoryHelper("ValidarMetadatosPreparacion",s);
        validate(data);
        Action<Action<DataSet>> reject=change=>{var broken=data.Copy();change(broken);Unavailable(()=>validate(broken));};
        reject(s=>s.Tables["Tablas"].Rows[0]["ENGINE"]="MyISAM");
        reject(s=>s.Tables["Tablas"].Rows.RemoveAt(0));
        reject(s=>SchemaRow(s,"Columnas","COLUMN_NAME","cabecera_hash")["CHARACTER_MAXIMUM_LENGTH"]=16);
        reject(s=>SchemaRow(s,"Columnas","COLUMN_NAME","plan_id")["IS_NULLABLE"]="NO");
        reject(s=>SchemaRow(s,"Columnas","COLUMN_NAME","contenido_doc")["COLLATION_NAME"]="utf8_general_ci");
        reject(s=>SchemaRow(s,"Indices","INDEX_NAME","uq_preparacion_clave")["NON_UNIQUE"]=1);
        reject(s=>SchemaRow(s,"Indices","INDEX_NAME","uq_preparacion_clave")["SUB_PART"]=4);
        reject(s=>SchemaRow(s,"Claves","CONSTRAINT_NAME","fk_preparacion_plan")["MISMO_ESQUEMA"]=0);
        reject(s=>SchemaRow(s,"Reglas","CONSTRAINT_NAME","fk_preparacion_plan")["DELETE_RULE"]="CASCADE");
        reject(s=>{var extra=s.Tables["Indices"].NewRow();extra.ItemArray=s.Tables["Indices"].Rows[0].ItemArray;extra["INDEX_NAME"]="unsupported_unique";s.Tables["Indices"].Rows.Add(extra);});
        validate(data);
        Console.WriteLine("DOC-120 additive preparation schema rejects incompatible metadata without requiring it for legacy plans: passed");
    }

    static void SegmentedCatalogCacheCases(){
        var fixture=new ResourceFixture();var context=fixture.Adapter.ResolverDestino(fixture.Http,"7");
        var catalog=new PerfilImportacionCatalogAdapter("RA","DA",fixture.Query);
        var snapshot=new SnapshotPerfilImportacionOperativa{Huella=new string('a',64),Configuracion=fixture.Configuration()};
        catalog.IniciarLecturasPreparacion(context,snapshot);
        Func<List<ValorReglaPerfilImportacion>> values=()=>new List<ValorReglaPerfilImportacion>{new ValorReglaPerfilImportacion{DestinoTipo="INDICE_GABINETE",DestinoRef="99",FuenteTipo="ARCHIVO",TipoValor="TEXTO",Valor="000123"}};
        Check(catalog.ValidarValoresOperativos(context,snapshot.Configuracion,values()).Count==0,"DOC120 cached catalog rejects valid index");var first=fixture.Reads;
        for(int n=0;n<100;n++)Check(catalog.ValidarValoresOperativos(context,snapshot.Configuracion,values()).Count==0,"DOC120 cached catalog changes values");
        Check(fixture.Reads==first,"DOC120 catalog metadata queried once per file");
        var stamp=catalog.SelloLecturasPreparacion(context,snapshot);Check(fixture.Reads>first,"DOC120 final catalog fence reused cached authority");
        snapshot.Huella=new string('b',64);catalog.IniciarLecturasPreparacion(context,snapshot);var before=fixture.Reads;catalog.ValidarValoresOperativos(context,snapshot.Configuracion,values());Check(fixture.Reads>before,"DOC120 request/snapshot cache survived its scope");
        fixture.CabinetAllowed=false;var denied=false;
        try{denied=catalog.SelloLecturasPreparacion(context,snapshot)!=stamp;}catch(PerfilImportacionException){denied=true;}
        Check(denied,"DOC120 changed authority did not invalidate final catalog fence");
        Console.WriteLine("DOC-120 preparation catalog cache: bounded request scope, fresh fences and no repeated metadata per file passed");
    }

    static void FocalVersionCases(){
        var lot=new LoteImportacionCarpetas{LoteId="34",TotalRaices=1,TotalArchivos=1,TotalBytes="677"};
        var data=new DataTable();foreach(var column in new[]{"tipo","recurso","raiz","cliente","revision","estado","recepcion","confirmado","datos","bytes"})data.Columns.Add(column,typeof(object));
        data.Rows.Add("R",52,52,"r1",1,"EXPEDIENTE_CONFIRMADO","",1,0,0);
        data.Rows.Add("A",78,52,"a1",2,"NO_INICIADO","RECIBIDO",0,0,677);
        var view=new VistaConsultaFocalImportacion();RepositoryHelper("ProyectarVersionesFocal",lot,view,data);
        var expected=PlanSegmentadoImportacionJson.HuellaTexto("DOC120_RECURSOS_1\n34\nA:78:52:a1:2\nR:52:r1:1\n");
        Check(view.HuellaRecursos==expected&&view.Progreso.recibidos==1&&view.Progreso.expedientes==1&&view.Progreso.guardados==0&&view.Progreso.bytesRecibidos=="677","DOC120 focal versions differ from client protocol or confirmed counters");
        var original=view.HuellaRecursos;data.Rows[1]["revision"]=3;RepositoryHelper("ProyectarVersionesFocal",lot,view,data);Check(view.HuellaRecursos!=original,"DOC120 other-resource revision absent from digest");
        data.Rows[1]["raiz"]=99;Unavailable(()=>RepositoryHelper("ProyectarVersionesFocal",lot,view,data));data.Rows[1]["raiz"]=52;
        data.Rows[1]["bytes"]=678;Unavailable(()=>RepositoryHelper("ProyectarVersionesFocal",lot,view,data));data.Rows[1]["bytes"]=677;
        data.Rows[1]["estado"]="UNKNOWN";Unavailable(()=>RepositoryHelper("ProyectarVersionesFocal",lot,view,data));
        Console.WriteLine("DOC-120 focal query: coherent version digest, scoped parent and confirmed counters passed (SQL-result doubles)");
    }
}
