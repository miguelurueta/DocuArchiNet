# DOC-119 — servicios y reglas

- Ticket: DOC-119
- Cambio OpenSpec: doc-119-produccion-importacion-carga-carpeta-seguimiento
- Clasificacion: cross_cutting

## Servicios y reglas

ServicioConsultaImportacion y MySqlImportacionCarpetasRepository.Consulta permanecen sin cambios. ConsultarLote/ConsultarRecursosLote/ExportarPaginaLote/ConsultarConfirmacionLote reautorizan propietario/ámbito/nivel/recursos. CSV conserva CeldaCsv y snapshot por página, sin reconciliación ni avance.

La corrección está en cliente: solicitudes incluyen filtros DTO existentes y CSV copia la solicitud de página consultada. Recepción y efecto siguen separados. Filtros no otorgan autoridad; un rechazo de consulta elimina resultados representados anteriores.

Recuperación por IDs y Doc111ActualizarRama se conservan; las pruebas previas de recarga/identidad/árbol pasan. Backend Framework/DDL: 2/2 PASS en la regresión combinada anterior al último ajuste de teclado, sin fuentes backend modificadas después. Sus casos internos no se suman otra vez.

Limpieza continúa deshabilitada, sin invocación. El responsable decidió expresamente operar sin limpieza el2026-11-12, conservar datos y no fijar cuota/plazo nuevo. SEG-03 no ejecutado. PDF/rechazo TIFF, integridad, esquema y efectos existentes se preservan. La UI reutiliza los mismos servicios; captura nivel explícito y consulta confirmaciones sin reenviar ConfirmarPlan. La corrección del árbol cuenta nodos lógicos tN y excluye sus enlaces de imagen tNi.
