# DOC-118 — persistencia V2, RV-04

Implementada6.4/D-14/RQ-14 tras «contiua con el flujo». Continúa dentro de DOC-118 y conecta el recorrido interno del adaptador con el núcleo SQL del repositorio existente. La tarea se incorporó antes del código. Progreso21/31, con las mismas10 tareas reales/finales anteriores pendientes.

`MySqlImportacionCarpetasRepository.DocumentoV2.vb` compone los cuatro callbacks de persistencia. Cada operación usa Operar, metadatos instalados y transacción existentes; locks lote→raíz→archivo→intento y lectura acotada/ordenada del diario. No agrega tablas, migraciones, workers, conexión alternativa ni DDL. Las escrituras son parametrizadas y comparan estado, token, solicitud, evidencia previa, ámbito/recurso y revisiones.

Inicio, autorización y observación actualizan evidencia y diario juntos. Los acuses se construyen después de que Operar retorna tras commit. La autorización no es idempotente como permiso:un segundo pedido se bloquea aunque se haya perdido la primera respuesta. Los eventos deben ser únicos, ordenados, completos y concordantes; no se infiere autorización de ausencia, edad o un documento aportado por el cliente.

El checkpoint conserva los tres documentos tipados en el diario y persiste el resultado normal existente en intento/archivo. Resultado, evento y consolidación comparten la transacción exterior. Solo admite los IDs observados; incertidumbre mantiene reclamación e IDs parciales, y una señal de stop posterior al gate se conserva. Un replay del mismo checkpoint devuelve historial sin nuevas escrituras o consolidación.

El lector documental existente despacha V2 a una proyección segura EN_CURSO/ALMACENAMIENTO_PREPARADO_V2, sin exponer las pruebas. El código distinto impide usar el checkpoint V1 con una evidencia V2 activa. La lectura V1 conserva roundtrip; JSON malformado, otra fase/estado/recurso o versión inválida falla cerrado. Este cambio de lector es explícito y trazado antes/después; no se modifican almacenamiento compartido, adaptador legacy o serialización de resultados públicos.

Verificación:

-20 casos SQL nuevos del núcleo de producción, integrado con el adaptador:flujo completo, gate concurrente único, respuestas perdidas de las cuatro fases, rollback en cada sentencia y consolidación, replay, stop, IDs parciales, diario duplicado/ausente/desordenado, token/contexto/revisión y compatibilidad V1.
- La transacción y la conexión son dobles locales; el escritor es simulado y el staging usa PDFs sintéticos. **No acredita atomicidad ni concurrencia de un servidor MySQL real.**
- Regresión18/18 comportamiento/cliente y205/205 herramientas:223 pruebas Node disjuntas. El harness conserva19 casos de contratos,21 de proveedor y20 de adaptador, más20 SQL; estos grupos internos C# no se suman a Node.
- El chequeo DDL existente se amplió al nuevo parcial y pasó por separado:1 prueba ya incluida en el total, sin contarse otra vez. Las columnas UPDATE corresponden al DDL instalado.
- Las14 fuentes históricas mantienen12 archivos intactos y2 deltas autorizados de prueba;10 informes históricos y16 fuentes de evaluación intactos. El lector modificado se registra aparte con SHA anterior/posterior.

No hubo sesiones, HTTP de negocio, SELECT o efectos documentales nuevos. Consumo1276 HTTP/113 SELECT/9 aperturas. Los intentos41/53/69 permanecen protegidos;69 no obtiene V2 retrospectivo. Composición pública, recuperación y calificación real siguen pendientes. Sin despliegue, DLL/PDB, permisos o configuración cambiados.

Las primitivas se contrastaron con documentación oficial de MySQL:[lecturas con bloqueo](https://dev.mysql.com/doc/refman/8.0/en/innodb-locking-reads.html), [comparación null-safe](https://dev.mysql.com/doc/refman/8.0/en/comparison-operators.html). Se usan sentencias compatibles con el esquema legacy; esa referencia no certifica un servidor5.1 real. Evidencia:[JSON local](doc118-persistencia-v2-local-20261108.json).
