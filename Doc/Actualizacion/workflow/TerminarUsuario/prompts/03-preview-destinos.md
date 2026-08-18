# 03 — Preview seguro de destinos de usuario

## ROL ESPERADO

Actúa como desarrollador senior de ASP.NET Web Forms, ASMX, MySQL y seguridad de workflows.

## OBJETIVO

Agregar `PreviewEnviarUsuario(idTarea)` al `WebServiceWorkflowModern.asmx` existente y resolver destinos directos de usuario mediante consultas exclusivamente de lectura.

## RESTRICCIONES CRÍTICAS

- Lee y aplica `prompts/00-contexto-obligatorio.md`.
- El preview no adquiere lock, no invoca motor legacy, no ejecuta eventos, no registra auditoría y no altera estados ni respuesta.
- Usar el ASMX moderno existente; no crear otro ASMX ni modificar `PreviewEnviarTarea`.
- No reutilizar `MySqlTransicionRutaRepository`: representa conectores salientes, no la lista directa de usuarios de la ruta.
- Repositorios con SQL parametrizado, modelos tipados y sin Session, `DataSet` ni HTML.
- El gate se evalúa en servidor y falla cerrada; no modificar configuración.

## REQUISITOS POSITIVOS

1. Crear repositorio y servicio de destinos que repliquen la semántica autorizada de `Solicita_listado_usuarios_workflow_ruta`: usuario activo, `UTIL_ASIGNA_TAREA=1`, actividad y ruta vigentes.
2. Validar contexto, `CAMBIO_USUARIO`, tarea activa y acceso del usuario actual.
3. Rechazar ruta cerrada y, cuando aplique, flujo o actividad de flujo cerrados.
4. Consultar el estado de respuesta solo para clasificarlo; si requiere confirmación/radicado, devolver bloqueo `RESPUESTA_PENDIENTE_REASIGNACION_NO_SOPORTADA` o código equivalente, sin reasignar.
5. Retornar contexto sanitizado, token y destinos mínimos para confirmación.

## CRITERIOS DE ACEPTACIÓN

- El endpoint solo ejecuta SELECT, sin estado, auditoría ni efectos de motor.
- No expone usuarios ni actividades fuera de contexto, permiso y gate permitidos.
- La lista solo incluye pares usuario–actividad válidos de la ruta y no conectores de Continuar flujo.
- `PreviewEnviarTarea` conserva contrato y resultados.

## PRUEBAS OBLIGATORIAS

Agregar y ejecutar pruebas de preview válido, gate inactivo, permiso negado, tarea inaccesible, ruta/flujo cerrado, respuesta pendiente, sin destinos, usuario inactivo, usuario sin `UTIL_ASIGNA_TAREA` y usuario de otra ruta. Ejecutar MSBuild y pruebas focales; no E2E ni carga.

## DOCUMENTACIÓN TÉCNICA

Crear o actualizar `Doc/Actualizacion/workflow/TerminarUsuario/03-preview-destinos/` con contrato JSON, secuencia de solo lectura, consultas permitidas, códigos, matriz de pruebas y diagramas necesarios.

## ENTREGABLE FINAL

Entregar endpoint, servicio, repositorio, payload JSON, pruebas/compilación, evidencia de no escritura, documentación y riesgos. No implementar ejecución ni UI en esta etapa.

