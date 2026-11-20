# DOC-120 - resultado real de guardado, pausa, cierre y retoma

**PASS_REAL_STORAGE_UI** en la ventana08bc33cc7b92403e8dd1ba47adca1a86, lote52/propietario141/empresa2. Revision del estado local cerrado y fuentes selladas; ninguna operacion nueva durante esta revision. [Estado, limites y fuentes](doc120-guardado-real-resultado-20261119.json), [tiempos](doc120-guardado-real-resultado-20261119.csv).

El lote pasa de80 a84 documentos guardados; quedan916 por guardar. Cuatro solicitudes ALMACENAR, todas200/OK, cuatro identidades unicas confirmadas por checkpoint RA y correlacion DA de gabinete/inventario/indice/expediente. Expediente696 y relacion1120 conservados. Los otros996 registros con sus valores/origenes/resultados, intentos/eventos previos y configuracion conservan los controles exigidos por el ejecutor.

| Archivo | Documento | Inventario | HTTP de guardado |
|---|---|---|---|
| 2177 | 18341 | 25471 | 0.989 s |
| 2178 | 18342 | 25472 | 0.938 s |
| 2179 | 18343 | 25473 | 0.974 s |
| 2180 | 18344 | 25474 | 1.022 s |

1. 2177: pausar durante espera deja terminar18341 y no admite el siguiente.
2. 2178: cancelar salida conserva ejecucion; termina18342 e inicia2179 automaticamente sin otro Continuar.
3. 2179: aceptar cierre espera18343; no inicia2180 y cierra el formulario despues de la respuesta.
4. Recargar/consultar52, continuar2180, pausar y volver a recargar conserva18344 sin repetir ninguno.

Las pausas y el cierre se solicitaron antes de estar disponible la respuesta del servidor (`serverResponseAlreadyAvailable=false`). Se retuvo solo su entrega al navegador; no se simulo el almacenamiento. Promedio HTTP0,98075s (0,938-1,022s), medido antes de la retencion. Excluye tiempos de interfaz, esperas de prueba, recepcion, preparacion y controles RA/DA.

Preparacion necesaria:3 PDF recibidos (134642 bytes) y4 materializaciones de valores;2177 ya recibido y no reenviado. Cero planes/lotes/expedientes nuevos, perfiles, DDL o borrados. Consumo623/1200 HTTP,50/80 SELECT externos de control y1/1 sesion. Ventana cerrada, admision cerrada y claves eliminadas; no repetir sus operaciones.

La muestra acredita el recorrido prioritario sobre cuatro archivos de un lote existente. No certifica carga completa de1000, creacion inicial del expediente/arbol, rollback nativo, descarga fisica de los PDF ni levantamiento de detencion administrativa. Las once tareas abiertas anteriores y la aceptacion manual permanecen pendientes;256/267. No hay cambios de producto/DLL/esquema en esta revision.

La ventana5db2662119bd4d9ab7985548e6cdfea5 conserva su STOPPED previo con cero accesos; la nueva ventana fue solicitada expresamente por el usuario mediante "lanza" y aceptada SI en consola privada. No reutiliza secretos ni repite fases de esa ventana.
