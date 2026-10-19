# Prompt 13 — Ejecutor desatendido durable (opcional)

## ROL ESPERADO

Actúa como arquitecto y desarrollador senior en ASP.NET WebForms, VB.NET, JavaScript y MySQL 5.1, respetando las fronteras existentes y el alcance de esta entrega.

## Activación y dependencia

Esta entrega es una evolución separada de la primera implementación; no se ejecuta por completar 12. Requiere decisión explícita de incorporar procesamiento desatendido y definir dónde se alojará. Lee guía 00, diseño y componentes reales 07–12. La creación de este prompt no autoriza instalar servicios, desplegar ni modificar infraestructura.

## OBJETIVO Y ENCARGO

Implementa ejecución durable para archivos ya recibidos, reutilizando el coordinador y sus contratos. No constituye una solución para leer archivos pendientes del dispositivo con el navegador cerrado.

## Trabajo

- Caracteriza dependencias restantes de HttpContext, Session, identidad, temporales y recursos locales. Un adaptador que simula sesión de usuario no satisface independencia del navegador. No modificar SII ni contratos compartidos para convertirlos en worker.
- Elige el host según infraestructura disponible y autorizada; documenta cuenta técnica, permisos mínimos, acceso a staging/gabinetes, arranque/parada y supervisión. No introducir una plataforma por preferencia sin necesidad.
- Recupera trabajo persistido y reclama fases atómicamente. Implementa señal de vida, revisiones y protección frente a dos ejecutores y frente al coordinador por peticiones. Definir quién es dueño del avance de cada lote; no mantener dos rutas competidoras sin exclusión.
- Autorización explícita: registra solicitante original y ejecutor técnico; revalida permisos y disponibilidad conforme al contrato. No usar cookies del usuario ni extender una sesión para ejecutar trabajos.
- Reiniciar después de caída/reciclado sin perder fases; reconciliar incertidumbre antes de repetir efectos. La exclusión expirada no basta para asegurar que un proceso anterior dejó de producir efectos.
- Mantener políticas completa/parcial, snapshot y nivel fijo del lote. Solo procesar contenido disponible y validado. Conservar detención segura, consulta de estado y acciones del usuario.
- Definir despliegue y retorno a modalidad anterior sin dos ejecutores activos y sin pérdida de trazabilidad. Entregar configuración de ejemplo sin secretos y SQL incremental MySQL 5.1 cuando aplique.

## CRITERIOS DE ACEPTACIÓN

En entorno autorizado demostrar continuidad tras cerrar navegador con archivos recibidos, reinicio del ejecutor, duplicación de instancias, pérdida de acceso al disco, permiso revocado y parada durante almacenamiento. Los archivos no recibidos siguen pendientes, sin acceso supuesto al disco local. No declarar completada esta evolución si los adaptadores aún requieren sesión del navegador; documentar la dependencia bloqueante y no ocultarla creando HttpContext ficticio.

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

Tras cerrar el navegador, avanzar solo archivos recibidos; reiniciar ejecutor y duplicar instancias sin duplicar efectos; pérdida de disco o permiso produce detención controlada.

Ejecutar pruebas unitarias o QA manual reproducible según el impacto, además de integración para persistencia y efectos. Registrar comandos, entorno, resultado y limitaciones. Para cambios VB/proyecto, compilar con MSBuild usando la configuración real de `GestionDocumental-Docuarchi.net.vbproj`; no sustituirlo por un build .NET moderno. Si faltan herramientas/referencias, registrar la limitación. Para JavaScript, comprobar sintaxis con `node --check <archivo-real>` y comportamiento con la infraestructura existente. Las suites DOC-88 aplicables son evidencia estática, no prueba de persistencia real.

La validación E2E del comportamiento entregado es parte integral del mismo cambio: código, pruebas, validación autorizada y evidencia saneada forman su unidad de entrega. El prompt 12 consolida regresión transversal; no sustituye las pruebas de cada componente. Para servicios/adaptadores internos, validar primero contratos y fallos mediante unitarias/integración; no crear endpoints de prueba para eludir el coordinador. Si el recorrido depende de componentes posteriores, registrar integración pendiente y no declarar cerrado el comportamiento integrado.

Reutilizar exclusivamente autenticación, configuración, validadores y utilidades de `tools/e2e`; no crear login, arnés, proyecto Playwright ni .env paralelos. Leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md` antes de autenticar. Ejecutar pruebas reales solo con autorización explícita del ambiente, cuentas y datos descartables; este prompt no concede esa autorización. Cubrir control de acceso, lectura sin mutación, escrituras autorizadas, concurrencia y regresión relacionada con esta entrega.

Usar secretos efímeros; no imprimir ni guardar credenciales, cookies, tokens o cadenas de conexión. Consultas de control solo SELECT; conservar evidencia saneada. Respetar gates de seguridad existentes, sin habilitar arbitrariamente funciones o permisos para probar. No reintroducir flags ni listas de usuarios/grupos como activación de la experiencia moderna. No cerrar sin validación requerida: ante ambiente o autorización ausente, registrar bloqueo explícito, casos no ejecutados y motivo; mocks y simulaciones no sustituyen evidencia real.

Todas estas verificaciones operativas son posteriores a la activación explícita del alcance opcional y a la elección autorizada del host. No ejecutar ni instalar el worker durante la revisión de este prompt.

## DOCUMENTACIÓN TÉCNICA

Actualizar documentación existente en `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/`: `Diseno/` para contratos y decisiones, y `Validacion/` para evidencia. Mantener guía 00 y consumidores sincronizados si cambia un contrato. No abrir una segunda fuente de verdad. Registrar archivos y funciones afectados, dependencia propietaria, casos ejecutados/no ejecutados y riesgos pendientes. Una revisión estructural PASS no certifica implementación ni ausencia de regresiones.

## ENTREGABLE FINAL

Solo después de activar expresamente esta evolución, alojar el coordinador en un ejecutor durable para contenido recibido, sin depender del navegador. Entregar los artefactos especificados en este prompt y una relación verificable de cambios, pruebas, resultados y pendientes. No declarar funcionalidades completas por entregar únicamente stubs, mocks o documentación.
