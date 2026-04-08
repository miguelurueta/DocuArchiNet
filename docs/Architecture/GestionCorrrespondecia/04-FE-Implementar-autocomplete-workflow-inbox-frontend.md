# PROMPT ARQUITECTONICO Ticket 04 FE

# Implementar autocomplete frontend para Workflow Inbox

## Rol esperado

Arquitecto de software senior frontend (React, hooks, servicios HTTP, accesibilidad, performance).

## Objetivo

Implementar la capa frontend de autocomplete para tareas workflow, manteniendo `AppInputSearch` como componente presentacional y ubicando la conexion a backend en un hook + servicio desacoplados.

La solucion debe permitir:

- consultar sugerencias desde backend
- mostrar sugerencias en `AppInputSearch`
- mantener busqueda libre manual por Enter/click
- actualizar la tabla al seleccionar una sugerencia
- no romper paginacion, busqueda `LIKE` ni flujo actual de consulta

## Contexto existente

- Documento tecnico:
  - `docs/Architecture/GestionCorrrespondecia/WorkflowInbox-Busqueda-Autocomplete-Architecture.md`
- Componente UI:
  - `src/app/Components/UI/AppInputSearch/`
- Pantalla objetivo:
  - `src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx`
- Hook de tabla existente:
  - `src/modules/gestionCorrespondencia/hooks/useGestionCorrespondenciaTable.ts`

## Ubicacion esperada

```txt
src/modules/gestionCorrespondencia/hooks/useWorkflowInboxAutocomplete.ts
src/modules/gestionCorrespondencia/services/workflowInboxAutocomplete.service.ts
src/modules/gestionCorrespondencia/types/*
src/modules/gestionCorrespondencia/tests/*
```

## Dependencia backend

Este ticket depende de que exista un endpoint backend de autocomplete o un contrato aprobado equivalente.

### Contrato sugerido

```txt
POST /api/workflowInboxgestion/inboxgestion/autocomplete
```

### Request sugerido

```ts
type WorkflowInboxAutocompleteRequest = {
  search: string;
  limit?: number;
};
```

### Response sugerido

```ts
type WorkflowInboxAutocompleteResponse = {
  items: Array<{
    value: string;
    label?: string;
    field?: string;
  }>;
};
```

### Regla de contrato

Si el contrato backend definitivo difiere del sugerido:

- adaptar el servicio
- mantener estable el contrato interno del hook
- no trasladar esa diferencia al componente `AppInputSearch`

## Dependencias frontend

- Depende de `01-FE-Implementar-AppInputSearch-core.md` para que `AppInputSearch` soporte:
  - `options`
  - `loading`
  - `onSearch(value)`
  - seleccion de opciones
  - `onChange(value)`
- Se integra despues o junto con `02-FE-Integrar-AppInputSearch-en-AppToolbar-GestionCorrespondencia.md`.
- Se apoya en `03-FE-Conectar-busqueda-like-GestionCorrespondencia.md` para que la busqueda real de tabla use `SearchType = 2`.

## Restricciones obligatorias

- no consumir APIs dentro de `AppInputSearch`
- no incluir endpoints conocidos dentro de `AppInputSearch`
- no retornar filas completas como sugerencias
- no deshabilitar input durante loading
- no ejecutar autocomplete si no se cumple `minLength`
- no dejar `limit` sin control
- no romper busqueda de tabla por texto libre
- no mezclar autocomplete con listado paginado
- no acoplar el hook a un componente especifico
- no usar `any`
- no llamar backend de autocomplete desde la pagina

## Separacion de responsabilidades obligatoria

### Flujo 1: autocomplete

- escritura del usuario
- evaluacion de `minLength`
- debounce
- request de sugerencias
- `options` para `AppInputSearch`

### Flujo 2: busqueda real de tabla

- Enter en el buscador
- click en icono de busqueda
- seleccion de sugerencia

Estos eventos deben terminar en:

```txt
table.onQueryChange({ search })
```

Reglas clave:

- el autocomplete no reemplaza la busqueda real
- la busqueda libre debe seguir funcionando aunque no existan sugerencias
- la pagina no conoce endpoint ni DTO backend de autocomplete
- `table.onQueryChange` sigue siendo el unico puente hacia la consulta de tabla

## Compatibilidad con 01-FE y 02-FE

`01-FE` permite `debounceMs` en `AppInputSearch` para `onSearch` por escritura. Este ticket define una regla mas especifica para el caso autocomplete.

Cuando `AppInputSearch` se integre con `useWorkflowInboxAutocomplete`:

- el debounce de sugerencias vive solo en `useWorkflowInboxAutocomplete`
- `AppInputSearch` debe usarse sin debounce propio para autocomplete, por ejemplo `debounceMs={0}`
- `onChange(value)` alimenta el texto del hook de autocomplete y el valor visual/controlado que corresponda
- `onSearch(value)` ejecuta busqueda real de tabla mediante `table.onQueryChange({ search: value })`
- seleccion de sugerencia ejecuta busqueda real mediante `table.onQueryChange({ search: selectedValue })`
- si se limpia el texto, se ejecuta `autocomplete.clear()` y se mantiene el flujo de limpieza de tabla definido por el producto

Nota:

- si `02-FE` ya conecto `onChange(value)` directamente a `table.onQueryChange({ search: value })`, este ticket puede evolucionar esa integracion para separar escritura/autocomplete de busqueda real
- esa evolucion no debe mover logica de request ni endpoints a la pagina

## Estrategia de debounce obligatoria

Debe existir una sola fuente de debounce para autocomplete.

### Regla obligatoria

El debounce de autocomplete debe vivir en:

```txt
useWorkflowInboxAutocomplete
```

No debe duplicarse en:

- el hook de tabla del modulo
- la pantalla
- `AppInputSearch`

### Consecuencia

- `AppInputSearch` sigue siendo presentacional
- el hook decide cuando consultar sugerencias
- Enter y click siguen permitiendo busqueda manual inmediata sin depender del autocomplete

## Contrato del hook obligatorio

Definir explicitamente:

```ts
type WorkflowInboxAutocompleteItem = {
  value: string;
  label?: string;
};

type UseWorkflowInboxAutocompleteParams = {
  minLength: number;
  limit: number;
};

type UseWorkflowInboxAutocompleteResult = {
  items: WorkflowInboxAutocompleteItem[];
  loading: boolean;
  error: Error | null;
  setSearchText: (value: string) => void;
  clear: () => void;
};
```

### Reglas

- `minLength` es obligatorio en el hook
- si el texto no cumple `minLength`:
  - no consulta backend
  - `items = []`
- `limit` debe estar controlado y no quedar libre
- `items` expone solo `value` y `label` hacia la UI

## Responsabilidades por pieza

### `workflowInboxAutocomplete.service.ts`

Debe:

- encapsular el request HTTP
- usar el endpoint aprobado
- mapear el contrato backend al contrato interno del hook si hace falta
- no conocer UI
- no conocer `AppInputSearch`

### `useWorkflowInboxAutocomplete.ts`

Debe:

- manejar debounce
- controlar `minLength`
- llamar servicio
- exponer `items`, `loading`, `error`
- evitar race conditions
- ignorar respuestas obsoletas si llega una request mas reciente
- limpiar sugerencias cuando el texto ya no cumpla `minLength`
- exponer `setSearchText` y `clear`

### Pantalla / integracion

Debe:

- pasar `options` y `loading` a `AppInputSearch`
- conectar escritura a `autocomplete.setSearchText(value)`
- conectar Enter/click a `table.onQueryChange({ search: value })`
- conectar seleccion de sugerencia a `table.onQueryChange({ search: selectedValue })`
- mantener Enter/click como busqueda real aunque no haya sugerencias
- no consumir directamente el servicio ni el endpoint

## Reglas de implementacion obligatorias

1. Crear hook `useWorkflowInboxAutocomplete`.
2. Crear servicio `workflowInboxAutocomplete.service.ts`.
3. Aplicar `minLength` antes de invocar backend.
4. Aplicar debounce solo en el hook.
5. Exponer `items`, `loading`, `error`, `setSearchText` y `clear`.
6. Mapear `items` al contrato `options` de `AppInputSearch`.
7. Al seleccionar sugerencia, propagar el valor a `table.onQueryChange({ search })`.
8. Mantener busqueda manual por Enter/click aunque no haya sugerencias.
9. Manejar errores sin romper input ni tabla.
10. Si `field` viene en la respuesta:
    - puede preservarse internamente si es util
    - pero `AppInputSearch.options` solo necesita `value` y `label`
11. Si el texto se limpia:
    - limpiar sugerencias
    - cancelar o invalidar request pendiente si aplica
    - no romper el flujo de busqueda libre
12. Evitar doble debounce entre `AppInputSearch` y `useWorkflowInboxAutocomplete`.
13. No introducir `any`; crear tipos explicitos para request, response e item interno.

## Riesgos a evitar

- duplicar debounce entre hook y componente
- llamar backend por cada tecla sin control
- mostrar sugerencias de campos no autorizados
- mezclar autocomplete con listado paginado
- perder busqueda por texto libre
- bloquear input durante loading
- acoplar el hook a `GestionCorrespondencia.tsx`
- sobrescribir resultados nuevos con respuestas viejas
- activar busqueda real de tabla por cada request de autocomplete
- trasladar diferencias del contrato backend al componente UI

## Pruebas unitarias obligatorias

- hook no llama backend cuando texto es menor a `minLength`
- hook llama backend con `search` y `limit` cuando corresponde
- hook expone `loading` durante request
- hook expone `items` al recibir respuesta
- hook maneja error sin lanzar excepcion al componente
- hook limpia sugerencias cuando el texto deja de cumplir `minLength`
- hook ignora respuestas tardias u obsoletas
- servicio llama endpoint esperado
- servicio adapta response backend al contrato interno si aplica
- mapping a `AppInputSearch.options` conserva `value` y `label`
- seleccion de sugerencia llama `table.onQueryChange({ search })`
- Enter/click llaman `table.onQueryChange({ search })` aunque no haya sugerencias
- autocomplete no dispara `table.onQueryChange` por cada request de sugerencias

## Pruebas QT / calidad

- usuario escribe texto suficiente y ve sugerencias
- usuario navega sugerencias con teclado
- usuario selecciona sugerencia y la tabla se filtra
- usuario presiona Enter con texto libre y la tabla se filtra aunque no haya sugerencias
- loading de sugerencias no bloquea escritura
- error de autocomplete no rompe la tabla
- autocomplete respeta limite de resultados
- limpiar texto limpia sugerencias sin romper busqueda libre
- no hay doble debounce observable ni requests duplicadas innecesarias

## Criterios de aceptacion

- autocomplete frontend queda desacoplado de `AppInputSearch`
- hook/servicio consumen contrato backend aprobado o lo adaptan localmente
- `AppInputSearch` recibe solo `options`, `loading` y callbacks
- seleccion de sugerencia filtra la tabla workflow
- texto libre sigue funcionando con busqueda `LIKE`
- existe una unica estrategia de debounce para autocomplete
- pruebas cubren hook, servicio e integracion visual
- la pantalla no consume endpoint ni servicio de autocomplete directamente

## Instruccion final

Antes de implementar:

- validar contrato backend de autocomplete
- validar `AppInputSearch`
- validar `useGestionCorrespondenciaTable`
- validar flujo actual de busqueda libre

Luego:

- implementar con TypeScript estricto
- mantener separacion de capas
- mantener `AppInputSearch` presentacional
- evitar doble debounce

Finalmente reportar:

- decisiones de diseno
- estrategia de debounce
- separacion autocomplete vs busqueda real
- manejo de errores
- manejo de respuestas obsoletas
- como queda preparado para evolucion futura

