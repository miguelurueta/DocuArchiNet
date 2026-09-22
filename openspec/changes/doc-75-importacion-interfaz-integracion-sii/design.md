<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06 -->
## Context

DOC-75 compone en frontend los contratos B03, B09 y B11 ya presentes. La lista/preview modernos existen; falta capturar tipología, preparar uno o varios elementos y crear una intención sin ejecutarla.

## Goals / Non-Goals

**Goals**: unificar preparación individual/múltiple; consumir catálogo, preflight y creación idempotente; conservar selección y foco.

**Non-Goals**: ejecutar o persistir desde frontend, consultar SII en preflight, modificar almacenamiento, expedientes, índices o mutadores legacy.

## Decisions

### D-01 — Colección única
`importar-servicio-web-preparation.js` recibe siempre una colección; individual contiene exactamente una fila y múltiple solo la selección explícita.

### D-02 — Estado aislado y cerrado
`importar-servicio-web-requirements.js` modela edición, preparando, listo, creando, creado y bloqueado. Confirmar exige tipología completa y `Executable=true`.

### D-03 — Backend como autoridad
La UI consume catálogo, `Requirements`, `Commands`, `ContextFingerprint` y `EffectPlans`; no fabrica IDs, requisitos, huellas ni efectos.

### D-04 — Cliente delgado e idempotente
`importar-servicio-web-intent-client.js` reutiliza el API existente para preflight y una sola creación por colección, deduplica concurrencia y nunca ejecuta la intención.

### D-05 — Popup secundario aditivo
El popup reutiliza modal/CSS modernos, conserva fila, selección, filtros, scroll y foco. Cancelar no muta; `Guardar todas` no inicializa preparación implícita.

### D-06 — Dependencias y semántica seguras
B03/B09/B11 ausentes, respuestas inválidas o `PREFLIGHT_STALE` bloquean. El resumen rotula efectos como previstos y nunca muestra `ExpedientId`.

## Risks / Trade-offs

- Catálogo o plan no disponibles bloquean el recorrido.
- Un preflight obsoleto exige preparar de nuevo.
- La integración anidada exige restauración de foco cuidadosa.

## Migration Plan

1. Agregar módulos/pruebas con gate apagado.
2. Integrar markup, CSS y scripts aditivamente.
3. Ejecutar focales, regresión y E2E solo con autorización.
4. Rollback retirando exclusivamente la integración aditiva.

## Open Questions

- La habilitación productiva continúa condicionada a evidencia de B11 en el ambiente objetivo.
