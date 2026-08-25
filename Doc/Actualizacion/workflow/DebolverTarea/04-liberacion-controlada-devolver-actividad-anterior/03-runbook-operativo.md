# Runbook de operación controlada

- Ticket: DOC-35
- Cambio OpenSpec: doc-35-liberacion-devolver-tarea-actividad
- Clasificación: cross_cutting

## Propósito y roles

Este procedimiento se usa solo después de una solicitud que autorice un ambiente concreto. La solicitud identifica aprobador de liberación, operador de despliegue y dueño funcional de Workflow. DOC-35 no asigna personas ni inicia operaciones.

## Antes de operar

1. Confirmar ambiente, versión, alcance, ventana, criterio de aborto y los tres roles.
2. Comparar el paquete solicitado con la referencia main del PR #29 y la evidencia DOC-34.
3. Confirmar que la operación mantiene la ruta moderna oficial de Devolver a actividad anterior y sus conectores entrantes de Ruta o Flujo.
4. Confirmar que no se propone reactivar postback, UpdatePanel, GridView, ModalPopupExtender, UI alternativa ni un cambio de gate.
5. Acordar el paquete previamente aprobado al que se regresará si se ordena reversión.

## Controles permitidos

Después de autorización explícita para el ambiente, el operador puede usar evidencia documental y consultas SELECT parametrizadas y saneadas para comprobar versión, estado esperado y auditoría. No debe guardar ni imprimir credenciales, cookies, cadenas de conexión, cuerpos de respuesta, destinos, identificadores de tarea ni datos personales.

Este runbook no ejecuta E2E, carga, mutaciones de Workflow, cambios de gate ni ajustes de configuración.

## Continuar o abortar

Se puede continuar únicamente si versión, alcance, controles de solo lectura y ventana coinciden con la solicitud aprobada. Se debe abortar antes de desplegar si falta alguno de esos elementos, si aparece una diferencia de contrato o versión, si un control no es conforme o si el aprobador retira la autorización. Abortar no modifica aplicación ni ambiente.

## Reversión por paquete

Cuando la gestión de despliegue aprobada ordene reversión, el operador restaura el paquete previamente acordado siguiendo el proceso oficial del ambiente. La reversión solo afecta nuevos intentos; no revierte tareas, auditoría ni transiciones de Workflow ya confirmadas. No se reactiva una ruta Web Forms alternativa ni se incorporan operaciones de respuestas.

## Registro de resultado

El registro debe indicar continuar, abortar o revertir, ambiente, versión y referencias saneadas a evidencia. Una aprobación no se reutiliza para otro ambiente ni para una nueva ventana.
