## Why

PRODUCCION-IMPORTACION-CARGA-CARPETA-LOTE-RECUPERACION. Ver detalle funcional completo del ticket en la seccion Jira Details.

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue DOC-118.
- Se formaliza una propuesta OpenSpec inicial derivada del ticket Jira.
- Se captura el resumen y la descripcion del ticket como punto de partida para refinement posterior.
- Se deja lista una base coherente para continuar con design, specs y tasks.

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
- `produccion-importacion-carga-carpeta-lote-recuperacion`: Capacidad derivada del ticket Jira para continuar el refinamiento funcional en OpenSpec.

### Modified Capabilities
- 

## Impact

- Nueva propuesta inicial en `openspec/changes/<changeName>/proposal.md`.
- Impacto funcional pendiente de refinamiento en los siguientes artefactos OpenSpec.

## Perfil tecnologico

- Perfil de revision: legacy-webforms-vb (v1).
- Alcance: ASP.NET Web Forms, code-behind VB.NET/C# y JavaScript gradual.
- Este perfil orienta la revision tecnica; no reemplaza los requisitos funcionales ni arquitectonicos del ticket.
