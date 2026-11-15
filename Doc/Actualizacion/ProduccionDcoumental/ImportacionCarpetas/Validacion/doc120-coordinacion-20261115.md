# DOC-120 — inicio único, recuperación y códigos repetidos

Solicitud: «aprobado implementa las tareas», sobre la maqueta coordinada y las tareas3.72–3.92. Base563160db5a2b64f013a06cdf62256320e248fea5, carpeta principal, rama feature/DOC-120. Implementación local; no despliegue ni nueva corrida autenticada.

## Resultado implementado

**Confirmar e importar** confirma una vez y coordina las operaciones existentes: recibir pendientes, crear por raíz, materializar valores y almacenar documentos. Usa IDs y revisiones, con una sola mutación activa. Enumera todas las raíces/archivos del lote sin filtros, valida páginas/revisión/ámbito y conserva filtros/página/CSV visibles. Ningún efecto siguiente se solicita sin acuse válido y lectura compatible con lo confirmado.

**Pausar importación** deja terminar la petición iniciada. Cerrar/destruir/cambiar de consulta invalida callbacks y consentimiento; no hay ejecución en segundo plano ni cola persistida. Una nueva consulta no ejecuta efectos; **Continuar importación** reconstruye pendientes y omite resultados ya confirmados. Los originales se vinculan por archivo y raíz del mismo inventario, con nombre/tamaño/fecha/ruta comprobados. Una reselección fuera de la página o filtro visible también es posible y no envía por sí sola.

Pérdida de respuesta: pausa y una lectura del resultado, sin replay. Fallo de lectura posterior al acuse: conserva CONFIRMADA e IDs conocidos. EN_CURSO/INCIERTO, detención, permisos/revisión, presupuesto o lectura contradictoria impiden continuar. RETOMAR_CREACION y RECONCILIAR permanecen explícitos; las operaciones individuales se mantienen en Ver acciones, con Ver avance del lote para volver al recorrido coordinado.

Se muestran fase/recurso, bytes de solicitud y conteos confirmados también en el pie visible durante scroll. El cierre muestra Resultado de la importación, documentos/expedientes confirmados, Ver expediente(s) por ID y Cerrar. Nodo oculto/ambiguo avisa sin inventar un destino. Revisión muestra código previsto y nombre informativo por separado. Los recursos afectados tienen versión ASPX nueva.

La importación permite códigos repetidos en el destino y entre raíces. La evaluación existente conserva metadatos de tipo/longitud/representabilidad pero ya no consulta candidatos de negocio ni compara raíces para imponer exclusividad. Cada nueva raíz mantiene su identidad; no se vincula automáticamente a expedientes anteriores. El editor ofrece PERMITIR; snapshots históricos BLOQUEAR/REUTILIZAR_VALIDADO permanecen legibles sin reescribirlos ni reutilizar implícitamente. Los planes guardados con bloqueos anteriores necesitan nueva revisión. El delegado interno existente conserva la extensión futura, sin implementar todavía su política configurable.

## Verificación local y trazabilidad

**207/207 pruebas Node únicas** en la regresión seleccionada, incluidos los dos wrappers Framework/DDL. Los casos internos del arnés no se suman. Las ejecuciones focales repetidas se superponen y tampoco se agregan a207. Log de la ejecución conjunta: `.tmp/doc120-coordinacion-regresion-final.log`. El refinamiento final de RUN01 comprueba dos carpetas homónimas con código/ruta/nombre de PDF iguales y contenidos locales distintos:9/9 del mismo conjunto repetidas en `.tmp/doc120-coordinacion-homonimos-final.log`, verificando el File correcto por ID sin sumarlas. Compilación aislada de proyecto, cero errores, advertencias heredadas de archivos fuera del cambio: `.tmp/doc120-coordinacion-build/output.log`. OpenSpec estricto y diff verificados. El JSON compañero fija fuentes normalizadas LF y artefactos locales por SHA256.

| Tareas | Comprobación |
| --- | --- |
|3.72/3.88 | Diseño, propuesta, refinamiento y especificación coherentes;40 escenarios. No se cambia el contrato individual de los endpoints. |
|3.73–3.78 | COORD01–15 y RUN01–06: secuencia,27 archivos, barreras EXPEDIENTE/LOTE/PARCIAL, identidad/acuse, presupuesto, doble inicio y dos vistas con revisiones compartidas. Concurrencia simulada, sin afirmar concurrencia MySQL real. |
|3.79–3.82 | RUN01–09: inicio, pausa, cierre, lectura, reapertura, original fuera de filtro/página, acceso por ID, conteos/bytes durante scroll y390/768/1280px; regresión de teclado/foco, navegación y menús existente. |
|3.83–3.85 | Recorrido completo y errores de cada fase; regresión de mensajes, recepción, reanudación, filtros/paginación/CSV, árbol, barra/búsqueda y carga individual. |
|3.89–3.91 | Arnés Framework ejecuta el lector técnico real, preparación y confirmación con códigos repetidos y candidatos posteriores; cero consultas de candidatos. Guardas técnicas, permisos, revisión e idempotencia se mantienen. Editor/fixture administrativo usan PERMITIR; las versiones históricas no se mutan. |
|3.92 | Esquema versionado inspeccionado, compatibilidad histórica documentada; sin DDL ni afirmar estado actual de IIS/MySQL. |

Capturas sintéticas locales: `.tmp/doc120-coordinacion-ui/avance-390.png`, `avance-768.png`, `avance-1280.png` y `resultado.png`. Formulario y CSS reales, XHR sintético y cero solicitudes HTTP a la aplicación. Los auxiliares de publicación administrativa se alinean con PERMITIR; su sintaxis y fixture se verifican localmente, sin ejecutar el ciclo E2E autenticado.

## Compatibilidad física y límites

El archivo versionado `Exploracion/02-evidencia-esquema-perfil.json` contiene19 filas de índices de expediente_archivo: PRIMARY compuesto por ID_EXPEDIENTE (posición1) y CODIGO_UNICO (posición2); las otras17 filas son no únicas. Ese compuesto admite el mismo código con IDs diferentes; no equivale a un UNIQUE independiente de CODIGO_UNICO. No se ha consultado el destino actual ni se han eliminado índices. Antes de afirmar compatibilidad real, la ventana debe cotejar el esquema servido; si difiere, detenerse sin DDL.

Topes del cliente por continuación:10000 recursos,400 páginas por tipo/enumeración,10000 lecturas y10000 efectos. Son límites finitos de seguridad operativa del cliente; no medición de capacidad INT-03 ni nuevas cuotas del servidor.

Sin cambios en SII, esquema, permisos, Web.config, limpieza o escritor compartido. Sin autenticación real, SELECT de negocio, HTTP de aplicación, creación, recepción o almacenamiento reales nuevos. La compilación produce archivos solo en `.tmp`, sin activar IIS.

Avance **106/112**:19 tareas locales de este bloque verificadas.3.86 conserva pendiente su autorización específica y3.87 la ejecución real; [ventana propuesta](doc120-coordinacion-plan-real-20261115.md). También siguen abiertas5.2,6.4,6.5 y6.6 para autorización/capacidad, aceptación y cierre integral. Esta entrega local no las sustituye ni archiva DOC-120.
