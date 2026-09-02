# DOC-44 — Estabilización Workflow

Estabilización reversible del consumidor moderno de Notas en `workflow/Webworkflow.aspx`, sin retirar el fallback legacy ni modificar el contrato compartido de DOC-42.

## Documentos

1. [Arquitectura](01-arquitectura.md)
2. [Contrato](02-contrato.md)
3. [Flujo y seguridad](03-flujo-y-seguridad.md)
4. [Pruebas y evidencia](04-pruebas-y-evidencia.md)
5. [Inventario de funciones](05-inventario-funciones.md)
6. [Liberación y operación controlada](06-liberacion-operacion-controlada.md)

## Diagramas

- [Índice](Diagramas/README.md)
- [Arquitectura](Diagramas/01-arquitectura.md)
- [Casos de uso](Diagramas/02-casos-de-uso.md)
- [Clases y componentes](Diagramas/03-clases.md)
- [Estados](Diagramas/04-estados.md)
- [Secuencia](Diagramas/05-secuencia.md)

## Decisiones

| Decisión | Evidencia |
| --- | --- |
| D-01: alcance Workflow | Inventario limitado a `Webworkflow`, cliente, configuración, pruebas y documentación. |
| D-02: contrato único | `WorkflowNotesModern` usa JSON e identidad explícita contra el ASMX DOC-42. |
| D-03: exclusión y rollback | Panel moderno y disparador legacy son mutuamente excluyentes; entrega apagada. |
| D-04: regresión verificable | Política local automatizada y E2E real protegida por tres autorizaciones. |
