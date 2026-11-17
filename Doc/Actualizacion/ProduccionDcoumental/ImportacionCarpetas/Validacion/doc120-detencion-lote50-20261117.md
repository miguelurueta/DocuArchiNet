# DOC-120 — diagnóstico real de detención del lote50

Estado: **DETENCIÓN PERSISTENTE CONFIRMADA; SIN REANUDACIÓN EJECUTADA**. El usuario autorizó expresamente hasta cinco SELECT, cero HTTP y cero cambios después del análisis del video como-retomar. Ejecución `8803b6f4f2c44b028bc279fbe42f4250`, finalizada2026-11-17T20:42:26.836Z:5/5 SELECT completados,0 HTTP/sesiones/efectos, consola cerrada para operaciones y credenciales eliminadas. No se repite automáticamente.

## Hallazgos reales en RA

| Comprobación | Resultado |
| --- | --- |
| Recurso | Lote50, plan90, versión45, destino223; propietario141/empresa2 |
| Estado persistido | DETENIDO, detencion_solicitada=1, revisión3026 |
| Inventario |1 raíz,1.000 archivos; raíz con identidad de expediente registrada |
| Recepción |1.000 RECIBIDO y1.000 intentos RECEPCION CONFIRMADO |
| Creación |1 intento CREAR_EXPEDIENTE CONFIRMADO |
| Documentos completados |4 archivos COMPLETADO con resultado registrado;4 intentos ALMACENAR CONFIRMADO |
| Pendientes documentales |996 RECIBIDO/NO_INICIADO; sin resultado documental registrado |
| Índices |Los1.000 archivos tienen valores efectivos registrados |
| Bloqueos de intento |0 RESERVADO/EN_CURSO/INCIERTO; sin marcadores activos en raíz o archivos ni filas inciertas |
| Última detención |Evento3544, actor141 (cuenta propietaria),2026-11-17 19:58:35 UTC /14:58:35 Bogotá |
| Estabilidad observada |Revisión3026 y último evento3545 iguales antes/después; inventario completo coincide con cabecera |

La causa actual del bloqueo es la señal persistente de detención. El código vigente no ofrece una operación general que levante esta detención y derive de nuevo el estado del lote. La vista ofrece Continuar antes de comprobar esa capacidad y el motor clasifica la detención como OPERACION_EN_CURSO; ambos defectos se reprodujeron localmente en el análisis previo. No hay un intento activo o incierto que justifique atribuir esta parada a un documento todavía en ejecución.

El registro del actor acredita la cuenta bajo la que se solicitó la detención; no demuestra qué interacción humana la originó. Este diagnóstico tampoco determina la causa histórica de los timeouts de almacenamiento.

## Alcance de la evidencia

Los cinco comandos son: cabecera/evento de detención, censo completo de archivos por estado, censo de intentos por fase/estado, detalle acotado de intentos/marcadores no resueltos y repetición de cabecera/evento. Se reutiliza queryRows y consola enmascarada, con parámetros fijos del recurso autorizado. El resultado solo contiene estados, identificadores, fechas y conteos: no descarga configuración, beneficiarios, nombres de cuenta, tokens, documentos ni evidencia JSON de negocio.

Las consultas usan conexiones de lectura separadas; coincidencia de revisión/evento/censos detecta cambios realizados por los escritores ordinarios del módulo, pero no constituye un snapshot transaccional global. No se leyeron DA, disco o contenido PDF. COMPLETADO y confirmaciones son evidencia del control RA; no son una nueva inspección física de cada documento. RECIBIDO no vuelve a comprobar que los originales retenidos sigan disponibles.

Ocho pruebas locales verifican selección fija/solo SELECT, censo completo, revisión/diario cambiados, intentos activos/inciertos, alcance ajeno, límite de detalle, saneamiento y parada al primer error. Fuentes y huellas en el JSON compañero. Sin cambios al cliente, DLL, SQL de esquema o reglas de negocio en este corte.

## Consecuencia para el desarrollo

Queda acreditada la necesidad de ReanudarLote como control explícito, con permiso/revisión/transacción/evento y salida validada de DETENIDO. Los cuatro documentos y el expediente deben conservarse; los996 restantes deben seleccionarse por estado/identidad. La reanudación y cualquier almacenamiento posterior no forman parte de esta autorización de lectura. La implementación general y la prueba real requieren su alcance correspondiente; no editar manualmente estado, detención o intentos para simular la recuperación.
