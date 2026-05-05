## Context

Actualmente existe (o se planea integrar) un visor PDF basado en `@embedpdf/*` bajo `src/app/Components/UI/AppVisorEmbedPdf`. El ticket SCRUMCORE-200 requiere eliminar completamente ese componente y todo lo relacionado (código, dependencias, wiring en UI/rutas/tests y documentación asociada).

El frontend es una SPA React+Vite. Se debe mantener TypeScript estricto y la arquitectura por módulos. Las pruebas unitarias usan Vitest.

## Goals / Non-Goals

**Goals:**
- Eliminar `src/app/Components/UI/AppVisorEmbedPdf` y cualquier export/import/uso directo o indirecto en `src/**`.
- Remover dependencias `@embedpdf/*` del `package.json` y actualizar `package-lock.json`.
- Asegurar que la app compila y los tests relevantes pasan (o documentar fallos no relacionados).
- Mantener la experiencia de usuario consistente: si había UI que abría el visor, debe degradar de forma controlada (remover opción o usar alternativa existente si aplica).

**Non-Goals:**
- Implementar un nuevo visor PDF alternativo (solo eliminar el actual).
- Cambiar arquitectura global (router/layout) o migrar stack (Vite/React/TS).
- Refactor masivo de módulos no relacionados.

## Decisions

### 1) Eliminar `@embedpdf/*` como dependencia de runtime
**Decision:** Remover `@embedpdf/*` de `dependencies` y limpiar el lockfile.

**Rationale:** El requerimiento del ticket es eliminar el componente y “todo lo relacionado”; mantener dependencias sin uso aumenta el bundle, superficie de mantenimiento y complejidad.

**Alternatives considered:**
- Mantener dependencias por “posible reutilización futura”: rechazado porque contradice el objetivo del ticket y deja deuda técnica.

### 2) Eliminar wiring del visor en UI antes que “dejar stubs”
**Decision:** Remover entradas de UI/rutas/botones/tests que dependan del visor, preferiblemente ocultando o removiendo la acción.

**Rationale:** Stubs o componentes vacíos suelen generar rutas rotas y errores de ejecución. Es mejor eliminar el flujo completo o degradarlo explícitamente.

**Alternatives considered:**
- Reemplazar por un placeholder: solo si alguna pantalla exige un “visor” por contrato. En ausencia de ese contrato, se elimina el entry-point.

## Risks / Trade-offs

- [Riesgo] Se rompen pantallas que asumían visor disponible → **Mitigación**: buscar referencias (imports/strings) y ajustar UI para ocultar la acción o usar alternativa (ej: descarga/preview existente).
- [Riesgo] Quedan referencias indirectas (tests, exports barrel, lazy imports) → **Mitigación**: `rg` global por `AppVisorEmbedPdf`, `embedpdf` y `@embedpdf`, y ejecutar `build` + tests.
- [Trade-off] Remover dependencia puede cambiar bundle y tree-shaking → **Mitigación**: validar `vite build` y verificar que no existan imports residuales.

## Migration Plan

1. Identificar todos los entry-points del visor (botones, rutas, menús, servicios) y removerlos/ajustarlos.
2. Eliminar la carpeta `src/app/Components/UI/AppVisorEmbedPdf`.
3. Remover `@embedpdf/*` de `package.json`.
4. Ejecutar `npm install` para actualizar `package-lock.json`.
5. Validar:
   - `npm run build`
   - `npm test`
6. Si existía documentación interna del visor, moverla a “archived/removed” o eliminarla según política del repo.

**Rollback:** Revertir el commit/PR que elimina el visor y restaurar dependencias desde Git.

## Open Questions

- ¿Hay algún flujo que requiera mantener un visor PDF (aunque no sea EmbedPdf) como parte de un requerimiento funcional actual?
- ¿Existe una alternativa ya aprobada en el proyecto (ej: `react-pdf`, descarga directa, visor nativo del navegador) que se deba usar en lugar de eliminar la acción?

## Validation Evidence (2026-05-05)

- `npm.cmd run build`: falla por errores TypeScript en `AppEditor` y `gestionCorrespondencia` (no relacionados con EmbedPdf).
- `npm.cmd test`: ejecuta y reporta fallos en suites de `AppEditor`/`gestionCorrespondencia`/`radicacion` (sin referencias a `AppVisorEmbedPdf` / `@embedpdf`).
