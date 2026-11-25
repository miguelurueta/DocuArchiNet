# Prompt PB04 para cerrar la compatibilidad de los accesos existentes

**Estado:** borrador de implementación e integración. Requiere PB00, PB01, PB02 y PF02; PB03 implementado u omitido justificadamente. Requiere D13 y las decisiones aplicables a las rutas integradas.

## Objetivo

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

## Verificación de aceptación

Ejecuta las suites locales relacionadas después de revisar que no requieren ambiente. Como base:

```text
node --test tests/doc119-context-menu.test.cjs tests/doc120-importacion-arbol.test.cjs
node --test tests/doc126-importacion-excel-ui.test.cjs
```

Selecciona además las pruebas `doc88-produccion-*` pertinentes si el cambio alcanza preparación, almacenamiento o proyección del cargador. No amplíes pruebas a servicios reales por el simple hecho de compartir un nombre de función.

Verifica integración con los handlers reales y transporte controlado: importar con otra selección activa, lote histórico, primer hijo, alias iguales, actualización parcial repetida, cancelación/rechazo y formularios ocultos inválidos. Para las operaciones que requieren IIS/DB/dispositivo/visor real, prepara casos con ambiente/cuentas y evidencia esperada y ejecútalos solo con la autorización correspondiente.

## Entrega

Crea `Doc/Diagnostico/modernizacion-arbol-produccion/implementacion/PB04-compatibilidad.md` con una fila por operación/acceso: validado, ajustado, diferido o pendiente de ambiente. Incluye pruebas y consumidores compartidos afectados. Ningún pendiente se presenta como «sin regresiones». Entrega las adaptaciones necesarias y deja a PB05 una matriz verificable, no una lista de afirmaciones generales.
