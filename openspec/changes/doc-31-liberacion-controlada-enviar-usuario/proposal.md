## Why

LIBERACION-CONTROLADA-ENVIAR-USUARIO. Ver detalle funcional completo del ticket en la seccion Jira Details.

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue DOC-31.
- Se formaliza una propuesta OpenSpec inicial derivada del ticket Jira.
- Se captura el resumen y la descripcion del ticket como punto de partida para refinement posterior.
- Se deja lista una base coherente para continuar con design, specs y tasks.

## Jira Details

> # 04 — Liberación y operación controlada
> 
> ## ROL ESPERADO
> 
> Actúa como responsable técnico de liberación para Workflow ASP.NET Web Forms, con foco en seguridad operativa y reversibilidad.
> 
> ## OBJETIVO
> 
> Preparar la decisión de liberación, matriz de ambientes y runbook operativo. Esta etapa no modifica ni despliega la funcionalidad.
> 
> ## CONTEXTO OBLIGATORIO
> 
> - Requiere 03 aprobado y ausencia de bloqueos críticos.
> - Leer `00-contexto-obligatorio.md`, evidencia de 03, versión aprobada y documentación operativa existente.
> - La aprobación técnica de pruebas no equivale a autorización operativa por ambiente.
> 
> ## REQUISITOS POSITIVOS
> 
> - Verificar que versión, artefactos, pruebas, auditoría, respuesta pendiente, experiencia moderna universal y Continuar flujo están identificados en evidencia.
> - Crear matriz por ambiente: autorización, versión, alcance funcional, ventana, responsables, evidencia y continuación.
> - Crear runbook para futura operación autorizada: verificaciones `SELECT`, comprobación sanitizada y procedimiento de reversión mediante la gestión de despliegue aprobada, sin reactivar una ruta UI alternativa.
> 
> ## RESTRICCIONES CRÍTICAS
> 
> - No editar configuración; no desplegar, ejecutar E2E/carga ni usar/registrar secretos.
> - No inferir autorización para un ambiente a partir de pruebas o de la autorización de otro ambiente.
> - No revertir transiciones confirmadas, reasignar respuestas ni tocar Continuar flujo.
> 
> ## REGLAS DE ANTIRREGRESIÓN
> 
> - Una reversión de despliegue solo afecta nuevos intentos y no altera tareas ya terminadas.
> - El contrato `IdConector` de Continuar flujo permanece intacto; Enviar a usuario conserva su ruta moderna oficial.
> 
> ## CRITERIOS DE ACEPTACIÓN
> 
> - La decisión es una sola: **bloquear**, **solicitar aprobación** o **lista para despliegue autorizado**.
> - La matriz identifica cada ambiente sin secretos y ningún despliegue queda implícito.
> 
> ## PRUEBAS OBLIGATORIAS
> 
> No ejecutar E2E, carga ni cambios de ambiente. Verificar de forma documental y con consultas autorizadas de solo lectura que la evidencia de 03, la ruta moderna universal y la reversión de despliegue es completa; registrar resultado y limitaciones.
> 
> ## DOCUMENTACIÓN TÉCNICA
> 
> Actualizar el paquete documental existente con decisión, matriz de ambientes, runbook, responsables, aprobaciones requeridas y riesgos residuales, sin secretos.
> 
> ## ENTREGABLE FINAL
> 
> Reportar ticket, precondiciones y decisión de liberación; confirmar que no se modificó configuración ni se ejecutó un despliegue.

## Jira Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: ENVIAR, LIBERACION, USUARIO

## Capabilities

### New Capabilities
- `liberacion-controlada-enviar-usuario`: Capacidad derivada del ticket Jira para continuar el refinamiento funcional en OpenSpec.

### Modified Capabilities
- 

## Impact

- Nueva propuesta inicial en `openspec/changes/<changeName>/proposal.md`.
- Impacto funcional pendiente de refinamiento en los siguientes artefactos OpenSpec.

