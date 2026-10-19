# Prompt 08 — Creación y resolución de expedientes

## ROL ESPERADO

Actúa como arquitecto y desarrollador senior en ASP.NET WebForms, VB.NET, JavaScript y MySQL 5.1, respetando las fronteras existentes y el alcance de esta entrega.

## OBJETIVO Y ENCARGO

Lee guía 00, prompt 03, diseño 04 y contratos operativos 07. Implementa el adaptador de expedientes, sin cambiar SII ni crear escrituras documentales que omitan reglas existentes.

## Evidencia a verificar

Inspecciona `Gestion/ClassGaExpediente.vb`, `Registrar_Expediente_Conservacion`, y la composición en `Gestion/ClassGaProducionDocumental.vb`, `Agregar_expediente_produccion_carpeta_a_la_estructura`. El segundo recorrido depende de controles de interfaz y no es por sí mismo un servicio reusable. Verifica parámetros, tipos de unidad, fechas, responsables, clasificación, nivel padre y efectos físicos/XML. Respeta los límites funcionales observados, incluidos los del código aunque su columna permita más caracteres.

## Trabajo

- Recibir un comando con identidad de operación/raíz, autoridad validada, datos efectivos, nivel padre, ámbito y revisión. No obtener destino de selección mutable de sesión ni cambiar controles para simular navegación.
- Exigir que la coordinación haya habilitado la unidad conforme a la política de integridad. No convertir una llamada al adaptador en mecanismo para saltarse la espera de archivos.
- Consultar resultado previo de la misma raíz: si está confirmado, verificar pertenencia, relación al nivel y estado, y devolverlo. Diferenciar reanudación del mismo lote de coincidencia con otro.
- Definir y aplicar la política de código aprobada. No usar nombre visible como clave única global, ni concatenar sufijos para resolver conflictos silenciosamente.
- Revalidar permisos y nivel antes de crear; cambios de ámbito/ubicación requieren revisión. No crear estructuras nuevas ni asignar un autorregistro para obtener valores predeterminados.
- Invocar la operación existente compatible mediante un adaptador nuevo. Verificar ID de expediente y relación real con el nivel confirmado; conservar ambos identificadores mediante el repositorio operativo.
- Examinar fallos entre creación legacy y registro de resultado. Correlacionar solo con evidencia fiable: un nombre coincidente no demuestra que la operación creó ese expediente. Si no se puede determinar el resultado, registrar INCIERTO y bloquear recreación automática.
- No borrar expedientes con efectos parciales para simular rollback. Reportar fase y recuperación permitida sin exponer SQL o rutas sensibles al usuario.

## CRITERIOS DE ACEPTACIÓN

Después de crear y verificar el expediente, devolver los datos reales requeridos por fuentes diferidas de índices. No convertirlos en reglas nuevas ni modificar la versión del perfil. Caracterizar también concurrencia con la creación manual existente: una exclusión propia del lote no protege otros escritores. No exponer la creación como endpoint que permita saltarse la habilitación de 10; el contrato interno y sus precondiciones se prueban aisladamente antes de integrar el coordinador.

Prueba dos raíces en un nivel, nombre repetido en ámbitos distintos, reejecución de la misma raíz, permisos revocados, nivel movido, código inválido y fallo después de crear pero antes de registrar. Demostrar que no se usa un destino nuevo por cambiar la selección del árbol. Entregar matriz de dependencias de sesión/efectos, adaptador, contrato de resultado conocido/rechazado/incierto y pruebas. Reutilizar repositorios 07; cualquier DDL adicional se entrega como SQL MySQL 5.1 coordinado, nunca contra SII.

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

Reejecutar una raíz devuelve el expediente confirmado; permiso revocado impide crear; pérdida de respuesta tras creación queda incierta y bloquea recreación. Verificar relación con el nivel original.

Ejecutar pruebas unitarias o QA manual reproducible según el impacto, además de integración para persistencia y efectos. Registrar comandos, entorno, resultado y limitaciones. Para cambios VB/proyecto, compilar con MSBuild usando la configuración real de `GestionDocumental-Docuarchi.net.vbproj`; no sustituirlo por un build .NET moderno. Si faltan herramientas/referencias, registrar la limitación. Para JavaScript, comprobar sintaxis con `node --check <archivo-real>` y comportamiento con la infraestructura existente. Las suites DOC-88 aplicables son evidencia estática, no prueba de persistencia real.

La validación E2E del comportamiento entregado es parte integral del mismo cambio: código, pruebas, validación autorizada y evidencia saneada forman su unidad de entrega. El prompt 12 consolida regresión transversal; no sustituye las pruebas de cada componente. Para servicios/adaptadores internos, validar primero contratos y fallos mediante unitarias/integración; no crear endpoints de prueba para eludir el coordinador. Si el recorrido depende de componentes posteriores, registrar integración pendiente y no declarar cerrado el comportamiento integrado.

Reutilizar exclusivamente autenticación, configuración, validadores y utilidades de `tools/e2e`; no crear login, arnés, proyecto Playwright ni .env paralelos. Leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md` antes de autenticar. Ejecutar pruebas reales solo con autorización explícita del ambiente, cuentas y datos descartables; este prompt no concede esa autorización. Cubrir control de acceso, lectura sin mutación, escrituras autorizadas, concurrencia y regresión relacionada con esta entrega.

Usar secretos efímeros; no imprimir ni guardar credenciales, cookies, tokens o cadenas de conexión. Consultas de control solo SELECT; conservar evidencia saneada. Respetar gates de seguridad existentes, sin habilitar arbitrariamente funciones o permisos para probar. No reintroducir flags ni listas de usuarios/grupos como activación de la experiencia moderna. No cerrar sin validación requerida: ante ambiente o autorización ausente, registrar bloqueo explícito, casos no ejecutados y motivo; mocks y simulaciones no sustituyen evidencia real.

## DOCUMENTACIÓN TÉCNICA

Actualizar documentación existente en `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/`: `Diseno/` para contratos y decisiones, y `Validacion/` para evidencia. Mantener guía 00 y consumidores sincronizados si cambia un contrato. No abrir una segunda fuente de verdad. Registrar archivos y funciones afectados, dependencia propietaria, casos ejecutados/no ejecutados y riesgos pendientes. Una revisión estructural PASS no certifica implementación ni ausencia de regresiones.

## ENTREGABLE FINAL

Resolver y verificar expedientes bajo el nivel confirmado mediante un adaptador interno, conservando la incertidumbre sin recreaciones automáticas. Entregar los artefactos especificados en este prompt y una relación verificable de cambios, pruebas, resultados y pendientes. No declarar funcionalidades completas por entregar únicamente stubs, mocks o documentación.
