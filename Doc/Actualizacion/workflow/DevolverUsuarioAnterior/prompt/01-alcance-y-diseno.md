# 01 — Alcance y diseño para ticket Jira

## ROL ESPERADO

Actúa como arquitecto senior de .NET Framework, VB.NET, ASP.NET Web Forms, MySQL y Workflow.

## OBJETIVO

Desde el ticket Jira inicial, consolidar la decisión técnica de **Devolver a usuario anterior** que habilita la etapa 02. Esta etapa no implementa código ni crea planificación paralela.

## CONTEXTO OBLIGATORIO

- Requiere ticket de inicio aprobado y lectura de `00-contexto-obligatorio.md`.
- Leer los dos documentos de `../Exploracion/`.
- Registrar en Jira si la salida habilita o bloquea 02.

## REQUISITOS POSITIVOS

- Precisar contratos objetivo de `PreviewDevolverUsuarioAnterior` y `EjecutarDevolverUsuarioAnterior`.
- Definir el registro histórico inmediato anterior, sus atributos mínimos y cuándo representa un usuario válido.
- Definir códigos de bloqueo para historial ausente, grupo, usuario retirado, auto-devolución, token y lock.
- Definir auditoría, lock, token y el adaptador exclusivo hacia `Terminar_Tarea_Workflow`.
- Registrar exclusiones: actividad anterior, grupos, otras operaciones modernas y todo tratamiento de respuestas.
- Fijar el algoritmo determinista de historial: qué registro representa el estado actual, cuál es el inmediatamente anterior, el orden estable y desempate, los campos de Ruta/Flujo aplicables y la definición verificable de usuario elegible.
- Definir `TokenVersion` opaco, vinculado a versión de tarea e identificador del registro histórico confirmado; la ejecución debe bloquear si el historial ya no coincide.
- Definir un lock exclusivo por `IdTarea`, independiente del token, y su liberación; token e historial se revalidan dentro del lock.
- Definir el uso de `Terminar_Tarea_Workflow`: parámetros de Ruta/Flujo, `Page = Nothing`, actualización legacy desactivada, política aprobada de notificación y comportamiento de eventos dinámicos. El adaptador no puede usar componentes de respuestas; si el motor legacy los construye internamente, los parámetros deben impedir invocar sus métodos y una prueba focal debe demostrarlo.
- Definir el reemplazo de la ruta legacy: el comando moderno se ofrece en contexto válido y el preview bloquea cuando no hay destino; el enlace no puede invocar `inicializa_tipo_adjunto_documento`, `Button_tool_devolver_a_usuario` ni otro postback.
- Definir el registro de presentación para esta operación sin evaluar `WorkflowCentroTrabajoModernActive`, sin retirar ni cambiar la política de gate de otras operaciones modernas.

## RESTRICCIONES CRÍTICAS

- No modificar código, configuración, endpoints, pruebas ni tickets Jira fuera de la evidencia autorizada.
- No usar `PreviewEnviarUsuario`, `PreviewEnviarTarea` ni sus contratos como sustituto del historial de usuario.
- No diseñar búsqueda, paginación, selector de destinos, postback ni ruta Web Forms alternativa.
- No incluir reglas, datos o componentes de respuestas.

## REGLAS DE ANTIRREGRESIÓN

- Preservar los contratos de Devolver a actividad anterior, Continuar flujo, Enviar a usuario y Enviar a grupo.
- El contrato público no recibe identificadores de destino ni de historial y la auto-devolución usa el usuario autenticado real.

## CRITERIOS DE ACEPTACIÓN

- La documentación identifica una operación por historial de usuario separada y verificable.
- Define cómo se bloquea un historial sin usuario, el contrato de token/historial, el lock por tarea y las políticas de notificación, eventos y sustitución de la ruta legacy.
- El ticket habilita 02 o queda bloqueado con causa y responsable.

## PRUEBAS OBLIGATORIAS

No ejecutar pruebas ni compilación por defecto: no hay código. Dejar la matriz de pruebas, compilación y QA reproducible para los tickets sucesores.

## DOCUMENTACIÓN TÉCNICA

Actualizar únicamente los documentos de exploración y la propuesta de arquitectura del paquete documental de DevolverUsuarioAnterior.

## ENTREGABLE FINAL

Reportar ticket, decisiones cerradas, bloqueos, archivos documentales actualizados y criterio exacto para iniciar 02.
