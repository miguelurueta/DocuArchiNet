<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05 -->
## 1. Refinamiento

- [x] 1.1 [S] Consolidar alcance con evidencia DOC-28, comando legacy y adaptadores modernos. Área/archivos: `openspec/changes/doc-29-interfaz-moderna-enviar-usuario/refinement.md`. Origen: D-01, RQ-01. Verificación: `npm.cmd --prefix tools/opsxj run opsxj:refine -- DOC-29` no reporta marcadores pendientes.
- [x] 1.2 [S] Convertir decisiones, requisitos y compatibilidad en diseño y especificación verificables. Área/archivos: `design.md`, `specs/interfaz-moderna-enviar-usuario/spec.md`. Origen: D-01, RQ-01. Verificación: D-01..D-05 y RQ-01..RQ-05 aparecen en refinement, diseño y especificación.

## 2. Implementación

- [x] 2.1 [S] Reemplazar enlace legacy por `workflow-user-send-trigger` sin atributo de gate. Área/archivos: `workflow/Webworkflow.aspx`. Origen: D-01, RQ-01. Verificación: no hay `onclick` hacia `ImageButtonEnviarUsuario` y el botón es `type="button"`.
- [x] 2.2 [M] Registrar estilos, scripts y bootstrap exclusivos antes de la rama del gate de Grupo/Continuar flujo. Área/archivos: `workflow/Webworkflow.aspx.vb`. Origen: D-01, RQ-01. Verificación: con gate apagado solo se enlaza el trigger de usuario y no se invoca `WorkflowModernPresentationBootstrap`.
- [x] 2.3 [M] Retirar la cadena Web Forms exclusiva de envío legacy de usuario. Área/archivos: `workflow/Webworkflow.aspx`, `workflow/Webworkflow.aspx.vb`, `workflow/Webworkflow.aspx.designer.vb`, `js/workflow/Webworkflow.js`. Origen: D-01, RQ-01. Verificación: la ruta no contiene `ImageButtonEnviarUsuario`, `Button_tool_enviar_usuario`, `After_envio_usuario_workflow` ni reasignación de respuesta.
- [x] 2.4 [M] Implementar modal y adaptador de usuario con debounce, historial de cursores, invalidación y respuesta obsoleta. Área/archivos: `workflow/Webworkflow.aspx`, `js/workflow/workflow-user-send-ui.js`. Origen: D-02, RQ-02. Verificación: preview usa JSON `same-origin` con sus cuatro campos y el archivo no contiene `IdConector` ni endpoint de Continuar flujo.
- [x] 2.5 [M] Implementar confirmación exclusiva con contrato usuario–actividad–token y diálogo genérico. Área/archivos: `js/workflow/workflow-user-send-confirmation.js`, `js/java_general/ConfirmationDialog.js`. Origen: D-03, RQ-03. Verificación: confirmar hace un POST a `EjecutarEnvioUsuario`; cancelar, bloqueo, error o invalidación no ejecutan ni disparan postback.
- [x] 2.6 [S] Integrar éxito correlacionado y actualización parcial sin estado compartido. Área/archivos: `workflow/Webworkflow.aspx`, `js/workflow/workflow-transition-page-presentation.js`, `js/workflow/workflow-user-send-confirmation.js`. Origen: D-04, RQ-04. Verificación: se quita la fila afectada, contador una vez y mensaje exclusivo de usuario.

## 3. Pruebas

- [x] 3.1 [M] Crear CJS para contrato, aislamiento, búsqueda, debounce, cursores, obsolescencia, vacío, error y selección. Área/archivos: `tests/workflow-user-send-ui.test.cjs`, `tests/workflow-user-send.test.cjs`. Origen: D-02, RQ-02. Verificación: `node --test tests/workflow-user-send-ui.test.cjs tests/workflow-user-send.test.cjs` pasa sin red.
- [x] 3.2 [M] Crear CJS para confirmación, doble clic, bloqueo, cancelación, teclado, foco, bootstrap y presentación parcial. Área/archivos: `tests/workflow-user-send-confirmation.test.cjs`, `tests/confirmation-dialog.test.cjs`, `tests/workflow-transition-page-presentation.test.cjs`. Origen: D-03, RQ-03. Verificación: la suite verifica ausencia de listeners, eventos y payload de Continuar flujo; durante un POST pendiente no permite cerrar, reemplazar ni recargar sin confirmación nativa.
- [x] 3.3 [S] Ejecutar compilación y batería focal de usuario, grupo, transición y gate. Área/archivos: `GestionDocumental-Docuarchi.net.sln`, `tests/workflow-*.test.cjs`. Origen: D-05, RQ-05. Verificación: MSBuild y Node focal terminan con código 0.

## 4. Evidencia y cierre

- [x] 4.1 [S] Actualizar el paquete técnico único con arquitectura, selectores, contrato, accesibilidad, seguridad, pruebas y relevo a etapa 03. Área/archivos: `Doc/Actualizacion/workflow/TerminarUsuario/01-implementacion-envio-usuario/`. Origen: D-05, RQ-05. Verificación: índice, cuatro documentos y diagramas reflejan DOC-29 sin duplicar contrato DOC-28.
- [x] 4.2 [S] Registrar evidencia y protocolo de QA visual; ejecutar E2E autenticado solo con autorización explícita. Área/archivos: `Doc/Actualizacion/workflow/TerminarUsuario/01-implementacion-envio-usuario/04-pruebas-y-evidencia.md`. Origen: D-05, RQ-05. Verificación: distingue pruebas locales de QA/E2E pendiente y no contiene secretos.
- [x] 4.3 [S] Validar trazabilidad OpenSpec y refinement antes de cierre. Área/archivos: `openspec/changes/doc-29-interfaz-moderna-enviar-usuario/`. Origen: D-01, RQ-01. Verificación: `opsxj:refine -- DOC-29 --sync` y `openspec.cmd validate doc-29-interfaz-moderna-enviar-usuario --strict` pasan.
