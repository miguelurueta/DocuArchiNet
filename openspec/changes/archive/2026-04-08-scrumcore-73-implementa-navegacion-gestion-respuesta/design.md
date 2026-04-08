## Context

`GestionCorrespondencia` hoy tiene dos mecanismos para abrir el flujo contextual de respuesta:

- un boton de toolbar que hace `navigate("respuesta")`
- una columna `acciones` en `AppTable` que ya expone acciones contextuales por fila, pero sin integracion de navegacion para este modulo

Ese estado tiene dos problemas:

1. El acceso a `GestionRespuesta` no esta ligado al registro concreto sobre el que el usuario actua.
2. El entry point correcto ya existe en la tabla, pero la navegacion real sigue viviendo en un boton global redundante.

Al mismo tiempo, `AppTable` es un componente reusable y no debe conocer rutas ni modulos especificos. El diseno correcto necesita permitir que una accion contextual de fila notifique al consumidor lo suficiente para que el modulo resuelva la navegacion, sin hardcodear `gestion-correspondencia` dentro de la tabla.

## Goals / Non-Goals

**Goals:**

- Abrir `GestionRespuesta` desde la accion contextual de la fila relevante.
- Incluir `row.id` en la URL mediante una ruta tipo `respuesta/:id`.
- Eliminar el boton redundante del toolbar.
- Mantener `AppTable` reusable, sin conocimiento de rutas ni de `gestionCorrespondencia`.
- Preservar el shell actual: bandeja principal montada, panel superpuesto y deep linking.

**Non-Goals:**

- No rediseñar el shell de `GestionCorrespondenciaRoute`.
- No mover la logica de navegacion a `GestionRespuesta`.
- No cambiar el comportamiento de acciones `api_call` existentes salvo para no romperlas.
- No introducir una abstraccion generica de router en toda la libreria de componentes.

## Decisions

### 1. La navegacion se resuelve en el modulo consumidor, no en `AppTable`

`AppTable` y `AppTableActionCellRenderer` pueden propagar eventos reutilizables, pero no deben decidir rutas.

El contrato deseado es:

- la tabla informa que ocurrio una accion contextual
- el modulo interpreta `actionId`
- el modulo toma `row.id`
- el modulo hace `navigate("respuesta/<id>")`

**Alternativa descartada:** meter `navigate` dentro del renderer de acciones.
Se descarta porque acoplaria un componente shared a una ruta de dominio concreta.

### 2. La extension reusable debe vivir en el flujo de acciones, no en clicks genericos de celda

`AppTable` ya tiene `onCellClicked` y `onRowClicked`, pero ese contrato es demasiado generico para una accion contextual de menu/boton.

La extension correcta es agregar un callback explicito del flujo de acciones, por ejemplo:

- `onActionTriggered`
- con `actionId`, `row`, `columnKey`

Eso mantiene separados:

- clicks normales de celda
- seleccion de filas
- acciones contextuales del renderer

**Alternativa descartada:** inferir la accion desde `onCellClicked`.
No conviene porque la celda `acciones` puede contener botones, menu dropdown y distintos tipos de comportamiento. El consumidor necesita una señal semantica, no solo un click bruto.

### 3. La ruta debe pasar de `respuesta` a `respuesta/:id`

La navegacion necesita reflejar el registro concreto. Por eso la ruta del modulo debe aceptar parametro y seguir soportando deep linking.

Esto impacta:

- `src/app/routes/routes.tsx`
- pruebas de routing del modulo
- cualquier `navigate("respuesta")` residual en `GestionCorrespondencia`

**Alternativa descartada:** guardar el id solo en estado local.
No conviene porque rompe deep linking, refresh y consistencia con el shell gobernado por URL.

### 4. `GestionRespuesta` sigue desacoplada del router

Aunque la URL incluya `:id`, la pagina secundaria no debe convertirse en controladora del flujo. El acceso al parametro y la composicion del shell siguen perteneciendo a la capa de rutas.

Se puede permitir que el shell o route layer entregue el id como contexto de render, pero `GestionRespuesta` no debe decidir como volver ni como construir la ruta.

**Alternativa descartada:** hacer que `GestionRespuesta` lea y gobierne directamente la navegacion.
Se descarta para preservar la separacion ya estabilizada en tickets anteriores.

### 5. El boton de toolbar debe eliminarse

Mantener el boton y la accion de fila al mismo tiempo dejaria dos entry points para el mismo flujo con semanticas distintas:

- uno global sin registro
- otro contextual con registro

Eso crea ambiguedad de UX y deuda tecnica.

**Alternativa descartada:** conservar el boton como fallback.
No conviene porque ya no representa el flujo correcto cuando la pantalla exige contexto de fila.

## Risks / Trade-offs

- **[Riesgo] Romper otras tablas al tocar el renderer de acciones** -> **Mitigacion:** agregar un callback opcional y mantener intacto el flujo actual cuando el consumidor no lo provee.
- **[Riesgo] Navegar sin `row.id` valido** -> **Mitigacion:** validar `row.id` antes de emitir o de navegar; si no existe, no navegar.
- **[Riesgo] Mezclar `client_event` y `api_call`** -> **Mitigacion:** limitar la nueva integracion al caso de acciones contextuales del modulo y cubrir que `api_call` siga intacto.
- **[Riesgo] Dejar rutas antiguas o tests desalineados** -> **Mitigacion:** actualizar de forma consistente rutas, pruebas de routing y pruebas de pagina.
- **[Riesgo] Aumentar demasiado el contrato shared de `AppTable`** -> **Mitigacion:** agregar el minimo payload reusable necesario: `actionId`, `row`, `columnKey`.

## Migration Plan

1. Corregir proposal/spec para modificar `gestion-correspondencia` y `app-table`, no crear una capability artificial.
2. Extender el flujo reusable de acciones de `AppTable` con un callback opcional para notificar acciones contextuales.
3. Actualizar `GestionCorrespondencia` para escuchar la accion de fila relevante y navegar a `respuesta/:id`.
4. Eliminar el boton `Abrir respuesta contextual` del toolbar.
5. Cambiar la ruta del modulo a `respuesta/:id` y actualizar las pruebas asociadas.
6. Validar que el shell actual sigue funcionando con deep linking y retorno visible.

Rollback:

- revertir el callback opcional del flujo de acciones
- restaurar la ruta simple `respuesta`
- restaurar el boton del toolbar si hiciera falta

## Open Questions

- Confirmar si la accion correcta del backend/shared para este flujo es efectivamente `gestionar_tramite` y si ese `actionId` es estable en la metadata dinamica.
- Definir si `GestionRespuesta` necesita consumir el `id` inmediatamente en esta fase o si basta con dejar la ruta preparada y las pruebas de navegacion.
