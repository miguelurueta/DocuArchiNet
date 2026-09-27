<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09 -->
## Context

DOC-83 cierra la validación transversal de la importación moderna de anexos SII ENLASE implementada por DOC-80, DOC-81 y DOC-82. El alcance es pruebas, evidencia, gate, compatibilidad y operación reversible; no es una cuarta implementación funcional.

## Goals / Non-Goals

**Goals**
- Convertir riesgos altos y muy altos en una matriz trazable de prueba, evidencia y criterio de cierre.
- Reutilizar la plataforma E2E y las suites focales existentes.
- Demostrar separación entre lectura, importación y asignación.
- Mantener gate y legacy restaurables y fallar cerrado ante evidencia incompleta.

**Non-Goals**
- Crear proveedor, endpoint, modal, login, gate o almacenamiento nuevos.
- Retirar legacy o cambiar la política de asignación.
- Ejecutar E2E real sin autorización vigente, o repetir una mutación ya demostrada sin necesidad técnica.
- Corregir producción debilitando una expectativa de prueba.

## Decisions

### D-01 — Cierre orientado a evidencia, no nueva funcionalidad

DOC-83 añade pruebas, validadores y documentación. Una falla productiva se conserva como hallazgo y requiere refinamiento explícito antes de cualquier corrección de lógica.

### D-02 — Inventario único de cobertura

Una matriz versionada relacionará cada riesgo y condición de cierre con requisito, prueba determinista, escenario E2E, autorización, mutabilidad, controles y evidencia. La prueba documental fallará si falta una fila obligatoria o referencia registrada.

### D-03 — Reutilización de tres recorridos existentes

`import-sii-enlase-read` demuestra lectura/preview sin cambios; `import-sii-enlase-ui` demuestra interacción real no mutadora; `import-sii-enlase-execution` demuestra intención, persistencia física y ausencia de transición. DOC-83 los orquesta y extiende solo donde exista una brecha verificable.

### D-04 — Fronteras de autorización independientes

Lectura exige ambiente y gate. Importación exige además ejecución y recurso descartable. Una asignación exitosa, si debe ejecutarse, usa autorización y reserva E2E independientes de la importación. Por defecto se prepara otra tarea; reutilizar la misma exige autorización expresa y una precondición funcional nueva verificable. Ninguna autorización se hereda entre corridas.

### D-05 — Gate global existente y rollback verificable

Se conserva `WorkflowCentroTrabajoModernActive`; no se crea gate ENLASE paralelo. El runner es el único autorizado para habilitarlo temporalmente y debe restaurar `false`, usuarios y grupos vacíos en `finally`. Gate apagado conserva la superficie legacy y no enlaza bootstrap moderno.

### D-06 — Evidencia mínima, saneada y autoritativa

Los controles son `SELECT` registrados. La evidencia conserva códigos, conteos, huellas, aserciones y eventos de recurso; excluye cuerpos SII, credenciales, cookies, tokens, cadenas de conexión e identidades innecesarias. Una señal visual no sustituye persistencia física o controles de tarea.

### D-07 — Idempotencia y aislamiento por tarea

La matriz debe cubrir intención única, selección múltiple, repetición/recuperación sin duplicados, existencia física y rechazo de respuestas de otra tarea. No se reejecutará una mutación solo para obtener otra captura cuando la evidencia previa siga siendo aplicable y trazable.

### D-08 — Asignación explícita conserva autoridad legacy

Importar nunca asigna. La cobertura verifica que `Buttonaceptar_Click` revalida documentos obligatorios y que una asignación real solo se prueba con autorización mutadora separada. DOC-83 no inventa `ValidateAssignment` ni anuncia habilitación preventiva.

### D-09 — Cierre fallando cerrado

No se declara validación completa si falta autorización E2E, recurso descartable, proveedor, integridad legacy, restauración o evidencia. El resultado se registra como bloqueo explícito; OPSXJ solo continúa cuando la matriz obligatoria está satisfecha o documenta una limitación externa aceptada.

## Risks / Trade-offs

- Las evidencias DOC-80/81/82 pertenecen a corridas y recursos concretos; deben citarse sin presentarlas como cobertura universal de ambientes.
- El gate global acopla temporalmente varias capacidades modernas, pero crear otro gate en un ticket de pruebas aumentaría el riesgo y excedería el alcance.
- Asignar una tarea es una mutación distinta de importar; ambas operaciones conservan reservas y autorizaciones independientes. Reutilizar la tarea subyacente es una excepción trazada, no una autorización heredada.
- Casos cero/uno/múltiples dependen de datos externos reales; la matriz debe distinguir cobertura determinista de observación E2E.
- El estado remoto del SII puede impedir una corrida; el fallo se conserva y no se reemplaza con mocks cuando el criterio exige integración real.

## Validation Plan

1. Inventariar suites, escenarios, perfiles, controles y evidencias DOC-80/81/82.
2. Ejecutar pruebas deterministas de contratos, autorización, gate, legacy, UI, persistencia, idempotencia y documentación.
3. Añadir escenarios/políticas faltantes sin duplicar infraestructura.
4. Solicitar autorización solo al llegar a una E2E real concreta y declarar su mutabilidad.
5. Comprobar restauración del gate y ausencia de cambios no autorizados incluso ante fallos.
6. Registrar resultados y limitaciones en documentación DOC-83 y evidencia OPSXJ.

## Rollback

Los cambios de DOC-83 son pruebas/documentación. Su rollback retira esos artefactos sin tocar producción. Durante E2E, el runner restaura el contenido original de configuración; si falla la integridad final, la corrida se detiene y no se declara éxito ni se continúa con otra etapa.
