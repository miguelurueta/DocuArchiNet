using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using Newtonsoft.Json;
using GestionDocumental_Docuarchi.net;
public static class Doc120PrerequisiteFixture {
    public static void Main(string[] args) {
        Console.OutputEncoding=new UTF8Encoding(false);
        var bin=Path.GetFullPath(args[0]);
        AppDomain.CurrentDomain.AssemblyResolve+=(sender,e)=>Assembly.LoadFrom(Path.Combine(bin,new AssemblyName(e.Name).Name+".dll"));
        Emit();
    }
    static string Hash(byte[] value){using(var sha=SHA256.Create())return BitConverter.ToString(sha.ComputeHash(value)).Replace("-","").ToLowerInvariant();}
    [MethodImpl(MethodImplOptions.NoInlining)]
    static void Emit() {
        var context=new ContextoImportacionCarpetas{UsuarioRef="1",AmbitoRef="2",NivelRef="7",RutaCanonica="synthetic-private-route",SelloDestino="synthetic-seal",PuedeImportar=true};
        var config=new ConfiguracionPerfilImportacion{NombreVisible="Synthetic",GabineteId=7,HuellaOrigenGabinete=Hash(Encoding.UTF8.GetBytes(PerfilImportacionJson.Escribir(new[]{"host-DA","3306","1","base-DA"}))),FondoRef="1",InstrumentoRef="2",AreaRef="3",SerieRef="4",TipoExpedienteRef="5",UnidadDocumentalRef="6",ModoIntegridad="COMPLETO_OBLIGATORIO",AmbitoIntegridad="EXPEDIENTE",EstrategiaCarpetas="RAIZ_EXPEDIENTE",PoliticaSubcarpetas="INCLUIR",PoliticaCarpetasVacias="RECHAZAR",PoliticaArchivosSueltos="RECHAZAR",Extensiones=new List<string>{"pdf"}};
        var rules=new List<ReglaPerfilImportacion>();var values=new List<ValorReglaPerfilImportacion>();
        foreach(var field in new[]{"CODIGO","NOMBRE","FECHA_INICIAL","FECHA_FINAL","RESPONSABLE"}) {
            var type=field.StartsWith("FECHA")?"FECHA":field=="RESPONSABLE"?"REFERENCIA":"TEXTO";
            var value=field=="CODIGO"?"000123":field=="NOMBRE"?"Órdenes 2026":field=="FECHA_INICIAL"?"2026-10-29":field=="FECHA_FINAL"?"2026-10-30":"8";
            rules.Add(new ReglaPerfilImportacion{DestinoTipo="CAMPO_EXPEDIENTE",DestinoRef=field,TipoValor=type,FuenteTipo="CARPETA",FormatoFecha=type=="FECHA"?"dd-MM-yyyy":null,LongitudMaxima=160});
            values.Add(new ValorReglaPerfilImportacion{DestinoTipo="CAMPO_EXPEDIENTE",DestinoRef=field,TipoValor=type,FuenteTipo="CARPETA",Valor=value,Procedencia="synthetic-private-marker"});
        }
        rules.Add(new ReglaPerfilImportacion{DestinoTipo="TIPOLOGIA",DestinoRef="TIPO_DOCUMENTAL",TipoValor="REFERENCIA",FuenteTipo="CONSTANTE",ValorConstante="9"});
        var snapshot=new SnapshotPerfilImportacionOperativa{PerfilId="4",VersionId="5",NumeroVersion=1,Configuracion=config,Reglas=rules};
        snapshot.Huella=BitConverter.ToString(PerfilImportacionJson.Huella(new VersionPerfilImportacion{Configuracion=config,Reglas=rules})).Replace("-","").ToLowerInvariant();
        Console.WriteLine(JsonConvert.SerializeObject(new[]{new {lote_id="39",plan_id="57",lote_estado="EN_CURSO",lote_revision="5",detencion_solicitada="1",contexto_doc=PerfilImportacionJson.Escribir(context),config_doc=PerfilImportacionJson.Escribir(snapshot),raiz_id="60",cliente_raiz_id="synthetic-root",datos_doc=PerfilImportacionJson.Escribir(new {Esquema=1,IdCliente="synthetic-root",NombreOriginal="synthetic-private-root",ValoresPlan=values}),raiz_estado="CREANDO",raiz_revision="2",intento_activo_id="96",identidad_expediente_doc=(string)null,valores_efectivos_doc=(string)null,intento_id="96",fase="CREAR_EXPEDIENTE",intento_estado="RESERVADO",token_reclamacion=new string('a',32),solicitud_hash=new string('b',64),fin_utc=(string)null,evidencia_doc=(string)null}}));
    }
}
