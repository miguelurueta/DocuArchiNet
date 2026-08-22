# DOC-31 — Liberación controlada de Enviar a usuario

## Why

DOC-30 aprobó la verificación transversal de la capacidad integrada de Enviar a usuario. La liberación requiere una decisión operativa explícita por ambiente, una matriz sin secretos y un runbook reversible; la evidencia técnica por sí sola no autoriza ningún despliegue.

## What Changes

- Consolidar la versión integrada, sus evidencias y los invariantes de operación en una decisión de liberación única.
- Crear una matriz de autorización por ambiente que declare que actualmente no hay un ambiente elegible ni una ventana aprobada.
- Crear un runbook para una futura operación autorizada con verificaciones sanitizadas de solo lectura, condiciones de abortar y reversión por el mecanismo de despliegue aprobado.
- Documentar que la reversión afecta únicamente intentos nuevos y conserva la ruta moderna de usuario y el contrato de Continuar flujo.

## Non-Goals

- No desplegar, editar configuración, activar gates, ejecutar E2E/carga ni consultar ambientes sin autorización.
- No modificar código, contratos, datos, auditoría, tareas ni respuestas.
- No inferir responsables nominales, ventanas ni autorizaciones desde pruebas previas.

## Capabilities

### New Capabilities

- `liberacion-controlada-enviar-usuario`: decisión y procedimiento documental para operar la capacidad moderna de forma autorizada y reversible.

### Modified Capabilities

- Ninguna capacidad funcional; DOC-31 no cambia el comportamiento entregado por DOC-28 y DOC-29.

## Impact

- Código y configuración: sin modificaciones.
- Operación: solicitud de aprobación por ambiente, sin despliegue implícito.
- Documentación: matriz de liberación, runbook, riesgos y enlaces al dictamen DOC-30.
