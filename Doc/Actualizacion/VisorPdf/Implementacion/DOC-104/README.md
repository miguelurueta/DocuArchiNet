# DOC-104 — procesamiento servidor del grafo

## Componentes

- `webservice/WebService_itext.asmx.vb`: validación, certificado, respuesta y auditoría.
- `itextshare/Class_ItexShare.vb`: lock, workspace, imagen, stamper, validación, reemplazo y clasificación.
- `workflow/Class_transparent.vb`: transparencia `LockBits`.
- `pdfjs/pdf_anotate_view/pdf_viewer_anotate.js`: refresco `getPage(i)`.
- `tools/e2e`: políticas, pruebas sintéticas y runner protegido.

## Rollback operativo

Antes del commit, cualquier fallo deja el original intacto. Después de `File.Replace`, una validación fallida intenta restaurar el backup. Si la restauración tampoco puede completarse, el backup se conserva para recuperación y la respuesta no confirma éxito.

## Ejecución segura

Las pruebas locales no requieren autenticación. Para modo real, leer `tools/e2e/AGENT-RUNBOOK.md` y ejecutar únicamente con ambiente, cuenta y archivos descartables autorizados. No registrar credenciales, cookies, rutas o contenido documental.

## Resultado E2E

El escenario real `normal` terminó `PASSED` con un único POST, efecto único y controles ODBC `SELECT` completos. El cliente normaliza la imagen a una fuente exclusiva —archivo o base64— antes de invocar el ASMX; el runner clasifica rechazos mediante códigos cerrados sin persistir el mensaje o cuerpo de respuesta.

La caracterización local adicional cubre rotaciones 0/90/180/270, huellas visuales reproducibles, tamaño 90 x 90, `/Linearized`, fallback `stream`, fallos de reemplazo/revalidación, rollback, locks y liberación de archivos. El caso real firmado no se ejecutó por inexistencia del fixture y fue aceptado explícitamente como riesgo residual de cierre.

El runner admite el escenario protegido `signed`: espera el rechazo cerrado del certificado, compara hash, longitud y `Last-Modified` del PDF antes/después, valida la huella `SELECT` y falla si detecta cualquier cambio. No debe ejecutarse sobre un documento no descartable.
