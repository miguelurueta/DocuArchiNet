# UC-RESET-PASSWORD

## Objetivo
Permitir que un usuario complete el cambio de contraseña con token de recuperación.

## Actores
- Usuario sin sesión.
- API de reset.

## Flujo principal
1. El usuario llega al formulario de nueva contraseña con token válido.
2. Informa nueva contraseña y confirmación.
3. Envía formulario.
4. La SPA invoca reset-password.
5. Ante éxito, navega al login sin recargar.

## Flujos alternos
- Token inválido/expirado/reutilizado: se notifica error y no navega.
- Contraseñas vacías o distintas: validación UI bloquea el submit.
