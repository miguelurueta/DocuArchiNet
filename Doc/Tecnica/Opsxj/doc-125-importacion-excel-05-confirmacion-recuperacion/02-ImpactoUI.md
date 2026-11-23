# DOC-125 — Confirmación y recuperación

- Ticket: DOC-125
- Cambio OpenSpec: doc-125-importacion-excel-05-confirmacion-recuperacion
- Clasificacion: cross_cutting (Transversal)
- Estado: refinamiento técnico, sin implementación ni E2E escritora ejecutada.

## Superficies UI

Esta fase entrega métodos Confirmar y ConsultarResultado en el ASMX existente y amplía la respuesta segura con conteo/UTC. No crea pantalla, menú o código de vista; fase06 consumirá estos contratos. La recuperación conserva el ID de operación, sin enviar tablas, filas ni conexiones desde el navegador.

## Validacion visual

No aplica validación visual nueva a este refinamiento/backend. Las pruebas de transporte, CSRF, autoridad y replay se incorporarán a los harness y E2E existentes. No se presenta una llamada sintética o el CI de OpenSpec como una interfaz terminada.
