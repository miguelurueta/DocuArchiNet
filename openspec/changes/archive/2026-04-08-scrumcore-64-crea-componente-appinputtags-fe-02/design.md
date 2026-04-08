## Context

`SCRUMCORE-64` corresponde al Ticket 02 FE de `AppInputTags`: integrar el componente con un contrato generico de autocomplete y acciones secundarias sin acoplarlo a endpoints.

El Ticket 01 FE ya creo `AppInputTags` en `src/app/Components/UI/AppInputTags/` y sincronizo la capability principal `app-input-tags`. Por eso este cambio no debe crear un segundo componente ni una capability nueva con el nombre generado `AppAppinputtagsFe02`. El scope correcto es evolucionar el componente existente y su spec `app-input-tags`.

El contexto funcional viene de `docs/Architecture/SelectDestinatario-Reusable/Ticket-02-FE-AutoComplete.md` y `docs/Architecture/SelectDestinatario-Reusable/AppInputTags-reqs.md`. La referencia de consumo real esta en `RadicacionForm.tsx`, donde `useAutocompleteCamposPlantilla` ya normaliza resultados de autocomplete para campos dinamicos.

## Goals / Non-Goals

**Goals:**

- Mantener `AppInputTags` como componente UI presentacional y desacoplado de APIs.
- Estabilizar el contrato de opciones como `AppAutocompleteOption` compatible con `{ value, label, meta }`.
- Permitir que cualquier hook padre entregue `options` y `loading` sin que el componente conozca endpoint, payload ni servicio.
- Conservar `onSearch(query)`, `minLength` y `debounceMs` como mecanismo de solicitud de sugerencias desde el consumidor.
- Mantener Enter y click en icono como eventos inmediatos que cancelan debounce pendiente.
- Mantener el input editable durante `loading`.
- Exponer acciones secundarias mediante slot/toolbar o integración con `AppDropdown`, sin acoplar logica de dominio.
- Preservar accesibilidad, `data-ident`, estilos locales y consistencia visual con `AppInput`.
- Cubrir el contrato con pruebas unitarias de `AppInputTags` y, si se toca un consumidor, pruebas del consumidor.

**Non-Goals:**

- No crear `AppAppinputtagsFe02` ni una carpeta nueva de componente.
- No introducir endpoints conocidos dentro de `AppInputTags`.
- No mover logica de `useAutocompleteCamposPlantilla` hacia la capa UI.
- No modificar payloads backend ni normalizadores de dominio salvo que una prueba revele incompatibilidad directa.
- No migrar todos los consumidores del proyecto en este ticket.
- No introducir dependencias nuevas para debounce o autocomplete.

## Decisions

### 1. Modificar la capability existente `app-input-tags`

Este cambio debe extender `openspec/specs/app-input-tags/spec.md` porque `SCRUMCORE-63` ya creo el componente y la spec principal. El proposal generado automaticamente fue corregido para usar `app-input-tags` como capability modificada.

Alternativa considerada: aceptar `app-appinputtags-fe-02` como capability nueva. Se descarta porque fragmentaria el contrato del mismo componente y podria crear specs duplicadas para `AppInputTags`.

### 2. `AppInputTags` sigue siendo presentacional

El componente solo debe recibir `options`, `loading`, `minLength`, `debounceMs` y callbacks. Cualquier hook, incluyendo `useAutocompleteCamposPlantilla`, es responsable de consultar APIs y normalizar respuestas.

Alternativa considerada: inyectar el hook de radicacion en el componente. Se descarta porque acoplaria un componente reusable de `src/app/Components/UI` al modulo `radicacion`.

### 3. Contrato generico de opciones

Se usara un contrato UI compatible con:

```ts
type AppAutocompleteOption = {
  value: string;
  label: string;
  meta?: Record<string, unknown>;
};
```

`AppInputTagsOption` puede evolucionar para aceptar `meta` sin romper consumidores actuales. Si el consumidor necesita ids, estos deben viajar como metadata u option extendida y ser interpretados fuera del componente base.

Alternativa considerada: mantener solo `{ value, label, id }`. Se conserva por compatibilidad, pero no debe impedir `meta`, porque el ticket pide opciones desde multiples APIs.

### 4. Debounce en el componente para solicitud de sugerencias

Para `AppInputTags`, `debounceMs` controla cuando se emite `onSearch(query)`. El hook padre decide que hacer con esa busqueda. Enter y click en icono cancelan el debounce pendiente y disparan la busqueda inmediata si cumple `minLength`.

Alternativa considerada: mover todo debounce al hook padre. Se descarta para este componente porque el Ticket 02 FE pide explicitamente que `debounceMs` y `minLength` gobiernen el disparo de `onSearch` desde `AppInputTags`. El componente no consulta API; solo regula eventos.

### 5. Loading no bloqueante

`loading` solo debe mostrar indicador visual en el suffix o zona de acciones. La interaccion se bloquea unicamente con `disabled` o `selectDisabled`.

Alternativa considerada: deshabilitar el control mientras carga. Se descarta porque degrada escritura rapida y rompe el requerimiento de no bloquear input durante loading.

### 6. Acciones secundarias como composicion

El componente debe permitir acciones secundarias mediante slot local, `toolbar` o items compatibles con `AppDropdown`. La accion "Eliminar todos" puede seguir existiendo como accion accesible, pero acciones de dominio deben venir del consumidor.

Alternativa considerada: que `AppInputTags` conozca acciones como "abrir informacion" por endpoint o modal. Se descarta como patron principal; si se mantiene una callback de compatibilidad, debe seguir siendo presentacional.

## Risks / Trade-offs

- [Risk] Duplicar specs/capabilities para el mismo componente -> Mitigation: usar `app-input-tags` como capability modificada y corregir el naming del proposal.
- [Risk] Acoplar `AppInputTags` a `RadicacionForm` o `useAutocompleteCamposPlantilla` -> Mitigation: dejar hooks y servicios en el consumidor; `AppInputTags` solo recibe datos normalizados.
- [Risk] Dobles ejecuciones de `onSearch` por debounce + Enter/click -> Mitigation: cancelar timers pendientes antes de disparos inmediatos y cubrir con fake timers.
- [Risk] Mutar `options` o `meta` -> Mitigation: mapear opciones de forma inmutable y no reescribir props.
- [Risk] `loading` bloquea el input por accidente -> Mitigation: probar que `loading=true` mantiene escritura y foco.
- [Risk] Acciones secundarias rompen accesibilidad -> Mitigation: exigir `aria-label` en acciones de limpiar/eliminar y usar `AppDropdown` cuando haya menu de acciones.

## Migration Plan

1. Extender la spec `app-input-tags` con requisitos de autocomplete generico, metadata opcional y composicion de acciones.
2. Ajustar `AppInputTags` solo si el contrato actual no cubre `meta`, acciones secundarias o pruebas del Ticket 02 FE.
3. Mantener consumidores actuales sin migracion masiva.
4. Si se integra `RadicacionForm`, hacerlo usando `useAutocompleteCamposPlantilla` como fuente de `options` y `loading`, sin mover logica de request al componente UI.
5. Ejecutar pruebas focales de `AppInputTags` y de cualquier consumidor tocado.

Rollback: revertir la integracion del consumidor si aparece una regresion funcional. El contrato UI debe permanecer reusable y no alterar rutas ni servicios.

## Open Questions

- Si `AppInputTagsOption.meta` debe ser `Record<string, unknown>` o un generic tipado por consumidor.
- Si `onAddTag` debe seguir recibiendo solo `value` o debe soportar una variante con el option completo seleccionado.
- Si las acciones secundarias deben mantenerse como `toolbar.render()` o evolucionar a `dropdownItems` compatibles con `AppDropdown`.
