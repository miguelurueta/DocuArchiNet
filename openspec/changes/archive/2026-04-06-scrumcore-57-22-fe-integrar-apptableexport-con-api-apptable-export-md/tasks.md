## 1. Contrato reusable de exportación backend

- [x] 1.1 Extender el contrato de `AppTableExport` para que el datasource pueda declarar una estrategia de exportación backend sin acoplar el componente shared a `/api/AppTable/export`.
- [x] 1.2 Definir reglas explícitas para decidir cuándo una exportación se resuelve localmente y cuándo debe delegarse al backend según modo y formato.

## 2. Integración con la API de exportación

- [x] 2.1 Crear o ajustar un servicio frontend reusable para consumir `POST /api/AppTable/export` y descargar el archivo final devuelto por la API.
- [x] 2.2 Mapear `AppTableExportFormat`, `AppTableExportMode`, `AppTableQueryState` y metadata del reporte al contrato backend esperado.

## 3. Ajustes en AppTableExport y primer consumidor real

- [x] 3.1 Actualizar `AppTableExport` para enrutar la descarga hacia la estrategia backend cuando el datasource actual la exponga.
- [x] 3.2 Integrar la capacidad backend real en `GestionCorrespondencia` como primer consumidor end-to-end sin duplicar lógica shared.

## 4. Validación automatizada y UX

- [x] 4.1 Agregar o ajustar pruebas de `AppTableExport` para cubrir descarga backend, formatos ejecutivos soportados, loading no destructivo y recuperación ante error.
- [x] 4.2 Agregar o ajustar pruebas del módulo consumidor para validar que `GestionCorrespondencia` usa la integración backend real con el contrato esperado y mantiene la tabla visible durante la exportación.
