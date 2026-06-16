# Tasks - SCRUMCORE-251

## 1. Refinement

- [x] 1.1 Consultar Jira mediante `opsxj:new SCRUMCORE-251`.
- [x] 1.2 Crear artefactos OpenSpec iniciales desde Jira.
- [x] 1.3 Normalizar placeholders del ticket a `SCRUMCORE-251`.
- [x] 1.4 Revisar punto real de composicion de tabs en `GestionRespuesta.tsx`.
- [x] 1.5 Refinar `design.md`, `spec.md` y `tasks.md` antes de publish.

## 2. Dependencia y arquitectura

- [ ] 2.1 Verificar si `react-resizable-panels` existe en `package.json`.
- [ ] 2.2 Instalar `react-resizable-panels` solo si no existe.
- [ ] 2.3 Crear `src/modules/gestionCorrespondencia/components/workbenchParallelTabs/GestionWorkbenchParallelTabs.tsx`.
- [ ] 2.4 Crear `GestionWorkbenchParallelTabs.module.css`.
- [ ] 2.5 Crear `index.ts` para exportar el nuevo componente.
- [ ] 2.6 Mantener el componente como layout presentacional sin services ni hooks de negocio.

## 3. Integracion funcional

- [ ] 3.1 Agregar estado local `GestionWorkbenchLayoutMode = "tabs" | "parallel"` en `GestionRespuesta.tsx`.
- [ ] 3.2 Agregar boton opt-in `Vista paralela` / `Vista normal`.
- [ ] 3.3 Exponer estado del boton con `aria-pressed`.
- [ ] 3.4 Mantener `GestionRespuestaDocumentosProvider` como wrapper comun del modo normal y paralelo.
- [ ] 3.5 Renderizar `AppTabs` sin cambios en modo normal.
- [ ] 3.6 Renderizar `Gestion` y `Documentos` en `GestionWorkbenchParallelTabs` en modo paralelo.
- [ ] 3.7 Confirmar que no se crea una segunda instancia de `DocumentosWorkbench`.
- [ ] 3.8 Confirmar que el contexto de adjuntos/documentos no se pierde al alternar modo.

## 4. UX, CSS y responsive

- [ ] 4.1 Definir paneles horizontales en desktop/tablet ancho.
- [ ] 4.2 Definir tamanos iniciales 50/50 y minimos seguros.
- [ ] 4.3 Agregar divisor visual con hover/focus.
- [ ] 4.4 Aplicar `min-height: 0`, `height: 100%` y overflow controlado en contenedores necesarios.
- [ ] 4.5 Mantener fallback a tabs normales en ancho reducido si la vista paralela degrada la UX.
- [ ] 4.6 Evitar tarjetas decorativas, gradientes u ornamentos fuera del lenguaje enterprise.

## 5. Pruebas

- [ ] 5.1 Agregar test del nuevo `GestionWorkbenchParallelTabs`.
- [ ] 5.2 Agregar/ajustar test de `GestionRespuesta` para validar modo normal por defecto.
- [ ] 5.3 Validar que el boton activa modo paralelo.
- [ ] 5.4 Validar que el boton vuelve a modo normal.
- [ ] 5.5 Validar que `Gestion` y `Documentos` aparecen simultaneamente en modo paralelo.
- [ ] 5.6 Validar `aria-pressed`.
- [ ] 5.7 Validar que no se duplican callbacks criticos por el toggle.
- [ ] 5.8 Ejecutar tests afectados de Gestion Correspondencia.
- [ ] 5.9 Ejecutar `npm.cmd run build`.
- [ ] 5.10 Ejecutar `npx.cmd openspec validate scrumcore-251-tabs-workbench-gestion-correspondencia --strict`.

## 6. Documentacion enterprise

- [ ] 6.1 Crear `docs/Architecture/GestionCorrrespondecia/Tabs-React-Resizable-Panels/SCRUMCORE-251-Arquitectura.md`.
- [ ] 6.2 Crear `SCRUMCORE-251-Implementacion-Detallada.md`.
- [ ] 6.3 Crear `SCRUMCORE-251-Pruebas.md`.
- [ ] 6.4 Crear `SCRUMCORE-251-Metadata.md`.
- [ ] 6.5 Documentar decision responsive, riesgos y pruebas ejecutadas.

## 7. Publish / cierre tecnico

- [ ] 7.1 Revisar diff final.
- [ ] 7.2 Commit de implementacion.
- [ ] 7.3 Push de `feature/SCRUMCORE-251`.
- [ ] 7.4 Crear PR.
- [ ] 7.5 Actualizar metadata con PR/commits/evidencias.
