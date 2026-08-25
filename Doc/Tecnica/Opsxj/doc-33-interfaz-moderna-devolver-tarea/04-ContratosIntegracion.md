# INTERFAZ-MODERNA-DEVOLVER-TAREA

- Ticket: DOC-33
- Cambio OpenSpec: doc-33-interfaz-moderna-devolver-tarea
- Clasificacion: cross_cutting (Transversal)
## Contratos e integraciones

Los endpoints ASMX permanecen autenticados y JSON:

| Operación | Payload mínimo | Uso UI |
| --- | --- | --- |
| `PreviewDevolverActividad` | `idTarea`, `termino`, `cursor`, `tamanoPagina` | Lista destinos autorizados y entrega token/cursor. |
| `EjecutarDevolverActividad` | `idTarea`, `idConector`, `tokenVersion` | Revalida y ejecuta el destino seleccionado. |

La respuesta ASMX se desempaqueta desde `d`. La UI usa campos publicados del preview: conector, actividad, destinatario/grupo resumido, contexto, token y paginación. El contrato de ejecución no contiene actividad final, usuario, grupo, Ruta, Flujo, `Page` ni información de infraestructura.

No hay handlers nuevos de servidor, cambios de esquema o conexiones adicionales. La compatibilidad se conserva porque DOC-33 registra sus assets aparte y no altera los módulos de Enviar a usuario, Enviar a grupo ni Continuar flujo.
