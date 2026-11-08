# DOC-118 — contratos internos V2, RV-01

RV-01 queda implementada como tarea6.1/D-11/RQ-11. La continuación «contiunua» posterior a RV-02-A y al siguiente paso RV-01 autoriza esta entrega local. Se añadió la tarea al mapa antes de modificar código. El avance pasa de17/27 a18/28 por esa única tarea nueva completada; las10 tareas reales/finales anteriores siguen pendientes.

`DocumentosAlmacenamientoV2Importacion.vb` define documentos internos de inicio, autorización de invocación y retorno observado, junto a su sobre de fase. El codec reutiliza `PerfilImportacionJson`: JSON cerrado, ASCII canónico, sin valores por defecto que conviertan V1 en V2. El inicio vincula contexto y recurso, solicitud, snapshot, recepción, valores, expediente/gabinete, revisiones e identidad opaca del original. Autorización y retorno incluyen huellas del inicio y de la autorización correspondiente.

Los tipos son internos (`Friend`) y sus transformaciones son puras. Origen SERVIDOR es un discriminador de esquema: cualquier cliente puede copiar un JSON válido; leerlo no acredita procedencia, proveedor, retención, permiso de invocación, retorno durable ni recuperación. No se conectó este contrato a HTTP, coordinador, adaptador o repositorio. La acreditación del proveedor y la verificación transaccional de documento/diario siguen pendientes. No hay despliegue ni DLL/PDB cambiados.

Los errores del codec son saneados. Los documentos no contienen token de reclamación, referencia de staging en claro, rutas, nombre de equipo ni mensajes/códigos arbitrarios del escritor. Las huellas se calculan sobre una selección explícita de metadatos; no incluyen credenciales, cookies o conexiones. Un retorno INCIERTO puede preservar IDs parciales o ausentes y no se presenta como confirmación.

Verificación ejecutada:

- `node --test tests/doc110-perfil-importacion-behavior.test.cjs tests/doc111-importacion-carpetas-client.test.cjs`:18/18, cero fallos. Compila VB y C# con el harness .NET Framework existente, ejecuta su regresión V1/DDL y el cliente con servicios sintéticos.
- Dentro de ese harness:19 casos V2 aprobados. Incluyen roundtrip, V1/69 sintético, versiones/orígenes ausentes o incorrectos, campos requeridos, propiedades extra, duplicados/casing/JSON inválido, referencias/revisiones/hashes, contradicciones de fase, vínculos exactos, IDs incompletos, transiciones puras, ámbito del resultado, copias independientes, saneamiento y ausencia de capacidad/efectos.
- Regresión de herramientas:205/205 en16 archivos. Total Node disjunto de este corte:223/223; los19 casos internos C# no se añaden al contador Node.
- Los14 hashes históricos de fuentes se conservan:12 archivos permanecen idénticos y2 archivos de prueba tienen deltas autorizados con antes/después. Los10 informes históricos y las16 fuentes de la evaluación de recuperación permanecen intactos.

La fixture usa archivos/PDF sintéticos propios y dobles existentes. No se ejecutaron sesiones de aplicación, peticiones de negocio, SELECT ni efectos documentales nuevos. Consumos reales conservados:1276 HTTP,113 SELECT y9 aperturas. Intentos41/53/69 protegidos;69 sigue V1 e inelegible.

RV-02-B es el siguiente paso: integrar una guardia no destructiva en el proveedor real. Este reporte no acredita ese puerto ni amplía el contrato vigente D-07/RQ-07. La evidencia estructurada está en [doc118-contratos-v2-local-20261108.json](doc118-contratos-v2-local-20261108.json).
