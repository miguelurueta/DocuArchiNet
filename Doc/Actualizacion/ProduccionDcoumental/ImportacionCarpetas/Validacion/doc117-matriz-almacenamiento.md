# DOC-117 — Matriz vigente de almacenamiento, 2026-11-06

| Caso vigente | Evidencia propia | Alcance |
| --- | --- | --- |
| ALM-01 PASS real | doc117-alm01-pass-20261105.md/.json | Valores finales, originales e incorporación; no repetir |
| ALM-02 PASS real de laboratorio | doc117-alm02-interno-pass-20261106.md/.json | Ventanas A/B; intento53 EN_CURSO/público INCIERTO preservado |
| ALM-03C PASS real de laboratorio | doc117-alm03c-coexistencia-pass-20261106.md/.json | Tres documentos, solapamiento99,7347ms, originales/preservación e incorporación física; original restaurado/reabierto |


## Resultado vigente — ALM-03C verificado

PASS real con variante de laboratorio: A16225/23323/intento63, individual16226/23324 y B16227/23325/intento64. Solapamiento interno99,7347ms, tres incorporaciones físicas y preservación posterior verificadas. Lote23 COMPLETADO, raíces37/38 COMPLETADA; consolidación interna propia de ALMACENAR, sin operación extra. DLL original restaurada/reabierta, dos respaldos nuevos y cuatro históricos idénticos,912 entradas originales y staging de lotes22/23 intactos.

Contador339+104+50+88=**581/659**; los cierres solo SELECT mantienen el mismo presupuesto. Fallo inicial de reapertura INPUT_REQUIRED y error posterior del control EN_CURSO conservados por SHA y resueltos mediante comprobaciones independientes, sin repetir documentos. La lectura incompleta de PersistirResultadoDocumento omitía ConsolidarEnTransaccion en su llamador; las notas anteriores de ausencia de consolidación quedan corregidas. Regresión final365/365 y OpenSpec estricto PASS. Informe/certificado doc117-alm03c-coexistencia-pass-20261106.md/.json.

Trazabilidad D-06/RQ-06/17.1 y D-08/RQ-08/18.1 completadas al consolidar esta evidencia. No se acredita concurrencia XML/SII, commits simultáneos o rendimiento ordinario. Casos antiguos e incertidumbres41/53 preservados, sin replay/reparación/reconciliación/limpieza. El checklist local termina18/18; publicación, revisión, sincronización/archivo OpenSpec y transición Jira son operaciones posteriores separadas.


## Registro histórico del refinamiento y sus avances

Los estados NO_EJECUTADO/NO_ACREDITADO y presupuestos siguientes pertenecen a sus momentos históricos; el resultado vigente es la tabla superior. Sus artefactos primarios permanecen inmutables.


Esta matriz conserva el refinamiento y añade resultados posteriores con su procedencia. ALM-01 tiene una corrida real acreditada el 2026-11-05 y las dos ventanas internas ALM-02 tienen PASS real con variante de laboratorio el 2026-11-06. ALM-03 se ejecutó parcialmente y quedó NO_ACREDITADO por pareja incompleta: solo A tiene entrada/salida interna. Lectura final completada, 146/320 SELECT acumulados: A CONFIRMADO, ningún documento individual del caso y B NO_INICIADO. DLL original y ambos respaldos restaurados/verificados; originales, clasificación y recursos previos conservados. Los reportes y apartados históricos mantienen sus SHAs y límites originales; sus declaraciones NO_EJECUTADO o lectura pendiente reflejan el momento anterior a los resultados posteriores. Estado actualizado el 2026-11-06, progreso 16/18.

| Caso/afirmación | Evidencia inspeccionada | Alcance y trabajo restante |
| --- | --- | --- |
| ALM-01: índices, tipología, nombre, PDF, vínculo e inventario | Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-rec-lote-pass-20261102.md; plan 6/lote 4, documentos 16208/16209, inventarios 23297/23298 | REAL histórico, código 96c3fafdd5e6254da8a6f3fbcb994908078cdd8ebff30c1ef1809ea4a6bf8e3d; no ejecución del commit actual |
| ALM-01: ceros, DECIMAL, fecha y visibilidad de NOMBRE | tests/ImportacionCarpetasBehaviorTests.cs DocumentVerificationCases; Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-adaptador-documento-local-20261030.md | Núcleo productivo con SELECT sintéticos; relacionar cada caso con la evidencia y ejecutar regresión local |
| ALM-01: formatos A/B | Snapshot Jira DOC-117 y soporte DOC-114 de preparación | La preparación no acredita almacenamiento final; proponer únicamente combinaciones finales no acreditadas |
| Original retenido y copia consumible | AdaptadorDocumentoImportacion.Almacenar; AlmacenStagingImportacion.CrearCopiaTrabajo; DocumentAdapterCases | Código/local histórico: retención, tamaño/hash y copia con nombre original; no exclusión global de escritores externos |
| ALM-02: lectura final del documento conocido | Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-lote2-verificacion-pass-20261102.md; documentos 16204/16205 | REAL histórico SOLO LECTURA y reapertura; no demuestra un nuevo fallo interno ni replay mutante |
| ALM-02: fallos de writer/verificador/checkpoint y retorno perdido | DocumentAdapterCases/DocumentPhaseCases; Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-fase-documento-local-20261030.md | LOCAL histórico con dobles explícitos; ventanas internas documentales reales no acreditadas por estas pruebas |
| ALM-02: fallo después de checkpoint y antes de checkpoint, ventanas exactas A/B | doc117-alm02-interno-pass-20261106.md/.json; lote 21, intentos 52/53, documentos 16222/16223 | REAL con variante de laboratorio propia: A confirmado/replay sin nuevo escritor; B EN_CURSO/público INCIERTO sin repetición; originales, relaciones, runtime original y recursos previos verificados. No acredita caída real ni ALM-03. |
| ALM-02: reconciliación | CoordinadorImportacionCarpetas.Reconciliacion.vb; ReconciliationCases; Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-reconciliacion-local-20261030.md | Verificación por IDs; INCIERTO con prueba suficiente. EN_CURSO, falta de IDs o incorporación discordante no autorizan reparar ni repetir |
| ALM-03: carga individual en degradación | Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-degradacion-individual-pass-20261102.md; documento 16212/inventario 23301 | REAL histórico durante esquema degradado; no importación simultánea. Código be095c78d774dc7f2d8b5fafb7f7dfb68c5eee0fe6623c8a9c8c94728e9a9a1e |
| ALM-03: solapamiento real | doc117-alm03-detenido-verificado-20261106.md/.json; corrida parcial 149655bd16ab32f957ddae7d44192109, A/documento 16224/inventario 23322/intento 58 | NO_ACREDITADO: solo un escritor observado, sin marcas individuales o resumen completo. Lectura final: A CONFIRMADO y físicamente verificado, individual sin documento del caso, B NO_INICIADO/sin intento. 146/320 SELECT, originales/recursos previos preservados, DLL original y respaldos verificados; sin afirmar serialización o XML/SII concurrente. |
| Negativos | DocumentAdapterCases/DocumentCatalogCases/DocumentVerificationCases | LOCAL histórico con dobles: recurso ajeno, nombres, relación, TIFF y metadatos. Reutilizar antes de añadir pruebas |

El refinamiento documental inicial no reprodujo fallos ni identificó un defecto que justificara modificar producto. La corrida posterior ALM-02 inyectó las dos ventanas acordadas mediante su propia variante documental, sin modificar el almacenamiento compartido. Las variantes de DOC-116 instrumentaban creación: sus PASS no se trasladan a ALM-02. El lote 18/intento 41 queda fuera del alcance de mutación de DOC-117.

## Revalidación local actual

`node --test tests/doc110-perfil-importacion-behavior.test.cjs`: 2/2 PASS el 2026-11-05, escritor/persistencia dobles y SELECT sintéticos. Ver doc117-local-20261105.md/.json para cada afirmación y huella. Se revalidaron original, replay, índices, incertidumbre, checkpoint, reconciliación y rechazos; la igualdad de bytes del original y ambas extensiones TIFF ahora tienen aserción explícita.

## Trabajo abierto

- 6.1 completada offline: dos manifiestos exactos y valores esperados A/B, con 8/8 pruebas locales. La lectura vigente y ausencia de raíces siguen siendo compuertas antes de ejecutar.
- 15.1 completada: ALM-01 real PASS el 2026-11-05; ver el apartado final y su evidencia. No repetir cargas acreditadas.
- 16.1 completada: ALM-02 interno real PASS con autorización propia, candidata temporal y restauración/reapertura verificadas; ver evidencia final del 2026-11-06.
- 17.1: ALM-03 real con observación de solapamiento y autorización propias.
- 18.1: cierre documental y gobierno completos después de resolver o acordar explícitamente los límites anteriores.

La consola run-doc117-publications-readonly.cjs se ejecutó con autorización de lectura el 2026-11-05: ver el apartado siguiente. No prepara planes, no pide MySQL y no autoriza efectos.

## Lectura autenticada de publicaciones

PASS: publicaciones 26/40 y 27/42, reglas coincidentes; snapshots y procedencia en doc117-publicaciones-lectura-pass-20261105.json/.md. Cero preparaciones, confirmaciones, recepciones y documentos. La ausencia de raíces y autorización de carga siguen sin verificarse/concederse.

## Preparación del driver ALM-01

Driver de una raíz por formato y preflight SELECT implementados; 65/65 pruebas locales y 85/85 en aislamiento, sin sumar ambas cuentas. Detalle y huellas: doc117-alm01-driver-local-20261105.md/.json. Ese reporte registra el estado anterior al preflight y a la carga real descritos a continuación; sus límites históricos se conservan.

## Preflight SELECT real

PASS: ausencia de ambas raíces/planes/expedientes y disponibilidad de publicaciones, tipología y gabinete/índice. Evidencia: doc117-alm01-preflight-pass-20261105.json/.md. Cero efectos; no habilita la carga. Propuesta de alcance para autorización ALM-01: doc117-alm01-autorizacion-propuesta-20261105.json.

## ALM-01 real completado

PASS con publicaciones 26/40 y 27/42, planes 35/36, lotes 19/20, expedientes 662/663, documentos 16220/16221 e inventarios 23318/23319. Índices efectivos derivados de identidad, originales físicos, replay y reapertura verificados; recursos previos conservados. Evidencia y huellas: doc117-alm01-pass-20261105.md/.json. ALM-02 y ALM-03 reales continúan NO_EJECUTADO.

## Preparación local ALM-02 — 2026-11-06

Candidata **desactivada** compilada con sustitución local del binding documental, fuera del proyecto de producto. Rechaza cualquier almacenamiento antes del escritor porque sus IDs de alcance son inválidos. No instalar esta DLL. El binding, la DLL del sitio y los 912 archivos de entrada del proyecto conservaron sus hashes. No se modificó `ClassAlmacenamiento`, SII, configuración, permisos ni recursos reales.

Las dos pruebas del soporte nuevo pasaron, incluyendo compilación/ejecución del helper VB con dobles locales: rechazo del alcance desactivado, actor/destino/archivo/intento ajenos, ventana posterior al checkpoint, ventana anterior al checkpoint, IDs discordantes y repetición del marcador. Otras 24 pruebas existentes pasaron en una corrida independiente. No se suman estos resultados a los reportes históricos para aparentar una misma corrida.

Detalle: `doc117-alm02-preparacion-local-20261106.md/.json`. La candidata no tiene recursos reales cerrados ni protocolo de activación/restauración DOC-117 compuesto; no está lista para instalación. La tarea 16.1 continúa pendiente. ALM-03 sigue pendiente de observación segura del solapamiento y autorización. La tarea 18.1 permanece abierta hasta resolver ambos escenarios o aceptar explícitamente un alcance reducido.

## Preflight ALM-02 SELECT real — 2026-11-06

**PASS, 8/8 SELECT**, con autorización específica del usuario para esta lectura. Publicación 38/gabinete 10/DA y política COMPLETO_OBLIGATORIO/LOTE verificadas; cinco reglas, código/nombre de carpeta completos, ausencia de expedientes/planes/raíces para los nombres propuestos e índices visibles concordantes. Intento 41/lote 18 observado EN_CURSO sin actuación. Sin login de aplicación ni efectos; DLL del sitio intacta.

Evidencia: `doc117-alm02-preflight-pass-20261106.md/.json`. No acredita permisos, tipología, valores finales, fallos internos, restauración o ALM-03; no concede autorización de carga o instalación. La candidata desactivada continúa sin ser instalable. No se completa la tarea 16.1 con este preflight y el progreso sigue 15/18.

## Restauración local aislada — 2026-11-06

PASS de ensayo con bytes reales de original/candidata desactivada en un sitio temporal bajo `.tmp`: respaldo doble byte a byte, ACL, guardián en proceso independiente y restauración después de la salida del dueño sin `finally`. Se conserva la DLL real. El límite de diez minutos se verificó con reloj inyectado; no se presenta como espera real de esa duración. Las 16 pruebas iniciales de lease DOC-116/DOC-117 pasaron.

Evidencia: `doc117-restauracion-local-20261106.md/.json`. La mecánica compartida se extrajo del soporte existente manteniendo gates propios. No se realizó instalación, carga, autenticación o nueva consulta real. La variante específica sigue pendiente y 16.1 no se completa.

El ID del intento documental nace dentro de `AvanzarLote` y el histórico no reejecuta el adaptador. El usuario aprobó editorialmente `doc117-alm02-ajuste-alcance-propuesto-20261106.md` mediante «aprobado» el 2026-11-06. El soporte local ya fija ese ID atómicamente antes del inicio/escritor, vinculado a solicitud y recursos cerrados. El gate LIVE exige el protocolo aprobado, el alcance completo y las tres autorizaciones propias; no se predicen intentos.

## Fijación del intento aplicada localmente — 2026-11-06

Evidencia `doc117-fijacion-intento-local-20261106.md/.json`: helper VB probado con el comando inmutable real; huella .NET contrastada contra la asignada por el coordinador con dobles; carrera de dos inicios con una sola aceptación; registro incompleto/no creable/desaparecido e intento ajeno rechazados; ventanas A/B conservan IDs. La copia del flag transitorio `EsNuevaReserva` no se presupone: se respeta su validación de entrada en el comando de producto.

Candidata desactivada nueva compilada y restauración aislada repetida con guardián independiente. DLL real y los 912 archivos de entrada del proyecto intactos. La herramienta de cálculo recibe datos por stdin y emite solo hashes. La aprobación editorial no habilita consultas nuevas, cargas, preparación de recursos ni instalación. 16.1–18.1 siguen pendientes.

Verificación conjunta final: 48/48 pruebas locales PASS y OpenSpec estricto PASS. El driver de preparación real quedó implementado/probado pero **NO_EJECUTADO**: propuesta y alcance en `doc117-alm02-setup-listo-20261106.md` y `doc117-alm02-recursos-propuestos-20261106.json`, con certificado local `doc117-alm02-setup-local-20261106.json`. Solicita separadamente hasta un plan/lote, dos recepciones/expedientes/materializaciones y 160 SELECT; cero almacenamiento documental o instalación. No se considera otorgada esa autorización.

## Preparación ALM-02 real posterior — 2026-11-06

El usuario autorizó esa preparación y se ejecutó con **PASS**: un solo plan 37/lote 21, dos originales de 677 bytes, expedientes 664/665 y dos materializaciones. La primera ejecución se detuvo al comparar las reglas; se corrigió el soporte reutilizando el comparador existente de snapshots publicados y se continuó el mismo plan, sin nueva preparación. Total **100/160 SELECT**; cero solicitudes ALMACENAR/documentos definitivos/cambios de DLL.

Originales, IDs, valores, ausencia de documentos y recursos previos verificados. El baseline de preservación cubre la continuación antes/después, sin atribuir una huella persistida entre ambos procesos. Evidencia `doc117-alm02-setup-pass-20261106.md/.json`. Regresión actual 51/51 PASS y OpenSpec estricto PASS. El estado NO_EJECUTADO del apartado anterior corresponde a la revisión previa a la autorización.

Candidata cerrada de lote 21 compilada localmente y restauración de esos mismos bytes ensayada en sitio temporal con guardián independiente; no instalada. La preparación no acredita las ventanas internas ALM-02, ALM-03 o el cierre. 16.1–18.1 permanecen pendientes y los recursos se conservan; no repetir la preparación ni gastar su autorización en nuevas cargas.

## Ejecutor interno revisado localmente — 2026-11-06

`doc117-alm02-interno-listo-20261106.md` y `doc117-alm02-interno-local-20261106.json` cierran el soporte revisable del lote 21, archivos 34/35, expedientes 664/665. Se añadió observación durable de entrada al escritor, prueba física por IDs para B sin alterar su estado, orquestación con restauración aun ante fallo de cierre de sesión y rechazo de autorizaciones de otra fase. Regresión local ampliada 134/134 PASS; OpenSpec estricto PASS.

La nueva candidata `85824b255a826664a8395754d8903e51218ba1a8303595ee6e181c8c0dc05a95` está compilada y no instalada. Ensayo de copia de sus bytes en sitio temporal PASS con guardián independiente, ambos respaldos y ACL verificados; no se traslada al runtime real. El manifiesto propuesto sigue sin concesión: tres solicitudes ALMACENAR (A, replay A confirmado y B una vez), dos documentos, 240 SELECT y 600 segundos. Ninguna ejecución autenticada, SELECT real, instalación o carga nueva en este avance. ALM-02 interno/ALM-03 reales siguen NO_EJECUTADO; progreso 15/18.

## ALM-02 interno real autorizado — resultado posterior 2026-11-06

PASS con autorización propia `8acc6c159c162668302fb82905560118`: lote 21, archivos 34/35 y expedientes 664/665. A/intento 52 confirmó documento 16222 e inventario 23320 pese al fallo posterior al checkpoint; replay exacto sin cambios de IDs, eventos, prueba física o marca de escritor. B/intento 53 produjo documento 16223/inventario 23321 e incorporación verificable, sin checkpoint final: EN_CURSO persistido y respuesta INCIERTO. No se repitió/reconcilió/reparó B.

Tres solicitudes ALMACENAR, dos entradas al escritor, dos documentos; inventario actor/empresa 8811 → 8813; 191/240 SELECT. Originales de 677 bytes/hash esperado conservados, tipología 79 e índice 14 TEXTO `EEE` verificados. Candidata temporal retirada, DLL original `4134f57f…` y ambos respaldos verificados; reapertura con original y controles repetidos. Huellas de recursos anteriores coincidentes, incluidos lote 18/intento 41 y lotes 19/20. Evidencia con SHAs: `doc117-alm02-interno-pass-20261106.md/.json`.

Se completa 16.1, progreso 16/18. ALM-03 y consolidación final siguen pendientes; no hay cierre/archivo ni aceptación implícita de concurrencia, XML o SII. Los estados NO_EJECUTADO anteriores corresponden a sus respectivas etapas históricas.

## Observabilidad ALM-03 — revisión local posterior

Revisión de llamadores y analizador local: 23/23 PASS, sin promover solapamiento de intervalos externos a ejecución interna del escritor. Un ensayo de instrumentación CLR únicamente sintético obtuvo captura y cerró su sesión; no se verificó todavía extracción por invocación/hilo/recurso. No se ejecutó ALM-03 de aplicación ni se accedió a IIS/cuentas/base de datos; la DLL original y las 912 entradas productivas siguen intactas.

Detalle: `doc117-alm03-observabilidad-local-20261106.json`. Decisión editorial propuesta en `doc117-alm03-observacion-propuesta-20261106.md`: excepción acotada de observación dentro de una copia del escritor exclusiva de laboratorio. Aún no aprobada/aplicada; no se rebaja RQ-06 ni se instalan marcas por inferencia. Progreso 16/18, pendientes 17.1–18.1. La prueba sintética y sus errores iniciales no acreditan coexistencia real, XML o SII.

## Observación interna ALM-03 aprobada y desarrollada localmente

El usuario aprobó la excepción editorial posterior. D-06/D-08/RQ-06 ya la reflejan; se construyó una envoltura interna sobre copia fuera del proyecto y un núcleo probado con escritor sintético. Regresión conjunta 184/184 PASS, candidata desactivada compilada y restauración de sus bytes en sitio temporal con guardián independiente PASS. DLL instalada y 912 entradas originales conservadas. Evidencia `doc117-alm03-metodo-local-20261106.md/.json`.

El analizador interno distingue solapamiento del método de serialización, sin otorgar aceptación de integridad o ALM-03 real solo por tiempos. La candidata rechaza siempre antes del cuerpo documental: faltan recursos/huellas reales, binding activo de llamadores y ejecutor cerrado. No hubo nuevas cargas, SELECT reales, autenticación o instalación. 17.1/18.1 permanecen pendientes, progreso 16/18; no se afirma concurrencia ordinaria, commits simultáneos, XML o SII.

## Lectura ALM-03 y preparación local posterior

Doce SELECT reales autorizados, sin efectos: ocho del preflight que detuvo la coincidencia individual/nivel 7, y cuatro de diagnóstico que localizaron expediente 467/relación 1072 en nivel 8, tipología 50 compatible y estados protegidos 41/52/53 conservados. El preflight completo no se aceptó y el diagnóstico no acredita autoridad/UI ni coexistencia. Ver `doc117-alm03-destino-lectura-20261106.md/.json`. Runtime original intacto.

Soporte de preparación propio listo localmente: importación en nivel 7 y verificación del destino individual existente en 8, con facultades/clasificación/gabinete/nombre comprobados antes de efectos. Propuesta de un plan/lote, dos recepciones, dos expedientes/materializaciones, cero documentos/ALMACENAR/DLL y hasta 160 SELECT nuevos. No ejecutada ni concedida aún. Regresión conjunta 221/221 PASS y OpenSpec estricto PASS; `doc117-alm03-setup-listo-20261106.md` y `doc117-alm03-setup-local-20261106.json`. Progreso 16/18, 17.1–18.1 pendientes.

## Preparación ALM-03 real autorizada después

PASS: 80/160 SELECT, plan 38/lote 22, raíces 35/36, archivos 36/37, expedientes 666/667 y relaciones 1091/1092. Dos recepciones y dos materializaciones; originales de 677 bytes/hash esperado, tipología 79/índice 14 `EEE` y ausencia documental verificados. Destino individual 467/nivel 8/1072 comprobado antes/después sin carga. Se comparó preservación de recursos anteriores en memoria; runtime original y 912 entradas intactos. Cero documentos, ALMACENAR, carga individual o instalación. Evidencia `doc117-alm03-setup-pass-20261106.md/.json`.

El alcance real y dos comandos futuros están sellados privadamente; aún no hay guardas activas/candidata/ejecutor de coexistencia revisados ni autorización de sus efectos. Esta preparación no acredita solapamiento. 17.1/18.1 pendientes, progreso 16/18; conservar todos los recursos y no repetir la preparación.

## Coexistencia ALM-03 lista localmente, sin ejecución real

Guardas activas cerradas y ejecutor completo preparados para los recursos de lote 22 y carga individual en expediente 467/nivel 8. La candidata de laboratorio `142e0d0b…` compila sin errores y su ensayo de restauración aislado PASS verificó guardián independiente, doble respaldo y ACL. Regresión final 238/238 PASS: método interno, comandos reales/sesiones en .NET, UI oficial con dobles, fallos sin retry, IDs retenidos antes de proyección, controles por identidades, índices/classificación y preservación. `--check` PASS y OpenSpec estricto PASS. Las 912 entradas productivas originales y la DLL instalada siguen intactas.

Evidencia `doc117-alm03-coexistencia-local-20261106.json`; alcance revisable `doc117-alm03-coexistencia-lista-20261106.md` y propuesta cerrada propia. No hubo login, SELECT real, nueva carga, documento o instalación en este avance. Tres cargas/documentos, 320 SELECT nuevos y candidata temporal por 600 segundos requieren permiso específico todavía no concedido. El setup anterior limita esos efectos a cero. ALM-03 de aplicación sigue **NO_EJECUTADO**; serialización futura será NO_ACREDITADO sin repetir. Progreso 16/18; 17.1/18.1 abiertas, sin aceptación XML/SII ni cierre por evidencia sintética.

## Autorización y corrida parcial ALM-03 posteriores

El usuario autorizó expresamente mediante «autorizado» el alcance revisado. Primer preflight detenido sin efectos a 32 SELECT: exigía erróneamente un mapeo opcional. Tres SELECT diagnósticos confirmaron cero mapeos predeterminados/de fecha y seis campos estándar visibles. Corrección local y continuación dentro de la misma autorización, mismos recursos y candidata, contador inicial de 35 y techo acumulado 320. Regresión 246/246 y verificación enfocada 17/17, sin sumar ambos resultados; `doc117-alm03-control-opcional-20261106.md/.json`.

La continuación solicitó A y la carga individual y se detuvo con `DOC117_ALM03_PAIR_RESULT_UNCERTAIN`. Solo A tiene marcas internas ENTER/EXIT SUCCESS, documento 16224/inventario 23322/intento 58/expediente 666/relación 1091; su checkpoint durable todavía no se afirma. No hay marcas individuales, resumen completo o despacho de B. El detalle del rechazo individual no fue retenido: no se inventa la causa ni se repite el almacenamiento. Acumulado 93/320 SELECT antes de lectura posterior. Runtime original restaurado en finally y ambos respaldos idénticos; 912 entradas originales intactas.

Evidencia `doc117-alm03-detenido-20261106.md/.json`. La consola de comprobación final espera exclusivamente credenciales MySQL de lectura, para verificar lo persistido y la preservación sin efectos nuevos. No se afirma todavía ausencia documental individual, estado durable posterior de B o invariancia posterior de los datos. ALM-03 **NO_ACREDITADO por pareja incompleta**, sin inferir serialización. 16/18; 17.1/18.1 abiertas, sin cierre o archivo. Los permisos pendientes descritos en apartados anteriores corresponden a su fecha histórica.

## Lectura final ALM-03 completada — estado vigente

El usuario completó la consola y el artefacto `doc117-alm03-post-stop-149655bd16ab32f957ddae7d44192109.json` terminó `READ_VERIFICATION_COMPLETE` a las 16:11:34 UTC. Se consumieron 53 SELECT adicionales: **32 + 3 + 58 + 53 = 146/320**, sin nuevas cargas, login de aplicación o instalación durante la comprobación. A/intento 58 está **CONFIRMADO**, documento 16224/inventario 23322 con incorporación única e índice 14 TEXTO `EEE`; ambos originales de staging conservados. Individual sin documento con el nombre del caso, conteos 9/11/11 y documentos previos/clasificación/configuración/valores esperados de seis índices estándar iguales al preflight. B/archivo 37 permanece **NO_INICIADO**, sin intento ALMACENAR. Inventario global 8813 → 8814 (+1).

Se verificaron iguales las huellas de planes/lotes/raíces/archivos/intentos/eventos anteriores y se comprobaron expresamente 41 EN_CURSO, 52 CONFIRMADO y 53 EN_CURSO. DLL original, ambos respaldos idénticos, 912 entradas originales y temporal individual conservados. La comprobación independiente enlazó el artefacto de lectura con el de ejecución por SHA y contrastó conteos, etapas, identidades, preservación y marcadores antes de publicar `doc117-alm03-detenido-verificado-20261106.md/.json`.

El informe anterior de lectura pendiente queda como evidencia histórica, sin sustituir sus huellas. ALM-03 sigue **NO_ACREDITADO por pareja incompleta**: falta observación individual y resumen de coexistencia; el código específico de su fallo no se retuvo y no se infiere su causa. No hubo replay, reconciliación, reparación o envío de B. 17.1/18.1 abiertas, 16/18, sin cierre/archivo ni aceptación XML/SII. Conservar este lote y sus recursos; una nueva prueba requiere diagnóstico previo y un caso separado.

## Diagnóstico local posterior del ejecutor

Se reprodujo la pérdida del código específico individual: la UI emitía `error.code` y la pareja lo descartaba al producir el error genérico. La corrección conserva resultados por operación, códigos cerrados y metadatos de fase/HTTP/conteos/IDs/proyección, sin payloads ni secretos. Una promesa FULFILLED no sustituye confirmación de negocio. También se manejan rechazos pendientes cuando falla Guardar y se rechazan IDs inválidos antes de registrarlos. Ambos envíos se esperan y cualquier error detiene B sin retry.

Regresión final **270/270 PASS**; sin nuevas autenticaciones, SELECT reales, cargas o instalación. El escritor/candidata nativa, las 912 entradas originales, DLL instalada, respaldos y artefactos históricos permanecen intactos. Evidencia `doc117-alm03-diagnostico-codigos-local-20261106.md/.json`. La causa individual histórica sigue sin determinar; la mejora de diagnóstico solo aplica a futuras pruebas. La siguiente investigación requiere otro caso aislado y, si el catch legacy devuelve un código público genérico, observación acotada de guardas antes del escritor. No se preparó ni autorizó otra ejecución real; acumulado vigente **146/320**, NO_ACREDITADO, 16/18.


## Diagnóstico previo al escritor preparado localmente — pendiente de permiso específico

Se preparó una candidata separada que evalúa la guarda individual original y registra solo códigos cerrados y 17 booleanos; termina antes del escritor tanto al aceptar como al rechazar. El cliente exige una cabecera de preparación vinculada al alcance antes de subir el PDF. No se relaja ninguna comparación original ni se atribuye todavía la causa histórica. Ocho variantes nativas, controles UI/permisos/incertidumbre y regresión conjunta final **286/286 PASS**; compilación completa sin errores y ensayo aislado con los mismos bytes de candidata PASS mediante guardián independiente, doble respaldo y ACL. Las 912 entradas originales, DLL instalada, respaldos y evidencia histórica permanecen intactos. Ningún login, SELECT real, carga o instalación real durante este avance.

Revisión `doc117-alm03-guard-diagnostic-listo-20261106.md`, propuesta `doc117-alm03-guard-diagnostic-propuesto-20261106.json` y certificado `doc117-alm03-guard-diagnostic-local-20261106.json`, en `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/`. Alcance propuesto: localhost, usuario 141/empresa 2, expediente 467/nivel 8/relación 1072/tipología 50, un PDF temporal distinto de 677 bytes, cero documentos/ALMACENAR/importaciones/recursos nuevos, hasta 160 SELECT adicionales desde 146 (máximo 306 acumulados) y DLL diagnóstica temporal por 600 segundos. Ningún retry, replay, reconciliación, reparación o limpieza. Requiere permiso específico nuevo; todas las banderas públicas de ejecución/instalación/subida siguen false y no se creó otra concesión real. La causa y la concurrencia real siguen sin acreditar; el caso detenido conserva A 16224/23322/intento 58 confirmado, individual sin documento y B sin iniciar. **16/18, 17.1/18.1 abiertas**, sin cierre/archivo.


## Diagnóstico autorizado detenido antes de controles y runtime — estado vigente

El usuario autorizó expresamente el diagnóstico separado; corrida `188de3a60bbbc41d97cd4ccf3fa04266`, terminada con salida 2 el 2026-11-06 a las 19:58:42 UTC. `STOPPED_REVIEW_REQUIRED`, cero SELECT nuevos y **146 acumulados**, historial vacío, DLL original verificada; no existen lease/diario/reserva ni PDF temporal nuevo de esta corrida. No hubo activación de candidata, envío individual o repetición. El preflight inicial falló con `DOC117_ALM03_PROBE_STOPPED` / `UNCLASSIFIED_FAILURE`; no se retuvo el código específico, no se afirma credencial incorrecta o sesión establecida y no se identifica la causa. El último estado de negocio sigue siendo el de la lectura previa de 146 SELECT; esta corrida no lo releyó.

Se corrigió localmente el catch para conservar códigos cerrados DOC111/DOC116, observación saneada del helper y etapas de apertura/selección/lectura/baseline. Regresión conjunta posterior **289/289 PASS**, OpenSpec estricto y --check PASS; 912 entradas originales, DLL, respaldos, candidata nativa y evidencia histórica intactos. La concesión terminada no se modificó y se rechaza con el código nuevo; no hay segunda autenticación ni otra concesión creada. Informe/certificado `doc117-alm03-guard-diagnostic-detenido-20261106.md/.json`. El diagnóstico nativo no se alcanzó; pendiente clasificar y resolver la preparación antes de cualquier nueva ejecución. **16/18, 17.1/18.1 abiertas; ALM-03 NO_ACREDITADO**, sin cierre/archivo ni crédito de concurrencia/XML/SII.


## Diagnóstico real completo sin escritor — rechazo de descripción confirmado

La preparación aislada `7b23741c6623372c66e79ba854fa6f37` terminó PREPARATION_VERIFIED: sesión, nivel 7 y lote 22 accesibles, cero controles SELECT/efectos/runtime. El postback y Producción dieron HTTP 200; el cuerpo no retenido por Chrome no impidió la sesión y no identifica el fallo anterior. Continuación derivada del permiso original aún sin consumir `00ee73257399269edc67a1d474e4e3a7`: DIAGNOSTIC_COMPLETE_NO_WRITER, salida 0 a las 20:21:21 UTC del 2026-11-06, una subida temporal, cero documentos/escritor/importaciones, 104 SELECT adicionales (52+52), **250 acumulados** dentro del techo 306. Guardar ese acumulado para cualquier fase futura; no reiniciar presupuesto.

La guarda de laboratorio reprodujo DOC117_ALM03_GUARD_INDIVIDUAL_ARGUMENTS_CHANGED: descriptionExact=false y descriptionEquivalent=true, las otras 15 comprobaciones verdaderas. El literal Comprobante de Egreso bajo Option Compare Binary no coincide exactamente, aunque sí tras normalizar Unicode/espacios/mayúsculas. No se conserva el valor recibido y no se distingue cuál transformación es necesaria. WriterInvoked=false; el marcador UI WRITER_BLOCKED es el terminal esperado. No se afirma recuperar el error histórico ni atribuirlo al escritor productivo.

Baseline antes/después idéntico, inventario 8814→8814, A/intento 58 confirmado 16224/23322 e índice EEE, B/37 sin iniciar, staging/individual/recursos anteriores conservados. DLL original restaurada, doble respaldo byte a byte, reapertura original y finally verificados; 912 entradas originales y evidencia histórica intactas. Temporal nuevo y anterior, diarios y respaldos conservados; sin replay, reconciliación, reparación o limpieza. Informe/certificado vigente doc117-alm03-guard-diagnostic-verificado-20261106.md/.json. Pendiente corregir el vínculo de descripción canónica en guarda/controles y diseñar otro caso de coexistencia con alcance propio; este diagnóstico no prueba concurrencia o serialización. **16/18, 17.1/18.1 abiertas**, sin cierre/archivo/XML/SII.


## Descripción canónica corregida localmente, sin nueva ejecución real

Se implementó una variante de guarda que vincula por SHA-256 UTF-8 la descripción exacta del catálogo de tipología 50/expediente 467/empresa 2, en lugar del literal mixto. Normalización solo para validar el nombre humano aprobado; no reescribe argumentos ni relaja la comparación exacta preparación/entrada. UI con vínculo exacto antes de Guardar y controles canónicos que revalidan el catálogo y los valores físicos exactos; deriva de metadatos y rechazos anteriores preservados. Guarda/controles históricos conservan su camino de evidencia; la variante nueva se usa separadamente.

Compilación completa desactivada sin errores y regresión conjunta final **310/310 PASS**, incluyendo guarda .NET, representaciones de catálogo, negativos de identidad/selección/archivo/mutación y UI/controles. Se actualizó --check para comprobar evidencia existente byte a byte en vez de suponer que no existe. DLL instalada, cuatro respaldos, 912 entradas originales, candidata nativa y artefactos históricos intactos. Ningún login, SELECT real, PDF, instalación o concesión real nueva. Candidata local sin Site/Run/recursos/binding canónico reales, escritor/importaciones desactivados. Informe/certificado doc117-alm03-descripcion-canonica-local-20261106.md/.json.

Evidencia real vigente: **250 SELECT acumulados**, A 16224/23322/intento 58 confirmado, B sin iniciar; no repetir. La representación real del catálogo aún no se leyó/fijó con el protocolo nuevo. Para el siguiente caso faltan recursos, alcance/presupuesto y binding revisados propios; no reutilizar ni reiniciar los permisos consumidos. **16/18, 17.1/18.1 abiertas**, sin acreditar almacenamiento/coexistencia real posterior ni cierre/archivo.


## Preparación ALM-03C lista localmente — ampliación de presupuesto pendiente

Implementado y probado el ejecutor de preparación de un caso nuevo con nombres DOC117-ALM03C, niveles 7/8, mismos destinos autorizables y protocolo de alcance V2 con descripción canónica exacta del catálogo. Un plan/lote, dos recepciones PDF de 677 bytes, dos expedientes/materializaciones, cero documentos/ALMACENAR/carga individual/DLL. Rechaza IDs históricos y planes/lotes dentro del inventario preservado antes de recibir; compara catálogo, inventario global y documentos individuales antes/después, conserva todos los recursos y sella comandos futuros sin enviarlos. Los fallos conservan etapa, códigos/HTTP públicos cerrados e IDs; sin retry/resume/replay/reconciliación/reparación/limpieza.

Regresión final **333/333 PASS**, --check y OpenSpec estricto PASS; calculadora .NET copiada/verificada fuera de bin. DLL instalada, cuatro respaldos, 912 entradas originales, candidatas y evidencia histórica intactas. Ningún login, SELECT real o efecto nuevo. Propuesta pública aún sin conceder y ninguna autorización real creada. doc117-alm03c-preparacion-lista-20261106.md, doc117-alm03c-recursos-propuestos-20261106.json y doc117-alm03c-setup-local-20261106.json.

**250 SELECT acumulados**: se solicita ampliar expresamente el techo histórico **320→410**, máximo 160 adicionales desde 250; no se ejecuta con el saldo de 70 ni se reinicia contador. La recepción/creación de recursos y la ampliación requieren permiso específico. La ejecución documental/coexistencia y runtime quedan para otra revisión con IDs/binding reales y otro permiso/presupuesto. Estado real conservado: A 16224/23322/intento 58 confirmado, B/37 sin iniciar. **16/18, 17.1/18.1 abiertas**, sin cierre/archivo o nuevo crédito de concurrencia/XML/SII.


## Preparación ALM-03C real aprobada y completada — estado vigente

El usuario aprobó expresamente un plan/lote nuevo, dos PDFs de 677 bytes y dos expedientes/materializaciones, con cero documentos/carga individual/DLL, y amplió el techo 320→410 desde 250. Primera corrida b83e24a81417007ea8f92c54d3c624b1 detenida a tres SELECT antes de efectos por error del control: COUNT de toda la tabla comparado contra 8814, que correspondía a usuario 141/empresa 2. Corrección para usar los mismos filtros, conteos saneados durables y continuación solo con prueba de cero efectos y acarreo de las tres lecturas. Regresión **336/336 PASS**; no se afirma cambio de datos por el rechazo inicial. Las menciones previas a inventario global 8814 se aclaran como conteo de actor/empresa, sin reescribir artefactos históricos.

Continuación 30d7b62134015710dbb8c455c8723de6 PASS, salida 0 a las 22:27:58 UTC del 2026-11-06: plan 39/lote 23, raíces 37/38, archivos 38/39, expedientes 668/669, relaciones 1093/1094; creación confirmada en intentos 61/62. Dos recepciones/materializaciones, cero documentos/ALMACENAR/carga individual/runtime. Canonical binding exacto del catálogo fijado y revalidado en alcance V2; dos comandos futuros sellados privadamente y no enviados. **89/160 SELECT de la fase (3+86), 339/410 acumulados**; ningún reinicio de contador. Inventario actor141/empresa2 8814→8814 y documentos individuales 11→11 con huellas conservadas; preservación previa y originales verificados. Auditoría local independiente de originales de lotes22/23, DLL, cuatro respaldos, 912 entradas originales y evidencia histórica PASS; sin temporal individual nuevo en servidor.

Informe/certificado doc117-alm03c-setup-pass-20261106.md/.json. Alcance run71a43d1962ba3285b3801c4f2080934c, SHA9221008ec1566b27b7ee765e7ed8b770a2ba1b12bf42778eb5b40281d7d2ea19. No repetir la preparación, ni enviar comandos anteriores o nuevos sin su gate propio. Falta adaptar candidata/ejecutor al caso V2, compilar/ensayar y revisar otro permiso de documentos/runtime y presupuesto desde339; el saldo71 no los autoriza. **16/18, 17.1/18.1 abiertas**, sin crédito de coexistencia/serialización, cierre/archivo/XML/SII.


## Coexistencia ALM-03C lista localmente — ejecución pendiente de permiso propio

Adaptación al alcance canónico V2 del lote23 y expedientes668/669 completada. Candidata cerrada dc45234f… compilada fuera de bin sin errores y ensayo de restauración de sus mismos bytes PASS con guardián independiente, doble respaldo y ACL. Ejecutor revisado con cabecera de alcance antes de seleccionar PDF, descripción exacta del catálogo, pareja A/individual en sesiones independientes, B solo después de sus controles, resultados separados y registro nativo cerrado de rechazos. Los resultados parciales siguen al cierre/restauración y comprobación SELECT sin reenviar; no acreditan negocio o concurrencia.

Regresión final **355/355 PASS**, 41 archivos, --check y OpenSpec estricto PASS. Auditoría local de las 912 entradas originales, DLL instalada, cuatro respaldos, originales de staging de lotes22/23 y evidencia histórica PASS. La ruta ALMACENAR mantiene lote EN_CURSO y raíces EXPEDIENTE_CONFIRMADO aun al confirmar los documentos: el control verifica ese checkpoint y no añade consolidación ni otra mutación. Ningún login, SELECT real, documento, temporal individual del caso o activación del runtime real durante este avance.

Revisión doc117-alm03c-coexistencia-lista-20261106.md, propuesta doc117-alm03c-coexistencia-propuesto-20261106.json y certificado doc117-alm03c-coexistencia-local-20261106.json. Propuesta concreta: máximo tres documentos/solicitudes, candidata temporal600s y hasta320 SELECT nuevos desde339, ampliando expresamente techo410→659. Sin nuevos planes/lotes/recepciones/expedientes/materializaciones, retry/replay/reparación/reconciliación/limpieza. Serialización real implica NO_ACREDITADO sin otro intento. Banderas públicas false, ninguna concesión real de coexistencia creada.

La aprobación de setup cubrió cero documentos y cero cambios de DLL: no concede este alcance documental/runtime ni la ampliación. El acumulado real permanece **339/410** y la coexistencia nueva **NO_EJECUTADA**; A antiguo16224/23322/intento58 confirmado y B/archivo37 sin iniciar se conservan sin repetir. **16/18, 17.1/18.1 abiertas**, sin aceptación de concurrencia/XML/SII, cierre o archivo.
