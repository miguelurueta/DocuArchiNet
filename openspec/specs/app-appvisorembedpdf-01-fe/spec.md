# app-appvisorembedpdf-01-fe Specification

## Purpose
TBD - created by archiving change scrumcore-201-crea-componente-appvisorembedpdf-01-fe. Update Purpose after archive.
## Requirements
### Requirement: Estructura obligatoria del módulo
El sistema SHALL implementar el componente bajo la estructura modular obligatoria:

- `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.tsx`
- `src/app/Components/UI/AppVisorEmbedPdf/index.ts`
- `src/app/Components/UI/AppVisorEmbedPdf/hooks/`
- `src/app/Components/UI/AppVisorEmbedPdf/engine/`
- `src/app/Components/UI/AppVisorEmbedPdf/plugins/`
- `src/app/Components/UI/AppVisorEmbedPdf/presentation/`
- `src/app/Components/UI/AppVisorEmbedPdf/styles/`
- `src/app/Components/UI/AppVisorEmbedPdf/types/`
- `src/app/Components/UI/AppVisorEmbedPdf/README.md`

#### Scenario: Estructura lista para plugins futuros
- **WHEN** se revisa el árbol de `src/app/Components/UI/AppVisorEmbedPdf/`
- **THEN** existen los folders requeridos y el export centralizado está en `index.ts`

### Requirement: API obligatoria (props)
El componente SHALL exponer exactamente esta interfaz de props como contrato público mínimo:

`AppVisorEmbedPdfProps { fileUrl?: string; className?: string; style?: React.CSSProperties; }`

#### Scenario: Uso mínimo esperado
- **WHEN** un consumidor renderiza `<AppVisorEmbedPdf fileUrl="/demo/demo.pdf" />`
- **THEN** el componente renderiza el visor sin requerir props adicionales

### Requirement: Engine Pdfium y plugins permitidos (01-FE)
El componente SHALL inicializar Pdfium Engine usando `usePdfiumEngine()` y SHALL registrar plugins usando `createPluginRegistration`.

El componente SHALL basarse únicamente en:
- `@embedpdf/core`
- `@embedpdf/engines`
- `@embedpdf/plugin-document-manager`
- `@embedpdf/plugin-viewport`
- `@embedpdf/plugin-scroll`
- `@embedpdf/plugin-render`

El componente MUST NOT implementar todavía (ni incluir wiring funcional) para:
`zoom`, `rotate`, `toolbar`, `search`, `thumbnails`, `annotations`, `signatures`, `password`, `print/download`.

#### Scenario: Plugins limitados a los permitidos
- **WHEN** se revisan imports del módulo `AppVisorEmbedPdf`
- **THEN** no hay imports/uso de plugins o features fuera del set permitido para 01-FE

### Requirement: Render y composición obligatoria
El componente SHALL renderizar el visor usando la composición nativa de EmbedPDF con:
`EmbedPDF`, `DocumentContent`, `Viewport`, `Scroller`, `RenderLayer`.

#### Scenario: Scroll vertical funcional
- **WHEN** el usuario hace scroll vertical sobre el visor
- **THEN** el documento navega verticalmente sin errores React

### Requirement: Virtualización y lazy rendering nativos
El componente SHALL habilitar virtualización nativa usando `Scroller` y SHALL usar lazy rendering nativo de EmbedPDF (render incremental por viewport/páginas visibles).

#### Scenario: No renderiza todo el documento de una vez
- **WHEN** se carga un PDF multipágina
- **THEN** el render se realiza de forma incremental (viewport/lazy) y el scroll se mantiene fluido

### Requirement: Estados de UI (engine/document) y UX base
El componente SHALL proveer:
- Loader mientras carga el engine
- Loader mientras carga el documento
- Empty state básico
- Error state básico
- Fondo tipo visor profesional y responsive

#### Scenario: Loader de engine
- **WHEN** el engine Pdfium aún no está listo
- **THEN** se muestra un loader visible de engine

#### Scenario: Empty state sin fileUrl
- **WHEN** `fileUrl` no existe
- **THEN** se carga un PDF demo configurable/local (sin hardcodear URLs externas dentro de la lógica reusable)

### Requirement: Encapsulación y regla de arquitectura (DocumentosWorkbench)
La lógica de EmbedPDF/Pdfium MUST permanecer encapsulada dentro de `AppVisorEmbedPdf`. `DocumentosWorkbench` MUST NOT conocer lógica EmbedPDF.

#### Scenario: Consumo desacoplado
- **WHEN** un módulo consumidor renderiza el componente
- **THEN** no requiere importar `@embedpdf/*` ni conocer detalles del engine/plugins

### Requirement: Estrategia de estilos permitida
El componente MUST usar únicamente CSS Modules o la estrategia visual ya utilizada por el proyecto. El componente MUST NOT usar `styled-components`, tailwind o mezcla de estrategias.

#### Scenario: Estilos consistentes con el proyecto
- **WHEN** se revisa el módulo `styles/` y su uso
- **THEN** se usa una sola estrategia (CSS Modules o la del proyecto) sin mezcla

