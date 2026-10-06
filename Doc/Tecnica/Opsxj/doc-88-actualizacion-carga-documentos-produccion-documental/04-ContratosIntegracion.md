# DOC-88 — Contratos e integración

## Contratos preservados

- Endpoint ASMX, nombres de métodos, parámetros y tipos públicos: sin cambios.
- URL y método del handler: `POST ../generic_control/fileuploadhandler_.ashx`, sin cambios.
- Forma JSON de `UploadFilesResult`: sin campos agregados, retirados ni renombrados.
- Firma de `UploadSaveFile`, `PreAlmacenaDocumentoProduccion` y `Almacenamiento`: sin cambios.
- Persistencia existente: sin SQL nuevo y sin consulta de existencia.

## Semántica de PRODUCCION

- `PRODUCCION_CARGA_CONTEXTO_INVALIDO`: selección, usuario, expediente o estructura no válida.
- `PRODUCCION_CARGA_CONFIGURACION_INCOMPLETA`: configuración o gabinete no materializado.
- `PRODUCCION_CARGA_ARCHIVO_NO_DISPONIBLE`: ruta vacía o archivo temporal ausente.
- `PRODUCCION_CARGA_ALMACENAMIENTO_RECHAZADO`: excepción inesperada dentro de la frontera de almacenamiento de producción.
- `PRODUCCION_CARGA_CONFIRMACION_INCIERTA`: no se obtuvo una respuesta JSON confirmable; no hay reintento automático.
- `PRODUCCION_CARGA_PROYECCION_FALLIDA`: el resultado confirmado no contiene los datos mínimos de fila o falla su inserción.

Un valor distinto de `YES` retornado por el almacenamiento sigue siendo terminal y se devuelve sin volver a escribir.

## Aislamiento

El JavaScript conserva el recorrido histórico cuando `evento_adjunta != "PRODUCCION"`. El handler servidor compartido permanece intacto, lo que preserva ADJUNTARADICACION, GESTION_RESPUESTA, Workflow, SII, ENLASE, versiones, digitalización y PQRS.
