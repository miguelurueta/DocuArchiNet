# Flujo, seguridad y accesibilidad

## Estados

- Carga: mensaje anunciado mediante `role=status`.
- Vacío: explicación y acción Nueva nota disponible.
- Éxito: confirmación de creación, actualización o eliminación.
- Error: mensaje funcional y botón Reintentar para lectura.
- Conflicto: informa que la versión cambió y recarga el estado vigente.

## Controles

El editor usa `role=dialog`, `aria-modal`, título asociado, cierre, cancelación y Escape. El foco entra al texto y vuelve al control que abrió el diálogo. Botones y acciones tienen un objetivo mínimo de 40 px y foco visible.

## Seguridad

Las credenciales son efímeras en TTY. No se persisten cookies, contraseñas ni conexiones. La autorización y la propiedad de cada nota se validan en servidor. El cliente bloquea reenvíos mientras existe una solicitud activa.
