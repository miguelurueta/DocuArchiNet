# Flujo, seguridad y piloto

## Orden obligatorio

1. El ASMX obtiene un contexto de sesión Gestión→Workflow y valida permisos.
2. `IWorkflowModernFeatureGate` decide si usuario/grupo está en piloto.
3. Se valida forma de tarea, conector y token.
4. El guard adquiere el lock MySQL por tarea y versión.
5. Dentro del lock se relee tarea activa y versión, y se autoriza el destino RUTA o FLUJO.
6. Se verifican respuesta y aprobación; firma, expediente, copia y balanceo permanecen en la primera fase del motor legacy.
7. El adaptador llama una sola vez a `Terminar_Tarea_Workflow` con `Page = Nothing` y actualización de controles desactivada.
8. El propio motor conserva `PRETERMINARACTIVIAD`, `Cambia_Estado`, `TERMINARACTIVIDAD`, correo y trazabilidad base.
9. Se normaliza el resultado, se registra auditoría adicional y se libera el lock siempre.

Ver [secuencia](Diagramas/02-secuencia-ejecucion.mmd), [concurrencia](Diagramas/03-concurrencia.mmd) y [estados](Diagramas/04-estados.mmd).

## Límites de seguridad

- Session solo aparece en el gate y adaptadores de infraestructura; no llega al servicio ni a repositorios de destino.
- El conector cliente nunca determina por sí solo usuario, grupo, ruta ni actividad destino.
- El guard no modifica tablas Workflow ni inicia una transacción de negocio.
- El audit adicional falla como advertencia segura; no expone la excepción ni revierte una transición ya confirmada por el motor.
- `PRETERMINARACTIVIAD` puede bloquear antes del cambio; `TERMINARACTIVIDAD` continúa como efecto posterior legacy.

## Riesgos conocidos

| Riesgo | Tratamiento |
| --- | --- |
| Reglas legacy adicionales (firma, expediente, copia, balanceo) | No se duplican: el motor las mantiene y normaliza su rechazo. |
| Conector cambia tras el preview | La relectura dentro del lock obliga a resolverlo de nuevo. |
| Dos nodos IIS | `GET_LOCK` se comparte en el servidor MySQL del módulo. |
| Error durante liberación | `Dispose` intenta `RELEASE_LOCK` y cierra la conexión; MySQL libera el lock al cerrar. |

## Piloto, métricas y rollback

Configuración inicial obligatoria:

```text
WorkflowCentroTrabajoModernActive=false
WorkflowCentroTrabajoModernUsers=
WorkflowCentroTrabajoModernGroups=
```

Para piloto se habilita temporalmente solo el usuario o grupo autorizado. Antes de habilitarlo deben existir build verde, prueba de validación, QA de RUTA/FLUJO/bloqueo y una tarea descartable para la prueba mutante. Registrar: tarea de prueba, tipo de transición, código público, resultado, advertencias, duración, evidencia de estado/auditoría y resultados de concurrencia.

Rollback: restablecer los tres valores anteriores y reciclar la configuración según el procedimiento operativo. No hay migración ni reversión de estados: las nuevas llamadas modernas quedan bloqueadas y `Webworkflow.aspx` sigue usando el camino existente.
