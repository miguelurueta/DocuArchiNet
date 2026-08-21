# Prompts Jira: Devolver a usuario anterior

Jira controla el orden, las aprobaciones y el cierre de esta modernización. Cada ticket enlaza uno y solo uno de los cinco prompts de etapa; el agente ejecuta exclusivamente la etapa indicada y deja la evidencia necesaria para desbloquear la siguiente. No se usa OpenSpec como una segunda fuente de planificación.

| Etapa Jira | Archivo | Requiere | Produce y desbloquea |
| --- | --- | --- | --- |
| Contexto común | `00-contexto-obligatorio.md` | Solo repositorio; no se adjunta a Jira | Límites comunes; no ejecuta trabajo por sí mismo. |
| 01 | `01-alcance-y-diseno.md` | Ticket de inicio aprobado | Decisión técnica y contrato objetivo para 02. |
| 02 | `02-backend-devolucion-segura.md` | 01 aprobado | Contrato completo, preview, ejecución, lock y auditoría para 03. |
| 03 | `03-ui-moderna-oficial.md` | 02 aprobado | Interfaz moderna oficial, sin ruta legacy, para 04. |
| 04 | `04-verificacion-transversal.md` | 03 aprobado | Evidencia técnica y recomendación para 05. |
| 05 | `05-liberacion-controlada.md` | 04 aprobado | Matriz de ambientes y runbook; no despliega. |

`00-contexto-obligatorio.md` es una instrucción común interna, no una etapa ni un ticket. Permanece versionado en este repositorio y no se adjunta ni se duplica en Jira. Los tickets Jira enlazan exclusivamente el prompt numerado de su etapa; el agente lee el contexto común desde esta carpeta antes de ejecutarla. La etapa 01 debe cerrar sus decisiones obligatorias antes del backend.

## Migración de tickets existentes

Actualizar los enlaces de Jira antes de usar esta versión. No se debe ejecutar un ticket que apunte a un archivo retirado.

| Etapa anterior | Destino | Tratamiento |
| --- | --- | --- |
| 01 — alcance y diseño | 01 — alcance y diseño | Se conserva y añade decisiones de compatibilidad de ejecución. |
| 02 — preview y 03 — ejecución | 02 — backend seguro | Consolidar en un único ticket; conserva criterios y pruebas de ambas etapas. |
| 04 — UI moderna | 03 — UI moderna oficial | Renumerar y actualizar el enlace. |
| 05 — verificación | 04 — verificación transversal | Renumerar y actualizar el enlace. |
| 06 — liberación | 05 — liberación controlada | Renumerar y actualizar el enlace. |

La operación se limita a **Devolver → Usuario anterior** de `workflow/Webworkflow.aspx`. Su destino se deriva exclusivamente del historial validado de la misma tarea. Actividad anterior, grupos, Continuar flujo, Enviar a usuario y Enviar a grupo quedan fuera de alcance. Ningún prompt ejecuta E2E autenticada sin autorización explícita.
