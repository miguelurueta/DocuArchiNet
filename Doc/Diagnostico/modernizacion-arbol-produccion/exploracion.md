# Exploración de la modernización de niveles y expedientes

La mejora busca que el usuario distinga inmediatamente un **nivel de organización**, que agrupa elementos, de un **expediente**, que contiene documentos. Debe conservar las operaciones actuales de Producción documental y permitir aprobar cada cambio antes de generar sus prompts de implementación.

La alternativa recomendada es conservar el `asp:TreeView`, modernizar su presentación y resolver explícitamente la coordinación entre elemento indicado, selección visual y destino utilizado por el servidor. El cambio de componente y la reescritura general del backend quedan como alternativas que requieren una decisión propia.

| Control de la exploración | Valor |
|---|---|
| Versión | 0.1 |
| Fecha | 2026-11-25 |
| Estado | En exploración |
| Responsable de aprobación funcional | Usuario solicitante |
| Responsable del análisis técnico | Codex como arquitecto de la revisión |
| Decisiones aprobadas individualmente | Ninguna registrada al crear esta versión |
| Primera decisión presentada | D01, conservación del TreeView |
| Base local de la revisión | Commit `1ac8cee2` y archivos indicados en las referencias |
| Alcance de esta entrega | Análisis y registro de decisiones; implementación posterior |

Referencias de la propuesta:

- [Maqueta interactiva](maqueta.html).
- [Diagnóstico técnico e inventario completo de operaciones](diagnostico.html).

## Cómo utilizar y mantener la exploración

El interés expresado por la mejora autoriza esta exploración; no equivale a aprobar sus detalles. La maqueta ilustra alternativas y contiene operaciones simuladas. Las recomendaciones siguientes permanecen pendientes hasta que el usuario decida sobre cada una.

1. Presentar una decisión con sus alternativas, recomendación e impacto.
2. Resolver las preguntas sobre esa decisión con código, ejemplos o una revisión de la maqueta.
3. Registrar la respuesta explícita del usuario y la opción elegida.
4. Actualizar las decisiones dependientes y el alcance de los futuros prompts.
5. Continuar con la siguiente decisión, conservando el historial.

Los estados de una decisión son `PENDIENTE`, `APROBADA`, `RECHAZADA`, `DIFERIDA` y `REABIERTA`. Una respuesta que modifica la propuesta genera una nueva revisión de esa decisión; no se interpreta como aceptación de las demás. El silencio no cambia el estado.

Una duda técnica puede quedar `RESUELTA POR CÓDIGO` sin que la solución de producto esté aprobada. Una duda de ambiente puede necesitar una medición autorizada. No se reemplaza esa medición con los datos sintéticos de la maqueta.

Las aprobaciones se registran en este archivo como fuente de verdad. Si se modifica una decisión aprobada, se conserva su registro anterior, se marca `REABIERTA` y se revisan los prompts y decisiones que dependían de ella. Un prompt ya generado debe indicar que necesita revisión cuando cambia alguna de sus decisiones de origen.

### Formato de una aprobación individual

Puede responderse, por ejemplo, `Apruebo D01 opción A`, `D01 opción A con este ajuste: ...`, `Rechazo D01` o `Dejamos D01 para después`. La respuesta se registra con fecha, autor, versión de la decisión y consecuencia concreta.

```text
Decisión: Dxx
Revisión de la decisión: 1
Estado anterior: PENDIENTE
Estado nuevo: APROBADA | RECHAZADA | DIFERIDA | REABIERTA
Opción o redacción aceptada:
Respuesta explícita del usuario:
Fecha de aprobación:
Decisiones y prompts afectados:
```

Este bloque es una plantilla de registro, no una aprobación.

## Objetivo y límites de la mejora

La modernización debe ayudar a responder tres preguntas: **qué elemento es**, **dónde está** y **qué puedo hacer con él**. El tipo, el estado de compartición y los permisos son conceptos diferentes y deben representarse por separado.

El nivel puede contener subniveles y expedientes, incluidos niveles vacíos. El expediente conserva su identidad documental y se presenta dentro de una relación con un nivel. Una hoja del árbol no identifica por sí sola un expediente.

La propuesta contempla identidad visual, ruta, acciones contextuales, accesibilidad, búsqueda y contenido según el tipo seleccionado. Las reglas archivísticas, los formularios completos, el almacenamiento documental, la importación, el índice, los rótulos y la autorización mantienen sus contratos actuales salvo aprobación expresa de un ajuste concreto.

Quedan fuera de la recomendación inicial: migrar el framework, sustituir todo el árbol, crear una SPA, introducir una base de datos nueva, arrastrar niveles, ampliar permisos, eliminar validaciones o reescribir los módulos de importación. Incluir alguno exige abrir la decisión correspondiente y recalcular el alcance.

## Evidencia y componentes revisados

Las referencias de línea ayudan a localizar el código; el nombre de la función prevalece si cambia su posición.

| Evidencia | Componente y ubicación | Qué establece |
|---|---|---|
| E01 | [Proyecto WebForms](../../../GestionDocumental-Docuarchi.net.vbproj) | VB.NET y objetivo .NET Framework 4.6.1. |
| E02 | [Página de Producción documental](../../../Gestion/WebFormProducionDocumental.aspx), `TreeViewArchivo`, línea 426; menús desde 280 | Configuración del árbol, menús, controles ocultos y UpdatePanels. |
| E03 | [Construcción y operaciones del árbol](../../../Gestion/Class_niveles_organizacion.vb), `Lista_niveles_de_organizacion`, línea 431 | Tipos de nodo, identidad, imágenes y anidación inicial. |
| E04 | [JavaScript de Producción](../../../js/gestion/WebFormProducionDocumental.js), `OnNodeClicked`, línea 1962; `OnSearchClick`, 2039; `CargarDatos`, 2803 | Selección, lectura del nombre canónico y búsqueda. |
| E05 | [Code behind de Producción](../../../Gestion/WebFormProducionDocumental.aspx.vb), `Button_inline_trevie_Click`, línea 824 | Sincronización de campos ocultos con sesión y consulta de archivos. |
| E06 | [Servicios de Producción](../../../webservice/WebServiceProducion.asmx.vb), `Get_cortar_expediente`, línea 308; `Get_pegar_expediente`, 342 | Uso de sesión en cortar y pegar; comprobación de tipo y propiedad de origen. |
| E07 | [Operación de traslado](../../../Gestion/Class_niveles_organizacion.vb), `Traslado_carpeta_nivel`, línea 1109 | Propiedad del destino, comprobación de duplicados y nueva relación dentro de una transacción. |
| E08 | [JavaScript de Producción](../../../js/gestion/WebFormProducionDocumental.js), `Doc119NivelPulsado`, línea 2; `Doc120MostrarExpediente`, 1066; `Doc111ActualizarRama`, 1080; `create_trenode_treview`, 1116 | Captura del destino de importación, navegación exacta e inserción incremental de nodos. |
| E09 | [Relaciones entre niveles y expedientes](../../../Gestion/Class_ra_pro_niveles_has_expediente_archivo.vb), líneas 8 y 45 | Identidad de relación y consulta que toma el primer nivel encontrado por expediente. |
| E10 | [Code behind de Producción](../../../Gestion/WebFormProducionDocumental.aspx.vb), handlers de compartir, permisos y ocultos, líneas 1626, 1643 y 2766 | Aplicabilidad de acciones por tipo y comprobaciones de propietario. |
| E11 | [JavaScript de Producción](../../../js/gestion/WebFormProducionDocumental.js), `clear_seleccion_treview`, línea 1867; `auto_zise`, 3012 | Limpieza local de selección y cálculos de tamaño. |
| E12 | [Menú contextual](../../../js/jquery.contextMenu.js) | Captura del elemento que invoca el menú, estado habilitado, teclado y retorno de foco. |
| E13 | [Contexto de importación](../../../Infrastructure/ProduccionDocumental/ImportacionCarpetas/ImportacionCarpetasContextAdapter.vb), `ResolverDestino` | Patrón existente de autoridad y destino explícitos para importación; no es un autorizador universal del gestor. |
| E14 | [Vista de Excel](../../../js/gestion/ImportacionExcel/ImportacionExcelVista.js), `mount` y `node` | Separación del propietario de formulario para evitar validación cruzada con `form1`. |
| E15 | [Pruebas de árbol e importación](../../../tests/doc120-importacion-arbol.test.cjs) y [pruebas de Excel](../../../tests/doc126-importacion-excel-ui.test.cjs) | Casos locales reutilizables de identidad, ramas, navegación y validación de formularios. |
| E16 | [Eliminación de expediente](../../../Gestion/ClassGaExpediente.vb), `Eliminar_Expediente`, línea 5391; [eliminación de nivel](../../../Gestion/Class_niveles_organizacion.vb), línea 547 | Eliminación de dominio y restricciones existentes; no equivale a quitar un nodo visual. |

El cambio OpenSpec activo DOC-127 corresponde a la verificación de Excel y no se convierte en el cambio de modernización del árbol. La creación de un cambio OpenSpec propio se decidirá al cerrar el alcance; esta exploración no modifica sus tareas ni presume un ticket nuevo.

## Dudas técnicas resueltas

### T01 Es posible mejorar la interfaz sin reemplazar el control

**Sí.** El control actual ya representa la jerarquía. Se puede añadir un decorador de presentación y accesos contextuales conservando el contrato de sus enlaces, tablas e identificadores. El alcance de esa alternativa se aprueba en D01. Evidencia E02 a E04.

### T02 Cómo se distingue un nivel de un expediente

El servidor construye niveles con `id_nivel` y expedientes con la identidad compuesta `relacionId|nivelId|expedienteId`, con separador final en el tooltip del enlace. La barra inversa distingue los títulos de nivel usados por el cliente. No se identifica el tipo por profundidad, icono, nombre ni condición de hoja. Una representación inválida no debe adivinarse. Evidencia E03, E04 y E08.

### T03 Un nivel vacío es un expediente

**No.** Puede no tener rama de hijos y seguir siendo nivel. También debe poder recibir su primer expediente, conservar expansor y actualizar la búsqueda. Evidencia E03, E08 y E15.

### T04 Un elemento compartido es un tercer tipo

**No.** El compartido es un estado y una relación de acceso. El tipo continúa siendo nivel o expediente. Ser receptor tampoco implica automáticamente solo lectura: cada capacidad depende de la autorización vigente. Evidencia E03, E10 y E13.

### T05 Se pueden introducir etiquetas dentro del enlace sin más cambios

**No es seguro con el manejador actual.** `OnNodeClicked` exige objetivo `A`, y otras funciones consumen `.text` e `innerText`. Una etiqueta dentro del enlace puede cambiar el objetivo y contaminar el nombre. El decorador debe separar la ornamentación del texto canónico o adaptar y verificar expresamente esos consumidores. Evidencia E04.

### T06 El clic en un icono y el clic en el texto ya equivalen

El manejador omite enlaces que contienen imágenes. El HTML generado por WebForms también puede incluir un enlace `tNi` y un enlace de texto `tN` para el mismo nodo. Ampliar la superficie de clic requiere resolver un único nodo canónico y emitir una sola activación. Evidencia E03, E04 y E08.

### T07 El árbol se reconstruye siempre desde un UpdatePanel

**No debe suponerse.** El árbol está antes de `UpdatePanelViewArchivo` y también se modifica directamente en JavaScript. Deben atenderse construcción inicial, nodos dinámicos y actualizaciones parciales, sin repetir listeners ni decoración. Evidencia E02, E04 y E08.

### T08 La selección visual ya es la selección del servidor

**No necesariamente.** El cliente cambia estilos y campos ocultos antes de activar `Button_inline_trevie`. El servidor actualiza después la sesión y consulta archivos. Deben distinguirse selección solicitada, confirmada y fallida. Evidencia E04 y E05.

### T09 Basta esperar el postback para resolver todas las carreras

Esperarlo mejora la coordinación de una página, pero los servicios actuales consumen una sesión compartida por las peticiones del usuario. Dos pestañas o una acción posterior sobre otro destino requieren que la operación compruebe su objetivo explícito; un cambio de color o una versión exclusivamente local no garantiza esa comprobación. D08 decide el alcance del ajuste de backend. Evidencia E05 y E06.

### T10 Limpiar selección también limpia el servidor

La función local `clear_seleccion_treview` vacía campos/variables del navegador, no contiene una llamada al servidor y presenta una asignación `.vaule` al texto oculto. Por tanto, esa función no acredita limpieza de sesión ni de todo el estado visual. La implementación futura debe definir y probar el alcance de limpiar, sin ejecutar después comandos contra un destino anterior. Evidencia E11.

### T11 Elegir un nivel debería mostrar documentos de todos sus descendientes

El comportamiento actual asigna expediente 0 cuando se selecciona nivel y llama al listado documental. No ofrece una consulta recursiva de sus documentos. Mostrar sus hijos directos puede construirse sobre la rama cargada; agregar documentos de descendientes sería una capacidad distinta, con permisos, volumen y consultas propios. D05 propone el resumen directo. Evidencia E05.

### T12 Los filtros visuales necesitan tablas nuevas

Los filtros sobre nodos ya cargados y el resumen directo no requieren por sí mismos persistencia nueva. Buscar elementos no cargados, obtener totales globales o virtualizar el árbol cambia los requisitos. D06 y D16 deben fijar el alcance antes de elegir consultas o esquema. Evidencia E02, E04 y E08.

### T13 Es suficiente el identificador del expediente para determinar su nivel

La interfaz transporta la relación completa. Además, `Solicita_id_nivel_expediente` consulta por expediente y toma la primera fila. Eso no demuestra unicidad de relaciones en el ambiente. Para actuar sobre una aparición concreta se recomienda comprobar relación, nivel y expediente; no usar alias ni la primera coincidencia como sustituto. Evidencia E03 y E09.

### T14 Mover equivale a copiar el nodo y borrarlo del DOM

**No.** La operación actual valida destino y duplicado, registra una nueva relación y elimina la relación fuente dentro de una transacción. El expediente conserva su ID y la relación puede cambiar. El árbol solo debe reflejar el éxito confirmado. Los detalles de aislamiento, errores y reintentos deben verificarse antes de cambiar la operación. Evidencia E06 y E07.

### T15 Un usuario con capacidad de mover puede pegar en cualquier nivel compartido

No se deduce así del flujo revisado. `Get_cortar_expediente` exige propiedad del nivel fuente y `Traslado_carpeta_nivel` exige propiedad del destino. No se amplía esa regla al añadir menús; cualquier cambio de negocio requiere aprobación independiente. Evidencia E06 y E07.

### T16 Eliminar y ocultar tienen el mismo efecto

**No.** Ocultar altera visibilidad de nivel/rama y dispone de restauración. `Eliminar_Expediente` tiene restricciones sobre control documental, volúmenes y relaciones y contiene eliminación de `expediente_archivo`, no solo de su aparición en el árbol. Eliminar nivel también tiene comprobaciones propias. D12 conserva los flujos reales y una confirmación específica. Evidencia E10 y E16.

### T17 Se puede cambiar la importación para acomodar el nuevo árbol

La importación ya captura nivel y contexto y navega al expediente por identidad exacta. Sus comprobaciones de ambigüedad y visibilidad forman parte del contrato a preservar. El adaptador visual debe integrarse con ese contrato; no hacer más permisivo el destino ni reutilizar la autoridad de importación para todas las operaciones del gestor. Evidencia E08, E13 y E15.

### T18 La mejora puede reintroducir el bloqueo de formularios

Sí, si añade controles que participen por error en la validación global. La corrección de Excel utiliza otro propietario de formulario sin mover necesariamente el diálogo. Los controles nuevos deben tener ámbito explícito y los botones auxiliares deben evitar envíos involuntarios. Se conservarán las pruebas de validación cruzada. Evidencia E14 y E15.

### T19 La maqueta demuestra que la producción queda sin regresiones

**No.** La maqueta utiliza datos sintéticos. La revisión previa registró 13 escenarios locales y 10 pruebas del árbol/importación aprobados; estos resultados son una base y no certifican la futura integración de WebForms, DLL, permisos y base de datos. La aceptación real se define en D18 y en la matriz de escenarios de este documento.

### T20 Hay que crear una API nueva para toda la aplicación

No se justifica por esta mejora. Primero se inventarían los contratos necesarios para selección y acciones afectadas. La recomendación es un ajuste acotado al módulo y sus adaptadores, reutilizando reglas vigentes; introducir endpoints o cambiar firmas es una consecuencia de las decisiones aprobadas, no un requisito visual automático.

## Registro de decisiones pendientes

Todas las opciones recomendadas en esta versión están pendientes. La primera ronda presenta solamente D01. Las dependencias indican qué debe revisarse antes de cerrar una decisión, no una autorización implícita.

| Decisión | Pregunta | Opción recomendada | Dependencias | Estado |
|---|---|---|---|---|
| D01 | Conservar o sustituir el TreeView | Conservarlo y modernizar por etapas | Ninguna | PENDIENTE |
| D02 | Cómo identificar los dos tipos | Icono y etiqueta textual permanentes | D01 | PENDIENTE |
| D03 | Cómo nombrar el explorador y sus estados | Niveles y expedientes; compartición separada | D02 | PENDIENTE |
| D04 | Cómo funcionan fila, expansión y teclado | Selección y expansión independientes | D01, D02 | PENDIENTE |
| D05 | Qué se muestra al seleccionar un nivel | Contenido directo; documentos al abrir expediente | D01, D08 | PENDIENTE |
| D06 | Qué busca y filtra el explorador | Nodos cargados, conservando ancestros y ámbito | D04, D05 | PENDIENTE |
| D07 | Cómo se presentan las operaciones | Acciones por tipo y capacidad confirmada | D02, D08, D09 | PENDIENTE |
| D08 | Cómo se garantiza el destino de una acción | Contexto explícito validado en servidor | D01, D10 | PENDIENTE |
| D09 | Cómo representar los permisos | Conservar reglas y capacidades por operación | D08, D10 | PENDIENTE |
| D10 | Qué identifica una aparición del expediente | Relación, nivel y expediente comprobados | D01 | PENDIENTE |
| D11 | Cómo se conserva cortar y pegar | Flujo actual con confirmación y resultado confirmado | D08, D09, D10 | PENDIENTE |
| D12 | Cómo se muestran ocultar y eliminar | Flujos separados y restricciones vigentes | D08, D09, D10 | PENDIENTE |
| D13 | Qué cambia en las importaciones | Compatibilidad con sus contratos actuales | D01, D08, D10 | PENDIENTE |
| D14 | Cómo se resuelven carga y errores | Estado explícito, recuperación y ausencia de reintento destructivo | D08, D11 | PENDIENTE |
| D15 | Qué preferencias se recuerdan | Filtros y expansión por página durante la sesión de uso | D06, D08 | PENDIENTE |
| D16 | Qué se hace con rendimiento y contadores | Contenido cargado; medir antes de carga diferida | D05, D06, D10 | PENDIENTE |
| D17 | Qué accesibilidad se exige | Teclado, foco, etiquetas y adaptación visual | D02, D04, D07 | PENDIENTE |
| D18 | Cómo se divide, acepta y convierte en prompts | Entregas verificables con decisiones trazadas | Todas las incluidas en cada entrega | PENDIENTE |

### D01 Conservación del control existente

**Pregunta:** ¿se mantiene el TreeView para la primera modernización?

- **A, recomendada:** conservar `asp:TreeView`, sus IDs, enlaces y estructura operativa; añadir presentación y adaptadores acotados.
- **B:** sustituir el componente y adaptar todos los consumidores de su DOM e identidad.
- **C:** diferir la decisión y realizar una comparación técnica adicional.

La opción A permite entregar primero claridad visual y verificar compatibilidad por operación. B necesita diseño de datos y migración propia; la estimación anterior de 10 a 15 días hábiles no debe aplicarse automáticamente a esa alternativa. Aprobación funcional de A no elige todavía el mecanismo de sincronización de D08.

**Aceptación:** el control existente sigue soportando búsqueda, inserción, traslado e importación; la maqueta no se copia como reemplazo directo de su marcado.

### D02 Identidad visual de niveles y expedientes

**Pregunta:** ¿se diferencia el tipo por icono y texto permanentes?

- **A, recomendada:** capas + «Nivel» y archivador + «Expediente», con estilos distinguibles también sin color.
- **B:** conservar solo iconos y tooltip, con menor cambio visual y menor claridad.

La etiqueta debe quedar fuera del alias canónico. El nivel vacío conserva su tipo. No se altera la identidad enviada al backend ni se cambia el nombre documental para incluir el tipo.

**Aceptación:** tipo correcto en nodos iniciales, creados, movidos, restaurados e importados; búsqueda y nombres sin texto de decoración.

### D03 Vocabulario y compartición

**Pregunta:** ¿el menú agrupador pasa a llamarse «Niveles y expedientes» y muestra compartición por separado?

- **A, recomendada:** ese nombre para el explorador; «Compartido» y «Compartido contigo» como estados adicionales cuando existan datos suficientes.
- **B:** «Estructura documental» como agrupador y el mismo tratamiento separado de estados.

No se muestra «Solo lectura» por el mero hecho de ser un nivel recibido. El estado necesita datos reales de autoridad para expresar capacidades. A y B son alternativas de vocabulario, no cambios de reglas de acceso.

**Aceptación:** el usuario reconoce tipo y estado sin consultar el tooltip y todos los accesos actuales siguen localizables.

### D04 Selección y expansión

**Pregunta:** ¿la fila activa el elemento y el expansor únicamente abre o cierra su rama?

- **A, recomendada:** una activación canónica por fila; expansor independiente; foco por teclado distinto de selección.
- **B:** mantener inicialmente la activación sobre el texto y limitar la primera entrega a la distinción visual.

A exige adaptar el manejo de objetivos anidados y omitir el enlace de imagen como segundo nodo lógico. Las flechas recorren/expanden; Enter activa. Expandir no debe activar una consulta documental nueva por sí solo.

**Aceptación:** texto, icono y etiqueta llevan al mismo elemento con una sola activación; expandir conserva selección y destino.

### D05 Contenido del panel derecho

**Pregunta:** ¿un nivel muestra sus elementos directos y un expediente sus documentos?

- **A, recomendada:** resumen directo de la rama cargada, ruta y acciones de nivel; listado actual al abrir un expediente.
- **B:** conservar el panel documental actual también al seleccionar nivel y mejorar únicamente su mensaje vacío.
- **C:** agregar documentos de descendientes; requiere una capacidad separada de consulta y autorización.

A no agrega documentos recursivos, no modifica la estructura almacenada y no necesita una consulta por cada nodo. El cambio de panel debe quedar asociado a selección confirmada para evitar que el encabezado y los documentos correspondan a destinos distintos.

**Aceptación:** nivel vacío, nivel mixto, expediente vacío y expediente con documentos tienen mensajes y acciones correctos.

### D06 Búsqueda y filtros

**Pregunta:** ¿la primera búsqueda moderna opera sobre la estructura ya disponible?

- **A, recomendada:** buscar niveles/expedientes cargados y filtrar por tipo, conservando ancestros de coincidencias.
- **B:** mantener inicialmente el buscador actual, sin filtros nuevos.
- **C:** búsqueda global del servidor y de elementos no cargados, como alcance adicional.

La búsqueda de archivos permanece separada. Filtrar no elimina elementos ni cambia silenciosamente la selección; si la selección queda fuera del filtro, se indica. Los ancestros mostrados como ruta no se cuentan como coincidencias adicionales. Restaurar la búsqueda recupera la expansión acordada.

**Aceptación:** nombres repetidos, códigos largos, tildes, rama contraída y cero resultados; no se revelan nodos ocultos o sin acceso para producir coincidencias.

### D07 Acciones visibles por elemento

**Pregunta:** ¿cada fila ofrece un botón de acciones y un menú adecuado a su tipo?

- **A, recomendada:** botón accesible además del clic derecho; opciones por tipo y capacidad confirmada, con destino explícito.
- **B:** conservar la distribución de menús, mejorando sus etiquetas y mensajes por tipo.

La recomendación conserva todos los comandos del inventario. El menú abierto sobre B no puede ejecutar sobre A. Mostrar un botón deshabilitado debe explicar su indisponibilidad cuando sea útil; el servidor conserva la decisión de autoridad.

**Aceptación:** menú sobre una fila distinta de la seleccionada, desde teclado y desde táctil; acceso a las operaciones completas y no solo a las representadas en la maqueta.

### D08 Destino autorizado y coordinación con el servidor

**Pregunta:** ¿la mejora de acciones incluye corregir la dependencia de un destino implícito de sesión en las rutas afectadas?

- **A, recomendada:** representar destino explícito por operación y validarlo en servidor antes de usar el flujo existente. Coordinar selección solicitada/confirmada y evitar aplicar respuestas fuera de contexto.
- **B:** conservar llamadas que solo consumen sesión y restringir la primera entrega a presentación; no declarar resuelto el riesgo entre páginas/pestañas.

El contexto conceptual incluye tipo, nivel, relación y expediente cuando corresponda, acción solicitada e identificador de solicitud/página para correlación. Los IDs del cliente no conceden autoridad; usuario/empresa y permisos se resuelven en servidor. La forma concreta de transportar, versionar y proteger el contexto se diseña después de aprobar A y de inventariar las firmas afectadas.

La comprobación y la invocación de la operación legacy deben referirse al mismo destino dentro de la petición. Un postback previo que cambia sesión, seguido de otra llamada sin destino, no cumple por sí solo esa condición. Los caminos antiguos siguen necesitando un plan de compatibilidad; no se promete aislamiento total de pestañas con un ajuste exclusivamente visual.

**Aceptación:** selección A/B rápida, clic derecho B, dos pestañas, destino retirado, permiso revocado y limpieza de selección. Ninguna mutación se ejecuta sobre un destino anterior de manera silenciosa.

### D09 Permisos y capacidades

**Pregunta:** ¿se conservan las reglas actuales y se muestran capacidades confirmadas por operación?

- **A, recomendada:** mantener las reglas y resolver aplicabilidad/autoridad de forma separada, reutilizando cada validación del dominio.
- **B:** revisar las reglas de negocio en un cambio independiente antes de ampliar accesos.

Una respuesta de capacidades sirve para presentar la interfaz; cada escritura vuelve a validar la autoridad vigente. El adaptador de importación se usa solo en su ámbito. Un módulo nuevo de contexto no debe suponer que editar, compartir, eliminar, importar y mover tienen el mismo permiso.

**Aceptación:** propietario, receptor con capacidades diferentes y permiso retirado después de abrir el menú; ausencia de acceso se expresa con un resultado controlado, sin inventar permisos.

### D10 Identidad de la relación seleccionada

**Pregunta:** ¿cada acción sobre una aparición del expediente comprueba la relación completa?

- **A, recomendada:** conservar y validar relación + nivel + expediente; usar alias únicamente como presentación.
- **B:** seguir resolviendo nivel solo por expediente; deja pendiente demostrar unicidad y resolver ambigüedades.

A no exige declarar que existen múltiples relaciones en producción. Evita depender de ese supuesto y permite detectar una relación obsoleta después de mover. Los rangos de IDs deben ser compatibles con los consumidores existentes: hay `Integer` en firmas legacy y referencias `UInteger` en importación; no se amplían rangos sin inventario.

**Aceptación:** dos alias iguales, relación inexistente, expediente movido y referencias fuera de rango. Ante ambigüedad, la operación informa y no elige la primera coincidencia.

### D11 Cortar y pegar expediente

**Pregunta:** ¿se conserva la operación de traslado actual y se hace explícito su portapapeles?

- **A, recomendada:** cortar expediente, seleccionar nivel de destino y confirmar origen/destino; actualizar el árbol después del resultado confirmado.
- **B:** conservar la presentación actual de cortar/pegar durante la primera entrega.

La regla actual exige propiedad de origen y destino y rechaza el mismo nivel/relación duplicada. El portapapeles del expediente es distinto del de archivos. No se incorpora movimiento de niveles ni arrastrar y soltar. Un resultado incierto no justifica repetir automáticamente una mutación.

**Aceptación:** cancelación, expediente movido por otro proceso, destino propio, destino compartido sin propiedad y primer hijo del nivel vacío; ID de expediente estable y nueva relación respetada.

### D12 Ocultar y eliminar

**Pregunta:** ¿ambas acciones conservan sus flujos separados y las restricciones vigentes?

- **A, recomendada:** ocultar/restaurar nivel con texto específico; eliminar con confirmación de tipo, nombre y alcance de la operación de dominio existente.
- **B:** diferir cambios de presentación de eliminación y conservar íntegramente su acceso actual.

La maqueta no ejecuta eliminación. El prompt posterior debe inspeccionar y conservar todas sus restricciones antes de cambiar etiquetas o confirmaciones. No se transforma «Eliminar expediente» en «Quitar de este nivel» ni se promete restaurar lo eliminado. Cualquier propuesta de eliminación lógica requiere otra decisión de negocio.

**Aceptación:** nivel con hijos, expediente con restricciones, cancelar, rechazo del servidor, éxito y restauración de ocultos; el DOM solo refleja éxitos confirmados.

### D13 Compatibilidad de importación y herramientas

**Pregunta:** ¿la modernización conserva las rutas de importación y sus formularios?

- **A, recomendada:** mantener importación de carpetas, consulta de lotes, Excel, perfiles, fuentes y proveedores; adaptar solo la integración visual necesaria.
- **B:** rediseñar también esas herramientas como alcance independiente.

Se preservan identidad compuesta, contexto, inserción del primer hijo, duplicados, ramas ocultas y acceso al expediente confirmado. La selección del árbol no cambia el destino de un lote ya iniciado. Los controles de Excel mantienen un propietario de formulario independiente de `form1`.

**Aceptación:** importación confirma mientras otra rama está seleccionada, actualización de rama contraída, lote histórico, error de inserción visual y validación de Excel sin bloqueo del gestor.

### D14 Estados de carga y recuperación

**Pregunta:** ¿se expresa selección pendiente, contenido confirmado, vacío y error con salida clara?

- **A, recomendada:** estados explícitos, correlación de respuestas y recuperación por consulta; sin reenvíos automáticos de operaciones destructivas.
- **B:** conservar los mensajes actuales, con corrección limitada de esperas que no finalizan.

Al fallar una lectura puede mantenerse el contenido anterior solo si se identifica como tal; no debe parecer que pertenece al nuevo nodo. Después de una mutación incierta se consulta su resultado/estado antes de ofrecer repetición. El contrato y las capacidades reales de recuperación de cada operación se inventarían antes de prometerlas.

**Aceptación:** error HTTP, sesión vencida, tiempo agotado, respuesta inválida, respuesta tardía, doble activación y fallo visual posterior a éxito del servidor.

### D15 Persistencia de preferencias

**Pregunta:** ¿qué recuerda el explorador y durante cuánto tiempo?

- **A, recomendada para la primera integración:** conservar filtros y expansión durante el uso de la página; no almacenar destinos autorizados ni portapapeles de escritura como preferencias persistentes.
- **B:** recordar preferencias entre recargas mediante almacenamiento de sesión, con ámbito de usuario/empresa y limpieza al cambiar de sesión.
- **C:** preferencias persistidas en servidor, como capacidad adicional.

La maqueta restablece sus datos al recargar. Un filtro almacenado no puede convertirse en prueba de permiso ni hacer reaparecer un nivel oculto. La selección recuperada necesita nueva resolución antes de una acción.

**Aceptación:** recarga, cambio de sesión, usuario/empresa distintos y preferencia que referencia una rama ya inexistente.

### D16 Rendimiento y contadores

**Pregunta:** ¿se limita inicialmente la mejora a la estructura cargada y a sus contadores explícitos?

- **A, recomendada:** decoración incremental, conteos de nodos disponibles o hijos directos con etiquetas claras; medir volumen antes de decidir otra estrategia.
- **B:** incorporar consulta global de totales, carga diferida y/o virtualización en el mismo cambio, aumentando el alcance.

No se consultan documentos por cada expediente solo para dibujar una insignia. Los números de documentos de la maqueta son sintéticos. Los totales almacenados, los nodos cargados y las coincidencias filtradas son magnitudes distintas. Si se detecta discrepancia, se debe corregir o reconciliar con una fuente definida.

**Aceptación:** registrar cantidad de nodos, profundidad, tiempos de carga/búsqueda y coste de redecoración con volumen representativo autorizado. Fijar umbrales contra esa línea base antes de cerrar el prompt de rendimiento.

### D17 Accesibilidad y adaptación

**Pregunta:** ¿se exige una interacción utilizable sin clic derecho y sin depender del color?

- **A, recomendada:** etiquetas textuales, foco visible independiente, flechas/Enter/Escape, apertura de menú por teclado y acceso táctil; revisión en móvil y zoom.
- **B:** limitar la primera entrega a la presentación de escritorio y programar accesibilidad como entrega dependiente explícita.

No basta con añadir roles ARIA: deben corresponder al comportamiento implementado sobre el DOM WebForms. Los nombres largos conservarán acceso a su texto completo. El layout debe coordinarse con los cálculos de `auto_zise` y evitar controles fuera del área visible.

**Aceptación:** recorrido de teclado, retorno del foco, lector de pantalla acordado, ancho reducido y zoom del navegador. Las capturas o el zoom CSS de la maqueta no reemplazan estas comprobaciones.

### D18 Entregas y preparación de prompts

**Pregunta:** ¿se implementa por entregas verificables a partir de las decisiones aprobadas?

- **A, recomendada:** primero identidad visual y compatibilidad; después coordinación/acciones; luego contenido y filtros; cierre con aceptación integral y despliegue controlado.
- **B:** integrar todas las mejoras en una única entrega con validación al final.

En A, una entrega visual puede avanzar con su conjunto de decisiones cerrado sin fingir que resolvió los ajustes de backend posteriores. La generación de un prompt requiere resolver sus dependencias; ejecutarlo requiere la autorización de implementación correspondiente. Las aprobaciones de diseño no autorizan E2E real, carga ni despliegue.

**Aceptación:** cada entrega tiene archivos afectados, decisiones, criterios, pruebas, límites, mecanismo de reversión de su presentación y evidencia de compatibilidad. No se inventan grupos de usuarios ni flags de activación como solución implícita.

## Contrato arquitectónico propuesto

Este contrato orienta D08 a D14; todavía no fija nombres de endpoints, clases ni DTO definitivos.

```text
Usuario indica un elemento
  → El cliente resuelve un único nodo y su identidad
  → Se presenta la selección solicitada
  → El servidor valida identidad, autoridad y contexto de la operación
  → Se confirma la selección o se informa su rechazo
  → La acción conserva su destino explícito y sus reglas vigentes
  → La respuesta se aplica solo al contexto al que corresponde
  → El árbol y el contenido reflejan el resultado confirmado
```

La decoración y la búsqueda local no escriben dominio ni auditoría documental. Una consulta nueva de contexto o capacidades debe definirse como lectura; seleccionar un elemento no debe convertirse en editar el expediente. Mantener variables de sesión de interfaz no equivale a modificar el dominio documental.

El estado de una acción debe distinguir validación rechazada, éxito de dominio, resultado incierto y fallo al reflejar el resultado en pantalla. Esta separación evita repetir una escritura exitosa porque falló la actualización visual.

La compatibilidad comprende tanto el acceso nuevo como los consumidores actuales que permanecen activos. No se declara resuelto el destino explícito dejando una acción relacionada con dependencia implícita de sesión fuera del inventario.

## Matriz de operaciones que debe conservarse

El inventario detallado y los handlers se mantienen en el [diagnóstico](diagnostico.html#operaciones). Esta matriz asigna sus familias a decisiones y a la futura preparación de prompts.

| Familia | Operaciones incluidas | Decisiones principales |
|---|---|---|
| Navegación | Selección, expansión, búsqueda, filtros, ruta, limpiar selección | D01, D04 a D06, D08, D10, D14 |
| Organización | Crear/editar niveles y expedientes; trasladar; eliminar | D07 a D12 |
| Visibilidad | Ocultar, listar ocultos, restaurar | D09, D12, D15 |
| Acceso | Compartir nivel, consultar/administrar permisos y distinguir propietario/receptor | D03, D07 a D09 |
| Archivo | Editar expediente completo; archivar, desarchivar y ubicación física | D07 a D10, D14 |
| Presentación documental | Configurar/descargar/imprimir rótulos; consultar/cambiar estado; crear/mostrar índice | D07 a D10, D14 |
| Importación | Carpetas, consulta/recuperación de lotes y apertura del expediente confirmado | D08, D10, D13, D14 |
| Herramientas | Excel, perfiles, fuentes, proveedores y validación de formularios | D07, D13 |
| Archivos | Cargar, digitalizar, copiar/pegar, visualizar, descargar, renombrar, eliminar, radicar y metadatos | D07 a D10, D13, D14 |
| Colaboración | Compartir archivos, firma y correo | D07 a D10, D14 |
| Operaciones múltiples | Exportar listado, descargar selección y actualizar índices | D07 a D10, D14 |

## Evidencia necesaria antes de la aceptación

| Caso | Resultado esperado | Decisiones que verifica |
|---|---|---|
| Nivel propio, compartido, recibido y vacío | Tipo correcto, estado separado y reglas de acceso conservadas | D02, D03, D09 |
| Mismo alias en dos nodos y referencias inválidas | Identidad completa, rechazo de ambigüedad y rango consistente | D02, D10 |
| Clic en texto, icono, etiqueta y expansor | Una activación por selección; expansión independiente | D04 |
| Menú sobre B cuando A estaba seleccionado | Acción dirigida a B con autoridad confirmada | D07, D08 |
| Selección A/B rápida y respuestas tardías | Encabezado, contenido y operación corresponden a la misma identidad | D08, D14 |
| Dos pestañas y permiso revocado | Sin escritura sobre destino anterior ni confianza en capacidades antiguas | D08, D09 |
| Limpiar y ejecutar otra acción | Sin selección residual utilizada silenciosamente | D08, D14 |
| Nivel con contenido mixto y expediente vacío | Panel correspondiente al tipo, sin agregación documental implícita | D05 |
| Búsqueda, filtros, ancestros y rama oculta | Ámbito claro, ruta conservada y sin revelar contenido excluido | D06, D15 |
| Crear, editar y añadir primer hijo | Nodos decorados, nombres canónicos e índices de búsqueda coherentes | D01, D02, D07 |
| Cortar y pegar con éxito, rechazo y resultado incierto | Reglas de origen/destino y nueva relación; sin repetición automática | D08 a D11, D14 |
| Eliminar cancelado/rechazado y ocultar/restaurar | Semánticas distintas y actualización solo tras éxito | D12 |
| Importación en otra rama, duplicados y lote histórico | Destino del lote estable y navegación exacta | D10, D13 |
| Excel oculto o incompleto | Navegación y envíos del gestor sin validación cruzada | D13 |
| Archivar, rótulos, estado, índice y archivos | Handler y expediente correcto; selección múltiple conservada | D07 a D10 |
| UpdatePanels y mutaciones repetidas | Sin duplicación de listeners, etiquetas, menús o solicitudes | D01, D04, D13 |
| Red fallida, sesión vencida y error después de escritura | Estado comprensible y recuperación sin duplicar operaciones | D14 |
| Teclado, táctil, móvil y zoom real | Acceso a acciones y foco visible/restaurado | D17 |
| Volumen representativo | Métricas acordadas, sin consultas por decoración ni conteos engañosos | D16 |

La ejecución autenticada y las mediciones de carga necesitan ambiente y cuentas expresamente autorizados conforme a [AGENT-RUNBOOK](../../../tools/e2e/AGENT-RUNBOOK.md). Los datos de ejecución, credenciales y secretos conservan sus ubicaciones locales excluidas de Git.

## Dudas que dependen de producto o del ambiente

Estas preguntas quedan delimitadas; no se pueden cerrar por preferencias del arquitecto o por inferencia de la maqueta.

| Duda | Resolución técnica disponible | Dato o decisión que falta | Cierre |
|---|---|---|---|
| Sustituir el árbol o conservarlo | Ambos son posibles, con costes de integración distintos | Elección del usuario | D01 |
| Nombre del agrupador | Dos alternativas equivalentes técnicamente | Preferencia de vocabulario | D03 |
| Resumen de nivel o vista documental anterior | Los hijos directos pueden representarse sin consulta recursiva | Elección de experiencia | D05 |
| Buscar elementos no cargados | Requiere contrato de búsqueda del servidor | Alcance local/global | D06 |
| Reglas nuevas para mover/compartir | Deben tratarse como cambio de negocio | Si se quieren ampliar reglas; recomendación conservarlas | D09, D11 |
| Número máximo de nodos y profundidad | Se puede medir sin sustituir previamente el árbol | Muestra y autorización del ambiente | D16 |
| Tiempo de respuesta aceptable | Debe fijarse frente a línea base y volumen real | Umbrales de aceptación | D16 |
| Relación única por expediente en los datos | El código revisado no acredita unicidad global | Validación de modelo/metadatos y, si se autoriza, SELECT de control | D10 |
| Recordar filtros entre recargas | Es opcional y requiere ámbito de usuario/empresa | Duración elegida | D15 |
| Navegadores y tecnología asistiva objetivo | La maqueta local no define el soporte contractual del producto | Matriz de soporte | D17 |
| Ventana de validación y despliegue | Se puede preparar por entrega | Ambiente, cuentas, responsable de aceptación y ventana | D18 |

Cada duda nueva se añade con fecha, efecto sobre las decisiones y evidencia necesaria. Si cambia el alcance, se revisa la estimación anterior de 10 a 15 días hábiles para un desarrollador dedicado; esa estimación no es un compromiso de plazo aprobado ni cubre automáticamente una migración completa del backend.

## Preparación posterior de prompts

Los siguientes son paquetes propuestos, no prompts ejecutables ni tareas autorizadas. Su orden permite revisar cada contrato antes de implementar el siguiente.

| Paquete | Objetivo del futuro prompt | Condiciones previas |
|---|---|---|
| PB00 | Congelar inventario de acciones, reglas, firmas y pruebas de compatibilidad del árbol | D01 y alcance de D18; registrar decisiones excluidas |
| PB01 | Definir e implementar resolución/validación de identidad y contexto de selección para las rutas aprobadas | D08, D09, D10, D14 |
| PB02 | Adaptar ejecución de acciones y cortar/pegar al destino explícito; conservar reglas y transacción | PB01; D07, D11, D12 |
| PB03 | Aportar lectura acotada de capacidades o contenido de nivel solo si se necesita y aprueba | D05, D06, D09, D16; justificar cada nueva consulta |
| PB04 | Integrar compatibilidad con importación, archivos y herramientas existentes | PB01/PB02 según rutas afectadas; D13 |
| PB05 | Verificar contratos, concurrencia, rechazos, regresión y compatibilidad de despliegue | Paquetes implementados y criterios de D18 |
| PF01 | Integrar decorador visual del árbol y encabezado | D01 a D04; puede prepararse sin activar acciones nuevas |
| PF02 | Integrar acciones, filtros, contenido contextual y recuperación | D05 a D17 aplicables y contratos backend disponibles |

La solicitud de prompts de backend no convierte un cambio exclusivamente visual en trabajo de servidor. Si una decisión aprobada se resuelve solo con CSS/JavaScript, se documenta así y no se inventa una tabla o endpoint. La exploración conserva también PF01/PF02 porque completar la experiencia requiere integración de cliente.

Cada prompt posterior debe contener:

1. Objetivo concreto, decisiones aprobadas y revisión de cada una.
2. Alcance incluido, exclusiones y dependencias implementadas/verificadas.
3. Archivos, funciones y consumidores actuales que se modifican.
4. Contratos de entrada/salida, identidad, rangos y errores; firmas definitivas solo después de inventariarlas.
5. Autoridad de usuario/empresa y permisos de cada operación; protección de solicitudes según el mecanismo acordado.
6. Comportamiento de sesión, correlación de respuestas, concurrencia y casos de resultado incierto.
7. Compatibilidad con la DLL/página y decisiones de despliegue; esquema de datos únicamente si una necesidad aprobada lo exige.
8. Criterios observables de aceptación y verificaciones locales pertinentes.
9. Pruebas de integración autorizadas, evidencia esperada y condiciones que impiden afirmar aceptación.
10. Estrategia de reversión de la entrega sin descartar cambios locales ni revertir escrituras de dominio por una corrección visual.

Un paquete puede cerrarse y generar su prompt cuando sus propias decisiones y dependencias estén resueltas. Las decisiones diferidas se excluyen expresamente; no se rellenan con valores predeterminados que aparenten aprobación. La implementación se solicita después y no se inicia al aprobar una preferencia visual.

## Historial de revisión y aprobaciones

| Fecha | Decisión o versión | Cambio registrado | Autor de la aprobación |
|---|---|---|---|
| 2026-11-25 | Versión 0.1 | Creación de exploración, respuestas T01 a T20, decisiones D01 a D18 y mapa de paquetes posteriores | No aplica a una aprobación funcional |
| 2026-11-25 | D01 revisión 1 | Primera decisión presentada para revisión individual; conserva estado PENDIENTE hasta respuesta explícita | Pendiente |

**Siguiente paso de revisión:** resolver D01. Después, revisar D02 y D10 para fijar la identidad visual y técnica antes de cerrar las acciones y su integración de backend.
