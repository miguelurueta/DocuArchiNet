using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public static class PerfilImportacionCatalogBehaviorTests {
    static void Check(bool condition,string message) {if(!condition)throw new Exception(message);}
    public static void Run() {
        int classificationReads=0;
        var classification=new PerfilImportacionCatalogAdapter("RA_TEST","DA_TEST",(origin,sql,parameters)=>{
            classificationReads++;
            Check(origin=="RA_TEST" && sql.Contains("@empresa") && sql.Contains("@instrumento") && sql.Contains("@area") && sql.Contains("@ref"),"classification authorized query");
            var t=new DataTable();t.Columns.Add("id",typeof(string));t.Columns.Add("nombre",typeof(string));
            bool sub=sql.Contains("FROM subseries_documentales");
            if(sub)Check(sql.Contains("s.Id_Series=@serie"),"subseries constrained by parent");
            t.Rows.Add(sub?"8":"7",sql.Contains("Codigo_Arbitrario")?(sub?"0002":"0001"):(sub?"Subserie ejemplo":"Serie ejemplo"));return t;
        });
        var cc=new ContextoPerfilImportacion{UsuarioRef="1",AmbitoRef="2",PuedeAdministrar=true};
        var cfg=new ConfiguracionPerfilImportacion{InstrumentoRef="1",AreaRef="2",SerieRef="7",SubserieRef="8"};
        var projected=classification.ClasificacionPropuesta(cc,cfg);
        Check(projected["SERIE_ID"]=="7" && projected["SERIE_CODIGO"]=="0001" && projected["SERIE_NOMBRE"]=="Serie ejemplo" && projected["SUBSERIE_CODIGO"]=="0002","three distinct representations");
        cfg.SubserieRef=null;projected=classification.ClasificacionPropuesta(cc,cfg);Check(projected.ContainsKey("SUBSERIE_ID") && projected["SUBSERIE_ID"]==null,"known absent subseries");
        cfg.SerieRef=null;Check(classification.ClasificacionPropuesta(cc,cfg).Count==0,"partial classification remains deferred");
        int readBefore=classificationReads;cc.PuedeAdministrar=false;
        bool classificationDenied=false;try{classification.ClasificacionPropuesta(cc,cfg);}catch(PerfilImportacionException){classificationDenied=true;}
        Check(classificationDenied && readBefore==classificationReads,"classification authorization precedes IO");
        bool changed=false; int cabinetReads=0; object indexAlias="Order number";
        Func<string,string,object[],DataTable> query=(origin,sql,parameters)=>{
            var table=new DataTable();
            if(sql.Contains("@@hostname")) {
                Check(origin=="DA_TEST","origin fingerprint must use DA");
                foreach(string col in new[]{"host","puerto","servidor","base"})table.Columns.Add(col,typeof(string));
                table.Rows.Add("synthetic","3306","1",changed?"other":"catalog");
            } else if(sql.Contains("SELECT RELACION_DA")) {
                Check(origin=="RA_TEST","relationship authority is RA");
                table.Columns.Add("RELACION_DA",typeof(uint));table.Rows.Add(10u);
            } else if(sql.Contains("FROM usuarios_da")) {
                Check(origin=="DA_TEST","DA user authority");
                table.Columns.Add("Clave_Usuario",typeof(uint));table.Rows.Add(10u);
            } else if(sql.Contains("information_schema.COLUMNS")) {
                Check(origin=="DA_TEST","index identity must use DA");
                foreach(string col in new[]{"id","CAMPO","ALEAS_CAMPO","IS_REQUIRED_BACKEND","DATA_TYPE","COLUMN_TYPE","NUMERIC_PRECISION","NUMERIC_SCALE","CHARACTER_SET_NAME","CHARACTER_MAXIMUM_LENGTH"})table.Columns.Add(col,typeof(string));
                table.Rows.Add("99","order_number",indexAlias,"0","varchar","varchar(20)",DBNull.Value,DBNull.Value,"utf8","20");
            } else if(sql.Contains("system1")) {
                Check(origin=="DA_TEST","same numeric cabinet in RA must never be consulted");cabinetReads++;
                table.Columns.Add("id",typeof(uint));table.Columns.Add("nombre",typeof(string));
                var reference=parameters[Array.IndexOf(parameters,"@ref")+1];
                if(reference==null || Convert.ToString(reference)=="7")table.Rows.Add(7u,"Synthetic cabinet");
            } else {throw new Exception("unexpected catalog query");}
            return table;
        };
        var adapter=new PerfilImportacionCatalogAdapter("RA_TEST","DA_TEST",query);
        var context=new ContextoPerfilImportacion{UsuarioRef="1",AmbitoRef="2",PuedeAdministrar=true};
        var config=new ConfiguracionPerfilImportacion{GabineteId=7};
        Check(adapter.Consultar(context,"GABINETE",config,1,1,"7").Single().text=="Synthetic cabinet","saved reference label uses authorized catalog");
        Check(adapter.Consultar(context,"GABINETE",config,1,1,"8").Count==0,"missing saved reference is not substituted");
        bool denied=false;try{adapter.Consultar(new ContextoPerfilImportacion{PuedeAdministrar=false},"GABINETE",config,1,1,"7");}catch(PerfilImportacionException){denied=true;}
        Check(denied,"point lookup cannot bypass authorization");
        bool invalidReference=false;try{adapter.Consultar(context,"GABINETE",config,1,1,"1 OR 1=1");}catch(PerfilImportacionException){invalidReference=true;}
        Check(invalidReference,"point lookup validates identity");
        var dto=PerfilImportacionJson.Leer<CatalogoPerfilSolicitud>("{\"catalogo\":\"GABINETE\",\"referencia\":\"7\"}");
        Check(dto.referencia=="7","closed DTO accepts point lookup reference");
        foreach(var emptyAlias in new object[]{DBNull.Value,""," ","0"," 0 "}){
            indexAlias=emptyAlias;
            var field=adapter.Consultar(context,"INDICE",config,1,50).Single();
            Check(field.text=="order_number","legacy empty alias falls back to physical name");
            Check(field.value=="99" && field.Tipo=="TEXTO" && field.Longitud==20,"label does not alter identity or metadata");
        }
        indexAlias="NOMBRE BENEFICARIO";
        Check(adapter.Consultar(context,"INDICE",config,1,1,"99").Single().text=="NOMBRE BENEFICARIO","real alias remains visible in point lookup");
        indexAlias="Order number";
        var rules=new List<ReglaPerfilImportacion>{new ReglaPerfilImportacion{DestinoTipo="INDICE_GABINETE",DestinoRef="99",FuenteTipo="ARCHIVO",LongitudMaxima=20}};
        Check(adapter.ValidarValores(context,config,new List<ValorReglaPerfilImportacion>{new ValorReglaPerfilImportacion{DestinoTipo="INDICE_GABINETE",DestinoRef="99",FuenteTipo="EXPEDIENTE",TipoValor="TEXTO",Valor=null}}).Count==0,"optional cabinet index accepts confirmed classification absence");
        Check(adapter.ValidarValores(context,config,new List<ValorReglaPerfilImportacion>{new ValorReglaPerfilImportacion{DestinoTipo="INDICE_GABINETE",DestinoRef="99",FuenteTipo="EXPEDIENTE",TipoValor="TEXTO",Valor=new string('x',21)}}).Any(x=>x.Codigo=="VALOR_INDICE_INVALIDO"),"inherited label still obeys cabinet length");
        Check(adapter.Validar(context,config,rules,false).Count==0,"DA cabinet and index are valid");
        Check(config.HuellaOrigenGabinete.Length==64 && cabinetReads>0,"source stamped on draft");
        changed=true;
        Check(adapter.Validar(context,config,rules,true).Any(x=>x.Codigo=="ORIGEN_CAMBIADO"),"routing change blocks publication");
        changed=false;rules[0].DestinoRef="7";
        Check(adapter.Validar(context,config,rules,false).Any(x=>x.Codigo=="INDICE_INCOMPATIBLE"),"foreign index rejected");
        var integer=new OpcionPerfilImportacion{Tipo="ENTERO",TipoFisico="tinyint(3) unsigned",Longitud=64};
        Check(PerfilImportacionCatalogAdapter.ValorCompatible(integer,"255"),"unsigned upper boundary");
        Check(!PerfilImportacionCatalogAdapter.ValorCompatible(integer,"256"),"overflow");
        Check(!PerfilImportacionCatalogAdapter.ValorCompatible(integer,"-1"),"unsigned negative");
        var numeric=new OpcionPerfilImportacion{Tipo="DECIMAL",TipoFisico="decimal(5,2)",Precision=5,Escala=2,Longitud=64};
        Check(PerfilImportacionCatalogAdapter.ValorCompatible(numeric,"999.99"),"decimal capacity");
        Check(!PerfilImportacionCatalogAdapter.ValorCompatible(numeric,"1000.00"),"decimal integral overflow");
        Check(!PerfilImportacionCatalogAdapter.ValorCompatible(numeric,"1.234"),"no silent scale truncation");
        foreach(var invalid in new[]{"abc","--1","1.2.3",".","1e2"})
            Check(!PerfilImportacionCatalogAdapter.ValorCompatible(numeric,invalid),"invalid decimal syntax: "+invalid);
        Check(!PerfilImportacionCatalogAdapter.ValorCompatible(new OpcionPerfilImportacion{Tipo="TEXTO",Charset="ascii",Longitud=20},"Órdenes"),"charset representability");
    }
}
