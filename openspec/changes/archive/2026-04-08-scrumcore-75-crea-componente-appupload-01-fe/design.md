## Context

- Cambio OpenSpec para `SCRUMCORE-75` enfocado en crear `AppUpload` como componente reusable de UI.
- Existe la arquitectura funcional en `docs/Architecture/AppUpload/AppUpload-Architecture.md` y tickets FE/BE alineados.
- El componente debe ser wrapper desacoplado de Ant Design `Upload` con estado controlado y estrategias de carga.
- UI debe mantener consistencia visual con `AppInput` (radius 12px, focus/hover/disabled).

## Goals / Non-Goals

**Goals:**
- Implementar `AppUpload` como componente reusable y controlado en `src/app/Components/UI/AppUpload/`.
- Soportar estrategias de carga `auto`, `manual`, `customRequest` y state machine estricta.
- Proveer UI con preview tipo galeria, acciones por archivo y layout responsive (46/23/2 columnas).
- Asegurar validaciones (accept, maxSize, validateFile), eventos completos y performance para listas.

**Non-Goals:**
- Implementar endpoints backend ni integraciones de negocio.
- Migrar pantallas consumidoras o crear flujos completos de carga.
- Cambiar estilos globales o reemplazar AntD.

## Decisions

1. **Wrapper desacoplado de AntD Upload**
   - Se usa AntD `Upload` como base pero el estado se controla externamente via `value`.
   - Razon: estandarizar contrato reusable sin acoplar a endpoints o screens.

2. **State machine estricta por archivo**
   - Estados permitidos: `queued -> uploading -> done/error -> removed`.
   - Razon: evitar inconsistencias de UI y eventos duplicados.

3. **Eventos y validaciones en el contrato**
   - `onProgress`, `onSuccess`, `onError`, `beforeUpload`, `validateFile`, `accept`, `maxSize`.
   - Razon: extender comportamiento sin introducir logica de negocio.

4. **UI de galeria + responsive**
   - Grid/list, cards con `aspect-ratio: 1/1`, hover elevation, acciones overlay.
   - Razon: cumplir UX consistente y escalable.

5. **Performance**
   - `React.memo` en items y evitar re-render completo de lista.
   - Razon: manejar colecciones grandes sin degradacion visible.

## Risks / Trade-offs

- **Complejidad de estado** → Mitigacion: state machine estricta + pruebas de transiciones.
- **Performance con muchas filas** → Mitigacion: memoizacion de items y diff por `uid`.
- **Memory leaks por previews** → Mitigacion: limpiar object URLs al remover.
- **Accesibilidad incompleta** → Mitigacion: focus visible, aria-labels y soporte teclado.

## Migration Plan

- Implementar `AppUpload` en UI compartida sin romper contratos existentes.
- Exponer API estable para que los consumidores migren cuando sea necesario.
- No hay migracion obligatoria en esta fase.

## Open Questions

- ¿Se requiere un preset de estilo adicional (compact/table) aparte de `sm/md/lg`?
- ¿Se necesita virtualizacion si la lista supera cierto umbral?
