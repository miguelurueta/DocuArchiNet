# SCRUMCORE-251 - Metadata

## Identificacion

- Ticket: `SCRUMCORE-251`
- Nombre tecnico: `tabs-workbench-gestion-correspondencia`
- Rama: `feature/SCRUMCORE-251`
- Tipo: Mejora UX enterprise / responsive hardening / composicion de workbench
- Modulo funcional: Gestion Correspondencia
- Alcance principal: frontend
- Backend: no modificado
- Fecha base de ticket: 2026-06-16
- Actualizacion responsive documentada: 2026-06-19
- Actualizacion asistente IA documentada: 2026-06-19
- Actualizacion asistente IA, callout, sugerencias, mobile y overlays AppEditor: 2026-06-20
- Actualizacion de validacion de tareas automatizables: 2026-06-22
- Autor operativo: Codex, guiado por usuario

## Estado actual

- Implementacion de vista paralela del workbench realizada previamente en la rama.
- Ajustes responsive actuales aplicados sobre tabs, toolbar, editor, upload, visor PDF, DocumentosWorkbench y header de detalle.
- Asistente IA flotante local implementado sobre el workbench en tabs `Gestion` y `Documentos`.
- Asistente IA actualizado con callout superior, hover sincronizado, animacion periodica enterprise, sugerencias demo, cierre mobile robusto y safe-area.
- Indicador de palabras/caracteres del AppEditor apilado sobre el indicador de pagina y compactado en altura.
- Grip de redimensionamiento de vista paralela ajustado a patron enterprise de puntos verticales con tooltip y feedback sutil.
- TypeScript verificado con `npx.cmd tsc --noEmit --pretty false`.
- Tests focalizados de `GestionWorkbenchParallelTabs` y `GestionRespuesta` verificados con 6 tests OK.
- `npm.cmd run build` ejecutado; bloqueado por error TypeScript externo al Scrum en Digitalizacion/Dynamsoft.
- QA manual obligatorio confirmado por usuario y marcado como completado en `tasks.md`.
- Commit responsive previo: `0cc874d fix(SCRUMCORE-251): polish responsive workbench UI`.
- Commit asistente IA: commit actual con mensaje `feat(SCRUMCORE-251): add workbench AI assistant`.
- Push: ejecutado sobre `feature/SCRUMCORE-251`.
- PR: `#294` draft, `https://github.com/miguelurueta/DocuArchiCore.react/pull/294`.

## Commits previos relevantes

- `2e6c00f feat(SCRUMCORE-251): proposal inicial OpenSpec`
- `33d1273 docs(SCRUMCORE-251): normalize Jira OpenSpec context`
- `11c4624 docs(SCRUMCORE-251): refine OpenSpec artifacts`
- `c2d4527 docs(SCRUMCORE-251): align tasks with architectural prompt`
- `6c19128 feat(SCRUMCORE-251): add parallel workbench tabs`
- `7bae54a fix(SCRUMCORE-251): refine mobile document workbench`
- `f77db8a fix(SCRUMCORE-251): polish mobile documents overlay`
- `d02fc0c fix(SCRUMCORE-251): refine tablet responsive workbench`
- `d77d3f5 fix(SCRUMCORE-251): tune mobile document panel height`
- `70ad45b fix(SCRUMCORE-251): refine document overlay breakpoints`
- `6f4111c fix(SCRUMCORE-251): support nest hub document overlay`
- `9d78901 fix(SCRUMCORE-251): compact mobile editor actions`
- `0cc874d fix(SCRUMCORE-251): polish responsive workbench UI`
- `feat(SCRUMCORE-251): add workbench AI assistant` (commit actual; hash verificable con `git log`)
- `12a751a docs(SCRUMCORE-251): close workbench validation`

## Archivos de codigo modificados en este bloque

- `src/app/Components/UI/AppTabs/AppTabs.module.css`
- `src/app/Components/UI/AppToolbar/AppToolbar.tsx`
- `src/app/Components/UI/AppToolbar/AppToolbar.module.css`
- `src/app/Components/UI/AppToolbar/AppToolbar.test.tsx`
- `src/app/Components/UI/AppUpload/AppUpload.tsx`
- `src/app/Components/UI/AppUpload/AppUpload.module.css`
- `src/app/Components/UI/AppUpload/AppUpload.test.tsx`
- `src/app/Components/UI/AppVisorEmbedPdf/styles/AppVisorEmbedPdf.module.css`
- `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.module.css`
- `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.tsx`
- `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.module.css`
- `src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.tsx`
- `src/modules/gestionCorrespondencia/style/GestionCorrespondenciaRoute.module.css`
- `src/modules/gestionCorrespondencia/pages/GestionRespuesta.tsx`
- `src/modules/gestionCorrespondencia/style/GestionRespuesta.module.css`
- `src/app/Components/UI/AppEditor/AppEditor.module.css`
- `src/modules/gestionCorrespondencia/components/workbenchParallelTabs/GestionWorkbenchParallelTabs.tsx`
- `src/modules/gestionCorrespondencia/components/workbenchParallelTabs/GestionWorkbenchParallelTabs.module.css`

## Documentacion enterprise actualizada

- `docs/Architecture/GestionCorrrespondecia/Tabs-React-Resizable-Panels/SCRUMCORE-251-Arquitectura.md`
- `docs/Architecture/GestionCorrrespondecia/Tabs-React-Resizable-Panels/SCRUMCORE-251-Implementacion-Detallada.md`
- `docs/Architecture/GestionCorrrespondecia/Tabs-React-Resizable-Panels/SCRUMCORE-251-Pruebas.md`
- `docs/Architecture/GestionCorrrespondecia/Tabs-React-Resizable-Panels/SCRUMCORE-251-Metadata.md`

## Resumen tecnico del bloque

1. AppToolbar
   - Se agrego la prop `density?: "default" | "compact"`.
   - Se agrego clase `compactDensity` para densidad visual compacta.
   - Se elimino el comportamiento CSS que forzaba columna global en `max-width: 1100px`.
   - El modo compacto queda controlado por media query interna del componente y por `density`.

2. Gestion tab / AppEditor shell
   - Se compacto la toolbar superior de acciones de Gestion.
   - Se redujeron gaps y padding verticales.
   - Se ajustaron alturas del area del editor por desktop, tablets y mobile.
   - Se preservo el flujo de `AppEditor` sin alterar su contrato funcional.

3. AppUpload / Adjuntos
   - Se agrego `className` a `AppUpload` para customizacion scoped.
   - Se agrego `role="listitem"` en cards para listas de archivos.
   - Se corrigio la persistencia visual de archivos con estrategia `auto` usando `filesRef`.
   - Se agrego test para asegurar que un archivo cargado queda visible con estado `done`.
   - En Gestion se configura `layout="list"`, `previewOnClick={false}` y acciones custom.
   - En cards de adjuntos se deja solo accion de eliminar.
   - Las cards se agrupan lado a lado, compactas, con wrap y menor altura en mobile.

4. Header de detalle
   - Los items de metadata (`Radicado`, `Remitente`, `Tramite`) reciben `title` completo.
   - En mobile la metadata se alinea a la derecha junto al boton de retorno con layout vertical.
   - Se protege legibilidad mediante wrapping, text-align right y tooltip nativo.

5. DocumentosWorkbench / AppVisorEmbedPdf
   - Se agregaron breakpoints especificos para mobile y tablet.
   - El alto de `workbenchBody`, `.viewer` y `.root` del visor PDF se sincroniza por rango.
   - Se ajustaron vistas objetivo: iPhone SE, Samsung Galaxy S8+, iPhone XR, iPhone 12 Pro, iPhone 14 Pro Max e iPad Mini.
   - Se mantuvo el overlay lateral de documentos y el rail sin tocar contratos ni services.

6. AppTabs
   - Se redujo padding general de `panelContent`.
   - En tablets se reducen gaps y padding.
   - El panel de Documentos tiene reglas responsive scoped mediante `:has([data-testid="documentos-workbench"])`.
   - El override de iPad Mini se ubico al final para evitar que el bloque mobile general lo pise.

7. Asistente IA flotante
   - Se agrego una capa flotante persistente dentro de `GestionRespuesta`.
   - El boton FAB se ubica abajo a la derecha y queda visible en ambos tabs.
   - El boton muestra `IA` con `RobotOutlined` cuando esta cerrado.
   - El boton muestra `CloseOutlined` cuando el chat esta abierto.
   - El label `IA` usa `assistantFabLabel` para poder ocultarlo sin ocultar el span interno del icono de Ant Design.
   - El panel se abre como chat compacto enterprise.
   - La animacion de apertura simula expansion desde el boton.
   - La animacion de cierre simula retorno hacia el boton antes de desmontar.
   - El callout superior muestra `¿Te ayudo con la respuesta?` solo cuando el chat esta cerrado.
   - El callout no contiene badge interno `IA`; la identidad IA queda en el FAB.
   - El hover/focus/active del callout y del FAB se sincronizan con `:has()`.
   - El FAB y el callout tienen animacion periodica de atencion con elevacion y halo sobrio.
   - La animacion se pausa durante interaccion del usuario.
   - El panel se hizo mas angosto: `clamp(258px, 24vw, 306px)`.
   - En mobile se ajusto a `min(300px, calc(100vw - 1.25rem))`.
   - En mobile muy pequeno se ajusto a `min(288px, calc(100vw - 0.75rem))`.
   - Se agrego safe-area inferior para evitar que el FAB quede oculto.
   - Se agrego `isAssistantClosing` para permitir animacion de salida.
   - Se agrego `assistantCloseTimeoutRef` para evitar timeouts obsoletos.
   - Se agrego `assistantMessages` como historial local en memoria.
   - Se agrego respuesta placeholder local para preparar futura integracion conversacional.
   - No se invoca backend ni servicio IA real.

8. Input del asistente
   - El input final es `input type="text"` normal, no `textarea`.
   - Se usa `assistantInputRef` como input no controlado para evitar re-render por caracter.
   - Se corrigio perdida de foco despues del primer caracter.
   - `Enter` envia el mensaje desde `handleAssistantKeyDownCapture`.
   - El panel detiene propagacion de teclado/pointer para que el AppEditor no intercepte foco.
   - Se agrego X interna para limpiar texto.
   - La X interna limpia el valor y conserva foco.
   - El boton enviar usa `SendOutlined`.
   - Al cerrar el chat se hace `blur()` del input para evitar desplazamientos por teclado virtual en mobile.
   - Se agregaron sugerencias demo que cargan texto en el input sin enviarlo automaticamente.

9. AppEditor overlay
   - `pageStatsIndicator` se reubico al centro con `left: 50%`, `right: auto` y `transform: translateX(-50%)`.
   - `pageStatsIndicator` queda encima de `pageIndicator`.
   - `pageIndicator` se bajo levemente en el estilo base.
   - `pageStatsIndicator` se compacto reduciendo `min-height` y `padding`.
   - Motivo: evitar solapamiento con el FAB de IA y mantener juntos los indicadores del editor.
   - No cambia conteo de palabras/caracteres ni paginacion visual.
   - No se altero el media query mobile existente del AppEditor.

10. Resize handle de vista paralela
   - `PanelResizeHandle` conserva `aria-label="Redimensionar paneles"`.
   - Se agrega `title="Arrastra para redimensionar"`.
   - El contenedor del handle queda transparente.
   - El `span` interno queda visible como grip de puntos verticales.
   - Hover/focus/active aplican feedback azul sutil.
   - Se conserva cursor `col-resize` y area interactiva de 10px.
   - No se cambia la libreria ni el comportamiento de resize.

## Evidencia tecnica

- TypeScript:
  - Comando: `npx.cmd tsc --noEmit --pretty false`
  - Resultado: OK
- Tests enfocados:
  - `npx.cmd vitest run src/app/Components/UI/AppUpload/AppUpload.test.tsx`: 11 tests OK.
  - `npx.cmd vitest run src/app/Components/UI/AppToolbar/AppToolbar.test.tsx`: 4 tests OK.
- Diff check:
  - Comando: `git diff --check`
  - Resultado: sin errores de whitespace; Git aviso conversion LF/CRLF por configuracion local.
- Validacion asistente IA:
  - Comando: `npx.cmd tsc --noEmit --pretty false`
  - Resultado: OK despues de agregar `RobotOutlined`, chat local, input no controlado, X interna, sugerencias demo, hover sincronizado, animaciones, cierre mobile robusto y reubicacion/apilado de `pageStatsIndicator`.
- OpenSpec:
  - Comando: `npx.cmd openspec validate scrumcore-251-tabs-workbench-gestion-correspondencia --strict`
  - Resultado: OK, cambio valido.
  - Nota: la CLI reporto `PostHogFetchNetworkError` por red restringida al enviar telemetria, sin afectar la validacion.
- Whitespace:
  - Comando: `git diff --check`
  - Resultado: OK, sin errores de whitespace; avisos LF/CRLF por configuracion local.
- Resize handle:
  - Comando: `npx.cmd vitest run src/modules/gestionCorrespondencia/components/workbenchParallelTabs/GestionWorkbenchParallelTabs.test.tsx`
  - Resultado: OK, 1 test OK.
  - Comando: `npx.cmd tsc --noEmit --pretty false`
  - Resultado: OK.
  - Comando: `npx.cmd openspec validate scrumcore-251-tabs-workbench-gestion-correspondencia --strict`
  - Resultado: OK, cambio valido; se repite aviso de telemetria PostHog por red restringida sin afectar validacion.
- Actualizacion 2026-06-20:
  - Comando repetido durante ajustes UI: `npx.cmd tsc --noEmit --pretty false`
  - Resultado: OK.
  - Alcance validado: callout del asistente, sugerencias, animaciones de atencion, cierre mobile, safe-area y flotantes del AppEditor.
- Actualizacion 2026-06-22:
  - Comando: `npx.cmd vitest run src/modules/gestionCorrespondencia/components/workbenchParallelTabs/GestionWorkbenchParallelTabs.test.tsx src/modules/gestionCorrespondencia/pages/GestionRespuesta.test.tsx`
  - Resultado: OK, 2 archivos, 6 tests.
  - Ajuste de test: `GestionRespuesta.test.tsx` incluye mocks para iconos del asistente IA (`RobotOutlined`, `CloseOutlined`, `SendOutlined`) para alinear la suite con el componente actual.
  - Cobertura agregada: documento seleccionado simulado se conserva al alternar modo y no se duplica la instancia visible de `DocumentosWorkbench`.
  - Comando: `npx.cmd tsc --noEmit --pretty false`
  - Resultado: OK.
  - Comando: `git diff --check`
  - Resultado: OK; solo avisos LF/CRLF por configuracion local.
  - Comando: `npm.cmd run build`
  - Resultado: fallido por `TS2552` en `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts:908`, simbolo `DynamsoftWebTwainFactory` no definido.
  - Decision: no se corrige `DynamsoftTwainClient.ts` dentro de SCRUMCORE-251 para no afectar Digitalizacion ni componentes fuera del alcance solicitado.
  - QA manual: usuario confirma ejecucion completa de los pasos 6.1 a 6.12; checklist actualizado como completado.
  - Commit de cierre de validacion: `12a751a docs(SCRUMCORE-251): close workbench validation`.
  - Push: `feature/SCRUMCORE-251` actualizado en `origin`.
  - Pull request: `#294`, draft, base `main`, head `feature/SCRUMCORE-251`.
  - URL: `https://github.com/miguelurueta/DocuArchiCore.react/pull/294`.
- Tests agregados/modificados:
  - `AppUpload.test.tsx`: caso de archivo visible con estrategia `auto`.
  - `AppToolbar.test.tsx`: assertion para que desktop no se marque como compacto.

## Riesgos residuales

- La matriz responsive depende de alto/ancho CSS reportado por DevTools; QA debe validar en dispositivos o emuladores equivalentes.
- Las reglas `:has()` requieren soporte moderno del navegador; el proyecto ya usa navegadores modernos y el selector queda scoped a UI interna.
- El uso de breakpoints por dispositivo es intencional por requerimiento visual, pero debe revisarse si se incorpora una estrategia fluida posterior.
- El build completo fue ejecutado y queda bloqueado por deuda/error externo en Digitalizacion/Dynamsoft, no por archivos de SCRUMCORE-251.
- No se agregaron pruebas automatizadas especificas del asistente IA en este bloque; queda cubierto por TypeScript y requiere QA visual/manual.
- El asistente IA es shell visual local; requiere ticket posterior para integracion real con API conversacional, seguridad, auditoria, persistencia y contexto de tramite.
