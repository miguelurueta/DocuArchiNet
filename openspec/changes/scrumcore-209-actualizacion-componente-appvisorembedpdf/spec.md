# SCRUMCORE-209 — Spec (Password Protected Plugin)

## Alcance
- Instalar y registrar `@embedpdf/plugin-password-protected`.
- Integrar el flujo password dentro de `AppVisorEmbedPdf` con UI mínima desacoplada.
- Mantener todo lo existente (zoom/rotate/thumbnails/print/export/pagination/virtualización).

## Instalación
- `npm install @embedpdf/plugin-password-protected`
- Validar `package.json` + `package-lock.json`.

## Estructura obligatoria (mínima)
Agregar únicamente:
`src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfPasswordPrompt.tsx`
`src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfPasswordPrompt.module.css`

## API obligatoria (prompt)
```ts
export interface AppPdfPasswordPromptProps {
  error?: boolean;
  isSubmitting?: boolean;
  onSubmit(password: string): void;
}
```

## Registro del plugin
Actualizar `src/app/Components/UI/AppVisorEmbedPdf/plugins/pluginRegistration.ts`:
- `createPluginRegistration(PasswordProtectedPluginPackage)`
- Mantener patrón existente de registro.

## Estados obligatorios (UX)
- `password-required`: prompt visible.
- `invalid-password`: prompt visible con `error=true`.
- `unlocked`: prompt oculto.
- `error/corrupt/engine error`: usar `ErrorState` existente (o extensión mínima).

## Hardening (taxonomy mínima)
Definir taxonomy de errores en el visor (sin romper APIs públicas):
- `PdfPasswordRequired`
- `PdfInvalidPassword`
- `PdfCorruptDocument`
- `PdfEngineError`

## Testing mínimo (Vitest/RTL)
Actualizar `AppVisorEmbedPdf.test.tsx`:
- Mock de plugin password (estado + provides/actions).
- Caso: password requerido → render prompt.
- Caso: submit exitoso → prompt desaparece.
- Caso: password inválido → error state visible en prompt.
- Caso: no afecta toolbar y plugins coexisten (render continúa).

## Documentación enterprise
Generar los 9 docs para `SCRUMCORE-209` bajo:
`docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/`
Incluye Mermaid:
- arquitectura
- flujo password
- secuencia unlock
- estados lifecycle

