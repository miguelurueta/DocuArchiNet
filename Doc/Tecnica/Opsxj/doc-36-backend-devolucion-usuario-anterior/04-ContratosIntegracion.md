# BACKEND-DEVOLUCION-USUARIO-ANTERIOR

- Ticket: DOC-36
- Cambio OpenSpec: doc-36-backend-devolucion-usuario-anterior
- Clasificacion: cross_cutting (Transversal)
## Contratos e integraciones

| Endpoint autenticado | Payload | Resultado | Efecto |
| --- | --- | --- | --- |
| `PreviewDevolverUsuarioAnterior` | `{ idTarea }` | `PrevisualizacionDevolverUsuarioAnteriorDto` | Solo lectura. |
| `EjecutarDevolverUsuarioAnterior` | `{ idTarea, tokenVersion }` | `ResultadoDevolverUsuarioAnteriorDto` | Devolución serializada y revalidada. |

El ASMX recompone sesión y autorización en servidor. No acepta usuario, actividad, grupo, Ruta, Flujo, conector ni identificador histórico. No cambia esquema ni expone secretos, SQL, controles Web Forms o excepciones internas.
