# DOC-119 — PRODUCCION-IMPORTACION-CARGA-CARPETA-SEGUIMIENTO

- Cambio: `doc-119-produccion-importacion-carga-carpeta-seguimiento`.
- Clasificación: `cross_cutting`.
- Estado de esta entrega: refinamiento propuesto; sin implementación ni aceptación operativa.

## Servicios y reglas

Reutilizar ServicioConsultaImportacion y MySqlImportacionCarpetasRepository.Consulta: ConsultarLote/ConsultarRecursosLote/ExportarPaginaLote/ConsultarConfirmacionLote reautorizan propietario/ámbito/nivel/recursos. CSV es de una página; snapshot de lectura por página, sin reconciliación ni avance.

Separar recepción y efecto: RECIBIDO no prueba almacenamiento. Recuperar por IDs; metadatos de reselección no sustituyen bytes/hash/autoridad servidor. Doc111ActualizarRama agrega relaciones confirmadas por ID sin selección alterada.

Limpieza predeterminada deshabilitada. SEG-03 exige resolución explícita; no crear endpoint/job/plazo/configuración ni borrar temporales. Conservar PDF/rechazo TIFF, integridad y efectos existentes. No hay cambios de servicio ni autorización real en esta entrega.
