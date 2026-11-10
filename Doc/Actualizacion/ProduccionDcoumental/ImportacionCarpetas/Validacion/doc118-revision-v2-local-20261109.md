# DOC-118 — revisión local consolidada V2, RV-08

Base revisada:`e68d9972c467967890912f620352c6ea9dc2619c`, carpeta principal `feature/DOC-118`. RV-08 consolida revisión y preparación; no modifica producto/pruebas, no despliega ni ejecuta datos reales. La continuación del usuario posterior a RV-07 autoriza6.8, bajo D-01/RQ-01 y D-10/RQ-10, sin ampliar contratos.

| Dimensión | Resultado de la revisión |
| --- | --- |
| Completitud |25/35 tareas tras cerrar6.8;9/9 entregables del plan V2 tienen evidencia local/preparación. Diez tareas reales/finales continúan abiertas y bloquean archivo. |
| Corrección |16 requisitos y35 escenarios trazados a código/pruebas/evidencia; la trazabilidad no equivale a aceptación real. RQ-11..RQ-16 tienen verificación local; los criterios de integración original conservan sus límites históricos y casos sin acreditar. |
| Coherencia |Códec, staging, adaptador, mismo repositorio/transacción, recuperación y entrada HTTP siguen D-11..D-16. No aparece otro motor, worker, DDL, backfill o modificación del almacenamiento compartido/SII. V1 habitual sigue en la composición. |

## Requisitos y comprobación

| Requisito | Código/evidencia inspeccionados | Alcance acreditado y límite |
| --- | --- | --- |
| RQ-01 | source-review.json, deltas autorizados, hashes de reportes; matriz/revisiones | Procedencia local comprobada; no atribuir evidencias históricas a otra corrida. |
| RQ-02 | Coordinador.Documento/Operaciones, Repository.Control; DocumentPhaseCases/DocumentGlobalBarrierCases/ControlCases | Exclusión local y preparación existentes;4.3/4.4 reales abiertas. |
| RQ-03 | Coordinador.Barreras; CreationBarrierCases; matriz COORD-01 | Tres políticas/inventario contrastados localmente y con evidencia real histórica explícita. No repetir cargas históricas. |
| RQ-04 | Repository.Control; ControlCases; matriz V2 | No takeover por edad;4.1/4.3 reales sin acreditar. |
| RQ-05 | Coordinador.Documento, Repository.Documento y V2; DocumentPhaseCases y pruebas V2 de respuesta perdida | Mismos IDs y no repetición verificados localmente; DOC117 histórico conserva su SHA;4.5 real abierta. |
| RQ-06 | Coordinador/Operaciones, ServicioConsulta, cliente JS; ControlCases/OperationHttpCases y suite del cliente | Detención/consulta locales;4.7/4.8 reales abiertas. |
| RQ-07 | Coordinador/Repository.Reconciliacion; ReconciliationCases/OperationHttpCases | Cierre preinicio y reconciliación conocidos conservados; no convertir EN_CURSO V1 ni recuperar41/53/69. |
| RQ-08 | ContextAdapter/RequestGuard/ServicioConsulta; ContextAuthorityCases/RequestGuardCases/HTTP V2 | Autoridad/destino/revisiones/snapshot y JSON cerrado locales; sin actualizar permisos reales ni inferir su vigencia. |
| RQ-09 | Repository.Consolidacion/Documento; ConsolidationCases y regresión V2 | Inventario/checkpoints/stop locales e históricos delimitados;5.1 de casos reales nuevos abierta. |
| RQ-10 | runbook/plan original y helpers E2E, presupuesto y observador | Preparación y pruebas sintéticas; el observador del runtime sigue sin calificar.4.1 y aceptación COORD-02/03 abiertas. |
| RQ-11 | DocumentosAlmacenamientoV2Importacion y19 casos de contratos | Datos V2 cerrados y vínculos, sin conceder capacidad por JSON; sin backfill. |
| RQ-12 | AlmacenStagingImportacion.Recuperacion y21 casos del proveedor; prueba nativa y matriz | NTFS local/guardia/identidad/época probados con fixtures. No acredita proveedor instalado, otro nodo o recuperación tras reinicio. |
| RQ-13 | AdaptadorDocumentoImportacion.ProtocoloV2/PersistenciaDocumentoV2 y20 casos del adaptador | Retención previa, una invocación máxima, observación/abortos, sin fallback; escritor simulado. |
| RQ-14 | Repository.DocumentoV2, Operar y20 casos SQL | Acuse tras Operar/commit, gate único y checkpoint/evento/consolidación en el código; transacción simulada, sin atomicidad MySQL real acreditada. |
| RQ-15 | Coordinador/Repository/Catálogo.RecuperacionV2 y22 casos de recuperación;12 de matriz | Negativo estrecho/positivo por IDs originales, guardia/vista hasta commit, replay sin escritura; comprobación física SELECT simulada. |
| RQ-16 | ASMX:93, composición de reconciliación, DTOs y19 casos HTTP | POST/sesión/CSRF/autoridad/cuerpo/proyección; dispatcher/composición productivos compilados, HTTP sintético y sin IIS alojado. |

Las referencias cortas se resuelven en las carpetas existentes `Services/ProduccionDocumental/ImportacionCarpetas`, `Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas`, `Infrastructure/ProduccionDocumental/ImportacionCarpetas` y `tests/`. El JSON adjunto contiene rutas completas, huellas, símbolos y escenarios por requisito. La [matriz RV-07](doc118-matriz-v2-local-20261109.md) identifica21 condiciones concretas y sus aserciones.

## CRITICAL — diez tareas que impiden archivar DOC-118

Estos son criterios pendientes de entrega completa, no diez defectos nuevos de código. Deben resolverse antes de declarar el ticket completo:

| Tarea | Acción concreta requerida |
| --- | --- |
|4.1 | Calificar símbolos/binario, llamada legacy activa y atención por otro hilo en runtime autorizado, siguiendo el plan original y sin alterar almacenamiento compartido. |
|4.3 | Acreditar dos sesiones sobre el mismo comando con llamada real activa, una entrada al escritor y bloqueo/historial concordante. |
|4.4 | Acreditar bloqueo de otro archivo elegible con revisión vigente y barrera satisfecha durante esa reclamación. |
|4.5 | Probar checkpoint confirmado y un replay autorizado tras pérdida de retorno, con IDs y escritor únicos. |
|4.7 | Probar señal de detención durante la llamada real y conservar su efecto/IDs sin nuevas fases afectadas. |
|4.8 | Probar cierre de navegador y consulta desde otra sesión autorizada sin reanudar ni cancelar implícitamente. |
|5.1 | Contrastar por SELECT inventario e identidades de los casos reales nuevos. |
|5.2 | Consolidar evidencia final de esos casos y sus limitaciones; esta revisión local es un corte anterior a la entrega final. |
|5.3 | Revalidar artefactos/trazabilidad sobre esa entrega final, después de resolver la aceptación requerida. |
|5.4 | Verificar Git limpio/ahead-behind0/0 de la entrega final; la sincronización de RV-08 solo acredita este corte. |

## WARNING — dependencias de la calificación real V2

1. `Infrastructure/ProduccionDocumental/ImportacionCarpetas/ComposicionDocumentoImportacion.vb:9` compone el adaptador V1. `AlmacenarV2` y `CrearPersistenciaDocumentoV2` existen internamente, pero ese enlace habitual no los utiliza. Acción:delimitar, implementar y revisar localmente el enlace para intentos nuevos antes de pedir despliegue; no backfill ni inventar pruebas V2.
2. `tools/e2e/tests/support/doc111-importacion-carpetas.cjs:9` no incluye RecuperarDocumentoV2 en su allowlist. Acción:ampliar ese driver y sus contratos/pruebas bajo alcance autorizado, reutilizando transporte/CSRF/contadores; no presentar el plan como un CLI listo.
3. `AlmacenStagingImportacion.Recuperacion.vb` liga evidencia a una época de AppDomain y proveedor local. Las pruebas de transacción usan dobles. Acción:calificar proveedor/época/original y MySQL real en el ambiente aprobado; rechazar otro nodo/reinicio y conservar el bloqueo si falta prueba.

No se detectó en esta revisión una divergencia local nueva que justifique cambiar producto. No se omiten las limitaciones anteriores ni se cambia su severidad para archivar. No se agregan sugerencias de estilo o refactorizaciones preventivas.

## Factibilidad, regresión y procedencia

La factibilidad **local acotada** pasó en RV-02-A/RV-02-B/RV-07:original NTFS sintético, handles reales, identidad/hash/enlaces, exclusión y vida útil. La calificación del sitio permanece SIN_ACREDITAR. Un proceso lector terminado libera su handle, pero no acredita una época ajena ni demuestra no invocación; el diario sigue siendo obligatorio.

Se reutiliza la regresión de RV-07 en la base revisada:223/223 Node (18 comportamiento/cliente y205 herramientas), con133 casos V2 internos incluidos en el arnés. No se suman esos casos a Node ni se repiten pruebas al editar únicamente documentos. Las huellas del código, pruebas y herramientas se comprueban de nuevo en RV-08; no hubo nuevas compilaciones, procesos de prueba o ejecución HTTP/SQL.

Las14 fuentes revisadas conservan12 archivos y2 deltas de pruebas documentados; los10 informes históricos siguen intactos. Las16 fuentes evaluadas conservan15 archivos y el único delta autorizado del comando en RV-05. No se cambian hashes originales para ocultar deltas. Los ocho informes de RV-02-A/RV-01..RV-07 se conservan con su SHA y límites; RV-08 añade revisión/preparación, no una corrida nueva.

## Resultado y siguiente acción

**Revisión local V2 consolidada; DOC-118 NO_LISTO_PARA_ARCHIVO.** RV-08 cierra el plan local de nueve entregables, no la aceptación real del ticket. Preparado el [plan real V2 con dependencias](doc118-plan-real-v2-20261109.md), con dos lotes nuevos, fallos de acuse acotados, dos invocaciones máximas al escritor y topes separados aún no autorizados. Antes de pedir ejecución, resolver composición/driver/enlace de fallos y actualizar el candidato revisable.

Sin autenticar, desplegar, consultar datos o modificar permisos/configuración/IIS/workers. Consumos históricos1276 HTTP/113 SELECT/9 aperturas sin cambios, sin capacidad adicional inferida.41/53/69 permanecen protegidos. Evidencia:[JSON de revisión](doc118-revision-v2-local-20261109.json).
