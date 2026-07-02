# Requerimientos Frontend - Radicacion Simplificada

## Objetivo Funcional

El modulo `src/modules/radicacion` debe permitir registrar una radicacion entrante, detectar si el usuario tiene un tramite documental activo, continuar la carga documental cuando corresponda y administrar radicados pendientes por gestion documental sin usar datos mock ni contratos legacy directos.

## Actores

| Actor | Uso |
|---|---|
| Usuario radicador | Registra radicaciones entrantes, toma pendientes y carga documentos. |
| Backend Radicacion | Registra radicados, expone estado activo, pendientes, contador y mutaciones de estado. |
| Backend Tramites | Provee lista de radicados pendientes compatible con `AppTable`. |
| Componente `AppTable` | Renderiza lista paginada de pendientes y dispara acciones de fila. |
| Contexto documental frontend | Conserva el tramite activo para habilitar `Documentos`. |

## Precondiciones

- El usuario esta autenticado y tiene permiso para radicar.
- Existe plantilla de radicacion disponible desde `/api/PlantillaRadicado/listaPlantilla`.
- El backend moderno expone contratos de pendientes y estado activo.
- `AppTable` esta disponible en `src/app/Components/UI/AppTable`.
- El modulo no debe depender de ASMX ni de `Session` legacy.

## Requerimientos Funcionales

| Id | Requerimiento |
|---|---|
| RF-01 | Al iniciar `src/modules/radicacion`, el frontend debe consultar si el usuario tiene un tramite activo en `estado = 0`. |
| RF-02 | Si existe activo `estado = 0`, el modulo debe restaurar contexto documental y entrar directamente al panel `Documentos`. |
| RF-03 | Si no existe activo, el modulo debe iniciar en el formulario de radicacion entrante. |
| RF-04 | `Documentos` debe permanecer inactivo cuando no hay tramite activo `estado = 0`. |
| RF-05 | `Documentos` no debe activarse para consulta de pendientes ni para radicados sin tramite documental activo. |
| RF-06 | Al radicar exitosamente un tramite que requiere gestion documental, el frontend debe guardar `idEstadoRadicado`, `consecutivoRadicado` y metadata de contexto. |
| RF-07 | La lista de pendientes debe renderizarse con `AppTable`, consumiendo contrato `DynamicUiTableDto` o adaptador equivalente. |
| RF-08 | La accion de tabla `asignacion-tarea` debe tomar/re-radicar el pendiente y dejarlo en `estado = 0`. |
| RF-09 | Si el usuario ya tiene activo `estado = 0`, la UI no debe permitir tomar otro pendiente. |
| RF-10 | El boton `Enviar a Pendientes` debe estar disponible solo cuando hay tramite activo `estado = 0`. |
| RF-11 | Enviar a pendiente debe cambiar el tramite a `estado = 1`, limpiar contexto documental y desactivar `Documentos`. |
| RF-12 | La accion `Limpiar` del formulario debe limpiar campos de radicacion entrante sin borrar el contexto documental activo. |
| RF-13 | El formulario debe usar una sola fuente de plantilla; no debe duplicar carga entre `RadicacionRoutePage` y `RadicacionForm`. |
| RF-14 | El modulo debe eliminar mocks visibles como datos reales en `Modalpendiente` y `CapDocument`. |

## Requerimientos No Funcionales

| Id | Requerimiento |
|---|---|
| RNF-01 | Las rutas y tabs deben usar llaves semanticas, no indices fragiles como `"1"`, `"2"`, `"3"`. |
| RNF-02 | Las mutaciones de pendiente deben manejar loading, error y reintentos sin perder contexto local. |
| RNF-03 | La lista de pendientes debe soportar paginacion/orden/filtros server side si el backend lo habilita. |
| RNF-04 | El estado documental debe centralizarse en un provider o store del modulo, no en props dispersas. |
| RNF-05 | Los services deben aislar contratos HTTP y no mezclar DTO backend dentro de componentes. |
| RNF-06 | Los tests deben cubrir guards de `Documentos`, limpieza de formulario y acciones de pendientes. |

## Reglas

| Tema | Regla |
|---|---|
| Estado `0` | Tramite activo para gestion documental; habilita `Documentos`. |
| Estado `1` | Tramite pendiente por gestion documental; aparece en lista de pendientes. |
| Sin activo | Formulario disponible; `Documentos` inactivo. |
| Consulta | No habilita `Documentos`. |
| Toma de pendiente | Debe validar que no exista otro activo `estado = 0`. |
| Envio a pendiente | Solo desde `estado = 0`; no borra documentos ni gabinete. |
| Limpiar formulario | Limpia datos de captura entrante; no borra contexto documental activo. |

## Criterios De Aceptacion

| Criterio | Evidencia Esperada |
|---|---|
| Inicio con activo entra a documentos. | Test de `RadicacionRoutePage` o guard: mock `estado-activo` verdadero y navegacion a documentos. |
| Inicio sin activo no habilita documentos. | Test de guard: `estado-activo` falso deja formulario activo y tab documentos disabled. |
| Lista usa `AppTable`. | `Modalpendiente` no usa tabla AntD mock; integra `AppTable` y accion `asignacion-tarea`. |
| Tomar pendiente activa documentos. | Test de accion: respuesta `estadoActual = 0` actualiza contexto y navega a documentos. |
| Enviar a pendiente desactiva documentos. | Test de accion: respuesta `estadoActual = 1` limpia contexto y bloquea tab. |
| Limpiar no borra contexto. | Test de `RadicacionForm`: reset de campos preserva `RadicacionDocumentalContext`. |
| No hay doble carga de plantilla. | Test o inspeccion: `RadicacionForm` recibe plantilla desde page/contexto. |

