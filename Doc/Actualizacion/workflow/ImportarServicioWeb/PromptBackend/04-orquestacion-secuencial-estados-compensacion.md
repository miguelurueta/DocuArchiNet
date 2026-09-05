# Prompt backend 04 — Orquestación secuencial, estados y compensación

Implementa la ejecución servidor de las intenciones creadas por el Prompt backend 03 y utiliza el cliente externo del Prompt backend 02.

Publica `ExecuteImportIntent` y `GetImportIntent`. `ImportServiceOrchestrator` es el único ejecutor moderno; el navegador y `JSProgresBar` solo solicitan ejecución y presentan estado confirmado.

## Objetivo

Trasladar al servidor la coreografía que hoy coordina el navegador, conservando el orden funcional demostrado y registrando cada fase alcanzada.

## Rutas canónicas de implementación

```txt
Services/Workflow/ImportarServicioWeb/
├── ImportServiceOrchestrator.vb
├── ImportIntentStateMachine.vb
├── ImportItemResultFactory.vb
└── ImportDocumentStoragePort.vb

Infrastructure/Workflow/ImportarServicioWeb/Storage/
└── LegacyImportDocumentStorageAdapter.vb

Tests/
├── importar-servicio-web-orchestrator.test.cjs
├── importar-servicio-web-state-machine.test.cjs
├── importar-servicio-web-failure-injection.test.cjs
└── importar-servicio-web-storage-adapter.test.cjs
```

- `ImportServiceOrchestrator.vb` es el único coordinador moderno y depende de interfaces de `Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebInterfaces.vb`.
- `ImportIntentStateMachine.vb` contiene transiciones puras; no accede a sesión, SQL, ASMX ni almacenamiento.
- `LegacyImportDocumentStorageAdapter.vb` es el único punto nuevo autorizado para invocar `AlmacenaDocumentoTareaWorkflow(...)` como caja negra.
- `ImportDocumentStoragePort.vb` adapta el comando normalizado al puerto sin exponer `ClassAlmacenamiento` al resto del caso de uso.
- Agregar nuevos `.vb` al `.vbproj`; no mover ni editar archivos legacy.

Está prohibido implementar la orquestación en `webservice/WebService_integracion_sii.asmx.vb`, `js/java_general/JSProgresBar.js`, `workflow/ClassAlmacenamiento.vb`, `Integracionccv/` o `ServiciosIntegracion/`.

## Ruta documental obligatoria

```txt
docs/Architecture/Workflow/ImportarServicioWeb/SCRUMCORE-000-orquestacion-estados-compensacion/
```

Sustituir `SCRUMCORE-000` por el ticket real. Crear el paquete `00-Indice.md` a `07-Metadata.md` y `Diagramas/`. Documentar explícitamente secuencia, máquina de estados, puntos de fallo, transacciones locales y la frontera inmutable con almacenamiento.

## Investigación obligatoria

- Confirmar el orden válido entre expediente, almacenamiento, índices y caché.
- Determinar las fronteras de transacción local y qué compensaciones son funcionalmente permitidas.
- Identificar los efectos existentes que ya son idempotentes y los que requieren protección adicional.

## Implementa

- `ImportServiceOrchestrator` con autorización, resolución de proveedor, carga de intención, ejecución secuencial y persistencia de transiciones.
- Máquina de estados con fases equivalentes a Validada, RecursoObtenido, ExpedientePreparado, DocumentoAlmacenado, ÍndicesActualizados, CachéActualizado, Reconciliada y Completada.
- Estados alternos RequiereDecision, FallidaAntesDePersistir, ResultadoIncierto, Parcial y Detenida.
- Resultado tipado por elemento con fase alcanzada, `persistenceKnown`, código seguro, capacidad de reintento y correlación.
- Estados persistidos compatibles con el mapeo normativo frontend/backend; cualquier estado nuevo exige versionar el contrato y actualizar sus fixtures.
- Detención cooperativa: no inicia nuevos elementos, no revierte confirmados y deja la intención reconciliable.
- Compensaciones únicamente donde estén aprobadas y probadas; en los demás casos registrar resultado parcial o incierto.
- Auditoría de transiciones sin secretos ni payload externo completo.
- Un adaptador de almacenamiento de la nueva orquestación que prepare los argumentos esperados y reutilice `AlmacenaDocumentoTareaWorkflow(...)` sin modificarla.

## Restricciones

- La orquestación nueva coexiste en paralelo con la coreografía vigente; no reemplaza ni modifica el flujo legacy.
- `AlmacenaDocumentoTareaWorkflow(...)` es una caja negra compartida: está prohibido cambiar su firma, lógica interna, efectos o consumidores existentes.
- Cualquier diferencia entre el contrato moderno y la función de almacenamiento se resuelve en el adaptador nuevo, nunca dentro de la función existente.
- Procesar elementos secuencialmente en esta primera modernización.
- No prometer una transacción distribuida sobre proveedor, base de datos y almacenamiento documental.
- No marcar como libre de persistencia un fallo cuya fase no pueda demostrarse.
- No reintentar automáticamente efectos mutadores sin clave idempotente y estado conocido.
- No usar la sesión como destino mutable durante la ejecución.

## Aceptación

- Un fallo inyectado antes y después de cada fase deja un estado coherente y consultable.
- Una detención conserva elementos confirmados y clasifica los pendientes.
- Un resultado incierto obliga a reconciliar antes de permitir reintento.
- Cada transición inválida es rechazada y auditada.
- No hay ejecución paralela de elementos.
- Una prueba de caracterización demuestra que el adaptador nuevo invoca la función de almacenamiento existente con los datos esperados, sin cambios en dicha función.

## Trazabilidad

Exploración backend: secciones 4, 9, 12, 13, 17 y 20; preguntas abiertas 4, 5, 6, 7 y 8.

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
Leer contratos B01, persistencia B03, `workflow/ClassAlmacenamiento.vb` y el método `AlmacenaDocumentoTareaWorkflow(...)` únicamente para construir el adaptador, además de los adaptadores paralelos de `Infrastructure/Workflow/Terminar/` como referencia estructural. Está prohibido modificar cualquiera de esos archivos existentes.

## Pruebas obligatorias
Ejecutar pruebas unitarias/focales, build/tsc segun impacto y E2E con Playwright cuando el flujo lo requiera; registrar comandos y resultados.

## Documentacion tecnica
Actualizar exclusivamente el paquete de **Ruta documental obligatoria**, incluidos diagramas de secuencia/estado y evidencia del diff que demuestra que almacenamiento y recorridos legacy no cambiaron.

## Entregable final
Entregar codigo, pruebas, documentacion, diagramas y evidencia coherente con lo realmente implementado.

Exigir `npm run build` o `tsc` segun impacto y registrar el resultado.

Exigir pruebas unitarias/focales con Vitest o Testing Library segun el alcance.
