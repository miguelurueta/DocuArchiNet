# UC-AUTH-TOKEN-EXPIRATION

## Objetivo
Detectar expiración de token y ejecutar logout controlado.

## Actores
- TokenWatcher
- Usuario autenticado
- Infraestructura JWT

## Precondiciones
- Usuario en ruta restringida.
- Token registrado en localStorage.

## Flujo principal
1. TokenWatcher ejecuta validación periódica.
2. Si token expiró y estrategia es redirect, muestra aviso de expiración.
3. Tras delay configurado, ejecuta `finalizarSesionYRedirigir`.

## Flujo alterno
- A1: Estrategia renew intenta renovar token y refrescar claims.
- A2: Si no hay token registrado, no interfiere UI.

## Postcondiciones
- Usuario logout y redirigido, o sesión renovada.
