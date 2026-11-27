# Prompt PF02 para integrar navegación y acciones contextuales

**Estado:** borrador de implementación. Requiere PF01, PB01, PB02 y PB03 si suministra datos necesarios. Decisiones D01 a D10 y D14 a D18 aplicables aprobadas; D11/D12 para acciones respectivas y D13 para accesos de importación.

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

Actúa como desarrollador senior de JavaScript y ASP.NET WebForms. Aplica [el contexto común](contexto-comun.md) e integra la experiencia aprobada en la página real: acciones sobre el elemento indicado, ruta, contenido según tipo, búsqueda y recuperación de errores. Consume los contratos backend ya implementados; no reproduzcas con datos sintéticos la maqueta.

## Áreas previstas

Revisa `Gestion/WebFormProducionDocumental.aspx`, `js/gestion/WebFormProducionDocumental.js`, los recursos nuevos de PF01, `js/jquery.contextMenu.js`, `js/gestion/ProduccionDocumentalBarra.js` y los callbacks de UpdatePanel realmente utilizados. Un cambio en un plugin compartido requiere verificar sus demás consumidores; prioriza una integración acotada al explorador.

## Selección y acciones

1. Define estados de selección solicitada, confirmada, vacía y fallida. Conecta sus transiciones al contrato PB01 y aplica respuestas únicamente al contexto que corresponda.
2. Al abrir acciones sobre B con A seleccionado, fija B como objetivo explícito y coordina con el servidor antes de ejecutar lo que requiera confirmación/autoridad. No actives una opción de escritura usando la última sesión por conveniencia.
3. El botón visible de acciones, el clic derecho y la apertura por teclado deben utilizar la misma identidad y reglas de aplicabilidad. Mantén todas las operaciones aprobadas localizables y las capacidades de servidor como presentación, no como autorización final.
4. Conserva confirmaciones y formularios reales. Seleccionar o abrir menú no elimina, mueve ni cambia permisos. Cancelar no llama a una mutación.
5. Corrige la limpieza local de variables/campos y coordínala con el contrato backend de limpiar. El panel no puede conservar un destino implícito para comandos mientras anuncia «Sin selección».
6. Diferencia portapapeles de expediente y de archivos. Muestra origen/destino para traslado según D11; refleja nueva relación solo después de éxito. No añadas arrastrar y soltar.

## Ruta y contenido

Muestra el tipo y la ruta del elemento confirmado. Si D05 aprueba resumen de nivel, presenta sus hijos directos y acciones de organización; al abrir expediente utiliza el listado real y sus operaciones. Distingue nivel vacío, expediente vacío y lectura fallida. No incluyas documentos recursivos ni contadores fabricados.

Un resultado anterior puede mantenerse durante carga/error solo si queda identificado como anterior y no ofrece comandos sobre un encabezado nuevo. Una respuesta tardía no debe reemplazar contenido de la selección más reciente. Navegar desde la ruta utiliza el mismo contrato de selección.

## Búsqueda y filtros

Implementa el ámbito aprobado en D06. Para filtros locales, conserva ancestros de coincidencias sin contarlos como resultados adicionales. Filtrar no borra dominio ni cambia la selección silenciosamente. Si la selección queda fuera del filtro, informa su situación.

Preserva nombres canónicos y búsqueda por identidad para alias repetidos. Recupera la expansión según D15 y mantén la búsqueda documental del listado como ámbito separado. Los accesos explícitos de importación deben seguir abriendo el expediente confirmado en una rama contraída, sin volver visibles contenedores ocultos por otra causa.

## Carga, foco y adaptación

Finaliza los indicadores de lectura ante éxito, rechazo, timeout y respuesta inválida. Lecturas repetibles y mutaciones requieren políticas distintas: no reenvíes una escritura automáticamente porque no llegó el acuse. Utiliza la comprobación de resultado que el backend realmente provea y expresa incertidumbre cuando no pueda resolverse.

Mantén foco al actualizar menús/nodos o cerrar diálogos. Usa teclado/táctil según D17 y controles de formulario con ámbito explícito. Ajusta alturas con `auto_zise` y la barra existente sin introducir doble desplazamiento ni perder acciones con zoom.

## CRITERIOS DE ACEPTACIÓN

- Clic derecho sobre otro nodo, teclado, foco recuperado y una sola solicitud por activación.
- A/B rápido, dos páginas, revocación, limpiar selección y respuestas fuera de orden.
- Nivel mixto/vacío, expediente con/sin documentos y errores diferenciados de vacío.
- Filtros y búsqueda con ancestros, códigos largos, alias repetidos, selección fuera del filtro y restauración de expansión.
- Crear, renombrar, trasladar, ocultar/restaurar e importar conservan contexto, búsqueda y decorado.
- UpdatePanel repetido sin listeners/menús duplicados; Excel incompleto no bloquea navegación.
- Viewports y zoom/navegador acordados; accesos por teclado/táctil a las operaciones completas.

Añade pruebas con el manejador real y respuestas controladas que comprueben identidad, correlación y cantidad de solicitudes. No limites la prueba a encontrar el rótulo «Nivel» en la página. Ejecuta las suites locales pertinentes de compatibilidad.

## ENTREGABLE FINAL

Entrega página integrada, recursos y pruebas; crea `Doc/Diagnostico/modernizacion-arbol-produccion/implementacion/PF02-experiencia.md` con estados, contratos consumidos, decisiones, resultados y casos que pasarán a PB04/PB05. La reversión debe poder retirar la presentación compatible sin borrar movimientos o documentos ya persistidos.

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
