# DOC-120 - control final propuesto para lote52

Corte posterior: [control nativo aprobado](doc120-lote52-control-final-resultado-20261120.md). La preparacion y apertura inferiores conservan su fecha/alcance historicos.

El usuario reporta la importacion completada. Ese reporte se conserva; la validacion nativa aun esta pendiente. [Alcance y fuentes](doc120-lote52-control-final-plan-20261120.json). Se reutiliza `inspect-doc120-final-state.cjs --lot 52 --root 73 --completed-readonly`, con SQL de inventario/huellas de QA, puente ODBC/errores y consola privada existentes. La ruta anterior --authorized de cinco SELECT permanece igual.

Nueva ventana:10 SELECT RA/DA y15min para nuevas lecturas;0 HTTP, sesiones de aplicacion o efectos. Solo lote52/raiz73/propietario141/empresa2, expediente696/relacion1120 y gabinete10 del snapshot confirmado. SI antes de acceso y credenciales MySQL exclusivamente SELECT en consola; sin certificado TLS HTTP porque no se abre navegador ni se usa HTTP.

1. Cabecera RA acotada y COMPLETADO, sin detencion/intentos pendientes.
2. Identidad de una raiz y expediente/relacion conocidos.
3. 1000 RECIBIDO/COMPLETADO con valores fijados, bytes previstos/recibidos44859102 y resultados confirmados: documentos/inventarios unicos.
4. Intentos confirmados de recepcion/guardado, uno por archivo; una creacion y ninguno reservado/en curso/incierto.
5. Nombre fisico del gabinete DA desde catalogo.
6. 1000 filas DA por expediente, comparadas contra cada resultado RA; no omitir filas inesperadas mediante filtro de propietario. Verificar empresa/usuario/gabinete/inventario/indice por fila.
7. Relacion RA del expediente con su nivel, propietario y empresa.
8-10. Segunda lectura de archivos, documentos DA y cabecera RA; huellas iguales. No hay transaccion compartida RA/DA ni garantia de snapshot global.

10/10 locales, seis nuevos casos y cuatro de compatibilidad del diagnostico anterior. Rechazan alcances distintos, conteos incompletos, resultados malformados/duplicados, empresa/indice incorrectos, cambio durante lectura e identificador de gabinete inseguro; falta de SI, DSN invalido y fallo ODBC no acceden/reintentan ni conservan secretos. No es evidencia nativa de1000 almacenados.

Preparar/abrir esta ventana no acredita integridad binaria de PDF, visualizacion en arbol, rendimiento de importacion completa ni cierre de DOC120. Las once tareas abiertas anteriores siguen pendientes. No reanuda/importa/crea/borra/reconcilia y no modifica servidor, esquema, perfiles o datos. Ante fallo se conserva STOPPED sin repetir.

ConsolaPID28696 abierta, run7ace0d2582d611840f51c50be3274aec. SI registrado; al corte0 SELECT/0 HTTP/0 efectos y resultado nativo pendiente. No repetir ni relanzar automaticamente esta ventana.
