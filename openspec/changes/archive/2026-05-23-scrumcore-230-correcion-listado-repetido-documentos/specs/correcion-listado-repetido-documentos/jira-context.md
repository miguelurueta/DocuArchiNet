# Jira Context - SCRUMCORE-230

## Summary

CORRECION-LISTADO-REPETIDO-DOCUMENTOS

## Description

> PROMPT ARQUITECTÓNICO — Corrección quirúrgica de listado documental repetido + validación estricta de Radicado
> Rol esperado
> Arquitecto frontend senior  (React 19, TypeScript estricto, AppTreeTable, Dynamic UI, integración API enterprise, state orchestration, testing enterprise)
> Objetivo
> Corregir el bug donde DocumentosWorkbench muestra repetidamente el mismo set documental entre tareas distintas por ausencia de filtro efectivo por Radicado, e implementar validación estricta de Radicado antes de consultar ListaDocumentosRadicados/query.
> La solución debe:
> garantizar aislamiento documental por tarea
> 
> evitar datos stale
> 
> preservar estabilidad UX
> 
> mantener compatibilidad con AppTreeTable/AppTable
> 
> evitar regresiones en visor y selección múltiple
> 
> IMPORTANTE
> Este ticket NO debe:
> modificar backend
> 
> cambiar endpoints
> 
> alterar contratos backend
> 
> romper AppTreeTable/AppTable
> 
> romper selección múltiple
> 
> romper flujo ver_documento
> 
> introducir estilos globales
> 
> usar any
> 
> El objetivo es exclusivamente:
> corregir filtrado documental
> 
> endurecer validación de Radicado
> 
> evitar reutilización de datos stale
> 
> mejorar estabilidad runtime
> 
> Dependencia
> DocumentosWorkbench
> 
> useGestionRespuestaDocumentosTable
> 
> gestionRespuestaDocumentosRequestMapper
> 
> getSolicitaGabinetePorTareaWorkflow
> 
> SCRUM-205 ListaDocumentosRadicados
> 
> AppTreeTable/AppTable
> 
> flujo actual de visualización documental
> 
> Contexto existente
> Actualmente:
> ListaDocumentosRadicados/query puede ejecutarse sin Radicado efectivo
> 
> el query termina devolviendo set general del gabinete
> 
> distintas tareas terminan viendo documentos repetidos
> 
> Problema raíz:
> El frontend no valida correctamente:
> Radicado
> 
> EstadoExistenciaRadicado
> 
> contexto real de tarea
> 
> y puede reutilizar información stale entre cambios rápidos de tarea.
> Estado actual
> El listado documental depende parcialmente de contexto incompleto y puede:
> reutilizar rows anteriores
> 
> consultar query sin Radicado válido
> 
> mostrar resultados incorrectos
> 
> mezclar tareas distintas
> 
> Ubicación esperada
> Hooks:src/modules/gestionCorrespondencia/hooks/useGestionRespuestaDocumentosTable.ts
> Adapters:src/modules/gestionCorrespondencia/adapters/gestionRespuestaDocumentosRequestMapper.ts
> Types:src/modules/gestionCorrespondencia/types/solicitaGabineteRadicadoWorkflow.types.ts
> Tests:src/modules/gestionCorrespondencia/tests/*src/modules/gestionCorrespondencia/adapters/*.test.ts
> Restricciones obligatorias
> NO modificar backendNO cambiar endpointsNO usar anyNO romper AppTreeTableNO romper AppTableNO romper selección múltipleNO romper flujo ver_documentoNO introducir estilos globales
> Regla arquitectónica obligatoria
> El Radicado válido para filtrar documentos debe provenir exclusivamente de la respuesta del endpoint de gabinete por tarea.
> Esto implica:
> NO usar Search como sustituto silencioso de Radicado
> 
> NO reutilizar Radicado stale de otra tarea
> 
> NO derivar Radicado desde UI si existe respuesta de gabinete
> 
> el query documental solo puede ejecutarse con contexto validado
> 
> Regla de source of truth
> La respuesta de:
> GET /api/workflow/ruta-trabajo/tareas/{idTareaWorkflow}/gabinete
> es la única fuente válida para:
> Radicado
> 
> NombreGabinete
> 
> EstadoExistenciaRadicado
> 
> Regla anti-stale obligatoria
> Al cambiar idTareaWf:
> invalidar rows previas
> 
> invalidar errores previos
> 
> limpiar resultados stale
> 
> resolver nuevamente gabinete
> 
> ignorar responses anteriores
> 
> Nunca:
> mostrar documentos de tarea previa como válidos para la nueva
> 
> Regla de concurrencia obligatoria
> Si cambia idTareaWf durante carga:
> ignorar responses stale
> 
> no sobrescribir rows actuales con responses anteriores
> 
> mantener estabilidad visual
> 
> Regla EstadoExistenciaRadicado obligatoria
> Si:
> EstadoExistenciaRadicado = "NO"
> Entonces:
> NO consultar ListaDocumentosRadicados/query
> 
> retornar error funcional controlado
> 
> mantener UX estable
> 
> evitar mostrar datos stale
> 
> Contrato backend vigente
> Gabinete por tarea
> 
> GET /api/workflow/ruta-trabajo/tareas/{idTareaWorkflow}/gabinete
> Response:
> Radicado
> 
> NombreGabinete
> 
> IdTareaWorkflow
> 
> EstadoExistenciaRadicado
> 
> Lista documentos
> 
> POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/query
> Campos relevantes:
> NombreGabinete
> 
> CampoRadicado
> 
> Radicado
> 
> Search
> 
> Reglas contractuales obligatorias
> CampoRadicado:
> usar "ENLASE" por defecto
> 
> Radicado:
> usar valor trim()
> 
> validar obligatorio
> 
> Search:
> NO reemplaza Radicado
> 
> Contrato frontend esperado
> Actualizar:
> solicitaGabineteRadicadoWorkflow.types.ts
> Debe incluir:
> Radicado?: string
> 
> NombreGabinete?: string
> 
> IdTareaWorkflow?: number
> 
> EstadoExistenciaRadicado?: "YES" | "NO"
> 
> Contrato mapper obligatorio
> buildListaDocumentosRadicadosRootQuery()
> Debe aceptar:
> nombreGabinete?: string
> 
> radicado?: string
> 
> idTareaWf?: number
> 
> Reglas de implementación obligatorias
> Completar tipos frontend
> 
> Agregar:
> Radicado
> 
> EstadoExistenciaRadicado
> 
> Root query
> 
> Debe enviar:
> CampoRadicado="ENLASE"
> 
> Radicado validado
> 
> NombreGabinete correcto
> 
> Hook documentos
> 
> load() debe:
> resolver gabinete
> 
> validar Radicado
> 
> validar EstadoExistenciaRadicado
> 
> solo entonces ejecutar query
> 
> Error controlado
> 
> Si Radicado inválido:
> retornar:{  ok: false,  message: "No fue posible cargar documentos: el radicado de la tarea es obligatorio."}
> Datos stale
> 
> Al cambiar tarea:
> limpiar rows previas
> 
> evitar render stale
> 
> mantener estabilidad visual
> 
> Logging dev-only
> 
> console.debug (solo no producción):
> idTareaWf
> 
> nombreGabinete
> 
> radicado
> 
> viewMode
> 
> Sin datos sensibles adicionales.
> Reglas de interacción
> documento activo no se rompe
> 
> selección múltiple intacta
> 
> loadChildren sigue funcionando
> 
> AppTreeTable estable
> 
> Accesibilidad y UX
> loading consistente
> 
> errores visibles
> 
> no flicker severo
> 
> no render stale
> 
> no pérdida foco
> 
> Reglas de performance
> evitar queries innecesarias
> 
> evitar doble fetch gabinete
> 
> evitar stale updates
> 
> mantener estabilidad rows
> 
> Manejo de errores obligatorio
> Caso A: Radicado vacío
> NO query documentos
> 
> error funcional controlado
> 
> Caso B: EstadoExistenciaRadicado=NO
> NO query documentos
> 
> error controlado
> 
> Caso C: request stale
> ignorar response
> 
> mantener estado correcto
> 
> Riesgos a evitar
> datos stale
> 
> rows incorrectas
> 
> mezcla entre tareas
> 
> race conditions
> 
> query sin Radicado
> 
> fallback incorrecto Search
> 
> selección/documento activo rotos
> 
> Pruebas unitarias obligatorias
> Mapper:
> CampoRadicado="ENLASE"
> 
> Radicado incluido correctamente
> 
> trim correcto
> 
> Hook:
> usa Radicado gabinete
> 
> NO query si Radicado vacío
> 
> NO query si EstadoExistenciaRadicado=NO
> 
> query correcta si Radicado válido
> 
> mensaje error exacto
> 
> Pruebas de integración UI obligatorias
> cambio tarea A -> B actualiza documentos
> 
> rows stale eliminadas
> 
> documento activo estable
> 
> selección múltiple intacta
> 
> Pruebas de interacción en navegador obligatorias
> cambio rápido tareas
> 
> no stale render
> 
> loading estable
> 
> no pérdida foco
> 
> Pruebas E2E obligatorias
> tareas distintas muestran documentos distintos
> 
> mismo radicado => mismo set válido
> 
> Radicado vacío => error controlado
> 
> selección múltiple intacta
> 
> ver_documento intacto
> 
> Pruebas de regresión obligatorias
> AppTreeTable estable
> 
> AppTable estable
> 
> visor PDF estable
> 
> Dynamic UI estable
> 
> Pruebas QT / calidad
> sin errores build
> 
> sin warnings TS/lint
> 
> sin errores consola
> 
> sin memory leaks
> 
> sin regresiones visuales
> 
> Criterios de aceptación
> Cambiar tarea cambia efectivamente set documental
> 
> No se repiten documentos incorrectamente
> 
> Radicado vacío NO consulta query
> 
> Error controlado visible
> 
> Datos stale no se muestran
> 
> loadChildren sigue funcionando
> 
> selección múltiple intacta
> 
> tests pasan correctamente
> 
> Documentación obligatoria
> Ruta:
> docs/modulos/gestioncorrespondencia/
> Archivos obligatorios:
> SCRUMCORE-[XX]-Arquitectura.md
> 
> SCRUMCORE-[XX]-Implementacion-Detallada.md
> 
> SCRUM-[XX]-Integracion-BackEnd.md
> 
> SCRUM-[XX]-Pruebas.md
> 
> SCRUM-[ID]-Metadata.md
> 
> Debe incluir obligatoriamente:
> source of truth Radicado
> 
> estrategia anti-stale
> 
> validación EstadoExistenciaRadicado
> 
> flujo query documental
> 
> fallback/error strategy
> 
> ejemplos request/response
> 
> matriz FE-BE
> 
> Entrega esperada
> Diff archivos tocados
> 
> Resumen técnico:
> 
> filtrado Radicado
> 
> anti-stale strategy
> 
> validación estricta
> 
> Evidencia tests ejecutados
> 
> Confirmación explícita:
> 
> backend NO modificado
> 
> endpoints NO modificados
> 
> AppTable/AppTreeTable NO impactados globalmente
> 
> Instrucción final
> Implementar una corrección quirúrgica y robusta del filtrado documental por Radicado garantizando aislamiento correcto entre tareas, validación estricta del contexto documental y eliminación completa de datos stale, preservando completamente la estabilidad de AppTreeTable, visor PDF y selección múltiple.

## Metadata

- Tipo: Historia
- Prioridad: Medium
- Labels: CORRECCION, DOCUMENTOS, LISTADO, RADICADO, REPETIDO
