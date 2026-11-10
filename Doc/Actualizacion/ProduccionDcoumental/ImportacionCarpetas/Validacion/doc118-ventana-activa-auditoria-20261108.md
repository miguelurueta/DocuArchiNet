# DOC-118 — ventana detenida y auditoría del intento69

La ampliación9 aperturas/1500 HTTP acumulados fue autorizada por «continua». La disponibilidad sumó1 HEAD (996 acumulados) y detectó worker31784. PREFLIGHT pasó con dos contextos independientes:1268 HTTP/57 SELECT/9 aperturas. PREPARE02 recuperó plan40/lote24 por lecturas, verificó ausencia física y conservó raíces39/40, archivos40/41 y expedientes670/671:1270 HTTP/75 SELECT. No se repitió preparación, recepción, creación o materialización. Git de esa entrega quedó sincronizado en dc0389b80474095776809d507c75d0f702a3f22c.

RUN02 despachó3 solicitudes ALMACENAR: A, disputa del mismo comando y archivoB. Esto cuenta solicitudes del coordinador, no entradas al escritor ni documentos confirmados. El observador consumió ARM,4 INSPECT y RESUME. Se detuvo con COMException HRESULT0x80131302, etapa genérica ARMED. El controlador terminó CLOSED/cleanupConfirmed y el runner confirmó cierre de sesiones, secretos eliminados y lease liberado. La consola y el controlador salieron; no se reenvió almacenamiento. Corte:1276 HTTP/81 SELECT/9 aperturas. C03 no fue preparado, no hubo SolicitarDetencion.

La traza externa conservó solo ATTACHED y ARM: su temporizador de20 minutos venció antes del intervalo interno. La progresión del protocolo es indicativa de observaciones validadas por el código sellado, pero no reemplaza la prueba directa perdida. No se acredita entrada única, peer durante retención, códigos de bloqueo, respuesta perdida ni Freeze/Thaw exitoso. El error de liberación puede haber ocultado el fallo previo de una acción; sus HTTP/código no quedaron guardados. No se atribuye una brecha al producto con esa información.

Se abrió después una consola distinta únicamente para la cuenta SELECT autorizada:0 login de aplicación,0 HTTP y0 mutaciones. La auditoría añadió14 SELECT, conservó huellas estables entre snapshots y eliminó sus secretos en finally:

| Recurso | Estado durable | Evidencia |
| --- | --- | --- |
| Lote24, archivo40, raíz39, expediente670 | EN_CURSO; intento69 EN_CURSO con reclamación activa, sin fin ni resultado | No hay DocumentoId/InventarioId durable. Efecto físico A SIN_CONFIRMAR; no afirmar ausencia ni cero efectos. |
| Archivo41, raíz40, expediente671 | NO_INICIADO; sin intento ALMACENAR ni reclamación | Ausencia física verificada en gabinete, inventario e índice. Este archivo41 no es el intento histórico protegido41. |

El lote conserva estado EN_CURSO y detencionSolicitada=false. Hay0 documentos confirmados observados; el total de efectos físicos permanece desconocido por A. Acumulado vigente1276 HTTP (C03=0),95 SELECT y9 aperturas. Restan224 HTTP/145 SELECT y0 aperturas; ConsultarLote C02 está en20/20. Recursos y staging se conservan, incluyendo intento69. Los intentos históricos41/53 y sus lotes18/21 no recibieron mutaciones.

No se permite recrear C02, repetir ALMACENAR, forzar EN_CURSO a INCIERTO, tomar posesión por antigüedad ni usar cierre preinicio sobre un intento iniciado. La recuperación anterior del lote24 deja de ser elegible por existir ALMACENAR. La autorización queda STOPPED_RUNTIME_REVIEW_REQUIRED y el runner no admite esa evidencia como una continuación. Resolver el intento requiere evidencia fiable y una vía que soporte el contrato existente; esta entrega no inventa una reparación.

## Corrección local del observador y alcance de sus pruebas

El código oficial de .NET identifica0x80131302 como [CORDBG_E_PROCESS_NOT_SYNCHRONIZED](https://github.com/dotnet/runtime/blob/main/src/coreclr/inc/corerror.xml). La relación con Thaw llamado en modo Run es una inferencia compatible con la implementación inspeccionada; la etapa antigua no identifica la sentencia exacta. [Debugger.Break(false)](https://learn.microsoft.com/en-us/dotnet/api/envdte.debugger.break?view=visualstudiosdk-2022) solicita el paso a modo Break y retorna antes de que termine; [Thread.Thaw](https://learn.microsoft.com/en-us/dotnet/api/envdte.thread.thaw?view=visualstudiosdk-2022) libera el hilo.

La candidata ahora espera Break antes de Thaw, verifica PID/hilo/liberación y espera Run después de Go, con hasta1000 ms por transición. Solo al liberar puede pausar brevemente el proceso completo; el intervalo entre acciones mantiene la política de retener exclusivamente el hilo elegido. El temporizador solo señaliza el watchdog; las llamadas COM se ejecutan en el STA del controlador. Se añaden etapas RELEASE_BREAK/RELEASE_THAW/RELEASE_GO/RELEASED. Esta sincronización todavía NO está calificada en runtime documental.

El propio controlador conserva historial saneado con estados, secuencias, PID/hilo, entradas, flags y fallos; no depende del temporizador externo ni se pierde al sobrescribir status con CLOSED. Heartbeats iguales no duplican entradas y el máximo es2048 eventos. El puente preserva orden/códigos de acción cerrados aun si falla liberación; el ejecutor persiste stopped-window antes de terminar. Nunca se guardan claves de operación, credenciales, cookies, cuerpos HTTP o filas SQL.

Regresión187/187 en14 archivos, cero fallos. Incluye auditoría sin mutaciones, prueba C# de las5 variantes de sincronización/liberación con callbacks sintéticos y prueba PowerShell real de la retención del historial al sobrescribir CLOSED. El controlador compila contra el SDK instalado. No hubo attach ni almacenamiento para probar la candidata corregida, ni replay del intento69. Progreso16/27; COORD02/03 no están aceptados.
