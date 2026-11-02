# DOC-111 — Una fase documental coordinada

Fecha: 2026-10-30. HEAD base `b2e93b726f221868cf2b82d43200a8e568871117`, cambios locales previos conservados. Tareas 10.3–10.6 completas localmente; 106/130. Avance parcial de 10.7/10.9/10.10. Sin commit, despliegue, migración o efectos documentales reales.

## Implementación

`CoordinadorImportacionCarpetas.Documento.vb` añade `AlmacenarArchivo(loteId, archivoId, revisionEsperada, claveOperacion)`. Reconstruye identidad y destino desde el lote, obtiene archivo/raíz/plan propios, autoriza recursos y exige valores efectivos fijados. No crea expedientes, materializa fuentes o recorre el lote ejecutando escritores dentro de la misma llamada.

La solicitud usa una huella estable por contexto/lote/archivo/revisión original/clave/fase, con token aleatorio solo para la nueva reserva. Se consulta el histórico antes de acceder al disco. Un intento existente devuelve la evidencia sin nueva reserva, nueva fábrica de staging o writer. Otra clave no permite repetir un archivo ya completado ni sobrepasar un intento pendiente.

Creación y documento comparten los puertos de lectura, comprobación de inventario y verificación de staging existentes. Se conserva la huella de creación anterior; el nuevo hash documental usa archivo y fase ALMACENAR. La construcción de cada coordinador habilita únicamente su fase.

La barrera compartida valida correlaciones, totales originales, snapshot/política y estados sin fabricar una vista de creación para un expediente ya confirmado:

- Completo/EXPEDIENTE: todos los archivos de la raíz deben ser elegibles y tener recepción coherente. No bloquea otra raíz independiente que esté lista.
- Completo/LOTE: exige la recepción del inventario completo, incluidas otras raíces. Otro efecto incierto/rechazado impide avanzar; un archivo completado conserva su prueba sin volver a ejecutarse.
- Parcial/NO_APLICA: solo selecciona el archivo elegible elegido por ID persistido. Conserva rechazados, incidencias y totales. No omite un fallo de staging del propio objetivo para pasar a otro efecto dentro de la llamada.

Antes de reservar se verifica físicamente la unidad exigida. El repositorio vuelve a evaluar los metadatos bajo el bloqueo común de reserva y de inicio. Después de reservar, el coordinador relee recurso/revisión/marcador, reautoriza y abre retenciones de los originales requeridos. En completo mantiene la unidad original retenida durante adaptador, verificación y checkpoint; en parcial solo el objetivo. El intento propio permite reconocer EN_CURSO exclusivamente para su archivo/ID exactos. Un fallo intermedio cierra las retenciones ya abiertas y no llama al writer.

El adaptador documental anterior se invoca una vez. Su verificación/checkpoint se conservan. Después se consultan el intento y el archivo: CONFIRMADO solo se devuelve con intento final confirmado, archivo COMPLETADO sin marcador, resultado idéntico e IDs/expediente/gabinete/valores coherentes. Si se pierde la respuesta del checkpoint pero quedó persistido, esta lectura recupera el resultado. Sin prueba durable suficiente, se devuelve INCIERTO y se conservan los IDs observados válidos; no se reejecuta el writer.

`ComposicionDocumentoImportacion.Crear(http)` enlaza autoridad, mismo repositorio, mismos límites/presupuestos PDF, staging y adaptador reales. Se compila con el proyecto; no se invoca contra un ambiente real. `ObtenerBarreraDocumental` reutiliza el lector anterior y `ObtenerIntentoAlmacenamiento` lee el histórico propio por fase/huella, sin caducar una reserva para habilitar replay.

## Detención, autoridad y límites

Una detención, revocación o pérdida de staging después de reservar impide crear el adaptador/iniciar el writer. El intento reservado permanece observable para recuperación explícita. Detener durante el writer conserva la señal y permite registrar su resultado confirmado. Se revalidan identidad/destino/recursos antes de efectos usando el snapshot del lote, sin obtener otra publicación del perfil.

Confirmar un archivo no declara completitud de raíz/lote. 10.7 sigue abierta para consolidar esos estados desde todos los resultados originales; 10.8 para reconciliar con evidencia; 10.9/10.10 mantienen implementación parcial por esas dependencias. La exposición de avance/detención/reconciliación de 10.11 sigue pendiente. No se modifica el ASMX ni se expone directamente el adaptador en este corte.

La función de almacenamiento, creación legacy, carga individual, SII e ImportarServicioWeb permanecen intactos. Se respeta la decisión del usuario de descartar aislamiento adicional del XML frente a otras cargas; las retenciones descritas protegen originales propios, sin afirmar exclusión global de writers externos.

## Evidencia ejecutada

| Verificación | Resultado y alcance |
|---|---|
| `node --test tests/doc110-perfil-importacion-behavior.test.cjs` | PASS, arnés completo Framework DOC-110/DOC-111 y regresión DDL de UPDATE de intentos. |
| Una fase documental | PASS: un writer/checkpoint, IDs persistidos, sin completar raíz/lote, histórico sin acceso al disco y clave distinta sin replay. PDF real local y persistencia/writer dobles. |
| Respuesta perdida e incertidumbre | PASS: confirmación durable recuperada por lectura; fallos de writer/verificación/checkpoint conservan incertidumbre/IDs y no repiten. |
| Detención/revocación | PASS: después de reservar no crea adaptador/writer; durante writer conserva señal y resultado confirmado. |
| Barreras completas | PASS: archivo pendiente bloquea EXPEDIENTE/LOTE; otra raíz pendiente bloquea solo LOTE; efecto incierto bloquea global, completado conserva su recepción. Metadatos sintéticos. |
| Retención física | PASS: original hermano retenido durante el efecto completo y liberado al terminar; original faltante impide reservar. PDF local. |
| Parcial | PASS: archivo elegible avanza conservando rechazados y todos los totales; objetivo inelegible se rechaza. |
| MSBuild Debug completo | PASS, salidas/intermedios `.tmp/doc111-fase-documento-build/`; log local `.tmp/doc111-fase-documento-build.log`, sin despliegue. |
| OpenSpec estricto y `git diff --check` | PASS. Funciones legacy protegidas y SII/ImportarServicioWeb sin diferencias. |

SQL DML/commit/rollback/carrera reales del nuevo flujo, HTTP/IIS autenticado, almacenamiento/incorporación física/XML reales y E2E: **NO_EJECUTADO**. Las pruebas usan los núcleos productivos y dobles explícitos; no acreditan transacciones o efectos reales. Se mantienen las autorizaciones de ambiente/cuentas pendientes para la aceptación.
