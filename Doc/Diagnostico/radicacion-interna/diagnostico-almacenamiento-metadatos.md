# Diagnóstico de almacenamiento y metadatos de radicación interna

La función central es `ClassAlmacenamiento.Almacenamiento`, en la línea 2219 de [ClassAlmacenamiento.vb](../../../workflow/ClassAlmacenamiento.vb). **Queda intocable por instrucción del usuario, D-RI-07.** El nuevo control prepara la carga; el almacenamiento definitivo conserva sus funciones, firmas, configuración y efectos existentes. La adaptación debe estar fuera del motor y usar sus contratos actuales.

Fecha: 2026-11-26. Base revisada: `95d7c8d377a960e551b62757c21c50d441e205d5`. Revisión de código, metadatos y llamadas; sin ejecutar almacenamiento, consultas de base, cargas reales, autenticación ni correo. No se modificó ninguna función de aplicación. Las verificaciones ejecutadas cubren la protección de fuentes, no la corrección funcional del motor.

## Qué queda congelado

La [baseline](almacenamiento-intocable-baseline.json) protege **el archivo completo `workflow/ClassAlmacenamiento.vb`**, además de registrar 24 implementaciones/sobrecargas de funciones de almacenamiento y metadatos en 10 archivos. En los demás archivos se congelan las funciones enumeradas, permitiendo cambios externos de UI en el mismo archivo sin tocar esas funciones.

| Archivo o familia | Protección y responsabilidad |
|---|---|
| `workflow/ClassAlmacenamiento.vb`, completo | Motor `Almacenamiento`, preparación de Producción, recepción legacy, variantes de almacenamiento, copia física y XML. Ningún cambio de cuerpo, firma, sobrecarga o helper de ese archivo dentro de esta implementación. |
| `Gestion/ClassGaProducionDocumental.vb`, cuatro funciones | `Almacena_documento_radicado_externo`, `Retorna_parametros_almacenamiento_documento_radicado_interno`, `Actualiza_documento_produccio_radicado`, `Solicita_datos_caracterizacion_archivo_produccion`. Se mantienen y se llaman sin reescribirlas. |
| `Docuarchi/Class_DETALLE_GABIENETE.vb` | Campos obligatorios, valores de índice y dos sobrecargas de estructura de gabinete: conservar orden, tipos, visibilidad y asignación. |
| `Docuarchi/ClassDaGabinete.vb` | Resolución de la matriz de documentos almacenados desde gabinete/imagen. |
| Relaciones expediente/gabinete y plantilla/gabinete | Lectores de asignación de índices y configuración, en `Gestion/Class_Ra_gabexp_relacion_index_gabinete_expediente.vb` y `Gestion/Class_ra_relacion_plantilla_gabinete.vb`. |
| `radicador/Class_plantillas_radicacion.vb` | `AsignaDatosCamposPlantillaRadicadoGabinete`: leer los valores del radicado y aplicar dimensiones configuradas. |
| `Gestion/ClassGaExpediente.vb` | Estructura del expediente y actualización del XML de índice. |
| `Docuarchi/Class_SYSTEM1RUT.vb` | Resolución de la ruta de almacenamiento. |
| `itextshare/Class_ItexShare.vb` | Contadores de páginas unificadas/PDF. |

No se autoriza saltar esta protección cambiando configuración de almacenamiento, helpers indirectos o DLL para reprogramar el motor; tampoco clonar la función o insertar directamente en inventario/gabinete/índices ni copiar archivos a su ubicación definitiva desde el nuevo control. El manifiesto recoge las funciones verificadas, no sustituye la revisión de dependencias indirectas.

## Recorrido real desde Producción y radicación interna

1. `Genera_Interface_Radicacion_Entrante`, en `ClassGaProducionDocumental.vb`, consulta la caracterización del principal mediante su ID de Producción: nombre, tipo documental, expediente, imagen, gabinete, fecha y folios. Los anexos son otras referencias reales; no archivos del navegador.
2. `Registra_Radicacion_entrante_interna`, en [ClassRadicador.vb](../../../radicador/ClassRadicador.vb), registra datos y relaciones `RA_PRO_RELACION_RAD_INTERNO` para principal/anexos, y confirma la transacción alrededor de la línea 2980.
3. Después del commit, compara gabinete de origen y destino. Si son distintos, `Almacena_documento_radicado_externo` prepara/copia el documento al gabinete destino. Si coinciden, el principal conserva su imagen y se vincula mediante `Actualiza_documento_produccio_radicado`.
4. Para el principal se genera además el archivo de detalle/rótulo del radicado. Para los anexos no se solicita ese rótulo. Estas salidas no deben confundirse con el principal original.
5. Continúan asignación Workflow, estados y notificación. Un fallo de archivo/metadatos en esta etapa puede ocurrir cuando el radicado ya está confirmado.

`Almacena_documento_radicado_externo`, línea 5514, lee el documento fuente, valida inventario/TRD/unidad del gabinete destino, obtiene expediente y prepara parámetros con `Retorna_parametros_almacenamiento_documento_radicado_interno`, línea 5734. Luego llama al motor existente. Sus flags distinguen almacenar copia y generar rótulo; mantenerlos según la rama vigente.

**Identificadores:** fuente de Producción, imagen de origen, registro de copia y rótulo son referencias distintas. El `ByRef id_imagen_almacenada` se usa en la llamada del documento y vuelve a usarse al almacenar el rótulo: al terminar puede identificar este último. `id_registro_copia` se asigna tras la copia documental; no se debe inferir que ambos retornos corresponden al mismo rol. En gabinete idéntico se consulta de nuevo la imagen original antes de actualizar su vínculo. El adaptador debe registrar la procedencia y el rol de cada referencia, sin alterar esa función.

## Recorrido de incorporación desde una carga nueva

El contrato existente con expediente previo es:

`PreAlmacenaDocumentoProduccion` (8429) → `AlmacenamientoDocumentoProduccionDocumental` (8893) → `Almacenamiento` (2219).

- `PreAlmacenaDocumentoProduccion` exige expediente/tipología positivos y ruta/descripcion no vacías. Resuelve gabinete del expediente, índices heredados y campos de fecha configurados.
- `AlmacenamientoDocumentoProduccionDocumental` valida archivo y expediente, resuelve clase documental, prepara matriz de índices, descompone Multi-TIFF cuando corresponde y llama al motor. Retorna imagen, registro, gabinete, tipo, fecha, nombre y estado de firma en la estructura existente.
- Esa función puede eliminar los archivos de entrada y `WF_RUTA_TEMPO_ADJUNTA` después de almacenar; no se le entrega una ruta compartida, de otro borrador o la única copia cuya conservación todavía no se resolvió.

La entrada nueva puede preparar un temporal propio y un adaptador externo para satisfacer ese contrato. No necesita que el usuario visite la pantalla de Producción, aunque los documentos sí reciban referencias internas de Producción. Esto no convierte automáticamente en válida una clasificación posterior sin expediente.

**Límite G01:** no se ha acreditado una ruta completa sin expediente usando estos preparadores intactos; el primero rechaza IDs no positivos. El motor general tiene parámetros opcionales, pero eso no demuestra que todo el recorrido interno, relaciones e índices admita omitir expediente. La variante B solo puede habilitarse si se demuestra compatibilidad con una ruta vigente, sin modificar funciones/configuración protegidas ni fabricar IDs. Si requiere tocar el motor, queda fuera del alcance actual; no se resuelve por excepción silenciosa ni eligiendo A sin acuerdo del usuario.

## Procedencia de los metadatos

| Dato o conjunto | Fuente y tratamiento observado | Obligación para la implementación |
|---|---|---|
| Nombre, tipología y clasificación del documento existente | `registro_producion_documental`; `Solicita_datos_caracterizacion_archivo_produccion`, línea 3075, obtiene descripción, ID de tipo, expediente, gabinete e imagen. | Resolver por referencia autorizada; no sustituirlos por un nombre de archivo o valores ficticios de la maqueta. |
| Usuario de almacenamiento y empresa | Argumentos del motor y contexto `GA_IDUSUARIOGESTION`/`GA_IDEMPRESA`; `DA_Login_Usuario` identifica al usuario de plataforma. | Mantener actor/ámbito autenticados. El tercero remitente del radicado no reemplaza esos IDs ni al responsable Workflow. |
| Fecha y folios del formulario | Campos dinámicos del radicado, inicialmente apoyados en la caracterización del principal. | Conservar la fuente y el significado; no derivarlos de `lastModified`, tamaño o número de anexos. |
| `FECHA_DOCUMENTO` del inventario creado por el motor | En el INSERT del motor, se usa `date1al`, inicializado con `Date.Today`. `FECHA_ELABORACION` usa el argumento correspondiente. | No prometer que editar la fecha del formulario modifica ambos campos del inventario. Caracterizar diferencias; no corregir el motor en este cambio. |
| Fechas configuradas en el gabinete | Prealmacenamiento usa `FechaCarga` para campos DATE relacionados; el preparador de Producción también establece fecha de elaboración con la fecha actual. El mapeo desde radicado formatea DATE/DATETIME con funciones vigentes. | Separar fecha documental, carga, elaboración e incorporación; conservar formato y precedencia existentes. |
| Folios/páginas efectivamente almacenadas | `Almacenamiento` consulta páginas del primer documento mediante `Retorna_numero_paginas_documentos_unificados`; si no hay conteo utiliza `Numero_Pag`. Multi-TIFF puede estar descompuesto en varias entradas. | No equiparar un documento lógico a una sola página ni al conteo de filas. Comparar PDF, Multi-TIFF y formatos admitidos con el comportamiento real. |
| Índices de gabinete | `DETALLE_GABIENETE`, campos visibles ordenados por `IDENTI`. El motor comprueba longitud de matriz; los valores son posicionales. | Mantener orden, nombre, tipo, visibilidad y dimensión; no construir el array según orden de propiedades JavaScript. |
| Campos de plantilla hacia gabinete | Relación plantilla/gabinete y `AsignaDatosCamposPlantillaRadicadoGabinete`, línea 209; consulta el radicado real y limita valores a `dimension_campo_ruta`. | El mapeo del radicado requiere que exista el registro. No ejecutarlo sobre un borrador sin consecutivo ni duplicar la regla de truncamiento. |
| Expediente, área, serie/subserie, clase y unidad | Estructura del expediente y reglas TRD/gabinete. La matriz combina índices, mapeos y campos de gestión con precedencias del código. | Usar lectores vigentes; no generar clasificación ni cambios de configuración para hacer encajar la UI. |
| Inventario y relaciones | Motor inserta `registro_producion_documental` y enlaza el registro del gabinete con `ID_INVENTARIO_DOCUMENTAL`; el radicador mantiene `RA_PRO_RELACION_RAD_INTERNO`. | No confundir inventario, imagen y radicado; conservar todos los vínculos requeridos y consultar su evidencia después de una operación incierta. |
| Índice electrónico y XML | Cuando corresponde, se actualizan orden, páginas inicial/final, `ra_cert_indice_expediente` y XML del expediente. Se genera además XML/índice junto al archivo almacenado. | Mantener ambos niveles de metadatos y sus relaciones; un archivo físico copiado no demuestra índice completo. |
| Tamaño/formato, nombres físicos y huellas | El motor deriva datos del archivo, asigna nombres `DIG...` y sus extensiones; el índice legacy genera su huella a partir de una referencia de registro. | No suponer que esa huella es SHA-256 del contenido. La huella del temporal nuevo verifica su recepción, no reemplaza las reglas legacy ni la evidencia de los derivados. |

El código de `ClassRadicador` construye cerca de 2896 un texto de INSERT de Producción y lo sustituye por el INSERT de `ra_registro_general_radicacion` antes de ejecutarlo. No se atribuye persistencia de metadatos a una sentencia únicamente por encontrarla en una cadena; el inventario efectivo se revisó en la llamada al motor.

## Qué hace el motor además de copiar archivos

`Almacenamiento` resuelve ruta física y configuración del gabinete; valida matriz de índices y condiciones de inventario/TRD/unidad; reserva identificadores, disco y carpeta en `system1`; construye registros de gabinete/inventario; actualiza contadores del expediente/unidad y disco; prepara el índice electrónico y auditoría aplicable; genera XML y copia documentos con `Generando_Archivo_Dat_Xml` / `Copia_Imagen_Almacenada_local`.

Tiene más de un límite transaccional. La reserva en `system1` confirma antes del bloque posterior de persistencia. El `Commit` del bloque documental está cerca de 3388; después se asigna `Id_Almacen` y, si corresponde, se guarda el XML de índice del expediente. Si falla ese guardado, la función puede devolver error aunque la transacción documental ya haya confirmado.

Por tanto, ni un error ni un timeout autorizan repetir almacenamiento. El adaptador conserva reclamación y evidencia propias y consulta el efecto. Si no puede determinarlo inequívocamente con los contratos actuales, mantiene estado incierto y atención controlada. No se añaden columnas/flags/operación al motor ni se prolonga artificialmente una transacción externa alrededor de commits internos para prometer atomicidad.

## Límites y observaciones a conservar sin corregirlos aquí

- Las fechas, folios y referencias descritas no son equivalentes entre todas las capas. La aceptación debe mostrar esas diferencias; no normalizarlas silenciosamente en la UI.
- Se observan asignaciones a caracterizar, como `ID_TIPO_EXPEDIENTE` del XML usando el ID de expediente, y un campo `ID_TIPO_UNIDAD_DOCUMENTAL` del preparador que toma `ID_UNIDAD_CONSERVACION`. Son observaciones del código leído, no una evaluación de datos reales. Cualquier corrección del motor/mapeador queda prohibida bajo D-RI-07.
- La función de conteo para documentos unificados devuelve páginas detectadas para PDF y puede conservar el valor de respaldo para otros formatos. El nuevo control no inventa otro cálculo ni cambia ese helper.
- Parte del almacenamiento usa sesión legacy y consume temporales. Un adaptador externo no acredita aislamiento por existir: debe demostrar pertenencia de rutas/contexto, ejecución y restauración segura cuando proceda, y comportamiento con dos pestañas. Si no puede lograrlo con las funciones intactas, esa integración se mantiene pendiente.

## Controles de protección y verificaciones

Antes y después de cada paquete:

```powershell
node tools/diagnostico/verify-radicacion-storage-freeze.cjs
```

El verificador compara las fuentes contra el commit de origen y la baseline; rechaza cambios de cuerpo/firma o cantidad de sobrecargas de los métodos protegidos y cualquier modificación del archivo central. Normaliza CRLF/LF; las líneas de ubicación son orientativas, por lo que una modificación externa de UI que desplace líneas no altera la huella de una función intacta. No tiene modo de actualización de baseline. Está prohibido editar baseline/verificador para permitir un cambio del almacenamiento.

Validación del propio control:

```powershell
node --test tools/diagnostico/verify-radicacion-storage-freeze.test.cjs
```

Pasaron seis casos: fuentes actuales intactas; rechazo de cambio central; rechazo de cambio en mapeo; aceptación de cambio fuera del ámbito protegido; equivalencia CRLF/LF; rechazo de huella alterada respecto del commit. Las alteraciones negativas se simulan en memoria; no se escriben en código de aplicación ni se ejecuta almacenamiento.

La evidencia funcional pendiente para P04/P05/P06 incluye: principal/anexos y rótulo separados, gabinetes iguales/distintos, tipologías/campos configurados, PDF/Multi-TIFF autorizados, fuentes de fecha/folios, actor/empresa, inventario e índices/XML, expediente cerrado, pérdida de respuesta antes/después del commit, temporales consumidos, dos pestañas y recuperación sin duplicar. Requiere el ambiente/cuentas/datos descartables autorizados y controles SELECT; el guard de fuentes no sustituye esa evidencia.

Resultado: es posible diseñar la UI y recepción temporal alrededor de las firmas existentes, pero la ruta operativa concreta depende de satisfacer sus precondiciones. **El almacenamiento y sus mapeos protegidos no se modifican para completar la nueva interfaz.**
