# DOC-111: barreras de metadatos previas a creación, 2026-10-29

Avance parcial de 10.4, 10.5 y 10.6. **97/130 tareas completas, sin incremento**. La evaluación y su lectura están implementadas para creación; no se certifican la coordinación de fase completa, disco, almacenamiento o aceptación real.

El parcial Barreras de CoordinadorImportacionCarpetas evalúa contexto/snapshot e inventario del plan confirmado contra los metadatos persistidos. Comprueba cantidades y suma de bytes originales, identidades de raíz/archivo, pertenencia y ausencia de duplicados, sin agrupar por nombres ni reducir totales. La elegibilidad se comparte con la confirmación a través de ServicioPreparacionImportacion.EsArchivoElegible, sin volver a consultar fuentes o proveedores.

- COMPLETO_OBLIGATORIO/EXPEDIENTE exige todos los archivos de la raíz; otra raíz incompleta no bloquea esa recepción independiente.
- COMPLETO_OBLIGATORIO/LOTE exige todos los archivos y raíces originales de la unidad. Ausencias, incidencias o estados inciertos bloquean la creación. Un resultado documental ya confirmado de otra raíz no elimina su archivo del inventario ni invalida una recepción original coherente.
- PARCIAL_PERMITIDO/NO_APLICA admite un archivo elegible recibido aunque otros estén pendientes o rechazados. Los rechazados permanecen contados y no se consideran elegibles para simular completitud.

La prueba requerida de recepción es estado RECIBIDO, bytes exactos/positivos, hash de contenido y stagingRef con forma canónica y ausencia de intento activo/efecto incierto o en curso. No comprueba contenido físico o hash del disco: las referencias son evidencia registrada por recepción, no una verificación de disponibilidad actual del archivo.

El parcial Barreras del mismo repositorio consulta únicamente columnas necesarias, con límites de filas derivados del plan más una fila para detectar exceso. ComprobarBarreraCreacion realiza la lectura previa bajo el bloqueo común. IniciarCreacionExpediente vuelve a cargar el plan sellado y evaluar la barrera bajo ese bloqueo antes del UPDATE de inicio. No se basa en un resultado cliente o lectura previa. El plan se carga mediante CargarPlan, que verifica su huella; no se sustituye por otra publicación.

Un bloqueo de barrera no cambia estados ni invoca legacy. La consulta previa no reserva. El coordinador todavía debe usarla antes de reservar; si una reserva existente falla la revalidación se conserva y no se habilita automáticamente otra. Las barreras de las fases posteriores y la revalidación física siguen pendientes.

Validación PASS:

- `node --test tests/doc110-perfil-importacion-behavior.test.cjs`: arnés Framework con familias previas y comprobación DDL existente. Casos de los tres modos, raíces homónimas, archivo pendiente/rechazado, corrupción de bytes/hash/referencia, ausencia/duplicidad/identidad ajena, inventario reducido, snapshot/política distintos, detención y preservación exacta de entradas/totales.
- Lector SQL con registros sintéticos: scope por lote/raíz según política, SELECT sin payload documental y límites originales. No acredita lectura/bloqueo/carrera MySQL real.
- Build Debug completo sin despliegue, salida/intermedios `.tmp/doc111-barreras-creacion-build/{bin,obj}`, log `.tmp/doc111-barreras-creacion-build.log`, código 0.
- OpenSpec estricto y revisión de diferencias PASS. Diffs de ClassGaExpediente, ClassGaProducionDocumental, ClassAlmacenamiento, ClassNeodynamic y conect vacíos.

Sin SQL/HTTP/E2E reales, creación, almacenamiento, modificaciones de datos/configuración o despliegue. Sin cambios de carga individual o SII.
