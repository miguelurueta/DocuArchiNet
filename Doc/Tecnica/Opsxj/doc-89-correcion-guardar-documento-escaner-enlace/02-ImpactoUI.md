# DOC-89 - Impacto UI

- Ticket: DOC-89
- Cambio OpenSpec: doc-89-correcion-guardar-documento-escaner-enlace
- Clasificacion: cross_cutting

## Superficies UI

Solo cambia el script inline de `workflow/Webworkflow.aspx` asociado al ciclo Microsoft AJAX de `ButtonAlmacenar`. Se conserva el nodo existente `#progres_bar`, incluido `role="status"`, `aria-live="polite"` y la clase moderna oficial.

## Flujo visual resultante

```text
Aceptar en WebFormEscan
  -> carga Dynamsoft
  -> ButtonAlmacenar en la página padre
  -> InitializeRequest muestra tarjeta compacta sin overlay_
  -> async postback y almacenamiento
  -> CheckStatus proyecta un nodo si Hidden_result_load_ = YES
  -> finally oculta/restaura el indicador
```

Durante el almacenamiento permanecen visibles el modal Enlace, el árbol y el visor. `Button_guardar_desicion` conserva el overlay modal anterior. Los demás postbacks conservan `posicion_update_pogres`.

## Validacion visual

La E2E registrada abre un navegador visible y exige un doble clic manual sobre `Aceptar`. Mide automáticamente que el indicador sea visible y compacto, no tenga `.overlay_`, no ocupe más del 25 % del viewport y quede oculto al finalizar. La ejecución espera ambiente, cuenta y tarea descartable expresamente autorizados.
