# Pruebas y evidencia — Verificación transversal

- Ticket: DOC-38
- Cambio OpenSpec: doc-38-verificacion-transversal
- Clasificacion: cross_cutting

## Evidencia requerida

La ejecución de DOC-38 registrará, para cada control, comando, resultado, alcance, fecha y limitación reproducible. La matriz mínima cubre:

- Pruebas CJS/VB de preview, historial, token, permiso, auto-devolución, lock, revalidación, adaptador, auditoría y ausencia de respuestas.
- Pruebas de interfaz de confirmación, cancelación, bloqueo, espera, accesibilidad, responsive, restauración de bandeja y aislamiento respecto de actividad anterior.
- Comparación focal de actividad anterior, continuar flujo, enviar a usuario y enviar a grupo.
- Compilación disponible y análisis estático de consultas de solo lectura, contratos exclusivos y rutas sin postback ni feature gate para usuario anterior.
- Actualización de `04-pruebas-y-evidencia.md` y `00-indice.md` del paquete de DevolverUsuarioAnterior con escenarios aprobados/fallidos, riesgos y recomendación para 05.

La evidencia existente de DOC-36 y DOC-37 se usa como línea base, pero DOC-38 registra su propio resultado para el SHA verificado. Un fallo de contrato, concurrencia, aislamiento o no regresión requiere corrección antes de recomendar liberación.

## QA/E2E WebForms

La QA manual de esta etapa es no autenticada y requiere autorización vigente antes de ejecutarse. El recorrido manual contempla confirmación, cancelación, historial no elegible, grupo, auto-devolución, cambio de historial, éxito y error funcional, bloqueo, concurrencia simulada, timeout, responsive y accesibilidad, sin alterar una tarea real.

E2E autenticada, ejecución sobre tareas descartables, carga, despliegue y liberación automática quedan excluidos hasta contar con autorización explícita de ambiente y cuentas de prueba. Esa exclusión se consigna en el informe final y no se interpreta como evidencia de cobertura de mutación real.

## Resultado local registrado

La línea base de DOC-38 no contiene modificaciones de producción. La suite local `node --test` sobre los 17 archivos `tests/*.test.cjs` aprobó 114 pruebas, incluyendo el comportamiento de usuario anterior, actividad anterior, enviar a usuario y enviar a grupo. La compilación Debug del proyecto Web Forms fue correcta; persisten advertencias históricas `MSB3247` de versiones de ensamblado sin errores de compilación.

## QA manual registrada — 2026-08-28

La correlación saneada `QA-MANUAL-DOC38-20260828` identifica los videos entregados por el operador durante la revisión. No se copian al repositorio para preservar la minimización de datos. El recorrido aprobó preview, cancelación, `Escape`, cambio de tarea, historial no elegible, antecedente sin usuario individual de flujo, exclusividad frente a Actividad anterior, responsive, foco y compatibilidad de `general_code_java.js` con `compatible-events5`.

La observación de doble clic/espera se declaró **no aplicable**: requería confirmar la transición de una tarea real, acción excluida de DOC-38. No se ejecutaron E2E autenticada automatizada, carga, despliegue ni liberación automática.

La recomendación final es **apto para continuar a 05 — liberación controlada documental**. Esta decisión no autoriza un despliegue, una transición real ni un cambio de configuración.
