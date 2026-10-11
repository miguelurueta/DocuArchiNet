# DOBLE-FACTOR-ORQUESTACION

- Ticket: DOC-95
- Cambio OpenSpec: doc-95-doble-factor-orquestacion
- Clasificacion: cross_cutting (Transversal)
## Superficies UI

- DOC-95 no agrega ni modifica páginas WebForms, UserControls, modales, tablas visuales, estilos o scripts de interfaz.
- La frontera disponible para la interfaz es `WebServiceLoginSegundoFactor.asmx`, con las operaciones JSON `ObtenerEstado`, `Verificar`, `Reenviar` y `Cancelar`.
- Los DTO exponen únicamente estado público, destino enmascarado, tiempos restantes, código/mensaje seguro y ruta local. La UI no recibe identidad, correo completo, challenge, nonce, HMAC ni excepciones.
- La construcción visual de la pantalla de captura de OTP permanece fuera del alcance de DOC-95 y no se declara implementada.

## Validacion visual

No aplica una comparación visual porque no hubo cambios de presentación. La validación manual se realizó sobre el recorrido HTTP autenticado: postback del login, estado pendiente, destino enmascarado, verificación concurrente con un ganador y emisión de cookie únicamente después de completar el segundo factor. La evidencia saneada está registrada en `05-PruebasEvidencia.md`.
