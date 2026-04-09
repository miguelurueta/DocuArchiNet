# PROMPT ARQUITECTONICO Ticket 09 FE

# Navegacion a GestionRespuesta desde accion de fila en GestionCorrespondencia

## Rol esperado

Arquitecto de software senior frontend (React, routing, componentes reutilizables, tablas enterprise, separacion de capas).

## Objetivo

Mover el flujo de apertura de `GestionRespuesta` desde el boton de toolbar hacia la accion por fila de `AppTable` en `GestionCorrespondencia`, navegando a una subruta con identificador del registro.

La solucion debe permitir:

- abrir `GestionRespuesta` desde la accion contextual de cada fila
- incluir el identificador del registro en la URL
- eliminar el boton `Abrir respuesta contextual` del toolbar
- mantener `AppTable` como componente reutilizable sin acoplarlo al dominio `gestionCorrespondencia`

## Contexto existente

- Pantalla objetivo:
  - `src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx`
- Shell de navegacion:
  - `src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.tsx`
- Ruta actual:
  - `src/app/routes/routes.tsx`
- Hook de tabla:
  - `src/modules/gestionCorrespondencia/hooks/useGestionCorrespondenciaTable.ts`
- Tabla reusable:
  - `src/app/Components/UI/AppTable/AppTable.tsx`
  - `src/app/Components/UI/AppTable/AppTable.types.ts`
- Renderer de acciones:
  - `src/app/Components/UI/AppTable/renderers/AppTableActionCellRenderer.tsx`

## Ubicacion esperada

```txt
src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx
src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.tsx
src/app/routes/routes.tsx
src/app/Components/UI/AppTable/*
src/modules/gestionCorrespondencia/tests/*
src/app/Components/UI/AppTable/tests/*
```

## Restricciones obligatorias

- no hardcodear navegacion de `gestionCorrespondencia` dentro de `AppTable`
- no convertir `AppTable` en componente acoplado a una ruta o modulo especifico
- no mantener al mismo tiempo el boton de toolbar y la accion por fila para el mismo flujo
- no romper exportacion, seleccion, paginacion ni contratos actuales de `AppTable`
- no perder el desacoplamiento actual de `GestionRespuesta` respecto al router
- no asumir que todas las tablas del sistema navegan igual
- no usar `any`

## Regla arquitectonica obligatoria

La navegacion debe resolverse en la capa del modulo consumidor, no dentro de `AppTable` como comportamiento fijo.

Esto implica:

- `AppTable` puede exponer o propagar un evento reusable de accion/click
- `GestionCorrespondencia` decide que hacer cuando la accion de fila relevante ocurra
- la interpretacion del `actionId` y la construccion de la URL viven en el modulo

## Contrato de navegacion obligatorio

La navegacion debe seguir un patron como:

```txt
/dashboard/gestion-correspondencia/respuesta/:id
```

Donde `:id` corresponde al identificador del registro seleccionado desde la fila.

### Regla de identificador

El identificador debe salir del row model efectivo de `AppTable`, no de texto visible ni de parsing de columnas.

Contrato esperado:

```ts
type RowNavigationPayload = {
  id: string;
}
```

## Reglas de implementacion obligatorias

1. Eliminar el boton `Abrir respuesta contextual` del toolbar en `GestionCorrespondencia.tsx`.
2. Reutilizar la accion contextual ya presente en la columna `acciones` del grid.
3. Resolver la navegacion desde el modulo `gestionCorrespondencia`, no desde `AppTable` como regla hardcodeada.
4. Extender `AppTable` o su renderer de acciones solo hasta el punto necesario para notificar:
   - `actionId`
   - row seleccionado
   - contexto minimo reutilizable
5. Si la accion relevante es `gestionar_tramite`, el modulo debe traducirla a:

```ts
navigate(`respuesta/${row.id}`)
```

6. La ruta del modulo debe aceptar parametro:

```txt
respuesta/:id
```

7. `GestionRespuesta` no debe tomar decisiones de router; solo consume el contexto que la capa de rutas le entregue.
8. Mantener deep linking directo a `/dashboard/gestion-correspondencia/respuesta/:id`.
9. Mantener visible la bandeja principal debajo del panel secundario segun el shell actual.
10. Si no existe `row.id` valido, no debe navegar y debe mantenerse comportamiento seguro.

## Diseno recomendado

### Opcion correcta

- `AppTableActionCellRenderer` emite un callback reusable cuando ocurre una accion `client_event`
- `AppTable` propaga ese callback al consumidor
- `GestionCorrespondencia` escucha el evento y decide navegar segun `actionId` y `row.id`

### Opcion a evitar

- meter `navigate("respuesta/...")` directamente dentro de `AppTableActionCellRenderer`
- meter conocimiento de `gestionCorrespondencia` dentro de `AppTable`
- hardcodear `gestionar_tramite -> respuesta/:id` en el componente reusable

## Riesgos a evitar

- acoplar `AppTable` a `GestionCorrespondencia`
- romper otras tablas que usan `AppTableActionCellRenderer`
- depender del label visible en lugar de `actionId`
- navegar sin `row.id`
- dejar dos entry points al mismo flujo de respuesta
- romper la ruta actual del shell secundario
- perder compatibilidad con acciones `api_call` ya existentes

## Pruebas unitarias obligatorias

- `GestionCorrespondencia` ya no renderiza el boton `Abrir respuesta contextual`
- la accion de fila relevante sigue renderizandose en la columna `acciones`
- al activar la accion contextual correcta se navega a `respuesta/:id`
- la URL incluye el identificador del registro
- si no existe `row.id`, no navega
- `GestionRespuesta` sigue renderizandose desde la subruta con parametro
- el shell mantiene visible la region principal al abrir `respuesta/:id`
- `AppTable` no queda acoplado a `gestionCorrespondencia`
- acciones `api_call` existentes del renderer no se rompen

## Pruebas QT / calidad

- usuario abre el menu o accion de la fila y entra al detalle contextual correcto
- la URL refleja el id del registro
- al refrescar la pagina en `respuesta/:id` el shell sigue resolviendo correctamente
- volver a la bandeja mantiene el patron de navegacion actual
- no aparece el boton redundante en toolbar
- otras tablas con `AppTable` no cambian comportamiento por este ajuste

## Criterios de aceptacion

- el acceso a `GestionRespuesta` se hace desde la accion de fila y no desde un boton de toolbar
- la navegacion incluye el identificador del registro en la URL
- `AppTable` sigue siendo reutilizable y desacoplado del dominio
- `GestionCorrespondencia` resuelve la navegacion desde el modulo consumidor
- la ruta `respuesta/:id` funciona con deep link
- no se rompe el shell persistente ni el contrato actual de `AppTable`

