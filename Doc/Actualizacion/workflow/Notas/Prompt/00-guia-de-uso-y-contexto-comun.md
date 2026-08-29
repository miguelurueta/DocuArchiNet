# Prompts de implementación — Notas de Workflow

## Propósito

Esta carpeta divide la modernización en entregas pequeñas, verificables y reversibles. Cada archivo posterior es un prompt autónomo para un agente de implementación. Deben ejecutarse en orden y con una propuesta OpenSpec dedicada; no forman parte de los cambios OpenSpec activos de otros dominios.

## Rol esperado

Arquitecto y desarrollador senior de DocuArchi con experiencia en ASP.NET Web Forms, VB.NET, ASMX y MySQL. Debe preservar las fronteras del monolito modular y revisar el código existente antes de modificarlo.

## Objetivo

Guiar una modernización gradual de Notas exclusivamente dentro del módulo `workflow/`, con contratos explícitos, seguridad en servidor, migración reversible y evidencia reproducible por fase.

## Restricciones críticas

- No modificar cambios OpenSpec ajenos ni sobrescribir trabajo no relacionado.
- No habilitar gates, audiencias, consumidores modernos, E2E reales ni escrituras sin la autorización indicada en `AGENTS.md` y el runbook.
- No guardar ni exponer secretos, cookies, cadenas de conexión, contenido de notas ni respuestas sensibles.
- No usar `Session("ID_TAREA_SELECCIONDA")` como origen de una tarea moderna, duplicar operaciones legacy ni cambiar decisiones de negocio no aprobadas.

## Contexto obligatorio

Las fuentes y el orden de ejecución de las fases se describen en las secciones siguientes. La implementación se limita a las rutas legacy existentes de páginas `.aspx`/code-behind `.vb`, servicios ASMX, modelos, servicios, repositorios, scripts y estilos vinculados a Notas de Workflow.

## Límite de módulo

El alcance actual termina en el consumidor Centro de Trabajo y en las rutas de Notas bajo `workflow/`. Los inventarios históricos pueden describir reutilizaciones de Notas fuera de ese módulo, pero son contexto de diagnóstico: no autorizan cambios, migraciones, pruebas, retiros ni E2E sobre otros módulos. Cualquier extensión futura requiere un ticket, propuesta OpenSpec y prompts propios.

## Pruebas obligatorias

Cada fase debe ejecutar las pruebas focales o QA manual reproducible que correspondan, compilar con MSBuild o `dotnet` cuando afecte código VB/proyecto y registrar comandos, resultados y evidencia saneada. Toda fase que introduzca o modifique un recorrido verificable integra su E2E en el mismo cambio; la fase 06 consolida trazabilidad y regresión, pero no es una entrega E2E independiente.

Cuando una fase modifique una operación o interfaz con estado, debe definir y validar estados controlados de carga, vacío, éxito, error, conflicto y recuperación, sin filtrar información sensible ni dejar controles bloqueados.

## Documentación técnica

Ubica y actualiza la documentación existente bajo `Doc/Actualizacion/workflow/Notas/`, incluida Exploración, requerimientos, propuesta OpenSpec y matriz de pruebas cuando corresponda. Si falta una ruta documental necesaria, regístrala como decisión pendiente; no crees documentación en la raíz.

## Entregable final

Cada fase entrega el cambio mínimo implementado, rutas modificadas, decisiones, pruebas y comandos ejecutados, resultados, evidencia saneada, riesgos, deuda pendiente y rollback aplicable.

## Orden de uso

1. `01-fundacion-backend-y-contratos.md`
2. `02-lectura-listado-y-contador.md`
3. `03-escrituras-transaccionales.md`
4. `04-centro-trabajo-y-ui.md`
5. `05-estabilizacion-consumidor-workflow.md`
6. `06-verificacion-y-retiro-legacy.md`

Los prompts 04 y 05 requieren que los contratos de lectura y escritura de las fases anteriores estén aprobados. No se habilita el consumidor moderno de Workflow hasta completar su matriz de verificación.

Las E2E reales que correspondan a cada cambio son obligatorias para cerrar su alcance. Deben ejecutarse únicamente cuando exista autorización explícita de ambiente, cuentas y tareas descartables, y reutilizan el arnés de `tools/e2e`; no se sustituyen por mocks, scripts ad hoc ni un login alterno. Su recorrido se limita a pantallas y contratos del módulo `workflow/`.

Requisito E2E completo: la E2E es parte integral del mismo cambio y de su cierre, no una tarea o entrega independiente. Reutiliza exclusivamente `tools/e2e`, su autenticación, configuración, validadores, evidencias y utilidades; no crear login, arnés, proyecto Playwright, configuración ni `.env` paralelos. Antes de una E2E autenticada lee `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`; ejecútala solo con ambiente, cuentas y datos o tareas descartables expresamente autorizados. Usa secretos efímeros y no exponer, imprimir ni persistir credenciales, cookies, tokens ni cadenas de conexión; las verificaciones son solo `SELECT` y toda evidencia saneada. Cubre, cuando aplique, autorización y control de acceso, lectura sin mutación, escrituras autorizadas, concurrencia y regresión. Respeta feature flags, gates, usuarios, grupos y seguridad sin habilitarlos arbitrariamente; la implementación no se considera terminada sin validación autorizada y, si falta una precondición, registra bloqueo explícito sin mocks, simulaciones ni evidencia ficticia.

Todo prompt que modifique un recorrido E2E debe incorporar y cumplir [bloque-e2e-integrado-en-modernizacion.md](bloque-e2e-integrado-en-modernizacion.md) dentro de su propio alcance. El bloque incorpora la E2E al mismo cambio, revisión y criterio de cierre; no se planifica ni entrega como tarea técnica independiente.

## Fuentes obligatorias

Antes de actuar, el agente debe leer completamente:

- `AGENTS.md`.
- `Doc/Actualizacion/workflow/Notas/Exploracion/diagnostico-modernizacion-notas-workflow.md`.
- `Doc/Actualizacion/workflow/Notas/Exploracion/modelo-requerimientos-modernizacion-notas-workflow.md`.
- `Doc/Actualizacion/workflow/Notas/Exploracion/modelo-ui-notas-workflow-moderno.html`, cuando la fase afecte interfaz.
- El prompt de esta carpeta que corresponda a la fase en ejecución.
- `tools/e2e/AGENT-RUNBOOK.md`, antes de diseñar o ejecutar cualquier E2E autenticado o real.

## Instrucciones comunes para copiar al inicio de cada ejecución

```text
Actúa como arquitecto y desarrollador senior de DocuArchi. Implementa únicamente el alcance de esta fase de modernización de Notas de Workflow.

Primero, lee AGENTS.md y los tres artefactos de Exploración de Notas. Revisa el árbol de trabajo y conserva cualquier cambio ajeno. No uses ni modifiques los cambios OpenSpec activos que no correspondan a Notas; crea o continúa una propuesta OpenSpec dedicada a esta modernización.

Trabaja como monolito modular: transporte ASMX/API → gate de contexto → servicio de aplicación → interfaces/repositorios → MySQL. No copies ni envuelvas Class_anotacion_tarea como la nueva implementación. Mantén WebForms solo como adaptador temporal. No modifiques rutas, páginas, scripts, consumidores ni pruebas de módulos ajenos a `workflow/`.

Las E2E reales aplicables a esta fase son obligatorias y deben reutilizar `tools/e2e`. No las ejecutes antes de recibir autorización explícita de ambiente y cuentas; para escrituras, requiere además autorización explícita para cada tarea descartable. No ejecutes carga ni habilites gates. No expongas ni registres credenciales, cookies ni cadenas de conexión. `WorkflowCentroTrabajoModernActive` debe permanecer `false` y sin usuarios ni grupos de piloto al terminar.

Usa idTarea explícito en cada contrato. La sesión solo resuelve identidad y contexto; nunca es la fuente de la tarea objetivo. Usa SQL parametrizado, DTOs tipados, resultados funcionales seguros y recursos liberados determinísticamente. Entrega cambios mínimos, pruebas proporcionales y un resumen con archivos modificados, verificación ejecutada, riesgos y decisiones pendientes.
```

## Criterio transversal de finalización

Una fase solo se considera completa cuando:

- Su alcance no invade fases posteriores ni módulos ajenos a `workflow/`.
- Los requerimientos y criterios de aceptación que le aplican tienen prueba automatizada o una evidencia explícita de por qué aún no procede.
- No introduce doble escritura, doble acción visual ni dependencia de `Session("ID_TAREA_SELECCIONDA")` en el flujo moderno.
- La activación permanece reversible y deshabilitada por defecto.
- No se cambia semántica de borrado, visibilidad histórica, supervisión o retención sin decisión de negocio registrada.
- Toda fase con recorrido E2E aplicable integra la prueba con el arnés existente y aporta evidencia saneada de la validación autorizada, o registra como bloqueo la falta de autorización explícita sin sustituirla por una prueba ficticia.
