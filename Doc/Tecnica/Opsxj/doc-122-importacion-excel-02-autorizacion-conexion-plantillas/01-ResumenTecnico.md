# DOC-122 — IMPORTACION-EXCEL-02-AUTORIZACION-CONEXION-PLANTILLAS

- Ticket: DOC-122
- Cambio OpenSpec: doc-122-importacion-excel-02-autorizacion-conexion-plantillas
- Clasificacion: cross_cutting

## Objetivo
Administrar acceso y plantillas Excel desde un ASMX tipado, con permiso independiente, conexión DA dedicada y guardado transaccional compatible con catálogos históricos.

## Alcance y compatibilidad
Fase02 de7. Agrega contexto, guardia de solicitud, adaptador de conexión DA del módulo, modelos, DTO, repositorio y servicio. Integra únicamente el indicador de permiso en la clase existente y registra archivos en vbproj. Conserva SII, rutas y mapeos históricos; EGRESOS intacto. Upload, lectura, preparación, confirmación y UI corresponden a otras fases. Código y pruebas locales listos; integración real abierta. Rollback de código conserva esquema aditivo y datos, sin limpieza automática.
