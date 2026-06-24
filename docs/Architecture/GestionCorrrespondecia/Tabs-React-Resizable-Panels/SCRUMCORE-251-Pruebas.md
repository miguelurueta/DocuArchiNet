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
   - Debe mostrar el callout superior `¿Te ayudo con la respuesta?` cuando esta cerrado.
   - El callout debe desaparecer cuando el chat esta abierto.
   - El callout no debe mostrar badge interno `IA`.
   - No debe tapar el indicador de palabras/caracteres del AppEditor.

2. Apertura/cierre
   - Click en `IA` abre el panel.
   - El panel debe animarse como si saliera del boton.
   - Click en la X del FAB debe cerrar el panel.
   - Click en la X del header debe cerrar el panel.
   - El cierre debe animarse hacia el boton antes de desmontar.
   - La animacion debe sentirse enterprise: sobria, sin rebote exagerado.
   - En mobile, al cerrar el chat, el boton `IA` debe seguir visible.
   - En mobile, el teclado virtual no debe dejar oculto el boton despues de cerrar.
   - Taps rapidos abrir/cerrar no deben dejar el panel en estado intermedio.

3. Hover, foco y animacion de atencion
   - Hover/focus/active sobre el boton debe resaltar tambien el callout.
   - Hover/focus/active sobre el callout debe resaltar tambien el boton.
   - El boton y el callout deben ejecutar una animacion periodica visible.
   - La animacion debe consistir en elevacion/halo sobrio, no vibracion.
   - La animacion debe pausarse durante hover/focus.

4. Input normal
   - El campo debe ser un input de una linea.
   - Debe permitir escribir varios caracteres seguidos sin perder foco.
   - El AppEditor no debe capturar el foco mientras se escribe.
   - `Enter` debe enviar el mensaje.
   - Mensajes vacios no deben enviarse.
   - Despues de enviar, el input debe limpiarse.
   - Despues de enviar, el foco debe permanecer en el input.

5. Boton limpiar
   - Al escribir texto debe aparecer la X interna del input.
   - Click en la X interna debe limpiar el texto.
   - Click en la X interna debe conservar foco en el input.
   - La X interna no debe cerrar el chat.

6. Sugerencias demo
   - Debe existir bloque `Sugerencias` debajo del historial de mensajes.
   - Cada sugerencia debe verse como chip compacto.
   - Click en una sugerencia debe cargar texto en el input.
   - Click en una sugerencia no debe enviar automaticamente el mensaje.
   - El usuario debe poder editar la sugerencia antes de enviar.

7. Mensajes
   - El mensaje del usuario debe agregarse al log.
   - La respuesta placeholder debe agregarse despues del mensaje del usuario.
   - Los mensajes deben tener animacion de entrada discreta.
   - `role="log"` y `aria-live="polite"` deben permanecer en el contenedor.

8. Responsive
   - El chat debe verse compacto en desktop.
   - El chat debe verse menos ancho que la primera version.
   - En mobile no debe exceder el viewport.
   - El input y botones deben permanecer accionables.
   - En mobile muy pequeno las sugerencias deben apilarse sin romper el layout.

9. No regresiones esperadas
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

## QA manual esperada de flotantes AppEditor

1. Indicador de pagina
   - Debe permanecer centrado abajo.
   - Debe conservar botones de navegacion y input de pagina.
   - Debe quedar un poco mas abajo que la ubicacion previa del estilo base.

2. Indicador de palabras/caracteres
   - Debe quedar centrado encima del indicador de pagina.
   - Debe ser un poco menos alto que la version previa.
   - No debe solaparse con el FAB de IA.
   - No debe alterar conteo de palabras o caracteres.

3. Mobile
   - El media query mobile existente debe conservar su comportamiento.
   - La reubicacion base no debe romper el layout en mobile.

## Validacion 2026-06-20

Comando ejecutado durante las iteraciones finales:

```powershell
npx.cmd tsc --noEmit --pretty false
```

Resultado:

- OK.
- Se ejecuto despues de:
  - mover callout arriba del FAB.
  - redisenar callout.
  - quitar badge interno `IA`.
  - apilar chat sobre boton.
  - cambiar texto a `¿Te ayudo con la respuesta?`.
  - agregar sugerencias demo.
  - sincronizar hover/focus entre callout y FAB.
  - ajustar chat mobile.
  - agregar animacion periodica enterprise.
  - corregir cierre mobile del FAB.
  - apilar flotantes del AppEditor.
  - compactar flotante de palabras/caracteres.

## Build

Comando ejecutado durante la validacion del estado actual:

```powershell
npm.cmd run build
```

Resultado:

- Fallido por error TypeScript externo a SCRUMCORE-251.
- Archivo: `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts`.
- Linea reportada: `908`.
- Error: `TS2552: Cannot find name 'DynamsoftWebTwainFactory'. Did you mean 'DynamsoftWebTwainObject'?`

Decision:

- No se modifica `DynamsoftTwainClient.ts` dentro de SCRUMCORE-251 para no afectar componentes de Digitalizacion que ya funcionan o pertenecen a otro alcance.
- El build global queda como bloqueo externo para cierre tecnico completo.
- La evidencia propia del Scrum queda cubierta por TypeScript, OpenSpec strict y tests focalizados de Gestion.

## Validacion 2026-06-22 - cierre de tareas automatizables

Comando ejecutado:

```powershell
npx.cmd vitest run src/modules/gestionCorrespondencia/components/workbenchParallelTabs/GestionWorkbenchParallelTabs.test.tsx src/modules/gestionCorrespondencia/pages/GestionRespuesta.test.tsx
```

Resultado:

- 2 archivos OK.
- 6 tests OK.

Cobertura agregada:

- `GestionRespuesta.test.tsx` actualiza el mock de `@ant-design/icons` para incluir `RobotOutlined`, `CloseOutlined` y `SendOutlined`, evitando falsos negativos por el asistente IA.
- Se valida que el documento seleccionado simulado permanece visible al alternar `tabs -> parallel -> tabs -> parallel`.
- Se valida que la instancia visible de `DocumentosWorkbench` no se duplica al alternar modo.
- Se mantiene la cobertura previa del modo normal por defecto, activacion/desactivacion de vista paralela, `aria-pressed`, `aria-checked` y fallback en ancho reducido.

Comandos complementarios:

```powershell
npx.cmd tsc --noEmit --pretty false
git diff --check
```

Resultado:

- TypeScript OK.
- Whitespace OK; solo avisos LF/CRLF de Git por configuracion local.

## Riesgos residuales de QA

- Validar en navegador real si `dvh` y DevTools mobile emulation difieren.
- Revisar que el selector `:has()` aplique en los navegadores soportados por el producto.
- Confirmar que los breakpoints no se solapan en dispositivos no listados.
- Confirmar que los nombres largos de adjuntos siguen siendo comprensibles mediante `title`.
- Confirmar visualmente que el nuevo z-index del asistente no cubre modales o popovers criticos.
- Confirmar que el input del asistente no pierde foco en navegadores reales con el AppEditor montado.

## Cierre QA manual confirmado por usuario

Fecha de registro: 2026-06-22.

Estado:

- El usuario confirma que el QA manual obligatorio fue ejecutado.
- Se omiten como pendientes las tareas manuales del checklist porque fueron validadas fuera del runner automatizado.
- Se actualiza `tasks.md` marcando como completados los pasos 6.1 a 6.12.

Alcance confirmado:

- Apertura de Gestion Correspondencia.
- Entrada a tramite con tabs `Gestion` y `Documentos`.
- Verificacion del modo normal.
- Activacion de `Vista paralela`.
- Visualizacion simultanea de `Gestion` y `Documentos`.
- Redimensionamiento de paneles.
- Seleccion de documento en `Documentos`.
- Retorno a `Vista normal`.
- Confirmacion de conservacion de seleccion/contexto.
- Reapertura de `Vista paralela`.
- Confirmacion de ausencia de doble carga evidente o errores de consola.
- Prueba en ancho reducido.
