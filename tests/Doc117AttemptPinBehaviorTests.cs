using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using Newtonsoft.Json;

// Current product command/coordinator with the existing local PDF and SQL doubles.
// No HTTP or database. The laboratory helper is invoked by reflection (Friend).
public static class Doc117AttemptPinBehaviorTests {
    static void Check(bool ok,string code){if(!ok)throw new Exception(code);}
    static readonly Type Owner=typeof(ImportacionCarpetasBehaviorTests);
    static object Fixture(string name){return Activator.CreateInstance(Owner.GetNestedType(name,BindingFlags.NonPublic),true);}
    static T Field<T>(object o,string name){return (T)o.GetType().GetField(name).GetValue(o);}
    static object Call(object o,string name,params object[] args){try{return o.GetType().GetMethod(name).Invoke(o,args);}catch(TargetInvocationException e){throw e.InnerException;}}
    static T Clone<T>(T value){return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(value));}
    static readonly Type Helper=typeof(Doc117SolicitudHash).Assembly.GetType("Doc117InternalDocumentCandidate");
    static object Lab(string method,params object[] args){try{return Helper.GetMethod(method,BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,args);}catch(TargetInvocationException e){throw e.InnerException;}}
    static void Reject(Action action,string code){try{action();}catch(InvalidOperationException e){Check(e.Message==code,"WRONG_REJECTION");return;}throw new Exception("MISSING_REJECTION");}
    static void Constant(string name,string expected){Check((string)Helper.GetField(name,BindingFlags.NonPublic|BindingFlags.Static).GetRawConstantValue()==expected,"SCOPE_FIELD_MISMATCH_"+name);}
    static object Source(bool b){
        var f=Fixture("DocumentAdapterFixture");var lot=Field<LoteImportacionCarpetas>(f,"Lot");var root=Field<RaizImportacionCarpetas>(f,"Root");var file=Field<ArchivoImportacionCarpetas>(f,"FileRecord");var attempt=Field<IntentoImportacionCarpetas>(f,"Attempt");
        lot.LoteId="900";lot.Contexto.UsuarioRef="141";lot.Contexto.AmbitoRef="2";lot.Contexto.NivelRef="7";
        lot.Snapshot.VersionId="38";lot.Snapshot.Configuracion.GabineteId=10;lot.Snapshot.Configuracion.OrigenGabinete="DA";Call(f,"Rehash");
        root.LoteId="900";root.RaizId=b?"904":"901";root.IdentidadExpediente.NivelRef="7";root.IdentidadExpediente.ExpedienteId=b?"912":"910";root.IdentidadExpediente.RelacionNivelId=b?"913":"911";
        file.RaizId=root.RaizId;file.ArchivoId=b?"905":"902";file.IntentoActivoId=b?"906":"903";
        // Stable, existing PDF fixture metadata; this command test does not run
        // the fixture's writer/staging. HashParity separately runs the coordinator.
        var fixturePdf=System.IO.File.ReadAllBytes(Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar)).Parent.FullName,"tools/e2e/fixtures/doc111-carga-individual.pdf"));
        file.BytesPrevistos=fixturePdf.Length.ToString(System.Globalization.CultureInfo.InvariantCulture);file.BytesRecibidos=file.BytesPrevistos;
        using(var hash=SHA256.Create())file.ContenidoHash=BitConverter.ToString(hash.ComputeHash(fixturePdf)).Replace("-","").ToLowerInvariant();
        attempt.LoteId=lot.LoteId;attempt.RaizId=root.RaizId;attempt.ArchivoId=file.ArchivoId;attempt.IntentoId=file.IntentoActivoId;
        attempt.SolicitudHash=Doc117SolicitudHash.Calcular(lot.Contexto,lot.LoteId,file.ArchivoId,file.Revision-1,b?"closed-b":"closed-a","ALMACENAR");return f;
    }
    static object Resource(object f){
        var cmd=(ComandoAlmacenamientoCarpetas)Call(f,"Prepare");
        return new {rootId=cmd.Raiz.RaizId,fileId=cmd.Archivo.ArchivoId,expedienteId=cmd.Raiz.IdentidadExpediente.ExpedienteId,relacionNivelId=cmd.Raiz.IdentidadExpediente.RelacionNivelId,fileName=cmd.Archivo.NombreOriginal,bytes=cmd.Archivo.BytesPrevistos,contentSha256=cmd.Archivo.ContenidoHash,requestSha256=cmd.Intento.SolicitudHash};
    }
    static void HashParity(){
        var f=Fixture("DocumentPhaseFixture");try{
            var source=Field<object>(f,"Source");var lot=Field<LoteImportacionCarpetas>(source,"Lot");var revision=Field<uint>(f,"OriginalRevision");
            var expected=Doc117SolicitudHash.Calcular(lot.Contexto,"20","30",revision,"stable-document","ALMACENAR");
            var result=(ResultadoFaseDocumentoImportacion)Call(f,"Run");
            Check(result.Resultado.Estado=="CONFIRMADO"&&Field<IntentoImportacionCarpetas>(source,"Attempt").SolicitudHash==expected&&Field<int>(source,"Writers")==1,"ACTUAL_COORDINATOR_HASH_PARITY");
            var context=Clone(lot.Contexto);context.RutaCanonica+="/cambio con acento á";
            var variants=new[]{Doc117SolicitudHash.Calcular(context,"20","30",revision,"stable-document","ALMACENAR"),Doc117SolicitudHash.Calcular(lot.Contexto,"21","30",revision,"stable-document","ALMACENAR"),Doc117SolicitudHash.Calcular(lot.Contexto,"20","31",revision,"stable-document","ALMACENAR"),Doc117SolicitudHash.Calcular(lot.Contexto,"20","30",revision+1,"stable-document","ALMACENAR"),Doc117SolicitudHash.Calcular(lot.Contexto,"20","30",revision,"different-key","ALMACENAR"),Doc117SolicitudHash.Calcular(lot.Contexto,"20","30",revision,"stable-document","CREAR_EXPEDIENTE")};
            Check(variants.All(h=>h!=expected)&&variants.Distinct().Count()==6,"HASH_FIELD_NOT_BOUND");
            var before=Field<int>(source,"Writers");Call(f,"Run");Check(Field<int>(source,"Writers")==before,"HASH_TEST_REPLAYS_WRITER");
            Console.WriteLine("DOC117_ACTUAL_COORDINATOR_HASH_PARITY: PASS");
        }finally{((IDisposable)f).Dispose();}
    }
    public static int Main(string[] args){
        var a=Source(false);var b=Source(true);try{
            if(args.Length==2&&args[0]=="--describe"){
                var scope=new{issueKey="DOC-117",protocol="FIRST_ATTEMPT_BEFORE_WRITER_V1",runtimeRoot=AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar),runId=new string('e',32),lotId="900",snapshotSha256=Field<LoteImportacionCarpetas>(a,"Lot").Snapshot.Huella,resources=new[]{Resource(a),Resource(b)}};
                File.WriteAllText(args[1],JsonConvert.SerializeObject(scope));Console.WriteLine("DOC117_SYNTHETIC_PRODUCT_COMMAND_SCOPE: PASS");return 0;
            }
            Check(args.Length==1&&args[0]=="--verify","ARGUMENTS");HashParity();
            var starts=0;var saves=0;var writers=0;
            foreach(var f in new[]{a,b}){
                var cmd=(ComandoAlmacenamientoCarpetas)Call(f,"Prepare");
                var suffix=Object.ReferenceEquals(f,a)?"A":"B";
                Constant("SitioAutorizado",AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar));Constant("LoteAutorizado",cmd.Raiz.LoteId);Constant("HuellaSnapshot",cmd.Snapshot.Huella);
                Constant("Raiz"+suffix,cmd.Raiz.RaizId);Constant("Archivo"+suffix,cmd.Archivo.ArchivoId);Constant("Expediente"+suffix,cmd.Raiz.IdentidadExpediente.ExpedienteId);Constant("Relacion"+suffix,cmd.Raiz.IdentidadExpediente.RelacionNivelId);Constant("Nombre"+suffix,cmd.Archivo.NombreOriginal);Constant("Bytes"+suffix,cmd.Archivo.BytesPrevistos);Constant("Contenido"+suffix,cmd.Archivo.ContenidoHash);Constant("Solicitud"+suffix,cmd.Intento.SolicitudHash);
                Lab("ExigirAlcance",cmd,false);
                Check(!cmd.Intento.EsNuevaReserva,"TRANSIENT_RESERVATION_FLAG_WAS_PERSISTED");
                var bad=Clone(cmd.Intento);bad.SolicitudHash=new string('0',64);bad.EsNuevaReserva=true;
                var lot=Field<LoteImportacionCarpetas>(f,"Lot");var root=Field<RaizImportacionCarpetas>(f,"Root");var file=Field<ArchivoImportacionCarpetas>(f,"FileRecord");
                var wrong=ComandoAlmacenamientoCarpetas.Preparar(lot.Contexto,root,file,lot.Snapshot,bad);
                Reject(()=>Lab("Iniciar",wrong,new Func<bool>(()=>{starts++;return true;})),"DOC117_ALM02_SCOPE_INVALID");
                var pin=Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"App_Data","DOC117-ALM02-"+new string('e',32),"intento-archivo-"+cmd.Archivo.ArchivoId+".json");
                Check((bool)Lab("Iniciar",cmd,new Func<bool>(()=>{Check(System.IO.File.Exists(pin),"PIN_NOT_DURABLE_BEFORE_START");var record=Newtonsoft.Json.Linq.JObject.Parse(System.IO.File.ReadAllText(pin));Check((string)record["intentoId"]==cmd.Intento.IntentoId&&(string)record["solicitudHash"]==cmd.Intento.SolicitudHash,"PIN_NOT_CORRELATED");starts++;return true;})),"START_FAILED");
                Lab("ExigirAlcance",cmd,true);
                // The product command returns immutable copies, not the internal claim.
                cmd.Intento.SolicitudHash=new string('0',64);Lab("ExigirAlcance",cmd,true);
                Lab("Invocar",cmd,new Action(()=>writers++));
                Reject(()=>Lab("Invocar",cmd,new Action(()=>writers++)),"DOC117_ALM02_WRITER_ALREADY_OBSERVED");
                var outcome=new ResultadoEfectoImportacion{Estado="CONFIRMADO",DocumentoId="990",InventarioId="991",Expediente=cmd.Raiz.IdentidadExpediente,OrigenGabinete="DA",GabineteId="10",ValoresEfectivos=cmd.Archivo.ValoresEfectivos};
                var save=new Func<ComandoAlmacenamientoCarpetas,ResultadoEfectoImportacion,ResultadoEfectoImportacion>((c,r)=>{saves++;return r;});
                Reject(()=>Lab("Registrar",cmd,outcome,save),"DOC117_ALM02_INTERNAL_FAILURE");
                Reject(()=>Lab("Iniciar",cmd,new Func<bool>(()=>{starts++;return true;})),"DOC117_ALM02_ALREADY_OBSERVED");
                Check(outcome.DocumentoId=="990"&&outcome.InventarioId=="991","OBSERVED_IDS_LOST");
            }
            Check(starts==2&&writers==2&&saves==1,"PIN_OR_WINDOW_BUDGET_CHANGED");Console.WriteLine("DOC117_ATTEMPT_PIN_WITH_ACTUAL_IMMUTABLE_COMMAND: PASS");return 0;
        }finally{((IDisposable)a).Dispose();((IDisposable)b).Dispose();}
    }
}
