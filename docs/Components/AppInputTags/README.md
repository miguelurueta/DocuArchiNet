# AppInputTags

## Proposito

`AppInputTags` es un componente reusable de la capa UI compartida para capturar una o varias etiquetas mediante `AutoComplete` + `Input` de Ant Design, con confirmacion manual, sugerencias, loading no bloqueante y acciones accesibles para eliminar tags.

El componente es presentacional: no consume APIs, no conoce endpoints y no importa hooks de dominio. Los consumidores son responsables de consultar servicios, normalizar opciones y pasar `options`, `loading` y callbacks.

## Ubicacion

- Implementacion: `src/app/Components/UI/AppInputTags/AppInputTags.tsx`
- Estilos: `src/app/Components/UI/AppInputTags/AppInputTags.module.css`
- Tests: `src/app/Components/UI/AppInputTags/AppInputTags.test.tsx`
- Export: `src/app/Components/UI/AppInputTags/index.ts`
- Export compartido: `src/app/Components/UI/index.ts`
- Spec OpenSpec: `openspec/specs/app-input-tags/spec.md`

## API publica

### `AppInputTagsProps`

- `name?: string`
  Nombre logico del campo cuando el consumidor lo integra con formularios.
- `label?: ReactNode`
  Label visible asociado al input.
- `value?: string[]`
  Lista controlada de tags visibles. Si se provee, el consumidor mantiene la fuente de verdad.
- `defaultValue?: string[]`
  Lista inicial para modo no controlado.
- `mode?: "single" | "multiple"`
  En `single`, un nuevo tag reemplaza el valor actual. En `multiple`, se acumulan tags sin duplicar.
- `options?: AppInputTagsOption[]`
  Opciones normalizadas de autocomplete.
- `placeholder?: string`
  Texto auxiliar del input.
- `minLength?: number`
  Longitud minima para emitir `onSearch`.
- `debounceMs?: number`
  Retardo para busqueda por escritura. `0` o `undefined` no espera debounce.
- `loading?: boolean`
  Muestra indicador visual sin bloquear el input.
- `clearOnEscape?: boolean`
  Permite limpiar el texto actual con `Escape`.
- `disabled?: boolean`
  Deshabilita el componente completo.
- `selectDisabled?: boolean`
  Deshabilita interaccion del input/select sin depender de `loading`.
- `size?: "sm" | "md" | "lg"`
  Variante visual de altura, padding e iconos.
- `error?: boolean`
  Activa estado visual y semantico de error.
- `state?: "default" | "error"`
  Variante de estado equivalente para integraciones que usen `state`.
- `helperText?: ReactNode`
  Texto de ayuda o error asociado mediante `aria-describedby`.
- `className?: string`
  Clase externa combinada con el wrapper del componente.
- `toolbar?: { render: () => ReactNode }`
  Slot para acciones secundarias, por ejemplo un `AppDropdown`.
- `onAddTag?: (tag: string) => void`
  Callback al agregar tag por Enter o seleccion de sugerencia.
- `onRemoveTag?: (tag: string) => void`
  Callback al eliminar un tag individual.
- `onRemoveAll?: () => void`
  Callback al activar la eliminacion masiva.
- `onSearch?: (query: string) => void`
  Callback de busqueda delegado al consumidor.
- `abrirInformacion?: (id: number) => void`
  Callback de compatibilidad para consumidores que necesiten abrir informacion asociada. El componente base no debe conocer el origen de los ids.
- `formItemDataIdent?: string`
  Identificador de pruebas/telemetria aplicado al wrapper.
- `selectDataIdent?: string`
  Identificador de pruebas/telemetria aplicado al input.
- `"aria-label"?: string`
  Nombre accesible cuando no hay label visible.
- `"aria-labelledby"?: string`
  Id externo que nombra el input.

### `AppInputTagsOption`

```ts
type AppInputTagsOption = {
  label: string;
  value: string;
  id?: number;
  meta?: Record<string, unknown>;
};
```

`label` y `value` son los unicos campos que `AppInputTags` necesita para renderizar y seleccionar sugerencias. `id` y `meta` permiten que los consumidores preserven datos normalizados sin acoplar el componente a estructuras de backend.

## Ejemplo basico

```tsx
import { AppInputTags } from "../../../app/Components/UI/AppInputTags";

export function TagsExample() {
  const [tags, setTags] = useState<string[]>([]);

  return (
    <AppInputTags
      aria-label="Destinatarios"
      mode="multiple"
      placeholder="Buscar destinatario"
      value={tags}
      onAddTag={(tag) => setTags((current) => [...current, tag])}
      onRemoveTag={(tag) => setTags((current) => current.filter((item) => item !== tag))}
      onRemoveAll={() => setTags([])}
    />
  );
}
```

## Uso con autocomplete

El autocomplete debe resolverse fuera del componente. Un hook de dominio consulta la API, normaliza la respuesta y entrega `options` + `loading`:

```tsx
<AppInputTags
  aria-label="Destinatario"
  mode="single"
  minLength={3}
  debounceMs={250}
  loading={autocomplete.isLoading}
  options={autocomplete.options}
  onSearch={(query) => autocomplete.setSearchText(query)}
  onAddTag={(tag) => form.setFieldValue("destinatario", [tag])}
  onRemoveAll={() => form.resetFields(["destinatario"])}
/>
```

Reglas de integracion:

- `AppInputTags` no arma payloads HTTP.
- `AppInputTags` no importa hooks como `useAutocompleteCamposPlantilla`.
- El consumidor transforma cualquier respuesta externa a `AppInputTagsOption[]`.
- `loading` no debe deshabilitar el input; use `disabled` o `selectDisabled` para bloquear interaccion.

## Acciones secundarias

El slot `toolbar` permite agregar acciones sin mezclar logica de dominio dentro del componente:

```tsx
<AppInputTags
  aria-label="Destinatarios"
  toolbar={{
    render: () => (
      <AppDropdown
        ariaLabel="Acciones de destinatarios"
        trigger={<AppButton variant="ghost">Acciones</AppButton>}
        items={[{ key: "clear", label: "Borrar todos", onSelect: handleClear }]}
      />
    ),
  }}
/>
```

Las acciones deben seguir siendo accesibles y no deben bloquear la escritura ni el autocomplete.

## Comportamiento

- Soporta uso controlado con `value` y no controlado con `defaultValue`.
- `mode="single"` reemplaza el tag actual.
- `mode="multiple"` agrega tags sin duplicar visualmente.
- `onSearch` se dispara por escritura cuando cumple `minLength`.
- `debounceMs` aplica solo a escritura.
- Enter y click en el icono de busqueda cancelan debounce pendiente y ejecutan busqueda inmediata.
- Clear y `Escape` no ejecutan `onSearch("")` automaticamente.
- `loading` muestra `Spin` sin bloquear el input.
- `disabled` y `selectDisabled` tienen prioridad sobre loading.

## Accesibilidad

- El input debe tener nombre accesible mediante `label`, `"aria-label"` o `"aria-labelledby"`.
- Cada tag se renderiza dentro de una lista accesible de etiquetas seleccionadas.
- La accion de remover tag expone un nombre como `Eliminar <tag>`.
- La accion masiva expone `aria-label="Eliminar todos"`.
- Las sugerencias mantienen la navegacion de teclado de `AutoComplete`.
- Los estilos son locales al CSS module del componente.

## Cobertura de pruebas

Se validan al menos estos escenarios:

- disponibilidad del componente y export compartido
- modo controlado y no controlado
- modos `single` y `multiple`
- `onAddTag`, `onRemoveTag` y `onRemoveAll`
- `onSearch`, `minLength`, `debounceMs`, Enter/click inmediato y cancelacion de debounce
- clear, `clearOnEscape`, loading y disabled
- render de sugerencias y seleccion por autocomplete
- metadata opcional en `options`
- desacoplamiento de hooks/servicios de dominio
- slot de acciones secundarias sin bloquear el input
- clases de size, error, helper text y accesibilidad

## Notas

- No usar `Input.Search` de Ant Design para este componente.
- No introducir llamadas HTTP ni endpoints conocidos dentro de `AppInputTags`.
- Para respuestas heterogeneas de backend, cree un hook o mapper en el consumidor y entregue opciones normalizadas al componente.
