## Why

DOC-120 consolida la verificación de importación de carpetas sobre la interfaz definitiva integrada por PR130 en main 8c0773d4371aeb35543bf182c0abdd0319cababa. El cierre DOC-119 conserva evidencia real/local diferenciada; hace falta una matriz única de vigencia y límites que consuma EXP/ALM/COORD/SEG sin repetir cargas por cambiar de ticket. Los dos índices exigidos por Jira no existen en la base inspeccionada.

## What Changes

Video194059,2026-11-14: conservar el contexto de creación sin confirmación, priorizar el recurso que bloquea el lote, mantener visible el aviso y reemplazar el ciclo principal de actualización por Revisar bloqueo. Conservar autoridad de reconciliación existente y distinguir ejecución activa de resultado incierto verificable. Continuación y fin solo por confirmaciones del servidor. La ventana autorizada64c70664 se detuvo en el primer intento contabilizado, sin filas guardadas ni causa ODBC conservada. El auxiliar corrige el descarte de categorías seguras y distingue intentos de lecturas concluidas;12/12 pruebas locales, sin repetir la corrida. Otra ventana de hasta5 SELECT, cero HTTP/efectos, requiere instrucción expresa y conserva1 intento previo. No se afirma recuperar el lote real mediante el ajuste de UI.

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
