# NUCLEO-INTRFAZ-INTEGRACION-SII

- Ticket: DOC-72
- Cambio OpenSpec: doc-72-nucleo-intrfaz-integracion-sii
- Clasificación: cross_cutting

## Objetivo

Implementar un núcleo frontend genérico para importar documentos desde servicios mediante un cliente API único, registro explícito de adaptadores, máquina de estados cerrada y UI accesible. La mutación y su orden permanecen en el orquestador backend.

## Alcance y compatibilidad

Se agregan cuatro módulos bajo `js/workflow/importar-servicio-web`, un CSS aislado, un modal WebForms y bootstrap desde code-behind. El gate apagado evita registrar los assets y conserva `btnloadservice`; la reversa consiste en mantener `WorkflowCentroTrabajoModernActive=false`.
