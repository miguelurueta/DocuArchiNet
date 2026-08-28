# Jira Context - DOC-38

## Summary

VERIFICACION-TRANSVERSAL

## Description

> # 04 — Verificación transversal y evidencia
> 
> ## ROL ESPERADO
> 
> Actúa como arquitecto de calidad y revisor técnico de Workflow ASP.NET Web Forms.
> 
> ## OBJETIVO
> 
> Verificar la capacidad completa sin crear otra implementación: contrato, capas, auditoría, seguridad, accesibilidad, compatibilidad y evidencia para liberación.
> 
> ## CONTEXTO OBLIGATORIO
> 
> - Requiere 03 aprobado y evidencia focal de 02 y 03.
> - Leer `00-contexto-obligatorio.md`, decisiones de 01, documentación y resultados de compilación/pruebas previos.
> - Si hay fallo, registrar o devolver ticket de corrección; 05 no se desbloquea hasta resolverlo.
> 
> ## REQUISITOS POSITIVOS
> 
> - Ejecutar compilación, pruebas CJS/VB locales, análisis estático y QA manual no autenticada autorizada.
> - Confirmar preview solo `SELECT`, algoritmo determinista de historial, token vinculado al historial, lock exclusivo por tarea, revalidación bajo lock, auditoría sanitizada, aislamiento y restauración de bandeja.
> - Confirmar permiso de devolución, usuario histórico, auto-devolución, Ruta/Flujo, concurrencia, política de notificación/eventos aprobada y ausencia de componentes de respuestas en la capacidad nueva.
> - Confirmar que UI no evalúa feature gate para esta operación y que no hay postback, handler ni fallback hacia actividad anterior.
> 
> ## RESTRICCIONES CRÍTICAS
> 
> - No cambiar código de producción, configuración, estado de tareas, auditoría, datos ni contratos para obtener evidencia.
> - No ejecutar E2E autenticada, carga, despliegue, archivo, publicación o liberación automática.
> - No considerar suficiente una revisión visual sin evidencia de contrato, concurrencia y no regresión.
> 
> ## REGLAS DE ANTIRREGRESIÓN
> 
> - Comparar contratos y pruebas de Devolver a actividad anterior, Continuar flujo, Enviar a usuario y Enviar a grupo; no deben verse afectados.
> - La devolución a usuario anterior no puede abrir, invocar ni sustituirse por la devolución a actividad anterior.
> 
> ## CRITERIOS DE ACEPTACIÓN
> 
> - Todos los escenarios críticos quedan aprobados o asociados a ticket de corrección con evidencia reproducible.
> - La recomendación para 05 es inequívoca: apto, bloqueado o requiere corrección.
> 
> ## PRUEBAS OBLIGATORIAS
> 
> Ejecutar MSBuild disponible, pruebas focales y QA manual de confirmación, cancelación, historial ausente, grupo, auto-devolución, historial cambiado, éxito, error, bloqueo, concurrencia, timeout, responsive y accesibilidad. Registrar comandos, resultados, cobertura, limitaciones y exclusión justificada de E2E/carga.
> 
> ## DOCUMENTACIÓN TÉCNICA
> 
> Actualizar `04-pruebas-y-evidencia.md` e `00-indice.md` del paquete documental de DevolverUsuarioAnterior con matriz, correlaciones sanitizadas, riesgos y decisión Jira.
> 
> ## ENTREGABLE FINAL
> 
> Reportar ticket, escenarios aprobados/fallidos, compilación, pruebas, QA, riesgos y recomendación para 05. No modificar configuración de ambiente.

## Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: TRANSVERSAL, VERIFICACION
