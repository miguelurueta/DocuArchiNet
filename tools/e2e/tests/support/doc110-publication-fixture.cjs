'use strict';

// Descubre referencias por el catálogo autorizado; solo crea configuración sintética.
// No crea catálogos ni expedientes. Presupuesto explícito para no recorrer sin límite.
async function discoverFixture(call){
  let queries=0;
  async function catalog(name,config){
    const result=[];
    for(let page=1;page<=10;page++){
      if(++queries>120)throw new Error('DOC110_FIXTURE_CATALOG_BUDGET');
      const response=await call('ConsultarCatalogo',{catalogo:name,configuracion:config,pagina:page,tamanoPagina:50});
      if(!Array.isArray(response.datos))throw new Error('DOC110_FIXTURE_CATALOG_INVALID');
      result.push(...response.datos);
      if(response.datos.length<50)return result;
    }
    throw new Error('DOC110_FIXTURE_CATALOG_PAGE_LIMIT');
  }
  function first(rows,label){if(!rows.length)throw new Error('DOC110_FIXTURE_MISSING_'+label);return rows[0].value;}
  let config={Esquema:1,NombreVisible:'Publicación sintética DOC-110',OrigenGabinete:'DA',
    ModoIntegridad:'COMPLETO_OBLIGATORIO',AmbitoIntegridad:'EXPEDIENTE',
    EstrategiaCarpetas:'RAIZ_EXPEDIENTE',PoliticaSubcarpetas:'INCLUIR',
    PoliticaCarpetasVacias:'RECHAZAR',PoliticaArchivosSueltos:'RECHAZAR',
    PoliticaCoincidencias:'BLOQUEAR',Extensiones:['pdf'],MaximoBytesArchivo:1048576};
  config.FondoRef=first(await catalog('FONDO',config),'FONDO');
  config.TipoExpedienteRef=first(await catalog('TIPO_EXPEDIENTE',config),'TIPO_EXPEDIENTE');
  config.UnidadDocumentalRef=first(await catalog('UNIDAD_DOCUMENTAL',config),'UNIDAD_DOCUMENTAL');
  let typology=null;
  search:for(const instrument of await catalog('INSTRUMENTO',config)){
    const ci={...config,InstrumentoRef:instrument.value};
    for(const area of await catalog('AREA',ci)){
      const ca={...ci,AreaRef:area.value};
      for(const series of await catalog('SERIE',ca)){
        const cs={...ca,SerieRef:series.value,SubserieRef:null};
        let options=await catalog('TIPOLOGIA',cs);
        if(options.length){config=cs;typology=options[0].value;break search;}
        for(const sub of await catalog('SUBSERIE',cs)){
          const css={...cs,SubserieRef:sub.value};
          options=await catalog('TIPOLOGIA',css);
          if(options.length){config=css;typology=options[0].value;break search;}
        }
      }
    }
  }
  if(!typology)throw new Error('DOC110_FIXTURE_NO_CLASSIFICATION_WITH_TYPOLOGY');
  let indices=null;
  for(const cabinet of await catalog('GABINETE',config)){
    const candidate={...config,GabineteId:Number(cabinet.value)};
    const fields=await catalog('INDICE',candidate);
    const supported=f=>['TEXTO','ENTERO','DECIMAL','FECHA'].includes(f.Tipo) && f.Longitud>0;
    if(fields.some(f=>f.Requerida && !supported(f)))continue;
    const usable=fields.filter(supported);
    if(!usable.length)continue;
    indices=usable.filter(f=>f.Requerida);
    if(!indices.length)indices=[usable[0]];
    config=candidate;break;
  }
  if(!indices)throw new Error('DOC110_FIXTURE_NO_COMPATIBLE_DA_CABINET');
  function rule(destination,ref,source,type,extra={}){
    return {DestinoTipo:destination,DestinoRef:ref,FuenteTipo:source,TipoValor:type,
      EtapaResolucion:'PREFLIGHT',Obligatoria:true,Orden:0,Extraccion:'COMPLETO',
      Posicion:0,QuitarExtension:false,RecortarEspacios:false,LongitudMaxima:160,...extra};
  }
  const rules=[
    rule('CAMPO_EXPEDIENTE','CODIGO','CARPETA','TEXTO',{LongitudMaxima:45}),
    rule('CAMPO_EXPEDIENTE','NOMBRE','CARPETA','TEXTO'),
    rule('CAMPO_EXPEDIENTE','FECHA_INICIAL','CONSTANTE','FECHA',{ValorConstante:'2026-01-01',FormatoFecha:'yyyy-MM-dd'}),
    rule('TIPOLOGIA','TIPO_DOCUMENTAL','CONSTANTE','REFERENCIA',{ValorConstante:typology})
  ];
  for(const field of indices){
    const value=field.Tipo==='FECHA'?'2026-01-01':field.Tipo==='TEXTO'?'E'.repeat(Math.min(3,field.Longitud)):'0';
    rules.push(rule('INDICE_GABINETE',field.value,'CONSTANTE',field.Tipo,{
      LongitudMaxima:Math.min(4096,field.Longitud),ValorConstante:value,
      ...(field.Tipo==='FECHA'?{FormatoFecha:'yyyy-MM-dd'}:{})
    }));
  }
  return {configuracion:config,reglas:rules};
}
module.exports={discoverFixture};
