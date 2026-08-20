# BUSQUEDA-ESCALABLE-ENVIAR-GRUPOS

- Ticket: DOC-26
- Cambio OpenSpec: doc-26-busqueda-escalable-enviar-grupos
- Clasificacion: cross_cutting

## Superficies UI

El modal moderno de workflow/Webworkflow.aspx incorpora etiqueta visible Buscar actividad o grupo, ayuda de longitud mínima, paginador y estado aria-live. El script comparte un único modelo de destinos para la tabla de escritorio y las tarjetas móviles. El CSS usa el breakpoint existente de 767 px: oculta la tabla y muestra tarjetas, y coloca los controles de página en una cuadrícula táctil.

La búsqueda espera 300 ms, cancela o descarta respuestas previas, vuelve a la primera página al limpiar e invalida una confirmación basada en resultados anteriores. Se conservan Escape, trampa de foco, retorno al trigger, navegación por teclado y prevención de doble clic. Tras un envío confirmado, el presentador cierra el contexto de tarea, vuelve a mostrar la bandeja, restablece su scroll horizontal y ejecuta el recálculo de altura existente.

## Validacion visual

La verificación automatizada CJS comprueba los selectores del campo, los botones de página, la representación compartida tabla/tarjetas, las reglas responsive de 767 px y, tras éxito, el cierre del contexto, scroll horizontal inicial y recálculo de la bandeja. Una validación manual autenticada de esta regresión permanece pendiente: el entorno actual no tiene secretos E2E efímeros configurados.
