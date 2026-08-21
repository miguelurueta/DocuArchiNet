# Prompt base obligatorio — Modernización de devolver a usuario anterior

Adjuntar este prompt al inicio de cada etapa de esta carpeta.

## USO DE ESTE ARCHIVO

Este archivo establece contexto, límites y criterios comunes. Por sí solo no autoriza implementar código, ejecutar pruebas, crear trabajo OpenSpec, generar paquetes documentales ni avanzar a una etapa posterior.

- Ejecutar una etapa únicamente cuando la solicitud incluya de forma expresa uno de los prompts `01` a `06`.
- No inferir una etapa a partir del objetivo global ni recorrer los prompts automáticamente.
- Una instrucción del usuario que detenga implementación o pruebas prevalece sobre la etapa indicada.

## CONTROL POR JIRA

Jira es la única fuente de estado, dependencias, aprobaciones y cierre.

- Cada ticket debe enlazar el prompt de su etapa y tener sus predecesores aprobados o cerrados.
- No crear tareas, propuestas ni artefactos OpenSpec paralelos.
- El resultado debe informar ticket, evidencia, archivos modificados, verificaciones y bloqueos.
- Si el ticket no identifica etapa, predecesores o autorización, detenerse y solicitar corrección.

## OBJETIVO

Modernizar únicamente **Devolver → Usuario anterior** del Centro de trabajo en `workflow/Webworkflow.aspx`.

La operación devuelve una tarea activa al usuario Workflow histórico inmediatamente anterior de esa misma tarea. El navegador envía solo `IdTarea` y `TokenVersion`; el servidor resuelve el usuario y actividad desde el historial autorizado.

## FUENTES DE DECISIÓN

- `Doc/Actualizacion/workflow/DevolverUsuarioAnterior/Exploracion/01-exploracion-modernizacion-devolver-usuario-anterior.md`.
- `Doc/Actualizacion/workflow/DevolverUsuarioAnterior/Exploracion/02-manual-requerimientos-devolver-usuario-anterior.md`.

## ALCANCE

- Reutilizar contexto autenticado, token de versión, `GET_LOCK`, auditoría, confirmación y actualización moderna de presentación.
- Implementar preview de historial, confirmación y ejecución moderna de devolución a usuario anterior.
- La experiencia moderna es la única ruta de esta operación para todo usuario con contexto Workflow válido; no depende de pilotos, listas de usuarios/grupos ni configuración de habilitación.

## RESTRICCIONES CRÍTICAS

- El destino debe ser el usuario histórico inmediato anterior de la misma tarea, resuelto y revalidado solo en servidor.
- No aceptar usuario, actividad, grupo, conector, Ruta o Flujo desde el navegador.
- Si no existe un usuario histórico válido, o el registro corresponde a grupo, bloquear; no abrir ni invocar Devolver a actividad anterior.
- No modificar contratos o comportamiento de Devolver a actividad anterior, Continuar flujo, Enviar a usuario ni Enviar a grupo.
- El preview ejecuta exclusivamente `SELECT` parametrizados y no modifica tarea, estado, auditoría, eventos ni datos de negocio.
- No crear banderas de habilitación, rutas UI alternativas, postbacks, `GridView`, `UpdatePanel`, `ModalPopupExtender` ni campos ocultos para autorizar o ejecutar.
- La operación no consulta, valida, bloquea, crea, actualiza, reasigna ni audita respuestas, radicados o confirmaciones. No referenciar `Classgestionrespuesta`, `Verifica_respuesta_*` ni `Reasigna_respuesta_envia_tarea_usuario`.
- El ASMX no manipula `Page`, controles Web Forms ni handlers. La mutación final ocurre mediante un adaptador específico a `Terminar_Tarea_Workflow`.
- No ejecutar E2E autenticada, carga ni una tarea real sin autorización explícita de ambiente y cuentas de prueba.
- No imprimir ni guardar credenciales, cookies ni cadenas de conexión.

## CRITERIOS DE ACEPTACIÓN

- Solo se muestra o ejecuta la operación cuando existe usuario histórico anterior válido.
- Usuario histórico ausente, de grupo, retirado o igual al usuario autenticado produce bloqueo sin cambiar la tarea.
- La auto-devolución compara contra el usuario Workflow autenticado, nunca contra `Id_Ruta_Workflow`.
- Un token vencido, lock ocupado o solicitud concurrente no provoca una segunda transición.
- Éxito, bloqueo y error devuelven mensajes funcionales sin detalles técnicos.
- La bandeja conserva su layout y restablece su desplazamiento horizontal después del éxito.
- Ninguna capa de la capacidad contiene tratamiento de respuestas.

## PRUEBAS Y COMPILACIÓN

- Las etapas de implementación ejecutan pruebas focales y la compilación disponible.
- La verificación transversal, QA manual y consolidación de evidencia pertenecen a la etapa 05.
- Registrar comando, resultado, cobertura y limitaciones reproducibles.

## DOCUMENTACIÓN TÉCNICA

- Actualizar las fuentes de decisión cuando cambie un contrato o requisito.
- La implementación documenta `00-indice.md`, `01-arquitectura.md`, `02-contrato.md`, `03-flujo-y-seguridad.md` y `04-pruebas-y-evidencia.md` bajo `Doc/Actualizacion/workflow/DevolverUsuarioAnterior/01-implementacion-devolver-usuario-anterior/`.
- Registrar archivos modificados, supuestos y riesgos residuales.

## ENTREGABLE FINAL

Entregar cambios, archivos, pruebas y compilación, documentación actualizada y riesgos o decisiones pendientes. No avanzar de etapa si falta un criterio verificable o una aprobación funcional.
