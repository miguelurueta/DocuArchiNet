# DOC-116 EXP-03 — preparación del permiso de creación

Estado: composición local lista; ejecución real pendiente de autorización específica. 95/95 comprobaciones locales PASS. La copia local prevalece. No se cambia producto, Web.config, almacenamiento, SII ni esquema.

Alcance propuesto: actor 141/empresa 2, nivel 215 (no propietario), concesión 629, perfil 24/versión 38 y gabinete 10. Nombres DOC116-20261105-EXP03-01/02. Un plan nuevo, un lote nuevo, dos PDF de 677 bytes en staging y una sola solicitud de creación que debe ser denegada. Cero expedientes nuevos y cero documentos definitivos. Recursos nuevos y anteriores conservados, incluidos lotes 16/17 y expedientes 657/658/659. Ventana sin otras importaciones/creaciones ni cambios de permisos sobre los niveles observados.

Mutación única autorizable: columna agregar_expediente de ra_pro_permisos_niveles, fila 629 del actor 141/nivel 215; secuencia 1 -> 0 -> 1 mediante el puente administrativo existente. Dos escrituras esperadas al permiso; ninguna otra columna. Controles MySQL exclusivamente SELECT. No se utiliza IMPORTAR_CARPETAS ni un permiso administrativo como sustituto.

Antes de preparar: comprobar identidad mediante el lote web 17 en nivel 7; comparar su contexto con la apertura vigente. Después validar empresa/actor/nivel activo/concesión/carga_archivo=1 y ausencia de propiedad para el nivel 215. Abortos ante cambio, ambigüedad o propietario.

Después de recibir: registrar el alcance cerrado del lote nuevo; el puente comprueba la conexión administrativa registrada contra la huella de ambiente previa, las tablas InnoDB, la fila y columnas ajenas. La huella histórica no se trata como comprobación actual: se revalida antes de escribir; un desacuerdo detiene sin mutar el permiso. Diario durable en tools/e2e/artifacts/doc116-level-permission-recovery.json, bloqueo exclusivo y recuperación de estados no restaurados.

Prueba: retirar 1 -> 0, verificar readback, enviar una creación; exigir 403/ACCESO_DENEGADO, snapshot/conteos sin cambio y cero relaciones nuevas. Finally restaura 0 -> 1 y comprueba por lectura independiente; se consulta otra vez el lote y se compara el estado. PASS exige denialUnchanged=true, permissionRestored=true, rootsCreated=0 y creationRequests=1. Ante fallo se conserva el diario y no se repite creación ni se fuerza una fila alterada.

Reutilización: preparación/consola/login/cliente existentes, ejecutor DOC116 y mutador nativo previamente probado. La factoría del puente es compartida por ambas entradas de prueba. Descubrimiento por escenario validado; se corrigió también el filtro del lanzador de lotes ya preparados para que Playwright encuentre su título completo.

Pruebas locales nuevas: rechazo de propietario, fila inesperada, permiso desactivado y recepción no autorizada antes de preparar; presupuesto de cero creaciones; puente sin autorización rechazado; resultado no se acepta sin denegación intacta/restauración confirmada. Sin ejecución real de EXP03. EXP02 interno sigue NO_EJECUTADO.
