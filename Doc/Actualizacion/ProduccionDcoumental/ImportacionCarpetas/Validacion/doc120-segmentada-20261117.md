# DOC-120 — preparación segmentada y continuación masiva

Estado: **IMPLEMENTADO Y VERIFICADO LOCALMENTE; CAPACIDAD REAL NO ACEPTADA**. Base `2d959f58aa0abc4aa02e69f8074b99276bc72788`; lectores compatibles `d5bae4640082f7a53fcd5522891715e7d876b0dc`; fuentes y resultados en [JSON compañero](doc120-segmentada-20261117.json). El commit que contiene este corte identifica la entrega de interfaz. No se modificó `bin/`, IIS, la base de datos ni el corpus de prueba.

## Resultado funcional

Una selección de1.000 archivos se prepara automáticamente por partes de metadatos. El usuario conserva Seleccionar, Revisar e Importar; no confirma cada parte. Los PDF se envían después de **Confirmar e importar**, con una mutación activa. Cada raíz seleccionada produce su propio expediente; las partes internas no producen expedientes adicionales.

Tres tablas RA aditivas separan el borrador, sus partes y resultados. El SQL08 es un script manual **sin aplicar**. Siete operaciones de preparación/consulta conservan el guard de identidad vigente, contexto, permiso y CSRF. El plan esquema2 usa fragmentos y huella global independientes del orden/frontera de partes; el lector esquema1 y el serializador/límite de perfiles permanecen compatibles.

La lectura posterior actualiza recurso y padre, con un resumen de versiones del lote que detecta cambios ajenos. Su SQL aún recorre identidades/revisiones mínimas para calcular ese resumen: menor número de HTTP no acredita por sí solo menor coste de MySQL. Catálogos comunes se reutilizan únicamente dentro de la solicitud/snapshot y se contrastan con una lectura fresca antes/después de validar.

**Vincular carpeta de originales** relaciona pendientes de la raíz elegida por ruta, nombre, tamaño y fecha; separa faltantes, modificados, ambiguos, sobrantes y recibidos. Vincular no envía y **Continuar importación** exige otro clic. No presupone un hash conocido para archivos pendientes. Un borrador tras recarga permite consultar su referencia; si no puede reconstruirse la correspondencia de IDs originales, exige una revisión nueva. La recarga de un lote confirmado conserva ese mismo lote y permite vincular sus pendientes.

## Comprobación y mediciones

| Prueba | Resultado | Alcance |
| --- | --- | --- |
| Regresión seleccionada |199/199; complemento de cliente15/15,14 repetidos: **200 casos únicos** |20 archivos de suites en el corte principal; incluye2 wrappers Framework/DDL, sin sumar sus casos internos. El wrapper Framework se repitió tras ampliar privacidad/equivalencia y pasó2/2; reemplaza su resultado anterior, no añade casos |
| Preparación Framework |1.000 metadatos, partes100/50, mismo hash, valores equivalentes al cálculo legado, caducidad, duplicados y conflicto de partes | Servicio y canonizadores reales; puerto de persistencia en memoria. No prueba bloqueos, commit o rollback MySQL |
| Compatibilidad física | Dos compilaciones completas,0 errores; recorrido **B→A→B**, mismas huellas de plan1 y plan2 de1.000 archivos | `check-doc120-plan-compatibility.cjs` y `ImportacionCarpetasPlanCompatibilityProbe.cs`; archivos sintéticos locales. No aplica DDL ni despliega aplicaciones |
| Coordinador Node |3.001 efectos sintéticos,3.044 lecturas, máximo1 mutación activa;6.665ms e incremento de heap muestreado30.785.680 bytes en la última corrida | Objetivos <=6.045 lecturas,10s y128MiB. Sin red/SQL; memoria muestreada, no pico exacto |
| Chromium1.000 |1 expediente y1.000 documentos sintéticos;3.047 lecturas durante el recorrido, incluidas3 lecturas de presentación adicionales | Backend de navegador simulado, sin HTTP de aplicación. Recepción/creación/almacenamiento no reales |
| Representación |766 nodos máximos; actualización p95=7,3ms; incremento heap39.148.505 bytes | Muestreo local; objetivos2.000 nodos/100ms/128MiB. Anchos390/768/1280px sin desbordamiento del diálogo |
| Recarga y recuperación | Recarga real de página contra servidor simulado;1 recibido omitido y126 originales vinculados/recibidos después de Continuar | Caso127 archivos; el caso1.000 prueba el recorrido completo y el helper prueba modificados/homónimos. No atribuir la recuperación127 a una recuperación real de1.000 |
| Privacidad | Revocación con sesión/token antiguos rechazada antes de leer cuerpo/crear servicio en20 ASMX y upload; cuenta/empresa ajenas, contexto y parentesco comprobados en servicios nuevos | Autoridad/HTTP/SQL-resultados sintéticos. No sustituye segunda cuenta real ni SELECT de control |
| Fuentes protegidas | PerfilImportacionJson, Web.config, SII y almacenamiento compartido sin cambios respecto a la base | No aumenta límites globales, no añade limpieza, jobs, formatos o gates por cuenta |

Entorno: Windows, Node v24.12.0, .NET Framework del repositorio, Chromium del Playwright local. JSON compañero conserva versión exacta, huellas LF de fuentes, logs y métricas. Los avisos de compilación existentes no se presentan como eliminados. Capturas locales: `.tmp/doc120-coordinacion-ui/segmented-1000-{390,768,1280}.png`; se inspeccionó además390px. Datos de ejecución, DLL y capturas permanecen excluidos de Git.

Reproducción principal: `node --test --test-concurrency=1` con la lista de20 suites del JSON. Cliente: `node --test tests/doc120-preparacion-segmentada.test.cjs`. Framework: `node --test tests/doc110-perfil-importacion-behavior.test.cjs`. Compatibilidad: `node tools/e2e/scripts/check-doc120-plan-compatibility.cjs --reader d5bae4640082f7a53fcd5522891715e7d876b0dc`. El auxiliar verifica igualdad de las fuentes compiladas del lector, sustituye solo el codebehind que anuncia la nueva capacidad y compila ambos binarios en `.tmp/`.

## Pendientes y despliegue

**139/148 tareas**,53 escenarios normativos.3.118 conserva una brecha de validación: el nuevo almacenamiento usa la transacción existente y CAS, pero no se ensayó fallo/rollback ni carrera real MySQL. No marcar completo por el doble en memoria.3.126/3.127 requieren carga/continuación real;3.86/3.87/5.2/6.4–6.6 mantienen sus condiciones anteriores. La matriz no convierte estos pendientes en PASS.

Instalar lectores compatibles antes de habilitar productores2; el retorno soportado vuelve a `d5bae464` conservando tablas/datos y lectura de los planes nuevos. Esa versión de retorno conserva la interfaz anterior: no ofrece la preparación automática ni la reselección masiva de esta entrega, y su coordinación antigua puede alcanzar el límite de lecturas en un lote grande. Un binario anterior que desconozca2 no es un retorno válido una vez que haya planes2. La compilación se realizó en `.tmp/`; **SQL08 y DLL no están aplicados al sitio**. El marcado de capacidad técnica no es un gate por cuenta; el JS contra una DLL anterior conserva el recorrido anterior.

La [ventana concreta](doc120-capacidad-plan-real-20261117.md) fija corpus, ambiente, recursos, efectos y presupuestos propuestos para revisar antes de autorizar. Preparar el plan no autoriza ejecutarlo ni adapta por sí solo un runner antiguo. La prueba real y el laboratorio de rollback necesitan sus autorizaciones; no se heredan de las corridas de27 archivos.
