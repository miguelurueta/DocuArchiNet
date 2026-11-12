# DOC-119 — servicios y reglas

- Cambio: `doc-119-produccion-importacion-carga-carpeta-seguimiento`.
- Clasificación: `cross_cutting`.

## Servicios y reglas

ServicioConsultaImportacion y MySqlImportacionCarpetasRepository.Consulta permanecen sin cambios. ConsultarLote/ConsultarRecursosLote/ExportarPaginaLote/ConsultarConfirmacionLote reautorizan propietario/ámbito/nivel/recursos. CSV conserva CeldaCsv y snapshot por página, sin reconciliación ni avance.

La corrección está en cliente: solicitudes incluyen filtros DTO existentes y CSV copia la solicitud de página consultada. Recepción y efecto siguen separados. Filtros no otorgan autoridad; un rechazo de consulta elimina resultados representados anteriores.

Recuperación por IDs y Doc111ActualizarRama se conservan; las pruebas previas de recarga/identidad/árbol pasan. Backend Framework/DDL: 2/2 PASS en la regresión combinada anterior al último ajuste de teclado, sin fuentes backend modificadas después. Sus casos internos no se suman otra vez.

Limpieza continúa deshabilitada, sin invocación ni decisión SEG-03. PDF/rechazo TIFF, integridad, esquema y efectos existentes se preservan.
