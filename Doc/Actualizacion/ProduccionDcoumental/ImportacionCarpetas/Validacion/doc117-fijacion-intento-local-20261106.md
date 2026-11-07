# DOC-117 — Fijación previa del intento, 2026-11-06

El ajuste editorial aprobado mediante «aprobado» se implementó en el soporte de laboratorio. **No hubo autenticación, nuevas consultas, cargas ni instalación en el sitio real.** La tarea 16.1 conserva su estado pendiente de integración real.

## Comportamiento implementado

La variante compila una identidad cerrada de sitio/actor/empresa/nivel/versión/gabinete, lote, dos raíces/archivos y sus expedientes/relaciones, nombres, bytes, contenido, snapshot y huellas de solicitud. `FIRST_ATTEMPT_BEFORE_WRITER_V1` no acepta un ID de intento futuro en el manifiesto.

Antes de delegar en `IniciarAlmacenamientoDocumento`, el helper crea un registro por archivo con `FileMode.CreateNew`, escribe IDs y huella de solicitud y hace `Flush(True)`. Comprueba sus bytes antes de iniciar. La autoridad original se ejecuta primero; desde el inicio, exige que el registro durable siga coincidiendo con el intento exacto. Falta, alteración, contenido parcial, recurso/solicitud ajenos, reparse point o otra activación detienen el recorrido. No se borran registros para reintentar. La transición transaccional de producto conserva la exclusión del escritor.

La ventana A delega en el checkpoint confirmado y falla antes de retornar; la B falla tras verificación y antes del checkpoint. Ambas mantienen los IDs observados. Los marcadores impiden una nueva activación del recurso. El replay confirmado sigue siendo responsabilidad del histórico del coordinador; no se habilita replay B ni reconciliación de EN_CURSO.

El wrapper de lease LIVE exige el alcance completo, su hash, el protocolo aprobado y las tres autorizaciones propias. No acepta escenarios DOC-116, otra raíz física ni intentos predichos como sustituto de las huellas revisadas.

## Verificación con producto real y dobles

El test compila el harness .NET existente y usa `ComandoAlmacenamientoCarpetas.Preparar` real. La prueba de huella ejecuta `CoordinadorImportacionCarpetas.AlmacenarArchivo` con el PDF y persistencia/escritor dobles existentes, y contrasta la `SolicitudHash` que el coordinador realmente asigna. Cambiar contexto, lote, archivo, revisión, clave o fase cambia la huella; el replay confirmado no incrementa el escritor.

El helper se ejecuta con comandos inmutables reales y metadatos del PDF determinista existente. Su callback de inicio ve el registro ya escrito; las dos ventanas conservan los IDs y solo A llama al checkpoint doble. Esta prueba del helper no escribe documentos reales. Las pruebas adicionales con dobles cubren carrera de dos inicios (uno solo aceptado), registro incompleto/no creable/desaparecido, intento ajeno y bloqueo posterior a los marcadores.

El ensayo con producto reveló que `EsNuevaReserva` tiene `JsonIgnore`: el constructor valida ese flag de entrada y no lo conserva en el comando. El helper respeta el contrato sin cambiar el modelo. Los archivos de variante cerrada se generan con BOM UTF-8 para conservar nombres Unicode en el compilador VB legacy.

La herramienta .NET `Doc117SolicitudHashConsole` recibe dos solicitudes/contexto solo por stdin y emite únicamente los hashes; rechaza campos desconocidos y fases diferentes. No guarda contexto, claves de operación o payloads. Puede compilarse localmente con `compile-doc117-request-hash.cjs` a un directorio nuevo de `.tmp`.

## Construcción y límites

La candidata desactivada actual se compiló con el proyecto completo a `.tmp/doc117-alm02-pin-dormant-20261106`, sin errores. Los 912 archivos de entrada del proyecto y la DLL real conservaron sus hashes. Candidata: `db2453183c3cdec0fa5b8988a7716c59785df501803719514cdc672471420a26`. Original: `4134f57f66fc259b5f33a553577c9a07e6bf176b2da027aaeeb8576325c282c5`. No instalar la candidata desactivada.

Con los IDs reales y un alcance revisado podrá usarse `build-doc117-alm02-dormant.cjs --scope <archivo> <directorio-nuevo>` para compilar la variante cerrada. Ese comando no instala ni autoriza efectos. El alcance de esta prueba sigue siendo sintético, no un manifiesto autorizado del ambiente.

También se probó esa rama del constructor con un alcance **sintético** y el proyecto completo: `COMPILED_SCOPED_NOT_INSTALLED`, sin errores, fuera de `bin`. No es la candidata real del ambiente y no debe instalarse. Huellas del build en el JSON de este reporte.

Se repitió el ensayo de archivos/procesos con esta candidata desactivada: guardián independiente restauró el sitio temporal tras la salida del propietario y ambos respaldos coincidieron byte a byte. No hubo carga de DLL ni activación IIS. La autorización editorial no concede la preparación real de recursos ni su almacenamiento. Evidencia, comandos y huellas actuales en `doc117-fijacion-intento-local-20261106.json`.

El driver de la próxima fase quedó preparado y probado: `doc117-alm02-setup-listo-20261106.md` y propuesta de recursos asociada. No se ejecutó contra el ambiente; deja los originales/expedientes listos y calcula el alcance antes de cualquier ALMACENAR o instalación. La aprobación de esa fase aún falta.

Regresión conjunta final: **48/48 PASS**, sin conexión, incluyendo el harness .NET existente, soporte DOC-116, helper/fijación/lease DOC-117, consola de huellas, preflight y preparación. Comando completo y fuentes revisadas en el JSON. OpenSpec estricto PASS. No se suman a esta cuenta corridas anteriores de los mismos tests.
