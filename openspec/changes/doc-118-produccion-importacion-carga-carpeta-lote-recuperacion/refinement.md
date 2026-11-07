<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento DOC-118 — coordinación y recuperación

## Fuente y alcance

Ticket DOC-118; perfil legacy-webforms-vb. Revisión del 2026-11-07 sobre la carpeta principal, base `5162b92996c7652a30fc39f3b4142a5dd17c166c`, que contiene DOC-117 integrado desde `ee36f5ae540c02fd9986670f0a7c0305a7db47f5`. La instantánea Jira permanece intacta en `specs/produccion-importacion-carga-carpeta-lote-recuperacion/jira-context.md`.

Este artefacto es la compuerta entre el ticket y la implementación. No se aprueba por generación automática: una persona responsable debe confirmar alcance, decisiones, compatibilidad y evidencia de código.

## Contexto inspeccionado

`source-review.json` identifica rutas y hashes normalizados del código inspeccionado. `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-matriz-coordinacion.md` contrasta cobertura real histórica, dobles locales y brechas de integración. Ninguna prueba histórica se atribuye al commit actual.

Ya existen coordinador, repositorio transaccional MySQL, ASMX, adaptador documental y consulta de lotes. Se preservan PDF, rechazo TIFF/Multi-TIFF, snapshot e inventario original. No se propone modificar AlmacenamientoDocumentoProduccionDocumental ni SII; tampoco aislar el XML después del commit, agregar motor, worker, tablas, migraciones, permisos, limpieza o rollback global.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código/documento | Design | Requirement | Trabajo vinculado en tasks.md |
| --- | --- | --- | --- | --- | --- |
| D-01 | Reutilizar evidencia con su SHA y ambiente; modificar producto únicamente después de reproducir una brecha. | source-review.json; doc111-matriz-final-20261102.md; doc115-07a-resultados.md | D-01 | RQ-01 | Separar evidencia histórica, revisión actual y ejecución nueva. |
| D-02 | Una petición ejecuta una fase sobre un recurso; toda reclamación documental activa o incierta impide otro escritor en el lote. | CoordinadorImportacionCarpetas.Operaciones.vb: AvanzarLote; MySqlImportacionCarpetasRepository.Control.vb: ReservarIntentoDocumental/HayReclamacionDocumental | D-02 | RQ-02 | Acreditar dos clientes independientes contra el mismo avance y contra otro archivo del lote. |
| D-03 | Preservar EXPEDIENTE/LOTE y PARCIAL_PERMITIDO/NO_APLICA, sin reducir el inventario original. | CoordinadorImportacionCarpetas.Barreras.vb: EvaluarBarreraAlmacenamiento; doc111-rec-expediente-pass-20261101.md | D-03 | RQ-03 | Reutilizar COORD-01 histórico A lista/B por recibir y contrastar los tres modos. |
| D-04 | No hay TTL documental habilitante: fecha antigua, desconexión o supuesta expiración no prueban ausencia de efecto ni liberan la reclamación. | MySqlImportacionCarpetasRepository.Control.vb: HayReclamacionDocumental/ValidarReservaDocumental | D-04 | RQ-04 | Acreditar bloqueo con primera llamada todavía activa; no inventar un vencimiento de lease. |
| D-05 | Repetir la misma solicitud consulta su resultado durable; efecto sin checkpoint conserva IDs observados y bloquea otra ejecución. | CoordinadorImportacionCarpetas.Documento.vb: AlmacenarArchivo/ProyectarDocumento; doc117-alm02-interno-pass-20261106.md | D-05 | RQ-05 | Distinguir respuesta perdida después del checkpoint de efecto sin confirmación durable. |
| D-06 | La detención es una señal persistida; no cancela la llamada activa. Cerrar navegador no equivale a detener. Consultar lote no reanuda fases. | CoordinadorImportacionCarpetas.vb: SolicitarDetencion; ServicioConsultaImportacion.vb; ImportacionCarpetas.js: cerrar/resume | D-06 | RQ-06 | Acreditar detención en vuelo y consulta posterior desde otra sesión autorizada. |
| D-07 | CerrarReservaSinInicio solo cierra una reserva ALMACENAR probada sin inicio y sin detención; ReconciliarRecurso verifica evidencia conocida de INCIERTO. | CoordinadorImportacionCarpetas.Reconciliacion.vb; MySqlImportacionCarpetasRepository.Reconciliacion.vb: PersistirCierreReservaSinInicio | D-07 | RQ-07 | No reparar EN_CURSO por antigüedad ni presentar el cierre RECHAZADO como éxito documental. |
| D-08 | Cada consulta, avance, detención o verificación exige autoridad actual; una publicación posterior no reemplaza el snapshot del lote. | ServicioConsultaImportacion.vb; CoordinadorImportacionCarpetas.Documento.vb; CoordinadorImportacionCarpetas.Reconciliacion.vb | D-08 | RQ-08 | Mapear permiso revocado, destino cambiado, revisión obsoleta y publicación posterior antes de agregar negativos. |
| D-09 | Derivar estados del inventario original y checkpoints; conservar efectos e IDs confirmados aun con detención. | MySqlImportacionCarpetasRepository.Documento.vb: RegistrarResultadoDocumento; MySqlImportacionCarpetasRepository.Consolidacion.vb: ConsolidarEnTransaccion | D-09 | RQ-09 | Acreditar estado incierto/activo prioritario y COMPLETADO solo con todos los efectos positivos requeridos. |
| D-10 | COORD-02/03 requieren prueba nueva de la llamada legacy activa, sin modificar el almacenamiento compartido ni heredar excepciones anteriores. | AGENTS.md; tools/e2e/AGENT-RUNBOOK.md; doc118-matriz-coordinacion.md | D-10 | RQ-10 | Definir observación viable, ambiente, cuentas, recursos y presupuesto antes de solicitar autorización específica. |

## Requisitos verificables

| ID | Resultado observable | Criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Procedencia y cambio mínimo | Escenarios WHEN/THEN en spec.md, origen D-01. | Reutilizar evidencia con su SHA y ambiente; modificar producto únicamente después de reproducir una brecha. |
| RQ-02 | Exclusión documental por lote | Escenarios WHEN/THEN en spec.md, origen D-02. | Una petición ejecuta una fase sobre un recurso; toda reclamación documental activa o incierta impide otro escritor en el lote. |
| RQ-03 | Barreras e inventario | Escenarios WHEN/THEN en spec.md, origen D-03. | Preservar EXPEDIENTE/LOTE y PARCIAL_PERMITIDO/NO_APLICA, sin reducir el inventario original. |
| RQ-04 | Antigüedad sin toma de posesión | Escenarios WHEN/THEN en spec.md, origen D-04. | No hay TTL documental habilitante: fecha antigua, desconexión o supuesta expiración no prueban ausencia de efecto ni liberan la reclamación. |
| RQ-05 | Respuesta perdida y certeza durable | Escenarios WHEN/THEN en spec.md, origen D-05. | Repetir la misma solicitud consulta su resultado durable; efecto sin checkpoint conserva IDs observados y bloquea otra ejecución. |
| RQ-06 | Detención y consulta posterior | Escenarios WHEN/THEN en spec.md, origen D-06. | La detención es una señal persistida; no cancela la llamada activa. Cerrar navegador no equivale a detener. Consultar lote no reanuda fases. |
| RQ-07 | Recuperación existente y limitada | Escenarios WHEN/THEN en spec.md, origen D-07. | CerrarReservaSinInicio solo cierra una reserva ALMACENAR probada sin inicio y sin detención; ReconciliarRecurso verifica evidencia conocida de INCIERTO. |
| RQ-08 | Autoridad vigente y snapshot | Escenarios WHEN/THEN en spec.md, origen D-08. | Cada consulta, avance, detención o verificación exige autoridad actual; una publicación posterior no reemplaza el snapshot del lote. |
| RQ-09 | Consolidación exacta | Escenarios WHEN/THEN en spec.md, origen D-09. | Derivar estados del inventario original y checkpoints; conservar efectos e IDs confirmados aun con detención. |
| RQ-10 | Integración observable y autorizada | Escenarios WHEN/THEN en spec.md, origen D-10. | COORD-02/03 requieren prueba nueva de la llamada legacy activa, sin modificar el almacenamiento compartido ni heredar excepciones anteriores. |

## Dependencias concretas de ejecución

COORD-01 tiene evidencia real histórica del caso exacto EXPEDIENTE A lista/B por recibir. COORD-02 y COORD-03 siguen NO_EJECUTADOS en integración para su alcance específico. DOC115-07A prueba recepción/confirmación; DOC117-ALM02 prueba pérdida de retorno y efecto sin checkpoint; DOC117-ALM03C prueba coexistencia con carga individual. Ninguno acredita dos clientes documentales disputando una reclamación durante una llamada legacy activa ni su detención en vuelo.

La observación debe demostrar entrada y permanencia dentro de la llamada real. Una pausa antes de invocarla, después de su retorno, o la mera superposición HTTP no sirven. La viabilidad de una observación externa sin alterar el almacenamiento compartido debe resolverse antes de ejecutar. Si no existe una vía compatible, registrar la limitación y revisar el alcance con la persona responsable; no declarar PASS sustituyendo el escenario por un doble.

Los intentos históricos 41 (DOC116, lote18) y 53 (DOC117, lote21) se conservan intactos. No se repiten, cierran, reconcilian ni reparan. Sus efectos no resueltos se declaran expresamente; tampoco sirven como recursos de prueba nuevos.

## Secuencia propuesta tras aprobación

1. Sustituir las ocho tareas genéricas de tasks.md por tareas atómicas con área, complejidad, verificación y Origen: D-XX, RQ-XX para las diez decisiones.
2. Acreditar COORD-01 mediante contraste documental y regresión local; mapear negativos de seguridad ya existentes.
3. Caracterizar observación compatible y preparar casos independientes COORD-02 y COORD-03, con límites y recursos nuevos identificados.
4. Solicitar autorización específica de ambiente, cuentas, recursos, fallos y presupuesto; ejecutar únicamente lo autorizado.
5. Corregir producto únicamente si se reproduce una brecha, con su regresión. Conservar incertidumbre y bloquear nuevos efectos cuando no hay recuperación fiable.
6. Verificar evidencia, consolidación e inventario; pasar revisiones y validación OPSXJ antes de considerar el archivo.

## Resultado del refinamiento

Estado: approved. Aprobación editorial registrada el 2026-11-07 por la indicación del usuario «continua», posterior al detalle de implementación y a la solicitud de revisión del refinamiento. Comprende alcance, decisiones y continuación con el desglose trazable presentado; no concede autorización de pruebas reales. tasks.md sustituye las ocho tareas genéricas por 25 tareas atómicas. La compuerta opsxj:refine verifica su trazabilidad; no acredita COORD-02/03 reales.

La validación estructural OpenSpec no sustituye esta aprobación ni concede autorización para E2E real. La entrega documental y su regresión local se registran en Git en la carpeta principal.


## Avance de preparación

El protocolo concreto está en `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-plan-integracion.md`; el mapa de negativos y recuperación está en `doc118-seguridad-recuperacion.md` de la misma carpeta. Se propone calificar un depurador externo sin alterar almacenamiento compartido. Disponibilidad local del ejecutable no implica calificación del runtime. Las tareas 4.x conservan su compuerta de autorización específica.
