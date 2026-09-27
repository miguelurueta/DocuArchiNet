# DOC-83 — Servicios y reglas

## Reglas productivas preservadas

- `ImportarServicioWebFeatureGate.EstaHabilitado(ContextoModuloWorkflow)` exige sesión válida y bandera activa.
- `ANEXOS_RADICADO_ENLASE` separa la capacidad ENLASE de constancias.
- Lectura y UI no pueden modificar los siete controles registrados.
- Ejecución múltiple usa una intención idempotente y resultado por elemento.
- Importar no asigna la tarea.
- `Webworkflow.Buttonaceptar_Click` revalida documentos obligatorios y detiene cualquier resultado distinto de `YES`.
- Resultado incierto no se reintenta automáticamente.

## Componentes DOC-83

- `doc83-sii-enlase-closure-matrix.json`: inventario ejecutable.
- `validateDoc83ClosureMatrix`: verifica encabezado, política, archivos, escenarios, controles, expectativas y brechas.
- `run-doc83-regression.cjs`: ejecuta desde la raíz la batería focal sin duplicar fixtures.
