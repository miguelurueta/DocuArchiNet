# PROMPT ARQUITECTONICO Ticket 04 FE

# Implementar autocomplete frontend para Workflow Inbox

## Rol esperado

Arquitecto de software senior frontend (React, hooks, servicios HTTP, accesibilidad, performance).

## Objetivo

Implementar la capa frontend para consumir sugerencias de autocomplete de tareas workflow, manteniendo `AppInputSearch` como componente presentacional y ubicando la conexion a backend en hook/servicio desacoplado.

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

Contrato sugerido:

```txt
POST /api/workflowInboxgestion/inboxgestion/autocomplete
```

Request sugerido:

```ts
type WorkflowInboxAutocompleteRequest = {
  search: string;
  limit?: number;
};
```

Response sugerido:

```ts
type WorkflowInboxAutocompleteResponse = {
  items: Array<{
    value: string;
    label?: string;
    field?: string;
  }>;
};
```

## Restricciones obligatorias

- no consumir APIs dentro de `AppInputSearch`
- no incluir endpoints conocidos dentro de `AppInputSearch`
- no retornar filas completas como sugerencias
- no deshabilitar input durante loading
- no ejecutar autocomplete sin `minLength`
- no dejar `limit` sin control
- no romper busqueda de tabla por texto libre

## Reglas de implementacion obligatorias

1. Crear hook de dominio para autocomplete, por ejemplo `useWorkflowInboxAutocomplete`.
2. Crear servicio HTTP dedicado para el endpoint de sugerencias.
3. Aplicar `minLength` antes de invocar backend.
4. Aplicar debounce en hook o reutilizar el debounce del componente de forma explicita, evitando doble debounce.
5. Exponer `items`, `loading`, `error` y metodo para actualizar texto.
6. Mapear `items` al contrato `options` de `AppInputSearch`.
7. Al seleccionar sugerencia, propagar el valor a `table.onQueryChange({ search })`.
8. Mantener busqueda manual por Enter/click aunque no haya sugerencias.
9. Manejar errores sin romper el input ni la tabla.

## Riesgos a evitar

- duplicar debounce en hook y componente sin control
- llamar backend por cada tecla sin limite
- mostrar sugerencias de campos no autorizados
- mezclar autocomplete con listado paginado
- perder busqueda por texto libre
- bloquear input durante loading
- acoplar el hook a un componente especifico

## Pruebas unitarias obligatorias

- hook no llama backend cuando texto es menor a `minLength`
- hook llama backend con `search` y `limit` cuando corresponde
- hook expone `loading` durante request
- hook expone `items` al recibir respuesta
- hook maneja error sin lanzar excepcion al componente
- servicio llama endpoint esperado
- mapping a `AppInputSearch.options` conserva `value` y `label`
- seleccion de sugerencia llama `table.onQueryChange({ search })`

## Pruebas QT / calidad

- usuario escribe texto suficiente y ve sugerencias
- usuario navega sugerencias con teclado
- usuario selecciona sugerencia y la tabla se filtra
- usuario presiona Enter con texto libre y la tabla se filtra aunque no haya sugerencias
- loading de sugerencias no bloquea escritura
- error de autocomplete no rompe la tabla
- autocomplete respeta limite de resultados

## Criterios de aceptacion

- autocomplete frontend queda desacoplado de `AppInputSearch`
- hook/servicio consumen contrato backend aprobado
- `AppInputSearch` recibe solo `options`, `loading` y callbacks
- seleccion de sugerencia filtra la tabla workflow
- texto libre sigue funcionando con busqueda `LIKE`
- pruebas cubren hook, servicio e integracion visual

