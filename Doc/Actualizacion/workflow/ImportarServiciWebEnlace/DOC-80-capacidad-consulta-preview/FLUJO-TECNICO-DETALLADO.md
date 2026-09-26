# Flujo técnico detallado

La representación normativa del recorrido está dividida en cuatro diagramas UML 2 escritos en PlantUML:

1. `Diagramas/01-componentes.puml`: Cliente → ASMX → contrato/proveedor → cliente SII → SII.
2. `Diagramas/02-consulta-anexos-secuencia.puml`: consulta, contexto autoritativo, referencias, normalización y errores.
3. `Diagramas/03-preview-descriptor-secuencia.puml`: reconsulta del anexo, descarga segura y descriptor temporal.
4. `Diagramas/04-streaming-actividad.puml`: decisiones HEAD/GET, autoridad, reclamación y consumo único.

Estas fuentes incluyen clases y métodos reales, parámetros, retornos, decisiones, alternativas y resultados HTTP. `diagram-contract.json` mantiene la trazabilidad estructural contra el código.
