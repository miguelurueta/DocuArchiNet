<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06 -->
# DOC-114 — Diseño de evidencia focal del preflight

## Context

Base main 6886677a02c8bf3a379eba5f8e7180d3d81de0fb. Ver motivación en [proposal.md](proposal.md) y contraste en [gap-analysis.md](gap-analysis.md). Los archivos de planificación iniciales eran plantillas; se sustituyen por un diseño específico, todavía draft. Fuente Jira íntegra en specs/produccion-importacion-carga-carpeta-plan-imple/jira-context.md.

El mapa de implementación canónico sigue en Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/design.md. La primera tarea añadirá allí las rutas DOC-114 propuestas; esta lista delimita candidatos, no los presenta como ya creados. Stack inspeccionado: WebForms/VB.NET Framework 4.6.1, ASMX, JavaScript y MySQL existente.

## Objetivos y fronteras

Acreditar preparación sin efectos documentales. Las escrituras permitidas en una corrida real son exclusivamente nuevas preparaciones enumeradas y autorizadas. No confirmar planes válidos, recibir archivos, crear expedientes, materializar valores ni almacenar documentos. El cálculo no lee binarios. No se implementa una funcionalidad nueva por tratar una capacidad existente como ausente.

## D-01 — Reutilizar servicio, entrada HTTP y autoridad

Conservar ServicioPreparacionImportacion.Abrir/ListarPerfiles/CalcularPlan/Preparar y WebServiceImportacionCarpetas.AbrirImportacion/PrepararPlan. La composición ya conecta ResolverDestino, AdaptadorPerfilImportacionOperativa, catálogos, resolvedor y repositorio único. Los guards resuelven sesión/actor/ámbito y validan cuerpo/CSRF; la selección de perfil no concede permisos de nivel/gabinete/creación/carga.

La suite usa la entrada HTTP real y verifica ok/codigo/datos, no solo HTTP 200. Negativos de autoridad deben rechazar sin nueva preparación. Alternativa descartada: un endpoint de pruebas, otro servicio de preflight o llamadas al creador para comprobar permisos; duplicarían autoridad o producirían efectos fuera de alcance. Origen: RQ-01.

## D-02 — Canonización e inventario completo del contrato vigente

Reutilizar CanonizadorManifiestoImportacion.Canonizar/RutaCanonica/EvaluarAgrupacion para cuotas, correlaciones, duplicados, rutas normalizadas y política de subcarpetas. Raíces homónimas se distinguen por seleccionId/idCliente; las rutas no son ubicaciones físicas. PrepararPlan recibe manifiesto completo en una petición, sin campos ni operación de fragmentación. No se agrega ensamblaje ni se promete detectar archivos locales omitidos que nunca fueron declarados.

La matriz negativa cubre raíz referenciada ausente, raíz declarada sin archivos, ID duplicado y ruta incompatible; cada caso tiene rechazo esperado explícito. Se conserva el inventario de las unidades con incidencias al evaluar políticas. Alternativa descartada: excluir inválidos antes de enviar o dividir arbitrariamente el manifiesto, porque alteraría denominadores y el contrato. Origen: RQ-02.

## D-03 — Reglas publicadas, catálogos y fuentes diferidas

Reutilizar CoordinadorProveedorIndices/ResolvedorReglasPerfilImportacion y PerfilImportacionCatalogAdapter.ValidarValoresOperativos. Fixtures de metadatos declaran dos formatos realmente configurados, ceros iniciales, fecha/conversión/longitud, requerido ausente y tipología no unívoca. Una regla por destino; diferidos TRAS_EXPEDIENTE conservan descriptor tipado y valor nulo. Preparar no materializa ID/consecutivo ni verifica huella/recepción/formato físico.

Los fixtures locales serán tools/e2e/fixtures/doc114-plan-cases.json, propuesta nueva de datos sintéticos sin secretos, binarios ni configuración de plataforma. Las referencias de publicaciones reales se validarán al preparar la corrida, con el perfil base DOC-111 cerrado, sin aceptar campos adicionales en ese esquema. Alternativa descartada: hardcodear reglas de extracción en el navegador o probar solo con publicaciones cuyo formato no corresponda al caso. Origen: RQ-03.

## D-04 — Matriz de políticas sin ejecutar barreras

EvaluarElegibilidad ya implementa COMPLETO_OBLIGATORIO/EXPEDIENTE, COMPLETO_OBLIGATORIO/LOTE y PARCIAL_PERMITIDO/NO_APLICA. PLAN-02 prepara A válida/B inválida en cada política y comprueba PuedeConfirmar, UnidadesElegibles, incidencias y conservación de B. La variante global debe provenir de una entrada/condición real de rechazo; no se fabrica una respuesta del servidor ni se altera la publicación en vuelo para obtenerla.

EXPEDIENTE permite A conservando B bloqueada; LOTE bloquea todo por error individual; PARCIAL mantiene el rechazado y admite unidades/archivos válidos; error global bloquea siempre. La elegibilidad de metadatos no acredita recepción completa. Alternativa descartada: repetir REC/documentos para probar una función de preparación; las barreras de ejecución pertenecen a entregas previas y se preservan. Origen: RQ-04.

## D-05 — Persistencia, integridad y determinismo existentes

CalcularPlan no persiste; Preparar fija reloj UTC/TTL y delega MySqlImportacionCarpetasRepository.GuardarPlan. Solicitud: nivelRef, versionId, raices, archivos. Respuesta: planId, revision, planHash, expiraUtc, nivelRef, snapshot, resultado. La autenticación y los valores finales se resuelven en servidor. No cambia DTO, DDL ni repositorio. Revisión/huella y asociación a usuario/ámbito/destino se comprueban sobre el plan persistido con SELECT.

Comparar el contenido canónico excluyendo identidad, revisión/control temporal documentados; no exigir igualdad del sobre entero entre preparaciones que generan IDs/timestamps nuevos. Elegir otra publicación compatible es legítimo y prepara otro plan. PLAN-03 diferencia esa selección de una versión inexistente/incompatible o de inyectar resultado/snapshot/autoridad en el cuerpo cerrado.

La protección por revisión/huella se caracteriza con ConfirmationServiceCases/PlanStorageCases. Si su prueba HTTP negativa requiere ConfirmarPlan, solo se admite con autorización separada, discrepancia comprobada y cero lotes nuevos; nunca se envía confirmación válida. No se corrompen filas SQL para probar integridad.

La revocación real será una fase operativa explícita con cuenta/recurso/mecanismo existentes autorizados y restauración comprobada antes de cerrar. El runner no concede ni revoca permisos arbitrariamente; si la fase no está disponible queda NO_EJECUTADO con responsable. Alternativa descartada: considerar todo cambio de versionId manipulación o suplir revocación real con un mock como aceptación. Origen: RQ-05.

## D-06 — Suite focal y herramientas compartidas

Rutas nuevas propuestas: tools/e2e/tests/doc114-plan-importacion.spec.cjs, tools/e2e/tests/support/doc114-plan-importacion.cjs, tools/e2e/scripts/run-doc114-plan-importacion.cjs y fixtures/doc114-plan-cases.json. Reutilizar tools/e2e/package.json/playwright.config.cjs/reporteros, openSession, authenticated-workflow-session, interactive-e2e-console, validateProfile y puente ODBC queryRows. El soporte aísla fixtures, invocaciones/control de operaciones y SELECT; no duplica login ni ejecuta runRec.

El selector solo elige escenario, valida referencias/perfiles base y alcance, solicita secretos efímeros en consola y delega al proceso compartido. La suite captura referencias numéricas de planes antes de otras aserciones, sin reintentar PrepararPlan ante incertidumbre. Guard de operaciones: lecturas y preparaciones explícitamente autorizadas; recepción/avance/administración bloqueados. Confirmación negativa excepcional se habilita únicamente para la discrepancia autorizada de D-05.

Controles SELECT: snapshot de planes asociados a caso/actor/ámbito/destino/publicación, contenido protegido y conteos de lotes/recursos documentales antes/después, reutilizando consultas existentes. No se crean tablas, índices ni consultas de escritura para instrumentación. No guardar filas de negocio, cuerpos privados, cookies, tokens o conexiones; conservar únicamente huellas y estados saneados. Las capturas/trazas de login quedan desactivadas según herramientas existentes.

PLAN-01/02/03 tendrán resultado EJECUTADO_OK/FALLIDO/NO_EJECUTADO con SHA/huellas de archivos del corte probado. Arnés local, REC DOC-111 y UI DOC-113 mantienen procedencia propia; no se renombran como aceptación PLAN. Alternativa descartada: copiar una suite completa o una configuración Playwright paralela. Origen: RQ-06.

## Matriz de recorridos propuestos

| Caso | Entradas y acciones | Aserción principal | Escrituras posibles |
|---|---|---|---|
| PLAN-01 | Metadatos válidos en dos formatos y variantes de tipología/nombre/requerido/conversión/longitud. | Valores/incidencias y diferidos correlacionados por archivo, sin IDs ficticios. | Planes de preparación aprobados; sin lotes/documentos. |
| PLAN-02 | A válida/B inválida en tres políticas; nivel ajeno/inventario incoherente/global. | Ámbito y elegibilidad correctos, B visible, rechazo global siempre. | Preparaciones aprobadas; rechazos de autoridad/forma no crean plan. |
| PLAN-03 | Plan preparado, cuerpo/referencias/versión incompatibles, cambio compatible de control y revocación real. | Autoridad/protección servidor y ausencia de efectos; restauración comprobada si se revoca. | Preparaciones autorizadas; cambio de permiso separado, sin confirmación válida. |

## Riesgos y dependencias

- [Perfiles/formatos no contrastados] → Comparar snapshots y reglas antes de crear planes; no asumir que 37/38/39 sirven a toda la matriz.
- [Revocación o publicación administrativa no autorizada] → Mantener esa fase pendiente; solicitar alcance concreto con recurso y reversa antes de ejecutar, sin ampliar permisos.
- [Operación de preparación incierta] → Conservar planId cuando exista y consultar estado por lectura antes de repetir.
- [Concurrencia ajena afecta conteos] → Acotar controles a contexto/casos; documentar exclusión temporal si fuese necesaria, sin bloquear globalmente producción.
- [Brecha funcional hallada al caracterizar] → Registrar reproducción y añadir tarea focal con área/verificación antes de modificar producto; no existe hoy una corrección autorizada por un fallo demostrado.

## Implementación, validación y reversa

Primero inventario común y caracterización local; después fixtures, soporte/controles, escenarios y selector; luego política/regresión/compilación; finalmente ejecución real autorizada y dossier. MSBuild Framework con salidas .tmp aisladas, sin desplegar DLL ni tocar Web.config. El alcance inicial es pruebas/documentación; cualquier ajuste de producto necesita evidencia de regresión concreta. La reversa retira el diff DOC-114 y conserva planes autorizados y recursos históricos. Sin nuevo job, esquema, motor ni cambios al almacenamiento/SII.

## Estado del corte

Diseño propuesto, sin código E2E nuevo ni pruebas ejecutadas en DOC-114. La alineación estructural y las tareas no conceden aprobación editorial ni autorización de runtime. Decisiones D-01 a D-06 y requisitos RQ-01 a RQ-06 siguen en draft.

## Implementación local posterior — 2026-11-03

Suite PLAN-01/02/03 y selector ya implementados en las rutas propuestas, sin login real. La sesión de prueba usa perfiles base cerrados más datos de casos/esperados e identidad, runtimeRoot y referencias negativas explícitas. El ejemplo está en tools/e2e/profiles/doc114-plan-session.profile.example.json; sus IDs/recursos son ilustrativos. El puente compartido añade OperationalImport con raíz runtime, prueba de plan/hash del mismo origen, autorización y recuperación separadas; el modo administrativo se conserva. Prueba local en tools/e2e/tests/doc114-permission-bridge-local.ps1, sin BD. No se modificó backend ni se autorizaron corridas reales. Corte 12/16; evidencia en Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc114-plan-preflight.md. Las frases anteriores sobre rutas no creadas describen el corte de planificación.

## Resultado técnico integrado DOC-114 — 2026-11-03

Los cortes propuestos/locales anteriores son históricos. PLAN-01/02/03 tienen evidencia real PASS; permiso operativo restaurado y planes11–23 conservados. El preflight de producto se reutilizó sin cambios. Se implementaron únicamente suite/soporte/selector/fixtures, la extensión autorizada del puente y helpers acotados para recursos de prueba. Ajustes del E2E: correlación de empresa documental por inventario, null/omisión conforme a atributos, orden canónico de publicación y código CONFIGURACION_INVALIDA del validador existente.

Cobertura: seis requisitos/diecisiete escenarios con procedencia real/local explícita. Homónimos/subcarpetas/cuotas/determinismo se acreditan por el arnés Framework existente; no se renombran como E2E. Límites: cuerpo original de plan11 no retenido y positivo por código no disponible; lectura/huellas y negativo ID/código conservan evidencia propia. Informe canónico: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc114-final-report-20261103.md. La aprobación editorial de este refinamiento permanece draft; las autorizaciones de runtime no la sustituyen.
