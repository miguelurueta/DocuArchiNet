<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09 -->
## Context

DOC-54 agrega lectura autoritativa sobre intenciones/resultados DOC-52/DOC-53. El Get actual proyecta items persistidos, pero aún no confirma de forma inequívoca documento y relación con la tarea original.

## Goals / Non-Goals

**Goals:** reconciliar intención completa o item focal; autorizar por contexto persistido; detectar relaciones ausentes, duplicadas o cruzadas; devolver lista mínima, saneada y deduplicada.

**Non-Goals:** modificar escritura, ASMX/JavaScript, `insert_row_documento_relacionado`, `dato_lista`, `AlmacenaDocumentoTareaWorkflow`, cachés SII o el adaptador Backend 06.

## Decisions

### D-01 — Servicio único y autorización opaca
`ServicioReconciliacionImportacion` revalida contexto y exige coincidencia con usuario/tarea de la intención. Ausente y no autorizado comparten respuesta segura.

### D-02 — Repositorio de lectura especializado
`MySqlImportReconciliationRepository` usa la infraestructura de datos compartida y parámetros para componer intención, items, documento y relación. No escribe ni usa Session.

### D-03 — Identidad focal compuesta
La búsqueda focal usa intención, proveedor e identidad externa; la tarea autoritativa procede de la intención persistida.

### D-04 — Confirmación estricta
Solo `DocumentId` con persistencia conocida y relación única a la tarea original produce `Disponible`; ausencia, duplicado, parcialidad o cruce queda inconsistente.

### D-05 — Mapping total y puro
`ImportItemResultMapper` traduce cada combinación de fase/consistencia a un estado único, sin consultar datos ni fabricar autoridad.

### D-06 — Incertidumbre conservadora
Timeout o lectura incompleta se expone como `Verificando`/`ResultadoIncierto`; no se infiere disponibilidad ni reintento seguro.

### D-07 — Contrato v1 mínimo
El DTO se extiende compatiblemente con fase y mínimos documentales; excluye rutas, secretos, excepciones, SQL y `dato_lista`.

### D-08 — Duplicados y correlación
Se deduplica por `(TaskId, DocumentId)`; filas incompatibles quedan inconsistentes y conservan correlación.

### D-09 — Entrega aditiva y rollback
Se agregan servicio, mapper, repositorio, contratos, tests, fixtures, `.vbproj` y documentación canónica. El rollback retira esas adiciones.

## Risks / Trade-offs

- Los nombres exactos de tablas legacy se verificarán por inspección durante implementación; el SQL queda encapsulado.
- Ante duplicados se prefiere visibilidad conservadora a seleccionar una fila arbitraria.
- La lectura compuesta debe filtrar primero la intención autorizada y aprovechar índices existentes antes de proponer DDL.

## Migration Plan

1. Extender modelo/DTO y definir snapshot.
2. Implementar repositorio read-only y mapper.
3. Integrar Get/Reconcile y registrar archivos.
4. Ejecutar pruebas, build, auditoría y documentación.
5. Rollback retirando adiciones; no hay migración de datos.

## Open Questions

- Ninguna bloqueante; nombres legacy se resolverán por lectura de código, sin consultas reales ni DDL.
