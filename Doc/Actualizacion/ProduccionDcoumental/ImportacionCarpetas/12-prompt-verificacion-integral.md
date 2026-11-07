# Prompt 12 — Verificación integral y regresión

## ROL ESPERADO

Actúa como arquitecto y desarrollador senior en ASP.NET WebForms, VB.NET, JavaScript y MySQL 5.1, respetando las fronteras existentes y el alcance de esta entrega.

## OBJETIVO Y ENCARGO

Lee guía 00, los prompts 02–11 y la implementación real. Construye una matriz de verificación trazable a contratos, reglas y decisiones confirmadas. Esta entrega no autoriza cargas autenticadas ni despliegues por sí sola: respeta permisos de ambiente y AGENTS/runbook.

## Verificación

1. Auditar correspondencia entre DDL entregado, contratos y código: columnas/índices reales, MySQL 5.1, orden de scripts, versiones y estado de aplicación conocido. No considerar aplicadas migraciones porque sus archivos existan.
2. Probar perfil/publicación y ambos formatos de extracción con datos sintéticos; nivel recibido desde el árbol, perfiles compatibles, cambios de sesión/otra pestaña y controles de autorización servidor.
3. Validar dos raíces, homónimos, subcarpetas, restricciones, inventario fragmentado y revisión alterada. Comprobar política completa/parcial y ámbito confirmado en preflight y ejecución, no solo UI.
4. Probar confirmación concurrente, clave con payload distinto, cuotas, transferencia truncada, reenvío de contenido diferente, ruta maliciosa y limpieza protegida.
5. Validar creación bajo el nivel correcto, gabinete e índices, incorporación, inventario e índice documental cuando aplique. Verificar que repetir operaciones confirmadas no duplica efectos.
6. Inyectar pérdida de respuesta después de crear/almacenar, fallos entre fases, exclusión vencida con petición activa, cancelación, permiso revocado y proceso interrumpido. Demostrar recuperación o bloqueo seguro con incertidumbre explícita.
7. Verificar carga individual y rutas compartidas pertinentes. Ejecutar suites DOC-88 relevantes y declarar su carácter estático. Revisar diff/dependencias para demostrar SII sin cambios; no concluir compatibilidad real únicamente por un diff vacío.
8. Comprobar rendimiento contra volumen y navegadores acordados, con fixtures y entorno autorizados: tiempos, memoria cliente, límites de petición, disco temporal, consultas/concurrencia. Si no hay volumen objetivo o autorización, registrar pendiente; no inventar una capacidad máxima ni ejecutar carga real.
9. Verificar teclado/foco, estados de error, reabrir modal tras UpdatePanel, móvil objetivo y correspondencia de cifras/CSV con la base.

## ENTREGABLE FINAL

Matriz obligatoria de integridad aprobada:

| Caso | Resultado esperado |
|---|---|
| Publicar perfil completo sin ámbito | Rechazo; debe elegir EXPEDIENTE o LOTE. |
| Completo/EXPEDIENTE: A lista, B con archivo faltante o inválido | A puede avanzar; B no crea/almacena hasta completar sus condiciones. Ambas permanecen en el informe original. |
| Completo/LOTE: A lista, B con archivo faltante o inválido | Ninguna inicia efectos; el plan con error impeditivo global del lote no se confirma. |
| Completo/LOTE: todos recibidos y válidos | Se habilita procesamiento sin exigir una transacción global de todos los efectos. |
| Parcial | Ámbito efectivo NO_APLICA; procesar elegibles y conservar rechazados/pendientes, sin declarar completo con incidencias pendientes. |
| Publicar otra versión/cambiar perfil mientras el lote existe | No cambia modo/ámbito del lote; reanudar usa su snapshot. |
| Fallo después de almacenar en cualquiera de los ámbitos | Conservar IDs/efectos confirmados; recuperar pendientes sin duplicación ni borrado como rollback. |
| Dos peticiones coinciden con la última recepción | Barrera y reclamaciones impiden inicio prematuro o doble efecto. |

Esta matriz verifica dos ramas de un mismo motor y modelo de datos; no autoriza servicios/tablas duplicados por ámbito.

Agregar casos de integración transversales: esquema nuevo ausente sin romper Producción/carga individual; inclusión/publicación de archivos VB/recursos; CSRF en confirmación y multipart; modo parcial con errores individuales confirmable y errores globales bloqueantes; valores diferidos/fecha estable; nombre/extensión conservados con staging aislado; limpieza legacy que no consume el original; carga manual o SII concurrentes sobre recursos compartidos; consulta/reconciliación autorizada cuando se suspenden nuevos efectos. No cambiar expectativas o huellas DOC-88/DOC-90 para esconder una regresión.

Crear `Validacion/matriz-verificacion.md` y `Validacion/resultados.md` con caso, requisito, entorno, tipo de prueba, evidencia y estado: aprobado, fallido o no ejecutado. Mantener fixtures sin datos reales y scripts reproducibles. Entregar instrucciones de aplicación manual de SQL y validación posterior; no desplegar automáticamente.

Corregir defectos del alcance cuando sea posible y volver a verificar únicamente lo afectado. No cambiar SII para lograr pruebas verdes. Distinguir demostración con mocks, comprobación estática e integración autenticada. No declarar listo para operación si faltan evidencias críticas de permisos, integridad o recuperación; señalar exactamente qué validación queda pendiente y por qué.

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

## CRITERIOS DE ACEPTACIÓN

Ejecutar la matriz de integridad y recuperación ya definida, incluir carga individual, permisos, concurrencia con escritores externos y ausencia de cambios SII. Medir capacidad solo contra volumen y entorno autorizados.

## PRUEBAS OBLIGATORIAS

Ejecutar la matriz de integridad y recuperación ya definida, incluir carga individual, permisos, concurrencia con escritores externos y ausencia de cambios SII. Medir capacidad solo contra volumen y entorno autorizados.

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
| INT-01 | Entregas integradas y esquema aplicado por el equipo. | Ejecutar casos anteriores por dependencia y matriz de integridad de este prompt con datos descartables. | Cada capacidad tiene evidencia de UI, servidor y efecto persistido; no se aceptan solo HTTP 200 ni barras completas. |
| INT-02 | Entorno preparado por el equipo para ausencia/incompatibilidad del esquema nuevo. | Abrir Producción, usar carga individual y solicitar importación. | Capacidad nueva rechazada controladamente; módulo y carga individual siguen funcionando. |
| INT-03 | Presupuesto, navegadores y concurrencia expresamente autorizados. | Ejecutar volumen acordado, interrupciones y recuperación; repetir regresión relacionada. | Sin pérdida/duplicación; métricas contra umbrales acordados; pendientes y riesgos documentados sin inventar capacidad. |

Para cada caso registrar estado EJECUTADO_OK, FALLIDO o NO_EJECUTADO, SHA probado, ambiente, datos sintéticos, pasos/comando real, evidencia saneada de UI/HTTP y comprobaciones persistidas de solo lectura. No declarar aprobada una capacidad únicamente con mocks o revisión estática. Un caso bloqueado debe identificar dependencia y responsable; no se omite del informe.

Entregar especificación automatizada y comando reproducible dentro de la infraestructura existente; el nombre final se inventaría en `Diseno/design.md`. Si la autorización no permite ejecutarla, entregar suite y evidencia local pertinente, manteniendo la validación real pendiente. No habilitar permisos ni ejecutar cargas por inferencia de este prompt.

## DOCUMENTACIÓN TÉCNICA

Actualizar documentación existente en `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/`: `Diseno/` para contratos y decisiones, y `Validacion/` para evidencia. Mantener guía 00 y consumidores sincronizados si cambia un contrato. No abrir una segunda fuente de verdad. Registrar archivos y funciones afectados, dependencia propietaria, casos ejecutados/no ejecutados y riesgos pendientes. Una revisión estructural PASS no certifica implementación ni ausencia de regresiones.
