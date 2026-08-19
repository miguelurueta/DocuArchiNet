# Prompts de implementación: Enviar a grupo

Ejecutar los prompts en este orden. Cada archivo es autosuficiente y debe respetar las reglas comunes de `00-contexto-obligatorio.md`, la arquitectura ya implementada en `../Terminar/` y el único paquete documental `../01-implementacion-envio-grupo/`; cada etapa actualiza las secciones que le correspondan, sin crear paquetes por etapa.

| Orden | Archivo | Propósito |
| --- | --- | --- |
| 0 | `00-contexto-obligatorio.md` | Restricciones aplicables a todo el trabajo. |
| 1 | `01-propuesta-openspec.md` | Formalizar contratos, secuencia y decisiones antes de editar código. |
| 2 | `02-contratos-autorizacion.md` | Contratos y autorización `Cambio_Ruta` en el contexto existente. |
| 3 | `03-preview-destinos.md` | Preview de solo lectura en el ASMX moderno existente. |
| 4 | `04-servicio-ejecucion.md` | Lock, revalidación, endpoint de ejecución y auditoría. |
| 5 | `05-adaptador-legacy.md` | Uso aislado del motor legacy de reenvío directo. |
| 6 | `06-asmx-ui.md` | Integración visual con el ASMX y componentes existentes. |
| 7 | `07-gate-auditoria.md` | Operación con gate único, trazabilidad y rollback. |
| 8 | `08-pruebas-verificacion.md` | Pruebas y verificación final. |
| 9 | `09-liberacion-activacion-controlada.md` | Decisión y preparación de activación autorizada por ambiente. |

No activar gates ni ejecutar E2E autenticado sin autorización explícita. Las pruebas aprobadas no autorizan una activación automática: una activación requiere aprobación expresa por ambiente. Las etapas no crean una segunda bandera ni una segunda fuente de configuración.
