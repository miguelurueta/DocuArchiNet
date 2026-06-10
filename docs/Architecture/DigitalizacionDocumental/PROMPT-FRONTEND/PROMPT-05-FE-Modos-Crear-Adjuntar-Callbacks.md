# PROMPT IMPLEMENTACION - Modos Crear/Adjuntar y Callbacks DigitalizacionDocumental
# Fase FE-05 - Orquestacion final de flujos y entrega de resultados

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## ROL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Actua como Arquitecto Frontend senior especialista en:

- React 19
- TypeScript estricto
- state machines UI
- flujos documentales enterprise
- consistencia transaccional UX
- retry seguro
- stale protection
- validacion contractual runtime
- callbacks entre modulos
- testing enterprise

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## OBJETIVO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Implementar la orquestacion final de `DigitalizacionDocumental` para los modos:

- `crear`;
- `adjuntar`.

El modulo debe:

- tomar paginas escaneadas;
- generar PDF;
- subir temporal;
- resolver metadata cuando aplique;
- llamar APIs backend;
- validar contratos runtime;
- entregar resultados consistentes al consumidor;
- soportar retry seguro;
- soportar stale protection;
- mantener consistencia transaccional UX.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CONTEXTO OBLIGATORIO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Depende de:

```txt
PROMPT-01-FE-Modulo-Reusable-DigitalizacionDocumental.md
PROMPT-02-FE-Dynamsoft-Adapter.md
PROMPT-03-FE-Workbench-Grafico-Captura-PDF.md
PROMPT-04-FE-Integracion-APIs-Digitalizacion.md
```

Backend esperado:

```txt
PROMPT-BACKEND/PROMPT-04-BE-CrearDocumentoDigitalizado-API.md
PROMPT-BACKEND/PROMPT-05-BE-ValidarAdjuntarDigitalizacion-API.md
PROMPT-BACKEND/PROMPT-06-BE-AdjuntarDigitalizacionPdf-API.md
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## REGLA ARQUITECTONICA OBLIGATORIA
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

`DigitalizacionDocumental` debe comportarse como una maquina de estados transaccional.

Esto implica:

- cada transicion debe ser explicita;
- no existen estados implicitos;
- no existen callbacks fuera de flujo validado;
- toda operacion debe terminar en:
  - completed;
  - error;
  - cancelled.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## SOURCE OF TRUTH OBLIGATORIA
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

`DigitalizacionContext` es la unica fuente valida para:

- modo;
- nombreGabinete;
- radicado;
- documento destino;
- workflow.

PROHIBIDO:

- reconstruir contexto desde UI;
- reconstruir contexto desde storage;
- reconstruir contexto desde URL.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## FLUJO MODO CREAR
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Secuencia obligatoria:

1. validar contexto;
2. inicializar scanner;
3. capturar paginas;
4. resolver metadata si `requiereMetadata`;
5. generar PDF;
6. upload temporal;
7. llamar crear documento;
8. validar contrato response;
9. construir resultado;
10. ejecutar `onCompleted`;
11. limpiar estado.

Si falla:

- no llamar `onCompleted`;
- mantener paginas capturadas si el error es recuperable;
- permitir retry;
- mostrar error funcional.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## FLUJO MODO ADJUNTAR
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Secuencia obligatoria:

1. validar contexto;
2. validar `idDocumentoDestino`;
3. llamar validacion previa de adjuntar;
4. si bloqueado:
   - abortar flujo;
   - mostrar error funcional;
5. capturar paginas;
6. generar PDF;
7. upload temporal;
8. llamar adjuntar;
9. validar contrato response;
10. construir resultado;
11. ejecutar `onCompleted`;
12. limpiar estado.

Si falla replace/append:

- mantener paginas capturadas;
- permitir retry;
- no cerrar modal automaticamente.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## MAQUINA DE ESTADOS OBLIGATORIA
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Crear orquestador:

```txt
src/modules/digitalizacion/hooks/useDigitalizacionOperationOrchestrator.ts
```

Estados:

```txt
idle
validatingContext
validatingTarget
scanning
generatingPdf
resolvingMetadata
uploading
creatingDocument
attachingDocument
completed
error
cancelled
```

PROHIBIDO:

- estados imposibles;
- transiciones implicitas;
- bypass de validaciones.

Ademas, prohibido permitir:

- dos submits simultaneos;
- cambio de modo durante operacion;
- confirmar sin paginas;
- confirmar con metadata requerida incompleta;
- resultado stale.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## VALIDACION CONTRACTUAL RUNTIME OBLIGATORIA
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Antes de ejecutar APIs:

Validar:

- contexto valido;
- modo valido;
- nombreGabinete valido;
- radicado valido cuando aplique;
- `idDocumentoDestino > 0` cuando adjuntar;
- paginas existentes;
- metadata obligatoria completa.

Despues de cada API:

Validar:

- `success === true`;
- `data != null`;
- ids > 0;
- payload minimo requerido.

Nunca asumir:

- success implicito;
- data existente;
- contrato completo.

Si response invalida:

- abortar flujo;
- error funcional tipado;
- NO continuar pipeline.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## VALIDACION CONTRACTUAL FINAL OBLIGATORIA
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Antes de ejecutar `onCompleted`:

Validar:

Modo crear:

- `accion = documento-creado`;
- `idDocumento > 0`;
- nombreGabinete valido.

Modo adjuntar:

- `accion = documento-adjuntado`;
- `idDocumento > 0`;
- nombreGabinete valido.

Si falta informacion:

- tratar como error contractual;
- NO ejecutar `onCompleted`.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## OWNERSHIP DE RESULTADO OBLIGATORIO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Debe existir estado explicito:

```txt
OperationResultState
```

Responsable de:

- resultado validado;
- payload callback;
- control de completitud.

No usar response backend directamente como callback.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CALLBACKS OBLIGATORIOS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

`onCompleted` debe ejecutarse solo cuando:

- success=true;
- contrato valido;
- contexto vigente;
- contexto no esta stale;
- componente sigue montado;
- resultado completo.

`onError` debe ejecutarse para:

- errores funcionales recuperables;
- bloqueos documentales;
- fallas contractuales API.

`onClose`:

- debe cancelar operaciones pendientes si es posible;
- debe abortar uploads cuando aplique;
- no debe reportar exito.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## REGLA ANTI DOBLE CALLBACK OBLIGATORIA
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

`onCompleted`:

- solo una vez por operacion exitosa;
- responses duplicadas ignoradas;
- responses stale ignoradas.

Nunca ejecutar:

```txt
onCompleted()
onCompleted()
```

para la misma operacion.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## RETRY SEGURO OBLIGATORIO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Permitir retry para:

- generatePdf;
- upload;
- create;
- attach.

No retry automatico para:

- documento firmado;
- documento bloqueado;
- radicado no modificable;
- contexto invalido;
- metadata obligatoria faltante.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## STALE PROTECTION OBLIGATORIA
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Si durante operacion:

- cambia `context`;
- se cierra modal;
- se desmonta componente;
- cambia `idDocumentoDestino`;

Entonces:

- abortar requests;
- abortar uploads;
- ignorar responses stale;
- NO ejecutar callbacks exito.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CANCELACION OBLIGATORIA
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Implementar `AbortController` cuando aplique.

Al cancelar:

- limpiar operacion activa;
- marcar cancelled;
- evitar mutacion de estado posterior.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## UX OBLIGATORIA
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

- accion principal muestra progreso;
- boton cancelar disponible salvo punto critico definido;
- progreso visible;
- errores visibles;
- retry visible;
- paginas no se pierden por fallo de API;
- paginas preservadas en errores recuperables;
- estado success consistente;
- no flicker severo;
- al completar, estado success breve o cierre segun patron local;
- modulo consumidor recibe resultado suficiente para refrescar listados/visor.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## PRUEBAS OBLIGATORIAS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Unitarias:

- modo crear completo;
- modo adjuntar completo;
- bloqueado antes de scanner;
- metadata requerida bloquea;
- retry upload;
- retry create;
- retry attach;
- stale response ignorada;
- doble submit bloqueado.
- doble callback bloqueado.

Validacion contractual:

- success=false;
- data=null;
- idDocumento invalido;
- payload parcial;
- contexto invalido.

Stale Protection:

- context cambia;
- modal cierra;
- upload cancelado;
- callback ignorado.

Integracion:

- scan -> PDF -> upload -> crear -> callback;
- scan -> PDF -> upload -> adjuntar -> callback;
- append bloqueado no inicia upload;
- bloqueo previo evita flujo;
- error API mantiene paginas.

Browser Interaction:

- retry visible;
- cancel visible;
- cambio contexto;
- cierre modal.

E2E:

- abrir desde modulo consumidor mock;
- crear documento;
- adjuntar documento;
- crear documento digitalizado;
- adjuntar a documento existente;
- cancelar sin side effects.
- cancelar;
- retry exitoso;
- stale response ignorada.

QT / Calidad:

- sin errores build;
- sin warnings TS/lint;
- sin `any`;
- sin memory leaks.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## DOCUMENTACION OBLIGATORIA
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Ruta:

```txt
docs/Architecture/DigitalizacionDocumental/
```

Archivos obligatorios:

1. `SCRUMCORE-[XX]-Arquitectura.md`

Debe incluir:

- state machine;
- source-of-truth;
- lifecycle;
- callbacks;
- stale protection.

2. `SCRUMCORE-[XX]-Implementacion-Detallada.md`

Debe incluir:

- orquestador;
- validaciones runtime;
- ownership resultado;
- retry strategy;
- cancelacion.

3. `SCRUM-[XX]-Integracion-BackEnd.md`

Debe incluir:

- contratos crear;
- contratos adjuntar;
- validaciones response;
- matrices FE-BE.

4. `SCRUM-[XX]-Pruebas.md`

Debe incluir:

- unitarias;
- integracion;
- browser interaction;
- E2E;
- regresion;
- cobertura.

5. `SCRUM-[ID]-Metadata.md`

Debe incluir:

- ticket;
- fecha;
- version;
- control cambios;
- referencias cruzadas.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## INSTRUCCION FINAL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Implementar la orquestacion final de `DigitalizacionDocumental` como una maquina de estados transaccional enterprise, garantizando validacion contractual runtime estricta, callbacks seguros, stale protection, retry controlado, cancelacion consistente y entrega confiable de resultados a modulos consumidores sin introducir regresiones.
