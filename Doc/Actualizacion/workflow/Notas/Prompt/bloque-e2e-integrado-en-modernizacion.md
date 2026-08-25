# Requisito obligatorio — E2E integrada al prompt principal

Copiar este bloque dentro de todo prompt cuyo alcance introduzca o modifique un recorrido E2E. La E2E es parte del mismo cambio y no una tarea, historia ni entrega independiente.

## Rol esperado

Arquitecto y desarrollador senior con experiencia en el flujo funcional afectado, automatización E2E y los controles de seguridad del repositorio.

## Objetivo

Integrar y validar la cobertura E2E real aplicable dentro del mismo cambio funcional, con evidencia saneada y sin ampliar el alcance autorizado.

## Contexto

Antes de diseñar la cobertura, revisar `AGENTS.md`, `tools/e2e/AGENT-RUNBOOK.md`, `tools/e2e/package.json`, la sesión autenticada, configuración, validadores y evidencias existentes. Ubicar pruebas, scripts y validadores solamente bajo las rutas existentes de `tools/e2e/`.

## Restricciones críticas

- No crear login, arnés, proyecto Playwright, configuración ni `.env` paralelo.
- No ejecutar pruebas reales, escrituras, carga ni cambios de gate sin ambiente, cuentas, datos/tareas descartables y autorización explícitos.
- No imprimir, exponer ni persistir secretos, cookies, tokens, cadenas de conexión, información sensible o respuestas sin sanear; las consultas de verificación son solo `SELECT`.
- No habilitar feature flags, gates, usuarios, grupos ni controles de seguridad arbitrariamente para facilitar una prueba.
- Reglas de anti-regresión: preservar los flujos existentes no afectados, reutilizar la infraestructura aprobada y no reemplazar la E2E por mocks, simulaciones, resultados inventados ni evidencia ficticia.

## Pruebas obligatorias

Además de la E2E aplicable, ejecutar pruebas unitarias o focales y el build, `tsc`, MSBuild o `dotnet` que correspondan al cambio. Registrar comandos, resultados, limitaciones y evidencia saneada. La E2E cubre, cuando aplique, autorización/control de acceso, lectura sin mutación, escrituras autorizadas, concurrencia y regresión relacionada.

## Documentación técnica

Actualizar la matriz de pruebas, la propuesta OpenSpec y la documentación existente del cambio con cobertura E2E, autorizaciones requeridas, comandos, resultados, bloqueos, riesgos y rollback. Si falta una ruta documental, registrar la ausencia; no crear documentación en la raíz.

## Criterios de aceptación

- La E2E aplicable está integrada en el mismo cambio funcional y reutiliza exclusivamente la infraestructura existente.
- Las validaciones autorizadas producen evidencia saneada; si falta autorización, ambiente, configuración o datos, el bloqueo queda explícito.
- No se altera arbitrariamente la configuración de seguridad y no se sustituye una E2E requerida por evidencia no real.

## Entregable final

Entregar código, pruebas unitarias/focales y E2E, comandos y resultados de build/validación, evidencia saneada, riesgos, bloqueos y rollback como una única unidad de entrega.

```text
Incluye las pruebas E2E reales como criterio de cierre de esta implementación. Código + pruebas E2E + validación autorizada + evidencia saneada constituyen una única unidad de entrega. Reutiliza exclusivamente la infraestructura existente en `tools/e2e`: sesión autenticada, configuración, validadores, evidencias y utilidades. No crees login, arnés, proyecto Playwright, configuración ni archivo `.env` paralelo.

Antes de diseñar o ejecutar una E2E autenticada, lee `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`. Ejecuta únicamente contra un ambiente, cuentas y datos/tareas descartables expresamente autorizados. Usa secretos efímeros, preferiblemente capturados de manera oculta por el mismo comando en la consola de ejecución, consultas de verificación solo `SELECT` y evidencia saneada; nunca imprimas, expongas ni persistas credenciales, cookies, tokens, cadenas de conexión, información sensible o respuestas sin sanear.

La suite debe cubrir los casos reales que correspondan al alcance implementado: autorización y control de acceso, lectura sin mutación, escrituras autorizadas, concurrencia cuando aplique y regresión de funcionalidades relacionadas. Cuando el repositorio ya disponga de una ejecución E2E oficial del flujo —como DOC-32— reutilízala como único ejecutor de transición; las pruebas de contratos o CRUD complementan la cobertura, pero no crean ni sustituyen esa ejecución. Mantén las protecciones de configuración, feature flags, gates, usuarios, grupos y controles de seguridad en su estado requerido; nunca los habilites arbitrariamente para facilitar una prueba. Toda evidencia debe estar saneada y libre de información sensible.

La implementación no se considera terminada hasta integrar la suite al mismo cambio, ejecutar las validaciones autorizadas y registrar los resultados o un bloqueo explícito de ambiente, configuración, datos o autorización. No reemplaces una E2E requerida por mocks, simulaciones, resultados inventados ni evidencia ficticia.
```
