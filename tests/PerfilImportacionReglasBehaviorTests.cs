using System;
using System.Collections.Generic;
using System.Linq;

public static class PerfilImportacionReglasBehaviorTests
{
    static void Check(bool condition, string name) { if (!condition) throw new Exception(name); }
    static ReglaPerfilImportacion Rule(string separator, int position) {
        return new ReglaPerfilImportacion {
            DestinoTipo = "INDICE_GABINETE", DestinoRef = "1", FuenteTipo = "ARCHIVO",
            Extraccion = "SEGMENTO", Separador = separator, Posicion = position,
            QuitarExtension = true, Obligatoria = true
        };
    }
    public static void Run() {
        var resolver = new ResolvedorReglasPerfilImportacion();
        foreach(var property in ResolvedorReglasPerfilImportacion.PropiedadesClasificacion) {
            var inherited=new ReglaPerfilImportacion {DestinoTipo="INDICE_GABINETE",DestinoRef="99",FuenteTipo="EXPEDIENTE",PropiedadExpediente=property,EtapaResolucion="TRAS_EXPEDIENTE",TipoValor="TEXTO",Obligatoria=true};
            var sample=new EjemploPerfilImportacion();
            Check(resolver.Resolver(new List<ReglaPerfilImportacion>{inherited},sample).Valores.Single().Diferido,"classification missing is deferred");
            sample.Expediente[property]="0007";
            Check(resolver.Resolver(new List<ReglaPerfilImportacion>{inherited},sample).Valores.Single().Valor=="0007","classification preserves text");
            var roundtrip=PerfilImportacionJson.Leer<ReglaPerfilImportacion>(PerfilImportacionJson.Escribir(inherited));
            Check(roundtrip.PropiedadExpediente==property,"classification persisted contract roundtrip");
            sample.Expediente[property]=null;
            Check(resolver.Resolver(new List<ReglaPerfilImportacion>{inherited},sample).Incidencias.Any(x=>x.Codigo=="VALOR_REQUERIDO"),"known absence required fails");
            inherited.Obligatoria=false;
            var absent=resolver.Resolver(new List<ReglaPerfilImportacion>{inherited},sample);
            Check(absent.Incidencias.Count==0 && !absent.Valores.Single().Diferido && absent.Valores.Single().Valor==null,"optional absence is not zero or deferred");
            if(!property.EndsWith("_ID")){inherited.TipoValor="ENTERO";Check(resolver.ValidarEstructura(new List<ReglaPerfilImportacion>{inherited}).Any(x=>x.Codigo=="TIPO_DEPENDENCIA_INVALIDO"),"classification name and code must be text");}
        }
        foreach (var sample in new[] {
            new { Rule = Rule("-", 0), Name = "000123-125-Compra.pdf" },
            new { Rule = Rule("__", 1), Name = "Compra__000123__125.tif" }
        }) {
            var result = resolver.Resolver(new List<ReglaPerfilImportacion>{ sample.Rule },
                new EjemploPerfilImportacion { NombreArchivo = sample.Name });
            Check(result.Incidencias.Count == 0 && result.Valores.Single().Valor == "000123", "configurable pattern preserves zeros");
        }
        var rule = Rule("-", 0);
        Check(resolver.ValidarEstructura(new List<ReglaPerfilImportacion>{rule, rule}).Any(x => x.Codigo == "DESTINO_DUPLICADO"), "duplicate destination");
        rule.Extraccion = "SQL";
        Check(resolver.ValidarEstructura(new List<ReglaPerfilImportacion>{rule}).Count > 0, "executable extraction rejected");
        rule = Rule("-", 8);
        Check(resolver.Resolver(new List<ReglaPerfilImportacion>{rule}, new EjemploPerfilImportacion {NombreArchivo="a.pdf"}).Incidencias.Any(x => x.Codigo == "SEGMENTO_AUSENTE"), "missing segment");
        rule = new ReglaPerfilImportacion {DestinoTipo="CAMPO_EXPEDIENTE", DestinoRef="CODIGO", FuenteTipo="CONSTANTE", ValorConstante=new string('1', 46)};
        Check(resolver.Resolver(new List<ReglaPerfilImportacion>{rule}, new EjemploPerfilImportacion()).Incidencias.Any(x=>x.Codigo=="CODIGO_EXCEDE_45"), "legacy code limit");
        rule.FuenteTipo = "ARCHIVO";
        Check(resolver.ValidarEstructura(new List<ReglaPerfilImportacion>{rule}).Count > 0, "no reduction across files");
        rule = new ReglaPerfilImportacion {DestinoTipo="INDICE_GABINETE", DestinoRef="1", FuenteTipo="EXPEDIENTE", PropiedadExpediente="ID", EtapaResolucion="TRAS_EXPEDIENTE", TipoValor="ENTERO"};
        var deferred = resolver.Resolver(new List<ReglaPerfilImportacion>{rule}, new EjemploPerfilImportacion());
        Check(deferred.Incidencias.Count == 0 && deferred.Valores.Single().Diferido && deferred.Valores.Single().Valor == null, "typed deferred value");
        rule.TipoValor="FECHA";rule.FormatoFecha="yyyy-MM-dd";
        Check(resolver.ValidarEstructura(new List<ReglaPerfilImportacion>{rule}).Any(x=>x.Codigo=="TIPO_DEPENDENCIA_INVALIDO"),"invalid deferred type rejected before execution");
        var date = new ReglaPerfilImportacion {DestinoTipo="CAMPO_EXPEDIENTE", DestinoRef="FECHA_INICIAL", FuenteTipo="CONSTANTE", TipoValor="FECHA", FormatoFecha="yyyy-MM-dd", ValorConstante="2024-02-29"};
        Check(resolver.Resolver(new List<ReglaPerfilImportacion>{date}, new EjemploPerfilImportacion()).Incidencias.Count==0, "leap year");
        date.ValorConstante="2023-02-29";
        Check(resolver.Resolver(new List<ReglaPerfilImportacion>{date}, new EjemploPerfilImportacion()).Incidencias.Count>0, "invalid leap year");
        date.ValorConstante="2026-04-03";
        var end = new ReglaPerfilImportacion {DestinoTipo="CAMPO_EXPEDIENTE", DestinoRef="FECHA_FINAL", FuenteTipo="CONSTANTE", TipoValor="FECHA", FormatoFecha="yyyy-MM-dd", ValorConstante="2026-04-02"};
        Check(resolver.Resolver(new List<ReglaPerfilImportacion>{date,end}, new EjemploPerfilImportacion()).Incidencias.Any(x=>x.Codigo=="RANGO_FECHAS_INVALIDO"), "date order");
        var validator = new ValidadorPerfilImportacion();
        foreach (string scope in new string[] {null, "EXPEDIENTE", "LOTE", "NO_APLICA", "OTRO"}) {
            var config = new ConfiguracionPerfilImportacion {AmbitoIntegridad=scope};
            var errors = validator.Validar(config, new List<ReglaPerfilImportacion>(), true);
            Check(errors.Any(x=>x.Campo=="AmbitoIntegridad") == (scope!="EXPEDIENTE" && scope!="LOTE"), "integrity matrix");
        }
        var partial = new ConfiguracionPerfilImportacion {ModoIntegridad="PARCIAL_PERMITIDO",AmbitoIntegridad="LOTE"};
        validator.Validar(partial,new List<ReglaPerfilImportacion>(),false);
        Check(partial.AmbitoIntegridad=="NO_APLICA","partial normalization");
        partial.PoliticaCoincidencias="SOBRESCRIBIR";
        Check(validator.Validar(partial,new List<ReglaPerfilImportacion>(),false).Any(x=>x.Campo=="PoliticaCoincidencias"),"unknown matching policy");
        Check(new ConfiguracionPerfilImportacion().PoliticaCoincidencias=="BLOQUEAR","matching default");
        partial.ModoIntegridad="OTRO";
        Check(validator.Validar(partial,new List<ReglaPerfilImportacion>(),false).Any(x=>x.Campo=="ModoIntegridad"),"unknown integrity mode");
        var validExample=PerfilImportacionJson.Leer<EjemploPerfilImportacion>("{\"NombreCarpeta\":\"00125\",\"Expediente\":{}}");
        Check(validExample.Expediente.Count==0,"HTTP example without client classification accepted");
        foreach(var property in ResolvedorReglasPerfilImportacion.PropiedadesClasificacion){
            bool rejected=false;
            try { PerfilImportacionJson.Leer<EjemploPerfilImportacion>("{\"Expediente\":{\""+property+"\":\"forged\"}}"); }
            catch(PerfilImportacionException ex){rejected=ex.Codigo=="CONFIGURACION_INVALIDA";}
            Check(rejected,"HTTP classification keys rejected before service");
        }
        string serialized = PerfilImportacionJson.Escribir(new ConfiguracionPerfilImportacion {NombreVisible="Órdenes"});
        Check(serialized.All(c=>c<128),"ASCII escaped JSON");
        Check(PerfilImportacionJson.Leer<ConfiguracionPerfilImportacion>(serialized).NombreVisible=="Órdenes","unicode roundtrip");
        foreach(string invalid in new[] {"{\"Esquema\":1,\"Esquema\":2}","{\"Esquema\":1,\"esquema\":2}",
            "{\"Esquema\":1.2}","{\"Esquema\":\"1\"}","{\"SQL\":\"delete\"}","{} {}","null"}) {
            bool rejected=false;
            try { PerfilImportacionJson.Leer<ConfiguracionPerfilImportacion>(invalid); } catch(PerfilImportacionException) { rejected=true; }
            Check(rejected,"closed JSON schema");
        }
    }
}
