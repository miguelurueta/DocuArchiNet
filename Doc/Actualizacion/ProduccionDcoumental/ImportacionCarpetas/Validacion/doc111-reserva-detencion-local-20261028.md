# DOC-111: reserva y detención locales, 2026-10-28

HEAD `b2e93b726f221868cf2b82d43200a8e568871117`, workspace con cambios sin commit. Avance: **76/130 tareas completas**. Se completa implementación local de 10.2. Se adelanta parte de 10.9, que permanece abierta por su dependencia de 10.7 y aceptación con efectos/resultados documentales reales. No se cierra 10.11 ni se habilita almacenamiento.

El repositorio único incorpora el archivo parcial `MySqlImportacionCarpetasRepository.Control.vb` y el puerto `IControlLoteImportacionRepository`. No se crean tablas, repositorios, configuraciones o endpoints. La reserva documental bloquea lote autorizado y recurso, comprueba revisión/estado/marcadores y registra intento, recurso, revisión del lote y evento en la misma transacción. Se mantienen los parámetros y la validación de esquema del repositorio existente. Hay puntos internos de observación antes del evento y después de reservar el recurso para una futura prueba de rollback autorizada.

CREAR_EXPEDIENTE solo admite raíz pendiente sin identidad previa; ALMACENAR exige archivo recibido sin efecto anterior y raíz confirmada; INCORPORAR exige estado ALMACENADO e identidades de documento, expediente y gabinete coherentes. Un intento pendiente/incierto o marcador documental huérfano impide otra reclamación del lote. La exclusión se limita al protocolo DOC-111: no excluye writers externos ni resuelve el aislamiento del almacenamiento legacy.

La repetición conserva intento/token históricos y devuelve EsNuevaReserva=False, incluso si se presenta el mismo token. Solo una reserva nueva devuelve True; el indicador tiene JsonIgnore y no se persiste. La fecha del intento no concede toma de control. RESERVADO no habilita una llamada legacy por sí solo: faltan inicio/ejecución, autoridad inmediata, barreras y adaptadores verificados. Los adaptadores no se invocan desde estas capacidades.

La detención pasa por el coordinador con identidad, referencia del lote, destino y facultad actuales, conservando snapshot. Solo registra señal/revisión/evento: no borra ni modifica intentos, resultados, IDs, bytes o totales; no fuerza DETENIDO ante una llamada viva. Repetir una señal registrada no cambia revisión/eventos. Antes de la primera señal, revisión vencida rechaza la mutación. La respuesta pública tipada no contiene token ni datos de staging. No se añade endpoint ASMX antes de completar sus dependencias.

Pruebas locales en el arnés Framework existente:

- Guardas productivas: fase/recurso/revisión, pertenencia, raíz bloqueada/terminal/incierta, detención, recepción/hash/bytes, efecto previo y coherencia de expediente/gabinete.
- Lectura de raíz sintética y rechazo de identidad cliente alterada, incluyendo diferencia de mayúsculas.
- Dos reclamantes distintos y dos copias del mismo comando: una adquisición nueva en un **doble explícito con exclusión en memoria y guardas productivas**. Esto no acredita una carrera o aislamiento MySQL real.
- Respuesta perdida simulada después de reservar: se recupera el token original sin nueva adquisición. No hay takeover por antigüedad.
- Detención con intento vivo, repetición sin segunda mutación, rechazo con revisión vencida, permiso revocado/propietario ajeno/destino alterado, conservación de totales y lotes terminales.

Verificación PASS:

- `node --test tests/doc110-perfil-importacion-behavior.test.cjs`: familias VB/C# compiladas/ejecutadas en .NET Framework, incluyendo control, recepción y regresión DOC-110.
- `node --test tests/doc111-importacion-carpetas-client.test.cjs tools/e2e/tests/doc88-production-document-upload-policy.test.cjs`: 15 pruebas locales aprobadas.
- Build Debug completo sin despliegue; salida/intermedios `.tmp/doc111-control-build/`, log `.tmp/doc111-control-build.log`, código 0.
- OpenSpec estricto y `git diff --check`.

**NO_EJECUTADO:** conexiones/SQL de este corte, carrera/rollback MySQL de reclamaciones, HTTP/E2E autenticados, creación/almacenamiento/incorporación real, despliegue o extracción TIFF. No se modifica `workflow/ClassAlmacenamiento.vb`, SII ni el contrato de carga individual. La función de almacenamiento existente queda intacta por instrucción del usuario; el adaptador permanece deshabilitado por las condiciones documentadas en 15.
