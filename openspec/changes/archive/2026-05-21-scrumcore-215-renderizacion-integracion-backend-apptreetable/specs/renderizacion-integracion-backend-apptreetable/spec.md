## ADDED Requirements

### Requirement: Carga backend-driven (query) para AppTreeTable
El sistema SHALL permitir que el consumidor de `AppTreeTable` cargue filas desde un backend usando un wrapper `success/message/data/errors`, transformando el payload a un modelo UI sin acoplar `AppTreeTable` a endpoints o dominios.

#### Scenario: Carga inicial exitosa
- **WHEN** el consumidor invoca `load()` y el backend responde HTTP 200 con `success=true` y `data.Rows[]`
- **THEN** el sistema renderiza una fila por cada elemento en `Rows[]`
- **THEN** el sistema renderiza columnas/celdas basadas en `Rows[].Values`
- **THEN** el sistema determina el orden de columnas según la estructura backend (ver regla de orden)
- **THEN** el sistema conserva metadatos por fila basados en `Rows[].Meta` para expand/acciones

#### Scenario: Orden de columnas backend-driven con fallback determinístico
- **WHEN** el backend incluye configuración de tabla/columnas (por ejemplo `data.Config`/`data.Columns`/equivalente) al enviar `IncludeConfig=true`
- **THEN** el sistema respeta el orden/visibilidad de columnas definido por dicha configuración
- **WHEN** el backend NO incluye configuración de columnas en el response (solo `Rows[].Values`)
- **THEN** el sistema usa un fallback determinístico: el orden de claves observado en `Rows[0].Values` (primera fila) y lo aplica a todas las filas
- **THEN** si `Rows` está vacío, el sistema muestra estado vacío sin intentar inferir columnas

#### Scenario: Error funcional controlado (success=false)
- **WHEN** el backend responde HTTP 200 con `success=false`
- **THEN** el sistema muestra el error funcional usando `errors[0].errorMessage` o `message` como fallback
- **THEN** el sistema mantiene disponible la acción de reintento (si está habilitada)

#### Scenario: Error técnico (HTTP no-2xx o red)
- **WHEN** el backend responde HTTP `4xx/5xx` o falla la red
- **THEN** el sistema muestra un error técnico genérico
- **THEN** el sistema permite reintentar la carga (si aplica)

### Requirement: Modo jerárquico con lazy-load (hierarchical)
El sistema SHALL soportar carga incremental de hijos (lazy-load) cuando el backend indique que un nodo tiene hijos (`Meta.HasChildren=true`), sin degradar el modo plano.

#### Scenario: Expandir nodo con hijos
- **WHEN** el usuario expande una fila cuyo `Meta.HasChildren=true`
- **THEN** el sistema invoca el callback de carga de hijos provisto por el consumidor
- **THEN** el sistema renderiza los hijos retornados como filas anidadas

#### Scenario: Expandir nodo sin hijos reales
- **WHEN** el usuario expande una fila con `Meta.HasChildren=true` pero el backend retorna `Rows=[]`
- **THEN** el sistema no falla ni bloquea la UI
- **THEN** el sistema trata el nodo como hoja para futuras expansiones (evitar cargas infinitas)

### Requirement: Acciones backend-driven por fila (action) con orquestación frontend
El sistema SHALL permitir ejecutar acciones por fila vía un endpoint `action` y actuar según el contrato retornado, sin implementar consumo API->API.

#### Scenario: Acción ver_documento -> resolve
- **WHEN** el usuario ejecuta `ActionId="ver_documento"` sobre una fila
- **THEN** el frontend invoca el endpoint `action` con `RowId`, `NodeType` y `Payload.{IdDocumento, NombreGabinete}`
- **THEN** si `action` retorna `DocumentResolveRequest`, el frontend invoca directamente `POST /api/gestor-documental/documentos/visualizacion/resolve`

#### Scenario: Acción no soportada o regla funcional incumplida
- **WHEN** el backend responde HTTP 200 con `success=false` para una acción
- **THEN** el sistema informa el error funcional usando `errors[0].errorMessage` o `message`
- **THEN** el sistema no deja el componente en un estado inconsistente (sin locks permanentes)

### Requirement: No interferir con plugins y componentes existentes
El sistema SHALL garantizar que la integración backend-driven del Listado en `DocumentosWorkbench` no cambie el comportamiento de otros componentes/plugins, especialmente `AppVisorEmbedPdf`.

#### Scenario: El visor mantiene comportamiento previo
- **WHEN** el usuario carga el rail de “Listado” y navega el visor de PDF en `DocumentosWorkbench`
- **THEN** el visor mantiene su comportamiento previo (sin cambios en zoom/rotate/plugins)
- **THEN** el listado renderiza/scroll dentro de su contenedor sin romper el layout existente
