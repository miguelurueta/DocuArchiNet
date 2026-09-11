# Metadatos, tipología e índices

## Traducción de tipología

El frontend envía `DocumentTypeId=tipo_doc_series.Id_Tipo_Doc_Series` y `DocumentTypeName`. `MySqlImportDocumentTypeResolver.Resolver` exige una coincidencia única en `ra_dig_tipos_docum_lista_chequeo` para `(IdTramite, DocumentTypeId)` y concordancia del nombre. Solo `ID_TIPO_DOCUMENTAL_CHEQUEO` llega como `IdTipoListaChequeo` al legacy.

## Autoridad de infraestructura

`MySqlImportStorageMetadataRepository.Resolver` obtiene `rutas_workflow.Nombre_Ruta` y el gabinete desde `dat_adic_tar<ruta>`/`config_gabinete`; fija `DOCUMENTO ELECTRONICO`. `StoreImportExecutionStep` obtiene recibo y código de barras de tarea mediante `Class_DAT_ADIC_TAR.SolicitaReciboCodigoBarrasSII`.

- `CODBARRAS`: código consultado en SII.
- `ENLASE`, `RECIBOCAJA` y radicado documental: recibo resuelto desde la tarea.
- `IdTipoDocumental`: ID TRD público.
- `IdTipoListaChequeo`: ID contextual interno.

| Gabinete | Fecha | Acto descriptivo | Matrícula |
|---|---|---|---|
| mercantil | `FECHAREGISTR` | `DESCRIACTO` | `matricula` |
| rup | `FECHAREGISTR` | `DESCRIPCIONA` | `proponente` |
| esal | `FECHAINSCRIP` | `DESCRIPCIONA` | `matricula` |

Cuando el sello no aporta razón social, `StoreImportExecutionStep` llama `Class_ra_sii_migra_imagenes.SolicitaEstructuraCamposSII` con matrícula/proponente y gabinete. Si no resuelve sujeto válido, retorna `SII_SUBJECT_METADATA_UNAVAILABLE` sin almacenar.

El libro elimina prefijos `RM`/`RE`/`RP`; fecha `yyyyMMdd` pasa a `yyyy-MM-dd`; columnas enteras reciben solo dígitos; textos respetan el DDL.

`LegacyImportDocumentStorageAdapter.Almacenar` llama una vez `ClassAlmacenamiento.AlmacenaDocumentoTareaWorkflow` con archivo temporal, gabinete, radicado, ruta Workflow, tarea, tipología contextual, almacenamiento `2`, clase documental, índices y login. Solo `YES` con `IdImagen>0` se interpreta como persistencia conocida.

La función legacy también llena `stru_datos_image_lista`, pero el adaptador moderno no usa esa estructura como autoridad de salida. El contrato moderno obtiene nombre, relación y documento mediante `MySqlImportReconciliationRepository`; por eso la interfaz solo recibe un documento confirmado en gabinete.
