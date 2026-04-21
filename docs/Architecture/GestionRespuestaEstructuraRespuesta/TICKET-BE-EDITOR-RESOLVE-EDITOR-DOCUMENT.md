# PROMPT ARQUITECTÓNICO — Ticket BE (Editor)
# Resolver carga inicial vs documento existente (ResolveEditorDocument)

## Rol esperado

Arquitecto de software senior backend (.NET, C#, ASP.NET Core, Clean Architecture, services, repositorios, contratos API, seguridad por claims, pruebas y documentación técnica).

---

## OBJETIVO

Centralizar en backend la lógica de resolución de contenido del editor:

- Si existe documento → devolver documento (`mode=existing`)
- Si no existe → devolver contenido inicial (`mode=initial`)

Eliminar completamente la lógica duplicada en frontend y reducir llamadas innecesarias.

---

## CONTEXTO EXISTENTE

APIs actuales:

- `GET /api/gestor-documental/editor/initial-content`
- `GET /api/gestor-documental/editor/document/by-context`
- `GET /api/gestor-documental/editor/document/{documentId}`

Problema actual:

- frontend decide qué endpoint llamar
- lógica duplicada en frontend
- múltiples llamadas innecesarias
- reglas de negocio distribuidas

---

## NUEVA API OFICIAL

`GET /api/gestor-documental/editor/document/resolve`

### Query params

- `contextCode` (string, requerido)
- `entityId` (long, requerido)
- `templateDefinitionId` (long, opcional)
- `templateCode` (string, opcional)
- `prefer` (string, opcional) valores:
  - `existing` (default)
  - `initial`

**Regla adicional (anti-regresión / compatibilidad):**

- Si `prefer` llega null/empty/whitespace → tratar como `existing`.
- Si `prefer` tiene un valor distinto a `existing|initial` (case-insensitive) → `400 BadRequest` (no continuar).

---

## CONTRATO DE RESPUESTA

`AppResponses<EditorResolveDocumentResponseDto>`

`EditorResolveDocumentResponseDto`:

- `mode` (string): `existing` | `initial`
- `contextCode` (string)
- `entityId` (long)
- `documentId` (long?)
- `templateDefinitionId` (long)
- `templateCode` (string)
- `html` (string)
- `images` (List)
- `tokensResueltos` (Dictionary<string,string?>?)

**Regla adicional (anti-fricción FE):**

- Mantener `tokensResueltos=null` cuando `mode=existing` (como se define más abajo).
- Mantener contrato `AppResponses` consistente también en `409 Conflict` (success=false, errors[] y message).

---

## REGLAS DE NEGOCIO (CERRADAS / NO AMBIGUAS)

### VALIDACIONES

- `contextCode` null/empty/whitespace → `400`
- `entityId <= 0` → `400`
- `contextCode` inválido o inactivo → `400` (**NO usar 404 en este ticket**)

**Validaciones adicionales recomendadas (anti-regresión):**

- si `templateDefinitionId` viene y `templateDefinitionId <= 0` → `400`
- si `templateCode` viene y es whitespace → tratar como null (o `400`, pero dejar explícito cuál; recomendado: normalizar a null)

---

## RESOLUCIÓN DEFAULT (`prefer=existing`)

1. Consultar documento existente:
   `IServiceSolicitaEditorDocumentByContext`

2. CASOS:

### A. EXISTE documento

Retornar:

- `mode=existing`
- `documentId`
- `html`
- `images`
- `tokensResueltos = null`

### B. NO EXISTE documento

Llamar:

- `IServiceInitialContentEditor`

Retornar:

- `mode=initial`
- `documentId = null`
- `html`
- `tokensResueltos`
- `images = []`

**Nota QT (aclaración para evitar ambigüedad):**

- El objetivo es minimizar llamadas: en `prefer=existing` se consulta primero “existing” y solo si no existe se llama “initial”.

---

## RESOLUCIÓN FORZADA (`prefer=initial`)

### CASOS

#### A. EXISTE documento

- SI contexto NO permite múltiples → retornar `409 Conflict`
- SI contexto SÍ permite múltiples:
  - retornar `initial` (crear nuevo)
  - `mode=initial`

#### B. NO EXISTE documento

- retornar `initial` normal

---

## REGLA CRÍTICA — MÚLTIPLES DOCUMENTOS

Si existen múltiples documentos y NO hay criterio determinístico formal:

- retornar `409 Conflict`

**NO implementar lógica de “último activo” en este ticket.**

**Regla adicional (para asegurar que este 409 sea alcanzable):**

- La implementación debe poder **detectar** el caso “múltiples” (por ejemplo, el servicio by-context debe exponer lista/contador o un resultado que permita identificar ambigüedad). Si hoy solo retorna uno, el ticket debe incluir el ajuste mínimo para detectar ambigüedad sin introducir heurísticas.

---

## PRIORIDAD DE PLANTILLA (OVERRIDE)

Orden de resolución:

1. `templateDefinitionId` (mayor prioridad)
2. `templateCode`
3. reglas del contexto (default)

---

## IMPLEMENTACIÓN — CONTROLLER

Ubicación:
`DocuArchi.Api/Controllers/GestorDocumental/Editor/`

Reglas:

- usar controller existente del módulo editor (NO crear nuevo si ya existe uno)
- agregar acción:
  - `[HttpGet("document/resolve")]`

Inyección:

- `IClaimValidationService`
- `IServiceResolveEditorDocument`

Flujo:

1. validar claim `defaulalias`
2. validar parámetros
3. llamar service
4. retornar `Ok` / `BadRequest` / `Conflict` según resultado

PROHIBIDO:

- lógica de negocio
- acceso a repositorio
- duplicar validaciones del service

---

## IMPLEMENTACIÓN — SERVICE

Interface:
`IServiceResolveEditorDocument`

Clase:
`ServiceResolveEditorDocument`

Método:

```csharp
Task<AppResponses<EditorResolveDocumentResponseDto>> ResolveAsync(
    string contextCode,
    long entityId,
    long? templateDefinitionId,
    string? templateCode,
    string prefer,
    string alias
);
```

Reglas:

- orquestar servicios existentes
- no duplicar lógica de repositorios
- no resolver claims
- try/catch obligatorio
- retornar `AppResponses` consistente

---

## SEGURIDAD

- validar claim `defaulalias` en controller
- service recibe alias ya validado
- NO reinterpretar identidad en service

---

## PRUEBAS OBLIGATORIAS

### UNITARIAS

- `prefer=existing` retorna documento
- no existe retorna initial
- `prefer=initial` + existe + no permitido → `409`
- `prefer=initial` + permitido → `initial`
- `contextCode` inválido → error
- `entityId` inválido → error
- múltiples documentos → `409`
- `prefer` inválido → `400` (regla adicional)
- `templateDefinitionId <= 0` → `400` (regla adicional)

### INTEGRACIÓN

- flujo completo controller → service → repos
- datos reales coherentes
- contratos correctos

### QT / PUNTA A PUNTA

- llamada única desde FE resuelve correctamente el modo (sin doble orquestación en FE)
- frontend recibe datos consistentes
- no hay doble llamada desde FE
- tiempos de respuesta correctos
- consistencia de html renderizable
- comportamiento correcto en conflictos (`409`)

### REGRESIÓN

- no romper endpoints existentes
- no alterar contratos existentes

---

## VALIDACIÓN SOLID (OBLIGATORIA)

- SRP: controller/service/repos separados
- OCP: extensible sin romper contrato
- LSP: interfaces sustituibles
- ISP: interfaces específicas
- DIP: dependencias por interfaces

Documentar resultados.

---

## DEUDA TÉCNICA (OBLIGATORIO)

Evaluar y documentar:

- duplicación lógica anterior en frontend
- ambigüedad previa de múltiples documentos
- falta de contrato unificado
- posibles mejoras futuras

Clasificar:

- crítica
- media
- menor

---

## DOCUMENTACIÓN OBLIGATORIA

Ruta:
`D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\Docs\GestorDocumental\Editor\`

Archivos:

1. `SCRUM-XXX-Integracion-Frontend.md`
   - endpoint
   - parámetros
   - response
   - ejemplos
   - manejo de `mode`

2. `SCRUM-XXX-Arquitectura.md`
   - diagramas:
     - clases
     - secuencia
     - casos de uso
     - estado
   - decisiones arquitectónicas
   - SOLID

3. `SCRUM-XXX-Implementacion-Detallada.md`
   - paso a paso técnico
   - archivos creados
   - lógica aplicada
   - decisiones

4. `SCRUM-XXX-Pruebas.md`
   - unitarias
   - integración
   - QT
   - cobertura

5. METADATA DEL TICKET
   - autor
   - fecha
   - contexto

---

## CRITERIOS DE ACEPTACIÓN

- endpoint único funcional
- elimina lógica frontend duplicada
- maneja existing vs initial correctamente
- maneja conflictos (`409`)
- no hay ambigüedad en múltiples documentos
- pruebas completas
- documentación completa
- arquitectura respetada

