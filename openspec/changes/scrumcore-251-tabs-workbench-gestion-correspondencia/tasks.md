# Tasks - SCRUMCORE-251

## 1. Refinement

- [x] 1.1 Consultar Jira mediante `opsxj:new SCRUMCORE-251`.
- [x] 1.2 Crear artefactos OpenSpec iniciales desde Jira.
- [x] 1.3 Normalizar placeholders del ticket a `SCRUMCORE-251`.
- [x] 1.4 Revisar punto real de composicion de tabs en `GestionRespuesta.tsx`.
- [x] 1.5 Refinar `design.md`, `spec.md` y `tasks.md` antes de publish.
- [x] 1.6 Leer y confirmar impacto en `DocumentosWorkbench.tsx`, `GestionRespuestaMainTabContent.tsx`, `GestionRespuesta.module.css` y `DocumentosWorkbench.module.css` antes de implementar.
- [x] 1.7 Confirmar si `AppTabs` mantiene montados ambos tabs o solo el tab activo.
- [x] 1.8 Confirmar que no se requiere tocar `GestionRespuestaDocumentosContext`, `useGestionRespuestaDocumentos`, `useGestionRespuestaDocumentosTable` ni `AppTabs`; si se toca alguno, documentar la razon.

## 2. Dependencia y arquitectura

- [x] 2.1 Verificar si `react-resizable-panels` existe en `package.json`.
- [x] 2.2 Instalar `react-resizable-panels` solo si no existe.
- [x] 2.3 Crear `src/modules/gestionCorrespondencia/components/workbenchParallelTabs/GestionWorkbenchParallelTabs.tsx`.
- [x] 2.4 Crear `GestionWorkbenchParallelTabs.module.css`.
- [x] 2.5 Crear `index.ts` para exportar el nuevo componente.
- [x] 2.6 Mantener el componente como layout presentacional sin services ni hooks de negocio.
- [x] 2.7 Importar `PanelGroup`, `Panel` y `PanelResizeHandle` exclusivamente desde `react-resizable-panels`.
- [x] 2.8 No implementar drag/resize manual con pointer events, timers ni hacks de layout.
- [x] 2.9 No introducir `any` en tipos nuevos o modificados.

## 3. Integracion funcional

- [x] 3.1 Agregar estado local `GestionWorkbenchLayoutMode = "tabs" | "parallel"` en `GestionRespuesta.tsx`.
- [x] 3.2 Agregar boton opt-in `Vista paralela` / `Vista normal`.
- [x] 3.3 Exponer estado del boton con `aria-pressed`.
- [x] 3.4 Mantener `GestionRespuestaDocumentosProvider` como wrapper comun del modo normal y paralelo.
- [x] 3.5 Renderizar `AppTabs` sin cambios en modo normal.
- [x] 3.6 Renderizar `Gestion` y `Documentos` en `GestionWorkbenchParallelTabs` en modo paralelo.
- [x] 3.7 Confirmar que no se crea una segunda instancia de `DocumentosWorkbench`.
- [x] 3.8 Confirmar que el contexto de adjuntos/documentos no se pierde al alternar modo.
- [x] 3.9 Usar iconografia existente para columnas/layout; no dibujar SVG manual.
- [x] 3.10 No persistir `layoutMode` en `localStorage`, `sessionStorage` ni storage equivalente.
- [x] 3.11 No cambiar contratos backend, endpoints, services, permisos, requests ni validaciones de negocio.
- [x] 3.12 No afectar firma, visor PDF, AppEditor, reemplazo de paginas anotadas ni flujo de documentos.

## 4. UX, CSS y responsive

- [x] 4.1 Definir paneles horizontales en desktop/tablet ancho.
- [x] 4.2 Definir tamanos iniciales 50/50 y minimos seguros.
- [x] 4.3 Agregar divisor visual con hover/focus.
- [x] 4.4 Aplicar `min-height: 0`, `height: 100%` y overflow controlado en contenedores necesarios.
- [x] 4.5 Mantener fallback a tabs normales en ancho reducido si la vista paralela degrada la UX.
- [x] 4.6 Evitar tarjetas decorativas, gradientes u ornamentos fuera del lenguaje enterprise.
- [x] 4.7 Agregar labels accesibles `aria-label="Gestion"` y `aria-label="Documentos"` en los paneles.
- [x] 4.8 Garantizar que el resize handle no bloquee scroll interno ni overlays del visor/documentos.
- [x] 4.9 Garantizar foco visible en boton y divisor cuando aplique.
- [x] 4.10 Documentar explicitamente la decision responsive final.

## 5. Pruebas

- [x] 5.1 Agregar test del nuevo `GestionWorkbenchParallelTabs`.
- [x] 5.2 Agregar/ajustar test de `GestionRespuesta` para validar modo normal por defecto.
- [x] 5.3 Validar que el boton activa modo paralelo.
- [x] 5.4 Validar que el boton vuelve a modo normal.
- [x] 5.5 Validar que `Gestion` y `Documentos` aparecen simultaneamente en modo paralelo.
- [x] 5.6 Validar `aria-pressed`.
- [x] 5.7 Validar que no se duplican callbacks criticos por el toggle.
- [ ] 5.8 Validar que no desaparece el documento seleccionado al alternar modo.
- [ ] 5.9 Validar que no hay doble carga evidente ni errores de consola al alternar modo.
- [x] 5.10 Ejecutar tests afectados de Gestion Correspondencia.
- [ ] 5.11 Ejecutar `npm.cmd run build`.
- [x] 5.12 Ejecutar `npx.cmd openspec validate scrumcore-251-tabs-workbench-gestion-correspondencia --strict`.

## 6. QA manual obligatorio

- [ ] 6.1 Abrir Gestion Correspondencia.
- [ ] 6.2 Entrar a un tramite con tabs `Gestion` y `Documentos`.
- [ ] 6.3 Verificar modo normal actual.
- [ ] 6.4 Activar `Vista paralela`.
- [ ] 6.5 Confirmar que `Gestion` y `Documentos` se ven simultaneamente.
- [ ] 6.6 Redimensionar los paneles.
- [ ] 6.7 Seleccionar un documento en `Documentos`.
- [ ] 6.8 Volver a `Vista normal`.
- [ ] 6.9 Confirmar que no se perdio seleccion/contexto.
- [ ] 6.10 Reabrir `Vista paralela`.
- [ ] 6.11 Confirmar que no hay doble carga evidente ni errores de consola.
- [ ] 6.12 Probar en ancho reducido.

## 7. Documentacion enterprise

- [x] 7.1 Crear `docs/Architecture/GestionCorrrespondecia/Tabs-React-Resizable-Panels/SCRUMCORE-251-Arquitectura.md`.
- [x] 7.2 Crear `SCRUMCORE-251-Implementacion-Detallada.md`.
- [x] 7.3 Crear `SCRUMCORE-251-Pruebas.md`.
- [x] 7.4 Crear `SCRUMCORE-251-Metadata.md`.
- [x] 7.5 Documentar decision responsive, riesgos y pruebas ejecutadas.
- [x] 7.6 Documentar archivos modificados, decisiones tomadas, riesgos residuales e instrucciones de uso para usuario final.
- [x] 7.7 Actualizar documentacion enterprise con compactacion de Gestion, AppToolbar density, AppUpload compacto, metadata mobile, DocumentosWorkbench y AppVisorEmbedPdf responsive por dispositivo.
- [x] 7.8 Actualizar metadata del ticket con evidencia TypeScript, archivos tocados, riesgos residuales y estado de commit.

## 8. Publish / cierre tecnico

- [x] 8.1 Revisar diff final.
- [x] 8.2 Commit de implementacion.
- [ ] 8.3 Push de `feature/SCRUMCORE-251`.
- [ ] 8.4 Crear PR.
- [ ] 8.5 Actualizar metadata con PR/commits/evidencias.
