<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07 -->

# Diseño de aceptación DOC-116

## Contexto y alcance

Fuente principal: copia local. Se reutilizan el mapa Diseno/design.md y contratos existentes; este documento decide cómo verificar brechas. Jira-context es la captura inmutable del ticket refinado. No hay defecto funcional demostrado que autorice reescribir el adaptador.

## Decisiones

### D-01 — Fuente local y procedencia

La copia principal local prevalece para inspección y revisión. La copia aislada solo protege Git; una divergencia se registra y nunca se resuelve sobrescribiendo producto local.

Evidencia local: Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorExpedienteImportacion.vb; tools/e2e/scripts/support/doc110-permission-db.ps1.

### D-02 — Reutilización del creador

Conservar adaptador, binding, barreras, autoridad y repositorio únicos. Histórico confirmado devuelve identidad sin otro escritor; IDs incompletos o checkpoint dudoso no autorizan recreación ni búsqueda por nombre.

Evidencia local: Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorExpedienteImportacion.vb Crear/RegistrarSalida; Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Creacion.vb CrearExpediente.

### D-03 — Destino fijo EXP-01

Preparar dos raíces para A, cambiar la selección visible a B y crear solo bajo A. Comprobar expediente/relación/ciclo/clasificación por SELECT e identidad histórica; no almacenar documentos definitivos.

Evidencia local: Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Creacion.vb CrearExpediente; Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorExpedienteImportacion.Legacy.vb InvocarConservacion.

### D-04 — Incertidumbre EXP-02

Distinguir respuesta confirmada descartada en cliente, fallo después de creación antes del checkpoint y pérdida de respuesta del checkpoint. Los dobles locales no acreditan falla real. No añadir hook productivo o endpoint; si no hay mecanismo interno seguro autorizado, registrar NO_EJECUTADO y bloquear cierre del caso.

Evidencia local: tests/ImportacionCarpetasBehaviorTests.cs CreationAdapterCases/CoordinatedCreationCases; Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorExpedienteImportacion.vb RegistrarSalida.

### D-05 — Autoridad de nivel EXP-03

Probar agregar_expediente en ra_pro_permisos_niveles para un actor que NO sea propietario de A y cuya vía efectiva sea esa concesión. IMPORTAR_CARPETAS es una facultad distinta; revocarla no sustituye el caso. Restaurar el valor exacto y comprobar cero creaciones.

Evidencia local: Infrastructure/ProduccionDocumental/ImportacionCarpetas/ImportacionCarpetasContextAdapter.vb ResolverDestino/AutorizarCreacionLegacy.

### D-06 — Infraestructura y mutaciones acotadas

Reutilizar sesión, safePost, consola, DSN MembershipUsers y controles SELECT. No usar runRec completo porque almacena documentos. El puente local es administrativo y la copia aislada tiene extensión operativa: ninguno acredita revocación de permiso de nivel. Una extensión de soporte requiere contrato cerrado, diario y restauración antes de cualquier ejecución.

Evidencia local: tools/e2e/tests/support/doc111-importacion-carpetas.cjs createPlanControls; tools/e2e/tests/support/doc110-permission-revocation.cjs startDatabaseBridge.

### D-07 — Cierre por evidencia y límites

EXP01/02/03 conservan resultado, SHA, comandos, ambiente y procedencia. No hay aceptación real nueva en este refinamiento. No modificar almacenamiento legacy/SII, aislamiento XML descartado, formatos, DDL o configuración; cambiar producto solo tras reproducir un defecto del alcance.

Evidencia local: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-matriz-final-20261102.md.

## Fronteras verificadas

AvanzarLote(CREAR_EXPEDIENTE) → coordinador existente → barrera/autoridad/staging → adaptador → Registrar_Expediente_Conservacion → IDs/relación → checkpoint en repositorio único. Una petición crea como máximo la fase solicitada; no llamar ALMACENAR ni PREPARAR_INDICES para simular cobertura adicional. La recepción PDF puede preparar staging bajo presupuesto; no es documento definitivo.

CrearExpediente consulta histórico antes de reservar. Un checkpoint confirmado se proyecta sin otra creación. RegistrarSalida falla de forma conservadora; EN_CURSO sin evidencia no se convierte en reserva nueva por antigüedad. Reconciliar requiere prueba persistida suficiente y autoridad, no nombre coincidente.

## Plan de escenarios

EXP01: preparar/confirmar un lote acotado, recibir contenidos autorizados, cambiar selección real A→B y ejecutar creación por raíz con los IDs/revisiones servidor. Consultar expediente y relación en A, clasificación y Archivo Gestión. La repetición usa el contrato histórico y conserva conteos. No asumir nivel7, perfil24 o usuario propietario por ser fixtures anteriores.

EXP02: A) consulta tras descarte de respuesta confirmada; B) fallo local ya cubierto por CreationAdapterCases/CoordinatedCreationCases; C) integración postlegacy/precheckpoint solo mediante una vía interna comprobada y autorizada. A y B no acreditan C. No borrar filas para fabricar incertidumbre ni añadir WebMethod/header/flag de fallo. Si C no puede hacerse sin alterar composición productiva, registrar dependencia y no cerrar el caso.

EXP03: ResolverDestino(CREAR_EXPEDIENTE) usa agregar_expediente=1 o propiedad del nivel. Preflight SELECT verifica identidad, empresa, no propiedad y concesión efectiva. Revocación limitada a la fila/columna autorizada; estado original y recuperación durables, restauración exacta en Finally y lectura independiente. El puente de DOC110 local no soporta ese contrato; tampoco basta la extensión IMPORTAR_CARPETAS de main. El contrato de extensión se revisa antes de implementarlo y no se usa una cuenta SELECT para mutar.

## Inventario propuesto de pruebas

| Ruta | Estado | Responsabilidad |
| --- | --- | --- |
| tools/e2e/tests/doc116-expedientes-importacion.spec.cjs | Implementada; ver matriz consolidada | Suite EXP01/02/03 con fases y evidencia diferenciada |
| tools/e2e/tests/support/doc116-expedientes-importacion.cjs | Implementada; ver matriz consolidada | Aserciones de creación, presupuestos, referencias y SELECT; transporte compartido |
| tools/e2e/scripts/run-doc116-expedientes-importacion.cjs | Implementada; ver matriz consolidada | Selector delgado delegado en consola/auth existentes |
| tools/e2e/fixtures/doc116-expedientes-cases.json | Implementada; ver matriz consolidada | Datos sintéticos y presupuestos revisables, sin secretos |

Se reutilizan package.json/playwright.config.cjs/reporteros, authenticated-workflow-session, interactive-e2e-console, doc110-safe-request, doc32-e2e-odbc y el arnés tests/doc110-perfil-importacion-behavior.test.cjs. No se copia runRec completo ni se crea proyecto/runner de autenticación. Los cambios compartidos requieren regresión de consumidores.

## Riesgos y salida

La copia local de utilidades difiere de main: registrar hashes y elegir explícitamente cualquier reconciliación futura; no copiar por conveniencia. El permiso de nivel puede impedir también consultas: usar controles independientes SELECT y restaurar antes de lectura UI final. La coexistencia con creación manual se caracteriza por sus límites y no promete aislamiento de XML externo.

Sin DDL/despliegue/configuración previstos. Si se reproduce un defecto, documentarlo, delimitar corrección y prueba antes de cambiar VB; compilar el proyecto Framework real solo si el cambio lo exige. El cierre requiere casos exactos o un estado de no ejecución que mantenga el bloqueo explícito, sin sustituirlo por mocks.

## Corte local 2026-11-05

Aprobación editorial mediante continuación del usuario registrada. Cliente de creación y SELECT no propietario implementados como soporte, con 11 contratos y dos regresiones safePost PASS; arnés Framework 2/2 PASS. No se agregó suite/selector E2E ni mutador de permisos: la composición real del fallo interno sigue indisponible y el permiso requiere fila/actor autorizados. Matriz y contratos de viabilidad en Validacion/doc116-expedientes-matriz.md, doc116-exp02-viabilidad-20261105.md y doc116-exp03-contrato-permiso-20261105.md.

## Composición E2E local — 2026-11-05

Suite y selector implementados para un lote ya confirmado, con dos raíces nuevas y dos originales recibidos. El runner no prepara, confirma ni recibe: el setup es un prerrequisito autorizado independiente; la aceptación de creación no se obtiene llamando runRec completo. --session contiene scenario, profile existente, identity para controles, levelB, lotId, roots[{rootId,revision,key}] y runtimeRoot del sitio; nunca credenciales. El operador debe revisar todos los recursos y la ventana sin otras creaciones en los niveles observados.

EXP01 crea dos raíces y repite cada comando exacto, comprobando SELECT, clasificación, ciclo, inventario sin documentos y conteos de nivel. La selección usa OnSearchClick/TreeViewArchivo del producto local, sin importar ni sobrescribir el helper DOC113 ausente en la copia principal. EXP02 cliente crea una raíz, descarta el resultado visible, comprueba el checkpoint antes del replay y conserva la otra raíz. caseComplete=false/internalFailure=NO_EJECUTADO impiden presentarlo como aceptación íntegra del fallo interno. EXP03 es rechazado antes de pedir credenciales hasta disponer del mutador seguro.

Playwright descubre cuatro casos; los dos no compuestos se declaran NO_EJECUTADO mediante skip explícito, no PASS. No se ejecutaron pruebas autenticadas. Véase Validacion/doc116-e2e-local-20261105.md.

## EXP03 compuesto localmente — 2026-11-05

Se extiende el puente administrativo existente con un parámetro Doc116LevelResourcesFile, conservando sus modos locales. El helper doc116-level-permission.ps1 fija tabla/columna/identidad, coteja hash del recurso y ambiente, exige no propiedad, actor/empresa/nivel/lote y una fila exacta. Requiere InnoDB en las cuatro tablas que bloquea; una instalación incompatible se rechaza antes de mutar, sin migración automática. UPDATE parametrizado mediante CAS en transacción, diario atómico y readback en conexión independiente sin pooling. EOF/fallo intenta restauración; discrepancia ajena conserva recuperación y no fuerza valores.

La sesión de EXP03 añade levelPermission{permissionId,environmentHash}; los IDs reales y el fingerprint de ambiente se obtienen y revisan antes de autorizar. El runner exige autorización específica levelPermission. La suite verifica que runtimeRoot sea el mismo sitio que el puente para no usar configuración de otra copia. El recurso del puente se conserva sin secretos en artifacts/doc116-level-resources-<runId>.json, con SHA efímero. La cuenta SELECT solo comprueba; el puente usa la conexión administrativa registrada.

La copia Git mantiene su variante operativa anterior, mientras la principal conserva la administrativa local: se aplicó la misma extensión sin copiar una variante encima de otra. Ese delta se registra por hashes. EXP03 tiene suite, soporte y aserciones locales; su aceptación MySQL/IIS aún NO_EJECUTADO. EXP02 interno sigue sin composición autenticada de fallo y se declara como tal.

## Estado consolidado de implementación — 2026-11-05

Los cortes anteriores describen etapas históricas. Suite, selector, setup compartido EXP01/02/03 y puente nativo están implementados. EXP01 real PASS (32/16; 657/658), EXP02 cliente PASS parcial (33/17; 659) y EXP03 real PASS restaurado (34/18; fila 629). No cambió producto ni se relajaron D04/RQ04.

20/20 tareas de trabajo/consolidación no equivalen a aceptación integral. EXP02 interno sigue NOT_COMPOSED/NO_EJECUTADO; no se archiva ni se cierra Jira. Estado/evidencia canónica: Validacion/doc116-verificacion-final-20261105.md y doc116-expedientes-matriz.md. La vía interna autenticada debe revisarse con presupuesto/recuperación y autorización específica antes de ejecutar. No se agrega hook público ni se sustituye por descarte cliente.

## Resolución del bloqueo de aceptación EXP02 interno

Se autorizó y ejecutó una compilación exclusiva de laboratorio derivada de la copia local, con alcance fijo lote 18/raíces 29/30, autoridad HTTP real y marca de un solo uso. Excepción antes de checkpoint y excepción al devolver checkpoint confirmado acreditadas; no se añadió endpoint/flag al runtime normal. DLL original restaurada, dos copias conservadas y verificadas byte por byte. Evidencia: Validacion/doc116-exp02-interno-resultados.md. El intento 41 conserva incertidumbre sin recreación. La aceptación funcional del alcance queda verificada; entrega/cierre formal aún pendientes. Los bloqueos de cortes anteriores son históricos.
