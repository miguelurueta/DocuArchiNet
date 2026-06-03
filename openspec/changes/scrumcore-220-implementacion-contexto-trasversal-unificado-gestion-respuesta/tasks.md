## 1. Refinamiento previo a publish

- [x] 1.1 Validar consumidores actuales de `GestionRespuestaDocumentosContext` y `useGestionRespuestaDocumentos`.
- [x] 1.2 Confirmar punto de obtencion de `radicado` e `idRespuestaRadicado` desde el flujo de estructura por tarea.
- [x] 1.3 Alinear `design.md`, delta `spec.md` y `tasks.md` con el prompt arquitectonico completo.
- [x] 1.4 Ejecutar `openspec validate` del change y corregir inconsistencias antes de publish.

## 2. Contexto documental transversal

- [x] 2.1 Extender el contrato tipado del contexto con `idTareaWf`, `radicado`, `idRespuestaRadicado`, `nombreGabinete`, `gabineteLoading`, `gabineteError` y `reloadGabinete`.
- [x] 2.2 Mantener `files` y `setFiles` sin cambiar su semantica actual.
- [x] 2.3 Extender props de `GestionRespuestaDocumentosProvider` para recibir `idTareaWf`, `radicado` e `idRespuestaRadicado`.
- [x] 2.4 Mantener `useGestionRespuestaDocumentos` backward-compatible con `available`, fallback fuera del provider y no-op seguro.
- [x] 2.5 Memoizar el value del context y `reloadGabinete` para evitar referencias nuevas innecesarias.

## 3. Resolucion segura de gabinete

- [x] 3.1 Actualizar `solicitaGabineteRadicadoWorkflow.service.ts` para soportar cancelacion tipada con `AbortSignal` sin cambiar endpoint ni contrato backend.
- [x] 3.2 Implementar carga automatica de gabinete por `idTareaWf` valido.
- [x] 3.3 Garantizar idempotencia: no re-fetch automatico si `idTareaWf` no cambia.
- [x] 3.4 Implementar `reloadGabinete` como recarga explicita para el id actual.
- [x] 3.5 Cancelar o invalidar requests anteriores cuando cambie `idTareaWf` o se desmonte el provider.
- [x] 3.6 Evitar race conditions con guard de request activa/secuencia para impedir stale updates.
- [x] 3.7 Normalizar `NombreGabinete` hacia `nombreGabinete?: string`.
- [x] 3.8 Aplicar fallback `undefined` cuando backend no retorne gabinete valido.
- [x] 3.9 Exponer `gabineteError?: string` sin romper render ni lanzar errores durante render.

## 4. Wiring de GestionRespuesta y consumidores

- [x] 4.1 Cablear `GestionRespuesta` para entregar `idTareaWf`, `radicado` e `idRespuestaRadicado` al provider.
- [x] 4.2 Verificar que visor, documentos y adjuntos lean estado compartido desde hook/context cuando aplique.
- [x] 4.3 Confirmar que ningun componente UI llama directamente al service ni a axios para gabinete.
- [x] 4.4 Preservar comportamiento actual de `AppVisorEmbedPdf`.
- [x] 4.5 Preservar comportamiento actual de `DocumentosWorkbench`.
- [x] 4.6 Preservar flujo actual de adjuntos y estado `files/setFiles`.
- [x] 4.7 No introducir cambios visuales ni estados UI locales dentro del contexto.

## 5. Pruebas obligatorias

- [x] 5.1 Agregar/ajustar pruebas unitarias del provider: estado inicial, props expuestas, loading, ready, error y fallback sin id valido.
- [x] 5.2 Agregar/ajustar pruebas del hook: fallback fuera del provider, `available`, `files/setFiles`, datos documentales y `reloadGabinete`.
- [x] 5.3 Probar idempotencia: una sola carga automatica por `idTareaWf`.
- [x] 5.4 Probar `reloadGabinete`: fuerza nueva request y actualiza `nombreGabinete`.
- [x] 5.5 Probar cancelacion/race conditions con cambio rapido de `idTareaWf`.
- [x] 5.6 Probar error backend: `gabineteError` visible, `gabineteLoading: false` y render estable.
- [x] 5.7 Probar integracion de consumers representativos: visor/documentos/adjuntos sin regresion.
- [x] 5.8 Ejecutar validaciones afectadas de TypeScript, tests y OpenSpec; registrar evidencia.

## 6. Documentacion SCRUMCORE-220

- [x] 6.1 Crear `docs/modulos/gestioncorrespondencia/normalizainiciorespuesta/contextounificadovariables/SCRUMCORE-220-Arquitectura.md`.
- [x] 6.2 Documentar en arquitectura: objetivo, decisiones, restricciones, vista estatica por capas, diagramas Mermaid de clases, secuencia y estados, ADRs, riesgos y trazabilidad.
- [x] 6.3 Crear `SCRUMCORE-220-Implementacion-Detallada.md` con archivos modificados, context/hook/service/page wiring, idempotencia, cancelacion y fallback.
- [x] 6.4 Crear `SCRUMCORE-220-Integracion-BackEnd.md` con endpoint gabinete, request, response, errores, retry/reload, fallback y compatibilidad FE-BE.
- [x] 6.5 Crear `SCRUMCORE-220-Pruebas.md` con unitarias, integracion, browser interaction, E2E/regresion y matriz de cobertura.
- [x] 6.6 Crear `SCRUMCORE-220-Metadata.md` con ticket, autor, fecha, version, control de cambios, referencias cruzadas y resultado de pruebas.

## 7. Cierre y publish

- [x] 7.1 Ejecutar `openspec validate scrumcore-220-implementacion-contexto-trasversal-unificado-gestion-respuesta --strict`.
- [x] 7.2 Revisar diff final para confirmar que no hay cambios UI ni logica de negocio fuera de alcance.
- [x] 7.3 Commit y push de refinamiento/implementacion/documentacion segun fase.
- [x] 7.4 Crear PR con resumen tecnico, pruebas ejecutadas, riesgos y documentacion generada.
- [ ] 7.5 Tras merge, archivar OpenSpec y actualizar estado Jira segun flujo SCRUM.
