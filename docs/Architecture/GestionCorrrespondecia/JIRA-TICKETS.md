# Tickets Gestion Correspondencia

Indice de tickets documentales del modulo.

## Frontend

| Ticket | Documento | Objetivo |
| --- | --- | --- |
| 01 FE | [Implementar AppInputSearch core](./01-FE-Implementar-AppInputSearch-core.md) | Construir o evolucionar `AppInputSearch` como componente reusable basado en AutoComplete + Input, con eventos deterministas, estilos y accesibilidad. |
| 02 FE | [Integrar AppInputSearch en AppToolbar GestionCorrespondencia](./02-FE-Integrar-AppInputSearch-en-AppToolbar-GestionCorrespondencia.md) | Ubicar el buscador en `AppToolbar.actionContent` sin duplicarlo en `AppTableQueryWrapper`. |
| 03 FE | [Conectar busqueda LIKE GestionCorrespondencia](./03-FE-Conectar-busqueda-like-GestionCorrespondencia.md) | Enviar busqueda simple con `SearchType = 2` para activar `LIKE` backend. |
| 04 FE | [Implementar autocomplete workflow inbox frontend](./04-FE-Implementar-autocomplete-workflow-inbox-frontend.md) | Crear hook/servicio frontend para sugerencias sin acoplar `AppInputSearch` a backend. |
| 13 FE | [AppModal Reasignar Respuesta (UI)](./13-FE-AppModal-Reasignar-Respuesta-UI.md) | Implementar el modal `ReasignarRespuestaModal` (AppModal + AppInputTags + AppButton) con responsive y accesibilidad. |
| 14 FE | [Conectar Dropdown Reasignar Tramite](./14-FE-Conectar-Dropdown-Reasignar-Tramite.md) | Conectar la opcion "Reasignar Tramite" del dropdown (acciones de la tabla) para abrir el modal sin modificar la tabla. |
| 15 FE | [Pruebas AppModal Reasignar Respuesta](./15-FE-Pruebas-AppModal-Reasignar-Respuesta.md) | Cubrir con pruebas unitarias/UI el modal y su integracion de apertura/cierre desde la accion del dropdown. |

## Backend

| Ticket | Documento | Objetivo |
| --- | --- | --- |
| 05 BE | [Normalizar SearchType LIKE workflow inbox](./05-BE-Normalizar-SearchType-like-workflow-inbox.md) | Formalizar `SearchType = 2` para busqueda global `LIKE` sin romper `SearchType = 1` ni `SearchType = 3`. |
| 06 BE | [Endurecer LIKE seguridad y rendimiento](./06-BE-Endurecer-like-seguridad-rendimiento-workflow-inbox.md) | Proteger `ApplyLikeSearch` con metadata segura, escape de caracteres y consistencia entre rows/count/export. |
| 07 BE | [Endpoint autocomplete workflow inbox](./07-BE-Endpoint-autocomplete-workflow-inbox.md) | Crear endpoint dedicado de sugerencias limitadas y seguras para Workflow Inbox. |
| 08 BE | [Pruebas regresion busqueda autocomplete](./08-BE-Pruebas-regresion-workflow-inbox-busqueda-autocomplete.md) | Cubrir busqueda, paginacion, exportacion y autocomplete con pruebas backend. |
