# DOC-120 - encabezado segun estado vigente

Tarea3.248: el video230607 muestra Consultar importacion durante recepcion/almacenamiento. La vista elegia el titulo por el origen model.consulta y conservaba una seccion generica de lote revisado. Ahora deriva titulo, subtitulo y seccion de los estados ya disponibles, sin modificar el modelo, los permisos, las acciones ni el procesamiento. [Fuentes y verificaciones](doc120-encabezado-estado-20261119.json).

| Estado visible | Titulo |
|---|---|
| No lote, seleccion/revision | Importar carpetas |
| Consulta sin coordinacion activa | Consultar importacion |
| Coordinacion activa o fase documental manual en vuelo | Importando carpetas |
| Pausa solicitada, falta acuse | Pausando importacion |
| Cierre voluntario pendiente de respuesta | Pausando para salir |
| Pausa local/original pendiente, sin trabajo en vuelo | Importacion en pausa |
| Detencion confirmada sin resultado incierto | Lote detenido |
| Resultado incierto/bloqueo por revisar | Revisar importacion |
| Estado del lote COMPLETADO, no pagina filtrada | Importacion finalizada |

La finalizacion depende del estado COMPLETADO del lote; no se deduce de una pagina filtrada. Pausando no declara la pausa aplicada mientras falta el acuse. Una detencion confirmada sin incertidumbre tiene prioridad sobre el bloqueo informativo que exige reanudar. Una solicitud documental manual en vuelo no queda titulada como pausa del coordinador anterior. Seleccion/revision, referencias consultadas y acciones conservan sus comportamientos.

El titulo principal final es Importacion finalizada y el panel de accion conserva Importacion completada; asi sigue siendo unico el encabezado usado por navegacion accesible/consumidores anteriores. Se actualiza el texto del mismo nodo de titulo y se conserva su aria-labelledby, foco y dialogs. La seccion pasa a Estado de la importacion, Avance de la importacion, Avance conservado, Pausa solicitada, Detencion del lote, Resultado por revisar o Resultado del lote segun el caso.

**118/118 pruebas locales finales en una seleccion de diez suites existentes, cero nuevos casos.** Mensajes, ultimo lote, cliente original, filtros/CSV/ayuda, guardado sintetico de cuatro, pausa/cancelacion/cierre/retoma, incertidumbre, envio manual, detencion/reanudacion y fidelidad390/768/1280. No suma corridas anteriores: reproduccion0/4, primer focal29/30 con expectativa de titulo completado anterior y primera amplia113/118 que detecto prioridad de detencion. Esa prioridad se corrigio antes de la corrida final118/118. Intermitencia contextual de teclado historica no reejecutada ni cerrada por esta seleccion.

ASPX versiona exclusivamente la vista modificada con doc120-estado-visible-20261119-10. Sin compilacion/sustitucion DLL, CSS, controlador, API o esquema. Flujo tecnico textual/anexo/manifiesto actualizados:124 fuentes/20236 lineas, FLOW_CURRENT; sintaxis y OpenSpec estricto comprobados antes de entrega.

Cero HTTP/SELECT/sesiones/efectos reales. La ventana08bc33cc conserva su PASS de cuatro documentos y SHA propio; sus resultados no se convierten en una nueva E2E del encabezado modificado. No reabrir ventanas cerradas ni ejecutar mas guardados para probar textos sin ventana correspondiente.257/268 tareas;48 S/220 M,111 escenarios, once pendientes anteriores conservados. El video manual acaba en126 guardados y proceso activo, no acredita fin del lote.
