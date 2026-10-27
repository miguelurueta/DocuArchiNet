using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// Utilidad manual, no incorporada al sitio. Misma canonicalización del perfil.
public static class Doc110MigrationJson {
    static JObject Parse(string input) {
        if(input==null || input.Length>262144)throw new InvalidOperationException("JSON_LIMIT");
        using(var reader=new JsonTextReader(new StringReader(input)) {
            DateParseHandling=DateParseHandling.None,MaxDepth=16
        }) {
            var value=JObject.Load(reader,new JsonLoadSettings {DuplicatePropertyNameHandling=DuplicatePropertyNameHandling.Error});
            if(reader.Read())throw new InvalidOperationException("JSON_TRAILING");
            return value;
        }
    }
    static JToken Sort(JToken input) {
        var obj=input as JObject;
        if(obj!=null) {
            var output=new JObject();
            foreach(var property in obj.Properties().OrderBy(p=>p.Name,StringComparer.Ordinal))output.Add(property.Name,Sort(property.Value));
            return output;
        }
        var array=input as JArray;
        return array==null?input.DeepClone():new JArray(array.Select(Sort));
    }
    public static string Canonical(string input) {return Write(Parse(input));}
    static string Write(JToken value) {
        return JsonConvert.SerializeObject(Sort(value),new JsonSerializerSettings {
            StringEscapeHandling=StringEscapeHandling.EscapeNonAscii,Culture=CultureInfo.InvariantCulture,
            TypeNameHandling=TypeNameHandling.None,DateParseHandling=DateParseHandling.None,MaxDepth=16
        });
    }
    public static string ConvertRule(string json,string previousId,string nextId) {
        uint id;
        if(!UInt32.TryParse(nextId,NumberStyles.None,CultureInfo.InvariantCulture,out id) || id==0 || id.ToString(CultureInfo.InvariantCulture)!=nextId)
            throw new InvalidOperationException("ID_INVALID");
        var rule=Parse(json);
        if(rule["ReglaId"]==null || rule["ReglaId"].Type!=JTokenType.String || (string)rule["ReglaId"]!=previousId)
            throw new InvalidOperationException("RULE_ID_MISMATCH");
        rule["ReglaId"]=nextId;
        return Write(rule);
    }
    public static byte[] Hash(string config,string[] rules) {
        var ordered=rules.Select(Parse).OrderBy(r=>(string)r["DestinoTipo"],StringComparer.Ordinal)
            .ThenBy(r=>(string)r["DestinoRef"],StringComparer.Ordinal);
        var document=new JObject();document.Add("configuracion",Parse(config));document.Add("reglas",new JArray(ordered));
        using(var sha=SHA256.Create())return sha.ComputeHash(Encoding.ASCII.GetBytes(Write(document)));
    }
}
