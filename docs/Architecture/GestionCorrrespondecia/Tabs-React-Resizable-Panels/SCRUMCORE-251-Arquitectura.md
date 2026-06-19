# SCRUMCORE-251 - Arquitectura

## Objetivo

SCRUMCORE-251 mejora el workbench de Gestion Correspondencia para permitir trabajo operativo mas fluido entre `Gestion` y `Documentos`, y endurece la respuesta visual de esa superficie en desktop pequeno, tablets y mobile.

El alcance inicial del ticket fue la vista paralela con `react-resizable-panels`. El bloque actual documentado aqui corresponde al hardening UI posterior: compactacion del tab Gestion, adjuntos compactos, metadatos legibles, altura responsive del tab Documentos y ajuste coordinado del visor PDF.

## Capas involucradas

```mermaid
flowchart TD
  A[GestionCorrespondenciaRoute] --> B[Header detalle y metadata]
  A --> C[GestionRespuesta]
  C --> D[AppTabs / panelContent]
  D --> E[GestionRespuestaMainTabContent]
  D --> F[DocumentosWorkbench]
  E --> G[AppToolbar]
  E --> H[AppEditor]
  E --> I[AppUpload]
  F --> J[AppVisorEmbedPdf]
  F --> K[AppCollapseRail / lista documentos]
```

## Principios de diseno preservados

- No se modifican endpoints, DTOs, services ni contratos backend.
- No se cambia la logica de negocio de Gestion Correspondencia.
- Los componentes shared (`AppToolbar`, `AppUpload`, `AppTabs`, `AppVisorEmbedPdf`) reciben ajustes compatibles hacia atras.
- Los ajustes visuales especificos de Gestion se encapsulan con clases CSS module del modulo.
- La UI se mantiene enterprise: baja decoracion, alta densidad, controles visibles, foco y labels preservados.

## Decisiones arquitectonicas

### ADR-251-01: Densidad controlada en AppToolbar

Se agrego `density?: "default" | "compact"` en `AppToolbar`.

Justificacion:

- El toolbar de Gestion necesitaba reducir altura sin afectar toolbars del resto del producto.
- Un prop explicito evita acoplar el comportamiento a un ancho global.
- El componente mantiene su media query interna para `compact`, pero `compactDensity` permite compactacion visual controlada por consumidor.

Impacto:

- `GestionRespuestaMainTabContent` usa `density="compact"`.
- El test de AppToolbar valida que desktop no aplique clase `compact` por accidente.

### ADR-251-02: AppUpload extensible por className y estado estable por ref

Se agrego `className` a `AppUpload` y se introdujo `filesRef`.

Justificacion:

- Gestion necesita compactar cards de adjuntos sin cambiar la apariencia global de upload.
- `filesRef` evita que actualizaciones async de estrategia `auto` lean arrays obsoletos y desaparezcan archivos cargados.
- `role="listitem"` facilita estilos scoped y mejora semantica de listas.

Impacto:

- Upload mantiene API existente.
- Se agrego cobertura para archivo visible en estrategia `auto`.
- En Gestion se reemplaza la accion visual por un unico boton de eliminar.

### ADR-251-03: Alturas responsive coordinadas por contenedor y visor

Documentos tiene tres capas que pueden imponer altura:

- `AppTabs.module.css` via `.panelContent`.
- `DocumentosWorkbench.module.css` via `.workbenchBody` y `.viewer`.
- `AppVisorEmbedPdf.module.css` via `.root`.

Decision:

- Mantener sincronizadas las alturas del workbench y el root del visor en los breakpoints mobile.
- Usar `:has([data-testid="documentos-workbench"])` en `AppTabs` para que los ajustes de panel solo afecten Documentos.
- Colocar el override de iPad Mini al final para evitar que el bloque mobile general lo sobrescriba.

Justificacion:

- El usuario reporto diferencias visuales por dispositivo. El problema real era que capas distintas imponian altos distintos.
- La solucion sincroniza el alto visible del visor con el alto del contenedor que lo hospeda.

### ADR-251-04: Metadata compacta con informacion completa

La metadata del detalle se reubica y compacta en mobile, manteniendo `title` completo.

Justificacion:

- El usuario necesita ver `Radicado`, `Remitente` y `Tramite` en mobile sin perder contexto.
- En pantallas estrechas se prioriza alineacion a la derecha y wrapping antes que truncamiento irreversible.

## Breakpoints documentados

### Mobile general

- `max-width: 768px`
- Documentos:
  - `workbenchBody`: `clamp(540px, 72dvh, 680px)`
  - `AppVisorEmbedPdf.root`: `min-height: 540px`
  - `panelContent`: `min-height: clamp(560px, 76dvh, 680px)`

### iPhone SE y pantallas bajas

- `max-width: 430px` y `max-height: 740px`
- Documentos y visor:
  - `clamp(410px, 65dvh, 490px)`

### Samsung Galaxy S8+

- `min-width: 350px`, `max-width: 380px`, `min-height: 720px`, `max-height: 760px`
- Documentos y visor:
  - `clamp(425px, 68dvh, 515px)`

### iPhone 12 Pro / familia 390x844 aproximada

- `max-width: 430px` y `min-height: 741px`
- Documentos y visor:
  - `clamp(575px, 73dvh, 650px)`

### iPhone XR / 14 Pro Max y pantallas altas

- Base alta:
  - `max-width: 430px` y `min-height: 880px`
  - `clamp(660px, 76dvh, 720px)`
- XR override:
  - `min-width: 400px`, `max-width: 430px`, `min-height: 840px`, `max-height: 920px`
  - `clamp(645px, 74dvh, 695px)`
  - `panelContent`: `clamp(660px, 76dvh, 710px)`

### iPad Mini

- `min-width: 744px`, `max-width: 834px`, `min-height: 1000px`, `max-height: 1150px`
- `panelContent`: `calc(100vh - 190px)`
- `DocumentosWorkbench.workbenchBody`: `calc(100vh - 215px)`
- `DocumentosWorkbench.viewer`: `calc(100vh - 215px)`
- `AppVisorEmbedPdf.root`: `calc(100vh - 215px)`

## Flujo visual resultante

1. El usuario entra al detalle de Gestion Correspondencia.
2. En el tab Gestion, la toolbar superior se muestra compacta y sin sticky.
3. El AppEditor ocupa una altura adaptada al viewport.
4. Adjuntos se mantienen debajo del editor con upload a lo ancho y cards pequenas agrupadas.
5. En el tab Documentos, el contenedor principal y el visor PDF comparten una altura coherente por dispositivo.
6. En mobile, el rail de documentos permanece disponible como overlay/rail lateral.
7. La metadata del detalle permanece accesible con tooltip nativo.

## Restricciones respetadas

- No se introducen nuevos hooks de negocio.
- No se persiste estado responsive.
- No se usa JavaScript para medir viewport.
- No se agregan dependencias nuevas en este bloque.
- No se toca la seleccion, firma, exportacion, permisos ni reemplazo de paginas del visor PDF.
- No se cambia la estructura de servicios de documentos.

## Riesgos y mitigaciones

- Riesgo: reglas por dispositivo aumentan costo de mantenimiento.
  - Mitigacion: los rangos quedan documentados y scoped al workbench de documentos.
- Riesgo: `:has()` no soportado en navegadores antiguos.
  - Mitigacion: producto apunta a navegadores modernos; selector queda aislado y no rompe funcionalidad core.
- Riesgo: cambios en viewport real de navegador pueden variar respecto a DevTools.
  - Mitigacion: QA debe validar matriz final en dispositivos/emuladores.
- Riesgo: cards de upload demasiado compactas en nombres largos.
  - Mitigacion: `title` en nombre, truncamiento controlado y accion de eliminar visible.
