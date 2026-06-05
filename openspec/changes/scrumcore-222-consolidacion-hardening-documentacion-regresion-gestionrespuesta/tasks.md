# SCRUMCORE-222 — Consolidacion, hardening, regresion y documentacion enterprise de GestionRespuesta

## Objetivo tecnico
- Consolidar y validar estabilidad del refactor transversal de GestionRespuesta sin introducir nuevas features ni cambios de endpoints/contratos.
- Verificar la integracion documento + visor + adjuntos y cerrar trazabilidad de los scrums 219, 220 y 221.

## Estado actual de avance
- [x] Contexto transversal y hooks ajustados para consumo documental compartido (nombreGabinete/flags de estado).
- [x] Eliminacion de fetch local de gabinete en `useListaDocumentosRadicadosTreeTable` e integracion con `GestionRespuestaDocumentosContext`.
- [x] `files/setFiles` preservado, sin cambios de UI funcionales.
- [x] Guardias de accion documental con estado de gabinete (`loading/error`) sin bloquear render general del arbol.

## Fase A — Auditoria / Baseline
- [x] Definir alcance y restricciones de consolidacion.
- [x] Levantar baseline de regresion de modulos impactados.
- [x] Revisar estado previo y confirmar migraciones base sin regresiones iniciales.
- [x] Definir matriz de control tecnica y manual.

## Fase B — Hardening (cambios minimos)
- [x] `GestionRespuestaDocumentosContext`:
  - [x] Estado transversal expandido: `idTareaWf`, `radicado`, `idRespuestaRadicado`, `files`, `setFiles`.
  - [x] Carga idempotente por `idTareaWf`.
  - [x] Cancelacion de requests obsoletos.
  - [x] `gabineteLoading`, `gabineteError`, `reloadGabinete`.
  - [x] Fallback seguro cuando gabinete no disponible.
- [x] `useGestionRespuestaDocumentos`:
  - [x] Consume y expone estado normalizado sin acoplar UI ni negocio.
- [x] `useListaDocumentosRadicadosTreeTable`:
  - [x] Remueve resolucion local de gabinete.
  - [x] Consume `nombreGabinete`, `gabineteLoading`, `gabineteError`, `reloadGabinete`.
  - [x] Mantiene contrato de retorno (`load`, `loadChildren`, `loading`, `error`, `rows`, `actions`).
  - [x] `ver_documento` valida contexto de gabinete de forma no bloqueante para el arbol.
- [x] Estabilidad de interaccion:
  - [x] Evita mutaciones de estado no relacionadas.
  - [x] Mantiene estabilidad del render del arbol y evita flicker de visor/listado.
- [x] Calidad y robustez:
  - [x] Control de race conditions y deduplicacion de fetch en cambios rapidos de tarea.

## Fase C — Cierre y documentacion enterprise
- [x] Documentacion tecnica consolidada completa:
  - [x] `SCRUMCORE-222-Arquitectura.md`
  - [x] `SCRUMCORE-222-Implementacion-Detallada.md`
  - [x] `SCRUMCORE-222-Integracion-BackEnd.md`
  - [x] `SCRUMCORE-222-Pruebas.md`
  - [x] `SCRUMCORE-222-Metadata.md`
- [x] Incluir trazabilidad con 219, 220 y 221.
- [x] Incluir riesgos tecnicos y mitigaciones.
- [x] Incluir evidencia de pruebas y estado ejecutado/pendiente.

## Criterios de aceptacion (checklist tecnico)
- [x] Sin cambios de endpoint.
- [x] Sin cambios funcionales no solicitados.
- [x] Sin regresiones visibles en flujo principal:
  - [x] GestionRespuesta tabs
  - [x] DocumentosWorkbench + arbol documental
  - [x] AppVisorEmbedPdf
  - [x] AppTreeTable / AppTable / SCRUM-205
  - [x] Adjuntos estables
- [x] Contexto transversal estable y no "god-context".
- [x] Pruebas y baseline registrados.

## Pruebas requeridas y estado
- [x] Unitarias:
  - [x] Context/provider.
  - [x] Hook documentos (actions + carga + fallback).
- [x] Integracion:
  - [x] Documento tree table + contexto transversal.
  - [x] Visor PDF con flujo de seleccion.
  - [x] Carga/recarga de gabinete sin re-fetch duplicado.
- [ ] E2E:
  - [ ] Flujo completo end-to-end gestion respuesta + arbol + visor + adjuntos.
  - [ ] Caso de error de gabinete con retry.
  - [ ] Escenario responsive de interaccion intensiva.
  - [ ] Resultado: BLOQUEADO por entorno (`PLAYWRIGHT_LOGIN_EMPRESA_ID`, `PLAYWRIGHT_LOGIN_MODULO_ID`, `PLAYWRIGHT_LOGIN_USER`, `PLAYWRIGHT_LOGIN_PASSWORD`).
- [x] Calidad:
  - [x] Sin cambios de contrato publicos.
- [ ] Pendientes de ejecucion local:
  - [ ] Ejecutar lote completo final de CI/tests y registrar salida.
  - [ ] Ejecutar E2E navegables y registrar evidencia (videos/logs/screens) cuando existan credenciales Playwright.

## Riesgos residuales
- [x] Regresion silenciosa baja bajo guardas y checks actuales.
- [x] Riesgo de stale-state mitigado con cancelacion y secuencia de request.
- [ ] Riesgo residual de ambiente: no ejecucion E2E por falta de credenciales/vars Playwright.
- [ ] Riesgo residual de regresion UX en breakpoints sin validacion completa responsive.

## Notas de no regresion
- [x] No se modifico endpoint.
- [x] No se toco la capa visual primaria de toolbar/navbar ni layout.
- [x] No se cambiaron contratos de accion/documentos ni de `load/loadChildren`.
