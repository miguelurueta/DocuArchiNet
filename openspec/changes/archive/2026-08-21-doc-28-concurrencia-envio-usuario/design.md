## Context

Ver `proposal.md` y la especificación de esta modificación. DOC-28 ya protege la ejecución mediante token y `GET_LOCK`, y el paquete E2E contiene antecedentes de concurrencia DOC-11. Sin embargo, el comando DOC-28 actual solo cubre una ejecución y prohíbe toda concurrencia. La nueva cobertura debe comprobar la exclusión mutua sin convertirse en una prueba de carga ni debilitar el aislamiento de secretos, gate o flujo legacy.

## Goals / Non-Goals

**Goals:**

- Exponer un único comando de carrera DOC-28 que solo se habilite con dos autorizaciones exactas: ejecución y concurrencia.
- Construir el payload de ambas solicitudes con destino y token de un preview vigente de la misma tarea y la misma sesión de prueba.
- Medir, mediante consultas `SELECT` de un parámetro, que hay una sola transición efectiva y trazabilidad posterior.
- Conservar evidencia agregada, reproducible y libre de secretos.

**Non-Goals:**

- No implementar niveles configurables, carga sostenida, métricas de rendimiento ni concurrencia de preview.
- No añadir identificadores de destino, token, usuario, grupo, ruta o actividad a variables de entorno.
- No recuperar, reiniciar ni preparar tareas; el ambiente debe entregar una tarea descartable lista.
- No modificar contratos ASMX, `GET_LOCK`, el motor legacy, gate ni páginas legacy.

## Decisions

### Carrera fija de dos solicitudes

El runner creará dos `BrowserContext` autenticados y realizará un preview en uno de ellos. Con el destino y token devueltos, enviará exactamente dos `EjecutarEnvioUsuario` en un `Promise.all` contra la misma tarea. La cardinalidad fija evita transformar la prueba de exclusión mutua en carga masiva y hace inequívoca la expectativa de una ganadora.

Se descarta reutilizar el runner DOC-10, porque mide preview de solo lectura y sus niveles configurables no demuestran la semántica mutante de usuario. También se descarta aceptar dos tokens o destinos configurados, ya que permitiría falsear la prueba de carrera.

### Autorización doble y validación previa

El validador DOC-28 tendrá un modo `concurrency` separado. Requerirá URL, cuenta Gestión, tarea positiva, conexión y consultas MySQL de lectura, más `DOC28_E2E_EXECUTION_AUTHORIZED=true` y `DOC28_E2E_CONCURRENCY_AUTHORIZED=true`. Validará todo antes de crear navegador o enviar HTTP.

La doble autorización diferencia una ejecución individual de una carrera que puede crear dos intentos auditados. Se descarta usar solo la autorización de ejecución porque hace demasiado fácil ejecutar una concurrencia accidental.

### Resultado y postcondición de exclusión mutua

El runner contará respuestas sin imprimir DTOs completos. Solo aceptará una respuesta exitosa con estado final `completada`; la otra deberá devolver `WORKFLOW_TRANSITION_IN_PROGRESS`, `WORKFLOW_VERSION_CONFLICT` o `WORKFLOW_TASK_UNAVAILABLE`. Comparará huellas de estado y auditoría antes/después: el estado debe cambiar, la auditoría debe cambiar y el conteo de éxitos debe ser uno.

La combinación de una única respuesta exitosa y una sola huella de estado posterior protege contra una doble transición sin depender de detalles privados de tablas. Se descarta inferir éxito por HTTP 200, porque los bloqueos funcionales también usan esa respuesta.

### Evidencia y cierre defensivo

El runner escribirá solo nombre de endpoint, cantidad fija de solicitudes, conteo de éxitos, códigos de bloqueo, estado final de la ganadora, banderas y huellas. Antes y después comprobará que la configuración local del gate continúa apagada y sin alcance; también dejará a la guía el control de páginas legacy.

Se desactivarán captura, video y trazas en la ruta DOC-28. Se descarta incluir destinos, tokens, cookies, conexiones, cuerpos de respuesta, rutas de base o métricas de login en la evidencia.

## Risks / Trade-offs

- [La tarea puede cambiar entre preview y carrera] → la corrida falla con un bloqueo permitido y evidencia el resultado; nunca vuelve a enviar usando datos de configuración.
- [El segundo intento registra auditoría adicional] → la aserción exige cambio de auditoría, no un conteo fijo de filas, porque el adaptador puede advertir sobre trazabilidad.
- [El ambiente no entrega una tarea descartable] → el validador no puede convertir una tarea en descartable; detener la corrida hasta autorización del responsable.
- [Dos inicios de sesión simultáneos introducen ruido] → los contextos se crean antes de disparar la carrera y no se usan para medir rendimiento.
- [Una futura petición de carga necesita más presión] → se abrirá un cambio distinto con límites, aprobación y métricas de rendimiento; este cambio conserva exactamente dos solicitudes.

## Migration Plan

1. Incorporar modo, comando, runner y pruebas estáticas sin ejecutar autenticación ni concurrencia real.
2. Documentar los requisitos de autorización doble, tarea descartable y cierre.
3. En un ambiente autorizado, ejecutar una sola carrera con tarea preparada y conservar la evidencia saneada.
4. Si se debe retirar la cobertura, eliminar únicamente el comando y runner DOC-28; no hay cambios de API, esquema ni configuración desplegada.
