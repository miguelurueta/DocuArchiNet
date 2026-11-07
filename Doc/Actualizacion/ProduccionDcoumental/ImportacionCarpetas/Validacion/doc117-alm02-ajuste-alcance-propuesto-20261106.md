# DOC-117 — Ajuste propuesto del alcance del intento ALM-02

Estado: **APROBADO EDITORIALMENTE** por el usuario mediante «aprobado» el 2026-11-06; aplicado al soporte local y a D-04. No concede autenticación, efectos, inyección o instalación. Mantiene los dos documentos/tres solicitudes máximas y la conservación del resultado incierto B. No cambia los requisitos RQ-01–08 ni el almacenamiento productivo. La verificación se registra en `doc117-fijacion-intento-local-20261106.md/.json`.

## Problema comprobado

El helper desactivado fija `IntentoA`/`IntentoB` como constantes de compilación. El coordinador asigna esos IDs mediante `ReservarIntento` dentro de `AlmacenarArchivo`, después de recibir `AvanzarLote`. La API pública no expone una reserva documental separada. El histórico y las reservas existentes devuelven una proyección sin volver a ejecutar el adaptador, por lo que provocar una primera reserva incompleta no permite continuar posteriormente con el escritor.

Por ello no existe actualmente una secuencia soportada que permita obtener el ID exacto, compilarlo y luego efectuar la primera escritura de ese mismo intento. Predecir el autoincremento, reservar por SQL, cambiar la API o reutilizar intentos inciertos no son soluciones admitidas.

Fuente: `CoordinadorImportacionCarpetas.Documento.vb`, `AlmacenarArchivo`: consulta del histórico, `ReservarIntento`, rama `Not reserva.EsNuevaReserva` y llamada al adaptador. `WebServiceImportacionCarpetas.asmx.vb` ofrece `AvanzarLote` y consulta/cierre/reconciliación, sin endpoint de reserva aislada.

## Revisión propuesta

Mantener **compilados y revisados antes de activar**: raíz física del sitio, actor 141/empresa 2/nivel 7, versión 38/gabinete 10, IDs del lote y de las dos raíces/archivos, identidad de expediente por raíz, nombre/tamaño/hash del original, huella del snapshot y huella esperada de cada solicitud documental. No compilar un autoincremento futuro.

Vincular el ID de intento real **una sola vez, antes de iniciar el escritor**, a esa solicitud ya cerrada:

1. Preparar/confirmar el lote, recibir dos originales y crear/materializar dos expedientes con el runtime original, únicamente después de la autorización de esa fase. Todavía no ejecutar `ALMACENAR`. Conservar y verificar sus IDs y generar dos comandos documentales nuevos, cada uno con revisión y clave propias.
2. Calcular las huellas esperadas de solicitud usando la misma estructura/serialización .NET que emplea el coordinador. Verificar con pruebas que un cambio de contexto, lote, archivo, revisión, clave o fase cambia la huella. No imprimir ni persistir secretos o el contexto completo para acreditarlo.
3. Compilar la variante específica y revisar todos esos valores. En la primera autorización del adaptador, comprobar la identidad cerrada del recurso y la huella del intento que acaba de reservar el coordinador. Rechazar verificación/reconciliación, reserva ajena o huella distinta.
4. Crear atómicamente un registro privado por archivo con `FileMode.CreateNew`, y vaciarlo a disco con `Flush(True)`. El registro solo contiene lote/raíz/archivo, ID real del intento, huella de solicitud y versión del protocolo. Debe existir **antes** de delegar en `IniciarAlmacenamientoDocumento` y antes de la llamada legacy.
5. Las autorizaciones posteriores dentro de la misma solicitud deben leer exactamente ese ID y esa huella. Si el registro ya existe con otro intento, falta, está incompleto, no es legible o no coincide con los recursos compilados, detener sin escritor. No borrar registros para permitir otro intento.
6. Mantener el inicio transaccional existente como barrera de escritor único. El registro privado no sustituye la autoridad, la reserva ni su transición atómica. Una carrera de comandos no puede habilitar otro intento por mera igualdad de archivo.
7. Después del primer efecto, escribir el marcador de ventana con la identidad fijada. A delega en el checkpoint y falla al retornar; su replay confirmado se resuelve desde el histórico y no entra al adaptador. B falla antes del checkpoint; mantiene IDs observados y EN_CURSO/público INCIERTO y no se repite, cierra, reconcilia ni repara.

Un fallo del registro anterior al inicio deja una reserva sin escritura y detiene la corrida. Un fallo después de iniciar o de escribir conserva incertidumbre y obliga a consultar; no concede reintento. No introducir un endpoint o bandera de activación de producto.

## Verificación requerida antes de activar

- Ejecutar el helper propuesto con dobles .NET y con el comando real: fijación durable antes del inicio, rechazo de intento/solicitud/archivo ajenos, registro incompleto y carrera. La creación fallida del registro debe impedir al escritor.
- Contrastar la huella calculada contra la producida por el coordinador; no aceptar una aproximación JavaScript a la serialización .NET.
- Conservar ambos escenarios exactos, IDs originales, límites de tres solicitudes y escritor único. No habilitar replay B ni un reparador.
- Actualizar, **solo tras aprobación editorial**, la propuesta ALM-02 y los validadores de candidata/lease para reconocer este protocolo de fijación previa al escritor. El gate actual que exige IDs de intento previos se conserva mientras tanto.
- Construir la candidata cerrada después de disponer de los IDs reales de lote/raíz/archivo y antes de solicitar su activación. Verificar respaldo y guardián de la variante exacta; el ensayo local de restauración ya disponible no sustituye esa revisión.

## Decisión aprobada

Se aprobó editorialmente la fijación atómica del primer intento que coincida con una solicitud previamente cerrada, antes del escritor, en lugar de compilar un ID todavía no asignado. Esta aprobación permite preparar y probar ese soporte; las cargas e instalación requieren después su autorización propia de ambiente, cuentas, recursos y presupuesto.

El helper fija el ID desde el wrapper de `_iniciar`: `CreateNew`/`Flush(True)` y lectura exacta del registro suceden antes de llamar a `IniciarAlmacenamientoDocumento`. La autoridad original se conserva y el wrapper de `_autorizar` exige la fijación desde que empieza esa fase. La comprobación de `EsNuevaReserva` corresponde al constructor del comando real: es un dato transitorio que no se copia en su representación inmutable.
