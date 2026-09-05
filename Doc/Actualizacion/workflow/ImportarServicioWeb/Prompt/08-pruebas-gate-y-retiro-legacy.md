# Prompt 08 — Pruebas, gate y retiro legacy

Completa la transición reversible y prepara el retiro de controles antiguos sin ampliar la autorización de pruebas.

Depende de B07 y de F01–F07. Aplica el gate canónico `WorkflowCentroTrabajoModernActive` según `../CONTRATO-COMPARTIDO-FRONTEND-BACKEND.md`.

## Objetivo

Demostrar que la experiencia moderna funciona bajo gate, que el recorrido anterior sigue disponible al apagarlo y que los controles legacy solo se eliminan después de evidencia suficiente.

## Rutas canónicas de implementación

```txt
Tests/
├── importar-servicio-web-ui-architecture.test.cjs
├── importar-servicio-web-gate.test.cjs
├── importar-servicio-web-legacy-ui-regression.test.cjs
└── importar-servicio-web-storage-invariance-ui.test.cjs

tools/validation/
└── Verify-ImportarServicioWebFrontend.ps1

tools/e2e/tests/
└── importar-servicio-web-modern.spec.cjs
```

- Reutilizar pruebas y fixtures de F01–F07 y B01–B07; no copiarlos.
- `Verify-ImportarServicioWebFrontend.ps1` es local, determinista, no autenticado y sin red.
- La especificación E2E reutiliza exclusivamente `tools/e2e`; crearla o editarla no autoriza ejecutarla.
- No crear otro proyecto Playwright, login, `.env`, configuración, arnés, carpeta E2E o paquete de pruebas paralelo.
- Este prompt no elimina ni modifica controles, handlers, ASMX, `JSProgresBar`, `ClassAlmacenamiento` o `AlmacenaDocumentoTareaWorkflow(...)`.

## Ruta documental obligatoria

```txt
docs/modulos/workflow/importar-servicio-web/SCRUMCORE-000-pruebas-gate-transicion-legacy/
```

Sustituir `SCRUMCORE-000` por el ticket real; crear `00-Indice.md` a `07-Metadata.md`, `Diagramas/` y `Evidencias/` exclusivamente allí. `Evidencias/` solo admite material saneado.

## Implementa

- Pruebas focales del núcleo, registro de adaptadores, SII, vista, preparación, progreso, reconciliación y bloqueo de tarea.
- Regresión con gate desactivado y activado, garantizando una sola entrada y un solo handler efectivo.
- Inventario de referencias de `Panel_list_inscripciones_sii`, `GridView_list_inscripciones_sii`, `ModalPopupExtender_edition_list_inscripciones_sii`, `Panel_sube_documento_integra_sii`, botones de postback y handlers asociados.
- Ocultamiento inicial del árbol visual legacy bajo gate.
- Criterios y tarea posterior, independiente y expresamente autorizada, para evaluar controles sin referencias; este prompt no elimina `btnloadservice`, endpoints, handlers ni código legacy.
- Evidencia saneada y rollback documentado.

## Restricciones operativas

- Antes de cualquier prueba autenticada de `PreviewEnviarTarea`, leer `tools/e2e/AGENT-RUNBOOK.md`.
- No modificar `AlmacenaDocumentoTareaWorkflow(...)`, `ClassAlmacenamiento`, sus consumidores ni rutas legacy.
- Gate apagado debe producir `FEATURE_DISABLED` sin efectos en endpoints modernos y conservar el recorrido vigente.
- No ejecutar E2E real, carga ni activar gates sin autorización explícita para ambiente y cuentas.
- No guardar ni imprimir credenciales, cookies o cadenas de conexión.
- Las consultas de control serán exclusivamente `SELECT`.
- `WorkflowCentroTrabajoModernActive` debe quedar en `false`, con usuarios y grupos vacíos, al terminar cualquier corrida autorizada.

## Aceptación

- Suites focales y regresiones existentes pasan.
- La evidencia identifica claramente qué se ocultó y qué todavía no puede eliminarse.
- Ninguna E2E de consulta modifica tarea, estado, documentos, expediente, índices, caché o auditoría.
- Una E2E mutadora, si se autoriza separadamente, demuestra documento visible en la tarea correcta, resultados parciales y ausencia de duplicados.

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
Leer F01–F07, B07, `AGENTS.md`, `tools/e2e/AGENT-RUNBOOK.md`, pruebas modernas existentes en `Tests/`, `workflow/Webworkflow.aspx(.vb)` y configuración actual del gate. El runbook no concede autorización de ejecución.

## Pruebas obligatorias
Ejecutar pruebas unitarias/focales, build/tsc segun impacto y E2E con Playwright cuando el flujo lo requiera; registrar comandos y resultados.

## Documentacion tecnica
Actualizar exclusivamente el paquete de **Ruta documental obligatoria**; separar evidencia local/autorizada y registrar comandos, resultados, cobertura, limitaciones y restauración del gate.

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
