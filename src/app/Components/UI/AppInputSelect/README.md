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

## Integracion backend

- use `fetchOptions(query)` para búsquedas remotas
- adapte el DTO backend a `{ label, value, disabled?, meta? }`
- si necesita debounce o cancelación, resuélvalo en el contenedor
