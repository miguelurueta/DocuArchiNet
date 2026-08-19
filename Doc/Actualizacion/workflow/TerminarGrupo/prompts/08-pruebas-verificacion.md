# 08 — Pruebas, evidencia y verificación

## ROL ESPERADO

Actúa como arquitecto de calidad y revisor técnico de una modernización incremental de Workflow ASP.NET Web Forms.

## OBJETIVO

Verificar que `Enviar a grupo` moderno respeta su contrato directo, conserva el motor legacy y no genera regresiones en continuar flujo ni en el fallback Web Forms.

## RESTRICCIONES CRITICAS

- Leer y aplicar `prompts/00-contexto-obligatorio.md`.
- Ejecutar solo compilación, pruebas locales y QA manual autorizados.
- No ejecutar E2E autenticado, carga ni activar gate sin autorización explícita, ambiente y cuentas descartables aprobadas.
- No alterar tareas, estados, auditoría ni configuraciones para obtener evidencia de preview.
- No considerar éxito una validación visual sin evidencia de contrato, concurrencia, seguridad y no regresión.
- No archivar el cambio OpenSpec sin autorización.

## REQUISITOS POSITIVOS

Verificar y registrar:

1. Preview exclusivamente de lectura y sin auditoría/estado/eventos.
2. Permiso `Cambio_Ruta` permitido y denegado.
3. Ruta, flujo y actividad de flujo cerrados.
4. Destino fuera de ruta o retirado entre preview y ejecución.
5. Aprobación pendiente, token vencido y doble ejecución concurrente.
6. Advertencia de correo/evento sin revertir éxito confirmado.
7. Gate inactivo con retorno exacto al postback legacy.
8. No regresión de `PreviewEnviarTarea`, `EjecutarEnvioTarea`, payload `IdConector` y sus pruebas existentes.
9. Separación de capas: Presentation sin SQL/reglas, Application sin Web Forms, Domain sin Infrastructure y adaptador como único llamador del motor.

## CRITERIOS DE ACEPTACION

- Compilación y pruebas focales finalizan correctamente, o sus limitaciones quedan justificadas y reproducibles.
- Los escenarios críticos no generan transición duplicada, fuga de información ni pérdida de contexto.
- La evidencia permite correlacionar resultado, código público y auditoría sanitizada.
- La configuración de gate no fue modificada y el fallback continúa disponible.

## REGLAS DE ANTIRREGRESION

- Mantener intactos los contratos, la presentación y las pruebas existentes de continuar flujo mientras se valida grupo.
- Comparar contratos, códigos y payloads de `PreviewEnviarTarea` y `EjecutarEnvioTarea` contra sus pruebas existentes; no aceptar cambios no declarados.
- Confirmar que continuar flujo sigue usando `IdConector`, sus destinos por conector y su adaptador legacy actual.
- Con gate inactivo, comparar el postback legacy de enviar a grupo antes/después de la implementación y registrar cualquier diferencia como bloqueo.
- Ubicar y actualizar, o registrar explícitamente la ausencia de, la documentación existente de `TerminarGrupo` y el artefacto OpenSpec aplicable.

## PRUEBAS OBLIGATORIAS

Ejecutar MSBuild del proyecto afectado cuando sea posible; ejecutar pruebas VB/JavaScript afectadas; registrar comandos, resultados y cobertura. Ejecutar QA manual de cancelación, éxito, bloqueo, error, teclado, Escape, foco, accesibilidad y vistas responsive. Documentar por qué E2E/carga no se ejecutaron si no hay autorización.

## DOCUMENTACION TECNICA

Actualizar exclusivamente `Doc/Actualizacion/workflow/TerminarGrupo/01-implementacion-envio-grupo/04-pruebas-y-evidencia.md` con matriz, comandos, compilación, QA, evidencia, limitaciones, correlaciones y riesgos residuales; actualizar OpenSpec para reflejar la verificación. No crear una carpeta documental para esta etapa.

## ENTREGABLE FINAL

Entregar reporte verificable de archivos revisados, pruebas/compilación, evidencia QA, documentación, resultados por escenario, riesgos y recomendación fundada de continuar, bloquear o solicitar corrección.
