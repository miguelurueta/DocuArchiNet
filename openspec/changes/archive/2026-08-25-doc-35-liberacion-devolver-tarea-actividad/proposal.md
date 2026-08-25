## Why

La implementación y la verificación transversal de Devolver a actividad anterior ya tienen evidencia técnica aprobada, pero no existe autorización operativa por ambiente. Se requiere una decisión de liberación única, una matriz explícita y un runbook reversible para evitar que una evidencia técnica se interprete como despliegue autorizado.

## What Changes

- Se documenta la decisión vigente de solicitar aprobación operativa para la versión integrada de la capacidad.
- Se crea una matriz de ambientes sin secretos que obliga a identificar autorización, versión, alcance, ventana, responsables, evidencia y continuación para cada ambiente.
- Se define un runbook de controles SELECT autorizados, registro saneado, aborto y reversión mediante el proceso de despliegue aprobado.
- Se preservan la ruta moderna oficial, los contratos de las operaciones vecinas y la prohibición de reactivar la UI Web Forms heredada.

## Jira Details

> # 04 — Liberación y operación controlada
> 
> ## ROL ESPERADO
> 
> Actúa como responsable técnico de liberación para Workflow ASP.NET Web Forms, con foco en seguridad operativa y reversibilidad de despliegue.
> 
> ## OBJETIVO
> 
> Preparar la decisión de liberación, matriz de ambientes y runbook operativo. Esta etapa no modifica ni despliega la funcionalidad.
> 
> ## CONTEXTO OBLIGATORIO
> 
> - Requiere 03 aprobado y ausencia de bloqueos críticos.
> - Leer `00-contexto-obligatorio.md`, `../Exploracion/`, evidencia de 03, versión aprobada y documentación operativa existente.
> - La aprobación técnica de pruebas no equivale a autorización operativa por ambiente.
> 
> ## REQUISITOS POSITIVOS
> 
> - Verificar que versión, artefactos, pruebas, auditoría, aristas entrantes Ruta/Flujo, búsqueda paginada, lock por tarea, ruta moderna única y aislamiento de respuestas estén identificados en evidencia.
> - Crear matriz por ambiente: autorización, versión, alcance funcional, ventana, responsables, evidencia y continuación.
> - Crear runbook para una operación autorizada: verificaciones `SELECT`, comprobación sanitizada y reversión mediante gestión de despliegue aprobada, sin reactivar postback ni ruta UI alternativa.
> 
> ## RESTRICCIONES CRÍTICAS
> 
> - No editar configuración, desplegar, ejecutar E2E/carga ni usar o registrar secretos.
> - No inferir autorización para un ambiente a partir de pruebas o autorización de otro ambiente.
> - No revertir transiciones confirmadas ni tocar las operaciones fuera de alcance.
> 
> ## REGLAS DE ANTIRREGRESIÓN
> 
> - Una reversión de despliegue solo afecta nuevos intentos y no altera tareas ya terminadas.
> - La devolución conserva su ruta moderna oficial y las operaciones existentes conservan sus contratos.
> 
> ## CRITERIOS DE ACEPTACIÓN
> 
> - La decisión es una sola: **bloquear**, **solicitar aprobación** o **lista para despliegue autorizado**.
> - La matriz identifica cada ambiente sin secretos y ningún despliegue queda implícito.
> 
> ## PRUEBAS OBLIGATORIAS
> 
> No ejecutar E2E, carga ni cambios de ambiente. Verificar documentalmente y con consultas autorizadas de solo lectura que la evidencia de 03, aristas Ruta/Flujo, ruta moderna única, aislamiento de respuestas y reversión de despliegue es completa; registrar resultado y limitaciones.
> 
> ## DOCUMENTACIÓN TÉCNICA
> 
> Actualizar el paquete documental con decisión, matriz de ambientes, runbook, responsables, aprobaciones requeridas y riesgos residuales, sin secretos.
> 
> ## ENTREGABLE FINAL
> 
> Reportar ticket, precondiciones y decisión de liberación; confirmar que no se modificó configuración ni se ejecutó un despliegue.

## Jira Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: ACTIVIDAD, ANTERIOR, DEVOLVER, LIBERACION

## Capabilities

### New Capabilities
- liberacion-devolver-tarea-actividad: paquete documental que controla la decisión y la operación futura de liberación de Devolver a actividad anterior.

### Modified Capabilities
- 

## Impact

- Documentación de actualización Workflow, artefactos OpenSpec y documentación técnica OPSXJ.
- No cambia código, contratos, configuración, datos, auditoría, tareas de Workflow ni ambientes.
