# Prompts de implementación: Enviar a usuario

Ejecutar estos prompts en orden. Cada archivo es autosuficiente y debe respetar las reglas comunes de `00-contexto-obligatorio.md`, la arquitectura entregada en `../Terminar/` y la exploración `../00-exploracion-arquitectura-envio-usuario.md`.

| Orden | Archivo | Propósito |
| --- | --- | --- |
| 0 | `00-contexto-obligatorio.md` | Restricciones aplicables a todas las etapas. |
| 1 | `01-propuesta-openspec.md` | Formalizar el cambio, contratos y decisiones antes de editar código. |
| 2 | `02-contratos-autorizacion.md` | Definir contratos y autorización efectiva `CAMBIO_USUARIO`. |
| 3 | `03-preview-destinos.md` | Crear preview de usuarios destino, exclusivamente de lectura. |
| 4 | `04-servicio-ejecucion.md` | Implementar ejecución, lock, revalidación y auditoría. |
| 5 | `05-adaptador-legacy.md` | Encapsular la llamada directa a `Terminar_Tarea_Workflow`. |
| 6 | `06-asmx-ui.md` | Integrar ASMX e interfaz moderna con fallback Web Forms. |
| 7 | `07-gate-auditoria.md` | Integrar gate único, trazabilidad y rollback operativo. |
| 8 | `08-pruebas-verificacion.md` | Ejecutar pruebas, QA y verificación final. |

La operación moderna se limita al comando **Enviar a usuario** de `workflow/Webworkflow.aspx`. No incluye reasignación de respuesta: ante una respuesta pendiente debe bloquear con mensaje funcional. No activar gates ni ejecutar E2E autenticado sin autorización explícita.

