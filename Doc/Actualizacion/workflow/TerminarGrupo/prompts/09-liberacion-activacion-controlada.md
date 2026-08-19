# 09 — Liberación y activación controlada

## ROL ESPERADO

Actúa como responsable técnico de liberación para Workflow ASP.NET Web Forms, con foco en control de cambios, seguridad operativa y reversibilidad.

## OBJETIVO

Determinar si la modernización de **Enviar a grupo** está lista para solicitar su activación y preparar una liberación controlada mediante la única configuración existente, `WorkflowCentroTrabajoModernActive`.

Que las pruebas de la etapa 08 sean satisfactorias es una **precondición técnica**: no constituye autorización para activar la funcionalidad, ni en uno ni en todos los ambientes.

## RESTRICCIONES CRÍTICAS

- Leer y aplicar `prompts/00-contexto-obligatorio.md`, `prompts/07-gate-auditoria.md` y `prompts/08-pruebas-verificacion.md`.
- No crear una bandera, fuente de configuración, evaluación de autorización, endpoint ni despliegue paralelo para `Enviar a grupo`.
- No modificar configuración de ningún ambiente ni activar el gate como consecuencia automática de este prompt.
- No ejecutar E2E autenticado, pruebas de carga ni consultas que no sean `SELECT` sin autorización explícita para el ambiente y las cuentas de prueba; antes de una prueba autenticada de `PreviewEnviarTarea`, leer `tools/e2e/AGENT-RUNBOOK.md`.
- No imprimir, registrar ni adjuntar credenciales, cookies, cadenas de conexión, tokens ni datos personales innecesarios.
- La habilitación para todos los ambientes exige una autorización explícita e independiente que identifique: ambientes exactos, ventana de cambio, responsables de aprobación y ejecución, versión a liberar, alcance de usuarios/grupos y plan de rollback. La frase «las pruebas pasaron» no satisface esta condición.
- Mientras no exista esa autorización, mantener `WorkflowCentroTrabajoModernActive=false` y las listas de usuarios y grupos vacías al terminar cualquier corrida.
- No alterar contratos, configuración ni comportamiento de `Continuar flujo`; sus endpoints, `IdConector` y adaptador legacy continúan sin cambios.

## PRECONDICIONES DE LIBERACIÓN

Verificar y documentar, sin cambiar estado operativo:

1. El reporte de `08-pruebas-verificacion` está completo, es reproducible y no tiene bloqueos críticos abiertos.
2. La compilación y las pruebas focales aprobadas finalizaron correctamente, o sus limitaciones tienen aceptación explícita y riesgo residual documentado.
3. Están verificadas las evidencias de autorización `Cambio_Ruta`, revalidación de tarea/ruta/flujo/actividad, token vencido, concurrencia, aprobación pendiente, destino inválido, auditoría sanitizada y fallback legacy.
4. Con el gate inactivo, `Enviar a grupo` conserva el postback Web Forms y `Continuar flujo` conserva el contrato con `IdConector`.
5. La versión, artefactos de despliegue, configuración objetivo y mecanismo de rollback se pueden identificar sin exponer secretos.

## DECISIÓN DE ACTIVACIÓN

1. Si falla una precondición o falta evidencia, recomendar **bloquear** la liberación y no modificar configuración.
2. Si las precondiciones se cumplen pero no existe autorización explícita por ambiente, recomendar **solicitar aprobación de activación** y dejar el gate inactivo.
3. Si existe autorización para un subconjunto de ambientes, preparar únicamente el plan para esos ambientes; no inferir autorización para los demás.
4. Si existe autorización expresa para todos los ambientes, validar primero la semántica real de la configuración existente: confirmar mediante código y configuración aprobada si el alcance global se representa con gate activo y listas vacías, o con otro valor ya definido. No asumir que una lista vacía habilita a todos.
5. La ejecución material de la activación solo puede realizarse en una operación separada, autorizada y trazable. Esta etapa entrega la decisión, la matriz de cambio y el runbook; no la ejecuta.

## RUNBOOK PROPUESTO PARA UNA OPERACIÓN AUTORIZADA

Documentar un runbook conciso, a ejecutar solo por el responsable autorizado:

1. Confirmar versión desplegada, aprobación vigente, ambiente objetivo y salud previa mediante verificaciones de solo lectura.
2. Tomar una referencia sanitizada de la configuración vigente y del comportamiento de fallback, sin incluir secretos.
3. Aplicar exclusivamente la configuración existente y el alcance aprobados.
4. Verificar, con pruebas autorizadas y no mutantes cuando sea posible, que el gate se evalúa como se espera, que el fallback permanece disponible y que no hubo regresión de `Continuar flujo`.
5. Ante error, discrepancia, fuga de información, transición inesperada o ausencia de evidencia, detener la liberación y aplicar el rollback aprobado: gate inactivo, usuarios/grupos vacíos y retorno al postback legacy para nuevos intentos. No revertir transiciones ya confirmadas.
6. Registrar resultado, hora, versión, ambientes, responsables, evidencia sanitizada, decisión de continuación o rollback y riesgos residuales.

## CRITERIOS DE ACEPTACIÓN

- El resultado distingue de forma inequívoca entre aprobación técnica de pruebas y autorización operativa de activación.
- No existe activación implícita ni global por defecto.
- La matriz por ambiente indica estado de autorización, versión, alcance, responsable, ventana, evidencia y rollback.
- El plan reutiliza el gate existente de forma fail-closed y conserva el fallback Web Forms.
- Si no hay autorización, el gate termina inactivo y no se modifican usuarios ni grupos.

## DOCUMENTACIÓN TÉCNICA

Crear o actualizar `Doc/Actualizacion/workflow/TerminarGrupo/09-liberacion-activacion-controlada/` con:

- `00-indice.md`: alcance y decisión de liberación.
- `01-matriz-ambientes.md`: autorización, versión, ventana, responsables, alcance, evidencia y estado por ambiente.
- `02-runbook-activacion-y-rollback.md`: pasos de activación autorizada, verificaciones y rollback.
- `03-riesgos-y-aprobaciones.md`: bloqueos, riesgos residuales y referencias a las aprobaciones, sin secretos.

## ENTREGABLE FINAL

Entregar un reporte verificable con el estado de cada precondición, la decisión fundada (**bloquear**, **solicitar aprobación** o **lista para activación autorizada**), la matriz de ambientes, el runbook de activación y rollback, y la confirmación de que esta etapa no activó ni modificó el gate.
