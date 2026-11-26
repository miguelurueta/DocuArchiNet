# Exploración de carga directa para radicación interna

Es viable que un usuario inicie la radicación interna desde un acceso propio y suba sus documentos sin recorrer la pantalla de Producción documental. La página actual necesita un registro documental existente para preparar el formulario y registrar sus relaciones. La nueva entrada requiere preparar archivos antes de confirmar y entregar al motor un contexto explícito; añadir un selector de archivos a la página actual no resuelve esas dependencias.

La interfaz simplificada del commit `23464d6b` está aprobada por el usuario. La [maqueta](maqueta.html) incorpora ahora la selección local de principal y anexos solicitada para revisar esta nueva procedencia. La recepción/persistencia en servidor y las reglas de expediente continúan en refinamiento. Se mantiene D-RI-01: el usuario interno o tercero elegido es el remitente y el usuario autenticado sigue siendo el ejecutor.

**D-RI-04 aprobada, 2026-11-26:** reutilizar `LoadFilePERSON` / `IniLoadPerson` de `generic_control/FileUploadHandler.js`, adaptando su presentación a la interfaz aprobada y agregando un modo de recepción temporal por operación antes de radicar. La incorporación definitiva ocurre al confirmar. El control existente necesita una adaptación acotada del envío/resultados para conectar la nueva recepción; sus eventos actuales de adjunto a radicado creado conservan sus contratos. No se construye otro cargador completo. Expediente, permisos, límites y caducidad siguen en refinamiento.

| Referencia | Valor |
|---|---|
| Fecha | 2026-11-25 |
| Estado | Selección local en maqueta; recepción y persistencia en exploración |
| Base revisada | `ddfe0785`, que incluye la interfaz aprobada `23464d6b` |
| Pregunta funcional principal | ¿Expediente obligatorio antes de confirmar o clasificación posterior? Pendiente de respuesta. |
| Diagnóstico y decisiones previas | [Diagnóstico de radicación interna](diagnostico.md) |
| Interfaz aprobada | [Maqueta simplificada](maqueta.html) |

## Dependencias verificadas

| Punto | Evidencia del código | Implicación |
|---|---|---|
| Apertura desde Producción | [WebFormProducionDocumental.aspx.vb](../../../Gestion/WebFormProducionDocumental.aspx.vb), `ButtonRadicar_Click`, líneas 1122–1195. | Valida la selección y prepara `RA_MODULO_SELECCIONADO` con plantilla y principal; los anexos son IDs de registros de Producción. |
| Preparación del formulario | [ClassGaProducionDocumental.vb](../../../Gestion/ClassGaProducionDocumental.vb), `Genera_Interface_Radicacion_Entrante`, línea 4440. | Consulta nombre, fecha, folios, imagen y gabinete por `id_producion_documental`; también resuelve anexos de sesión. Una referencia temporal de carga todavía no cumple ese contrato. |
| Plantilla y autoridad | [ClassGaProducionDocumental.vb](../../../Gestion/ClassGaProducionDocumental.vb), `Solicita_radicar_documento_produccion`, línea 4072. | Resuelve la plantilla interna predeterminada y comprueba permisos de radicación y de nivel a partir del documento/expediente. La entrada independiente necesita resolverlos sin exigir una fila seleccionada en Producción. |
| Confirmación | [WebFormRadicacionEntranteInterna.aspx.vb](../../../radicador/WebFormRadicacionEntranteInterna.aspx.vb), `Buttonradicar_entrante_Click`, línea 244. | Lee documento/plantilla de sesión y expediente desde `PG_SELECCION_ID_EXPEIDENTE`; la carga independiente no debe heredar esas selecciones de otra pestaña. |
| Relación obligatoria con el principal | [ClassRadicador.vb](../../../radicador/ClassRadicador.vb), `Registra_Radicacion_entrante_interna`, líneas 2947–2980. | Inserta `RA_PRO_RELACION_RAD_INTERNO` con el ID de Producción del principal y los anexos antes del commit. Pasar ID cero no implementa un origen nuevo. |
| Incorporación posterior | [ClassRadicador.vb](../../../radicador/ClassRadicador.vb), líneas 2993–3134. | Vuelve a consultar principal/anexos de Producción y copia o actualiza documentos según los gabinetes. La persistencia de archivos forma parte del recorrido, no solo del formulario. |
| Cargador compartido | [FileUploadHandler.js](../../../generic_control/FileUploadHandler.js), envío HTTP alrededor de línea 965; [FileUploadHandler_.ashx.vb](../../../generic_control/FileUploadHandler_.ashx.vb), ramas desde línea 340. | Existen transporte, progreso, catálogos y almacenamiento reutilizables. El handler distingue eventos que tienen contratos diferentes. |
| Adjunto clásico | [ClassAlmacenamiento.vb](../../../workflow/ClassAlmacenamiento.vb), rama `ADJUNTARADICACION_CLASICA`, línea 6217. | Exige plantilla, registro de estado, radicado, tarea y trámite existentes. Sirve para adjuntar a una radicación creada; no prepara un borrador antes de radicar. |
| Adjunto simplificado | [ClassAlmacenamiento.vb](../../../workflow/ClassAlmacenamiento.vb), sobrecarga de doce argumentos, línea 5797; [ServicioAdjuntoRadicacion.vb](../../../Services/RadicacionSimplificada/Adjuntos/ServicioAdjuntoRadicacion.vb). | Exige estado positivo y contexto autorizado de Radicación Simplificada. No se debe reutilizar debilitando esas condiciones ni mezclando su evento con la nueva entrada. |
| Almacenamiento en Producción | [ClassAlmacenamiento.vb](../../../workflow/ClassAlmacenamiento.vb), `PreAlmacenaDocumentoProduccion`, línea 8429, y `AlmacenamientoDocumentoProduccionDocumental`, línea 8893. | Requiere expediente y tipología válidos, resuelve gabinete e índices y almacena de forma persistente. Cargar por esta vía no equivale a un temporal descartable. |
| Permiso de carga actual | [ClassGaProducionDocumental.vb](../../../Gestion/ClassGaProducionDocumental.vb), `SolicitaCargarDocumentoExpediente`, línea 1045. | Exige expediente abierto y autoridad de carga; utiliza la selección del árbol de sesión. La nueva entrada necesita verificar los mismos recursos con contexto propio. |

El registro contempla ramas con `id_expeidente = 0` alrededor de las líneas 2256 y 2269. Eso no acredita un recorrido completo sin expediente: la preparación, las relaciones con Producción y la carga documental mantienen otras dependencias. La posibilidad de clasificar después requiere un diseño específico.

## Alternativas de integración

| Alternativa | Recorrido | Reutilización y coste técnico |
|---|---|---|
| A. Entrada independiente conservando registros documentales | Subir a un borrador; seleccionar un expediente autorizado; al confirmar, incorporar principal/anexos al almacenamiento documental y obtener IDs válidos; continuar la radicación. | Conserva más contratos del registro actual. Requiere desacoplar la preparación visual, gestionar el borrador y registrar la recuperación si la incorporación o radicación falla. El usuario no visita Producción aunque los documentos sí tengan registros internos allí. |
| B. Entrada independiente también en persistencia | Preparar principal/anexos sin IDs previos de Producción; registrar e incorporar documentos mediante un contrato documental común. | Necesita adaptar preparación, relaciones, almacenamiento y recuperación. Es la opción a estudiar si debe admitirse radicación sin expediente o sin registros de Producción. No consiste en omitir las validaciones actuales. |

Si se conserva el expediente antes de confirmar, A es la candidata con menor ruptura del registro existente. La decisión no queda aprobada por esta exploración. Tampoco debe implementarse cargando documentos definitivos apenas se abre el selector: al cancelar, esos documentos ya existirían.

En A, el formulario debe poder mostrar metadatos de un borrador sin llamar al generador con un ID inexistente. La incorporación definitiva obtiene los IDs que necesita el registro actual. Si esta adaptación no puede aislarse con claridad, conviene extraer un contrato común de documentos y preparación antes de conectar la entrada nueva.

## Recorrido propuesto para el usuario

Desde Producción, el formulario sigue abriendo con principal, anexos y contexto precargados. Desde el nuevo acceso, el lugar de esa misma línea documental permite **Agregar documento principal** y **Agregar anexos**. Se conserva el formulario compacto aprobado: remitente, datos y destino. No hace falta añadir un selector técnico de procedencia.

La carga muestra nombre, tamaño y estado por archivo. El principal se elige explícitamente; varios archivos no convierten arbitrariamente al primero en principal. Se puede retirar o sustituir un archivo antes de confirmar. La confirmación se habilita solo cuando los archivos requeridos están recibidos y verificados y los datos/documentos pertenecen al mismo intento.

La elección o clasificación del expediente se integra según la respuesta pendiente. Evitar presentar un expediente ficticio o reutilizar el último seleccionado de otra pantalla. La nueva carga no convierte al tercero remitente en propietario del borrador ni en actor de auditoría.

### Recorrido disponible en la maqueta

El HTML abre en **Subir documentos**. **Elegir archivo** permite seleccionar un principal; **Agregar anexos** admite varios archivos. Se muestran nombre, tamaño y estado local, con acciones para cambiar o quitar archivos. Los dos espacios también admiten arrastre. Fecha, folios y asunto empiezan vacíos; la cabecera no presenta un expediente de Producción para esta entrada.

**Desde Producción** recupera los documentos y datos precargados del ejemplo. Cada origen conserva su propia selección dentro de la pestaña y el resumen indica cuál se utiliza. No se mezclan archivos locales y documentos existentes. La maqueta comprueba límites ilustrativos de extensión/tamaño/cantidad y detecta repeticiones exactas por nombre y huella; no valida contenido documental ni envía bytes al sistema.

Los límites del ejemplo son 20 MB por archivo, 50 MB en total y 10 archivos, con extensiones PDF, DOC/DOCX, JPG/JPEG, PNG y TIF/TIFF. Esos valores no aprueban Q-CD-03 ni sustituyen la política real de recepción. Reiniciar o recargar el HTML descarta los archivos locales; no representa una limpieza de documentos del ambiente.

## Contratos y controles necesarios

1. **Identidad del borrador:** referencia de operación creada en servidor, asociada a ejecutor, empresa/ámbito y configuración autorizada. Documentos existentes y archivos nuevos deben declarar su origen; la autoridad no procede de un ID de usuario o una ruta enviados por el navegador.
2. **Recepción temporal:** cada archivo tiene una referencia interna única, nombre visible, rol, tamaño, huella y estado de recepción. El temporal se aísla por operación y archivo. El handler compartido actual deriva su ruta temporal del usuario y nombre del archivo; ese esquema no demuestra aislamiento entre borradores simultáneos.
3. **Validación de archivos:** aplicar en servidor tipos permitidos, tamaño por archivo y total, cantidad, integridad y reglas de conversión/almacenamiento. Las extensiones y tamaños que filtra JavaScript no acreditan recepción válida. Los valores finales se refinan con las capacidades reales del gabinete y digitalización.
4. **Expediente y tipología:** revalidar estado abierto, nivel, tipologías y permisos para el ejecutor cuando corresponda. Preparar el contexto sin modificar las selecciones globales de otra operación.
5. **Confirmación de una versión concreta:** verificar plantilla, remitente, responsable y conjunto de archivos; impedir confirmar durante una carga pendiente o después de modificar una selección ya revisada. Un archivo validado para otro borrador no se puede reutilizar por su nombre.
6. **Persistencia y reintento:** relacionar el intento con los documentos incorporados y el radicado obtenido. Si se incorporaron documentos y falla la radicación, recuperar el mismo intento antes de volver a crear archivos o consecutivos. Mantener los resultados parciales identificados en H09.
7. **Cancelación y caducidad:** retirar temporales propios no utilizados; definir la conservación del borrador. Una limpieza de temporales no puede borrar documentos incorporados o relacionados a un radicado. Cerrar después de registrar conserva el resultado.
8. **Compatibilidad:** preservar Producción, Radicación clásica, Simplificada y los demás consumidores del cargador. La nueva preparación previa al registro debe tener un contexto propio; no convertirla en un adjunto a un radicado inexistente.

El repositorio contiene un patrón de recepción por operación en [ServicioRecepcionImportacion.vb](../../../Services/ProduccionDocumental/ImportacionCarpetas/ServicioRecepcionImportacion.vb) y [ImportacionCarpetasUpload.ashx.vb](../../../generic_control/ImportacionCarpetasUpload.ashx.vb): contexto autorizado, referencias de archivo, recepción verificada y tratamiento de reenvíos. Es una referencia de diseño; reutilizar directamente ese flujo de importación de carpetas exigiría revisar sus contratos y no está propuesto como atajo.

## Preguntas para el refinamiento

| Referencia | Pregunta | Efecto |
|---|---|---|
| Q-CD-01 | ¿Expediente antes de confirmar o clasificación después? | Determina el modelo de incorporación documental y la alternativa de integración. Pregunta presentada al usuario; pendiente. |
| Q-CD-02 | ¿Desde qué acceso se abre y qué usuarios pueden usar carga directa? | Define menú, plantilla inicial y autoridad de carga/radicación sin entrar en Producción. |
| Q-CD-03 | ¿Qué tipos, cantidad y tamaños se admiten y cómo se obtiene el número de folios? | Define recepción, validación, conversión y datos que el usuario debe completar. |
| Q-CD-04 | ¿Se mezclan en un mismo intento documentos de Producción y archivos nuevos? | Define si se admite origen mixto; la entrada independiente no lo aprueba automáticamente. |
| Q-CD-05 | ¿Cuánto dura un borrador y puede reanudarse después de salir? | Define caducidad, consulta, recuperación y limpieza de temporales. |
| Q-CD-06 | ¿Cuándo y con qué tipología se incorporan los archivos al almacenamiento definitivo? | Define qué queda persistido ante cancelación o fallo parcial y quién puede consultarlo. |

Estas preguntas se suman a Q-RI-01 a Q-RI-05 para la representación de terceros. La aprobación de la interfaz no aprueba automáticamente reglas nuevas de clasificación, permisos o persistencia.

## Aceptación de la futura capacidad

- Iniciar una radicación directa con sesión nueva, sin haber visitado Producción; resolver plantilla y contexto autorizado sin arrastrar una selección anterior.
- Subir un principal y varios anexos; sustituir y retirar archivos; confirmar los roles y la descripción; impedir confirmar mientras una carga esté pendiente o rechazada.
- Probar nombres repetidos en el mismo borrador, en dos pestañas y en dos usuarios; no sobrescribir ni mezclar contenido. Verificar nombres y límites también en servidor.
- Cambiar expediente, tipología, trámite, remitente o destino después de subir; invalidar o revalidar exactamente las dependencias afectadas.
- Confirmar como usuario interno y como tercero remitente, conservando al ejecutor autenticado y al responsable real.
- Recuperar ante fallo de recepción, incorporación, commit, Workflow, correo o respuesta perdida sin duplicar el mismo intento.
- Cancelar antes de confirmar, caducar temporales y cerrar después de registrar; conservar cualquier documento ya incorporado según la regla aprobada.
- Repetir el recorrido existente desde Producción y los adjuntos clásico/Simplificado sin alterar sus contratos.

## Verificación de la exploración

Se revisaron código y contratos de los componentes citados. Pasaron **11 comprobaciones existentes** con `node --test tests/doc90-radicacion-classic-attachment.test.cjs tests/radicacion-simple-attachment-upload-signatures.test.cjs`. Verifican la separación actual de adjuntos clásicos/Simplificados y sus consumidores; no prueban una carga directa nueva.

La selección local se comprobó con nueve grupos de casos en Chromium mediante `verificar-carga-directa-maqueta.cjs`, fuera de Git en `.tmp/diagnostico-radicacion-interna-20261125/`. Se verificaron selector y arrastre, roles, retirada/sustitución, nombres coincidentes, límites ilustrativos, separación de orígenes, datos pendientes, descarte de lecturas tardías y resumen. No se detectaron errores JavaScript ni peticiones HTTP externas.

La modificación corresponde a la maqueta HTML. No se cambió código de aplicación ni se ejecutó una carga al servidor, radicación, conexión autenticada o E2E de ambiente. La implementación requiere cerrar Q-CD-01 y el refinamiento de los contratos afectados.
