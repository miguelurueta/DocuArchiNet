# DOC-110 — Diseño del perfil de importación

## Context

Estado de cierre, 2026-10-27: implementación y checklist completos (178/178). La revisión final contrasta 17 requisitos y 87 escenarios con código y evidencia; ver [informe final](../../../../Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc110-verificacion-final-20261027.md). Las referencias inferiores a implementación propuesta, tareas pendientes o falta de autorización describen la fase original de diseño y quedan superadas por este estado y las aceptaciones fechadas del checklist.

Arquitectura materializada: página WebForms → cliente JavaScript → fachada ASMX con contexto/guard → servicios → repositorios RA y catálogos RA/DA. Fuentes, columnas e historial se leen de RA por petición, sin fallback de autorización a archivo; el ajuste servidor Doc110.Fuentes.TablasAdicionales solo limita nombres admisibles, no concede una fuente. Proveedores operativos se crean/configuran/publican desde UI. El registro SQL sintético inactivo de 15.19 es exclusivamente un fixture de regresión de configuración antigua, no un mecanismo funcional de alta.

Condiciones originales A1/A3 comprobadas mediante implementación y pruebas; A2 mantiene permisos separados. A4 resuelta para las corridas y datos de prueba autorizados, sin autorización implícita de producción. La revocación se aceptó con cambio directo puntual y restauración autorizados, porque la aplicación externa queda fuera del proyecto. La tarea 7.6 se cerró por aceptación explícita sin PASS para combinaciones no ejecutadas; SII conserva evidencia estática y de contrato, sin afirmar una importación SII real. La continuidad y los reintentos de lotes siguen siendo contrato para la entrega operativa futura.

Alcance operativo confirmado posteriormente por el usuario: esta instalación tiene y siempre tendrá una sola empresa. La aceptación real entre empresas es NO_APLICA; no se preparan cuentas ni empresas adicionales para ese escenario. Se conservan las defensas de contexto servidor, permisos, recursos, sesión, CSRF y revisión. No cambia el ámbito de integridad EXPEDIENTE/LOTE ni la política de fuentes. Decisión y tareas afectadas: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Exploracion/criterio-empresa-unica-doc110.md`.

Estado: refinamiento aprobado explícitamente por el usuario el 2026-10-20; implementación autorizada posteriormente en el flujo OPSXJ. Este documento desarrolla D-01–D-07 y RQ-01–RQ-07. La fuente Jira original permanece en `specs/produccion-importacion-carpeta-perfil/jira-context.md`; el prompt 02 y el diseño común están en `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/`. Evidencia de implementación parcial: `Validacion/doc110-implementacion-base.md` bajo esa raíz documental. Las tareas abiertas no se consideran implementadas por aprobación del diseño.

Stack observado: WebForms/VB.NET 4.6.1, JavaScript, ASMX y MySQL 5.1. No se crean capas MVC/Core ni una plataforma de conectores. UI → ASMX → servicio → repositorio/adaptador de catálogos → respuesta. Los servicios reciben contexto explícito; no leen controles de pantalla ni reciben autoridad del cliente.

## Goals / Non-Goals

Administrar perfiles: crear cabecera/borrador, editar configuración y reglas, probar nombres/metadatos, publicar versiones inmutables, consultar historial y seleccionar una versión compatible. Desactivar conserva historial y evita nuevas selecciones; no cambia versiones publicadas.

Fuera del ticket: leer carpetas/bytes, transferir archivos, preparar planes/lotes, crear expedientes, almacenar/incorporar documentos, ejecutar coordinador/worker. No implementar ASHX de recepción ni tablas operativas. El nivel viene del árbol existente; no crear otro árbol ni vincular el perfil a un nivel que sustituya el contexto de apertura.

## Evidencia de código

Corrección de topología confirmada por el usuario: [orígenes de datos](../../../../Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/08-origenes-datos.md). El sistema es multibase; RA para tablas propias/empresa/usuario no implica RA para cada catálogo. `Class_system1` tiene consumidores RA y DA. No cerrar identidad de gabinete/índices ni SQL definitivo por igualdad de ID/nombre entre bases. Esta precisión limita las afirmaciones anteriores de origen común.

| Ruta/símbolo | Uso permitido y límite |
|---|---|
| `webservice/WebServiceProducion.asmx.vb`: `ServiceSolicitaCargarDocumentoExpediente`, `ServiceSolicitaListaTipologiasExpediente` | Patrón ASMX/ScriptService con sesión. No reutilizar selección mutable de expediente para administrar perfiles. |
| `Gestion/ClassGaProducionDocumental.vb`: `SolicitaCargarDocumentoExpediente` | Lee `Session("GA_IDUSUARIOGESTION")`, valida usuario positivo, propiedad de nivel y permiso `carga_archivo`. No demuestra autorización administrativa del perfil. |
| `Class_ra_pro_niveles.Solicita_estado_nivel_propietario` y `Class_ra_pro_permisos_niveles.Solicita_datos_estrctura_permiso_nivel_usuario_gestion` | Referencias para verificación del nivel; no traducir propiedad/carga en permiso de publicar perfiles. |
| `Infrastructure/Shared/Data/ModuleSessionConnectionStringResolver.vb`: `Resolve` | Candidato de conexión desde configuración de módulo en sesión; no representa identidad/ámbito de autorización ni debe exponer cadena de conexión. |
| `Services/Workflow/Notas/ServicioNotasWorkflow.vb` | Referencia de servicios con contexto explícito y repositorio; no reutilizar tareas Workflow para el perfil. |
| `tools/e2e/package.json`, `playwright.config.cjs`, `tests/support/authenticated-workflow-session.cjs` | Infraestructura existente. Extensión compatible para Producción solo si se confirma necesaria. |

La revisión inicial fue estática; posteriormente se inspeccionaron metadatos y conteos mediante SELECT, como registra diagnostico-dominio-empresa.md. No se publicaron secretos. La revisión ampliada se registra en [evidencia de autorización y solicitudes](evidencia-autorizacion-solicitudes.md). Existe infraestructura administrativa: `Class_remit_dest_interno_perfil_produccion.AsignaPermisosPerfilUsuarioGestion` carga `GA_MANAGER_CONFIGURACION` y otras capacidades desde el perfil; `ClassGestorSesion` la invoca al construir sesión. `ClassGagestorInicio` y `WebFormGatoponimica` muestran consumidores de permisos en presentación y servidor. Se reutiliza esta infraestructura, sin roles o login paralelos.

La búsqueda ampliada, incluida sobre archivos versionados, no identificó un mecanismo CSRF explícito reutilizable para el nuevo ASMX. No demuestra vulnerabilidad ni ausencia absoluta de protección en el despliegue; falta validación HTTP autorizada. La cabecera CORS global observada no se toma como protección CSRF ni se cambia en este ticket.

## Decisions

### D-01 — Perfil independiente y no regresión (RQ-01)

No invocar creación de expediente, almacenamiento o recepción al editar/ensayar/publicar. SII, autorregistro, intenciones Workflow, carga individual y relaciones compartidas de gabinete permanecen intactos. Esquema propio ausente/incompatible produce disponibilidad controlada solo para perfiles; no falla la carga de Producción. No ejecutar DDL al iniciar.

La superficie administrativa debe integrarse en Producción mediante controles propios, respetando UpdatePanel, ClientID, foco, inicialización/destrucción y CSS acotado. No crear la ventana de importación completa del prompt 05. Cambiar perfil conserva contexto del nivel. La UI consume capacidades del servidor; ocultar acciones no es autorización.

### D-02 — Versiones y publicación (RQ-02)

Decisión aprobada 2026-10-20: [contrato de disponibilidad y coincidencias](../../../../Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/06-coincidencias-disponibilidad.md). Desactivar impide nuevas selecciones/confirmaciones; lotes ya confirmados continúan con snapshot y permisos vigentes. Detención explícita o pérdida de autorización se resuelven en el coordinador. Confirmación/desactivación concurrentes requieren orden transaccional; DOC-110 no ejecuta lotes.

Cabecera mutable: identidad, código por ámbito, nombre, descripción, disponibilidad y referencia publicada. Versiones: BORRADOR o PUBLICADA, revisión, configuración y reglas propias. Un borrador puede estar incompleto pero nunca contener referencias/estructuras malformadas. PUBLICADA es inmutable; modificar configuración exige nueva versión.

Guardar borrador exige revisión esperada y pertenencia perfil/ámbito; actualizar mediante comparación de revisión, rechazando cero filas afectadas como conflicto. Publicar valida configuración completa, bloquea/revalida cabecera y versión y confirma estado, huella y referencia publicada en una sola transacción. Una publicación con revisión obsoleta no se reintenta silenciosamente. Nueva versión asigna número con exclusión por perfil y restricción única; no usar MAX+1 sin bloqueo.

Tras respuesta perdida, consultar perfil/versión para conocer resultado antes de reenviar mutación. Registrar actor/UTC desde servidor. Desactivar afecta disponibilidad de nuevas selecciones, no borra historial ni modifica configuración publicada. Los lotes futuros quedan fuera de este ticket y deberán respetar su propio snapshot y permisos.

### D-03 — Configuración y reglas declarativas (RQ-03)

Política de coincidencias de expediente aprobada: BLOQUEAR por defecto o REUTILIZAR_VALIDADO; persistir en configuración versionada y posteriormente snapshot. El contrato común 06 establece condiciones acumulativas y pruebas. Configurar una opción no acredita capacidad operativa: el adaptador 08 caracteriza identidad/comparación y no reutiliza por nombre. Reutilización no omite archivos ni modifica datos del expediente para hacerlo coincidir.

Contrato documental de tarea 1.3: [fuentes de expediente](../../../../Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/05-contrato-fuentes-expediente.md). Define matriz de código/fechas/responsable, combinaciones admitidas, tipos, etapas y pruebas. Es definición técnica para revisión, no aprobación automática. Coincidencias está definida en contrato aprobado 06; tarea documental 1.9 completada. Para entradas de creación solo se habilitan fuentes que pueden resolverse antes de crear; no heredar el ID futuro como entrada. El límite inicial de código de expediente es 45 por compatibilidad conservadora y no se confunde con los 64 del código de perfil.

Reutilizar catálogos de clasificación, responsables, gabinete, índices y tipologías mediante consultas autorizadas. Guardar referencias, no duplicar catálogos ni escribir relaciones globales. Verificar pertenencia y compatibilidad al publicar y seleccionar; código de tipología no equivale automáticamente a su ID interno.

Una asignación final por destino: CAMPO_EXPEDIENTE, INDICE_GABINETE o TIPOLOGIA. Fuentes CONSTANTE, CARPETA, ARCHIVO o EXPEDIENTE; etapas PREFLIGHT/TRAS_EXPEDIENTE. Las alternativas, si se implementan, tienen precedencia dentro de una sola regla. Rechazar destinos repetidos, SQL libre, scripts y expresiones ejecutables. No fijar el formato `1246566-125-OrdenCompra`.

Resolvedor puro recibe configuración y metadatos sintéticos; devuelve valores, incidencias por campo y fuentes diferidas tipadas. No lee File ni disco. Validar obligatorio/tipo/longitud/coherencia, ceros iniciales y representación de caracteres. Campos dependientes de IDs futuros permanecen diferidos: validar su fuente sin inventar valor ni crear expediente para obtenerlo. Política de código/fecha/responsable/coincidencias pertenece a configuración explícita; no publicar con valores globales inventados para eludir una definición faltante.

### D-04 — Integridad configurada, no ejecutada (RQ-04)

COMPLETO_OBLIGATORIO por defecto requiere elegir EXPEDIENTE o LOTE antes de publicar. Borrador puede conservar ámbito pendiente; publicación no. PARCIAL_PERMITIDO explícito usa NO_APLICA. La elección queda en la versión publicada y las entregas posteriores la copiarán al plan/lote. DOC-110 no implementa barreras, contadores ni rollback documental.

### D-05 — Autoridad, contexto y transporte (RQ-05)

Contrato técnico para 1.5/1.6: [HTTP y seguridad](../../../../Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/07-contrato-http-seguridad-perfil.md). Especifica métodos/DTO, revisiones de cabecera/versión, propósito administrativo frente a futura importación y guard local con token ligado a sesión. A3 queda definido en diseño; implementación y pruebas HTTP siguen abiertas. No atribuir al código legacy un validador que no se identificó.

Actualización A1: [origen del dominio de empresa](diagnostico-dominio-empresa.md). Resuelto en diseño: persistencia del perfil en la misma base documental RA que usuario/empresa y estructura. Usar `Resolve(..., "RA_")`, no conexión sin prefijo. `GA_IDEMPRESA` es empresa del usuario; `RA_ID_EMPRESA` es empresa del catálogo de módulos y no la sustituye. Las menciones anteriores a verificar namespace quedan cubiertas por este contrato y su comprobación futura de despliegue.

Contrato detallado: [ámbito y permiso](contrato-ambito-permiso.md). Concreta empresa positiva, mapeo físico propuesto y permiso vigente por solicitud. La asignación corresponde a otra aplicación fuera del repo, confirmada por el usuario; contrato en `contrato-administracion-externa.md`, aceptación conjunta aún sin ejecutar.

Usar usuario autenticado servidor; `GA_IDUSUARIOGESTION` es evidencia de identidad usada en Producción, no autoridad suficiente por sí sola. Ámbito estable de perfil requiere mapeo a entidad de negocio autorizada, no nombre de BD/cadena de conexión ni dato cliente. El mapeo de ámbito se resolvió en A1 mediante el origen documental RA; la política administrativa A2 se resolvió por aprobación explícita de permisos separados.

| Operación conceptual | Control obligatorio |
|---|---|
| Listar/obtener/historial | Identidad, ámbito y lectura del recurso. |
| Crear/guardar/nueva versión | Edición administrativa en ámbito; revisión y estado cuando aplique. |
| Validar/ensayar | Lectura autorizada de configuración/catálogos; sin persistencia documental. |
| Publicar | Publicación administrativa, revisión y validación completa actual. |
| Activar/desactivar | Disponibilidad administrativa y revisión de cabecera. |
| Seleccionar | Versión publicada disponible, lectura y compatibilidad con nivel autorizado. No concede creación/carga. |

## Decisión aprobada: permisos separados

El usuario aprobó el 2026-10-20 dos capacidades funcionales específicas sobre la infraestructura existente de perfiles/permisos:

| Capacidad lógica | Alcance | Entrega propietaria |
|---|---|---|
| `ADMINISTRAR_PERFILES_IMPORTACION` | Crear/editar borradores, ensayar configuración administrativa, publicar versiones y activar/desactivar perfiles autorizados. | DOC-110; incluye lectura administrativa de recursos autorizados. |
| `IMPORTAR_CARPETAS` | Abrir importación, seleccionar perfil publicado y ejecutar carga; consultar, cancelar o recuperar lotes según propietario/ámbito y estado. | Entrega posterior de carga; DOC-110 solo define el contrato de separación. |

Los nombres son identificadores lógicos aprobados; columna/clave de sesión se concretan según convenciones existentes, sin crear otro sistema de roles. Ambos parten deshabilitados (0), sin concesión automática a usuarios actuales ni herencia de `GA_MANAGER_CONFIGURACION` o `GA_MANAGER_GESTION`. Ausencia, nulo o valor inválido no concede acceso.

La asignación usa la administración existente, con permisos de asignación vigentes; nadie se otorga la capacidad desde el editor de perfiles. Cada operación valida en servidor identidad, capacidad, ámbito y recurso. El botón refleja esa decisión, no la reemplaza. Importar no autoriza administrar; administrar no concede importar. Nivel, gabinete, creación y carga siguen exigiendo sus permisos específicos cuando corresponda. Consulta/recuperación de lotes no añade permisos nuevos inicialmente y conserva validación de propietario/ámbito/estado.

DOC-110 entrega columna administrativa, carga/consulta/autorización local y contrato de asignación para la aplicación externa existente; no implementa su escritor. No implementa aún el permiso operativo ni operaciones de lotes; la futura selección para importar debe exigir IMPORTAR_CARPETAS, mientras la consulta/ensayo administrativo usa ADMINISTRAR_PERFILES_IMPORTACION. Las operaciones compartidas distinguen propósito para no conceder autorización por una condición OR indiscriminada.

Cualquier cambio de esquema de permisos se entrega en SQL MySQL 5.1 para aplicación manual, con default 0, pre/postcontroles y sin asignaciones masivas. Se permite extender el esquema existente de permisos solo lo necesario para estas capacidades; sigue prohibido modificar SII o catálogos documentales compartidos. Esta aprobación define política, no acredita columnas, asignaciones, código o SQL ya creados.

Toda mutación exige protección CSRF verificada y contexto servidor. Sesión habilitada, SameSite o esconder botones no bastan por sí solos. A3 es un requisito de implementación/verificación anterior a habilitar mutaciones, no prueba de un fallo existente ni impedimento para avanzar contratos y reglas puras. Reutilizar protección compatible si se identifica; si no hay una aplicable, concretar una extensión acotada al ASMX nuevo, sin tocar SII/login/configuración global.

Sobre de respuesta alineado con diseño común: `ok`, `codigo`, `mensaje`, `operacionId`, `datos`, `incidencias`. ASMX retorna DTO, no JSON doblemente serializado; cliente desempaqueta `d` cuando corresponda. IDs como cadenas canónicas, revisión entera, fechas UTC ISO 8601. Incidencias identifican campo/regla y motivo seguro. No aceptar nombres SQL, esquema de conexión o paths físicos.

| Operación | Entrada mínima conceptual | Salida/efecto |
|---|---|---|
| Crear | Código, nombre y descripción permitidos | Perfil/borrador y revisiones persistidos. |
| Obtener/listar | Referencias o filtros/paginación acotados | Datos autorizados y capacidades; sin mutar. |
| Guardar | Perfil/versión/revisión y configuración/reglas de borrador | Nueva revisión o conflicto; no publica. |
| Ensayar | Referencia/configuración permitida y metadatos de ejemplo | Valores/diferidos/incidencias; cero efectos documentales. |
| Publicar | Perfil/versión y revisión | Versión publicada y referencia habitual, o rechazo completo. |
| Nueva versión | Perfil y versión origen autorizada, revisión según contrato | Borrador nuevo; origen intacto. |
| Disponibilidad | Perfil/revisión y estado solicitado | Disponibilidad revisada; historial conservado. |
| Seleccionar | Nivel explícito y referencia de versión | Configuración compatible; mismo nivel, sin plan/lote. |

Distinguir solicitud malformada, sin sesión, prohibido, conflicto, configuración inválida y esquema no disponible. Probar mapeo de 400/401/403/409/503 bajo ASMX/IIS; no mostrar HTML/stack/SQL. UI finaliza promesas ante timeout/expiración y consulta resultado tras pérdida de respuesta. Autorización precede consultas que revelen existencia ajena.

### D-06 — Persistencia MySQL 5.1 (RQ-06)

Entregable documental preparado: [SQL 01](../../../../Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Sql/01-perfil-importacion-mysql51.sql) y [procedimiento manual/diccionario físico](../../../../Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Sql/01-aplicacion-perfil-importacion.md). Tres tablas propias y extensión administrativa, sin aplicación a BD. Estados/políticas se validan en servicio; el DDL no resuelve por sí mismo las decisiones operativas de coincidencias o desactivación.

Adoptar sección 2 del modelo común como diccionario base de `ra_integracion_perfil`, `ra_integracion_perfil_version`, `ra_integracion_perfil_regla`. No copiar sus seis entidades operativas. PK INT UNSIGNED AUTO_INCREMENT y referencias INT UNSIGNED (version_publicada_id nullable), transporte decimal canónico String, revisiones INT UNSIGNED, fechas DATETIME UTC, estado VARCHAR cerrado y huella BINARY(32) SHA-256 desde aplicación. Configuración/reglas LONGTEXT ASCII con JSON escapado y esquema cerrado, límites de tamaño/profundidad y rechazo de propiedades/claves inválidas. Textos visibles UTF8 de tres bytes, sin sustitución silenciosa.

Unicidad: ámbito/código; perfil/número de versión; versión/tipo/referencia destino. FK propias compatibles con RESTRICT; referencias legacy lógicas verificadas sin alterar índices o tablas ajenas. Referencia publicada debe pertenecer al perfil. Gabinete/índices respetan INT UNSIGNED observado; verificar esquema del ambiente antes de cerrar DDL, sin afirmar despliegue.

Entregar `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Sql/01-perfil-importacion-mysql51.sql`. Pre/postcomprobaciones SELECT, orden, recuperación parcial y detección de estructura incompatible. Sin USE de BD operativa, secretos, borrados, JSON nativo, utf8mb4, columnas generadas o CHECK como garantía. DDL con commits implícitos: no prometer rollback global. El equipo aplica manualmente; verificar estructura física antes de emitir DDL; A1 ya define el origen RA.

### D-07 — Pruebas por capacidad (RQ-07)

Pruebas unitarias de reglas, conversiones, duplicación de destino, diferidos y modos; integración de concurrencia/publicación/ámbitos/esquema ausente; MSBuild real y sintaxis JavaScript. DOC-88 complementa regresión estática, no acredita persistencia ni E2E.

Reutilizar `tools/e2e`: autenticación, configuración, runners, fixtures, reporteros y evidencia. No crear proyecto/login/.env paralelos ni modificar globalmente workers/timeouts para pasar. Si soporte de Producción falta, extensión compatible con regresión de consumidores; nunca tareas Workflow ficticias.

- PER-01: crear, rechazar completo sin ámbito, publicar EXPEDIENTE y recargar versión/reglas.
- PER-02: publicar otra versión LOTE, comprobar anterior intacta y rechazar cuenta restringida.
- PER-03: guardar/recargar dos patrones y rechazar requerido/tipología inválidos.

Antes de autenticar: AGENTS/runbook y autorización específica de ambiente/cuentas/datos descartables. Escrituras funcionales solo en datos autorizados; controles SELECT y secretos efímeros. Registrar SHA, comandos, UI/HTTP/persistencia y evidencia saneada. NO_EJECUTADO por dependencia/autorización nunca es PASS. No se ejecutaron estas pruebas al redactar el diseño.

## Inventario propuesto de archivos exactos

Complemento de integración y pruebas: [inventario de implementación](inventario-implementacion.md). Identifica el arnés existente vbc/csc/MSBuild + node:test y fija rutas propuestas de pruebas; no se crea un framework nuevo. La superficie de asignación es externa por confirmación del usuario; contrato y aceptación conjunta documentados, sin archivo productivo local inventado.

Todos los nombres nuevos son propuestas para DOC-110, no archivos implementados. No cambiar rutas SII.

| Archivo | Responsabilidad |
|---|---|
| `Modelo/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionModels.vb` | Identidad, versión, regla, resultado tipado. |
| `Modelo/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionInterfaces.vb` | Puertos de repositorio, contexto/autorización y catálogos. |
| `DTOs/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionDtos.vb` | Contratos HTTP del perfil. |
| `Services/ProduccionDocumental/ImportacionCarpetas/ServicioPerfilImportacion.vb` | Casos de uso, autorización, publicación y selección. |
| `Services/ProduccionDocumental/ImportacionCarpetas/ValidadorPerfilImportacion.vb` | Validación de configuración y catálogos. |
| `Services/ProduccionDocumental/ImportacionCarpetas/ResolvedorReglasPerfilImportacion.vb` | Ensayo puro y fuentes diferidas. |
| `Services/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionJson.vb` | Serialización canónica ASCII, esquema cerrado y huella sobre configuración/reglas; compartida por servicio/repositorio. |
| `Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlPerfilImportacionRepository.vb` | Persistencia parametrizada/transacciones propias. |
| `Infrastructure/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionCatalogAdapter.vb` | Consultas legacy compatibles sin duplicar catálogos. |
| `Infrastructure/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionContextAdapter.vb` | Contexto servidor y capacidades específicas aprobadas; ámbito de empresa documental en origen RA según A1. |
| `Infrastructure/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionRequestGuard.vb` | Validación de transporte/sesión/token acotada a la nueva fachada y emisión desde página autorizada; contrato común 07. |
| `webservice/WebServiceImportacionCarpetas.asmx` y `.asmx.vb` | Fachada de perfil; sin recepción/ejecución. |
| `Gestion/WebFormProducionDocumental.aspx` y `.aspx.vb` | Punto de acceso/controles según autorización; conservar carga individual. |
| `js/gestion/WebFormProducionDocumental.js` | Entrada al administrador/selección, no otro árbol. |
| `js/gestion/ImportacionCarpetas/PerfilImportacionApi.js` | Transporte y sobre ASMX. |
| `js/gestion/ImportacionCarpetas/PerfilImportacionEditor.js` | Estado, formulario, reglas e historial. |
| `tools/e2e/tests/doc110-perfil-importacion.spec.cjs` | PER-01–03 en infraestructura existente. |
| `tools/e2e/package.json` | Comando focal, delegando en infraestructura existente. |
| `GestionDocumental-Docuarchi.net.vbproj` | Compile/Content de archivos productivos; no fixtures/secretos. |

Las rutas de pruebas de componentes VB mediante runner C# y node:test están fijadas en el inventario complementario según arneses observados. El guard CSRF está inventariado y definido en el contrato común 07; implementación y pruebas no modifican SII.

## Risks / Trade-offs

La configuración flexible exige esquema cerrado y límites; LONGTEXT no sustituye validación. Publicación válida hoy no garantiza permiso/catálogo vigente mañana: revalidar selección. Un token opaco no autoriza recurso. Control de revisión y unicidad protege concurrencia propia, no crea una transacción documental global. El diseño de importación completo sigue en los prompts posteriores.

## Migration Plan

Aplicar los contratos A1/A3 e implementar el mapeo aprobado de A2; revisar trazabilidad; entregar SQL 01 manual y repositorios; implementar reglas/servicios/transporte/UI; comprobar MSBuild y regresión; ejecutar PER autorizados. Registrar esquema aplicado por equipo, no asumirlo. Si el esquema falta, rechazar solo perfiles. Reversión operativa retira nueva capacidad preservando versiones/datos; no DROP de historial ni cambio de permisos ajenos automático.

## Open Questions / condiciones de aprobación

Investigación específica actualizada: [GA_MANAGER_CONFIGURACION](diagnostico-ga-manager-configuracion.md). Solo se localizaron inicialización y carga, no consumidores de autorización; asignarlo al perfil nuevo sería una política nueva sobre un campo existente. `GA_IDEMPRESA` sí tiene una procedencia de negocio identificada (`ClassAdmonEmpresa.Retorna_id_empresa_usuario_gestion`); validar empresa positiva en el origen RA definido. A1 está resuelto en diseño y requiere contexto válido; A2 se resolvió por decisión explícita del usuario, no por el nombre de una clave existente.

| ID | Pendiente concreto | Qué bloquea | Evidencia actual |
|---|---|---|---|
| A1 — RESUELTO EN DISEÑO | Perfil en origen RA; ámbito empresa documental positiva del usuario en ese origen. | Verificar selección de conexión y despliegue al implementar. | Rastreo completo y SELECT saneados en diagnostico-dominio-empresa.md; no confundir empresa de módulo con empresa del usuario. |
| A2 — RESUELTO | Permisos específicos ADMINISTRAR_PERFILES_IMPORTACION e IMPORTAR_CARPETAS, separados y deshabilitados por defecto. | Implementación/asignación mediante infraestructura existente; DOC-110 solo capacidad administrativa. | Aprobación explícita del usuario el 2026-10-20; no reutilizar GA_MANAGER_CONFIGURACION. |
| A3 — DEFINIDO EN DISEÑO | Guard local de sesión/token y contrato HTTP del documento común 07. | Implementación y pruebas antes de habilitar mutaciones. | Sin validador legacy aplicable identificado; propuesta técnica documentada, no protección desplegada. |
| A4 | Confirmar ambiente, cuentas y aplicación manual de SQL. | Integración/E2E reales. | Sin autorización nueva en esta entrega. |

No sustituir el contrato A1 ni resolver A3 mediante valores inventados ni marcar approved para satisfacer la herramienta. Fuentes de negocio incompletas bloquean publicación del perfil concreto; no justifican crear expedientes para completarlo.

Confirmación de alcance: el usuario indicó que los permisos se administran fuera del repo. Ver [contrato externo](contrato-administracion-externa.md). La dependencia condiciona habilitación y validación integrada, no exige otro sistema de permisos ni bloquea preparación/implementación local.

COR-03/RQ-09: extender ListarPerfiles con termino opcional hasta 160 caracteres, búsqueda literal codigo OR nombre mediante LOCATE parametrizado bajo ambito_ref; mantener filtros exactos y paginación. Reutilizar autocomplete 1.8.22, API/guard y permisos. Menú acotado, espera 300 ms, descarte de respuestas antiguas y destrucción al reconstruir/cerrar. Sin DDL. Contrato en Diseno/02-contratos-operacion.md.

COR-04: detalle guiado con catálogos por campo y época de vista, referencia puntual opcional sobre ConsultarCatalogo existente y etiquetas autorizadas RA/DA; sin DDL. Protección de cambios por comparación de payload, publicación solo guardada y recuperación contextual. Desglose 12.7–12.17 y evidencia Validacion/doc110-cor04-detalle-guiado.md.

## D-08 — AMP-01: proveedores de datos de índices

Extensión solicitada por el usuario, separada de las cuatro fuentes existentes. Contrato: [proveedores de datos para índices](../../../../Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/09-proveedores-datos-indices.md). Primera entrega: contrato extensible y adaptador común de tablas/vistas configurables por proveedor; API concreta posterior. Resolvedor puro, consulta coordinada en servicio, autorización por ámbito, versión concreta, clave configurable y mapeo de propiedad a índice; no modificar SII. Ausencia/ambigüedad/error no se convierten a 0. Preservar huellas publicadas y esquema cerrado. 14.2 define selección y validación de metadatos del origen configurable. Una tabla concreta solo condiciona la aceptación integrada 14.17; no bloquea el mecanismo genérico ni su SQL de configuración. No fijar tablas en código ni admitir SQL libre. Trazabilidad: RQ-11 y tareas 14.1–14.19.
AMP-01: decisiones concretas 14.2/14.3 en Diseno/10-catalogo-proveedores-tabla.md bajo la raíz funcional. Administración de proveedores desde interfaz con borradores y publicación (corrección del usuario 2026-10-22), conexiones lógicas resueltas en servidor, filtro de empresa obligatorio (vista administrada si hace falta), adaptador MySQL común con metadatos previos y LIMIT 2. SQL 04 crea catálogo RA y versiones, sin aplicar; implementación de componentes aún no conectada al servicio/UI.

D-08 ampliada: el contrato 10 sustituye el registro técnico por scripts. D-09/RQ-12 definen el ciclo administrativo de proveedores dentro de DOC-110. SQL 04 instalado no basta para estado/revisión/publicación: diseñar migración incremental y filtros PUBLICADA en todos los consumidores. Autorización de descubrimiento/configuración pendiente de definición explícita, sin conceder acceso arbitrario por el permiso del perfil. Tareas 14.20–14.29 pendientes; no se acredita implementación por esta corrección documental.

D-09 complemento: fuente exclusiva por conexion autorizada mediante ExclusivaEmpresa=true en politica servidor por ambito/conexion/tabla. Permite empresa_columna vacia sin migracion. Default false conserva filtro anterior. Ver contrato 10, seccion AMP-01 fuente exclusiva de empresa.


D-09 / AMP-02 (2026-10-23): contrato vigente de migracion en Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/11-administracion-fuentes-da.md. Fuentes, columnas e historial en RA, permiso ADMINISTRAR_FUENTES_IMPORTACION, descubrimiento inicial bajo esa facultad y conexion de sesion, historial atomico y politica por peticion sin fallback a archivo. No modificar contratos publicados ni tablas DA. Complementa RQ-12; reemplaza la persistencia Web.config cuando se implemente 15.9, no antes. SQL 07 preparado para aplicacion manual; runtime sin cambio en esta entrega documental.

D-09 restriccion de seguridad (2026-10-23): contrato 12-restriccion-tablas-fuentes.md limita candidatos y fuentes consultables al prefijo literal imp_02; excepciones exactas por alias solo en Doc110.Fuentes.TablasAdicionales del servidor. No sustituye autorizacion funcional RA, ni permite registrar excepciones desde proveedor/fuente. Reemplaza el alcance amplio de descubrimiento inicial definido en contrato 11.

D-09 presentacion administrativa (2026-10-23): aplicar composicion de cabecera/cuerpo/tablas de SII moderno y tarjetas/contexto del prototipo de carpetas, con CSS exclusivo de DOC-110. Fuentes y proveedores mantienen facultades y servicios separados. Matriz de decisiones en Exploracion/13-revision-interfaz-administracion.md. Sin extender esta entrega a importacion real ni modificar SII.

D-03 ampliacion 2026-10-24: herencia explicita de SERIE_ID/CODIGO/NOMBRE y SUBSERIE_ID/CODIGO/NOMBRE, con Codigo_Arbitrario textual. Contrato vigente: Diseno/14-herencia-clasificacion.md bajo la raiz funcional. Ensayo autorizado proyecta clasificacion del perfil y la identifica como propuesta; solo el consumidor operativo futuro confirma datos del expediente. Sin DDL ni cambios a publicaciones.
