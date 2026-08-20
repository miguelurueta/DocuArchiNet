## Context

La presentación de `workflow/Webworkflow.aspx` se entrega de forma constante. La política central que antes evaluaba un gate de piloto se convierte en una política oficial: todo contexto Workflow válido recibe bootstraps y contratos ASMX modernos. El host `Defaul/WebFormInicioDocuarchiGestion.aspx` conserva su viewport pasivo independiente.

Véanse `proposal.md` y la delta de `infraestructura-visual-aislada-reversible` para el alcance funcional. El gate operativo debe conservar su configuración segura: inactivo, sin usuarios ni grupos al finalizar cualquier verificación no autorizada.

## Goals / Non-Goals

**Goals:**

- Suprimir la dependencia de configuración histórica sin retirar el viewport responsive del host.
- Mantener una presentación y operaciones modernas oficiales para todos los contextos Workflow válidos.
- Demostrar por pruebas estáticas y compilación que no queda una restricción de despliegue por perfil, grupo o configuración de gate.

**Non-Goals:**

- Cambiar la configuración local de gate, usuarios o grupos durante el desarrollo.
- Cambiar endpoints ASMX, permisos, sesión, handlers, postbacks o flujos legacy de negocio.
- Ejecutar E2E autenticada, pruebas de carga o QA visual en un ambiente sin autorización explícita.

## Decisions

### D-01 — Viewport permanente y pasivo en el host superior

`workflowCentroTrabajoModernShellViewport` permanecerá como `HtmlMeta` estático y visible en la página inicial. Se eliminarán la propiedad, las constantes, los lectores de configuración y el `PreRender` que hoy deciden su visibilidad.

El viewport solo describe el ancho CSS del navegador: no carga CSS/JavaScript DOC-2, no agrega la clase raíz moderna y no autoriza una operación. De este modo se conserva la adaptación móvil sin conservar un segundo gate.

Se descarta evaluar `WorkflowModernPresentationBootstrap` en el host. Ese bootstrap completa el contexto Workflow y puede limpiar datos de sesión cuando son inconsistentes; usarlo para un `meta` convertiría una decisión pasiva de presentación en una operación con efectos sobre sesión.

### D-02 — Política oficial central sin alcance de despliegue

Se eliminarán `WorkflowCentroTrabajoModernEnabled` y `WorkflowCentroTrabajoModernPilotProfiles` de `Web.config` junto con cualquier referencia de producción. No se mantendrán aliases ni valores ignorados: una configuración externa que los escriba no podrá activar ni desactivar ninguna experiencia.

La implementación central de `IWorkflowModernFeatureGate` conservará la comprobación de que el contexto Workflow sea válido, pero devolverá la habilitación oficial para todos esos contextos. No leerá `WorkflowCentroTrabajoModernActive`, modo oficial, usuarios, grupos, exclusiones, metadatos de piloto ni rollback. Las claves locales se conservan sin efecto para respetar la regla operativa del repositorio de dejarlas inactivas y vacías.

Se descarta activar el gate local o usar listas globales de usuarios, porque ambas opciones mantienen una segunda vía de despliegue y dejan usuarios con una experiencia distinta.

### D-03 — Regresión focal de política oficial

Las pruebas focales verificarán que el host declara un único viewport visible y ya no contiene las claves, el perfil de sesión ni lectores de configuración históricos. También comprobarán que `Webworkflow.aspx` entrega sus recursos, bootstraps y controles habilitados para contextos válidos, que el evaluador central no lee configuración de gate, y que continúan los bloqueos de sesión, permisos, requisitos y concurrencia de los contratos ASMX.

Las aserciones de configuración no asumirán un estado de despliegue activo: se limitarán a la ausencia de las claves retiradas y a la existencia del gate único. Así respetan el estado seguro requerido en el repositorio.

### D-04 — Despliegue atómico y reversa por versión

El marcado del host, su code-behind y la retirada de claves se despliegan en la misma versión. Si aparece una regresión visual del host, la reversa es restaurar la versión anterior del paquete; no se reintroduce una segunda activación en caliente.

La reversa operativa de Workflow es restaurar la versión anterior del paquete. Las claves de gate ya no cambian la disponibilidad de los contratos o diálogos modernos.

### D-05 — Controles modernos únicos, sin respaldo legacy visible

`Continuar flujo` y `Enviar a grupo` serán controles modernos únicos, de tipo botón para impedir la navegación o el postback implícito. Los inicializadores ASMX existentes se enlazan para todo contexto Workflow válido. Los controles no conservan `onclick`, `ImageButtonterminar`, `ImageButtonEnviaActividad` ni un enlace a un diálogo legacy.

Se conserva el bloqueo funcional de los contratos cuando sesión, permisos, requisitos, destino o concurrencia no son válidos. Se elimina exclusivamente el bloqueo de despliegue, porque no representa autorización de negocio.

### D-06 — Invalidación explícita de activos visuales

El adaptador `centro-trabajo-visual.js` clasifica los controles por selectores y agrega las clases que determinan su ubicación y color. Como es un archivo estático con URL versionada, cada ajuste a esa clasificación se acompaña de una nueva versión en el markup; se incrementa simultáneamente la URL del CSS `workflow-centro-trabajo-moderno.css` cuando el cambio toca su estado visual.

Se descarta depender de recarga forzada del usuario o de TTL de proxy: ambos dejan abiertas sesiones con el adaptador anterior y causan una diferencia de posición entre acciones que usan selector por `onclick` y por `id`.

## Risks / Trade-offs

- [El viewport pasa a todos los perfiles del host] → Validar el host a 1366, 1024, 768 y 375 px antes de liberar; no se modifican CSS ni handlers en este cambio.
- [Una automatización externa conserva las claves retiradas] → Documentar el cambio incompatible y buscar referencias de configuración antes de desplegar; valores desconocidos no cambian el comportamiento de la aplicación.
- [Se elimina una salvaguarda de despliegue] → Conservar y probar los controles de sesión, permisos, requisitos, concurrencia y auditoría en los servicios.
- [Una clave local aparentemente apaga la experiencia] → Pruebas explícitas de que la política oficial no lee configuración de gate y documentación de que la reversión es por versión.
- [La configuración local conserva el gate operativo apagado] → Mantenerla así; la política oficial no la consulta.

## Migration Plan

1. Actualizar el marcado del host para emitir el viewport de forma estática y eliminar el código histórico de su code-behind.
2. Eliminar las dos claves históricas de la configuración y actualizar la especificación/documentación de alcance.
3. Incorporar pruebas focales, ejecutar la suite afectada y compilar el proyecto disponible.
4. Comprobar en un ambiente autorizado que la página inicial y Workflow conservan legibilidad y navegación en los viewports definidos, que las dos acciones modernas están habilitadas para contextos válidos y que los contratos conservan sus bloqueos de negocio.
5. Si la verificación visual o funcional encuentra una regresión, volver a la versión anterior del paquete; no se usa un gate para revertir la experiencia.

## Verification Evidence

- `node --test tests/workflow-modern-feature-gate.test.cjs tests/workflow-group-send.test.cjs tests/workflow-transition-ui.test.cjs tests/workflow-transition-confirmation-integration.test.cjs` finalizó con código `0`: 37 pruebas correctas.
- `msbuild.exe .\GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /verbosity:minimal` finalizó con código `0`. Conserva advertencias históricas de conflictos de ensamblados y análisis VB, sin errores de compilación.
- La búsqueda de referencias de producción a las dos claves históricas no devolvió resultados y `git diff --check` no informó errores de espacios.
- La verificación final confirmó `WorkflowCentroTrabajoModernActive=false`, `WorkflowCentroTrabajoModernUsers=` y `WorkflowCentroTrabajoModernGroups=`. No se ejecutaron E2E autenticadas, carga ni se activó el gate.
- La extensión de presentación constante se verificó con `node --test tests/workflow-modern-feature-gate.test.cjs tests/workflow-group-send.test.cjs tests/workflow-transition-ui.test.cjs tests/workflow-transition-confirmation-integration.test.cjs`: 38 pruebas correctas, 0 fallos.
- `msbuild.exe .\GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /verbosity:minimal` finalizó con código `0`; conserva advertencias históricas de ensamblados y análisis VB, sin errores.
- `git diff --check` no informó errores de espacios. `openspec validate consolidar-gate-centro-trabajo-moderno --strict` resultó válido.
- La comprobación de configuración final confirmó `WorkflowCentroTrabajoModernActive=false`, `WorkflowCentroTrabajoModernOfficialMode=false`, `WorkflowCentroTrabajoModernUsers=` y `WorkflowCentroTrabajoModernGroups=`. No se ejecutaron E2E autenticadas, carga ni se activó el gate.
- El retiro final de accesos legacy se verificó con la misma suite focal: 38 pruebas correctas, 0 fallos. Las aserciones comprueban dos `<button>` modernos con `WorkflowCentroTrabajoModernOperationDisabledAttribute`, ausencia de `onclick`, `ImageButtonterminar` e `ImageButtonEnviaActividad` en los disparadores, y ausencia de enlace de operación cuando el bootstrap está inactivo.
- La compilación Debug volvió a finalizar con código `0`; `git diff --check` no informó errores de espacios. La configuración se comprobó sin modificarla y permanece fail-closed: gate y modo oficial en `false`, usuarios y grupos vacíos.
- La corrección de activos visuales entrega `workflow-centro-trabajo-moderno.css?v=20260820-modern-actions3` y `centro-trabajo-visual.js?v=20260820-modern-actions3`. La suite focal volvió a finalizar con 38 pruebas correctas; `git diff --check` no informó errores de espacios. No se cambió el gate ni se ejecutó E2E.
- La política oficial global se verificó con la suite focal: 38 pruebas correctas, 0 fallos. La regresión comprueba que el evaluador no lee configuración, alcance de usuario/grupo, piloto, exclusiones ni rollback, y que los dos controles no tienen ruta legacy visible.
- `msbuild.exe .\GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /verbosity:minimal` finalizó con código `0`. Persiste el conjunto de advertencias históricas de ensamblados y análisis VB, sin errores.
- `tools/validation/Verify-Doc14PilotGate.ps1` se ejecutó de forma aislada contra la DLL actualizada: un contexto válido devolvió `activo/WORKFLOW_MODERN_OFFICIAL` y uno inválido `inactivo/WORKFLOW_CONTEXT_INVALID`. No conectó con ningún ambiente.
- La comprobación de `Web.config` confirmó `WorkflowCentroTrabajoModernActive=false`, `WorkflowCentroTrabajoModernOfficialMode=false`, usuarios y grupos vacíos. No se ejecutaron E2E autenticadas, carga ni se modificaron gates.
