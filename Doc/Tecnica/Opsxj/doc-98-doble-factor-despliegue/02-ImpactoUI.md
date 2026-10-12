# DOBLE-FACTOR-DESPLIEGUE

- Ticket: DOC-98
- Cambio OpenSpec: doc-98-doble-factor-despliegue
- Clasificacion: cross_cutting (Transversal)
## Superficies UI

- DOC-98 no cambia paginas Web Forms, JavaScript, CSS, modales ni controles.
- La UI 2FA existente de DOC-96 se conserva: pendiente, destino enmascarado, verificar, reenviar y cancelar.
- La activacion no se representa ni se decide en UI por usuario; se resuelve transversalmente por empresa/modulo antes de presentar el challenge.

## Validacion visual

No aplica una nueva validacion visual porque no hay cambios UI. En una operacion autorizada se reutiliza el recorrido E2E DOC-96 y se registra unicamente su resultado saneado.
