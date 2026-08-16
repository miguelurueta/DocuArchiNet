# LISTA-MODERNA-DESTINO

- Ticket: DOC-12
- Cambio OpenSpec: doc-12-lista-moderna-destino
- Clasificacion: cross_cutting (Transversal)
## Superficies UI

- `#workflow-transition-trigger` conserva la acción legacy y recibe el atributo de activación emitido por servidor.
- Con gate activo se carga un host modal independiente, sin `UpdatePanel`, para estados cargando, sin destinos, error controlado, lista disponible y destino seleccionado.
- En escritorio se usa tabla compacta; bajo 768 px se usan tarjetas. Solo se presentan radicado, tipo, grupo actual, nombre, destinatario/grupo y tipo de destino publicados por el DTO.
- El diálogo incluye foco inicial, Escape, Tab atrapado, retorno de foco, región viva y foco visible de alto contraste.

## Validacion visual

La verificación estática y unitaria está aprobada. Falta el recorrido manual autorizado en escritorio y móvil para capturar foco, Escape, teclado, contraste, visualización de tabla/tarjetas y el comportamiento con gate activo e inactivo. Esta condición no habilita ninguna transición ni modifica el recorrido legacy.
