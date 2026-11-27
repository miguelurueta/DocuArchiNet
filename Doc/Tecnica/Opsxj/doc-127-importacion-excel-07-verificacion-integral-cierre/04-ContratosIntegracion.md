# DOC-127 — Verificación integral, capacidad y cierre

- Ticket: DOC-127
- Cambio OpenSpec: doc-127-importacion-excel-07-verificacion-integral-cierre
- Clasificacion: cross_cutting; perfil legacy-webforms-vb.

## Contratos e integraciones

Reutilizar ASMX/ASHX, sesión/CSRF, conexión del módulo y DTOs existentes; el navegador envía referencias, no conexiones. La tabla importada sólo sirve como fuente cuando la política, metadatos y proveedor/versiones publicados permiten su consulta. MySqlProveedorDatosIndices usa igualdad por clave y filtro de empresa opcional con LIMIT2, preservando OK/SIN_COINCIDENCIA/AMBIGUO. EXPLAIN se entrega para ejecución manual y se contrasta con metadatos SELECT. Los controles no publican fuentes ni cambian SII.
