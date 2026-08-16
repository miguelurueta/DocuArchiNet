# Pruebas y evidencia

## Ejecutado

| Comprobación | Comando / evidencia | Resultado |
| --- | --- | --- |
| Pruebas focales de UI | `node --test tests/workflow-transition-ui.test.cjs` | 8/8 aprobadas. Verifica contrato ASMX, error de red, gate inactivo, selección, ausencia de `EjecutarEnvioTarea`, rebootstrap tras UpdatePanel, foco visible y viewport móvil. |
| Sintaxis JavaScript | `node --check js/workflow/workflow-transition-ui.js` | Aprobada. |
| Integridad de diff | `git diff --check` | Aprobada. |
| Build .NET Framework 4.6.1 | `MSBuild.exe GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /m /nologo` | 0 errores; 277 advertencias existentes de referencias y variables potencialmente no inicializadas. |
| Precompilación ASP.NET | `aspnet_compiler.exe -v /GestionDocumental-Docuarchi.net -p <raíz-del-repo> -f <temporal>` | No concluyente: detecta una copia anidada con `web.config` como aplicación hija (`allowDefinition='MachineToApplication'`); no corresponde al marcado DOC-12. |
| OpenSpec estricto | `openspec validate doc-12-lista-moderna-destino --strict` | Aprobada. |
| Gobierno de refinamiento | `npm.cmd --prefix tools/opsxj run opsxj:refine -- DOC-12 --json` | Aprobado: decisiones, requisitos y orígenes de tareas trazables. |
| Gobierno completo | `npm.cmd --prefix tools/opsxj run opsxj:validate -- DOC-12 --json` | Aprobado después de archivar: valida documentación, refinamiento, 21/21 tareas, revisión OpenSpec y evidencia `unit`/`manual_qa` contra el SHA actual. |
| QA autenticada con gate activo | Recorrido autorizado en la aplicación y grabación de QA del 2026-08-16 | Aprobado: carga de destinos para distintas tareas, Escape, retorno de foco, Tab atrapado, selección sin envío y cambio de tarea sin perder la interfaz moderna. |
| QA responsive con gate activo | Grabación de QA del 2026-08-16, corregida y confirmada después del ajuste | Aprobado en escritorio, iPhone XR, Pixel 8, Samsung Galaxy S8+, Surface Duo e iPad Air. En móvil se valida tarjetas, contexto, cierre y acciones accesibles mediante scroll interno; en iPad se conserva tabla. |
| E2E autenticada sin envío | Recorrido autorizado de preview y selección | Aprobada por QA: abrir y seleccionar no ejecuta la transición ni cambia la tarea o la auditoría. |
| Rollback manual con gate inactivo | Recarga completa con el gate restaurado a `false` | Aprobado por QA: se conserva la lista/modal legacy y no aparece la interfaz moderna. |

No se ejecutó carga ni una operación de envío. Al terminar el recorrido se restauró el gate a `false` y se vació el piloto temporal.
