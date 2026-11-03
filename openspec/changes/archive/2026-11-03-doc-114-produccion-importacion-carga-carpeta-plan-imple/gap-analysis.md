# DOC-114 — Reutilización y brechas por inspección

Fecha: 2026-11-03. Base main 6886677a. Lectura de código/pruebas; ninguna ejecución DOC-114 ni efecto del runtime.

| Capacidad | Evidencia existente | Trabajo justificado |
|---|---|---|
| Contexto y autoridad | ServicioPreparacionImportacion.Abrir/ListarPerfiles; ImportacionCarpetasContextAdapter.ResolverDestino/AutorizarRecursos. | Reutilizar y acreditar negativos reales. |
| Manifiesto/cuotas/rutas/homónimos | CanonizadorManifiestoImportacion; DTO PrepararPlanImportacionSolicitud; ManifestCases. | Matriz de casos sin inventar fragmentación. |
| Dos formatos, ceros, fechas, diferidos | tests/ImportacionCarpetasBehaviorTests.cs:1885 PreflightCalculationCases; resolvedor/coordinador compartidos. | Ejecutar arnés existente y diseñar fixtures reales equivalentes. |
| Tipo desconocido | PerfilImportacionCatalogAdapter.ValidarValoresOperativos distingue TIPOLOGIA/TIPOLOGIA_CODIGO; TIPOLOGIA_NO_UNIVOCA. | Verificar incidencia por archivo con publicación configurada. |
| EXPEDIENTE/LOTE/PARCIAL | EvaluarElegibilidad y casos integrados de Preparar con A/B y errores globales. | PLAN-02 real, sin reconstruir elegibilidad. |
| Plan persistido | ServicioPreparacionImportacion.Preparar; MySqlImportacionCarpetasRepository.GuardarPlan:235. | Controles SELECT para resultado/TTL/huella; mismo repositorio de 07. |
| Protección/revocación/determinismo | PreflightCalculationCases, PlanStorageCases, ConfirmationServiceCases, AuthenticatedRevocationCases. | PLAN-03 focal y autorización para revocación/restauración. |
| Infraestructura E2E | DOC-111 REC y DOC-113 UI, sesión/consola/DSN compartidos. | Suite PLAN-01/02/03 no encontrada; añadir recorrido focal sin ejecutar REC. |

## Precisiones

PrepararPlan recibe nivelRef/versionId/raices/archivos. Respuesta: planId/revision/planHash/expiraUtc/nivelRef/snapshot/resultado. Identidad autenticada y valores calculados pertenecen al servidor. CalcularPlan no persiste; Preparar crea un plan: «sin efectos documentales» no significa «sin escrituras de plan». El número de preparaciones y sus datos deben concretarse antes de ejecutar.

No hay fragmentos en el DTO vigente ni protocolo que ensamblar. Se valida consistencia interna, sin inferir archivos locales omitidos que el servidor desconoce. Cambiar a una versión compatible es legítimo; se distingue de inyectar autoridad o alterar revisión/huella del plan protegido.

Consultar coincidencias no garantiza ausencia de carrera futura: la comprobación al crear corresponde al ejecutor y no se invoca desde preflight para probarla. Diferidos no se materializan durante preparación; no se acredita recepción ni formato físico con metadatos.

## Evidencia y pendientes

No se demostró un defecto funcional nuevo. Existen aserciones locales relevantes ya escritas; su presencia no es ejecución DOC-114. Los REC/UI históricos no certifican PLAN-01/02/03. No se crearon las rutas E2E propuestas ni se modificó producto.

Corte siguiente aplicado: design/spec/tasks alineados con las seis decisiones de refinement.md, sin ejecutar sus tareas. Mantener tareas sin completar hasta ejecutar su entregable/verificación. Marcador draft conservado; recursos reales y revocación requieren autorización específica.

## Implementación local posterior — 2026-11-03

Suite PLAN-01/02/03 y selector ya implementados en las rutas propuestas, sin login real. La sesión de prueba usa perfiles base cerrados más datos de casos/esperados e identidad, runtimeRoot y referencias negativas explícitas. El ejemplo está en tools/e2e/profiles/doc114-plan-session.profile.example.json; sus IDs/recursos son ilustrativos. El puente compartido añade OperationalImport con raíz runtime, prueba de plan/hash del mismo origen, autorización y recuperación separadas; el modo administrativo se conserva. Prueba local en tools/e2e/tests/doc114-permission-bridge-local.ps1, sin BD. No se modificó backend ni se autorizaron corridas reales. Corte 12/16; evidencia en Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc114-plan-preflight.md. Las frases anteriores sobre rutas no creadas describen el corte de planificación.

## Resultado técnico integrado DOC-114 — 2026-11-03

Los cortes propuestos/locales anteriores son históricos. PLAN-01/02/03 tienen evidencia real PASS; permiso operativo restaurado y planes11–23 conservados. El preflight de producto se reutilizó sin cambios. Se implementaron únicamente suite/soporte/selector/fixtures, la extensión autorizada del puente y helpers acotados para recursos de prueba. Ajustes del E2E: correlación de empresa documental por inventario, null/omisión conforme a atributos, orden canónico de publicación y código CONFIGURACION_INVALIDA del validador existente.

Cobertura: seis requisitos/diecisiete escenarios con procedencia real/local explícita. Homónimos/subcarpetas/cuotas/determinismo se acreditan por el arnés Framework existente; no se renombran como E2E. Límites: cuerpo original de plan11 no retenido y positivo por código no disponible; lectura/huellas y negativo ID/código conservan evidencia propia. Informe canónico: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc114-final-report-20261103.md. La aprobación editorial de este refinamiento permanece draft; las autorizaciones de runtime no la sustituyen.
