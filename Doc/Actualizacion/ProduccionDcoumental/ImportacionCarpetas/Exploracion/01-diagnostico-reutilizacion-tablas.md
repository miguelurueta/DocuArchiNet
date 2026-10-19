# Diagnóstico de reutilización de tablas para la nueva integración

Fecha: 2026-10-19.

Actualización posterior: se realizó inspección real de metadatos mediante la conexión ODBC configurada. Ver [03-validacion-bd-y-definicion-perfil.md](03-validacion-bd-y-definicion-perfil.md) y [02-evidencia-esquema-perfil.json](02-evidencia-esquema-perfil.json). El alcance de solo repositorio que sigue describe la primera etapa; las dudas sobre `ra_ser_config_servicio`, nombres de tablas de tipologías externas y esquema físico se precisan en esa actualización.

## Alcance y conclusión

Revisión estática de consultas VB.NET, consumidores y scripts SQL del repositorio. No se consultó la base desplegada ni se ejecutaron cambios de esquema o datos. Para las tablas legacy no se localizó en esta revisión un DDL completo que permita certificar todas las columnas, restricciones, índices o triggers; las capacidades descritas se basan en los accesos observados.

Recomendación: reutilizar catálogos, metadatos del gabinete y almacenamiento documental existentes; crear configuración y estado operativo propios para la nueva integración. No convertir el autorregistro SII en el perfil de carpetas. SII, su esquema, configuración y consumidores permanecen intactos.

Este diagnóstico precisa la exploración inicial: encontrar campos parecidos en autorregistro no implica que sus tablas sean una base adecuada para un perfil independiente.

## Tablas y decisión

| Tabla o familia | Evidencia y capacidad | Decisión |
|---|---|---|
| `ra_de_fondo_documental`, catálogos de instrumentos/organigramas, `areas_depart_radicacion`, `series_documentales`, `subseries_documentales` | Clasificación existente consumida por Gestión. | Referenciar identificadores y validar relaciones; no duplicar catálogos. |
| `tipo_doc_series`, `ra_tipo_expediente` | Tipologías por serie/subserie y tipos de expediente. | Reutilizar en lectura y validar vigencia/compatibilidad. |
| `system1`, `detalle_gabienete` | Gabinetes y definición de campos, tipos y propiedades. | Reutilizar como autoridad del destino; las reglas nuevas referencian campos reales. |
| `permisos_usuarios_gabinetes`, `permisos_grupos_gabinetes` | Permisos existentes sobre gabinetes. | Integrar autorización vigente, sin confundirla con permiso de administrar perfiles. |
| `ra_auto_registro_expediente` | Nombre y `funcion_servicio_datos`; despacho a funciones de autorregistro. | Referencia funcional, no cabecera del nuevo perfil. |
| `ra_auto_campos_gestion_expediente` | Fondo, instrumento, área, serie y subserie por autorregistro. | Posible lectura como plantilla para inicializar una versión nueva; no dependencia editable compartida por defecto. |
| `ra_auto_campo_unico_expediente` | Campo, obligatorio y único por autorregistro. | Referencia de identidad/obligatoriedad; no almacena las reglas de extracción requeridas en los accesos revisados. |
| `ra_auto_detalle_campos_expediente` | Catálogo de nombre, tipo y longitud utilizado en joins de autorregistro. | Candidato de lectura como catálogo de destinos, previa validación de cobertura y esquema real. |
| `ra_auto_rel_campos_gabinete_expediente` | Relación de campos gabinete → expediente. Consulta revisada sin filtro por autorregistro. | No utilizar su lector como resolvedor aislado por perfil ni insertar reglas nuevas para este fin. |
| `ra_auto_rel_campos_plantilla_rad_expediente` | Relaciona campos de plantilla de radicado con expediente. | No corresponde al origen nombre de archivo; no forzar reutilización. |
| `ra_gabexp_relacion_index_gabinete_expediente`, `ra_gabexp_relacion_campos_gabinete_expediente`, `ra_gabexp_campos_detalle_expediente` | Herencia expediente → gabinete con relación predeterminada por gabinete. | Consumir relaciones compatibles en lectura; no cambiar el default para una importación particular. |
| `ra_campos_fecha_gabinete_produccion` | Campos de fecha seleccionados por gabinete. | Respetar/consultar configuración existente; no usarla como motor genérico de extracción. |
| `RA_CONFIG_UPLOAD_GESTION` | Extensiones y tamaño por proceso activo. | Consultar como política base compatible; no modificar registros usados por otros recorridos para configurar perfiles. |
| `RA_PRO_CONFIG_PRODUCION_DOCUMENTAL` | Opciones generales; consulta sin filtro de perfil y uso de la primera fila. | No agregar una fila por perfil: el lector no está diseñado para ello. |
| `ra_ser_servicioIntegracion` | Identificador, nombre, fecha y estado del servicio. | Es un catálogo de servicios observado, no un perfil documental completo. No se necesita registrar la nueva integración en el recorrido SII. |
| `ra_ser_config_servicio`, `ra_ser_tipos_serviciointegracion` | Las clases homónimas revisadas están vacías. | Evidencia insuficiente para afirmar que implementan configuración reutilizable. Verificar esquema antes de cualquier decisión. |
| `ra_mig_config_migracion`, `ra_mig_registro_migracion` | Configuración OCR/compresión/formato y registro de migraciones/versiones. | No representan el nuevo perfil ni el lote carpeta → expediente. |
| `workflow_import_intent*`, `workflow_import_inscription`, registros relacionados/cache SII | DDL exige tareas/trámites/rutas e incluye inscripciones y claves específicas. | Solo referencia. No modificar ni escribir lotes de carpetas en estas tablas. |
| `expediente_archivo`, `registro_producion_documental`, relaciones de nivel e índices documentales | Destino documental existente. | Reutilizar mediante operaciones documentales compatibles, sin insertar directamente saltándose reglas. |

## Hallazgos determinantes

### 1. Autorregistro tiene consecuencias posteriores a la creación

En `Gestion/ClassGaExpediente.vb`, las funciones `Actualiza_Expediente_Conservacion` (línea 1636 en la revisión) y `Actualiza_expediente_produccion` (línea 2339) rechazan la edición si `ra_auto_registro_expediente_id_auto_registro <> 0`.

Guardar un ID de autorregistro para reutilizar configuración no es una referencia inocua: activa una restricción de negocio. No se debe asociar por comodidad ni alterar esa restricción. La decisión de editabilidad de los expedientes importados debe ser explícita; su trazabilidad al perfil nuevo se conserva en el registro nuevo de importación.

### 2. Un lector recibe un ID que no aplica en su consulta

`Gestion/Class_ra_auto_rel_campos_gabinete_expediente.vb`, función `Solicita_estructura_relacion_auto_registro_gabinete_expediente`, recibe `id_auto_registro`, pero su SELECT une las tablas sin filtrar por ese parámetro. El comentario también indica que no se tiene en cuenta la identificación.

No es apto para aislar reglas por perfil. No se propone corregir el lector compartido: cualquier lectura nueva debe tener contrato y filtro propios y no cambiar SII.

### 3. La herencia expediente → gabinete tiene alcance compartido

`Gestion/Class_Ra_gabexp_relacion_index_gabinete_expediente.vb` selecciona por `system1_id_gabinete` y `default_relacion=1`. Cambiar ese default para un perfil podría afectar otros consumidores del gabinete.

La nueva regla por archivo debe ser independiente y componer un valor final por campo con precedencia explícita. No debe alterar una relación compartida para lograr una excepción local.

### 4. El esquema operativo SII tiene identidad Workflow

Los scripts de DOC-52 y DOC-67 definen `task_id`, `route_id`, `procedure_id`, `target_task_id` y claves de idempotencia que incorporan tarea. DOC-67 incluye matrícula e inscripción.

Reutilizarlo exigiría datos ficticios o cambios incompatibles con el aislamiento obligatorio. Los estados de carpeta/archivo deben tener almacenamiento propio.

### 5. No se encontró un motor persistido de extracción de nombres que cubra el requisito

En las familias examinadas, las relaciones conectan campos entre entidades, pero no describen conjuntamente fuente nombre de archivo/carpeta, separador, posición, conversión, fallback y precedencia por versión de perfil. La conclusión está limitada al código revisado; no certifica ausencia de tablas no documentadas en la base desplegada.

## Modelo nuevo mínimo recomendado para configuración

Nombres conceptuales, pendientes de diseño y convenciones del repositorio:

1. `integracion_perfil`: identidad estable, nombre, descripción, ámbito y estado.
2. `integracion_perfil_version`: versión inmutable publicada, referencias de clasificación/gabinete, valores fijos tipados y políticas. Puede referenciar opcionalmente la procedencia de una plantilla interna sin mantener dependencia mutable.
3. `integracion_perfil_regla`: reglas por versión para destinos permitidos; fuente constante, carpeta, archivo o expediente; método y parámetros, tipo, obligatoriedad y precedencia. No necesita una tabla distinta por cada posición ni por cada campo.

El control de acceso específico al perfil puede necesitar una relación adicional si el mecanismo existente no expresa ese permiso. No se presupone que los permisos de gabinete cubran administración o selección de perfiles.

Las reglas se asocian a la versión; no se edita una versión publicada que ya respalda un lote. La configuración actualizada crea otra versión. Los identificadores de catálogos no son copias de sus definiciones: solo seleccionan las entidades existentes.

Para ejecución hacen falta entidades separadas de lote, carpeta, archivo y eventos, como se describe en la exploración principal. La separación evita confundir perfil reutilizable con resultados de una importación.

## Verificación necesaria antes de aprobar DDL

- Confirmar columnas, tipos, claves, nulabilidad, índices, triggers y ubicación real de cada tabla candidata mediante introspección de solo lectura autorizada.
- Confirmar conexiones: hay accesos mediante fábricas DA y RA; no asumir que todas las tablas están en la misma base ni que se pueden crear claves foráneas entre ellas.
- Comprobar completitud del catálogo de campos de expediente y esquema de campos del gabinete.
- Definir la editabilidad posterior de expedientes importados y el uso correcto del parámetro de autorregistro de las operaciones existentes.
- Confirmar composición de permisos para nueva integración.
- Revisar todos los cambios para impedir modificaciones a SII o a configuración compartida usada por él.

No se ejecutaron pruebas E2E, cargas ni escrituras de base de datos. La revisión documental no requirió repetir las pruebas de carga estáticas ejecutadas anteriormente.

## Evidencia principal

- `Gestion/Class_ra_auto_registro_expediente.vb`.
- `Gestion/Class_ra_auto_campos_gestion_expediente.vb`.
- `Gestion/Class_ra_auto_campo_unico_expediente.vb`.
- `Gestion/Class_ra_auto_rel_campos_gabinete_expediente.vb`.
- `Gestion/Class_ra_auto_rel_campos_plantilla_rad_expediente.vb`.
- `Gestion/Class_Ra_gabexp_relacion_index_gabinete_expediente.vb`.
- `Gestion/Class_ra_gabexp_relacion_campos_gabinete_expediente.vb`.
- `Gestion/ClassGaExpediente.vb`.
- `Gestion/Class_ra_pro_config_producion_documental.vb`.
- `Gestion/Class_ra_config_upload_gestion.vb`.
- `Docuarchi/Class_DETALLE_GABIENETE.vb`.
- `Docuarchi/ClassRaCamposFechaGabineteProduccion.vb`.
- `ServiciosIntegracion/Class_ra_ser_servicioIntegracion.vb`.
- `ServiciosIntegracion/Class_ra_ser_config_servicio.vb`.
- `ServiciosIntegracion/Class_ra_ser_tipos_serviciointegracion.vb`.
- `Gestion_migracion/Class_ra_mig_config_migracion.vb`.
- `Gestion_migracion/Class_ra_mig_registro_migracion.vb`.
- `Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportExpedientConfigurationRepository.vb`.
- `Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-52-preflight-intencion-idempotencia/Sql/001-create-import-intents.sql`.
- `Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-67-creacion-vinculacion-expediente/Sql/005-create-doc67-consolidated-workflow-mysql51.sql`.
