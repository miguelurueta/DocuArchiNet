<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07 -->

## Context

DOC-79 cierra la transición verificable de Importar Servicio Web bajo el gate canónico. El repositorio ya contiene contratos, módulos frontend, pruebas focales y una plataforma E2E reutilizable; faltan consolidación, cobertura específica de UI/gate y criterios de retiro legacy.

## Goals / Non-Goals

**Goals**
- Validar localmente los contratos frontend y de coexistencia sin red.
- Demostrar gate backend completo y alternancia UI reversible.
- Consolidar evidencia E2E existente y documentar inventario/rollback.

**Non-Goals**
- Eliminar controles, handlers, ASMX o almacenamiento legacy.
- Crear infraestructura E2E, perfiles, autenticación o configuración paralela.
- Activar el gate o ejecutar pruebas autenticadas sin autorización explícita.

## Decisions

### D-01 — Validador local único
`tools/validation/Verify-ImportarServicioWebFrontend.ps1` compondrá las suites Node focales sin autenticación ni red. Las cuatro suites canónicas agregarán cobertura reutilizando contratos existentes. Relacionado con RQ-01.

### D-02 — Gate completo en servidor
La validación comprobará las ocho operaciones modernas, el corte anterior a dependencias/efectos, `FEATURE_DISABLED` y la restricción por usuario y grupo. El gate visual nunca será evidencia suficiente de autorización. Relacionado con RQ-02.

### D-03 — Alternancia UI sin doble ejecución
La vista conservará el árbol legacy como fallback. Cuando el gate moderno esté activo se ocultará inicialmente ese árbol y existirá una sola entrada y un solo handler efectivo; apagado conservará el recorrido anterior. Relacionado con RQ-03.

### D-04 — Contrato de ejecución visible
Las pruebas verificarán una llamada `ExecuteImportIntent` por intención, espera global sin porcentajes ficticios y reconciliación final de todos los documentos confirmados sin duplicados. Relacionado con RQ-04.

### D-05 — E2E compartida y autorizada
Se extenderá únicamente la suite compartida cuando falte una aserción estructural. Se reutilizarán escenarios y evidencia disponibles; cualquier corrida real seguirá el runbook y restaurará el gate en `finally`. Relacionado con RQ-05.

### D-06 — Retiro legacy separado
DOC-79 producirá inventario de controles, postbacks y handlers, clasificando referencias y evidencia. La eliminación física será otro cambio con autorización propia. Relacionado con RQ-06.

### D-07 — Documentación canónica
El paquete técnico vivirá solo en `Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-79-pruebas-gate-transicion-legacy/`, con evidencia saneada y diagrama. No se recreará `docs/`. Relacionado con RQ-07.

## Risks / Trade-offs

- Las pruebas estructurales pueden acoplarse a texto: se limitarán a contratos públicos e invariantes de seguridad.
- Ocultar legacy sin eliminarlo conserva superficie técnica, pero mantiene rollback inmediato.
- Si la evidencia E2E previa no cubre una aserción nueva, se declarará el bloqueo y se solicitará autorización, sin simular resultados.

## Migration and Rollback

1. Incorporar primero pruebas y validador local.
2. Aplicar el ocultamiento reversible sin borrar controles legacy.
3. Validar con gate apagado y activado solo en un ambiente autorizado.
4. Restaurar siempre `WorkflowCentroTrabajoModernActive=false`, usuarios y grupos vacíos.
5. Ante regresión, apagar el gate y conservar el recorrido legacy; no revertir datos ni reejecutar intenciones.
