# Diagnóstico — carga de documentos en Producción Documental

## Síntoma reportado

En el módulo **Producción Documental**, durante la carga de documentos, la interfaz presenta el mensaje:

> Referencia a objeto no establecida como instancia de un objeto.

Este documento registra una exploración estática del código. No constituye todavía una confirmación de la línea exacta mediante ejecución real, traza de servidor o reproducción E2E.

## Conclusión ejecutiva

El defecto pertenece a la misma familia arquitectónica de la corrección realizada en Radicación Simplificada mediante DOC-85: una cadena legacy depende de contexto mutable y consume estructuras de salida suponiendo que fueron materializadas correctamente.

La diferencia es que en Producción Documental no se observa, con la evidencia actual, una pérdida del radicado. El contexto involucrado está compuesto por:

- usuario de Gestión;
- nivel seleccionado;
- expediente seleccionado;
- permisos sobre el nivel;
- gabinete de producción;
- clasificación TRD y tipología documental;
- relaciones entre campos del expediente y campos del gabinete;
- ruta temporal del archivo.

La ruta actual resuelve estos datos en momentos diferentes y desde fuentes diferentes. No existe un contexto de carga tipado, autorizado e inmutable que viaje desde la validación inicial hasta el almacenamiento.

## Recorrido funcional actual

```text
Clic en Cargar archivo
  -> js/gestion/WebFormProducionDocumental.js
  -> WebServiceProducion.ServiceSolicitaCargarDocumentoExpediente
  -> ClassGaProducionDocumental.SolicitaCargarDocumentoExpediente
  -> creación del formulario dinámico de carga
  -> WebServiceProducion.Service_parameter_upload("PRODUCCION")
  -> WebServiceProducion.ServiceSolicitaListaTipologiasExpediente
  -> selección local de archivo y tipología
  -> generic_control/FileUploadHandler_.ashx
  -> ClassAlmacenamiento.UploadSaveFile, sobrecarga legacy de diez argumentos
  -> rama WF_TIPO_ADJUNTA = "PRODUCCION"
  -> PreAlmacenaDocumentoProduccion
  -> AlmacenamientoDocumentoProduccionDocumental
  -> ClassAlmacenamiento.Almacenamiento
  -> respuesta uploadFiles
  -> FileUploadHandler.js registra la fila en la interfaz
```

## Fronteras que pueden emitir el mensaje sin contexto

El texto exacto puede llegar a la interfaz sin el nombre de la operación desde dos fronteras principales:

1. `webservice/WebServiceProducion.asmx.vb`, en el `Catch` de `Service_parameter_upload`, asigna solamente `ex.Message` a `parameter_upload.error_result`.
2. `generic_control/FileUploadHandler_.ashx.vb`, en el `Catch` exterior de `ProcessRequest`, asigna solamente `ex.Message` a `uploadFiles.error_sistema`.

Esto impide distinguir visualmente entre dos momentos:

- **Falla antes de abrir el modal:** configuración de carga o preparación de tipologías.
- **Falla después de pulsar Guardar:** handler, prealmacenamiento, almacenamiento o proyección de la respuesta.

La implementación debe caracterizar primero ambos escenarios y no asumir que cualquier aparición del mensaje proviene de la misma línea.

## Riesgo principal: consumo de resultados parciales

`AlmacenamientoDocumentoProduccionDocumental` inicializa el arreglo `EsctructuraExpediente` con `Nothing`, llama a `SolicitaDatosEstructuraExpediente` y, si el texto retornado es `YES`, consume directamente `EsctructuraExpediente(0)`.

Aunque la implementación actual del resolver intenta retornar error cuando no encuentra filas, el consumidor no protege por sí mismo las invariantes siguientes:

- el arreglo no es `Nothing`;
- contiene al menos un elemento;
- el elemento corresponde al expediente solicitado;
- el gabinete y la clasificación fueron resueltos para el mismo contexto autorizado;
- la selección continúa vigente al guardar el archivo.

Este acoplamiento entre un retorno textual y parámetros `ByRef` es comparable al defecto de DOC-85: el consumidor confía en `YES` como sustituto de validar el objeto producido.

## Contexto mutable y resolución repetida

La autorización inicial usa `PG_SELECCION_TREVIEEW_PRODUCCION` desde sesión. El navegador recibe `IdExpediente` y posteriormente lo envía al handler. El almacenamiento vuelve a resolver, por separado:

- gabinete de producción del expediente;
- identificador del gabinete Docuarchi;
- relación de campos expediente-gabinete;
- campos de fecha configurados;
- estructura completa del expediente;
- clasificación documental;
- parámetros físicos de almacenamiento.

No existe una unidad transaccional o un contexto inmutable que garantice que todas esas resoluciones pertenecen a la misma selección autorizada.

## Hallazgos adicionales

### Pérdida de errores JavaScript

En `js/gestion/WebFormProducionDocumental.js` y `generic_control/FileUploadHandler.js` existen numerosos `Catch` que leen `ex.mensaje` en lugar de `ex.message`. En JavaScript estándar esto oculta la excepción original y produce mensajes terminados en `undefined`.

### Proyección incorrecta después de cargar

En `_RegistraArchivoInterfaz`, la rama `insert_row_producion_documental` concatena una propiedad inexistente llamada `id_imageinsert_row_documento_relacionado`. Esto puede construir una cadena de fila inválida aun cuando el almacenamiento haya finalizado correctamente.

La presencia de este defecto implica que deben separarse dos resultados observables:

1. persistencia del archivo;
2. actualización correcta de la lista en pantalla.

Un error visual posterior no autoriza a repetir ciegamente el almacenamiento, porque podría duplicar documentos.

## Relación con DOC-85

La corrección de Radicación Simplificada separó un contexto autoritativo y evitó redescubrir el radicado desde una fuente secundaria. Para Producción Documental es razonable aplicar el mismo principio, pero no reutilizar literalmente el contexto de Radicación ni modificar su comportamiento.

La revisión de historial indica que las funciones específicas de Producción Documental ya existían desde la carga inicial del proyecto. DOC-85 mantuvo deliberadamente la rama `PRODUCCION` dentro del recorrido legacy. Por ello, el síntoma no debe atribuirse automáticamente a DOC-85.

## Hipótesis ordenadas para caracterización

1. **Contexto de expediente incompleto o desactualizado:** el almacenamiento consume una estructura no materializada o incompatible con la selección original.
2. **Falla al preparar configuración o tipologías:** el modal no abre y `Service_parameter_upload` expone únicamente el mensaje interno.
3. **Falla profunda de almacenamiento:** una dependencia de `AlmacenamientoDocumentoProduccionDocumental` recibe información parcial y el error llega encapsulado por varias capas.
4. **Persistencia correcta con falla de proyección:** el documento existe, pero `_RegistraArchivoInterfaz` falla o construye una fila corrupta.
5. **Estado compartido entre cargas:** variables globales como `FilePerson`, `CDproduccion` o la ruta temporal de sesión conservan información de una operación previa.

## Evidencia necesaria para confirmar la causa exacta

Sin imprimir datos sensibles, una reproducción controlada debe registrar:

- si el modal alcanzó a abrir;
- endpoint que devolvió el error;
- código de etapa saneado;
- si el handler recibió un `IdExpediente` positivo;
- si la selección de sesión correspondía al mismo expediente y nivel;
- si el expediente, gabinete y tipología fueron encontrados;
- si la persistencia ocurrió antes del error;
- conteo anterior y posterior mediante consultas exclusivamente `SELECT`;
- resultado de un reintento controlado para verificar idempotencia o duplicación.

No se deben registrar credenciales, cookies, tokens, cadenas de conexión, rutas físicas sensibles ni contenido documental.

## Alcance recomendado

La futura corrección debe aislarse a la carga `PRODUCCION`. No debe cambiar la semántica de:

- Radicación Simplificada y `ADJUNTARADICACION`;
- Gestión de respuestas;
- Workflow seleccionado o por enlace;
- importación SII o ENLASE;
- versiones documentales;
- digitalización;
- carga de PQRS;
- adjuntos, documentos relacionados o restantes consumidores del handler compartido.

## Estado de la exploración

- Diagnóstico estático: completado.
- Línea exacta confirmada por traza o reproducción: pendiente.
- Cambios de aplicación: ninguno.
- E2E autenticada: no ejecutada.
- Consultas a bases de datos: no ejecutadas.

