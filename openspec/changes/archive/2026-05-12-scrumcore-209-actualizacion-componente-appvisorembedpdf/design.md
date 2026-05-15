# SCRUMCORE-209 — Design (Password Protected via DocumentManager)

## Objetivo
Agregar soporte enterprise para PDFs protegidos con contraseña en `AppVisorEmbedPdf` usando **exclusivamente** el flujo oficial del `DocumentManager`:

`openDocumentUrl({ url, password, autoActivate: true })`

## Restricciones (no negociables)
- No existe paquete/plugin npm independiente para password protection.
- Prohibido implementar decrypt/custom crypto/pdf.js/parsers/hacks WASM.
- No romper ni alterar: virtualización, zoom, rotate, thumbnails, print, export, toolbar, paginación.
- Encapsulación: `DocumentosWorkbench` no conoce password ni estados internos.

## UX
Cuando el documento requiere password:
- Mostrar `AppPdfPasswordPrompt` como overlay dentro del visor.
- Capturar password + submit.
- Reintentar `openDocumentUrl` con el password.
- Si password inválido: mostrar feedback y permitir reintento.

## Arquitectura
- `AppVisorEmbedPdf.tsx` administra el flujo (detecta error/password requerido y reintenta).
- `AppPdfPasswordPrompt` es presentacional (solo input + botón + feedback).
- Mantener memoización para evitar rerenders por scroll/virtualización.

## Accesibilidad
- Input password con `aria-label`.
- Botón submit con `aria-label`.
- `role="alert"` para mensaje de password inválido.
- Focus automático al input cuando el prompt aparece.

