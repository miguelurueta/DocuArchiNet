# DOC-118 — recuperación interna V2, RV-05

Implementada6.5/D-15/RQ-15 tras «continua» posterior a RV-04 y a la recuperación limitada propuesta. El mapa se amplió antes del código y queda22/32; las mismas10 tareas reales/finales anteriores siguen pendientes. No se trasladó trabajo a otro ticket.

El coordinador y repositorio incorporan parciales de recuperación interna y puertos sin escritor. El coordinador resuelve propietario/ámbito/destino actuales, carga la vista del servidor, valida plan/inventario y reautoriza. Clasifica la evidencia V2 original y el diario antes de adquirir la guardia. Un gate sin retorno, IDs incompletos, V1/69, stop negativo, cambio de identidad o época no conceden recuperación.

La rama negativa solo admite PREPARADO coherente sin gate, retorno o resultado. Exige guardia y tres SELECT de ausencia en gabinete, inventario e índice del expediente completo. Es deliberadamente conservadora:otro documento del expediente impide este cierre. Persiste RECHAZADO/EJECUCION_CERRADA_SIN_INVOCACION, retira únicamente el marcador exacto y devuelve el archivo a NO_INICIADO; no confirma un documento ni encadena ALMACENAR. Conserva recepción, PDF, expediente, valores e inventario.

La rama positiva requiere RETORNO_OBSERVADO y autorización concordantes, IDs completos registrados por el servidor y verificación física mediante el catálogo existente por los mismos IDs. El comando de comprobación conserva EN_CURSO y EsVerificacion=true, por lo que los puertos de almacenamiento lo rechazan. Confirma esos IDs sin repetir almacenamiento ni convertir el intento a INCIERTO; conserva una señal de stop.

El repositorio vuelve a autorizar bajo locks y exige la misma vista, plan, evidencia, diario, IDs/fechas de evento, revisiones y marcador. ConservarDurante mantiene la guardia mientras Operar realiza commit/rollback. Resultado, punteros, evento y consolidación son atómicos en ese código. El historial concordante no toca el original ni realiza nuevas escrituras, incluso después de otra reclamación o de una respuesta perdida. No se añade una transición general EN_CURSO→NO_INICIADO.

Verificación ejecutada:

-22 casos nuevos dentro del harness .NET Framework:negativo/positivo, original preservado, V1/69, gate sin retorno, IDs parciales, lector activo, original ausente/reemplazado, huellas discrepantes, stop, revisiones, autoridad, vista/plan/diario/token cambiados, marcador posterior, rollback en cuatro escrituras/consolidación, guardia perdida, historial, respuesta perdida, bloqueo de puertos de almacenamiento, privacidad y otra época.
- Staging/guardia reales sobre PDFs sintéticos; comprobación física SELECT y transacción son dobles locales. **No acredita MySQL real, una instalación de producción o recuperación de intentos históricos.** El escritor no se invoca.
-18/18 pruebas Node de comportamiento/cliente y205/205 herramientas:223 disjuntas. Los casos internos C# previos19/21/20/20 y los22 nuevos están incluidos en el harness; no se añaden al contador Node. El chequeo DDL también incluye el parcial nuevo.
- Las14 fuentes históricas conservan12 archivos intactos y2 deltas de pruebas.10 informes históricos no cambian. De16 fuentes de la evaluación,15 siguen idénticas; el comando de comprobación tiene un delta autorizado antes/después sin reescribir el hash histórico.

No hubo nuevas sesiones, HTTP de negocio, SELECT reales o efectos documentales. Consumos reales1276 HTTP/113 SELECT/9 aperturas, sin ampliarlos.41/53/69 protegidos. Sin despliegue, DLL/PDB, configuración, permisos, workers o migraciones. La operación sigue interna; RV-06 debe exponerla con sesión/CSRF/contexto. Otro proceso/nodo o reinicio mantiene bloqueo por la época del proveedor.

Evidencia:[JSON local](doc118-recuperacion-v2-local-20261109.json).
