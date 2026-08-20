# 06 — Liberación y activación controlada

## ROL ESPERADO

Actúa como responsable técnico de liberación para Workflow ASP.NET Web Forms, con foco en seguridad operativa y reversibilidad.

## OBJETIVO

Preparar la decisión de liberación, matriz de ambientes y runbook reversible. Esta etapa no modifica, despliega ni activa la funcionalidad.

## CONTEXTO OBLIGATORIO

- Requiere 05 aprobado y ausencia de bloqueos críticos.
- Leer `00-contexto-obligatorio.md`, evidencia de 05, configuración aprobada y documentación de gate/rollback existente.
- La aprobación técnica de pruebas no equivale a autorización operativa por ambiente.

## REQUISITOS POSITIVOS

- Verificar que versión, artefactos, pruebas, gate único, auditoría, respuesta pendiente, fallback y Continuar flujo están identificados en evidencia.
- Crear matriz por ambiente: autorización, versión, alcance usuarios/grupos, ventana, responsables, evidencia, continuación y rollback.
- Crear runbook para futura operación autorizada: verificaciones `SELECT`, uso exclusivo del gate existente, comprobación sanitizada y rollback a `WorkflowCentroTrabajoModernActive=false` con usuarios/grupos vacíos.

## RESTRICCIONES CRÍTICAS

- No activar, desactivar ni editar configuración; no desplegar, ejecutar E2E/carga ni usar/registrar secretos.
- No inferir autorización global a partir de pruebas, listas vacías o autorización de otro ambiente.
- No revertir transiciones confirmadas, reasignar respuestas ni tocar Continuar flujo.

## REGLAS DE ANTIRREGRESIÓN

- El rollback solo afecta nuevos intentos mediante el gate; no altera tareas ya terminadas.
- El fallback Web Forms y el contrato `IdConector` de Continuar flujo permanecen intactos.

## CRITERIOS DE ACEPTACIÓN

- La decisión es una sola: **bloquear**, **solicitar aprobación** o **lista para activación autorizada**.
- La matriz identifica cada ambiente sin secretos y ninguna activación queda implícita.

## PRUEBAS OBLIGATORIAS

No ejecutar E2E, carga ni cambios de ambiente. Verificar de forma documental y con consultas autorizadas de solo lectura que la evidencia de 05, gate, fallback y rollback es completa; registrar resultado y limitaciones.

## DOCUMENTACIÓN TÉCNICA

Actualizar el paquete documental existente con decisión, matriz de ambientes, runbook, responsables, aprobaciones requeridas y riesgos residuales, sin secretos.

## ENTREGABLE FINAL

Reportar ticket, precondiciones y decisión de liberación; confirmar que no se modificó gate ni se ejecutó una activación.
