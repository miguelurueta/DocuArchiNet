# DOC-116 EXP-02 — resultado descartado en cliente

PASS de esta variante revisado el 2026-11-05. Plan 33, lote 17, publicación 38 y nivel 7. Expediente 659, relación 1084, raíz 27. La raíz 28 permanece sin crear.

La primera creación se confirmó; se descartó su resultado en el cliente, se consultó el lote y se verificaron la identidad y el checkpoint persistidos por SELECT antes del replay idéntico. La repetición conservó IDs, estado y huellas sin crear otro expediente. No se repitió ninguna operación incierta.

Presupuesto ejecutado: una preparación, una confirmación/un lote, dos PDF de 677 bytes recibidos en staging y dos solicitudes de creación/replay. Un expediente nuevo, cero documentos definitivos y cero cambios de permisos. Recursos nuevos y anteriores conservados, incluido lote 16 y expedientes 657/658.

Las huellas antes/después coinciden en las seis tablas operativas dentro del contexto preservado: 32 planes, 12 lotes, 20 raíces, 21 archivos, 34 intentos y 125 eventos; límites anteriores plan 32/lote 16. Los controles independientes verificaron la identidad, relación, clasificación, Archivo Gestión, ausencia de efectos documentales y segunda raíz intacta.

Salida explícita: caseComplete=false, internalFailure=NO_EJECUTADO, responseObservation=CLIENT_RESULT_DISCARDED. No acredita pérdida de respuesta HTTP, caída del servidor ni fallo postlegacy/precheckpoint. El fallo interno autenticado sigue NOT_COMPOSED y el caso EXP-02 completo no se considera aceptado.

Procedencia: `tools/e2e/artifacts/doc116-exp02-20261105-authorized.json`; SHA256 44fb66a5b1ae09c204edbfbfb3d730c9dbab7367135be51b52b5759c24a794b7. Código revisado 7c2f443540ab1431fe804ec0402eeb1aa84e95c405b513013c8038ae8506a8d1. Backend sin cambios 4134f57f66fc259b5f33a553577c9a07e6bf176b2da027aaeeb8576325c282c5. Evidencia saneada: `doc116-exp02-resultados-20261105.json`. Validación local previa: 85/85 PASS. No repetir esta sesión.

Tarea 4.3 satisfecha para las variantes viables y autorizadas; no oculta la dependencia del fallo interno. Progreso 17/20. Tareas pendientes: 4.1 parcial (recursos/autorización EXP03), 4.4 y 4.5. DOC-116 permanece abierto.
