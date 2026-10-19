# Prompt 09 — Almacenamiento e incorporación documental

## ROL ESPERADO

Actúa como arquitecto y desarrollador senior en ASP.NET WebForms, VB.NET, JavaScript y MySQL 5.1, respetando las fronteras existentes y el alcance de esta entrega.

## OBJETIVO Y ENCARGO

Lee guía 00, prompt 03, diseño 04 y contratos 07/08. Implementa un adaptador propio que almacene archivos recibidos y verifique incorporación al expediente confirmado, reutilizando operaciones compatibles sin modificar SII.

## Investigación obligatoria

En `workflow/ClassAlmacenamiento.vb`, verifica `PreAlmacenaDocumentoProduccion`, `AlmacenamientoDocumentoProduccionDocumental` y sus llamadas a almacenamiento. La segunda recibe `List(Of CDcamposAsignaAlmacenamiento)` pero conserva dependencias de sesión/temporales. Examina qué pasos producen imagen, registro, relación e índice y cuáles pueden fallar después de persistir. No suponer que almacenar e incorporar son dos llamadas independientes ni ejecutar ambas a ciegas.

El adaptador SII `LegacyImportDocumentStorageAdapter` exige contexto Workflow: es referencia, no una operación de carpetas. No enviar tareas ficticias ni alterar contratos SII para reutilizarlo.

Hallazgo concreto a caracterizar: `AlmacenamientoDocumentoProduccionDocumental` obtiene el nombre de `FileInfo(RutaArchivoAlmacenar).Name` y elimina `MatrizDocumentosFinal` después de persistir; también puede eliminar el temporal de sesión. Esto contradice pasarle el único staging retenido y asumir que su nombre opaco no altera el nombre documental. Utilizar una copia de trabajo aislada y consumible, conservando staging y evidencia hasta verificar; preservar nombre/extensión mediante un contrato realmente soportado o una copia con nombre seguro dentro de un directorio único. Si no puede garantizarse sin cambiar el método compartido, ese punto de reutilización no es apto. La política de limpieza de 11 controla staging, no impide los borrados internos del método legacy.

## Trabajo

- Comando explícito: identidad de operación/archivo, temporal verificado, expediente/nivel, gabinete, tipología, campos índice resueltos y autoridad. Cargar datos desde el plan/lote, no aceptar destinos arbitrarios del cliente.
- Componer una asignación por campo, comprobar pertenencia al gabinete, tipo, longitud, obligatoriedad y tipología compatible. Aplicar constantes/herencia/nombre según perfil sin cambiar relaciones globales de gabinete.
- Verificar dependencia de sesión. Aislar cualquier contexto de compatibilidad con restauración en Finally y exclusión probada frente a otros consumidores; no presentar esa solución como apta para worker. Si no es aislable, documentar el límite y usar una operación inferior compatible sin modificar SII.
- Preparar temporales únicos y formatos admitidos; caracterizar Multi-TIFF, inventario e índice electrónico. No anunciar formatos no verificados.
- Retornar resultado estructurado: rechazado antes de efectos, persistencia confirmada con IDs, parcial con fase pendiente o incierto. No clasificar excepción de proyección/limpieza como garantía de que no se almacenó.
- Verificar gabinete, documento, expediente y efectos de incorporación. Si falta un efecto compatible, ejecutar solo ese paso con su identidad; no volver a almacenar. Las consultas de reconciliación deben justificar cómo atribuyen el documento a esta operación.
- Persistir IDs/evidencia mediante 07 y conservar temporales necesarios ante incertidumbre. Eliminar temporales solo con la política segura de 11, sin ocultar el éxito documental por un error de limpieza.

## CRITERIOS DE ACEPTACIÓN

Probar que nombres originales y extensiones se conservan conforme a la política incluso con temporales opacos, homónimos y Multi-TIFF; un fallo posterior a persistencia/borrado no destruye el staging retenido. Resolver/revalidar campos diferidos con los IDs reales del expediente usando el resolvedor 02. Probar concurrencia con escritores externos del expediente/índice y separación de sesión/temporales; una exclusión del lote no demuestra aislamiento con carga individual o SII.

Pruebas con índices extraídos de dos estructuras configurables, ceros iniciales, tipología inexistente, campo ajeno al gabinete, homónimos, almacenamiento ya confirmado, incorporación faltante, relación a otro expediente y respuesta perdida tras persistencia. Caracterizar fallos del índice/XML y limpieza. No prometer exactamente una vez cuando no existe correlación fiable: bloquear reintento automático y reportar revisión.

Entregar adaptador y verificador, mapa de efectos y ventanas de fallo, compatibilidad de formatos y pruebas. No escribir otro registro de lotes ni cambiar tablas SII; SQL incremental MySQL 5.1 solo si el diseño operativo exige un cambio justificado.

## CONTEXTO Y CONTRATO DE INTEGRACIÓN

Leer `00-guia-ejecucion-prompts.md` y los artefactos vigentes de `Diseno/`; distinguir propuestas de componentes existentes. Para esta entrega, documentar el contrato request/response, autoridad, errores, estados y efectos permitidos. Flujo de referencia: Cliente → entrada ASMX/ASHX (responsabilidad de Controller) → Service → Repository/adaptadores → respuesta; no imponer ASP.NET MVC a WebForms. Los adaptadores internos de 08/09 no se exponen como operaciones públicas independientes.

## PRECONDICIONES DE RUTAS

Antes de implementar, inventariar los archivos exactos en `Diseno/design.md`, con propietario (prompt), existentes/nuevos, responsabilidad y consumidores. Es un mapa de implementación complementario a `Diseno/01-modelo-datos.md` y `Diseno/02-contratos-operacion.md`, no otro diseño de contratos. Si existe un cambio OpenSpec, referenciar ese mapa desde su `design.md` sin duplicarlo. Verificar estas ubicaciones propuestas contra el repositorio; no afirmar que los archivos nuevos ya existen ni crear todas las capas por ejecutar esta entrega.

| Responsabilidad | Ubicación común propuesta | Propiedad |
|---|---|---|
| Modelos y puertos | `Modelo/ProduccionDocumental/ImportacionCarpetas/` | 04 define; cada entrega añade solo su contrato. |
| DTOs de entrada/salida | `DTOs/ProduccionDocumental/ImportacionCarpetas/` | 02/06/07/10/11 según operación. |
| Servicios y reglas | `Services/ProduccionDocumental/ImportacionCarpetas/` | 02 perfil; 06 plan; 07 recepción; 10 coordinación; 11 consulta/mantenimiento. |
| Repositorios | `Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/` | 02 perfil; 07-A operación. Los demás consumen sus interfaces. |
| Adaptadores documentales | `Infrastructure/ProduccionDocumental/ImportacionCarpetas/` | 08 expedientes; 09 almacenamiento e incorporación. |
| Servicio web nuevo | `webservice/WebServiceImportacionCarpetas.asmx` y `webservice/WebServiceImportacionCarpetas.asmx.vb` | Una fachada de las operaciones anteriores; sin reglas documentales en transporte. |
| Recepción multipart nueva | `generic_control/ImportacionCarpetasUpload.ashx` y `generic_control/ImportacionCarpetasUpload.ashx.vb` | 07-B; delega en su servicio; no sustituye el handler compartido. |
| Integración visual existente | `Gestion/WebFormProducionDocumental.aspx`, su code-behind `.aspx.vb` y `js/gestion/WebFormProducionDocumental.js` | 05 integra; 11 extiende resultados sobre la misma ventana. |
| JavaScript específico nuevo | `js/gestion/ImportacionCarpetas/` | 05 estado/transporte/renderizado; 11 seguimiento; nombres exactos en el inventario. |

Registrar los archivos nuevos de compilación y contenido en `GestionDocumental-Docuarchi.net.vbproj` según el tipo de recurso. Mantener namespaces y referencias compatibles con .NET Framework 4.6.1. Las rutas nuevas son destino de implementación, no evidencia de componentes existentes. El host opcional del 13 requiere su propio inventario tras decidir alojamiento; no inventar un ejecutable desplegado.

## RESTRICCIONES CRÍTICAS

No debe modificarse SII: código, tablas, configuración, contratos ni consumidores. Preservar carga individual y contratos legacy; no cambiar pruebas/huellas para ocultar regresiones. No crear otro árbol ni cambiar el nivel al elegir perfil. No usar autorregistro, tareas ficticias, valores fijos del ejemplo ni temporizadores del prototipo como implementación productiva.

Mantener COMPLETO_OBLIGATORIO configurable por EXPEDIENTE o LOTE; exigir elección al publicar y congelarla en plan/lote. PARCIAL_PERMITIDO usa NO_APLICA. No prometer rollback global: conservar efectos confirmados y reconciliar incertidumbre. Cada prompt implementa solo su responsabilidad según la guía 00; no duplicar motor, repositorios ni endpoints.

El equipo aplica SQL manualmente: MySQL 5.1, sin migraciones automáticas. Los scripts de perfil pertenecen a 02 y los operativos a 07; otros cambios justificados usan incrementos coordinados. No ejecutar SQL por tener conexión ni declarar aplicado un script por existir.

## PRUEBAS OBLIGATORIAS

Verificar gabinete, relación e índice; simular fallo posterior a persistir y recuperar solo la fase faltante; demostrar conservación de staging y nombre/extensión frente a limpieza legacy.

Ejecutar pruebas unitarias o QA manual reproducible según el impacto, además de integración para persistencia y efectos. Registrar comandos, entorno, resultado y limitaciones. Para cambios VB/proyecto, compilar con MSBuild usando la configuración real de `GestionDocumental-Docuarchi.net.vbproj`; no sustituirlo por un build .NET moderno. Si faltan herramientas/referencias, registrar la limitación. Para JavaScript, comprobar sintaxis con `node --check <archivo-real>` y comportamiento con la infraestructura existente. Las suites DOC-88 aplicables son evidencia estática, no prueba de persistencia real.

La validación E2E del comportamiento entregado es parte integral del mismo cambio: código, pruebas, validación autorizada y evidencia saneada forman su unidad de entrega. El prompt 12 consolida regresión transversal; no sustituye las pruebas de cada componente. Para servicios/adaptadores internos, validar primero contratos y fallos mediante unitarias/integración; no crear endpoints de prueba para eludir el coordinador. Si el recorrido depende de componentes posteriores, registrar integración pendiente y no declarar cerrado el comportamiento integrado.

Reutilizar exclusivamente autenticación, configuración, validadores y utilidades de `tools/e2e`; no crear login, arnés, proyecto Playwright ni .env paralelos. Leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md` antes de autenticar. Ejecutar pruebas reales solo con autorización explícita del ambiente, cuentas y datos descartables; este prompt no concede esa autorización. Cubrir control de acceso, lectura sin mutación, escrituras autorizadas, concurrencia y regresión relacionada con esta entrega.

Usar secretos efímeros; no imprimir ni guardar credenciales, cookies, tokens o cadenas de conexión. Consultas de control solo SELECT; conservar evidencia saneada. Respetar gates de seguridad existentes, sin habilitar arbitrariamente funciones o permisos para probar. No reintroducir flags ni listas de usuarios/grupos como activación de la experiencia moderna. No cerrar sin validación requerida: ante ambiente o autorización ausente, registrar bloqueo explícito, casos no ejecutados y motivo; mocks y simulaciones no sustituyen evidencia real.

## DOCUMENTACIÓN TÉCNICA

Actualizar documentación existente en `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/`: `Diseno/` para contratos y decisiones, y `Validacion/` para evidencia. Mantener guía 00 y consumidores sincronizados si cambia un contrato. No abrir una segunda fuente de verdad. Registrar archivos y funciones afectados, dependencia propietaria, casos ejecutados/no ejecutados y riesgos pendientes. Una revisión estructural PASS no certifica implementación ni ausencia de regresiones.

## ENTREGABLE FINAL

Almacenar e incorporar cada archivo con índices del perfil, verificar efectos y preservar staging y nombre original sin modificar operaciones compartidas. Entregar los artefactos especificados en este prompt y una relación verificable de cambios, pruebas, resultados y pendientes. No declarar funcionalidades completas por entregar únicamente stubs, mocks o documentación.
