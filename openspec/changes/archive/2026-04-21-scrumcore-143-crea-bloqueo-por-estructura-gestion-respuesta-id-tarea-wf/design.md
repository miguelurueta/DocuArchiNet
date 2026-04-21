## Context

La ruta de detalle de gestion de correspondencia (`GestionCorrespondenciaRoute`) consulta la estructura por `idTareaWf` mediante `useEstructuraRespuestaIdTarea`. Hoy esa consulta solo alimenta metadatos visuales y la vista `GestionRespuesta` sigue habilitada incluso cuando:

- el `idTareaWf` no es valido,
- la API responde sin estructura (`isEmpty`),
- o hay error de integracion.

El ticket SCRUMCORE-143 requiere bloquear el flujo de "Gestion Respuesta" cuando no exista estructura para el `idTareaWf`, evitando edicion sin contexto documental valido.

## Goals / Non-Goals

**Goals:**
- Bloquear el acceso funcional a la vista de Gestion Respuesta cuando no exista estructura para el `idTareaWf`.
- Definir un estado UI explicito para bloqueo (mensaje + accion de retorno a bandeja).
- Mantener comportamiento actual cuando la estructura exista (sin regresiones en toolbar, tabs y editor).
- Dejar trazabilidad de estados de carga, bloqueo y exito para pruebas.

**Non-Goals:**
- No rediseñar el layout general de `GestionCorrespondenciaRoute`.
- No modificar contratos backend del endpoint `solicita-estructura-respuesta-id-tarea`.
- No cambiar logica de negocio de edicion en `AppEditor`.

## Decisions

### 1) Punto de control en la ruta contenedora, no dentro del editor
- **Decision:** el bloqueo se resuelve en `GestionCorrespondenciaRoute`, que ya es el lugar donde se consulta estructura y controla el panel de detalle.
- **Rationale:** evita duplicar validaciones en tabs internas y centraliza la regla "sin estructura no hay detalle utilizable".
- **Alternativas consideradas:**
  - Bloquear en `GestionRespuestaMainTabContent`: descartado porque solo cubre un tab y dejaria inconsistencias con otros tabs.
  - Bloquear en servicio/hook lanzando excepcion dura: descartado porque mezcla capa de datos con politica de UI.

### 2) Modelo de estados explicito para el detalle
- **Decision:** usar un estado derivado con 4 casos: `loading`, `ready`, `blocked-empty`, `blocked-error`.
- **Rationale:** separa claramente carga de bloqueo definitivo y facilita testing de rutas.
- **Alternativas consideradas:**
  - Tratar cualquier no-exito como loading infinito: descartado por mala UX y falta de cierre de flujo.
  - Redireccion automatica inmediata a bandeja: descartado; oculta la razon del bloqueo al usuario.

### 3) Componente de bloqueo reutilizable en el panel de detalle
- **Decision:** renderizar una superficie de bloqueo en `detailBody` con mensaje contextual y CTA "Volver a bandeja".
- **Rationale:** mantiene consistencia visual del shell actual y evita desmontar estructura del panel.
- **Alternativas consideradas:**
  - Toast + panel vacio: descartado por baja claridad.
  - Modal bloqueante: descartado por friccion innecesaria en navegacion.

### 4) Regla funcional de bloqueo
- **Decision:** bloquear cuando `hasDetail === true` y se cumpla alguno:
  - `idTareaWf` invalido,
  - `isEmpty === true`,
  - `error !== null`.
- **Rationale:** cubre ausencia de estructura real y errores de integracion sin permitir edicion insegura.
- **Alternativas consideradas:**
  - Bloquear solo por `isEmpty`: insuficiente ante errores HTTP/timeout.

## Risks / Trade-offs

- **[Riesgo] Falsos bloqueos por errores transitorios de red** -> **Mitigacion:** distinguir copy de `blocked-error` y ofrecer accion de retorno; en futuras iteraciones agregar "Reintentar".
- **[Riesgo] Regresion en tests existentes de route/tab** -> **Mitigacion:** actualizar y ampliar pruebas unitarias para todos los estados derivados.
- **[Trade-off] Mas ramas de render en la ruta** -> **Mitigacion:** encapsular condicion en helper/selector local para mantener legibilidad.

## Migration Plan

1. Implementar estado derivado de bloqueo en `GestionCorrespondenciaRoute` usando `useEstructuraRespuestaIdTarea` (`loading`, `error`, `isEmpty`).
2. Agregar UI de bloqueo en `detailBody` con mensaje y boton de retorno.
3. Mantener `detailContent` solo para estado `ready`.
4. Actualizar tests de ruta (`GestionCorrespondenciaRoute.spec.test.tsx`) y de hook si aplica.
5. Validar `npm run test -- --run` y `npm run spec:validate`.

Rollback:
- Revertir cambios de render condicional en `GestionCorrespondenciaRoute` y restaurar comportamiento previo.

## Open Questions

- El estado `blocked-error` debe mostrar detalle tecnico minimo (ej. codigo HTTP) o mensaje generico?
- Debe existir boton "Reintentar" en el bloqueo o solo retorno a bandeja para esta version?
- Cuando `idTareaWf` es invalido, el copy debe diferenciarse de "sin estructura"?
