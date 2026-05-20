## ADDED Requirements

### Requirement: AppTreeTable soporta data backend-driven (query)
El sistema SHALL permitir que `AppTreeTable` renderice filas provenientes de una respuesta backend (wrapper `success/message/data/errors`) transformada a un modelo de UI, sin acoplar el componente a endpoints o dominios específicos.

#### Scenario: Carga inicial exitosa
- **WHEN** el consumidor invoca `load()` y el backend responde `success=true` con `data.Rows[]`
- **THEN** el sistema renderiza una fila por cada elemento en `Rows[]`
- **THEN** el sistema muestra valores de columnas basados en `Rows[].Values`
- **THEN** el sistema conserva metadatos por fila basados en `Rows[].Meta` para habilitar acciones posteriores

#### Scenario: Error funcional controlado (success=false)
- **WHEN** el backend responde HTTP 200 con `success=false`
- **THEN** el sistema muestra el mensaje funcional usando `errors[0].errorMessage` o `message` como fallback
- **THEN** el sistema permite reintentar la carga sin recargar la página completa

#### Scenario: Error técnico HTTP
- **WHEN** el backend responde con HTTP `4xx/5xx` o falla la red
- **THEN** el sistema muestra un error técnico genérico
- **THEN** el sistema ofrece una acción de reintento (si está habilitada por el consumidor)

### Requirement: AppTreeTable soporta modo jerárquico con lazy-load
El sistema SHALL soportar carga incremental de hijos (lazy-load) cuando el backend indique que un nodo tiene hijos (`HasChildren=true`), sin afectar el modo plano.

#### Scenario: Expandir nodo con hijos
- **WHEN** el usuario expande una fila con `HasChildren=true`
- **THEN** el sistema invoca un callback de carga de hijos provisto por el consumidor
- **THEN** el sistema renderiza los hijos retornados como filas anidadas

#### Scenario: Expandir nodo sin hijos reales
- **WHEN** el usuario expande una fila con `HasChildren=true` pero el backend retorna `Rows=[]`
- **THEN** el sistema no falla ni bloquea la UI
- **THEN** el sistema trata el nodo como hoja para futuras expansiones (no repetir carga infinita)

### Requirement: Acciones backend-driven por fila (action)
El sistema SHALL permitir ejecutar acciones por fila mediante una operación de backend (`action`) y actuar según el contrato retornado, sin implementar consumo API->API.

#### Scenario: Acción "ver_documento" retorna DocumentResolveRequest
- **WHEN** el usuario ejecuta la acción `ver_documento` sobre una fila
- **THEN** el frontend invoca el endpoint de `action` con `RowId`, `NodeType` y `Payload` mínimo (`IdDocumento`, `NombreGabinete`)
- **THEN** si el backend retorna `DocumentResolveRequest`, el frontend invoca directamente `visualizacion/resolve` con `IdDocumento` y `NombreGabinete`

#### Scenario: Acción no soportada o regla funcional incumplida
- **WHEN** el backend responde `success=false` para una acción
- **THEN** el sistema informa el error funcional al usuario usando `errors[0].errorMessage` o `message`
- **THEN** el sistema no deja el componente en un estado inconsistente (sin locks permanentes)

### Requirement: No interferencia con componentes existentes
El sistema SHALL garantizar que la integración de backend-driven rendering en `AppTreeTable` no cambie el comportamiento de otros componentes del Workbench (incluyendo `AppVisorEmbedPdf`).

#### Scenario: Render del Listado no afecta el visor
- **WHEN** el usuario navega en `DocumentosWorkbench` y carga el rail de Listado
- **THEN** el visor de PDF mantiene su comportamiento previo (sin cambios en zoom/rotate/plugins)
- **THEN** el listado se renderiza y scrollea dentro de su contenedor sin romper el layout existente

