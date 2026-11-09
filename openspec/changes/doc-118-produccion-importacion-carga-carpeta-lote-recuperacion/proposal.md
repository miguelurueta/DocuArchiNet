## Why

La evidencia histórica acredita barreras, inventario y varios replays, pero no demuestra aún dos clientes documentales disputando una reclamación ni detención durante una llamada legacy activa. DOC-118 debe cerrar esas brechas sin duplicar infraestructura ni repetir efectos inciertos.

## What Changes

- Contrastar código y evidencia de COORD-01/02/03 con procedencia verificable.
- Definir exclusión por lote, respuesta perdida, antigüedad sin takeover, detención y recuperación limitada.
- Preparar únicamente integraciones faltantes y autorizarlas por separado; corregir producto solo ante una brecha reproducida.
- Preservar arquitectura legacy, PDF, snapshot, inventario y efectos confirmados.

## Jira Details

> Alcance refinado
> Validar y reforzar solo brechas reproducidas de coordinación/recuperación existente. Prioridad: efectos inciertos, dos clientes y detención segura.
> Base y procedencia
> Revisión arquitectónica 2026-11-04 solicitada por el usuario frente a lo ya implementado. Base entregada: PR125 integrado en main, merge a3c7263763a89901ba78d05ccebc284a43a4cd76. DOC110–115 están Listo. Esta revisión es documental: no ejecuta nuevas pruebas ni certifica escenarios faltantes. Los resultados históricos conservan su SHA y ambiente; no equivalen a una nueva corrida del commit actual.
> Implementación existente comprobada
> Una fase por recurso/petición; barreras por EXPEDIENTE/LOTE y parcial; reclamaciones, revisiones, consolidación desde inventario original, detención y reconciliación por evidencia conocida. REC reales cubrieron los tres modos y replays; lote2 acredita cierre de reserva sin inicio y reconciliación documental. DOC115 añade concurrencia real de confirmación/recepción con conexiones físicas independientes; NO acredita concurrencia de escritores documentales ni una llamada legacy en vuelo.
> Rutas que deben reutilizarse
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.vb
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Barreras.vb
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Consolidacion.vb
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Reconciliacion.vb
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Operaciones.vb
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.Reconciliacion.vb
> Evidencia existente y su alcance
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-barreras-creacion-local-20261029.md
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-consolidacion-local-20261030.md
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-reconciliacion-local-20261030.md
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-reserva-detencion-local-20261028.md
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc115-07a-resultados.md
> Escenarios y trabajo pendiente
> COORD-01 — COBERTURA REAL: reutilizar los REC EXPEDIENTE/LOTE/PARCIAL y sus pruebas de barrera/inventario. Contrastar el caso exacto A lista/B pendiente con la evidencia antes de cerrarlo; no repetir los recorridos completos ya aprobados por defecto.
> COORD-02 — BRECHA PRIORITARIA: dos clientes/sesiones independientes solicitan avance documental, respuesta perdida y reclamación vencida mientras la primera llamada sigue activa. Debe haber un solo efecto; vencimiento jamás prueba ausencia; INCIERTO bloquea otra ejecución hasta verificación fiable. Definir puntos de fallo autorizados sin modificar el almacenamiento compartido. Los dobles locales y la recepción 07A no sustituyen esta integración.
> COORD-03 — BRECHA PRIORITARIA: detención durante fase activa, cierre de navegador y consulta desde otra sesión autorizada. Conservar efecto confirmado/IDs/snapshot; impedir nuevas fases afectadas y verificar qué reanudación soporta realmente el contrato. No inventar un endpoint de reanudar ni continuidad desatendida.
> Seguridad y disponibilidad — mapear pruebas existentes de permiso revocado, destino cambiado, revisión obsoleta y publicación nueva. La reanudación conserva snapshot; consulta/verificación siguen exigiendo autoridad. Completar solo negativos no acreditados en la fase correspondiente.
> Criterio de cierre
> Acreditar exclusión o bloqueo seguro de efectos documentales, detención en vuelo y consolidación exacta; documentar estados no resueltos y recuperación permitida. No duplicar máquina de estados, repositorio, motor o worker. No declarar completitud si persisten efectos inciertos.
> Reglas comunes de ejecución
> Reutilizar modelos, repositorio operativo, tablas, ASMX/ASHX, adaptadores, coordinador y herramientas E2E existentes. Antes de programar, contrastar cada criterio con código y evidencia; modificar producto solo ante una brecha reproducida y cubrir su regresión. Leer AGENTS.md y tools/e2e/AGENT-RUNBOOK.md antes de autenticar. Pruebas reales, cambios de permisos, fallos inyectados, limpieza y carga requieren autorización específica de ambiente, cuentas y recursos; este ticket no la concede. Controles RA/DA exclusivamente SELECT, secretos efímeros, evidencia saneada. No repetir efectos inciertos ni cargas ya acreditadas sin motivo.
> Decisiones vigentes que sustituyen exigencias históricas
> No modificar AlmacenamientoDocumentoProduccionDocumental ni SII. Se conserva PDF como formato soportado por este adaptador; TIFF/Multi-TIFF deben rechazarse, no implementarse para satisfacer el prompt anterior. El aislamiento adicional del XML posterior al commit frente a otras cargas fue descartado expresamente por el usuario: queda fuera del alcance y no se afirma como garantía. No habilitar workers, migraciones, permisos, cuotas globales ni limpieza automática por inferencia. Preservar EXPEDIENTE/LOTE y PARCIAL_PERMITIDO/NO_APLICA, snapshot, inventario original y efectos confirmados. Sin rollback global.
> Fuente transversal
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-matriz-final-20261102.md
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc115-verificacion-final-20261104.md

## Jira Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: CARGA, CARPETA, EXPEDIENTE, IMPORTACION, LOTE, PRODUCION, RECUPERACION

## Capabilities

### New Capabilities
- `produccion-importacion-carga-carpeta-lote-recuperacion`: criterios verificables de coordinación y recuperación de la importación existente.

### Modified Capabilities

No se introduce otro motor ni se propone cambiar contratos públicos en este refinamiento.

## Impact

Continuación de enlace/driver:«continua» posterior a RV-08 autoriza6.9/6.10/D-17/RQ-17 en código candidato y regresión local. Se integran la composición para intentos nuevos y la operación cerrada del driver existente; no se modifica almacenamiento compartido/InvocarAlmacenamiento/SII ni se despliega o ejecuta. Fuente:`v2-binding-driver-authorization.json`.

Continuación RV-08:«continua» posterior a RV-07 autoriza6.8, revisión consolidada y plan real separado sobre evidencia local existente. No implementa enlaces del escritor/driver ni despliega o ejecuta. La revisión conserva diez tareas reales/finales abiertas y no permite archivar DOC-118. Fuente:`recovery-v2-review-authorization.json`.

Continuación RV-07:«continua» posterior a RV-06 autoriza6.7, matriz local de intercalaciones bajo los contratos D-12/RQ-12..D-16/RQ-16 existentes. Solo pruebas/evidencia, sin cambios de producto, despliegue o ejecución real. Fuente:`recovery-v2-matrix-authorization.json`.

Continuación RV-06 del2026-11-09:«continua» posterior a RV-05 y al siguiente paso HTTP autoriza6.6/D-16/RQ-16. Se expone recuperación explícita mediante la entrada operativa y composición existentes, con referencias mínimas y respuesta de control diferenciada. Solo implementación/regresión local; almacenamiento habitual V1, sin despliegue o ejecución real. Fuente:`recovery-v2-http-authorization.json`.

Revisión transversal del coordinador VB, repositorio MySQL, consulta ASMX y cliente JavaScript existentes; esta entrega solo modifica documentación y especificaciones. COORD-02/03 requieren preparación y autorización específica posterior. El estado editorial está aprobado por el usuario; no implica pase ni autorización de integración real.

Ampliación local RV-01 posterior a la brecha observada en69:se añaden contratos internos y codec V2 con regresión en el harness existente, bajo D-11/RQ-11 y6.1. El usuario autorizó continuar ese siguiente paso después de RV-02-A. No se conectan al flujo vigente ni habilitan recuperación; la integración del protocolo candidato sigue pendiente y69 permanece inelegible. Véase `recovery-v2-contract-authorization.json`.

Ampliación local RV-02-B:«continua» posterior a RV-01 autoriza6.2/D-12/RQ-12, puerto interno de retención/guardia no destructiva en un parcial del staging existente. Se valida sobre fixtures locales; no modifica V1, no activa recuperación ni acredita producción, otro proceso/nodo o reinicio. Véase `recovery-v2-staging-authorization.json`.

Ampliación local RV-03:«continua» posterior a RV-02-B autoriza6.3/D-13/RQ-13, recorrido interno del adaptador con retención previa al inicio y puerto obligatorio de persistencia V2. Se prueba con dobles explícitos; no modifica la composición pública V1 ni implementa MySQL, que corresponde a RV-04. Véase `recovery-v2-adapter-authorization.json`.

Continuación RV-04:«contiua con el flujo» autoriza6.4/D-14/RQ-14 dentro de DOC-118, sin trasladar trabajo a otro ticket. Se implementan los callbacks V2 en el repositorio existente y su lectura compatible; se valida el núcleo SQL con el adaptador mediante un doble transaccional. Sin DDL, conexión real, activación pública o backfill. Fuente:`recovery-v2-persistence-authorization.json`.

Continuación RV-05 del2026-11-09:«continua» posterior a RV-04 autoriza6.5/D-15/RQ-15, recuperación interna limitada. Cierre negativo requiere expediente sin huellas; positivo exige los IDs observados/verificados. Guardia y comparación de vista hasta commit, sin escritor o conversión general de estados. Regresión local, sin exposición HTTP, despliegue, ejecución real o mutación de69. Fuente:`recovery-v2-recovery-authorization.json`.

## Perfil tecnologico

- Perfil de revision: legacy-webforms-vb (v1).
- Alcance: ASP.NET Web Forms, code-behind VB.NET/C# y JavaScript gradual.
- Este perfil orienta la revision tecnica; no reemplaza los requisitos funcionales ni arquitectonicos del ticket.
