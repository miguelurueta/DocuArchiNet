# DOC-73 — Impacto UI

- Ticket: DOC-73
- Cambio: doc-73-adaptador-interfaz-integracion-sii
- Impacto: cross_cutting

## Superficies UI

El contenedor genérico DOC-72 delega el render al adaptador cuando el proveedor es `INTEGRACIONSII`. La tabla muestra libro, inscripción, fecha, acto/naturaleza, noticia y referencia; usa `textContent` y deshabilita selección no importable.

## Validacion visual

Se verificaron por pruebas la semántica del diálogo, foco, anuncios y ausencia de progreso ficticio. No se hizo E2E autenticado ni consulta SII real por falta de autorización ambiental.

