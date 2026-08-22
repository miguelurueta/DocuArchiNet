# Runbook de operación controlada

- Ticket: DOC-31
- Cambio OpenSpec: doc-31-liberacion-controlada-enviar-usuario
- Clasificación: cross_cutting

## Propósito y roles

Este procedimiento solo se usa después de que una solicitud apruebe un ambiente concreto. Los roles que la solicitud debe identificar son: aprobador de liberación, operador de despliegue y dueño funcional de Workflow. DOC-31 no asigna personas ni inicia ninguna operación.

## Antes de operar

1. Confirmar que la solicitud contiene ambiente, versión, alcance, ventana, criterio de abortar y los tres roles.
2. Comparar la versión solicitada con el artefacto aprobado en `main` y la evidencia DOC-30.
3. Confirmar que la operación conserva la ruta moderna de Enviar a usuario y que no incluye Grupo ni Continuar flujo.
4. Confirmar que el gate de Centro de trabajo permanece en `false` con listas vacías; esta comprobación no autoriza cambiarlo.
5. Acordar el paquete previamente aprobado al que se regresará si se ordena reversión.

## Controles permitidos

Solo después de autorización explícita para el ambiente, el operador puede usar evidencia documental y consultas `SELECT` sanitizadas para comprobar versión, estado esperado y auditoría. No debe guardar ni imprimir credenciales, cookies, cadenas de conexión, cuerpos de respuesta, destinos ni datos personales. No se ejecutan E2E, carga, cambios de gate, ajustes de configuración ni mutaciones de Workflow como parte de este runbook.

## Continuar o abortar

Se puede continuar únicamente si versión, alcance, controles de solo lectura y ventana coinciden con la solicitud aprobada. Se debe abortar antes de desplegar si falta alguno de esos elementos, si el gate no está en el estado esperado, si aparece una diferencia de contrato o si el aprobador retira la autorización. El abortar no modifica la aplicación ni el ambiente.

## Reversión por paquete

Cuando la gestión de despliegue aprobada ordene reversión, el operador restaura el paquete previamente acordado siguiendo el proceso oficial del ambiente. La reversión afecta solo intentos nuevos: no revierte tareas, respuestas, auditoría ni transiciones de Workflow ya confirmadas. No se habilita una ruta Web Forms alternativa, no se reasigna respuesta y no se cambia el contrato `IdConector` de Continuar flujo.

## Registro de resultado

El resultado debe registrar la decisión continuar, abortar o revertir, la versión, el ambiente y referencias sanitizadas a evidencia. La aprobación de un ambiente no se reutiliza para otro ambiente ni para una nueva ventana.
