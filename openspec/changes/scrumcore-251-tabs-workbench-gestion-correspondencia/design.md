# Design - SCRUMCORE-251 - Tabs paralelos Workbench Gestion Correspondencia

## Context

`SCRUMCORE-251` solicita una mejora opt-in para que el Workbench de Gestion Correspondencia permita trabajar en paralelo los tabs normales `Gestion` y `Documentos` usando `react-resizable-panels`.

El punto real de composicion actual esta en `src/modules/gestionCorrespondencia/pages/GestionRespuesta.tsx`:

- Construye dos `AppTabItem`: `gestion` y `documentos`.
- Renderiza ambos contenidos mediante `AppTabs`.
- Envuelve los dos tabs en un unico `GestionRespuestaDocumentosProvider`.
- `GestionRespuestaMainTabContent` concentra editor, adjuntos y acciones de gestion.
- `DocumentosWorkbench` concentra listado, visor PDF, firma, carga y reemplazo de paginas anotadas.

La mejora debe ser una capa de composicion visual. No debe cambiar endpoints, hooks de negocio, permisos, flujo de firma ni logica del visor.

## Goals

- Mantener el modo actual de tabs como comportamiento por defecto.
- Agregar un control visible para alternar entre `Vista paralela` y `Vista normal`.
- Mostrar `Gestion` y `Documentos` simultaneamente en desktop/tablet ancho.
- Permitir redimensionar ambos paneles con `react-resizable-panels`.
- Preservar el contexto compartido existente bajo `GestionRespuestaDocumentosProvider`.
- Evitar duplicar providers, requests o estados criticos por montar el layout paralelo.
- Documentar la decision responsive y los riesgos residuales.

## Non-Goals

- No modificar contratos backend.
- No modificar endpoints ni servicios de documentos.
- No reescribir `DocumentosWorkbench`.
- No alterar firma, visor PDF, reemplazo de paginas anotadas ni AppEditor.
- No persistir preferencias en storage.
- No introducir rutas nuevas.
- No implementar resizing manual.
- No implementar IA real, streaming, persistencia conversacional ni auditoria del chat dentro de este cambio.

## Architecture

### Integration Point

La integracion debe vivir en `GestionRespuesta.tsx`, porque ahi se conoce la composicion de ambos tabs y ya existe el provider compartido.

Estructura esperada:

```tsx
<GestionRespuestaDocumentosProvider ...>
  <div className={styles.layoutToolbar}>
    <AppButton aria-pressed={isParallel}>...</AppButton>
  </div>

  {isParallel ? (
    <GestionWorkbenchParallelTabs
      gestion={<GestionRespuestaMainTabContent ... />}
      documentos={<DocumentosWorkbench ... />}
    />
  ) : (
    <AppTabs items={items} fullWidth className={styles.tabs} />
  )}
</GestionRespuestaDocumentosProvider>
```

La instancia de `GestionRespuestaDocumentosProvider` debe mantenerse fuera del switch de layout para conservar estado compartido de adjuntos y contexto de documentos.

### New Layout Component

Crear un componente presentacional:

```txt
src/modules/gestionCorrespondencia/components/workbenchParallelTabs/
  GestionWorkbenchParallelTabs.tsx
  GestionWorkbenchParallelTabs.module.css
  index.ts
```

Responsabilidades:

- Renderizar `PanelGroup`, `Panel` y `PanelResizeHandle`.
- Exponer paneles accesibles con `aria-label="Gestion"` y `aria-label="Documentos"`.
- Definir tamanos iniciales y minimos.
- Resolver layout, overflow y divisor visual.
- No importar services, hooks de negocio ni contexto de Gestion Correspondencia.

Contrato propuesto:

```ts
type GestionWorkbenchParallelTabsProps = {
  gestion: React.ReactNode;
  documentos: React.ReactNode;
  className?: string;
};
```

### State

Usar estado controlado local en `GestionRespuesta.tsx`, salvo que durante implementacion aparezca necesidad real de compartirlo:

```ts
type GestionWorkbenchLayoutMode = "tabs" | "parallel";
```

El estado no se persiste porque Jira exige un modo opt-in sin requerimiento de preferencia permanente. Esto reduce efectos colaterales entre tramites.

### Dependency

Agregar `react-resizable-panels` si no existe en `package.json`.

Justificacion:

- Provee resize accesible y probado.
- Evita implementar pointer events y constraints manuales.
- Encapsula `PanelGroup`, `Panel` y `PanelResizeHandle`.
- Encaja con una UX enterprise de paneles productivos.

## UX And Responsive Decisions

### Desktop / Tablet ancho

- Mostrar dos paneles horizontales.
- Proporcion inicial: `Gestion` 50%, `Documentos` 50%.
- Minimo recomendado: 35% por panel.
- El handle debe ser visible, sobrio y usable con hover/focus.

### Mobile / ancho reducido

- No forzar dos columnas horizontales.
- Mantener modo tabs o bloquear visualmente el modo paralelo si el ancho no permite productividad real.
- Si se decide mantener el boton visible en mobile, debe degradar a modo tabs o mostrar una explicacion corta sin montar dos columnas inutiles.

Decision inicial: fallback a tabs normales en ancho reducido, para evitar degradar `DocumentosWorkbench`, visor PDF y AppEditor.

## Accessibility

- El control debe ser un `button` o `AppButton` accesible.
- Debe exponer `aria-pressed`.
- Debe tener texto visible: `Vista paralela` cuando esta inactivo y `Vista normal` cuando esta activo.
- Los paneles deben tener labels accesibles.
- El resize handle debe tener foco visible y no bloquear scroll interno.

## Data And State Safety

- No crear nuevos providers alrededor de cada panel.
- No duplicar `GestionRespuestaDocumentosProvider`.
- No duplicar llamadas de `DocumentosWorkbench` por renderizar dos instancias.
- Al alternar, debe conservarse el contexto de gestion y documentos siempre que los componentes permanecen bajo el mismo provider.
- Si `AppTabs` desmonta el tab inactivo, el modo paralelo puede montar ambos contenidos una sola vez. Esto es aceptable mientras no haya dos instancias de `DocumentosWorkbench`.

## Risks / Trade-offs

- `DocumentosWorkbench` contiene visor y layouts con alturas complejas; el contenedor paralelo debe usar `min-height: 0`, `height: 100%` y overflow controlado.
- Montar `Gestion` y `Documentos` simultaneamente puede iniciar carga de documentos aunque el usuario venga del tab Gestion. Se debe observar que no haya requests duplicados, solo la carga normal de una unica instancia.
- El visor PDF puede necesitar recalculo de dimensiones al pasar de tabs a panel. El cambio no debe introducir timers; debe resolverse con layout estable y constraints.
- En mobile, dos columnas reducen usabilidad; por eso el fallback debe ser conservador.

## Addendum 2026-06-20 - Asistente IA y overlays

### Assistant Shell

El asistente IA se mantiene como shell local de UI dentro de `GestionRespuesta.tsx`.

Decisiones:

- Boton flotante persistente con label `IA` e icono `RobotOutlined`.
- Callout superior con texto `¿Te ayudo con la respuesta?` cuando el chat esta cerrado.
- Panel tipo chat compacto con log local, sugerencias demo, input normal, limpiar y enviar.
- Sin backend ni servicio IA real.
- Input no controlado mediante `useRef` para evitar perdida de foco y re-render por caracter.
- Envio por Enter desde captura de teclado del panel.
- Cierre con animacion antes de desmontar.
- Cierre mobile con `blur()` del input, timeout controlado y safe-area inferior.

### Assistant Attention Pattern

El patron de atencion visual usa animacion periodica sobria:

- elevacion leve.
- halo azul controlado.
- pausa en hover/focus.
- hover/focus sincronizado entre FAB y callout mediante `:has()`.

Se evita:

- vibracion.
- rebote fuerte.
- decoracion excesiva.
- textos largos persistentes.

### Suggestions

Las sugerencias demo son botones de texto debajo del historial de mensajes.

Regla de comportamiento:

- Seleccionar sugerencia carga texto en el input.
- No envia automaticamente.
- El usuario conserva control editorial.

### AppEditor overlays

El indicador de palabras/caracteres se apila encima del indicador de pagina.

Justificacion:

- Evitar solapamiento con el FAB de IA.
- Mantener juntos los indicadores relacionados del editor.
- No modificar logica de conteo ni paginacion.

El media query mobile existente del AppEditor se conserva.

## Migration Plan

1. Confirmar si `react-resizable-panels` existe en dependencias.
2. Crear `GestionWorkbenchParallelTabs` como layout presentacional.
3. Agregar estado `layoutMode` en `GestionRespuesta.tsx`.
4. Agregar boton de toggle en la banda superior del Workbench.
5. Mantener `GestionRespuestaDocumentosProvider` como wrapper comun.
6. Renderizar `AppTabs` en modo normal y `GestionWorkbenchParallelTabs` en modo paralelo.
7. Ajustar CSS de `GestionRespuesta.module.css` y del nuevo componente.
8. Agregar tests de toggle, accesibilidad y doble render controlado.
9. Crear documentacion enterprise en `docs/Architecture/GestionCorrrespondecia/Tabs-React-Resizable-Panels/`.
10. Ejecutar validacion OpenSpec, tests afectados y build.

## Open Questions

- Confirmar durante implementacion si `AppTabs` mantiene montados ambos panes o desmonta el inactivo.
- Confirmar breakpoint final para desactivar/degradar vista paralela.
- Confirmar si el boton debe vivir antes de los tabs o alineado a la derecha de la barra de tabs segun CSS final.
