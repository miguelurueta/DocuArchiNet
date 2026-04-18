# Ticket 01 FE

## Titulo

Implementar `AppInputSelect` core como componente reusable sobre Ant Design

## Objetivo

Crear el componente real `AppInputSelect` en la capa shared de UI, basado en
`Select` de Ant Design, con API tipada, soporte para opciones locales y remotas
y compatibilidad con formularios del proyecto. El entregable de esta FE es la
implementacion del componente, no la creación de nuevos documentos de arquitectura.

## Contexto existente

- Arquitectura de referencia: `docs/Architecture/AppInputSelect/AppInputSelect-Architecture.md`
- Referencia de filosofia shared: `src/app/Components/UI/AppButton/`, `AppInput/`
- Base visual y de interacción: `Select` de Ant Design

## Restricciones (obligatorio)

- No usar `any`
- Mantener `Select` de Ant Design como base principal
- No embutir logica de negocio del dominio
- La integracion backend debe entrar por props, callbacks o adaptadores
- Tipado estricto para `value`, `option`, `onChange`

## Ubicacion (obligatoria)

```txt
src/app/Components/UI/AppInputSelect/
```

## Contratos (obligatorios)

```ts
export type AppInputSelectSize = "sm" | "md" | "lg";

export type AppInputSelectOption<TValue extends string | number = string> = {
  label: ReactNode;
  value: TValue;
  disabled?: boolean;
  meta?: Record<string, unknown>;
};

export type AppInputSelectProps<TValue extends string | number = string> = {
  value?: TValue | TValue[];
  defaultValue?: TValue | TValue[];
  options?: AppInputSelectOption<TValue>[];
  placeholder?: string;
  size?: AppInputSelectSize;
  mode?: "single" | "multiple" | "tags";
  disabled?: boolean;
  loading?: boolean;
  allowClear?: boolean;
  searchable?: boolean;
  noDataText?: ReactNode;
  fetchOptions?: (query?: string) => Promise<{ options: AppInputSelectOption<TValue>[] }>;
  onChange?: (value: TValue | TValue[], option?: unknown) => void;
  onSearch?: (query: string) => void;
  className?: string;
  status?: "error" | "warning";
};
```

## Reglas de implementacion (obligatorio)

- Implementar `AppInputSelect.tsx` como wrapper real sobre `Select`.
- Permitir opciones estaticas via `options`.
- Permitir opciones remotas via `fetchOptions`.
- Exponer `notFoundContent` consistente para `noDataText`.
- Soportar `single`, `multiple` y `tags` sin romper la API shared.
- Mantener compatibilidad con formularios controlados y no controlados.
- Exportar el componente desde el índice shared correspondiente.

## Estructura sugerida

```tsx
<Select
  value={mappedValue}
  options={mappedOptions}
  loading={loading}
  mode={antdMode}
  notFoundContent={emptyNode}
  onSearch={handleSearch}
  onChange={handleChange}
/>
```

## Entregables de esta FE

- `AppInputSelect.tsx`
- `index.ts`
- tipos exportados del componente
- implementación funcional con opciones locales y remotas

## Pruebas obligatorias

- Renderiza placeholder y opciones locales
- Selecciona valor y dispara `onChange`
- Modo remoto invoca `fetchOptions`
- Renderiza `noDataText` cuando no hay datos
- Respeta `disabled` y `loading`

## Criterios de aceptacion

- Componente reusable creado en UI shared
- Carpeta del componente creada con implementación real y exports correspondientes
- API estable y tipada
- Compatible con Ant Design y formularios del proyecto
- Integracion backend desacoplada y reutilizable
