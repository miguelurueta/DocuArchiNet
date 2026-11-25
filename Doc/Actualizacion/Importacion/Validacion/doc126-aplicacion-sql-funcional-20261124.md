# DOC-126: preparación SQL autorizada aplicada

El usuario autorizó aplicar las tres copias SQL preparadas para ejecutar la E2E con su cuenta existente. Se aplicaron exclusivamente SQL23 en DA11, SQL24 en RA12 y SQL25 en DA11, empresa2, actor141 y conexión6, usando recursos actuales del repositorio. Los hashes de las copias se contrastaron con el manifiesto antes de ejecutar; no se reaplicaron scripts históricos DOC125.

Resultado verificado desde conexiones nuevas: dos tablas InnoDB imp_02_doc126_e2e_agregar/reemplazar; dos fuentes/políticas activas revisión2 con historial; tres plantillas activas revisión1, IDs57/58/59; una fila sintética en agregar y dos en reemplazar. El destino CREAR sigue ausente. No se modificaron permisos, Web.config, binarios o datos de los documentos del usuario. SQL23 contenía DDL con commit implícito; SQL24/25 se confirmaron sólo tras LISTO_PARA_COMMIT sin errores. Se comprobaron ausencia inicial de destinos/configuración y ausencia de triggers en las tablas modificadas.

El alcance funcional ya es válido y coincide con DLL/assets vigentes. Usa las plantillas58/59 y los XLSX sintéticos existentes,1000 filas por caso, dos borrados máximos del fixture de reemplazo y cero altas/ediciones de plantilla durante la E2E. Se incluyen27 tablas preexistentes en los controles de huella, incluida la importación manual del usuario. No se confunde esta prueba con exclusividad de permiso, usuario restringido o rollback/carrera.

Se abrió la consola privada para introducir la contraseña de la cuenta autorizada. Los DSN y credenciales de controles se resuelven desde configuración/recursos vigentes y se mantienen sólo en memoria; no se piden nuevas conexiones ni se guardan claves. Se realizaron dos intentos; el segundo terminó con LOGIN_CREDENTIALS_REJECTED antes de crear cargas. No hay aceptación funcional automática. Las referencias de plantilla y huellas se obtuvieron con el repositorio/servicio VB del harness existente, sin inventar hashes.

Estado de tareas:57/62, pendientes6.9/6.10/6.11/7.5/8.8. La creación e importación manual confirmada por el usuario sigue validada. Aplicación SQL acreditada; aceptación automática se registrará con el resultado real del runner. Datos de ejecución, alcances y salida de consola permanecen excluidos de Git.

## Resultado real y corrección del control

El primer intento se detuvo antes de iniciar sesión porque el control compartido DOC124 rechazaba la columna histórica AÑO_FERIADO de imp_02__001_feriados. Se corrigió el helper para admitir letras/números/marcas Unicode y guion bajo, hasta64 caracteres, manteniendo rechazo de delimitadores, expresiones y controles; no se renombró ni modificó la tabla. Las tres lecturas independientes de metadatos se paralelizaron y se añadió progreso por conteos. Regresión afectada38/38 PASS.

El segundo intento completó las huellas iniciales de27 tablas y alcanzó el inicio de sesión real. El servidor rechazó las credenciales para el módulo GESTOR del perfil existente; no se creó una sesión autenticada ni una carga de importación. SELECT posterior acredita cero ejecuciones de las plantillas58/59 y filas iniciales1/2 intactas. La contraseña se descartó al terminar cada consola; no se reutilizan secretos de otro intento ni se modifican contraseñas o permisos para forzar la prueba.

Pendiente confirmar con el usuario que GESTOR/IIS local es el mismo acceso manual y recibir la contraseña vigente sólo en consola privada. SQL23–25 sí están aplicados y verificados; no volver a ejecutarlos. Los intentos fallidos no acreditan agregar/reemplazar, acceso exclusivo o restringido, rollback/carrera ni asistente automatizado. Se conserva la validación manual del usuario. DLL/assets siguen coincidiendo con el scope; no se modificó código del producto.
