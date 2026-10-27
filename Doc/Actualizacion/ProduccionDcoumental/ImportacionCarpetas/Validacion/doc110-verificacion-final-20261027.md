# DOC-110: verificación final del cambio

Complemento OPSXJ posterior: la revisión de gobierno detectó documentos técnicos obligatorios todavía en plantilla y referencias de trazabilidad incompletas para el validador. Se completaron y normalizaron sin cambios funcionales; `opsxj:validate` PASS, 69 controles, 2026-10-27T20:55:31.345Z. [Correcciones, registros y estado Git/PR](doc110-cierre-opsxj-20261027.md). La conclusión funcional anterior no equivalía a una entrega completa en Git: la implementación permanece sin commit y no hay PR DOC-110.

Actualización posterior a la revisión: el usuario autorizó sincronizar y archivar. El 2026-10-27 se creó la especificación principal `openspec/specs/produccion-importacion-carpeta-perfil/spec.md`, con 17 requisitos y 87 escenarios idénticos a los del delta y validación estricta PASS. El cambio completo se movió a `openspec/changes/archive/2026-10-27-doc-110-produccion-importacion-carpeta-perfil/`, con sus artefactos y gobernanza; se ajustaron enlaces relativos del diseño y la ruta del refinamiento. Ya no figura como cambio activo. No se realizó commit, push, publicación Jira ni despliegue. Las afirmaciones inferiores sobre archivo no ejecutado corresponden al momento anterior de la revisión.

Fecha: 2026-10-27. Cambio: `doc-110-produccion-importacion-carpeta-perfil`, esquema `spec-driven`. Revisión con `openspec-verify-change`, sobre el árbol de trabajo actual y evidencias fechadas. No se archivó, publicó ni creó commit durante esta verificación.

## Resultado

| Dimensión | Resultado |
|---|---|
| Completitud | 178/178 tareas, cero pendientes; 17 bloques Requirement y 87 Scenario inventariados |
| Corrección | Se identificó implementación, contrato o decisión de alcance para los 17 requisitos; pruebas y evidencia agrupadas abajo |
| Coherencia | Capas WebForms/ASMX/servicio/repositorio, SQL manual y autorización servidor conservados; estado documental actualizado |
| Incidencias críticas | Ninguna identificada en el alcance de esta revisión |
| Advertencias nuevas | Ninguna; límites y excepciones de aceptación conservados expresamente |

La cuenta de tareas no significa que todos los escenarios hayan tenido una nueva ejecución real. La revisión distingue pruebas locales, corridas previas autenticadas, aceptación expresa, NO_APLICA y contratos de la entrega operativa futura.

## Requisitos, implementación y cobertura

Las rutas de código son relativas a la raíz del repositorio. Los nombres de documentos de aceptación son relativos a esta carpeta Validacion.

| Requisito | Implementación o decisión comprobada | Verificación / evidencia |
|---|---|---|
| Empresa única | Decisión explícita del usuario en Exploracion/criterio-empresa-unica-doc110.md; controles servidor conservados | E2E entre empresas NO_APLICA; no se convierte en PASS |
| RQ-03, herencia de clasificación | ServicioPerfilImportacion.Ensayar, PerfilImportacionCatalogAdapter.ClasificacionPropuesta y ResolvedorReglasPerfilImportacion | Catálogo autorizado sustituye propuesta cliente, código textual, ausencia/diferido; arnés .NET y corrida --inheritance en doc110-continuacion-20261024.md |
| RQ-01, aislamiento/compatibilidad | Nueva fachada webservice/WebServiceImportacionCarpetas.asmx.vb, servicios propios y entrada acotada en Producción | Ensayo sin efectos, UI/UpdatePanel y carga individual; doc110-sin-esquema-ambiente-pruebas.md y doc110-regresion-carga-individual-preparacion.md |
| RQ-02, versiones/concurrencia | MySqlPerfilImportacionRepository.Publicar, CrearVersion y Transaccion; revisión, FOR UPDATE, huella y cabecera en una transacción | Servicio/codec .NET y carreras autenticadas documentadas en doc110-continuacion-20261024.md; cliente no reenvía mutaciones inciertas |
| RQ-03, reglas/catálogos | ValidadorPerfilImportacion, ResolvedorReglasPerfilImportacion y PerfilImportacionCatalogAdapter | Dos patrones, fechas, código con ceros, destinos únicos, tipo/longitud, responsable y diferidos; arnés .NET, --matching y --acceptance previos |
| RQ-04, integridad | Modelo/configuración, ValidadorPerfilImportacion y editor | Completo exige EXPEDIENTE/LOTE explícito; parcial NO_APLICA; publicación real de ambos ámbitos en doc110-per01-03-consolidacion-preparacion.md |
| RQ-05, autoridad/selección | PerfilImportacionContextAdapter, PerfilImportacionRequestGuard, fachada Ejecutar y ServicioPerfilImportacion.Seleccionar | Sesión, token de otro contexto, permiso vigente, propósito administrativo, autorización RA y nivel; expiración/revocación reales y guard local. Ver documentos de sesión/revocación |
| RQ-06, SQL manual | Sql/01, 03, 04, 05 y 07 con procedimientos de aplicación y recuperación; sin migración automática en runtime | MySQL 5.1, InnoDB, publicación preservada y esquema parcial. SQL 01 contiene solo DDL por instrucción posterior del usuario; precondiciones/controles están en 01-aplicacion-perfil-importacion.md |
| RQ-07, evidencia/E2E | tools/e2e con login compartido, runners, safePost y captura de huellas | PER-01–03 en doc110-per01-03-consolidacion-preparacion.md; comandos/resultados reales separados de simulación; no nuevo login o proyecto Playwright |
| RQ-08, identidades | LastInsertedId en misma conexión/transacción, reglas serializadas con ID incremental y transporte decimal canónico | Arnés de migración/codec, doc110-identidades-select-20261027.md, doc110-correccion-incrementales.md y doc110-instalacion-nueva-aislada-20261027.md |
| RQ-09, autocomplete | Listar parametrizado por ámbito/LOCATE y PerfilImportacionEditor.js con widget existente | Node y Chromium: teclado, código/nombre, respuesta atrasada, limpiar, foco/reapertura; corrida real más reciente 18.2 s |
| RQ-10, detalle guiado | PerfilImportacionEditor.js, PerfilImportacionApi.js y catálogos autorizados | Dependencias, etiquetas fuera de página, publicado, controles aplicables y navegación con edición pendiente; Node/Chromium y doc110-cor04-detalle-guiado.md |
| RQ-11, proveedores versionados | CoordinadorProveedorIndices, ReglasProveedorIndices, MySqlProveedorDatosIndices y MySqlProveedorIndicesRepository | Consulta parametrizada LIMIT 2, cache por ensayo, ausencia/ambigüedad, versión explícita y denegación previa al origen; arnés .NET y ciclo autenticado documentado |
| RQ-12, administración visual | ServicioAdministracionProveedor, repositorio, ValidadorAdministracionProveedor y ProveedorAdminEditor.js | Alta/configuración/publicación UI, BORRADOR excluido de consumidores, dos versiones, concurrencia/disponibilidad e inmutabilidad; doc110-estados-concurrencia-proveedor-20261027.md y corridas previas |
| Fuente exclusiva de empresa | PoliticaFuentesProveedor.Permite y MySqlProveedorDatosIndices.Consultar | Omisión de columna solo con ExclusivaEmpresa autorizada; modalidad COLUMNA conserva filtro exacto; arnés de política/proveedor y fuente exclusiva real documentada |
| AMP-02, fuentes persistentes | ServicioFuenteImportacion, ReglasFuenteImportacion y MySqlFuenteImportacionRepository | Cabecera/columnas/historial transaccionales, permiso adicional, política RA sin fallback, revocación sin alterar consumidores; doc110-fuentes-transacciones-preparacion.md y recuperación/ciclo de fuentes previos |
| Límite literal imp_02 | RestriccionTablasFuente, MetadatosFuenteImportacion, política y validación de proveedor | Pruebas .NET de excepción exacta por alias; metadatos RA/DA reales y rechazo ASMX del fixture 14/29 intacto/inactivo. Ver doc110-proveedor-legado-sintetico-20261027.md |

Las implementaciones VB citadas se encuentran en Modelo/ProduccionDocumental/ImportacionCarpetas, Services/ProduccionDocumental/ImportacionCarpetas, Infrastructure/ProduccionDocumental/ImportacionCarpetas e Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas. Clientes en js/gestion/ImportacionCarpetas. Los 33 archivos productivos de esas áreas y DTOs están incluidos en el vbproj; ninguna entrada ausente tras normalizar separadores de ruta.

## Comprobaciones ejecutadas en esta revisión

| Comando / control | Resultado |
|---|---|
| `node --test tests/doc110-*.test.cjs` | 27 PASS, 0 fallos, 10.9 s. Incluye compilación VB/C# y ejecución .NET Framework de diez familias de comportamiento sin conexión/HTTP |
| `node --test tools/e2e/tests/doc110-*.test.cjs tools/e2e/tests/doc88-production-document-upload-policy.test.cjs` | 73 PASS, 0 fallos, 2.1 s; dobles locales, no escrituras reales |
| Playwright, ocho archivos especificados abajo, `--reporter=line --output=artifacts/doc110-final-ui` | 15 PASS, 4 skipped, 20.7 s. Los cuatro omitidos son la suite autenticada anterior de PER/persistencia, sin sus variables de autorización; no se acredita una nueva ejecución real |
| `MSBuild.exe GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /p:DeployOnBuild=false /p:OutputPath=.tmp\doc110-final-build\bin\ /p:IntermediateOutputPath=.tmp\doc110-final-build\obj\ /clp:ErrorsOnly /nologo` | Salida 0. Compilación completa en directorio temporal; no reemplazó la DLL activa ni desplegó el sitio |
| OpenSpec validate `--strict` y apply | PASS, 178 completas, cero pendientes, all_done |
| `git diff --check` | Salida 0; aviso de conversión LF/CRLF en reporte OPSXJ preexistente |
| Diff de SII y de entradas legacy `js/gestion/WebFormProducionDocumental.js`, `webservice/WebServiceProducion.asmx.vb`, `Gestion/ClassGaProducionDocumental.vb` | Sin modificaciones en esas rutas. La página de entrada y el cargador de permisos sí tienen extensiones DOC-110; no se presenta como ausencia de cambios en todo Producción |

Archivos Chromium seleccionados: doc110-admin-layout.spec.cjs, doc110-admin-navigation.spec.cjs, doc110-fuentes-guided.spec.cjs, doc110-navigation.spec.cjs, doc110-perfil-autocomplete.spec.cjs, doc110-perfil-guided.spec.cjs, doc110-perfil-importacion.spec.cjs y doc110-source-transactions.spec.cjs, bajo tools/e2e/tests. Los 15 aprobados usan componentes reales y transporte simulado; los cuatro casos autenticados quedaron omitidos y su aceptación previa procede del runner --acceptance, no de esta corrida local.

Huella de revisión: HEAD `b2e93b726f221868cf2b82d43200a8e568871117`, agregado de 13 archivos `54874912e51c68d6f47bcfb09386a1ca6c1a7ffc1ab6f230aa455aa25879a8dc`. Manifiesto en tools/e2e/artifacts/doc110-final-verification-code.json; incluye fuentes/pruebas seleccionadas y DLL activa. No representa todos los archivos del cambio ni reemplaza hashes de corridas anteriores. DLL activa SHA-256 `80F305D1BC7D1F629431FA27EC011CF04A031EB323AD8C377F1E18FCDDCE2A21`, conservada tras MSBuild. HEAD por sí solo no describe los archivos sin commit.

## Coherencia y decisiones conservadas

Se actualizaron el resumen de propuesta, el estado actual del diseño y la introducción del procedimiento SQL 01: aún hablaban de propuesta o migración sin aplicar. Los antecedentes fechados se conservan como históricos, con enlace a las aceptaciones que los superan. El SQL ejecutable no se alteró: la instrucción posterior del usuario exige solo DDL y tiene un hash de instalación ya probado.

No se exige implementar ahora continuidad/detención de lotes, reconsulta en reintentos, reutilización documental o selección operativa IMPORTACION: los escenarios correspondientes fijan contrato para la entrega operativa futura. El servicio actual rechaza ese propósito. No se detectó un requisito operativo faltante dentro del alcance administrativo de DOC-110.

Límites de aceptación que permanecen visibles:

- Empresa única: pruebas reales entre empresas NO_APLICA, sin retirar las defensas servidor ni atribuirles PASS.
- Tarea 7.6: cierre por aceptación explícita del usuario. Revocación/restauración y rechazo sin facultad tienen evidencia propia; combinaciones no ejecutadas no se presentan como probadas.
- Aplicación externa de permisos fuera del proyecto: efecto de revocación comprobado con cambio puntual directo autorizado y restauración, sin afirmar integración externa.
- SII: preservación estática/de contrato y regresión de carga individual; no se realizó una importación SII real como aceptación de este cambio.
- 15.19: fixture legado sintético inactivo 14/29, no proveedor histórico real. El NOT_RUN anterior se conserva como tal.

No se repitieron cargas reales, renombrados de tablas, cambios de permisos ni altas durante esta revisión. No se realizó una auditoría exhaustiva de todos los archivos ajenos del árbol de trabajo; existen documentos temporales y otros archivos sin seguimiento que no deben incorporarse automáticamente al cierre.

## Evaluación final

Sin incidencias críticas identificadas en completitud, correspondencia funcional o coherencia. Cambio listo para preparar su archivo, conservando las decisiones y límites anteriores. Este informe no ejecuta archivo OpenSpec, publicación Jira, commit, push o despliegue de producción.
