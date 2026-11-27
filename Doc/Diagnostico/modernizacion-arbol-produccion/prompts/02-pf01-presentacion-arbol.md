# Prompt PF01 para modernizar la presentación del TreeView

**Estado:** borrador de implementación. Requiere PB00 y las decisiones D01, D02, D03, D04, D17 y el alcance de D18 aprobadas para las alternativas descritas.

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

Actúa como desarrollador senior de JavaScript y ASP.NET WebForms. Aplica [el contexto común](contexto-comun.md) e integra la distinción visual de niveles y expedientes sobre el árbol existente, sin sustituir su DOM operativo ni habilitar acciones de backend nuevas.

Si D01 elige reemplazar el control o D04 conserva exclusivamente la activación sobre texto, revisa el alcance del prompt antes de editar. No fuerces las opciones de la maqueta sobre decisiones distintas.

## Áreas previstas

Revisa `Gestion/WebFormProducionDocumental.aspx`, `js/gestion/WebFormProducionDocumental.js`, `Styles/Aplicaction.css`, los estilos de Producción realmente utilizados y `GestionDocumental-Docuarchi.net.vbproj`. Confirma los nombres y consumidores antes de modificar. Crea recursos específicos del explorador cuando corresponda y limita sus selectores a ese ámbito.

## Implementación requerida

El flujo de presentación comienza con el nodo canónico del árbol, resuelve su tipo, agrega decoración fuera del nombre y conserva el enlace original para la activación. Después de una mutación vuelve a decorar solo los nodos afectados, manteniendo selección, foco y cantidad de solicitudes.

1. Resuelve un nodo canónico por cada enlace de texto del árbol. Identifica tipo desde el contrato de construcción y sus referencias válidas; no por imagen, texto, profundidad ni presencia de hijos.
2. Añade icono y etiqueta «Nivel»/«Expediente» según D02. Mantén compartición separada del tipo. El texto canónico del enlace permanece intacto para `.text`, `innerText`, búsqueda y renombrado.
3. Implementa una decoración idempotente para nodos iniciales y dinámicos. Atiende inserción, cambio de nombre, movimiento, restauración e importación. Si se usa observación de mutaciones, acótala al árbol, procesa cambios pertinentes y evita observar/redecorar indefinidamente los cambios propios.
4. Preserva tablas, IDs, ramas `nXNodes`, enlaces `tN`/`tNi` y el contrato de las funciones de búsqueda, inserción y eliminación. Los auxiliares no deben ser nuevos enlaces que el índice interprete como nodos.
5. Si D04 aprueba selección por fila, normaliza el objetivo al enlace canónico y ejecuta una única activación compatible. Icono, etiqueta y texto deben seleccionar lo mismo. El expansor mantiene su función independiente y no dispara otra consulta. No dupliques el manejador anterior.
6. Diferencia foco de selección y habilita el comportamiento de teclado aprobado. No suprimas los controles existentes hasta que su acceso equivalente esté integrado. Conserva nombres largos y acceso al texto completo.
7. Ajusta tamaño, contraste, separación y respuesta a viewport/zoom sin invalidar `auto_zise`. Agrega controles auxiliares `type="button"` y evita que formularios ajenos validen los envíos del gestor.

## Límites del paquete

No cambies reglas de permisos, consultas documentales, sesión, eliminación, traslado o importación para satisfacer una etiqueta visual. No muestres «Solo lectura» por el estado de compartición. No presentes esta entrega como solución a la coordinación entre pestañas; esa responsabilidad corresponde a PB01/PB02.

## CRITERIOS DE ACEPTACIÓN

- Nivel vacío, propio, compartido y recibido; expediente con código largo; dos alias iguales.
- Construcción inicial y primer hijo dinámico; renombrado sin etiquetas agregadas al nombre; movimiento/restauración/importación conservan el estilo.
- Repetir inicialización y actualizaciones parciales no duplica elementos, listeners ni envíos.
- Clic/teclado no producen dos solicitudes; expandir no cambia la selección involuntariamente.
- Búsqueda y `Doc120MostrarExpediente` conservan identidad y expansión.
- Revisar anchos representativos y zoom real, foco y retorno del foco según D17. No acreditar lector de pantalla solo por añadir roles.

Ejecuta las pruebas locales pertinentes de PB00 y añade pruebas de comportamiento para las modificaciones del decorador y la normalización de clics. El navegador debe comprobar el nombre canónico y el número de activaciones, no solo la presencia de una clase CSS.

## ENTREGABLE FINAL

Entrega recursos integrados en la página/proyecto, pruebas, instrucciones para retirar solo esta capa visual y `Doc/Diagnostico/modernizacion-arbol-produccion/implementacion/PF01-presentacion.md`. Describe qué permanece sujeto a los contratos de selección actuales y qué necesita PB01/PF02. No entregues únicamente otra maqueta aislada.

## PRUEBAS OBLIGATORIAS

Ejecuta pruebas unitarias o focales que verifiquen comportamiento real del alcance, además de los casos de aceptación de este prompt. Registra cada comando, resultado y limitación; no basta comprobar texto de clases o que existe un archivo. Si cambia VB.NET o el proyecto, aporta compilación MSBuild compatible con .NET Framework 4.6.1 y su configuración real. Si solo cambia presentación, justifica por qué no aplica build de backend y verifica carga de recursos e integración en la página real.

El recorrido E2E o la validación manual reproducible con evidencia forman parte integral del mismo cambio funcional y de su aceptación. No se consideran una entrega posterior que permita anunciar el paquete como validado mientras falta ese recorrido. La validación local con respuestas controladas se identifica como tal.

Reutilizar la infraestructura existente en `tools/e2e`: autenticación, configuración, inventario, validadores y evidencia. No crear login, arnés, proyecto Playwright, configuración ni `.env` paralelos. Antes de autenticar leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`; ejecutar únicamente con ambiente, cuentas y datos descartables expresamente autorizados para el recorrido.

Usar secretos efímeros; no imprimir, guardar ni persistir credenciales, cookies, tokens o cadenas de conexión. Los controles de base serán solo SELECT y la evidencia saneada excluirá datos documentales sensibles. Cubrir autorización, lectura sin mutación de dominio, escrituras autorizadas cuando el alcance las incluya, concurrencia y regresión relacionada. En una lectura o una entrega de presentación, la ausencia de escrituras es un criterio verificable, no una razón para inventarlas.

No habilitar arbitrariamente flags o gates existentes. No introducir activación por usuarios/grupos ni reintroducir `WorkflowCentroTrabajoModernActive`. No cerrar la aceptación integral sin validación autorizada: si falta ambiente, autorización o infraestructura, registrar bloqueo explícito de esa aceptación y lo que sí se verificó, sin usar mocks, simulaciones ni evidencia ficticia para acreditar un recorrido real.

## DOCUMENTACIÓN TÉCNICA

Ubicar y actualizar documentación existente de esta capacidad: exploración, guía, contratos del paquete y documento de resultado indicado en ENTREGABLE FINAL. Documenta funciones creadas o modificadas con ruta, parámetros, retorno, errores y consumidores cuando corresponda. Incluye decisiones aplicadas, evidencia saneada, compatibilidad y pendientes; no dupliques las aprobaciones fuera de la exploración ni des por existentes los resultados de paquetes futuros.
