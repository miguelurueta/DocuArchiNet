## Why

DOC-80 publicó la lectura segura de anexos SII para actividades ENLASE, pero la ruta de escritura disponible sigue acoplada a sesión, retorna texto/`YES` y no ofrece una confirmación autoritativa de los efectos físicos y lógicos. DOC-81 cierra esa frontera para importar una selección individual o múltiple de forma idempotente, reconciliable y sin asignar la tarea.

## What Changes

- Extender la preparación moderna para tipologías autorizadas y selección de colección ENLASE.
- Crear o reutilizar una única intención por selección con idempotencia y exclusión concurrente.
- Revalidar el contexto ENLASE antes de cualquier efecto.
- Adaptar la única función legacy de almacenamiento al puerto moderno, sin duplicar su lógica.
- Confirmar registro lógico, relación con la tarea y existencia física antes de anunciar éxito.
- Persistir y reconciliar resultados por elemento, incluidos parcial, recuperable e incierto.
- Mantener fuera del orquestador la asignación/cierre de tarea y preservar la importación de constancias.
- Agregar pruebas focales, caracterización, integración y la definición E2E reutilizando la plataforma existente.

## Capabilities

### New Capabilities

- `reconciliacion-persistencia-servicio-enlace-sii`: preparación, persistencia idempotente y reconciliación de anexos SII en tareas ENLASE.

### Modified Capabilities

- `importar-servicio-web`: reutiliza intención, preflight, orquestación, estados, repositorios y endpoints existentes para una capacidad adicional, sin cambiar el contrato de constancias.

## Impact

- Código: `DTOs/Workflow/ImportarServicioWeb/`, `Modelo/Workflow/ImportarServicioWeb/`, `Services/Workflow/ImportarServicioWeb/`, `Infrastructure/Repositories/Workflow/ImportarServicioWeb/`, composición ASMX y un adaptador explícito hacia `workflow/ClassAlmacenamiento.vb`.
- Datos: se reutiliza la persistencia de intención/resultados; cualquier extensión de esquema debe ser aditiva y estar respaldada por repositorio y rollback.
- Seguridad: contexto y autoridad provienen de sesión/repositorios; no se aceptan rutas, permisos o autoridad desde el navegador.
- Operación: no se ejecutarán pruebas mutadoras reales sin autorización expresa y datos descartables.