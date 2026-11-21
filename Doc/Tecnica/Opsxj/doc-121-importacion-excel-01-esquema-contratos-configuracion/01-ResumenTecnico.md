# IMPORTACION-EXCEL-01-ESQUEMA-CONTRATOS-CONFIGURACION

- Ticket: DOC-121
- Cambio OpenSpec: doc-121-importacion-excel-01-esquema-contratos-configuracion
- Clasificacion: cross_cutting (Transversal)
## Objetivo

Entregar la fase 01 contractual de importación XLS/XLSX a tablas imp_02_: inventario, esquema revisable, request/response y configuración. El usuario eligió el flujo de los prompts actuales y ejecutará los archivos SQL manualmente.

## Alcance y compatibilidad

Se preservan los tres catálogos imp01_*, consumidores SII, perfiles, fuentes y proveedor de índices de carpetas. Se agregan únicamente artefactos OpenSpec, documentación, SQL revisable y un auditor local de esa entrega. Sin código VB compilable, endpoints, UI, cambios efectivos de configuración, DLL o despliegue. La reversión conserva objetos aditivos; cualquier cambio estructural posterior se entrega en otro SQL para ejecución manual.

Contrato: Doc/Actualizacion/Importacion/Diseno/01-contratos-configuracion-doc121.md. Guía: Doc/Actualizacion/Importacion/Sql/README.md. La estructura base 01–03 se verificó por SELECT en el ambiente físico inspeccionado; 08 de CONTRATOS se aplicó y verificó por autorización expresa posterior. Las 16 tareas de la fase contractual están completas. EGRESOS es solo de pruebas por aclaración del usuario y se conserva intacta; su adecuación no condiciona esta entrega. La cuenta/host/DA efectiva se acredita al implementar la fase de conexión, sin dar por verificadas sus concesiones aquí.
