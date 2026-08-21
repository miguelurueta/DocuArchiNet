# Prompts Jira: Devolver a actividad anterior

Jira controla el orden, las aprobaciones y el cierre de esta modernización. Cada ticket enlaza uno y solo uno de los cuatro prompts de ejecución; el agente ejecuta exclusivamente la etapa indicada y deja la evidencia necesaria para desbloquear la siguiente. No se usa una planificación paralela.

| Etapa Jira | Archivo | Requiere | Produce y desbloquea |
| --- | --- | --- | --- |
| Contexto común | `00-contexto-obligatorio.md` | Solo repositorio; no se adjunta a Jira | Límites comunes; no ejecuta trabajo por sí mismo. |
| 01 | `01-backend-devolucion-actividad-segura.md` | Decisiones de Exploración aprobadas | Contrato completo, preview, ejecución, lock y auditoría para 02. |
| 02 | `02-ui-moderna-oficial.md` | 01 aprobado | Interfaz moderna oficial, sin ruta legacy, para 03. |
| 03 | `03-verificacion-transversal.md` | 02 aprobado | Evidencia técnica y recomendación para 04. |
| 04 | `04-liberacion-controlada.md` | 03 aprobado | Matriz de ambientes y runbook; no despliega. |

`00-contexto-obligatorio.md` y los documentos de `../Exploracion/` son instrucciones internas versionadas en el repositorio; no son tickets ni adjuntos Jira. El ticket backend solo inicia cuando las decisiones marcadas como precondición estén aprobadas en Exploración.

## Migración de tickets existentes

Actualizar los enlaces de Jira antes de usar esta versión. No se debe ejecutar un ticket que apunte a un archivo retirado.

| Etapa anterior | Destino | Tratamiento |
| --- | --- | --- |
| 01 — alcance y diseño | `../Exploracion/` | Retirar como ticket; conservar sus decisiones y aprobaciones en el repositorio. |
| 02 — preview y 03 — ejecución | 01 — backend seguro | Consolidar en un único ticket; conserva criterios y pruebas de ambas etapas. |
| 04 — UI moderna | 02 — UI moderna oficial | Renumerar y actualizar el enlace. |
| 05 — verificación | 03 — verificación transversal | Renumerar y actualizar el enlace. |
| 06 — liberación | 04 — liberación controlada | Renumerar y actualizar el enlace. |

La operación se limita a **Devolver → Elegir actividad anterior** de `workflow/Webworkflow.aspx`. Aplica a una actividad predecesora válida de la Ruta o Flujo actual, mediante un conector entrante revalidado en servidor. **Usuario anterior**, Continuar flujo, Enviar a usuario y Enviar a grupo quedan fuera de alcance. Ningún prompt ejecuta E2E autenticada sin autorización explícita.
