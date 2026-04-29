# SCRUMCORE-193 \u2014 AppVisorPdf API integration (04-FE)

## Goal
Crear una capa **API desacoplada** para `AppVisorPdf` alineada al consumo del repo:

- Usa `src/api/Clienteaxios.ts` como cliente HTTP (sin endpoints hardcodeados en UI).
- Retorna `ApiResponse<T>` en todos los m\u00e9todos.
- Propaga errores 400/401/403 de forma consistente para que el notificador central (`useAxiosErrorNotifier`) pueda actuar.
- Mantiene tipado estricto (sin `any`).

## Non-goals

- NO integrar `AppVisorPdf` en m\u00f3dulos/pantallas consumidoras.
- NO implementar endpoints reales del backend (solo adaptador listo + tests con mocks).

## Proposed structure

```
src/app/Components/UI/AppVisorPdf/
  domain/
    annotations.types.ts
    visorPdfApi.types.ts
  infrastructure/
    visorPdfApi.ts
```

## Key decisions

1. **Inversi\u00f3n de dependencias**
   - `AppVisorPdf` no importa axios directo.
   - El adaptador `infrastructure/visorPdfApi.ts` encapsula el uso de `clienteApi`.

2. **Contratos y DTOs**
   - `VisorPdfAnnotationsPayloadV1` se reutiliza desde `domain/annotations.types.ts`.
   - `VisorPdfStampConfig` y la interface `AppVisorPdfApi` viven en `domain/visorPdfApi.types.ts`.

3. **Errores**
   - El adaptador no "traduce" 401/403 a mensajes: los **propaga** (para notificador central).
   - Para 400 se mantiene un mapeo m\u00ednimo si el backend responde en `ApiResponse<T>` (sin acoplarse a un shape espec\u00edfico m\u00e1s all\u00e1 del envelope).

4. **Testing**
   - Pruebas unitarias por funci\u00f3n del API con `vi.mock` sobre `Clienteaxios` (patr\u00f3n del repo).
   - Happy path + verificaci\u00f3n de payloads enviados.
   - Error path: 401/403 se propagan.

