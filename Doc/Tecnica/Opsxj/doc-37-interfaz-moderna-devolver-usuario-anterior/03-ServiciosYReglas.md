# Servicios y reglas — Devolver a usuario anterior

- Ticket: DOC-37
- Cambio OpenSpec: doc-37-interfaz-moderna-devolver-usuario-anterior
- Clasificación: cross_cutting

## Servicios y reglas

DOC-37 no modifica las capas VB.NET de DOC-36. `workflow-return-user-previous-ui.js` consume `PreviewDevolverUsuarioAnterior` para obtener actividad histórica, usuario histórico y token opaco; su integración de confirmación consume `EjecutarDevolverUsuarioAnterior` para efectuar la mutación validada en servidor. Historial ausente, grupo, usuario no disponible, auto-devolución, token vencido, conflicto y lock ocupado se tratan como bloqueos funcionales. El navegador no autoriza, no recibe identificadores de historial y no construye consultas SQL, handlers Web Forms ni componentes de respuestas.
