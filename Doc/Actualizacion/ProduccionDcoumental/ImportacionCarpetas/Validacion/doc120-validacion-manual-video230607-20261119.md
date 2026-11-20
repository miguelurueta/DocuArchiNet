# DOC-120 - revision de la prueba manual230607

El video muestra recuperacion y continuidad correctas del lote52, sin un fallo funcional nuevo visible en la secuencia revisada. **Al terminar la grabacion la importacion sigue activa:127 PDF recibidos y126 documentos guardados; se esta guardando el archivo127.** No es una importacion completada. [Procedencia, tiempos y limites](doc120-validacion-manual-video230607-20261119.json).

| Momento aproximado | Observacion |
|---|---|
|00:00-00:08|Abre el modulo y consulta52; no se prepara ni confirma otro lote.|
|00:22-00:35|Continuar encuentra1174.pdf/ref2181 sin seleccion local. Muestra rutas/detalles y ofrece vincular o pausar; conserva84 guardados.|
|00:35-01:05|Vincula la carpeta y continua recepcion/guardado del mismo lote.|
|01:34-01:39|Abre consentimiento de salida y elige Seguir importando; el proceso sigue.|
|01:44-01:48|Abre consentimiento otra vez, elige Pausar y salir y desaparece el formulario. No permite medir por video el instante exacto de respuesta HTTP.|
|01:51-01:59|Reabre y consulta52; ofrece Continuar. Consultar por si solo no muestra inicio de guardado.|
|02:00-02:20|Continuar comprueba pendientes y requiere volver a vincular los originales tras el cierre. La seleccion local se habia liberado; los resultados confirmados permanecen.|
|03:08|127 recibidos/1 expediente/126 guardados. Sigue guardando1215.pdf, referencia2223, archivo127.|

La falta de seleccion local al retomar es el flujo previsto: vincular no crea otra importacion y los ya recibidos se omiten. El usuario puede decidir pausar o volver a vincular; no se observa un mensaje de fallo de almacenamiento en esta secuencia.

Hay una ambiguedad de texto visible: el titulo sigue siendo Consultar importacion durante el guardado y aparece Importacion del lote revisado. La vista fija el titulo por model.consulta (ImportacionCarpetasVista.js:262), sin considerar que la coordinacion esta activa. Se registra como observacion de claridad; no se altera el producto en esta revision.

Los cambios de conteos son visuales y no sustituyen controles RA/DA de esta prueba manual. La E2E08bc33cc aprobo cuatro documentos y pausa/cierre/retoma con sus propias verificaciones; esta grabacion amplifica evidencia visual, no ese presupuesto ni la certificacion a1000. Las fuentes locales aun coinciden con las de la E2E; el video no identifica por si solo el binario/cache servido. No acredita finalizacion, integridad fisica de todos los PDF ni ausencia de duplicados para todos los nuevos registros.

Inspeccion de fotogramas en memoria, sin persistir capturas, secretos o el video en Git. Cero nuevas solicitudes HTTP, SELECT, sesiones o efectos; sin cambiar codigo, DLL, esquema o perfil. Se conserva256/267 con once pendientes, incluida aceptacion completa; no marcar3.87/3.126/3.127 por este recorrido parcial. No se pausa ni reanuda remotamente la importacion durante esta revision.
