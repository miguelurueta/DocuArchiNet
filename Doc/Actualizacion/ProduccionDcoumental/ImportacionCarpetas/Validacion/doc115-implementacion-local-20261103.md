# DOC-115 — implementación y validación local

Refinamiento aprobado explícitamente por el usuario el 2026-11-03. Base 8bf61815f15357947d7d3308fefb5192626988a8, rama feature/DOC-115. No se autentica ni se usa conexión MySQL en esta evidencia.

## Cambios y responsabilidades

El producto ya implementaba las garantías de recepción. Se conserva su código VB, proyecto, DDL, Web.config, carga individual y SII. Se amplía el harness C# existente: rechazo de campos gabinete/expediente/nivel/estado; detección de archivo retenido alterado antes de consumir replay; competencia entre dos archivos por cuota inyectada; replay sin duplicar bytes; rechazo de un junction real. El archivo de prueba temporal elimina solo el enlace, sin recorrer su destino.

tools/e2e/tests/support/doc115-recepcion-importacion.cjs reutiliza createPreparationClient/createPlanControls, safePost y la sesión Producción existente. Incluye lista cerrada de operaciones sin AvanzarLote, presupuestos de preparaciones/lotes/recepciones, validación de recursos, referencias conservadas y controles SELECT. tools/e2e/tests/doc115-recepcion-importacion.spec.cjs contiene RECIB-01/02/03 con evidencia saneada. El runner interactivo usa utilidades compartidas, usuario visible y contraseñas enmascaradas; no solicita URL MySQL ni crea .env/login/configuración Playwright. Se agrega el comando test:doc115:recepcion al paquete existente. La única extensión del soporte DOC-114 permite los scopes RECIB con las mismas guardas y presupuestos.

## Comandos y resultados

Desde el worktree:

- node --test tests/doc115-recepcion-contracts.test.cjs tools/e2e/tests/doc114-plan-controls.test.cjs tools/e2e/tests/doc114-plan-serialization.test.cjs tools/e2e/tests/doc111-importacion-carpetas-driver.test.cjs tools/e2e/tests/doc110-safe-request.test.cjs — 76/76 PASS.
- node --test tests/doc110-perfil-importacion-behavior.test.cjs — 2/2 PASS; compila VB y pruebas C# en .NET Framework y ejecuta disco real con repositorio doble. Las aserciones DOC-115 se exigen expresamente en el wrapper.
- node --check tools/e2e/tests/support/doc115-recepcion-importacion.cjs; node --check tools/e2e/scripts/run-doc115-recepcion-importacion.cjs; node --check tools/e2e/tests/doc115-recepcion-importacion.spec.cjs — PASS.
- Desde tools/e2e: node node_modules/@playwright/test/cli.js test tests/doc115-recepcion-importacion.spec.cjs --list — tres casos descubiertos; ninguno ejecutado.

Inicialmente faltaban dos DLL ignoradas y node_modules en la copia aislada. Se reutilizaron DLL locales verificadas por hash y las dependencias instaladas del paquete existente mediante junction local. No se descargaron paquetes, navegadores ni secretos. El primer intento del harness y una regresión fallaron por esas dependencias; las corridas indicadas anteriores finalizaron PASS después de resolverlas. No cambió VB/proyecto de producto, por lo que no corresponde un despliegue ni build MSBuild adicional por este corte de pruebas.

## Uso reproducible y límites

npm.cmd --prefix tools/e2e run test:doc115:recepcion -- --session <archivo-recursos-revisado.json> --check valida recursos sin autenticar. Sin --check requiere consola interactiva y todas las autorizaciones específicas; no ejecutar antes de revisar archivo, presupuesto y ambiente.

El JSON de sesión contiene scenario, profile (contrato existente DOC-111), identity con userRef/ambitoRef para controles SELECT, runtimeRoot y requests (uno o dos manifiestos de una raíz/un archivo). Los IDs de control se contrastan con el contexto persistido y no confieren autoridad. Nombres pertenecen a profile.rootNames y los bytes se corresponden con la fixture sintética. No incluir credenciales. El script de archivo usa el PDF fixture como contenido; el nombre lógico del manifiesto puede ajustarse a la regla publicada.

RECIB-01 tiene dos preparaciones y hasta un lote; RECIB-02 dos preparaciones, hasta dos lotes y ocho recepciones; RECIB-03 una preparación, hasta un lote y tres recepciones contando la interrumpida. Se conservan recursos. No hay reintentos automáticos. Tras error se consulta evidencia/estado antes de otra ejecución.

La concurrencia HTTP de una misma sesión puede serializarse y no sustituye conexiones MySQL independientes. Cuota agregada finita y divergencias disco/fila se prueban localmente con inyección/dobles; la configuración vigente del lote es ilimitada. La interrupción RECIB-03 destruye una petición HTTP antes del Content-Length completo, pero no se ejecutó en ambiente real. La pérdida de resultado usa descarte deliberado en cliente después de una recepción confirmada; no se presenta como caída de servidor o pérdida de respuesta de red. Si un corte deja INCIERTO, la suite se detiene y conserva evidencia; no fabrica una reconciliación pública.

Estado: 22/28 tareas locales/documentales completadas. 07-A integración y RECIB-01/02/03 reales: NO_EJECUTADO por falta de autorización específica de ambiente, cuentas y recursos descartables. Responsable de habilitar esa ejecución: equipo del ambiente. No cerrar DOC-115 ni atribuir aceptación real a estos resultados locales.
