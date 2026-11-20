# DOC-120 — lectura de revisión sellada con reglas en otro orden

Estado: **CORRECCIÓN REPRODUCIDA LOCALMENTE Y DESPLEGADA; CONSULTA AUTENTICADA DEL CASO PENDIENTE**. Producto `beae0afc194d061db76ca9e4c9d3a2ea522490b4`; [huellas, pruebas y despliegue](doc120-lectura-plan-sellado-20261117.json). Referencia comunicada por el usuario: `62232125f0f64fdfb1537b2251288a35`, con1.000/1.000 metadatos revisados y fallo al consultar resultados, sin envío ni creación según ese reporte. No se encontró una traza local persistida que permita atribuir inequívocamente esa referencia a una fila de BD; no se consultaron sus datos ni se reutilizaron credenciales de otra ventana.

## Defecto y corrección

`PrepararPersistencia` ordena las reglas del snapshot antes de calcular la huella del plan. El sellado esquema2 escribía `p.Snapshot`, que conserva el orden de publicación (`ORDER BY orden`), en lugar del snapshot canónico `plan.Snapshot`. Al reconstruir el plan desde la base, el lector calculaba una huella diferente si ambos órdenes no coincidían y devolvía `CAPACIDAD_NO_DISPONIBLE`.

La regresión usa un `IDataRecord` con cabecera/referencias persistidas y1.000 archivos fragmentados, con reglas NOMBRE/CODIGO en el orden de publicación y CODIGO/NOMBRE en el plan canónico. El lector productivo reproduce el rechazo antes de la corrección. Después:

- El sellado escribe el snapshot canónico que corresponde a la huella calculada.
- El lector normaliza únicamente el orden de reglas de planes2 ya guardados antes de contrastar su huella. No actualiza filas ni cambia valores, fechas, inventario, propietario, contexto o huella persistida.
- Alterar un valor de regla continúa rechazándose. Cabecera/referencias, cantidades, integridad y hash de fragmentos permanecen comprobados.
- Esquema1 y la serialización/huellas/límites de perfiles conservan sus contratos.

## Verificación y límites

18/18 casos Node en este corte:2 wrappers Framework/DDL y16 cliente; no sumar los casos internos ni estos resultados a cortes históricos. Framework comprueba lectura de cabeceras originales/canónicas, rechazo de contenido alterado y1.000 archivos; el cliente comprueba que un fallo de lectura después de sellar se retoma sobre el mismo plan con consultas únicamente, sin repetir inicio/partes/validación/sellado ni confirmar/enviar. Build completo Debug aislado:0 errores.

Se detectó una DLL recompilada desde el despliegue anterior. La primera comprobación de sustitución abortó antes de modificarla. Tras verificar su huella actual se conservó esa copia, se instalaron DLL/PDB del producto corregido y se contrastaron sus SHA256. No se reinició el grupo compartido de IIS. Run `65a3b228329a4a9f85261325804fa790`:0 SELECT/0 efectos,1 POST JSON anónimo con401/SESION_REQUERIDA. Esta comprobación acredita arranque/guard de sesión, no la lectura autenticada de la preparación del usuario ni una importación real de1.000 PDF.

La actualización de DLL reinicia el dominio ASP.NET; el sitio usa sesión InProc. Si la sesión/selección de la pestaña se conservan, **Continuar revisión** consulta el plan ya sellado, como acredita el caso cliente. Si debe volver a iniciar sesión o recargar, el recorrido anterior puede exigir reselección/nueva revisión por pérdida de referencias en memoria; no prometer continuación automática de un borrador tras perder ese contexto. La corrección permite leer las revisiones existentes sin migrarlas, pero este corte no incorpora un flujo nuevo de recuperación de revisión tras expirar la sesión.

Mínimo recomendado de retorno: **`beae0afc` o una versión posterior que conserve ambas correcciones de esquema y lectura de plan**. Sustituye `af1ff073` como versión recomendada para este defecto. Paquete y copias anteriores permanecen en `.tmp/`, excluidos de Git. Sin SQL adicional, DDL, nuevos lotes, recepciones o expedientes.3.118/3.126/3.127/5.2 y cierre operativo continúan pendientes;139/148 tareas. La coincidencia del defecto con el caso real se confirma al consultar la revisión con la cuenta del usuario, todavía no ejecutado por el agente.
