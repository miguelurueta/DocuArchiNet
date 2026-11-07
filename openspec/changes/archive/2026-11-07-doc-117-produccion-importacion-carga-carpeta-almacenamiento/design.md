## Context

DOC-117, perfil legacy-webforms-vb. Revisión local: 2026-11-05. Base aislada: main 9a1bce2be9cfd640184269c0e02d6ef94ff181f6, con DOC-116 integrado; el snapshot Jira conserva su procedencia anterior sin reescritura. Ver proposal.md para motivación.

Se inspeccionaron los diez archivos enumerados en source-review.json. Comparación normalizada: 10/10 coinciden con la copia aislada. Esta comparación no acredita ejecución del binario actual. No se realizaron nuevas pruebas autenticadas.

## Goals / Non-Goals

Goals: matriz ALM por afirmación, pruebas faltantes acotadas y reutilización del escritor/verificador únicos.

Non-Goals: reimplementar almacenamiento, SII, aislamiento XML descartado, TIFF, workers, DDL, permisos/configuración, limpieza automática o rollback global. El intento 41 del lote 18 de DOC-116 queda preservado EN_CURSO/public INCIERTO y fuera de toda mutación.

## Decisions

### D-01 — Fuente y procedencia

La copia principal local prevalece; cada evidencia conserva SHA, fecha, ambiente y tipo real/local. Una divergencia con la copia aislada se revisa sin sobrescribir producto.

Evidencia: Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorDocumentoImportacion.vb; tests/ImportacionCarpetasBehaviorTests.cs.

### D-02 — Original y llamada única

Reutilizar comando, staging, adaptador y binding existentes: retener el original verificado, entregar copia privada consumible con nombre original e invocar legacy una vez por reserva válida.

Evidencia: Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorDocumentoImportacion.vb Almacenar; Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorDocumentoImportacion.Legacy.vb InvocarAlmacenamiento; Infrastructure/ProduccionDocumental/ImportacionCarpetas/AlmacenStagingImportacion.vb CrearCopiaTrabajo.

### D-03 — Valores y relaciones ALM-01

Verificar por IDs conocidos gabinete, expediente, inventario, incorporación, tipología, nombre/extensión y valores efectivos; conservar ceros de texto/referencia y comparar DECIMAL por valor numérico.

Evidencia: Infrastructure/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionCatalogAdapter.Documento.vb PrepararDatosAlmacenamiento/LeerAlmacenamiento; tests/ImportacionCarpetasBehaviorTests.cs DocumentVerificationCases.

### D-04 — Incertidumbre ALM-02

Separar descarte cliente, fallo previo al checkpoint y fallo de retorno posterior al commit. Leer el histórico antes de cualquier efecto; sin prueba durable suficiente conservar INCIERTO y los IDs válidos.

Evidencia: Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorDocumentoImportacion.vb Almacenar; Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Documento.vb AlmacenarArchivo/ProyectarDocumento.

Revisión editorial aprobada por el usuario mediante «aprobado» el 2026-11-06: el ID del intento nace dentro de `AvanzarLote` y no se predice ni reserva por SQL. Para el soporte exclusivo de laboratorio, compilar actor/destino, lote/raíces/archivos, expedientes, nombres/tamaños/hashes, snapshot y huellas exactas de solicitud. Fijar el primer intento concordante con creación atómica y `Flush(True)` antes de delegar en el inicio transaccional existente; las comprobaciones posteriores exigen el mismo registro durable. La fijación no sustituye la autoridad ni la barrera de escritor único. Registro ausente, alterado, ajeno o incompleto detiene la ejecución; no se borra para habilitar otra reserva. A se resuelve desde el histórico confirmado; B no se repite ni se repara.

Las huellas se calculan con .NET y la misma serialización/ASCII del coordinador, con contraste contra su salida real en pruebas locales. El flag `EsNuevaReserva` es transitorio/JsonIgnore: `ComandoAlmacenamientoCarpetas.Preparar` valida la reserva de entrada y no conserva ese flag en su copia inmutable. El helper respeta ese contrato. Este ajuste permite implementación y pruebas locales del soporte; no autoriza instalación, autenticación o efectos. Detalle aprobado: `doc117-alm02-ajuste-alcance-propuesto-20261106.md` en Validacion.

Instrumentación local posterior de la entrada al escritor: el binding de laboratorio `_invocar` exige la fijación concordante y crea una marca atómica durable una sola vez antes de delegar al escritor existente. El checkpoint exige esa marca. Así se observa directamente la entrada única y se rechaza una segunda invocación; no se modifica la función compartida. Ejecutor y revisión de la candidata cerrada del lote 21 en `doc117-alm02-interno-listo-20261106.md`; la ejecución real y su autorización siguen pendientes.

### D-05 — Reconciliación sin escritor

Usar solo reconciliación soportada con intento INCIERTO, IDs completos y relaciones concordantes. No reparar incorporación ausente ni buscar candidatos por nombre; EN_CURSO no autoriza reconciliar o repetir.

Evidencia: Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Reconciliacion.vb ReconciliarIntento; Infrastructure/ProduccionDocumental/ImportacionCarpetas/VerificadorReconciliacionImportacion.vb.

### D-06 — Coexistencia ALM-03

Acreditar solapamiento real entre importación y carga individual únicamente con alcance autorizado; observar sesiones, copias/originales, IDs y relaciones. Degradación secuencial no es simultaneidad ni acredita XML/SII concurrente.

Evidencia: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-degradacion-individual-pass-20261102.md.

Revisión local posterior 2026-11-06: los intervalos externos de los llamadores no demuestran solapamiento dentro del método compartido; la planificación entre marca y llamada puede producir un falso positivo. El analizador local conserva explícitamente esa limitación. Un ensayo sintético con el colector CLR instalado obtuvo captura, pero aún no acredita extracción por invocación/hilo/recurso. Propuesta pendiente de decisión editorial: `doc117-alm03-observacion-propuesta-20261106.md`, con excepción acotada de observación dentro de una copia exclusiva de laboratorio del escritor. La excepción no está aprobada ni aplicada; permanece la prohibición vigente, no se reduce RQ-06 y ALM-03 continúa NO_EJECUTADO. Evidencia local `doc117-alm03-observabilidad-local-20261106.json`.

Aprobación editorial posterior del usuario mediante «aprobado»: se permite desarrollar/probar marcas internas exclusivamente en una copia de laboratorio de `AlmacenamientoDocumentoProduccionDocumental`, con entrada tras las guardas previas y salida en finally, preservando operaciones, errores, retornos y ByRef. Fuentes productivas/proyecto/bin permanecen intactos. La anotación previa de espera describe el momento anterior. Esta excepción no concede instalación, login, SELECT reales, recursos o almacenamiento. Prototipo desactivado primero; el binding activo deberá cerrar los tres recursos, identidades/huellas y dos sesiones independientes antes de presentarse para otra autorización. No introducir esperas, barreras entre escritores, reintentos o instrumentación productiva. El solapamiento será del método instrumentado, sin afirmar commits simultáneos, rendimiento ordinario o XML/SII.

Lectura autorizada posterior: el destino individual 467 conserva relación 1072 en nivel 8, mientras la importación propuesta pertenece a nivel 7. No exigir que ambos destinos coincidan ni modificar la relación para ello. El alcance propuesto siguiente distingue explícitamente ambos niveles y exige sus facultades actuales, clasificación/gabinete/tipología y ausencia de nombre individual antes de preparar recursos. Las doce consultas de lectura no autorizan una carga en nivel 8 ni acreditan todavía su UI. Detalle `doc117-alm03-destino-lectura-20261106.md` y preparación revisable `doc117-alm03-setup-listo-20261106.md`.

### D-07 — Rechazos y autoridad

Reutilizar negativos de nombre inseguro, campo ajeno, tipología inválida, relación incorrecta, staging alterado y TIFF/Multi-TIFF; comprobar que el escritor no se ejecuta.

Evidencia: Infrastructure/ProduccionDocumental/ImportacionCarpetas/ComandoAlmacenamientoCarpetas.vb Preparar/ValidarDatos; tests/ImportacionCarpetasBehaviorTests.cs DocumentAdapterCases/DocumentCatalogCases.

### D-08 — Ejecución y cierre

No modificar almacenamiento compartido, SII o XML descartado. Cada integración requiere autorización específica de ambiente, cuentas, recursos y fallos; controles SELECT por DSN registrado, secretos efímeros y resultados saneados.

Única excepción editorial aprobada posteriormente para D-06: observación interna del escritor en copia exclusiva de laboratorio, desarrollada y verificada fuera del proyecto. No autoriza modificar la función ordinaria, instalar la copia ni ejecutar cargas reales. Ver `doc117-alm03-observacion-propuesta-20261106.md` y la aprobación posterior registrada en tareas.

Evidencia: AGENTS.md; tools/e2e/AGENT-RUNBOOK.md; tools/e2e/tests/support/doc111-importacion-carpetas.cjs.

### Alternativas y secuencia

Se descarta construir un adaptador nuevo porque ya hay comando inmutable, retención, invocación y verificación. Se descarta reparar efectos con un segundo almacenamiento porque puede duplicar documentos. Se descarta atribuir coexistencia a la degradación previa porque no hubo solapamiento de escritores. Se descarta añadir hooks de fallo productivos: los constructores Friend existentes permiten dobles locales; una variante temporal de laboratorio exigiría diseño separado, fuentes exactas, respaldo verificado, guardián de restauración y autorización específica. La autorización de DOC-116 no se reutiliza.

ALM-01: vincular resultados REC y casos locales por nombre, tipo, fecha, decimal, ceros e incorporación. Para formatos A/B de DOC-114, comparar valores planificados con finales: PLAN-01 no sustituye almacenamiento final. Preparar una propuesta de carga solo para combinaciones no acreditadas.

ALM-02: primero ejecutar dobles locales que observan un solo writer; después proponer por separado las ventanas internas pendientes. Si el efecto existe y falta checkpoint, no reejecutar. Si el estado es EN_CURSO o faltan IDs, registrar la limitación; solo un INCIERTO reconciliable y concordante puede pasar por la capacidad soportada. Falta de incorporación implica conservar incertidumbre; no se añade un reparador.

ALM-03: propuesta separada con recursos distintos, dos solicitudes cuya ejecución se solape y evidencia de tiempos/límites del escritor. Dos envíos cercanos o serializados por sesión no bastan: declarar SERIALIZADO/NO_ACREDITADO. Observar originales y sus hashes, nombres, documento/inventario y relaciones finales; no extender las conclusiones a XML/SII.

## Risks / Trade-offs

- Evidencia histórica parcial → matriz con SHA propio y resultados por afirmación, sin trasladar PASS al commit actual.
- Resultado posterior a efecto no confirmado → consultar sin invocar otra vez; conservar IDs y estado incierto.
- Error del verificador o relación ausente → no confirmar ni reparar incorporación por inferencia.
- Sesión ASP.NET serializada → comprobar solapamiento efectivo o no atribuir simultaneidad.
- Datos reales no autorizados → dejar NO_EJECUTADO; el refinamiento no permite pruebas reales.

## Migration Plan

1. Revisar este diseño, requisitos y propuesta de tareas; registrar aprobación editorial antes de implementación.
2. Sustituir las tareas genéricas por tasks-proposal.md después de su aprobación.
3. Ejecutar regresión local y documentar brechas; modificar producto solo tras reproducir una.
4. Presentar presupuestos concretos de integración antes de solicitar autorización.
5. Cerrar solo con evidencia requerida o aceptación explícita de un alcance reducido; ninguna integración omitida se marca PASS. No hay despliegue ni migración previstos. Cualquier DLL de laboratorio exige su propio protocolo de restauración.


### D-06 — Ajuste local de descripción posterior al diagnóstico real

El diagnóstico de guardas confirmó descriptionExact=false/descriptionEquivalent=true sin entrar al escritor. Para una futura variante, obtener el texto canónico de la tipología autorizada mediante SELECT contextual inequívoco, comprobar su equivalencia con el nombre humano aprobado y fijar su hash UTF-8 exacto. La UI debe coincidir con ese texto antes de despachar; la guarda conserva el argumento original y exige igualdad exacta entre preparación y entrada. Los controles verifican ese mismo texto físico y su invariancia de catálogo, sin normalizar valores almacenados. Variante local versionada y desactivada; no cambia la guarda de evidencia anterior ni habilita reutilizar casos, presupuestos o permisos consumidos. Detalle doc117-alm03-descripcion-canonica-local-20261106.md/.json.
