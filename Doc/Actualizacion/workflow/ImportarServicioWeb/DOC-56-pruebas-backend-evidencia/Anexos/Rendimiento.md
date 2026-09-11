# Rendimiento y disponibilidad

`MySqlExternalServiceTelemetryRepository.Registrar` persiste `FechaInicioUtc`, `FechaFinUtc` y `DuracionMs` por llamada externa. `ConsultarDisponibilidad` agrupa por proveedor, operación y periodo y calcula total, exitosos, fallidos, disponibilidad porcentual y latencia promedio.

Las operaciones instrumentadas de SII son `SOLICITAR_TOKEN`, `CONSULTAR_SELLO` y `DESCARGAR_ANEXO`. Cada fase posee su propia correlación; dos pares token/sello en el recorrido completo corresponden a consulta y ejecución, no a duplicación dentro de una fase.

La muestra individual autorizada está en `Evidencias/2026-09-11-e2e-rup-telemetria.md`. Las métricas concurrentes siguen pendientes y no deben mezclarse con las de ejecución individual.
