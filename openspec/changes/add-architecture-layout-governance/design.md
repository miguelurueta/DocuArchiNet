## Context

OPSXJ ya persiste un manifiesto `opsxj-governance.json` v3, perfiles de arquitectura y tecnología, refinement trazable y una validación local. Esos controles no contienen una taxonomía física de capas ni pueden comparar las rutas declaradas con los archivos afectados. Véase `proposal.md` para la motivación y la especificación delta de `legacy-opsxj-governance` para el comportamiento requerido.

La aplicación conserva directorios legacy que no pueden someterse retroactivamente a una organización nueva. La política aplica a cambios nuevos que declaren arquitectura modular y no modifica el comportamiento Web Forms, VB.NET ni los flujos de negocio.

## Goals / Non-Goals

**Goals:**

- Declarar una convención de rutas versionada, legible por personas y ejecutable por OPSXJ.
- Extender el manifiesto de gobierno existente con el contrato arquitectónico del cambio, sin crear una fuente de verdad duplicada.
- Detectar incoherencias de rutas y límites de capas en creación, refinement, revisión técnica y validación local.
- Exigir excepciones mínimas, expirables y auditables cuando una desviación esté justificada.
- Mantener compatibilidad para manifiestos v2/v3 y cambios que no pertenecen a una modernización modular.

**Non-Goals:**

- Reorganizar el código legacy existente o cambiar namespaces VB.NET de forma masiva.
- Inferir reglas de negocio a partir del nombre de una carpeta.
- Crear un `GenericRepository`, una abstracción genérica de dominio o un adaptador legacy común obligatorio para todos los módulos.
- Convertir OPSXJ en un sustituto de la compilación, pruebas funcionales o revisión humana de código.

## Decisions

### D-01 — Catálogo de estructura como fuente ejecutable

Se creará `tools/opsxj/scripts/lib/architectureLayoutCatalog.js` con versión, patrones y restricciones. La documentación humana se generará o se comprobará contra el mismo catálogo en `Doc/Arquitectura/convenciones/estructura-modular.md`.

El catálogo versión 1 define:

```text
DTOs/{module}/{useCase}
Services/{module}/{useCase}
Modelo/{module}/{useCase}
Domain/Shared
Infrastructure/Shared/Data
Infrastructure/{module}/{useCase}
Infrastructure/Repositories/{module}
```

También define raíces retiradas (`workflow/modern`) y límites mínimos: Application no usa dependencias Web Forms; `Shared` no conoce símbolos del módulo; los repositorios son propios del módulo; y solo las fronteras legacy declaradas pueden llamar símbolos legacy autorizados.

Se descarta codificar estos patrones dispersos en cada comando porque degradaría la consistencia y haría imposible versionar una excepción.

### D-02 — Sección `architecture` en el manifiesto de gobierno v4

No se creará un segundo archivo de manifiesto. Los cambios nuevos con arquitectura modular recibirán `architecture` dentro de `opsxj-governance.json` versión 4:

```json
{
  "architecture": {
    "layoutVersion": 1,
    "scope": "modular",
    "module": "Workflow",
    "useCase": "Terminar",
    "allowedRoots": [
      "DTOs/Workflow/Terminar",
      "Services/Workflow/Terminar",
      "Modelo/Workflow/Terminar",
      "Infrastructure/Workflow/Terminar",
      "Infrastructure/Repositories/Workflow"
    ],
    "legacyBoundaries": [
      {
        "path": "Infrastructure/Workflow/Terminar/WorkflowLegacyExecutorAdapter.vb",
        "symbols": ["Terminar_Tarea_Workflow", "Cambia_Estado"]
      }
    ],
    "exceptions": []
  }
}
```

`--module` y `--use-case` se validan como par. Los cambios que no producen código modular declaran `scope: "not_applicable"` y un motivo; el perfil `enterprise-legacy-modernization` exige `scope: "modular"` salvo `docs_only`. Esto reduce falsos positivos sin dejar que un cambio de modernización omita su arquitectura.

Se descarta un manifiesto independiente porque duplicaría identidad, estado de gobernanza y trazabilidad de OpenSpec.

### D-03 — Validación por manifiesto y archivos afectados

`architectureGovernanceService.js` validará el esquema del manifiesto y los archivos afectados por el cambio. El conjunto se obtiene de las rutas declaradas y de los archivos modificados que Git pueda identificar; archivos Markdown, evidencia y artefactos OpenSpec se excluyen del análisis de capas.

La validación aplica estas comprobaciones:

- ubicación de archivos por clase declarada;
- ausencia de raíces retiradas;
- dependencias Web Forms prohibidas en DTOs, modelos y servicios;
- ausencia de símbolos específicos del módulo dentro de `Domain/Shared` e `Infrastructure/Shared/Data`;
- llamadas a símbolos legacy únicamente desde una `legacyBoundary` declarada;
- existencia, datos obligatorios y vigencia de las excepciones.

La validación devuelve checks estructurados `PASS`, `FAIL` o `NOT_APPLICABLE`, para que `opsxj:validate` y los reportes JSON puedan consumirla. Un cambio v2/v3 sin `architecture` devuelve `NOT_APPLICABLE`, no un bloqueo.

Se descarta un escaneo completo del repositorio, porque convertiría deuda histórica fuera del cambio en falsos bloqueos.

### D-04 — Excepciones explícitas y de alcance mínimo

Cada excepción contiene `rule`, `paths`, `reason`, `approvedBy` y `expiresOn` en formato ISO `YYYY-MM-DD`. Una excepción vence automáticamente después de esa fecha, no puede cubrir raíces completas y solo suprime el check y archivo declarados.

La excepción se revisa en refinement y validación; no se aceptan comodines, aprobación ausente ni expiración indefinida. Se descarta una lista global de exclusiones porque permitiría ocultar desviaciones permanentes.

### D-05 — Integración secuencial en OPSXJ

- `opsxj:orchestrate:new` y su alias aceptan `--module`, `--use-case` y `--architecture-scope`; validan la entrada y escriben el manifiesto v4.
- `opsxj:orchestrate:refine` verifica que el manifiesto, `refinement.md`, diseño, especificación y tareas expresen las mismas rutas y frontera legacy.
- `opsxj:technical-review` / `opsxj:prompt-review` recibe el manifiesto si existe y emite hallazgos bloqueantes por rutas ambiguas, rutas retiradas o capas incompatibles.
- `opsxj:validate` agrega los checks de arquitectura a su resultado existente antes de permitir publicación o archivo.
- `tools/validation/Verify-ArchitectureStructure.ps1` ofrece un gate invocable desde CI para cambios con manifiesto v4; OPSXJ lo invoca o consume su resultado sin requerir una UI o red.

Se conservan nombres de comando existentes y compatibilidad de argumentos. No se añade una operación remota: el gobierno sigue siendo local hasta el flujo explícito de archive/close.

## Risks / Trade-offs

- [Catálogo demasiado rígido] → el alcance `not_applicable` y las excepciones acotadas cubren cambios que no producen capas modulares.
- [Falsos positivos por código legacy] → se inspeccionan archivos afectados y rutas declaradas, no todo el repositorio.
- [Manifiestos ambiguos] → pares obligatorios módulo/caso de uso, rutas derivadas por catálogo y validación de esquema.
- [Deuda de excepciones] → vencimiento obligatorio, aprobación identificable y bloqueo automático al expirar.
- [Diferencia entre documentación y código] → refinement y `opsxj:validate` comparan ambos contra el mismo manifiesto.

## Migration Plan

1. Añadir catálogo, servicio de gobierno, esquema de manifiesto v4 y pruebas unitarias de parsing, rutas, excepciones y compatibilidad.
2. Extender CLI, generación de artefactos, refinement, revisión técnica y validación sin cambiar el formato de manifiestos v2/v3.
3. Añadir el validador PowerShell y la convención documental; ejecutar pruebas Vitest y validación focal con manifiestos de ejemplo.
4. Activar el requisito para cambios nuevos que usen `enterprise-legacy-modernization`; mantener cambios históricos como `NOT_APPLICABLE`.
5. Incorporar el comando de validación en CI antes de publicación/archivo. Rollback: retirar la activación del gate para cambios nuevos, conservando el manifiesto y los reportes como evidencia; no afecta runtime de la aplicación.

## Open Questions

No hay preguntas abiertas que alteren el enfoque. La lista inicial de símbolos legacy por módulo se declarará por cambio en `legacyBoundaries`, en lugar de mantener una lista global imposible de gobernar.
