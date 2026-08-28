# Liberación controlada de Devolver → Usuario anterior

- Ticket: DOC-39
- Cambio OpenSpec: doc-39-liberacion-controlada
- Clasificacion: cross_cutting

## Objetivo

DOC-39 prepara la decisión y los controles documentales para liberar la capacidad ya implementada de **Devolver → Usuario anterior** en Workflow ASP.NET Web Forms. Esta etapa separa la evidencia técnica de la autorización de cada ambiente: una compilación, una prueba o un merge no habilitan por sí mismos un despliegue.

La única decisión vigente es **solicitar aprobación**. Para el ambiente conocido GESTOR aún no existe un registro formal de autorización, ventana ni responsables de la operación. Por ese motivo DOC-39 no despliega, no edita configuración, no habilita gates, no ejecuta E2E/carga ni confirma una transición real.

## Alcance y compatibilidad

La línea base está formada por la implementación de DOC-36, la interfaz moderna y las correcciones de compatibilidad de DOC-37, y la evidencia transversal DOC-38. El commit candidato documentado es `615aac83` (`docs(workflow): cerrar verificacion transversal DOC-38`); identifica la línea revisada, pero no constituye una autorización operativa ni prueba de que esté instalada en GESTOR.

La preparación conserva como ruta oficial `workflow/Webworkflow.aspx` y los módulos modernos de Usuario anterior. No reintroduce postback, controles Web Forms, rutas alternativas ni un fallback hacia Devolver a actividad anterior. Las operaciones Devolver a actividad anterior, Continuar flujo, Enviar a usuario y Enviar a grupo mantienen sus contratos y sus flujos independientes.

El alcance de DOC-39 es exclusivamente documental y de gobierno de liberación. No modifica servicios ASMX, repositorios, scripts, tablas de negocio, auditoría ni datos de tareas. Una reversión futura se gestiona exclusivamente por el proceso de despliegue autorizado y afecta nuevos intentos; nunca corrige ni revierte transiciones ya confirmadas.

## Línea base de evidencia

| Precondición | Referencia | Estado documental |
| --- | --- | --- |
| Backend, token, lock y auditoría | DOC-36, `04-pruebas-y-evidencia.md` | Evidencia local disponible. |
| Ruta moderna y compatibilidad de eventos | DOC-37, commits `cbe5a469` a `16c74ca3` | Implementación identificada. |
| No regresión, compilación y QA visual | DOC-38, correlación `QA-MANUAL-DOC38-20260828` | Aprobado como evidencia técnica. |
| Versión candidata | `615aac83` | Identificada para aprobación por ambiente. |

Los resultados anteriores no sustituyen la aprobación de ambiente, la ventana de cambio ni la designación de responsables. La matriz y el runbook de este paquete mantienen esas condiciones explícitas y sin secretos.
