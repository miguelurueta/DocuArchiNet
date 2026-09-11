# 08 — Interior del almacenamiento legacy reutilizado

Fuente: `workflow/ClassAlmacenamiento.vb`, función de solo lectura `AlmacenaDocumentoTareaWorkflow`.

```mermaid
flowchart TB
    A[AlmacenaDocumentoTareaWorkflow] --> F{FileInfo.Exists}
    F -- no --> EF[retorna archivo inaccesible]
    F -- sí --> T{Extension TIF}
    T -- sí --> MT[ClassNeodynamic.Extraer_Documento_de_Multitif_fisico]
    T -- no --> M1[MatrizDocumentosFinal con archivo único]
    MT --> NR[Class_DETALLE_GABIENETE.SolicitaNombreCampoRadicadoGabinete]
    M1 --> NR
    NR --> IDX1[agrega campo radicado y ENLASE]
    IDX1 --> IT[Class_DAT_ADIC_TAR.SolicitaidImagenTareaworkflow]
    IT --> CF[ClassGaTipoDocumental.SolicitaIdTipoFormatoDocumento]
    CF --> FE[ClassGestionFechas.FormateaFechaAlmacenamiento]
    FE --> SII{NombreCaso SII}
    SII -- sí --> SO[elimina prefijo S0 de ValorObjeto]
    SII -- no --> EX
    SO --> EX[ClassGaExpediente.SolicitaEstructuraExpedienteDocumentoVinculante]
    EX --> E{EstructuraGestion.ID_EXPEDIENTE distinto de 0}
    E -- sí --> ED[ClassGaExpediente.SolicitaDatosEstructuraExpediente]
    ED --> LC[ra_dig_tipos_docum_lista_chequeo.SolicitaDatosTipoDocumentalListaChequeo]
    LC --> ND[Class_ra_tipo_doc_series.SolicitaNombreTipoDocumentalSerieSubSerie]
    E -- no --> CL{IdTipoListaChek válido}
    CL -- sí --> CD[ClassGaTipoDocumental.SolicitaEstructuraClasificacionTipoDocumento]
    CL -- no --> MG
    ND --> MG[construye estructure_gestion y campos EXPEDIENTE/CLASE/FECHA/TIPO/SERIE/SUBSERIE]
    CD --> MG
    MG --> IX[agrega CDcamposAsignaAlmacenamiento no vacíos]
    IX --> VM[Class_DETALLE_GABIENETE.SolicitaValoresCamposDocumentoGabinete]
    VM --> ET[Class_da_extension.SolicitaTipoArchivoDocuarchiExtension]
    ET --> AL[ClassAlmacenamiento.Almacenamiento]
    AL --> OK{Resultado YES}
    OK -- no --> ER[retorna error legacy]
    OK -- sí --> UI[llena stru_datos_image_lista]
    UI --> DL[elimina MatrizDocumentosFinal]
    DL --> TMP{TipoAlmacenamiento distinto de 0}
    TMP -- sí --> DT[elimina WF_RUTA_TEMPO_ADJUNTA y limpia sesión]
    TMP -- no --> YES[retorna YES]
    DT --> YES
```

Parámetros formales: `ActivaGuardaValorRadicado`, `NombreGabinete`, `Radicado`, `RutaArchivoAlmacenar`, `NombreRutaWorkflow`, `IdRutaWorkflow`, `IdTareaWorkflow`, `DescripcionTipo`, `IdTipoListaChek`, `TipoAlmacenamiento`, `CDcamposAsignaAlmacenamiento`, `ValorObjeto`, `NombreCaso`, `NombreClaseFormatoDocumento`, `IdImagenAlamacenada` y `EstructuraDatosImagen`.

La ruta moderna no duplica estas operaciones: construye el comando, invoca esta función una vez y reconcilia después usando datos persistidos.
