# Matriz de migración E2E Workflow

| DOC | Estado | Próxima incorporación | Arneses actuales |
| --- | --- | --- | --- |
| DOC-41 Notas | Piloto implementado | Validar equivalencia en una corrida autorizada antes de retirar el arnés legado | Se conservan `test:notes:*`; la plataforma agrega `notes-anonymous` y `notes-read`. |
| DOC-32 | Pendiente | Registrar adaptador y perfiles para preview, ejecución y concurrencia en un cambio aprobado | `run-doc32-return-activity-interactive.cjs` permanece como fuente operativa. |
| DOC-33 | Pendiente | Registrar adaptador y perfiles para preview, ejecución y bloqueo UI en un cambio aprobado | Se conserva el arnés DOC-33. |
| DOC-36 / DOC-37 | Pendiente | Evaluar su contrato de recursos y adaptar sus etapas en cambios separados | Se conservan `run-workflow-e2e.cjs` y sus perfiles actuales. |

Cada migración debe ser aditiva: registro cerrado, perfil no sensible, pruebas locales y autorización explícita antes de cualquier E2E real. No se migran ni se alteran gates, páginas legacy o recursos de negocio como parte de esta matriz.
