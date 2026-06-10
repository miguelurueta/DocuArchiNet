# Jira Context - SCRUMCORE-241

## Summary

MODULO-REUSABLE-DIGITALIZACIONDOCUMENTAL- WORKBENCH-GRAFICO-CAPTURA-03-FE

## Description

> # PROMPT IMPLEMENTACION - Workbench Grafico DigitalizacionDocumental
> # Fase FE-03 - Interfaz de captura, preview, miniaturas y metadata
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## ROL
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> Actua como Arquitecto Frontend senior especialista en:
> 
> - React 19
> - TypeScript estricto
> - UX documental enterprise
> - state orchestration
> - componentizacion reusable
> - accesibilidad
> - integracion con hooks de scanner
> - validacion contractual runtime
> - testing enterprise
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## OBJETIVO
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> Implementar la interfaz grafica principal de `DigitalizacionDocumental`:
> 
> - toolbar de scanner;
> - panel de miniaturas;
> - preview del documento capturado;
> - panel de metadata;
> - footer de acciones;
> - estados visuales completos;
> - soporte modo `crear` y modo `adjuntar`.
> 
> La interfaz debe ser:
> 
> - reusable;
> - desacoplada;
> - accesible;
> - estable;
> - preparada para integracion futura con scanner real y backend.
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## CONTEXTO OBLIGATORIO
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> Depende de:
> 
> ```txt
> PROMPT-01-FE-Modulo-Reusable-DigitalizacionDocumental.md
> PROMPT-02-FE-Dynamsoft-Adapter.md
> ```
> 
> Debe integrarse con componentes UI existentes del repo cuando existan.
> 
> Referencias visuales a revisar antes de implementar:
> 
> ```txt
> src/modules/gestionCorrespondencia/components/documentosWorkbench/
> src/app/Components/UI/
> src/app/Components/UI/AppUpload*
> src/app/Components/UI/AppVisor*
> ```
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## REGLA ARQUITECTONICA OBLIGATORIA
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> El Workbench grafico NO es la fuente de verdad del scanner ni del PDF.
> 
> Debe consumir exclusivamente estado normalizado proveniente de:
> 
> - `useDigitalizacionScanner()`;
> - `useDigitalizacionDocumentalState()`.
> 
> Esto implica:
> 
> - no duplicar estado scanner;
> - no reconstruir paginas;
> - no reconstruir PDF;
> - no almacenar copias inconsistentes.
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## SOURCE OF TRUTH OBLIGATORIA
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> Scanner State:
> 
> ```txt
> useDigitalizacionScanner()
> ```
> 
> Metadata State:
> 
> ```txt
> useDigitalizacionDocumentalState()
> ```
> 
> Context State:
> 
> ```txt
> DigitalizacionContext
> ```
> 
> PROHIBIDO:
> 
> - reconstruir paginas desde thumbnails;
> - reconstruir PDF desde preview;
> - usar object URLs como fuente de verdad;
> - usar estado UI como fuente documental.
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## UBICACION ESPERADA
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> ```txt
> src/modules/digitalizacion/
> ├─ components/
> │  └─ DigitalizacionDocumentalModal/
> ├─ hooks/
> ├─ types/
> ├─ adapters/
> └─ tests/
> ```
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## INTERFAZ GRAFICA OBLIGATORIA
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> Layout:
> 
> ```txt
> DigitalizacionDocumental
> ├─ Header contextual
> │  ├─ modo: Crear documento / Adjuntar digitalizacion
> │  ├─ radicado
> │  ├─ gabinete
> │  └─ documento destino si aplica
> ├─ Toolbar scanner
> │  ├─ selector scanner
> │  ├─ controles: escanear, reintentar, limpiar
> │  ├─ controles paginas: rotar, eliminar
> │  └─ accion primaria
> ├─ Body
> │  ├─ columna miniaturas
> │  ├─ preview central
> │  └─ panel metadata
> └─ Footer
>    ├─ estado operacion
>    ├─ cancelar
>    └─ confirmar
> ```
> 
> Header:
> 
> - modo actual;
> - gabinete;
> - radicado;
> - documento destino.
> 
> Toolbar:
> 
> - selector scanner;
> - escanear;
> - reintentar;
> - limpiar;
> - rotar;
> - eliminar pagina;
> - accion principal.
> 
> Body:
> 
> - miniaturas;
> - preview;
> - metadata.
> 
> Footer:
> 
> - estado operacion;
> - cancelar;
> - confirmar.
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## VALIDACION CONTRACTUAL RUNTIME OBLIGATORIA
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> Antes de habilitar acciones validar:
> 
> Contexto:
> 
> - context existe;
> - modo valido;
> - nombreGabinete valido.
> 
> Scanner:
> 
> - scanner seleccionado cuando aplique;
> - runtime valido.
> 
> Paginas:
> 
> - pages existe;
> - `pages.length > 0`.
> 
> Metadata:
> 
> - metadata obligatoria completa cuando aplique.
> 
> Estado:
> 
> - no operacion incompatible activa;
> - no estado stale.
> 
> Si falla:
> 
> - bloquear accion;
> - error funcional visible;
> - no continuar flujo.
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## REGLA ANTI-STALE OBLIGATORIA
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> Si:
> 
> - cambia context;
> - modal se cierra;
> - scanner se reinicializa.
> 
> Debe:
> 
> - limpiar miniaturas;
> - limpiar preview;
> - limpiar metadata runtime;
> - limpiar errores;
> - invalidar respuestas stale;
> - ignorar operaciones antiguas.
> 
> Nunca debe sobrevivir:
> 
> - paginas anteriores;
> - metadata anterior;
> - object URLs anteriores;
> - errores anteriores.
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## REGLAS UX OBLIGATORIAS
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> - No crear landing page.
> - No usar hero.
> - No usar cards anidadas.
> - No usar textos largos de instrucciones.
> - Mantener interfaz operativa, densa y escaneable.
> - Usar iconos en botones cuando existan.
> - Boton principal cambia segun modo:
>   - `Guardar documento`;
>   - `Adjuntar digitalizacion`.
> - Deshabilitar accion primaria si:
>   - contexto invalido;
>   - no hay paginas;
>   - metadata requerida incompleta;
>   - hay operacion en progreso.
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## ESTADOS VISUALES OBLIGATORIOS
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> Implementar estados:
> 
> ```txt
> contextInvalid
> initializingScanner
> scannerUnavailable
> noScanner
> readyEmpty
> scanning
> pagesCaptured
> generatingPdf
> uploading
> resolvingMetadata
> saving
> success
> error
> ```
> 
> Cada estado debe tener:
> 
> - affordance visual clara;
> - no bloquear toda la pantalla salvo operacion critica;
> - retry cuando aplique;
> - no perder paginas capturadas por errores recuperables.
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## PANEL METADATA
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> Debe soportar:
> 
> - configuracion cargando;
> - lista de chequeo cargando;
> - select de tipologia/lista chequeo;
> - validaciones;
> - errores funcionales;
> - TRD;
> - estado obligatorio;
> - error de unicidad;
> - resumen TRD resuelto:
>   - area;
>   - serie;
>   - subserie;
>   - tipo documental.
> 
> En modo `adjuntar`, metadata puede ser opcional, pero debe mostrar tipologia/documento destino si el contexto la provee.
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## PREVIEW Y MINIATURAS
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> Miniaturas:
> 
> - lista estable;
> - seleccion de pagina;
> - eliminar pagina;
> - rotar pagina;
> - contador paginas.
> 
> Preview:
> 
> - pagina seleccionada;
> - estado vacio si no hay paginas;
> - no usar `AppVisorEmbedPdf` como scanner si no aplica;
> - mantener dimensiones estables;
> - evitar layout shift.
> 
> PROHIBIDO:
> 
> - usar `AppVisorEmbedPdf` como scanner;
> - generar layout shift;
> - reconstruir paginas desde preview.
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## REGLAS DE OBJECT URL OBLIGATORIAS
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> Toda URL generada mediante:
> 
> ```txt
> URL.createObjectURL()
> ```
> 
> debe:
> 
> - registrarse;
> - liberarse mediante `URL.revokeObjectURL()`.
> 
> cuando:
> 
> - se elimina pagina;
> - cambia contexto;
> - se limpia scanner;
> - se desmonta componente.
> 
> PROHIBIDO:
> 
> - persistir object URLs;
> - reutilizar URLs stale.
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## INTERACCION OBLIGATORIA
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> Accion principal:
> 
> ```txt
> Guardar documento
> ```
> 
> o
> 
> ```txt
> Adjuntar digitalizacion
> ```
> 
> Debe permanecer deshabilitada si:
> 
> - contexto invalido;
> - metadata obligatoria incompleta;
> - no existen paginas;
> - operacion activa;
> - error contractual critico.
> 
> Cancelar:
> 
> - limpia estado;
> - respeta contrato FE-01.
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## ACCESIBILIDAD
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> - botones con labels accesibles;
> - foco visible;
> - navegacion por teclado basica;
> - mensajes de error asociados;
> - estados loading anunciables;
> - contraste suficiente.
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## PERFORMANCE
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> - evitar re-render masivo por cada cambio menor;
> - memoizar listas de paginas si aplica;
> - thumbnails con dimensiones estables;
> - limpiar URLs temporales locales con `URL.revokeObjectURL`;
> - no persistir object URLs.
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## PRUEBAS OBLIGATORIAS
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> Tests:
> 
> - render modo crear;
> - render modo adjuntar;
> - contexto invalido;
> - no scanner;
> - paginas capturadas;
> - boton principal disabled sin paginas;
> - metadata requerida bloquea confirmacion;
> - seleccion de miniatura;
> - eliminar pagina;
> - limpiar paginas;
> - error visible;
> - retry visible.
> 
> Validacion contractual runtime:
> 
> - context invalido;
> - nombreGabinete invalido;
> - pages vacias;
> - metadata incompleta.
> 
> Anti-stale:
> 
> - cambio context;
> - cierre modal;
> - respuestas stale ignoradas.
> 
> Object URLs:
> 
> - `revokeObjectURL` ejecutado;
> - URLs no persisten.
> 
> Integracion:
> 
> - hook scanner integrado;
> - hook metadata integrado;
> - callbacks correctos.
> 
> Browser Interaction:
> 
> - seleccion miniatura;
> - cambio preview;
> - limpiar paginas;
> - retry scanner.
> 
> E2E:
> 
> - flujo crear;
> - flujo adjuntar;
> - cambio contexto;
> - reset completo;
> - metadata requerida.
> 
> QT / Calidad:
> 
> - sin errores build;
> - sin warnings TS/lint;
> - sin `any`;
> - sin memory leaks;
> - sin regressions UI.
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## DOCUMENTACION OBLIGATORIA
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> Ruta:
> 
> ```txt
> docs/Architecture/DigitalizacionDocumental/
> ```
> 
> Archivos obligatorios:
> 
> 1. `SCRUMCORE-[XX]-Arquitectura.md`
> 
> Debe incluir:
> 
> - arquitectura visual;
> - diagramas Mermaid;
> - estados;
> - source-of-truth;
> - ownership de estado;
> - riesgos.
> 
> 2. `SCRUMCORE-[XX]-Implementacion-Detallada.md`
> 
> Debe incluir:
> 
> - layout;
> - miniaturas;
> - preview;
> - metadata;
> - validaciones runtime;
> - anti-stale.
> 
> 3. `SCRUM-[XX]-Integracion-BackEnd.md`
> 
> Debe incluir:
> 
> - contratos futuros;
> - metadata;
> - scanner;
> - upload.
> 
> 4. `SCRUM-[XX]-Pruebas.md`
> 
> Debe incluir:
> 
> - unitarias;
> - integracion;
> - browser interaction;
> - E2E;
> - regresion;
> - cobertura.
> 
> 5. `SCRUM-[ID]-Metadata.md`
> 
> Debe incluir:
> 
> - ticket;
> - version;
> - fecha;
> - control cambios;
> - referencias cruzadas.
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## INSTRUCCION FINAL
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> Implementar el Workbench grafico de `DigitalizacionDocumental` como una interfaz documental reusable, accesible y enterprise, basada en source-of-truth explicita, validacion contractual runtime estricta, proteccion anti-stale, ownership correcto de estado y manejo seguro de miniaturas/preview, preparada para integrar scanner, metadata y persistencia en fases posteriores.

## Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: 03-FE, CAPTURA, DIGITALIZACIONDOCUMENTAL, GRAFICO, MODULO, REUTILISABLE, WORKBENCH
