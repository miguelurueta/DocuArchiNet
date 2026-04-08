# AppInputSearch

## Proposito

`AppInputSearch` es un componente reusable de la capa UI compartida para representar campos de busqueda con autocomplete presentacional, eventos deterministas y estilos alineados con `AppInput`.

El componente compone `AutoComplete` + `Input` de Ant Design, pero no consume APIs, no conoce endpoints y no contiene reglas de dominio.

## Ubicacion

- Implementacion: `src/app/Components/UI/AppInputSearch/AppInputSearch.tsx`
- Estilos: `src/app/Components/UI/AppInputSearch/AppInputSearch.module.css`
- Tests: `src/app/Components/UI/AppInputSearch/AppInputSearch.test.tsx`
- Export: `src/app/Components/UI/AppInputSearch/index.ts`
- Spec OpenSpec: `openspec/specs/app-input-search/spec.md`

## API publica

```ts
type AppInputSearchOption = {
  value: string;
  label?: string;
};

type AppInputSearchProps = {
  value?: string;
  defaultValue?: string;
  placeholder?: string;
  disabled?: boolean;
  autoFocus?: boolean;
  debounceMs?: number;
  minLength?: number;
  loading?: boolean;
  clearOnEscape?: boolean;
  options?: AppInputSearchOption[];
  onChange?: (value: string) => void;
  onSearch?: (value: string) => void;
  onClear?: () => void;
  onFocus?: () => void;
  onBlur?: () => void;
  size?: "sm" | "md" | "lg";
  label?: ReactNode;
  helperText?: ReactNode;
  error?: boolean;
  state?: "default" | "error";
  className?: string;
  "aria-label"?: string;
  "aria-labelledby"?: string;
};
```

Valores por defecto:

- `size = "md"`
- `debounceMs = 0`
- `minLength = undefined`
- `loading = false`
- `clearOnEscape = false`

## Eventos

- `onChange(value)` se ejecuta en cada cambio de texto.
- `onSearch(value)` se ejecuta por Enter, click en el icono de busqueda, seleccion de opcion y debounce de escritura cuando `debounceMs > 0`.
- Enter y click en el icono cancelan o neutralizan el debounce pendiente para evitar duplicados.
- `minLength` bloquea busquedas cortas en todos los caminos de `onSearch`.
- Clear ejecuta `onChange("")` y `onClear()`, pero no ejecuta `onSearch("")` automaticamente.
- Escape limpia solo cuando `clearOnEscape = true`.

## Ejemplo

```tsx
import { AppInputSearch } from "../../../app/Components/UI/AppInputSearch";

export function SearchExample() {
  const [search, setSearch] = useState("");

  return (
    <AppInputSearch
      aria-label="Buscar documentos"
      placeholder="Buscar por radicado"
      value={search}
      debounceMs={300}
      minLength={3}
      onChange={setSearch}
      onSearch={(value) => {
        // El consumidor decide que hacer con la busqueda.
        console.log(value);
      }}
    />
  );
}
```

## Uso en AppTableQueryWrapper

`AppTableQueryWrapper` usa `AppInputSearch` como campo de busqueda de tabla cuando `showSearch` esta activo:

```tsx
<AppInputSearch
  className={styles.searchInput}
  placeholder={searchPlaceholder}
  value={queryState.search}
  onChange={(search) => onQueryChange({ search })}
  aria-label="Buscar en la tabla"
/>
```

Cuando `showSearch={false}`, el wrapper no renderiza el buscador.

## Uso en GestionCorrespondencia

`GestionCorrespondencia` renderiza el buscador en `AppToolbar.actionContent`:

```tsx
<AppInputSearch
  aria-label="Buscar tareas workflow"
  className={styles.toolbarSearch}
  placeholder="Buscar tareas workflow"
  value={table.queryState.search}
  onChange={(search) => table.onQueryChange({ search })}
/>
```

La pantalla no consume APIs desde el buscador. La consulta real sigue el flujo del hook de tabla.

## Accesibilidad

- El componente requiere un nombre accesible mediante `aria-label`, `aria-labelledby` o `label`.
- Al usar `AutoComplete`, el input expone semantica de `combobox`.
- El boton de clear usa `aria-label="Limpiar"`.
- El boton de busqueda usa `aria-label="Buscar"`.
- `disabled` tiene prioridad sobre `loading`.
- `loading` es visual y no bloquea la escritura.

## Cobertura

Las pruebas cubren:

- modo controlado y no controlado
- `onChange(value)`
- `onSearch` por Enter, click y debounce
- cancelacion de debounce pendiente
- `minLength`
- clear y Escape
- loading editable
- `options` sin mutacion
- variantes de tamano
- integracion con `AppTableQueryWrapper`
- integracion con `GestionCorrespondencia`
