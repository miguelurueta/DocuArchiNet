<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento — DOC-38 verificación transversal

## Fuente y alcance

- Ticket: `DOC-38` — Verificación transversal y evidencia.
- Cambio OpenSpec: `doc-38-verificacion-transversal`.
- Perfil tecnológico: `legacy-webforms-vb`; la capacidad revisada vive en ASP.NET Web Forms, VB.NET, MySQL y JavaScript CJS.
- Fuentes revisadas: `prompt/00-contexto-obligatorio.md`, decisiones de `01-alcance-y-diseno.md`, el paquete técnico de DevolverUsuarioAnterior y las evidencias focales de DOC-36 y DOC-37.

DOC-38 es una compuerta de verificación para **Devolver → Usuario anterior**. No implementa endpoints, UI, configuración ni cambios de datos. Reúne evidencia reproducible y emite una recomendación inequívoca para la etapa 05; un hallazgo crítico se devuelve como corrección antes de liberar.

## Contexto inspeccionado

- El backend exclusivo está bajo `Services/Workflow/DevolverUsuarioAnterior/`; sus pruebas contractuales están en `tests/workflow-return-user-previous*.test.cjs`.
- La interfaz moderna se registra en `workflow/Webworkflow.aspx` y sus módulos se prueban junto con la devolución a actividad anterior para detectar cruces de rutas.
- El paquete técnico conserva los contratos, el flujo de seguridad y la matriz inicial en `Doc/Actualizacion/workflow/DevolverUsuarioAnterior/01-implementacion-devolver-usuario-anterior/`.
- La evidencia autenticada, de carga, despliegue y liberación automática está fuera de esta etapa salvo autorización posterior y explícita.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | DOC-38 solo verifica y documenta; no cambia código de producción, contratos, configuración, datos, tareas ni auditoría. | Árbol de cambios y `git diff` del ticket. | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | La evidencia local combina compilación disponible, pruebas CJS/VB, análisis estático y QA manual no autenticada autorizada; se registran comando, resultado y limitación. | Proyecto `.vbproj`, `tests/` y documentación técnica. | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | La revisión demuestra que preview usa solo `SELECT` parametrizados, el historial es determinista, el token vincula el antecedente y la ejecución se revalida dentro de lock exclusivo por tarea. | Servicio, repositorios, contratos y pruebas DOC-36. | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | La mutación permanece aislada en el adaptador de `Terminar_Tarea_Workflow`, con auditoría saneada y sin componentes de respuestas, notificaciones ni eventos no aprobados. | Adaptador, servicio y pruebas de aislamiento. | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | La UI de usuario anterior es exclusiva, no evalúa el feature gate, no usa postback ni fallback a actividad anterior y conserva/restaura la bandeja. | `Webworkflow.aspx`, módulos UI y pruebas DOC-37. | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | La no regresión compara Devolver actividad anterior, Continuar flujo, Enviar usuario y Enviar grupo; no se acepta evidencia visual aislada. | Suites focales y contratos existentes. | D-06 | RQ-06 | Origen: D-06, RQ-06 |
| D-07 | El informe final solo recomienda “apto para 05” cuando todos los controles críticos pasan; de lo contrario registra evidencia reproducible y requiere ticket de corrección. | Matriz final e índice documental. | D-07 | RQ-07 | Origen: D-07, RQ-07 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | La verificación no altera comportamiento ni estado de negocio. | Al revisar el diff y ejecutar controles locales, no hay escritura de producción ni llamadas autenticadas. | Evita convertir una compuerta de calidad en una liberación accidental. |
| RQ-02 | El informe permite repetir cada control local y conocer sus límites. | Cada control indica comando, resultado, cobertura y exclusiones de E2E/carga. | Evita decisiones basadas solo en una inspección visual. |
| RQ-03 | Preview, historial, token y lock cumplen el contrato de devolución segura. | Las pruebas y análisis confirman lectura sin mutación, antecedente exacto y una sola transición. | Evita devolver a un usuario o estado distinto. |
| RQ-04 | El motor y auditoría conservan el límite de aislamiento. | La revisión no encuentra referencias de respuestas ni activación de eventos o notificaciones no aprobadas. | Evita efectos laterales heredados. |
| RQ-05 | La presentación moderna no se mezcla con otras devoluciones. | Las pruebas cubren cancelación, confirmación, bloqueo, espera, responsive, accesibilidad y restauración. | Evita postbacks o rutas cruzadas. |
| RQ-06 | Los flujos vecinos conservan sus contratos y pruebas. | La suite focal se ejecuta junto con actividad anterior, continuar, enviar a usuario y enviar a grupo. | Evita una regresión transversal de Workflow. |
| RQ-07 | La decisión para 05 es auditable y accionable. | El paquete técnico registra escenarios aprobados/fallidos, riesgos y recomendación; un fallo crítico genera corrección. | No habilita una liberación con evidencia incompleta. |

## Resultado del refinamiento

- Estado: `approved`.
- La implementación de DOC-38 consiste en ejecutar y documentar la matriz aprobada; no incluye modificación de producción.
- El siguiente paso operativo es realizar las tareas de verificación con sus autorizaciones aplicables y actualizar el paquete técnico con los resultados.
