# DOC-73 — Resumen técnico

- Ticket: DOC-73
- Cambio OpenSpec: doc-73-adaptador-interfaz-integracion-sii
- Clasificacion: cross_cutting

## Objetivo

Agregar el adaptador frontend de consulta/listado para `INTEGRACIONSII` sobre el núcleo DOC-72, consumiendo únicamente el cliente API moderno y los DTOs normalizados.

## Alcance y compatibilidad

Se agregan mapper, modelo local y adaptador bajo `js/workflow/importar-servicio-web/sii/`. Se conserva el gate apagado, el recorrido legacy y la responsabilidad de autorización del backend. No se modifican endpoints, almacenamiento, caché ni auditoría.
