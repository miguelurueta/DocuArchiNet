# DOC-118 — coordinación y recuperación

- Ticket: DOC-118
- Cambio OpenSpec: doc-118-produccion-importacion-carga-carpeta-lote-recuperacion
- Clasificación: cross_cutting
- Estado: refinamiento aprobado y preparación local en curso; sin despliegue ni integración real nueva.

## Superficies UI

Se inspeccionó js/gestion/ImportacionCarpetas/ImportacionCarpetas.js: consultar, cerrar y resume. Consultar lote recupera la vista; no ejecuta una fase ni borra la señal de detención. Cerrar libera el cliente y conserva referencias; no prueba cancelación del servidor. La consulta desde otra sesión exige autoridad actual y referencia al lote, conservando snapshot e identidades.

## Validacion visual

Esta entrega no modifica marcado, estilos ni interacciones y no acredita una validación visual nueva. COORD-03 requiere evidenciar, en ejecución posteriormente autorizada, detención durante llamada real, cierre del navegador y consulta desde otra sesión. Los botones o hints del cliente no sustituyen los controles de autoridad, revisión y reclamación del servidor.
