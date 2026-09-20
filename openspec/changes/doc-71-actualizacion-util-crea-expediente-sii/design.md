## Context

DOC-71: ACTUALIZACION-UTIL-CREA-EXPEDIENTE-SII

## Jira Details

> Prompt backend 13 — Importación sin expediente según configuración del trámite
> Actúa como arquitecto y desarrollador senior de   WebForms/VB.NET. Corrige la separación entre importar documentos y gestionar expedientes en ImportarServicioWeb, conservando intacto el recorrido legacy y creando un cambio OpenSpec independiente.
> Rol esperado
> Arquitecto senior y desarrollador backend responsable de contratos, orquestación, persistencia, reconciliación y pruebas reales de ImportarServicioWeb, con dominio de   WebForms,  , .NET Framework 4.6.1 y MySQL 5.1.
> Diagnóstico confirmado
> La implementación moderna confunde dos conceptos diferentes:
> ExpedienteObligatorio: indica si la operación debe ejecutar efectos de expediente.
> 
> CreacionAutomaticaHabilitada: autoriza crear físicamente un expediente que no existe.
> 
> MySqlImportExpedientConfigurationRepository.Obtener(...) asigna actualmente ambos valores desde util_Estado_Crea_ExpedienteSII. Después, ImportExpedientCoordinator.Resolver(...) devuelve EXPEDIENT_CREATION_DISABLED antes de almacenar el documento cuando la bandera vale 0.
> Esto obliga indebidamente a configurar util_Estado_Crea_ExpedienteSII=1 en trámites que no deben crear expediente. En instalaciones con histórico documental, esa activación puede crear expedientes incompletos, dejar documentos anteriores fuera y romper la coherencia de foliación.
> Objetivo
> Aplicar esta regla autoritativa:
> util_Estado_Crea_ExpedienteSII
> Comportamiento
> 0 
> Importar el documento sin ejecutar resolución, creación, vínculo, caché ni actualización de índices de expediente. Esos efectos terminan como NoAplica. 
> 1 
> Ejecutar el flujo vigente de resolución de sujeto, caché, búsqueda, creación cuando sea necesaria, vínculo, índices y confirmación. 
> La bandera 0 no es un error y no debe producir EXPEDIENT_CREATION_DISABLED por sí sola. El documento debe poder completar su almacenamiento y la intención debe finalizar sin reconciliación falsa por efectos de expediente omitidos deliberadamente.
> Rutas canónicas de revisión e implementación
> Modelo/Workflow/ImportarServicioWeb/
> └── ImportarServicioWebModels.vb
> 
> Infrastructure/Repositories/Workflow/ImportarServicioWeb/
> ├── MySqlImportExpedientConfigurationRepository.vb
> └── PhysicalImportExpedientRepository.vb
> 
> Services/Workflow/ImportarServicioWeb/
> ├── ImportExpedientCoordinator.vb
> ├── ImportEffectPlanBuilder.vb
> ├── ImportExecutionSteps.vb
> ├── ServicioEjecucionImportacion.vb
> ├── ServicioPreflightImportacion.vb
> └── ServicioReconciliacionImportacion.vb
> 
> DTOs/Workflow/ImportarServicioWeb/
> └── ImportarServicioWebDtos.vb
> 
> tests/
> └── importar-servicio-web-*.test.cjs
> 
> tools/e2e/
> ├── scripts/adapters/importar-servicio-web-e2e-adapter.cjs
> └── tests/importar-servicio-web-modern.spec.cjsVerifica los nombres y rutas reales antes de modificar; no inventes clases ni métodos ausentes.
> Contexto obligatorio
> Antes de diseñar o modificar código, leer el cambio OpenSpec que se cree, la documentación DOC-67/DOC-70, AGENTS.md, tools/e2e/AGENT-RUNBOOK.md y todas las rutas canónicas enumeradas. Confirmar por análisis estructural las firmas, dependencias y estados reales; cualquier ruta o símbolo inexistente debe corregirse en el diseño y no inventarse.
> Investigación obligatoria
> Trazar desde PreflightImport hasta reconciliación todos los lugares que asumen expediente obligatorio.
> 
> Inventariar estados persistidos para resolución, vínculo, índices y caché, y definir cómo representar NoAplica sin reutilizar Confirmado ni Fallido de forma engañosa.
> 
> Confirmar qué condiciones permiten declarar completa una intención cuyos documentos se almacenaron sin expediente.
> 
> Verificar que el almacenamiento mediante AlmacenaDocumentoTareaWorkflow(...) funciona sin ExpedientId y sin índices dinámicos de gabinete; si no puede demostrarse, registrar el bloqueo y no simular éxito.
> 
> Identificar consultas, DTOs, verificadores y scripts de evidencia que actualmente exigen ITEMSHAVEEXPEDIENT, relación, índices o caché para todos los trámites.
> 
> Verificar que el flujo con bandera 1 conserva exactamente el comportamiento DOC-67 existente.
> 
> Requisitos positivos
> Implementar todos los comportamientos siguientes como una unidad coherente y verificable.
> 1. Semántica de configuración
> Dejar de derivar dos conceptos incompatibles de manera que la bandera 0 bloquee toda importación.
> 
> Modelar explícitamente el modo SinExpediente frente a GestionarExpediente, usando nombres inequívocos.
> 
> La configuración se obtiene exclusivamente desde el contexto confiable del trámite; el cliente no puede enviar ni alterar el modo.
> 
> Incluir el modo en el fingerprint de preflight para detectar cambios entre preparación y creación de intención.
> 
> 2. Preflight y plan de efectos
> Con bandera 0, el plan por item debe declarar:
> almacenamiento documental: Planned;
> 
> resolución de expediente: NotApplicable;
> 
> vínculo documental: NotApplicable;
> 
> índices de expediente: NotApplicable;
> 
> caché de vínculo/expediente: NotApplicable;
> 
> Executable=true cuando las demás validaciones estén satisfechas.
> 
> Con bandera 1, conservar los cinco efectos previstos del flujo de expediente.
> No exponer nombres físicos, SQL, gabinete dinámico, IDs de expediente ni detalles internos.
> 3. Ejecución sin expediente
> Cuando el modo sea SinExpediente, no invocar resolución de sujeto SII, caché, búsqueda, verificación ni creación de expediente.
> 
> No llamar solicitarToken, consultarExpedienteMercantil ni consultarExpedienteProponente para resolver sujeto.
> 
> Obtener y almacenar únicamente los sellos seleccionados mediante el flujo documental autorizado.
> 
> No escribir relaciones, caché ni índices de expediente.
> 
> Persistir estados explícitos NoAplica para los efectos omitidos, o una representación equivalente aprobada y verificable.
> 
> Completar item e intención cuando el almacenamiento documental esté confirmado y todos los demás efectos sean Confirmado o NoAplica según el plan.
> 
> No convertir NoAplica en reconciliación pendiente ni en éxito físico ficticio.
> 
> 4. Ejecución con expediente
> Mantener la secuencia actual: sujeto SII → caché → búsqueda → creación si está ausente → vínculo → índices → caché.
> 
> PhysicalImportExpedientRepository.Crear(...) conserva la comprobación tardía de CreacionAutomaticaHabilitada; no crear si la configuración autoritativa lo prohíbe.
> 
> Conservar idempotencia, detección de conflictos, postcondiciones físicas, reintento y reconciliación DOC-67.
> 
> No usar datos del propietario como sustituto de nit/sujeto dentro de este cambio; esa regla requiere un cambio independiente.
> 
> 5. Consulta, respuesta y reconciliación
> Exponer estados seguros y estables que permitan al frontend diferenciar Confirmado, Pendiente, Fallido y NoAplica.
> 
> Ajustar GetImportIntent y reconciliación para no exigir expediente en modo SinExpediente.
> 
> Una intención sin expediente no puede afirmar que creó, encontró o vinculó uno.
> 
> Conservar contratos existentes de forma aditiva y documentar cualquier nuevo valor de estado.
> 
> Flujo objetivo
> Validar contexto/tarea/tipología
>   -> leer configuración autoritativa
>   -> ¿util_Estado_Crea_ExpedienteSII = 1?
>        SÍ -> resolver sujeto -> caché/buscar/crear expediente
>           -> almacenar documento -> vincular -> índices -> caché
>           -> completar con efectos de expediente confirmados
>        NO -> omitir sujeto/caché/búsqueda/creación
>           -> almacenar documento
>           -> marcar resolución/vínculo/índices/caché como NoAplica
>           -> completar sin expedienteRestricciones críticas
> No modificar funciones legacy ni AlmacenaDocumentoTareaWorkflow(...).
> 
> No activar creación automática para conseguir que una prueba pase.
> 
> No crear expedientes para trámites configurados en 0.
> 
> No consultar primero SII para decidir unilateralmente si el trámite “parece” apto para expediente.
> 
> No reconstruir, migrar ni refoliar históricos dentro de este cambio.
> 
> No agregar fallback silencioso basado en propietario, organización, matrícula o datos personales.
> 
> No registrar respuestas SII crudas, NIT, teléfonos, correos, direcciones, cookies, tokens o credenciales.
> 
> Mantener .NET Framework 4.6.1, MySQL 5.1, procesamiento secuencial e implementación moderna paralela.
> 
> Matriz mínima de pruebas
> Bandera
> Expediente previo
> Resultado esperado
> 0 
> No aplica 
> Documento almacenado; efectos de expediente NoAplica; cero consultas/escrituras de expediente. 
> 1 
> Existe en caché 
> Verificar, vincular y completar sin crear duplicado. 
> 1 
> Existe físicamente, no en caché 
> Encontrar, vincular, registrar caché y completar. 
> 1 
> No existe 
> Crear una vez, verificar, vincular y completar. 
> Cambio 0 ↔ 1 entre preflight y creación 
> Cualquiera 
> Rechazar con fingerprint/preflight obsoleto antes de persistir. 
> Agregar casos de error para almacenamiento rechazado, respuesta incierta, conflicto de identidad y configuración ausente. Demostrar que NoAplica solo aparece cuando la configuración autoritativa lo ordena.
> Pruebas obligatorias
> Prueba de regresión que reproduzca el defecto confirmado: bandera 0 no devuelve EXPEDIENT_CREATION_DISABLED antes del almacenamiento.
> 
> Pruebas unitarias del plan para Planned/NotApplicable.
> 
> Dobles estrictos que fallen si se invoca sujeto SII, caché o repositorio de expediente con bandera 0.
> 
> Pruebas de persistencia, consulta y reconciliación de estados NoAplica.
> 
> Pruebas de fingerprint y rechazo ante cambio de configuración.
> 
> Regresión completa del flujo DOC-67 con bandera 1.
> 
> Compilación MSBuild y suite transversal de ImportarServicioWeb.
> 
> E2E real obligatoria
> Código, E2E, validación autorizada y evidencia saneada forman una única unidad de entrega dentro del mismo cambio; no crear una tarea o entrega E2E independiente.
> 
> Reutilizar exclusivamente tools/e2e, su runner, autenticación, perfiles, reservas, gate y verificadores; no crear un arnés paralelo.
> 
> Antes de ejecutar, leer AGENTS.md y tools/e2e/AGENT-RUNBOOK.md.
> 
> Ejecutar únicamente con ambiente, cuentas, gate, TLS y datos/tareas descartables expresamente autorizados; si falta cualquiera, registrar bloqueo y no sustituir la evidencia real.
> 
> Usar secretos efímeros; queda prohibido imprimir, registrar o persistir credenciales, cookies, tokens, cadenas de conexión y respuestas externas crudas.
> 
> Ejecutar una muestra descartable autorizada de un trámite con bandera 0: importar un sello y demostrar por consultas SELECT que existe el documento, no se creó/vinculó expediente, no se escribió caché ni índices de expediente y la intención terminó completa con efectos NoAplica.
> 
> Confirmar mediante telemetría saneada que el flujo con bandera 0 no llamó token ni consulta de sujeto SII; solo se permiten las llamadas necesarias para listar/obtener el sello.
> 
> Ejecutar otra muestra descartable autorizada de un trámite creador con bandera 1 y demostrar que el comportamiento DOC-67 continúa correcto.
> 
> Probar repetición/idempotencia sin duplicar documento, expediente, vínculo o caché.
> 
> Cubrir autorización, lectura sin mutación, escritura autorizada, repetición/idempotencia, una carrera controlada cuando la infraestructura existente la soporte y regresión de ambas ramas 0/1.
> 
> Cubrir, cuando aplique, autorización/control de acceso, lectura sin mutación, escrituras autorizadas, concurrencia y regresión relacionada.
> 
> Las consultas de control son exclusivamente SELECT; nunca imprimir respuestas SII crudas ni datos personales.
> 
> Toda ejecución requiere autorización expresa para ambiente, cuenta, gate, TLS y recurso descartable.
> 
> Restaurar siempre WorkflowCentroTrabajoModernActive=false, usuarios/grupos vacíos y comprobar páginas legacy sin cambios.
> 
> Si falta una muestra compatible para cualquiera de las dos ramas, registrar el bloqueo y no declarar la matriz E2E completa.
> 
> Respetar feature flags, gates, usuarios y grupos sin habilitarlos arbitrariamente. No cerrar sin validación autorizada: registrar bloqueo explícito y prohibir mocks, simulaciones, resultados inventados y evidencia ficticia.
> 
> Ruta documental obligatoria
> docs/Architecture/Workflow/ImportarServicioWeb/<TICKET>-importacion-sin-expediente/Documentar arquitectura real, secuencia de ambas ramas, matriz de estados, contratos, seguridad, migración si aplica, rollback, casos de uso, inventario técnico y evidencia saneada.
> Criterios de aceptación
> Un trámite con bandera 0 importa documentos sin gestionar expediente y termina correctamente.
> 
> Ninguna operación de expediente ni consulta de sujeto se ejecuta en esa rama.
> 
> Un trámite con bandera 1 conserva creación/vinculación DOC-67 sin regresiones.
> 
> Preflight, ejecución, consulta y reconciliación representan coherentemente NoAplica.
> 
> El backend deja de obligar a activar creación de expediente para importar documentos.
> 
> El recorrido legacy permanece intacto y el gate queda apagado por defecto.
> 
> Entregable final
> Entregar cambio OpenSpec independiente, implementación, contratos aditivos, pruebas, compilación, E2E autorizadas, documentación técnica y evidencia saneada. No mezclar este cambio con la resolución de sujeto de establecimientos ni con reconstrucción histórica de expedientes.

## Goals / Non-Goals

**Goals**
- Refinar alcance tecnico usando el contexto completo de Jira.
- Definir decisiones arquitectonicas, riesgos y plan de migracion.

**Non-Goals**
- Cambios fuera del alcance descrito por el ticket.

## Decisions

1. Las decisiones funcionales y tecnicas se completan durante `opsxj:refine`; no se inyectan politicas de otro perfil tecnologico.


## Risks / Trade-offs

- El refinamiento debe identificar compatibilidad, riesgos y limites del modulo afectado antes de iniciar cambios.

## Migration Plan

1. Completar y aprobar `refinement.md` antes de marcar tareas de implementacion.
2. Sincronizar cada decision con design, spec y tasks mediante `opsxj:refine --sync`.

## Open Questions

- TBD
