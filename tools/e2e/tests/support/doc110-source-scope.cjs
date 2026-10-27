'use strict';
const {expect}=require('@playwright/test');
const {safePost}=require('./doc110-safe-request.cjs');

// Escenario local sin excepciones configuradas. Solo metadatos, nunca filas de negocio.
async function exerciseSourceScope(auth,baseUrl,token){
  async function query(payload,expected=200){
    const response=await safePost(auth.context.request,new URL('webservice/WebServiceImportacionCarpetas.asmx/ConsultarMetadatosFuente',baseUrl).href,{
      headers:{'X-Importacion-Perfil-CSRF':token},data:{solicitud:payload}
    },'ConsultarMetadatosFuente');
    expect(response.status()).toBe(expected);
    let result;try{result=(await response.json()).d;}catch{throw new Error('DOC110_SOURCE_SCOPE_NON_JSON');}
    expect(result?.ok).toBe(expected===200);
    if(expected===403){expect(result.codigo).toBe('ACCESO_DENEGADO');expect(result.datos==null).toBe(true);}
    return result.datos;
  }
  const connections=await query({tipo:'CONEXIONES',pagina:1});
  expect(connections.map(x=>x.value).sort()).toEqual(['DA','RA']);
  for(const origin of ['RA','DA']){
    let finished=false,found=false;
    const names=new Set();
    for(let page=1;page<=100;page++){
      const items=await query({tipo:'TABLAS',conexionRef:origin,pagina:page});
      expect(Array.isArray(items)).toBe(true);expect(items.length).toBeLessThanOrEqual(25);
      for(const item of items){
        // Boolean assertions avoid printing unrelated schema names on failure.
        expect(typeof item.value==='string' && item.value.startsWith('imp_02')).toBe(true);
        expect(names.has(item.value)).toBe(false);names.add(item.value);
        if(item.value==='imp_02_egresos')found=true;
      }
      if(items.length<25){finished=true;break;}
    }
    expect(finished).toBe(true);
    if(origin==='DA')expect(found).toBe(true);
    console.log('DOC110_PASS: SOURCE_PREFIX_PAGINATION_'+origin);
    for(const table of ['doc110_forbidden_probe','impX02_egresos','IMP_02_egresos'])
      await query({tipo:'COLUMNAS',conexionRef:origin,tabla:table,pagina:1},403);
    console.log('DOC110_PASS: SOURCE_OUTSIDE_LITERAL_PREFIX_DENIED_'+origin);
  }
  const columns=await query({tipo:'COLUMNAS',conexionRef:'DA',tabla:'imp_02_egresos',pagina:1});
  expect(columns.map(x=>x.value)).toEqual(['CODIGOTIPO','TIPO','CONSECUTIVO','FECHA','NIT','BENEFICIARIO']);
  expect(columns.every(x=>x.Tipo==='TEXTO' && x.Longitud===250)).toBe(true);
  console.log('DOC110_PASS: SOURCE_EGRESOS_METADATA_ONLY');
}
module.exports={exerciseSourceScope};
