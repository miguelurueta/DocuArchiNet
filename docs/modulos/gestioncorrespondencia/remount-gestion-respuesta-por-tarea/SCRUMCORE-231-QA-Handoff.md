# SCRUMCORE-231 - QA Handoff

Fecha: 2026-06-05

## 1) Objetivo del handoff

Entregar SCRUMCORE-231 a validacion QA/e2e con implementacion completa, evidencia local verde y pendientes enterprise explicitamente trazados.

El alcance del handoff es validar en entorno real que el remount por cambio de tarea aisla completamente el estado de `GestionRespuesta` y no introduce regresiones en documentos, visor, arbol, adjuntos ni navegacion.

## 2) Estado para Scrum

- Implementacion: completa.
- Validacion local: completa para alcance automatizado focalizado.
- Validacion QA/e2e: pendiente.
- Cierre Jira: pendiente.
- Archive OpenSpec: pendiente.

## 3) Evidencia disponible antes de QA

- Vitest focalizado: 9 archivos, 50 tests, 50 passed.
- ESLint focalizado: pass sin hallazgos.
- Playwright discovery: disponible, 20 tests en 15 archivos.
- Bloqueo e2e local: falta configuracion `PLAYWRIGHT_LOGIN_EMPRESA_ID`.

## 4) Configuracion requerida para QA/e2e

Variables obligatorias:

- `PLAYWRIGHT_LOGIN_EMPRESA_ID`
- `PLAYWRIGHT_LOGIN_MODULO_ID`
- `PLAYWRIGHT_LOGIN_USER`
- `PLAYWRIGHT_LOGIN_PASSWORD`

Variable opcional:

- `PLAYWRIGHT_API_URL`

Default observado:

```text
http://localhost/DocuArchiApi
```

## 5) Comandos sugeridos para QA

### 5.1 Regresion e2e focalizada SCRUMCORE-231

```powershell
npx.cmd playwright test playwright/gestionCorrespondencia/gestionRespuesta.estructura934.spec.ts playwright/gestionCorrespondencia/documentosWorkbench.visual229.spec.ts playwright/gestionCorrespondencia/documentosWorkbench.smoke.spec.ts playwright/gestionCorrespondencia/documentosWorkbench.radicado230.spec.ts playwright/gestionCorrespondencia/documentosWorkbench.columnas225.spec.ts playwright/appvisorEmbedPdfThumbnails.spec.ts
```

### 5.2 Smoke aislado de DocumentosWorkbench

```powershell
npx.cmd playwright test playwright/gestionCorrespondencia/documentosWorkbench.smoke.spec.ts --reporter=line --workers=1 --timeout=30000
```

### 5.3 Regresion completa e2e disponible

```powershell
npm.cmd run test:e2e
```

## 6) Checklist funcional QA

- Entrar al modulo de gestion correspondencia desde bandeja.
- Abrir una tarea en `/dashboard/gestion-correspondencia/respuesta/:idA`.
- Confirmar que el detalle carga en estado correcto.
- Confirmar que tabs de `GestionRespuesta` operan sin contenido stale.
- Confirmar que `DocumentosWorkbench` muestra documentos de la tarea actual.
- Confirmar que el visor PDF corresponde a la seleccion actual.
- Confirmar que el arbol/listado no conserva seleccion de una tarea anterior.
- Cambiar a `/dashboard/gestion-correspondencia/respuesta/:idB`.
- Confirmar remount completo del detalle.
- Confirmar que no persisten `files`, `activeRowId`, `activeFileUrl`, editor ni adjuntos de `idA`.
- Cambiar rapidamente entre `idA`, `idB` e `idC`.
- Confirmar que respuestas async antiguas no sobrescriben la tarea activa.
- Validar reload del navegador en detalle.
- Validar comportamiento responsive.
- Validar adjuntos antes y despues del cambio de tarea.
- Validar regresion `SCRUM-205` en thumbnails del visor.

## 7) Criterios de aceptacion QA

SCRUMCORE-231 puede pasar a cierre solo si:

- El cambio de tarea fuerza detalle limpio y coherente.
- No hay contaminacion visual ni de estado entre tareas.
- Tabs, visor, arbol y adjuntos reflejan siempre la tarea activa.
- No aparecen errores por hooks, lifecycle o stale requests.
- La regresion `SCRUM-205` se mantiene estable.
- Los e2e focalizados pasan o QA adjunta evidencia manual equivalente aprobada.

## 8) Criterios de no cierre

No cerrar si:

- Falta configurar credenciales Playwright.
- No existe evidencia QA/e2e real.
- El visor conserva documento de una tarea anterior.
- El arbol conserva seleccion de una tarea anterior.
- Los adjuntos se mezclan entre tareas.
- Hay errores de consola relacionados con lifecycle, hooks, visor o documentos.
- Jira no fue actualizado.
- OpenSpec no fue archivado despues de completar evidencia.

## 9) Resultado esperado posterior a QA

Cuando QA complete evidencia:

- Marcar tareas pendientes de Capa 5 como completadas en `openspec/changes/scrumcore-231-remount-gestion-respuesta-por-tarea/tasks.md`.
- Actualizar ticket SCRUMCORE-231 en Jira con resultado QA/e2e.
- Archivar el change OpenSpec.
- Registrar evidencia final en documentacion enterprise.

## 10) Referencias

- `docs/modulos/gestioncorrespondencia/remount-gestion-respuesta-por-tarea/SCRUMCORE-231-Pendientes-Enterprise.md`
- `docs/modulos/gestioncorrespondencia/remount-gestion-respuesta-por-tarea/SCRUMCORE-231-Pruebas.md`
- `docs/modulos/gestioncorrespondencia/remount-gestion-respuesta-por-tarea/SCRUMCORE-231-Metadata.md`
- `openspec/changes/scrumcore-231-remount-gestion-respuesta-por-tarea/tasks.md`
