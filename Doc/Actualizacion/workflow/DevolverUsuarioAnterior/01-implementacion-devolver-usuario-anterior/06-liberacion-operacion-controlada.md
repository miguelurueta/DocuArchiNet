# Liberación y operación controlada — DOC-39

## Decisión vigente

La decisión única de DOC-39 es **solicitar aprobación**. DOC-38 aportó evidencia técnica aprobada, pero no se ha registrado autorización formal de liberación, ventana de cambio ni responsables operativos para GESTOR. En consecuencia, este documento no autoriza un despliegue ni una operación sobre tareas Workflow.

## Precondiciones de la línea base

| Control | Evidencia identificada | Alcance |
| --- | --- | --- |
| Backend seguro | DOC-36: preview de solo lectura, historial, token, lock, adaptador y auditoría. | Línea base técnica. |
| Interfaz moderna | DOC-37: ruta oficial, exclusividad, actualización tras cambio de tarea y compatibilidad de eventos. | Línea base funcional. |
| Verificación transversal | DOC-38: 114 pruebas locales, compilación Debug y `QA-MANUAL-DOC38-20260828`. | Recomendación para preparar la etapa 05. |
| Versión candidata | Commit `615aac83`. | Identificación para una futura aprobación por ambiente. |

La referencia de versión sirve para que el responsable compare el artefacto que se proponga liberar. No demuestra que el commit esté instalado ni concede permiso para modificar GESTOR.

## Matriz de ambiente

| Ambiente | Autorización | Versión candidata | Alcance | Ventana | Responsables | Evidencia | Continuación |
| --- | --- | --- | --- | --- | --- | --- | --- |
| GESTOR | Sin registro formal | `615aac83` | Devolver → Usuario anterior y sus contratos preservados | Sin ventana aprobada | Sin responsable de liberación ni QA designados | DOC-36, DOC-37 y DOC-38 identificados | Solicitar aprobación, ventana y asignación de roles. |

Cada ambiente adicional requiere una fila propia. No se reutilizan aprobaciones, ventanas, cuentas, evidencias de instalación ni permisos entre ambientes. La matriz no admite secretos, cookies, tokens, nombres de cuenta, hosts o cadenas de conexión.

## Runbook para una ventana futura autorizada

Este runbook es una preparación documental. Solo un responsable designado puede iniciarlo tras aprobar explícitamente ambiente, versión, alcance, ventana, cuentas de prueba y plan de reversión.

1. Confirmar que la fila del ambiente tenga autorización, ventana, responsable de despliegue, responsable de QA y la versión de artefacto aprobada.
2. Comparar la versión propuesta con `615aac83` y con la evidencia DOC-38; detener la operación si existe una diferencia de contrato o una evidencia crítica ausente.
3. Antes de operar, acordar con el administrador autorizado los controles `SELECT` de solo lectura y el formato de evidencia saneada. DOC-39 no ejecuta esas consultas.
4. Ejecutar el despliegue solo mediante la gestión de despliegue aprobada para ese ambiente. No reactivar postback, gates, rutas UI alternativas ni cambios de configuración fuera de la aprobación.
5. Comprobar la versión instalada y los contratos de preview/ejecución mediante controles aprobados. Si se autoriza una tarea de prueba, conservar únicamente conteos y resultados saneados; no conservar tokens, usuarios ni identificadores de tarea.
6. Ante una diferencia crítica, detener nuevos intentos y escalarla al responsable de liberación. Una reversión se realiza solo con la gestión de despliegue aprobada y no modifica tareas, estados, historial ni auditoría existentes.

## Controles de solo lectura autorizables

La consulta de historial, si se aprueba para una tarea de prueba autorizada, debe usar únicamente `SELECT` y parámetros. Un patrón de revisión es:

```sql
SELECT id_Estado,
       Inicio_Tareas_Workflow_id_Tarea,
       Id_Actividad,
       Id_Usuario,
       Fecha_Inicio,
       Fecha_Seleccion,
       Fecha_Fin,
       ID_ACTIVIDAD_FLUJO_TRABAJO,
       ID_USUARIO_WORKFLOW_FLUJO_TRABAJO
FROM estados_tarea_workflow
WHERE Inicio_Tareas_Workflow_id_Tarea = @IdTarea
ORDER BY id_Estado DESC;
```

El responsable autorizado valida que la consulta corresponda al ambiente, a una tarea de prueba aprobada y a la base de datos correcta. La evidencia publicada reduce el resultado a fecha, ambiente, versión, control y resultado; excluye filas completas, usuarios, identificadores, tokens, credenciales y detalles de conexión. La verificación de auditoría usa el repositorio de auditoría aprobado por el administrador y el mismo criterio de sanitización.

## Compatibilidad y reversión

Usuario anterior conserva `PreviewDevolverUsuarioAnterior` y `EjecutarDevolverUsuarioAnterior` como ruta moderna oficial. Devolver a actividad anterior, Continuar flujo, Enviar a usuario y Enviar a grupo no se usan como fallback y no cambian sus contratos durante la liberación.

La reversión restaura un artefacto previamente aprobado mediante la gestión de despliegue. Solo afecta solicitudes posteriores a ese cambio; no se usa para revertir transiciones confirmadas, editar `estados_tarea_workflow`, borrar auditoría ni modificar tareas terminadas.

## Riesgos y salida

El riesgo residual actual es la ausencia de aprobación de ambiente, ventana y responsables. Hasta que esos elementos se registren por separado para GESTOR, cualquier despliegue debe permanecer detenido. Cuando estén completos, se revisa de nuevo esta matriz y se emite una única decisión actualizada: bloquear, solicitar aprobación o lista para despliegue autorizado.
