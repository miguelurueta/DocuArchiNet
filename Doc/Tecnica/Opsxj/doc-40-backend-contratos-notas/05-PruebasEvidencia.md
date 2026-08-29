# BACKEND-CONTRATOS-NOTAS

- Ticket: DOC-40
- Cambio OpenSpec: doc-40-backend-contratos-notas
- Clasificacion: cross_cutting (Transversal)

## Evidencia requerida

La evidencia local ejecutada el 2026-08-28 es `node --test tests/workflow-notes-contracts.test.cjs tests/workflow-user-send.test.cjs`: 16 pruebas aprobadas y ninguna fallida. Comprueba contratos con `idTarea` e `idNota`, ausencia de dependencias WebForms, permiso de anotaciones calculado en servidor, coherencia `IdRutaWorkflow`/`IdRuta`, repositorio fail-closed e inclusión de archivos en el proyecto.

La compilación `msbuild.exe GestionDocumental-Docuarchi.net.sln /t:Build /p:Configuration=Debug /m` terminó con 0 errores y 310 advertencias heredadas del monolito. No se usaron credenciales, cookies, cadenas de conexión ni contenido de notas.

También se ejecutó por consola interactiva existente de `tools/e2e`, sin navegador ni autenticación Workflow, `npm.cmd --prefix tools/e2e run inspect:notes:schema`. Solo consultó `information_schema`: siete esquemas con `ANOTACION_TAREA` MyISAM, `Dato_Anotacion TEXT utf8` de 65.535 bytes, clave compuesta y un índice individual por tarea; tres `wf_log_workflow` InnoDB, con `datos_operacion LONGTEXT latin1`; máximo aproximado observado de 17.048 notas en un esquema. No se consultaron filas de notas ni se efectuaron escrituras.

El motor confirmado por el usuario para `workflowdocument` y `workflowtconta` es MySQL 5.1. Por tanto no se propone `utf8mb4`: la fundación limita el contenido a Unicode BMP y rechaza pares sustitutos antes de persistir. Tras ese ajuste se verificó `node --test tests/workflow-notes-contracts.test.cjs` con 6 pruebas aprobadas y `msbuild.exe GestionDocumental-Docuarchi.net.sln /t:Build /p:Configuration=Debug /m` con 0 errores y 310 advertencias heredadas.

También se verificó `git diff --check` sin errores de espacio, `npm.cmd --prefix Tools/opsxj run opsxj:refine -- DOC-40 --sync` con estado PASS y `openspec.cmd validate doc-40-backend-contratos-notas --strict` válido.

## QA/E2E WebForms

No aplica E2E ni QA WebForms en DOC-40: la fase no publica endpoint, no migra consumidor y no altera una pantalla. La primera fase que exponga una acción de Notas deberá incluir su E2E dentro del mismo cambio, reutilizar la infraestructura autorizada, exigir ambiente y cuentas explícitos, y conservar el gate desactivado cuando finalice la ejecución.

La fase ya diseñada para esa exposición es `Prompt/02-lectura-listado-y-contador.md`: cubrirá rechazo anónimo, lectura autorizada, aislamiento por tarea y contexto, contador y ausencia de mutación. DOC-40 deja esa E2E bloqueada deliberadamente porque no hay endpoint, ambiente, cuentas ni tarea descartable autorizados para esa fase.
