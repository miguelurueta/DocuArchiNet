# DOC-118 — contraste local de seguridad, recuperación y consolidación

Fecha: 2026-11-07. Origen: D-07/RQ-07, D-08/RQ-08 y D-09/RQ-09. Base `efd7a6b4ec4d981ca4ded4f0efbd08ce03082875`, sin cambios funcionales. Las líneas son referencias de esa base. Salvo indicación contraria, las pruebas citadas están en `tests/ImportacionCarpetasBehaviorTests.cs` y usan HTTP, autoridad, SQL o escritores sintéticos; no equivalen a integración MySQL/legacy nueva.

## Negativos por operación

| Operación/condición | Prueba o inspección existente | Resultado acreditado y límite |
| --- | --- | --- |
| Permiso revocado con sesión/token antiguos | AuthenticatedRevocationCases, línea1764 | Rechaza las 12 operaciones ASMX, incluidas consultar, avanzar, detener, reconciliar y cierre preinicio, antes de leer cuerpo o componer servicios. Autoridad y HTTP sintéticos. |
| Consulta de lote/confirmación sin permiso de gabinete | QueryCases, línea352 | CabinetAllowed=false rechaza consulta de confirmación/exportación; ImportAllowed=false rechaza consulta. No lee páginas de exportación sin autoridad. |
| Consulta con destino cambiado o propietario ajeno | QueryCases, línea352 | Moved=true y UsuarioRef ajeno rechazan la consulta; lote/archivos no se modifican. |
| Detención sin autoridad o con destino cambiado | ControlCases, línea267, aserciones en320–329 | allowed=false, usuario ajeno y SelloDestino cambiado rechazan; revisión obsoleta no cambia señal ni conteos. Repetición de detención confirmada no duplica mutación. |
| Replay documental con permiso revocado | DocumentPhaseCases, línea2180 | Allowed=false rechaza replay antes de abrir staging; mantiene una sola escritura documental. |
| Revocación después de reserva y antes de escritor | DocumentPhaseCases, línea2193; DocumentAdapterCases, línea2519 | Modo revoke conserva reserva y cero escritores; RevokeBeforeWriter no ejecuta escritor. No prueba revocación real en base. |
| Avance con clave distinta/revisión anterior | DocumentPhaseCases, línea2180 | CONFLICTO_REVISION sin otro escritor después del efecto confirmado. No acredita por sí solo exclusión de dos sesiones con revisiones vigentes. |
| Destino/identidad o snapshot incompatibles | CreationCommandCases y DocumentAdapterCases | Rechazos de contexto/identidad y huella/valores alterados antes de efectos. El mapa no atribuye a cada endpoint una carrera real de cambio de destino. |
| Reconciliación con revocación durante verificación | ReconciliationCases, línea2368 | RevokeAfterVerify rechaza sin checkpoint/evento. |
| Reconciliación con revisión obsoleta o vista cambiada | ReconciliationCases, líneas2369 y2386 | Revisión antigua no llega al verificador; cambio de revisión del archivo después de SELECT impide persistencia. |
| Publicación posterior frente a versión histórica | PublishedReadCases, línea1842 | VersionPublicadaId=6 mientras se consulta versión5: conserva la identidad histórica válida y rechaza sustituirla por6. Es prueba del lector de publicación, no de un lote real activo durante publicación nueva. |

## Brechas delimitadas de seguridad

La revocación autenticada está cubierta localmente para todas las operaciones públicas. Destino/revisión tienen negativos locales en las capas indicadas. No se ha acreditado una nueva carrera real de revocación o cambio de destino en cada fase; no se repiten cambios de permisos por defecto.

El caso exacto «publicación nueva mientras continúa un lote confirmado» dispone de piezas locales (lector histórico, validación de snapshot y consulta sin sustitución), pero no una aceptación real nueva que publique y continúe ese lote. Los casos COORD02/03 deben comparar el snapshot original antes/después; eso tampoco prueba una publicación concurrente. Si se exige esa integración adicional, debe agregarse una tarea y autorización específica, sin atribuirle el PASS del lector histórico ni modificar una publicación por inferencia.

No se encontró un defecto de producto reproducido al hacer este contraste. No se agregan pruebas que solo cambien una variable de publicación desconectada del servicio y aparenten demostrar continuidad.

## Recuperación por estado

| Estado/evidencia | Operación admitida | Límite |
| --- | --- | --- |
| RESERVADO ALMACENAR/WEB_DOCUMENTO, sin inicio/resultado/evidencia, evento de reserva único, vista exacta y sin detención | CerrarReservaSinInicio | PersistirCierreReservaSinInicio cierra RECHAZADO/RESERVA_CERRADA_SIN_INICIO y libera solo esa reserva. Respuesta pública confirma el cierre, no un documento. |
| Cierre preinicio ya persistido | Repetición del mismo cierre | Devuelve resultado conocido sin afectar una reserva posterior. |
| EN_CURSO o RESERVADO antiguo sin prueba de cierre preinicio | Consulta; bloqueo de nueva ejecución | ReconciliationCases asigna InicioUtc=2000 y comprueba que no verifica/persiste ni libera el marcador. La edad no habilita recuperación. |
| INCIERTO con identidad completa y evidencia concordante | ReconciliarRecurso | Verifica el efecto conocido y persiste resultado/consolidación; no vuelve a almacenar ni crear. |
| INCIERTO sin IDs completos o evidencia fiable | Consulta y conservación de incertidumbre | Cero checkpoint/evento; no sustituir identidad ni liberar reclamación. |
| Detención solicitada con intento activo | Conservar llamada y consultar | No cancelar implícitamente; cierre preinicio no admite detención. Reconciliación de un INCIERTO conocido puede conservar la señal y dejar DETENIDO si queda trabajo. |
| COMPLETADO con resultado durable | Consulta o historial de solicitud autorizada | Mantener IDs; ningún replay del escritor. |

Evidencia local: CloseUnstartedReservationCases (línea2302), ReconciliationCases (línea2348), DocumentPhaseCases (línea2180) y ConsolidationCases (línea2480). Los intentos históricos41/53 nunca son sujetos de estos procedimientos nuevos.

## Consolidación y transacción exterior

`MySqlImportacionCarpetasRepository.Documento.vb:RegistrarResultadoDocumento` ejecuta persistencia del checkpoint y `ConsolidarEnTransaccion` dentro de la misma llamada a Operar con conexión/transacción compartidas. Leer únicamente PersistirResultadoDocumento omite ese caller y conduce a una conclusión incorrecta sobre la consolidación.

`MySqlImportacionCarpetasRepository.Consolidacion.vb` vuelve a cargar el inventario bajo bloqueo del lote y deriva conteos/estados con CAS. La consolidación usa raíces, archivos, bytes y checkpoints originales; los rechazados no desaparecen. No hay endpoint público que deba agregarse para «completar» esa operación.

| Situación | Estado y conservación esperados |
| --- | --- |
| Efecto incierto o marcador incompatible | PENDIENTE_VERIFICACION, sin declarar completitud |
| Intento activo aunque sea antiguo | EN_CURSO, incluso con señal de detención |
| Sin actividad, señal de detención y trabajo sin completar | DETENIDO; se mantienen efectos positivos y originales |
| Todos los efectos requeridos positivos; señal al finalizar el último | Puede resultar COMPLETADO conservando la señal |
| Modo parcial con rechazo original | Mantener conteos originales y resultado parcial correspondiente |

Las pruebas locales verifican derivación, SQL sintético y conexión de helpers; los REC reales históricos acreditan sus propios inventarios. COORD02/03 aún deben verificar el inventario de sus recursos nuevos mediante SELECT autorizado.


## Contención nueva del intento69

Lote24/archivo40 continúa EN_CURSO con reclamación activa y sin evidencia durable. Se conservaron las referencias y la misma huella operativa entre las auditorías de95 y113 SELECT; gabinete/inventario/índice muestran0 registros actuales para expediente670. El archivo41 mantiene NO_INICIADO y ausencia física. No hubo otro ALMACENAR ni cambio de estado. Esto cierra4.6 en su alcance de contención, sin convertir ausencia en permiso de replay ni atribuir un código HTTP no retenido. Reconciliar exige INCIERTO con evidencia y el cierre preinicio no admite EN_CURSO. Véase `doc118-auditoria-fisica-liberacion-20261108.md`.
