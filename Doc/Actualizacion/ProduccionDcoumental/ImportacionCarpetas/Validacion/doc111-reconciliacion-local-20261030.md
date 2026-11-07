# DOC-111 — Reconciliación explícita de evidencia incierta

Fecha: 2026-10-30. HEAD base `b2e93b726f221868cf2b82d43200a8e568871117`, workspace con cambios locales previos. Tarea 10.8 completada localmente; 110/130. Sin commit, despliegue ni efectos reales.

## Contrato y límites

`CoordinadorImportacionCarpetas.Reconciliacion.vb` añade `ReconciliarIntento(loteId, intentoId, revisionEsperada)`. Resuelve un intento CREAR_EXPEDIENTE o ALMACENAR persistido como INCIERTO, con reclamación propia y todos los IDs conocidos. Comprueba actor/empresa/destino actuales, plan/snapshot, metadatos originales y valores por elemento. Reautoriza después de verificar y antes de guardar. Un resultado confirmado coherente permite repetición de lectura sin exigir una revisión antigua ni emitir eventos.

RESERVADO/EN_CURSO, antigüedad, vencimiento, IDs incompletos, resultado ausente o SELECT discordante no habilitan ejecución ni liberan la reclamación. Los errores técnicos del verificador conservan incertidumbre sin exponer mensajes privados. La revocación impide guardar. La ruta no reserva intentos, consulta proveedores, abre staging ni invoca creación/almacenamiento/incorporación. RECEPCION e INCORPORAR no se resuelven mediante esta capacidad; recepción mantiene su mecanismo existente.

`VerificadorReconciliacionImportacion` tiene exclusivamente cuatro puertos de consulta. Reutiliza catálogos y lectores SELECT existentes para creación y documento. Exige el ciclo Archivo Gestión por nombre y el ID de ciclo registrado, además de relaciones y valores esperados. No busca candidatos por nombre para atribuir un efecto. Los comandos `PrepararVerificacion` conservan el estado incierto; su marca inmutable impide usarlos en los inicios/resultados ordinarios del repositorio y bindings legacy. Las fábricas ordinarias siguen exigiendo una reserva nueva.

## Persistencia

El parcial `.Reconciliacion.vb` pertenece al repositorio único. Lee lote/plan/raíz/archivo/intento bajo el bloqueo de lote y vuelve a comparar la vista completa antes de escribir. Solo acepta la confirmación canónica de la evidencia previamente persistida: el verificador no puede sustituir IDs, valores o ciclo en su respuesta.

La transacción convierte el intento INCIERTO en CONFIRMADO mediante CAS por identidad, fase, token/hash y estado no finalizado. Creación fija la identidad de la raíz y libera su reclamación; documento confirma el archivo y libera su reclamación. Si procede, la raíz incierta vuelve a EXPEDIENTE_CONFIRMADO. Se conservan todas las demás reclamaciones, política, totales y señal de detención.

El lote pasa a LISTO o conserva DETENIDO según su señal/estado; la consolidación se aplica inmediatamente en la misma conexión/transacción, antes del commit. LISTO → COMPLETADO se añade expresamente a la tabla cerrada para permitir que los últimos checkpoints reconciliados acrediten completitud. El cálculo exige todas las pruebas positivas originales; no se interpreta LISTO como éxito. Otra incertidumbre vuelve a PENDIENTE_VERIFICACION; una detención con trabajo pendiente permanece DETENIDO. Terminar todo conserva tanto COMPLETADO como la señal.

Una repetición exacta del checkpoint no escribe ni emite eventos. Ante respuesta perdida de guardado se consulta de nuevo el resultado persistido; no se repite el guardado ni el efecto. El wrapper transaccional existente debe abortar ante CAS fallido; rollback/carreras MySQL reales siguen pendientes de aceptación.

`ComposicionReconciliacionImportacion` reutiliza sesión, autoridad, catálogo y conexiones vigentes, sin escritores ni staging. Esta capacidad es interna: 10.11, su exposición HTTP, sigue pendiente.

## Verificación local

- PASS: `node --test tests/doc110-perfil-importacion-behavior.test.cjs`, arnés .NET Framework bajo Option Compare Text y contraste de UPDATE de intentos con el DDL, incluyendo reconciliación.
- PASS: creación/documento conocidos, ciclo cambiado, IDs incompletos, metadatos originales alterados, reclamación antigua aún activa, revisión obsoleta y permiso revocado.
- PASS: SELECT negativo/error, guardado fallido, respuesta perdida, repetición histórica y del checkpoint, sustitución de IDs y vista cambiada después de consultar.
- PASS: fallo de cada CAS del documento sin evento/confirmación en el doble; detención conservada con trabajo restante y otra incertidumbre sin liberar.
- PASS: comandos de verificación rechazados por inicios/resultados ordinarios antes de intentar conexión.
- PASS: MSBuild Debug completo, `.tmp/doc111-reconciliacion-build/`, sin despliegue; OpenSpec estricto y `git diff --check`.

Los SELECT, estados y ejecutores SQL son sintéticos. Las fixtures reutilizan PDF local y escritores/persistencia dobles de fases previas; la reconciliación no repite sus escritores. MySQL/HTTP/IIS autenticado, commit/rollback/carreras reales, XML y efectos documentales reales: **NO_EJECUTADO**. No se hereda autorización de corridas anteriores. Las funciones legacy y SII/ImportarServicioWeb siguen intactas. La exclusión adicional del XML frente a otras cargas permanece descartada por instrucción del usuario.
