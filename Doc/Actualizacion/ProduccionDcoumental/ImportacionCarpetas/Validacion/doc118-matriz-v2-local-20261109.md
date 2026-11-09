# DOC-118 — matriz local de intercalaciones V2, RV-07

Verificada6.7 bajo D-12/RQ-12..D-16/RQ-16 y RC-01..RC-07, tras «continua» posterior a RV-06. No cambia contratos ni código de producto. El arnés existente añade12 casos; los121 casos V2 anteriores siguen pasando. Progreso24/34, con las mismas10 tareas reales/finales abiertas. Del plan V2 queda RV-08.

Las ventanas del adaptador utilizan barreras explícitas entre tareas propias, con liberación en finally y esperas acotadas. Se ejecutan adaptador, códec, núcleo del repositorio y guardia reales de código; el escritor, los SELECT y la transacción son dobles. No es una prueba de concurrencia/atomicidad MySQL real ni de una llamada legacy alojada. El arnés compila también el dispatcher y la composición HTTP productivos desde RV-06.

| Condición | Evidencia local concreta | Resultado |
| --- | --- | --- |
| V1, prueba incompleta o versión/origen falsos | Contratos V2:19 casos previos; recuperación: `v1_and_synthetic_69_are_not_backfilled`; matriz: `protected_v1_attempts_are_unchanged_in_mixed_protocol` | Bloqueo sin convertir ni completar evidencia.41/53/69 en el caso nuevo son fixtures sintéticas. |
| Pruebas/IDs/actor aportados por HTTP | HTTP: `unknown_duplicate_coerced_or_forged_fields_block`, `body_is_authoritative_over_asmx_object` | JSON cerrado; no concede capacidad por documento aportado. |
| PREPARADO con adaptador todavía reteniendo | Matriz: `live_prepared_executor_blocks_recovery` | Recuperación bloqueada; el adaptador continúa tras liberar la barrera y confirma con una sola invocación. |
| Autorización durable o escritor activo | Matriz: `live_gate_and_writer_never_grant_recovery` | Ninguna recuperación desplaza al ejecutor; gate sin retorno no prueba ausencia. |
| Retorno durable con IDs mientras sigue la retención | Matriz: `live_observed_ids_still_require_executor_exit` | IDs completos no sustituyen la guardia exclusiva; no hay escritura de recuperación. |
| Checkpoint confirmado mientras sigue la retención | Matriz: `committed_history_is_readable_while_retention_is_live` | Historial sin guardia ni nueva escritura; el ejecutor conserva su único checkpoint. |
| Respuesta de inicio perdida y salida del adaptador | Matriz: `lost_start_reply_closes_only_after_retention_release` | PREPARADO coherente permite cierre de control, sin escritor ni pérdida del PDF. |
| Respuesta de autorización perdida y salida del adaptador | Matriz: `lost_gate_reply_remains_ambiguous_after_exit` | Conserva EN_CURSO y reclamación; handle libre/cero filas no prueban no invocación. |
| Observación persistida pero respuesta perdida | Matriz: `lost_observation_reply_recovers_original_observed_ids` | Verifica y confirma los IDs originales; total de invocaciones permanece uno. |
| Dos recuperadores, rama negativa y positiva | Matriz: `two_recoverers_only_one_commit_then_history` | La guardia excluye al segundo; cuatro escrituras y una consolidación, luego historial. |
| Rollback con otro recuperador disputando | Matriz: `rollback_excludes_contender_then_retry_commits_once`; recuperación previa: rollback en cada escritura/consolidación | Cero escrituras publicadas al fallar; guardia liberada y posterior cierre único. |
| Lector en otro proceso, vivo y terminado | Matriz: `reader_process_exit_allows_only_same_epoch_recovery` | Vivo bloquea; tras salir puede recuperarse el original cuya evidencia sigue siendo de la época del padre. El hijo es un lector sintético, no un ejecutor V2. |
| Evidencia de otra época tras salir el proceso | Matriz: `foreign_epoch_stays_blocked_after_reader_process_exit`; proveedor previo: `other_process_reader_blocks_then_guard_succeeds` | La terminación no acredita otra época, nodo o reinicio. Mantiene bloqueo. |
| Terminación del proceso que conserva un handle | Prueba nativa existente: `process_exit_releases_handle`, dentro de205 pruebas de herramientas | Solo acredita liberación del handle en fixture NTFS; no convierte una prueba ajena ni demuestra un resultado de invocación. |
| Original ausente, reemplazado, hardlink/junction o hash distinto | Proveedor V2:21 casos previos; recuperación: `original_missing_or_replaced_is_not_recreated` | No crea, copia ni sustituye originales para recuperar. |
| Guardia perdida o Dispose concurrente | Proveedor: `concurrent_dispose_waits_for_guard_section`; recuperación: `guard_survives_writes_and_lost_guard_prevents_commit` | Exclusión hasta commit/rollback; guardia inválida impide confirmar. |
| Diario duplicado, desordenado o contradictorio | SQL V2: `duplicated_missing_and_unknown_journal_rejected`; recuperación: `corrupted_or_reordered_history_is_rejected` | Conserva evidencia y reclama bloqueo. |
| Autoridad, destino, revisión, token, vista o plan cambiados | Recuperación: `authority_denied_before_read_or_commit`, `snapshot_changes_before_commit_are_rejected`; HTTP: negativos de autoridad/revisión | Rechaza persistencia contra vista obsoleta y no expone resultados ajenos. |
| Respuesta de recuperación perdida, replay y reclamación posterior | Recuperación/HTTP: historial después de otra reclamación y respuesta perdida | Solo historial autorizado; no borra marcador posterior ni vuelve a consolidar. |
| Detención e inventario original | Recuperación: `stop_blocks_negative_and_preserves_positive`, `prepared_closes_control_preserves_original_and_inventory` | Negativo con stop bloqueado; positivo conserva stop, identidad y totales. |
| Compatibilidad V1 y límites públicos | Regresión V1 existente de documentos, reconciliación y cierre;133 casos V2 y huellas de producto sin cambios en RV-07 | No se activa escritor V2 habitual, backfill, transición general, TIFF, limpieza o worker. |

Pasaron18/18 pruebas Node de comportamiento/cliente y205/205 de herramientas:223 disjuntas. Los12 casos nuevos pertenecen al arnés y no se suman al total Node. Los dos procesos lectores nuevos terminaron; las tareas pausadas se liberaron y esperaron antes de cerrar fixtures. Los tres hijos de las pruebas previas del proveedor también siguen cerrándose en esa corrida.

No hubo sesiones, HTTP de negocio, SELECT reales, efectos documentales, despliegue, DLL/PDB o configuración del runtime. Consumos reales1276 HTTP/113 SELECT/9 aperturas, sin cambios.41/53/69 reales permanecen protegidos. La matriz es completa para la verificación local acotada del protocolo; no completa COORD-02/03 reales, no califica producción ni autoriza ejecutar.

Evidencia y referencias verificables:[JSON de la matriz](doc118-matriz-v2-local-20261109.json).
