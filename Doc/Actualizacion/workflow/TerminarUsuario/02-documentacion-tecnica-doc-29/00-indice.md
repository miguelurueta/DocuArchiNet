# DOC-29 — Documentación técnica de Enviar a usuario

Índice raíz de la documentación técnica DOC-29. La documentación se divide por componente para que cada responsabilidad tenga su propio paquete, estructura homogénea y diagramas sin contenido duplicado.

- Ticket: DOC-29
- Cambio OpenSpec: doc-29-interfaz-moderna-enviar-usuario
- Clasificación: cross_cutting

- [Interfaz moderna de Enviar a usuario](01-interfaz-moderna-envio-usuario/00-indice.md)
- [Lista preview paginada de usuarios](02-lista-preview-paginada/00-indice.md)
- [Verificación transversal posterior (DOC-30)](../03-verificacion-transversal-doc-30/00-indice.md)

Estado: validación local y QA visual por grabación completos. Se revisaron fotogramas distribuidos de la grabación que cubren apertura, búsqueda, estado transitorio, selección, recarga y cierre; el diálogo conserva su geometría. El preview E2E autenticado preservó las huellas de estado y auditoría, y la corrida E2E UI integral aprobada confirmó un único envío `completada`, bloqueo de cierre/recarga durante el proceso y cambios de estado y auditoría. DOC-30 verificó posteriormente el snapshot sin nuevas operaciones autenticadas y emitió dictamen apto para solicitar aprobación operativa. No se activó el gate, ni se ejecutó carga o concurrencia mutante.
