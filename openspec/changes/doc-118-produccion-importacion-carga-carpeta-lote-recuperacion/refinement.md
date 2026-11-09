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
| D-11 | Fijar contratos internos V2 sin conceder capacidad por JSON ni conectarlos al flujo vigente. | DocumentosAlmacenamientoV2Importacion.vb; ImportacionCarpetasDocumentosV2BehaviorTests.cs; doc118-contratos-v2-local-20261108.md | D-11 | RQ-11 | RV-01/6.1: esquema cerrado, vínculos canónicos, compatibilidad V1 y cero efectos/capacidad. |
| D-12 | Añadir retención y guardia V2 no destructivas al proveedor existente; fallar cerrado ante ámbito, recurso, proveedor o época distintos. | AlmacenStagingImportacion.Recuperacion.vb; ImportacionCarpetasStagingV2Interfaces.vb; pruebas del harness | D-12 | RQ-12 | RV-02-B/6.2: original exacto, exclusión local, vida útil y sección sincronizada; sin activar recuperación. |
| D-13 | Retener antes del inicio V2 y exigir acuses internos concordantes antes de una única llamada al escritor; observar retorno y propagar abortos. | AdaptadorDocumentoImportacion.ProtocoloV2.vb; PersistenciaDocumentoV2Importacion.vb; harness existente | D-13 | RQ-13 | RV-03/6.3: dobles de persistencia y escritor, retención real; sin fallback V1 o composición pública. |
| D-14 | Persistir evidencia y diario V2 en la transacción existente; autorización de un solo uso y checkpoint atómico. | MySqlImportacionCarpetasRepository.DocumentoV2.vb; lector documental compatible; harness SQL | D-14 | RQ-14 | RV-04/6.4: comparación exacta, rollback, replay sin efectos y acuse posterior al commit. |
| D-15 | Recuperar internamente solo V2 con prueba/guardia concordantes, cierre sin invocación o confirmación por IDs observados; cero escritor. | Parciales RecuperacionV2 de coordinador/repositorio; comando y catálogo de comprobación | D-15 | RQ-15 | RV-05/6.5: autoridad/vista exactas, guardia hasta commit, rollback e historial sin afectar intentos posteriores. |
| D-16 | Exponer recuperación explícita mediante entrada operativa cerrada y composición sin escritor. | ASMX/DTOs, composición de reconciliación y parcial RecuperacionV2 del coordinador | D-16 | RQ-16 | RV-06/6.6: sesión/CSRF/autoridad/contexto/revisiones, proyección segura y cierre sin invocación diferenciado. |

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
| RQ-11 | Documentos internos V2 sin capacidad de ejecución | Escenarios WHEN/THEN en spec.md, origen D-11;19 casos del harness. | V1 y D-07/RQ-07 intactos; no backfill, procedencia durable, proveedor o gate acreditados por JSON. |
| RQ-12 | Guardia de staging V2 no destructiva | Escenarios WHEN/THEN en spec.md, origen D-12; proveedor real sobre fixture local. | Sin cambios V1/AbrirRetiro; otro proceso/nodo o reinicio conserva bloqueo; no acredita producción ni69. |
| RQ-13 | Adaptador V2 retenido y sin repetición | Escenarios WHEN/THEN en spec.md, origen D-13; intercalaciones y abortos locales. | RV-04 debe implementar commit/diario; este recorrido interno no activa V2, no usa fallback V1 ni recupera69. |
| RQ-14 | Persistencia V2 y autorización de un solo uso | Escenarios WHEN/THEN en spec.md, origen D-14; núcleo SQL con doble transaccional. | Sin DDL o backfill; autorización repetida bloquea; checkpoint/evento/consolidación juntos. |
| RQ-15 | Recuperación interna V2 sin escritor | Escenarios WHEN/THEN en spec.md, origen D-15; cierre negativo/positivo bajo guardia. | Sin reset general, conversión a INCIERTO o backfill;69/V1 y proveedor distinto bloqueados; HTTP reservado para RV-06. |
| RQ-16 | Recuperación HTTP explícita y acotada | Escenarios WHEN/THEN en spec.md, origen D-16; guard operativo y proyección pública. | Solo cuatro referencias; autoridad RECONCILIAR, RA/DA fijados y cero escritor; V1 y69 excluidos. |

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

Estado: approved. Aprobación editorial registrada el 2026-11-07 por la indicación del usuario «continua», posterior al detalle de implementación y a la solicitud de revisión del refinamiento. Comprende alcance, decisiones y continuación con el desglose trazable presentado; no concede autorización de pruebas reales. tasks.md sustituye las ocho tareas genéricas por 27 tareas atómicas. La compuerta opsxj:refine verifica su trazabilidad; no acredita COORD-02/03 reales.

La validación estructural OpenSpec no sustituye esta aprobación ni concede autorización para E2E real. La entrega documental y su regresión local se registran en Git en la carpeta principal.

Ampliación local RV-01 del 2026-11-08:el usuario indicó «contiunua» después de la entrega RV-02-A y del siguiente paso explícito RV-01. Esa continuación autoriza únicamente los documentos internos y su regresión. Se incorporaron D-11/RQ-11 y6.1 antes de modificar código; el mapa actual es18/28 tras completarla, con las mismas10 tareas anteriores pendientes. No acepta por inferencia la integración del protocolo candidato ni amplía autorización de ejecución real. Fuente:`recovery-v2-contract-authorization.json`.


## Avance de preparación

Continuación RV-08:«continua» posterior a RV-07 autoriza6.8, revisión local y preparación de plan real separado bajo D-01/RQ-01 y D-10/RQ-10. No amplía contratos ni autoriza composición del escritor, extensión del driver, despliegue, ejecución real, presupuesto adicional o recuperación de41/53/69. La revisión distingue dependencia de implementación y autorización futura; no solicita ejecutar un plan todavía sin enlace V2.

Continuación RV-07:«continua» posterior a RV-06 y al siguiente paso de matriz autoriza6.7, verificación de D-12/RQ-12..D-16/RQ-16 y RC-01..RC-07 sin ampliar contratos. Reutiliza fixtures y añade únicamente intercalaciones no cubiertas en conjunto, con staging real, procesos propios y SQL/transacción simulados. No concede despliegue, activación del escritor V2, ejecución real o recuperación de41/53/69.

Continuación RV-06:«continua» posterior a la entrega RV-05 y al siguiente paso explícito de integración HTTP autoriza6.6/D-16/RQ-16 y regresión local. La operación solo compone recuperación; no activa el almacenamiento normal V2 ni concede despliegue, ejecución real o mutación de41/53/69.

Continuación RV-05:«continua» después de RV-04 y de la recuperación limitada propuesta autoriza6.5/D-15/RQ-15 y su regresión local. La rama negativa se restringe a expediente sin huellas documentales. No concede ejecución real, publicación HTTP o mutación de41/53/69.

Ampliación RV-04:«contiua con el flujo» posterior a RV-03 y al siguiente paso RV-04 autoriza6.4/D-14/RQ-14, implementación local del repositorio y regresión. No concede ejecución MySQL real ni activación pública.

Ampliación RV-03:«continua» posterior a RV-02-B y al siguiente paso de retención desde el adaptador autoriza6.3/D-13/RQ-13. El recorrido se prueba con puerto de persistencia simulado; RV-04 y la activación pública siguen pendientes. No amplía autorizaciones reales.

Ampliación local RV-02-B:«continua» posterior a RV-01 y al siguiente paso explícito de integrar la guardia autoriza6.2/D-12/RQ-12. Se incorpora antes de implementar. Es el puerto local y su regresión; no autoriza activar recuperación, desplegar, tocar workers o ejecutar contra datos reales.

El protocolo concreto está en `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-plan-integracion.md`; el mapa de negativos y recuperación está en `doc118-seguridad-recuperacion.md` de la misma carpeta. Se propone calificar un depurador externo sin alterar almacenamiento compartido. Disponibilidad local del ejecutable no implica calificación del runtime. Las tareas 4.x conservan su compuerta de autorización específica.
