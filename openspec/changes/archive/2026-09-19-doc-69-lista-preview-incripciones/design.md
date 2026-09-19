<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08 -->
## Context
`GetPreview` descarga actualmente el anexo para calcular longitud, descarta los bytes y devuelve `ExternalKey` como descriptor. No existe frontera de canje. La sesión `InProc` descarta una solución en memoria para granja web.

## Goals / Non-Goals
**Goals:** descriptor opaco; snapshot compartido; una descarga SII; GET/HEAD autenticados; consumo único; streaming seguro; errores opacos; evidencia.

**Non-Goals:** cambiar frontend, handlers legacy, `ClassAlmacenamiento`, ejecución DOC-67, servir URL externa, JSON/base64 o desactivar TLS.

## Architecture
```text
GetPreview -> gate/contexto -> SII (resolver+descargar una vez)
           -> DescriptorService -> MySqlPreviewRepository
HEAD/GET handler -> gate/contexto -> ContentService -> MySqlPreviewRepository
```

## Decisions
### D-01 — Snapshot temporal compartido
Crear `workflow_import_preview_descriptor` en `workflowdocument`: hash, autoridad, metadatos, BLOB, expiración y estado. Es estado técnico, sin efectos de negocio.

### D-02 — Descriptor opaco
Generar 32 bytes aleatorios Base64URL; persistir solo SHA-256. Ligar a usuario Workflow, tarea, proveedor y `ExternalKey`. El cliente no aporta URL, tipo, tamaño ni disposición.

### D-03 — HEAD y GET
`HEAD` lee metadatos sin BLOB, consumo ni SII. `GET` actualiza condicionalmente `Disponible -> Reclamado` dentro de transacción, carga el BLOB y finaliza `Consumido`. No revierte una reclamación incierta.

### D-04 — Política binaria
Máximo configurable 10 MiB. Allowlist inline: PDF, PNG, JPEG y TIFF. Nombre saneado. Headers: tipo, longitud, disposición, `no-store, private`, `no-cache`, `nosniff` y `SAMEORIGIN`.

### D-05 — Descarga única
SII no entrega longitud confiable; `GetPreview` descarga una vez y persiste esos bytes. HEAD/GET no llaman SII y el handler nunca crea cliente externo.

### D-06 — Expiración y errores
TTL predeterminado cinco minutos, acotado. Limpieza oportunista parametrizada. Ausente, vencido, ajeno, alterado o consumido: HTTP 404 vacío; infraestructura: 503 sin detalle. No registrar descriptor/contenido/PII.

### D-07 — Composición y rollback
Handler con `IRequiresSessionState`, mismo context gate y feature gate antes de I/O. Composición compartida. Rollback por gate apagado; legacy sin cambios.

### D-08 — Verificación
`node:test` cubre SQL, autorización, headers, límites, concurrencia y conteo externo; MSBuild registra handler; E2E reutiliza `tools/e2e`, controles `SELECT` y restauración del gate.

## Risks / Trade-offs
- BLOB temporal en DB: mitigado por 10 MiB, TTL e índice.
- Reclamar antes de emitir impide reintento tras corte, pero evita doble exposición.
- TIFF inline no garantiza renderizado del navegador.

## Migration Plan
1. Tabla MySQL 5.1 sin FK cross-database. 2. Repositorio/servicios. 3. Persistir descarga de `GetPreview`. 4. Handler. 5. Suites/build/docs/E2E. Rollback: gate apagado y limpieza de expirados.
