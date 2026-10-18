# Inventario inicial de consumidores DOC-105

Este inventario se deriva de referencias estáticas reales. No representa todavía cobertura E2E: todas las filas permanecen pendientes hasta una ejecución autorizada.

## Constructores enriquecidos

| Constructor | Entrada conocida | Transporte enriquecido | Fallback preservado |
| --- | --- | --- | --- |
| `Docuarchi/ClassVisualisaDocumento.vb` | Workflow y Gestión de Correspondencia llaman `Visualiza_documento_workflow_visor` | Emite `doc_ref`, selecciona `pdf_load_mode` y abre `pdf_anotate_view` | URL directa a `Handler_image_wf.ashx?rut_image=...` cuando el visor express no aplica |
| `workflow/ClassWorflowVisor.vb` | Clasificación, aprobación y visor externo autenticado | Para `LIBRE_PDF` emite `doc_ref`, selecciona `pdf_load_mode` y abre el visor enriquecido | Handler directo para tipos/modos no enriquecidos |
| `Gestion/ClassGaProducionDocumental.vb` | `Gestion/WebFormProducionDocumental.aspx.vb` llama `Activa_visualizacion_documento_producion` | Emite `doc_ref`, selecciona `pdf_load_mode` y abre el visor enriquecido | Visor Docuarchi o flujo previo según tipo/permiso |

`Infrastructure/Documentos/Streaming/EstrategiaCargaPdf.vb` selecciona `range` solo para PDF de al menos 1 MiB con marcador `/Linearized` dentro de los primeros 4096 bytes; los demás usan `stream`. La selección no copia ni modifica el PDF almacenado.

## Llamadas centrales

| Consumidor | Archivo y símbolo | Tipo | Scroll/zoom | Mutación DOC-105 |
| --- | --- | --- | --- | --- |
| Workflow principal | `workflow/Webworkflow.aspx.vb`, llamadas activas alrededor de 1898 y 2220 | Enriquecido por función central | Sí | Un guardado de grafo descartable |
| Gestión de Correspondencia | `Gestion_correspondencia/WebForm_interface_gestion_tramite.aspx.vb`, llamada alrededor de 1090 | Handler en visor nativo; `anula_visor_espress = 1` | No aplica a PDF.js | No |
| Producción Documental | `Gestion/WebFormProducionDocumental.aspx.vb`, llamada alrededor de 996 | Enriquecido por constructor propio | Sí | No |
| Aprobación/clasificación | `Gestion/WebFormGaVisorClasificacion.aspx.vb` y `workflow/ClassWorflowVisor.vb` | Enriquecido o fallback según tipo | Sí cuando abre PDF.js | No |

## Consumidores directos del handler

| Clase de consumidor | Archivos | Validación aplicable |
| --- | --- | --- |
| Descarga Docuarchi | `Docuarchi/WebFormVisorDescarga.aspx.vb`, `Docuarchi/WebFormDaVisorExternoDescarga.aspx.vb` | Autorización, HTTP y transferencia; scroll/zoom no aplican |
| Preview externo Docuarchi | `Docuarchi/WebFormDaVisorExterno.aspx.vb`, `Docuarchi/WebFormDaVisorExternoPublico.aspx.vb` | Apertura/permiso, HTTP y transporte |
| Preview externo Gestión | `Gestion/WebFormGaVisorDocumentoExterno.aspx.vb` | NOT_APPLICABLE para rutas identificadas; ver revisión 7.5.7 |
| Versiones | `Gestion/Class_ra_ver_version_documento.vb` | Preview/descarga de versión, cero mutaciones |
| Migración | `Gestion_migracion/Class_ra_mig_registro_migracion.vb` | Preview/descarga migrada, cero mutaciones |
| Otros formatos Workflow | `workflow/WebFormVisor_Otros_Formatos.aspx.vb` | Descarga/preview; scroll/zoom no aplican |

Las referencias internas de los tres constructores y el propio `workflow/Handler_image_wf.ashx` no se cuentan como consumidores adicionales. Cada clase anterior tendrá estado independiente `PASSED`, `FAILED`, `BLOCKED` o `NOT_APPLICABLE` en la matriz final.

## Reglas de no regresión

Revisión 7.5.7: las referencias encontradas en `radicador/WebFormCorespondenciaporenviar.aspx.vb` y `radicador/WebFormArchivaenviocorrespo.aspx.vb` asignan el visor legacy al iframe y en la instrucción siguiente lo reemplazan por `WebFormGaVisorClasificacion.aspx`. No existe render intermedio. La política local comprueba ambas secuencias. El archivo legacy se conserva, no se eliminan permisos ni se fuerza su apertura directa. Esta conclusión se limita a las rutas del repositorio: no acredita ausencia de enlaces externos ni aprobación E2E del legacy.

- Conservar permisos, logs normales de visualización y fallback existentes.
- No construir una ruta física desde la prueba ni llamar al handler eludiendo el consumidor.
- No reintroducir `WorkflowCentroTrabajoModernActive` ni activación por usuarios/grupos.
- No declarar que un consumidor pasó porque otro comparte constructor o handler.
