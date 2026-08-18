# Pruebas y evidencia — DOC-13

Fecha de registro: 2026-08-17.

## Validaciones automatizadas ejecutadas

| Validación | Comando | Resultado |
| --- | --- | --- |
| Sintaxis de componentes y callback | `node --check js\\java_general\\ConfirmationDialog.js`; `node --check js\\workflow\\workflow-transition-page-presentation.js` | Correcta. |
| Pruebas JavaScript focales | `node --test tests/workflow-transition-ui.test.cjs tests/confirmation-dialog.test.cjs tests/workflow-transition-confirmation-integration.test.cjs tests/workflow-transition-page-presentation.test.cjs` | 20 pruebas aprobadas, 0 fallos. Incluye limpieza sin fila DOM, autocierre del aviso de éxito, bloqueo de cierre, reemplazo y navegación durante envío, y supresión de detalles técnicos de red. |
| Validación OpenSpec estricta | `openspec.cmd validate doc-13-confirmacion-especializada --strict` | Correcta. |
| Compilación Debug .NET Framework | `& 'C:\\Program Files\\Microsoft Visual Studio\\18\\Enterprise\\MSBuild\\Current\\Bin\\amd64\\MSBuild.exe' .\\GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /m` | Código de salida 0; 0 errores. La corrida incremental posterior reportó 1 advertencia de redirección de ensamblado, ajena a los activos DOC-13. |

El repositorio no contiene una solución `.sln` en su raíz; por eso se compiló directamente el proyecto web `GestionDocumental-Docuarchi.net.vbproj`.

## E2E autenticado

No ejecutado. `tools/e2e` está diseñado para contratos DOC-10 y DOC-11, no incluye escenario DOC-13. Además, sus scripts exigen una URL de ambiente, cuentas autorizadas, una tarea de prueba y, según el modo, conexión y consulta de auditoría. Ningún dato de ese tipo se incluyó ni se intentó inferir.

La ausencia de E2E no sustituye la prueba manual: la salida exitosa solo puede comprobarse en un ambiente autorizado y con una tarea activa desechable. La verificación de que la fila se retira exclusivamente después de un éxito correlacionado queda cubierta unitariamente por `workflow-transition-page-presentation.test.cjs` y debe confirmarse manualmente abajo.

## QA manual pendiente

| Escenario | Resultado esperado | Estado |
| --- | --- | --- |
| Activación del piloto | Con gate activo aparece el diálogo moderno; con gate inactivo se conserva el flujo legacy. La desactivación temporal y recarga del 2026-08-17 mostraron la ventana legacy anterior al continuar el flujo; el piloto se restauró inmediatamente después. | Aprobado manualmente el 2026-08-17 |
| Apertura y cancelación | Se muestran solo los datos disponibles; Cancelar, `X` y Escape cierran sin enviar ni modificar fila, contador o visor. | Aprobado manualmente con `melbaa` el 2026-08-16 |
| Éxito | La primera ejecución confirmó el envío, pero dejó visible el contexto y las acciones porque la fila no estaba en el DOM oculto. Se corrigió el callback para restaurar lista y limpiar siempre una selección correlacionada. La repetición muestra el retorno a la lista sin contexto, visor ni acciones residuales. El aviso no intrusivo se verificó: desaparece automáticamente a los seis segundos. | Aprobado manualmente el 2026-08-17 |
| Bloqueo funcional | Se conserva la tarea, se muestra mensaje seguro y solo se habilita reintento cuando el servidor lo permita. La prueba con el gate temporalmente desactivado devolvió antes una validación segura de sesión de tarea por el reinicio de ASP.NET originado por el cambio de configuración: la confirmación permaneció abierta y la tarea no se retiró. | Parcial: se validó el comportamiento visual seguro; no el código específico `WORKFLOW_MODERN_INACTIVE` |
| Error técnico | Se conserva la tarea y se muestra un error controlado sin excepción, HTML ni detalles internos. La simulación Offline del 2026-08-17 evidenció que el navegador exponía `Failed to fetch`; se corrigió para usar el mensaje seguro configurado y la repetición confirmó “No fue posible enviar la tarea. Intente nuevamente.” con la tarea y las acciones conservadas. | Aprobado manualmente el 2026-08-17 |
| Concurrencia | Doble clic no duplica envío; durante la solicitud se bloquean `X`, Cancelar, fondo, Escape, reemplazo y cierre programático. La grabación autorizada con demora temporal del cliente del 2026-08-17 muestra el aviso de espera tras los intentos de cierre, conserva la confirmación abierta y las acciones inactivas hasta completar un único resultado. La prueba focal verifica además que el doble clic ejecuta una sola solicitud. | Aprobado manualmente y por prueba focal el 2026-08-17 |
| Accesibilidad | Foco inicial en el diálogo, Tab/Shift+Tab contenidos, Escape cancela antes del envío y anuncia espera durante él, foco retorna al disparador, roles y anuncio son operables. Las grabaciones autorizadas del 2026-08-17 confirman que Tab y Shift+Tab mantienen el foco visible dentro de la confirmación y no lo trasladan al selector de destinos situado detrás; Escape cierra solo la confirmación y devuelve el foco al botón `Seleccionar` del destino; durante el envío se anuncia que se debe esperar la respuesta. | Aprobado manualmente y por prueba focal el 2026-08-17 |
| Responsive | En escritorio y móvil no hay desborde, recorte ni salida del foco fuera del popup. La grabación autorizada del 2026-08-17 muestra la confirmación y el selector en `390 × 894` y `321 × 568`: título, `X`, resumen, acción primaria y Cancelar permanecen visibles y alcanzables, sin recorte. | Aprobado manualmente el 2026-08-17 |

## Límites de la evidencia

No se registran credenciales, cookies, URL interna, identificadores de personas, radicados reales, SQL ni resultados de negocio. La evidencia visual y la ejecución manual deben agregarse solo desde un ambiente de prueba autorizado.

Al finalizar el QA se restauró la configuración original del piloto (`WorkflowCentroTrabajoModernActive=false` y sin usuarios habilitados). La habilitación para `melbaa` fue temporal y no hace parte de la entrega funcional.
