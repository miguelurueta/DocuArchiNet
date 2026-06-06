# implmenta-carga-detalle-gestion-respuesta Specification

## Purpose
TBD - created by archiving change scrumcore-145-implmenta-carga-detalle-gestion-respuesta. Update Purpose after archive.
## Requirements
### Requirement: Aislamiento por `parsedId` del ciclo de vida del detalle
El sistema SHALL montar y desmontar totalmente el arbol de `GestionRespuesta` al cambiar `:id` en la ruta para evitar contaminacion de estado entre tareas.

#### Scenario: Remount completo por cambio de detalle
- **WHEN** la navegacion cambia de `/dashboard/gestion-correspondencia/respuesta/:idA` a `/dashboard/gestion-correspondencia/respuesta/:idB`
- **THEN** el sistema SHALL desmontar la instancia anterior de `GestionRespuesta` y montar una nueva, incluyendo providers y contenido asociado, con identidad React basada en `parsedId`.
- **AND** el sistema MUST resetear estado local, editor, visor y adjuntos del detalle previo.

### Requirement: Anti-stale en transiciones rapidas entre tareas
El sistema SHALL ignorar respuestas asincronas de peticiones antiguas tras un cambio de `parsedId` y evitar contaminacion del estado recien montado.

#### Scenario: Navegacion rapida entre tareas
- **WHEN** el usuario navega rapidamente entre tareas y una peticion en curso de la tarea anterior resuelve despues del cambio
- **THEN** el sistema SHALL descartar esa respuesta o proteger el estado con cancelacion/lifecycle cleanup para evitar sobrescritura del estado de la nueva tarea.

### Requirement: Orquestacion de carga del detalle de gestion respuesta
El sistema SHALL orquestar la carga del detalle de `GestionRespuesta` en un flujo deterministico basado en el estado de estructura asociado al `idTareaWf`.

#### Scenario: Carga inicial de detalle
- **WHEN** el usuario abre la ruta de detalle con un `idTareaWf` potencialmente valido
- **THEN** el sistema MUST entrar en estado de carga hasta resolver la consulta de estructura y MUST diferir la activacion completa del contenido operativo
- **AND** la activacion operativa SHALL depender de una instancia montada por `parsedId`.

#### Scenario: Cambio de ruta reutiliza instancia incorrecta
- **WHEN** la consulta o estado previa de estructura cambia de `:idA` a `:idB`
- **THEN** el sistema SHALL descartar cualquier estado residual de `:idA` y volver a estados predecibles antes de la nueva carga de `:idB`.
- **AND** no SHALL reutilizar `editorValue`, `activeRowId`, `activeFileUrl` ni `files` de la tarea anterior.

#### Scenario: Detalle habilitado en estado ready
- **WHEN** la consulta de estructura finaliza con datos validos
- **THEN** el sistema MUST habilitar la vista de detalle operativa y MUST exponer metadata de contexto consistente

#### Scenario: Detalle bloqueado por contexto no resoluble
- **WHEN** la estructura no puede resolverse (id invalido, respuesta vacia o error de integracion)
- **THEN** el sistema MUST mantener bloqueado el contenido operativo y MUST mostrar estado de bloqueo con accion de salida a bandeja

### Requirement: Activacion controlada de dependencias del detalle
El sistema SHALL activar editor y dependencias de detalle solo despues de confirmar contexto valido de estructura.

#### Scenario: Dependencias diferidas durante loading
- **WHEN** el detalle esta en estado de carga
- **THEN** el sistema MUST evitar interacciones operativas de edicion y MUST mostrar placeholders/contexto transitorio en cabecera

#### Scenario: Dependencias habilitadas en ready
- **WHEN** el detalle transiciona a estado ready
- **THEN** el sistema MUST habilitar editor, tabs y acciones del detalle sin requerir recarga manual del usuario

### Requirement: Consistencia de render entre tabs y panel de detalle
El sistema SHALL mantener consistencia de render del panel de detalle y sus tabs frente a transiciones de carga.

#### Scenario: Transicion estable entre estados
- **WHEN** el estado del detalle cambia entre `loading`, `ready` y `blocked`
- **THEN** el sistema MUST evitar renderes ambiguos (contenido operativo junto a estado bloqueado) y MUST conservar semantica de navegacion del panel secundario
- **AND** un cambio de `parsedId` SHALL forzar estado inicial predecible del subarbol y limpieza del estado local.

