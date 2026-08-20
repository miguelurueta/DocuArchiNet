# 01 — Alcance y diseño para ticket Jira

## ROL ESPERADO

Actúa como arquitecto senior de .NET Framework, VB.NET, ASP.NET Web Forms y Workflow legacy.

## OBJETIVO

Desde el ticket Jira inicial de **Enviar a usuario**, consolidar la decisión técnica que habilita la etapa 02. Esta etapa no implementa código ni crea una planificación paralela.

## CONTEXTO OBLIGATORIO

- Verificar que el ticket actual enlaza este archivo y que no tiene predecesores pendientes.
- Leer `00-contexto-obligatorio.md`, `../00-exploracion-arquitectura-envio-usuario.md` y la arquitectura existente de `../Terminar/`.
- Jira es la fuente de estado: registrar si la salida habilita o bloquea el ticket 02.

## REQUISITOS POSITIVOS

- Precisar contrato objetivo de `PreviewEnviarUsuario` y `EjecutarEnvioUsuario`, destino usuario–actividad, `CAMBIO_USUARIO`, búsqueda paginada, lock, auditoría y fallback Web Forms.
- Documentar respuesta pendiente como bloqueo sin reasignación, aislamiento de Continuar flujo y matriz mínima de aceptación/pruebas.
- Registrar dependencias, exclusiones, rutas de código que podrán tocarse en 02 y riesgos que requieran decisión funcional.

## RESTRICCIONES CRÍTICAS

- No modificar código, configuración, gate, endpoints, pruebas ni tickets Jira fuera de la evidencia autorizada.
- No crear OpenSpec, tareas técnicas paralelas ni un segundo backlog.
- No asumir decisiones abiertas ni autorizar 02 si falta un criterio verificable.

## REGLAS DE ANTIRREGRESIÓN

- Preservar `PreviewEnviarTarea`, `EjecutarEnvioTarea`, `ServicioTransicionTarea`, `IdConector` y el fallback legacy como contratos fuera de alcance.
- No diseñar conectores ficticios ni rutas de reasignación de respuesta.

## CRITERIOS DE ACEPTACIÓN

- La documentación identifica una sola operación directa a usuario y sus exclusiones verificables.
- El ticket deja una decisión inequívoca: habilita 02 o queda bloqueado con causa y responsable.

## PRUEBAS OBLIGATORIAS

No ejecutar pruebas ni compilación por defecto porque no hay código. Dejar para los tickets sucesores la matriz de evidencia: comando MSBuild aplicable según el proyecto, prueba focal o QA manual reproducible y resultado esperado. Si el ticket solicita línea base, ejecutar solo verificaciones locales no mutantes y registrar comando, resultado y limitaciones.

No aplica E2E en esta etapa: no existe cambio ejecutable ni ambiente que validar. Registrar esta justificación formal en el handoff y exigir en 05 evidencia manual reproducible, o E2E autorizado, después de implementar.

## DOCUMENTACIÓN TÉCNICA

Actualizar solo `00-indice.md`, `01-arquitectura.md` y `02-contrato.md` bajo `../01-implementacion-envio-usuario/`, incluyendo ticket actual, ticket sucesor y riesgos.

## ENTREGABLE FINAL

Reportar ticket actual, decisiones cerradas, bloqueos, archivos documentales actualizados y criterio exacto para iniciar 02.
