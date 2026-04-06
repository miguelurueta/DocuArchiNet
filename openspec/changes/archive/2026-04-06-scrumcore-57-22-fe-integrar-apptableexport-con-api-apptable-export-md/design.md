## Context

`AppTableExport` ya resuelve la capa reusable de UI con `AppDropdown`, `AppTableQueryWrapper`, modos `currentPage`, `selectedRows`, `allLoaded` y `allMatching`, pero hoy sigue materializando la descarga en frontend y solo soporta `csv` real. En paralelo, el backend ya expone `POST /api/AppTable/export`, que devuelve el archivo final y acepta un contrato de exportación basado en la misma semántica de consulta de `workflowInboxgestion`.

La brecha actual no es visual sino de integración operativa: el componente reusable todavía no sabe delegar la descarga al backend cuando el flujo lo requiere. Eso impide completar la arquitectura prevista para `xlsx`, `pdf` y, en general, para `allMatching` server-side con archivo final generado por API.

Este cambio debe consolidar `AppTableExport` como pieza reusable capaz de convivir con dos estrategias:
- exportación local para datasets ya presentes en frontend
- exportación backend para modos o formatos que requieren archivo generado por API

La integración debe seguir desacoplada del módulo concreto. `GestionCorrespondencia` puede ser el primer consumidor real, pero el contrato de `AppTableExport` no debe quedar amarrado a `workflowInboxgestion` ni a un DTO puntual.

## Goals / Non-Goals

**Goals:**
- Integrar `AppTableExport` con una estrategia reusable de exportación backend usando `/api/AppTable/export`.
- Mantener `AppDropdown` y `paginationActions` como patrón visual oficial del trigger.
- Permitir que el datasource declare cuándo una exportación debe resolverse por backend en lugar de por filas locales.
- Habilitar archivo final server-side para formatos como `xlsx` y `pdf`, y para `allMatching` cuando el dataset no vive completo en frontend.
- Propagar `queryState` y metadata del reporte hacia el flujo backend sin acoplar `AppTableExport` a un módulo específico.

**Non-Goals:**
- No rediseñar `AppDropdown` ni `AppTableQueryWrapper`.
- No mover la lógica backend dentro de `AppTable.tsx`.
- No convertir `AppTableExport` en un componente específico de `GestionCorrespondencia`.
- No redefinir el endpoint backend existente si el contrato ya resulta suficiente.

## Decisions

### 1. AppTableExport debe soportar estrategia híbrida: local o backend

El componente reusable debe poder resolver una exportación por dos rutas:
- local: usa filas ya presentes (`currentPage`, `selectedRows`, `allLoaded`, o un `allMatching` basado en dataset remoto)
- backend: delega en una operación async que devuelve el archivo final

La alternativa de mantener solo una estrategia local se descarta porque contradice la arquitectura maestra y deja fuera `xlsx`/`pdf`. La alternativa de mover toda exportación a backend también se descarta por ahora porque `csv` local sigue siendo útil para casos simples y ya existe funcionalmente.

### 2. La integración backend debe entrar por un contrato reusable del datasource

El componente no debe conocer `/api/AppTable/export` directamente. Debe recibir una capacidad declarativa adicional desde el datasource o strategy inyectable, por ejemplo una operación equivalente a `exportFile(...)` o `runServerExport(...)`, responsable de:
- construir el request backend
- ejecutar la llamada
- resolver bytes/blob/nombre de archivo

La alternativa de hardcodear el endpoint en `AppTableExport.tsx` se descarta porque rompería el desacople del shared layer.

### 3. La selección entre export local y export backend depende de modo y formato

Regla recomendada:
- `currentPage`, `selectedRows`, `allLoaded`: pueden seguir resolviéndose localmente en `csv`
- `allMatching`: debe poder resolverse por backend cuando el datasource lo declare
- `xlsx` y `pdf`: deben preferir backend cuando exista capacidad server-side

La alternativa de decidir solo por formato o solo por modo se descarta porque el sistema necesita flexibilidad suficiente para convivir con tablas client-side y server-side.

### 4. El request backend debe reutilizar query state y metadata de reporte

El backend ya expone un contrato con `Search`, `SearchType`, `SortField`, `SortDir`, `Page`, `PageSize`, `StructuredFilters`, `Format`, `ExportMode` y `ReportTitle`. La integración frontend debe traducir desde:
- `AppTableQueryState`
- `AppTableExportMode`
- `AppTableExportFormat`
- `AppTableExportReportMeta`

La alternativa de enviar solo filas o metadata parcial se descarta porque no asegura coherencia entre tabla visible y archivo exportado.

### 5. La descarga backend debe mantener el mismo patrón visual no destructivo

Aunque la exportación se delegue a API, `exportLoading` debe seguir siendo el único loading visible. La tabla no debe activar skeleton ni overlay destructivo. Esto preserva la UX ya consolidada en los tickets previos.

### 6. GestionCorrespondencia será el primer consumidor de la exportación backend real

Se usará `GestionCorrespondencia` como integración inicial porque ya dispone de query state, metadata de reporte, datasource real y endpoint backend compatible. La alternativa de crear primero un consumidor ficticio o abstracto se descarta porque no valida el flujo end-to-end.

## Risks / Trade-offs

- [Riesgo: duplicar contratos entre frontend y backend] → Mitigación: mapear el contrato reusable de `AppTableExport` sobre el DTO backend existente, en lugar de introducir un request paralelo innecesario.
- [Riesgo: acoplar la solución a WorkflowInbox] → Mitigación: encapsular la llamada real a `/api/AppTable/export` en el adapter del módulo o en un servicio reusable, no en el componente shared.
- [Riesgo: mezclar semántica local y server-side sin reglas claras] → Mitigación: definir una política explícita por formato y modo dentro del datasource/strategy.
- [Riesgo: degradar UX durante descargas largas] → Mitigación: mantener `exportLoading` aislado del loading de tabla y bloquear solo el menú de exportación.
- [Riesgo: dejar `xlsx` y `pdf` visibles pero no ejecutables] → Mitigación: exponer esos formatos solo cuando exista una capacidad backend real para el datasource actual.

## Migration Plan

1. Definir el contrato reusable que permita a `AppTableExport` distinguir export local de export backend.
2. Crear o ajustar un servicio frontend para consumir `/api/AppTable/export`.
3. Integrar esa capacidad en `GestionCorrespondencia` como primer datasource backend real.
4. Ajustar `AppTableExport` para enrutar formatos/modos hacia la estrategia correcta.
5. Cubrir con pruebas del reusable y del módulo consumidor los casos de descarga backend, loading y errores.

Rollback:
- Si la integración backend introduce regresión, se puede volver temporalmente al flujo local existente manteniendo el contrato visual y los modos ya implementados.
- El rollback no requiere tocar `AppDropdown` ni `AppTableQueryWrapper`, solo la estrategia de exportación.

## Open Questions

- Si `csv` para `allMatching` debe seguir permitiendo fallback local por dataset remoto o si toda exportación `allMatching` debe consolidarse en backend cuando exista capacidad server-side.
- Si el contrato backend necesita ampliar `ReportTitle` a metadata más rica o si la metadata faltante debe resolverse internamente en la API a partir del contexto del módulo.
