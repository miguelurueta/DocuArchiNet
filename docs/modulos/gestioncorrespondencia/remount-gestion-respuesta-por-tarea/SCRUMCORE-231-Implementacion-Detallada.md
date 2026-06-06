# SCRUMCORE-231 — Implementación Detallada

## 1) Alcance del cambio
Este ticket aplica una estrategia de remount de ciclo de vida en la ruta de detalle de `GestionRespuesta` para aislar estado entre tareas al cambiar `:id`, sin introducir lógica funcional nueva.

## 2) Archivos modificados
- `src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.tsx`
- `src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.spec.test.tsx`
- `openspec/changes/scrumcore-231-remount-gestion-respuesta-por-tarea/tasks.md`

## 3) Regla de remount
- `parsedId` se usa como discriminante principal de identidad del subárbol de detalle.
- Se establece una key estable por tarea:
  - `gestion-respuesta-${parsedId}`
  - fallback explícito cuando `parsedId` no es válido.
- La key se aplica en el contenedor que engloba el detalle completo (providers, tabs, árbol, visor) para evitar remount parcial.

## 4) Estrategia de scope de key
- `key` sobre el nodo de detalle principal dentro de `GestionCorrespondenciaRoute`.
- Justificación: garantiza desmontaje/montaje de todo el árbol visual y contextual de detalle, evitando reutilización indirecta de providers o estado local.

## 5) Estado y comportamiento preservado
- Preservadas las guardas de flujo:
  - estado `loading`
  - estado `blocked`
  - render de “sin selección válida / detalle oculto”.
- No se alteraron contratos de data fetching, tabs, actions ni providers.
- No se tocaron endpoints ni contratos backend.

## 6) Cambios en pruebas
- Se agregó `DetalleRemountProbe` para contar remounts de un marcador de detalle en ruta.
- Se agregó `StatefulRemountProbe` para confirmar que estado local asociado al detalle no persiste entre rutas.
- Se añadieron casos:
  - `/respuesta/924` → `/respuesta/925`: remount invocado.
  - Estado local de panel de detalle reiniciado.

## 7) Anti-stale / lifecycle
- En este ticket se estableció la base de remount con cambio de key.
- No se agregaron cambios funcionales adicionales a providers/services.
- Los casos de stale async pendientes quedan en checklist de hardening y regresión para validación complementaria (pruebas de navegación rápida y teardown).

## 8) Interacción con refactors previos (219/220/221)
- **SCRUMCORE-219**: Tipado/mapeo de `idRespuestaRadicado` → no impacta.
- **SCRUMCORE-220**: Contexto transversal documental → permanece intacto.
- **SCRUMCORE-221**: Hook de documentos consume contexto de gabinete → permanece intacto.
- **231**: Remount del detalle evita contaminación cruzada entre tareas pese al contexto compartido.

## 9) Riesgo controlado
- Riesgo de remount parcial minimizado con key en el contenedor de detalle completo.
- Riesgo de regresión silenciosa controlado con pruebas unitarias de remount y tareas pendientes de regresión UI.

## 10) Criterios Go/No-Go
- **Go**: key aplicado correctamente, pruebas de remount verdes, lint limpio, sin cambio de API funcional.
- **No-Go**: remount parcial, cambio no determinístico de key, regresión en navegación o tests.
