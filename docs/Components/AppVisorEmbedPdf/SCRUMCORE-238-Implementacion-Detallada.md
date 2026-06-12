# SCRUMCORE-238 - AppVisorEmbedPdf: deduplicacion de carga en PDFs grandes

## 1. Resumen ejecutivo

Se corrigio un falso prompt de contrasena que aparecia al volver a abrir un PDF grande no protegido despues de firmarlo, navegar a otro documento y regresar al mismo documento.

El problema no estaba en el contrato de firma ni en el flujo de reemplazo de paginas anotadas. La evidencia de consola mostro que el PDF llegaba como `application/pdf`, sin password enviada, y que el visor intentaba abrir el mismo documento dos veces seguidas. En PDFs pequenos la condicion de carrera no era visible por la rapidez de apertura. En un PDF de aproximadamente 869 MB, la segunda apertura alcanzaba a cerrar el documento que la primera apertura estaba preparando, provocando `engine open failed` y un estado visual que terminaba asociado a prompt de documento protegido.

La solucion aplicada es enterprise y de bajo riesgo: deduplicar cargas identicas en curso dentro de `AppVisorEmbedPdf`, reutilizando la misma promesa de carga cuando llega un segundo `load()` con la misma identidad funcional.

## 2. Sintoma reportado

Flujo observado:

1. El usuario abre un PDF grande no protegido.
2. El visor lo carga correctamente.
3. El usuario pega firma.
4. El frontend genera el PDF anotado de una sola pagina, sube chunks y llama el endpoint final.
5. Backend responde exitosamente.
6. La firma queda quemada correctamente y el visor marca la firma como persistida sin recargar el PDF grande.
7. El usuario abre otro documento.
8. El usuario vuelve al PDF grande.
9. Aparece prompt de contrasena aunque el documento no tiene contrasena.

El comportamiento era intermitente y dependiente del tamano del documento. Al recargar completamente la pestana del navegador, el PDF grande podia abrir de nuevo sin prompt, lo cual descartaba que fuera un password real persistido en el archivo.

## 3. Evidencia tecnica

Logs relevantes del caso real:

```txt
[DV][attempt:8][req:8] download blob ok
{ blobSize: 869151386, blobType: 'application/pdf' }

[DV][password][open-attempt]
{ documentKey: 'CORRESPO:9927', managedSeq: 15, hasPassword: false, hasValidatedPassword: false }

[DV][visor] openDocumentUrl dispatched
{ managedSeq: 15, documentId: 'doc-...' }

[DV][visor] load() start
{ seq: 16, attemptId: 8, documentKey: 'CORRESPO:9927' }

[DV][password][open-attempt]
{ documentKey: 'CORRESPO:9927', managedSeq: 16, hasPassword: false, hasValidatedPassword: false }

[DV][visor] closeDocument before open (guard maxDocuments)
{ managedSeq: 16, documentId: 'doc-...' }

[DV][visor] engine open failed (task err)
{ managedSeq: 16 }
```

Lectura de la evidencia:

- `blobSize` era mayor a 0 y `blobType` era `application/pdf`.
- `hasPassword` era `false`.
- `hasValidatedPassword` era `false`.
- El request final de reemplazo se enviaba con `OriginalPdfPassword: false` para este PDF no protegido.
- La firma se quemaba correctamente antes del problema.
- El fallo ocurria despues, al navegar fuera y volver al PDF grande.
- Aparecian dos `load()` consecutivos para el mismo `documentKey`, mismo intento funcional y mismo PDF.
- La segunda apertura cerraba el documento abierto por la primera mediante `closeDocument before open`.
- El fallo real del engine era `OPEN_FAILED`, no una validacion real de password.

## 4. Causa raiz

La causa raiz fue una condicion de carrera entre cargas duplicadas del mismo documento en el modo managed del visor.

`DocumentosWorkbench` ya tenia un guardrail para reducir cargas repetidas, pero bajo ciertos re-renders del flujo de documento pesado podia llegar una segunda llamada imperativa a:

```ts
visorRef.current?.load(...)
```

antes de que la primera carga terminara el handshake interno con PDFium/EmbedPDF.

Antes del ajuste, `AppVisorEmbedPdf` hacia lo siguiente:

1. Recibia el primer `load()`.
2. Incrementaba `loadSeqRef`.
3. Abortaba carga previa.
4. Seteaba `managedUrl`.
5. Esperaba permisos.
6. Abria el documento con `openDocumentUrl`.
7. Quedaba esperando `task.wait()`.
8. Recibia un segundo `load()` equivalente antes de terminar.
9. Volvia a incrementar `loadSeqRef`.
10. Volvia a preparar apertura.
11. El host ejecutaba `closeDocument before open` para cumplir la politica single-active document.
12. Ese cierre podia afectar el documento que la primera apertura acababa de despachar.
13. En PDFs grandes, el engine aun no habia terminado de estabilizar el documento.
14. PDFium/EmbedPDF reportaba fallo de apertura.

En documentos pequenos la ventana de carrera es mucho menor, por eso el problema no se reproducia de forma visible.

## 5. Decision de diseno

Se eligio deduplicacion de cargas identicas en curso dentro de `AppVisorEmbedPdf`.

No se eligio:

- ocultar el prompt por tamano de archivo;
- tratar todos los `OPEN_FAILED` como no-password sin resolver la carrera;
- cambiar la logica de firma;
- recargar el PDF grande despues de firmar;
- crear reglas especiales para el documento `CORRESPO:9927`;
- depender de delays o timers artificiales;
- modificar el backend;
- modificar el contrato de reemplazo de paginas anotadas.

La deduplicacion es el punto correcto porque `AppVisorEmbedPdf` es la frontera que controla la apertura real del engine. Si dos llamadas representan el mismo documento y la misma fuente (`fileUrl`) mientras la primera sigue en curso, abrir de nuevo no aporta valor y si introduce riesgo.

## 6. Cambio implementado

Archivo:

- `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.tsx`

Se agrego:

```ts
const inFlightLoadRef = useRef<{ key: string; promise: Promise<AppVisorLoadResult> } | null>(null);
```

La logica existente de `load()` se preservo como `runLoad()`. Sobre ella se agrego un wrapper `load()` que calcula una identidad funcional:

```ts
const loadKey = [
  input.attemptId ?? "",
  input.documentKey ?? "",
  input.url,
  input.isElectronicallySigned ?? "",
].join("|");
```

Si ya existe una carga en curso con la misma identidad:

```ts
if (inFlightLoad?.key === loadKey) {
  dvLog("[DV][visor]", "load() duplicate in-flight reused", {
    attemptId: input.attemptId,
    documentKey: input.documentKey,
  });
  return inFlightLoad.promise;
}
```

Si no existe, se ejecuta `runLoad(input)` y se registra la promesa en `inFlightLoadRef`.

Al finalizar la promesa, se limpia el registro solo si la clave sigue siendo la misma:

```ts
void promise.finally(() => {
  if (inFlightLoadRef.current?.key === loadKey) {
    inFlightLoadRef.current = null;
  }
});
```

Tambien se limpia `inFlightLoadRef` en `cancelCurrentLoad()` para que un cambio real de documento o cancelacion explicita no quede bloqueado por una promesa anterior.

## 7. Invariantes preservadas

El cambio preserva:

- firma actual;
- firma persistida visualmente sin reload post-exito;
- upload temporal por chunks;
- reemplazo fisico de paginas anotadas;
- bloqueo de listas durante guardado;
- `latest-wins` entre documentos diferentes;
- cancelacion explicita;
- prompt real de password cuando el engine reporta un PDF realmente protegido;
- validacion de permisos del visor;
- politica single-active document para no acumular documentos abiertos en el engine.

El cambio solo intercepta una condicion:

- misma `attemptId`;
- mismo `documentKey`;
- misma `url`;
- mismo estado `isElectronicallySigned`;
- carga aun en curso.

Cuando cualquiera de esos valores cambia, el visor conserva el comportamiento anterior y ejecuta una carga nueva.

## 8. Por que es una solucion enterprise

La solucion es sostenible porque:

- corrige la causa raiz observada y no solo el sintoma visual;
- esta encapsulada en la frontera del visor, donde ocurre la apertura real del PDF;
- no agrega reglas por tamano, documento, gabinete o usuario;
- mantiene trazabilidad con `window.__DV_DEBUG__`;
- evita reabrir PDFium innecesariamente;
- reduce presion de memoria en PDFs grandes;
- no altera el contrato HTTP;
- no cambia el ciclo de firma ni el PDF generado;
- no usa delays fragiles;
- es compatible con navegacion normal entre documentos distintos.

## 9. Como comprobar en consola

Activar debug:

```js
window.__DV_DEBUG__ = true
```

Escenario esperado:

1. Abrir PDF grande.
2. Firmar y guardar.
3. Abrir otro documento.
4. Volver al PDF grande.

Resultado esperado:

- Si ocurre una segunda llamada identica en curso, debe aparecer:

```txt
[DV][visor] load() duplicate in-flight reused
```

- No deberia aparecer una secuencia donde el segundo `load()` cierre el documento que el primero acaba de abrir:

```txt
[DV][visor] closeDocument before open
[DV][visor] engine open failed (task err)
```

para la misma identidad funcional del documento.

- No debe aparecer prompt falso de contrasena en PDFs no protegidos.

Si aparece prompt de contrasena, revisar si existe:

```txt
[DV][password][prompt-open:onDocumentError]
[DV][password][encryption-inspection] encrypted: true
```

Si `encrypted: true`, el caso ya no es la carrera de carga: el engine o el archivo descargado esta indicando cifrado real.

## 10. Pruebas ejecutadas

Comandos ejecutados:

```powershell
npx.cmd tsc --noEmit --pretty false
npx.cmd vitest run src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx --reporter verbose
npm.cmd run build
```

Resultados:

- TypeScript: OK.
- Vitest visor: OK, 22 tests pasaron.
- Build productivo: OK.

Nota:

- El primer intento de `vitest` y `build` con timeout de 120 segundos expiro por tiempo, no por error de codigo.
- Al repetir con mayor margen, ambos comandos finalizaron correctamente.

## 11. Riesgos residuales y seguimiento

Este cambio corrige el caso observado de doble carga identica en curso. Si en el futuro aparece un prompt en PDF grande, se debe validar:

1. Si el blob descargado sigue siendo `application/pdf`.
2. Si `blobSize` es mayor a 0.
3. Si hay doble `load()` con misma identidad.
4. Si aparece `load() duplicate in-flight reused`.
5. Si `PdfErrorCode.Password` viene acompanado de `encrypted: true`.
6. Si el backend devolvio un PDF final corrupto o incompleto despues del reemplazo.
7. Si el navegador alcanza limites de memoria por el tamano del PDF.

Mientras no exista `encrypted: true`, no se debe concluir que el documento realmente requiere password.

## 12. Ajuste complementario: password validada se perdia durante firma

### 12.1 Sintoma

Despues de corregir la carrera de carga en PDFs grandes, se detecto otro caso en PDFs realmente protegidos con contrasena:

1. El visor abria el documento protegido.
2. El usuario ingresaba la contrasena.
3. PDFium/EmbedPDF validaba correctamente la contrasena:

```txt
[DV][password][retry:ok]
{ documentKey: 'CORRESPO:9926', managedSeq: 1, documentId: 'doc-...', hasValidatedPassword: true }
```

4. El usuario pegaba firma.
5. Al ejecutar guardar paginas anotadas, durante `replace-export` reaparecia el prompt de contrasena.
6. El request final salia sin password:

```txt
[DV][reemplazo-paginas][final-request]
{ OriginalPdfPassword: false }
```

7. Backend respondia `400 Validation` porque el PDF original si estaba protegido y el request final no llevaba `OriginalPdfPassword`.

### 12.2 Evidencia tecnica

Logs relevantes:

```txt
[DV][password][retry:ok]
{ documentKey: 'CORRESPO:9926', managedSeq: 1, documentId: 'doc-...', hasValidatedPassword: true }

[DV][firma][replace-export:start]
{ documentId: 'doc-...', pageNumbers: [1], hasAnySignaturePlaced: true }

[DV][password][prompt-open:onDocumentError]
{ documentKey: 'CORRESPO:9926', managedSeq: 1, documentId: 'doc-...', hasValidatedPassword: false, lastAttemptHadPassword: false }

[DV][password][encryption-inspection]
{ documentKey: 'CORRESPO:9926', encrypted: true, contentType: 'application/pdf' }

[DV][reemplazo-paginas][final-request]
{ OriginalPdfPassword: false }
```

Lectura:

- El PDF si estaba cifrado (`encrypted: true`).
- La contrasena si habia sido validada antes (`retry:ok`, `hasValidatedPassword: true`).
- Antes del request final, la memoria local del visor ya habia perdido la password (`hasValidatedPassword: false`).
- Por eso `DocumentosWorkbench` recibia `undefined` desde `visorRef.current?.getOriginalPdfPassword()` y enviaba `OriginalPdfPassword: false`.

### 12.3 Causa raiz

La causa no era backend ni contrasena invalida.

La causa estaba en el ciclo de vida de `EmbedPdfDocumentHost`: el effect que limpia password al cambiar documento dependia de callbacks recibidos desde el componente padre:

```ts
useEffect(() => {
  setPassword(null);
  validatedPdfPasswordRef.current = null;
  onOriginalPdfPasswordChange(null);
  return () => {
    onOriginalPdfPasswordChange(null);
  };
}, [fileUrl, onExportAnnotatedPdfPagesReady, onMarkAnnotatedPagesPersistedReady, onOriginalPdfPasswordChange]);
```

En el padre, esos callbacks se estaban pasando inline:

```tsx
onExportAnnotatedPdfPagesReady={(handler) => {
  exportAnnotatedPdfPagesRef.current = handler;
}}
onMarkAnnotatedPagesPersistedReady={(handler) => {
  markAnnotatedPagesPersistedRef.current = handler;
}}
onOriginalPdfPasswordChange={(password) => {
  originalPdfPasswordRef.current = ...
}}
```

Durante la firma/export/upload hay re-renders normales. En cada render, esas funciones inline cambian de identidad. Como el effect del host las tenia como dependencias, React ejecutaba cleanup y luego re-ejecutaba el effect aunque `fileUrl` no hubiera cambiado. Ese cleanup limpiaba:

- `validatedPdfPasswordRef.current`
- `originalPdfPasswordRef.current`
- estado visual del prompt/password

Por eso la password validada se perdia durante una operacion legitima sobre el mismo documento.

### 12.4 Solucion aplicada

Se estabilizaron los callbacks del padre con `useCallback`, de forma que el effect del host solo limpie password cuando realmente cambia el documento/fuente (`fileUrl`) o se desmonta el host.

Callbacks agregados en `AppVisorEmbedPdf`:

```ts
const handleExportAnnotatedPdfPagesReady = useCallback((handler) => {
  exportAnnotatedPdfPagesRef.current = handler;
}, []);

const handleMarkAnnotatedPagesPersistedReady = useCallback((handler) => {
  markAnnotatedPagesPersistedRef.current = handler;
}, []);

const handleOriginalPdfPasswordChange = useCallback((password: string | null) => {
  originalPdfPasswordRef.current = typeof password === "string" && password.length > 0 ? password : null;
  dvLog("[DV][password][memory]", {
    documentKey: lastLoadIdentityRef.current?.documentKey,
    hasPassword: Boolean(originalPdfPasswordRef.current),
  });
}, []);
```

Y se reemplazaron los callbacks inline por referencias estables:

```tsx
onExportAnnotatedPdfPagesReady={handleExportAnnotatedPdfPagesReady}
onMarkAnnotatedPagesPersistedReady={handleMarkAnnotatedPagesPersistedReady}
onOriginalPdfPasswordChange={handleOriginalPdfPasswordChange}
```

### 12.5 Garantias preservadas

El ajuste preserva:

- la password no se guarda en `localStorage`, `sessionStorage`, IndexedDB, logs, telemetria ni estado persistente;
- la password sigue viviendo solo en memoria volatil;
- la password se limpia al cambiar `fileUrl`, desmontar host, resetear visor o cancelar carga;
- el request final solo incluye `OriginalPdfPassword` si el usuario ya valido una password real;
- no se expone el valor de la password en logs;
- la firma, exportacion de paginas anotadas y upload por chunks no cambian de contrato;
- PDFs sin contrasena siguen enviando `OriginalPdfPassword: false`.

### 12.6 Log de validacion

Se agrego un log seguro:

```txt
[DV][password][memory]
{ documentKey: 'CORRESPO:9926', hasPassword: true }
```

Este log no imprime la contrasena. Solo permite confirmar si el visor conserva o limpia la password en memoria.

Flujo esperado en PDF protegido:

```txt
[DV][password][retry:ok] hasValidatedPassword: true
[DV][password][memory] hasPassword: true
[DV][firma][replace-export:start]
[DV][reemplazo-paginas][final-request] OriginalPdfPassword: true
```

No debe aparecer `OriginalPdfPassword: false` despues de una password validada para el mismo documento.

### 12.7 Pruebas ejecutadas despues del ajuste

Comandos ejecutados:

```powershell
npx.cmd tsc --noEmit --pretty false
npx.cmd vitest run src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx --reporter verbose
npm.cmd run build
```

Resultados:

- TypeScript: OK.
- Vitest visor: OK, 22 tests pasaron.
- Build productivo: OK.

Validacion manual reportada:

- El PDF con contrasena ya no pierde la password durante la firma.
- El request final vuelve a enviar `OriginalPdfPassword: true`.
- El flujo de firma queda funcional.
