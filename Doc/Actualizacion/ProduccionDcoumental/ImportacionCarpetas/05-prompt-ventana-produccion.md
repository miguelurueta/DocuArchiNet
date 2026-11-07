# Prompt 05 — Ventana integrada en Producción

## ROL ESPERADO

Actúa como arquitecto y desarrollador senior en ASP.NET WebForms, VB.NET, JavaScript y MySQL 5.1, respetando las fronteras existentes y el alcance de esta entrega.

## OBJETIVO Y ENCARGO

Lee la guía 00, el prompt 03, `Diseno/02-contratos-operacion.md` cuando exista y `Prototipo/README.md`. Implementa la interfaz real del recorrido aprobado en WebForms y JavaScript. Consume perfil 02, plan 06 y operaciones definidas para 07/10/11; no implementes sus reglas dentro del frontend.

## Trabajo

- Inspecciona `Gestion/WebFormProducionDocumental.aspx`, `js/gestion/WebFormProducionDocumental.js`, el contrato del nodo seleccionado y los controles de modal/tabla existentes. Ubica la acción «Importar carpetas» junto a las acciones apropiadas, sin reemplazar la carga individual.
- Habilita apertura solo con nivel elegible; solicita validación al servidor. Captura el contexto por instancia de ventana, no mediante variables globales compartidas con otra carga. Mostrar ruta de origen de solo lectura.
- Adapta la base gráfica, sin copiar el árbol ilustrativo ni sus catálogos. Conserva Perfil → Carpetas → Revisión → Resultados, panel de destino y distinción nivel/gabinete. Filtra perfiles compatibles sin cambiar el nivel por seleccionar un perfil.
- Implementa inventario con identidades de raíz/archivo, referencias File y rutas relativas. Agregar carpetas repetidamente; manejar homónimos, limitaciones de vacías y falta de soporte del navegador. No cargar todo el contenido en memoria ni depender de posiciones de filas.
- Renderiza grandes inventarios de forma acotada; escapar nombres externos, liberar previews y mostrar incidencias accesibles. No ejecutar extracción documental definitiva en el navegador.
- Conecta preparación/confirmación con revisión del plan. Cambios de contenido invalidan el plan; muestra política completa/parcial y ámbito efectivo junto a la confirmación. No permitir cambiarla mediante un checkbox de ocasión que el perfil no autorice.
- Separa adaptadores HTTP de estado/renderizado. Resuelve errores, expiración, timeout y cancelación de peticiones; no dejar promesas pendientes. No interpretar HTTP 200 como documento incorporado.
- Integra resultados reales cuando existan sus servicios. No mantener DEMO, escenarios de error ficticios, catálogos fijos ni temporizadores como ejecución productiva.
- Respeta ClientID, ciclo de UpdatePanel, botones sin postback accidental, inicialización única y destrucción de eventos. No duplicar bibliotecas ni introducir CSS global que afecte otros módulos.

## Límites

Propiedad: esta entrega implementa etapas 1–3 y el contenedor visual de resultados; 11 conecta la consulta, recuperación, CSV y actualización del árbol sobre esos mismos componentes. No construir otra ventana en 11 ni dos clientes HTTP para la misma operación. Consumir `puedeConfirmar` y acciones permitidas del servidor; PARCIAL_PERMITIDO no se bloquea globalmente por cualquier error individual. No copiar la semántica de errores/temporizadores de la maqueta como lógica de producción.

No diseñar tablas ni modificar almacenamiento/SII. Si falta un servicio, usa dobles únicamente en pruebas y registra la integración pendiente; la UI productiva no debe simular éxito. No crear otra administración de estructuras.

## CRITERIOS DE ACEPTACIÓN

Verifica selección de nivel y apertura única, cambio de perfil sin cambio de destino, varias raíces, nombres repetidos, errores que bloquean, revisión invalidada, teclado/foco del modal, cierre/reapertura y actualizaciones parciales sin eventos duplicados. Comprobar respuesta manipulada/inesperada sin romper la pantalla. Entregar componentes, contratos consumidos, capturas de estados reales o identificadas como pruebas y resultados. Las pruebas autenticadas siguen la autorización de la guía 00.

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

Abrir desde un nivel, cambiar perfil conservando destino, agregar homónimos, invalidar revisión y cerrar/reabrir tras UpdatePanel; comprobar foco y ausencia de handlers duplicados.

Ejecutar pruebas unitarias o QA manual reproducible según el impacto, además de integración para persistencia y efectos. Registrar comandos, entorno, resultado y limitaciones. Para cambios VB/proyecto, compilar con MSBuild usando la configuración real de `GestionDocumental-Docuarchi.net.vbproj`; no sustituirlo por un build .NET moderno. Si faltan herramientas/referencias, registrar la limitación. Para JavaScript, comprobar sintaxis con `node --check <archivo-real>` y comportamiento con la infraestructura existente. Las suites DOC-88 aplicables son evidencia estática, no prueba de persistencia real.

La validación E2E del comportamiento entregado es parte integral del mismo cambio: código, pruebas, validación autorizada y evidencia saneada forman su unidad de entrega. El prompt 12 consolida regresión transversal; no sustituye las pruebas de cada componente. Para servicios/adaptadores internos, validar primero contratos y fallos mediante unitarias/integración; no crear endpoints de prueba para eludir el coordinador. Si el recorrido depende de componentes posteriores, registrar integración pendiente y no declarar cerrado el comportamiento integrado.

Reutilizar exclusivamente autenticación, configuración, validadores y utilidades de `tools/e2e`; no crear login, arnés, proyecto Playwright ni .env paralelos. Leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md` antes de autenticar. Ejecutar pruebas reales solo con autorización explícita del ambiente, cuentas y datos descartables; este prompt no concede esa autorización. Cubrir control de acceso, lectura sin mutación, escrituras autorizadas, concurrencia y regresión relacionada con esta entrega.

Usar secretos efímeros; no imprimir ni guardar credenciales, cookies, tokens o cadenas de conexión. Consultas de control solo SELECT; conservar evidencia saneada. Respetar gates de seguridad existentes, sin habilitar arbitrariamente funciones o permisos para probar. No reintroducir flags ni listas de usuarios/grupos como activación de la experiencia moderna. No cerrar sin validación requerida: ante ambiente o autorización ausente, registrar bloqueo explícito, casos no ejecutados y motivo; mocks y simulaciones no sustituyen evidencia real.

## E2E POR CAPACIDAD Y CRITERIO DE CIERRE

### Reutilizaci?n obligatoria de infraestructura E2E

Antes de escribir pruebas, inventariar y reutilizar `tools/e2e/package.json`, `tools/e2e/playwright.config.cjs`, `tools/e2e/tests/support/`, `tools/e2e/scripts/`, `tools/e2e/fixtures/` y `tools/e2e/AGENT-RUNBOOK.md`. Ubicar las nuevas especificaciones en `tools/e2e/tests/` y registrar solo los comandos necesarios en el paquete existente. Usar los reporteros y directorios de evidencia ya configurados; conservar saneamiento de secretos y datos sensibles tambi?n en capturas y trazas.

Reutilizar `tools/e2e/tests/support/authenticated-workflow-session.cjs` cuando su contrato soporte el acceso requerido; verificarlo antes, sin fabricar tareas Workflow. Si falta soporte para Producci?n, ampliar de forma compatible el soporte compartido y su configuraci?n/runner existente, con pruebas de regresi?n de los consumidores afectados. Reutilizar validadores de autorizaci?n, perfiles y utilidades aplicables identificados en el inventario; documentar qu? se reutiliza y qu? extensi?n m?nima necesita esta capacidad. No copiar suites completas ni introducir login, almacenamiento de sesi?n, .env, dependencias, instalaci?n de navegadores, runner o configuraci?n Playwright paralelos. Un script delgado nuevo dentro de `tools/e2e/scripts/` solo puede seleccionar escenarios y delegar en los mecanismos compartidos; no reimplementar autenticaci?n ni autorizaci?n.

No cambiar globalmente workers, reintentos, timeouts, permisos o configuraci?n para lograr pruebas verdes. Cualquier extensi?n compartida debe mantener los contratos de las suites existentes y acreditar regresi?n pertinente. Las pruebas DOC-88 de pol?tica son evidencia est?tica complementaria, no reemplazan los escenarios E2E reales. El inventario de reutilizaci?n y la ausencia de infraestructura duplicada son criterios de aceptaci?n de esta entrega; para 04 se documentan y para 13 solo se implementan al activar su alcance opcional.

Implementar estos escenarios como parte de la entrega, reutilizando `tools/e2e`. Son pruebas futuras: esta documentación no acredita suites creadas ni corridas. Leer [la estrategia de comprobación progresiva](Validacion/01-estrategia-e2e-capacidades.md).

| ID | Precondiciones | Pasos del recorrido | Aserciones obligatorias |
|---|---|---|---|
| UI-01 | Producción y servicios 02/06 disponibles. | Seleccionar nivel A, abrir ventana, cambiar perfil y navegar árbol a B mientras permanece abierta. | La ventana conserva A; servidor valida A; no aparece segundo selector ni se cambia destino por perfil. |
| UI-02 | Fixtures sintéticos con dos raíces homónimas y subcarpetas. | Agregar ambas raíces por selector; preparar revisión; agregar otro archivo y volver a confirmar. | Identidades distintas, rutas relativas correctas; cambio invalida plan; no confirma revisión obsoleta. |
| UI-03 | Ventana integrada con UpdatePanel. | Abrir/cerrar/reabrir, provocar actualización parcial y navegar con teclado; probar error de servicio. | Una sola solicitud por acción, foco recuperado y error visible; nunca éxito simulado. |

Para cada caso registrar estado EJECUTADO_OK, FALLIDO o NO_EJECUTADO, SHA probado, ambiente, datos sintéticos, pasos/comando real, evidencia saneada de UI/HTTP y comprobaciones persistidas de solo lectura. No declarar aprobada una capacidad únicamente con mocks o revisión estática. Un caso bloqueado debe identificar dependencia y responsable; no se omite del informe.

Entregar especificación automatizada y comando reproducible dentro de la infraestructura existente; el nombre final se inventaría en `Diseno/design.md`. Si la autorización no permite ejecutarla, entregar suite y evidencia local pertinente, manteniendo la validación real pendiente. No habilitar permisos ni ejecutar cargas por inferencia de este prompt.

## DOCUMENTACIÓN TÉCNICA

Actualizar documentación existente en `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/`: `Diseno/` para contratos y decisiones, y `Validacion/` para evidencia. Mantener guía 00 y consumidores sincronizados si cambia un contrato. No abrir una segunda fuente de verdad. Registrar archivos y funciones afectados, dependencia propietaria, casos ejecutados/no ejecutados y riesgos pendientes. Una revisión estructural PASS no certifica implementación ni ausencia de regresiones.

## ENTREGABLE FINAL

Integrar la ventana y las etapas Perfil, Carpetas y Revisión en Producción, consumiendo servicios reales y dejando el seguimiento al prompt 11. Entregar los artefactos especificados en este prompt y una relación verificable de cambios, pruebas, resultados y pendientes. No declarar funcionalidades completas por entregar únicamente stubs, mocks o documentación.
