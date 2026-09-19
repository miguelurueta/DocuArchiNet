# Metadata y despliegue

- Ticket: DOC-68
- Cambio: `doc-68-listado-tipologias`
- Plataforma: ASP.NET WebForms/ASMX, VB.NET, .NET Framework 4.6.1, MySQL 5.1.
- Rollout: gate apagado por defecto.
- Rollback: retirar cableado/DTOs aditivos; no hay migración ni escritura de datos.
- E2E: reutilizar `tools/e2e`; restaurar gate a `false` y listas vacías.
