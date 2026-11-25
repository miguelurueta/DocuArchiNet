# Guía de los prompts de modernización de niveles y expedientes

Estos prompts convierten la [exploración](../exploracion.md) en entregas concretas de implementación. Se preparan por solicitud del usuario del 25 de noviembre de 2026, sobre las alternativas recomendadas. La solicitud de redactarlos no cambia el estado de aprobación de las decisiones D01 a D18.

**Estado de la serie:** los ocho prompts pasan la revisión estructural con perfil `legacy-webforms-vb`; ver [el informe](../revision-prompts.md). Las 18 decisiones continúan pendientes de refinamiento. Se puede comenzar el refinamiento de cada paquete sin cerrar todas las decisiones; modificar código exige resolver las que afecten a esa parte y contar con autorización de implementación. PB00 es un inventario documental.

## Cómo utilizar un prompt

1. Leer [contexto-comun.md](contexto-comun.md), la exploración vigente y el prompt elegido.
2. Refinar las decisiones y dependencias del paquete al abrirlo. Justificar los detalles técnicos dentro del alcance acordado; presentar al usuario cambios de comportamiento, reglas o alcance. Si una decisión cambió de opción o revisión, adaptar el prompt antes de modificar el código afectado.
3. Registrar lo acordado y ejecutar únicamente la parte habilitada del paquete. No convertir recomendaciones en aprobaciones ni pegar toda la serie como una instrucción para implementar sin verificaciones entre entregas.
4. Revisar código, contratos y evidencia al finalizar cada paquete. Registrar lo implementado, lo omitido y las limitaciones.
5. Actualizar la exploración y esta guía si cambia el alcance. La exploración conserva las aprobaciones; esta guía organiza su ejecución.

Para un agente que puede leer el repositorio basta con indicar: «Lee el contexto común y ejecuta el prompt de este archivo». Para otro entorno hay que adjuntar también el contexto común, las decisiones aprobadas y los contratos de los paquetes anteriores. Ningún prompt es independiente de esos insumos.

## Paquetes y orden recomendado

Los códigos PB identifican backend/preparación/verificación; PF identifica integración de frontend. Los números de archivo siguen el orden de trabajo recomendado, no el número del paquete.

| Orden | Paquete y archivo | Entrega | Dependencias técnicas |
|---|---|---|---|
| 1 | [PB00 Inventario y contratos](01-pb00-inventario-contratos.md) | Consumidores, firmas, permisos, rutas y línea base | Código vigente y exploración |
| 2 | [PF01 Presentación del árbol](02-pf01-presentacion-arbol.md) | Tipo visible, decoración compatible y foco | PB00 |
| 3 | [PB01 Identidad y contexto](03-pb01-identidad-contexto.md) | Contexto explícito y autoridad para las rutas acordadas | PB00 |
| 4 | [PB02 Acciones y traslado](04-pb02-acciones-traslado.md) | Operaciones sobre destino comprobado y traslado compatible | PB01 |
| 5 | [PB03 Lecturas y capacidades](05-pb03-lecturas-capacidades.md) | Lecturas acotadas que el cliente realmente necesite | PB01; puede omitirse con justificación |
| 6 | [PF02 Experiencia contextual](06-pf02-experiencia-contextual.md) | Menús, selección confirmada, ruta, filtros y contenido | PF01, PB01, PB02; PB03 si aporta datos necesarios |
| 7 | [PB04 Compatibilidad integral](07-pb04-compatibilidad-integral.md) | Integración de importación, formularios, archivos y restantes accesos | PF02, PB02; PB03 si aplica |
| 8 | [PB05 Verificación y cierre](08-pb05-verificacion-cierre.md) | Evidencia integrada, compilación, aceptación y preparación de despliegue | Paquetes incluidos implementados/verificados; omisiones justificadas |

PF01 puede revisarse como primera mejora de presentación. No resuelve por sí solo los riesgos de sesión, permisos o varias pestañas. PB01/PB02 deben estar disponibles antes de habilitar en PF02 las nuevas acciones que dependen de ellos. Si se comienza por backend, PF01 puede realizarse después de PB01/PB02 sin cambiar esos contratos.

PB03 es opcional: si el contenido directo y la presentación de permisos se resuelven correctamente con datos ya disponibles, se registra la omisión y se adapta PF02. No se construye una API solo para completar la numeración.

## Decisiones que se refinan antes del código afectado

La lista incluye dependencias transitivas relevantes. Es una guía para refinar cada paquete, no una exigencia de aprobar las 18 decisiones antes de comenzar. Leer siempre la redacción y las opciones aceptadas en la exploración.

| Paquete | Decisiones necesarias para la alternativa redactada |
|---|---|
| PB00 | Ninguna para inventario documental; registrar el alcance aún pendiente |
| PF01 | D01, D02, D03, D04, D17 y el alcance de entrega de D18 |
| PB01 | D01, D08, D09, D10, D14 y el alcance de D18 |
| PB02 | Las de PB01; D07, D11 y D12 |
| PB03 | Las de PB01; D05, D06 y D16 según lecturas incluidas |
| PF02 | D01 a D10, D14 a D18; D11/D12 para las acciones respectivas y D13 para accesos de importación |
| PB04 | Decisiones de los paquetes integrados y D13 |
| PB05 | Decisiones de cada función incluida, D16/D17 aplicables y aceptación de D18 |

Una decisión diferida se excluye expresamente de la entrega. Si su ausencia impide cumplir un contrato necesario, esa parte no se modifica; puede continuar el refinamiento independiente. PB00 puede documentar una firma propuesta sin que eso autorice implementarla.

## Reglas para mantener la serie

- Cada prompt referencia el contexto común y delimita sus propios archivos, comportamiento, pruebas y resultado.
- Las recomendaciones de la exploración no son decisiones automáticas. Si se aprueba conservar únicamente la presentación, no se ejecutan como consecuencia los ajustes de backend.
- Cambiar una decisión obliga a revisar los prompts que la consumen y a registrar la incompatibilidad de la versión anterior.
- Las tareas de implementación deben desglosarse en cambios verificables. Un paquete no autoriza una reescritura general para evitar el trabajo de compatibilidad.
- Los prompts no crean un ticket ni cambian el OpenSpec DOC-127 existente. Si se adopta OpenSpec para esta mejora, se crea y vincula su cambio propio cuando corresponda.

## Evidencia y salidas previstas

Las salidas documentales siguientes se crean **durante la ejecución de cada paquete**, no se dan por existentes en esta preparación:

| Paquete | Documento de resultado previsto bajo esta carpeta |
|---|---|
| PB00 | `../implementacion/PB00-contratos-vigentes.md` |
| PF01 | `../implementacion/PF01-presentacion.md` |
| PB01 | `../implementacion/PB01-contexto.md` |
| PB02 | `../implementacion/PB02-acciones.md` |
| PB03 | `../implementacion/PB03-lecturas.md`, incluyendo su omisión si corresponde |
| PF02 | `../implementacion/PF02-experiencia.md` |
| PB04 | `../implementacion/PB04-compatibilidad.md` |
| PB05 | `../implementacion/PB05-verificacion-cierre.md` |

Los documentos anteriores contienen contratos y resultados resumidos sin secretos. Capturas, trazas, datos de ejecución y staging permanecen en ubicaciones locales excluidas de Git. Las evidencias deben indicar comando, fecha, resultado y límite de lo que acreditan.

## Próximo paso

Al abrir el siguiente paquete, iniciar su refinamiento. D01 determina la arquitectura de la alternativa inicial y se resuelve antes de cambiar el control. PB00 puede utilizarse para afinar los contratos mientras continúan las aprobaciones. Pasar el revisor no inicia implementación ni sustituye esos acuerdos.
