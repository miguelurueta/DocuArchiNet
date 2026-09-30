# Prompt 01 — Corrección del contexto de radicado al adjuntar documentos

Actúa como arquitecto y desarrollador senior especialista en ASP.NET Web Forms, VB.NET, .NET Framework 4.6.1, JavaScript legacy y MySQL. Corrige la regresión de Radicación Simplificada que intenta almacenar un documento con el radicado vacío, sin modificar el comportamiento de los demás consumidores del control compartido de carga.

La solución debe recuperar la semántica comprobada de la implementación legacy: para adjuntar desde el módulo de radicación, el radicado autoritativo procede del registro de estado seleccionado y llega explícitamente al almacenamiento. No debe redescubrirse desde `DAT_ADIC_TAR` antes de consultar la plantilla.

## Objetivo

Conseguir que `ADJUNTARADICACION` almacene el documento utilizando el `consecutivo_radicado` del registro seleccionado en `ra_rad_estados_modulo_radicacion`, incluso cuando la columna dinámica correspondiente de `DAT_ADIC_TAR<ruta>` todavía no exista, sea `NULL` o esté vacía.

La corrección debe quedar aislada de Gestión de respuestas, Workflow seleccionado, Producción documental, ENLASE, SII, Consulta de radicado y cualquier otro evento atendido por `FileUploadHandler_.ashx` o `ClassAlmacenamiento`.

## Diagnóstico confirmado

El error visible es:

```txt
Imposible encontrar el radicado () en registro general, imposible determinar la plantilla de radicación
```

El recorrido defectuoso actual es:

```txt
ADJUNTARADICACION
  -> SolicitaDatosEstructuraEstadoRadicado(id_estado_radicado)
  -> StruRegistroEstado.consecutivo_radicado contiene el valor correcto
  -> PreAlmacenaDocumentosRadicacion(...)
  -> SolicitaDatosCamposIndiceGabinete(...)
  -> SolicitaRadicadoTareaWorkflow(...)
  -> DAT_ADIC_TAR devuelve YES con Radicado=""
  -> SolicitaNombrePlantillaRadicado("")
  -> error antes de alcanzar el fallback agregado después del Select Case
```

La función `SolicitaRadicadoTareaWorkflow(...)` considera exitosas tanto la ausencia de fila como una columna `NULL` y deja el parámetro `ByRef RadicadoTarea` vacío. `SolicitaDatosCamposIndiceGabinete(...)` utiliza inmediatamente ese vacío para consultar la plantilla. Por ello, el fallback ubicado después de esa llamada es inalcanzable cuando ocurre el defecto.

No atribuir este error a caché del navegador ni a una pérdida inicial en JavaScript: se confirmó que IIS sirve los recursos versionados, recibe el POST en el handler correcto y carga el ensamblado compilado. La pérdida funcional ocurre en el backend al volver a resolver el radicado desde la tarea.

## Evidencia legacy obligatoria

Antes de diseñar o modificar código, revisar en modo de solo lectura la implementación de referencia ubicada en:

```txt
D:\imagenesda\GestorDocumental\Desarrollo\GestionDocumental-Docuarchi.net\copia\GestionDocumental-Docuarchi.net\
```

En esa versión, la rama equivalente `ENLACE_RADICADO`:

1. Lee `RA_ID_REGISTRO_RADICADO`.
2. Ejecuta `Solicita_datos_estructura_estado_radicado(...)`.
3. Obtiene `stru_.consecutivo_radicado`.
4. Lo pasa directamente como `registro_radicado` a `Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado(...)`.
5. El almacenamiento asigna `Radicado = registro_radicado` antes de consultar la plantilla.
6. No depende de `DAT_ADIC_TAR` para descubrir la identidad del radicado.

Usar esa implementación como evidencia semántica, no copiarla literalmente ni modificar el repositorio de referencia.

## Fuente autoritativa

Para `ADJUNTARADICACION`, la identidad documental se resuelve exclusivamente mediante:

```txt
id_estado_radicado
  -> ra_rad_estados_modulo_radicacion
  -> consecutivo_radicado
```

El identificador del estado debe validarse en servidor. El radicado enviado por JavaScript puede utilizarse únicamente para detectar inconsistencias, nunca como fuente autoritativa ni como autorización.

El servidor debe rechazar de forma cerrada:

- identificador de estado cero, negativo o inexistente;
- registro no autorizado para el usuario o contexto actual;
- `consecutivo_radicado` vacío en el registro autoritativo;
- diferencia entre el radicado informativo enviado por el cliente y el valor de base de datos;
- tarea, trámite, plantilla o gabinete incompatibles con el registro seleccionado.

## Diseño obligatorio

### 1. Contexto tipado e inmutable

Introducir un contexto específico para la operación, con un nombre coherente con las convenciones reales, equivalente a:

```vb
Public Class ContextoAdjuntoRadicacion
    Public Property IdRegistroEstado As Long
    Public Property Radicado As String
    Public Property IdTareaWorkflow As Long
    Public Property IdTipoTramite As Integer
    Public Property IdPlantilla As Integer
    Public Property NombreGabinete As String
End Class
```

No es obligatorio conservar exactamente todas estas propiedades si el análisis demuestra que alguna no es necesaria, pero `IdRegistroEstado`, `Radicado`, `IdTareaWorkflow` e identidad del destino no pueden viajar como valores ambiguos o volver a obtenerse desde estado web mutable.

### 2. Resolución única en servidor

Crear una operación específica, o una abstracción equivalente, que resuelva el contexto desde `id_estado_radicado`, valide su pertenencia y devuelva el radicado no vacío. Esta operación debe ejecutarse una sola vez por carga lógica y antes de construir índices o consultar la plantilla.

No leer el radicado autoritativo desde:

- `DAT_ADIC_TAR<ruta>`;
- `RA_RADICADO_REGISTRO` como única fuente;
- `DG_RADICADO` como única fuente;
- texto recibido del navegador;
- nombre visible, etiqueta HTML o fila del GridView.

### 3. Separación de responsabilidades

Separar conceptualmente estas operaciones:

```txt
Resolver identidad del radicado
Construir campos e índices del gabinete
Almacenar el documento
```

Crear un método nuevo para construir datos de gabinete con un radicado ya validado. Puede llamarse `ConstruirDatosCamposIndiceGabineteConRadicado` o adoptar la convención real del código.

Ese método debe:

- recibir el radicado como argumento obligatorio;
- rechazar vacío antes de consultar la plantilla;
- no ejecutar `SolicitaRadicadoTareaWorkflow(...)`;
- no sobrescribir el valor recibido;
- construir los campos requeridos para el almacenamiento actual;
- mantener la semántica de plantilla, tipología, gabinete y tarea.

### 4. Adaptación legacy sin ruptura

Mantener intacto el método compartido `SolicitaDatosCamposIndiceGabinete(...)` para los consumidores que todavía resuelven el radicado desde la tarea. Puede delegar en el nuevo constructor después de aplicar su resolución histórica, pero no cambiar su firma, retorno ni semántica desde este cambio.

No agregar otro parámetro opcional primitivo a `UploadSaveFile(...)`, `PreAlmacenaDocumentosRadicacion(...)` o al resolver compartido. Preferir un contexto específico o una sobrecarga inequívoca utilizada exclusivamente por `ADJUNTARADICACION`.

### 5. Flujo objetivo

```txt
FileUploadHandler_.ashx
  -> detectar exactamente ADJUNTARADICACION
  -> resolver ContextoAdjuntoRadicacion en servidor
  -> validar identidad y correspondencia informativa del cliente
  -> prealmacenar con contexto explícito
  -> construir índices con Contexto.Radicado
  -> consultar plantilla con Contexto.Radicado
  -> almacenar documento
  -> devolver la estructura de imagen existente
  -> insertar/actualizar la interfaz mediante el recorrido actual
```

No debe existir una segunda resolución del radicado entre la construcción del contexto y el almacenamiento.

## Rutas canónicas de revisión

Verificar nombres, firmas y dependencias reales antes de modificar:

```txt
generic_control/
├── FileUploadHandler.js
└── FileUploadHandler_.ashx.vb

js/RadicadorSimplificado/
└── Web_form_radicacion_simpilificada.js

RadicadorSimplificado/
└── Web_form_radicacion_simpilificada.aspx

radicador/
├── Class_ra_rad_estados_modulo_radicacion.vb
└── Class_ra_registro_general_radicacion.vb

workflow/
├── ClassAlmacenamiento.vb
└── Class_DAT_ADIC_TAR.vb

Docuarchi/
└── ClassDaGabinete.vb

tests/
├── radicacion-simple-attachment-radicado-fallback.test.cjs
└── bootstrap-table-global-contract.test.cjs
```

Revisar el diff no confirmado existente antes de editar. Parte del intento previo ya agregó parámetros y un fallback tardío; no asumir que ese estado es correcto ni duplicar rutas.

## Restricciones críticas

- No modificar el comportamiento de `GESTION_RESPUESTA`.
- No modificar el comportamiento de `WORKFLOWSELECCION`.
- No modificar el comportamiento de `PRODUCCION`.
- No modificar ENLASE, importación SII ni importación de sellos.
- No cambiar globalmente el contrato de `SolicitaRadicadoTareaWorkflow(...)` en esta corrección.
- No reinterpretar el retorno legacy `YES` para todos sus consumidores.
- No sustituir el radicado de todos los flujos por una variable de sesión.
- No confiar en el radicado enviado por JavaScript para autorizar o almacenar.
- No consultar dos veces el registro de estado sin una razón documentada.
- No resolver el defecto mediante recarga parcial o total, postback, `DataBind` o reinicialización de la lista.
- No reemplazar funciones JavaScript compartidas ni volver a declarar identificadores globales.
- No introducir un feature gate para una corrección de integridad ya aprobada.
- No cambiar el gate `WorkflowCentroTrabajoModernActive` ni sus usuarios o grupos.
- No ejecutar E2E autenticada, carga ni mutaciones externas sin autorización explícita.
- No imprimir o persistir credenciales, cookies, tokens ni cadenas de conexión.

## Tareas atómicas sugeridas

1. Caracterizar con una prueba el fallo actual: `DAT_ADIC_TAR` vacío provoca la consulta de plantilla con radicado vacío antes del fallback.
2. Caracterizar las ramas compartidas de `FileUploadHandler_.ashx.vb` y `UploadSaveFile(...)` para impedir desplazamientos accidentales entre llamadas de firma similar.
3. Introducir el contexto tipado de adjunto de radicación y su resolver autoritativo.
4. Crear el constructor de campos de gabinete que recibe un radicado obligatorio y no lo redescubre.
5. Adaptar exclusivamente `ADJUNTARADICACION` para usar ese recorrido.
6. Retirar del intento previo los parámetros opcionales o fallbacks que queden redundantes, sin alterar cambios del usuario no relacionados.
7. Mantener el adaptador legacy para los demás módulos y demostrar que conserva su comportamiento.
8. Agregar pruebas de valores vacíos, discrepancia cliente-servidor, registro inexistente y pertenencia inválida.
9. Agregar pruebas de regresión de las ramas compartidas y de carga múltiple.
10. Compilar con MSBuild, ejecutar suites focales y documentar resultados y limitaciones.

## Matriz mínima de no regresión

| Evento o capacidad | Fuente del radicado | Resultado esperado |
|---|---|---|
| `ADJUNTARADICACION` | `ra_rad_estados_modulo_radicacion.consecutivo_radicado` | Guarda aunque `DAT_ADIC_TAR` esté vacío. |
| `GESTION_RESPUESTA` | Recorrido existente | Firma y comportamiento sin cambios. |
| `WORKFLOWSELECCION` | Resolución histórica de tarea | Comportamiento sin cambios. |
| `PRODUCCION` | Expediente seleccionado | Comportamiento sin cambios. |
| `ENLACE_RADICADO` | Recorrido propio existente | Comportamiento sin cambios. |
| Integración SII | Resolver de integración | Comportamiento sin cambios. |
| Importación de sellos | Adaptador y proyección propios | Comportamiento sin cambios. |

## Pruebas obligatorias

- Prueba que reproduzca el defecto con tarea válida y radicado vacío en `DAT_ADIC_TAR`.
- Prueba que demuestre que la plantilla se consulta con `StruRegistroEstado.consecutivo_radicado`.
- Prueba negativa con `id_estado_radicado` inexistente.
- Prueba negativa con `consecutivo_radicado` vacío en el registro autoritativo.
- Prueba negativa ante discrepancia entre el valor informativo del navegador y base de datos.
- Prueba de aislamiento que falle si el contexto nuevo aparece en una rama diferente de `ADJUNTARADICACION`.
- Prueba de caracterización de todas las llamadas similares a `UploadSaveFile(...)`; delimitar bloques por sus `If evento_adjunta`, no usar una expresión global que pueda coincidir con otra rama.
- Prueba que demuestre que `SolicitaDatosCamposIndiceGabinete(...)` conserva el recorrido legacy para consumidores existentes.
- Prueba de varios archivos que demuestre que todos conservan el mismo contexto validado sin mezcla con otra operación.
- Suite `bootstrap-table-global-contract.test.cjs` para confirmar que esta corrección no reintroduce la colisión JavaScript anterior.
- Compilación completa de `GestionDocumental-Docuarchi.net.vbproj` con MSBuild.
- `git diff --check` y revisión manual del diff para detectar cambios fuera de alcance.

Las pruebas locales deben ser deterministas y no depender de autenticación, red, SII ni escritura en bases de datos reales. Una prueba estructural no sustituye una prueba ejecutable del constructor o resolver nuevo.

## Criterios de aceptación

- Adjuntar un documento desde Radicación Simplificada no consulta la plantilla con radicado vacío.
- Funciona cuando la tarea todavía no tiene el radicado disponible en `DAT_ADIC_TAR`.
- El radicado usado para plantilla, índices y almacenamiento coincide con el registro de estado seleccionado.
- Una discrepancia o ausencia de identidad falla antes del almacenamiento definitivo.
- El archivo no queda registrado bajo otra tarea, gabinete, plantilla o radicado.
- No se agregan parámetros opcionales ambiguos a contratos compartidos.
- No cambia ninguna rama diferente de `ADJUNTARADICACION`.
- Enlace, SII, Producción, Gestión de respuestas, Workflow e importación de sellos conservan pruebas de regresión aprobadas.
- La compilación termina sin errores y las advertencias preexistentes se distinguen de cualquier advertencia nueva.

## Ruta documental obligatoria

Crear un único paquete documental para el ticket real dentro de:

```txt
Doc/Actualizacion/RadicacionSimplificada/Adjunta/<DOC-ID>-correccion-contexto-radicado-adjunto/
```

Sustituir `<DOC-ID>` por el identificador aprobado. Documentar allí:

- diagnóstico y punto exacto de ruptura;
- comparación con la implementación legacy funcional;
- diseño y responsabilidades del contexto;
- contrato de entrada, errores y validaciones;
- diagrama del flujo anterior y corregido;
- matriz de no regresión;
- archivos modificados;
- comandos y resultados de pruebas;
- limitaciones, evidencia manual saneada y cualquier E2E bloqueada por autorización.

No duplicar documentación en carpetas paralelas ni modificar paquetes archivados para reescribir su historia.

## Entregable final

Entregar código, pruebas y documentación coherentes con la implementación real. El resumen debe indicar expresamente:

1. Dónde se resolvió el radicado autoritativo.
2. Cómo se evitó la segunda resolución desde `DAT_ADIC_TAR`.
3. Qué contratos legacy permanecieron intactos.
4. Qué pruebas demuestran aislamiento y no regresión.
5. Qué validaciones no se ejecutaron por falta de autorización.

## Contexto obligatorio

Antes de implementar, leer completamente:

- `AGENTS.md`;
- este prompt;
- los archivos de las rutas canónicas;
- las pruebas focales existentes;
- la implementación legacy de referencia indicada;
- el diff actual del repositorio.

No implementar por sustitución textual de llamadas repetidas. Delimitar cada modificación por el bloque funcional completo y verificar después que ningún argumento o contexto exclusivo haya quedado conectado a otra rama.

## Criterio de cierre

No declarar la corrección terminada solo porque desaparezca el mensaje visual. Debe demostrarse que la plantilla, los índices y el almacenamiento reciben el mismo radicado autoritativo, que `DAT_ADIC_TAR` vacío deja de bloquear esta capacidad y que los demás módulos conservan su comportamiento.
