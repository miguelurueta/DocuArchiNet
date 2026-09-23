# Pruebas y evidencia

- Ticket: DOC-78
- Cambio OpenSpec: doc-78-proteccion-contexto
- Clasificacion: cross_cutting

## Evidencia requerida

Ejecución del 23 de septiembre de 2026:

- `node --test Tests/importar-servicio-web-*.test.cjs`: 434 aprobadas, 0 fallidas.
- `msbuild GestionDocumental-Docuarchi.net.sln /t:Build /p:Configuration=Debug /m`: 0 errores y 310 advertencias preexistentes.

Las suites focales cubren contexto inmutable, preflight anterior al efecto, bloqueo/restauración, códigos normativos, recuperación con `IntentId`, señal multi-pestaña y rechazo de documentos de otra tarea.

## QA/E2E WebForms

El E2E debe ejecutarse únicamente con autorización explícita, usando la infraestructura reutilizable y el runbook. Debe confirmar tarea original, cambio de contexto, recuperación sin reejecución, ausencia de cambios en preview y restauración final del gate moderno.
