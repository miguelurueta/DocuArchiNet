# 05 — Verificación transversal y evidencia

## ROL ESPERADO

Actúa como arquitecto de calidad y revisor técnico de Workflow ASP.NET Web Forms.

## OBJETIVO

Verificar la capacidad completa sin crear otra implementación: contrato, capas, auditoría, seguridad, accesibilidad, compatibilidad y evidencia para liberación.

## CONTEXTO OBLIGATORIO

- Requiere 04 aprobado y evidencia focal de 02 a 04.
- Leer `00-contexto-obligatorio.md`, documentación y resultados de compilación/pruebas previos.
- Si hay fallo, registrar o devolver ticket de corrección; 06 no se desbloquea hasta resolverlo.

## REQUISITOS POSITIVOS

- Ejecutar compilación, pruebas CJS/VB locales, análisis estático y QA manual no autenticada autorizada.
- Confirmar preview solo `SELECT`, historial de una sola tarea, usuario histórico único, revalidación bajo lock, auditoría sanitizada, aislamiento y restauración de bandeja.
- Confirmar permiso de devolución, usuario histórico, auto-devolución, token, Ruta o Flujo y concurrencia.
- Confirmar por pruebas estáticas y focales la ausencia total de tratamiento de respuestas y de fallback a actividad anterior.

## RESTRICCIONES CRÍTICAS

- No cambiar código de producción, configuración, estado de tareas, auditoría, datos ni contratos para obtener evidencia.
- No ejecutar E2E autenticada, carga, despliegue, archivo, publicación o liberación automática.
- No considerar suficiente una revisión visual sin evidencia de contrato, concurrencia y no regresión.

## REGLAS DE ANTIRREGRESIÓN

- Comparar contratos y pruebas de Devolver a actividad anterior, Continuar flujo, Enviar a usuario y Enviar a grupo; no deben verse afectados.
- La devolución a usuario anterior no puede abrir, invocar ni sustituirse por la devolución a actividad anterior.

## CRITERIOS DE ACEPTACIÓN

- Todos los escenarios críticos quedan aprobados o asociados a ticket de corrección con evidencia reproducible.
- La recomendación para 06 es inequívoca: apto, bloqueado o requiere corrección.

## PRUEBAS OBLIGATORIAS

Ejecutar MSBuild disponible, pruebas focales y QA manual de confirmación, cancelación, historial ausente, grupo, auto-devolución, éxito, error, bloqueo, concurrencia, responsive y accesibilidad. Registrar comandos, resultados, cobertura, limitaciones y exclusión justificada de E2E/carga.

## DOCUMENTACIÓN TÉCNICA

Actualizar `04-pruebas-y-evidencia.md` e `00-indice.md` del paquete documental de DevolverUsuarioAnterior con matriz, correlaciones sanitizadas, riesgos y decisión Jira.

## ENTREGABLE FINAL

Reportar ticket, escenarios aprobados/fallidos, compilación, pruebas, QA, riesgos y recomendación para 06. No modificar configuración de ambiente.
