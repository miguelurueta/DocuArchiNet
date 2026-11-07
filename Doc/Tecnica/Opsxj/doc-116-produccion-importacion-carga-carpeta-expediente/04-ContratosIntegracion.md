# DOC-116 — 04-ContratosIntegracion

- Ticket: DOC-116
- Cambio OpenSpec: doc-116-produccion-importacion-carga-carpeta-expediente
- Clasificacion: cross_cutting

## Contratos e integraciones

ASMX/ASHX existentes, sin endpoints ni flags productivos nuevos. Reutilizados openSession, createDriver, safePost, consola interactiva y DSN MembershipUsers. El ejecutor de lote preparado mantiene setup cero; la entrada explícita de setup valida escenario y presupuestos distintos EXP01/02/03. La cuenta de control solo envía SELECT; el puente administrativo usa su conexión registrada.

El contexto de EXP03 fija actor 141/empresa 2/nivel 215/concesión 629, no propietario. El recurso nativo contiene IDs y hash de ambiente, se verifica contra la conexión vigente antes de mutar y exige tablas InnoDB. CAS solo de agregar_expediente, diario durable, bloqueo y readback independiente. Las variantes distintas del soporte principal/aislado se conservan y sus SHAs se registran.

La consulta de identidad usa un lote web con contexto comparable al destino vigente; la fixture de repositorio 11 no sirve por sus ruta/sello sintéticos. Los intentos previos se detuvieron sin preparar y quedaron documentados.

EXP02 cliente conserva caseComplete=false/internalFailure=NO_EJECUTADO. EXP03 exige denialUnchanged=true, permissionRestored=true y cero creaciones. El rechazo o falta de restauración nunca se convierte en PASS.
