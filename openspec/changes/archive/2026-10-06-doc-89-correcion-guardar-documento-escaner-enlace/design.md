<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07 -->
# Design - DOC-89 correccion del progreso al guardar desde escaner

## Contexto

El guardado desde el iframe de digitalizacion no recarga la pagina. `activa_document_save()` pulsa el boton oculto `ButtonAlmacenar` de `Webworkflow.aspx`; Microsoft AJAX inicia un async postback y `ButtonAlmacenar_Click` persiste mediante `ClassAlmacenamiento.UploadSaveFileScan`. El falso repintado aparece antes de esa persistencia: `InitializeRequest` llama `posicion_update_pogres_modal`, que combina la geometria completa de `.overlay_` con el fondo blanco de `.ctw-loading-indicator` sobre `#progres_bar`.

La frontera mas segura es la pagina padre. El escaner, sus callbacks, el servicio de carga, el code-behind y la proyeccion incremental ya completan correctamente la operacion y son compartidos por otros modulos.

## Objetivos

- Mantener el async postback y una unica persistencia.
- Mostrar progreso compacto sin ocultar el formulario de Enlace.
- Preservar `Button_guardar_desicion` y todos los consumidores del escaner.
- Dejar pruebas estructurales, funcionales y E2E reutilizables.

## No objetivos

- Corregir callbacks Dynamsoft, el codigo `-2003` o el typo `heigth`.
- Cambiar almacenamiento, formatos PDF/PDF-A/TIF, sesion o temporales.
- Recargar la pagina/iframe, reconstruir el arbol o introducir una segunda escritura.
- Cambiar globalmente `.overlay_`, `.ctw-loading-indicator` o `posicion_update_pogres_modal`.

## Decisiones

### D-01 - Rama visual especifica para ButtonAlmacenar

`InitializeRequest` distingue tres recorridos:

1. `ButtonAlmacenar`: muestra el `#progres_bar` existente como indicador compacto y centrado, sin invocar `posicion_update_pogres_modal` y sin agregar `overlay_`.
2. `Button_guardar_desicion`: conserva `posicion_update_pogres_modal('progres_bar')`.
3. Demas postbacks: conservan `posicion_update_pogres('progres_bar')`.

La implementacion puede encapsular mostrar/limpiar el indicador compacto en funciones locales de `Webworkflow.aspx` si eso hace simetrica la limpieza, pero no debe crear una utilidad global ni cambiar firmas compartidas.

### D-02 - Compatibilidad de Button_guardar_desicion

La rama legacy permanece textual y funcionalmente equivalente: aplica el overlay modal al iniciar y lo retira al terminar. Las pruebas fijan esta invariancia para impedir que la correccion de escaner cambie cargas tradicionales.

### D-03 - Frontera de persistencia inalterada

No se editan `WebFormEscan.aspx`, `WebFormEscan.js`, `online_demo_operation.js`, `Webform_save_digital_image.aspx*`, `ClassAlmacenamiento.vb` ni `ButtonAlmacenar_Click`. Tampoco se cambia el contrato `Hidden_result_load_ -> Hidden_date_row_ -> insert_row_documento_relacionado`.

### D-04 - Ciclo de vida simetrico

La funcion de progreso compacto debe ser idempotente y su limpieza debe ocurrir en `endRequest` para exito y error. Debe restaurar exclusivamente las propiedades/clases que ella haya introducido, ocultar `#progres_bar` y no dejar bloqueo de puntero, dimensiones completas ni posicion `(0,0)` residual.

### D-05 - Experiencia moderna unica

Se reutiliza la semantica accesible ya emitida (`role=status`, `aria-live=polite`) sin gates adicionales. No se reintroduce `WorkflowCentroTrabajoModernActive`, no se segmenta por usuarios/grupos y no se crea un camino legacy paralelo.

### D-06 - Caracterizacion de la frontera compartida

Una prueba CJS focal inspecciona la pagina y contratos relacionados. Debe demostrar al menos:

- `ButtonAlmacenar` permanece en el UpdatePanel y no es `PostBackTrigger`.
- Su rama no agrega `overlay_` ni llama el modal de pantalla completa.
- `Button_guardar_desicion` mantiene su recorrido.
- `ButtonAlmacenar_Click` contiene una sola llamada a `UploadSaveFileScan`.
- El exito consume una vez `Hidden_result_load_` y proyecta una vez el nodo.
- Las ramas `Hidden21` 1 a 5 y los callbacks de transporte no fueron modificados.

### D-07 - Validacion integral y reversa

Se reutiliza `tools/e2e` para una prueba de Enlace que mida navegacion, async postback, visibilidad, cantidad de escrituras/nodos y limpieza final. La corrida real requiere autorizacion explicita del ambiente, cuenta, tarea/expediente y archivo descartable. La reversa consiste en retirar solo la rama compacta y restaurar la condicion anterior; no requiere migracion de datos.

## Alternativas evaluadas

| Alternativa | Resultado | Decision |
| --- | --- | --- |
| Indicador compacto especifico para `ButtonAlmacenar` | Un cambio local, conserva almacenamiento y consumidores compartidos. | Seleccionada. |
| Overlay translucido con tarjeta interna | Requiere separar DOM/CSS y aumenta superficie visual compartida. | Descartada para DOC-89. |
| Overlay acotado al modal Enlace | Exige resolver limites y ciclo de vida del modal/iframe. | Descartada por mayor acoplamiento. |
| Cambiar globalmente `.overlay_` o `.ctw-loading-indicator` | Puede alterar numerosos postbacks y modulos. | Prohibida. |

## Flujo resultante

```text
WebFormEscan.activa_document_save (Hidden21=1)
  -> window.parent.ButtonAlmacenar.click()
  -> Webworkflow.InitializeRequest
       -> muestra #progres_bar compacto, sin overlay_
  -> async postback UpdatePanel_boton_tool
  -> Webworkflow.ButtonAlmacenar_Click
  -> ClassAlmacenamiento.UploadSaveFileScan (una vez)
       -> error: mensaje existente
       -> YES: Hidden_result_load_ + Hidden_date_row_
  -> Webworkflow.CheckStatus/endRequest
       -> oculta y limpia progreso
       -> si YES: insert_row_documento_relacionado (una vez)
  -> interfaz, visor y seleccion permanecen en contexto
```

## Riesgos y mitigaciones

- **Doble registro de `endRequest`:** caracterizar el registro actual y hacer idempotente la limpieza; no agregar handlers repetidos.
- **Estilos inline residuales del overlay:** la rama compacta no llama la funcion modal y la limpieza focal restaura los valores introducidos.
- **Regresion en guardado tradicional:** prueba textual/DOM fija la rama separada de `Button_guardar_desicion`.
- **Cambio accidental en escaner compartido:** gate de paths y diff; cualquier edicion de la frontera excluida detiene el cambio.
- **Doble clic:** no resolver con retries ni segunda escritura; validar que la infraestructura actual produce una sola operacion efectiva antes de cerrar.

## Plan de implementacion y reversa

1. Agregar primero la caracterizacion focal en `tools/e2e/tests`.
2. Separar la rama visual de `ButtonAlmacenar` en `workflow/Webworkflow.aspx`.
3. Ejecutar pruebas focales y regresion existente; inspeccionar diff de paths excluidos.
4. Compilar la solucion y ejecutar QA/E2E autorizada.
5. Para revertir, retirar la funcion/rama compacta y restaurar la condicion previa en `InitializeRequest`/`CheckStatus`; no tocar datos ni almacenamiento.

## Preguntas abiertas

No hay preguntas de diseño pendientes. Los datos concretos de ambiente y recursos descartables se solicitan solo al llegar a la corrida E2E real.
