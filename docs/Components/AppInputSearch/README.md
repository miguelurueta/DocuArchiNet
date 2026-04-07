# AppInputSearch

## Proposito

`AppInputSearch` es un componente reusable de la capa UI compartida para representar campos de busqueda sin que los consumidores repitan iconografia, estilos y reglas de accesibilidad sobre `AppInput`.

El componente compone `AppInput` y conserva su contrato base de input de texto. Su primera adopcion real esta en `AppTableQueryWrapper`, donde reemplaza el `AppInput` usado para filtrar la tabla sin cambiar el manejo externo de `queryState.search`.

## Ubicacion

- Implementacion: `src/app/Components/UI/AppInputSearch/AppInputSearch.tsx`
- Estilos: `src/app/Components/UI/AppInputSearch/AppInputSearch.module.css`
- Tests: `src/app/Components/UI/AppInputSearch/AppInputSearch.test.tsx`
- Export: `src/app/Components/UI/AppInputSearch/index.ts`
- Spec OpenSpec: `openspec/specs/app-input-search/spec.md`

## API publica

### `AppInputSearchProps`

`AppInputSearchProps` compone el contrato de texto de `AppInput` mediante `Omit<AppInputTextProps, "prefix" | "type">`.

- `value?: string`
  Valor controlado del campo de busqueda.
- `defaultValue?: string`
  Valor inicial cuando se usa como campo no controlado.
- `onChange?: ChangeEventHandler<HTMLInputElement>`
  Notifica cambios de texto al consumidor. El componente no administra el estado de busqueda internamente.
- `placeholder?: string`
  Texto auxiliar mostrado cuando el campo esta vacio.
- `label?: ReactNode`
  Label visible heredado de `AppInput`.
- `"aria-label"?: string`
  Nombre accesible cuando el campo no tiene label visible.
- `disabled?: boolean`
  Impide la interaccion y conserva la semantica accesible de campo deshabilitado.
- `error?: boolean`
  Activa el estado visual y semantico de error delegado por `AppInput`.
- `state?: "default" | "error"`
  Variante de estado heredada de `AppInput`.
- `helperText?: ReactNode`
  Texto de ayuda asociado al campo mediante `aria-describedby` cuando corresponde.
- `className?: string`
  Clase CSS adicional combinada con los estilos internos.
- `showIcon?: boolean`
  Controla si se renderiza el icono decorativo de busqueda. Por defecto es `true`.

## Ejemplo de uso

```tsx
import { AppInputSearch } from "../../../app/Components/UI/AppInputSearch";

export function SearchExample() {
  const [search, setSearch] = useState("");

  return (
    <AppInputSearch
      aria-label="Buscar documentos"
      placeholder="Buscar por radicado"
      value={search}
      onChange={(event) => setSearch(event.target.value)}
    />
  );
}
```

## Uso en AppTableQueryWrapper

`AppTableQueryWrapper` usa `AppInputSearch` como campo de busqueda de la tabla:

```tsx
<AppInputSearch
  className={styles.searchInput}
  placeholder={searchPlaceholder}
  value={queryState.search}
  onChange={(event) => onQueryChange({ search: event.target.value })}
  aria-label="Buscar en la tabla"
/>
```

Este uso conserva el mismo contrato previo del wrapper:

- el valor sigue viniendo de `queryState.search`
- los cambios siguen notificandose mediante `onQueryChange({ search })`
- `searchPlaceholder` sigue controlando el placeholder
- `showSearch={false}` evita renderizar el buscador

## Comportamiento

- Renderiza un input de texto basado en `AppInput`.
- No administra estado interno de busqueda.
- Permite uso controlado o no controlado siguiendo el contrato base de React y `AppInput`.
- Agrega un icono de busqueda decorativo mediante `SearchOutlined`.
- Permite ocultar el icono con `showIcon={false}`.
- Combina estilos propios con `className` externa.

## Accesibilidad

- El nombre accesible debe venir de `label` o de `"aria-label"`.
- El icono de busqueda se marca como decorativo con `aria-hidden="true"`.
- El icono no crea un control interactivo adicional.
- Los estados `disabled`, `error`, `helperText` y `aria-describedby` se delegan a `AppInput`.
- El componente debe seguir siendo consultable por rol de textbox y nombre accesible en pruebas de comportamiento.

## Cobertura de pruebas

Se validan al menos estos escenarios:

- render de valor controlado, placeholder y nombre accesible
- propagacion de `onChange` sin administrar estado interno
- preservacion de estados `disabled` y `error`
- icono de busqueda decorativo
- ocultamiento del icono con `showIcon={false}`
- composicion con `className` externa
- integracion en `AppTableQueryWrapper` con `[SPEC:app-input-search]`

## Notas

- `AppInputSearch` no usa `Input.Search` de Ant Design directamente; compone `AppInput` para mantener el contrato UI local.
- `prefix` permanece reservado para la composicion interna del icono. Los consumidores no deben usar `AppInputSearch` para prefijos arbitrarios.
- Si futuros consumidores requieren boton de limpiar, busqueda por Enter o debounce, debe ampliarse el contrato mediante una spec nueva antes de implementarlo.
