# DOC-88 — Corrección de carga en Producción Documental

## Resultado

La caracterización estática y la reproducción E2E confirmaron seis puntos inseguros del recorrido:

1. `SolicitaCargarDocumentoExpediente` dividía una selección de sesión sin comprobar `Nothing`, cantidad de segmentos ni identificadores.
2. `Service_parameter_upload("PRODUCCION")` consumía una estructura de configuración por defecto aunque el repositorio retornara `YES`.
3. `SolicitaListaTipologiasExpediente` consumía `EstruUnidadConservacion(0)` sin comprobar que el arreglo estuviera materializado.
4. `AlmacenamientoDocumentoProduccionDocumental` consumía `EsctructuraExpediente(0)` y `.Count` sin validar arreglo o lista.
5. `_RegistraArchivoInterfaz` usaba la propiedad inexistente `id_imageinsert_row_documento_relacionado` después de que el servidor ya había almacenado el documento.
6. `FileUploadHandler_.ashx.vb` aplicaba `.Trim()` al parámetro opcional `radicado_radicacion`; `PRODUCCION` no lo envía y reproducía la referencia nula antes de `UploadSaveFile`.

La E2E real aisló el incidente reportado en el sexto punto. Los otros hallazgos permanecen cubiertos como defensas locales: podían producir fallos durante la preparación o almacenamiento, y la proyección incorrecta podía ocultar un almacenamiento exitoso.

## Corrección quirúrgica

- Se validan selección, usuario, expediente, nivel, configuración, tipología, gabinete, archivo, arreglos y listas en las clases exclusivas de Producción Documental.
- El handler acepta `radicado_radicacion` ausente sin desreferenciarlo y sanea por etapa únicamente los errores de `PRODUCCION`.
- La fila visual de `PRODUCCION` usa `id_image`, que ya forma parte de `UploadFilesResult`.
- Un error devuelto por `Almacenamiento` termina el recorrido: no existe consulta posterior ni reintento.
- Una respuesta vacía o una falla de proyección retorna un código controlado y no vuelve a enviar el archivo.
- El bloqueo de doble activación existe únicamente mientras una solicitud `PRODUCCION` está activa.

## Compatibilidad

`generic_control/FileUploadHandler_.ashx.vb` fue modificado únicamente en la lectura segura del parámetro opcional y en el saneamiento condicionado por `evento_adjunta = "PRODUCCION"`. La huella actual está protegida por las suites DOC-85/DOC-88 y los demás eventos conservan su `Catch` heredado. En `FileUploadHandler.js`, todo comportamiento nuevo exige `evento_adjunta == "PRODUCCION"`.

No se agregó SQL, tabla, columna, token, feature gate ni consulta de existencia. Tampoco se modificaron Workflow, SII, ENLASE, versiones, PQRS o Radicación Simplificada.

## Reversa

Revertir exclusivamente los cambios DOC-88 en:

- `Gestion/ClassGaProducionDocumental.vb`
- `webservice/WebServiceProducion.asmx.vb`
- `workflow/ClassAlmacenamiento.vb`
- `generic_control/FileUploadHandler.js`
- `generic_control/FileUploadHandler_.ashx.vb`

No existe migración de base de datos ni dato auxiliar que deba revertirse.
