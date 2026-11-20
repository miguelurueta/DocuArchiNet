# Medición y propuesta de optimización — DOC-120, 2026-11-17

Implementación de3.153/3.154 y propuesta de3.155. La aprobación de espera/reanudación no aprueba reducir las barreras de integridad.

## Medición implementada

El importador emite `DOC120_MEDICION` mediante los Trace.Listeners existentes. No cambia Web.config, listeners ni el escritor compartido. Cada registro contiene categoría cerrada, lote/archivo como referencias numéricas públicas, milisegundos, finalización de la llamada y, cuando existe, diagnóstico público de operación. `finalizo=1` significa que regresó la función; no certifica que se haya guardado un documento. Una excepción conserva su tipo/resultado original aunque falle el listener.

Etapas: TOTAL; CONSULTA_PREFLIGHT (lectura de archivo y vista, registros separados); ORIGINALES_PREVIOS; RESERVA; ORIGINALES_RETENIDOS; ADAPTADOR; DATOS_DOCUMENTALES; ESCRITOR_LEGACY; COMPROBACION_POSTERIOR; CHECKPOINT; LECTURA_FINAL. Las etapas anidadas se superponen: no sumar TOTAL, ADAPTADOR y sus hijos como si fueran intervalos independientes. La comprobación posterior puede no ocurrir si el escritor no devolvió evidencia suficiente. El checkpoint V2 se mide en su frontera propia; el de V1 mantiene su frontera previa.

No se registran PDF, nombres/rutas, índices, SQL, mensajes de excepción, tokens internos, credenciales o conexiones. No se agrega endpoint de medición. Para obtener tiempos reales hace falta un listener operativo autorizado y la versión desplegada; compilar esta versión no produce mediciones del IIS actual.

## Línea base local

`tests/ImportacionCarpetasMedicionBehaviorTests.cs` ejecuta el selector/barrera productivos con cargas de metadatos y puertos de staging sintéticos. Cada corrida representa dos comprobaciones de una operación documental: previa y retenida. Hay una raíz; por eso EXPEDIENTE y LOTE contienen el mismo inventario en este laboratorio.

| Archivos en la unidad | Completo/EXPEDIENTE: cargas/comprobaciones | Completo/LOTE: cargas/comprobaciones | Parcial/NO_APLICA: cargas/comprobaciones |
| --- | --- | --- | --- |
|1|2/2|2/2|2/2|
|100|200/200|200/200|2/2|
|1.000|2.000/2.000|2.000/2.000|2/2|

El PDF representado mide927 bytes: para1.000 originales completos se representan1.854.000 bytes examinados por operación, frente a1.854 en parcial. Son bytes representados por los dobles; **bytes físicos medidos=0, SELECT reales=0 y escritor real=0**. La evidencia JSON conserva los tiempos del laboratorio separados de esas cantidades. El arnés existente también conserva pruebas físicas con PDF local y retención de originales; no constituyen una medición física del corpus completo.

Si durante todo un lote deN archivos permanecieranN originales elegibles/recibidos, el modelo produciría2×N² visitas en ambas pasadas. Es una extrapolación del algoritmo, no un conteo real de consultas ni una causa certificada del timeout observado. El tiempo real también depende de permisos/catálogos, tamaño y ubicación del staging, writer/derivados y checkpoints. No comparar el laboratorio parcial con completo como recomendación de cambiar la política.

## Propuesta para un cambio posterior

Primero medir un documento real con las etapas anteriores, conservando su inventario, tamaño y alcance. Si ORIGINALES_* domina, proponer una vista de metadatos en bloque y una evaluación de autoridad por frontera de fase, manteniendo revalidación inmediatamente antes del efecto. La vista deberá incluir revisiones de lote/raíz/archivo, hash, referencia y tamaño de cada original del alcance; cualquier diferencia invalida el conjunto. El cambio exige pruebas de equivalencia y autorización específica: **todavía no se implementa**.

La revisión física global no se omite ni se sustituye por un sello permanente de «ya revisado». Para reducir lecturas físicas habría que diseñar una retención continua desde la primera comprobación hasta el efecto, con validación fresca del conjunto y liberación garantizada. Deben medirse también el número de handles, bloqueo de cambios y costo de conservarlos. Una caché entre solicitudes o entre usuarios sería otro contrato y queda fuera de esta propuesta.

Antes de optimizar: cubrir original cambiado/faltante, revisión nueva, permiso revocado, incorporación concurrente, pausa/detención, excepción al adquirir o liberar retenciones y respuesta perdida del writer/checkpoint. El conjunto completo debe seguir bloqueando si falta cualquier original exigido. Si ESCRITOR_LEGACY domina, estudiar esa dependencia por separado: esta entrega solo la mide y no modifica `AlmacenamientoDocumentoProduccionDocumental`.

## Criterio de aprobación futura

Comparar versión actual/propuesta con el mismo corpus y política, medir SELECT internos, lecturas/bytes físicos, handles y tiempos por etapa. Rechazo de todos los casos de integridad anteriores, cero duplicados y preservación de resultados son requisitos previos a aceptar una mejora. El diagnóstico cerrado del lote50 no concede permiso para ejecutar esta comparación.
