# DOC-120 —3.187: reservas y rollback MySQL aprobados

**PASS_FILE_RESERVATIONS_REPOSITORY**, run655467f4fa2c7933ce172836cca88ff7, código `6fc5d3430dc67d6b7c56244cd5bb491737f60f79`. Ventana2026-11-20T19:50:47Z–19:51:49Z, cerrada y claves eliminadas. [Registro primario, huellas y resultados](doc120-reservas-archivo-resultado-20261120.json).3.187 completa en el alcance nativo autorizado; **267/270**, pendientes6.4/6.5/6.6.

## Fixture y alcance autorizado

Se reutilizaron los ejecutores DOC-115/DOC-111, su login/driver oficial y el arnés Framework; perfil24-v38/nivel7/141-empresa2, destino registrado MembershipUsers. Plan94/lote57/raíz78; archivos3105/3106, dos PDF estándar de677 bytes. Un expediente697 y relación1121 creados y verificados físicamente por SELECT. Cero invocaciones al adaptador de almacenamiento y cero documentos/inventarios documentales nuevos. Sin cambios de perfiles, esquema o datos completados de los lotes29/50/52.

El propietario del plan se verificó por SELECT antes de confirmar. Tras recibir ambos PDF, el flujo oficial creó un expediente y materializó valores; no despachó ALMACENAR. Los casos posteriores se ejecutaron directamente contra el repositorio, con conexiones físicas independientes (`Pooling=false`) y barrera de inicio. No es una carrera de dos pestañas de navegador ni una nueva medición de rendimiento.

## Verificación nativa

| Caso | Resultado real |
|---|---|
| Dos conexiones sobre A con solicitudes distintas | Un ganador; la otra reserva rechazada por OPERACION_EN_CURSO. Replay conserva intento/token/hash y no crea otra reserva. |
| Revisión antigua | CONFLICTO_REVISION; hash completo del estado anterior conservado. |
| Contención del lote | B no puede reservar mientras A tiene reserva activa. |
| B activo y A con incidencia conocida | B RESERVADO bloquea A; se revierte también la limpieza provisional del negativo de A. No se ignora una reserva por ser otro archivo. |
| Incidencia sin reserva, fallo antes del evento | Se observa el resultado negativo provisional dentro del tx; rollback restaura lote/raíz/archivos/intentos/eventos. |
| Fallo tras la reserva provisional | Se observa EN_CURSO/intento activo dentro del tx; rollback completo sin nueva reserva durable. |
| Incidencia con reserva propia, fallo antes del evento | Se observa intento RECHAZADO con evidencia/fin provisional; rollback conserva la reserva previa, token, evidencia y revisiones. |
| Token ajeno | Cierre rechazado y estado exacto conservado. |
| Dos cierres anteriores a invocación | Intentos6139/6140 RECHAZADO con fin y negativo propio; token/hash originales preservados. Archivos aún recibidos y sin intento activo. |
| Cierre de ventana | Ningún intento RESERVADO/EN_CURSO/INCIERTO en57; ningún evento V2 de inicio/autorización/retorno/resultado para3105/3106. |

B activo significa aquí una reserva durable RESERVADO, condición de exclusión del contrato. No se invoca al adaptador documental para simular una ejecución prolongada. Las dos incidencias `ORIGINAL_NO_COMPROBADO` son deliberadas de laboratorio y permanecen con su historial; no se retiraron PDF ni se borraron registros para limpiar el resultado.

## Conservación y consumo

Los17 grupos de datos anteriores tienen huellas iguales antes/después: planes, lotes, raíces, archivos, intentos, eventos, preparaciones y sus partes/resultados, perfiles/versiones/reglas/permisos, expedientes, relaciones y conteos documentales. Del fixture se conservan otros5 grupos: originales de lote/raíz/archivo (incluidos recepción/valores/identidad), tres intentos previos de recepción/creación y su diario.154 fuentes/dependencias y backend `b0b5d0d329f62124aa45ea49248746b90ad40c824a8a0a6adf73291d5d39dc4f` sin cambios. Nuevos registros:1 plan/1 lote/1 raíz/2 archivos/5 intentos;13 eventos de preparación y7 de los casos nativos.

Consumo:256 HTTP/1 sesión para preparar el fixture;23 llamadas nativas al repositorio y52 controles SELECT nativos. El envoltorio empleó73 SELECT, incluido1 del fixture: **125 controles externos** de160 permitidos, sin sumar dos veces ese SELECT. Los SELECT internos del producto no se midieron como controles externos.35/35 pruebas locales del ejecutor y2/2 envoltorios Framework/DDL aprobadas previamente; no se suman internos o corridas superpuestas.

Diario SHA256 `404603b01175399ca9230160a4b746d068ef86e57433326ef83e3ec36d566bda`; manifiesto `ccc1fb4c9c586ed837a35d7965842dea289e96d1caccc590b9819ce466513b72`. Las ventanas anteriores4a3c1b2e (parada sin reservas) y83b36216 (inspección SELECT) conservan sus resultados; no se reinterpretan como carreras aprobadas ni se reabren. Aceptación de límites, revisión formal de SHA e integración final siguen en6.4–6.6.

Validación documental: OpenSpec estricto, diff y auditoría local de refinement15/15 aprobados. El estado de aplicación confirma267 completas/3 pendientes. Esto no sustituye la revisión formal6.5 ni declara aceptación global del cambio.
