## Why

Corte de entrega2026-11-20: implementación y pruebas funcionales/nativas completas;3.118/3.187/3.196 tienen sus primarios reales y1.000 documentos están acreditados. La revisión actual no ejecuta nuevos efectos. Revisión final y propuesta de límites en Validacion/doc120-verificacion-final-20261120.md y doc120-aceptacion-limites-final-20261120.md; aceptación expresa6.4/entrega6.6 pendientes. Las descripciones iniciales y propuestas cronológicas inferiores conservan su alcance histórico; no reabren pruebas ya resueltas.


DOC-120 consolida la verificación de importación de carpetas sobre la interfaz definitiva integrada por PR130 en main 8c0773d4371aeb35543bf182c0abdd0319cababa. El cierre DOC-119 conserva evidencia real/local diferenciada; hace falta una matriz única de vigencia y límites que consuma EXP/ALM/COORD/SEG sin repetir cargas por cambiar de ticket. Los dos índices exigidos por Jira no existen en la base inspeccionada.

## What Changes

Auditoria solicitada tras regresiones: revisar transiciones completas y corregir cuatro defectos reproducidos en paginacion entre lotes, borradores de filtros, ayuda durante acuses y estado de revision heredado por consulta. Modelo de consulta propio y regresion amplia local; tareas3.235-3.238, AUDCTX01-04. Sin operaciones reales nuevas.

Video200257: al retomar un lote consultado, cerrar prioriza el lote visible pendiente sobre una preparacion guardada vacia o completada. Seleccion sin confirmar u otro lote guardado se advierten por separado. Cancelar conserva ejecucion; aceptar espera acuse. CTX-EXIT01-03/tareas3.231-3.234; ajuste cliente y pruebas sinteticas locales, sin operaciones reales nuevas.

Video191801: consentimiento del modulo previo a pausar tambien en preparacion/confirmacion; cancelar conserva ejecucion. Una continuacion principal sobre ticket vigente y contador de ultima parte confirmada, con invalidacion al cambiar perfil/seleccion. REV-EXIT01-03/tareas3.227-3.230; cliente/local, sin operaciones reales nuevas.

Aprobacion explicita2026-11-19: control y consentimiento activos se presentan como Pausar y salir, con Cerrar en reposo/pausa/completado. CTL-10/tareas3.224-3.226; solo etiqueta/accesibilidad, espera y negocio existentes conservados.

Video103048: mensajes de continuar/pausa vigentes con render inmediato del cambio de intencion; aviso local compacto con ruta/fase/referencia desplegables y dialogo limitado al viewport. MNS-01-03/tareas3.220-3.223; cliente y pruebas locales, sin operaciones reales nuevas.

Video092932: recuperacion por carpeta/PDF desde aviso local, pausa antes del selector, accion visible despues de pausar/cancelar y Continuar explicito; no reemplaza temporal RECIBIDO ausente ni crea otro lote. ORI-01-03/tareas3.216-3.219, solo ajuste cliente y verificacion local.

Correccion del video090901: dialogo de consentimiento antes de cerrar lote confirmado pendiente; cancelar conserva ejecucion, aceptar espera la solicitud actual y cierra sin otra fase. Contrato CTL-07-09 y tareas3.212-3.215, sin nueva operacion real.

Continuación2026-11-19 tras revisión arquitectónica: política compartida de controles, una transición pendiente, cierre voluntario tras acuse y pausa del envío manual. Se conserva cierre forzado/recuperación por referencia y se verifica localmente sin operaciones reales nuevas. Ver ampliación D-04/D-05/D-10 en design.md.

Dictamen de viabilidad y plan solicitado tras las reglas de independencia: [contrato09](../../../Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/09-procesamiento-independiente-archivos.md). A no depende de la disponibilidad fisica de B para almacenarse; el lote agrupa/proyecta resultados. Un faltante comprobado pausa y exige Continuar con los demas/Pausar para revisar; no omitir ni reintentar por defecto. D-04/D-05/D-07/D-09 y RQ-04/RQ-05/RQ-07/RQ-09 incorporan creacion con un original utilizable, barreras/reserva/V2 focales, estado individual, lectura versionada y costos medibles. Mantener autoridad, expediente confirmado, exclusividad de escritor activo y resultados inciertos. Esta ampliacion sustituye las clausulas de comprobacion fisica cruzada y bloqueo por faltante individual del diseno anterior, conserva las fuentes/evidencias historicas y no convierte COMPLETO_OBLIGATORIO en perfil parcial: completitud se verifica al cierre. Implementacion local autorizada por "implementa",2026-11-18. Contrato cerrado en Diseno/contrato-focal-implementado-doc120.md: pruebas de procedencia y referencias documentales en eventos existentes, indice no unico manual09 (tarea3.197), lectura1/2 y negativo sin invocacion. No se crean tablas/columnas; el indice no se aplica automaticamente. Sin despliegue o nuevas operaciones reales; las tareas3.187/3.196 y las once pendientes anteriores conservan su alcance.



Aprobación2026-11-16: «continua» responde a la solicitud de aprobar el contrato08. Autoriza su implementación local, incluidos scripts aditivos, operaciones segmentadas y lector de plan2; no aplicar migración ni probar negocio real. Esta aprobación sustituye el estado pendiente de la propuesta inmediatamente anterior y completa3.97.

Continuación2026-11-16 sobre el mapa3.97–3.128: preparar el contrato concreto de capacidad y su línea base local. [Propuesta de preparación segmentada](../../../Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/08-preparacion-segmentada-capacidad.md) define borrador versionado, partes idempotentes, sellado, revisión paginada, lectura focal y recuperación por carpeta. Estado PROPUESTO:3.97 exige aprobación de los nuevos contratos y persistencia antes de implementarlos; no modifica aún las restricciones vigentes ni concede DDL, despliegue o ventana real.3.98 registra reproducción local y objetivos de verificación, sin declarar capacidad operativa.

Solicitud2026-11-16 tras video123321: aclarar el error de preparación de selecciones grandes. Publicar el límite JSON vigente como metadato de interfaz, impedir localmente una solicitud que lo exceda y reconocer únicamente señales cerradas ASMX/HTTP413; preservar incertidumbre y privacidad en otras respuestas. No amplía capacidad, límites, contratos de negocio ni autoriza una nueva corrida real o activación IIS.

Aprobación2026-11-15 sobre maqueta563160db: implementar coordinación con Confirmar e importar, pausa y recuperación explícita, con una solicitud por fase y consultas posteriores coherentes; permitir códigos de negocio repetidos sin reutilizar expedientes ni debilitar la idempotencia. Alcance3.72–3.85/3.88–3.92 y preparación3.86; no autoriza prueba real3.87, despliegue, DDL ni configuración futura de políticas. La ampliación de D-04/D-05 sustituye el requisito anterior de un clic por fase para este recorrido.

Diagnósticof23baf65 autorizado: FECHA_INICIAL canónica de10 caracteres rechazada por límite de origen menor. Corregir únicamente ValidarValorResuelto sobre copia (FECHA yyyy-MM-dd/longitud10), preservando entrada original y snapshot. Regresión local29/29 y compilación0 errores.3.30/3.31 completas por diagnóstico/corrección/propuesta;47/51. Lote39 e intento96 no se modifican; recuperación propuesta necesita implementación revisada y autorización separada, sin replay por inferencia.

La ventana autorizada704fde28 reproduce CONFIGURACION_INVALIDA en COMMAND tras1 SELECT, antes de catálogos, sin efectos. Se afina únicamente el auxiliar para clasificar el guard y se prepara un modo de una SELECT RA;26/26 locales. Nueva autorización solicitada, todavía sin respuesta;3.30/3.31 y recuperación siguen abiertas.

Continuación posterior al diagnóstico933961f9: preparar un auxiliar de prerrequisitos que reutiliza comando/catálogo reales y queryRows, con IPC efímero, etapas/códigos públicos y ningún escritor. Copia contrafactual explícita anterior a la detención; no prueba sesión/transacción ni permite replay.23/23 locales; nueva ventana propuesta de hasta30 SELECT RA/DA requiere autorización propia y conserva6 anteriores. Tareas3.29–3.31; causa histórica y recuperación siguen abiertas.

Video194059,2026-11-14: conservar el contexto de creación sin confirmación, priorizar el recurso que bloquea el lote, mantener visible el aviso y reemplazar el ciclo principal de actualización por Revisar bloqueo. Conservar autoridad de reconciliación existente y distinguir ejecución activa de resultado incierto verificable. Continuación y fin solo por confirmaciones del servidor. La primera ventana64c70664 quedó STOPPED; el auxiliar corrigió el reporte seguro con12/12 locales. La nueva ventana autorizada933961f9 concluyó5 SELECT,0 HTTP/efectos: creación96 RESERVADO, sin checkpoint de inicio ni identidad registrada; recepción95 confirmada y detención posterior. Diagnóstico de estado3.27 completo,44/48; causa precisa y recuperación del lote real siguen sin resolver. No se repite creación ni se liberan reclamaciones por inferencia.

Corrección de fidelidad tras2642e604: sustituir el aspecto anterior del asistente por los controles y distribución del HTML de referencia; Consultar importación y Consultar último lote permanecen visibles en todas las etapas. Consulta secundaria con preparación conservada y File aislados por contexto, selección de recurso sin efectos y comparación visual automatizada. Tareas3.19–3.23 y dos escenarios RQ-04 adicionales delimitan esta solicitud, sin ampliar backend, datos o capacidades de negocio.

Ampliación autorizada2026-11-13: implementar la organización de interfaz contrastada con el repositorio (Seleccionar/Revisar/Seguimiento), corrigiendo C01/C02/C03 de la maqueta y conservando contratos, fases explícitas, filtros, consultas, CSV y recuperación. El mapa atómico3.10–3.18 y los escenarios añadidos en RQ-04 delimitan esta presentación; no incorporan unión/separación PDF ni alteran servidor, esquema, SII o almacenamiento. La evidencia local posterior conserva los cortes históricos y no concede aceptación integral.

- Consolidar índices y dependencias por requisito/caso, con SHA/ambiente y clasificación real/local/estática/no ejecutada.
- Seleccionar regresión focal de interfaz, contratos compartidos, integridad y recuperación según huellas y brechas.
- Delimitar INT-02 sin extrapolar el antecedente de una tabla ausente; preparar INT-03 con contrato de medición y autorización propios.
- Corregir producto únicamente ante un fallo reproducido; conservar SII/almacenamiento/esquema, PDF y decisión de operar sin limpieza.

## Jira Details

> Alcance refinado
> Consolidación de verificación integral y brechas transversales; no repetir desarrollo ni todas las cargas de DOC110–119. La consolidación final se realiza después de la reorganización de interfaz incluida en DOC-119 y verifica la versión resultante con regresión focal.
> Base y procedencia
> Revisión arquitectónica 2026-11-04 solicitada por el usuario frente a lo ya implementado. Base entregada: PR125 integrado en main, merge a3c7263763a89901ba78d05ccebc284a43a4cd76. DOC110–115 están Listo. Esta revisión es documental: no ejecuta nuevas pruebas ni certifica escenarios faltantes. Los resultados históricos conservan su SHA y ambiente; no equivalen a una nueva corrida del commit actual.
> Implementación existente comprobada
> DOC111: REC EXPEDIENTE/LOTE/PARCIAL, IDs/índices y replays, recuperación real del lote2, degradación de una tabla y carga individual. DOC113: UI01/02/03. DOC114: PLAN01/02/03 con permisos restaurados. DOC115: RECIB01/02/03 y 07A con conexiones independientes. Estas evidencias conservan su alcance: no acreditan capacidad máxima, limpieza real, SII real ni todos los escenarios de efectos en vuelo.
> Rutas que deben reutilizarse
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-matriz-final-20261102.md
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc115-verificacion-final-20261104.md
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/tools/e2e/tests/doc111-importacion-carpetas.spec.cjs
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/tools/e2e/tests/doc113-ventana-importacion.spec.cjs
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/tools/e2e/tests/doc114-plan-importacion.spec.cjs
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/tools/e2e/tests/doc115-recepcion-importacion.spec.cjs
> Evidencia existente y su alcance
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-matriz-final-20261102.md
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-cierre-tecnico-20261102.md
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc113-verificacion-final-20261103.md
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc114-final-report-20261103.md
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc115-verificacion-final-20261104.md
> Escenarios y trabajo pendiente
> INT-01 — consolidar matriz única requisito→caso→componente→prueba→SHA/ambiente→resultado real/local/estático/NO_EJECUTADO. Reutilizar evidencia verificada y consumir resultados de EXP/ALM/COORD/SEG; resolver faltantes críticos y contradicciones. No heredar PASS solo por pertenecer al mismo módulo ni ejecutar ciegamente todos los prompts.
> INT-02 — REUTILIZABLE CON LÍMITE: degradación DOC111 probó ausencia de ra_integracion_evento, recuperación y carga individual. Cubrir solo variantes de incompatibilidad realmente exigidas sin extrapolar una tabla a todos los fallos de esquema. SQL manual y estado aplicado verificado; no reinstalar migraciones existentes.
> INT-03 — PENDIENTE DE DEFINICIÓN/AUTORIZACIÓN: acordar cantidad/tamaño de archivos, navegadores, concurrencia, disco temporal y umbrales de tiempo/memoria antes de medir. Ejecutar únicamente presupuesto aprobado. Sin objetivo o ambiente, registrar pendiente y no prometer capacidad máxima.
> Regresión focal — confirmar matriz completa/parcial, valores diferidos/fecha estable, revocación, detención e incertidumbre con las brechas de DOC116–119; revisar diff/SII y carga individual pertinente. SII real no ejecutado debe permanecer declarado. Mantener fuera el aislamiento XML adicional descartado y los formatos no soportados.
> Actualidad de evidencia — comparar huellas de componentes; un cambio posterior invalida solo los casos afectados. CI opsxj/OpenSpec es complementario y no ejecuta todos los E2E/documentos. No atribuir las 93 pruebas Node ni 2 Framework de DOC115 a una corrida integral nueva.
> Criterio de cierre
> Mantener Validacion/matriz-verificacion.md y Validacion/resultados.md como índices de evidencia existente; evitar otra máquina o plataforma de pruebas. Cada no ejecutado debe tener motivo, responsable y efecto sobre la salida a operación. No declarar listo si quedan brechas críticas de autoridad/integridad/recuperación; los límites aceptados deben quedar explícitos.
> Reglas comunes de ejecución
> Reutilizar modelos, repositorio operativo, tablas, ASMX/ASHX, adaptadores, coordinador y herramientas E2E existentes. Antes de programar, contrastar cada criterio con código y evidencia; modificar producto solo ante una brecha reproducida y cubrir su regresión. Leer AGENTS.md y tools/e2e/AGENT-RUNBOOK.md antes de autenticar. Pruebas reales, cambios de permisos, fallos inyectados, limpieza y carga requieren autorización específica de ambiente, cuentas y recursos; este ticket no la concede. Controles RA/DA exclusivamente SELECT, secretos efímeros, evidencia saneada. No repetir efectos inciertos ni cargas ya acreditadas sin motivo.
> Decisiones vigentes que sustituyen exigencias históricas
> No modificar AlmacenamientoDocumentoProduccionDocumental ni SII. Se conserva PDF como formato soportado por este adaptador; TIFF/Multi-TIFF deben rechazarse, no implementarse para satisfacer el prompt anterior. El aislamiento adicional del XML posterior al commit frente a otras cargas fue descartado expresamente por el usuario: queda fuera del alcance y no se afirma como garantía. No habilitar workers, migraciones, permisos, cuotas globales ni limpieza automática por inferencia. Preservar EXPEDIENTE/LOTE y PARCIAL_PERMITIDO/NO_APLICA, snapshot, inventario original y efectos confirmados. Sin rollback global.
> Fuente transversal
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-matriz-final-20261102.md
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc115-verificacion-final-20261104.md
> Secuencia de verificación final — acuerdo 2026-11-10
> Orden acordado: seguimiento operativo de DOC-119 → reorganización de menús como fase final de DOC-119 (UI01–05) → consolidación y cierre final de DOC-120. Se puede preparar la matriz antes, pero la aprobación integral debe corresponder a la interfaz definitiva. No convertir DOC-120 en otro desarrollo de seguimiento ni cerrar la verificación antes del cambio visual previsto.
> Actualizar la base de contraste: las referencias a a3c7263763a89901ba78d05ccebc284a43a4cd76 son históricas. main 9faa8a35106d8f35ca1164f6d580596b36cd4c06 incorpora el cierre técnico de DOC-118 y PR128; consumir también los resultados efectivos de DOC116–119, con su SHA y ambiente. La ejecución posterior registrará su propio SHA; ningún número histórico de pruebas se presenta como una nueva validación integral.
> Regresión de la interfaz definitiva y vigencia de evidencia
> Incluir en INT01 y en la regresión focal los casos UI01–05 de DOC-119: menú superior y contextual, destino seleccionado/pulsado y ruta canónica, permisos, consulta por referencia, avisos y recuperación de pendientes, recarga/reapertura/postback, conservación de selección y actualización del árbol, teclado/foco y menú completamente visible. Si cambió el componente contextual compartido, acreditar los consumidores afectados y la carga individual pertinente.
> Comparar huellas/diff y mantener la evidencia que siga siendo aplicable. Reejecutar solo los casos afectados por cambios o brechas; no repetir todas las cargas DOC110–119. Si la interfaz cambia después de una corrida de DOC-120, registrar el nuevo SHA, revisar el impacto y actualizar los resultados afectados antes de declarar lista la versión resultante.
> El cierre mantiene todos los criterios originales de INT01/02/03: matriz única y resultados trazables, incompatibilidades realmente exigidas, capacidad con presupuesto definido y límites explícitos. No declarar capacidad máxima, limpieza real o SII real sin evidencia. Las brechas críticas de destino/autoridad/integridad/recuperación impiden declarar lista la versión.
> Límites del ajuste de alcance
> La reorganización usa el asistente y los servicios existentes; no autoriza cambios de almacenamiento, SII, esquema, formatos soportados, jobs o limpieza automática. Mantener la decisión explícita sobre SEG03 de DOC-119. Conservar las reglas originales de autorización para pruebas reales, carga, permisos y fallos, controles exclusivamente SELECT y secretos efímeros.

## Capabilities

### New Capabilities
- produccion-importacion-carga-carpeta-verificacion: consolidación trazable de aceptación, vigencia y regresión focal sobre la interfaz definitiva.

### Modified Capabilities
Ninguna modificación funcional incondicional. Una brecha exige reproducción y tarea acotada antes de corregirla.

## Impact

Documentación de Validacion y dossier OPSXJ; pruebas existentes WebForms/VB/JavaScript seleccionadas por impacto. Sin endpoints/DTO/tablas/plataforma de pruebas nuevos, instalación o ejecución real en esta fase.

## E2E existente con Chromium visible -2026-11-19

Solicitud expresa del usuario: observar interaccion en Chromium y reutilizar E2E. run-doc119-seguimiento.cjs conserva controles, presupuestos y operaciones solo lectura; --headed habilita ventana visible y slowMo300ms, sin ampliar guardias ni efectos. Nueva autorizacion de ventana antes de credenciales/autenticacion. Conserva modo anterior, --check y referencias de consulta; no reabre estados cerrados. Consulta/filtros/CSV/privacidad/recuperacion de importaciones existentes, sin confirmar planes, enviar PDF, crear expedientes o almacenar documentos. Esta ventana no valida pausa durante guardado; ese caso requiere presupuesto de efectos propio. Credenciales solo en consola privada. Cinco casos nuevos de argumentos/presentacion/consentimiento y regresion de guardias existentes; abrir consola no equivale a E2E aprobada.

Parametros del video214029: --lot permite seleccionar el ID canonico52 del catalogo autorizado de la cuenta, sin convertirlo en indice de menu, reutilizar sesion interrumpida o elegir otro si falta. La recuperacion tambien queda limitada al lote solicitado. PRUEBA-IMPORT/1 carpeta/1000 archivos/42,8 MB/EN_CURSO son datos visibles de referencia; Egresos v3 pertenece a seleccion previa y no sustituye la version confirmada del lote consultado. Se validan datos actuales del servidor. Ventana nueva con credenciales privadas y consentimiento; solo lectura, sin certificar guardados por esa ventana.

Consulta secundaria conservada tras E2E97c9d4c3: si la pagina visible solo contiene archivos completados pero hay pendientes fuera de pagina, el pie coordinado no debe eliminar Actualizar estado al reemplazar su accion principal por Continuar importacion. Devolver ese mismo boton al query-control, sin duplicar acciones ni iniciar operaciones. El auxiliar exige el control disponible y traduce su timeout a DOC119_RESULT_CONTROL_NOT_READY, sin mensaje privado. Ventana real STOPPED preservada; reproduccion local de la ausencia del control y regresion antes de otra ventana, sin reintentos automaticos.

## Prioridad E2E de guardado real, pausa y cierre

El usuario prioriza efectos reales del lote52 propietario141/empresa2. Ventana propuesta y ejecutor:4 ALMACENAR sin reintento; hasta4 recepciones PDF/96MiB y4 MATERIALIZAR_VALORES solo si los cuatro proximos pendientes los requieren. Expediente existente obligatorio; no crear planes/lotes/expedientes ni cambiar perfil, borrar, reconciliar o levantar detencion administrativa.1200 HTTP/80 SELECT/1 sesion/45min para nuevas operaciones; las solicitudes ya iniciadas esperan respuesta. Reutilizar login, consultas/controles SQL y verificaciones de documento existentes con Chromium visible. Seleccion exacta de los cuatro proximos pendientes en orden, sin saltar un archivo no elegible; validar todos los originales locales antes del primer envio.

Casos prioritarios: A guardar y pausar antes de admitir B; B continuar y cancelar cierre sin pausar, comprobando que C inicia automaticamente; C aceptar cierre y esperar su respuesta antes de salir; D recargar/consultar el mismo lote y continuar con otro archivo, sin replay. Se retiene solo la ENTREGA de respuesta al navegador para hacer observable la espera; la solicitud de almacenamiento y sus efectos son reales. Registrar si la respuesta ya habia llegado del servidor al solicitar pausa/cierre y medir HTTP antes de retencion, sin declarar una transaccion SQL artificialmente abierta. Reanudar corresponde a Continuar importacion tras pausa cliente; no equivale a levantar DETENIDO de servidor.

Cada guardado requiere checkpoint RA e identidad documental/inventario/expediente consistente en DA, conteo incremental sin duplicados y conservacion de los otros996 archivos y expediente/perfil originales. Guardia rechaza fuera de muestra/fase/revision, llamadas simultaneas o repetidas antes de red. Ante fallo, cierra admision y espera solicitudes iniciadas antes de cerrar Chromium; nunca replay. Claves en consola privada; ventana SI requerida antes de acceso. Preparar/abrir consola no es QA aprobada. Las ventanas de lectura anteriores se conservan cerradas y no autorizan efectos por si mismas.
