# DOC-114 — resultado técnico consolidado

PLAN-01, PLAN-02 y PLAN-03: **EJECUTADO_OK** en el mismo ambiente de pruebas localhost y cuenta autorizados. Se conservan **13 planes (11–23)**; cero confirmaciones, lotes, recepción de contenido, expedientes o documentos nuevos por estas pruebas. IMPORTAR_CARPETAS quedó restaurado en 1, con otros campos intactos, reapertura operativa y diario RESTORED. La aprobación editorial/publicación del cambio permanece separada del runtime autorizado.

| Escenario | Resultado | Recursos | Evidencia |
| --- | --- | --- | --- |
| PLAN-01 | PASS, siete casos | Planes11–17; 11 revalidado por lectura, seis nuevos en la continuación | [Resultado](doc114-plan01-result-20261103.json) |
| PLAN-02 | PASS, siete intentos / cuatro planes / tres rechazos | Planes18–21; planes11–17 sin cambios | [Resultado](doc114-plan02-result-20261103.json) |
| PLAN-03 | PASS, ocho intentos operativos acumulados autorizados | Planes22/23; 22 conservado, cinco rechazos verificados al continuar; permiso restaurado | [Resultado y restauración](doc114-plan03-result-20261103.json) |

La repetición negativa adicional de PLAN-03 fue expresamente autorizada. Hubo además dos consultas diagnósticas anónimas 401, separadas del presupuesto de la cuenta operativa. Los intentos fallidos y sus referencias siguen en el [historial](doc114-plan-preflight.md); no se presentan como corridas aprobadas ni se borraron/reemplazaron planes.

## Alcance implementado

Se reutiliza el preflight existente de DOC-111: ServicioPreparacionImportacion, ASMX, RequestGuard, catálogos, resolvedor y repositorio único. El cambio aporta suite focal, cliente acotado, controles SELECT, fixtures, selector fino y evidencia DOC-114. No se modificaron archivos VB, proyecto, DTO, esquema, almacenamiento/SII ni Web.config. La compilación aislada Framework del corte inicial sigue aplicando porque sus fuentes no cambiaron; no se desplegó una DLL.

Soporte de recursos de prueba: perfiles26/40 y27/41 creados/publicados con aprobación específica; después se publicó27/42 cambiando únicamente la regla de tipología a REFERENCIA. Se conservan24/38,25/39 y el contenido histórico41. Los helpers de publicación están separados de la lista de operaciones del preparador; no forman parte de una preparación normal. El puente DOC-110 mantiene su modo administrativo y añade solo OperationalImport autorizado con prueba de base operativa por plan/huella, compare-and-set, diario y restauración verificada.

Correcciones del E2E acreditadas por regresión: empresa del documento filtrada en inventario correlacionado; equivalencia de Proveedor/Procedencia null con omisión según atributos del modelo; orden canónico de reglas al contrastar publicación y snapshot; código exacto CONFIGURACION_INVALIDA para inyecciones según la DLL existente. La protección sigue comprobando IDs, prioridades, valores, procedencias configuradas, revisión/huella y contexto.

## Reproducibilidad y procedencia

Comando estable desde el proyecto candidato: `npm.cmd --prefix tools/e2e run test:doc114:plan -- --session <recursos-revisados.runtime.json> --scenario PLAN-01 --check`; sustituir escenario por PLAN-02/03. Sin --check requiere consola y alcance concreto autorizado. Reutiliza configuración/reporteros/login/consola/DSN existentes; no se introdujeron .env, autenticación, motor o plataforma paralelos. El ejemplo versionado contiene referencias ilustrativas, no aprobación del ambiente.

Node regresión final aplicable: **88/88 PASS** (driver, controles, serialización, publicadores y consumidor de revocación); precondiciones de continuación **4/4 PASS**. Puente PowerShell por AST con proveedor en memoria PASS sin BD; huella de documentos canónicos contrastada con HuellaPlan Framework **1/1 PASS**. Arnés Framework existente **2/2 PASS** y MSBuild aislado exit0, fuentes backend conservadas. Los logs/hashes se consolidan en el manifiesto final; estas comprobaciones locales no sustituyen los tres resultados reales.

DLL servida antes/después de los tres casos: **4134f57f66fc259b5f33a553577c9a07e6bf176b2da027aaeeb8576325c282c5**. Scope de código ejecutado: PLAN-01 `0b3bf1b95684894cfd114f712a4089840f88ea55e84d34a59e9dec9b52351600`; PLAN-02 `2b0c75629be41dc6e5a90a96e5019453641ae102894a25fca75c347f6179b728`; PLAN-03 `7f5f17515b39859c1038492ae19c9f7c3f619af8f55ef6fb6ef113728d3876ad`. HEAD4461453f100e96e47c122d7a0f7222034b8e93da identifica el commit del worktree al probar; cada scopeSha identifica el contenido candidato correspondiente y no atribuye a HEAD ni al runtime los cambios sin commit.

## Cobertura de requisitos y escenarios

| Requisito | Escenario | Evidencia | Procedencia |
| --- | --- | --- | --- |
| RQ-01 | Contexto válido | PLAN-01/02/03, contexto persistido | SERVIDOR REAL |
| RQ-01 | Nivel ajeno o permiso revocado | PLAN-02 foreign-level; PLAN-03 revoked-permission | SERVIDOR REAL |
| RQ-02 | Homónimos y subcarpetas | ManifestCases y PreflightCalculationCases, arnés Framework existente | LOCAL FRAMEWORK |
| RQ-02 | Manifiesto incoherente o excedido | PLAN-02 global-error/inconsistent-manifest; cuotas/colisiones en ManifestCases | REAL + LOCAL |
| RQ-03 | Dos formatos configurables | PLAN-01 valid-format-a/b, versiones 40/42 | SERVIDOR REAL |
| RQ-03 | Errores por archivo | PLAN-01 cinco variantes; PLAN-02 TIF rechazado | SERVIDOR REAL |
| RQ-03 | Identidad futura y contenido no recibido | PLAN-01 índice14 Diferido=true/Valor=null/PropiedadPendiente; arnés sin business IO | REAL + LOCAL |
| RQ-04 | Completo por expediente | PLAN-02 plan18, r1 elegible y r2 rechazada visible | SERVIDOR REAL |
| RQ-04 | Completo por lote | PLAN-02 plan19, ninguna raíz elegible | SERVIDOR REAL |
| RQ-04 | Parcial con rechazados | PLAN-02 planes20/21, raíz mixta conserva PDF/TIF | SERVIDOR REAL |
| RQ-04 | Error global | PLAN-02 tres rechazos sin nuevos planes; conteos SELECT | SERVIDOR REAL |
| RQ-05 | Contenido determinista | PreflightCalculationCases compara Escribir(first) con segundo cálculo idéntico | LOCAL FRAMEWORK |
| RQ-05 | Autoridad o referencias alteradas | PLAN-03 inyecciones/nivel/versión/permiso; PlanStorageCases/ConfirmationServiceCases para revisión-huella | REAL + LOCAL |
| RQ-05 | Cambio compatible de publicación | PLAN-03 plan23 versión42 y plan22 versión40 intacto | SERVIDOR REAL |
| RQ-06 | Sin alcance de runtime aprobado | Selector --check sin login; omisión de tres casos sin flags; tests del selector | LOCAL POLÍTICA |
| RQ-06 | Ejecución autorizada | PLAN-01/02/03 propios, autorizaciones/budgets concretos y SHA separados | SERVIDOR REAL |
| RQ-06 | Cierre con dependencia pendiente | Fallos y recursos pendientes conservados hasta superar controles/restauración; no hubo PASS anticipado | HISTORIAL REAL + POLÍTICA |

Los seis requisitos y diecisiete escenarios tienen correspondencia explícita. Homónimos/subcarpetas/cuotas y determinismo se acreditan por el arnés Framework existente, sin denominarlos E2E de servidor real. El reporte de verificación detalla los métodos/archivos y los límites de esa procedencia.

## Límites de la aceptación

El cuerpo HTTP original del plan11 no se retuvo; su caso combina las aserciones originales y la revalidación posterior de huella, valores, manifiesto, contexto, snapshot publicado y lectura final. No se afirma comparación retroactiva de ese cuerpo. El catálogo no ofrecía un código único utilizable para el caso positivo por código: dos formatos válidos usan referencia y la versión41 TEXTO verifica negativamente que el ID79 no se interpreta como código. No se fabricaron códigos de catálogo.

Solo se validaron metadatos: no se acredita PDF físico, recepción, huella de contenido binario o materialización de IDs futuros. Los campos pendientes conservaron descriptor tipado y valor null. Las capturas/trazas/video de la suite estuvieron desactivadas. Las credenciales no se persistieron; se conservaron IDs, hashes y estados saneados.

La finalización técnica no marca automáticamente refinamiento aprobado, publicación remota, merge ni cierre Jira. Consultar [verificación](doc114-verification-20261103.md) y [manifiesto](doc114-final-manifest-20261103.json) para el estado de las comprobaciones finales.

## Aprobación editorial posterior — 2026-11-03

El usuario aprobó la continuación de publicación del cierre técnico verificado: confirmación «continua» y reporte revisado SHA256 63571253664acd4d2026827bdef91b204fcd3d3efd65a6e1a833fe6b336785f8. La aprobación pendiente descrita antes es histórica; publicación/merge/cierre remoto aún no se atribuyen como hechos. Especificación principal sincronizada con seis requisitos y diecisiete escenarios.
