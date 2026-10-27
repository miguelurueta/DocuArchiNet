# Herencia de serie y subserie — DOC-110

Fecha: 2026-10-24. D-03 / RQ-03. Extensión del contrato 05.

## Contrato implementado

Origen EXPEDIENTE, etapa TRAS_EXPEDIENTE. No crea un proveedor adicional. Destino principal INDICE_GABINETE; también se admiten estas propiedades como clave heredada del proveedor por el resolvedor común. No se habilita herencia circular hacia CAMPO_EXPEDIENTE.

| Propiedad | Representación | Fuente del ensayo RA | Tipos permitidos |
|---|---|---|---|
| SERIE_ID | Identidad interna | series_documentales.Id_Series | TEXTO, ENTERO, REFERENCIA |
| SERIE_CODIGO | Código configurado, sin reemplazo automático | series_documentales.Codigo_Arbitrario | TEXTO |
| SERIE_NOMBRE | Nombre | series_documentales.Nombre_Serie | TEXTO |
| SUBSERIE_ID | Identidad interna | subseries_documentales.Id_SubSeries | TEXTO, ENTERO, REFERENCIA |
| SUBSERIE_CODIGO | Código configurado, sin reemplazo automático | subseries_documentales.Codigo_Arbitrario | TEXTO |
| SUBSERIE_NOMBRE | Nombre | subseries_documentales.Nombre_Subserie | TEXTO |

Codigo_Arbitrario no es Consecutivo_Serie ni Consecutivo_Subserie. No se inventan prefijos ni se convierten códigos a números. Si el código está vacío se informa la incidencia de valor vacío/requerido; no se utiliza el ID como fallback.

## Lectura y autoridad

Metadatos verificados mediante SELECT de information_schema en RA, resuelta por ODBC y gestor_modulos. Sin escrituras ni exposición de cadenas o registros personales. IDs int, nombres y Codigo_Arbitrario varchar. El catálogo reutiliza joins parametrizados de instrumento, área, serie y empresa; la subserie está limitada a su serie. Una referencia inaccesible se rechaza.

PerfilImportacionCatalogAdapter implementa IClasificacionEnsayoPerfil. ServicioPerfilImportacion obtiene una sola proyección por ensayo y reemplaza las seis propiedades del contexto recibido del cliente en una copia; no acepta esa entrada como clasificación confirmada. Los resultados afectados llevan Procedencia=PROPUESTA_PERFIL y la interfaz lo explica. Para versiones publicadas se usan exclusivamente reglas/configuración persistidas. Los nombres y códigos consultados corresponden al catálogo vigente; no son una instantánea histórica del expediente ni alteran la huella publicada.

Serie aún sin seleccionar: propiedades pendientes (clave ausente en contexto). Serie seleccionada sin subserie: las tres propiedades de subserie tienen ausencia conocida (null). Ausencia conocida en regla obligatoria produce VALOR_REQUERIDO; opcional devuelve null, nunca 0. El gabinete vuelve a exigir sus propias restricciones de obligatoriedad/tipo/longitud.

## Contrato de consumo operativo futuro

DOC-110 no crea ni reutiliza expedientes. Cuando la importación confirme el expediente destino, su adaptador deberá construir el contexto desde ese expediente autorizado y comprobar compatibilidad de instrumento, área, serie y subserie con el plan. No debe reutilizar la propuesta del ensayo como si fuera una confirmación.

Evidencia de mapeo: expediente_archivo tiene CODIGO_SERIE_TRD, CODIGO_SUB_SERIE_TRD, NOMBRE_SERIE_TRD, NOMBRE_SUBSERIE_TRD, id_instrumento e ID_EMPRESA_EXPEDIENTE. ModernExpedientMutationBridge, líneas de asignación de clasificación, demuestra que CODIGO_SERIE_TRD recibe IdSerie y CODIGO_SUB_SERIE_TRD recibe IdSubSerie: son referencias internas, no Codigo_Arbitrario. Se revisó como referencia sin modificar SII.

El lector operativo deberá unir esas referencias con sus catálogos bajo contexto autorizado y distinguir nombres conservados en el expediente de etiquetas actuales. Debe verificar permisos del expediente además de empresa; empresa coincidente por sí sola no autoriza acceso. Expediente incompatible o lectura incierta bloquea resolución; no sustituir clasificación ni elegir otro expediente. El lector operativo y sus pruebas quedan en la entrega de importación, fuera del ensayo DOC-110.

## Persistencia y compatibilidad

Se reutiliza ra_integracion_perfil_regla.regla_doc, sin SQL nuevo. Los seis nombres son valores adicionales de PropiedadExpediente; la configuración del perfil conserva SerieRef/SubserieRef. Procedencia solo pertenece a la respuesta de ensayo y se omite cuando no aplica. Nuevas reglas se guardan en borrador/nueva versión, nunca editando una publicación existente.

## Evidencia local

- Build aislado .NET: PASS, sin copiar DLL a bin ni desplegar.
- Arnés .NET existente: PASS con pruebas de seis propiedades, ceros iniciales, diferido/ausente/obligatorio, tipo y longitud, round-trip JSON, catálogo parametrizado y rechazo antes de IO, sustitución de contexto cliente y publicación inmutable.
- Editor Node: 11 PASS.
- Playwright doc110-perfil-guided: 2 PASS con transporte simulado; selector, guardado/reapertura y ensayo resuelto/pendiente/ausente.
- Aceptación HTTP autenticada y persistencia real de nuevas reglas pendientes. Los dobles no acreditan MySQL ni la futura importación.
