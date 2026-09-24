# Prompt 04 — Pruebas, gate, compatibilidad y cierre OPSXJ

## Contexto de ejecución OPSXJ

Este prompt se ejecuta dentro de un ticket OPSXJ previamente creado y revisado. No ejecutar `opsxj:new`, no crear otro ticket, no reiniciar la orquestación y no crear un cambio OpenSpec manual. Continúa exclusivamente sobre el contexto, identificador y artefactos OPSXJ recibidos.

Este prompt completa la validación del flujo; no amplía por sí mismo la autorización para ejecutar E2E, activar gates o usar cuentas reales.

## Rol esperado

Actúa como QA senior, arquitecto de confiabilidad y responsable de transición reversible. Verifica evidencia real; no reemplaces fallos con mocks, resultados inventados ni validaciones manuales incompletas.

## Contexto obligatorio

Lee completamente antes de diseñar o ejecutar validaciones:

- `../exploracion/modernizacion-importacion-anexos-sii-enlase.md` como base arquitectónica común y fuente de la matriz E2E y de reutilización.
- Prompts 01 a 03 y sus entregables reales.
- `AGENTS.md`.
- `tools/e2e/AGENT-RUNBOOK.md` antes de cualquier prueba autenticada aplicable.
- Infraestructura E2E existente de `ImportarServicioWeb`.
- Perfiles, fixtures, validadores y evidencia saneada existentes.
- Configuración vigente del gate y flujo legacy `ENLASE`.
- Documentación y pruebas de DOC-56 y DOC-79 como patrones de seguridad y restauración.

La cobertura debe demostrar las decisiones y riesgos documentados en la exploración, no limitarse a verificar que la interfaz abre. Mantén trazabilidad entre cada riesgo alto o muy alto, su prueba, evidencia y resultado OPSXJ.

Si las pruebas o la evidencia real contradicen la exploración, registra el hallazgo, conserva el fallo y actualiza la documentación mediante OPSXJ. No adaptes la expectativa después de la corrida para declarar éxito, no ocultes el resultado y no cierres el ticket mientras exista una contradicción material sin resolver.

## Objetivo

Demostrar que la capacidad moderna de anexos SII funciona de extremo a extremo, conserva el flujo legacy cuando corresponde, no contamina otras tareas y puede activarse o revertirse de manera segura mediante el mecanismo aprobado.

## Reutilización obligatoria

- Reutilizar exclusivamente `tools/e2e`, su autenticación, configuración, sanitización y utilidades.
- Extender escenarios existentes cuando sea técnicamente coherente.
- No crear otro proyecto Playwright, login, `.env`, arnés, configuración, carpeta E2E o sistema de evidencias paralelo.
- Reutilizar fixtures y pruebas focales de `ImportarServicioWeb`; no copiarlos para cambiar solamente nombres.

## Cobertura obligatoria

### Pruebas locales y de integración

- Registro y resolución de la capacidad.
- Autorización del contexto `ENLASE`.
- Consulta, preview y descarga segura.
- Preparación individual y múltiple.
- Tipología predeterminada inequívoca y selección manual cuando sea ambigua.
- Idempotencia, concurrencia, resultado parcial e incierto.
- Existencia física, deduplicación y reconciliación.
- Actualización de documentos relacionados.
- Protección ante cambio de tarea.
- Revalidación anterior a la asignación.
- Accesibilidad y comportamiento adaptable.
- Gate habilitado y deshabilitado.
- Regresión de constancias SII y del flujo legacy.

### E2E autorizada

Cuando exista autorización expresa, cubrir como mínimo:

1. Lectura con una tarea `ENLASE` válida sin producir mutaciones.
2. Cero, uno y múltiples anexos.
3. Selección total y preparación de múltiples elementos.
4. Preview seguro.
5. Importación autorizada con datos descartables.
6. Resultado visible en la lista documental de la tarea original.
7. Ausencia de duplicados ante repetición o recuperación.
8. Rechazo de contexto, capacidad o acceso directo no autorizado.
9. Bloqueo de asignación cuando falten documentos.
10. Asignación explícita después de completar requisitos.
11. Regresión con gate desactivado.
12. Restauración íntegra del ambiente al finalizar.

## Reglas operativas de seguridad

- No ejecutar E2E real, carga ni activar gates sin autorización explícita para ambiente, cuentas y datos.
- No guardar, imprimir ni incorporar a evidencia credenciales, cookies, tokens o cadenas de conexión.
- Utilizar secretos efímeros durante la ejecución autorizada.
- Las consultas de control deben ser exclusivamente `SELECT`.
- Una prueba de lectura no puede cambiar tarea, estado, documento, expediente, índice, caché o auditoría funcional.
- Una prueba mutadora requiere autorización separada y datos descartables identificados.
- Ante fallo de integridad del gate, proveedor, legacy o evidencia, detener la corrida de forma segura.
- Verificar y documentar la restauración del gate, usuarios y grupos conforme a la configuración aprobada.

## Gate y transición

- Inspeccionar primero si `WorkflowCentroTrabajoModernActive` es reutilizable para esta capacidad sin acoplar indebidamente ambos recorridos.
- Si se requiere alcance independiente, proponerlo y obtener aprobación dentro de OPSXJ; no inventar ni activar un gate nuevo silenciosamente.
- Ocultar la interfaz no reemplaza autorización backend.
- Cada endpoint moderno debe validar la habilitación y el contexto completo.
- No retirar controles, handlers o endpoints legacy en este prompt salvo que el ticket lo autorice expresamente y exista inventario sin referencias más rollback probado.
- La primera entrega debe priorizar alternancia reversible.

## Matriz de cierre

| Condición | Requisito de cierre |
|---|---|
| Suites focales | Todas aprobadas |
| Build/compilación | Aprobado o limitación externa demostrada |
| E2E requerida | Ejecutada con autorización y evidencia saneada |
| E2E no autorizada | Bloqueo explícito; no declarar validación completa |
| Gate | Restaurado y verificado |
| Flujo legacy | Disponible cuando el gate está apagado |
| Capacidad moderna | Autorizada en backend, no solo visible en UI |
| Persistencia | Documento y relación verificados |
| Idempotencia | Sin duplicados |
| Tarea | No asignada por la importación |
| Asignación | Solo después de revalidación explícita |
| Evidencia | Saneada y trazable |
| Documentación | Coincide con el código realmente entregado |

## Restricciones críticas y antirregresión

- No corregir una prueba debilitando validaciones productivas.
- No introducir esperas arbitrarias para ocultar carreras.
- No aceptar un resultado visual como prueba única de persistencia.
- No ejecutar scripts SQL mutadores como consultas de control.
- No eliminar legacy únicamente porque la interfaz moderna abrió correctamente.
- No cerrar OPSXJ con pruebas obligatorias omitidas sin bloqueo formal.
- No declarar éxito basándose en mocks cuando el criterio exige integración real.

## Documentación y evidencia

Documenta exclusivamente en:

```text
Doc/Actualizacion/workflow/ImportarServiciWebEnlace/<TICKET>-pruebas-gate-compatibilidad/
```

Incluye inventario legacy, matriz de pruebas, comandos, resultados, evidencia saneada, gate, autorización, restauración, rollout, rollback, limitaciones y metadata OPSXJ.

## Entregable final

Entrega código de pruebas, validadores, documentación y evidencia. Código, E2E autorizada y evidencia forman una sola unidad cuando el criterio funcional exige recorrido real. Continúa publicación, sincronización y cierre únicamente mediante OPSXJ.
