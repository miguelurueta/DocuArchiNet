# DOC-118 — lectura durable del lote25

La consola original estaba en CodexSandboxDesktop y no era visible para el usuario. Se cerró únicamente su proceso propio, se conservó el reporte pendiente por hash y se abrió la misma auditoría en el escritorio normal mediante ejecución aprobada fuera del sandbox. No se reiniciaron contadores ni se ejecutó una mutación de negocio.

La auditoría terminó:25 SELECT nuevos, cero HTTP/login de aplicación y cero mutaciones. Libro de la ventana anterior:301 HTTP/59 SELECT/2 sesiones. Histórico1277/113/9 conservado; acumulado1578/172/11. Credenciales efímeras eliminadas al finalizar.

Archivo42/raíz41/expediente672:intento74 CONFIRMADO, documento16235/inventario23339; verificación física por esos IDs KNOWN_IDS_VERIFIED. Archivo43/raíz42/expediente673:recibido y expediente confirmado, almacenamiento NO_INICIADO, sin intento ni documento y ausencia verificada. Lote25 EN_CURSO, sin detención ni reclamaciones activas. Huella del snapshot estable:3c382e332f0e5ccdb3835408f0ab95e9efee10aa796c04a0050f262cf8ab594d.

La respuesta del cliente original sigue siendo históricamente UNKNOWN; esta lectura acredita el resultado durable actual. No acredita OTHER_FILE ni la pérdida controlada del retorno después del checkpoint. No se reenviaron los comandos antiguos ni se limpió el lote. Las seis comprobaciones de integración permanecen abiertas hasta el recorrido completo. Fuente:doc118-auditoria-lote25-20261110.json.
