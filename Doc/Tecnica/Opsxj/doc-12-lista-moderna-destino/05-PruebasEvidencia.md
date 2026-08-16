# LISTA-MODERNA-DESTINO

- Ticket: DOC-12
- Cambio OpenSpec: doc-12-lista-moderna-destino
- Clasificacion: cross_cutting (Transversal)
## Evidencia requerida

- `unit` — 2026-08-16: `node --test tests/workflow-transition-ui.test.cjs`, 8/8 aprobadas. Cubre envoltorio ASMX, contexto permitido, error de red, gate inactivo, contrato de selección, ausencia de `EjecutarEnvioTarea`, rebootstrap tras UpdatePanel y el shell móvil con cabecera/cierre visibles.
- `build` — 2026-08-16: `MSBuild.exe GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /m /nologo`, 0 errores. El proyecto conserva 277 advertencias históricas de referencias y variables potencialmente no inicializadas.
- `aspnet_precompile` — 2026-08-16: no concluyente. `aspnet_compiler.exe` se detiene por un `web.config` de una copia anidada que ASP.NET interpreta como aplicación hija; no informa un error atribuible a DOC-12.
- `manual_qa_active` — 2026-08-16: recorrido autorizado con la interfaz nueva. Verifica carga en distintas tareas, Escape, retorno de foco, Tab atrapado, selección sin envío y responsive en iPhone XR, Pixel 8, Samsung Galaxy S8+, Surface Duo, iPad Air y escritorio. La primera revisión detectó corte de modal en móviles estrechos; se corrigió y se confirmó nuevamente.
- `e2e_authenticated` — 2026-08-16: recorrido autorizado de apertura y selección. No cambió tarea, estado ni auditoría; no se ejecutó envío.
- `manual_qa_rollback` — 2026-08-16: con el gate restaurado a `false` y sin piloto, la recarga presentó la lista/modal legacy en lugar de la interfaz moderna.

## QA/E2E WebForms

La QA con gate activo, la E2E autenticada y el rollback manual con gate inactivo se ejecutaron con autorización. El gate quedó finalmente en `false` y el piloto vacío; la página volvió a la lista/modal legacy al recargar.

La validación de gobierno ejecutada el 2026-08-16 aprobó documentación, refinamiento, 21/21 tareas, revisión OpenSpec y evidencias `unit` y `manual_qa` para el SHA actual. DOC-12 permanece archivado; las acciones restantes, si aplican, son el PR y el cierre remoto de Jira.
