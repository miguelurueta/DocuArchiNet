# DOC-115 — RECIB-02, revisión del resultado

Estado: EJECUTADO_OK. La consola y el checkpoint coinciden en PASS y escenario RECIB-02, dentro del alcance autorizado. Planes 26/27 y lotes 7/8; dos preparaciones, dos confirmaciones y ocho solicitudes de recepción. Una recepción original válida en staging del primer lote; el segundo conserva su estado inicial como control.

Se verificaron rechazo de ID de archivo ajeno, campos de destino/ruta adicionales, token ausente, contenido truncado y exceso de bytes; recepción válida de 677 bytes, reenvío idéntico y rechazo de contenido cambiado. Las aserciones comprobaron por SELECT que los rechazos de autoridad no alteraran ambos recursos, que los rechazos de contenido fueran terminales antes de otra escritura y que la recepción persistiera tamaño/huella con un único intento confirmado. El reenvío no cambió revisión, contenido ni intentos. El lote 6 conserva su huella antes/después. Conteos documentales sin cambio; no creación ni almacenamiento definitivo.

Código: ed4a011fc7cdaacd1bc208ae9a10592b644df207f87fd7d06f49a9670e40fff1; coincide con el ámbito actual: true. Backend: 4134f57f66fc259b5f33a553577c9a07e6bf176b2da027aaeeb8576325c282c5; coincide con la DLL actual: true. Fin UTC: 2026-11-04T12:08:51.723Z. Esta revisión lee evidencia guardada; no vuelve a autenticar ni consulta la BD.

El rótulo final DOC115_RECIB01_FINISHED era un error de texto de la consola auxiliar. Las validaciones, IDs, presupuestos y archivos corresponden a RECIB-02. Se corrige el rótulo para ejecuciones futuras sin reejecutar la prueba ni modificar su salida histórica.

Límites: exceso de bytes declarados y contenido real no equivalen a cuota agregada finita configurada; esa guarda tiene evidencia local con límites inyectados. Las partes rechazadas se conservan. El PASS de RECIB-01 y este PASS no acreditan RECIB-03 ni las carreras MySQL de conexiones independientes de 07-A. La tarea 5.2 y el cambio DOC-115 siguen abiertos (22/28 tareas).
