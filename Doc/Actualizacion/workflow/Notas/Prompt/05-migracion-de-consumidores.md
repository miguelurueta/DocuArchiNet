# Prompt 05 — Migrar Radicación, Gestión e histórico sin duplicación

## Prompt para ejecutar

```text
Aplica primero el contexto común de Prompt/00-guia-de-uso-y-contexto-comun.md. Requiere que Centro de Trabajo tenga adaptación moderna revisada, aunque la bandera continúe deshabilitada.

Objetivo: sustituir los clientes duplicados de Notas en Radicación Entrante y Gestión de Correspondencia por el cliente y contrato modernos. Tratar la consulta histórica como un consumidor separado de solo lectura y no asumir su política de visibilidad.

Rol esperado: arquitecto y desarrollador senior de ASP.NET Web Forms/VB.NET y JavaScript, responsable de migrar consumidores de forma individual, reversible y sin duplicación funcional.

Contexto obligatorio: revisa el adaptador de Centro de Trabajo aprobado, contratos modernos, páginas `.aspx`, code-behind `.vb`, scripts, estilos y endpoints ASMX existentes de Radicación Entrante, Gestión de Correspondencia e histórico. Ubica cambios únicamente en las rutas del consumidor y adaptadores existentes; el cliente compartido permanece en su ruta establecida por la fase 04 y el dominio en las capas Workflow correspondientes.

Restricciones críticas:
- No migrar los tres consumidores en una modificación indivisible, habilitar gates, retirar endpoints legacy, aplicar migraciones ni ejecutar E2E autenticada.
- No copiar CRUD/serialización, reactivar permisos comentados, depender de tarea mutable de sesión ni de `Session("ID_TAREA_SELECCIONDA")`, exponer mutaciones en histórico ni producir doble escritura/modales.
- No asumir las decisiones DP-01/DP-02 ni cambiar semántica legacy; registra el bloqueo y preserva el fallback individual.
- No romper consumidores no migrados ni cambios ajenos; cualquier dependencia nueva requiere justificación aprobada.

Pruebas obligatorias: ejecuta pruebas focales o QA manual reproducible por consumidor para autorización, tarea/nota cruzada, conflictos, contenido, cursor, rollback y ausencia de doble operación. Integra con la migración de cada consumidor sus E2E de regresión; no las posterga a la fase 06 ni a una entrega separada. Compila con MSBuild o `dotnet` cuando cambie código VB y registra comandos, resultados y evidencia saneada.

Documentación técnica: actualiza bajo `Doc/Actualizacion/workflow/Notas/` la matriz por consumidor, propuesta OpenSpec y matriz de pruebas con contrato, rutas, fallback, pruebas, resultado, deuda legacy y rollback. No crear documentación en la raíz.

Entregable final: entrega una migración verificable por consumidor, cliente compartido sin duplicación, comandos/pruebas con resultado, matriz de migración, riesgos y deuda que pasa a la fase 06.

E2E integrada obligatoria por consumidor: incorpora y cumple `bloque-e2e-integrado-en-modernizacion.md` con cada migración. Reutiliza la autenticación, configuración, validadores y evidencias existentes; ejecuta pruebas reales solo sobre ambiente, cuentas y tareas autorizados, con secretos efímeros, controles `SELECT` y evidencia saneada. Cubre autorización, lectura sin mutación, escritura autorizada si la pantalla la expone, concurrencia cuando aplique, fallback/rollback y regresión. Ante ausencia de ambiente, datos o autorización, registra el bloqueo explícito; no uses mocks, simulaciones, resultados inventados ni habilitación arbitraria de gates.

Controles E2E obligatorios: la E2E es parte integral del mismo cambio y no una tarea o entrega independiente. Reutiliza exclusivamente `tools/e2e`, su autenticación, configuración, validadores, evidencias y utilidades; no crear login, arnés, proyecto Playwright, configuración ni `.env` paralelos. Antes de una E2E autenticada lee `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`; ejecútala solo con ambiente, cuentas y datos o tareas descartables expresamente autorizados. Usa secretos efímeros y no exponer, imprimir ni persistir credenciales, cookies, tokens ni cadenas de conexión; las verificaciones son solo `SELECT` y toda evidencia saneada. Cubre, cuando aplique, autorización y control de acceso, lectura sin mutación, escrituras autorizadas, concurrencia y regresión. Respeta feature flags, gates, usuarios, grupos y seguridad sin habilitarlos arbitrariamente; la implementación no se considera terminada sin validación autorizada y registra bloqueo explícito sin mocks, simulaciones ni evidencia ficticia.

Alcance: RF-11, RF-14, RF-19 y RF-20; RN-08, RN-10, RN-15 y RN-16; RNF-04, RNF-05 y RNF-07.

1. Inventaría por consumidor los puntos de entrada actuales, identidad de tarea, permisos, scripts AJAX, contadores, eventos y fallback. No cambies los tres consumidores en una única modificación no reversible; implementa y verifica uno por vez.
2. Extrae o reutiliza el único cliente de Notas construido en fase 04. Elimina duplicación funcional, pero conserva adaptadores mínimos para particularidades de cada pantalla; no copies de nuevo CRUD ni serialización.
3. Migra Radicación Entrante usando contratos con idTarea explícito y los resultados funcionales existentes. Conserva el mecanismo de rollback por consumidor y evita doble escritura o modales simultáneos.
4. Migra Gestión de Correspondencia con las mismas garantías. No reactives ni reproduzcas verificaciones de permiso comentadas del legacy: el permiso lo determina el gate de servidor.
5. Para histórico, primero verifica que DP-02 y DP-01 estén aprobadas. Implementa únicamente lectura, orden estable y política de estado explícita. No expongas creación, edición ni borrado en histórico.
6. Revisa que toda pantalla use el resultado de tarea validada por backend y que ninguna ruta moderna dependa de la última tarea guardada en sesión ni de `Session("ID_TAREA_SELECCIONDA")`.
7. Agrega pruebas de regresión por consumidor: autorización directa, tarea ajena/inactiva, nota cruzada, contenido texto, conflicto, cursor si corresponde, rollback de bandera y ausencia de doble operación. Integra la E2E indicada, ejecútala solo con autorización y no actives el gate.

Fuera de alcance: retirar endpoints legacy, eliminar WebFormAnotacion, aplicar migraciones de datos/esquema no aprobadas y activar usuarios/grupos piloto.

Criterios de aceptación:
- Existe un único contrato y cliente de Notas para los tres consumidores operativos.
- Cada consumidor puede volver individualmente al flujo legacy sin afectar datos ni crear duplicados.
- Histórico se limita a lectura bajo política aprobada.
- Las rutas modernas no usan tarea mutable de sesión ni autorización solo visual.

Entrega una matriz por consumidor con: contrato usado, flag/fallback, pruebas, resultado y deuda legacy que queda para fase 06.
```
