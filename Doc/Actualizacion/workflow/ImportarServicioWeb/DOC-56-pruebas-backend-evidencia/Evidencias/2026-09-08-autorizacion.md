# Registro saneado de autorización

- Fecha: 2026-09-08, zona America/Bogota.
- Alcance autorizado explícitamente en la sesión: ambiente de certificación, cuenta Workflow de prueba, tarea 219887, dato SII indicado por el solicitante, activación temporal del gate y TLS local.
- Alcance E2E: lectura real; ejecución y concurrencia únicamente mediante los perfiles registrados y controles de recurso descartable.
- Custodia: las credenciales se capturan exclusivamente en TTY interactiva y no forman parte de esta evidencia.
- Restauración obligatoria: gate `false`, usuarios y grupos vacíos al terminar, incluso ante fallo.
