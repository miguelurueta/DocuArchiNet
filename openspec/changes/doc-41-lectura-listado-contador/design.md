## Context

DOC-41: LECTURA-LISTADO-CONTADOR

## Jira Details

> # Prompt 02 — Lectura, listado y contador seguros
> 
> ## Prompt para ejecutar
> 
> ```text
> Aplica primero el contexto común de Prompt/00-guia-de-uso-y-contexto-comun.md. Requiere la fundación de Prompt 01 aprobada o disponible en la misma propuesta.
> 
> Objetivo: implementar el camino moderno de solo lectura para listar, consultar contenido y contar notas de una tarea autorizada. No implementar crear, editar ni eliminar en esta fase.
> 
> Rol esperado: arquitecto y desarrollador senior de ASP.NET Web Forms/VB.NET y MySQL, responsable de una lectura moderna segura, acotada y compatible con los contratos de la fase 01.
> 
> Contexto obligatorio: parte de la fundación aprobada y revisa los contratos de DTOs, modelos, servicios e interfaces de Notas. Ubica repositorios en `Infrastructure/Repositories/Workflow/`, reglas de aplicación en `Services/Workflow/`, contratos en DTOs/modelos Workflow y los endpoints ASMX, si son necesarios, en `webservice/`. Conserva las fronteras de páginas `.aspx` y code-behind `.vb`; no crees lógica de negocio en la interfaz.
> 
> Restricciones críticas:
> - No crear, editar, eliminar, migrar consumidores, cambiar el gate ni consultar una base real sin autorización.
> - No usar `Class_anotacion_tarea`, `SELECT *` para el contador, SQL dinámico por orden/filtro ni `Session("ID_TAREA_SELECCIONDA")` como tarea objetivo.
> - No revelar existencia, contenido, conteos ni cursores de otra tarea o contexto; preserva comportamiento legacy y cambios ajenos.
> - No activar histórico sin una política de negocio aprobada.
> 
> Pruebas obligatorias: implementa pruebas focales de repositorio y servicio para autorización, orden, cursor, aislamiento y contador. Integra en este mismo cambio la E2E de rechazo anónimo y lectura autorizada con controles antes/después que demuestren ausencia de mutación. Ejecuta MSBuild o `dotnet` cuando afecte proyectos VB y registra comandos, resultados y evidencia saneada; complementa con QA manual reproducible solo si no existe una prueba automatizable apropiada.
> 
> Documentación técnica: actualiza propuesta OpenSpec, modelo de requerimientos y matriz de pruebas bajo `Doc/Actualizacion/workflow/Notas/` con el contrato de lectura, límites de cursor, política de histórico, rutas modificadas y riesgos. No documentes en la raíz.
> 
> Entregable final: entrega contratos de lectura, implementación parametrizada, pruebas y comandos ejecutados con resultado, más la política pendiente de histórico y precondiciones concretas para la fase 03.
> 
> Flujo paso a paso esperado: el endpoint recibe `idTarea` y filtros permitidos; el gate resuelve actor y autoriza la tarea; el servicio valida cursor/orden y aplica la política de visibilidad; el repositorio ejecuta la consulta parametrizada; el servicio normaliza un resultado seguro y el transporte devuelve listado, contenido o contador sin revelar datos de otro contexto.
> 
> E2E integrada obligatoria: incorpora y cumple `bloque-e2e-integrado-en-modernizacion.md` en este cambio. La cobertura incluye autorización/control de acceso, rechazo anónimo, listado y consulta autorizados, lectura sin mutación, aislamiento entre tareas/contextos y regresión de contador/paginación. Si no existe autorización, ambiente, cuenta o tarea descartable, deja el bloqueo explícito; no uses mocks ni una infraestructura paralela.
> 
> Controles E2E obligatorios: la E2E es parte integral del mismo cambio y no una tarea o entrega independiente. Reutiliza exclusivamente `tools/e2e`, su autenticación, configuración, validadores, evidencias y utilidades; no crear login, arnés, proyecto Playwright, configuración ni `.env` paralelos. Antes de una E2E autenticada lee `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`; ejecútala solo con ambiente, cuentas y datos o tareas descartables expresamente autorizados. Usa secretos efímeros y no exponer, imprimir ni persistir credenciales, cookies, tokens ni cadenas de conexión; las verificaciones son solo `SELECT` y toda evidencia saneada. Cubre, cuando aplique, autorización y control de acceso, lectura sin mutación, escrituras autorizadas, concurrencia y regresión. Respeta feature flags, gates, usuarios, grupos y seguridad sin habilitarlos arbitrariamente; la implementación no se considera terminada sin validación autorizada y registra bloqueo explícito sin mocks, simulaciones ni evidencia ficticia.
> 
> Alcance: RF-01, RF-02, RF-04, RF-09, RF-11, RF-12, RF-15 y RF-19; RN-08, RN-15 y RN-16; RS-02, RS-03, RS-10; RNF-01, RNF-02 y RNF-06.
> 
> 1. Implementa repositorios MySQL parametrizados para listado, contenido y contador, detrás de interfaces de Notas. No uses Class_anotacion_tarea como repositorio ni SELECT * para contar.
> 2. En toda lectura, usa idTarea del contrato y el puerto de acceso autorizado a tarea. Verifica pertenencia nota-tarea antes de devolver contenido. Una nota de otra tarea nunca se revela por id.
> 3. Implementa paginación por cursor protegido. El cursor debe estar ligado a idTarea, actor o contexto, filtros y orden. No aceptar cursores trasladados desde otra tarea o usuario. Define tamaño por defecto y máximo; no cargar sin límite.
> 4. Usa orden estable por defecto: fechaCreacion DESC e idNota DESC. Cualquier orden alternativo debe venir de una lista blanca; no interpolar campos de orden ni filtros en SQL.
> 5. Implementa el contador como COUNT(*) parametrizado bajo exactamente la misma política de visibilidad del listado operativo. El contrato no debe incentivar sondeo inferior a 30 segundos; el consumidor se actualizará por evento en una fase posterior.
> 6. Delimita el histórico: si la decisión DP-02 (visibilidad histórica) no está aprobada, no actives ni expongas el modo histórico moderno. Puedes dejar la interfaz preparada para una política explícita, sin asumir que histórico equivale a operativo.
> 7. Define y prueba respuestas sin filtración de existencia: tarea no accesible, nota de otra tarea, cursor inválido y orden no permitido devuelven resultados funcionales seguros.
> 8. Agrega pruebas de repositorio con dobles/fakes y pruebas de servicio para: orden estable, paginación, cursor cruzado, contenido cruzado, tarea inactiva y contador consistente. Integra la E2E indicada y no la ejecutes ni consultes una base real sin autorización.
> 
> Fuera de alcance: mutaciones, idempotencia de creación, UI, migración de pantallas y baja de endpoints legacy.
> 
> Criterios de aceptación:
> - Ningún endpoint moderno de lectura acepta idNota sin idTarea.
> - El orden es determinista y el contador no materializa filas para contarlas.
> - Cursor y orden no introducen fuga entre contextos ni SQL dinámico inseguro.
> - Histórico permanece bloqueado o definido por política explícita; no se infiere de forma accidental desde ESTADO_TAREA.
> - No se altera la bandera de activación ni se crean acciones de escritura.
> 
> Entrega un resumen de contratos reales, pruebas, política pendiente de histórico y precondiciones de la fase 03.
> ```

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
