# AppInputSelect

Wrapper reusable sobre `Select` de Ant Design para opciones locales y remotas.

## Casos de uso

- formularios con catálogos simples
- filtros con búsqueda remota
- selección múltiple con apariencia consistente

## Ejemplo básico

```tsx
<AppInputSelect
  label="Tipo"
  placeholder="Seleccione una opcion"
  size="md"
  options={[
    { label: "Radicado", value: "radicado" },
    { label: "Expediente", value: "expediente" },
  ]}
/>
```

## Ejemplo con formulario

```tsx
<Form.Item label="Dependencia" name="dependenciaId">
  <AppInputSelect
    placeholder="Seleccione una dependencia"
    options={dependencias}
    allowClear
  />
</Form.Item>
```

## Ejemplo remoto

```tsx
<AppInputSelect
  aria-label="Buscar tercero"
  searchable
  fetchOptions={async (query) => {
    const response = await api.get("/terceros", { params: { q: query } });
    return {
      options: response.data.items.map((item) => ({
        label: item.nombre,
        value: item.id,
      })),
    };
  }}
/>
```

## Ejemplo multiple

```tsx
<AppInputSelect
  mode="multiple"
  size="lg"
  allowClear
  options={rolesOptions}
  placeholder="Seleccione roles"
/>
```

## Ejemplo con estado vacio custom

```tsx
<AppInputSelect
  aria-label="Buscar tercero"
  searchable
  noDataText="No se encontraron coincidencias"
  fetchOptions={buscarTerceros}
/>
```

## Integracion backend

- use `fetchOptions(query)` para búsquedas remotas
- adapte el DTO backend a `{ label, value, disabled?, meta? }`
- si necesita debounce o cancelación, resuélvalo en el contenedor

Ejemplo de adaptación:

```ts
const options = response.data.items.map(toAppInputSelectOption);
```

## Accesibilidad

- use `aria-label` o `aria-labelledby` cuando no exista label visible
- use `helperText` para enlazar soporte o error al control
- preserve `disabled`, `loading`, `status` y `error` como estados del wrapper

## Troubleshooting

- si los datos no cargan: valide el `fetchOptions` y la adaptación del DTO
- si hay resultados vacíos: revise si `fetchOptions` devuelve `options: []`
- si quiere debounce o cancelación: resuélvalo en el contenedor
