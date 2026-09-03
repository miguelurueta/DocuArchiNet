# 05 — Liberación y operación controlada

## Rol esperado

Responsable técnico de liberación con foco en seguridad, observabilidad y reversibilidad.

## Objetivo

Preparar la decisión de despliegue, matriz de ambientes y runbook operativo de la modernización. Esta etapa no despliega.

## Contexto obligatorio

- Requiere 04 aprobado y ausencia de defectos críticos.
- Leer `00-contexto-obligatorio.md`, evidencia final, versión aprobada, inventario y mecanismos de despliegue/rollback existentes.
- La aprobación técnica no autoriza ningún ambiente.

## Requisitos positivos

- Identificar versión, artefactos, scripts si existen, responsables, ventana, dependencias y evidencia.
- Definir verificaciones previas y posteriores con consultas exclusivamente `SELECT` y resultados saneados.
- Verificar acceso autorizado, listado, detalle, descarga adjunta, consumidor secundario y ausencia de ruta legacy alcanzable.
- Definir monitoreo de códigos funcionales y errores internos sin registrar datos sensibles.
- Definir rollback por artefactos de despliegue para nuevos intentos, sin alterar autorizaciones o tareas existentes y sin reactivar controles inseguros ad hoc.
- Registrar decisión por ambiente: bloquear, solicitar autorización o listo para despliegue autorizado.

## Restricciones críticas

- No desplegar, editar configuración, habilitar gates ni ejecutar pruebas reales o carga desde esta etapa.
- No inferir autorización de un ambiente a partir de otro.
- No modificar datos históricos como rollback.

## Criterios de aceptación

- Matriz, responsables, ventana, verificación, observabilidad y rollback son ejecutables y no contienen secretos.
- La versión liberable coincide exactamente con la verificada en 04.
- La decisión final es explícita por ambiente.

## Reglas de antirregresión

- Preservar operaciones vecinas, selección de tarea, tabla, colores, iconos, índice y scroll.
- No reactivar postbacks, controles ocultos o handlers retirados como mecanismo de contingencia.
- No cambiar contratos ni datos históricos durante rollback; este solo revierte artefactos para nuevos intentos.

## Pruebas obligatorias

No se ejecutan pruebas reales en esta etapa. Verificar documentalmente que el build MSBuild, las pruebas unitarias/focales y los recorridos autorizados de 04 corresponden exactamente a la versión candidata; registrar comandos, resultados, referencias y cualquier ausencia como bloqueo.

## Documentación técnica

Actualizar el paquete documental canónico con matriz de ambientes, runbook, observabilidad, responsables, riesgos y rollback. Si falta una ruta, registrar la ausencia; no crear documentación en la raíz.

## Entregable final

Entregar documentación, diagramas y evidencia coherente con la versión aprobada. Confirmar expresamente que no se desplegó, no se modificó configuración y no se ejecutaron pruebas reales.
