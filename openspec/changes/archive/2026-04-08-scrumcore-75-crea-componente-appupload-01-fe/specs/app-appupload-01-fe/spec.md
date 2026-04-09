## ADDED Requirements

### Requirement: AppUpload controlado con contrato estable
El componente AppUpload SHALL operar en modo controlado cuando se provea `value` y SHALL reflejar cambios externos sin efectos colaterales.

#### Scenario: Sincronizacion de estado controlado
- **WHEN** el contenedor actualiza `value` con una nueva lista de archivos
- **THEN** AppUpload renderiza la lista actualizada manteniendo el orden

### Requirement: Validacion de archivos previa a carga
El componente AppUpload SHALL validar archivos usando `accept`, `maxSize` y `validateFile` antes de iniciar cualquier carga.

#### Scenario: Archivo rechazado por validacion
- **WHEN** un archivo no cumple `accept` o `maxSize` o `validateFile` retorna false
- **THEN** el archivo no entra en estado `queued` y se notifica via `onError`

### Requirement: State machine estricta por archivo
AppUpload SHALL seguir la secuencia `queued -> uploading -> done/error -> removed` sin saltos de estado.

#### Scenario: Transicion de carga exitosa
- **WHEN** un archivo pasa de `queued` a `uploading` y finaliza sin errores
- **THEN** el estado final del archivo es `done` y se emite `onSuccess`

### Requirement: Eventos completos de upload
AppUpload SHALL emitir `onProgress`, `onSuccess` y `onError` durante el ciclo de carga.

#### Scenario: Progreso sincronizado con UI
- **WHEN** se recibe progreso incremental de carga
- **THEN** `onProgress` se emite con `percent` entre 0 y 100 y la UI refleja ese valor

### Requirement: Soporte Drag & Drop con estados visuales
AppUpload SHALL soportar drag & drop cuando `drag` es true y SHALL mostrar estado visual valido/invalido.

#### Scenario: Hover de archivo invalido
- **WHEN** un archivo invalido se arrastra sobre el drop area
- **THEN** la UI muestra estado visual de rechazo y no agrega el archivo

### Requirement: Layout responsive y cards visuales
AppUpload SHALL renderizar layout responsive (Desktop 46 columnas, Tablet 23, Mobile 2) con cards 1:1 y hover elevation.

#### Scenario: Cambio de breakpoint
- **WHEN** el viewport cambia a mobile
- **THEN** la grilla usa 2 columnas con padding reducido y previews compactos

### Requirement: Accesibilidad y rendimiento
AppUpload SHALL soportar teclado (Enter para preview, Delete para remove) y SHALL evitar re-render completo de la lista.

#### Scenario: Accion por teclado
- **WHEN** el usuario presiona Delete sobre un item enfocado
- **THEN** el archivo se elimina y se emite `onRemove`
