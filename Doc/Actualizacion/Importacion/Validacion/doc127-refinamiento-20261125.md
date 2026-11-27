# DOC-127 — Refinamiento técnico

**Revisión técnica de Codex: PASS.** Base `d1d143954bcd0dc45198b862437d241e6dcc5643`. Se concretaron **14 decisiones, 14 requisitos, 42 escenarios y 33 tareas** (3 S, 30 M, 0 L); implementación y aceptación **0/33**. No se atribuye aprobación humana global.

Se inventariaron 61 rutas existentes y 10 documentos de evidencia, con huellas UTF-8/LF en el JSON. Se contrastaron responsabilidades y símbolos del cargador, límites, transacción/locks, recuperación, consumidor por clave y harness. El inventario no afirma una revisión línea por línea de cada archivo ni una ejecución nueva.

La comparación conserva las 26 fuentes no OpenSpec de la revisión final DOC126; DOC125 coincide en 17 de 34 fuentes y requiere evaluar diferencias por componente. Los resultados originales mantienen SHA, ambiente, runtime y fecha. La declaración manual del usuario sigue reconocida con su alcance original.

Decisiones centrales: preservar 20.000 filas configuradas y 10 MB decimales, conexión DA del módulo, E2E y tablas existentes. Completar capacidad de 1.000/5.000/10.000 por formato y modo, fronteras, EXPLAIN manual y matriz de recuperación/autoridad sin repetir resultados aceptados innecesariamente. No convertir 8.000/12.759 preparadas o 1.000 confirmadas en aceptación física de 20.000. Lectura real v6 y corrección local v7 permanecen diferenciadas.

Esta entrega sólo modifica planificación y documentación. No compila producto, ejecuta pruebas funcionales, autentica, genera/aplica SQL o despliega. Las tareas identifican dependencias de autorización real, aplicación manual confirmada y plan aportado; el trabajo local independiente puede avanzar.

Validación: **refinamiento 15/15 PASS**, **OpenSpec del cambio estricto PASS**, **trazabilidad 33/33 tareas y 14/14 decisiones/requisitos PASS**, contrato de las cinco notas técnicas y `git diff --check` PASS. Contexto Jira y detalle original de la propuesta conservados. La validación documental no sustituye los gates finales del SHA de aceptación.

Artefactos: `openspec/changes/doc-127-importacion-excel-07-verificacion-integral-cierre/`. Próxima etapa: manifiesto/matriz de reutilización, preflight SQL revisable y verificación local aislada antes de solicitar nuevas ventanas reales.
