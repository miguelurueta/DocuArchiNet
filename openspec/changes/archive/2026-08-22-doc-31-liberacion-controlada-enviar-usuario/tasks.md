<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06 -->
## 1. Base de liberación

- [x] 1.1 [S] Registrar la decisión operativa y la versión de referencia integrada. Área/archivos: `Doc/Actualizacion/workflow/TerminarUsuario/04-liberacion-controlada-doc-31/00-indice.md`. Origen: D-01, RQ-01. Verificación: el documento declara `solicitar aprobación operativa` y el merge de referencia sin autorizar despliegue.
- [x] 1.2 [M] Crear la matriz de ambientes sin secretos, con cero ambientes elegibles hasta recibir aprobación nominativa. Área/archivos: `Doc/Actualizacion/workflow/TerminarUsuario/04-liberacion-controlada-doc-31/02-matriz-ambientes.md`. Origen: D-02, RQ-02. Verificación: cada campo operativo requerido está definido y no hay ambiente, ventana ni responsable inferido.

## 2. Runbook y compatibilidad

- [x] 2.1 [M] Documentar el runbook de verificación autorizada, abortar y reversión por paquete. Área/archivos: `Doc/Actualizacion/workflow/TerminarUsuario/04-liberacion-controlada-doc-31/03-runbook-operativo.md`. Origen: D-04, RQ-03. Verificación: solo permite evidencia documental o `SELECT`, excluye secretos y conserva tareas ya confirmadas.
- [x] 2.2 [S] Documentar el límite de reversión por paquete para intentos nuevos. Área/archivos: `Doc/Actualizacion/workflow/TerminarUsuario/04-liberacion-controlada-doc-31/03-runbook-operativo.md`. Origen: D-05, RQ-03. Verificación: prohíbe revertir tareas, respuestas y auditoría ya confirmadas.
- [x] 2.3 [S] Documentar invariantes de ruta moderna, respuesta y transición por conector. Área/archivos: `Doc/Actualizacion/workflow/TerminarUsuario/04-liberacion-controlada-doc-31/04-compatibilidad-y-riesgos.md`. Origen: D-06, RQ-04. Verificación: declara ausencia de fallback usuario y preservación de `IdConector` en Continuar flujo.

## 3. Validación documental

- [x] 3.1 [S] Corroborar documentalmente evidencia DOC-30, versión integrada y ausencia de autorización de ambiente. Área/archivos: `Doc/Actualizacion/workflow/TerminarUsuario/03-verificacion-transversal-doc-30/` y `04-liberacion-controlada-doc-31/`. Origen: D-01, RQ-01. Verificación: referencias cruzadas consistentes y sin comandos de ambiente ejecutados.
- [x] 3.2 [S] Validar OpenSpec, gobierno OPSXJ y documentación de liberación. Área/archivos: `openspec/changes/doc-31-liberacion-controlada-enviar-usuario/` y `Doc/Tecnica/Opsxj/doc-31-liberacion-controlada-enviar-usuario/`. Origen: D-03, RQ-02. Verificación: validación estricta y de gobierno aprobadas.
