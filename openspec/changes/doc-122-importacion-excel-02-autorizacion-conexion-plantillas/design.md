<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07 -->
## Context
DOC-122, fase02 de7. Prompt02 íntegro en jira-context.md y contratos DOC121 constituyen la base elegida por el usuario.

## Goals / Non-Goals
Acceso y administración de plantillas. No incluye upload, lector, preparación, confirmación ni UI. EGRESOS es exclusivamente ejemplo de pruebas.

## Inventario previo a edición
Nuevos archivos contrastados con las carpetas existentes; registrar compilación y contenido en vbproj:
- `Modelo/ProduccionDocumental/ImportacionExcel/ImportacionExcelModelos.vb`
- `DTOs/ProduccionDocumental/ImportacionExcel/ImportacionExcelDtos.vb`
- `Services/ProduccionDocumental/ImportacionExcel/ServicioPlantillasExcel.vb`
- `Infrastructure/Repositories/ProduccionDocumental/ImportacionExcel/MySqlPlantillaExcelRepository.vb`
- `Infrastructure/ProduccionDocumental/ImportacionExcel/ImportacionExcelContextAdapter.vb`
- `Infrastructure/ProduccionDocumental/ImportacionExcel/ImportacionExcelConexionAdapter.vb`
- `Infrastructure/ProduccionDocumental/ImportacionExcel/ImportacionExcelRequestGuard.vb`
- `webservice/WebServiceImportacionExcel.asmx`
- `webservice/WebServiceImportacionExcel.asmx.vb`
- `tests/ImportacionExcelBehaviorTests.cs`
- `tests/doc122-importacion-excel-behavior.test.cjs`
- `tools/e2e/tests/importacion-excel.spec.cjs`

Existentes: Gestion/Class_remit_dest_interno_perfil_produccion.vb y GestionDocumental-Docuarchi.net.vbproj. Inspección de conexión previa al ajuste: Defaul/GestorModuleSesion.vb.AsignaAtributosConexionModuloDocuarchi carga DA_ID_EMPRESA/DA_ID_MODULO y cuenta desde gestor_modulos; Infrastructure/Shared/Data/ModuleSessionConnectionStringResolver.vb.Resolve materializa la conexión; Defaul/conect.vb.Dbase_Conction_Mysql_DA usa el mismo origen. Se adapta ImportacionExcelConexionAdapter y sus pruebas; el resolver compartido y los cargadores legacy permanecen intactos. Reutilizar resolución por sesión y comparación de tokens. No modificar SII.

Reutilización E2E solicitada: extender tools/e2e/scripts/run-doc110-profile-session.cjs, inspeccionado junto con interactive-e2e-console.cjs, para modos Excel de lectura/escritura. Conserva perfil local DOC88, autenticación y safePost existentes. Verificación focal en tests/doc122-e2e-reuse.test.cjs; no hay runner paralelo ni nuevas credenciales persistidas.

## Decisions
### D-01 Autorización vigente propia
Consultar RA en cada operación: usuario activo, empresa y permiso exacto ADMINISTRAR_CARGAS_IMPORTACION=1. Indicador de sesión solo para presentación.
Evidencia: PerfilImportacionContextAdapter.Resolver/AutorizarFuentes.

### D-02 Conexión DA del módulo
Resolver DA mediante ModuleSessionConnectionStringResolver y la sesión del módulo cargada desde gestor_modulos. Exigir empresa/módulo vigentes y correspondencia exacta del ID histórico con servidor/base/proveedor de esa DA. No leer credenciales de imp01_xmlconexion ni exigir otra conexión en Web.config.
Evidencia: ModuleSessionConnectionStringResolver.Resolve.

### D-03 Contrato y metadatos
DTO por ID, paginación acotada y destino imp_02_ validado en information_schema. CREAR admite destino ausente; AGREGAR/REEMPLAZAR requieren columnas existentes. Guardar no altera destino.
Evidencia: Class_imp01_plantillaimp / Class_imp01_campos_plantilla.

### D-04 Guardado atómico
Transacción InnoDB para cabecera y campos; bloqueo y comparación de revisión más huella vigente. Un ganador, conflicto para el segundo, rollback completo ante fallo.
Evidencia: MySqlPerfilImportacionRepository.

### D-05 Compatibilidad histórica
Conservar ruta histórica, ID de mapeos y transformaciones no cambiadas. Snapshot v1 canónico DOC121; NULL explícito, orden ordinal, SHA256. Leer sin clave permitido; guardar exige clave mapeada. Flags inconsistentes dan error controlado.
Evidencia: Doc/Actualizacion/Importacion/Diseno/01-contratos-configuracion-doc121.md.

### D-06 ASMX y solicitud
POST JSON, mismo origen, CSRF propio ligado a sesión/usuario/empresa; bootstrap valida origen antes de emitir token. DTO de respuesta tipados. Límites positivos con defaults 10000/10MB decimal.
Evidencia: PerfilImportacionRequestGuard / WebServiceImportacionCarpetas.

### D-07 Verificación y límites
Build aislado y pruebas nativas sintéticas; E2E reutiliza tools/e2e y requiere autorización específica. Instalación SQL manual y datos EGRESOS intactos.
Evidencia: AGENTS.md / tools/e2e/AGENT-RUNBOOK.md.

## Configuración
Actualización aprobada por el usuario al pedir continuar con los recursos de conexión del repositorio: usar la cuenta del módulo DA, sin exigir otra cuenta dedicada. El ajuste sustituye la decisión inicial Web.config/configuración duplicada y queda reflejado en delta DOC121. Límites ImportacionExcel.MaxFilasDatos y MaxArchivoMB opcionales/positivos en appSettings; defaults10000/10 sin exigir cifrar secciones. Nueva ruta web/<guid>.xls[x]; resolución física privada pertenece a fase03.

## Risks / Trade-offs
La cuenta de conexión se comparte con el módulo. Ya no se afirma separación física de privilegios por cuenta ni se rechaza automáticamente la cuenta del módulo por su nombre; los límites funcionales se mantienen por autorización y validación de destino. No se elevan ni revocan privilegios existentes. Un cambio de empresa, módulo o configuración de sesión invalida la fábrica de repositorio capturada.

Si catálogos no son InnoDB, guardado falla cerrado; adecuación mediante SQL incremental manual. Bloqueo detecta modificaciones previas; editores antiguos pueden escribir después del commit, por eso preparación/confirmación posteriores deben revalidar huella. No afirmar protección de escritores ajenos al protocolo.

## Migration Plan
Sin sesión DA de la empresa/módulo o sin correspondencia del ID histórico la capacidad queda cerrada. No modificar credenciales ni privilegios de gestor_modulos. Conservar SQL00–09 DOC121 y datos. Build fuera de bin. Rollback del código conserva esquema aditivo.

## Open Questions
Sin decisión funcional pendiente. Validación del enrutamiento DA y autorización de E2E real pendientes operativos, no impiden implementar ni probar localmente.
