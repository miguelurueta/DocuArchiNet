<!-- opsxj:refinement version=1 state=approved -->

# DOC-112 — Refinamiento documental para revisión

## Fuente y alcance

- Ticket DOC-112, prompt 04: diseño físico y contratos.
- Base de referencia main `40c5f55e`, con DOC-110/111 integrados.
- Tecnología observada: WebForms/VB.NET Framework 4.6.1, JavaScript, ASMX/ASHX y MySQL 5.1. Se documenta el sistema existente; no se aplica una plantilla de frontend/API diferente.
- Fuente Jira inalterada: `specs/produccion-importacion-carga-carpeta-contrato/jira-context.md`.
- Entregables comunes: Diseno/design.md y Diseno/01–04; OpenSpec mantiene el plan y las condiciones de revisión.

## Contexto inspeccionado

- `DTOs/ProduccionDocumental/ImportacionCarpetas/ImportacionCarpetasDtos.vb`.
- `Modelo/ProduccionDocumental/ImportacionCarpetas/ImportacionCarpetasModels.vb`.
- `Modelo/ProduccionDocumental/ImportacionCarpetas/ImportacionCarpetasInterfaces.vb`.
- `Services/ProduccionDocumental/ImportacionCarpetas/ServicioPreparacionImportacion.vb`.
- `Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.vb`.
- `Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Valores.vb`.
- `Services/ProduccionDocumental/ImportacionCarpetas/LimitesImportacionCarpetas.vb`.
- `Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.vb`.
- `Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorDocumentoImportacion.vb`.
- `webservice/WebServiceImportacionCarpetas.asmx.vb`.
- `generic_control/ImportacionCarpetasUpload.ashx.vb`.

También se contrastaron la guía 00, los cuatro diseños comunes y el inventario, SQL operativo 02 y migración 03 de identidades. La documentación de exploración y aceptación anterior se usa como evidencia con alcance histórico, sin nuevas conexiones o efectos.

## Decisiones refinadas

| ID | Decisión verificable | Evidencia | Design | Requirement | Tasks |
|---|---|---|---|---|---|
| D-01 | Actualizar los cuatro diseños comunes y su inventario a partir de DOC-110/111 integrados; OpenSpec referencia esos contratos y no mantiene un segundo diseño físico. | Diseno/design.md; guía 00; archivo DOC-111 y matriz final | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Contrastar perfil/version/regla y las seis tablas operativas con repositorios y SQL existente, incluida migración de identidades de DOC-110; separar referencias legacy y catálogos de tablas propias. | Sql/01-perfil-importacion-mysql51.sql; Sql/03-migrar-identificadores-perfil-mysql51.sql; Sql/02-carga-carpetas-mysql51.sql; MySqlImportacionCarpetasRepository.vb | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Distinguir seleccionId/idCliente de PK servidor; describir SHA-256, canonicalización y comparación del contenido completo para plan, ruta y fase, sin unicidad global del nombre visible. | ImportacionCarpetasDtos.vb; MySqlImportacionCarpetasRepository.vb; PerfilImportacionJson.vb | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Mantener separados administrar e importar; resolver identidad/empresa en servidor y permisos de nivel/gabinete/acción, CSRF de JSON/multipart y errores seguros sin rutas, SQL o tokens. | PerfilImportacionContextAdapter.vb; ImportacionCarpetasContextAdapter.vb; guards; ImportacionCarpetasUpload.ashx.vb | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Documentar publicación inmutable, JSON ASCII versionado y acotado, valores PREFLIGHT/TRAS_EXPEDIENTE, IDs aún inexistentes, fechas generadas y valores efectivos ya fijados. | PerfilImportacionModels.vb; ImportacionCarpetasModels.vb; PerfilImportacionJson.vb; CoordinadorImportacionCarpetas.Valores.vb | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | Inventariar la fachada ASMX y el handler multipart actuales con entradas/salidas, propietario, revisión, errores y efectos. Documentar equivalencias de la guía sin inventar endpoints de adaptador. | WebServiceImportacionCarpetas.asmx.vb; ImportacionCarpetasDtos.vb; ImportacionCarpetasInterfaces.vb; ImportacionCarpetasUpload.ashx.vb | D-06 | RQ-06 | Origen: D-06, RQ-06 |
| D-07 | Separar PLAN, RECEPCION, EFECTO, RAIZ, INTENTO y LOTE; contrastar transiciones cerradas y agregados con CoordinadorImportacionCarpetas sin equiparar bytes recibidos con incorporación. | CoordinadorImportacionCarpetas.vb; CoordinadorImportacionCarpetas.Consolidacion.vb; ImportacionCarpetasModels.vb | D-07 | RQ-07 | Origen: D-07, RQ-07 |
| D-08 | Conservar COMPLETO_OBLIGATORIO con ámbito EXPEDIENTE o LOTE y PARCIAL_PERMITIDO/NO_APLICA; describir CAS/revisión/reclamaciones y barreras del mismo coordinador. | CoordinadorImportacionCarpetas.Barreras.vb; MySqlImportacionCarpetasRepository.Barreras.vb; MySqlImportacionCarpetasRepository.Control.vb | D-08 | RQ-08 | Origen: D-08, RQ-08 |
| D-09 | Caracterizar creación y almacenamiento actuales: contexto temporal delimitado, copia consumible, nombre original, Archivo Gestión por nombre y prueba de IDs/relaciones/índices; preservar función legacy y SII. | AdaptadorExpedienteImportacion.Legacy.vb; AdaptadorDocumentoImportacion.vb; ComandoAlmacenamientoCarpetas.vb; workflow/ClassAlmacenamiento.vb | D-09 | RQ-09 | Origen: D-09, RQ-09 |
| D-10 | Documentar incertidumbre entre SQL/disco/XML/respuesta, evidencia positiva/negativa y cierre de reserva sin inicio; un timeout o antigüedad no habilita replay. | CoordinadorImportacionCarpetas.Reconciliacion.vb; MySqlImportacionCarpetasRepository.Reconciliacion.vb; MySqlImportacionCarpetasRepository.Documento.vb | D-10 | RQ-10 | Origen: D-10, RQ-10 |
| D-11 | Registrar límites derivados del perfil/transporte/JSON/sesión, disponibilidad de publicaciones y condiciones operativas de volumen/navegador/retención con responsable y efecto del bloqueo. | LimitesImportacionCarpetas.vb; ServicioConsultaImportacion.Limpieza.vb; Diseno/04-decisiones-y-pendientes.md | D-11 | RQ-11 | Origen: D-11, RQ-11 |
| D-12 | Recorrer contratos en papel con dos raíces, barreras, diferidos, revisión y fallos; separar evidencia heredada de DOC-111 de la revisión DOC-112 y validar enlaces/D-RQ-tareas. | Diseno/01–04; tasks.md; refinement.md; Validacion/doc111-matriz-final-20261102.md | D-12 | RQ-12 | Origen: D-12, RQ-12 |

## Requisitos verificables

| ID | Resultado observable | Criterio de aceptación | Compatibilidad |
|---|---|---|---|
| RQ-01 | Inventario y propietarios verificables | Al cotejar un componente se identifica su ruta, propietario y estado real sin crear otra capa. | Solo documentación, con fuente/propietario y evidencia delimitada. |
| RQ-02 | Diccionario físico reproducible | Cada columna y clave documentada tiene tipo, nulabilidad, ámbito, índice y fuente; un script existente no acredita instalación en otro ambiente. | Solo documentación, con fuente/propietario y evidencia delimitada. |
| RQ-03 | Identidad y repetición sin ambigüedad | Misma clave/contenido devuelve el resultado previo; cambiar contenido, revisión o recurso no autoriza duplicar el efecto. | Solo documentación, con fuente/propietario y evidencia delimitada. |
| RQ-04 | Matriz de autoridad por fase | Un importador no necesita administración; sesión/permiso/token inválidos impiden la operación afectada antes de sus efectos. | Solo documentación, con fuente/propietario y evidencia delimitada. |
| RQ-05 | Valores resolubles sin crear durante preview | Un dato que requiere expediente conserva dependencia tipada; un checkpoint fijado, incluso vacío, no se recalcula por repetición. | Solo documentación, con fuente/propietario y evidencia delimitada. |
| RQ-06 | Cliente y servidor usan el mismo contrato | Cada operación tiene DTO y observable existentes; detención equivale a SolicitarDetencion y no promete cancelación destructiva o un CancelarLote inexistente. | Solo documentación, con fuente/propietario y evidencia delimitada. |
| RQ-07 | Transiciones y totales consistentes | Una transferencia recibida no confirma un documento y un rechazado sigue en los totales originales. | Solo documentación, con fuente/propietario y evidencia delimitada. |
| RQ-08 | Tres políticas con una coordinación común | EXPEDIENTE avanza la raíz lista, LOTE bloquea ambas si falta un archivo y PARCIAL avanza elegibles conservando rechazados. | Solo documentación, con fuente/propietario y evidencia delimitada. |
| RQ-09 | Adaptadores caracterizados sin cambiar escritores compartidos | Almacenamiento ya incorporado no se incorpora otra vez; NOMBRE respeta visibilidad; código de formato con signo no se confunde con ID de recurso. | Solo documentación, con fuente/propietario y evidencia delimitada. |
| RQ-10 | Recuperación sin inventar causalidad | Un resultado incierto conserva recursos; solo evidencia válida permite reconciliar o cerrar la reserva nunca iniciada bajo el mismo bloqueo de inicio. | Solo documentación, con fuente/propietario y evidencia delimitada. |
| RQ-11 | Límites y exclusiones explícitos | No se convierten 500 archivos/25 MB del prototipo en promesa; sin política de retención se conservan originales. | Solo documentación, con fuente/propietario y evidencia delimitada. |
| RQ-12 | Revisión documental reproducible | DOC-112 no ejecuta E2E/MSBuild/SQL ni acredita otra implementación al revisar contratos ya existentes. | Solo documentación, con fuente/propietario y evidencia delimitada. |

## Resultado del refinamiento

Estado: borrador refinado y listo para revisión del responsable. Las decisiones históricas de ambos ámbitos, permisos separados y conservación de efectos se reutilizan; no se solicita redefinirlas. La confirmación editorial de este refinamiento es distinta de esas decisiones existentes. El marcador seguirá draft hasta esa revisión explícita.

La validación esperada ahora es OpenSpec estricto y trazabilidad completa; el control de aprobación seguirá abierto. La implementación posterior del plan modifica solo documentación común. Volumen/navegadores/retención se registran con responsable y efecto, sin convertir hipótesis en reglas fijas.

## Depuración del plan por implementación existente

El contraste [gap-analysis.md](gap-analysis.md) reduce 52 tareas a doce correcciones documentales (6 S y 6 M). Los doce D/RQ se mantienen como criterios de revisión; su cobertura aprovecha lo ya implementado y documentado. Las referencias de la tabla Tasks identifican el origen de cada corrección, no doce componentes nuevos. No se da por completada ninguna tarea DOC-112 por el PASS de DOC-111.

Las condiciones operativas que siguen abiertas se registran con responsable por rol y efecto; las aceptaciones históricas se conservan con límites. El plan no requiere rediseñar diccionarios completos, inventar contratos o repetir REC. La aprobación editorial sigue abierta; esta reducción no cambia el marcador draft.

## Aplicación documental por instrucción de continuar

Las doce correcciones se aplicaron el 2026-11-02, conservando las capacidades y evidencia de DOC-110/111. El acta en Validacion/doc112-contratos-revision.md registra cambios, comprobaciones, límites y condiciones operativas. OpenSpec estricto y los controles documentales pasan; no se ejecutaron E2E, SQL o compilación. La autorización de trabajo no se transforma en aprobación editorial del resultado: se conserva draft hasta revisión del responsable.

## Aprobación editorial registrada — 2026-11-02

El usuario respondió «aprobado» a la solicitud explícita de aprobar esta entrega documental para publicación y cierre. La aprobación cubre las doce correcciones y el reporte verificado SHA256 `a7ec370f0a8a71eaef4aef1b59654c8311d80423f09565999f22743916208db3`; los estados draft/abiertos descritos en los cortes anteriores son históricos. Se mantiene el alcance documental, sin autorizar ejecución del producto, cambios legacy, SQL o nuevos E2E.
