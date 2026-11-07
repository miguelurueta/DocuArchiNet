# DOC-111: contrato del ejecutor en el cierre de reserva

Actualización posterior: la sesión 12 cerró la reserva 5 y almacenó el primer documento mediante intento 6. Se detuvo en su verificación por NOMBRE no visible/vacío en gabinete, con nombre original correcto en inventario e índice. [Estado vigente y diagnóstico](doc111-intento6-nombre-20261101.md). No repetir el primer almacenamiento.

La sesión 11 terminó sin aceptación: `CerrarReservaSinInicio`, HTTP 409, `CONFLICTO_REVISION`. SELECT posterior de las 10:31:12 UTC confirma lote 2/revisión 13, intento 5 RESERVADO, expedientes 648/649 confirmados y el mismo historial. No hubo cierre, nuevo intento ni avance de almacenamiento en esta sesión.

La causa fue una validación incorrecta introducida en el cierre: se comparaba `Intento.EjecutorRef` con `Contexto.UsuarioRef`. El coordinador existente asigna `WEB_DOCUMENTO` al ejecutor técnico; el usuario se comprueba por separado mediante el contexto persistido, pertenencia del lote y autoridad vigente. La prueba añadida anteriormente sobrescribía ese campo con el usuario y ocultaba la incompatibilidad.

Se corrige el guard para el canal técnico `WEB_DOCUMENTO`. La prueba de reserva documental ahora conserva `request.EjecutorRef` recibido del coordinador real, y la regresión de cierre exige que siga siendo el identificador técnico, diferente del usuario. Se conservan las comprobaciones de usuario ajeno, canal inesperado, revisión, evidencia, fin, diario, CAS, exclusión e idempotencia. No se cambia almacenamiento legacy ni Web.config.

Verificación:

- Arnés Framework y coherencia SQL/DDL: PASS con el coordinador y ejecutor técnico reales, SQL/escritores dobles declarados.
- Build Debug aislado `.tmp/doc111-ejecutor-build`: 0 errores y 308 advertencias existentes.
- Prueba adicional con metadatos reales obtenidos mediante SELECT, sin bloqueos: guard puro de cierre con callbacks SQL/evento que no escriben, PASS; revisión 13, contrato de ejecutor correcto. `tools/e2e/artifacts/doc111-lot2-guard-probe.json`. No acredita transacción real ni cierre efectivo.
- Los 27 archivos protegidos siguen idénticos a HEAD normalizando finales de línea.

Fuentes: SHA-256 `5930f399e8518643e6227d26fe544e48a6091318fca2e14b4214fec501edf99c`, 110 archivos; manifiesto `doc111-sha-ejecutor-20261101.json`. DLL desplegada a las 10:34:15 UTC: `892853f896bf8784c751b73da7ed6c5344d78df8c00c8e84c5bb49dc8a530351`. Backup `.tmp/doc111-ejecutor-deployment-20261101-103415/previous.dll`, SHA-256 `c35831b4086eaf367e28910d5afd49d826ec70061b1855b67b29deb33bf2c8d6`. Salud del sitio en `tools/e2e/artifacts/doc111-ejecutor-deployment.json`.

Siguiente sesión 12: mismo lote 2/intento 5/expedientes 648/649, mismas autorizaciones y credenciales exclusivamente en la consola enmascarada. No se reutilizan ni conservan los secretos de la sesión anterior. El runner vuelve a comprobar el estado real antes de cualquier escritura; el resultado se distingue de REC completo. **121/130**, cierre y almacenamiento reales todavía pendientes.
