# Tasks — SCRUMCORE-203 (Documentación enterprise AppVisorEmbedPdf)

## Prompt base (plantilla obligatoria)

Se usa como referencia para este SCRUM (adaptado a ruta `docs/` del repo):

```txt
docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/
```

Documentos obligatorios a generar:

- Carpeta: `docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/`
- `SCRUM-SCRUMCORE-203-Metadata.md`
- `SCRUM-SCRUMCORE-203-Objetivo-General.md`
- `SCRUM-SCRUMCORE-203-Responsabilidades-del-Componente.md`
- `SCRUM-SCRUMCORE-203-Arquitectura-Tecnica.md`
- `SCRUM-SCRUMCORE-203-Informacion-Tecnica-del-Componente.md`
- `SCRUM-SCRUMCORE-203-APIs-Utilizadas.md`
- `SCRUM-SCRUMCORE-203-Comportamiento-del-Componente.md`
- `SCRUM-SCRUMCORE-203-Testing-Enterprise.md`
- `SCRUM-SCRUMCORE-203-Evidencias-Tecnicas.md`

## Checklist

### Artefactos (antes de implementar)
- [x] Confirmar alcance exacto de SCRUMCORE-203 (docs-only para `AppVisorEmbedPdf`).
- [x] Crear carpeta destino: `docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/SCRUM-SCRUMCORE-203-Documentacion-AppVisorEmbedPdf/`.
- [x] Dejar los documentos del SCRUM directamente en `docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/` (sin subcarpeta).

### Documentación obligatoria (generación)
- [x] Generar `docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/SCRUM-SCRUMCORE-203-Metadata.md` (Jira + Git + CI/CD).
  - Debe incluir únicamente: Ticket ID, Sprint, Responsable, Fecha, Estado, Branch, Commit.
- [x] Generar `docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/SCRUM-SCRUMCORE-203-Objetivo-General.md`.
  - Debe incluir: qué se documenta, problema que resuelve, alcance funcional, objetivo arquitectónico, impacto técnico, resultado esperado.
- [x] Generar `docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/SCRUM-SCRUMCORE-203-Responsabilidades-del-Componente.md`.
  - Debe incluir: responsabilidades principales, encapsulación, qué NO hace, límites/restricciones, estrategia de desacoplamiento, responsabilidades del consumer.
- [x] Generar `docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/SCRUM-SCRUMCORE-203-Arquitectura-Tecnica.md` (incluye Mermaid cuando aplique).
  - Debe incluir: estructura modular/capas, hooks, services/adapters, plugins, providers/contexts, rendering pipeline, dependencias, diagramas (flujo/secuencia/estados/componentes).
- [x] Generar `docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/SCRUM-SCRUMCORE-203-Informacion-Tecnica-del-Componente.md`.
  - Debe incluir: ruta/naming, props/types, configuración, responsive, compatibilidad DS, deps, WASM/workers, lazy/virtualización, memoization, estrategia de rendering.
- [x] Generar `docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/SCRUM-SCRUMCORE-203-APIs-Utilizadas.md`.
  - Debe incluir: endpoints/HTTP/contratos; si no aplica: “N/A” + rationale + qué debería documentarse al agregar APIs.
- [x] Generar `docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/SCRUM-SCRUMCORE-203-Comportamiento-del-Componente.md`.
  - Debe incluir: lifecycle, estados (loading/error/empty/success/fallback), manejo de errores, responsive, rerenders, cleanup/unmount, memoria/performance.
- [x] Generar `docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/SCRUM-SCRUMCORE-203-Testing-Enterprise.md`.
  - Debe incluir: unit/integration/e2e (Playwright), visual regression, rerender testing, performance, a11y; escenarios/fixtures/mocks/resultados/cobertura/evidencias.
- [x] Generar `docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/SCRUM-SCRUMCORE-203-Evidencias-Tecnicas.md`.
  - Debe incluir: links a screenshots/reportes/traces/videos/logs/métricas/coverage/performance (si existen) o “pendiente en CI” con plan.

### Validación (con tu aprobación antes de correr comandos)
- [ ] Verificar si Playwright está disponible en el repo (`package.json` + config) y si requiere instalación/descarga de browsers.
- [ ] Requerido: instalar Playwright/browsers si falta (comando típico: `npx playwright install`) y dejarlo documentado en `...-Testing-Enterprise.md`.
- [ ] Pedir aprobación para correr `npm.cmd test` (Vitest) y adjuntar resultados a `...-Testing-Enterprise.md` / `...-Evidencias-Tecnicas.md`.
- [ ] Pedir aprobación para correr Playwright (`npm.cmd run test:e2e`) y adjuntar evidencia (si aplica para este SCRUM).
- [x] Requerido: agregar escenario Playwright re-render `playwright/appvisorEmbedPdfRerender.spec.ts` y documentar resultados.
- [x] Instalar browsers Playwright (si falta) y ejecutar Playwright E2E del escenario de re-render (evidencia en docs).
- [x] Ejecutar Vitest focalizado (evidencia en docs).

### Entrega
- [ ] `opsxj:archive SCRUMCORE-203` (PR).
- [ ] Tras merge: `opsxj:close SCRUMCORE-203`.
