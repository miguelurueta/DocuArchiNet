# DOC-112 — Entrega documental de contratos

- Ticket: DOC-112
- Cambio OpenSpec: doc-112-produccion-importacion-carga-carpeta-contrato
- Clasificacion: cross_cutting (Transversal)

## Contratos e integraciones

Se corrigieron los nombres conceptuales para coincidir con doce operaciones públicas de WebServiceImportacionCarpetas.asmx y el handler ImportacionCarpetasUpload.ashx, incluidos ConsultarConfirmacionLote, ConsultarRecursosLote, ExportarPaginaLote y CerrarReservaSinInicio. Las fichas identifican DTO, campos, propietario, datos devueltos y efectos permitidos. No existe ListarLotes público ni CancelarLote ni un endpoint por adaptador.

El contrato conserva POST JSON con solicitud y sobre ASMX d frente a multipart con tres metadatos y un archivo. Los guards y autoridad de importar están separados de administración. El sobre público y EstadoHttp se describen según código; la revisión no conserva valores de token, cookies, identidad privada o conexiones. Reconciliar verifica evidencia de un intento sin invocar de nuevo el escritor.

No cambia compatibilidad de consumidores, aplicación individual o SII. Las diferencias físicas se explican con SQL 02 y migración 03/activación 04 ya existentes. Fuente común: [contratos vigentes](../../../Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/02-contratos-operacion.md); evidencia del alcance: [acta DOC-112](../../../Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc112-contratos-revision.md).
