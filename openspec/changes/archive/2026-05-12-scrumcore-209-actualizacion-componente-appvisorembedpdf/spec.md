# SCRUMCORE-209 — Spec (Password Protected via DocumentManager)

## Alcance
Extender `AppVisorEmbedPdf` para soportar PDFs protegidos con contraseña usando DocumentManager oficial de EmbedPDF.

## Implementación obligatoria
### 1) Detección/flujo
- Abrir documento con `provides.openDocumentUrl({ url, name, autoActivate: true })`.
- Ante fallo que corresponda a password required/invalid:
  - Mostrar prompt.
  - Reintentar `openDocumentUrl({ url, password, autoActivate: true })`.

### 2) UI mínima (desacoplada)
Crear:
- `src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfPasswordPrompt.tsx`
- `src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfPasswordPrompt.module.css`

API:
```ts
export interface AppPdfPasswordPromptProps {
  isInvalidPassword?: boolean;
  isLoading?: boolean;
  onSubmit(password: string): void;
}
```

### 3) Estados obligatorios
- engine loading / error (ya existen)
- document loading (ya existe)
- password required
- invalid password
- success

## Testing (Vitest/RTL)
Actualizar `AppVisorEmbedPdf.test.tsx`:
- Mock de `useDocumentManagerCapability().provides.openDocumentUrl`
- Simular:
  - documento protegido: primer open falla y el componente muestra prompt
  - submit password: se llama openDocumentUrl con `password`
  - password inválido: prompt muestra feedback `isInvalidPassword`

## Documentación enterprise
Generar `SCRUM-SCRUMCORE-209-*.md` (9 archivos) en:
`docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/`
Incluye Mermaid: arquitectura/flujo/secuencia/estados.

