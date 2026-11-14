using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;

public static partial class ImportacionCarpetasBehaviorTests {
    static ConfiguracionPerfilImportacionException MessageFailure(Action action,string code,string cause) {
        try { action(); } catch(ConfiguracionPerfilImportacionException error) {
            Check(error.Codigo==code&&error.Incidencias.Any(i=>i.Codigo==cause),"DOC120 wrong diagnostic cause "+cause);
            return error;
        }
        throw new Exception("DOC120 expected diagnostic rejection "+cause);
    }
    static void MessageDiagnosticCases() {
        var resolver=new ResolvedorReglasPerfilImportacion();
        var rule=new ReglaPerfilImportacion{DestinoTipo="CAMPO_EXPEDIENTE",DestinoRef="NOMBRE",FuenteTipo="CARPETA",TipoValor="TEXTO",Extraccion="SEGMENTO",Separador="_",Posicion=2,LongitudMaxima=8};
        var result=resolver.Resolver(new List<ReglaPerfilImportacion>{rule},new EjemploPerfilImportacion{NombreCarpeta="A_B"});
        Check(result.Incidencias.Single().Codigo=="SEGMENTO_AUSENTE"&&result.Incidencias.Single().Mensaje.Contains("Segmento requerido: 3 (desde 1)"),"DOC120 lost required segment position");
        rule.Extraccion="COMPLETO";
        result=resolver.Resolver(new List<ReglaPerfilImportacion>{rule},new EjemploPerfilImportacion{NombreCarpeta="1234567890"});
        Check(result.Incidencias.Single().Mensaje.Contains("Longitud observada: 10. Máximo permitido: 8."),"DOC120 lost observed length/limit");
        rule.DestinoRef="FECHA_INICIAL";rule.TipoValor="FECHA";rule.LongitudMaxima=10;rule.FormatoFecha="yyyyMMdd";
        result=resolver.Resolver(new List<ReglaPerfilImportacion>{rule},new EjemploPerfilImportacion{NombreCarpeta="20260230"});
        Check(result.Incidencias.Single().Codigo=="FECHA_INVALIDA"&&result.Incidencias.Single().Mensaje.Contains("Formato requerido: yyyyMMdd."),"DOC120 lost actual input date format");
        foreach(var bad in new[]{"",new string('x',161),"not-a-date"}) {
            var f=new CreationCommandFixture();var index=bad=="not-a-date"?2:1;f.Root.ValoresPlan[index].Valor=bad;
            var before=PerfilImportacionJson.Escribir(new{f.Lot,f.Root,f.Attempt});
            var error=MessageFailure(()=>f.Prepare(),"CONFIGURACION_INVALIDA",bad==""?"VALOR_REQUERIDO":bad=="not-a-date"?"FECHA_INVALIDA":"LONGITUD_EXCEDIDA");
            Check(error.Incidencias.Single().Campo==f.Root.ValoresPlan[index].DestinoRef&&PerfilImportacionJson.Escribir(new{f.Lot,f.Root,f.Attempt})==before,"DOC120 diagnostic mutates command or loses rejected field");
        }
        using(var f=new DocumentAdapterFixture()) {
            f.FileRecord.ValoresEfectivosFijados=false;
            MessageFailure(()=>ComandoAlmacenamientoCarpetas.Preparar(f.Lot.Contexto,f.Root,f.FileRecord,f.Lot.Snapshot,f.Attempt),"CONFIGURACION_INVALIDA","INDICES_PENDIENTES");
            Check(f.Starts==0&&f.Writers==0,"DOC120 storage diagnostic starts writer");
        }
        using(var f=new ReceiptFixture()) using(var input=new StagingStream(f.Pdf.Take(f.Pdf.Length-1).ToArray())) {
            MessageFailure(()=>f.Service.Recibir(f.Request,input),"ARCHIVO_RECHAZADO","BYTES_NO_COINCIDEN");
            Check(f.Repository.Commits==0&&f.Completed().Length==0,"DOC120 rejected receipt confirmed content");
        }
        using(var f=new ReceiptFixture()) {
            var invalid=(byte[])f.Pdf.Clone();invalid[0]=0;
            using(var input=new StagingStream(invalid)) MessageFailure(()=>f.Service.Recibir(f.Request,input),"ARCHIVO_RECHAZADO","PDF_FIRMA_INVALIDA");
            Check(f.Repository.Commits==0,"DOC120 bad PDF accepted");
        }
        foreach(var code in new[]{"CONFIGURACION_INVALIDA","ACCESO_DENEGADO","SESION_REQUERIDA"}) {
            var error=new ConfiguracionPerfilImportacionException(new List<IncidenciaPerfilImportacion>{new IncidenciaPerfilImportacion("PRIVATE_SQL_PASSWORD", "LONGITUD_EXCEDIDA","PRIVATE_SQL_PASSWORD")},code);
            var response=new RespuestaPerfilImportacion();DiagnosticoOperacionImportacion.Publicar(response,error);
            var text=PerfilImportacionJson.Escribir(response);
            Check(!text.Contains("PRIVATE_SQL_PASSWORD")&&response.codigo==code,"DOC120 public diagnostic leak");
            Check(response.incidencias.Count==(code=="CONFIGURACION_INVALIDA"?1:0),"DOC120 denied response leaks detail");
        }
        var reference=new string('a',32);var previous=HttpContext.Current;var log=new StringWriter();var listener=new TextWriterTraceListener(log);
        try {
            HttpContext.Current=null;Trace.Listeners.Add(listener);
            DiagnosticoOperacionImportacion.Registrar(new IOException("PRIVATE_SQL_PASSWORD"),"RECEPCION",reference);
            Check(log.ToString().Contains("operacion="+reference)&&log.ToString().Contains("fase=RECEPCION")&&!log.ToString().Contains("PRIVATE_SQL_PASSWORD"),"DOC120 missing or unsafe diagnostic trace");
        } finally {Trace.Listeners.Remove(listener);listener.Dispose();HttpContext.Current=previous;}
        Console.WriteLine("DOC-120 contextual diagnostics: rules, creation, receipt, storage, privacy and trace listener passed");
    }
}
