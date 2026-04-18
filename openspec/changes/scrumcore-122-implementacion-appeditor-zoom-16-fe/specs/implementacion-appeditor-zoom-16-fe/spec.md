## ADDED Requirements

### Requirement: Control UI de zoom visual en modo paginado
El sistema SHALL exponer un control UI de zoom visual cuando `AppEditor` opere en `paginationMode="visual"`.

#### Scenario: Control visible solo en modo visual
- **WHEN** `AppEditor` se renderiza con `paginationMode="visual"`
- **THEN** el sistema SHALL mostrar un control de zoom con decremento, valor actual y aumento

#### Scenario: Sin control de zoom en modo continuo
- **WHEN** `AppEditor` se renderiza con `paginationMode="none"`
- **THEN** el sistema SHALL ocultar el control de zoom y no aplicar escalado visual sobre la experiencia continua

### Requirement: Rango y estado de zoom sin breaking changes
El sistema SHALL soportar configuracion de zoom visual con valores por defecto y limites sin romper la API publica existente de `AppEditor`.

#### Scenario: Zoom por defecto en 100 por ciento
- **WHEN** `AppEditor` se renderiza en modo visual sin props explicitas de zoom
- **THEN** el sistema SHALL inicializar el zoom visual en `1` y mostrar `100%`

#### Scenario: Respeto de minimos y maximos configurados
- **WHEN** el usuario intenta disminuir o aumentar el zoom fuera del rango permitido
- **THEN** el sistema SHALL limitar el valor final al rango configurado por `minZoomLevel` y `maxZoomLevel`

#### Scenario: Soporte de modo controlado y no controlado
- **WHEN** el consumidor usa `zoomLevel` y `onZoomChange` o solo `defaultZoomLevel`
- **THEN** el sistema SHALL mantener una experiencia consistente sin requerir cambios en integraciones existentes

### Requirement: Zoom puramente visual sin cambios en persistencia
El sistema SHALL aplicar el zoom como una capacidad exclusivamente visual de la UI, sin alterar el documento persistido ni sus atributos serializados.

#### Scenario: HTML sin cambios tras variar el zoom
- **WHEN** el usuario modifica el zoom visual del editor
- **THEN** el HTML serializado SHALL permanecer igual al previo al cambio de zoom

#### Scenario: Atributos persistidos intactos
- **WHEN** el documento contiene imagenes con `data-width`, `data-align` o nodos `PageBreak`
- **THEN** el sistema SHALL preservar esos atributos sin recalcularlos ni mutarlos por efecto del zoom

### Requirement: Integracion estable con paginacion multi-hoja
El sistema SHALL mantener alineadas la paginacion visual, el scroll del `canvas`, los overlays y el contexto de pagina bajo cambios de zoom.

#### Scenario: Cambio de zoom sin desalineacion de hojas
- **WHEN** el usuario aumenta o disminuye el zoom visual en modo multi-hoja
- **THEN** las hojas visuales, la capa de contenido y los overlays SHALL permanecer alineados

#### Scenario: Scroll continuo y robusto con zoom
- **WHEN** el usuario navega un documento multipagina tras cambiar el zoom
- **THEN** el scroll SHALL mantenerse continuo dentro del `canvas` sin jitter ni saltos bruscos

#### Scenario: Contador de pagina coherente bajo zoom
- **WHEN** el usuario cambia el zoom y sigue editando o navegando el documento
- **THEN** `Pagina X de Y` SHALL seguir calculandose de forma estable y coherente con la hoja activa

### Requirement: Compatibilidad con seleccion, PageBreak e imagenes
El sistema SHALL mantener estabilidad de interaccion en modo visual al combinar zoom con seleccion, `PageBreak` e imagenes locales o remotas.

#### Scenario: Seleccion estable tras zoom
- **WHEN** el usuario selecciona contenido antes o despues de variar el zoom
- **THEN** la seleccion SHALL mantenerse coherente sin romper cursor ni foco del editor

#### Scenario: PageBreak compatible con zoom
- **WHEN** el documento contiene `PageBreak` manuales y el usuario cambia el zoom
- **THEN** el sistema SHALL mantener consistencia de navegacion, calculo de pagina y segmentacion visual

#### Scenario: Imagenes compatibles con zoom
- **WHEN** el documento contiene imagenes redimensionadas, alineadas, locales o remotas
- **THEN** el sistema SHALL mantener una experiencia estable de render, foco y paginacion visual sin romper resize ni alineacion

### Requirement: Recalculo acotado y sin flicker critico
El sistema SHALL recalcular metricas visuales derivadas del zoom de forma controlada para evitar degradacion perceptible de la experiencia.

#### Scenario: Recalculo solo ante cambios relevantes
- **WHEN** cambie el zoom, el contenedor, el contenido o una imagen impacte el layout
- **THEN** el sistema SHALL recalcular metricas y offsets solo en la medida necesaria para mantener coherencia visual

#### Scenario: Sin flicker critico al cambiar zoom
- **WHEN** el usuario interactua con el control de zoom en un documento multipagina
- **THEN** la UI SHALL evitar parpadeos criticos o reposicionamientos erraticos del contenido
