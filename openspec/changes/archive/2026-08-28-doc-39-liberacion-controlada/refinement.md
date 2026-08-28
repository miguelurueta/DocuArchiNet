<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento — DOC-39 liberación controlada

## Fuente y alcance

- Ticket: `DOC-39` — Liberación controlada.
- Cambio OpenSpec: `doc-39-liberacion-controlada`.
- Perfil tecnológico: `legacy-webforms-vb`; Workflow usa ASP.NET Web Forms, VB.NET, MySQL y JavaScript CJS.
- Fuentes revisadas: `prompt/00-contexto-obligatorio.md`, `prompt/05-liberacion-controlada.md`, evidencia DOC-38 y el paquete técnico de DevolverUsuarioAnterior.

DOC-39 prepara la operación controlada de Devolver → Usuario anterior. No implementa funcionalidad, no cambia configuración y no despliega. La salida actual es **solicitar aprobación** porque no se han documentado autorización por ambiente, ventana ni responsables operativos.

## Contexto inspeccionado

- DOC-38 está archivado, validado y su PR fue fusionado; su evidencia local y manual recomienda continuar a la etapa 05 documental.
- El backend, contratos, UI y pruebas de Usuario anterior están documentados bajo `Doc/Actualizacion/workflow/DevolverUsuarioAnterior/01-implementacion-devolver-usuario-anterior/`.
- La operación moderna mantiene preview de solo lectura, token opaco, lock por tarea, adaptador exclusivo y aislamiento de respuestas.
- La etapa de liberación no hereda autorizaciones ni credenciales de QA; cada ambiente requiere aprobación independiente.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código o documento | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | DOC-38 y una versión identificada son precondiciones, no autorización operativa. | Evidencia DOC-38 y PR fusionado. | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Cada ambiente tiene matriz independiente, sin secretos ni aprobación reutilizable. | Matriz documental DOC-39. | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | El runbook describe una operación futura autorizada, sin desplegar ni cambiar configuración en DOC-39. | Runbook documental DOC-39. | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | La preparación usa documentación y controles `SELECT` autorizados con evidencia saneada. | Contratos DOC-36/DOC-38 y matriz de controles. | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | La reversión se realiza por gestión de despliegue y no toca transiciones confirmadas. | Runbook y política de reversión. | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | Usuario anterior conserva la ruta moderna y las operaciones vecinas sus contratos. | Evidencia DOC-37/DOC-38 y paquete técnico. | D-06 | RQ-06 | Origen: D-06, RQ-06 |
| D-07 | La decisión única actual es solicitar aprobación hasta completar autorización, ventana y responsables. | Matriz de ambiente y decisión DOC-39. | D-07 | RQ-07 | Origen: D-07, RQ-07 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | La evidencia técnica y la versión candidata están identificadas. | La revisión las registra sin iniciar despliegue. | Evita confundir QA aprobada con permiso operativo. |
| RQ-02 | Cada ambiente queda explícitamente autorizado o sin autorización. | La matriz no contiene secretos ni mezcla aprobaciones. | Evita un despliegue en ambiente no autorizado. |
| RQ-03 | Existe un procedimiento de operación condicionado a autorización. | Sin ventana y responsable aprobados, el runbook no se ejecuta. | Evita cambios implícitos de ambiente. |
| RQ-04 | Los controles no producen mutación. | Solo se permiten evidencia y `SELECT` aprobados. | Protege tareas, auditoría y datos de negocio. |
| RQ-05 | La reversión no modifica la historia Workflow. | Solo afecta nuevos intentos mediante proceso de despliegue. | Evita corrupción o reversión de transiciones. |
| RQ-06 | Las rutas y contratos existentes se preservan. | La revisión descarta postback, fallback y cruces de destinos. | Evita regresiones en operaciones vecinas. |
| RQ-07 | La decisión es inequívoca y accionable. | Sin precondiciones completas se solicita aprobación o se bloquea. | Impide liberar con evidencia operativa incompleta. |

## Resultado del refinamiento

- Estado: `approved`.
- DOC-39 se limita a documentación, controles autorizables de solo lectura y decisión de liberación; no autoriza ni ejecuta un despliegue.
- El siguiente paso es implementar las tareas documentales y registrar las aprobaciones de ambiente sin secretos.
