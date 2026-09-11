# E2E autorizada — ejecución detenida en reconciliación

- Fecha: 2026-09-09, zona America/Bogota.
- Escenario: `import-sii-execution`, una muestra sobre recurso descartable autorizado.
- Resultado: la corrida alcanzó `ExecuteImportIntent` y se detuvo en `GetImportIntent` con `IMPORT_E2E_GET_FAILED_IMPORT_UNAVAILABLE`.
- Decisión de seguridad: no se reejecutó la intención ni se reutilizó el recurso, porque la persistencia podía haberse completado antes de perder la lectura de reconciliación.
- Gate al cierre y después del diagnóstico: inactivo, usuarios y grupos vacíos.

## Diagnóstico SELECT

| Fragmento | Resultado |
| --- | --- |
| Intención e item en Workflow | OK |
| Nombre documental en producción | Falló |
| Conteo documental en producción | Falló |
| Conteo de relación en la intención | OK |
| Conteo de otras tareas en la intención | OK |

La causa fue un cruce de fuentes: `MySqlImportReconciliationRepository` intentaba consultar `registro_producion_documental` usando la conexión Workflow. La tabla de producción documental pertenece a la conexión DocuArchi.

## Corrección implementada

El repositorio ahora reconstruye intención y relaciones desde Workflow y enriquece existencia/nombre desde una conexión DocuArchi separada. La composición ASMX exige y entrega ambas conexiones. Las consultas permanecen parametrizadas y exclusivamente `SELECT`.

Validación local posterior: 11 pruebas focalizadas aprobadas y compilación del proyecto con 0 errores. Las pruebas de concurrencia permanecen pendientes; no se contabiliza DOC-56 completo mientras falten esas muestras y sus métricas.

## Recuperación posterior

Se agregó el escenario canónico `import-sii-recovery`, que obtiene una intención existente mediante un control `SELECT` e invoca exclusivamente `GetImportIntent`. Sus regresiones locales pasaron y los intentos autenticados conservaron los tres controles sin cambios. La primera recuperación reveló además que la sesión Workflow directa no aportaba la conexión `DA_`. La rama directa ahora deriva ambas cadenas exclusivamente de la sesión confiable y la composición falla cerrada si DocuArchi no está disponible; se retiró el fallback incorrecto a `MyDbContext`.

La recuperación autenticada final terminó satisfactoriamente: `success=true`, una intención recuperada, ningún código de error, latencia total de 5081 ms y tres controles sin cambios. El escenario no declaró ni consumió recursos y no invocó ejecución. El bloque `finally` restauró el gate a `false`, con usuarios y grupos vacíos, también después de las corridas fallidas anteriores; queda aprobada la restauración segura 4.7.

## Validación posterior

- Runner determinista local: 100 pruebas aprobadas en 30 archivos.
- OpenSpec estricto: cambio válido.
- OPSXJ: bloqueo esperado con ocho requisitos pendientes, incluidos E2E/concurrencia/evidencias y tareas OpenSpec incompletas. Este resultado demuestra el bloqueo de cierre 5.4; no aprueba todavía 5.3.

## Concurrencia autorizada

- Escenario: `import-sii-concurrency`, nivel fijo y máximo autorizado de 2 solicitudes simultáneas sobre un recurso descartable nuevo.
- Resultado: una ejecución aceptada y una bloqueada con `VERSION_CONFLICT`; una sola intención creada y una sola ejecución documental aceptada.
- Correlaciones: 2 solicitudes reales con identificadores independientes generados por el runner; se conserva únicamente el conteo.
- Controles: 3 comprobados y con cambio esperado.
- Ciclo del recurso: listo, reservado y consumido.
- Latencias por la secuencia fija: consulta SII 703 ms, preflight 36 ms, creación 22 ms y carrera de ejecución 19/1383 ms.
- Estadísticos de las cinco observaciones: mínimo 19 ms, promedio 432.6 ms, p50 36 ms, p95 1383 ms, p99 1383 ms y máximo 1383 ms.
- Ventana de la carrera: 1383 ms; throughput observado: 1.45 solicitudes/s.
- Errores inesperados: 0. Bloqueos optimistas esperados: 1.
- Restauración: gate `false`, usuarios y grupos vacíos.

> Rectificación: esta corrida confirmó una ejecución aceptada, un conflicto y cambios en las tablas modernas, pero el arnés todavía no consultaba DocuArchi después de la carrera. Por tanto, no constituye evidencia suficiente de almacenamiento físico ni de documento único. Las tareas 4.3 y 4.5 se reabrieron. El adaptador fue endurecido para exigir posteriormente `GetImportIntent` con exactamente un item `Disponible`, `DocumentId` positivo y `PersistenceKnown=true`; esa reconciliación comprueba que existe exactamente un registro documental y una relación válida.

La recuperación endurecida de la intención concurrente devolvió `DOCUMENT_STORAGE_REJECTED`. En consecuencia, el item no tiene almacenamiento confirmado y el DTO oculta `DocumentId`; no se declara ningún documento almacenado. El valor `Accepted` observado en la carrera corresponde a aceptación del procesamiento de la intención, no al éxito del item. La evidencia de concurrencia queda invalidada para los criterios de documento único hasta corregir el rechazo legacy y repetirla con un recurso nuevo.

La causa raíz está en la inicialización legacy del login Workflow: `SolicitaIdUsuarioGestionRelacionadoUsuarioWorkflow` resolvía `id_usuario_gestion_wf`, pero esa rama no lo propagaba a `Session("GA_IDUSUARIOGESTION")`. El valor conservaba el cero inicial de `Global.asax`; el almacenamiento con inventario documental rechaza explícitamente un usuario Gestión igual a cero. Se agregó la asignación que ya realiza la rama Radicación, sin modificar `ClassAlmacenamiento`, junto con una regresión específica. Las pruebas focalizadas y la compilación finalizaron en cero.
