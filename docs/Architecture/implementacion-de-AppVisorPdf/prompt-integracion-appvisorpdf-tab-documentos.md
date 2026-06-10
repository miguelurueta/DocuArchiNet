# Integración funcional `AppVisorPdf` en Tab Documentos

Integrar de forma **funcional** el componente `AppVisorPdf` en el Tab **Documentos** del proyecto `DocuArchiCore.react`, reemplazando el estado actual “sin visor ni acciones, solo layout base”, conectándolo a la lógica de negocio (listado/selección/carga de documentos) sin romper el responsive.

**Archivo principal a modificar**
- `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx`

**Estilos**
- `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.module.css`

## Contexto de UI
- El tab Documentos usa `DocumentosToolbar` (basado en `AppToolbar`) y un sidebar plegable con `AppCollapseRail`.
- En mobile/tablet el sidebar debe funcionar como **overlay**; en desktop puede ser inline según esté implementado.
- Existe un mecanismo para auto-colapsar el overlay al cambiar de tab; no lo elimines.
- Evitar regresiones de accesibilidad: no usar `aria-hidden` sobre un ancestro que contenga el foco. Si se requiere bloquear interacción, usar el patrón actual con `inert`/blur (como ya se usa en `AppCollapseRail`).

## Objetivo

Implementar `AppVisorPdf` en el panel principal del Tab Documentos, con integración real a datos y acciones del módulo, manteniendo:
- Layout estable (sin pantallas en blanco al abrir/cerrar sidebar).
- Toolbar responsive (desktop/tablet/mobile).
- Sidebar (`AppCollapseRail`) overlay estable en mobile/tablet, incluyendo iPad Pro.

## Requisitos (funcional / negocio)

1) **Render del visor**
- Renderiza `AppVisorPdf` en el área principal del workbench (donde hoy no hay visor).
- Debe ocupar el espacio disponible sin overflow horizontal.
- El scroll debe ser interno donde aplique (no “romper” el scroll del layout).

2) **Fuente del PDF (real)**
- La carga del PDF debe provenir del flujo real del módulo:
  - listado de documentos/anexos disponibles
  - selección de documento
  - obtención del contenido (URL segura/stream/base64/bytes) según la implementación existente del backend
- Manejar autenticación/autorización si aplica (headers/tokens) usando las utilidades existentes del proyecto.
- Evitar exponer URLs sensibles; respetar el modelo de seguridad actual.

3) **Estados**
- Manejar estados de forma completa:
  - `loading` (listado y visor)
  - `empty` (no hay documentos disponibles)
  - `error` (fallo de listado o descarga)
  - `unauthorized/forbidden` (si aplica)
  - “documento no soportado” (si el archivo no es PDF o no puede renderizarse)

4) **Interacciones**
- Sidebar (`AppCollapseRail`) debe mostrar el **listado real** de documentos (plegable/agrupable si aplica).
- Al seleccionar un documento:
  - el visor debe cargarlo y reflejar el título/estado
  - en mobile/tablet, el overlay puede auto-cerrarse tras seleccionar (si es mejor UX) sin romper el flujo
- Toolbar:
  - Mantener los botones/acciones existentes (labels/posiciones) y conectarlos a acciones reales si existen (buscar, vincular, abrir, etc.)
  - Si alguna acción no tiene backend listo, dejarla deshabilitada con tooltip/estado (sin romper UI)

5) **Performance**
- Evitar re-render excesivo del visor al abrir/cerrar sidebar.
- No bloquear el hilo principal: usar lazy-load/suspense si el visor es pesado (si ya hay patrón en el repo).
- Evitar descargas repetidas: cachear por documento seleccionado (según patrones existentes).

6) **Responsive / breakpoints a validar**
- Mobile ~`360×800`
- Tablet ~`768×1024`
- iPad Pro `1024×1366` y rango tablet `901–1122` (caso crítico)
- Desktop `>= 1280`

**Criterio clave:** abrir/cerrar el sidebar overlay NO puede dejar la pantalla en blanco ni colapsar la UI.

## Requisitos de estilo/layout

- Ajustar `DocumentosWorkbench.module.css` para:
  - asegurar `min-height: 0` en wrappers con `display: grid/flex`
  - evitar overflow horizontal del visor
  - asegurar que el panel principal pueda crecer/encogerse sin reflow agresivo
  - compatibilidad con overlay (z-index/backdrop manejado por `AppCollapseRail`)

## Accesibilidad / focus

- No reintroducir `aria-hidden` para ocultar overlays cuando haya elementos focusables dentro.
- Mantener el patrón de overlay con backdrop y bloqueo de interacción usando `inert`/blur.

## Tests / validación

- Si hay tests existentes del workbench, actualízalos (puedes mockear `AppVisorPdf`).
- Documentar en el PR la validación manual por breakpoints (ideal con screenshots).

## Entregables

- Cambios en:
  - `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx`
  - `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.module.css`
  - servicios/hooks/selectores necesarios para obtener listado y contenido de documentos (donde corresponda en el módulo)
  - tests relevantes (si aplica)

## Definition of Done

- El Tab Documentos lista documentos reales y permite seleccionar uno.
- `AppVisorPdf` renderiza el PDF seleccionado desde la fuente real (sin mocks).
- Responsive estable en mobile/tablet/iPad Pro/desktop.
- Sidebar overlay funciona y se auto-oculta al cambiar de tab.
- Acciones principales del toolbar conectadas (o deshabilitadas de forma explícita sin romper UI).
- Manejo de estados `loading/empty/error` completo.
