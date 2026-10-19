# Prompt 06 — Validación y plan de importación

## ROL ESPERADO

Actúa como arquitecto y desarrollador senior en ASP.NET WebForms, VB.NET, JavaScript y MySQL 5.1, respetando las fronteras existentes y el alcance de esta entrega.

## OBJETIVO Y ENCARGO

Lee guía 00, prompts 02/03 y diseño 04. Implementa el servicio de preparación/preflight sin crear expedientes ni documentos. Depende del perfil publicado, sus reglas y los contratos definidos; no construir un segundo motor de reglas.

## Contrato y trabajo

Esta entrega es propietaria del servicio de validación del contexto de apertura y de preparación del plan. La persistencia de plan, si se requiere, se consume de 07-A conforme al diseño 04; 07-B depende del preflight terminado, no al revés. No crear repositorio ni script paralelo para resolver esa dependencia.

Recibe contexto del nivel de apertura, referencia de perfil/versión e inventario completo con identidades estables. La identidad autenticada y ámbito efectivo se resuelven en servidor. Devuelve plan identificado/versionado, resultados por raíz/archivo, valores resueltos, incidencias y posibilidad de confirmar.

1. Valida forma, cuotas, duplicación de identificadores, rutas relativas y completitud del manifiesto. Si es fragmentado, no habilites revisión final hasta recibir todos sus fragmentos comprobados.
2. Recupera versión publicada y configuración autorizada. Valida nivel, gabinete, permisos y coherencia de clasificación/tipologías. Obtener un perfil no concede permisos de creación/carga.
3. Aplica agrupación explícita: una raíz por expediente en el nivel recibido; subcarpetas conforme al perfil. No fabricar carpetas vacías ni niveles.
4. Usa el resolvedor para constantes, valores de carpeta, nombre de archivo y datos del expediente. Distingue código de tipología de ID interno. Una asignación final por campo; conserva ceros iniciales cuando corresponda y valida límites funcionales además de físicos.
5. Resuelve política de código de expediente y conflictos por identidad/ámbito, no solo nombre. Las consultas previas no garantizan ausencia de carreras; dejar explícita la comprobación al ejecutar.
6. Aplica COMPLETO_OBLIGATORIO o PARCIAL_PERMITIDO y su ámbito confirmado. Clasifica errores impeditivos/advertencias y unidades afectadas. En completo no excluyas inválidos del denominador ni autorices efectos parciales. En parcial reporta cada rechazado.
7. Conserva o protege el plan mediante el mecanismo definido en 04, asociado a usuario/ámbito, versión, inventario y destino. No aceptar valores finales reenviados por el cliente como autoridad. Define expiración/revisión y qué cambios lo invalidan.
8. La ausencia de contenido binario limita la validación a metadatos; no marcar como verificados formato real, huella o recepción. Documenta valores que solo se validarán al recibir.

## CRITERIOS DE ACEPTACIÓN

Implementar ambos ámbitos aprobados. En COMPLETO_OBLIGATORIO/EXPEDIENTE, una raíz elegible puede confirmarse/procesarse sin omitir del inventario otras raíces bloqueadas; cada raíz espera todos sus archivos recibidos y validados. En COMPLETO_OBLIGATORIO/LOTE, un error impeditivo en cualquier archivo bloquea la confirmación global y la ejecución espera recepción/validación total. Errores globales bloquean siempre. El ámbito proviene de la versión del perfil, no de un cambio libre en la ventana. No confundir elegibilidad del plan con recepción ya completada.

Probar errores globales frente a errores individuales: en parcial debe ser posible confirmar unidades elegibles manteniendo rechazados visibles. Devolver `puedeConfirmar`, alcance bloqueado y motivos. Para IDs/consecutivos inexistentes y campos heredados que dependen de ellos, emitir fuentes diferidas tipadas; nunca IDs ficticios ni un valor definitivo inventado. La materialización posterior debe validar la misma regla/versionado y conservar valores efectivos sin alterar el plan aprobado.

Pruebas de dos formatos de nombre configurables, tipo desconocido, campo requerido, conversión/longitud, subcarpetas, raíz homónima, permiso revocado, nivel ajeno, inventario incompleto, plan alterado y cambio de perfil. Comprobar completo/parcial según ámbito confirmado, sin efecto documental ni llamada al adaptador de creación. El mismo inventario/configuración produce un plan determinista salvo campos de control documentados.

Entrega servicio, entrada HTTP compatible, repositorio de lectura/plan según diseño y pruebas. Si guardar planes necesita DDL, coordina su propiedad con el script operativo del 07 y entrega SQL manual compatible; no crear un repositorio alternativo de lotes.

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

Comparar planes para inventario y versión iguales; rechazar nivel ajeno y plan manipulado; conservar errores individuales en parcial y bloquear errores globales; demostrar cero llamadas de creación/almacenamiento.

Ejecutar pruebas unitarias o QA manual reproducible según el impacto, además de integración para persistencia y efectos. Registrar comandos, entorno, resultado y limitaciones. Para cambios VB/proyecto, compilar con MSBuild usando la configuración real de `GestionDocumental-Docuarchi.net.vbproj`; no sustituirlo por un build .NET moderno. Si faltan herramientas/referencias, registrar la limitación. Para JavaScript, comprobar sintaxis con `node --check <archivo-real>` y comportamiento con la infraestructura existente. Las suites DOC-88 aplicables son evidencia estática, no prueba de persistencia real.

La validación E2E del comportamiento entregado es parte integral del mismo cambio: código, pruebas, validación autorizada y evidencia saneada forman su unidad de entrega. El prompt 12 consolida regresión transversal; no sustituye las pruebas de cada componente. Para servicios/adaptadores internos, validar primero contratos y fallos mediante unitarias/integración; no crear endpoints de prueba para eludir el coordinador. Si el recorrido depende de componentes posteriores, registrar integración pendiente y no declarar cerrado el comportamiento integrado.

Reutilizar exclusivamente autenticación, configuración, validadores y utilidades de `tools/e2e`; no crear login, arnés, proyecto Playwright ni .env paralelos. Leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md` antes de autenticar. Ejecutar pruebas reales solo con autorización explícita del ambiente, cuentas y datos descartables; este prompt no concede esa autorización. Cubrir control de acceso, lectura sin mutación, escrituras autorizadas, concurrencia y regresión relacionada con esta entrega.

Usar secretos efímeros; no imprimir ni guardar credenciales, cookies, tokens o cadenas de conexión. Consultas de control solo SELECT; conservar evidencia saneada. Respetar gates de seguridad existentes, sin habilitar arbitrariamente funciones o permisos para probar. No reintroducir flags ni listas de usuarios/grupos como activación de la experiencia moderna. No cerrar sin validación requerida: ante ambiente o autorización ausente, registrar bloqueo explícito, casos no ejecutados y motivo; mocks y simulaciones no sustituyen evidencia real.

## DOCUMENTACIÓN TÉCNICA

Actualizar documentación existente en `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/`: `Diseno/` para contratos y decisiones, y `Validacion/` para evidencia. Mantener guía 00 y consumidores sincronizados si cambia un contrato. No abrir una segunda fuente de verdad. Registrar archivos y funciones afectados, dependencia propietaria, casos ejecutados/no ejecutados y riesgos pendientes. Una revisión estructural PASS no certifica implementación ni ausencia de regresiones.

## ENTREGABLE FINAL

Preparar un plan autorizado y determinista, con incidencias y valores diferidos explícitos, sin crear expedientes ni almacenar documentos. Entregar los artefactos especificados en este prompt y una relación verificable de cambios, pruebas, resultados y pendientes. No declarar funcionalidades completas por entregar únicamente stubs, mocks o documentación.
