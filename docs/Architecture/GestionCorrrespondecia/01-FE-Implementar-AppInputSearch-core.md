# PROMPT ARQUITECTONICO Ticket 01 FE

# Implementar AppInputSearch core (UI + eventos + estilos)

## Rol esperado

Arquitecto de software senior frontend (React, componentes UI enterprise, accesibilidad, testing).

## Objetivo

Construir o evolucionar el componente reusable `AppInputSearch` basado en `AutoComplete` + `Input` de Ant Design, con semantica controlada de eventos, estados de loading, accesibilidad y variantes de tamano, alineado visual y funcionalmente con `AppInput`.

## Contexto existente

- Documento tecnico:
  - `docs/Architecture/GestionCorrrespondecia/WorkflowInbox-Busqueda-Autocomplete-Architecture.md`
- Documento de componente:
  - `docs/Components/AppInputSearch/README.md`
- Estilos base:
  - `src/app/Components/UI/AppInput`
- Implementacion objetivo:
  - `src/app/Components/UI/AppInputSearch/`

## Ubicacion obligatoria

```txt
src/app/Components/UI/AppInputSearch/
```

## Restricciones obligatorias

- no consumir APIs dentro del componente
- no acoplar a modulos o pantallas
- no usar `Input.Search` de Ant Design
- no bloquear input durante loading
- no introducir estilos globales
- no romper consistencia con `AppInput`
- no duplicar logica de conexion con `GestionCorrespondencia`

## Contrato obligatorio

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

## Reglas de implementacion obligatorias

### 1. Controlado vs no controlado

- si se provee `value`, el componente es controlado
- `defaultValue` solo aplica en modo no controlado
- nunca mezclar ambos comportamientos
- en modo controlado, el valor visible debe provenir de la prop `value`

### 2. Eventos

- `onChange` se dispara en cada cambio de input
- `onSearch` se dispara por:
  - Enter, inmediato
  - click en icono, inmediato
  - debounce por escritura

Regla critica:

- Enter y click en icono ignoran debounce pendiente
- se debe cancelar o neutralizar el debounce pendiente para evitar duplicacion de eventos

### 3. Validacion de busqueda

- `onSearch` solo se ejecuta si `length >= minLength` cuando `minLength` este definido
- texto vacio no debe disparar busqueda automaticamente por clear

### 4. Debounce

- `debounceMs = 0` o `undefined` significa sin debounce
- debounce solo aplica a escritura
- debounce no bloquea eventos manuales por Enter o click
- el componente debe limpiar timers al desmontar

### 5. Clear

- limpiar debe ejecutar:
  - `onChange("")`
  - `onClear()`
- limpiar no debe ejecutar `onSearch("")` automaticamente
- Escape limpia solo si `clearOnEscape = true`

### 6. Options / Autocomplete

- `options` alimenta `AutoComplete`
- no mutar `options`
- si `options` esta vacio, el input sigue funcionando normalmente
- mantener navegacion por teclado en sugerencias
- seleccionar una sugerencia debe propagar el valor de forma deterministica

### 7. Loading

- input permanece editable
- foco no se pierde
- mostrar indicador visual de loading
- no bloquear eventos manuales

### 8. Estilos

- alineado a `AppInput`:
  - border radius `12px`
  - estados: focus, hover, error, disabled
- variantes size:
  - `sm`
  - `md`
  - `lg`
- las variantes afectan:
  - altura
  - padding
  - tamano de icono

### 9. Accesibilidad

- soportar `aria-label` o `aria-labelledby`
- boton de clear con `aria-label="Limpiar"`
- autocomplete con navegacion por teclado
- no degradar roles accesibles de `AutoComplete`

## Reglas de consistencia

- no mezclar logica de busqueda con consumo de API
- no introducir side effects externos
- no romper patron reusable UI
- no divergir visualmente de `AppInput`
- no duplicar logica de debounce fuera del componente

## Riesgos a evitar

- doble ejecucion de `onSearch`
- ruptura de modo controlado/no controlado
- perdida de accesibilidad del autocomplete
- bloqueo del input en loading
- estilos inconsistentes con `AppInput`
- mutacion de `options`
- comportamiento distinto entre Enter, click y debounce

## Pruebas unitarias obligatorias

- `onChange` se ejecuta en cada input
- `onSearch` se ejecuta por Enter
- `onSearch` se ejecuta por debounce
- `onSearch` se ejecuta por click en icono
- Enter ignora debounce pendiente
- `minLength` bloquea busquedas cortas
- `debounceMs = 0` desactiva debounce
- clear dispara `onChange("")`
- clear dispara `onClear()`
- clear no dispara `onSearch("")`
- Escape limpia solo si `clearOnEscape = true`
- modo controlado respeta `value` externo
- modo no controlado usa `defaultValue`
- loading no bloquea input
- loading no pierde foco
- options renderiza sugerencias
- options vacio no rompe el input
- size aplica clases correctas
- accesibilidad exige label accesible
- boton clear es accesible

## Pruebas QT / calidad

- usuario escribe texto y debounce dispara busqueda correctamente
- usuario presiona Enter y obtiene busqueda inmediata sin duplicados
- usuario hace click en icono y obtiene busqueda inmediata
- usuario escribe texto menor a `minLength` y no se ejecuta busqueda
- usuario limpia con boton, el input queda vacio y no se dispara busqueda automaticamente
- usuario presiona Escape y limpia solo si esta habilitado
- usuario navega sugerencias con teclado y puede seleccionar opcion
- loading activo mantiene input editable
- integracion visual consistente con `AppInput`

## Criterios de aceptacion

- componente reusable implementado correctamente
- semantica de eventos deterministica
- sin duplicacion de eventos
- accesibilidad funcional correcta
- estilos consistentes con `AppInput`
- comportamiento controlado/no controlado correcto
- cobertura de pruebas completa unitarias y QT

