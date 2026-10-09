# DOC-92 — Pruebas y evidencia

- Ticket: DOC-92
- Cambio OpenSpec: doc-92-segundo-factor-persistencia
- Clasificacion: cross_cutting

## Evidencia requerida

La validación debe cubrir compilación, firmas heredadas de DOC-91, uso exclusivo de las diez columnas existentes, SQL parametrizado, incremento atómico de intentos, ganador único por `Consumed`, rollback del reemplazo y ausencia total de scripts DDL.

Los comandos previstos son:

- `node --test tests/login-second-factor-persistence.test.cjs tests/login-second-factor-foundation.test.cjs`
- `npm.cmd --prefix tools/e2e run test:doc92:persistence`
- validación estricta de OpenSpec y refinamiento OPSXJ.

Resultados del 2026-10-09:

| Comando | Código | Resultado |
| --- | ---: | --- |
| `node --test tests/login-second-factor-persistence.test.cjs tests/login-second-factor-foundation.test.cjs` | 0 | 6 aprobadas, 0 fallidas; incluye MSBuild y dobles transaccionales. |
| `npm.cmd --prefix tools/e2e run test:doc92:persistence` | 0 | 1 omitida (`SKIP`) por falta de autorización MySQL vigente. |
| `npm.cmd --prefix tools/opsxj run opsxj:refine -- DOC-92 --sync` | 0 | Refinamiento aprobado y trazabilidad completa. |
| `openspec.cmd validate doc-92-segundo-factor-persistencia --strict` | 0 | Cambio OpenSpec válido. |

La prueba MySQL crea la tabla exacta únicamente dentro de una base descartable cuyo nombre empieza por `doc92_`; sin autorización vigente se omite de forma segura. El `SKIP` no demuestra concurrencia real de MySQL.

## QA/E2E WebForms

No existe recorrido E2E WebForms en DOC-92 porque el repositorio todavía no está conectado al login productivo. La prueba de integración es de persistencia y concurrencia, no una prueba de interfaz.

La ejecución contra un ambiente real no está autorizada en este turno. No se ejecutarán DDL ni consultas de escritura sobre bases compartidas; la evidencia real anterior de `information_schema` solo confirma que la tabla y sus diez columnas ya existen.
