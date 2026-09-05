# Prompt backend 07 — Pruebas, integración local y evidencia autorizada

Valida el backend modernizado de los prompts 01 a 06 sin ampliar la autorización operativa ni depender inicialmente de servicios reales.

## Objetivo

Demostrar contratos, seguridad, compatibilidad, idempotencia, estados y reconciliación mediante una pirámide reproducible de pruebas.

## Rutas canónicas de implementación

```txt
Tests/
├── importar-servicio-web-architecture.test.cjs
├── importar-servicio-web-contract-suite.test.cjs
├── importar-servicio-web-gate-regression.test.cjs
├── importar-servicio-web-legacy-regression.test.cjs
└── importar-servicio-web-storage-invariance.test.cjs

tools/validation/
└── Verify-ImportarServicioWebModern.ps1

tools/e2e/tests/
└── importar-servicio-web-modern.spec.cjs
```

- Reutilizar las pruebas y fixtures focales ubicados por B01–B06; no copiarlos a otra carpeta.
- `Verify-ImportarServicioWebModern.ps1` realiza validación local determinista, no autenticada y sin red externa.
- El archivo E2E se crea o modifica únicamente como parte del cambio integral y reutiliza la infraestructura existente de `tools/e2e`.
- Crear o preparar la prueba E2E no autoriza ejecutarla.

No crear otro proyecto de pruebas, configuración Playwright, login, `.env`, arnés, carpeta `e2e/` paralela ni scripts de validación fuera de `tools/validation/`.

## Ruta documental obligatoria

```txt
docs/Architecture/Workflow/ImportarServicioWeb/SCRUMCORE-000-pruebas-backend-evidencia/
```

Sustituir `SCRUMCORE-000` por el ticket real. Crear `00-Indice.md` a `07-Metadata.md`, `Diagramas/` y `Evidencias/`. Esta última solo contiene evidencia saneada; no almacena credenciales, cookies, tokens, cadenas de conexión ni respuestas externas crudas.

## Implementa

- Pruebas focales de registro de proveedores, contexto inmutable, autorización, serialización, traducción legacy y mensajes seguros.
- Pruebas de transporte con servidor HTTP local simulado: token, consulta, descarga, timeout, cancelación, respuesta inválida, TLS/configuración y límites.
- Pruebas de preflight libre de efectos, persistencia de intención, unicidad, carreras e idempotencia.
- Pruebas de la máquina de estados y fallo inyectado antes y después de cada fase persistente.
- Pruebas de detención, resultado parcial, resultado incierto, reintento permitido/prohibido y reconciliación.
- Integración local aislada de documento, tarea, expediente, índices y caché.
- Regresión de ASMX y gate activado/desactivado sin retirar el recorrido anterior.
- Matriz de trazabilidad entre riesgos, requisitos, pruebas y evidencia saneada.
- Pruebas contractuales compartidas que validen las ocho operaciones, el mapeo de estados y los mismos fixtures consumidos por frontend.
- Pruebas que demuestren que `JSProgresBar` no inicia efectos de la ruta moderna y que el orquestador backend es su único ejecutor.
- Prueba de caracterización que verifique la reutilización de `AlmacenaDocumentoTareaWorkflow(...)` desde el adaptador nuevo.
- Verificación automática del diff que falle si se modifica el archivo que implementa `AlmacenaDocumentoTareaWorkflow(...)` o se sustituyen rutas legacy.

## Restricciones operativas

- No modificar código productivo existente como parte de este prompt de validación, especialmente `AlmacenaDocumentoTareaWorkflow(...)`.
- No realizar E2E real, carga, llamadas a SII, mutaciones de ambiente ni activación de gates sin autorización explícita para ambiente y cuentas.
- Antes de cualquier prueba autenticada de `PreviewEnviarTarea`, leer completo `tools/e2e/AGENT-RUNBOOK.md`.
- No guardar ni imprimir credenciales, cookies, tokens o cadenas de conexión.
- Toda consulta de control será exclusivamente `SELECT`.
- El preview no puede cambiar tarea, estado ni auditoría.
- Después de una corrida autorizada, `WorkflowCentroTrabajoModernActive` debe quedar en `false`, con usuarios y grupos vacíos.

## Aceptación

- Las pruebas focales y de integración local pasan sin red externa ni secretos.
- Existe evidencia de que dos solicitudes concurrentes no duplican intención ni documento.
- Cada punto de fallo produce el estado y la reconciliación definidos.
- Gate apagado conserva el recorrido legacy; gate encendido no duplica handlers ni operaciones.
- Gate apagado devuelve `FEATURE_DISABLED` sin efectos en cada endpoint moderno.
- La evidencia confirma que la implementación de `AlmacenaDocumentoTareaWorkflow(...)` permanece sin cambios y es reutilizada por la ruta nueva.
- La evidencia autorizada está saneada y registra restauración del gate.
- Cualquier prueba no ejecutada por falta de autorización queda declarada como pendiente, nunca simulada como aprobada.

## Entregable final

Informar archivos cambiados, comandos ejecutados, resultados, riesgos residuales, preguntas abiertas y pruebas no realizadas. Separar claramente evidencia local de evidencia obtenida en un ambiente autorizado.

## Trazabilidad

Exploración backend: secciones 17, 18, 19 y 20; `AGENTS.md`; `tools/e2e/AGENT-RUNBOOK.md` cuando corresponda.

## Correcciones opsxj:prompt-review

Estas reglas fueron agregadas desde `opsxj:prompt-review` para cubrir hallazgos estructurales corregibles. Deben ajustarse al contexto real del ticket antes de enviar a implementacion.

## Rol esperado
Definir el rol tecnico esperado para ejecutar el ticket.

## Objetivo
Describir el objetivo funcional y tecnico verificable.

## Restricciones criticas
- No introducir cambios fuera del alcance declarado.
- No romper comportamiento existente ni contratos publicos.

## Criterios de aceptacion
- El comportamiento implementado cumple el flujo esperado y queda validado con evidencia.

## Contexto obligatorio
Leer B01–B06, `AGENTS.md`, `tools/e2e/AGENT-RUNBOOK.md`, `tools/validation/Verify-Doc10Preview.ps1`, pruebas modernas de `Tests/` y la infraestructura actual de `tools/e2e`. El runbook no concede autorización para ejecutar E2E.

## Pruebas obligatorias
Ejecutar pruebas unitarias/focales, build/tsc segun impacto y E2E con Playwright cuando el flujo lo requiera; registrar comandos y resultados.

## Documentacion tecnica
Actualizar exclusivamente el paquete de **Ruta documental obligatoria**. Registrar por separado evidencia local y autorizada, comandos/resultados, cobertura, limitaciones, restauración del gate y verificaciones de invariancia.

## Entregable final
Entregar codigo, pruebas, documentacion, diagramas y evidencia coherente con lo realmente implementado.

Agregar regla para [ANTI_REGRESSION_DETAIL_REQUIRED]: Reglas explicitas de no romper, preservar, no llamar o no usar workarounds.

Exigir `npm run build` o `tsc` segun impacto y registrar el resultado.

Exigir pruebas unitarias/focales con Vitest o Testing Library segun el alcance.

Declarar que código + E2E + validación autorizada + evidencia saneada son una única unidad de entrega dentro del mismo cambio; no crear una tarea o entrega E2E independiente.

Reutilizar exclusivamente `tools/e2e`, su autenticación, configuración, validadores, evidencias y utilidades; prohibir login, arnés, proyecto Playwright, configuración o `.env` paralelos.

Antes de autenticar, exigir lectura de `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`; ejecutar solo con ambiente, cuentas y datos/tareas descartables expresamente autorizados.

Exigir secretos efímeros, prohibir exponer/imprimir/persistir credenciales, cookies, tokens y cadenas de conexión, usar verificaciones solo `SELECT` y conservar evidencia saneada.

Exigir cobertura E2E, cuando aplique, de autorización/control de acceso, lectura sin mutación, escrituras autorizadas, concurrencia y regresión relacionada.

Respetar feature flags, gates, usuarios, grupos y seguridad sin habilitarlos arbitrariamente; no cerrar sin validación autorizada, registrar bloqueo explícito y prohibir mocks, simulaciones, resultados inventados y evidencia ficticia.
