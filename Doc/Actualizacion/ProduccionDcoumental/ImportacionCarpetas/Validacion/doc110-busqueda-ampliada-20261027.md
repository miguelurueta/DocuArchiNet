# DOC-110: búsqueda asistida, teclado y reapertura

Fecha: 2026-10-27. Tarea 11.3 completa, D-07/RQ-07 y COR-03. Tras el PASS autenticado, el usuario confirma instalación de empresa única permanente y declara innecesaria la prueba entre empresas: ese criterio es NO_APLICA, no una corrida PASS. [Decisión y alcance](../Exploracion/criterio-empresa-unica-doc110.md). Sin nueva escritura de datos. Las notas de pendiente inferiores conservan el historial anterior a esta decisión.

El recorrido autenticado existente conserva las búsquedas por código parcial y nombre parcial, selección por clic y limpieza. Se añade `exerciseProfileSearch`, compartido con la prueba Chromium local:

- Busca el código del perfil ya listado y localiza su sugerencia exacta.
- Usa ArrowDown/Enter para seleccionar esa sugerencia; no supone que esté en primera posición.
- Espera que el listado sustituya el campo anterior, comprueba el código seleccionado y limpia con foco restaurado.
- Consulta un término aleatorio sin coincidencias y exige el aviso de resultado vacío y menú oculto.
- Cierra por Escape y reabre; exige campo vacío y un único menú autocomplete.

Solo utiliza ListarPerfiles desde el editor; no abre detalles, crea, guarda ni publica perfiles. En el runner sin modo adicional tampoco se ejecutan ciclos de fuentes o proveedores. No guarda ni imprime el término aleatorio, credenciales o token.

## Verificación local

```powershell
node --test tests/doc110-perfil-importacion-editor.test.cjs
cd tools/e2e
npx.cmd playwright test tests/doc110-perfil-autocomplete.spec.cjs --reporter=line
```

12 pruebas Node PASS y 1 Chromium PASS (4.4 s). Chromium usa el widget productivo y transporte simulado, sin login/BD. La prueba de comportamiento existente comprueba respuestas fuera de orden y render seguro; el recorrido compartido espera la sustitución efectiva del campo antes de seguir, evitando depender de una pausa arbitraria.

## Corrida autenticada preparada

```powershell
npm.cmd --prefix tools/e2e run test:doc110:session -- --headed
```

Usa ambiente configurado y cuenta de prueba ya autorizados; solicita credenciales efímeras en la consola existente. Marcador nuevo esperado: AUTOCOMPLETE_KEYBOARD_EMPTY_RESULT_REOPEN. Si no hay perfil existente adecuado, el recorrido registra AUTOCOMPLETE_REQUIRES_EXISTING_PROFILE y no acredita la comprobación.

Estado HTTP nuevo: **FALLÓ**, resultado aportado por el usuario: caso 20.1 s. Pasó AUTOCOMPLETE_CODE_NAME_SELECTION_CLEAR; la extensión falló porque el selector genérico section[role=dialog] alcanzó también los paneles ocultos de proveedor y fuente, produciendo tres elementos role=status. No acredita resultado vacío/reapertura completos ni evidencia de escritura; el recorrido solo usa listados.

Corrección: el helper identifica el diálogo por aria-labelledby=doc110-title y el aviso por data-doc110=estado. La regresión Chromium incluye ahora los otros dos administradores ocultos bajo el mismo host y exige tres diálogos antes de ejecutar el helper. Resultado corregido local: **1 passed (3.8 s)**. No cambió código productivo. Repetición autenticada pendiente; no se convierte la corrida fallida en PASS por la prueba local.

Segunda corrida autenticada: **FALLÓ**, caso 17.3 s, después del marcador previo. El aviso ya se verificó, pero la aserción global .ui-autocomplete alcanzó tres menús. La primera corrección no había acotado todos los selectores del helper.

Corrección adicional: buscador, botón Limpiar, sugerencias y menú se resuelven dentro del diálogo de perfiles. También se acotan las sugerencias del recorrido anterior por código/nombre. La regresión local mantiene tres diálogos y cuatro menús (dos dentro de administradores ocultos y otro fuera del host), con texto de sugerencia idéntico en menús ajenos; exige que los cuatro sobrevivan al recorrido, mientras el editor de perfiles conserva su único menú. Resultado: **1 passed (3.9 s)**. Solo pruebas modificadas. Repetición HTTP pendiente; ambos fallos se conservan como tales.

Repetición autenticada corregida: **1 passed (19.1 s)**, caso 18.1 s, resultado aportado por el usuario. Pasaron AUTOCOMPLETE_CODE_NAME_SELECTION_CLEAR y AUTOCOMPLETE_KEYBOARD_EMPTY_RESULT_REOPEN. Quedan acreditados contra IIS código/nombre parciales, selección por clic y teclado, limpieza, resultado vacío y reapertura con un único menú propio. Esta corrida supera la repetición HTTP pendiente; las dos corridas fallidas anteriores se conservan como antecedentes de corrección del test, sin atribuirles PASS.

Las respuestas fuera de orden siguen acreditadas localmente con dobles. La denegación de una segunda cuenta sin facultad tampoco demuestra separación de dos ámbitos con permiso. 11.3 conserva pendiente ese criterio hasta evidencia específica. Sin DDL, cambios de permisos, fuentes, SII o consultas DA.
