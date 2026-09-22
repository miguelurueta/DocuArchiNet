# DOC-74 — Vista segura de recursos externos

## Objetivo y alcance

Mostrar recursos SII aún no importados mediante `GetPreview` y el handler same-origin, sin confiar en URLs externas. La vista es aditiva al modal de importación y no modifica almacenamiento ni visores legacy.

## Componentes

- Presentación: `workflow/Webworkflow.aspx` y `Styles/importar-servicio-web-modern.css`.
- Controlador: `importar-servicio-web-preview.js`.
- Estado: `importar-servicio-web-preview-state.js`.
- Adaptador/mapping: módulos SII existentes.
- Backend reutilizado: ASMX `GetPreview` y `ImportarServicioWebPreview.ashx`.

## Dependencias

Backend B01/B02/B06/B10, sesión Workflow autorizada, proveedor `INTEGRACIONSII` y gate existente. El rollout permanece bloqueado hasta confirmar B10 en el ambiente objetivo.

## Documentos

1. [Arquitectura](01-Arquitectura.md)
2. [Flujo de integración](02-FlujoIntegracion.md)
3. [Contrato y mapping](03-ContratoUploadYMapping.md)
4. [Estados y antirregresión](04-EstadosErroresYAntiregresion.md)
5. [Pruebas y evidencia](05-PruebasEvidencia.md)
6. [Diagramas](06-Diagramas.md)
7. [Metadata](07-Metadata.md)
