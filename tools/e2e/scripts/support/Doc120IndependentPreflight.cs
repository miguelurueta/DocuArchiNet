using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GestionDocumental_Docuarchi.net;
using MySql.Data.MySqlClient;

// Offline readers and pure validators only. No connection, repository instance,
// HttpContext, disk staging, reservation, writer or control INSERT is constructed.
public static class Doc120IndependentPreflight {
    static string stage="INPUT";
    static Type repository;
    static readonly HashSet<string> Readers=new HashSet<string>{"LeerPlan","ReconstruirPlanSegmentadoPersistido","LeerRaizControl","LeerArchivoRecepcion","LeerIntentoDocumental"};
    static object Invoke(string name,params object[] args){
        if(!Readers.Contains(name))throw new InvalidOperationException();
        return repository.GetMethod(name,BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,args);
    }
    static DataTable Table(JArray rows){
        var table=new DataTable();
        foreach(JObject row in rows)foreach(var p in row.Properties())if(!table.Columns.Contains(p.Name))table.Columns.Add(p.Name,typeof(object));
        var binary=new HashSet<string>{"plan_hash","solicitud_hash","ruta_hash","contenido_hash"};
        foreach(JObject row in rows){
            var target=table.NewRow();
            foreach(DataColumn c in table.Columns){
                var value=row[c.ColumnName];
                if(value==null||value.Type==JTokenType.Null)target[c]=DBNull.Value;
                else if(binary.Contains(c.ColumnName))target[c]=Convert.FromBase64String((string)value);
                else if(c.ColumnName.EndsWith("_utc",StringComparison.Ordinal))target[c]=DateTime.ParseExact((string)value,"yyyy-MM-dd HH:mm:ss",CultureInfo.InvariantCulture);
                else target[c]=(string)value;
            }
            table.Rows.Add(target);
        }
        return table;
    }
    static List<T> Read<T>(JArray rows,string method){
        var result=new List<T>();using(var table=Table(rows))using(var reader=table.CreateDataReader())while(reader.Read())result.Add((T)Invoke(method,reader));return result;
    }
    static string Hex(string base64){return BitConverter.ToString(Convert.FromBase64String(base64)).Replace("-","").ToLowerInvariant();}
    public static int Main(string[] args){
        if(args.Length!=1&&(args.Length!=2||args[1]!="--native-select"))return 2;
        Console.InputEncoding=new UTF8Encoding(false);Console.OutputEncoding=new UTF8Encoding(false);
        var bin=Path.GetFullPath(args[0]);
        AppDomain.CurrentDomain.AssemblyResolve+=(sender,e)=>{
            var name=new AssemblyName(e.Name).Name;
            if(name.IndexOfAny(Path.GetInvalidFileNameChars())>=0)return null;
            var file=Path.Combine(bin,name+".dll");return File.Exists(file)?Assembly.LoadFrom(file):null;
        };
        return args.Length==2?RunNative():Run();
    }
    [MethodImpl(MethodImplOptions.NoInlining)]
    static int RunNative(){
        int reserved=0;
        try{
            stage="NATIVE_INPUT";var input=JObject.Parse(Console.In.ReadToEnd());
            var dsn=(string)input["dsn"];if(dsn!="MembershipUsers")throw new InvalidOperationException();
            string server=null,database=null,port=null;
            foreach(var hive in new[]{Microsoft.Win32.Registry.CurrentUser,Microsoft.Win32.Registry.LocalMachine}){
                using(var key=hive.OpenSubKey(@"SOFTWARE\ODBC\ODBC.INI\"+dsn)){
                    if(key==null)continue;server=key.GetValue("SERVER") as string;database=key.GetValue("DATABASE") as string;port=Convert.ToString(key.GetValue("PORT"),CultureInfo.InvariantCulture);break;
                }
            }
            if(String.IsNullOrWhiteSpace(server)||String.IsNullOrWhiteSpace(database)||String.IsNullOrWhiteSpace((string)input["user"])||String.IsNullOrWhiteSpace((string)input["password"]))throw new InvalidOperationException();
            repository=typeof(MySqlImportacionCarpetasRepository);
            var settings=new MySqlConnectionStringBuilder{Server=server,Database=database,UserID=(string)input["user"],Password=(string)input["password"],Port=String.IsNullOrEmpty(port)?3306:UInt32.Parse(port),Pooling=false,ConnectionTimeout=10,DefaultCommandTimeout=30};
            stage="NATIVE_OPEN";
            using(var connection=new MySqlConnection(settings.ConnectionString)){
                connection.Open();stage="NATIVE_CONTEXT";reserved++;
                ContextoImportacionCarpetas context;
                using(var cmd=new MySqlCommand("SELECT contexto_doc FROM ra_integracion_lote WHERE lote_id=50 AND solicitante_ref='141' AND ambito_ref='2' AND nivel_ref='223' LIMIT 2",connection)){
                    using(var reader=cmd.ExecuteReader()){if(!reader.Read())throw new InvalidOperationException();context=PerfilImportacionJson.Leer<ContextoImportacionCarpetas>(Convert.ToString(reader[0],CultureInfo.InvariantCulture));if(reader.Read())throw new InvalidOperationException();}
                }
                // Closed, inspected static read methods only. Budget reserves their
                // maximum SELECT count before entry; no transaction/FOR UPDATE.
                stage="NATIVE_METADATA";reserved+=5;
                var metadata=repository.GetMethod("LeerMetadatos",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,new object[]{connection,"'ra_integracion_plan','ra_integracion_lote','ra_integracion_raiz','ra_integracion_archivo','ra_integracion_intento','ra_integracion_evento'","'ra_integracion_plan','ra_integracion_lote','ra_integracion_raiz','ra_integracion_archivo','ra_integracion_intento','ra_integracion_evento','ra_integracion_perfil_version','remit_dest_interno_perfil_produccion'",null});
                repository.GetMethod("ValidarMetadatos",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,new object[]{metadata});
                stage="NATIVE_INDEX";reserved++;
                repository.GetMethod("ExigirIndiceControlFocal",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,new object[]{connection,null});
                stage="NATIVE_LOT";reserved+=2;
                var lot=(LoteImportacionCarpetas)repository.GetMethod("BuscarLote",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,new object[]{connection,null,context,"lote_id",50U});
                if(lot==null||lot.DetencionSolicitada||lot.TotalArchivos!=1000||lot.TotalRaices!=1)throw new InvalidOperationException();
                stage="NATIVE_PLAN";reserved+=3;
                repository.GetMethod("CargarPlan",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,new object[]{connection,null,context,UInt32.Parse(lot.PlanId),false});
            }
            Console.WriteLine(JsonConvert.SerializeObject(new{ok=true,stage="NATIVE_READS_PASSED",selectReserved=reserved,readOnly=true,writerInvoked=false,sqlWritesTested=false}));return 0;
        }catch(Exception error){return Fail(error,reserved);}
    }
    static int Fail(Exception error,int reserved){
        while(error is TargetInvocationException&&error.InnerException!=null)error=error.InnerException;
        var frames=new StackTrace(error,false).GetFrames()??new StackFrame[0];
        var methods=frames.Select(f=>f.GetMethod()).Where(m=>m!=null&&m.DeclaringType!=null&&repository!=null&&m.DeclaringType.Assembly==repository.Assembly).Select(m=>m.DeclaringType.Name+"."+m.Name).Where(n=>n.All(c=>Char.IsLetterOrDigit(c)||"._$-".Contains(c))).Take(8).ToArray();
        var profile=error as PerfilImportacionException;
        Console.WriteLine(JsonConvert.SerializeObject(new{ok=false,stage,selectReserved=reserved,exceptionType=error.GetType().Name,methods,code=profile==null?null:profile.Codigo,readOnly=true,writerInvoked=false}));return 2;
    }
    [MethodImpl(MethodImplOptions.NoInlining)]
    static int Run(){
        try{
            var packet=JObject.Parse(Console.In.ReadToEnd());repository=typeof(MySqlImportacionCarpetasRepository);
            var row=(JObject)packet["lot"][0];
            if((string)row["lote_id"]!="50"||(string)row["solicitante_ref"]!="141"||(string)row["ambito_ref"]!="2"||(string)row["nivel_ref"]!="223")throw new InvalidOperationException();
            var lot=new LoteImportacionCarpetas{LoteId="50",PlanId=(string)row["plan_id"],Contexto=PerfilImportacionJson.Leer<ContextoImportacionCarpetas>((string)row["contexto_doc"]),Snapshot=PerfilImportacionJson.Leer<SnapshotPerfilImportacionOperativa>((string)row["config_doc"]),
                TotalRaices=UInt32.Parse((string)row["total_raices"]),TotalArchivos=UInt32.Parse((string)row["total_archivos"]),TotalBytes=(string)row["total_bytes"],Estado=(string)row["estado"],Revision=UInt32.Parse((string)row["revision"]),DetencionSolicitada=(string)row["detencion_solicitada"]!="0"};
            stage="READ_PLAN";var plan=Read<PlanImportacionCarpetas>((JArray)packet["plan"],"LeerPlan").Single();
            if(plan.PlanId!=lot.PlanId||plan.Estado!="CONFIRMADO"||lot.TotalArchivos!=1000||lot.TotalRaices!=1||lot.DetencionSolicitada)throw new InvalidOperationException();
            stage="PLAN_FRAGMENTS";
            if(plan.EsquemaDoc==2){
                var fragments=((JArray)packet["fragments"]).Cast<JObject>().Select(r=>new FragmentoPlanImportacion{Tipo=(string)r["tipo"],Numero=UInt32.Parse((string)r["numero"]),Contenido=(string)r["contenido_doc"],Huella=Hex((string)r["contenido_hash"])}).ToList();
                Invoke("ReconstruirPlanSegmentadoPersistido",plan,fragments);
            }
            stage="READ_ROOTS";var roots=Read<RaizImportacionCarpetas>((JArray)packet["roots"],"LeerRaizControl");
            stage="READ_FILES";var files=Read<ArchivoImportacionCarpetas>((JArray)packet["files"],"LeerArchivoRecepcion");
            stage="READ_ATTEMPTS";var attempts=Read<IntentoImportacionCarpetas>((JArray)packet["attempts"],"LeerIntentoDocumental");
            stage="EVALUATE";
            typeof(CoordinadorImportacionCarpetas).GetMethod("EvaluarConsolidacion",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,new object[]{new VistaConsolidacionImportacion{Lote=lot,Plan=plan,Raices=roots,Archivos=files,Intentos=attempts}});
            stage="SEAL";var seal=ControlArchivoIndependienteImportacion.Sellar(lot,plan,roots,files);
            stage="SERIALIZE_CONTROLS";long characters=PerfilImportacionJson.Escribir(seal.Cabecera).Length;
            foreach(var f in files){characters+=PerfilImportacionJson.Escribir(seal.Pruebas[f.ArchivoId]).Length;ControlArchivoIndependienteImportacion.Verificar(lot,roots.Single(r=>r.RaizId==f.RaizId),f,seal.Cabecera,seal.Pruebas[f.ArchivoId]);}
            Console.WriteLine(JsonConvert.SerializeObject(new{ok=true,stage="PURE_PREFLIGHT_PASSED",files=files.Count,roots=roots.Count,attempts=attempts.Count,proofs=seal.Pruebas.Count,controlCharacters=characters,readOnly=true,writerInvoked=false,sqlWritesTested=false}));return 0;
        }catch(Exception error){return Fail(error,0);}
    }
}
