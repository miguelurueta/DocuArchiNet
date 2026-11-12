# DOC-119 — recuperación y enlaces de TreeView WebForms

Estado: **defecto reproducido y corrección local verificada; pendiente repetir SEG-02 real**. Progreso7/24; SEG-01 sigue aceptado. No se abre2.7 ni se inicia reorganización UI. [Evidencia saneada del fallo](doc119-recuperacion-arbol-20261112.json).

## Fallo real conservado

Corrida eb9311cafe72474198dc0c6d77c3fddb, código7464a92d, modo RECOVERY_ONLY, lote29/plan45, propietario141/empresa2, destino7 y alterno40. Cerrada el2026-11-12T20:35:52.740Z, credenciales eliminadas. Consumo **280HTTP/19SELECT/1sesión**, dentro de600/60/2. No se repitió SEG-01 ni se pidieron credenciales de Carolina.

Comprobó por SELECT los datos preparados:27 archivos,26 pendientes, uno recibido de677 bytes, un expediente, cero documentos almacenados e intentos RECEPCION/CREAR_EXPEDIENTE confirmados. Huella inicial b6c564f13b168822728f35e09ec7f551dde0b1da9ad183c68d65223bf108b278, coincidente con SEG-01 aceptado.

El ejecutor avanzó hasta el control TREE_RELATION_COUNT_INVALID, posterior a recarga, reselección local, nueva consulta y comprobaciones de selección/expansión. No terminó la aceptación del árbol ni ejecutó el control SELECT posterior; no acredita invariancia de negocio de toda SEG-02. La versión anterior no conservó conteos DOM antes/después, por lo que este registro **no demuestra un duplicado real ni su cantidad**. Los datos preparados se conservaron; no se enviaron confirmación/recepción/creación nuevas.

## Reproducción de la representación WebForms

El renderer de Microsoft aplica el ToolTip a los enlaces de imagen y texto del mismo nodo. La imagen usa el ID de texto con sufijo i: TreeViewArchivot1i y TreeViewArchivot1 comparten título. Fuente primaria: [TreeNode.cs, renderer de .NET Framework](https://github.com/microsoft/referencesource/blob/main/System.Web/UI/WebControls/TreeNode.cs#L1275), con generación del ID de imagen en líneas1101–1104.

Se incorporó ese HTML a Chromium con el cliente y la función Doc111ActualizarRama reales, servicios sintéticos y sin tráfico a IIS/MySQL. **Antes de corregir**, dos casos nuevos fallaron: padre con imagen impedía insertar por PARENT_AMBIGUOUS; relación ya representada provocaba TREE_RELATION_COUNT_INVALID. El caso de duplicado lógico real seguía rechazándose. Este patrón reproduce un defecto compatible con el fallo real; no sustituye una observación DOM que la corrida anterior no conservó.

## Corrección y regresión

Doc111ActualizarRama identifica padres y relaciones por enlaces lógicos cuyo ID cumple TreeViewArchivot seguido únicamente de dígitos. El observador de la prueba aplica el mismo contrato de nodos, descartando el enlace de imagen; dos IDs de texto distintos con una misma relación siguen contando como duplicado. No se borran nodos ni se fuerzan relaciones ausentes. Se conservan identidad, selección y expansión.

El ASPX referencia el JS con versión doc119-rama-20261112-1. Ambos ejecutores comprueban también el hash servido del JS del árbol; el ejecutor de datos incorpora el ASPX a su evidencia de código. No se cambia servicio, DTO, esquema, permisos o almacenamiento.

El verificador ahora conserva un diagnóstico saneado de disponibilidad y conteos lógicos antes/después, más dos indicadores de selección/expansión. En éxito y en rechazo permite distinguir cero, uno o varios nodos sin guardar HTML, títulos, nombres, tokens o cabeceras. Los ejecutores revalidan esa lista cerrada antes de escribirla al estado.

Regresión dirigida: **5/5 Chromium PASS**: rama disponible, oculta, WebForms con imagen e inserción, WebForms con relación existente y rechazo de duplicado lógico real. **49/49 políticas/correlación PASS**, incluyendo saneamiento de diagnósticos. No se equiparan esos casos locales con aceptación integrada real; el cierre técnico de esta corrección requiere repetir recuperación en la ventana autorizada.

La suite cliente completa posterior pasó **28/28**, incluidos esos cinco casos; no se suman como33 pruebas distintas. Sintaxis JS, diff y OpenSpec estricto también pasan. El consumidor del script inventariado es la página de Producción Documental; los demás manejadores legacy del mismo archivo no se modificaron.

## Continuación preparada

Mismo comando de solo recuperación: `node tools/e2e/scripts/run-doc119-datos-aceptacion.cjs --run <32-hex-nuevo> --authorized-plan --recovery-only ccb25ec9d37a4da083caccfbe80001b1`. Nuevo runId,600HTTP/60SELECT/2sesiones, propietario y MySQL SELECT por consola enmascarada; no pide Carolina y no repite preparación o SEG-01. Conservar corrida eb9311ca y su consumo; no reintento automático tras el fallo.

La falta de raíces físicas homónimas o de una inserción nueva observada sigue siendo una limitación explícita de estos datos; no se modifica esa regla de aceptación para abrir2.7 por inferencia.
