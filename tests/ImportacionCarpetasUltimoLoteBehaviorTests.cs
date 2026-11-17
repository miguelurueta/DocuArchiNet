using System;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

public static partial class ImportacionCarpetasBehaviorTests {
    sealed partial class QueryRepository : IConsultaUltimoLoteImportacionRepository {
        public string LastId="20";public int LastReads;
        public string ObtenerUltimoLoteId(ContextoImportacionCarpetas identity){Owner(identity,Lot.LoteId);LastReads++;return LastId;}
    }
    static void UltimoLoteCases(){
        var repository=new QueryRepository();var identity=ReceiptClone(repository.Lot.Contexto);var destination=ReceiptClone(identity);bool allowed=true;
        var service=new ServicioConsultaImportacion(()=>identity,repository,n=>destination,(c,s)=>{if(!allowed)throw new PerfilImportacionException("ACCESO_DENEGADO","fixture");},25);
        var request=new ConsultarUltimoLoteImportacionSolicitud();var before=PerfilImportacionJson.Escribir(repository.Lot);
        var found=service.ConsultarUltimo(request);Check(found.encontrado&&found.loteId=="20"&&repository.LastReads==1&&before==PerfilImportacionJson.Escribir(repository.Lot),"Latest lookup changed lot or did not validate the candidate");
        repository.LastId=null;found=service.ConsultarUltimo(request);Check(!found.encontrado&&found.loteId==null,"Empty lookup exposes a reference");repository.LastId="20";
        allowed=false;Rejected("ACCESO_DENEGADO",()=>service.ConsultarUltimo(request));allowed=true;
        destination.SelloDestino="changed";Rejected("ACCESO_DENEGADO",()=>service.ConsultarUltimo(request));destination=ReceiptClone(repository.Lot.Contexto);
        repository.LastId="99";Rejected("ACCESO_DENEGADO",()=>service.ConsultarUltimo(request));repository.LastId="20";
        identity.UsuarioRef="99";Rejected("ACCESO_DENEGADO",()=>service.ConsultarUltimo(request));identity=ReceiptClone(repository.Lot.Contexto);
        identity.AmbitoRef="99";Rejected("ACCESO_DENEGADO",()=>service.ConsultarUltimo(request));identity=ReceiptClone(repository.Lot.Contexto);
        identity.PuedeImportar=false;var reads=repository.LastReads;Rejected("ACCESO_DENEGADO",()=>service.ConsultarUltimo(request));Check(reads==repository.LastReads,"Revoked identity reached discovery");identity=ReceiptClone(repository.Lot.Contexto);
        var rows=new DataTable();rows.Columns.Add("lote_id",typeof(object));rows.Rows.Add("20");int sqlReads=0;
        Func<string,object[],DataTable> query=(sql,args)=>{sqlReads++;Check(sql=="SELECT lote_id FROM ra_integracion_lote WHERE solicitante_ref=@usuario AND ambito_ref=@ambito ORDER BY lote_id DESC LIMIT 1","Latest SQL lacks owner/company boundary, deterministic order or limit");Check(args.Length==4&&Convert.ToString(args[1])==identity.UsuarioRef&&Convert.ToString(args[3])==identity.AmbitoRef,"Latest SQL trusts external authority");return rows;};
        Check((string)RepositoryHelper("LeerUltimoLoteId",identity,query)=="20"&&sqlReads==1,"Latest SQL result missing");rows.Clear();Check(RepositoryHelper("LeerUltimoLoteId",identity,query)==null,"Empty SQL invents latest ID");
        identity.UsuarioRef="1 OR 1=1";Rejected("ACCESO_DENEGADO",()=>RepositoryHelper("LeerUltimoLoteId",identity,query));Check(sqlReads==2,"Invalid identity reached SELECT");identity=ReceiptClone(repository.Lot.Contexto);
        rows.Rows.Add("PRIVATE_SQL");Rejected("CAPACIDAD_NO_DISPONIBLE",()=>RepositoryHelper("LeerUltimoLoteId",identity,query));
        var resources=new ResourceFixture();int factories=0;var constructor=typeof(WebServiceImportacionCarpetas).GetConstructors(BindingFlags.NonPublic|BindingFlags.Instance).Single();
        Func<HttpContext,ImportacionCarpetasContextAdapter,ServicioConsultaImportacion> compose=(h,a)=>{factories++;return service;};
        var endpoint=(WebServiceImportacionCarpetas)constructor.Invoke(new object[]{resources.Adapter,(Func<LimitesImportacionCarpetas>)(()=>RequestLimits()),null,null,compose,null});
        var previous=HttpContext.Current;
        Func<string,bool,RespuestaPerfilImportacion> invoke=(body,token)=>{var worker=new ImportRequest{Body=System.Text.Encoding.UTF8.GetBytes(body)};var http=SessionFixture(worker:worker);worker.Token=ImportacionCarpetasRequestGuard.Emitir(http,RequestContext());if(!token)worker.Token=null;HttpContext.Current=http;return endpoint.ConsultarUltimoLote(new ConsultarUltimoLoteImportacionSolicitud());};
        try {
            Check(invoke("{\"solicitud\":{}}",false).codigo=="SOLICITUD_NO_VERIFICADA"&&factories==0,"Latest bypassed CSRF");
            Check(!invoke("{\"solicitud\":{\"usuarioRef\":\"99\"}}",true).ok&&factories==0,"Latest accepted supplied owner");
            var result=invoke("{\"solicitud\":{}}",true);Check(result.ok&&((ConsultaUltimoLoteImportacionResultado)result.datos).loteId=="20"&&factories==1,"Latest endpoint envelope or composition failed");
        }finally{HttpContext.Current=previous;}
        Console.WriteLine("DOC-120 last lot discovery: SELECT scope, current authority, empty result and guarded ASMX passed (synthetic ports)");
    }
}
