# PROMPT ARQUITECTONICO Ticket 01 FE

# Implementar AppInputSearch core (UI + eventos + estilos)

## Rol esperado

Arquitecto de software senior frontend (React 19 + TypeScript estricto + componentes UI enterprise + accesibilidad + testing).

## Objetivo

Construir o evolucionar el componente reusable `AppInputSearch` basado en:

- `AutoComplete` + `Input` de Ant Design

Con:

- semantica controlada de eventos
- debounce configurable
- soporte de loading sin bloqueo
- accesibilidad completa
- variantes de tamano `sm | md | lg`
- consistencia visual con `AppInput`

## Contexto existente

- Documento tecnico:
  - `docs/Architecture/GestionCorrrespondecia/WorkflowInbox-Busqueda-Autocomplete-Architecture.md`
- Documento del componente:
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
- no duplicar logica de conexion con modulos
- no mantener fuentes de verdad duplicadas entre props y estado interno

## Contrato obligatorio

```ts
type AppInputSearchOption = {
  value: string;
  label?: string;
};

type AppInputSearchState = "default" | "error";

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

  className?: string;
  error?: boolean;
  state?: AppInputSearchState;
  helperText?: React.ReactNode;
  label?: React.ReactNode;

  "aria-label"?: string;
  "aria-labelledby"?: string;
};
```

Nota de compatibilidad:

- este contrato cambia `onChange` desde `ChangeEventHandler<HTMLInputElement>` hacia `(value: string) => void`
- los consumidores existentes deben migrar de `event.target.value` a `value`
- `className`, `error`, `state`, `helperText` y `label` se conservan para mantener composicion y consistencia con `AppInput`
- el icono de busqueda pasa a ser parte del comportamiento core; si existe `showIcon` en la implementacion actual, debe eliminarse o marcarse como deprecated de forma explicita antes de removerlo

## Valores por defecto obligatorios

```txt
size = "md"
debounceMs = 0 (sin debounce)
minLength = undefined (sin restriccion)
state = "default"
```

## Reglas de implementacion obligatorias

### 1. Controlado vs no controlado

- si se provee `value`, el componente es controlado
- `defaultValue` solo aplica en modo no controlado
- no mezclar ambos comportamientos
- en modo controlado:
  - el valor visible siempre proviene de `value`
  - no mantener estado interno paralelo
- en modo no controlado:
  - el estado interno es la unica fuente de verdad visual
  - `defaultValue` solo se usa para inicializacion

### 2. Eventos

- `onChange` se dispara en cada cambio de input

`onSearch` se dispara por:

- Enter, inmediato
- click en icono, inmediato
- debounce por escritura

Regla critica:

```txt
Enter y click en icono:
- ignoran debounce pendiente
- cancelan debounce pendiente
- evitan duplicacion de eventos
```

### 3. Seleccion de opciones obligatoria

Al seleccionar una opcion:

```txt
- ejecutar onChange(selectedValue)
- ejecutar onSearch(selectedValue) inmediatamente
- no usar debounce
- cancelar debounce pendiente
- comportamiento deterministico
```

### 4. Validacion de busqueda

- `onSearch` solo se ejecuta si `length >= minLength` cuando `minLength` este definido
- si `minLength` es `undefined`, no restringe busqueda
- texto vacio no dispara busqueda automatica en clear

### 5. Debounce

- `debounceMs = 0` o `undefined` significa sin debounce
- debounce solo aplica a escritura
- debounce no afecta Enter ni click
- debounce no afecta seleccion de opciones
- limpiar timers al desmontar

### 6. Clear

La accion de limpiar debe ejecutar:

```txt
onChange("")
onClear()
```

Reglas:

- no ejecutar `onSearch("")`
- boton clear visible solo si:
  - hay valor visible
  - no esta `disabled`
- Escape limpia solo si `clearOnEscape = true`
- Escape debe respetar `disabled`

### 7. Iconos obligatorios

Debe existir:

- icono de busqueda siempre visible
- boton de clear condicional
- indicador visual de loading cuando `loading = true`

Reglas:

```txt
- icono de busqueda dispara onSearch inmediato
- boton clear sigue reglas de seccion Clear
- loading no reemplaza la interaccion manual de busqueda
```

### 8. Options / Autocomplete

- `options` alimenta `AutoComplete`
- no mutar `options`
- si `options` esta vacio, el input sigue funcionando
- navegacion por teclado obligatoria
- seleccion de sugerencias debe ser deterministica
- no ejecutar llamadas HTTP ni resolver endpoints desde el componente

### 9. Loading

- input permanece editable
- foco no se pierde
- mostrar indicador visual
- no bloquear eventos manuales

Prioridad:

```txt
disabled tiene prioridad sobre loading
```

- si `disabled = true`, no hay interaccion

### 10. Accesibilidad obligatoria

- debe soportar:

```txt
aria-label o aria-labelledby
```

- uno de los dos debe estar presente cuando no exista `label` visible
- boton clear:

```txt
aria-label="Limpiar"
```

- icono de busqueda interactivo debe tener nombre accesible
- mantener roles accesibles de `AutoComplete`
- navegacion por teclado obligatoria
- estados `error` / `state = "error"` deben reflejar `aria-invalid`
- `helperText` debe asociarse con `aria-describedby` cuando aplique

### 11. Estilos obligatorios

Debe alinearse con `AppInput`:

- border radius: `12px`
- estados:
  - focus
  - hover
  - error
  - disabled

Variantes de tamano:

- `sm`
- `md`
- `lg`

Afectan:

- altura
- padding
- tamano de iconos

Reglas:

- usar CSS module local
- no introducir estilos globales
- `className` solo extiende el contenedor/control sin romper clases internas

## Reglas de consistencia

- no mezclar logica de busqueda con API
- no introducir side effects externos
- no duplicar logica de debounce fuera del componente
- mantener patron reusable UI
- mantener consistencia visual con `AppInput`
- mantener compatibilidad con `AppTableQueryWrapper` mediante migracion explicita de `onChange`

## Inmutabilidad obligatoria

- no mutar `options`
- no mutar props
- no mantener fuentes de verdad duplicadas

## Riesgos a evitar

- doble ejecucion de `onSearch`
- ruptura de controlado/no controlado
- perdida de accesibilidad
- bloqueo del input en loading
- estilos inconsistentes
- mutacion de `options`
- comportamiento distinto entre Enter, click, seleccion de opcion y debounce
- romper consumidores existentes por el cambio de firma de `onChange`

## Pruebas unitarias obligatorias

- `onChange` en cada input
- `onSearch` por Enter
- `onSearch` por debounce
- `onSearch` por click en icono
- Enter ignora y cancela debounce pendiente
- click en icono ignora y cancela debounce pendiente
- seleccion de opcion dispara `onChange(selectedValue)`
- seleccion de opcion dispara `onSearch(selectedValue)`
- seleccion de opcion no usa debounce
- `minLength` bloquea correctamente
- `debounceMs = 0` desactiva debounce
- clear ejecuta `onChange("")`
- clear ejecuta `onClear()`
- clear no ejecuta `onSearch("")`
- boton clear aparece solo con valor visible y no disabled
- Escape respeta `clearOnEscape`
- Escape no limpia cuando `disabled = true`
- modo controlado respeta `value`
- modo controlado no mantiene fuente interna paralela
- modo no controlado usa `defaultValue`
- loading no bloquea input
- loading no pierde foco
- `disabled` tiene prioridad sobre loading
- options vacio no rompe
- options renderiza sugerencias
- navegacion y seleccion de opciones funcionan
- size aplica clases correctas
- `error` / `state = "error"` aplica estado visual y `aria-invalid`
- `helperText` queda asociado por `aria-describedby`
- accesibilidad valida con `label`, `aria-label` o `aria-labelledby`
- boton clear es accesible
- consumidores migrados no usan `event.target.value`

## Pruebas QT / calidad

- escritura con debounce funcional
- Enter dispara busqueda sin duplicados
- click icono dispara busqueda inmediata
- seleccion de sugerencia dispara busqueda inmediata
- `minLength` bloquea busquedas cortas
- clear limpia sin disparar busqueda
- Escape limpia correctamente
- navegacion de sugerencias con teclado
- loading mantiene input editable
- disabled bloquea interaccion aunque loading este activo
- consistencia visual con `AppInput`
- `AppTableQueryWrapper` sigue filtrando con la nueva firma `onChange(value)`

## Criterios de aceptacion

- componente reusable implementado
- semantica de eventos deterministica
- sin duplicacion de eventos
- accesibilidad completa
- estilos consistentes con `AppInput`
- comportamiento controlado/no controlado correcto
- consumidores existentes migrados a `onChange(value)`
- cobertura completa de pruebas unitarias y QT

## Instruccion final

Antes de implementar:

- validar `AppInput`
- validar estilos base
- validar contrato de props
- validar consumidores actuales de `AppInputSearch`

Luego:

- implementar con TypeScript estricto
- mantener componente puro
- respetar separacion de responsabilidades
- migrar consumidores existentes de `onChange(event)` a `onChange(value)`
- actualizar `docs/Components/AppInputSearch/README.md`

Finalmente reportar:

- decisiones de diseno
- manejo de debounce
- control de eventos
- manejo de accesibilidad
- consistencia visual lograda
- consumidores actualizados

