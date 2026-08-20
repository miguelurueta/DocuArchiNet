# BUSQUEDA-ESCALABLE-ENVIAR-GRUPOS

- Ticket: DOC-26
- Cambio OpenSpec: doc-26-busqueda-escalable-enviar-grupos
- Clasificacion: cross_cutting

## Evidencia requerida

El 2026-08-20 se ejecutó `node --test tests/*.test.cjs` con código 0 y 46 pruebas exitosas. Se cubrieron request y respuesta paginados, límites de término, parámetros de búsqueda, actividad única con varios grupos, ausencia de escrituras, respuesta obsoleta, invalidación de confirmación, layout común para escritorio y móvil y la restauración de la bandeja tras un éxito moderno: contexto oculto, listado visible, scroll horizontal en cero y recálculo de altura.

También se ejecutó MSBuild sobre `GestionDocumental-Docuarchi.net.vbproj` con `/t:Build /p:Configuration=Debug /m`, con código 0. La solución conserva advertencias heredadas de ensamblados y variables legacy, sin errores de compilación atribuidos a DOC-26.

## QA/E2E WebForms

No se ejecutó E2E autenticado, carga ni activación del gate para esta corrección. La prueba manual autenticada requiere secretos efímeros de ambiente y cuenta; no se reutilizaron ni registraron credenciales previas. La evidencia disponible es local, reproducible y no mutante; no sustituye una validación autorizada en ambiente.
