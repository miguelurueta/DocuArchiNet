<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-35-liberacion-devolver-tarea-actividad

## Fuente y alcance

- Ticket: DOC-35 — LIBERACION-DEVOLVER-TAREA-ACTIVIDAD
- Cambio OpenSpec: doc-35-liberacion-devolver-tarea-actividad
- Fuente Jira: specs/liberacion-devolver-tarea-actividad/jira-context.md
- Perfil tecnológico: documentación transversal para Workflow ASP.NET Web Forms; no se introducen reglas de implementación ni cambios de framework.

Este artefacto establece la compuerta entre la evidencia técnica y una futura operación. DOC-35 entrega la decisión, matriz y runbook, pero no autoriza ni ejecuta un despliegue.

## Contexto inspeccionado

- Contexto obligatorio y Exploración de Devolver a actividad anterior: semántica de conectores entrantes Ruta/Flujo, preview SELECT, lock por tarea, token, ruta moderna única y aislamiento de respuestas.
- Paquete DOC-34: compilación sin errores, 83 pruebas CJS focales correctas, QA no autenticada y dictamen apto para solicitar la fase 04.
- Referencia de versión: merge del PR #29 en main; no hay ambiente, ventana, responsables nominales ni autorización operativa entregados para DOC-35.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | La decisión vigente es solicitar aprobación operativa y usa DOC-34 junto con el merge del PR #29 como base técnica. | Doc/Actualizacion/workflow/DebolverTarea/03-verificacion-transversal-devolver-actividad-anterior/04-pruebas-y-evidencia.md | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | La matriz inicia sin ambientes elegibles y exige autorización, versión, alcance, ventana, responsables, evidencia y continuación por ambiente. | Doc/Actualizacion/workflow/DebolverTarea/04-liberacion-controlada-devolver-actividad-anterior/02-matriz-ambientes.md | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | El runbook solo habilita controles documentales y SELECT saneados después de autorización explícita; prohíbe E2E, carga y cambios de configuración. | Doc/Actualizacion/workflow/DebolverTarea/04-liberacion-controlada-devolver-actividad-anterior/03-runbook-operativo.md | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | La reversión ocurre por paquete aprobado, afecta intentos nuevos y conserva ruta moderna, contratos, lock y aislamiento de respuestas. | Doc/Actualizacion/workflow/DebolverTarea/04-liberacion-controlada-devolver-actividad-anterior/04-compatibilidad-y-riesgos.md | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | El registro final es saneado y aborta ante autorización, versión, contrato o control no conforme. | openspec/changes/doc-35-liberacion-devolver-tarea-actividad/tasks.md | D-05 | RQ-05 | Origen: D-05, RQ-05 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | El paquete declara una decisión única basada en DOC-34 y la versión de referencia. | WHEN no hay ambiente autorizado THEN solicita aprobación operativa. | La evidencia técnica no autoriza operación. |
| RQ-02 | Cada ambiente queda identificado o explícitamente fuera de alcance. | WHEN falta un campo obligatorio THEN no se habilita operación. | La autorización no se reutiliza entre ambientes, versiones o ventanas. |
| RQ-03 | El operador conoce controles permitidos y criterios de aborto. | WHEN la autorización está completa THEN solo usa SELECT saneados y evidencia documental. | No se ejecuta E2E, carga ni cambio de ambiente en DOC-35. |
| RQ-04 | La operación futura mantiene las invariantes de la capacidad. | WHEN se ordena reversión THEN restaura paquete sin alterar tareas confirmadas. | No se reactiva UI legacy ni se cambia el contrato. |
| RQ-05 | El resultado operativo queda trazable sin secretos. | WHEN un control falla THEN aborta y registra referencias saneadas. | No se exponen credenciales, cookies, cadenas de conexión ni datos de tarea. |

## Resultado del refinamiento

- Estado: aprobado. La matriz enlaza cada decisión con design, requisitos y tareas de cierre.
- La sincronización agregará encabezados de trazabilidad sin reemplazar el contenido refinado.
