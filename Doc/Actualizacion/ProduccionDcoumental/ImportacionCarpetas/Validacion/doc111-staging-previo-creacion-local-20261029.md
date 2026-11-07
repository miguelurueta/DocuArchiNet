# DOC-111: staging físico antes de reservar creación, 2026-10-29

Avance parcial de coordinación/barreras de creación, **97/130 tareas completas, sin incremento**. No se crean expedientes ni se habilita ejecución HTTP. El paso interno de comprobación y reserva no completa el coordinador ni las fases de almacenamiento/incorporación.

AlmacenStagingImportacion incorpora VerificarRetenido al puerto existente. Reutiliza VerificarReferencia/VerificarCompleto, contención/sin enlaces, bloqueo de staging, lectura acotada, hash y validador PDF. Exige RECIBIDO con referencia/hash/bytes persistidos coherentes; no inventa un intento, crea copia, sustituye contenido o borra archivos. Verificar(contexto,intento) conserva su validación anterior y comparte el mismo núcleo.

El repositorio incorpora ObtenerBarreraCreacion, que devuelve una vista interna del lote/plan sellado/raíz/metadatos bajo bloqueo común. El lector incluye revisión de archivo y mantiene consultas acotadas al inventario original, sin bytes documentales. La consulta booleana anterior y el inicio siguen usando la misma evaluación de barrera.

ReservarCreacionConStaging valida solicitud de raíz/revisión, autoridad actual y barrera. Carga los archivos completos por ID y contrasta identidad, revisión, recepción, bytes/hash/referencia y nombre/ruta del manifiesto antes de abrir el staging. Reautoriza durante el recorrido y antes de llamar una sola vez a la reserva. No llama al adaptador de creación.

En completo, cualquier archivo requerido no comprobable impide reservar. En parcial, errores de archivo perdido/corrupto producen IDs inciertos y permiten continuar con otro elegible físicamente válido; no se eliminan archivos o ajustan totales. Sin un archivo válido no reserva. Conflicto de revisión, revocación, staging ocupado o fallo de infraestructura no se omiten como incidencias individuales. Una reserva histórica conserva EsNuevaReserva=False.

Validación PASS:

- `node --test tests/doc110-perfil-importacion-behavior.test.cjs`: familias previas y nuevos casos Framework con PDFs sintéticos en temporales propios y doble de reserva. Comprueba bytes retenidos sin modificación, falta de binario, contenido modificado con igual tamaño, revisión cambiada, permiso revocado, bloqueo del staging, solicitud de otra raíz y reserva histórica.
- Dos archivos en parcial: uno perdido y otro válido permiten una única reserva con listas de IDs correctas y totales intactos; ambos perdidos impiden reservar. No se degrada ni reescribe el commit de recepción por la lectura fallida.
- Build Debug completo sin despliegue, salida/intermedios `.tmp/doc111-staging-previo-creacion-build/{bin,obj}`, log `.tmp/doc111-staging-previo-creacion-build.log`, código 0.
- OpenSpec estricto y revisión de diferencias PASS. Funciones ClassGaExpediente, ClassGaProducionDocumental, ClassAlmacenamiento, ClassNeodynamic y conect sin cambios.

Límites: las listas/verificaciones son transitorias y los bloqueos físicos se liberan al terminar cada lectura. No prueban exclusión durable hasta un efecto posterior ni autorizan reutilizar indefinidamente el chequeo. Falta componer el coordinador integral, revalidar/excluir en cada fase y registrar las incidencias correspondientes. La reserva se probó con doble, no con escrituras MySQL reales. Sin SQL/HTTP/E2E reales, efectos documentales, cambios de configuración o despliegue; SII y carga individual preservados.
