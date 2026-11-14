using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GestionDocumental_Docuarchi.net;

// Read-only diagnostic host: no creator, writer, repository or HTTP session.
public static class Doc120CreationPrerequisites {
    static string stage="INPUT";
    static JObject detail;
    public static int Main(string[] args) {
        if(args.Length!=1)return 2;
        Console.InputEncoding=new UTF8Encoding(false);Console.OutputEncoding=new UTF8Encoding(false);
        var bin=Path.GetFullPath(args[0]);
        AppDomain.CurrentDomain.AssemblyResolve+=(sender,eventArgs)=>{
            var name=new AssemblyName(eventArgs.Name).Name;
            if(name.IndexOfAny(Path.GetInvalidFileNameChars())>=0)return null;
            var file=Path.Combine(bin,name+".dll");return File.Exists(file)?Assembly.LoadFrom(file):null;
        };
        return Run();
    }
    [MethodImpl(MethodImplOptions.NoInlining)]
    static int Run() {
        LoteImportacionCarpetas lot=null;RaizImportacionCarpetas root=null;
        try {
            var input=JObject.Parse(Console.ReadLine());
            lot=input["lot"].ToObject<LoteImportacionCarpetas>();
            root=input["root"].ToObject<RaizImportacionCarpetas>();
            var attempt=input["attempt"].ToObject<IntentoImportacionCarpetas>();
            // Counterfactual copies ONLY, to check prerequisites before the later stop.
            // This never proves current authority, a start transaction or permission to replay.
            lot.DetencionSolicitada=false;attempt.EsNuevaReserva=true;
            stage="COMMAND";
            var command=ComandoCreacionExpedienteImportacion.Preparar(lot.Contexto,lot,root,attempt);
            stage="LEGACY_INPUT";command.ValidarEntradasLegacy();
            if((bool?)input["commandOnly"]==true){Emit("PASS","COMMAND_COMPATIBLE");return 0;}
            stage="CATALOG";
            var names=new PerfilImportacionCatalogAdapter("RA","DA",Read).ResolverDatosCreacionExpediente(command);
            stage="LEGACY_CATALOG_VALUES";command.ValidarDatosLegacy(names);
            Emit("PASS","CATALOG_COMPATIBLE");return 0;
        } catch(Exception error) {
            while(error.InnerException!=null)error=error.InnerException;
            var known=error as PerfilImportacionException;
            if(stage=="COMMAND"&&known!=null&&known.Codigo=="CONFIGURACION_INVALIDA") {
                try{detail=DiagnoseConfiguration(lot,root);}catch{detail=new JObject(new JProperty("guard","UNCLASSIFIED"));}
            }
            Emit("STOPPED",known==null?"UNCLASSIFIED":known.Codigo);return 2;
        }
    }
    // Diagnostic classification only AFTER the actual command rejected the copy.
    // Uses the actual structural/value validator and actual fingerprint routine.
    // No value, configured name, hash or exception message leaves this process.
    static JObject Finding(string guard,string field=null,string issue=null) {
        return new JObject(new JProperty("guard",guard),new JProperty("field",field),new JProperty("issue",issue));
    }
    static bool Reference(string value){return ValidadorPerfilImportacion.ReferenciaValida(value);}
    static JObject DiagnoseConfiguration(LoteImportacionCarpetas lot,RaizImportacionCarpetas root) {
        var snapshot=lot.Snapshot;
        if(snapshot==null||snapshot.Configuracion==null||snapshot.Reglas==null||!Reference(snapshot.PerfilId)||!Reference(snapshot.VersionId)||snapshot.NumeroVersion==0||snapshot.Huella==null||!Regex.IsMatch(snapshot.Huella,"\\A[0-9a-f]{64}\\z"))return Finding("SNAPSHOT_SHAPE");
        var copy=PerfilImportacionJson.Leer<SnapshotPerfilImportacionOperativa>(PerfilImportacionJson.Escribir(snapshot));
        var errors=new ValidadorPerfilImportacion().Validar(copy.Configuracion,copy.Reglas,true);
        if(errors.Count!=0){var f=Finding("PROFILE_VALIDATION",errors[0].Campo,errors[0].Codigo);f["incidenceCount"]=errors.Count;return f;}
        if(PerfilImportacionJson.Escribir(copy.Configuracion)!=PerfilImportacionJson.Escribir(snapshot.Configuracion))return Finding("CONFIGURATION_NORMALIZED");
        var fingerprint=BitConverter.ToString(PerfilImportacionJson.Huella(new VersionPerfilImportacion{Configuracion=snapshot.Configuracion,Reglas=snapshot.Reglas})).Replace("-","").ToLowerInvariant();
        if(fingerprint!=snapshot.Huella)return Finding("SNAPSHOT_FINGERPRINT");
        var rules=snapshot.Reglas.Where(r=>r.DestinoTipo=="CAMPO_EXPEDIENTE").ToDictionary(r=>r.DestinoRef,StringComparer.Ordinal);
        if(root.ValoresPlan==null||root.ValoresPlan.Count!=rules.Count){var f=Finding("ROOT_FIELD_COUNT");f["actualCount"]=root.ValoresPlan==null?0:root.ValoresPlan.Count;f["expectedCount"]=rules.Count;return f;}
        var fields=new Dictionary<string,ValorReglaPerfilImportacion>(StringComparer.Ordinal);
        var valueValidator=typeof(ResolvedorReglasPerfilImportacion).GetMethod("ValidarValorResuelto",BindingFlags.Static|BindingFlags.NonPublic);
        foreach(var value in root.ValoresPlan) {
            if(value==null||value.DestinoTipo!="CAMPO_EXPEDIENTE"||String.IsNullOrEmpty(value.DestinoRef)||!rules.ContainsKey(value.DestinoRef)||value.Diferido||!String.IsNullOrEmpty(value.PropiedadPendiente))return Finding("ROOT_FIELD_SHAPE",value==null?null:value.DestinoRef);
            if(fields.ContainsKey(value.DestinoRef))return Finding("ROOT_FIELD_DUPLICATE",value.DestinoRef);
            var rule=rules[value.DestinoRef];var error=(string)valueValidator.Invoke(null,new object[]{rule,value.Valor});
            if(value.FuenteTipo!=rule.FuenteTipo||value.TipoValor!=rule.TipoValor||error!=null){var f=Finding("ROOT_FIELD_RULE",value.DestinoRef,error);f["sourceMatches"]=value.FuenteTipo==rule.FuenteTipo;f["typeMatches"]=value.TipoValor==rule.TipoValor;f["valueValid"]=error==null;f["valueLength"]=value.Valor==null?0:value.Valor.Length;return f;}
            if(value.DestinoRef=="RESPONSABLE"&&!Reference(value.Valor))return Finding("RESPONSIBLE_REFERENCE","RESPONSABLE");
            fields.Add(value.DestinoRef,value);
        }
        if(fields.ContainsKey("FECHA_FINAL")&&String.CompareOrdinal(fields["FECHA_INICIAL"].Valor,fields["FECHA_FINAL"].Valor)>0)return Finding("DATE_RANGE","FECHA_FINAL");
        return Finding("UNCLASSIFIED");
    }
    static DataTable Read(string origin,string sql,object[] parameters) {
        Console.WriteLine(JsonConvert.SerializeObject(new {type="SELECT",origin,sql,parameters}));
        Console.Out.Flush();
        var response=JObject.Parse(Console.ReadLine());
        if((string)response["type"]!="ROWS")throw new InvalidOperationException();
        var table=new DataTable();
        foreach(var column in response["columns"].Values<string>())table.Columns.Add(column,typeof(object));
        foreach(var item in (JArray)response["rows"]) {
            var row=table.NewRow();
            foreach(DataColumn column in table.Columns) {
                var value=item[column.ColumnName];
                row[column]=value==null||value.Type==JTokenType.Null?DBNull.Value:(object)value.ToString();
            }
            table.Rows.Add(row);
        }
        return table;
    }
    static void Emit(string status,string code) {
        // The parent validates a closed list again; no exception messages or row values.
        Console.WriteLine(JsonConvert.SerializeObject(new {type="RESULT",status,stage,code,detail,counterfactual=true,sessionAuthorityChecked=false,sessionConnectionCompatibilityChecked=false,startTransactionChecked=false,recoveryAuthorized=false}));
    }
}
