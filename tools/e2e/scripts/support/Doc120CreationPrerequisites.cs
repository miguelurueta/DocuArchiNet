using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GestionDocumental_Docuarchi.net;

// Read-only diagnostic host: no creator, writer, repository or HTTP session.
public static class Doc120CreationPrerequisites {
    static string stage="INPUT";
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
        try {
            var input=JObject.Parse(Console.ReadLine());
            var lot=input["lot"].ToObject<LoteImportacionCarpetas>();
            var root=input["root"].ToObject<RaizImportacionCarpetas>();
            var attempt=input["attempt"].ToObject<IntentoImportacionCarpetas>();
            // Counterfactual copies ONLY, to check prerequisites before the later stop.
            // This never proves current authority, a start transaction or permission to replay.
            lot.DetencionSolicitada=false;attempt.EsNuevaReserva=true;
            stage="COMMAND";
            var command=ComandoCreacionExpedienteImportacion.Preparar(lot.Contexto,lot,root,attempt);
            stage="LEGACY_INPUT";command.ValidarEntradasLegacy();
            stage="CATALOG";
            var names=new PerfilImportacionCatalogAdapter("RA","DA",Read).ResolverDatosCreacionExpediente(command);
            stage="LEGACY_CATALOG_VALUES";command.ValidarDatosLegacy(names);
            Emit("PASS","CATALOG_COMPATIBLE");return 0;
        } catch(Exception error) {
            while(error.InnerException!=null)error=error.InnerException;
            var known=error as PerfilImportacionException;
            Emit("STOPPED",known==null?"UNCLASSIFIED":known.Codigo);return 2;
        }
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
        Console.WriteLine(JsonConvert.SerializeObject(new {type="RESULT",status,stage,code,counterfactual=true,sessionAuthorityChecked=false,sessionConnectionCompatibilityChecked=false,startTransactionChecked=false,recoveryAuthorized=false}));
    }
}
