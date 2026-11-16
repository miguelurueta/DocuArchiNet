# DOC-120 — Contrato propuesto de preparación segmentada y lectura focal

Estado2026-11-16: **PROPUESTO PARA REVISIÓN, pendiente de aprobación de3.97**. Este documento concreta la dependencia del mapa3.97–3.128; todavía no sustituye los contratos vigentes ni habilita endpoints, esquema, cuotas o escrituras nuevas. La continuación del usuario autoriza preparar esta propuesta y medir localmente3.98. [Contrato vigente](02-contratos-operacion.md), [tareas](../../../../../openspec/changes/doc-120-produccion-importacion-carga-carpeta-verificacion/tasks.md).

## Resultado visible y alcance

El usuario selecciona la carpeta completa, pulsa Preparar revisión, revisa y pulsa Confirmar e importar una vez. La segmentación queda dentro de Preparar revisión; no añade confirmaciones ni requiere elegir grupos de archivos. Una raíz con1.000 PDF representa un expediente con1.000 documentos cuando todos son elegibles. Varias raíces siguen generando un expediente por raíz; un segmento no constituye una raíz, un lote ni un expediente.

El avance distingue preparación de metadatos, recepción de bytes, creación, preparación de datos documentales y almacenamiento. La revisión no recibe PDF ni crea expedientes. La coordinación conserva una mutación activa, las fases y barreras actuales, los filtros/CSV y las acciones avanzadas. ID_EXPEDIENTE sigue identificando el expediente; CODIGO_UNICO repetible es su etiqueta y el nombre es informativo.

## Causa y límites independientes

1. PrepararPlan actual envía el manifiesto completo. El corpus de1.000 PDF genera172.409 caracteres usando el inventario real del cliente; el preflight detiene el envío con el límite ASMX de referencia102.400. No se ha inspeccionado la configuración de IIS en esta medición.
2. PerfilImportacionJson también clona el plan y serializa/lee manifiesto y resultado completos con límite262.144. Partir solo el POST deja ese problema de persistencia y lectura.
3. readAll enumera el lote entero después de cada efecto. El coste derivado para una raíz/1.000 archivos es43 lecturas por snapshot y129.086 para3.001 efectos. El motor real con transporte/efectos simulados se bloquea después de232 recepciones, sin llegar a crear o almacenar.
4. La reselección actual permite un archivo por vez; recuperar cientos de originales requiere una asociación masiva segura.

Cambios propuestos: transporte segmentado, persistencia versionada, revisión paginada y lectura focal. Se mantienen Web.config, el serializador de perfiles, almacenamiento compartido, recepción PDF, SII y ausencia de limpieza/jobs. No se adoptan envíos paralelos: ASMX habilita sesión y los escritores/staging necesitan análisis separado antes de introducir concurrencia.

## Identidad y modelo persistente propuestos

La preparación obtiene una identidad distinta del plan confirmado. El servidor fija propietario, empresa/RA, nivel, perfil/versión publicada, snapshot, sello de destino, caducidad y totales declarados. La clave de preparación es aleatoria de128 bits y sirve para idempotencia, nunca para autorización. Todos los IDs se transmiten como cadenas decimales con los límites actuales; cantidades/bytes mantienen la representación validada existente.

Se proponen tres tablas aditivas InnoDB/MySQL5.1, mediante un nuevo script manual `Sql/08-preparacion-segmentada-mysql51.sql`; no se modifica el significado ni se reutilizan tablas de raíces/archivos confirmados como borradores:

| Entidad propuesta | Contenido y relaciones | Restricciones principales |
| --- | --- | --- |
| ra_integracion_preparacion | Identidad; ámbito/usuario/nivel; versión de perfil; contexto/configuración canónicos; estado/revisión; clave; cantidades previstas; fechas; plan_id nullable tras sellar | PK propia; clave única por ámbito/usuario/clave; FK perfil-versión; plan_id único/FK al plan cuando exista; índices por contexto/fecha y estado |
| ra_integracion_preparacion_parte | preparacion_id; tipo RAICES/ARCHIVOS; número de parte; metadatos canónicos; hash; estado de validación; sello de catálogos | FK preparación; unicidad preparación/tipo/número; cuerpo acotado; identidad y contenido inmutables tras aceptar |
| ra_integracion_preparacion_resultado | preparacion_id; tipo de resultado; número de fragmento; resultado/incidencias canónicos; hash | FK preparación; unicidad preparación/tipo/número; fragmentación independiente de la entrada para evitar que muchas incidencias desborden una parte |

No se usa JSON nativo de MySQL ni concatenación ilimitada de LONGTEXT. Cada documento se valida con canonización cerrada y límites de profundidad/tipos. Las columnas y los índices definitivos se contrastarán con el detector físico en3.99; la propuesta no concede ejecución DDL.

Al sellar, ra_integracion_plan conserva el contexto/snapshot y recibe esquema_doc=2. manifiesto_doc y resultado_doc contienen referencias versionadas a esa preparación sellada, sus conteos y huellas, no el inventario agregado. El repositorio valida la relación en ambos sentidos, contexto, versión y estado. Los lectores de esquema1 conservan exactamente su comportamiento y huellas; no convierten silenciosamente sus documentos a esquema2.

El hash de plan2 usa dominio/versionado explícito y registros canónicos ordenados por identidad de raíz/archivo, con delimitación de longitudes UTF-8. Incluye contexto/snapshot, inventario, valores, incidencias y elegibilidad global; excluye fronteras y orden de llegada de partes. Debe poder calcularse incrementalmente sin llamar al serializador de perfiles sobre el agregado. Las pruebas cruzadas de orden/particionado y lectura persistida son obligatorias antes de confirmar2.

## Operaciones propuestas sobre el ASMX existente

Todas conservan RespuestaPerfilImportacion, guardas HTTP/sesión y diagnóstico seguro. Los nombres siguientes son el contrato concreto que se somete a aprobación; no están implementados. La autoridad se reconstruye por solicitud. Ninguna clave/hash o referencia del cliente sustituye permisos actuales.

| Operación | Entrada esencial | Salida acotada y semántica |
| --- | --- | --- |
| IniciarPreparacionPlan | nivelRef, versionId, clavePreparacion, totales de raíces/archivos/bytes y partes por tipo | preparacionId, revisión, estado, vencimiento y límites efectivos; contexto/snapshot fijados por servidor. Misma clave y cabecera devuelve la misma preparación; cabecera distinta es conflicto |
| RegistrarPartePreparacion | preparacionId, tipo, número, revisión esperada y metadatos de esa parte | Identidad, hash calculado por servidor, revisión y estado de parte. Misma identidad/contenido es idempotente incluso tras acuse perdido; contenido distinto no sustituye lo aceptado |
| ConsultarPreparacionPlan | preparacionId, tipo de consulta y página | Cabecera/estado y páginas de partes/validaciones confirmadas; permite resolver un acuse perdido o recuperar el borrador, sin reenviar automáticamente |
| ValidarPartePreparacion | preparacionId, tipo/número y revisión esperada | Resultado de validación y referencias de fragmentos, sin devolver todos los valores/incidencias. Solo con todas las raíces registradas; reglas/consultas autorizadas y cache acotada a esta solicitud/snapshot |
| SellarPreparacionPlan | preparacionId, revisión y clave de cierre estable | planId, revisión/hash del plan y resumen global de elegibilidad. Idempotente: mismo cierre devuelve el mismo plan; no confirma el lote |
| ConsultarRevisionPlan | planId/revisión, tipo RESUMEN/VALORES/INCIDENCIAS y página | Resumen global o página limitada de revisión; nunca serializa el plan completo |
| ConsultarRecursoLote | loteId, tipo RAICES/ARCHIVOS, recursoId | Lectura focal coherente definida abajo; no cambia estado ni solicita avance |

ConfirmarPlan, consulta de confirmación y las fases de recepción/creación/valores/almacenamiento mantienen sus identidades y efectos. ConfirmarPlan amplía su lector a esquema2 y conserva transacción, clave, hash, revisión, vigencia y revalidación. Un plan2 produce un solo lote por la unicidad existente sobre plan_id, con todo el inventario autorizado; las inserciones se recorren por segmentos dentro de la misma transacción de confirmación.

## Estados, cierre y consistencia

- BORRADOR admite registro y validación. Partes pendientes/invalidas no habilitan Confirmar plan; la revisión no se anuncia como lista mientras falta trabajo.
- SELLADA enlaza exactamente un plan REVISABLE o BLOQUEADO conforme a elegibilidad/integridad. Las partes, resultados y snapshot quedan inmutables. No se añaden archivos a un plan sellado ni a un lote confirmado.
- EXPIRADA o INVALIDADA impide continuar esa preparación. Catálogos/publicación/sello incompatibles o cambios de selección/perfil/destino requieren nueva revisión; no se mezcla la revisión nueva con partes anteriores.

Se registran primero las partes de raíces, después los archivos; dentro de cada tipo se admite llegada fuera de orden. La validación de archivos exige raíces completas. El cierre comprueba todas las posiciones previstas, cantidades, suma de bytes, pertenencia a raíz, identidades/rutas duplicadas y sellos coherentes. Una raíz puede abarcar muchas partes. Las validaciones locales no reemplazan las invariantes globales.

EXPEDIENTE, LOTE y PARCIAL_PERMITIDO/NO_APLICA se evalúan sobre todo el inventario. Una incidencia tardía puede bloquear una unidad completa según la política actual; no se sella primero una parte elegible como un plan independiente. No se consultan candidatos para imponer exclusividad por CODIGO_UNICO ni se reutilizan expedientes por coincidencia.

Los resultados de parte se calculan contra una revisión de borrador y se publican mediante CAS; un cambio concurrente impide adoptar el resultado obsoleto. No se mantiene una transacción de BD abierta mientras el navegador envía la siguiente parte ni durante todo el tiempo de preparación. El sellado y la confirmación tienen sus propias transacciones cortas de consistencia. No hay timeout que habilite repetir un escritor incierto.

## Límites y presupuestos locales propuestos

| Dimensión | Objetivo de implementación local | Comprobación |
| --- | --- | --- |
| Parte de entrada | Hasta100 entradas de un tipo; cuerpo real con envoltura <=mínimo entre65.536 caracteres UTF-16 y límite efectivo del host | Medir JSON.stringify antes del POST; ajustar cantidad automáticamente. Un elemento indivisible excesivo se rechaza con causa precisa |
| Respuesta nueva | <=65.536 caracteres y nunca mayor al límite efectivo negociado | Medir el sobre serializado, incluidos escape Unicode/incidencias; paginar por tamaño además de cantidad |
| Fragmento persistido | <=262.144 caracteres canónicos por documento; configuraciones conservan su límite actual | Pruebas con resultado mucho mayor que entrada; dividir resultados sin alterar fronteras del inventario lógico |
| Revisión/DOM | Hasta25 filas de detalle visibles por página; totales globales separados |1.000 archivos no crean1.000 filas ni se reconstruyen por cada actualización de bytes |
| Enumeración inicial | B(N,R)=2+ceil(R/25)+ceil(N/25) solicitudes de consulta, con revisión coherente | Para1 raíz/1.000 archivos:43. Contabilizar también el coste inicial si el usuario continúa tras una recarga |
| Lectura posterior | Hasta2 solicitudes focales por efecto ordinario, sin reenumerar el inventario completo | Para3.001 efectos: <=6.045 lecturas incluyendo la inicial; sin elevar el tope actual10.000 |
| Efectos |3N+R para el recorrido ordinario actual: recepción, creación, valores, almacenamiento | Para1.000 archivos/1 raíz:3.001; efectos estrictamente secuenciales |
| CPU Node local | Recorrido completo simulado de1.000 archivos <=10s, sin transporte real | Mismo auxiliar/versión/máquina y sin pausas artificiales; superar el objetivo bloquea declarar cumplido3.120 |
| Memoria Node local | Incremento de heap muestreado <=128MiB para el recorrido completo | Informar método y limitación de muestreo; no equiparar con pico exacto o memoria IIS |
| Chromium local | Hasta2.000 nodos dentro del asistente; actualización de avance p95<=100ms; incremento de heap <=128MiB | Registrar versión/máquina/viewport; medir navegación/avance y memoria de1.000 metadatos sin cargar1.000 bytes PDF en memoria; superar umbral requiere corrección o decisión explícita |
| Capacidad operativa | Instrumentar IIS y SQL por separado; sin umbral operativo inventado | Objetivos operativos y presupuestos de SELECT/HTTP/disco/tiempo requieren el plan3.125 y su autorización |

Estos valores son objetivos de diseño propuestos, no cuotas nuevas aplicadas ni una garantía de aceptación. El máximo100 no asegura que100 entradas quepan: el tamaño real determina el corte. La publicación de capacidades por el host incluye versión de contrato disponible y límites efectivos; es una comprobación técnica, no una lista de cuentas o gate de experiencia.

Mantener los límites actuales de archivo/staging y del coordinador. Si una capacidad necesaria falta o el esquema es incompatible, detener antes de escribir y explicar qué falta; no reintentar mediante un POST monolítico que se sabe excesivo. No se aplican aumentos globales a JSON, HTTP o cuotas para enmascarar el problema.

## Lectura focal y seguridad del siguiente efecto

ConsultarRecursoLote reconstruye autoridad, lee lote y recurso, padre si corresponde y las barreras/contadores globales pertinentes. La respuesta incluye ámbito público de lote/nivel/perfil/versión, revisión de lectura, estado/acciones, detención, intentos activos, incertidumbre, recurso/parentesco y contadores. El propietario/empresa se comprueban internamente, sin exponer datos de otra cuenta.

La lectura debe ser consistente mediante el patrón de transacción/contraste de revisión soportado por el repositorio MySQL5.1. Si no obtiene coherencia, devuelve conflicto y el cliente pausa; no realiza polling. El acuse previo se conserva aunque falle esta lectura. La respuesta no convierte un acuse perdido en permiso para repetir el efecto.

La cola conserva su inventario inicial y actualiza solo el recurso y padre leídos; comprueba identidad, revisión, acciones y valores de acuses anteriores. Detectar alteraciones fuera de la actualización comprobable invalida el avance y exige nueva consulta explícita. Permisos y barreras se revalidan también dentro de la siguiente mutación mediante los mecanismos existentes. La cache del navegador nunca autoriza una escritura.

Los contadores son confirmados por servidor. La implementación debe medir SQL/filas y usar los índices apropiados; reducir llamadas HTTP no acredita por sí solo menor coste de BD. El estado completo del lote procede de servidor, no de deducir que ya se envió el último archivo.

## Recuperación de originales

Con un lote ya confirmado, el usuario elige la raíz pendiente y reselecciona su carpeta completa. La asociación usa el inventario de ese lote/raíz, ruta relativa, nombre, tamaño y lastModified. Los IDs originales conservados evitan mezclar File de otra preparación. Raíces homónimas requieren asociación explícita por raíz confirmada; nombre/código solos no bastan.

Se informa vinculados, faltantes, modificados, duplicados, sobrantes y ya recibidos. La reselección solo conserva referencias File en memoria y nunca confirma, recibe, crea ni almacena. No existe un hash previo conocido para el contenido pendiente; la verificación PDF/hash del servidor ocurre al recibir. Solo Continuar importación inicia los pendientes autorizados.

Pausar espera la solicitud en curso. Cerrar/recargar invalida callbacks y consentimiento; se conserva referencia de preparación/lote, no claves, File, cola activa o permiso persistente. Un borrador se recupera consultando partes aceptadas y reseleccionando el mismo inventario faltante sin cambiar sus identidades; si no puede demostrarse esa correspondencia, se informa que requiere nueva revisión y no se adivinan IDs por orden de selección. Un lote terminado queda de consulta.

## Compatibilidad, despliegue y reversión

El esquema1 conserva lectores, huellas y ConfirmarPlan. La migración propuesta es aditiva y manual; el detector de capacidad segmentada es independiente para no romper consultas de lotes antiguos por faltar tablas nuevas. No se aplica DDL, no se repiten migraciones, no se habilita producción ni se elimina ningún borrador mediante esta propuesta.

Antes de emitir planes2 debe existir una versión compatible de lectores/confirmación que permita regresar la aplicación conservando el soporte2. La reversión soportada vuelve a esa versión compatible y conserva las tablas/datos; no permite instalar un binario anterior que desconozca planes2 mientras haya datos que deba gestionar. El despliegue debe comprobar esta compatibilidad en laboratorio y documentar la versión mínima. No se ofrece downgrade destructivo ni rollback de expedientes/documentos reales.

Sin jobs o limpieza nueva, borradores expirados se retienen conforme a la decisión existente; la operación debe presupuestar disco y registrar el límite. Diseñar una limpieza futura es otro alcance.

## Casos de aceptación propuestos para incorporar tras aprobación

| Caso | Resultado exigido |
| --- | --- |
| CAP-01 |1.000 metadatos se segmentan por tamaño/cantidad y generan una revisión, con revisión previa y sin bytes PDF |
| CAP-02 | Una raíz atraviesa partes y crea un expediente con1.000 documentos; varias raíces conservan sus identidades, incluidos códigos repetidos/homónimos |
| CAP-03 | Parte repetida idéntica conserva identidad; distinta se rechaza; acuse perdido se consulta sin crear otra preparación |
| CAP-04 | Partes incompletas, duplicados/rutas entre partes, contexto o catálogo incompatible impiden sellar/confirmar |
| CAP-05 | Orden y fronteras de partes no alteran resultado/hash canónicos; integridad EXPEDIENTE/LOTE/PARCIAL se conserva |
| CAP-06 | Consulta de revisión y persistencia no pasan por una serialización agregada limitada ni ocultan incidencias |
| CAP-07 | Confirmación perdida se recupera sobre el mismo plan/lote; confirmación concurrente no duplica inventario |
| CAP-08 | Lecturas focales cumplen presupuesto; detención, revocación, revisión ajena y resultado incierto detienen el siguiente efecto |
| CAP-09 | Reseleccionar una carpeta vincula solo los pendientes correctos; modificados/ambiguos no se envían; Continuar es explícito |
| CAP-10 | Pausa, cierre y recarga en las fases actuales conservan resultados, sin replay ni trabajo en segundo plano |
| CAP-11 | Otra cuenta no puede leer/completar/confirmar preparaciones ni acceder al recurso focal por IDs conocidos |
| CAP-12 | Lectores1/2, fuentes protegidas, UI390/768/1280 y reversión compatible pasan sin atribuir simulación a capacidad real |

La prueba real corresponde a3.125–3.127 y5.2: ambiente/cuentas/recursos/efectos y presupuestos expresos, controles exclusivamente SELECT y secretos efímeros. El corpus disponible tiene1.000 PDF/44.859.102 bytes; su preparación en disco no autoriza crear ni almacenar sus objetos en la aplicación.
