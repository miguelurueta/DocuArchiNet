# DOC-117 ALM-03: diagnóstico de guardas preparado, pendiente de permiso real

El diagnóstico previo al escritor está compilado y probado localmente. Su objetivo es obtener el código de la guarda individual que rechace, junto con comprobaciones booleanas del contexto y los argumentos. La causa histórica sigue sin determinarse. Este avance no ejecutó autenticación, SELECT reales, cargas ni instalación en el sitio real.

La evidencia real vigente continúa siendo `READ_VERIFICATION_COMPLETE`: **146/320 SELECT**, A confirmado con documento **16224**, inventario **23322** e intento **58**; individual sin documento del caso y B sin iniciar. Se conserva el informe independiente `doc117-alm03-detenido-verificado-20261106.md/.json`. Este diagnóstico usa otro nombre de archivo y otro diario; no continúa la pareja detenida.

## Alcance concreto para revisión

| Concepto | Propuesta pendiente de autorización |
| --- | --- |
| Ambiente | `https://localhost/GestionDocumental-Docuarchi.net/` |
| Identidad de aplicación | Usuario 141, empresa 2; sesión oficial existente |
| Destino individual | Expediente 467, nivel 8, relación 1072, tipología 50, gabinete 10 |
| Archivo temporal | `DOC117-ALM03-GUARD-DIAG-20261106.pdf`, 677 bytes |
| Envíos | Uno, por la UI oficial; detenerse antes de invocar el escritor |
| Documentos/ALMACENAR/importaciones | Cero |
| Planes/lotes/recepciones/expedientes/materializaciones | Cero nuevos |
| Controles | Cuenta MySQL exclusivamente SELECT; hasta 160 adicionales, partiendo de 146, techo 306 acumulados |
| Runtime | Candidata de laboratorio temporal durante un máximo de 600 segundos; doble respaldo, ACL y guardián independiente |
| Recursos existentes | Preservar A, B, intento 41, intento 53, lotes y documentos previos, staging, temporal individual anterior y evidencia histórica |
| Repetición/limpieza | Ningún retry, replay, reconciliación, reparación ni borrado |

La subida temporal y el cambio temporal de DLL requieren un permiso nuevo y específico. Las autorizaciones anteriores no habilitan este ejecutor. No se creó otra concesión real. El JSON revisable es [doc117-alm03-guard-diagnostic-propuesto-20261106.json](doc117-alm03-guard-diagnostic-propuesto-20261106.json); sus banderas de ejecución, activación y subida permanecen en `false`.

## Comportamiento preparado

La candidata sustituye únicamente en una copia de laboratorio la llamada directa de `PreAlmacenaDocumentoProduccion` por el diagnóstico terminal. Reutiliza la guarda individual original con sus comparaciones estrictas. El observador del escritor queda desactivado incondicionalmente y las importaciones están fuera del alcance. Ninguna de las 912 entradas originales de compilación/recursos se modificó.

Antes de seleccionar el archivo y pulsar Guardar, el cliente exige en la preparación oficial una cabecera que vincula el servidor a la huella del alcance diagnóstico. Si falta, se detiene antes de subir el PDF. La DLL se verifica también antes del envío; el guardián independiente restaura la original si vence la ventana o termina el propietario.

La respuesta terminal esperada es `DOC117_ALM03_DIAGNOSTIC_WRITER_BLOCKED`, tanto si las guardas aceptan como si rechazan. El diario nuevo conserva solo protocolo, huellas, resultado, código cerrado y 17 booleanos: sitio, actor, empresa, llamador, relación, nivel, expediente seleccionado, workflow, gabinete, expediente del argumento, tipología, tipo de almacenamiento, descripción exacta/equivalente, clase, coincidencia de temporal y nombre. No registra valores de sesión, SID, etiquetas observadas, rutas, cuerpos HTTP o secretos.

La equivalencia de descripción por mayúsculas/espacios/normalización es informativa; no relaja la guarda original. La prueba sintética de una diferencia de mayúsculas reproduce un rechazo posible, pero **no identifica la causa del fallo real anterior**. Tampoco se comprueba separación de sesiones, entrada al método o concurrencia en esta prueba individual.

Un resultado incierto, evidencia ausente/inválida o identidades documentales inesperadas queda retenido sin repetir la solicitud. El recorrido restaura la original, reabre la sesión oficial y compara las lecturas previas/posteriores. Un cambio de negocio o de preservación detiene la corrida para revisión. El PDF temporal nuevo puede quedar conservado, igual que los diarios y respaldos. No se ejecuta limpieza.

## Validación local y huellas

- Compilación completa: `COMPILED_NO_WRITER_DIAGNOSTIC`, cero errores; candidata fuera de `bin`.
- Regresión conjunta final: **286/286 PASS**, con guarda nativa compilada, ocho variantes nativas, UI oficial con dobles, rechazo de permisos incompatibles, desenlaces inciertos sin retry y puente ODBC que bloquea escrituras.
- Ensayo aislado con los mismos bytes de candidata: PASS; el propietario terminó sin restaurar, el guardián independiente restauró y verificó ambos respaldos y transferencia de ACL. Ninguna activación del runtime real.
- `--check` y validación OpenSpec estricta: PASS, sin login ni consultas reales.
- DLL instalada original y respaldos históricos byte a byte iguales; 912 entradas originales intactas. Artefactos históricos de ejecución/lectura y certificado de comprobación final conservados por SHA.

| Artefacto | SHA-256 |
| --- | --- |
| Propuesta | `3915557c583d10f7b33d7e20f9b889598126c7ce646920ced59fd5de1e4aa785` |
| Alcance diagnóstico | `364bd1443cb90cedd66447d2e2ab6ba09225f5ff426bb1a2ccaa61ec2bbd77cc` |
| Código revisado | `901f59d2fa397a6b3b27982477fe7ab1cfe30d8a1298eef2022a7c2f48ca495a` |
| Candidata diagnóstica | `8952bd88c52e7375ad590ae955d9b157b782c9040bb0767915b43cffd05fc8eb` |
| DLL original | `4134f57f66fc259b5f33a553577c9a07e6bf176b2da027aaeeb8576325c282c5` |
| 912 entradas originales | `412a115d0a4e7d8b6b839aa5ba17082337fce14bb129332645db82509909d1d3` |

Certificado local: `doc117-alm03-guard-diagnostic-local-20261106.json`. Build: `.tmp/doc117-alm03-guard-probe-build-20261106/build-result.json`. Ensayo: `.tmp/doc117-alm03-lease-50bf9fcd817a7d73943e286722a0a324/rehearsal-result.json`. Regresión: `.tmp/doc117-alm03-guard-probe-final-regression-20261106.tap`.

Comprobación local sin efectos: `node tools/e2e/scripts/run-doc117-alm03-guard-diagnostic.cjs --check`. La ejecución real exige un archivo de autorización específico ligado a propuesta, alcance, candidata, ambiente y código; el perfil registrado continúa siendo `.profiles/doc111-importacion-carpetas-lote-20261102.runtime.json`, sin credenciales persistidas.

DOC-117 permanece en **16/18**, con **17.1 y 18.1 abiertas**. El diagnóstico no concede PASS de ALM-03, serialización, concurrencia, aceptación XML/SII, cierre o archivo.
