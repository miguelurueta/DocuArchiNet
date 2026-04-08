## Context

La navegacion de `gestion-correspondencia` ya evoluciono en dos fases previas:

- `SCRUMCORE-70` reemplazo el `Drawer` por un shell persistente gobernado por routing.
- `SCRUMCORE-71` refino la experiencia secundaria hasta un panel superpuesto que ocupa el shell del modulo, con accion visible de retorno y sin acoplar `GestionRespuesta` al router.

El ticket `SCRUMCORE-72` no debe reabrir la discusion de layout. Su objetivo es consolidar pruebas de regresion para el patron tipo Gmail ya implementado, de forma que cambios futuros no reviertan el shell observable a un modal, no desmonten la bandeja principal y no oculten el flujo de retorno.

El estado actual ya tiene cobertura de rutas y pagina, pero el riesgo sigue siendo alto porque:

- la navegacion secundaria cambio rapido en varios tickets seguidos
- existen nombres y expectativas heredadas del patron `Drawer`
- el modulo combina `routes`, `pages`, `README` y spec viva en OpenSpec

Por eso este cambio debe fijar el contrato observable de navegacion sin introducir otro refactor funcional.

## Goals / Non-Goals

**Goals:**

- Consolidar el comportamiento esperado de navegacion Gmail en pruebas de regresion del modulo.
- Alinear proposal/spec/tests con el shell real: panel superpuesto, bandeja principal montada, retorno visible y deep-linking.
- Reducir falsos positivos derivados de asserts ligados a implementaciones viejas como `Drawer`, `dialog` o layouts laterales.
- Dejar claro que la URL sigue siendo la fuente de verdad y que `GestionRespuesta` sigue desacoplada del router.

**Non-Goals:**

- No rediseñar otra vez `GestionCorrespondenciaRoute`.
- No volver del panel superpuesto a `Drawer`, modal o split view lateral.
- No agregar backend, estado de negocio ni logica funcional de respuesta.
- No convertir las pruebas en snapshots amplios o asserts sobre clases CSS internas de Ant Design.

## Decisions

### 1. La regresion se valida por contrato observable, no por implementacion interna

Las pruebas deben verificar lo que realmente importa al usuario y a la arquitectura:

- existe shell del modulo
- existe region principal visible
- existe region secundaria superpuesta cuando la subruta esta activa
- la region secundaria no usa `role="dialog"`
- existe accion visible `Volver a la bandeja`
- cerrar vuelve a la ruta base

Se evita afirmar detalles frágiles como estructura DOM exacta, nombres de clases hash de CSS Modules o primitives internas de Ant Design.

**Alternativa descartada:** snapshot integral del shell.
No conviene porque este modulo ya mezcla wrappers de layout, Ant Design y contenido placeholder; el snapshot seria ruidoso y poco util para detectar regresiones de comportamiento.

### 2. Las pruebas de routing siguen siendo el centro del contrato Gmail

`GestionCorrespondenciaRoute.spec.test.tsx` debe seguir siendo la suite principal para validar:

- ruta base
- subruta secundaria
- deep link
- retorno a la bandeja
- ausencia de `dialog`

Ese archivo ya vive en la capa correcta, porque el shell de navegacion esta en `routes/`, no en `pages/` ni en `AppTable`.

**Alternativa descartada:** mover toda la regresion a pruebas de `GestionRespuesta`.
No conviene porque `GestionRespuesta` no gobierna navegacion y el ticket debe reforzar precisamente ese desacoplamiento.

### 3. La pagina de detalle se prueba como contenido contextual, no como controlador de flujo

`GestionRespuesta` debe seguir probandose de forma indirecta dentro del shell o con asserts de contenido visible, pero sin exigirle navegar, cerrar ni conocer rutas.

Esto preserva el contrato actual:

- `GestionRespuesta` solo compone contenido
- `GestionCorrespondenciaRoute` resuelve apertura/cierre
- la URL gobierna el estado secundario

**Alternativa descartada:** pasar callbacks de cierre a `GestionRespuesta` y probarlos ahi.
Se descarta porque reintroduce acoplamiento entre la pagina secundaria y el mecanismo de navegacion.

### 4. La regresion debe neutralizar lenguaje heredado de Drawer

Parte del riesgo actual es semantico: varios tests o mensajes historicos todavia hablan de `drawer`, aunque el contrato vigente ya no lo usa.

Este cambio debe corregir:

- nombres de tests
- copy de spec/proposal si siguen arrastrando el patron viejo
- documentacion que pueda inducir a implementar otra vez un overlay modal

El objetivo es que el lenguaje del repositorio empuje siempre hacia el mismo patron observable.

**Alternativa descartada:** dejar el naming historico mientras el comportamiento pase.
No conviene porque vuelve ambiguo el criterio de aceptacion y facilita regresiones por malentendidos.

### 5. La suite debe cubrir regresion de shell completo y no solo de render aislado

Ademas de los tests de ruta, el modulo debe conservar cobertura de:

- loading inicial de `GestionCorrespondenciaRoutePage`
- error state
- render de la pagina principal con datos resueltos

La razon es simple: el shell Gmail depende de que la ruta principal siga pudiendo vivir montada debajo del panel superpuesto. Si esa ruta rompe sus estados base, la navegacion secundaria deja de ser confiable aunque el test de routing aislado siga pasando.

**Alternativa descartada:** limitar el ticket solo al archivo de routes.
Seria insuficiente; la regresion real del patron Gmail cruza `routes` y `route page`.

## Risks / Trade-offs

- **[Riesgo] Tests demasiado atados al copy visible** -> **Mitigacion:** afirmar encabezados y acciones clave, pero evitar textos largos completos cuando el comportamiento pueda validarse con labels, roles y test ids existentes.
- **[Riesgo] Mantener referencias residuales a `Drawer` en nombres o mensajes** -> **Mitigacion:** normalizar lenguaje a `panel superpuesto`, `shell`, `retorno`, `bandeja principal`.
- **[Riesgo] Cobertura falsa por no validar la relacion entre ruta base y subruta** -> **Mitigacion:** mantener pruebas de deep link y de retorno a la ruta base dentro de `MemoryRouter`.
- **[Riesgo] Regresiones visuales no capturadas por Vitest** -> **Mitigacion:** enfocar la suite en estructura observable estable y complementar con revision de PR cuando cambie CSS del shell.
- **[Riesgo] Confundir este ticket con otro cambio funcional de layout** -> **Mitigacion:** dejar explicito en spec y tasks que el alcance es consolidacion de regresion, no nueva navegacion.

## Migration Plan

1. Ajustar proposal/spec para modificar la capability real `gestion-correspondencia`, no una capability artificial derivada del nombre Jira.
2. Reforzar el delta spec con escenarios de regresion del shell Gmail ya implementado.
3. Actualizar o extender pruebas de `GestionCorrespondenciaRoute` y `GestionCorrespondenciaRoutePage` solo donde falte cobertura observable.
4. Ejecutar pruebas focales del modulo y validar que no se introducen cambios funcionales no deseados.
5. Sincronizar la spec principal y archivar el cambio una vez la regresion quede consolidada.

No requiere migracion de datos ni estrategia de rollback distinta al rollback normal de frontend, porque el cambio esperado es de pruebas y documentacion contractual, con cambios funcionales nulos o minimos.

## Open Questions

- Si el prompt arquitectonico de `03-FE` exige algun escenario adicional de regresion no capturado todavia en la suite actual, habra que incorporarlo en `specs` antes de pasar a `tasks`.
- Si existe documentacion externa al modulo que siga describiendo el patron como `Drawer`, conviene decidir en este ticket si se corrige tambien o se deja para limpieza documental posterior.
