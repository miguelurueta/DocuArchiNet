using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// Metadata arrives through stdin from the existing SELECT-only ODBC bridge.
// No connection or credentials, and no repository instance or business writer.
public static class Doc120SchemaReadiness {
    public static int Main(string[] args){
        try{
            if(args.Length!=1)return 2;
            var directory=Path.GetFullPath(args[0]);
            AppDomain.CurrentDomain.AssemblyResolve+=(sender,eventArgs)=>{
                var name=new AssemblyName(eventArgs.Name).Name;
                if(name!="MySql.Data"&&name!="Newtonsoft.Json")return null;
                return Assembly.LoadFrom(Path.Combine(directory,name+".dll"));
            };
            var packet=JObject.Parse(Console.In.ReadToEnd());
            var assembly=Assembly.LoadFrom(Path.Combine(directory,"GestionDocumental-Docuarchi.net.dll"));
            var repository=assembly.GetType("GestionDocumental_Docuarchi.net.MySqlImportacionCarpetasRepository",true);
            foreach(var pair in new[]{new[]{"legacy","ValidarMetadatos"},new[]{"preparation","ValidarMetadatosPreparacion"}}){
                var data=packet[pair[0]].ToObject<DataSet>();
                var method=repository.GetMethod(pair[1],BindingFlags.Static|BindingFlags.NonPublic);
                if(method==null)throw new InvalidOperationException();
                method.Invoke(null,new object[]{data});
            }
            Console.WriteLine("{\"ok\":true,\"legacy\":true,\"preparation\":true,\"newTables\":3,\"newColumns\":31,\"readOnly\":true}");
            return 0;
        }catch{
            Console.WriteLine("{\"ok\":false,\"code\":\"SCHEMA_INCOMPATIBLE\",\"readOnly\":true}");
            return 2;
        }
    }
}
