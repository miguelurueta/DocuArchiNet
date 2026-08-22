# Matriz de ambientes y autorización

- Ticket: DOC-31
- Cambio OpenSpec: doc-31-liberacion-controlada-enviar-usuario
- Clasificación: cross_cutting

## Estado actual

La matriz contiene **cero ambientes elegibles**. DOC-31 no recibió una solicitud operativa que nombre ambiente, ventana, aprobador o responsables nominales. La decisión vigente es solicitar aprobación operativa; no existe un despliegue implícito.

| Ambiente | Autorización | Versión | Alcance | Ventana | Responsables por rol | Evidencia | Continuación |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Ninguno incluido en DOC-31 | No otorgada | `main@43d42045beea0984c1b63193e66d12f6a49e5e1c` solo como referencia | Ninguna operación | No otorgada | Sin asignación nominal | DOC-30 y PR #23 | Solicitar aprobación por ambiente |

## Registro requerido para habilitar un ambiente

Una solicitud posterior debe registrar, sin secretos:

| Campo | Regla |
| --- | --- |
| Ambiente | Nombre exacto y ámbito de la operación. |
| Autorización | Aprobación explícita para ese ambiente y esa versión. |
| Versión | SHA o artefacto aprobado, verificable contra `main`. |
| Alcance | Enviar a usuario; exclusiones explícitas de Grupo y Continuar flujo. |
| Ventana | Inicio, final y criterio de abortar aprobados. |
| Responsables | Roles de aprobador, operador de despliegue y dueño funcional identificados en la solicitud. |
| Evidencia | Referencia sanitizada a DOC-30, PR, validaciones y controles de solo lectura. |
| Continuación | Continuar, abortar o revertir mediante el procedimiento aprobado. |

Una autorización de pruebas, de otro ambiente o de otro cambio no llena ninguno de estos campos.
