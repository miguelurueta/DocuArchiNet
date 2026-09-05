# Prompt backend 03 — Preflight, intención persistida e idempotencia

Implementa la preparación autoritativa de una importación y su identidad persistente sobre los contratos de los prompts backend 01 y 02.

Publica `PreflightImport` y `CreateImportIntent` con los DTO, códigos e invariantes definidos en el contrato compartido.

## Objetivo

Separar la validación previa de los efectos y crear una intención inmutable e idempotente para colecciones de uno o varios elementos.

## Rutas canónicas de implementación

```txt
Services/Workflow/ImportarServicioWeb/
├── ServicioPreflightImportacion.vb
└── ServicioIntencionImportacion.vb

Infrastructure/Repositories/Workflow/ImportarServicioWeb/
└── MySqlImportIntentRepository.vb

Infrastructure/Workflow/ImportarServicioWeb/
└── MySqlImportIntentConcurrencyGuard.vb

Tests/
├── importar-servicio-web-preflight.test.cjs
├── importar-servicio-web-intent-idempotency.test.cjs
└── importar-servicio-web-intent-concurrency.test.cjs

Tests/Fixtures/Workflow/ImportarServicioWeb/intents-v1/
├── single-item-intent.json
├── multiple-items-intent.json
├── repeated-equivalent-intent.json
└── conflicting-intent.json
```

- Extender contratos únicamente en los archivos canónicos `DTOs/Workflow/ImportarServicioWeb/ImportarServicioWebDtos.vb`, `Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebModels.vb` e `ImportarServicioWebInterfaces.vb` creados por Backend 01.
- La implementación MySQL pertenece a `Infrastructure/Repositories`; servicios y modelos no contienen SQL.
- Cualquier script de esquema aprobado debe ubicarse en el paquete documental del ticket bajo `Sql/`, con aplicación manual y rollback documentados; no se ejecuta automáticamente.
- Agregar nuevos `.vb` al `.vbproj` sin modificar implementaciones existentes.

No crear intención o preflight en ASMX, `App_Code`, `Session`, `ClassRaSiiCahcheInscripcion`, `ClassAlmacenamiento` o tablas/cachés legacy existentes.

## Ruta documental obligatoria

```txt
docs/Architecture/Workflow/ImportarServicioWeb/SCRUMCORE-000-preflight-intencion-idempotencia/
```

Sustituir `SCRUMCORE-000` por el ticket real. Crear `00-Indice.md` a `07-Metadata.md`, `Diagramas/` y, solo si existe un cambio persistente aprobado, `Sql/` con script versionado, precondiciones y rollback.

## Investigación obligatoria

- Confirmar la identidad externa canónica de una inscripción SII.
- Precisar si el caché pertenece al radicado, a la intención o al elemento.
- Inventariar tablas, archivos, transacciones locales y auditoría afectadas por cada fase.
- Diseñar la migración persistente y su rollback antes de aplicarla.

## Implementa

- Preflight sin mutación que revalide usuario, permiso, tarea, ruta, proveedor, selección, tipología y destino.
- Plan tipado de efectos y requisitos, independiente de la cardinalidad de la colección.
- Intención persistida con `operationId`, contexto original, selección, requisitos, versión, estado, fechas y correlación.
- Elementos de intención con proveedor, clave externa canónica, tarea destino y estado propio.
- Restricción de unicidad e idempotencia aplicada en servidor y, cuando corresponda, en almacenamiento.
- Reutilización segura de una intención ante solicitudes repetidas equivalentes y conflicto explícito ante payload incompatible.
- Repositorios parametrizados; ninguna consulta nueva concatena valores funcionales.

## Restricciones

- Crear persistencia y endpoints nuevos en paralelo; no reemplazar ni alterar los endpoints mutadores, cachés o tablas del recorrido vigente.
- No modificar ni invocar `AlmacenaDocumentoTareaWorkflow(...)` durante el preflight.
- Preflight no crea expediente, vínculo, índice, caché ni documento.
- No usar una comprobación del cliente como garantía contra carreras.
- No obtener contexto general tomando implícitamente el primer elemento.
- No guardar tipología de la operación moderna en `Session("DG_LISTA_CHEQUEO")`.
- No aplicar migraciones ni escrituras de ambiente sin autorización y plan aprobado.

## Aceptación

- Individual y múltiple utilizan el mismo contrato y difieren solo en cardinalidad.
- Dos solicitudes concurrentes del mismo elemento no crean dos intenciones ejecutables.
- Repetir la misma solicitud devuelve la intención existente o un resultado idempotente definido.
- Un cambio de tarea, usuario o requisitos produce conflicto, no reutilización silenciosa.
- Las pruebas cubren carreras, unicidad, SQL parametrizado y preflight libre de efectos.

## Trazabilidad

Exploración backend: secciones 9, 10, 11 y 18; hallazgos B-07, B-08 y B-09; preguntas abiertas 3, 4, 5 y 8.

## Correcciones opsxj:prompt-review

Estas reglas fueron agregadas desde `opsxj:prompt-review` para cubrir hallazgos estructurales corregibles. Deben ajustarse al contexto real del ticket antes de enviar a implementacion.

## Rol esperado
Definir el rol tecnico esperado para ejecutar el ticket.

## Objetivo
Describir el objetivo funcional y tecnico verificable.

## Restricciones criticas
- No introducir cambios fuera del alcance declarado.
- No romper comportamiento existente ni contratos publicos.

## Criterios de aceptacion
- El comportamiento implementado cumple el flujo esperado y queda validado con evidencia.

## Contexto obligatorio
Leer los contratos canónicos de Backend 01, `Domain/Shared/ContextoModulo.vb`, `Infrastructure/Shared/Data/AdoNetDataInfrastructure.vb`, `Infrastructure/Shared/Data/ModuleConnectionFactory.vb` y repositorios modernos de `Infrastructure/Repositories/Workflow/` como referencia. No modificar cachés, repositorios o tablas legacy.

## Pruebas obligatorias
Ejecutar pruebas unitarias/focales, build/tsc segun impacto y E2E con Playwright cuando el flujo lo requiera; registrar comandos y resultados.

## Documentacion tecnica
Actualizar exclusivamente el paquete de **Ruta documental obligatoria**; documentar modelo persistente, unicidad, transacciones locales, SQL parametrizado, rollback y pruebas de carreras.

## Entregable final
Entregar codigo, pruebas, documentacion, diagramas y evidencia coherente con lo realmente implementado.

## Requisitos positivos
- Implementar el comportamiento esperado con contratos tipados y responsabilidades claras.
- Mantener la integracion sobre los puntos de extension existentes del repo.
- Dejar evidencia de pruebas y documentacion tecnica actualizada.

Exigir `npm run build` o `tsc` segun impacto y registrar el resultado.

Exigir pruebas unitarias/focales con Vitest o Testing Library segun el alcance.

Cuando el ticket afecte un flujo completo de usuario, navegacion, integracion entre vistas, persistencia de estado u operacion transaccional, exigir E2E real con Playwright; si no aplica, documentar justificacion formal y evidencia manual.
