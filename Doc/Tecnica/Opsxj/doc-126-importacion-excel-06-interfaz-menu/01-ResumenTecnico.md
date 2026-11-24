# DOC-126 — Interfaz Excel y menú

- Ticket: DOC-126
- Cambio OpenSpec: doc-126-importacion-excel-06-interfaz-menu
- Clasificacion: webforms_ui (WebForms UI)
- Estado: refinamiento técnico; implementación y aceptación aún sin ejecutar.

## Objetivo

Integrar el recorrido de plantillas, libro, hoja, preparación y confirmación en Producción Documental, usando permiso Excel propio y servicios DOC122–125. El plan define15decisiones,15requisitos,45escenarios y44tareas, sin cambiar la semántica de las transacciones existentes.

## Alcance y compatibilidad

Inventario y decisiones vigentes en OpenSpec design.md. Mantener plataforma y conexiones actuales,20000filas/10MB y datos anteriores. El constructor de menú existente funciona sin host de carpetas; reutilizarlo. Sólo se prevén tres assets Content y un helper de recuperación en la guarda actual; sin nueva arquitectura de backend. Esta entrega documenta el plan y no acredita código ni E2E DOC126.
