## ADDED Requirements

### Requirement: Registro persistente del ciclo OPSXJ

El sistema SHALL conservar, por ticket, un registro local sin secretos de cada etapa ejecutada del ciclo OPSXJ. Cada entrada SHALL identificar la etapa, resultado, fecha, SHA evaluado, origen o responsable cuando esté disponible, referencia verificable y detalle de fallo cuando exista. La confirmación de revisión SHALL persistirse para el SHA confirmado; la variable `OPSXJ_OPENSPEC_REVIEW_CONFIRMED` seguirá siendo compatible y, cuando se use, SHALL registrar dicha confirmación.

#### Scenario: Revisión confirmada para el SHA actual

- **WHEN** se valida un ticket con una revisión formal confirmada para el SHA actual
- **THEN** el registro conserva la confirmación y una consulta posterior de estado muestra la etapa de revisión como completada sin requerir nuevamente la variable de entorno.

#### Scenario: Cambio de SHA después de una revisión

- **WHEN** el SHA actual no coincide con el SHA de la última revisión confirmada
- **THEN** el checklist informa la revisión como desactualizada y no la usa para habilitar el archivo.

#### Scenario: Falla de una etapa

- **WHEN** una validación, archivo o cierre falla
- **THEN** el registro conserva el resultado fallido y su detalle sin sobrescribir una ejecución anterior correcta de otro SHA.

### Requirement: Checklist consolidado y compatible del ciclo

El sistema SHALL mostrar mediante `opsxj:status` un checklist ordenado de las etapas aplicables: inicio, refinement, revisión, validación, archivo, PR fusionado y cierre Jira. Cada etapa SHALL incluir estado, última actualización, SHA o referencia cuando aplique y una siguiente acción clara. El estado SHALL combinar el registro persistente con las comprobaciones vivas de Git, OpenSpec, GitHub y Jira sin reportar una etapa como completada si su evidencia está desactualizada.

#### Scenario: Ticket cerrado completamente

- **WHEN** un ticket tiene OpenSpec archivado, PR fusionado y Jira finalizado
- **THEN** el checklist muestra todas las etapas aplicables completadas y la siguiente acción indica que no se requiere trabajo local.

#### Scenario: Ticket histórico sin registro

- **WHEN** se consulta un ticket histórico que no tiene registro persistente
- **THEN** el comando conserva la compatibilidad histórica, muestra las comprobaciones que puede observar y señala las etapas no registradas sin bloquear la consulta.

#### Scenario: Salida estructurada

- **WHEN** se solicita el estado en formato JSON
- **THEN** la respuesta incluye el checklist ordenado y datos suficientes para que una interfaz o automatización identifique etapas completadas, pendientes, bloqueadas o desactualizadas.
