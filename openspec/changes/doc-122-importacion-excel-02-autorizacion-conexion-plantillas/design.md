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

Existentes: Gestion/Class_remit_dest_interno_perfil_produccion.vb y GestionDocumental-Docuarchi.net.vbproj. Reutilizar resolución por sesión y comparación de tokens. No modificar SII.

## Decisions
### D-01 Autorización vigente propia
Consultar RA en cada operación: usuario activo, empresa y permiso exacto ADMINISTRAR_CARGAS_IMPORTACION=1. Indicador de sesión solo para presentación.
Evidencia: PerfilImportacionContextAdapter.Resolver/AutorizarFuentes.

### D-02 Conexión dedicada
Vincular empresa e ID histórico con connectionStrings protegido; comprobar servidor/base con sesión DA_ e imp01_xmlconexion. Sin fallback ni credenciales históricas.
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
ImportacionExcel.Destinos: JSON [{empresaId,conexionId,nombre,connectionStringName}]. appSettings y connectionStrings deben usar ASP.NET Protected Configuration. Sin secretos ni cambios en Web.config durante implementación. Límites ImportacionExcel.MaxFilasDatos y MaxArchivoMB positivos. Nueva ruta web/<guid>.xls[x]; resolución física privada pertenece a fase03. No interpretar rutas históricas enviadas por navegador.

## Risks / Trade-offs
Si catálogos no son InnoDB, guardado falla cerrado; adecuación mediante SQL incremental manual. Bloqueo detecta modificaciones previas; editores antiguos pueden escribir después del commit, por eso preparación/confirmación posteriores deben revalidar huella. No afirmar protección de escritores ajenos al protocolo.

## Migration Plan
Sin cuenta dedicada configurada la capacidad queda cerrada. Conservar SQL00–09 DOC121 y datos. Build fuera de bin. Rollback del código conserva esquema aditivo.

## Open Questions
Sin decisión funcional pendiente. Configuración protegida y autorización de E2E real pendientes operativos, no impiden implementar ni probar localmente.
