# Prompt PB04 para cerrar la compatibilidad de los accesos existentes

**Estado:** borrador de implementación e integración. Requiere PB00, PB01, PB02 y PF02; PB03 implementado u omitido justificadamente. Requiere D13 y las decisiones aplicables a las rutas integradas.

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

Actúa como arquitecto de integración ASP.NET WebForms/JavaScript. Aplica [el contexto común](contexto-comun.md) y completa los consumidores que permiten que la modernización conserve las operaciones reales del gestor. Este paquete debe resolver las dependencias identificadas, no limitarse a una inspección visual de la maqueta.

## Importación de carpetas

Revisa `Doc119NivelPulsado`, `Doc119InvocarImportacion`, `Doc111ActualizarRama`, `Doc120MostrarExpediente` y `create_trenode_treview`, junto con los consumidores de `ImportacionCarpetas`. Conserva captura de nivel explícito, autoridad, contexto y resultado de lote.

1. Un lote iniciado conserva su destino aunque cambie la selección del árbol. No reinterpretes una recuperación con la selección actual de la página.
2. Inserta y decora solo relaciones confirmadas y conserva la selección/expansión de otras ramas. Atiende el primer hijo de un nivel vacío, la etiqueta modificada y la relación ya existente sin duplicar nodos.
3. Mantén búsqueda por identidad compuesta y rechazo de objetivos ambiguos o contenedores ocultos ajenos a la contracción WebForms.
4. Si el lote se confirmó pero falló reflejar su resultado, ofrece recuperación de la vista sin reimportar. No anuncies un error de dominio por un fallo de inserción visual.

## Excel, perfiles y formularios

Conserva la herramienta de Excel y los accesos de perfiles, fuentes y proveedores con sus autorizaciones. La mejora del explorador no cambia su identidad de lote, recuperación o destino interno.

Verifica el propietario independiente de los controles de Excel y de cualquier control nuevo. No actives validación global de `form1` por un diálogo oculto o incompleto. Comprueba inicialización repetida, cierre/reapertura y ausencia de formularios anidados incorrectos o propietarios duplicados.

## Documentos y operaciones del expediente

Completa la matriz de PB00 para archivo físico, rótulos, estado, índice, carga individual, digitalización, visor, descargas, renombrado/eliminación de archivos, copiar/pegar, metadatos, radicación, firma, correo, exportación e índices por lote.

Para cada acceso:

- Confirma handler, destino, permiso, formato y efecto real.
- Adapta el contexto donde haga falta usando PB01/PB02; no dejes un botón oculto o menú antiguo que ejecute silenciosamente con otro destino.
- Conserva la selección de filas y el portapapeles documental, separados de la selección del árbol.
- Distingue editar el alias/nodo y abrir el formulario completo del expediente; no fusiones sus formularios por parecer equivalentes.
- Verifica consumidores fuera de Producción antes de cambiar utilidades compartidas, visores o cargadores. Limita la corrección al adaptador del módulo cuando sea suficiente.

## CRITERIOS DE ACEPTACIÓN

Ejecuta las suites locales relacionadas después de revisar que no requieren ambiente. Como base:

```text
node --test tests/doc119-context-menu.test.cjs tests/doc120-importacion-arbol.test.cjs
node --test tests/doc126-importacion-excel-ui.test.cjs
```

Selecciona además las pruebas `doc88-produccion-*` pertinentes si el cambio alcanza preparación, almacenamiento o proyección del cargador. No amplíes pruebas a servicios reales por el simple hecho de compartir un nombre de función.

Verifica integración con los handlers reales y transporte controlado: importar con otra selección activa, lote histórico, primer hijo, alias iguales, actualización parcial repetida, cancelación/rechazo y formularios ocultos inválidos. Para las operaciones que requieren IIS/DB/dispositivo/visor real, prepara casos con ambiente/cuentas y evidencia esperada y ejecútalos solo con la autorización correspondiente.

## ENTREGABLE FINAL

Crea `Doc/Diagnostico/modernizacion-arbol-produccion/implementacion/PB04-compatibilidad.md` con una fila por operación/acceso: validado, ajustado, diferido o pendiente de ambiente. Incluye pruebas y consumidores compartidos afectados. Ningún pendiente se presenta como «sin regresiones». Entrega las adaptaciones necesarias y deja a PB05 una matriz verificable, no una lista de afirmaciones generales.

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

## Comportamiento esperado del flujo

La secuencia es resolver identidad, confirmar destino y autoridad, invocar el flujo acordado y reflejar exclusivamente el resultado del contexto correcto. Un error de visualización posterior no deshace un éxito de dominio ni habilita repetir una escritura.

## PRUEBAS OBLIGATORIAS

Ejecuta pruebas unitarias o focales que verifiquen comportamiento real del alcance, además de los casos de aceptación de este prompt. Registra cada comando, resultado y limitación; no basta comprobar texto de clases o que existe un archivo. Si cambia VB.NET o el proyecto, aporta compilación MSBuild compatible con .NET Framework 4.6.1 y su configuración real. Si solo cambia presentación, justifica por qué no aplica build de backend y verifica carga de recursos e integración en la página real.

El recorrido E2E o la validación manual reproducible con evidencia forman parte integral del mismo cambio funcional y de su aceptación. No se consideran una entrega posterior que permita anunciar el paquete como validado mientras falta ese recorrido. La validación local con respuestas controladas se identifica como tal.

Reutilizar la infraestructura existente en `tools/e2e`: autenticación, configuración, inventario, validadores y evidencia. No crear login, arnés, proyecto Playwright, configuración ni `.env` paralelos. Antes de autenticar leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`; ejecutar únicamente con ambiente, cuentas y datos descartables expresamente autorizados para el recorrido.

Usar secretos efímeros; no imprimir, guardar ni persistir credenciales, cookies, tokens o cadenas de conexión. Los controles de base serán solo SELECT y la evidencia saneada excluirá datos documentales sensibles. Cubrir autorización, lectura sin mutación de dominio, escrituras autorizadas cuando el alcance las incluya, concurrencia y regresión relacionada. En una lectura o una entrega de presentación, la ausencia de escrituras es un criterio verificable, no una razón para inventarlas.

No habilitar arbitrariamente flags o gates existentes. No introducir activación por usuarios/grupos ni reintroducir `WorkflowCentroTrabajoModernActive`. No cerrar la aceptación integral sin validación autorizada: si falta ambiente, autorización o infraestructura, registrar bloqueo explícito de esa aceptación y lo que sí se verificó, sin usar mocks, simulaciones ni evidencia ficticia para acreditar un recorrido real.

## DOCUMENTACIÓN TÉCNICA

Ubicar y actualizar documentación existente de esta capacidad: exploración, guía, contratos del paquete y documento de resultado indicado en ENTREGABLE FINAL. Documenta funciones creadas o modificadas con ruta, parámetros, retorno, errores y consumidores cuando corresponda. Incluye decisiones aplicadas, evidencia saneada, compatibilidad y pendientes; no dupliques las aprobaciones fuera de la exploración ni des por existentes los resultados de paquetes futuros.
