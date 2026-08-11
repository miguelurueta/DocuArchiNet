# Backlog Jira — Centro de Trabajo Workflow

## Épica

**Modernización controlada del Centro de Trabajo Workflow WebForms**

Referencia funcional: `../centro-trabajo-workflow-sin-bandeja.html`.

Referencia de implementación: `../plan-migracion-controlada-centro-trabajo-workflow.md`.

## Secuencia obligatoria

1. `JIRA-01` — Línea base y contrato de regresión.
2. `JIRA-02` — Activación reversible.
3. `JIRA-03` — Adaptador de ciclo de vida WebForms.
4. `JIRA-04` — Layout centro de trabajo.
5. `JIRA-05` — Menús y acciones.
6. `JIRA-06` — Documentos, visor e índice.
7. `JIRA-07` — Responsive y accesibilidad.
8. `JIRA-08` — QA, piloto, despliegue y estabilización.

No se inicia un ticket sin que el anterior tenga evidencia de salida aprobada. Cada ticket debe documentar impacto, pruebas y reversión en `Doc/Tecnica/Opsxj/<cambio>/`.

Cada prompt declara al inicio el rol especializado que debe asumir quien lo implemente; ese rol condiciona las decisiones, validaciones y documentación esperadas.

El contrato visual reutilizable y obligatorio está en `CONTRATO-CSS-COMPONENTES-REUTILIZABLES.md`; replica el CSS del HTML base mediante componentes scoped, no mediante estilos aislados por pantalla.
