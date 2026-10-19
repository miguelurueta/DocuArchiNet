# Prompt 10 — Coordinación y recuperación del lote

## ROL ESPERADO

Actúa como arquitecto y desarrollador senior en ASP.NET WebForms, VB.NET, JavaScript y MySQL 5.1, respetando las fronteras existentes y el alcance de esta entrega.

## OBJETIVO Y ENCARGO

Lee guía 00, prompt 03, diseño 04 y componentes reales 06–09. Implementa el servicio que coordina fases persistidas por unidad y archivo. No duplicar adaptadores, recepción ni repositorios.

## Trabajo

1. Definir operación acotada de avance: autorizar lote, cargar plan/configuración efectiva y reclamar una fase mediante estado/revisión persistidos. No procesar todo el lote en una petición larga ni iniciar tareas en memoria que prometan sobrevivir al reciclado.
2. Aplicar política publicada y ámbito confirmado. COMPLETO_OBLIGATORIO espera validación/recepción de todos los archivos de la unidad antes de crear/almacenar. PARCIAL_PERMITIDO procesa elegibles y mantiene rechazados/pendientes visibles. No reducir el inventario original para obtener COMPLETADO.
3. Ejecutar creación/verificación de expediente una vez por raíz y procesar documentos mediante 08/09. Revalidar permisos, destino y referencias antes del efecto; no confiar solo en el preview.
4. Persistir reclamaciones, intentos, transiciones y resultados. Una exclusión vencida no prueba que el efecto no ocurrió: reconciliar antes de repetir. Considerar dos clientes y una petición lenta que termina después de vencer su reclamación; no permitir efectos duplicados por un segundo ejecutor.
5. Clasificar reintentos: recepción segura, rechazo corregible, fase pendiente conocida e incertidumbre. No aplicar un reintento general a cualquier excepción. Acotar número/frecuencia y conservar diagnóstico seguro.
6. Implementar detención en puntos seguros. Cancelar pendientes no revierte efectos confirmados ni garantiza interrupción de llamadas en vuelo. Estados inciertos siguen requiriendo verificación.
7. Consolidar lote/raíz desde estados persistidos: completo solo con todos los efectos exigidos confirmados; parcial, detenido, fallido o pendiente de verificación con totales reconciliables. No derivar verdad de barras de progreso cliente.
8. Conservar snapshot y ámbito del nivel; cambios en perfil publicado o sesión no reescriben el lote. Revalidar disponibilidad externa conforme a contrato sin cambiar destinos silenciosamente.

## Primera modalidad de ejecución

Ambos ámbitos están aprobados: implementar una barrera parametrizada en el mismo coordinador. EXPEDIENTE evalúa todos los archivos de la raíz; LOTE evalúa todas las raíces/archivos del inventario confirmado. Dos solicitudes concurrentes no deben saltarse la barrera por un contador obsoleto: comprobar estados persistidos y habilitación bajo la coordinación transaccional definida. Mantener modo/ámbito del snapshot en reanudación; ninguna petición puede cambiarlos. Un fallo posterior conserva avances y bloquea completar la unidad; no hay rollback global ni garantía de éxito simultáneo.

Propiedad: avance, cancelación y reconciliación pertenecen a este servicio; 11 consulta/presenta sus acciones, no implementa otra máquina de estados. Si un permiso o el perfil se desactiva, preservar evidencias y resultados y aplicar la política explícita de suspensión de nuevos efectos sin sustituir versión. Autorizar consulta/reconciliación separadamente para permitir una investigación legítima; nunca habilitarla a un usuario que perdió autorización.

El cliente puede solicitar avances sucesivos y consultar estado. No prometer continuidad desatendida al cerrar el navegador. El servicio debe permitir que otro cliente autorizado recupere el mismo lote; los bytes locales no recibidos requieren selección nuevamente. Dejar el puerto de ejecución separado para 13 sin implementar una plataforma de workers ahora.

## CRITERIOS DE ACEPTACIÓN

Inyectar fallos antes/después de cada efecto, pérdida de respuesta, concurrencia, exclusión vencida, expiración de sesión, cancelación con llamada en vuelo y cambio de permisos. Verificar que completo espera la unidad exigida y nunca aparece completo con un archivo incierto. Probar reanudación sin recrear expediente ni reinsertar documentos confirmados. Entregar coordinador, transiciones implementadas, endpoints, pruebas y limitaciones de reconciliación. Cualquier cambio de esquema se entrega como SQL manual MySQL 5.1 con dependencias documentadas.

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

Dos solicitudes de avance no duplican efectos; barreras por raíz/lote esperan todos sus contenidos; cancelación conserva resultados; exclusión vencida y respuesta perdida exigen reconciliación.

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
| COORD-01 | Dos raíces A/B; B con archivo pendiente; tres perfiles de integridad. | Solicitar avance antes y después de completar recepción en cada política. | EXPEDIENTE permite A y espera B; LOTE no produce efectos hasta recibir todo; parcial conserva rechazados y procesa elegibles. |
| COORD-02 | Lote listo y dos clientes autorizados. | Enviar avances simultáneos; perder respuesta y simular reclamación vencida con operación en vuelo. | Sin efectos duplicados; incertidumbre bloquea reejecución hasta reconciliar; COMPLETADO solo con todos los efectos confirmados. |
| COORD-03 | Lote en ejecución. | Cancelar durante una fase, cerrar navegador, consultar desde otra sesión autorizada y reanudar pendientes permitidos. | Efectos confirmados conservados; detención segura; snapshot no cambia; primera modalidad no promete continuidad sin cliente. |

Para cada caso registrar estado EJECUTADO_OK, FALLIDO o NO_EJECUTADO, SHA probado, ambiente, datos sintéticos, pasos/comando real, evidencia saneada de UI/HTTP y comprobaciones persistidas de solo lectura. No declarar aprobada una capacidad únicamente con mocks o revisión estática. Un caso bloqueado debe identificar dependencia y responsable; no se omite del informe.

Entregar especificación automatizada y comando reproducible dentro de la infraestructura existente; el nombre final se inventaría en `Diseno/design.md`. Si la autorización no permite ejecutarla, entregar suite y evidencia local pertinente, manteniendo la validación real pendiente. No habilitar permisos ni ejecutar cargas por inferencia de este prompt.

## DOCUMENTACIÓN TÉCNICA

Actualizar documentación existente en `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/`: `Diseno/` para contratos y decisiones, y `Validacion/` para evidencia. Mantener guía 00 y consumidores sincronizados si cambia un contrato. No abrir una segunda fuente de verdad. Registrar archivos y funciones afectados, dependencia propietaria, casos ejecutados/no ejecutados y riesgos pendientes. Una revisión estructural PASS no certifica implementación ni ausencia de regresiones.

## ENTREGABLE FINAL

Coordinar avance acotado, barreras de integridad por expediente o lote, detención y recuperación sobre las fases persistidas. Entregar los artefactos especificados en este prompt y una relación verificable de cambios, pruebas, resultados y pendientes. No declarar funcionalidades completas por entregar únicamente stubs, mocks o documentación.
