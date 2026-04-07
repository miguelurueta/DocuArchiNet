# PROMPT ARQUITECTÓNICO  Ticket 02 FE
# Integrar AppInputTags con Autocomplete genérico y AppDropdown

Rol esperado:
Arquitecto de software senior frontend (React, hooks, accesibilidad, testing)


OBJETIVO

Conectar `AppInputTags` con el contrato genérico de autocomplete (`AppAutocompleteRequest` / `AppAutocompleteOption`), garantizando que pueda tomar `options` desde cualquier API y usar `debounceMs` + `minLength` configurables, manteniendo el loading y sin acoplarse a endpoints.


CONTEXTO EXISTENTE

- El documento técnico: `docs/Architecture/SelectDestinatario-Reusable/AppInputTags-reqs.md` describe la necesidad de `AppDropdown`, eliminación masiva y estado de loading.
- Ya se definió el contrato genérico en `docs/Architecture/AppInputSearch/AppInputSearch-Architecture.md`.
- El hook actual (`useAutocompleteCamposPlantilla`) normaliza resultados a `{ value, label }`.


UBICACIÓN (OBLIGATORIA)

```
src/app/Components/UI/AppInputTags/
```


RESTRICCIONES (OBLIGATORIAS)

- no acoplar a endpoints específicos
- no bloquear el input durante loading
- no duplicar la lógica de debounce fuera del control
- mantener accesibilidad y `data-ident`


CONTRATO (OBLIGATORIO)

El componente interactúa con:

- `AppAutocompleteRequest` (query, fieldName, context, searchFields)
- `AppAutocompleteOption` ({ value, label, meta })
- Prop `options` que recibe las opciones normalizadas
- Prop `loading` para el estado visual
- Prop `onSearch` que se invoca con un string
- Props `debounceMs` y `minLength` para controlar cuándo se dispara la búsqueda


REGLAS DE IMPLEMENTACIÓN (OBLIGATORIAS)

1. Autocomplete y debounce
   - `onSearch` solo se ejecuta si el texto alcanza `minLength`
   - `debounceMs` controla la pausa tras escritura
   - Enter/click en icono son inmediatos y cancelan debounce

2. Opciones desde cualquier API
   - `options` es un array plano; no se manipula dentro del componente
   - el hook del padre maneja la consulta y pasa `loading` + `options`

3. Loading
   - se muestra un spinner en el suffix (ej. `Spin` o `LoadingOutlined`)
   - el input permanece activo

4. AppDropdown
   - el componente debe exponer slots para acciones (eliminación masiva, filtros)
   - `AppDropdown` se usa para agrupar acciones adicionales y resguardar accesibilidad


PRUEBAS UNITARIAS (OBLIGATORIAS)

- `onSearch` no se dispara antes de `minLength`
- `debounceMs` controla el retraso
- Enter/click bypassan debounce
- `loading` true muestra spinner sin bloquear input
- `options` se renderiza desde múltiples estructuras


PRUEBAS QT (CALIDAD / E2E)

- Hook simulado responde con delay y `options`
- Loading visible mientras el hook consulta
- `options` vacías no rompen el control


CRITERIOS DE ACEPTACIÓN

- `AppInputTags` puede usarse con cualquier hook que devuelva `AppAutocompleteOption`
- `RadicacionForm` integra el hook existente y pasa `loading` + `options`
- el componente continúa alineado con `AppDropdown` y `AppToolbar`
