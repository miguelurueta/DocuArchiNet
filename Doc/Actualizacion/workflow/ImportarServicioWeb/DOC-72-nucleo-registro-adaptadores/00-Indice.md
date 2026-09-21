# DOC-72 — Núcleo frontend y registro de adaptadores

## Propósito

Este paquete describe la implementación existente de DOC-72 en el monolito `DocuArchiNet`: cuatro módulos JavaScript, integración WebForms bajo gate y el borde ASMX consumido. No atribuye al frontend fases mutadoras que pertenecen al backend ni presenta como accesibles desde la UI operaciones que aún no tienen controles.

## Contenido

1. [Arquitectura y decisiones verificadas](01-Arquitectura.md)
2. [Integración WebForms y gate](02-IntegracionWebForms.md)
3. [Contrato HTTP y adaptadores](03-ContratoAdaptadores.md)
4. [Estados, transiciones y concurrencia](04-Estados.md)
5. [Accesibilidad](05-Accesibilidad.md)
6. [Pruebas y evidencia](06-PruebasEvidencia.md)
7. [Metadata y reversa](07-Metadata.md)
8. [Inventario técnico](08-InventarioTecnico.md)
9. [Casos de uso](09-CasosDeUso.md)
10. [Alcance de revisión y pendientes](10-AlcanceRevision.md)
11. [Contrato automático de diagramas](diagram-contract.json)

## Diagramas requeridos

- [Componentes](Diagramas/01-Componentes.md)
- [Apertura y consulta](Diagramas/02-AperturaConsulta.md)
- [Ejecución única programática](Diagramas/03-EjecucionUnica.md)
- [Validaciones y errores del borde ASMX](Diagramas/04-ValidacionesErrores.md)
