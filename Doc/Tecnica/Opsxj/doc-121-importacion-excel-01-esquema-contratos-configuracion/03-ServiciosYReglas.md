# IMPORTACION-EXCEL-01-ESQUEMA-CONTRATOS-CONFIGURACION

- Ticket: DOC-121
- Cambio OpenSpec: doc-121-importacion-excel-01-esquema-contratos-configuracion
- Clasificacion: cross_cutting (Transversal)
## Servicios y reglas

La fase 01 define servicios futuros de plantillas, lectura, preparación y confirmación; no los compila o publica. Reutiliza la resolución de conexiones por sesión, el patrón de guardas y la validación ValorCompatible, con contexto/permiso propio del cargador.

Subida completa multipart, validación completa y confirmación transaccional por ID. AGREGAR rechaza cualquier conflicto; REEMPLAZAR conserva estructura y revierte DELETE/INSERT juntos. CREATE identificado por comentario de carga constituye una fase estructural distinta. ConsultarResultado no repite escrituras y CONFIRMADA conserva su resultado histórico. Duplicados se comparan con tipo/collation real, no con igualdad del navegador.

La instalación no modifica consumidores SII, materialización de valores ni el almacenamiento compartido de carpetas. Los límites iniciales son 10000 filas y 10000000 bytes por carga, independientes y configurables por servidor; la medición real pertenece a las fases ejecutables.
