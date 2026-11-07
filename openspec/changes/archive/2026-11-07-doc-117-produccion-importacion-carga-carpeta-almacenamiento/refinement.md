<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento DOC-117

## Fuente y alcance

Copia principal local autoritativa. Ticket original preservado en specs/produccion-importacion-carga-carpeta-almacenamiento/jira-context.md. Perfil legacy-webforms-vb. Estado: aprobado editorialmente para trabajo local; no concede autorización para integración real.

## Contexto inspeccionado

- Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorDocumentoImportacion.vb
- Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorDocumentoImportacion.Legacy.vb
- Infrastructure/ProduccionDocumental/ImportacionCarpetas/ComandoAlmacenamientoCarpetas.vb
- Infrastructure/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionCatalogAdapter.Documento.vb
- Infrastructure/ProduccionDocumental/ImportacionCarpetas/AlmacenStagingImportacion.vb
- Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Documento.vb
- Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Reconciliacion.vb
- Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.Documento.vb
- Infrastructure/ProduccionDocumental/ImportacionCarpetas/VerificadorReconciliacionImportacion.vb
- tests/ImportacionCarpetasBehaviorTests.cs

Comparación y huellas normalizadas en source-review.json. Comportamiento inspeccionado: copia retenida, una invocación, IDs en Finally, verificación SELECT, checkpoint e histórico antes de reservar, reconciliación sin writer.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | La copia principal local prevalece; cada evidencia conserva SHA, fecha, ambiente y tipo real/local. Una divergencia con la copia aislada se revisa sin sobrescribir producto. | Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorDocumentoImportacion.vb; tests/ImportacionCarpetasBehaviorTests.cs | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Reutilizar comando, staging, adaptador y binding existentes: retener el original verificado, entregar copia privada consumible con nombre original e invocar legacy una vez por reserva válida. | Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorDocumentoImportacion.vb Almacenar; Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorDocumentoImportacion.Legacy.vb InvocarAlmacenamiento; Infrastructure/ProduccionDocumental/ImportacionCarpetas/AlmacenStagingImportacion.vb CrearCopiaTrabajo | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Verificar por IDs conocidos gabinete, expediente, inventario, incorporación, tipología, nombre/extensión y valores efectivos; conservar ceros de texto/referencia y comparar DECIMAL por valor numérico. | Infrastructure/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionCatalogAdapter.Documento.vb PrepararDatosAlmacenamiento/LeerAlmacenamiento; tests/ImportacionCarpetasBehaviorTests.cs DocumentVerificationCases | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Separar descarte cliente, fallo previo al checkpoint y fallo de retorno posterior al commit. Leer el histórico antes de cualquier efecto; sin prueba durable suficiente conservar INCIERTO y los IDs válidos. | Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorDocumentoImportacion.vb Almacenar; Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Documento.vb AlmacenarArchivo/ProyectarDocumento | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Usar solo reconciliación soportada con intento INCIERTO, IDs completos y relaciones concordantes. No reparar incorporación ausente ni buscar candidatos por nombre; EN_CURSO no autoriza reconciliar o repetir. | Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Reconciliacion.vb ReconciliarIntento; Infrastructure/ProduccionDocumental/ImportacionCarpetas/VerificadorReconciliacionImportacion.vb | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | Acreditar solapamiento real entre importación y carga individual únicamente con alcance autorizado; observar sesiones, copias/originales, IDs y relaciones. Degradación secuencial no es simultaneidad ni acredita XML/SII concurrente. | Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-degradacion-individual-pass-20261102.md | D-06 | RQ-06 | Origen: D-06, RQ-06 |
| D-07 | Reutilizar negativos de nombre inseguro, campo ajeno, tipología inválida, relación incorrecta, staging alterado y TIFF/Multi-TIFF; comprobar que el escritor no se ejecuta. | Infrastructure/ProduccionDocumental/ImportacionCarpetas/ComandoAlmacenamientoCarpetas.vb Preparar/ValidarDatos; tests/ImportacionCarpetasBehaviorTests.cs DocumentAdapterCases/DocumentCatalogCases | D-07 | RQ-07 | Origen: D-07, RQ-07 |
| D-08 | No modificar almacenamiento compartido, SII o XML descartado. Cada integración requiere autorización específica de ambiente, cuentas, recursos y fallos; controles SELECT por DSN registrado, secretos efímeros y resultados saneados. | AGENTS.md; tools/e2e/AGENT-RUNBOOK.md; tools/e2e/tests/support/doc111-importacion-carpetas.cjs | D-08 | RQ-08 | Origen: D-08, RQ-08 |

## Requisitos verificables

| ID | Resultado observable | Escenarios |
| --- | --- | --- |
| RQ-01 | Procedencia verificable | Dos escenarios observables en spec.md |
| RQ-02 | Original conservado y efecto único | Dos escenarios observables en spec.md |
| RQ-03 | Valores finales ALM-01 | Dos escenarios observables en spec.md |
| RQ-04 | Incertidumbre ALM-02 | Dos escenarios observables en spec.md |
| RQ-05 | Reconciliación documental acotada | Dos escenarios observables en spec.md |
| RQ-06 | Coexistencia ALM-03 con límites | Dos escenarios observables en spec.md |
| RQ-07 | Rechazos antes del escritor | Dos escenarios observables en spec.md |
| RQ-08 | Aceptación y autorización | Dos escenarios observables en spec.md |

## Resultado y compuertas

El usuario indicó «continua» tras la petición explícita de aprobar diseño y aplicar las 18 tareas. Se registra aprobación editorial del diseño y plan local; tasks.md reemplaza las ocho tareas genéricas y conserva 18 tareas abiertas con origen D/RQ. La aprobación no concede autorización del ambiente, cuentas, recursos, fallos internos ni coexistencia real. Las integraciones continúan NO_EJECUTADO hasta su autorización específica.
