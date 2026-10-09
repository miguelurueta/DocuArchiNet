# DOBLE-FACTOR-CONTRATO

- Ticket: DOC-91
- Cambio OpenSpec: doc-91-doble-factor-contrato
- Clasificacion: cross_cutting

## Servicios y reglas

No se crea una capa `Services` en DOC-91. Se definen puertos para reloj, OTP, HMAC, llaves, configuración, challenge, destinatario, correo, finalización legacy y Session; únicamente reloj, OTP, HMAC, proveedor de llaves y Session reciben implementación. La política fija LOGIN, EMAIL=1, 5 intentos, cooldown de 60 segundos, 2 reenvíos y expiración de 1 a 10 minutos.

No existe implementación de repositorio, SMTP o finalizador. El inventario de firmas y la relación interfaz–implementación están en `Doc/Actualizacion/Login/Implementacion/DOC-91/03-INVENTARIO-TECNICO.md`.
