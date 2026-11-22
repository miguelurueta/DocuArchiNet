# DOC-123 — Lectura y validación Excel

- Ticket: DOC-123
- Cambio OpenSpec: doc-123-importacion-excel-03-lectura-validacion-xls-xlsx
- Clasificacion: cross_cutting (Transversal)

## Objetivo

Planificar el lector interno XLS/XLSX con filas normalizadas e incidencias antes de cualquier carga. Refinamiento revisado2026-11-22; no implementado todavía.

## Alcance y compatibilidad

Reutiliza modelos/plantillas DOC121/DOC122, conexión DA del módulo y harness existente. Mantiene net461, límites10000/10MB decimal, SII/FastExcel y huella v1. No crea endpoints, UI, SQL ni carga real. Design/spec/refinement/tasks de OpenSpec constituyen el plan vigente. La observabilidad de errores/fórmulas del paquete3.6.0 es compuerta previa a integración.
