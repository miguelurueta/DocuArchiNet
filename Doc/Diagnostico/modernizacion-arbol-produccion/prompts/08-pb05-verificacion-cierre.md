# Prompt PB05 para verificar y preparar el cierre de la modernización

**Estado:** borrador de verificación. Requiere los paquetes incluidos implementados y comprobados, PB03 omitido solo si está justificado, y criterios de aceptación D18 aprobados. No autoriza por sí mismo despliegue ni pruebas reales.

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

Actúa como arquitecto responsable de aceptación técnica. Lee [el contexto común](contexto-comun.md), la exploración vigente y los resultados de PB00, PF01, PB01, PB02, PB03, PF02 y PB04. Verifica la experiencia integrada y sus contratos. Corrige defectos dentro del alcance aprobado; registra cambios de alcance o de reglas como decisiones que necesitan revisión.

## Auditoría de trazabilidad

El flujo de cierre contrasta decisiones con código, comprueba los contratos de todos los accesos, ejecuta las verificaciones aplicables y reúne su evidencia antes de concluir aceptación o pendientes. Un resultado local no reemplaza una comprobación de ambiente ni autoriza publicar la entrega.

1. Relaciona cada decisión aprobada con código y verificación. Distingue lo implementado de lo solo descrito o simulado.
2. Comprueba que los contratos finales coinciden entre backend, JavaScript, página, proyecto y consumidores legacy. Verifica que no cambió una firma por una prueba que solo cubre el nuevo consumidor.
3. Revisa todos los accesos de la matriz PB04. Un menú modernizado no acredita un botón oculto antiguo, una operación de carga o un ASMX que quedó fuera.
4. Identifica cambios compartidos, dependencias nuevas y efectos de dominio. Confirma que no se introdujeron tablas, activaciones por usuario/grupo o cambios de negocio no aprobados.

## Verificación funcional y de autoridad

Cubre al menos los escenarios de la exploración: tipo/estado de cada nodo, nivel vacío, nombres largos/repetidos, identidad compuesta, selección/expansión, búsqueda/filtros/ruta, creación/edición, traslado, ocultos, eliminación, permisos, importación, archivo físico, rótulos/estado/índice y operaciones documentales simples y múltiples.

Exige casos de A/B rápido, clic derecho B con A seleccionado, dos pestañas, limpiar selección, permiso revocado, relación movida/retirada, doble activación, timeout, respuesta inválida y fallo visual después de éxito de dominio. Confirma que el resultado incierto no se convierte en reenvío automático.

Las consultas de contexto/capacidades no deben escribir dominio ni auditoría documental. Valida que las capacidades de presentación no reemplazan la autoridad de ejecución.

## Compilación, navegador y ambiente

- Compila el backend con la configuración real y verifica archivos Compile/Content y empaquetado. Registra las herramientas y límites de compilación.
- Ejecuta las pruebas locales pertinentes y las nuevas pruebas de contrato/comportamiento; no amplíes indefinidamente pruebas sin relación con un cambio o defecto detectado.
- Revisa el DOM WebForms integrado, no solo el de la maqueta: ciclos de UpdatePanel, nodos iniciales/dinámicos, foco y formularios con propietario independiente.
- Valida teclado/táctil, matriz de navegadores, lector de pantalla y zoom acordados en D17. Registra lo probado y lo pendiente.
- Mide volumen/tiempos, consultas y coste de decoración según D16. Establece comparación con la línea base; no uses el pequeño ejemplo como prueba de capacidad.
- Ejecuta pruebas autenticadas o de carga únicamente con ambiente, cuentas y alcance expresamente autorizados conforme al runbook. Si falta esa autorización, entrega el protocolo preparado y marca pendiente la aceptación que depende de él.

## Preparación de publicación y reversión

Describe los archivos/DLL/recursos que deben publicarse juntos, las combinaciones compatibles y el orden de despliegue cuando se haya aprobado. La caché de estáticos no debe mezclar contratos de cliente/servidor incompatibles.

Prepara comprobaciones previas y posteriores y una reversión compatible. No reconstruyas relaciones, borres documentos o reviertas una escritura de negocio para retirar una mejora visual. Si un cambio no admite revertir solo estáticos, declara la dependencia de DLL y no prometas esa reversión parcial.

No publiques ni cambies configuración de ambiente por ejecutar este prompt salvo autorización de despliegue vigente. Si la implementación adoptó un cambio OpenSpec propio, verifica sus artefactos y tareas; no archives automáticamente ni uses DOC-127 como contenedor de esta mejora.

## Entrega de aceptación

Crea `Doc/Diagnostico/modernizacion-arbol-produccion/implementacion/PB05-verificacion-cierre.md` con:

1. Decisiones/versiones y alcance realmente entregado.
2. Matriz de operaciones con pruebas/evidencia y resultado por acceso.
3. Compilación, verificación local, navegador y ambiente claramente diferenciados.
4. Defectos corregidos, pendientes y efecto sobre aceptación/despliegue.
5. Métricas frente a la línea base y límites de lo demostrado.
6. Plan concreto de publicación/reversión y autorizaciones pendientes.
7. Estado de Git y sincronización remota de los cambios propios.

Concluye si la entrega cumple sus criterios aprobados o qué verificaciones faltan. «No encontré fallos en las pruebas ejecutadas» no equivale a garantizar ausencia total de regresiones.

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

## CRITERIOS DE ACEPTACIÓN

Deben quedar verificadas las condiciones funcionales, de contrato, autoridad y compatibilidad incluidas en el paquete. Cada condición requiere evidencia identificable o un pendiente explícito; no se declara aceptación completa si falta una comprobación necesaria.

## PRUEBAS OBLIGATORIAS

Ejecuta pruebas unitarias o focales que verifiquen comportamiento real del alcance, además de los casos de aceptación de este prompt. Registra cada comando, resultado y limitación; no basta comprobar texto de clases o que existe un archivo. Si cambia VB.NET o el proyecto, aporta compilación MSBuild compatible con .NET Framework 4.6.1 y su configuración real. Si solo cambia presentación, justifica por qué no aplica build de backend y verifica carga de recursos e integración en la página real.

El recorrido E2E o la validación manual reproducible con evidencia forman parte integral del mismo cambio funcional y de su aceptación. No se consideran una entrega posterior que permita anunciar el paquete como validado mientras falta ese recorrido. La validación local con respuestas controladas se identifica como tal.

Reutilizar la infraestructura existente en `tools/e2e`: autenticación, configuración, inventario, validadores y evidencia. No crear login, arnés, proyecto Playwright, configuración ni `.env` paralelos. Antes de autenticar leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`; ejecutar únicamente con ambiente, cuentas y datos descartables expresamente autorizados para el recorrido.

Usar secretos efímeros; no imprimir, guardar ni persistir credenciales, cookies, tokens o cadenas de conexión. Los controles de base serán solo SELECT y la evidencia saneada excluirá datos documentales sensibles. Cubrir autorización, lectura sin mutación de dominio, escrituras autorizadas cuando el alcance las incluya, concurrencia y regresión relacionada. En una lectura o una entrega de presentación, la ausencia de escrituras es un criterio verificable, no una razón para inventarlas.

No habilitar arbitrariamente flags o gates existentes. No introducir activación por usuarios/grupos ni reintroducir `WorkflowCentroTrabajoModernActive`. No cerrar la aceptación integral sin validación autorizada: si falta ambiente, autorización o infraestructura, registrar bloqueo explícito de esa aceptación y lo que sí se verificó, sin usar mocks, simulaciones ni evidencia ficticia para acreditar un recorrido real.

## DOCUMENTACIÓN TÉCNICA

Ubicar y actualizar documentación existente de esta capacidad: exploración, guía, contratos del paquete y documento de resultado indicado en ENTREGABLE FINAL. Documenta funciones creadas o modificadas con ruta, parámetros, retorno, errores y consumidores cuando corresponda. Incluye decisiones aplicadas, evidencia saneada, compatibilidad y pendientes; no dupliques las aprobaciones fuera de la exploración ni des por existentes los resultados de paquetes futuros.
