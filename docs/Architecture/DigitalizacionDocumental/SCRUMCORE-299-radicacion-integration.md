# SCRUMCORE-299 - Integración de AppDigitalizador en Captura de Documentos/Radicación

## Objetivo

Eliminar la presentación de laboratorio en el flujo productivo de Radicación y mantener únicamente la experiencia de digitalización dentro de la pantalla de captura existente.

## Estado previo detectado

- `AppDigitalizador` mostraba un bloque de cabecera de sandbox:
  - `Digitalizador documental`
  - `Entrada corporativa de digitalizacion`
  - badge del módulo
- `DigitalizacionDocumentalWorkspace` mostraba tarjetas contextuales fijas:
  - `Gabinete`
  - `Radicado`
  - `Documento destino`
- El workspace mostraba estado de sandbox (`readyEmpty`, etc.) en el header.
- También mostraba una barra de acciones inferior heredada:
  - `Pendiente captura PDF`
  - `Listo para operar`
  - botón `Cancelar`
  - botón `Guardar documento`

## Cambios implementados

### Control de visibilidad desde AppDigitalizador

- Se añadieron props de configuración de presentación:
  - `showHeader?: boolean`
  - `showWorkspaceSummary?: boolean`
  - `showWorkspaceState?: boolean`
  - `showLegacyFooter?: boolean`
- `showHeader`, `showWorkspaceSummary` y `showWorkspaceState` ya estaban en uso para ocultar elementos de contexto/sandbox en el flujo productivo.
- `showLegacyFooter` permite controlar la renderización del bloque operativo inferior heredado.

### Control de visibilidad en el workspace

- `DigitalizacionDocumentalWorkspaceProps` quedó con:
  - `showSummary?: boolean`
  - `showStateBadge?: boolean`
  - `showLegacyFooter?: boolean`
- Estado por defecto:
  - `showSummary: true`
  - `showStateBadge: true`
  - `showLegacyFooter: true`
- El footer legado se renderiza solo cuando `showLegacyFooter` es `true`.
- En ese footer siguen presentes:
  - `Cancelar`
  - `Guardar documento`/`Adjuntar digitalizacion`
  - estado de operación heredado (`Pendiente captura PDF`, `Listo para operar`, etc. según estado previo)

### Integración con Radicación

- En `src/modules/radicacion/components/CapDocument.tsx` se configura `AppDigitalizador` con:
  - `showHeader={false}`
  - `showWorkspaceSummary={false}`
  - `showWorkspaceState={false}`
  - `showLegacyFooter={false}`
- De esta forma el workspace productivo inicia sin elementos de prueba/sandbox y conserva la ruta y flujo actual.

### Restauración del toolbar superior de Radicación

- `CapDocument` vuelve a orquestar un `toolbar` propio de Radicación por encima de `AppDigitalizador` con:
  - `Imprimir Rótulo`
  - `Guardar Rótulo`
  - `Detalle Radicado`
  - `Scanner`
- Este toolbar conserva `z-index` y distribución propios del módulo de Radicación.
- La jerarquía visual quedó:
  1. Header/Tabs de Radicación (flujo global, sin cambios)
  2. Toolbar de Radicación (nuevo en `CapDocument`)
  3. Toolbar interna de `AppDigitalizador`
  4. Workspace de captura digital

### Redistribución de altura útil del workspace

- Al ocultar el footer en Radicación, se elimina la franja inferior vacía heredada del sandbox.
- El área útil del contenedor pasa a estar disponible para:
  - vista central de preview PDF
  - miniaturas
  - `AppTreeTable`
  - organizador de páginas
- No se agregan contenedores ni placeholders para reemplazar el footer.
- No se introdujeron alturas fijas para simular la separación anterior.

### Recuperación de panel lateral con `AppTreeTable`

- `CapDocument` vuelve a montar un panel lateral con `AppTreeTable`.
- El panel quedó configurado con:
  - estado persistente (`showTreeCollapsed` en estado local de `CapDocument`)
  - remount único del panel (`AppTreeTable` permanece montado siempre)
  - visibilidad toggle sin reconstrucción del árbol (solo alterna `collapsed`).
- Se implementó con `AppCollapseRail` en `variant="overlay"` para que el panel se superponga al visor al abrirse y no fuerce recálculo de grilla.
- Al colapsar el panel:
  - desaparece visualmente
  - el workspace de digitalización ocupa inmediatamente el espacio
- Al expandir:
  - reaparece sin perder estado interno del panel
  - no se pierde estado del digitalizador (`AppDigitalizador` y su workspace no se desmontan)

### Ajuste de layout de incrustación

- `AppDigitalizador` se ajustó para adaptarse al contenedor:
  - `root` con `height:100%`, `flex:1`, `min-height:0`, `overflow:hidden`
  - `workspaceFrame` con `flex:1` y `min-*` para heredar espacio disponible
- `DigitalizacionDocumentalWorkspace` se alineó al contenedor:
  - `shell` sin tope de `100vh`
  - `block-size:100%` y límites inferiores en `0` para no forzar altura artificial
- Se conserva:
  - ruta de navegación
  - layout global de Radicación
  - sidebar, header y tabs de Radicación
- Sin redirecciones o layouts nuevos.

## Resultado esperado

- Pantalla productiva de Captura de Documentos/Radicación entra directo al `AppDigitalizador` integrado.
- No existe una pantalla intermedia de laboratorio.
- No se renderiza barra inferior heredada en el camino productivo.
- La altura útil se aprovecha íntegramente por los componentes de digitalización.

## Validaciones realizadas

- No cambia la ruta de navegación.
- No se crean rutas, módulos ni redirecciones nuevas.
- Se conserva la separación de entorno de Sandbox para pruebas técnicas.
- `AppVisorEmbedPdf` y otros módulos de digitalización se mantienen en su dominio actual.
### Correcciones de comportamiento (continuación SCRUMCORE-299)

- **AppTreeTable como panel Gmail overlay**
  - `AppTreeTable` en `CapDocument` permanece siempre montado.
  - La visibilidad se controla por `collapsed` y propiedades de presentación (`transform`, `visibility`, `opacity` y `pointer-events`) sin desmontaje.
  - El panel usa `AppCollapseRail` en `variant="overlay"` y `placement="left"`.
  - Al cerrar, el workspace reutiliza inmediatamente su espacio; al abrir, se superpone sin reconstruir digitalizador ni árbol.

- **Sin remount de `AppDigitalizador`**
  - La alternancia de `AppTreeTable` no altera el estado del visor ni su ciclo de vida.
  - Conserva documento cargado, página actual, zoom y selección cuando el panel se abre o cierra.

- **Unificación visual del toolbar de Radicación**
  - El toolbar superior de Radicación se construye con `AppButton` para los botones:
    - Imprimir Rótulo
    - Guardar Rótulo
    - Detalle Radicado
    - Scanner
  - No se introducen variantes visuales fuera de `AppButton` ni estilos de control de botones personalizados.
