# DOC-83 — Contratos e integraciones

- Ticket: DOC-83
- Cambio OpenSpec: doc-83-pruebas-servicio-sii-enlace
- Clasificacion: cross_cutting

## Contratos e integraciones

DOC-83 no agrega endpoint, DTO, esquema o proveedor. Reutiliza los escenarios registrados:

| Escenario | Etapa | Recurso | Autorización mínima |
|---|---|---|---|
| `import-sii-enlase-anonymous` | anonymous | sin sesión, tarea, controles ni secretos | `environment,gate` |
| `import-sii-enlase-read` | read | tarea no mutadora | `environment,gate` |
| `import-sii-enlase-ui` | read | tarea no mutadora | `environment,gate` |
| `import-sii-enlase-execution` | execution | tarea descartable mutadora | `environment,gate,execution,discardable-resource` |

`local-tls` solo se incorpora cuando el perfil solicita ignorar el certificado local. Los perfiles no pueden contener secretos, SQL libre, conexiones o selección de scripts. Los controles proceden exclusivamente de `CONTROL_REGISTRY` y son `SELECT` parametrizados.

La evidencia se reduce al esquema seguro de la plataforma. Integridad de gate, archivos legacy y evidencia son condiciones obligatorias de salida.
