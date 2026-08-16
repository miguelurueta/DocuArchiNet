# DOC-12 — Lista moderna de destinos

- Fecha: 2026-08-16
- Estado: implementación, QA manual y E2E autorizada completadas; gate restaurado al valor seguro.
- Alcance: Presentation de `workflow/Webworkflow.aspx`; no modifica el motor de transición, Application, Domain ni Infrastructure de envío.

## Archivos entregados

| Ruta | Responsabilidad |
| --- | --- |
| `workflow/WorkflowModernPresentationBootstrap.vb` | Evalúa el gate del ASMX para emitir un bootstrap visual por solicitud. |
| `workflow/Webworkflow.aspx` y `.vb` | Conservan el enlace legacy; el code-behind registra CSS, JavaScript y bootstrap solo con el gate activo, sin bloques de código en la cabecera Web Forms. |
| `js/workflow/workflow-transition-ui.js` | Carga preview, representa destinos y publica la selección sin enviar. |
| `Styles/workflow-transition-modern.css` | Estilos aislados, responsive y accesibles del modal. |
| `tests/workflow-transition-ui.test.cjs` | Pruebas unitarias de contrato, gate inactivo, selección y regresión de la cabecera Web Forms. |

## Paquete

1. [Arquitectura](01-arquitectura.md)
2. [Contrato de datos y callback](02-contrato.md)
3. [Flujo y seguridad](03-flujo-y-seguridad.md)
4. [Pruebas y evidencia](04-pruebas-y-evidencia.md)
5. [Diagrama de activación y modal](Diagramas/activacion-y-modal.mmd)

## Rollback

Desactivar `WorkflowCentroTrabajoModernActive` para el piloto. En la siguiente carga no se emite el bootstrap ni se cargan los assets modernos; el enlace, `GridView_envia_flujo` y el modal Web Forms existentes siguen siendo el recorrido activo.
