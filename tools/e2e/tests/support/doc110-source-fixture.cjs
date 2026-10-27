'use strict';
// Contratos de fixtures de prueba; no configuran fuentes en el producto.
function sourceFixture(table='imp_02_egresos'){
  if(table==='imp_02_egresos')return {table,keyColumn:'CONSECUTIVO',removedColumn:'FECHA',removedType:'TEXTO',ambiguousKey:'44'};
  if(table==='imp_02_creditos')return {table,keyColumn:'CONSECUTIVO',removedColumn:'FECHADOCUMENTO',removedType:'FECHA',ambiguousKey:null};
  throw new Error('DOC110_SOURCE_FIXTURE_UNSUPPORTED');
}
module.exports={sourceFixture};
