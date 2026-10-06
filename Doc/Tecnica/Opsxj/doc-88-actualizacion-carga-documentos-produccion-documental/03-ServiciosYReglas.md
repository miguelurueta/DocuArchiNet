# ACTUALIZACION-CARGA-DOCUMENTOS-PRODUCCION-DOCUMENTAL

- Ticket: DOC-88
- Cambio OpenSpec: doc-88-actualizacion-carga-documentos-produccion-documental
- Clasificacion: cross_cutting (Transversal)
## Servicios y reglas

### Recorrido de persistencia inspeccionado

`FileUploadHandler_.ashx.vb`, para `evento_adjunta=PRODUCCION`, llama a `ClassAlmacenamiento.UploadSaveFile`. La rama delega en `PreAlmacenaDocumentoProduccion` y finalmente en `AlmacenamientoDocumentoProduccionDocumental`, que invoca `Almacenamiento`.

`Almacenamiento` reserva el identificador físico y registra el documento en el gabinete. En la transacción de inventario inserta la relación en `registro_producion_documental` y devuelve `ID_REGISTRO_PRODUCION_DOCUMENTAL` mediante `LastInsertedId`.

### Evidencia MySQL de solo lectura

La exploración se realizó exclusivamente mediante consultas `SELECT` sobre `information_schema` y agregados sin exponer filas documentales, credenciales ni cadenas de conexión.

- La tabla canónica `registro_producion_documental` contiene `ID_DOCUMENTO_DOCUARCHI_ALMACEN`, `NOMBRE_GABINETE`, `EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE`, usuario, tipología, nombre y fechas.
- No existe una columna de identificador de intento, UUID, token, operación o hash en esa tabla.
- No existe un índice `UNIQUE` que represente una carga lógica. La única restricción única es la clave primaria compuesta por el identificador de registro y `remit_dest_interno_idremit_dest_interno`.
- El par `(NOMBRE_GABINETE, ID_DOCUMENTO_DOCUARCHI_ALMACEN)` está indexado de forma no única. En los 12.759 registros examinados no presentó duplicados.
- Una firma natural por expediente, usuario, tipología, segundo nombre, fecha y gabinete no es utilizable para deduplicar: produjo 616 grupos repetidos y 7.916 filas adicionales.
- No hay triggers sobre `registro_producion_documental` que aporten una identidad de intento.

### Contrato reutilizable y límite

`ClassGaProducionDocumental.Solicita_id_inventario_documental(id_imagen, nombre_gabinete, ...)` ya consulta la relación posterior al almacenamiento mediante `ID_DOCUMENTO_DOCUARCHI_ALMACEN` y `NOMBRE_GABINETE`. Este contrato permite reconciliar un documento cuando el servidor conoce el identificador físico.

No resuelve por sí solo una respuesta HTTP perdida: el siguiente reintento del navegador no conoce necesariamente el nuevo `id_imagen`, y una búsqueda por nombre, expediente o tipología puede confundir cargas legítimas repetidas.

### Decisión aprobada para D-06

Se adopta el cambio mínimo para no afectar el cargador compartido:

- no se agrega tabla, columna, token ni identidad persistente;
- no se consulta si el documento existe;
- un retorno de `Almacenamiento` diferente de `YES` termina el recorrido;
- un retorno `YES` proyecta únicamente los identificadores entregados por esa misma llamada;
- una falla visual o una respuesta perdida no provoca otra escritura automática;
- el doble clic se contiene únicamente mientras la solicitud de `PRODUCCION` está activa.

Esta decisión no promete idempotencia fuerte entre solicitudes independientes. Ante una confirmación perdida el resultado es incierto y requiere verificación operativa, pero la aplicación no intenta deduplicar con una firma ambigua ni repite el almacenamiento por sí sola.
