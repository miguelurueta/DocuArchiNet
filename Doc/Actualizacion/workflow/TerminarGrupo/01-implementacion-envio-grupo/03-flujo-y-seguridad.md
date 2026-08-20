# Flujo, seguridad y rollback

- Ticket: DOC-15
- Cambio OpenSpec: doc-15-base-enviar-grupo
- Clasificacion: cross_cutting

## Preview

El ASMX recrea el contexto autenticado, calcula `Cambio_Ruta`, evalúa el gate existente y consulta solo actividades de la ruta mediante `SELECT`. No adquiere lock, no audita, no ejecuta eventos ni motor legacy.

## Ejecución

El servicio valida solicitud, toma `GET_LOCK` por tarea y versión, vuelve a verificar permiso, tarea, token, ruta/flujo/actividad y destino. Después valida aprobaciones pendientes y delega una sola vez al adaptador directo.

El adaptador llama `Terminar_Tarea_Workflow` con `Page = Nothing`, sin conector ni identificadores de flujo. No añade validación de respuesta radicada.

## Gate, auditoría y rollback

Se reutiliza exclusivamente `WorkflowCentroTrabajoModernActive` y el gate existente. Con gate inactivo el botón conserva el postback legacy. La auditoría usa `Canal=MODERNO`, `Mecanismo=ASMX_ENVIO_GRUPO` e `IdConector=0`; una advertencia posterior no revierte éxito confirmado.

No existe autorización para activar ambientes. El rollback autorizado es gate inactivo, usuarios/grupos vacíos y retorno del nuevo intento al flujo legacy; no revierte transiciones confirmadas.

## DOC-26 — Consulta de destinos

Al abrir el modal, el preview entrega solo la primera página. Escribir dos o más caracteres espera 300 ms y llama `BuscarDestinosEnvioGrupo`; limpiar restablece página uno. La UI usa `AbortController` cuando el navegador lo ofrece y una secuencia monotónica para descartar respuestas tardías. Un nuevo filtro, página, reintento o preview invalida la selección y cierra una confirmación basada en resultados anteriores.

El servidor vuelve a validar sesión, gate existente, `Cambio_Ruta`, tarea activa y ruta o flujo aplicable. La consulta se restringe a la ruta autorizada y usa solo `SELECT` parametrizados para `@idRuta`, `@termino`, `@limite` y `@desplazamiento`. Agrupa por actividad y filtra grupos con `EXISTS`, por lo que una actividad con varios grupos se muestra una sola vez. Se obtiene una fila adicional para `TieneMas`; no se ejecuta `COUNT(*)` por interacción ni se modifica el esquema.
