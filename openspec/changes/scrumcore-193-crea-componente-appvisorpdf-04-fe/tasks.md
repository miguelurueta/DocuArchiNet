## 1. Domain contracts

- [ ] 1.1 Crear `src/app/Components/UI/AppVisorPdf/domain/visorPdfApi.types.ts` con `VisorPdfStampConfig` + `AppVisorPdfApi`
- [ ] 1.2 Reutilizar `VisorPdfAnnotationsPayloadV1` desde `src/app/Components/UI/AppVisorPdf/domain/annotations.types.ts`

## 2. Infrastructure adapter

- [ ] 2.1 Implementar `src/app/Components/UI/AppVisorPdf/infrastructure/visorPdfApi.ts` usando `src/api/Clienteaxios.ts`
- [ ] 2.2 Mantener `ApiResponse<T>` en el resultado de cada m\u00e9todo (sin normalizar a datos planos)
- [ ] 2.3 Propagar errores 401/403 (sin capturarlos y convertirlos a success)
- [ ] 2.4 Implementar mapping m\u00ednimo para 400 si aplica, sin acoplarse a shapes no documentados

## 3. Tests

- [ ] 3.1 Test `getAnnotations`: retorna `ApiResponse<VisorPdfAnnotationsPayloadV1>` usando mock del cliente
- [ ] 3.2 Test `saveAnnotations`: env\u00eda payload correcto
- [ ] 3.3 Test: 401/403 se propagan (reject) para consumo por notificador central

## 4. Documentation

- [ ] 4.1 Actualizar `src/app/Components/UI/AppVisorPdf/README.md` con endpoints esperados y contratos `ApiResponse<T>`
- [ ] 4.2 Documentar ejemplo de mocks (vi.mock Clienteaxios) y manejo 400/401/403

