# 08 — Pruebas, evidencia y verificación

## ROL ESPERADO

Actúa como arquitecto de calidad y revisor técnico de una modernización incremental de Workflow ASP.NET Web Forms.

## OBJETIVO

Verificar que Enviar a usuario moderno termina la tarea únicamente por `Terminar_Tarea_Workflow`, conserva el motor legacy, bloquea respuesta pendiente y no genera regresiones en Continuar flujo ni en el fallback Web Forms.

## RESTRICCIONES CRÍTICAS

- Lee y aplica `prompts/00-contexto-obligatorio.md`.
- Ejecutar solo compilación, pruebas locales y QA manual autorizados.
- No ejecutar E2E autenticado, carga ni activar gate sin autorización explícita, ambiente y cuentas descartables aprobadas.
- No alterar tareas, estados, auditoría ni configuración para obtener evidencia del preview.
- No considerar suficiente una validación visual sin evidencia de contrato, concurrencia, seguridad y no regresión.
- No archivar el cambio OpenSpec sin autorización.

## REQUISITOS POSITIVOS

Verificar y registrar:

1. Preview exclusivamente de lectura, sin auditoría, estado ni eventos.
2. Permiso `CAMBIO_USUARIO` permitido y denegado.
3. Ruta, flujo y actividad de flujo cerrados.
4. Usuario destino inactivo, fuera de ruta, sin actividad asociada o con `UTIL_ASIGNA_TAREA=0`.
5. Respuesta pendiente bloqueada sin llamadas a métodos de reasignación.
6. Destino retirado entre preview y ejecución, token vencido y doble ejecución concurrente.
7. Advertencia de correo/evento sin revertir éxito confirmado.
8. Gate inactivo con retorno exacto a postback legacy.
9. No regresión de `PreviewEnviarTarea`, `EjecutarEnvioTarea`, payload `IdConector` y pruebas actuales.
10. Separación de capas: Presentation sin SQL/reglas, Application sin Web Forms y adaptador como único llamador del motor.

## CRITERIOS DE ACEPTACIÓN

- Compilación y pruebas focales finalizan correctamente, o sus limitaciones quedan justificadas y reproducibles.
- Escenarios críticos no producen transición duplicada, fuga de información, reasignación de respuesta ni pérdida de contexto.
- Evidencia correlaciona resultado, código público y auditoría sanitizada.
- Configuración de gate no fue modificada y fallback continúa disponible.

## PRUEBAS OBLIGATORIAS

Ejecutar MSBuild cuando sea posible; ejecutar pruebas VB/JavaScript afectadas y registrar comandos, resultados y cobertura. Ejecutar QA manual de cancelación, éxito, respuesta pendiente, bloqueo, error, teclado, Escape, foco, accesibilidad y vistas responsive. Documentar por qué E2E/carga no se ejecutaron si no hay autorización.

## DOCUMENTACIÓN TÉCNICA

Crear o actualizar `Doc/Actualizacion/workflow/TerminarUsuario/08-pruebas-verificacion/` con matriz, comandos, compilación, QA, evidencia, limitaciones, correlaciones, exclusiones y riesgos residuales; actualizar OpenSpec para reflejar la verificación.

## ENTREGABLE FINAL

Entregar reporte verificable de archivos revisados, pruebas/compilación, evidencia QA, documentación, resultados por escenario, riesgos y recomendación fundada de continuar, bloquear o solicitar corrección.

