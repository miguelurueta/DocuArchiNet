# BUSQUEDA-ESCALABLE-ENVIAR-GRUPOS

- Ticket: DOC-26
- Cambio OpenSpec: doc-26-busqueda-escalable-enviar-grupos
- Clasificacion: cross_cutting

## Evidencia requerida

El 2026-08-20 se ejecutó node --test tests/workflow-group-send.test.cjs con código 0 y 12 pruebas exitosas. Se cubrieron request y respuesta paginados, límites de término, parámetros de búsqueda, actividad única con varios grupos, ausencia de escrituras, respuesta obsoleta, invalidación de confirmación y layout común para escritorio y móvil.

También se ejecutó msbuild .\GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /verbosity:minimal con código 0. La solución conserva advertencias heredadas de ensamblados y variables legacy, sin errores de compilación atribuidos a DOC-26.

## QA/E2E WebForms

No se ejecutó E2E autenticado, carga ni activación del gate. Estas operaciones requieren autorización explícita de ambiente, cuentas de prueba y ventana operativa. La evidencia disponible es local, reproducible y no mutante; no sustituye una validación autorizada en ambiente.
