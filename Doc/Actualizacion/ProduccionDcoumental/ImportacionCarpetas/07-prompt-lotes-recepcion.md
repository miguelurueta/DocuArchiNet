# Prompt 07 — Lotes y recepción de archivos

## ROL ESPERADO

Actúa como arquitecto y desarrollador senior en ASP.NET WebForms, VB.NET, JavaScript y MySQL 5.1, respetando las fronteras existentes y el alcance de esta entrega.

## OBJETIVO Y ENCARGO

Lee guía 00, prompt 03 y diseño 04. Implementa la persistencia operativa y recepción recuperable de archivos de un plan validado por 06. Esta entrega no crea expedientes ni almacena documentos definitivos.

## Trabajo

Dividir la entrega en 07-A (DDL y repositorios de plan/operación a partir de contratos 04 y perfil 02) y 07-B (confirmación/recepción integradas al preflight 06). 07-A no espera a que 06 esté implementado. Un único script operativo y una única implementación de repositorios sirven a ambas fases. La protección HTTP y autorización se aplican también a multipart y reintentos.

- Implementa repositorios de lote, raíz, archivo e intentos/eventos definidos, con transacciones acotadas, consultas parametrizadas y separación de lectura/escritura. Expón contratos que consumirán 08–11; no acoples al HTML.
- Confirmar plan mediante clave de idempotencia y restricción única de ámbito. Guardar configuración efectiva, nivel, inventario original y política completa/parcial. Misma clave con otro payload: conflicto; repetición idéntica: IDs y estado previos. Evita check-then-insert sin exclusión transaccional.
- Recibir cada archivo solo si corresponde al lote autorizado y estado permitido. Rechazar IDs cruzados aunque los nombres coincidan. El contrato de recepción no acepta un gabinete o expediente arbitrario que eluda el plan.
- Escribir en staging controlado mientras se recibe, con identidad física generada, contención de ruta y acceso no público. No concatenar nombre original/ruta relativa para construir la ruta de escritura. Un archivo parcial no se considera RECIBIDO.
- Validar bytes reales, cuotas, formato permitido, huella y completitud. Los límites vienen de configuración aprobada y del servidor; no elevar IIS/ASP.NET globalmente para ocultar un límite. No fingir carga fragmentada si no está implementada.
- Coordinar cierre/renombrado del temporal y registro de recepción. Diseña recuperación de escritura completada sin fila actualizada, fila que referencia temporal ausente y recepción concurrente. No prometer una transacción única entre disco y MySQL.
- Repetir contenido ya confirmado devuelve recepción previa; contenido distinto bajo la misma identidad requiere rechazo/revisión. Proteger cuotas frente a concurrencia y a reintentos contabilizados dos veces.
- No permitir que el cliente fuerce estados. Implementa revisión/exclusión persistida según diseño para que 10 reclame fases de forma segura.

## SQL y entregables

El staging confirmado pertenece a esta capa y conserva el contenido necesario para recuperación. Entregar a 09 una referencia controlada, no autorización implícita para consumir el único original. Su copia de trabajo legacy, huella, nombre/extensión y limpieza son responsabilidades coordinadas con 09/11 según la guía 00.

Entregar `Sql/02-carga-carpetas-mysql51.sql` con DDL completo de entidades operativas, índices y relaciones compatibles. Referenciar las tablas de perfil desplegadas por `Sql/01-perfil-importacion-mysql51.sql`, sin recrearlas. Añadir pre/postcomprobaciones de solo lectura, orden de aplicación, repetición y recuperación parcial. El usuario aplica el SQL. Si ya existe un despliegue, entregar incremento separado.

## CRITERIOS DE ACEPTACIÓN

Probar doble confirmación simultánea, clave repetida con payload diferente, archivo ajeno, ruta maliciosa, exceder cuota, archivo truncado, dos recepciones concurrentes, desconexión, resultado de recepción perdido y contenido cambiado. Verificar que completo espera todos los archivos de la unidad exigida y que recepción no produce efectos documentales. Las pruebas de transacciones requieren un entorno de pruebas autorizado; reportar lo que solo se probó con dobles.

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

Confirmación simultánea devuelve un lote; payload distinto con la misma clave se rechaza; transferencia truncada no queda RECIBIDO; reenvío y cuotas concurrentes no duplican conteos.

Ejecutar pruebas unitarias o QA manual reproducible según el impacto, además de integración para persistencia y efectos. Registrar comandos, entorno, resultado y limitaciones. Para cambios VB/proyecto, compilar con MSBuild usando la configuración real de `GestionDocumental-Docuarchi.net.vbproj`; no sustituirlo por un build .NET moderno. Si faltan herramientas/referencias, registrar la limitación. Para JavaScript, comprobar sintaxis con `node --check <archivo-real>` y comportamiento con la infraestructura existente. Las suites DOC-88 aplicables son evidencia estática, no prueba de persistencia real.

La validación E2E del comportamiento entregado es parte integral del mismo cambio: código, pruebas, validación autorizada y evidencia saneada forman su unidad de entrega. El prompt 12 consolida regresión transversal; no sustituye las pruebas de cada componente. Para servicios/adaptadores internos, validar primero contratos y fallos mediante unitarias/integración; no crear endpoints de prueba para eludir el coordinador. Si el recorrido depende de componentes posteriores, registrar integración pendiente y no declarar cerrado el comportamiento integrado.

Reutilizar exclusivamente autenticación, configuración, validadores y utilidades de `tools/e2e`; no crear login, arnés, proyecto Playwright ni .env paralelos. Leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md` antes de autenticar. Ejecutar pruebas reales solo con autorización explícita del ambiente, cuentas y datos descartables; este prompt no concede esa autorización. Cubrir control de acceso, lectura sin mutación, escrituras autorizadas, concurrencia y regresión relacionada con esta entrega.

Usar secretos efímeros; no imprimir ni guardar credenciales, cookies, tokens o cadenas de conexión. Consultas de control solo SELECT; conservar evidencia saneada. Respetar gates de seguridad existentes, sin habilitar arbitrariamente funciones o permisos para probar. No reintroducir flags ni listas de usuarios/grupos como activación de la experiencia moderna. No cerrar sin validación requerida: ante ambiente o autorización ausente, registrar bloqueo explícito, casos no ejecutados y motivo; mocks y simulaciones no sustituyen evidencia real.

07-A se verifica con pruebas de repositorios/transacciones y revisión del SQL en entorno autorizado; no depende de una UI ni de 06 implementado. 07-B añade el recorrido real de confirmación/recepción. Registrar la evidencia y estado de cierre por fase, sin exigir en 07-A efectos documentales que no le pertenecen.

## DOCUMENTACIÓN TÉCNICA

Actualizar documentación existente en `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/`: `Diseno/` para contratos y decisiones, y `Validacion/` para evidencia. Mantener guía 00 y consumidores sincronizados si cambia un contrato. No abrir una segunda fuente de verdad. Registrar archivos y funciones afectados, dependencia propietaria, casos ejecutados/no ejecutados y riesgos pendientes. Una revisión estructural PASS no certifica implementación ni ausencia de regresiones.

## ENTREGABLE FINAL

Entregar primero la persistencia 07-A y después confirmación y recepción 07-B; mantener un único esquema operativo y staging recuperable. Entregar los artefactos especificados en este prompt y una relación verificable de cambios, pruebas, resultados y pendientes. No declarar funcionalidades completas por entregar únicamente stubs, mocks o documentación.
