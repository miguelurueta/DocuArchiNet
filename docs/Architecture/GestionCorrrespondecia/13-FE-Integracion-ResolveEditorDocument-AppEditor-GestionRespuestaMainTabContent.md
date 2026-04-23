# Ticket FE SCRUM-154

## Titulo

Integración Frontend: `Resolve Editor Document` para inicializar `AppEditor` en `GestionRespuestaMainTabContent`

## Rol esperado

Arquitecto de software senior frontend (React 19, TypeScript estricto, hooks, integración API, accesibilidad, testing, Clean Architecture).

## Objetivo

Integrar en frontend el endpoint `GET /api/gestor-documental/editor/document/resolve` para cargar el HTML del editor (modo `existing` o `initial`) y setearlo en `AppEditor` dentro de `GestionRespuestaMainTabContent`, garantizando una carga controlada, tipada y desacoplada, sin romper el comportamiento actual del editor.

## Dependencia

- Depende de la disponibilidad del endpoint backend `resolve` y del contrato `AppResponses<EditorResolveDocumentResponseDto?>`.
- No depende de otros tickets frontend, pero debe respetar el comportamiento actual de `AppEditor`.

## Contexto existente

Frontend:
- `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.tsx`
- `AppEditor` (Tiptap) con estado controlado (`value` / `onChange`)

Backend:
- Endpoint `GET /api/gestor-documental/editor/document/resolve`
- Soporta `mode`: `existing` | `initial`
- Retorna HTML listo para inicializar Tiptap

Estado actual:
- El editor se inicializa sin integración con `resolve`, usando estado local o valores no sincronizados con backend.
- No se distingue si existe documento persistido o si debe generarse un HTML inicial.

## Ubicación esperada

- UI:
  - `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.tsx`
  - `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/*`
- Hook (caso de uso del módulo, desacoplado):
  - `src/modules/gestionCorrespondencia/hooks/useResolveEditorDocument.ts`
- Service (cliente API reusable):
  - `src/app/services/editor/resolveEditorDocument.ts`
- Tipos:
  - `src/app/types/editor.ts` (o tipos junto al service si el repo lo prefiere; evitar duplicar DTOs)

## Endpoint

- Método: `GET`
- Ruta: `/api/gestor-documental/editor/document/resolve`
- Claim requerido: `defaulalias` (string)

## Parámetros (querystring)

- `contextCode` (string, requerido)
  - Código de contexto del editor (`ra_editor_context_definitions`).
- `entityId` (long, requerido > 0)
  - Identificador de la entidad de negocio (ej: id_Radicado).
- `idTareaWf` (long, opcional)
  - Requerido solo cuando el resultado debe ser `mode=initial` (ya sea porque no existe documento o porque se fuerza `prefer=initial`).
- `templateDefinitionId` (long, opcional)
  - Override explícito de plantilla (solo `initial`).
- `templateCode` (string, opcional)
  - Override de plantilla por código (solo `initial`).
- `prefer` (string, opcional)
  - `existing` | `initial`
  - Default: `existing`

## Reglas de comportamiento (alto nivel)

- Si `prefer` es inválido -> `400 BadRequest` (Validation).
- Si existe más de un documento activo para (contexto, entidad) y no hay criterio determinístico -> `409 Conflict`.
- Si el resultado es `mode=initial` y `idTareaWf <= 0` -> `400 BadRequest` (Validation).

## Respuesta

Wrapper: `AppResponses<EditorResolveDocumentResponseDto?>`

Campos relevantes de `data`:
- `mode`: `existing` | `initial`
- `contextCode`, `entityId`
- `documentId`:
  - `null` en `initial`
  - valor en `existing`
- `templateDefinitionId`, `templateCode`
- `html`: HTML listo para inicializar el editor (Tiptap)
- `images`: lista de imágenes asociadas (en `initial` se retorna `[]`)
- `tokensResueltos`:
  - `null` en `existing`
  - diccionario en `initial`

## Restricciones obligatorias

- NO usar `any`.
- NO acoplar lógica del endpoint directamente en el componente.
  - Regla concreta: la UI no debe hacer `fetch/axios` ni construir la URL; solo consume el hook.
- NO romper comportamiento actual del editor.
- NO sobrescribir contenido si el usuario ya inició edición.
- NO reinicializar el editor si el HTML no cambia.
- NO introducir re-render innecesario del editor.
- NO consumir API fuera de un hook/servicio desacoplado.

## Regla arquitectónica obligatoria

La carga del documento del editor debe resolverse a través de un hook/servicio desacoplado, no directamente en la UI.

Esto implica:
- La UI no conoce detalles del endpoint.
- El hook encapsula `loading`, `error`, `data`, `refetch`.
- `AppEditor` solo recibe HTML final.
- No hay llamadas directas a red dentro del componente.

## Contrato esperado (Frontend)

Definir contrato tipado estricto:

- `export type AppResponse<T> = { data: T; ... }` (alineado al wrapper real del proyecto).
- `export interface EditorResolveDocumentResponseDto { ... }`
- `export type EditorResolveMode = "existing" | "initial"`

Hook:
- `useResolveEditorDocument(params)`
  - expone `data`, `loading`, `error`, `refetch`

## Reglas de implementación obligatorias

- Implementar `resolveEditorDocument(params)` como service (sin UI).
- Implementar `useResolveEditorDocument(params)` como hook desacoplado:
  - encapsula `loading/error/data`
  - expone `refetch()` para reintento
- Integrar el hook en `GestionRespuestaMainTabContent`:
  - La UI decide qué renderizar (loading/error/editor) usando el estado del hook.
- Evitar reinicializar `AppEditor` si el HTML no cambia.
- Manejar race conditions:
  - usar `AbortController` por request o `requestId` en `ref` para ignorar respuestas viejas.
- NO sobrescribir contenido si el usuario ya empezó a editar:
  - Definir una señal “dirty” en UI:
    - `isDirty = editorValue !== initialResolvedHtml` (o una estrategia equivalente)
  - Regla concreta:
    - Solo hacer `setEditorValue(resolvedHtml)` cuando:
      - `!isDirty` **y**
      - `resolvedHtml !== editorValue`
- Mapear correctamente `mode` (`existing` vs `initial`) y conservarlo para diagnóstico.

## Reglas de migración segura

- Si el endpoint falla, el editor no debe quedar en estado inconsistente.
- Debe existir fallback controlado:
  - loading -> placeholder
  - error -> bloque de error con reintento
  - success -> editor editable
- Integración incremental:
  - si la carga no llega, no debe romper el resto del layout ni otras acciones de la pantalla.

## Reglas de consistencia visual

- Loading ocupa el área completa del editor.
- Error reemplaza completamente el editor.
- No coexistencia de editor y error.
- Uso obligatorio de `Skeleton` (Ant Design) para estado de carga.

## Reglas de interacción

- No permitir edición mientras `loading` está activo.
- Permitir reintento sin recargar la pantalla (`refetch()`).
- No bloquear otros componentes del layout.
- Mantener comportamiento actual del editor una vez cargado.

## Accesibilidad y teclado

- Loading debe ser perceptible para lectores de pantalla (ej. `aria-busy="true"` en el contenedor del editor).
- Botón “Reintentar” accesible por teclado.
- No romper foco del usuario al cambiar estados (no forzar focus salvo necesidad).
- No bloquear navegación con teclado en el resto de la pantalla.

## Manejo de errores recomendado (UI)

Dónde mostrar el error (obligatorio):
- Mostrar en la misma pantalla `GestionRespuestaMainTabContent`, dentro del área del editor (workbench) usando `GestionRespuestaEditorContainer` como contenedor.
- No usar toast global como mecanismo principal, ya que el error bloquea el documento del editor.

Cómo mostrar (obligatorio):
- `loading`: renderizar `Skeleton` de Ant Design ocupando el área del editor.
- `400 BadRequest`: error de validación dentro del contenedor del editor.
- `409 Conflict`: error de conflicto dentro del contenedor del editor (si no hay selector de documento, error bloqueante con detalle).
- `5xx`/red: error genérico con botón “Reintentar”.

Opcional:
- si loading supera 2–3s, agregar texto “Cargando documento del editor…” y acción “Reintentar”.

## Riesgos a evitar

- Reinicializar el editor múltiples veces.
- Sobrescribir contenido editado por el usuario.
- Race conditions por requests concurrentes.
- Inconsistencias entre `existing` e `initial`.
- Acoplar el hook al componente (o al revés) de forma rígida.
- Render innecesario del editor.

## Pruebas obligatorias

Unitarias (hook/service):
- El service construye request correctamente (querystring, prefer, etc.) y mapea el wrapper `AppResponses`.
- El hook mapea correctamente `loading`, `error`, `data`.
- El hook permite `refetch()` y mantiene el último estado de forma consistente.
- El hook controla race conditions (abort/ignore respuesta vieja).
- Tipado estricto sin `any`.

Integración UI (Testing Library):
- En success: el editor recibe el HTML y se renderiza.
- En loading: se ve `Skeleton` y no se permite edición.
- En error: se renderiza bloque de error y no se renderiza el editor.
- “Reintentar” llama `refetch()` y transiciona estados.
- No se reinicializa el editor innecesariamente cuando `html` no cambia.
- No sobrescribe contenido cuando `isDirty === true`.

E2E / navegador:
- Si el proyecto tiene suite E2E activa para este módulo, cubrir:
  - flujo `existing` vs `initial`
  - error 400/409
  - reintento

## Criterios de aceptación

- El editor carga HTML desde backend correctamente.
- Soporta `existing` e `initial`.
- Loading visible con `Skeleton` (Ant Design) ocupando el área del editor.
- Error visible con botón “Reintentar” en el área del editor.
- Tipado estricto (sin `any`).
- No sobrescribe contenido si el usuario ya inició edición.
- No reinicializa el editor si el HTML no cambia.
- Maneja race conditions.
- No hay regresiones funcionales del editor.
- No hay errores de build.
- No hay warnings de lint introducidos por el cambio.

## Instrucción final (orden de trabajo)

Antes de implementar:
- Revisar contrato backend real (wrapper `AppResponses` exacto y `EditorResolveDocumentResponseDto`).
- Definir interfaces TypeScript en capa de tipos.
- Diseñar el hook desacoplado (`useResolveEditorDocument`) y el service (`resolveEditorDocument`).

Luego:
- Implementar service.
- Implementar hook (incluyendo cancelación/race control).
- Integrar en `GestionRespuestaMainTabContent` y contenedor `GestionRespuestaEditorContainer`.
- Validar estados UI (loading/error/success) y reglas de no sobrescritura.

Finalmente reportar:
- Contrato definido.
- Estrategia de integración (no acoplamiento + no reinicialización).
- Pruebas ejecutadas y evidencia.
- Validación de no regresión.
- Impacto en consumidores (si aplica).

