# Arquitectura

- Ticket: DOC-54
- Cambio OpenSpec: doc-54-reconciliacion-lista-documetos
- Clasificacion: cross_cutting

`ServicioReconciliacionImportacion` es el coordinador read-only de Get/Reconcile. Revalida el contexto, solicita un snapshot autorizado al repositorio y proyecta resultados con `ImportItemResultMapper`. `MySqlImportReconciliationRepository` es la única clase nueva con SQL y usa la infraestructura parametrizada compartida.

El núcleo modela intención, item y cardinalidad documental sin DTO, SQL o Session. ASMX, JavaScript, `ClassAlmacenamiento`, `dato_lista`, escritura vigente y cachés SII permanecen intactos. La integración de compatibilidad para la grilla queda reservada al Backend 06.
