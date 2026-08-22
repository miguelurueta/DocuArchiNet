# Contratos, endpoints y códigos — Lista preview

- Ticket: DOC-29
- Cambio OpenSpec: doc-29-interfaz-moderna-enviar-usuario
- Clasificacion: cross_cutting

## Endpoints y contratos

La lista invoca `PreviewEnviarUsuario` mediante `POST` JSON y `credentials: "same-origin"`:

```json
{ "idTarea": 821, "consulta": "ana", "cursor": "", "tamanoPagina": 25 }
```

La respuesta contiene contexto mínimo, `tokenVersion`, `destinos`, `cursorSiguiente`, `tieneMas` y el tamaño de página. Cada destino expone `idUsuarioWorkflowDestino`, `idActividadDestino`, nombre, cargo y actividad. El cursor es opaco y el token se conserva para la confirmación; ninguno representa autorización delegada al navegador.

## Compatibilidad de integración

La consulta no acepta ni devuelve `IdConector`, `Page`, `Session`, identidad, permisos, SQL o controles Web Forms. Un término, cursor, tarea o contexto inválidos reciben un error funcional sanitizado. La página renderiza únicamente `destinos` retornados, no una lista completa cargada en cliente.

Los botones de paginación usan el cursor anterior almacenado para **Anterior** y `cursorSiguiente` para **Siguiente**. La confirmación posterior usa otro endpoint y solo la terna directa usuario–actividad–token.
