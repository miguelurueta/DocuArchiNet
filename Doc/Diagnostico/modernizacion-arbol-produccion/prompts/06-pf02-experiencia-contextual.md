# Prompt PF02 para integrar navegación y acciones contextuales

**Estado:** borrador de implementación. Requiere PF01, PB01, PB02 y PB03 si suministra datos necesarios. Decisiones D01 a D10 y D14 a D18 aplicables aprobadas; D11/D12 para acciones respectivas y D13 para accesos de importación.

## Objetivo

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

## Verificación de aceptación

- Clic derecho sobre otro nodo, teclado, foco recuperado y una sola solicitud por activación.
- A/B rápido, dos páginas, revocación, limpiar selección y respuestas fuera de orden.
- Nivel mixto/vacío, expediente con/sin documentos y errores diferenciados de vacío.
- Filtros y búsqueda con ancestros, códigos largos, alias repetidos, selección fuera del filtro y restauración de expansión.
- Crear, renombrar, trasladar, ocultar/restaurar e importar conservan contexto, búsqueda y decorado.
- UpdatePanel repetido sin listeners/menús duplicados; Excel incompleto no bloquea navegación.
- Viewports y zoom/navegador acordados; accesos por teclado/táctil a las operaciones completas.

Añade pruebas con el manejador real y respuestas controladas que comprueben identidad, correlación y cantidad de solicitudes. No limites la prueba a encontrar el rótulo «Nivel» en la página. Ejecuta las suites locales pertinentes de compatibilidad.

## Entrega

Entrega página integrada, recursos y pruebas; crea `Doc/Diagnostico/modernizacion-arbol-produccion/implementacion/PF02-experiencia.md` con estados, contratos consumidos, decisiones, resultados y casos que pasarán a PB04/PB05. La reversión debe poder retirar la presentación compatible sin borrar movimientos o documentos ya persistidos.
