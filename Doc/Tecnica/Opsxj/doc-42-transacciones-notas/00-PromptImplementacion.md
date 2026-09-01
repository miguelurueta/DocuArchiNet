# DOC-42 — Prompt de implementación

## ROL ESPERADO

Arquitecto y desarrollador senior ASP.NET Web Forms/VB.NET, MySQL 5.1 y Playwright E2E.

## CONTEXTO

Implementar escrituras modernas de Notas Workflow reutilizando los contratos y la plataforma E2E existentes.

## OBJETIVO

Entregar operaciones de notas seguras, idempotentes, transaccionales y verificables.

## RESTRICCIONES CRÍTICAS

- No modificar WebForms, consumidores legacy ni gates.
- No ejecutar E2E real, migraciones ni escrituras de ambiente sin autorización explícita.
- No imprimir ni persistir credenciales, cookies, tokens o cadenas de conexión.
- Usar autorización atómica, SQL parametrizado, idempotencia y auditoría sin contenido completo.

## CRITERIOS DE ACEPTACIÓN

- Crear es idempotente por tarea, autor y UUID de cliente.
- Actualizar/eliminar condicionan tarea, nota, propietario, estado y ETag SHA-256.
- Fallos revierten nota, idempotencia y auditoría en una única transacción.
- Preflight incompatible devuelve `Unavailable` sin escribir.

## PRUEBAS OBLIGATORIAS

Ejecutar pruebas locales de contratos, servicio, repositorio y plataforma E2E. Registrar comandos y resultados saneados; la E2E real queda condicionada a autorización independiente.

La E2E es parte integral del mismo cambio y su cierre. Reutilizar exclusivamente la infraestructura existente de `tools/e2e/E2E-TEST/` (sin login, arnés, proyecto ni configuración paralelos), leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`, y exigir autorización explícita de ambiente, cuentas y datos descartables. Usar secretos efímeros: no imprimir ni persistir credenciales, cookies, tokens o cadenas de conexión; usar solo consultas `SELECT` y evidencia saneada. La cobertura verificable incluye autorización, creación idempotente, conflicto de versión, concurrencia, regresión y auditoría. Proteger configuración, dejar gates/usuarios/grupos sin cambios y registrar bloqueo explícito si falta autorización; no simular resultados.
No crear login, arnés, Playwright, configuración ni `.env` paralelos; reutilizar autenticación, validadores, evidencias y utilidades existentes.
Cubrir, cuando aplique, autorización/control de acceso, lectura sin mutación, escrituras autorizadas, concurrencia y regresión relacionada.
Respetar feature flags, gates, usuarios y grupos sin habilitarlos arbitrariamente; no se considera terminada la entrega sin validación autorizada y registrar bloqueo explícito sin mocks, simulaciones ni evidencia ficticia.

## DETALLE DE CONTRATOS Y FLUJO

Documentar cada contrato request/response tipado, campos requeridos y códigos seguros para cada endpoint. El flujo es: validar contexto y tarea; ejecutar preflight; reservar UUID; mutar con predicados atómicos; auditar en la misma transacción; confirmar o revertir; devolver versión sin exponer contenido indebido. Mantener compatibilidad con lecturas DOC-41, arnés legacy, gates y consumidores existentes; cualquier regresión debe bloquear el cierre.

## EVIDENCIA DE COMPILACIÓN

Ejecutar MSBuild o `dotnet` cuando se modifique VB.NET y registrar comando, resultado y referencia saneada.

## DOCUMENTACIÓN TÉCNICA

Actualizar OpenSpec y `Doc/Actualizacion/workflow/Notas/`; conservar el arnés legado y reutilizar `tools/e2e/E2E-TEST/`.

## UBICACIÓN DE CÓDIGO

DTOs/modelos/interfaces en `DTOs/` y `Services/Workflow/`; repositorios en `Infrastructure/Repositories/Workflow/`; ASMX en `webservice/`; pruebas en `tests/` y `tools/e2e/tests/`.

## ENTREGABLE FINAL

Código, migración reversible no aplicada, pruebas locales, evidencia saneada y registro explícito de cualquier bloqueo de ambiente.
