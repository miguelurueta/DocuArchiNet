# DOC-71 — Importación SII sin expediente

## Rol esperado

Arquitecto y desarrollador backend senior de WebForms/VB.NET, .NET Framework 4.6.1 y MySQL 5.1,
responsable de contratos, orquestación, persistencia, reconciliación y evidencia segura de
`ImportarServicioWeb`.

## Contexto

La ruta moderna confundía la autorización para crear un expediente con la posibilidad de importar un
documento. DOC-71 separa ambos comportamientos y conserva sin cambios el recorrido legacy.

## Objetivo

Permitir que los trámites configurados con `util_Estado_Crea_ExpedienteSII=0` almacenen y actualicen
índices documentales sin producir efectos de expediente, preservando íntegramente DOC-67 cuando el valor
es `1`.

## Requisitos positivos

- Resolver el modo exclusivamente desde el trámite confiable e incluirlo en el fingerprint.
- Almacenar documentos y actualizar los campos documentales disponibles en ambas ramas.
- Exponer `NoAplica` para efectos de expediente deliberadamente omitidos.
- No bloquear ni borrar valores existentes cuando SII omita NIT/cédula, razón social o matrícula.
- Completar únicamente después de confirmar todos los efectos aplicables.

## Restricciones críticas

- No modificar funciones legacy ni `AlmacenaDocumentoTareaWorkflow(...)`.
- No crear, buscar, vincular o cachear expedientes en modo `SinExpediente`.
- Consultar siempre el expediente SII para construir índices; no usar propietario u otra identidad como fallback silencioso.
- No reconstruir, migrar o refoliar históricos.
- No registrar respuestas SII crudas, datos personales, credenciales, tokens, cookies o conexiones.
- Mantener procesamiento secuencial, MySQL 5.1 y .NET Framework 4.6.1.

## Alcance

`util_Estado_Crea_ExpedienteSII` es la autoridad del modo de importación. El valor `0` selecciona
`SinExpediente`; el valor `1` selecciona `GestionarExpediente`. El cliente no envía ni modifica el
modo. La implementación moderna permanece detrás del gate existente y no modifica la ruta legacy ni
`AlmacenaDocumentoTareaWorkflow(...)`.

## Arquitectura y secuencia

El repositorio de configuración traduce la bandera a `ModoExpedienteImportacion`. El preflight incorpora
el modo en `CanonicalValue()` y, por tanto, en el fingerprint. La creación de intención vuelve a resolver
la configuración y rechaza una huella obsoleta antes de persistir.

Ambas ramas consultan primero el expediente SII: MERCANTIL/ESAL usan `consultarExpedienteMercantil` por
matrícula y RUP usa `consultarExpedienteProponente` por proponente. En `GestionarExpediente` se conserva
DOC-67: consultar caché, buscar o crear con guarda tardía, almacenar, vincular, actualizar índices y
registrar caché. En `SinExpediente`, el coordinador bifurca después de resolver los datos para índices,
persiste el plan sin `ExpedientId`, almacena el sello, descubre todo el universo documental por `ENLASE`
y actualiza sus índices por `ID` + `ENLASE`. No escribe `ID_EXPEDIENTE`, relación ni caché de expediente.

La actualización sin expediente filtra los campos contra la estructura física permitida, aplica longitud
y tipo compatibles con MySQL 5.1, exige una sola fila afectada y relee los valores antes de confirmar.
NIT/cédula y razón social pueden no venir incluso en la consulta de expediente SII: su ausencia no debe
borrar con vacío valores ya persistidos ni bloquear cuando la identidad consultable sí fue resuelta. Solo
se actualizan los campos efectivamente suministrados y válidos.

## Estados y finalización

| Efecto | `SinExpediente` | `GestionarExpediente` |
| --- | --- | --- |
| Almacenamiento documental | `Confirmado` | `Confirmado` |
| Resolución/destino de expediente | `NoAplica` | `Confirmado` o fallo explícito |
| Relación documento-expediente | `NoAplica` | `Confirmado` o fallo explícito |
| Índices documentales del gabinete | `Confirmado` | `Confirmado` |
| Caché e índices exclusivos de expediente | `NoAplica` | `Confirmado` o fallo explícito |

Un item sin expediente solo alcanza `Reconciliada` y `Completada` después de confirmar almacenamiento e
índices documentales. `NoAplica` se serializa por nombre, se relee como enum y nunca se transforma en
evidencia física. La consulta no publica un expediente creado, encontrado o vinculado en esa rama.

## Contratos

El contrato es aditivo. `DestinationMode` admite `WithoutExpedient`; el plan conserva
`DOCUMENT_STORAGE` y `DOCUMENT_INDEXES` como `Planned`, y publica `EXPEDIENT_RESOLUTION`,
`DOCUMENT_LINK` y `LINK_CACHE` como `NotApplicable`. Las respuestas de efectos admiten `NoAplica` en
los estados de expediente, relación, caché e índices exclusivos de expediente. `ExpedientId` permanece
nulo en la rama 0.

## Seguridad e idempotencia

- La autoridad procede del trámite resuelto en servidor y participa en el fingerprint.
- La rama 0 consulta el expediente SII para construir índices, pero no busca ni crea un expediente local,
  no vincula documentos y no registra caché de expediente.
- Los nombres dinámicos se validan; los valores SQL son parámetros y la postlectura confirma el efecto.
- No se registran respuestas SII crudas, identidad personal, cookies, tokens, credenciales ni conexiones.
- Reintento y reconciliación reutilizan estados persistidos y no fabrican efectos omitidos.
- La consulta de expediente SII completa la identidad antes de bifurcar; los metadatos descargados pueden
  completar únicamente campos todavía vacíos y nunca sobrescriben con vacío.
- Los índices documentales requieren escritura y postlectura `Confirmado`; `NoAplica` queda reservado a
  los efectos físicos del expediente.
- La ejecución E2E repite `CreateImportIntent` con la misma carga y clave antes de ejecutar; exige el
  mismo `IntentId` y `VersionToken`, y registra únicamente `idempotentCreate=CONFIRMED`.
- Los controles E2E son `SELECT`; el gate debe terminar en `false`, con usuarios y grupos vacíos.

## Inventario técnico

- Modelo: `Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebModels.vb`.
- Configuración y persistencia: `MySqlImportExpedientConfigurationRepository.vb`,
  `MySqlImportEffectConfigurationRepository.vb`, `MySqlImportIntentRepository.vb`,
  `MySqlImportRelatedDocumentRepository.vb` y `MySqlImportReconciliationRepository.vb`.
- Orquestación: `ImportEffectPlanBuilder.vb`, `ImportExpedientCoordinator.vb`,
  `ImportServiceOrchestrator.vb`, `ImportRelatedDocumentPlan.vb`,
  `ImportRelatedDocumentCoordinator.vb`, `ImportExecutionSteps.vb` e `ImportItemResultMapper.vb`.
- Índices: `SiiDocumentIndexAdapter.vb` y `LegacySiiDocumentIndexPhysicalGateway.vb`.
- Verificación: `tests/importar-servicio-web-doc71-no-expedient-mode.test.cjs`, suite transversal
  `tests/importar-servicio-web-*.test.cjs` y arnés `tools/e2e`.

## Despliegue y rollback

No se requiere DDL nuevo: los estados se almacenan como texto y las columnas existentes admiten
`NoAplica`. El rollback operativo consiste en desplegar la versión anterior con el gate moderno apagado;
no se reconstruyen expedientes ni se refolian históricos. Antes y después de cualquier E2E autorizada se
debe verificar `WorkflowCentroTrabajoModernActive=false` y alcance vacío.

## Evidencia local — 2026-09-20

- `node --test tests/importar-servicio-web-*.test.cjs`: 370 aprobadas, 0 fallidas.
- `node --test tools/e2e/tests/importar-servicio-web-modern.spec.cjs`: 21 aprobadas, 0 fallidas;
  incluye repetición idempotente de creación con identidad estable.
- `msbuild GestionDocumental-Docuarchi.net.sln /t:Build /p:Configuration=Debug /m`: 0 errores;
  permanecen advertencias históricas del proyecto.
- E2E real: rama 0 ejecutada con el resultado detallado a continuación; rama 1 pendiente de una muestra
  creadora nueva y autorizada.

### Corrida real rama 0 — 2026-09-20

La ejecución autorizada sobre la muestra MERCANTIL sin expediente completó una intención y almacenó un
documento. Confirmó la repetición idempotente de `CreateImportIntent`, los índices documentales, la
relectura y la reconciliación; relación, caché e índices exclusivos de expediente quedaron `NoAplica`.
Seis controles persistentes cambiaron y la caché de vínculo permaneció sin cambios, como exige la rama
0. El runner emitió `E2E_PLATFORM_MUTATION_EXPECTATION_FAILED` porque su expectativa histórica exigía
incorrectamente que esa caché cambiara en toda ejecución. El recurso de negocio quedó consumido y no se
reutilizará. La política se corrigió para exigir caché sin cambio en `without-expedient` y cambio en
`with-expedient`; sus pruebas quedaron 21/21 y las del runner transversal 22/22. Esta corrida conserva
evidencia real útil de almacenamiento e idempotencia, pero la inspección física invalidó su afirmación
de índices documentales: matrícula, NIT/cédula y razón social no se propagaron a los documentos del mismo
`ENLASE`. El defecto quedó corregido en código y cubierto por la suite, pero requiere una muestra nueva de
rama 0; tampoco cierra la matriz hasta validar una muestra creadora de la rama 1.

Una segunda generación preparada de la misma muestra se ejecutó el 2026-09-20 después de la corrección.
El runner terminó con `success=true`, confirmó la repetición idempotente, reportó modo
`without-expedient`, consumió la reserva y dejó sin cambio la caché de vínculo; el gate quedó apagado y
sin alcance. Esta evidencia corrige el falso negativo del arnés, pero el `SELECT` saneado de
`import-document-index-state` consulta estados del diario, no `MATRICULA`, `NITCEDULA` ni
`RAZONSOCIAL` del gabinete. Hasta contar con una comprobación física de todos los documentos enlazados,
la segunda corrida tampoco cierra por sí sola la validación de índices ni sustituye la E2E de rama 1.

La validación definitiva de rama 0 se realizó el 2026-09-20 con la tarea descartable `220584`, recibo
`S002194109` y muestra MERCANTIL. El runner terminó `success=true`, en modo `without-expedient`, con
idempotencia confirmada, seis controles persistentes modificados y caché de vínculo sin cambio. El
responsable inspeccionó físicamente todos los documentos del `ENLASE` y confirmó `MATRICULA`,
`NITCEDULA` y `RAZONSOCIAL` actualizados. El gate quedó apagado y sin alcance; esta evidencia cierra la
rama 0, pero no sustituye la E2E creadora pendiente de rama 1.

La rama creadora se validó el 2026-09-20 con la tarea descartable `220585`, recibo `S002495171` y muestra
MERCANTIL. El runner terminó `success=true`, detectó `with-expedient`, confirmó la repetición idempotente
y registró cambio en los siete controles, incluida la caché de vínculo. Ejecución, consulta y
reconciliación concluyeron correctamente; el gate quedó apagado, sin alcance y sin diferencias legacy.
Las ramas 0 y 1 de la matriz E2E quedan cubiertas.

## Pruebas obligatorias

- Regresión de bandera 0 con cero llamadas o escrituras de expediente.
- Identidad SII completa, parcial y ausente sin sobrescrituras vacías.
- Persistencia y relectura de `NoAplica`, finalización e invalidación de fingerprint.
- Regresión DOC-67 con bandera 1, suite transversal y MSBuild.
- E2E real de ramas 0/1, repetición e idempotencia únicamente cuando esté autorizada.

## E2E integrada y cierre seguro

La E2E pertenece a este mismo cambio y es obligatoria para cerrarlo. Debe reutilizar exclusivamente
`tools/e2e`, sus perfiles, autenticación, reservas, gate y verificadores; no se permite un arnés paralelo.
Antes de ejecutarla se debe leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`, y obtener autorización
explícita para ambiente, cuentas, TLS, gate y cada recurso descartable.

Se reutilizarán autenticación, configuración, validadores, evidencias y utilidades existentes, sin login,
arnés, Playwright, configuración ni `.env` paralelos. Se ejecutará solo con ambiente, usuarios/cuentas y
datos o tareas descartables expresamente autorizados.

Reutilizar exclusivamente `tools/e2e`, su autenticación, configuración, validadores, evidencias y
utilidades. No crear login, arnés, proyecto Playwright, configuración ni `.env` paralelos.

Las consultas de control serán exclusivamente `SELECT`. Los secretos serán efímeros y no se imprimirán
ni persistirán. La evidencia conservará solo códigos, conteos, estados y huellas saneadas. Se cubrirán una
muestra con bandera 0, otra con bandera 1 y una repetición idempotente. Si falta autorización o muestra
compatible, el cambio quedará bloqueado sin sustituir evidencia real por mocks. En `finally` se comprobará
`WorkflowCentroTrabajoModernActive=false`, usuarios y grupos vacíos, y páginas legacy sin cambios.

No se expondrán, imprimirán ni persistirán credenciales, cookies, tokens o cadenas de conexión. Cuando
aplique, se cubrirán autorización/control de acceso, lectura sin mutación, escrituras autorizadas,
concurrencia y regresión relacionada. Se respetarán feature flags, gates, usuarios y grupos sin habilitarlos
arbitrariamente. No se cerrará sin validación autorizada; se registrará bloqueo explícito sin mocks,
simulaciones ni evidencia ficticia.

Usar secretos efímeros. No exponer, imprimir, persistir ni guardar credenciales, cookies, tokens o cadenas
de conexión; usar verificaciones solo `SELECT` y conservar evidencia saneada. Respetar feature flags,
gates, usuarios y grupos sin habilitarlos arbitrariamente. No cerrar sin validación autorizada, registrar
bloqueo explícito y prohibir mocks, simulaciones, resultados inventados y evidencia ficticia.

## Criterios de aceptación

- La bandera 0 almacena el documento y actualiza los índices disponibles sin gestionar expediente.
- La consulta de expediente SII es obligatoria; NIT/razón social parciales no borran índices existentes.
- La bandera 1 conserva resolución, creación, vínculo, índices, caché e idempotencia DOC-67.
- Consulta y reconciliación distinguen `Confirmado`, `NoAplica`, fallos e incertidumbre.
- Suite automatizada, compilación, E2E autorizada y restauración del gate quedan demostradas.

## Documentación técnica

Este documento constituye el índice técnico de arquitectura, secuencia, estados, contratos, seguridad,
inventario, despliegue, rollback y evidencia del cambio. Los requisitos normativos permanecen en el cambio
OpenSpec `doc-71-actualizacion-util-crea-expediente-sii`.

## Entregable final

Cambio OpenSpec, implementación aditiva, contratos, pruebas automatizadas, compilación, arnés E2E
sensible al modo, E2E autorizada y evidencia saneada. No se archivará ni cerrará mientras falte la E2E
real o alguna compuerta OPSXJ.
