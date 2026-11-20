# DOC-120 — ventana nativa de reservas por archivo (3.187)

**Corte vigente:** el usuario autorizó el fixture adicional;655467f4 cerró PASS_FILE_RESERVATIONS_REPOSITORY sobre el lote nuevo57, tres rollbacks/carrera/exclusión/cierres verificados. [Resultado final](doc120-reservas-archivo-resultado-20261120.md).3.187 completa,267/270, sin almacenamiento documental. La parada/inspección y las propuestas inferiores son historia; no ordenan repetir pruebas ni solicitar de nuevo una autorización ya consumada.

## Resultado de la primera ventana y revisión

Inspección83b36216 cerrada `PASS_READONLY_INSPECTION`,7 SELECT/0 conexiones escritoras/0 mutaciones, claves eliminadas. Confirma lote50 **1.000/1.000 RECIBIDO/COMPLETADO**;122/123 ya tienen resultados confirmados. Lote29:2 completados y25 PENDIENTE/NO_INICIADO, sin valores fijados. No hay candidatos recibidos y pendientes para la carrera. [Resultado y huellas](doc120-reservas-archivo-inspeccion-20261120.json). No resetear resultados ni reutilizar archivos completados para fabricar una reserva.

La ventana aprobada excluía recepciones y creaciones. Para completar3.187 hace falta autorizar un fixture E2E pequeño adicional: perfil24/publicación vigente y nivel7 ya existentes,141/2, reutilizar el PDF estándar `tools/e2e/fixtures/doc111-carga-individual.pdf` del arnés (677 bytes por archivo), dos inventarios/recepciones y una raíz/expediente confirmado; cero almacenamiento documental. Un plan/lote de prueba adicional, con fuentes, nombres y topes sellados antes de ejecutar; conservación de todo resultado previo. **Esta alternativa no está ejecutada ni autorizada por la ventana anterior.** El arnés de reservas queda preparado sobre el alcance50/122-123; su adaptación al fixture adicional y la ejecución necesitan ese nuevo alcance, sin marcar3.187.

Run4a3c1b2e, código86f6f9d0, cerrado `STOPPED_REVIEW_REQUIRED/DOC120_FILES_NOT_PENDING` a2026-11-20T19:19:56Z. Los dos IDs no cumplen conjuntamente RECIBIDO/NO_INICIADO/sin resultado/sin intento; el diagnóstico no permite inferir cuál condición cambió.33 SELECT del envoltorio y3 lecturas nativas (lote y dos archivos) antes de la guarda; no se alcanzó ningún método de reserva/incidencia/adaptador,0 referencias. Sin postflight global ni atribuir conservación observada a toda la BD. Claves eliminadas, ventana cerrada y no reejecutable. [Registro público](doc120-reservas-archivo-parada-20261120.json).

Se agrega al mismo ejecutor `--repository-files-inspect`, un modo de diagnóstico separado y ligado a `inspectOnly=true` en manifiesto nuevo. Solo solicita cuenta SELECT, verifica destino registrado y consulta estados/candidatos de los lotes E2E29/50, propietario141/empresa2; máximo12 SELECT, cero conexiones escritoras o métodos mutantes. No elige automáticamente otros recursos para reservar.33/33 pruebas del ejecutor tras añadir esta guarda, sin sumar las32 previas superpuestas; los2 envoltorios Framework/DDL permanecen aprobados y el C# no cambió.3.187 sigue pendiente hasta recursos válidos y una ventana nativa autorizada con resultado positivo.

Preparación solicitada por el usuario: verificar reservas por archivo, escritor activo y rollback de incidencias en MySQL. Se amplían las entradas existentes `run-doc115-recepcion-importacion.cjs` y `run-doc111-persistence.ps1`, el helper nativo07A y su parcial `ImportacionCarpetasRepositoryIntegrationTests.cs`. Se conservan las ventanas cerradas anteriores. **Sin ejecución MySQL acreditada todavía; 3.187 permanece pendiente y el avance es266/270.**

## Recursos y límites concretos

Ambiente de pruebas registrado en `MembershipUsers`, con la misma huella de destino aprobada para la integración anterior. Usuario141/empresa2; lote E2E50, plan90, perfil28/version45 congelada, nivel223 y gabinete10. Archivos122 (A) y123 (B), previamente usados como corpus de pruebas. Se exige que sigan RECIBIDO/NO_INICIADO, sin resultado ni intento activo, y que no exista otro intento activo/incierto en el lote. Si cambió alguna condición, detener antes de mutaciones y revisar; no buscar otros IDs ni reabrir una ventana cerrada.

Es el lote existente de las E2E y mediciones anteriores, con raíz/expediente ya confirmados. Esta variante evita preparar otra importación o inventar resultados documentales para habilitar la reserva. La prueba segmentada anterior usó perfil24/nivel7 y no se repite.

Máximos fijos:48 llamadas al repositorio,64 SELECT externos nativos y96 del envoltorio,15 minutos de admisión por etapa;2 reservas confirmadas,2 cierres anteriores a invocación,2 intentos nuevos y4–7 eventos nuevos (incluida consolidación). Los SELECT internos del repositorio están dentro de esas llamadas y no se presentan como parte del límite de160 controles externos. Cero HTTP, sesiones de navegador, recepción de PDF, invocaciones al adaptador documental, documentos, expedientes, planes, lotes, DDL, cambios de perfil o borrados.

## Secuencia verificable

1. Fallo controlado antes de insertar el evento de incidencia de A sin reserva: la fila provisional existe dentro de la transacción; después del fallo, el hash de lote/raíz/archivos/intentos/eventos vuelve exactamente al anterior.
2. Fallo después de reservar provisionalmente A: mismo contraste transaccional y rollback completo.
3. Revisión antigua rechazada sin cambiar datos. Dos llamadas nativas, conexiones físicas independientes (`Pooling=false`) y barrera de inicio compiten por A con solicitudes distintas: un ganador y un rechazo. Replay del ganador conserva intento/token/hash y no constituye otra reserva. B no puede reservar mientras A mantiene su reserva.
4. Fallo al registrar incidencia con reserva propia de A: se comprueba cierre provisional del intento y rollback de token, evidencia, revisiones y diario. Un token ajeno tampoco puede cerrarla.
5. Cerrar A mediante el método nativo antes de invocación. Reservar B y comprobar que A sigue bloqueado aunque su incidencia individual sea reconocida; la limpieza provisional de ese negativo también revierte al rechazar el avance. Cerrar B con su token y revisión.
6. Releer ambos intentos: RECHAZADO con evidencia negativa, fin, token y hash originales. Ambos archivos siguen recibidos, sin intento activo ni resultado documental; ningún evento V2 de inicio/autorización/retorno/resultado para esos IDs. Conservar los resultados confirmados y los otros998 archivos, perfiles, versiones, permisos, planes, preparaciones y los registros anteriores.

La reserva B en estado RESERVADO es una exclusión durable de escritor activo según el contrato. No se invoca un adaptador para mantenerlo bloqueado ni se acredita un almacenamiento real nuevo. `ORIGINAL_NO_COMPROBADO` es una incidencia deliberada de laboratorio: no se elimina ni altera ningún PDF para provocarla. Al finalizar quedan las dos incidencias y su historial; no se borran ni se realiza un almacenamiento para limpiarlas.

## Ejecución y evidencia

El manifiesto privado se conserva en `.tmp/doc120-files/<run>/resources.json`, con huellas del backend y de las fuentes compiladas/ejecutores. `--repository-files --session <manifest> --check` verifica archivos/huellas/destino registrado sin abrir BD ni pedir claves. La ejecución utiliza la consola privada existente: cuenta SELECT y cuenta escritora de pruebas, ambas enmascaradas y solo en memoria. El estado de consola de uso único y el diario `tools/e2e/artifacts/doc120-files-<run>.json` no contienen secretos. Una parada conserva referencias y exige revisión; no hay replay automático.

La ventana real necesita autorización concreta del ambiente/cuentas/recursos y entrada privada de las claves; este documento y las pruebas locales no sustituyen esa ejecución. No se marca3.187 por compilar o por pasar dobles locales. Los límites de capacidad y cierre global siguen en6.4–6.6.

Validación local de preparación:32/32 pruebas Node del ejecutor y2/2 envoltorios Framework/DDL; las guardas nativas previas a conexión son casos internos del mismo arnés y no se suman como suites adicionales. No hay MySQL/HTTP en esas pruebas.
