# SCRUMCORE-251 - Pruebas

## Pruebas automatizadas existentes del ticket

Comando documentado previamente:

```powershell
npx.cmd vitest run src/modules/gestionCorrespondencia/components/workbenchParallelTabs/GestionWorkbenchParallelTabs.test.tsx src/modules/gestionCorrespondencia/pages/GestionRespuesta.test.tsx
```

Cobertura:

- `GestionWorkbenchParallelTabs` renderiza `Gestion` y `Documentos` simultaneamente.
- Los paneles tienen labels accesibles.
- Existe divisor accesible.
- `GestionRespuesta` inicia en modo tabs.
- El boton activa modo paralelo.
- El boton vuelve a modo tabs.
- El switch refleja estado con `aria-pressed` y `aria-checked`.
- `data-layout-state` refleja estado visual.
- En ancho reducido el boton queda deshabilitado.

## Pruebas agregadas/modificadas en el bloque actual

### TypeScript

Comando:

```powershell
npx.cmd tsc --noEmit --pretty false
```

Resultado:

- OK.

### AppUpload

Archivo:

- `src/app/Components/UI/AppUpload/AppUpload.test.tsx`

Caso:

- `mantiene visible el archivo cargado con estrategia auto`

Valida:

- Upload con `strategy="auto"`.
- Cambio de input file.
- El archivo queda visible por `title`.
- `onChange` final recibe archivo con `status: "done"`.

Riesgo cubierto:

- Regresion donde un archivo cargado desaparecia por updates async con estado obsoleto.

Comando ejecutado:

```powershell
npx.cmd vitest run src/app/Components/UI/AppUpload/AppUpload.test.tsx
```

Resultado:

- 1 archivo OK.
- 11 tests OK.

### AppToolbar

Archivo:

- `src/app/Components/UI/AppToolbar/AppToolbar.test.tsx`

Cambio:

- El test desktop valida que el section no contiene clase `compact`.

Riesgo cubierto:

- Evitar que la compactacion de toolbar para Gestion contamine desktop o toolbars globales.

Comando ejecutado:

```powershell
npx.cmd vitest run src/app/Components/UI/AppToolbar/AppToolbar.test.tsx
```

Resultado:

- 1 archivo OK.
- 4 tests OK.

## Validacion TypeScript

Comando ejecutado:

```powershell
npx.cmd tsc --noEmit --pretty false
```

Resultado:

- OK.
- Sin errores TypeScript atribuibles al bloque actual.

## Nota sobre ejecucion combinada

Se intento ejecutar AppUpload y AppToolbar en un unico comando Vitest. Esa ejecucion combinada llego a reportar AppToolbar OK, pero expiro por timeout del runner antes de cerrar todo el proceso. Se repitieron ambos archivos por separado desde el workspace real y ambos pasaron.

## Validacion de whitespace

Comando ejecutado:

```powershell
git diff --check
```

Resultado:

- OK.
- Git reporta avisos LF/CRLF por configuracion local en archivos existentes; no son errores de whitespace del diff.

## Validacion manual responsive esperada

La validacion visual debe cubrir:

1. Mobile pequeno / iPhone SE
   - El tab Documentos no queda demasiado bajo.
   - El visor PDF/estado vacio ocupa altura acorde al contenedor.
   - El rail lateral de documentos permanece accionable.

2. Samsung Galaxy S8+
   - Workbench y visor PDF usan el alto aumentado.
   - No se corta el estado vacio.

3. iPhone XR
   - El panel externo de Documentos queda ligeramente mas alto.
   - El visor mantiene altura sincronizada con el workbench.

4. iPhone 12 Pro
   - El visor no queda corto respecto al div.
   - No se genera scroll vertical innecesario por altura excesiva.

5. iPhone 14 Pro Max
   - La pantalla alta aprovecha mas espacio del visor.
   - El panel no sobrepasa visualmente el contenedor.

6. iPad Mini
   - El override final de `AppTabs` aplica y no es pisado por `max-width: 768px`.
   - `panelContent`, `workbenchBody`, `viewer` y `AppVisorEmbedPdf.root` quedan alineados.

7. Tablets grandes / iPad Air / iPad Pro
   - Toolbar compacta no deja espacio vertical excesivo.
   - Separacion entre toolbar, editor y adjuntos permanece reducida.

8. Desktop pequeno
   - Toolbar de Gestion se mantiene compacta.
   - AppEditor no ocupa altura excesiva.

## QA funcional esperada

- Cargar archivos en Adjuntos.
- Confirmar que se visualizan en cards compactas.
- Confirmar que las cards se agrupan lado a lado.
- Confirmar que solo existe accion de eliminar.
- Confirmar que eliminar remueve el archivo correcto.
- Confirmar que el upload mantiene dropzone a lo ancho.
- Confirmar que metadata de cabecera muestra tooltip completo.
- Confirmar que el boton de volver sigue accionable.
- Confirmar que no se altera la navegacion entre tabs.
- Confirmar que Documentos sigue permitiendo abrir lista lateral.

## Build

No se ejecuto build completo en este bloque final. La validacion principal fue TypeScript. La metadata previa del ticket conserva el antecedente de deuda preexistente en build general relacionada con `DynamsoftTwainClient.ts`.

## Riesgos residuales de QA

- Validar en navegador real si `dvh` y DevTools mobile emulation difieren.
- Revisar que el selector `:has()` aplique en los navegadores soportados por el producto.
- Confirmar que los breakpoints no se solapan en dispositivos no listados.
- Confirmar que los nombres largos de adjuntos siguen siendo comprensibles mediante `title`.
