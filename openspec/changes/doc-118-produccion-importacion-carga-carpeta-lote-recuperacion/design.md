## Context

DOC-118 contrasta la coordinación y recuperación ya implementadas con sus brechas de evidencia. Base local inspeccionada: `5162b92996c7652a30fc39f3b4142a5dd17c166c`. La procedencia por archivo está en source-review.json. Perfil legacy-webforms-vb; no se propone una segunda máquina de estados.

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

## Goals / Non-Goals

Objetivo: cerrar únicamente brechas reproducidas de exclusión documental, certeza del resultado y detención segura, reutilizando lo ya acreditado. Esta entrega define criterios; no modifica producto ni ejecuta integración real.

Fuera de alcance: modificar almacenamiento compartido/SII, aislamiento adicional del XML, TIFF/Multi-TIFF, workers, nuevo repositorio/endpoint de reanudar, migraciones, permisos, limpieza automática y rollback global.

## Decisions

### D-01 — Procedencia y cambio mínimo (RQ-01)

Reutilizar evidencia con su SHA y ambiente; modificar producto únicamente después de reproducir una brecha.

Evidencia: source-review.json; doc111-matriz-final-20261102.md; doc115-07a-resultados.md. Separar evidencia histórica, revisión actual y ejecución nueva.

### D-02 — Exclusión documental por lote (RQ-02)

Una petición ejecuta una fase sobre un recurso; toda reclamación documental activa o incierta impide otro escritor en el lote.

Evidencia: CoordinadorImportacionCarpetas.Operaciones.vb: AvanzarLote; MySqlImportacionCarpetasRepository.Control.vb: ReservarIntentoDocumental/HayReclamacionDocumental. Acreditar dos clientes independientes contra el mismo avance y contra otro archivo del lote.

### D-03 — Barreras e inventario (RQ-03)

Preservar EXPEDIENTE/LOTE y PARCIAL_PERMITIDO/NO_APLICA, sin reducir el inventario original.

Evidencia: CoordinadorImportacionCarpetas.Barreras.vb: EvaluarBarreraAlmacenamiento; doc111-rec-expediente-pass-20261101.md. Reutilizar COORD-01 histórico A lista/B por recibir y contrastar los tres modos.

### D-04 — Antigüedad sin toma de posesión (RQ-04)

No hay TTL documental habilitante: fecha antigua, desconexión o supuesta expiración no prueban ausencia de efecto ni liberan la reclamación.

Evidencia: MySqlImportacionCarpetasRepository.Control.vb: HayReclamacionDocumental/ValidarReservaDocumental. Acreditar bloqueo con primera llamada todavía activa; no inventar un vencimiento de lease.

### D-05 — Respuesta perdida y certeza durable (RQ-05)

Repetir la misma solicitud consulta su resultado durable; efecto sin checkpoint conserva IDs observados y bloquea otra ejecución.

Evidencia: CoordinadorImportacionCarpetas.Documento.vb: AlmacenarArchivo/ProyectarDocumento; doc117-alm02-interno-pass-20261106.md. Distinguir respuesta perdida después del checkpoint de efecto sin confirmación durable.

### D-06 — Detención y consulta posterior (RQ-06)

La detención es una señal persistida; no cancela la llamada activa. Cerrar navegador no equivale a detener. Consultar lote no reanuda fases.

Evidencia: CoordinadorImportacionCarpetas.vb: SolicitarDetencion; ServicioConsultaImportacion.vb; ImportacionCarpetas.js: cerrar/resume. Acreditar detención en vuelo y consulta posterior desde otra sesión autorizada.

### D-07 — Recuperación existente y limitada (RQ-07)

CerrarReservaSinInicio solo cierra una reserva ALMACENAR probada sin inicio y sin detención; ReconciliarRecurso verifica evidencia conocida de INCIERTO.

Evidencia: CoordinadorImportacionCarpetas.Reconciliacion.vb; MySqlImportacionCarpetasRepository.Reconciliacion.vb: PersistirCierreReservaSinInicio. No reparar EN_CURSO por antigüedad ni presentar el cierre RECHAZADO como éxito documental.

### D-08 — Autoridad vigente y snapshot (RQ-08)

Cada consulta, avance, detención o verificación exige autoridad actual; una publicación posterior no reemplaza el snapshot del lote.

Evidencia: ServicioConsultaImportacion.vb; CoordinadorImportacionCarpetas.Documento.vb; CoordinadorImportacionCarpetas.Reconciliacion.vb. Mapear permiso revocado, destino cambiado, revisión obsoleta y publicación posterior antes de agregar negativos.

### D-09 — Consolidación exacta (RQ-09)

Derivar estados del inventario original y checkpoints; conservar efectos e IDs confirmados aun con detención.

Evidencia: MySqlImportacionCarpetasRepository.Documento.vb: RegistrarResultadoDocumento; MySqlImportacionCarpetasRepository.Consolidacion.vb: ConsolidarEnTransaccion. Acreditar estado incierto/activo prioritario y COMPLETADO solo con todos los efectos positivos requeridos.

### D-10 — Integración observable y autorizada (RQ-10)

COORD-02/03 requieren prueba nueva de la llamada legacy activa, sin modificar el almacenamiento compartido ni heredar excepciones anteriores.

Evidencia: AGENTS.md; tools/e2e/AGENT-RUNBOOK.md; doc118-matriz-coordinacion.md. Definir observación viable, ambiente, cuentas, recursos y presupuesto antes de solicitar autorización específica.

## Comportamiento y límites observados

AvanzarLote despacha una fase CREAR_EXPEDIENTE, MATERIALIZAR_VALORES o ALMACENAR por recurso. AlmacenarArchivo consulta primero el historial de la misma solicitud dentro del contexto autorizado: un replay confirmado devuelve la identidad durable sin escritor nuevo. Para una ejecución nueva exige revisión, barrera, staging y autoridad; la reserva transaccional usa el orden lote → raíz → archivo. HayReclamacionDocumental considera RESERVADO, EN_CURSO e INCIERTO y marcadores huérfanos, sin habilitar sustitución por fecha.

SolicitarDetencion persiste la señal y consolida conservando la reclamación activa. RegistrarResultadoDocumento persiste checkpoint y llama a ConsolidarEnTransaccion en la misma transacción exterior. Un efecto completado al final conserva IDs y señal de detención; si todos los efectos requeridos son positivos puede resultar COMPLETADO. Con actividad sigue EN_CURSO; incertidumbre exige PENDIENTE_VERIFICACION; sin actividad y con trabajo sin completar, la señal permite DETENIDO. No se convierte un resultado incierto en fallo seguro.

CerrarReservaSinInicio exige ALMACENAR/WEB_DOCUMENTO, RESERVADO, evento único de reserva y ausencia de inicio/resultado/evidencia, además de la vista y revisiones exactas y detencion_solicitada=false. El resultado durable es RECHAZADO/RESERVA_CERRADA_SIN_INICIO, sin documento/inventario; la respuesta pública confirma el cierre de la reserva, no un documento. No resuelve EN_CURSO ni una detención activa. ReconciliarRecurso verifica INCIERTO con identidad y evidencia conocidas, sin volver a almacenar; no fuerza EN_CURSO a INCIERTO.

El cliente resume() abre la consulta del lote recordado; cerrar() libera recursos del cliente. Cerrar no persiste una detención ni demuestra que terminó la petición del servidor. Una sesión nueva debe satisfacer autorización actual y conservar el snapshot. No se promete continuidad desatendida ni reset de detención.

## Risks / Trade-offs

La exclusión conservadora puede dejar recursos bloqueados sin recuperación automática; es preferible mantener incertidumbre explícita a duplicar efectos. Una respuesta pública INCIERTO puede corresponder a intento durable EN_CURSO: distinguir ambas capas en la evidencia. Los resultados históricos retienen su SHA/ambiente y no certifican la instalación actual.

La integración de COORD-02/03 depende de observación real de la llamada legacy activa sin modificar almacenamiento compartido. La excepción instrumental de DOC117 no se transfiere. Sin observabilidad compatible o autorización específica, esos casos permanecen NO_EJECUTADOS y no satisfacen el cierre.

## Migration Plan

Entrega documental sin despliegue ni migración. Después de aprobación humana: tareas atómicas, contraste de cobertura, preparación de integración, autorización específica y ejecución acotada. Un defecto reproducido habilita una corrección acotada y verificable. Revertir documentación mediante Git no revierte efectos documentales ni autoriza limpiar recursos.

## Acceptance and review gates

Diez decisiones y diez requisitos; escenarios en spec.md y cobertura en doc118-matriz-coordinacion.md. La aprobación de refinement.md y el reemplazo trazable de las tareas genéricas son compuertas antes de implementación. Las pruebas reales requieren una autorización separada posterior a un plan concreto. No hay autorización de ejecución implícita en este diseño.
