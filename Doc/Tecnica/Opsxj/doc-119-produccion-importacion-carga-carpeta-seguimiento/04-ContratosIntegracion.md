# DOC-119 — PRODUCCION-IMPORTACION-CARGA-CARPETA-SEGUIMIENTO

- Cambio: `doc-119-produccion-importacion-carga-carpeta-seguimiento`.
- Clasificación: `cross_cutting`.
- Estado de esta entrega: refinamiento propuesto; sin implementación ni aceptación operativa.

## Contratos e integraciones

ASMX/ASHX/DTO actuales se mantienen. AbrirImportacion recibe nivelRef; contexto/ruta se reautorizan en servidor. Consulta contextual no crea búsqueda por nivel ni historial nuevo, y un lote conserva destino original.

ConsultarRecursosLote y ExportarPaginaLote reutilizan loteId/tipo/pagina/tamanoPagina y filtros raíz/estado/recepción ya admitidos por servicio. La ausencia de envío de filtros del cliente es hallazgo a reproducir antes de corregir. Confirmación sin acuse se consulta con planId; no autoriza reenviar ConfirmarPlan.

El runner REC actual puede tener efectos: revisar modos y autorizaciones antes de reutilizarlo. No se creará login, repositorio, coordinador ni esquema paralelo. SII y almacenamiento compartido permanecen fuera de alcance. Este documento describe propuesta y contrato inspeccionado, no una integración nueva certificada.
