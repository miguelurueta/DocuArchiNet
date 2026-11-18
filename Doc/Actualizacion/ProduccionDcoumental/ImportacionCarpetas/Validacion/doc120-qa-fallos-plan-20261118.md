# Ventana QA de fallos y recuperación sobre recursos existentes

Solicitud2026-11-18: «continua con tus pruebas E2E y luego yo hago la mia al final». Ambiente autorizado localhost, propietario141/empresa2. Se reutilizan lote29 (corpus DOC119 ya preparado,27 archivos/2 raíces/version40/nivel7) y lote50 (1.000 archivos/1 raíz/version45/nivel223). Sin plan o lote nuevo, DDL, publicación, reanudación del servidor, SII, limpieza ni borrado. No se retiran originales recibidos para provocar faltantes.

## Recursos, operaciones y límites

Preflight SELECT y de interfaz, sin efectos: lote29 conserva a1 recibido/no almacenado, los dos primeros pendientes en orden de archivo_id (a10/a11 en el preflight observado), no en orden numérico del identificador cliente; segunda raíz r2 pendiente/sin identidad. La selección queda ligada por correlaciones originales, no por IDs adivinados. Lote50 conserva dos siguientes pendientes recibidos con valores fijados. Revisiones, ausencia de intentos activos/inciertos, versión, gabinete y propietario deben coincidir antes de cualquier efecto. Si cambió el corpus se detiene, sin preparar otro automáticamente.

Preparación acotada: recibir exclusivamente C (a11 en el preflight observado) del lote29,677 bytes del PDF local ya verificado por hash/fecha/nombre. Se esperan2 documentos del lote29 (A/C) y1 expediente nuevo para r2; B conserva su ausencia de original y resultado pendiente. Los otros25 archivos no cambian.

Lote50:1 almacenamiento cuya respuesta se suprime al navegador después de comprobar checkpoint RA/DA, y1 archivo sobre el que compiten dos sesiones autenticadas independientes. Se envía además una revisión antigua únicamente después de comprobar un ganador. Se esperan2 nuevos documentos; hay4 solicitudes ALMACENAR como máximo en este lote. La guardia limita las llamadas, no garantiza por sí sola ausencia de defectos del servidor; si la carrera duplica un efecto se detiene y conserva evidencia, sin borrar.

Topes globales:1200 HTTP/120 SELECT externos/3 sesiones/45min,1 recepción de677 bytes/1 CREAR_EXPEDIENTE/2 materializaciones/6 solicitudes ALMACENAR. Resultado esperado4 documentos; límite superior6 si falla la exclusión del servidor.2 llamadas ConfirmarPlan únicamente para el plan90 ya confirmado/lote50: deben converger al mismo lote, cero creaciones. Se reconstruye la revisión original y se verifica contra solicitud_hash persistido antes del POST; la revisión actual del plan confirmado no es la revisión de la solicitud original. La tercera sesión es Carolina y solo consulta/exporta recursos ajenos para verificar rechazo; jamás ejecuta efectos.

## Escenarios y evidencia

1. Consulta y confirmación repetida de plan90, mismo lote50. Esta comprobación no representa una selección nueva ni prueba todas las partes/sellado/rollback de3.118.
2. Interfaz real del lote29: A confirmado, B sin original local; el diálogo identifica nombre/ruta/fase/ID/avance, espera consentimiento y Pausar no inicia el siguiente.
3. Recarga/consulta del mismo lote: A no se repite; al continuar con los demás, B queda pendiente y C puede crear su expediente y almacenarse. Retener la respuesta de C ya confirmada permite pulsar Pausar mientras el navegador espera; la pausa se aplica después del acuse y no inicia otro archivo. Nueva recarga conserva ambos documentos y vuelve a presentar B pendiente, sin efecto sobre él.
4. Verificar relación del expediente nuevo una sola vez en el árbol y etiqueta igual al código previsto. Guardar capturas únicamente del contenido del módulo, excluidas de Git; sin cuentas, claves o cookies.
5. Suprimir entrega al navegador de la respuesta de D del lote50 tras comprobar documento/checkpoint. La interfaz se detiene; consulta/recarga demuestra el resultado persistido sin repetir D. Es un fallo de entrega de respuesta en el navegador, no pérdida de commit ni rollback simulado.
6. Dos sesiones sobre E, ventanas HTTP solapadas: un ganador y un rechazo; revisión antigua rechazada y mismo documento. No certificar solapamiento físico de conexiones MySQL o rollback sin evidencia propia de3.187.
7. Carolina no puede consultar/exportar lotes29/50. Huellas de25 y998 recursos ajenos, conteos RA/DA y ausencia de actividad/incertidumbre al cierre. Categorías Server-Timing por guardado; no sumar etapas anidadas ni comparar el corpus677 bytes con Egresos como benchmark pareado.

Ejecutor `run-doc120-qa-failures.cjs`, guardas `doc120-qa-window.cjs`,8/8 comprobaciones locales. Fuente de originales intacta: `readPrepared` DOC119. Las claves solo se usan en consola/memoria y se eliminan; JSON/PNG locales no contienen cuerpos de respuesta, valores, tokens o credenciales. Cualquier fallo termina la ventana sin replay automático. Una continuación necesita revisar resultados/consumo y los recursos ya cambiados.

## Rollback pendiente de laboratorio

Esta ventana no inyecta fallos en transacciones MySQL ni conecta al escritor con una cuenta SELECT. La prueba de rollback y cierres de3.187, y los casos restantes de partes/confirmación3.118, requieren cuenta de escritura/corpus de laboratorio y límites concretos propios. Se consultó disponibilidad de ese entorno mientras se prepara esta ventana. No dar por cerrada la aceptación integral por aprobar estos escenarios parciales.

## Primera ventana cerrada, corrección del ejecutor

Run86709c052d0c4bb9942c666e9a5a2ec5: STOPPED en CONFIRMED_PLAN_CONVERGENCE,278 HTTP/6 SELECT/1 sesión. Una reconfirmación con revisión actual devolvió409/CONFLICTO_REVISION correctamente. Cero recepciones, creaciones o solicitudes de almacenamiento. Se corrige solo el ejecutor: revisión original ligada por hash y selección B/C por orden de ID persistido. Ocho pruebas locales verifican ambos supuestos y la continuación exclusiva. El estado anterior permanece intacto; una ventana nueva enlazada necesita claves privadas nuevas porque las anteriores fueron eliminadas. No se modifica la aplicación ni su DLL.
