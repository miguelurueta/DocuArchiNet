# Exploración de modernización de Información de la tarea

## Estado del documento

- Tipo: exploración técnica y funcional.
- Capacidad: `Detalle → Información de la tarea` del Centro de Trabajo Workflow.
- Estado: diagnóstico inicial; no constituye autorización de implementación.
- Evidencia visual: `Grabación 2026-09-03 163123-info.mp4`.
- Duración analizada: 11,73 segundos.
- Resolución: 1626 × 940, 30 fps.

## Objetivo

Comprender el recorrido actual de consulta de información de una tarea, identificar su acoplamiento con ASP.NET Web Forms y las tablas dinámicas de Ruta, y establecer una dirección segura para modernizar la experiencia sin exponer estructura física ni afectar otras opciones del menú `Detalle`.

## Recorrido observado

1. El usuario tiene una tarea seleccionada en el Centro de Trabajo.
2. Abre el menú `Detalle`.
3. Selecciona `Información de la tarea`.
4. El navegador ejecuta el recorrido identificado internamente como `S-DTS`.
5. El servidor consulta los datos adicionales de la tarea seleccionada.
6. Se abre una ventana legacy titulada `Información tarea`.
7. La ventana presenta una lista vertical de nombre físico de columna y valor.
8. El usuario cierra la ventana mediante el botón `×`.

La grabación no demuestra estados sin tarea, tarea inexistente o ajena, ausencia de datos, errores, carga, cambio entre tareas, navegación por teclado, responsive ni campos con contenido extenso.

## Contenido visible en la evidencia

La ventana presenta directamente campos físicos semejantes a:

- `ID_DAT`;
- `INICIO_TAREAS_WORKFLOW_ID_TAREA`;
- `ID_GABINETE`;
- `ID_IMAGEN`;
- `RADICADO`;
- `FECHARADICADO`;
- `BENEFICIARIO`;
- `FECHAVENCIMIENTO`;
- `TRAMITE`;
- `FLUJO_INTERNO_WF`;
- `IDENTIFICACION`;
- `ASUNTO`;
- `FLUJO_TRABAJO_WF`;
- `ESTADO_MODULO_RADICADO`;
- `ESTADO_TRAMITE`;
- campos variables como `SUBTOTAL` y `SUBTOTALES`.

La mezcla confirma que la superficie combina identificadores técnicos, contexto Workflow y datos funcionales configurables de la Ruta sin jerarquía ni política visible de exposición.

## Superficie legacy identificada

| Responsabilidad | Elemento actual |
|---|---|
| Acceso | `workflow/Webworkflow.aspx`, menú `Detalle` |
| Comando | `prevent_tool_menucab(event, this, 'S-DTS')` |
| Intercambio cliente-servidor | `Hidden_menucab` y `Button_tool_menucab` |
| Procesamiento | `Button_tool_menucab_Click` en `workflow/Webworkflow.aspx.vb` |
| Consulta | `Class_DAT_ADIC_TAR.Listar_datos_tarea_workflow` |
| Generación visual | `Class_DAT_ADIC_TAR.Genera_interface_detalle_tarea_workflow` |
| Modal | `Panel_detalle_flujo` y `ModalPopupExtender_edition_detalle_flujo` |
| Actualización parcial | `UpdatePanel_detalle_flujo` |
| Contenido | `Table_detalle_flujo` (`asp:Table`) |
| Redimensionamiento | `auto_zise_popup_detalle_tarea_workflow` en `js/workflow/Webworkflow.js` |
| Fuente de datos | Tabla dinámica `DAT_ADIC_TAR<ruta>` |
| Identidad de Ruta | `Session("WF_RUTAWORKFLOW")` |
| Identidad de tarea | `Session("ID_TAREA_SELECCIONDA")` |

## Flujo técnico actual

```text
Usuario
  │
  ├─ Detalle → Información de la tarea
  ▼
prevent_tool_menucab(..., "S-DTS")
  │
  ├─ Hidden_menucab
  └─ Button_tool_menucab.click()
  ▼
Webworkflow.aspx.vb
  │
  ├─ Lee tarea y sufijo de Ruta desde Session
  ▼
Class_DAT_ADIC_TAR.Listar_datos_tarea_workflow
  │
  ├─ SELECT * FROM DAT_ADIC_TAR<ruta>
  ├─ WHERE tarea = <valor de sesión>
  └─ Convierte todas las columnas en nombre/valor
  ▼
Genera_interface_detalle_tarea_workflow
  │
  ├─ Crea TableRow, TableCell y Label por campo
  └─ Actualiza UpdatePanel
  ▼
ModalPopupExtender + asp:Table
```

## Hallazgos

### Prioridad crítica

#### Exposición indiscriminada de estructura física

La consulta usa `SELECT *` y la interfaz reproduce todas las columnas encontradas. Un cambio de esquema puede modificar la UI sin una decisión funcional y hacer visibles nuevos campos internos o sensibles.

La superficie debe basarse en un contrato explícito o en una configuración de campos publicables, nunca en el conjunto físico completo de columnas.

#### Construcción dinámica de SQL

El nombre de tabla se forma con `Session("WF_RUTAWORKFLOW")` y el identificador de tarea se concatena en la condición. Aunque ambos valores provengan de sesión, no deben considerarse autorización ni entrada segura. La Ruta debe resolverse desde una tarea autorizada y mapearse contra metadatos internos permitidos; la tarea debe parametrizarse.

#### Identidad dependiente de sesión

La tarea consultada proviene de `Session("ID_TAREA_SELECCIONDA")`. Esto crea riesgo de inconsistencia entre pestañas, selección reciente y postbacks. El contrato futuro debe recibir `IdTarea` explícito y revalidarlo contra el contexto autenticado.

#### Ausencia de contrato de exposición

No existe una separación entre:

- campos que todo usuario con acceso puede ver;
- campos funcionales configurados para la Ruta;
- identificadores de soporte restringidos;
- campos que nunca deben exponerse.

Esta política debe definirse antes de construir la nueva interfaz.

### Prioridad alta

#### Presentación acoplada a datos

`Class_DAT_ADIC_TAR` consulta la base de datos y, en otra función de la misma clase, construye `TableRow`, `TableCell` y `Label`. Esto impide probar reglas de exposición sin controles Web Forms y dificulta reutilizar los datos desde una superficie moderna.

#### Errores internos visibles

Las funciones concatenan `ex.Message` en resultados de error que pueden llegar a la interfaz. La solución futura debe devolver códigos y mensajes funcionales saneados y conservar detalles técnicos únicamente en el mecanismo interno aprobado.

#### Etiquetas técnicas y transformación defectuosa

El código intenta ejecutar `columname.Replace("_", " ")`, pero no asigna el resultado. Las cadenas son inmutables, por lo que la transformación no cambia el valor presentado. Aun corrigiendo esa línea, convertir guiones bajos en espacios no produce nombres funcionales confiables.

#### Redimensionamiento frágil

`auto_zise_popup_detalle_tarea_workflow` escribe altura sobre `#Panel_detalle_flujon`, mientras el control declarado es `Panel_detalle_flujo`. El cálculo depende además de medidas manuales del encabezado, pie y contenedor.

### Prioridad media

- La ventana es estrecha para valores largos y desaprovecha el resto del área.
- No agrupa información por significado.
- Los identificadores internos tienen la misma jerarquía que los datos de negocio.
- Los valores vacíos ocupan filas sin aportar información.
- No se distinguen fechas, estados, importes o texto extenso mediante formatos adecuados.
- No existen estados modernos de carga, vacío, error o reintento.
- No se evidencia foco inicial, trampa de foco, restauración, Escape o lectura accesible.
- La opción comparte el despachador genérico `Button_tool_menucab_Click` con numerosos comandos no relacionados.

## Valor funcional de la modernización

La opción sí aporta valor: permite consultar contexto sin abandonar la tarea. El problema no es su existencia sino que hoy funciona como un inspector técnico de una fila de base de datos.

La modernización debe convertirla en un resumen funcional de solo lectura, útil para quien tramita la tarea, manteniendo información técnica únicamente para perfiles y escenarios expresamente aprobados.

## Modelo de información propuesto

### Resumen de negocio

- Radicado.
- Trámite.
- Beneficiario o interesado.
- Identificación, si el permiso y la finalidad lo permiten.
- Asunto.
- Fecha de radicación.
- Fecha de vencimiento.
- Estado funcional.

### Contexto Workflow

- Actividad actual.
- Ruta o Flujo.
- Estado de la tarea.
- Fecha de asignación o inicio, si aplica.
- Identificador de tarea solo cuando sea funcionalmente necesario o el perfil sea de soporte.

### Información adicional de la Ruta

- Únicamente campos marcados como visibles mediante configuración aprobada.
- Etiqueta funcional independiente del nombre físico.
- Tipo de dato y formato conocidos.
- Orden configurado y estable.
- Política explícita para vacíos, datos personales y contenido extenso.

### Campos que no deben aparecer por defecto

- `ID_DAT`.
- `ID_GABINETE`.
- `ID_IMAGEN`.
- indicadores internos como `FLUJO_INTERNO_WF` o `ESTADO_MODULO_RADICADO`.
- nombres de tabla, columna, Ruta física o información de sesión.

## Orientación arquitectónica

```text
Trigger moderno de Detalle
          │
          ▼
Modal/panel accesible de solo lectura
          │
          ▼
Contrato moderno con IdTarea explícito
          │
          ├─ Contexto autenticado
          ├─ Acceso y pertenencia a la tarea
          ├─ Resolución interna de Ruta/Flujo
          └─ Resultado funcional saneado
          │
          ▼
Servicio de información de tarea
          │
          ├─ Resumen estable de Workflow
          └─ Campos configurados y publicables de Ruta
          │
          ▼
Repositorios parametrizados
```

Los repositorios modernos existentes de Workflow pueden orientar la resolución confiable de contexto y tarea, pero no deben reutilizarse a ciegas: su contrato actual responde a transiciones y no define la política de publicación de campos dinámicos.

## Contrato preliminar

Una respuesta podría separar explícitamente:

```text
ResultadoInformacionTarea
├─ Codigo
├─ Mensaje
├─ Contexto
│  ├─ IdTareaVisible
│  ├─ Radicado
│  ├─ Tramite
│  ├─ Actividad
│  ├─ RutaOFlujo
│  └─ Estado
└─ Secciones[]
   ├─ Titulo
   └─ Campos[]
      ├─ ClaveFuncional
      ├─ Etiqueta
      ├─ ValorPresentacion
      ├─ Tipo
      └─ EsSensible
```

`ClaveFuncional` no debe revelar necesariamente el nombre físico. La UI representa `ValorPresentacion` como texto y no interpreta HTML recibido.

## Experiencia visual sugerida

- Conservar `Detalle → Información de la tarea` como acceso reconocible.
- Usar modal o panel lateral de aproximadamente 480–600 px en escritorio y pantalla completa en móvil.
- Mantener tamaño estable y scroll exclusivamente dentro del contenido.
- Presentar un encabezado con radicado, trámite y estado.
- Agrupar campos en `Resumen`, `Workflow` e `Información adicional`.
- Ocultar filas vacías salvo que su ausencia tenga significado funcional.
- Permitir expandir texto largo sin modificar el tamaño general de la ventana.
- Mostrar carga, vacío, error saneado y reintento.
- Cerrar con botón y Escape, con restauración del foco al comando de origen.
- No incluir edición, actualización, copiado masivo ni descarga dentro de este alcance inicial.

## Opciones consideradas

| Opción | Ventaja | Riesgo | Recomendación |
|---|---|---|---|
| Cambiar solo estilos del modal actual | Bajo esfuerzo inicial | Conserva `SELECT *`, sesión, SQL dinámico y exposición técnica | Rechazar. |
| Crear UI moderna sobre la respuesta legacy | Mejora visual rápida | Hereda datos indiscriminados y errores inseguros | No recomendable. |
| Contrato explícito con campos fijos | Máximo control | Puede omitir particularidades válidas de cada Ruta | Usar para el resumen estable. |
| Contrato híbrido: resumen fijo + campos configurados | Controla exposición y conserva extensibilidad | Requiere gobernar configuración y tipos | Recomendado. |

## Decisiones pendientes

1. ¿Qué perfiles pueden abrir Información de la tarea?
2. ¿El acceso depende de que la tarea esté activa, asignada o simplemente sea consultable?
3. ¿Qué campos forman el resumen obligatorio para todas las Rutas?
4. ¿Qué configuración existente define etiquetas, orden y visibilidad de campos adicionales?
5. ¿Los identificadores internos se ocultan siempre o existe un modo de soporte autorizado?
6. ¿Cómo deben enmascararse identificación y otros datos personales?
7. ¿Qué diferencia funcional existe entre estado de tarea, estado de trámite y estado del módulo de radicación?
8. ¿Se deben mostrar importes como `SUBTOTAL` y `SUBTOTALES`, y con qué moneda/formato?
9. ¿La opción debe funcionar sobre tareas terminadas o históricas?
10. ¿Modal centrado o panel lateral es el patrón oficial para consultas breves del Centro de Trabajo?

## Alcance seguro por etapas

1. Aprobar catálogo, visibilidad, etiquetas y política de datos sensibles.
2. Inventariar configuración de campos y consumidores de las funciones legacy.
3. Crear DTO, servicio y repositorios de solo lectura.
4. Agregar pruebas de autorización, aislamiento y no mutación.
5. Integrar la superficie moderna en el menú `Detalle`.
6. Ejecutar pruebas focales, compilación y E2E real autorizada.
7. Retirar el recorrido `S-DTS`, controles y modal legacy solo con referencias activas en cero.
8. Documentar liberación y rollback sin reactivar rutas inseguras.

## Cobertura esperada para un cambio futuro

- Rechazo anónimo.
- Ausencia de tarea seleccionada.
- Tarea inexistente, ajena, inactiva o no consultable.
- Cambio de tarea durante una solicitud y descarte de respuesta obsoleta.
- Resolución segura de Ruta/Flujo.
- Consulta parametrizada y lista blanca de tabla/campos.
- Exclusión de campos técnicos y sensibles.
- Etiquetas, tipos, formatos, orden y valores vacíos.
- Cero, uno y muchos campos adicionales.
- Valores extensos y caracteres especiales representados como texto.
- Error público saneado sin SQL, sesión ni excepciones.
- Evidencia antes/después que confirme lectura sin mutación.
- Carga, vacío, error/reintento, responsive, scroll interno, teclado, Escape y foco.
- Regresión del menú `Detalle`, tabla de tareas, colores, iconos, índice y operaciones vecinas.
- Ausencia de postback, campo oculto y modal legacy en la ruta moderna final.

Toda prueba autenticada futura requiere leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`, además de autorización explícita para ambiente, cuenta y tareas/datos. Las consultas de control deben ser únicamente `SELECT` y la evidencia debe permanecer saneada.

## Riesgos residuales de exploración

- La evidencia cubre una sola Ruta y un único usuario.
- No se ha confirmado el catálogo completo de tablas `DAT_ADIC_TAR<ruta>`.
- No se ha determinado si existen campos binarios, HTML, documentos o valores de gran tamaño.
- No se ha comprobado si otras páginas consumen `Listar_datos_tarea_workflow` o `Genera_interface_detalle_tarea_workflow` de forma dinámica.
- No se ha definido la política organizacional de exposición de datos personales.
- El prototipo visual y los prompts de implementación aún no existen para esta capacidad.

## Conclusión

La modernización es técnicamente válida y funcionalmente útil, pero no debe reducirse a aplicar estilos al modal actual. El principal cambio requerido es pasar de un inspector de `SELECT *` dependiente de sesión a un contrato de información funcional, autorizado y gobernado.

La opción recomendada es un modelo híbrido: resumen estable de Workflow más campos adicionales configurados y explícitamente publicables. Esta capacidad debe gestionarse como un DOC independiente de Autorizaciones y no debe iniciar implementación hasta resolver las decisiones de visibilidad, datos sensibles y alcance histórico.
