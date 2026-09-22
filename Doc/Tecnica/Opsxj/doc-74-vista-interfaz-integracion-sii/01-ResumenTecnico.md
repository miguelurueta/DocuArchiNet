# VISTA-INTERFAZ-INTEGRACION-SII

- Ticket: DOC-74
- Cambio OpenSpec: doc-74-vista-interfaz-integracion-sii
- Clasificacion: cross_cutting (Transversal)

## Objetivo

Incorporar al modal moderno de importación una vista segura para recursos SII todavía no almacenados. El navegador obtiene un descriptor temporal mediante `GetPreview` y consume exclusivamente el handler same-origin; una URL externa presente en la fila nunca actúa como autoridad.

## Alcance y compatibilidad

- [x] Superficies afectadas identificadas: `workflow/Webworkflow.aspx`, su code-behind, CSS moderno, scripts de importación SII, `.vbproj` y pruebas focales.
- [x] Comportamiento preservado: importación, almacenamiento y visor legacy permanecen intactos; el visor se invoca únicamente con una selección autorizada ya renderizada por servidor.
- [x] Estrategia de reversa: retirar markup, CSS, registro y módulos aditivos; no requiere reversión de datos ni cambios de esquema.

El gate `WorkflowCentroTrabajoModernActive` permanece en `false`. El rollout productivo sigue condicionado a B10.
