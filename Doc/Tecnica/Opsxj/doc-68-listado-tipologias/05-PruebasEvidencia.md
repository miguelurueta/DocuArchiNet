# LISTADO-TIPOLOGIAS

- Ticket: DOC-68
- Cambio OpenSpec: doc-68-listado-tipologias
- Clasificacion: cross_cutting (Transversal)
## Evidencia requerida

- [x] `unit` — 2026-09-19: `node --test` sobre contratos, mapper, presentación, catálogo, conteo y composición; 17/17 pruebas aprobadas. Referencia: `Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-68-listado-enriquecido-catalogo-tipologias/06-PruebasEvidencia.md`.
- [x] `manual_qa` — 2026-09-19: E2E autenticada `import-sii-read` aprobada para MERCANTIL, ESAL y RUP; metadatos, estado, acciones, tipologías, cardinalidad y filtro de sellos verificados. Referencia: `tools/e2e/artifacts/workflow-e2e-platform-import-sii-read.json` y el paquete técnico DOC-68.

## QA/E2E WebForms

Se reutilizó el runner oficial:

```text
npm.cmd --prefix tools/e2e run test:workflow:platform -- --scenario import-sii-read --profile <perfil-no-sensible> --authorize environment,gate,local-tls
```

Resultados: MERCANTIL, ESAL y RUP `PASSED`; cuatro operaciones contractuales sin error en cada corrida aprobatoria, siete controles exclusivamente `SELECT` sin cambios, cero recursos mutados y gate restaurado a `false` con usuarios/grupos vacíos. La tarea MERCANTIL multiinscripción confirmó tres sellos 505; ESAL y RUP confirmaron un sello cada una. La evidencia pública se saneó y no conserva credenciales, cookies, conexiones ni payload externo crudo.
