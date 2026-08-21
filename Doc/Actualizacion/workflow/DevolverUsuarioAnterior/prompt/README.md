# Prompts Jira: Devolver a usuario anterior

Jira controla el orden, las aprobaciones y el cierre de esta modernización. Cada ticket enlaza uno y solo uno de estos prompts; el agente ejecuta exclusivamente la etapa indicada y deja la evidencia necesaria para desbloquear la siguiente. No se usa OpenSpec como una segunda fuente de planificación.

| Etapa Jira | Archivo | Requiere | Produce y desbloquea |
| --- | --- | --- | --- |
| Contexto | `00-contexto-obligatorio.md` | Siempre adjunto | Límites comunes; no ejecuta trabajo por sí mismo. |
| 01 | `01-alcance-y-diseno.md` | Ticket de inicio aprobado | Decisión técnica y contrato objetivo para 02. |
| 02 | `02-preview-historial-autorizacion.md` | 01 aprobado | Autorización y preview seguro para 03. |
| 03 | `03-ejecucion-directa-segura.md` | 02 aprobado | Adaptador, ejecución, lock y auditoría para 04. |
| 04 | `04-ui-moderna-oficial.md` | 03 aprobado | Interfaz moderna oficial para 05. |
| 05 | `05-verificacion-transversal.md` | 04 aprobado | Evidencia técnica y recomendación para 06. |
| 06 | `06-liberacion-controlada.md` | 05 aprobado | Matriz de ambientes y runbook; no despliega. |

La operación se limita a **Devolver → Usuario anterior** de `workflow/Webworkflow.aspx`. Su destino se deriva exclusivamente del historial validado de la misma tarea. Actividad anterior, grupos, Continuar flujo, Enviar a usuario y Enviar a grupo quedan fuera de alcance. Ningún prompt ejecuta E2E autenticada sin autorización explícita.
