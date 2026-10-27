# DOC-110: aceptación de carga individual y preservación de SII

9.3 completa con carga individual real, comprobaciones locales y revisión de integridad descritas a continuación. Las secciones de preparación conservan el historial anterior; sus estados pendientes quedan superados por esta aceptación.

## Aceptación real confirmada

El usuario ejecuta el runner oficial con LUZ.AGUILERA y confirma ambiente, cuenta, archivo, ejecución y recurso descartables. Resultado: «La plataforma E2E terminó correctamente (production-document-upload-execution); controles=1; sinCambios=NO».

El reporte saneado tools/e2e/artifacts/workflow-e2e-platform-production-document-upload-execution.json confirma success true, sin failureCode, stage execution y count 1. Códigos: invalidSelection BLOCKED, preparation CONFIRMED, documentType CONFIRMED, storage CONFIRMED, projection CONFIRMED y navigation NOT_OBSERVED. El control SELECT production-document-records fue comprobado y changed true.

Se utilizó el PDF sintético doc110-carga-individual.pdf en el expediente descartable 467, tipo 50 «Comprobante de Egreso». El recorrido exige un único envío al handler y proyección visual confirmada. El control SQL acredita cambio en el registro documental del expediente; no se presenta su huella como un recuento SQL detallado de filas ni como una consulta de descarga del binario.

sinCambios=NO es el resultado esperado para una ejecución que almacena el PDF: el control documental debe cambiar. No señala por sí mismo una regresión o modificación de SII.

El intento anterior con villavicencio se detuvo con EXPEDIENT_NOT_LISTED antes de seleccionar/enviar el archivo; no se le atribuye almacenamiento. La corrida aceptada corresponde a LUZ.AGUILERA. No hace falta repetir la carga exitosa ni borrar el documento para registrar PASS.

## Alcance de preservación de SII

La carga DOC-88 real se combina con las 59 comprobaciones locales y ausencia de cambios rastreados en rutas legacy revisadas. No se alteró ninguna huella aprobada para obtener PASS. No se ejecutó una importación SII ni se afirma integración funcional SII nueva: para las rutas SII conservadas, la evidencia es de código/contratos e integridad, diferenciada de la carga individual real. Esto completa el criterio de 9.3 con sus límites explícitos.

## Verificación local

Se ejecutaron los seis archivos tests/doc88-*.test.cjs, las tres suites tests/importar-servicio-web-modern-*.test.cjs y tools/e2e/tests/doc88-production-document-upload-policy.test.cjs: **37 pruebas PASS**. También tools/e2e/tests/importar-servicio-web-modern.spec.cjs mediante node --test: **22 PASS**. Esta última es una suite Node de políticas/contratos, no una corrida Chromium ni una importación SII real.

En conjunto son **59 comprobaciones locales PASS**. Incluyen huella aprobada del handler compartido, aislamiento de la rama PRODUCCION, preparación/validación de tipologías, errores terminales, un único envío sin consulta de existencia/reintento, proyección visual y contratos/caché/fallback de SII. No sustituyen almacenamiento/consulta reales.

La revisión git diff no muestra modificaciones rastreadas en workflow, las rutas SII consultadas, workflow/ClassAlmacenamiento.vb, generic_control/FileUploadHandler_.ashx.vb, generic_control/FileUploadHandler.js ni webservice/WebServiceProducion.asmx.vb. Los cambios de DOC-110 en el code-behind de Producción añaden host/contexto y PreRender; no cambian su rutina de carga. Esto acredita inspección de código, no ejecución funcional de SII.

## Recorrido real preparado

Se reutiliza el escenario oficial production-document-upload-execution de la plataforma DOC-88. No se crea otro uploader, login o framework. Su control production-document-records es SELECT parametrizado por expediente y exige cambio confirmado; el recorrido verifica envío único, resultado del handler, proyección visual y conservación del contexto.

El perfil DOC-88 existente apunta al expediente 467 y tipo 50 «Comprobante de Egreso». No contiene etiqueta que confirme que el expediente siga siendo descartable. Se solicita al usuario confirmar ese destino o indicar otro antes de añadir un documento; no se infiere esa condición del mero hecho de existir en un perfil antiguo.

El PDF antiguo doc88-manual-integracion.pdf no existe. Se prepara tools/e2e/fixtures/doc110-carga-individual.pdf, una página sintética con el texto «DOC-110 - PRUEBA DESCARTABLE» y sin datos reales. El perfil nuevo doc110-production-upload-execution.local.runtime.json reutiliza la configuración no sensible existente, cambia solo escenario/fixture y pasa validateProfile. El perfil DOC-88 original se conserva. Preparar el perfil no autoriza por sí mismo añadir al expediente 467.

Comando previsto tras confirmar destino:

```powershell
npm.cmd --prefix tools/e2e run test:workflow:platform -- --scenario production-document-upload-execution --profile doc110-production-upload-execution.local.runtime.json --authorize environment,account,discardable-file,execution,discardable-resource
```

El runner oficial solicita credenciales efímeras de la cuenta y de MySQL de solo lectura para sus controles. No se guardan en el perfil ni deben enviarse por chat. Se conservan las confirmaciones propias del runner compartido. No se modifica ese runner para omitir controles o repetir una carga de resultado incierto.

El usuario confirma explícitamente el expediente 467 como destino descartable para añadir el PDF sintético. Se abre la consola con el comando preparado; las credenciales y confirmaciones del runner se introducen allí. Esta confirmación supera el pendiente de destino de las notas anteriores.

Primer lanzamiento detenido antes de login/carga con E2E_PLATFORM_AUTHORIZATION_ARGUMENT_REQUIRED: el comando omitía execution y discardable-resource que SAFE_STAGE_AUTHORIZATIONS añade para la etapa execution. Se corrige el comando, sin cambiar ni relajar el runner. requiredAuthorizationsFor y preflightPlatform ejecutados localmente confirman la lista exacta de cinco autorizaciones y preflight PASS. No hubo solicitud de almacenamiento ni resultado incierto; repetición pendiente.

Estado: resultado de almacenamiento real/consulta **PENDIENTE**. No se importan lotes SII ni se modifica su configuración para esta regresión. No cerrar 9.3 por la confirmación, apertura de consola, pruebas locales o preparación del PDF; se requiere resultado y controles de la corrida.
