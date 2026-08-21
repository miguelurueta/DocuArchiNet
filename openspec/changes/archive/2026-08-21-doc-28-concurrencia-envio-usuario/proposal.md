## Why

El envío directo a usuario ya revalida bajo `GET_LOCK`, pero la evidencia E2E DOC-28 no demuestra que dos solicitudes simultáneas sobre la misma tarea no dupliquen la transición. La concurrencia es crítica para proteger la reasignación de una tarea descartable en condiciones reales de doble clic, reintento o solicitudes paralelas.

## What Changes

- Incorporar una carrera E2E DOC-28 de exactamente dos solicitudes de envío, construidas con el mismo destino y token obtenidos de un preview vigente.
- Exigir autorización explícita adicional para concurrencia, tarea descartable, controles MySQL de solo lectura y evidencia saneada.
- Validar que una única solicitud complete el envío, la otra se bloquee con un código funcional permitido y no exista una segunda transición.
- Documentar el cierre de gate/legacy y los límites: no se incorpora carga masiva ni se almacenan secretos, cookies, token, destino o cuerpos de respuesta.

## Capabilities

### New Capabilities

- `e2e-concurrencia-enviar-usuario-workflow`: Carrera E2E segura y autorizada que verifica la exclusión mutua de `EjecutarEnvioUsuario`.

### Modified Capabilities

- Ninguna.

## Impact

- `tools/e2e/scripts/`, `tools/e2e/package.json`, `tools/e2e/tests/`, pruebas estáticas y documentación operativa DOC-28.
- No se alteran los contratos ASMX, el motor legacy, la configuración del gate ni los flujos de usuario.
- La ejecución real exige una autorización específica de concurrencia; la implementación y sus pruebas locales no generan solicitudes autenticadas ni modificaciones de datos.
