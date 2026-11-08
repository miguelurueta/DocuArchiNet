# Plan candidato de implementación — recuperación V2

Pendiente de revisión; no forma parte todavía de las27 tareas activas. Deriva de D-05/RQ-05, D-07/RQ-07, D-08/RQ-08, D-09/RQ-09 y D-10/RQ-10; los RC citan `recovery-en-curso-spec.md`. No autoriza ejecutar sobre69 ni ampliar presupuestos. S/M son complejidades relativas.

| ID | Área y archivos previstos | Cambio acotado | Origen | Verificación requerida |
| --- | --- | --- | --- | --- |
| RV-01 [M] | `Modelo/.../ImportacionCarpetasModels.vb`, serializador y `tests/ImportacionCarpetasBehaviorTests.cs` | Fijar documentos V2 de inicio, autorización y retorno; versión/origen del servidor e invariantes de compatibilidad V1. | RC-01, RC-03, RC-07 | V1 no se vuelve elegible; metadatos extra/incompletos o enviados por cliente no conceden capacidad; hashes no incluyen secretos. |
| RV-02 [M] | `Modelo/.../ImportacionCarpetasInterfaces.vb`, `Infrastructure/.../AlmacenStagingImportacion.vb`, nuevo parcial de recuperación del mismo staging | Probar y exponer una guardia exclusiva no destructiva con identidad física acreditada; falla cerrada ante proveedor/nodo no acreditado. No reutilizar `AbrirRetiro`. | RC-02, RC-07 | Un lector vivo en otro proceso impide adquirir; un recurso distinto no sirve; original ausente/cambiado y errores liberan handles sin borrar/copiar; ningún lock se infiere por fecha. **Compuerta de factibilidad antes de continuar.** |
| RV-03 [M] | `Infrastructure/.../AdaptadorDocumentoImportacion.vb` y su composición existente | Conservar retención en el adaptador antes del inicio V2 hasta la salida; exigir autorización de invocación y registrar retorno observado. | RC-02, RC-03 | Pausas antes/después del inicio, autorización perdida y abortos no duplican invocación; si no se conoce el resultado del gate no se llama al escritor; sin suprimir abortos. |
| RV-04 [M] | `Infrastructure/Repositories/.../MySqlImportacionCarpetasRepository.Documento.vb`, `.Control.vb` y parcial de recuperación | Persistir los eventos/documentos V2 con comparación de token/hash/contexto y locks existentes, sin DDL. | RC-01, RC-03, RC-06 | Dos autorizaciones concurrentes no habilitan dos llamadas; diario incompleto/duplicado o respuesta perdida no se transforma en permiso; V1 sin cambios. |
| RV-05 [M] | `Services/.../CoordinadorImportacionCarpetas.Reconciliacion.vb`, repositorio de reconciliación y `Infrastructure/.../ComandoAlmacenamientoCarpetas.vb` | Clasificar V2 con guardia y evidencia; cierre negativo como excepción estrecha y confirmación positiva con IDs observados, en operaciones sin puerto de almacenamiento. | RC-01, RC-02, RC-04, RC-05, RC-06 | Rechazar69/V1; casos negativos y positivos conservan inventario/autoridad; ningún reset general EN_CURSO→NO_INICIADO ni paso forzado a INCIERTO; positivo verifica mismos IDs; rollback si falla evento/consolidación. |
| RV-06 [M] | `webservice/WebServiceImportacionCarpetas.asmx.vb`, composición y DTOs existentes | Exponer la operación explícita con referencias mínimas y guards Gestión/CSRF/contexto; no aceptar pruebas del cliente. | RC-01, RC-06, RC-07 | Sesión/permiso/destino/revisión inválidos no alcanzan recuperación; respuesta distingue cierre de control de documento confirmado; no filtra tokens, rutas o excepciones. |
| RV-07 [M] | `tests/ImportacionCarpetasBehaviorTests.cs`, pruebas de staging y drivers existentes | Cubrir intercalaciones, proceso vivo/terminado, proveedor no acreditado, diario y compatibilidad. | RC-01..RC-07 | Matriz del diseño completa: una llamada como máximo, cero escrituras al bloquear, replay solo historial y ninguna modificación de intentos protegidos. |
| RV-08 [S] | OpenSpec, `Doc/Tecnica/Opsxj/.../`, herramientas E2E existentes | Consolidar revisión y preparar un plan real separado solo después de pasar la compuerta de factibilidad y la regresión. | D-01/RQ-01, D-10/RQ-10 | Revisión local y Git sincronizado; ninguna aceptación real se infiere; ambiente, recursos nuevos y límites concretos antes de solicitar ejecución. |

Los puntos suspensivos representan los directorios existentes `ProduccionDocumental/ImportacionCarpetas`; no autorizan un segundo repositorio, motor o worker. Los cambios de producto solo se hacen después de aceptar el diseño e integrar el desglose al mapa activo.

## Casos que deben seguir bloqueados

- Intento69 y cualquier V1 sin evidencia versionada original.
- Ejecutor vivo/pausado que conserva retención, original sin identidad compartida o proveedor no acreditado.
- Autorización de invocación sin retorno durable completo, aun con conteos físicos cero.
- IDs recibidos del cliente, obtenidos por coincidencia de nombres, incompletos o discrepantes.
- Cambio de autoridad, revisión, snapshot o reclamación entre lectura y persistencia.
- Cierre negativo con detención, journal contradictorio o marcador de un intento posterior.

## Criterio de revisión

No se debe aprobar una implementación que prometa liberar69 o todos los EN_CURSO. La candidata ofrece recuperación limitada para pruebas V2 emitidas desde el inicio. Si RV-02 no puede demostrar la exclusión necesaria con el proveedor actual, debe reducirse el alcance o conservarse el bloqueo; no sustituir la prueba por una declaración manual.
