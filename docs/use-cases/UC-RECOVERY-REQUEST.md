# UC-RECOVERY-REQUEST

## Objetivo
Permitir que un usuario sin sesión inicie el proceso de recuperación de contraseña.

## Actores
- Usuario sin sesión.
- API de recuperación.

## Flujo principal
1. El usuario abre la ruta pública de recuperación.
2. Informa el identificador (usuario/email corporativo según configuración).
3. Envía el formulario.
4. La SPA invoca el contrato de recovery.
5. Si la respuesta es funcional, navega a verificación OTP sin recargar.

## Flujos alternos
- Identificador vacío: se bloquea submit y se muestra validación UI.
- Error técnico API: se notifica error y se desbloquea operación.
