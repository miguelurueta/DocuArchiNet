## Context

DOC-70: PREFLING-DESCRIPCION

## Jira Details

> # Prompt backend 11 — Preflight con plan de efectos y destino
> 
> Actúa como arquitecto y desarrollador senior de ASP.NET WebForms/VB.NET. Extiende el preflight implementado sin ejecutar ni anticipar mutaciones de DOC-67 y crea un cambio OpenSpec nuevo.
> 
> ## Objetivo
> 
> Permitir que el frontend explique qué ocurrirá con la selección antes de crear la intención: tarea destino, tipología, requisitos y clases de efectos previstas, sin exponer identificadores físicos sensibles ni consultar nuevamente SII.
> 
> ## Rutas canónicas de implementación
> 
> ```txt
> DTOs/Workflow/ImportarServicioWeb/
> └── ImportarServicioWebDtos.vb
> 
> Services/Workflow/ImportarServicioWeb/
> ├── ServicioPreflightImportacion.vb
> └── ImportEffectPlanBuilder.vb
> 
> Infrastructure/Repositories/Workflow/ImportarServicioWeb/
> └── MySqlImportEffectConfigurationRepository.vb
> 
> Tests/
> ├── importar-servicio-web-preflight-effect-plan.test.cjs
> ├── importar-servicio-web-preflight-no-effects.test.cjs
> └── importar-servicio-web-preflight-no-sii-calls.test.cjs
> ```
> 
> - Extender `PreflightImportResponseDto` de forma aditiva con un plan por `ClientItemId`.
> - Reutilizar `Requirements`, `Commands` y `ContextFingerprint`; no crear un segundo preflight.
> - Resolver configuración mediante contexto confiable y repositorios parametrizados.
> 
> ## Ruta documental obligatoria
> 
> ```txt
> docs/Architecture/Workflow/ImportarServicioWeb/<TICKET>-preflight-plan-efectos-destino/
> ```
> 
> Crear paquete canónico, diagramas, matriz de campos, códigos funcionales y evidencia de ausencia de efectos.
> 
> ## Investigación obligatoria
> 
> - Precisar qué información de destino puede confirmarse antes de resolver/crear el expediente físico.
> - Diferenciar plan lógico confirmado de resultado físico todavía pendiente.
> - Inventariar configuración local necesaria para almacenamiento, expediente, vínculo, caché e índices.
> - Confirmar que todos los datos SII necesarios ya están en selección/intención o persistencia local.
> 
> ## Implementa
> 
> - `ImportEffectPlanDto` por item con `ClientItemId`, `TargetTaskId`, tipología confirmada, modo de destino, expediente requerido y efectos previstos: almacenamiento, resolución de expediente, vínculo, caché e índices.
> - Requisitos globales y por item, con código estable, satisfacción y mensaje seguro.
> - Indicador explícito `Executable` que solo sea verdadero cuando contexto, selección, tipología y configuración estén confirmados.
> - Códigos diferenciados para configuración ausente, tipología inválida, destino ambiguo y operación no disponible.
> - Huella de contexto que cubra los campos autoritativos usados para construir el plan.
> 
> ## Restricciones
> 
> - Preflight no crea intención, expediente, documento, vínculo, caché, índice, archivo temporal ni auditoría funcional.
> - No exponer `ExpedientId`, nombres físicos de tablas, gabinete dinámico, SQL, rutas o secretos.
> - No llamar token, `consultarInformacionSello`, preview ni descarga SII durante preflight.
> - No confiar en destino, gabinete, expediente o requisitos enviados por el cliente.
> - No modificar el orden ni el comportamiento síncrono de `ExecuteImportIntent`.
> 
> ## Aceptación
> 
> - Uno o varios elementos producen planes independientes dentro de la misma respuesta.
> - El plan informa efectos previstos sin afirmar que ya ocurrieron.
> - Repetir el preflight produce la misma huella mientras contexto/configuración no cambien.
> - Las pruebas demuestran cero escrituras y cero llamadas externas SII.
> - `CreateImportIntent` acepta únicamente selección/requisitos coherentes con el fingerprint vigente y conserva su idempotencia.
> 
> ## Trazabilidad
> 
> Brecha frontend: plan de efectos, destino lógico y requisitos confirmados. Dependencias: backend 03, 08, 09 y DOC-67 archivado.
> 
> ## Pruebas obligatorias
> 
> Usar dobles de repositorio, `node:test`, fixtures compartidos, pruebas SQL parametrizado y MSBuild. No ejecutar E2E real ni mutaciones de ambiente sin autorización.
> 
> ## E2E real obligatoria
> 
> - Reutilizar el runner, escenario de importación, autenticación, perfiles, gate y verificadores de `tools/e2e`; no crear otro arnés ni duplicar perfiles completos.
> - Extender la etapa preflight de `import-sii-execution` para capturar el plan antes de crear la intención y comprobarlo contra configuración autoritativa mediante consultas `SELECT`.
> - Probar selección individual y múltiple en los registros disponibles, verificando `ClientItemId`, `TargetTaskId`, tipología, modo de destino, expediente requerido, efectos previstos, requisitos y `Executable`.
> - Demostrar que repetir el mismo preflight produce el mismo fingerprint y que cambiar tarea, tipología o configuración genera rechazo/fingerprint distinto.
> - Tomar huellas `SELECT` antes y después para demostrar cero cambios en intención, expediente, documento, vínculo, caché, índices y auditoría funcional durante preflight.
> - Confirmar con telemetría saneada que preflight realiza cero llamadas de token, consulta, preview o descarga SII.
> - Como prueba integrada final, continuar una muestra descartable autorizada hasta una sola intención/ejecución y comprobar que los efectos reales son compatibles con el plan, sin exigir que el preflight revele `ExpedientId`.
> - Toda ejecución real requiere autorización expresa, evidencia saneada y restauración del gate a `false` con usuarios/grupos vacíos.
> 
> ## Entregable final
> 
> Entregar contratos aditivos, servicios, repositorios, pruebas, documentación y evidencia de que preflight permanece puro y no consume SII.

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
