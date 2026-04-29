# app-appvisorpdf-02-fe (Delta Spec)

## ADDED Requirements

### Requirement: AppVisorPdf PDF engine SHALL be implemented with pdf.js behind a stable interface
El sistema SHALL implementar un motor PDF real basado en `pdfjs-dist` encapsulado tras un
contrato estable `PdfEngine` para mantener la UI desacoplada del engine.

#### Scenario: Engine is defined behind PdfEngine interface
- **WHEN** `AppVisorPdf` needs to load or render a PDF page
- **THEN** it SHALL do so through `PdfEngine.load()` and `PdfEngine.renderPage()` and not by calling pdf.js directly from UI components

#### Scenario: Engine supports cancellation
- **WHEN** a render is in progress and the consumer changes `input` or `zoom`
- **THEN** the engine SHALL support cancellation via `AbortSignal` to avoid wasted work and race conditions

### Requirement: AppVisorPdf SHALL render incrementally for large PDFs
El sistema SHALL renderizar de forma incremental, evitando renderizar todas las p\u00e1ginas upfront.

#### Scenario: Only visible pages are rendered
- **WHEN** a PDF with many pages is loaded
- **THEN** the viewport SHALL render only the active page plus a small buffer of nearby pages

#### Scenario: UI remains responsive on large PDFs
- **WHEN** the user opens a large PDF
- **THEN** the UI SHALL remain responsive and SHALL avoid long blocking tasks caused by full-document rendering

### Requirement: AppVisorPdf SHALL cache rendered pages by page and zoom with bounded limits
El sistema SHALL cachear renderizados por llave `pageNumber|zoom` con una pol\u00edtica LRU y
l\u00edmites acotados.

#### Scenario: Cache uses LRU eviction
- **WHEN** cache reaches its configured limit
- **THEN** the least recently used entries SHALL be evicted first

#### Scenario: Cache resets on input change
- **WHEN** `input` changes to a different PDF
- **THEN** the engine/viewport SHALL clear the previous cache to avoid memory leaks and stale renders

### Requirement: AppVisorPdf SHALL provide real loading and error states
El sistema SHALL exponer estados reales de `loading` y `error` durante `load()` y `renderPage()`,
con mensajes amigables para UI.

#### Scenario: Loading state is visible while loading
- **WHEN** the engine starts loading a PDF
- **THEN** the component SHALL surface `loading` state (or equivalent UI status) until load completes

#### Scenario: Errors are surfaced without stack traces
- **WHEN** pdf loading or rendering fails
- **THEN** the UI SHALL surface an error message without exposing internal stack traces

