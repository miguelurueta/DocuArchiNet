## 1. Refinement y procedencia

- [x] 1.1 [S] Verificar las huellas de fuentes y reportes revisados. Área/archivos: `source-review.json`. Origen: D-01, RQ-01. Verificación: coinciden 14 hashes de fuentes y 10 históricos sin atribuirlos a otra ejecución.
- [x] 1.2 [S] Registrar la aprobación del alcance presentado. Área/archivos: `refinement.md, opsxj-governance.json`. Origen: D-01, RQ-01. Verificación: marcador y manifiesto approved con referencia a «continua» posterior al detalle de implementación.
- [x] 1.3 [S] Sustituir las ocho tareas genéricas por el mapa trazable. Área/archivos: `tasks.md`. Origen: D-01, RQ-01. Verificación: cada checkbox tiene complejidad, área, origen y verificación; diez parejas D/RQ cubiertas.

## 2. Contraste del comportamiento existente

- [x] 2.1 [S] Acreditar documentalmente COORD-01 en sus tres políticas. Área/archivos: `doc118-matriz-coordinacion.md`. Origen: D-03, RQ-03. Verificación: REC EXPEDIENTE prueba A lista/B por recibir y REC LOTE/PARCIAL mantienen procedencia sin repetir cargas.
- [x] 2.2 [M] Mapear negativos de autoridad, destino, revisión y publicación por fase. Área/archivos: `tests/ImportacionCarpetasBehaviorTests.cs, doc118-seguridad-recuperacion.md`. Origen: D-08, RQ-08. Verificación: cada combinación identifica una aserción existente o brecha concreta y distingue local de real.
- [x] 2.3 [S] Documentar recuperaciones admitidas para cada estado. Área/archivos: `CoordinadorImportacionCarpetas.Reconciliacion.vb, MySqlImportacionCarpetasRepository.Reconciliacion.vb, doc118-seguridad-recuperacion.md`. Origen: D-07, RQ-07. Verificación: distinguir cierre preinicio sin detención, reconciliación INCIERTO y bloqueo EN_CURSO; intentos 41/53 excluidos de mutación.
- [x] 2.4 [S] Contrastar consolidación con su transacción exterior e inventario original. Área/archivos: `MySqlImportacionCarpetasRepository.Documento.vb, MySqlImportacionCarpetasRepository.Consolidacion.vb, doc118-seguridad-recuperacion.md`. Origen: D-09, RQ-09. Verificación: identificar ConsolidarEnTransaccion dentro de RegistrarResultadoDocumento y estados con actividad, incertidumbre y detención final.

## 3. Preparación local de integración

- [x] 3.1 [M] Caracterizar una observación externa compatible de la llamada legacy activa. Área/archivos: `workflow/ClassAlmacenamiento.vb, tools/e2e/tests/support/doc117-alm03-method-observation.cjs, doc118-plan-integracion.md`. Origen: D-10, RQ-10. Verificación: identificar candidata externa y calificación necesaria o ausencia de vía validada; no aceptar copia instrumental DOC117 por inferencia.
- [x] 3.2 [M] Preparar protocolo de dos sesiones disputando almacenamiento. Área/archivos: `doc118-plan-integracion.md, CoordinadorImportacionCarpetas.Documento.vb, MySqlImportacionCarpetasRepository.Control.vb`. Origen: D-02, RQ-02. Verificación: dos sesiones distintas del propietario, mismo comando y otro archivo elegible del lote con revisión vigente y prueba de entrada única.
- [x] 3.3 [M] Preparar pérdida de respuesta y contraste de antigüedad sin takeover. Área/archivos: `doc118-plan-integracion.md, CoordinadorImportacionCarpetas.Documento.vb`. Origen: D-04, RQ-04; D-05, RQ-05. Verificación: corte del retorno solo al cliente y consulta durable; tiempo transcurrido sin inventar TTL ni modificar timestamps.
- [x] 3.4 [M] Preparar detención en vuelo y consulta desde otra sesión. Área/archivos: `doc118-plan-integracion.md, ServicioConsultaImportacion.vb, js/gestion/ImportacionCarpetas/ImportacionCarpetas.js`. Origen: D-06, RQ-06. Verificación: señal y efecto retenidos, bloqueo posterior y consulta sin reanudar ni asumir cancelación por cierre.
- [x] 3.5 [S] Ejecutar regresión local de coordinación y cliente. Área/archivos: `tests/doc110-perfil-importacion-behavior.test.cjs, tests/doc111-importacion-carpetas-client.test.cjs`. Origen: D-02, RQ-02; D-06, RQ-06. Verificación: ambos archivos pasan sin HTTP ni conexión de datos; evidencia con contadores reales.
- [x] 3.6 [M] Implementar la coordinación acotada entre una llamada activa y las acciones de otra sesión. Área/archivos: `tools/e2e/tests/support/doc118-active-call.cjs`, `tests/doc118-active-call.test.cjs`. Origen: D-10, RQ-10; D-02, RQ-02; D-06, RQ-06. Verificación: 16 pruebas locales verifican correlación interna, pausa máxima, hilo estable, liberación ante fallo, incertidumbre y ausencia de reintentos; el helper no concede autorización ni PASS de integración.

- [x] 3.7 [M] Enlazar dos sesiones y controles existentes con la ventana activa de DOC118. Área/archivos: `tools/e2e/tests/support/doc118-session-bridge.cjs`, `tools/e2e/tests/support/doc111-importacion-carpetas.cjs`, `tests/doc118-session-bridge.test.cjs`. Origen: D-02, RQ-02; D-06, RQ-06; D-10, RQ-10. Verificación: 36 pruebas del adaptador y regresión conjunta192/192 verifican reutilización de openSession/createDriver/createSelectControls, sesiones/grants, presupuestos y detención sin otro transporte/login ni efectos reales.

- [x] 3.8 [M] Preparar captura enmascarada y preflight autenticado por fases con los helpers existentes. Área/archivos: `tools/e2e/scripts/run-doc118-live.cjs`, `tools/e2e/scripts/support/doc118-live-plan.cjs`, `tests/doc118-live-preflight.test.cjs`. Origen: D-08, RQ-08; D-10, RQ-10. Verificación: regresión192/192, arrastre de contadores y bloqueo ante presupuesto agotado, binding verificado antes de captura, secretos solo en memoria y preflight readOnly; el rechazo real queda separado del PASS local y no provoca retries.

## 4. Integración real: compuertas antes de ejecutar

- [ ] 4.1 [M] Calificar el observador externo en runtime autorizado. Área/archivos: `doc118-plan-integracion.md, doc118-resultados-integracion.md`. Origen: D-10, RQ-10. Verificación: símbolos/binario coinciden, se observa el interior de la llamada y otro hilo atiende peticiones durante intervalo controlado sin modificar almacenamiento compartido.
- [x] 4.2 [S] Registrar autorización específica del plan y recursos nuevos. Área/archivos: `doc118-plan-integracion.md`. Origen: D-10, RQ-10. Verificación: ambiente, cuenta propietaria, dos sesiones, recursos, observación/fallos, límites y corte aprobados antes de autenticar o ejecutar efectos.
- [ ] 4.3 [M] Acreditar disputa del mismo comando con primera llamada activa. Área/archivos: `doc118-resultados-integracion.md, tools/e2e`. Origen: D-02, RQ-02; D-04, RQ-04. Verificación: segunda sesión obtiene historial/bloqueo seguro, una entrada al escritor y una identidad; antigüedad no habilita takeover.
- [ ] 4.4 [M] Acreditar bloqueo de otro archivo elegible del mismo lote. Área/archivos: `doc118-resultados-integracion.md, tools/e2e`. Origen: D-02, RQ-02. Verificación: segunda petición tiene revisión vigente y barrera satisfecha pero no entra otro escritor mientras sigue la primera reclamación.
- [ ] 4.5 [M] Acreditar recuperación del retorno perdido después del checkpoint. Área/archivos: `doc118-resultados-integracion.md, tools/e2e`. Origen: D-05, RQ-05. Verificación: tras probar checkpoint confirmado, consulta y un único replay del mismo comando devuelven mismos IDs sin otro escritor.
- [x] 4.6 [M] Acreditar bloqueo de cualquier incertidumbre observada en casos nuevos. Área/archivos: `doc118-resultados-integracion.md, doc118-seguridad-recuperacion.md`. Origen: D-05, RQ-05; D-07, RQ-07. Verificación: conservar IDs/estado sin otro efecto; si no ocurre registrar limitación y evidencia histórica, sin inyectar fallo adicional por inferencia.
- [ ] 4.7 [M] Acreditar detención con almacenamiento realmente en vuelo. Área/archivos: `doc118-resultados-integracion.md, tools/e2e`. Origen: D-06, RQ-06. Verificación: detención confirmada dentro del intervalo activo, efecto/IDs retenidos al finalizar y ninguna fase nueva afectada.
- [ ] 4.8 [M] Acreditar cierre de navegador y consulta posterior autorizada. Área/archivos: `doc118-resultados-integracion.md, ImportacionCarpetas.js`. Origen: D-06, RQ-06; D-08, RQ-08. Verificación: otra sesión observa snapshot, señal y estado durable sin reanudar ni cancelar implícitamente.

## 5. Verificación y entrega final

- [ ] 5.1 [M] Verificar inventario e identidades de los casos nuevos por SELECT. Área/archivos: `doc118-resultados-integracion.md`. Origen: D-09, RQ-09. Verificación: totales originales y documentos/inventarios coinciden exactamente; ningún efecto incierto se presenta completo.
- [ ] 5.2 [S] Consolidar evidencia final y limitaciones en documentación técnica. Área/archivos: `Doc/Tecnica/Opsxj/doc-118-produccion-importacion-carga-carpeta-lote-recuperacion/, doc118-matriz-coordinacion.md`. Origen: D-01, RQ-01; D-10, RQ-10. Verificación: cada caso tiene resultado/procedencia y no ejecutados no figuran PASS.
- [ ] 5.3 [S] Validar artefactos y trazabilidad finales para revisión OPSXJ. Área/archivos: `openspec/changes/doc-118-produccion-importacion-carga-carpeta-lote-recuperacion/`. Origen: D-01, RQ-01. Verificación: OpenSpec estricto y refinement pasan sobre entrega final sin sustituir integración.
- [ ] 5.4 [S] Verificar sincronización Git de la entrega final. Área/archivos: `feature/DOC-118 de la carpeta principal`. Origen: D-01, RQ-01. Verificación: cambios registrados, carpeta limpia y ahead/behind 0/0 con remoto.

## 6. Contratos internos V2, sin activar recuperación

- [x] 6.1 [M] RV-01: fijar documentos internos V2 de inicio, autorización y retorno observado y su serialización cerrada. Área/archivos: `Modelo/ProduccionDocumental/ImportacionCarpetas/DocumentosAlmacenamientoV2Importacion.vb`, proyecto VB, harness y pruebas de comportamiento existentes. Origen: D-11, RQ-11; RC-01, RC-03, RC-07 candidatos. Reproducción: intento69 durable EN_CURSO sin checkpoint y evidencia inicial V1 que no distingue autorización/retorno; no admite recuperación retrospectiva. Verificación:19 casos V2 pasan dentro del harness existente; regresión conjunta18/18 y herramientas205/205. Roundtrip canónico, rechazo de V1/forma extra/incompleta/contradictoria, vínculos exactos y ausencia de autoridad concedida por JSON; sin HTTP ni datos reales. Autorización local: «contiunua» posterior a la entrega RV-02-A y al siguiente paso RV-01; no habilita RV-02-B ni recuperación real.

- [x] 6.2 [M] RV-02-B: añadir el puerto V2 de retención/guardia no destructiva al staging existente. Área/archivos: `ImportacionCarpetasStagingV2Interfaces.vb`, `AlmacenStagingImportacion.Recuperacion.vb`, proyecto y harness existentes. Origen: D-12, RQ-12; RC-02, RC-07 candidatos. Reproducción:69 EN_CURSO carece de prueba V2 y RV-02-A solo validó primitivas, sin puerto real. Verificación:21 casos del proveedor real sobre PDF sintético,3 procesos propios cerrados, exclusión/Dispose/identidad/ámbito, hardlink/junction, ausencia y rechazo de otra época;18/18 regresión y205/205 herramientas. No reutilizar AbrirRetiro ni cambiar V1. Autorización: «continua» posterior a RV-01 y al siguiente paso RV-02-B; sin despliegue, recuperación real o aumento de presupuestos.

- [x] 6.3 [M] RV-03: añadir el recorrido interno V2 del adaptador con retención previa al inicio, acuses tipados de persistencia y observación de retorno. Área/archivos: `AdaptadorDocumentoImportacion.ProtocoloV2.vb`, `PersistenciaDocumentoV2Importacion.vb`, proyecto y harness existentes. Origen: D-13, RQ-13; RC-02, RC-03 candidatos. Reproducción:69 V1 EN_CURSO sin checkpoint y el adaptador actual adquiere su retención después del inicio. Verificación:20 casos nuevos, regresión18/18 y herramientas205/205; bloqueo antes/durante/después de inicio/gate, cero escritor ante acuse perdido/alterado, una llamada, IDs parciales, cancelación/ThreadAbort propagados y retención liberada. Puerto de persistencia simulado, sin fallback V1; MySQL es RV-04. Autorización:«continua» posterior a RV-02-B y al siguiente paso RV-03; sin activar composición pública, desplegar ni tocar69.

- [x] 6.4 [M] RV-04: persistir inicio, autorización, retorno y checkpoint V2 en el repositorio MySQL existente. Área/archivos: nuevo parcial `MySqlImportacionCarpetasRepository.DocumentoV2.vb`, lector documental compatible, proyecto y harness. Origen: D-14, RQ-14; RC-01, RC-03, RC-06 candidatos. Reproducción:RV-03 solo dispone de dobles y69 carece de diario V2. Verificación:20 casos SQL nuevos integrados con adaptador sobre doble transaccional,18/18 regresión y205/205 herramientas; DDL estático ampliado pasa. Locks lote/raíz/archivo/intento, token/hash/evidencia/diario, gate de un solo uso, acuse tras Operar/commit, checkpoint/evento/consolidación en tx y V1 compatible. Sin DDL, ejecución real, activación pública o backfill. Autorización:«contiua con el flujo» posterior a RV-03 y al siguiente paso RV-04.

- [x] 6.5 [M] RV-05: implementar recuperación interna V2 con guardia, clasificación y persistencia comparada. Área/archivos: parciales de coordinador/repositorio, comando de comprobación, catálogo SELECT y harness existentes. Origen: D-15, RQ-15; RC-01/02/04/05/06 candidatos. Reproducción:RV-04 conserva el diario pero no recupera EN_CURSO;69 permanece V1. Verificación:22 casos nuevos y223/223 Node; cierre negativo solo preparado/no invocación/expediente sin huellas y confirmación positiva solo IDs observados/verificados, guardia hasta commit/rollback, autoridad/vista/revisiones exactas, rollback e historial sin efectos. Staging real sintético, SELECT/transacción simulados, cero escritor. Sin conversión a INCIERTO, reset general, backfill, despliegue o ejecución real. Autorización:«continua» posterior a RV-04 y al siguiente paso de recuperación limitada V2.

## Ejecución y defectos reproducidos





Las rutas cortas de informes se resuelven en `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/`; las rutas de coordinador y repositorio son las de design.md y source-review.json. S/M indican complejidad relativa, no duración.

Las tareas 4.x y 5.1 requieren evidencia real nueva. Preparar protocolos o aprobar refinement no las completa ni autoriza ejecución. Calificar un observador conectado al runtime también exige autorización previa; 4.1/4.2 se preparan juntas y solo se ejecuta lo específicamente autorizado.

Ante un defecto reproducido, insertar una tarea acotada con ruta, origen D/RQ, reproducción y regresión antes de modificar producto. No se inventan cambios preventivos ni una tarea genérica de implementación. No reutilizar intentos históricos 41/53.


La continuación local del 2026-11-08 implementó las siete fases y cerró la regresión160/160. También verificó disponibilidad/INSPECT/CLOSE del SDK sin ARM ni Freeze. Estas preparaciones apoyan4.1/4.3–4.8 y5.1, pero no completan sus criterios reales: permanecen11 tareas pendientes. Véase doc118-ejecutor-fases-local-20261108.md.


Corte de auditoría física y prueba sintética:4.6 cerrada por contención de69, referencias/huella preservadas y ausencia actual sin replay; quedan10 tareas pendientes. La liberación SDK pasó en proceso sintético, sin acreditar4.1 o COORD02/03. Fuente:`doc118-auditoria-fisica-liberacion-20261108.md`.

Corte RV-01: se añadió y completó6.1; mapa18/28 con las mismas10 tareas reales/finales pendientes. El contrato interno no modifica D-07/RQ-07 ni completa4.1 o COORD02/03.

Corte RV-02-B: se añadió y completó6.2; mapa19/29 y las mismas10 tareas anteriores pendientes. El puerto real está validado en fixtures NTFS locales; producción y recuperación siguen sin activar/calificar. Otra época del AppDomain, incluso tras reinicio, mantiene bloqueo.

Corte RV-03: se añadió y completó6.3; mapa20/30 y las mismas10 tareas anteriores pendientes. Recorrido interno del adaptador con dobles de persistencia/escritor y retención real. RV-04 y la composición pública V2 siguen pendientes.

Corte RV-04: se añadió y completó6.4; mapa21/31, mismas10 tareas anteriores pendientes. Persistencia MySQL implementada y núcleo SQL integrado localmente con adaptador; atomicidad/concurrencia real MySQL, composición pública y recuperación siguen sin acreditar/activar.

Corte RV-05 del2026-11-09:se añadió y completó6.5; mapa22/32, mismas10 tareas anteriores pendientes. Recuperación interna implementada y probada con guardia real/SELECT y transacción simulados; exposición pública y calificación real pendientes.69/V1 siguen inelegibles.
