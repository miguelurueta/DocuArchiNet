# DOC-120 — ampliación de beneficiario confirmada por el usuario

El usuario confirma expresamente que BENEFICIARIO ya fue ampliado en la tabla y modificado en el perfil, y aporta la grabación `Grabación 2026-11-20 111928-perfil-modificado.mp4`. Se concilian **3.130 y 3.131 como completas**, por declaración del operador y evidencia visual del perfil. No se repiten ALTER, actualización de metadatos, guardado o publicación.

## Fuente y observación

| Dato | Evidencia |
| --- | --- |
| Archivo original | Downloads del usuario; 7.411.088 bytes. Se conserva en su ubicación original, excluido de Git. |
| SHA-256 | `5c2989cc543c4947e482325e4f1721c8cf0928abb8413479cbf4ce11326a64ea` |
| Video | 12,4333 s; 1494×794. Fotogramas de 1, 5, 9 y 12 s inspeccionados en memoria, sin guardar imágenes ni audio. |
| Pantalla | Perfiles de importación, aviso «Operación completada». |
| Regla de beneficiario | Alrededor de 5 s se ve la propiedad del proveedor BENFICIARIO y el campo NOMBRE BENEFICI…; a 9–12 s se ve **Longitud máxima = 255** y Longitud máxima de clave = 255. |
| Ampliación física | Confirmación directa del usuario: ya aplicada en la tabla. Esta revisión no vuelve a consultar ni modificar el esquema. |

El valor visible es **255**, no 250. Se registra el estado aportado por el operador, que supera el objetivo histórico de ampliar los 60 caracteres; no se cambia el perfil para hacerlo coincidir artificialmente con la cifra de la propuesta inicial.

## Conciliación y límites

- **3.130:** aplicación física confirmada por el usuario; deja de ser un pendiente de ejecución o de credenciales. No se atribuye el efecto a los scripts d605ad7e/fa226614, que siguen detenidos antes de escribir.
- **3.131:** modificación del perfil confirmada por el usuario y valor ampliado visible. Se cierra la conciliación manual solicitada en el corte anterior. No se publica otra versión ni se reescriben los snapshots históricos.
- El video no expone una definición SQL completa, el ancho exacto de la columna, charset/default/índices, un ID de versión, ni el evento de publicación. No se presentan esos detalles como una nueva auditoría SQL/API. La procedencia del cambio es la confirmación manual actual y su evidencia visual, conservando los registros anteriores de versiones43/45 con su fecha y alcance.
- La revisión solo lee el video local y los artefactos del cambio: cero HTTP de aplicación, autenticaciones, SELECT de control, escrituras, DLL, DDL o cambios de perfil.

Progreso **264/270**, seis tareas abiertas. La importación funcional de 1.000 documentos ya estaba acreditada; su remanente de métricas/aceptación y el cierre global permanecen separados de esta conciliación. No se reabre ninguna ventana de prueba cerrada.
