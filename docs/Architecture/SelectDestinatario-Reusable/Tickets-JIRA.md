# Ticket 01 FE

## Titulo

Extraer y reutilizar `AppInputTags` como componente UI

## Objetivo

Sacar `SelectDestinatario` y `BaseSelectUsuarios` de `RadicacionForm.tsx`, convertirlos en `AppInputTags` dentro de `src/app/Components/UI` y documentar su API para que cualquier formulario del repo pueda consumirlo sin reimplementar la lógica de tags y dropdown.

## Contexto existente

- La lógica actual vive íntegra dentro de `RadicacionForm.tsx` y maneja tags, menú de información y apertura manual del select.
- El componente se renderiza desde un `<Card>` con reglas, identificadores y modal de información.
- Ya contamos con `AppInputSearch` y un contrato genérico de autocomplete que se puede complementar para alimentar las opciones.

## Restricciones (obligatorio)

- No duplicar la lógica de tags, dropdown y menú contextual en otros forms.
- El componente debe seguir usando `AutoComplete` y Ant Design.
- El componente debe poder integrarse en `AppToolbar` y `AppDropdown`.
- Mantener los `data-ident` actuales y la accesibilidad (`aria-label`).

## Ubicacion (obligatoria)

```txt
src/app/Components/UI/AppInputTags/
```

## Contratos (obligatorios)

```ts
type AppInputTagsProps = {
  name: string;
  label: React.ReactNode;
  mode?: "single" | "multiple";
  options: { label: string; value: string }[];
  rules?: Rule[];
  minLength?: number;
  debounceMs?: number;
  loading?: boolean;
  toolbar?: {
    render: () => React.ReactNode;
  };
  onAddTag: (tag: string) => void;
  onRemoveTag: (tag: string) => void;
  onRemoveAll: () => void;
  abrirInformacion: (id: number) => void;
  selectDisabled?: boolean;
  formItemDataIdent?: string;
  selectDataIdent?: string;
};
```

## Reglas de implementacion (obligatorio)

- La adición/eliminación de tags no debe depender de eventos `KeyPress`.
- El componente debe exponer métodos para agregar tags manualmente y eliminar etiquetas ya existentes.
- El dropdown masivo (`AppDropdown`) debe incluir opción de “Eliminar todos”.
- Mantener la lógica de menú contextual con “Información” y “Cerrar”.
- El componente debe aparecer dentro de un `AppToolbar` o proveer un slot visual para acciones adicionales.

## Riesgos a evitar

- Reinsertar botones de edición/borrado donde no se desean.
- Romper la validación dinámica que hoy usa `destinatarioRequired`.
- Fundir la lógica de autocomplete dentro del componente; debe mantenerse desacoplado a APIs.

## Pruebas obligatorias

- El modo `single` no permite más de una etiqueta.
- `onAddTag`/`onRemoveTag` funcionan desde botones/dropdown, no desde keypress.
- La acción “Eliminar todos” vacía el campo sin recargar.
- El loading se muestra mientras se consultan sugerencias.
- El toolbar con `AppDropdown` renderiza y ejecuta acciones.

## Criterios de aceptacion

- Componente disponible desde `src/app/Components/UI`.
- `RadicacionForm` lo usa sin perder el modal de información.
- Nuevos forms pueden reutilizarlo sin duplicar tags o dropdown.

# Ticket 02 FE

## Titulo

Integrar Autocomplete + debounce + minLength configurable

## Objetivo

Conectar `AppInputTags` con el contrato genérico de autocomplete existente, permitiendo que el componente dispare `onSearch` cuando el usuario escribe y reciba `options` desde cualquier API mediante el hook.

## Contexto existente

- Ya se definió `AppAutocompleteRequest` y `AppAutocompleteOption` en la documentación de `AppInputSearch`.
- `AppInputTags` debe consumir ese contrato y solo mostrar `options`, `loading` y comportarse según `mode`.

## Restricciones (obligatorio)

- No acoplar a endpoints específicos.
- Exponer props `debounceMs` y `minLength` para controlar cuándo se dispara `onSearch`.
- `loading` debe influenciar el icono de `AutoComplete` y no bloquear el input.

## Reglas de implementacion (obligatorio)

- El componente llama a `onSearch(query)` cuando el texto alcanza `minLength` tras un `debounce` configurado.
- `options` debe ser un array normalizado (`{ label, value }`) que puede venir de cualquier API.
- `onSearch` es invocado solo desde dentro del componente, pero la consulta la maneja el padre.

## Pruebas obligatorias

- Sin alcanzar `minLength` no se dispara `onSearch`.
- `debounceMs` controla el tiempo entre escrituras y llamadas.
- `loading` true muestra spinner en suffix del input.

## Criterios de aceptacion

- Cualquier hook que respeta el contrato genérico puede alimentar el componente.
- `RadicacionForm` usa el hook actual (`useAutocompleteCamposPlantilla`) para obtener `options`.
- El componente mantiene su comportamiento independientemente de la API.

# Ticket 03 BE

## Titulo

Normalizar adaptadores backend para el autocomplete del nuevo componente

## Objetivo

Confirmar y documentar los payloads/respuestas de los endpoints de autocomplete existentes para alimentar `AppInputTags` sin modificar su implementación.

## Contexto existente

- Existen tres endpoints principales (`solicitaAutoCompleteCampos`, `autoCompleteTercero`, `solicitaAutoCompleteDestinatarioRestriccion`).
- El hook actual (`useAutocompleteCamposPlantilla`) ya mapea a resultados normalizados.

## Restricciones (obligatorio)

- Mantener la compatibilidad semántica actual (campos/metadatos) con los sitios que consumen estos endpoints.
- Los adaptadores deben devolver `{ value, label }` y no obligar al componente a conocer las diferencias.

## Reglas de implementacion (obligatorio)

- Definir mappers explícitos desde `AppAutocompleteRequest` hacia cada payload conocido.
- Normalizar la respuesta a `AppAutocompleteOption` (`label = texValue`, `value = idValue` o similar).
- Documentar cualquier campo adicional necesario (IDs, restricciones, contexto).

## Pruebas obligatorias

- Validar los hooks con mocks que emulen cada endpoint.
- Asegurar que `AppInputTags` recibe opciones incluso si la API responde con estructuras heterogéneas.

## Criterios de aceptacion

- Existen adaptadores por endpoint documentados en `docs/Architecture/AppInputSearch/AppInputSearch-Architecture.md`.
- Be respeta el contrato sin cambios en el componente.

