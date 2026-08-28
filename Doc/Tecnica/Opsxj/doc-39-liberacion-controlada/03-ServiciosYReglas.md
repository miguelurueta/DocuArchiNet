# Servicios y reglas de liberación

- Ticket: DOC-39
- Cambio OpenSpec: doc-39-liberacion-controlada
- Clasificacion: cross_cutting

## Servicios y reglas

La liberación conserva los servicios existentes `PreviewDevolverUsuarioAnterior` y `EjecutarDevolverUsuarioAnterior`. El preview realiza únicamente lecturas parametrizadas y resuelve el antecedente de la misma tarea en servidor. La ejecución revalida token, historial y permiso dentro de un lock exclusivo por tarea antes de una única mutación a través del adaptador dedicado de `Terminar_Tarea_Workflow`.

Las reglas que el artefacto aprobado debe preservar son las siguientes:

- El navegador no aporta usuario destino, actividad, grupo, ruta, flujo ni identificador de historial.
- Un antecedente sin usuario individual de Workflow, un usuario retirado, una auto-devolución, un token inválido o un lock ocupado produce bloqueo funcional sin segunda transición.
- El adaptador usa `Page = Nothing` y mantiene desactivadas la actualización de interfaz legacy, las notificaciones y los eventos dinámicos definidos para esta capacidad.
- Ninguna capa de Usuario anterior trata respuestas, radicados o confirmaciones; no hay referencias a `Classgestionrespuesta`, `Verifica_respuesta_*` ni `Reasigna_respuesta_envia_tarea_usuario`.
- La auditoría de éxito no revierte una transición ya realizada si un registro posterior falla; ese resultado se comunica como advertencia saneada.

DOC-39 no invoca estos servicios ni modifica sus reglas. Su control de liberación verifica la trazabilidad de esas garantías mediante evidencia aprobada y, solo durante una ventana futura autorizada, consultas `SELECT` aprobadas. Las consultas no reciben ni exponen credenciales, tokens, cookies, cadenas de conexión ni datos de sesión.

## Operación y contención

Si un control previo detecta una versión distinta, un contrato no identificado o evidencia crítica incompleta, la operación no inicia y el responsable escala la diferencia al propietario de la liberación. Si tras un despliegue autorizado se requiere reversión, se usa la gestión de despliegue aprobada para restaurar el artefacto anterior; no se usa la interfaz Workflow ni SQL mutante para corregir tareas ya procesadas.
