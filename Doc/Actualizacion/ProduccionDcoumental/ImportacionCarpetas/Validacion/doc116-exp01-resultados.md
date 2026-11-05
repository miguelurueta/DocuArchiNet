# DOC-116 EXP-01 — aceptación real

PASS revisado el 2026-11-05. Plan 32, lote 16, publicación 38, nivel destino 7. Tras seleccionar visualmente el nivel 8, las dos creaciones se comprobaron por IDs y SELECT independientes bajo el nivel 7; el nivel 8 no ganó expedientes.

| Raíz | Expediente | Relación al nivel 7 | Replay |
| --- | --- | --- | --- |
| 25 | 657 | 1082 | Mismos IDs, sin cambios |
| 26 | 658 | 1083 | Mismos IDs, sin cambios |

Presupuesto ejecutado: una preparación, una confirmación, dos PDF de 677 bytes recibidos en staging y cuatro solicitudes de creación/replay. Cero documentos definitivos y cero cambios de permisos. Conservados ambos expedientes y originales.

Las huellas antes/después coinciden en las seis tablas operativas para los planes y lotes anteriores del contexto observado (31 planes, 11 lotes, 18 raíces, 19 archivos, 30 intentos y 114 eventos). Los controles finales comprobaron las dos identidades y relaciones, clasificación, Archivo Gestión y ausencia de documentos definitivos.

Procedencia: `tools/e2e/artifacts/doc116-exp01-20261105-attempt3.json`; SHA256 c1581523fc8d8bff301f653113773ed121a5d713c69db2ff13d337ecaf037346. Backend sin cambios: 4134f57f66fc259b5f33a553577c9a07e6bf176b2da027aaeeb8576325c282c5. Huella del código revisado: 78b8448db71e4d6581edb7b026f2e35c7a1f7bf33146b50f291abe3cca32ffcf. Resultado saneado detallado: `doc116-exp01-resultados-20261105.json`. Pruebas locales seleccionadas: 83/83 PASS.

No repetir esta sesión: sus efectos ya están confirmados. Los intentos previos y el diagnóstico se conservaron; no llegaron a PrepararPlan. La referencia web se corrigió de la fixture de repositorio 11 al lote HTTP 9.

Progreso: 16/20 tareas. EXP-02 cliente y EXP-03 reales pendientes de autorización específica y ejecución; EXP-02 interno sigue sin composición autenticada. El PASS de EXP-01 no cierra DOC-116.
