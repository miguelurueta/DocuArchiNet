using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using GestionDocumental_Docuarchi.net;

// Runs in separate processes against the compiled compatible and producer DLLs.
// Synthetic documents only: no connection, WebForms session or business writer.
public static class ImportacionCarpetasPlanCompatibilityProbe {
    sealed class Bundle {
        public string Legacy,LegacyHash,PreparacionId,PlanHash;
        public PlanImportacionCarpetas Header;
        public List<FragmentoPlanImportacion> Fragments;
    }
    static PlanImportacionCarpetas Plan(ushort schema,int count){
        var p=new PlanImportacionCarpetas {EsquemaDoc=schema,PlanId="12",PreparacionId=schema==2?"77":null,Estado="REVISABLE",Revision=1,Manifiesto=new ManifiestoImportacionCarpetas(),Resultado=new ResultadoPreparacionImportacion(),
            CreadoUtc="2026-11-17T12:00:00Z",ExpiraUtc="2026-11-17T13:00:00Z",
            Contexto=new ContextoImportacionCarpetas{UsuarioRef="1",AmbitoRef="2",NivelRef="3",RutaCanonica="SYNTHETIC",SelloDestino="SYNTHETIC",PuedeImportar=true},
            Snapshot=new SnapshotPerfilImportacionOperativa{PerfilId="4",VersionId="5",Huella=new string('a',64),Configuracion=new ConfiguracionPerfilImportacion()}};
        p.Manifiesto.Raices.Add(new RaizManifiestoImportacion{SeleccionId="s1",IdCliente="r1",Nombre="Synthetic"});
        for(int i=0;i<count;i++){
            p.Manifiesto.Archivos.Add(new ArchivoManifiestoImportacion{IdCliente="a"+i,RaizIdCliente="r1",Nombre=i+".pdf",RutaRelativa="Synthetic/"+i+".pdf",Bytes="677",UltimaModificacionLocal="1234"});
            p.Resultado.Elementos.Add(new ValoresElementoImportacion{Ambito="ARCHIVO",RaizIdCliente="r1",ArchivoIdCliente="a"+i,
                Valores=new List<ValorReglaPerfilImportacion>{new ValorReglaPerfilImportacion{DestinoTipo="INDICE_GABINETE",DestinoRef="9",Valor=new string('\u00f1',200),TipoValor="TEXTO"}}});
        }
        p.Resultado.UnidadesElegibles.Add("r1");p.Resultado.PuedeConfirmar=true;return p;
    }
    public static int Main(string[] args){
        try{
            Bundle b;
            if(args.Length==1){
                var p=Plan(2,1000);p.Huella=PlanSegmentadoImportacionJson.Hexadecimal(PlanSegmentadoImportacionJson.HuellaPlan(p));
                var header=PlanSegmentadoImportacionJson.ClonarPlan(p);header.Manifiesto=new ManifiestoImportacionCarpetas();header.Resultado=new ResultadoPreparacionImportacion();
                var legacy=PerfilImportacionJson.Escribir(Plan(1,1));
                b=new Bundle{Legacy=legacy,LegacyHash=PlanSegmentadoImportacionJson.HuellaTexto(legacy),PreparacionId=p.PreparacionId,PlanHash=p.Huella,Header=header,Fragments=PlanSegmentadoImportacionJson.Fragmentar(p)};
            }else{
                b=JsonConvert.DeserializeObject<Bundle>(File.ReadAllText(args[1]));
                var legacy=PerfilImportacionJson.Leer<PlanImportacionCarpetas>(b.Legacy);
                if(legacy.EsquemaDoc!=1||PlanSegmentadoImportacionJson.HuellaTexto(PerfilImportacionJson.Escribir(PlanSegmentadoImportacionJson.ClonarCompatible(legacy)))!=b.LegacyHash)throw new Exception("Legacy hash changed");
                b.Header.PreparacionId=b.PreparacionId;PlanSegmentadoImportacionJson.Reconstruir(b.Header,b.Fragments);
                var p=PlanSegmentadoImportacionJson.ClonarCompatible(b.Header);
                if(p.Manifiesto.Archivos.Count!=1000||p.Resultado.Elementos.Count!=1000||PlanSegmentadoImportacionJson.Hexadecimal(PlanSegmentadoImportacionJson.HuellaPlan(p))!=b.PlanHash)throw new Exception("Plan2 changed across binaries");
                b.Fragments=PlanSegmentadoImportacionJson.Fragmentar(p);p.Manifiesto=new ManifiestoImportacionCarpetas();p.Resultado=new ResultadoPreparacionImportacion();b.Header=p;
            }
            File.WriteAllText(args[0],JsonConvert.SerializeObject(b));
            Console.WriteLine("PASS_SYNTHETIC_PLAN1_PLAN2_COMPATIBILITY files=1000 schema1Hash="+b.LegacyHash+" schema2Hash="+b.PlanHash);
            return 0;
        }catch(Exception e){Console.Error.WriteLine(e.GetType().Name+": "+e.Message);return 1;}
    }
}
