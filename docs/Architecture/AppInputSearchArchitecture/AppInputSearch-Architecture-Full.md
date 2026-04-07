# Arquitectura y Requerimientos: AppInputSearch (Control Input)

## Objetivo

Definir una arquitectura reusable para el control `AppInputSearch` que permita estandarizar la entrada de busqueda en el frontend, soportando escenarios locales y server-side, sin acoplarse a un modulo especifico.

Este documento es fuente unica de verdad para:

- prompts de IA
- tickets Jira
- implementacion frontend
- pruebas de regresion

## Alcance

Aplica a:

- `AppInputSearch` como control reusable
- `AppTableQueryWrapper` y otros contenedores de consulta
- pantallas con busqueda simple local o server-side
- formularios con busqueda embebida (toolbar, header, filtros rapidos)

No aplica a:

- redisenio visual general del sistema
- definicion de endpoints backend
- cambios de negocio por modulo
- reemplazo de librerias UI base

## Estado actual

### Frontend

- No existe un control unificado para busqueda.
- Cada modulo implementa su propio input, debounce y reglas de reset.
- No hay contrato unico para eventos `search`, `clear`, `submit` y `change`.

### Backend

- Algunos endpoints aceptan `Search` y `SearchType`.
- No existe un contrato global que vincule el control de input con el estado de consulta.

## Problema a resolver

Se necesita un control de entrada de busqueda reusable que:

- normalice UX y semantica de eventos
- reduzca duplicacion de logica (debounce, clear, submit)
- funcione tanto en busqueda local como server-side
- permita integrarse al `AppTableQueryState`

## Principios de arquitectura

### 1. Control reusable, no acoplado

`AppInputSearch` debe ser un control UI generico y reusable.

No debe:

- conocer endpoints
- conocer `AppTable` ni `AppTableQueryWrapper`
- asumir que la busqueda es siempre server-side

### 2. Contrato de eventos explicito

El control debe exponer eventos claros y consistentes:

- `onChange` para cambios inmediatos de texto
- `onSearch` para disparo de busqueda (debounced o submit)
- `onClear` para reset

### 3. Debounce controlado por props

El debounce no debe estar hardcodeado. Debe ser configurable y opcional.

### 4. Estado visual independiente

El control puede mostrar `loading` propio, sin bloquear el resto del UI.

## Arquitectura objetivo

```txt
Frontend
  -> AppInputSearch
     -> props de control y UX
     -> eventos onChange / onSearch / onClear
  -> Contenedor (AppTableQueryWrapper u otro)
     -> mantiene query state
     -> decide cuando disparar busqueda server-side
```

## Diseno frontend

### Base UI (Ant Design)

Para cumplir autocomplete + minLength + debounce + click en icono + loading, se usara Ant Design asi:

- `AutoComplete` como contenedor de sugerencias
- `Input` (no `Input.Search`) como campo base
- `Spin` o `LoadingOutlined` para indicador de carga

Regla:

- AntD provee el render y dropdown
- `AppInputSearch` controla toda la semantica de eventos

### Criterios CSS y variantes de tamano

El control debe alinear su estilo con `AppInput` para consistencia visual.

Referencias de estilo (existentes en `AppInput`):

- `border-radius: 12px`
- `box-shadow: 0 8px 22px rgba(15, 52, 96, 0.08)`
- `min-height: 2.75rem` (base)
- `padding-inline: 0.95rem`
- hover: `border-color: #6f9ff0`
- focus: `border-color: #0f5bd8` + `box-shadow: 0 0 0 3px rgba(15, 91, 216, 0.16)`
- error: `border-color: #c63d3d` + `background: #fff8f8`
- disabled: `background: #f4f6fb` + `color: #6d7b8b` + sin sombra

Border radius moderno:

- usar `12px` como base (match `AppInput`)
- opcional: permitir override por clase si se requiere en temas futuros

Variantes de tamano:

- `sm`
  - `min-height: 2.25rem`
  - `padding-inline: 0.75rem`
  - icono mas compacto

- `md` (default)
  - `min-height: 2.75rem`
  - `padding-inline: 0.95rem`

- `lg`
  - `min-height: 3.25rem`
  - `padding-inline: 1.1rem`
  - icono ligeramente mayor

Regla:

- las variantes deben impactar solo alto/padding/icono, no la semantica ni el contrato

Clases CSS sugeridas (nombres de referencia):

- `.field`
- `.label`
- `.input`
- `.inputSm`
- `.inputMd`
- `.inputLg`
- `.inputError`
- `.inputDisabled`
- `.helperText`
- `.helperTextError`
- `.icon`
- `.iconLoading`

Snippet CSS de referencia (alineado a AppInput):

```css
.input {
  border-radius: 12px;
  border-width: 1px;
  box-shadow: 0 8px 22px rgba(15, 52, 96, 0.08);
  min-height: 2.75rem;
  padding-inline: 0.95rem;
  transition:
    border-color 120ms ease,
    box-shadow 120ms ease,
    background-color 120ms ease;
}

.input:hover {
  border-color: #6f9ff0 !important;
}

.input:focus,
.input:focus-within {
  border-color: #0f5bd8 !important;
  box-shadow: 0 0 0 3px rgba(15, 91, 216, 0.16) !important;
  outline: none;
}

.inputError {
  border-color: #c63d3d !important;
  background: #fff8f8 !important;
}

.inputError:focus,
.inputError:focus-within {
  box-shadow: 0 0 0 3px rgba(198, 61, 61, 0.16) !important;
}

.inputDisabled {
  background: #f4f6fb !important;
  box-shadow: none;
  color: #6d7b8b !important;
}

.inputSm {
  min-height: 2.25rem;
  padding-inline: 0.75rem;
}

.inputMd {
  min-height: 2.75rem;
  padding-inline: 0.95rem;
}

.inputLg {
  min-height: 3.25rem;
  padding-inline: 1.1rem;
}
```

### Responsabilidades

`AppInputSearch` debe:

- renderizar input con placeholder
- ofrecer boton clear cuando hay texto
- soportar submit por Enter
- soportar debounce opcional
- exponer `value` controlado o `defaultValue`
- permitir icono de busqueda y loading
- ser accesible (aria-label)
- soportar autocomplete con sugerencias controladas

No debe:

- disparar fetch directo
- conocer filtros avanzados
- mutar query state por su cuenta

### Props sugeridas

```ts
type AppInputSearchProps = {
  value?: string;
  defaultValue?: string;
  placeholder?: string;
  disabled?: boolean;
  autoFocus?: boolean;
  debounceMs?: number; // 0 o undefined = sin debounce
  minLength?: number;  // minimo para disparar onSearch
  loading?: boolean;
  clearOnEscape?: boolean;
  options?: { value: string; label?: string }[];
  onChange?: (value: string) => void;
  onSearch?: (value: string) => void;
  onClear?: () => void;
  onFocus?: () => void;
  onBlur?: () => void;
};
```

### Semantica de eventos

- `onChange` se dispara en cada cambio de texto.
- `onSearch` se dispara:
  - al presionar Enter
  - al hacer click en el icono de busqueda
  - al completar debounce si `debounceMs` > 0
  - solo si `value.length >= minLength` (si se define)
- `onClear` se dispara cuando el usuario limpia el campo.

### UX y reglas

- El boton clear solo aparece cuando hay texto.
- El icono de busqueda debe ser interactivo y disparar `onSearch` si existe.
- Si `loading = true`, mostrar estado visual sin bloquear el input.
- `Escape` debe limpiar el input si `clearOnEscape = true`.
- `options` alimenta las sugerencias del `AutoComplete`.

### UX de loading en autocomplete

Reglas:

- `loading` no debe bloquear el input.
- El dropdown de `AutoComplete` puede mostrar un estado de carga liviano.
- Si no hay resultados y `loading = true`, mostrar un indicador de carga en `notFoundContent`.

Opciones validas:

- icono con `Spin` o `LoadingOutlined` en el suffix del input
- `notFoundContent` con spinner y texto corto (ej. "Buscando...")

Opciones no validas:

- overlay que bloquee el input
- deshabilitar el input solo por estar cargando sugerencias

### Flujos de ejemplo

```txt
Flujo A: debounce (minLength=3)
  usuario escribe -> onChange
  si length >= 3 -> debounce -> onSearch

Flujo B: Enter
  usuario presiona Enter -> onSearch
  si length < 3 -> no dispara

Flujo C: click en icono
  usuario hace click -> onSearch
  si length < 3 -> no dispara
```

### Diagrama de estados

```txt
┌────────┐    type/change     ┌──────────┐
│ Idle   │ ────────────────▶ │ Typing   │
└────────┘                    └────┬─────┘
                                  │ length < minLength
                                  │
                                  ▼
                             ┌──────────┐
                             │ Waiting  │  (sin busqueda)
                             └────┬─────┘
                                  │ length >= minLength
                                  ▼
                             ┌──────────┐   debounce/Enter/click
                             │ Ready    │ ─────────────────────▶ onSearch
                             └────┬─────┘
                                  │
                                  ▼
                             ┌──────────┐
                             │ Loading  │
                             └────┬─────┘
                                  │ results/finish
                                  ▼
                             ┌──────────┐
                             │ Results  │
                             └──────────┘

        clear/Escape
Results/Typing/Waiting/Ready ─────────────▶ Cleared
                                   │
                                   ▼
                                 Idle
```

### Accesibilidad

- El input debe tener `aria-label` o `aria-labelledby`.
- El boton clear debe tener `aria-label="Limpiar"`.
- El control debe ser navegable por teclado.

## Integracion con query state

Cuando se use con `AppTableQueryState`:

- el contenedor es responsable de actualizar `search`.
- al cambiar `search`, el contenedor debe resetear `page = 1`.
- `AppInputSearch` no conoce `page` ni `pageSize`.

## Consumo de API para autocomplete (contexto del repo)

Regla:

- `AppInputSearch` NO consume API directamente.
- El contenedor o hook del modulo es responsable de consultar y pasar `options` + `loading`.

Patron recomendado (existente en el repo):

- hooks de consulta usan `clienteApi` + `react-query`
- normalizan items a `{ value, label }`
- controlan errores y loading fuera del input

Contrato de integracion sugerido:

```txt
Modulo (hook)
  -> consume API (axios + react-query)
  -> normaliza opciones
  -> entrega options + loading

AppInputSearch
  -> renderiza AutoComplete
  -> dispara onSearch (debounce/minLength)
```

Notas:

- el minLength se evalua en el control para evitar llamadas innecesarias
- el loading se refleja en icono y/o dropdown sin bloquear input
- el control no decide endpoint ni payload

Ejemplo de contrato de hook (sin implementacion):

```ts
type AppInputSearchOption = {
  value: string;
  label?: string;
};

type UseAppInputSearchApiResult = {
  options: AppInputSearchOption[];
  loading: boolean;
  error?: Error;
  search: (text: string) => void;
  clear: () => void;
};
```

## Contrato generico de autocomplete

Objetivo:

- unificar consumo de APIs dispares
- mantener `AppInputSearch` desacoplado de payloads especificos
- centralizar mapeos por endpoint en el hook/servicio

### Request generico

```ts
type AppAutocompleteRequest = {
  query: string;
  fieldName?: string;      // name_campo
  fieldControl?: string;   // tbl_control
  context?: Record<string, unknown>; // metadata extra (idScript, restriccion, etc.)
  searchFields?: string[]; // columnas a considerar en la busqueda
};
```

Ejemplo de `context`:

```ts
const request: AppAutocompleteRequest = {
  query: "cami",
  fieldName: "DESTINATARIO_COR",
  fieldControl: "TERCERO",
  searchFields: ["nombre", "apellido", "documento"],
  context: {
    idScript: 123,
    restriccion: {
      IdRestriTipoDestInterno: 1,
      IdTipoRestriccion: 2,
      DescripcionTipo: "Interno",
      MoluloRadicacion: 1,
      ModuloRadicacionSimple: 0,
      ModuloRadicacionInterna: 1,
    },
  },
};
```

### Response generica

```ts
type AppAutocompleteOption = {
  value: string;
  label: string;
  meta?: Record<string, unknown>;
};
```

Ejemplo de `meta` en response:

```ts
const option: AppAutocompleteOption = {
  value: "987",
  label: "Camila Urueta",
  meta: {
    docId: "CC-123456",
    tipo: "TERCERO",
    raw: { idValue: "987", texValue: "Camila Urueta" },
  },
};
```

### Adaptadores por endpoint (ejemplos)

- `solicitaAutoCompleteCampos`
  - `query -> TextoBuscado`
  - `fieldControl -> tbl_control`
  - `fieldName -> name_campo`

- `autoCompleteTercero`
  - `context.idScript -> idScript`
  - `fieldName -> nombreCampo`
  - `query -> valueCampo`

- `solicitaAutoCompleteDestinatarioRestriccion`
  - `query -> ValueAuto`
  - `context.restriccion -> CDeRelacionEstadoRetriccionDto`

Regla:

- el mapper vive en el hook/servicio, no en `AppInputSearch`

Ejemplo de mapeo real (pseudo):

```ts
function mapRequestToEndpoint(
  endpoint: string,
  req: AppAutocompleteRequest,
) {
  if (endpoint === "/api/PlantillaRadicado/solicitaAutoCompleteCampos") {
    return {
      TextoBuscado: req.query,
      defaultDbAlias: "",
      tbl_control: req.fieldControl ?? "",
      name_campo: req.fieldName ?? "",
      searchFields: req.searchFields ?? [],
    };
  }
  if (endpoint === "/api/PlantillaRadicado/autoCompleteTercero") {
    return {
      idScript: Number(req.context?.idScript ?? 0),
      nombreCampo: String(req.fieldName ?? ""),
      valueCampo: req.query,
    };
  }
  if (endpoint === "/api/PlantillaRadicado/solicitaAutoCompleteDestinatarioRestriccion") {
    return {
      ValueAuto: req.query,
      CDeRelacionEstadoRetriccionDto: req.context?.restriccion ?? {},
    };
  }
  return { query: req.query };
}
```

Ejemplo de normalizacion de response (pseudo):

```ts
function mapResponseToOptions(payload: unknown): AppAutocompleteOption[] {
  const list =
    Array.isArray(payload) ? payload :
    Array.isArray((payload as { data?: unknown }).data) ? (payload as { data: unknown[] }).data :
    Array.isArray((payload as { Data?: unknown }).Data) ? (payload as { Data: unknown[] }).Data :
    [];

  return list
    .map((item) => {
      const anyItem = item as Record<string, unknown>;
      const value =
        anyItem.idValue ??
        anyItem.id_value ??
        anyItem.id ??
        anyItem.Id ??
        anyItem.idTercero ??
        "";
      const label =
        anyItem.texValue ??
        anyItem.valueCampo ??
        anyItem.value_campo ??
        anyItem.Value ??
        anyItem.nombre ??
        anyItem.descripcion ??
        anyItem.label ??
        anyItem.text ??
        "";
      return {
        value: String(value ?? ""),
        label: String(label ?? "").trim(),
        meta: { raw: anyItem },
      };
    })
    .filter((opt) => opt.label.length > 0);
}
```

## Manejo de errores en autocomplete

Reglas:

- los errores se manejan en el hook/servicio, no en el control
- `AppInputSearch` solo refleja un estado visual no bloqueante
- el input debe seguir siendo usable aunque falle la API

Opciones de UX validas:

- mostrar `notFoundContent` con mensaje corto ("Sin resultados" o "Error al consultar")
- mantener el dropdown abierto si el usuario sigue escribiendo
- limpiar opciones anteriores solo cuando se confirma un error

Opciones no validas:

- bloquear el input por error
- mostrar modal o toast desde el control base

## Decisiones explicitas

### Decision 1

`AppInputSearch` es un control UI, no un gestor de consultas.

### Decision 2

El debounce es opcional y configurable por props.

### Decision 3

El control puede ser controlado (`value`) o no controlado (`defaultValue`).

### Decision 4

`onSearch` es el evento canonico para disparar consulta.

### Decision 5

Se usara `AutoComplete` + `Input` de Ant Design, manteniendo la logica de debounce, minLength y click en icono dentro de `AppInputSearch`.

## Plan de implementacion

### Fase 1: Contrato y API

- definir `AppInputSearchProps`
- definir eventos y semantica

### Fase 2: Componente base

- render input con icono y clear
- soportar keyboard (Enter, Escape)

### Fase 3: Debounce y minLength

- integrar debounce opcional
- respetar `minLength`

### Fase 4: Accesibilidad y estados

- aria labels
- estados disabled y loading

## Estrategia de pruebas

### Unitarias

- `onChange` se dispara en cada input
- `onSearch` se dispara en Enter
- `onSearch` se dispara con debounce
- `minLength` bloquea busqueda corta
- `onClear` se dispara al limpiar

### Integracion

- con `AppTableQueryWrapper` resetea page a 1
- respeta `debounceMs` en server-side
- no bloquea input cuando `loading = true`

## Riesgos

- duplicar logica de debounce en cada modulo
- mezclar responsabilidades entre control y contenedor
- disparar busqueda con valores cortos sin `minLength`

## Recomendacion final

Implementar `AppInputSearch` como control reusable, liviano y desacoplado, con contrato claro de eventos y soporte de debounce opcional. La logica de consulta debe vivir en el contenedor, no en el control.
