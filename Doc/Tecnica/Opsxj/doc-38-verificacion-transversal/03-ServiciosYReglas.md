# Servicios y reglas — Verificación transversal

- Ticket: DOC-38
- Cambio OpenSpec: doc-38-verificacion-transversal
- Clasificacion: cross_cutting

## Servicios y reglas

La verificación revisa `ServicioDevolverUsuarioAnterior` y sus contratos e infraestructura sin modificar su comportamiento. El preview debe conservar consultas `SELECT` parametrizadas; el destino se resuelve exclusivamente desde el historial inmediato de la misma tarea y el navegador no aporta usuario, actividad, grupo, Ruta, Flujo ni identificador de historial.

El token opaco vincula tarea, estado y antecedente confirmado. La ejecución adquiere un lock exclusivo por tarea y, dentro de él, relee permiso, tarea, snapshots e historial antes de una sola mutación. El análisis cubre bloqueo por historial ausente o grupal, usuario retirado, auto-devolución, token inválido o vencido, historial cambiado y lock ocupado.

El único límite mutante permitido es el adaptador dedicado a `Terminar_Tarea_Workflow`: usa `Page = Nothing`, interfaz legacy, notificaciones y eventos dinámicos desactivados. Ninguna capa nueva puede tratar respuestas, radicados o confirmaciones; la auditoría registra la acción funcional con datos saneados y no revierte una transición ya exitosa por una advertencia posterior.
