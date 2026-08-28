<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07 -->
## Context

DOC-38 verifica la capacidad ya implementada de Devolver → Usuario anterior en Workflow ASP.NET Web Forms. El resultado no se basa solo en la interfaz: debe relacionar controles estáticos, pruebas locales y QA manual no autenticada con los límites de seguridad definidos por DOC-36 y DOC-37.

## Goals / Non-Goals

**Goals**

- Obtener evidencia reproducible de contrato, concurrencia, aislamiento, UI y no regresión.
- Actualizar el paquete técnico con una recomendación verificable para la etapa 05.
- Reportar cualquier fallo crítico como corrección, sin ocultarlo detrás de una prueba parcial.

**Non-Goals**

- Crear endpoints, modificar producción, activar gates, alterar tareas reales o ejecutar E2E autenticada, carga, despliegue o liberación automática.

## Decisions

### D-01 — Verificación sin mutación

La etapa opera sobre código y evidencia local. Solo se permiten comandos de lectura, compilación y pruebas autorizadas; la revisión comprueba que no se cambian configuraciones, contratos, datos, auditoría ni estado de tareas para obtener resultados favorables.

### D-02 — Matriz de evidencia reproducible

Cada control registra su comando, resultado, cobertura y limitación. La compilación disponible, las pruebas CJS/VB, el análisis estático y la QA manual no autenticada son fuentes complementarias; una inspección visual no sustituye el contrato o la concurrencia.

### D-03 — Seguridad de preview y ejecución

La matriz verifica que preview conserva solo `SELECT` parametrizados y que el historial identifica el antecedente inmediato elegible de forma determinista. También verifica token opaco ligado a tarea/historial, lock exclusivo por tarea y revalidación dentro del lock antes de una sola mutación.

### D-04 — Límite del motor legacy

La transición se revisa únicamente a través del adaptador dedicado a `Terminar_Tarea_Workflow`, con `Page = Nothing`, actualización legacy, notificaciones y eventos no aprobados desactivados. Ninguna capa nueva puede incluir componentes de respuestas; auditoría solo contiene datos saneados.

### D-05 — Exclusividad de interfaz

La operación moderna de usuario anterior no evalúa `WorkflowCentroTrabajoModernActive`, no usa postback y no abre ni sustituye la devolución a actividad anterior. La evidencia cubre confirmación, cancelación, bloqueo, espera, accesibilidad, responsive y restauración de la bandeja.

### D-06 — Comparación de regresión

La suite se ejecuta con las rutas de devolución a actividad anterior, continuar flujo, enviar a usuario y enviar a grupo. Sus contratos no se reutilizan como destino de usuario anterior ni se modifican para satisfacer esta verificación.

### D-07 — Decisión de salida

El informe final registra escenarios aprobados y fallidos, riesgos y exclusiones. Solo todos los controles críticos aprobados permiten recomendar la etapa 05; cualquier fallo crítico se devuelve con evidencia reproducible y ticket de corrección.

## Risks / Trade-offs

- La QA manual no autenticada no prueba mutaciones reales; por eso se complementa con contratos, pruebas y análisis estático, y se declara la exclusión de E2E autenticada y carga.
- La compilación puede conservar advertencias históricas. Se registra su resultado sin clasificarlas como regresiones nuevas sin evidencia.
- La evidencia local no autoriza cambio de configuración ni liberación; esas acciones pertenecen a etapas posteriores.

## Verification Design

La matriz se organiza por RQ-01 a RQ-07. Cada fila enlaza un control local o manual con los límites de preview, historial, ejecución, UI o no regresión. Los resultados se incorporan a `04-pruebas-y-evidencia.md` y se enlazan desde `00-indice.md`; una fila crítica fallida bloquea la recomendación.
