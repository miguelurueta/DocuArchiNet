## Context

DOC-31: LIBERACION-CONTROLADA-ENVIAR-USUARIO

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

## Goals / Non-Goals

**Goals**
- Refinar alcance tecnico usando el contexto completo de Jira.
- Definir decisiones arquitectonicas, riesgos y plan de migracion.

**Non-Goals**
- Cambios fuera del alcance descrito por el ticket.

## Decisions

1. Las decisiones funcionales y tecnicas se completan durante `opsxj:refine`; no se inyectan politicas de otro perfil tecnologico.


## Risks / Trade-offs

- El refinamiento debe identificar compatibilidad, riesgos y limites del modulo afectado antes de iniciar cambios.

## Migration Plan

1. Completar y aprobar `refinement.md` antes de marcar tareas de implementacion.
2. Sincronizar cada decision con design, spec y tasks mediante `opsxj:refine --sync`.

## Open Questions

- TBD
