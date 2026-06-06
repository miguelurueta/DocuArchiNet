# SCRUMCORE-231 - Pendientes Enterprise

Fecha: 2026-06-05

## 1) Estado ejecutivo

SCRUMCORE-231 esta implementado a nivel de codigo y validado localmente con pruebas automatizadas focalizadas.

El cierre enterprise completo permanece pendiente porque la validacion UI/e2e real requiere entorno QA/browser con credenciales Playwright y posterior cierre administrativo en Jira/OpenSpec.

No se modifico codigo para forzar, omitir, simular o cerrar artificialmente resultados pendientes.

## 2) Estado OpenSpec observado

- Change: `scrumcore-231-remount-gestion-respuesta-por-tarea`
- Schema: `spec-driven`
- Comando:
  - `openspec.cmd instructions apply --change "scrumcore-231-remount-gestion-respuesta-por-tarea" --json`
- Progreso observado:
  - 18/23 tareas completas
  - 5 tareas pendientes

## 3) Tareas pendientes

### 3.1 Capa 5 - Cobertura transversal y regresion

- Ejecutar validacion UI completa: cambio entre tareas desde bandeja, tabs, visor sincronizado y arbol estable.
- Ejecutar validacion de integridad de adjuntos, reload y responsive.
- Validar ausencia de regresion en `AppTable`, `AppTreeTable`, `AppVisorEmbedPdf` y `SCRUM-205`.
- Ejecutar o confirmar pruebas e2e de remount y estado reseteado.

### 3.2 Capa 6 - Trazabilidad y cierre enterprise

- Actualizar estado del ticket en Jira.
- Archivar el change OpenSpec cuando toda la evidencia este completa.

## 4) Evidencia local ejecutada

### 4.1 Vitest focalizado

Comando ejecutado:

```powershell
npx.cmd vitest run src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.spec.test.tsx src/modules/gestionCorrespondencia/tests/GestionCorrespondenciaRoutePage.test.tsx src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx src/modules/gestionCorrespondencia/tests/useGestionRespuestaDocumentos.test.tsx src/modules/gestionCorrespondencia/tests/useEstructuraRespuestaIdTarea.test.tsx src/modules/gestionCorrespondencia/tests/useListaDocumentosRadicadosTreeTable.test.tsx src/modules/gestionCorrespondencia/tests/gestionCorrespondenciaTableRequestMapper.test.ts src/modules/gestionCorrespondencia/tests/solicitaEstructuraRespuestaIdTarea.service.test.ts src/modules/gestionCorrespondencia/tests/workflowInboxAutocomplete.service.test.ts
```

Resultado:

- 9 archivos de prueba ejecutados.
- 50 tests ejecutados.
- 50 tests passed.
- 0 fallos.

Alcance cubierto:

- Ruta `GestionCorrespondenciaRoute`.
- Remount por cambio de `parsedId`.
- Reset de estado local simulado.
- Navegacion rapida consecutiva.
- Integracion local de `DocumentosWorkbench`.
- Hooks documentales relacionados.
- Mappers y servicios relacionados de `gestionCorrespondencia`.

### 4.2 ESLint focalizado

Comando ejecutado:

```powershell
npx.cmd eslint src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.tsx src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.spec.test.tsx src/modules/gestionCorrespondencia/tests/GestionCorrespondenciaRoutePage.test.tsx src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx src/modules/gestionCorrespondencia/tests/useGestionRespuestaDocumentos.test.tsx src/modules/gestionCorrespondencia/tests/useEstructuraRespuestaIdTarea.test.tsx src/modules/gestionCorrespondencia/tests/useListaDocumentosRadicadosTreeTable.test.tsx
```

Resultado:

- Pass sin hallazgos reportados por consola.

## 5) Evidencia Playwright/e2e

### 5.1 Descubrimiento de suite e2e

Comando ejecutado:

```powershell
npm.cmd run test:e2e -- --list
```

Resultado:

- Playwright disponible.
- 20 tests listados.
- 15 archivos de spec detectados.

Specs relevantes para SCRUMCORE-231 y regresion asociada:

- `playwright/gestionCorrespondencia/documentosWorkbench.columnas225.spec.ts`
- `playwright/gestionCorrespondencia/documentosWorkbench.radicado230.spec.ts`
- `playwright/gestionCorrespondencia/documentosWorkbench.smoke.spec.ts`
- `playwright/gestionCorrespondencia/documentosWorkbench.visual229.spec.ts`
- `playwright/gestionCorrespondencia/gestionRespuesta.estructura934.spec.ts`
- `playwright/appvisorEmbedPdfThumbnails.spec.ts`

### 5.2 Intento de regresion e2e relevante

Comando ejecutado:

```powershell
npx.cmd playwright test playwright/gestionCorrespondencia/gestionRespuesta.estructura934.spec.ts playwright/gestionCorrespondencia/documentosWorkbench.visual229.spec.ts playwright/gestionCorrespondencia/documentosWorkbench.smoke.spec.ts playwright/gestionCorrespondencia/documentosWorkbench.radicado230.spec.ts playwright/gestionCorrespondencia/documentosWorkbench.columnas225.spec.ts playwright/appvisorEmbedPdfThumbnails.spec.ts
```

Resultado:

- Ejecucion iniciada con 10 tests usando 4 workers.
- El proceso local finalizo por timeout.
- Se observaron fallos tempranos asociados a entorno real no configurado.
- No se toma como evidencia de pass.
- No se usa para cerrar Capa 5.

### 5.3 Diagnostico e2e aislado

Comando ejecutado:

```powershell
npx.cmd playwright test playwright/gestionCorrespondencia/documentosWorkbench.smoke.spec.ts --reporter=line --workers=1 --timeout=30000
```

Resultado:

- Bloqueo por configuracion faltante.
- Error exacto:

```text
Missing required env var: PLAYWRIGHT_LOGIN_EMPRESA_ID
```

## 6) Variables requeridas para cierre QA/e2e

Variables obligatorias observadas en specs de `gestionCorrespondencia`:

- `PLAYWRIGHT_LOGIN_EMPRESA_ID`
- `PLAYWRIGHT_LOGIN_MODULO_ID`
- `PLAYWRIGHT_LOGIN_USER`
- `PLAYWRIGHT_LOGIN_PASSWORD`

Variable opcional:

- `PLAYWRIGHT_API_URL`

Default observado cuando `PLAYWRIGHT_API_URL` no esta definido:

```text
http://localhost/DocuArchiApi
```

## 7) Criterio de cierre pendiente

Para cerrar Capa 5 se requiere ejecutar con entorno QA/e2e real y conservar evidencia de pass para:

- Flujo bandeja -> detalle.
- Cambio entre tareas.
- Tabs operativos tras cambio de tarea.
- Visor sincronizado con tarea actual.
- Arbol estable sin stale state.
- Adjuntos sin contaminacion entre tareas.
- Reload del detalle.
- Responsive.
- Regresion de `AppTable`.
- Regresion de `AppTreeTable`.
- Regresion de `AppVisorEmbedPdf`.
- Regresion `SCRUM-205`.
- E2E de remount y estado reseteado.

Para cerrar Capa 6 se requiere:

- Evidencia completa de Capa 5.
- Actualizacion del ticket SCRUMCORE-231 en Jira.
- Archive del change OpenSpec.

## 8) Decision de trazabilidad

- No se marcan como completadas las tareas pendientes sin evidencia e2e/QA real.
- No se omite ningun pendiente de Capa 5.
- No se cierra Jira/OpenSpec antes de completar la evidencia.
- No se hicieron cambios de codigo.
- El estado enterprise correcto al 2026-06-05 es: implementacion y validacion local OK; QA/e2e real y cierre administrativo pendientes.

## 9) Archivos de codigo impactados por SCRUMCORE-231

Estos archivos forman parte del alcance tecnico ya implementado y validado localmente:

- `src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.tsx`
- `src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.spec.test.tsx`

No se modificaron archivos de codigo durante esta documentacion de pendientes.

## 10) Riesgos residuales

- Riesgo de estado stale en condiciones reales de red si no se completa e2e/QA.
- Riesgo visual/regresivo en interaccion multi-componente entre `DocumentosWorkbench`, `AppTreeTable` y `AppVisorEmbedPdf`.
- Riesgo de adjuntos o visor con estado previo no observable en unit tests.
- Riesgo de cierre administrativo prematuro si Jira/archive se ejecuta sin evidencia real.
