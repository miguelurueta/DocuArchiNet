# Ticket FE SCRUM-139

## Titulo

Integración Frontend: Configuración Upload **solo imágenes** para adjuntos de `AppEditor`

## Rol esperado

Arquitecto de software senior frontend (React 19, TypeScript estricto, hooks, integración API, componentes reutilizables, accesibilidad, testing, Clean Architecture).

## Objetivo

Acoplar la funcionalidad de adjuntar **imágenes** de `src/app/Components/UI/AppEditor` para que restrinja:

- **Tipo de imagen** (extensión permitida)
- **Peso máximo** (bytes)

usando configuración dinámica desde la API:

- `GET /api/gestor-documental/configuracion-upload?nameProceso=EDITOR`

La validación debe ocurrir **antes** de intentar subir/adjuntar la imagen al editor y debe ser consistente en toda la app para el proceso `EDITOR`.

## Alcance (solo imágenes)

Este ticket aplica únicamente a **imágenes** adjuntas desde `AppEditor`.  
Cualquier archivo no-imagen (PDF/DOC/ZIP/etc.) debe ser **bloqueado**, aunque el editor soporte adjuntos genéricos.

## Dependencia

- Depende del endpoint backend `configuracion-upload` y del contrato:
  - `AppResponses<List<RaConfiguracionUploadModel>>`
- Depende de que el front ya provea el claim `defaulalias` (usado como `defaultDbAlias`) vía autenticación/headers.
- No depende de otros tickets frontend, pero debe respetar el comportamiento actual de `AppEditor` (sin regresiones).

## Contexto existente

Frontend:
- `src/app/Components/UI/AppEditor/*` (Tiptap)
- Existe funcionalidad de adjuntar imágenes

Backend:
- Método: `GET`
- Ruta: `/api/gestor-documental/configuracion-upload`
- Querystring requerido: `nameProceso`
- Claim requerido: `defaulalias` (usado como `defaultDbAlias`)

Ejemplo:
- `GET /api/gestor-documental/configuracion-upload?nameProceso=EDITOR`

Nota:
- En backend, `extensionUpload` viene como string con extensiones separadas por coma (el ejemplo de documentos es referencial del formato).
- Para `EDITOR`, se espera configuración de **imágenes**, por ejemplo: `.JPG,.PNG` y extensible a futuro (`.JPEG,.WEBP,.GIF`, etc).

## Contrato de respuesta

Tipo: `AppResponses<List<RaConfiguracionUploadModel>>`

### Wrapper: `AppResponses<T>`

- `success` (bool)
- `message` (string)
- `data` (T)
- `meta` (AppMeta|null)
- `errors` (object[]|null)

### DTO: `RaConfiguracionUploadModel`

- `idConfigUploadGestion` (int?)
- `extensionUpload` (string?)  (ej: `.JPG,.PNG,.JPEG`)
- `lengUpload` (long?)         (tamaño máximo en bytes, ej: `10485760`)
- `nameProceso` (string?)
- `estadoProceso` (int?)

## Restricciones obligatorias

- NO usar `any`.
- NO acoplar la llamada a la API directamente en el componente UI que renderiza el editor:
  - Regla concreta: la UI no debe hacer `fetch/axios` ni construir la URL; solo consume un hook.
- La configuración debe resolverse mediante hook/service desacoplado.
- Validar **antes** de adjuntar/subir.
- No romper el comportamiento actual del editor cuando la API no está disponible: fallback controlado y explícito.

## Arquitectura esperada (obligatoria)

Ubicación sugerida (ajustar a convenciones del repo):

- Tipos:
  - `src/app/types/uploadConfig.ts`
    - `AppResponses<T>` (alineado al DTO real del proyecto)
    - `RaConfiguracionUploadModel`
    - `UploadConfigEditorImages` (modelo FE normalizado)
- Service (cliente API reusable):
  - `src/app/services/gestorDocumental/getUploadConfig.ts`
    - `getUploadConfig({ nameProceso })`
- Hook:
  - `src/app/hooks/useUploadConfig.ts` (o `useUploadConfigByProceso.ts`)
    - `useUploadConfig("EDITOR")` -> `{ data, loading, error, refetch }`
- Integración:
  - Punto único de validación dentro de `src/app/Components/UI/AppEditor/*` (antes de adjuntar la imagen).

## Reglas de parsing de extensiones (obligatorio)

`extensionUpload` viene como string con extensiones separadas por coma, ejemplo:

- `".JPG, .PNG, JPEG"`

Normalización requerida:

1) Split por `,`
2) Trim
3) Uppercase
4) Asegurar prefijo `.`

Salida esperada:

- `[".JPG", ".PNG", ".JPEG"]`

## Reglas de validación (obligatorio)

### 1) Validación por extensión (fuente de verdad)

- Determinar extensión desde `file.name` (último `.`, uppercase).
- Si el archivo no tiene extensión, bloquear.
- Bloquear si la extensión no está en la lista permitida.

### 2) Validación por tamaño

- Usar `lengUpload` como tamaño máximo permitido en bytes.
- Bloquear si `file.size > lengUpload`.

### 3) Validación opcional por MIME (segunda barrera)

- Si `file.type` está disponible:
  - Debe iniciar con `image/`
- Esta validación es complementaria; la fuente de verdad para permitir/bloquear es `extensionUpload`.

## Estados y UX (obligatorio)

- `loading`: indicar “Cargando configuración de upload…” y **bloquear** adjunto.
- `error`: mostrar `message` y (si existe) `errors` + botón “Reintentar” (`refetch`).
- `empty` (`success=true`, `data=[]` o sin reglas activas):
  - Política obligatoria: **bloquear adjunto** y mostrar “Sin configuración de upload para imágenes (EDITOR).”
- Validación fallida:
  - Tipo no permitido: “Tipo de imagen no permitido. Permitidos: …”
  - Tamaño excedido: “La imagen supera el límite de X MB.”

## Accesibilidad (obligatorio)

- Los mensajes de validación deben ser perceptibles para lector de pantalla (ej. `role="alert"`).
- Botón “Reintentar” accesible por teclado.
- No romper el foco del usuario al cambiar de estado.

## Riesgos a evitar

- Permitir adjuntar archivos no-imagen por configuración incorrecta.
- Comportamientos distintos según pantalla (la validación debe ser centralizada en `AppEditor`).
- Reintentos que disparen validaciones inconsistentes (cache/estado).

## Pruebas obligatorias

Unitarias:
- Parser de `extensionUpload` -> lista normalizada.
- Validador de archivo (extensión/tamaño/MIME opcional).

Integración UI:
- Con config OK (ej. `.JPG,.PNG`): permite adjuntar solo si pasa reglas.
- Extensión inválida: bloquea y muestra error.
- Tamaño excedido: bloquea y muestra error.
- `loading`: bloquea adjunto y muestra estado.
- `error`: muestra error + “Reintentar”.
- `empty`: bloquea adjunto y muestra mensaje.

E2E (si existe suite para editor):
- Adjuntar `.JPG` permitido con tamaño válido.
- Adjuntar `.PDF` bloqueado.
- Error backend mostrado correctamente.

## Criterios de aceptación

- `AppEditor` restringe adjuntos a **solo imágenes** según `configuracion-upload` (`nameProceso=EDITOR`).
- `extensionUpload` se interpreta correctamente aunque venga con/sin punto y con espacios.
- `lengUpload` se respeta en bytes.
- Estados `loading/error/empty` tienen UX clara y bloquean adjunto.
- Tipado estricto (sin `any`).
- No hay regresiones en el editor (edición y render siguen funcionando).
- No hay errores de build/lint introducidos por el cambio.
