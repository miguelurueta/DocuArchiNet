# 02 — Autorización, preview y búsqueda paginada

## ROL ESPERADO

Actúa como desarrollador senior VB.NET de capas Domain, Application e Infrastructure para Workflow.

## OBJETIVO

Implementar el corte de servidor: autorización de devolución, `PreviewDevolverActividad` de solo lectura y búsqueda paginada de actividades anteriores autorizadas.

## CONTEXTO OBLIGATORIO

- Requiere 01 aprobado y lectura de `00-contexto-obligatorio.md`.
- La salida habilita 03; no habilita ejecución, UI ni liberación.

## REQUISITOS POSITIVOS

- Crear DTOs, modelos, puertos y códigos públicos exclusivos de devolución.
- Exponer `PreviewDevolverActividad(idTarea, termino?, cursor?, tamanoPagina?)` en el ASMX moderno existente.
- Validar contexto, tarea activa, permiso de devolución, Ruta o Flujo y pertenencia del conector entrante antes de devolver resultados.
- Para Flujo, reducir al conjunto de conectores cuyo destino sea la actividad actual del flujo y pertenezcan al flujo y usuario/grupo reales.
- Para Ruta, reducir al conjunto de actividades predecesoras de la actividad actual y pertenecientes a la ruta de la tarea.
- Filtrar y paginar en servidor con `SELECT` parametrizado, límite, orden estable, cursor seguro y datos mínimos.
- Permitir búsqueda por actividad, usuario o grupo sobre el universo ya autorizado; aplicar longitud mínima y tamaño máximo de página.

## RESTRICCIONES CRÍTICAS

- No ejecutar el motor, lock, auditoría, eventos ni cambios de tarea o datos de negocio.
- No crear UI, ejecución, banderas de habilitación ni una lista completa en cliente.
- No modificar `PreviewEnviarTarea`, `EjecutarEnvioTarea`, `PreviewEnviarGrupo`, `EjecutarEnvioGrupo` ni Continuar flujo.
- No tratar respuestas en ningún caso.
- No ejecutar E2E autenticada ni carga sin autorización explícita.

## REGLAS DE ANTIRREGRESIÓN

- El preview no revela conectores o actividades fuera del contexto autorizado.
- Cursor, término o identificador manipulados producen resultado público seguro y sin escritura.

## CRITERIOS DE ACEPTACIÓN

- La primera página y las siguientes se resuelven desde el servidor sin materializar el catálogo completo en navegador.
- Tarea inválida, permiso ausente, Ruta/Flujo inconsistente, cursor inválido o lista vacía devuelven bloqueo funcional estable.
- La respuesta no incluye datos de respuesta, SQL, sesión, HTML ni controles Web Forms.

## PRUEBAS OBLIGATORIAS

Agregar pruebas focales para permiso, preview sin escritura, Ruta, Flujo, filtro parametrizado, límite, orden, cursor, término mínimo, lista sintética extensa y ausencia de componentes de respuesta. Ejecutar compilación disponible y pruebas afectadas; no E2E.

## DOCUMENTACIÓN TÉCNICA

Actualizar arquitectura, contrato, flujo de seguridad y evidencia del paquete documental de DevolverTarea.

## ENTREGABLE FINAL

Reportar ticket, archivos, pruebas, compilación, evidencia de no escritura y riesgos. No implementar adaptador, ejecución ni UI.
