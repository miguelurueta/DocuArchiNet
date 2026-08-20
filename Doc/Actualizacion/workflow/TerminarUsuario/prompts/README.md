# Prompts Jira: Enviar a usuario

Jira controla el orden, las aprobaciones y el cierre. Cada ticket fijo enlaza uno y solo uno de estos prompts; el agente ejecuta exclusivamente el archivo indicado por el ticket actual y deja la evidencia necesaria para desbloquear el siguiente. No se usa OpenSpec como segunda fuente de planificación.

| Etapa Jira | Archivo | Requiere | Produce y desbloquea |
| --- | --- | --- | --- |
| Contexto | `00-contexto-obligatorio.md` | Siempre adjunto | Límites comunes; no ejecuta trabajo por sí mismo. |
| 01 | `01-alcance-y-diseno.md` | Ticket de inicio aprobado | Decisión técnica y contrato objetivo para 02. |
| 02 | `02-preview-paginado-autorizacion.md` | 01 aprobado | Autorización, preview paginado y búsqueda segura para 03. |
| 03 | `03-ejecucion-directa-segura.md` | 02 aprobado | Adaptador, ejecución directa, lock y auditoría para 04. |
| 04 | `04-ui-moderna-fallback.md` | 03 aprobado | Interfaz moderna oficial para 05. |
| 05 | `05-verificacion-transversal.md` | 04 aprobado | Evidencia técnica y recomendación de liberación para 06. |
| 06 | `06-liberacion-controlada.md` | 05 aprobado | Matriz de ambientes y runbook; no activa. |

La documentación se consolida en `Doc/Actualizacion/workflow/TerminarUsuario/01-implementacion-envio-usuario/`. La operación moderna se limita a **Enviar a usuario** de `workflow/Webworkflow.aspx`; ante respuesta pendiente bloquea y nunca reasigna. La ruta moderna aplica a todo usuario con contexto Workflow válido; ningún prompt ejecuta E2E autenticado sin autorización explícita.
