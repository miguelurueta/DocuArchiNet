# Prompt PF01 para modernizar la presentación del TreeView

**Estado:** borrador de implementación. Requiere PB00 y las decisiones D01, D02, D03, D04, D17 y el alcance de D18 aprobadas para las alternativas descritas.

## Objetivo

Actúa como desarrollador senior de JavaScript y ASP.NET WebForms. Aplica [el contexto común](contexto-comun.md) e integra la distinción visual de niveles y expedientes sobre el árbol existente, sin sustituir su DOM operativo ni habilitar acciones de backend nuevas.

Si D01 elige reemplazar el control o D04 conserva exclusivamente la activación sobre texto, revisa el alcance del prompt antes de editar. No fuerces las opciones de la maqueta sobre decisiones distintas.

## Áreas previstas

Revisa `Gestion/WebFormProducionDocumental.aspx`, `js/gestion/WebFormProducionDocumental.js`, `Styles/Aplicaction.css`, los estilos de Producción realmente utilizados y `GestionDocumental-Docuarchi.net.vbproj`. Confirma los nombres y consumidores antes de modificar. Crea recursos específicos del explorador cuando corresponda y limita sus selectores a ese ámbito.

## Implementación requerida

1. Resuelve un nodo canónico por cada enlace de texto del árbol. Identifica tipo desde el contrato de construcción y sus referencias válidas; no por imagen, texto, profundidad ni presencia de hijos.
2. Añade icono y etiqueta «Nivel»/«Expediente» según D02. Mantén compartición separada del tipo. El texto canónico del enlace permanece intacto para `.text`, `innerText`, búsqueda y renombrado.
3. Implementa una decoración idempotente para nodos iniciales y dinámicos. Atiende inserción, cambio de nombre, movimiento, restauración e importación. Si se usa observación de mutaciones, acótala al árbol, procesa cambios pertinentes y evita observar/redecorar indefinidamente los cambios propios.
4. Preserva tablas, IDs, ramas `nXNodes`, enlaces `tN`/`tNi` y el contrato de las funciones de búsqueda, inserción y eliminación. Los auxiliares no deben ser nuevos enlaces que el índice interprete como nodos.
5. Si D04 aprueba selección por fila, normaliza el objetivo al enlace canónico y ejecuta una única activación compatible. Icono, etiqueta y texto deben seleccionar lo mismo. El expansor mantiene su función independiente y no dispara otra consulta. No dupliques el manejador anterior.
6. Diferencia foco de selección y habilita el comportamiento de teclado aprobado. No suprimas los controles existentes hasta que su acceso equivalente esté integrado. Conserva nombres largos y acceso al texto completo.
7. Ajusta tamaño, contraste, separación y respuesta a viewport/zoom sin invalidar `auto_zise`. Agrega controles auxiliares `type="button"` y evita que formularios ajenos validen los envíos del gestor.

## Límites del paquete

No cambies reglas de permisos, consultas documentales, sesión, eliminación, traslado o importación para satisfacer una etiqueta visual. No muestres «Solo lectura» por el estado de compartición. No presentes esta entrega como solución a la coordinación entre pestañas; esa responsabilidad corresponde a PB01/PB02.

## Verificación de aceptación

- Nivel vacío, propio, compartido y recibido; expediente con código largo; dos alias iguales.
- Construcción inicial y primer hijo dinámico; renombrado sin etiquetas agregadas al nombre; movimiento/restauración/importación conservan el estilo.
- Repetir inicialización y actualizaciones parciales no duplica elementos, listeners ni envíos.
- Clic/teclado no producen dos solicitudes; expandir no cambia la selección involuntariamente.
- Búsqueda y `Doc120MostrarExpediente` conservan identidad y expansión.
- Revisar anchos representativos y zoom real, foco y retorno del foco según D17. No acreditar lector de pantalla solo por añadir roles.

Ejecuta las pruebas locales pertinentes de PB00 y añade pruebas de comportamiento para las modificaciones del decorador y la normalización de clics. El navegador debe comprobar el nombre canónico y el número de activaciones, no solo la presencia de una clase CSS.

## Entrega

Entrega recursos integrados en la página/proyecto, pruebas, instrucciones para retirar solo esta capa visual y `Doc/Diagnostico/modernizacion-arbol-produccion/implementacion/PF01-presentacion.md`. Describe qué permanece sujeto a los contratos de selección actuales y qué necesita PB01/PF02. No entregues únicamente otra maqueta aislada.
