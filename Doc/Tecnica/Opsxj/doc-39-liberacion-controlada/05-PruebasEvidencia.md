# Pruebas y evidencia de liberación controlada

- Ticket: DOC-39
- Cambio OpenSpec: doc-39-liberacion-controlada
- Clasificacion: cross_cutting

## Evidencia requerida

DOC-39 parte de evidencia técnica ya aprobada y añade controles documentales. No ejecuta una prueba autenticada, carga, cambio de ambiente, despliegue ni devolución real.

| Tipo | Referencia verificable | Resultado | Límite |
| --- | --- | --- | --- |
| Unitario y estático | DOC-38: `node --test tests/*.test.cjs` | 114 pruebas aprobadas. | Corresponde a la línea base local, no al ambiente. |
| Compilación | DOC-38: `msbuild GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /m:1 /v:minimal` | Correcta, con advertencias históricas `MSB3247`. | No instala artefactos. |
| QA manual | `QA-MANUAL-DOC38-20260828` | Preview, cancelación, teclado, cambio de tarea, bloqueos, exclusividad, responsive y compatibilidad JS aprobados. | No confirmó una devolución final. |
| Revisión DOC-39 | Matriz, runbook y decisión de este paquete | Sin cambios operativos realizados. | Requiere aprobaciones formales antes de una ventana. |

Los resultados de consultas futuras se deben sanear antes de adjuntarse: se conservan solo fecha, ambiente, versión, control evaluado, conteos y resultado. Se excluyen nombres de usuarios, identificadores de tareas, tokens, sesiones, cookies, credenciales, cadenas de conexión, hosts y filas completas de auditoría o historial.

## QA/E2E WebForms

No se ejecutó E2E, carga ni QA autenticada durante DOC-39. La autorización histórica de QA utilizada en DOC-38 no se reutiliza ni se interpreta como autorización de liberación para GESTOR. Toda futura comprobación manual o automatizada exige autorización explícita para su ambiente, cuentas de prueba y ventana.

Una vez autorizada la operación, los controles de solo lectura se limitan a la versión del artefacto, la configuración aprobada por el responsable de despliegue y el historial/auditoría que corresponda a una tarea de prueba autorizada. Esta etapa deja el procedimiento preparado, pero no lo inicia ni conserva datos de ejecución.

## Riesgos residuales y decisión

El riesgo principal es confundir la evidencia de pruebas con autorización para operar GESTOR. La matriz indica que autorización, ventana y responsables no están registrados formalmente; por ello la decisión de DOC-39 es **solicitar aprobación**. Un fallo de control crítico en una futura ventana obliga a **bloquear** la operación y escalarlo; solo la matriz completa, el responsable designado y la autorización explícita permiten declarar una liberación lista para despliegue autorizado.
