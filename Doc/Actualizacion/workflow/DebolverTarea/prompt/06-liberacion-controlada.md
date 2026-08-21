# 06 — Liberación y operación controlada

## ROL ESPERADO

Actúa como responsable técnico de liberación para Workflow ASP.NET Web Forms, con foco en seguridad operativa y reversibilidad de despliegue.

## OBJETIVO

Preparar la decisión de liberación, matriz de ambientes y runbook operativo. Esta etapa no modifica ni despliega la funcionalidad.

## CONTEXTO OBLIGATORIO

- Requiere 05 aprobado y ausencia de bloqueos críticos.
- Leer `00-contexto-obligatorio.md`, evidencia de 05, versión aprobada y documentación operativa existente.
- La aprobación técnica de pruebas no equivale a autorización operativa por ambiente.

## REQUISITOS POSITIVOS

- Verificar que versión, artefactos, pruebas, auditoría, conectores entrantes, búsqueda paginada, experiencia moderna universal y aislamiento de respuestas estén identificados en evidencia.
- Crear matriz por ambiente: autorización, versión, alcance funcional, ventana, responsables, evidencia y continuación.
- Crear runbook para una operación autorizada: verificaciones `SELECT`, comprobación sanitizada y reversión mediante gestión de despliegue aprobada, sin reactivar una ruta UI alternativa.

## RESTRICCIONES CRÍTICAS

- No editar configuración, desplegar, ejecutar E2E/carga ni usar o registrar secretos.
- No inferir autorización para un ambiente a partir de pruebas o autorización de otro ambiente.
- No revertir transiciones confirmadas ni tocar las operaciones fuera de alcance.

## REGLAS DE ANTIRREGRESIÓN

- Una reversión de despliegue solo afecta nuevos intentos y no altera tareas ya terminadas.
- La devolución conserva su ruta moderna oficial y las operaciones existentes conservan sus contratos.

## CRITERIOS DE ACEPTACIÓN

- La decisión es una sola: **bloquear**, **solicitar aprobación** o **lista para despliegue autorizado**.
- La matriz identifica cada ambiente sin secretos y ningún despliegue queda implícito.

## PRUEBAS OBLIGATORIAS

No ejecutar E2E, carga ni cambios de ambiente. Verificar documentalmente y con consultas autorizadas de solo lectura que la evidencia de 05, ruta moderna universal, aislamiento de respuestas y reversión de despliegue es completa; registrar resultado y limitaciones.

## DOCUMENTACIÓN TÉCNICA

Actualizar el paquete documental con decisión, matriz de ambientes, runbook, responsables, aprobaciones requeridas y riesgos residuales, sin secretos.

## ENTREGABLE FINAL

Reportar ticket, precondiciones y decisión de liberación; confirmar que no se modificó configuración ni se ejecutó un despliegue.
