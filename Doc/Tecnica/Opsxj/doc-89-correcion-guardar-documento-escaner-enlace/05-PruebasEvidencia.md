# DOC-89 - Pruebas y evidencia

- Ticket: DOC-89
- Cambio OpenSpec: doc-89-correcion-guardar-documento-escaner-enlace
- Clasificacion: cross_cutting

Fecha de verificación local: 2026-10-06.

## Evidencia requerida

| Verificación | Resultado |
| --- | --- |
| `npm.cmd --prefix tools/e2e run test:doc89:policy` | 8/8 correctas |
| Activación moderna `doc2WorkflowActivation.test.js` | 26/26 correctas |
| Políticas DOC-82, carga compartida y DOC-88 | 37/37 correctas |
| Núcleo/registro/perfil de plataforma E2E | 12/12 correctas |
| MSBuild Debug de `GestionDocumental-Docuarchi.net.sln` | Correcta, 0 errores, 1 advertencia legacy MSB3247 |
| `git diff --check` | Sin errores; solo avisos de normalización LF/CRLF |

La política DOC-89 verifica UpdatePanel asíncrono, ausencia de `PostBackTrigger`, indicador compacto, limpieza en `finally`, invariancia de `Button_guardar_desicion`, una llamada a `UploadSaveFileScan`, una proyección y las cinco ramas del escáner compartido.

## QA/E2E WebForms

### QA manual positiva

El 2026-10-06 se revisó la grabación manual posterior a la corrección. La evidencia se identifica únicamente por la huella SHA-256 `3F6C43155B405048FF4F7AC5DEFE837EC9BCEFC2DF809785888A609AC3B2778F`; dura 29,07 segundos y tiene resolución 1624x886. No se incorporan el video ni fotogramas al repositorio porque contienen información documental y personal.

La secuencia observable confirma:

- apertura de `Guardar como`, selección de tipología y aceptación;
- interfaz de Enlace perceptible durante toda la operación, sin superficie blanca de pantalla completa;
- indicador `Procesando...` compacto y centrado mientras árbol, visor y controles permanecen visibles;
- incremento visual de 2 a 3 nodos, es decir, exactamente un nodo nuevo;
- conservación del documento seleccionado, del contenido del visor y de la página 3/3;
- desaparición del indicador al concluir, sin bloqueo visual residual.

Esta grabación cierra la QA manual positiva de la tarea 4.2. No demuestra por sí sola cantidad de solicitudes HTTP, invocaciones de almacenamiento ni controles `SELECT`; esos controles pertenecen a la E2E instrumentada. Tampoco cubre rechazo de almacenamiento, `Button_guardar_desicion`, adjunto tradicional o un consumidor no-Workflow.

### E2E instrumentada pendiente

El 2026-10-06 el responsable autorizó expresamente el ambiente de certificación configurado, la cuenta de prueba, la tarea y el documento digitalizado descartable para una única ejecución mutante DOC-89. La autorización se registra sin identidades, identificadores de negocio, contenido documental ni secretos. El perfil runtime reutiliza únicamente URL, módulo, ambiente, DSN ODBC, navegador y política TLS no sensibles de la infraestructura E2E existente; los controles de datos permanecen limitados al `SELECT` parametrizado registrado por la plataforma.

La E2E real instrumentada está autorizada y pendiente de resultado. No se declarará aprobada hasta que la plataforma produzca evidencia saneada satisfactoria.

Intentos saneados del 2026-10-06:

- el preflight inicial rechazó la ruta relativa del perfil antes de autenticación;
- la selección automática y el primer fallback manual rechazaron el contexto antes de almacenar; en ambos casos el control permaneció intacto y la reserva fue liberada;
- la corrida con reconocimiento de contexto Workflow/ENLASE ejecutó el almacenamiento real y superó las comprobaciones de un async postback, indicador compacto sin overlay, una sola proyección, ausencia de navegación y conservación de contexto/visor;
- esa corrida terminó con `SCANNER_LINK_E2E_SCANNER_STATE_NOT_PRESERVED` únicamente al consultar el estado Dynamsoft posterior. Debido a que la escritura pudo completarse, no se reintenta el mismo documento;
- el observador se corrigió para consultar `DWObject` o `Dynamsoft.DWT.GetWebTwain`, usar el paginador visible como segunda fuente y emitir códigos separados para iframe, aceptación y buffer. La corrección del observador pasó 8/8 pruebas DOC-89 y 12/12 pruebas de plataforma.
- después del reinicio se recuperó como respaldo una reserva local huérfana. Un intento posterior se detuvo antes de abrir sesión porque Google Chrome no pudo escribir en su perfil global; la reserva se liberó y el perfil runtime pasó a usar el Chromium administrado por Playwright, cuyo arranque aislado y las 8/8 pruebas DOC-89 fueron satisfactorios;
- la siguiente ejecución almacenó el segundo documento descartable y superó, antes del punto de fallo, las comprobaciones de progreso compacto, un solo clic/postback, una sola proyección y ausencia de navegación. Antes de confirmar el hito manual se abrió el nodo recién creado, cambiando el `src` del visor respecto de la captura previa; por ello el observador detuvo la corrida con `SCANNER_LINK_E2E_CONTEXT_NOT_PRESERVED`. La observación humana confirmó que el documento se guardó y abrió correctamente, pero no sustituye la aserción automática de conservación del visor;
- el hito se aclaró para no seleccionar el nodo nuevo ni cambiar el visor antes de responder y el diagnóstico se separó en `SCANNER_LINK_E2E_TASK_NOT_PRESERVED` y `SCANNER_LINK_E2E_VIEWER_NOT_PRESERVED`. El segundo documento no se reutiliza.
- una ejecución posterior superó las comprobaciones de progreso compacto, un solo clic/postback, una sola proyección, ausencia de navegación y conservación de tarea/visor, y se detuvo únicamente con `SCANNER_LINK_E2E_SCANNER_FRAME_UNAVAILABLE`. La reserva se liberó y el documento almacenado no se reutiliza;
- el iframe estable de Enlace es `#IframeDitaliza_`; el observador dejó de buscarlo por coincidencia de URL en la colección global de frames y obtiene ahora su `contentFrame()` desde ese elemento concreto, evitando que una URL final vacía o distinta oculte el iframe realmente operado.
- con el iframe localizado por DOM, la siguiente ejecución alcanzó la comprobación del visor y se detuvo con `SCANNER_LINK_E2E_VIEWER_NOT_PRESERVED`, pese a que no hubo interacción manual posterior. La proyección incremental no modifica el visor: el falso negativo provenía de comparar el `src` de `#IframeVisor_` aun cuando `#Area_Visor` estaba oculto durante la operación del escáner;
- el observador compara ahora la selección documental explícita y la visibilidad del área del visor. La URL normalizada solo debe permanecer idéntica cuando el visor estaba visible antes del guardado; un `src` oculto actualizado en segundo plano no representa pérdida funcional de contexto. La reserva se liberó y el documento almacenado no se reutiliza.

### Cierre aceptado de la E2E

El 2026-10-06 el responsable solicitó marcar la validación E2E 5.5 como terminada. La aceptación se apoya en la observación funcional repetida del almacenamiento correcto y en que la última ejecución alcanzó, antes del falso negativo del visor oculto, las comprobaciones instrumentadas de progreso compacto, un solo clic/postback, una sola proyección, ausencia de navegación y conservación de tarea. El control `workflow-assignment-state` permaneció sin cambios y la reserva se liberó.

Este cierre no altera ni reinterpreta el artefacto automático: conserva `success: false` y `SCANNER_LINK_E2E_VIEWER_NOT_PRESERVED`. Por tanto, la evidencia demuestra la ejecución autorizada y la aceptación responsable de 5.5, pero no se presenta como una corrida automática integralmente exitosa. El observador corregido queda cubierto por 8/8 pruebas DOC-89 y 6/6 pruebas del kernel de plataforma.

### Aceptación de riesgo para QA manual restante

El 2026-10-06 el responsable autorizó cerrar 4.3 y 4.4 por aceptación de riesgo, sin provocar nuevas mutaciones ni ampliar la autorización a otros recursos. No se declara una reproducción manual adicional del rechazo de almacenamiento, `Button_guardar_desicion`, adjunto tradicional o consumidor no-Workflow.

La decisión se sustenta en la prueba focal que cubre limpieza del progreso ante error, invariancia textual y funcional de `Button_guardar_desicion`, ausencia de cambios en el escáner compartido y sus cinco continuaciones, suites de regresión relacionadas y compilación ya registradas. La salvedad permanece trazable: esos recorridos manuales se aceptan por riesgo y no equivalen a evidencia E2E positiva individual de cada consumidor.

Copiar el perfil de ejemplo con un nombre runtime dentro de `tools/e2e/profiles`, configurar únicamente datos no sensibles y ejecutar:

```powershell
npm.cmd --prefix tools/e2e run test:workflow:platform -- --scenario scanner-link-overlay-execution --profile <perfil-runtime.json> --authorize environment,account,execution,discardable-resource
```

La corrida solicita por TTY la cuenta Workflow y credenciales de lectura, abre el navegador visible y reserva la tarea. No debe ejecutarse hasta disponer de ambiente, cuenta, tarea y documento digitalizado descartables autorizados.
