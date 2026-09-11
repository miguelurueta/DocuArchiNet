# DOC-56 — documentación técnica de la implementación

Este paquete describe el código efectivo de `ImportarServicioWeb` mediante símbolos, contratos, tablas y decisiones ejecutables. Los diagramas están segmentados para no omitir llamadas ni mezclar responsabilidades.

## Documentos

- [01 — Resumen técnico](01-ResumenTecnico.md)
- [02 — Impacto UI](02-ImpactoUI.md)
- [03 — Servicios y reglas](03-ServiciosYReglas.md)
- [04 — Contratos e integraciones](04-ContratosIntegracion.md)
- [05 — Pruebas y evidencia](05-PruebasEvidencia.md)
- [06 — Rollout y rollback](06-RolloutRollback.md)
- [07 — Metadatos e índices](07-Metadata.md)
- [08 — Inventario de funciones y ASMX implementados en DOC-55](08-InventarioFuncionesYAsmx-DOC55.md)

## Anexos

- [Rendimiento y disponibilidad](Anexos/Rendimiento.md)

## Diagramas derivados del código

1. [Frontera ASMX y composición](Diagramas/01-frontera-composicion.md)
2. [Consulta y preview SII](Diagramas/02-consulta-preview-sii.md)
3. [Preflight, idempotencia e intención](Diagramas/03-preflight-intencion.md)
4. [Ejecución y almacenamiento](Diagramas/04-ejecucion-almacenamiento.md)
5. [Máquina de estados y reintento](Diagramas/05-estados-reintento.md)
6. [Reconciliación y contrato de salida](Diagramas/06-reconciliacion-salida.md)
7. [Persistencia y telemetría](Diagramas/07-persistencia-telemetria.md)
8. [Interior del almacenamiento legacy reutilizado](Diagramas/08-almacenamiento-legacy.md)
9. [Flujo completo con funciones implementadas](Diagramas/09-flujo-funciones-implementadas.md)

`Evidencias/` conserva únicamente resultados saneados: nunca credenciales, cookies, tokens, cadenas de conexión, URLs firmadas ni cuerpos externos crudos.
