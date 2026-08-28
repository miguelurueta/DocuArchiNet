## Why

DOC-38 aprobó la evidencia técnica y de QA para Devolver → Usuario anterior, pero esa aprobación no constituye autorización operativa de un ambiente. DOC-39 prepara una decisión de liberación controlada, trazable y reversible sin desplegar ni alterar Workflow.

## What Changes

- Define la precondición documental que separa evidencia técnica de autorización por ambiente.
- Establece una matriz de ambiente sin secretos para autorización, versión, alcance, ventana, responsables, evidencia y continuación.
- Define un runbook para una operación futura autorizada, con controles `SELECT` y reversión mediante la gestión de despliegue aprobada.
- Registra una única decisión de salida: solicitar aprobación, bloquear o lista para despliegue autorizado.
- No modifica código de producción, configuración, contratos, tareas, auditoría ni datos de negocio.

## Capabilities

### New Capabilities

- `liberacion-controlada`: gobierna la preparación documental y la decisión verificable de liberar Devolver → Usuario anterior por ambiente.

### Modified Capabilities

- Ninguna. DOC-39 no cambia el comportamiento de la devolución ni de las operaciones vecinas.

## Impact

- Documentación operativa y de evidencia bajo `Doc/Actualizacion/workflow/DevolverUsuarioAnterior/`.
- Matriz y runbook técnico bajo `Doc/Tecnica/Opsxj/`.
- Sin cambios en `workflow/`, servicios ASMX, repositorios, configuración, APIs ni tablas de negocio.
