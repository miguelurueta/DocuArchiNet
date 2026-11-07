# DOC-110: limpiar resultados anteriores cuando un nuevo ensayo falla

Fecha: 2026-10-26. Avance de 7.8, D-05/D-07 y RQ-05/RQ-07. Corrección de cliente; no cambia contratos HTTP ni permisos.

## Problema y cambio

Después de un ensayo exitoso, el editor conservaba sus valores si el siguiente ensayo fallaba: solo vaciaba el área de resultados dentro del callback de éxito. Esto podía presentar un valor anterior junto al rechazo actual por sesión, autorización o transporte.

`PerfilImportacionEditor.js` ahora vacía el resultado al solicitar un ensayo nuevo. Mientras espera y después de un fallo, el área ya no presenta los valores previos; el mensaje contractual sigue en el estado del editor. Ante éxito muestra únicamente los valores/incidencias de esa respuesta. La referencia ASPX pasa a `doc110-ensayo-20261026-1` para invalidar caché del script.

No añade solicitudes, reenvíos, guardados ni publicaciones. Conserva el manejo de contexto/revisión y los bloqueos existentes. El cambio no modifica SII ni código de carga individual.

## Verificación

Regresión reproducida **antes** del cambio con el arnés JSDOM existente: al iniciar el segundo ensayo persistía `INDICE_GABINETE / 8: Resultado anterior` donde se esperaba un área vacía. La prueba falló por ese motivo.

Después del cambio:

```powershell
node --test tests/doc110-perfil-importacion-editor.test.cjs tests/doc110-perfil-importacion-client.test.cjs
node --check js/gestion/ImportacionCarpetas/PerfilImportacionEditor.js
node --check tools/e2e/tests/doc110-perfil-guided.spec.cjs
```

**17 PASS**, sintaxis PASS. La regresión del editor cubre sesión requerida, acceso denegado, revisión conflictiva, capacidad no disponible, respuesta inválida y transporte no disponible. Comprueba vaciado inmediato, mensaje tras rechazo, liberación del estado ocupado, ausencia de reenvío/escrituras y documento original intacto. El cliente existente cubre por separado HTTP 401/403/409/503, HTML y timeout; los dobles no acreditan respuestas reales IIS.

Desde `tools/e2e`:

```powershell
npx.cmd playwright test tests/doc110-fuentes-guided.spec.cjs tests/doc110-perfil-guided.spec.cjs --config=playwright.config.cjs
```

**4 passed (8.9 s)** con transporte simulado. El escenario guiado de perfil obtiene un resultado, recibe rechazo en el siguiente ensayo, verifica área vacía y controles disponibles, y continúa con ensayos posteriores correctos. Se conserva regresión fuente/proveedor/perfil y revocación.

Sin build .NET adicional: se modifican únicamente JavaScript, versión de referencia ASPX, pruebas y documentación; no se cambia VB ni el proyecto. Sin despliegue ni nueva autenticación real. OpenSpec estricto y diff de espacios verificados. La tarea 7.8 continúa abierta por sus criterios HTTP reales restantes, incluidas expiración/revocación; este avance no convierte simulaciones en aceptación integrada.
