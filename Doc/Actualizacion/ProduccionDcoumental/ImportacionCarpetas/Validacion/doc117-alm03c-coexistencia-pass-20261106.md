# DOC-117 ALM-03C: coexistencia real verificada

**PASS de coexistencia con variante de laboratorio**, después de completar el cierre independiente de solo SELECT. Terminación de la comprobación: 2026-11-07T00:07:40.998Z. Tres documentos, preservación e incorporación física confirmadas; DLL original restaurada y reabierta. **581/659 SELECT acumulados**, sin reiniciar contador.

| Recurso | Documento | Inventario | Intento | Expediente/relación |
| --- | --- | --- | --- | --- |
| Importación A | 16225 | 23323 | 63 CONFIRMADO | 668/1093 |
| Individual | 16226 | 23324 | No aplica | 467/1072 |
| Importación B | 16227 | 23325 | 64 CONFIRMADO | 669/1094 |

El plan39/lote23 utilizó dos originales ya recibidos de677bytes y el PDF individual nuevo del caso. A/individual se solicitaron en sesiones independientes y se esperaron ambos resultados. Sus controles de checkpoint, relaciones, índices e incorporación física pasaron antes del único envío B. La evidencia nativa completa tiene tres entradas y tres salidas exitosas, fijación de ambos intentos y recursos vinculados por SHA. No hubo rechazo nativo de guardas.

El solapamiento de A/individual fue997347ticks a10MHz, **99,7347ms**, dentro del método instrumentado. La aceptación se limita a esa variante y esos recursos; no afirma commits simultáneos, rendimiento del binario ordinario ni XML/SII concurrente. El inventario del usuario141/empresa2 pasó8814→8817. Los conteos gabinete/inventario/índice son1/1/1 para cada expediente nuevo y10/12/12 para el individual, desde9/11/11. Las dos importaciones tienen checkpoints durables y valores finales verificados; la carga individual conserva clasificación, relación, catálogo canónico, índices estándar, metadatos y documentos anteriores.

La ejecución documental original terminó con código2 por DOC111_LOGIN_INPUT_REQUIRED durante la reapertura, después de restaurar la DLL. HTTP200 y postedFields=DIFFERENT indican que el formulario recibido difirió de las entradas efímeras y se pidió un dato obligatorio. No se identifica el campo concreto ni se afirma contraseña incorrecta. El primer cierre independiente sí reabrió la cuenta con el original, reconstruyó la base previa por su SHA exacto y preservó recursos, pero se detuvo a493SELECT por un error del control: esperaba EN_CURSO/EXPEDIENTE_CONFIRMADO.

**Corrección de esa interpretación:** RegistrarResultadoDocumento llama a ConsolidarEnTransaccion en la misma transacción exterior, después de PersistirResultadoDocumento. El resultado correcto es loteCOMPLETADO, dos raícesCOMPLETADA y seis intentos confirmados con fecha de fin. No se invocó una operación adicional para consolidar o reparar datos. Las afirmaciones contrarias de las revisiones previas se conservan como evidencia histórica y quedan corregidas por este informe. Los verificadores antiguos y las concesiones consumidas no deben reutilizarse.

La comprobación final se ejecutó exclusivamente con SELECT, sin login nuevo, documentos o instalación. Verificó estructuralmente el agregado, ambos checkpoints, los tres documentos físicos y la preservación. La base individual se reconstruyó en RAM excluyendo exclusivamente16226/23324 identificado por la salida nativa, y coincidió con el SHA completo de antes de los envíos; no se fabricaron filas ni se persistieron valores del catálogo o secretos.

Contabilidad:339previos +104ejecución +50primer cierre +88cierre final = **581**. El techo659 fue aprobado expresamente al autorizar320adicionales desde339; ambos cierres usaron el mismo saldo y ninguna carga se repitió. La preparación del plan39/lote23 no se repitió.

La DLL original4134f57f… está instalada; diarioRESTORED, dos respaldos nuevos y cuatro históricos idénticos byte a byte. Las912 entradas originales y los originales de staging de lotes22/23 conservan sus hashes. Las huellas de planes/lotes/raíces/archivos/intentos/eventos anteriores coinciden; el caso antiguo A16224/23322/intento58 y B/archivo37 no se repitió, y los intentos protegidos41/52/53 se mantienen dentro de la preservación. Ningún replay, reparación, reconciliación, limpieza o cambio permanente de configuración/permisos/esquema.

Regresión local final **365/365 PASS**,43archivos; compilación completa y ensayo aislado de restauración con los mismos bytes de candidata PASS. No se suman corridas históricas para obtener ese total. Evidencia primaria inmutable: ejecución ec82e738600489f9ec7837a32eca1525, primer cierre y cierre final vinculados por SHA, consola final salida0. El certificado adjunto fija sus huellas, marcas, autorizaciones y resultados.

Trazabilidad: D-06/RQ-06/tarea17.1 acredita la coexistencia observada. D-08/RQ-08/tarea18.1 consolida esa evidencia con ALM-01/02, límites y documentación de gobierno; OpenSpec estricto se verifica en el certificado. Finalizar el checklist local no archiva OpenSpec, publica una entrega ni cambia Jira.
