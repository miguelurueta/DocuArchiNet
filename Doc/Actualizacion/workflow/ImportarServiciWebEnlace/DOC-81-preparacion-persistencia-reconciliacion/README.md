# DOC-81 — Preparación, persistencia y reconciliación ENLASE

## Resultado implementado

DOC-81 conecta la capacidad `ANEXOS_RADICADO_ENLASE` con el núcleo moderno de preflight, intención idempotente, descarga segura, almacenamiento adaptado y reconciliación. Una importación solo queda `Completado` cuando existe una relación lógica única con la tarea y el recurso físico existe una sola vez. El flujo no asigna, cierra ni avanza la tarea y no ejecuta efectos de expediente de constancias.

## Contenido

- [Arquitectura y flujo](01-ARQUITECTURA-Y-FLUJO.md)
- [Estados, idempotencia y reconciliación](02-ESTADOS-IDEMPOTENCIA-RECONCILIACION.md)
- [Inventario y trazabilidad](03-INVENTARIO-Y-TRAZABILIDAD.md)
- [Pruebas, seguridad y rollback](04-PRUEBAS-SEGURIDAD-ROLLBACK.md)
- `Diagramas/*.puml`: fuentes UML 2 en PlantUML.
- `diagram-contract.json`: referencias estructurales al código.

## Convención

- `CODE:` símbolo resoluble en código.
- `EXT:` actor o sistema externo.
- `CONCEPT:` estado o decisión conceptual.

La prueba documental valida archivos, sintaxis básica y correspondencia estructural de las referencias declaradas. No demuestra por sí sola fidelidad conductual completa ni cobertura funcional total.