# Prompt PB03 para aportar lecturas de contexto cuando sean necesarias

**Estado:** borrador opcional de implementación. Requiere PB00/PB01 y sus decisiones; D05, D06, D09 y D16 aprobadas según los datos solicitados.

## ROL ESPERADO

Actúa como arquitecto y desarrollador senior de ASP.NET WebForms, VB.NET sobre .NET Framework 4.6.1 y JavaScript/jQuery. Respeta los contratos vigentes de Producción documental y el alcance específico del paquete.

## Refinamiento e implementación del paquete

Comienza por refinar las decisiones indicadas en este archivo y sus dependencias, sin exigir que estén cerradas las 18 decisiones de la exploración. Relee el código y los resultados anteriores, delimita el alcance y registra en la exploración la opción, evidencia y revisión que realmente se acuerden. Los detalles técnicos dentro del alcance autorizado se justifican; los cambios de comportamiento, permisos, semántica o alcance requieren aprobación del usuario.

Una decisión pendiente no impide iniciar este refinamiento. Antes de modificar el código afectado debe quedar resuelta la decisión necesaria y existir autorización de implementación. Continúa el análisis independiente si una respuesta sigue pendiente. No conviertas las alternativas recomendadas en aprobaciones ni vuelvas a pedir acuerdos ya registrados. Después implementa y verifica solo la parte habilitada.

## RESTRICCIONES CRÍTICAS

- Preservar identidad, nombres canónicos, reglas de negocio, permisos y formatos públicos de los consumidores vigentes. No romper la búsqueda, la selección ni los accesos alternativos para cumplir solo el diseño visual.
- No sustituir el TreeView, migrar framework, crear tablas/endpoints o ampliar permisos como efecto implícito del paquete. Toda necesidad fuera de alcance se refina y registra antes de implementarla.
- No ejecutar pruebas autenticadas, carga ni escrituras del ambiente sin su autorización específica. No imprimir ni guardar credenciales, cookies, tokens o cadenas de conexión en prompts o evidencia versionada. Los SELECT de control no modifican dominio ni auditoría documental.

## OBJETIVO

Actúa como arquitecto de ASP.NET WebForms y consultas MySQL. Lee [el contexto común](contexto-comun.md) y determina qué información necesita realmente PF02 que no esté ya disponible de forma autorizada. Implementa solo esas lecturas. Si no hace falta backend adicional, documenta la omisión y el origen de cada dato del cliente.

## Inventario de necesidades

El flujo parte de una necesidad concreta del cliente, comprueba si existe una fuente autorizada y decide si hace falta una lectura adicional. Si se implementa, valida identidad y autoridad antes de consultar y devuelve solo contenido directo, capacidades o indicadores del alcance acordado; si no hace falta, registra la omisión y la fuente que usará PF02.

Para ruta, contenido directo, capacidades e indicadores registra consumidor, fuente existente, vigencia, identidad y motivo para consultar. No uses los números o permisos simulados de la maqueta como datos de producción.

El contenido de un nivel es su conjunto directo de subniveles y expedientes según D05. No conviertas esta lectura en agregación recursiva de documentos. La búsqueda aprobada sobre nodos cargados no necesita automáticamente un buscador global.

## Si se requieren nuevas lecturas

1. Reutiliza identidad y autoridad PB01. El usuario y la empresa no se aceptan como autoridad solo porque lleguen en la petición.
2. Define operaciones acotadas para las necesidades aprobadas. No expongas SQL libre, nombres de tablas ni un ejecutor genérico de métodos.
3. Valida tipo/referencia y aplica visibilidad y reglas de acceso antes de devolver nombres, rutas, hijos o capacidades. No reveles un elemento ajeno para explicar por qué está denegado.
4. Parametriza consultas y ajusta tipos, collation y sintaxis al motor confirmado. No añadas esquema, índices o versiones de base de datos por suposición.
5. Evita consultas por cada nodo y carga repetida de toda la jerarquía. Si la respuesta es parcial, limita su alcance de forma explícita y proporciona una navegación compatible; no inventes totales completos a partir de una página.
6. Define ausencia, vacío, denegación y fallo con resultados distintos y controlados. Incluye la identidad y correlación necesarias para rechazar respuestas tardías en PF02.
7. Las capacidades se calculan por operación con sus reglas vigentes. Distingue no aplicable, no autorizado y no disponible cuando el contrato lo requiera. Cada escritura mantiene revalidación en servidor; un permiso devuelto antes no autoriza ejecución futura.
8. Contadores deben indicar si miden hijos directos, nodos disponibles o total completo. No consultes cantidad de documentos por expediente solo para decorar el árbol.

## Límites de efectos

Estas lecturas no cambian documentos, estados, relaciones, permisos ni auditoría documental. No marcan tareas, importaciones o expedientes como vistos/procesados salvo una decisión de negocio propia. Si se mantiene sesión de interfaz por compatibilidad, documenta ese efecto y no lo confundas con una escritura de dominio.

No sustituyas el cargador documental ni el adaptador de importación para proporcionar ruta o capacidades del explorador. Si el contrato aprobado no necesita datos nuevos, termina con omisión justificada en lugar de crear servicios vacíos.

## CRITERIOS DE ACEPTACIÓN

- Nivel vacío y mixto; expediente usado como nivel; relación inválida.
- Propietario y receptor con capacidades diferentes; revocación de permiso entre lectura y escritura.
- Rama oculta, usuario/empresa ajenos, ruta inválida y ausencia de recurso.
- Identidad de respuesta y solicitud desfasadas; fallo de consulta distinto de contenido vacío.
- Cantidad de consultas y tamaño de respuesta acordes con el contrato; sin consulta por insignia ni carga global innecesaria.

Ejecuta pruebas locales de autorización, consultas parametrizadas y forma de respuesta, con sustituciones o datos aislados. Una inspección de base real necesita autorización aplicable y solo SELECT de control.

## ENTREGABLE FINAL

Crea `Doc/Diagnostico/modernizacion-arbol-produccion/implementacion/PB03-lecturas.md`. Si se implementa, incluye contratos, origen/vigencia de datos, consultas, errores, pruebas y compilación. Si se omite, incluye la necesidad evaluada y la fuente existente de cada dato para que PF02 pueda integrarse sin asumir un servicio inexistente.

## PRECONDICIONES DE RUTAS

Antes de modificar varias responsabilidades, inventariar los archivos exactos existentes y propuestos en `Doc/Diagnostico/modernizacion-arbol-produccion/implementacion/design.md`, indicando responsabilidad, consumidor y razón del cambio. Este inventario se completa durante el refinamiento y no altera el cambio OpenSpec DOC-127.

| Responsabilidad | Ruta de referencia para inventariar |
|---|---|
| Modelos o valores del explorador, solo si son necesarios | `Modelo/ProduccionDocumental/Explorador/` |
| DTOs del contrato, solo si hacen falta | `DTOs/ProduccionDocumental/Explorador/` |
| Servicios o casos de uso acotados | `Services/ProduccionDocumental/Explorador/` |
| Repositorios cuando sean necesarios | `Infrastructure/Repositories/ProduccionDocumental/Explorador/` |
| Adaptadores WebForms específicos | `Infrastructure/ProduccionDocumental/Explorador/` |
| Servicio web y consumidores actuales | `webservice/WebServiceProducion.asmx` y `webservice/WebServiceProducion.asmx.vb` |
| Página y eventos | `Gestion/WebFormProducionDocumental.aspx` y `Gestion/WebFormProducionDocumental.aspx.vb` |

Las carpetas `Explorador/` son ubicaciones propuestas para responsabilidades nuevas que se justifiquen, no componentes existentes ni capas obligatorias. Primero reutiliza lo compatible y elige archivos concretos en el inventario. No se crean clases vacías para llenar la tabla. Registrar explícitamente cada archivo nuevo compilado o Content en `GestionDocumental-Docuarchi.net.vbproj` cuando corresponda y verificar su publicación.


## Archivos de integración que se deben localizar

Revisa `Gestion/WebFormProducionDocumental.aspx`, `Gestion/WebFormProducionDocumental.aspx.vb`, `js/gestion/WebFormProducionDocumental.js` y `webservice/WebServiceProducion.asmx.vb`, además del inventario de PB00. No se autoriza modificar todas estas rutas: el refinamiento delimita las realmente afectadas.

## PRUEBAS OBLIGATORIAS

Ejecuta pruebas unitarias o focales que verifiquen comportamiento real del alcance, además de los casos de aceptación de este prompt. Registra cada comando, resultado y limitación; no basta comprobar texto de clases o que existe un archivo. Si cambia VB.NET o el proyecto, aporta compilación MSBuild compatible con .NET Framework 4.6.1 y su configuración real. Si solo cambia presentación, justifica por qué no aplica build de backend y verifica carga de recursos e integración en la página real.

El recorrido E2E o la validación manual reproducible con evidencia forman parte integral del mismo cambio funcional y de su aceptación. No se consideran una entrega posterior que permita anunciar el paquete como validado mientras falta ese recorrido. La validación local con respuestas controladas se identifica como tal.

Reutilizar la infraestructura existente en `tools/e2e`: autenticación, configuración, inventario, validadores y evidencia. No crear login, arnés, proyecto Playwright, configuración ni `.env` paralelos. Antes de autenticar leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`; ejecutar únicamente con ambiente, cuentas y datos descartables expresamente autorizados para el recorrido.

Usar secretos efímeros; no imprimir, guardar ni persistir credenciales, cookies, tokens o cadenas de conexión. Los controles de base serán solo SELECT y la evidencia saneada excluirá datos documentales sensibles. Cubrir autorización, lectura sin mutación de dominio, escrituras autorizadas cuando el alcance las incluya, concurrencia y regresión relacionada. En una lectura o una entrega de presentación, la ausencia de escrituras es un criterio verificable, no una razón para inventarlas.

No habilitar arbitrariamente flags o gates existentes. No introducir activación por usuarios/grupos ni reintroducir `WorkflowCentroTrabajoModernActive`. No cerrar la aceptación integral sin validación autorizada: si falta ambiente, autorización o infraestructura, registrar bloqueo explícito de esa aceptación y lo que sí se verificó, sin usar mocks, simulaciones ni evidencia ficticia para acreditar un recorrido real.

## DOCUMENTACIÓN TÉCNICA

Ubicar y actualizar documentación existente de esta capacidad: exploración, guía, contratos del paquete y documento de resultado indicado en ENTREGABLE FINAL. Documenta funciones creadas o modificadas con ruta, parámetros, retorno, errores y consumidores cuando corresponda. Incluye decisiones aplicadas, evidencia saneada, compatibilidad y pendientes; no dupliques las aprobaciones fuera de la exploración ni des por existentes los resultados de paquetes futuros.
