# UC-OTP-VERIFY

## Objetivo
Validar segundo factor OTP para completar autenticación.

## Actores
- Usuario
- Frontend SPA
- Servicio OTP

## Precondiciones
- Usuario llega con challenge OTP vigente.
- Código OTP de 6 dígitos.

## Flujo principal
1. Usuario ingresa OTP y confirma.
2. Frontend invoca `seVerificarSegundoFactor`.
3. Si éxito, persiste token y navega a `/dashboard`.

## Flujos alternos
- A1: OTP inválido, se muestra error funcional y no navega.
- A2: OTP expirado por timer, se muestra modal de expiración y se redirige/reintenta.

## Postcondiciones
- Sesión activa o usuario vuelve al inicio del flujo.
