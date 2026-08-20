# 05 — Verificación transversal y evidencia

## ROL ESPERADO

Actúa como arquitecto de calidad y revisor técnico de Workflow ASP.NET Web Forms.

## OBJETIVO

Verificar la capacidad completa sin crear otra implementación: contrato, capas, auditoría, seguridad, accesibilidad, compatibilidad y evidencia para liberación.

## CONTEXTO OBLIGATORIO

- Requiere 04 aprobado y evidencia focal de los tickets 02 a 04.
- Leer `00-contexto-obligatorio.md`, contratos/documentación existente y resultados de compilación/pruebas previos.
- Si hay fallo, registrar o devolver un ticket de corrección específico; 06 no se desbloquea hasta resolverlo.

## REQUISITOS POSITIVOS

- Ejecutar compilación, pruebas locales CJS/VB, análisis estático y QA manual no autenticado autorizado.
- Confirmar preview solo `SELECT`, revalidación bajo lock, respuesta pendiente sin reasignación, destinos autorizados, auditoría sanitizada, experiencia moderna universal y aislamiento de Continuar flujo.
- Verificar búsqueda: universo autorizado antes del filtro, límite, orden, cursor, privacidad, respuesta obsoleta, teclado, foco, Escape y responsive.

## RESTRICCIONES CRÍTICAS

- No cambiar código de producción, configuración, estado de tareas, auditoría, datos ni contratos para obtener evidencia.
- No ejecutar E2E autenticado, carga, activación, archivo, publicación o liberación automática.
- No considerar suficiente una revisión visual sin evidencia de contrato, concurrencia y no regresión.

## REGLAS DE ANTIRREGRESIÓN

- Comparar `PreviewEnviarTarea`, `EjecutarEnvioTarea`, `IdConector` y Continuar flujo con sus contratos/pruebas existentes; confirmar que Enviar a usuario no expone una ruta UI alternativa.
- Un resultado de búsqueda o preview nunca elimina revalidación de ejecución ni amplía autorizaciones.

## CRITERIOS DE ACEPTACIÓN

- Todos los escenarios críticos quedan aprobados o asociados a ticket de corrección con evidencia reproducible.
- La recomendación para 06 es inequívoca: apto, bloqueado o requiere corrección.

## PRUEBAS OBLIGATORIAS

Ejecutar MSBuild disponible, pruebas focales y QA manual de cancelación, éxito, bloqueo, búsqueda, responsive y accesibilidad. Registrar comandos, resultados, cobertura, limitaciones y exclusión justificada de E2E/carga.

## DOCUMENTACIÓN TÉCNICA

Actualizar `04-pruebas-y-evidencia.md` e `00-indice.md` del paquete existente; documentar matriz, correlaciones sanitizadas, riesgos, decisión Jira y ticket de corrección si aplica.

## ENTREGABLE FINAL

Reportar ticket, escenarios aprobados/fallidos, compilación, pruebas, QA, riesgos y recomendación para 06. No modificar configuración de ambiente.
