# DOC-117 ALM-03: rechazo individual reproducido antes del escritor

El diagnóstico real autorizado terminó `DIAGNOSTIC_COMPLETE_NO_WRITER`, salida 0, el 2026-11-06 a las **20:21:21 UTC / 15:21:21 Bogotá**. Se identificó un rechazo de la **guarda de laboratorio** en la comparación literal de la descripción de tipología. El escritor no fue invocado y no se creó ningún documento.

## Hallazgo

La guarda `Doc117Alm03ClosedGuard.PrepararIndividual`, línea 85, compara `nombre <> "Comprobante de Egreso"` bajo `Option Compare Binary`. El diagnóstico capturó:

```json
{
  "outcome": "GUARDS_REJECTED",
  "code": "DOC117_ALM03_GUARD_INDIVIDUAL_ARGUMENTS_CHANGED",
  "writerInvoked": false,
  "descriptionExact": false,
  "descriptionEquivalent": true
}
```

Las otras 15 comprobaciones fueron verdaderas: sitio, actor 141, empresa 2, llamador, relación 1072, nivel 8, expediente seleccionado, workflow, gabinete, expediente del argumento 467, tipología 50, tipo de almacenamiento, clase documental, coincidencia de temporal y nombre del archivo. Solo falló la descripción literal. Su comparación equivalente aplica normalización Unicode NFKC, compactación/recorte de espacios y comparación sin distinguir mayúsculas.

Esto confirma una diferencia de representación en la descripción admitida por la selección oficial, frente al literal de la guarda. **No se conservó el texto recibido y no se puede distinguir si cambió mayúsculas, espacios, Unicode o una combinación.** La guarda original no fue relajada por el diagnóstico.

El helper UI recibió HTTP 200 y el marcador terminal previsto `PRODUCCION_DOCUMENTAL_E2E_DIAGNOSTIC_WRITER_BLOCKED`: una solicitud de PDF, sin identidades documentales. Ese bloqueo del escritor es el comportamiento esperado del diagnóstico, no un fallo de escritura. No hubo ENTER/EXIT de escritor ni prueba de almacenamiento documental.

## Preparación inicial y continuación

La sesión aislada `7b23741c6623372c66e79ba854fa6f37` terminó `PREPARATION_VERIFIED`: sesión establecida, nivel 7 seleccionado y lote 22 accesible, sin SELECT de control, PDF o cambio de DLL. Login y Producción respondieron HTTP 200. Los campos enviados coincidían con la entrada efímera (`MATCHED`); Chrome no conservó el cuerpo del postback (`BODY_NOT_RETAINED`), pero la sesión protegida y la lectura funcionaron. Esa observación no determina la causa de la detención anterior `188de3a60bbbc41d97cd4ccf3fa04266`.

La continuación `00ee73257399269edc67a1d474e4e3a7` conservó propuesta, alcance, candidata nativa, efectos y techo aprobados. La concesión anterior no se modificó: se enlazaron por SHA la detención de cero efectos y la preparación verificada, con el código JavaScript corregido para conservar diagnósticos de preflight. La autorización original para un PDF temporal y activación diagnóstica seguía sin consumir; el usuario había indicado continuar tras revisar el fallo.

## Controles y cierre

| Control | Resultado verificado |
| --- | --- |
| SELECT de esta continuación | 104: 52 antes y 52 después |
| Acumulado | **146 + 0 + 104 = 250**, dentro del techo 306 de esta autorización y del techo anterior 320 |
| Subidas temporales | Una, PDF de 677 bytes con hash aprobado |
| Escritor / documentos nuevos / importaciones | Cero |
| Inventario global | 8814 → 8814 |
| A | Intento 58 CONFIRMADO, documento 16224/inventario 23322, incorporación e índice EEE verificados |
| B | Archivo 37 NO_INICIADO, sin nuevo intento ALMACENAR ni envío |
| Preservación | Huellas previas/posteriores iguales para lote, documento físico A, staging, destino/documentos individuales y recursos anteriores |
| Runtime | DLL original restaurada, ambos respaldos idénticos, reapertura autenticada con original y finally verificados |
| Fuentes | 912 entradas originales intactas; candidata nativa sin cambios |
| Temporales/evidencia | PDF nuevo, temporal anterior, diarios, staging y respaldos conservados; sin limpieza |

Las guardas no aceptaron los argumentos, por lo que no se creó una reserva de entrada al escritor. Se verificó por separado que cualquier directorio de reserva de esta corrida no contenga entradas/salidas de escritor. La comparación completa de baseline fue idéntica antes y después.

## Límites y siguiente corrección

La nueva prueba confirma un rechazo actual reproducible y compatible con la falta de entrada individual del caso histórico. **No recupera el error que aquella corrida descartó**, ni permite afirmar retrospectivamente que fuera su única causa. El fallo de preparación inicial posterior tampoco se reprodujo: esta vez la sesión/nivel/lote funcionaron.

La corrección siguiente debe revisar cómo la guarda de laboratorio vincula la descripción canónica real de tipología 50 y cómo los controles construyen sus valores esperados. No se debe cambiar la tipología, el expediente o la clasificación para sortear el rechazo, ni atribuirlo al escritor productivo. Este avance no modifica todavía esa guarda ni instala una candidata corregida.

El diagnóstico no envió B, repitió A, reconcilió o reparó intentos anteriores. No acredita concurrencia o serialización. **DOC-117 permanece 16/18; 17.1 y 18.1 abiertas**, sin cierre/archivo o aceptación XML/SII. Cualquier prueba futura debe partir del acumulado **250**, conservar este caso y definir su propio alcance; no reiniciar contadores ni reutilizar estas concesiones para otro envío.

## Evidencia

- Preparación: `tools/e2e/artifacts/doc117-alm03-preparation-7b23741c6623372c66e79ba854fa6f37.json`.
- Corrida diagnóstica: `tools/e2e/artifacts/doc117-alm03-guard-probe-00ee73257399269edc67a1d474e4e3a7.json`.
- Prueba nativa: `App_Data/DOC117-ALM03-GUARD-DIAG-148776b731119b39ff896564a60c5a6f/probe.json`.
- Lease y respaldos: `.tmp/doc117-alm03-guard-probe-lease-148776b731119b39ff896564a60c5a6f/`.
- Concesiones/continuación: `.tmp/doc117-alm03-preparation-authorization.json` y `.tmp/doc117-alm03-guard-probe-continuation-authorization.json`.
- Certificado independiente con SHA: `doc117-alm03-guard-diagnostic-verificado-20261106.json`.

Las 289 pruebas de la corrección de preflight permanecen PASS y la nueva preparación aislada pasó 10 pruebas enfocadas, sin sumar resultados. `--check` de ambos ejecutores y OpenSpec estricto PASS después de la ejecución. Ningún secreto, cookie, token, SID, cuerpo HTTP, descripción observada o conexión se incluye en este informe.
