# Jira Context - DOC-117

## Summary

PRODUCCION-IMPORTACION-CARGA-CARPETA-ALMACENAMIENTO

## Description

> Alcance refinado
> Revisión focal del adaptador de almacenamiento/incorporación existente y su verificador; conservar la función compartida intacta.
> Base y procedencia
> Revisión arquitectónica 2026-11-04 solicitada por el usuario frente a lo ya implementado. Base entregada: PR125 integrado en main, merge a3c7263763a89901ba78d05ccebc284a43a4cd76. DOC110–115 están Listo. Esta revisión es documental: no ejecuta nuevas pruebas ni certifica escenarios faltantes. Los resultados históricos conservan su SHA y ambiente; no equivalen a una nueva corrida del commit actual.
> Implementación existente comprobada
> Comando explícito y valores efectivos verificados; copia consumible privada con nombre original, retención del staging, llamada legacy única, IDs documento/inventario capturados y comprobación de gabinete/expediente/índices/incorporación. Errores posteriores conservan INCIERTO sin replay. REC reales verificaron PDFs, nombres, índices, IDs y repetición/reapertura; lote2 tuvo reconciliación del documento16204 sin volver a almacenarlo. Esta evidencia no acredita todos los negativos ni concurrencia externa.
> Rutas que deben reutilizarse
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorDocumentoImportacion.vb
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorDocumentoImportacion.Legacy.vb
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Infrastructure/ProduccionDocumental/ImportacionCarpetas/AlmacenStagingImportacion.vb
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Documento.vb
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.Documento.vb
> Evidencia existente y su alcance
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-adaptador-documento-local-20261030.md
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-fase-documento-local-20261030.md
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-valores-diferidos-local-20261030.md
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-rec-lote-pass-20261102.md
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-lote2-verificacion-pass-20261102.md
> Escenarios y trabajo pendiente
> ALM-01 — COBERTURA PARCIAL REAL/LOCAL: índices, tipología, nombre/extensión, vínculo e inventario tienen REC real; ceros/homónimos y guards están caracterizados localmente. DOC114 probó dos formatos en preparación; eso no acredita por sí solo su almacenamiento final. Completar únicamente combinaciones sin evidencia.
> ALM-02 — COBERTURA PARCIAL REAL/LOCAL: lote2 acredita conservación del mismo documento y reconciliación; el fallo inyectado exacto después de almacenar/antes de checkpoint y sus ventanas requieren vincular pruebas locales e integración faltante. Con IDs insuficientes o evidencia discordante, conservar INCIERTO. Si falta incorporación no existe licencia para volver a almacenar ni inventar un reparador: comprobar la vía soportada y reportar bloqueo cuando no sea segura.
> ALM-03 — NO ACREDITADO COMO CONCURRENCIA REAL: la carga individual durante degradación de DOC111 no equivale a importación y carga individual simultáneas. Si se requiere probar coexistencia, acotar a sesión, copia/original y relaciones con recursos autorizados. No reabrir el aislamiento XML descartado ni declarar SII concurrente validado; cualquier escenario SII requiere alcance y autorización separados.
> Negativos — reutilizar pruebas de campo ajeno al gabinete, tipología inexistente, relación incorrecta, nombre inseguro, staging alterado, pérdida de respuesta y checkpoint fallido. Mantener rechazo explícito TIFF/Multi-TIFF; no ampliar formatos.
> Criterio de cierre
> Matriz ALM01–03 con procedencia real/local y límites, llamada legacy protegida, original conservado y ausencia de doble almacenamiento. Ninguna corrección preventiva de la función legacy o del XML. Integraciones no autorizadas permanecen NO_EJECUTADO y no se convierten en PASS.
> Reglas comunes de ejecución
> Reutilizar modelos, repositorio operativo, tablas, ASMX/ASHX, adaptadores, coordinador y herramientas E2E existentes. Antes de programar, contrastar cada criterio con código y evidencia; modificar producto solo ante una brecha reproducida y cubrir su regresión. Leer AGENTS.md y tools/e2e/AGENT-RUNBOOK.md antes de autenticar. Pruebas reales, cambios de permisos, fallos inyectados, limpieza y carga requieren autorización específica de ambiente, cuentas y recursos; este ticket no la concede. Controles RA/DA exclusivamente SELECT, secretos efímeros, evidencia saneada. No repetir efectos inciertos ni cargas ya acreditadas sin motivo.
> Decisiones vigentes que sustituyen exigencias históricas
> No modificar AlmacenamientoDocumentoProduccionDocumental ni SII. Se conserva PDF como formato soportado por este adaptador; TIFF/Multi-TIFF deben rechazarse, no implementarse para satisfacer el prompt anterior. El aislamiento adicional del XML posterior al commit frente a otras cargas fue descartado expresamente por el usuario: queda fuera del alcance y no se afirma como garantía. No habilitar workers, migraciones, permisos, cuotas globales ni limpieza automática por inferencia. Preservar EXPEDIENTE/LOTE y PARCIAL_PERMITIDO/NO_APLICA, snapshot, inventario original y efectos confirmados. Sin rollback global.
> Fuente transversal
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-matriz-final-20261102.md
> https://github.com/miguelurueta/DocuArchiNet/blob/a3c7263763a89901ba78d05ccebc284a43a4cd76/Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc115-verificacion-final-20261104.md

## Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: ALMACENAMIENTO, CARGA, CARPETA, EXPEDIENTE, IMPORTACION, PRODUCION
