<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-36-backend-devolucion-usuario-anterior

## Fuente y alcance

- Ticket: `DOC-36` — BACKEND-DEVOLUCION-USUARIO-ANTERIOR
- Cambio OpenSpec: `doc-36-backend-devolucion-usuario-anterior`
- Fuente Jira: `specs/*/jira-context.md`
- Perfil tecnológico: VB.NET, ASP.NET Web Forms/ASMX y MySQL.

La autorización explícita del usuario el 2026-08-26 permite formalizar las decisiones de etapa 01 que faltaban. DOC-36 implementará exclusivamente el servidor: no incluye interfaz, configuración ni E2E autenticada.

## Contexto inspeccionado

- `workflow/ClassWorkflow.vb`: el legado mezcla usuario anterior con selector de actividad y pasa `Id_Ruta_Workflow` donde debería usar el usuario autenticado.
- `workflow/Class_estados_tarea_workflow.vb`: el antecedente observado se obtiene con dos filas de `estados_tarea_workflow`, ordenadas por `id_Estado DESC`.
- `workflow/ClassWorkflow.vb`: la firma de `Terminar_Tarea_Workflow` permite desactivar correo, actualización de interfaz y eventos dinámicos; el recorrido legado deja algunos valores por defecto y depende de `Page`.
- `Services/Workflow/Devolver/ServicioDevolverActividad.vb` e infraestructura asociada: son referencia de aislamiento, token, `GET_LOCK`, auditoría y punto mutante, pero sus contratos de actividad no se reutilizarán.
- `Doc/Actualizacion/workflow/DevolverUsuarioAnterior/Exploracion/`: define la separación obligatoria de actividad anterior, grupos y respuestas.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | El antecedente único es la segunda fila por `id_Estado DESC`; una fila inválida bloquea y no se omite. | `Class_estados_tarea_workflow.Solicita_datos_tarea_usuario_anterior_a_devolver` | D-01 | RQ-01 | 2.2, 2.3; Origen: D-01, RQ-01 |
| D-02 | El token opaco vincula tarea, estado actual y estado histórico por cinco minutos. | `TareaDevolverActividad.TokenVersion`, codecs modernos | D-02 | RQ-02 | 2.1, 2.3, 3.2; Origen: D-02, RQ-02 |
| D-03 | El lock es `GET_LOCK` exclusivo por tarea, independiente del token y liberado en la misma conexión. | `MySqlDevolverActividadConcurrencyGuard` | D-03 | RQ-03 | 2.4, 3.3; Origen: D-03, RQ-03 |
| D-04 | Permiso específico y auto-devolución se revalidan con el usuario Workflow autenticado. | `ClassWorkflow.Devolver_tarea_workflow_usuario_anterior` y contexto moderno | D-04 | RQ-01 | 2.2, 2.3, 3.1; Origen: D-04, RQ-01 |
| D-05 | Un adaptador exclusivo es el único punto mutante y usa el motor con `Page = Nothing`. | `ClassWorkflow.Terminar_Tarea_Workflow` | D-05 | RQ-04 | 2.5, 3.4; Origen: D-05, RQ-04 |
| D-06 | Correo, actualización legacy, eventos dinámicos y reasignaciones quedan en `0`; la capacidad no trata respuestas. | Parámetros opcionales de `Terminar_Tarea_Workflow` | D-06 | RQ-04 | 2.5, 3.5; Origen: D-06, RQ-04 |
| D-07 | Auditoría saneada usa `ASMX_DEVOLVER_USUARIO_ANTERIOR`; su fallo posterior es advertencia. | `WorkflowLegacyAuditoriaAdapter` | D-07 | RQ-05 | 2.5, 3.6; Origen: D-07, RQ-05 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Preview resuelve cero o un usuario histórico elegible sin escritura. | GIVEN historial válido WHEN preview THEN entrega solo usuario, actividad mínima y token; GIVEN ausencia, grupo, retiro, inconsistencia o auto-devolución THEN bloquea. | No mezclar con actividad anterior ni grupos. |
| RQ-02 | Ejecución acepta únicamente tarea y token y bloquea cambios de snapshot. | GIVEN token vigente WHEN historial es idéntico THEN permite continuar; WHEN cambia THEN bloquea sin alternativa. | El navegador no conoce el identificador histórico. |
| RQ-03 | Un intento por tarea llega al motor. | GIVEN solicitudes concurrentes, incluso con tokens distintos, WHEN compiten THEN una sola adquiere el lock. | No alterar el guard de otras operaciones. |
| RQ-04 | El motor recibe solo valores revalidados y no ejecuta correo, UI, eventos o respuestas. | GIVEN ejecución elegible WHEN invoca el adaptador THEN usa la matriz aprobada y una sola llamada. | `Terminar_Tarea_Workflow` conserva comportamiento para otros llamadores. |
| RQ-05 | La auditoría saneada informa resultado sin revertir éxito. | GIVEN falla de auditoría posterior WHEN mutación confirma THEN devuelve advertencia y referencia opaca. | No filtrar datos sensibles. |

## Reglas de trazabilidad obligatorias

1. Cada decision `D-XX` debe estar desarrollada en `design.md`, reflejada en al menos un requirement/scenario de `spec.md` y vinculada a una tarea mediante `Origen: D-XX, RQ-XX`.
2. Cada tarea con checkbox debe conservar su origen. Las tareas de validacion, rollout y documentacion tambien deben indicar la decision o requisito que verifican.
3. Las reglas de frontend, WebForms, Node u otro framework solo se agregan cuando el perfil tecnologico y el codigo afectado las justifican.
4. El estado solo puede cambiar a `approved` cuando no haya marcadores pendientes, las decisiones sean especificas y la matriz sea completa.

## Resultado del refinamiento

- Estado: aprobado por autorización explícita del usuario el 2026-08-26.
- Evidencia de decisiones: `Doc/Actualizacion/workflow/DevolverUsuarioAnterior/01-implementacion-devolver-usuario-anterior/`.
- Siguiente paso: sincronizar `design.md`, `spec.md` y `tasks.md`, e implementar exclusivamente DOC-36.
