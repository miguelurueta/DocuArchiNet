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

## Validacion del bloque Asistente IA flotante

Comando ejecutado despues de incorporar y ajustar el asistente:

```powershell
npx.cmd tsc --noEmit --pretty false
```

Resultado:

- OK.
- Sin errores TypeScript en:
  - `src/modules/gestionCorrespondencia/pages/GestionRespuesta.tsx`
  - `src/modules/gestionCorrespondencia/style/GestionRespuesta.module.css`
  - `src/app/Components/UI/AppEditor/AppEditor.module.css`

Validaciones tecnicas cubiertas por TypeScript:

- `useRef<HTMLInputElement | null>` compatible con el input final.
- `KeyboardEvent<HTMLElement>` importado como type-only.
- `handleAssistantKeyDownCapture` tipado y usado por el panel.
- Uso valido de iconos Ant Design:
  - `RobotOutlined`
  - `CloseOutlined`
  - `SendOutlined`
- Props ARIA y eventos React validos en:
  - `section`
  - `input`
  - `button`
  - `form`

### OpenSpec strict

Comando ejecutado:

```powershell
npx.cmd openspec validate scrumcore-251-tabs-workbench-gestion-correspondencia --strict
```

Resultado funcional:

- OK.
- Salida principal: `Change 'scrumcore-251-tabs-workbench-gestion-correspondencia' is valid`.

Nota operativa:

- La CLI intento hacer flush de telemetria PostHog y reporto `PostHogFetchNetworkError` por red restringida/EACCES.
- Ese error corresponde a telemetria externa, no a la validacion del cambio.
- El comando finalizo con exit code 0 y el cambio OpenSpec quedo valido.

### Whitespace

Comando ejecutado despues de documentar el asistente:

```powershell
git diff --check
```

Resultado:

- OK.
- Sin errores de whitespace.
- Git reporto avisos LF/CRLF por configuracion local en archivos del workspace.

## Validacion del grip de redimensionamiento

Comandos ejecutados despues de ajustar el handle visual:

```powershell
npx.cmd tsc --noEmit --pretty false
npx.cmd vitest run src/modules/gestionCorrespondencia/components/workbenchParallelTabs/GestionWorkbenchParallelTabs.test.tsx
```

Resultado:

- TypeScript OK.
- Test de `GestionWorkbenchParallelTabs` OK.
- 1 archivo de test ejecutado.
- 1 test OK.

Cobertura del test existente:

- El layout paralelo renderiza ambos paneles.
- El separador conserva `aria-label="Redimensionar paneles"`.
- El ajuste visual no elimina el handle accesible.

QA manual esperada:

- El contenedor del separador no debe verse como barra gris.
- El grip interno debe verse como puntos verticales.
- Al pasar mouse por el separador debe aparecer feedback azul sutil.
- Al enfocar por teclado debe existir feedback visual discreto.
- El tooltip nativo debe mostrar `Arrastra para redimensionar`.
- El cursor debe mantenerse como `col-resize`.
- El usuario debe poder seguir redimensionando los paneles.

## QA manual esperada del Asistente IA

1. Boton flotante
   - Debe verse abajo a la derecha del workbench.
   - Debe estar visible en tab `Gestion`.
   - Debe estar visible en tab `Documentos`.
   - Debe mostrar icono de robot y texto `IA` cuando esta cerrado.
   - Debe mostrar una `X` visible cuando el chat esta abierto.
   - No debe tapar el indicador de palabras/caracteres del AppEditor.

2. Apertura/cierre
   - Click en `IA` abre el panel.
   - El panel debe animarse como si saliera del boton.
   - Click en la X del FAB debe cerrar el panel.
   - Click en la X del header debe cerrar el panel.
   - El cierre debe animarse hacia el boton antes de desmontar.
   - La animacion debe sentirse enterprise: sobria, sin rebote exagerado.

3. Input normal
   - El campo debe ser un input de una linea.
   - Debe permitir escribir varios caracteres seguidos sin perder foco.
   - El AppEditor no debe capturar el foco mientras se escribe.
   - `Enter` debe enviar el mensaje.
   - Mensajes vacios no deben enviarse.
   - Despues de enviar, el input debe limpiarse.
   - Despues de enviar, el foco debe permanecer en el input.

4. Boton limpiar
   - Al escribir texto debe aparecer la X interna del input.
   - Click en la X interna debe limpiar el texto.
   - Click en la X interna debe conservar foco en el input.
   - La X interna no debe cerrar el chat.

5. Mensajes
   - El mensaje del usuario debe agregarse al log.
   - La respuesta placeholder debe agregarse despues del mensaje del usuario.
   - Los mensajes deben tener animacion de entrada discreta.
   - `role="log"` y `aria-live="polite"` deben permanecer en el contenedor.

6. Responsive
   - El chat debe verse compacto en desktop.
   - El chat debe verse menos ancho que la primera version.
   - En mobile no debe exceder el viewport.
   - El input y botones deben permanecer accionables.

7. No regresiones esperadas
   - La navegacion entre tabs debe continuar funcionando.
   - La vista paralela debe continuar funcionando.
   - El editor debe seguir editable.
   - El indicador de caracteres debe seguir visible, ahora desplazado a la izquierda.
   - DocumentosWorkbench y AppVisorEmbedPdf no deben cambiar comportamiento.

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
- Confirmar que el boton IA no bloquea acciones de editor, adjuntos ni documentos.
- Confirmar que el chat no invoca backend ni genera errores de red.

## Build

No se ejecuto build completo en este bloque final. La validacion principal fue TypeScript. La metadata previa del ticket conserva el antecedente de deuda preexistente en build general relacionada con `DynamsoftTwainClient.ts`.

## Riesgos residuales de QA

- Validar en navegador real si `dvh` y DevTools mobile emulation difieren.
- Revisar que el selector `:has()` aplique en los navegadores soportados por el producto.
- Confirmar que los breakpoints no se solapan en dispositivos no listados.
- Confirmar que los nombres largos de adjuntos siguen siendo comprensibles mediante `title`.
- Confirmar visualmente que el nuevo z-index del asistente no cubre modales o popovers criticos.
- Confirmar que el input del asistente no pierde foco en navegadores reales con el AppEditor montado.
