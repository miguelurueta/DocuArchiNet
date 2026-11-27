# DOC-127 — Bloqueo del gestor por validación de Excel

Corregido el hallazgo aportado por el usuario, trazado a D-12/RQ-12 y tarea7.1. El árbol ejecuta `OnNodeClicked`, muestra «Cargando» y pulsa el botón de envío; el navegador detiene ese envío por restricciones de controles Excel que pertenecen a `form1`, aunque la ventana esté oculta. Además de los tres `required` vacíos, una posición numérica inválida dejada en el editor produce el mismo bloqueo.

Todos los controles Excel se asocian explícitamente a un formulario auxiliar oculto, hermano del formulario ASP.NET. La ventana conserva su ubicación y estilos. El propietario auxiliar impide sus propios envíos y se elimina al destruir la instancia. Los controles dinámicos usan la misma asociación. Se conservan `required`, límites numéricos, validación explícita de `Guardar plantilla` y validadores propios del gestor. La referencia del script cambia a `doc127-1` para renovar la caché.

Se amplió el fixture Chromium existente de DOC126. Antes de corregir, fallaron los dos casos de envío y pasaron los tres de campos obligatorios:5 pruebas,3 PASS/2 FAIL. Después pasaron **81/81** pruebas locales de UI/cliente/adaptadores DOC126–127, incluidas las cinco nuevas. El caso del árbol ejecuta los handlers reales extraídos del archivo de producción y captura el evento submit; no consulta un expediente en IIS.

La cobertura verifica dos selecciones sucesivas de expediente con Excel nunca abierto, envío después de cerrar con una posición inválida, rechazo de tabla/clave/libro vacíos antes del servidor y conservación de un `required` propio del gestor. La prueba existente de `endRequest` comprueba también un único propietario auxiliar al remontar y su retirada al desaparecer el host. Se conservan los recorridos locales del asistente, modos, CAS, foco, recuperación y scopes.

Un GET sin autenticación del JavaScript estático servido por IIS local devolvió200 y la misma huella SHA256 que el archivo corregido. No acredita una sesión funcional ni aceptación del video. DLL, conector, Web.config, API y CSS conservan sus huellas respecto al corte de aplicación SQL anterior. No se ejecutaron importaciones, SQL, E2E autenticadas ni despliegue de DLL para esta corrección.

Refinamiento15/15 PASS, OpenSpec estricto PASS y `git diff --check` PASS. Las huellas de archivos y registros figuran en el JSON adjunto. **20/33 tareas**: este hallazgo local está resuelto;7.1 conserva pendiente el corte integral, junto con la aceptación real/capacidad y el cierre.
