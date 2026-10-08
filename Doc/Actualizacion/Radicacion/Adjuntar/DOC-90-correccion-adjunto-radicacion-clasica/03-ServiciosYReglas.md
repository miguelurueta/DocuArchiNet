# DOC-90 — Servicios y reglas

- Ticket: DOC-90
- Cambio OpenSpec: doc-90-actualizar-adjuntar-documento-radicacion
- Clasificacion: cross_cutting

## Ruta clásica

`FileUploadHandler_` traduce `ADJUNTARADICACION_CLASICA` a `WF_TIPO_ADJUNTA = "ADJUNTARADICACION_CLASICA"`, conserva la ruta temporal en sesión e invoca `ClassAlmacenamiento.UploadSaveFile` con diez argumentos.

La rama clásica de esa sobrecarga aplica en orden:

1. Comprueba que `RA_MODULO_SELECCIONADO` identifica `RADICACION` / `RADICACION ENTRANTE`.
2. Convierte `RA_ID_PLANTILLA_RADICADO_SELECCIONADO` y `RA_ID_REGISTRO_RADICADO` desde sesión y rechaza valores no positivos.
3. Ejecuta `Class_ra_rad_estados_modulo_radicacion.SolicitaDatosEstructuraEstadoRadicado`.
4. Rechaza una resolución fallida, una plantilla distinta de la activa, radicado vacío, tarea no positiva o trámite no positivo.
5. Materializa `DG_LISTA_CHEQUEO` desde la tipología seleccionada.
6. Llama una sola vez `PreAlmacenaDocumentosRadicacion` con tarea, trámite, radicado y plantilla de `stru_registro_estado`/sesión ya validados.
7. Propaga `id_tarea_workflow` y retorna `YES` solo después del prealmacenamiento exitoso.

El navegador no envía el ID que autoriza la carga clásica. El evento discrimina el contrato; la identidad documental sigue proviniendo del estado de servidor.

## Preparación legacy

`PreAlmacenaDocumentosRadicacion` recibe opcionalmente `ConsecutivoRadicadoEstado` e `IdPlantillaRadicadoEstado`. Cuando la rama clásica los suministra, la tarea interna ejecuta `ConstruirDatosCamposIndiceGabineteConRadicado` antes del almacenamiento, con el radicado y la plantilla autoritativos. No intenta primero `SolicitaDatosCamposIndiceGabinete`, porque ese recorrido legacy redescubre el radicado desde `DAT_ADIC_TAR` y puede encontrarlo vacío. Los demás llamadores usan los valores predeterminados y conservan su recorrido histórico.

La E2E autenticada detectó esta diferencia temporal: asignar el radicado después de construir los índices era demasiado tarde y producía `El radicado autoritativo es obligatorio para construir los índices del gabinete.`. La corrección mueve el uso del contexto autoritativo al punto exacto de construcción; no agrega autoridad proveniente del navegador.

## Frontera DOC-85

La sobrecarga de doce argumentos no cambió: exige `WF_TIPO_ADJUNTA = "ADJUNTARADICACION"`, ID positivo, conexión autorizada y resolución por usuario; delega en `ServicioAdjuntoRadicacion.Adjuntar` y `PreAlmacenaDocumentosRadicacionConContexto`. No consulta `RA_ID_REGISTRO_RADICADO`, no redescubre desde `DAT_ADIC_TAR` y no acepta el fallback clásico.

## Errores previos al almacenamiento

- Registro de sesión inexistente o cero.
- Módulo distinto de Radicación Entrante.
- Plantilla de sesión inexistente o distinta de la plantilla del registro.
- Registro no resoluble.
- Consecutivo de radicado vacío.
- Tarea Workflow no positiva.
- Tipo de trámite no positivo.
- Tipología obligatoria ausente, validada por la preparación existente.

Cada error termina la rama antes de `PreAlmacenaDocumentosRadicacion` o antes de `AlmacenaDocumentosRadicacion`, según corresponda.
