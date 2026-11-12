# Jira Context - DOC-119

## Summary

PRODUCCION-IMPORTACION-CARGA-CARPETA-SEGUIMIENTO

## Description

> Alcance refinado
> Validación operativa del seguimiento existente: lectura/CSV, recuperación del cliente y árbol. Limpieza permanece condicionada a una decisión explícita de retención y habilitación. Incluye, como fase final posterior a la validación operativa, la reorganización acotada de accesos de importación y consulta en el menú superior y contextual de cada nivel, conservando el asistente y la recuperación existentes.
> Base y procedencia
> Revisión arquitectónica 2026-11-04 solicitada por el usuario frente a lo ya implementado. Base entregada: PR125 integrado en main, merge a3c7263763a89901ba78d05ccebc284a43a4cd76. DOC110–115 están Listo. Esta revisión es documental: no ejecuta nuevas pruebas ni certifica escenarios faltantes. Los resultados históricos conservan su SHA y ambiente; no equivalen a una nueva corrida del commit actual.
> Implementación existente comprobada
> Consulta autorizada, filtros/páginas, totales originales, CSV seguro por página, recuperación por IDs/sessionStorage, reselección y actualización por relación/ID sin cambiar selección. Consultar/exportar no ejecuta reconciliación. DOC113 acredita apertura, preparación, foco/teclado/postback/reapertura; no acredita todos los estados del seguimiento. Limpieza interna existe con diario y verificación; la composición actual usa por defecto política deshabilitada y permite inyección explícita. No hay endpoint, botón ni job de borrado. El informe local histórico menciona claves de configuración: no asumirlas instaladas ni añadirlas a Web.config.
> Rutas que deben reutilizarse
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Services/ProduccionDocumental/ImportacionCarpetas/ServicioConsultaImportacion.vb
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Services/ProduccionDocumental/ImportacionCarpetas/ServicioConsultaImportacion.Limpieza.vb
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Infrastructure/ProduccionDocumental/ImportacionCarpetas/ComposicionLimpiezaOriginalImportacion.vb
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/js/gestion/ImportacionCarpetas/ImportacionCarpetas.js
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/js/gestion/ImportacionCarpetas/ImportacionCarpetasVista.js
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/js/gestion/WebFormProducionDocumental.js
> Evidencia existente y su alcance
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-consulta-exportacion-local-20261028.md
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-recuperacion-local-20261028.md
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-cliente-rama-local-20261030.md
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-limpieza-local-20261030.md
> Escenarios y trabajo pendiente
> SEG-01 — LOCAL + consultas reales REC: falta acreditar el recorrido integrado de filtros/paginación/descarga CSV y usuario ajeno con estados mixtos. Verificar lectura sin mutación con SELECT, totalOriginal separado de totalFiltrado, recibido frente a almacenado y fórmulas CSV neutralizadas. La exportación vigente es de una página; no presentarla como exportación total.
> SEG-02 — COBERTURA PARCIAL: REC acredita reapertura por IDs; navegador local acredita reselección, homónimos y árbol con helper real/servicio sintético. Completar recarga autenticada con recepción pendiente y actualización deduplicada del nivel original, conservando selección. Metadatos no prueban igualdad: bytes/hash/autoridad se verifican en servidor.
> SEG-03 — CONDICIONADO, NO_EJECUTADO REAL: limpieza local protege activos/inciertos/permanentes y solo retira el original .bin elegible, sin poda de .part/historial. Antes de ejecución real se requiere política de retención, mecanismo operativo interno concreto y autorización específica. Si se acuerda operación sin limpieza habilitada, documentar ese límite y consumo de disco; no marcar SEG03 ejecutado ni inventar plazo/borrado automático.
> Guía operativa — actualizar pasos de consulta, CSV paginado, selección local pendiente, incertidumbre, detención y recuperación de DOC118. Separar comandos de lectura de efectos. No crear un segundo coordinador ni un planificador web.
> Criterio de cierre
> Cerrar SEG01/02 con evidencia integrada y resolver explícitamente el alcance/estado de SEG03. Registrar discrepancias documentales contra código actual. Mantener resultados y temporales de pruebas conservados; este ticket no autoriza su eliminación.
> Reglas comunes de ejecución
> Reutilizar modelos, repositorio operativo, tablas, ASMX/ASHX, adaptadores, coordinador y herramientas E2E existentes. Antes de programar, contrastar cada criterio con código y evidencia; modificar producto solo ante una brecha reproducida y cubrir su regresión. Leer AGENTS.md y tools/e2e/AGENT-RUNBOOK.md antes de autenticar. Pruebas reales, cambios de permisos, fallos inyectados, limpieza y carga requieren autorización específica de ambiente, cuentas y recursos; este ticket no la concede. Controles RA/DA exclusivamente SELECT, secretos efímeros, evidencia saneada. No repetir efectos inciertos ni cargas ya acreditadas sin motivo.
> Decisiones vigentes que sustituyen exigencias históricas
> No modificar AlmacenamientoDocumentoProduccionDocumental ni SII. Se conserva PDF como formato soportado por este adaptador; TIFF/Multi-TIFF deben rechazarse, no implementarse para satisfacer el prompt anterior. El aislamiento adicional del XML posterior al commit frente a otras cargas fue descartado expresamente por el usuario: queda fuera del alcance y no se afirma como garantía. No habilitar workers, migraciones, permisos, cuotas globales ni limpieza automática por inferencia. Preservar EXPEDIENTE/LOTE y PARCIAL_PERMITIDO/NO_APLICA, snapshot, inventario original y efectos confirmados. Sin rollback global.
> Fuente transversal
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-matriz-final-20261102.md
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc115-verificacion-final-20261104.md
> Acuerdo de alcance UI/UX — 2026-11-10
> Actualización autorizada por el usuario tras revisar la grabación de Producción Documental. Secuencia: acreditar primero el seguimiento operativo de DOC-119; ejecutar al final de DOC-119 la reorganización de accesos descrita aquí; después consolidar y cerrar DOC-120 sobre la interfaz resultante. Esta fase queda dentro de DOC-119, sin crear otro ticket ni trasladar SEG01/02 a DOC-120.
> La base a3c7263763a89901ba78d05ccebc284a43a4cd76 citada arriba conserva su carácter histórico. El punto de contraste actual es main 9faa8a35106d8f35ca1164f6d580596b36cd4c06, posterior al cierre de DOC-118 y al merge de PR128. Antes de ejecutar, registrar el SHA real y contrastar las evidencias de DOC116–118; no atribuir resultados históricos a una nueva corrida.
> Fase final de DOC-119 — reorganización acotada de interfaz
> UI-01 — Integrar los accesos operativos en el menú Importación existente: Importar carpetas… y Consultar importación…. Separar visualmente perfiles/proveedores/fuentes y respetar sus permisos actuales. Retirar la franja permanente de controles de importación cuando sus funciones tengan un acceso equivalente verificado. La consulta por referencia pasa a su diálogo o vista de consulta; no se elimina.
> UI-02 — Ofrecer Importar carpetas aquí… y el acceso a consulta pertinente en el menú contextual de cada nivel elegible y autorizado. El menú superior toma el nivel seleccionado; el contextual toma explícitamente el nivel pulsado, aunque antes estuviera seleccionado otro. No aplicar la acción indiscriminadamente a expedientes, documentos o zonas vacías. Mostrar el nivel y la ruta canónica autorizada en el asistente; impedir cambios silenciosos de destino.
> UI-03 — Reutilizar el mismo asistente, servicios, contratos y controles de autoridad. Conservar consulta por referencia, último lote y verificación de confirmación pendiente, además de recuperación por IDs/sessionStorage, reselección y actualización deduplicada del árbol sin perder selección. Los pendientes relevantes mantienen aviso visible. Esta reorganización no exige construir un historial nuevo ni ampliar el contrato de consulta; la consulta contextual debe respetar el contexto y la autoridad existentes.
> UI-04 — Corregir el recorte del menú contextual dentro del área visible, con ajuste de posición y desplazamiento cuando su altura lo requiera. Permitir apertura por teclado, navegación, Escape y retorno del foco. Revisar los otros consumidores si se modifica jquery.contextMenu.js, porque es un componente compartido.
> UI-05 — Verificar ambos accesos, nivel seleccionado frente a nivel pulsado, niveles homónimos, falta de selección, permisos denegados/revocados, reapertura/postback/recarga, consulta y recuperación de confirmación pendiente, foco/teclado y visibilidad del menú. Reutilizar las pruebas existentes y ampliar solo los casos afectados; comprobar también los menús compartidos y la carga individual pertinente. No repetir cargas ni efectos inciertos sin motivo. Registrar resultado, SHA, ambiente y límites reales.
> Cierre y límites complementarios de DOC-119
> Mantener SEG01/02 y resolver explícitamente SEG03 según el criterio original. Un defecto reproducible de destino, autoridad, acceso o recuperación se corrige y verifica durante DOC-119; no se difiere bajo la etiqueta de mejora visual. La reorganización UI01–05 se realiza después de la validación operativa y queda verificada antes del cierre final de DOC-120. Actualizar la guía operativa con la navegación definitiva.
> No modificar AlmacenamientoDocumentoProduccionDocumental, SII, esquema de base de datos ni semántica de recepción/confirmación/recuperación para implementar esta reorganización. No habilitar limpieza ni jobs. Las reglas de autorización de pruebas reales, retención, SELECT y secretos siguen vigentes; esta edición de Jira no autoriza ejecutar cargas, fallos inyectados o borrados.

## Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: CARGA, CARPETA, EXPEDIENTE, IMPORTACION, PRODUCION, SEGUIMIENTO
