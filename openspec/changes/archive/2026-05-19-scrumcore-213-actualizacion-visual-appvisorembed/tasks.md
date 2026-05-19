## 1. Setup

- [x] 1.1 Revisar UI actual del tab “Firma personal”
- [x] 1.2 Confirmar que no se muestra `blobUrl`/`UrlTemporal` en UI final

## 2. UI Implementation

- [x] 2.1 Renderizar preview de imagen con `<img alt="Firma personal">` usando `personal.blobUrl`
- [x] 2.2 Ajustar estilos enterprise del preview en `AppPdfSignatureModal.module.css` (contain + size)
- [x] 2.3 Eliminar botón “Usar firma personal” y dejar un único CTA “Usar firma”
- [x] 2.4 “Usar firma” en tab personal llama `onStartPlacement(stamp)` y resetea estado del modal
- [x] 2.5 Paginación: indicador editable para ir a página (usa `scrollToPage`)

## 3. Tests (Vitest + RTL)

- [x] 3.1 Test: en estado ready existe `<img alt="Firma personal">`
- [x] 3.2 Test: no se renderiza texto con `blob:` ni `UrlTemporal`
- [x] 3.3 Test: no existe botón “Usar firma personal” y sí existe “Usar firma”
- [x] 3.4 Test: click “Usar firma” dispara placement (mocks) sin pasos extra
- [x] 3.5 Test: escribir número en paginación navega con `scrollToPage`

## 4. Documentación enterprise (SCRUMCORE-213)

- [x] 4.1 Crear `SCRUMCORE-213-Metadata.md` (branch/commits/tests)
- [x] 4.2 Crear `SCRUMCORE-213-Comportamiento-del-Componente.md` (preview + CTA único)
- [x] 4.3 Crear `SCRUMCORE-213-Arquitectura-Tecnica.md` (Mermaid UI simplificada)
- [x] 4.4 Crear `SCRUMCORE-213-Testing-Enterprise.md` (evidencias unit)
- [x] 4.5 Documentar paginación editable (UX + uso `scrollToPage`)
