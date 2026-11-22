<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento DOC-122

## Fuente y alcance
El usuario solicitó iniciar DOC-122 y continuar su implementación sobre el prompt02 previamente elegido. Revisión técnica Codex 2026-11-21; ajuste posterior autorizado con «continua» tras solicitar reutilizar las conexiones del repositorio: se elimina la cuenta adicional obligatoria y el vínculo duplicado Web.config; aprobación de alcance no equivale a instalación ni E2E real.

## Contexto inspeccionado
- PerfilImportacionContextAdapter.Resolver/AutorizarFuentes: Consultar RA en cada operación: usuario activo, empresa y permiso exacto ADMINISTRAR_CARGAS_IMPORTACION=1. Indicador de sesión solo para presentación.
- ModuleSessionConnectionStringResolver.Resolve: Resolver DA mediante ModuleSessionConnectionStringResolver y la sesión del módulo cargada desde gestor_modulos. Exigir empresa/módulo vigentes y correspondencia exacta del ID histórico con servidor/base/proveedor de esa DA. No leer credenciales de imp01_xmlconexion ni exigir otra conexión en Web.config.
- Class_imp01_plantillaimp / Class_imp01_campos_plantilla: DTO por ID, paginación acotada y destino imp_02_ validado en information_schema. CREAR admite destino ausente; AGREGAR/REEMPLAZAR requieren columnas existentes. Guardar no altera destino.
- MySqlPerfilImportacionRepository: Transacción InnoDB para cabecera y campos; bloqueo y comparación de revisión más huella vigente. Un ganador, conflicto para el segundo, rollback completo ante fallo.
- Doc/Actualizacion/Importacion/Diseno/01-contratos-configuracion-doc121.md: Conservar ruta histórica, ID de mapeos y transformaciones no cambiadas. Snapshot v1 canónico DOC121; NULL explícito, orden ordinal, SHA256. Leer sin clave permitido; guardar exige clave mapeada. Flags inconsistentes dan error controlado.
- PerfilImportacionRequestGuard / WebServiceImportacionCarpetas: POST JSON, mismo origen, CSRF propio ligado a sesión/usuario/empresa; bootstrap valida origen antes de emitir token. DTO de respuesta tipados. Límites positivos con defaults 10000/10MB decimal.
- AGENTS.md / tools/e2e/AGENT-RUNBOOK.md: Build aislado y pruebas nativas sintéticas; E2E reutiliza tools/e2e y requiere autorización específica. Instalación SQL manual y datos EGRESOS intactos.

## Decisiones aprobadas
| ID | Decision verificable | Evidencia de codigo | Design | Requirement | Tasks |
|---|---|---|---|---|---|
| D-01 | Consultar RA en cada operación: usuario activo, empresa y permiso exacto ADMINISTRAR_CARGAS_IMPORTACION=1. Indicador de sesión solo para presentación. | PerfilImportacionContextAdapter.Resolver/AutorizarFuentes | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Resolver DA mediante ModuleSessionConnectionStringResolver y la sesión del módulo cargada desde gestor_modulos. Exigir empresa/módulo vigentes y correspondencia exacta del ID histórico con servidor/base/proveedor de esa DA. No leer credenciales de imp01_xmlconexion ni exigir otra conexión en Web.config. | ModuleSessionConnectionStringResolver.Resolve | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | DTO por ID, paginación acotada y destino imp_02_ validado en information_schema. CREAR admite destino ausente; AGREGAR/REEMPLAZAR requieren columnas existentes. Guardar no altera destino. | Class_imp01_plantillaimp / Class_imp01_campos_plantilla | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Transacción InnoDB para cabecera y campos; bloqueo y comparación de revisión más huella vigente. Un ganador, conflicto para el segundo, rollback completo ante fallo. | MySqlPerfilImportacionRepository | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Conservar ruta histórica, ID de mapeos y transformaciones no cambiadas. Snapshot v1 canónico DOC121; NULL explícito, orden ordinal, SHA256. Leer sin clave permitido; guardar exige clave mapeada. Flags inconsistentes dan error controlado. | Doc/Actualizacion/Importacion/Diseno/01-contratos-configuracion-doc121.md | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | POST JSON, mismo origen, CSRF propio ligado a sesión/usuario/empresa; bootstrap valida origen antes de emitir token. DTO de respuesta tipados. Límites positivos con defaults 10000/10MB decimal. | PerfilImportacionRequestGuard / WebServiceImportacionCarpetas | D-06 | RQ-06 | Origen: D-06, RQ-06 |
| D-07 | Build aislado y pruebas nativas sintéticas; E2E reutiliza tools/e2e y requiere autorización específica. Instalación SQL manual y datos EGRESOS intactos. | AGENTS.md / tools/e2e/AGENT-RUNBOOK.md | D-07 | RQ-07 | Origen: D-07, RQ-07 |

## Requisitos verificables
| ID | Resultado observable | Escenario o criterio de aceptacion | Riesgo/compatibilidad |
|---|---|---|---|
| RQ-01 | Consultar RA en cada operación: usuario activo, empresa y permiso exacto ADMINISTRAR_CARGAS_IMPORTACION=1. Indicador de sesión solo para presentación. | Permiso exclusivo; Acceso revocado | Autorización vigente propia |
| RQ-02 | Resolver DA mediante ModuleSessionConnectionStringResolver y la sesión del módulo cargada desde gestor_modulos. Exigir empresa/módulo vigentes y correspondencia exacta del ID histórico con servidor/base/proveedor de esa DA. No leer credenciales de imp01_xmlconexion ni exigir otra conexión en Web.config. | Vínculo manipulado; Configuración ausente | Conexión DA del módulo |
| RQ-03 | DTO por ID, paginación acotada y destino imp_02_ validado en information_schema. CREAR admite destino ausente; AGREGAR/REEMPLAZAR requieren columnas existentes. Guardar no altera destino. | Consulta acotada; Destino inválido | Contrato y metadatos |
| RQ-04 | Transacción InnoDB para cabecera y campos; bloqueo y comparación de revisión más huella vigente. Un ganador, conflicto para el segundo, rollback completo ante fallo. | Concurrencia; Fallo de mapeo; Cambio antiguo | Guardado atómico |
| RQ-05 | Conservar ruta histórica, ID de mapeos y transformaciones no cambiadas. Snapshot v1 canónico DOC121; NULL explícito, orden ordinal, SHA256. Leer sin clave permitido; guardar exige clave mapeada. Flags inconsistentes dan error controlado. | Ruta histórica; Alta web; Flags y clave; Canonización | Compatibilidad histórica |
| RQ-06 | POST JSON, mismo origen, CSRF propio ligado a sesión/usuario/empresa; bootstrap valida origen antes de emitir token. DTO de respuesta tipados. Límites positivos con defaults 10000/10MB decimal. | Solicitud forjada; Límites | ASMX y solicitud |
| RQ-07 | Build aislado y pruebas nativas sintéticas; E2E reutiliza tools/e2e y requiere autorización específica. Instalación SQL manual y datos EGRESOS intactos. | Prueba sintética; Operación real | Verificación y límites |

## Resultado del refinamiento
Diseño/spec/tareas cubren D-01 a D-07 y RQ-01 a RQ-07. Sin marcadores de decisión pendientes. Enrutamiento del módulo y validación real se registran aparte; SQL manual y EGRESOS preservados.

Aceptación posterior2026-11-22: E2E real2/2 PASS,23 controles exclusivamente SELECT,14 llamadas ASMX, una plantilla inactiva propia/dos mapeos, configuración y datos previos preservados.14/14 tareas. Detalle y límites en Doc/Actualizacion/Importacion/Validacion/doc122-e2e-real-20261122.md/json.
