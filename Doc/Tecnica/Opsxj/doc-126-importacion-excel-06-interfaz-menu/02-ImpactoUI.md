# DOC-126 — Interfaz Excel y menú

- Ticket: DOC-126
- Cambio OpenSpec: doc-126-importacion-excel-06-interfaz-menu
- Clasificacion: webforms_ui (WebForms UI)
- Estado: implementación local: 40/44; aceptación UI real y cierre abiertos.

## Superficies UI

Acceso Excel en el único menú Importación, host autorizado independiente de nivel/carpetas y diálogo con selección/edición de plantillas, libro/hoja/modo, límites, preview/incidencias y resultado. Cambiar intención invalida preparación/consentimiento; un estado incierto obliga a consultar el ID. La selección inactiva no recibe archivo.

## Validacion visual

Browser local con transporte simulado verifica teclado, etiquetas, foco, endRequest, retorno al acceso, tres viewport y zoom 200%, incluyendo el botón de confirmación dentro del scroll. Tipos/XSS/incidencias/timeout se prueban como datos sintéticos. La aceptación física con cuentas exclusiva/restringida y servicios reales sigue abierta. Datos visibles se limitan a decisiones operativas, sin tokens, rutas, hashes o diagnósticos del proveedor.

## Corte actual: asistente desde Excel

Ampliación D-16/RQ-16 implementada localmente: 47/52 tareas, 160/160 pruebas y build sin errores. Análisis independiente sin plantilla ni efectos durables; propuesta editable, clave explícita y File sólo en memoria hasta recepción habitual con contraste de hash. No requiere migración SQL y preserva los contratos de carga. Evidencia actual y límites en `Doc/Actualizacion/Importacion/Validacion/doc126-asistente-20261124.md`. 8.8 y las cuatro tareas anteriores de aceptación/gobierno siguen abiertas; sin despliegue ni E2E real. Los números anteriores pertenecen al corte inicial.
