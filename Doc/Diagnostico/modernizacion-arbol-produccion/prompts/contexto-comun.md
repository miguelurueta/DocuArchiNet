# Contexto común para implementar la modernización del árbol

Este archivo forma parte de todos los prompts de la serie. Actúa como arquitecto y desarrollador senior en ASP.NET WebForms, VB.NET sobre .NET Framework 4.6.1, JavaScript/jQuery y persistencia MySQL, siguiendo las convenciones del repositorio vigente.

## Lecturas y alcance autorizado

Lee completos los `AGENTS.md` aplicables, [la exploración](../exploracion.md), [la guía](00-guia-ejecucion.md), el prompt elegido y los resultados de sus dependencias. Consulta [el diagnóstico](../diagnostico.html) y [la maqueta](../maqueta.html) para entender la experiencia y los consumidores actuales.

Las decisiones D01 a D18 se toman de la exploración vigente y de instrucciones explícitas posteriores del usuario. Al preparar esta serie estaban pendientes. Comienza cada paquete con refinamiento de sus decisiones, sin exigir aprobación global de las 18. Antes de modificar aplicación, registra las decisiones de esa parte y qué revisión/opción se ejecutará. Si falta una decisión necesaria, identifica el punto concreto y continúa el análisis independiente. No declares aprobación por haber recibido el archivo del prompt ni por estar presente la maqueta.

El refinamiento contrasta código y dependencias, justifica detalles técnicos dentro del alcance autorizado y presenta al usuario las alternativas que cambian comportamiento, permisos, semántica o alcance. Registra las respuestas explícitas y después implementa/verifica la parte acordada. Las decisiones permanecen centralizadas en la exploración y no se marcan aprobadas por defecto.

No vuelvas a pedir una aprobación que ya consta en la sesión o la exploración. Las autorizaciones de diseño, implementación, pruebas autenticadas y despliegue tienen alcances distintos; utiliza la que corresponde a la acción. La solicitud inicial de esta serie autoriza redactar prompts, no ejecutarlos.

## Trabajo en la carpeta principal

1. Revisa rama, HEAD, cambios de trabajo y staging antes de editar. Conserva el trabajo concurrente y limita el commit a los archivos de la entrega.
2. La carpeta principal es la fuente de verdad. Si se utiliza un worktree autorizado, integra y verifica allí la entrega principal sin descartar avances locales.
3. Relee las funciones por nombre; las líneas de los documentos son orientativas. Comprueba consumidores antes de cambiar una firma o un evento.
4. Respeta la arquitectura existente. Si se añaden archivos compilados o Content, revisa las inclusiones del `.vbproj` y el mecanismo de publicación; no asumas inclusión automática.
5. Registra al terminar los cambios propios en Git y comprueba/comunica su estado frente al remoto. No integres ni envíes cambios ajenos sin autorización.

## Contratos de compatibilidad

- Conserva `TreeViewArchivo`, los enlaces canónicos `TreeViewArchivotN`, los enlaces de imagen `tNi` cuando existen y los contenedores `TreeViewArchivonNNodes`, salvo una adaptación completa expresamente aprobada.
- Un nivel vacío sigue siendo nivel. Un expediente se identifica por relación + nivel + expediente. Alias, profundidad e imagen no son identidad ni permiso.
- Preserva los nombres canónicos consumidos por `.text`, `innerText`, `ITEMS_DATOS` y la búsqueda. La decoración no debe añadirse al alias ni convertirse en otro nodo indexado.
- Soporta construcción del servidor, `create_trenode_treview`, cambio de nombre, movimiento, ocultar/restaurar e inserción desde importación, incluyendo el primer hijo del nivel vacío.
- Un elemento apuntado en el menú, una selección solicitada y una selección confirmada son estados diferentes hasta resolverlos. Ningún comando puede usar silenciosamente otro destino.
- Conserva los contratos observables de los accesos legacy que continúan activos. Si un adaptador expone una respuesta nueva, transforma resultados sin romper consumidores que esperan cadenas, JSON serializado o respuestas ASMX existentes.
- Las operaciones de dominio conservan su semántica: eliminar expediente puede eliminar su registro; mover cambia la relación; ocultar no equivale a eliminar. La presentación no redefine estos efectos.
- El estado compartido no implica solo lectura. Cortar/pegar tiene reglas actuales de propiedad de origen y destino que deben respetarse hasta una decisión explícita de negocio.

## Autoridad y ejecución de operaciones

Obtén usuario y empresa del mecanismo autenticado vigente. Los IDs de la petición son datos a validar, no autoridad. Valida existencia, pertenencia, relación, tipo y permiso de la acción. Respeta la protección de solicitudes del módulo y documenta cómo se integra el nuevo camino, sin imprimir tokens, cookies ni credenciales.

Cuando se apruebe destino explícito, la comprobación y la operación deben usar ese mismo objetivo dentro de la petición. Un preseleccionado en sesión seguido de otra llamada sin objetivo no acredita aislamiento entre pestañas. Evita cambiar variables globales de sesión para simular varios destinos sin un contrato que lo controle.

Para escrituras, distingue rechazo antes de ejecutar, éxito confirmado, resultado incierto y fallo visual posterior. No reintentes automáticamente una mutación ante pérdida de respuesta. Si el flujo no dispone de reconciliación durable, decláralo y proporciona verificación compatible antes de ofrecer repetición; no inventes una garantía de idempotencia.

No modifiques la autorización de todas las operaciones reutilizando como regla universal el adaptador específico de importación.

## Persistencia y rendimiento

La serie no presupone tablas nuevas. Utiliza recursos existentes y consultas parametrizadas en rutas nuevas. No cambies el esquema o ejecutes migraciones como efecto de cargar la página. Si aparece una necesidad de persistencia no cubierta por decisiones aprobadas, documenta el requisito y revisa su alcance antes de implementarla.

No atribuyas al ambiente una versión de MySQL, una unicidad de relaciones o un volumen de nodos por extrapolación. Comprueba el soporte de las consultas con la configuración/documentación disponible; las lecturas reales del ambiente siguen las autorizaciones aplicables.

Evita consultas por nodo para decorar o mostrar indicadores. Distingue hijos directos, nodos cargados, coincidencias y totales globales. Cambiar carga diferida o virtualización requiere un contrato adicional y queda fuera de la alternativa inicial.

## Verificación y entrega

Ejecuta verificaciones locales pertinentes al cambio y añade pruebas de comportamiento cuando protejan contratos reales; no produzcas pruebas que solo repliquen la implementación. No edites las expectativas de pruebas existentes para aceptar una pérdida de funcionalidad.

Antes de pruebas autenticadas lee `tools/e2e/AGENT-RUNBOOK.md`. E2E real, cuentas, ambiente y carga necesitan autorización explícita. Los datos de ejecución y secretos quedan fuera de Git; los SELECT de control no deben modificar tarea, estado ni auditoría. Un preview/consulta nueva debe ser lectura de dominio y no generar una mutación documental.

Compila los cambios VB.NET con el proyecto/configuración y herramientas disponibles. Si la compilación o las pruebas requieren recursos no disponibles, indica el límite real y no declares integración acreditada por pasar pruebas de JavaScript.

En el documento de resultado del paquete registra decisiones aplicadas, archivos, contratos, pruebas, errores pendientes, riesgos y siguiente dependencia. Para la reversión, describe cómo retirar la entrega de presentación o adaptar firmas de forma compatible; no reviertas datos de dominio automáticamente ni uses `git reset` destructivo para limpiar cambios ajenos.
