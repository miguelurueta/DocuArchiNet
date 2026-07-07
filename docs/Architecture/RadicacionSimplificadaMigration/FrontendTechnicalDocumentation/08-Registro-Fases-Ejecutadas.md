# Registro De Fases Ejecutadas - Radicacion Simplificada

## Objetivo

Documentar las fases de infraestructura frontend ya ejecutadas en el modulo `src/modules/radicacion`, dejando evidencia de alcance, archivos principales, validaciones y restricciones para las siguientes fases.

Este documento no reemplaza los prompts arquitectonicos. Resume el estado implementado para evitar repetir trabajo o mezclar fases.

## Fases Cerradas

| Fase | Ticket | Prompt | Estado | Resultado |
|---|---|---|---|---|
| TD-FE-01 | SCRUMCORE-290 | `PROMPT-TD-FE-01-Unificar-Fuente-Plantilla-Radicacion.md` | Ejecutado | La plantilla se carga desde el boundary del modulo y se propaga por props. |
| TD-FE-02 | SCRUMCORE-291 | `PROMPT-TD-FE-02-Contexto-Documental-Unico-Guards.md` | Ejecutado | Existe `RadicacionDocumentalContext` unico y `RadicacionDocumentosGuard`. |
| FE-06 | SCRUMCORE-292 | `PROMPT-FE-06-Inicio-Modulo-Estado-Activo-Contexto-Documental.md` | Ejecutado | El modulo consulta `estado-activo`, restaura o limpia contexto y controla el bootstrap. |
| TD-FE-04 | SCRUMCORE-293 | `PROMPT-TD-FE-04-Rutas-Tabs-Limpieza-UI-Prototipo.md` | Ejecutado y mergeado | Rutas/helpers centralizados, tabs semanticas y limpieza de UI mock. |
| FE-05 | SCRUMCORE-298 | `PROMPT-FE-05-Modal-Pendientes-AppTable-Asignacion-Radicado.md` | Ejecutado | Modal de pendientes usa `AppTable`, toma pendiente, actualiza contexto y navega a Documentos. |

## TD-FE-01 - Fuente Unica De Plantilla

### Alcance Implementado

- `RadicacionRoutePage` conserva la responsabilidad de cargar la plantilla.
- `RadicacionPage` recibe `plantilla` y `camposPlantilla`.
- `RadicacionTabs` propaga los datos hacia `RadicacionForm`.
- `RadicacionForm` deja de ser fuente de carga inicial de plantilla.

### Resultado Arquitectonico

```text
RadicacionRoutePage
  -> useCamposPlantilla()
  -> RadicacionPage
  -> RadicacionTabs
  -> RadicacionForm
```

### Restricciones Vigentes

- No reintroducir `useCamposPlantilla()` dentro de componentes internos.
- No duplicar la carga de `/api/PlantillaRadicado/listaPlantilla`.
- Mantener `camposPlantilla` y `plantilla` derivados de la misma consulta.

## TD-FE-02 - Contexto Documental Unico

### Alcance Implementado

- `RadicacionDocumentalProvider` administra el estado documental del modulo.
- `useRadicacionDocumentalContext` es el unico hook de acceso al contexto.
- `RadicacionDocumentosGuard` centraliza la regla de acceso a Documentos.
- `setContextoDocumental()` y `clearContextoDocumental()` quedan como operaciones oficiales.

### Regla Vigente

`Documentos` solo puede renderizar cuando se cumple:

```text
estadoActual === 0
AND requiereGestionDocumental === true
AND tieneTramiteDocumentalActivoEstado0 === true
AND idEstadoRadicado > 0
```

### Restricciones Vigentes

- No crear stores paralelos para estado documental.
- No activar `CapDocument` por consecutivo, gabinete, workflow o seleccion de fila.
- No duplicar la regla documental dentro de `RadicacionTabs`.

## FE-06 - Startup Guard Y Restauracion De Estado

### Alcance Implementado

- `RadicacionStartupGuard` consulta `GET /api/radicacion/pendientes/estado-activo`.
- Si existe tramite activo, restaura el `RadicacionDocumentalContext`.
- Si no existe tramite activo, limpia el contexto.
- Mientras inicializa, bloquea el render funcional con loading.
- Ante error de bootstrap, muestra estado de error con reintento.

### Resultado Arquitectonico

```text
RadicacionRoutePage
  -> RadicacionDocumentalProvider
  -> RadicacionStartupGuard
  -> RadicacionPage
```

### Restricciones Vigentes

- Las paginas no deben consultar `estado-activo`.
- Las paginas no deben restaurar ni limpiar contexto.
- El bootstrap pertenece al `RadicacionStartupGuard`.

## TD-FE-04 - Rutas, Tabs Y Limpieza De Prototipo

### Alcance Implementado

- Las keys numericas de tabs fueron reemplazadas por keys de dominio:

```text
ia
radicacion
documentos
gestion-radicados
```

- Las rutas del modulo quedaron centralizadas en `radicacionRoutes`.
- `CapDocument` ya no inicializa el digitalizador con contexto mock.
- `CapDocument` ya no muestra gabinete ni documentos ficticios.
- `Modalpendiente` ya no muestra tabla ni datos mock en runtime.

### Pull Request

```text
PR #318 - SCRUMCORE-293 consolidar navegacion de radicacion
Merge commit: 3ce62785c4fab16f2efd966aaec0dfb2a05eeb69
```

### Restricciones Vigentes

- No usar keys numericas para tabs.
- No hardcodear rutas en componentes.
- No mostrar datos mock en runtime productivo.
- No implementar pendientes/AppTable dentro de TD-FE-04.

## Validaciones Ejecutadas

### Suite Focalizada De TD-FE-04

```bash
npm test -- --run src/modules/radicacion/hooks/RadicacionTabs.spec.test.tsx src/modules/radicacion/components/CapDocument.spec.test.tsx src/modules/radicacion/components/Modalpendiente.spec.test.tsx src/modules/radicacion/routes/radicacionRoutes.test.ts
```

Resultado documentado:

```text
4 test files passed
10 tests passed
```

### Suite De Infraestructura De Radicacion

```bash
npm test -- --run src/modules/radicacion/pages/RadicacionRoutePage.spec.test.tsx src/modules/radicacion/components/RadicacionStartupGuard.spec.test.tsx src/modules/radicacion/hooks/RadicacionTabs.spec.test.tsx src/modules/radicacion/components/RadicacionDocumentosGuard.spec.test.tsx src/modules/radicacion/context/RadicacionDocumentalContext.spec.test.tsx src/modules/radicacion/services/radicacionPendientes.service.test.ts src/modules/radicacion/routes/radicacionRoutes.test.ts
```

Resultado documentado:

```text
7 test files passed
22 tests passed
```

## Estado De Dependencias Para Siguientes Fases

### Puede Ejecutarse Sin Backend Nuevo De Pendientes

- `PROMPT-TD-FE-05-Limpiar-Formulario-Radicacion-Entrante.md`
- `PROMPT-TD-FE-03-Refactor-RadicacionForm-Secciones-Hooks.md`, despues de TD-FE-05

### Puede Ejecutarse Si Existe Endpoint De Registro

- `PROMPT-FE-01-Conectar-Registro-Radicacion-Entrante.md`

FE-01 no debe implementar FE-02, FE-03, FE-04, FE-05, FE-06 ni FE-07.

### Ya Ejecutado Contra APIs Modernas De Pendientes

- `PROMPT-FE-05-Modal-Pendientes-AppTable-Asignacion-Radicado.md`

### No Cerrar Contra Datos Reales Sin Backend De Pendientes

- `PROMPT-FE-04-Pendientes-Radicacion-Gestion-Documental.md`
- `PROMPT-FE-07-Enviar-Tramite-Activo-A-Pendiente.md`

## Notas De Continuidad

- FE-05 debe reutilizar el `RadicacionDocumentalContext` existente.
- FE-05 no debe crear un contexto paralelo.
- FE-05 debe extender el modelo documental solo si el contrato de `tomar` lo requiere.
- Los endpoints de listado, contador y tomar pendiente pertenecen al alcance de FE-05 y sus prompts BE asociados, no a TD-FE-04.
