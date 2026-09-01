<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04 -->
## Purpose

Modernizar el consumo de Notas en el Centro de Trabajo con una experiencia segura, accesible y reversible que conserva el flujo legacy como fallback.

## ADDED Requirements
### Requirement: INTEGRAR-CENTRO-TRABAJO-NOTA
El sistema SHALL implementar el alcance definido para DOC-43.
#### Scenario: Flujo principal
- **WHEN** se ejecuta el caso de uso principal del ticket
- **THEN** el comportamiento coincide con las reglas funcionales esperadas
#### Scenario: No-regresion
- **WHEN** se valida el modulo afectado
- **THEN** no se rompen flujos existentes

#### Scenario: Gate y fallback (D-01, RQ-01)
- **WHEN** `WorkflowCentroTrabajoModernActive` está deshabilitado
- **THEN** permanece operativo el flujo legacy sin doble operación

#### Scenario: Adaptador único (D-02, RQ-02)
- **WHEN** se consulta o modifica una nota
- **THEN** se usa el adaptador moderno con `idTarea` explícito, serialización JSON real y una sola operación

#### Scenario: Seguridad y accesibilidad (D-03, RQ-03)
- **WHEN** se muestra contenido o un conflicto
- **THEN** el texto se renderiza de forma segura y los controles mantienen foco, teclado y recuperación de errores

#### Scenario: Validación y rollback (D-04, RQ-04)
- **WHEN** se ejecuta QA o E2E autorizada
- **THEN** se cubren los viewports definidos, se conserva evidencia saneada y el gate termina apagado
### Requirement: Detalle funcional Jira
El sistema SHALL considerar las reglas detalladas del ticket.

#### Scenario: Reglas del ticket
- # Prompt 04 — Integrar Centro de Trabajo y UI aprobada
- 
- ## Prompt para ejecutar
- 
- ```text
- Aplica primero el contexto común de Prompt/00-guia-de-uso-y-contexto-comun.md. Requiere los contratos modernos de lectura y escritura aprobados y pruebas de las fases 01 a 03.
- 
- Objetivo: migrar únicamente el Centro de Trabajo Workflow al contrato moderno de Notas, detrás de una activación reversible que permanece deshabilitada por defecto. Implementar la lista cronológica definida en modelo-ui-notas-workflow-moderno.html sin cambiar reglas de backend.
- 
- Rol esperado: arquitecto y desarrollador senior de ASP.NET Web Forms, JavaScript y accesibilidad, responsable de integrar un único adaptador cliente sobre los contratos aprobados sin alterar el dominio.
- 
- Contexto obligatorio: revisa el modelo UI aprobado, el contrato CSS y las rutas existentes del Centro de Trabajo antes de cambiar páginas `.aspx`, code-behind `.vb`, scripts, estilos o `webservice/`. Ubica el adaptador en el script existente del consumidor y los estilos bajo el alcance CSS indicado; no traslada autorización ni reglas de negocio al navegador.
- 
- Restricciones críticas:
- - No cambiar reglas backend, habilitar `WorkflowCentroTrabajoModernActive`, usuarios/grupos piloto ni ejecutar E2E autenticada.
- - No usar `innerHTML`, JSON concatenado, `Session("ID_TAREA_SELECCIONDA")`, una acción Enviar como guardado de nota, doble escritura ni dos canales de actualización en paralelo.
- - No romper GridView, scripts o eventos legacy mientras el gate está deshabilitado; conserva fallback y cambios ajenos.
- - No introducir dependencias cliente nuevas si el proyecto ya dispone de una alternativa; justifica por escrito cualquier excepción aprobada.
- 
- Pruebas obligatorias: ejecuta pruebas focales del adaptador/cliente y QA manual reproducible de accesibilidad, estados y matriz responsive. Integra en este mismo cambio la E2E del recorrido de Centro de Trabajo, fallback, control de acceso y regresión relacionada; no la difieras a la fase 06 ni a una entrega separada. Compila con MSBuild o `dotnet` si se modifica código VB/proyecto y registra comandos, resultados, navegador, viewport y evidencia estática saneada.
- 
- Documentación técnica: actualiza el modelo UI, matriz de pruebas, propuesta OpenSpec y documentación del consumidor bajo `Doc/Actualizacion/workflow/Notas/` con contrato cliente, fallback, accesibilidad, responsive, rutas modificadas y rollback. No documentes en la raíz.
- 
- Entregable final: entrega el adaptador único, estilos encapsulados, pruebas, matriz QA responsive, comandos/resultados, evidencia estática, archivos modificados y rollback exacto del Centro de Trabajo.
- 
- E2E integrada obligatoria: incorpora y cumple `bloque-e2e-integrado-en-modernizacion.md` en este cambio. Reutiliza solo la infraestructura E2E existente y ejecuta la validación real únicamente con ambiente, usuario y tarea descartable autorizados, sin habilitar arbitrariamente el gate, usuarios ni grupos. Cubre autorización, lectura sin mutación cuando aplique, operaciones autorizadas, fallback y regresión. Si el gate, ambiente, datos o autorización no permiten una corrida válida, deja el bloqueo explícito sin mocks, simulaciones ni evidencia ficticia.
- 
- Controles E2E obligatorios: la E2E es parte integral del mismo cambio y no una tarea o entrega independiente. Reutiliza exclusivamente `tools/e2e`, su autenticación, configuración, validadores, evidencias y utilidades; no crear login, arnés, proyecto Playwright, configuración ni `.env` paralelos. Antes de una E2E autenticada lee `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`; ejecútala solo con ambiente, cuentas y datos o tareas descartables expresamente autorizados. Usa secretos efímeros y no exponer, imprimir ni persistir credenciales, cookies, tokens ni cadenas de conexión; las verificaciones son solo `SELECT` y toda evidencia saneada. Cubre, cuando aplique, autorización y control de acceso, lectura sin mutación, escrituras autorizadas, concurrencia y regresión. Respeta feature flags, gates, usuarios, grupos y seguridad sin habilitarlos arbitrariamente; la implementación no se considera terminada sin validación autorizada y registra bloqueo explícito sin mocks, simulaciones ni evidencia ficticia.
- 
- Alcance: RF-13, RF-14 y requerimientos de UX, accesibilidad y compatibilidad; RS-04 y RS-05; RNF-04, RNF-05 y RNF-07.
- 
- 1. Lee el modelo UI aprobado y el contrato CSS reutilizable del Centro de Trabajo. Encapsula nuevos estilos bajo .workflow-centro-trabajo-moderno; no rompas estilos legacy ni uses Enviar como acción primaria para guardar nota.
- 2. Crea un cliente JavaScript único y tipado/estructurado para Notas. Usa serialización JSON real; nunca concatenes JSON. Renderiza texto con textContent o mecanismo equivalente, nunca innerHTML para contenido, autor o metadatos de nota.
- 3. Integra listar, paginar, contar, crear, editar y eliminar usando idTarea explícito que provenga del contexto de la pantalla y del contrato moderno, no de Session("ID_TAREA_SELECCIONDA"). Las acciones se muestran solo como reflejo de respuestas/autorización de servidor.
- 4. Implementa estados de carga, vacío, error, éxito y conflicto. Mantén foco, teclado, etiquetas accesibles, Escape en diálogos y objetivos táctiles mínimos de 40 px. No dejes controles bloqueados después de error o conflicto.
- 5. Actualiza contador después de operaciones confirmadas y cambio de tarea. Elimina el sondeo de 600 ms solo para el camino moderno; no abras dos canales que escriban o actualicen el mismo elemento en paralelo.
- 6. Conserva GridView, scripts y eventos legacy como fallback mientras la bandera está en false. La ruta moderna y la legacy no deben ejecutar una misma operación dos veces. No modifiques el valor final de WorkflowCentroTrabajoModernActive: queda false, sin usuarios ni grupos piloto.
- 7. Prueba unitariamente el cliente y, si existe infraestructura apta sin autenticación, los adaptadores de pantalla. Incluye XSS de texto, comillas/Unicode/saltos, error, conflicto, teclado y dos cambios de tarea consecutivos. Integra la E2E indicada, ejecútala solo con autorización y no actives el gate.
- 8. Ejecuta QA responsive no autenticada y conserva una matriz de evidencia para 375 px, 768 px, 1024 px y 1440 px; en móvil valida también orientación vertical y horizontal. En cada viewport comprueba que no haya desplazamiento horizontal no intencional, recortes de la lista, diálogos o acciones, objetivos táctiles menores de 40 px, foco invisible ni superposición de controles.
- 9. Valida el flujo de carga, vacío, error, éxito y conflicto al menos en 375 px y 1024 px. Adjunta capturas o evidencia estática de cada viewport/estado, con navegador y resultado; cubre los navegadores soportados por el proyecto y registra como limitación verificable cualquier navegador o dispositivo no disponible.
- 
- Fuera de alcance: cualquier consumidor o módulo ajeno a `workflow/`, cambios de servidor no necesarios para el adaptador y retiro de legacy.
- 
- Criterios de aceptación:
- - La nueva lista coincide con el modelo aprobado y no introduce una grilla objetivo nueva.
- - No hay innerHTML ni JSON concatenado para datos de notas.
- - El fallback legacy sigue disponible y no hay doble operación.
- - La integración no activa ninguna audiencia ni cambia la configuración de gate.
- - La matriz QA responsive cubre 375 px, 768 px, 1024 px y 1440 px, orientación móvil, estados relevantes y evidencia de foco, objetivos táctiles y ausencia de recortes o desplazamiento horizontal no intencional.
- 
- Entrega la matriz QA responsive con capturas o evidencia estática, pruebas, archivos modificados y plan exacto de rollback del consumidor Centro de Trabajo.
- ```
