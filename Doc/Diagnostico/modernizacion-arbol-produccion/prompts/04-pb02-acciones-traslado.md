# Prompt PB02 para ejecutar acciones sobre un destino comprobado

**Estado:** borrador de implementación. Requiere PB00 y PB01 verificados; decisiones de PB01 y D07, D11, D12 aprobadas para las operaciones incluidas.

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

Actúa como desarrollador senior de ASP.NET WebForms, VB.NET y persistencia transaccional. Aplica [el contexto común](contexto-comun.md) y conecta las acciones aprobadas al contrato de identidad/autoridad de PB01. Mantén las reglas existentes y elimina, en las rutas incluidas, la posibilidad de ejecutar silenciosamente sobre una selección de sesión diferente del destino solicitado.

## Áreas y alcance finito

Parte del mapa de consumidores de PB00. Revisa `Gestion/WebFormProducionDocumental.aspx.vb`, `webservice/WebServiceProducion.asmx.vb`, `Gestion/Class_niveles_organizacion.vb`, `Gestion/ClassGaExpediente.vb` y las funciones de cliente que disparan esas operaciones.

Incluye crear/editar nivel y expediente, cortar/pegar, ocultar/restaurar y eliminar cuando estén dentro de la entrega aprobada. Identifica también accesos duplicados desde barra, menú contextual y botones ocultos. Los accesos de archivo físico, rótulos, índice, estados y documentos deben figurar en el mapa: adapta los que comparten el contrato ahora y deja una dependencia explícita de PB04 para los restantes. No declares completa una familia dejando activo un camino que use otro destino.

## Implementación de acciones

1. Para cada operación, valida tipo e identidad y su autoridad vigente mediante el contrato PB01; conserva además las comprobaciones de negocio específicas del handler existente.
2. Invoca el flujo de dominio sobre esa misma identidad dentro de la petición. Si el legacy requiere sesión, limita y documenta su uso en el adaptador; no bases la seguridad en un postback previo de preselección. Prueba solicitudes de dos páginas con destinos distintos.
3. Conserva formas de respuesta de accesos legacy. Un resultado nuevo debe distinguir rechazo, éxito confirmado e incertidumbre; separa un fallo de renderizado posterior de un fracaso de dominio.
4. Mantén la creación de expediente desde un nivel y desde un expediente, con destino de nivel explícito y sin tomar la primera relación por accidente. Crear subnivel sobre expediente continúa sujeto al rechazo actual.
5. Actualiza búsqueda, identidad de nodo, selección y contadores solo a partir de éxitos confirmados. Evita arreglos de DOM que aparenten éxito cuando falló el servidor.

## Traslado

Conserva `Get_cortar_expediente`, `Get_pegar_expediente` y `Traslado_carpeta_nivel` como contratos a adaptar o envolver con compatibilidad según PB00. No conviertas un nivel en elemento trasladable.

- Respeta propiedad de origen y destino, mismo nivel y relación ya existente; no sustituyas propiedad por una capacidad genérica de escritura.
- Comprueba origen, relación fuente y expediente vigentes al ejecutar. La referencia capturada al cortar puede haber quedado obsoleta.
- Mantén la unidad transaccional de alta de relación destino, eliminación de relación fuente y ajustes de contadores que corresponden a las reglas actuales. Verifica cardinalidades y manejo de error/rollback; una relación inexistente no se anuncia como trasladada.
- Devuelve identidad confirmada del expediente y su nueva relación. La UI no debe seguir usando la relación vieja ni regenerar un expediente.
- Ante pérdida de respuesta, ofrece comprobación del estado origen/destino con el alcance autorizado antes de repetir. No crees un registro durable de idempotencia sin una necesidad de persistencia aprobada ni prometas exactamente una ejecución sin demostrarla.

## Ocultar y eliminar

Ocultar nivel conserva su flujo de visibilidad/restauración. Eliminar utiliza la operación de dominio y todas sus restricciones, incluyendo hijos, relaciones o volúmenes cuando corresponda. El nombre del comando y la confirmación deben reflejar ese efecto; «Quitar del árbol» no sustituye una eliminación real.

Una cancelación no llama al servicio. Un rechazo conserva el estado local anterior. Un resultado incierto mantiene la advertencia y habilita verificación compatible; no elimina filas por suposición. Revalida permisos al ejecutar aunque el menú se hubiera abierto antes de una revocación.

## CRITERIOS DE ACEPTACIÓN

- Acción sobre B cuando A estaba seleccionado, desde cada acceso vigente y en dos páginas/pestañas.
- Crear sobre destino correcto; relación ambigua/obsoleta; expediente con nombre repetido.
- Cortar sobre origen no propio, pegar en destino no propio, mismo nivel, relación duplicada y fuente retirada después de cortar.
- Traslado exitoso cambia la relación y conserva el expediente; fallo intermedio no deja un éxito falso ni contadores anunciados incorrectamente.
- Cancelación/rechazo de eliminación y sus restricciones de dominio; ocultar/restaurar separado.
- Respuesta perdida y fallo visual después del éxito no ocasionan reenvío automático.

Utiliza pruebas de comportamiento y de servicio/transacción con recursos de prueba aislados o dependencias sustituibles. No ejecutes mutaciones en la base operativa para verificar el prompt. Compila el backend y registra cualquier verificación que requiera ambiente autorizado.

## ENTREGABLE FINAL

Crea `Doc/Diagnostico/modernizacion-arbol-produccion/implementacion/PB02-acciones.md` con matriz por acción/acceso, reglas preservadas, firmas y respuestas, transacción, pruebas y rutas que consumirá PB04. Entrega el código integrado y la evidencia; deja contratos suficientes para que PF02 renderice resultados confirmados sin otra interpretación de identidad.

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
