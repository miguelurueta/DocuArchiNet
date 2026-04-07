# Ticket 01 FE

## Titulo

Implementar `AppInputSearch` core (UI + eventos + estilos)

## Objetivo

Construir el control reusable con AutoComplete + Input de AntD, semantica de eventos (debounce, minLength, click icono), estados de loading, accesibilidad y variantes `sm|md|lg`, alineado a `AppInput`.

## Contexto existente

- especificacion completa en `docs/Architecture/AppInputSearch/AppInputSearch-Architecture.md`
- estilos base en `src/app/Components/UI/AppInput`

## Restricciones (obligatorio)

- no consumir APIs dentro del control
- no acoplar a modulo o pantalla
- no usar `Input.Search`
- no bloquear input durante loading

## Ubicacion (obligatoria)

```txt
src/app/Components/UI/AppInputSearch/
```

## Contratos (obligatorios)

```ts
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
  options?: { value: string; label?: string }[];
  onChange?: (value: string) => void;
  onSearch?: (value: string) => void;
  onClear?: () => void;
  onFocus?: () => void;
  onBlur?: () => void;
  size?: "sm" | "md" | "lg";
};
```

## Reglas de implementacion (obligatorio)

- onSearch se dispara por Enter, debounce y click en icono
- onSearch solo si length >= minLength (si se define)
- debounceMs = 0 o undefined desactiva debounce
- clearOnEscape limpia y dispara onClear
- options alimenta AutoComplete
- estilos alineados a AppInput (radius 12px, focus/hover/error/disabled)
- variantes size solo afectan alto/padding/icono
- aria-label o aria-labelledby, clear con aria-label "Limpiar"

## Pruebas obligatorias

- onChange en cada input
- onSearch por Enter/debounce/click
- minLength bloquea busqueda corta
- onClear en clear/Escape
- variantes size aplican clases

## Criterios de aceptacion

- componente reusable creado y documentado
- semantica de eventos cumple especificacion
- estilos consistentes con AppInput
- accesibilidad basica cubierta
