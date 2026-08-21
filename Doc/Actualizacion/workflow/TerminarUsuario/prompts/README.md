# Prompts Jira: Enviar a usuario

Jira controla el orden, las aprobaciones y el cierre. Cada ticket fijo enlaza uno y solo uno de los cuatro prompts de etapa; el agente ejecuta exclusivamente el archivo indicado por el ticket actual y deja la evidencia necesaria para desbloquear el siguiente. No se usa OpenSpec como segunda fuente de planificación.

`00-contexto-obligatorio.md` es una instrucción común, no una etapa ni un ticket: se adjunta o referencia en cada ticket. La decisión arquitectónica vigente está en `../00-exploracion-arquitectura-envio-usuario.md`; antes de iniciar la etapa 01 debe estar aprobada y sin decisiones abiertas.

| Etapa Jira | Archivo | Requiere | Produce y desbloquea |
| --- | --- | --- | --- |
| Contexto común | `00-contexto-obligatorio.md` | Siempre adjunto o referenciado | Límites comunes; no ejecuta trabajo por sí mismo. |
| 01 | `01-backend-envio-usuario.md` | Diseño vigente aprobado | Contrato completo, preview de solo lectura y ejecución directa segura para 02. |
| 02 | `02-ui-moderna-oficial.md` | 01 aprobado | Interfaz moderna oficial y evidencia focal para 03. |
| 03 | `03-verificacion-transversal.md` | 02 aprobado | Evidencia técnica independiente y recomendación de liberación para 04. |
| 04 | `04-liberacion-controlada.md` | 03 aprobado | Matriz de ambientes y runbook; no activa. |

## Migración de tickets existentes

Antes de usar esta versión, actualizar los enlaces de los tickets Jira ya creados. No se debe ejecutar un ticket que todavía apunte a un archivo retirado.

| Etapa anterior | Destino | Tratamiento |
| --- | --- | --- |
| 01 — alcance y diseño | Decisión arquitectónica vigente | Retirar como ticket de implementación; registrar la aprobación o bloqueo en la exploración. |
| 02 — preview y 03 — ejecución | 01 — backend seguro | Consolidar en un único ticket; conserva todos los criterios y pruebas de ambas etapas. |
| 04 — UI moderna | 02 — UI moderna oficial | Renumerar y actualizar el enlace. |
| 05 — verificación | 03 — verificación transversal | Renumerar y actualizar el enlace. |
| 06 — liberación | 04 — liberación controlada | Renumerar y actualizar el enlace. |

La documentación se consolida en `Doc/Actualizacion/workflow/TerminarUsuario/01-implementacion-envio-usuario/`. La operación moderna se limita a **Enviar a usuario** de `workflow/Webworkflow.aspx`; ante respuesta pendiente bloquea y nunca reasigna. La ruta moderna aplica a todo usuario con contexto Workflow válido y no evalúa un feature gate ni una configuración de habilitación. Ningún prompt ejecuta E2E autenticado sin autorización explícita.
