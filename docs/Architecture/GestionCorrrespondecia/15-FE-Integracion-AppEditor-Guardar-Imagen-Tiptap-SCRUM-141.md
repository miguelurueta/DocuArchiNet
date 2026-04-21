# Ticket FE SCRUM-141

## Titulo

Integración Frontend: Subida de imágenes desde `AppEditor` (Tiptap) y reemplazo por URL/UID sin degradar UX/performance

## Rol esperado

Arquitecto de software senior frontend (React 19, TypeScript estricto, hooks, editores rich text, performance, accesibilidad, testing, Clean Architecture).

## Objetivo

Implementar en frontend la inserción de imágenes desde `AppEditor` de forma que:

- La imagen se suba al backend mediante API `guardar-imagen`.
- Se reciba `imageId` / `imageUid` y opcionalmente `publicUrl`.
- El editor inserte/reemplace la imagen con `src` correcto.
- La experiencia sea fluida (sin bloqueos, sin reinicializar el editor, sin re-render innecesario, con feedback claro).

## Dependencias

- Endpoint backend: `POST /api/gestor-documental/editor/guardar-imagen`.
- Contrato: `AppResponses<GuardaEditorImageResponseDto?>`.
- Infraestructura HTTP del proyecto (Axios recomendado para progreso).
- Definición de estrategia para resolución de `imageUid` cuando `publicUrl` sea `null`.

## Contexto existente

Frontend:
- `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.tsx`
- `src/app/Components/UI/AppEditor/*` (Tiptap)

Estado actual:
- El editor permite insertar imágenes, pero no existe un flujo formalizado y desacoplado que controle validación previa, upload, placeholder, reemplazo y manejo de error/race conditions.

## Ubicación esperada

- `src/app/Components/UI/AppEditor/*`
- `src/app/services/gestorDocumental/editor/uploadEditorImage.ts`
- `src/app/hooks/useUploadEditorImage.ts`
- `src/app/types/editorImages.ts`
- `src/app/Components/UI/AppEditor/tests/*`

## API

- Método: `POST`
- Ruta: `/api/gestor-documental/editor/guardar-imagen`
- Content-Type: `multipart/form-data`
- Form field: `File`
- Claim requerido: `defaulalias`

### Validación backend

- Tipos permitidos: `image/png`, `image/jpeg`, `image/jpg`, `image/webp`
  - Nota FE: en navegadores, JPG normalmente reporta `image/jpeg`. `image/jpg` puede no venir; no depender solo de `file.type`.
- Tamaño máximo: 5MB

## Contrato de respuesta

Tipo: `AppResponses<GuardaEditorImageResponseDto?>`

`GuardaEditorImageResponseDto`:
- `imageId` (long)
- `imageUid` (string)
- `fileName` (string)
- `contentType` (string)
- `fileSize` (long)
- `storageTypeCode` (string) (actualmente `"db"`)
- `publicUrl` (string|null)

## Restricciones obligatorias

- NO usar `any`.
- NO llamar API directamente desde componentes de pantalla/módulos consumidores.
- La capacidad debe ser shared/encapsulada en `AppEditor`.
- NO reinicializar el editor durante el upload.
- NO bloquear el thread principal.
- NO permitir inserción sin validación previa.
- Manejar cancelación y race conditions (múltiples uploads simultáneos).
- No dejar estado inconsistente en error.

## Regla arquitectónica obligatoria

La subida de imágenes debe implementarse como capacidad shared encapsulada en `AppEditor`, desacoplando completamente a los módulos consumidores del flujo de:

- Validación
- Upload
- Placeholder
- Reemplazo final
- Cancelación
- Reintento
- Manejo de errores

## Contrato esperado (Frontend)

Tipos:
- `AppResponses<T>`
- `GuardaEditorImageResponseDto`

Hook:

`useUploadEditorImage() -> { upload, loading, error, progress?, cancel? }`

Semántica:
- `upload(file)` valida y sube
- `AppEditor` inserta placeholder optimista
- `AppEditor` reemplaza `src` al éxito
- Permite cancelación y reintento sin reinicializar

## Reglas de implementación obligatorias

### 1) Validación previa (antes de subir)

- Validar tamaño: `file.size <= 5MB`.
- Validar tipo:
  - Preferir validación por extensión + allowlist (si existe configuración SCRUM-139 para editor).
  - Usar `file.type` como segunda barrera cuando exista, pero no como única fuente de verdad.

### 2) Service desacoplado

- Construir `FormData` con el campo exacto `File`.
- No mutar el editor desde el service.

### 3) Hook reusable

- Manejar estado `loading/error`.
- Cancelación:
  - Con Axios: `AbortController`/signal o cancel token equivalente.
- Race conditions:
  - No dejar que respuestas viejas reemplacen placeholders incorrectos.

### 4) Placeholder optimista inmediato (UX)

- Insertar placeholder inmediato para evitar sensación de lag:
  - `src` inicial puede ser `ObjectURL` (`URL.createObjectURL(file)`).
- Marcar el placeholder con un identificador único:
  - `clientUploadId` (uuid) en attrs/metadata del nodo.
- Feedback visible: estado `uploading` (spinner/badge no invasivo).

### 5) Reemplazo correcto al éxito

- Al éxito:
  - Preferir `publicUrl` como `src`.
  - Guardar `imageUid`/`imageId` en metadata/attrs del nodo (para persistencia y diagnósticos).
- El reemplazo debe buscar el nodo por `clientUploadId`, no por posición en el documento.

### 6) Resolución de UID cuando `publicUrl` es null (decisión obligatoria)

- No usar `imageUid` directamente como `src` sin resolver.
- Estrategia requerida:
  - Mantener `ObjectURL` solo durante la sesión (visualización inmediata).
  - Persistir `imageUid` como metadata en el nodo (ej. `data-image-uid`).
  - Definir mecanismo de render/transformación para que, al reabrir el documento, el frontend pueda resolver `imageUid` (requiere endpoint de lectura o estrategia posterior).

Regla de seguridad de persistencia:
- El documento no debe terminar guardado con `src="blob:..."`.

### 7) Manejo de error y reintento

- Si falla el upload:
  - No romper el documento.
  - Placeholder debe transicionar a estado `error` o eliminarse de forma controlada.
  - Permitir “Reintentar” (reutilizando el mismo placeholder o reinserción controlada).

### 8) Memory leak prevention (obligatorio)

- Revocar `ObjectURL` cuando:
  - el upload finaliza con éxito (ya no se necesita)
  - hay error/cancelación
  - el editor/unmount ocurre

## Progreso (compatibilidad técnica)

- `progress` es opcional.
- Si se usa Axios, exponer progreso real (`onUploadProgress`).
- Si no hay soporte de progreso, usar indicador indeterminado (spinner) pero mantener feedback visible.

## Accesibilidad y teclado

- Estados “subiendo” con `role="status"` y “error” con `role="alert"`.
- Acciones “Cancelar/Reintentar” accesibles por teclado.
- No perder cursor/selección del usuario durante cambios de placeholder.

## Pruebas obligatorias

Unitarias:
- Validación tipo/tamaño.
- Construcción de `FormData` (field `File`).
- Hook maneja `loading/error/cancel` y race conditions.
- Garantía de revoke de `ObjectURL`.

Integración UI (Testing Library):
- Placeholder se inserta inmediatamente.
- El editor no se reinicializa.
- `src` se reemplaza al éxito (por `clientUploadId`).
- Error permite reintento.
- Edición continúa durante upload.

E2E (obligatorias si existe suite activa para editor):
- Upload exitoso con PNG/JPG.
- Bloqueo por tamaño > 5MB.
- Manejo de error backend.

## Criterios de aceptación

- Upload correcto vía `multipart/form-data` a `/guardar-imagen` con field `File`.
- Validación FE + backend (5MB, tipos permitidos).
- UX fluida:
  - placeholder inmediato
  - no freeze
  - no reinicialización del editor
  - feedback visible
- Reemplazo correcto:
  - por `publicUrl` si existe
  - si `publicUrl=null`, no se usa `imageUid` como `src`; se persiste `imageUid` como metadata y se evita `blob:` en documento persistido.
- Manejo correcto de errores, cancelación y reintento.
- Sin memory leaks (ObjectURL revocado).
- Tipado estricto sin `any`.
- Sin regresiones, sin errores de build/lint, sin errores en consola.

