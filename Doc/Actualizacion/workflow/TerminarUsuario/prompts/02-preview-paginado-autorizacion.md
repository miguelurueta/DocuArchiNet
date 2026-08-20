# 02 — Autorización, preview y búsqueda paginada

## ROL ESPERADO

Actúa como desarrollador senior VB.NET de capas Domain, Application e Infrastructure para Workflow.

## OBJETIVO

Implementar el primer corte funcional de servidor: contratos exclusivos de usuario, autorización `CAMBIO_USUARIO`, `PreviewEnviarUsuario` exclusivamente de lectura y búsqueda paginada de destinos autorizados.

## CONTEXTO OBLIGATORIO

- Requiere ticket 01 aprobado y que el ticket actual enlace este archivo.
- Leer `00-contexto-obligatorio.md`, evidencia de 01 y la exploración de Enviar a usuario.
- La salida habilita 03; no habilita UI, activación ni liberación.

## REQUISITOS POSITIVOS

- Crear DTOs, modelos, puertos y códigos públicos exclusivos de usuario, sin `IdConector`.
- Calcular `CAMBIO_USUARIO` en servidor, fail-closed y sin aceptar permisos del navegador.
- Exponer `PreviewEnviarUsuario(idTarea, consulta?, cursor?, tamanoPagina?)` solo en el ASMX moderno existente.
- Reducir primero a destino usuario–actividad autorizado y luego filtrar con `SELECT` parametrizado: tarea activa del usuario actual, ruta/flujo abiertos, usuario activo, actividad/ruta vigentes, `UTIL_ASIGNA_TAREA=1` y respuesta permitida.
- Limitar página en servidor, ordenar establemente, devolver cursor seguro, `hayMas`, token y datos mínimos para seleccionar.

## RESTRICCIONES CRÍTICAS

- No ejecutar motor legacy, lock, auditoría, eventos ni cambios de tarea, respuesta o configuración.
- No crear directorio global, UI, ejecución, gate nuevo ni lista completa en cliente.
- No modificar `PreviewEnviarTarea`, `EjecutarEnvioTarea`, `SolicitudTransicionWorkflow`, `DestinoTransicionDto` ni Continuar flujo.
- No ejecutar E2E autenticado, carga ni activar gate.

## REGLAS DE ANTIRREGRESIÓN

- Preview solo ejecuta `SELECT` y no revela destinos fuera del conjunto autorizado.
- Continuar flujo conserva endpoints, `IdConector`, payloads y adaptador existentes.

## CRITERIOS DE ACEPTACIÓN

- Gate, contexto, permiso, tarea, ruta/flujo, respuesta y destino se validan antes de devolver resultados.
- Cursor inválido, token vencido, usuario fuera de ruta o lista extensa producen resultado público seguro, sin fuga ni escritura.

## PRUEBAS OBLIGATORIAS

Agregar pruebas focales para permiso, preview sin escritura, respuesta pendiente, destino inactivo/fuera de ruta, `UTIL_ASIGNA_TAREA=0`, filtro parametrizado, cursor, límite, orden y lista sintética extensa. Ejecutar MSBuild disponible y pruebas afectadas; registrar comando, resultado y limitaciones. No E2E.

## DOCUMENTACIÓN TÉCNICA

Actualizar `01-arquitectura.md`, `02-contrato.md`, `03-flujo-y-seguridad.md` y `04-pruebas-y-evidencia.md` del paquete único con endpoint, payload, límites, privacidad, evidencia y relevo a 03.

## ENTREGABLE FINAL

Reportar ticket, archivos, pruebas, compilación, evidencia de no escritura y riesgos. No implementar adaptador, ejecución, UI ni activación.
