# Baselines y limitaciones heredadas

| Fuente | Condición | Resultado comprobado | Uso en DOC-105 |
| --- | --- | --- | --- |
| DOC-100 | Baseline del visor | Primer canvas 878 ms; scroll 3.548 ms; zoom 435 ms | Umbrales históricos, no presupuesto remoto |
| DOC-100 | Scheduler posterior | Primer canvas 988 ms; scroll 2.773 ms; zoom 104 ms; resultado global `FAILED` por dependencia pendiente de DOC-101 | Evidencia de mejora interactiva, no cierre transversal |
| DOC-101 | Cargador selectivo | Mediana primer canvas 519 ms; matriz de capacidades `PASSED` | Baseline local posterior; Wacom físico no ejecutado |
| DOC-102 | Transporte adaptativo | Mismo PDF de 34 páginas: 1.123 ms a 624 ms, modo `STREAM`, un `200`, una tarea larga, `PASSED` | No interpretar `STREAM` como descarga parcial |
| DOC-102 | POC linealizado | La cobertura única inicial bajó de 28 % a 9,12 %, sin mejora equivalente en transferencia física/tiempo por solapamientos | Separar bytes únicos de físicos |
| DOC-103 | Guardado de grafo | Un POST, efecto único y control `SELECT` sobre `workflowdocument`, `PASSED` | Única mutación permitida en DOC-105 |
| DOC-104 | Procesamiento servidor | Escenario normal real con un POST y controles `SELECT`, `PASSED` | Reutilizar protecciones y clasificación cerrada |
| DOC-104 | Documento firmado | Fixture firmado no disponible; riesgo residual aceptado y documentado | No simular ni declarar esta capacidad ejecutada |

## Comparabilidad exigida

Una comparación DOC-105 solo es válida con el mismo PDF, consumidor, ambiente, navegador, perfil de red y estado de caché. Los resultados locales anteriores no sustituyen las celdas remotas de 10/20/50/100 Mbps.

## Pendientes que impiden una afirmación transversal hoy

- Existe evidencia autorizada `remote-100` fría/caliente para Correspondencia; faltan sus perfiles 10/20/50 Mbps. Producción ya cuenta con matriz fría/caliente completa en 10/20/50/100 Mbps.
- Existe evidencia 10/20/50/100 Mbps para aprobación/externo; falta cada clase de handler simple.
- Los presupuestos finales de primera página ya se derivaron y verificaron contra las ocho celdas de Producción; no se extrapolan a capacidades no ejecutadas de otros consumidores.
- El rollback debe ensayarse en ambiente autorizado; un procedimiento escrito no equivale a prueba ejecutada.
