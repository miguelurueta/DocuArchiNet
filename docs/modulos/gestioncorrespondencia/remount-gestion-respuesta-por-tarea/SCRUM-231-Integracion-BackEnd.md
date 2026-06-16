# SCRUM-231 - Integración Back-End

## 1) Alcance de integración FE-BE
No se introducen cambios de endpoint ni payloads.
Este ticket no modifica consumo BE, únicamente su ciclo de vida de UI al cambiar `:id`.

## 2) Endpoints involucrados (sin cambios)
- `GET /api/workflow/ruta-trabajo/tareas/{id}` (flujo de detalle según implementaciones previas 219/220/221).
- Rutas usadas por `SCRUM-205` y submódulos de documentos/visor (sin cambios de contrato en este ticket).

## 3) Relación FE-BE
La remoción de estado entre tareas se hace en capa de render de React:
- Cambia `:id` en la ruta.
- React cambia la key del contenedor de detalle.
- Se desmonta el subárbol anterior y se monta uno nuevo.
- Backend sigue respondiendo igual, con la misma semántica de id.

## 4) Manejo de errores y fallback
- Sin cambios de manejo de error en API para este ticket.
- Mantiene los comportamientos previos (`loading`, `blocked`, mensajes vacíos/no seleccionados).

## 5) Retry / fallback
- Retry del backend no tocado por este ticket.
- El aislamiento entre tareas opera por remount del subárbol y no altera flujo de retry existente.

## 6) Matriz FE-BE (resumen)

| Caso | Frontend en 231 | Backend |
|---|---|---|
| Cambio `id` en ruta | remount por key nuevo | entrega data de la nueva tarea |
| `id` inválido | mantiene comportamiento actual de ruta | no se modifica |
| navegación rápida | desmonta árbol anterior | requests en curso se resuelven naturalmente bajo el ciclo de vida React |

## 7) Compatibilidad legacy
- No hay modificaciones de contrato backend.
- No hay cambios de tipos de payload en requests.

## 8) Conclusión
La integración con BE permanece estable; se refuerza únicamente la separación de estado entre instancias de detalle en frontend.
