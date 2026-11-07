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

- [x] 3.7 [M] Enlazar dos sesiones y controles existentes con la ventana activa de DOC118. Área/archivos: `tools/e2e/tests/support/doc118-session-bridge.cjs`, `tools/e2e/tests/support/doc111-importacion-carpetas.cjs`, `tests/doc118-session-bridge.test.cjs`. Origen: D-02, RQ-02; D-06, RQ-06; D-10, RQ-10. Verificación: 23 pruebas del adaptador y regresión conjunta 102/102 verifican reutilización de openSession/createDriver/createSelectControls, sesiones/grants, presupuestos y detención sin otro transporte/login ni efectos reales.

- [x] 3.8 [M] Preparar captura enmascarada y preflight autenticado por fases con los helpers existentes. Área/archivos: `tools/e2e/scripts/run-doc118-live.cjs`, `tools/e2e/scripts/support/doc118-live-plan.cjs`, `tests/doc118-live-preflight.test.cjs`. Origen: D-08, RQ-08; D-10, RQ-10. Verificación: regresión106/106, binding verificado antes de captura, secretos solo en memoria y preflight readOnly; el rechazo real queda separado del PASS local y no provoca retries.

## 4. Integración real: compuertas antes de ejecutar

- [ ] 4.1 [M] Calificar el observador externo en runtime autorizado. Área/archivos: `doc118-plan-integracion.md, doc118-resultados-integracion.md`. Origen: D-10, RQ-10. Verificación: símbolos/binario coinciden, se observa el interior de la llamada y otro hilo atiende peticiones durante intervalo controlado sin modificar almacenamiento compartido.
- [x] 4.2 [S] Registrar autorización específica del plan y recursos nuevos. Área/archivos: `doc118-plan-integracion.md`. Origen: D-10, RQ-10. Verificación: ambiente, cuenta propietaria, dos sesiones, recursos, observación/fallos, límites y corte aprobados antes de autenticar o ejecutar efectos.
- [ ] 4.3 [M] Acreditar disputa del mismo comando con primera llamada activa. Área/archivos: `doc118-resultados-integracion.md, tools/e2e`. Origen: D-02, RQ-02; D-04, RQ-04. Verificación: segunda sesión obtiene historial/bloqueo seguro, una entrada al escritor y una identidad; antigüedad no habilita takeover.
- [ ] 4.4 [M] Acreditar bloqueo de otro archivo elegible del mismo lote. Área/archivos: `doc118-resultados-integracion.md, tools/e2e`. Origen: D-02, RQ-02. Verificación: segunda petición tiene revisión vigente y barrera satisfecha pero no entra otro escritor mientras sigue la primera reclamación.
- [ ] 4.5 [M] Acreditar recuperación del retorno perdido después del checkpoint. Área/archivos: `doc118-resultados-integracion.md, tools/e2e`. Origen: D-05, RQ-05. Verificación: tras probar checkpoint confirmado, consulta y un único replay del mismo comando devuelven mismos IDs sin otro escritor.
- [ ] 4.6 [M] Acreditar bloqueo de cualquier incertidumbre observada en casos nuevos. Área/archivos: `doc118-resultados-integracion.md, doc118-seguridad-recuperacion.md`. Origen: D-05, RQ-05; D-07, RQ-07. Verificación: conservar IDs/estado sin otro efecto; si no ocurre registrar limitación y evidencia histórica, sin inyectar fallo adicional por inferencia.
- [ ] 4.7 [M] Acreditar detención con almacenamiento realmente en vuelo. Área/archivos: `doc118-resultados-integracion.md, tools/e2e`. Origen: D-06, RQ-06. Verificación: detención confirmada dentro del intervalo activo, efecto/IDs retenidos al finalizar y ninguna fase nueva afectada.
- [ ] 4.8 [M] Acreditar cierre de navegador y consulta posterior autorizada. Área/archivos: `doc118-resultados-integracion.md, ImportacionCarpetas.js`. Origen: D-06, RQ-06; D-08, RQ-08. Verificación: otra sesión observa snapshot, señal y estado durable sin reanudar ni cancelar implícitamente.

## 5. Verificación y entrega final

- [ ] 5.1 [M] Verificar inventario e identidades de los casos nuevos por SELECT. Área/archivos: `doc118-resultados-integracion.md`. Origen: D-09, RQ-09. Verificación: totales originales y documentos/inventarios coinciden exactamente; ningún efecto incierto se presenta completo.
- [ ] 5.2 [S] Consolidar evidencia final y limitaciones en documentación técnica. Área/archivos: `Doc/Tecnica/Opsxj/doc-118-produccion-importacion-carga-carpeta-lote-recuperacion/, doc118-matriz-coordinacion.md`. Origen: D-01, RQ-01; D-10, RQ-10. Verificación: cada caso tiene resultado/procedencia y no ejecutados no figuran PASS.
- [ ] 5.3 [S] Validar artefactos y trazabilidad finales para revisión OPSXJ. Área/archivos: `openspec/changes/doc-118-produccion-importacion-carga-carpeta-lote-recuperacion/`. Origen: D-01, RQ-01. Verificación: OpenSpec estricto y refinement pasan sobre entrega final sin sustituir integración.
- [ ] 5.4 [S] Verificar sincronización Git de la entrega final. Área/archivos: `feature/DOC-118 de la carpeta principal`. Origen: D-01, RQ-01. Verificación: cambios registrados, carpeta limpia y ahead/behind 0/0 con remoto.

## Ejecución y defectos reproducidos

Las rutas cortas de informes se resuelven en `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/`; las rutas de coordinador y repositorio son las de design.md y source-review.json. S/M indican complejidad relativa, no duración.

Las tareas 4.x y 5.1 requieren evidencia real nueva. Preparar protocolos o aprobar refinement no las completa ni autoriza ejecución. Calificar un observador conectado al runtime también exige autorización previa; 4.1/4.2 se preparan juntas y solo se ejecuta lo específicamente autorizado.

Ante un defecto reproducido, insertar una tarea acotada con ruta, origen D/RQ, reproducción y regresión antes de modificar producto. No se inventan cambios preventivos ni una tarea genérica de implementación. No reutilizar intentos históricos 41/53.
