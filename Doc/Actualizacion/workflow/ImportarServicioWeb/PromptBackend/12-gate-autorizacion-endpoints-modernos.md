# Prompt backend 12 — Gate y autorización de endpoints modernos

Actúa como arquitecto y desarrollador senior de ASP.NET WebForms/VB.NET. Refuerza la frontera de seguridad de `WebServiceImportarServicioWebModern` sin modificar la lógica funcional de consulta, preview, preflight, importación, expediente o reconciliación. Crea un cambio OpenSpec independiente.

## Diagnóstico confirmado

`webservice/WebServiceImportarServicioWebModern.asmx.vb` protege actualmente sus operaciones con `FeatureEnabled()`, que solo evalúa `WorkflowCentroTrabajoModernActive`. `Web.config` ya declara `WorkflowCentroTrabajoModernUsers` y `WorkflowCentroTrabajoModernGroups`, pero esa pertenencia no se valida en la frontera de todos los endpoints.

Ocultar o deshabilitar controles en `workflow/Webworkflow.aspx(.vb)` no autoriza una llamada ASMX directa. Este cambio es obligatorio antes de habilitar la ruta moderna en producción.

## Objetivo

Aplicar una política única, denegatoria por defecto y verificable que exija simultáneamente:

1. `WorkflowCentroTrabajoModernActive = true`.
2. Sesión Workflow autenticada y válida.
3. Usuario incluido en `WorkflowCentroTrabajoModernUsers`.
4. Grupo efectivo incluido en `WorkflowCentroTrabajoModernGroups`.

La política debe proteger por igual operaciones de lectura y mutación, sin confiar en datos de identidad enviados por el cliente.

## Superficie obligatoria

Aplicar la misma autorización antes de cualquier composición, consulta externa o efecto en estos métodos reales de `WebServiceImportarServicioWebModern`:

- `ResolveCapabilities(ResolveCapabilitiesRequestDto)`.
- `QueryItems(QueryItemsRequestDto)`.
- `GetPreview(GetPreviewRequestDto)`.
- `PreflightImport(PreflightImportRequestDto)`.
- `CreateImportIntent(CreateImportIntentRequestDto)`.
- `ExecuteImportIntent(ExecuteImportIntentRequestDto)`.
- `GetImportIntent(GetImportIntentRequestDto)`.
- `ReconcileImportIntent(ReconcileImportIntentRequestDto)`.

Incluir también cualquier handler de streaming creado para consumir `DescriptorId`; no basta con proteger `GetPreview`.

## Rutas canónicas de implementación

```txt
Services/Workflow/ImportarServicioWeb/
├── IImportServiceFeatureGate.vb
└── ImportServiceFeatureGate.vb

webservice/
└── WebServiceImportarServicioWebModern.asmx.vb

Tests/
├── importar-servicio-web-endpoint-gate.test.cjs
├── importar-servicio-web-gate-scope.test.cjs
└── importar-servicio-web-gate-no-effects.test.cjs

tools/e2e/tests/
└── importar-servicio-web-modern.spec.cjs
```

Reutilizar, si ya existe, la resolución autoritativa de usuario y grupo Workflow. No crear un segundo modelo de identidad ni consultar pertenencia desde valores enviados por el navegador.

## Ruta documental obligatoria

```txt
Doc/Actualizacion/workflow/ImportarServicioWeb/<TICKET>-gate-autorizacion-endpoints/
```

Documentar matriz de decisiones, configuración, endpoints cubiertos, códigos seguros, pruebas, rollback y evidencia saneada.

## Implementa

- `IImportServiceFeatureGate` con una evaluación explícita sobre el contexto autenticado del servidor.
- Parseo normalizado de allowlists, sin coincidencias parciales, sensible a la semántica real de login/grupo y tolerante únicamente a espacios separadores.
- Denegación segura ante configuración vacía, sesión ausente, identidad ambigua, usuario no autorizado o grupo no autorizado.
- Evaluación centralizada al inicio de cada endpoint y del handler de streaming.
- Separación entre `FEATURE_DISABLED` cuando el booleano está apagado y `FORBIDDEN` cuando la característica está activa pero el sujeto queda fuera del alcance. No revelar cuál usuario, grupo o regla falló.
- Cero llamadas SII, creación de intención, escritura, descarga o composición de servicios cuando la autorización falla.
- Conservación del contexto inmutable de tarea y de todas las validaciones funcionales existentes después de autorizar.

## Reglas de seguridad

- Nunca confiar en usuario, grupo, rol, tarea o autorización incluidos en request, query string, header personalizado o JavaScript.
- No considerar la visibilidad de la UI como control de acceso.
- No convertir listas vacías en acceso global ni agregar comodines implícitos.
- No registrar credenciales, cookies, identificadores de sesión ni configuración sensible.
- No cambiar contratos DTO salvo que sea estrictamente necesario para devolver un código seguro ya compatible.
- No modificar `AlmacenaDocumentoTareaWorkflow(...)`, `ClassAlmacenamiento`, clientes SII, resolución de expediente ni repositorios DOC-67.
- No activar el gate desde código, pruebas o configuración versionada.

## Matriz mínima de aceptación

| Gate | Sesión | Usuario | Grupo | Resultado |
|---|---|---|---|---|
| Apagado | Cualquiera | Cualquiera | Cualquiera | `FEATURE_DISABLED`, sin efectos |
| Encendido | Ausente/inválida | No aplica | No aplica | `FORBIDDEN`, sin efectos |
| Encendido | Válida | Fuera de lista | Autorizado | `FORBIDDEN`, sin efectos |
| Encendido | Válida | Autorizado | Fuera de lista | `FORBIDDEN`, sin efectos |
| Encendido | Válida | Autorizado | Autorizado | Continúa con validaciones funcionales existentes |

Cada fila debe probarse sobre los ocho métodos y el handler de streaming aplicable. La aceptación autorizada no implica que la operación de negocio deba tener éxito; demuestra que solo el sujeto dentro del alcance alcanza sus validaciones posteriores.

## Pruebas automáticas obligatorias

- Pruebas focales deterministas para parseo, coincidencia exacta, listas vacías, mayúsculas/minúsculas según la regla existente y denegación por defecto.
- Pruebas estructurales que inventaríen explícitamente los ocho endpoints requeridos y fallen si alguno omite la política central.
- Pruebas de acceso directo al ASMX, sin depender de que la UI esté visible.
- Verificación de cero efectos y cero llamadas externas en todos los rechazos.
- Regresión del recorrido legacy con gate apagado.
- Compilación compatible con .NET Framework 4.6.1 y ejecución de suites focales existentes de ImportarServicioWeb.

## E2E real obligatoria

- Reutilizar exclusivamente `tools/e2e`, su autenticación, runner, perfiles y restauración del gate; no crear arnés, login, `.env` ni proyecto Playwright paralelo.
- Antes de ejecutar, leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`.
- Requiere autorización explícita para ambiente, cuentas, activación temporal del gate y cualquier recurso descartable.
- Probar una cuenta autorizada y cuentas controladas que fallen por usuario y por grupo, invocando directamente un endpoint de lectura y uno mutador.
- Confirmar mediante evidencia saneada y consultas exclusivamente `SELECT` que los rechazos no generan intención, documento, expediente, vínculo, caché, índice ni llamadas SII.
- Al finalizar, restaurar `WorkflowCentroTrabajoModernActive=false` y vaciar usuarios y grupos, incluso ante fallo.
- Si no existen cuentas controladas o autorización suficiente, registrar el bloqueo y no sustituir la evidencia con mocks ni afirmar que la E2E pasó.

## Aceptación final

- Todos los endpoints modernos y el stream aplican la misma política central antes de cualquier trabajo.
- Solo la combinación booleano activo + sesión válida + usuario autorizado + grupo autorizado atraviesa el gate.
- Las respuestas de rechazo son seguras y no distinguen información aprovechable sobre allowlists.
- La UI y el backend quedan alineados, pero el backend conserva la autoridad aun ante llamadas directas.
- El recorrido legacy permanece intacto y el gate queda apagado por defecto.

## Entregable

Entregar implementación, pruebas focales, compilación, E2E autorizada, documentación técnica y evidencia saneada. No mezclar este cambio con funcionalidad de importación ni declarar habilitación productiva sin completar la matriz de autorización.
