# Matriz de ambientes y autorización

- Ticket: DOC-35
- Cambio OpenSpec: doc-35-liberacion-devolver-tarea-actividad
- Clasificación: cross_cutting

## Estado actual

La matriz tiene **cero ambientes elegibles**. DOC-35 no recibió ambiente, versión operativa, ventana, aprobador ni responsables nominales. La decisión vigente es solicitar aprobación operativa y ningún despliegue está implícito.

| Ambiente | Autorización | Versión | Alcance | Ventana | Responsables por rol | Evidencia | Continuación |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Ninguno incluido en DOC-35 | No otorgada | main en merge PR #29 solo como referencia | Ninguna operación | No otorgada | Sin asignación nominal | DOC-34 y PR #29 | Solicitar aprobación por ambiente |

## Registro requerido para habilitar un ambiente

| Campo | Regla |
| --- | --- |
| Ambiente | Nombre exacto y ámbito de la operación. |
| Autorización | Aprobación explícita para ese ambiente, esa versión y esa ventana. |
| Versión | SHA o paquete aprobado, verificable contra main. |
| Alcance | Devolver a actividad anterior; exclusiones explícitas de Usuario anterior, Continuar flujo, Enviar a usuario y Enviar a grupo. |
| Ventana | Inicio, final y criterio de aborto aprobados. |
| Responsables | Roles de aprobador de liberación, operador de despliegue y dueño funcional de Workflow. |
| Evidencia | Referencias saneadas a DOC-34, PR #29 y controles de solo lectura autorizados. |
| Continuación | Continuar, abortar o revertir mediante el procedimiento aprobado. |

Una autorización de pruebas, de otro ambiente, de otra versión o de otra ventana no completa ningún campo de esta matriz.
