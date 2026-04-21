# Design

## Context

`SCRUMCORE-140` cubre la integracion entre el dropdown de acciones de la tabla de Gestion Correspondencia y el modal `ReasignarRespuestaModal`.

El trigger funcional es la accion `Reasignar Tramite` (ids `reasignar_tramite` / `reasignar_tramite_menu`) emitida por `onActionTriggered`.

Referencia de arquitectura:
- `docs/Architecture/GestionCorrrespondecia/14-FE-Conectar-Dropdown-Reasignar-Tramite.md`

## Scope

Incluye:
- Conexion del handler de acciones para abrir modal de reasignacion.
- Mapeo de contexto de fila para `radicado` y `nota` con fallback seguro.
- Manejo de estado local `open/context/users` en el contenedor de pagina.
- Cierre de modal por cancelar/cerrar.

No incluye:
- Cambios en AppTable base.
- Cambios en query/paginacion/columnas/render de tabla.
- Integracion backend o persistencia de reasignacion.

## Design Decisions

### 1) Integracion por composition, sin tocar AppTable

La tabla ya expone `onActionTriggered`, por lo tanto la integracion se hace solo en el contenedor `GestionCorrespondencia.tsx`.

Resultado:
- la tabla permanece intacta
- el cambio queda encapsulado en handlers de pagina

### 2) Apertura por action id normalizado

Se normaliza `actionId` a lowercase para reducir fragilidad por variantes de origen.

Se abre modal para:
- `reasignar_tramite`
- `reasignar_tramite_menu`

### 3) Resolucion defensiva de datos de fila

Se usa una funcion de resolucion de campos para extraer `radicado` y `nota` desde varias claves probables del row (`RADICADO`, `radicado`, `Asunto`, etc.) con fallback final.

Beneficio:
- evita acoplarse a una sola forma de payload del grid
- mantiene UX consistente aun con datos incompletos

### 4) Flujo coexistente con navegacion actual

La logica de `gestionar_tramite` y `gestionar_tramite_menu` se conserva para navegacion al detalle.

`reasignar_*` abre modal; `gestionar_*` navega. No hay regresion de rutas.

## Technical Approach

- Contenedor: `src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx`
  - estado local modal/contexto
  - handler de accion de tabla
  - render del modal
- Verificacion funcional:
  - `src/modules/gestionCorrespondencia/tests/GestionCorrespondencia.test.tsx`
  - escenario: apertura al disparar `reasignar_tramite`

