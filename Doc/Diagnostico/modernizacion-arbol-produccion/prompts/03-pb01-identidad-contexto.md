# Prompt PB01 para validar identidad y contexto de las operaciones

**Estado:** borrador de implementación. Requiere PB00; decisiones D01, D08, D09, D10, D14 y alcance de D18 aprobados para contexto explícito. Si D08 conserva solo sesión, este diseño debe revisarse.

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

Actúa como arquitecto y desarrollador de ASP.NET WebForms/VB.NET. Lee [el contexto común](contexto-comun.md) y el inventario PB00. Implementa el contrato mínimo para que las rutas aprobadas resuelvan un destino explícito, autorizado y correlacionado, sin depender exclusivamente de la última selección mutable de sesión.

## Áreas previstas

Revisa `Gestion/WebFormProducionDocumental.aspx.vb`, `webservice/WebServiceProducion.asmx.vb`, `Gestion/Class_ra_pro_niveles_has_expediente_archivo.vb`, las validaciones de autoridad del módulo y sus consumidores JavaScript. Añade DTO/servicio/adaptador/repositorio solo si corresponde a la estructura real del proyecto; revisa inclusiones del `.vbproj`.

## Contrato a concretar

El flujo de contexto recibe identidad y correlación, valida la autoridad autenticada y la relación del recurso y devuelve el destino normalizado o su rechazo controlado. La acción posterior comprueba ese destino dentro de su propia petición; una lectura de contexto no concede autorización durable ni ejecuta la operación documental.

1. Define una representación tipada de nivel o aparición de expediente. Para nivel, referencia válida de nivel; para expediente, relación + nivel + expediente. Rechaza mezclas, referencias vacías, inválidas, desbordadas y relaciones que ya no corresponden al recurso.
2. Acuerda rangos con todos los consumidores legacy y evita conversiones silenciosas. Si el transporte usa cadenas decimales para IDs, normalízalas con una regla única y verifica el rango antes de convertir. No amplíes `Integer` a `UInteger` solo porque importación utilice otro tipo.
3. Obtén usuario/empresa de autoridad autenticada y valida recurso, ámbito, visibilidad aplicable y permisos específicos. La relación enviada debe comprobarse contra datos del servidor. No uses alias ni la primera relación encontrada como identidad.
4. Concreta correlación de página/solicitud y, si se adopta versión de contexto, quién la emite y valida y cómo caduca. Un contador del cliente no es prueba de autoridad. Documenta protección de solicitudes según el patrón del módulo.
5. Define resultado de lectura/validación con identidad normalizada, correlación y errores funcionales. Si contiene capacidades, explica su vigencia y deja la revalidación en cada escritura. No serialices mensajes SQL, cadenas de conexión o excepciones crudas.

## Integración de selección y compatibilidad

Implementa el punto de integración de selección acordado en PB00, manteniendo compatibles los contratos públicos existentes. La selección de interfaz puede actualizar sesión legacy cuando se necesite, pero el adaptador no debe considerar esa actualización una autorización durable para otra petición.

Para acciones posteriores, expón una entrada interna reutilizable que permita validar y ejecutar sobre el mismo destino en la misma petición. PB02 conectará sus operaciones. No publiques un endpoint genérico para invocar cualquier método por nombre ni cambies masivamente los ASMX existentes.

Define limpiar selección de forma coherente con campos, variables y estado del servidor que realmente consume el módulo. Preserva cancelación/errores del navegador; no introduzcas escrituras de dominio o auditoría documental para una lectura de contexto. Registra las rutas legacy que siguen pendientes de PB02/PB04.

## CRITERIOS DE ACEPTACIÓN

- Identidad inexistente, mezcla de nivel/expediente, relación ajena, referencia fuera de rango y dos alias iguales.
- Usuario/empresa distintos, permiso retirado, nodo oculto cuando corresponde y sesión vencida.
- A/B rápido, solicitud tardía y dos páginas con contextos distintos. Demuestra qué compara el servidor y qué descarta el cliente.
- Limpieza deja las rutas afectadas sin destino anterior utilizable silenciosamente.
- Los consumidores legacy mantienen formato de respuesta o disponen de una adaptación integrada, probada y documentada.

Utiliza pruebas de resolución y autorización con dependencias sustituibles, sin conexiones reales por defecto. Compila el VB.NET y prueba el transporte/contrato integrado; las pruebas estáticas o solo de cliente no acreditan este paquete completo.

## ENTREGABLE FINAL

Entrega código integrado, pruebas y `Doc/Diagnostico/modernizacion-arbol-produccion/implementacion/PB01-contexto.md`. Incluye firmas definitivas, campos, validaciones, errores, semántica de sesión/correlación, consumidores y rutas todavía sin adaptar. PB02 debe poder consumir el contrato sin inventar otra identidad o autorización.

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


## PRUEBAS OBLIGATORIAS

Ejecuta pruebas unitarias o focales que verifiquen comportamiento real del alcance, además de los casos de aceptación de este prompt. Registra cada comando, resultado y limitación; no basta comprobar texto de clases o que existe un archivo. Si cambia VB.NET o el proyecto, aporta compilación MSBuild compatible con .NET Framework 4.6.1 y su configuración real. Si solo cambia presentación, justifica por qué no aplica build de backend y verifica carga de recursos e integración en la página real.

El recorrido E2E o la validación manual reproducible con evidencia forman parte integral del mismo cambio funcional y de su aceptación. No se consideran una entrega posterior que permita anunciar el paquete como validado mientras falta ese recorrido. La validación local con respuestas controladas se identifica como tal.

Reutilizar la infraestructura existente en `tools/e2e`: autenticación, configuración, inventario, validadores y evidencia. No crear login, arnés, proyecto Playwright, configuración ni `.env` paralelos. Antes de autenticar leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`; ejecutar únicamente con ambiente, cuentas y datos descartables expresamente autorizados para el recorrido.

Usar secretos efímeros; no imprimir, guardar ni persistir credenciales, cookies, tokens o cadenas de conexión. Los controles de base serán solo SELECT y la evidencia saneada excluirá datos documentales sensibles. Cubrir autorización, lectura sin mutación de dominio, escrituras autorizadas cuando el alcance las incluya, concurrencia y regresión relacionada. En una lectura o una entrega de presentación, la ausencia de escrituras es un criterio verificable, no una razón para inventarlas.

No habilitar arbitrariamente flags o gates existentes. No introducir activación por usuarios/grupos ni reintroducir `WorkflowCentroTrabajoModernActive`. No cerrar la aceptación integral sin validación autorizada: si falta ambiente, autorización o infraestructura, registrar bloqueo explícito de esa aceptación y lo que sí se verificó, sin usar mocks, simulaciones ni evidencia ficticia para acreditar un recorrido real.

## DOCUMENTACIÓN TÉCNICA

Ubicar y actualizar documentación existente de esta capacidad: exploración, guía, contratos del paquete y documento de resultado indicado en ENTREGABLE FINAL. Documenta funciones creadas o modificadas con ruta, parámetros, retorno, errores y consumidores cuando corresponda. Incluye decisiones aplicadas, evidencia saneada, compatibilidad y pendientes; no dupliques las aprobaciones fuera de la exploración ni des por existentes los resultados de paquetes futuros.
