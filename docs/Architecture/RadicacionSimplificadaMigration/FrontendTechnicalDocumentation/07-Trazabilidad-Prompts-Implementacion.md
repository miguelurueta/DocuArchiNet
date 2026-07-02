# Trazabilidad De Prompts E Implementacion

## Mapa General

| Documento tecnico | Prompts relacionados | Resultado esperado |
|---|---|---|
| `01-Requerimientos-Frontend-Radicacion-Simplificada.md` | Todos los prompts FE, TD-FE y BE-API | Reglas funcionales consolidadas. |
| `02-Modelo-Tecnico-Frontend.md` | `PROMPT-TD-FE-01`, `PROMPT-TD-FE-02`, `PROMPT-FE-05`, `PROMPT-FE-06`, `PROMPT-FE-07` | Arquitectura objetivo del modulo. |
| `03-Diagrama-Clases-Frontend.md` | `PROMPT-TD-FE-02`, `PROMPT-TD-FE-03`, `PROMPT-FE-05` | Componentes, hooks, services y contexto alineados. |
| `04-Diagramas-Estado-Frontend.md` | `PROMPT-FE-06`, `PROMPT-FE-07`, `PROMPT-TD-FE-05` | Estados UI y documental sin ambiguedad. |
| `05-Diagramas-Secuencia-Frontend.md` | `PROMPT-BE-API-02`, `PROMPT-BE-API-04`, `PROMPT-BE-API-05`, prompts FE consumidores | Flujo FE/BE de inicio, toma y envio a pendiente. |
| `06-Casos-Uso-Frontend.md` | Prompts funcionales FE | Casos de uso validables por QA y desarrollo. |

## Orden Recomendado

| Orden | Frente | Prompt | Implementa | Criterio de aceptacion |
|---:|---|---|---|---|
| 1 | TD Front | `PROMPT-TD-FE-01-Unificar-Fuente-Plantilla-Radicacion.md` | Una sola fuente de plantilla. | No hay doble request ni doble `useCamposPlantilla`. |
| 2 | TD Front | `PROMPT-TD-FE-02-Contexto-Documental-Unico-Guards.md` | Provider y guard documental. | `Documentos` no renderiza sin `estado = 0`. |
| 3 | Backend | `PROMPT-BE-API-02-Estado-Activo-Radicacion.md` | Consulta de activo al iniciar. | Retorna contexto si hay activo y false si no hay. |
| 4 | Front | `PROMPT-FE-06-Inicio-Modulo-Estado-Activo-Contexto-Documental.md` | Inicio inteligente del modulo. | Con activo navega a documentos; sin activo abre formulario. |
| 5 | Backend | `PROMPT-BE-API-01-Listado-Radicados-Pendientes.md` | Lista server para pendientes. | Retorna datos compatibles con `AppTable`. |
| 6 | Backend | `PROMPT-BE-API-03-Contador-Pendientes-Radicacion.md` | Contador liviano de pendientes. | Cuenta solo `estado = 1`. |
| 7 | Backend | `PROMPT-BE-API-05-Tomar-Radicado-Pendiente.md` | Toma de pendiente `1 -> 0`. | Bloquea si el usuario ya tiene activo. |
| 8 | Front | `PROMPT-FE-05-Modal-Pendientes-AppTable-Asignacion-Radicado.md` | Modal con `AppTable` y accion `asignacion-tarea`. | Tomar pendiente actualiza contexto y navega a documentos. |
| 9 | Backend | `PROMPT-BE-API-04-Enviar-Radicado-Pendiente.md` | Envio de activo a pendiente `0 -> 1`. | No borra documentos ni gabinete. |
| 10 | Front | `PROMPT-FE-07-Enviar-Tramite-Activo-A-Pendiente.md` | Accion UI de envio a pendiente. | Limpia contexto y desactiva documentos. |
| 11 | TD Front | `PROMPT-TD-FE-05-Limpiar-Formulario-Radicacion-Entrante.md` | Limpieza quirurgica de formulario. | Limpia captura sin borrar contexto documental. |
| 12 | TD Front | `PROMPT-TD-FE-04-Rutas-Tabs-Limpieza-UI-Prototipo.md` | Rutas/tabs semanticas y limpieza de mocks. | Tabs semanticas y prototipos no se confunden con datos reales. |
| 13 | TD Front | `PROMPT-TD-FE-03-Refactor-RadicacionForm-Secciones-Hooks.md` | Refactor incremental del formulario. | Secciones y hooks extraidos con pruebas. |

## Dependencias Criticas

| Dependencia | Motivo |
|---|---|
| `TD-FE-02` antes de `FE-06`, `FE-05` y `FE-07` | Todos escriben o leen contexto documental. |
| `BE-API-02` antes de `FE-06` | El inicio inteligente depende del estado activo. |
| `BE-API-05` antes de `FE-05` | La accion de tabla necesita mutacion real. |
| `BE-API-04` antes de `FE-07` | Enviar a pendiente necesita persistencia real. |
| `TD-FE-05` despues del contexto | La limpieza debe probar que no borra contexto documental. |

## Cierre De Compatibilidad

Los prompts son compatibles si se respeta esta regla:

```txt
El contexto documental es la unica fuente frontend para decidir si Documentos esta activo.
```

Por lo tanto, ningun componente debe activar `CapDocument` por:

- solo tener consecutivo de radicado;
- seleccionar una fila en pendientes;
- abrir un modal;
- consultar un radicado;
- tener `id_tarea_workflow` sin `estado = 0`.

