# Prompt 05 — Ventana integrada en Producción

## Encargo y dependencias

Lee la guía 00, el prompt 03, `Diseno/02-contratos-operacion.md` cuando exista y `Prototipo/README.md`. Implementa la interfaz real del recorrido aprobado en WebForms y JavaScript. Consume perfil 02, plan 06 y operaciones definidas para 07/10/11; no implementes sus reglas dentro del frontend.

## Trabajo

- Inspecciona `Gestion/WebFormProducionDocumental.aspx`, `js/gestion/WebFormProducionDocumental.js`, el contrato del nodo seleccionado y los controles de modal/tabla existentes. Ubica la acción «Importar carpetas» junto a las acciones apropiadas, sin reemplazar la carga individual.
- Habilita apertura solo con nivel elegible; solicita validación al servidor. Captura el contexto por instancia de ventana, no mediante variables globales compartidas con otra carga. Mostrar ruta de origen de solo lectura.
- Adapta la base gráfica, sin copiar el árbol ilustrativo ni sus catálogos. Conserva Perfil → Carpetas → Revisión → Resultados, panel de destino y distinción nivel/gabinete. Filtra perfiles compatibles sin cambiar el nivel por seleccionar un perfil.
- Implementa inventario con identidades de raíz/archivo, referencias File y rutas relativas. Agregar carpetas repetidamente; manejar homónimos, limitaciones de vacías y falta de soporte del navegador. No cargar todo el contenido en memoria ni depender de posiciones de filas.
- Renderiza grandes inventarios de forma acotada; escapar nombres externos, liberar previews y mostrar incidencias accesibles. No ejecutar extracción documental definitiva en el navegador.
- Conecta preparación/confirmación con revisión del plan. Cambios de contenido invalidan el plan; muestra política completa/parcial y ámbito efectivo junto a la confirmación. No permitir cambiarla mediante un checkbox de ocasión que el perfil no autorice.
- Separa adaptadores HTTP de estado/renderizado. Resuelve errores, expiración, timeout y cancelación de peticiones; no dejar promesas pendientes. No interpretar HTTP 200 como documento incorporado.
- Integra resultados reales cuando existan sus servicios. No mantener DEMO, escenarios de error ficticios, catálogos fijos ni temporizadores como ejecución productiva.
- Respeta ClientID, ciclo de UpdatePanel, botones sin postback accidental, inicialización única y destrucción de eventos. No duplicar bibliotecas ni introducir CSS global que afecte otros módulos.

## Límites

Propiedad: esta entrega implementa etapas 1–3 y el contenedor visual de resultados; 11 conecta la consulta, recuperación, CSV y actualización del árbol sobre esos mismos componentes. No construir otra ventana en 11 ni dos clientes HTTP para la misma operación. Consumir `puedeConfirmar` y acciones permitidas del servidor; PARCIAL_PERMITIDO no se bloquea globalmente por cualquier error individual. No copiar la semántica de errores/temporizadores de la maqueta como lógica de producción.

No diseñar tablas ni modificar almacenamiento/SII. Si falta un servicio, usa dobles únicamente en pruebas y registra la integración pendiente; la UI productiva no debe simular éxito. No crear otra administración de estructuras.

## Aceptación y entrega

Verifica selección de nivel y apertura única, cambio de perfil sin cambio de destino, varias raíces, nombres repetidos, errores que bloquean, revisión invalidada, teclado/foco del modal, cierre/reapertura y actualizaciones parciales sin eventos duplicados. Comprobar respuesta manipulada/inesperada sin romper la pantalla. Entregar componentes, contratos consumidos, capturas de estados reales o identificadas como pruebas y resultados. Las pruebas autenticadas siguen la autorización de la guía 00.
