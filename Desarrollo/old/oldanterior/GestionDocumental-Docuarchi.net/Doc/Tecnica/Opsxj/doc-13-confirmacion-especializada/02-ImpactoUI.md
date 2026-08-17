# CONFIRMACION-ESPECIALIZADA

- Ticket: DOC-13
- Cambio OpenSpec: doc-13-confirmacion-especializada
- Clasificacion: cross_cutting (Transversal)
## Superficies UI

- `workflow/Webworkflow.aspx` carga los estilos y scripts de la confirmación solo para el piloto moderno.
- El selector de destinos sigue siendo el primer paso. Al elegir una fila, el adaptador abre la confirmación y conserva detrás el selector como contexto visual bloqueado.
- El diálogo expone únicamente radicado, tipo, grupo actual, destino, destinatario o grupo y mecanismo disponibles en el preview. No infiere trámite ni actividad adicional.
- Durante el envío bloquea el botón principal, Cancelar, la X, Escape, fondo y apertura de otra confirmación. Tras éxito cierra, limpia el contexto correlacionado y muestra un aviso temporal.
- Ante bloqueo funcional o error técnico, conserva la confirmación, la selección y las acciones para que el usuario pueda leer el mensaje y reintentar cuando aplique.
- Usa foco inicial en el diálogo, trampa de Tab y Shift+Tab, retorno al disparador, roles y anuncios de estado. En 390 × 894 y 321 × 568 el contenido y las acciones se mantienen visibles y alcanzables.

## Validacion visual

El recorrido manual autorizado del 2026-08-17 verificó cancelación, Escape, foco, navegación por teclado, protección durante envío, éxito, error seguro, aviso temporal, escritorio y móvil. La evidencia detallada está en `Doc/Actualizacion/workflow/Terminar/05-confirmacion-especializada/04-pruebas-y-evidencia.md`.
