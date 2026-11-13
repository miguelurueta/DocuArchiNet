# DOC-120 — ResumenTecnico

- Ticket: DOC-120
- Cambio OpenSpec: doc-120-produccion-importacion-carga-carpeta-verificacion
- Clasificacion: cross_cutting

## Corte vigente2026-11-13

Avance22/26. Corrección DOC120-UI06: carga inicial con indicador neutro accesible en el menú, sin franja amarilla ni desplazamiento41px; errores y confirmación sin acuse siguen visibles. Cambian JS/CSS y las URLs ASPX de esos recursos. Regresión focal41/41 en Chromium/cliente local con respuestas sintéticas (28 cliente,9 navegación,4 degradación); sin IIS autenticado ni cambios de negocio. Huellas nuevas: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-apertura-20261113.json. Las afirmaciones15/15/sin delta siguientes corresponden al corte histórico2026-11-12. Se conservan las cuatro tareas abiertas y los límites de capacidad/aceptación.

## Antecedente2026-11-12

## Objetivo

Consolidar INT-01/02/03 sobre la interfaz entregada por DOC-119 (main8c0773d4), con resultados trazables por componente y alcance real/local/estático. La continuación sobre73106588 aprueba trabajo local.

## Alcance y compatibilidad

Avance21/25: índices matriz-verificacion.md y resultados.md creados,32 casos clasificados,25 primarios y dependencias de11 componentes;15/15 huellas UI y114 entradas del arnés fijadas. Las pruebas UI99/33 históricas se reutilizan por igualdad de huellas/diff. Regresión actual Framework/DDL y políticas/UI:6 Node únicos entre dos cortes, sin sumar casos internos o políticas repetidas.

Única corrección de código: auxiliar E2E de degradación abre el menú moderno; reproducción previa y regresión local. Sin cambios de producto, SII, almacenamiento, esquema, configuración, permisos o limpieza. INT-03 sigue sin definición/autorización y la aceptación final de límites permanece abierta; no se solicita archivo/cierre.
