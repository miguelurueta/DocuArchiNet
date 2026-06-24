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
- [x] 5.8 Validar que no desaparece el documento seleccionado al alternar modo.
- [x] 5.9 Validar que no hay doble carga evidente ni errores de consola al alternar modo.
- [x] 5.10 Ejecutar tests afectados de Gestion Correspondencia.
- [x] 5.11 Ejecutar `npm.cmd run build`.
  - Resultado: ejecutado y bloqueado por error TypeScript externo a SCRUMCORE-251 en `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts:908` (`DynamsoftWebTwainFactory` no definido). No se modifica Digitalizacion para no afectar componentes fuera del alcance.
- [x] 5.12 Ejecutar `npx.cmd openspec validate scrumcore-251-tabs-workbench-gestion-correspondencia --strict`.

## 6. QA manual obligatorio

- [x] 6.1 Abrir Gestion Correspondencia.
- [x] 6.2 Entrar a un tramite con tabs `Gestion` y `Documentos`.
- [x] 6.3 Verificar modo normal actual.
- [x] 6.4 Activar `Vista paralela`.
- [x] 6.5 Confirmar que `Gestion` y `Documentos` se ven simultaneamente.
- [x] 6.6 Redimensionar los paneles.
- [x] 6.7 Seleccionar un documento en `Documentos`.
- [x] 6.8 Volver a `Vista normal`.
- [x] 6.9 Confirmar que no se perdio seleccion/contexto.
- [x] 6.10 Reabrir `Vista paralela`.
- [x] 6.11 Confirmar que no hay doble carga evidente ni errores de consola.
- [x] 6.12 Probar en ancho reducido.

## 7. Documentacion enterprise

- [x] 7.1 Crear `docs/Architecture/GestionCorrrespondecia/Tabs-React-Resizable-Panels/SCRUMCORE-251-Arquitectura.md`.
- [x] 7.2 Crear `SCRUMCORE-251-Implementacion-Detallada.md`.
- [x] 7.3 Crear `SCRUMCORE-251-Pruebas.md`.
- [x] 7.4 Crear `SCRUMCORE-251-Metadata.md`.
- [x] 7.5 Documentar decision responsive, riesgos y pruebas ejecutadas.
- [x] 7.6 Documentar archivos modificados, decisiones tomadas, riesgos residuales e instrucciones de uso para usuario final.
- [x] 7.7 Actualizar documentacion enterprise con compactacion de Gestion, AppToolbar density, AppUpload compacto, metadata mobile, DocumentosWorkbench y AppVisorEmbedPdf responsive por dispositivo.
- [x] 7.8 Actualizar metadata del ticket con evidencia TypeScript, archivos tocados, riesgos residuales y estado de commit.
- [x] 7.9 Documentar asistente IA flotante, decisiones de arquitectura, implementacion, eventos, foco, animaciones, limitaciones y QA manual.
- [x] 7.10 Actualizar spec OpenSpec con requisitos del asistente IA y no solapamiento con indicadores del AppEditor.
- [x] 7.11 Actualizar metadata con archivos tocados por IA, evidencia TypeScript, alcance sin backend y riesgos residuales.

## 8. Asistente IA flotante del workbench

- [x] 8.1 Agregar boton flotante azul persistente en `GestionRespuesta`.
- [x] 8.2 Garantizar que el boton sea visible en tabs `Gestion` y `Documentos`.
- [x] 8.3 Cambiar label del boton a `IA`.
- [x] 8.4 Usar `RobotOutlined` para estado cerrado.
- [x] 8.5 Usar `CloseOutlined` para estado abierto.
- [x] 8.6 Corregir ocultamiento del label para no ocultar el span interno del icono Ant Design.
- [x] 8.7 Implementar panel tipo chat local sin backend ni extension.
- [x] 8.8 Agregar header, log de mensajes, composer, input, limpiar y enviar.
- [x] 8.9 Agregar respuesta placeholder local para preparar integracion futura.
- [x] 8.10 Animar apertura del chat desde el boton flotante.
- [x] 8.11 Animar cierre del chat hacia el boton flotante antes de desmontar.
- [x] 8.12 Ajustar animacion a estilo enterprise sobrio.
- [x] 8.13 Reducir ancho del panel de chat en desktop y mobile.
- [x] 8.14 Cambiar textarea inicial por input normal de una linea.
- [x] 8.15 Cambiar input controlado por input no controlado via `useRef`.
- [x] 8.16 Corregir perdida de foco despues del primer caracter escrito.
- [x] 8.17 Enviar mensaje con Enter cuando el foco esta en el input.
- [x] 8.18 Mantener foco en el input despues de enviar.
- [x] 8.19 Agregar X interna para limpiar texto del input.
- [x] 8.20 Mantener foco despues de limpiar texto.
- [x] 8.21 Aislar eventos de teclado/pointer del panel para evitar captura por AppEditor.
- [x] 8.22 Mover `pageStatsIndicator` del AppEditor hacia la izquierda para no chocar con el FAB IA.
- [x] 8.23 Validar TypeScript despues de los cambios del asistente IA.
- [x] 8.24 Agregar callout superior del asistente con texto corto orientado a cliente.
- [x] 8.25 Quitar badge interno `IA` del callout para evitar duplicidad visual.
- [x] 8.26 Sincronizar hover/focus/active entre FAB IA y callout.
- [x] 8.27 Agregar animacion periodica enterprise de atencion para FAB y callout.
- [x] 8.28 Agregar sugerencias demo de respuesta debajo del historial del chat.
- [x] 8.29 Permitir que las sugerencias carguen texto en el input sin enviar automaticamente.
- [x] 8.30 Ajustar panel de chat para desktop/mobile con ancho mas compacto.
- [x] 8.31 Corregir cierre mobile con blur del input, limpieza de timeout y safe-area.
- [x] 8.32 Mantener visible el FAB despues de cerrar el chat en mobile.

## 8.1 AppEditor overlays asociados al asistente

- [x] 8.1.1 Apilar `pageStatsIndicator` encima de `pageIndicator`.
- [x] 8.1.2 Centrar `pageStatsIndicator` con `left: 50%` y `transform: translateX(-50%)`.
- [x] 8.1.3 Bajar levemente ambos flotantes del AppEditor en el estilo base.
- [x] 8.1.4 Hacer menos alto el flotante de palabras/caracteres.
- [x] 8.1.5 No modificar el media query mobile existente del AppEditor.
- [x] 8.1.6 Validar TypeScript despues de los ajustes de overlays.

## 9. Grip de redimensionamiento enterprise

- [x] 9.1 Hacer transparente el contenedor base de `PanelResizeHandle`.
- [x] 9.2 Mantener area interactiva de 10px para resize.
- [x] 9.3 Mantener `aria-label="Redimensionar paneles"`.
- [x] 9.4 Agregar tooltip nativo `Arrastra para redimensionar`.
- [x] 9.5 Conservar visible el `span` interno del grip.
- [x] 9.6 Reemplazar barra solida por patron de puntos verticales via CSS.
- [x] 9.7 Agregar hover/focus/active azul sutil.
- [x] 9.8 Mantener cursor `col-resize`.
- [x] 9.9 Validar TypeScript.
- [x] 9.10 Ejecutar test de `GestionWorkbenchParallelTabs`.
- [x] 9.11 Documentar arquitectura, implementacion, pruebas, metadata y spec OpenSpec del grip.
- [x] 9.12 Ajustar el grip interno para visualizarse como bolitas pequenas.

## 10. Publish / cierre tecnico

- [x] 10.1 Revisar diff final.
- [x] 10.2 Commit de implementacion.
- [x] 10.3 Push de `feature/SCRUMCORE-251`.
- [x] 10.4 Crear PR.
- [x] 10.5 Actualizar metadata con PR/commits/evidencias.
- [x] 10.6 Documentar actualizacion 2026-06-20 de asistente IA, sugerencias, mobile y AppEditor overlays.
