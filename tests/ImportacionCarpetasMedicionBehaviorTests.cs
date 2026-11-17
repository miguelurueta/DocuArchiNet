using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

public static partial class ImportacionCarpetasBehaviorTests {
    sealed class TimingListener : TraceListener {
        public readonly List<string> Lines=new List<string>();public bool Fail;
        public override void Write(string s){}public override void WriteLine(string s){}
        public override void TraceEvent(TraceEventCache cache,string source,TraceEventType type,int id,string message){if(Fail)throw new Exception("synthetic private sink");Lines.Add(message);}
    }
    sealed class CountedStaging : IAlmacenStagingImportacion,IRetencionVerificadaImportacion {
        readonly ArchivoImportacionCarpetas file;readonly Action verify;
        public CountedStaging(ArchivoImportacionCarpetas f,Action v){file=f;verify=v;}
        public RecepcionVerificadaImportacion Recepcion {get{return new RecepcionVerificadaImportacion{StagingRef=file.StagingRef,BytesRecibidos=file.BytesRecibidos,ContenidoHash=file.ContenidoHash};}}
        public RecepcionVerificadaImportacion VerificarRetenido(ContextoImportacionCarpetas c){verify();return Recepcion;}
        public IRetencionVerificadaImportacion ConservarVerificado(ContextoImportacionCarpetas c){verify();return this;}
        public void Dispose(){}
        public RecepcionVerificadaImportacion Recibir(ContextoImportacionCarpetas c,IntentoImportacionCarpetas i,Stream s,string b){throw new Exception("Unexpected receipt");}
        public RecepcionVerificadaImportacion Verificar(ContextoImportacionCarpetas c,IntentoImportacionCarpetas i){throw new Exception("Unexpected verification");}
        public RecepcionVerificadaImportacion VerificarReenvio(ContextoImportacionCarpetas c,IntentoImportacionCarpetas i,Stream s){throw new Exception("Unexpected resend");}
        public string CrearCopiaTrabajo(ContextoImportacionCarpetas c,string r,string n){throw new Exception("Unexpected copy");}
    }
    static void MedicionAlmacenamientoCases(){
        var listener=new TimingListener();Trace.Listeners.Add(listener);
        try {
            using(var f=new DocumentPhaseFixture()){f.Run();Check(listener.Lines.Any(l=>l.Contains("etapa=ORIGINALES_PREVIOS"))&&listener.Lines.Any(l=>l.Contains("etapa=ORIGINALES_RETENIDOS"))&&listener.Lines.Any(l=>l.Contains("etapa=TOTAL")),"Timing stages not observed");}
            var count=listener.Lines.Count;MedicionAlmacenamientoImportacion.Medir("PRIVATE_SQL", "20","30",()=>true);MedicionAlmacenamientoImportacion.Medir("TOTAL", "PRIVATE_CONNECTION","30",()=>true);Check(listener.Lines.Count==count,"Timing accepts private labels/references");
            listener.Fail=true;Check(MedicionAlmacenamientoImportacion.Medir("TOTAL","20","30",()=>42)==42,"Broken sink changes success");
            bool original=false;try{MedicionAlmacenamientoImportacion.Medir<int>("TOTAL","20","30",()=>{throw new IOException("synthetic original");});}catch(IOException){original=true;}Check(original,"Broken sink replaces original exception");listener.Fail=false;
            Check(listener.Lines.All(l=>!l.Contains("PRIVATE")&&!l.Contains(".pdf")&&!l.Contains("staging")&&!l.Contains("Password")),"Timing leaked business content");
        }finally{Trace.Listeners.Remove(listener);listener.Dispose();}
        var rows=new List<object>();
        foreach(var n in new[]{1,100,1000})foreach(var policy in new[]{"EXPEDIENTE","LOTE","NO_APLICA"})using(var f=new DocumentPhaseFixture()) {
            var view=f.View();view.Lote.Snapshot.Configuracion.ModoIntegridad=policy=="NO_APLICA"?"PARCIAL_PERMITIDO":"COMPLETO_OBLIGATORIO";view.Lote.Snapshot.Configuracion.AmbitoIntegridad=policy;
            view.Lote.Snapshot.Huella=Hex(PerfilImportacionJson.Huella(new VersionPerfilImportacion{Configuracion=view.Lote.Snapshot.Configuracion,Reglas=view.Lote.Snapshot.Reglas}));
            view.Plan.Snapshot=ReceiptClone(view.Lote.Snapshot);view.Archivos.Clear();view.Plan.Manifiesto.Archivos.Clear();
            for(int i=0;i<n;i++){var file=ReceiptClone(f.Source.FileRecord);file.ArchivoId=(30+i).ToString();file.ClienteArchivoId="f"+i;file.NombreOriginal=i+".pdf";file.RutaRelativa="Folder/"+file.NombreOriginal;view.Archivos.Add(file);view.Plan.Manifiesto.Archivos.Add(new ArchivoManifiestoImportacion{IdCliente=file.ClienteArchivoId,RaizIdCliente="r1",Nombre=file.NombreOriginal,RutaRelativa=file.RutaRelativa,Bytes=file.BytesPrevistos});}
            view.Lote.TotalArchivos=(uint)n;view.Lote.TotalBytes=(UInt64.Parse(f.Source.FileRecord.BytesPrevistos)*(ulong)n).ToString();
            int loads=0,checks=0,authorities=0;ulong representedBytes=0;
            var byId=view.Archivos.ToDictionary(a=>a.ArchivoId);
            Func<ContextoImportacionCarpetas,string,string,ArchivoImportacionCarpetas> load=(c,l,id)=>{loads++;return byId[id];};
            Func<LoteImportacionCarpetas,ArchivoImportacionCarpetas,IAlmacenStagingImportacion> stage=(l,file)=>new CountedStaging(file,()=>{checks++;representedBytes+=UInt64.Parse(file.BytesRecibidos);});
            Action<ContextoImportacionCarpetas,ConfiguracionPerfilImportacion> authorize=(c,s)=>authorities++;
            var method=typeof(CoordinadorImportacionCarpetas).GetMethod("VerificarStagingCreacion",BindingFlags.Static|BindingFlags.NonPublic);var guards=new List<IRetencionVerificadaImportacion>();var clock=Stopwatch.StartNew();
            foreach(bool retained in new[]{false,true})try{method.Invoke(null,new object[]{view,load,stage,authorize,retained?guards:null,view.Archivos[0],null});}catch(TargetInvocationException e){throw e.InnerException;}
            clock.Stop();foreach(var guard in guards)guard.Dispose();int expected=policy=="NO_APLICA"?2:2*n;
            Check(loads==expected&&checks==expected&&representedBytes==(ulong)expected*UInt64.Parse(f.Source.FileRecord.BytesPrevistos),"Unexpected storage barrier cost");
            rows.Add(new{files=n,policy,metadataLoads=loads,stagingPortChecks=checks,representedBytes,authorities,localMilliseconds=clock.Elapsed.TotalMilliseconds,physicalBytesMeasured=0,realSelect=0,realWriter=0});
        }
        var output=Environment.GetEnvironmentVariable("DOC120_BASELINE_FILE");if(!String.IsNullOrEmpty(output))File.WriteAllText(output,PerfilImportacionJson.Escribir(rows));
        Console.WriteLine("DOC-120 storage timing and 1/100/1000 barrier baseline: passed; synthetic ports, no SQL or physical throughput claim");
    }
}
