# UC-AUTH-LOGIN

## Objetivo
Permitir al usuario autenticarse por login y acceder al dashboard o al flujo OTP.

## Actores
- Usuario
- Frontend SPA
- Servicio de autenticación

## Precondiciones
- Usuario tiene módulo seleccionado.
- Credenciales cargadas.

## Flujo principal
1. Usuario envía formulario de login.
2. Frontend invoca `seLoginUsuario`.
3. Si la respuesta es éxito, persiste sesión y navega a `/dashboard`.

## Flujos alternos
- A1: Si backend responde `SECOND_FACTOR_REQUIRED`, frontend navega a `/verificar-otp`.
- A2: Si backend responde error funcional/técnico, se notifica mediante `useAxiosErrorNotifier`.

## Postcondiciones
- Sesión activa en localStorage o paso a OTP.
