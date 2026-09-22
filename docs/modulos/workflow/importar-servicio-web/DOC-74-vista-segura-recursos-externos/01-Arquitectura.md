# Arquitectura

## Decisiones

- La UI reutiliza `ImportarServicioWebApi.getPreview`; no crea transporte HTTP.
- `ExternalKey` y `ProviderId` solicitan el descriptor. La URL de una fila no participa.
- `descriptorUrl` valida base64url y produce únicamente `../workflow/ImportarServicioWebPreview.ashx?d=...`.
- Una máquina pura separa estado y DOM; el controlador deduplica solicitudes concurrentes.
- El panel forma parte del diálogo actual. En móvil se vuelve subvista con retorno explícito.
- La acción del documento importado solo aparece si el identificador interno coincide exactamente con una fila autorizada renderizada por servidor para la tarea actual. El adaptador reutiliza la selección compuesta de esa fila y delega al postback del visor existente.

## Responsabilidades

| Componente | Responsabilidad |
| --- | --- |
| Preview state | Transiciones y snapshot sin dependencias de navegador |
| Preview controller | Solicitud, descriptor, deduplicación, renovación y clasificación MIME/error |
| UI | Render, foco, scroll, navegación y enlace de acciones |
| SII mapper/adapter | Identidad externa, identidad interna opcional y botón por fila |
| ASMX/handler | Autorización, contenido, expiración y streaming |

## Alternativas descartadas

- URL SII directa, `window.open`, base64/JSON y segundo cliente HTTP: rompen la mediación.
- Modificar el visor o almacenamiento: amplía el alcance y arriesga regresiones legacy.
- Recargar en resize/foco: puede consumir dos veces un descriptor temporal.
