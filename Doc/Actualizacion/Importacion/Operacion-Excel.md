# Operación de importación Excel

La opción de importación Excel utiliza los permisos y la conexión del módulo actual. Para cargar archivos se necesita **ADMINISTRAR_CARGAS_IMPORTACION**; el acceso a carpetas, fuentes o proveedores tiene permisos independientes. La instalación se verifica mediante la [guía SQL](Sql/README.md); no se vuelven a aplicar scripts sobre objetos ya compatibles.

## Archivo y plantilla

Se admiten **XLS y XLSX**. La configuración local vigente permite **20.000 filas de datos y 10 MB decimales (10.000.000 bytes)**. El encabezado no cuenta como fila de datos. El servidor valida sus límites efectivos; un cambio de configuración puede invalidar una preparación anterior. El valor predeterminado de filas sin configuración es 10.000. La evidencia actual no certifica todavía confirmación física de 20.000 filas.

Seleccione una plantilla autorizada para el formato, hoja y destino que va a utilizar. Para crearla desde un archivo, use el asistente de análisis, elija la hoja y revise nombres, tipos y clave única antes de guardar. El análisis ayuda a configurar; revise el resultado y cualquier diagnóstico por campo y regla. Una plantilla histórica conserva sus mapeos y revisión. Cada plantilla describe un destino y sus reglas; el archivo se valida de nuevo en cada carga.

## Recorrido guiado

El semáforo muestra **verde: completado; amarillo: siguiente paso; rojo: requiere atención; gris: pendiente**. Los pasos son Plantilla, Recibir archivo, Validar archivo y Confirmar importación.

1. Revise la plantilla, formato, hoja, destino y modo.
2. Pulse **Recibir archivo**. Conserve el ID de carga mostrado para poder consultar su estado.
3. Pulse **Validar archivo**. Revise la muestra, total de filas y todas las incidencias indicadas. Esta preparación no inserta ni reemplaza filas de negocio; sí conserva el archivo y su registro técnico.
4. Revise el destino y pulse **Confirmar carga** cuando esté habilitado. REEMPLAZAR requiere además el consentimiento explícito. Esta acción escribe los datos.
5. Consulte el resultado y las filas confirmadas. Conserve el ID; no vuelva a iniciar otra carga para averiguar el estado de la anterior.

## Modos e integridad

**CREAR** materializa un destino nuevo compatible y sus filas. **AGREGAR** incorpora filas al destino existente y rechaza íntegramente el archivo ante claves repetidas internas o existentes. **REEMPLAZAR** sustituye el contenido del ámbito autorizado en una transacción DML; un fallo con rollback conocido conserva el contenido anterior. No se omiten duplicados ni se actualizan automáticamente filas por coincidencia.

La clave debe estar informada y tener la restricción UNIQUE completa correspondiente. MySQL decide las equivalencias por su tipo y collation; dos textos distintos visualmente pueden coincidir. Revise el código y diagnóstico de la incidencia. No corrija un conflicto cambiando la collation o eliminando registros sin un procedimiento autorizado.

## Interrupciones y recuperación

Cerrar el diálogo, pulsar Escape o cerrar el navegador **no cancela** una confirmación que ya llegó al servidor. No existe pausa por fila ni una continuación desde un porcentaje. La unidad de confirmación es la carga completa.

Si no conoce el resultado, conserve el ID y consulte **la misma carga** con el usuario y empresa autorizados. Una CONFIRMADA es un resultado histórico; volver a confirmar la misma identidad devuelve ese resultado sin otra importación. Si el estado es incierto, no inicie otra carga ni cambie su identidad: consulte primero. Un fallo conocido y una operación todavía en curso requieren acciones distintas; siga el diagnóstico presentado.

CREATE utiliza DDL y puede dejar una estructura propia después de un fallo. La recuperación comprueba marca y esquema; no borre ni adopte una tabla ajena para reintentar. Una modificación de plantilla, archivo, límites o política después de preparar requiere revisar el conflicto y preparar de nuevo cuando corresponda. Los errores de acceso no se resuelven concediendo permisos desde una prueba.

## Tabla como fuente de índices

Importar una tabla no publica automáticamente una fuente o un proveedor. Su uso posterior requiere que la política de la empresa, metadatos y configuración publicada permitan la consulta. El proveedor compara la columna clave configurada con el valor resuelto por el perfil y aplica el filtro empresarial cuando corresponde. Una fila devuelve sus propiedades; ninguna devuelve SIN_COINCIDENCIA y más de una, AMBIGUO. La importación de carpetas y SII conservan sus contratos.

El [EXPLAIN manual](Sql/29-explain-proveedor-doc127.sql) corresponde al proveedor de EGRESOS inspeccionado. Sirve para revisar su plan; no cambia datos ni acredita el plan de otros proveedores.

## Actualización y soporte

Antes de un despliegue autorizado, identificar SHA y hashes del binario, assets y configuración actuales; conservar respaldo fuera de Git. Compilar en `.tmp/` no despliega la DLL de IIS. Una reversión de código/configuración necesita verificar compatibilidad con la base conservada; no borrar tablas, columnas, índices o resultados automáticamente. Los cambios estructurales se entregan en otro SQL manual.

Para reportar un problema, conserve ID, código de incidencia, fecha, modo y versión evaluada. No incluya contraseñas, cookies, cadenas de conexión ni contenido sensible de celdas en tickets o evidencias. El [informe local DOC127](Validacion/doc127-implementacion-local-20261125.md) distingue pruebas locales, aceptación histórica y comprobaciones pendientes.
