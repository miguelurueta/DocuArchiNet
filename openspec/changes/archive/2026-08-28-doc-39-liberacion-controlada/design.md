<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07 -->
## Context

DOC-39 es la etapa 05 de la modernización de Devolver → Usuario anterior en Workflow ASP.NET Web Forms. DOC-38 dejó evidencia local, compilación, QA manual y una recomendación técnica apta para esta etapa; ninguna de esas evidencias concede autorización de despliegue en GESTOR ni en otro ambiente.

## Goals / Non-Goals

**Goals**

- Producir una decisión única y auditable de liberación.
- Preparar una matriz por ambiente sin secretos y un runbook reversible.
- Conservar la ruta moderna exclusiva y el aislamiento de respuestas al preparar la operación.

**Non-Goals**

- Desplegar, editar configuración, ejecutar E2E/carga, confirmar una transición real o almacenar credenciales.

## Decisions

### D-01 — Línea base de evidencia y versión

La liberación solo puede partir de la evidencia DOC-38 aprobada y de una versión identificada por artefacto o commit aprobado. La aprobación de pruebas se registra como precondición, no como autorización de ambiente.

### D-02 — Matriz de autorización independiente por ambiente

Cada ambiente requiere su propia fila con autorización explícita, versión, alcance, ventana, responsables, evidencia y plan de continuación. La matriz no contiene secretos y una aprobación no se reutiliza entre ambientes.

### D-03 — Runbook sin ejecución implícita

El runbook describe una operación que solo podrá ejecutar un responsable autorizado en la ventana aprobada. DOC-39 redacta el procedimiento, pero no despliega, no habilita gates y no modifica configuración.

### D-04 — Controles de solo lectura y evidencia saneada

Las verificaciones previas y posteriores permitidas son consultas `SELECT` aprobadas y comprobaciones sanitizadas de versión, contrato, auditoría, historial, token y lock. No se usan credenciales ni se registran datos sensibles.

### D-05 — Reversión por gestión de despliegue

La reversión se limita al artefacto desplegado y se ejecuta mediante el proceso de despliegue aprobado. Afecta solo nuevos intentos; no revierte transiciones confirmadas ni modifica tareas, estados o auditoría.

### D-06 — Preservación de contratos y ruta moderna

La preparación confirma que Usuario anterior conserva la ruta moderna oficial y que Actividad anterior, Continuar flujo, Enviar a usuario y Enviar a grupo mantienen contratos y comportamientos independientes.

### D-07 — Decisión de salida explícita

Mientras falten autorización de ambiente, ventana o responsables, la única decisión válida es **solicitar aprobación**. Solo una matriz completa y autorizada permite cambiar la decisión a **lista para despliegue autorizado**; un control crítico fallido obliga a **bloquear**.

## Risks / Trade-offs

- Una matriz sin datos de ambiente no autoriza la operación; evita que la evidencia técnica se interprete como despliegue implícito.
- Los controles `SELECT` no prueban una transición real; esa limitación queda explícita y no habilita E2E ni carga.
- La reversión del despliegue no corrige ni revierte acciones históricas de usuarios; protege la consistencia de Workflow.

## Verification Design

La verificación de DOC-39 es documental y de solo lectura. Comprueba trazabilidad DOC-38, consistencia de versión, completitud de la matriz, contenido del runbook y límites de reversión; valida que no se hayan realizado cambios de ambiente ni operaciones mutantes.
