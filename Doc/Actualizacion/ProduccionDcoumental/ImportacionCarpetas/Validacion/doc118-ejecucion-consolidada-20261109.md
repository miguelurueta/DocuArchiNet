# DOC-118 — ejecución consolidada y corrección del ejecutor

**4.1 completada; 29/38 tareas. COORD-02/03 aún sin aceptación.** La instrucción de continuar sin pedir autorizaciones repetidas se ejecutó con el motor existente y una ventana finita adicional en el ambiente/cuenta ya documentados. El histórico 1277 HTTP/113 SELECT/9 sesiones se conserva; la ventana consumió 301 HTTP/34 SELECT/2 sesiones. Acumulado 1578/147/11. Se agotó su cupo de dos sesiones; no se reinicia ni reutiliza.

## Resultado real

PREFLIGHT pasó con dos sesiones independientes y protección de los lotes 18/21/24. PREPARE02 creó plan 41/lote 25, raíces 41/42, archivos 42/43, expedientes 672/673 y relaciones 1097/1098. Ambos PDFs quedaron recibidos y sus valores materializados; había cero documentos antes del almacenamiento. Las raíces son nuevas por runId y las etiquetas sintéticas PDF conservan compatibilidad con el SDK.

El observador quedó calificado dentro de ClassAlmacenamiento.AlmacenamientoDocumentoProduccionDocumental: fuente y símbolos fijados, worker 31784/hilo 1608, una entrada A y cero B. Entre 2026-11-09T21:51:42.8153580Z y 2026-11-09T21:51:43.4001627Z permaneció congelado el hilo seleccionado con los demás hilos activos. Una consulta protegida de la segunda sesión respondió y el INSPECT posterior acreditó la misma llamada interna. Esto satisface 4.1; no acredita concurrencia natural ni rendimiento.

SAME_COMMAND obtuvo la respuesta de bloqueo RESULTADO_INCIERTO aceptada por el helper. La inspección posterior detuvo el recorrido antes de OTHER_FILE; el resultado de la primera petición continuó UNKNOWN. ALMACENAR=2 cuenta la petición original y su contendiente, no dos efectos confirmados. La herramienta antigua perdió la categoría exacta del error; no se atribuye a reloj, aplicación o base de datos sin prueba. No se ejecutaron FINISH02, PREPARE03, RUN03 ni VERIFY y no se acredita un checkpoint o recuperación del retorno perdido.

Se liberó el hilo a 2026-11-09T21:51:43.9942676Z; SDK CLOSED/cleanupConfirmed=true, ambas sesiones cerradas, secretos efímeros de la ejecución eliminados y lease liberado. El lote 25 se conserva protegido sin reenvío, limpieza, cambio de estado o recuperación inferida. El efecto documental sigue pendiente de lectura durable. El staging/artefactos permanecen locales excluidos de Git.

## Corrección y verificación

Se conservaron el motor de siete fases, login, transporte, controles SELECT y SDK existentes. El selector --current-window valida pin actual, procedencia, recursos frescos y suma de contadores; despacha las fases autorizadas sin confirmaciones por fase y se detiene ante incertidumbre. El modo histórico conserva sus condiciones. La autorización original queda intacta; el manifiesto adicional registra consumo y parada.

El ejecutor ahora conserva únicamente categorías cerradas del observador en failureCode. Tras RESUME espera hasta 35 s a la petición original, cubriendo su plazo HTTP de 30 s y la lectura del checkpoint; antes esperaba solo 1 s. La pausa máxima no aumenta. Si no se obtiene resultado sigue UNKNOWN, sin retry ni nueva llamada. Un caso local reproduce una respuesta que tarda más de un segundo después de la liberación y verifica que se espera y se conserva su diagnóstico. No se presenta esta corrección como solución probada del fallo real.

La auditoría SELECT existente admite las referencias retenidas del lote nuevo y distingue nombres de raíz de etiquetas PDF. Rechaza expedientes/identidades ajenas e intentos históricos, verifica checkpoint concordante antes de confirmar IDs y conserva ausencia física separada de la autorización de replay. Su consola requiere solo las dos entradas SELECT enmascaradas; no realiza login de aplicación ni mutaciones. Estado al consolidar: **AWAITING_SELECT_CREDENTIALS**. La ausencia de esas entradas no se interpreta como resultado de base de datos.

Regresión completa final: **260/260 Node**, cero fallos/omitidos; 157 casos V2 internos incluidos sin sumarlos a Node. Los 13 casos añadidos respecto de 247 cubren ámbito/contadores/evidencia, preparación nueva, espera/diagnóstico y auditoría vinculada. Se conservaron las huellas originales de fuentes/históricos y se registran los deltas de herramientas desde 142f7f7c101eb8ffbaba817e646a511aa611c028. Función compartida de almacenamiento, SII, adaptadores protegidos y DLL/PDB permanecen intactos; no hubo otro despliegue.

Quedan nueve tareas: 4.3/4.4/4.5/4.7/4.8 y 5.1–5.4. El siguiente dato necesario es el resultado SELECT del lote 25. No se archiva DOC-118 ni se declara terminada la integración. Evidencia saneada: doc118-ejecucion-consolidada-20261109.json.

Consulta adicional de metadatos IIS, sin red ni datos privados: en la ventana temporal se observaron POST AvanzarLote con HTTP200/409/200 y duraciones116196024/125/4183ms, respectivamente; el primero tuvo Win32=64. Se leen solo hora/estado/duración, sin identificar comando o recurso. Por ello no se atribuye cada entrada a un efecto ni se infiere checkpoint por HTTP200. La consulta SELECT sigue siendo necesaria.
