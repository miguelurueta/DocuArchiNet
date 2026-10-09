# DOC-93 — Pruebas y evidencia

- Ticket: DOC-93
- Cambio OpenSpec: doc-93-segundo-factor-smtp
- Clasificacion: cross_cutting

## Evidencia requerida

La implementación deberá ejecutar pruebas estructurales, runner conductual sin red, regresiones DOC-91/DOC-92, MSBuild, OpenSpec estricto y gobierno OPSXJ. La matriz incluye cardinalidad 0/1/>1, nulos, rangos, banderas, credenciales, SSL, timeout normal/acotado/overflow, cuerpo mínimo, excepción, disposición y sanitización.

En esta fase solo se validan los artefactos de planificación; las evidencias funcionales se registrarán después de implementar las tareas y no se anticipan como aprobadas.

## QA/E2E WebForms

E2E WebForms no aplica: DOC-93 no publica UI ni endpoint. El envío SMTP real tampoco se ejecuta sin autorización explícita y vigente para ambiente, cuenta y buzón descartable. Un test con dobles no acredita conectividad ni entrega real.
